import unittest
from unittest.mock import patch

from scripts.test262.central.pilot_restore import docker_tools, run_restore_drill


class RestorePilotTests(unittest.TestCase):
    def test_rejects_remote_before_database_or_dump(self):
        with patch('psycopg.connect') as connect, patch('subprocess.run') as run:
            with self.assertRaises(ValueError):
                run_restore_drill('host=production.example dbname=catalogue_pilot user=postgres', '/tmp/pilot', {})
            connect.assert_not_called()
            run.assert_not_called()

    def test_client_tools_receive_password_only_in_environment(self):
        environment = {'PGHOST':'127.0.0.1','PGPASSWORD':'disposable-secret',
                       'PGDATABASE':'catalogue_pilot','GH_TOKEN':'must-not-enter-container'}
        with patch('subprocess.run') as run:
            docker_tools('/tmp/pilot')(['pg_dump','--file','/tmp/pilot/catalogue.dump'], environment)
        argv = run.call_args.args[0]
        self.assertNotIn('disposable-secret', ' '.join(argv))
        self.assertNotIn('GH_TOKEN', argv)
        self.assertIn('PGPASSWORD', argv)
        self.assertEqual(run.call_args.kwargs['env'], environment)
        with patch('subprocess.run') as run:
            with self.assertRaises(ValueError):
                docker_tools('/tmp/pilot')(['unexpected-command'], environment)
            run.assert_not_called()
