import unittest
from scripts.test262.central.pilot import disposable_settings


class PilotSafetyTests(unittest.TestCase):
    def test_refuses_remote_or_performance_connection_before_connecting(self):
        for dsn in ('host=production.example dbname=catalogue_pilot user=postgres',
                    'host=127.0.0.1 hostaddr=10.0.0.1 dbname=catalogue_pilot user=postgres',
                    'host=127.0.0.1 dbname=postgres user=postgres',
                    'host=127.0.0.1 dbname=catalogue_pilot user=test262_supervisor'):
            with self.subTest(dsn=dsn), self.assertRaises(ValueError):
                disposable_settings(dsn)
        self.assertEqual(disposable_settings('host=127.0.0.1 dbname=catalogue_pilot user=postgres')['dbname'], 'catalogue_pilot')
