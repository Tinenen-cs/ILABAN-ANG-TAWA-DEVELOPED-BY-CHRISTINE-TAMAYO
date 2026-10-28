# Portable executable method

Reference: [How to Build a Standalone Executable with Unity](https://www.youtube.com/watch?v=h6BcN6Z289w). Retrieved auto-subtitles, metadata, description and chapters with yt-dlp on 2026-10-28; raw reference copies remain local under Logs/VIDEO_REFERENCE.

## Verified video method

The video teaches WinRAR self-extraction, not Enigma virtualization:

1. Make the normal Unity build folder with all dependencies.
2. Optionally create a 96x96 image and convert it to ICO.
3. Add the entire build folder to a WinRAR archive.
4. Choose Fastest compression and Create SFX archive.
5. Under Advanced > SFX options > Setup, enter the relative folder and game EXE path.
6. Under Modes, select unpack to a temporary folder.
7. Under Text and icon, choose the ICO, then build and launch the SFX.
8. The presenter recommends keeping extraction progress visible so users know large builds are starting.

Chapters: Intro 0:00, Prerequisites 0:20, Tutorial 0:55.

## Project additions and free-tool adaptation

WinRAR is not assumed to have a paid license. Use the free [7-Zip/LZMA SDK](https://www.7-zip.org/sdk.html) SFX module from the official download, implementing the same temporary-extraction-and-launch method. Its SDK files are public domain; include its license/provenance. This is our adaptation, not a video claim. Keep compression low for faster start, visible progress, the full build hierarchy and exact game name. The safe ZIP needs no packer at runtime. Test the single file independently before recommending it; any issue makes the ZIP the recommended Canvas file.

Windows x64 Mono, native borderless fullscreen, no console, Tagalog/Pormal defaults, extracted-folder QA, Defender scans, SHA-256 manifests and Drive/GitHub verification are project-specific additions. Unity's [Windows build reference](https://docs.unity3d.com/6000.0/Documentation/Manual/WindowsStandaloneBinaries.html) is the official build source. No installer, service, registry setup or antivirus exclusion is added by our packaging.

## Actual outcome
The free SDK adaptation produced B, but Windows normal launch returned WinError 740 (elevation required). B is rejected and kept local; no elevation or protection bypass was attempted. A passes extraction and full-match tests and is recommended for Canvas. This is a measured result of our adaptation, not a claim that the video's licensed WinRAR implementation fails.
