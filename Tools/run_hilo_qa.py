from pathlib import Path
from PIL import Image,ImageDraw
import subprocess,json,hashlib,datetime
root=Path(__file__).resolve().parents[1];folder=root/'QA_SCREENSHOTS/HILO_VICTIM';folder.mkdir(parents=True,exist_ok=True)
head=subprocess.check_output(['git','rev-parse','HEAD'],cwd=root,text=True).strip()
(root/'RESUME_STATE.txt').write_text(f'HEAD={head} | Hilo victim display: about to capture actual hits1-5, recovery/break, both sides/all3 maps | UTC={datetime.datetime.now(datetime.timezone.utc).isoformat()}\n',encoding='utf-8')
log=root/'Logs/Hilo-Victim-Capture.log';exe=root/'Builds/Windows/ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO.exe'
process=subprocess.run([str(exe),'-screen-fullscreen','0','-screen-width','1280','-screen-height','720','-ilaban-hilo',str(folder),'-logFile',str(log)],timeout=90)
issues=[line for line in log.read_text(encoding='utf-8',errors='replace').splitlines() if any(x in line for x in ['Exception','NullReference','MISSING GLYPH','LAYOUT TEXT OVERFLOW','error CS'])]
assert process.returncode==0 and (folder/'PASS.txt').exists() and not issues,(process.returncode,issues)
rows=json.loads((folder/'states.json').read_text(encoding='utf-8'))['stages'];assert len(rows)==48
for row in rows:
 expected=0
 if '-hit' in row['file']:expected=int(row['file'].rsplit('-hit',1)[1].split('.')[0])
 assert row['streak']==expected
 assert row['leftFilled']==(expected if row['victim']==0 else 0)
 assert row['rightFilled']==(expected if row['victim']==1 else 0)
 assert (row['victimDizzy']>0)==(expected==5)
 path=folder/row['file'];assert Image.open(path).size==(1280,720)
 row['sha256']=hashlib.sha256(path.read_bytes()).hexdigest()
for map in range(3):
 for attacker in range(2):
  sheet=Image.new('RGB',(1920,920),(25,30,34));draw=ImageDraw.Draw(sheet)
  stages=['empty','hit1','hit2','hit3','hit4','hit5','recovered','streak-broken']
  for i,stage in enumerate(stages):
   path=folder/f'Map{map}-Attacker{attacker+1}-{stage}.png';pic=Image.open(path).convert('RGB').resize((640,360));x=i%3*640;y=i//3*306
   # Contact previews only; the original framebuffer files retain1280x720.
   pic.thumbnail((530,280));sheet.paste(pic,(x,y+22));draw.text((x+4,y+4),f'Map{map} attackerP{attacker+1} / victimP{2-attacker}: {stage}',fill='white')
  sheet.save(folder/f'Map{map}-Attacker{attacker+1}-contact.png')
model_path='Assets/Scripts/Runtime/DuelModel.cs'
before=subprocess.check_output(['git','rev-parse','2fcbfa9:'+model_path],cwd=root,text=True).strip();after=subprocess.check_output(['git','hash-object',model_path],cwd=root,text=True).strip();assert before==after,'Combat model must remain unchanged'
report=dict(passed=True,exit=process.returncode,native_captures=48,maps=3,both_attackers=True,model_git_blob_before=before,model_git_blob_after=after,issues=issues,states=rows)
(root/'Docs/QA_EVIDENCE/hilo_victim_capture.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print('PASS:48 native captures, actual victim-side fills1-5, same victim dizzy, break/recovery resets, all3maps/both attackers; DuelModel unchanged.')
