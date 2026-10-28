# ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO

A two-player Filipino street brawler made by Christine Tamayo. Pick a character, choose one of three barangay arenas, and push your rival into the open kanal. The game is Tagalog-first and runs as a portable Windows app.

## Preview

**Pamagat**

![Pamagat ng laro](Docs/ReleasePreview/title.png)

**Laban sa Kalye ng Palengke**

![Laban sa palengke](Docs/ReleasePreview/match.png)

## UPDATE3 release status

The visible drain now triggers immediately on both sides of every map. Original painted festival pieces, measured43% head proportions, uniform button icons, bounded focus feedback, painted background motion and mirrored name-plate win medals are implemented. Exactly five Hilo segments remain for the five-hit dizzy rule; no health/timer/extra attacks.

Verification/evidence: Docs/HEAD_RATIO.md, Docs/background_motion_check.json, Docs/QA_EVIDENCE and QA_SCREENSHOTS/UPDATE3. Windows standalone candidate is built from this real project. Verified Canvas EXE: **213,747,712 bytes**, SHA-256 `4ec4b62c193724a5d2948681e1cf3a39924a27deb950f18bfe7b9c11bcc1b362`. Both EXE-only full-match tests and Defender scans passed. Download the current game from the [verified UPDATE3 Release](https://github.com/Tinenen-cs/ILABAN-ANG-TAWA-DEVELOPED-BY-CHRISTINE-TAMAYO/releases/tag/v1.2-update3).

**Drive EXE/ZIP are still older UPDATE2 files.** Large replacements are pending because the minimized owner browser cannot be restored through the automation input guard. Four Drive support files are updated. Use the current GitHub Release for the game. Physical keyboard/controller/USB, human audio/art, clean-PC and strict steady60fps checks remain manual; see HANDOFF.md. Final graphics tests use Direct3D11 after a recorded Direct3D12 editor teardown access violation; standalone tests exited0.

[PUBLIC source repository](https://github.com/Tinenen-cs/ILABAN-ANG-TAWA-DEVELOPED-BY-CHRISTINE-TAMAYO). [Drive folder](https://drive.google.com/drive/folders/1g1_TxVdJFwQab1QGKS9eUD65fRjbtfbH).

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
