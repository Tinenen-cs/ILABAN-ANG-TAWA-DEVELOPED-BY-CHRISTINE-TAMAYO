# Resume this Unity project

Project: `C:\Users\chris\Documents\CSEL GAMEDEV FILES\ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO`
Branch: `phase3-polish`.

Read `HANDOFF.md`, `CHECKLIST.md` and `Docs/CHARACTER_DESIGN.md`. Do not push, force-push, rewrite history, merge, or add/upload/send any `CHARACTER REFERENCE` photos. Keep all graded combat and Tagalog/Pormal rules intact.

The latest Windows x64 build was made from `%TEMP%\Ilaban-Final-Validation`. Its source HEAD matched real commit `2f3fbbe` when built; later commits only updated packaging helpers and QA documentation. Set `$env:ILABAN_PROJECT=Join-Path $env:TEMP 'Ilaban-Final-Validation'` before rerunning `Tools/run_portable_qa.py` or `Tools/package_portable.py`.

Verified: 65 rule assertions; EditMode 9/9; graphics-enabled PlayMode 4/4; portable QA 1280x720, 1366x768, 1920x1080 and 2560x1440; extracted ZIP launched and completed a simulated gamepad match. Details and hashes are in `HANDOFF.md`, `Docs/portable_qa_results.json`, `Docs/package_verification.json` and `Docs/QA_EVIDENCE`.

Remaining manual checks: physical controllers/hot-swap, keyboard rollover and latency, listening/audio balance, presentation monitor fit, clean PC without Unity, antivirus/SmartScreen. Cutout rig articulation/expression swaps remain partial. Do not rerun `Tools/generate_ilaban_assets.py` blindly; it does not reproduce the approved art.
