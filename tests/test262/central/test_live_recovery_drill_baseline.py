"""Frozen-baseline contracts; all APIs, database, Docker and workers are fake."""
from contextlib import redirect_stdout
from copy import deepcopy
import hashlib
import io
import json
from pathlib import Path
import subprocess
from types import SimpleNamespace
import unittest
from unittest.mock import Mock, patch
import xml.etree.ElementTree as ET

import test_live_recovery_drill as existing
from scripts.test262.central import live_recovery_drill as drill
from scripts.test262.central.client import Client, sha


class ReadOnlyCatalogue(existing.Catalogue):
    """Use the real Client.view with a fail-closed, transaction-aware database."""

    view = Client.view

    def __init__(self, base, fixture, default='on', transaction='on', fail_setup=False):
        super().__init__(base, fixture)
        self.settings = {'SHOW default_transaction_read_only': default,
                         'SHOW transaction_read_only': transaction}
        self.setup = []
        self.fail_setup = fail_setup
        self.closed = False

    def execute(self, sql, parameters=None):
        expected = ['SET TRANSACTION ISOLATION LEVEL REPEATABLE READ READ ONLY',
                    'SET LOCAL default_transaction_read_only=on',
                    'SHOW default_transaction_read_only', 'SHOW transaction_read_only']
        if sql in expected:
            if self.transaction_depth != 1 or sql != expected[len(self.setup)]:
                raise AssertionError('Read-only SQL order or transaction scope differs')
            self.setup.append(sql)
            self.events.append(('sql', sql))
            if self.fail_setup and sql.startswith('SET LOCAL'):
                raise OSError('synthetic read-only setup failure')
            return SimpleNamespace(fetchone=lambda: (self.settings.get(sql),))
        if self.setup != expected or self.transaction_depth != 1:
            raise AssertionError('Catalogue accessed before read-only settings verified')
        return super().execute(sql, parameters)

    def read(self, table, filters=None):
        if len(self.setup) != 4 or self.transaction_depth != 1:
            raise AssertionError('Catalogue read outside verified read-only transaction')
        return super().read(table, filters)

    def call(self, operation, *args):
        raise AssertionError('Mutation RPC forbidden in baseline: ' + operation)

    def put(self, table, rows):
        raise AssertionError('Catalogue write forbidden in baseline: ' + table)

    def close(self):
        self.closed = True
        self.events.append(('close',))


class BaselineCase(existing.DrillCase):
    def argv(self, phase='verify', *extra):
        return ['--authorization', drill.AUTHORIZATION, '--phase', phase,
                '--repository', 'repo', '--producer', 'producer',
                '--root', self.args.root, '--output', self.args.output,
                '--image-archive', self.args.image_archive, *extra]

    def readonly_client(self, **kwargs):
        return ReadOnlyCatalogue(deepcopy(self.base),
                                 deepcopy(self.client.tables['fixtures'][0]), **kwargs)

    def verified_report(self):
        report = deepcopy(self.args.baseline_report)
        report.update(schema_version=1, repository=drill.REPOSITORY, campaign=drill.AUTHORIZATION,
                      stage='baseline-verified', phase_complete=True, complete=False,
                      baseline_read_only=True, source_revision=existing.SOURCE,
                      actions={'id': 123, 'run_attempt': 1, 'head_sha': existing.SOURCE},
                      actual_fixture_executions=0, execution_intents=[], invocations=[],
                      control_plane_unchanged=True, sealed_upstream_matches=True,
                      preflight_comparison={'matches': True, 'expected_sha256': sha(self.base),
                                            'actual_sha256': sha(self.base)})
        report.update({key: False for key in ('publication', 'authority_changed', 'permissions_changed',
                                             's3_access', 'imports_or_restores', 'cron_witness')})
        return report

    def serve_report(self, report, *, workflow=None, metadata=None):
        raw, meta, encoded = existing.PreviousReceiptTests.artifact(self, report=report)
        if metadata:
            meta.update(metadata)
        run = existing.PreviousReceiptTests.completed_run(self)
        if workflow:
            run.update(workflow)
        self.patches['github'].side_effect = [run, {'artifacts': [meta]}, raw]
        return raw, meta, encoded

    def invoke(self, argv, client=None):
        with patch.object(drill, 'check_invocation', side_effect=self.authorize_main), \
                patch.object(drill, 'report_failure'), redirect_stdout(io.StringIO()):
            if client is None:
                return drill.main(argv)
            with patch.object(drill, 'Client', return_value=client):
                return drill.main(argv)


class VerifyBaselineTests(BaselineCase):
    def test_verify_main_uses_server_readonly_snapshot_and_emits_exact_zero_execution_baseline(self):
        client = self.readonly_client()
        before = deepcopy(client.tables)
        self.assertEqual(self.invoke(self.argv(), client), 0)
        report = json.loads(Path(self.args.output).read_text())
        self.assertEqual((report['stage'], report['phase'], report['phase_complete'], report['complete']),
                         ('baseline-verified', 'verify', True, False))
        self.assertIs(report['baseline_read_only'], True)
        self.assertEqual(report['actual_fixture_executions'], 0)
        self.assertEqual(report['execution_intents'], [])
        self.assertEqual(report['invocations'], [])
        self.assertEqual(report['native_execution_identity_sha256'], sha(self.base))
        for field in self.args.baseline_report:
            self.assertEqual(report[field], self.args.baseline_report[field], field)
        self.assertEqual(report['preflight_comparison']['expected_sha256'], sha(self.base))
        self.assertEqual(report['preflight_comparison']['actual_sha256'], sha(self.base))
        self.assertIs(report['preflight_comparison']['matches'], True)
        self.assertEqual(client.tables, before)
        self.assertEqual(client.events[0], ('transaction', 'begin'))
        self.assertEqual(client.events[-2:], [('transaction', 'end'), ('close',)])
        self.assertTrue(client.closed)
        self.patches['stage_runtime'].assert_not_called()
        self.patches['work_staged'].assert_not_called()
        self.patches['run_container'].assert_not_called()
        self.assert_no_mutations()

    def test_verify_readonly_flags_or_setup_failure_block_all_catalogue_reads_and_workers(self):
        for options in ({'default': 'off'}, {'transaction': 'off'}, {'fail_setup': True}):
            with self.subTest(settings=options):
                client = self.readonly_client(**options)
                self.assertEqual(self.invoke(self.argv(), client), 1)
                self.assertEqual([e for e in client.events if e[0] == 'read'], [])
                self.assertTrue(client.closed)
                self.assertEqual(client.transaction_depth, 0)
                report = json.loads(Path(self.args.output).read_text())
                self.assertFalse(report['complete'])
                self.assertNotIn('baseline_read_only', report)
        self.patches['stage_runtime'].assert_not_called()
        self.patches['work_staged'].assert_not_called()

    def test_verify_existing_either_latch_or_leased_work_cannot_authorize_new_campaign(self):
        for phase in ('exhaust', 'recover', 'leased'):
            with self.subTest(state=phase):
                client = self.readonly_client()
                if phase == 'leased':
                    client.tables['work_items'] = [{'state': 'leased', 'work_item_id': 'unrelated'}]
                else:
                    client.tables['runs'] = [{'external_run_key': drill.AUTHORIZATION + ':' + phase}]
                before = deepcopy(client.tables)
                self.assertEqual(self.invoke(self.argv(), client), 1)
                self.assertEqual(client.tables, before)
                self.assertNotEqual(json.loads(Path(self.args.output).read_text())['stage'], 'baseline-verified')
                self.assertTrue(client.closed)
        self.patches['stage_runtime'].assert_not_called()
        self.patches['work_staged'].assert_not_called()

    def test_verify_native_drift_checkpoints_sanitized_component_evidence_before_rejection(self):
        client = self.readonly_client()
        secret = '/private/not-for-report.dll'
        client.tables['provenances'][0]['identity_document']['binaries'][secret] = 'd' * 64
        self.assertEqual(self.invoke(self.argv(), client), 1)
        encoded = Path(self.args.output).read_text()
        report = json.loads(encoded)
        comparison = report['preflight_comparison']
        self.assertFalse(comparison['matches'])
        self.assertEqual(comparison['actual_sha256'], sha(self.base))
        self.assertEqual(comparison['expected_sha256'],
                         sha(client.tables['provenances'][0]['identity_document']))
        self.assertEqual(comparison['file_differences'][0]['status'], 'missing')
        self.assertEqual(comparison['file_differences'][0]['path_sha256'], sha(secret))
        self.assertNotIn(secret, encoded)
        self.assertEqual(report['error_class'], 'ValueError')
        self.patches['stage_runtime'].assert_not_called()
        self.assert_no_mutations()

    def test_verify_sealed_upstream_revision_or_url_drift_reports_false_before_writes(self):
        for field in ('revision', 'upstream_url'):
            with self.subTest(field=field):
                client = self.readonly_client()
                client.tables['corpora'][0][field] = 'wrong'
                self.assertEqual(self.invoke(self.argv(), client), 1)
                report = json.loads(Path(self.args.output).read_text())
                self.assertIs(report['sealed_upstream_matches'], False)
                self.assertIs(report['preflight_comparison']['matches'], True)
                self.assertEqual(client.tables['runs'], [])
        self.patches['stage_runtime'].assert_not_called()

    def test_verify_control_plane_change_during_probe_prevents_baseline_completion(self):
        client = self.readonly_client()
        def image(args):
            client.tables['reporting_targets'][0]['version'] += 1
            return existing.IMAGE
        self.patches['image_id'].side_effect = image
        self.assertEqual(self.invoke(self.argv(), client), 1)
        report = json.loads(Path(self.args.output).read_text())
        self.assertFalse(report['complete'])
        self.assertNotEqual(report['stage'], 'baseline-verified')
        self.assertEqual(client.tables['runs'], [])
        self.patches['stage_runtime'].assert_not_called()
        self.patches['work_staged'].assert_not_called()


class BaselineAuthenticationTests(BaselineCase):
    def test_previous_requires_integer_one_execution_and_exact_full_observation_digest(self):
        for value in (True, 1.0):
            with self.subTest(count=value, kind=type(value).__name__):
                report = existing.PreviousReceiptTests.previous(self)
                report['actual_fixture_executions'] = value
                self.serve_report(report)
                with self.assertRaisesRegex(ValueError, 'single-execution exhaustion'):
                    drill.fetch_previous(123, existing.SOURCE)
        for digest in ('', 'a' * 63, 'a' * 65, 'A' * 64, 'g' * 64):
            with self.subTest(digest=digest):
                report = existing.PreviousReceiptTests.previous(self)
                report['first_observation_sha256'] = digest
                self.serve_report(report)
                with self.assertRaisesRegex(ValueError, 'single-execution exhaustion'):
                    drill.fetch_previous(123, existing.SOURCE)
        report = existing.PreviousReceiptTests.previous(self)
        del report['first_observation_sha256']
        self.serve_report(report)
        with self.assertRaises((ValueError, KeyError)):
            drill.fetch_previous(123, existing.SOURCE)
        self.assert_no_mutations()

    def test_baseline_requires_integer_zero_execution_count_not_boolean_or_float(self):
        for value in (False, 0.0):
            with self.subTest(value=value, kind=type(value).__name__):
                report = self.verified_report()
                report['actual_fixture_executions'] = value
                self.serve_report(report)
                with self.assertRaises(ValueError):
                    drill.fetch_baseline(123, existing.SOURCE)
        self.assert_no_mutations()

    def test_baseline_authenticates_workflow_artifact_bytes_source_and_report_digest(self):
        expected = self.verified_report()
        raw, meta, encoded = self.serve_report(expected)
        report, receipt = drill.fetch_baseline(123, existing.SOURCE)
        self.assertEqual(report, expected)
        self.assertEqual(receipt, {
            'run_id': 123, 'run_attempt': 1, 'artifact_id': meta['id'],
            'zip_sha256': hashlib.sha256(raw).hexdigest(),
            'report_sha256': hashlib.sha256(encoded).hexdigest()})
        self.assertEqual([c.args[0] for c in self.patches['github'].call_args_list], [
            f'repos/{drill.REPOSITORY}/actions/runs/123/attempts/1',
            f'repos/{drill.REPOSITORY}/actions/runs/123/artifacts?per_page=100',
            f"repos/{drill.REPOSITORY}/actions/artifacts/{meta['id']}/zip"])
        self.assert_no_mutations()

    def test_baseline_rejects_each_workflow_or_artifact_authentication_mismatch(self):
        cases = [('workflow', 'head_sha', 'd' * 40), ('workflow', 'run_attempt', 2),
                 ('workflow', 'conclusion', 'failure'), ('workflow', 'path', 'other.yml'),
                 ('metadata', 'digest', 'sha256:' + 'd' * 64),
                 ('metadata', 'expired', True), ('metadata', 'size_in_bytes', 0),
                 ('metadata', 'workflow_run', {'id': 999, 'head_sha': existing.SOURCE})]
        for owner, field, value in cases:
            with self.subTest(owner=owner, field=field):
                self.serve_report(self.verified_report(), **{owner: {field: value}})
                with self.assertRaises(ValueError):
                    drill.fetch_baseline(123, existing.SOURCE)
        self.assert_no_mutations()

    def test_baseline_rejects_each_phase_readonly_identity_execution_and_effect_mismatch(self):
        cases = [('schema_version', 2), ('repository', 'foreign/repo'), ('campaign', 'replacement'),
                 ('phase', 'exhaust'), ('stage', 'verified'), ('phase_complete', False),
                 ('complete', True), ('baseline_read_only', False),
                 ('source_revision', 'd' * 40), ('actual_fixture_executions', 1),
                 ('execution_intents', [{'work_item_id': 'physical'}]), ('invocations', [{}]),
                 ('control_plane_unchanged', False), ('sealed_upstream_matches', False),
                 ('required_variants', ['strict']), ('native_execution_identity_sha256', 'd' * 64),
                 ('image_archive_sha256', 'A' * 64), ('fixture_image_id', 'fixture:mutable')]
        cases += [(key, True) for key in ('publication', 'authority_changed', 'permissions_changed',
                                         's3_access', 'imports_or_restores', 'cron_witness')]
        for field, value in cases:
            with self.subTest(field=field):
                report = self.verified_report()
                report[field] = value
                self.serve_report(report)
                with self.assertRaisesRegex(ValueError, 'zero-execution frozen identity'):
                    drill.fetch_baseline(123, existing.SOURCE)
        self.assert_no_mutations()

    def test_baseline_rejects_actions_sha_root_comparison_and_missing_required_fields(self):
        for owner, field, value in (
                ('actions', 'head_sha', 'd' * 40), ('actions', 'id', 999),
                ('actions', 'run_attempt', 2), ('preflight_comparison', 'matches', False),
                ('preflight_comparison', 'expected_sha256', 'd' * 64),
                ('preflight_comparison', 'actual_sha256', 'd' * 64)):
            with self.subTest(owner=owner, field=field):
                report = self.verified_report()
                report[owner][field] = value
                self.serve_report(report)
                with self.assertRaises(ValueError):
                    drill.fetch_baseline(123, existing.SOURCE)
        for field in ('baseline_read_only', 'image_archive_sha256', 'fixture_image_id', 'actions'):
            with self.subTest(missing=field):
                report = self.verified_report()
                del report[field]
                self.serve_report(report)
                with self.assertRaises((ValueError, KeyError)):
                    drill.fetch_baseline(123, existing.SOURCE)
        self.assert_no_mutations()

    def test_previous_exhaustion_report_requires_its_own_actions_source_sha(self):
        report = existing.PreviousReceiptTests.previous(self)
        report['actions']['head_sha'] = 'd' * 40
        self.serve_report(report)
        with self.assertRaisesRegex(ValueError, 'single-execution exhaustion'):
            drill.fetch_previous(123, existing.SOURCE)
        self.assert_no_mutations()


class ImageArchiveTests(BaselineCase):
    def test_archive_hash_streams_empty_and_multichunk_exact_bytes_and_detects_one_byte_change(self):
        archive = Path(self.args.image_archive)
        for payload in (b'', b'x' * (2 * 1024 * 1024 + 17)):
            with self.subTest(size=len(payload)):
                archive.write_bytes(payload)
                self.assertEqual(drill.image_archive_sha256(self.args), hashlib.sha256(payload).hexdigest())
                archive.write_bytes(payload + b'y')
                self.assertNotEqual(drill.image_archive_sha256(self.args), hashlib.sha256(payload).hexdigest())
        self.patches['run'].assert_not_called()

    def test_restore_hash_mismatch_or_missing_archive_never_calls_docker_or_inspects_image(self):
        archive = Path(self.args.image_archive)
        archive.write_bytes(b'substituted archive')
        with self.assertRaisesRegex(ValueError, 'do not load'):
            drill.restore_image(self.args)
        self.args.image_archive = str(archive.with_name('absent.tar.gz'))
        with self.assertRaises(FileNotFoundError):
            drill.restore_image(self.args)
        self.patches['run'].assert_not_called()
        self.patches['image_id'].assert_not_called()
        self.assert_no_mutations()

    def test_restore_checks_digest_before_loading_and_requires_loaded_immutable_image(self):
        events = []
        digest_function = drill.image_archive_sha256
        def digest(args):
            events.append('digest')
            return digest_function(args)
        self.patches['run'].side_effect = lambda *a, **k: events.append('load')
        self.patches['image_id'].side_effect = lambda args: events.append('inspect') or existing.IMAGE
        with patch.object(drill, 'image_archive_sha256', side_effect=digest):
            result = drill.restore_image(self.args)
        self.assertEqual(events, ['digest', 'load', 'inspect'])
        self.assertEqual(result, {'archive_sha256': self.args.baseline_report['image_archive_sha256'],
                                  'image_id': existing.IMAGE})
        self.patches['run'].assert_called_once_with(
            ['docker', 'image', 'load', '--input', self.args.image_archive],
            check=True, stdout=subprocess.DEVNULL, stderr=subprocess.PIPE)
        self.patches['image_id'].side_effect = None
        self.patches['image_id'].return_value = 'sha256:' + 'd' * 64
        with self.assertRaisesRegex(ValueError, 'Loaded image differs'):
            drill.restore_image(self.args)
        self.assert_no_mutations()

    def test_restore_docker_failure_propagates_without_inspection_or_retry(self):
        self.patches['run'].side_effect = subprocess.CalledProcessError(1, ['fake-docker'])
        with self.assertRaises(subprocess.CalledProcessError):
            drill.restore_image(self.args)
        self.patches['run'].assert_called_once()
        self.patches['image_id'].assert_not_called()
        self.assert_no_mutations()

    def test_restore_only_authenticates_report_then_archive_and_never_opens_database(self):
        baseline = self.verified_report()
        self.serve_report(baseline)
        self.patches['run'].side_effect = None
        argv = self.argv('exhaust', '--baseline-run-id', '123', '--restore-image-only')
        self.assertEqual(self.invoke(argv), 0)
        self.assertEqual(self.patches['github'].call_count, 3)
        report = json.loads(Path(self.args.output).read_text())
        self.assertEqual(report['stage'], 'image-verified')
        self.assertEqual(report['fixture_executions'], 0)
        self.assertEqual(report['image']['archive_sha256'], baseline['image_archive_sha256'])
        self.assertEqual(report['image']['image_id'], existing.IMAGE)
        self.patches['Client'].assert_not_called()
        self.patches['stage_runtime'].assert_not_called()
        self.patches['work_staged'].assert_not_called()
        self.assert_no_mutations()

    def test_main_invalid_baseline_blocks_database_and_docker_for_both_live_phases(self):
        for phase in ('exhaust', 'recover'):
            for restore_only in (False, True):
                with self.subTest(phase=phase, restore_only=restore_only):
                    report = self.verified_report()
                    report['actual_fixture_executions'] = 1
                    self.serve_report(report)
                    extras = ['--baseline-run-id', '123']
                    if phase == 'recover':
                        extras += ['--previous-run-id', '456']
                    if restore_only:
                        extras += ['--restore-image-only']
                    self.assertEqual(self.invoke(self.argv(phase, *extras)), 1)
                    self.assertFalse(json.loads(Path(self.args.output).read_text())['complete'])
        self.patches['Client'].assert_not_called()
        self.patches['run'].assert_not_called()
        self.assert_no_mutations()

    def test_recovery_restore_authenticates_previous_exhaustion_before_docker_or_database(self):
        self.serve_report(self.verified_report())
        with patch.object(drill, 'fetch_previous', side_effect=ValueError('bad previous receipt')) as previous:
            result = self.invoke(self.argv('recover', '--baseline-run-id', '123',
                                           '--previous-run-id', '456', '--restore-image-only'))
        self.assertEqual(result, 1)
        previous.assert_called_once_with(456, existing.SOURCE)
        self.assertEqual(self.patches['github'].call_count, 3)
        self.patches['Client'].assert_not_called()
        self.patches['run'].assert_not_called()
        self.assert_no_mutations()

    def test_verify_rejects_baseline_previous_or_restore_switch_before_authenticating_artifacts(self):
        for extras in (['--baseline-run-id', '123'], ['--previous-run-id', '123'],
                       ['--restore-image-only']):
            with self.subTest(arguments=extras):
                self.assertEqual(self.invoke(self.argv('verify', *extras)), 1)
        self.patches['github'].assert_not_called()
        self.patches['Client'].assert_not_called()
        self.patches['run'].assert_not_called()

    def test_both_live_phases_require_baseline_run_before_artifact_or_database_access(self):
        for phase in ('exhaust', 'recover'):
            with self.subTest(phase=phase):
                extras = ['--previous-run-id', '456'] if phase == 'recover' else []
                self.assertEqual(self.invoke(self.argv(phase, *extras)), 1)
        self.patches['github'].assert_not_called()
        self.patches['Client'].assert_not_called()
        self.patches['run'].assert_not_called()


class FrozenLiveBaselineTests(BaselineCase):
    def test_exhaust_rejects_second_distinct_variant_before_second_physical_execution(self):
        def extra_worker(client, args, *, before_execute):
            pid = client.one('runs', run_id=args.run)['provenance_id']
            for variant in ('non-strict', 'strict'):
                lease = dict(fixture_id=existing.FIXTURE, provenance_id=pid, variant=variant,
                             work_item_id=existing.identity('repo', pid, existing.FIXTURE, variant, 0),
                             lease_generation=1)
                before_execute(lease)
                self.physical.append(deepcopy(lease))
            return dict(attempts=2, stop_reason='budget-exhausted')
        self.patches['work_staged'].side_effect = extra_worker
        with self.assertRaisesRegex(ValueError, 'excess fixture execution'):
            self.run_drill()
        self.assertEqual([lease['variant'] for lease in self.physical], ['non-strict'])
        self.assertEqual([lease['variant'] for lease in self.report['execution_intents']], ['non-strict'])
        self.assertFalse(self.report['complete'])

    def test_exhaust_records_digest_of_full_stored_observation_not_report_projection(self):
        result = self.run_drill()
        full = self.client.tables['observations'][0]
        self.assertEqual(full['payload_sha256'], existing.bytea(sha({'outcome': 'pass', 'offline': True})))
        self.assertEqual(result['first_observation_sha256'], sha(full))
        projected = result['invocations'][0]['observations'][0]
        self.assertNotIn('payload_sha256', projected)
        self.assertNotEqual(result['first_observation_sha256'], sha(projected))

    def test_recovery_rechecks_full_prior_evidence_before_runtime_claim_execution_and_finalization(self):
        self.run_drill()
        previous = self.prepare_recovery()
        tables = deepcopy(self.client.tables)
        first_physical = deepcopy(self.physical)
        for field in ('payload_sha256', 'charged_attempts'):
            for timing in ('before-runtime', 'before-claim', 'before-execute', 'after-worker'):
                with self.subTest(field=field, timing=timing):
                    self.client = existing.Catalogue(deepcopy(self.base),
                                                     deepcopy(tables['fixtures'][0]))
                    self.client.tables = deepcopy(tables)
                    self.args.previous_report = deepcopy(previous)
                    self.args.output = str(self.folder / field / timing / 'report.json')
                    self.report = {'actions': {'id': 456, 'run_attempt': 1},
                                   'baseline_source': deepcopy(self.baseline_source)}
                    self.physical = deepcopy(first_physical)
                    def change_evidence(*unused):
                        if field == 'payload_sha256':
                            self.client.tables['observations'][0][field] = existing.bytea('e' * 64)
                        else:
                            # Python dict equality admits this change; canonical
                            # budget hashing must distinguish integer from float.
                            self.client.tables['budget_scopes'][0][field] = 1.0
                    self.patches['stage_runtime'].side_effect = (
                        change_evidence if timing == 'before-claim' else None)
                    self.patches['work_staged'].side_effect = self.fake_worker
                    if timing == 'before-runtime':
                        change_evidence()
                    elif timing == 'before-execute':
                        original_call = self.client.call
                        def claim_then_change(operation, *args):
                            value = original_call(operation, *args)
                            if operation == 'claim':
                                change_evidence()
                            return value
                        self.client.call = claim_then_change
                    elif timing == 'after-worker':
                        def work_then_change(client, args, *, before_execute):
                            value = self.fake_worker(client, args, before_execute=before_execute)
                            change_evidence()
                            return value
                        self.patches['work_staged'].side_effect = work_then_change
                    self.patches['stage_runtime'].reset_mock()
                    with self.assertRaisesRegex(ValueError, 'Original observation payload or previous charges'):
                        self.run_drill()
                    self.assertFalse(self.report['complete'])
                    self.assertEqual(len(self.physical), 2 if timing == 'after-worker' else 1)
                    if timing == 'before-runtime':
                        self.patches['stage_runtime'].assert_not_called()
                        self.assertEqual([e for e in self.client.events if e[0] in ('put', 'call')], [])
                    elif timing == 'before-claim':
                        self.assertEqual([e for e in self.client.events if e[:2] == ('call', 'claim')], [])
                    elif timing == 'before-execute':
                        self.assertEqual(len([e for e in self.client.events if e[:2] == ('call', 'claim')]), 1)

    def test_recovery_accepts_reordered_equivalent_full_rows_and_preserves_digest(self):
        first = deepcopy(self.run_drill())
        self.prepare_recovery()
        observation = self.client.tables['observations'][0]
        budget = self.client.tables['budget_scopes'][0]
        self.client.tables['observations'][0] = dict(reversed(list(observation.items())))
        self.client.tables['budget_scopes'][0] = dict(reversed(list(budget.items())))
        final = self.run_drill()
        self.assertTrue(final['complete'])
        self.assertEqual(final['actual_fixture_executions'], 2)
        self.assertEqual(final['first_observation_sha256'], first['first_observation_sha256'])
        self.assertEqual(final['first_observation_sha256'], sha(self.client.tables['observations'][0]))
        self.assertEqual(self.client.tables['budget_scopes'][0], budget)

    def add_alternative_fixture(self):
        original = deepcopy(self.original)
        original['path'] = 'test/built-ins/Array/other-offline-fixture.js'
        path = Path(self.args.root) / original['path']
        path.write_bytes(b'// alternate inert fixture, never executed\n')
        original['sha256'] = hashlib.sha256(path.read_bytes()).hexdigest()
        normalized = existing.normalize_fixture(original, self.args.root)
        fixture = dict(self.client.tables['fixtures'][0], fixture_id='alternative',
                       upstream_path=original['path'], content_sha256=existing.bytea(normalized['sha256']),
                       dependency_manifest_digest=existing.bytea(normalized['dependency_manifest_digest']))
        self.client.tables['fixtures'].insert(0, fixture)
        self.client.tables['fixture_variants'].extend(
            dict(fixture_id='alternative', variant=v, required=True) for v in ('non-strict', 'strict'))
        self.client.tables['provenance_fixture_eligibility'].insert(
            0, dict(provenance_id='base', fixture_id='alternative', eligibility='runnable'))
        originals = {self.original['path']: self.original, original['path']: original}
        self.patches['bridge'].side_effect = lambda request: [
            deepcopy(originals[path]) for path in request['paths']]
        return fixture

    def test_both_live_phases_reuse_fixed_fixture_even_when_other_valid_candidate_is_first(self):
        self.add_alternative_fixture()
        first = deepcopy(self.run_drill())
        self.prepare_recovery()
        final = self.run_drill()
        self.assertEqual(first['fixture']['fixture_id'], existing.FIXTURE)
        self.assertEqual(final['fixture'], first['fixture'])
        self.assertEqual([row['fixture_id'] for row in self.physical], [existing.FIXTURE] * 2)
        selections = [e[2] for e in self.client.events
                      if e[:2] == ('read', 'provenance_fixture_eligibility')
                      and e[2].get('provenance_id') == 'base']
        self.assertEqual(selections, [
            dict(provenance_id='base', eligibility='runnable', fixture_id=existing.FIXTURE)] * 2)
        self.assertEqual([call.args[0]['paths'] for call in self.patches['bridge'].call_args_list],
                         [[existing.PATH], [existing.PATH]])

    def test_missing_fixed_fixture_never_falls_back_to_other_eligible_fixture_in_either_phase(self):
        self.run_drill()
        previous = self.prepare_recovery()
        self.add_alternative_fixture()
        self.client.tables['provenance_fixture_eligibility'] = [
            row for row in self.client.tables['provenance_fixture_eligibility']
            if row['fixture_id'] != existing.FIXTURE]
        for phase in ('exhaust', 'recover'):
            with self.subTest(phase=phase):
                self.args.phase = phase
                self.args.previous_report = deepcopy(previous) if phase == 'recover' else None
                self.report = {'actions': {'id': 456, 'run_attempt': 1},
                               'baseline_source': deepcopy(self.baseline_source)}
                self.patches['bridge'].reset_mock()
                self.patches['stage_runtime'].reset_mock()
                count = len(self.client.events)
                with self.assertRaisesRegex(ValueError, 'No eligible'):
                    self.run_drill()
                self.patches['bridge'].assert_not_called()
                self.patches['stage_runtime'].assert_not_called()
                self.assertEqual([e for e in self.client.events[count:] if e[0] in ('put', 'call')], [])
                self.assertEqual(len(self.physical), 1)

    def test_verify_alone_can_select_first_eligible_fixture_without_a_frozen_baseline(self):
        fixture = self.add_alternative_fixture()
        self.args.phase = 'verify'
        self.args.baseline_report = None
        result = self.run_drill()
        self.assertEqual(result['fixture']['fixture_id'], fixture['fixture_id'])
        self.assertEqual(result['fixture']['upstream_path'], fixture['upstream_path'])
        self.assertEqual(result['actual_fixture_executions'], 0)
        self.assertEqual(result['stage'], 'baseline-verified')
        self.patches['stage_runtime'].assert_not_called()
        self.patches['work_staged'].assert_not_called()
        self.assert_no_mutations()

    def test_recovery_rejects_changed_first_observation_payload_with_unchanged_id(self):
        self.run_drill()
        previous = self.prepare_recovery()
        saved = deepcopy(self.client.tables)
        for timing in ('before-recovery', 'during-recovery'):
            with self.subTest(timing=timing):
                self.client.tables = deepcopy(saved)
                self.args.previous_report = deepcopy(previous)
                self.args.output = str(Path(self.args.output).parent / (timing + '/report.json'))
                self.physical = [dict(variant='non-strict')]
                self.report = {'actions': {'id': 456, 'run_attempt': 1},
                               'baseline_source': deepcopy(self.baseline_source)}
                def change_observation(count=None):
                    self.client.tables['observations'][0]['outcome'] = 'fail'
                self.after_execute = change_observation if timing == 'during-recovery' else None
                if timing == 'before-recovery':
                    change_observation()
                self.patches['stage_runtime'].reset_mock()
                count = len(self.client.events)
                with self.assertRaises(ValueError):
                    self.run_drill()
                self.assertFalse(self.report['complete'])
                if timing == 'before-recovery':
                    self.patches['stage_runtime'].assert_not_called()
                    self.assertEqual([e for e in self.client.events[count:] if e[0] in ('put', 'call')], [])
                    self.assertEqual(len(self.physical), 1)
                else:
                    self.assertEqual(len(self.physical), 2)

    def test_both_phases_reject_each_frozen_field_before_runtime_or_database_mutation(self):
        original = deepcopy(self.args.baseline_report)
        self.run_drill()
        previous = self.prepare_recovery()
        saved = deepcopy(self.client.tables)
        for phase in ('exhaust', 'recover'):
            for field in original:
                if field == 'phase':
                    continue
                with self.subTest(phase=phase, field=field):
                    self.client.tables = deepcopy(saved)
                    self.args.phase = phase
                    self.args.previous_report = deepcopy(previous) if phase == 'recover' else None
                    self.args.baseline_report = deepcopy(original)
                    self.args.baseline_report[field] = (
                        ['strict'] if field == 'required_variants' else
                        dict(original[field], fixture_id='foreign') if field == 'fixture' else 'changed')
                    self.report = {'actions': {'id': 456, 'run_attempt': 1},
                                   'baseline_source': deepcopy(self.baseline_source)}
                    self.patches['stage_runtime'].reset_mock()
                    self.patches['work_staged'].reset_mock()
                    start = len(self.client.events)
                    rejection = ('No eligible' if phase == 'exhaust' and field == 'fixture'
                                 else 'frozen baseline')
                    with self.assertRaisesRegex(ValueError, rejection):
                        self.run_drill()
                    self.assertEqual([e for e in self.client.events[start:] if e[0] in ('put', 'call')], [])
                    self.patches['stage_runtime'].assert_not_called()
                    self.patches['work_staged'].assert_not_called()
                    self.assertEqual(len(self.physical), 1)

    def test_live_requires_baseline_and_verify_phase_before_any_write(self):
        for baseline in (None, {}, dict(self.args.baseline_report, phase='exhaust')):
            with self.subTest(baseline=baseline):
                self.args.baseline_report = baseline
                with self.assertRaisesRegex(ValueError, 'frozen baseline'):
                    self.run_drill()
                self.assert_no_mutations()
        self.patches['stage_runtime'].assert_not_called()

    def test_recovery_rejects_different_authenticated_baseline_receipt_before_new_writes(self):
        self.run_drill()
        previous = self.prepare_recovery()
        for field, value in (('run_id', 999), ('artifact_id', 999),
                             ('zip_sha256', 'd' * 64), ('report_sha256', 'e' * 64)):
            with self.subTest(field=field):
                self.args.previous_report = deepcopy(previous)
                self.args.previous_report['baseline_source'][field] = value
                self.report = {'actions': {'id': 456, 'run_attempt': 1},
                               'baseline_source': deepcopy(self.baseline_source)}
                count = len(self.client.events)
                with self.assertRaisesRegex(ValueError, 'Previous source/full identity'):
                    self.run_drill()
                self.assertEqual([e for e in self.client.events[count:] if e[0] in ('put', 'call')], [])
                self.assertEqual(len(self.physical), 1)

    def test_verified_baseline_drives_two_phases_without_rebuilding_or_replacing_evidence(self):
        client = self.readonly_client()
        self.assertEqual(self.invoke(self.argv(), client), 0)
        baseline = json.loads(Path(self.args.output).read_text())
        self.serve_report(baseline)
        authenticated, receipt = drill.fetch_baseline(123, existing.SOURCE)
        self.args.baseline_report = authenticated
        self.baseline_source = receipt
        self.report = {'actions': {'id': 234, 'run_attempt': 1, 'head_sha': existing.SOURCE},
                       'baseline_source': receipt}
        first = deepcopy(self.run_drill())
        self.prepare_recovery()
        final = self.run_drill()
        self.assertEqual([row['variant'] for row in self.physical], ['non-strict', 'strict'])
        self.assertEqual(final['actual_fixture_executions'], 2)
        self.assertIs(final['complete'], True)
        self.assertEqual(final['invocations'][0], first['invocations'][0])
        self.assertEqual(final['invocations'][1]['observations'][0], first['invocations'][0]['observations'][0])
        self.assertEqual(final['baseline_source'], receipt)
        self.assertEqual(self.worker_images, [authenticated['fixture_image_id']] * 2)
        self.patches['run'].assert_not_called()


class StableProjectAndWorkflowTests(unittest.TestCase):
    def test_both_sdk_projects_override_verify_determinism_without_overriding_snapshot_paths(self):
        # Scoped project-structure regression guard, not a substitute for the
        # parent's evaluated properties and two controlled binary rebuilds.
        paths = ('tests/Jroc.Testing/Jroc.Testing.csproj',
                 'scripts/test262/NativeScreeningHost/NativeScreeningHost.csproj')
        for path in paths:
            with self.subTest(project=path):
                project = ET.fromstring((drill.REPO / path).read_text())
                self.assertEqual(project.attrib['Sdk'], 'Microsoft.NET.Sdk')
                properties = project.findall('./PropertyGroup/Deterministic')
                self.assertEqual([p.text for p in properties], ['true'])
                group = next(group for group in project.findall('PropertyGroup')
                             if group.find('Deterministic') is not None)
                self.assertNotIn('Condition', group.attrib)
                self.assertNotIn('Condition', properties[0].attrib)
                self.assertEqual(project.findall('.//DeterministicSourcePaths'), [])
                self.assertEqual(project.findall('Import'), [])
        testing = ET.parse(drill.REPO / paths[0]).getroot()
        self.assertEqual([p.attrib['Version'] for p in testing.findall('.//PackageReference')
                          if p.attrib['Include'] == 'Verify.Xunit'], ['30.4.0'])
        host = ET.parse(drill.REPO / paths[1]).getroot()
        self.assertIn('..\\..\\..\\tests\\Jroc.Testing\\Jroc.Testing.csproj',
                      [p.attrib['Include'] for p in host.findall('.//ProjectReference')])

    def test_workflow_builds_saves_once_and_restores_authenticated_archive_without_database_secret(self):
        text = (drill.REPO / drill.WORKFLOW).read_text()
        trigger = text.split('on:\n', 1)[1].split('permissions:\n', 1)[0]
        self.assertIn('  workflow_dispatch:\n', trigger)
        self.assertNotIn('workflow_run:', trigger)
        self.assertNotIn('schedule:', trigger)
        self.assertNotIn('push:', trigger)
        self.assertIn('options: [verify, exhaust, recover]', trigger)
        header, step_text = text.split('    steps:\n', 1)
        self.assertIn("github.ref == 'refs/heads/master'", header)
        self.assertIn("inputs.authorization == '" + drill.AUTHORIZATION + "'", header)
        self.assertIn('github.run_attempt == 1', header)
        # Existing repository convention: scope assertions to individual YAML
        # step blocks. No optional parser dependency or workflow shell execution.
        steps = step_text.split('      - ')[1:]
        build, = [s for s in steps if 'docker build ' in s]
        save, = [s for s in steps if 'docker image save ' in s]
        self.assertEqual(build, save)
        self.assertIn("if: inputs.phase == 'verify'\n", build)
        self.assertIn('gzip -n', save)
        download, = [s for s in steps if 'actions/download-artifact@' in s]
        restore, = [s for s in steps if '--restore-image-only' in s]
        guarded, = [s for s in steps if 'TEST262_DATABASE_URL:' in s]
        self.assertIn("if: inputs.phase != 'verify'\n", download)
        self.assertIn("if: inputs.phase != 'verify'\n", restore)
        self.assertLess(steps.index(download), steps.index(restore))
        self.assertLess(steps.index(restore), steps.index(guarded))
        self.assertIn('run-id: ${{ inputs.baseline_run_id }}\n', download)
        self.assertIn('name: test262-recovery-image-${{ inputs.baseline_run_id }}-1\n', download)
        self.assertIn('--baseline-run-id "$BASELINE_RUN_ID"', restore)
        for step in (restore, guarded):
            self.assertIn('--image-archive artifacts/test262/fixture-image.tar.gz', step)
            self.assertIn('BASELINE_RUN_ID: ${{ inputs.baseline_run_id }}\n', step)
        self.assertIn('TEST262_DATABASE_URL: ${{ secrets.TEST262_COORDINATOR_DATABASE_URL }}\n', guarded)
        for step in steps:
            if step != guarded:
                self.assertNotIn('secrets.', step)
                self.assertNotIn('TEST262_DATABASE_URL', step)
        self.assertEqual(text.count('docker build '), 1)
        self.assertNotIn('docker image load', text)  # Only the tested authenticator may load.
        self.assertNotIn('secrets.', header)
        self.assertNotIn('TEST262_DATABASE_URL', header)
        image, = [s for s in steps if 'path: artifacts/test262/fixture-image.tar.gz\n' in s]
        report, = [s for s in steps if 'path: artifacts/test262/recovery-drill\n' in s]
        self.assertIn("if: success() && inputs.phase == 'verify'\n", image)
        self.assertIn('retention-days: 7\n', image)
        self.assertIn('if-no-files-found: error\n', image)
        self.assertIn('if: always()\n', report)
        self.assertIn('retention-days: 90\n', report)
        dotnet, = [s for s in steps if 'actions/setup-dotnet@' in s]
        node, = [s for s in steps if 'actions/setup-node@' in s]
        self.assertIn('dotnet-version: 10.0.403\n', dotnet)
        self.assertIn("node-version: '24.22.1'\n", node)
