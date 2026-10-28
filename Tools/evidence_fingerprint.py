"""Hash explicit game-source roots, never local reference-photo folders."""
from pathlib import Path
import hashlib

def sha(path):
    digest=hashlib.sha256()
    with Path(path).open('rb') as stream:
        for chunk in iter(lambda:stream.read(1024*1024),b''):digest.update(chunk)
    return digest.hexdigest()

def source_fingerprint(root):
    root=Path(root);rows=[]
    for folder in ['Assets/Scripts','Assets/Resources/Ilaban','Assets/Tests','Assets/Scenes','ProjectSettings','Packages']:
        for path in sorted((root/folder).rglob('*')):
            if path.is_file():rows.append((path.relative_to(root).as_posix(),sha(path)))
    digest=hashlib.sha256()
    for name,value in sorted(rows):digest.update((name+'\0'+value+'\n').encode())
    return {'sha256':digest.hexdigest(),'files':len(rows)}
