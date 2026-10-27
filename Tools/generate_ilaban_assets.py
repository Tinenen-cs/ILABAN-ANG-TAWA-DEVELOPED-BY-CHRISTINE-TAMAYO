"""Original procedural placeholder art and synthesized Filipino-inspired music.
Never overwrites academy artwork. Reproducible, no third-party samples.
"""
from pathlib import Path
from PIL import Image, ImageDraw
import numpy as np
import json, wave, math
ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/'Assets/Resources/Ilaban'
for part in ['UI','Items','Bodies','Stages','Audio/Music','Audio/SFX']: (OUT/part).mkdir(parents=True,exist_ok=True)
COLORS=['#e2993e','#4cc4b4','#d884a1','#70ae74','#dcaf50','#66a4cc','#ac90ce','#df9472']

def save(im,path): im.save(OUT/(path+'.png'))

def item(i):
 im=Image.new('RGBA',(32,32));d=ImageDraw.Draw(im);wood='#ad693c';edge='#392b34';light='#ebc188'
 if i==0: d.rounded_rectangle((10,2,24,18),3,fill=light,outline=edge,width=2);d.rectangle((15,17,19,30),fill=wood,outline=edge)
 if i==1: d.ellipse((7,2,24,30),fill='#ec704c',outline=edge,width=2);d.line((10,12,16,18,22,10),fill=light,width=3)
 if i==2: d.polygon([(4,22),(7,10),(18,5),(27,16),(23,27),(11,28)],fill='#9fa7a5',outline=edge);d.line((8,14,16,9,23,16),fill='#d7dbcb',width=2)
 if i==3: d.rounded_rectangle((13,2,20,29),2,fill='#34454b',outline=edge,width=2);d.rectangle((7,18,23,22),fill='#677275')
 if i==4: d.rectangle((7,7,25,27),fill='#babbaa',outline=edge,width=2);d.ellipse((7,3,25,11),fill='#e3dfbe',outline=edge,width=2);d.rectangle((9,14,23,23),fill='#c86c43')
 if i==5:
  d.rectangle((7,1,24,15),fill=wood,outline=edge,width=2);d.rectangle((3,16,28,20),fill=light,outline=edge);d.line((6,19,4,31),fill=wood,width=4);d.line((25,19,28,31),fill=wood,width=4)
 if i==6: d.rectangle((13,1,20,9),fill='#c7a26c');d.polygon([(13,8),(20,8),(25,15),(25,30),(8,30),(8,15)],fill='#4d9978',outline=edge);d.rectangle((10,18,23,25),fill='#e4d6a4')
 if i==7:
  d.line((15,1,17,20),fill=wood,width=3)
  for x in range(5,29,3):d.line((16,15,x,31),fill='#c9ac62',width=2)
 return im
for i in range(8):save(item(i),f'Items/Item_{i}')
# New body atlases. Faces are drawn separately from unchanged original portrait resources at runtime.
for i,c in enumerate(COLORS):
 atlas=Image.new('RGBA',(256,672))
 for row in range(7):
  for frame in range(4):
   im=Image.new('RGBA',(64,96));d=ImageDraw.Draw(im);bob=frame%2 if row in (0,1,4,6) else 0
   stride=[-4,0,4,0][frame] if row==1 else 0
   d.line((26,70+bob,24+stride,90),fill='#28353e',width=8);d.line((38,70+bob,40-stride,90),fill='#28353e',width=8)
   d.rectangle((17+stride,88,29+stride,93),fill='#49382f');d.rectangle((35-stride,88,47-stride,93),fill='#49382f')
   d.polygon([(20,36+bob),(44,36+bob),(47,70+bob),(17,70+bob)],fill=c,outline='#2d2a36')
   d.rectangle((21,53+bob,43,58+bob),fill='#e9d7a6');d.line((31,38,31,65),fill='#fff0c8',width=2)
   extension=[0,4,8,2][frame] if row==2 else 0
   armY=48 if row!=6 else 31
   d.line((20,42+bob,13,57+bob),fill='#d8a078',width=6);d.line((43,42+bob,47+extension,armY+bob),fill='#d8a078',width=6)
   art=item(i).resize((20,20),Image.Resampling.NEAREST)
   im.alpha_composite(art,(min(36+extension,44),armY-12+bob))
   if row==3: im=im.transform(im.size,Image.Transform.AFFINE,(1,.10,-5,0,1,0))
   atlas.alpha_composite(im,(frame*64,row*96))
 save(atlas,f'Bodies/Body_{i}')
# Original pixel street sketches, deliberately labelled placeholders in the asset inventory.
for stage,name in enumerate(['Palengke','Looban','Plaza']):
 im=Image.new('RGB',(640,360),'#192b46');d=ImageDraw.Draw(im)
 for y in range(360):
  t=y/360;d.line((0,y,640,y),fill=(int(23+25*t),int(42+16*t),int(64+18*t)))
 d.ellipse((487,28,536,77),fill='#f3d0a0')
 for idx,x in enumerate(range(-10,650,95)):
  y=75+(idx%3)*19; col=['#8a6556','#b07755','#566f71','#9c7e65'][(idx+stage)%4]
  d.rectangle((x,y,x+89,247),fill=col,outline='#29343d',width=3)
  d.polygon([(x-5,y),(x+44,y-25),(x+94,y)],fill='#473c45')
  for xx in [x+13,x+53]:
   d.rectangle((xx,y+24,xx+24,y+62),fill='#efc184',outline='#463b3a',width=3)
   d.line((xx+12,y+24,xx+12,y+62),fill='#60504a',width=2)
  d.rectangle((x+29,185,x+60,249),fill='#35494d')
  if stage==0:
   d.rectangle((x-4,172,x+93,184),fill='#dec49b');
   for xx in range(x-4,x+93,18):d.rectangle((xx,172,xx+9,183),fill='#bd6054')
   d.rectangle((x+4,224,x+81,246),fill='#8f6748');
   for xx in range(x+9,x+79,10):d.ellipse((xx,217,xx+8,225),fill='#d8b85b')
  if stage==1:
   d.line((x,130,x+90,145),fill='#252833',width=2)
   for xx in range(x+8,x+90,16):d.rectangle((xx,132,xx+10,147),fill='#adb9ab')
 if stage==2:
  d.rectangle((240,143,400,248),fill='#c5a181',outline='#42474c',width=3)
  d.polygon([(230,143),(320,99),(410,143)],fill='#815d58');d.rectangle((293,182,347,248),fill='#354e5c')
  d.ellipse((281,133,359,153),fill='#e3cf9f');d.rectangle((308,155,332,178),fill='#608f84')
 d.rectangle((0,249,640,360),fill='#3c525e')
 for y in range(255,360,14):
  d.line((0,y,640,y),fill='#58717b',width=1)
  for x in range((y%28)*3,640,61):d.line((x,y,x-8,y+13),fill='#293c4a')
 for x in [25,605]:
  d.rectangle((x,109,x+4,256),fill='#28343d');d.rectangle((x-5,109,x+10,129),fill='#f2c379',outline='#594e49',width=2)
 d.line((0,55,640,66),fill='#2c2835',width=2)
 for x in range(5,640,24): d.polygon([(x,56),(x+15,57),(x+8,72)],fill=['#cb7462','#e3c16b','#74ad9e'][x%3])
 save(im,f'Stages/{name}')
# UI primitives, authored without touching the original image files.
for name,size in [('PortraitMask',(128,128)),('HiloDot',(24,24)),('Suntok',(96,96))]:
 im=Image.new('RGBA',size);d=ImageDraw.Draw(im);w,h=size
 d.ellipse((1,1,w-2,h-2),fill='#f0d3a2',outline='#9e734a',width=max(1,w//22))
 if name=='Suntok':
  d.rounded_rectangle((25,28,68,65),8,fill='#ddaa7f',outline='#513b37',width=3)
  for x in range(27,66,10):d.line((x,29,x,42),fill='#7c5245',width=2)
  d.rounded_rectangle((55,43,77,57),4,fill='#ddaa7f',outline='#513b37',width=2)
 save(im,f'UI/{name}')
for name in ['Bumalik','Simulan']:
 im=Image.new('RGBA',(192,56));d=ImageDraw.Draw(im)
 d.polygon([(7,5),(181,2),(188,10),(185,20),(191,33),(182,48),(15,53),(3,46),(8,36),(1,20)],fill='#765039',outline='#dfb777',width=3);save(im,f'UI/{name}')
im=Image.new('RGBA',(96,40));d=ImageDraw.Draw(im);d.rectangle((1,1,94,32),fill='#f4e3b9',outline='#443c3a',width=2);d.polygon([(44,31),(52,39),(58,31)],fill='#f4e3b9');save(im,'UI/SpeechBubble')
for name in ['Palengke','Looban','Plaza']:
 im=Image.new('RGBA',(128,64));d=ImageDraw.Draw(im);d.ellipse((2,10,125,59),fill='#b09d7a',outline='#28343d',width=3);d.ellipse((10,15,117,55),fill='#101e29');d.arc((13,19,115,54),0,175,fill='#65817c',width=3);d.line((24,14,42,2,111,4,117,16),fill='#cab78b',width=5);save(im,f'UI/Imburnal_{name}')
im=Image.new('RGBA',(320,180));p=im.load()
for y in range(180):
 for x in range(320):
  r=((x-160)/173)**2+((y-90)/109)**2;p[x,y]=(5,14,25,int(min(150,max(0,(r-.25)*90))))
save(im,'UI/Vignette')
im=Image.new('RGBA',(64,64));p=im.load()
for y in range(64):
 for x in range(64):p[x,y]=(255,199,103,int(max(0,1-math.hypot(x-31.5,y-31.5)/32)**2*180))
save(im,'UI/Glow')
# Original short plucked-string compositions; inspired by rondalla timbre, no borrowed melodies.
RATE=22050;rng=np.random.default_rng(4126);inventory=[]
def audio(name,signal,music=False,title='',description=''):
 signal=np.asarray(signal,dtype=np.float64);signal-=signal.mean();rms=np.sqrt(np.mean(signal**2));signal*=min(.82/(np.max(np.abs(signal))+1e-9),(.16 if music else .14)/(rms+1e-9))
 if not music:
  fade=min(len(signal)//2,220);signal[:fade]*=np.linspace(0,1,fade);signal[-fade:]*=np.linspace(1,0,fade)
 stereo=np.stack([signal,np.roll(signal,93)*.92],axis=1) if music else signal
 path=OUT/'Audio'/('Music' if music else 'SFX')/(name+'.wav')
 with wave.open(str(path),'wb') as f:f.setnchannels(2 if music else 1);f.setsampwidth(2);f.setframerate(RATE);f.writeframes((stereo*32767).astype('<i2').tobytes())
 inventory.append(dict(file=str(path.relative_to(ROOT)).replace('\\','/'),title=title or name,author='Original procedural synthesis for Christine Tamayo',source='Tools/generate_ilaban_assets.py',license='CC0-1.0',license_url='https://creativecommons.org/publicdomain/zero/1.0/',description=description,rms=float(np.sqrt(np.mean(signal**2))),peak=float(np.max(np.abs(signal))),loop=music))
def pluck(note,duration=.6):
 t=np.arange(int(RATE*duration))/RATE;f=440*2**((note-69)/12);return sum(np.sin(2*np.pi*f*h*t)*np.exp(-t*(4+h))/h**1.5 for h in range(1,7))*np.minimum(1,t/.004)
for idx,name in enumerate(['Title','Selection','Palengke','Looban','Plaza']):
 beat=[.5,.375,.4,.5,.6][idx];length=beat*32;n=int(length*RATE);s=np.zeros(n)
 notes=[[60,64,67,72,69,67,64,62],[67,69,72,76,74,72,69,67],[62,66,69,74,71,69,66,64],[57,60,64,69,67,64,60,59],[65,69,72,77,74,72,69,67]][idx]
 for k in range(32):
  v=pluck(notes[(k+idx*(k//8))%8],.85)*.32;at=int(k*beat*RATE);indices=(at+np.arange(len(v)))%n;np.add.at(s,indices,v)
  if k%4==0:
   v=pluck(notes[0]-24,1.0)*.36;np.add.at(s,(at+np.arange(len(v)))%n,v)
 audio(name,s,True,'Himig '+name,'Original looping plucked-string sketch; listening review pending.')
for idx,name in enumerate(['menu_move','menu_back','menu_confirm','carousel_slide','highlight_loop','confirm_sting','punch','step','dizzy','fall','splash','win']+[f'hit_{i}' for i in range(8)]):
 duration=2.4 if name=='win' else 1.1 if name in ['dizzy','fall','highlight_loop'] else .25
 t=np.arange(int(RATE*duration))/RATE;noise=rng.normal(0,1,len(t));base=120+idx*31
 if name=='win':
  s=np.zeros(len(t))
  for k,note in enumerate([60,64,67,72]):
   v=pluck(note,.9)*.6;at=int(k*.3*RATE);s[at:at+min(len(v),len(s)-at)]+=v[:len(s)-at]
 elif name in ['splash','punch','step','carousel_slide']:s=noise*np.exp(-t*(9 if name!='splash' else 4))*.35
 elif name=='dizzy':s=np.sin(2*np.pi*(480*t+80*np.sin(2*np.pi*3*t)))*np.exp(-t*3)
 elif name=='fall':s=np.sin(2*np.pi*(650*t-230*t*t))*np.exp(-t*3)
 else:s=(np.sin(2*np.pi*base*t)+.35*np.sin(2*np.pi*base*2.71*t)+noise*.12)*np.exp(-t*(9 if name.startswith('hit') else 13))
 audio(name,s,title=name,description='Original synthesized cue; item timbre is a placeholder.' if name.startswith('hit') else 'Original synthesized game cue.')
(ROOT/'Docs/ilaban_audio_manifest.json').write_text(json.dumps(inventory,indent=2),encoding='utf-8')
print('Generated 8 body atlases, 8 items, 3 stages, UI primitives and',len(inventory),'audio clips.')
