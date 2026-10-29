using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
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
        Texture2D[][][] actionFrames;
        AnchorBank animationAnchors;
        static readonly string[] ActionNames={"Idle","Walk","Strike","Hurt","Dizzy","Fall","Win","Lose"};
        readonly Dictionary<string,Sprite> sprites=new Dictionary<string,Sprite>();
        readonly Dictionary<string,AudioClip> clips=new Dictionary<string,AudioClip>();
        readonly List<Actor> previews=new List<Actor>();
        readonly List<RectTransform> flames=new List<RectTransform>();
        readonly List<RectTransform> confetti=new List<RectTransform>();
        readonly List<RawImage> lamps=new List<RawImage>();
        readonly List<RectTransform> leaves=new List<RectTransform>(),flags=new List<RectTransform>();
        readonly float[] readyAt={-10,-10},hoverUntil=new float[8];
        RectTransform ambience;
        readonly List<float>[] frameSamples={new List<float>(),new List<float>(),new List<float>()};
        [Serializable] sealed class FrameReport {public FrameRow[] maps;}
        [Serializable] sealed class FrameRow {public string map;public int samples;public float averageFps,p95Milliseconds,maxMilliseconds;}
        // Approved maps: normalized lamp/bulb centres (x, y from top).
        static readonly Vector2[][] MapLamps={
            new[]{new Vector2(.065f,.42f),new Vector2(.265f,.42f),new Vector2(.595f,.39f),new Vector2(.71f,.10f),new Vector2(.958f,.42f)},
            new[]{new Vector2(.229f,.29f),new Vector2(.700f,.33f),new Vector2(.836f,.29f)},
            new[]{new Vector2(.146f,.165f),new Vector2(.428f,.23f),new Vector2(.858f,.26f),new Vector2(.944f,.165f)}};
        Texture2D MapTex(int arena) {var n=Roster.StageFiles[arena];var light=arena==0?"Umaga":"Gabi";return Tex("Maps/"+n+"/"+n+"_"+light)??Tex("Maps/"+n+"/"+n+"_Gabi")??Tex("Stages/"+n);}
        Actor[] actors;
        TMP_Text[] names;
        Image[][] hiloPips,winMarkers;
        TMP_Text[] winSparkles;
        Bubble[] bubbles;
        RectTransform pausePanel,resultPanel,roundPanel;
        Actor resultWinner;
        TMP_Text roundTitle,roundReason;
        TMP_Text resultTitle,resultReason,focusLabel;
        Button[] menuButtons;
        RectTransform[] cards;
        Image[] stageBars;
        Image[][] cardMarkers;
        TMP_Text[] readiness;
        Actor[] selectionShowcase;
        Button[] confirmButtons,resultButtons;
        Button selectionStartButton; TMP_Text selectionHint;
        int selectionConsumedFrame=-1,stageChosenFrame=-1; bool selectionAwaitingRelease;
        Slider[] settingsSliders;
        TMP_Text[] settingValues;
        RectTransform exitPanel,errorPanel;
        TMP_Text errorMessage;
        Button errorButton;
        bool errorOpen;
        Button[] exitButtons;

        AudioSource music,sfx,highlight;
        int selectedArena,focus,setting,language;
        int screenEnteredFrame=-1;
        bool exitOpen;
        int exitReturnFocus;
        public int Language=>language;
        bool fromFight;
        float enteredAt;
        float clock,resultTime,master=.8f,musicLevel=.55f,soundLevel=.7f;
        readonly Color cream=new Color32(245,229,194,255),gold=new Color32(224,181,105,255),ink=new Color32(16,30,40,255);
        readonly Color[] playerColors={new Color32(81,214,218,255),new Color32(249,135,116,255)};
        static readonly Color[] HiloColors={new Color32(239,189,93,255),new Color32(243,158,75,255),new Color32(239,120,65,255),new Color32(224,83,60,255),new Color32(190,48,49,255)};
        [Serializable] sealed class AnchorBank {public CharacterAnchors[] characters=Array.Empty<CharacterAnchors>();}
        [Serializable] sealed class CharacterAnchors {public ActionAnchors[] actions=Array.Empty<ActionAnchors>();}
        [Serializable] sealed class ActionAnchors {public FrameAnchors[] frames=Array.Empty<FrameAnchors>();}
        [Serializable] sealed class FrameAnchors {public Vector2 head=Vector2.zero,grip=Vector2.zero,tip=Vector2.zero;}
        sealed class Actor { public RectTransform root,headTop; public RawImage body,dust; public int id; }
        sealed class Bubble { public RectTransform root,tail; public TMP_Text label; public CanvasGroup alpha; public float started=-10; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot() { if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="Pambungad")return;if(FindAnyObjectByType<AcademyGame>()==null) new GameObject("Ilaban ang Tawa").AddComponent<AcademyGame>(); }
        void Awake()
        {
            Application.targetFrameRate=60;
            controls=new AcademyInputManager(); reactions=new ReactionBank();
            font=Resources.Load<TMP_FontAsset>("Ilaban/Fonts/KalyeDisplay");bodyFont=Resources.Load<TMP_FontAsset>("Ilaban/Fonts/KalyeBody");
            actionFrames=new Texture2D[8][][];
            for(int i=0;i<8;i++) {
                string path="Characters/"+Roster.Files[i];heads[i]=Tex(path+"/Portrait");bodies[i]=Tex(path+"/Body");items[i]=Tex("Items/Item_"+i);
                actionFrames[i]=new Texture2D[ActionNames.Length][];
                for(int action=0;action<ActionNames.Length;action++) {
                    int frameCount=action==2?6:4;actionFrames[i][action]=new Texture2D[frameCount];
                    for(int frame=0;frame<frameCount;frame++)actionFrames[i][action][frame]=Tex(path+"/Actions/"+ActionNames[action]+"_"+frame.ToString("00"));
                }
                if(actionFrames[i][0][0]!=null)bodies[i]=actionFrames[i][0][0];
            }
            animationAnchors=JsonUtility.FromJson<AnchorBank>(Resources.Load<TextAsset>("Ilaban/Characters/AnimationAnchors").text);
            for(int i=0;i<3;i++) stages[i]=MapTex(i);
            foreach(var clip in Resources.LoadAll<AudioClip>("Ilaban/Audio")) clips[clip.name]=clip;
            music=gameObject.AddComponent<AudioSource>();sfx=gameObject.AddComponent<AudioSource>();highlight=gameObject.AddComponent<AudioSource>();
            music.loop=true;highlight.loop=false;
            reactions.Mode=PlayerPrefs.GetInt("Ilaban.ReactionStyle",0)==1?LanguageMode.GenZ:LanguageMode.Pormal;
            master=PlayerPrefs.GetFloat("Ilaban.Master",.8f);musicLevel=PlayerPrefs.GetFloat("Ilaban.Music",.55f);soundLevel=PlayerPrefs.GetFloat("Ilaban.Sound",.7f);ApplyAudio();
            language=PlayerPrefs.GetInt("Ilaban.Language",0)==1?1:0;
            if(FindAnyObjectByType<AudioListener>()==null) gameObject.AddComponent<AudioListener>();
            BuildCanvas();Show(ScreenState.Title);
        }
        Texture2D Tex(string path)=>Resources.Load<Texture2D>("Ilaban/"+path);
        Sprite Skin(string path)
        {
            if(sprites.TryGetValue(path,out var sprite)) return sprite;
            var t=Tex("UI/"+path);sprite=Sprite.Create(t,new Rect(0,0,t.width,t.height),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect,new Vector4(8,8,8,8));sprites[path]=sprite;return sprite;
        }
        Sprite Art(string path,Vector4 border=default)
        {
            string key="art:"+path;if(sprites.TryGetValue(key,out var sprite))return sprite;
            var t=Tex("UI/"+path);if(t==null)return null;
            sprite=Sprite.Create(t,new Rect(0,0,t.width,t.height),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect,border);sprites[key]=sprite;return sprite;
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
        Image RoundedPanel(Transform parent,Rect rect,Color color) {var panel=Image(parent,rect,color);panel.sprite=PlateSprite();panel.type=UnityEngine.UI.Image.Type.Sliced;return panel;}
        Image WoodPanel(Transform parent,Rect rect) {
            var panel=Image(parent,rect,new Color32(171,112,58,255));panel.sprite=PlateSprite();panel.type=UnityEngine.UI.Image.Type.Sliced;panel.name="Kahoy na balangkas";
            RoundedPanel(panel.transform,new Rect(7,7,rect.width-14,rect.height-14),new Color(.055f,.075f,.075f,.97f));return panel;
        }
        void KeyHint(Transform parent,float x,float width,string key,string text,float textWidth) {
            var cap=RoundedPanel(parent,new Rect(x,9,width,30),new Color32(112,72,43,255));cap.name="Pindutan "+key;Label(cap.transform,new Rect(2,2,width-4,26),key,17,cream);
            Label(parent,new Rect(x+width+10,7,textWidth,34),text,17,gold,TextAnchor.MiddleLeft);
        }
        RawImage Raw(Transform parent,Rect rect,Texture2D texture)
        {
            var r=Rect(parent,"Pixel artwork",rect);var image=r.gameObject.AddComponent<RawImage>();image.texture=texture;image.raycastTarget=false;return image;
        }
        TMP_Text Label(Transform parent,Rect rect,string text,int size=24,Color? color=null,TextAnchor alignment=TextAnchor.MiddleCenter)
        {
            text=Tr(text);
            var r=Rect(parent,"Teksto",rect);var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=size>=27?font:bodyFont;t.text=text;t.fontSize=size;t.enableAutoSizing=true;t.fontSizeMax=size;t.fontSizeMin=Mathf.Max(10,size*.6f);t.color=color??cream;t.alignment=alignment==TextAnchor.MiddleLeft?TextAlignmentOptions.MidlineLeft:alignment==TextAnchor.MiddleRight?TextAlignmentOptions.MidlineRight:TextAlignmentOptions.Midline;t.raycastTarget=false;t.textWrappingMode=TextWrappingModes.Normal;t.overflowMode=TextOverflowModes.Truncate;return t;
        }
        Button Button(Transform parent,Rect rect,string text,Action action,bool enabled=true)
        {
            bool roundBack=false; // Shared full button geometry and centered labels on every screen.
            if(roundBack){Label(parent,new Rect(rect.x+63,rect.y+4,135,rect.height-8),text,22,cream,TextAnchor.MiddleLeft);rect.width=rect.height;}
            var image=Image(parent,rect,Color.white);image.sprite=PlateSprite();image.type=UnityEngine.UI.Image.Type.Sliced;image.pixelsPerUnitMultiplier=1;image.color=new Color32(129,87,57,255);image.raycastTarget=true;
            if(roundBack){image.sprite=Skin("PortraitMask");image.type=UnityEngine.UI.Image.Type.Simple;image.name="Bumalik";}
            image.rectTransform.pivot=new Vector2(.5f,.5f);image.rectTransform.anchoredPosition=new Vector2(rect.x+rect.width/2,-rect.y-rect.height/2);
            image.name=Tr(text);var b=image.gameObject.AddComponent<Button>();b.targetGraphic=image;b.interactable=enabled;b.navigation=new Navigation {mode=Navigation.Mode.None};
            var colors=b.colors;colors.normalColor=Color.white;colors.highlightedColor=new Color(1f,.84f,.48f);colors.pressedColor=new Color(.75f,.9f,.83f);colors.selectedColor=new Color(1f,.88f,.62f);colors.disabledColor=new Color(.48f,.48f,.46f);colors.colorMultiplier=1;colors.fadeDuration=.08f;b.colors=colors;
            var outline=image.gameObject.AddComponent<Outline>();outline.effectColor=new Color32(239,196,119,180);outline.effectDistance=new Vector2(1.5f,-1.5f);outline.enabled=false;
            var feedback=image.gameObject.AddComponent<PlateButtonFeedback>();feedback.image=image;feedback.outline=outline;feedback.onHover=()=>Sound("menu_move");
            int icon=ButtonIcon(text);
            if(icon>=0){var ir=Rect(image.transform,"Sagisag",new Rect(13,rect.height/2-10,20,20));var graphic=ir.gameObject.AddComponent<PlateIcon>();graphic.kind=icon;graphic.color=cream;graphic.raycastTarget=false;feedback.icon=ir;}
            b.onClick.AddListener(()=>{Sound("menu_confirm");action();});Label(image.transform,new Rect(40,4,Mathf.Max(1,rect.width-80),rect.height-8),roundBack?"":Tr(text),25,cream);return b;
        }
        static int ButtonIcon(string text) {
            if(text=="Bumalik")return 0;
            if(text=="Kumpirmahin"||text=="Handa na"||text=="Oo"||text=="Sige")return 1;
            if(text=="Simulan"||text=="Simulan ang Laro"||text=="Magpatuloy")return 2;
            if(text=="Umalis")return 4;
            if(text=="Maglaro Muli")return 5;
            if(text=="Pumili ng Ibang Tauhan")return 6;
            if(text=="Pangunahing Talaan")return 7;
            if(text=="Hindi")return 8;
            return 3; // Settings rows and Mga Kontrol use the same gear.
        }
        static Sprite plateSprite;
        static Sprite PlateSprite() {
            if(plateSprite!=null)return plateSprite;
            const int n=96;var tex=new Texture2D(n,n,TextureFormat.RGBA32,false);tex.name="Antialiased padded signboard";tex.filterMode=FilterMode.Bilinear;tex.wrapMode=TextureWrapMode.Clamp;
            // Signed distance coverage, three transparent texels, and a complete curved rim.
            // Every nine-slice corner retains its original radius; no rectangular cut lines.
            for(int y=0;y<n;y++)for(int x=0;x<n;x++){
                var q=new Vector2(Mathf.Abs(x+.5f-n/2f)-31,Mathf.Abs(y+.5f-n/2f)-31);
                float distance=new Vector2(Mathf.Max(q.x,0),Mathf.Max(q.y,0)).magnitude+Mathf.Min(Mathf.Max(q.x,q.y),0)-14;
                float alpha=Mathf.Clamp01(.5f-distance);var color=Color.Lerp(Color.white,new Color32(237,195,126,255),Mathf.Clamp01(distance+2));color.a=alpha;tex.SetPixel(x,y,color);
            }tex.Apply();
            plateSprite=Sprite.Create(tex,new Rect(0,0,n,n),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect,new Vector4(20,20,20,20));plateSprite.name="Filipino signboard button";return plateSprite;
        }
        string Tr(string value) {
            if(language==0||string.IsNullOrEmpty(value))return value;
            switch(value) {
                case "Simulan ang Laro":return "Start Game";case "Mga Kontrol":return "Settings";case "Lumabas":case "Umalis":return "Exit";case "Pumili ng Tauhan":return "Choose a Fighter";case "Pumili ng Entablado":return "Choose an Arena";
                case "Pumili at kumpirmahin ang tauhan ng bawat manlalaro.":return "Choose and confirm a fighter for each player.";
                case "Pindutin ang Simulan para magpatuloy.":return "Press Start to continue.";case "Bumalik":return "Back";case "Simulan":return "Start";case "Kumpirmahin":return "Confirm";case "Handa na":return "Ready";case "Magpatuloy":return "Resume";case "Pangunahing Talaan":return "Main Menu";case "Pansamantalang Ihinto":return "Paused";
                case "Paunawa":return "Notice";case "Sige":return "OK";
                case "Pumili at kumpirmahin muna ang tauhan ng bawat manlalaro.":return "Choose and confirm each player's fighter first.";
                case "Pumili muna ng isang entablado.":return "Choose an arena first.";case "Hindi wasto ang napiling ayos.":return "That setting is not valid.";case "Hindi pa maaaring piliin ang kilos na ito.":return "That action is not available yet.";
                case "Pumili ng Ibang Tauhan":return "Choose Other Fighters";case "Maglaro Muli":return "Play Again";case "Pumili Muli":return "Choose Again";case "Sunod na Suntok":return "Next Hit";case "Hilo":return "Dizzy";case "Wika ng Reaksyon":return "Reaction Style";case "Wika ng Laro":return "Game Language";
                case "Pangunahing Lakas":return "Master Volume";case "Panalo":return "Wins";case "Musika":return "Music";case "Tunog":return "Sound";case "Umalis sa Laro?":return "Leave the Game?";case "Gusto mo na bang umalis sa laro?":return "Do you want to leave the game?";case "Oo":return "Yes";case "Hindi":return "No";
                case "Manlalaro 1":return "Player 1";case "Manlalaro 2":return "Player 2";case "Kumpirmahin: F":return "Confirm: F";case "Kumpirmahin: Ctrl kanan / 0":return "Confirm: Right Ctrl / 0";
                case "KALIWA / KANAN  PUMILI    ENTER / SUNTOK  KUMPIRMAHIN":return "LEFT / RIGHT  SELECT    ENTER / PUNCH  CONFIRM";case "KALIWA / KANAN  PUMILI    SUNTOK  PILIIN":return "LEFT / RIGHT  SELECT    PUNCH  CONFIRM";
                case "Isang suntok, isang hakbang paatras. Limang sunod na tama: hilo.\nItulak ang katunggali sa imburnal upang manalo.":return "One punch, one step back. Five hits in a row cause dizziness.\nPush your rival into the drain to win.";
                case "L A R O   N G   B A R A N G A Y":return "B A R A N G A Y   B R A W L";case "Tatlong lugar. Isang tuntunin: iwasan ang imburnal.":return "Three places. One rule: avoid the drain.";case "LARO NG BARANGAY":return "BARANGAY BRAWL";case "Pormal":return "Formal";case "Kabataan":return "Youth";
                case "MGA PINDUTAN":return "CONTROLS";case "Mga galaw at tunog ng laro.":return "Movement and sound settings.";case "Looban":return "Neighborhood Lane";
                case "Kalye sa Palengke":return "Market Street";case "Plaza ng Barangay":return "Barangay Plaza";
                case "Sa pagitan ng mga puwesto, tawa ang pinakamabentang paninda.":return "Between the stalls, laughter is the best seller.";
                case "Makitid ang daan, maluwag ang puwang para sa halakhak.":return "A narrow lane leaves plenty of room for laughter.";
                case "Sa liwanag ng mga parol, buong barangay ang nakikisaya.":return "Under the lanterns, the whole neighborhood joins in.";
                case "Sa bawat palo, pati labada ay napapahiyaw.":return "Every swing makes even the laundry cry out.";
                case "Isang tsinelas lamang, abot ang kabilang kanto.":return "One slipper reaches all the way down the lane.";
                case "Sa bato-bato-pik, ang tawa ang laging panalo.":return "In rock-paper-scissors, laughter always wins.";
                case "Bantay ng barangay at tagapagpatigil ng yabang.":return "The neighborhood guard keeps big egos in check.";
                case "Maingay ang lata, mas maingay ang halakhak.":return "The can is loud, but the laughter is louder.";
                case "May dalang upuan ngunit ayaw magpahinga.":return "Carries a chair but never sits down.";
                case "Maasim ang suka, matamis ang tagumpay.":return "The vinegar is sour; victory is sweet.";
                case "Walis ang hawak, kalat ang iniiwasan.":return "A broom in hand, sweeping messes away.";
                case "Kapwa Nahulog!":return "Both Fell!";case "Kapwa nahulog sa imburnal!":return "Both fighters fell into the drain!";
                default:
                    if(value.StartsWith("Kumpirmahin: ",StringComparison.Ordinal))return "Confirm: "+value.Substring(13);
                    if(value.StartsWith("Manlalaro ",StringComparison.Ordinal))return "Player "+value.Substring(10);
                    if(value.StartsWith("Panalo si ",StringComparison.Ordinal))return "Winner: "+value.Substring(10);
                    if(value.StartsWith("Nahulog sa imburnal si ",StringComparison.Ordinal))return value.Substring(24)+" fell into the drain!";
                    return value;
            }
        }
        void Music(string name) {if(clips.TryGetValue(name,out var clip) && music.clip!=clip) {music.clip=clip;music.Play();}}
        readonly Dictionary<string,float> lastSoundAt=new Dictionary<string,float>();
        float nextImpactSoundAt;
        float menuSoundUntil,queuedCueAt,lastCharacterCueAt=-10;
        int queuedCharacter=-1,lastCharacterCue=-1;
        void Sound(string name) {
            if(!clips.TryGetValue(name,out var clip))return;
            if(name.StartsWith("hit_",StringComparison.Ordinal)){if(clock<nextImpactSoundAt)return;nextImpactSoundAt=clock+.07f;}
            if(lastSoundAt.TryGetValue(name,out var last)&&clock-last<.035f)return;
            bool menu=name.StartsWith("menu_",StringComparison.Ordinal)||name=="carousel_slide"||name=="confirm_sting";
            if(menu){highlight.Stop();menuSoundUntil=Mathf.Max(menuSoundUntil,clock+clip.length);}
            lastSoundAt[name]=clock;sfx.PlayOneShot(clip);
        }
        void QueueCharacterCue(int character){queuedCharacter=Mathf.Clamp(character,0,7);queuedCueAt=Mathf.Max(clock+.025f,menuSoundUntil+.02f,lastCharacterCueAt+.28f);}
        void UpdateCharacterCue(){
            if(ScreenNow==ScreenState.Title||ScreenNow==ScreenState.Controls||Paused){queuedCharacter=-1;return;}
            if(queuedCharacter<0||clock<Mathf.Max(queuedCueAt,menuSoundUntil+.02f)||highlight.isPlaying)return;
            if(clips.TryGetValue("reaction_"+queuedCharacter,out var clip)){highlight.clip=clip;highlight.Play();lastCharacterCue=queuedCharacter;lastCharacterCueAt=clock;}queuedCharacter=-1;
        }
        void ApplyAudio() {AudioListener.volume=master;music.volume=musicLevel;sfx.volume=soundLevel;highlight.volume=soundLevel*.25f;}
        void SaveAudio() {ApplyAudio();PlayerPrefs.SetFloat("Ilaban.Master",master);PlayerPrefs.SetFloat("Ilaban.Music",musicLevel);PlayerPrefs.SetFloat("Ilaban.Sound",soundLevel);}
        public void Show(ScreenState state)
        {
            ScreenNow=state;screenEnteredFrame=Time.frameCount;enteredAt=clock;focus=0;exitOpen=false;errorOpen=false;previews.Clear();flames.Clear();confetti.Clear();cards=null;cardMarkers=null;menuButtons=null;confirmButtons=null;selectionStartButton=null;selectionHint=null;resultButtons=null;pausePanel=null;resultPanel=null;exitPanel=null;exitButtons=null;
            if(view!=null) {view.gameObject.SetActive(false);Destroy(view.gameObject);}
            view=Rect(safe,"Screen "+state,new Rect(0,0,1280,720));view.gameObject.AddComponent<CanvasGroup>();
            for(int i=0;i<3;i++)stages[i]=MapTex(i);
            scenery.texture=stages[selectedArena];
            if(state==ScreenState.Title) {scenery.texture=Tex("Backgrounds/Pamagat")??scenery.texture;Music("Title");Title();}
            if(state==ScreenState.Select) {scenery.texture=Tex("Backgrounds/PumiliTauhan")??scenery.texture;Music("Selection");Select();}
            if(state==ScreenState.StageSelect) {scenery.texture=Tex("Backgrounds/PumiliEntablado")??scenery.texture;Music("Selection");StageSelect();}
            if(state==ScreenState.Fight) {Music(Roster.StageFiles[Model.ArenaIndex]);Fight();}
            if(state==ScreenState.Controls) Controls();
            Ambience(state==ScreenState.Fight?Model.ArenaIndex:selectedArena,true);
            BuildExitDialog();BuildErrorDialog();
            if(state==ScreenState.Select)QueueCharacterCue(Selected[0]);else if(state!=ScreenState.StageSelect){highlight.Stop();queuedCharacter=-1;}
        }
        // Only subtle lamp and banderita motion is layered over the fixed stage paintings.
        void Ambience(int arena,bool enabled)
        {
            lamps.Clear();leaves.Clear();flags.Clear();
            if(ambience!=null){ambience.gameObject.SetActive(false);Destroy(ambience.gameObject);}
            ambience=Rect(scenery.transform,"Buhay ng Kalye",new Rect(0,0,1,1));Stretch(ambience);
            // Configure after each screen chooses its actual painting. Title and stage
            // selection previously disabled this layer, leaving completely static art.
            if(enabled&&scenery.texture!=null)ambience.gameObject.AddComponent<PaintingMotion>().Configure(scenery.texture.name);
        }
        Texture2D Fx(string name)=>Tex("Maps/FX/"+name);
        RawImage Area(Transform parent,Texture2D texture,Rect normalized)
        {
            var image=Raw(parent,new Rect(0,0,0,0),texture);var r=image.rectTransform;
            r.anchorMin=new Vector2(normalized.xMin,1-normalized.yMax);r.anchorMax=new Vector2(normalized.xMax,1-normalized.yMin);r.offsetMin=r.offsetMax=Vector2.zero;return image;
        }
        void UpdateAmbience() {} // PaintingMotion owns an independent unscaled clock.
        void Shade(float alpha=.72f)=>Image(view,new Rect(0,0,1280,720),new Color(.025f,.06f,.09f,alpha));
        void PaintedText(Transform parent,Rect area,string title,string asset,int size=48,bool always=false){
            var live=Label(parent,area,title,size);var tex=Tex("UI/Heading"+asset);
            if(tex==null||(!always&&language!=0))return;
            live.color=new Color(cream.r,cream.g,cream.b,0);float fit=Mathf.Min(area.width/tex.width,area.height/tex.height);float w=tex.width*fit,h=tex.height*fit;
            var art=Raw(parent,new Rect(area.x+(area.width-w)/2,area.y+(area.height-h)/2,w,h),tex);art.name="Pintadong pamagat: "+title;
        }
        void Heading(string title,string subtitle) {var plate=RoundedPanel(view,new Rect(335,20,610,78),new Color(.025f,.055f,.07f,.72f));string asset=title=="Mga Kontrol"?"Controls":title=="Pumili ng Tauhan"?"Characters":"Stages";PaintedText(view,new Rect(335,24,610,70),title,asset);Label(view,new Rect(100,101,1080,32),subtitle,20,gold);}
        void Title()
        {
            Shade(.59f);FestivalBanderitas.Create(view,new Rect(18,0,1244,76));Label(view,new Rect(0,82,1280,36),"L A R O   N G   B A R A N G A Y",23,gold);
            PaintedText(view,new Rect(90,148,1100,174),"ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO","Logo",70,true);
            string[] labels={"Simulan ang Laro","Mga Kontrol","Umalis"};menuButtons=new Button[3];
            for(int i=0;i<3;i++) {int index=i;menuButtons[i]=Button(view,new Rect(440,366+i*67,400,55),labels[i],()=>ActivateTitle(index));}
            Label(view,new Rect(220,619,840,35),language==0?"Pumili ng pindutan upang magpatuloy":"Choose a button to continue",23,gold);
            Label(view,new Rect(240,667,800,25),language==0?"DALAWANG MANLALARO  /  TATLONG ENTABLADO":"TWO PLAYERS  /  THREE ARENAS",18);
        }
        void BuildExitDialog(){
            exitPanel=Rect(view,"Kumpirmasyon sa Pag-alis",new Rect(0,0,1280,720));var blocker=Image(exitPanel,new Rect(0,0,1280,720),new Color(0,0,0,.65f));blocker.raycastTarget=true;
            var plate=Rect(exitPanel,"Tanong",new Rect(390,257,500,208));WoodPanel(plate,new Rect(0,0,500,208));
            Label(plate,new Rect(20,18,460,48),"Umalis sa Laro?",30,gold);Label(plate,new Rect(24,70,452,42),"Gusto mo na bang umalis sa laro?",22);
            exitButtons=new[]{Button(plate,new Rect(38,130,200,52),"Oo",QuitGame),Button(plate,new Rect(262,130,200,52),"Hindi",CancelExit)};exitPanel.gameObject.SetActive(false);
        }
        void BuildErrorDialog(){
            errorPanel=Rect(view,"Paunawa",new Rect(0,0,1280,720));var blocker=Image(errorPanel,new Rect(0,0,1280,720),new Color(0,0,0,.65f));blocker.raycastTarget=true;
            var plate=Rect(errorPanel,"Paunawa na kahoy",new Rect(350,235,580,250));WoodPanel(plate,new Rect(0,0,580,250));
            Label(plate,new Rect(20,18,540,40),"Paunawa",30,gold);errorMessage=Label(plate,new Rect(30,65,520,90),"",24);
            errorButton=Button(plate,new Rect(190,177,200,48),"Sige",CloseError);errorPanel.gameObject.SetActive(false);
        }
        public void ShowError(string message){if(errorPanel==null)return;errorMessage.text=Tr(message);errorOpen=true;screenEnteredFrame=Time.frameCount;errorPanel.gameObject.SetActive(true);errorPanel.SetAsLastSibling();errorButton.GetComponent<PlateButtonFeedback>().SetFocused(true);Sound("menu_back");}
        public void CloseError(){errorOpen=false;if(errorPanel!=null)errorPanel.gameObject.SetActive(false);screenEnteredFrame=Time.frameCount;}
        public void TryStartSelection(){
            if(ScreenNow!=ScreenState.Select||Time.frameCount==selectionConsumedFrame)return;
            if(!Ready[0]||!Ready[1])ShowError("Pumili at kumpirmahin muna ang tauhan ng bawat manlalaro.");else Show(ScreenState.StageSelect);
        }
        void TryStartStage(){if(Time.frameCount!=stageChosenFrame)StartMatch();}
        public void ActivateResult(int index){if(Model==null||!Model.MatchFinished||index<0||index>2){ShowError("Hindi pa maaaring piliin ang kilos na ito.");return;}if(index==0)StartMatch();else if(index==1)OpenSelect();else RequestExit();}
        public void ActivateTitle(int index) {if(index==0) OpenSelect();else if(index==1){fromFight=false;setting=0;Show(ScreenState.Controls);}else RequestExit();}
        public void RequestExit(){if(exitPanel==null||exitOpen)return;exitReturnFocus=focus;exitOpen=true;focus=1;screenEnteredFrame=Time.frameCount;exitPanel.gameObject.SetActive(true);exitPanel.SetAsLastSibling();}
        public void SetLanguage(int value){if(value<0||value>1){ShowError("Hindi wasto ang napiling ayos.");return;}language=value==1?1:0;PlayerPrefs.SetInt("Ilaban.Language",language);PlayerPrefs.Save();if(ScreenNow==ScreenState.Controls)Show(ScreenState.Controls);}
        public void SetReactionStyle(int value){if(value<0||value>1){ShowError("Hindi wasto ang napiling ayos.");return;}reactions.Mode=value==1?LanguageMode.GenZ:LanguageMode.Pormal;PlayerPrefs.SetInt("Ilaban.ReactionStyle",value==1?1:0);PlayerPrefs.Save();}
        public void CancelExit(){exitOpen=false;if(exitPanel!=null)exitPanel.gameObject.SetActive(false);focus=exitReturnFocus;screenEnteredFrame=Time.frameCount;}
        public void QuitGame(){
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying=false;
#else
            Application.Quit();
#endif
        }
        void OpenSelect() {Ready[0]=Ready[1]=false;Paused=false;Show(ScreenState.Select);}
        void Select()
        {
            Shade(.72f);Heading("Pumili ng Tauhan","Pumili at kumpirmahin ang tauhan ng bawat manlalaro.");
            readiness=new TMP_Text[2];selectionShowcase=new Actor[2];confirmButtons=new Button[2];
            for(int p=0;p<2;p++) {
                int side=p;float x=28+p*278;
                readiness[p]=Label(view,new Rect(x,125,270,62),"",18,playerColors[p]);
                selectionShowcase[p]=ActorView(view,Selected[p],new Rect(x+12,187,245,350));
                Label(view,new Rect(x-5,516,280,56),Roster.Names[Selected[p]],23,playerColors[p]);
                confirmButtons[p]=Button(view,new Rect(x+5,575,260,47),Ready[p]?"Handa na":"Kumpirmahin",()=>Confirm(side),!Ready[p]);
            }
            cards=new RectTransform[8];cardMarkers=new Image[8][];
            for(int i=0;i<8;i++) {
                int id=i;float x=610+(i%4)*155,y=163+(i/4)*195;
                var r=Rect(view,"Tauhan "+i,new Rect(x,y,145,182));cards[i]=r;
                var cardSurface=RoundedPanel(r,new Rect(0,0,145,182),new Color(.055f,.115f,.14f,.98f));cardSurface.raycastTarget=true;var cardButton=cardSurface.gameObject.AddComponent<Button>();cardButton.targetGraphic=cardSurface;cardButton.navigation=new Navigation{mode=Navigation.Mode.None};var cardFeedback=cardSurface.gameObject.AddComponent<PlateButtonFeedback>();cardFeedback.image=cardSurface;var cardOutline=cardSurface.gameObject.AddComponent<Outline>();cardOutline.enabled=false;cardFeedback.outline=cardOutline;cardButton.onClick.AddListener(()=>{int side=Ready[0]?1:0;if(!Ready[side]){Selected[side]=id;Sound("carousel_slide");RefreshDescriptions();QueueCharacterCue(id);}});
                cardFeedback.onHover=()=>{hoverUntil[id]=clock+.4f;Sound("menu_move");QueueCharacterCue(id);};
                var preview=ActorView(r,i,new Rect(27,10,90,132));previews.Add(preview);
                Image(r,new Rect(4,136,137,43),new Color(.025f,.055f,.07f,.95f));Label(r,new Rect(5,134,135,46),Roster.Names[i].Replace("Aling ","Aling\n").Replace("Kuya ","Kuya\n").Replace("Ate ","Ate\n").Replace("Kap ","Kap\n").Replace("Manong ","Manong\n").Replace("Tito ","Tito\n").Replace("Nanay ","Nanay\n"),16);
                cardMarkers[i]=new Image[2];for(int p=0;p<2;p++) {cardMarkers[i][p]=Image(r,new Rect(p==0?1:139,3,5,174),playerColors[p]);var badge=Label(cardMarkers[i][p].transform,new Rect(p==0?5:-25,0,25,34),(p+1).ToString(),22,playerColors[p]);}

            }
            Label(view,new Rect(605,553,620,42),Roster.Descriptions[Selected[0]],20,playerColors[0]);
            Label(view,new Rect(605,596,620,42),Roster.Descriptions[Selected[1]],20,playerColors[1]);
            Button(view,new Rect(35,646,205,52),"Bumalik",()=>Show(ScreenState.Title));
            selectionStartButton=Button(view,new Rect(1040,646,205,52),"Simulan",TryStartSelection,Ready[0]&&Ready[1]);
            var unavailable=selectionStartButton.gameObject.AddComponent<UnavailableButtonNotice>();unavailable.button=selectionStartButton;unavailable.onUnavailable=TryStartSelection;
            selectionHint=Label(view,new Rect(260,642,760,58),"KALIWA / KANAN  PUMILI    F / CTRL KANAN  KUMPIRMAHIN",20,gold);
            RefreshSelectionReadiness();
        }
        void Slant(Transform parent,Rect rect,Color color) {var r=Rect(parent,"Slanted card",rect);var g=r.gameObject.AddComponent<SlantGraphic>();g.color=color;g.raycastTarget=false;}
        void Confirm(int side) {if(Ready[side])return;Ready[side]=true;readyAt[side]=clock;selectionConsumedFrame=Time.frameCount;selectionAwaitingRelease=true;Sound("confirm_sting");QueueCharacterCue(Selected[side]);RefreshSelectionReadiness();}
        void RefreshSelectionReadiness(){
            bool both=Ready[0]&&Ready[1];
            for(int p=0;p<2;p++)if(confirmButtons!=null&&confirmButtons[p]!=null){
                confirmButtons[p].interactable=!Ready[p];confirmButtons[p].GetComponentInChildren<TMP_Text>().text=Tr(Ready[p]?"Handa na":"Kumpirmahin");
                if(readiness!=null)readiness[p].text=Tr("Manlalaro "+(p+1))+"\n"+(Ready[p]?Tr("Handa na"):Tr("Kumpirmahin: "+controls.Prompt(p,"Confirm")));
            }
            if(selectionStartButton!=null){selectionStartButton.interactable=both;var feedback=selectionStartButton.GetComponent<PlateButtonFeedback>();feedback.SetFocused(both);if(both)feedback.image.color=Color.Lerp(new Color32(151,101,59,255),new Color32(179,128,77,255),.5f+.5f*Mathf.Sin(clock*2.4f));}
            if(selectionHint!=null)selectionHint.text=Tr(both?"Pindutin ang Simulan para magpatuloy.":"KALIWA / KANAN  PUMILI    F / CTRL KANAN  KUMPIRMAHIN");
        }
        void StageSelect()
        {
            Shade(.68f);Heading("Pumili ng Entablado","Tatlong lugar. Isang tuntunin: iwasan ang imburnal.");
            cards=new RectTransform[3];stageBars=new Image[3];
            for(int i=0;i<3;i++) {
                int index=i;float x=93+i*371;
                var r=Rect(view,"Entablado "+i,new Rect(x,157,350,365));r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=new Vector2(x+175,-339.5f);cards[i]=r;
                var stageSurface=RoundedPanel(r,new Rect(0,0,350,365),new Color(.05f,.10f,.14f));stageSurface.raycastTarget=true;var stageButton=stageSurface.gameObject.AddComponent<Button>();stageButton.targetGraphic=stageSurface;stageButton.navigation=new Navigation{mode=Navigation.Mode.None};var stageFeedback=stageSurface.gameObject.AddComponent<PlateButtonFeedback>();stageFeedback.image=stageSurface;var stageOutline=stageSurface.gameObject.AddComponent<Outline>();stageOutline.enabled=false;stageFeedback.outline=stageOutline;stageButton.onClick.AddListener(()=>ChooseStage(index));stageBars[i]=Image(r,new Rect(12,352,326,5),gold);
                var crop=Rect(r,"Rounded preview",new Rect(10,8,330,280));var mask=crop.gameObject.AddComponent<Image>();mask.sprite=PlateSprite();mask.type=UnityEngine.UI.Image.Type.Sliced;mask.color=Color.white;crop.gameObject.AddComponent<Mask>().showMaskGraphic=false;
                stageFeedback.onHover=()=>Sound("menu_move");
                Raw(crop,new Rect(-120,-10,580,326),stages[i]);
                RoundedPanel(r,new Rect(10,290,330,57),new Color(.025f,.05f,.07f,.97f));Label(r,new Rect(15,294,320,45),Roster.Stages[i],30);

            }
            focusLabel=Label(view,new Rect(150,558,980,44),Tr(Roster.StageDescriptions[selectedArena]),25,gold);
            Button(view,new Rect(35,646,205,52),"Bumalik",OpenSelect);Button(view,new Rect(1040,646,205,52),"Simulan",TryStartStage);
            Label(view,new Rect(260,645,760,53),"KALIWA / KANAN  PUMILI    ENTER / SUNTOK  KUMPIRMAHIN",20,gold);
        }
        public void SelectArena(int index){if(index<0||index>=3){ShowError("Pumili muna ng isang entablado.");return;}selectedArena=index;}
        void ChooseStage(int index) {stageChosenFrame=Time.frameCount;selectedArena=index;scenery.texture=stages[index];Ambience(index,true);ambience.SetAsFirstSibling();if(focusLabel!=null)focusLabel.text=Tr(Roster.StageDescriptions[index]);Sound("carousel_slide");}
        public void StartMatch() {if(!Ready[0]||!Ready[1]){ShowError("Pumili at kumpirmahin muna ang tauhan ng bawat manlalaro.");return;}if(selectedArena<0||selectedArena>=3){ShowError("Pumili muna ng isang entablado.");return;}Model=new DuelModel(Selected[0],Selected[1],selectedArena);Paused=false;resultTime=0;Show(ScreenState.Fight);for(int p=0;p<2;p++)React(p,"ready");}
        Actor ActorView(Transform parent,int id,Rect area)
        {
            float originalWidth=area.width;area.width=area.height*bodies[id].width/bodies[id].height;area.x+=(originalWidth-area.width)/2;
            var a=new Actor {id=id,root=Rect(parent,"Tauhan",area)};
            a.root.pivot=new Vector2(.5f,0);a.root.anchoredPosition=new Vector2(area.x+area.width/2,-area.y-area.height);
            a.dust=Raw(a.root,new Rect(area.width/2-70,area.height-28,140,40),Tex("UI/Glow"));a.dust.color=new Color(.9f,.8f,.6f,.3f);
            a.body=Raw(a.root,new Rect(0,0,area.width,area.height),bodies[id]);a.body.uvRect=new Rect(0,0,1,1);
            a.headTop=Rect(a.root,"Head top anchor",new Rect(area.width/2,8,0,0));
            return a;
        }
        void Fight()
        {
            scenery.texture=stages[Model.ArenaIndex];
            // KanaL artwork is fixed at both stage edges and remains in the combat plane.
            for(int side=0;side<2;side++){var canal=Raw(view,new Rect(side==0?0:DuelModel.StageWidth-DuelModel.DrainArtWidth,DuelModel.DrainArtY,DuelModel.DrainArtWidth,DuelModel.DrainArtHeight),Fx("Kanal")??Fx("Imburnal"));if(side==1)canal.uvRect=new Rect(1,0,-1,1);}
            actors=new Actor[2];names=new TMP_Text[2];hiloPips=new Image[2][];winMarkers=new Image[2][];winSparkles=new TMP_Text[2];bubbles=new Bubble[2];
            for(int p=0;p<2;p++) {
                actors[p]=ActorView(view,Model.players[p].character,new Rect(0,0,170,255));
                actors[p].root.anchoredPosition=new Vector2(Model.players[p].x,-598);actors[p].body.uvRect=Model.players[p].facing<0?new Rect(1,0,-1,1):new Rect(0,0,1,1);Animate(actors[p],Model.players[p].pose,Model.players[p].pose==Pose.Punch?StrikeFrame(Model.players[p].actionLength-Model.players[p].actionTime):(int)(clock*8)%4);
                float hudX=p==0?20:1160;float barX=p==0?130:780;float nameX=barX;
                var join=Image(view,new Rect(p==0?106:1130,57,44,36),Color.white);join.sprite=Art("HudWoodBar",new Vector4(30,30,30,30));join.type=UnityEngine.UI.Image.Type.Sliced;join.name="Kahoy na dugtungan";
                var ring=Image(view,new Rect(hudX,14,100,100),Color.white);ring.sprite=Art("HudWoodRing");ring.type=UnityEngine.UI.Image.Type.Simple;ring.name=p==0?"P1 wooden portrait ring":"P2 wooden portrait ring";
                var mask=Image(ring.transform,new Rect(10,10,80,80),Color.white,"PortraitMask");mask.gameObject.AddComponent<Mask>().showMaskGraphic=false;
                var portrait=Raw(mask.transform,new Rect(0,0,80,80),heads[Model.players[p].character]);portrait.uvRect=new Rect(0,0,1,1);
                var namePlate=Image(view,new Rect(nameX,20,370,31),new Color(.94f,.82f,.62f,.96f));namePlate.sprite=PlateSprite();namePlate.type=UnityEngine.UI.Image.Type.Sliced;namePlate.name=p==0?"P1 wooden name plate":"P2 wooden name plate";
                names[p]=Label(namePlate.transform,new Rect(9,2,352,26),Roster.Names[Model.players[p].character],19,ink,p==0?TextAnchor.MiddleLeft:TextAnchor.MiddleRight);
                float nameWidth=Mathf.Clamp(names[p].GetPreferredValues(Roster.Names[Model.players[p].character]).x+2,80,272);
                float plateWidth=Mathf.Clamp(nameWidth+96,240,370);
                namePlate.rectTransform.sizeDelta=new Vector2(plateWidth,31);
                namePlate.rectTransform.anchoredPosition=new Vector2(p==0?barX:barX+370-plateWidth,-20);
                names[p].rectTransform.anchoredPosition=new Vector2(p==0?9:plateWidth-9-nameWidth,-2);
                names[p].rectTransform.sizeDelta=new Vector2(nameWidth,26);
                float medalOrigin=p==0?nameWidth+21:plateWidth-nameWidth-41;

                var bar=Raw(view,new Rect(barX,55,370,54),Tex("UI/HudWoodBar"));bar.name=p==0?"P1 Hilo wooden bar":"P2 Hilo wooden bar";
                string hilo=language==0?"Hilo":"Dizzy";
                var meterLabel=Label(bar.transform,new Rect(p==0?16:306,12,48,28),hilo,16,cream,p==0?TextAnchor.MiddleLeft:TextAnchor.MiddleRight);meterLabel.name="Hilo meter label";
                hiloPips[p]=new Image[5];
                for(int i=0;i<5;i++){
                    float sx=p==0?70+i*57:242-i*57;
                    hiloPips[p][i]=Image(bar.transform,new Rect(sx,14,52,26),new Color(.28f,.22f,.17f,.98f));
                    hiloPips[p][i].sprite=Art("HudSegment",new Vector4(12,12,12,12));hiloPips[p][i].type=UnityEngine.UI.Image.Type.Sliced;hiloPips[p][i].name="Hilo segment "+(i+1);
                }
                winMarkers[p]=new Image[3];
                for(int i=0;i<3;i++){
                    float sx=medalOrigin+(p==0?i*22:-i*22);
                    winMarkers[p][i]=Image(namePlate.transform,new Rect(sx,5,20,20),new Color(.34f,.27f,.20f,.9f));winMarkers[p][i].sprite=Art("HudMedal");winMarkers[p][i].name="Panalo medal "+(i+1);
                }
                winSparkles[p]=Label(namePlate.transform,new Rect(p==0?medalOrigin-5:medalOrigin-49,-6,74,42),"*",30,gold);winSparkles[p].name="Panalo sparkle";winSparkles[p].gameObject.SetActive(false);
                var bubble=new Bubble();bubble.root=Rect(view,"Reaksyon",new Rect(0,0,160,50));
                bubble.tail=Image(bubble.root,new Rect(80,48,14,14),cream).rectTransform;bubble.tail.pivot=new Vector2(.5f,.5f);bubble.tail.localRotation=Quaternion.Euler(0,0,45);
                RoundedPanel(bubble.root,new Rect(0,0,160,50),cream);bubble.label=Label(bubble.root,new Rect(8,3,144,43),"",17,ink);bubble.label.fontSizeMin=12;
                bubble.alpha=bubble.root.gameObject.AddComponent<CanvasGroup>();bubble.alpha.blocksRaycasts=false;bubble.alpha.alpha=0;bubbles[p]=bubble;
            }
            var versus=Image(view,new Rect(588,18,104,54),new Color(.98f,.88f,.70f,.98f));versus.sprite=PlateSprite();versus.type=UnityEngine.UI.Image.Type.Sliced;versus.name="Tapat badge";Label(versus.transform,new Rect(0,1,104,50),"VS",36,gold);
            Raw(view,new Rect(0,0,1280,720),Tex("UI/Vignette"));
            var hintPlate=Image(view,new Rect(288,652,704,48),new Color(.025f,.055f,.07f,.78f));hintPlate.sprite=PlateSprite();hintPlate.type=UnityEngine.UI.Image.Type.Sliced;hintPlate.name="Mga pindutan hint plate";
            KeyHint(hintPlate.transform,20,50,"ESC",language==0?"Ihinto":"Pause",100);KeyHint(hintPlate.transform,223,34,"H",language==0?"Mga Kontrol":"Controls",140);KeyHint(hintPlate.transform,448,88,"F / CTRL",language==0?"Suntok":"Punch",125);
            roundPanel=Rect(view,"Bunga ng Yugto",new Rect(370,192,540,130));WoodPanel(roundPanel,new Rect(0,0,540,130));
            roundTitle=Label(roundPanel,new Rect(20,18,500,42),"",30,gold);roundReason=Label(roundPanel,new Rect(20,66,500,48),"",20);roundPanel.gameObject.SetActive(false);
            resultPanel=Rect(view,"Bunga ng Laban",new Rect(320,145,640,465));WoodPanel(resultPanel,new Rect(0,0,640,465));
            resultTitle=Label(resultPanel,new Rect(20,16,600,40),"",30,gold);resultReason=Label(resultPanel,new Rect(24,58,592,32),"",20);
            resultWinner=ActorView(resultPanel,Selected[0],new Rect(245,94,150,145));
            resultButtons=new[]{Button(resultPanel,new Rect(105,252,430,48),"Maglaro Muli",()=>ActivateResult(0)),Button(resultPanel,new Rect(105,312,430,48),"Pumili ng Ibang Tauhan",()=>ActivateResult(1)),Button(resultPanel,new Rect(105,372,430,48),"Umalis",()=>ActivateResult(2))};
            Label(resultPanel,new Rect(30,427,580,26),"KALIWA / KANAN  PUMILI    SUNTOK  PILIIN",16,gold);resultPanel.gameObject.SetActive(false);
            Color[] paper={new Color32(243,185,96,255),new Color32(74,201,192,255),new Color32(231,119,99,255),new Color32(246,225,183,255)};
            for(int i=0;i<28;i++){float x=i%2==0?15+(i/2%5)*23:1140+(i/2%5)*23;var piece=Image(view,new Rect(x,0,7+i%3*2,12),paper[i%paper.Length]);confetti.Add(piece.rectTransform);piece.gameObject.SetActive(false);}
            pausePanel=Rect(view,"Pahinga",new Rect(385,150,510,450));WoodPanel(pausePanel,new Rect(0,0,510,450));Label(pausePanel,new Rect(0,16,510,47),"Pansamantalang Ihinto",34);
            string[] pauseLabels={"Magpatuloy","Mga Kontrol","Pumili ng Ibang Tauhan","Pangunahing Talaan","Umalis"};menuButtons=new Button[5];
            for(int i=0;i<5;i++){int index=i;menuButtons[i]=Button(pausePanel,new Rect(50,76+i*62,410,48),pauseLabels[i],()=>ActivatePause(index));}
            Label(pausePanel,new Rect(30,398,450,36),"KALIWA / KANAN  PUMILI    SUNTOK  PILIIN",16,gold);pausePanel.gameObject.SetActive(false);
        }
        public void ActivatePause(int index){
            if(index==0){Paused=false;pausePanel.gameObject.SetActive(false);}
            else if(index==1){fromFight=true;Show(ScreenState.Controls);Paused=true;}
            else if(index==2)OpenSelect();
            else if(index==3){Paused=false;Show(ScreenState.Title);}
            else RequestExit();
        }
        static string EnglishReaction(string line) {
            switch(line) {
                case "Handa na ako!":return "I am ready!";case "Magsimula na tayo!":return "Let us begin!";case "Pagbutihin natin!":return "Let us do our best!";case "Nakahanda ang lahat!":return "Everything is ready!";case "Maligayang paglalaro!":return "Have a good match!";case "Tawa ang puhunan!":return "Laughter is our strength!";case "Simulan ang paligsahan!":return "Start the contest!";case "Magkita tayo sa gitna!":return "Meet me in the middle!";
                case "Ito na ang suntok!":return "Here comes the punch!";case "Humanda ka!":return "Get ready!";case "Tingnan ang aking lakas!":return "See my strength!";case "Tuloy ang laban!":return "The match goes on!";case "Lumapit ka rito!":return "Come closer!";case "Heto ang hamon!":return "Here is my challenge!";case "Subukin ang aking galing!":return "Test my skill!";case "Hindi ako uurong!":return "I will not back down!";
                case "Aray!":return "Ouch!";case "Tinamaan ako!":return "I got hit!";case "Masakit iyon!":return "That hurt!";case "Napaatras ako!":return "I was pushed back!";case "Malakas ang suntok!":return "That was a strong punch!";case "Muntik na ako!":return "That was close!";case "Tuloy pa rin!":return "I am still going!";case "Kaya ko pa!":return "I can keep going!";
                case "Nahihilo ako!":return "I feel dizzy!";case "Umiikot ang paligid!":return "Everything is spinning!";case "Kailangan kong magpahinga!":return "I need a moment!";case "Teka muna!":return "Wait a moment!";case "Nawawala ang aking balanse!":return "I am losing my balance!";case "Panandaliang pahinga!":return "A short rest!";case "Sandali lamang!":return "Just a moment!";case "Lilinaw rin ang paligid!":return "The world will settle soon!";
                case "Nahulog ako!":return "I fell!";case "Nasa imburnal ako!":return "I am in the drain!";case "Natalo ako ngayon!":return "I lost this time!";case "Babangon akong muli!":return "I will get back up!";case "Mag-iingat na ako!":return "I will be more careful!";case "Muli akong lalaban!":return "I will fight again!";case "Nawala ang aking tuntungan!":return "I lost my footing!";case "Sa susunod ay babawi ako!":return "I will make it up next time!";
                case "Ako ang nanalo!":return "I won!";case "Tagumpay natin ito!":return "We did it!";case "Salamat sa mahusay na laban!":return "Thank you for a great match!";case "Mabuhay ang barangay!":return "Long live the neighborhood!";case "Masaya ang tagumpay!":return "Victory feels good!";case "Panalo ang pagtitiyaga!":return "Persistence wins!";case "Hanggang sa susunod!":return "Until next time!";case "Tawa ang ating gantimpala!":return "Laughter is our reward!";
                case "Pamalo ko ay handa na!":return "My wooden baton is ready!";case "Labada muna pagkatapos!":return "Laundry can wait until later!";case "Palo at tawa ang dala ko!":return "I bring a swing and a laugh!";
                case "Tsinelas ng bayan!":return "The neighborhood slipper!";case "Nasaan ang kapareha nito?":return "Where is its pair?";case "Tsinelas ko, huwag mawala!":return "Do not lose my slipper!";
                case "Bato-bato-pik, tawa ang kapalit!":return "Rock-paper-scissors, laughter is the prize!";case "Bato lamang ang baon ko!":return "I brought only a stone!";case "Mabigat ang aking bulsa!":return "My pocket is heavy!";
                case "Batuta ng barangay!":return "The barangay baton!";case "Tigil muna sa gulo!":return "Time to stop the trouble!";case "Bantay ako sa kanto!":return "I keep watch on this corner!";
                case "Tak-tak-tak!":return "Clang-clang-clang!";case "Lata ang maingay, ako ang masaya!":return "The can is loud, and I am louder!";case "Walang laman, maraming ingay!":return "Empty can, plenty of noise!";
                case "May upuan na tayo!":return "We have a chair now!";case "Silya ko, huwag masira!":return "Do not break my chair!";case "Upo muna pagkatapos!":return "I will sit down later!";
                case "Suka ang laman nito!":return "There is vinegar inside!";case "Asim at tawa ang dala ko!":return "I bring a sour smell and a laugh!";case "Ingat sa aking bote!":return "Watch out for my bottle!";
                case "Walisin ko ang kalat!":return "I will sweep the mess away!";case "Tingting ang aking sandigan!":return "My broom is my trusty tool!";case "Malinis ang ating laban!":return "Let us keep this match clean!";
                default:return line;
            }
        }
        void React(int p,string kind)
        {
            string line=reactions.Choose(p,Model.players[p].character,kind,clock);if(line==null)return;
            bubbles[p].label.text=language==1&&reactions.Mode==LanguageMode.Pormal?EnglishReaction(line):line;bubbles[p].started=clock;
            if(kind=="win"||kind=="lose")Sound("reaction_"+Model.players[p].character);else QueueCharacterCue(Model.players[p].character);
        }
        void Controls()
        {
            Shade(.87f);Heading("Mga Kontrol","Mga galaw at tunog ng laro.");
            for(int p=0;p<3;p++){
                float x=90+p*371;var card=WoodPanel(view,new Rect(x,141,358,185));
                Label(card.transform,new Rect(12,8,334,32),p==2?(language==0?"Mando":"Gamepad"):Tr("Manlalaro "+(p+1)),23,gold);
                string move=p==0?"A / D":p==1?(language==0?"Kaliwa / Kanan":"Left / Right"):(language==0?"Palanca":"Stick");
                string punch=p==0?"F":p==1?(language==0?"Ctrl kanan / 0":"Right Ctrl / 0"):"A / X";
                var walk=Rect(card.transform,"Lakad",new Rect(14,47,328,40));KeyHint(walk,0,174,move,language==0?"Lakad":"Move",115);
                var attack=Rect(card.transform,"Suntok",new Rect(14,100,328,40));KeyHint(attack,0,174,punch,language==0?"Suntok":"Punch",115);
                Label(card.transform,new Rect(12,151,334,23),p==2?(language==0?"B: Bumalik":"B: Back"):(language==0?"H: Mga Kontrol   ESC: Ihinto":"H: Controls   ESC: Pause"),15,cream);
            }
            Label(view,new Rect(120,338,1040,48),"Isang suntok, isang hakbang paatras. Limang sunod na tama: hilo.\nItulak ang katunggali sa imburnal upang manalo.",21,gold);
            WoodPanel(view,new Rect(170,390,940,210));menuButtons=new Button[6];
            settingsSliders=new Slider[3];settingValues=new TMP_Text[3];
            for(int i=0;i<5;i++){
                int index=i;var row=Button(view,new Rect(196,400+i*37,888,34),SettingText(i),()=>{if(index<2){setting=focus=index;AdjustSetting(1);}});menuButtons[i]=row;
                if(i<2)continue;
                var texts=row.GetComponentsInChildren<TMP_Text>();var label=texts[0];string[] title={"Pangunahing Lakas","Musika","Tunog"};label.text=Tr(title[i-2]);label.alignment=TextAlignmentOptions.MidlineLeft;label.rectTransform.sizeDelta=new Vector2(465,26);
                settingValues[i-2]=Label(row.transform,new Rect(485,6,72,22),Mathf.RoundToInt((i==2?master:i==3?musicLevel:soundLevel)*100)+"%",17,gold,TextAnchor.MiddleRight);
                settingsSliders[i-2]=VolumeSlider(row.transform,new Rect(568,10,290,14),i-2);
            }
            menuButtons[5]=Button(view,new Rect(510,618,260,52),fromFight?"Magpatuloy":"Bumalik",CloseControls);
            Label(view,new Rect(270,681,740,25),language==0?"PATAAS / PABABA: PUMILI    KALIWA / KANAN: BAGUHIN":"UP / DOWN: SELECT    LEFT / RIGHT: CHANGE",17,gold);
        }
        Slider VolumeSlider(Transform parent,Rect area,int index) {
            var root=Rect(parent,"Volume slider",area);var track=root.gameObject.AddComponent<Image>();track.color=new Color32(55,68,68,255);track.raycastTarget=true;
            var fill=Image(root,new Rect(0,0,0,area.height),new Color32(109,207,190,255));
            fill.rectTransform.anchorMin=new Vector2(0,.5f);fill.rectTransform.anchorMax=new Vector2(0,.5f);fill.rectTransform.pivot=new Vector2(0,.5f);fill.rectTransform.sizeDelta=Vector2.zero;fill.rectTransform.anchoredPosition=Vector2.zero;
            var handle=Image(root,new Rect(0,-3,20,20),new Color32(247,216,157,255));handle.raycastTarget=true;
            handle.rectTransform.anchorMin=handle.rectTransform.anchorMax=new Vector2(0,.5f);handle.rectTransform.pivot=new Vector2(.5f,.5f);handle.rectTransform.anchoredPosition=Vector2.zero;
            var slider=root.gameObject.AddComponent<Slider>();slider.direction=Slider.Direction.LeftToRight;slider.minValue=0;slider.maxValue=1;slider.fillRect=fill.rectTransform;slider.handleRect=handle.rectTransform;slider.targetGraphic=handle;slider.navigation=new Navigation{mode=Navigation.Mode.None};
            slider.value=index==0?master:index==1?musicLevel:soundLevel;
            slider.onValueChanged.AddListener(value=>{if(index==0)master=value;else if(index==1)musicLevel=value;else soundLevel=value;SaveAudio();if(settingValues!=null&&settingValues[index]!=null)settingValues[index].text=Mathf.RoundToInt(value*100)+"%";});
            return slider;
        }
        string SettingText(int i) {
            if(i==0)return (language==0?"Wika ng Laro":"Game Language")+": "+(language==0?"Tagalog":"English");
            if(i==1)return Tr("Wika ng Reaksyon")+": "+(reactions.Mode==LanguageMode.Pormal?Tr("Pormal"):Tr("Kabataan"));
            string[] labels={"Pangunahing Lakas","Musika","Tunog"};int n=i-2;float value=n==0?master:n==1?musicLevel:soundLevel;return Tr(labels[n])+": "+Mathf.RoundToInt(value*100)+"%";
        }
        void AdjustSetting(int direction) {
            if(focus==5) return;
            int i=setting;
            if(i==0){SetLanguage(1-language);return;}
            if(i==1)SetReactionStyle(reactions.Mode==LanguageMode.Pormal?1:0);
            if(i==2) master=Mathf.Clamp01(master+direction*.1f);if(i==3) musicLevel=Mathf.Clamp01(musicLevel+direction*.1f);if(i==4) soundLevel=Mathf.Clamp01(soundLevel+direction*.1f);
            if(i>=2){SaveAudio();if(settingsSliders!=null)settingsSliders[i-2].SetValueWithoutNotify(i==2?master:i==3?musicLevel:soundLevel);if(settingValues!=null)settingValues[i-2].text=Mathf.RoundToInt((i==2?master:i==3?musicLevel:soundLevel)*100)+"%";}Sound("menu_move");
        }
        void CloseControls() {Show(fromFight?ScreenState.Fight:ScreenState.Title);Paused=fromFight;}
        void Update()
        {
            clock+=Time.unscaledDeltaTime;controls.Poll(clock);if(view!=null)view.GetComponent<CanvasGroup>().alpha=Mathf.Clamp01((clock-enteredAt)/.16f);
            if(Keyboard.current!=null&&Keyboard.current.enterKey.wasPressedThisFrame&&(Keyboard.current.leftAltKey.isPressed||Keyboard.current.rightAltKey.isPressed))Screen.fullScreenMode=Screen.fullScreenMode==FullScreenMode.Windowed?FullScreenMode.FullScreenWindow:FullScreenMode.Windowed;
            var cr=(RectTransform)canvas.transform;float fit=Mathf.Min(cr.rect.width/1280f,cr.rect.height/720f);safe.localScale=new Vector3(fit,fit,1);
            // Cover unused aspect-ratio space with a proportion-preserving background.
            float cover=Mathf.Max(cr.rect.width/1280f,cr.rect.height/720f);scenery.rectTransform.sizeDelta=new Vector2(1280*cover,720*cover);scenery.rectTransform.anchorMin=scenery.rectTransform.anchorMax=new Vector2(.5f,.5f);scenery.rectTransform.pivot=new Vector2(.5f,.5f);scenery.rectTransform.anchoredPosition=ScreenNow==ScreenState.Fight?Vector2.zero:new Vector2(Mathf.Sin(clock*.2f)*2,Mathf.Sin(clock*.17f)*2);
            UpdateAmbience();
            if(Phase2Smoke.Active&&ScreenNow==ScreenState.Fight&&Model!=null&&!Model.Finished&&!Paused&&clock-enteredAt>1&&Time.realtimeSinceStartup-Phase2Smoke.LastCaptureAt>.5f)frameSamples[Model.ArenaIndex].Add(Time.unscaledDeltaTime*1000);
            UpdateCharacterCue();
            // Screen changes and pointer callbacks consume the initiating press.
            if(Time.frameCount==screenEnteredFrame)return;
            if(errorOpen){if(controls.Any("Confirm")||controls.Any("Cancel"))CloseError();return;}
            if(exitOpen){
                NavMenu(2);if(controls.Any("Cancel"))CancelExit();else if(controls.Any("Confirm")){if(focus==0)QuitGame();else CancelExit();}
                for(int i=0;i<2;i++)exitButtons[i].GetComponent<PlateButtonFeedback>().SetFocused(i==focus);return;
            }
            if(ScreenNow==ScreenState.Title) {NavMenu(3);if(controls.Any("Confirm"))ActivateTitle(focus);}
            else if(ScreenNow==ScreenState.Select) {
                bool bothBefore=Ready[0]&&Ready[1];
                if(controls.Any("Proceed")){TryStartSelection();if(ScreenNow!=ScreenState.Select||errorOpen)return;}
                else {
                    bool cancel0=controls.Press(0,"Cancel"),cancel1=controls.Press(1,"Cancel");
                    if(cancel0||cancel1){
                        bool hadReady=Ready[0]||Ready[1];
                        for(int p=0;p<2;p++)if((p==0?cancel0:cancel1)&&Ready[p])Ready[p]=false;
                        selectionConsumedFrame=Time.frameCount;selectionAwaitingRelease=true;
                        if(!hadReady){Show(ScreenState.Title);return;}
                    } else {
                        for(int p=0;p<2;p++)if(!Ready[p]){int dir=controls.Navigate(p).x;if(dir!=0){Selected[p]=(Selected[p]+dir+8)%8;Sound("carousel_slide");RefreshDescriptions();QueueCharacterCue(Selected[p]);}if(controls.Press(p,"Confirm"))Confirm(p);}
                        if(bothBefore&&Ready[0]&&Ready[1]&&!selectionAwaitingRelease&&controls.Any("Confirm")) {TryStartSelection();if(ScreenNow!=ScreenState.Select)return;}
                    }
                }
                if(!controls.AnyHeld("Confirm")&&!controls.AnyHeld("Proceed"))selectionAwaitingRelease=false;
                RefreshSelectionReadiness();
                for(int p=0;p<2;p++){bool react=Ready[p]?(clock-readyAt[p]<.6f||Mathf.Repeat(clock-readyAt[p],3.2f)<.24f):clock-enteredAt<.35f;Animate(selectionShowcase[p],react?Pose.Win:Pose.Idle,(int)(clock*4)%4);}
                for(int i=0;i<8;i++) {int owner=Selected[0]==i?0:Selected[1]==i?1:-1;bool active=owner>=0;cards[i].localScale=Vector3.one*(active?1.025f:1);for(int p=0;p<2;p++)cardMarkers[i][p].gameObject.SetActive(Selected[p]==i);var feedback=cards[i].GetComponentInChildren<PlateButtonFeedback>();if(feedback!=null){feedback.allowScale=false;feedback.SetFocused(active);}Animate(previews[i],clock<hoverUntil[i]||(owner>=0&&Ready[owner]&&clock-readyAt[owner]<.6f)?Pose.Win:Pose.Idle,(int)(clock*(active?5:3))%4);}
            }
            else if(ScreenNow==ScreenState.StageSelect) {
                int dir=controls.Navigate(0).x!=0?controls.Navigate(0).x:controls.Navigate(1).x;if(dir!=0)ChooseStage((selectedArena+dir+3)%3);
                for(int i=0;i<3;i++){cards[i].localScale=Vector3.one*(i==selectedArena?1.025f:.97f);stageBars[i].color=i==selectedArena?gold:new Color(.22f,.31f,.34f);var feedback=cards[i].GetComponentInChildren<PlateButtonFeedback>();if(feedback!=null){feedback.allowScale=false;feedback.SetFocused(i==selectedArena);}}
                if(controls.Any("Cancel"))OpenSelect();else if(dir==0&&(controls.Any("Confirm")||controls.Any("Proceed")))TryStartStage();
            }
            else if(ScreenNow==ScreenState.Controls) {
                NavMenu(6);setting=focus;if(controls.Navigate(0).x!=0||controls.Navigate(1).x!=0)AdjustSetting(controls.Navigate(0).x!=0?controls.Navigate(0).x:controls.Navigate(1).x);
                if(controls.Any("Cancel")||controls.Any("Help"))CloseControls();else if(controls.Any("Confirm")) {if(focus==5)CloseControls();else AdjustSetting(1);}
            }
            else if(ScreenNow==ScreenState.Fight) UpdateFight();
            if(Time.frameCount==screenEnteredFrame)return;
            if(ScreenNow!=ScreenState.Fight && ScreenNow!=ScreenState.Controls && controls.Any("Help")){fromFight=false;Show(ScreenState.Controls);}
            if(menuButtons!=null && (ScreenNow==ScreenState.Title||ScreenNow==ScreenState.Controls||Paused)) for(int i=0;i<menuButtons.Length;i++){menuButtons[i].transform.localScale=Vector3.one*(i==focus?1.035f:1);var f=menuButtons[i].GetComponent<PlateButtonFeedback>();if(f!=null)f.SetFocused(i==focus);}
        }
        public string PerformanceJson(){
            var report=new FrameReport{maps=new FrameRow[3]};
            for(int i=0;i<3;i++){var samples=frameSamples[i];samples.Sort();float sum=0;foreach(float value in samples)sum+=value;report.maps[i]=new FrameRow{map=Roster.Stages[i],samples=samples.Count,averageFps=samples.Count>0?1000f*samples.Count/sum:0,p95Milliseconds=samples.Count>0?samples[Mathf.Min(samples.Count-1,(int)(samples.Count*.95f))]:0,maxMilliseconds=samples.Count>0?samples[samples.Count-1]:0};}
            return JsonUtility.ToJson(report,true);
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
            if(Paused) {NavMenu(5);if(controls.Any("Confirm"))ActivatePause(focus);return;}
            if(!Model.Finished) {
                Model.Tick(Time.deltaTime,controls.Read(0),controls.Read(1));
                foreach(var ev in Model.events)React(ev.player,ev.kind);
                foreach(var sound in Model.sounds)Sound(sound);if(Model.Finished)Sound("crowd");
                if(Model.Finished) {
                    resultTime=0;resultPanel.gameObject.SetActive(Model.MatchFinished);roundPanel.gameObject.SetActive(!Model.MatchFinished);resultTitle.text=Model.winner<0?Tr("Kapwa Nahulog!"):Tr("Panalo si "+Roster.Names[Model.players[Model.winner].character]+"!");
                    if(Model.winner>=0)resultWinner.id=Model.players[Model.winner].character;
                    roundTitle.text=Model.winner<0?Tr("Kapwa Nahulog!"):Tr("Panalo si "+Roster.Names[Model.players[Model.winner].character]+"!");roundReason.text=language==0?"Susunod na yugto...":"Next round...";
                    resultReason.text=Model.winner<0?Tr("Kapwa nahulog sa imburnal!"):Tr("Nahulog sa imburnal si "+Roster.Names[Model.players[1-Model.winner].character]+"!");
                }
            } else {resultTime+=Time.deltaTime;
                if(!Model.MatchFinished){if(resultTime>=1.6f&&Model.BeginNextRound()){resultTime=0;Show(ScreenState.Fight);for(int p=0;p<2;p++)React(p,"ready");return;}}
                else {NavMenu(3);if(controls.Any("Cancel")){OpenSelect();return;}if(controls.Any("Confirm")){ActivateResult(focus);return;}}
            }
            for(int i=0;i<confetti.Count;i++){var piece=confetti[i];piece.gameObject.SetActive(Model.Finished);if(Model.Finished){float y=Mathf.Repeat(resultTime*100+i*27,210);piece.anchoredPosition=new Vector2(piece.anchoredPosition.x,-y);piece.localRotation=Quaternion.Euler(0,0,Mathf.Sin(clock*4+i)*35);}}
            for(int p=0;p<2;p++) {
                var f=Model.players[p];var actor=actors[p];float fall=f.fallen?Mathf.Clamp01(resultTime/.20f):0;
                bool celebration=Model.Finished&&!f.fallen&&p==Model.winner;
                actor.root.anchoredPosition=celebration?new Vector2(640,-655):new Vector2(f.x,-(598+fall*170));actor.root.localScale=Vector3.one*(celebration?1.06f:1-fall*.75f);
                actor.root.localRotation=Quaternion.Euler(0,0,f.dizzyTime>0?Mathf.Sin(clock*12)*6:f.fallen?fall*80:0);
                actor.root.gameObject.SetActive((!f.fallen||resultTime<.20f)&&!(Model.MatchFinished&&p==Model.winner));
                int actionFrame=f.pose==Pose.Punch?StrikeFrame(f.actionLength-f.actionTime):(int)(clock*8)%4;
                if(AnimationCapture.Active&&AnimationCapture.Frame>=0)actionFrame=AnimationCapture.Frame;
                actor.body.uvRect=f.facing<0?new Rect(1,0,-1,1):new Rect(0,0,1,1);Animate(actor,celebration?Pose.Win:f.pose,actionFrame);
                // Incoming hits belong to the victim HUD; streak ownership stays in DuelModel.
                for(int i=0;i<5;i++)hiloPips[p][i].color=i<Model.players[1-p].streak?HiloColors[i]:new Color(.30f,.23f,.17f,.95f);
                for(int i=0;i<winMarkers[p].Length;i++){bool won=i<f.wins;winMarkers[p][i].color=won?Color.white:new Color(.43f,.34f,.25f,.88f);float pop=won&&Model.Finished&&p==Model.winner&&i==f.wins-1?1+.18f*Mathf.Sin(Mathf.Clamp01(resultTime/.45f)*Mathf.PI):1;winMarkers[p][i].rectTransform.localScale=Vector3.one*pop;}
                bool sparkle=Model.Finished&&p==Model.winner&&resultTime<.65f;winSparkles[p].gameObject.SetActive(sparkle);if(sparkle){winSparkles[p].color=new Color(gold.r,gold.g,gold.b,1-resultTime/.65f);winSparkles[p].rectTransform.localScale=Vector3.one*(.8f+resultTime);winSparkles[p].transform.SetAsLastSibling();}
            }
            if(Model.MatchFinished)Animate(resultWinner,Pose.Win,(int)(clock*6)%4);
            UpdateReactionBubbles();
            // A five-star orbit is separate from the streak dots and persists for the victim's recovery.
            UpdateDizzyStars();
            if(impactLabels==null || impactLabels[0]==null) {impactLabels=new TMP_Text[2];for(int i=0;i<2;i++)impactLabels[i]=Label(view,new Rect(0,0,110,80),"*",72,gold);}
            for(int i=0;i<2;i++) {bool visible=false;foreach(var burst in Model.bursts)if(!Model.Finished&&Mathf.Abs(burst.x-Model.players[i].x)<5){visible=true;impactLabels[i].rectTransform.anchoredPosition=new Vector2(burst.x-55,-430);impactLabels[i].color=new Color(1,.88f,.58f,Mathf.Clamp01(burst.life/.35f));}impactLabels[i].gameObject.SetActive(visible);}
            if(resultButtons!=null)for(int i=0;i<resultButtons.Length;i++){resultButtons[i].transform.localScale=Vector3.one*(i==focus?1.045f:1);var f=resultButtons[i].GetComponent<PlateButtonFeedback>();if(f!=null)f.SetFocused(i==focus);}

            for(int i=0;i<flames.Count;i++)flames[i].localScale=Vector3.one*(1+.06f*Mathf.Sin(clock*3+i));
        }
        TMP_Text[] stars;
        TMP_Text[] impactLabels;
        void UpdateDizzyStars() {
            if(stars==null||stars[0]==null) {stars=new TMP_Text[10];for(int i=0;i<10;i++)stars[i]=Label(view,new Rect(0,0,82,84),"*",76,gold);}
            for(int p=0;p<2;p++)for(int i=0;i<5;i++) {
                var star=stars[p*5+i];star.gameObject.SetActive(Model.players[p].dizzyTime>0&&!Model.Finished);
                float angle=clock*5+i*Mathf.PI*2/5;
                star.rectTransform.anchoredPosition=new Vector2(Model.players[p].x-33+Mathf.Cos(angle)*66,-(350+Mathf.Sin(angle)*17));
                star.transform.SetAsLastSibling();
            }
        }
        void Animate(Actor actor,Pose pose,int frame) {
            if(actor==null||actor.body==null||actor.root==null)return;
            int action=pose==Pose.Walk?1:pose==Pose.Punch?2:pose==Pose.Hurt?3:pose==Pose.Dizzy?4:pose==Pose.Fall?5:pose==Pose.Win?6:pose==Pose.Lose?7:0;
            var frames=actionFrames[actor.id][action];var wholeBody=frames[frame%frames.Length];if(wholeBody!=null&&actor.body.texture!=wholeBody)actor.body.texture=wholeBody;
            var anchor=animationAnchors.characters[actor.id].actions[action].frames[frame%frames.Length];
            float x=actor.body.uvRect.width<0?896-anchor.head.x:anchor.head.x;
            actor.headTop.anchoredPosition=new Vector2(x/896*actor.root.sizeDelta.x,-anchor.head.y/512*actor.root.sizeDelta.y);
            actor.dust.color=new Color(1,.78f,.45f,pose==Pose.Walk?.3f+(frame%2)*.08f:pose==Pose.Punch?.45f:pose==Pose.Win?.3f:.06f);
        }
        void UpdateReactionBubbles() {
            var heads=new Vector2[2];var positions=new Vector2[2];var visible=new bool[2];
            for(int p=0;p<2;p++) {
                var bubble=bubbles[p];float age=clock-bubble.started;
                visible[p]=age>=0&&age<1.1f&&(!Model.Finished||(Model.players[p].fallen&&resultTime<.20f));bubble.root.gameObject.SetActive(visible[p]);
                if(!visible[p])continue;
                heads[p]=view.InverseTransformPoint(actors[p].headTop.position);
                positions[p]=new Vector2(Mathf.Clamp(heads[p].x-80,8,1112),Mathf.Clamp(heads[p].y+66,-570,-158));
                bubble.alpha.alpha=Mathf.Min(1,age/.08f)*Mathf.Clamp01((1.1f-age)/.25f);
            }
            if(visible[0]&&visible[1]&&new Rect(positions[0].x,positions[0].y-60,160,60).Overlaps(new Rect(positions[1].x,positions[1].y-60,160,60))) {
                int raised=heads[0].x<=heads[1].x?0:1;int lower=1-raised;
                positions[raised].y=Mathf.Min(-158,Mathf.Max(positions[0].y,positions[1].y)+64);
                positions[lower].y=Mathf.Min(positions[lower].y,positions[raised].y-64);
            }
            for(int p=0;p<2;p++)if(visible[p]) {
                var bubble=bubbles[p];bubble.root.anchoredPosition=positions[p];bubble.root.localScale=Vector3.one;
                bubble.tail.anchoredPosition=new Vector2(Mathf.Clamp(heads[p].x-positions[p].x,12,148),-48);bubble.root.SetAsLastSibling();
            }
        }
        // Visual phases align the extended weapon with the existing .12-second hit event.
        // This only chooses artwork; DuelModel remains the sole owner of attack timing/range.
        public static int StrikeFrame(float elapsed) => elapsed<.035f?0:elapsed<.08f?1:elapsed<DuelModel.PunchStartup?2:elapsed<.18f?3:elapsed<.24f?4:5;

        void OnDestroy() {controls?.Dispose();foreach(var sprite in sprites.Values)Destroy(sprite);}
#if UNITY_EDITOR
        public void CapturePreview(string path) {StartCoroutine(CaptureFrame(path));}
        System.Collections.IEnumerator CaptureFrame(string path) {yield return new WaitForEndOfFrame();var image=ScreenCapture.CaptureScreenshotAsTexture();System.IO.File.WriteAllBytes(path,image.EncodeToPNG());Destroy(image);}
#endif
    }
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class SkyCloudGraphic : MaskableGraphic {
        protected override void OnPopulateMesh(VertexHelper vh){
            vh.Clear();var r=rectTransform.rect;
            foreach(var ellipse in new[]{new Vector3(.25f,.42f,.24f),new Vector3(.50f,.55f,.31f),new Vector3(.75f,.40f,.23f)}){
                int first=vh.currentVertCount;Vector2 centre=new Vector2(r.xMin+r.width*ellipse.x,r.yMin+r.height*ellipse.y);vh.AddVert(centre,color,Vector2.zero);
                for(int i=0;i<=32;i++){float a=i*Mathf.PI/16;var edge=color;edge.a=0;vh.AddVert(centre+new Vector2(Mathf.Cos(a)*r.width*ellipse.z,Mathf.Sin(a)*r.height*.48f),edge,Vector2.zero);if(i>0)vh.AddTriangle(first,first+i,first+i+1);}
            }
        }
    }
    public sealed class UnavailableButtonNotice : MonoBehaviour, IPointerClickHandler {
        public Button button;public Action onUnavailable;
        public void OnPointerClick(PointerEventData e){if(!button.interactable&&e.button==PointerEventData.InputButton.Left)onUnavailable?.Invoke();}
    }
    public sealed class PlateButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Image image;public Outline outline;public RectTransform icon;public bool allowScale=true;public Action onHover;bool focused,hovered,pressed;
        public void SetFocused(bool value){focused=value;Apply();}
        public void OnPointerEnter(PointerEventData eventData){hovered=true;onHover?.Invoke();Apply();}
        public void OnPointerExit(PointerEventData eventData){hovered=false;Apply();}
        public void OnPointerDown(PointerEventData eventData){pressed=true;Apply();}
        public void OnPointerUp(PointerEventData eventData){pressed=false;Apply();}
        void OnDisable(){hovered=pressed=false;}
        void Apply(){bool active=focused||hovered;
            if(image!=null)image.color=pressed?new Color32(148,103,65,255):active?new Color32(163,114,67,255):new Color32(129,87,57,255);
            if(outline!=null)outline.enabled=active||pressed;
            // Retain geometry: feedback comes from fill, rim and a two-pixel icon slide.
            // This also leaves settings rows comfortably inside their wooden container.
            transform.localScale=Vector3.one;
            if(icon!=null)icon.anchoredPosition=new Vector2(active?15:13,icon.anchoredPosition.y);
        }

    }
    public sealed class PlateIcon : MaskableGraphic
    {
        public int kind;
        protected override void OnPopulateMesh(VertexHelper vh){
            vh.Clear();var r=rectTransform.rect;
            Vector3 P(float x,float y)=>new Vector3(r.xMin+x*r.width,r.yMin+y*r.height);
            void Tri(float ax,float ay,float bx,float by,float cx,float cy){int n=vh.currentVertCount;vh.AddVert(P(ax,ay),color,Vector2.zero);vh.AddVert(P(bx,by),color,Vector2.zero);vh.AddVert(P(cx,cy),color,Vector2.zero);vh.AddTriangle(n,n+1,n+2);}
            void Line(float ax,float ay,float bx,float by){Vector2 a=P(ax,ay),b=P(bx,by),d=(b-a).normalized;Vector2 q=new Vector2(-d.y,d.x)*r.width*.065f;int n=vh.currentVertCount;vh.AddVert(a+q,color,Vector2.zero);vh.AddVert(b+q,color,Vector2.zero);vh.AddVert(b-q,color,Vector2.zero);vh.AddVert(a-q,color,Vector2.zero);vh.AddTriangle(n,n+1,n+2);vh.AddTriangle(n,n+2,n+3);}
            if(kind==0){Line(.15f,.5f,.9f,.5f);Line(.15f,.5f,.48f,.82f);Line(.15f,.5f,.48f,.18f);}
            else if(kind==1){Line(.08f,.48f,.38f,.18f);Line(.38f,.18f,.91f,.84f);}
            else if(kind==2)Tri(.2f,.08f,.9f,.5f,.2f,.92f);
            else if(kind==3){
                // Continuous toothed annulus with a clear circular hole, never a dotted ring.
                for(int i=0;i<64;i++){float a=i*Mathf.PI/32,b=(i+1)*Mathf.PI/32;float ra=i%8<4?.46f:.35f,rb=(i+1)%8<4?.46f:.35f;
                    float ax=.5f+Mathf.Cos(a)*ra,ay=.5f+Mathf.Sin(a)*ra,bx=.5f+Mathf.Cos(b)*rb,by=.5f+Mathf.Sin(b)*rb;
                    float cx=.5f+Mathf.Cos(a)*.19f,cy=.5f+Mathf.Sin(a)*.19f,dx=.5f+Mathf.Cos(b)*.19f,dy=.5f+Mathf.Sin(b)*.19f;
                    Tri(ax,ay,bx,by,dx,dy);Tri(ax,ay,dx,dy,cx,cy);
                }
            }
            else if(kind==5){for(int i=0;i<22;i++){float a=(45+i*12)*Mathf.Deg2Rad,b=(57+i*12)*Mathf.Deg2Rad;Line(.5f+Mathf.Cos(a)*.32f,.5f+Mathf.Sin(a)*.32f,.5f+Mathf.Cos(b)*.32f,.5f+Mathf.Sin(b)*.32f);}Line(.73f,.75f,.70f,.43f);Line(.73f,.75f,.99f,.71f);}
            else if(kind==6){Line(.1f,.7f,.88f,.7f);Line(.88f,.7f,.65f,.9f);Line(.88f,.7f,.65f,.5f);Line(.9f,.3f,.12f,.3f);Line(.12f,.3f,.35f,.1f);Line(.12f,.3f,.35f,.5f);}
            else if(kind==7){Line(.08f,.48f,.5f,.89f);Line(.5f,.89f,.92f,.48f);Line(.22f,.5f,.22f,.13f);Line(.22f,.13f,.78f,.13f);Line(.78f,.13f,.78f,.5f);Line(.46f,.13f,.46f,.4f);Line(.46f,.4f,.61f,.4f);}
            else if(kind==8){Line(.2f,.2f,.8f,.8f);Line(.2f,.8f,.8f,.2f);}
            else{Line(.2f,.12f,.2f,.88f);Line(.2f,.88f,.68f,.88f);Line(.2f,.12f,.68f,.12f);Line(.45f,.5f,.96f,.5f);Line(.96f,.5f,.72f,.73f);Line(.96f,.5f,.72f,.27f);}
        }
    }
    public sealed class SlantGraphic : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();var r=rectTransform.rect;float cut=Mathf.Min(46,r.width*.15f);
            vh.AddVert(new Vector3(r.xMin+cut,r.yMax,0),color,new Vector2(0,1));vh.AddVert(new Vector3(r.xMax,r.yMax,0),color,new Vector2(1,1));vh.AddVert(new Vector3(r.xMax-cut,r.yMin,0),color,new Vector2(1,0));vh.AddVert(new Vector3(r.xMin,r.yMin,0),color,new Vector2(0,0));vh.AddTriangle(0,1,2);vh.AddTriangle(2,3,0);
        }
    }
}


