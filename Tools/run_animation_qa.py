from pathlib import Path
import os,subprocess,shutil,json,hashlib
from PIL import Image
root=Path(__file__).resolve().parents[1];project=Path(os.environ.get('ILABAN_PROJECT',root))
folder=root/'QA_SCREENSHOTS/ANIMATION/Unity';log=root/'Logs/Animation-Capture.log'
assert root.resolve() in folder.resolve().parents,'Capture cleanup must stay within the project'
if folder.exists():shutil.rmtree(folder)
folder.mkdir(parents=True)
exe=project/'Builds/Windows/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe'
result=subprocess.run([str(exe),'-screen-fullscreen','0','-screen-width','1280','-screen-height','720','-ilaban-animation',str(folder),'-logFile',str(log)],timeout=180)
text=log.read_text(encoding='utf-8',errors='replace');issues=[line for line in text.splitlines() if any(word in line for word in ['Exception','MISSING GLYPH','NullReference','error CS'])]
files=sorted(folder.rglob('*.png'));assert result.returncode==0 and len(files)==172 and (folder/'PASS.txt').exists() and not issues,(result.returncode,len(files),issues)
assert all(Image.open(path).size==(1280,720) for path in files)
(root/'Docs/animation_capture_evidence.json').write_text(json.dumps({'passed':True,'exit':result.returncode,'frames':172,'issues':issues,'files':[{'file':str(path.relative_to(root)),'sha256':hashlib.sha256(path.read_bytes()).hexdigest()} for path in files]},indent=2))
print('PASS: 172 native Unity animation/pair screenshots, no reported exceptions.')
