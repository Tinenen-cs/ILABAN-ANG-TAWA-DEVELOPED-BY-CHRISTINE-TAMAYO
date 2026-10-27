# Current handoff ? 28 October 2026

Local branch **phase3-polish**. No push or merge authorized. Private reference photos remain ignored and on disk. No original assets deleted. Earlier remote photo history is an owner decision; do not rewrite it.

## Implemented in this continuation

- Approved original adult character art now active: Filipino internal resource names, full portrait UVs, six-part visual rigs, aspect-preserving display. Original six accepted faces kept; accepted ponytail portrait and existing adult Tito Silya portrait retained.
- Scripted idle/walk/punch/hurt/dizzy/fall/win and item flourishes, simple effects. Visual transforms do not access hitbox/force/timing. This is a simple cut-out rig, not finished joint-based animation; expression-head swaps remain partial.
- Six generated morning/afternoon map edits, original dusk maps untouched. Oras selector changes visuals only; only dusk enables lamp glow.
- Original title, character-select, stage-select and opening backgrounds; five aligned parallax PNGs. Image tool inputs contained no private photographs.
- Two-line roster names, more space for readiness/controls, dark HUD backplates, reduced seam-exposing part motion, menu fades. Layout-only UI reference use; warm wood/Philippine scenery remains distinct.
- No dead spell/knockout pose names or unused round helpers. Jump enum is a forbidden-state test sentinel, not an action.
- Complete updated provenance, source prompts, privacy rules, audit scripts, master checklist and manual test script.

## Evidence and current verification

- Earlier checkpoint: build, 65 rule assertions, EditMode 7/7 and PlayMode 4/4.
- Expanded EditMode result: **9/9 passed**, including all nine map/time textures and eight complete character rig sets. Result XML copied to Docs/QA_EVIDENCE/EditMode.xml.
- Preliminary portable 1280x720 smoke: actual simulated gamepad movement/punches caused fifth-hit dizzy and manhole result. Screen captures identified fit/contrast/seam issues, fixed afterward.
- Static active-English keyword scan: zero candidates. Audio audit: 30 active clips, zero clipped samples. These are heuristic/code checks, not a perfect translation or auditory-quality claim.
- Final release rebuild, all-four-resolution smoke and portable ZIP extraction checks are being completed. Consult Docs/portable_qa_results.json and the append-only final results below before assuming completion.
- Batch test runner occasionally stalls during editor lifecycle. Tests/build run in an isolated copy; the user's editor is not forcibly closed. A written pass XML is distinguished from clean process exit.

## Files / commands

Read CODEX_RESUME.md for the exact PowerShell resume command. Build entrypoint: Hexbound.Editor.FinalizationSetup.ValidateAndBuild. Tools/run_portable_qa.py captures all required resolutions and fails on runtime exceptions, missing glyphs, actual TMP overflow or incorrect capture size. Tools/audit_submission.py writes Docs/FINAL_STATIC_AUDIT.json. Never screen-copy the desktop; use in-game capture only.

## Remaining non-automatable checks

Real USB controllers and second controller, hot-swap feel, unplug/replug, keyboard rollover/latency, speakers/headphones, actual monitor, unsigned-executable antivirus behavior, and a clean PC without Unity. Mark NEEDS MANUAL TEST, never passed by inference.

## Submission

Once present and verified, upload only SUBMISSION/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.zip to Canvas. The ZIP includes the EXE, Data folder, UnityPlayer.dll, Mono runtime and Tagalog BASAHIN_MUNA.txt. CHECKLIST.md and QA_SCREENSHOTS are optional extras if the professor allows them.

## Claude Code notes (2026-10-28, stopped at student's request)
- Found: SUBMISSION zip (05:02) predates the final build log (05:06) and last commit (05:04), and was built from the TEMP validation copy. Rebuild + re-zip before submitting.
- Tools/package_portable.py, run_portable_qa.py, capture_large_window.py, run_final_checks.py now accept env ILABAN_PROJECT (default = TEMP validation copy).
- BUG: dizzy stars orbit at y=335, hidden behind reaction bubbles (y 263-332); in QA_SCREENSHOTS/Final_1280x720/05-dizzy.png the dizzy indicator is not visible. Fix in AcademyGame.UpdateDizzyStars (raise/enlarge, draw last).
- BUG: P1/P2 bubbles touch when fighters are close (AcademyGame bubble x shift 240 < width 250).
- Tools/finalize_evidence.py (Codex) has not been run yet; run it after the rebuild/QA/zip.

## Final correction step 3 evidence

- Clean working branch commit 3979522 was cloned locally into %TEMP%\Ilaban-Final-Validation; git rev-parse HEAD matched real project exactly in both. User Unity Editor stayed open on real project.
- Unity 6000.6.2f1 final build completed from clone; 65 rubric assertions passed. No error CS or warning CS entries.
- Final corrected-source EditMode 9/9 and graphics-enabled PlayMode 4/4 passed after the supported asterisk fix. XML saved to Docs/QA_EVIDENCE/EditMode.xml and PlayMode.xml.
- Portable QA and screenshot inspection are rerunning on the corrected standalone build. Fresh package follows.


- The first portable screen pass caught a missing star-font glyph. It was replaced with a supported large asterisk; the corrected build has 65 assertions, EditMode 9/9, and graphics PlayMode 4/4.
