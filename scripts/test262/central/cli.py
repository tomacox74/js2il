"""python -m scripts.test262.central.cli --help"""
import argparse
import json
import os
from pathlib import Path
from .client import Client, canonical, Outbox
from .diagnostics import report_failure, stage


def parser():
    p=argparse.ArgumentParser(description='Scoped central Test262 catalogue supervisor')
    p.add_argument('--repository',default=os.getenv('TEST262_REPOSITORY_ID'))
    p.add_argument('--producer',default=os.getenv('TEST262_PRODUCER_ID'))
    sub=p.add_subparsers(dest='command',required=True)
    i=sub.add_parser('import')
    i.add_argument('source'); i.add_argument('--source-uri'); i.add_argument('--root')
    i.add_argument('--missing-artifacts',action='append',default=[])
    i.add_argument('--outbox',required=True)
    e=sub.add_parser('export'); e.add_argument('--output',required=True)
    e.add_argument('--seconds', type=int, help='Total export deadline; incomplete output stays .partial')
    enrichment=sub.add_parser('enrich-specification-metadata',
                              help='Append source-verified specification metadata to an existing corpus')
    enrichment.add_argument('--corpus',required=True)
    enrichment.add_argument('--root',required=True)
    f=sub.add_parser('flush'); f.add_argument('--outbox',required=True)
    f=sub.add_parser('finalize',help='Finalize collection after durable outbox flush')
    f.add_argument('--context',required=True); f.add_argument('--outbox',required=True)
    f.add_argument('--run-key',required=True)
    f.add_argument('--conclusion',choices=['success','failure','cancelled'],required=True)
    f.add_argument('--flush-verified',action='store_true')
    r=sub.add_parser('reconcile-runs',help='Inspect exact Actions attempts; dry-run unless --apply')
    r.add_argument('--repository-name',default='tomacox74/js2il')
    r.add_argument('--limit',type=int,default=100)
    r.add_argument('--actions-run-id',type=int)
    r.add_argument('--actions-run-attempt',type=int)
    r.add_argument('--apply',action='store_true')
    b=sub.add_parser('reconcile-budgets',help='Inspect settled terminal run budgets; dry-run unless --apply')
    b.add_argument('--limit',type=int,default=100)
    b.add_argument('--actions-run-id',type=int)
    b.add_argument('--apply',action='store_true')
    publication=sub.add_parser('reconcile-publications',help='Reconcile existing reservations only; never publishes')
    publication.add_argument('--repository-name',default='tomacox74/js2il')
    publication.add_argument('--limit',type=int,default=100)
    publication.add_argument('--pr-number',type=int)
    publication.add_argument('--apply',action='store_true')
    q=sub.add_parser('prepare')
    q.add_argument('--kind',choices=['mvp-composite','native'],required=True)
    q.add_argument('--repository-name',default='tomacox74/js2il')
    q.add_argument('--revision',required=True); q.add_argument('--root',required=True)
    q.add_argument('--jroc',default='src/Cli/bin/Release/net10.0/Jroc.dll')
    q.add_argument('--host',default='scripts/test262/NativeScreeningHost/bin/Release/net10.0/NativeScreeningHost.dll')
    q.add_argument('--run-key',required=True); q.add_argument('--budget-key',required=True)
    q.add_argument('--candidate-limit',type=int,default=200); q.add_argument('--accepted-limit',type=int,default=100)
    q.add_argument('--attempt-limit',type=int,default=400); q.add_argument('--budget-ms',type=int,default=1200000)
    q.add_argument('--cap-ms',type=int,default=120000); q.add_argument('--runtime-timeout',type=int,default=30)
    q.add_argument('--compile-timeout',type=int,default=60); q.add_argument('--area',default='')
    q.add_argument('--compilation-coverage',action='store_true')
    q.add_argument('--output',required=True)
    refresh=sub.add_parser('refresh',help='Refresh validated master registrations without screening/publication')
    refresh.add_argument('--repository-name',default='tomacox74/js2il')
    refresh.add_argument('--revision',required=True); refresh.add_argument('--root',required=True)
    refresh.add_argument('--host',default='scripts/test262/NativeScreeningHost/bin/Release/net10.0/NativeScreeningHost.dll')
    refresh.add_argument('--runtime-timeout',type=int,default=30)
    refresh.add_argument('--compile-timeout',type=int,default=60); refresh.add_argument('--cap-ms',type=int,default=120000)
    refresh.add_argument('--output',required=True)
    w=sub.add_parser('work'); w.add_argument('--context',required=True); w.add_argument('--root',required=True)
    w.add_argument('--outbox',required=True); w.add_argument('--image',default='test262-fixture:local')
    w.add_argument('--limit',type=int,default=100); w.add_argument('--seconds',type=int,default=600)
    w.add_argument('--output',help='Retain the bounded worker stop/budget receipt')
    w.add_argument('--jroc',default='src/Cli/bin/Release/net10.0/Jroc.dll')
    w.add_argument('--host',default='scripts/test262/NativeScreeningHost/bin/Release/net10.0/NativeScreeningHost.dll')
    enlist=sub.add_parser('enlist',help='Join a prepared shared allocation from a verified independent checkout')
    enlist.add_argument('--context',required=True); enlist.add_argument('--root',required=True)
    enlist.add_argument('--run-key',required=True); enlist.add_argument('--outbox',required=True)
    enlist.add_argument('--image',default='test262-fixture:local')
    enlist.add_argument('--limit',type=int,default=100); enlist.add_argument('--seconds',type=int,default=600)
    enlist.add_argument('--output')
    enlist.add_argument('--replay-only',action='store_true',help='Replay an existing outbox and end an interrupted collection; no execution')
    enlist.add_argument('--jroc',default='src/Cli/bin/Release/net10.0/Jroc.dll')
    enlist.add_argument('--host',default='scripts/test262/NativeScreeningHost/bin/Release/net10.0/NativeScreeningHost.dll')
    s=sub.add_parser('seal'); s.add_argument('--context',required=True); s.add_argument('--batch-key',required=True); s.add_argument('--output',required=True)
    g=sub.add_parser('generate'); g.add_argument('--batch-file',required=True); g.add_argument('--cache',required=True); g.add_argument('--root',required=True); g.add_argument('--output',required=True)
    pub=sub.add_parser('publish'); pub.add_argument('--batch-file',required=True); pub.add_argument('--patch',required=True)
    pub.add_argument('--manifest',required=True); pub.add_argument('--body',required=True)
    pub.add_argument('--repository-name',default='tomacox74/js2il')
    return p


def main(argv=None):
    args=parser().parse_args(argv)
    if not args.repository or not args.producer:
        raise ValueError('Explicit repository/producer identities required')
    with stage('connect'):
        client=Client()
    try:
        if client.contract['repository_id']!=args.repository or client.contract['producer_id']!=args.producer:
            raise ValueError('Configured identities do not match authenticated database subject')
        if args.command=='export':
            result=client.export(args.output, seconds=args.seconds)
        elif args.command=='enrich-specification-metadata':
            from .inventory import enrich_specification_metadata
            result=enrich_specification_metadata(client,args)
        elif args.command=='flush':
            outbox=Outbox(args.outbox,args.repository,args.producer,client.epoch)
            try:
                outbox.flush(client)
            finally:
                outbox.db.close()
            result={'flushed':True}
        elif args.command=='enlist':
            from .enlist import run as enlist
            result=enlist(client,args)
        elif args.command in ('finalize','reconcile-runs'):
            from . import lifecycle
            result=getattr(lifecycle,args.command.replace('-','_'))(client,args)
        elif args.command=='reconcile-budgets':
            from .budgets import reconcile_budgets
            result=reconcile_budgets(client,args)
        elif args.command=='reconcile-publications':
            from .publication import reconcile_publications
            result=reconcile_publications(client,args.repository_name,limit=args.limit,
                                          pr_number=args.pr_number,apply=args.apply)
        elif args.command=='publish':
            from .publication import publish
            result=publish(client,args)
        elif args.command=='import':
            from .importer import import_snapshot
            if client.contract['deployment_state']=='active':
                raise ValueError('Historical import requires shadow authority; avoid mixing importer with active execution')
            result=import_snapshot(client,args)
        else:
            from . import commands
            if args.command=='work':
                from .worker import work
                context=json.loads(Path(args.context).read_text())
                for field in ('kind','run','budget','cap_ms'):
                    setattr(args,field,context[field])
                args.runtime_timeout=context['identity']['timeouts']['runtime']
                args.compile_timeout=context['identity']['timeouts']['compile']
                args.compilation_coverage=context.get('compilation_coverage',False)
                if args.repository!=context['repository'] or args.producer!=context['producer']:
                    raise ValueError('Context belongs to another supervisor')
                result=work(client,args)
                if args.output:
                    Path(args.output).parent.mkdir(parents=True,exist_ok=True)
                    Path(args.output).write_text(canonical(result)+'\n')
            else:
                result=getattr(commands,args.command)(client,args)
        print(canonical(result))
    except Exception as error:
        if not getattr(error, '_test262_stage', None):
            error._test262_stage = args.command
        raise
    finally:
        client.close()


def run(argv=None):
    try:
        main(argv)
        return 0
    except Exception as error:
        report_failure(error)
        return 1


if __name__=='__main__':
    raise SystemExit(run())
