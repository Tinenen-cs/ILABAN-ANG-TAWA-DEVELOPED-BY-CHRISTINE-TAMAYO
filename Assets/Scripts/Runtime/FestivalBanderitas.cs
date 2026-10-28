using UnityEngine;
using UnityEngine.UI;
namespace Hexbound {
    // Separate painted pieces, one rope, top-edge pivots. No mesh clipping or flat flags.
    public sealed class FestivalBanderitas : MonoBehaviour {
        readonly RectTransform[] pennants=new RectTransform[14];
        public static FestivalBanderitas Create(Transform parent, Rect area) {
            var go=new GameObject("Pintadong banderitas",typeof(RectTransform));go.transform.SetParent(parent,false);
            var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(area.x,-area.y);r.sizeDelta=area.size;
            var result=go.AddComponent<FestivalBanderitas>();result.Build(area.width,area.height);return result;
        }
        void Build(float width,float height) {
            void Piece(string asset,Rect area,bool pivotTop,out RectTransform rect) {
                var g=new GameObject(asset,typeof(RectTransform),typeof(CanvasRenderer),typeof(RawImage));g.transform.SetParent(transform,false);
                rect=(RectTransform)g.transform;rect.anchorMin=rect.anchorMax=new Vector2(0,1);rect.pivot=pivotTop?new Vector2(.5f,1):new Vector2(0,1);rect.anchoredPosition=new Vector2(area.x,-area.y);rect.sizeDelta=area.size;
                var raw=g.GetComponent<RawImage>();raw.texture=Resources.Load<Texture2D>("Ilaban/UI/Festival/"+asset);raw.raycastTarget=false;
            }
            Piece("Rope",new Rect(0,0,width,height*.53f),false,out _);
            for(int i=0;i<pennants.Length;i++) {
                float u=(i+.65f)/pennants.Length;
                // Matches the two valleys of the painted rope, with a raised central knot.
                float sag=.07f+.37f*Mathf.Sin(Mathf.Repeat(u*2,1)*Mathf.PI);
                float h=height*.56f,w=h*.70f;
                Piece("Flag"+(i%6),new Rect(u*width,sag*height,w,h),true,out pennants[i]);
            }
        }
        void Update(){for(int i=0;i<pennants.Length;i++)pennants[i].localRotation=Quaternion.Euler(0,0,Mathf.Sin(Time.unscaledTime*1.25f+i*.53f)*3f);}
    }
}
