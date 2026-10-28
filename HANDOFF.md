# Handoff — 2026-10-28

Branch: `phase3-polish`. Project: `C:\Users\chris\Documents\CSEL GAMEDEV FILES\ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO`.

## Done

- Rebuilt the game from the real project folder with Unity 6000.6.2f1. Current source passes 65 rule assertions, graphics-enabled EditMode 11/11 and PlayMode 9/9. Fresh native smoke passes all five sizes: 1280x720, 1366x768, 1920x1080, 2560x1440 and 3440x1440, with no reported exceptions, missing glyphs or TMP overflow. Both Hilo labels remain clear and the reviewed HUDs fit every size.
- Reviewed 1280x720 match/dizzy and 3440x1440 match/result. The Hilo HUD is visible, five amber/red streak pips fill/reset, and dizzy stars show. HUD screenshots for all five sizes are under `QA_SCREENSHOTS/HUD/`.
- Step 1 complete: eight authored six-phase weapon-strike sheets are integrated. Weapons raise and reach horizontally toward the opponent, with no separate weapon object or thin straight trail. All 272 textures share an 896x512 canvas and an actual feet-center actor pivot. Every frame passed body-mask connectivity/bounds/ground checks; all 48 strikes passed grip/tip checks. Swing/impact audio and the fully extended frame align to the unchanged hit event.
- The native build captured 108 animation/pair screenshots under `QA_SCREENSHOTS/ANIMATION/Unity/`, covering six strike and four hurt frames per character in both facings and all 28 pair types. I reviewed all eight strike/hurt asset montages and native representative frames; no split body, floating item or clipping was found. `Docs/animation_capture_evidence.json` stores native capture hashes.
- Fresh source checks for the strike swap: EditMode 11/11, graphics-enabled PlayMode 9/9, build plus 65 assertions passed from the real folder. Other reaction/locomotion states still use whole-body effect variants and need dedicated pose/expression polish.
- Created the auto-resume PowerShell script/docs and ran a harmless `codex exec` test (exit 0). No reference photos were read or sent to tools.

## In progress

- Step 2 complete: per-frame head positions are authored in animation data, mirrored with the whole sprite, and transformed with the actor through motion/dizzy/fall. Bubbles use rounded 160x50 plates and short tails, stay centered on their own heads, clamp to the screen/HUD-safe region, and separate vertically when close. Fresh EditMode 11/11, PlayMode 9/9 and 65 rule assertions passed. Native close-range smoke proof is under `QA_SCREENSHOTS/Step2_Bubbles/`.
- Step 3 complete: joined wood/brass rings and Hilo bars, uncovered mirrored labels, five amber/red segments per player, win medal pop/sparkle, bottom keycaps and shared wooden menu frames. Native review found pause-before-first-update placement and double/edge focus scaling bugs; both are fixed and covered by regressions. All tests/build and five-size native re-captures passed. HUD captures are under `QA_SCREENSHOTS/HUD/`.
- Step 4 complete after steps 1–3: watchdog source, setup helper, exclusive lock/reset tests, harmless read-only CLI smoke, active-process guard, hidden start and scheduled task are verified. Current relaunch count is **0**; it sees the active Codex processes and correctly skips a second editing agent. The real second-watchdog poll exited because the first owned the lock.
- Latest distributable ZIP is intentionally not rebuilt here; final EXE/Drive work is queued separately.

## Next five steps

1. Continue character reaction/expression and complete-action pose art using fictional source drawings only; add menu-safe matching cues.
2. Finish the original heading-lettering assets, controls keycap grid and remaining common button details.
3. Correct Looban/Plaza to fixed night lighting and add subtle isolated banderitas/leaves without moving the painted background.
4. Refresh rule/input/build and five-size capture evidence after those changes; hardware/clean-PC checks remain manual.
5. Run the clean-tree pre-check, write TASK_COMPLETE.flag, disable IlabanAutoResume and stop its verified watchdog process. End before the separately queued final EXE/Drive task. No publishing until the photo-history decision.

## Resume command

`codex exec --sandbox workspace-write -C "C:\Users\chris\Documents\CSEL GAMEDEV FILES\ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO" "Read RESUME_STATE.txt and HANDOFF.md, run git status, continue the remaining character reactions and original UI/art checklist. Do not ask questions or start the queued EXE/Drive task."`

For the next Codex session: read `RESUME_STATE.txt`, `HANDOFF.md`, `PENDING_NETWORK.md`, `CHECKLIST.md`; run `git status`; continue from step 1 above. Do not ask questions. Do not start final EXE/Drive task in this continuation.

## Auto-resume / network / power

`Tools/autoresume.ps1 -SelfTest` passed its exclusive-lock, reset-plus-two-minutes (4620-second example), 900-second fallback, and harmless Codex CLI checks. Fresh `codex --help`/`codex exec --help` and official non-interactive documentation were consulted. The task is registered with current-user logon and PT5M triggers; its hidden process and settings are recorded in `Docs/QA_EVIDENCE/autoresume_setup.json`. Another copy exits on the held lock. Three failed relaunches persist a halt across scheduled starts.

Safety limitation: the watchdog conservatively blocks while **any** codex.exe host is running, including an idle App/IDE, because this script cannot prove that those remote sessions are idle. Close those hosts before expecting a CLI relaunch. Actual reboot/usage-limit recovery remains NEEDS MANUAL TEST; it has relaunched this task **0 times**, because the current Codex process guard is active.

The first combined setup command was rejected by automatic review; running normal PowerShell setup without an outer execution-policy override succeeded. No unresolved setup block remains. AC sleep and monitor timeouts were already 0 seconds before setup and remain 0; restore commands: `powercfg /change standby-timeout-ac 0` and `powercfg /change monitor-timeout-ac 0`. DC settings were not changed. Disable the task with `Disable-ScheduledTask -TaskName IlabanAutoResume` after completion. `PENDING_NETWORK.md` retains the public-photo-history hold and separate Drive task.

## NEEDS MANUAL TEST

Human presentation/feel review of authored strikes, signature facial reactions/cues and dedicated locomotion poses, physical gamepad/hot-swap and keyboard rollover, full-screen fit on the presentation monitor, subjective audio levels, clean-PC/SmartScreen, final ZIP extraction/playback (queued separately), and owner decision on the public photo history.

## Current action-art checkpoint

48 additional authored whole-body walk/hurt/dizzy/win/lose poses are integrated for all eight fictional characters. Fall uses the lose pose with the existing whole-body sink transform; idle uses the ready stance. Six-frame strikes remain unchanged. All 272 masks and 48 strike grip/tip checks passed after regeneration. Eight soft reaction cues wait for menu audio and coalesce rapidly repeated selections. Fresh Unity tests/rule checks are running; do not claim their new results until XML/logs are checked. Next five: verify tests; review new native animation captures; finish heading/control assets; night maps and isolated foliage; final evidence/pre-check. Resume with the existing exact command above.
