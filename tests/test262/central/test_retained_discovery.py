from datetime import datetime, timedelta, timezone
import json
from pathlib import Path
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import Mock, patch

from scripts.test262.central import retained_discovery as discovery


class RetainedDiscoveryTests(unittest.TestCase):
    def setUp(self):
        self.report = discovery.initial_report(discovery.INVENTORY.read_bytes())
        self.aid = self.report['archives'][0]['artifact_id']
        self.size = self.report['archives'][0]['expected_size_in_bytes']
        self.key = f'PRIVATE/cutover-evidence/100-1/{self.aid}.zip'
        self.version = {'Key': self.key, 'VersionId': 'opaque-version', 'Size': self.size}
        self.s3 = Mock()
        self.s3.list_object_versions.return_value = {'Versions': [self.version], 'IsTruncated': False}
        self.s3.head_object.return_value = {
            'VersionId': 'opaque-version', 'ContentLength': self.size,
            'ServerSideEncryption': 'aws:kms', 'SSEKMSKeyId': 'PRIVATE-kms'}
        self.s3.get_object_retention.return_value = {'Retention': {
            'Mode': 'COMPLIANCE', 'RetainUntilDate': datetime.now(timezone.utc)+timedelta(days=10)}}

    def run_discovery(self):
        discovery.discover(self.s3, 's3://PRIVATE-bucket/PRIVATE', 'PRIVATE-kms', self.report)
        return self.report['archives'][0]

    def test_scoped_candidate_checks_exact_version_without_payload_or_private_fields(self):
        row = self.run_discovery()
        candidate = row['candidates'][0]
        self.assertEqual(row['state'], 'candidate_versions_require_payload_verification')
        self.assertEqual(candidate['version_id'], 'opaque-version')
        self.assertTrue(candidate['metadata_candidate_qualifies'])
        self.assertFalse(candidate['payload_digest_verified'])
        self.assertEqual(candidate['preservation_run_id'], 100)
        self.assertNotIn('sha256', candidate)
        self.assertTrue(self.report['discovery_complete'])
        self.assertFalse(self.report['full_archive_coverage_verified'])
        self.assertEqual(self.s3.list_object_versions.call_args.kwargs,
                         {'Bucket': 'PRIVATE-bucket', 'Prefix': 'PRIVATE/cutover-evidence/', 'MaxKeys': 100})
        self.assertEqual(self.s3.head_object.call_args.kwargs['VersionId'], 'opaque-version')
        self.assertEqual(self.s3.get_object_retention.call_args.kwargs['VersionId'], 'opaque-version')
        self.assertEqual({call[0] for call in self.s3.mock_calls},
                         {'list_object_versions', 'head_object', 'get_object_retention'})
        self.assertNotIn('PRIVATE', json.dumps(self.report))
        self.assertNotIn('s3://', json.dumps(self.report))

    def test_pagination_keeps_multiple_versions_and_delete_markers_are_not_candidates(self):
        second = dict(self.version, VersionId='older')
        self.s3.list_object_versions.side_effect = [
            {'Versions': [self.version], 'DeleteMarkers': [{'Key': self.key, 'VersionId': 'deleted'}],
             'IsTruncated': True, 'NextKeyMarker': self.key, 'NextVersionIdMarker': 'deleted'},
            {'Versions': [second], 'IsTruncated': False}]
        self.s3.head_object.side_effect = lambda **kw: dict(
            self.s3.head_object.return_value, VersionId=kw['VersionId'])
        row = self.run_discovery()
        self.assertEqual([c['version_id'] for c in row['candidates']], ['opaque-version', 'older'])
        self.assertEqual(self.report['delete_markers_scanned'], 1)
        self.assertEqual(self.s3.list_object_versions.call_args.kwargs['VersionIdMarker'], 'deleted')
        self.assertEqual(self.report['entries_scanned'], 3)

    def test_empty_or_out_of_layout_scope_is_not_claimed_lost_or_retained(self):
        self.s3.list_object_versions.return_value = {
            'Versions': [dict(self.version, Key='PRIVATE/cutover-evidence/other-name.zip')],
            'IsTruncated': False}
        self.run_discovery()
        self.assertTrue(self.report['listing_complete'])
        self.assertTrue(all(r['state'] == 'no_candidate_in_scoped_listing' for r in self.report['archives']))
        self.assertEqual(self.report['out_of_layout_entries'], 1)
        self.s3.head_object.assert_not_called()
        self.assertEqual(len(self.report['archives']), 33)

    def test_unprotected_wrong_length_or_kms_is_visible_and_never_verified(self):
        for field, value, expected in (
                ('ContentLength', self.size+1, 'length_matches_inventory'),
                ('SSEKMSKeyId', 'wrong-kms', 'configured_kms_matches'),
                ('ServerSideEncryption', 'AES256', 'configured_kms_matches')):
            with self.subTest(field=field):
                self.setUp()
                self.s3.head_object.return_value[field] = value
                if field == 'ContentLength':
                    # A consistent listing/head length can still differ from the required archive.
                    self.version['Size'] = value
                candidate = self.run_discovery()['candidates'][0]
                self.assertFalse(candidate[expected])
                self.assertFalse(candidate['metadata_candidate_qualifies'])
                self.assertFalse(candidate['payload_digest_verified'])
        self.setUp()
        self.s3.get_object_retention.return_value['Retention']['RetainUntilDate'] = (
            datetime.now(timezone.utc)-timedelta(seconds=1))
        self.assertFalse(self.run_discovery()['candidates'][0]['current_compliance_lock'])

    def test_truncated_repeating_or_invalid_pages_fail_without_absence_claims(self):
        for page in (
                {'Versions': [], 'IsTruncated': True},
                {'Versions': [dict(self.version, Key='outside/scope.zip')], 'IsTruncated': False},
                {'Versions': [dict(self.version, VersionId='null')], 'IsTruncated': False},
                {'Versions': [self.version, self.version], 'IsTruncated': False},
                {'Versions': [self.version]}):
            with self.subTest(page=page):
                self.setUp()
                self.s3.list_object_versions.return_value = page
                with self.assertRaises(ValueError):
                    self.run_discovery()
                self.assertFalse(self.report['discovery_complete'])
                self.assertTrue(all(r['state'] == 'not_assessed' for r in self.report['archives']))
        self.setUp()
        self.s3.list_object_versions.return_value = {
            'Versions': [], 'IsTruncated': True, 'NextKeyMarker': self.key, 'NextVersionIdMarker': 'same'}
        with self.assertRaisesRegex(ValueError, 'continuation'):
            self.run_discovery()
        self.assertEqual(self.s3.list_object_versions.call_count, 2)
        self.setUp()
        with patch.object(discovery, 'MAX_PAGES', 1):
            self.s3.list_object_versions.return_value = {
                'Versions': [self.version], 'IsTruncated': True,
                'NextKeyMarker': self.key, 'NextVersionIdMarker': 'next'}
            with self.assertRaisesRegex(ValueError, 'budget'):
                self.run_discovery()
        self.assertFalse(self.report['listing_complete'])
        self.s3.head_object.assert_not_called()

    def test_page_and_candidate_limits_do_not_silently_truncate_or_succeed(self):
        self.s3.list_object_versions.return_value = {
            'Versions': [self.version]*101, 'IsTruncated': False}
        with self.assertRaisesRegex(ValueError, 'page exceeds'):
            self.run_discovery()
        self.assertFalse(self.report['discovery_complete'])
        self.s3.head_object.assert_not_called()
        self.setUp()
        self.s3.list_object_versions.return_value = {
            'Versions': [self.version, dict(self.version, VersionId='second')], 'IsTruncated': False}
        with patch.object(discovery, 'MAX_CANDIDATES', 1), self.assertRaisesRegex(ValueError, 'bound'):
            self.run_discovery()
        self.assertFalse(self.report['listing_complete'])
        self.assertTrue(all(a['state'] == 'not_assessed' for a in self.report['archives']))
        self.s3.head_object.assert_not_called()

    def test_metadata_errors_are_sanitized_preserved_and_fail_the_operation(self):
        error = RuntimeError('PRIVATE connection details')
        error.response = {'Error': {'Code': 'AccessDenied', 'Message': 'PRIVATE'}}
        self.s3.head_object.side_effect = error
        with self.assertRaises(ValueError):
            self.run_discovery()
        row = self.report['archives'][0]
        self.assertEqual(row['state'], 'candidate_metadata_error')
        self.assertEqual(row['candidates'][0]['aws_error_code'], 'AccessDenied')
        self.assertFalse(self.report['discovery_complete'])
        self.assertTrue(self.report['listing_complete'])
        self.assertNotIn('PRIVATE', json.dumps(self.report))

    def test_cli_denial_persists_failure_without_success_fallback_or_overwrite(self):
        error = RuntimeError('PRIVATE access denial')
        error.response = {'Error': {'Code': 'AccessDenied'}}
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory)/'report.json'
            with patch.dict('os.environ', {'TEST262_BACKUP_S3_URI': 's3://PRIVATE/PRIVATE',
                                          'TEST262_BACKUP_KMS_KEY': 'PRIVATE',
                                          'GITHUB_SHA': 'a'*40}), \
                 patch.dict('sys.modules', {'boto3': SimpleNamespace(client=Mock(return_value=self.s3))}):
                self.s3.list_object_versions.side_effect = error
                self.assertEqual(discovery.main(['--output', str(path)]), 1)
            report = json.loads(path.read_bytes())
            self.assertEqual(report['aws_error_code'], 'AccessDenied')
            self.assertFalse(report['listing_complete'])
            self.assertTrue(all(a['state'] == 'not_assessed' for a in report['archives']))
            self.assertNotIn('PRIVATE', json.dumps(report))
            with self.assertRaises(ValueError):
                discovery.main(['--output', str(path)])

    def test_inventory_drift_fails_before_any_object_store_call(self):
        raw = json.loads(discovery.INVENTORY.read_bytes())
        raw['original_history']['manifest']['gaps'].pop()
        with self.assertRaises(ValueError):
            discovery.initial_report(json.dumps(raw).encode())
        self.assertFalse(self.s3.mock_calls)


if __name__ == '__main__':
    unittest.main()
