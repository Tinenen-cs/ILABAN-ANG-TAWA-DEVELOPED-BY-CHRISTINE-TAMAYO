"""Smoke-test delivered bytes from fresh non-project folders, including Downloads."""
from pathlib import Path
import tempfile,shutil,subprocess,json,time,os,hashlib
from PIL import Image
root=Path(__file__).resolve().parents[1];title='ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO'
package=json.loads((root/'Docs/package_verification.json').read_text());a=Path(package['extracted_to'])/title/(title+'.exe')
b=root/'SUBMISSION'/(title+'.exe');fresh=Path(tempfile.mkdtemp(prefix='Ilaban Single File With Spaces '));shutil.copy2(b,fresh/b.name)
downloads=Path.home()/'Downloads/Ilaban Final Portable Test';downloads.mkdir(exist_ok=True);shutil.copy2(b,downloads/b.name)
out=root/'QA_SCREENSHOTS/FINAL';out.mkdir(parents=True,exist_ok=True);results=[]
rejected=json.loads((root/'Docs/single_exe_build.json').read_text()).get('passed') is False
for name,exe in [('A_Extracted_1920x1080',a),('B_Temp_Spaces_1920x1080',fresh/b.name),('B_Downloads_Native_1920x1080',downloads/b.name)]:
    if name.startswith('B_') and rejected:
        results.append({'case':name,'passed':False,'launched':False,'result':'SKIPPED: previously rejected B requires elevation; use A','error':'Historical temp-copy failure: WinError 740. No elevation attempted.'})
        continue
    folder=out/name
    assert root.resolve() in folder.resolve().parents
    if folder.exists():shutil.rmtree(folder)
    folder.mkdir();log=root/'Logs'/(name+'.log');start=time.monotonic()
    try:
        proc=subprocess.Popen([str(exe),'-screen-fullscreen','1','-screen-width','1920','-screen-height','1080','-ilaban-smoke',str(folder),'-logFile',str(log)],cwd=exe.parent)
    except OSError as error:
        if name.startswith('A_'):raise
        results.append({'case':name,'passed':False,'launched':False,'error':str(error)})
        rejected=True
        (root/'Docs/final_delivery_tests.json').write_text(json.dumps(results,indent=2))
        continue
    opening_seconds=None
    while proc.poll() is None and time.monotonic()-start<180:
        if opening_seconds is None and (folder/'00-opening.png').exists():opening_seconds=round(time.monotonic()-start,2)
        time.sleep(.1)
    if proc.poll() is None:proc.kill();raise RuntimeError('Delivery test timed out: '+name)
    text=log.read_text(encoding='utf-8',errors='replace') if log.exists() else 'MISSING LOG'
    issues=[line for line in text.splitlines() if any(x in line for x in ['Exception','MISSING GLYPH','LAYOUT TEXT OVERFLOW','NullReference','error CS','MISSING LOG'])]
    sizes={Image.open(path).size for path in folder.glob('*.png')}
    passed=proc.returncode==0 and (folder/'PASS.txt').exists() and not issues and sizes=={(1920,1080)}
    row={'case':name,'exe':str(exe),'exit':proc.returncode,'passed':passed,'issues':issues,'image_sizes':[list(x) for x in sizes],'seconds_to_opening_capture':opening_seconds,'elapsed_seconds':round(time.monotonic()-start,2),'native_monitor':[1920,1080],'default_launch_check':'separate desktop double-click check','clean_pc':False}
    results.append(row);(root/'Docs/final_delivery_tests.json').write_text(json.dumps(results,indent=2));print(json.dumps(row),flush=True)
    if not passed:raise RuntimeError('Delivery failed: '+name)
(root/'Docs/final_delivery_tests.json').write_text(json.dumps(results,indent=2))
print('Safe ZIP A passed at 1920x1080/native. Check candidate-specific results; rejected B is never represented as passed.')
