"""Repeatable continuous whole-image proportion correction. Never draw separate body parts.
Head landmarks: crown from authored anchors, chin from largest upper skin component (+6px outline).
Body height: crown to the shared y512 sole line, excluding a raised weapon/ponytail/effects.
"""
from pathlib import Path
from PIL import Image, ImageDraw
import numpy as np, json, shutil, subprocess, sys, hashlib
from import_strike_sheets import regions
R=Path(__file__).resolve().parents[1]; TARGET=.43
original=R/'ArtSource/ProportionOriginals'; original.mkdir(exist_ok=True)
for name in ['character_weapon_anchors.json','character_action_poses.json','head_measurements_before.json']:
    if not (original/name).exists():shutil.copy2(R/'Docs'/name,original/name)
measure=json.loads((original/'head_measurements_before.json').read_text(encoding='utf-8'))
weapon=json.loads((original/'character_weapon_anchors.json').read_text(encoding='utf-8'))
poses=json.loads((original/'character_action_poses.json').read_text(encoding='utf-8'))
report=[]; mapping={}
for row in measure:
    path=R/row['file']; backup=original/row['file'];backup.parent.mkdir(parents=True,exist_ok=True)
    if not backup.exists():shutil.copy2(path,backup)
    im=Image.open(backup).convert('RGBA'); top=row['head'][1]; neck=row['neck']; newneck=top+TARGET*(512-top)
    # The three revised characters need correction in every pose; the balanced five
    # change only when outside the target's 2.5 percentage-point safety band.
    change=row['character'] in ['TitoSilya','AlingBotengSuka','NanayTingting'] or abs(row['ratio']-TARGET)>.025
    if not change:newneck=neck
    def warp_y(y):
        return top+(y-top)*(newneck-top)/(neck-top) if y<=neck else newneck+(y-neck)*(512-newneck)/(512-neck)
    def width_scale(y):
        factor=min(1,(newneck-top)/(neck-top))
        return factor+(1-factor)*max(0,min(1,(y-neck)/55))
    def point(p):return [round(row['head'][0]+(p[0]-row['head'][0])*width_scale(p[1])),round(warp_y(p[1]))]
    # One continuous mesh for every pixel including the baked-in item; no detached
    # head/torso images, no moving child transforms, same feet and ground line.
    if change:
        breaks=sorted(set([0,top,neck,min(511,neck+55),512]+list(range(0,513,8))))
        mesh=[];cx=row['head'][0]
        for a,b in zip(breaks,breaks[1:]):
            ya,yb=round(warp_y(a)),round(warp_y(b))
            if yb<=ya:continue
            sa,sb=width_scale(a),width_scale(b)
            mesh.append(((0,ya,896,yb),(cx-cx/sa,a,cx-cx/sb,b,cx+(896-cx)/sb,b,cx+(896-cx)/sa,a)))
        im=im.transform((896,512),Image.Transform.MESH,mesh,Image.Resampling.BICUBIC)
    im.save(path,optimize=True)
    mapped_head=point(row['head']); mapped_neck=round(warp_y(neck));ratio=(mapped_neck-mapped_head[1])/(512-mapped_head[1])
    assert abs(ratio-TARGET)<=.03,(row['character'],row['pose'],ratio)
    report.append({**row,'before_ratio':row['ratio'],'ratio':ratio,'head':mapped_head,'neck':mapped_neck,'changed':change})
    mapping[(row['character'],row['pose'])]=dict(head=mapped_head,neck=mapped_neck,ratio=ratio)
    if row['pose'].startswith('Strike_'):
        w=next(x for x in weapon if x['character']==row['character']);f=w['frames'][int(row['pose'][-2:])]
        for key in ['head','grip','tip']:f[key]=point(f[key])
        w['grip']=w['frames'][3]['grip'];w['tip']=w['frames'][3]['tip']
    else:next(x for x in poses if x['character']==row['character'] and x['pose']==row['pose'])['head']=mapped_head
(R/'Docs/character_weapon_anchors.json').write_text(json.dumps(weapon,indent=2),encoding='utf-8')
(R/'Docs/character_action_poses.json').write_text(json.dumps(poses,indent=2),encoding='utf-8')
subprocess.run([sys.executable,str(R/'Tools/generate_character_action_frames.py')],check=True)
runtime=[]
for f in json.loads((R/'Docs/character_action_frames.json').read_text(encoding='utf-8')):
    name=Path(f['file']).parts[-3];pose=Path(f['pose_source']).stem;land=mapping[(name,pose)]
    p=R/f['file'];runtime.append(dict(resource=f['file'].split('Assets/Resources/')[1][:-4],ratio=land['ratio'],sha256=hashlib.sha256(p.read_bytes()).hexdigest()))
(R/'Assets/Resources/Ilaban/Characters/HeadRatios.json').write_text(json.dumps({'target':TARGET,'frames':runtime},indent=2),encoding='utf-8')
(R/'Docs/head_measurements_after.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
names=[w['character'] for w in weapon];lineup=Image.new('RGB',(1920,330),(28,38,43));d=ImageDraw.Draw(lineup)
table=['# Measured head proportions - UPDATE3','','Target: 43% of crown-to-sole height, tolerance 3 percentage points. Crown uses authored head-top anchors; chin/neck boundary uses the largest upper warm-skin component plus a 6px outline allowance. Excludes items raised above the crown, trailing hair and visual effects. This is an explicit reproducible landmark measurement, not an unlabelled claim of anatomical segmentation. Original source frames/landmarks are preserved in ArtSource/ProportionOriginals.','','Continuous whole-image mesh correction keeps the canvas 896x512, feet pivot (448,512), ground line and baked-in weapons. No runtime parts rig. Portraits are cropped from the corrected ready pose; runtime previews use the same corrected idle frames.','','| Character | Ready before | Ready after | All 12 source poses after |','|---|---:|---:|---:|']
for i,n in enumerate(names):
    row=next(x for x in report if x['character']==n and x['pose']=='Strike_00');rr=[x['ratio'] for x in report if x['character']==n]
    table.append(f'| {n} | {row["before_ratio"]:.1%} | {row["ratio"]:.1%} | {min(rr):.1%}–{max(rr):.1%} |')
    im=Image.open(R/f'ArtSource/StrikeFrames/{n}/Strike_00.png').convert('RGBA');im.save(R/f'Assets/Resources/Ilaban/Characters/{n}/Body.png')
    head=row['head'];side=row['neck']-head[1]+28
    im.crop((round(head[0]-side/2),max(0,head[1]-8),round(head[0]+side/2),max(0,head[1]-8)+side)).resize((256,256),Image.Resampling.LANCZOS).save(R/f'Assets/Resources/Ilaban/Characters/{n}/Portrait.png')
    # Identical pixel scale for all eight, never fit each crop to a different width.
    spr=im.resize((448,256),Image.Resampling.LANCZOS)
    lineup.paste(spr,(i*240+120-224,30),spr);d.text((i*240+5,300),n,fill='white')
(R/'Docs/HEAD_RATIO.md').write_text('\n'.join(table)+'\n\nAll 272 runtime frames are bound by SHA-256 to the measured source-pose ratio in HeadRatios.json. Run `python Tools/test_head_ratios.py` after art changes; rerun this importer after reimporting source sheets.\n',encoding='utf-8')
q=R/'QA_SCREENSHOTS/UPDATE3';q.mkdir(parents=True,exist_ok=True);lineup.save(q/'heads-after.png')
print('Corrected/measured 96 source poses; generated 272 runtime frames, portraits and lineup.')
