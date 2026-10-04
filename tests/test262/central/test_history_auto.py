import unittest
from unittest.mock import patch
from scripts.test262.central.history_auto import failed_run, pending_rows


class HistoryAutoTests(unittest.TestCase):
    def row(self, uri='one'):
        return {'source_uri': uri, 'sha256': 'a', 'archive_sha256': 'b'}

    def test_only_hash_verified_receipts_skip_sources(self):
        rows = [self.row(), self.row('two')]
        # Imported row counts alone are not verification receipts.
        self.assertEqual(pending_rows(rows, []), rows)
        receipt = {'document': dict(rows[0], observations=0)}
        self.assertEqual(pending_rows(rows, [receipt]), [rows[1]])
        with self.assertRaisesRegex(ValueError, 'changed'):
            pending_rows([dict(rows[0], sha256='changed')], [receipt])
        with self.assertRaisesRegex(ValueError, 'Conflicting'):
            pending_rows(rows, [receipt, {'document': dict(rows[0], observations=2)}])

    def test_failure_latch_survives_many_successful_noops(self):
        success = {'id': 1, 'status': 'completed', 'conclusion': 'success'}
        failure = {'id': 2, 'status': 'completed', 'conclusion': 'cancelled'}
        with patch('scripts.test262.central.history_auto.gh_json', side_effect=[
                {'workflow_runs': [success]*100}, {'workflow_runs': [failure]}]) as gh:
            self.assertEqual(failed_run('owner/repo', '3'), '2')
            self.assertIn('page=2', gh.call_args.args[0])

    def test_current_run_is_not_mistaken_for_a_failure(self):
        current = {'id': 3, 'status': 'in_progress', 'conclusion': None}
        with patch('scripts.test262.central.history_auto.gh_json', return_value={'workflow_runs': [current]}):
            self.assertIsNone(failed_run('owner/repo', '3'))
