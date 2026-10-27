"""Offline metadata-only contracts for the #2343 retention audit."""
from contextlib import redirect_stderr, redirect_stdout
import copy
from datetime import datetime, timedelta, timezone
import io
import json
import os
from pathlib import Path
import re
import subprocess
import tempfile
import unittest
from unittest.mock import Mock, patch

from botocore.exceptions import BotoCoreError, ClientError, EndpointConnectionError
from scripts.test262.central import recovery_coverage as coverage
from scripts.test262.central import retention_audit as audit


ROOT = Path(__file__).resolve().parents[3]
LEDGER = ROOT/'docs/ECMA262/evidence/historical-retention-ledger-2242.json'
PRESERVATION = ROOT/'docs/ECMA262/evidence/retained-recovery-preservation-40393597860.json'
NOW = datetime(2026, 10, 27, tzinfo=timezone.utc)
PRIVATE = 'PRIVATE-do-not-publish'
BUCKET = PRIVATE+'-bucket'
BASE = PRIVATE+'/destination'
KMS = 'arn:aws:kms:us-east-1:123456789012:key/'+PRIVATE
SAFE_FLAGS = (
    'database_access', 'object_store_writes', 'payload_reads', 'payload_digest_verified',
    'notifications_enabled', 'policy_changes', 'imports', 'restores', 'fixture_execution',
)
CONFIG_CALLS = (
    'get_bucket_versioning', 'get_object_lock_configuration',
    'get_bucket_lifecycle_configuration', 'get_bucket_encryption',
)


def aws_error(code):
    return ClientError({'Error': {'Code': code, 'Message': PRIVATE},
                        'ResponseMetadata': {'RequestId': PRIVATE}}, PRIVATE)


class MetadataOnlyS3:
    """No fallback method: payload/list/write/KMS calls fail immediately."""
    def __init__(self, ledger, preservation):
        self.calls = []
        self.failures = {}
        self.config = {
            'get_bucket_versioning': {'Status': 'Enabled', 'MFADelete': 'Disabled'},
            'get_object_lock_configuration': {'ObjectLockConfiguration': {
                'ObjectLockEnabled': 'Enabled',
                'Rule': {'DefaultRetention': {'Mode': 'COMPLIANCE', 'Days': 35}}}},
            'get_bucket_lifecycle_configuration': {'Rules': [
                {'Status': 'Disabled', 'Filter': {'Prefix': BASE+'-outside/'}}]},
            'get_bucket_encryption': {'ServerSideEncryptionConfiguration': {'Rules': [
                {'ApplyServerSideEncryptionByDefault': {
                    'SSEAlgorithm': 'aws:kms', 'KMSMasterKeyID': KMS},
                 'BucketKeyEnabled': True}]}},
        }
        self.targets = {}
        for row in ledger['archives']:
            run = 40393597860 if row['verification']['run_id'] == 40393597860 else 37715977350
            self.targets[f"{BASE}/cutover-evidence/{run}-1/{row['artifact_id']}.zip"] = \
                copy.deepcopy(row['object'])
        self.targets[f'{BASE}/cutover-evidence/40393597860-1/recovery-preservation-report.json'] = \
            copy.deepcopy(preservation['report']['receipt'])
        self.head_changes = {}
        self.lock_changes = {}
        self.head_missing = set()
        self.lock_missing = set()

    def read(self, method, kwargs):
        self.calls.append((method, kwargs))
        if method in self.failures:
            raise self.failures[method]
        if kwargs.get('Key') in self.failures:
            raise self.failures[kwargs['Key']]
        if kwargs != {'Bucket': BUCKET}:
            key = kwargs['Key']
            if kwargs != {'Bucket': BUCKET, 'Key': key,
                          'VersionId': self.targets[key]['version_id']}:
                raise AssertionError('Every object request must pin its accepted immutable version')
            return self.targets[key]
        return copy.deepcopy(self.config[method])

    def get_bucket_versioning(self, **kwargs):
        return self.read('get_bucket_versioning', kwargs)

    def get_object_lock_configuration(self, **kwargs):
        return self.read('get_object_lock_configuration', kwargs)

    def get_bucket_lifecycle_configuration(self, **kwargs):
        return self.read('get_bucket_lifecycle_configuration', kwargs)

    def get_bucket_encryption(self, **kwargs):
        return self.read('get_bucket_encryption', kwargs)

    def head_object(self, **kwargs):
        row = self.read('head_object', kwargs)
        response = dict({'VersionId': row['version_id'], 'ContentLength': row['content_length'],
                         'ServerSideEncryption': 'aws:kms', 'SSEKMSKeyId': KMS},
                        **self.head_changes)
        return {key: value for key, value in response.items() if key not in self.head_missing}

    def get_object_retention(self, **kwargs):
        row = self.read('get_object_retention', kwargs)
        response = dict({'Mode': 'COMPLIANCE', 'RetainUntilDate': row['retain_until']},
                        **self.lock_changes)
        return {'Retention': {key: value for key, value in response.items()
                              if key not in self.lock_missing}}


class RetentionAuditTests(unittest.TestCase):
    def setUp(self):
        self.ledger = json.loads(LEDGER.read_bytes())
        self.preservation = json.loads(PRESERVATION.read_bytes())
        self.s3 = MetadataOnlyS3(self.ledger, self.preservation)

    def assert_public(self, report):
        encoded = json.dumps(report)
        for secret in (PRIVATE, BUCKET, BASE, KMS, 'Traceback', 'RequestId'):
            self.assertNotIn(secret, encoded)

    def assert_safe_flags(self, report):
        for flag in SAFE_FLAGS:
            self.assertIs(report[flag], False, flag)

    def configuration(self, base=BASE):
        return audit.audit_configuration(self.s3, BUCKET, base, KMS)

    def objects(self, now=NOW):
        return audit.audit_objects(self.s3, BUCKET, BASE, KMS,
                                   self.ledger, self.preservation, now)

    def test_initial_report_requires_aware_clock_and_never_claims_side_effects(self):
        report = audit.initial_report(NOW)
        self.assert_safe_flags(report)
        self.assertIs(report['audit_complete'], False)
        self.assertEqual(datetime.fromisoformat(report['checked_at']), NOW)
        for invalid in (datetime(2026, 10, 27), '2026-10-27', 42):
            with self.subTest(case=type(invalid).__name__):
                with self.assertRaises((ValueError, TypeError)):
                    audit.initial_report(invalid)

    def test_configuration_projects_four_independent_sanitized_observations(self):
        result = self.configuration()
        self.assertEqual(set(result), {'versioning', 'object_lock', 'lifecycle', 'encryption'})
        self.assertEqual([name for name, _ in self.s3.calls], list(CONFIG_CALLS))
        self.assertTrue(all(kwargs == {'Bucket': BUCKET} for _, kwargs in self.s3.calls))
        self.assertTrue(all(row['state'] == 'observed' for row in result.values()))
        self.assertIs(result['versioning']['enabled'], True)
        self.assertEqual(result['versioning']['mfa_delete'], 'Disabled')
        self.assertEqual(result['object_lock']['default_mode'], 'COMPLIANCE')
        self.assertEqual(result['object_lock']['default_days'], 35)
        self.assertEqual(result['encryption']['algorithms'], ['aws:kms'])
        self.assertIs(result['encryption']['configured_kms_identity_matches_exactly'], True)
        self.assertEqual(result['lifecycle']['rules'], [{
            'index': 0, 'enabled': False, 'destination_relation': 'outside',
            'conditions_unassessed': False, 'transition_count': 0,
            'noncurrent_transition_count': 0,
        }])
        self.assert_public(result)

    def test_configuration_denials_missing_settings_and_sdk_errors_do_not_stop_reads(self):
        for partition, error, state, code in (
                ('denied', aws_error('AccessDenied'), 'unavailable', 'AccessDenied'),
                ('missing', aws_error('NoSuchLifecycleConfiguration'), 'not_configured',
                 'NoSuchLifecycleConfiguration'),
                ('unknown', aws_error(PRIVATE), 'unavailable', 'UnrecognizedAwsError'),
                ('derived_sdk', EndpointConnectionError(endpoint_url='https://'+PRIVATE),
                 'unavailable', None),
                ('sdk', BotoCoreError(), 'unavailable', None)):
            with self.subTest(partition=partition):
                self.s3.calls.clear()
                self.s3.failures = {'get_bucket_lifecycle_configuration': error}
                result = self.configuration()
                row = result['lifecycle']
                self.assertEqual(row['state'], state)
                self.assertEqual(row['error_class'], type(error).__name__)
                if code is not None:
                    self.assertEqual(row['aws_error_code'], code)
                self.assertEqual([name for name, _ in self.s3.calls], list(CONFIG_CALLS))
                self.assertEqual(result['encryption']['state'], 'observed')
                self.assert_public(result)
        self.s3.failures = {'get_bucket_versioning': aws_error('AccessDenied')}
        self.s3.calls.clear()
        result = self.configuration()
        self.assertEqual(result['versioning']['state'], 'unavailable')
        self.assertTrue(all(result[name]['state'] == 'observed'
                            for name in ('object_lock', 'lifecycle', 'encryption')))
        self.assertEqual(len(self.s3.calls), 4)

    def test_configuration_invalid_shapes_never_imply_protection(self):
        for method, name, response in (
                ('get_bucket_versioning', 'versioning', {'Status': PRIVATE}),
                ('get_object_lock_configuration', 'object_lock',
                 {'ObjectLockConfiguration': {'ObjectLockEnabled': PRIVATE}}),
                ('get_bucket_lifecycle_configuration', 'lifecycle', {'Rules': PRIVATE}),
                ('get_bucket_encryption', 'encryption',
                 {'ServerSideEncryptionConfiguration': {'Rules': [None]}})):
            with self.subTest(observation=name):
                self.s3.config[method] = response
                result = self.configuration()
                self.assertEqual(result[name]['state'], 'invalid')
                self.assert_public(result)

    def test_absent_lock_lifecycle_and_encryption_are_not_denial_or_protection(self):
        for partition, method, code in (
                ('object_lock', 'get_object_lock_configuration',
                 'ObjectLockConfigurationNotFoundError'),
                ('lifecycle', 'get_bucket_lifecycle_configuration',
                 'NoSuchLifecycleConfiguration'),
                ('encryption', 'get_bucket_encryption',
                 'ServerSideEncryptionConfigurationNotFoundError')):
            with self.subTest(observation=partition):
                self.s3.calls.clear()
                self.s3.failures = {method: aws_error(code)}
                result = self.configuration()
                self.assertEqual(result[partition], {
                    'state': 'not_configured', 'error_class': 'ClientError',
                    'aws_error_code': code,
                })
                self.assertEqual([name for name, _ in self.s3.calls], list(CONFIG_CALLS))
                self.assert_public(result)

    def test_malformed_nested_encryption_and_lifecycle_shapes_are_invalid(self):
        baseline = copy.deepcopy(self.s3.config)
        cases = (
            ('encryption_outer', 'get_bucket_encryption', 'encryption',
             {'ServerSideEncryptionConfiguration': []}),
            ('encryption_default', 'get_bucket_encryption', 'encryption',
             {'ServerSideEncryptionConfiguration': {'Rules': [
                 {'ApplyServerSideEncryptionByDefault': PRIVATE}]}}),
            ('encryption_algorithm', 'get_bucket_encryption', 'encryption',
             {'ServerSideEncryptionConfiguration': {'Rules': [
                 {'ApplyServerSideEncryptionByDefault': {'SSEAlgorithm': PRIVATE}}]}}),
            ('rule_status', 'get_bucket_lifecycle_configuration', 'lifecycle',
             {'Rules': [{'Status': PRIVATE}]}),
            ('rule_filter', 'get_bucket_lifecycle_configuration', 'lifecycle',
             {'Rules': [{'Status': 'Enabled', 'Filter': PRIVATE}]}),
            ('rule_date', 'get_bucket_lifecycle_configuration', 'lifecycle',
             {'Rules': [{'Status': 'Enabled', 'Expiration': {'Date': '2030-01-01'}}]}),
            ('rule_days', 'get_bucket_lifecycle_configuration', 'lifecycle',
             {'Rules': [{'Status': 'Enabled', 'Expiration': {'Days': True}}]}),
        )
        for partition, method, name, response in cases:
            with self.subTest(partition=partition):
                self.s3.config = dict(baseline, **{method: response})
                self.s3.calls.clear()
                result = self.configuration()
                self.assertEqual(result[name]['state'], 'invalid')
                self.assertEqual(len(self.s3.calls), 4)
                self.assert_public(result)

    def test_lifecycle_overlap_filters_and_destructive_actions_are_projected_safely(self):
        prefixes = ('', PRIVATE+'/', BASE+'/', BASE+'/nested/', BASE+'-outside/')
        self.s3.config['get_bucket_lifecycle_configuration'] = {'Rules': [
            {'ID': PRIVATE, 'Status': 'Enabled', 'Filter': {'Prefix': prefix},
             'Expiration': {'Days': 7, 'ExpiredObjectDeleteMarker': False},
             'NoncurrentVersionExpiration': {'NoncurrentDays': 2, 'NewerNoncurrentVersions': 3},
             'Transitions': [{'Days': 1, 'StorageClass': 'GLACIER'}],
             'NoncurrentVersionTransitions': [{'NoncurrentDays': 1, 'StorageClass': 'GLACIER'}]}
            for prefix in prefixes] + [
                {'ID': PRIVATE, 'Status': 'Disabled',
                 'Filter': {'And': {'Prefix': BASE+'/', 'Tags': [
                     {'Key': PRIVATE, 'Value': PRIVATE}], 'ObjectSizeGreaterThan': 9}},
                 'Expiration': {'Date': datetime(2030, 1, 1, tzinfo=timezone.utc)}}]}
        result = self.configuration()
        rows = result['lifecycle']['rules']
        self.assertEqual([r['index'] for r in rows], list(range(6)))
        self.assertEqual([r['destination_relation'] for r in rows],
                         ['overlaps']*4+['outside', 'overlaps'])
        self.assertEqual([r['conditions_unassessed'] for r in rows], [False]*5+[True])
        self.assertEqual([r['enabled'] for r in rows], [True]*5+[False])
        for row in rows[:5]:
            self.assertEqual(row['expiration_days'], 7)
            self.assertIs(row['expired_object_delete_marker'], False)
            self.assertEqual(row['noncurrent_expiration_days'], 2)
            self.assertEqual(row['newer_noncurrent_versions'], 3)
            self.assertEqual(row['transition_count'], 1)
            self.assertEqual(row['noncurrent_transition_count'], 1)
        self.assertEqual(rows[-1]['expiration_date'], '2030-01-01T00:00:00+00:00')
        self.assert_public(result)
        root = self.configuration(base='')['lifecycle']['rules']
        self.assertTrue(all(row['destination_relation'] == 'overlaps' for row in root))

    def test_configuration_kms_matching_is_exact_not_alias_resolution(self):
        self.s3.config['get_bucket_encryption']['ServerSideEncryptionConfiguration']['Rules'][0][
            'ApplyServerSideEncryptionByDefault']['KMSMasterKeyID'] = 'alias/'+PRIVATE
        result = self.configuration()
        self.assertIs(result['encryption']['configured_kms_identity_matches_exactly'], False)
        self.assert_public(result)

    def test_lifecycle_tag_and_size_conditions_are_each_unassessed(self):
        filters = (
            {'Tag': {'Key': PRIVATE, 'Value': PRIVATE}},
            {'ObjectSizeGreaterThan': 9},
            {'ObjectSizeLessThan': 99},
            {'And': {'Prefix': BASE+'/', 'ObjectSizeGreaterThan': 9}},
        )
        self.s3.config['get_bucket_lifecycle_configuration'] = {'Rules': [
            {'Status': 'Enabled', 'Filter': value} for value in filters]}
        result = self.configuration()
        self.assertEqual(result['lifecycle']['state'], 'observed')
        self.assertEqual([row['conditions_unassessed'] for row in result['lifecycle']['rules']],
                         [True]*4)
        self.assertEqual([row['destination_relation'] for row in result['lifecycle']['rules']],
                         ['overlaps']*4)
        self.assert_public(result)

    def test_configuration_suspended_versioning_and_year_retention_are_not_invented(self):
        self.s3.config['get_bucket_versioning'] = {'Status': 'Suspended', 'MFADelete': 'Enabled'}
        self.s3.config['get_object_lock_configuration'] = {'ObjectLockConfiguration': {
            'ObjectLockEnabled': 'Enabled',
            'Rule': {'DefaultRetention': {'Mode': 'GOVERNANCE', 'Years': 2}}}}
        result = self.configuration()
        self.assertEqual(result['versioning']['state'], 'observed')
        self.assertIs(result['versioning']['enabled'], False)
        self.assertEqual(result['versioning']['mfa_delete'], 'Enabled')
        self.assertEqual(result['object_lock']['default_mode'], 'GOVERNANCE')
        self.assertEqual(result['object_lock']['default_years'], 2)
        self.assertNotIn('default_days', result['object_lock'])
        self.assert_public(result)

    def test_objects_pin_all_41_archives_and_receipt_without_payload_reads(self):
        result = self.objects()
        self.assertEqual(len(result), 42)
        self.assertEqual(len(self.s3.calls), 84)
        self.assertEqual({name for name, _ in self.s3.calls},
                         {'head_object', 'get_object_retention'})
        expected = set(self.s3.targets)
        for method in ('head_object', 'get_object_retention'):
            requests = [kwargs for name, kwargs in self.s3.calls if name == method]
            self.assertEqual({kwargs['Key'] for kwargs in requests}, expected)
            self.assertEqual(len(requests), 42)
        self.assertEqual(sum('/37715977350-1/' in key for key in expected), 8)
        self.assertEqual(sum('/40393597860-1/' in key for key in expected), 34)
        archives = {row['artifact_id']: row for row in result if 'artifact_id' in row}
        self.assertEqual(set(archives), {r['artifact_id'] for r in self.ledger['archives']})
        for source in self.ledger['archives']:
            row = archives[source['artifact_id']]
            self.assertEqual(row['version_id'], source['object']['version_id'])
            self.assertEqual(row['expected_sha256'], source['object']['sha256'])
            self.assertEqual(row['content_length'], source['object']['content_length'])
            self.assertEqual(row['retain_until'], source['object']['retain_until'])
            self.assertEqual(row['encryption'], 'aws:kms')
            self.assertEqual(row['lock_mode'], 'COMPLIANCE')
        receipt = [row for row in result if row.get('kind') == 'preservation_receipt']
        self.assertEqual(len(receipt), 1)
        self.assertEqual(receipt[0]['version_id'],
                         self.preservation['report']['receipt']['version_id'])
        self.assertEqual(receipt[0]['expected_sha256'],
                         self.preservation['report']['receipt']['sha256'])
        for row in result:
            self.assertEqual(row['state'], 'observed')
            for flag in ('version_matches', 'length_matches', 'kms_identity_matches',
                         'prior_lock_not_shortened', 'compliance_lock_current', 'metadata_verified'):
                self.assertIs(row[flag], True, flag)
            self.assertIs(row['payload_digest_verified'], False)
        self.assert_public(result)

    def test_object_verification_rejects_each_independent_metadata_mismatch(self):
        for partition, head, lock, field in (
                ('version', {'VersionId': 'other-version'}, {}, 'version_matches'),
                ('length', {'ContentLength': 1}, {}, 'length_matches'),
                ('kms', {'SSEKMSKeyId': 'alias/'+PRIVATE}, {}, 'kms_identity_matches'),
                ('governance', {}, {'Mode': 'GOVERNANCE'}, 'compliance_lock_current'),
                ('shortened', {}, {'RetainUntilDate': '2026-11-01T00:00:00+00:00'},
                 'prior_lock_not_shortened')):
            with self.subTest(partition=partition):
                self.s3.head_changes, self.s3.lock_changes = head, lock
                result = self.objects()
                self.assertTrue(all(row['state'] == 'observed' for row in result))
                self.assertTrue(all(row[field] is False and row['metadata_verified'] is False
                                    for row in result))
                self.assert_public(result)
        self.s3.head_changes = {'ServerSideEncryption': 'AES256'}
        self.s3.lock_changes = {}
        result = self.objects()
        self.assertTrue(all(row['kms_identity_matches'] is True and
                            row['encryption'] == 'AES256' and
                            row['metadata_verified'] is False for row in result))

    def test_object_lock_current_boundary_and_extension_are_exact(self):
        deadline = datetime(2030, 1, 1, tzinfo=timezone.utc)
        self.s3.lock_changes = {'RetainUntilDate': deadline}
        for seconds, current in ((-1, True), (0, False), (1, False)):
            with self.subTest(offset=seconds):
                result = self.objects(deadline+timedelta(seconds=seconds))
                self.assertTrue(all(row['compliance_lock_current'] is current for row in result))
                self.assertTrue(all(row['metadata_verified'] is current for row in result))
                self.assertTrue(all(row['prior_lock_not_shortened'] is True for row in result))

    def test_invalid_missing_versions_dates_and_lengths_are_not_verified(self):
        for partition, head, lock in (
                ('version_none', {'VersionId': None}, {}),
                ('version_null', {'VersionId': 'null'}, {}),
                ('version_empty', {'VersionId': ''}, {}),
                ('length_bool', {'ContentLength': True}, {}),
                ('date_missing', {}, {'RetainUntilDate': None}),
                ('date_naive_string', {}, {'RetainUntilDate': '2030-01-01T00:00:00'}),
                ('date_naive_object', {}, {'RetainUntilDate': datetime(2030, 1, 1)})):
            with self.subTest(partition=partition):
                self.s3.head_changes, self.s3.lock_changes = head, lock
                result = self.objects()
                self.assertEqual(len(result), 42)
                self.assertTrue(all(row['state'] == 'invalid' for row in result))
                self.assertTrue(all(row['metadata_verified'] is False for row in result))
                self.assert_public(result)

    def test_source_expiry_unknown_and_before_at_after_are_not_conflated(self):
        source = self.ledger['archives'][0]
        deadline = datetime.fromisoformat(source['object']['retain_until'])
        for partition, expiry, expected in (
                ('unknown', None, None),
                ('before', deadline-timedelta(seconds=1), True),
                ('equal', deadline, False),
                ('after', deadline+timedelta(seconds=1), False)):
            with self.subTest(partition=partition):
                source['actions_expires_at_from_frozen_inventory'] = (
                    expiry.isoformat() if expiry is not None else None)
                row = next(row for row in self.objects()
                           if row.get('artifact_id') == source['artifact_id'])
                self.assertIs(row['covers_frozen_actions_expiry'], expected)
                self.assertIs(row['metadata_verified'], True)

    def test_missing_object_metadata_fields_are_explicitly_invalid(self):
        for partition, head_missing, lock_missing in (
                ('version', {'VersionId'}, set()),
                ('length', {'ContentLength'}, set()),
                ('date', set(), {'RetainUntilDate'}),
                ('mode', set(), {'Mode'})):
            with self.subTest(partition=partition):
                self.s3.head_missing, self.s3.lock_missing = head_missing, lock_missing
                result = self.objects()
                self.assertEqual(len(result), 42)
                self.assertTrue(all(row['state'] == 'invalid' and
                                    row['metadata_verified'] is False for row in result))
                self.assert_public(result)

    def test_object_sdk_and_retention_denials_remain_safe_observations(self):
        for partition, method, error, code in (
                ('head_sdk', 'head_object', BotoCoreError(), None),
                ('retention_denied', 'get_object_retention', aws_error('AccessDenied'),
                 'AccessDenied'),
                ('retention_unknown', 'get_object_retention', aws_error(PRIVATE),
                 'UnrecognizedAwsError')):
            with self.subTest(partition=partition):
                self.s3.failures = {method: error}
                result = self.objects()
                self.assertEqual(len(result), 42)
                for row in result:
                    self.assertEqual(row['state'], 'unavailable')
                    self.assertIs(row['metadata_verified'], False)
                    self.assertEqual(row['error_class'], type(error).__name__)
                    if code is not None:
                        self.assertEqual(row['aws_error_code'], code)
                self.assert_public(result)

    def test_object_audit_rejects_naive_clock_before_reads(self):
        with self.assertRaises(ValueError):
            self.objects(datetime(2026, 10, 27))
        self.assertEqual(self.s3.calls, [])

    def test_one_denied_target_retains_safe_failure_and_continues_other_41(self):
        self.s3.failures[next(iter(self.s3.targets))] = aws_error('AccessDenied')
        result = self.objects()
        unavailable = [row for row in result if row['state'] == 'unavailable']
        self.assertEqual(len(unavailable), 1)
        self.assertEqual(unavailable[0]['error_class'], 'ClientError')
        self.assertEqual(unavailable[0]['aws_error_code'], 'AccessDenied')
        self.assertIs(unavailable[0]['metadata_verified'], False)
        self.assertEqual(sum(row['metadata_verified'] is True for row in result), 41)
        self.assert_public(result)

    def test_audit_combines_independent_observations_and_separates_coverage_failures(self):
        for partition in ('healthy', 'configuration_denied', 'object_denied',
                          'metadata_unverified', 'coverage_failed'):
            with self.subTest(partition=partition):
                s3 = MetadataOnlyS3(self.ledger, self.preservation)
                if partition == 'configuration_denied':
                    s3.failures['get_bucket_versioning'] = aws_error('AccessDenied')
                if partition == 'object_denied':
                    s3.failures[next(iter(s3.targets))] = aws_error('AccessDenied')
                if partition == 'metadata_unverified':
                    s3.lock_changes = {'RetainUntilDate': NOW}
                report = audit.initial_report(NOW)
                fetch = Mock(side_effect=AssertionError('Use injected bounded coverage collector'))
                with patch.object(coverage, 'collect', return_value={'findings': []},
                                  side_effect=ValueError(PRIVATE) if partition == 'coverage_failed'
                                  else None) as collect:
                    audit.audit(s3, f's3://{BUCKET}/{BASE}', KMS, report, NOW, fetch)
                collect.assert_called_once_with(fetch, NOW)
                self.assertIs(report['audit_complete'],
                              partition in ('healthy', 'metadata_unverified'))
                self.assertIs(report['coverage_collection_complete'],
                              partition != 'coverage_failed')
                self.assertEqual(len(report['objects']), 42)
                self.assertEqual(set(report['configuration']),
                                 {'versioning', 'object_lock', 'lifecycle', 'encryption'})
                if partition == 'coverage_failed':
                    finding = next(row for row in report['findings'] if 'coverage' in row['code'])
                    self.assertEqual(finding, {
                        'kind': 'workflow-receipts', 'code': 'workflow_coverage_unavailable',
                        'state': 'unknown', 'owner_issue': 2343,
                    })
                    self.assertEqual(report['coverage'], {
                        'state': 'unavailable', 'fetch_state': 'unavailable',
                        'error_class': 'ValueError',
                    })
                self.assertIs(report['object_metadata_verified'],
                              partition not in ('object_denied', 'metadata_unverified'))
                self.assert_safe_flags(report)
                self.assertIs(report['object_store_access'], True)
                self.assert_public(report)

    def test_fresh_extended_versions_supersede_dated_archive_findings_only(self):
        now = datetime(2026, 12, 1, tzinfo=timezone.utc)
        self.s3.lock_changes = {'RetainUntilDate': datetime(2030, 1, 1, tzinfo=timezone.utc)}
        report = audit.initial_report(now)
        with patch.object(coverage, 'collect_check',
                          return_value={'fetch_state': 'unavailable'}) as check:
            audit.audit(self.s3, f's3://{BUCKET}/{BASE}', KMS, report, now,
                        Mock(side_effect=AssertionError('No unbounded GitHub calls')))
        self.assertEqual(check.call_count, 2)
        self.assertIs(report['audit_complete'], True)
        self.assertIs(report['coverage_collection_complete'], True)
        self.assertIs(report['object_metadata_verified'], True)
        self.assertEqual(report['summary']['metadata_verified_count'], 42)
        unknown = {row['artifact_id'] for row in self.ledger['archives']
                   if not row['actions_expires_at_from_frozen_inventory']}
        for row in report['objects']:
            self.assertIs(row['covers_frozen_actions_expiry'],
                          None if row.get('artifact_id') in unknown else True)
        historical_findings = report['coverage']['historical_findings']
        expired = [row for row in historical_findings
                   if row['code'] == 'recorded_archive_lock_expired']
        self.assertEqual(len(expired), 41)
        historical = [row for row in historical_findings if row['code'] ==
                      'historical_archive_protection_beyond_actions_expiry_unproven']
        self.assertEqual(len(historical), 41)
        workflow_findings = [row for row in report['coverage']['findings']
                             if row.get('kind') in ('backup', 'restore')]
        self.assertTrue(workflow_findings)
        self.assertEqual([row for row in report['findings'] if 'artifact_id' not in row],
                         workflow_findings)
        archive_findings = [row for row in report['findings'] if 'artifact_id' in row]
        self.assertEqual({row['artifact_id'] for row in archive_findings}, unknown)
        self.assertTrue(all(row['code'] == 'protection_beyond_actions_expiry_unproven'
                            for row in archive_findings))
        self.assertEqual(report['original_private_receipt_self_version'], 'unrecorded_not_assessed')
        self.assertIs(report['retention_policy_adopted'], False)
        self.assert_safe_flags(report)
        self.assert_public(report)

    def test_fresh_live_metadata_supersedes_dated_receipt_expiry_at_and_after_april_horizon(self):
        horizon = datetime(2027, 4, 30, tzinfo=timezone.utc)
        live_deadline = datetime(2030, 1, 1, tzinfo=timezone.utc)
        self.s3.lock_changes = {'RetainUntilDate': live_deadline}
        failed_workflow = {
            'fetch_state': 'observed', 'latest_run_id': 9, 'latest_state': 'completed',
            'latest_conclusion': 'failure', 'receipt_verified': True,
            'retention_verified': True, 'receipt_checked_at': horizon.isoformat(),
            'retention_until': live_deadline.isoformat(),
        }
        for offset in (0, 1):
            with self.subTest(offset=offset):
                now = horizon+timedelta(seconds=offset)
                self.s3.calls.clear()
                report = audit.initial_report(now)
                with patch.object(coverage, 'collect_check', return_value=failed_workflow):
                    audit.audit(self.s3, f's3://{BUCKET}/{BASE}', KMS, report, now,
                                Mock(side_effect=AssertionError('No unbounded GitHub calls')))
                dated = report['coverage']['findings']
                self.assertEqual([row for row in dated
                                  if row.get('kind') == 'preservation_receipt'], [{
                    'kind': 'preservation_receipt',
                    'code': 'recorded_preservation_receipt_lock_expired',
                    'state': 'attention', 'owner_issue': 2343,
                }])
                self.assertEqual(sum(row['code'] == 'recorded_archive_lock_expired'
                                     for row in dated), 41)
                self.assertEqual(
                    {(row['kind'], row['code']) for row in report['findings']
                     if row.get('kind') in ('backup', 'restore')},
                    {('backup', 'latest_attempt_unsuccessful'),
                     ('restore', 'latest_attempt_unsuccessful')})
                self.assertFalse(any(row['code'] in (
                    'recorded_preservation_receipt_lock_expired', 'recorded_archive_lock_expired',
                    'historical_archive_protection_beyond_actions_expiry_unproven',
                    'recorded_retention_extension_unavailable') for row in report['findings']))
                self.assertIs(report['object_metadata_verified'], True)
                self.assertEqual(report['summary']['metadata_verified_count'], 42)
                self.assertEqual({row['retain_until'] for row in report['objects']},
                                 {live_deadline.isoformat()})
                self.assertEqual(len([row for row in report['objects']
                                      if row['kind'] == 'preservation_receipt']), 1)
                self.assertEqual(report['original_private_receipt_self_version'],
                                 'unrecorded_not_assessed')
                unknown = {row['artifact_id'] for row in self.ledger['archives']
                           if not row['actions_expires_at_from_frozen_inventory']}
                self.assertEqual(
                    {row['artifact_id'] for row in report['findings']
                     if row['code'] == 'protection_beyond_actions_expiry_unproven'}, unknown)
                self.assert_safe_flags(report)
                self.assert_public(report)

    def test_fresh_live_metadata_supersedes_unknown_extension_but_not_workflow_failures(self):
        now = datetime(2026, 12, 1, tzinfo=timezone.utc)
        self.s3.lock_changes = {'RetainUntilDate': datetime(2030, 1, 1, tzinfo=timezone.utc)}
        report = audit.initial_report(now)
        with patch.object(coverage, 'load_retention_extension', side_effect=OSError(PRIVATE)), \
             patch.object(coverage, 'collect_check', return_value={'fetch_state': 'unavailable'}):
            audit.audit(self.s3, f's3://{BUCKET}/{BASE}', KMS, report, now,
                        Mock(side_effect=AssertionError('No unbounded GitHub calls')))
        self.assertIn('recorded_retention_extension_unavailable',
                      {row['code'] for row in report['coverage']['findings']})
        self.assertNotIn('recorded_retention_extension_unavailable',
                         {row['code'] for row in report['findings']})
        expected = [row for row in report['coverage']['findings']
                    if row.get('kind') in ('backup', 'restore')]
        self.assertEqual({row['kind'] for row in expected}, {'backup', 'restore'})
        self.assertEqual([row for row in report['findings']
                          if row.get('kind') in ('backup', 'restore')], expected)
        self.assertIs(report['object_metadata_verified'], True)
        self.assertEqual(report['original_private_receipt_self_version'], 'unrecorded_not_assessed')
        self.assert_public(report)

    def test_configuration_negative_findings_distinguish_35_day_floor_from_observation(self):
        cases = (
            ('short', 'Enabled', {'Mode': 'COMPLIANCE', 'Days': 34}, False, True),
            ('floor', 'Enabled', {'Mode': 'COMPLIANCE', 'Days': 35}, False, False),
            ('above', 'Enabled', {'Mode': 'COMPLIANCE', 'Days': 36}, False, False),
            ('year', 'Enabled', {'Mode': 'COMPLIANCE', 'Years': 1}, False, False),
            ('governance', 'Enabled', {'Mode': 'GOVERNANCE', 'Days': 365}, False, True),
            ('suspended', 'Suspended', {'Mode': 'COMPLIANCE', 'Days': 35}, True, False),
        )
        for partition, status, default, versioning, floor in cases:
            with self.subTest(partition=partition):
                self.s3.config['get_bucket_versioning'] = {'Status': status}
                self.s3.config['get_object_lock_configuration'] = {'ObjectLockConfiguration': {
                    'ObjectLockEnabled': 'Enabled', 'Rule': {'DefaultRetention': default}}}
                report = audit.initial_report(NOW)
                with patch.object(coverage, 'collect', return_value={'findings': []}):
                    audit.audit(self.s3, f's3://{BUCKET}/{BASE}', KMS, report, NOW)
                codes = {row['code'] for row in report['findings']}
                self.assertEqual('bucket_versioning_not_enabled' in codes, versioning)
                self.assertEqual('default_compliance_floor_not_met' in codes, floor)
                self.assertIs(report['audit_complete'], True)
                self.assertIs(report['retention_policy_adopted'], False)
                self.assertTrue(all(row['owner_issue'] == 2343 for row in report['findings']))
                self.assert_safe_flags(report)
                self.assert_public(report)

    def test_coverage_api_failure_does_not_discard_successful_aws_observations(self):
        report = audit.initial_report(NOW)
        with patch.object(coverage, 'collect', side_effect=subprocess.CalledProcessError(
                1, ['gh'], stderr=PRIVATE)):
            audit.audit(self.s3, f's3://{BUCKET}/{BASE}', KMS, report, NOW)
        self.assertIs(report['audit_complete'], False)
        self.assertIs(report['coverage_collection_complete'], False)
        self.assertIs(report['object_metadata_verified'], True)
        self.assertEqual(len(report['objects']), 42)
        self.assertEqual(set(report['configuration']),
                         {'versioning', 'object_lock', 'lifecycle', 'encryption'})
        finding = next(row for row in report['findings'] if 'coverage' in row['code'])
        self.assertEqual(finding, {
            'kind': 'workflow-receipts', 'code': 'workflow_coverage_unavailable',
            'state': 'unknown', 'owner_issue': 2343,
        })
        self.assertEqual(report['coverage'], {
            'state': 'unavailable', 'fetch_state': 'unavailable',
            'error_class': 'CalledProcessError',
        })
        self.assert_safe_flags(report)
        self.assert_public(report)

    def test_audit_authenticates_both_local_evidence_files_before_any_aws_read(self):
        self.assertEqual(audit.PRESERVATION_SHA256,
                         '4e5b1c4590ec3325c6bebd39d9a5001d2ebf9a72a858aef8e6d1e94762901530')
        original_read = Path.read_bytes
        for partition, target in (('ledger', LEDGER), ('preservation', PRESERVATION)):
            with self.subTest(partition=partition):
                self.s3.calls.clear()
                report = audit.initial_report(NOW)
                def altered(path):
                    raw = original_read(path)
                    return raw+b'\n' if path.resolve() == target else raw
                with patch.object(Path, 'read_bytes', altered), \
                     patch.object(coverage, 'collect') as collect:
                    with self.assertRaises(ValueError):
                        audit.audit(self.s3, f's3://{BUCKET}/{BASE}', KMS, report, NOW)
                self.assertEqual(self.s3.calls, [])
                collect.assert_not_called()
                self.assertIs(report['audit_complete'], False)
                self.assertIs(report['object_store_access'], False)
                self.assert_safe_flags(report)

    def cli_environment(self):
        return {
            'GITHUB_REF': 'refs/heads/master', 'GITHUB_SHA': 'a'*40,
            'GITHUB_REPOSITORY': 'tomacox74/js2il',
            'GITHUB_RUN_ID': '123456', 'GITHUB_RUN_ATTEMPT': '2',
            'TEST262_BACKUP_S3_URI': f's3://{BUCKET}/{BASE}',
            'TEST262_BACKUP_KMS_KEY': KMS,
        }

    def test_cli_rejects_invalid_scope_config_and_overwrite_before_aws_reads(self):
        cases = (
            ('branch', {'GITHUB_REF': 'refs/heads/topic'}),
            ('tag', {'GITHUB_REF': 'refs/tags/master'}),
            ('repository_missing', {'GITHUB_REPOSITORY': ''}),
            ('repository_wrong', {'GITHUB_REPOSITORY': 'other/repository'}),
            ('sha_short', {'GITHUB_SHA': 'a'*39}),
            ('sha_long', {'GITHUB_SHA': 'a'*41}),
            ('sha_invalid', {'GITHUB_SHA': 'z'*40}),
            ('run_zero', {'GITHUB_RUN_ID': '0'}),
            ('run_negative', {'GITHUB_RUN_ID': '-1'}),
            ('attempt_zero', {'GITHUB_RUN_ATTEMPT': '0'}),
            ('attempt_invalid', {'GITHUB_RUN_ATTEMPT': '2x'}),
            ('uri_missing', {'TEST262_BACKUP_S3_URI': ''}),
            ('uri_invalid', {'TEST262_BACKUP_S3_URI': 'https://'+PRIVATE}),
            ('kms_missing', {'TEST262_BACKUP_KMS_KEY': ''}),
            ('overwrite', {}),
        )
        with tempfile.TemporaryDirectory() as directory:
            for partition, changes in cases:
                with self.subTest(partition=partition):
                    path = Path(directory)/(partition+'.json')
                    if partition == 'overwrite':
                        path.write_bytes(b'accepted existing evidence')
                    output = io.StringIO()
                    with patch.dict(os.environ, dict(self.cli_environment(), **changes), clear=True), \
                         patch('boto3.client') as client, patch.object(audit, 'audit') as run, \
                         redirect_stdout(output), redirect_stderr(output):
                        try:
                            status = audit.main(['--output', str(path)])
                        except SystemExit as error:
                            status = error.code
                    self.assertNotEqual(status, 0)
                    client.assert_not_called()
                    run.assert_not_called()
                    if partition == 'overwrite':
                        self.assertEqual(path.read_bytes(), b'accepted existing evidence')
                    else:
                        report = json.loads(path.read_bytes())
                        self.assertIs(report['audit_complete'], False)
                        self.assert_safe_flags(report)
                        self.assert_public(report)
                    self.assertNotIn(PRIVATE, output.getvalue())

    def test_cli_requires_output_and_never_overwrites_its_source(self):
        with redirect_stderr(io.StringIO()), patch('boto3.client') as client:
            with self.assertRaises(SystemExit):
                audit.main([])
            client.assert_not_called()
        source = Path(audit.__file__)
        raw = source.read_bytes()
        with patch.dict(os.environ, self.cli_environment(), clear=True), \
             patch('boto3.client') as client, redirect_stdout(io.StringIO()), \
             redirect_stderr(io.StringIO()):
            try:
                status = audit.main(['--output', str(source)])
            except SystemExit as error:
                status = error.code
        self.assertNotEqual(status, 0)
        self.assertEqual(source.read_bytes(), raw)
        client.assert_not_called()

    def test_cli_disables_sdk_retries_and_persists_safe_partial_reports(self):
        for partition, error in (('success', None), ('denied', aws_error('AccessDenied')),
                                 ('unknown', aws_error(PRIVATE)), ('sdk', BotoCoreError()),
                                 ('input', ValueError(PRIVATE)), ('incomplete', None),
                                 ('metadata_unverified', None)):
            with self.subTest(partition=partition), tempfile.TemporaryDirectory() as directory:
                path = Path(directory)/'audit.json'
                output = io.StringIO()
                def run(s3, uri, kms, report, **kwargs):
                    report['stage'] = 'metadata'
                    report['configuration'] = {'partial': {'state': 'observed'}}
                    if error is not None:
                        raise error
                    report['audit_complete'] = partition in ('success', 'metadata_unverified')
                    report['object_metadata_verified'] = partition == 'success'
                with patch.dict(os.environ, self.cli_environment(), clear=True), \
                     patch('boto3.client', return_value=self.s3) as client, \
                     patch('botocore.config.Config') as config, \
                     patch.object(audit, 'audit', side_effect=run), \
                     redirect_stdout(output), redirect_stderr(output):
                    status = audit.main(['--output', str(path)])
                self.assertEqual(status, 0 if partition == 'success' else 1)
                config.assert_called_once_with(retries={'total_max_attempts': 1})
                client.assert_called_once_with('s3', config=config.return_value)
                report = json.loads(path.read_bytes())
                self.assertEqual(report['configuration'], {'partial': {'state': 'observed'}})
                self.assertEqual(report['run_id'], 123456)
                self.assertEqual(report['run_attempt'], 2)
                self.assertEqual(report['source_revision'], 'a'*40)
                self.assert_safe_flags(report)
                self.assert_public(report)
                self.assertNotIn(PRIVATE, output.getvalue())
                if partition in ('denied', 'unknown'):
                    self.assertEqual(report['aws_error_code'],
                                     'AccessDenied' if partition == 'denied'
                                     else 'UnrecognizedAwsError')

    def test_workflow_audit_step_is_protected_operation_scoped_and_read_only(self):
        workflow = (ROOT/'.github/workflows/test262-cutover-evidence.yml').read_text()
        self.assertIn("if: github.ref == 'refs/heads/master'", workflow)
        self.assertIn('environment: test262-catalogue-backup', workflow)
        self.assertRegex(workflow, r'options:\s*(?:.*\n)*?\s+- audit-retention')
        blocks = re.split(r'(?m)^      - ', workflow)
        audit_step = next(block for block in blocks
                          if 'python -m scripts.test262.central.retention_audit' in block)
        self.assertIn("if: inputs.operation == 'audit-retention'", audit_step)
        self.assertIn('GH_TOKEN: ${{ github.token }}', audit_step)
        for variable in ('TEST262_BACKUP_S3_URI', 'TEST262_BACKUP_KMS_KEY'):
            self.assertIn(variable+': ${{ vars.'+variable+' }}', audit_step)
        self.assertIn('--output /tmp/test262-cutover-report/retention-audit-report.json',
                      audit_step)
        self.assertNotIn('secrets.', audit_step)
        for unsafe in ('central.monitor', 'put-object', 'get-object ', 'kms decrypt',
                       'central.recovery_preservation', 'central.cutover_evidence'):
            self.assertNotIn(unsafe, audit_step)
        for module, operation in (
                ('cutover_evidence', 'preserve'),
                ('recovery_preservation', 'preserve-recovery'),
                ('retained_evidence', 'verify-retained'),
                ('retained_discovery', 'discover-retained')):
            step = next(block for block in blocks
                        if 'python -m scripts.test262.central.'+module in block)
            self.assertIn("if: inputs.operation == '"+operation+"'", step)
        credentials = next(block for block in blocks if 'configure-aws-credentials@' in block)
        match = re.search(r"inputs.operation == 'audit-retention'\s*&&\s*'([^']+)'",
                          credentials)
        self.assertIsNotNone(match, 'Audit must select its own intersecting session policy')
        policy = json.loads(match.group(1))
        actions = {action for row in policy['Statement'] for action in row['Action']}
        self.assertEqual(actions, {
            's3:GetBucketVersioning', 's3:GetBucketObjectLockConfiguration',
            's3:GetLifecycleConfiguration', 's3:GetEncryptionConfiguration',
            's3:GetObjectVersion', 's3:GetObjectRetention',
        })
        self.assertTrue(all(row['Effect'] == 'Allow' for row in policy['Statement']))
        upload = next(block for block in blocks if 'actions/upload-artifact@' in block)
        self.assertIn('if: always()', upload)
        self.assertIn('path: /tmp/test262-cutover-report/*report.json', upload)
        validation = next(block for block in blocks if 'Validate reviewed operation' in block)
        self.assertRegex(validation, r'(?:[| ])audit-retention(?:[|)])')


if __name__ == '__main__':
    unittest.main()
