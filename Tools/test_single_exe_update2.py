"""Run ONLY the single delivered EXE from two independent empty locations."""
from pathlib import Path
import hashlib, json, os, shutil, subprocess, tempfile, time
from PIL import Image

root=Path(__file__).resolve().parents[1]
report=json.loads((root/'Docs/single_exe_build.json').read_text())
source=Path(report['file']);results=[]
assert hashlib.sha256(source.read_bytes()).hexdigest()==report['sha256']
cases=[('EmptyTemp',Path(tempfile.mkdtemp(prefix='IlabanExeOnly-'))),
       ('PathWithSpaces',Path(tempfile.mkdtemp(prefix='Ilaban Test With Spaces '))/'Laro Para Sa Paaralan')]
for case,folder in cases:
    folder.mkdir(parents=True,exist_ok=True)
    assert not list(folder.iterdir()),'The launch directory must start empty'
    exe=folder/source.name;shutil.copy2(source,exe)
    assert len(list(folder.iterdir()))==1,'No external companion assets may be present'
    out=root/'QA_SCREENSHOTS/UPDATE2/SingleExe'/case
    out.mkdir(parents=True,exist_ok=True)
    # These output folders are owned by this test; remove only its known prior captures.
    for path in out.glob('*.png'):path.unlink()
    (out/'PASS.txt').unlink(missing_ok=True)
    log=root/'Logs'/('Update2-SingleExe-'+case+'.log')
    start=time.monotonic()
    proc=subprocess.Popen([str(exe),'-screen-fullscreen','1','-screen-width','1920','-screen-height','1080',
                           '-ilaban-smoke',str(out),'-logFile',str(log)],cwd=folder)
    code=proc.wait(timeout=220)
    text=log.read_text(encoding='utf-8',errors='replace') if log.exists() else 'MISSING LOG'
    issues=[line for line in text.splitlines() if any(x in line for x in ['Exception','MISSING GLYPH','LAYOUT TEXT OVERFLOW','error CS','MISSING LOG'])]
    files=list(out.glob('*.png'));sizes={Image.open(path).size for path in files}
    passed=code==0 and (out/'PASS.txt').exists() and len(files)==17 and sizes=={(1920,1080)} and not issues
    row={'case':case,'folder':str(folder),'launch_directory_files':[p.name for p in folder.iterdir()],
         'exe_sha256':hashlib.sha256(exe.read_bytes()).hexdigest(),'exit':code,'passed':passed,'issues':issues,
         'captures':len(files),'image_sizes':[list(size) for size in sizes],
         'seconds':round(time.monotonic()-start,2),'full_three_win_match':passed,'clean_pc':False}
    results.append(row)
    (root/'Docs/update2_single_exe_tests.json').write_text(json.dumps(results,indent=2),encoding='utf-8')
    print(json.dumps(row),flush=True)
    assert passed,row
report.update(tested=True,passed=all(row['passed'] for row in results),tests='Docs/update2_single_exe_tests.json')
(root/'Docs/single_exe_build.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print('PASS: single EXE alone, both independent folders, 34 native captures and two complete three-win matches.')
