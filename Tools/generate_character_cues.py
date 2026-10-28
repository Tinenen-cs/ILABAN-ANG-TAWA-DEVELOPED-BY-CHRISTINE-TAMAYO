"""Original short item/voice-like cues; no recordings, keys or network calls."""
from pathlib import Path
import json,wave,numpy as np
root=Path(__file__).resolve().parents[1];out=root/'Assets/Resources/Ilaban/Audio/SFX';rate=22050
names=['Pamalo','Tsinelas','Bato','Batuta','Lata','Silya','Bote','Walis tingting'];rows=[]
for i,item in enumerate(names):
    t=np.arange(int(rate*.18))/rate;rng=np.random.default_rng(731+i);noise=rng.normal(0,1,len(t))
    f=[170,260,110,310,820,190,1120,150][i]
    tone=np.sin(2*np.pi*(f*t+18*t*t))*np.exp(-t*26)
    if i in (4,6):tone+=.4*np.sin(2*np.pi*f*2.71*t)*np.exp(-t*34)
    if i in (1,7):tone=.15*tone+noise*.2*np.exp(-t*(36 if i==1 else 20))
    if i==5:tone+=np.roll(tone,int(rate*.026))*.35
    voice=np.sin(2*np.pi*(320+16*i)*t+np.sin(2*np.pi*8*t)*.45)*np.exp(-t*23)*.22
    signal=(tone+voice)*np.minimum(1,t/.004);signal[-220:]*=np.linspace(1,0,220);signal-=signal.mean();signal*=.28/(np.abs(signal).max()+1e-9)
    path=out/f'reaction_{i}.wav'
    with wave.open(str(path),'wb') as w:w.setnchannels(1);w.setsampwidth(2);w.setframerate(rate);w.writeframes((signal*32767).astype('<i2').tobytes())
    rows.append({'file':path.relative_to(root).as_posix(),'title':item+' reaction','author':'Original procedural synthesis for Christine Tamayo','source':'Tools/generate_character_cues.py','license':'CC0-1.0','description':'Soft item-themed tink and original voice-like blip; no words or recordings. Queued after menu cues.','rms':float(np.sqrt(np.mean(signal*signal))),'peak':float(np.abs(signal).max()),'seconds':.18,'loop':False})
manifest=root/'Docs/ilaban_audio_manifest.json';old=json.loads(manifest.read_text(encoding='utf-8-sig'));old=[r for r in old if '/reaction_' not in r['file']];manifest.write_text(json.dumps(old+rows,indent=2),encoding='utf-8')
(root/'Docs/character_cue_manifest.json').write_text(json.dumps(rows,indent=2),encoding='utf-8');print('Generated eight original 180 ms character cues, peak 0.28; no external recordings.')
