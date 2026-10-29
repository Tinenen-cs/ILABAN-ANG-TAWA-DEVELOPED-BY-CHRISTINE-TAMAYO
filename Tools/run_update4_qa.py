from pathlib import Path
from PIL import Image
import subprocess,os,sys,json,datetime,hashlib
r=Path(__file__).resolve().parents[1];step=sys.argv[1] if len(sys.argv)>1 else 'Final'
sizes=[(1280,720)] if step!='Final' else [(1280,720),(1366,768),(1920,1080),(2560,1440)]
results=[]
for w,h in sizes:
 q=r/'QA_SCREENSHOTS/UPDATE4'/step/f'{w}x{h}';q.mkdir(parents=True,exist_ok=True);log=r/'Logs'/f'UPDATE4-{step}-{w}x{h}.log'
 (r/'RESUME_STATE.txt').write_text('HEAD='+subprocess.check_output(['git','rev-parse','HEAD'],text=True).strip()+f' | UPDATE4 {step}: about to capture native{w}x{h}, confirm-then-Simulan, all screens/full match | UTC='+datetime.datetime.now(datetime.timezone.utc).isoformat(),encoding='utf-8')
 # Borderless sizing avoids Windows work-area clamping at2560x1440.
 exe=r/'Builds/Windows/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe'
 process=subprocess.Popen([str(exe),'-popupwindow','-screen-fullscreen','0','-screen-width',str(w),'-screen-height',str(h),'-ilaban-smoke',str(q),'-logFile',str(log)])
 import ctypes,time
 user=ctypes.windll.user32;callback_type=ctypes.WINFUNCTYPE(ctypes.c_bool,ctypes.c_void_p,ctypes.c_void_p)
 deadline=time.monotonic()+155
 while process.poll() is None and time.monotonic()<deadline:
  def resize(hwnd,arg):
   pid=ctypes.c_ulong();user.GetWindowThreadProcessId(ctypes.c_void_p(hwnd),ctypes.byref(pid))
   if pid.value==process.pid and user.IsWindowVisible(ctypes.c_void_p(hwnd)):
    user.SetWindowLongW(ctypes.c_void_p(hwnd),-16,ctypes.c_long(0x90000000));user.SetWindowPos(ctypes.c_void_p(hwnd),None,0,0,w,h,0x0020|0x0010)
   return True
  user.EnumWindows(callback_type(resize),0);time.sleep(.2)
 if process.poll() is None:process.kill();raise RuntimeError('UPDATE4 QA timeout')
 issues=[line for line in log.read_text(encoding='utf-8',errors='replace').splitlines() if any(t in line for t in ['Exception','NullReference','MISSING GLYPH','LAYOUT TEXT OVERFLOW','error CS'])]
 files=sorted(q.glob('*.png'));passed=process.returncode==0 and (q/'PASS.txt').exists() and len(files)==20 and not issues and all(Image.open(p).size==(w,h) for p in files)
 results.append(dict(resolution=f'{w}x{h}',passed=passed,exit=process.returncode,issues=issues,captures=len(files),files=[dict(file=p.relative_to(r).as_posix(),sha256=hashlib.sha256(p.read_bytes()).hexdigest()) for p in files]))
 (r/'Docs'/f'update4_{step.lower()}_qa.json').write_text(json.dumps(results,indent=2),encoding='utf-8');print(results[-1]['resolution'],passed,len(files),'captures',issues,flush=True)
 assert passed,'Inspect '+str(log)
