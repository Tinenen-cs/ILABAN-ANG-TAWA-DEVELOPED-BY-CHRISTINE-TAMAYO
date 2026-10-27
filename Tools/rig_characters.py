from pathlib import Path
from PIL import Image,ImageDraw
import json
root=Path('.')
keys=['AlingPamalo','KuyaTsinelas','AteBatoBatoPik','KapBatuta','ManongLatalata','TitoSilya','AlingBotengSuka','NanayTingting']
for key in keys:
 folder=root/'Assets/Resources/Ilaban/Characters'/key
 im=Image.open(folder/'Body.png').convert('RGBA');w,h=im.size
 # Disjoint partition of the approved base, kept on a common canvas for exact registration.
 boundaries={'Head':[(0,0),(w,0),(w,.43*h),(0,.43*h)],'Torso':[(.27*w,.43*h),(.70*w,.43*h),(.70*w,.72*h),(.27*w,.72*h)],'ArmL':[(0,.43*h),(.27*w,.43*h),(.27*w,.72*h),(0,.72*h)],'ArmR':[(.70*w,.43*h),(w,.43*h),(w,.72*h),(.70*w,.72*h)],'LegL':[(0,.72*h),(.50*w,.72*h),(.50*w,h),(0,h)],'LegR':[(.50*w,.72*h),(w,.72*h),(w,h),(.50*w,h)]}
 for name,points in boundaries.items():
  mask=Image.new('L',im.size);ImageDraw.Draw(mask).polygon(points,fill=255)
  import PIL.ImageChops
  part=im.copy();part.putalpha(PIL.ImageChops.multiply(im.getchannel('A'),mask));part.save(folder/(name+'.png'))
Path('Docs/RIG_PARTITIONS.json').write_text(json.dumps(dict(note='Disjoint normalized base-image partitions; approved original artwork unchanged. Visual transforms only; no combat model access.',keys=keys,head_bottom=.43,torso_bottom=.72),indent=2),encoding='utf-8')
print('Exported six registered rig parts per adult character.')
