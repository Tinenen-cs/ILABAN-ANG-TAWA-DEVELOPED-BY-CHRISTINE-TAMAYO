using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
namespace Hexbound.Editor {
    public static class Update4AudioSetup {
        // Editor-only asset authoring. The saved mixer is ordinary native Unity data;
        // no reflection/editor API is used by the shipped game.
        public static void Ensure(){
            const string path="Assets/Resources/Ilaban/Audio/IlabanAudio.mixer";
            var mixer=AssetDatabase.LoadAssetAtPath<AudioMixer>(path);
            if(mixer!=null){RequireGroups(mixer);Debug.Log("UPDATE4 AUDIO MIXER VERIFIED");return;}
            Type type=null;foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()){type=assembly.GetType("UnityEditor.Audio.AudioMixerController");if(type!=null)break;}
            if(type==null)throw new InvalidOperationException("Installed Unity editor has no AudioMixerController");
            const BindingFlags flags=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Static;
            mixer=(AudioMixer)type.GetMethod("CreateMixerControllerAtPath",flags).Invoke(null,new object[]{path});
            var master=type.GetProperty("masterGroup",flags).GetValue(mixer);
            foreach(string name in new[]{"Musika","Tunog"}){
                var group=type.GetMethod("CreateNewGroup",flags).Invoke(mixer,new object[]{name,false});
                type.GetMethod("AddChildToParent",flags).Invoke(mixer,new[]{group,master});
            }
            EditorUtility.SetDirty(mixer);AssetDatabase.SaveAssets();AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
            RequireGroups(AssetDatabase.LoadAssetAtPath<AudioMixer>(path));Debug.Log("UPDATE4 AUDIO MIXER CREATED: Master/Musika and Master/Tunog");
        }
        static void RequireGroups(AudioMixer mixer){foreach(string name in new[]{"Musika","Tunog"})if(mixer.FindMatchingGroups(name).Length!=1)throw new InvalidOperationException("Expected one mixer group "+name);}
    }
}
