"""Verify current evidence without overwriting authored checklists or inventing results."""
from pathlib import Path
import argparse,json,hashlib,xml.etree.ElementTree as ET,datetime,zipfile
from PIL import Image
from evidence_fingerprint import source_fingerprint,sha
root=Path(__file__).resolve().parents[1]
parser=argparse.ArgumentParser();parser.add_argument('--local-only',action='store_true',help='Verify local source/build/tests; final ZIP task remains queued.');args=parser.parse_args()
evidence=root/'Docs/QA_EVIDENCE';tests={}
for name in ['EditMode','PlayMode']:
    report=ET.parse(evidence/(name+'.xml')).getroot()
    assert report.get('result')=='Passed' and int(report.get('failed','1'))==0,name
    tests[name]={'passed':int(report.get('passed')),'total':int(report.get('total')),'started':report.get('start-time'),'finished':report.get('end-time')}
provenance=json.loads((evidence/'build_provenance.json').read_text());assert provenance['project']==str(root.resolve())
assert provenance['source']==source_fingerprint(root),'Source changed since the validation build'
exe=root/'Builds/Windows/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe'
assert sha(exe)==provenance['exe_sha256'],'Built player changed'
build_time=datetime.datetime.fromisoformat(provenance['utc']).timestamp()
log=(root/'Logs/UIRelease-Build.log').read_text(encoding='utf-8',errors='replace')
assert 'ILABAN VALIDATION PASSED: 65 rubric assertions' in log
import re
assert not re.search(r'\b(?:error|warning) CS\d+',log),'C# compiler diagnostics remain'
results=json.loads((root/'Docs/portable_qa_results.json').read_text());assert {r['resolution'] for r in results}=={'1280x720','1366x768','1920x1080','2560x1440','3440x1440'} and all(r['passed'] for r in results)
captures=[]
for result in results:
    size=tuple(map(int,result['resolution'].split('x')));folder=Path(result.get('folder',root/'QA_SCREENSHOTS'/('Final_'+result['resolution'])))
    assert (folder/'PASS.txt').exists() and 'Handog ni Christine Tamayo' in (folder/'00-opening-ui.txt').read_text(encoding='utf-8')
    files=sorted(folder.glob('*.png'));assert len(files)==15,(folder,len(files))
    for path in files:
        assert path.stat().st_mtime>=build_time,'Stale capture: '+str(path)
        assert Image.open(path).size==size
        captures.append({'file':path.relative_to(root).as_posix(),'size':list(size),'sha256':sha(path)})
frames=json.loads((root/'Docs/character_action_validation.json').read_text());assert frames['all_passed'] and frames['frames_checked']==272
animation=json.loads((root/'Docs/animation_capture_evidence.json').read_text());assert animation['passed'] and animation['frames']==108
for row in animation['files']:
    path=root/row['file'];assert path.stat().st_mtime>=build_time,'Stale native animation capture'
    assert sha(path)==row['sha256']
audit=json.loads((root/'Docs/FINAL_STATIC_AUDIT.json').read_text());assert audit['audio_files']==38 and audit['clipped_samples']==0 and not any(audit[k] for k in ['tagalog_default_english_candidates','mojibake_files','missing_audio_references'])
package={'status':'QUEUED_SEPARATELY','verified_current':False}
if not args.local_only:
    package=json.loads((root/'Docs/package_verification.json').read_text());archive=Path(package['zip'])
    assert package['passed'] and archive.stat().st_size==package['bytes'] and sha(archive)==package['sha256']
    with zipfile.ZipFile(archive) as z:
        assert z.testzip() is None
        for path in sorted((root/'Builds/Windows').rglob('*')):
            if path.is_file():
                names=[n for n in z.namelist() if n.endswith('/'+path.relative_to(root/'Builds/Windows').as_posix())]
                assert len(names)==1 and hashlib.sha256(z.read(names[0])).hexdigest()==sha(path),'ZIP is older than current build: '+str(path)
    package['verified_current']=True
(evidence/'captures.json').write_text(json.dumps(captures,indent=2),encoding='utf-8')
summary={'utc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'local_passed':True,'tests':tests,'rule_assertions':65,'resolutions':results,'native_ui_captures':len(captures),'native_animation_captures':108,'action_frame_checks':272,'audio_files':38,'package':package,'build_source':provenance['source']}
(evidence/'final_summary.json').write_text(json.dumps(summary,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in summary.items() if k not in ['resolutions','package']},indent=2));print('Package:',package.get('status','verified current'))
