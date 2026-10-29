# Bug log

Record only observed failures or concrete code defects, with verification status. Final results are appended after test runs.

| Issue | Fix | Verification |
| --- | --- | --- |
| Initial project still allowed jump, spells, guard, shove, combos and timeout draws | Replaced active rules with horizontal movement, equal punch and manhole-only ending | 64 initial pure C# assertions passed; expanded tests pending |
| Frame-dependent decaying knockback did not move a fixed distance | One direct 52-unit backward step per landed hit | 8 items, both directions and no residual drift passed; 50-hit Unity case added |
| Sequential attack resolution favored P1 | Snapshot both active strikes before applying either | Simultaneous displacement assertions passed |
| A simultaneous trade could incorrectly award fifth-hit dizziness | Trades now break both streaks before any dizzy award | New regression case added; rerun pending |
| New pad was assigned after its first event and missed its first punch | Pair on device-added/reconnected notification before processing input | Replacement regression rerun pending |
| Old disconnect behavior forced pause | Disconnect now preserves game state and keyboard availability | Virtual-device and interactive smoke verification pending |
| Both players could confirm with the same Enter key | Enter belongs only to P1; P2 uses Right Ctrl/Numpad 0 | Isolation cases passed before reconnect test |
| UI helper named Image hid Image.Type enum | Qualified UnityEngine.UI.Image.Type | Subsequent compilation reached validation |
| Edit-mode input simulation ignored runtime action processing | Run standalone input checks in Play Mode; add InputTestFixture tests | Initial keyboard/isolation cases passed in Play Mode |
| Result overlay covered reaction bubbles and fall presentation | Moved results to upper-center safe region; bubbles above actors | Graphical verification pending |
| Mirroring an actor with top-left pivot displaced it | Mirror atlas UV instead of the whole actor root | Graphical verification pending |
| TMP font creation accessed absent TMP Settings | Create and save TMP Settings before creating the static atlas | Build rerun pending |
| Unity test asmdefs duplicated implicit test-runner references | Keep optional TestAssemblies references and remove duplicate explicit entries | Compile rerun pending |

Original artwork, face files, audio and Main.unity have not been deleted. Procedural placeholders are explicitly tracked in Docs/ASSET_REPLACEMENTS.md.

## Claude Code session (2026-10-28)
| # | Bug | Status |
|---|---|---|
| C1 | Title "ILABAN ANG TAWA" invisible: 82pt label in 91px rect with Truncate overflow hid the whole line (AcademyGame.cs:140) | FIXED: overflow mode on that label; verified in built exe screenshot |
| C2 | Title advanced to Pumili ng Tauhan with no input after ~15s in built exe | NOT A BUG: runInBackground=0, so only the focused game window gets input; the newly launched window had focus and received outside keystrokes. Title correctly advances on any key. NEEDS MANUAL TEST: leave title idle 60s untouched |
| C3 | Character card names truncated (Manong Latalata, Aling Boteng Suka, Ate Bato-Bato Pik) | OPEN: fix with item C layout |
| C4 | QA capture method used screen copy and grabbed other windows | FIXED: captures now use PrintWindow on the game window only |
| C5 | Phase 3 font swap: Tiny5 pixel font replaced by Lilita One + Nunito SDF (Bilinear, outline + underlay); arrow glyphs removed from controls text | FIXED: no MISSING GLYPH in smoke run |
| C6 | Smoke harness was destroyed by the Pambungad -> Pamagat scene load | FIXED: DontDestroyOnLoad |
| C7 | Labels overflowed their rects with the wider font | MITIGATED: TMP auto-size (min 60%); select screen redo pending |
| C8 | Character art policy | The real-person photos were used only as expression and outfit reference (see Docs/CHARACTER_DESIGN.md). No likeness reproduced. |

## Continuation fixes (28 October 2026)

- New approved character assets existed but runtime still loaded academy keys: connected Filipino resource keys and full portrait UVs; added eight complete rig-resource checks.
- Character-card long names truncated: two-line display names and taller readiness regions. Screenshot verification in progress.
- Bright map scenery reduced HUD contrast: added dark backplates without changing HUD gameplay content.
- Whole-canvas rotations of disjoint rig parts exposed seams: reduced local part rotations/displacements; whole-character dizzy/fall transforms provide the strong motion. Visual polish remains limited by the simple cut-out rig.
- Preliminary smoke captured a completed actual-input match but reported undersized readiness/label regions: enlarged key labels, bubble text area, punch labels, stars and rules text. Audit now uses TMP's actual overflow flag after auto-sizing, rather than unscaled preferred height.
- Direct batch launch failed while user had working project open: tests/build use an isolated source copy and leave user's editor intact.
- Dead spell/knockout pose labels and unused round-transition helpers removed. Jump enum retained only as a forbidden-state test sentinel.
- Heuristic active-string scan: zero English candidates. Audio sample audit: 30 files, zero clipped samples. These are not claims of perfect translation or listening quality.

- Final QA: Windows decorated-window clamp prevented native 1440p; exact-size borderless diagnostic capture passed. Intro capture advanced past opening during startup delays; bounded intro clock and required-opening harness fixed it. Headless PlayMode TMP importer needs graphics; graphics-enabled 4/4 result retained.


## 2026-10-28 final UI and animation pass

- Root cause of the split-on-hit glitch: fighters had separate arm, leg, torso and head textures, and the punch/hurt motion independently offset or rotated those pieces around a full-canvas origin. Replaced combat actors with one whole-body image per frame; knockback and facing now move/flip the parent sprite as one object. All 272 action frames share a transparent 272x512 canvas, bottom-center pivot and common ground line. Connectivity checks report one connected body silhouette for each of the eight base characters.
- Root cause of the thin stray "second stick" in the strike: the procedural strike frame drew a narrow straight line from grip to tip on top of the weapon already present in the full-body base art. Removed that line and replaced it with a broad, curved motion ribbon. Fight actors have no item/weapon child; item thumbnails remain selection-only. Verified in the refreshed strike montages.
- Strike poses remain PLACEHOLDERS: six timed whole-body frames have effects, but weapon-specific hand poses do not yet raise and extend each prop toward the opponent. Grip/tip anchors are stored in `Docs/character_weapon_anchors.json`; this is not proof that the artwork reaches the opponent. Do not mark weapon choreography complete.
- Added the original carved wood/brass portrait rings, segmented wooden Hilo meter, medal tallies, center VS badge and bottom hint plate. Removed fight HUD dark rectangles, `Sunod na Suntok`, and player control lines. Hilo has five amber-to-red pips, fills once per consecutive hit and resets with the streak. The PlayMode test checks five-pip ordering/reset and absence of fighter item objects.
- Reaction bubbles now follow each actor's head anchor, clamp to the screen and separate in close quarters. Bubbles are hidden on the result screen.
- Strike frames include a curved ribbon, impact sparkle/burst and foot dust. Item-specific hit audio uses `hit_0.wav` through `hit_7.wav`; impact playback is rate-limited. Dedicated per-character reaction voice cues remain partial.
- Unity graphics-enabled EditMode, PlayMode and build passed; 65 rule assertions and portable smoke passed at 1280x720, 1366x768, 1920x1080, 2560x1440 and 3440x1440. See `Docs/QA_EVIDENCE/`, `Docs/portable_qa_results.json`, `QA_SCREENSHOTS/HUD/` and `QA_SCREENSHOTS/ANIMATION/`.
- Fixed slider fill anchors that caused the control bars to grow outside their tracks. Added PlayMode size bounds. Hid reaction bubbles after a match ends so they do not compete with the result panel.
- Updated highlight/ready animation to show a selected item flourish and ready sparkle. Facial expression swaps, polished limb motion and dedicated reaction voice cues remain partial.
- Earlier bubble test flake: it sampled before the first post-StartMatch layout update and sometimes saw the default (0,0) RectTransform. The test now waits one frame, confirms Fight state and measures a fresh visible interval; final PlayMode passes.

## Step 1 verified — authored whole-body strikes

- Replaced the relaxed-carry strike placeholders with 48 authored weapon poses for all eight characters. The straight trail that looked like a second stick and ambiguous hurt speed marks are removed. Weapons are baked into the same image as the complete body; only whole-body UV mirroring is used.
- Corrected generated-source review defects before import: Manong recovery was missing its can, and Tito wind-up had an extra hand. Selected sheets now show one weapon and two arms/hands per character.
- All actions now share an 896x512 canvas and a real bottom-center actor pivot. Normalized whole-body frames preserve the ground line while knockback moves the whole actor by the existing model step. Extended strike artwork begins at the unchanged 0.12-second hit event.
- Fresh verification: EditMode 11/11, PlayMode 9/9, build/65 rule assertions; 272 body-mask connectivity/bounds/pivot tests; 48 grip/tip checks; 108 native Unity screenshots cover six strike and four hurt frames for all eight in both facings and all 28 pair types. Evidence: Docs/animation_capture_evidence.json. Other reaction/locomotion poses remain effect variants pending further polish.

## Step 2 verified — head-owned bubbles

The old bubble code used fighter foot X plus a fixed offset, then pushed nearby bubbles into horizontal lanes. That made bubbles drift away from the heads. Each action frame now provides a head-top coordinate; the rendered anchor mirrors and moves with the complete sprite. Rounded bubbles center on that anchor, and close-range collisions raise one bubble by a safe vertical lane instead of moving it sideways. A small tail retains the head's X target. Tests cover owner centering, tail direction, vertical non-overlap and HUD/screen bounds; 11 EditMode, 9 PlayMode and all 65 rule assertions pass. Combat timing/range/hitboxes are unchanged.

## Step 3 verified — mirrored wooden Hilo HUD and pause layout

- P2's first Hilo segment used x=300 while its label started at x=306, covering the word. Segments now occupy x=14..294 with a separate label slot, and both meters are tested empty / 1..5 / reset.
- Added ring/bar wood connectors, bottom keycaps, medal sparkle and matching wood menu frames. No life/stamina/timer/K.O. widget exists.
- The top-left pause fighter was not an extra object: Fight created both actors at the helper's default origin, and an immediate pause returned before their first layout update. Actors now receive stage positions and facing when constructed; a pause-before-first-update regression checks their exact placement.
- Focus scaling pivoted buttons at their top-left and cards scaled twice (parent and child). Buttons now scale around center; card child feedback uses color/outline while the parent has one bounded scale. Selected card non-overlap is tested.
- Final step-3 checks: EditMode 11/11, PlayMode 9/9, 65 rule assertions, zero C# warnings/errors, five exact native resolution runs, no reported glyph/overflow/runtime issues. HUD and pause/controls/result/selection captures were reviewed.

### Selection reaction audio overlaps
Character cues now share a dedicated non-looping source, wait for menu audio to finish, and coalesce repeated highlights. Menu actions take priority and stop an active reaction blip. Combat impacts keep their existing separate one-shot playback and throttling. Added a PlayMode regression.

### Fixed-light maps and evidence finalizer
The previous night labels pointed to dusk art and Looban retained a parked vehicle. Replaced with reviewed full-night relights and an empty alley; archived unused dusk textures outside Assets. Morning lamp overlays are now disabled. The old evidence finalizer contained hardcoded obsolete test totals and rewrote checklists; replaced it with verification of real XML, current source/build hashes, native captures and audits. --local-only explicitly preserves the separately queued packaging task.

### Frozen last-hit sparkle on result
Native result review found the final impact star persisted because combat burst lifetimes stop ticking once the manhole ends the round. Rendering now hides combat bursts immediately when Finished; result confetti/medal sparkle still plays. Gameplay end timing is unchanged. Added a result-screen regression to InputAndCanvasTests.

### HUD grain transparency pinholes
Full-resolution review exposed bright hairlines in the wood bar. The procedural grain was drawn directly with low alpha, replacing the opaque wood pixels. Grain now draws onto a separate layer and alpha-composites onto the solid frame, retaining subtle color without holes. Ring aperture and rounded outside edges remain transparent. Rebuild/retest/native proof required after the texture correction.

### Optional single-file SFX rejected
Official public-domain LZMA SDK 26.05 7zSD.sfx wrapper was created from the exact tested build. Windows refused its normal launch with WinError 740 (elevation required). No elevation, manifest workaround, security setting or antivirus exclusion was attempted. B fails the no-setup requirement and is not recommended or uploaded. A ZIP passes and is the final Canvas deliverable.

### Desktop-injected navigation remains inconclusive
Normal double-click A launch, pointer selection and Alt+Enter worked. Sky-injected F/Enter/arrows did not advance action-map navigation. Root cause was not established from these injections; InputSystem simulated keyboard/isolation tests passed. Physical keyboard navigation must be checked by the owner; no physical-keyboard pass or guarantee is claimed. No gamepad was connected.

UPDATE2 step 1: Jeepney source faces left but positive-x travel made it reverse. Reversed the path without changing art; reused painted title plate for the opening and separated tagline. Build/65 checks/native smoke passed; viewed opening and title.
UPDATE2 step 2 root cause: StartPressed included every left mouse press, entering selection before intended button callbacks. Removed the any-key title fallback; pointer callbacks and isolated Confirm actions own routing. Added keyboard/gamepad and pointer regressions. Passed 11 PlayMode tests.
UPDATE2 step 3: binary-alpha corners plus point-filtered UI and long-bar skin stretched into tall panels caused jagged/cut edges. Replaced shared corners with padded signed-distance coverage and preserved nine-slice borders; bilinear UI import; tall panels use continuous shared rounded rim. All five native sizes passed.
UPDATE2 step 7: 118-unit logic reach was shorter than the illustrated chest-height weapons; common reach is now 190 for all eight, timing and one 52-unit step unchanged. Increased non-shoving body gap to 112; stationary updates no longer correct overlapping test snapshots. Match tally used to reset on StartMatch: BeginNextRound now preserves wins and selected arena, first to three finishes the match. Native simulated match to three passed.
UPDATE2 step 9: missing invalid-action feedback fixed with a universal raycast-blocking focus-trapped notice and consumed dismissal input. Selection Start stays clickable to explain missing confirmations. All 13 tests passed; first editor process crashed on shutdown after tests, clean rerun and build exited 0. Standalone full three-win smoke exited 0.

UPDATE2 step 12 regression check caught missing CanvasRenderer on the new masked cloud Graphic during PlayMode clipping. Added explicit required CanvasRenderer before the Graphic. Initial Step11 smoke completion/performance alone did not verify absence of logged exceptions; superseded by rerun and final log-scanned QA. Five-segment Hilo logic remains unchanged.

UPDATE2 final audit: Editor exit now directly calls the editor API under UNITY_EDITOR, avoiding reflection against an assembly name that may differ between Unity versions. Standalone retains Application.Quit. Opening evidence checks the actual loaded painted HeadingLogo resource and full title rather than obsolete removed intro credit text.

## UPDATE3 drain trigger
Cause: artwork spanned 220px per edge but the model used an unrelated 112px threshold. A feet center could stand in the visible water. Fix: shared 220px artwork geometry with 20px inner stone rim; inclusive water-edge triggers at 200/1080, both before and after input/hits. All maps share geometry. Exact 52px knockback and attack timing unchanged. New boundary/movement tests cover all maps, sides and players.

UPDATE3 head proportions: generated revised sheets had inconsistent crown-to-sole proportions (ready poses Tito 48.8%, Suka 55.8%, Nanay 56.5%). Repeatable whole-image continuous mesh normalization targets 43%, preserving original pixel sources, feet-center and baked weapons. Source/measurement conventions and actual per-character ranges: Docs/HEAD_RATIO.md. No separate head/body runtime objects. All 272 measured runtime images are hash-bound to the ratio test. All eight ready frames viewed side by side at the same pixel scale.

UPDATE3 focus: old shared feedback scaled buttons 1.04; confirmed buttons pulsed up to 1.032. Removed both geometry growth paths. Focus/hover/press now share warm fills, thin brass rim and a restrained 2px icon slide; action buttons retain full parent padding. New PlayMode test traverses all screens (including inactive pause/result/popups) and asserts 8px parent padding, shared20px icons and scale bounds.

UPDATE3 background motion root cause: Show enabled ambience only for Fight; title/stage initial paintings never got motion. Prior generic FX were tiny and did not follow painted objects. Fix: configure after the actual screen texture is chosen, with painting-specific color-masked transparent pixel cutouts from the five approved paintings. Separate upper foliage, cloth, lamp/window and sky layers use an independent unscaled clock behind all fighters/UI. No floor or vehicle pixels. Original paintings remain unchanged. Native stationary-pair proof and thresholds are in QA_SCREENSHOTS/UPDATE3/motion_check.txt.

Motion proof caught overly broad cloth/sky masks and stage-select mean difference 8.61/255 (limit6). Tightened cloth to flag-sized connected components and reduced sway, without relaxing thresholds. Looban has no painted cloth in the approved source, so three already-original festival pennants hang from its wires; provenance records their resource IDs. Original lighting/foliage/sky pixels remain the source for the other layers.

UPDATE3 HUD name plates: fixed370px names left empty space while medals floated below portrait over scenery. New plates use TMP preferred text width plus96px for three20px medals/padding, min240/max370, anchored to the portrait side. P1 medals immediately follow the name; P2 mirrors them before the name. Win pop/sparkle retained inside plate; old row removed. A PlayMode regression covers all8 names on both sides, parent containment and name separation. Five Hilo segments unchanged. Motion-proof harness now waits for the screen fade before freezing UI, so evidence shows fully opaque stationary actors.

UPDATE3 final regression first attempt: all16 PlayMode tests passed in XML, but Unity exited3221225477 after teardown/cleanup (same intermittent native editor shutdown failure recorded in UPDATE2). Retaining original log in Logs/UPDATE3-PlayMode-native-teardown-crash.log; rerunning graphics PlayMode and build. Do not count the failed process as a clean run.

Second final PlayMode attempt also passed16/16 XML but exited3221225477 after shutdown. Windows Application event1000 identifies Unity.exe/ntdll.dll access violation; graphics log used Direct3D12 on AMD Radeon graphics. Running the graphics-enabled editor harness with explicit Direct3D11 to check backend-specific teardown stability; no test/rule relaxed and standalone defaults unchanged. This is a diagnostic inference, not proof that D3D12 caused the native fault.

## Incoming Hilo display correction
Cause: HUD indexed Model.players[p].streak, the hits LANDED by its owner. Display now reads Model.players[1-p].streak: consecutive hits TAKEN by the HUD owner. DuelModel is byte-for-byte unchanged: shared timing/range/52px step, fifth-hit victim dizzy and existing streak breaks/recovery all retained. Old symmetric tests set both streaks equal, hiding the side inversion; replaced with asymmetric assertions and real-hit integration on both sides.

## UPDATE4 step1 confirm/start flow
Root cause: Confirm directly openedStageSelect on second readiness; generic stageConfirm also mixed choose/start in one frame. Removed automatic transition, added an explicit guarded Simulan action, ready-only pulse/hint and disabled-click notice, consumed confirmation frame/held release, and isolated ready cancellation. Fixed sharedESC previously unreadying P1 then returning toTitle through unreadyP2. Stage choices consume their frame; only a separate start activates the match. Keyboard/two-gamepad and actual mouse-callback regressions passed;20 native screenshots exercise the new route and a full3-win match.

## UPDATE4 step2 settings
Removed reaction-style row and all row icons. Pormal is read-only in ReactionBank and Choose uses only its formal category pools; old Ilaban.ReactionStyle is deleted at startup and by compatibility setter. The unused legacy data schema remains for existing resource validation, with no UI/saved route to it. Music/effects already had separate source volumes; now distinct native Master/Musika and Master/Tunog mixer groups also route music versus hits/menu/actions/opening. Sliders retain isolated source controls and master uses AudioListener. Corrected horizontal settings navigation changing the focused row before adjusting its slider. Four42px rows48px apart replace five34px rows; labels distinguish background music/effects. During authoring, a comment swallowed a closing brace; fixed before verification, no failing build is counted.
