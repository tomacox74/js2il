import json
from datetime import datetime, timezone
from pathlib import Path
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import patch

from scripts.test262.central import backup


class BackupSnapshotTests(unittest.TestCase):
    def test_snapshot_keeps_all_counts_and_mvcc_identity_with_local_bounded_budget(self):
        queries = []

        class Database:
            def __enter__(self):
                return self

            def __exit__(self, *args):
                pass

            def execute(self, query):
                queries.append(query)
                if query.startswith('SET '):
                    return self
                if query == 'SELECT pg_export_snapshot(), transaction_timestamp()':
                    return SimpleNamespace(fetchone=lambda: ('snapshot-identity', datetime(2026, 10, 25, tzinfo=timezone.utc)))
                if 'to_jsonb' in query:
                    return SimpleNamespace(fetchone=lambda: ({'minimum_writer_epoch': 2},))
                if 'FROM pg_tables' in query:
                    return [('fixtures',), ('fixture_specification_metadata',)]
                if 'information_schema.views' in query:
                    return [('v_historical_candidates',), ('v_fixture_specification_references',)]
                counts = {'test262."fixtures"': 53432, 'test262."fixture_specification_metadata"': 0,
                          'test262_reporting."v_historical_candidates"': 42,
                          'test262_reporting."v_fixture_specification_references"': 53432}
                return SimpleNamespace(fetchone=lambda: (counts[query.removeprefix('SELECT count(*) FROM ')],))

        with tempfile.TemporaryDirectory() as directory, \
                patch.dict('os.environ', TEST262_BACKUP_DATABASE_URL='private-dedicated-dsn'), \
                patch.dict('sys.modules', psycopg=SimpleNamespace(connect=lambda dsn: Database())), \
                patch.object(backup, 'libpq_environment', return_value={}), \
                patch.object(backup, 'run') as dump:
            dump.side_effect = lambda command, env: Path(command[-1]).write_bytes(b'complete dump')
            progress = []
            archive, manifest_path = backup.create_snapshot(SimpleNamespace(output=directory), progress.append)
            manifest = json.loads(manifest_path.read_text())
            self.assertEqual(queries[:3], ['SET TRANSACTION ISOLATION LEVEL REPEATABLE READ READ ONLY',
                                          "SET LOCAL statement_timeout='10min'",
                                          'SELECT pg_export_snapshot(), transaction_timestamp()'])
            self.assertEqual(manifest['snapshot_started_at'], '2026-10-25T00:00:00+00:00')
            self.assertIn('--snapshot=snapshot-identity', dump.call_args.args[0])
            self.assertEqual(manifest['row_counts'], {'fixtures': 53432, 'fixture_specification_metadata': 0})
            self.assertEqual(manifest['view_counts'], {'v_historical_candidates': 42,
                                                     'v_fixture_specification_references': 53432})
            self.assertFalse(manifest['contains_performance_data'])
            self.assertEqual(archive.read_bytes(), b'complete dump')
            self.assertEqual(progress[-1], 'snapshot.view-count.v_fixture_specification_references')


if __name__ == '__main__':
    unittest.main()
