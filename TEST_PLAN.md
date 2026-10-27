# ILABAN ANG TAWA - manual acceptance test

Status: planned. Baseline tests of the academy prototype do not establish compliance with the new rubric. Write PASS or FAIL and evidence after executing each test. Hardware checks require the student's controllers and a second Windows PC.

| Step | Test and expected result | Pass | Fail | Evidence |
| --- | --- | --- | --- | --- |
| 1 | Open the project in Unity 6000.6.2f1, wait for import, open Assets/Scenes/Main.unity, and check Console errors. | | | Not run |
| 2 | Press Play, start the game, choose both characters independently and confirm both. Check previews, items, menu cues and stage transition. | | | Not run |
| 3 | Hold P1 A/D/F while P2 is idle, then reverse using arrows/Right Ctrl/Numpad 0. Only the intended player responds. | | | Not run |
| 4 | Hold movement and punch on both players simultaneously. Check input response and keyboard rollover. | | | Not run |
| 5 | Attempt W, Up, Space and every gamepad button while idle, walking, punching, hit, dizzy, falling and paused. No jump, kick, crouch, special, guard or extra attack may occur. | | | Not run |
| 6 | Land a single punch from each direction with movement released. Record position before/after; repeat ten times per character. Every hit has the same step distance. | | | Not run |
| 7 | Land five consecutive hits with the same attacker. The fifth causes stars/spiral and wobble, then recovers after the configured window. | | | Not run |
| 8 | After four hits, let the attacker be struck; repeat after a missed punch and after the streak timeout. The next hit must not trigger a five-hit dizzy. | | | Not run |
| 9 | Push each player into each edge imburnal on all three maps. Round ends at fall detection, fall remains visible and results appear promptly with correct names. | | | Not run |
| 10 | Inspect all screens. No health/poise bars or K.O. behavior; required title exact; other visible text is Tagalog. | | | Not run |
| 11 | Connect two controllers. Move/punch independently, navigate menus and confirm. Test an available generic USB controller. | | | Not run |
| 12 | Switch between keyboard and controller mid-match, unplug one controller, continue playing, reconnect it. No crash, cross-player control or forced pause. | | | Not run |
| 13 | Play all maps. Listen for three distinct tracks plus separate title/selection music. Verify all menu, item, dizzy, fall/splash and victory cues against credits. | | | Not run |
| 14 | Check 1920x1080, 1366x768, 1280x720, 2560x1440 and 1920x1200. Confirm no stretching, bars, clipped controls or hidden manholes. | | | Not run |
| 15 | Pause, resume, replay and return to the title using keyboard and controllers. Check focus loss and new rounds for stuck input/state. | | | Not run |
| 16 | Extract the final portable ZIP into a different folder. Launch the named EXE with its Data folder and UnityPlayer.dll present. Complete one match. | | | Not run |
| 17 | Copy that entire folder to a clean Windows PC without Unity and repeat. Check player logs for errors and missing resources. | | | Not run |

## Bugs identified during initial inspection
- Required Unity 6000.3.27f1 is absent locally; upgrade/import verification in progress.
- Downloaded project has no .git directory; existing Git history reconnection in progress.
- Gameplay, art, audio and English UI are still the academy prototype, including disallowed moves.
- Existing controller disconnect logic forces pause, contrary to the new hot-swap requirement.
- Existing render pipeline draws a fixed 1280x720 GUI; aspect-ratio handling needs review.
- Actual map reference folder is MAPS REFERCES (spelling preserved); character references are present.

## Setup verification (28 October 2026)

- PASS: original baseline imported and compiled in Unity 6000.6.2f1; ProjectSetup.ValidateBatch passed.
- PASS: SubmissionSetup.ValidateBaseline passed in the newly named project folder.
- PASS: InputChecks.ValidateBatch passed 36 virtual-device assertions. These validate existing bindings, not the new restricted control scheme.
- PASS: Main.unity opened interactively; entering Play reached the existing character selection with sprites present.
- Unity upgrade retained scene and asset GUIDs and added editor serialization fields. Compiler deprecation warnings remain for FindFirstObjectByType; no compile errors occurred.
- Physical controllers, final art/audio, final rubric gameplay, standalone builds and clean-PC tests remain pending.
- Original folder is retained because Windows locks it; all further edits belong in the fully named sibling folder.

## Current continuation acceptance evidence

Prior baseline notes above are historical; use this section and the final checklist for the current game.

| Check | Pass | Fail | Evidence / qualification |
| --- | --- | --- | --- |
| New adult character paths and all 9 map/time resources | X | | Expanded EditMode 9/9 XML in Logs/Verified-EditMode.xml |
| Horizontal-only, equal punch, five-hit dizzy, recovery, streak resets, manhole-only round result | X | | 65 assertions; 50-hit Unity case |
| Reaction state cannot change combat timing/displacement | X | | RubricTests deterministic comparison |
| Latest built actual simulated gamepad match | X | | Preliminary Final_1280x720/PASS.txt; genuine movement and punches to fall, not injected final position |
| Corrected UI labels/HUD contrast and rig articulation | | | Source fixes after preliminary screenshots; final re-capture pending |
| Latest PlayMode rerun lifecycle | | | Prior 4/4; new runner stalls independently of tests, under investigation |
| Four resolution captures | | | Tools/run_portable_qa.py writes dimensions and error checks to Docs/portable_qa_results.json |
| Active English keyword scan | X | | Zero heuristic candidates; literal UI text dump also captured per screen |
| Audio peak audit | X | | 30 active clips, zero samples at clipping threshold; listening remains manual |
| Portable ZIP extracted and launched | | | Pending final package |

### Manual checks after the final ZIP is ready

For each row mark one box, then note device details or any failure:

- Real USB gamepad plus a second controller: [ ] PASS [ ] FAIL.
- Keyboard-to-gamepad switch, then unplug/replug mid-match: [ ] PASS [ ] FAIL.
- Two people on one keyboard (hold movement and punch together): [ ] PASS [ ] FAIL.
- Entire extracted folder on a PC without Unity: [ ] PASS [ ] FAIL.
- Speakers and headphones, all volume settings and item sounds: [ ] PASS [ ] FAIL.
- Actual monitor fit, text readability and 60-second idle title: [ ] PASS [ ] FAIL.
- Antivirus/SmartScreen handling of the unsigned executable: [ ] PASS [ ] FAIL.

Do not treat an extracted-folder launch on the development PC as a clean-PC result.
