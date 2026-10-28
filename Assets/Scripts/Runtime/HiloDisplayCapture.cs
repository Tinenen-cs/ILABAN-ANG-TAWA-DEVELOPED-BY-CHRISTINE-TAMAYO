using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
namespace Hexbound {
    // Opt-in evidence only. Real model hits advance deterministically; presentation is frozen between captures.
    public sealed class HiloDisplayCapture : MonoBehaviour {
        string folder;AcademyGame game;MethodInfo draw;
        const BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
        [Serializable] class Row {public int map,attacker,victim,streak,leftFilled,rightFilled;public float victimDizzy;public string file;}
        [Serializable] class Report {public List<Row> stages=new List<Row>();}
        readonly Report report=new Report();
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot(){var args=Environment.GetCommandLineArgs();for(int i=0;i<args.Length-1;i++)if(args[i]=="-ilaban-hilo"){var capture=new GameObject("Opt-in victim Hilo proof").AddComponent<HiloDisplayCapture>();capture.folder=args[i+1];DontDestroyOnLoad(capture.gameObject);break;}}
        void Tick(int count,DuelInput a=default,DuelInput b=default){for(int i=0;i<count;i++)game.Model.Tick(1f/60,a,b);}
        void Hit(int attacker){
            Tick(26);int safety=0;
            while(Mathf.Abs(game.Model.players[0].x-game.Model.players[1].x)>DuelModel.PunchReach-8&&safety++<120){var input=new DuelInput{move=attacker==0?1:-1};Tick(1,attacker==0?input:default,attacker==1?input:default);}
            var punch=new DuelInput{punch=true};Tick(1,attacker==0?punch:default,attacker==1?punch:default);Tick(8);
        }
        IEnumerator Start(){
            Directory.CreateDirectory(folder);Application.runInBackground=true;
            while(FindAnyObjectByType<AcademyGame>()==null)yield return null;
            game=FindAnyObjectByType<AcademyGame>();game.enabled=false;game.SetLanguage(0);Time.timeScale=0;
            draw=typeof(AcademyGame).GetMethod("UpdateFight",Flags);
            for(int map=0;map<3;map++)for(int attacker=0;attacker<2;attacker++){
                game.Selected[0]=0;game.Selected[1]=1;game.Ready[0]=game.Ready[1]=true;game.SelectArena(map);game.StartMatch();
                game.Model.players[0].x=attacker==0?440:620;game.Model.players[1].x=attacker==0?620:800;
                yield return null; // Retire the previous view, including its star objects.
                yield return Capture(map,attacker,0,"empty");
                for(int hits=1;hits<=5;hits++){
                    Hit(attacker);if(game.Model.players[attacker].streak!=hits)throw new Exception("Real hit streak mismatch");
                    if((game.Model.players[1-attacker].dizzyTime>0)!=(hits==5))throw new Exception("Dizzy must belong to victim on hit5");
                    yield return Capture(map,attacker,hits,"hit"+hits);
                }
                Tick(150);if(game.Model.players[1-attacker].dizzyTime!=0)throw new Exception("Victim did not recover");
                yield return Capture(map,attacker,0,"recovered");
                Hit(attacker);Tick(190);yield return Capture(map,attacker,0,"streak-broken");
            }
            File.WriteAllText(Path.Combine(folder,"states.json"),JsonUtility.ToJson(report,true));
            File.WriteAllText(Path.Combine(folder,"PASS.txt"),"48 native captures: all3 maps/both attackers, empty, five actual hits, victim dizzy on fifth, recovery and streak-break reset. DuelModel unchanged; explicit deterministic Tick calls for proof only.");
            Application.Quit(0);
        }
        IEnumerator Capture(int map,int attacker,int expected,string suffix){
            draw.Invoke(game,null);var pips=(Image[][])typeof(AcademyGame).GetField("hiloPips",Flags).GetValue(game);
            var row=new Row{map=map,attacker=attacker,victim=1-attacker,streak=game.Model.players[attacker].streak,victimDizzy=game.Model.players[1-attacker].dizzyTime,file=$"Map{map}-Attacker{attacker+1}-{suffix}.png"};
            for(int i=0;i<5;i++){if(pips[0][i].color.r>.5f)row.leftFilled++;if(pips[1][i].color.r>.5f)row.rightFilled++;}
            if(row.leftFilled!=(attacker==1?expected:0)||row.rightFilled!=(attacker==0?expected:0))throw new Exception("HUD fills on wrong side");
            report.stages.Add(row);yield return new WaitForEndOfFrame();
            var shot=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(folder,row.file),shot.EncodeToPNG());Destroy(shot);
        }
    }
}
