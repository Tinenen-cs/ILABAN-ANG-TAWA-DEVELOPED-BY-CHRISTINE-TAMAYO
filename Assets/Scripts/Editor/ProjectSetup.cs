using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
namespace Hexbound.Editor
{
    public sealed class PixelImport : AssetPostprocessor
    {
        public override uint GetVersion() => 3;
        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/Resources/")) return;
            var t=(TextureImporter)assetImporter;
            t.textureType=TextureImporterType.Default;
            bool smooth=assetPath.Contains("/Ilaban/Maps/")||assetPath.Contains("/Ilaban/Backgrounds/")||assetPath.Contains("/Ilaban/Characters/");t.filterMode=assetPath.StartsWith("Assets/Resources/Ilaban/")&&!smooth?FilterMode.Point:FilterMode.Bilinear;t.maxTextureSize=4096; t.textureCompression=TextureImporterCompression.Uncompressed;
            t.mipmapEnabled=false; t.alphaIsTransparency=true; t.npotScale=TextureImporterNPOTScale.None;
            t.maxTextureSize=4096; t.wrapMode=TextureWrapMode.Clamp;
        }
    }
    [InitializeOnLoad]
    public static class ProjectSetup
    {
        static ProjectSetup() { EditorApplication.delayCall+=OpenStartupScene; }
        static void OpenStartupScene()
        {
            EnsureScene();
            if (Application.isBatchMode || EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (SessionState.GetBool("Hexbound.StartupOpened",false)) return;
            SessionState.SetBool("Hexbound.StartupOpened",true);
            var active=SceneManager.GetActiveScene();
            if (SceneManager.sceneCount==1 && !active.isDirty && string.IsNullOrEmpty(active.path))
                EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
            if (SceneManager.GetActiveScene().path=="Assets/Scenes/Main.unity") ScenePreviewBuilder.FocusGame();
        }
        static void EnsureScene()
        {
            if (!File.Exists("Assets/Scenes/Main.unity")) CreateScene();
        }
        [MenuItem("Hexbound/Open Main Scene")]
        public static void OpenScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EnsureScene(); EditorSceneManager.OpenScene("Assets/Scenes/Main.unity"); ScenePreviewBuilder.FocusGame();
        }
        public static void CreateScene()
        {
            // Additive construction preserves any open scene and unsaved user work.
            var previous=SceneManager.GetActiveScene();
            if (previous.isDirty && string.IsNullOrEmpty(previous.path)) {
                Debug.LogWarning("Save your untitled scene, then use Hexbound/Open Main Scene to finish setup."); return;
            }
            bool replaceEmpty=string.IsNullOrEmpty(previous.path);
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,replaceEmpty?NewSceneMode.Single:NewSceneMode.Additive);
            var camera=new GameObject("Main Camera"); SceneManager.MoveGameObjectToScene(camera,scene);
            camera.tag="MainCamera";
            var cam=camera.AddComponent<Camera>(); cam.orthographic=true; cam.backgroundColor=new Color(.03f,.04f,.08f);
            cam.clearFlags=CameraClearFlags.SolidColor; camera.transform.position=new Vector3(0,0,-10);
            camera.AddComponent<AudioListener>();
            var game=new GameObject("Hexbound Game"); SceneManager.MoveGameObjectToScene(game,scene); game.AddComponent<AcademyGame>();
            Directory.CreateDirectory("Assets/Scenes"); EditorSceneManager.SaveScene(scene,"Assets/Scenes/Main.unity");
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Main.unity",true)};
            if (!replaceEmpty) {
                if (previous.IsValid()) SceneManager.SetActiveScene(previous);
                EditorSceneManager.CloseScene(scene,true);
            }
            PlayerSettings.companyName="Christine Tamayo"; PlayerSettings.productName="ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO";
            PlayerSettings.defaultScreenWidth=1280; PlayerSettings.defaultScreenHeight=720;
            PlayerSettings.runInBackground=false;
            AssetDatabase.SaveAssets();
        }
        public static void ValidateBatch()
        {
            EnsureScene();
            int checks=RuleChecks.Run();
            Require(Resources.Load<UnityEngine.InputSystem.InputActionAsset>("Input/AcademyControls")!=null,"input action asset");
            for(int i=0;i<8;i++) {
                Require(Resources.Load<Texture2D>("Ilaban/Characters/"+Roster.Files[i]+"/Portrait")!=null,"original face "+i);
                Require(Resources.Load<Texture2D>("Ilaban/Characters/"+Roster.Files[i]+"/Body")!=null,"body atlas "+i);
                Require(Resources.Load<Texture2D>("Ilaban/Items/Item_"+i)!=null,"item "+i);
            }
            for(int i=0;i<3;i++) {Require(Resources.Load<Texture2D>("Ilaban/Stages/"+Roster.StageFiles[i])!=null,"map "+i);Require(Resources.Load<AudioClip>("Ilaban/Audio/Music/"+Roster.StageFiles[i])!=null,"map music "+i);}
            Require(Resources.LoadAll<AudioClip>("Ilaban/Audio").Length==30,"30 original clips");
            var bank=new ReactionBank();Require(bank.Mode==LanguageMode.Pormal,"formal build default");
            foreach(var categories in new[]{bank.Data.Pormal,bank.Data.GenZ}) {Require(categories.Length==6,"six reaction categories");foreach(var category in categories)Require(category.lines.Length>=6&&category.lines.Length<=10,"reaction pool size");}
            foreach(var n in new[]{"KalyeDisplay","KalyeBody"}){var font=Resources.Load<TMPro.TMP_FontAsset>("Ilaban/Fonts/"+n);Require(font!=null,"OFL font asset "+n);foreach(char c in "Ññ'[]:!?")Require(font.HasCharacter(c),"Filipino glyph "+c+" in "+n);}
            Debug.Log("ILABAN VALIDATION PASSED: "+checks+" rubric assertions and asset/reaction checks");
        }
        static void Require(bool pass,string name) {if(!pass)throw new System.Exception("Missing "+name);}
    }
}
