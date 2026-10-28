from pathlib import Path
import json, hashlib, xml.etree.ElementTree as ET, subprocess
from PIL import Image
root=Path(__file__).resolve().parents[1]
results=json.loads((root/'Docs/portable_qa_results.json').read_text())
package=json.loads((root/'Docs/package_verification.json').read_text())
assert len(results)==5 and all(r['passed'] for r in results)
assert package['passed']
assert hashlib.sha256(Path(package['zip']).read_bytes()).hexdigest()==package['sha256']
for name in ['EditMode','PlayMode']:
    tree=ET.parse(root/f'Docs/QA_EVIDENCE/{name}.xml').getroot()
    assert tree.get('failed')=='0' and tree.get('result')=='Passed'
captures=[]
for result in results:
    folder=Path(result.get('folder',root/'QA_SCREENSHOTS'/('Final_'+result['resolution'])))
    opening=(folder/'00-opening-ui.txt').read_text(encoding='utf-8')
    assert 'Handog ni Christine Tamayo' in opening,'Opening capture must show the intro, not title'
    for file in sorted(folder.glob('*.png')):
        captures.append(dict(file=str(file.relative_to(root)),size=list(Image.open(file).size),sha256=hashlib.sha256(file.read_bytes()).hexdigest()))
(root/'Docs/QA_EVIDENCE/captures.json').write_text(json.dumps(captures,indent=2))
path=root/'Tools/write_master_checklist.py';text=path.read_text(encoding='utf-8')
text=text.replace("('p','Latest PlayMode rerun and build lifecycle verification in progress; previous checkpoint passed 4/4.')","('x','PlayMode 4/4 passed with graphics enabled; final Windows x64 build exited 0 and passed 65 rubric assertions.')")
text=text.replace("('p','Latest screenshots at four required resolutions, corrected overflow audit, final extracted build launch and package validation in progress.')","('x','Four exact-resolution captures, real opening capture, no reported overflow/glyph/runtime errors, extracted ZIP launch and full simulated match verified.')")
path.write_text(text,encoding='utf-8');subprocess.run(['python',str(path)],cwd=root,check=True)
handoff=root/'HANDOFF.md';text=handoff.read_text(encoding='utf-8')
text=text.replace('Final release rebuild, all-four-resolution smoke and portable ZIP extraction checks are being completed. Consult Docs/portable_qa_results.json and the append-only final results below before assuming completion.','Final release rebuild, all-four-resolution smoke and portable ZIP extraction checks are complete. See the final results below and versioned JSON evidence.')
text+='''
## Final verified release

- Windows x64 build: Unity 6000.6.2f1, Logs/Final-release.log, exit 0, 65 rubric assertions. No C# compiler errors or warnings found. Unity licensing/network diagnostic messages are distinct from compilation results.
- EditMode 9/9 and PlayMode 4/4 passed; XML under Docs/QA_EVIDENCE. PlayMode requires graphics because TMP can open its editor resource importer; the headless attempt was not counted as a pass.
- 25 captures at each of 1280x720, 1366x768, 1920x1080 and 2560x1440. The 1440p capture uses an exact-size borderless window; no monitor-mode changes. Native Unity framebuffer capture, not resized images. Docs/QA_EVIDENCE/captures.json stores dimensions and hashes.
- All five resolution runs check Tagalog/Pormal defaults, both-player readiness, three fixed-light maps, actual simulated gamepad movement/punches to dizzy and manhole win, pause and controls. No reported runtime exceptions, missing glyphs or TMP overflow.
- Initial capture exposed an opening timing problem during slow loading. Opening time now advances at a bounded rate and the harness requires a real intro before title. Normal players can still skip with a key/controller button.
- Portable ZIP extracted to a fresh temporary folder and completed the same actual-input match. This is a development-PC test, not a clean-PC test. Exact ZIP hash, size and extraction path: Docs/package_verification.json.
- All work is committed locally on phase3-polish. No push, history rewrite or merge performed. Full PNG captures and SUBMISSION stay local; evidence manifests are versioned.

Score risks: cut-out rigs have modest articulation and do not yet have polished joint pivots/expression swaps; audio is original synthesized material, with subjective mix/realism awaiting listening. Hardware/clean-PC checks remain manual. No Exceeds-level or grade guarantee is claimed.
''';handoff.write_text(text,encoding='utf-8')
resume=root/'CODEX_RESUME.md';text=resume.read_text(encoding='utf-8');text+='\nFinal release packaged and extraction-tested. All four exact-resolution smoke runs passed; EditMode 9/9, PlayMode 4/4. See HANDOFF final section. Remaining work is optional articulation/audio polish, manual device/clean-PC checks and owner-controlled publishing. Do not push.\n';resume.write_text(text,encoding='utf-8')
plan=root/'TEST_PLAN.md';text=plan.read_text(encoding='utf-8')
text=text.replace('| Corrected UI labels/HUD contrast and rig articulation | | | Source fixes after preliminary screenshots; final re-capture pending |','| Corrected UI labels and HUD contrast | X | | Four-resolution re-capture; articulation remains a documented polish limitation |')
text=text.replace('| Latest PlayMode rerun lifecycle | | | Prior 4/4; new runner stalls independently of tests, under investigation |','| Latest PlayMode rerun lifecycle | X | | Graphics-enabled runner passed 4/4 and exited; headless TMP importer attempt excluded |')
text=text.replace('| Four resolution captures | | | Tools/run_portable_qa.py writes dimensions and error checks to Docs/portable_qa_results.json |','| Four resolution captures | X | | Exact 720p, 768p, 1080p and 1440p; JSON dimensions and native PNG hashes saved |')
text=text.replace('| Portable ZIP extracted and launched | | | Pending final package |','| Portable ZIP extracted and launched | X | | Docs/package_verification.json; fresh temp extraction, launch and full simulated match passed on this PC |')
plan.write_text(text,encoding='utf-8')
(root/'BUG_LOG.md').open('a',encoding='utf-8').write('\n- Final QA: Windows decorated-window clamp prevented native 1440p; exact-size borderless diagnostic capture passed. Intro capture advanced past opening during startup delays; bounded intro clock and required-opening harness fixed it. Headless PlayMode TMP importer needs graphics; graphics-enabled 4/4 result retained.\n')
print('Final evidence and handoff verified',len(captures),'captures',flush=True)
