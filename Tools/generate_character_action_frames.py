"""Generate whole-body action placeholders from local fictional character art only."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageEnhance
import json,math
ROOT=Path(__file__).resolve().parents[1];ART=ROOT/'Assets/Resources/Ilaban/Characters'
CHARS=['AlingPamalo','KuyaTsinelas','AteBatoBatoPik','KapBatuta','ManongLatalata','TitoSilya','AlingBotengSuka','NanayTingting']
ITEMS=['Pamalo','Tsinelas','Bato','Batuta','Lata','Silya','Bote','Walis tingting']
COLORS=[(245,188,99,255),(231,136,105,255),(174,218,226,255),(240,196,95,255),(189,199,194,255),(177,128,84,255),(103,190,146,255),(213,181,103,255)]
ACTIONS=['Idle','Walk','Strike','Hurt','Dizzy','Fall','Win','Lose']
ANCHORS=[{'grip':[122,357],'tip':[199,348]},{'grip':[164,194],'tip':[205,184]},{'grip':[183,236],'tip':[218,230]},{'grip':[130,340],'tip':[207,334]},{'grip':[185,300],'tip':[220,298]},{'grip':[205,320],'tip':[235,330]},{'grip':[174,264],'tip':[212,254]},{'grip':[205,300],'tip':[250,300]}]
def star(d,x,y,r,c):
 pts=[]
 for n in range(10):
  a=-math.pi/2+n*math.pi/5;rr=r if n%2==0 else r*.42;pts.append((x+math.cos(a)*rr,y+math.sin(a)*rr))
 d.polygon(pts,fill=c)
def base_frame(source):
 im=Image.open(source).convert('RGBA');im=im.crop(im.getchannel('A').getbbox());h=500;w=max(1,round(im.width*h/im.height));im=im.resize((w,h),Image.Resampling.LANCZOS)
 canvas=Image.new('RGBA',(272,512),(0,0,0,0));canvas.alpha_composite(im,((272-w)//2,12));return canvas
def lean(image,amount):
 # The single full-body image leans as one shape; the bottom-center pivot stays fixed.
 return image.transform((272,512),Image.Transform.AFFINE,(1,amount,-amount*512,0,1,0),resample=Image.Resampling.BICUBIC)
def render(index,base,action,frame,anchor):
 im=base.copy();d=ImageDraw.Draw(im);color=COLORS[index]
 if action=='Strike':
  gx,gy=anchor['grip'];tx,ty=anchor['tip']
  if frame==1:im=lean(im,-.018)
  elif frame in (2,3,4):im=lean(im,.025 if frame<4 else .012)
  d=ImageDraw.Draw(im)
  if frame==2:
   d.arc((gx-28,gy-50,gx+62,gy+38),205,332,fill=(*color[:3],210),width=5)
   d.arc((gx-21,gy-43,gx+54,gy+31),207,327,fill=(255,240,190,170),width=2)
  elif frame==3:
   # A broad curved motion ribbon reads as an effect; avoid a thin straight
   # line from the hand to the tip, which looked like a second floating stick.
   d.arc((gx-22,gy-52,tx+20,gy+43),205,326,fill=(*color[:3],205),width=10)
   d.arc((gx-16,gy-45,tx+13,gy+35),208,322,fill=(255,239,188,185),width=3)
  elif frame==4:
   cx,cy=tx,ty
   for k in range(10):
    a=k*math.pi/5;d.line((cx+math.cos(a)*5,cy+math.sin(a)*5,cx+math.cos(a)*21,cy+math.sin(a)*21),fill=color,width=3)
   star(d,cx,cy,9,(255,244,195,255))
  if frame in (2,3,4):
   d.ellipse((43,495,98,504),fill=(213,174,116,90));d.ellipse((178,495,230,504),fill=(213,174,116,70))
  if frame in (1,2,3,4):star(d,gx,gy,5,color);d.ellipse((gx-3,gy-3,gx+3,gy+3),fill=color)
 elif action=='Idle':
  if frame in (1,3):star(d,207+(frame-1)*9,145,6,color)
 elif action=='Walk':
  if frame%2:d.ellipse((35,495,101,503),fill=(208,177,125,75));d.ellipse((178,495,242,503),fill=(208,177,125,75))
 elif action=='Hurt':
  if frame==0:im=Image.blend(im,Image.new('RGBA',im.size,(255,95,82,32)),.16);d=ImageDraw.Draw(im)
  if frame in (1,2):d.line((22,245,8,259),fill=(255,226,163,220),width=4);d.line((244,238,264,246),fill=(255,226,163,220),width=4)
 elif action=='Dizzy':
  for k in range(3):star(d,84+k*52,30+(k%2)*8,9,color)
 elif action=='Fall':
  d.ellipse((42,493,231,508),fill=(52,83,91,85))
  for k in range(6):d.arc((105-k*4,473-k*2,169+k*4,507+k*2),190,350,fill=(104,202,201,210),width=3)
 elif action=='Win':star(d,42+frame*3,72,9,color);star(d,230-frame*3,96,8,(255,243,191,255))
 elif action=='Lose':im=ImageEnhance.Color(im).enhance(.62);d=ImageDraw.Draw(im);d.ellipse((62,492,210,507),fill=(55,71,77,100))
 return im
def main():
 manifest=[]
 for i,name in enumerate(CHARS):
  base=base_frame(ART/name/'Body.png');out=ART/name/'Actions';out.mkdir(exist_ok=True)
  for action in ACTIONS:
   count=6 if action=='Strike' else 4
   for f in range(count):
    path=out/f'{action}_{f:02}.png';render(i,base,action,f,ANCHORS[i]).save(path,optimize=True)
    manifest.append({'file':path.relative_to(ROOT).as_posix(),'size':[272,512],'action':action,'frame':f,'weapon':ITEMS[i] if action=='Strike' else None,'weapon_is_baked_into_fullbody':True,'strike_pose_placeholder':action=='Strike','strike_pose_note':'weapon remains in relaxed carry pose; item-specific reach pose not drawn yet' if action=='Strike' else None,'grip':ANCHORS[i]['grip'] if action=='Strike' else None,'tip':ANCHORS[i]['tip'] if action=='Strike' else None,'pivot':'bottom-center; shared ground line'})
 (ROOT/'Docs/character_action_frames.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
 (ROOT/'Docs/character_weapon_anchors.json').write_text(json.dumps([{'character':n,'item':ITEMS[i],**ANCHORS[i]} for i,n in enumerate(CHARS)],ensure_ascii=False,indent=2),encoding='utf-8')
 print(f'Generated {len(manifest)} whole-body frames for eight characters; strike frames have six phases; no reference photos read.')
if __name__=='__main__':main()
