# Credits

Developer: Christine Tamayo.

## Active fonts

- **Lilita One**, Juan Pablo del Peral ? OFL 1.1. Source: https://github.com/google/fonts/tree/main/ofl/lilitaone . Source and license are included in Assets/Fonts. Static SDF `KalyeDisplay` for titles and large names.
- **Nunito Bold**, Vernon Adams / Cyreal / Jacques Le Bailly and Nunito project contributors ? OFL 1.1. Source: https://github.com/google/fonts/tree/main/ofl/nunito . Included license in Assets/Fonts. Static SDF `KalyeBody` for labels, hints and reactions. Bilinear atlases and shared dark outline/underlay; printable ASCII and Filipino glyph checks in RubricTests.
- Inactive legacy fonts remain in the archive for rollback; active UI uses only the pair above.
- TMP essential resources include Liberation Sans for editor/default support, under its bundled OFL license at `Assets/TextMesh Pro/Fonts/LiberationSans - OFL.txt`; gameplay labels explicitly use KalyeDisplay/KalyeBody.

## Active character and environment art

- Eight original fictional adult chibi portraits and approved full-body bases: text-prompt generation recorded by the preceding art session; source files under Assets/Art/Characters, design notes in Docs/CHARACTER_DESIGN.md. No photographs, identities or copied likenesses are used as image inputs. Original real-person reference photos are private local files and excluded from Git on this branch.
- Scripted registered body partitions: Tools/rig_characters.py, derived from the approved fictional base poses. Runtime keys are Filipino names; internal legacy academy resources are inactive.
- Three approved Takipsilim backgrounds: retained unchanged in Assets/Resources/Ilaban/Maps.
- Six Umaga/Hapon map edits and four original screen backgrounds: generated with the built-in image_gen tool using only text and approved fictional scenery. Saved paths, generation source paths and prompt summaries: Docs/new_art_manifest.json. Each output was viewed. Smooth painterly treatment; no heavy pixel filter. No third-party map art was copied.
- Original procedural item icons, UI primitives, jeepney and effects: Tools/generate_ilaban_assets.py. Source and scope documented in Docs/ASSET_REPLACEMENTS.md. These small assets still have a simple placeholder aesthetic.
- New AI-generated art is project-specific; no third-party royalty-free license is asserted for image-generator outputs. Original procedural new assets/audio are dedicated CC0-1.0 as stated below. Unity/TMP shaders and packages retain their own licenses.

## Active audio: complete file credits

All titles below are original synthesized cues or compositions generated for Christine Tamayo. Source for every file: [generate_ilaban_assets.py](Tools/generate_ilaban_assets.py). License for every newly generated clip: [CC0-1.0](https://creativecommons.org/publicdomain/zero/1.0/). No external samples or songs are used. See AUDIO_LIST.md for loudness, playback and cue placement.

- `Assets/Resources/Ilaban/Audio/Music/Title.wav` ? **Himig Title**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/Music/Selection.wav` ? **Himig Selection**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/Music/Palengke.wav` ? **Himig Palengke**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/Music/Looban.wav` ? **Himig Looban**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/Music/Plaza.wav` ? **Himig Plaza**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/menu_move.wav` ? **menu_move**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/menu_back.wav` ? **menu_back**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/menu_confirm.wav` ? **menu_confirm**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/carousel_slide.wav` ? **carousel_slide**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/highlight_loop.wav` ? **highlight_loop**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/confirm_sting.wav` ? **confirm_sting**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/punch.wav` ? **punch**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/step.wav` ? **step**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/dizzy.wav` ? **dizzy**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/fall.wav` ? **fall**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/splash.wav` ? **splash**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/win.wav` ? **win**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/opening_bell.wav` ? **opening_bell**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/jeepney_engine.wav` ? **jeepney_engine**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/banderitas.wav` ? **banderitas**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/opening_sting.wav` ? **opening_sting**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/crowd.wav` ? **crowd**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/hit_0.wav` ? **hit_0**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/hit_1.wav` ? **hit_1**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/hit_2.wav` ? **hit_2**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/hit_3.wav` ? **hit_3**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/hit_4.wav` ? **hit_4**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/hit_5.wav` ? **hit_5**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/hit_6.wav` ? **hit_6**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.
- `Assets/Resources/Ilaban/Audio/SFX/hit_7.wav` ? **hit_7**; author: original procedural synthesis for Christine Tamayo; source: `Tools/generate_ilaban_assets.py`; CC0-1.0.

## Inherited inactive assets

Original academy artwork/audio and Cinzel, Cinzel Decorative and EB Garamond fonts remain for rollback. Font licenses are preserved in Assets/Art/Fonts. See Docs/README_ACADEMY_ORIGINAL.md and AUDIO_LIST.md for legacy provenance. No repository-wide license is changed by the new-media CC0 declaration. Unity and its packages remain under their respective software terms.

## Fonts (Phase 3 update: smooth fonts replace the pixel font)
| Font | Use | Author | Source | License |
|---|---|---|---|---|
| Lilita One | Display: logo, titles, name plates (TMP asset `KalyeDisplay`) | Juan Montoreano | https://github.com/google/fonts/tree/main/ofl/lilitaone | SIL OFL 1.1 (`Assets/Fonts/OFL_LilitaOne.txt`) |
| Nunito Bold | Body: hints, descriptions, bubbles, menus (TMP asset `KalyeBody`) | The Nunito Project Authors (Vernon Adams, Jacques Le Bailly) | https://github.com/googlefonts/nunito (file via Fontsource CDN) | SIL OFL 1.1 (`Assets/Fonts/OFL_Nunito.txt`) |

## Map art
| File | Source | Notes |
|---|---|---|
| `Maps/Palengke`, `Looban`, `Plaza` `_Takipsilim.png` | AI image generation in the Codex session "Reskin game as Ilaban Ang Tawa" (OpenAI image tool), approved by the student | Original 1672x941, unmodified |
| `Maps/FX/*.png` | Generated by `Tools/generate_map_fx.py` (this project) | Clouds, lamp glow, floor shimmer, mist, imburnal |

## Character art (Phase 3 update)
| File | Tool | Notes |
|---|---|---|
| `Assets/Art/Characters/Source/portrait_1..8.jpg` | Figma AI image generation (model gemini-3.1-flash-lite-image), file https://www.figma.com/design/9J9nqsjyg38lIZVMeLXRYC | Generated from WRITTEN descriptions only (Docs/CHARACTER_DESIGN.md). The student's reference photos were used only as expression and outfit reference and were NOT uploaded. Original fictional adult characters. |
