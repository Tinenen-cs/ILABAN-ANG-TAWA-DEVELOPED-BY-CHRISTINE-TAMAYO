from pathlib import Path
from PIL import Image
root=Path('.');im=Image.open(root/'ArtSource/UI/PaintedHeadings.png').convert('RGBA')
for name,top,bottom in [('Logo',0,290),('Controls',291,479),('Characters',480,646),('Stages',647,814),('Win',815,1024)]:
 cut=im.crop((0,top,im.width,bottom));box=cut.getchannel('A').getbbox();assert box;cut=cut.crop(box);cut.save(root/'Assets/Resources/Ilaban/UI'/('Heading'+name+'.png'),optimize=True)
print('Sliced five alpha-transparent original heading signs.')
