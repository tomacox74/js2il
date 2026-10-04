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


if __name__=='__main__':
    unittest.main()
