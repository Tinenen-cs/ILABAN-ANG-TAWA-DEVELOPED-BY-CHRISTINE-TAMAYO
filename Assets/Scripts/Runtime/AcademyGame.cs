using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using System;
using System.Collections.Generic;

namespace Hexbound
{
    // Runtime Canvas preserves Main.unity and original asset GUIDs. Rendering never drives combat.
    public sealed class AcademyGame : MonoBehaviour
    {
        public enum ScreenState { Title, Select, StageSelect, Fight, Controls }
        public ScreenState ScreenNow { get; private set; }
        public DuelModel Model { get; private set; }
        public readonly int[] Selected={0,4};
        public readonly bool[] Ready={false,false};
        public bool Paused { get; private set; }
        public LanguageMode ReactionLanguage => reactions.Mode;
        AcademyInputManager controls;
        ReactionBank reactions;
        Canvas canvas;
        RectTransform safe,view;
        RawImage scenery;
        TMP_FontAsset font,bodyFont;
        readonly Texture2D[] heads=new Texture2D[8],bodies=new Texture2D[8],items=new Texture2D[8],stages=new Texture2D[3];
        readonly Dictionary<string,Sprite> sprites=new Dictionary<string,Sprite>();
        readonly Dictionary<string,AudioClip> clips=new Dictionary<string,AudioClip>();
        readonly List<Actor> previews=new List<Actor>();
        readonly List<RectTransform> flames=new List<RectTransform>();
        readonly List<RawImage> clouds=new List<RawImage>(),lamps=new List<RawImage>();
        RectTransform ambience,jeepney;
        RawImage shimmer;
        // Approved maps: normalized lamp/bulb centres (x, y from top) and the far-street line for passing traffic.
        static readonly Vector2[][] MapLamps={
            new[]{new Vector2(.065f,.42f),new Vector2(.265f,.42f),new Vector2(.595f,.39f),new Vector2(.71f,.10f),new Vector2(.958f,.42f)},
            new[]{new Vector2(.229f,.29f),new Vector2(.700f,.33f),new Vector2(.836f,.29f)},
            new[]{new Vector2(.146f,.165f),new Vector2(.428f,.23f),new Vector2(.858f,.26f),new Vector2(.944f,.165f)}};
        static readonly float[] FarStreet={.62f,.61f,.585f};
        Texture2D MapTex(int arena) {var n=Roster.StageFiles[arena];return Tex("Maps/"+n+"/"+n+"_Takipsilim")??Tex("Stages/"+n);}
        Actor[] actors;
        TMP_Text[] names,deviceLabels;
        Image[][] dots;
        Bubble[] bubbles;
        RectTransform pausePanel,resultPanel;
        TMP_Text resultTitle,resultReason,focusLabel;
        Button[] menuButtons;
        RectTransform[] cards;
        Image[] stageBars;
        Image[][] cardMarkers;
        TMP_Text[] readiness;
        Actor[] selectionShowcase;
        Image[] selectionBars;
        AudioSource music,sfx,highlight;
        int selectedArena,focus,setting;
        bool fromFight;
        float clock,resultTime,master=.8f,musicLevel=.55f,soundLevel=.7f;
        readonly Color cream=new Color32(245,229,194,255),gold=new Color32(224,181,105,255),ink=new Color32(16,30,40,255);
        readonly Color[] playerColors={new Color32(81,214,218,255),new Color32(249,135,116,255)};
        sealed class Actor { public RectTransform root; public RawImage body,head,dust; public int id; }
        sealed class Bubble { public RectTransform root; public TMP_Text label; public CanvasGroup alpha; public float started=-10; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot() { if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="Pambungad")return;if(FindAnyObjectByType<AcademyGame>()==null) new GameObject("Ilaban ang Tawa").AddComponent<AcademyGame>(); }
        void Awake()
        {
            Application.targetFrameRate=60;
            controls=new AcademyInputManager(); reactions=new ReactionBank();
            font=Resources.Load<TMP_FontAsset>("Ilaban/Fonts/KalyeDisplay");bodyFont=Resources.Load<TMP_FontAsset>("Ilaban/Fonts/KalyeBody");
            for(int i=0;i<8;i++) { heads[i]=Resources.Load<Texture2D>("Characters/"+Roster.Files[i]+"/portraits"); bodies[i]=Tex("Bodies/Body_"+i);items[i]=Tex("Items/Item_"+i); }
            for(int i=0;i<3;i++) stages[i]=MapTex(i);
            foreach(var clip in Resources.LoadAll<AudioClip>("Ilaban/Audio")) clips[clip.name]=clip;
            music=gameObject.AddComponent<AudioSource>();sfx=gameObject.AddComponent<AudioSource>();highlight=gameObject.AddComponent<AudioSource>();
            music.loop=true;highlight.loop=true;highlight.clip=clips["highlight_loop"];
            master=PlayerPrefs.GetFloat("Ilaban.Master",.8f);musicLevel=PlayerPrefs.GetFloat("Ilaban.Music",.55f);soundLevel=PlayerPrefs.GetFloat("Ilaban.Sound",.7f);ApplyAudio();
            if(FindAnyObjectByType<AudioListener>()==null) gameObject.AddComponent<AudioListener>();
            BuildCanvas();Show(ScreenState.Title);
        }
        Texture2D Tex(string path)=>Resources.Load<Texture2D>("Ilaban/"+path);
        Sprite Skin(string path)
        {
            if(sprites.TryGetValue(path,out var sprite)) return sprite;
            var t=Tex("UI/"+path);sprite=Sprite.Create(t,new Rect(0,0,t.width,t.height),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect,new Vector4(8,8,8,8));sprites[path]=sprite;return sprite;
        }
        void BuildCanvas()
        {
            var go=new GameObject("Ilaban Canvas",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));go.transform.SetParent(transform,false);
            canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=10;
            var scaler=go.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1280,720);scaler.matchWidthOrHeight=.5f;
            var canvasRect=(RectTransform)go.transform;
            var background=Image(canvasRect,new Rect(0,0,1280,720),new Color32(35,54,67,255));Stretch(background.rectTransform);
            scenery=Raw(canvasRect,new Rect(0,0,1280,720),stages[0]);
            safe=Rect(canvasRect,"Safe layout",new Rect(0,0,1280,720));safe.anchorMin=safe.anchorMax=new Vector2(.5f,.5f);safe.pivot=new Vector2(.5f,.5f);safe.anchoredPosition=Vector2.zero;
            // The background expands into surplus aspect-ratio space; gameplay/UI keeps its proportions.
            if(FindAnyObjectByType<EventSystem>()==null) {
                var es=new GameObject("Ilaban EventSystem",typeof(EventSystem),typeof(InputSystemUIInputModule));es.transform.SetParent(transform,false);
                // Pointer-only module: keyboard/controller navigation is isolated by AcademyInputManager.
                var module=es.GetComponent<InputSystemUIInputModule>();module.AssignDefaultActions();module.move=null;module.submit=null;module.cancel=null;
            }
        }
        static void Stretch(RectTransform r) {r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;}
        RectTransform Rect(Transform parent,string name,Rect area)
        {
            var g=new GameObject(name,typeof(RectTransform));g.transform.SetParent(parent,false);var r=(RectTransform)g.transform;
            r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(area.x,-area.y);r.sizeDelta=area.size;return r;
        }
        Image Image(Transform parent,Rect rect,Color color,string skin=null)
        {
            var r=Rect(parent,skin??"Panel",rect);var im=r.gameObject.AddComponent<Image>();im.color=color;im.raycastTarget=false;
            if(skin!=null) {im.sprite=Skin(skin);im.type=UnityEngine.UI.Image.Type.Sliced;}
            return im;
        }
        RawImage Raw(Transform parent,Rect rect,Texture2D texture)
        {
            var r=Rect(parent,"Pixel artwork",rect);var image=r.gameObject.AddComponent<RawImage>();image.texture=texture;image.raycastTarget=false;return image;
        }
        TMP_Text Label(Transform parent,Rect rect,string text,int size=24,Color? color=null,TextAnchor alignment=TextAnchor.MiddleCenter)
        {
            var r=Rect(parent,"Teksto",rect);var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=size>=27?font:bodyFont;t.text=text;t.fontSize=size;t.enableAutoSizing=true;t.fontSizeMax=size;t.fontSizeMin=Mathf.Max(10,size*.6f);t.color=color??cream;t.alignment=alignment==TextAnchor.MiddleLeft?TextAlignmentOptions.MidlineLeft:alignment==TextAnchor.MiddleRight?TextAlignmentOptions.MidlineRight:TextAlignmentOptions.Midline;t.raycastTarget=false;t.textWrappingMode=TextWrappingModes.Normal;t.overflowMode=TextOverflowModes.Truncate;return t;
        }
        Button Button(Transform parent,Rect rect,string text,Action action,bool enabled=true)
        {
            var image=Image(parent,rect,Color.white,text=="Bumalik"?"Bumalik":"Simulan");image.raycastTarget=true;
            var b=image.gameObject.AddComponent<Button>();b.targetGraphic=image;b.interactable=enabled;b.navigation=new Navigation {mode=Navigation.Mode.None};
            var colors=b.colors;colors.highlightedColor=new Color(1,.86f,.6f);colors.pressedColor=new Color(.7f,.85f,.8f);colors.disabledColor=new Color(.45f,.45f,.45f);b.colors=colors;
            b.onClick.AddListener(()=>{Sound("menu_confirm");action();});Label(image.transform,new Rect(8,4,rect.width-16,rect.height-8),text,25,cream);return b;
        }
        void Music(string name) {if(clips.TryGetValue(name,out var clip) && music.clip!=clip) {music.clip=clip;music.Play();}}
        void Sound(string name) {if(clips.TryGetValue(name,out var clip)) sfx.PlayOneShot(clip);}
        void ApplyAudio() {AudioListener.volume=master;music.volume=musicLevel;sfx.volume=soundLevel;highlight.volume=soundLevel*.07f;}
        void SaveAudio() {ApplyAudio();PlayerPrefs.SetFloat("Ilaban.Master",master);PlayerPrefs.SetFloat("Ilaban.Music",musicLevel);PlayerPrefs.SetFloat("Ilaban.Sound",soundLevel);}
        public void Show(ScreenState state)
        {
            ScreenNow=state;focus=0;previews.Clear();flames.Clear();cards=null;cardMarkers=null;menuButtons=null;pausePanel=null;resultPanel=null;
            if(view!=null) {view.gameObject.SetActive(false);Destroy(view.gameObject);}
            view=Rect(safe,"Screen "+state,new Rect(0,0,1280,720));
            scenery.texture=stages[selectedArena];
            Ambience(state==ScreenState.Fight?Model.ArenaIndex:selectedArena,state!=ScreenState.Controls);
            if(state==ScreenState.Title) {Music("Title");Title();}
            if(state==ScreenState.Select) {Music("Selection");Select();}
            if(state==ScreenState.StageSelect) {Music("Selection");StageSelect();}
            if(state==ScreenState.Fight) {Music(Roster.StageFiles[Model.ArenaIndex]);Fight();}
            if(state==ScreenState.Controls) Controls();
            if(state==ScreenState.Select || state==ScreenState.StageSelect) highlight.Play();else highlight.Stop();
        }
        // Street life layered over every stage: drifting clouds, a jeepney on the far road, swaying banderitas, flickering lamps.
        // Parented to the cover-fit map so every layer stays pinned to the painted lamps and street at any aspect ratio.
        void Ambience(int arena,bool enabled)
        {
            clouds.Clear();lamps.Clear();jeepney=null;shimmer=null;
            if(ambience!=null)Destroy(ambience.gameObject);
            ambience=Rect(scenery.transform,"Buhay ng Kalye",new Rect(0,0,1,1));Stretch(ambience);
            if(!enabled)return;
            for(int i=0;i<3;i++) {var c=Area(ambience,Fx("Cloud"),new Rect(0,.02f+i*.07f,.16f-i*.03f,.08f-i*.015f));c.color=new Color(1,1,1,.32f-i*.07f);clouds.Add(c);}
            jeepney=Area(ambience,Tex("UI/Jeepney"),new Rect(0,FarStreet[arena]-.075f,.09f,.075f)).rectTransform;jeepney.GetComponent<RawImage>().color=new Color(.82f,.78f,.74f,.92f);
            foreach(var p in MapLamps[arena]) {var l=Area(ambience,Fx("Glow"),new Rect(p.x-.045f,p.y-.08f,.09f,.16f));lamps.Add(l);}
            shimmer=Area(ambience,Fx("Shimmer"),new Rect(0,.70f,.4f,.10f));
        }
        Texture2D Fx(string name)=>Tex("Maps/FX/"+name);
        RawImage Area(Transform parent,Texture2D texture,Rect normalized)
        {
            var image=Raw(parent,new Rect(0,0,0,0),texture);var r=image.rectTransform;
            r.anchorMin=new Vector2(normalized.xMin,1-normalized.yMax);r.anchorMax=new Vector2(normalized.xMax,1-normalized.yMin);r.offsetMin=r.offsetMax=Vector2.zero;return image;
        }
        void UpdateAmbience()
        {
            for(int i=0;i<clouds.Count;i++) {var r=clouds[i].rectTransform;float w=r.anchorMax.x-r.anchorMin.x,x=Mathf.Repeat(clock*(.006f+i*.004f)+i*.37f,1+w)-w;r.anchorMin=new Vector2(x,r.anchorMin.y);r.anchorMax=new Vector2(x+w,r.anchorMax.y);}
            if(jeepney!=null) {float w=jeepney.anchorMax.x-jeepney.anchorMin.x,x=Mathf.Repeat(clock*.07f,1.6f)-w;float bob=((int)(clock*8)%2)*.002f;jeepney.anchorMin=new Vector2(x,jeepney.anchorMin.y);jeepney.anchorMax=new Vector2(x+w,jeepney.anchorMax.y);jeepney.anchoredPosition=new Vector2(0,bob*720);}
            for(int i=0;i<lamps.Count;i++) {float flicker=Mathf.PerlinNoise(clock*5,i*3.1f);lamps[i].color=new Color(1,1,1,.18f+flicker*.22f+(Mathf.PerlinNoise(clock*40,i)>.92f?-.12f:0));}
            if(shimmer!=null) {var r=shimmer.rectTransform;float x=.3f+Mathf.Sin(clock*.35f)*.3f;r.anchorMin=new Vector2(x,r.anchorMin.y);r.anchorMax=new Vector2(x+.4f,r.anchorMax.y);shimmer.color=new Color(1,1,1,.10f+.05f*Mathf.Sin(clock*1.7f));}
        }
        void Shade(float alpha=.72f)=>Image(view,new Rect(0,0,1280,720),new Color(.025f,.06f,.09f,alpha));
        void Heading(string title,string subtitle) {Label(view,new Rect(30,28,1220,62),title,48);Label(view,new Rect(100,92,1080,32),subtitle,20,gold);}
        void Title()
        {
            Shade(.59f);Label(view,new Rect(0,102,1280,36),"L A R O   N G   B A R A N G A Y",23,gold);
            Label(view,new Rect(90,166,1100,91),"ILABAN ANG TAWA",82).overflowMode=TextOverflowModes.Overflow;
            Label(view,new Rect(90,264,1100,42),"DEVELOPED BY CHRISTINE TAMAYO",31,gold);
            string[] labels={"Simulan ang Laro","Mga Kontrol","Lumabas"};menuButtons=new Button[3];
            for(int i=0;i<3;i++) {int index=i;menuButtons[i]=Button(view,new Rect(440,366+i*67,400,55),labels[i],()=>ActivateTitle(index));}
            Label(view,new Rect(220,619,840,35),"Pindutin ang anumang pindutan upang magsimula",23,gold);
            Label(view,new Rect(240,667,800,25),"DALAWANG MANLALARO  /  TATLONG ENTABLADO",18);
        }
        void ActivateTitle(int index) {if(index==0) OpenSelect();else if(index==1){fromFight=false;Show(ScreenState.Controls);}else Application.Quit();}
        void OpenSelect() {Ready[0]=Ready[1]=false;Paused=false;Show(ScreenState.Select);}
        void Select()
        {
            Shade(.72f);Heading("Pumili ng Tauhan","Pumili at kumpirmahin ang tauhan ng bawat manlalaro.");
            readiness=new TMP_Text[2];selectionShowcase=new Actor[2];
            for(int p=0;p<2;p++) {
                int side=p;float x=28+p*278;
                readiness[p]=Label(view,new Rect(x,132,270,37),"",23,playerColors[p]);
                selectionShowcase[p]=ActorView(view,Selected[p],new Rect(x+12,187,245,350));
                Label(view,new Rect(x-5,526,280,40),Roster.Names[Selected[p]],27,playerColors[p]);
                Button(view,new Rect(x+5,575,260,47),"Kumpirmahin",()=>Confirm(side));
            }
            cards=new RectTransform[8];cardMarkers=new Image[8][];
            for(int i=0;i<8;i++) {
                int id=i;float x=610+(i%4)*155,y=163+(i/4)*195;
                var r=Rect(view,"Tauhan "+i,new Rect(x,y,145,182));cards[i]=r;
                Slant(r,new Rect(0,0,145,182),new Color(.055f,.115f,.14f,.98f));
                var preview=ActorView(r,i,new Rect(27,10,90,132));previews.Add(preview);
                Raw(r,new Rect(108,95,27,27),items[i]);
                Image(r,new Rect(4,136,137,43),new Color(.025f,.055f,.07f,.95f));Label(r,new Rect(7,138,131,40),Roster.Names[i],19);
                cardMarkers[i]=new Image[2];for(int p=0;p<2;p++) {cardMarkers[i][p]=Image(r,new Rect(p==0?1:139,3,5,174),playerColors[p]);var badge=Label(cardMarkers[i][p].transform,new Rect(p==0?5:-25,0,25,25),(p+1).ToString(),22,playerColors[p]);}
                var hit=Image(r,new Rect(0,0,145,182),Color.clear);hit.raycastTarget=true;var button=hit.gameObject.AddComponent<Button>();button.navigation=new Navigation{mode=Navigation.Mode.None};button.onClick.AddListener(()=>{int side=Ready[0]?1:0;if(!Ready[side]) {Selected[side]=id;Sound("carousel_slide");RefreshDescriptions();}});
            }
            Label(view,new Rect(605,558,620,33),Roster.Descriptions[Selected[0]],20,playerColors[0]);
            Label(view,new Rect(605,594,620,33),Roster.Descriptions[Selected[1]],20,playerColors[1]);
            Button(view,new Rect(35,646,205,52),"Bumalik",()=>Show(ScreenState.Title));
            Button(view,new Rect(1040,646,205,52),"Simulan",()=>{if(Ready[0]&&Ready[1])Show(ScreenState.StageSelect);},Ready[0]&&Ready[1]);
            Label(view,new Rect(260,642,760,58),"KALIWA / KANAN  PUMILI    F / CTRL KANAN  KUMPIRMAHIN",20,gold);
        }
        void Slant(Transform parent,Rect rect,Color color) {var r=Rect(parent,"Slanted card",rect);var g=r.gameObject.AddComponent<SlantGraphic>();g.color=color;g.raycastTarget=false;}
        void Confirm(int side) {if(Ready[side])return;Ready[side]=true;Sound("confirm_sting");if(Ready[0]&&Ready[1]) Show(ScreenState.StageSelect);}
        void StageSelect()
        {
            Shade(.68f);Heading("Pumili ng Entablado","Tatlong lugar. Iisang tuntunin: iwasan ang imburnal.");
            cards=new RectTransform[3];stageBars=new Image[3];
            for(int i=0;i<3;i++) {
                int index=i;float x=93+i*371;
                var r=Rect(view,"Entablado "+i,new Rect(x,160,350,390));cards[i]=r;
                Slant(r,new Rect(0,0,350,390),new Color(.05f,.10f,.14f));stageBars[i]=Image(r,new Rect(5,382,340,6),gold);
                var crop=Rect(r,"Slanted preview",new Rect(10,8,330,300));var mask=crop.gameObject.AddComponent<SlantGraphic>();mask.color=Color.white;crop.gameObject.AddComponent<Mask>().showMaskGraphic=false;
                Raw(crop,new Rect(-120,-10,580,326),stages[i]);
                Image(r,new Rect(10,309,330,71),new Color(.025f,.05f,.07f,.97f));Label(r,new Rect(15,319,320,50),Roster.Stages[i],30);
                var hit=Image(r,new Rect(0,0,350,390),Color.clear);hit.raycastTarget=true;var b=hit.gameObject.AddComponent<Button>();b.navigation=new Navigation{mode=Navigation.Mode.None};b.onClick.AddListener(()=>ChooseStage(index));
            }
            focusLabel=Label(view,new Rect(150,565,980,48),Roster.StageDescriptions[selectedArena],25,gold);
            Button(view,new Rect(35,646,205,52),"Bumalik",OpenSelect);Button(view,new Rect(1040,646,205,52),"Simulan",StartMatch);
            Label(view,new Rect(260,645,760,53),"KALIWA / KANAN  PUMILI    ENTER / SUNTOK  KUMPIRMAHIN",20,gold);
        }
        void ChooseStage(int index) {selectedArena=index;scenery.texture=stages[index];Ambience(index,true);ambience.SetAsFirstSibling();if(focusLabel!=null)focusLabel.text=Roster.StageDescriptions[index];Sound("carousel_slide");}
        public void StartMatch() {if(!Ready[0]||!Ready[1])return;Model=new DuelModel(Selected[0],Selected[1],selectedArena);Paused=false;resultTime=0;Show(ScreenState.Fight);for(int p=0;p<2;p++)React(p,"ready");}
        Actor ActorView(Transform parent,int id,Rect area)
        {
            var a=new Actor {id=id,root=Rect(parent,"Tauhan",area)};
            a.dust=Raw(a.root,new Rect(-10,area.height-28,area.width+20,40),Tex("UI/Glow"));a.dust.color=new Color(.9f,.8f,.6f,.3f);
            a.body=Raw(a.root,new Rect(0,0,area.width,area.height),bodies[id]);a.body.uvRect=new Rect(0,6f/7,.25f,1f/7);
            a.head=Raw(a.root,new Rect(area.width*.23f,-area.height*.03f,area.width*.56f,area.height*.41f),heads[id]);a.head.uvRect=new Rect(0,.5f,.5f,.5f);
            return a;
        }
        void Fight()
        {
            scenery.texture=stages[Model.ArenaIndex];
            // Manholes live in gameplay space so they sit exactly on the model's fall points.
            foreach(float x in new[]{DuelModel.LeftRiftX,DuelModel.RightRiftX}) Raw(view,new Rect(x-110,552,220,69),Fx("Imburnal"));
            actors=new Actor[2];names=new TMP_Text[2];deviceLabels=new TMP_Text[2];dots=new Image[2][];bubbles=new Bubble[2];
            for(int p=0;p<2;p++) {
                actors[p]=ActorView(view,Model.players[p].character,new Rect(0,0,170,255));
                float x=p==0?22:958;
                var ring=Image(view,new Rect(x+101,20,96,96),playerColors[p],"PortraitMask");
                var mask=Image(ring.transform,new Rect(5,5,86,86),Color.white,"PortraitMask");mask.gameObject.AddComponent<Mask>().showMaskGraphic=false;
                var portrait=Raw(mask.transform,new Rect(0,0,86,86),heads[Model.players[p].character]);portrait.uvRect=new Rect(0,.5f,.5f,.5f);
                names[p]=Label(view,new Rect(x,119,300,32),Roster.Names[Model.players[p].character],25);
                deviceLabels[p]=Label(view,new Rect(x-10,154,320,30),"",18,playerColors[p]);
                Label(view,new Rect(x,187,300,23),"Sunod na Suntok",18,gold);
                dots[p]=new Image[5];for(int i=0;i<5;i++) dots[p][i]=Image(view,new Rect(x+77+i*30,215,22,22),new Color(.24f,.32f,.35f),"HiloDot");
                var orb=Image(view,new Rect(p==0?28:1156,610,96,96),playerColors[p],"Suntok");Label(orb.transform,new Rect(-20,63,136,28),"Suntok",23,cream);
                var bubble=new Bubble();bubble.root=Rect(view,"Reaksyon",new Rect(0,0,250,69));Image(bubble.root,new Rect(0,0,250,69),Color.white,"SpeechBubble");bubble.label=Label(bubble.root,new Rect(10,4,230,53),"",23,ink);bubble.alpha=bubble.root.gameObject.AddComponent<CanvasGroup>();bubble.alpha.blocksRaycasts=false;bubble.alpha.alpha=0;bubbles[p]=bubble;
            }
            Raw(view,new Rect(0,0,1280,720),Tex("UI/Vignette"));
            Label(view,new Rect(470,668,340,33),"Ihinto [ESC]    Mga Kontrol [H]",21,gold);
            resultPanel=Rect(view,"Bunga ng Laban",new Rect(330,22,620,226));Image(resultPanel,new Rect(0,0,620,226),new Color(.025f,.065f,.09f,.94f));
            resultTitle=Label(resultPanel,new Rect(15,8,590,48),"",32,gold);resultReason=Label(resultPanel,new Rect(15,58,590,43),"",24);
            Button(resultPanel,new Rect(20,111,275,51),"Maglaro Muli",StartMatch);Button(resultPanel,new Rect(325,111,275,51),"Pumili Muli",OpenSelect);Label(resultPanel,new Rect(15,176,590,34),"SUNTOK  MULING MAGLARO    ESC  PUMILI MULI",18,gold);resultPanel.gameObject.SetActive(false);
            pausePanel=Rect(view,"Pahinga",new Rect(385,240,510,280));Image(pausePanel,new Rect(0,0,510,280),new Color(.025f,.065f,.09f,.96f));Label(pausePanel,new Rect(0,15,510,47),"Pansamantalang Ihinto",34);
            menuButtons=new[]{Button(pausePanel,new Rect(75,85,360,52),"Magpatuloy",()=>{Paused=false;pausePanel.gameObject.SetActive(false);}),Button(pausePanel,new Rect(75,155,360,52),"Pangunahing Talaan",()=>Show(ScreenState.Title))};Label(pausePanel,new Rect(30,225,450,30),"KALIWA / KANAN  PUMILI    SUNTOK  PILIIN",19,gold);pausePanel.gameObject.SetActive(false);
        }
        void React(int p,string kind)
        {
            string line=reactions.Choose(p,Model.players[p].character,kind,clock);if(line==null)return;
            bubbles[p].label.text=line;bubbles[p].started=clock;
        }
        void Controls()
        {
            Shade(.87f);Heading("Mga Kontrol","Malaya ang bawat manlalaro sa sariling teklado o mando.");
            Label(view,new Rect(110,145,1060,151),"MANLALARO 1    A / D  LUMAKAD    F  SUNTOK / KUMPIRMAHIN\nMANLALARO 2    PALASO KALIWA / KANAN  LUMAKAD    CTRL KANAN / 0  SUNTOK\nMANDO    PALANCA / MGA PALASO  LUMAKAD    IBABA / KALIWANG PINDUTAN  SUNTOK\nH  MGA KONTROL    ESC  BUMALIK / IHINTO",23,cream,TextAnchor.MiddleLeft);
            Label(view,new Rect(110,310,1060,60),"Isang suntok, isang hakbang paatras. Limang sunod na tama: hilo.\nItulak ang katunggali sa imburnal upang manalo.",24,gold);
            string[] labels={"Wika ng Reaksyon","Pangunahing Lakas","Musika","Tunog"};menuButtons=new Button[5];
            for(int i=0;i<4;i++) {int index=i;menuButtons[i]=Button(view,new Rect(280,398+i*50,720,44),SettingText(i),()=>{setting=focus=index;AdjustSetting(1);});}
            menuButtons[4]=Button(view,new Rect(510,630,260,52),"Bumalik",CloseControls);
            Label(view,new Rect(270,687,740,24),"PATAAS / PABABA  PUMILI    KALIWA / KANAN  BAGUHIN",18,gold);
        }
        string SettingText(int i)=>i==0?"Wika ng Reaksyon: "+(reactions.Mode==LanguageMode.Pormal?"Pormal":"Kabataan"):(i==1?"Pangunahing Lakas":i==2?"Musika":"Tunog")+": "+Mathf.RoundToInt((i==1?master:i==2?musicLevel:soundLevel)*100)+"%";
        void AdjustSetting(int direction) {
            if(focus==4) return;
            int i=setting;
            if(i==0) reactions.Mode=reactions.Mode==LanguageMode.Pormal?LanguageMode.GenZ:LanguageMode.Pormal;
            if(i==1) master=Mathf.Clamp01(master+direction*.1f);if(i==2) musicLevel=Mathf.Clamp01(musicLevel+direction*.1f);if(i==3) soundLevel=Mathf.Clamp01(soundLevel+direction*.1f);
            SaveAudio();menuButtons[i].GetComponentInChildren<TMP_Text>().text=SettingText(i);Sound("menu_move");
        }
        void CloseControls() {Show(fromFight?ScreenState.Fight:ScreenState.Title);Paused=fromFight;}
        void Update()
        {
            clock+=Time.unscaledDeltaTime;controls.Poll(clock);
            var cr=(RectTransform)canvas.transform;float fit=Mathf.Min(cr.rect.width/1280f,cr.rect.height/720f);safe.localScale=new Vector3(fit,fit,1);
            // Cover unused aspect-ratio space with a proportion-preserving background.
            float cover=Mathf.Max(cr.rect.width/1280f,cr.rect.height/720f);scenery.rectTransform.sizeDelta=new Vector2(1280*cover,720*cover);scenery.rectTransform.anchorMin=scenery.rectTransform.anchorMax=new Vector2(.5f,.5f);scenery.rectTransform.pivot=new Vector2(.5f,.5f);scenery.rectTransform.anchoredPosition=ScreenNow==ScreenState.Fight?Vector2.zero:new Vector2(Mathf.Sin(clock*.2f)*2,Mathf.Sin(clock*.17f)*2);
            UpdateAmbience();
            if(ScreenNow==ScreenState.Title) {
                NavMenu(3);if(controls.Any("Confirm"))ActivateTitle(focus);
                else if(controls.StartPressed && controls.Navigate(0)==Vector2Int.zero && controls.Navigate(1)==Vector2Int.zero && !controls.Any("Help") && !controls.Any("Pause") && !controls.Any("Cancel")) OpenSelect();
            }
            else if(ScreenNow==ScreenState.Select) {
                for(int p=0;p<2;p++) {if(!Ready[p]) {int dir=controls.Navigate(p).x;if(dir!=0) {Selected[p]=(Selected[p]+dir+8)%8;Sound("carousel_slide");RefreshDescriptions();}if(controls.Press(p,"Confirm")) {Confirm(p);if(ScreenNow!=ScreenState.Select)return;}}
                    if(controls.Press(p,"Cancel")) {if(Ready[p])Ready[p]=false;else{Show(ScreenState.Title);return;}}}
                for(int p=0;p<2;p++){Animate(selectionShowcase[p],Pose.Idle,(int)(clock*4)%4);}
                for(int p=0;p<2;p++)readiness[p].text="Manlalaro "+(p+1)+"  /  "+(Ready[p]?"Handa na":"Kumpirmahin: "+controls.Prompt(p,"Confirm"));
                for(int i=0;i<8;i++) {bool active=i==Selected[0]||i==Selected[1];cards[i].localScale=Vector3.one*(active?1.035f:1);for(int p=0;p<2;p++)cardMarkers[i][p].gameObject.SetActive(Selected[p]==i);Animate(previews[i],active?Pose.Punch:Pose.Idle,(int)(clock*(active?5:3))%4);}
            }
            else if(ScreenNow==ScreenState.StageSelect) {
                int dir=controls.Navigate(0).x!=0?controls.Navigate(0).x:controls.Navigate(1).x;if(dir!=0)ChooseStage((selectedArena+dir+3)%3);
                for(int i=0;i<3;i++){cards[i].localScale=Vector3.one*(i==selectedArena?1.04f:.96f);stageBars[i].color=i==selectedArena?gold:new Color(.22f,.31f,.34f);}
                if(controls.Any("Cancel"))OpenSelect();else if(controls.Any("Confirm"))StartMatch();
            }
            else if(ScreenNow==ScreenState.Controls) {
                NavMenu(5);setting=focus;if(controls.Navigate(0).x!=0||controls.Navigate(1).x!=0)AdjustSetting(controls.Navigate(0).x!=0?controls.Navigate(0).x:controls.Navigate(1).x);
                if(controls.Any("Cancel")||controls.Any("Help"))CloseControls();else if(controls.Any("Confirm")) {if(focus==4)CloseControls();else AdjustSetting(1);}
            }
            else if(ScreenNow==ScreenState.Fight) UpdateFight();
            if(ScreenNow!=ScreenState.Fight && ScreenNow!=ScreenState.Controls && controls.Any("Help")){fromFight=false;Show(ScreenState.Controls);}
            if(menuButtons!=null && (ScreenNow==ScreenState.Title||ScreenNow==ScreenState.Controls||Paused)) for(int i=0;i<menuButtons.Length;i++)menuButtons[i].transform.localScale=Vector3.one*(i==focus?1.035f:1);
        }
        void NavMenu(int count) {var a=controls.Navigate(0);var b=controls.Navigate(1);int dir=a.y!=0?-a.y:b.y!=0?-b.y:a.x!=0?a.x:b.x;if(dir!=0){focus=(focus+dir+count)%count;Sound("menu_move");}}
        void RefreshDescriptions() {
            // Rebuild only on selection, never every animation frame; preserved state stays in Selected/Ready.
            Show(ScreenState.Select);
        }
        void UpdateFight()
        {
            if(controls.Any("Help")&&!Model.Finished){fromFight=true;Paused=true;Show(ScreenState.Controls);return;}
            if(controls.Any("Pause")&&!Model.Finished) {Paused=!Paused;focus=0;Sound("menu_back");}
            pausePanel.gameObject.SetActive(Paused);
            if(Paused) {NavMenu(2);if(controls.Any("Confirm")){if(focus==0)Paused=false;else Show(ScreenState.Title);}return;}
            if(!Model.Finished) {
                Model.Tick(Time.deltaTime,controls.Read(0),controls.Read(1));
                foreach(var ev in Model.events)React(ev.player,ev.kind);
                foreach(var sound in Model.sounds)Sound(sound);if(Model.Finished)Sound("crowd");
                if(Model.Finished) {
                    resultTime=0;resultPanel.gameObject.SetActive(true);resultTitle.text=Model.winner<0?"Kapwa Nahulog!":"Panalo si "+Roster.Names[Model.players[Model.winner].character]+"!";
                    resultReason.text=Model.winner<0?"Kapwa nahulog sa imburnal!":"Nahulog sa imburnal si "+Roster.Names[Model.players[1-Model.winner].character]+"!";
                }
            } else {resultTime+=Time.deltaTime;if(resultTime>.65f){if(controls.Any("Cancel")){OpenSelect();return;}if(controls.Any("Confirm")){StartMatch();return;}}}
            for(int p=0;p<2;p++) {
                var f=Model.players[p];var actor=actors[p];float fall=f.fallen?Mathf.Clamp01(resultTime/.65f):0;
                actor.root.anchoredPosition=new Vector2(f.x-85,-(343+fall*170));actor.root.localScale=Vector3.one*(1-fall*.75f);
                actor.head.rectTransform.localScale=Vector3.one; // Keep each original face readable while the body faces its rival.
                actor.root.localRotation=Quaternion.Euler(0,0,f.dizzyTime>0?Mathf.Sin(clock*12)*6:f.fallen?fall*80:0);
                actor.root.gameObject.SetActive(!f.fallen||resultTime<.8f);Animate(actor,f.pose,(int)(clock*8)%4);if(f.facing<0){var uv=actor.body.uvRect;uv.x+=uv.width;uv.width=-uv.width;actor.body.uvRect=uv;}
                deviceLabels[p].text=controls.DeviceName(p)+" / "+controls.Prompt(p,"Move")+" + "+controls.Prompt(p,"Punch");
                for(int i=0;i<5;i++)dots[p][i].color=i<f.streak?playerColors[p]:new Color(.24f,.32f,.35f);
                var bubble=bubbles[p];float age=clock-bubble.started;bool visible=age>=0&&age<1.1f;bubble.root.gameObject.SetActive(visible);
                if(visible) {bubble.alpha.alpha=Mathf.Min(1,age/.08f)*Mathf.Clamp01((1.1f-age)/.25f);float x=Mathf.Clamp(f.x-125,330,700);if(p==1 && Mathf.Abs(x-bubbles[0].root.anchoredPosition.x)<255)x=Mathf.Min(950,x+240);bubble.root.anchoredPosition=new Vector2(x+Mathf.Sin(age*70)*Mathf.Max(0,1-age*6)*3,-263);bubble.root.localScale=Vector3.one*Mathf.Lerp(.85f,1,Mathf.Clamp01(age/.1f));}
            }
            // A five-star orbit is separate from the streak dots and persists for the victim's recovery.
            UpdateDizzyStars();
            if(impactLabels==null || impactLabels[0]==null) {impactLabels=new TMP_Text[2];for(int i=0;i<2;i++)impactLabels[i]=Label(view,new Rect(0,0,110,80),"*",72,gold);}
            for(int i=0;i<2;i++) {bool visible=false;foreach(var burst in Model.bursts)if(Mathf.Abs(burst.x-Model.players[i].x)<5){visible=true;impactLabels[i].rectTransform.anchoredPosition=new Vector2(burst.x-55,-430);impactLabels[i].color=new Color(1,.88f,.58f,Mathf.Clamp01(burst.life/.35f));}impactLabels[i].gameObject.SetActive(visible);}

            for(int i=0;i<flames.Count;i++)flames[i].localScale=Vector3.one*(1+.06f*Mathf.Sin(clock*3+i));
        }
        TMP_Text[] stars;
        TMP_Text[] impactLabels;
        void UpdateDizzyStars() {
            if(stars==null||stars[0]==null) {stars=new TMP_Text[10];for(int i=0;i<10;i++)stars[i]=Label(view,new Rect(0,0,22,22),"*",27,gold);}
            for(int p=0;p<2;p++)for(int i=0;i<5;i++) {var star=stars[p*5+i];star.gameObject.SetActive(Model.players[p].dizzyTime>0&&!Model.Finished);float angle=clock*5+i*Mathf.PI*2/5;star.rectTransform.anchoredPosition=new Vector2(Model.players[p].x-11+Mathf.Cos(angle)*46,-(335+Mathf.Sin(angle)*13));}
        }
        void Animate(Actor actor,Pose pose,int frame) {
            int row=pose==Pose.Walk?1:pose==Pose.Punch?2:pose==Pose.Hurt?3:pose==Pose.Dizzy?4:pose==Pose.Fall?5:pose==Pose.Win?6:0;
            actor.body.uvRect=new Rect(frame*.25f,1-(row+1)/7f,.25f,1/7f);
            actor.dust.color=new Color(1,.78f,.45f,pose==Pose.Walk?.3f+frame*.08f:pose==Pose.Punch?.45f:pose==Pose.Win?.3f:.06f);
            int head=pose==Pose.Hurt?1:pose==Pose.Dizzy||pose==Pose.Fall?2:pose==Pose.Win?3:0;actor.head.uvRect=new Rect((head%2)*.5f,1-(head/2+1)*.5f,.5f,.5f);
            actor.head.rectTransform.anchoredPosition=new Vector2(actor.root.sizeDelta.x*.23f,actor.root.sizeDelta.y*.03f+(frame%2==0?0:1));
        }
        void OnDestroy() {controls?.Dispose();foreach(var sprite in sprites.Values)Destroy(sprite);}
#if UNITY_EDITOR
        public void CapturePreview(string path) {StartCoroutine(CaptureFrame(path));}
        System.Collections.IEnumerator CaptureFrame(string path) {yield return new WaitForEndOfFrame();var image=ScreenCapture.CaptureScreenshotAsTexture();System.IO.File.WriteAllBytes(path,image.EncodeToPNG());Destroy(image);}
#endif
    }
    public sealed class SlantGraphic : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();var r=rectTransform.rect;float cut=Mathf.Min(46,r.width*.15f);
            vh.AddVert(new Vector3(r.xMin+cut,r.yMax,0),color,new Vector2(0,1));vh.AddVert(new Vector3(r.xMax,r.yMax,0),color,new Vector2(1,1));vh.AddVert(new Vector3(r.xMax-cut,r.yMin,0),color,new Vector2(1,0));vh.AddVert(new Vector3(r.xMin,r.yMin,0),color,new Vector2(0,0));vh.AddTriangle(0,1,2);vh.AddTriangle(2,3,0);
        }
    }
}
