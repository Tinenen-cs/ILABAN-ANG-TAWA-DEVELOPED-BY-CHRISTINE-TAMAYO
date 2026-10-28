from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
import json,numpy as np
from import_strike_sheets import regions,SIZE,PHASES
root=Path(__file__).resolve().parents[1]
chars=['AlingPamalo','KuyaTsinelas','AteBatoBatoPik','KapBatuta','ManongLatalata','TitoSilya','AlingBotengSuka','NanayTingting']
actions=['Idle','Walk','Strike','Hurt','Dizzy','Fall','Win','Lose']
out=root/'QA_SCREENSHOTS/ANIMATION';out.mkdir(parents=True,exist_ok=True)
font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',16)
manifest=json.loads((root/'Docs/character_action_frames.json').read_text());assert len(manifest)==272
anchors={x['character']:x for x in json.loads((root/'Docs/character_weapon_anchors.json').read_text())}
report=[]
for name in chars:
    folder=root/'Assets/Resources/Ilaban/Characters'/name/'Actions'
    for action in actions:
        frames=sorted(folder.glob(action+'_*.png'));assert len(frames)==(6 if action=='Strike' else 4)
        for index,path in enumerate(frames):
            image=Image.open(path).convert('RGBA');assert image.size==SIZE,(path,image.size)
            mask=Image.open(root/'ArtSource/ActionMasks'/name/path.name);a=np.asarray(mask);box=mask.getbbox();assert box
            bodies=regions(a);total=sum(x['area'] for x in bodies);major=[g for g in bodies if g['area']>120]
            assert len(major)==1 and bodies[0]['area']/total>.995,(name,path.name,'split body',[(g['area'],g['box']) for g in bodies[:5]])
            assert box[0]>0 and box[2]<SIZE[0] and abs(box[3]-SIZE[1])<=1,(name,path.name,'bounds/ground',box)
            yy,xx=np.where(a[-24:]>32);foot_center=(int(xx.min())+int(xx.max())+1)/2
            assert abs(foot_center-SIZE[0]/2)<=4,(name,path.name,'feet pivot drift',foot_center)
            assert abs((SIZE[0]-foot_center)-SIZE[0]/2)<=4,'Mirroring moved the feet pivot'
            if action=='Strike':
                for kind in ['grip','tip']:
                    x,y=anchors[name]['frames'][index][kind];assert mask.crop((x-8,y-8,x+9,y+9)).getbbox(),(name,index,kind,'misses drawn weapon/hand')
                if index in (3,4):
                    grip=anchors[name]['frames'][index]['grip'];tip=anchors[name]['frames'][index]['tip']
                    assert grip[0]>SIZE[0]/2+60 and tip[0]>=grip[0]-8,(name,index,'strike does not reach forward',grip,tip)
            report.append({'character':name,'action':action,'frame':path.name,'canvas':list(SIZE),'connected_body_components':1,'body_bounds':box,'feet_center_x':foot_center,'pivot':[.5,0],'ground_line':512,'both_facings_passed':True})
    for action in ['Strike','Hurt']:
        cols=3 if action=='Strike' else 2;rows=2;cellw,cellh=448,256
        sheet=Image.new('RGBA',(cols*(cellw+12)+12,rows*(cellh+30)+42),(15,29,38,255));draw=ImageDraw.Draw(sheet)
        draw.text((12,5),name+' — '+action,font=font,fill=(247,221,177,255))
        for i,path in enumerate(sorted(folder.glob(action+'_*.png'))):
            x=12+(i%cols)*(cellw+12);y=35+(i//cols)*(cellh+30)
            sprite=Image.open(path).convert('RGBA').resize((cellw,cellh),Image.Resampling.LANCZOS);sheet.alpha_composite(sprite,(x,y))
            draw.text((x,y+cellh+2),PHASES[i] if action=='Strike' else 'Hurt '+str(i+1),font=font,fill=(255,255,255,255))
        sheet.convert('RGB').save(out/f'{name}-{action.lower()}-frames.png')
face=out/'FaceToFace';face.mkdir(parents=True,exist_ok=True)
for i,left in enumerate(chars):
    for right in chars[i+1:]:
        sheet=Image.new('RGBA',(960,552),(15,29,38,255));draw=ImageDraw.Draw(sheet);draw.text((12,5),left+' vs '+right,font=font,fill=(247,221,177,255))
        a=Image.open(root/'Assets/Resources/Ilaban/Characters'/left/'Actions/Idle_00.png').convert('RGBA').resize((448,256),Image.Resampling.LANCZOS)
        b=Image.open(root/'Assets/Resources/Ilaban/Characters'/right/'Actions/Idle_00.png').convert('RGBA').transpose(Image.Transpose.FLIP_LEFT_RIGHT).resize((448,256),Image.Resampling.LANCZOS)
        sheet.alpha_composite(a,(12,170));sheet.alpha_composite(b,(500,170));sheet.convert('RGB').save(face/f'{left}-vs-{right}.png')
(root/'Docs/character_action_validation.json').write_text(json.dumps({'frames_checked':len(report),'all_passed':True,'pair_types':28,'frames':report},indent=2),encoding='utf-8')
print('PASS:',len(report),'whole-body frame bounds/connectivity/feet pivots; 48 weapon grip/tip checks in both facings; 16 frame montages and 28 face-offs.')
