using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Reflection;
namespace Hexbound.Tests {
    public class Update4Tests : InputTestFixture {
        const BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
        AcademyGame Game(){var old=Object.FindAnyObjectByType<AcademyGame>();if(old!=null)Object.DestroyImmediate(old.gameObject);var g=new GameObject("UPDATE4 flow regression").AddComponent<AcademyGame>();g.enabled=false;g.SetLanguage(0);g.Show(AcademyGame.ScreenState.Select);return g;}
        void Pump(AcademyGame g,bool newFrame=true){typeof(AcademyGame).GetField("screenEnteredFrame",Flags).SetValue(g,-1);if(newFrame){typeof(AcademyGame).GetField("selectionConsumedFrame",Flags).SetValue(g,-1);typeof(AcademyGame).GetField("stageChosenFrame",Flags).SetValue(g,-1);}typeof(AcademyGame).GetMethod("Update",Flags).Invoke(g,null);}
        Button StartButton(AcademyGame g)=>(Button)typeof(AcademyGame).GetField("selectionStartButton",Flags).GetValue(g);
        [Test] public void KeyboardAndGamepadsRequireFreshSimulanAfterBothConfirm(){
            var keys=InputSystem.AddDevice<Keyboard>();var a=InputSystem.AddDevice<Gamepad>();var b=InputSystem.AddDevice<Gamepad>();
            for(int mode=0;mode<2;mode++){
                var g=Game();Assert.IsFalse(StartButton(g).interactable);
                if(mode==0)Press(keys.enterKey);else Press(a.startButton);Pump(g);Assert.IsTrue((bool)typeof(AcademyGame).GetField("errorOpen",Flags).GetValue(g));g.CloseError();
                if(mode==0)Release(keys.enterKey);else Release(a.startButton);Pump(g);
                if(mode==0)Press(keys.fKey);else Press(a.buttonSouth);Pump(g);Assert.IsTrue(g.Ready[0]);Assert.IsFalse(g.Ready[1]);Assert.AreEqual(AcademyGame.ScreenState.Select,g.ScreenNow);Assert.IsFalse(StartButton(g).interactable);
                if(mode==0)Release(keys.fKey);else Release(a.buttonSouth);Pump(g);
                if(mode==0)Press(keys.rightCtrlKey);else Press(b.buttonSouth);Pump(g);Assert.IsTrue(g.Ready[0]&&g.Ready[1]);Assert.AreEqual(AcademyGame.ScreenState.Select,g.ScreenNow);Assert.IsTrue(StartButton(g).interactable);
                Assert.AreEqual("Handa na",((Button[])typeof(AcademyGame).GetField("confirmButtons",Flags).GetValue(g))[1].GetComponentInChildren<TMP_Text>().text);
                StartButton(g).onClick.Invoke();Pump(g,false);Assert.AreEqual(AcademyGame.ScreenState.Select,g.ScreenNow,"The ready-making press/frame is consumed even if Start is invoked on it.");
                if(mode==0)Release(keys.rightCtrlKey);else Release(b.buttonSouth);Pump(g);
                if(mode==0)Press(keys.escapeKey);else Press(b.buttonEast);Pump(g);Assert.AreEqual(AcademyGame.ScreenState.Select,g.ScreenNow);Assert.IsFalse(g.Ready[1]);Assert.IsFalse(StartButton(g).interactable);if(mode==1)Assert.IsTrue(g.Ready[0],"P2 cancel cannot unlock P1.");
                if(mode==0)Release(keys.escapeKey);else Release(b.buttonEast);Pump(g);
                int before=g.Selected[1];if(mode==0)Press(keys.rightArrowKey);else Set(b.leftStick,Vector2.right);Pump(g);Assert.AreEqual((before+1)%8,g.Selected[1],"Unconfirmed fighter can change.");
                if(mode==0)Release(keys.rightArrowKey);else Set(b.leftStick,Vector2.zero);Pump(g);
                if(!g.Ready[0]){if(mode==0)Press(keys.fKey);else Press(a.buttonSouth);Pump(g);if(mode==0)Release(keys.fKey);else Release(a.buttonSouth);Pump(g);}
                if(mode==0)Press(keys.rightCtrlKey);else Press(b.buttonSouth);Pump(g);if(mode==0)Release(keys.rightCtrlKey);else Release(b.buttonSouth);Pump(g);
                if(mode==0)Press(keys.enterKey);else Press(a.startButton);Pump(g);Assert.AreEqual(AcademyGame.ScreenState.StageSelect,g.ScreenNow);Assert.IsNull(g.Model);
                if(mode==0)Release(keys.enterKey);else Release(a.startButton);Pump(g);
                if(mode==0){Press(keys.rightArrowKey);Press(keys.enterKey);}else{Set(a.leftStick,Vector2.right);Press(a.buttonSouth);}Pump(g);Assert.AreEqual(AcademyGame.ScreenState.StageSelect,g.ScreenNow,"Choosing a stage and Confirm on one frame must not start.");Assert.IsNull(g.Model);
                if(mode==0){Release(keys.rightArrowKey);Release(keys.enterKey);}else{Set(a.leftStick,Vector2.zero);Release(a.buttonSouth);}Pump(g);
                if(mode==0)Press(keys.enterKey);else Press(a.buttonSouth);Pump(g);Assert.AreEqual(AcademyGame.ScreenState.Fight,g.ScreenNow);
                if(mode==0)Release(keys.enterKey);else Release(a.buttonSouth);Object.DestroyImmediate(g.gameObject);
            }
        }
        [Test] public void SettingsMigrateOldReactionPreferenceAndHaveNoRowIcons(){
            PlayerPrefs.SetInt("Ilaban.ReactionStyle",1);PlayerPrefs.Save();var g=Game();g.Show(AcademyGame.ScreenState.Controls);
            Assert.IsFalse(PlayerPrefs.HasKey("Ilaban.ReactionStyle"));Assert.AreEqual(LanguageMode.Pormal,g.ReactionLanguage);g.SetReactionStyle(1);Assert.AreEqual(LanguageMode.Pormal,g.ReactionLanguage);Assert.IsFalse(PlayerPrefs.HasKey("Ilaban.ReactionStyle"));
            var rows=(Button[])typeof(AcademyGame).GetField("menuButtons",Flags).GetValue(g);Assert.AreEqual(5,rows.Length);
            for(int i=0;i<4;i++)Assert.IsNull(rows[i].GetComponentInChildren<PlateIcon>());Assert.IsNotNull(rows[4].GetComponentInChildren<PlateIcon>());
            foreach(var text in g.GetComponentsInChildren<TMP_Text>())Assert.IsFalse(text.text.Contains("Wika ng Reaksyon")||text.text.Contains("Kabataan"));
            var bank=new ReactionBank();var pool=System.Array.Find(bank.Data.Pormal,x=>x.category=="taunt").lines;for(int i=0;i<40;i++)Assert.Contains(bank.Choose(0,i%8,"taunt",i+1),pool);
            g.SetLanguage(1);bool music=false,effects=false;foreach(var text in g.GetComponentsInChildren<TMP_Text>()){music|=text.text=="Music (background)";effects|=text.text=="Sound effects";Assert.IsFalse(text.text.Contains("Reaction Style"));}Assert.IsTrue(music&&effects);
            g.SetLanguage(0);Object.DestroyImmediate(g.gameObject);
        }
        [Test] public void VolumeSlidersAffectOnlyTheirOwnMixerCategory(){
            string[] prefs={"Ilaban.Master","Ilaban.Music","Ilaban.Sound"};var has=new bool[3];var values=new float[3];for(int i=0;i<3;i++){has[i]=PlayerPrefs.HasKey(prefs[i]);values[i]=PlayerPrefs.GetFloat(prefs[i]);}float listener=AudioListener.volume;
            var g=Game();try{
                g.Show(AcademyGame.ScreenState.Controls);var sliders=(Slider[])typeof(AcademyGame).GetField("settingsSliders",Flags).GetValue(g);
                var music=(AudioSource)typeof(AcademyGame).GetField("music",Flags).GetValue(g);var effects=(AudioSource)typeof(AcademyGame).GetField("sfx",Flags).GetValue(g);var cues=(AudioSource)typeof(AcademyGame).GetField("highlight",Flags).GetValue(g);
                Assert.AreEqual("Musika",music.outputAudioMixerGroup.name);Assert.AreEqual("Tunog",effects.outputAudioMixerGroup.name);Assert.AreNotSame(music.outputAudioMixerGroup,effects.outputAudioMixerGroup);Assert.AreSame(effects.outputAudioMixerGroup,cues.outputAudioMixerGroup);
                float master=AudioListener.volume,sound=effects.volume,cue=cues.volume;sliders[1].value=.23f;
                Assert.AreEqual(.23f,music.volume,.0001f);Assert.AreEqual(sound,effects.volume);Assert.AreEqual(cue,cues.volume);Assert.AreEqual(master,AudioListener.volume);
                sliders[2].value=.31f;Assert.AreEqual(.31f,effects.volume,.0001f);Assert.AreEqual(.31f*.25f,cues.volume,.0001f);Assert.AreEqual(.23f,music.volume,.0001f);Assert.AreEqual(master,AudioListener.volume);
                sliders[0].value=.42f;Assert.AreEqual(.42f,AudioListener.volume,.0001f);Assert.AreEqual(.23f,music.volume,.0001f);Assert.AreEqual(.31f,effects.volume,.0001f);
            }finally{Object.DestroyImmediate(g.gameObject);for(int i=0;i<3;i++){if(has[i])PlayerPrefs.SetFloat(prefs[i],values[i]);else PlayerPrefs.DeleteKey(prefs[i]);}PlayerPrefs.Save();AudioListener.volume=listener;}
        }
        [Test] public void SettingsKeyboardAndGamepadNavigateFourRowsThenBack(){
            var keys=InputSystem.AddDevice<Keyboard>();var pad=InputSystem.AddDevice<Gamepad>();
            for(int mode=0;mode<2;mode++){
                var g=Game();g.Show(AcademyGame.ScreenState.Controls);
                for(int i=0;i<4;i++){if(mode==0)Press(keys.sKey);else Press(pad.dpad.down);Pump(g);if(mode==0)Release(keys.sKey);else Release(pad.dpad.down);Pump(g);}
                Assert.AreEqual(4,typeof(AcademyGame).GetField("focus",Flags).GetValue(g));if(mode==0)Press(keys.fKey);else Press(pad.buttonSouth);Pump(g);Assert.AreEqual(AcademyGame.ScreenState.Title,g.ScreenNow);
                if(mode==0)Release(keys.fKey);else Release(pad.buttonSouth);Object.DestroyImmediate(g.gameObject);
            }
        }
        [Test] public void MouseConfirmAndStageCardsNeverAutomaticallyStart(){
            var g=Game();var start=StartButton(g);start.GetComponent<UnavailableButtonNotice>().OnPointerClick(new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left});Assert.IsTrue((bool)typeof(AcademyGame).GetField("errorOpen",Flags).GetValue(g));g.CloseError();
            var buttons=(Button[])typeof(AcademyGame).GetField("confirmButtons",Flags).GetValue(g);buttons[0].onClick.Invoke();buttons[1].onClick.Invoke();Assert.IsTrue(g.Ready[0]&&g.Ready[1]);Assert.AreEqual(AcademyGame.ScreenState.Select,g.ScreenNow);
            typeof(AcademyGame).GetField("selectionConsumedFrame",Flags).SetValue(g,-1);start.onClick.Invoke();Assert.AreEqual(AcademyGame.ScreenState.StageSelect,g.ScreenNow);
            var cards=(RectTransform[])typeof(AcademyGame).GetField("cards",Flags).GetValue(g);cards[2].GetComponentInChildren<Button>().onClick.Invoke();Assert.AreEqual(AcademyGame.ScreenState.StageSelect,g.ScreenNow);Assert.IsNull(g.Model);
            var stageStart=System.Array.Find(g.GetComponentsInChildren<Button>(),x=>x.name=="Simulan");stageStart.onClick.Invoke();Assert.IsNull(g.Model,"Stage choice frame is consumed.");typeof(AcademyGame).GetField("stageChosenFrame",Flags).SetValue(g,-1);stageStart.onClick.Invoke();Assert.AreEqual(AcademyGame.ScreenState.Fight,g.ScreenNow);Object.DestroyImmediate(g.gameObject);
        }
    }
}
