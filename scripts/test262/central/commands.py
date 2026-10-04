"""Trusted coordinator operations; fixture execution lives in worker.py."""
import argparse
from datetime import datetime, timezone
import json
import os
from pathlib import Path
import subprocess
import sys
from .client import bytea, canonical, fixture_environment, identity, sha
from .inventory import normalize_fixture, register, registrations, REPO
from .importer import provenance, start_run

sys.path.insert(0, str(REPO/'scripts/test262'))
import catalog
import nativePorting


def git(*arguments):
    return subprocess.check_output(['git','-C',str(REPO),*arguments],text=True).strip()


def github_validation(repository, revision):
    # gh credential is scoped to trusted coordinator; not fixture environment.
    result = json.loads(subprocess.check_output(['gh','api',f'repos/{repository}/actions/workflows/test262-mvp.yml/runs?head_sha={revision}&per_page=100'],text=True))
    valid = [r for r in result['workflow_runs'] if r['head_sha']==revision and r['head_branch']=='master'
             and r['event']=='push' and r['conclusion']=='success' and r['status']=='completed']
    if not valid:
        raise ValueError('No successful master-push Test262 MVP validation for exact target SHA')
    run = max(valid,key=lambda r:r['id'])
    # The server response is evidence. Restrict to known fields; never execute retrieved text.
    return {key:run[key] for key in ('id','head_sha','head_branch','event','conclusion','html_url','workflow_id','updated_at')}


def prepare(client, args):
    root = Path(args.root).resolve()
    source = git('rev-parse','HEAD')
    if source != args.revision or git('status','--porcelain','--untracked-files=no'):
        raise ValueError('Prepare requires a clean checkout of the requested exact revision')
    inventory = catalog.bridge({'command':'inventory','root':str(root)})
    pin = json.loads((REPO/'tests/test262/test262.pin.json').read_text())['upstream']
    normalized = [normalize_fixture(row,root) for row in inventory]
    corpus, ids = register(client,args.repository,pin,normalized)
    snapshot = registrations(client,args.repository,source,ids)
    entry = Path(args.jroc if args.kind=='mvp-composite' else args.host).resolve()
    capabilities = {} if args.kind=='mvp-composite' else json.loads(subprocess.check_output(['dotnet',str(entry),'--capabilities'],text=True,env=fixture_environment()))
    env = catalog.environment(entry)
    # Include all native testing implementation as well as tooling; SHA is diagnostic, binary identities authoritative.
    tooling = catalog.hash_files(REPO,['scripts/test262/**/*.py','scripts/test262/**/*.js','scripts/test262/NativeScreeningHost/**/*.cs','tests/Jroc.Testing/**/*.cs','tests/test262/test262.pin.json'])
    doc = {'runner':args.kind,'upstream':pin,'inventory':corpus,'binaries':catalog.hash_files(entry.parent,['*.dll','*.deps.json','*.runtimeconfig.json']),
           'harness':catalog.hash_files(root,['harness/**/*']),'tooling':tooling,'capabilities':capabilities,
           'environment_identity':env['identity'],'timeouts':{'runtime':args.runtime_timeout,'compile':args.compile_timeout,'cap_ms':args.cap_ms}}
    pid = provenance(client,args.repository,corpus,args.kind,doc)
    eligibility=[]
    for original, row in zip(inventory,normalized):
        state = 'runnable'
        if row['is_support_file']:
            state='policy-excluded'
        elif row['metadata_state']!='valid':
            state='metadata-error'
        elif args.kind=='mvp-composite' and original['state']!='runnable':
            state='harness-gap'
        elif args.kind=='native':
            if not nativePorting.native_eligible_state(dict(original,reasons=canonical(original['reasons']))):
                state='harness-gap'
            meta=row['metadata']; negative=meta.get('negative') or {}
            includes=capabilities.get('includes',[])
            if negative.get('phase') in ('parse','early','resolution') or 'raw' in meta.get('flags',[]) or any(i not in includes for i in meta.get('includes',[])) or row['dependency_manifest_digest'] is None:
                state='harness-gap'
        eligibility.append({'repository_id':args.repository,'provenance_id':pid,'fixture_id':ids[row['path']],
                            'eligibility':state,'reason_codes':[reason['code'] for reason in original['reasons']],
                            'diagnostic':{'source_state':original['state']}})
    client.put('provenance_fixture_eligibility',eligibility)
    run = start_run(client,args,pid,args.run_key,source,'mvp' if args.kind=='mvp-composite' else 'native')
    budget = identity(args.repository,'budget',args.budget_key)
    client.put('budget_scopes',[{'budget_scope_id':budget,'repository_id':args.repository,'scope_kind':'run','external_key':args.budget_key,
                                 'candidate_limit':args.candidate_limit,'accepted_limit':args.accepted_limit,
                                 'attempt_limit':args.attempt_limit,'time_limit_ms':args.budget_ms}])
    registered={r['upstream_path'] for r in client.read('registrations',{'snapshot_id':snapshot})}
    eligible={e['fixture_id'] for e in eligibility if e['eligibility']=='runnable'}
    choices=[r for r in normalized if ids[r['path']] in eligible and (args.kind!='native' or r['path'] not in registered) and (not args.area or r['path'].startswith('test/'+args.area+'/'))]
    terminal={(w['fixture_id'],w['variant']) for w in client.read('work_items',{'provenance_id':pid}) if w['state'] in ('completed','deferred','cancelled')}
    reconciliation=client.one('reconciliation_state',pipeline='native',channel='master') if args.kind=='native' else None
    choices=[r for r in choices if any((ids[r['path']],v['variant']) not in terminal for v in r['variants'])]
    # Preserve component-sensitive retry hints and a bounded rotating discovery share.
    balanced=[r for r,_ in catalog.area_balanced([(dict(r,path=r['path']),None) for r in choices])]
    if args.kind=='native':
        cur_hint=reconciliation or {}
        prior=cur_hint.get('last_reconciled_revision') or source
        if prior!=source and subprocess.run(['git','-C',str(REPO),'merge-base','--is-ancestor',prior,source],capture_output=True).returncode:
            raise ValueError('Prior validated target is not an ancestor')
        components=set(nativePorting.classify_changed_components(git('diff','--name-only',prior,source).splitlines()))
        fixtures_by_id={}
        eligible_runs={r['run_id'] for r in client.read('runs') if r['trust_class'] in ('trusted','legacy')}
        failure_hints=set()
        for observed in client.read('observations',{'outcome':'fail'}):
            if observed['run_id'] not in eligible_runs or observed['provenance_id']==pid:
                continue
            if observed['fixture_id'] not in fixtures_by_id:
                fixtures_by_id[observed['fixture_id']]=client.one('fixtures',fixture_id=observed['fixture_id'])
            original_fixture=fixtures_by_id[observed['fixture_id']]
            if components.intersection(nativePorting.candidate_components(original_fixture['upstream_path'])):
                failure_hints.add((original_fixture['upstream_path'],original_fixture['content_sha256'][2:]))
        retries=[r for r in balanced if (r['path'],r['sha256']) in failure_hints]
        fallback=[r for r in balanced if r not in retries]
        cursor=cur_hint.get('fallback_cursor',0)
        if fallback:
            offset=cursor%len(fallback); fallback=fallback[offset:]+fallback[:offset]
        reserve=max(1,args.candidate_limit//5)
        choices=retries[:args.candidate_limit-reserve]+fallback[:max(reserve,args.candidate_limit-len(retries[:args.candidate_limit-reserve]))]
        if len(choices)<args.candidate_limit:
            choices+=retries[args.candidate_limit-reserve:args.candidate_limit-reserve+(args.candidate_limit-len(choices))]
    else:
        choices=balanced[:args.candidate_limit]
    work_rows=[{'work_item_id':identity(args.repository,pid,ids[r['path']],v['variant'],0),'repository_id':args.repository,
               'provenance_id':pid,'fixture_id':ids[r['path']],'variant':v['variant'],'retry_generation':0,
               'coherent_area':nativePorting.coherent_area(r['path']),'priority':0,'attempt_limit':3,
               'selection_reason':{'reason':'fresh provenance discovery'}} for r in choices for v in r['variants']]
    validation=None
    if args.kind=='native':
        proof=github_validation(args.repository_name,source)
        validation=identity(args.repository,'test262-mvp.yml',proof['id'])
        client.put('validation_events',[{'validation_id':validation,'repository_id':args.repository,'workflow_identity':'test262-mvp.yml',
                                          'external_run_key':str(proof['id']),'target_revision':source,'conclusion':'success','proof':proof}])
    target={'repository_id':args.repository,'channel':'master','evidence_kind':args.kind,'provenance_id':pid,
            'registration_snapshot_id':snapshot,'validation_id':validation}
    old=client.one('reporting_targets',channel='master',evidence_kind=args.kind)
    if old:
        client.call('transition','reporting_targets',{k:target[k] for k in ('repository_id','channel','evidence_kind')},old['version'],
                    {k:target[k] for k in ('provenance_id','registration_snapshot_id','validation_id')})
    else:
        client.put('reporting_targets',[target])
    if args.kind=='native':
        cur=reconciliation
        if cur is None:
            cur={'repository_id':args.repository,'pipeline':'native','channel':'master','authority_epoch':client.epoch,'version':0,'fallback_cursor':0}
            client.put('reconciliation_state',[cur])
        base=cur.get('last_reconciled_revision') or source
        if base!=source and subprocess.run(['git','-C',str(REPO),'merge-base','--is-ancestor',base,source],capture_output=True).returncode:
            raise ValueError('Previous target is not an ancestor; explicit audited authority reset required')
        # Independent recovery at the same SHA gets a new run key/plan, even with an unchanged cursor.
        rec=identity(args.repository,'reconcile',args.run_key)
        client.put('reconciliations',[{'reconciliation_id':rec,'repository_id':args.repository,'pipeline':'native','channel':'master',
                                       'validation_id':validation,'base_revision':base,'attribution':'validated-range',
                                       'components':nativePorting.classify_changed_components(git('diff','--name-only',base,source).splitlines()),
                                       'plan_sha256':bytea(sha({'work':work_rows,'next_cursor':cur['fallback_cursor']+len(choices)})),'expected_cursor_version':cur['version']}])
        client.call('reconcile',rec,cur['version'],cur['fallback_cursor']+len(choices),work_rows)
    else:
        client.put('work_items',work_rows)
    client.put('budget_work_items',[{'repository_id':args.repository,'budget_scope_id':budget,'work_item_id':w['work_item_id']} for w in work_rows])
    result={'repository':args.repository,'producer':args.producer,'run':run,'budget':budget,'provenance':pid,
            'kind':args.kind,'revision':source,'registration_snapshot':snapshot,'validation':validation,
            'corpus':corpus,'candidate_ids':[ids[r['path']] for r in choices],'cap_ms':args.cap_ms,'identity':doc}
    Path(args.output).parent.mkdir(parents=True,exist_ok=True)
    Path(args.output).write_text(canonical(result)+'\n')
    return {key:value for key,value in result.items() if key!='identity'}


def seal(client,args):
    context=json.loads(Path(args.context).read_text())
    if context['kind']!='native':
        raise ValueError('Native batch needs native context')
    with client.view():
        trusted={r['run_id'] for r in client.read('runs',{'provenance_id':context['provenance']}) if r['trust_class']=='trusted'}
        budget=client.one('budget_scopes',budget_scope_id=context['budget'])
        accepted=[]; bindings=[]
        for fid in context['candidate_ids']:
            by_variant={}; contradicted=False
            for o in client.read('observations',{'provenance_id':context['provenance'],'fixture_id':fid}):
                if o['run_id'] not in trusted or client.one('observation_invalidations',observation_id=o['observation_id']):
                    continue
                # Only product evidence contradicts; infrastructure errors/incomplete attempts are retryable.
                if o['outcome'] in ('fail','unsupported'):
                    contradicted=True
                elif o['outcome']=='pass' and o['source_kind']=='live':
                    by_variant.setdefault(o['variant'],[]).append(o)
            required=[v['variant'] for v in client.read('fixture_variants',{'fixture_id':fid}) if v['required']]
            if contradicted or not required or any(not by_variant.get(v) for v in required):
                continue
            accepted.append(fid)
            for v in required:
                chosen=max(by_variant[v],key=lambda o:(o['received_at'],o['observation_id']))
                bindings.append({'fixture_id':fid,'variant':v,'observation_id':chosen['observation_id']})
            if len(accepted)>=budget['accepted_limit']:
                break
    if not accepted:
        return {'accepted':0,'batch':None}
    batch=identity(args.repository,'native-batch',args.batch_key)
    client.put('native_batches',[{'batch_id':batch,'repository_id':args.repository,'batch_key':args.batch_key,'provenance_id':context['provenance'],
                                  'validation_id':context['validation'],'registration_snapshot_id':context['registration_snapshot'],'budget_scope_id':context['budget']}])
    client.put('batch_fixtures',[{'batch_id':batch,'fixture_id':f,'state':'accepted'} for f in accepted])
    client.put('batch_evidence',[dict(row,batch_id=batch) for row in bindings])
    stored=client.one('native_batches',batch_id=batch)
    client.call('transition','native_batches',{'batch_id':batch},stored['version'] if stored else 0,{'state':'sealed'})
    result={'batch':batch,'accepted':len(accepted),'context':context}
    Path(args.output).write_text(canonical(result)+'\n')
    return result


def generate(client,args):
    """Build a disposable nativePorting cache solely from central sealed evidence."""
    batch=json.loads(Path(args.batch_file).read_text())
    context=batch['context']; batch_id=batch['batch']
    with client.view():
        return generate_from_view(client,args,context,batch_id)


def generate_from_view(client,args,context,batch_id):
    stored=client.one('native_batches',batch_id=batch_id)
    if stored is None or stored['state']!='sealed' or context['revision']!=git('rev-parse','HEAD'):
        raise ValueError('Generation needs fresh sealed acceptance at the current checkout SHA')
    cache=Path(args.cache)
    if cache.exists():
        raise ValueError('Generation cache must be new; never restore downstream authority')
    db=nativePorting.connect(cache)
    pin=context['identity']['upstream']['commit']
    nativePorting.create_run(db,argparse.Namespace(run_id=context['run'],batch_id=batch_id,trigger_revision=context['revision'],base_revision=context['revision'],pin=pin,
                              candidate_limit=500,accepted_limit=500,variant_limit=2000,time_limit=86400))
    doc=context['identity']; compiler=sha(doc['binaries']); harness=sha(doc['harness']); environment=sha(doc['environment_identity'])
    db.execute('INSERT INTO active_provenance VALUES(?,?,?,?,0)',(context['run'],compiler,harness,environment))
    for member in client.read('batch_fixtures',{'batch_id':batch_id,'state':'accepted'}):
        f=client.one('fixtures',fixture_id=member['fixture_id'])
        evidence=list(client.read('batch_evidence',{'batch_id':batch_id,'fixture_id':f['fixture_id']}))
        db.execute('INSERT INTO candidates VALUES(?,?,?,?,?,?,?,?,?,0)',(context['run'],f['upstream_path'],f['content_sha256'][2:],canonical(sorted(e['variant'] for e in evidence)),
                   'central sealed evidence','native',context['provenance'],sha(doc['capabilities']),'accepted'))
        for e in evidence:
            o=client.one('observations',observation_id=e['observation_id'])
            db.execute('''INSERT INTO attempts(run_id,path,variant,fixture_sha256,pin,compiler_identity,harness_identity,environment_identity,phase,diagnostic,outcome,failure_class,started_at,finished_at) VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?)''',
                       (context['run'],f['upstream_path'],e['variant'],f['content_sha256'][2:],pin,compiler,harness,environment,o['phase'],o['diagnostic_summary'],o['outcome'],o['failure_class'],datetime.fromisoformat(o['started_at']).timestamp(),datetime.fromisoformat(o['finished_at']).timestamp()))
    db.commit()
    result=nativePorting.generate_batch(db,argparse.Namespace(run_id=context['run'],upstream=args.root,destination=str(REPO),output=args.output))
    db.close()
    return result
