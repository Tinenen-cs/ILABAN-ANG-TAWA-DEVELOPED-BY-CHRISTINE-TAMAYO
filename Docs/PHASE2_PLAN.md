# Phase 2 implementation plan

Status: preparation; no existing gameplay or UI code replaced.

## Verified starting point

- Working project: ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO, Unity 6000.6.2f1.
- Branch: phase2-ui, based on main checkpoint 42d5b65.
- Backup: ../Fight-Fght-backups/Ilaban-before-phase2-20261028-021648.tar.
- No Windows standalone build or portable ZIP exists in the project.
- Main.unity is the only scene. No prefab files are present.
- AcademyGame.cs draws all menus, HUD, results and help using immediate-mode GUI.
- DuelModel.cs still permits jumping, blocking, shoving, spells, combos and timeout draws.
- AcademyInputManager.cs uses separate action maps but disconnecting a controller pauses gameplay.
- Existing art and music are still academy-themed. Baseline import/rule checks and 36 virtual-device input assertions passed previously; these do not verify the new rubric.
- UI REFERENCE is absent in both the old and named project folders.

## Confirmed decisions

- Use this project and complete the missing Phase 1 work on this branch.
- Preserve Main.unity, its GUID and the original resource identities; keep original art backed up.
- Final maps: Kalye sa Palengke, Looban, Plaza ng Barangay.
- Final names confirmed by user: Aling Pamalo, Kuya Tsinelas, Ate Bato-Bato Pik, Kap Batuta, Manong Latalata, Tito Silya, Aling Boteng Suka, Nanay Tingting.
- Keep Pormal as the graded build default; Kabataan is optional in settings.
- No merge into main until explicit user approval.

## Proposed work sequence

1. Establish the rubric rules: horizontal-only movement, one equal straight punch, fixed step-back displacement, attacker's five-hit streak and victim dizzy recovery, instant manhole round end. Remove timeout wins and disallowed moves from active input/UI. Update rule tests to check the new behavior.
2. Rebind P2 punch to Right Ctrl/Numpad 0; preserve independent keyboard maps and exclusive controller ownership; remove disconnect-induced pause. Test virtual devices and document physical USB checks separately.
3. Add a Canvas with Canvas Scaler and anchored safe regions inside the existing Main scene. Use separate components for character selection, stage selection, match HUD, controls/settings and results. Keep game rules independent of layout code.
4. Match the supplied reference screenshots after they are provided. Character selection uses a horizontal card carousel with visible P1/P2 cursors and ready states. Stage selection uses three tall slanted cards, with a highlighted-card glow, a Tagalog description and bottom-corner Bumalik/Simulan buttons. All menus support keyboard and gamepad.
5. Match HUD: circular corner portraits and names; device prompts below; five dots represent the attacker's streak, never remaining life. Remove timer, poise, mana and extra ability icons. Keep one Suntok orb per side and a central pause hint. Preserve visible edge manholes and safe space for reaction bubbles.
6. Store all reaction lines in Resources/ReaksyonTeksto.json, with six categories per language and two or three item-specific lines per character. A separate presenter handles duration, fade, pop, shake, repeat prevention and cooldown; it never mutates the combat model or consumes gameplay random state.
7. Recreate assets additively under Resources/Ilaban and preserve the old resources. Use original faces, pixel/chibi bodies, Point filtering and no compression. Track placeholders explicitly. Add normalized original/CC0 audio, per-file credits and master/music/SFX controls.
8. Append Phase 2 acceptance cases to CHECKLIST.md and TEST_PLAN.md. Validate four required 16:9 sizes plus 16:10; run rule/input checks and interactive playtests. Build Windows x64 using Mono, default Pormal, package the complete portable folder, and test the extracted copy.
9. Commit and push phase2-ui; leave merge pending review. Clean-PC and physical-controller results require actual hardware testing and must remain pending until performed.

## Existing files expected to change

- Assets/Scripts/Runtime/AcademyGame.cs: replace the old visual screens and route game events.
- Assets/Scripts/Runtime/DuelModel.cs and RuleChecks.cs: implement and verify rubric behavior.
- Assets/Scripts/Runtime/AcademyInputManager.cs: restricted controls and hot-swap.
- Assets/Scripts/Runtime/Roster.cs and Resources/Input/AcademyControls.inputactions: confirmed names, equal items, stage metadata and bindings.
- Assets/Scripts/Editor/ProjectSetup.cs, InputChecks.cs, PreviewCapture.cs and SubmissionSetup.cs: validation/build/preview integration for the new rules and layout.
- Assets/Scenes/Main.unity, Packages/manifest.json, Packages/packages-lock.json and selected ProjectSettings files: Canvas/UI setup and required package/settings changes only.
- README.md, CREDITS.md, CHECKLIST.md and TEST_PLAN.md: accurate progress, credits and observed results.

No original images or audio need to be deleted or overwritten. New art/audio will have new paths. Retain old identifiers internally when renaming them would break references; player-visible names and final documentation use the confirmed names.

## Inputs still needed

- The match-HUD screenshot and arena-selection screenshot, in UI REFERENCE or at an exact supplied local path.
- Approval to modify the existing files listed above, because the user's Phase 2 brief explicitly says to ask before overwriting anything.
