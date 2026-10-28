from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
import json, numpy as np
root=Path(__file__).resolve().parents[1]
chars=['AlingPamalo','KuyaTsinelas','AteBatoBatoPik','KapBatuta','ManongLatalata','TitoSilya','AlingBotengSuka','NanayTingting']
actions=['Idle','Walk','Strike','Hurt','Dizzy','Fall','Win','Lose']
report=[];out=root/'QA_SCREENSHOTS'/'ANIMATION';out.mkdir(parents=True,exist_ok=True)
font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',16)
for name in chars:
 folder=root/'Assets'/'Resources'/'Ilaban'/'Characters'/name/'Actions'
 for action in actions:
  frames=sorted(folder.glob(action+'_*.png'))
  expected=6 if action=='Strike' else 4
  assert len(frames)==expected,(name,action,len(frames))
  for frame in frames:
   im=Image.open(frame).convert('RGBA');assert im.size==(272,512),(frame,im.size);box=im.getchannel('A').getbbox();assert box and box[3]==512,(frame,box)
   report.append({'character':name,'action':action,'frame':frame.name,'size':list(im.size),'pivot':'feet-center; shared bottom ground line'})
 for action in ('Strike','Hurt'):
  cellw,cellh=136,256;margin=12;labelh=26;cols=6 if action=='Strike' else 4
  sheet=Image.new('RGBA',(margin+cols*(cellw+margin),labelh+cellh+margin),(15,29,38,255));d=ImageDraw.Draw(sheet)
  d.text((margin,4),f'{name} - {action} / frames 1-4',font=font,fill=(247,221,177,255))
  for i,frame in enumerate(sorted(folder.glob(action+'_*.png'))):
   im=Image.open(frame).convert('RGBA').resize((cellw,cellh),Image.Resampling.LANCZOS);x=margin+i*(cellw+margin);y=labelh
   sheet.alpha_composite(im,(x,y));d.text((x,y+cellh-20),str(i+1),font=font,fill=(255,255,255,255))
  sheet.convert('RGB').save(out/f'{name}-{action.lower()}-frames.png')
manifest=root/'Docs'/'character_action_frames.json';data=json.loads(manifest.read_text(encoding='utf-8'));assert len(data)==272
anchors={x['character']:x for x in json.loads((root/'Docs'/'character_weapon_anchors.json').read_text(encoding='utf-8'))}
# Validate shared source-body silhouette with a 4-connected component pass.
def components(path):
 a=np.asarray(Image.open(path).convert('RGBA'))[:,:,3]>32;h,w=a.shape;seen=np.zeros_like(a);sizes=[]
 for y,x in zip(*np.where(a&~seen)):
  if seen[y,x]:continue
  stack=[(int(y),int(x))];seen[y,x]=1;count=0
  while stack:
   cy,cx=stack.pop();count+=1
   for ny,nx in ((cy-1,cx),(cy+1,cx),(cy,cx-1),(cy,cx+1)):
    if 0<=ny<h and 0<=nx<w and a[ny,nx] and not seen[ny,nx]:seen[ny,nx]=1;stack.append((ny,nx))
  sizes.append(count)
 return sorted(sizes,reverse=True)
for name in chars:
 anchor=anchors[name]['grip'];strike_paths=sorted((root/'Assets'/'Resources'/'Ilaban'/'Characters'/name/'Actions').glob('Strike_*.png'))
 for frame in strike_paths:
  im=Image.open(frame).convert('RGBA');patch=im.getchannel('A').crop((anchor[0]-4,anchor[1]-4,anchor[0]+5,anchor[1]+5));assert patch.getbbox(),(name,frame.name,'grip outside whole-body frame')
 sizes=components(root/'Assets'/'Resources'/'Ilaban'/'Characters'/name/'Body.png')
 report.append({'character':name,'action':'BodySilhouette','connected_components_over_100px':sum(x>100 for x in sizes),'largest_component_pixels':sizes[0],'grip_anchor':anchors[name]['grip'],'strike_frames':6})
(root/'Docs'/'character_action_validation.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
face=root/'QA_SCREENSHOTS'/'ANIMATION'/'FaceToFace';face.mkdir(parents=True,exist_ok=True)
for a in range(len(chars)):
 for b in range(a+1,len(chars)):
  sheet=Image.new('RGBA',(584,552),(15,29,38,255));d=ImageDraw.Draw(sheet);d.text((12,6),f'{chars[a]} vs {chars[b]}',font=font,fill=(247,221,177,255))
  left=Image.open(root/'Assets/Resources/Ilaban/Characters'/chars[a]/'Actions'/'Idle_00.png').convert('RGBA').resize((272,512),Image.Resampling.LANCZOS)
  right=Image.open(root/'Assets/Resources/Ilaban/Characters'/chars[b]/'Actions'/'Idle_00.png').convert('RGBA').transpose(Image.Transpose.FLIP_LEFT_RIGHT).resize((272,512),Image.Resampling.LANCZOS)
  sheet.alpha_composite(left,(12,30));sheet.alpha_composite(right,(300,30));sheet.convert('RGB').save(face/f'{chars[a]}-vs-{chars[b]}.png')
print('face-off pair captures',28)
print('validated',len([x for x in report if 'frame' in x]),'frames; strike/hurt montages',len(chars)*2)
for x in report:
 if x.get('action')=='BodySilhouette':print(x)
