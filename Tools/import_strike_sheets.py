"""Slice authored whole-body sheets; align complete sprites, never rig body parts."""
from pathlib import Path
import json
import numpy as np
from PIL import Image

ROOT=Path(__file__).resolve().parents[1]
SIZE=(896,512)
PHASES=['Ready','WindUp','Swing','Strike','Impact','Recovery']

def regions(alpha):
    """4-connected alpha runs, including thin weapon handles and broom twigs."""
    parent=[];runs=[];previous=[]
    def find(x):
        while parent[x]!=x:
            parent[x]=parent[parent[x]];x=parent[x]
        return x
    def union(a,b):
        a,b=find(a),find(b)
        if a!=b:parent[b]=a
    for y,row in enumerate(alpha>32):
        edges=np.flatnonzero(np.diff(np.pad(row.astype(np.int8),(1,1))))
        current=[];j=0
        for left,right in zip(edges[::2],edges[1::2]):
            ident=len(parent);parent.append(ident)
            while j<len(previous) and previous[j][1]<=left:j+=1
            k=j
            while k<len(previous) and previous[k][0]<right:
                union(ident,previous[k][2]);k+=1
            current.append((int(left),int(right),ident));runs.append((y,int(left),int(right),ident))
        previous=current
    groups={}
    for y,left,right,ident in runs:
        root=find(ident);g=groups.setdefault(root,{'area':0,'box':[left,y,right,y+1],'runs':[]})
        g['area']+=right-left;g['runs'].append((y,left,right));b=g['box'];b[0]=min(b[0],left);b[1]=min(b[1],y);b[2]=max(b[2],right);b[3]=max(b[3],y+1)
    return sorted(groups.values(),key=lambda g:g['area'],reverse=True)

def import_character(name,config):
    source=ROOT/'ArtSource/StrikeSheets'/f'{name}.png';image=Image.open(source).convert('RGBA')
    groups=regions(np.asarray(image)[:,:,3]);bodies=[g for g in groups if g['area']>10000]
    assert len(bodies)==6,(name,'expected six separate whole-body cutouts',[(g['area'],g['box']) for g in bodies])
    bodies=sorted(bodies,key=lambda g:(g['box'][1]+g['box'][3])/2)
    bodies=sorted(bodies[:3],key=lambda g:g['box'][0])+sorted(bodies[3:],key=lambda g:g['box'][0])
    scale=496/max(g['box'][3]-g['box'][1] for g in bodies)
    out=ROOT/'ArtSource/StrikeFrames'/name;out.mkdir(parents=True,exist_ok=True);frames=[]
    for index,g in enumerate(bodies):
        b=g['box'];foot_runs=[r for r in g['runs'] if r[0]>=b[3]-24]
        foot_x=(min(r[1] for r in foot_runs)+max(r[2] for r in foot_runs))/2
        # One complete sprite is cropped and translated. No limbs/items are moved independently.
        mask=np.zeros((image.height,image.width),dtype=bool)
        for y,left,right in g['runs']:mask[y,left:right]=True
        for _ in range(2):
            pad=np.pad(mask,1);mask=pad[1:-1,1:-1]|pad[:-2,1:-1]|pad[2:,1:-1]|pad[1:-1,:-2]|pad[1:-1,2:]
        rgba=np.array(image);rgba[:,:,3]=np.where(mask,rgba[:,:,3],0)
        box=[max(0,b[0]-3),max(0,b[1]-3),min(image.width,b[2]+3),min(image.height,b[3]+3)]
        cut=Image.fromarray(rgba).crop(box);resized=cut.resize((round(cut.width*scale),round(cut.height*scale)),Image.Resampling.LANCZOS)
        x=round(SIZE[0]/2-(foot_x-box[0])*scale);y=round(SIZE[1]-(b[3]-box[1])*scale)
        assert x>=0 and x+resized.width<=SIZE[0],(name,index,'weapon would clip',x,resized.width)
        canvas=Image.new('RGBA',SIZE);canvas.alpha_composite(resized,(x,y));path=out/f'Strike_{index:02}.png';canvas.save(path,optimize=True)
        def point(p):return [round(x+(p[0]-box[0])*scale),round(y+(p[1]-box[1])*scale)]
        grip,tip,head=point(config['grips'][index]),point(config['tips'][index]),point(config['heads'][index])
        for kind,p in [('grip',grip),('tip',tip)]:
            assert 0<=p[0]<SIZE[0] and 0<=p[1]<SIZE[1],(name,index,kind,p)
            assert canvas.getchannel('A').crop((p[0]-8,p[1]-8,p[0]+9,p[1]+9)).getbbox(),(name,index,kind,'anchor misses drawn body/weapon',p)
        frames.append({'phase':PHASES[index],'grip':grip,'tip':tip,'head':head,'source_box':box,'foot_pivot':[SIZE[0]//2,SIZE[1]],'scale':scale})
    return {'character':name,'item':config['item'],'canvas':list(SIZE),'pivot':[.5,0],'ground_line':512,'grip':frames[3]['grip'],'tip':frames[3]['tip'],'frames':frames}

def main():
    layout=json.loads((ROOT/'Docs/strike_sheet_layout.json').read_text(encoding='utf-8'))
    result=[import_character(name,config) for name,config in layout.items()]
    (ROOT/'Docs/character_weapon_anchors.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
    print('Imported',len(result),'whole-body strike sheets, six phases each, shared',SIZE,'canvas and feet pivot.')

if __name__=='__main__':main()
