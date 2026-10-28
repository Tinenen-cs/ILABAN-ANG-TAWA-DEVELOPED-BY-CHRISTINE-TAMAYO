using System;
using System.IO;
using System.Collections;
using UnityEngine;
namespace Hexbound {
    // Explicit opt-in framebuffer proof. Freezes fighters/UI while ambience keeps real time.
    public sealed class BackgroundMotionProbe : MonoBehaviour {
        string folder;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot(){var args=Environment.GetCommandLineArgs();for(int i=0;i<args.Length-1;i++)if(args[i]=="-ilaban-motion"){var probe=new GameObject("Motion proof only").AddComponent<BackgroundMotionProbe>();probe.folder=args[i+1];DontDestroyOnLoad(probe.gameObject);break;}}
        IEnumerator Start(){
            Directory.CreateDirectory(folder);Application.runInBackground=true;
            while(FindAnyObjectByType<AcademyGame>()==null)yield return null;
            var game=FindAnyObjectByType<AcademyGame>();game.SetLanguage(0);
            for(int scene=0;scene<5;scene++){
                game.enabled=true;
                if(scene<3){game.Selected[0]=0;game.Selected[1]=4;game.Ready[0]=game.Ready[1]=true;game.SelectArena(scene);game.StartMatch();}
                else game.Show(scene==3?AcademyGame.ScreenState.Title:AcademyGame.ScreenState.StageSelect);
                yield return null;game.enabled=false;yield return new WaitForSecondsRealtime(.6f);
                var motion=FindAnyObjectByType<PaintingMotion>();if(motion==null||!motion.isActiveAndEnabled||motion.LayerCount<4)throw new Exception("Motion missing/disabled in scene "+scene);
                float before=motion.Age;yield return Capture(scene+"-a");yield return new WaitForSecondsRealtime(1.5f);yield return Capture(scene+"-b");
                if(motion.Age-before<1.5f)throw new Exception("Ambient clock is not running independently");
            }
            File.WriteAllText(Path.Combine(folder,"PASS.txt"),"Five scenes, stationary fighters/UI, enabled painted motion clock, two native captures 1.5 seconds apart. Pixel thresholds checked by Tools/check_background_motion.py.");Application.Quit(0);
        }
        IEnumerator Capture(string name){yield return new WaitForEndOfFrame();var shot=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(folder,name+".png"),shot.EncodeToPNG());Destroy(shot);}
    }
}
