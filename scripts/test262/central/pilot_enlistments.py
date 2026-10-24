"""Two checkout/LOGIN/producer/process/outbox enlistments on disposable PostgreSQL.

The independent clients use production identity verification, enlistment and Docker
execution. Faults are restricted to the loopback pilot. No production enrollment.
"""
from datetime import datetime, timezone
import json
import os
from pathlib import Path
import shutil
import sqlite3
import subprocess
import sys
import tempfile
import time
from types import SimpleNamespace
import uuid

from .client import Client, Outbox, bytea, canonical, fixture_environment, identity, sha
from .execution_identity import execution_identity
from .importer import provenance, start_run
from .inventory import REPO, register
from .pilot import disposable_settings
from .pilot_drills import require, observation, expect_rejection


def enroll(admin,repository,index):
    import psycopg
    from psycopg import sql
    from psycopg.conninfo import make_conninfo
    disposable_settings(admin)
    login='catalogue_pilot_independent_'+('a' if index==0 else 'b')
    producer=str(uuid.uuid4());password=uuid.uuid4().hex
    with psycopg.connect(admin,autocommit=True) as db:
        db.execute(sql.SQL('CREATE ROLE {} LOGIN INHERIT PASSWORD {}').format(sql.Identifier(login),sql.Literal(password)))
        db.execute(sql.SQL('GRANT test262_worker TO {}').format(sql.Identifier(login)))
        db.execute("INSERT INTO test262.producers(producer_id,repository_id,kind,display_name,credential_subject) VALUES(%s,%s,'local',%s,%s)",
                   (producer,repository,'independent checkout '+str(index),login))
        db.execute("INSERT INTO test262.api_subjects VALUES(%s,%s,%s,'coordinator','trusted',true)",(login,repository,producer))
    return {'producer':producer,'login':login,'dsn':make_conninfo(admin,user=login,password=password)}


def clone(source,destination,revision):
    subprocess.run(['git','clone','--no-hardlinks','--quiet',str(source),str(destination)],check=True,capture_output=True)
    subprocess.run(['git','-C',str(destination),'checkout','--quiet','--detach',revision],check=True,capture_output=True)


def worker_environment(worker):
    env=fixture_environment()
    # This supervisor receives only its own execution credential, never admin or
    # coordinator/publication credentials. Fixtures get the production allowlist.
    env.update(TEST262_DATABASE_URL=worker['dsn'],TEST262_AUTHORITY_EPOCH='1',
               TEST262_REPOSITORY_ID=worker['repository'],TEST262_PRODUCER_ID=worker['producer'],
               TEST262_PILOT_CANARY='must-not-reach-fixtures')
    return env


def launch(worker,arguments,log):
    stream=open(log,'w')
    try:
        child=subprocess.Popen([sys.executable,'-m',*arguments],cwd=worker['checkout'],
            env=worker_environment(worker),stdout=stream,stderr=subprocess.STDOUT)
    finally:
        stream.close()
    return child


def flush(worker,outbox,log):
    child=launch(worker,['scripts.test262.central.cli','flush','--outbox',str(outbox)],log)
    try:
        require(child.wait(timeout=60)==0,'Independent outbox replay failed')
    finally:
        if child.poll() is None:
            child.kill();child.wait(timeout=10)


def pending(path,repository,producer):
    spool=Outbox(path,repository,producer,1)
    try:
        return sum(spool.db.execute('SELECT count(*) FROM '+table+' WHERE receipt IS NULL').fetchone()[0]
                   for table in ('messages','completions'))
    finally:
        spool.db.close()


def check_permissions(client):
    denied={}
    for operation,values in (
            ('put',('work_items',[])),('transition',('reporting_targets',{},0,{})),
            ('reconcile',(str(uuid.uuid4()),0,0,[])),('finish_budget',(str(uuid.uuid4()),False))):
        denied[operation]=expect_rejection(client,operation,values,'42501')
    # Repository-bound reads must not leak a different repository's scope.
    require(not list(client.read('runs',{'repository_id':str(uuid.uuid4())})), 'Foreign repository leaked')
    return denied


def run_drill(client,admin,base,fixture,revision,output,report):
    disposable_settings(admin)
    require(client.db.info.dbname=='catalogue_pilot' and client.db.info.user=='catalogue_pilot_worker',
            'Independent drill requires the established disposable pilot')
    output=Path(output).resolve();output.mkdir(parents=True,exist_ok=True)
    report.update(complete=False,production_enrollment=False,scope='two independent local checkouts on one CI host',
                  partial_inventory=True,remote_hosts_tested=False,scenarios={},workers=[],
                  validation='synthetic successful central validation in disposable scope only')
    workers=[]
    try:
        with tempfile.TemporaryDirectory(prefix='independent-enlistments-') as temporary:
            temp=Path(temporary)
            pin=json.loads((REPO/'tests/test262/test262.pin.json').read_text())['upstream']
            corpus,ids=register(client,base.repository,pin,[fixture]);fid=ids[fixture['path']]
            for index in range(2):
                worker=enroll(admin,base.repository,index);workers.append(worker)
                worker.update(repository=base.repository,checkout=temp/f'checkout-{index}',root=temp/f'upstream-{index}')
                clone(REPO,worker['checkout'],revision)
                clone(base.root,worker['root'],pin['commit'])
                # Independently staged exact reviewed binaries. Rebuilding on a
                # different environment is supported only if identity matches.
                for entry in (Path(base.jroc).resolve(),Path(base.host).resolve()):
                    shutil.copytree(entry.parent,worker['checkout']/entry.parent.relative_to(REPO),dirs_exist_ok=True)
                # Preserve generated testing sources included by existing provenance.
                for folder in ('scripts/test262/NativeScreeningHost/obj','tests/Jroc.Testing/obj'):
                    if (REPO/folder).exists():
                        shutil.copytree(REPO/folder,worker['checkout']/folder,dirs_exist_ok=True)
                (worker['checkout']/'node_modules').symlink_to(REPO/'node_modules',target_is_directory=True)
                connection=Client(dsn=worker['dsn'],epoch=1)
                try:
                    denied=check_permissions(connection)
                finally:
                    connection.close()
                report['workers'].append({'producer_id':worker['producer'],'login':worker['login'],
                    'checkout_revision':revision,'upstream_commit':pin['commit'],'denied_operations':denied})
            snapshots=list(client.read('registration_snapshots',{'source_revision':revision}))
            snapshot=next((s['snapshot_id'] for s in snapshots if s['inventory_sha256']==bytea(sha([]))),None)
            if snapshot is None:
                snapshot=identity(base.repository,'independent-empty-registrations',revision)
                client.put('registration_snapshots',[{'snapshot_id':snapshot,'repository_id':base.repository,
                    'source_revision':revision,'inventory_sha256':bytea(sha([])),
                    'warnings':['Controlled empty disposable independent-worker snapshot']}])
                client.call('transition','registration_snapshots',{'snapshot_id':snapshot},0,{'state':'sealed'})
            validation=identity(base.repository,'independent-disposable-validation',revision)
            client.put('validation_events',[{'validation_id':validation,'repository_id':base.repository,
                'workflow_identity':'independent-disposable-validation','external_run_key':revision,
                'target_revision':revision,'conclusion':'success','proof':{'synthetic_disposable_validation':True}}])
            for kind,fault in (('mvp-composite','lost-ack'),('native','pause-before-upload'),('native','offline-before-upload')):
                name=kind+'-'+fault;folder=output/name;folder.mkdir()
                args=SimpleNamespace(**vars(base));args.kind=kind
                args.runtime_timeout=30;args.compile_timeout=60;args.cap_ms=120000;args.compilation_coverage=False
                document=execution_identity(args,corpus)
                pid=provenance(client,base.repository,corpus,kind,document)
                owner=start_run(client,base,pid,'independent-prepare-'+name,revision,'native' if kind=='native' else 'mvp')
                budget=str(uuid.uuid4())
                client.put('budget_scopes',[{'budget_scope_id':budget,'repository_id':base.repository,'scope_kind':'batch',
                    'external_key':'independent-'+name,'candidate_limit':1,'accepted_limit':1,'attempt_limit':2,'time_limit_ms':240000}])
                client.put('provenance_fixture_eligibility',[{'repository_id':base.repository,'provenance_id':pid,
                    'fixture_id':fid,'eligibility':'runnable','reason_codes':[]}])
                # The second native scenario deliberately uses a fresh retry generation;
                # it is explicitly labelled, never mistaken for unnecessary repetition.
                generation=1 if fault=='offline-before-upload' else 0
                rows=[{'work_item_id':identity(base.repository,'independent',name,v['variant']),
                    'repository_id':base.repository,'provenance_id':pid,'fixture_id':fid,'variant':v['variant'],
                    'retry_generation':generation,'coherent_area':'built-ins/Math/abs','attempt_limit':2,
                    'selection_reason':{'disposable_independent_drill':name,'deliberate_retry':bool(generation)}} for v in fixture['variants']]
                client.put('work_items',rows)
                client.put('budget_work_items',[{'repository_id':base.repository,'budget_scope_id':budget,'work_item_id':r['work_item_id']} for r in rows])
                target={'repository_id':base.repository,'channel':'master','evidence_kind':kind,
                        'provenance_id':pid,'registration_snapshot_id':snapshot,'validation_id':validation if kind=='native' else None}
                old=client.one('reporting_targets',channel='master',evidence_kind=kind)
                if old:
                    client.call('transition','reporting_targets',{k:target[k] for k in ('repository_id','channel','evidence_kind')},old['version'],
                        {k:target[k] for k in ('provenance_id','registration_snapshot_id','validation_id')})
                else:
                    client.put('reporting_targets',[target])
                context={'repository':base.repository,'producer':base.producer,'run':owner,'budget':budget,'provenance':pid,
                    'kind':kind,'revision':revision,'corpus':corpus,'registration_snapshot':snapshot,'validation':target['validation_id'],
                    'cap_ms':120000,'identity':document}
                context_file=folder/'context.json';context_file.write_text(canonical(context))
                children=[];configs=[]
                try:
                    for i,worker in enumerate(workers):
                        worker_args={'repository':base.repository,'producer':worker['producer'],'context':str(context_file),
                            'root':str(worker['root']),'jroc':'src/Cli/bin/Release/net10.0/Jroc.dll',
                            'host':'scripts/test262/NativeScreeningHost/bin/Release/net10.0/NativeScreeningHost.dll',
                            'image':base.image,'run_key':name,'outbox':str(folder/f'outbox-{i}.sqlite'),
                            'limit':1,'seconds':120,'output':str(folder/f'receipt-{i}.json')}
                        config={'args':worker_args,'control':{'ready':str(folder/f'ready-{i}'),
                            'peer_ready':str(folder/f'ready-{1-i}'),'checkpoint':str(folder/'checkpoint.json'),
                            'fault':fault if i==0 else None}}
                        configs.append(config);path=folder/f'worker-{i}.json';path.write_text(canonical(config))
                        children.append(launch(worker,['scripts.test262.central.pilot_enlistment_child',str(path)],folder/f'worker-{i}.log'))
                    if fault=='pause-before-upload':
                        deadline=time.monotonic()+100
                        while not (folder/'checkpoint.json').exists():
                            require(children[0].poll() is None and time.monotonic()<deadline,'Independent durable checkpoint failed')
                            time.sleep(.1)
                        require(pending(configs[0]['args']['outbox'],base.repository,workers[0]['producer'])==2,
                                'Killed worker did not persist upload/completion')
                        children[0].kill()
                    codes=[child.wait(timeout=120) for child in children]
                    require(codes[0]!=0 and codes[1]==0,'Independent fault/concurrent worker failed')
                finally:
                    for child in children:
                        if child.poll() is None:
                            child.kill();child.wait(timeout=10)
                for i,worker in enumerate(workers):
                    flush(worker,configs[i]['args']['outbox'],folder/f'flush-{i}.log')
                    flush(worker,configs[i]['args']['outbox'],folder/f'replay-{i}.log')
                    require(pending(configs[i]['args']['outbox'],base.repository,worker['producer'])==0,'Independent outbox did not drain')
                # End the interrupted supervisor's collection without running a
                # replacement fixture, even after its local checkout has drifted.
                replay_args=configs[0]['args']
                command=['scripts.test262.central.cli','enlist','--replay-only',
                    '--context',replay_args['context'],'--root',replay_args['root'],
                    '--run-key',name,'--outbox',replay_args['outbox'],
                    '--output',str(folder/'recovery-receipt.json')]
                recovered=launch(workers[0],command,folder/'recovery.log')
                try:
                    require(recovered.wait(timeout=60)==0,'Interrupted enlistment did not finalize after replay')
                finally:
                    if recovered.poll() is None:
                        recovered.kill();recovered.wait(timeout=10)
                recovery=json.loads((folder/'recovery-receipt.json').read_text())
                require(recovery['new_attempts']==0 and recovery['collection_state']=='incomplete' and
                        not recovery['suite_completion_claimed'],'Replay overstated interrupted collection')
                runs=[identity(base.repository,w['producer'],f"worker:{w['producer']}:{name}") for w in workers]
                observations=[o for run in runs for o in client.read('observations',{'run_id':run})]
                stored=client.one('budget_scopes',budget_scope_id=budget)
                require(len(observations)==2 and len({o['work_item_id'] for o in observations})==2 and
                    all(o['outcome'] not in ('infrastructure-error','incomplete') for o in observations) and
                    stored['charged_attempts']==2 and stored['reserved_attempts']==stored['reserved_ms']==0,
                    'Independent execution/replay lost, repeated or mischarged work')
                report['scenarios'][name]={'complete':True,'identity':document,'provenance_id':pid,
                    'source_revision':revision,'runs':runs,'budget':stored,'fault':fault,
                    'observations':[{k:o[k] for k in ('observation_id','run_id','fixture_id','variant','outcome','lease_generation')} for o in observations],
                    'pending_outboxes':0,'distinct_claims':2,'fresh_executions':2,'recovery':recovery}
            negative_launches(client,workers[0],context,output,report)
            cross_worker_controls(client,workers,base,corpus,fid,revision,report)
            report['complete']=True
    finally:
        import psycopg
        from psycopg import sql
        # Revoke even after a failed drill. Existing rows/evidence remain for audit/restore.
        with psycopg.connect(admin,autocommit=True) as db:
            for worker in workers:
                connected=Client(dsn=worker['dsn'],epoch=1)
                db.execute('UPDATE test262.api_subjects SET enabled=false WHERE login_name=%s',(worker['login'],))
                db.execute('UPDATE test262.producers SET enabled=false WHERE producer_id=%s',(worker['producer'],))
                try:
                    try:
                        list(connected.read('runs',page=1))
                    except psycopg.errors.InsufficientPrivilege:
                        pass
                    else:
                        raise ValueError('Revocation did not fence the existing worker connection')
                finally:
                    connected.close()
                db.execute(sql.SQL('REVOKE test262_worker FROM {}').format(sql.Identifier(worker['login'])))
                db.execute(sql.SQL('ALTER ROLE {} NOLOGIN').format(sql.Identifier(worker['login'])))
                require(not db.execute('SELECT enabled FROM test262.api_subjects WHERE login_name=%s',(worker['login'],)).fetchone()[0],
                        'Independent binding did not revoke')
        report['revoked_producers']=[w['producer'] for w in workers]
        report['existing_sessions_fenced']=True
        (output/'independent-enlistments-report.json').write_text(json.dumps(report,indent=2)+'\n')


def negative_launches(client,worker,context,output,report):
    """Supported CLI must fail before a run/claim/outbox for stale/offline cold starts."""
    from psycopg.conninfo import make_conninfo
    folder=output/'negative-launches';folder.mkdir()
    source=folder/'context.json';source.write_text(canonical(context))
    before={t:list(client.read(t)) for t in ('runs','observations','budget_scopes','work_items','reporting_targets')}
    result={}
    for scenario in ('dirty-checkout','changed-binary','offline-cold-start'):
        spool=folder/(scenario+'.sqlite')
        args=['scripts.test262.central.cli','enlist','--context',str(source),'--root',str(worker['root']),
            '--run-key',scenario,'--outbox',str(spool),'--limit','1']
        selected=dict(worker)
        path=None;original=None
        if scenario=='dirty-checkout':
            path=worker['checkout']/'README.md';original=path.read_bytes();path.write_bytes(original+b'\nDisposable dirty checkout\n')
        elif scenario=='changed-binary':
            # Both native and MVP output folders may share the compiler name;
            # alter a native binary that is hashed before invocation.
            path=worker['checkout']/'scripts/test262/NativeScreeningHost/bin/Release/net10.0/Jroc.Compiler.dll'
            original=path.read_bytes();path.write_bytes(original+b'disposable mismatch')
        else:
            selected['dsn']=make_conninfo(worker['dsn'],port=1,connect_timeout=2)
        try:
            child=launch(selected,args,folder/(scenario+'.log'))
            try:
                require(child.wait(timeout=60)!=0 and not spool.exists(),'Unsafe cold start created an outbox/executed work')
            finally:
                if child.poll() is None:
                    child.kill();child.wait(timeout=10)
        finally:
            if path is not None:
                path.write_bytes(original)
        result[scenario]={'rejected':True,'outbox_created':False}
    require({t:list(client.read(t)) for t in before}==before,'Negative launch changed central state')
    report['negative_launches']=result


def cross_worker_controls(client,workers,base,corpus,fid,revision,report):
    """Real server expiry across different producer logins; synthetic evidence only."""
    first=Client(dsn=workers[0]['dsn'],epoch=1);second=Client(dsn=workers[1]['dsn'],epoch=1)
    try:
        pid=provenance(client,base.repository,corpus,'native',{'synthetic_independent_expiry':True})
        args=[]
        for i,worker in enumerate(workers):
            a=SimpleNamespace(repository=base.repository,producer=worker['producer'])
            a.run=start_run((first,second)[i],a,pid,'independent-expiry',revision,'native');args.append(a)
        budget=str(uuid.uuid4());item=str(uuid.uuid4())
        client.put('budget_scopes',[{'budget_scope_id':budget,'repository_id':base.repository,'scope_kind':'batch',
            'external_key':'independent-expiry','candidate_limit':1,'accepted_limit':1,'attempt_limit':3,'time_limit_ms':3000}])
        client.put('work_items',[{'work_item_id':item,'repository_id':base.repository,'provenance_id':pid,'fixture_id':fid,
            'variant':'strict','coherent_area':'synthetic-independent-expiry','attempt_limit':3}])
        client.put('budget_work_items',[{'repository_id':base.repository,'budget_scope_id':budget,'work_item_id':item}])
        old=first.call('claim',args[0].run,budget,1000,31)
        delay=(datetime.fromisoformat(old['lease_expires_at'])-datetime.now(timezone.utc)).total_seconds()
        time.sleep(max(0,delay)+.2)
        expect_rejection(first,'renew',(item,old['lease_generation'],31),'40001')
        new=second.call('claim',args[1].run,budget,1000,31)
        require(new['work_item_id']==item and new['lease_generation']==old['lease_generation']+1,'Cross-producer reclaim failed')
        expect_rejection(first,'renew',(item,new['lease_generation'],31),'40001')
        before=client.one('budget_scopes',budget_scope_id=budget)
        late=observation(args[0],pid,old);late.update(outcome='fail',failure_class='unresolved')
        first.call('ingest',str(uuid.uuid4()),[late])
        receipt=first.call('complete',item,old['lease_generation'],late['observation_id'])
        require(receipt.get('retained_late_evidence') and not receipt.get('completed') and
                client.one('budget_scopes',budget_scope_id=budget)==before,'Late worker changed replacement budget')
        current=observation(args[1],pid,new);current.update(outcome='pass',failure_class=None)
        request=str(uuid.uuid4());ack=second.call('ingest',request,[current])
        second.call('complete',item,new['lease_generation'],current['observation_id'])
        require(second.call('ingest',request,[current])==ack,'Identical cross-worker replay differs')
        changed=dict(current,outcome='fail',failure_class='unresolved')
        expect_rejection(second,'ingest',(str(uuid.uuid4()),[changed]),'P0001')
        observations=list(client.read('observations',{'provenance_id':pid}))
        settled=client.one('budget_scopes',budget_scope_id=budget)
        require(len(observations)==2 and {o['outcome'] for o in observations}=={'pass','fail'} and
                settled['charged_attempts']==2 and settled['charged_ms']==1000 and
                settled['reserved_attempts']==settled['reserved_ms']==0,'Cross-worker expiry evidence/accounting failed')
        report['cross_worker_expiry']={'synthetic_evidence':True,'real_deadline_wait':True,
            'old_generation':old['lease_generation'],'new_generation':new['lease_generation'],
            'late_completion_fenced':True,'contradictory_evidence_retained':True,'budget':settled,
            'observation_ids':[o['observation_id'] for o in observations]}
    finally:
        first.close();second.close()
