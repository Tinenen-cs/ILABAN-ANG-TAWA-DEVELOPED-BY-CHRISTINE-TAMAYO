from pathlib import Path
import subprocess, os, sys
root=Path(__file__).resolve().parents[1]
project=Path(os.environ.get('ILABAN_PROJECT',root))
unity=Path(r'C:\Program Files\Unity\Hub\Editor\6000.6.2f1\Editor\Unity.exe')
if not unity.exists():raise SystemExit('Unity Editor not found: '+str(unity))
evidence=root/'Docs/QA_EVIDENCE';evidence.mkdir(parents=True,exist_ok=True)
logs=root/'Logs';logs.mkdir(exist_ok=True)
for mode in ['EditMode','PlayMode','Build']:
 args=[str(unity),'-batchmode','-projectPath',str(project),'-logFile',str(logs/('UIRelease-'+mode+'.log'))]
 if mode=='EditMode':args+=['-runTests','-testPlatform','EditMode','-assemblyNames','Ilaban.EditMode.Tests','-testResults',str(evidence/'EditMode.xml')]
 elif mode=='PlayMode':args+=['-runTests','-testPlatform','PlayMode','-assemblyNames','Ilaban.PlayMode.Tests','-testResults',str(evidence/'PlayMode.xml')]
 else:args+=['-quit','-executeMethod','Hexbound.Editor.SubmissionSetup.ValidateAndBuild']
 print('Running graphics-enabled Unity '+mode+' on '+str(project),flush=True)
 result=subprocess.run(args)
 print(mode,'exit',result.returncode,flush=True)
 if result.returncode:raise SystemExit(result.returncode)
