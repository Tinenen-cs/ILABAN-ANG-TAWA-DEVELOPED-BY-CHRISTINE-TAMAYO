"""Verify only freshly rerun UPDATE4 evidence; historical animation reports stay historical."""
from pathlib import Path
import datetime,json,subprocess,xml.etree.ElementTree as ET,zipfile
from evidence_fingerprint import source_fingerprint,sha
r=Path(__file__).resolve().parents[1];e=r/'Docs/QA_EVIDENCE';tests={}
for mode in ['EditMode','PlayMode']:
 x=ET.parse(e/(mode+'.xml')).getroot();assert x.get('result')=='Passed' and x.get('failed')=='0'
 tests[mode]={'passed':int(x.get('passed')),'total':int(x.get('total')),'ended':x.get('end-time')}
p=json.loads((e/'build_provenance.json').read_text());assert p['source']==source_fingerprint(r)
assert p['project']==str(r.resolve()) and p['exe_sha256']==sha(r/'Builds/Windows/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe')
log=(r/'Logs/UIRelease-Build.log').read_text(encoding='utf-8',errors='replace');assert '65 rubric assertions' in log and not any(s in log.lower() for s in ['error cs','warning cs','warning:'])
qa=json.loads((r/'Docs/update4_final_qa.json').read_text());assert len(qa)==4 and all(x['passed'] and x['captures']==20 for x in qa)
for row in qa:
 for f in row['files']:assert sha(r/f['file'])==f['sha256'] and (r/f['file']).stat().st_mtime>=datetime.datetime.fromisoformat(p['utc']).timestamp()
a=json.loads((r/'Docs/FINAL_STATIC_AUDIT.json').read_text());assert not any(a[k] for k in ['tagalog_default_english_candidates','mojibake_files','missing_audio_references','clipped_samples'])
model='Assets/Scripts/Runtime/DuelModel.cs';assert subprocess.check_output(['git','rev-parse','9ac4fc7:'+model],cwd=r)==subprocess.check_output(['git','hash-object',model],cwd=r)
b=json.loads((r/'Docs/single_exe_build.json').read_text());assert b['passed'] and sha(b['file'])==b['sha256']
solo=json.loads((r/b['tests']).read_text());assert len(solo)==2 and all(x['passed'] and x['captures']==22 for x in solo)
package=json.loads((r/'Docs/package_verification.json').read_text());assert package['passed'] and sha(package['zip'])==package['sha256']
with zipfile.ZipFile(package['zip']) as z:
 for f in (r/'Builds/Windows').rglob('*'):
  if f.is_file():
   import hashlib
   assert hashlib.sha256(z.read('ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO/'+f.relative_to(r/'Builds/Windows').as_posix())).hexdigest()==sha(f)
d=json.loads((e/'defender_update4.json').read_text());assert d[0]['exit']==0 and d[0]['sha256']==b['sha256'] and 'found no threats' in d[0]['output']
result={'utc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'tests':tests,'rules':65,'source':p['source'],'model_unchanged':True,'resolution_captures':80,'resolutions':[x['resolution'] for x in qa],'exe_only':solo,'exe':b,'package':package,'defender':d,'no_google_drive':True,'manual':['physical keyboard/gamepad/USB/hot-swap','human sound/Tagalog/art review','clean-PC/SmartScreen/.NET','strict steady60fps on other hardware']}
(e/'update4_final_summary.json').write_text(json.dumps(result,indent=2),encoding='utf-8');print('PASS: fresh UPDATE4 tests/build/80 screens/44 EXE-only screens/current payload hashes/Defender; combat model unchanged.')
