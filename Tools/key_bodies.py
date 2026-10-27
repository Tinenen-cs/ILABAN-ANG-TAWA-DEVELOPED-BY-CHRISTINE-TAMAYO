"""Chroma-key the Figma full-body poses (pure green background) into transparent, feet-aligned PNGs.

In:  Assets/Art/Characters/Source/body_N.jpg
Out: Assets/Art/Characters/Keyed/body_N.png  (cropped, feet on the bottom edge, height 1024)
"""
from pathlib import Path
import numpy as np
from PIL import Image, ImageFilter

ROOT = Path(__file__).resolve().parents[1] / 'Assets/Art/Characters'
(ROOT / 'Keyed').mkdir(exist_ok=True)
for n in range(1, 9):
    rgb = np.asarray(Image.open(ROOT / f'Source/body_{n}.jpg').convert('RGB')).astype(float)
    r, g, b = rgb[..., 0], rgb[..., 1], rgb[..., 2]
    greenness = g - np.maximum(r, b)
    alpha = np.clip(1 - (greenness - 40) / 60, 0, 1)          # soft edge between 40..100 green dominance
    # Despill: pull green down to the other channels on the fringe.
    rgb[..., 1] = np.where(greenness > 0, np.maximum(r, b) + np.clip(greenness, 0, 25) * .2, g)
    a = Image.fromarray((alpha * 255).astype(np.uint8)).filter(ImageFilter.MinFilter(3)).filter(ImageFilter.GaussianBlur(.6))
    im = Image.fromarray(rgb.astype(np.uint8)).convert('RGBA'); im.putalpha(a)
    box = im.getchannel('A').point(lambda v: 255 if v > 40 else 0).getbbox()
    im = im.crop((box[0] - 8, box[1] - 8, box[2] + 8, box[3]))
    scale = 1024 / im.height
    im.resize((round(im.width * scale), 1024), Image.Resampling.LANCZOS).save(ROOT / f'Keyed/body_{n}.png')
print('ok')
