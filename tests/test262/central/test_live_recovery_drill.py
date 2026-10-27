"""Offline bounded-recovery proofs: no DB connection, GitHub call or JS execution."""
from contextlib import contextmanager, redirect_stdout
from copy import deepcopy
import hashlib
import io
import json
import os
from pathlib import Path
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import Mock, patch
import zipfile

from scripts.test262.central import commands, live_recovery_drill as drill, worker
from scripts.test262.central.client import Outbox, bytea, fixture_environment, identity, sha
from scripts.test262.central.inventory import normalize_fixture


SOURCE = 'a' * 40
UPSTREAM = 'b' * 40
IMAGE = 'sha256:' + 'c' * 64
FIXTURE = 'fixture-one'
PATH = 'test/built-ins/Array/recovery-example.js'
REAL_IMAGE_ID = drill.image_id


class Catalogue:
    """Strict recording model, not an emulation of a live PostgreSQL server."""

    def __init__(self, base, fixture):
        self.epoch = 7
        self.contract = dict(repository_id='repo', producer_id='producer',
                             permission='coordinator', trust_class='trusted',
                             deployment_state='active', minimum_writer_epoch=7)
        self.events = []
        self.tables = {
            'reporting_targets': [
                dict(channel='master', evidence_kind='native', provenance_id='base',
                     registration_snapshot_id='snapshot', version=9),
                dict(channel='master', evidence_kind='mvp-composite', provenance_id='mvp', version=4)],
            'reconciliation_state': [dict(pipeline='native', channel='master',
                                          fallback_cursor=91, version=6)],
            'provenances': [dict(provenance_id='base', corpus_id='corpus', identity_document=base)],
            'corpora': [dict(corpus_id='corpus', inventory_state='sealed', revision=UPSTREAM,
                            upstream_url=base['upstream']['cloneUrl'])],
            'registration_snapshots': [dict(snapshot_id='snapshot', state='sealed',
                                            source_revision=SOURCE)],
            'registrations': [],
            'provenance_fixture_eligibility': [
                dict(provenance_id='base', fixture_id=FIXTURE, eligibility='runnable')],
            'fixtures': [fixture],
            'fixture_variants': [dict(fixture_id=FIXTURE, variant=v, required=True)
                                 for v in ('non-strict', 'strict')],
            'runs': [], 'budget_scopes': [], 'work_items': [], 'observations': [],
            'budget_work_items': [], 'validation_events': [],
        }
        self.db = self
        self.fail_operation = None
        self.transaction_depth = 0
        self.metrics = []

    @contextmanager
    def transaction(self):
        self.events.append(('transaction', 'begin'))
        self.transaction_depth += 1
        try:
            yield
        finally:
            self.transaction_depth -= 1
            self.events.append(('transaction', 'end'))

    def execute(self, sql, parameters=None):
        if sql == 'SELECT test262.api_contract()':
            return SimpleNamespace(fetchone=lambda: (deepcopy(self.contract),))
        if sql == 'SELECT pg_advisory_xact_lock(%s)':
            self.events.append(('lock', parameters))
            return None
        if sql == 'SELECT pg_advisory_xact_lock(hashtextextended(%s,0))':
            if self.transaction_depth != 1:
                raise AssertionError('Repository claim lock must be inside claim transaction')
            self.events.append(('repository-lock', parameters))
            return None
        raise AssertionError('Unexpected SQL in offline catalogue')

    def read(self, table, filters=None):
        if table not in self.tables:
            raise AssertionError('Unexpected table read: ' + table)
        self.events.append(('read', table, deepcopy(filters)))
        return deepcopy([r for r in self.tables[table]
                         if all(r.get(k) == v for k, v in (filters or {}).items())])

    def one(self, table, **filters):
        rows = self.read(table, filters)
        if len(rows) > 1:
            raise AssertionError('Ambiguous fake lookup: ' + table)
        return rows[0] if rows else None

    def put(self, table, rows):
        allowed = {'provenances', 'provenance_fixture_eligibility', 'budget_scopes',
                   'work_items', 'budget_work_items', 'validation_events'}
        if table not in allowed:
            raise AssertionError('Forbidden drill write: ' + table)
        self.events.append(('put', table, deepcopy(rows)))
        for row in deepcopy(rows):
            if table == 'work_items':
                old = next((w for w in self.tables[table]
                            if w['work_item_id'] == row['work_item_id']), None)
                if old:
                    continue
                row.update(state='pending', lease_generation=0)
            if table == 'budget_scopes':
                row.update(reserved_attempts=0, reserved_ms=0, charged_attempts=0,
                           charged_ms=0, state='open')
            self.tables[table].append(row)

    def mutable(self, table, key, value):
        return next(r for r in self.tables[table] if r[key] == value)

    def call(self, operation, *args):
        self.events.append(('call', operation, deepcopy(args)))
        if operation == self.fail_operation:
            raise TimeoutError('uncertain fake response')
        if operation == 'start_run':
            row = deepcopy(args[0])
            row.update(repository_id='repo', producer_id='producer', trust_class='trusted',
                       state='running', started_at='2026-10-27T00:00:00+00:00')
            self.tables['runs'].append(row)
            return row
        if operation == 'claim':
            if self.transaction_depth != 1:
                raise AssertionError('Claim must be in same guarded transaction')
            run, budget_id, cap, _ = args
            budget = self.mutable('budget_scopes', 'budget_scope_id', budget_id)
            enrolled = {w['work_item_id'] for w in self.tables['budget_work_items']
                        if w['budget_scope_id'] == budget_id}
            pending = [w for w in self.tables['work_items']
                       if w['work_item_id'] in enrolled and w['state'] == 'pending']
            if not pending:
                return None
            if budget['charged_attempts'] >= budget['attempt_limit']:
                return {'budget_exhausted': True}
            row = pending[0]
            row.update(state='leased', lease_generation=1)
            budget['charged_attempts'] += 1
            budget['reserved_attempts'] += 1
            budget['reserved_ms'] += cap
            return deepcopy(row)
        if operation == 'ingest':
            _, rows = args
            self.tables['observations'].extend(deepcopy(rows))
            return {'accepted': len(rows)}
        if operation == 'complete':
            work_id, generation, observation = args
            row = self.mutable('work_items', 'work_item_id', work_id)
            row['state'] = 'completed'
            enrolled = [r for r in self.tables['budget_work_items'] if r['work_item_id'] == work_id]
            budget = self.mutable('budget_scopes', 'budget_scope_id', enrolled[-1]['budget_scope_id'])
            budget.update(reserved_attempts=0, reserved_ms=0)
            return {'work_item_id': work_id, 'lease_generation': generation}
        if operation == 'finish_run':
            run, state, ended, evidence = args
            if evidence['source'] not in ('supervisor', 'github-actions'):
                raise AssertionError('Lifecycle source is not allowed by production SQL')
            if evidence['reason'] not in ('collection-finished', 'job-cancelled', 'job-failed',
                                          'job-interrupted', 'flush-unproven'):
                raise AssertionError('Lifecycle reason is not allowed by production SQL')
            if state == 'complete' and not any(m[0] == run and m[1] == 'screening' for m in self.metrics):
                raise AssertionError('Physical collection completion requires its screening metric')
            row = self.mutable('runs', 'run_id', run)
            row.update(state=state, ended_at=ended)
            return deepcopy(row)
        if operation == 'finish_budget':
            budget = self.mutable('budget_scopes', 'budget_scope_id', args[0])
            if not budget['external_key'].startswith('actions:'):
                raise AssertionError('Production only finalizes genuine Actions budget keys')
            enrolled = {w['work_item_id'] for w in self.tables['budget_work_items']
                        if w['budget_scope_id'] == args[0]}
            unfinished = any(w['state'] != 'completed' and w['work_item_id'] in enrolled
                             for w in self.tables['work_items'])
            budget['state'] = 'exhausted' if unfinished else 'closed'
            return {'result': deepcopy(budget)}
        if operation == 'metric':
            self.metrics.append(deepcopy(args))
            return {}
        raise AssertionError('Forbidden drill operation: ' + operation)


class DrillCase(unittest.TestCase):
    def setUp(self):
        directory = tempfile.TemporaryDirectory()
        self.addCleanup(directory.cleanup)
        self.folder = Path(directory.name)
        self.args = SimpleNamespace(authorization=drill.AUTHORIZATION, repository='repo',
                                    producer='producer', revision=SOURCE, root=str(self.folder / 'upstream'),
                                    image='fixture:local', output=str(self.folder / 'report.json'),
                                    phase='exhaust')
        source = Path(self.args.root) / PATH
        source.parent.mkdir(parents=True)
        source.write_bytes(b'// inert offline bytes; never executed\n')
        self.original = dict(path=PATH, sha256=hashlib.sha256(source.read_bytes()).hexdigest(),
                             variants=['non-strict', 'strict'], metadata={}, state='runnable', reasons=[])
        self.normalized = normalize_fixture(self.original, self.args.root)
        self.base = dict(upstream=dict(commit=UPSTREAM, cloneUrl='https://example.invalid/test262'),
                         inventory='corpus', binaries={'host.dll': '1' * 64},
                         harness={'assert.js': '2' * 64}, tooling={'worker.py': '3' * 64},
                         capabilities={'includes': []}, environment_identity={'runtime': 'offline'},
                         timeouts={'runtime': 30, 'compile': 60, 'cap_ms': 120000})
        fixture = dict(fixture_id=FIXTURE, upstream_path=PATH, corpus_id='corpus',
                       content_sha256=bytea(self.normalized['sha256']), is_support_file=False,
                       dependency_manifest_digest=bytea(self.normalized['dependency_manifest_digest']),
                       metadata={}, metadata_state='valid')
        self.client = Catalogue(deepcopy(self.base), fixture)
        archive = self.folder / 'fixture-image.tar.gz'
        archive.write_bytes(b'offline frozen image archive; never loaded')
        self.args.image_archive = str(archive)
        self.baseline_source = dict(run_id=100, run_attempt=1, artifact_id=70,
                                    zip_sha256='4' * 64, report_sha256='5' * 64)
        document = dict(self.base, operator_recovery_drill={
            'campaign': drill.AUTHORIZATION, 'fixture_image': IMAGE})
        self.args.baseline_report = dict(
            phase='verify', source_revision=SOURCE, corpus='corpus',
            provenance_id=identity('repo', 'native', sha(document)), native_provenance_id='base',
            native_execution_identity_sha256=sha(self.base), execution_identity_sha256=sha(document),
            fixture_image_id=IMAGE, image_archive_sha256=hashlib.sha256(archive.read_bytes()).hexdigest(),
            control_plane_sha256=sha(dict(
                authority=self.client.contract,
                native_target=self.client.tables['reporting_targets'][0],
                mvp_target=self.client.tables['reporting_targets'][1],
                reconciliation=self.client.tables['reconciliation_state'][0])),
            fixture={k: fixture[k] for k in (
                'fixture_id', 'upstream_path', 'content_sha256', 'dependency_manifest_digest')},
            required_variants=['non-strict', 'strict'])
        self.physical = []
        self.after_execute = None
        self.fake_outcome = 'pass'
        self.worker_images = []
        self.patches = {}
        for owner, name, options in [
                (commands, 'git', {'side_effect': self.git}),
                (commands, 'current_master', {'return_value': SOURCE}),
                (commands, 'github_validation', {'return_value': {'id': 42, 'head_sha': SOURCE}}),
                (commands.catalog, 'bridge', {'side_effect': lambda request: [deepcopy(self.original)]}),
                (drill, 'execution_identity', {'side_effect': lambda *a: deepcopy(self.base)}),
                (drill, 'image_id', {'return_value': IMAGE}),
                (drill.subprocess, 'check_output', {'side_effect': self.subprocess_output}),
                (drill.subprocess, 'run', {'side_effect': AssertionError('No real external processes')}),
                (drill, 'stage_runtime', {'return_value': None}),
                (drill, 'work_staged', {'side_effect': self.fake_worker}),
                (drill, 'github', {'side_effect': AssertionError('Unexpected GitHub request')}),
                (drill, 'Client', {'side_effect': AssertionError('No real database clients')}),
                (worker, 'run_container', {'side_effect': AssertionError('No real fixture execution')})]:
            p = patch.object(owner, name, **options)
            self.patches[name] = p.start()
            self.addCleanup(p.stop)
        self.report = {'actions': {'id': 123, 'run_attempt': 1, 'head_sha': SOURCE},
                       'baseline_source': deepcopy(self.baseline_source)}

    @staticmethod
    def git(*args):
        if args == ('rev-parse', 'HEAD'):
            return SOURCE
        if args == ('status', '--porcelain', '--untracked-files=no') or args[0] == 'diff':
            return ''
        raise AssertionError('Unexpected git request')

    @staticmethod
    def subprocess_output(command, **kwargs):
        if command[0] == 'git' and command[-2:] == ['rev-parse', 'HEAD']:
            return UPSTREAM + '\n'
        if command[0] == 'git' and 'status' in command:
            return ''
        raise AssertionError('Unexpected external command')

    def fake_worker(self, client, args, *, before_execute):
        self.worker_images.append(args.image)
        outbox = Outbox(args.outbox, args.repository, args.producer, client.epoch)
        count = 0
        try:
            outbox.flush(client)
            while True:
                lease = client.call('claim', args.run, args.budget, args.cap_ms, 151)
                if lease is None or lease.get('budget_exhausted'):
                    break
                before_execute(lease)
                self.physical.append(deepcopy(lease))
                self.client.events.append(('physical', lease['variant']))
                row = dict(observation_id='observation-' + lease['variant'], run_id=args.run,
                           provenance_id=lease['provenance_id'], fixture_id=FIXTURE,
                           work_item_id=lease['work_item_id'], variant=lease['variant'],
                           outcome=self.fake_outcome, lease_generation=1,
                           payload_sha256=bytea(sha({'outcome': self.fake_outcome, 'offline': True})))
                outbox.enqueue([row])
                outbox.flush(client)
                count += 1
                if self.after_execute:
                    self.after_execute(count)
            client.call('metric', args.run, 'screening', 1, 0, count, 0)
            return dict(attempts=count, stop_reason='budget-exhausted' if lease else 'queue-empty-or-delayed')
        finally:
            outbox.db.close()

    def run_drill(self):
        self.report.setdefault('baseline_source', deepcopy(self.baseline_source))
        with redirect_stdout(io.StringIO()):
            return drill.run(self.client, self.args, self.report)

    @staticmethod
    def authorize_main(args):
        args.revision = SOURCE
        return {'id': 123, 'run_attempt': 1, 'head_sha': SOURCE}

    def prepare_recovery(self):
        previous = deepcopy(self.report)
        self.args.previous_report = previous
        self.args.phase = 'recover'
        self.args.output = str(Path(tempfile.mkdtemp(dir=self.folder)) / 'report.json')
        self.report = {'actions': {'id': 456, 'run_attempt': 1, 'head_sha': SOURCE},
                       'baseline_source': deepcopy(self.baseline_source)}
        return previous

    def run_both(self):
        self.run_drill()
        self.prepare_recovery()
        return self.run_drill()

    def assert_no_mutations(self):
        self.assertEqual([e for e in self.client.events if e[0] in ('put', 'call')], [])
        self.assertEqual(self.physical, [])


class InvocationTests(DrillCase):
    def environment(self):
        return dict(GITHUB_REPOSITORY=drill.REPOSITORY, GITHUB_EVENT_NAME='workflow_dispatch',
                    GITHUB_REF='refs/heads/master', GITHUB_RUN_ATTEMPT='1',
                    GITHUB_SHA=SOURCE, GITHUB_RUN_ID='123')

    def authenticated_run(self):
        return dict(id=123, run_attempt=1, repository={'full_name': drill.REPOSITORY},
                    head_repository={'full_name': drill.REPOSITORY}, head_sha=SOURCE,
                    head_branch='master', path=drill.WORKFLOW, event='workflow_dispatch',
                    status='in_progress', html_url='https://example.invalid/actions/123')

    def test_exact_manual_invocation_returns_only_scoped_proof(self):
        self.patches['github'].side_effect = None
        self.patches['github'].return_value = self.authenticated_run()
        with patch.dict(os.environ, self.environment(), clear=True):
            result = drill.check_invocation(self.args)
        self.assertEqual(result, {k: self.authenticated_run()[k] for k in
                                 ('id', 'run_attempt', 'head_sha', 'head_branch', 'event', 'path', 'html_url')})
        self.patches['github'].assert_called_once_with(
            'repos/' + drill.REPOSITORY + '/actions/runs/123/attempts/1')
        self.assertEqual(self.args.revision, SOURCE)
        self.assert_no_mutations()

    def test_authorization_and_environment_mismatches_fail_before_remote_lookup(self):
        cases = [('authorization', None, ''), ('wrong-token', None, 'wrong'),
                 ('repository', 'GITHUB_REPOSITORY', 'foreign/repository'),
                 ('event', 'GITHUB_EVENT_NAME', 'push'), ('branch', 'GITHUB_REF', 'refs/heads/topic'),
                 ('attempt', 'GITHUB_RUN_ATTEMPT', '2'), ('missing-attempt', 'GITHUB_RUN_ATTEMPT', ''),
                 ('sha-prefix', 'GITHUB_SHA', SOURCE[:12]), ('sha-upper', 'GITHUB_SHA', 'A' * 40),
                 ('run-zero', 'GITHUB_RUN_ID', '0'), ('run-nonnumeric', 'GITHUB_RUN_ID', 'run')]
        for label, key, value in cases:
            with self.subTest(case=label):
                env = self.environment()
                self.args.authorization = drill.AUTHORIZATION
                if key:
                    env[key] = value
                else:
                    self.args.authorization = value
                with patch.dict(os.environ, env, clear=True), self.assertRaises(ValueError):
                    drill.check_invocation(self.args)
        self.patches['github'].assert_not_called()
        self.assert_no_mutations()

    def test_authenticated_workflow_identity_rejects_each_foreign_field(self):
        cases = [('id', 124), ('run_attempt', 2), ('repository', {'full_name': 'foreign/repo'}),
                 ('head_repository', {'full_name': 'foreign/repo'}), ('head_sha', 'd' * 40),
                 ('head_branch', 'topic'), ('path', '.github/workflows/other.yml'),
                 ('event', 'push'), ('status', 'completed')]
        self.patches['github'].side_effect = None
        for key, value in cases:
            with self.subTest(field=key):
                run = self.authenticated_run()
                run[key] = value
                self.patches['github'].return_value = run
                with patch.dict(os.environ, self.environment(), clear=True), self.assertRaises(ValueError):
                    drill.check_invocation(self.args)
        self.assert_no_mutations()

    def test_dirty_wrong_or_superseded_source_stops_before_github(self):
        for label in ('dirty', 'head', 'master'):
            with self.subTest(case=label):
                self.patches['git'].side_effect = (
                    lambda *a: 'modified' if a[0] == 'status' else SOURCE) if label == 'dirty' else (
                    lambda *a: 'd' * 40 if a[0] == 'rev-parse' else '')
                if label == 'master':
                    self.patches['git'].side_effect = self.git
                    self.patches['current_master'].return_value = 'd' * 40
                with patch.dict(os.environ, self.environment(), clear=True), self.assertRaises(ValueError):
                    drill.check_invocation(self.args)
        self.patches['github'].assert_not_called()
        self.assert_no_mutations()


class IdentitySelectionTests(DrillCase):
    def test_image_identity_requires_exact_immutable_sha256_digest(self):
        for label, value in [('tag', 'fixture:latest'), ('short', 'sha256:abc'),
                             ('upper', 'sha256:' + 'A' * 64), ('empty', '')]:
            with self.subTest(case=label):
                self.patches['check_output'].side_effect = None
                self.patches['check_output'].return_value = value
                with self.assertRaisesRegex(ValueError, 'Immutable'):
                    REAL_IMAGE_ID(self.args)
        self.patches['check_output'].return_value = IMAGE + '\n'
        self.assertEqual(REAL_IMAGE_ID(self.args), IMAGE)
        self.patches['check_output'].assert_called_with(
            ['docker', 'image', 'inspect', self.args.image, '--format', '{{.Id}}'], text=True)
        self.assert_no_mutations()

    def test_control_plane_requires_every_authority_field(self):
        for key, value in [('repository_id', 'foreign'), ('producer_id', 'foreign'),
                           ('permission', 'worker'), ('trust_class', 'legacy'),
                           ('deployment_state', 'inactive'), ('minimum_writer_epoch', 6)]:
            with self.subTest(field=key):
                original = deepcopy(self.client.contract)
                self.client.contract[key] = value
                with self.assertRaises(ValueError):
                    drill.control_plane(self.client, self.args)
                self.client.contract = original
        proof = drill.control_plane(self.client, self.args)
        self.assertEqual(proof['authority'], self.client.contract)
        self.assertEqual(proof['reconciliation']['fallback_cursor'], 91)
        self.assert_no_mutations()

    def test_full_execution_identity_rejects_each_nested_component_drift(self):
        expected = deepcopy(self.base)
        for key in ('binaries', 'harness', 'tooling', 'capabilities', 'environment_identity', 'timeouts'):
            with self.subTest(component=key):
                self.base = deepcopy(expected)
                self.base[key]['drift'] = 'changed'
                with self.assertRaisesRegex(ValueError, 'identity/image'):
                    drill.check_identity(self.args, 'corpus', expected, IMAGE)
        self.base = expected
        self.patches['image_id'].return_value = 'sha256:' + 'd' * 64
        with self.assertRaisesRegex(ValueError, 'identity/image'):
            drill.check_identity(self.args, 'corpus', expected, IMAGE)
        self.assert_no_mutations()

    def test_upstream_pin_dirty_source_and_master_are_rechecked(self):
        for label in ('upstream', 'upstream-dirty', 'source', 'source-dirty', 'master'):
            with self.subTest(case=label):
                self.patches['check_output'].side_effect = self.subprocess_output
                self.patches['git'].side_effect = self.git
                self.patches['current_master'].return_value = SOURCE
                if label.startswith('upstream'):
                    self.patches['check_output'].side_effect = (
                        lambda command, **k: 'd' * 40 if command[-1] == 'HEAD' else '') if label == 'upstream' else (
                        lambda command, **k: UPSTREAM if command[-1] == 'HEAD' else 'modified')
                elif label == 'master':
                    self.patches['current_master'].return_value = 'd' * 40
                else:
                    self.patches['git'].side_effect = (
                        lambda *a: 'd' * 40 if a[0] == 'rev-parse' else '') if label == 'source' else (
                        lambda *a: SOURCE if a[0] == 'rev-parse' else 'modified')
                with self.assertRaises(ValueError):
                    drill.check_identity(self.args, 'corpus', self.base, IMAGE)
        self.assert_no_mutations()

    def select(self):
        target = self.client.tables['reporting_targets'][0]
        return drill.select_fixture(self.client, self.args, target, self.base)

    def test_selection_returns_exact_verified_two_variant_fixture(self):
        fixture, row, reasons = self.select()
        self.assertEqual(fixture['fixture_id'], FIXTURE)
        self.assertEqual(row['sha256'], hashlib.sha256((Path(self.args.root) / PATH).read_bytes()).hexdigest())
        self.assertEqual(row['dependency_manifest_digest'], sha([]))
        self.assertEqual([(v['variant'], v['required']) for v in row['variants']],
                         [('non-strict', True), ('strict', True)])
        self.assertEqual(reasons, [])
        self.assert_no_mutations()

    def test_selection_skips_registered_support_invalid_foreign_and_ineligible_candidates(self):
        original = deepcopy(self.client.tables)
        for label in ('registered', 'support', 'metadata', 'corpus', 'ineligible', 'empty'):
            with self.subTest(case=label):
                self.client.tables = deepcopy(original)
                fixture = self.client.tables['fixtures'][0]
                if label == 'registered':
                    self.client.tables['registrations'] = [dict(snapshot_id='snapshot', upstream_path=PATH)]
                elif label == 'support':
                    fixture['is_support_file'] = True
                elif label == 'metadata':
                    fixture['metadata_state'] = 'error'
                elif label == 'corpus':
                    fixture['corpus_id'] = 'foreign'
                elif label == 'ineligible':
                    self.client.tables['provenance_fixture_eligibility'][0]['eligibility'] = 'harness-gap'
                else:
                    self.client.tables['provenance_fixture_eligibility'] = []
                with self.assertRaisesRegex(ValueError, 'No eligible'):
                    self.select()
        self.assert_no_mutations()

    def test_selection_rejects_missing_duplicate_extra_and_optional_variants(self):
        cases = [('empty', []), ('strict-only', [('strict', True)]),
                 ('duplicate', [('strict', True), ('strict', True)]),
                 ('extra', [('strict', True), ('non-strict', True), ('module', True)]),
                 ('optional', [('strict', True), ('non-strict', False)])]
        for label, variants in cases:
            with self.subTest(case=label):
                self.client.tables['fixture_variants'] = [
                    dict(fixture_id=FIXTURE, variant=v, required=r) for v, r in variants]
                with self.assertRaisesRegex(ValueError, 'No eligible'):
                    self.select()
        self.assert_no_mutations()

    def test_selection_rejects_unsealed_stale_and_mismatched_metadata_or_dependencies(self):
        tables = deepcopy(self.client.tables)
        for label in ('unsealed', 'stale', 'hash', 'dependency', 'metadata', 'local-variants'):
            with self.subTest(case=label):
                self.client.tables = deepcopy(tables)
                self.original['variants'] = ['non-strict', 'strict']
                if label in ('unsealed', 'stale'):
                    snapshot = self.client.tables['registration_snapshots'][0]
                    snapshot['state' if label == 'unsealed' else 'source_revision'] = 'wrong'
                elif label == 'local-variants':
                    self.original['variants'] = ['strict']
                else:
                    key = {'hash': 'content_sha256', 'dependency': 'dependency_manifest_digest',
                           'metadata': 'metadata'}[label]
                    self.client.tables['fixtures'][0][key] = {} if key == 'metadata' else bytea('d' * 64)
                    if key == 'metadata':
                        self.client.tables['fixtures'][0][key] = {'flags': ['onlyStrict']}
                with self.assertRaises(ValueError):
                    self.select()
        self.assert_no_mutations()


class DriverTests(DrillCase):
    def test_invalid_phase_or_unbound_previous_report_is_rejected_before_journal_and_writes(self):
        for label, phase, previous in [('unknown', 'foreign', None), ('unbound', 'recover', None),
                                       ('unexpected', 'exhaust', {'unexpected': True})]:
            with self.subTest(case=label):
                self.args.phase, self.args.previous_report = phase, previous
                with patch.object(drill, 'checkpoint') as checkpoint, self.assertRaises(ValueError):
                    self.run_drill()
                checkpoint.assert_not_called()
        self.assert_no_mutations()

    def test_two_allocations_recover_only_missing_variant_without_shared_writes(self):
        before = deepcopy({k: self.client.tables[k] for k in
                           ('reporting_targets', 'reconciliation_state', 'corpora', 'registrations')})
        first_result = deepcopy(self.run_drill())
        self.assertIs(first_result['phase_complete'], True)
        self.assertIs(first_result['complete'], False)
        self.assertEqual(first_result['actual_fixture_executions'], 1)
        self.prepare_recovery()
        result = self.run_drill()
        self.assertTrue(result['complete'])
        self.assertEqual(result['stage'], 'verified')
        self.assertEqual(result['actual_fixture_executions'], 2)
        self.assertEqual([r['variant'] for r in self.physical], ['non-strict', 'strict'])
        first, second = result['invocations']
        self.assertNotEqual(first['context']['run'], second['context']['run'])
        self.assertNotEqual(first['context']['budget'], second['context']['budget'])
        self.assertEqual([r['external_run_key'] for r in self.client.tables['runs']],
                         [drill.AUTHORIZATION + ':exhaust', 'actions:123:1',
                          drill.AUTHORIZATION + ':recover', 'actions:456:1'])
        self.assertEqual([r['external_key'] for r in self.client.tables['budget_scopes']],
                         ['actions:123', 'actions:456'])
        self.assertEqual(self.worker_images, [IMAGE, IMAGE])
        markers = [row for row in self.client.tables['runs'] if not row['external_run_key'].startswith('actions:')]
        self.assertEqual([row['state'] for row in markers], ['cancelled', 'cancelled'])
        for marker in markers:
            self.assertIs(marker['environment_diagnostics']['administrative_latch'], True)
            self.assertEqual(marker['environment_diagnostics']['fixture_executions'], 0)
            self.assertEqual(marker['ended_at'], marker['started_at'])
            self.assertNotIn(marker['run_id'], [metric[0] for metric in self.client.metrics])
            self.assertNotIn(marker['run_id'], [o['run_id'] for o in self.client.tables['observations']])
            finishes = [e[2] for e in self.client.events
                        if e[:2] == ('call', 'finish_run') and e[2][0] == marker['run_id']]
            self.assertEqual(len(finishes), 1)
            self.assertEqual(finishes[0][1:], (
                'cancelled', marker['started_at'],
                dict(source='supervisor', external_run_key=marker['external_run_key'],
                     source_revision=SOURCE, reason='job-cancelled', flush_verified=True)))
        self.assertEqual([m[0] for m in self.client.metrics],
                         [first['context']['run'], second['context']['run']])
        self.assertEqual([m[4] for m in self.client.metrics], [1, 1])
        self.assertEqual(first['context']['budget'], identity('repo', 'budget', 'actions:123'))
        self.assertEqual(second['context']['budget'], identity('repo', 'budget', 'actions:456'))
        self.assertEqual(first['worker'], dict(attempts=1, stop_reason='budget-exhausted'))
        self.assertEqual(first['context']['recovery'],
                         dict(resumed_fixtures=0, discovery_fixtures=1, queued_variants=2,
                              reused_terminal_variants=0, cursor_before=0, cursor_after=1))
        self.assertEqual(second['context']['recovery'],
                         dict(resumed_fixtures=1, discovery_fixtures=0, queued_variants=1,
                              reused_terminal_variants=1, cursor_before=1, cursor_after=1))
        self.assertEqual([r['state'] for r in first['work']], ['completed', 'pending'])
        self.assertEqual([r['state'] for r in second['work']], ['completed', 'completed'])
        self.assertEqual(first['budget'], self.client.one(
            'budget_scopes', budget_scope_id=first['context']['budget']))
        self.assertEqual((first['budget']['state'], second['budget']['state']), ('exhausted', 'closed'))
        for invocation in (first, second):
            self.assertEqual(invocation['budget']['attempt_limit'], 1)
            self.assertEqual(invocation['budget']['charged_attempts'], 1)
            self.assertEqual((invocation['budget']['reserved_attempts'], invocation['budget']['reserved_ms']), (0, 0))
            self.assertEqual(invocation['outbox'], dict(integrity='ok', pending_messages=0, pending_completions=0))
        self.assertEqual(first['observations'][0], second['observations'][0])
        enrollments = [e[2] for e in self.client.events if e[:2] == ('put', 'budget_work_items')]
        self.assertEqual([len(rows) for rows in enrollments], [2, 1])
        self.assertEqual(enrollments[1][0]['work_item_id'], self.physical[1]['work_item_id'])
        self.assertEqual({k: self.client.tables[k] for k in before}, before)
        self.assertEqual([e[1] for e in self.client.events if e[0] == 'lock'],
                         [(int(sha(drill.AUTHORIZATION)[:15], 16),)] * 2)
        for flag in ('publication', 'authority_changed', 'permissions_changed', 's3_access',
                     'imports_or_restores', 'cron_witness'):
            self.assertIs(result[flag], False)
        self.assertEqual(json.loads(Path(self.args.output).read_text()), result)

    def test_completed_campaign_replay_different_dispatch_or_source_never_resets_lifetime(self):
        self.run_both()
        original = deepcopy(self.client.tables)
        for label in ('same-dispatch', 'different-dispatch', 'different-source'):
            with self.subTest(case=label):
                self.args.phase = 'exhaust'
                self.args.previous_report = None
                self.report = {'actions': {'id': 123 if label == 'same-dispatch' else 789, 'run_attempt': 1}}
                if label == 'different-source':
                    self.args.revision = 'd' * 40
                    self.client.tables['registration_snapshots'][0]['source_revision'] = self.args.revision
                    self.patches['git'].side_effect = lambda *a: self.args.revision if a[0] == 'rev-parse' else ''
                    self.patches['current_master'].return_value = self.args.revision
                    # Preserve the original replay test's focus: even an otherwise
                    # matching newly verified source cannot replace the lifetime latch.
                    self.args.baseline_report['source_revision'] = self.args.revision
                with patch.dict(os.environ, {'GITHUB_RUN_ID': '999' if label != 'same-dispatch' else '123'}):
                    with self.assertRaisesRegex(ValueError, 'already started'):
                        self.run_drill()
                self.assertEqual(len(self.physical), 2)
                self.assertFalse(self.report['complete'])
        self.assertEqual(self.client.tables['budget_scopes'], original['budget_scopes'])
        self.assertEqual(self.client.tables['observations'], original['observations'])

    def test_existing_either_marker_latches_campaign_before_any_write(self):
        for index in (0, 1):
            table = 'runs'
            with self.subTest(table=table, phase=index):
                key = drill.AUTHORIZATION + (':exhaust' if index == 0 else ':recover')
                field = 'run_id'
                ident = identity('repo', 'producer', key)
                self.client.tables[table] = [{field: ident, 'external_run_key': key}]
                with self.assertRaisesRegex(ValueError, 'already started'):
                    self.run_drill()
                self.client.tables[table] = []
        self.assert_no_mutations()

    def test_incomplete_inventory_or_target_identity_stops_before_mutations(self):
        tables = deepcopy(self.client.tables)
        for label in ('no-target', 'inventory', 'target-identity', 'upstream-pin', 'upstream-url'):
            with self.subTest(case=label):
                self.client.tables = deepcopy(tables)
                if label == 'no-target':
                    self.client.tables['reporting_targets'] = []
                elif label == 'inventory':
                    self.client.tables['corpora'][0]['inventory_state'] = 'open'
                elif label == 'target-identity':
                    self.client.tables['provenances'][0]['identity_document']['timeouts']['runtime'] = 999
                else:
                    self.client.tables['corpora'][0]['revision' if label == 'upstream-pin' else 'upstream_url'] = 'wrong'
                with self.assertRaises(ValueError):
                    self.run_drill()
                self.assertFalse(self.report['complete'])
        self.assert_no_mutations()

    def test_each_control_plane_drift_stops_before_second_execution(self):
        for table, field in [('reporting_targets', 'version'), ('reconciliation_state', 'fallback_cursor')]:
            with self.subTest(table=table):
                def drift(count):
                    self.client.tables[table][0][field] += 1
                self.run_drill()
                self.prepare_recovery()
                drift(1)
                with self.assertRaisesRegex(ValueError, 'control plane|Control-plane'):
                    self.run_drill()
                self.assertEqual(len(self.physical), 1)
                self.assertEqual(len(self.client.tables['runs']), 2)
                self.assertFalse(self.report['complete'])
            # Each independent case gets a fresh fake without reloading source.
            self.setUp_fresh_catalogue()

    def setUp_fresh_catalogue(self):
        fixture = deepcopy(self.client.tables['fixtures'][0])
        self.client = Catalogue(deepcopy(self.base), fixture)
        self.physical = []
        self.worker_images = []
        self.report = {'actions': {'id': 123, 'run_attempt': 1}}
        self.args.phase = 'exhaust'
        self.args.previous_report = None
        # A new fake catalogue must not replay old local outbox rows.
        self.args.output = str(Path(tempfile.mkdtemp(dir=self.folder)) / 'report.json')

    def test_identity_drift_after_first_execution_stops_recovery(self):
        def drift(count):
            self.patches['image_id'].return_value = 'sha256:' + 'd' * 64
        self.run_drill()
        self.prepare_recovery()
        drift(1)
        with self.assertRaisesRegex(ValueError, 'identity/image|full identity'):
            self.run_drill()
        self.assertEqual(len(self.physical), 1)
        self.assertEqual(len(self.client.tables['runs']), 2)
        self.assertFalse(self.report['complete'])

    def test_guard_rejects_foreign_retry_duplicate_and_excess_leases(self):
        for label in ('fixture', 'provenance', 'variant', 'work', 'generation', 'duplicate', 'excess'):
            with self.subTest(case=label):
                def invalid_worker(client, args, *, before_execute):
                    pid = client.one('runs', run_id=args.run)['provenance_id']
                    lease = dict(fixture_id=FIXTURE, provenance_id=pid, variant='non-strict',
                                 work_item_id=identity('repo', pid, FIXTURE, 'non-strict', 0), lease_generation=1)
                    fields = {'fixture': 'fixture_id', 'provenance': 'provenance_id',
                              'variant': 'variant', 'work': 'work_item_id', 'generation': 'lease_generation'}
                    if label in fields:
                        lease[fields[label]] = 2 if label == 'generation' else 'foreign'
                    elif label == 'duplicate':
                        before_execute(lease)
                    else:
                        before_execute(lease)
                        other = dict(lease, variant='strict', work_item_id=identity('repo', pid, FIXTURE, 'strict', 0))
                        before_execute(other)
                    before_execute(lease)
                    raise AssertionError('Invalid guard returned')
                self.patches['work_staged'].side_effect = invalid_worker
                with self.assertRaises(ValueError):
                    self.run_drill()
                self.assertEqual(self.physical, [])
                self.assertFalse(self.report['complete'])
                self.setUp_fresh_catalogue()

    def test_checkpoint_precedes_every_write_and_execution(self):
        original = drill.checkpoint
        def record(args, report):
            self.client.events.append(('checkpoint', report['stage'], len(report['execution_intents'])))
            original(args, report)
        with patch.object(drill, 'checkpoint', side_effect=record):
            self.run_both()
        events = self.client.events
        self.assertEqual(events[0], ('checkpoint', 'preflight', 0))
        for index, event in enumerate(events):
            if event[0] == 'physical':
                previous = events[index - 1]
                self.assertEqual(previous[0], 'checkpoint')
                self.assertEqual(previous[2], 1 if event[1] == 'non-strict' else 2)
        first_write = next(i for i, e in enumerate(events) if e[0] in ('put', 'call'))
        self.assertTrue(any(e[:2] == ('checkpoint', 'prepare-exhaustion') for e in events[:first_write]))

    def test_checkpoint_io_failure_stops_at_initial_interphase_and_execution_intent(self):
        original = drill.checkpoint
        for label in ('initial', 'interphase', 'intent'):
            with self.subTest(case=label):
                def fail(args, report):
                    if (label == 'initial' or
                            label == 'interphase' and report['stage'] == 'prepare-recovery' or
                            label == 'intent' and len(report['execution_intents']) == 1):
                        raise OSError('uncertain local write')
                    original(args, report)
                with patch.object(drill, 'checkpoint', side_effect=fail), self.assertRaises(OSError):
                    if label == 'interphase':
                        self.run_drill()
                        self.prepare_recovery()
                    self.run_drill()
                self.assertFalse(self.report['complete'])
                self.assertEqual(len(self.physical), 1 if label == 'interphase' else 0)
                if label == 'initial':
                    self.assert_no_mutations()
                self.setUp_fresh_catalogue()

    def test_uncertain_acknowledgment_or_finalization_never_starts_recovery(self):
        for operation in ('claim', 'ingest', 'complete', 'finish_run', 'finish_budget'):
            with self.subTest(operation=operation):
                self.client.fail_operation = operation
                with self.assertRaises(TimeoutError):
                    self.run_drill()
                self.assertEqual(len(self.client.tables['runs']), 1 if operation == 'finish_run' else 2)
                self.assertLessEqual(len(self.physical), 1)
                self.assertFalse(self.report['complete'])
                self.setUp_fresh_catalogue()

    def test_wrong_worker_attempts_stop_reason_and_nonproduct_evidence_fail_closed(self):
        original_worker = self.fake_worker
        for label in ('zero-attempts', 'two-attempts', 'wrong-stop', 'missing', 'infrastructure'):
            with self.subTest(case=label):
                def wrong(client, args, *, before_execute):
                    result = original_worker(client, args, before_execute=before_execute)
                    if label in ('zero-attempts', 'two-attempts'):
                        result['attempts'] = 0 if label == 'zero-attempts' else 2
                    elif label == 'wrong-stop':
                        result['stop_reason'] = 'worker-time-limit'
                    elif label == 'missing':
                        client.tables['observations'] = []
                    else:
                        client.tables['observations'][0]['outcome'] = 'infrastructure-error'
                    return result
                self.patches['work_staged'].side_effect = wrong
                with self.assertRaises(ValueError):
                    self.run_drill()
                self.assertEqual(len(self.physical), 1)
                self.assertFalse(self.report['complete'])
                self.setUp_fresh_catalogue()

    def test_existing_leased_work_stops_before_claim_rpc(self):
        self.client.tables['work_items'].append(dict(
            work_item_id='foreign-work', fixture_id='foreign', provenance_id='foreign',
            variant='strict', state='leased', lease_generation=1))
        with self.assertRaisesRegex(ValueError, 'Existing leased'):
            self.run_drill()
        self.assertEqual(self.physical, [])
        self.assertEqual([e for e in self.client.events if e[:2] == ('call', 'claim')], [])
        self.assertEqual(self.client.tables['work_items'][0]['state'], 'leased')
        self.assertFalse(self.report['complete'])

    def test_guard_rechecks_identity_and_control_plane_between_claim_and_execution(self):
        for label in ('image', 'control'):
            with self.subTest(case=label):
                original_call = self.client.call
                def drift(operation, *args):
                    result = original_call(operation, *args)
                    if operation == 'claim':
                        if label == 'image':
                            self.patches['image_id'].return_value = 'sha256:' + 'd' * 64
                        else:
                            self.client.tables['reporting_targets'][0]['version'] += 1
                    return result
                self.client.call = drift
                with self.assertRaises(ValueError):
                    self.run_drill()
                self.assertEqual(self.physical, [])
                self.assertEqual(len([e for e in self.client.events if e[:2] == ('call', 'claim')]), 1)
                self.assertFalse(self.report['complete'])
                self.patches['image_id'].return_value = IMAGE
                self.setUp_fresh_catalogue()

    def test_previous_budget_marker_and_recovery_latch_must_remain_exact(self):
        self.run_drill()
        previous = self.prepare_recovery()
        saved = deepcopy(self.client.tables)
        for label in ('charges', 'reserved', 'marker', 'recover-marker', 'recover-run', 'recover-budget'):
            with self.subTest(case=label):
                self.client.tables = deepcopy(saved)
                self.report = {'actions': {'id': 456, 'run_attempt': 1}}
                if label in ('charges', 'reserved'):
                    budget = self.client.tables['budget_scopes'][0]
                    budget['charged_attempts' if label == 'charges' else 'reserved_ms'] += 1
                elif label == 'marker':
                    self.client.tables['runs'][0]['source_revision'] = 'd' * 40
                elif label == 'recover-marker':
                    self.client.tables['runs'].append(dict(run_id=identity(
                        'repo', 'producer', drill.AUTHORIZATION + ':recover'),
                        external_run_key=drill.AUTHORIZATION + ':recover'))
                elif label == 'recover-run':
                    self.client.tables['runs'].append(dict(run_id=identity('repo', 'producer', 'actions:456:1')))
                else:
                    self.client.tables['budget_scopes'].append(dict(
                        budget_scope_id=identity('repo', 'budget', 'actions:456')))
                event_count = len(self.client.events)
                with self.assertRaisesRegex(ValueError, '[Pp]revious charges|allocation already exists'):
                    self.run_drill()
                self.assertEqual(len(self.physical), 1)
                self.assertEqual([e for e in self.client.events[event_count:] if e[0] in ('put', 'call')], [])
                self.assertEqual(previous['invocations'][0]['budget']['charged_attempts'], 1)

    def test_recovery_rejects_each_previous_identity_field_before_writes(self):
        self.run_drill()
        previous = self.prepare_recovery()
        for field in ('source_revision', 'execution_identity_sha256', 'fixture_image_id',
                      'provenance_id', 'control_plane_sha256', 'fixture', 'actions'):
            with self.subTest(field=field):
                self.args.previous_report = deepcopy(previous)
                if field == 'fixture':
                    self.args.previous_report[field]['content_sha256'] = bytea('d' * 64)
                elif field == 'actions':
                    self.args.previous_report[field]['id'] = 456
                else:
                    self.args.previous_report[field] = 'wrong'
                self.report = {'actions': {'id': 456, 'run_attempt': 1}}
                count = len(self.client.events)
                with self.assertRaisesRegex(ValueError, 'Previous source/full identity'):
                    self.run_drill()
                self.assertEqual([e for e in self.client.events[count:] if e[0] in ('put', 'call')], [])
                self.assertEqual(len(self.physical), 1)

    def test_all_product_outcomes_complete_but_are_not_misreported_as_passes(self):
        for outcome in ('fail', 'unsupported'):
            with self.subTest(outcome=outcome):
                self.fake_outcome = outcome
                result = self.run_both()
                self.assertIs(result['complete'], True)
                self.assertEqual([o['outcome'] for o in result['invocations'][1]['observations']],
                                 [outcome, outcome])
                self.assertEqual(len(self.physical), 2)
                self.setUp_fresh_catalogue()

    def test_prepared_identity_and_variant_accounting_cannot_escape_fixed_plan(self):
        prepare = commands.prepare_from_inventory
        for label in ('run', 'budget', 'candidate_ids', 'resumed', 'queued', 'reused'):
            with self.subTest(field=label):
                def corrupt(client, args, prepared, *, recovery_drill):
                    context = prepare(client, args, prepared, recovery_drill=recovery_drill)
                    if label in ('run', 'budget'):
                        context[label] = 'foreign'
                    elif label == 'candidate_ids':
                        context[label] = ['foreign']
                    else:
                        key = {'resumed': 'resumed_fixtures', 'queued': 'queued_variants',
                               'reused': 'reused_terminal_variants'}[label]
                        context['recovery'][key] += 1
                    return context
                with patch.object(commands, 'prepare_from_inventory', side_effect=corrupt), self.assertRaises(ValueError):
                    self.run_drill()
                self.patches['work_staged'].assert_not_called()
                self.assertEqual(self.physical, [])
                self.assertFalse(self.report['complete'])
                self.setUp_fresh_catalogue()

    def test_budget_reservation_charge_receipt_and_exhaustion_state_require_exact_evidence(self):
        finalize = drill.lifecycle.finalize
        for label in ('reserved-attempts', 'reserved-ms', 'charges', 'receipt', 'state'):
            with self.subTest(case=label):
                def corrupt(client, args):
                    receipt = finalize(client, args)
                    context = json.loads(Path(args.context).read_text())
                    budget = client.mutable('budget_scopes', 'budget_scope_id', context['budget'])
                    if label == 'receipt':
                        receipt['budget']['result']['charged_attempts'] = 999
                    elif label == 'state':
                        budget['state'] = 'closed'
                        receipt['budget']['result'] = deepcopy(budget)
                    else:
                        key = {'reserved-attempts': 'reserved_attempts', 'reserved-ms': 'reserved_ms',
                               'charges': 'charged_attempts'}[label]
                        budget[key] += 1
                        receipt['budget']['result'] = deepcopy(budget)
                    return receipt
                with patch.object(drill.lifecycle, 'finalize', side_effect=corrupt), self.assertRaises(ValueError):
                    self.run_drill()
                self.assertEqual(len(self.physical), 1)
                self.assertFalse(self.report['complete'])
                self.setUp_fresh_catalogue()

    def test_recovery_rejects_replaced_first_observation_and_noncompleted_work(self):
        self.run_drill()
        previous = self.prepare_recovery()
        tables = deepcopy(self.client.tables)
        fake_worker = self.fake_worker
        for label in ('observation', 'work', 'run'):
            with self.subTest(case=label):
                self.client.tables = deepcopy(tables)
                self.args.previous_report = deepcopy(previous)
                self.report = {'actions': {'id': 456, 'run_attempt': 1}}
                self.args.output = str(Path(tempfile.mkdtemp(dir=self.folder)) / 'report.json')
                self.physical = [dict(variant='non-strict')]
                def corrupt(client, args, *, before_execute):
                    result = fake_worker(client, args, before_execute=before_execute)
                    if label == 'observation':
                        client.tables['observations'][0]['observation_id'] = 'replacement'
                    elif label == 'run':
                        client.tables['observations'][0]['run_id'] = 'foreign'
                    else:
                        client.tables['work_items'][0]['state'] = 'cancelled'
                    return result
                self.patches['work_staged'].side_effect = corrupt
                with self.assertRaisesRegex(ValueError, 'Recovery lost/replaced|Original observation payload'):
                    self.run_drill()
                self.assertFalse(self.report['complete'])
                self.assertEqual(len(self.physical), 2)

    def test_fixed_campaign_key_prevents_different_producer_from_restarting_exhaustion(self):
        self.run_drill()
        budgets = deepcopy(self.client.tables['budget_scopes'])
        self.args.producer = 'different-producer'
        self.client.contract['producer_id'] = self.args.producer
        self.args.baseline_report['control_plane_sha256'] = sha(
            drill.control_plane(self.client, self.args))
        self.report = {'actions': {'id': 789, 'run_attempt': 1}}
        count = len(self.client.events)
        with self.assertRaisesRegex(ValueError, 'already started'):
            self.run_drill()
        self.assertEqual([e for e in self.client.events[count:] if e[0] in ('put', 'call')], [])
        self.assertEqual(self.client.tables['budget_scopes'], budgets)
        self.assertEqual(len(self.physical), 1)

    def test_preclaim_repository_lock_covers_control_read_and_actual_claim(self):
        self.run_drill()
        events = self.client.events
        claims = [i for i, e in enumerate(events) if e[:2] == ('call', 'claim')]
        self.assertEqual(len(claims), 2)
        for index in claims:
            preceding = events[:index]
            begin = max(i for i, e in enumerate(preceding) if e == ('transaction', 'begin'))
            window = preceding[begin:]
            lock_index = next(i for i, e in enumerate(window) if e[0] == 'repository-lock')
            self.assertEqual(window[lock_index], ('repository-lock', ('test262:repo',)))
            after_lock = window[lock_index + 1:]
            self.assertIn(('read', 'reporting_targets', {'channel': 'master', 'evidence_kind': 'native'}), after_lock)
            self.assertIn(('read', 'work_items', {'state': 'leased'}), after_lock)
            self.assertNotIn(('transaction', 'end'), window)
            self.assertEqual(events[index + 1], ('transaction', 'end'))


class PrivatePlannerTests(DrillCase):
    def prepared(self):
        args = SimpleNamespace(**vars(self.args), kind='native', run_key='actions:123:1',
                               budget_key='actions:123', candidate_limit=1, accepted_limit=1,
                               attempt_limit=1, budget_ms=1200000, area='', cap_ms=120000,
                               repository_name=drill.REPOSITORY)
        prepared = dict(source=SOURCE, corpus='corpus', ids={PATH: FIXTURE}, snapshot='snapshot',
                        pid='base', doc=deepcopy(self.base), normalized=[deepcopy(self.normalized)],
                        eligibility=deepcopy(self.client.tables['provenance_fixture_eligibility']))
        return args, prepared, dict(campaign=drill.AUTHORIZATION, fixture_id=FIXTURE, cursor_before=0)

    def test_private_plan_rejects_each_unbounded_or_foreign_option_before_start_run(self):
        cases = ('kind', 'candidate-limit', 'accepted-limit', 'attempt-limit', 'campaign',
                 'negative-cursor', 'third-cursor', 'boolean-cursor', 'foreign-fixture',
                 'extra-fixture', 'missing-variant', 'optional-variant')
        for label in cases:
            with self.subTest(case=label):
                args, prepared, private = self.prepared()
                if label == 'kind':
                    args.kind = 'mvp-composite'
                elif label.endswith('-limit'):
                    setattr(args, label.replace('-', '_'), 2)
                elif label == 'campaign':
                    private['campaign'] = 'foreign'
                elif label.endswith('-cursor'):
                    private['cursor_before'] = {'negative-cursor': -1, 'third-cursor': 2,
                                                 'boolean-cursor': True}[label]
                elif label == 'foreign-fixture':
                    private['fixture_id'] = 'foreign'
                elif label == 'extra-fixture':
                    prepared['normalized'].append(deepcopy(self.normalized))
                elif label == 'missing-variant':
                    prepared['normalized'][0]['variants'].pop()
                else:
                    prepared['normalized'][0]['variants'][0]['required'] = False
                with self.assertRaisesRegex(ValueError, 'fixed single-fixture'):
                    commands.prepare_from_inventory(self.client, args, prepared, recovery_drill=private)
        self.assert_no_mutations()

    def test_private_plan_preserves_terminal_work_and_queues_only_pending_exact_id(self):
        args, prepared, private = self.prepared()
        private['cursor_before'] = 1
        self.client.tables['work_items'] = [
            dict(provenance_id='base', fixture_id=FIXTURE, variant=v, state=state,
                 work_item_id=identity('repo', 'base', FIXTURE, v, 0), lease_generation=generation)
            for v, state, generation in [('non-strict', 'completed', 1), ('strict', 'pending', 0)]]
        old = deepcopy(self.client.tables['work_items'][0])
        target = deepcopy(self.client.tables['reporting_targets'])
        cursor = deepcopy(self.client.tables['reconciliation_state'])
        result = commands.prepare_from_inventory(self.client, args, prepared, recovery_drill=private)
        rows = [e[2] for e in self.client.events if e[:2] == ('put', 'work_items')][0]
        self.assertEqual([(w['variant'], w['work_item_id'], w['attempt_limit']) for w in rows],
                         [('strict', identity('repo', 'base', FIXTURE, 'strict', 0), 1)])
        self.assertEqual(self.client.tables['work_items'][0], old)
        self.assertEqual(self.client.tables['reporting_targets'], target)
        self.assertEqual(self.client.tables['reconciliation_state'], cursor)
        self.assertEqual(result['planning_scope'], dict(private, shared_cursor_changed=False,
                                                        reporting_target_changed=False))
        self.assertEqual(result['recovery'], dict(resumed_fixtures=1, discovery_fixtures=0,
                         queued_variants=1, reused_terminal_variants=1, cursor_before=1, cursor_after=1))
        self.assertEqual([e[1] for e in self.client.events if e[0] == 'call'], ['start_run'])

    def test_private_first_plan_queues_exact_two_variants_without_shared_target_or_cursor_writes(self):
        args, prepared, private = self.prepared()
        targets = deepcopy(self.client.tables['reporting_targets'])
        cursor = deepcopy(self.client.tables['reconciliation_state'])
        result = commands.prepare_from_inventory(self.client, args, prepared, recovery_drill=private)
        work = self.client.tables['work_items']
        self.assertEqual([(w['variant'], w['work_item_id'], w['attempt_limit']) for w in work],
                         [(v, identity('repo', 'base', FIXTURE, v, 0), 1) for v in ('non-strict', 'strict')])
        self.assertEqual(result['candidate_ids'], [FIXTURE])
        self.assertEqual(result['recovery'], dict(resumed_fixtures=0, discovery_fixtures=1,
                         queued_variants=2, reused_terminal_variants=0, cursor_before=0, cursor_after=1))
        self.assertEqual(self.client.tables['budget_scopes'][0]['external_key'], 'actions:123')
        self.assertEqual(self.client.tables['budget_scopes'][0]['attempt_limit'], 1)
        self.assertEqual(self.client.tables['reporting_targets'], targets)
        self.assertEqual(self.client.tables['reconciliation_state'], cursor)
        self.assertEqual({e[1] for e in self.client.events if e[0] == 'put'},
                         {'budget_scopes', 'work_items', 'budget_work_items', 'validation_events'})
        self.assertEqual([e[1] for e in self.client.events if e[0] == 'call'], ['start_run'])


class ClaimProxyTests(unittest.TestCase):
    def test_claim_guard_runs_before_rpc_and_other_operations_delegate_unchanged(self):
        events = []
        client = Mock(epoch=7)
        @contextmanager
        def transaction():
            events.append(('transaction', 'begin'))
            yield
            events.append(('transaction', 'end'))
        client.db = SimpleNamespace(transaction=transaction)
        client.call.side_effect = lambda operation, *values: events.append(('rpc', operation, values)) or {'ok': True}
        proxy = drill.GuardedClaims(client, lambda: events.append(('guard',)))
        self.assertEqual(proxy.epoch, 7)
        self.assertEqual(proxy.call('claim', 'run', 'budget', 120000, 151), {'ok': True})
        self.assertEqual(proxy.call('metric', 'run'), {'ok': True})
        self.assertEqual(events, [('transaction', 'begin'), ('guard',),
                                  ('rpc', 'claim', ('run', 'budget', 120000, 151)),
                                  ('transaction', 'end'),
                                  ('rpc', 'metric', ('run',))])

    def test_claim_guard_uncertainty_never_calls_rpc(self):
        client = Mock()
        @contextmanager
        def transaction():
            try:
                yield
            finally:
                client.rolled_back()
        client.db = SimpleNamespace(transaction=transaction)
        proxy = drill.GuardedClaims(client, Mock(side_effect=OSError('uncertain guard')))
        with self.assertRaises(OSError):
            proxy.call('claim', 'run', 'budget', 120000, 151)
        client.call.assert_not_called()
        client.rolled_back.assert_called_once_with()


class WorkerBoundaryTests(DrillCase):
    def worker_args(self):
        return SimpleNamespace(outbox=str(self.folder / 'worker.sqlite'), repository='repo', producer='producer',
                               run='run', budget='budget', root=self.args.root, cap_ms=120000,
                               kind='native', limit=2, seconds=600)

    def lease(self, variant):
        return dict(fixture_id=FIXTURE, provenance_id='pid', variant=variant, work_item_id='work-' + variant,
                    lease_generation=1)

    def test_worker_hook_runs_for_each_verified_claim_immediately_before_container(self):
        args = self.worker_args()
        client = Mock(epoch=7)
        client.call.side_effect = [self.lease('non-strict'), self.lease('strict'), {}]
        client.one.return_value = deepcopy(self.client.tables['fixtures'][0])
        client.read.return_value = []
        events = []
        outbox = Mock()
        def guard(lease):
            events.append(('guard', lease['variant']))
        def execute(args, fixture, variant, cap, manifest, work):
            events.append(('execute', variant))
            return dict(outcome='pass', phase='runtime')
        with patch.object(worker, 'Outbox', return_value=outbox), \
                patch.object(worker, 'run_container', side_effect=execute), redirect_stdout(io.StringIO()):
            result = worker.work_staged(client, args, before_execute=guard)
        self.assertEqual(events, [('guard', 'non-strict'), ('execute', 'non-strict'),
                                  ('guard', 'strict'), ('execute', 'strict')])
        self.assertEqual((result['attempts'], result['stop_reason']), (2, 'worker-attempt-limit'))
        self.assertEqual([c.args[0][0]['variant'] for c in outbox.enqueue.call_args_list], ['non-strict', 'strict'])
        self.assertEqual([c.args[0][0]['outcome'] for c in outbox.enqueue.call_args_list], ['pass', 'pass'])
        self.assertEqual(outbox.flush.call_count, 3)

    def test_guard_rejection_propagates_without_container_observation_or_metric(self):
        client = Mock(epoch=7)
        client.call.return_value = self.lease('strict')
        client.one.return_value = deepcopy(self.client.tables['fixtures'][0])
        client.read.return_value = []
        outbox = Mock()
        guard = Mock(side_effect=ValueError('authorization changed'))
        with patch.object(worker, 'Outbox', return_value=outbox), self.assertRaisesRegex(ValueError, 'authorization'):
            worker.work_staged(client, self.worker_args(), before_execute=guard)
        guard.assert_called_once_with(self.lease('strict'))
        self.patches['run_container'].assert_not_called()
        outbox.enqueue.assert_not_called()
        self.assertEqual([c.args[0] for c in client.call.call_args_list], ['claim'])

    def test_changed_fixture_bytes_fail_before_guard_or_container(self):
        client = Mock(epoch=7)
        client.call.return_value = self.lease('strict')
        client.one.return_value = dict(self.client.tables['fixtures'][0], content_sha256=bytea('d' * 64))
        client.read.return_value = []
        guard, outbox = Mock(), Mock()
        with patch.object(worker, 'Outbox', return_value=outbox), self.assertRaisesRegex(ValueError, 'bytes differ'):
            worker.work_staged(client, self.worker_args(), before_execute=guard)
        guard.assert_not_called()
        outbox.enqueue.assert_not_called()
        self.patches['run_container'].assert_not_called()


class PreviousReceiptTests(DrillCase):
    def previous(self):
        return dict(schema_version=1, repository=drill.REPOSITORY, stage='exhaustion-verified',
                    campaign=drill.AUTHORIZATION, phase='exhaust', phase_complete=True, complete=False,
                    source_revision=SOURCE, actions={'id': 123, 'run_attempt': 1, 'head_sha': SOURCE},
                    actual_fixture_executions=1, execution_intents=[{'work_item_id': 'work'}],
                    first_observation_sha256=sha({'observation_id': 'offline-observation',
                                                 'payload_sha256': bytea('6' * 64), 'outcome': 'pass'}),
                    invocations=[{'budget': {'state': 'exhausted', 'charged_attempts': 1},
                                  'worker': {'attempts': 1, 'stop_reason': 'budget-exhausted'}}],
                    control_plane_unchanged=True, publication=False, authority_changed=False,
                    permissions_changed=False, s3_access=False, imports_or_restores=False, cron_witness=False)

    def completed_run(self):
        return dict(id=123, run_attempt=1, head_sha=SOURCE, head_branch='master',
                    repository={'full_name': drill.REPOSITORY},
                    head_repository={'full_name': drill.REPOSITORY}, path=drill.WORKFLOW,
                    event='workflow_dispatch', status='completed', conclusion='success')

    def artifact(self, report=None, members=None):
        encoded = json.dumps(self.previous() if report is None else report).encode()
        buffer = io.BytesIO()
        with zipfile.ZipFile(buffer, 'w') as archive:
            for name, value in members if members is not None else [('report.json', encoded)]:
                archive.writestr(name, value)
        raw = buffer.getvalue()
        meta = dict(id=77, name='test262-recovery-drill-123-1', expired=False,
                    size_in_bytes=len(raw), workflow_run={'id': 123, 'head_sha': SOURCE},
                    digest='sha256:' + hashlib.sha256(raw).hexdigest())
        return raw, meta, encoded

    def mock_fetch(self, *, run=None, meta=None, raw=None, artifacts=None):
        default_raw, default_meta, _ = self.artifact()
        replies = [self.completed_run() if run is None else run,
                   {'artifacts': [default_meta if meta is None else meta] if artifacts is None else artifacts},
                   default_raw if raw is None else raw]
        self.patches['github'].side_effect = replies

    def test_previous_receipt_authenticates_run_zip_digest_and_bounded_report(self):
        raw, meta, encoded = self.artifact()
        self.mock_fetch(meta=meta, raw=raw)
        report, proof = drill.fetch_previous(123, SOURCE)
        self.assertEqual(report, self.previous())
        self.assertEqual(proof, dict(run_id=123, run_attempt=1, artifact_id=77,
                                    zip_sha256=hashlib.sha256(raw).hexdigest(),
                                    report_sha256=hashlib.sha256(encoded).hexdigest()))
        calls = self.patches['github'].call_args_list
        self.assertEqual([c.args[0] for c in calls],
                         ['repos/' + drill.REPOSITORY + '/actions/runs/123/attempts/1',
                          'repos/' + drill.REPOSITORY + '/actions/runs/123/artifacts?per_page=100',
                          'repos/' + drill.REPOSITORY + '/actions/artifacts/77/zip'])
        self.assertEqual(calls[-1].kwargs, {'binary': True})
        self.assert_no_mutations()

    def test_previous_run_id_rejects_missing_boolean_zero_negative_and_string(self):
        for label, value in [('missing', None), ('boolean', True), ('zero', 0),
                             ('negative', -1), ('string', '123')]:
            with self.subTest(case=label), self.assertRaises(ValueError):
                drill.fetch_previous(value, SOURCE)
        self.patches['github'].assert_not_called()
        self.assert_no_mutations()

    def test_previous_workflow_rejects_each_identity_outcome_or_attempt_mismatch(self):
        cases = [('id', 124), ('run_attempt', 2), ('head_sha', 'd' * 40),
                 ('head_branch', 'topic'), ('repository', {'full_name': 'foreign/repo'}),
                 ('head_repository', {'full_name': 'foreign/repo'}),
                 ('path', '.github/workflows/other.yml'), ('event', 'push'),
                 ('status', 'in_progress'), ('conclusion', 'failure')]
        for key, value in cases:
            with self.subTest(field=key):
                run = self.completed_run()
                run[key] = value
                self.mock_fetch(run=run)
                with self.assertRaisesRegex(ValueError, 'workflow source/scope/outcome'):
                    drill.fetch_previous(123, SOURCE)
        self.assert_no_mutations()

    def test_previous_artifact_rejects_expiry_size_scope_digest_and_cardinality(self):
        raw, original, _ = self.artifact()
        for label in ('expired', 'zero', 'oversize', 'run', 'source', 'digest', 'length',
                      'missing', 'duplicate', 'foreign-name'):
            with self.subTest(case=label):
                meta = deepcopy(original)
                artifacts = None
                if label == 'expired':
                    meta['expired'] = True
                elif label in ('zero', 'oversize', 'length'):
                    meta['size_in_bytes'] = {'zero': 0, 'oversize': 2000001, 'length': len(raw) + 1}[label]
                elif label in ('run', 'source'):
                    meta['workflow_run']['id' if label == 'run' else 'head_sha'] = 999 if label == 'run' else 'd' * 40
                elif label == 'digest':
                    meta['digest'] = 'sha256:' + 'd' * 64
                elif label == 'missing':
                    artifacts = []
                elif label == 'duplicate':
                    artifacts = [meta, deepcopy(meta)]
                else:
                    meta['name'] = 'test262-recovery-drill-999-1'
                self.mock_fetch(meta=meta, raw=raw, artifacts=artifacts)
                with self.assertRaises(ValueError):
                    drill.fetch_previous(123, SOURCE)
        self.assert_no_mutations()

    def test_previous_report_rejects_tampered_campaign_phase_execution_and_budget_proof(self):
        for label in ('campaign', 'phase', 'phase-complete', 'complete', 'source', 'run', 'attempt',
                      'executions', 'intents', 'invocations', 'budget'):
            with self.subTest(case=label):
                report = self.previous()
                if label in ('campaign', 'phase'):
                    report[label] = 'foreign'
                elif label == 'phase-complete':
                    report['phase_complete'] = False
                elif label == 'complete':
                    report['complete'] = True
                elif label == 'source':
                    report['source_revision'] = 'd' * 40
                elif label in ('run', 'attempt'):
                    report['actions']['id' if label == 'run' else 'run_attempt'] = 999
                elif label == 'executions':
                    report['actual_fixture_executions'] = 2
                elif label in ('intents', 'invocations'):
                    report['execution_intents' if label == 'intents' else 'invocations'] = []
                else:
                    report['invocations'][0]['budget']['state'] = 'open'
                raw, meta, _ = self.artifact(report=report)
                self.mock_fetch(meta=meta, raw=raw)
                with self.assertRaises((ValueError, IndexError)):
                    drill.fetch_previous(123, SOURCE)
        self.assert_no_mutations()

    def test_previous_report_requires_exact_schema_stage_accounting_and_forbidden_effect_flags(self):
        fields = ('schema_version', 'repository', 'stage', 'control_plane_unchanged',
                  'publication', 'authority_changed', 'permissions_changed', 's3_access',
                  'imports_or_restores', 'cron_witness', 'charges', 'attempts', 'stop')
        for label in fields:
            with self.subTest(field=label):
                report = self.previous()
                if label in ('schema_version', 'repository', 'stage'):
                    report[label] = 2 if label == 'schema_version' else 'wrong'
                elif label == 'control_plane_unchanged':
                    report[label] = False
                elif label == 'charges':
                    report['invocations'][0]['budget']['charged_attempts'] = 2
                elif label == 'attempts':
                    report['invocations'][0]['worker']['attempts'] = 2
                elif label == 'stop':
                    report['invocations'][0]['worker']['stop_reason'] = 'worker-time-limit'
                else:
                    report[label] = True
                raw, meta, _ = self.artifact(report=report)
                self.mock_fetch(meta=meta, raw=raw)
                with self.assertRaisesRegex(ValueError, 'verified single-execution'):
                    drill.fetch_previous(123, SOURCE)
        self.assert_no_mutations()

    def test_previous_archive_rejects_missing_renamed_and_oversized_report_without_extraction(self):
        for label, members in [('missing', []), ('nested', [('nested/report.json', b'{}')]),
                               ('oversized', [('report.json', b' ' * 1000001)]),
                               ('invalid-json', [('report.json', b'not-json')])]:
            with self.subTest(case=label):
                raw, meta, _ = self.artifact(members=members)
                self.mock_fetch(meta=meta, raw=raw)
                with patch.object(zipfile.ZipFile, 'extractall', side_effect=AssertionError('No extraction')), \
                        self.assertRaises(ValueError):
                    drill.fetch_previous(123, SOURCE)
        self.assert_no_mutations()

    def test_main_authenticates_previous_evidence_before_database_connection(self):
        def authorize(args):
            args.revision = SOURCE
            return {'id': 456}
        with patch.object(drill, 'check_invocation', side_effect=authorize), \
                patch.object(drill, 'fetch_baseline', return_value=({}, self.baseline_source)), \
                patch.object(drill, 'fetch_previous', side_effect=ValueError('tampered receipt')) as previous, \
                patch.object(drill, 'report_failure'):
            result = drill.main(['--authorization', drill.AUTHORIZATION, '--phase', 'recover',
                                 '--previous-run-id', '123', '--baseline-run-id', '100',
                                 '--image-archive', self.args.image_archive, '--root', self.args.root,
                                 '--output', self.args.output])
        self.assertEqual(result, 1)
        previous.assert_called_once()
        self.patches['Client'].assert_not_called()
        self.assertFalse(json.loads(Path(self.args.output).read_text())['complete'])

    def test_main_rejects_previous_id_on_exhaust_and_missing_id_on_recover(self):
        for label in ('exhaust', 'recover'):
            with self.subTest(phase=label), \
                    patch.object(drill, 'check_invocation', return_value={'id': 456}), \
                    patch.object(drill, 'fetch_previous') as fetch, patch.object(drill, 'report_failure'):
                argv = ['--authorization', drill.AUTHORIZATION, '--phase', label,
                        '--image-archive', self.args.image_archive,
                        '--root', self.args.root, '--output', self.args.output]
                if label == 'exhaust':
                    argv.extend(['--previous-run-id', '123'])
                self.assertEqual(drill.main(argv), 1)
                fetch.assert_not_called()
                self.patches['Client'].assert_not_called()


class OutboxAndEntrypointTests(DrillCase):
    def test_checkpoint_fsyncs_pending_report_before_atomic_replacement(self):
        events = []
        replace = Path.replace
        fsync = os.fsync
        def sync(fd):
            events.append('fsync')
            fsync(fd)
        def promote(path, destination):
            events.append('replace')
            self.assertEqual(events, ['fsync', 'replace'])
            self.assertEqual(path.name, 'report.json.pending')
            self.assertEqual(json.loads(path.read_text()), {'complete': False, 'stage': 'safe'})
            return replace(path, destination)
        with patch.object(drill.os, 'fsync', side_effect=sync), patch.object(Path, 'replace', promote):
            drill.checkpoint(self.args, {'complete': False, 'stage': 'safe'})
        self.assertEqual(json.loads(Path(self.args.output).read_text()), {'complete': False, 'stage': 'safe'})
        self.assertFalse(Path(self.args.output + '.pending').exists())

    def test_checkpoint_fsync_or_replace_failure_never_promotes_incomplete_report(self):
        previous = {'complete': False, 'stage': 'previous'}
        drill.checkpoint(self.args, previous)
        for label in ('fsync', 'replace'):
            with self.subTest(operation=label):
                target, name = (drill.os, 'fsync') if label == 'fsync' else (Path, 'replace')
                with patch.object(target, name, side_effect=OSError('uncertain offline IO')), self.assertRaises(OSError):
                    drill.checkpoint(self.args, {'complete': True, 'stage': 'unverified'})
                self.assertEqual(json.loads(Path(self.args.output).read_text()), previous)
                self.assertTrue(Path(self.args.output + '.pending').is_file())

    def test_main_double_journal_failure_returns_nonzero_and_reports_safe_failure_stages(self):
        client = Mock()
        self.patches['Client'].side_effect = None
        self.patches['Client'].return_value = client
        def fail(client, args, report):
            report.update(stage='execute-exhaustion', complete=True)
            drill.checkpoint(args, report)
        def io_failure(*args):
            raise OSError('offline IO uncertainty')
        with patch.object(drill, 'check_invocation', side_effect=self.authorize_main), \
                patch.object(drill, 'fetch_baseline', return_value=({}, self.baseline_source)), \
                patch.object(drill, 'run', side_effect=fail), \
                patch.object(drill, 'checkpoint', side_effect=io_failure) as journal, \
                patch.object(drill, 'report_failure') as failure:
            result = drill.main(['--authorization', drill.AUTHORIZATION, '--phase', 'exhaust',
                                 '--baseline-run-id', '100', '--image-archive', self.args.image_archive,
                                 '--repository', 'repo', '--producer', 'producer',
                                 '--root', self.args.root, '--output', self.args.output])
        self.assertEqual(result, 1)
        self.assertEqual(journal.call_count, 2)
        self.assertFalse(journal.call_args.args[1]['complete'])
        self.assertEqual([c.args[0]._test262_stage for c in failure.call_args_list],
                         ['recovery-drill.failure-journal', 'recovery-drill.execute-exhaustion'])
        client.close.assert_called_once_with()

    def test_outbox_proof_requires_both_observation_and_completion_receipts(self):
        self.args.outbox, self.args.epoch = str(self.folder / 'proof.sqlite'), 7
        outbox = Outbox(self.args.outbox, 'repo', 'producer', 7)
        try:
            outbox.enqueue([dict(observation_id='observation', work_item_id='work', lease_generation=1)])
            for label in ('message', 'completion'):
                with self.subTest(case=label), self.assertRaisesRegex(ValueError, 'Unacknowledged'):
                    drill.outbox_proof(self.args)
                outbox.db.execute('UPDATE messages SET receipt=?', ('{}',))
                outbox.db.commit()
            outbox.db.execute('UPDATE completions SET receipt=?', ('{}',))
            outbox.db.commit()
            self.assertEqual(drill.outbox_proof(self.args),
                             dict(integrity='ok', pending_messages=0, pending_completions=0))
        finally:
            outbox.db.close()

    def test_outbox_integrity_failure_closes_database(self):
        db = Mock()
        db.execute.return_value.fetchall.return_value = [('corrupt',)]
        self.args.outbox, self.args.epoch = 'unused', 7
        with patch.object(drill, 'Outbox', return_value=SimpleNamespace(db=db)), \
                self.assertRaisesRegex(ValueError, 'integrity'):
            drill.outbox_proof(self.args)
        db.close.assert_called_once_with()

    def test_main_authorizes_before_connect_and_returns_nonzero_sanitized_failure(self):
        output = self.folder / 'failed.json'
        with patch.object(drill, 'check_invocation', side_effect=ValueError('offline secret marker')), \
                patch.object(drill, 'report_failure') as failure:
            result = drill.main(['--authorization', 'wrong', '--phase', 'exhaust',
                                 '--image-archive', self.args.image_archive,
                                 '--root', self.args.root, '--output', str(output)])
        self.assertEqual(result, 1)
        self.patches['Client'].assert_not_called()
        proof = json.loads(output.read_text())
        self.assertEqual(proof, dict(complete=False, stage='authorization', error_class='ValueError'))
        self.assertNotIn('offline secret marker', output.read_text())
        failure.assert_called_once()

    def test_main_closes_client_on_success_and_driver_failure(self):
        for success in (True, False):
            with self.subTest(case='success' if success else 'failure'):
                client = Mock()
                self.patches['Client'].side_effect = None
                self.patches['Client'].return_value = client
                with patch.object(drill, 'check_invocation', side_effect=self.authorize_main), \
                        patch.object(drill, 'fetch_baseline', return_value=({}, self.baseline_source)), \
                        patch.object(drill, 'run', side_effect=None if success else TimeoutError('uncertain')) as run, \
                        patch.object(drill, 'report_failure'):
                    result = drill.main(['--authorization', drill.AUTHORIZATION, '--phase', 'exhaust',
                                         '--baseline-run-id', '100', '--image-archive', self.args.image_archive,
                                         '--root', self.args.root,
                                         '--repository', 'repo', '--producer', 'producer',
                                         '--output', self.args.output])
                self.assertEqual(result, 0 if success else 1)
                self.assertEqual(run.call_args.args[0], client)
                client.close.assert_called_once_with()

    def test_fixture_environment_drops_supervisor_credentials(self):
        env = dict(PATH='/offline/bin', LANG='C', TEST262_DATABASE_URL='offline-db-marker',
                   GH_TOKEN='offline-token-marker', AWS_SECRET_ACCESS_KEY='offline-s3-marker',
                   PGPASSWORD='offline-password-marker', AUTHORIZATION=drill.AUTHORIZATION)
        self.assertEqual(fixture_environment(env), {'PATH': '/offline/bin', 'LANG': 'C'})

    def test_workflow_is_manual_master_token_guarded_with_step_scoped_secrets(self):
        text = (drill.REPO / drill.WORKFLOW).read_text()
        trigger = text.split('on:\n', 1)[1].split('permissions:\n', 1)[0]
        self.assertIn('  workflow_dispatch:\n', trigger)
        self.assertNotIn('push:', trigger)
        self.assertNotIn('schedule:', trigger)
        self.assertNotIn('pull_request', trigger)
        self.assertIn("github.ref == 'refs/heads/master'", text)
        self.assertIn("inputs.authorization == '" + drill.AUTHORIZATION + "'", text)
        self.assertIn('github.run_attempt == 1', text)
        self.assertIn('group: test262-central-native-target', text)
        self.assertIn('cancel-in-progress: false', text)
        self.assertIn('environment: test262-catalogue', text)
        self.assertIn('ref: ${{ github.sha }}', text)
        self.assertIn('persist-credentials: false', text)
        self.assertEqual(text.split('permissions:\n', 1)[1].split('jobs:', 1)[0],
                         '  contents: read\n  actions: read\n')
        steps = text.split('    steps:\n', 1)[1].split('      - ')
        secret_steps = [s for s in steps if 'secrets.' in s]
        self.assertEqual(len(secret_steps), 1)
        self.assertIn('python -m scripts.test262.central.live_recovery_drill', secret_steps[0])
        self.assertIn('TEST262_DATABASE_URL: ${{ secrets.TEST262_COORDINATOR_DATABASE_URL }}', secret_steps[0])
        for forbidden in ('aws ', 's3://', 'central.publication', 'central.importer',
                          'central.history_import', 'central.retained_restore', 'permissions: write-all'):
            self.assertNotIn(forbidden, text)
        self.assertIn('if: always()', text)
        self.assertIn('path: artifacts/test262/recovery-drill', text)
