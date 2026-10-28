# Rubric verification - incoming Hilo display

- [x] Precheck: previous update implementation tasks1-10 complete; existing manual checks remain documented in HANDOFF.md.
- [x] Display mapping changed only: each HUD reads the opponent current attack streak, showing incoming consecutive hits on the victim side.
- [x] DuelModel.cs unchanged; five straight consecutive hits, shared range/timing/one-step knockback and all streak-break rules retained.
- [x] Exactly five segments, Tagalog Hilo / English Dizzy; no health/KO display or damage-drain behavior.
- [x] EditMode15/15 (includes RubricTests), graphics PlayMode17/17 including asymmetric real-hit integration;65 rule assertions. All process exits0; build zero C# diagnostics.
- [x] Captured48 native1280x720 screenshots across all3 maps/both attackers, empty/fill1-5/fifth-hit victim dizzy/recovery/streak-break. Viewed all six contact sheets and native fill1-5. Evidence: Docs/QA_EVIDENCE/hilo_victim_capture.json; screenshots: QA_SCREENSHOTS/HILO_VICTIM.
- [x] Real-folder Windowsx64 build passed; completeZIP/singleEXE repack next.
- [ ] Pending isolated EXE-only full-match tests, FILE_INFO size/SHA256, main push and verified GitHub Release.

NEEDS MANUAL TEST: physical keyboard/gamepad/USB/hot-swap, human sound/art/Tagalog review, cleanPC and strict steady60fps. Historical Drive binary replacements remain pending and are outside this request.
