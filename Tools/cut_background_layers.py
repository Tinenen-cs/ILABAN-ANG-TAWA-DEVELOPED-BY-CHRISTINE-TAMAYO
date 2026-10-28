"""Extract masked pixels from approved fictional paintings, without repainting sources.
Semantic color masks are restricted to reviewed upper-scene regions; no floor/vehicles.
"""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json,hashlib
from import_strike_sheets import regions
R=Path(__file__).resolve().parents[1];base=R/'Assets/Resources/Ilaban'
scenes={
 'Palengke_Umaga':('Maps/Palengke/Palengke_Umaga.png',[(.32,.18,.44,.44),(.77,0,1,.22)],(.28,.05,.73,.23),(.32,.13,.71,.29)),
 'Looban_Gabi':('Maps/Looban/Looban_Gabi.png',[(0,0,.48,.25),(.92,.12,1,.43)],(.38,.10,.68,.22),(.49,.03,.66,.20)),
 'Plaza_Gabi':('Maps/Plaza/Plaza_Gabi.png',[(0,0,.40,.25),(.78,0,1,.21)],(.12,.06,.94,.29),(.46,.03,.69,.15)),
 'Pamagat':('Backgrounds/Pamagat.png',[(0,0,.25,.36),(.78,0,1,.28)],(.22,.30,.66,.51),(.30,.10,.64,.32)),
 'PumiliEntablado':('Backgrounds/PumiliEntablado.png',[(0,0,.26,.30),(.78,0,1,.24)],(.02,0,.98,.22),(.32,.12,.68,.25)),
}
report=[]
for name,(source,leaf_boxes,flag_box,cloud_box) in scenes.items():
 im=Image.open(base/source).convert('RGBA');rgb=np.asarray(im).astype(float);h,w=rgb.shape[:2];red,green,blue=[rgb[:,:,i] for i in range(3)]
 # Green foliage, saturated festival cloth, warm lit glass, and soft sky texture.
 predicates={'leaf':(green>red*1.06)&(green>blue*1.04)&(green>22),
 'flag':((np.maximum.reduce([red,green,blue])-np.minimum.reduce([red,green,blue]))>55)&((red>blue*1.2)|(blue>red*1.15)),
 'glow':(red>155)&(green>75)&(red>blue*1.45)&(green>blue*1.16),
 'cloud':(blue>red*.72)&(blue>green*.90)}
 layers=[];out=base/'Motion'/name;out.mkdir(parents=True,exist_ok=True)
 specs=[(f'Leaf{i}','leaf',box) for i,box in enumerate(leaf_boxes)]+[('Flags','flag',flag_box),('LampsWindows','glow',(0,.12,1,.51)),('Clouds','cloud',cloud_box)]
 for asset,kind,box in specs:
  region=Image.new('L',(w,h));draw=ImageDraw.Draw(region);draw.rectangle(tuple(round(v*(w if i%2==0 else h)) for i,v in enumerate(box)),fill=255)
  # Erode/feather the region rim before intersecting the exact pixel selection.
  region=region.filter(ImageFilter.GaussianBlur(5));alpha=np.asarray(region,dtype=float)*predicates[kind]
  if kind=='flag':
   # Keep individual cloth-sized connected regions, rejecting whole sky/roof patches.
   selected=np.zeros((h,w),dtype='uint8')
   for g in regions(alpha.astype('uint8')):
    x0,y0,x2,y2=g['box']
    if 40<g['area']<12000 and x2-x0<w*.085 and y2-y0<h*.15:
     for yy,left,right in g['runs']:selected[yy,left:right]=255
   alpha=selected
   if name=='Looban_Gabi':
    # This approved painting has wires but no cloth flags. Reuse our original
    # painted pennant instead of labelling a moving sky rectangle as a flag.
    for k in range(3):layers.append(dict(id='Festival'+str(k),kind='flag',textureResource='Ilaban/UI/Festival/Flag'+str(k),x=.45+k*.065,y=.12+k*.007,width=.024,height=.055))
    continue
  mask=Image.fromarray(alpha.astype('uint8')).filter(ImageFilter.GaussianBlur(.6));bbox=mask.getbbox();assert bbox,(name,asset)
  rgba=im.copy();rgba.putalpha(mask);rgba=rgba.crop(bbox);rgba.save(out/(asset+'.png'),optimize=True)
  x,y,x1,y1=bbox;layers.append(dict(id=asset,kind=kind,x=x/w,y=y/h,width=(x1-x)/w,height=(y1-y)/h))
  assert y1/h<.56,(name,asset,'must not reach combat floor')
 (out/'Layout.json').write_text(json.dumps({'source':source,'layers':layers},indent=2),encoding='utf-8')
 report.append(dict(painting=name,source=source,source_sha256=hashlib.sha256((base/source).read_bytes()).hexdigest(),layers=layers))
(R/'Docs/background_layer_sources.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print('Extracted painting-specific foliage/cloth/light/sky layers; Looban reuses three original festival pennants because its source has wires but no flags. Source pixels unchanged.')
