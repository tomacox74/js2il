import unittest
from types import SimpleNamespace
from unittest.mock import Mock

from scripts.test262.central.pilot_drills import run_drills, expect_rejection


class QueueDrillTests(unittest.TestCase):
    def test_refuses_other_connections_before_any_mutation(self):
        for host, database, user in [('remote.example', 'catalogue_pilot', 'catalogue_pilot_worker'),
                                     ('127.0.0.1', 'postgres', 'catalogue_pilot_worker'),
                                     ('127.0.0.1', 'catalogue_pilot', 'postgres')]:
            client = Mock()
            client.db.info = SimpleNamespace(host=host, dbname=database, user=user)
            with self.assertRaises(ValueError):
                run_drills(client, None, None, [], None)
            client.put.assert_not_called()
            client.call.assert_not_called()

    def test_requires_expected_sqlstate_not_any_error(self):
        import psycopg
        client = Mock()
        client.call.side_effect = psycopg.errors.InsufficientPrivilege('wrong rejection')
        with self.assertRaises(ValueError):
            expect_rejection(client, 'ingest', (), 'P0001')
        client.call.side_effect = psycopg.errors.RaiseException('expected conflict')
        self.assertEqual(expect_rejection(client, 'ingest', (), 'P0001'), 'P0001')
        client.call.side_effect = None
        with self.assertRaises(ValueError):
            expect_rejection(client, 'ingest', (), 'P0001')
