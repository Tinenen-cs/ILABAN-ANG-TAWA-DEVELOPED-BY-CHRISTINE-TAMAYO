# ILABAN ANG TAWA — release checklist

Status: [x] verified in source/tests/captures; [~] implemented partially or NEEDS MANUAL TEST. No grade level is guaranteed.

1. [x] Opening jeepney animation is restricted to the opening; no jeepney actor is used in the fight. Evidence: `QA_SCREENSHOTS/Final_1280x720/00-opening.png`, `04-match-0.png`.
2. [~] Themed headings use localized text and themed plates; fully painted lettering assets are incomplete. Evidence: title and controls captures.
3. [x] Tagalog and Pormal are defaults; English and reaction selection work. Evidence: controls and English settings captures.
4. [x] Language, reaction and volume controls fit their rows; slider fill stays in bounds. Evidence: `InputAndCanvasTests.SettingsAndExitUseTheirOwnScreenFlow`.
5. [x] Themed title buttons and spacing fit at all five QA sizes.
6. [~] Shared focus, hover and press feedback exists; every button and device state still needs manual review.
7. [x] Mga Kontrol opens the controls/settings view. Evidence: PlayMode routing test.
8. [~] Umalis dialog defaults to Hindi. Gamepad feel and actual application exit need a physical review.
9. [~] Eight character cards, portraits, readiness and descriptions work. Fine detail and every two-player readiness combination need manual review.
10. [~] All eight characters now have authored whole-body weapon strikes (`Strike_00.png` through `Strike_05.png`: ready, wind-up, swing, forward strike, impact, recovery). Each weapon stays in the hand and extends toward the opponent; there is no independent weapon overlay. All 272 action textures share an 896x512 canvas, actual feet-center transform pivot and ground line. Every body mask passed connectivity, bounds, ground and both-facing checks. 108 native Unity frame/pair captures are under `QA_SCREENSHOTS/ANIMATION/Unity/`. Walk/hurt/dizzy/fall/win/lose still use whole-body effect variants; dedicated facial-expression swaps remain partial. Evidence: `Docs/character_action_validation.json`, `Docs/animation_capture_evidence.json`, `Docs/strike_generation.md`.
11. [x] Three stage cards show distinct location art, names and hints at all five sizes.
12. [~] Shared themed buttons work across views; exhaustive manual state review remains.
13. [x] Exactly three maps: Palengke (Umaga), Looban (Takipsilim), Plaza (Takipsilim); no time selector.
14. [x] Joined original wood/brass portrait rings and mirrored five-segment Hilo bars, wood name plates, center VS, win medals with pop/sparkle, and a rounded bottom keycap plate are used. Both labels remain uncovered. No health/stamina/timer/K.O. objects. Hilo starts empty, fills with consecutive hits and resets with the streak. Pause, controls and results share the wood frame. Evidence: five native screenshots in `QA_SCREENSHOTS/HUD/`, both-player Hilo/label PlayMode checks.
15. [~] Lamp glow animates. Banderitas and foliage remain part of fixed artwork.
16. [x] Pause/controls transitions have no orphan fighters; immediate pause before the first fight update preserves correct stage placement. Root cause and regression are recorded in `BUG_LOG.md` and `PauseAndSettingsOverlaysHaveNoOrphanFighterObjects`.
17. [x] Rounded 160x50 reaction bubbles track each per-frame head anchor through facing/motion, with a small tail pointing to that head. Close bubbles separate vertically while their horizontal centers stay on their owners. Screen/HUD bounds and non-overlap are tested. A falling actor's bubble follows it briefly before hiding with the actor. Evidence: `ReactionBubblesTrackHeadAndStayInSeparateHudSafeLanes`, `QA_SCREENSHOTS/Step2_Bubbles/05-dizzy.png`.
18. [x] No interactive Suntok orb remains. Fight hint shows the punch control.
19. [x] Result panel, winner pose, confetti and win sound work. Evidence: `06-result.png` at each resolution.
20. [~] Windows build starts windowed for QA and supports Alt+Enter. Exact screenshot layouts passed all five sizes; full-screen monitor behavior still needs a manual device check.
21. [~] Rubric evidence is documented; no Exceeds-level claim is made.
22. [x] This checklist and `UPDATE_PROGRESS.md` cover all 25 release items.
23. [~] Current source passed 65 rule assertions, EditMode 11/11, PlayMode 9/9 and five native resolution runs (1280x720, 1366x768, 1920x1080, 2560x1440, 3440x1440). No reported exceptions, glyph/overflow issues, default English candidates or clipped audio samples. Action-frame audit and 108 native animation captures passed. Latest ZIP rebuild/extraction remains in the separately queued EXE/Drive task.
24. [~] Local work is on `phase3-polish`; no push, merge, force-push or history rewrite. The PUBLIC repository's prior history contains reference photos. Owner decision about history/privacy is pending.
25. [~] README and local evidence are prepared. The ZIP and public release link await the separate final EXE/Drive task and the public-history decision.

## Rubric self-check

| Criterion | Evidence | Remaining limitation |
| --- | --- | --- |
| Filipino theme and interface | Tagalog-first interface, original scenery/characters, three arenas, themed wood/brass HUD, five-size screenshots | Strike pose polish, character expressions, foliage motion and painted heading art are partial. |
| Gameplay rules | `Tools/run_final_checks.py`, 65 assertions, model checks and PlayMode tests | Physical feel/hardware checks remain. |
| Player input | Separate keyboard bindings and simulated gamepad ownership/hot-swap | Physical two-controller hot-swap and keyboard rollover need manual checks. |

## NEEDS MANUAL TEST

- Human feel/presentation review of the authored weapon strikes; technical grip, grounding, bounds, connectivity and facing checks pass.
- Facial expression swaps and signature per-character reaction sounds.
- Physical controllers, disconnect/reconnect feel, and keyboard rollover.
- Full-screen monitor fit and speaker/headphone balance.
- Clean Windows PC without Unity; antivirus/SmartScreen behavior for unsigned executable.
- Owner decision on public reference-photo history before any publishing.
- Latest SUBMISSION ZIP rebuild, extraction and simulated match are part of the queued final executable task, not this step.
- Actual watchdog reboot/usage-limit recovery with Codex App/IDE hosts closed. Lock, process-guard, reset parsing, harmless CLI and scheduled registration tests passed; current editing relaunch count is zero.

## Resume safeguards

- [x] Tested `Tools/autoresume.ps1`, hidden background start and current-user `IlabanAutoResume` scheduled task (logon plus five minutes). Evidence: `Docs/QA_EVIDENCE/autoresume_tests.json`, `autoresume_setup.json`, local `Tools/autoresume.log`.
- [x] One exclusive watchdog lock, conservative Codex-process guard, persisted three-failure halt, connectivity backoff and reset-time wait. No account/key changes or limit bypass.
- [x] AC sleep/display prior values recorded (both already zero), same values reapplied. No DC power changes.
