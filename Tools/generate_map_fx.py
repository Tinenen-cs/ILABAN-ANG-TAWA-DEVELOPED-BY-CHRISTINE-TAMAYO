"""Soft (non-pixel) overlay textures that sit on the painterly maps.

Output: Assets/Resources/Ilaban/Maps/FX/{Imburnal,Cloud,Glow,Shimmer,Mist}.png
"""
from pathlib import Path
import numpy as np
from PIL import Image, ImageFilter, ImageDraw

OUT = Path(__file__).resolve().parents[1] / 'Assets/Resources/Ilaban/Maps/FX'
OUT.mkdir(parents=True, exist_ok=True)

def radial(w, h, power=2.0):
    y, x = np.mgrid[0:h, 0:w]
    d = np.sqrt(((x - w / 2) / (w / 2)) ** 2 + ((y - h / 2) / (h / 2)) ** 2)
    return np.clip(1 - d, 0, 1) ** power

def rgba(rgb, alpha):
    a = np.dstack([np.full(alpha.shape, c, np.uint8) for c in rgb] + [(alpha * 255).astype(np.uint8)])
    return Image.fromarray(a, 'RGBA')

# Manhole seen at a low angle on wet concrete: iron rim, grate slots, a dark wet halo.
W, H = 512, 160
im = Image.new('RGBA', (W, H), (0, 0, 0, 0))
halo = rgba((8, 10, 14), radial(W, H, 1.2) * .55)
im.alpha_composite(halo)
d = ImageDraw.Draw(im)
box = (46, 30, W - 46, H - 30)
d.ellipse(box, fill=(52, 50, 48, 255))
d.ellipse((60, 38, W - 60, H - 40), fill=(28, 27, 27, 255))
for i in range(-5, 6):  # grate slots, foreshortened
    x = W / 2 + i * 30
    d.line((x - 8, 48, x + 8, H - 50), fill=(70, 66, 60, 255), width=5)
d.arc(box, 190, 340, fill=(214, 168, 104, 255), width=4)  # warm rim highlight from street light
im = im.filter(ImageFilter.GaussianBlur(1.2))
im.save(OUT / 'Imburnal.png')

cloud = np.zeros((128, 384))
for cx, cy, r in [(110, 78, 52), (190, 60, 70), (280, 80, 50), (200, 92, 60)]:
    y, x = np.mgrid[0:128, 0:384]
    cloud = np.maximum(cloud, np.clip(1 - np.hypot(x - cx, y - cy) / r, 0, 1) ** .6)
Image.fromarray((cloud * 255).astype(np.uint8)).filter(ImageFilter.GaussianBlur(6))
rgba((255, 244, 230), np.asarray(Image.fromarray((cloud * 255).astype(np.uint8)).filter(ImageFilter.GaussianBlur(6))) / 255 * .9).save(OUT / 'Cloud.png')

rgba((255, 214, 150), radial(256, 256, 2.2)).save(OUT / 'Glow.png')

band = np.tile(np.exp(-((np.arange(512) - 256) / 90.0) ** 2), (96, 1)) * np.exp(-((np.arange(96)[:, None] - 48) / 30.0) ** 2)
rgba((255, 236, 200), band * .8).save(OUT / 'Shimmer.png')

rng = np.random.default_rng(7)
mist = np.asarray(Image.fromarray((rng.random((32, 128)) * 255).astype(np.uint8)).resize((1024, 256), Image.Resampling.BICUBIC).filter(ImageFilter.GaussianBlur(18))) / 255
mist = mist * np.exp(-((np.arange(256)[:, None] - 128) / 80.0) ** 2)
rgba((236, 242, 250), np.clip(mist, 0, 1) * .7).save(OUT / 'Mist.png')
print('ok')
