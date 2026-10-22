import copy
from datetime import datetime, timezone, timedelta
import json
from pathlib import Path
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import patch

from scripts.test262.central.client import sha
from scripts.test262.central.specification_backfill import (
    AnnotationWriter, CanaryPause, annotations, recovery_receipt, retained_fingerprints, run,
)


class SpecificationBackfillTests(unittest.TestCase):
    def test_writer_rejects_unreviewed_bytes_scope_tables_and_oversize_batches(self):
        row = {'fixture_id': 'f', 'metadata': {'es6id': 1e-7}}
        writes = []
        client = SimpleNamespace(put=lambda table, rows: writes.append((table, rows)))
        writer = AnnotationWriter(client, {'f': sha(row, database_numbers=True)}, pause=True)
        for table, rows in [('fixtures', [row]), ('fixture_specification_metadata', []),
                            ('fixture_specification_metadata', [dict(row, fixture_id='foreign')]),
                            ('fixture_specification_metadata', [dict(row, metadata={'es6id': '1e-7'})]),
                            ('fixture_specification_metadata', [row] * 251)]:
            with self.assertRaises(ValueError):
                writer.put(table, rows)
        self.assertEqual(writes, [])
        with self.assertRaises(CanaryPause):
            writer.put('fixture_specification_metadata', [row])
        self.assertEqual(len(writes), 1)  # Pause happens after an acknowledged batch.

    def test_history_checks_ignore_new_observations_but_detect_changed_deleted_and_inventory_rows(self):
        before = {'rows': {'fixtures': {'f': 'one'}, 'corpora': {'c': 'two'},
                           'observations': {'o': 'three'}, 'observation_payloads': {'o': 'four'}}}
        after = copy.deepcopy(before)
        after['rows']['observations']['new'] = 'new'
        self.assertEqual(retained_fingerprints(before, after)['observations']['historical_count'], 1)
        for table in before['rows']:
            for action in ('delete', 'modify'):
                broken = copy.deepcopy(after)
                key = next(iter(before['rows'][table]))
                if action == 'delete':
                    del broken['rows'][table][key]
                else:
                    broken['rows'][table][key] = 'changed'
                with self.assertRaises(ValueError):
                    retained_fingerprints(before, broken)
        after['rows']['fixtures']['other'] = 'changed'
        with self.assertRaises(ValueError):
            retained_fingerprints(before, after)

    def test_annotations_require_identical_payload_and_recorded_provenance(self):
        payload = {'fixture_id': 'f', 'metadata': {'es6id': 1e-7}}
        row = dict(payload, actor='coordinator', recorded_at='now')
        expected = {'f': sha(payload, database_numbers=True)}
        client = SimpleNamespace(read=lambda *args, **kwargs: iter([row]))
        self.assertEqual(annotations(client, expected), {'f': sha(row)})
        row['actor'] = ''
        with self.assertRaises(ValueError):
            annotations(client, expected)
        row['actor'] = 'coordinator'
        row['metadata'] = {'es6id': '1e-7'}
        with self.assertRaises(ValueError):
            annotations(client, expected)

    def test_recovery_requires_recent_retained_catalogue_only_matching_run(self):
        report = {'backup_verified': True, 'bucket_retention_verified': True,
                  'contains_performance_data': False, 'stage': 'complete',
                  'checked_at': datetime.now(timezone.utc).isoformat(), 'prefix': '2026/10/22/123-1',
                  'receipt': {'version_id': 'retained', 'sha256': 'a' * 64},
                  'objects': {'catalogue.dump': {}, 'manifest.json': {}}}
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / 'receipt.json'
            path.write_text(json.dumps(report))
            self.assertEqual(recovery_receipt(path, 123)['run_id'], 123)
            for change in ({'backup_verified': False}, {'contains_performance_data': True},
                           {'bucket_retention_verified': False}, {'prefix': '2026/10/22/124-1'},
                           {'checked_at': (datetime.now(timezone.utc)-timedelta(hours=7)).isoformat()}):
                path.write_text(json.dumps(dict(report, **change)))
                with self.assertRaises(ValueError):
                    recovery_receipt(path, 123)

    def test_preflight_never_calls_enrichment(self):
        class Client:
            def view(self):
                from contextlib import nullcontext
                return nullcontext()
        baseline = {'snapshot_token': 'snapshot', 'as_of': 'now',
                    'rows': {'fixtures': {'f': 'source'}, 'corpora': {'c': 'corpus'},
                             'observations': {}, 'observation_payloads': {},
                             'fixture_specification_metadata': {}}}
        args = SimpleNamespace(corpus='c', root='root', mode='preflight')
        with tempfile.TemporaryDirectory() as directory, \
                patch('scripts.test262.central.specification_backfill.prepare', return_value=({'f': 'digest'}, {})), \
                patch('scripts.test262.central.specification_backfill.snapshot', return_value=baseline), \
                patch('scripts.test262.central.specification_backfill.annotations', return_value={}), \
                patch('scripts.test262.central.specification_backfill.enrich_specification_metadata') as enrich:
            args.output = str(Path(directory) / 'report.json')
            report = {}
            run(Client(), args, report)
            enrich.assert_not_called()
            self.assertTrue(report['verified'])
            self.assertFalse(report['annotations_written'])

    def test_apply_resumes_acknowledged_batch_and_replays_without_rewriting_provenance(self):
        from contextlib import nullcontext
        before = {'snapshot_token': 'before', 'as_of': 'earlier',
                  'rows': {'fixtures': {'f': 'source'}, 'corpora': {'c': 'corpus'},
                           'observations': {'o': 'old'}, 'observation_payloads': {'o': 'payload'},
                           'fixture_specification_metadata': {}}}
        after = copy.deepcopy(before)
        after['snapshot_token'] = 'after'
        after['rows']['fixture_specification_metadata'] = {'f': 'immutable actor/time'}
        after['rows']['observations']['new'] = 'concurrent ingestion'
        with tempfile.TemporaryDirectory() as directory, \
                patch('scripts.test262.central.specification_backfill.prepare', return_value=({'f': 'digest'}, {})), \
                patch('scripts.test262.central.specification_backfill.snapshot', side_effect=[before, after]), \
                patch('scripts.test262.central.specification_backfill.annotations', side_effect=[
                    {}, {'f': 'immutable actor/time'}, {'f': 'immutable actor/time'}, {'f': 'immutable actor/time'}]), \
                patch('scripts.test262.central.specification_backfill.enrich_specification_metadata', side_effect=[
                    CanaryPause(), {'enriched_fixtures': 1}, {'enriched_fixtures': 1}]) as enrich:
            args = SimpleNamespace(corpus='c', root='root', mode='apply',
                                   output=str(Path(directory) / 'receipt.json'))
            report = {}
            run(SimpleNamespace(view=lambda: nullcontext()), args, report)
            self.assertEqual(enrich.call_count, 3)
            self.assertTrue(report['verified'])
            self.assertTrue(report['identical_replay'])
            self.assertEqual(report['interrupted_batch_annotations'], 1)
            self.assertEqual(report['newly_annotated_fixtures'], 1)
            self.assertEqual(report['historical_fingerprints']['observations']['historical_count'], 1)


if __name__ == '__main__':
    unittest.main()
