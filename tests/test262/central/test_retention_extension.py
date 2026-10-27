"""Offline contracts for the finite, explicitly authorized #2343 operation.

The fake has independent stored retention deadlines and no payload or general
write API. No AWS credentials, network, workflow, database, or fixture execution.
"""
from contextlib import redirect_stderr, redirect_stdout
import copy
from datetime import datetime, timedelta, timezone
import hashlib
import io
import json
import os
from pathlib import Path
import re
import tempfile
import unittest
from unittest.mock import patch

from botocore.exceptions import BotoCoreError, ClientError
from scripts.test262.central import recovery_coverage as coverage
from scripts.test262.central import retention_audit as audit
from scripts.test262.central import retention_extension as extension


ROOT = Path(__file__).resolve().parents[3]
LEDGER = ROOT/'docs/ECMA262/evidence/historical-retention-ledger-2242.json'
PRESERVATION = ROOT/'docs/ECMA262/evidence/retained-recovery-preservation-40393597860.json'
LEDGER_SHA = '5b1f6081dc7b5e61229a1be44bca4afc43889b735d6f5201e32c3022c8132be4'
PRESERVATION_SHA = '4e5b1c4590ec3325c6bebd39d9a5001d2ebf9a72a858aef8e6d1e94762901530'
NOW = datetime(2026, 10, 27, tzinfo=timezone.utc)
TARGET = datetime(2027, 4, 30, tzinfo=timezone.utc)
TOKEN = 'extend-42-retention-20270430'
PRIVATE = 'PRIVATE-do-not-publish'
BUCKET = PRIVATE+'-bucket'
BASE = PRIVATE+'/destination'
KMS = 'arn:aws:kms:us-east-1:123456789012:key/'+PRIVATE
URI = f's3://{BUCKET}/{BASE}'
CONFIG_CALLS = (
    'get_bucket_versioning', 'get_object_lock_configuration',
    'get_bucket_lifecycle_configuration', 'get_bucket_encryption',
)
SAFE_FLAGS = (
    'uploads', 'deletions', 'imports', 'restores', 'production_changed',
    'permissions_changed', 'bucket_configuration_changes', 'payload_reads',
    'payload_digest_verified', 'notifications_enabled', 'retention_policy_adopted',
)


def aws_error(code='AccessDenied'):
    return ClientError({'Error': {'Code': code, 'Message': PRIVATE+' credentials=secret'},
                        'ResponseMetadata': {'RequestId': PRIVATE}}, PRIVATE)


def point(value):
    return value if isinstance(value, datetime) else datetime.fromisoformat(value.replace('Z', '+00:00'))


class RetentionS3:
    """Only exact-version metadata and approved COMPLIANCE retention writes."""
    def __init__(self):
        ledger, preservation = json.loads(LEDGER.read_bytes()), json.loads(PRESERVATION.read_bytes())
        self.pins = {}
        self.identities = {}
        for row in ledger['archives']:
            origin = 40393597860 if row['verification']['run_id'] == 40393597860 else 37715977350
            key = f"{BASE}/cutover-evidence/{origin}-1/{row['artifact_id']}.zip"
            self.pins[key] = copy.deepcopy(row['object'])
            self.identities[key] = {'kind': 'archive', 'artifact_id': row['artifact_id']}
        key = f'{BASE}/cutover-evidence/40393597860-1/recovery-preservation-report.json'
        self.pins[key] = copy.deepcopy(preservation['report']['receipt'])
        self.identities[key] = {'kind': 'preservation_receipt'}
        self.deadlines = {key: point(pin['retain_until']) for key, pin in self.pins.items()}
        self.calls = []
        self.writes = []
        self.read_counts = {}
        self.failures = {}
        self.overrides = {}
        self.write_failure = None
        self.write_response = {'ResponseMetadata': {'HTTPStatusCode': 200}}
        self.apply_failed_write = False
        self.before_put = None
        self.config = {
            'get_bucket_versioning': {'Status': 'Enabled'},
            'get_object_lock_configuration': {'ObjectLockConfiguration': {
                'ObjectLockEnabled': 'Enabled',
                'Rule': {'DefaultRetention': {'Mode': 'COMPLIANCE', 'Days': 35}}}},
            'get_bucket_lifecycle_configuration': {'Rules': [
                {'Status': 'Disabled', 'Filter': {'Prefix': BASE+'-outside/'}}]},
            'get_bucket_encryption': {'ServerSideEncryptionConfiguration': {'Rules': [
                {'ApplyServerSideEncryptionByDefault': {
                    'SSEAlgorithm': 'aws:kms', 'KMSMasterKeyID': KMS}}]}},
        }

    def configuration(self, method, kwargs):
        if kwargs != {'Bucket': BUCKET}:
            raise AssertionError('Configuration reads must use only the configured bucket')
        self.calls.append((method, copy.deepcopy(kwargs)))
        if method in self.failures:
            raise self.failures[method]
        return copy.deepcopy(self.config[method])

    def get_bucket_versioning(self, **kwargs):
        return self.configuration('get_bucket_versioning', kwargs)

    def get_object_lock_configuration(self, **kwargs):
        return self.configuration('get_object_lock_configuration', kwargs)

    def get_bucket_lifecycle_configuration(self, **kwargs):
        return self.configuration('get_bucket_lifecycle_configuration', kwargs)

    def get_bucket_encryption(self, **kwargs):
        return self.configuration('get_bucket_encryption', kwargs)

    def read(self, method, kwargs):
        key = kwargs['Key']
        pin = self.pins[key]
        if kwargs != {'Bucket': BUCKET, 'Key': key, 'VersionId': pin['version_id']}:
            raise AssertionError('Every metadata read must pin the accepted immutable version')
        self.calls.append((method, copy.deepcopy(kwargs)))
        identity = (method, key)
        count = self.read_counts[identity] = self.read_counts.get(identity, 0)+1
        if (method, key, count) in self.failures:
            raise self.failures[(method, key, count)]
        if method == 'head_object':
            result = {'VersionId': pin['version_id'], 'ContentLength': pin['content_length'],
                      'ServerSideEncryption': 'aws:kms', 'SSEKMSKeyId': KMS}
        else:
            result = {'Mode': 'COMPLIANCE', 'RetainUntilDate': self.deadlines[key]}
        result.update(self.overrides.get((method, key, count), {}))
        return result

    def head_object(self, **kwargs):
        return self.read('head_object', kwargs)

    def get_object_retention(self, **kwargs):
        return {'Retention': self.read('get_object_retention', kwargs)}

    def put_object_retention(self, **kwargs):
        key = kwargs['Key']
        expected = {'Bucket': BUCKET, 'Key': key, 'VersionId': self.pins[key]['version_id'],
                    'Retention': {'Mode': 'COMPLIANCE', 'RetainUntilDate': TARGET}}
        if kwargs != expected:
            raise AssertionError('Only the approved exact-version retention request is allowed')
        for method in CONFIG_CALLS:
            if not any(name == method for name, _ in self.calls):
                raise AssertionError('All four configuration reads must precede every write')
        for target in self.pins:
            for method in ('head_object', 'get_object_retention'):
                if self.read_counts.get((method, target), 0) < 1:
                    raise AssertionError('All 42 exact versions must pass preflight before writing')
        if TARGET <= self.deadlines[key]:
            raise AssertionError('Never reset or shorten an equal/longer existing lock')
        if self.before_put:
            self.before_put(kwargs)
        self.calls.append(('put_object_retention', copy.deepcopy(kwargs)))
        self.writes.append(copy.deepcopy(kwargs))
        error = self.write_failure(len(self.writes), key) if self.write_failure else None
        if error is None or self.apply_failed_write:
            self.deadlines[key] = TARGET
        if error:
            raise error
        return copy.deepcopy(self.write_response)


class RetentionExtensionTests(unittest.TestCase):
    def setUp(self):
        self.s3 = RetentionS3()
        self.report = extension.initial_report(NOW)

    def extend(self, now=NOW, **kwargs):
        extension.extend(self.s3, URI, KMS, self.report, TOKEN, now=now, **kwargs)
        return self.report

    def assert_public(self, report):
        encoded = json.dumps(report)
        for secret in (PRIVATE, BUCKET, BASE, KMS, 'credentials=', 'RequestId', 'Traceback', 's3://'):
            self.assertNotIn(secret, encoded)
        for flag in SAFE_FLAGS:
            self.assertIs(report[flag], False, flag)

    def assert_counts(self, attempts, acknowledged, verified, skipped=0):
        summary = self.report['summary']
        self.assertEqual(summary['target_count'], 42)
        self.assertEqual(summary['put_attempt_count'], attempts)
        self.assertEqual(summary['put_acknowledged_count'], acknowledged)
        self.assertEqual(summary['target_verified_count'], verified)
        self.assertEqual(summary['unchanged_or_longer_count'], skipped)
        self.assertEqual(len(self.s3.writes), attempts)

    def assert_final_42(self):
        rows = self.report['objects']
        self.assertEqual(len(rows), 42)
        self.assertTrue(all('final_verification' in row for row in rows))
        self.assertEqual({row['version_id'] for row in rows},
                         {pin['version_id'] for pin in self.s3.pins.values()})
        # A failed immediate probe does not exempt untouched later objects from final reads.
        last_put = max((i for i, (name, _) in enumerate(self.s3.calls)
                        if name == 'put_object_retention'), default=-1)
        final = self.s3.calls[last_put+1:]
        for method in ('head_object', 'get_object_retention'):
            self.assertEqual({args['Key'] for name, args in final if name == method},
                             set(self.s3.pins))

    def assert_rejected_preflight(self):
        self.assertFalse(self.report['complete'])
        self.assertFalse(self.report['preflight_complete'])
        self.assertFalse(self.report['protection_verified'])
        self.assertFalse(self.report['retention_writes_attempted'])
        self.assertEqual(self.s3.writes, [])
        self.assert_public(self.report)

    def test_initial_report_fixed_authority_aware_clock_and_safe_defaults(self):
        self.assertEqual(extension.AUTHORIZATION, TOKEN)
        self.assertEqual(extension.RETAIN_UNTIL, TARGET)
        self.assertEqual(self.report['schema_version'], 1)
        self.assertEqual(self.report['operation'], 'extend-retention')
        self.assertEqual(self.report['target_retain_until'], TARGET.isoformat())
        self.assertEqual(point(self.report['checked_at']), NOW)
        for field in ('complete', 'preflight_complete', 'protection_verified',
                      'retention_writes_attempted'):
            self.assertIs(self.report[field], False)
        self.assert_public(self.report)
        for invalid in (datetime(2026, 10, 27), '2026-10-27', 42):
            with self.subTest(clock=invalid), self.assertRaises((ValueError, TypeError)):
                extension.initial_report(invalid)

    def test_success_extends_exact_8_plus_33_archives_and_one_receipt(self):
        self.extend()
        self.assertTrue(self.report['complete'])
        self.assertTrue(self.report['preflight_complete'])
        self.assertTrue(self.report['protection_verified'])
        self.assertTrue(self.report['retention_writes_attempted'])
        self.assert_counts(42, 42, 42)
        self.assertEqual(self.report['summary']['earliest_verified_retain_until'], TARGET.isoformat())
        self.assertEqual(set(self.s3.deadlines.values()), {TARGET})
        expected = set(self.s3.pins)
        self.assertEqual({args['Key'] for args in self.s3.writes}, expected)
        self.assertEqual(sum('/37715977350-1/' in key for key in expected), 8)
        self.assertEqual(sum('/40393597860-1/' in key for key in expected), 34)
        self.assertEqual(sum(key.endswith('recovery-preservation-report.json')
                             for key in expected), 1)
        self.assertEqual(self.report['ledger_sha256'], LEDGER_SHA)
        self.assertEqual(self.report['preservation_sha256'], PRESERVATION_SHA)
        self.assertEqual(self.report['audit_collector_sha256'],
                         hashlib.sha256(Path(audit.__file__).read_bytes()).hexdigest())
        for row in self.report['objects']:
            self.assertEqual(row['state'], 'extended_verified')
            self.assertIs(row['put_attempted'], True)
            self.assertIs(row['put_acknowledged'], True)
            self.assertIs(row['target_protection_verified'], True)
            self.assertIs(row['preflight']['metadata_verified'], True)
            key = next(key for key, identity in self.s3.identities.items()
                       if all(row.get(field) == value for field, value in identity.items()))
            self.assertEqual(row['expected_sha256'], self.s3.pins[key]['sha256'])
            self.assertEqual(point(row['final_verification']['retain_until']), TARGET)
        self.assert_final_42()
        self.assert_public(self.report)

    def test_equal_later_and_mixed_locks_are_idempotent_without_shortening(self):
        for partition in ('equal', 'later', 'mixed'):
            with self.subTest(partition=partition):
                self.setUp()
                original = {}
                for index, key in enumerate(self.s3.pins):
                    if partition == 'equal' or (partition == 'mixed' and index % 3 == 0):
                        self.s3.deadlines[key] = TARGET
                    elif partition == 'later' or (partition == 'mixed' and index % 3 == 1):
                        self.s3.deadlines[key] = TARGET+timedelta(days=12)
                    original[key] = self.s3.deadlines[key]
                self.extend()
                needed = {key for key, deadline in original.items() if deadline < TARGET}
                self.assertEqual({args['Key'] for args in self.s3.writes}, needed)
                self.assert_counts(len(needed), len(needed), 42, 42-len(needed))
                for key in self.s3.pins:
                    self.assertEqual(self.s3.deadlines[key], max(original[key], TARGET))
                self.assertEqual(sum(row['state'] == 'already_protected'
                                     for row in self.report['objects']), 42-len(needed))
                for row in self.report['objects']:
                    if row['state'] == 'already_protected':
                        self.assertFalse(row['put_attempted'])
                        self.assertFalse(row['put_acknowledged'])
                self.assertTrue(self.report['complete'])
                self.assert_final_42()
                self.assert_public(self.report)

    def test_before_at_after_horizon_and_naive_clock_guards(self):
        self.s3.deadlines = dict.fromkeys(self.s3.pins, TARGET+timedelta(days=1))
        self.extend(now=TARGET-timedelta(microseconds=1))
        self.assertTrue(self.report['complete'])
        for clock in (TARGET, TARGET+timedelta(microseconds=1), TARGET.replace(tzinfo=None)):
            with self.subTest(clock=clock):
                self.setUp()
                with self.assertRaises((ValueError, TypeError)):
                    self.extend(now=clock)
                self.assertEqual(self.s3.calls, [])
                self.assert_rejected_preflight()

    def test_authorization_and_nonempty_destination_kms_guards_before_reads(self):
        cases = [(token, URI, KMS) for token in ('', 'wrong', TOKEN+' ', None)]
        cases += [(TOKEN, uri, KMS) for uri in ('', None, 'https://'+PRIVATE, 's3://', URI+'?private=1')]
        cases += [(TOKEN, URI, kms) for kms in ('', '  ', None)]
        for token, uri, kms in cases:
            with self.subTest(token=token, uri=uri, kms=kms):
                self.setUp()
                with self.assertRaises(ValueError):
                    extension.extend(self.s3, uri, kms, self.report, token, now=NOW)
                self.assertEqual(self.s3.calls, [])
                self.assert_rejected_preflight()

    def test_both_frozen_evidence_digests_are_authenticated_before_reads(self):
        self.assertEqual(coverage.LEDGER_SHA256, LEDGER_SHA)
        self.assertEqual(audit.PRESERVATION_SHA256, PRESERVATION_SHA)
        read = Path.read_bytes
        for target in (LEDGER, PRESERVATION):
            with self.subTest(target=target.name):
                self.setUp()
                def tampered(path):
                    raw = read(path)
                    return raw+b'\n' if path.resolve() == target else raw
                with patch.object(Path, 'read_bytes', tampered), self.assertRaises(ValueError):
                    self.extend()
                self.assertEqual(self.s3.calls, [])
                self.assert_rejected_preflight()

    def test_missing_duplicate_or_extra_target_rejected_before_reads(self):
        targets = audit.retention_targets(*audit.load_evidence())
        self.assertEqual(len(targets), 42)
        for partition, altered in (
                ('missing', targets[:-1]),
                ('duplicate', targets[:-1]+[targets[0]]),
                ('extra', targets+[targets[0]])):
            with self.subTest(partition=partition):
                self.setUp()
                with patch.object(audit, 'retention_targets', return_value=altered), \
                     self.assertRaises(ValueError):
                    self.extend()
                self.assertEqual(self.s3.calls, [])
                self.assert_rejected_preflight()

    def test_invalid_last_target_prevents_every_write(self):
        for partition, method, change in (
                ('version', 'head_object', {'VersionId': 'wrong-version'}),
                ('length', 'head_object', {'ContentLength': 0}),
                ('kms', 'head_object', {'SSEKMSKeyId': PRIVATE+'-wrong'}),
                ('encryption', 'head_object', {'ServerSideEncryption': 'AES256'}),
                ('mode', 'get_object_retention', {'Mode': 'GOVERNANCE'}),
                ('expired', 'get_object_retention', {'RetainUntilDate': NOW}),
                ('naive_deadline', 'get_object_retention',
                 {'RetainUntilDate': NOW.replace(tzinfo=None)}),
                ('shortened', 'get_object_retention', {'RetainUntilDate': NOW+timedelta(days=1)}),
                ('denied', 'head_object', None)):
            with self.subTest(partition=partition):
                self.setUp()
                last = list(self.s3.pins)[-1]
                if change is None:
                    self.s3.failures[(method, last, 1)] = aws_error()
                else:
                    self.s3.overrides[(method, last, 1)] = change
                self.extend()
                self.assert_rejected_preflight()
                self.assertEqual(len(self.report['objects']), 42)
                self.assertFalse(self.report['objects'][-1]['preflight']['metadata_verified'])
                self.assertTrue(all(row['preflight']['metadata_verified']
                                    for row in self.report['objects'][:-1]))
                self.assertTrue(all(row['state'] == 'not_attempted'
                                    for row in self.report['objects']))
                self.assertTrue(all(not row['put_attempted'] and not row['put_acknowledged']
                                    for row in self.report['objects']))

    def test_configuration_denied_unknown_missing_and_unsafe_defaults_block_writes(self):
        cases = [(method, aws_error()) for method in CONFIG_CALLS]
        cases += [
            ('get_bucket_lifecycle_configuration', aws_error(PRIVATE)),
            ('get_bucket_encryption', BotoCoreError()),
            ('get_bucket_versioning', {'Status': 'Suspended'}),
            ('get_bucket_versioning', {}),
            ('get_object_lock_configuration', {'ObjectLockConfiguration': {
                'ObjectLockEnabled': 'Disabled'}}),
            ('get_object_lock_configuration', aws_error('ObjectLockConfigurationNotFoundError')),
            ('get_bucket_encryption', aws_error('ServerSideEncryptionConfigurationNotFoundError')),
        ]
        for default in ({'Mode': 'COMPLIANCE', 'Days': 34},
                        {'Mode': 'GOVERNANCE', 'Days': 35},
                        {'Mode': 'COMPLIANCE', 'Years': 0}, {}):
            cases.append(('get_object_lock_configuration', {'ObjectLockConfiguration': {
                'ObjectLockEnabled': 'Enabled', 'Rule': {'DefaultRetention': default}}}))
        for algorithm, identity in (('aws:kms', PRIVATE+'-wrong'), ('AES256', KMS)):
            cases.append(('get_bucket_encryption', {'ServerSideEncryptionConfiguration': {
                'Rules': [{'ApplyServerSideEncryptionByDefault': {
                    'SSEAlgorithm': algorithm, 'KMSMasterKeyID': identity}}]}}))
        for method, response in cases:
            with self.subTest(method=method, response=type(response).__name__):
                self.setUp()
                if isinstance(response, Exception):
                    self.s3.failures[method] = response
                else:
                    self.s3.config[method] = response
                self.extend()
                self.assert_rejected_preflight()
                self.assertEqual({name for name, _ in self.s3.calls if name in CONFIG_CALLS},
                                 set(CONFIG_CALLS))

    def test_one_year_floor_and_absent_lifecycle_are_valid_known_observations(self):
        self.s3.config['get_object_lock_configuration']['ObjectLockConfiguration']['Rule'] = {
            'DefaultRetention': {'Mode': 'COMPLIANCE', 'Years': 1}}
        self.s3.failures['get_bucket_lifecycle_configuration'] = aws_error('NoSuchLifecycleConfiguration')
        self.extend()
        self.assertTrue(self.report['complete'])
        self.assert_counts(42, 42, 42)
        self.assertEqual(self.report['configuration']['lifecycle']['state'], 'not_configured')
        self.assertEqual(self.report['configuration']['object_lock']['default_years'], 1)
        self.assert_public(self.report)

    def test_first_and_later_denied_writes_stop_but_finally_observe_all_42(self):
        for failure_at in (1, 4):
            with self.subTest(failure_at=failure_at):
                self.setUp()
                self.s3.write_failure = lambda count, key: aws_error() if count == failure_at else None
                self.extend()
                self.assertFalse(self.report['complete'])
                self.assertTrue(self.report['preflight_complete'])
                self.assertFalse(self.report['protection_verified'])
                self.assert_counts(failure_at, failure_at-1, failure_at-1)
                rows = self.report['objects']
                self.assertEqual([row['state'] for row in rows[:failure_at-1]],
                                 ['extended_verified']*(failure_at-1))
                self.assertEqual(rows[failure_at-1]['state'], 'write_failed')
                self.assertTrue(rows[failure_at-1]['put_attempted'])
                self.assertFalse(rows[failure_at-1]['put_acknowledged'])
                self.assertTrue(all(row['state'] == 'not_attempted' for row in rows[failure_at:]))
                self.assertEqual(sum(deadline == TARGET for deadline in self.s3.deadlines.values()),
                                 failure_at-1)
                self.assert_final_42()
                self.assert_public(self.report)

    def test_ambiguous_applied_write_keeps_failure_even_when_final_probe_proves_protection(self):
        for failure_at in (1, 42):
            with self.subTest(failure_at=failure_at):
                self.setUp()
                self.s3.apply_failed_write = True
                self.s3.write_failure = lambda count, key: aws_error(PRIVATE) if count == failure_at else None
                self.extend()
                self.assert_counts(failure_at, failure_at-1, failure_at)
                self.assertFalse(self.report['complete'])
                self.assertEqual(self.report['protection_verified'], failure_at == 42)
                row = self.report['objects'][failure_at-1]
                self.assertEqual(row['state'], 'write_failed')
                self.assertTrue(row['target_protection_verified'])
                self.assertFalse(row['put_acknowledged'])
                self.assertEqual(point(row['final_verification']['retain_until']), TARGET)
                self.assert_final_42()
                self.assert_public(self.report)

    def test_missing_or_non_200_acknowledgement_is_never_success_or_retried(self):
        for response in (None, [], {}, {'ResponseMetadata': None},
                         {'ResponseMetadata': []}, {'ResponseMetadata': {}},
                         {'ResponseMetadata': {'HTTPStatusCode': 204}},
                         {'ResponseMetadata': {'HTTPStatusCode': 500}}):
            with self.subTest(response=response):
                self.setUp()
                self.s3.write_response = response
                self.extend()
                self.assert_counts(1, 0, 1)
                self.assertFalse(self.report['complete'])
                self.assertEqual(self.report['objects'][0]['state'], 'write_failed')
                self.assert_final_42()
                self.assert_public(self.report)

    def test_post_write_bad_metadata_stops_next_put_despite_later_good_final_probe(self):
        for method, response in (
                ('head_object', {'ContentLength': 0}),
                ('head_object', {'SSEKMSKeyId': PRIVATE+'-wrong'}),
                ('get_object_retention', {'RetainUntilDate': TARGET-timedelta(microseconds=1)}),
                ('get_object_retention', {'Mode': 'GOVERNANCE'})):
            with self.subTest(method=method, response=response):
                self.setUp()
                first = next(iter(self.s3.pins))
                self.s3.overrides[(method, first, 2)] = response
                self.extend()
                self.assert_counts(1, 1, 1)
                self.assertEqual(self.report['objects'][0]['state'], 'verification_failed')
                self.assertTrue(self.report['objects'][0]['put_acknowledged'])
                self.assertTrue(self.report['objects'][0]['target_protection_verified'])
                self.assertFalse(self.report['complete'])
                self.assert_final_42()
                self.assert_public(self.report)

    def test_final_verification_requires_observed_deadline_not_merely_approved_target(self):
        later = TARGET+timedelta(days=30)
        self.s3.deadlines = dict.fromkeys(self.s3.pins, later)
        last = list(self.s3.pins)[-1]
        self.s3.overrides[('get_object_retention', last, 2)] = {'RetainUntilDate': TARGET}
        self.extend()
        self.assert_counts(0, 0, 41, 42)
        self.assertFalse(self.report['complete'])
        self.assertFalse(self.report['protection_verified'])
        row = self.report['objects'][-1]
        self.assertFalse(row['target_protection_verified'])
        self.assertFalse(row['final_verification']['prior_lock_not_shortened'])
        self.assert_final_42()

    def test_final_probe_denial_cannot_be_hidden_by_successful_write_verification(self):
        last = list(self.s3.pins)[-1]
        self.s3.failures[('head_object', last, 3)] = aws_error()
        self.extend()
        self.assert_counts(42, 42, 41)
        self.assertFalse(self.report['complete'])
        self.assertFalse(self.report['protection_verified'])
        self.assertFalse(self.report['objects'][-1]['target_protection_verified'])
        self.assertEqual(self.report['objects'][-1]['final_verification']['state'], 'unavailable')
        self.assert_public(self.report)

    def test_checkpoints_capture_safe_before_put_and_acknowledged_verified_progress(self):
        snapshots = []
        def checkpoint(report):
            self.assert_public(report)
            snapshots.append(copy.deepcopy(report))
        def before_put(request):
            self.assertTrue(snapshots, 'Must journal locally before an irreversible request')
            row = next(row for row in snapshots[-1]['objects']
                       if row['version_id'] == request['VersionId'])
            self.assertTrue(row['put_attempted'])
            self.assertFalse(row['put_acknowledged'])
        self.s3.before_put = before_put
        self.extend(checkpoint=checkpoint)
        self.assertTrue(self.report['complete'])
        self.assertIsNone(snapshots[0]['summary']['earliest_verified_retain_until'])
        for pin in self.s3.pins.values():
            progressed = [row for snapshot in snapshots for row in snapshot.get('objects', [])
                          if row['version_id'] == pin['version_id']]
            self.assertTrue(any(row['put_acknowledged'] for row in progressed))
            self.assertTrue(any(row['state'] == 'extended_verified' for row in progressed))
        self.assertEqual(snapshots[-1]['summary'], self.report['summary'])
        self.assertTrue(snapshots[-1]['complete'])

    def test_checkpoint_failure_before_or_after_first_write_stops_further_mutations(self):
        for phase in ('initial', 'before_request', 'after_ack'):
            for error_type in (OSError, ValueError, TypeError):
                with self.subTest(phase=phase, error=error_type.__name__):
                    self.setUp()
                    def checkpoint(report):
                        attempted = any(row['put_attempted'] for row in report['objects'])
                        acknowledged = any(row['put_acknowledged'] for row in report['objects'])
                        if (phase == 'initial' or (phase == 'before_request' and attempted)
                                or (phase == 'after_ack' and acknowledged)):
                            raise error_type(PRIVATE)
                    self.extend(checkpoint=checkpoint)
                    expected = int(phase == 'after_ack')
                    self.assert_counts(expected, expected, expected)
                    self.assertEqual(self.report['retention_writes_attempted'], bool(expected))
                    self.assertEqual(sum(row['put_attempted'] for row in self.report['objects']),
                                     expected)
                    self.assertEqual(sum(row['put_acknowledged'] for row in self.report['objects']),
                                     expected)
                    self.assertEqual(sum(value == TARGET for value in self.s3.deadlines.values()),
                                     expected)
                    self.assertFalse(self.report['complete'])
                    self.assertFalse(self.report['protection_verified'])
                    self.assertTrue(self.report['preflight_complete'])
                    self.assertEqual(self.report['journal_error'],
                                     {'state': 'incomplete', 'error_class': error_type.__name__})
                    self.assert_final_42()
                    self.assert_public(self.report)

    def test_acknowledged_write_checkpoint_failure_still_collects_all_final_versions(self):
        failed = False
        def checkpoint(report):
            nonlocal failed
            if not failed and any(row['put_acknowledged'] for row in report['objects']):
                failed = True
                raise OSError(PRIVATE)
        self.extend(checkpoint=checkpoint)
        self.assertTrue(failed)
        self.assertFalse(self.report['complete'])
        self.assert_counts(1, 1, 1)
        self.assertTrue(self.report['objects'][0]['put_attempted'])
        self.assertTrue(self.report['objects'][0]['put_acknowledged'])
        self.assertEqual(sum(value == TARGET for value in self.s3.deadlines.values()), 1)
        self.assert_final_42()
        self.assert_public(self.report)

    def test_final_checkpoint_failure_is_incomplete_even_when_all_42_are_protected(self):
        for error_type in (OSError, ValueError, TypeError):
            with self.subTest(error=error_type.__name__):
                self.setUp()
                reached_final = False
                def checkpoint(report):
                    nonlocal reached_final
                    if report['complete']:
                        reached_final = True
                        raise error_type(PRIVATE)
                self.extend(checkpoint=checkpoint)
                self.assertTrue(reached_final)
                self.assert_counts(42, 42, 42)
                self.assertTrue(self.report['protection_verified'])
                self.assertFalse(self.report['complete'])
                self.assertEqual(self.report['stage'], 'incomplete')
                self.assertEqual(self.report['journal_error'],
                                 {'state': 'incomplete', 'error_class': error_type.__name__})
                self.assert_final_42()
                self.assert_public(self.report)

    def cli_environment(self):
        return {
            'GITHUB_REF': 'refs/heads/master', 'GITHUB_REPOSITORY': 'tomacox74/js2il',
            'GITHUB_SHA': 'a'*40, 'GITHUB_RUN_ID': '123456', 'GITHUB_RUN_ATTEMPT': '2',
            'TEST262_BACKUP_S3_URI': URI, 'TEST262_BACKUP_KMS_KEY': KMS,
        }

    def invoke_cli(self, path, token=TOKEN):
        output = io.StringIO()
        with redirect_stdout(output), redirect_stderr(output), \
             patch.object(extension, 'datetime', wraps=datetime) as clock:
            clock.now.return_value = NOW
            try:
                status = extension.main(['--authorization', token, '--output', str(path)])
            except SystemExit as error:
                status = error.code
        self.assertNotIn(PRIVATE, output.getvalue())
        return status

    def test_cli_rejects_bad_scope_token_and_configuration_before_client(self):
        changes = (
            {'GITHUB_REF': 'refs/heads/topic'}, {'GITHUB_REF': 'refs/tags/master'},
            {'GITHUB_REPOSITORY': ''}, {'GITHUB_REPOSITORY': 'other/repository'},
            {'GITHUB_SHA': 'a'*39}, {'GITHUB_SHA': 'z'*40}, {'GITHUB_SHA': 'a'*41},
            {'GITHUB_RUN_ID': '0'}, {'GITHUB_RUN_ID': '-1'},
            {'GITHUB_RUN_ATTEMPT': '0'}, {'GITHUB_RUN_ATTEMPT': '2x'},
            {'TEST262_BACKUP_S3_URI': ''}, {'TEST262_BACKUP_S3_URI': 'https://'+PRIVATE},
            {'TEST262_BACKUP_KMS_KEY': ''}, {'TEST262_BACKUP_KMS_KEY': '  '},
        )
        for change, token in [(change, TOKEN) for change in changes]+[({}, ''), ({}, 'wrong')]:
            with self.subTest(change=change, token=token), tempfile.TemporaryDirectory() as directory:
                path = Path(directory)/'extension.json'
                with patch.dict(os.environ, dict(self.cli_environment(), **change), clear=True), \
                     patch('boto3.client') as client:
                    self.assertNotEqual(self.invoke_cli(path, token), 0)
                    client.assert_not_called()
                report = json.loads(path.read_bytes())
                self.assertFalse(report['complete'])
                self.assert_public(report)

    def test_cli_requires_explicit_arguments_and_refuses_existing_source_or_symlink_output(self):
        with patch('boto3.client') as client, redirect_stderr(io.StringIO()):
            for args in ([], ['--authorization', TOKEN], ['--output', 'unused.json']):
                with self.subTest(args=args), self.assertRaises(SystemExit):
                    extension.main(args)
            client.assert_not_called()
        with tempfile.TemporaryDirectory() as directory:
            existing = Path(directory)/'existing.json'
            existing.write_bytes(b'accepted existing evidence')
            link = Path(directory)/'link.json'
            link.symlink_to(existing)
            source = Path(extension.__file__)
            for path in (existing, link, source, LEDGER, PRESERVATION):
                with self.subTest(path=path.name):
                    before = path.read_bytes()
                    with patch.dict(os.environ, self.cli_environment(), clear=True), \
                         patch('boto3.client') as client:
                        self.assertNotEqual(self.invoke_cli(path), 0)
                        client.assert_not_called()
                    self.assertEqual(path.read_bytes(), before)

    def test_cli_real_extension_uses_local_checkpoints_retries_one_and_no_gh_token(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory)/'extension.json'
            journals = []
            def before_put(request):
                saved = json.loads(path.read_bytes())
                row = next(row for row in saved['objects']
                           if row['version_id'] == request['VersionId'])
                self.assertTrue(row['put_attempted'])
                self.assertFalse(row['put_acknowledged'])
                self.assert_public(saved)
                journals.append(saved)
            self.s3.before_put = before_put
            with patch.dict(os.environ, self.cli_environment(), clear=True), \
                 patch('boto3.client', return_value=self.s3) as client, \
                 patch('botocore.config.Config') as config, \
                 patch.object(extension, 'initial_report', return_value=self.report):
                self.assertEqual(self.invoke_cli(path), 0)
            config.assert_called_once_with(retries={'total_max_attempts': 1})
            client.assert_called_once_with('s3', config=config.return_value)
            saved = json.loads(path.read_bytes())
            self.assertEqual(len(journals), 42)
            self.assertTrue(saved['complete'])
            self.assertTrue(saved['protection_verified'])
            self.assertEqual(saved['source_revision'], 'a'*40)
            self.assertEqual(saved['run_id'], 123456)
            self.assertEqual(saved['run_attempt'], 2)
            self.assertIsNotNone(point(saved['checked_at']).tzinfo)
            self.assert_public(saved)
            self.assertEqual({entry.name for entry in path.parent.iterdir()}, {path.name})

    def test_cli_partial_failure_reports_actual_mutations_and_nonzero_status(self):
        for error in (aws_error(), aws_error(PRIVATE), BotoCoreError(), OSError(PRIVATE)):
            with self.subTest(error=type(error).__name__), tempfile.TemporaryDirectory() as directory:
                self.setUp()
                path = Path(directory)/'extension.json'
                self.s3.write_failure = lambda count, key: error if count == 3 else None
                with patch.dict(os.environ, self.cli_environment(), clear=True), \
                     patch('boto3.client', return_value=self.s3), \
                     patch.object(extension, 'initial_report', return_value=self.report):
                    self.assertEqual(self.invoke_cli(path), 1)
                saved = json.loads(path.read_bytes())
                self.assertFalse(saved['complete'])
                self.assertEqual(saved['summary']['put_attempt_count'], 3)
                self.assertEqual(saved['summary']['put_acknowledged_count'], 2)
                self.assertEqual(sum(value == TARGET for value in self.s3.deadlines.values()), 2)
                self.assert_public(saved)

    def test_cli_client_creation_failure_persists_safe_report_without_any_storage_calls(self):
        for error in (aws_error(), aws_error(PRIVATE), BotoCoreError(), OSError(PRIVATE)):
            with self.subTest(error=type(error).__name__), tempfile.TemporaryDirectory() as directory:
                path = Path(directory)/'extension.json'
                with patch.dict(os.environ, self.cli_environment(), clear=True), \
                     patch('boto3.client', side_effect=error):
                    self.assertEqual(self.invoke_cli(path), 1)
                saved = json.loads(path.read_bytes())
                self.assertFalse(saved['complete'])
                self.assertFalse(saved['retention_writes_attempted'])
                self.assertEqual(saved['objects'], [])
                self.assertEqual(self.s3.calls, [])
                self.assert_public(saved)

    def test_cli_output_initialization_io_failure_is_safe_nonzero_before_client(self):
        for operation in ('mkdir', 'open'):
            with self.subTest(operation=operation), tempfile.TemporaryDirectory() as directory:
                path = Path(directory)/'extension.json'
                original = getattr(Path, operation)
                def fail_output(target, *args, **kwargs):
                    if ((operation == 'mkdir' and target == path.parent)
                            or (operation == 'open' and target == path)):
                        raise OSError(PRIVATE)
                    return original(target, *args, **kwargs)
                with patch.dict(os.environ, self.cli_environment(), clear=True), \
                     patch('boto3.client') as client, \
                     patch.object(Path, operation, fail_output):
                    self.assertEqual(self.invoke_cli(path), 1)
                    client.assert_not_called()
                self.assertFalse(path.exists())
                self.assertEqual(self.s3.calls, [])

    def test_cli_checkpoint_io_failure_stops_writes_and_retains_safe_local_progress(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory)/'extension.json'
            original_replace = Path.replace
            def failing_replace(source, target):
                if self.s3.writes:
                    raise OSError(PRIVATE)
                return original_replace(source, target)
            with patch.dict(os.environ, self.cli_environment(), clear=True), \
                 patch('boto3.client', return_value=self.s3), \
                 patch.object(Path, 'replace', failing_replace):
                self.assertEqual(self.invoke_cli(path), 1)
            self.assertEqual(len(self.s3.writes), 1)
            saved = json.loads(path.read_bytes())
            self.assertFalse(saved['complete'])
            self.assertTrue(saved['retention_writes_attempted'])
            self.assertEqual(sum(row['put_attempted'] for row in saved['objects']), 1)
            self.assert_public(saved)

    def test_cli_nonzero_if_either_completion_or_protection_is_false(self):
        for complete, protected in ((False, False), (False, True), (True, False), (True, True)):
            with self.subTest(complete=complete, protected=protected), \
                 tempfile.TemporaryDirectory() as directory:
                path = Path(directory)/'extension.json'
                def extend(s3, uri, kms, report, authorization, **kwargs):
                    report.update(complete=complete, protection_verified=protected)
                with patch.dict(os.environ, self.cli_environment(), clear=True), \
                     patch('boto3.client', return_value=self.s3), \
                     patch.object(extension, 'extend', side_effect=extend):
                    self.assertEqual(self.invoke_cli(path), 0 if complete and protected else 1)

    def test_workflow_static_authorization_privilege_and_report_guards(self):
        # Deliberately static: never execute even the workflow's local shell.
        workflow = (ROOT/'.github/workflows/test262-cutover-evidence.yml').read_text()
        self.assertIn("if: github.ref == 'refs/heads/master'", workflow)
        self.assertIn('environment: test262-catalogue-backup', workflow)
        self.assertIn('group: test262-cutover-evidence-preservation', workflow)
        self.assertIn('cancel-in-progress: false', workflow)
        self.assertRegex(workflow, r'options:\s*(?:.*\n)*?\s+- extend-retention')
        self.assertRegex(workflow, r'retention_authorization:\n(?:[^\n]*\n)*?\s+default: [\'"]{2}')
        validation = workflow.split('- name: Validate reviewed operation', 1)[1].split(
            '- uses: aws-actions/configure-aws-credentials', 1)[0]
        self.assertIn('RETENTION_AUTHORIZATION: ${{ inputs.retention_authorization }}', validation)
        branch = re.search(r'(?m)^\s+extend-retention\)\s*(.*?)^\s+;;', validation, re.S)
        self.assertIsNotNone(branch, 'Extension must have its own guarded authorization branch')
        self.assertIn(f'[ "$RETENTION_AUTHORIZATION" != "{TOKEN}" ]', branch[1])
        self.assertIn('exit 1', branch[1])
        blocks = re.split(r'(?m)^      - ', workflow)
        credentials = next(block for block in blocks if 'configure-aws-credentials@' in block)
        policies = [json.loads(raw) for raw in re.findall(
            r"'(\{\"Version\".*?\})' \|\|", credentials)]
        self.assertEqual(len(policies), 5)
        match = re.search(r"inputs.operation == 'extend-retention'\s*&&\s*'([^']+)'",
                          credentials)
        self.assertIsNotNone(match)
        policy = json.loads(match[1])
        self.assertEqual(policy, policies[2])
        actions = {action for statement in policy['Statement'] for action in statement['Action']}
        self.assertEqual(actions, {
            's3:GetBucketVersioning', 's3:GetBucketObjectLockConfiguration',
            's3:GetLifecycleConfiguration', 's3:GetEncryptionConfiguration',
            's3:GetObjectVersion', 's3:GetObjectRetention', 's3:PutObjectRetention'})
        self.assertTrue(all(statement['Effect'] == 'Allow' for statement in policy['Statement']))
        extension_step = next(block for block in blocks
                              if 'python -m scripts.test262.central.retention_extension' in block)
        self.assertIn("if: inputs.operation == 'extend-retention'", extension_step)
        self.assertIn('--authorization "$RETENTION_AUTHORIZATION"', extension_step)
        self.assertIn('--output /tmp/test262-cutover-report/retention-extension-report.json',
                      extension_step)
        env = extension_step.split('env:', 1)[1].split('run:', 1)[0]
        self.assertEqual(set(re.findall(r'(?m)^\s+([A-Z0-9_]+):', env)),
                         {'TEST262_BACKUP_S3_URI', 'TEST262_BACKUP_KMS_KEY',
                          'RETENTION_AUTHORIZATION'})
        self.assertNotIn('secrets.', extension_step)
        for module, operation in (
                ('cutover_evidence', 'preserve'), ('recovery_preservation', 'preserve-recovery'),
                ('retained_evidence', 'verify-retained'), ('retained_discovery', 'discover-retained'),
                ('retention_audit', 'audit-retention')):
            block = next(block for block in blocks
                         if 'python -m scripts.test262.central.'+module in block)
            self.assertIn(f"if: inputs.operation == '{operation}'", block)
        upload = next(block for block in blocks if 'actions/upload-artifact@' in block)
        self.assertIn('if: always()', upload)
        self.assertIn('path: /tmp/test262-cutover-report/*report.json', upload)


if __name__ == '__main__':
    unittest.main()
