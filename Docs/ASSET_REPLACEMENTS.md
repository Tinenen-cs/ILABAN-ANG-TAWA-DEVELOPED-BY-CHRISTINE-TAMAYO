# Art finishing inventory

The executable uses clean procedural placeholder bodies, items, UI and map sketches. Original illustrated faces are preserved. This is playable production scaffolding, not final reference-quality artwork.

- Eight `Bodies/Body_0` through `Body_7`: 256 x 672 atlas, 4 columns x 7 rows of 64 x 96. Rows: idle, walk, punch, hurt, dizzy, fall, win. The face is a separate unchanged portrait layer. Keep the head region clear; no extra attacks.
- Eight `Items/Item_0` through `Item_7`: 32 x 32 transparent; pamalo, tsinelas, bato, batuta, lata, silya, bote, walis tingting. Item appearance cannot alter reach or timing.
- Three `Stages/Palengke`, `Stages/Looban`, `Stages/Plaza`: 640 x 360 minimum; full street plane under the actors, empty HUD/bubble space, hazards added separately. Draw Philippine storefronts, rooftops, wiring, banderitas and dusk lighting. Reference photos guide style only.
- Three `UI/Imburnal_*`: 128 x 64 transparent. Shared hazard logic is at stage edges regardless of the illustration.
- `UI/Bumalik`, `UI/Simulan`: 192 x 56; warm painted street-sign / torn-paper buttons. Text is drawn separately in Tagalog.
- `UI/HiloDot`: 24 x 24; `UI/Suntok`: 96 x 96; `UI/PortraitMask`: 128 x 128.
- `UI/SpeechBubble`: 96 x 40; 8-pixel nine-slice borders; tail at bottom. No text baked in.
- `UI/Glow`: 64 x 64; `UI/Vignette`: 320 x 180.
- `UI/Jeepney`: 160 x 80; `UI/Banderitas`: 640 x 72.

Use Point filtering, Compression None, mipmaps off, wrap Clamp. UI atlases are sampled by UV; the consistent sprite conversion basis is 100 pixels per unit. Runtime importer enforces Point/no compression for Ilaban resources. Do not overwrite original portrait files; review any face change separately.

UI screenshot layout is used for hierarchy and placement only. Philippine visual treatment must remain distinct from the supplied fantasy design.
