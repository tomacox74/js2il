import copy
from datetime import datetime, timedelta, timezone
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

from scripts.test262.central import retained_readbacks as readbacks

EVIDENCE = readbacks.ROOT/'docs/ECMA262/evidence/retained-readbacks-20261026.json'


class RetainedReadbackTests(unittest.TestCase):
    def setUp(self):
        self.baseline = json.loads(readbacks.BASELINE.read_bytes())
        lengths = {a['artifact_id']: a.get('size_in_bytes') for a in self.baseline['archives']}
        prior = self.baseline['reused_readback']
        start = datetime(2026, 10, 26, 1, tzinfo=timezone.utc)
        self.rows = []
        for index, original in enumerate(self.baseline['retained_objects']):
            aid = original['artifact_id']
            if aid == prior['object']['artifact_id']:
                continue
            report = copy.deepcopy(prior['report'])
            report['object'] = dict(original, content_length=lengths[aid], encryption='aws:kms',
                                    lock_mode='COMPLIANCE', retention_verified=True)
            created = start+timedelta(minutes=index*3)
            report['checked_at'] = (created+timedelta(minutes=1)).isoformat()
            self.rows.append({
                'requested_artifact_id': aid, 'run_id': 100+index, 'run_attempt': 1,
                'source_revision': readbacks.REVISION, 'created_at': created.isoformat(),
                'completed_at': (created+timedelta(minutes=2)).isoformat(),
                'run_conclusion': 'success', 'operation': 'verify-retained', 'job_id': 200+index,
                'execution_guards': dict(readbacks.GUARDS),
                'actions_artifact': {'artifact_id': 300+index, 'size_in_bytes': 1800,
                                     'zip_sha256': 'a'*64, 'expires_at': '2027-01-24T00:00:00Z'},
                'report_sha256': readbacks.sha(readbacks.encoded(report)), 'report': report,
            })

    def assemble(self, rows):
        with tempfile.TemporaryDirectory() as directory:
            paths = []
            for index, row in enumerate(rows):
                path = Path(directory)/f'readback-{index}.json'
                path.write_bytes(readbacks.encoded(row))
                paths.append(path)
            return readbacks.assemble(paths)

    def test_complete_set_reuses_eighth_without_extending_historical_coverage(self):
        batch = self.assemble(self.rows)
        readbacks.validate(batch)
        self.assertEqual(batch['summary'], {
            'new_readbacks': 7, 'reused_readbacks': 1, 'verified_selected_objects': 8,
            'required_containers': 41, 'required_containers_without_identified_retained_object': 33,
            'original_archive_gaps': 11})
        self.assertEqual(batch['prior_readback'], self.baseline['reused_readback'])
        self.assertFalse(batch['history_complete'])
        self.assertFalse(batch['full_archive_coverage_verified'])

    def test_missing_duplicate_or_replaced_object_is_rejected(self):
        for rows in (self.rows[:-1], self.rows+[self.rows[0]],
                     [self.rows[0], self.rows[0], *self.rows[2:]]):
            with self.subTest(rows=len(rows)), self.assertRaises(ValueError):
                self.assemble(rows)

    def test_success_requires_read_only_run_source_and_skipped_preservation(self):
        for change in (
                lambda r: r.update(source_revision='0'*40),
                lambda r: r.update(operation='preserve'),
                lambda r: r.update(run_conclusion='failure'),
                lambda r: r['execution_guards'].update(
                    {'Verify reports/outboxes and retain original evidence in encrypted S3': 'success'}),
                lambda r: r.update(run_attempt=2)):
            rows = copy.deepcopy(self.rows)
            change(rows[0])
            with self.subTest(change=change), self.assertRaises(ValueError):
                self.assemble(rows)

    def test_identity_length_digest_and_timing_cannot_be_changed_even_with_new_report_hash(self):
        for change in (
                lambda r: r['report']['object'].update(version_id='different-version'),
                lambda r: r['report']['object'].update(sha256='b'*64),
                lambda r: r['report']['object'].update(content_length=1),
                lambda r: r['report']['object'].update(retain_until='2026-10-25T00:00:00Z'),
                lambda r: r['report']['object'].update(retention_verified=False),
                lambda r: r['report'].update(checked_at='2026-10-25T00:00:00Z'),
                lambda r: r['report'].update(history_complete=True),
                lambda r: r['report'].update(history_complete=0),
                lambda r: r['report'].update(source_metadata_verified=1),
                lambda r: r['report'].update(full_archive_coverage_verified=True)):
            rows = copy.deepcopy(self.rows)
            change(rows[0])
            rows[0]['report_sha256'] = readbacks.sha(readbacks.encoded(rows[0]['report']))
            with self.subTest(change=change), self.assertRaises(ValueError):
                self.assemble(rows)

    def test_private_fields_and_uncanonical_report_bytes_are_rejected(self):
        for change in (
                lambda r: r.update(bucket='PRIVATE'),
                lambda r: r['actions_artifact'].update(key='PRIVATE'),
                lambda r: r['report'].update(permission_audit='PRIVATE'),
                lambda r: r['report']['object'].update(kms_key='PRIVATE'),
                lambda r: r.update(report_sha256='0'*64)):
            rows = copy.deepcopy(self.rows)
            change(rows[0])
            if rows[0]['report_sha256'] != '0'*64:
                rows[0]['report_sha256'] = readbacks.sha(readbacks.encoded(rows[0]['report']))
            with self.subTest(change=change), self.assertRaises(ValueError):
                self.assemble(rows)

    def test_concurrent_runs_or_modified_reused_evidence_are_rejected(self):
        batch = self.assemble(self.rows)
        batch['readbacks'][1]['created_at'] = batch['readbacks'][0]['created_at']
        with self.assertRaises(ValueError):
            readbacks.validate(batch)
        batch = self.assemble(self.rows)
        batch['prior_readback']['object']['content_length'] = 1
        with self.assertRaises(ValueError):
            readbacks.validate(batch)

    def test_failed_assembly_never_writes_success_and_does_not_overwrite_evidence(self):
        with tempfile.TemporaryDirectory() as directory:
            output = Path(directory)/'new.json'
            with patch.object(readbacks, 'assemble', side_effect=ValueError('partial')), \
                 self.assertRaises(ValueError):
                readbacks.main(['--reports', 'unused.json', '--output', str(output)])
            self.assertFalse(output.exists())
            output.write_text('existing evidence')
            with self.assertRaises(ValueError):
                readbacks.main(['--reports', 'unused.json', '--output', str(output)])
            self.assertEqual(output.read_text(), 'existing evidence')

    def test_durable_actual_batch_is_complete_and_contains_no_private_identifiers(self):
        batch = json.loads(EVIDENCE.read_bytes())
        readbacks.validate(batch)
        forbidden = {'bucket', 'prefix', 'key', 'kms_key', 'SSEKMSKeyId', 'repository_id',
                     'producer_id', 'import_id', 'database_url', 'permission_audit', 'project_id'}
        def inspect(value):
            if isinstance(value, dict):
                self.assertFalse(forbidden & value.keys())
                for child in value.values():
                    inspect(child)
            elif isinstance(value, list):
                for child in value:
                    inspect(child)
        inspect(batch)
        raw = json.dumps(batch)
        self.assertNotIn('s3://', raw)
        self.assertNotRegex(raw, r'[0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12}')
        with patch.object(readbacks, 'assemble', side_effect=AssertionError('unexpected collection')):
            readbacks.main(['--verify', str(EVIDENCE)])


if __name__ == '__main__':
    unittest.main()
