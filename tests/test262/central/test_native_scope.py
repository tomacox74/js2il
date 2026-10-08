"""Porting scope regressions using full preparation/sealing and the real generator."""
from contextlib import ExitStack, nullcontext
import hashlib
import json
from pathlib import Path
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import patch

from scripts.test262.central import cli, commands


class Coordinator:
    def __init__(self, fixtures=None):
        self.epoch = 2
        self.fixtures = fixtures or {}
        self.tables = {}
        self.calls = []

    def view(self):
        return nullcontext()

    def put(self, table, rows):
        self.tables.setdefault(table, []).extend(rows)

    def read(self, table, filters=None):
        if table == 'runs':
            return [{'run_id': 'run', 'trust_class': 'trusted'}]
        if table == 'observations':
            if 'fixture_id' not in (filters or {}):
                return []
            return [{'observation_id': filters['fixture_id'] + '-pass', 'run_id': 'run',
                     'variant': 'strict', 'outcome': 'pass', 'source_kind': 'live', 'received_at': 'now'}]
        if table == 'fixture_variants':
            return [{'variant': 'strict', 'required': True}]
        if table == 'batch_fixtures':
            return [{'fixture_id': fid, 'state': 'accepted'} for fid in self.fixtures]
        if table == 'batch_evidence':
            return [{'fixture_id': filters['fixture_id'], 'variant': 'strict',
                     'observation_id': filters['fixture_id'] + '-pass'}]
        return self.tables.get(table, [])

    def one(self, table, **key):
        if table == 'fixtures':
            return dict(self.fixtures[key['fixture_id']], fixture_id=key['fixture_id'])
        if table == 'budget_scopes':
            return {'accepted_limit': 100}
        if table == 'native_batches':
            return {'state': 'sealed', 'version': 0}
        if table == 'observations':
            return {'phase': 'runtime', 'diagnostic_summary': '', 'outcome': 'pass',
                    'failure_class': None,
                    'started_at': '2026-10-08T07:00:00+00:00', 'finished_at': '2026-10-08T07:00:01+00:00'}
        return None

    def call(self, operation, *args):
        self.calls.append((operation, args))
        return {}


class NativeScopeTests(unittest.TestCase):
    def prepare(self, kind='native', area=''):
        directory = tempfile.TemporaryDirectory()
        self.addCleanup(directory.cleanup)
        repo = Path(directory.name)
        root = repo / 'upstream'
        paths = ['test/built-ins/Array/new.js', 'test/language/new.js',
                 'test/harness/assert-samevalue-same.js', 'test/intl402/new.js',
                 'test/annexB/new.js', 'test/built-ins/Array/registered.js',
                 'test/built-ins/Array/_support.js']
        inventory = []
        ids = {path: 'fixture-' + str(i) for i, path in enumerate(paths)}
        for path in paths:
            fixture = root / path
            fixture.parent.mkdir(parents=True, exist_ok=True)
            fixture.write_text('assert.sameValue(1, 1);')
            inventory.append({'path': path, 'sha256': 'a' * 64, 'variants': ['strict'],
                              'metadata': {}, 'state': 'runnable', 'reasons': []})
        pin = repo / 'tests/test262/test262.pin.json'
        pin.parent.mkdir(parents=True)
        pin.write_text(json.dumps({'upstream': {'commit': 'pin'}}))
        client = Coordinator()
        client.tables['registrations'] = [{'upstream_path': paths[5]}]
        args = cli.parser().parse_args(['--repository', 'repo', '--producer', 'producer',
            'prepare', '--kind', kind, '--root', str(root), '--revision', 'revision',
            '--run-key', 'run', '--budget-key', 'budget', '--area', area,
            '--output', str(repo / 'context.json')])

        def git(*arguments):
            return 'revision' if arguments == ('rev-parse', 'HEAD') else ''

        with ExitStack() as stack:
            for name, value in [('REPO', repo), ('git', git),
                                ('register', lambda *a, **k: ('corpus', ids)),
                                ('registrations', lambda *a: 'snapshot'),
                                ('provenance', lambda *a: 'provenance'),
                                ('start_run', lambda *a: 'run'),
                                ('github_validation', lambda *a: {'id': 1})]:
                stack.enter_context(patch.object(commands, name, value))
            stack.enter_context(patch.object(commands.catalog, 'bridge', return_value=inventory))
            stack.enter_context(patch.object(commands.catalog, 'hash_files', return_value={}))
            stack.enter_context(patch.object(commands.catalog, 'environment', return_value={'identity': {}}))
            stack.enter_context(patch.object(commands.subprocess, 'check_output', return_value='{"includes":[]}'))
            result = commands.prepare(client, args)
        queued = next((a[-1] for op, a in client.calls if op == 'reconcile'),
                      client.tables.get('work_items', []))
        return result, client, queued, ids

    def test_native_preparation_excludes_unportable_roots_before_scheduling(self):
        result, client, queued, ids = self.prepare()
        expected = {ids['test/built-ins/Array/new.js'], ids['test/language/new.js']}
        self.assertEqual(set(result['candidate_ids']), expected)
        self.assertEqual({w['fixture_id'] for w in queued}, expected)
        self.assertEqual({w['work_item_id'] for w in queued},
                         {w['work_item_id'] for w in client.tables['budget_work_items']})
        eligibility = {e['fixture_id']: e for e in client.tables['provenance_fixture_eligibility']}
        for path in ('test/harness/assert-samevalue-same.js', 'test/intl402/new.js', 'test/annexB/new.js'):
            self.assertEqual(eligibility[ids[path]]['eligibility'], 'policy-excluded')
            self.assertIn('native-porting-area', eligibility[ids[path]]['reason_codes'])
        self.assertEqual(len(eligibility), len(ids))  # Retain the full catalogue.

    def test_explicit_harness_area_does_not_bypass_native_porting_scope(self):
        result, client, queued, ids = self.prepare(area='harness')
        self.assertEqual(result['candidate_ids'], [])
        self.assertEqual(queued, [])
        self.assertEqual(client.tables['budget_work_items'], [])

    def test_mvp_can_still_screen_harness_and_other_test_roots(self):
        result, client, queued, ids = self.prepare(kind='mvp-composite')
        expected = {ids[path] for path in ids if not path.endswith('_support.js')}
        self.assertEqual(set(result['candidate_ids']), expected)
        self.assertEqual({w['fixture_id'] for w in queued}, expected)

    def seal(self, fixtures):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / 'context.json'
            path.write_text(json.dumps({'kind': 'native', 'provenance': 'provenance',
                'budget': 'budget', 'candidate_ids': list(fixtures), 'validation': 'validation',
                'registration_snapshot': 'snapshot'}))
            client = Coordinator({fid: {'upstream_path': upstream} for fid, upstream in fixtures.items()})
            result = commands.seal(client, SimpleNamespace(context=str(path), repository='repo',
                batch_key='batch', output=str(Path(directory) / 'batch.json')))
            return result, client

    def test_old_mixed_context_seals_only_portable_candidates(self):
        result, client = self.seal({'builtin': 'test/built-ins/Array/new.js',
            'language': 'test/language/new.js', 'harness': 'test/harness/assert-samevalue-same.js'})
        self.assertEqual(result['accepted'], 2)
        self.assertEqual({r['fixture_id'] for r in client.tables['batch_fixtures']}, {'builtin', 'language'})
        self.assertEqual({r['fixture_id'] for r in client.tables['batch_evidence']}, {'builtin', 'language'})

    def test_unportable_only_context_never_creates_a_batch(self):
        result, client = self.seal({'harness': 'test/harness/assert-samevalue-same.js'})
        self.assertEqual(result, {'accepted': 0, 'batch': None})
        self.assertNotIn('native_batches', client.tables)

    def test_existing_mixed_sealed_batch_is_rejected_before_generation_writes(self):
        with tempfile.TemporaryDirectory() as directory:
            repo = Path(directory)
            fixture = repo / 'tests/Jroc.Test262.Tests/built-ins/Array/JavaScript/new.js'
            cache = repo / 'generation.sqlite'
            data = b'assert.sameValue(1, 1);'
            digest = hashlib.sha256(data).hexdigest()
            client = Coordinator({'builtin': {'upstream_path': 'test/built-ins/Array/new.js',
                                               'content_sha256': '\\x' + digest},
                                  'harness': {'upstream_path': 'test/harness/assert-samevalue-same.js',
                                              'content_sha256': '\\x' + digest}})
            for row in client.fixtures.values():
                source = repo / 'upstream' / row['upstream_path']
                source.parent.mkdir(parents=True, exist_ok=True)
                source.write_bytes(data)
            context = {'revision': 'revision', 'run': 'run', 'provenance': 'provenance',
                       'identity': {'upstream': {'commit': 'pin'}, 'binaries': {}, 'harness': {},
                                    'environment_identity': {}, 'capabilities': {}}}
            args = SimpleNamespace(cache=str(cache), root=str(repo / 'upstream'), output=str(repo / 'generated.json'))
            with patch.object(commands, 'git', return_value='revision'), patch.object(commands, 'REPO', repo):
                with self.assertRaisesRegex(ValueError, 'outside supported areas'):
                    commands.generate_from_view(client, args, context, 'old-sealed-batch')
            self.assertFalse(cache.exists())
            self.assertFalse(fixture.exists())
            self.assertEqual(client.tables, {})

    def test_generator_preflights_all_paths_before_copying_any_fixture(self):
        with tempfile.TemporaryDirectory() as directory:
            repo = Path(directory)
            source = repo / 'upstream/test/built-ins/Array/new.js'
            source.parent.mkdir(parents=True)
            source.write_text('assert.sameValue(1, 1);')
            target = repo / 'checkout/tests/Jroc.Test262.Tests/built-ins/Array/JavaScript/new.js'
            db = commands.nativePorting.connect(repo / 'cache.sqlite')
            self.addCleanup(db.close)
            commands.nativePorting.create_run(db, SimpleNamespace(run_id='run', batch_id='batch',
                trigger_revision='revision', base_revision='revision', pin='pin',
                candidate_limit=2, accepted_limit=2, variant_limit=4, time_limit=10))
            db.execute('INSERT INTO candidates VALUES(?,?,?,?,?,?,?,?,?,0)',
                       ('run', 'test/built-ins/Array/new.js', commands.nativePorting.file_sha256(source),
                        '["strict"]', 'test', 'native', 'provenance', 'capabilities', 'accepted'))
            db.commit()
            summary = {'accepted': ['test/built-ins/Array/new.js', 'test/harness/assert-samevalue-same.js']}
            with patch.object(commands.nativePorting, 'report', return_value=summary):
                with self.assertRaisesRegex(ValueError, 'outside supported areas'):
                    commands.nativePorting.generate_batch(db, SimpleNamespace(run_id='run',
                        upstream=str(repo / 'upstream'), destination=str(repo / 'checkout')))
            self.assertFalse(target.exists())
