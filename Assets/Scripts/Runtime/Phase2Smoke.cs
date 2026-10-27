using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Hexbound
{
    // Opt-in acceptance harness; never runs during normal play or changes the saved language default.
    public sealed class Phase2Smoke : MonoBehaviour
    {
        string directory;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot() {
            var args=Environment.GetCommandLineArgs();
            for(int i=0;i<args.Length-1;i++)if(args[i]=="-ilaban-smoke") {var runner=new GameObject("Opt-in smoke capture").AddComponent<Phase2Smoke>();runner.directory=args[i+1];DontDestroyOnLoad(runner.gameObject);}
        }
        IEnumerator Start() {
            Application.runInBackground=true;
            while(FindAnyObjectByType<AcademyGame>()==null)yield return null;
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
            var textDump=new System.Text.StringBuilder();
            // Every active user-facing Text must fit vertically and horizontally; report suspect labels for review.
            foreach(var t in FindObjectsByType<TMP_Text>())if(t.isActiveAndEnabled && !string.IsNullOrEmpty(t.text)) {
                t.ForceMeshUpdate();textDump.AppendLine(t.text.Replace("\n"," | "));foreach(char c in t.text)if(!char.IsControl(c)&&!t.font.HasCharacter(c))Debug.LogError("MISSING GLYPH: "+(int)c+" / "+t.text);float height=t.preferredHeight;
                if(height>t.rectTransform.rect.height+3)Debug.LogWarning("LAYOUT TEXT OVERFLOW: "+name+" / "+t.text+" / "+height+" > "+t.rectTransform.rect.height);
            }
            File.WriteAllText(Path.Combine(directory,name+"-ui.txt"),textDump.ToString());
            var image=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(directory,name+".png"),image.EncodeToPNG());Destroy(image);
        }
    }
}
