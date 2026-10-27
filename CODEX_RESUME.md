# CODEX RESUME (read this first, then HANDOFF.md)

- Branch: `phase3-polish` (local only, NOT pushed). Last commit: run `git log -1`. Never merge to `main` without approval.
- One agent at a time. Commit after every step and update HANDOFF.md.
- PRIVACY: `CHARACTER REFERENCE/` photos are public on origin/main and origin/phase2-ui. They are untracked on phase3-polish.
  Do not push or rewrite history until the student decides (top of HANDOFF.md).

## Decisions
3 maps only; 9 map backgrounds (Umaga / Hapon / Takipsilim); layout-only UI references with a Filipino street look;
Lilita One + Nunito, no pixel text; original chibi adult characters, no likeness, photos never uploaded; Pormal default;
the item is only a visual skin of the straight punch; scene order Pambungad, Pamagat, Pumili ng Tauhan, Pumili ng Entablado,
Laban, Resulta; no merge to main without approval.

## Next 5 steps
1. Image work (HANDOFF.md section E): Umaga/Hapon for the 3 maps (lamps OFF, same layout), then the 4 screen backgrounds. [image tool]
2. Scripted character rig and switch to the new keys/art (HANDOFF.md B2). Re-run rules tests. [no image tool needed]
3. Oras selector on Pumili ng Entablado, select-screen layout redo (B3), HUD polish (B4).
4. Audio (B5), English-string scan, QA screenshots at 4 resolutions (B7), master CHECKLIST.md + Rubric Self-Check (B8).
5. Build, SUBMISSION zip with BASAHIN_MUNA.txt, extract-and-run test.

Resume commands: HANDOFF.md section G. Figma: https://www.figma.com/design/9J9nqsjyg38lIZVMeLXRYC

## Codex continuation: character rig step (28 October 2026)
Approved original base art now split into six disjoint registered visual parts by Tools/rig_characters.py. Runtime loads the new Filipino keys, full portraits, and code-driven part transforms. No real-person photos used or uploaded. UI name-label heights adjusted. Runtime compilation/tests still pending for this step. No push or merge permitted.

### Continuation: layout and time controls
Oras selector wired for Umaga/Hapon/Takipsilim (Up/Down); dusk-only lamp glow. Actor aspect ratios preserved, two-line card names and transition fade added. Removed unused spell/knockout pose and round helper remnants; Jump remains a test-only sentinel. Compile and screenshot verification pending. Latest resume: open this project on phase3-polish; read CODEX_RESUME.md then HANDOFF.md; do not push.

### Artwork checkpoint
Six lighting-only map edits plus four original screen backgrounds generated using image_gen (text/map inputs only), each viewed and imported as new assets. Dusk maps unchanged. Full paths and prompt summaries: Docs/new_art_manifest.json. Lamps disabled by code outside Takipsilim. Full build/QA of integrated art pending. No remote writes.
