"""Claim one variant, execute without database credentials, persist, then acknowledge.

Fixtures execute in a fresh container with no host PID/network namespace or secret
mounts. A child-environment allowlist alone is insufficient isolation from /proc.
"""
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import subprocess
import tempfile
import time
import uuid
from .client import canonical, fixture_environment, Outbox
from .importer import timestamp
from .inventory import REPO


def run_container(args, fixture, variant, cap_ms, manifest, work):
    root = Path(args.root).resolve()
    for dependency in manifest.get('dependencies', []):
        path = root / dependency['dependency_path']
        if not path.is_file() or hashlib.sha256(path.read_bytes()).hexdigest() != dependency['content_sha256']:
            raise ValueError('Dependency changed: ' + dependency['dependency_path'])
    name = 'test262-' + uuid.uuid4().hex
    common = ['docker','run','--name',name,'--rm','--network','none','--cap-drop','ALL',
              '--pids-limit','512','--security-opt','no-new-privileges','--read-only',
              '--tmpfs','/tmp:rw,nosuid,size=256m','--mount','type=bind,src='+str(REPO)+',dst=/repo,readonly',
              '--mount','type=bind,src='+str(root)+',dst=/upstream,readonly',
              '--mount','type=bind,src='+str(work)+',dst=/work',args.image]
    if args.kind == 'mvp-composite':
        jroc = '/repo/' + Path(args.jroc).resolve().relative_to(REPO).as_posix()
        request = {'root':'/upstream','output':'/work/case','fixture':fixture['upstream_path'],'variant':variant,
                   'jroc':jroc,'timeout':args.runtime_timeout,'compileTimeout':args.compile_timeout}
        command = common[:2] + ['-i'] + common[2:] + ['node','/repo/scripts/test262/catalogBridge.js']
        stdin = canonical(request)
    else:
        host = '/repo/' + Path(args.host).resolve().relative_to(REPO).as_posix()
        plan = {'upstream_root':'/upstream','timeout_ms':max(1,cap_ms-10000),'variant_limit':1,
                'time_limit_seconds':max(1,cap_ms//1000),
                'candidates':[{'path':fixture['upstream_path'],'sha256':fixture['content_sha256'][2:],'variants':[variant]}]}
        (work/'plan.json').write_text(canonical(plan))
        command = common + ['dotnet',host,'--plan','/work/plan.json']
        stdin = None
    try:
        result = subprocess.run(command,input=stdin,text=True,capture_output=True,timeout=cap_ms/1000,
                                env=fixture_environment(),cwd=REPO)
        if result.returncode:
            raise RuntimeError(result.stderr[-8000:])
        output = json.loads(result.stdout)
        return output
    finally:
        # subprocess timeout only kills docker CLI; explicitly terminate its container tree.
        subprocess.run(['docker','rm','-f',name],capture_output=True,env=fixture_environment(),timeout=30)


def work(client, args):
    snapshot = client.snapshot()['tables']
    fixtures = {f['fixture_id']: f for f in snapshot['fixtures']}
    dependencies = {}
    for row in snapshot['fixture_dependencies']:
        dependencies.setdefault(row['fixture_id'], []).append({'dependency_path':row['dependency_path'],'content_sha256':row['content_sha256'][2:]})
    outbox = Outbox(args.outbox,args.repository,args.producer,client.epoch)
    outbox.flush(client)
    started = time.monotonic()
    count = 0
    timeouts = 0
    cap = args.cap_ms
    if args.kind == 'mvp-composite' and cap < (args.runtime_timeout+args.compile_timeout+30)*1000:
        raise ValueError('Execution cap must cover compile/runtime and 30s startup grace')
    while count<args.limit and time.monotonic()-started<args.seconds:
        lease = client.call('claim',args.run,args.budget,cap,max(31,(cap+999)//1000+30))
        if lease is None or lease.get('budget_exhausted'):
            break
        fixture = fixtures[lease['fixture_id']]
        if hashlib.sha256((Path(args.root)/fixture['upstream_path']).read_bytes()).hexdigest() != fixture['content_sha256'][2:]:
            raise ValueError('Fixture bytes differ from sealed inventory')
        begin = time.time()
        tick = time.monotonic()
        try:
            with tempfile.TemporaryDirectory(prefix='fixture-') as directory:
                workdir = Path(directory)
                workdir.chmod(0o777)
                # Permit traversal to the one isolated scratch directory, not a credentials directory.
                value = run_container(args,fixture,lease['variant'],cap,{'dependencies':dependencies.get(fixture['fixture_id'],[])},workdir)
            if args.kind=='mvp-composite':
                cls = value['classification']
                matched = cls['verdict']=='matched'
                outcome, failure = ('pass',None) if matched else ('fail','unresolved')
                phase = 'execution'
                observed_type = None
            else:
                outcome, failure = value['outcome'],value.get('failure_class')
                phase = value.get('phase','unknown')
                observed_type = value.get('observed_error_type')
        except (RuntimeError,ValueError,subprocess.TimeoutExpired) as error:
            value = {'diagnostic':str(error)[-8000:]}
            outcome,failure,phase,observed_type = 'infrastructure-error','infrastructure-error','timeout' if isinstance(error,subprocess.TimeoutExpired) else 'unknown',None
        finish = time.time()
        if phase not in ('load','parse','early','resolution','compile','runtime','execution','timeout','planning','unknown'):
            phase='unknown'
        row = {'observation_id':str(uuid.uuid4()),'repository_id':args.repository,'run_id':args.run,
               'provenance_id':lease['provenance_id'],'fixture_id':lease['fixture_id'],'variant':lease['variant'],
               'work_item_id':lease['work_item_id'],'lease_generation':lease['lease_generation'],'outcome':outcome,
               'failure_class':failure,'phase':phase,'observed_error_type':observed_type,
               'diagnostic_summary':value.get('diagnostic',value.get('detail',''))[-8000:],
               'started_at':timestamp(begin),'finished_at':timestamp(finish),
               'active_ms':int((time.monotonic()-tick)*1000),'source_kind':'live','payload':value}
        if phase=='timeout':
            timeouts+=1
        outbox.enqueue([row])
        outbox.flush(client)
        count+=1
        print(canonical({'completed':count,'path':fixture['upstream_path'],'variant':lease['variant'],'outcome':outcome}),flush=True)
    client.call('metric',args.run,'screening',1,int((time.monotonic()-started)*1000),count,timeouts)
    return {'attempts':count,'seconds':time.monotonic()-started}
