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
            game.Show(AcademyGame.ScreenState.Select);yield return null;
            var selectionCards=(RectTransform[])typeof(AcademyGame).GetField("cards",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(game);
            Rect bounds(RectTransform r){var corners=new Vector3[4];r.GetWorldCorners(corners);return new Rect(corners[0],corners[2]-corners[0]);}
            for(int a=0;a<selectionCards.Length;a++)for(int b=a+1;b<selectionCards.Length;b++)Assert.IsFalse(bounds(selectionCards[a]).Overlaps(bounds(selectionCards[b])),"Selected character cards must not overlap.");
            game.Ready[0]=true;game.Ready[1]=false;game.StartMatch();Assert.AreNotEqual(AcademyGame.ScreenState.Fight,game.ScreenNow);game.Ready[1]=true;game.StartMatch();yield return null;
            Assert.IsNotNull(Object.FindAnyObjectByType<Canvas>());Assert.AreEqual(LanguageMode.Pormal,game.ReactionLanguage);
            foreach(var t in Object.FindObjectsByType<TMP_Text>()){Assert.IsNotNull(t.font);Assert.That(t.font.name=="KalyeDisplay"||t.font.name=="KalyeBody","Unexpected font "+t.font.name);}
            foreach(var t in Object.FindObjectsByType<UnityEngine.UI.Text>())Assert.Fail("Legacy text: "+t.name);
            foreach(var g in Object.FindObjectsByType<Transform>()){string name=g.name.ToLowerInvariant();Assert.IsFalse(name.Contains("health")||name.Contains("stamina")||name.Contains("timer")||name.Contains("knockout"));}
            game.Model.players[1].x=DuelModel.RightRiftX;yield return null;Assert.IsTrue(game.Model.Finished);Assert.AreEqual(0,game.Model.winner);
            var impacts=(TMP_Text[])typeof(AcademyGame).GetField("impactLabels",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(game);foreach(var impact in impacts)Assert.IsFalse(impact.gameObject.activeSelf,"Combat impact sparkles must not remain frozen over the result screen.");
        }

        [Test] public void TitleRoutesKeyboardGamepadAndPointerWithoutAnyClickStart(){
            var keys=InputSystem.AddDevice<Keyboard>();var pad=InputSystem.AddDevice<Gamepad>();var mouse=InputSystem.AddDevice<Mouse>();
            var previousGame=Object.FindAnyObjectByType<AcademyGame>();if(previousGame!=null)Object.DestroyImmediate(previousGame.gameObject);var game=new GameObject("Routing regression").AddComponent<AcademyGame>();game.enabled=false;
            const System.Reflection.BindingFlags flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
            var focus=typeof(AcademyGame).GetField("focus",flags);var update=typeof(AcademyGame).GetMethod("Update",flags);
            for(int device=0;device<2;device++)for(int option=1;option<=2;option++){
                game.Show(AcademyGame.ScreenState.Title);typeof(AcademyGame).GetField("screenEnteredFrame",flags).SetValue(game,-1);typeof(AcademyGame).GetField("screenEnteredFrame",flags).SetValue(game,-1);focus.SetValue(game,option);
                if(device==0)Press(keys.fKey);else Press(pad.buttonSouth);update.Invoke(game,null);
                Assert.AreEqual(option==1?AcademyGame.ScreenState.Controls:AcademyGame.ScreenState.Title,game.ScreenNow,"Device="+device+", option="+option+", key="+keys.fKey.isPressed+", frame="+Time.frameCount);
                if(option==2){Assert.IsTrue((bool)typeof(AcademyGame).GetField("exitOpen",flags).GetValue(game));Assert.AreEqual(1,focus.GetValue(game),"Hindi is default.");game.CancelExit();}
                if(device==0)Release(keys.fKey);else Release(pad.buttonSouth);typeof(AcademyGame).GetField("screenEnteredFrame",flags).SetValue(game,-1);
            }
            game.Show(AcademyGame.ScreenState.Title);typeof(AcademyGame).GetField("screenEnteredFrame",flags).SetValue(game,-1);Press(mouse.leftButton);update.Invoke(game,null);
            Assert.AreEqual(AcademyGame.ScreenState.Title,game.ScreenNow,"A generic pointer press must not start selection before onClick.");Release(mouse.leftButton);
            var buttons=(Button[])typeof(AcademyGame).GetField("menuButtons",flags).GetValue(game);buttons[1].onClick.Invoke();update.Invoke(game,null);
            Assert.AreEqual(AcademyGame.ScreenState.Controls,game.ScreenNow,"Pointer settings callback owns its press.");
            game.Show(AcademyGame.ScreenState.Title);typeof(AcademyGame).GetField("screenEnteredFrame",flags).SetValue(game,-1);buttons=(Button[])typeof(AcademyGame).GetField("menuButtons",flags).GetValue(game);buttons[2].onClick.Invoke();
            Assert.IsTrue((bool)typeof(AcademyGame).GetField("exitOpen",flags).GetValue(game));game.CancelExit();game.enabled=true;
        }
        [Test] public void ValidationPopupsTrapKeyboardAndGamepadAndCoverInvalidActions(){
            var keys=InputSystem.AddDevice<Keyboard>();var pad=InputSystem.AddDevice<Gamepad>();var previousGame=Object.FindAnyObjectByType<AcademyGame>();if(previousGame!=null)Object.DestroyImmediate(previousGame.gameObject);var game=new GameObject("Modal regression").AddComponent<AcademyGame>();game.enabled=false;
            const System.Reflection.BindingFlags flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;var type=typeof(AcademyGame);var open=type.GetField("errorOpen",flags);var frame=type.GetField("screenEnteredFrame",flags);var update=type.GetMethod("Update",flags);
            game.Show(AcademyGame.ScreenState.Select);game.Ready[0]=game.Ready[1]=false;game.TryStartSelection();Assert.IsTrue((bool)open.GetValue(game));frame.SetValue(game,-1);Press(keys.fKey);update.Invoke(game,null);Assert.IsFalse((bool)open.GetValue(game));Assert.IsFalse(game.Ready[0]||game.Ready[1],"Dismissal must not confirm a fighter underneath.");Release(keys.fKey);
            game.Ready[0]=true;game.TryStartSelection();Assert.IsTrue((bool)open.GetValue(game));frame.SetValue(game,-1);Press(pad.buttonSouth);update.Invoke(game,null);Assert.IsFalse((bool)open.GetValue(game));Assert.IsFalse(game.Ready[1]);Release(pad.buttonSouth);
            game.SelectArena(-1);Assert.IsTrue((bool)open.GetValue(game));game.CloseError();game.Show(AcademyGame.ScreenState.Controls);game.SetLanguage(2);Assert.IsTrue((bool)open.GetValue(game));game.CloseError();game.ActivateResult(0);Assert.IsTrue((bool)open.GetValue(game));game.CloseError();game.enabled=true;
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
        [UnityTest] public IEnumerator PauseHasFiveOptionsAndExitNeverQuitsOnEscape(){
            var game=Object.FindAnyObjectByType<AcademyGame>();if(game==null)game=new GameObject("Pause test").AddComponent<AcademyGame>();game.Ready[0]=game.Ready[1]=true;game.StartMatch();
            typeof(AcademyGame).GetProperty("Paused").GetSetMethod(true).Invoke(game,new object[]{true});yield return null;
            const System.Reflection.BindingFlags flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
            var buttons=(Button[])typeof(AcademyGame).GetField("menuButtons",flags).GetValue(game);Assert.AreEqual(5,buttons.Length);
            game.ActivatePause(4);Assert.IsTrue((bool)typeof(AcademyGame).GetField("exitOpen",flags).GetValue(game));Assert.AreEqual(1,typeof(AcademyGame).GetField("focus",flags).GetValue(game));game.CancelExit();Assert.IsTrue(game.Paused);
            game.ActivatePause(1);Assert.AreEqual(AcademyGame.ScreenState.Controls,game.ScreenNow);Assert.IsTrue(game.Paused);
            game.Ready[0]=game.Ready[1]=true;game.StartMatch();game.ActivatePause(2);Assert.AreEqual(AcademyGame.ScreenState.Select,game.ScreenNow);Assert.IsFalse(game.Ready[0]||game.Ready[1]);
        }
        [UnityTest] public IEnumerator PauseAndSettingsOverlaysHaveNoOrphanFighterObjects(){
            var game=Object.FindAnyObjectByType<AcademyGame>();if(game==null)game=new GameObject("Test game").AddComponent<AcademyGame>();game.Ready[0]=game.Ready[1]=true;game.StartMatch();
            var setter=typeof(AcademyGame).GetProperty("Paused").GetSetMethod(true);setter.Invoke(game,new object[]{true});yield return null;
            var actors=(System.Array)typeof(AcademyGame).GetField("actors",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(game);for(int p=0;p<2;p++){var root=(RectTransform)actors.GetValue(p).GetType().GetField("root").GetValue(actors.GetValue(p));Assert.AreEqual(new Vector2(game.Model.players[p].x,-598),root.anchoredPosition,"Pausing before the first fight update must preserve on-stage fighter placement.");}
            int[] fighterCount(){int count=0;foreach(var t in Object.FindObjectsByType<Transform>())if(t.name=="Tauhan")count++;return new[]{count};}
            Assert.AreEqual(2,fighterCount()[0],"Only the two expected fighters exist during pause.");game.Show(AcademyGame.ScreenState.Controls);yield return null;yield return null;Assert.AreEqual(0,fighterCount()[0],"Settings must not retain orphan fighter sprites.");
        }
        [UnityTest] public IEnumerator FightHudHasFiveHiloPipsAndNoSeparateItemSprites(){
            var game=Object.FindAnyObjectByType<AcademyGame>();if(game==null)game=new GameObject("Test game").AddComponent<AcademyGame>();game.Ready[0]=game.Ready[1]=true;game.StartMatch();yield return null;
            const System.Reflection.BindingFlags flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
            var pips=(Image[][])typeof(AcademyGame).GetField("hiloPips",flags).GetValue(game);Assert.AreEqual(2,pips.Length);Assert.AreEqual(5,pips[0].Length);Assert.AreEqual(5,pips[1].Length);
            foreach(var text in Object.FindObjectsByType<TMP_Text>()){string value=text.text.ToLowerInvariant();Assert.IsFalse(value.Contains("health")||value.Contains("stamina")||value.Contains("timer")||value.Contains("knockout")||value.Contains("k.o.")||value.Contains("sunod na suntok"),"Forbidden HUD wording: "+text.text);}
            Rect localBox(RectTransform r)=>new Rect(r.anchoredPosition.x,r.anchoredPosition.y-r.sizeDelta.y,r.sizeDelta.x,r.sizeDelta.y);
            for(int p=0;p<2;p++){
                var label=pips[p][0].transform.parent.GetComponentInChildren<TMP_Text>();Assert.AreEqual("Hilo",label.text,"Both players must have a visible Hilo label.");
                foreach(var pip in pips[p]){Assert.IsFalse(localBox(label.rectTransform).Overlaps(localBox(pip.rectTransform)),"A Hilo segment must not cover its label.");Assert.Less(pip.color.r,.5f,"Both meters start empty.");}
            }
            var actors=(System.Array)typeof(AcademyGame).GetField("actors",flags).GetValue(game);foreach(var item in actors){var actorRoot=(RectTransform)item.GetType().GetField("root").GetValue(item);Assert.AreEqual(new Vector2(.5f,0),actorRoot.pivot,"The complete fighter rotates and scales around its fixed feet-center pivot.");Assert.AreEqual(2,actorRoot.GetComponentsInChildren<RawImage>(true).Length,"Each fighter is a single whole-body image plus ground dust; no separate weapon child.");foreach(Transform child in actorRoot.GetComponentsInChildren<Transform>(true))Assert.IsFalse(child.name.ToLowerInvariant().Contains("item"),"No independent weapon/item object is allowed.");}
            var update=typeof(AcademyGame).GetMethod("Update",flags);for(int count=1;count<=5;count++){for(int p=0;p<2;p++){game.Model.players[p].streak=count;game.Model.players[p].streakTime=2;}update.Invoke(game,null);for(int p=0;p<2;p++)for(int i=0;i<5;i++)Assert.AreEqual(i<count,pips[p][i].color.r>.5f,"Both Hilo meters fill in order from one through five.");}
            for(int p=0;p<2;p++){game.Model.players[p].streak=0;game.Model.players[p].streakTime=0;}update.Invoke(game,null);foreach(var playerPips in pips)foreach(var pip in playerPips)Assert.Less(pip.color.r,.5f,"The meter resets when the streak breaks.");
        }
        [UnityTest] public IEnumerator EveryCharacterUsesWholeBodyFramesOnOneSharedCanvas(){
            var game=Object.FindAnyObjectByType<AcademyGame>();if(game==null)game=new GameObject("Test game").AddComponent<AcademyGame>();
            const System.Reflection.BindingFlags flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
            var frames=(Texture2D[][][])typeof(AcademyGame).GetField("actionFrames",flags).GetValue(game);
            Assert.AreEqual(8,frames.Length,"All eight characters have action sets.");
            for(int c=0;c<8;c++){Assert.AreEqual(8,frames[c].Length,"Eight whole-body actions per character.");for(int a=0;a<8;a++){Assert.AreEqual(a==2?6:4,frames[c][a].Length,"Action frame count is stable.");foreach(var frame in frames[c][a]){Assert.IsNotNull(frame,"Missing whole-body frame.");Assert.AreEqual(896,frame.width);Assert.AreEqual(512,frame.height);}}}
            Assert.AreEqual(3,AcademyGame.StrikeFrame(DuelModel.PunchStartup),"The fully extended strike must coincide with the unchanged hit event.");
            game.Show(AcademyGame.ScreenState.Title);yield return null;var actor=typeof(AcademyGame).GetField("actors",flags).GetValue(game);
            Assert.IsNull(actor,"No combat fighter is split into independent body parts on menu screens.");
        }
        [UnityTest] public IEnumerator CharacterReactionCuesWaitForMenuAudio(){
            var game=Object.FindAnyObjectByType<AcademyGame>();if(game==null)game=new GameObject("Test game").AddComponent<AcademyGame>();game.Show(AcademyGame.ScreenState.Select);yield return null;
            const System.Reflection.BindingFlags flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
            var type=typeof(AcademyGame);var sound=type.GetMethod("Sound",flags);var queue=type.GetMethod("QueueCharacterCue",flags);var play=type.GetMethod("UpdateCharacterCue",flags);var clock=type.GetField("clock",flags);float before=(float)clock.GetValue(game);
            sound.Invoke(game,new object[]{"menu_move"});queue.Invoke(game,new object[]{2});play.Invoke(game,null);
            Assert.AreEqual(2,type.GetField("queuedCharacter",flags).GetValue(game),"Reaction remains queued while a menu sound is active.");
            float until=(float)type.GetField("menuSoundUntil",flags).GetValue(game);clock.SetValue(game,until+.05f);play.Invoke(game,null);
            var source=(AudioSource)type.GetField("highlight",flags).GetValue(game);Assert.AreEqual("reaction_2",source.clip.name);Assert.AreEqual(-1,type.GetField("queuedCharacter",flags).GetValue(game));
            sound.Invoke(game,new object[]{"menu_back"});Assert.IsFalse(source.isPlaying,"A new menu action takes priority over a character blip.");clock.SetValue(game,before);game.Show(AcademyGame.ScreenState.Title);
        }
        [UnityTest] public IEnumerator ReactionBubblesTrackHeadAndStayInSeparateHudSafeLanes(){
            var game=Object.FindAnyObjectByType<AcademyGame>();if(game==null)game=new GameObject("Test game").AddComponent<AcademyGame>();game.Ready[0]=game.Ready[1]=true;game.StartMatch();yield return null;Assert.IsFalse(game.Model.Finished);Assert.AreEqual(AcademyGame.ScreenState.Fight,game.ScreenNow);
            const System.Reflection.BindingFlags flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
            var actors=(System.Array)typeof(AcademyGame).GetField("actors",flags).GetValue(game);var bubbles=(System.Array)typeof(AcademyGame).GetField("bubbles",flags).GetValue(game);float now=(float)typeof(AcademyGame).GetField("clock",flags).GetValue(game);
            for(int i=0;i<2;i++){object bubble=bubbles.GetValue(i);bubble.GetType().GetField("started").SetValue(bubble,now-.2f);}yield return null;
            game.Model.players[0].x=650;game.Model.players[1].x=720;for(int i=0;i<2;i++){object bubble=bubbles.GetValue(i);bubble.GetType().GetField("started").SetValue(bubble,now-.2f);}yield return null;
            var positions=new UnityEngine.Vector2[2];for(int i=0;i<2;i++){
                object actor=actors.GetValue(i),bubble=bubbles.GetValue(i);var head=(RectTransform)actor.GetType().GetField("headTop").GetValue(actor);var bubbleRoot=(RectTransform)bubble.GetType().GetField("root").GetValue(bubble);var tail=(RectTransform)bubble.GetType().GetField("tail").GetValue(bubble);
                Vector2 headPoint=((RectTransform)bubbleRoot.parent).InverseTransformPoint(head.position);positions[i]=bubbleRoot.anchoredPosition;
                Assert.AreEqual(Mathf.Clamp(headPoint.x-80,8,1112),positions[i].x,.1f,"Each bubble stays centered on its own animated head; close fighters use vertical separation.");
                Assert.AreEqual(headPoint.x,positions[i].x+tail.anchoredPosition.x,.1f,"The short tail points at its own head.");Assert.Greater(positions[i].y-60,headPoint.y,"Bubble and tail remain above the head.");Assert.LessOrEqual(positions[i].y,-158,"Bubbles stay below the HUD.");Assert.GreaterOrEqual(positions[i].x,8);Assert.LessOrEqual(positions[i].x+160,1272);
            }
            Assert.IsFalse(new Rect(positions[0].x,positions[0].y-60,160,60).Overlaps(new Rect(positions[1].x,positions[1].y-60,160,60)),"Close fighters' bubbles and tails cannot overlap.");
            game.Model.players[0].x=410;game.Model.players[1].x=850;yield return null;
            for(int i=0;i<2;i++){object actor=actors.GetValue(i),bubble=bubbles.GetValue(i);var head=(RectTransform)actor.GetType().GetField("headTop").GetValue(actor);var bubbleRoot=(RectTransform)bubble.GetType().GetField("root").GetValue(bubble);Vector2 point=((RectTransform)bubbleRoot.parent).InverseTransformPoint(head.position);Assert.AreEqual(point.x-80,bubbleRoot.anchoredPosition.x,.1f,"Separated bubble follows its own head.");Assert.AreEqual(point.y+66,bubbleRoot.anchoredPosition.y,.1f);}
        }
    }
}
