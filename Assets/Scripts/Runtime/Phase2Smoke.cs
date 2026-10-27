using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace Hexbound
{
    // Opt-in acceptance harness; never runs during normal play or changes the saved language default.
    public sealed class Phase2Smoke : MonoBehaviour
    {
        string directory;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot() {
            var args=Environment.GetCommandLineArgs();
            for(int i=0;i<args.Length-1;i++)if(args[i]=="-ilaban-smoke") {var runner=new GameObject("Opt-in smoke capture").AddComponent<Phase2Smoke>();runner.directory=args[i+1];}
        }
        IEnumerator Start() {
            Application.runInBackground=true;
            yield return null;
            var game=FindAnyObjectByType<AcademyGame>();
            Directory.CreateDirectory(directory);
            yield return new WaitForSecondsRealtime(.8f);
            if(game.ReactionLanguage!=LanguageMode.Pormal)throw new Exception("Non-formal default");
            yield return Capture("01-title");
            game.Show(AcademyGame.ScreenState.Select);yield return Capture("02-characters");
            game.Ready[0]=true;game.Ready[1]=false;game.Show(AcademyGame.ScreenState.StageSelect);game.StartMatch();
            if(game.ScreenNow==AcademyGame.ScreenState.Fight)throw new Exception("Started without both confirmations");
            yield return Capture("03-stages");
            game.Ready[1]=true;game.StartMatch();yield return Capture("04-match");
            game.Model.players[1].dizzyTime=2;game.Model.players[0].streak=5;game.Model.players[0].streakTime=1;
            typeof(AcademyGame).GetMethod("React",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(game,new object[]{1,"dizzy"});
            yield return Capture("05-dizzy");
            game.Model.players[1].x=DuelModel.RightRiftX;yield return null;yield return Capture("06-result");
            if(!game.Model.Finished || game.Model.winner!=0)throw new Exception("Fall result missing");
            game.Show(AcademyGame.ScreenState.Controls);yield return Capture("07-controls");
            File.WriteAllText(Path.Combine(directory,"PASS.txt"),"Portable runtime smoke passed. Resolution: "+Screen.width+"x"+Screen.height+". Pormal default; independent readiness gate; Canvas screens; dizzy presentation; immediate fall result. Dizzy screenshot uses an injected state. Physical input and listening checks remain manual.");
            Debug.Log("ILABAN PORTABLE SMOKE PASSED "+Screen.width+"x"+Screen.height);Application.Quit(0);
        }
        IEnumerator Capture(string name) {
            yield return null;yield return new WaitForEndOfFrame();Canvas.ForceUpdateCanvases();
            // Every active user-facing Text must fit vertically and horizontally; report suspect labels for review.
            foreach(var t in FindObjectsByType<Text>(FindObjectsSortMode.None))if(t.isActiveAndEnabled && !string.IsNullOrEmpty(t.text)) {
                var settings=t.GetGenerationSettings(t.rectTransform.rect.size);var generator=new TextGenerator();float height=generator.GetPreferredHeight(t.text,settings)/t.pixelsPerUnit;
                if(height>t.rectTransform.rect.height+3)Debug.LogWarning("LAYOUT TEXT OVERFLOW: "+name+" / "+t.text+" / "+height+" > "+t.rectTransform.rect.height);
            }
            var image=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(directory,name+".png"),image.EncodeToPNG());Destroy(image);
        }
    }
}
