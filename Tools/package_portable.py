from pathlib import Path
import os, shutil, zipfile, hashlib, json, subprocess, time

root=Path(__file__).resolve().parents[1]
title='ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO'
source=Path(os.environ.get('ILABAN_PROJECT',os.path.join(os.environ['TEMP'],'Ilaban-Phase2-Validation')))/'Builds/Windows'
submission=root/'SUBMISSION'; package=submission/title
if source.resolve()!=(root/'Builds/Windows').resolve():shutil.copytree(source,root/'Builds/Windows',dirs_exist_ok=True)
shutil.copytree(source,package,dirs_exist_ok=True)
(package/'BASAHIN_MUNA.txt').write_text('''ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO

Para sa Windows na 64-bit.
Ilabas muna ang LAHAT ng laman ng siping nakaimpake sa iisang bagong lalagyan.
Buksan ang ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe.
Panatilihing magkakasama ang lahat ng kasamang talaksan at lalagyan.

Manlalaro 1: A / D para gumalaw; F para sumuntok.
Manlalaro 2: kaliwa / kanan para gumalaw; Ctrl kanan o 0 sa numerong pad para sumuntok.
Sa mando: palanca o mga palaso para gumalaw; A o X para sumuntok.
Kumpirmahin ang tauhan ng bawat manlalaro bago simulan.
ESC: ihinto o ituloy. H: mga kontrol.
Limang sunod na tama: hilo nang dalawang segundo.
Itulak ang katunggali sa imburnal para manalo.
Walang talon, sipa, lakas-buhay o takdang oras.

Sa pagpili ng entablado: kaliwa / kanan para pumili;
itaas / ibaba para sa Umaga, Hapon o Takipsilim.
Nasa Mga Pagpipilian ang Pangunahing Lakas, Musika at Tunog.

Kung hindi bumukas, subukan ang ibang kompyuter na may Windows na 64-bit.
Itala ang anumang babala at ipaalam sa gumawa.
Hindi pa nasubok sa hiwalay na kompyuter na walang Unity.
''',encoding='utf-8')
for name in ['CREDITS.md','AUDIO_LIST.md']:
    shutil.copy2(root/name,package/name)
archive=submission/(title+'.zip')
with zipfile.ZipFile(archive,'w',zipfile.ZIP_DEFLATED,compresslevel=6) as z:
    for file in sorted(package.rglob('*')):
        if file.is_file():z.write(file,file.relative_to(submission))
extract=Path(os.environ['TEMP'])/('Ilaban-Extracted-'+time.strftime('%Y%m%d-%H%M%S'))
with zipfile.ZipFile(archive) as z:
    assert z.testzip() is None
    z.extractall(extract)
exe=extract/title/(title+'.exe')
captures=root/'QA_SCREENSHOTS/Extracted_Portable';log=root/'Logs/Extracted-Portable.log'
print('Extracted to',extract,flush=True)
proc=subprocess.Popen([str(exe),'-screen-fullscreen','0','-screen-width','1280','-screen-height','720','-ilaban-smoke',str(captures),'-logFile',str(log)])
try:code=proc.wait(timeout=180)
except subprocess.TimeoutExpired:proc.kill();raise
issues=[l for l in log.read_text(encoding='utf-8',errors='replace').splitlines() if any(s in l for s in ['Exception','MISSING GLYPH','LAYOUT TEXT OVERFLOW','error CS'])]
result=dict(zip=str(archive),bytes=archive.stat().st_size,sha256=hashlib.sha256(archive.read_bytes()).hexdigest(),extracted_to=str(extract),exit=code,passed=code==0 and (captures/'PASS.txt').exists() and not issues,issues=issues,clean_pc_test=False)
(root/'Docs/package_verification.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
print(json.dumps(result,indent=2),flush=True)
assert result['passed']
