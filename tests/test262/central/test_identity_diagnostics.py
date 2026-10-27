"""Offline original-identity diagnostics; no database, network, build or fixtures."""
import ast
from contextlib import ExitStack, redirect_stderr
from copy import deepcopy
import io
import json
import os
from pathlib import Path
import re
import sys
import tempfile
from types import ModuleType, SimpleNamespace
import unittest
from unittest.mock import Mock, call, patch

from scripts.test262.central import identity_diagnostics as diagnostic
from scripts.test262.central.client import bytea, identity, sha


SOURCE = '08b50be490e521bbd50c63ca3799f8bb33b58707'
PROVENANCE = 'b60fd015-e157-5f39-a8a6-b26b6ff1638c'
SNAPSHOT = 'eb93f530-6805-50a6-85ce-29bd798dde82'
VALIDATION = 'b2761fd5-fe24-559e-a912-bbecf5058a19'
REPOSITORY_ID = 'faf01df8-aa65-4375-a708-6c1e555f957b'
CONTROLLER = 'c' * 40
UPSTREAM = 'b' * 40
WORKFLOW = '.github/workflows/test262-identity-diagnostics.yml'
DIGEST_A = 'a' * 64
DIGEST_B = 'b' * 64
SECRET = 'postgresql://private-user:private-password@private-host/catalogue'


class JsonValue:
    """Keep parameter payloads inspectable without importing a real DB driver."""

    def __init__(self, value):
        self.obj = value


class IdentityDiagnosticsTests(unittest.TestCase):
    def setUp(self):
        directory = tempfile.TemporaryDirectory()
        self.addCleanup(directory.cleanup)
        self.folder = Path(directory.name)
        self.args = SimpleNamespace(
            authorization='identity-readonly-2340-20261027', profile='refresh',
            root='/offline/upstream', output=str(self.folder / 'report.json'),
            repository=REPOSITORY_ID, producer='scoped-producer', epoch=7)
        self.document = {
            'runner': 'native', 'inventory': 'sealed-corpus',
            'upstream': {'commit': UPSTREAM, 'cloneUrl': 'https://example.invalid/test262'},
            'binaries': {'NativeScreeningHost.dll': DIGEST_A},
            'harness': {'harness/assert.js': DIGEST_A},
            'tooling': {'tests/Jroc.Testing/obj/Release/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs': DIGEST_A},
            'capabilities': {'flags': ['onlyStrict'], 'includes': ['assert.js']},
            'environment_identity': {'node': {'major': 24}, 'dotnet_runtime': '10.0.7'},
            'timeouts': {'runtime': 30, 'compile': 60, 'cap_ms': 120000}}
        self.contract = dict(
            api_contract_version=1, repository_id=REPOSITORY_ID,
            producer_id=self.args.producer, trust_class='trusted',
            permission='coordinator', deployment_state='active', minimum_writer_epoch=7)
        self.rows = {
            'provenances': dict(
                provenance_id=PROVENANCE, repository_id=REPOSITORY_ID,
                evidence_kind='native', identity_version=1,
                identity_sha256=bytea(sha(self.document)),
                identity_document=self.document, corpus_id='sealed-corpus'),
            'corpora': dict(corpus_id='sealed-corpus', inventory_state='sealed',
                            revision=UPSTREAM, upstream_url=self.document['upstream']['cloneUrl']),
            'registration_snapshots': dict(
                snapshot_id=SNAPSHOT, repository_id=REPOSITORY_ID,
                state='sealed', source_revision=SOURCE),
            'validation_events': dict(
                validation_id=VALIDATION, repository_id=REPOSITORY_ID,
                target_revision=SOURCE, conclusion='success', external_run_key='40565545982')}
        self.environment = dict(
            GITHUB_REPOSITORY='tomacox74/js2il', GITHUB_EVENT_NAME='workflow_dispatch',
            GITHUB_REF='refs/heads/master', GITHUB_RUN_ATTEMPT='1',
            GITHUB_SHA=CONTROLLER, GITHUB_RUN_ID='123',
            TEST262_DATABASE_URL=SECRET,
            TEST262_DIAGNOSTIC_FILE=str(self.folder / 'diagnostic.json'))
        self.actions = dict(
            id=123, run_attempt=1, repository={'full_name': 'tomacox74/js2il'},
            head_repository={'full_name': 'tomacox74/js2il'}, head_sha=CONTROLLER,
            head_branch='master', path=WORKFLOW, event='workflow_dispatch',
            status='in_progress', html_url='https://github.com/tomacox74/js2il/actions/runs/123')
        self.stack = ExitStack()
        self.addCleanup(self.stack.close)
        self.stack.enter_context(patch.dict(os.environ, self.environment, clear=True))
        # Fail closed if a test forgets to fake any externally executing seam.
        self.stack.enter_context(patch.object(
            diagnostic.subprocess, 'check_output', side_effect=AssertionError('External command forbidden')))
        self.github = self.stack.enter_context(patch.object(
            diagnostic, 'github', return_value=deepcopy(self.actions)))
        self.execution = self.stack.enter_context(patch.object(
            diagnostic, 'execution_identity', return_value=deepcopy(self.document)))
        psycopg = ModuleType('psycopg')
        psycopg.connect = Mock(side_effect=AssertionError('Real database forbidden'))
        types = ModuleType('psycopg.types')
        json_types = ModuleType('psycopg.types.json')
        json_types.Jsonb = JsonValue
        self.stack.enter_context(patch.dict(sys.modules, {
            'psycopg': psycopg, 'psycopg.types': types, 'psycopg.types.json': json_types}))
        self.connect = psycopg.connect

    def fake_database(self, default='on', transaction='on', row_result=None):
        db = Mock()
        db.__enter__ = Mock(return_value=db)

        def close_connection(*unused):
            db.close()
            return False

        db.__exit__ = Mock(side_effect=close_connection)
        tx = Mock()
        tx.__enter__ = Mock()
        tx.__exit__ = Mock(return_value=False)
        db.transaction.return_value = tx

        def execute(sql, parameters=None):
            if sql == 'SHOW default_transaction_read_only':
                value = default
            elif sql == 'SET TRANSACTION ISOLATION LEVEL REPEATABLE READ READ ONLY':
                return None
            elif sql == 'SHOW transaction_read_only':
                value = transaction
            elif sql == 'SELECT test262.api_contract()':
                value = deepcopy(self.contract)
            elif sql == 'SELECT test262.api_read(%s,%s,%s,%s,%s)':
                self.assertEqual(parameters[0], 7)
                self.assertEqual(parameters[3:], (None, 2))
                table = parameters[1]
                expected_filters = {
                    'provenances': {'provenance_id': PROVENANCE},
                    'corpora': {'corpus_id': self.rows['provenances']['corpus_id']},
                    'registration_snapshots': {'snapshot_id': SNAPSHOT},
                    'validation_events': {'validation_id': VALIDATION}}
                self.assertEqual(parameters[2].obj, expected_filters[table])
                value = row_result if row_result is not None else {
                    'rows': [deepcopy(self.rows[table])], 'next': None}
            else:
                raise AssertionError('Forbidden SQL: ' + sql)
            return SimpleNamespace(fetchone=lambda: (value,))

        db.execute.side_effect = execute
        self.connect.side_effect = None
        self.connect.return_value = db
        # Synthetic rows cannot possess the immutable real UUID. Only bind this
        # exact synthetic document; all changed identity inputs use real UUIDs.
        original_digest = sha(self.document)
        uuid_function = self.stack.enter_context(patch.object(
            diagnostic, 'identity', side_effect=lambda *parts:
            PROVENANCE if parts == (REPOSITORY_ID, 'native', original_digest) else identity(*parts)))
        return db, uuid_function

    def fake_git(self, source_status='', upstream_status='', final_head=SOURCE, final_status=''):
        heads = iter([SOURCE, final_head])
        statuses = iter([source_status, final_status])

        def git(*args):
            if args == ('rev-parse', 'HEAD'):
                return next(heads)
            if args == ('status', '--porcelain', '--untracked-files=no'):
                return next(statuses)
            if args == ('rev-parse', '--is-shallow-repository'):
                return 'true' if self.args.profile == 'drill' else 'false'
            if args == ('-C', self.args.root, 'rev-parse', 'HEAD'):
                return UPSTREAM
            if args == ('-C', self.args.root, 'status', '--porcelain', '--untracked-files=no'):
                return upstream_status
            raise AssertionError('Unexpected git read')

        return patch.object(diagnostic, 'git', side_effect=git)

    def argv(self):
        arguments = ['--authorization', self.args.authorization, '--profile', self.args.profile,
                     '--root', self.args.root, '--output', self.args.output,
                     '--repository', self.args.repository, '--producer', self.args.producer]
        if self.args.epoch is not None:
            arguments.extend(['--epoch', str(self.args.epoch)])
        return arguments

    def test_equal_identity_has_exact_root_and_component_hashes_without_differences(self):
        result = diagnostic.compare(self.document, deepcopy(self.document))
        self.assertTrue(result['matches'])
        self.assertEqual(result['expected_sha256'], sha(self.document))
        self.assertEqual(result['actual_sha256'], sha(self.document))
        self.assertEqual(result['file_differences'], [])
        self.assertEqual(result['field_differences'], [])
        self.assertEqual([row['field'] for row in result['components']], sorted(self.document))
        for row in result['components']:
            self.assertEqual(row, dict(field=row['field'], expected_present=True,
                actual_present=True, expected_sha256=sha(self.document[row['field']]),
                actual_sha256=sha(self.document[row['field']]), matches=True))

    def test_changed_added_missing_files_include_raw_digests_and_generated_obj_paths(self):
        path = next(iter(self.document['tooling']))
        actual = deepcopy(self.document)
        actual['tooling'][path] = DIGEST_B
        actual['binaries'] = {'new.dll': DIGEST_B}
        result = diagnostic.compare(self.document, actual)
        self.assertFalse(result['matches'])
        self.assertEqual(result['actual_sha256'], sha(actual))
        self.assertEqual(result['file_differences'], [
            dict(component='binaries', path='NativeScreeningHost.dll',
                 path_sha256=sha('NativeScreeningHost.dll'), expected_present=True,
                 actual_present=False, expected_sha256=DIGEST_A, actual_sha256=None, status='missing'),
            dict(component='binaries', path='new.dll', path_sha256=sha('new.dll'),
                 expected_present=False, actual_present=True, expected_sha256=None,
                 actual_sha256=DIGEST_B, status='added'),
            dict(component='tooling', path=path, path_sha256=sha(path),
                 expected_present=True, actual_present=True, expected_sha256=DIGEST_A,
                 actual_sha256=DIGEST_B, status='changed')])

    def test_missing_components_distinguish_absence_from_null_and_empty_maps(self):
        result = diagnostic.compare({'tooling': {'x.py': DIGEST_A}, 'compiler_options': None},
                                    {'harness': {}, 'timeouts': None})
        components = {row['field']: row for row in result['components']}
        self.assertEqual(components['compiler_options']['expected_sha256'], sha(None))
        self.assertIsNone(components['compiler_options']['actual_sha256'])
        self.assertFalse(components['harness']['expected_present'])
        self.assertTrue(components['harness']['actual_present'])
        self.assertFalse(components['harness']['matches'])
        self.assertEqual(result['file_differences'][0]['status'], 'missing')
        self.assertEqual(result['field_differences'], [
            dict(field='compiler_options', expected_present=True, actual_present=False,
                 expected_sha256=sha(None), actual_sha256=None),
            dict(field='timeouts', expected_present=False, actual_present=True,
                 expected_sha256=None, actual_sha256=sha(None))])

    def test_nested_capability_environment_timeout_and_unknown_values_are_hash_only(self):
        actual = deepcopy(self.document)
        actual['capabilities']['flags'] = ['raw']
        actual['environment_identity']['node']['major'] = 25
        actual['timeouts']['runtime'] = 31
        actual[SECRET] = {SECRET: SECRET}
        result = diagnostic.compare(self.document, actual)
        fields = {row['field']: row for row in result['field_differences']}
        for field, before, after in [
                ('capabilities.flags', ['onlyStrict'], ['raw']),
                ('environment_identity.node.major', 24, 25), ('timeouts.runtime', 30, 31)]:
            self.assertEqual(fields[field], dict(field=field, expected_present=True,
                actual_present=True, expected_sha256=sha(before), actual_sha256=sha(after)))
        self.assertEqual(fields['redacted-' + sha(SECRET)]['actual_sha256'], sha({SECRET: SECRET}))
        nested = diagnostic.compare({'capabilities': {SECRET: 'before'}},
                                    {'capabilities': {SECRET: SECRET}})
        self.assertEqual(nested['field_differences'][0]['field'], 'capabilities.redacted-' + sha(SECRET))
        self.assertNotIn(SECRET, json.dumps([result, nested]))

    def test_json_boolean_integer_and_float_differences_are_not_python_equal_matches(self):
        before = {'capabilities': {'async': True}, 'timeouts': {'runtime': 1}}
        after = {'capabilities': {'async': 1}, 'timeouts': {'runtime': 1.0}}
        result = diagnostic.compare(before, after)
        self.assertFalse(result['matches'])
        self.assertEqual(result['expected_sha256'], sha(before))
        self.assertEqual(result['actual_sha256'], sha(after))
        self.assertNotEqual(result['expected_sha256'], result['actual_sha256'])
        self.assertEqual(result['field_differences'], [
            dict(field='capabilities.async', expected_present=True, actual_present=True,
                 expected_sha256=sha(True), actual_sha256=sha(1)),
            dict(field='timeouts.runtime', expected_present=True, actual_present=True,
                 expected_sha256=sha(1), actual_sha256=sha(1.0))])
        self.assertEqual([row['matches'] for row in result['components']], [False, False])

    def test_unsafe_paths_are_redacted_but_still_distinguishable_by_hash(self):
        unsafe = ['/private/token', '../token', 'a/../token', './token', 'a//token',
                  'https://private/token', 'C:\\private\\token', 'a\nsecret', SECRET]
        result = diagnostic.compare({'tooling': {path: DIGEST_A for path in unsafe}},
                                    {'tooling': {path: DIGEST_B for path in unsafe}})
        self.assertEqual(len(result['file_differences']), len(unsafe))
        self.assertEqual({row['path_sha256'] for row in result['file_differences']},
                         {sha(path) for path in unsafe})
        for row in result['file_differences']:
            self.assertEqual(row['path'], '<redacted>')
            self.assertEqual((row['expected_sha256'], row['actual_sha256']), (DIGEST_A, DIGEST_B))
        for path in unsafe:
            self.assertNotIn(path, json.dumps(result))

    def test_invalid_identity_shapes_and_file_digests_are_rejected(self):
        for before, after in [
                ([], {}), ({}, None), ({'tooling': []}, {}),
                ({}, {'binaries': 'not-a-map'}),
                ({'harness': {'x.js': 'a' * 63}}, {}),
                ({}, {'tooling': {'x.py': 'A' * 64}}),
                ({'binaries': {'host.dll': SECRET}}, {}),
                ({}, {'binaries': {'host.dll': None}})]:
            with self.subTest(before=before, after=after), self.assertRaises(ValueError):
                diagnostic.compare(before, after)

    def test_database_connection_enforces_read_only_defaults_and_timeouts(self):
        sentinel = Mock()
        self.connect.side_effect = None
        self.connect.return_value = sentinel
        self.assertIs(diagnostic.open_database(), sentinel)
        self.connect.assert_called_once_with(SECRET, autocommit=True,
            options='-c default_transaction_read_only=on -c statement_timeout=120000 -c lock_timeout=15000')

    def test_original_reads_exact_fixed_sql_filters_in_read_only_repeatable_transaction(self):
        db, uuid_function = self.fake_database()
        result = diagnostic.read_original(self.args)
        self.assertEqual(result, dict(
            document=self.document, corpus=self.rows['corpora'], database_read_only=True,
            authority={key: self.contract[key] for key in
                       ('repository_id', 'producer_id', 'minimum_writer_epoch', 'deployment_state')}))
        uuid_function.assert_called_once_with(REPOSITORY_ID, 'native', sha(self.document))
        self.assertEqual([entry.args[0] for entry in db.execute.call_args_list], [
            'SHOW default_transaction_read_only',
            'SET TRANSACTION ISOLATION LEVEL REPEATABLE READ READ ONLY',
            'SHOW transaction_read_only', 'SELECT test262.api_contract()',
            *['SELECT test262.api_read(%s,%s,%s,%s,%s)'] * 4])
        self.assertEqual([entry.args[1][1] for entry in db.execute.call_args_list[4:]],
                         ['provenances', 'corpora', 'registration_snapshots', 'validation_events'])
        self.assertEqual(db.mock_calls[0], call.__enter__())
        self.assertLess(db.mock_calls.index(call.transaction().__enter__()),
                        db.mock_calls.index(call.execute(
                            'SET TRANSACTION ISOLATION LEVEL REPEATABLE READ READ ONLY')))
        self.assertLess(db.mock_calls.index(call.transaction().__exit__(None, None, None)),
                        db.mock_calls.index(call.__exit__(None, None, None)))
        db.close.assert_called_once_with()

    def test_either_read_only_flag_off_blocks_all_api_reads_and_closes_connection(self):
        for default, transaction, statements in [
                ('off', 'on', ['SHOW default_transaction_read_only']),
                ('on', 'off', ['SHOW default_transaction_read_only',
                 'SET TRANSACTION ISOLATION LEVEL REPEATABLE READ READ ONLY',
                 'SHOW transaction_read_only'])]:
            with self.subTest(default=default, transaction=transaction):
                db, _ = self.fake_database(default, transaction)
                with self.assertRaises(ValueError):
                    diagnostic.read_original(self.args)
                self.assertEqual([entry.args[0] for entry in db.execute.call_args_list], statements)
                db.close.assert_called_once_with()

    def test_contract_scope_mismatches_fail_before_baseline_reads(self):
        original = deepcopy(self.contract)
        for key, value in [
                ('api_contract_version', 2), ('repository_id', 'foreign'),
                ('producer_id', 'foreign'), ('trust_class', 'untrusted'),
                ('permission', 'ingest'), ('deployment_state', 'shadow'),
                ('minimum_writer_epoch', 8)]:
            with self.subTest(key=key):
                self.contract = dict(original, **{key: value})
                db, _ = self.fake_database()
                with self.assertRaises(ValueError):
                    diagnostic.read_original(self.args)
                self.assertEqual(db.execute.call_count, 4)
                db.close.assert_called_once_with()
        self.contract = original
        self.args.repository = 'foreign'
        db, _ = self.fake_database()
        with self.assertRaises(ValueError):
            diagnostic.read_original(self.args)
        self.assertEqual(db.execute.call_count, 4)
        db.close.assert_called_once_with()

    def test_missing_ambiguous_or_paginated_baseline_is_not_accepted(self):
        for result in [
                {'rows': [], 'next': None},
                {'rows': [self.rows['provenances']] * 2, 'next': None},
                {'rows': [self.rows['provenances']], 'next': {'provenance_id': 'next'}}]:
            with self.subTest(count=len(result['rows']), next=result['next']):
                db, _ = self.fake_database(row_result=result)
                with self.assertRaises(ValueError):
                    diagnostic.read_original(self.args)
                self.assertEqual(db.execute.call_count, 5)
                db.close.assert_called_once_with()

    def test_api_read_failure_exits_transaction_and_closes_connection_without_probe(self):
        db, _ = self.fake_database()
        execute = db.execute.side_effect

        def fail_read(sql, parameters=None):
            if sql == 'SELECT test262.api_read(%s,%s,%s,%s,%s)':
                raise ConnectionError(SECRET)
            return execute(sql, parameters)

        db.execute.side_effect = fail_read
        with self.assertRaises(ConnectionError):
            diagnostic.read_original(self.args)
        transaction_exit = db.transaction.return_value.__exit__
        self.assertEqual(transaction_exit.call_args.args[0], ConnectionError)
        self.assertEqual(db.__exit__.call_args.args[0], ConnectionError)
        db.close.assert_called_once_with()
        self.execution.assert_not_called()

    def test_each_immutable_baseline_binding_is_enforced(self):
        cases = {
            'provenances': {
                'provenance_id': 'foreign', 'repository_id': 'foreign', 'evidence_kind': 'mvp-composite',
                'identity_version': 2, 'identity_sha256': bytea(DIGEST_B), 'corpus_id': 'foreign'},
            'corpora': {'corpus_id': 'foreign', 'inventory_state': 'draft',
                        'revision': 'd' * 40, 'upstream_url': 'https://foreign.invalid'},
            'registration_snapshots': {'snapshot_id': 'foreign', 'repository_id': 'foreign',
                                       'state': 'draft', 'source_revision': 'd' * 40},
            'validation_events': {'validation_id': 'foreign', 'repository_id': 'foreign',
                                  'target_revision': 'd' * 40, 'conclusion': 'failure',
                                  'external_run_key': '40566656556'}}
        original = deepcopy(self.rows)
        for table, changes in cases.items():
            for key, value in changes.items():
                with self.subTest(table=table, key=key):
                    self.rows = deepcopy(original)
                    self.rows[table][key] = value
                    db, _ = self.fake_database()
                    with self.assertRaises(ValueError):
                        diagnostic.read_original(self.args)
                    db.close.assert_called_once_with()

    def test_self_consistent_rehashed_document_still_requires_original_provenance_uuid(self):
        self.document['timeouts']['runtime'] = 31
        self.rows['provenances']['identity_sha256'] = bytea(sha(self.document))
        db, uuid_function = self.fake_database()
        uuid_function.side_effect = identity
        with self.assertRaises(ValueError):
            diagnostic.read_original(self.args)
        uuid_function.assert_called_once_with(REPOSITORY_ID, 'native', sha(self.document))
        db.close.assert_called_once_with()

    def test_manual_master_controller_and_both_checkout_depths_are_authenticated(self):
        for profile in ('refresh', 'drill'):
            self.args.profile = profile
            with self.subTest(profile=profile), self.fake_git():
                result = diagnostic.check_invocation(self.args)
            self.assertEqual(result, {key: self.actions[key] for key in
                ('id', 'run_attempt', 'head_sha', 'event', 'path', 'html_url')})
        self.assertEqual(self.github.call_args_list, [
            call('repos/tomacox74/js2il/actions/runs/123/attempts/1')] * 2)

    def test_invalid_authorization_environment_or_run_identifiers_never_contact_github(self):
        for attribute, value in [('authorization', 'wrong'), ('profile', 'live')]:
            with self.subTest(attribute=attribute), patch.object(self.args, attribute, value):
                with self.assertRaises(ValueError):
                    diagnostic.check_invocation(self.args)
        for key, value in [
                ('GITHUB_REPOSITORY', 'foreign/repo'), ('GITHUB_EVENT_NAME', 'push'),
                ('GITHUB_REF', 'refs/heads/topic'), ('GITHUB_RUN_ATTEMPT', '2'),
                ('GITHUB_SHA', ''), ('GITHUB_SHA', 'A' * 40),
                ('GITHUB_RUN_ID', '0'), ('GITHUB_RUN_ID', '123/other')]:
            with self.subTest(key=key, value=value), patch.dict(os.environ, {key: value}):
                with self.assertRaises(ValueError):
                    diagnostic.check_invocation(self.args)
        self.github.assert_not_called()
        self.connect.assert_not_called()

    def test_foreign_controller_metadata_stops_before_checkout_or_database(self):
        changes = [
            ('id', 124), ('run_attempt', 2), ('repository', {'full_name': 'foreign/repo'}),
            ('head_repository', {'full_name': 'foreign/repo'}), ('head_sha', 'd' * 40),
            ('head_branch', 'topic'), ('path', '.github/workflows/test262-recovery-drill.yml'),
            ('event', 'push'), ('status', 'completed')]
        for key, value in changes:
            self.github.return_value = dict(deepcopy(self.actions), **{key: value})
            with self.subTest(key=key), patch.object(diagnostic, 'git') as git:
                with self.assertRaises(ValueError):
                    diagnostic.check_invocation(self.args)
                git.assert_not_called()
        self.connect.assert_not_called()

    def test_wrong_dirty_or_wrong_depth_source_is_rejected_for_each_profile(self):
        for profile in ('refresh', 'drill'):
            self.args.profile = profile
            depth = 'true' if profile == 'drill' else 'false'
            for reads in [('d' * 40,), (SOURCE, ' M tracked.py'),
                          (SOURCE, '', 'false' if depth == 'true' else 'true')]:
                with self.subTest(profile=profile, reads=reads), patch.object(
                        diagnostic, 'git', side_effect=reads):
                    with self.assertRaises(ValueError):
                        diagnostic.check_invocation(self.args)
        self.connect.assert_not_called()

    def test_run_reads_real_guarded_baseline_and_reports_equal_or_changed_rebuilds(self):
        for changed in (False, True):
            with self.subTest(changed=changed):
                db, _ = self.fake_database()
                actual = deepcopy(self.document)
                if changed:
                    actual['binaries']['NativeScreeningHost.dll'] = DIGEST_B
                self.execution.return_value = actual
                with self.fake_git():
                    report = diagnostic.run(self.args, {})
                self.assertTrue(report['complete'])
                self.assertEqual(report['stage'], 'compared')
                self.assertEqual(report['comparison']['matches'], not changed)
                self.assertEqual(report['comparison']['expected_sha256'], sha(self.document))
                self.assertEqual(report['comparison']['actual_sha256'], sha(actual))
                self.assertTrue(report['database_read_only'])
                self.assertTrue(report['sealed_upstream_matches'])
                self.assertEqual(report['source_revision'], SOURCE)
                self.assertEqual(report['original_provenance'], PROVENANCE)
                self.assertEqual(report['fixture_executions'], 0)
                for key in ('catalogue_writes', 'publication', 'permission_or_authority_changes',
                            'imports_or_restores', 's3_access',
                            'historical_failed_build_document_retained'):
                    self.assertIs(report[key], False)
                self.assertEqual(json.loads(Path(self.args.output).read_text()), report)
                self.assertFalse(Path(self.args.output + '.pending').exists())
                self.assertNotIn(SECRET, json.dumps(report))
                build, corpus = self.execution.call_args.args
                self.assertEqual(vars(build), dict(root=self.args.root, kind='native',
                    host='scripts/test262/NativeScreeningHost/bin/Release/net10.0/NativeScreeningHost.dll',
                    runtime_timeout=30, compile_timeout=60, cap_ms=120000,
                    compilation_coverage=False))
                self.assertEqual(corpus, 'sealed-corpus')
                db.close.assert_called_once_with()

    def test_wrong_or_dirty_upstream_blocks_identity_probe(self):
        for reads in [('d' * 40,), (UPSTREAM, ' M harness/assert.js')]:
            with self.subTest(reads=reads):
                db, _ = self.fake_database()
                with patch.object(diagnostic, 'check_invocation', return_value={}), \
                        patch.object(diagnostic, 'git', side_effect=reads):
                    with self.assertRaises(ValueError):
                        diagnostic.run(self.args, {})
                self.execution.assert_not_called()
                db.close.assert_called_once_with()

    def test_source_change_after_comparison_is_not_marked_complete(self):
        for final_head, final_status in [('d' * 40, ''), (SOURCE, ' M tracked.py')]:
            with self.subTest(head=final_head, status=final_status):
                db, _ = self.fake_database()
                with self.fake_git(final_head=final_head, final_status=final_status):
                    report = {}
                    with self.assertRaises(ValueError):
                        diagnostic.run(self.args, report)
                self.assertFalse(report['complete'])
                self.assertEqual(report['stage'], 'compare')
                self.assertEqual(report['comparison']['matches'], True)
                db.close.assert_called_once_with()
        self.assertEqual(self.execution.call_count, 2)

    def test_rebuilt_pin_drift_is_an_explicit_sanitized_diagnostic_not_a_fixture_run(self):
        for field, value in [('commit', 'd' * 40), ('cloneUrl', SECRET)]:
            with self.subTest(field=field):
                db, _ = self.fake_database()
                actual = deepcopy(self.document)
                actual['upstream'][field] = value
                self.execution.return_value = actual
                with self.fake_git():
                    report = diagnostic.run(self.args, {})
                self.assertTrue(report['complete'])
                self.assertFalse(report['sealed_upstream_matches'])
                self.assertFalse(report['comparison']['matches'])
                self.assertEqual(report['comparison']['field_differences'], [
                    dict(field='upstream.' + field, expected_present=True, actual_present=True,
                         expected_sha256=sha(self.document['upstream'][field]), actual_sha256=sha(value))])
                self.assertEqual(report['fixture_executions'], 0)
                self.assertNotIn(SECRET, Path(self.args.output).read_text())
                db.close.assert_called_once_with()

    def test_main_returns_success_for_diagnostic_mismatch_and_preserves_hash_report(self):
        self.fake_database()
        self.execution.return_value['timeouts']['compile'] = 61
        with self.fake_git():
            self.assertEqual(diagnostic.main(self.argv()), 0)
        report = json.loads(Path(self.args.output).read_text())
        self.assertTrue(report['complete'])
        self.assertFalse(report['comparison']['matches'])
        self.assertEqual(report['comparison']['field_differences'], [
            dict(field='timeouts.compile', expected_present=True, actual_present=True,
                 expected_sha256=sha(60), actual_sha256=sha(61))])

    def test_main_database_error_closes_connection_and_preserves_sanitized_diagnostic(self):
        db, _ = self.fake_database()
        db.execute.side_effect = RuntimeError(SECRET)
        stderr = io.StringIO()
        with self.fake_git(), redirect_stderr(stderr):
            self.assertEqual(diagnostic.main(self.argv()), 1)
        report = json.loads(Path(self.args.output).read_text())
        failure = json.loads((self.folder / 'diagnostic.json').read_text())
        self.assertFalse(report['complete'])
        self.assertEqual(report['stage'], 'read-original-identity')
        self.assertEqual(report['error_class'], 'RuntimeError')
        self.assertEqual(failure['error_type'], 'RuntimeError')
        self.assertEqual(failure['stage'], 'identity-diagnostic.read-original-identity')
        self.assertNotIn(SECRET, json.dumps([report, failure]) + stderr.getvalue())
        self.execution.assert_not_called()
        db.close.assert_called_once_with()

    def test_main_identity_probe_error_preserves_stage_after_database_has_closed(self):
        db, _ = self.fake_database()

        def fail_probe(*unused):
            db.close.assert_called_once_with()
            raise RuntimeError(SECRET)

        self.execution.side_effect = fail_probe
        stderr = io.StringIO()
        with self.fake_git(), redirect_stderr(stderr):
            self.assertEqual(diagnostic.main(self.argv()), 1)
        report = json.loads(Path(self.args.output).read_text())
        failure = json.loads((self.folder / 'diagnostic.json').read_text())
        self.assertFalse(report['complete'])
        self.assertEqual(report['stage'], 'compute-rebuilt-identity')
        self.assertEqual(report['error_class'], 'RuntimeError')
        self.assertNotIn('comparison', report)
        self.assertEqual(failure['stage'], 'identity-diagnostic.compute-rebuilt-identity')
        self.assertNotIn(SECRET, json.dumps([report, failure]) + stderr.getvalue())

    def test_main_invalid_scope_and_authorization_fail_before_database_or_probe(self):
        for key, value in [('repository', 'foreign'), ('producer', ''),
                           ('epoch', None), ('authorization', 'wrong')]:
            with self.subTest(key=key), patch.object(self.args, key, value), redirect_stderr(io.StringIO()):
                self.assertEqual(diagnostic.main(self.argv()), 1)
            report = json.loads(Path(self.args.output).read_text())
            self.assertFalse(report['complete'])
            self.assertEqual(report['stage'], 'authorization')
            self.assertEqual(report['error_class'], 'ValueError')
        self.connect.assert_not_called()
        self.execution.assert_not_called()
        self.github.assert_not_called()

    def test_main_failed_error_journal_still_returns_nonzero_with_safe_diagnostics(self):
        stderr = io.StringIO()
        with patch.object(self.args, 'authorization', 'wrong'), \
                patch.object(diagnostic, 'write_report', side_effect=OSError(SECRET)), \
                redirect_stderr(stderr):
            self.assertEqual(diagnostic.main(self.argv()), 1)
        self.assertIn('identity-diagnostic.failure-journal', stderr.getvalue())
        self.assertIn('identity-diagnostic.authorization', stderr.getvalue())
        self.assertNotIn(SECRET, stderr.getvalue())
        self.connect.assert_not_called()

    def test_controller_imports_only_identity_and_read_only_support_not_workers(self):
        tree = ast.parse(Path(diagnostic.__file__).read_text())
        modules = set()
        for node in ast.walk(tree):
            if isinstance(node, ast.Import):
                modules.update(alias.name for alias in node.names)
            elif isinstance(node, ast.ImportFrom):
                modules.add(node.module)
        self.assertEqual(modules, {
            'argparse', 'datetime', 'json', 'os', 'pathlib', 're', 'subprocess',
            'sys', 'types', 'psycopg', 'psycopg.types.json',
            'scripts.test262.central.client', 'scripts.test262.central.diagnostics',
            'scripts.test262.central.execution_identity', 'scripts.test262.central.monitoring'})

    def test_workflow_pins_original_profiles_and_isolates_controller_before_checkout(self):
        text = (Path(__file__).resolve().parents[3] / WORKFLOW).read_text()
        trigger = text.split('permissions:', 1)[0]
        self.assertIn('  workflow_dispatch:\n', trigger)
        self.assertNotIn('workflow_run:', trigger)
        self.assertNotIn('schedule:', trigger)
        self.assertEqual(re.findall(r'^  ([a-z_]+):', trigger, re.MULTILINE), ['workflow_dispatch'])
        self.assertIn("github.ref == 'refs/heads/master'", text)
        self.assertIn("inputs.authorization == 'identity-readonly-2340-20261027'", text)
        self.assertIn('github.run_attempt == 1', text)
        self.assertIn('  contents: read\n  actions: read\n', text)
        self.assertNotIn(': write', text)
        self.assertIn('runs-on: ubuntu-24.04', text)
        self.assertIn('- profile: refresh\n            fetch_depth: 0\n            build_cli: false', text)
        self.assertIn('- profile: drill\n            fetch_depth: 1\n            build_cli: true', text)
        self.assertLess(text.index('ref: ${{ github.sha }}'),
                        text.index('cp scripts/test262/central/identity_diagnostics.py'))
        self.assertLess(text.index('cp scripts/test262/central/identity_diagnostics.py'),
                        text.index('ref: ' + SOURCE))
        self.assertIn('cp scripts/test262/central/identity_diagnostics.py "$RUNNER_TEMP/identity_diagnostics.py"', text)
        self.assertIn('fetch-depth: ${{ matrix.fetch_depth }}', text)
        self.assertEqual(text.count('persist-credentials: false'), 2)
        self.assertIn('dotnet-version: 10.0.403', text)
        self.assertIn("node-version: '24.22.1'", text)
        self.assertIn("python-version: '3.12'", text)
        self.assertIn('export GITHUB_SHA=' + SOURCE, text)
        self.assertIn('BUILD_CLI: ${{ matrix.build_cli }}', text)
        self.assertIn('if [ "$BUILD_CLI" = \'true\' ]; then\n'
                      '            dotnet build src/Cli/Jroc.csproj -c Release --nologo\n'
                      '          fi\n'
                      '          dotnet build scripts/test262/NativeScreeningHost/NativeScreeningHost.csproj -c Release --nologo', text)
        compare_step = text.split('      - name: Read immutable original provenance', 1)[1]
        compare_step, upload = compare_step.split('      - name: Retain sanitized', 1)
        before = text.split('      - name: Read immutable original provenance', 1)[0]
        self.assertNotIn('secrets.', before + upload)
        self.assertNotIn('GH_TOKEN:', before + upload)
        self.assertEqual(text.count('secrets.'), 1)
        self.assertIn('TEST262_DATABASE_URL: ${{ secrets.TEST262_COORDINATOR_DATABASE_URL }}', compare_step)
        self.assertIn('GH_TOKEN: ${{ github.token }}', compare_step)
        self.assertIn('python "$RUNNER_TEMP/identity_diagnostics.py"', compare_step)
        self.assertNotIn('export GITHUB_SHA', compare_step)
        self.assertNotRegex(text, r'GITHUB_SHA.*>>.*GITHUB_ENV')
        self.assertIn('--authorization "$AUTHORIZATION" --profile "$PROFILE" --root "$TEST262_ROOT"', compare_step)
        self.assertIn('if: always()', upload)
        for forbidden in ('docker ', 'aws ', 's3://', 'central.cli', 'central.worker',
                          'live_recovery_drill', 'pg_restore', 'api_put', 'git reset', 'git restore'):
            self.assertNotIn(forbidden, text)
