# Current handoff ? 28 October 2026

Local branch: `phase3-polish`. Reference photos stay private and gitignored. No push, force-push, history rewrite or merge was made. The owner still controls repository privacy and publishing.

## Implemented

- Eight original fictional adult characters with Filipino internal resource keys, approved portraits/bases and six-part scripted cutout rigs. The rigs animate idle, walk, punch, hurt, dizzy, fall and win. Joint pivots and expression-head swaps remain partial.
- Three smooth painterly maps in Umaga, Hapon and Takipsilim. Existing dusk art remains; day versions turn lamps off.
- Original title, character-selection, stage-selection and opening backgrounds, five parallax layers, Lilita One/Nunito gameplay fonts, Tagalog UI and Pormal default reactions.
- Layout uses the supplied UI references only for screen organization. Visual treatment remains Filipino wood, tin, market and barangay scenery.
- Combat rules remain intact: left/right movement, no jump, straight punch, one fixed step per hit, five-hit dizzy for two seconds, no health/K.O./timer, manhole-only instant end, isolated player inputs.
- Dizzy indicator: five large supported asterisks orbit at y=350 and each draws last in the UI. P1/P2 reaction bubbles use non-overlapping central lanes with margins from the HUD.

## Final verification

- Unity 6000.6.2f1 Windows x64 build from a clean local clone of source commit `2f3fbbe`. At build time, the validation clone and real project had matching `git rev-parse HEAD`; the student's Unity Editor stayed open on the real folder. Build passed the 65 rubric assertions and reported no C# compiler errors or warnings.
- EditMode 9/9 and graphics-enabled PlayMode 4/4 passed. XML: `Docs/QA_EVIDENCE/EditMode.xml`, `Docs/QA_EVIDENCE/PlayMode.xml`.
- Portable smoke passed at 1280x720, 1366x768, 1920x1080 and 2560x1440. Each run captured opening, title, character select, all nine map/time choices, match, dizzy, result, pause and controls. I inspected dizzy, match and result captures; stars are visible, bubbles are separated, and HUD remains clear. Logs report no missing glyphs, TMP overflow or runtime exceptions. Full-resolution screenshot hashes are in `Docs/QA_EVIDENCE/captures.json`.
- The fresh portable ZIP was extracted to `C:\Users\chris\AppData\Local\Temp\Ilaban-Extracted-20261028-054255`, launched and played through a simulated gamepad match to dizzy and manhole result. No reported errors. This validates the development PC only.
- ZIP path, byte size, SHA-256 and extraction result are recorded in `Docs/package_verification.json`; four-resolution results are in `Docs/portable_qa_results.json`.

## Remaining NEEDS MANUAL TEST

- Physical USB gamepads, a second controller, hot-swap and unplug/replug feel.
- Two-person keyboard rollover and real input latency.
- Speaker/headphone balance and subjective audio mix.
- The actual presentation monitor's fit/readability.
- Launch on a clean PC without Unity and antivirus/SmartScreen behavior for the unsigned executable.

Score limitations: character motion is a simple cutout rig, and the original synthesized sound still needs listening review. No grade or Exceeds-level guarantee is claimed.

## Claude Code notes (resolved during finalization)

The stale submission, hidden/missing dizzy glyph, and touching reaction bubbles were identified in the prior audit. The submission ZIP was rebuilt from the validation build; the supported asterisk glyph and separated safe lanes are covered by the final four-resolution captures. `Tools/finalize_evidence.py` ran successfully and confirmed 100 captures plus both XML test suites and the ZIP hash.

Canvas submission instruction: upload only `SUBMISSION/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.zip`, unless the instructor requests supporting files. Do not publish this branch until the owner resolves the earlier public reference-photo history.
