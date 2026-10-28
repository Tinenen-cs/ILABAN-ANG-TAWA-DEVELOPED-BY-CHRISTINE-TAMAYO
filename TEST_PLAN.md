# Test plan and current evidence

Run from the real project folder with Unity 6000.6.2f1 installed:

1. Set ILABAN_PROJECT to this project folder; run python Tools/run_final_checks.py. This runs EditMode, graphics-enabled PlayMode, the validation build and 65 assertions. Current totals are 11 and 10; use actual XML, never hardcoded historical totals.
2. Run python Tools/capture_character_actions.py for all 272 whole-body masks, feet pivots, ground lines and both-facing grip/tip checks.
3. Run python Tools/run_portable_qa.py for 1280x720, 1366x768, 1920x1080, 2560x1440 and 3440x1440. This launches a real built executable and simulated input through ready selection, pause/controls, five-hit dizzy and manhole result. Large windows use exact client sizing without changing monitor modes.
4. Run python Tools/run_animation_qa.py for 108 native frame/pair screenshots. Review strike/hurt frames for all eight characters and representative head/bubble/HUD/result captures at every size.
5. Run python Tools/audit_submission.py for default English candidates, glyph/mojibake, audio references and 38 WAV peak checks.
6. Run python Tools/finalize_evidence.py --local-only to verify current source/build fingerprint, XML, 65 log assertion result and fresh native capture hashes. It never rewrites the checklist or claims the queued ZIP passed.

Packaging/Drive are a separate queued task. After its pre-check passes, that task runs Tools/package_portable.py with the same ILABAN_PROJECT, verifies fresh extraction/full match, then Tools/finalize_evidence.py without --local-only to compare every ZIP build file to the current build. Old Docs/package_verification.json is historical until then.

Manual pass/fail sheet, after the final ZIP is ready:

- Two physical USB controllers, independent input: [ ] PASS [ ] FAIL.
- Keyboard/controller switch and unplug/replug during match: [ ] PASS [ ] FAIL.
- Shared keyboard simultaneous movement/punch rollover: [ ] PASS [ ] FAIL.
- Clean Windows 10/11 64-bit PC without Unity: [ ] PASS [ ] FAIL.
- Speaker/headphone balance and every item cue: [ ] PASS [ ] FAIL.
- Presentation monitor/fullscreen, Alt+Enter and 16:10 fit: [ ] PASS [ ] FAIL.
- Unsigned-app antivirus/SmartScreen handling: [ ] PASS [ ] FAIL.
- Human Tagalog, art consistency, grip/pose/readiness review: [ ] PASS [ ] FAIL.
- Reboot/real limit reset auto-resume with App/IDE hosts closed: [ ] PASS [ ] FAIL.

A development-PC extraction test never counts as a clean-PC or physical-device test. Do not publish until the owner resolves the public reference-photo history hold.

## UPDATE2 final desktop evidence
Normal single-EXE double-click, native 1920x1080 fullscreen, Alt+Enter toggle/back, Controls pointer route, Hindi-default dialog and actual Oo quit: verified. Desktop-injected Escape navigation inconclusive; physical keyboard/controllers/USB, listening, Editor stop and clean-PC/SmartScreen remain manual. Two independent EXE-only simulated matches to three wins passed.
