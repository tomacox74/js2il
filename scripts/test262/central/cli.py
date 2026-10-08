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
    f=sub.add_parser('flush'); f.add_argument('--outbox',required=True)
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
    w.add_argument('--jroc',default='src/Cli/bin/Release/net10.0/Jroc.dll')
    w.add_argument('--host',default='scripts/test262/NativeScreeningHost/bin/Release/net10.0/NativeScreeningHost.dll')
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
        elif args.command=='flush':
            Outbox(args.outbox,args.repository,args.producer,client.epoch).flush(client)
            result={'flushed':True}
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

