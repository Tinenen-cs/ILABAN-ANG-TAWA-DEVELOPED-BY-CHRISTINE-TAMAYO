"""Embed the verified complete ZIP in an original no-install GUI launcher."""
from pathlib import Path
import datetime, hashlib, json, os, shutil, struct, subprocess, tempfile, zipfile

root=Path(__file__).resolve().parents[1]
title='ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO'
package=json.loads((root/'Docs/package_verification.json').read_text())
archive=Path(package['zip'])
assert package['passed'] and hashlib.sha256(archive.read_bytes()).hexdigest()==package['sha256']
stage=Path(tempfile.mkdtemp(prefix='Ilaban-Launcher-Build-'))
source=(root/'Tools/PortableLauncher.cs').read_text().replace('__PAYLOAD_HASH__',package['sha256'])
(stage/'Launcher.cs').write_text(source,encoding='utf-8')
with zipfile.ZipFile(archive) as z:
    rows=[hashlib.sha256(z.read(n)).hexdigest()+'  '+n for n in z.namelist() if not n.endswith('/')]
(stage/'files.sha256').write_text('\n'.join(rows)+'\n',encoding='utf-8')
output=root/'SUBMISSION'/(title+'.exe')
if output.exists():
    old=root/'Logs/ArchivedPortableCandidates'/datetime.datetime.now(datetime.timezone.utc).strftime('%Y%m%dT%H%M%SZ')
    old.mkdir(parents=True,exist_ok=True);shutil.copy2(output,old/output.name)
compiler=Path(os.environ['WINDIR'])/'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
args=[str(compiler),'/nologo','/target:winexe','/platform:x64','/optimize+','/utf8output',
      '/reference:System.IO.Compression.dll','/reference:System.IO.Compression.FileSystem.dll',
      '/reference:System.Windows.Forms.dll','/reference:System.Drawing.dll',
      '/win32manifest:'+str(root/'Tools/PortableLauncher.manifest'),
      '/resource:'+str(archive)+',game.zip','/resource:'+str(stage/'files.sha256')+',files.sha256',
      '/out:'+str(output),str(stage/'Launcher.cs')]
subprocess.run(args,check=True)
data=output.read_bytes();offset=struct.unpack_from('<I',data,0x3c)[0]
assert struct.unpack_from('<H',data,offset+24+68)[0]==2
report={'file':str(output),'bytes':len(data),'sha256':hashlib.sha256(data).hexdigest(),
        'method':'Original C# GUI launcher with complete verified ZIP resource, temp extraction and per-file SHA verification',
        'payload_sha256':package['sha256'],'payload_files':len(rows),'no_console':True,'requested_execution_level':'asInvoker',
        'requires':'Windows 10/11 x64 with built-in .NET Framework 4.x','tested':False,'passed':False}
(root/'Docs/single_exe_build.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print(json.dumps(report,indent=2))
