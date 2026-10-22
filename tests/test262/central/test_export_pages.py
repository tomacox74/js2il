from contextlib import contextmanager
import hashlib
import json
from pathlib import Path
import tempfile
import unittest

from scripts.test262.central.client import Client, EXPORT_TABLES


class PagedDatabase:
    def __init__(self):
        self.requests = []
        self.in_snapshot = False
        self.tables = {'fixtures': [{'index': i} for i in range(2501)],
                       'observation_payloads': [{'index': i} for i in range(61)]}

    def execute(self, query, values=None):
        if query.startswith('SELECT txid_current_snapshot()'):
            from datetime import datetime, timezone
            result = ('1:2:', datetime(2026, 10, 22, tzinfo=timezone.utc))
        else:
            epoch, table, filters, after, page = values
            self.requests.append((table, page, self.in_snapshot))
            limit = min(page, 25) if table == 'observation_payloads' else page
            start = after['index'] + 1 if after else 0
            rows = self.tables.get(table, [])[start:start+limit]
            cursor = {'index': rows[-1]['index']} if len(rows) == limit else None
            result = ({'rows': rows, 'next': cursor},)

        class Result:
            def fetchone(self):
                return result
        return Result()


class ExportPageTests(unittest.TestCase):
    def client(self):
        client = Client.__new__(Client)
        client.db = PagedDatabase()
        client.epoch = 2
        client.contract = {'repository_id': 'repository'}
        client.Jsonb = lambda value: value

        @contextmanager
        def view():
            client.db.in_snapshot = True
            try:
                yield client
            finally:
                client.db.in_snapshot = False
        client.view = view
        return client

    def test_export_uses_maximum_pages_with_complete_snapshot_and_digest(self):
        client = self.client()
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / 'export.ndjson'
            receipt = client.export(path, seconds=30)
            lines = path.read_bytes().splitlines(keepends=True)
        records = [json.loads(line) for line in lines]
        self.assertEqual(receipt['counts']['fixtures'], 2501)
        self.assertEqual(receipt['counts']['observation_payloads'], 61)
        self.assertEqual(receipt['counts']['fixture_specification_metadata'], 0)
        self.assertEqual(set(receipt['counts']), set(EXPORT_TABLES))
        self.assertEqual(records[-1]['footer']['sha256'], hashlib.sha256(b''.join(lines[:-1])).hexdigest())
        self.assertEqual(records[0]['header']['snapshot_token'], '1:2:')
        self.assertEqual([r['row']['index'] for r in records if r.get('table') == 'fixtures'], list(range(2501)))
        self.assertEqual([r['row']['index'] for r in records if r.get('table') == 'observation_payloads'], list(range(61)))
        self.assertTrue(all(size == 1000 and in_snapshot for _, size, in_snapshot in client.db.requests))
        self.assertEqual(sum(table == 'fixtures' for table, _, _ in client.db.requests), 3)
        # Continuation must use the server cursor even when payload pages are capped at 25.
        self.assertEqual(sum(table == 'observation_payloads' for table, _, _ in client.db.requests), 3)

    def test_ordinary_reads_keep_their_existing_default_page_size(self):
        client = self.client()
        self.assertEqual(len(list(client.read('fixtures'))), 2501)
        self.assertTrue(all(size == 500 for _, size, _ in client.db.requests))
        self.assertEqual(len(client.db.requests), 6)


if __name__ == '__main__':
    unittest.main()
