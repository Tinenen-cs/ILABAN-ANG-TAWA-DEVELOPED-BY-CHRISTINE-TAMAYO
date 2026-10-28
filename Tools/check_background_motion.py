from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np, subprocess,json,datetime
r=Path(__file__).resolve().parents[1];q=r/'QA_SCREENSHOTS/UPDATE3/Motion';q.mkdir(parents=True,exist_ok=True);log=r/'Logs/UPDATE3-Motion.log'
(r/'RESUME_STATE.txt').write_text('HEAD='+subprocess.check_output(['git','rev-parse','HEAD'],cwd=r,text=True).strip()+' | UPDATE3: about to capture five stationary motion pairs; verify thresholds and floor | UTC='+datetime.datetime.now(datetime.timezone.utc).isoformat(),encoding='utf-8')
p=subprocess.run([str(r/'Builds/Windows/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe'),'-screen-fullscreen','0','-screen-width','1280','-screen-height','720','-ilaban-motion',str(q),'-logFile',str(log)],timeout=65)
issues=[s for s in log.read_text(encoding='utf-8',errors='replace').splitlines() if any(t in s for t in ['Exception','NullReference','MISSING GLYPH','error CS'])]
assert p.returncode==0 and (q/'PASS.txt').exists() and not issues,(p.returncode,issues)
rows=[];contact=Image.new('RGB',(1280,5*380));d=ImageDraw.Draw(contact)
for i,name in enumerate(['Palengke','Looban','Plaza','Title','StageSelect']):
 a=np.asarray(Image.open(q/f'{i}-a.png').convert('RGB')).astype(float);b=np.asarray(Image.open(q/f'{i}-b.png').convert('RGB')).astype(float);assert a.shape==(720,1280,3)
 diff=np.abs(b-a);mask=np.zeros((720,1280),bool)
 if i<3:mask[140:332,:]=True
 elif i==3:mask[:76,:]=True;mask[140:600,:250]=True;mask[140:600,1030:]=True
 else:mask[:140,:]=True;mask[140:620,:80]=True;mask[140:620,1200:]=True
 mean=float(diff[mask].mean());changed=float((diff[mask].max(axis=1)>2).mean());floor=float(diff[630:650,:].mean())
 passed=.02<mean<6 and .001<changed<.4 and floor<.001
 rows.append(dict(scene=name,interval_seconds=1.5,background_mean_absolute_rgb=mean,changed_fraction_over_2=changed,floor_mean_absolute_rgb=floor,passed=passed))
 for j in range(2):
  pic=Image.fromarray((a if j==0 else b).astype('uint8')).resize((640,360));contact.paste(pic,(j*640,i*380+20))
 d.text((5,i*380),f'{name} A / B: MAD {mean:.4f}, changed {changed:.2%}, floor {floor:.4f}',fill='white')
 Image.fromarray(np.clip(diff*12,0,255).astype('uint8')).save(q/f'{i}-difference-x12.png')
contact.save(q/'contact.png')
text='Native1280x720 framebuffer pairs, stationary fighters/UI, real-time1.5s interval.\nThresholds set before captures: 0.02 < background mean absolute RGB difference <6/255; 0.1% < pixels changed by >2 <40%; floor difference <0.001.\n'+ '\n'.join(json.dumps(row) for row in rows)+'\n'
(r/'QA_SCREENSHOTS/UPDATE3/motion_check.txt').write_text(text,encoding='utf-8');(r/'Docs/background_motion_check.json').write_text(json.dumps(rows,indent=2),encoding='utf-8');print(text)
assert all(x['passed'] for x in rows),'Motion visibility/stability/floor threshold failed'
