# Handoff — 2026-10-28

Branch: `phase3-polish`. Project: `C:\Users\chris\Documents\CSEL GAMEDEV FILES\ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO`.

## Done

- Rebuilt the game from the real project folder with Unity 6000.6.2f1. Build passes 65 rule assertions. Graphics-enabled EditMode 11/11, PlayMode 9/9. Portable smoke and screenshot checks pass at 1280x720, 1366x768, 1920x1080, 2560x1440 and 3440x1440.
- Reviewed 1280x720 match/dizzy and 3440x1440 match/result. The Hilo HUD is visible, five amber/red streak pips fill/reset, and dizzy stars show. HUD screenshots for all five sizes are under `QA_SCREENSHOTS/HUD/`.
- Combat now draws one whole-body texture per actor/action; 272 frames share 272x512 canvases and fixed feet-center grounding. Structural checks report one connected base silhouette for each character. The user-reported floating stick came from our procedural straight grip-to-tip line; replaced it with a curved ribbon, and verified the refreshed montage. No separate item object exists in the fight hierarchy.
- Strike choreography is still placeholder art: the held prop does not yet raise and extend toward the opponent. Do not claim the requested weapon poses complete. All rules/timing/hitboxes remain unchanged.
- Created the auto-resume PowerShell script/docs and ran a harmless `codex exec` test (exit 0). No reference photos were read or sent to tools.

## In progress

- Final local docs/checkpoint commit and watchdog registration/launch.
- Latest distributable ZIP is intentionally not rebuilt here; final EXE/Drive work is queued separately.

## Next five steps

1. Verify `Tools/autoresume.ps1` parses and its lock/complete gates work; record the test.
2. Register and launch the watchdog at logon and every five minutes; current `Tools/codex-active.lock` prevents it from duplicating this foreground session.
3. Set AC standby/display timeouts to never; both were already 0, so undo is `powercfg /change standby-timeout-ac 0` and `powercfg /change monitor-timeout-ac 0` (these restore current prior values).
4. Commit this local UI/animation/watchdog checkpoint; update `RESUME_STATE.txt` and this handoff.
5. Finish final evidence audit and source clean-tree check. Stop before queued final EXE/Drive task; do not publish while public history contains photos.

## Resume command

`powershell -NoProfile -ExecutionPolicy Bypass -File Tools\autoresume.ps1`

For the next Codex session: read `RESUME_STATE.txt`, `HANDOFF.md`, `PENDING_NETWORK.md`, `CHECKLIST.md`; run `git status`; continue from step 1 above. Do not ask questions. Do not start final EXE/Drive task in this continuation.

## Auto-resume / network / power

`Tools/autoresume.ps1` was tested via the harmless CLI smoke command; one unsupported option was corrected after consulting `codex --help` and `codex exec --help`. The watchdog has not yet relaunched this task; relaunch count: **0**. `PENDING_NETWORK.md` records the public-photo-history hold and queued Drive sign-in/upload. Powercfg showed AC sleep and display timeouts were already 0; commands to restore those previous values are both `... 0`.

## NEEDS MANUAL TEST

Weapon-specific poses, signature facial reactions/cues, physical gamepad/hot-swap and keyboard rollover, full-screen fit on the presentation monitor, subjective audio levels, clean-PC/SmartScreen, final ZIP extraction/playback (queued separately), and owner decision on the public photo history.
