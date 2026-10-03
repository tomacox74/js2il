"""Reconcile GitHub side effects with a central publication reservation. Never merges."""
import hashlib
import json
import os
from pathlib import Path
import subprocess
from .client import bytea, identity, sha
from .commands import git, REPO


def gh(*args):
    return json.loads(subprocess.check_output(['gh',*args],text=True))


def publish(client,args):
    batch=json.loads(Path(args.batch_file).read_text());context=batch['context']
    revision=context['revision'];batch_id=batch['batch']
    if git('rev-parse','origin/master')!=revision or git('rev-parse','HEAD')!=revision:
        raise ValueError('Publication target has advanced; reconcile and refresh acceptance first')
    patch=Path(args.patch).read_bytes();manifest=Path(args.manifest).read_bytes()
    pubid=identity(args.repository,'publication',batch_id)
    branch='test262/native-central-'+batch_id
    tables=client.snapshot()['tables']
    accepted=next(b for b in tables['native_batches'] if b['batch_id']==batch_id)
    if accepted['state']!='sealed':
        raise ValueError('Publication acceptance is stale or conflicted')
    existing=next((p for p in tables['publications'] if p['publication_id']==pubid),None)
    patchsha=hashlib.sha256(patch).hexdigest();manifestsha=hashlib.sha256(manifest).hexdigest()
    record={'publication_id':pubid,'repository_id':args.repository,'batch_id':batch_id,'branch_name':branch,
            'patch_sha256':bytea(patchsha),'manifest_sha256':bytea(manifestsha)}
    client.put('publications',[record])
    if existing and existing['state'] not in ('reserved','open','updating'):
        raise ValueError('Publication is closed or ownership lost; create a reviewed new batch')
    # Reservation is committed before any GitHub side effect. A crash can reconcile branch/PR by stable name.
    remote=subprocess.run(['git','ls-remote','--heads','origin',branch],cwd=REPO,capture_output=True,text=True,check=True).stdout.strip()
    if remote:
        head=remote.split()[0]
        if existing and existing.get('expected_head') and existing['expected_head']!=head:
            client.call('transition','publications',{'publication_id':pubid},existing['version'],{'state':'ownership-lost','closure_reason':'remote head changed'})
            raise ValueError('Publication branch ownership lost')
        subprocess.run(['git','fetch','origin',branch],cwd=REPO,check=True)
        # Unknown crash-after-push is adopted only if parent, patch bytes and commit marker match.
        if git('rev-parse',head+'^')!=revision or git('show','-s','--format=%B',head).strip()!=f'test262: native central batch {batch_id}':
            raise ValueError('Remote branch does not belong to this publication')
        actual=subprocess.check_output(['git','diff','--binary',revision,head,'--'],cwd=REPO)
        if hashlib.sha256(actual).hexdigest()!=patchsha:
            raise ValueError('Remote branch patch differs from reserved patch')
    else:
        subprocess.run(['git','checkout','-b',branch,revision],cwd=REPO,check=True)
        subprocess.run(['git','apply','--index',args.patch],cwd=REPO,check=True)
        subprocess.run(['git','-c','user.name=Test262 Catalogue','-c','user.email=test262-catalogue@users.noreply.github.com','commit','-m',f'test262: native central batch {batch_id}'],cwd=REPO,check=True)
        head=git('rev-parse','HEAD')
        subprocess.run(['git','push','origin','HEAD:refs/heads/'+branch],cwd=REPO,check=True)
    prs=gh('pr','list','--repo',args.repository_name,'--head',branch,'--state','all','--json','number,state,headRefOid')
    if len(prs)>1:
        raise ValueError('Multiple PRs for central publication; manual reconciliation required')
    if prs:
        pr=prs[0]
        if pr['state']!='OPEN' or pr['headRefOid']!=head:
            raise ValueError('PR state/head changed; do not create a duplicate')
        number=pr['number']
    else:
        url=subprocess.check_output(['gh','pr','create','--repo',args.repository_name,'--base','master','--head',branch,
                                      '--title',f'test262: native batch {batch_id}', '--body-file',args.body],text=True).strip()
        number=int(url.rsplit('/',1)[-1])
    current=next(p for p in client.snapshot()['tables']['publications'] if p['publication_id']==pubid)
    client.call('transition','publications',{'publication_id':pubid},current['version'],{'state':'open','pr_number':number,'expected_head':head})
    return {'publication_id':pubid,'pr_number':number,'head':head,'requires_independent_pr_ci':True}
