"""Reconcile GitHub side effects with a central publication reservation. Never merges."""
import hashlib
import json
from pathlib import Path
import subprocess
from .client import bytea, identity
from .commands import git, REPO


def gh(*args):
    return json.loads(subprocess.check_output(['gh',*args],text=True))


def publication_base(client,publication):
    batch=client.one('native_batches',batch_id=publication['batch_id'])
    validation=client.one('validation_events',validation_id=batch['validation_id']) if batch else None
    if not validation or validation['conclusion']!='success':
        raise ValueError('Publication has no verified target revision')
    return validation['target_revision']


def pull_scope(pr,repository_name):
    return (pr.get('baseRefName')=='master' and
            (pr.get('headRepository') or {}).get('nameWithOwner')==repository_name)


def list_pulls(repository_name,branch):
    return gh('pr','list','--repo',repository_name,'--head',branch,'--state','all',
              '--json','number,state,headRefOid,baseRefName,headRepository')


def owned_head(client,publication,head,pull_number=None):
    """Verify the batch's exact parent, commit marker and patch, including deleted branches."""
    ref=f'refs/pull/{pull_number}/head' if pull_number is not None else publication['branch_name']
    try:
        subprocess.run(['git','fetch','--no-tags','origin',ref],cwd=REPO,check=True,capture_output=True)
        if git('show','-s','--format=%P',head)!=publication_base(client,publication):
            return False
        if git('show','-s','--format=%B',head).strip()!=f'test262: native central batch {publication["batch_id"]}':
            return False
        actual=subprocess.check_output(['git','diff','--binary',head+'^',head,'--'],cwd=REPO)
        return hashlib.sha256(actual).hexdigest()==publication['patch_sha256'].removeprefix('\\x')
    except (subprocess.CalledProcessError,ValueError):
        return False


def recover_reservation(client,repository_name,publication):
    """Adopt proven PRs; keep unconfirmed reservations fenced for their original retry."""
    branch=publication['branch_name']
    remote=subprocess.run(['git','ls-remote','--heads','origin',branch],cwd=REPO,capture_output=True,text=True,check=True).stdout.strip()
    head=remote.split()[0] if remote else None
    prs=list_pulls(repository_name,branch)
    key={'publication_id':publication['publication_id']}
    if len(prs)>1:
        return client.call('transition','publications',key,publication['version'],
                           {'state':'blocked','closure_reason':'multiple PRs for reservation; manual reconciliation required'})
    if prs:
        pr=prs[0];pr_head=pr['headRefOid']
        if (not pull_scope(pr,repository_name) or
                (publication.get('expected_head') and publication['expected_head']!=pr_head) or
                (head is not None and head!=pr_head) or
                (pr['state']=='OPEN' and head is None) or
                not owned_head(client,publication,pr_head,pr['number'])):
            return client.call('transition','publications',key,publication['version'],
                               {'state':'blocked','closure_reason':'unverifiable PR for reservation; manual reconciliation required'})
        if pr['state']=='MERGED':
            update={'state':'merged','closure_reason':'GitHub confirms merge'}
        elif pr['state']=='CLOSED':
            update={'state':'closed-deferred','closure_reason':'GitHub confirms closure without merge'}
        elif pr['state']=='OPEN':
            update={'state':'open'}
        else:
            raise ValueError('Unrecognized publication PR state')
        return client.call('transition','publications',key,publication['version'],
                           dict(update,pr_number=pr['number'],expected_head=pr_head))
    if head is not None and not owned_head(client,publication,head):
        return client.call('transition','publications',key,publication['version'],
                           {'state':'blocked','closure_reason':'foreign branch occupies reserved publication name'})
    if publication.get('pr_number'):
        raise ValueError('Recorded publication PR is missing; manual reconciliation required')
    # GitHub and the catalogue cannot be committed atomically. No PR visible does
    # not prove another publisher stopped; deleting/cancelling here can race it.
    # Keep this reservation (and its single-active slot) for a same-batch retry.
    return {'state':publication['state'],'recovery':'resume-original-batch','head':head}


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
        if ((pr.get('head',{}).get('repo') or {}).get('full_name')!=repository_name or
                pr.get('base',{}).get('ref')!='master' or
                pr.get('head',{}).get('ref')!=publication['branch_name']):
            patch={'state':'ownership-lost','closure_reason':'GitHub PR repository/base/branch differs from reservation'}
        elif publication['expected_head'] and pr['head']['sha']!=publication['expected_head']:
            patch={'state':'ownership-lost','closure_reason':'GitHub PR head differs from reserved head'}
        elif pr.get('merged'):
            patch={'state':'merged','closure_reason':'GitHub confirms merge'}
        elif pr['state']=='closed':
            patch={'state':'closed-deferred','closure_reason':'GitHub confirms closure without merge'}
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
    # A rerun resumes its own reservation; unconfirmed others keep their slot.
    reconcile_publications(client,args.repository_name,pubid)
    existing=client.one('publications',publication_id=pubid)
    if existing:
        if existing['state'] not in ('reserved','open','updating'):
            raise ValueError('Publication is closed or ownership lost; create a reviewed new batch')
        recovered=recover_reservation(client,args.repository_name,existing)
        if recovered['state'] not in ('reserved','open','updating'):
            raise ValueError('Publication is closed or ownership lost; no branch recreation allowed')
    if git('rev-parse','origin/master')!=revision or git('rev-parse','HEAD')!=revision:
        raise ValueError('Publication target has advanced; reconcile and refresh acceptance first')
    patch=Path(args.patch).read_bytes();manifest=Path(args.manifest).read_bytes()
    proof=json.loads(manifest)
    if (proof.get('valid') is not True or proof.get('base_revision')!=revision or
            proof.get('patch_digest')!=hashlib.sha256(patch).hexdigest()):
        raise ValueError('Publication patch lacks matching validation proof')
    branch='test262/native-central-'+batch_id
    accepted=client.one('native_batches',batch_id=batch_id)
    if (accepted is None or accepted['state']!='sealed' or
            publication_base(client,{'batch_id':batch_id})!=revision):
        raise ValueError('Publication acceptance is stale or conflicted')
    existing=client.one('publications',publication_id=pubid)
    patchsha=hashlib.sha256(patch).hexdigest();manifestsha=hashlib.sha256(manifest).hexdigest()
    record={'publication_id':pubid,'repository_id':args.repository,'batch_id':batch_id,'branch_name':branch,
            'patch_sha256':bytea(patchsha),'manifest_sha256':bytea(manifestsha)}
    client.put('publications',[record])
    if existing and existing['state'] not in ('reserved','open','updating'):
        raise ValueError('Publication is closed or ownership lost; create a reviewed new batch')
    # Reservation is committed before any GitHub side effect. A crash can reconcile branch/PR by stable name.
    prs=list_pulls(args.repository_name,branch)
    if len(prs)>1 or any(not pull_scope(pr,args.repository_name) for pr in prs):
        raise ValueError('Publication PR scope is ambiguous or foreign')
    if prs and prs[0]['state']!='OPEN':
        current=client.one('publications',publication_id=pubid)
        recover_reservation(client,args.repository_name,current)
        raise ValueError('Publication PR is closed; no branch recreation allowed')
    remote=subprocess.run(['git','ls-remote','--heads','origin',branch],cwd=REPO,capture_output=True,text=True,check=True).stdout.strip()
    if prs and not remote:
        raise ValueError('Open publication PR lost its branch; manual reconciliation required')
    if remote:
        head=remote.split()[0]
        if existing and existing.get('expected_head') and existing['expected_head']!=head:
            client.call('transition','publications',{'publication_id':pubid},existing['version'],{'state':'ownership-lost','closure_reason':'remote head changed'})
            raise ValueError('Publication branch ownership lost')
        subprocess.run(['git','fetch','origin',branch],cwd=REPO,check=True)
        # Unknown crash-after-push is adopted only if parent, patch bytes and commit marker match.
        if git('show','-s','--format=%P',head)!=revision or git('show','-s','--format=%B',head).strip()!=f'test262: native central batch {batch_id}':
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
    prs=list_pulls(args.repository_name,branch)
    if len(prs)>1:
        raise ValueError('Multiple PRs for central publication; manual reconciliation required')
    if prs:
        pr=prs[0]
        if not pull_scope(pr,args.repository_name) or pr['state']!='OPEN' or pr['headRefOid']!=head:
            raise ValueError('PR state/head changed; do not create a duplicate')
        number=pr['number']
    else:
        url=subprocess.check_output(['gh','pr','create','--repo',args.repository_name,'--base','master','--head',branch,
                                      '--title',f'test262: native batch {batch_id}', '--body-file',args.body],text=True).strip()
        number=int(url.rsplit('/',1)[-1])
    current=client.one('publications',publication_id=pubid)
    client.call('transition','publications',{'publication_id':pubid},current['version'],{'state':'open','pr_number':number,'expected_head':head})
    return {'publication_id':pubid,'pr_number':number,'head':head,'requires_independent_pr_ci':True}
