using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
namespace Hexbound.Editor
{
    [InitializeOnLoad]
    public static class InputChecks
    {
        static int count;
        static void Check(bool pass,string description){if(!pass)throw new Exception("INPUT FAIL: "+description);count++;}
        static InputChecks(){EditorApplication.update+=RunWhenPlaying;}
        public static void ValidateBatch(){SessionState.SetBool("Ilaban.InputValidation",true);EditorApplication.isPlaying=true;}
        static void RunWhenPlaying(){
            if(!SessionState.GetBool("Ilaban.InputValidation",false)||!EditorApplication.isPlaying||EditorApplication.isCompiling)return;
            SessionState.SetBool("Ilaban.InputValidation",false);
            try{Run();EditorApplication.Exit(0);}catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}
        }
        static void Run()
        {
            count=0;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;var previous=InputSystem.settings.updateMode;InputSystem.settings.updateMode=InputSettings.UpdateMode.ProcessEventsManually;
            var keyboard=InputSystem.AddDevice<Keyboard>();var a=InputSystem.AddDevice<Gamepad>();var b=InputSystem.AddDevice<Gamepad>();Gamepad replacement=null;
            try {using(var input=new AcademyInputManager()) {
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.A,Key.F));InputSystem.Update();input.Poll(0);
                Check(input.Read(0).move<-.9f&&input.Read(0).punch,"P1 keyboard");Check(input.Read(1).move==0&&!input.Read(1).punch,"P1 cannot drive P2");Check(input.Press(0,"Confirm")&&!input.Press(1,"Confirm"),"independent confirmation");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.Update();input.Poll(.1f);
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.RightArrow,Key.RightCtrl));InputSystem.Update();input.Poll(.2f);
                Check(input.Read(1).move>.9f&&input.Read(1).punch,"P2 keyboard");Check(input.Read(0).move==0&&!input.Read(0).punch,"P2 cannot drive P1");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.Update();input.Poll(.3f);
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Enter));InputSystem.Update();input.Poll(.4f);Check(input.Press(0,"Confirm")&&!input.Press(1,"Confirm"),"Enter cannot confirm both players");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.Update();input.Poll(.5f);
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Numpad0));InputSystem.Update();input.Poll(.6f);Check(input.Read(1).punch&&!input.Read(0).punch,"numpad zero isolation");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W,Key.UpArrow,Key.G,Key.R,Key.T));InputSystem.Update();input.Poll(.7f);
                for(int p=0;p<2;p++){var value=input.Read(p);Check(!value.jump&&!value.push&&!value.special&&!value.power&&!value.block,"no forbidden bindings "+p);}
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.QueueStateEvent(a,new GamepadState{leftStick=new Vector2(1,0)}.WithButton(GamepadButton.West));InputSystem.QueueStateEvent(b,new GamepadState{leftStick=new Vector2(-1,0)});InputSystem.Update();input.Poll(1);
                Check(input.Read(0).move>.9f&&input.Read(0).punch,"first controller owns P1");Check(input.Read(1).move<-.9f&&!input.Read(1).punch,"second controller owns P2");
                InputSystem.QueueStateEvent(a,new GamepadState{leftStick=new Vector2(.05f,0)});InputSystem.Update();input.Poll(1.1f);Check(input.Read(0).move==0,"stick deadzone");
                InputSystem.RemoveDevice(a);InputSystem.Update();input.Poll(1.2f);Check(input.ControllerDisconnected,"disconnect detected");Check(input.Read(1).move<-.9f,"remaining pad retains slot");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.D,Key.F));InputSystem.Update();input.Poll(1.3f);Check(input.Read(0).punch&&input.Read(0).move>.9f,"keyboard hot swap");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());replacement=InputSystem.AddDevice<Gamepad>();InputSystem.QueueStateEvent(replacement,new GamepadState().WithButton(GamepadButton.South));InputSystem.Update();input.Poll(1.4f);Check(input.Read(0).punch&&!input.Read(1).punch,"replacement owns vacant slot");
            }} finally {if(a.added)InputSystem.RemoveDevice(a);InputSystem.RemoveDevice(b);InputSystem.RemoveDevice(keyboard);if(replacement!=null)InputSystem.RemoveDevice(replacement);InputSystem.settings.updateMode=previous;}
            Debug.Log("ILABAN INPUT CHECKS PASSED: "+count);
        }
    }
}
