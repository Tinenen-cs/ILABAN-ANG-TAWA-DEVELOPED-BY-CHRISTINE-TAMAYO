Ilaban AutoResume watchdog

Purpose: resume local work from the checked-in checkpoint files after Codex has stopped. It is not an offline model and cannot bypass usage limits. It waits for network availability and respects a readable reset time; otherwise it waits 15 minutes between limited relaunch attempts.

Files: Tools/autoresume.ps1, Tools/autoresume.log, RESUME_STATE.txt, HANDOFF.md, PENDING_NETWORK.md and CHECKLIST.md.

Safety gates:
- `Tools/autoresume.lock` uses an exclusive open handle; concurrent scheduled/watchdog copies exit.
- `Tools/codex-active.lock` marks a foreground Codex session; the watchdog skips relaunch while it exists.
- It also checks Windows process command lines for `codex.exe` already referencing this repository.
- `TASK_COMPLETE.flag` stops the loop. Three consecutive nonzero Codex exits also stop it.
- A network check to github.com:443 is required before launching. No credentials or tokens are written.
- It does not push or publish. The pending executable/Drive task remains separate.

Resume command tested on this PC:
`codex exec --ephemeral --sandbox read-only -C <project-folder> "Reply with exactly AUTORESUME_SMOKE_OK"`
The exact invocation returned exit code 0 and `AUTORESUME_SMOKE_OK` on 2026-10-28. The watchdog uses the same verified `codex exec` entry point with workspace-write so the agent can continue project files.

Start manually:
`powershell -NoProfile -ExecutionPolicy Bypass -File Tools\autoresume.ps1`

Scheduled task: `IlabanAutoResume` is registered at logon and every five minutes. Stop it with:
`Disable-ScheduledTask -TaskName IlabanAutoResume`
Undo the plugged-in no-sleep setting with the previous AC standby and monitor timeout values recorded in HANDOFF.md.
