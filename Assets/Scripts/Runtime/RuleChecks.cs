using System;
namespace Hexbound
{
    public static class RuleChecks
    {
        static int count;
        static void Check(bool pass,string message){if(!pass)throw new Exception("RULE FAIL: "+message);count++;}
        static void Step(DuelModel m,int frames,DuelInput a=default,DuelInput b=default){for(int i=0;i<frames;i++)m.Tick(1f/60,a,b);}
        static DuelModel Close(int id=0){var m=new DuelModel(id,7-id);m.players[0].x=500;m.players[1].x=580;return m;}
        static void Punch(DuelModel m,int player=0){m.Tick(1f/60,player==0?new DuelInput{punch=true}:default,player==1?new DuelInput{punch=true}:default);Step(m,25);}
        public static int Run()
        {
            count=0;
            for(int id=0;id<8;id++) {
                var m=Close(id);float x=m.players[1].x;Punch(m);
                Check(Math.Abs(m.players[1].x-x-DuelModel.StepBack)<.001f,"exact step for item "+id);
                Check(m.players[0].streak==1,"attacker owns streak "+id);
                Check(m.players[1].streak==0,"victim does not gain streak "+id);
                Step(m,60);Check(Math.Abs(m.players[1].x-x-DuelModel.StepBack)<.001f,"no residual knockback "+id);
                m=Close(id);float start=m.players[0].x;Punch(m,1);Check(Math.Abs(start-m.players[0].x-DuelModel.StepBack)<.001f,"mirrored step "+id);
            }
            var d=Close();Step(d,180,new DuelInput{jump=true,push=true,special=true,power=true,block=true});
            Check(d.players[0].pose==Pose.Idle&&d.players[1].x==580,"all forbidden controls ignored");
            Check(typeof(Fighter).GetField("health")==null,"no health field");
            d=Close();for(int i=0;i<5;i++){d.players[0].x=500;d.players[1].x=580;Punch(d);if(i<4)Check(d.players[1].dizzyTime==0,"not dizzy before fifth");}
            Check(d.players[1].dizzyTime>0 && d.players[0].streak==5,"fifth consecutive hit makes victim dizzy");
            float locked=d.players[1].x;Step(d,20,default,new DuelInput{move=1,punch=true,jump=true});Check(d.players[1].x==locked,"dizzy locks controls");
            Step(d,140);Check(d.players[1].dizzyTime==0&&d.players[0].streak==0,"defined recovery and fifth-dot reset");
            d=Close();Punch(d);d.players[0].x=500;d.players[1].x=580;Punch(d,1);Check(d.players[0].streak==0,"attacker being hit resets streak");
            d=Close();Punch(d);d.players[1].x=900;Punch(d);Check(d.players[0].streak==0,"miss resets streak");
            d=Close();Punch(d);Step(d,190);Check(d.players[0].streak==0,"timeout resets streak");
            d=Close();d.Tick(1f/60,new DuelInput{punch=true},new DuelInput{punch=true});Step(d,25);Check(d.players[0].x==448&&d.players[1].x==632,"simultaneous equal strikes");Check(d.players[0].streak==0&&d.players[1].streak==0,"trade breaks both streaks");
            d=Close();d.players[0].streak=d.players[1].streak=4;d.players[0].streakTime=d.players[1].streakTime=3;d.Tick(1f/60,new DuelInput{punch=true},new DuelInput{punch=true});Step(d,25);Check(d.players[0].dizzyTime==0&&d.players[1].dizzyTime==0,"fifth-hit trade does not award dizzy");
            d=Close();Step(d,120,new DuelInput{move=1});Check(d.players[1].x==580&&d.players[0].x<=520,"walking does not shove rival");
            for(int side=0;side<2;side++)for(int edge=0;edge<2;edge++) {
                d=new DuelModel(0,1);d.players[side].x=edge==0?DuelModel.HoleInnerEdge:1280-DuelModel.HoleInnerEdge;d.Tick(.016f,default,default);
                Check(d.Finished&&d.winner==1-side&&d.players[side].fallen,"immediate fall side "+side+" edge "+edge);
                float pos=d.players[0].x;Step(d,40,new DuelInput{move=1,punch=true});Check(d.players[0].x==pos,"round ends immediately");
            }
            d=new DuelModel(0,1);Step(d,6600);Check(!d.Finished,"no timeout result");
            return count;
        }
    }
}
