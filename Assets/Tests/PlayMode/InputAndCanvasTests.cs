using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
namespace Hexbound.Tests
{
    public class InputAndCanvasTests : InputTestFixture
    {
        [Test] public void KeyboardIsolationAndSimultaneousPunches(){var keys=InputSystem.AddDevice<Keyboard>();using(var input=new AcademyInputManager()){Press(keys.aKey);Press(keys.fKey);input.Poll(1);Assert.Less(input.Read(0).move,0);Assert.IsTrue(input.Read(0).punch);Assert.AreEqual(0,input.Read(1).move);Assert.IsFalse(input.Read(1).punch);Release(keys.fKey);InputSystem.QueueStateEvent(keys,new UnityEngine.InputSystem.LowLevel.KeyboardState(Key.A,Key.F,Key.RightCtrl));InputSystem.Update();input.Poll(2);Assert.IsTrue(input.Read(0).punch);Assert.IsTrue(input.Read(1).punch);}}
        [Test] public void GamepadRemovalReplacementAndGenericJoystick(){var a=InputSystem.AddDevice<Gamepad>();var b=InputSystem.AddDevice<Gamepad>();using(var input=new AcademyInputManager()){Press(a.buttonWest);input.Poll(1);Assert.IsTrue(input.Read(0).punch);Assert.IsFalse(input.Read(1).punch);InputSystem.RemoveDevice(a);input.Poll(2);Assert.IsTrue(input.ControllerDisconnected);var replacement=InputSystem.AddDevice<Gamepad>();Press(replacement.buttonSouth);input.Poll(3);Assert.IsTrue(input.Read(0).punch);Assert.IsFalse(input.Read(1).punch);InputSystem.RemoveDevice(replacement);var joystick=InputSystem.AddDevice<Joystick>();Press(joystick.trigger);input.Poll(4);Assert.IsTrue(input.Read(0).punch);Assert.IsFalse(input.Read(1).punch);}}
        [Test] public void ForbiddenKeysNeverProduceExtraMoves(){var keys=InputSystem.AddDevice<Keyboard>();var pad=InputSystem.AddDevice<Gamepad>();using(var input=new AcademyInputManager()){Press(keys.wKey);Press(keys.upArrowKey);Press(keys.spaceKey);Press(pad.buttonSouth);Press(pad.dpad.up);input.Poll(1);for(int p=0;p<2;p++){var action=input.Read(p);Assert.IsFalse(action.jump||action.push||action.special||action.power||action.block);}}}
        [UnityTest] public IEnumerator ScreensUseOnlyStaticPixelTMPAndReadinessGate(){
            var game=Object.FindAnyObjectByType<AcademyGame>();if(game==null)game=new GameObject("Test game").AddComponent<AcademyGame>();
            game.Show(AcademyGame.ScreenState.Select);yield return null;game.Ready[0]=true;game.Ready[1]=false;game.StartMatch();Assert.AreNotEqual(AcademyGame.ScreenState.Fight,game.ScreenNow);game.Ready[1]=true;game.StartMatch();yield return null;
            Assert.IsNotNull(Object.FindAnyObjectByType<Canvas>());Assert.AreEqual(LanguageMode.Pormal,game.ReactionLanguage);
            foreach(var t in Object.FindObjectsByType<TMP_Text>()){Assert.IsNotNull(t.font);Assert.That(t.font.name=="KalyeDisplay"||t.font.name=="KalyeBody","Unexpected font "+t.font.name);}
            foreach(var t in Object.FindObjectsByType<UnityEngine.UI.Text>())Assert.Fail("Legacy text: "+t.name);
            foreach(var g in Object.FindObjectsByType<Transform>()){string name=g.name.ToLowerInvariant();Assert.IsFalse(name.Contains("health")||name.Contains("stamina")||name.Contains("timer")||name.Contains("knockout"));}
            game.Model.players[1].x=DuelModel.RightRiftX;yield return null;Assert.IsTrue(game.Model.Finished);Assert.AreEqual(0,game.Model.winner);
        }

        [UnityTest] public IEnumerator SettingsAndExitUseTheirOwnScreenFlow(){
            var game=Object.FindAnyObjectByType<AcademyGame>();if(game==null)game=new GameObject("Test game").AddComponent<AcademyGame>();
            game.Show(AcademyGame.ScreenState.Title);yield return null;game.ActivateTitle(1);Assert.AreEqual(AcademyGame.ScreenState.Controls,game.ScreenNow,"Settings selection must remain on settings.");
            game.Show(AcademyGame.ScreenState.Title);game.RequestExit();Assert.AreEqual(AcademyGame.ScreenState.Title,game.ScreenNow,"Exit must open a dialog, not change screens.");
            var focus=typeof(AcademyGame).GetField("focus",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);Assert.AreEqual(1,focus.GetValue(game),"No is the safe default.");game.CancelExit();Assert.AreEqual(AcademyGame.ScreenState.Title,game.ScreenNow);Assert.AreEqual(0,focus.GetValue(game));
            game.Show(AcademyGame.ScreenState.Controls);yield return null;
            var sliders=(System.Array)typeof(AcademyGame).GetField("settingsSliders",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(game);
            foreach(Slider slider in sliders){Assert.AreEqual(290,slider.GetComponent<RectTransform>().sizeDelta.x,.01f,"Slider track stays within its row.");Assert.LessOrEqual(slider.fillRect.rect.width,290.01f,"Slider fill cannot overflow its track.");}
        }
        [UnityTest] public IEnumerator PauseAndSettingsOverlaysHaveNoOrphanFighterObjects(){
            var game=Object.FindAnyObjectByType<AcademyGame>();if(game==null)game=new GameObject("Test game").AddComponent<AcademyGame>();game.Ready[0]=game.Ready[1]=true;game.StartMatch();yield return null;
            var setter=typeof(AcademyGame).GetProperty("Paused").GetSetMethod(true);setter.Invoke(game,new object[]{true});yield return null;
            int[] fighterCount(){int count=0;foreach(var t in Object.FindObjectsByType<Transform>())if(t.name=="Tauhan")count++;return new[]{count};}
            Assert.AreEqual(2,fighterCount()[0],"Only the two expected fighters exist during pause.");game.Show(AcademyGame.ScreenState.Controls);yield return null;yield return null;Assert.AreEqual(0,fighterCount()[0],"Settings must not retain orphan fighter sprites.");
        }
        [UnityTest] public IEnumerator FightHudHasFiveHiloPipsAndNoSeparateItemSprites(){
            var game=Object.FindAnyObjectByType<AcademyGame>();if(game==null)game=new GameObject("Test game").AddComponent<AcademyGame>();game.Ready[0]=game.Ready[1]=true;game.StartMatch();yield return null;
            const System.Reflection.BindingFlags flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
            var pips=(Image[][])typeof(AcademyGame).GetField("hiloPips",flags).GetValue(game);Assert.AreEqual(2,pips.Length);Assert.AreEqual(5,pips[0].Length);Assert.AreEqual(5,pips[1].Length);
            foreach(var text in Object.FindObjectsByType<TMP_Text>()){string value=text.text.ToLowerInvariant();Assert.IsFalse(value.Contains("health")||value.Contains("stamina")||value.Contains("timer")||value.Contains("knockout"),"Forbidden HUD wording: "+text.text);}
            var actors=(System.Array)typeof(AcademyGame).GetField("actors",flags).GetValue(game);foreach(var item in actors){var actorRoot=(RectTransform)item.GetType().GetField("root").GetValue(item);Assert.AreEqual(2,actorRoot.GetComponentsInChildren<RawImage>(true).Length,"Each fighter is a single whole-body image plus ground dust; no separate weapon child.");foreach(Transform child in actorRoot.GetComponentsInChildren<Transform>(true))Assert.IsFalse(child.name.ToLowerInvariant().Contains("item"),"No independent weapon/item object is allowed.");}
            var update=typeof(AcademyGame).GetMethod("Update",flags);for(int count=1;count<=5;count++){game.Model.players[0].streak=count;game.Model.players[0].streakTime=2;update.Invoke(game,null);for(int i=0;i<5;i++)Assert.AreEqual(i<count,pips[0][i].color.r>.5f,"Hilo pips fill in order from one through five.");}
            game.Model.players[0].streak=0;game.Model.players[0].streakTime=0;update.Invoke(game,null);foreach(var pip in pips[0])Assert.Less(pip.color.r,.5f,"The meter resets when the streak breaks.");
        }
        [UnityTest] public IEnumerator EveryCharacterUsesWholeBodyFramesOnOneSharedCanvas(){
            var game=Object.FindAnyObjectByType<AcademyGame>();if(game==null)game=new GameObject("Test game").AddComponent<AcademyGame>();
            const System.Reflection.BindingFlags flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
            var frames=(Texture2D[][][])typeof(AcademyGame).GetField("actionFrames",flags).GetValue(game);
            Assert.AreEqual(8,frames.Length,"All eight characters have action sets.");
            for(int c=0;c<8;c++){Assert.AreEqual(8,frames[c].Length,"Eight whole-body actions per character.");for(int a=0;a<8;a++){Assert.AreEqual(a==2?6:4,frames[c][a].Length,"Action frame count is stable.");foreach(var frame in frames[c][a]){Assert.IsNotNull(frame,"Missing whole-body frame.");Assert.AreEqual(272,frame.width);Assert.AreEqual(512,frame.height);}}}
            game.Show(AcademyGame.ScreenState.Title);yield return null;var actor=typeof(AcademyGame).GetField("actors",flags).GetValue(game);
            Assert.IsNull(actor,"No combat fighter is split into independent body parts on menu screens.");
        }
        [UnityTest] public IEnumerator ReactionBubblesTrackHeadAndStayInSeparateHudSafeLanes(){
            var game=Object.FindAnyObjectByType<AcademyGame>();if(game==null)game=new GameObject("Test game").AddComponent<AcademyGame>();game.Ready[0]=game.Ready[1]=true;game.StartMatch();yield return null;Assert.IsFalse(game.Model.Finished);Assert.AreEqual(AcademyGame.ScreenState.Fight,game.ScreenNow);
            const System.Reflection.BindingFlags flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
            var actors=(System.Array)typeof(AcademyGame).GetField("actors",flags).GetValue(game);var bubbles=(System.Array)typeof(AcademyGame).GetField("bubbles",flags).GetValue(game);float now=(float)typeof(AcademyGame).GetField("clock",flags).GetValue(game);
            for(int i=0;i<2;i++){object bubble=bubbles.GetValue(i);bubble.GetType().GetField("started").SetValue(bubble,now-.2f);}yield return null;
            game.Model.players[0].x=650;game.Model.players[1].x=800;for(int i=0;i<2;i++){object bubble=bubbles.GetValue(i);bubble.GetType().GetField("started").SetValue(bubble,now-.2f);}yield return null;
            var positions=new UnityEngine.Vector2[2];for(int i=0;i<2;i++){object actor=actors.GetValue(i),bubble=bubbles.GetValue(i);var root=(RectTransform)actor.GetType().GetField("root").GetValue(actor);var bubbleRoot=(RectTransform)bubble.GetType().GetField("root").GetValue(bubble);Assert.AreEqual(root.anchoredPosition.y+75,bubbleRoot.anchoredPosition.y,.01f,"Bubble follows head height.");positions[i]=bubbleRoot.anchoredPosition;}
            Assert.Less(positions[0].x+170,positions[1].x,"Close fighters' bubbles stay separated.");
            Assert.GreaterOrEqual(positions[0].x,8,"Bubble stays inside left edge.");Assert.LessOrEqual(positions[1].x+170,1272,"Bubble stays inside right edge.");
            game.Model.players[0].x=410;game.Model.players[1].x=850;yield return null;
            for(int i=0;i<2;i++){object bubble=bubbles.GetValue(i);var bubbleRoot=(RectTransform)bubble.GetType().GetField("root").GetValue(bubble);Assert.AreEqual(game.Model.players[i].x-85,bubbleRoot.anchoredPosition.x,1,"Separated bubble tracks its fighter.");}
        }
    }
}
