"""Disposable PostgreSQL integration tests. Never point TEST262_TEST_DSN at production."""
from concurrent.futures import ThreadPoolExecutor
import json
import os
from pathlib import Path
import unittest
import uuid
import psycopg
from psycopg.types.json import Jsonb

DSN=os.environ.get('TEST262_TEST_DSN')
ROOT=Path(__file__).resolve().parents[3]


@unittest.skipUnless(DSN,'Disposable PostgreSQL TEST262_TEST_DSN required')
class PostgresTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.db=psycopg.connect(DSN,autocommit=True)
        if cls.db.execute("SELECT to_regclass('public.perf_results')").fetchone()[0]:
            raise RuntimeError('Refusing production/performance database')
        cls.db.execute('CREATE ROLE anon; CREATE ROLE authenticated; CREATE ROLE service_role;')
        for migration in sorted((ROOT/'supabase/migrations').glob('*test262_catalogue*.sql')):
            cls.db.execute(migration.read_text())
        # Roll back test fixtures while retaining the migration.
        with cls.db.transaction(force_rollback=True):
            cls.db.execute((ROOT/'tests/test262/central/api.sql').read_text())
            cls.db.execute((ROOT/'tests/test262/central/native.sql').read_text())
        cls.repo=str(uuid.uuid4()); cls.producer=str(uuid.uuid4()); cls.corpus=str(uuid.uuid4()); cls.prov=str(uuid.uuid4());cls.budget=str(uuid.uuid4())
        cls.db.execute("INSERT INTO test262.repositories(repository_id,provider,provider_repository_id,canonical_name) VALUES(%s,'github','disposable','test/repo')",(cls.repo,))
        cls.db.execute("INSERT INTO test262.producers(producer_id,repository_id,kind,display_name,credential_subject) VALUES(%s,%s,'local','test','worker')",(cls.producer,cls.repo))
        cls.db.execute('CREATE ROLE catalogue_test_worker LOGIN INHERIT; GRANT test262_coordinator TO catalogue_test_worker')
        cls.db.execute("INSERT INTO test262.api_subjects VALUES('catalogue_test_worker',%s,%s,'coordinator','trusted',true)",(cls.repo,cls.producer))
        cls.db.execute("INSERT INTO test262.corpora(corpus_id,upstream_url,revision,revision_algorithm,inventory_digest,expected_fixture_count) VALUES(%s,'test',repeat('a',40),'sha1',sha256(''::bytea),10)",(cls.corpus,))
        cls.db.execute('INSERT INTO test262.repository_corpora(repository_id,corpus_id) VALUES(%s,%s)',(cls.repo,cls.corpus))
        cls.fixtures=[str(uuid.uuid4()) for _ in range(10)]
        for i,f in enumerate(cls.fixtures):
            cls.db.execute("INSERT INTO test262.fixtures(fixture_id,corpus_id,upstream_path,content_sha256,metadata_state) VALUES(%s,%s,%s,sha256('fixture'::bytea),'valid')",(f,cls.corpus,f'test/{i}.js'))
            cls.db.execute("INSERT INTO test262.fixture_variants(fixture_id,variant) VALUES(%s,'strict')",(f,))
        cls.db.execute("UPDATE test262.corpora SET inventory_state='sealed' WHERE corpus_id=%s",(cls.corpus,))
        cls.db.execute("INSERT INTO test262.provenances(provenance_id,repository_id,corpus_id,evidence_kind,identity_version,identity_sha256,identity_document,capability_sha256) VALUES(%s,%s,%s,'native',1,sha256('identity'::bytea),'{}',sha256('cap'::bytea))",(cls.prov,cls.repo,cls.corpus))
        cls.db.execute("INSERT INTO test262.budget_scopes(budget_scope_id,repository_id,scope_kind,external_key,candidate_limit,accepted_limit,attempt_limit,time_limit_ms) VALUES(%s,%s,'run','parallel',10,10,4,4000)",(cls.budget,cls.repo))
        for f in cls.fixtures:
            cls.db.execute("INSERT INTO test262.work_items(repository_id,provenance_id,fixture_id,variant,coherent_area,attempt_limit) VALUES(%s,%s,%s,'strict','test',3)",(cls.repo,cls.prov,f))
        cls.db.execute('INSERT INTO test262.budget_work_items SELECT repository_id,%s,work_item_id FROM test262.work_items WHERE repository_id=%s',(cls.budget,cls.repo))
        cls.db.execute("UPDATE test262.schema_contract SET deployment_state='active'")

    @classmethod
    def worker(cls):
        db=psycopg.connect(DSN,autocommit=True)
        db.execute('SET SESSION AUTHORIZATION catalogue_test_worker')
        return db

    def test_parallel_claims_and_shared_budget(self):
        def claim(index):
            with self.worker() as db:
                run=str(uuid.uuid4())
                db.execute('SELECT test262.api_start_run(1,%s)',(Jsonb({'run_id':run,'provenance_id':self.prov,'external_run_key':str(index),'source_revision':'b'*40,'run_kind':'native'}),))
                return db.execute('SELECT test262.api_claim(1,%s,%s,1000,31)',(run,self.budget)).fetchone()[0]
        with ThreadPoolExecutor(max_workers=8) as pool:
            results=list(pool.map(claim,range(8)))
        claimed=[r for r in results if r and not r.get('budget_exhausted')]
        self.assertEqual(len(claimed),4)
        self.assertEqual(len({r['work_item_id'] for r in claimed}),4)
        self.assertEqual(self.db.execute('SELECT reserved_attempts,reserved_ms FROM test262.budget_scopes WHERE budget_scope_id=%s',(self.budget,)).fetchone(),(4,4000))

    def test_scoped_login_cannot_write_raw_tables(self):
        with self.worker() as db:
            with self.assertRaises(psycopg.errors.InsufficientPrivilege):
                db.execute('INSERT INTO test262.repositories(provider,provider_repository_id,canonical_name) VALUES(\'github\',\'forbidden\',\'test/repo\')')
            with self.assertRaises(psycopg.errors.InsufficientPrivilege):
                db.execute('SELECT * FROM test262.observations')
            db.execute('BEGIN ISOLATION LEVEL REPEATABLE READ READ ONLY')
            rows=[];after=None
            while True:
                page=db.execute('SELECT test262.api_read(1,%s,%s,%s,3)',('fixtures',Jsonb({}),Jsonb(after) if after else None)).fetchone()[0]
                rows+=page['rows'];after=page['next']
                if not after:
                    break
            db.execute('ROLLBACK')
            self.assertEqual(sorted(r['fixture_id'] for r in rows),sorted(self.fixtures))

    def test_indexed_replay_normalizes_uuid_keys_and_rejects_conflicts(self):
        fixture = self.fixtures[0]
        row = {'repository_id': self.repo, 'provenance_id': self.prov,
               'fixture_id': fixture, 'eligibility': 'runnable', 'reason_codes': [],
               'diagnostic': {'replay_test': True}}
        with self.worker() as db:
            receipt = db.execute('SELECT test262.api_put(1,%s,%s)',
                                 ('provenance_fixture_eligibility', Jsonb([row]))).fetchone()[0]
            self.assertEqual(receipt['records'], 1)
            normalized = dict(row, repository_id=self.repo.upper(),
                              provenance_id=self.prov.upper(), fixture_id=fixture.upper())
            self.assertEqual(db.execute('SELECT test262.api_put(1,%s,%s)',
                             ('provenance_fixture_eligibility', Jsonb([normalized]))).fetchone()[0], receipt)
            with self.assertRaises(psycopg.errors.RaiseException):
                db.execute('SELECT test262.api_put(1,%s,%s)',
                           ('provenance_fixture_eligibility', Jsonb([dict(row, eligibility='unresolved')])))
            with self.assertRaises(psycopg.errors.InsufficientPrivilege):
                db.execute('SELECT test262.api_put(1,%s,%s)',
                           ('provenance_fixture_eligibility', Jsonb([dict(row, repository_id=str(uuid.uuid4()))])))
        stored = self.db.execute('SELECT eligibility,diagnostic FROM test262.provenance_fixture_eligibility WHERE provenance_id=%s AND fixture_id=%s',
                                 (self.prov, fixture)).fetchone()
        self.assertEqual(stored, ('runnable', {'replay_test': True}))

    def test_client_paged_reads_and_streamed_export(self):
        import hashlib, tempfile
        from psycopg.conninfo import make_conninfo
        from scripts.test262.central.client import Client
        self.db.execute("ALTER ROLE catalogue_test_worker PASSWORD 'catalogue-test'")
        client=Client(make_conninfo(DSN,user='catalogue_test_worker',password='catalogue-test'),1)
        try:
            self.assertEqual(sorted(r['fixture_id'] for r in client.read('fixtures',page=3)),sorted(self.fixtures))
            self.assertEqual(client.one('fixtures',fixture_id=self.fixtures[0])['upstream_path'],'test/0.js')
            self.assertIsNone(client.one('fixtures',fixture_id=str(uuid.uuid4())))
            with tempfile.TemporaryDirectory() as directory:
                output=Path(directory)/'export.ndjson'
                summary=client.export(output)
                lines=output.read_text().splitlines()
        finally:
            client.close()
        header,footer=json.loads(lines[0])['header'],json.loads(lines[-1])['footer']
        self.assertEqual(header['repository_id'],self.repo)
        self.assertEqual(footer['counts']['fixtures'],10)
        self.assertEqual(footer['sha256'],hashlib.sha256(''.join(l+'\n' for l in lines[:-1]).encode()).hexdigest())
        self.assertEqual(summary['sha256'],footer['sha256'])
        self.assertEqual(sum(footer['counts'].values()),len(lines)-2)

    def test_overlapping_mvp_snapshots_preserve_old_run_and_replay_outbox(self):
        """Real restricted APIs: old initialization revision must not block older snapshots."""
        import sqlite3, tempfile
        from types import SimpleNamespace
        from unittest.mock import patch
        from scripts.test262.central.client import Client, Outbox, identity, sha
        from scripts.test262.central.importer import import_snapshot, start_run
        repository = str(uuid.uuid4())
        self.db.execute("INSERT INTO test262.repositories(repository_id,provider,provider_repository_id,canonical_name) VALUES(%s,'github','import-replay','test/import-replay')",(repository,))
        producer = str(uuid.uuid4())
        role = 'catalogue_test_importer'
        self.db.execute('CREATE ROLE catalogue_test_importer LOGIN INHERIT; GRANT test262_coordinator TO catalogue_test_importer')
        self.db.execute("INSERT INTO test262.producers(producer_id,repository_id,kind,display_name,credential_subject) VALUES(%s,%s,'local','import test',%s)", (producer,repository,role))
        self.db.execute("INSERT INTO test262.api_subjects VALUES(%s,%s,%s,'coordinator','legacy',true)",(role,repository,producer))
        self.db.execute("UPDATE test262.schema_contract SET deployment_state='shadow'")
        client = Client(DSN, 1)
        client.db.execute('SET SESSION AUTHORIZATION catalogue_test_importer')
        client.contract = client.db.execute('SELECT test262.api_contract()').fetchone()[0]
        try:
            with tempfile.TemporaryDirectory() as directory:
                root = Path(directory)
                args = SimpleNamespace(repository=repository, producer=producer, root=None, missing_artifacts=[])
                for name, revision in (('latest', 'b'*40), ('older', 'c'*40)):
                    with sqlite3.connect(root/(name+'.sqlite')) as db:
                        db.executescript('''CREATE TABLE settings(key,value); CREATE TABLE provenance(id,document);
                            CREATE TABLE fixtures(provenance,path,sha256,variants,state,reasons);
                            CREATE TABLE results(provenance,path,variant,document,finished,verdict);''')
                        db.executemany('INSERT INTO settings VALUES(?,?)',[('schema','1'),('compiler_commit',revision)])
                        db.execute('INSERT INTO provenance VALUES(?,?)', ('shared',json.dumps({'upstream':{'cloneUrl':'disposable-import','commit':'a'*40}})))
                        db.execute('INSERT INTO fixtures VALUES(?,?,?,?,?,?)',('shared','test/import-replay.js','d'*64,'["default"]','runnable','[]'))
                        db.execute('INSERT INTO results VALUES(?,?,?,?,?,?)',('shared','test/import-replay.js','default','{}',1,'matched'))
                args.source=str(root/'latest.sqlite');args.source_uri='test:latest';args.outbox=str(root/'latest-outbox.sqlite')
                # Seed through the old implementation, reproducing the already-deployed run.
                with patch('scripts.test262.central.importer.start_mvp_run',
                           side_effect=lambda c,a,p,k,r: start_run(c,a,p,'legacy-mvp:'+k,'b'*40)):
                    first = import_snapshot(client,args)
                run_id = identity(repository,producer,'legacy-mvp:shared')
                before_run = client.one('runs',run_id=run_id)
                before_observations = client.rows('observations',run_id=run_id)
                self.assertEqual(len(before_observations),1)
                # Reproduce the former failure using the real API before testing the fix.
                with self.assertRaises(psycopg.errors.RaiseException):
                    start_run(client,args,before_run['provenance_id'],'legacy-mvp:shared','c'*40)
                args.source=str(root/'older.sqlite');args.source_uri='test:older';args.outbox=str(root/'older-outbox.sqlite')
                second = import_snapshot(client,args)
                self.assertNotEqual(first['import_id'],second['import_id'])
                self.assertEqual(first['observations'],second['observations'])
                self.assertEqual(client.one('runs',run_id=run_id),before_run)
                self.assertEqual(client.rows('observations',run_id=run_id),before_observations)
                mapped = client.rows('import_records',import_id=second['import_id'])
                self.assertEqual([r['entity_id'] for r in mapped],[before_observations[0]['observation_id']])
                # Force a replay of the pre-fix exact stored observation payload/request.
                original = Outbox(str(root/'latest-outbox.sqlite'),repository,producer,1)
                payload,digest = original.db.execute('SELECT payload,digest FROM messages').fetchone()
                self.assertEqual(sha(json.loads(payload)),digest)
                original.db.execute('UPDATE messages SET receipt=NULL');original.db.commit()
                original.flush(client)
                self.assertEqual(original.db.execute('SELECT count(*) FROM messages WHERE receipt IS NULL').fetchone()[0],0)
                original.db.close()
                self.assertEqual(client.rows('observations',run_id=run_id),before_observations)
                # Retry old source: completed checkpoint prevents duplicate execution/upload.
                self.assertEqual(import_snapshot(client,args)['observations'],1)
                checkpoints = client.rows('legacy_control_records',import_id=second['import_id'],source_table='mvp_import_checkpoint')
                self.assertEqual(len(checkpoints),1)
                settings = client.rows('legacy_control_records',import_id=second['import_id'],source_table='settings')
                self.assertIn({'key':'compiler_commit','value':'c'*40},[r['document'] for r in settings])
                # A newly discovered provenance uses its upstream pin, regardless of snapshot settings.
                with sqlite3.connect(args.source) as db:
                    db.execute("UPDATE provenance SET id='fresh'")
                    db.execute("UPDATE fixtures SET provenance='fresh'")
                    db.execute("UPDATE results SET provenance='fresh'")
                args.source_uri='test:fresh';args.outbox=str(root/'fresh-outbox.sqlite')
                third = import_snapshot(client,args)
                new_run = client.one('runs',run_id=identity(repository,producer,'legacy-mvp:fresh'))
                self.assertEqual(new_run['source_revision'],'a'*40)
                self.assertEqual(third['observations'],1)
                self.assertIn('not a compiler source revision',third['limitations']['mvp_run_revision'])
        finally:
            client.close()
            self.db.execute("UPDATE test262.schema_contract SET deployment_state='active'")


if __name__=='__main__':
    unittest.main()
