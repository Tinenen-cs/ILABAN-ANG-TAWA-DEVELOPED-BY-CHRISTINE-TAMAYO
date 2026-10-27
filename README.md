# ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO

A local two-player Philippine street brawler by Christine Tamayo, for the 2026 midterm. Open in Unity **6000.6.2f1**. Work is on local branch `phase3-polish`; do not push or merge without the owner's decision.

## Play

The portable Windows folder is packaged under SUBMISSION when final checks finish. Extract the entire ZIP and double-click **ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe**. Keep its Data folder, UnityPlayer.dll and MonoBleedingEdge together. No Unity installation is needed on the target PC; a clean-PC test remains a separate manual check.

P1: A/D to walk, F to punch/confirm. P2: left/right arrows to walk, Right Ctrl or Numpad 0 to punch/confirm. Gamepad: stick/D-pad to walk, South or West face button to punch/confirm. Escape pauses; H opens controls/settings. The first controller owns P1, the next P2; keyboard remains available. Generic joystick trigger is supported, subject to hardware driver compatibility.

Choose both characters independently and confirm each. Choose Palengke, Looban or Plaza; Up/Down changes Oras between Umaga, Hapon and Takipsilim. Time of day changes art only. Every straight punch steps the target back exactly 52 logical units. Five consecutive hits cause a two-second dizzy recovery; a miss, three-second gap or being hit breaks the attacker's streak. Only falling into an edge imburnal ends the round. No health, timer, jump or extra moves.

## Project and verification

Pambungad and Pamagat are scenes, followed by Main. Character selection, stage selection, combat and results are screens within Main; scene structure is intentionally preserved. Runtime Canvas and cover-fit scenery keep proportional UI and full-screen backgrounds. New adult characters use scripted visual parts, independent of combat rules. Pormal reactions are the default on every launch; Kabataan can be selected in Mga Kontrol.

- CHECKLIST.md: continuous status and rubric evidence.
- TEST_PLAN.md: automated results and manual checks.
- BUG_LOG.md: observed problems and fixes.
- AUDIO_LIST.md and CREDITS.md: all sound/font/art provenance.
- Docs/new_art_manifest.json: generated map/screen artwork and prompt summaries.
- CODEX_RESUME.md and HANDOFF.md: exact continuation state.

No original art was deleted. Legacy academy resources remain for rollback and can increase build size. The old Fight-Fght-main folder remains untouched. Real-person reference photos are ignored on this branch and must not be pushed; earlier remote history is for the owner to resolve. Do not force-push or rewrite history automatically.

## Build and tests

Use FinalizationSetup.ValidateAndBuild in Unity batch mode, or Ilaban ang Tawa > Build Windows x64. Tools/run_final_checks.py runs EditMode, PlayMode and build on the isolated validation copy. Run Tools/audit_submission.py for audio peak checks and heuristic UI-string scanning. Use the built game's `-ilaban-smoke <absolute-output-folder>` argument for opt-in captures and an actual simulated gamepad match. Normal play does not execute the harness.

Physical USB behavior, keyboard rollover, audio listening, the real monitor, unsigned-executable warnings and a PC without Unity require manual verification. Generated art and procedural animation remain a stylized student prototype; see the final checklist for partial polish items.
