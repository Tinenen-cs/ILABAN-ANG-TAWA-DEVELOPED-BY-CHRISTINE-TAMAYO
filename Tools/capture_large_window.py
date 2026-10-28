"""Request an exact client size without changing the user's monitor mode.
Windows can clamp decorated Unity windows to the work area; a borderless
window may extend past it. Screenshots still come from Unity's framebuffer.
"""
import ctypes, subprocess, os, time, json, shutil
from pathlib import Path
from PIL import Image
root=Path(__file__).resolve().parents[1]
exe=Path(os.environ.get('ILABAN_PROJECT',root))/'Builds/Windows/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe'
width=int(os.environ.get('ILABAN_CAPTURE_WIDTH','2560'));height=int(os.environ.get('ILABAN_CAPTURE_HEIGHT','1440'))
folder=root/'QA_SCREENSHOTS'/f'Exact_{width}x{height}';log=root/'Logs'/f'Exact_{width}x{height}.log'
assert root.resolve() in folder.resolve().parents,'Capture cleanup must stay within the project'
if folder.exists():shutil.rmtree(folder)
folder.mkdir(parents=True)
proc=subprocess.Popen([str(exe),'-popupwindow','-screen-fullscreen','0','-screen-width',str(width),'-screen-height',str(height),'-ilaban-smoke',str(folder),'-logFile',str(log)])
user=ctypes.windll.user32
callback_type=ctypes.WINFUNCTYPE(ctypes.c_bool,ctypes.c_void_p,ctypes.c_void_p)
deadline=time.monotonic()+150
while proc.poll() is None and time.monotonic()<deadline:
    def resize(hwnd,arg):
        pid=ctypes.c_ulong();user.GetWindowThreadProcessId(ctypes.c_void_p(hwnd),ctypes.byref(pid))
        if pid.value==proc.pid and user.IsWindowVisible(ctypes.c_void_p(hwnd)):
            user.SetWindowLongW(ctypes.c_void_p(hwnd),-16,ctypes.c_long(0x90000000))
            user.SetWindowPos(ctypes.c_void_p(hwnd),None,0,0,width,height,0x0020|0x0010)
        return True
    user.EnumWindows(callback_type(resize),0)
    time.sleep(.2)
if proc.poll() is None:proc.kill();raise RuntimeError('timeout')
sizes={Image.open(p).size for p in folder.glob('*.png')}
issues=[l for l in log.read_text(encoding='utf-8',errors='replace').splitlines() if any(s in l for s in ['Exception','MISSING GLYPH','LAYOUT TEXT OVERFLOW'])]
passed=proc.returncode==0 and (folder/'PASS.txt').exists() and sizes=={(width,height)} and not issues
result=dict(resolution=f'{width}x{height}',exit=proc.returncode,passed=passed,image_sizes=[list(s) for s in sizes],issues=issues,method='Borderless window exact client sizing; Unity framebuffer capture',folder=str(folder))
report=root/'Docs/portable_qa_results.json';results=[r for r in json.loads(report.read_text()) if r['resolution']!=f'{width}x{height}'];results.append(result);report.write_text(json.dumps(results,indent=2))
print(json.dumps(result,indent=2),flush=True)
assert passed
