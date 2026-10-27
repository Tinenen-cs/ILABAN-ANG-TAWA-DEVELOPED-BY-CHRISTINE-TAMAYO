# CODEX RESUME (read this first)

Branch: `phase3-polish` (never merge to `main` without the student's approval). Last commit: see `git log -1`.
Only ONE agent works on the repo at a time. Full detail: `HANDOFF.md`, `BUG_LOG.md`, `CHECKLIST.md`.

## Student decisions (do not change)
- 3 maps only: Palengke, Looban, Plaza. 9 map backgrounds = 3 maps x (Umaga, Hapon, Takipsilim). Takipsilim = approved art, unmodified, full res, Bilinear.
- UI references (`UI REFERENCE/`) are LAYOUT ONLY. Filipino street look (wood/tin signboards, parol glow, banderitas).
- Fonts: Lilita One (`KalyeDisplay`) + Nunito Bold (`KalyeBody`), SDF, Bilinear. NO pixel text.
- Characters: ORIGINAL chibi ADULTS, no likeness of the reference photos, photos never uploaded. Portraits approved (Figma).
- Reaction language default = Pormal. Pure Tagalog UI; only English allowed: `ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO`.
- Graded rules: left/right only, no jump, straight punch only, exactly one step back, 5-hit dizzy, no health/K.O./timer, manhole ends round.

## Commands
- Build + 65 rule checks: `Unity.exe -batchmode -quit -projectPath <proj> -executeMethod Hexbound.Editor.FinalizationSetup.ValidateAndBuild -logFile Logs/batch-build.log`
- Tests: `Unity.exe -batchmode -projectPath <proj> -runTests -testPlatform EditMode|PlayMode -testResults Logs/<x>.xml`
- In-game screenshots (never screen-copy the desktop): `"Builds/Windows/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe" -screen-fullscreen 0 -screen-width 1280 -screen-height 720 -ilaban-smoke <abs folder>`

## Next steps (status kept current in HANDOFF.md)
| Step | Needs Figma/image tool? |
|---|---|
| Full-body base poses for 8 characters (`Assets/Art/Characters/Source/body_N.png`) | YES (Figma file https://www.figma.com/design/9J9nqsjyg38lIZVMeLXRYC) |
| Any regenerated character art / new expression heads | YES |
| Split bodies into parts + scripted animation (idle, walk, suntok, hit, dizzy, fall, win) + effects | No (Codex can do) |
| Umaga/Hapon recolors (script), Oras selector, screen backgrounds by script | No |
| UI redo (slanted cards, HUD), audio, English scan, QA at 4 resolutions, build, SUBMISSION zip | No |
