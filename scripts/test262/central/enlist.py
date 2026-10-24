"""Join a coordinator-prepared shared allocation from a verified independent checkout.

Cold starts fail closed without central access. A disconnected in-flight result
stays in the existing durable outbox; `flush` replays it without claiming work.
"""
import json
from pathlib import Path
import re
import subprocess
import time
from .client import canonical, identity, Outbox
from .execution_identity import execution_identity
from .importer import start_run, timestamp
from .inventory import REPO
from .worker import work


def require(condition, message):
    if not condition:
        raise ValueError(message)


def check_scope(client, args, context):
    c=client.contract
    require(c['repository_id']==args.repository==context['repository'] and
            c['producer_id']==args.producer and c['permission']=='coordinator' and
            c['trust_class']=='trusted' and c['deployment_state']=='active',
            'Approved active repository-bound worker identity required')
    require(context['kind'] in ('native','mvp-composite'), 'Unsupported execution kind')
    owner=client.one('runs',run_id=context['run'])
    require(owner and owner['trust_class']=='trusted' and owner['producer_id']==context['producer'] and
            owner['source_revision']==context['revision'] and owner['provenance_id']==context['provenance'],
            'Context does not match the central preparation run')
    provenance=client.one('provenances',provenance_id=context['provenance'])
    require(provenance and provenance['corpus_id']==context['corpus'] and
            provenance['evidence_kind']==context['kind'] and
            provenance['identity_document']==context['identity'], 'Central provenance differs from context')
    target=client.one('reporting_targets',channel='master',evidence_kind=context['kind'])
    require(target and target['provenance_id']==context['provenance'] and
            target['registration_snapshot_id']==context['registration_snapshot'] and
            target.get('validation_id')==context.get('validation'), 'Prepared target is stale')
    snapshot=client.one('registration_snapshots',snapshot_id=context['registration_snapshot'])
    require(snapshot and snapshot['state']=='sealed' and snapshot['source_revision']==context['revision'],
            'Registration snapshot is not sealed at this source revision')
    if context['kind']=='native':
        validation=client.one('validation_events',validation_id=context['validation']) if context.get('validation') else None
        require(validation and validation['conclusion']=='success' and validation['target_revision']==context['revision'],
                'Native enlistment requires matching successful central validation')
    budget=client.one('budget_scopes',budget_scope_id=context['budget'])
    require(budget and budget['state']=='open' and not re.fullmatch(r'actions:[1-9][0-9]*',budget['external_key']),
            'Independent workers require an open non-Actions shared allocation')
    require(context['cap_ms']==context['identity']['timeouts']['cap_ms'], 'Execution cap differs from identity')
    return budget


def check_checkout(args, context):
    def git(root,*command):
        return subprocess.check_output(['git','-C',str(root),*command],text=True).strip()
    require(git(REPO,'rev-parse','HEAD')==context['revision'] and
            not git(REPO,'status','--porcelain','--untracked-files=no'),
            'Enlistment requires the exact clean source revision')
    upstream=context['identity']['upstream']['commit']
    require(git(args.root,'rev-parse','HEAD')==upstream and
            not git(args.root,'status','--porcelain','--untracked-files=no'),
            'Pinned upstream checkout differs or has tracked edits')
    actual=execution_identity(args,context['corpus'])
    require(actual==context['identity'], 'Build/harness/tooling/pin/environment identity differs')
    return actual


class CurrentTarget:
    def __init__(self,client,args,context):
        self.client,self.args,self.context=client,args,context

    def __getattr__(self,name):
        return getattr(self.client,name)

    def call(self,operation,*values):
        # Recheck central freshness immediately before every new claim. Results
        # already executed retain their original provenance when replayed.
        if operation=='claim':
            check_scope(self.client,self.args,self.context)
        return self.client.call(operation,*values)


def run(client,args):
    context=json.loads(Path(args.context).read_text())
    replay_only=getattr(args,'replay_only',False)
    require(replay_only or (args.limit>0 and args.seconds>0), 'Enlistment must have positive execution bounds')
    require(re.fullmatch(r'[A-Za-z0-9_.:-]{1,160}',args.run_key) and
            not args.run_key.startswith('actions:'), 'Use a stable non-Actions local/downstream run key')
    for field in ('kind','budget','cap_ms'):
        setattr(args,field,context[field])
    args.runtime_timeout=context['identity']['timeouts']['runtime']
    args.compile_timeout=context['identity']['timeouts']['compile']
    args.compilation_coverage=context.get('compilation_coverage',False)
    # Producer identity participates in both run and external key namespaces.
    key=f'worker:{args.producer}:{args.run_key}'
    args.run=identity(args.repository,args.producer,key)
    if replay_only:
        result=replay(client,args,context,key)
        if args.output:
            Path(args.output).parent.mkdir(parents=True,exist_ok=True)
            Path(args.output).write_text(canonical(result)+'\n')
        return result
    check_scope(client,args,context)
    check_checkout(args,context)
    old=client.one('runs',run_id=args.run)
    require(not old or old['state']=='running', 'Collection already ended; flush its outbox or use a new run key')
    args.run=start_run(client,args,context['provenance'],key,context['revision'],
                       'native' if args.kind=='native' else 'mvp')
    result=work(CurrentTarget(client,args,context),args)
    outbox=Outbox(args.outbox,args.repository,args.producer,client.epoch)
    try:
        outbox.flush(client)
    finally:
        outbox.db.close()
    ended=client.call('finish_run',args.run,'complete',timestamp(time.time()),
        {'source':'supervisor','external_run_key':key,'source_revision':context['revision'],
         'reason':'collection-finished','flush_verified':True})
    result.update(producer_id=args.producer,source_revision=context['revision'],
        provenance_id=context['provenance'],collection_state=ended['state'],
        suite_completion_claimed=False,publication_eligible=False)
    if args.output:
        Path(args.output).parent.mkdir(parents=True,exist_ok=True)
        Path(args.output).write_text(canonical(result)+'\n')
    return result


def replay(client,args,context,key):
    c=client.contract
    require(c['repository_id']==args.repository==context['repository'] and c['producer_id']==args.producer,
            'Replay identity differs from authenticated worker')
    old=client.one('runs',run_id=args.run)
    require(old and old['producer_id']==args.producer and old['source_revision']==context['revision'] and
            old['provenance_id']==context['provenance'] and old['external_run_key']==key,
            'Replay requires the original central run identity')
    require(Path(args.outbox).is_file(), 'Replay requires an existing durable outbox')
    outbox=Outbox(args.outbox,args.repository,args.producer,client.epoch)
    try:
        outbox.flush(client)
    finally:
        outbox.db.close()
    ended=old
    if old['state']=='running':
        ended=client.call('finish_run',args.run,'incomplete',timestamp(time.time()),
            {'source':'supervisor','external_run_key':key,'source_revision':context['revision'],
             'reason':'job-interrupted','flush_verified':True})
    return {'run_id':args.run,'new_attempts':0,'replayed':True,'collection_state':ended['state'],
            'source_revision':context['revision'],'provenance_id':context['provenance'],
            'suite_completion_claimed':False,'publication_eligible':False}
