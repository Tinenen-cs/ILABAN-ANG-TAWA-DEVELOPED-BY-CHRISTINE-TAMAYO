from pathlib import Path
import json,re,wave,hashlib,numpy as np
root=Path(__file__).resolve().parents[1]
# Scan visible default-Tagalog strings from fresh native captures, not English fallback translations in source.
findings=[];folder=root/'QA_SCREENSHOTS/Final_1280x720'
english=re.compile(r'\b(settings?|back|player|players|choose|movement|keys|game language|reaction style|master volume|music|sound|up|down|select|left|right|change|start|ready|quit|continue|confirm|punch|move)\b',re.I)
for p in sorted(folder.glob('*-ui.txt')):
 if 'english' in p.name.lower():continue
 text=p.read_text(encoding='utf-8-sig').replace('\n',' ')
 text=re.sub(r'ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO',' ',text,flags=re.I)
 for match in english.finditer(text):findings.append({'file':p.relative_to(root).as_posix(),'word':match.group(0)})
# Mojibake glyph guard over authored text, source, and text captures.
mojibake=[]
for base in (root/'Assets/Scripts',root/'Docs',root/'Tools',root/'QA_SCREENSHOTS/Final_1280x720'):
 for p in base.rglob('*'):
  if not p.is_file() or p.suffix.lower() not in {'.cs','.md','.txt','.json','.py'}:continue
  try:text=p.read_text(encoding='utf-8-sig')
  except UnicodeDecodeError:continue
  if any(marker in text for marker in [chr(0x00c3)+chr(0x2018),chr(0x00c3)+chr(0x00b1),chr(0xfffd)]):mojibake.append(p.relative_to(root).as_posix())
# Audio inventory and direct named references used by runtime scripts.
audio_root=root/'Assets/Resources/Ilaban/Audio';clips={p.stem for p in audio_root.rglob('*.wav')}
sources='\n'.join(p.read_text(encoding='utf-8-sig') for p in (root/'Assets/Scripts/Runtime').glob('*.cs'))
referenced=set(re.findall(r'(?:Sound|Music)\(\s*"([A-Za-z0-9_]+)"',sources))|set(re.findall(r'clips\["([A-Za-z0-9_]+)"\]',sources))
missing=sorted(name for name in referenced if name not in clips and not name.endswith('_'))
# All eight impact variants are required by the combat model.
missing += [f'hit_{i}' for i in range(8) if f'hit_{i}' not in clips]
wave_rows=[]
for p in sorted(audio_root.rglob('*.wav')):
 with wave.open(str(p),'rb') as w:
  samples=np.frombuffer(w.readframes(w.getnframes()),dtype='<i2').astype(float)/32768
  peak=float(np.abs(samples).max()) if len(samples) else 0
  wave_rows.append({'file':p.relative_to(root).as_posix(),'samples':len(samples),'peak':peak,'rms':float(np.sqrt(np.mean(samples*samples))) if len(samples) else 0,'clipped':int((np.abs(samples)>=.999).sum())})
report={'tagalog_default_english_candidates':findings,'mojibake_files':mojibake,'audio_files':len(wave_rows),'missing_audio_references':sorted(set(missing)),'clipped_samples':sum(x['clipped'] for x in wave_rows),'audio':wave_rows}
(root/'Docs/FINAL_STATIC_AUDIT.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in report.items() if k!='audio'},ensure_ascii=False,indent=2))
assert not findings,'English candidate found in default Tagalog UI captures'
assert not mojibake,'Mojibake found'
assert not missing,'Missing referenced audio'
assert len(wave_rows)==30 and report['clipped_samples']==0
