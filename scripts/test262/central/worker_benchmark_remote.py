"""One trial across independent Actions runners against a dedicated benchmark DB."""
import argparse
from contextlib import closing
from datetime import datetime
import json
import math
import os
import re
from pathlib import Path
from statistics import median
import subprocess
import tempfile
from time import monotonic, sleep
from types import SimpleNamespace
from uuid import UUID

from .client import Client, identity
from .collection import (
    actions_origin, artifact_proof, digest_string, history, load_context, open_collection,
    terminal_proof, write_json,
)
from .collection_bundle import digest
from .enlist import check_checkout
from .execution_identity import execution_identity
from .lifecycle import gh
from .pilot import isolation_probe
from .pilot_drills import require
from .worker import stage_runtime
from .worker_benchmark import (
    AREA, CAP_MS, COUNTS, execute_slot, finish_trial, prepare_inventory,
    prepare_trial, select_fixtures, write_plot,
)

WORKFLOW = '.github/workflows/test262-worker-benchmark-remote.yml'
BENCHMARK_DB = 'catalogue_benchmark'


def resolve_bundle(args):
    run, _, revision = actions_origin()
    require(0 < args.bundle_run < run, 'Benchmark bundle must come from an earlier run')
    context = {'actions_run_id': args.bundle_run, 'actions_attempt': 1, 'event_sha': revision}
    history(context, args.repository_name, completed=True, slots=[], workflow=WORKFLOW)
    artifact_id, _ = artifact_proof(context, args.repository_name,
                                    f'test262-split-prepare-{args.bundle_run}-1')
    print(f'artifact_id={artifact_id}')


def check_connection(client, *, slot=None):
    info = client.db.info
    login = f'catalogue_benchmark_worker_{slot}' if slot is not None else 'catalogue_benchmark_coordinator'
    project = os.environ.get('BENCHMARK_SUPABASE_PROJECT')
    if project:
        require(re.fullmatch('[a-z0-9]{20}', project), 'Invalid benchmark Supabase project reference')
        expected = info.dbname == 'postgres' and (
            (info.host == f'db.{project}.supabase.co' and info.user == login)
            or (info.host.endswith('.pooler.supabase.com') and info.port == 5432
                and info.user == f'{login}.{project}'))
    else:
        expected = info.dbname == BENCHMARK_DB and info.user == login
    require(expected and client.db.pgconn.ssl_in_use,
            'Benchmark requires a dedicated TLS database and the expected benchmark login')
    require(client.contract['repository_id'] == str(UUID(os.environ['BENCHMARK_REPOSITORY_ID'])),
            'Benchmark credential is bound to a different repository')


def prepare(client, args):
    run, _, revision = actions_origin()
    require(revision == args.revision, 'Master advanced from the requested benchmark SHA')
    require(len(args.producer) in COUNTS and len(set(args.producer)) == len(args.producer),
            'Benchmark needs one, two, or four distinct workers')
    repo = client.contract['repository_id']
    pin, selected = select_fixtures(Path(args.root).resolve())
    corpus, candidates, snapshot, validation = prepare_inventory(client, repo, revision, args.kind, pin, selected)
    base = SimpleNamespace(**vars(args), repository=repo, corpus=corpus,
                           runtime_timeout=30, compile_timeout=60, cap_ms=CAP_MS)
    base.producer = client.contract['producer_id']
    document = execution_identity(base, corpus, bundled=True)
    directory = Path(args.context).parent
    context = prepare_trial(
        client, base, document, snapshot, validation, candidates,
        [{'producer': str(UUID(producer))} for producer in args.producer], run,
        corpus=corpus, identity=document, area=AREA, repeat=args.repeat, bundle_run=args.bundle_run,
        fixture_manifest=[(row['path'], row['sha256']) for row in selected],
        image_id=subprocess.check_output(['docker', 'image', 'inspect', '--format', '{{.Id}}', args.image],
                                         text=True).strip(),
        runtime_digest=digest(directory / 'runtime.tar.gz'),
        image_digest=digest(directory / 'fixture-image.tar.gz'),
    )
    write_json(args.context, context)


def work(client, args):
    context, registered = load_context(client, args.context)
    producer = client.contract['producer_id']
    require(1 <= args.slot <= len(context['worker_producers'])
            and context['worker_producers'][args.slot - 1] == producer,
            'Benchmark worker identity does not match its slot')
    require(registered['artifact_id'] == args.artifact_id
            and registered['artifact_digest'] == '\\x' + digest_string(args.artifact_digest),
            'Benchmark worker artifact does not match the opened collection')
    job = history(context, args.repository_name, completed=False, slots=[args.slot], workflow=WORKFLOW)[args.slot]
    require(job['status'] == 'in_progress', 'Benchmark execution job is not running')
    base = SimpleNamespace(**vars(args), repository=context['repository'], kind=context['kind'],
                           runtime_timeout=30, compile_timeout=60, cap_ms=CAP_MS,
                           limit=context['work_count'], seconds=180)
    check_checkout(base, context)
    image_id = subprocess.check_output(['docker', 'image', 'inspect', '--format', '{{.Id}}', args.image],
                                       text=True).strip()
    require(image_id == context['image_id'], 'Loaded fixture image identity differs')

    def wait_for_peers():
        deadline = monotonic() + 180
        while monotonic() < deadline:
            state = client.call('collection', context['run'])
            if all(member['run_id'] for member in state['workers']):
                return
            sleep(1)
        raise TimeoutError('Benchmark workers did not all join')

    folder = Path(args.output)
    folder.mkdir(parents=True, exist_ok=True)
    with tempfile.TemporaryDirectory(prefix='benchmark-runtime-') as runtime:
        stage_runtime(base, runtime)
        Path(runtime).chmod(0o755)
        base.runtime = runtime
        probe_start = monotonic()
        isolation = isolation_probe(base, Path(args.root).resolve())
        probe_ms = (monotonic() - probe_start) * 1000
        result = execute_slot(client, producer, base, context, args.slot, job['id'], folder, wait_for_peers)
    write_json(folder / 'metrics.json', dict(result, job_id=job['id'], isolation=isolation,
                                            container_probe_ms=round(probe_ms, 3)))


def read_worker_metrics(directory):
    # A single downloaded artifact is extracted directly into the target.
    # Multiple artifacts each receive their own subdirectory.
    root = Path(directory)
    paths = [*root.glob('metrics.json'), *root.glob('*/metrics.json')]
    return [json.loads(path.read_text()) for path in paths]


def finish(client, args):
    run, attempt, _ = actions_origin()
    owner_run = identity(client.contract['repository_id'], client.contract['producer_id'], f'actions:{run}:{attempt}')
    registered = client.call('collection', owner_run)
    require(registered is not None, 'Benchmark collection was not prepared')
    context = registered['context']
    proof = terminal_proof(context, args.repository_name, workflow=WORKFLOW)
    if any(p['state'] != 'success' or not p['retained'] for p in proof['workers']):
        result = client.call('finish_collection', owner_run, proof)
        write_json(args.output, result)
        raise ValueError('Benchmark workers failed or lack recovery evidence')
    metrics = read_worker_metrics(args.metrics)
    expected = {(p['slot'], p['job_id']) for p in proof['workers']}
    require(len(metrics) == len(expected) and {(m['slot'], m['job_id']) for m in metrics} == expected,
            'Benchmark metrics are missing or do not match execution jobs')
    started = min(datetime.fromisoformat(m['started_at']) for m in metrics)
    ended = max(datetime.fromisoformat(m['ended_at']) for m in metrics)
    require(ended > started, 'Invalid screening interval')
    trial = finish_trial(client, context, proof, metrics, (ended - started).total_seconds())
    pages = gh(f'repos/{args.repository_name}/actions/runs/{run}/attempts/{attempt}/jobs?per_page=100')
    jobs = [job for page in pages for job in page['jobs'] if job['name'] == 'prepare'
            or job['id'] in {job_id for _, job_id in expected}]
    require(len(jobs) == len(expected) + 1, 'Benchmark billing jobs are missing')
    minutes = sum(math.ceil((datetime.fromisoformat(job['completed_at'])
                             - datetime.fromisoformat(job['started_at'])).total_seconds() / 60) for job in jobs)
    write_json(args.output, dict(trial, complete=True, runner_topology='independent-actions-runners',
                                 actions_run_id=run, revision=context['revision'], identity=context['identity'],
                                 image_id=context['image_id'], bundle_run=context['bundle_run'],
                                 candidates=context['fixture_manifest'], kind=context['kind'], area=context['area'],
                                 limits=context['limits'], repeat=context['repeat'], worker_count=len(metrics),
                                 prepare_and_execute_runner_minutes=minutes))


def completed_job_minutes(repository, run):
    pages = gh(f'repos/{repository}/actions/runs/{run}/attempts/1/jobs?per_page=100')
    jobs = [job for page in pages for job in page['jobs']]
    require(jobs and all(job['conclusion'] == 'success' for job in jobs),
            'Benchmark job timings require a completed successful run')
    require(all(job['run_id'] == run and job['run_attempt'] == 1 for job in jobs),
            'Benchmark billing job belongs to another attempt')
    return sum(math.ceil((datetime.fromisoformat(job['completed_at'])
                         - datetime.fromisoformat(job['started_at'])).total_seconds() / 60) for job in jobs)


def compare(args):
    reports = [json.loads(Path(path).read_text()) for path in args.reports]
    output = Path(args.output)
    output.mkdir(parents=True, exist_ok=True)
    for kind in sorted({report['kind'] for report in reports}):
        group = [report for report in reports if report['kind'] == kind]
        baseline = group[0]
        fields = ('revision', 'identity', 'image_id', 'bundle_run', 'candidates', 'limits', 'area', 'outcome_digest')
        require(baseline['bundle_run'] > 0, 'Use a common seeded bundle for every measured trial')
        require(all(row['complete'] and row['runner_topology'] == 'independent-actions-runners'
                    and all(row[field] == baseline[field] for field in fields) for row in group),
                'Benchmark revisions, inputs, execution identities, or outcomes differ')
        require(len(group) == len({row['actions_run_id'] for row in group}) == 9
                and {(row['worker_count'], row['repeat']) for row in group}
                    == {(count, repeat) for count in COUNTS for repeat in (1, 2, 3)},
                'Supply three distinct trials per worker count for each evidence kind')
        summary = {}
        for count in COUNTS:
            trials = [row for row in group if row['worker_count'] == count]
            summary[str(count)] = {
                'wall_seconds': round(median(row['wall_seconds'] for row in trials), 3),
                'variants_per_minute': round(median(row['completed_per_minute'] for row in trials), 2),
                'runner_minutes': median(
                    completed_job_minutes(args.repository_name, row['actions_run_id']) for row in trials),
                'actions_run_ids': [row['actions_run_id'] for row in sorted(trials, key=lambda row: row['repeat'])],
            }
        for row in summary.values():
            row['speedup'] = round(summary['1']['wall_seconds'] / row['wall_seconds'], 3)
            if args.rate is not None:
                row['estimated_cost_usd'] = round(row['runner_minutes'] * args.rate, 6)
        write_json(output / f'{kind}.json', dict(summary=summary, kind=kind,
                                                **{field: baseline[field] for field in fields}))
        write_plot(output / f'{kind}.svg', summary, 'Median variants per minute across independent runners')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    sub = parser.add_subparsers(dest='command', required=True)
    for name in ('prepare', 'open', 'work', 'finish'):
        command = sub.add_parser(name)
        command.add_argument('--repository-name', default=os.environ.get('GITHUB_REPOSITORY'))
        if name in ('prepare', 'open', 'work'):
            command.add_argument('--context', required=True)
        if name in ('prepare', 'work'):
            command.add_argument('--root', required=True)
            command.add_argument('--image', default='test262-fixture:local')
            command.add_argument('--jroc', default='src/Cli/bin/Release/net10.0/Jroc.dll')
            command.add_argument('--host', default='scripts/test262/NativeScreeningHost/bin/Release/net10.0/NativeScreeningHost.dll')
        if name in ('open', 'work'):
            command.add_argument('--artifact-id', type=int, required=True)
            command.add_argument('--artifact-digest', required=True)
    sub.choices['prepare'].add_argument('--revision', required=True)
    sub.choices['prepare'].add_argument('--kind', choices=('mvp-composite', 'native'), required=True)
    sub.choices['prepare'].add_argument('--bundle-run', type=int, default=0)
    resolver = sub.add_parser('resolve-bundle')
    resolver.add_argument('--bundle-run', type=int, required=True)
    resolver.add_argument('--repository-name', default=os.environ.get('GITHUB_REPOSITORY'))
    sub.choices['prepare'].add_argument('--repeat', type=int, choices=range(1, 4), required=True)
    sub.choices['prepare'].add_argument('--producer', action='append', required=True)
    sub.choices['work'].add_argument('--slot', type=int, choices=range(1, 5), required=True)
    sub.choices['work'].add_argument('--output', required=True)
    sub.choices['finish'].add_argument('--metrics', required=True)
    sub.choices['finish'].add_argument('--output', required=True)
    comparison = sub.add_parser('compare')
    comparison.add_argument('reports', nargs='+')
    comparison.add_argument('--output', required=True)
    comparison.add_argument('--repository-name', default='tomacox74/js2il')
    comparison.add_argument('--rate', type=float, help='Optional USD per billed Linux minute')
    args = parser.parse_args()
    if args.command == 'resolve-bundle':
        resolve_bundle(args)
        return
    if args.command == 'compare':
        if args.rate is not None and (not math.isfinite(args.rate) or args.rate < 0):
            parser.error('--rate must be finite and non-negative')
        compare(args)
        return
    try:
        with closing(Client()) as client:
            check_connection(client, slot=getattr(args, 'slot', None))
            {'prepare': prepare, 'open': open_collection, 'work': work, 'finish': finish}[args.command](client, args)
    except Exception as error:
        # Connection errors may contain a credential or host. Do not emit them.
        if isinstance(error, (ValueError, TimeoutError)):
            print(f'Benchmark {args.command} failed: {error}')
        else:
            print(f'Benchmark {args.command} failed: {type(error).__name__}')
        raise SystemExit(1) from None


if __name__ == '__main__':
    main()
