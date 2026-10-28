from pathlib import Path
from PIL import Image,ImageDraw
root=Path('Assets/Resources/Ilaban/Maps/FX')
flag=Image.new('RGBA',(64,96));d=ImageDraw.Draw(flag);d.polygon([(5,4),(59,4),(32,87)],fill=(255,255,255,255));d.line([(5,4),(59,4)],fill=(205,190,159,255),width=3);flag.save(root/'Flag.png',optimize=True)
leaf=Image.new('RGBA',(128,96));d=ImageDraw.Draw(leaf)
for box in [(8,12,66,37),(37,31,101,54),(62,7,120,29),(17,53,73,80)]:
 d.ellipse(box,fill=(47,84,53,255),outline=(85,111,59,255),width=2)
d.line([(10,77),(103,16)],fill=(74,79,42,255),width=4);leaf.save(root/'Leaves.png',optimize=True)
print('Two original isolated ambience accents generated.')
