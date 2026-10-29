# UPDATE4 rubric verification - 2026-10-29

Current evidence supersedes historical release reports. Windows x64, Unity6000.6.2f1, real project folder. No Google Drive used.

| Requirement | Status | Evidence |
|---|---|---|
| Two players; isolated P1/P2 keyboard and gamepad input | MET | InputAndCanvasTests and Update4Tests, PlayMode XML |
| Left/right only; no jump in any state | MET | RubricTests and65 rule assertions; unchanged DuelModel blob |
| One straight punch; shared range/timing; no extra attack | MET | RubricTests,65 assertions; unchanged DuelModel |
| Exactly one52-unit step back per hit | MET | RubricTests/65 assertions; unchanged DuelModel |
| Exactly5 consecutive hits cause victim dizzy; breaks reset | MET | Real-hit PlayMode integration;48 fresh both-side/all-map captures |
| Victim-side Hilo, exactly5segments, fill1-5/recovery/reset | MET | QA_SCREENSHOTS/HILO_VICTIM; Docs/QA_EVIDENCE/hilo_victim_capture.json; screenshots viewed |
| Round ends only on drain fall; immediate result | MET | RubricTests/65; full3-drain-win simulated match at4sizes and isolated EXE tests |
| No health, stamina, timer or K.O. UI | MET | RubricTests/InputAndCanvasTests; current screenshots viewed |
| Confirm only readies; separate Simulan; cancel; no press bleed | MET | Keyboard/two-gamepad/mouse Update4Tests; one/both-ready/cancel screenshots |
| Stage choice stays on screen; separate Simulan starts | MET | Keyboard/gamepad/mouse regressions; all3map captures |
| Plain Tagalog default; fixed Pormal; legacy setting removed | MET | Migration/reaction-pool tests; fresh Tagalog UI English scan clean |
| Four clean settings rows; no gear row icons | MET | Update4Tests; Tagalog/English4resolution screenshots viewed |
| Master/music/effects independent; native mixer groups | MET | VolumeSlidersAffectOnlyTheirOwnMixerCategory; Master/Musika/Tunog asset |
| Batch compile/tests/65 assertions | MET | EditMode15/15, graphics PlayMode23/23,65 checks; build exit0, zero errors/warnings |
| Every screen at1280x720/1366x768/1920x1080/2560x1440 | MET |80 fresh native captures; Docs/update4_final_qa.json; all contact sheets viewed |
| English/glyph/audio audit | MET | FINAL_STATIC_AUDIT.json:0 English candidates,0 missing refs/glyphs/clip samples;38clips |
| Fullscreen/Alt+Enter, no console; EXE alone in2emptyfolders | MET |44 native1920captures, two full3-win matches, clean logs/exit0; packaged screenshots viewed; Docs/update4_single_exe_tests.json |
| Current Defender/hash | MET | Windows Defender exit0/found no threats; SHA/size in SUBMISSION/FILE_INFO.txt and defender_update4.json |
| GitHub current release readback | NOT MET | Publishing next; do not treat old release as current |
| Actual keyboard rollover, physical gamepads/USB/hot-swap | NEEDS MANUAL TEST | Automated virtual-device tests cannot verify physical devices |
| Sound quality, Tagalog phrasing and art consistency | NEEDS MANUAL TEST | Human listening/review required; automated routing/layout passed |
| Clean school PC/.NET/unsigned SmartScreen; steady60fps | NEEDS MANUAL TEST | Same-machine isolated tests are not clean-PC/hardware proof |

Combat model unchanged from9ac4fc7. Previous update implementation tasks1-10 were checked complete before UPDATE4. No reference-photo files tracked or used. Original faces and full-body assets unchanged in this UI/settings update.

Manual test: launch the outer EXE on the presentationPC; ready both players and verify the screen stays; use Simulan, choose a map then Simulan; test Alt+Enter, isolated controls,5hits on the victim meter and a drain fall; listen to music/effects sliders separately. Physicalcontroller/USB tests remain manual.

Final Canvas EXE: C:\Users\chris\Documents\CSEL GAMEDEV FILES\ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO\SUBMISSION\ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe

Size: 213752320 bytes. SHA-256: `b45faf27a87160fe62ce4612532fc6fb5078d0e7f7ded0eeb0ed5b3b919e849f`.

UPDATE4 report: Docs/QA_EVIDENCE/update4_final_summary.json.
