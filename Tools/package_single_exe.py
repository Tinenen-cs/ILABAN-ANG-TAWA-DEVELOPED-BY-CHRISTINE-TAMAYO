"""Free SDK SFX: temporary extraction, visible progress, exact game launch, no installer."""
from pathlib import Path
import os,subprocess,json,hashlib,struct,tempfile,shutil
root=Path(__file__).resolve().parents[1];title='ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO'
sdk=Path(os.environ['TEMP'])/'Ilaban-PackTools/SDK';module=sdk/'bin/7zSD.sfx';archiver=sdk/'bin/7zr.exe'
assert module.exists() and archiver.exists(),'Download official LZMA SDK as documented first'
package=json.loads((root/'Docs/package_verification.json').read_text());assert package['passed']
source=Path(package['extracted_to'])/title;assert (source/(title+'.exe')).exists()
stage=Path(tempfile.mkdtemp(prefix='Ilaban-SFX-Source-'));shutil.copytree(source,stage,dirs_exist_ok=True)
license=(sdk/'DOC/lzma-sdk.txt').read_text(encoding='utf-8');license=license[license.index('LICENSE'):license.index('LZMA SDK Contents')]
(stage/'LZMA_SDK_LICENSE.txt').write_text(license,encoding='utf-8')
output=root/'SUBMISSION'/(title+'.exe');archive=Path(os.environ['TEMP'])/'Ilaban-PackTools/game.7z'
if archive.exists():archive.unlink()
subprocess.run([str(archiver),'a','-t7z',str(archive),'.','-mx=1','-m0=LZMA','-ms=16m'],cwd=stage,check=True,stdout=subprocess.DEVNULL)
# RunProgram receives additional command-line arguments (official SfxSetup.cpp).
config=(' ;!@Install@!UTF-8!\n'.lstrip()+'Title="'+title+'"\nProgress="yes"\nDirectory=""\nRunProgram="\\"%%T\\\\'+title+'.exe\\""\n;!@InstallEnd@!\n').encode('utf-8')
with output.open('wb') as stream:
    stream.write(module.read_bytes());stream.write(config)
    with archive.open('rb') as src:shutil.copyfileobj(src,stream)
data=output.read_bytes();offset=struct.unpack_from('<I',data,0x3c)[0]
assert struct.unpack_from('<H',data,offset+24+68)[0]==2,'SFX must be a GUI binary, no console'
result={'file':str(output),'bytes':len(data),'sha256':hashlib.sha256(data).hexdigest(),'sdk':'LZMA SDK 26.05','sdk_url':'https://www.7-zip.org/sdk.html','module_sha256':hashlib.sha256(module.read_bytes()).hexdigest(),'archive_sha256':hashlib.sha256(archive.read_bytes()).hexdigest(),'extracts_to_temp':True,'visible_progress':True,'no_console':True,'tested':False}
(root/'Docs/single_exe_build.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
shutil.copy2(source/'BASAHIN_MUNA.txt',root/'SUBMISSION/BASAHIN_MUNA.txt')
print(json.dumps(result,indent=2))
