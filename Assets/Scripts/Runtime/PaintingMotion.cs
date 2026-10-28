using System;
using UnityEngine;
using UnityEngine.UI;
namespace Hexbound {
    public sealed class PaintingMotion : MonoBehaviour {
        [Serializable] public class Layer {public string id,kind,textureResource;public float x,y,width,height;}
        [Serializable] public class Layout {public Layer[] layers;}
        public int LayerCount {get;private set;}
        public float Age {get;private set;}
        RawImage[] images;Layer[] data;
        public void Configure(string painting) {
            var asset=Resources.Load<TextAsset>("Ilaban/Motion/"+painting+"/Layout");
            if(asset==null){enabled=false;return;}
            data=JsonUtility.FromJson<Layout>(asset.text).layers;LayerCount=data.Length;images=new RawImage[LayerCount];
            for(int i=0;i<data.Length;i++) {
                var layer=data[i];var g=new GameObject("Pintadong "+layer.kind,typeof(RectTransform),typeof(CanvasRenderer),typeof(RawImage));g.transform.SetParent(transform,false);
                var rect=(RectTransform)g.transform;rect.anchorMin=new Vector2(layer.x,1-layer.y-layer.height);rect.anchorMax=new Vector2(layer.x+layer.width,1-layer.y);rect.offsetMin=rect.offsetMax=Vector2.zero;
                var image=g.GetComponent<RawImage>();image.texture=Resources.Load<Texture2D>(string.IsNullOrEmpty(layer.textureResource)?"Ilaban/Motion/"+painting+"/"+layer.id:layer.textureResource);image.raycastTarget=false;images[i]=image;
                if(image.texture==null)throw new InvalidOperationException("Missing painted motion layer "+painting+"/"+layer.id);
            }
        }
        void Update() {
            if(images==null)return;Age+=Time.unscaledDeltaTime;
            for(int i=0;i<images.Length;i++) {
                var image=images[i];var r=image.rectTransform;float phase=Age*(data[i].kind=="flag"?1.15f:.85f)+i*.7f;
                if(data[i].kind=="leaf"||data[i].kind=="flag")r.localRotation=Quaternion.Euler(0,0,Mathf.Sin(phase)*(data[i].kind=="leaf"?1.0f:1.2f));
                else if(data[i].kind=="cloud"){r.anchoredPosition=new Vector2(Mathf.Sin(Age*.18f)*4,0);image.color=new Color(1,1,1,.65f);}
                else {float warm=1.20f+Mathf.Sin(phase)*.08f;image.color=new Color(warm,warm*.96f,1,.55f);}
            }
        }
    }
}
