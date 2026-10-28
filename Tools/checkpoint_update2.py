"""Append explicit, evidence-based UPDATE2 checkpoints; never infer a pass."""
from pathlib import Path
import datetime, subprocess, sys

root = Path(__file__).resolve().parents[1]
step, result, next_step = sys.argv[1:4]
head = subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=root, text=True).strip()
stamp = datetime.datetime.now(datetime.timezone.utc).isoformat()
note = f'\n## UPDATE2 step {step} — {stamp}\n\n{result}\n\nNext: {next_step}\n'
for name in ['HANDOFF.md', 'CHECKLIST.md', 'UPDATE_PROGRESS.md']:
    path = root/name
    path.write_text(path.read_text(encoding='utf-8-sig')+note, encoding='utf-8')
(root/'RESUME_STATE.txt').write_text(f'HEAD={head} | UPDATE2 step {step}: {result} | Next={next_step} | UTC={stamp}\n', encoding='utf-8')
print(note)
