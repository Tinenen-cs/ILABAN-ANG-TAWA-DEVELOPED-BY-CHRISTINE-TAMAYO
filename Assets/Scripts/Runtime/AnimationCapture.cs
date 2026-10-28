using System;
using System.Collections;
using System.IO;
using UnityEngine;
namespace Hexbound
{
    // Explicit command-line inspection only. Normal games never enable this harness.
    public sealed class AnimationCapture : MonoBehaviour
    {
        public static bool Active { get; private set; }
        public static int Frame { get; private set; }=-1;
        string directory;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot() {
            var args=Environment.GetCommandLineArgs();
            for(int i=0;i<args.Length-1;i++)if(args[i]=="-ilaban-animation"&&!Active){Active=true;var run=new GameObject("Opt-in animation inspection").AddComponent<AnimationCapture>();run.directory=args[i+1];DontDestroyOnLoad(run.gameObject);}
        }
        IEnumerator Start() {
            Application.runInBackground=true;Directory.CreateDirectory(directory);
            while(FindAnyObjectByType<AcademyGame>()==null)yield return null;
            var game=FindAnyObjectByType<AcademyGame>();game.SetLanguage(0);game.SetReactionStyle(0);Time.timeScale=0;
            for(int character=0;character<8;character++) {
                game.Selected[0]=game.Selected[1]=character;game.Ready[0]=game.Ready[1]=true;game.StartMatch();
                foreach(var action in new[]{"Strike","Hurt"})for(int frame=0;frame<(action=="Strike"?6:4);frame++) {
                    Frame=frame;
                    for(int p=0;p<2;p++){var f=game.Model.players[p];f.x=(p==0?470:810)+(action=="Hurt"?(p==0?-DuelModel.StepBack:DuelModel.StepBack):0);f.pose=action=="Strike"?Pose.Punch:Pose.Hurt;f.stun=action=="Hurt"?1:0;f.actionLength=DuelModel.PunchDuration;f.actionTime=DuelModel.PunchDuration;}
                    yield return Capture(Roster.Files[character]+"/"+action+"_"+frame.ToString("00"));
                }
            }
            Frame=0;
            for(int a=0;a<8;a++)for(int b=a+1;b<8;b++) {
                game.Selected[0]=a;game.Selected[1]=b;game.StartMatch();game.Model.players[0].x=500;game.Model.players[1].x=780;
                yield return Capture("FaceToFace/"+Roster.Files[a]+"-vs-"+Roster.Files[b]);
            }
            File.WriteAllText(Path.Combine(directory,"PASS.txt"),"108 Unity framebuffer captures: six strike and four hurt frames per character in both facings, plus all 28 pair types. Inspection freezes simulation with timeScale=0; normal combat is unchanged.");
            Debug.Log("ILABAN ANIMATION CAPTURE PASSED");Application.Quit(0);
        }
        IEnumerator Capture(string name) {
            yield return new WaitForSecondsRealtime(.22f);yield return new WaitForEndOfFrame();Canvas.ForceUpdateCanvases();
            string path=Path.Combine(directory,name+".png");Directory.CreateDirectory(Path.GetDirectoryName(path));var shot=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(path,shot.EncodeToPNG());Destroy(shot);
        }
    }
}
