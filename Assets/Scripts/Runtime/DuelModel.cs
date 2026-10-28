using System;
using System.Collections.Generic;

namespace Hexbound
{
    // Jump remains a test sentinel only; no code can enter it.
    public enum Pose { Idle, Walk, Punch, Hurt, Jump, Dizzy, Fall, Win, Lose }
    public struct DuelInput { public float move; public bool punch, jump, push, special, power, block; }
    public sealed class Fighter
    {
        public int character, wins, facing, streak;
        public float x, actionTime, actionLength, stun, dizzyTime, streakTime, stepClock;
        public bool attackPending, fallen;
        public Pose pose;
        public Fighter(int character, float x, int facing) { this.character=character; this.x=x; this.facing=facing; }
    }
    public sealed class Burst { public int character; public float x, life=.35f; }
    public struct CombatEvent
    {
        public int player;
        public string kind;
        public CombatEvent(int player, string kind) { this.player=player; this.kind=kind; }
    }
    // Deterministic, presentation-independent rubric rules. No health, vertical movement or timeout.
    public sealed class DuelModel
    {
        public const float StepBack=52, PunchReach=190, BodyGap=112, MoveSpeed=210;
        public const float PunchDuration=.36f, PunchStartup=.12f, DizzyDuration=2, StreakWindow=3;
        // One shared combat-plane geometry for all three paintings. The water opening
        // ends 20px before the inner stone rim of the 220px artwork.
        public const float StageWidth=1280, DrainArtWidth=220, DrainArtY=550, DrainArtHeight=72, DrainInnerRim=20;
        public const float LeftRiftX=80, RightRiftX=1200, HoleInnerEdge=DrainArtWidth-DrainInnerRim;
        public const int KnockdownHits=5;
        public readonly Fighter[] players;
        public readonly List<Burst> bursts=new List<Burst>();
        public readonly List<string> sounds=new List<string>();
        public readonly List<CombatEvent> events=new List<CombatEvent>();
        public int winner=-2, round=1;
        public float elapsed;
        public string reason="";
        readonly int startingArena;
        public int ArenaIndex => startingArena;
        public bool Finished => winner!=-2;
        public bool MatchFinished => Finished && winner>=0 && players[winner].wins>=3;
        public static bool OverRift(float x,float unused=0) => x<=HoleInnerEdge || x>=StageWidth-HoleInnerEdge;
        public DuelModel(int p1,int p2,int arena=0)
        {
            startingArena=Math.Max(0,Math.Min(2,arena));
            players=new[]{new Fighter(p1,525,1),new Fighter(p2,755,-1)};
        }
        public bool BeginNextRound() {
            if(!Finished||MatchFinished)return false;
            for(int i=0;i<2;i++)players[i]=new Fighter(players[i].character,i==0?525:755,i==0?1:-1){wins=players[i].wins};
            winner=-2;round++;elapsed=0;reason="";bursts.Clear();events.Clear();sounds.Clear();return true;
        }
        void Emit(int p,string kind) { events.Add(new CombatEvent(p,kind)); }
        public void Tick(float dt,DuelInput a,DuelInput b)
        {
            sounds.Clear(); events.Clear();
            if(dt<=0 || float.IsNaN(dt) || float.IsInfinity(dt)) return;
            for(int i=bursts.Count-1;i>=0;i--) { bursts[i].life-=dt; if(bursts[i].life<=0) bursts.RemoveAt(i); }
            if(Finished) return;
            // Detect falls before processing controls as well as after hits/movement.
            if(CheckFalls()) return;
            dt=Math.Min(dt,.05f); elapsed+=dt;
            Update(0,a,dt); Update(1,b,dt);
            bool resolveA=Ready(0),resolveB=Ready(1);
            bool hitA=resolveA && InRange(0),hitB=resolveB && InRange(1);
            // Resolve simultaneous strikes from the same snapshot; neither player has priority.
            if(resolveA) players[0].attackPending=false;
            if(resolveB) players[1].attackPending=false;
            if(resolveA && !hitA) ResetStreak(players[0]);
            if(resolveB && !hitB) ResetStreak(players[1]);
            if(hitA) Hit(0,hitB);
            if(hitB) Hit(1,hitA);
            if(hitA && hitB) { ResetStreak(players[0]); ResetStreak(players[1]); }
            CheckFalls();
        }
        void Update(int i,DuelInput input,float dt)
        {
            var p=players[i]; var enemy=players[1-i];
            p.facing=enemy.x>=p.x?1:-1;
            p.streakTime=Math.Max(0,p.streakTime-dt);
            if(p.streakTime==0) p.streak=0;
            p.stun=Math.Max(0,p.stun-dt);
            p.dizzyTime=Math.Max(0,p.dizzyTime-dt);
            p.actionTime=Math.Max(0,p.actionTime-dt);
            if(p.dizzyTime>0) { p.pose=Pose.Dizzy; return; }
            if(p.stun>0) { p.pose=Pose.Hurt; return; }
            if(p.actionTime>0) return;
            p.pose=Pose.Idle;
            if(input.punch) {
                p.pose=Pose.Punch; p.actionLength=p.actionTime=PunchDuration; p.attackPending=true;
                sounds.Add("punch"); Emit(i,"taunt"); return;
            }
            float movement=Math.Max(-1,Math.Min(1,input.move))*MoveSpeed*dt;
            float next=p.x+movement;
            // Body collision stops the mover; it never displaces the other fighter.
            if(p.x<enemy.x && movement>0) next=Math.Min(next,Math.Max(p.x,enemy.x-BodyGap));
            else if(p.x>enemy.x && movement<0) next=Math.Max(next,Math.Min(p.x,enemy.x+BodyGap));
            if(Math.Abs(next-p.x)>.001f) {
                p.pose=Pose.Walk; p.stepClock-=dt;
                if(p.stepClock<=0) { p.stepClock=.26f; sounds.Add("step"); }
            }
            p.x=next;
        }
        bool Ready(int i) { var p=players[i]; return p.attackPending && p.actionTime<=PunchDuration-PunchStartup && p.stun<=0 && p.dizzyTime<=0; }
        bool InRange(int i) { var p=players[i]; var q=players[1-i]; return !q.fallen && q.dizzyTime<=0 && Math.Abs(q.x-p.x)<=PunchReach && (q.x-p.x)*p.facing>=0; }
        static void ResetStreak(Fighter p) { p.streak=0; p.streakTime=0; }
        void Hit(int i,bool trade)
        {
            var p=players[i]; var target=players[1-i];
            int direction=target.x>=p.x?1:-1;
            target.x+=direction*StepBack; // Exactly one step, independent of dt, item and prior motion.
            target.stun=.22f; target.actionTime=0; target.attackPending=false; target.pose=Pose.Hurt;
            ResetStreak(target); p.streak=trade?0:p.streak+1; p.streakTime=trade?0:StreakWindow;
            bursts.Add(new Burst {character=p.character,x=target.x});
            sounds.Add("hit_"+p.character); Emit(1-i,"hurt");
            if(p.streak==KnockdownHits) {
                target.dizzyTime=DizzyDuration; target.pose=Pose.Dizzy; p.streakTime=.85f;
                sounds.Add("dizzy"); Emit(1-i,"dizzy");
            }
        }
        bool CheckFalls()
        {
            bool a=OverRift(players[0].x),b=OverRift(players[1].x);
            if(!a && !b) return false;
            winner=a&&b?-1:a?1:0;
            for(int i=0;i<2;i++) {
                var p=players[i]; p.fallen=i==0?a:b; p.attackPending=false; p.actionTime=0;
                if(p.fallen) { p.pose=Pose.Fall; Emit(i,"lose"); }
                else { p.pose=Pose.Win; p.wins++; Emit(i,"win"); }
            }
            reason=winner<0?"Kapwa nahulog sa imburnal!":"Nahulog sa imburnal si "+RosterName(players[winner==0?1:0].character)+"!";
            sounds.Add("fall"); sounds.Add("splash"); sounds.Add("win"); return true;
        }
        // Presentation supplies the final display name; this model also runs outside Unity.
        static string RosterName(int id) => "["+id+"]";
    }
}
