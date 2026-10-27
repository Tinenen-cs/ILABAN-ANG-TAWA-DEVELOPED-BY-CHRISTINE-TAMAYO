# HANDOFF: ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO

Branch `phase3-polish` (local only, NOT pushed, see B1). Never merge to `main` without the student's approval.
Verified 2026-10-28 by Claude Code: batch build + 65 rule assertions PASSED, EditMode 7/7, PlayMode 4/4, 0 compile errors/warnings.

## !! PRIVACY ISSUE (student decision needed)
`CHARACTER REFERENCE/` (8 photos of real people, one a child) was committed by Codex in `ec68355` and is PUBLIC on GitHub
(`origin/main`, `origin/phase2-ui`; repo visibility PUBLIC). On `phase3-polish` the folder is now untracked and gitignored (files kept on disk).
Removing it from GitHub needs a history rewrite + force-push (or making the repo private). ONLY with the student's explicit OK.
Do NOT push `phase3-polish` until the student decides.

## A. DONE (verified)
| # | Item | Evidence | Verified by |
|---|---|---|---|
| A1 | Product/company/window title renamed | ProjectSettings.asset productName/companyName; commit ec68355 | file check, exe title bar |
| A2 | GitHub repo renamed | remote https://github.com/Tinenen-cs/ILABAN-ANG-TAWA-DEVELOPED-BY-CHRISTINE-TAMAYO | git remote |
| A3 | Old folder kept as backup | `..\Fight-Fght-main` untouched | dir listing |
| A4 | CHECKLIST, TEST_PLAN, CREDITS, AUDIO_LIST, BUG_LOG, HANDOFF, CODEX_RESUME exist | repo root | ls |
| A5 | Combat: one step back, fair simultaneous punches, no jump in any state, punch only, 5-hit dizzy, no health/K.O./timer, manhole ends round at once | DuelModel.cs, RubricTests.cs | 65 assertions + EditMode 7/7 |
| A6 | Isolated P1/P2 maps, gamepad + generic joystick, device removal/replacement, simultaneous presses | AcademyInputManager.cs, InputAndCanvasTests.cs | PlayMode 4/4 (simulated devices) |
| A7 | Title shows exact `ILABAN ANG TAWA` + `DEVELOPED BY CHRISTINE TAMAYO` (was invisible, fixed) | commit 010278b | exe screenshot |
| A8 | Roster 8 Tagalog names + descriptions, ReaksyonTeksto.json, Pormal default | Roster.cs, Assets/Resources/ReaksyonTeksto.json | tests + smoke PASS.txt |
| A9 | Fonts Lilita One (KalyeDisplay) + Nunito Bold (KalyeBody), SDF Bilinear, outline + shadow, OFL license files | commit 31412d8, Assets/Fonts | smoke: no MISSING GLYPH; EditMode test |
| A10 | 3 approved maps 1672x941, Bilinear, cover fit, imburnal at both fall points, clouds / lamp flicker / jeepney / floor shimmer | commit d9e2630, Assets/Resources/Ilaban/Maps | QA_SCREENSHOTS/Fonts_1280x720/04-match.png |
| A11 | Opening (Pambungad) and title (Pamagat) scenes before Main | ProjectSettings/EditorBuildSettings.asset | build + smoke |
| A12 | Character design notes (original adults, photo policy) | Docs/CHARACTER_DESIGN.md, commit 6e14e15 | review |
| A13 | 8 portraits (Figma, text prompts only) incl. Ate Bato-Bato Pik ponytail redo | Assets/Art/Characters/Source/portrait_N.jpg; 9effafa, 29718e1 | QA_SCREENSHOTS/portrait_sheet.png |
| A14 | 8 full-body base poses, chroma-keyed | Assets/Art/Characters/Keyed/body_N.png; 29718e1 | QA_SCREENSHOTS/body_sheet.png |
| A15 | Windows x64 build + portable smoke at 1280x720 | Builds/Windows (gitignored) | `ILABAN PORTABLE SMOKE PASSED` in Player.log |

## B. PARTIAL / IN PROGRESS
| # | Item | Exists | Missing | Next action |
|---|---|---|---|---|
| B1 | Push | main, phase2-ui on GitHub | phase3-polish not pushed | Wait for the privacy decision, then `git push -u origin phase3-polish` |
| B2 | New characters in game | Runtime art exported: Assets/Resources/Ilaban/Characters/<NewKey>/Body.png (512 tall) + Portrait.png (512x512) by `python Tools/export_characters.py` | Game still shows OLD art/keys (Lyra..Dorian) | Build scripted rig in AcademyGame.ActorView/Animate (slice Body.png into head/torso/legL/legR; code-driven idle, walk, suntok with item skin, hit/step-back, dizzy, fall, win; effects). Then set Roster.Files to the new keys (list in Tools/export_characters.py), load `Ilaban/Characters/<Key>/Portrait` with full uvRect (not the 2x2 sheet), update ProjectSetup.cs line 78 check. Re-run rules tests |
| B3 | Select screens | Grid of 8 cards, 3 stage cards, independent P1/P2 confirm + Handa na gate | Slanted single row (or 2x4) with two-line name plates; names truncated (BUG_LOG C3) | Rebuild Select()/StageSelect() per approved plan; check 1280x720 + 1366x768 |
| B4 | Match HUD | Round portraits + ring, name, device label, 5-dot Hilo Meter, one Suntok orb per side, `Ihinto [ESC]` | Small VS mark; readability on all maps/times | Polish after B2 |
| B5 | Audio | Synthesized clips: separate Palengke/Looban/Plaza music, Title, Selection, item hits; AUDIO_LIST.md; Pangunahing Lakas / Musika / Tunog settings | CC0 downloads, opening + win music, loudness normalization, needs-manual-download table | Follow AUDIO_LIST.md |
| B6 | Scene order | Pambungad -> Pamagat -> Main | Tauhan / Entablado / Laban / Resulta are SCREENS inside Main.unity, not scenes | Split into scenes or document clearly |
| B7 | QA screenshots | 1280x720 sets (QA_SCREENSHOTS/B_1280x720, Fonts_1280x720) | 1920x1080, 1366x768, 2560x1440; all 9 map versions | Run smoke at each resolution |
| B8 | CHECKLIST.md | Phase 1/2 content from Codex | Not updated this session | Rewrite as master checklist + Rubric Self-Check |

## C. PENDING (not started)
| Item | Output | Notes |
|---|---|---|
| Umaga/Hapon map versions (6) | see E | lamps OFF, same composition |
| Oras selector | AcademyGame.StageSelect: Up/Down = `Oras: Umaga / Hapon / Takipsilim`, default Takipsilim | visual only; music follows map; mist (Umaga), sun shimmer (Hapon); lamp glow only at Takipsilim (MapLamps in AcademyGame.cs) |
| Screen backgrounds (4) | see E | |
| English-string scan | all .cs / .json / .unity | only allowed English: the title format |
| Full simulated match in exe | Phase2Smoke.cs | extend to real punches into the manhole |
| SUBMISSION | `SUBMISSION/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.zip` + `BASAHIN_MUNA.txt` (pure Tagalog) | extract to temp, launch, check Player.log |
| Rubric Self-Check table | end of CHECKLIST.md | 3 criteria with evidence |

## D. NEEDS MANUAL TEST (never mark passed)
Physical USB gamepad; second gamepad; hot-swap keyboard and gamepad mid-match; unplug/replug feel; two players on one keyboard;
clean PC without Unity; speaker and headphone loudness; the student's real monitor; antivirus/SmartScreen on the unsigned exe;
title screen left idle 60 s (BUG_LOG C2).

## E. Image work for Codex
| Image | Path | Size | Status | Style notes |
|---|---|---|---|---|
| Portrait redo: Ate Bato-Bato Pik (high ponytail) | Assets/Art/Characters/Source/portrait_2.jpg | 1024x1024 | EXISTS (accepted) | |
| Portrait redo: Tito Silya (match other 7) | Assets/Art/Characters/Source/portrait_7.jpg | 1024x1024 | Original KEPT; one redo rejected (looked too young) | Optional redo only if clearly ADULT (30s, stubble) |
| 8 full-body base poses | Source/body_N.jpg -> Keyed/body_N.png | 896x1200 -> 1024 tall | EXIST | |
| Runtime character art | Assets/Resources/Ilaban/Characters/<Key>/Body.png, Portrait.png | 512 tall / 512x512 | EXIST | regenerate: `python Tools/export_characters.py` |
| Extra expression heads (optional) | Assets/Resources/Ilaban/Characters/<Key>/Head_Hurt.png | 512x512 transparent | MISSING (optional) | only if a pose needs it |
| Palengke / Looban / Plaza Umaga | Assets/Resources/Ilaban/Maps/<Map>/<Map>_Umaga.png | 1672x941 | MISSING | same composition; soft cool sunrise, pale golden-blue sky, lamps and bulbs OFF, light mist, long soft shadows |
| Palengke / Looban / Plaza Hapon | Assets/Resources/Ilaban/Maps/<Map>/<Map>_Hapon.png | 1672x941 | MISSING | bright warm sun, blue sky + white clouds, lamps OFF, short crisp shadows, mostly dry floor |
| Takipsilim (approved) | Assets/Resources/Ilaban/Maps/<Map>/<Map>_Takipsilim.png | 1672x941 | EXISTS, do not modify | |
| Title (Pamagat) | Assets/Resources/Ilaban/Backgrounds/Pamagat.png | 1920x1080 | MISSING | painterly dusk street like the maps; clear space for logo top-center and menu center |
| Character select | Assets/Resources/Ilaban/Backgrounds/PumiliTauhan.png | 1920x1080 | MISSING | barangay court or tindahan corner at dusk, wide empty middle band, lamp glow, vignette; must not look like a fight map |
| Map select | Assets/Resources/Ilaban/Backgrounds/PumiliEntablado.png | 1920x1080 | MISSING | softly blurred and darkened street, clear middle band, parol/bulbs, banderitas strip at top |
| Opening parallax layers | Assets/Resources/Ilaban/Backgrounds/Pambungad/{Sky,Skyline,Wires,Midground,Street}.png | 2560x1080, transparent except Sky | MISSING | dusk street; jeepney drives across (Assets/Resources/Ilaban/UI/Jeepney.png exists) |

Folders `Ilaban/Backgrounds/`, `Ilaban/Maps/`, `Ilaban/Characters/` import Bilinear, uncompressed (ProjectSetup.cs import rule).

## F. Decisions
3 maps only (Palengke, Looban, Plaza); 9 map backgrounds (each x Umaga / Hapon / Takipsilim); maps not pixelated.
UI references are layout only; Filipino street look (wood/tin signboards, parol, banderitas), no purple fantasy.
Lilita One + Nunito, no pixel text. Original chibi ADULT characters, no real-person likeness; photos used only for expression and
outfit, never uploaded. Pormal default. The item is only a visual skin of the straight punch.
Scene order Pambungad, Pamagat, Pumili ng Tauhan, Pumili ng Entablado, Laban, Resulta. No merge to `main` without approval.

## G. Resume commands
- Unity Editor must be closed for batch mode.
- Build + 65 checks: `"C:\Program Files\Unity\Hub\Editor\6000.6.2f1\Editor\Unity.exe" -batchmode -quit -projectPath "<proj>" -executeMethod Hexbound.Editor.FinalizationSetup.ValidateAndBuild -logFile Logs/batch-build.log`
- Tests: same exe with `-batchmode -projectPath "<proj>" -runTests -testPlatform EditMode -testResults Logs/EditMode-results.xml` (then PlayMode)
- Screenshots (in-game capture only, never screen-copy the desktop): `"Builds/Windows/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe" -screen-fullscreen 0 -screen-width 1280 -screen-height 720 -ilaban-smoke "<abs folder>"`

## H. Figma
https://www.figma.com/design/9J9nqsjyg38lIZVMeLXRYC (Codex cannot use the Figma generator; prompts follow Docs/CHARACTER_DESIGN.md)

## Codex continuation: character rig step (28 October 2026)
Approved original base art now split into six disjoint registered visual parts by Tools/rig_characters.py. Runtime loads the new Filipino keys, full portraits, and code-driven part transforms. No real-person photos used or uploaded. UI name-label heights adjusted. Runtime compilation/tests still pending for this step. No push or merge permitted.
