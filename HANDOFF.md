# HANDOFF (Claude Code session, 2026-10-28)

Branch: `phase3-polish`. `main` untouched — never merge without the student's approval.

## Audit results (Section 1)
- Codex left 58 uncommitted/untracked changes. Committed as `3e70b0d` WIP checkpoint.
- Compile: interactive editor's log (Logs/editor.log) shows an earlier CS0119 at AcademyGame.cs:97,
  already fixed; last two Tundra builds succeeded after the final code edit.
  Remaining: 2x CS0618 (FindFirstObjectByType obsolete) at AcademyGame.cs:28 and :50.
- Batch-mode compile NOT yet run: Unity Editor PID 22572 has this project open (blocks batch mode).
- No built exe exists in the project (no Builds/ folder). Codex's build output is gone or elsewhere.
- Stage PNGs (Palengke/Looban/Plaza) exist at 640x360: Codex placeholders, not AI-generated finals.
- `UI REFERENCE` screenshots: NOT FOUND in either folder. Need from the student.

## Next steps
1. Close the editor (with the student's OK), then run batch compile + EditMode/PlayMode tests.
2. Fix CS0618 warnings (use FindAnyObjectByType).
3. Rebuild Windows x64, smoke test, then work through items B–K in order.

## Codex session findings (read-only: ~/.codex/sessions/2026/10/28/rollout-...01a17e88....jsonl)
- Student decisions: folder renamed to the full title; ONLY 3 maps (Palengke, Looban, Plaza); 8 roster names confirmed;
  UI references = LAYOUT ONLY, keep Filipino street theme.
- Codex built/validated in a COPY of the project: `%LOCALAPPDATA%\Temp\Ilaban-Phase2-Validation` (has Builds\Windows\*.exe).
  Its last actions: launching that exe for screenshots into QA_SCREENSHOTS\Preliminary; robocopying Fonts back into the real project.
  That copy is reference only; the real project is the source of truth.
- AI backgrounds DID finish: ~/.codex/generated_images/01a17e88.../ (3 PNGs, 1672x941). Copied to
  Assets/Resources/Ilaban/Stages/HD/{Plaza,Palengke,Looban}_HD.png. Not wired into the game yet. 640x360 placeholders kept.
- UI references saved in `UI REFERENCE/` (character select, match HUD, arena select), provided by the student in chat.

## Progress (Claude Code)
- Unity editor closed. Batch compile: 0 errors, 0 warnings (the CS0618 warnings were stale).
- Tests: EditMode 7/7 passed, PlayMode 4/4 passed (Logs/*-results.xml). Build validation: 65 rubric assertions passed.
- Build command: `Unity.exe -batchmode -quit -projectPath <proj> -executeMethod Hexbound.Editor.FinalizationSetup.ValidateAndBuild`
  -> Builds/Windows (gitignored).
- Smoke test: exe runs, Player.log clean. Fixed the invisible title (BUG_LOG C1). Open: C2, C3.
- SCREENSHOTS: use PrintWindow on the game HWND only (see BUG_LOG C4); never screen-copy.
- C2 closed (not a bug). QA screenshots: use exe arg `-ilaban-smoke <folder>` (Phase2Smoke.cs, in-process capture).
- NEXT: item B (wire Stages/HD backgrounds + manhole + animated parts) -> C (slanted card layout) -> D..K.

## STUDENT UPDATE (map art / fonts / time of day / screen backgrounds) - supersedes pixel maps and Tiny5
1. [x] Approved maps: Codex originals 1672x941 (MAP ART/APPROVED did not exist) -> Assets/Resources/Ilaban/Maps/<Map>/<Map>_Takipsilim.png
2. [x] Full-res maps, Bilinear (ProjectSetup import rule), cover fit, soft FX (Tools/generate_map_fx.py): clouds, lamp flicker,
       jeepney, floor shimmer, imburnal at fall points. Commit d9e2630.
3. [x] Fonts: Lilita One (KalyeDisplay) + Nunito Bold (KalyeBody), SDF Bilinear, outline+underlay; tests updated (7/7, 4/4)
4. [ ] Umaga/Hapon versions + Oras selector (Up/Down on Pumili ng Entablado)
5. [ ] Backgrounds: Pambungad (layers), Pamagat, Pumili ng Entablado, Pumili ng Tauhan
6. [ ] UI redo (C/D), QA at 4 resolutions, SUBMISSION zip
RESUME: build = `Unity.exe -batchmode -quit -projectPath <proj> -executeMethod Hexbound.Editor.FinalizationSetup.ValidateAndBuild`
        smoke = `"Builds/Windows/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe" -screen-fullscreen 0 -screen-width 1280 -screen-height 720 -ilaban-smoke <abs folder>`
