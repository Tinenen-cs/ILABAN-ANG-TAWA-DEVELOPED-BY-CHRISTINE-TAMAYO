from pathlib import Path
import subprocess,os
root=Path.cwd();project=Path(os.environ['TEMP'])/'Ilaban-Phase2-Validation';unity=r'C:\Program Files\Unity\Hub\Editor\6000.6.2f1\Editor\Unity.exe'
startup=subprocess.STARTUPINFO();startup.dwFlags|=subprocess.STARTF_USESHOWWINDOW;startup.wShowWindow=0
for mode in ['EditMode','PlayMode','Build']:
 args=[unity,'-nographics','-batchmode','-projectPath',str(project),'-logFile',str(root/'Logs'/('Verified-'+mode+'.log'))]
 if mode=='Build':args+=['-quit','-executeMethod','Hexbound.Editor.FinalizationSetup.ValidateAndBuild']
 else:args+=['-runTests','-testPlatform',mode,'-assemblyNames','Ilaban.'+mode+'.Tests','-testResults',str(root/'Logs'/('Verified-'+mode+'.xml'))]
 print('Running',mode,flush=True)
 result=subprocess.run(args,startupinfo=startup)
 print(mode,'exit',result.returncode,flush=True)
 if result.returncode:raise SystemExit(result.returncode)
