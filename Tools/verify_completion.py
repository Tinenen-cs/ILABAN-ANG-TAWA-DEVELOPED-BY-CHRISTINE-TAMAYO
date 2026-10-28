"""Read-only pre-check for finishing this local overhaul before queued delivery."""
from pathlib import Path
import json,re,subprocess,argparse
from evidence_fingerprint import source_fingerprint
root=Path(__file__).resolve().parents[1]
p=argparse.ArgumentParser();p.add_argument('--allow-dirty',action='store_true');args=p.parse_args()
rows=re.findall(r'^(\d+)\. \[([x~ ])\]',(root/'CHECKLIST.md').read_text(encoding='utf-8-sig'),re.M)
assert len(rows)==25 and {int(n) for n,s in rows}==set(range(1,26)) and all(s!=' ' for n,s in rows),'Checklist 1-25 must have explicit verified/manual/queued states'
report=json.loads((root/'Docs/QA_EVIDENCE/final_summary.json').read_text());assert report['local_passed'] and report['rule_assertions']==65
assert all(v['passed']==v['total'] and v['total']>0 for v in report['tests'].values())
assert len(report['resolutions'])==5 and all(v['passed'] for v in report['resolutions'])
assert report['build_source']==source_fingerprint(root),'Source differs from final verified build'
watch=json.loads((root/'Docs/QA_EVIDENCE/autoresume_tests.json').read_text(encoding='utf-8-sig'));assert watch['lock_exclusive'] and watch['harmless_cli_passed']
assert subprocess.check_output(['git','branch','--show-current'],cwd=root,text=True).strip()=='phase3-polish'
status=subprocess.check_output(['git','status','--porcelain'],cwd=root,text=True)
if not args.allow_dirty:assert not status,'Commit the remaining evidence/docs before completion'
print('PASS: 25 explicit checklist states; tests, 65 assertions, five resolutions, current build fingerprint, watchdog smoke and branch verified.')
print('Checklist: verified='+str(sum(s=='x' for n,s in rows))+', manual/queued='+str(sum(s=='~' for n,s in rows)))
print('HEAD='+subprocess.check_output(['git','rev-parse','HEAD'],cwd=root,text=True).strip());print('Git status: '+('clean' if not status else 'checkpoint pending'))
