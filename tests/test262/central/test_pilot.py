import unittest
from unittest.mock import Mock
from scripts.test262.central.pilot import disposable_settings, PilotClient, LostAcknowledgement


class PilotSafetyTests(unittest.TestCase):
    def test_refuses_remote_or_performance_connection_before_connecting(self):
        for dsn in ('host=production.example dbname=catalogue_pilot user=postgres',
                    'host=127.0.0.1 hostaddr=10.0.0.1 dbname=catalogue_pilot user=postgres',
                    'host=127.0.0.1 dbname=postgres user=postgres',
                    'host=127.0.0.1 dbname=catalogue_pilot user=test262_supervisor'):
            with self.subTest(dsn=dsn), self.assertRaises(ValueError):
                disposable_settings(dsn)
        self.assertEqual(disposable_settings('host=127.0.0.1 dbname=catalogue_pilot user=postgres')['dbname'], 'catalogue_pilot')

    def test_lost_ack_occurs_after_commit_and_only_once(self):
        underlying = Mock()
        underlying.call.return_value = {'acknowledged': True}
        client = PilotClient(underlying, lose_ack=True)
        with self.assertRaises(LostAcknowledgement):
            client.call('ingest', 'stable-request', [{'observation_id': 'stable-observation'}])
        underlying.call.assert_called_once_with('ingest', 'stable-request', [{'observation_id': 'stable-observation'}])
        self.assertEqual(client.call('ingest', 'stable-request', [{'observation_id': 'stable-observation'}]), {'acknowledged': True})
        self.assertEqual(underlying.call.call_count, 2)

    def test_only_first_claim_waits_for_second_worker(self):
        underlying = Mock()
        underlying.call.side_effect = [{'work_item_id': 'first'}, {'work_item_id': 'second'}]
        barrier = Mock()
        client = PilotClient(underlying, barrier)
        client.call('claim', 'run', 'budget', 120000, 150)
        client.call('claim', 'run', 'budget', 120000, 150)
        barrier.wait.assert_called_once_with(timeout=60)
        self.assertEqual(client.first_lease, {'work_item_id': 'first'})
