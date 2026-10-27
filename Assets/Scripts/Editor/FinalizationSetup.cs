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
            // Smooth OFL pair (student request): Lilita One for display, Nunito Bold for body. Body is the display font's fallback.
            var body=FontAsset("KalyeBody","Assets/Fonts/Nunito-Bold.ttf",.16f);
            var display=FontAsset("KalyeDisplay","Assets/Fonts/LilitaOne-Regular.ttf",.2f);
            if(display.fallbackFontAssetTable==null)display.fallbackFontAssetTable=new System.Collections.Generic.List<TMP_FontAsset>();
            if(!display.fallbackFontAssetTable.Contains(body)){display.fallbackFontAssetTable.Add(body);EditorUtility.SetDirty(display);}
            TMP_Settings.defaultFontAsset=body;EditorUtility.SetDirty(settings);
            MakeScene("Pambungad",true);MakeScene("Pamagat",false);
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Pambungad.unity",true),new EditorBuildSettingsScene("Assets/Scenes/Pamagat.unity",true),new EditorBuildSettingsScene("Assets/Scenes/Main.unity",true)};
            PlayerSettings.productName=SubmissionSetup.ProductName;PlayerSettings.companyName="Christine Tamayo";
            AssetDatabase.SaveAssets();AssetDatabase.Refresh();
            Debug.Log("ILABAN FINALIZATION ASSETS PREPARED: static font, opening and title scenes");
        }
        static TMP_FontAsset FontAsset(string name,string source,float outline)
        {
            string path="Assets/Resources/Ilaban/Fonts/"+name+".asset";
            var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if(font!=null)return font;
            var ttf=AssetDatabase.LoadAssetAtPath<Font>(source);if(ttf==null)throw new Exception("Missing font source "+source);
            font=TMP_FontAsset.CreateFontAsset(ttf,64,9,GlyphRenderMode.SDFAA,2048,2048,AtlasPopulationMode.Dynamic,false);
            font.name=name;
            string characters="";for(int i=32;i<127;i++)characters+=(char)i;characters+="\u00D1\u00F1\u00C1\u00E1\u00C9\u00E9\u00CD\u00ED\u00D3\u00F3\u00DA\u00FA\u2018\u2019\u201C\u201D\u2026\u2013\u2014\u2022\u00D7";
            font.TryAddCharacters(characters,out string missing);
            foreach(char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789\u00D1\u00F1'-,.!?:[]()") if(!font.HasCharacter(c))throw new Exception(name+" missing required glyph "+c);
            font.atlasPopulationMode=AtlasPopulationMode.Static;
            AssetDatabase.CreateAsset(font,path);
            foreach(var texture in font.atlasTextures){texture.filterMode=FilterMode.Bilinear;AssetDatabase.AddObjectToAsset(texture,font);}
            // Shared readability style: dark outline plus a soft drop shadow (underlay) for bright and dark maps.
            var m=font.material;m.name=name+" outlined";
            m.SetFloat(ShaderUtilities.ID_OutlineWidth,outline);m.SetColor(ShaderUtilities.ID_OutlineColor,new Color(.05f,.04f,.06f,1));m.EnableKeyword("OUTLINE_ON");
            m.EnableKeyword("UNDERLAY_ON");m.SetColor(ShaderUtilities.ID_UnderlayColor,new Color(0,0,0,.6f));m.SetFloat(ShaderUtilities.ID_UnderlayOffsetX,.6f);m.SetFloat(ShaderUtilities.ID_UnderlayOffsetY,-.8f);m.SetFloat(ShaderUtilities.ID_UnderlaySoftness,.4f);
            AssetDatabase.AddObjectToAsset(m,font);EditorUtility.SetDirty(font);
            return font;
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
