# Handoff — 2026-10-28

Branch: `phase3-polish`. Project: `C:\Users\chris\Documents\CSEL GAMEDEV FILES\ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO`.

## Done

- Rebuilt the game from the real project folder with Unity 6000.6.2f1. Build passes 65 rule assertions. Graphics-enabled EditMode 11/11, PlayMode 9/9. Portable smoke and screenshot checks pass at 1280x720, 1366x768, 1920x1080, 2560x1440 and 3440x1440.
- Reviewed 1280x720 match/dizzy and 3440x1440 match/result. The Hilo HUD is visible, five amber/red streak pips fill/reset, and dizzy stars show. HUD screenshots for all five sizes are under `QA_SCREENSHOTS/HUD/`.
- Step 1 complete: eight authored six-phase weapon-strike sheets are integrated. Weapons raise and reach horizontally toward the opponent, with no separate weapon object or thin straight trail. All 272 textures share an 896x512 canvas and an actual feet-center actor pivot. Every frame passed body-mask connectivity/bounds/ground checks; all 48 strikes passed grip/tip checks. Swing/impact audio and the fully extended frame align to the unchanged hit event.
- The native build captured 108 animation/pair screenshots under `QA_SCREENSHOTS/ANIMATION/Unity/`, covering six strike and four hurt frames per character in both facings and all 28 pair types. I reviewed all eight strike/hurt asset montages and native representative frames; no split body, floating item or clipping was found. `Docs/animation_capture_evidence.json` stores native capture hashes.
- Fresh source checks for the strike swap: EditMode 11/11, graphics-enabled PlayMode 9/9, build plus 65 assertions passed from the real folder. Other reaction/locomotion states still use whole-body effect variants and need dedicated pose/expression polish.
- Created the auto-resume PowerShell script/docs and ran a harmless `codex exec` test (exit 0). No reference photos were read or sent to tools.

## In progress

- Step 2: replace close-fighter horizontal bubble shifts with actual head anchors and a vertical collision solution, then test and commit.
- Watchdog file exists from the interrupted work but is not registered or running. Finish/test strikes, bubbles and HUD before completing watchdog setup, per the latest user order.
- Latest distributable ZIP is intentionally not rebuilt here; final EXE/Drive work is queued separately.

## Next five steps

1. Finish head-centered bubbles with small tails, screen/HUD bounds and overlap handling; run rule/input checks and commit step 2.
2. Finish mirrored wooden Hilo HUD, winner tally and bottom keycaps; run rule/input checks and commit step 3.
3. Finish/test the watchdog after steps 1–3; register/start it, record power settings and commit step 4.
4. Continue reaction/expression and action-pose polish using fictional assets only; retain honest placeholder/manual statuses for any uncompleted art.
5. Refresh all five resolution captures/evidence and clean-tree pre-check, then stop before the separately queued final EXE/Drive task.

## Resume command

`codex exec --sandbox workspace-write -C "C:\Users\chris\Documents\CSEL GAMEDEV FILES\ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO" "Read RESUME_STATE.txt and HANDOFF.md, run git status, finish step 2 head-anchored reaction bubbles and tests, then continue the numbered steps. Do not ask questions or start the queued EXE/Drive task."`

For the next Codex session: read `RESUME_STATE.txt`, `HANDOFF.md`, `PENDING_NETWORK.md`, `CHECKLIST.md`; run `git status`; continue from step 1 above. Do not ask questions. Do not start final EXE/Drive task in this continuation.

## Auto-resume / network / power

`Tools/autoresume.ps1` was tested via the harmless CLI smoke command; one unsupported option was corrected after consulting `codex --help` and `codex exec --help`. The watchdog has not yet relaunched this task; relaunch count: **0**. `PENDING_NETWORK.md` records the public-photo-history hold and queued Drive sign-in/upload. Powercfg showed AC sleep and display timeouts were already 0; commands to restore those previous values are both `... 0`.

## NEEDS MANUAL TEST

Human presentation/feel review of authored strikes, signature facial reactions/cues and dedicated locomotion poses, physical gamepad/hot-swap and keyboard rollover, full-screen fit on the presentation monitor, subjective audio levels, clean-PC/SmartScreen, final ZIP extraction/playback (queued separately), and owner decision on the public photo history.
