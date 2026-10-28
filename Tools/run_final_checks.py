from pathlib import Path
import subprocess, os, sys, time, shutil, json, datetime, xml.etree.ElementTree as ET
from evidence_fingerprint import source_fingerprint,sha
root=Path(__file__).resolve().parents[1]
project=Path(os.environ.get('ILABAN_PROJECT',root))
unity=Path(r'C:\Program Files\Unity\Hub\Editor\6000.6.2f1\Editor\Unity.exe')
if not unity.exists():raise SystemExit('Unity Editor not found: '+str(unity))
evidence=root/'Docs/QA_EVIDENCE';evidence.mkdir(parents=True,exist_ok=True)
logs=root/'Logs';logs.mkdir(exist_ok=True)
modes=os.environ.get('ILABAN_CHECK_MODES','EditMode,PlayMode,Build').split(',')
for mode in modes:
 if mode in ('EditMode','PlayMode'):
  xml=evidence/(mode+'.xml')
  if xml.exists():shutil.copy2(xml,logs/(mode+'-previous.xml'));xml.unlink()
 args=[str(unity),'-batchmode','-projectPath',str(project),'-logFile',str(logs/('UIRelease-'+mode+'.log'))]
 if mode=='EditMode':args+=['-runTests','-testPlatform','EditMode','-assemblyNames','Ilaban.EditMode.Tests','-testResults',str(evidence/'EditMode.xml')]
 elif mode=='PlayMode':args+=['-runTests','-testPlatform','PlayMode','-assemblyNames','Ilaban.PlayMode.Tests','-testResults',str(evidence/'PlayMode.xml')]
 elif mode=='Rules':args+=['-quit','-executeMethod','Hexbound.Editor.SubmissionSetup.ValidateBaseline']
 elif mode=='Build':args+=['-quit','-executeMethod','Hexbound.Editor.SubmissionSetup.ValidateAndBuild']
 else:raise ValueError('Unknown check mode '+mode)
 print('Running graphics-enabled Unity '+mode+' on '+str(project),flush=True)
 result=subprocess.run(args)
 print(mode,'exit',result.returncode,flush=True)
 if result.returncode:raise SystemExit(result.returncode)
 if mode in ('EditMode','PlayMode'):
  report=ET.parse(xml).getroot();assert report.get('result')=='Passed' and report.get('failed')=='0'
 else:
  assert 'ILABAN VALIDATION PASSED: 65 rubric assertions' in (logs/('UIRelease-'+mode+'.log')).read_text(encoding='utf-8',errors='replace')
 if mode=='Build':
  exe=project/'Builds/Windows/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe'
  (evidence/'build_provenance.json').write_text(json.dumps({'project':str(project.resolve()),'head':subprocess.check_output(['git','rev-parse','HEAD'],cwd=project,text=True).strip(),'utc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'source':source_fingerprint(project),'exe_sha256':sha(exe),'unity':'6000.6.2f1','rules':65},indent=2),encoding='utf-8')
 time.sleep(3) # Let Unity's native data-store/graphics helpers exit before the next editor invocation.
