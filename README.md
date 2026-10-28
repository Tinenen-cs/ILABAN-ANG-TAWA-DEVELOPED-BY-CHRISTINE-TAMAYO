# ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO

A two-player Filipino street brawler made by Christine Tamayo. Pick a character, choose one of three barangay arenas, and push your rival into the open kanal. The game is Tagalog-first and runs as a portable Windows app.

## Preview

**Pamagat**

![Pamagat ng laro](Docs/ReleasePreview/title.png)

**Laban sa Kalye ng Palengke**

![Laban sa palengke](Docs/ReleasePreview/match.png)

## Hilo display fix - v1.2.1

Each Hilo Meter now shows consecutive hits taken by its owner: the opponent current streak. Only the victim bar fills; the fifth hit makes that same character dizzy. Streak breaks and recovery clear the display. Exactly five segments and Tagalog Hilo / English Dizzy remain. Combat rules, range, timing and one-step knockback are unchanged.

Verified: EditMode15/15, graphics PlayMode17/17,65 rule assertions,48 native fill/reset/dizzy captures across both sides/all3 maps, two EXE-only full3-win matches from emptyTemp/space-path folders, and clean Defender scans. See RUBRIC_CHECKLIST.md and Docs/QA_EVIDENCE/hilo_final_summary.json. Older broad UPDATE3 resolution/animation reports are historical; this focused change does not alter HUD geometry.

Canvas EXE: **213,749,248 bytes**, SHA-256 `721e4593dffc456f0cfab5c2819c1c99648cc042b457889442f2b1e4dfe8aec3`. [Hilo fix release](https://github.com/Tinenen-cs/ILABAN-ANG-TAWA-DEVELOPED-BY-CHRISTINE-TAMAYO/releases/tag/v1.2.1-hilo-victim) publication is pending final server verification. The original GUI launcher embeds all dependencies and uses Windows built-in.NET Framework4.x; keep the complete folder/ZIP backup.

Repository PUBLIC. Drive binaries remain older and are outside this task; use the current GitHub release after verification. Physical keyboard/controller/USB, human audio/art and clean-PC checks remain manual.

## Controls

- **Manlalaro 1:** A / D to move; F to punch and confirm.
- **Manlalaro 2:** Left / Right arrows to move; Right Ctrl or Numpad 0 to punch and confirm.
- **Gamepad:** left stick or D-pad to move; A / X to punch and confirm; B to go back. The first connected controller controls P1 and the second controls P2.
- **ESC:** pause or return from a menu. **H:** open controls during a match. **Alt+Enter:** toggle fullscreen and windowed mode.

## Game rules

Movement is left and right only. Every player has the same straight punch. A landed hit moves the target back exactly one step. Five consecutive hits make the target dizzy for two seconds; a miss, a three-second gap or being hit resets the streak. The round ends immediately when a player falls into the kanal. First to three round wins takes the match; earlier rounds advance automatically. All eight use 190-unit reach, .12-second startup, .36-second cycle and one 52-unit step per hit. The five-segment Hilo meter tracks landed-hit streak, never health. There are no health bars, knockout system, jump, kick, extra move or match timer.

## Submit in Canvas

Upload the verified outer single EXE in SUBMISSION. Exact hash/size and current delivery links are in SUBMISSION/FILE_INFO.txt. The earlier elevation-dependent SFX was replaced and remains archived; do not submit historical copies.

## Requirements and credits

- Windows 10 or Windows 11, 64-bit.
- Two keyboard players share one computer, or connect compatible gamepads.
- See [CREDITS.md](CREDITS.md) for character, stage, font and audio credits; see [AUDIO_LIST.md](AUDIO_LIST.md) for cue details.

## Project verification

The source is developed with Unity 6000.6.2f1. Automated test, build, screenshot and package evidence is recorded under `Docs/QA_EVIDENCE/`, `Docs/portable_qa_results.json` and `Docs/QA_EVIDENCE/final_summary.json`. `Docs/package_verification.json` verifies the current ZIP, fresh extraction and complete simulated match. Optional SFX rejection and desktop/Defender results are recorded under Docs. Physical keyboard/controller/clean-PC/audio tests remain manual. See [CHECKLIST.md](CHECKLIST.md) for verified and manual items.
