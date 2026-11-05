from datetime import datetime, timezone
import unittest
from unittest.mock import Mock

from scripts.test262.central.client import sha
from scripts.test262.central.collection_status import markdown, snapshot


class CollectionStatusTests(unittest.TestCase):
    def test_snapshot_scopes_backlog_and_distinguishes_expiry_and_unknown_throughput(self):
        context = dict(run='owner', budget='budget', provenance='provenance', kind='native',
                       revision='a' * 40, actions_run_id=123, actions_attempt=1, work_count=3)
        member = dict(slot=1, producer_id='worker', run_id='child', actions_job_id=456,
                      run_state='running')
        client = Mock()
        client.call.return_value = dict(context=context, context_sha256='\\x' + sha(context),
                                        workers=[member], phase='open')

        def read(table, filters):
            if table == 'budget_work_items':
                self.assertEqual(filters, {'budget_scope_id': 'budget'})
                return [dict(work_item_id=id) for id in ('first', 'second', 'missing')]
            if table == 'work_items':
                self.assertEqual(filters, {'provenance_id': 'provenance'})
                return [dict(work_item_id='first', state='leased', lease_owner='worker',
                             lease_expires_at='2026-11-05T01:00:00Z'),
                        dict(work_item_id='second', state='completed'),
                        dict(work_item_id='foreign-allocation', state='pending')]
            self.assertEqual((table, filters), ('run_metrics', {'run_id': 'child'}))
            return []

        client.read.side_effect = read
        client.one.return_value = dict(state='open')
        result = snapshot(client, context, datetime(2026, 11, 5, 2, tzinfo=timezone.utc))
        self.assertEqual(result['counts'], dict(pending=0, leased=1, completed=1,
                                                deferred=0, cancelled=0))
        self.assertEqual((result['expired_leases'], result['missing_work']), (1, 1))
        self.assertEqual(result['workers'][0]['expired_leases'], 1)
        self.assertIsNone(result['workers'][0]['attempts_per_minute'])
        self.assertIn('unknown', markdown(result))
        client.call.assert_called_once_with('collection', 'owner')
        client.put.assert_not_called()

    def test_changed_context_is_rejected_before_reading_work(self):
        client = Mock()
        client.call.return_value = dict(context={'run': 'other'}, context_sha256='\\x' + 'a' * 64)
        with self.assertRaisesRegex(ValueError, 'registered collection'):
            snapshot(client, {'run': 'owner'})
        client.read.assert_not_called()
