"""Ten real fixture variants through the production worker on an empty local database."""
import argparse
from collections import Counter
import json
import os
from pathlib import Path
import subprocess
import tempfile
import time
from types import SimpleNamespace
import uuid

from .client import Client, Outbox, fixture_environment
from .importer import provenance, start_run
from .inventory import REPO, normalize_fixture, register
from .worker import container_arguments, stage_runtime, work_staged


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
              'kind':'mvp-composite','variant_limit':10}
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
        workargs.budget=str(uuid.uuid4())
        client.put('budget_scopes',[{'budget_scope_id':workargs.budget,'repository_id':repo,'scope_kind':'run',
                    'external_key':'isolated-pilot','candidate_limit':10,'accepted_limit':10,'attempt_limit':10,'time_limit_ms':1200000}])
        client.put('provenance_fixture_eligibility',[{'repository_id':repo,'provenance_id':pid,'fixture_id':f,'eligibility':'runnable','reason_codes':[]} for f in ids.values()])
        workrows=[{'work_item_id':str(uuid.uuid4()),'repository_id':repo,'provenance_id':pid,'fixture_id':ids[r['path']],
                  'variant':v['variant'],'coherent_area':'built-ins/Math/abs','attempt_limit':1} for r in selected for v in r['variants']]
        client.put('work_items',workrows)
        client.put('budget_work_items',[{'repository_id':repo,'budget_scope_id':workargs.budget,'work_item_id':w['work_item_id']} for w in workrows])
        with tempfile.TemporaryDirectory(prefix='pilot-runtime-') as runtime:
            stage_runtime(workargs,runtime);Path(runtime).chmod(0o755);workargs.runtime=runtime
            report['stage']='isolation-probe'
            report['isolation']=isolation_probe(workargs,Path(args.root).resolve())
            report['stage']='execute-worker'
            report['worker']=work_staged(client,workargs)
        report['stage']='verify-results'
        observations=list(client.read('observations',{'run_id':workargs.run}))
        workitems=list(client.read('work_items',{'provenance_id':pid}))
        budget=client.one('budget_scopes',budget_scope_id=workargs.budget)
        # Re-flush acknowledged ingestion/completion messages: no new evidence or budget charge.
        outbox=Outbox(workargs.outbox,repo,producer,1);outbox.flush(client);outbox.flush(client)
        pending=outbox.db.execute('SELECT count(*) FROM messages WHERE receipt IS NULL').fetchone()[0]+outbox.db.execute('SELECT count(*) FROM completions WHERE receipt IS NULL').fetchone()[0]
        outbox.db.close()
        replay_count=len(list(client.read('observations',{'run_id':workargs.run})))
        report.update(revision=revision,upstream_commit=actual,fixtures=[r['path'] for r in selected],
                      observations=len(observations),outcomes=dict(Counter(o['outcome'] for o in observations)),
                      completed_work=sum(w['state']=='completed' for w in workitems),pending_outbox=pending,
                      replay_observations=replay_count,charged_attempts=budget['charged_attempts'],reserved_attempts=budget['reserved_attempts'])
        if (len(observations)!=10 or replay_count!=10 or report['completed_work']!=10 or pending or
                budget['charged_attempts']!=10 or budget['reserved_attempts'] or budget['reserved_ms'] or
                any(o['outcome'] in ('infrastructure-error','incomplete') for o in observations)):
            raise ValueError('Pilot execution/lease/outbox/budget invariant failed')
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
    try:
        run(p.parse_args())
    except Exception as error:
        # Driver errors can include connection fragments. Report only the error type.
        import sys
        print('Isolated pilot failed ('+type(error).__name__+'). Inspect pilot-report.json and job steps.', file=sys.stderr)
        raise SystemExit(1)
