from pathlib import Path
from PIL import Image,ImageDraw
import math
root=Path('Assets/Resources/Ilaban/UI');root.mkdir(exist_ok=True)
# Brass-edged varnished wood portrait ring; transparent portrait aperture.
S=256;ring=Image.new('RGBA',(S,S),(0,0,0,0));d=ImageDraw.Draw(ring)
d.ellipse((5,5,251,251),fill=(95,50,29,255),outline=(240,200,124,255),width=5)
d.ellipse((14,14,242,242),fill=(144,86,48,255),outline=(56,37,28,255),width=5)
d.ellipse((22,22,234,234),fill=(177,112,63,255),outline=(222,179,109,255),width=8)
d.ellipse((35,35,221,221),fill=(0,0,0,0),outline=(63,42,30,255),width=9)
for a in range(0,360,30):
 r=math.radians(a);x=128+math.cos(r)*112;y=128+math.sin(r)*112
 d.ellipse((x-3,y-3,x+3,y+3),fill=(255,224,160,255))
grain=Image.new('RGBA',ring.size);d=ImageDraw.Draw(grain)
for x in range(47,209,18):d.line((x,18,x+9,14),fill=(198,132,75,105),width=2)
ring=Image.alpha_composite(ring,grain)
ring.save(root/'HudWoodRing.png',optimize=True)
# Single long wood-and-tin channel for the Hilo segments.
W,H=760,120;bar=Image.new('RGBA',(W,H),(0,0,0,0));d=ImageDraw.Draw(bar)
d.rounded_rectangle((2,2,W-3,H-3),radius=28,fill=(83,49,32,255),outline=(239,197,123,255),width=5)
d.rounded_rectangle((11,11,W-12,H-12),radius=22,fill=(111,67,42,255),outline=(55,39,31,255),width=3)
d.rounded_rectangle((18,18,W-19,H-19),radius=16,fill=(69,49,37,255),outline=(193,143,86,255),width=2)
grain=Image.new('RGBA',bar.size);d=ImageDraw.Draw(grain)
for y in (27,35,87,95):d.line((30,y,W-30,y+(-2 if y<50 else 1)),fill=(223,176,105,65),width=2)
for x in range(32,W-30,46):d.line((x,24,x+12,94),fill=(244,207,142,24),width=2)
bar=Image.alpha_composite(bar,grain)
bar.save(root/'HudWoodBar.png',optimize=True)
# Segment plate tinted by Unity from amber through vermilion.
seg=Image.new('RGBA',(180,56),(0,0,0,0));d=ImageDraw.Draw(seg);d.rounded_rectangle((1,1,178,54),radius=10,fill=(231,170,82,255),outline=(255,225,157,255),width=3);d.rounded_rectangle((6,6,173,49),radius=7,outline=(127,75,39,255),width=2)
seg.save(root/'HudSegment.png',optimize=True)
# Small round medal with a five-point star.
med=Image.new('RGBA',(96,96),(0,0,0,0));d=ImageDraw.Draw(med);d.ellipse((5,5,91,91),fill=(103,59,35,255),outline=(240,198,121,255),width=5);d.ellipse((13,13,83,83),fill=(188,122,60,255),outline=(250,223,166,255),width=3)
pts=[]
for n in range(10):
 a=-math.pi/2+n*math.pi/5;r=27 if n%2==0 else 12;pts.append((48+math.cos(a)*r,48+math.sin(a)*r))
d.polygon(pts,fill=(255,236,183,255),outline=(104,62,34,255));med.save(root/'HudMedal.png',optimize=True)
print('Generated wood ring, Hilo bar, five-segment plate, and medal art.')
