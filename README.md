# ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO

A Unity 2D, local two-player college game project by Christine Tamayo. The Filipino street-brawler adaptation is in progress. The current playable baseline still contains academy artwork and the original moves; it is not the final rubric-compliant game.

## Open on this computer

Use the installed **Unity 6000.6.2f1** Editor. The downloaded project originally recorded 6000.3.27f1, which is not installed here. Let the upgraded editor finish importing and compiling before pressing Play.

1. In Unity Hub, choose Add project from disk and select the project folder containing Assets, Packages and ProjectSettings.
2. Choose Unity 6000.6.2f1. Open Assets/Scenes/Main.unity.
3. Press Play and click inside the Game view. Press any key to start.
4. For the existing baseline, P1 selects with WASD and confirms/punches with F; P2 uses arrows and J. Enter advances the menus. The new controls and restricted moveset are tracked in CHECKLIST.md.

Windows Build Support is installed for this editor. The original scene and Resources paths remain intact. Do not open the upgraded project in an older editor; restore the external backup first if rollback is needed.

## Progress and acceptance

- [CHECKLIST.md](CHECKLIST.md): all requested features, including eight item-themed characters and three maps.
- [TEST_PLAN.md](TEST_PLAN.md): manual acceptance tests; pending tests are not claimed as passes.
- [CREDITS.md](CREDITS.md): source and license inventory.
- [Original README](Docs/README_ACADEMY_ORIGINAL.md): preserved baseline documentation, controls and asset workflow; historical claims there are not verification on this Windows computer.

Confirmed final maps: **Kalye sa Palengke, Looban, Plaza ng Barangay**. Character reference images are in CHARACTER REFERENCE; map references are in MAPS REFERCES (original spelling).

## Existing source structure

- Assets/Scenes/Main.unity: single saved entry scene.
- Assets/Scripts/Runtime/AcademyGame.cs: title, selection, fight, pause, result UI and audio.
- Assets/Scripts/Runtime/DuelModel.cs: pure C# combat and round rules.
- Assets/Scripts/Runtime/Roster.cs: eight fighters and three stage definitions.
- Assets/Scripts/Runtime/AcademyInputManager.cs: per-player input maps and device assignment.
- Assets/Resources/Input/AcademyControls.inputactions: editable action asset.
- Assets/Resources/Characters, Effects, Stages, Audio, UI: runtime assets.
- Assets/Scripts/Editor: scene setup, baseline validation and Windows build commands.
- Tools: existing asset/audio generators and combat checks.

The baseline uses immediate-mode GUI and sprite-sheet cells, not Animator controllers or Rigidbody2D. Preserve its scene structure and asset GUIDs while implementing the new requirements.

## Build

Use **Ilaban ang Tawa > Build Windows x64** in the editor. The build command uses Mono and writes the required executable filename into Builds/Windows. This command being available does not mean a final build has passed acceptance.

Distribute the entire output folder, including its Data directory and UnityPlayer.dll, inside a ZIP. An EXE by itself is not portable. Clean-PC testing remains required.

## Repository and backup

The existing repository was renamed to [ILABAN-ANG-TAWA-DEVELOPED-BY-CHRISTINE-TAMAYO](https://github.com/Tinenen-cs/ILABAN-ANG-TAWA-DEVELOPED-BY-CHRISTINE-TAMAYO), retaining its identity and history. The old Fight-Fght URL redirects to it. The pre-upgrade source archive is outside this folder in Fight-Fght-backups. No original art has been deleted.
