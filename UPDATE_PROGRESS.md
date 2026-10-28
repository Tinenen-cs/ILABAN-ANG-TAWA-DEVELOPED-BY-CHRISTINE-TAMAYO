# Final delivery progress

Finalization prompt authorized main/release/Drive. Pre-check passed; main fast-forward and rebuild complete.

1. [x] Opening-only animated jeepney, skippable opening, no moving vehicle in fights. Evidence: 00-opening.png and three 04-match captures in each QA folder.
2. [x] Original painted logo and four headings with live localized TMP underneath. Exact full developer title preserved. Assets/Resources/Ilaban/UI/Heading*.png.
3. [x] Tagalog and Pormal defaults; language and reaction controls work. Native default/English captures and RubricTests.
4. [x] Three keycap control cards plus five settings rows, keyboard/gamepad navigation and bounded sliders. SettingsAndExitUseTheirOwnScreenFlow; 08-controls.png.
5. [x] Rounded wood/tin title buttons, inner highlight and original code-drawn icons. 01-title.png.
6. [~] Shared hover/focus/press colors, outline, centered scale, moving icons and soft sound implemented. NEEDS MANUAL TEST for subjective pointer/controller feel.
7. [x] Mga Kontrol stays in controls, with consumed per-screen input. SettingsAndExitUseTheirOwnScreenFlow and native routing capture.
8. [~] Umalis opens confirmation, defaults to Hindi and returns focus correctly. Automated routing passed. NEEDS MANUAL TEST for actual quit and physical gamepad feel.
9. [~] Eight non-overlapping cards, names, item icons, two-player ready checks, round back button, intro/hover/ready/wait poses and sparkle. Automated layout/readiness gates passed; NEEDS MANUAL TEST for every human two-player selection combination.
10. [~] Eight complete whole-body action sets: 272 textures, 48 six-phase strikes and 48 authored walk/reaction source poses; idle uses ready stance and fall sinks the lose pose as one body. No item child/parts rig; common 896x512 canvas, feet pivot and ground line. 272 structural checks, 48 grip/tip checks in both facings and 108 native captures passed in final confirmation. Signature cues wait behind menu sounds. NEEDS MANUAL TEST for animation feel and facial/art consistency.
11. [x] Three large stage cards with separate names/description/hint zones; selection scale preserves gaps. Native screenshots at five sizes.
12. [~] Shared modern themed button states across all screens. NEEDS MANUAL TEST for exhaustive device/pointer feel.
13. [x] Exactly three fixed-light maps: Palengke Umaga; Looban Gabi; Plaza Gabi. Inactive variants outside Assets. No time selector; Looban vehicle removed. RubricTests and ProjectSetup checks.
14. [x] Original wood/brass portrait rings joined to five amber/red Hilo segments; empty start, fill/reset with streak, win medals/pop, name plates, center VS and bottom keycaps. No health/stamina/timer/K.O. HUD. Open street kanal at each edge. FightHudHasFiveHiloPipsAndNoSeparateItemSprites; QA_SCREENSHOTS/HUD.
15. [x] Only subtle night-lamp glow, isolated flags and leaves animate over fixed fight paintings. Morning glow off. No moving vehicle, cloud or floor layer. AcademyGame.Ambience/UpdateAmbience.
16. [x] Pause/controls use centered wooden panels and no orphan fighters. Immediate-pause placement regression passed. PauseAndSettingsOverlaysHaveNoOrphanFighterObjects.
17. [x] Small rounded head-owned bubbles, tails, per-frame head anchors, facing/motion/fall tracking, HUD clamp and vertical non-overlap. ReactionBubblesTrackHeadAndStayInSeparateHudSafeLanes. Dizzy stars are larger and always drawn above bubbles.
18. [x] Nonfunctional attack orbs removed; non-interactive bottom keycap hint only. Native match screenshots.
19. [x] Centered result panel above whole-body winner, painted Panalo sign, options/confetti/jingle and winner medal sparkle. Final-hit effect hides on finish; regression passed. 06-result.png.
20. [~] Fresh build configured borderless native desktop resolution; Alt+Enter toggle implemented; five exact-size native window tests passed in final confirmation. NEEDS MANUAL TEST for actual presentation monitor/fullscreen and 16:10 hardware.
21. [~] Rubric behavior has real automated evidence; no Exceeds/grade guarantee. Physical feel and human theme/Tagalog review remain NEEDS MANUAL TEST.
22. [x] Checklist covers 1-25; UPDATE_PROGRESS.md and handoff keep evidence and next commands.
23. [~] Latest Unity EditMode 11/11, graphics-enabled PlayMode 10/10, 65 assertions and real-folder build passed. Final five-size/native animation confirmation passed; 75 UI and 108 animation captures verified. Final ZIP extraction and full match passed; hardware/clean-PC checks NEEDS MANUAL TEST.
24. [~] Tested source fast-forwarded to main under the finalization prompt. PUBLIC repository; no reference-photo objects in outgoing commits. No force-push/history rewrite. Final push/release verification in progress; old public history still exists.
25. [~] Current safe ZIP extraction/full-match and Defender checks passed. B single-file EXE rejected (elevation required), excluded from uploads. README/FILE_INFO and final screenshots prepared; verified Drive/release upload in progress.

Drive: desktop absent, connector 100 MB limit rejects 200 MB ZIP; official rclone installed and awaiting owner Google authorization. No credentials stored in repo. Watchdog remains stopped/Disabled.
