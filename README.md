# ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO

A two-player Filipino street brawler made by Christine Tamayo. Pick a character, choose one of three barangay arenas, and push your rival into the open kanal. The game is Tagalog-first and runs as a portable Windows app.

## Preview

**Pamagat**

![Pamagat ng laro](Docs/ReleasePreview/title.png)

**Laban sa Kalye ng Palengke**

![Laban sa palengke](Docs/ReleasePreview/match.png)

## Download and open the portable game

The release page is [GitHub Releases](https://github.com/Tinenen-cs/ILABAN-ANG-TAWA-DEVELOPED-BY-CHRISTINE-TAMAYO/releases/latest). A new release is not published yet: final packaging/Drive is queued separately, and publishing is held for the owner decision about reference photos in public Git history. When version 1.0 is published, open the latest release and download **ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.zip**. On a phone, download the ZIP and transfer it to the Windows computer using a cable, cloud drive or school file service. You can also download the same ZIP directly on a school computer.

On Windows 10 or 11, right-click the ZIP and choose **Extract All**. Save it in a folder you can find, such as Documents. Open the extracted folder and double-click **ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe**. Keep the `_Data` folder, `UnityPlayer.dll` and all other included files beside the EXE. No Unity installation or game installer is needed.

If Windows SmartScreen shows a warning, choose **More info**, then **Run anyway**, only if you downloaded the game from the release page above. An antivirus may also ask about an unsigned student build. If it blocks the game, follow your school's device policy or ask the instructor rather than disabling protection.

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

The source is developed with Unity 6000.6.2f1. Automated test, build, screenshot and package evidence is recorded under `Docs/QA_EVIDENCE/`, `Docs/portable_qa_results.json` and `Docs/QA_EVIDENCE/final_summary.json`. The existing `Docs/package_verification.json` describes an older ZIP; current final packaging is queued separately. See [CHECKLIST.md](CHECKLIST.md) for verified and manual items.
