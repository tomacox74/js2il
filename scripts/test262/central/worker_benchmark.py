"""Measure managed queue scaling on one disposable Actions runner."""
import argparse
from collections import Counter, defaultdict
from concurrent.futures import ThreadPoolExecutor
from contextlib import closing
from datetime import datetime, timezone
import json
import math
import os
from pathlib import Path
from statistics import median
import subprocess
import tempfile
from threading import Barrier
from time import monotonic
from types import SimpleNamespace

from .client import Client, bytea, identity, sha
from .collection import write_json
from .execution_identity import catalog, execution_identity
from .importer import provenance, start_run
from .inventory import REPO, normalize_fixture, register
from .pilot import isolation_probe, provision
from .pilot_drills import require
from .pilot_enlistments import enroll
from .worker import stage_runtime, work_staged

COUNTS = (1, 2, 4)
AREA = 'built-ins/Math/abs'
CAP_MS = 120000
REPEATS = 3


class TimedClient:
    """Measure round trips without changing claim or outbox behavior."""

    def __init__(self, client):
        self.client = client
        self.epoch = client.epoch
        self.samples = defaultdict(list)
        self.claims = 0

    def one(self, *args, **kwargs):
        return self.client.one(*args, **kwargs)

    def read(self, *args, **kwargs):
        return self.client.read(*args, **kwargs)

    def call(self, operation, *args):
        started = monotonic()
        result = self.client.call(operation, *args)
        if operation in ('claim', 'ingest', 'complete'):
            self.samples[operation].append(round((monotonic() - started) * 1000, 3))
        if operation == 'claim' and result and 'work_item_id' in result:
            self.claims += 1
        return result


def run_slot(worker, base, context, slot, job, folder, ready):
    key = f"worker:{worker['producer']}:actions:{context['actions_run_id']}:1:{slot}:1"
    args = SimpleNamespace(**vars(base))
    args.producer = worker['producer']
    args.run = identity(args.repository, args.producer, key)
    args.budget = context['budget']
    args.outbox = str(folder / f'worker-{slot}.sqlite')
    with closing(Client(dsn=worker['dsn'], epoch=1)) as db:
        db.call('join_collection', context['run'], args.run, job, sha(context))
        ready.wait(timeout=30)
        measured = TimedClient(db)
        result = work_staged(measured, args)
        db.call('finish_run', args.run, 'complete', datetime.now(timezone.utc).isoformat(),
                {'source': 'supervisor', 'external_run_key': key, 'source_revision': context['revision'],
                 'reason': 'collection-finished', 'flush_verified': True})
    return dict(result, slot=slot, claims=measured.claims, api_ms=dict(measured.samples))


def run_trial(db, base, document, snapshot, validation, candidates, workers, trial, folder):
    folder.mkdir()
    run_key = f'actions:{trial}'
    pid = provenance(db, base.repository, base.corpus, base.kind, dict(document, disposable_trial=trial))
    run = start_run(db, base, pid, run_key + ':1', base.revision,
                    'native' if base.kind == 'native' else 'mvp')
    budget = identity(base.repository, 'worker-benchmark', run_key)
    limit = len(candidates)
    limits = dict(candidate_limit=limit, accepted_limit=limit, attempt_limit=limit,
                  time_limit_ms=limit * CAP_MS)
    db.put('budget_scopes', [dict(limits, budget_scope_id=budget, repository_id=base.repository,
                                  scope_kind='run', external_key=run_key)])
    target_key = dict(repository_id=base.repository, channel='master', evidence_kind=base.kind)
    target = dict(provenance_id=pid, registration_snapshot_id=snapshot, validation_id=validation)
    current = db.one('reporting_targets', channel='master', evidence_kind=base.kind)
    if current:
        db.call('transition', 'reporting_targets', target_key, current['version'], target)
    else:
        db.put('reporting_targets', [dict(target_key, **target)])
    work = [dict(row, work_item_id=identity(budget, row['fixture_id'], row['variant']),
                 repository_id=base.repository, provenance_id=pid, attempt_limit=1)
            for row in candidates]
    context = dict(run=run, repository=base.repository, producer=base.producer, budget=budget,
                   provenance=pid, revision=base.revision, kind=base.kind, registration_snapshot=snapshot,
                   validation=validation, managed_version=1, actions_run_id=trial, actions_attempt=1,
                   event_sha=base.revision, worker_producers=[w['producer'] for w in workers],
                   cap_ms=CAP_MS, limits=limits, work_count=limit,
                   work_digest=sha(sorted(row['work_item_id'] for row in work)))
    db.call('register_collection', {'context': context, 'work': work})
    db.call('open_collection', run, trial, sha(context))
    ready = Barrier(len(workers))
    started = monotonic()
    with ThreadPoolExecutor(max_workers=len(workers)) as pool:
        futures = [pool.submit(run_slot, w, base, context, slot, trial * 10 + slot, folder, ready)
                   for slot, w in enumerate(workers, 1)]
        results = [future.result() for future in futures]
    wall_seconds = monotonic() - started
    # Simulated terminal Actions proof is scoped to the fresh loopback catalogue.
    proof = {'workers': [dict(slot=w['slot'], job_id=trial * 10 + w['slot'], state='success', retained=True)
                         for w in results]}
    settled = db.call('finish_collection', run, proof)
    observations = list(db.read('observations', {'provenance_id': pid}))
    queue = list(db.read('work_items', {'provenance_id': pid}))
    outcomes = Counter((o['fixture_id'], o['variant'], o['outcome']) for o in observations)
    require(settled['state'] == 'complete' and settled['budget']['result']['state'] == 'closed',
            'Managed benchmark failed finalization')
    require(len(observations) == len({o['work_item_id'] for o in observations}) == limit
            and all(w['state'] == 'completed' and w['lease_generation'] == 1 for w in queue)
            and settled['budget']['result']['charged_attempts'] == limit,
            'Benchmark lost, retried, or duplicated work')
    return {
        'workers': results, 'wall_seconds': wall_seconds, 'claimed': sum(w['claims'] for w in results),
        'completed': len(observations), 'completed_per_minute': 60 * len(observations) / wall_seconds,
        'fixture_active_ms': [o['active_ms'] for o in observations],
        'outcomes': dict(Counter(o['outcome'] for o in observations)),
        'timeouts': sum(o['phase'] == 'timeout' for o in observations),
        'queue': dict(Counter(w['state'] for w in queue)), 'outcome_digest': sha(sorted(outcomes.items())),
        'budget': settled['budget']['result'], 'provenance': pid,
        'worker_active_minutes': sum(w['seconds'] for w in results) / 60,
    }


def summarize(trials, rate):
    baseline = median(t['wall_seconds'] for t in trials['1'])
    summary = {}
    for count, runs in trials.items():
        seconds = median(t['wall_seconds'] for t in runs)
        # This is only the shared runner's screening interval rounded per trial.
        minutes = median(math.ceil(t['wall_seconds'] / 60) for t in runs)
        summary[count] = dict(
            wall_seconds=round(seconds, 3), speedup=round(baseline / seconds, 3),
            variants_per_minute=round(median(t['completed_per_minute'] for t in runs), 2),
            shared_screening_runner_minutes=minutes,
            screening_cost_usd=round(minutes * rate, 6) if rate is not None else None,
            worker_active_minutes=round(median(t['worker_active_minutes'] for t in runs), 3),
            fixture_active_ms=round(median(v for t in runs for v in t['fixture_active_ms']), 3),
            api_median_ms={
                op: round(median(ms for t in runs for w in t['workers'] for ms in w['api_ms'].get(op, [])), 3)
                for op in ('claim', 'ingest', 'complete')
            },
        )
    return summary


def write_plot(path, summary):
    maximum = max(row['variants_per_minute'] for row in summary.values())
    bars = []
    for index, (count, row) in enumerate(summary.items()):
        y = 66 + 40 * index
        rate = row['variants_per_minute']
        width = round(310 * rate / maximum, 2)
        bars.append(f'<text x="10" y="{y+15}">{count} workers</text>'
                    f'<rect x="105" y="{y}" width="{width}" height="22" fill="#1565c0"/>'
                    f'<text x="{115+width}" y="{y+15}">{rate:.2f}</text>')
    path.write_text('<svg xmlns="http://www.w3.org/2000/svg" width="510" height="200" '
                    'role="img" aria-label="Median variants per minute by worker count">'
                    '<rect width="510" height="200" fill="white"/>'
                    '<g font-family="sans-serif" font-size="14" fill="#17212b">'
                    '<text x="10" y="31" font-weight="bold">Median variants per minute on one runner</text>'
                    + ''.join(bars) + '</g></svg>\n', encoding='utf-8')


def run(args):
    admin = os.environ['TEST262_PILOT_ADMIN_DSN']
    repo, owner, dsn = provision(admin)
    root = Path(args.root).resolve()
    revision = subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=REPO, text=True).strip()
    pin = json.loads((REPO / 'tests/test262/test262.pin.json').read_text())['upstream']
    require(subprocess.check_output(['git', '-C', str(root), 'rev-parse', 'HEAD'], text=True).strip() == pin['commit'],
            'Benchmark upstream pin mismatch')
    inventory = catalog.bridge({'command': 'inventory', 'root': str(root)})
    selected = [normalize_fixture(row, root) for row in sorted(inventory, key=lambda row: row['path'])
                if row['path'].startswith('test/' + AREA + '/') and row['state'] == 'runnable'
                and sorted(row['variants']) == ['non-strict', 'strict']][:5]
    require(len(selected) == 5, 'Benchmark needs five fixed two-variant fixtures')
    image_id = subprocess.check_output(['docker', 'image', 'inspect', '--format', '{{.Id}}', args.image],
                                       text=True).strip()
    report = dict(complete=False, revision=revision, upstream_commit=pin['commit'], image_id=image_id,
                  kind=args.kind, area=AREA, repetitions=REPEATS, runner_topology='single-host',
                  unavailable={'8': 'The managed catalogue supports at most four worker slots'},
                  candidates=[(row['path'], row['sha256']) for row in selected], trials={str(n): [] for n in COUNTS})
    output = Path(args.output)
    output.mkdir(parents=True)
    try:
        with closing(Client(dsn=dsn, epoch=1)) as db, tempfile.TemporaryDirectory(prefix='benchmark-runtime-') as runtime:
            corpus, ids = register(db, repo, pin, selected)
            candidates = [dict(fixture_id=ids[row['path']], variant=v['variant'], coherent_area=AREA)
                          for row in selected for v in row['variants']]
            snapshot = identity(repo, 'benchmark-registrations', revision)
            db.put('registration_snapshots', [{'snapshot_id': snapshot, 'repository_id': repo,
                    'source_revision': revision, 'inventory_sha256': bytea(sha([]))}])
            db.call('transition', 'registration_snapshots', {'snapshot_id': snapshot}, 0, {'state': 'sealed'})
            validation = None
            if args.kind == 'native':
                validation = identity(repo, 'benchmark-validation', revision)
                db.put('validation_events', [{'validation_id': validation, 'repository_id': repo,
                    'workflow_identity': 'disposable-benchmark', 'external_run_key': revision,
                    'target_revision': revision, 'conclusion': 'success', 'proof': {'synthetic_disposable_validation': True}}])
            base = SimpleNamespace(**vars(args), repository=repo, producer=owner, corpus=corpus, revision=revision,
                                   runtime_timeout=30, compile_timeout=60, cap_ms=CAP_MS,
                                   limit=len(candidates), seconds=180, runtime=runtime)
            document = execution_identity(base, corpus, bundled=True)
            report['execution_identity'] = document
            stage_runtime(base, runtime)
            Path(runtime).chmod(0o755)
            probe_ms = []
            for _ in range(3):
                started = monotonic()
                report['isolation'] = isolation_probe(base, root)
                probe_ms.append(round((monotonic() - started) * 1000, 3))
            report['isolated_container_probe_ms'] = probe_ms
            workers = [enroll(admin, repo, i, login=f'catalogue_benchmark_worker_{i}') for i in range(1, 5)]
            # Rotate the first configuration to reduce order/warm-up bias.
            for repeat in range(REPEATS):
                for count in COUNTS[repeat:] + COUNTS[:repeat]:
                    trial = 236600 + repeat * 10 + count
                    report['trials'][str(count)].append(
                        run_trial(db, base, document, snapshot, validation, candidates, workers[:count],
                                  trial, output / str(trial)))
            require(len({t['outcome_digest'] for runs in report['trials'].values() for t in runs}) == 1,
                    'Fixture outcomes changed across concurrency levels')
            report['summary'] = summarize(report['trials'], args.rate)
            write_plot(output / 'throughput.svg', report['summary'])
            report['complete'] = True
    finally:
        write_json(output / 'benchmark.json', report)
    print(json.dumps(report['summary'], indent=2))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--kind', choices=('mvp-composite', 'native'), required=True)
    parser.add_argument('--root', required=True)
    parser.add_argument('--output', required=True)
    parser.add_argument('--image', default='test262-fixture:pilot')
    parser.add_argument('--jroc', default='src/Cli/bin/Release/net10.0/Jroc.dll')
    parser.add_argument('--host', default='scripts/test262/NativeScreeningHost/bin/Release/net10.0/NativeScreeningHost.dll')
    parser.add_argument('--rate', type=float, help='Optional USD per billed Linux minute')
    args = parser.parse_args()
    if args.rate is not None and (not math.isfinite(args.rate) or args.rate < 0):
        parser.error('--rate must be finite and non-negative')
    run(args)


if __name__ == '__main__':
    main()
