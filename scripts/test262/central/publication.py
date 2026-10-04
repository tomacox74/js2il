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


def owned_head(publication,head):
    """True only when the remote commit is exactly this reservation's marker and patch bytes."""
    subprocess.run(['git','fetch','--no-tags','origin',publication['branch_name']],cwd=REPO,check=True,capture_output=True)
    if git('show','-s','--format=%B',head).strip()!=f'test262: native central batch {publication["batch_id"]}':
        return False
    actual=subprocess.check_output(['git','diff','--binary',head+'^',head,'--'],cwd=REPO)
    return hashlib.sha256(actual).hexdigest()==publication['patch_sha256'].removeprefix('\\x')


def recover_reservation(client,repository_name,publication):
    """Resolve a reservation abandoned between branch push and PR number persistence."""
    branch=publication['branch_name']
    remote=subprocess.run(['git','ls-remote','--heads','origin',branch],cwd=REPO,capture_output=True,text=True,check=True).stdout.strip()
    head=remote.split()[0] if remote else None
    prs=gh('pr','list','--repo',repository_name,'--head',branch,'--state','all','--json','number,state,headRefOid')
    key={'publication_id':publication['publication_id']}
    if len(prs)>1 or (prs and (head is None or prs[0]['headRefOid']!=head or not owned_head(publication,head))):
        return client.call('transition','publications',key,publication['version'],
                           {'state':'blocked','closure_reason':'unverifiable PR for abandoned reservation; manual reconciliation required'})
    if prs:
        pr=prs[0]
        if pr['state']=='MERGED':
            patch={'state':'merged','closure_reason':'GitHub confirms merge'}
        elif pr['state']=='CLOSED':
            patch={'state':'closed-deferred','closure_reason':'GitHub confirms closure without merge'}
        else:
            patch={'state':'open'}
        return client.call('transition','publications',key,publication['version'],dict(patch,pr_number=pr['number'],expected_head=head))
    if head is not None:
        if not owned_head(publication,head):
            return client.call('transition','publications',key,publication['version'],
                               {'state':'blocked','closure_reason':'foreign branch occupies reserved publication name'})
        subprocess.run(['git','push','origin','--delete',branch],cwd=REPO,check=True)
    return client.call('transition','publications',key,publication['version'],
                       {'state':'cancelled','closure_reason':'reclaimed reservation abandoned before PR creation'})


def reconcile_publications(client,repository_name,current=None):
    publications=[p for state in ('reserved','open','updating') for p in client.read('publications',{'state':state})]
    for publication in publications:
        if publication['publication_id']==current:
            continue
        if not publication['pr_number']:
            recover_reservation(client,repository_name,publication)
            continue
        pr=gh('api',f'repos/{repository_name}/pulls/{publication["pr_number"]}')
        patch=None
        if pr.get('merged'):
            patch={'state':'merged','closure_reason':'GitHub confirms merge'}
        elif pr['state']=='closed':
            patch={'state':'closed-deferred','closure_reason':'GitHub confirms closure without merge'}
        elif publication['expected_head'] and pr['head']['sha']!=publication['expected_head']:
            patch={'state':'ownership-lost','closure_reason':'GitHub PR head differs from reserved head'}
        if patch:
            client.call('transition','publications',{'publication_id':publication['publication_id']},publication['version'],patch)
            continue
        head=pr['head']['sha']
        checks=gh('api','--paginate','--slurp',f'repos/{repository_name}/commits/{head}/check-runs?per_page=100')
        rows=[]
        for page in checks:
            for check in page['check_runs']:
                if check['head_sha']!=head or check['status']!='completed':
                    continue
                conclusion=check['conclusion']
                normalized='success' if conclusion=='success' else ('cancelled' if conclusion=='cancelled' else 'failure')
                rows.append({'publication_id':publication['publication_id'],'check_identity':'check:'+check['app']['slug']+':'+check['name'],
                             'external_run_key':str(check['id'])+':'+str(check['completed_at']),'head_revision':head,'conclusion':normalized})
        client.put('publication_checks',rows)


def publish(client,args):
    batch=json.loads(Path(args.batch_file).read_text());context=batch['context']
    revision=context['revision'];batch_id=batch['batch']
    pubid=identity(args.repository,'publication',batch_id)
    # A rerun of this batch resumes its own reservation; any other abandoned one is reclaimed.
    reconcile_publications(client,args.repository_name,pubid)
    if git('rev-parse','origin/master')!=revision or git('rev-parse','HEAD')!=revision:
        raise ValueError('Publication target has advanced; reconcile and refresh acceptance first')
    patch=Path(args.patch).read_bytes();manifest=Path(args.manifest).read_bytes()
    branch='test262/native-central-'+batch_id
    accepted=client.one('native_batches',batch_id=batch_id)
    if accepted is None or accepted['state']!='sealed':
        raise ValueError('Publication acceptance is stale or conflicted')
    existing=client.one('publications',publication_id=pubid)
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
    current=client.one('publications',publication_id=pubid)
    client.call('transition','publications',{'publication_id':pubid},current['version'],{'state':'open','pr_number':number,'expected_head':head})
    return {'publication_id':pubid,'pr_number':number,'head':head,'requires_independent_pr_ci':True}
