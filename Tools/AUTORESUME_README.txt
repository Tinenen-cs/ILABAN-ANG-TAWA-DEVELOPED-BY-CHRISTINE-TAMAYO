ILABAN AUTO-RESUME

Run: powershell -NoProfile -File Tools\autoresume.ps1
Setup/start: powershell -NoProfile -File Tools\setup_autoresume.ps1
Test: powershell -NoProfile -File Tools\autoresume.ps1 -SelfTest
One safe inspection poll: powershell -NoProfile -File Tools\autoresume.ps1 -Once

The installed CLI help and a harmless read-only command were checked on this PC.
The watchdog uses a fresh `codex exec --sandbox workspace-write -C <project> <resume instruction>` so it reads the durable checkpoint files, rather than selecting an unrelated session with --last. It uses the current Windows user's Codex sign-in. It never changes accounts, reads keys, or bypasses limits.

SAFETY AND LIMITATIONS
- An exclusive open handle on Tools/autoresume.lock allows one watchdog. It keeps that handle while its child Codex runs.
- ANY existing codex.exe process blocks launch. This deliberately includes App/IDE servers, even when idle, because their process command line cannot prove that a repo session is idle. Close those hosts before expecting an automatic CLI relaunch. The watchdog cannot safely resume work while an unobservable App/IDE session may still be active.
- Tools/autoresume-state.json persists launches, failure count, next retry UTC and the three-failure halt. Scheduled starts cannot reset that halt. Inspect the error and deliberately remove/reset that local state only after addressing the failure.
- It polls every 60 seconds. Offline checks back off 5, 15, 30, 60, 120, 300, 600, 600 seconds, then every 600 seconds. Connectivity is tested before every launch.
- A readable 'try again at h:mm AM/PM' limit message waits until that time plus two minutes. Unknown reset times wait 15 minutes. There are no limit bypasses.
- Tools/autoresume.log records actions and errors. Only error/limit diagnostic lines from the last agent run are retained, with credential-like strings redacted; the full tool transcript is not stored by the watchdog. Logs/state/locks stay local and gitignored.
- TASK_COMPLETE.flag stops the loop. On completion disable IlabanAutoResume and let the watchdog exit. Stop only the verified watchdog PowerShell process if needed, never another Codex agent.
- Final EXE/ZIP delivery and Drive upload remain separately queued. No push, release, photo upload, merge or history rewrite is performed by setup.

The scheduled task has an at-logon trigger and a five-minute trigger, one instance only. Its current registration result and previous plugged-in power timeouts are in Docs/QA_EVIDENCE/autoresume_setup.json. Both AC sleep/display timeouts were already 0 (never) before setup. HANDOFF.md records restore commands.

Official command/protocol references consulted:
https://learn.chatgpt.com/docs/non-interactive-mode
https://learn.chatgpt.com/docs/app-server
Local help remains authoritative for the installed alpha CLI's supported arguments.
