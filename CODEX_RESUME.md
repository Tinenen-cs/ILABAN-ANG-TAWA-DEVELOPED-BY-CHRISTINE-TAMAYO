# Resume this project

Working folder: `C:\Users\chris\Documents\CSEL GAMEDEV FILES\ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO`.
Stay on **phase3-polish**. Read HANDOFF.md, CHECKLIST.md and Docs/CHARACTER_DESIGN.md. One editor agent at a time.

**Do not push, force-push, rewrite history or merge.** The owner decides after resolving repository privacy. Real-person reference photos are local-only and ignored; never add or upload them, including to an image tool.

Current art: 8 approved fictional adult bases/portraits now connected using Filipino keys and six registered scripted visual parts; no old academy faces in active play. Original accepted art preserved. 3 maps x Umaga/Hapon/Takipsilim, smooth painterly style, lamps off by day. Four original menu/opening backgrounds and five parallax layers are imported. Fonts are Lilita One/Nunito, not pixel fonts. Pormal remains default.

Tests/build are run in `%TEMP%\Ilaban-Phase2-Validation` while the user's working Unity editor stays open. Latest portable builds are copied back after verification. The 1280x720 preliminary actual-input match passed; screenshot findings were corrected. Current final verification status is in HANDOFF.md and Docs/portable_qa_results.json when present.

Exact resume command (PowerShell):

```powershell
Set-Location -LiteralPath 'C:\Users\chris\Documents\CSEL GAMEDEV FILES\ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO'
git switch phase3-polish
Get-Content -LiteralPath CODEX_RESUME.md
Get-Content -LiteralPath HANDOFF.md
```

Do not re-run Tools/generate_ilaban_assets.py blindly: it regenerates procedural fallback art/audio and does not recreate approved generated map/character art. Source map/screen prompts and output paths: Docs/new_art_manifest.json and Docs/opening_layers_manifest.json. Tools/rig_characters.py creates the deterministic rig partitions only.

Latest verification: 3979522 is cloned in %TEMP%\Ilaban-Final-Validation with exact matching HEAD. Final build passed 65 rule assertions; EditMode 9/9, graphics-enabled PlayMode 4/4. QA/package remain in progress.

