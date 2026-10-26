import copy
import io
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import Mock, patch
import zipfile

from scripts.test262.central import archive_inventory as inventory

ROOT = Path(__file__).resolve().parents[3]
LEDGER = ROOT/'docs/ECMA262/evidence/historical-archive-inventory-2242.json'


class ArchiveInventoryTests(unittest.TestCase):
    def setUp(self):
        self.ledger = json.loads(LEDGER.read_bytes())

    def test_frozen_scope_and_every_original_source_are_accounted_for(self):
        inventory.validate(self.ledger)
        self.assertEqual(self.ledger['summary'], {
            'workflow_runs': 104, 'original_sources': 62, 'original_archive_gaps': 11,
            'required_archives': 41, 'required_archives_with_retained_object': 8,
            'required_archives_without_identified_retained_object': 33,
            'import_recovery_archives': 36, 'incomplete_import_recovery_archives': 4,
            'outbox_databases': 67, 'outbox_wal_shm_members': 4,
            'verified_source_entries_in_reports': 63, 'unique_verified_sources_in_reports': 62,
            'planning_only_archives': 43, 'runs_without_artifacts': 24,
            'unsuccessful_runs_without_artifacts': 3})
        self.assertEqual(len(self.ledger['issue_referenced_artifact_ids']), 33)
        self.assertFalse(self.ledger['history_complete'])
        self.assertFalse(self.ledger['full_archive_coverage_verified'])

    def test_omissions_duplicates_changed_gaps_and_false_retention_are_rejected(self):
        for mutate in (
                lambda r: r['original_history']['manifest']['artifacts'].pop(),
                lambda r: r['original_history']['manifest']['gaps'].pop(),
                lambda r: r['archives'].pop(),
                lambda r: r['workflow_runs'].pop(),
                lambda r: r['archives'].append(r['archives'][0]),
                lambda r: r['retained_objects'][0].update(version_id='not-the-original-version'),
                lambda r: r.update(history_complete=True),
                lambda r: r['reused_readback']['object'].update(content_length=1),
                lambda r: r['reused_readback']['report'].update(selected_object_verified=False),
                lambda r: r['archives'][0].update(retained_object_artifact_id=None)):
            report = copy.deepcopy(self.ledger)
            mutate(report)
            with self.subTest(mutate=mutate), self.assertRaises((ValueError, KeyError)):
                inventory.validate(report)

    def test_failed_and_cancelled_outboxes_keep_their_wal_shm_and_are_not_deduplicated(self):
        failed = {a['artifact_id']: a for a in self.ledger['archives']
                  if 'import_report' in a and not a['import_report']['complete']}
        self.assertEqual(set(failed), {11296419607, 11305023882, 11316103861, 11386604548})
        self.assertTrue(all(a['required_for_scope'] and a['retained_object_artifact_id'] is None
                            for a in failed.values()))
        for aid in (11296419607, 11305023882):
            names = {m['name'] for m in failed[aid]['members']}
            self.assertTrue({'11290148901-outbox.sqlite', '11290148901-outbox.sqlite-wal',
                             '11290148901-outbox.sqlite-shm'} <= names)
            broken = copy.deepcopy(self.ledger)
            archive = next(a for a in broken['archives'] if a['artifact_id'] == aid)
            archive['members'] = [m for m in archive['members'] if not m['name'].endswith('-wal')]
            with self.assertRaises(ValueError):
                inventory.validate(broken)

    def test_missing_verified_source_reference_cannot_be_hidden_by_recomputed_totals(self):
        report = copy.deepcopy(self.ledger)
        recovery = next(a for a in report['archives'] if a['artifact_id'] == 11515534653)
        recovery['import_report']['snapshots'][0]['state'] = 'uploading'
        recovery['import_report']['complete'] = False
        report['summary'] = inventory.summary(report)
        with self.assertRaisesRegex(ValueError, 'no verified report'):
            inventory.validate(report)

    def test_noop_plans_and_no_artifact_runs_are_explicit_not_new_original_gaps(self):
        plans = [a for a in self.ledger['archives'] if a['classification'] == 'planning_only']
        self.assertEqual(len(plans), 43)
        self.assertEqual(sum(a['auto_plan']['state'] == 'blocked' for a in plans), 40)
        self.assertTrue(all(not a['required_for_scope'] and 'import_report' not in a for a in plans))
        absent = {r['run_id']: r for r in self.ledger['workflow_runs']
                  if not r['artifact_ids'] and r['conclusion'] != 'skipped'}
        self.assertEqual(set(absent), {37219426911, 37364118548, 37426152050})
        self.assertEqual(absent[37364118548]['job_evidence'][0]['recorded_steps'], 0)

    def test_report_projection_validates_pins_and_drops_private_payloads(self):
        source = self.ledger['original_history']['manifest']['artifacts'][0]
        history = self.ledger['original_history']['manifest']
        snapshot = {'artifact_id': source['artifact_id'], 'state': 'verified',
                    'sha256': source['databases'][0]['sha256'],
                    'source_uri': source['databases'][0]['source_uri'],
                    'archive_sha256': source['archive_sha256'],
                    'verified_observation_mappings': 1,
                    'result': {'observations': 1, 'import_id': 'PRIVATE'},
                    'project': 'PRIVATE', 'permission_audit': 'PRIVATE'}
        report = {'history_complete': False, 'complete': True, 'gaps': history['gaps'],
                  'manifest_sha256': 'a'*64, 'selected_artifact_ids': [source['artifact_id']],
                  'snapshots': [snapshot], 'database_url': 'PRIVATE'}
        result = inventory.project_import(report, history, 'a'*64)
        self.assertNotIn('PRIVATE', json.dumps(result))
        snapshot['archive_sha256'] = 'b'*64
        with self.assertRaises(ValueError):
            inventory.project_import(report, history, 'a'*64)
        self.assertEqual(inventory.project_plan({'state': 'blocked', 'failed_run_id': '37426152050',
                                                'private_key': 'PRIVATE'}),
                         {'state': 'blocked', 'failed_run_id': '37426152050'})

    def test_public_ledger_contains_no_private_identities_or_storage_locations(self):
        forbidden = {'bucket', 'prefix', 'key', 'kms_key', 'SSEKMSKeyId', 'repository_id',
                     'producer_id', 'import_id', 'database_url', 'permission_audit', 'project_id'}
        def inspect(value):
            if isinstance(value, dict):
                self.assertFalse(forbidden & value.keys())
                for item in value.values():
                    inspect(item)
            elif isinstance(value, list):
                for item in value:
                    inspect(item)
        inspect(self.ledger)
        raw = json.dumps(self.ledger)
        self.assertNotIn('s3://', raw)
        self.assertNotRegex(raw, r'[0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12}')

    def test_absolute_ranges_are_cached_bounded_and_reject_ignored_requests(self):
        data = b'x'*(inventory.ArtifactRanges.BLOCK*2+100)
        fetch = Mock(side_effect=lambda start, end: data[start:end+1])
        reader = inventory.ArtifactRanges(1, len(data), fetch)
        reader.seek(-22, 2)
        self.assertEqual(reader.read(22), b'x'*22)
        reader.seek(-64, 2)
        reader.read(32)
        self.assertEqual(fetch.call_count, 1)
        self.assertLess(reader.downloaded, len(data))
        for offset, whence in ((-1, 0), (len(data)+1, 0), (0, 3)):
            with self.assertRaises(ValueError):
                reader.seek(offset, whence)
        bad = inventory.ArtifactRanges(1, len(data), lambda start, end: data)
        bad.seek(-22, 2)
        with self.assertRaises(ValueError):
            bad.read(22)
        limited = inventory.ArtifactRanges(1, len(data), fetch)
        limited.BUDGET = 1
        with self.assertRaises(ValueError):
            limited.read(1)

    def test_zip64_index_can_be_read_without_fetching_large_payload(self):
        # zipfile emits the same ZIP64 entry structures for small synthetic inputs.
        stream = io.BytesIO()
        with patch.object(zipfile, 'ZIP64_LIMIT', 100):
            with zipfile.ZipFile(stream, 'w', zipfile.ZIP_DEFLATED) as archive:
                archive.writestr('11290148901-outbox.sqlite', b'x'*1000)
        raw = stream.getvalue()
        reader = inventory.ArtifactRanges(1, len(raw), lambda start, end: raw[start:end+1])
        with zipfile.ZipFile(reader) as archive:
            self.assertEqual(inventory.members(archive)[0]['name'], '11290148901-outbox.sqlite')
        for name in ('../import-report.json', '/import-report.json', 'private/identity.json'):
            stream = io.BytesIO()
            with zipfile.ZipFile(stream, 'w') as archive:
                archive.writestr(name, '{}')
            with zipfile.ZipFile(io.BytesIO(stream.getvalue())) as archive, self.assertRaises(ValueError):
                inventory.members(archive)

    def test_pagination_refuses_changed_or_over_budget_counts(self):
        fetch = Mock(side_effect=[{'total_count': 101, 'rows': list(range(100))},
                                  {'total_count': 101, 'rows': [100]}])
        self.assertEqual(len(inventory.paged('path?created=window', 'rows', fetch, 200)), 101)
        self.assertIn('&per_page=100&page=2', fetch.call_args.args[0])
        for pages in ([{'total_count': 201, 'rows': []}],
                      [{'total_count': 101, 'rows': list(range(100))},
                       {'total_count': 100, 'rows': []}]):
            with self.assertRaises(ValueError):
                inventory.paged('path', 'rows', Mock(side_effect=pages), 200)

    def test_offline_verification_has_no_network_and_collection_cannot_overwrite_evidence(self):
        with patch.object(inventory, 'github', side_effect=AssertionError('network')), \
             patch.object(inventory, 'collect', side_effect=AssertionError('network')):
            inventory.main(['--verify', str(LEDGER)])
            with self.assertRaises(ValueError):
                inventory.main(['--collect', '--output', str(LEDGER)])
        with tempfile.TemporaryDirectory() as directory:
            target = Path(directory)/'failed.json'
            with patch.object(inventory, 'collect', side_effect=ValueError('incomplete evidence')), \
                 self.assertRaises(ValueError):
                inventory.main(['--collect', '--output', str(target)])
            self.assertFalse(target.exists())


if __name__ == '__main__':
    unittest.main()
