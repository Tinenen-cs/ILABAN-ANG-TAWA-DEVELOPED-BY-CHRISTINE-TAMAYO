# ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO

A two-player Filipino street brawler made by Christine Tamayo. Pick a character, choose one of three barangay arenas, and push your rival into the open kanal. The game is Tagalog-first and runs as a portable Windows app.

## Preview

**Pamagat**

![Pamagat ng laro](Docs/ReleasePreview/title.png)

**Laban sa Kalye ng Palengke**

![Laban sa palengke](Docs/ReleasePreview/match.png)

## Which file to download and upload

**Upload `ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.zip` to Canvas.** It contains the tested Windows x64 EXE and every required companion file. Size: 200,010,588 bytes. SHA-256: `e4dbd4a302f20bca4c52c8bc8b4c66e9f55f6110e41dc16069c310c5ece905c3`.

- [Google Drive folder](https://drive.google.com/drive/folders/1g1_TxVdJFwQab1QGKS9eUD65fRjbtfbH): verified game ZIP and supporting files; Anyone with the link: Viewer. [Download the game ZIP](https://drive.google.com/file/d/1wDqFNpaPGaU3QmxbVzE-fA0lP0VZ_1wr/view).
- [GitHub Release v1.0](https://github.com/Tinenen-cs/ILABAN-ANG-TAWA-DEVELOPED-BY-CHRISTINE-TAMAYO/releases/tag/v1.0): download the verified ZIP asset. GitHub replaces spaces with dots in its download filename; its hash is identical to the Canvas ZIP. The source repository is PUBLIC.

On a phone, download the ZIP then transfer it to a Windows computer, or download directly on a school PC. Right-click > Extract All, open the extracted folder and double-click **ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe**. Keep `_Data`, `UnityPlayer.dll`, `MonoBleedingEdge` and all other companions together. No Unity installation or game installer is needed.

The optional single-file SFX EXE was rejected: Windows requested administrator elevation (WinError 740). It is not recommended or uploaded. Use the safe ZIP. If Canvas rejects a raw EXE, upload the ZIP; rename a ZIP only if the professor explicitly instructs you to do so.

Defender scanned both candidate deliverables and found no threats on this PC. A clean-PC and unsigned-file SmartScreen check remain manual. If SmartScreen warns, verify the official download/hash and follow school policy; only choose More info > Run anyway when you trust that verified file. Never disable antivirus.

## Controls

- **Manlalaro 1:** A / D to move; F to punch and confirm.
- **Manlalaro 2:** Left / Right arrows to move; Right Ctrl or Numpad 0 to punch and confirm.
- **Gamepad:** left stick or D-pad to move; A / X to punch and confirm; B to go back. The first connected controller controls P1 and the second controls P2.
- **ESC:** pause or return from a menu. **H:** open controls during a match. **Alt+Enter:** toggle fullscreen and windowed mode.

## Game rules

Movement is left and right only. Every player has the same straight punch. A landed hit moves the target back exactly one step. Five consecutive hits make the target dizzy for two seconds; a miss, a three-second gap or being hit resets the streak. The round ends immediately when a player falls into the kanal. There are no health bars, knockout system, jump, kick, extra move or match timer.

## Submit in Canvas

Upload this exact file: **SUBMISSION/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.zip**. The instructor should extract it and see the portable EXE, its required Unity data and support files, and the Tagalog `BASAHIN_MUNA.txt` guide. No source project is needed unless the instructor separately asks for it.

## Requirements and credits

- Windows 10 or Windows 11, 64-bit.
- Two keyboard players share one computer, or connect compatible gamepads.
- See [CREDITS.md](CREDITS.md) for character, stage, font and audio credits; see [AUDIO_LIST.md](AUDIO_LIST.md) for cue details.

## Project verification

The source is developed with Unity 6000.6.2f1. Automated test, build, screenshot and package evidence is recorded under `Docs/QA_EVIDENCE/`, `Docs/portable_qa_results.json` and `Docs/QA_EVIDENCE/final_summary.json`. `Docs/package_verification.json` verifies the current ZIP, fresh extraction and complete simulated match. Optional SFX rejection and desktop/Defender results are recorded under Docs. Physical keyboard/controller/clean-PC/audio tests remain manual. See [CHECKLIST.md](CHECKLIST.md) for verified and manual items.
