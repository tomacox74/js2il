"""Ten real fixture variants through the production worker on an empty local database."""
import argparse
from concurrent.futures import ThreadPoolExecutor
from collections import Counter
import json
import os
from pathlib import Path
import subprocess
import tempfile
import time
import threading
from types import SimpleNamespace
import uuid

from .client import Client, Outbox, fixture_environment
from .importer import provenance, start_run
from .inventory import REPO, normalize_fixture, register
from .worker import container_arguments, stage_runtime, work_staged


class LostAcknowledgement(TimeoutError):
    """Pilot-only transport fault after the server commits an ingestion request."""


class PilotClient:
    def __init__(self, client, barrier=None, lose_ack=False):
        self.client = client
        self.barrier = barrier
        self.lose_ack = lose_ack
        self.first_claim = True
        self.first_lease = None

    def __getattr__(self, name):
        return getattr(self.client, name)

    def call(self, operation, *args):
        if operation == 'claim' and self.first_claim:
            self.first_claim = False
            if self.barrier:
                self.barrier.wait(timeout=60)
            result = self.client.call(operation, *args)
            self.first_lease = result
            return result
        result = self.client.call(operation, *args)
        if operation == 'ingest' and self.lose_ack:
            self.lose_ack = False
            raise LostAcknowledgement('Pilot simulated acknowledgement loss after commit')
        return result


def execute_pair(dsn, workers):
    """Independent connections/outboxes; restart worker zero after a lost ingest ACK."""
    barrier = threading.Barrier(2)

    def execute(index, args):
        connection = Client(dsn=dsn, epoch=1)
        client = PilotClient(connection, barrier, lose_ack=index == 0)
        result = {'run_id': args.run}
        try:
            try:
                result['worker'] = work_staged(client, args)
            except LostAcknowledgement:
                observations = list(connection.read('observations', {'run_id': args.run}))
                outbox = Outbox(args.outbox, args.repository, args.producer, 1)
                pending = outbox.db.execute('SELECT count(*) FROM messages WHERE receipt IS NULL').fetchone()[0]
                completions = outbox.db.execute('SELECT count(*) FROM completions WHERE receipt IS NULL').fetchone()[0]
                outbox.db.close()
                if len(observations) != 1 or pending != 1 or completions != 1:
                    raise ValueError('Lost ACK must leave one committed observation and durable pending upload/completion')
                result['recovery'] = {'committed_before_restart': 1, 'pending_upload_before_restart': pending,
                                      'pending_completion_before_restart': completions,
                                      'observation_id': observations[0]['observation_id']}
                connection.close()
                connection = Client(dsn=dsn, epoch=1)
                # work_staged flushes the existing spool before claiming new work.
                restart_args = SimpleNamespace(**vars(args))
                restart_args.limit -= 1
                result['worker'] = work_staged(connection, restart_args)
                result['worker']['attempts'] += 1
            result['first_work_item'] = client.first_lease['work_item_id']
            return result
        finally:
            connection.close()

    with ThreadPoolExecutor(max_workers=2) as pool:
        futures = [pool.submit(execute, i, args) for i, args in enumerate(workers)]
        return [future.result() for future in futures]


def disposable_settings(dsn):
    from psycopg.conninfo import conninfo_to_dict
    settings = conninfo_to_dict(dsn)
    if (settings.get('host') not in ('localhost', '127.0.0.1') or
            settings.get('dbname') != 'catalogue_pilot' or settings.get('user') != 'postgres' or
            settings.get('hostaddr') not in (None, '127.0.0.1')):
        raise ValueError('Pilot requires postgres on loopback database catalogue_pilot')
    return settings


def provision(dsn):
    import psycopg
    from psycopg import sql
    from psycopg.conninfo import make_conninfo
    disposable_settings(dsn)
    repo, producer = str(uuid.uuid4()), str(uuid.uuid4())
    password = uuid.uuid4().hex  # Disposable credential, never printed or uploaded.
    with psycopg.connect(dsn, autocommit=True) as db:
        if db.execute("SELECT to_regclass('public.perf_results') IS NOT NULL OR EXISTS(SELECT 1 FROM pg_namespace WHERE nspname IN ('test262','test262_reporting'))").fetchone()[0]:
            raise ValueError('Pilot refuses an existing catalogue/performance database')
        db.execute('CREATE ROLE anon; CREATE ROLE authenticated; CREATE ROLE service_role;')
        for migration in sorted((REPO/'supabase/migrations').glob('*test262_catalogue*.sql')):
            db.execute(migration.read_text())
        db.execute("INSERT INTO test262.repositories(repository_id,provider,provider_repository_id,canonical_name) VALUES(%s,'github','disposable-pilot','disposable/worker-pilot')", (repo,))
        db.execute("INSERT INTO test262.producers(producer_id,repository_id,kind,display_name,credential_subject) VALUES(%s,%s,'local','isolated pilot','catalogue_pilot_worker')", (producer,repo))
        db.execute(sql.SQL('CREATE ROLE catalogue_pilot_worker LOGIN PASSWORD {} INHERIT').format(sql.Literal(password)))
        db.execute('GRANT test262_coordinator TO catalogue_pilot_worker')
        db.execute("INSERT INTO test262.api_subjects VALUES('catalogue_pilot_worker',%s,%s,'coordinator','trusted',true)",(repo,producer))
        # Activation exists ONLY in this fresh loopback service, never Supabase.
        db.execute("UPDATE test262.schema_contract SET deployment_state='active'")
    return repo, producer, make_conninfo(dsn, user='catalogue_pilot_worker', password=password)


def isolation_probe(args, root):
    probe = r"""
const fs=require('fs'),os=require('os');
let readonly=false;try{fs.writeFileSync('/pilot-write','x')}catch(e){readonly=true}
const status=fs.readFileSync('/proc/self/status','utf8');
const checks={unprivileged:process.getuid()===65532,readonly,
 noCredentials:!Object.keys(process.env).some(k=>/^(TEST262_|PG|GH_TOKEN|GITHUB_TOKEN|AWS_)/.test(k)),
 noCheckout:!fs.existsSync('/repo/.git'),noOutbox:!fs.existsSync('/repo/artifacts'),
 noNetwork:Object.keys(os.networkInterfaces()).every(k=>k==='lo'),
 noCapabilities:/CapEff:\s+0+\n/.test(status),noNewPrivileges:/NoNewPrivs:\s+1\n/.test(status),pidNamespace:process.pid===1};
console.log(JSON.stringify(checks));if(Object.values(checks).some(v=>!v))process.exit(1);
"""
    with tempfile.TemporaryDirectory(prefix='pilot-probe-') as directory:
        work = Path(directory);work.chmod(0o777)
        result = subprocess.run(container_arguments(args, root, work, 'pilot-probe-'+uuid.uuid4().hex)+['node','-e',probe],
            capture_output=True,text=True,check=True,timeout=60,env=fixture_environment())
        return json.loads(result.stdout)


def run(args):
    import sys
    sys.path.insert(0,str(REPO/'scripts/test262'))
    import catalog
    output = Path(args.output);output.mkdir(parents=True,exist_ok=True)
    report = {'complete':False,'scope':'disposable database only','production_cutover':False,
              'kind':'mvp-composite','variant_limit':10,'worker_count':args.workers}
    started=time.monotonic()
    client=None
    try:
        report['stage']='provision'
        admin=os.environ['TEST262_PILOT_ADMIN_DSN']
        repo,producer,dsn=provision(admin)
        client=Client(dsn=dsn,epoch=1)
        os.environ['TEST262_PILOT_CANARY']='must-not-reach-fixtures'
        report['stage']='select-fixtures'
        inventory=catalog.bridge({'command':'inventory','root':str(Path(args.root).resolve())})
        selected=[]
        for row in sorted(inventory,key=lambda r:r['path']):
            if (row['path'].startswith('test/built-ins/Math/abs/') and row['state']=='runnable'
                    and sorted(row['variants'])==['non-strict','strict']):
                normalized=normalize_fixture(row,args.root)
                if normalized['dependency_manifest_digest'] and not normalized['is_support_file']:
                    selected.append(normalized)
            if len(selected)==5:break
        if len(selected)!=5:raise ValueError('Need five pinned Math.abs fixtures with two runnable variants')
        pin=json.loads((REPO/'tests/test262/test262.pin.json').read_text())['upstream']
        actual=subprocess.check_output(['git','-C',args.root,'rev-parse','HEAD'],text=True).strip()
        if actual!=pin['commit']:raise ValueError('Pilot upstream pin mismatch')
        report['stage']='register-and-enqueue'
        corpus,ids=register(client,repo,pin,selected)
        revision=subprocess.check_output(['git','rev-parse','HEAD'],text=True,cwd=REPO).strip()
        entry=Path(args.jroc).resolve()
        document={'pilot':True,'partial_inventory':True,'upstream':pin,'inventory':corpus,
                  'runner':'mvp-composite','binaries':catalog.hash_files(entry.parent,['*.dll','*.deps.json','*.runtimeconfig.json']),
                  'harness':catalog.hash_files(Path(args.root),['harness/**/*']),
                  'environment_identity':catalog.environment(entry)['identity'],
                  'timeouts':{'runtime':30,'compile':60,'cap_ms':120000}}
        pid=provenance(client,repo,corpus,'mvp-composite',document)
        workargs=SimpleNamespace(repository=repo,producer=producer,root=args.root,jroc=str(entry),
                 kind='mvp-composite',image=args.image,run_key='isolated-pilot',cap_ms=120000,
                 runtime_timeout=30,compile_timeout=60,limit=10,seconds=600,outbox=str(output/'outbox.sqlite'))
        workargs.run=start_run(client,workargs,pid,'isolated-pilot',revision,'mvp')
        workers=[workargs]
        if args.workers == 2:
            second=SimpleNamespace(**vars(workargs))
            second.run_key='isolated-pilot-2'
            second.run=start_run(client,second,pid,'isolated-pilot-2',revision,'mvp')
            second.outbox=str(output/'outbox-2.sqlite')
            workargs.limit=second.limit=5
            workers.append(second)
        workargs.budget=str(uuid.uuid4())
        for worker_args in workers:
            worker_args.budget=workargs.budget
        client.put('budget_scopes',[{'budget_scope_id':workargs.budget,'repository_id':repo,'scope_kind':'run',
                    'external_key':'isolated-pilot','candidate_limit':10,'accepted_limit':10,'attempt_limit':10,'time_limit_ms':1200000}])
        client.put('provenance_fixture_eligibility',[{'repository_id':repo,'provenance_id':pid,'fixture_id':f,'eligibility':'runnable','reason_codes':[]} for f in ids.values()])
        workrows=[{'work_item_id':str(uuid.uuid4()),'repository_id':repo,'provenance_id':pid,'fixture_id':ids[r['path']],
                  'variant':v['variant'],'coherent_area':'built-ins/Math/abs','attempt_limit':1} for r in selected for v in r['variants']]
        client.put('work_items',workrows)
        client.put('budget_work_items',[{'repository_id':repo,'budget_scope_id':workargs.budget,'work_item_id':w['work_item_id']} for w in workrows])
        with tempfile.TemporaryDirectory(prefix='pilot-runtime-') as runtime:
            stage_runtime(workargs,runtime);Path(runtime).chmod(0o755);workargs.runtime=runtime
            for worker_args in workers:
                worker_args.runtime=runtime
            report['stage']='isolation-probe'
            report['isolation']=isolation_probe(workargs,Path(args.root).resolve())
            report['stage']='execute-worker'
            if args.workers == 2:
                report['workers']=execute_pair(dsn,workers)
                if (len({w['first_work_item'] for w in report['workers']}) != 2 or
                        any(w['worker']['attempts'] != 5 for w in report['workers']) or
                        'recovery' not in report['workers'][0]):
                    raise ValueError('Concurrent claim/recovery pilot invariant failed')
            else:
                report['worker']=work_staged(client,workargs)
            if args.process_kill:
                from .pilot_process import run_process_drill
                report['stage']='process-kill-drill'
                report['process_kill']={}
                run_process_drill(client,dsn,workargs,corpus,next(iter(ids.values())),revision,
                                  document,output,report['process_kill'])
        report['stage']='verify-results'
        observations=[row for worker_args in workers for row in client.read('observations',{'run_id':worker_args.run})]
        workitems=list(client.read('work_items',{'provenance_id':pid}))
        budget=client.one('budget_scopes',budget_scope_id=workargs.budget)
        # Re-flush acknowledged ingestion/completion messages: no new evidence or budget charge.
        pending=0
        for worker_args in workers:
            outbox=Outbox(worker_args.outbox,repo,producer,1);outbox.flush(client);outbox.flush(client)
            pending+=outbox.db.execute('SELECT count(*) FROM messages WHERE receipt IS NULL').fetchone()[0]+outbox.db.execute('SELECT count(*) FROM completions WHERE receipt IS NULL').fetchone()[0]
            outbox.db.close()
        replay_count=sum(len(list(client.read('observations',{'run_id':a.run}))) for a in workers)
        if args.workers == 2:
            recovered=report['workers'][0]['recovery']['observation_id']
            report['recovered_observation_count']=sum(o['observation_id']==recovered for o in observations)
            if report['recovered_observation_count'] != 1:
                raise ValueError('Recovery duplicated or lost the committed observation')
        report.update(revision=revision,upstream_commit=actual,fixtures=[r['path'] for r in selected],
                      observations=len(observations),outcomes=dict(Counter(o['outcome'] for o in observations)),
                      completed_work=sum(w['state']=='completed' for w in workitems),pending_outbox=pending,
                      replay_observations=replay_count,charged_attempts=budget['charged_attempts'],reserved_attempts=budget['reserved_attempts'])
        if (len(observations)!=10 or replay_count!=10 or report['completed_work']!=10 or pending or
                budget['charged_attempts']!=10 or budget['reserved_attempts'] or budget['reserved_ms'] or
                any(o['outcome'] in ('infrastructure-error','incomplete') for o in observations)):
            raise ValueError('Pilot execution/lease/outbox/budget invariant failed')
        report['complete']=True
        if args.queue_drills:
            from .pilot_drills import run_drills
            report['complete']=False
            report['stage']='queue-control-drills'
            report['queue_drills']={}
            run_drills(client,workargs,corpus,list(ids.values()),revision,report['queue_drills'])
            report['complete']=True
        if args.restore_drill:
            from .pilot_restore import run_restore_drill
            report['complete']=False
            report['stage']='backup-restore-drill'
            report['restore_drill']={}
            run_restore_drill(admin,output,report['restore_drill'])
            report['complete']=True
        report['stage']='verified'
    except Exception as error:
        report['error_type']=type(error).__name__
        raise
    finally:
        if client:client.close()
        report['seconds']=round(time.monotonic()-started,2)
        (output/'pilot-report.json').write_text(json.dumps(report,indent=2)+'\n')
        # The report contains no connection strings, passwords or fixture payloads.
        print(json.dumps(report))
        if os.getenv('GITHUB_STEP_SUMMARY'):
            with open(os.environ['GITHUB_STEP_SUMMARY'],'a') as stream:
                stream.write('## Isolated worker pilot\n\n```json\n'+json.dumps(report,indent=2)+'\n```\n')


if __name__=='__main__':
    p=argparse.ArgumentParser();p.add_argument('--root',required=True);p.add_argument('--output',required=True)
    p.add_argument('--jroc',default='src/Cli/bin/Release/net10.0/Jroc.dll');p.add_argument('--image',default='test262-fixture:pilot')
    p.add_argument('--workers',type=int,choices=(1,2),default=1)
    p.add_argument('--queue-drills',action='store_true')
    p.add_argument('--process-kill',action='store_true')
    p.add_argument('--restore-drill',action='store_true')
    try:
        run(p.parse_args())
    except Exception as error:
        # Driver errors can include connection fragments. Report only the error type.
        import sys
        print('Isolated pilot failed ('+type(error).__name__+'). Inspect pilot-report.json and job steps.', file=sys.stderr)
        raise SystemExit(1)
