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
14. [x] Original carved wood/brass portrait rings, wood name plates, center VS badge, winner medals, bottom hint plate and five amber-to-red Hilo pips are used. There is no health, stamina, timer or K.O. object. Hilo fills on consecutive hits and resets when a streak breaks. Evidence: `QA_SCREENSHOTS/HUD/`, `FightHudHasFiveHiloPipsAndNoSeparateItemSprites`.
15. [~] Lamp glow animates. Banderitas and foliage remain part of fixed artwork.
16. [x] Pause/controls transitions have no orphan fighters. Evidence: PlayMode test.
17. [x] Smaller reaction bubbles follow each head anchor, stay clamped, and separate in close quarters; they hide when the result appears. Evidence: PlayMode test and `05-dizzy.png`.
18. [x] No interactive Suntok orb remains. Fight hint shows the punch control.
19. [x] Result panel, winner pose, confetti and win sound work. Evidence: `06-result.png` at each resolution.
20. [~] Windows build starts windowed for QA and supports Alt+Enter. Exact screenshot layouts passed all five sizes; full-screen monitor behavior still needs a manual device check.
21. [~] Rubric evidence is documented; no Exceeds-level claim is made.
22. [x] This checklist and `UPDATE_PROGRESS.md` cover all 25 release items.
23. [~] Current local source build passed 65 rule assertions, EditMode 11/11, PlayMode 9/9, and portable QA at 1280x720, 1366x768, 1920x1080, 2560x1440 and 3440x1440. Screenshot/text audit and action-frame structural audit passed. The distributable ZIP has not been rebuilt for this latest HUD/action change; queued executable/Drive work stays separate.
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
