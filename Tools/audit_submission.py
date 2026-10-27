from pathlib import Path
import json,re,wave,numpy as np
root=Path('.')
findings=[]
# Scan active user-facing literal calls plus every reaction line; technical identifiers excluded.
words=re.compile(r'\b(health|stamina|timer|knockout|choose|select your|player [12]|press any|keyboard|controller|special|guard|kick|wins|round draw|match draw|back to|continue|sound off|sound on|begin)\b',re.I)
for p in [root/'Assets/Scripts/Runtime/AcademyGame.cs',root/'Assets/Scripts/Runtime/OpeningSequence.cs',root/'Assets/Resources/ReaksyonTeksto.json']:
 for no,line in enumerate(p.read_text(encoding='utf-8-sig').splitlines(),1):
  for literal in re.findall(r'"((?:[^"\\]|\\.)*)"',line):
   if words.search(literal) and not any(x in literal for x in ['Screen ','Input/','Ilaban/']):findings.append(dict(file=p.as_posix(),line=no,text=literal))
clips=[]
for p in sorted((root/'Assets/Resources/Ilaban/Audio').rglob('*.wav')):
 with wave.open(str(p),'rb') as w:
  samples=np.frombuffer(w.readframes(w.getnframes()),dtype='<i2').astype(float)/32768
  clips.append(dict(file=p.as_posix(),rms=float(np.sqrt(np.mean(samples*samples))),peak=float(np.abs(samples).max()),clipped_samples=int((np.abs(samples)>=.999).sum())))
report=dict(ui_english_candidates=findings,audio_files=len(clips),audio=clips,notes=['Automated English keyword scan is heuristic; actual player-visible strings also dumped by portable QA.','Required developer title is the sole English phrase allowed. Hardware key labels are technical.','Real hardware, clean-PC and listening verification remain manual.'])
(root/'Docs/FINAL_STATIC_AUDIT.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print('English candidates:',len(findings),'Audio:',len(clips),'Clipped samples:',sum(x['clipped_samples'] for x in clips))
for x in findings:print(x)
