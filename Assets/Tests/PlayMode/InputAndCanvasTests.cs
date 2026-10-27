using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using TMPro;
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
    }
}
