import unittest
from unittest.mock import Mock

from scripts.test262.central.pilot_process import validate_connection, PauseBeforeUpload


class ProcessKillTests(unittest.TestCase):
    def test_rejects_remote_redirect_admin_and_wrong_database(self):
        for dsn in ('host=remote.example dbname=catalogue_pilot user=catalogue_pilot_worker',
                    'host=127.0.0.1 hostaddr=10.0.0.1 dbname=catalogue_pilot user=catalogue_pilot_worker',
                    'host=127.0.0.1 dbname=postgres user=catalogue_pilot_worker',
                    'host=127.0.0.1 dbname=catalogue_pilot user=postgres'):
            with self.assertRaises(ValueError):
                validate_connection(dsn)
        validate_connection('host=127.0.0.1 dbname=catalogue_pilot user=catalogue_pilot_worker')

    def test_pause_signals_stable_identity_before_any_upload(self):
        client, sender, gate = Mock(), Mock(), Mock()
        gate.wait.return_value = False
        with self.assertRaises(TimeoutError):
            PauseBeforeUpload(client, sender, gate).call('ingest', 'request', [{'observation_id': 'stable'}])
        sender.send.assert_called_once_with({'stage': 'durable-before-upload', 'observation_id': 'stable'})
        client.call.assert_not_called()
        gate.wait.assert_called_once_with(timeout=300)
