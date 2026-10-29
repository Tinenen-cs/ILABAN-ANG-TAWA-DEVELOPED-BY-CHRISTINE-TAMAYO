# ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO

A two-player Filipino street brawler made by Christine Tamayo. Pick a character, choose one of three barangay arenas, and push your rival into the open kanal. The game is Tagalog-first and runs as a portable Windows app.

## Preview

**Pamagat**

![Pamagat ng laro](Docs/ReleasePreview/title.png)

**Laban sa Kalye ng Palengke**

![Laban sa palengke](Docs/ReleasePreview/match.png)

## UPDATE4 - confirm first, then Simulan

Both players confirm their fighter and stay on selection with Handa na. Once both are ready, a separate Simulan action opens stage selection: Enter, controller Start, mouse, or a fresh confirm after releasing the ready-making press. Esc/controller B cancels readiness. Stage cards only choose; Simulan starts the match.

Settings now contain language, master, Music (background) and Sound effects. Tagalog labels explain each sound category. Music and effects use separate native mixer groups; Pormal reactions are locked and old saved Kabataan preferences are deleted. Alt+Enter changes display mode without activating menu buttons. Victim-side five-segment Hilo remains unchanged.

Current release verification is recorded in RUBRIC_CHECKLIST.md and Docs/QA_EVIDENCE/update4_final_summary.json. Older release reports below are historical. No Google Drive is used for UPDATE4. Physical input, listening and clean-PC checks remain manual.

Canvas file: **213,752,320 bytes**, SHA-256 `b45faf27a87160fe62ce4612532fc6fb5078d0e7f7ded0eeb0ed5b3b919e849f`. Single EXE verified alone in two empty folders,44 captures/two complete matches; Defender clean. EditMode15/15, PlayMode23/23,65 rule checks,80 current four-resolution screens and48 fresh Hilo states. Release publication next: [UPDATE4](https://github.com/Tinenen-cs/ILABAN-ANG-TAWA-DEVELOPED-BY-CHRISTINE-TAMAYO/releases/tag/v1.3-update4).

## Controls

- **Manlalaro 1:** A / D to move; F to punch and confirm.
- **Manlalaro 2:** Left / Right arrows to move; Right Ctrl or Numpad 0 to punch and confirm.
- **Gamepad:** left stick or D-pad to move; A / X to punch and confirm; B to go back. The first connected controller controls P1 and the second controls P2.
- **ESC:** pause or return from a menu. **H:** open controls during a match. **Alt+Enter:** toggle fullscreen and windowed mode.

## Game rules

Movement is left and right only. Every player has the same straight punch. A landed hit moves the target back exactly one step. Five consecutive hits make the target dizzy for two seconds; a miss, a three-second gap or being hit resets the streak. The round ends immediately when a player falls into the kanal. First to three round wins takes the match; earlier rounds advance automatically. All eight use 190-unit reach, .12-second startup, .36-second cycle and one 52-unit step per hit. The five-segment Hilo meter tracks consecutive hits taken (the opponent streak), never health. There are no health bars, knockout system, jump, kick, extra move or match timer.

## Submit in Canvas

Upload the verified outer single EXE in SUBMISSION. Exact hash/size and current delivery links are in SUBMISSION/FILE_INFO.txt. The earlier elevation-dependent SFX was replaced and remains archived; do not submit historical copies.

## Requirements and credits

- Windows 10 or Windows 11, 64-bit.
- Two keyboard players share one computer, or connect compatible gamepads.
- See [CREDITS.md](CREDITS.md) for character, stage, font and audio credits; see [AUDIO_LIST.md](AUDIO_LIST.md) for cue details.

## Project verification

The source is developed with Unity 6000.6.2f1. Automated test, build, screenshot and package evidence is recorded under `Docs/QA_EVIDENCE/`, `Docs/portable_qa_results.json` and `Docs/QA_EVIDENCE/final_summary.json`. `Docs/package_verification.json` verifies the current ZIP, fresh extraction and complete simulated match. Optional SFX rejection and desktop/Defender results are recorded under Docs. Physical keyboard/controller/clean-PC/audio tests remain manual. See [CHECKLIST.md](CHECKLIST.md) for verified and manual items.
