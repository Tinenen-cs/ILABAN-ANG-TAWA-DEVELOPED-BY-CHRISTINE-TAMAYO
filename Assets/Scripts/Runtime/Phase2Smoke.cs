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
            Directory.CreateDirectory(directory);
            UnityEngine.InputSystem.InputSystem.settings.backgroundBehavior=UnityEngine.InputSystem.InputSettings.BackgroundBehavior.IgnoreFocus;
            yield return new WaitForSecondsRealtime(3.7f);yield return Capture("00-opening");
            while(FindAnyObjectByType<AcademyGame>()==null)yield return null;
            var game=FindAnyObjectByType<AcademyGame>();yield return new WaitForSecondsRealtime(.5f);
            if(game.ReactionLanguage!=LanguageMode.Pormal)throw new Exception("Non-formal default");
            yield return Capture("01-title");
            game.Show(AcademyGame.ScreenState.Select);yield return Capture("02-characters");
            game.Ready[0]=true;game.Ready[1]=false;game.Show(AcademyGame.ScreenState.StageSelect);game.StartMatch();
            if(game.ScreenNow==AcademyGame.ScreenState.Fight)throw new Exception("Started without both confirmations");
            for(int map=0;map<3;map++)for(int time=0;time<3;time++) {
                game.SelectArena(map);game.SetTimeOfDay(time);yield return Capture("03-stage-"+map+"-"+time);
                game.Ready[1]=true;game.StartMatch();yield return Capture("04-match-"+map+"-"+time);
            }
            game.SelectArena(0);game.SetTimeOfDay(2);game.Ready[0]=game.Ready[1]=true;game.StartMatch();
            var pad=UnityEngine.InputSystem.InputSystem.AddDevice<UnityEngine.InputSystem.Gamepad>();
            float deadline=Time.realtimeSinceStartup+45,nextPunch=0;bool dizzyCaptured=false;
            while(!game.Model.Finished && Time.realtimeSinceStartup<deadline) {
                var a=game.Model.players[0];var rival=game.Model.players[1];
                float move=rival.x-a.x>90?1:0;
                var state=new UnityEngine.InputSystem.LowLevel.GamepadState {leftStick=new Vector2(move,0)};
                if(move==0 && Time.realtimeSinceStartup>=nextPunch){state=state.WithButton(UnityEngine.InputSystem.LowLevel.GamepadButton.West);nextPunch=Time.realtimeSinceStartup+.48f;}
                UnityEngine.InputSystem.InputSystem.QueueStateEvent(pad,state);yield return null;
                if(!dizzyCaptured&&rival.dizzyTime>0){dizzyCaptured=true;yield return Capture("05-dizzy");}
            }
            UnityEngine.InputSystem.InputSystem.QueueStateEvent(pad,new UnityEngine.InputSystem.LowLevel.GamepadState());yield return null;
            if(!game.Model.Finished||game.Model.winner!=0)throw new Exception("Simulated input match did not end with manhole win");
            yield return Capture("06-result");UnityEngine.InputSystem.InputSystem.RemoveDevice(pad);
            game.StartMatch();typeof(AcademyGame).GetProperty("Paused").GetSetMethod(true).Invoke(game,new object[]{true});yield return Capture("07-pause");
            game.Show(AcademyGame.ScreenState.Controls);yield return Capture("08-controls");
            File.WriteAllText(Path.Combine(directory,"PASS.txt"),"Portable smoke passed at "+Screen.width+"x"+Screen.height+". Pormal default; readiness gate; all 9 map/time choices; actual simulated gamepad movement/punches to manhole; dizzy and results; pause; controls. Physical input/listening remain manual.");
            Debug.Log("ILABAN PORTABLE SMOKE PASSED "+Screen.width+"x"+Screen.height);Application.Quit(0);
        }
        IEnumerator Capture(string name) {
            yield return new WaitForSecondsRealtime(.22f);yield return new WaitForEndOfFrame();Canvas.ForceUpdateCanvases();
            var textDump=new System.Text.StringBuilder();
            // Every active user-facing Text must fit vertically and horizontally; report suspect labels for review.
            foreach(var t in FindObjectsByType<TMP_Text>())if(t.isActiveAndEnabled && !string.IsNullOrEmpty(t.text)) {
                t.ForceMeshUpdate();textDump.AppendLine(t.text.Replace("\n"," | "));foreach(char c in t.text)if(!char.IsControl(c)&&!t.font.HasCharacter(c))Debug.LogError("MISSING GLYPH: "+(int)c+" / "+t.text);float height=t.preferredHeight;
                if(t.isTextOverflowing)Debug.LogWarning("LAYOUT TEXT OVERFLOW: "+name+" / "+t.text+" / "+height+" > "+t.rectTransform.rect.height);
            }
            File.WriteAllText(Path.Combine(directory,name+"-ui.txt"),textDump.ToString());
            var image=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(directory,name+".png"),image.EncodeToPNG());Destroy(image);
        }
    }
}
