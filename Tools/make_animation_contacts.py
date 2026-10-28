"""Contact proofs from native framebuffer captures; no game art is altered."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
root=Path(__file__).resolve().parents[1];folder=root/'QA_SCREENSHOTS/ANIMATION/Unity';out=root/'QA_SCREENSHOTS/ANIMATION/NativeContacts';out.mkdir(parents=True,exist_ok=True)
font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',17)
for name in ['AlingPamalo','KuyaTsinelas','AteBatoBatoPik','KapBatuta','ManongLatalata','TitoSilya','AlingBotengSuka','NanayTingting']:
 files=[folder/name/f'Strike_{i:02}.png' for i in range(6)]+[folder/name/f'Hurt_{i:02}.png' for i in range(4)]
 sheet=Image.new('RGB',(1600,760),(18,29,37));d=ImageDraw.Draw(sheet)
 for i,path in enumerate(files):
  x=(i%4)*400;y=(i//4)*250;shot=Image.open(path).crop((210,320,1070,630));shot.thumbnail((398,220));sheet.paste(shot,(x,y+26));d.text((x+6,y+3),name+' '+path.stem,font=font,fill=(245,225,185))
 sheet.save(out/(name+'.png'))
print('Eight native strike/hurt contact proofs created.')
pairs=sorted((folder/'FaceToFace').glob('*.png'))
assert len(pairs)==28
for page in range(4):
 sheet=Image.new('RGB',(1600,460),(18,29,37));d=ImageDraw.Draw(sheet)
 for i,path in enumerate(pairs[page*7:(page+1)*7]):
  x=i%4*400;y=i//4*230;shot=Image.open(path).crop((245,320,1035,630));shot.thumbnail((396,196));sheet.paste(shot,(x,y+29));d.text((x+4,y+2),path.stem,font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',12),fill=(245,225,185))
 sheet.save(out/('Pairs_'+str(page+1)+'.png'))
print('All 28 native pair proofs arranged in four pages.')
