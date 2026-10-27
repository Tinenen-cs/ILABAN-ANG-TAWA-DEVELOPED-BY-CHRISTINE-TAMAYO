using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
namespace Hexbound.Editor
{
    public static class FinalizationSetup
    {
        public static void Prepare()
        {
            var settings=Resources.Load<TMP_Settings>("TMP Settings");
            if(settings==null){settings=ScriptableObject.CreateInstance<TMP_Settings>();AssetDatabase.CreateAsset(settings,"Assets/Resources/TMP Settings.asset");AssetDatabase.SaveAssets();AssetDatabase.Refresh();}
            Directory.CreateDirectory("Assets/Resources/Ilaban/Fonts");
            string path="Assets/Resources/Ilaban/Fonts/StreetPixel.asset";
            var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if(font==null) {
                var source=Resources.Load<Font>("UI/Tiny5");
                font=TMP_FontAsset.CreateFontAsset(source,32,6,GlyphRenderMode.SDFAA,1024,1024,AtlasPopulationMode.Dynamic,false);
                font.name="StreetPixel";
                string characters="";for(int i=32;i<127;i++)characters+=(char)i;characters+="Ññ";
                if(!font.TryAddCharacters(characters,out string missing))throw new Exception("Missing required font glyphs: "+missing);
                font.atlasPopulationMode=AtlasPopulationMode.Static;
                AssetDatabase.CreateAsset(font,path);
                foreach(var texture in font.atlasTextures){texture.filterMode=FilterMode.Point;AssetDatabase.AddObjectToAsset(texture,font);}
                font.material.name="StreetPixel outlined";font.material.SetFloat(ShaderUtilities.ID_OutlineWidth,.08f);font.material.SetColor(ShaderUtilities.ID_OutlineColor,new Color(.03f,.05f,.07f,1));font.material.EnableKeyword("OUTLINE_ON");
                AssetDatabase.AddObjectToAsset(font.material,font);EditorUtility.SetDirty(font);
            }
            TMP_Settings.defaultFontAsset=font;EditorUtility.SetDirty(settings);
            MakeScene("Pambungad",true);MakeScene("Pamagat",false);
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Pambungad.unity",true),new EditorBuildSettingsScene("Assets/Scenes/Pamagat.unity",true),new EditorBuildSettingsScene("Assets/Scenes/Main.unity",true)};
            PlayerSettings.productName=SubmissionSetup.ProductName;PlayerSettings.companyName="Christine Tamayo";
            AssetDatabase.SaveAssets();AssetDatabase.Refresh();
            Debug.Log("ILABAN FINALIZATION ASSETS PREPARED: static font, opening and title scenes");
        }
        static void MakeScene(string name,bool opening) {
            string path="Assets/Scenes/"+name+".unity";
            if(File.Exists(path))return;
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var camera=new GameObject("Main Camera",typeof(Camera),typeof(AudioListener));camera.tag="MainCamera";camera.transform.position=new Vector3(0,0,-10);camera.GetComponent<Camera>().backgroundColor=new Color(.04f,.08f,.12f);camera.GetComponent<Camera>().clearFlags=CameraClearFlags.SolidColor;
            var game=new GameObject(opening?"Pambungad":"Ilaban ang Tawa");if(opening)game.AddComponent<OpeningSequence>();else game.AddComponent<AcademyGame>();
            EditorSceneManager.SaveScene(scene,path);
        }
        public static void ValidateAndBuild(){Prepare();SubmissionSetup.ValidateAndBuild();}
    }
}
