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
        TMP_Text logo,credit;
        Image fade;
        RawImage street;
        RawImage[] layers;
        AudioSource sound;
        float elapsed;
        bool enginePlayed,stingerPlayed;
        public float Elapsed => elapsed;
        void Awake()
        {
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
            flags=Art(content,new Rect(0,-160,1280,144),"UI/Banderitas").rectTransform;
            jeep=Art(content,new Rect(-350,445,320,160),"UI/Jeepney").rectTransform;
            logo=Text(content,new Rect(80,210,1120,100),"ILABAN ANG TAWA",80);logo.gameObject.SetActive(false);
            credit=Text(content,new Rect(130,330,1020,110),"Isang Laro mula sa Pilipinas\nHandog ni Christine Tamayo",30);credit.alpha=0;
            Text(content,new Rect(200,666,880,29),"Pindutin ang anumang pindutan upang laktawan",22);
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
            jeep.anchoredPosition=new Vector2(Mathf.Lerp(-350,1390,Mathf.Clamp01((elapsed-.6f)/4.5f)),-445+Mathf.Sin(elapsed*25)*1.5f);
            for(int i=0;i<layers.Length;i++)layers[i].rectTransform.anchoredPosition=new Vector2(-200+Mathf.Sin(elapsed*.3f)*(i+1)*3,0);
            if(elapsed>.6f&&!enginePlayed){enginePlayed=true;Cue("jeepney_engine");}
            if(elapsed>2.5f&&!stingerPlayed){stingerPlayed=true;Cue("opening_sting");logo.gameObject.SetActive(true);}
            if(stingerPlayed){float t=elapsed-2.5f;logo.rectTransform.localScale=new Vector3(1+Mathf.Sin(t*12)*Mathf.Exp(-t*5)*.22f,1-Mathf.Sin(t*12)*Mathf.Exp(-t*5)*.18f,1);credit.alpha=Mathf.Clamp01((elapsed-3.1f)/.7f);}
            float alpha=elapsed<.65f?1-elapsed/.65f:elapsed>5.7f?(elapsed-5.7f)/.6f:0;fade.color=new Color(0,0,0,Mathf.Clamp01(alpha));
            bool skip=false;if(Keyboard.current!=null)foreach(var key in Keyboard.current.allKeys)if(key.wasPressedThisFrame){skip=true;break;}
            foreach(var pad in Gamepad.all)foreach(var control in pad.allControls)if(control is UnityEngine.InputSystem.Controls.ButtonControl button&&button.wasPressedThisFrame)skip=true;
            foreach(var stick in Joystick.all)if(stick.trigger.wasPressedThisFrame)skip=true;
            if(elapsed>=6.3f||(skip&&!Phase2Smoke.Active))SceneManager.LoadScene("Pamagat");
        }
    }
}
