from pathlib import Path
import json,hashlib
from PIL import Image
r=Path(__file__).resolve().parents[1]
data=json.loads((r/'Assets/Resources/Ilaban/Characters/HeadRatios.json').read_text(encoding='utf-8'))
assert len(data['frames'])==272
for f in data['frames']:
 p=r/'Assets/Resources'/(f['resource']+'.png')
 assert abs(f['ratio']-data['target'])<=.03,(p,'head ratio')
 assert hashlib.sha256(p.read_bytes()).hexdigest()==f['sha256'],(p,'art changed; remeasure')
 assert Image.open(p).size==(896,512)
print('PASS: 272 head ratios within 3 percentage points; measured art hashes/canvases match.')
