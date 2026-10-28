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
- Watchdog file exists from the interrupted work but is not registered or running. Finish/test strikes, bubbles and HUD before completing watchdog setup, per the latest user order.
- Latest distributable ZIP is intentionally not rebuilt here; final EXE/Drive work is queued separately.

## Next five steps

1. Finish/test the watchdog now that steps 1–3 passed; verify one-agent detection and lock/reset behavior.
2. Register/start it, record scheduled task and prior power settings, and commit step 4; rerun RubricTests/65 rules.
3. Continue remaining original UI/art work: reaction/expression poses, headings/controls details, fixed night relighting and subtle foliage/banderitas.
4. Refresh final test/build/capture evidence after remaining source/art changes; keep hardware/clean-PC checks manual.
5. Run the clean-tree pre-check and stop before the separately queued final EXE/Drive task. No public publishing until the photo-history decision.

## Resume command

`codex exec --sandbox workspace-write -C "C:\Users\chris\Documents\CSEL GAMEDEV FILES\ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO" "Read RESUME_STATE.txt and HANDOFF.md, run git status, finish step 4 safe auto-resume setup and tests, then continue the original UI/art checklist. Do not ask questions or start the queued EXE/Drive task."`

For the next Codex session: read `RESUME_STATE.txt`, `HANDOFF.md`, `PENDING_NETWORK.md`, `CHECKLIST.md`; run `git status`; continue from step 1 above. Do not ask questions. Do not start final EXE/Drive task in this continuation.

## Auto-resume / network / power

`Tools/autoresume.ps1` was tested via the harmless CLI smoke command; one unsupported option was corrected after consulting `codex --help` and `codex exec --help`. The watchdog has not yet relaunched this task; relaunch count: **0**. `PENDING_NETWORK.md` records the public-photo-history hold and queued Drive sign-in/upload. Powercfg showed AC sleep and display timeouts were already 0; commands to restore those previous values are both `... 0`.

## NEEDS MANUAL TEST

Human presentation/feel review of authored strikes, signature facial reactions/cues and dedicated locomotion poses, physical gamepad/hot-swap and keyboard rollover, full-screen fit on the presentation monitor, subjective audio levels, clean-PC/SmartScreen, final ZIP extraction/playback (queued separately), and owner decision on the public photo history.
