# Bug log

Record only observed failures or concrete code defects, with verification status. Final results are appended after test runs.

| Issue | Fix | Verification |
| --- | --- | --- |
| Initial project still allowed jump, spells, guard, shove, combos and timeout draws | Replaced active rules with horizontal movement, equal punch and manhole-only ending | 64 initial pure C# assertions passed; expanded tests pending |
| Frame-dependent decaying knockback did not move a fixed distance | One direct 52-unit backward step per landed hit | 8 items, both directions and no residual drift passed; 50-hit Unity case added |
| Sequential attack resolution favored P1 | Snapshot both active strikes before applying either | Simultaneous displacement assertions passed |
| A simultaneous trade could incorrectly award fifth-hit dizziness | Trades now break both streaks before any dizzy award | New regression case added; rerun pending |
| New pad was assigned after its first event and missed its first punch | Pair on device-added/reconnected notification before processing input | Replacement regression rerun pending |
| Old disconnect behavior forced pause | Disconnect now preserves game state and keyboard availability | Virtual-device and interactive smoke verification pending |
| Both players could confirm with the same Enter key | Enter belongs only to P1; P2 uses Right Ctrl/Numpad 0 | Isolation cases passed before reconnect test |
| UI helper named Image hid Image.Type enum | Qualified UnityEngine.UI.Image.Type | Subsequent compilation reached validation |
| Edit-mode input simulation ignored runtime action processing | Run standalone input checks in Play Mode; add InputTestFixture tests | Initial keyboard/isolation cases passed in Play Mode |
| Result overlay covered reaction bubbles and fall presentation | Moved results to upper-center safe region; bubbles above actors | Graphical verification pending |
| Mirroring an actor with top-left pivot displaced it | Mirror atlas UV instead of the whole actor root | Graphical verification pending |
| TMP font creation accessed absent TMP Settings | Create and save TMP Settings before creating the static atlas | Build rerun pending |
| Unity test asmdefs duplicated implicit test-runner references | Keep optional TestAssemblies references and remove duplicate explicit entries | Compile rerun pending |

Original artwork, face files, audio and Main.unity have not been deleted. Procedural placeholders are explicitly tracked in Docs/ASSET_REPLACEMENTS.md.

## Claude Code session (2026-10-28)
| # | Bug | Status |
|---|---|---|
| C1 | Title "ILABAN ANG TAWA" invisible: 82pt label in 91px rect with Truncate overflow hid the whole line (AcademyGame.cs:140) | FIXED: overflow mode on that label; verified in built exe screenshot |
| C2 | Title advanced to Pumili ng Tauhan with no input after ~15s in built exe | NOT A BUG: runInBackground=0, so only the focused game window gets input; the newly launched window had focus and received outside keystrokes. Title correctly advances on any key. NEEDS MANUAL TEST: leave title idle 60s untouched |
| C3 | Character card names truncated (Manong Latalata, Aling Boteng Suka, Ate Bato-Bato Pik) | OPEN: fix with item C layout |
| C4 | QA capture method used screen copy and grabbed other windows | FIXED: captures now use PrintWindow on the game window only |
