# Whole-body strike artwork

Tool: built-in imagegen. Inputs: only the eight fictional `Assets/Resources/Ilaban/Characters/<Character>/Body.png` drawings. No photos or real-person likeness references were used.

Selected source sheets live in `ArtSource/StrikeSheets/<Character>.png`; runtime images are sliced and aligned by the local sprite import pipeline. Original sheets remain outside Unity Resources. Six phases in reading order: ready, wind-up, swing, fully extended strike, impact hold, recovery.

## Prompt set

Common prompt: preserve the fictional character's face, outfit colors, clean dark outlines and shaded cartoon style; use a consistent large-head chibi proportion, about three heads tall. Six whole-body grounded poses in a regular three-column, two-row grid on an alpha-transparent background. All poses face right in a side-on stance with a fixed camera, scale, feet-center pivot and ground line. Wind back, then extend the front arm and weapon horizontally at shoulder/chest height toward an opponent; brace for impact and recover. Exactly one weapon firmly gripped in each body drawing. Keep full head, feet, hands and weapon within a cell with transparent gutters. No labels, borders, background, duplicated weapon, extra arm, jump, kick or second attack. No effects in source sheets; the pipeline supplies trails, sparkle and dust separately from source art.

Character weapon directions:

- Aling Pamalo: one wooden laundry paddle, raised back then a side swing with the paddle head leading.
- Kuya Tsinelas: one blue flip-flop, a slap with the sole facing the target.
- Ate Bato-Bato Pik: one stone held inside the forward fist, overhand wind-up then forward extension; never release it.
- Kap Batuta: one black baton, raised wind-up then a straight horizontal jab.
- Manong Latalata: one intact silver can, wind back then a forward can swing; keep it visible during recovery.
- Tito Silya: one white plastic chair securely held in front with two hands, pull back then a short forward push-swing. Exactly two arms/hands.
- Aling Boteng Suka: one intact clear vinegar bottle with a red cap, draw back then a forward bottle swing. No broken glass.
- Nanay Tingting: one twig broom, both hands on the handle, draw back then sweep horizontally with the bristles leading.

Review corrections: removed the earlier procedural thin straight trail that looked like a floating stick. The generated Manong recovery was checked for a missing can; the Tito wind-up was checked for an extra hand. Corrected variants are selected before runtime import.

## Additional action sheets

Selected files: ArtSource/ActionSheets/<Character>.png, sliced into ArtSource/ActionFrames/<Character>/{WalkA,WalkB,Hurt,Dizzy,Win,Lose}.png. Prompt: use only the supplied fictional ready drawing; preserve adult chibi identity, outfit, one naturally gripped weapon and three-head proportions. Six isolated transparent whole-body poses in a 3x2 grid: two grounded walking strides, hurt recoil, confused dizzy face, confident win/ready signature expression, embarrassed lose pose. Keep two arms/hands, no jump/attack/special, no loose item, text or scenery. Feet align to the same ground after uniform scaling. All eight source sheets were visually reviewed before integration.
