import unittest
from unittest.mock import Mock

from scripts.test262.central.client import Client


class Cursor:
    def __init__(self, count):
        self.rows = iter(({'id': i},) for i in range(count))
        self.sizes = []
        self.closed = False

    def __enter__(self):
        return self

    def __exit__(self, *error):
        self.closed = True

    def fetchmany(self, size):
        from itertools import islice
        self.sizes.append(size)
        return list(islice(self.rows, size))


class BulkExportTests(unittest.TestCase):
    def client(self, count):
        client = Client.__new__(Client)
        client.epoch = 2
        client.contract = {'bulk_export_version': 1, 'permission': 'coordinator', 'trust_class': 'trusted'}
        cursor = Cursor(count)
        client.db = Mock()
        client.db.execute.return_value.fetchone.return_value = ('owned_portal',)
        client.db.cursor.return_value = cursor
        return client, cursor

    def test_complete_bounded_batches_without_duplicates(self):
        for table, size, count in (('fixtures', 1000, 2105), ('observation_payloads', 25, 52)):
            with self.subTest(table=table):
                client, cursor = self.client(count)
                rows = list(client.export_rows(table))
                self.assertEqual([row['id'] for row in rows], list(range(count)))
                self.assertEqual(cursor.sizes, [size] * 4)
                self.assertTrue(cursor.closed)
                client.db.execute.assert_called_once_with('SELECT test262.api_export_open(%s,%s)', (2, table))
                client.db.cursor.assert_called_once_with(name='owned_portal')

    def test_early_close_releases_portal_without_prefetching_the_table(self):
        client, cursor = self.client(10000)
        rows = client.export_rows('fixtures')
        self.assertEqual(next(rows), {'id': 0})
        self.assertEqual(cursor.sizes, [1000])
        rows.close()
        self.assertTrue(cursor.closed)
        self.assertEqual(cursor.sizes, [1000])

    def test_older_contract_and_nontrusted_logins_keep_existing_reads(self):
        for contract in ({}, {'bulk_export_version': 1, 'permission': 'ingest', 'trust_class': 'trusted'},
                         {'bulk_export_version': 1, 'permission': 'coordinator', 'trust_class': 'legacy'}):
            client, cursor = self.client(1)
            client.contract = contract
            client.read = Mock(return_value=iter([{'old': True}]))
            self.assertEqual(list(client.export_rows('fixtures')), [{'old': True}])
            client.read.assert_called_once_with('fixtures', page=1000)
            client.db.execute.assert_not_called()

    def test_advertised_endpoint_errors_are_not_hidden_by_fallback(self):
        client, cursor = self.client(1)
        client.db.execute.side_effect = PermissionError('denied')
        client.read = Mock()
        with self.assertRaises(PermissionError):
            list(client.export_rows('fixtures'))
        client.read.assert_not_called()


if __name__ == '__main__':
    unittest.main()
