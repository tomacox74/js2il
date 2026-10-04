import json
import sqlite3
import unittest
from types import SimpleNamespace
from unittest.mock import MagicMock, patch

from scripts.test262.central.client import bytea, identity
from scripts.test262.central.inventory import inventory_digest, register
from scripts.test262.central.importer import import_mvp


class ImportRetryTests(unittest.TestCase):
    def inventory(self):
        return [{'path': 'test/x.js', 'sha256': 'a'*64, 'metadata': {}, 'metadata_state': 'unresolved',
                 'is_support_file': False, 'feature_tags': [], 'dependency_manifest_digest': None,
                 'dependencies': [], 'variants': [{'variant': 'default', 'required': True}]}]

    def sealed(self, rows, state='sealed'):
        return {'inventory_state': state, 'upstream_url': 'upstream', 'revision': 'a'*40,
                'revision_algorithm': 'sha1', 'inventory_digest': bytea(inventory_digest(rows)),
                'expected_fixture_count': len(rows)}

    def test_sealed_reuse_checks_identity_and_performs_no_writes(self):
        rows = self.inventory()
        client = MagicMock(); client.one.return_value = self.sealed(rows)
        corpus, ids = register(client, 'repo', {'cloneUrl': 'upstream', 'commit': 'a'*40}, rows, reuse_sealed=True)
        self.assertEqual(corpus, identity('upstream', 'a'*40, inventory_digest(rows)))
        self.assertIn('test/x.js', ids)
        client.put.assert_not_called(); client.call.assert_not_called()
        client.one.assert_called_once_with('corpora', corpus_id=corpus)

    def test_sealed_mismatch_fails_closed_and_staging_is_not_skipped(self):
        for change in ({'inventory_digest': bytea('b'*64)}, {'expected_fixture_count': 99}):
            client = MagicMock(); client.one.return_value = dict(self.sealed(self.inventory()), **change)
            with self.assertRaises(ValueError):
                register(client, 'repo', {'cloneUrl': 'upstream', 'commit': 'a'*40}, self.inventory(), reuse_sealed=True)
            client.put.assert_not_called()
        client = MagicMock(); client.one.return_value = self.sealed(self.inventory(), state='staging')
        register(client, 'repo', {'cloneUrl': 'upstream', 'commit': 'a'*40}, self.inventory(), reuse_sealed=True)
        self.assertIn('fixture_variants', [c.args[0] for c in client.put.call_args_list])
        client.call.assert_called_once()

    def snapshot(self):
        db = sqlite3.connect(':memory:'); db.row_factory = sqlite3.Row
        db.executescript('''CREATE TABLE settings(key,value); CREATE TABLE provenance(id,document);
            CREATE TABLE fixtures(provenance,path,variants,state,reasons);
            CREATE TABLE results(provenance,path,variant,document,finished,verdict);''')
        db.execute('INSERT INTO provenance VALUES(?,?)', ('p', json.dumps({'upstream': {'commit': 'a'*40}})))
        db.execute('INSERT INTO fixtures VALUES(?,?,?,?,?)', ('p', 'test/x.js', '["default"]', 'runnable', '[]'))
        db.executemany('INSERT INTO results VALUES(?,?,?,?,?,?)',
                      [('p', 'test/x.js', str(i), '{}', 1, 'matched') for i in range(201)])
        self.addCleanup(db.close)
        return db

    def run_mvp(self, client, outbox):
        with patch('scripts.test262.central.importer.normalize_fixture', return_value={}), \
             patch('scripts.test262.central.importer.register', return_value=('corpus', {'test/x.js': 'fixture'})), \
             patch('scripts.test262.central.importer.provenance', return_value='pid'), \
             patch('scripts.test262.central.importer.start_run', return_value='run'), \
             patch('scripts.test262.central.importer.observation', side_effect=lambda *a: {'observation_id': a[4]['variant']}):
            return import_mvp(client, self.snapshot(), SimpleNamespace(repository='repo'), 'import', outbox)

    def test_mapping_batches_and_checkpoint_only_after_observation_ack(self):
        client = MagicMock(); client.one.return_value = None
        outbox = MagicMock()
        events = []
        client.put.side_effect = lambda table, rows: events.append((table, len(rows)))
        outbox.flush.side_effect = lambda c: events.append(('ack', 0))
        self.assertEqual(self.run_mvp(client, outbox), 201)
        self.assertEqual([n for table, n in events if table == 'import_records'], [100, 100, 1])
        self.assertEqual(events[-2:], [('ack', 0), ('legacy_control_records', 1)])
        self.assertEqual([len(c.args[0]) for c in outbox.enqueue.call_args_list], [100, 100, 1])

    def test_upload_failure_does_not_create_completion_checkpoint(self):
        client = MagicMock(); client.one.return_value = None
        outbox = MagicMock(); outbox.flush.side_effect = RuntimeError('upload failed')
        with self.assertRaises(RuntimeError):
            self.run_mvp(client, outbox)
        self.assertFalse(any(c.args[0] == 'legacy_control_records' for c in client.put.call_args_list))

    def test_completed_provenance_is_reused_but_wrong_count_rejected(self):
        client = MagicMock(); client.one.return_value = {'document': {'observations': 201}}
        outbox = MagicMock()
        self.assertEqual(self.run_mvp(client, outbox), 201)
        client.put.assert_not_called(); outbox.enqueue.assert_not_called()
        client.one.return_value = {'document': {'observations': 200}}
        with self.assertRaises(ValueError):
            self.run_mvp(client, outbox)
