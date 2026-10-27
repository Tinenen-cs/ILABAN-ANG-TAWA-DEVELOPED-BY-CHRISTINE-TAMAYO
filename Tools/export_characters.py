"""Export approved character art into runtime Resources under the new internal keys.

Roster order (Roster.cs) differs from the design-sheet order (Docs/CHARACTER_DESIGN.md), so map by index.
Out: Assets/Resources/Ilaban/Characters/<Key>/Body.png (512 tall, feet on bottom edge), Portrait.png (512x512)
"""
from pathlib import Path
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
ART = ROOT / 'Assets/Art/Characters'
OUT = ROOT / 'Assets/Resources/Ilaban/Characters'
KEYS = ['AlingPamalo', 'KuyaTsinelas', 'AteBatoBatoPik', 'KapBatuta', 'ManongLatalata', 'TitoSilya', 'AlingBotengSuka', 'NanayTingting']
DESIGN = [1, 5, 2, 6, 8, 7, 3, 4]  # roster index -> design-sheet number

for key, n in zip(KEYS, DESIGN):
    d = OUT / key
    d.mkdir(parents=True, exist_ok=True)
    body = Image.open(ART / f'Keyed/body_{n}.png')
    body.resize((round(body.width * 512 / body.height), 512), Image.Resampling.LANCZOS).save(d / 'Body.png')
    Image.open(ART / f'Source/portrait_{n}.jpg').convert('RGB').resize((512, 512), Image.Resampling.LANCZOS).save(d / 'Portrait.png')
print('ok')
