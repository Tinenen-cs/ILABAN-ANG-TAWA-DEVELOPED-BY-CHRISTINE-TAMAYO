from pathlib import Path
import os, shutil, zipfile, hashlib, json, subprocess, time, tempfile

root=Path(__file__).resolve().parents[1]
title='ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO'
source=Path(os.environ.get('ILABAN_PROJECT',os.path.join(os.environ['TEMP'],'Ilaban-Phase2-Validation')))/'Builds/Windows'
submission=root/'SUBMISSION'; package=Path(tempfile.mkdtemp(prefix='Ilaban-Package-'))/title
if source.resolve()!=(root/'Builds/Windows').resolve():shutil.copytree(source,root/'Builds/Windows',dirs_exist_ok=True)
shutil.copytree(source,package,dirs_exist_ok=True)
(package/'BASAHIN_MUNA.txt').write_text('''ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO

Laro ito para sa Windows 10 o 11 na 64-bit.
Hindi kailangang maglagay ng Unity o ibang installer.

1. Kunin ang ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.zip mula sa pahina ng GitHub Releases. Maaari itong kunin sa telepono at ilipat sa kompyuter, o i-download sa kompyuter ng paaralan.
2. Sa Windows, i-right click ang ZIP at piliin ang Extract All. Piliin ang lalagyan kung saan mo gustong ilagay ang laro.
3. Buksan ang bagong lalagyan. I-double click ang ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe. Panatilihing magkasama ang EXE, _Data, UnityPlayer.dll at iba pang kasamang talaksan.
4. Kung nagpakita ng babala ang SmartScreen, piliin ang More info at Run anyway kung nakuha mo ang laro mula sa opisyal na release. Huwag patayin ang pananggalang laban sa virus.

Mga pindutan:
Manlalaro 1: A / D para maglakad; F para sumuntok at magkumpirma.
Manlalaro 2: kaliwa / kanan para maglakad; Ctrl kanan o 0 sa numerong pad para sumuntok at magkumpirma.
Mando: kaliwang palanca o mga palaso para maglakad; A / X para sumuntok at magkumpirma; B para bumalik.
ESC: ihinto o ituloy ang laro. H: buksan ang mga kontrol. Alt+Enter: palitan ang buong tabing at bintana.

Mga tuntunin:
Kaliwa at kanan lamang ang galaw. Tuwid na suntok lamang ang atake.
Bawat tama ay tumutulak ng katunggali nang isang hakbang paatras.
Limang sunod na tama ang nagpapahilo nang dalawang segundo.
Kapag nahulog sa kanal ang manlalaro, agad nagtatapos ang laban.
Walang lakas-buhay, knockout, talon, sipa, dagdag na galaw o takdang oras.

Sa Canvas, i-upload ang eksaktong siping ito:
ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.zip
Makikita ng guro ang EXE, mga kailangang kasamang talaksan at gabay na ito.
''',encoding='utf-8')
for name in ['CREDITS.md','AUDIO_LIST.md']:
    shutil.copy2(root/name,package/name)
archive=submission/(title+'.zip')
with zipfile.ZipFile(archive,'w',zipfile.ZIP_DEFLATED,compresslevel=6) as z:
    for file in sorted(package.rglob('*')):
        if file.is_file():z.write(file,Path(title)/file.relative_to(package))
extract=Path(tempfile.mkdtemp(prefix='Ilaban-Extracted-'))
with zipfile.ZipFile(archive) as z:
    assert z.testzip() is None
    z.extractall(extract)
exe=extract/title/(title+'.exe')
captures=root/'QA_SCREENSHOTS/Extracted_Portable';shutil.rmtree(captures,ignore_errors=True);captures.mkdir(parents=True);log=root/'Logs/Extracted-Portable.log'
print('Extracted to',extract,flush=True)
proc=subprocess.Popen([str(exe),'-screen-fullscreen','0','-screen-width','1280','-screen-height','720','-ilaban-smoke',str(captures),'-logFile',str(log)])
try:code=proc.wait(timeout=180)
except subprocess.TimeoutExpired:proc.kill();raise
issues=[l for l in log.read_text(encoding='utf-8',errors='replace').splitlines() if any(s in l for s in ['Exception','MISSING GLYPH','LAYOUT TEXT OVERFLOW','error CS'])]
result=dict(zip=str(archive),bytes=archive.stat().st_size,sha256=hashlib.sha256(archive.read_bytes()).hexdigest(),extracted_to=str(extract),exit=code,passed=code==0 and (captures/'PASS.txt').exists() and not issues,issues=issues,clean_pc_test=False)
(root/'Docs/package_verification.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
print(json.dumps(result,indent=2),flush=True)
assert result['passed']
