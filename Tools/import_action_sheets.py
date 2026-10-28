"""Slice complete authored walk/reaction drawings and fix their feet-center origin."""
from pathlib import Path
from PIL import Image
import json,numpy as np
from import_strike_sheets import regions,SIZE
root=Path(__file__).resolve().parents[1];poses=['WalkA','WalkB','Hurt','Dizzy','Win','Lose'];report=[]
for name in ['AlingPamalo','KuyaTsinelas','AteBatoBatoPik','KapBatuta','ManongLatalata','TitoSilya','AlingBotengSuka','NanayTingting']:
    source=root/'ArtSource/ActionSheets'/f'{name}.png';image=Image.open(source).convert('RGBA');bodies=[g for g in regions(np.asarray(image)[:,:,3]) if g['area']>10000]
    assert len(bodies)==6,(name,'need six whole bodies',[(g['area'],g['box']) for g in bodies])
    bodies=sorted(bodies,key=lambda g:(g['box'][1]+g['box'][3])/2);bodies=sorted(bodies[:3],key=lambda g:g['box'][0])+sorted(bodies[3:],key=lambda g:g['box'][0]);scale=496/max(g['box'][3]-g['box'][1] for g in bodies)
    out=root/'ArtSource/ActionFrames'/name;out.mkdir(parents=True,exist_ok=True)
    for pose,g in zip(poses,bodies):
        b=g['box'];feet=[r for r in g['runs'] if r[0]>=b[3]-24];foot_x=(min(r[1] for r in feet)+max(r[2] for r in feet))/2
        crown=[r for r in g['runs'] if r[0]<b[1]+26];head_x=(min(r[1] for r in crown)+max(r[2] for r in crown))/2
        mask=np.zeros((image.height,image.width),dtype=bool)
        for y,left,right in g['runs']:mask[y,left:right]=True
        for _ in range(2):
            p=np.pad(mask,1);mask=p[1:-1,1:-1]|p[:-2,1:-1]|p[2:,1:-1]|p[1:-1,:-2]|p[1:-1,2:]
        rgba=np.array(image);rgba[:,:,3]=np.where(mask,rgba[:,:,3],0);box=[max(0,b[0]-3),max(0,b[1]-3),min(image.width,b[2]+3),min(image.height,b[3]+3)]
        cut=Image.fromarray(rgba).crop(box);cut=cut.resize((round(cut.width*scale),round(cut.height*scale)),Image.Resampling.LANCZOS);x=round(SIZE[0]/2-(foot_x-box[0])*scale);y=round(SIZE[1]-(b[3]-box[1])*scale)
        assert x>=0 and x+cut.width<SIZE[0],(name,pose,'would clip')
        frame=Image.new('RGBA',SIZE);frame.alpha_composite(cut,(x,y));frame.save(out/(pose+'.png'),optimize=True)
        report.append({'character':name,'pose':pose,'file':(out/(pose+'.png')).relative_to(root).as_posix(),'head':[round(x+(head_x-box[0])*scale),round(y+(b[1]-box[1])*scale)],'foot_pivot':[448,512],'scale':scale,'source_box':box})
(root/'Docs/character_action_poses.json').write_text(json.dumps(report,indent=2),encoding='utf-8');print('Imported 48 authored whole-body walk/reaction poses, shared canvas/pivot.')
