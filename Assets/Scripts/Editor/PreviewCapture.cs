using UnityEditor;
using UnityEngine;
namespace Hexbound.Editor
{
    public static class PreviewCapture
    {
        [MenuItem("Ilaban ang Tawa/Inspect Phase 2")]
        public static void Start(){ProjectSetup.OpenScene();EditorApplication.isPlaying=true;}
    }
}
