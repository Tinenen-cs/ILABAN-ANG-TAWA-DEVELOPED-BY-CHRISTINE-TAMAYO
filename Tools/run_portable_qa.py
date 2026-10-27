from pathlib import Path
import subprocess,shutil,os,json,sys
root=Path.cwd();qa=Path(os.environ.get('ILABAN_PROJECT',os.path.join(os.environ['TEMP'],'Ilaban-Phase2-Validation')));exe=qa/'Builds/Windows/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe'
results=[]
for w,h in [(1280,720),(1366,768),(1920,1080),(2560,1440)]:
 if w==2560:
  subprocess.run([sys.executable,str(root/'Tools/capture_large_window.py')],check=True)
  break
 folder=root/'QA_SCREENSHOTS'/f'Final_{w}x{h}';log=root/'Logs'/f'Final_{w}x{h}.log'
 print('Capturing',w,h,flush=True)
 proc=subprocess.Popen([str(exe),'-screen-fullscreen','0','-screen-width',str(w),'-screen-height',str(h),'-ilaban-smoke',str(folder),'-logFile',str(log)])
 try:code=proc.wait(timeout=100)
 except subprocess.TimeoutExpired:proc.kill();raise RuntimeError('Portable smoke timed out: '+str(folder))
 text=log.read_text(encoding='utf-8',errors='replace')
 issues=[l for l in text.splitlines() if any(k in l for k in ['Exception','MISSING GLYPH','LAYOUT TEXT OVERFLOW','NullReference','error CS'])]
 from PIL import Image
 sizes={Image.open(p).size for p in folder.glob('*.png')}
 passed=code==0 and (folder/'PASS.txt').exists() and not issues and sizes=={(w,h)}
 results.append(dict(resolution=f'{w}x{h}',exit=code,passed=passed,image_sizes=[list(s) for s in sizes],issues=issues))
 (root/'Docs/portable_qa_results.json').write_text(json.dumps(results,indent=2),encoding='utf-8')
 print('Result',passed,'issues',len(issues),'sizes',sizes,flush=True)
 if not passed:raise RuntimeError('Portable QA failed; inspect '+str(log))
print('ALL RESOLUTIONS PASSED',flush=True)
