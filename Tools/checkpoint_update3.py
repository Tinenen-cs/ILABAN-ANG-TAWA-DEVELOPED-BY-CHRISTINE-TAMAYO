from pathlib import Path
import datetime, subprocess, sys
root=Path(__file__).resolve().parents[1]
step,result,next_step=sys.argv[1:4]
head=subprocess.check_output(['git','rev-parse','HEAD'],cwd=root,text=True).strip()
stamp=datetime.datetime.now(datetime.timezone.utc).isoformat()
note=f'# UPDATE3 step {step} - {stamp}\n\nDone: {result}\n\nIn progress / next: {next_step}\n\nNext five: follow remaining UPDATE3 tasks in Docs/UPDATE3_REQUEST.md in order; verify current screenshot; run required regression; checkpoint evidence; continue remaining task.\n\nResume: `codex exec -C "{root}" "Read HANDOFF.md, CHECKLIST.md and Docs/UPDATE3_REQUEST.md; run git status; finish UPDATE3 task {next_step}. No reference photos, no history rewrite."`\n\n---\n'
for name in ['HANDOFF.md','CHECKLIST.md','UPDATE_PROGRESS.md']:
 p=root/name;p.write_text(note+p.read_text(encoding='utf-8-sig'),encoding='utf-8')
(root/'RESUME_STATE.txt').write_text(f'HEAD={head} | UPDATE3 step {step}: {result} | Next={next_step} | UTC={stamp}\n',encoding='utf-8')
print(note)
