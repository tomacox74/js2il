"""python -m scripts.test262.central.cli --help"""
import argparse
import json
import os
from pathlib import Path
import sys
from .client import Client, canonical, Outbox


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
    q.add_argument('--output',required=True)
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


def main():
    args=parser().parse_args()
    if not args.repository or not args.producer:
        raise ValueError('Explicit repository/producer identities required')
    client=Client()
    try:
        if client.contract['repository_id']!=args.repository or client.contract['producer_id']!=args.producer:
            raise ValueError('Configured identities do not match authenticated database subject')
        if args.command=='export':
            result=client.export(args.output)
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
                if args.repository!=context['repository'] or args.producer!=context['producer']:
                    raise ValueError('Context belongs to another supervisor')
                result=work(client,args)
            else:
                result=getattr(commands,args.command)(client,args)
        print(canonical(result))
    finally:
        client.close()


if __name__=='__main__':
    # Never echo a DSN or connection exception containing credentials into Actions logs.
    try:
        main()
    except Exception as error:
        print('Catalogue operation failed ('+type(error).__name__+'). Inspect scoped database/audit logs.',file=sys.stderr)
        raise SystemExit(1)
