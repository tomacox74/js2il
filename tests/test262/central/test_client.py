import json
from pathlib import Path
import sqlite3
import tempfile
import unittest
from scripts.test262.central.client import Outbox, fixture_environment, identity
from scripts.test262.central.inventory import inventory_digest, normalize_fixture
from scripts.test262.central.importer import open_snapshot, source_hash


class FakeServer:
    def __init__(self):
        self.seen={}; self.receipts={}; self.drop_ack=True; self.calls=[]

    def call(self, operation,*args):
        self.calls.append(operation)
        if operation=='ingest':
            request,rows=args
            if request in self.seen and self.seen[request]!=rows:
                raise ValueError('Collision')
            self.seen[request]=rows
            self.receipts.setdefault(request,{'request_id':request,'records':len(rows)})
            if self.drop_ack:
                self.drop_ack=False
                raise TimeoutError('Committed, response lost')
            return self.receipts[request]
        return {'completed':True}


class ClientTests(unittest.TestCase):
    def test_commit_after_timeout_and_completion_ack(self):
        with tempfile.TemporaryDirectory() as directory:
            path=Path(directory)/'outbox.sqlite'
            out=Outbox(path,'repository','producer',1)
            rows=[{'observation_id':'observation','work_item_id':'work','lease_generation':1}]
            request=out.enqueue(rows,'request')
            server=FakeServer()
            with self.assertRaises(TimeoutError):
                out.flush(server)
            out.db.close()
            resumed=Outbox(path,'repository','producer',1)
            resumed.flush(server); resumed.flush(server)
            self.assertEqual(len(server.seen),1)
            self.assertEqual(server.calls,['ingest','ingest','complete'])
            self.assertIsNotNone(resumed.db.execute('SELECT receipt FROM messages').fetchone()[0])
            with self.assertRaises(ValueError):
                resumed.enqueue([{'different':True}],request)
            with self.assertRaises(ValueError):
                Outbox(path,'repository','another-producer',1)

    def test_credentials_not_in_fixture_environment(self):
        environment=fixture_environment({'PATH':'/bin','HOME':'/work','TEST262_DATABASE_URL':'secret',
                                         'PGPASSWORD':'secret','GH_TOKEN':'secret','AWS_ACCESS_KEY_ID':'secret'})
        self.assertEqual(environment,{'PATH':'/bin','HOME':'/work'})

    def test_inventory_identity_includes_negatives_and_dependencies(self):
        row={'path':'test/language/example.js','sha256':'a'*64,'variants':['strict'],'state':'runnable',
             'metadata':{'negative':{'phase':'runtime','type':'TypeError'}}}
        normalized=normalize_fixture(row)
        self.assertEqual(normalized['metadata_state'],'unresolved')
        before=inventory_digest([normalized])
        normalized['variants'][0]['expected_error_type']='RangeError'
        self.assertNotEqual(before,inventory_digest([normalized]))
        self.assertIsNone(normalized['dependency_manifest_digest'])

    def test_snapshot_readonly_and_digest_independent_of_insert_order(self):
        with tempfile.TemporaryDirectory() as directory:
            digests=[]
            for index,values in enumerate(([1,2],[2,1])):
                path=Path(directory)/f'{index}.sqlite'
                db=sqlite3.connect(path); db.execute('CREATE TABLE values_(id INTEGER)')
                db.executemany('INSERT INTO values_ VALUES(?)',[(v,) for v in values]); db.commit();db.close()
                source=open_snapshot(path)
                with self.assertRaises(sqlite3.OperationalError):
                    source.execute('DELETE FROM values_')
                digests.append(source_hash(source)); source.close()
            self.assertEqual(digests[0],digests[1])


if __name__=='__main__':
    unittest.main()


class IsolationAndToolTests(unittest.TestCase):
    def test_backup_tools_receive_uri_components_not_pgdatabase_uri(self):
        from scripts.test262.central.backup import libpq_environment
        env=libpq_environment('postgresql://writer:s%40cret@db.example:6543/catalogue?sslmode=require')
        self.assertEqual((env['PGHOST'],env['PGPORT'],env['PGUSER'],env['PGPASSWORD'],env['PGDATABASE'],env['PGSSLMODE']),
                         ('db.example','6543','writer','s@cret','catalogue','require'))
        self.assertFalse(any('://' in v for k,v in env.items() if k.startswith('PG')))

    def test_fixture_container_mount_excludes_checkout_secrets(self):
        import argparse
        from scripts.test262.central.inventory import REPO
        from scripts.test262.central.worker import stage_runtime
        with tempfile.TemporaryDirectory() as entry_dir, tempfile.TemporaryDirectory() as out:
            entry=REPO/'artifacts'/'stage-test'/'bin'/'Jroc.dll'
            entry.parent.mkdir(parents=True,exist_ok=True); entry.write_bytes(b'dll')
            try:
                staged=stage_runtime(argparse.Namespace(kind='mvp-composite',jroc=str(entry),host=None),Path(out)/'r')
                files={p.relative_to(staged).as_posix() for p in staged.rglob('*') if p.is_file()}
            finally:
                import shutil; shutil.rmtree(REPO/'artifacts'/'stage-test')
        self.assertIn('scripts/test262/catalogBridge.js',files)
        self.assertIn('scripts/test262/runMvp.js',files)
        self.assertIn('tests/test262/mvp-suites.json',files)
        self.assertIn('artifacts/stage-test/bin/Jroc.dll',files)
        self.assertFalse(any(f.startswith('.git') or 'outbox' in f or f.endswith('.py') for f in files))
