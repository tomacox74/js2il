"""Real native variants and production planning, only in the empty loopback pilot.

The weekly schedule input is simulated, not a claim that GitHub's cron fired.
Inventory/registrations and successful master validation are controlled disposable
fixtures; compiler, harness, pin, claims, execution, evidence and budgets are real.
"""
import json
from pathlib import Path
import subprocess
import tempfile
from types import SimpleNamespace
from unittest.mock import patch

from . import commands, lifecycle
from .client import Outbox, bytea, fixture_environment, identity, sha
from .importer import provenance
from .inventory import REPO, register
from .pilot_drills import require
from .pilot_process import validate_connection
from .schedule import collection_kind
from .worker import stage_runtime, work_staged


class LostFinalizationResponse(TimeoutError):
    pass


class InterruptOnce:
    def __init__(self, client, operation):
        self.client=client; self.operation=operation; self.interrupted=False

    def __getattr__(self, name):
        return getattr(self.client,name)

    def call(self, operation, *args):
        result=self.client.call(operation,*args)
        if operation==self.operation and not self.interrupted:
            self.interrupted=True
            raise LostFinalizationResponse('Disposable committed operation lost its response')
        return result


def native_identity(args, corpus, pin, scenario):
    import sys
    sys.path.insert(0,str(REPO/'scripts/test262'))
    import catalog
    entry=Path(args.host).resolve();root=Path(args.root).resolve()
    return {'pilot':True,'partial_inventory':True,'scheduled_recovery_drill':scenario,
        'runner':'native','upstream':pin,'inventory':corpus,
        'binaries':catalog.hash_files(entry.parent,['*.dll','*.deps.json','*.runtimeconfig.json']),
        'harness':catalog.hash_files(root,['harness/**/*']),
        'tooling':catalog.hash_files(REPO,['scripts/test262/**/*.py','scripts/test262/**/*.js',
            'scripts/test262/NativeScreeningHost/**/*.cs','tests/Jroc.Testing/**/*.cs','tests/test262/test262.pin.json']),
        'capabilities':json.loads(subprocess.check_output(['dotnet',str(entry),'--capabilities'],
            text=True,env=fixture_environment(),timeout=30)),
        'environment_identity':catalog.environment(entry)['identity'],
        'timeouts':{'runtime':30,'compile':60,'cap_ms':120000}}


def run_drill(client, dsn, base, fixture, revision, output, report):
    validate_connection(dsn)  # Reject production/remote before any write.
    output=Path(output);output.mkdir(parents=True,exist_ok=True)
    report.update(complete=False,scope='disposable loopback catalogue only',
        trigger='simulated schedule input through production collection_kind',
        github_cron_observed=False,production_cutover=False,
        validation='synthetic disposable master validation; not production acceptance',
        registration_inventory='controlled empty disposable snapshot',scenarios={})
    require(collection_kind('schedule')=='native','Scheduled recovery must select native')
    require(sorted(v['variant'] for v in fixture['variants'])==['non-strict','strict'],
            'Recovery drill requires both real pinned variants')
    pin=json.loads((REPO/'tests/test262/test262.pin.json').read_text())['upstream']
    require(subprocess.check_output(['git','-C',base.root,'rev-parse','HEAD'],text=True).strip()==pin['commit'],
            'Recovery upstream revision differs from pin')
    corpus,ids=register(client,base.repository,pin,[fixture])
    snapshot=identity(base.repository,'scheduled-recovery-empty-registrations',revision)
    client.put('registration_snapshots',[{'snapshot_id':snapshot,'repository_id':base.repository,
        'source_revision':revision,'inventory_sha256':bytea(sha([])),
        'warnings':['Controlled empty registration set for disposable recovery drill']}])
    client.call('transition','registration_snapshots',{'snapshot_id':snapshot},0,{'state':'sealed'})
    validation_proof={'id':2340,'head_sha':revision,'head_branch':'master','event':'push',
        'conclusion':'success','synthetic_disposable_validation':True}
    for scenario,attempt_limit,budget_ms,interrupted_op in (
            ('attempt',1,240000,'finish_run'),('time',10,120000,'finish_budget')):
        folder=output/scenario;folder.mkdir(exist_ok=True)
        args=SimpleNamespace(**vars(base))
        args.kind=collection_kind('schedule');args.revision=revision
        args.repository_name='disposable/worker-pilot';args.area='built-ins/Math/abs'
        args.candidate_limit=args.accepted_limit=1
        args.attempt_limit=attempt_limit;args.budget_ms=budget_ms
        args.cap_ms=120000;args.runtime_timeout=30;args.compile_timeout=60
        args.limit=10;args.seconds=600;args.compilation_coverage=False
        original_doc=native_identity(args,corpus,pin,scenario)
        pid=provenance(client,args.repository,corpus,'native',original_doc)
        fid=ids[fixture['path']]
        eligibility=[{'repository_id':args.repository,'provenance_id':pid,'fixture_id':fid,
                      'eligibility':'runnable','reason_codes':[]}]
        client.put('provenance_fixture_eligibility',eligibility)
        prepared=dict(source=revision,corpus=corpus,ids=ids,snapshot=snapshot,pid=pid,
                      doc=original_doc,normalized=[fixture],eligibility=eligibility)
        scenario_report={'source_revision':revision,'upstream':pin,'provenance_id':pid,
                         'identity':original_doc,'fixture':fixture['path'],'invocations':[]}
        report['scenarios'][scenario]=scenario_report
        with tempfile.TemporaryDirectory(prefix='native-recovery-runtime-') as runtime:
            stage_runtime(args,runtime);Path(runtime).chmod(0o755);args.runtime=runtime
            first_budget=None;first_observation=None;cursor_after_first=None
            # Each event is a new scheduled allocation; retries of an event are
            # never substituted with a new budget identity.
            for index in range(3):
                require(native_identity(args,corpus,pin,scenario)==original_doc,
                        'Recovery binary/harness/tooling/environment identity changed')
                event_id=(2340000 if scenario=='attempt' else 2340100)+index
                args.run_key=f'actions:{event_id}:1';args.budget_key=f'actions:{event_id}'
                args.output=str(folder/f'context-{index}.json')
                with patch.object(commands,'github_validation',return_value=validation_proof), \
                     patch.object(commands,'current_master',return_value=revision):
                    context=commands.prepare_from_inventory(client,args,prepared)
                args.run=context['run'];args.budget=context['budget']
                args.outbox=str(folder/f'outbox-{index}.sqlite')
                before_seal=commands.seal(client,SimpleNamespace(context=args.output,repository=args.repository,
                    batch_key=f'before-{event_id}',output=str(folder/f'before-batch-{index}.json')))
                require(before_seal['accepted']==0,'Incomplete/no-op recovery was accepted before execution')
                worker=work_staged(client,args)
                if index==0:
                    require(worker['attempts']==1 and worker['stop_reason']=='budget-exhausted',
                            'First invocation must exhaust the allocation, not its worker limits')
                elif index==1:
                    require(context['recovery']['resumed_fixtures']==1 and context['recovery']['queued_variants']==1 and
                            context['recovery']['reused_terminal_variants']==1 and worker['attempts']==1,
                            'Scheduled recovery repeated a completed variant or lost the missing variant')
                    require(context['recovery']['cursor_before']==cursor_after_first==context['recovery']['cursor_after'],
                            'Recovery-only planning advanced discovery cursor')
                else:
                    require(worker['attempts']==0 and context['candidate_ids']==[] and
                            context['recovery']['cursor_before']==context['recovery']['cursor_after'],
                            'Repeated scheduling executed completed work or moved discovery cursor')
                finish_args=SimpleNamespace(repository=args.repository,producer=args.producer,context=args.output,
                    outbox=args.outbox,run_key=args.run_key,conclusion='success',flush_verified=True)
                transport=InterruptOnce(client,interrupted_op) if index==0 else client
                try:
                    receipt=lifecycle.finalize(transport,finish_args)
                except LostFinalizationResponse:
                    receipt=lifecycle.finalize(client,finish_args)
                budget=client.one('budget_scopes',budget_scope_id=args.budget)
                require(receipt['budget']['result']==budget,'Lifecycle receipt differs from stored budget')
                require(client.call('finish_budget',args.budget,True)['result']==budget,
                        'Repeated finalization changed budget timestamp/charges')
                require(budget['reserved_attempts']==budget['reserved_ms']==0,'Reservation leaked')
                outbox=Outbox(args.outbox,args.repository,args.producer,client.epoch)
                try:
                    outbox.flush(client);outbox.flush(client)
                    pending=sum(outbox.db.execute(f'SELECT count(*) FROM {t} WHERE receipt IS NULL').fetchone()[0]
                                for t in ('messages','completions'))
                finally:
                    outbox.db.close()
                require(pending==0,'Recovery left an unacknowledged outbox')
                observations=list(client.read('observations',{'provenance_id':pid}))
                require(all(o['outcome']=='pass' for o in observations),'Native fixture did not pass')
                work=list(client.read('work_items',{'provenance_id':pid}))
                if index==0:
                    require(budget['state']=='exhausted' and len(observations)==1 and
                            sum(w['state']=='pending' for w in work)==1,'Exhaustion lost unfinished required work')
                    first_budget=budget;first_observation=observations[0]
                    cursor_after_first=context['recovery']['cursor_after']
                    require(receipt['state']=='complete','Collection completion must remain separate from budget exhaustion')
                else:
                    require(client.one('budget_scopes',budget_scope_id=first_budget['budget_scope_id'])==first_budget,
                            'New allocation reset or recharged exhausted original budget')
                    require(len(observations)==2 and len({o['variant'] for o in observations})==2 and
                            client.one('observations',observation_id=first_observation['observation_id'])==first_observation,
                            'Recovery lost/replaced/duplicated original variant evidence')
                    require(budget['state']=='closed','Recovered/no-op scope did not drain')
                if index==1:
                    sealed=commands.seal(client,SimpleNamespace(context=args.output,repository=args.repository,
                        batch_key=f'after-{event_id}',output=str(folder/f'batch-{index}.json')))
                    require(sealed['accepted']==1,'Complete same-provenance variants did not form one acceptance')
                    bindings=list(client.read('batch_evidence',{'batch_id':sealed['batch']}))
                    require(len(bindings)==2 and len({o['run_id'] for o in observations})==2,
                            'Acceptance did not reuse evidence across the two exact-provenance runs')
                    scenario_report['accepted_batch']=sealed
                scenario_report['invocations'].append({'event':args.run_key,'run_id':args.run,'budget':budget,
                    'recovery':context['recovery'],'worker':worker,'lifecycle':receipt,
                    'observations':[{k:o[k] for k in ('observation_id','run_id','provenance_id','fixture_id','variant','outcome','lease_generation')} for o in observations],
                    'work':[{k:w[k] for k in ('work_item_id','fixture_id','variant','state','lease_generation')} for w in work],
                    'pending_outbox':pending})
        scenario_report['interrupted_operation']=interrupted_op
        scenario_report['complete']=True
    report['complete']=True
    (output/'scheduled-recovery-report.json').write_text(json.dumps(report,indent=2)+'\n')
    return report
