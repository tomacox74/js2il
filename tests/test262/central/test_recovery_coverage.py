from datetime import datetime, timedelta, timezone
import copy
import hashlib
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

from scripts.test262.central import recovery_coverage


NOW = datetime(2026, 10, 25, tzinfo=timezone.utc)
OBSERVED = datetime(2026, 10, 27, 3, 43, 45, 646475, tzinfo=timezone.utc)
HORIZON = datetime(2027, 4, 30, tzinfo=timezone.utc)
EXTENSION = recovery_coverage.LEDGER.parent/'retention-extension-40445723674.json'
EXTENSION_SHA256 = 'facd2a698d64137ca063ffdbf26903096370c2aeecc7f047728f8809853c3a59'
SOURCE_REVISION = '968756594b68290acadb81f01336cc7c02454b99'


class RecoveryCoverageTests(unittest.TestCase):
    def healthy(self):
        return {'fetch_state':'observed','latest_run_id':1,'latest_state':'completed',
                'latest_conclusion':'success','receipt_verified':True,'retention_verified':True,
                'receipt_checked_at':'2026-10-24T12:00:00+00:00',
                'retention_until':'2026-11-27T00:00:00+00:00'}

    def codes(self, check):
        return {r['code'] for r in recovery_coverage.assess({'backup':check}, NOW)}

    def test_latest_failure_is_visible_despite_older_valid_receipt(self):
        check=self.healthy()
        self.assertEqual(self.codes(check),set())
        check['latest_conclusion']='failure'
        self.assertIn('latest_attempt_unsuccessful',self.codes(check))
        check['latest_state']='in_progress'
        self.assertIn('latest_attempt_incomplete',self.codes(check))

    def test_stale_receipt_and_expired_lock_are_not_healthy(self):
        check=self.healthy()
        check.update(receipt_checked_at='2026-10-20T00:00:00+00:00',
                     retention_until='2026-10-24T00:00:00+00:00')
        self.assertEqual(self.codes(check),{'backup_older_than_monitor_threshold','recorded_object_lock_expired'})
        check['receipt_checked_at']='2026-10-26T00:00:00+00:00'
        self.assertIn('backup_older_than_monitor_threshold',self.codes(check))

    def test_missing_or_unverifiable_evidence_stays_unknown(self):
        findings=recovery_coverage.assess({'restore':{'fetch_state':'unavailable'}},NOW)
        self.assertTrue(findings)
        self.assertTrue(all(r['state']=='unknown' for r in findings))
        check=self.healthy();check['receipt_verified']=None
        self.assertIn('verified_receipt_missing_expired_or_invalid',self.codes(check))

    def test_monitor_and_drill_review_boundaries_are_exact_not_rpo_guarantees(self):
        check=self.healthy()
        check['receipt_checked_at']='2026-10-23T12:00:00+00:00'
        self.assertNotIn('backup_older_than_monitor_threshold',self.codes(check))
        check['receipt_checked_at']='2026-10-23T11:59:59+00:00'
        self.assertIn('backup_older_than_monitor_threshold',self.codes(check))
        check['receipt_checked_at']='2026-10-17T00:00:00+00:00'
        findings=recovery_coverage.assess({'restore':check},NOW)
        self.assertNotIn('weekly_drill_overdue',{r['code'] for r in findings})
        check['receipt_checked_at']='2026-10-16T23:59:59+00:00'
        findings=recovery_coverage.assess({'restore':check},NOW)
        self.assertIn('weekly_drill_overdue',{r['code'] for r in findings})

    def test_ledger_digest_authenticates_exact_bytes(self):
        self.assertTrue(recovery_coverage.LEDGER.is_absolute())
        self.assertEqual(recovery_coverage.LEDGER_SHA256,
                         '5b1f6081dc7b5e61229a1be44bca4afc43889b735d6f5201e32c3022c8132be4')
        ledger = recovery_coverage.load_ledger()
        self.assertIsInstance(ledger, dict)
        self.assertEqual(len(ledger['archives']), 41)
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory)/'ledger.json'
            raw = recovery_coverage.LEDGER.read_bytes()
            path.write_bytes(raw)
            self.assertEqual(recovery_coverage.load_ledger(path), ledger)
            path.write_bytes(raw+b'\n')
            with self.assertRaises(ValueError):
                recovery_coverage.load_ledger(path)

    def test_inventory_reads_only_bounded_metadata_and_never_claims_archive_preservation(self):
        ledger = recovery_coverage.load_ledger()
        calls = []
        def check(repository, kind, fetch):
            calls.append((repository, kind, fetch))
            return self.healthy()
        def fetch(*args, **kwargs):
            self.fail('Original Actions archive metadata must not be re-inventoried')
        with patch.object(recovery_coverage, 'collect_check', side_effect=check):
            report = recovery_coverage.collect(fetch, NOW)
        self.assertEqual([row[1] for row in calls], ['backup', 'restore'])
        self.assertTrue(all(row[2] is fetch for row in calls))
        self.assertEqual(report['schema_version'], 3)
        self.assertIs(report['accepted_historical_container_coverage'], True)
        self.assertEqual(report['accepted_historical_gap_count'], 11)
        self.assertEqual(report['summary']['required_container_count'], 41)
        self.assertEqual(report['summary']['earliest_recorded_lock_deadline'],
                         ledger['summary']['earliest_compliance_deadline'])
        self.assertEqual(len(report['archives']), 41)
        by_id = {row['artifact_id']: row for row in report['archives']}
        self.assertEqual(set(by_id), {row['artifact_id'] for row in ledger['archives']})
        for source in ledger['archives']:
            row = by_id[source['artifact_id']]
            self.assertEqual(row['sha256'], source['object']['sha256'])
            self.assertEqual(row['independent_archive_state'], 'historical_exact_version_proof')
            self.assertEqual(row['actions_state'], 'not_assessed')
            self.assertEqual(row['recorded_verified_at'], source['verification']['checked_at'])
            self.assertEqual(row['recorded_retain_until'], source['object']['retain_until'])
            self.assertEqual(row['recorded_lock_state'], 'deadline_future')
        for flag in ('archive_preservation_verified', 'object_store_access',
                     'history_complete', 'notifications_enabled', 'database_access'):
            self.assertIs(report[flag], False, flag)
        self.assertTrue(all(row['owner_issue'] == 2343 for row in report['findings']))
        self.assertNotIn('original_actions_archive_', json.dumps(report['findings']))

    def test_archive_findings_pin_lock_expiry_and_protection_boundaries(self):
        ledger = recovery_coverage.load_ledger()
        source = ledger['archives'][0]
        deadline = datetime.fromisoformat(source['object']['retain_until'])
        for partition, expiry, unproven in (
                ('unknown', None, True),
                ('before', (deadline-timedelta(seconds=1)).isoformat(), False),
                ('equal', deadline.isoformat(), True),
                ('after', (deadline+timedelta(seconds=1)).isoformat(), True)):
            for offset, expired in ((-1, False), (0, True), (1, True)):
                with self.subTest(partition=partition, offset=offset):
                    source['actions_expires_at_from_frozen_inventory'] = expiry
                    with patch.object(recovery_coverage, 'load_ledger', return_value=ledger), \
                         patch.object(recovery_coverage, 'load_retention_extension',
                                      side_effect=OSError('private unavailable evidence')), \
                         patch.object(recovery_coverage, 'collect_check', return_value=self.healthy()):
                        report = recovery_coverage.collect(now=deadline+timedelta(seconds=offset))
                    self.assert_extension_unknown(report, 'OSError')
                    row = next(row for row in report['archives']
                               if row['artifact_id'] == source['artifact_id'])
                    self.assertEqual(row['recorded_lock_state'],
                                     'deadline_elapsed' if expired else 'deadline_future')
                    findings = [row for row in report['findings']
                                if row.get('artifact_id') == source['artifact_id']]
                    codes = {row['code'] for row in findings}
                    self.assertEqual(
                        'historical_archive_protection_beyond_actions_expiry_unproven' in codes,
                        unproven)
                    self.assertEqual('recorded_archive_lock_expired' in codes, expired)
                    self.assertTrue(all(row['owner_issue'] == 2343 and
                                        row['state'] == 'attention' for row in findings))

    def report_at(self, now):
        with patch.object(recovery_coverage, 'collect_check', return_value=self.healthy()):
            return recovery_coverage.collect(now=now)

    def assert_extension_unknown(self, report, error_class=None):
        findings = [row for row in report['findings']
                    if row.get('kind') == 'retention-extension']
        self.assertEqual(findings, [{
            'kind': 'retention-extension',
            'code': 'recorded_retention_extension_unavailable',
            'state': 'unknown', 'owner_issue': 2343,
        }])
        if error_class is not None:
            self.assertEqual(report['retention_extension'],
                             {'state': 'unavailable', 'error_class': error_class})
        self.assertEqual(report['summary']['extension_archive_count'], 0)
        self.assertEqual(report['summary']['extension_receipt_count'], 0)
        for row in report['archives']:
            self.assertEqual(row['latest_verified_at'], row['recorded_verified_at'])
            self.assertEqual(row['latest_verified_retain_until'], row['recorded_retain_until'])
            self.assertEqual(row['latest_recorded_lock_state'], row['recorded_lock_state'])
        self.assertNotIn('private unavailable evidence', json.dumps(report))

    def test_extension_digest_authenticates_canonical_bytes_before_json_or_missing_path(self):
        ledger = recovery_coverage.load_ledger()
        self.assertEqual(recovery_coverage.EXTENSION, EXTENSION)
        self.assertTrue(recovery_coverage.EXTENSION.is_absolute())
        self.assertEqual(recovery_coverage.EXTENSION_SHA256, EXTENSION_SHA256)
        raw = EXTENSION.read_bytes()
        self.assertEqual(hashlib.sha256(raw).hexdigest(), EXTENSION_SHA256)
        expected = recovery_coverage.load_retention_extension(ledger, OBSERVED)
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory)/'extension.json'
            path.write_bytes(raw)
            self.assertEqual(recovery_coverage.load_retention_extension(ledger, OBSERVED, path),
                             expected)
            for changed in (raw+b'\n', b'not JSON'):
                with self.subTest(change=changed[-8:]):
                    path.write_bytes(changed)
                    with patch.object(recovery_coverage.json, 'loads') as parse:
                        with self.assertRaises(ValueError):
                            recovery_coverage.load_retention_extension(ledger, OBSERVED, path)
                        parse.assert_not_called()
            with self.assertRaises(OSError):
                recovery_coverage.load_retention_extension(
                    ledger, OBSERVED, Path(directory)/'absent.json')

    def test_extension_observation_before_at_and_after_never_applies_future_proof(self):
        ledger = recovery_coverage.load_ledger()
        for offset in (-1, 0, 1):
            now = OBSERVED+timedelta(microseconds=offset)
            with self.subTest(offset=offset):
                projection = recovery_coverage.load_retention_extension(ledger, now)
                report = self.report_at(now)
                if offset < 0:
                    self.assertEqual(projection['state'], 'not_yet_observed')
                    self.assertFalse(projection.get('archives'))
                    self.assert_extension_unknown(report)
                    self.assertEqual(report['retention_extension']['state'], 'not_yet_observed')
                    continue
                self.assertEqual(projection['state'], 'recorded_verified')
                self.assertEqual(projection['checked_at'], OBSERVED.isoformat())
                self.assertEqual(projection['run_id'], 40445723674)
                self.assertEqual(projection['source_revision'], SOURCE_REVISION)
                self.assertEqual(projection['evidence_sha256'], EXTENSION_SHA256)
                self.assertEqual(projection['archive_count'], 41)
                self.assertEqual(projection['receipt_count'], 1)
                self.assertIs(projection['fresh_object_store_readback'], False)
                self.assertEqual(set(projection['archives']),
                                 {row['artifact_id'] for row in ledger['archives']})
                for source in ledger['archives']:
                    final = projection['archives'][source['artifact_id']]
                    self.assertEqual(final['version_id'], source['object']['version_id'])
                    self.assertEqual(final['expected_sha256'], source['object']['sha256'])
                    self.assertEqual(final['content_length'], source['object']['content_length'])
                    self.assertEqual(final['retain_until'], HORIZON.isoformat())
                receipt = projection['receipt']
                self.assertEqual(receipt['version_id'], '1OfBQkTAEhYjkGHsz1j17zEMkYvtlTzu')
                self.assertEqual(receipt['expected_sha256'],
                                 '61d0fff3c0f96feadcdb3a4fa948b1bfd7fbd0032f38e8e4febb9179df3bfd42')
                self.assertEqual(receipt['content_length'], 32367)
                self.assertEqual(receipt['retain_until'], HORIZON.isoformat())
                self.assertNotIn('recorded_retention_extension_unavailable',
                                 {row['code'] for row in report['findings']})

    def test_november_and_december_latest_proof_preserves_immutable_archive_history(self):
        ledger = recovery_coverage.load_ledger()
        expected_historical = []
        for source in ledger['archives']:
            expected_historical.extend([
                {'artifact_id': source['artifact_id'], 'code': 'recorded_archive_lock_expired',
                 'state': 'attention', 'owner_issue': 2343},
                {'artifact_id': source['artifact_id'],
                 'code': 'historical_archive_protection_beyond_actions_expiry_unproven',
                 'state': 'attention', 'owner_issue': 2343},
            ])
        for now in (datetime(2026, 11, 30, 23, tzinfo=timezone.utc),
                    datetime(2026, 12, 1, tzinfo=timezone.utc)):
            with self.subTest(now=now.isoformat()):
                report = self.report_at(now)
                self.assertEqual(report['historical_findings'], expected_historical)
                self.assertEqual(report['ledger_sha256'], recovery_coverage.LEDGER_SHA256)
                self.assertEqual(report['schema_version'], 3)
                self.assertEqual(report['accepted_historical_gap_count'], 11)
                self.assertIs(report['accepted_historical_container_coverage'], True)
                self.assertEqual(report['summary']['earliest_recorded_lock_deadline'],
                                 ledger['summary']['earliest_compliance_deadline'])
                self.assertEqual(report['summary']['earliest_latest_verified_archive_lock_deadline'],
                                 HORIZON.isoformat())
                self.assertEqual(report['summary']['extension_archive_count'], 41)
                self.assertEqual(report['summary']['extension_receipt_count'], 1)
                self.assertEqual(report['summary']['known_actions_expiry_covered_archive_count'], 40)
                self.assertEqual(report['summary']['unknown_actions_expiry_archive_count'], 1)
                by_id = {row['artifact_id']: row for row in report['archives']}
                unknown = {source['artifact_id'] for source in ledger['archives']
                           if not source['actions_expires_at_from_frozen_inventory']}
                current = [row for row in report['findings'] if 'artifact_id' in row]
                self.assertEqual(current, [{
                    'artifact_id': next(iter(unknown)),
                    'code': 'historical_archive_protection_beyond_actions_expiry_unproven',
                    'state': 'attention', 'owner_issue': 2343,
                }])
                for source in ledger['archives']:
                    row = by_id[source['artifact_id']]
                    self.assertEqual(row['sha256'], source['object']['sha256'])
                    self.assertEqual(row['recorded_verified_at'], source['verification']['checked_at'])
                    self.assertEqual(row['recorded_retain_until'], source['object']['retain_until'])
                    self.assertEqual(row['recorded_lock_state'], 'deadline_elapsed')
                    self.assertEqual(row['latest_verified_at'], OBSERVED.isoformat())
                    self.assertEqual(row['latest_verified_retain_until'], HORIZON.isoformat())
                    self.assertEqual(row['latest_recorded_lock_state'], 'deadline_future')
                    self.assertIs(row['protection_beyond_frozen_actions_expiry'],
                                  None if source['artifact_id'] in unknown else True)
                public = report['retention_extension']
                self.assertNotIn('archives', public)
                self.assertEqual(public['receipt']['kind'], 'preservation_receipt')
                self.assertEqual(public['receipt']['version_id'], '1OfBQkTAEhYjkGHsz1j17zEMkYvtlTzu')
                for forbidden in ('bucket', 'key', 'kms_identity', 'role', 'error_message'):
                    self.assertNotIn(forbidden, public)
                for flag in ('database_access', 'object_store_access', 'notifications_enabled',
                             'history_complete', 'archive_preservation_verified'):
                    self.assertIs(report[flag], False, flag)

    def test_april_deadline_before_equal_after_marks_archives_and_receipt_expired_not_deleted(self):
        ledger = recovery_coverage.load_ledger()
        for offset in (-1, 0, 1):
            with self.subTest(offset=offset):
                report = self.report_at(HORIZON+timedelta(seconds=offset))
                expired = [row for row in report['findings']
                           if row['code'] == 'recorded_archive_lock_expired']
                self.assertEqual({row['artifact_id'] for row in expired},
                                 {row['artifact_id'] for row in ledger['archives']} if offset >= 0
                                 else set())
                receipt = [row for row in report['findings']
                           if row.get('kind') == 'preservation_receipt']
                self.assertEqual(receipt, [{
                    'kind': 'preservation_receipt',
                    'code': 'recorded_preservation_receipt_lock_expired',
                    'state': 'attention', 'owner_issue': 2343,
                }] if offset >= 0 else [])
                self.assertEqual({row['latest_recorded_lock_state'] for row in report['archives']},
                                 {'deadline_elapsed' if offset >= 0 else 'deadline_future'})
                self.assertEqual(report['retention_extension']['state'], 'recorded_verified')
                self.assertNotIn('deleted', json.dumps(report['findings']))
                self.assertIs(report['object_store_access'], False)

    def test_latest_frozen_actions_expiry_before_equal_after_and_unknown_is_strict(self):
        ledger = recovery_coverage.load_ledger()
        source = ledger['archives'][0]
        for partition, expiry, covered in (
                ('before', (HORIZON-timedelta(seconds=1)).isoformat(), True),
                ('equal', HORIZON.isoformat(), False),
                ('after', (HORIZON+timedelta(seconds=1)).isoformat(), False),
                ('unknown', None, None)):
            with self.subTest(partition=partition):
                source['actions_expires_at_from_frozen_inventory'] = expiry
                with patch.object(recovery_coverage, 'load_ledger', return_value=ledger):
                    report = self.report_at(HORIZON-timedelta(seconds=1))
                row = next(row for row in report['archives']
                           if row['artifact_id'] == source['artifact_id'])
                self.assertIs(row['protection_beyond_frozen_actions_expiry'], covered)
                self.assertEqual(
                    [row['code'] for row in report['findings']
                     if row.get('artifact_id') == source['artifact_id']],
                    [] if covered else [
                        'historical_archive_protection_beyond_actions_expiry_unproven'])

    def test_unavailable_extension_is_explicit_unknown_without_private_diagnostics(self):
        for error in (OSError('private unavailable evidence'), ValueError('private unavailable evidence')):
            with self.subTest(error=type(error).__name__):
                with patch.object(recovery_coverage, 'load_retention_extension', side_effect=error):
                    report = self.report_at(datetime(2026, 12, 1, tzinfo=timezone.utc))
                self.assert_extension_unknown(report, type(error).__name__)
                self.assertEqual(sum(row['code'] == 'recorded_archive_lock_expired'
                                     for row in report['findings']), 41)
                self.assertEqual([row for row in report['findings'] if 'artifact_id' in row],
                                 report['historical_findings'])

    def test_authenticated_invalid_extension_fails_closed_for_provenance_scope_and_metadata(self):
        """Re-pin mutated bytes to exercise validation beyond the outer digest gate."""
        baseline = json.loads(EXTENSION.read_bytes())
        ledger = recovery_coverage.load_ledger()
        cases = []
        def case(name, path, value):
            cases.append((name, path, value))
        for prefix in ((), ('report',)):
            for field, value in (('schema_version', 2), ('repository', 'other/repository'),
                                 ('operation', 'audit-retention'), ('run_id', 40445723675),
                                 ('run_attempt', 2), ('source_revision', '0'*40)):
                case('.'.join(prefix+(field,)), prefix+(field,), value)
        case('run_conclusion', ('run_conclusion',), 'failure')
        case('ledger_sha256', ('report', 'ledger_sha256'), '0'*64)
        for field in ('complete', 'preflight_complete', 'protection_verified'):
            for value in (False, 1, None):
                case(field+repr(value), ('report', field), value)
        case('report_wrong_shape', ('report',), [])
        case('report_missing_fields', ('report',), {})
        case('wrong_preservation_digest', ('report', 'preservation_sha256'), '0'*64)
        case('wrong_fixed_target', ('report', 'target_retain_until'),
             (HORIZON-timedelta(seconds=1)).isoformat())
        case('wrong_target_count', ('report', 'summary', 'target_count'), 41)
        case('wrong_verified_count', ('report', 'summary', 'target_verified_count'), 41)
        for field in ('uploads', 'deletions', 'imports', 'restores', 'production_changed',
                      'permissions_changed', 'bucket_configuration_changes', 'database_access',
                      'fixture_execution', 'payload_reads', 'payload_digest_verified',
                      'notifications_enabled', 'retention_policy_adopted'):
            case(field, ('report', field), True)
        for value in ('2026-10-27T03:43:45.646475', 'not-a-timestamp', None):
            case('checked_at'+repr(value), ('report', 'checked_at'), value)
        for name, objects in (
                ('missing_archive', baseline['report']['objects'][1:]),
                ('missing_receipt', baseline['report']['objects'][:-1]),
                ('duplicate_archive', baseline['report']['objects'][:-2]+[
                    baseline['report']['objects'][0], baseline['report']['objects'][-1]]),
                ('extra_receipt', baseline['report']['objects']+[baseline['report']['objects'][-1]]),
                ('objects_wrong_shape', {})):
            case(name, ('report', 'objects'), objects)
        for index in (0, -1):
            for path_prefix in (('report', 'objects', index),
                                ('report', 'objects', index, 'final_verification')):
                for field, value in (('version_id', 'wrong-version'),
                                     ('expected_sha256', '0'*64),
                                     ('kind', 'unknown-target')):
                    case(str(path_prefix)+field, path_prefix+(field,), value)
            final = ('report', 'objects', index, 'final_verification')
            for field, value in (
                    ('content_length', 1), ('content_length', True), ('metadata_verified', False),
                    ('metadata_verified', 1), ('payload_digest_verified', True),
                    ('lock_mode', 'GOVERNANCE'), ('encryption', 'AES256'),
                    ('state', 'unavailable'), ('version_matches', False),
                    ('length_matches', False), ('kms_identity_matches', False),
                    ('prior_lock_not_shortened', False), ('compliance_lock_current', False),
                    ('retain_until', (HORIZON-timedelta(microseconds=1)).isoformat()),
                    ('retain_until', '2027-04-30T00:00:00'),
                    ('retain_until', 'invalid')):
                case(str(index)+field+repr(value), final+(field,), value)
        case('outer_archive_identity', ('report', 'objects', 0, 'artifact_id'), -1)
        case('final_archive_identity',
             ('report', 'objects', 0, 'final_verification', 'artifact_id'), -1)
        case('missing_final_observation', ('report', 'objects', 0, 'final_verification'), None)
        case('incomplete_final_observation', ('report', 'objects', 0, 'final_verification'), {})
        original_read = Path.read_bytes
        for name, path, value in cases:
            with self.subTest(partition=name):
                envelope = copy.deepcopy(baseline)
                target = envelope
                for key in path[:-1]:
                    target = target[key]
                target[path[-1]] = value
                raw = json.dumps(envelope).encode()
                def read_bytes(path):
                    return raw if path == EXTENSION else original_read(path)
                with patch.object(Path, 'read_bytes', read_bytes), \
                     patch.object(recovery_coverage, 'EXTENSION_SHA256',
                                  hashlib.sha256(raw).hexdigest()):
                    with self.assertRaises(ValueError):
                        recovery_coverage.load_retention_extension(ledger, HORIZON)
                    report = self.report_at(HORIZON)
                self.assert_extension_unknown(report, 'ValueError')

    def test_longer_final_horizon_is_accepted_and_loader_rejects_naive_clock(self):
        ledger = recovery_coverage.load_ledger()
        with self.assertRaises(ValueError):
            recovery_coverage.load_retention_extension(ledger, OBSERVED.replace(tzinfo=None))
        envelope = json.loads(EXTENSION.read_bytes())
        longer = datetime(2030, 1, 1, tzinfo=timezone.utc).isoformat()
        for row in envelope['report']['objects']:
            row['final_verification']['retain_until'] = longer
        envelope['report']['summary']['earliest_verified_retain_until'] = longer
        raw = json.dumps(envelope).encode()
        original_read = Path.read_bytes
        def read_bytes(path):
            return raw if path == EXTENSION else original_read(path)
        with patch.object(Path, 'read_bytes', read_bytes), \
             patch.object(recovery_coverage, 'EXTENSION_SHA256', hashlib.sha256(raw).hexdigest()):
            projection = recovery_coverage.load_retention_extension(ledger, HORIZON)
            report = self.report_at(HORIZON)
        self.assertEqual(projection['state'], 'recorded_verified')
        self.assertEqual({row['retain_until'] for row in projection['archives'].values()}, {longer})
        self.assertEqual(projection['receipt']['retain_until'], longer)
        self.assertEqual(report['summary']['earliest_latest_verified_archive_lock_deadline'], longer)
        self.assertFalse(any(row['code'] in ('recorded_archive_lock_expired',
                                             'recorded_preservation_receipt_lock_expired')
                             for row in report['findings']))

    def test_latest_workflow_failure_remains_independent_of_good_extension_and_older_receipt(self):
        check = self.healthy()
        check['latest_conclusion'] = 'failure'
        with patch.object(recovery_coverage, 'collect_check', return_value=check):
            report = recovery_coverage.collect(now=OBSERVED)
        self.assertEqual(report['retention_extension']['state'], 'recorded_verified')
        self.assertEqual(
            {(row['kind'], row['code']) for row in report['findings']
             if row.get('kind') in ('backup', 'restore')},
            {('backup', 'latest_attempt_unsuccessful'), ('restore', 'latest_attempt_unsuccessful'),
             ('backup', 'backup_older_than_monitor_threshold')})

    def test_naive_clock_is_rejected(self):
        with self.assertRaises(ValueError):
            recovery_coverage.collect(now=datetime(2026,10,25))


if __name__=='__main__':
    unittest.main()
