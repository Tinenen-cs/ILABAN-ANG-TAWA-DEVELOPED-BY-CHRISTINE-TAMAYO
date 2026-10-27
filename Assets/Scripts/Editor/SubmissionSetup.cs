using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Hexbound.Editor
{
    // Keep the original scene, scripts and resource paths intact during migration.
    public static class SubmissionSetup
    {
        public const string ProductName = "ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO";

        [MenuItem("Ilaban ang Tawa/Apply Project Identity")]
        public static void ApplyIdentity()
        {
            PlayerSettings.companyName = "Christine Tamayo";
            PlayerSettings.productName = ProductName;
            AssetDatabase.SaveAssets();
            Debug.Log("ILABAN PROJECT IDENTITY SAVED");
        }

        public static void ValidateBaseline()
        {
            ApplyIdentity();
            ProjectSetup.ValidateBatch();
            Debug.Log("ILABAN BASELINE VALIDATION PASSED on Unity " + Application.unityVersion);
        }

        [MenuItem("Ilaban ang Tawa/Build Windows x64")]
        public static void BuildWindows()
        {
            ApplyIdentity();
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64))
                throw new InvalidOperationException("Install Windows Build Support in Unity Hub first.");
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            var directory = Path.Combine("Builds", "Windows");
            Directory.CreateDirectory(directory);
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Main.unity" },
                locationPathName = Path.Combine(directory, ProductName + ".exe"),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Windows build failed: " + report.summary.result);
            Debug.Log("ILABAN WINDOWS BUILD PASSED: " + report.summary.outputPath);
        }
    }
}
