"""Remove only stale test-only subassets shipped in the Unity template profile."""
from pathlib import Path
import re
target=Path('Assets/Settings/DefaultVolumeProfile.asset')
text=target.read_text(encoding='utf-8')
missing={'60f3b30c03e6ba64d9a27dc9dba8f28d','0fd9ee276a1023e439cf7a9c393195fa','5a00a63fdd6bd2a45ab1f2d869305ffd','74955a4b0b4243bc87231e8b59ed9140'}
parts=re.split(r'(?=^--- !u!)',text,flags=re.M)
removed=[];kept=[]
for block in parts:
    stale=any('guid: '+g in block for g in missing)
    stale|=('m_Script: {fileID: 0}' in block and ('Tests' in block or 'TestComponent' in block))
    if stale:
        match=re.match(r'--- !u!\d+ &(-?\d+)',block)
        if not match: raise RuntimeError('Missing subasset identifier')
        removed.append(match.group(1))
    else: kept.append(block)
if removed:
    backup=Path('Docs/backups/DefaultVolumeProfile.before-cleanup.txt');backup.parent.mkdir(parents=True,exist_ok=True)
    if not backup.exists(): backup.write_text(text,encoding='utf-8')
    clean=''.join(kept)
    for file_id in removed: clean=re.sub(r'^  - \{fileID: '+re.escape(file_id)+r'\}\n','',clean,flags=re.M)
    target.write_text(clean,encoding='utf-8')
print('Removed stale template test subassets:',len(removed))
