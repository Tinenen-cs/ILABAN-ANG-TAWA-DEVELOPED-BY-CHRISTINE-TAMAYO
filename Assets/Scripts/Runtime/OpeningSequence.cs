using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
namespace Hexbound
{
    public sealed class OpeningSequence : MonoBehaviour
    {
        Canvas canvas;
        RectTransform content,flags,jeep;
        RawImage logo,logoGlow,logoShine;
        RectTransform logoPlate;
        Image fade;
        RawImage street,jeepImage;
        RawImage[] layers;
        AudioSource sound;
        float elapsed;
        bool enginePlayed,stingerPlayed;
        bool english;
        public float Elapsed => elapsed;
        void Awake()
        {
            english=false;foreach(var arg in Environment.GetCommandLineArgs())if(arg=="-ilaban-smoke"){english=false;break;}else english=PlayerPrefs.GetInt("Ilaban.Language",0)==1;
            Application.targetFrameRate=60;
            if(FindAnyObjectByType<AudioListener>()==null)gameObject.AddComponent<AudioListener>();
            AudioListener.volume=PlayerPrefs.GetFloat("Ilaban.Master",.8f);sound=gameObject.AddComponent<AudioSource>();sound.volume=PlayerPrefs.GetFloat("Ilaban.Sound",.7f);
            var g=new GameObject("Pambungad Canvas",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler));g.transform.SetParent(transform,false);canvas=g.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            var scale=g.GetComponent<CanvasScaler>();scale.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scale.referenceResolution=new Vector2(1280,720);scale.matchWidthOrHeight=.5f;
            content=R(g.transform,new Rect(0,0,1280,720));content.anchorMin=content.anchorMax=new Vector2(.5f,.5f);content.pivot=new Vector2(.5f,.5f);content.anchoredPosition=Vector2.zero;
            layers=new RawImage[5];string[] layerNames={"Sky","Skyline","Midground","Wires","Street"};
            for(int i=0;i<5;i++)layers[i]=Art(content,new Rect(-200,0,1680,720),"Backgrounds/Pambungad/"+layerNames[i]);
            street=layers[4];
            var shade=R(content,new Rect(0,0,1280,720)).gameObject.AddComponent<Image>();shade.color=new Color(.02f,.06f,.1f,.35f);
            flags=(RectTransform)FestivalBanderitas.Create(content,new Rect(0,-160,1280,144)).transform;
            jeepImage=Art(content,new Rect(-380,386,360,250),"UI/JeepneyFrames");jeep=jeepImage.rectTransform;jeepImage.uvRect=new Rect(0,.15f,.25f,.57f);
            logoPlate=R(content,new Rect(90,165,1100,174));logoPlate.name="Pintadong pambungad";
            logoPlate.pivot=new Vector2(.5f,.5f);logoPlate.anchoredPosition=new Vector2(640,-252);
            var texture=Resources.Load<Texture2D>("Ilaban/UI/HeadingLogo");
            float logoWidth=Mathf.Min(1100,174f*texture.width/texture.height),logoHeight=logoWidth*texture.height/texture.width;
            var area=new Rect((1100-logoWidth)/2,(174-logoHeight)/2,logoWidth,logoHeight);
            logoGlow=Art(logoPlate,area,"UI/HeadingLogo");logoGlow.color=new Color(1,.75f,.3f,.18f);logoGlow.rectTransform.localScale=Vector3.one*1.015f;
            logo=Art(logoPlate,area,"UI/HeadingLogo");logo.name="ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO";
            logoShine=Art(logoPlate,area,"UI/HeadingLogo");logoShine.color=new Color(1,1,.8f,.4f);
            logoPlate.gameObject.SetActive(false);
            Text(content,new Rect(200,666,880,29),english?"Press any button to skip":"Pindutin ang anumang pindutan upang laktawan",22);
            fade=R(content,new Rect(0,0,1280,720)).gameObject.AddComponent<Image>();fade.color=Color.black;
            Cue("opening_bell");Cue("banderitas");
        }
        RectTransform R(Transform parent,Rect area){var g=new GameObject("Pambungad",typeof(RectTransform));g.transform.SetParent(parent,false);var r=(RectTransform)g.transform;r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(area.x,-area.y);r.sizeDelta=area.size;return r;}
        RawImage Art(Transform parent,Rect area,string resource){var image=R(parent,area).gameObject.AddComponent<RawImage>();image.texture=Resources.Load<Texture2D>("Ilaban/"+resource);image.raycastTarget=false;return image;}
        TMP_Text Text(Transform parent,Rect area,string value,int size){var t=R(parent,area).gameObject.AddComponent<TextMeshProUGUI>();t.font=Resources.Load<TMP_FontAsset>("Ilaban/Fonts/KalyeDisplay");t.text=value;t.fontSize=size;t.color=new Color32(247,226,181,255);t.alignment=TextAlignmentOptions.Midline;t.raycastTarget=false;return t;}
        void Cue(string name){var clip=Resources.Load<AudioClip>("Ilaban/Audio/SFX/"+name);if(clip!=null)sound.PlayOneShot(clip);}
        void Update()
        {
            // Asset-loading stalls must not skip the whole opening on its first frame.
            elapsed+=Mathf.Min(Time.unscaledDeltaTime,.1f);
            var rect=(RectTransform)canvas.transform;float fit=Mathf.Min(rect.rect.width/1280,rect.rect.height/720);content.localScale=Vector3.one*fit;
            flags.anchoredPosition=new Vector2(0,Mathf.Lerp(160,-10,Mathf.SmoothStep(0,1,elapsed/.8f)));flags.localRotation=Quaternion.Euler(0,0,Mathf.Sin(elapsed*3)*Mathf.Max(0,1-elapsed/2)*4);
            float progress=Mathf.Clamp01((elapsed-.6f)/4.5f);jeep.anchoredPosition=new Vector2(Mathf.Lerp(1390,-380,progress),-386+Mathf.Sin(elapsed*25)*1.5f);jeepImage.uvRect=new Rect((Mathf.FloorToInt(elapsed*8)%4)*.25f,.15f,.25f,.57f);
            for(int i=0;i<layers.Length;i++)layers[i].rectTransform.anchoredPosition=new Vector2(-200+Mathf.Sin(elapsed*.3f)*(i+1)*3,0);
            if(elapsed>.6f&&!enginePlayed){enginePlayed=true;Cue("jeepney_engine");}
            if(elapsed>2.5f&&!stingerPlayed){stingerPlayed=true;Cue("opening_sting");logoPlate.gameObject.SetActive(true);}
            if(stingerPlayed){
                float t=elapsed-2.5f;float pop=1+Mathf.Sin(t*12)*Mathf.Exp(-t*5)*.16f;logoPlate.localScale=Vector3.one*pop;
                logoGlow.color=new Color(1,.75f,.3f,.12f+Mathf.Sin(t*2)*.04f);
                // A narrow crop of the SAME transparent lettering sweeps across it: no opaque rectangle.
                float u=Mathf.Repeat(t*.55f,1.3f)-.15f,left=Mathf.Clamp01(u),right=Mathf.Clamp01(u+.10f);
                var r=logo.rectTransform;logoShine.rectTransform.anchoredPosition=r.anchoredPosition+new Vector2(left*r.rect.width,0);
                logoShine.rectTransform.sizeDelta=new Vector2((right-left)*r.rect.width,r.rect.height);
                logoShine.uvRect=new Rect(left,0,right-left,1);
            }
            float alpha=elapsed<.65f?1-elapsed/.65f:elapsed>5.7f?(elapsed-5.7f)/.6f:0;fade.color=new Color(0,0,0,Mathf.Clamp01(alpha));
            bool skip=false;if(Keyboard.current!=null)foreach(var key in Keyboard.current.allKeys)if(key.wasPressedThisFrame){skip=true;break;}
            foreach(var pad in Gamepad.all)foreach(var control in pad.allControls)if(control is UnityEngine.InputSystem.Controls.ButtonControl button&&button.wasPressedThisFrame)skip=true;
            foreach(var stick in Joystick.all)if(stick.trigger.wasPressedThisFrame)skip=true;
            if(elapsed>=6.3f||(skip&&!Phase2Smoke.Active))SceneManager.LoadScene("Pamagat");
        }
    }
}
