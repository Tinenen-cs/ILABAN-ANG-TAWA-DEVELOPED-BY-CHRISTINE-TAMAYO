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

## Historical v1.0 outcome
The free SDK adaptation produced B, but Windows normal launch returned WinError 740 (elevation required). B is rejected and kept local; no elevation or protection bypass was attempted. A passes extraction and full-match tests and is recommended for Canvas. This is a measured result of our adaptation, not a claim that the video's licensed WinRAR implementation fails.

## UPDATE2 free launcher adaptation

The earlier installer-oriented SDK SFX remains rejected for WinError 740. UPDATE2 uses the same complete-build temporary-extraction method with an original free C# GUI launcher (Tools/PortableLauncher.cs), compiled by the installed Windows .NET Framework compiler. No download, paid packer, installer, administrator prompt or console is used. Its normal application manifest is asInvoker; it does not modify or elevate the failed SFX.

The launcher embeds the verified complete ZIP and a per-file SHA-256 manifest, shows a short Tagalog progress window, extracts into a unique owned Temp directory, validates every file, launches the original Unity game with forwarded arguments, waits for it and cleans up only that owned folder after success. Zip paths are checked against traversal. Windows 10/11's built-in .NET Framework 4.x is required. The actual game remains native borderless Windows x64 Mono.

A one-file distribution still extracts its Unity dependencies at runtime. Tests must use copies of ONLY the outer EXE in empty Temp and space-containing folders, prove opening/title/select/fight/three-win results, scan the actual EXE and retain the complete folder/ZIP backup. Do not recommend or upload until those checks pass. Clean-PC and unsigned SmartScreen behavior remain manual.
