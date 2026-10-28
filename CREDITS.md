# Credits

Developer: Christine Tamayo.

## Active fonts

- **Lilita One**, Juan Pablo del Peral ? OFL 1.1. Source: https://github.com/google/fonts/tree/main/ofl/lilitaone. License is included in Assets/Fonts.
- **Nunito Bold**, Vernon Adams / Cyreal / Nunito contributors ? OFL 1.1. License is included in Assets/Fonts.
- TMP essential resources retain their bundled license in Assets/TextMesh Pro.

## Character art

- Eight fictional adult characters use original full-body base art and portraits. No real-person photos or likeness inputs were supplied to image tools.
- Full-body action frames (idle, walk, strike, hurt, dizzy, fall, win and lose) are generated locally from the approved fictional full-body bases. Every frame uses a 272x512 transparent canvas and shared bottom-center feet pivot. Six strike placeholders include a broad curved trail, impact burst/sparkle and foot dust. The underlying item stays in the source carry pose; the weapon is not yet reposed into the requested shoulder-height strike. `Tools/generate_character_action_frames.py` reads only character base PNGs in `Assets/Resources/Ilaban/Characters`.
- `Tools/generate_hud_art.py` creates the original varnished wood/brass portrait ring, carved Hilo bar, five inset segment frames and winner medals. The HUD art uses no supplied reference pixels.
- Original local reference photos remain private and ignored by Git. See `Docs/character_action_frames.json` and `Docs/character_action_validation.json` for asset dimensions and silhouette checks.

## Maps and interface art

- Three active maps: Kalye sa Palengke (Umaga), Looban (Takipsilim), and Plaza ng Barangay (Takipsilim). Original art manifests and prompt summaries are in `Docs/new_art_manifest.json`.
- Jeepney frames, open kanal art, UI plates and small effects are original assets made from text prompts or local procedural drawing. See `Docs/ASSET_REPLACEMENTS.md`.
- Image-generation outputs are project-specific; no third-party image license is claimed for them.

## Audio

- All active WAV files are original procedural cues generated for Christine Tamayo. Source: `Tools/generate_ilaban_assets.py`; license: CC0-1.0. No external songs or samples are used. See `AUDIO_LIST.md` for playback roles and technical checks.
- `punch.wav` is the short swing cue; `hit_0.wav` through `hit_7.wav` are item-specific impact cues. Other action cues cover steps, dizzy, fall, splash, win and crowd reactions. Repeated impact sounds are rate-limited in the runtime to avoid harsh stacking.
