from pathlib import Path
import subprocess,sys,json,hashlib
from PIL import Image
r=Path(__file__).resolve().parents[1];step=sys.argv[1];q=r/'QA_SCREENSHOTS/UPDATE3'/('Step'+step);q.mkdir(parents=True,exist_ok=True)
log=r/'Logs'/('UPDATE3-Step'+step+'.log')
p=subprocess.run([str(r/'Builds/Windows/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe'),'-screen-fullscreen','0','-screen-width','1280','-screen-height','720','-ilaban-smoke',str(q),'-logFile',str(log)],timeout=110)
s=log.read_text(encoding='utf-8',errors='replace');issues=[line for line in s.splitlines() if any(t in line for t in ['Exception','NullReference','LAYOUT TEXT OVERFLOW','MISSING GLYPH'])]
assert p.returncode==0 and (q/'PASS.txt').exists() and not issues,(p.returncode,issues)
files=sorted(q.glob('*.png'));assert len(files)==17 and all(Image.open(f).size==(1280,720) for f in files)
(r/'Docs'/('update3_step'+step+'_smoke.json')).write_text(json.dumps({'passed':True,'exit':p.returncode,'issues':issues,'files':[{'file':f.relative_to(r).as_posix(),'sha256':hashlib.sha256(f.read_bytes()).hexdigest()} for f in files]},indent=2),encoding='utf-8')
print('Step'+step+' native 1280 smoke passed: 17 screens, full three-win match, clean logs.')
