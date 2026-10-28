"""Prepare authored full-body strikes and whole-body effect variants; never split limbs."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageEnhance
import json,math
ROOT=Path(__file__).resolve().parents[1];ART=ROOT/'Assets/Resources/Ilaban/Characters'
CHARS=['AlingPamalo','KuyaTsinelas','AteBatoBatoPik','KapBatuta','ManongLatalata','TitoSilya','AlingBotengSuka','NanayTingting']
COLORS=[(245,188,99,255),(231,136,105,255),(174,218,226,255),(240,196,95,255),(189,199,194,255),(177,128,84,255),(103,190,146,255),(213,181,103,255)]
ACTIONS=['Idle','Walk','Strike','Hurt','Dizzy','Fall','Win','Lose'];SIZE=(896,512)

def star(d,x,y,r,c):
    pts=[]
    for n in range(10):
        a=-math.pi/2+n*math.pi/5;rr=r if n%2==0 else r*.42;pts.append((x+math.cos(a)*rr,y+math.sin(a)*rr))
    d.polygon(pts,fill=c)

def lean(image,amount):
    return image.transform(SIZE,Image.Transform.AFFINE,(1,amount,-amount*512,0,1,0),resample=Image.Resampling.BICUBIC)

def render(index,base,action,frame,anchor):
    body=base.copy();color=COLORS[index];cx=SIZE[0]//2
    if action=='Walk':body=lean(body,[-.009,0,.009,0][frame])
    if action=='Hurt':body=lean(body,[-.045,-.065,-.035,0][frame])
    # Keep the pre-effect body mask as structural evidence for every action frame.
    body_mask=body.getchannel('A');im=body.copy()
    if action=='Hurt' and frame==0:
        im=Image.blend(im,Image.new('RGBA',SIZE,(255,95,82,255)),.16);im.putalpha(body_mask)
    if action=='Lose':im=ImageEnhance.Color(im).enhance(.62)
    d=ImageDraw.Draw(im);gx,gy=anchor['grip'];tx,ty=anchor['tip']
    if action=='Strike':
        if frame in (2,3):
            # Curved ribbon only: no separate prop, straight stick or body-part overlay.
            d.arc((gx-38,gy-52,max(gx+25,tx+12),gy+42),205,326,fill=(*color[:3],185),width=10)
            d.arc((gx-31,gy-45,max(gx+18,tx+5),gy+34),208,322,fill=(255,239,188,175),width=3)
        if frame==4:
            for k in range(10):
                a=k*math.pi/5;d.line((tx+math.cos(a)*7,ty+math.sin(a)*7,tx+math.cos(a)*23,ty+math.sin(a)*23),fill=color,width=3)
            star(d,tx,ty,9,(255,244,195,255))
        if frame in (2,3,4):
            d.ellipse((cx-139,495,cx-83,504),fill=(213,174,116,90));d.ellipse((cx+83,495,cx+139,504),fill=(213,174,116,70))
            star(d,gx,gy,5,color)
    elif action=='Idle':
        if frame in (1,3):star(d,tx+8,ty-9,5,color)
    elif action=='Walk':
        if frame%2:
            d.ellipse((cx-141,495,cx-75,503),fill=(208,177,125,75));d.ellipse((cx+75,495,cx+141,503),fill=(208,177,125,75))
    elif action=='Hurt':
        if frame in (1,2):
            star(d,cx-140,225,9,(255,107,102,200))
    elif action=='Dizzy':
        for k in range(3):star(d,cx-52+k*52,30+(k%2)*8,9,color)
    elif action=='Fall':
        for k in range(6):d.arc((cx-32-k*4,473-k*2,cx+32+k*4,507+k*2),190,350,fill=(104,202,201,210),width=3)
    elif action=='Win':
        star(d,cx-135+frame*3,72,9,color);star(d,cx+135-frame*3,96,8,(255,243,191,255))
    return im,body_mask

def main():
    manifest=[];anchors={x['character']:x for x in json.loads((ROOT/'Docs/character_weapon_anchors.json').read_text())}
    for i,name in enumerate(CHARS):
        source=ROOT/'ArtSource/StrikeFrames'/name;ready=Image.open(source/'Strike_00.png').convert('RGBA')
        out=ART/name/'Actions';out.mkdir(exist_ok=True);masks=ROOT/'ArtSource/ActionMasks'/name;masks.mkdir(parents=True,exist_ok=True)
        for action in ACTIONS:
            count=6 if action=='Strike' else 4
            for f in range(count):
                base=Image.open(source/f'Strike_{f:02}.png').convert('RGBA') if action=='Strike' else ready
                anchor=anchors[name]['frames'][f if action=='Strike' else 0]
                image,body_mask=render(i,base,action,f,anchor);path=out/f'{action}_{f:02}.png';image.save(path,optimize=True);body_mask.save(masks/path.name,optimize=True)
                manifest.append({'file':path.relative_to(ROOT).as_posix(),'size':list(SIZE),'action':action,'frame':f,'weapon':anchors[name]['item'],'weapon_is_baked_into_fullbody':True,'strike_pose_placeholder':False,'action_pose_placeholder':action not in ('Strike','Idle'),'phase':anchor['phase'] if action=='Strike' else None,'grip':anchor['grip'] if action=='Strike' else None,'tip':anchor['tip'] if action=='Strike' else None,'pivot':[.5,0],'foot_pivot':[448,512],'ground_line':512,'body_mask':(masks/path.name).relative_to(ROOT).as_posix()})
    (ROOT/'Docs/character_action_frames.json').write_text(json.dumps(manifest,indent=2),encoding='utf-8')
    print(f'Prepared {len(manifest)} whole-body frames, including 48 authored weapon-strike poses; no reference photos read.')

if __name__=='__main__':main()
