using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Hexbound
{
    public sealed class AcademyInputManager : IDisposable
    {
        readonly InputActionAsset asset;
        readonly InputActionMap[] maps=new InputActionMap[2];
        readonly InputDevice[] pads=new InputDevice[2];
        readonly bool[] used=new bool[2];
        readonly Vector2Int[] previous=new Vector2Int[2],navigation=new Vector2Int[2];
        readonly float[] repeatAt=new float[2];
        Keyboard keyboard;
        bool pendingDisconnect;
        public bool ControllerDisconnected {get;private set;}
        public AcademyInputManager() {
            asset=UnityEngine.Object.Instantiate(Resources.Load<InputActionAsset>("Input/AcademyControls"));
            for(int p=0;p<2;p++) maps[p]=asset.FindActionMap("Player"+(p+1),true);
            Refresh(); asset.Enable();InputSystem.onDeviceChange+=DeviceChanged;
        }
        void DeviceChanged(InputDevice device,InputDeviceChange change){if(change==InputDeviceChange.Added||change==InputDeviceChange.Removed||change==InputDeviceChange.Disconnected||change==InputDeviceChange.Reconnected)Refresh();}
        void Refresh() {
            bool changed=keyboard!=Keyboard.current; keyboard=Keyboard.current;
            for(int p=0;p<2;p++) if(pads[p]!=null && !pads[p].added) { pads[p]=null; used[p]=false; pendingDisconnect=true; changed=true; }
            foreach(var pad in InputSystem.devices) {
                if(!(pad is Gamepad) && !(pad is Joystick)) continue;
                if(pad==pads[0] || pad==pads[1]) continue;
                for(int p=0;p<2;p++) if(pads[p]==null) {pads[p]=pad;changed=true;break;}
            }
            if(changed || maps[0].devices==null) for(int p=0;p<2;p++) {
                var devices=new List<InputDevice>(); if(keyboard!=null) devices.Add(keyboard); if(pads[p]!=null) devices.Add(pads[p]);
                maps[p].devices=devices.ToArray();
            }
        }
        public void Poll(float now) {
            Refresh();ControllerDisconnected=pendingDisconnect;pendingDisconnect=false;
            for(int p=0;p<2;p++) {
                foreach(var a in maps[p].actions) if(a.activeControl!=null && (a.WasPressedThisFrame() || a.name=="Move" && a.ReadValue<Vector2>().sqrMagnitude>.1f)) used[p]=a.activeControl.device is Gamepad || a.activeControl.device is Joystick;
                Vector2 move=maps[p]["Move"].ReadValue<Vector2>();
                var dir=new Vector2Int(Mathf.Abs(move.x)>.5f?(int)Mathf.Sign(move.x):0,Mathf.Abs(move.y)>.5f?(int)Mathf.Sign(move.y):0);
                navigation[p]=Vector2Int.zero;
                if(dir!=Vector2Int.zero && (dir!=previous[p] || now>=repeatAt[p])) {navigation[p]=dir;repeatAt[p]=now+(dir!=previous[p]?.32f:.15f);}
                previous[p]=dir;
            }
        }
        public Vector2Int Navigate(int p)=>navigation[p];
        public bool Press(int p,string action)=>maps[p].FindAction(action)?.WasPressedThisFrame()??false;
        public bool Any(string action)=>Press(0,action)||Press(1,action);
        public bool StartPressed {
            get {
                if(Keyboard.current!=null) foreach(var key in Keyboard.current.allKeys) if(key.wasPressedThisFrame) return true;
                return Any("Confirm")||Any("Punch")||Any("Pause")||Any("Cancel")||(Mouse.current!=null && Mouse.current.leftButton.wasPressedThisFrame);
            }
        }
        public DuelInput Read(int p)=>new DuelInput {move=maps[p]["Move"].ReadValue<Vector2>().x,punch=Press(p,"Punch")};
        public string DeviceName(int p)=>used[p]?"Mando":"Teklado";
        public string Prompt(int p,string action) {
            if(used[p]) return action=="Move"?"Palanca / mga palaso":action=="Punch"?"A / X":action=="Confirm"?"A / X":action=="Cancel"?"B":"Pindutang paghinto";
            return action=="Move"?(p==0?"A / D":"Mga palaso"):action=="Punch"||action=="Confirm"?(p==0?"F":"Ctrl kanan / 0"):"ESC";
        }
        public void Dispose() {InputSystem.onDeviceChange-=DeviceChanged;asset.Disable();UnityEngine.Object.Destroy(asset);}
    }
}
