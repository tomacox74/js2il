"""Comparison and database fences for independent worker benchmarks."""
import json
import os
from pathlib import Path
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import Mock, patch

from scripts.test262.central import worker_benchmark_remote as benchmark


class BenchmarkTests(unittest.TestCase):
    def test_benchmark_connections_require_tls_and_the_exact_slot(self):
        repo = 'a1234567-1234-4234-8234-123456789abc'
        info = SimpleNamespace(dbname='catalogue_benchmark', user='catalogue_benchmark_worker_2')
        client = SimpleNamespace(db=SimpleNamespace(info=info, pgconn=SimpleNamespace(ssl_in_use=True)),
                                 contract={'repository_id': repo})
        with patch.dict(os.environ, {'BENCHMARK_REPOSITORY_ID': repo}):
            benchmark.check_connection(client, slot=2)
            with self.assertRaises(ValueError):
                benchmark.check_connection(client, slot=1)
            client.db.pgconn.ssl_in_use = False
            with self.assertRaises(ValueError):
                benchmark.check_connection(client, slot=2)

    def test_supabase_pooler_requires_the_dedicated_project_reference(self):
        project = 'a' * 20
        repo = 'a1234567-1234-4234-8234-123456789abc'
        info = SimpleNamespace(dbname='postgres', host='aws-0-us-west-2.pooler.supabase.com',
                               port=5432, user=f'catalogue_benchmark_worker_2.{project}')
        client = SimpleNamespace(db=SimpleNamespace(info=info, pgconn=SimpleNamespace(ssl_in_use=True)),
                                 contract={'repository_id': repo})
        with patch.dict(os.environ, {'BENCHMARK_REPOSITORY_ID': repo, 'BENCHMARK_SUPABASE_PROJECT': project}):
            benchmark.check_connection(client, slot=2)
            info.user = 'catalogue_benchmark_worker_2.' + 'b' * 20
            with self.assertRaises(ValueError):
                benchmark.check_connection(client, slot=2)

    def test_prepare_passes_distinct_queue_candidates_and_fixture_manifest(self):
        producer = 'a1234567-1234-4234-8234-123456789abc'
        args = SimpleNamespace(revision='revision', producer=[producer], root='.', kind='native',
                               context='context.json', repeat=1, bundle_run=0, image='fixture')
        client = SimpleNamespace(contract={'repository_id': 'repo', 'producer_id': 'coordinator'})
        candidates = [{'fixture_id': 'fixture', 'variant': 'strict'}]
        with patch.object(benchmark, 'actions_origin', return_value=(23, 1, args.revision)), \
                patch.object(benchmark, 'select_fixtures', return_value=({}, [{'path': 'test.js', 'sha256': 'digest'}])), \
                patch.object(benchmark, 'prepare_inventory', return_value=('corpus', candidates, 'snapshot', 'validation')), \
                patch.object(benchmark, 'execution_identity', return_value={}), \
                patch.object(benchmark.subprocess, 'check_output', return_value='image'), \
                patch.object(benchmark, 'digest', return_value='digest'), \
                patch.object(benchmark, 'prepare_trial', autospec=True, return_value={}) as trial, \
                patch.object(benchmark, 'write_json'):
            benchmark.prepare(client, args)
        self.assertEqual(trial.call_args.args[5], candidates)
        self.assertEqual(trial.call_args.kwargs['fixture_manifest'], [('test.js', 'digest')])

    def test_reads_single_and_multiple_downloaded_worker_artifacts(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            single = root / 'metrics.json'
            single.write_text(json.dumps({'slot': 1}))
            self.assertEqual(benchmark.read_worker_metrics(root), [{'slot': 1}])
            single.unlink()
            for slot in (1, 2):
                path = root / f'worker-{slot}' / 'metrics.json'
                path.parent.mkdir()
                path.write_text(json.dumps({'slot': slot}))
            self.assertEqual(sorted(row['slot'] for row in benchmark.read_worker_metrics(root)), [1, 2])

    def test_compare_verifies_provenance_and_uses_total_job_minutes(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            reports = []
            for count in (1, 2, 4):
                for repeat in (1, 2, 3):
                    path = root / f'{count}-{repeat}.json'
                    path.write_text(json.dumps(dict(
                        kind='native', complete=True, runner_topology='independent-actions-runners',
                        revision='sha', identity={'binaries': 'same'}, image_id='same', bundle_run=1,
                        candidates=['same'], limits={'attempt_limit': 10}, area='Math', outcome_digest='same',
                        actions_run_id=10 * count + repeat, repeat=repeat, worker_count=count,
                        wall_seconds=20 / count, completed_per_minute=30 * count,
                    )))
                    reports.append(path)
            args = SimpleNamespace(reports=reports, output=root / 'summary', rate=.01, repository_name='test/repo')
            with patch.object(benchmark, 'completed_job_minutes', side_effect=lambda _, run: run // 10 + 3):
                benchmark.compare(args)
            summary = json.loads((args.output / 'native.json').read_text())['summary']
            self.assertEqual(summary['4']['speedup'], 4)
            self.assertEqual(summary['4']['estimated_cost_usd'], .07)
            self.assertTrue((args.output / 'native.svg').is_file())
            changed = json.loads(reports[-1].read_text())
            reports[-1].write_text(json.dumps(dict(changed, image_id='different')))
            with patch.object(benchmark, 'completed_job_minutes', new=Mock()) as jobs:
                with self.assertRaisesRegex(ValueError, 'identities'):
                    benchmark.compare(args)
                jobs.assert_not_called()

    def test_minutes_round_each_job(self):
        def job(start, end):
            return dict(run_id=12, run_attempt=1, conclusion='success',
                        started_at=f'2026-11-03T00:{start}Z', completed_at=f'2026-11-03T00:{end}Z')
        with patch.object(benchmark, 'gh', return_value=[{'jobs': [
            job('00:00', '01:01'), job('01:00', '01:01'), job('01:01', '01:55'),
        ]}]):
            self.assertEqual(benchmark.completed_job_minutes('test/repo', 12), 4)
