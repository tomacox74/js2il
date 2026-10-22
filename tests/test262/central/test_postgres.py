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

    def test_indexed_read_cursors_seek_on_actual_api_queries_at_scale(self):
        """Inspect nested api_read plans, not a hand-written approximation of its SQL."""
        plans = []

        def capture(notice):
            message = notice.message_primary
            if 'plan:\n{' in message:
                plan = json.loads(message[message.index('{'):])
                if ('SELECT coalesce(jsonb_agg(r.j ORDER BY r.n)' in plan.get('Query Text', '') or
                        'SELECT to_jsonb(x) FROM test262.' in plan.get('Query Text', '')):
                    plans.append(plan)

        def nodes(plan):
            yield plan
            for child in plan.get('Plans', []):
                yield from nodes(child)

        def fixture_id(index):
            return '10000000-0000-0000-0000-' + format(index, '012x')

        self.db.execute("LOAD 'auto_explain'")
        self.db.add_notice_handler(capture)
        try:
            with self.db.transaction(force_rollback=True):
                self.db.execute('SET TRANSACTION ISOLATION LEVEL REPEATABLE READ')
                self.db.execute("SET LOCAL client_min_messages='notice'; SET LOCAL auto_explain.log_level='notice'; SET LOCAL auto_explain.log_format='json'; SET LOCAL auto_explain.log_analyze=on; SET LOCAL auto_explain.log_timing=off; SET LOCAL auto_explain.log_nested_statements=on; SET LOCAL auto_explain.log_min_duration=-1")
                other, own_corpus, other_corpus = (str(uuid.uuid4()) for _ in range(3))
                self.db.execute("INSERT INTO test262.repositories(repository_id,provider,provider_repository_id,canonical_name) VALUES(%s,'github',%s,'test/pagination-other')", (other, other))
                # Temporary operator binding allows auto_explain settings in this disposable DB.
                # The function's normal api_identity/scope checks still execute unchanged.
                self.db.execute("INSERT INTO test262.api_subjects VALUES(session_user,%s,%s,'coordinator','trusted',true)", (self.repo, self.producer))
                for repository, corpus in ((self.repo, own_corpus), (other, other_corpus)):
                    self.db.execute("INSERT INTO test262.corpora(corpus_id,upstream_url,revision,revision_algorithm,inventory_digest,expected_fixture_count) VALUES(%s,'disposable-pagination',repeat('a',40),'sha1',sha256(%s::bytea),16384)", (corpus, corpus.encode()))
                    self.db.execute('INSERT INTO test262.repository_corpora(repository_id,corpus_id) VALUES(%s,%s)', (repository, corpus))
                # Interleave foreign fixtures so a missing ownership predicate is observable.
                self.db.execute("""INSERT INTO test262.fixtures(fixture_id,corpus_id,upstream_path,content_sha256,metadata_state)
                    SELECT ('10000000-0000-0000-0000-'||lpad(to_hex(i),12,'0'))::uuid,
                           CASE WHEN mod(i,2)=0 THEN %s::uuid ELSE %s::uuid END,
                           'test/pagination-'||i||'.js',sha256('fixture'::bytea),'valid'
                    FROM generate_series(1,32768) i""", (own_corpus, other_corpus))
                self.db.execute("INSERT INTO test262.fixture_variants(fixture_id,variant) SELECT fixture_id,'strict' FROM test262.fixtures WHERE corpus_id IN (%s,%s)", (own_corpus, other_corpus))
                self.db.execute("INSERT INTO test262.fixture_dependencies(fixture_id,dependency_path,content_sha256,dependency_kind) SELECT fixture_id,'harness/example.js',sha256('dependency'::bytea),'harness' FROM test262.fixtures WHERE corpus_id IN (%s,%s)", (own_corpus, other_corpus))
                self.db.execute("UPDATE test262.corpora SET inventory_state='sealed' WHERE corpus_id IN (%s,%s)", (own_corpus, other_corpus))
                provenances = ['20000000-0000-0000-0000-' + format(i, '012x') for i in (1, 2, 3)]
                for index, provenance in enumerate(provenances):
                    repository, corpus = (self.repo, own_corpus) if index < 2 else (other, other_corpus)
                    self.db.execute("INSERT INTO test262.provenances(provenance_id,repository_id,corpus_id,evidence_kind,identity_version,identity_sha256,identity_document,capability_sha256) VALUES(%s,%s,%s,'native',1,sha256(%s::bytea),'{}',sha256('cap'::bytea))", (provenance, repository, corpus, provenance.encode()))
                    self.db.execute("INSERT INTO test262.provenance_fixture_eligibility(repository_id,provenance_id,fixture_id,eligibility) SELECT %s,%s,fixture_id,'runnable' FROM test262.fixtures WHERE corpus_id=%s", (repository, provenance, corpus))
                for table in ('fixtures', 'fixture_variants', 'fixture_dependencies', 'provenance_fixture_eligibility'):
                    self.db.execute('ANALYZE test262.' + table)

                def read(table, filters, after, page=64):
                    plans.clear()
                    self.db.execute('SET LOCAL auto_explain.log_min_duration=0')
                    try:
                        result = self.db.execute('SELECT test262.api_read(1,%s,%s,%s,%s)',
                                                 (table, Jsonb(filters), Jsonb(after) if after else None, page)).fetchone()[0]
                    finally:
                        self.db.execute('SET LOCAL auto_explain.log_min_duration=-1')
                    self.assertEqual(len(plans), 1, plans)
                    self.assertNotIn('api_scope(', plans[0]['Query Text'])
                    scans = [node for node in nodes(plans[0]['Plan'])
                             if node.get('Relation Name') == table and node.get('Index Cond')]
                    self.assertTrue(scans, plans[0])
                    return result, scans

                for offset in (4096, 30000):
                    with self.subTest(table='fixtures', offset=offset):
                        result, scans = read('fixtures', {}, {'fixture_id': fixture_id(offset)})
                        self.assertEqual([row['fixture_id'] for row in result['rows']],
                                         [fixture_id(i) for i in range(offset+2, offset+130, 2)])
                        self.assertTrue(any('>' in scan['Index Cond'] for scan in scans), scans)
                        # Count examined rows rather than impose a flaky wall-clock threshold.
                        self.assertLessEqual(sum(scan.get('Actual Rows', 0) + scan.get('Rows Removed by Filter', 0)
                                                 for scan in scans), 256, scans)

                    for table, extra_key, extra_value in (
                            ('fixture_variants', 'variant', 'strict'),
                            ('fixture_dependencies', 'dependency_path', 'harness/example.js')):
                        with self.subTest(table=table, offset=offset):
                            result, scans = read(table, {}, {'fixture_id': fixture_id(offset), extra_key: extra_value})
                            self.assertEqual([row['fixture_id'] for row in result['rows']],
                                             [fixture_id(i) for i in range(offset+2, offset+130, 2)])
                            self.assertTrue(any('>' in scan['Index Cond'] for scan in scans), scans)
                            self.assertLessEqual(sum(scan.get('Actual Rows', 0) + scan.get('Rows Removed by Filter', 0)
                                                     for scan in scans), 256, scans)

                    with self.subTest(table='provenance_fixture_eligibility', offset=offset):
                        result, scans = read('provenance_fixture_eligibility', {},
                                             {'provenance_id': provenances[0], 'fixture_id': fixture_id(offset)})
                        self.assertEqual([(row['provenance_id'], row['fixture_id']) for row in result['rows']],
                                         [(provenances[0], fixture_id(i)) for i in range(offset+2, offset+130, 2)])
                        self.assertTrue(any('>' in scan['Index Cond'] and 'provenance_id' in scan['Index Cond']
                                            and 'fixture_id' in scan['Index Cond'] for scan in scans), scans)
                        self.assertLessEqual(sum(scan.get('Actual Rows', 0) + scan.get('Rows Removed by Filter', 0)
                                                 for scan in scans), 256, scans)

                # Compound cursor must cross the provenance boundary without skipping/duplicating.
                result, _ = read('provenance_fixture_eligibility', {},
                                 {'provenance_id': provenances[0], 'fixture_id': fixture_id(32760)})
                expected = [(provenances[0], fixture_id(i)) for i in range(32762, 32769, 2)]
                expected += [(provenances[1], fixture_id(i)) for i in range(2, 121, 2)]
                self.assertEqual([(row['provenance_id'], row['fixture_id']) for row in result['rows']], expected)
                # Typed equality filters must also be index conditions, including UUID normalization.
                result, scans = read('fixtures', {'fixture_id': fixture_id(30010).upper()}, None)
                self.assertEqual([row['fixture_id'] for row in result['rows']], [fixture_id(30010)])
                self.assertTrue(any('fixture_id =' in scan['Index Cond'] for scan in scans), scans)
                result, _ = read('provenance_fixture_eligibility', {'provenance_id': provenances[1]},
                                 {'provenance_id': provenances[1], 'fixture_id': fixture_id(30000)})
                self.assertEqual({row['provenance_id'] for row in result['rows']}, {provenances[1]})
                # A known foreign fixture/provenance must remain invisible through the same API.
                for table, filters in (('fixtures', {'fixture_id': fixture_id(30001)}),
                                       ('fixture_variants', {'fixture_id': fixture_id(30001)}),
                                       ('fixture_dependencies', {'fixture_id': fixture_id(30001)}),
                                       ('provenance_fixture_eligibility', {'provenance_id': provenances[2]})):
                    self.assertEqual(self.db.execute('SELECT test262.api_read(1,%s,%s,NULL,64)',
                                                     (table, Jsonb(filters))).fetchone()[0]['rows'], [])
                # A bulk export keeps a single scan alive across bounded FETCHes.
                self.db.execute('SET LOCAL transaction_read_only=on')
                for table, expected_count in (('fixtures', 16394), ('fixture_variants', 16394),
                                              ('fixture_dependencies', 16384),
                                              ('provenance_fixture_eligibility', 32768)):
                    with self.subTest(bulk_table=table):
                        plans.clear()
                        self.db.execute('SET LOCAL auto_explain.log_min_duration=0')
                        portal = self.db.execute('SELECT test262.api_export_open(1,%s)', (table,)).fetchone()[0]
                        count = 0
                        with self.db.cursor(name=portal) as cursor:
                            while batch := cursor.fetchmany(1000):
                                self.assertLessEqual(len(batch), 1000)
                                count += len(batch)
                        self.db.execute('SET LOCAL auto_explain.log_min_duration=-1')
                        # Earlier tests may have added a small number of owned eligibility rows.
                        expected = self.db.execute('SELECT count(*) FROM test262."' + table +
                            '" x WHERE test262.api_scope(%s,to_jsonb(x),%s)', (table, self.repo)).fetchone()[0]
                        self.assertEqual(count, expected)
                        self.assertGreaterEqual(count, expected_count)
                        self.assertEqual(len(plans), 1, plans)
                        self.assertNotIn('api_scope(', plans[0]['Query Text'])
                        self.assertNotIn('ORDER BY', plans[0]['Query Text'])
                        scans = [node for node in nodes(plans[0]['Plan']) if node.get('Relation Name') == table]
                        self.assertTrue(scans, plans[0])
                        # The planner may use one indexed lookup per owned fixture.
                        # Bound total visits, not loop count: a page-by-page full
                        # rescan would exceed this linear bound by many multiples.
                        examined = sum((scan.get('Actual Rows', 0) + scan.get('Rows Removed by Filter', 0))
                                       * scan.get('Actual Loops', 1) for scan in scans)
                        self.assertLessEqual(examined, 2 * count + 32, scans)
                        self.assertFalse(any(node['Node Type'] in ('Limit', 'WindowAgg', 'Aggregate', 'Sort')
                                             for node in nodes(plans[0]['Plan'])), plans[0])
        finally:
            self.db.remove_notice_handler(capture)

    def test_typed_reader_scope_matches_existing_helper_for_every_export_table(self):
        from scripts.test262.central.client import EXPORT_TABLES, sha
        with self.db.transaction(force_rollback=True):
            self.db.execute('SET TRANSACTION ISOLATION LEVEL REPEATABLE READ')
            other, corpus, fixture = (str(uuid.uuid4()) for _ in range(3))
            self.db.execute("INSERT INTO test262.repositories(repository_id,provider,provider_repository_id,canonical_name) VALUES(%s,'github',%s,'test/typed-scope')", (other, other))
            self.db.execute("INSERT INTO test262.api_subjects VALUES(session_user,%s,%s,'coordinator','trusted',true)", (self.repo, self.producer))
            # Shared corpora remain visible through this repository's link.
            self.db.execute('INSERT INTO test262.repository_corpora VALUES(%s,%s,clock_timestamp())', (other, self.corpus))
            self.db.execute("INSERT INTO test262.corpora(corpus_id,upstream_url,revision,revision_algorithm,inventory_digest,expected_fixture_count) VALUES(%s,'test',repeat('a',40),'sha1',sha256('scope'::bytea),1)", (corpus,))
            self.db.execute('INSERT INTO test262.repository_corpora VALUES(%s,%s,clock_timestamp())', (other, corpus))
            self.db.execute("INSERT INTO test262.fixtures(fixture_id,corpus_id,upstream_path,content_sha256,metadata_state) VALUES(%s,%s,'test/foreign.js',sha256('fixture'::bytea),'valid')", (fixture, corpus))
            self.db.execute("INSERT INTO test262.fixture_variants(fixture_id,variant) VALUES(%s,'strict')", (fixture,))
            self.db.execute("INSERT INTO test262.fixture_dependencies(fixture_id,dependency_path,content_sha256,dependency_kind) VALUES(%s,'harness/foreign.js',sha256('dependency'::bytea),'harness')", (fixture,))
            self.db.execute("UPDATE test262.corpora SET inventory_state='sealed' WHERE corpus_id=%s", (corpus,))
            metadata = {'specification': {'schema_version': 1, 'raw_identifiers': {}, 'reference_state': 'absent'}}
            for source in (self.fixtures[0], fixture):
                self.db.execute("""INSERT INTO test262.fixture_specification_metadata(fixture_id,upstream_revision,upstream_path,content_sha256,metadata,metadata_sha256)
                    SELECT f.fixture_id,c.revision,f.upstream_path,f.content_sha256,%s,sha256(convert_to(test262.canonical_json(%s),'UTF8'))
                    FROM test262.fixtures f JOIN test262.corpora c USING(corpus_id) WHERE fixture_id=%s ON CONFLICT DO NOTHING""",
                    (Jsonb(metadata), Jsonb(metadata), source))
            expected_tables = {}
            for table in EXPORT_TABLES:
                with self.subTest(table=table):
                    expected = self.db.execute('SELECT to_jsonb(x) FROM test262."' + table + '" x WHERE test262.api_scope(%s,to_jsonb(x),%s)', (table, self.repo)).fetchall()
                    actual, after = [], None
                    while True:
                        page = self.db.execute('SELECT test262.api_read(1,%s,%s,%s,1000)',
                                               (table, Jsonb({}), Jsonb(after) if after else None)).fetchone()[0]
                        actual.extend(page['rows'])
                        after = page['next']
                        if not after:
                            break
                    self.assertEqual({sha(row) for row in actual}, {sha(row[0]) for row in expected})
                    self.assertEqual(len(actual), len(expected))
                    expected_tables[table] = [row[0] for row in expected]
            self.db.execute('SET LOCAL transaction_read_only=on')
            for table in EXPORT_TABLES:
                with self.subTest(bulk_table=table):
                    portal = self.db.execute('SELECT test262.api_export_open(1,%s)', (table,)).fetchone()[0]
                    with self.db.cursor(name=portal) as cursor:
                        actual = [row[0] for row in cursor]
                    self.assertEqual({sha(row) for row in actual}, {sha(row) for row in expected_tables[table]})
                    self.assertEqual(len(actual), len(expected_tables[table]))
            for table in ('fixtures', 'fixture_variants', 'fixture_dependencies', 'fixture_specification_metadata'):
                self.assertEqual(self.db.execute('SELECT test262.api_read(1,%s,%s,NULL,10)',
                                                (table, Jsonb({'fixture_id': fixture}))).fetchone()[0]['rows'], [])

    def test_indexed_reader_keeps_invalid_argument_rejection(self):
        with self.worker() as db:
            for table, filters, after, size in (
                ('fixtures', [], None, 10),
                ('fixtures', {'fixture_id': None}, None, 10),
                ('fixtures', {'fixture_id;--': self.fixtures[0]}, None, 10),
                ('fixtures', {}, [], 10),
                ('fixtures', {}, {}, 10),
                ('provenance_fixture_eligibility', {}, {'provenance_id': self.prov}, 10),
                ('fixtures', {}, None, 0), ('fixtures', {}, None, 1001),
            ):
                with self.subTest(table=table, filters=filters, after=after, size=size):
                    with self.assertRaises(psycopg.errors.RaiseException):
                        db.execute('SELECT test262.api_read(1,%s,%s,%s,%s)',
                                   (table, Jsonb(filters), Jsonb(after) if after is not None else None, size))

    def test_bulk_export_requires_trusted_coordinator_and_read_only_snapshot(self):
        function = 'test262.api_export_open(bigint,text)'
        for role in ('anon', 'authenticated', 'service_role', 'test262_ingest'):
            self.assertFalse(self.db.execute('SELECT has_function_privilege(%s,%s,\'EXECUTE\')',
                                            (role, function)).fetchone()[0])
        for begin in (None, 'BEGIN READ ONLY', 'BEGIN ISOLATION LEVEL REPEATABLE READ'):
            with self.subTest(transaction=begin), self.worker() as db:
                if begin:
                    db.execute(begin)
                with self.assertRaises(psycopg.errors.InvalidTransactionState):
                    db.execute("SELECT test262.api_export_open(1,'fixtures')")
                db.execute('ROLLBACK')
        for table in (None, 'api_subjects', 'public.perf_results', 'fixtures; DROP SCHEMA test262'):
            with self.subTest(table=table), self.worker() as db:
                db.execute('BEGIN ISOLATION LEVEL REPEATABLE READ READ ONLY')
                with self.assertRaises(psycopg.errors.RaiseException):
                    db.execute('SELECT test262.api_export_open(1,%s)', (table,))
                db.execute('ROLLBACK')
        with self.worker() as db:
            fraction = db.execute('SHOW cursor_tuple_fraction').fetchone()[0]
            db.execute('BEGIN ISOLATION LEVEL REPEATABLE READ READ ONLY')
            portal = db.execute("SELECT test262.api_export_open(1,'fixtures')").fetchone()[0]
            self.assertEqual(db.execute('SELECT is_holdable,is_scrollable FROM pg_cursors WHERE name=%s',
                                        (portal,)).fetchone(), (False, False))
            self.assertEqual(db.execute('SHOW cursor_tuple_fraction').fetchone()[0], fraction)
            db.execute('COMMIT')
            self.assertEqual(db.execute('SELECT count(*) FROM pg_cursors WHERE name=%s', (portal,)).fetchone()[0], 0)
        with self.worker() as db:
            db.execute('BEGIN ISOLATION LEVEL REPEATABLE READ READ ONLY')
            with self.assertRaises(psycopg.errors.SerializationFailure):
                db.execute("SELECT test262.api_export_open(0,'fixtures')")
            db.execute('ROLLBACK')
        self.db.execute('CREATE ROLE catalogue_bulk_untrusted LOGIN INHERIT; GRANT test262_coordinator TO catalogue_bulk_untrusted')
        self.db.execute("INSERT INTO test262.api_subjects VALUES('catalogue_bulk_untrusted',%s,%s,'coordinator','untrusted',true)", (self.repo, self.producer))
        with psycopg.connect(DSN, autocommit=True) as db:
            db.execute('SET SESSION AUTHORIZATION catalogue_bulk_untrusted')
            db.execute('BEGIN ISOLATION LEVEL REPEATABLE READ READ ONLY')
            with self.assertRaises(psycopg.errors.InsufficientPrivilege):
                db.execute("SELECT test262.api_export_open(1,'fixtures')")
            db.execute('ROLLBACK')

    def test_bulk_export_snapshot_deadline_and_portal_cleanup(self):
        from contextlib import closing
        import tempfile
        from unittest.mock import patch
        from scripts.test262.central.client import Client
        from scripts.test262.central.verify_specification_deployment import verify_export
        client = Client.__new__(Client)
        client.db = self.worker()
        self.addCleanup(client.close)
        client.db.execute("SET statement_timeout='120s'")
        client.epoch, client.Jsonb = 1, Jsonb
        client.contract = client.db.execute('SELECT test262.api_contract()').fetchone()[0]
        self.assertEqual(client.contract['bulk_export_version'], 1)
        original = client.export_rows
        late = str(uuid.uuid4())
        inserted = []

        def concurrent_insert(table):
            with closing(original(table)) as rows:
                for row in rows:
                    if not inserted:
                        self.db.execute("""INSERT INTO test262.budget_scopes(budget_scope_id,repository_id,scope_kind,external_key,candidate_limit,accepted_limit,attempt_limit,time_limit_ms)
                            VALUES(%s,%s,'run',%s,1,1,1,1000)""", (late, self.repo, late))
                        inserted.append(True)
                    yield row

        client.export_rows = concurrent_insert
        with tempfile.TemporaryDirectory() as directory:
            output = Path(directory) / 'snapshot.ndjson'
            summary = client.export(output, seconds=60)
            self.assertEqual(verify_export(output, summary), summary)
            exported = [json.loads(line) for line in output.read_text().splitlines()[1:-1]]
            self.assertNotIn(late, {r['row']['budget_scope_id'] for r in exported if r['table'] == 'budget_scopes'})
            self.assertIn(late, {r['budget_scope_id'] for r in client.rows('budget_scopes')})
            self.assertEqual(client.db.execute("SELECT count(*) FROM pg_cursors WHERE name LIKE 'test262_export_%'").fetchone()[0], 0)
            clock = [0]

            def expire(table):
                with closing(original(table)) as rows:
                    yield next(rows)
                    clock[0] = 2
                    yield next(rows)

            client.export_rows = expire
            output = Path(directory) / 'incomplete.ndjson'
            with patch('scripts.test262.central.client.EXPORT_TABLES', ('fixtures',)), \
                 patch('scripts.test262.central.client.time.monotonic', side_effect=lambda: clock[0]):
                with self.assertRaises(TimeoutError):
                    client.export(output, seconds=1)
            self.assertFalse(output.exists())
            records = [json.loads(line) for line in output.with_name(output.name + '.partial').read_text().splitlines()]
            self.assertFalse(any('footer' in row for row in records))
            self.assertEqual(client.db.execute("SELECT count(*) FROM pg_cursors WHERE name LIKE 'test262_export_%'").fetchone()[0], 0)

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

    def test_specification_metadata_ingestion_enrichment_reporting_and_export(self):
        import copy
        import hashlib
        import subprocess
        import tempfile
        from types import SimpleNamespace
        from psycopg.conninfo import make_conninfo
        from scripts.test262 import catalog
        from scripts.test262.central.client import Client, bytea, sha
        from scripts.test262.central.inventory import enrich_specification_metadata, normalize_fixture, register

        repository, producer = str(uuid.uuid4()), str(uuid.uuid4())
        role = 'catalogue_specification_worker'
        self.db.execute("INSERT INTO test262.repositories(repository_id,provider,provider_repository_id,canonical_name) VALUES(%s,'github','specification-metadata','test/specification')", (repository,))
        self.db.execute("INSERT INTO test262.producers(producer_id,repository_id,kind,display_name,credential_subject) VALUES(%s,%s,'local','specification test',%s)", (producer, repository, role))
        self.db.execute("CREATE ROLE catalogue_specification_worker LOGIN INHERIT PASSWORD 'disposable-specification-test'; GRANT test262_coordinator,test262_reporter TO catalogue_specification_worker")
        self.db.execute("INSERT INTO test262.api_subjects VALUES(%s,%s,%s,'coordinator','trusted',true)", (role, repository, producer))
        client = Client(make_conninfo(DSN, user=role, password='disposable-specification-test'), 1)
        self.addCleanup(client.close)
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / 'test').mkdir()
            fixtures = {
                'modern': 'esid: sec-example\ndescription: Modern description\ninfo: Algorithm detail',
                'legacy': 'es5id: 10.5-1-s\ndescription: Legacy description',
                'absent': 'description: No reference',
            }
            scalar_cases = {
                'modern-null': ('esid: null', {'esid': None}, 'ambiguous'),
                'modern-number': ('esid: 19.1', {'esid': 19.1}, 'ambiguous'),
                'modern-boolean': ('esid: false', {'esid': False}, 'ambiguous'),
                'legacy-null': ('es5id: null', {'es5id': None}, 'ambiguous'),
                'legacy-number': ('es6id: 19.1', {'es6id': 19.1}, 'ambiguous'),
                'legacy-boolean': ('es5id: true', {'es5id': True}, 'ambiguous'),
                'quoted-null': ('es5id: "null"', {'es5id': 'null'}, 'legacy'),
                'quoted-number': ('es6id: "19.1"', {'es6id': '19.1'}, 'legacy'),
                'quoted-boolean': ("esid: 'false'", {'esid': 'false'}, 'unresolved'),
                'scientific-small': ('esid: 1e-7', {'esid': 1e-7}, 'ambiguous'),
                'small-decimal': ('es5id: 0.00001', {'es5id': 0.00001}, 'ambiguous'),
                'scientific-large': ('es6id: 1e30', {'es6id': 10 ** 30}, 'ambiguous'),
            }
            fixtures.update({name: case[0] for name, case in scalar_cases.items()})
            fixture_count = len(fixtures)
            expected_references = {
                'modern': ({'esid': 'sec-example'}, 'unresolved'),
                'legacy': ({'es5id': '10.5-1-s'}, 'legacy'),
                'absent': ({}, 'absent'),
                **{name: case[1:] for name, case in scalar_cases.items()},
            }
            for name, frontmatter in fixtures.items():
                (root / 'test' / (name + '.js')).write_text('/*---\n' + frontmatter + '\nflags: [raw]\n---*/\nvoid 0;\n')
            subprocess.run(['git', 'init', '-q', str(root)], check=True)
            subprocess.run(['git', '-C', str(root), 'add', 'test'], check=True)
            subprocess.run(['git', '-C', str(root), '-c', 'user.name=Catalogue Test',
                            '-c', 'user.email=catalogue-test@example.invalid', 'commit', '-qm', 'Source fixtures'], check=True)
            revision = subprocess.check_output(['git', '-C', str(root), 'rev-parse', 'HEAD'], text=True).strip()
            original = catalog.bridge({'command': 'metadata', 'root': str(root),
                                       'paths': ['test/' + name + '.js' for name in fixtures]})
            rows = [normalize_fixture(row, root) for row in original]
            upstream = {'cloneUrl': 'https://example.invalid/test262', 'commit': revision}
            new_corpus, new_ids = register(client, repository, upstream, copy.deepcopy(rows))
            new_report = client.db.execute("""
                SELECT upstream_path,reference_state,raw_identifiers,description,metadata_source,
                       upstream_revision,content_sha256,mapping_status,specification_edition,interpreted_clause,
                       esid,es5id,es6id
                FROM test262_reporting.v_fixture_specification_references
                WHERE repository_id=%s AND corpus_id=%s ORDER BY upstream_path
                """, (repository, new_corpus)).fetchall()
            self.assertEqual(len(new_report), fixture_count)
            for row in new_report:
                name = Path(row[0]).stem
                identifiers, state = expected_references[name]
                self.assertEqual((row[1], row[2]), (state, identifiers))
                self.assertEqual(row[3], {'absent': 'No reference', 'legacy': 'Legacy description',
                                         'modern': 'Modern description'}.get(name))
                self.assertEqual(row[10:], tuple(identifiers.get(key) for key in ('esid', 'es5id', 'es6id')))
                self.assertEqual((row[4], row[5], row[7], row[8], row[9]),
                                 ('inventory', revision, 'not-attempted', None, None))
                self.assertEqual(row[6], hashlib.sha256((root / row[0]).read_bytes()).hexdigest())

            # Model the already-deployed stripped inventory, then attach evidence to it.
            old_rows = copy.deepcopy(rows)
            for row in old_rows:
                for key in ('description', 'info', 'author', 'esid', 'es5id', 'es6id', 'specification'):
                    row['metadata'].pop(key)
            old_corpus, old_ids = register(client, repository, upstream, old_rows)
            self.assertNotEqual(new_corpus, old_corpus)
            provenance = str(uuid.uuid4())
            client.put('provenances', [{
                'provenance_id': provenance, 'repository_id': repository, 'corpus_id': old_corpus,
                'evidence_kind': 'native', 'identity_version': 1, 'identity_sha256': bytea(sha('identity')),
                'identity_document': {}, 'capability_sha256': bytea(sha('capabilities')),
            }])
            run, observation = str(uuid.uuid4()), str(uuid.uuid4())
            client.call('start_run', {'run_id': run, 'provenance_id': provenance,
                                     'external_run_key': 'before-enrichment',
                                     'source_revision': 'b' * 40, 'run_kind': 'native'})
            client.call('ingest', str(uuid.uuid4()), [{
                'observation_id': observation, 'repository_id': repository, 'run_id': run,
                'provenance_id': provenance, 'fixture_id': old_ids['test/modern.js'],
                'variant': 'non-strict', 'outcome': 'pass', 'phase': 'runtime',
                'started_at': '2026-10-21T00:00:00Z', 'finished_at': '2026-10-21T00:00:01Z',
                'active_ms': 1000, 'source_kind': 'live', 'payload': {'original': True},
            }])
            before = {table: client.rows(table, **filters) for table, filters in (
                ('fixtures', {'corpus_id': old_corpus}), ('corpora', {'corpus_id': old_corpus}),
                ('observations', {'run_id': run}), ('observation_payloads', {'observation_id': observation}),
            )}
            unavailable = client.db.execute("""
                SELECT reference_state,metadata_source FROM test262_reporting.v_fixture_specification_references
                WHERE repository_id=%s AND corpus_id=%s
                """, (repository, old_corpus)).fetchall()
            self.assertEqual(unavailable, [('unresolved', 'unavailable')] * fixture_count)
            args = SimpleNamespace(corpus=old_corpus, root=root)
            for _ in range(2):
                self.assertEqual(enrich_specification_metadata(client, args)['enriched_fixtures'], fixture_count)
            after = {table: client.rows(table, **filters) for table, filters in (
                ('fixtures', {'corpus_id': old_corpus}), ('corpora', {'corpus_id': old_corpus}),
                ('observations', {'run_id': run}), ('observation_payloads', {'observation_id': observation}),
            )}
            self.assertEqual(after, before)
            enriched_report = client.db.execute("""
                SELECT upstream_path,reference_state,raw_identifiers,metadata_source,upstream_revision,content_sha256
                FROM test262_reporting.v_fixture_specification_references
                WHERE repository_id=%s AND corpus_id=%s ORDER BY upstream_path
                """, (repository, old_corpus)).fetchall()
            self.assertEqual([r[:3] for r in enriched_report], [r[:3] for r in new_report])
            for row in enriched_report:
                self.assertEqual((row[3], row[4]), ('enrichment', revision))
                self.assertEqual(row[5], hashlib.sha256((root / row[0]).read_bytes()).hexdigest())
            annotations = client.rows('fixture_specification_metadata')
            self.assertEqual(len(annotations), fixture_count)
            self.assertEqual({r['actor'] for r in annotations}, {role})
            self.assertEqual(list(client.read('fixture_specification_metadata', page=1)), annotations)
            with self.assertRaises(psycopg.errors.InsufficientPrivilege):
                client.db.execute('SELECT * FROM test262.fixture_specification_metadata')
            annotation = next(r for r in annotations if r['fixture_id'] == old_ids['test/modern.js'])
            payload = {key: annotation[key] for key in (
                'fixture_id', 'upstream_revision', 'upstream_path', 'content_sha256', 'metadata', 'metadata_sha256')}
            with self.assertRaises(psycopg.errors.RaiseException):
                client.put('fixture_specification_metadata', [dict(payload, metadata={})])
            for patch in ({'upstream_revision': 'c' * 40}, {'upstream_path': 'test/wrong.js'},
                          {'content_sha256': bytea('d' * 64)}, {'metadata_sha256': bytea('e' * 64)}):
                with self.subTest(patch=patch), self.assertRaises(psycopg.errors.CheckViolation):
                    client.put('fixture_specification_metadata', [
                        dict(payload, fixture_id=new_ids['test/modern.js'], **patch)])
            with self.assertRaises(psycopg.errors.InsufficientPrivilege):
                client.put('fixture_specification_metadata', [dict(payload, fixture_id=self.fixtures[0])])
            self.db.execute("UPDATE test262.api_subjects SET trust_class='legacy' WHERE login_name=%s", (role,))
            try:
                with self.assertRaises(psycopg.errors.InsufficientPrivilege):
                    client.put('fixture_specification_metadata', [payload])
            finally:
                self.db.execute("UPDATE test262.api_subjects SET trust_class='trusted' WHERE login_name=%s", (role,))
            for statement in ('UPDATE test262.fixture_specification_metadata SET metadata=metadata WHERE fixture_id=%s',
                              'DELETE FROM test262.fixture_specification_metadata WHERE fixture_id=%s'):
                with self.assertRaises(psycopg.errors.CheckViolation):
                    self.db.execute(statement, (payload['fixture_id'],))
            with self.worker() as other:
                invisible = other.execute('SELECT test262.api_read(1,%s,%s,NULL,10)',
                                          ('fixture_specification_metadata', Jsonb({}))).fetchone()[0]
                self.assertEqual(invisible['rows'], [])
            output = root / 'export.ndjson'
            summary = client.export(output)
            records = [json.loads(line) for line in output.read_text().splitlines()]
            self.assertEqual(summary['counts']['fixture_specification_metadata'], fixture_count)
            exported = [r['row'] for r in records if r.get('table') == 'fixture_specification_metadata']
            self.assertEqual(sorted(exported, key=lambda r: r['fixture_id']),
                             sorted(annotations, key=lambda r: r['fixture_id']))
            exported_fixtures = {r['row']['fixture_id']: r['row'] for r in records if r.get('table') == 'fixtures'}
            self.assertEqual(exported_fixtures[new_ids['test/modern.js']]['metadata']['esid'], 'sec-example')
            self.assertEqual(exported_fixtures[new_ids['test/legacy.js']]['metadata']['es5id'], '10.5-1-s')
            self.assertEqual(exported_fixtures[new_ids['test/absent.js']]['metadata']['specification']['reference_state'], 'absent')
            for name, (identifiers, state) in expected_references.items():
                metadata = exported_fixtures[new_ids[f'test/{name}.js']]['metadata']
                self.assertEqual(metadata['specification']['raw_identifiers'], identifiers)
                self.assertEqual(metadata['specification']['reference_state'], state)
                enriched = next(r for r in exported if r['fixture_id'] == old_ids[f'test/{name}.js'])
                self.assertEqual(enriched['metadata']['specification'], metadata['specification'])
                self.assertEqual(enriched['metadata_sha256'], bytea(sha(enriched['metadata'], database_numbers=True)))
            self.assertEqual(next(r['row'] for r in records if r.get('table') == 'observations'),
                             before['observations'][0])

            # Existing buggy extraction is immutable, not silently replaced by the fixed reader.
            previous_row = copy.deepcopy(next(r for r in rows if r['path'] == 'test/legacy-number.js'))
            previous_row['metadata']['es6id'] = '19.1'
            previous_row['metadata']['specification']['raw_identifiers'] = {'es6id': '19.1'}
            previous_row['metadata']['specification']['reference_state'] = 'legacy'
            previous_corpus, previous_ids = register(client, repository, upstream, [previous_row])
            previous_fixture = previous_ids[previous_row['path']]
            previous_metadata = {key: previous_row['metadata'][key] for key in (
                'description', 'info', 'author', 'esid', 'es5id', 'es6id', 'specification')}
            client.put('fixture_specification_metadata', [{
                'fixture_id': previous_fixture, 'upstream_revision': revision,
                'upstream_path': previous_row['path'], 'content_sha256': bytea(previous_row['sha256']),
                'metadata': previous_metadata, 'metadata_sha256': bytea(sha(previous_metadata)),
            }])
            previous_annotation = client.one('fixture_specification_metadata', fixture_id=previous_fixture)
            previous_inventory = client.rows('fixtures', corpus_id=previous_corpus)
            with self.assertRaisesRegex(psycopg.errors.RaiseException, 'Conflicting immutable replay'):
                enrich_specification_metadata(client, SimpleNamespace(corpus=previous_corpus, root=root))
            self.assertEqual(client.one('fixture_specification_metadata', fixture_id=previous_fixture),
                             previous_annotation)
            self.assertEqual(client.rows('fixtures', corpus_id=previous_corpus), previous_inventory)
            self.assertEqual(client.rows('observations', run_id=run), before['observations'])

            staging_corpus, staging_fixture = str(uuid.uuid4()), str(uuid.uuid4())
            client.put('corpora', [{
                'corpus_id': staging_corpus, 'upstream_url': upstream['cloneUrl'], 'revision': revision,
                'revision_algorithm': 'sha1', 'inventory_digest': bytea(sha('staging')), 'expected_fixture_count': 1,
            }])
            client.put('repository_corpora', [{'repository_id': repository, 'corpus_id': staging_corpus}])
            client.put('fixtures', [{
                'fixture_id': staging_fixture, 'corpus_id': staging_corpus, 'upstream_path': payload['upstream_path'],
                'content_sha256': payload['content_sha256'], 'metadata_state': 'valid',
            }])
            with self.assertRaises(psycopg.errors.CheckViolation):
                client.put('fixture_specification_metadata', [dict(payload, fixture_id=staging_fixture)])

    def test_overlapping_mvp_snapshots_preserve_old_run_and_replay_outbox(self):
        """Real restricted APIs: old initialization revision must not block older snapshots."""
        import sqlite3, tempfile
        from types import SimpleNamespace
        from unittest.mock import patch
        from scripts.test262.central.client import Client, Outbox, identity, sha
        from scripts.test262.central.importer import import_snapshot, start_run
        from psycopg.conninfo import make_conninfo
        repository = str(uuid.uuid4())
        self.db.execute("INSERT INTO test262.repositories(repository_id,provider,provider_repository_id,canonical_name) VALUES(%s,'github','import-replay','test/import-replay')",(repository,))
        producer = str(uuid.uuid4())
        role = 'catalogue_test_importer'
        self.db.execute('CREATE ROLE catalogue_test_importer LOGIN INHERIT; GRANT test262_coordinator TO catalogue_test_importer')
        self.db.execute("INSERT INTO test262.producers(producer_id,repository_id,kind,display_name,credential_subject) VALUES(%s,%s,'local','import test',%s)", (producer,repository,role))
        self.db.execute("INSERT INTO test262.api_subjects VALUES(%s,%s,%s,'coordinator','legacy',true)",(role,repository,producer))
        self.db.execute("UPDATE test262.schema_contract SET deployment_state='shadow'")
        self.db.execute("ALTER ROLE catalogue_test_importer PASSWORD 'disposable-import-test'")
        client = Client(make_conninfo(DSN,user=role,password='disposable-import-test'), 1)
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
                        db.execute('INSERT INTO fixtures VALUES(?,?,?,?,?,?)',('shared','test/import-replay.js','d'*64,'["strict"]','runnable','[]'))
                        db.execute('INSERT INTO results VALUES(?,?,?,?,?,?)',('shared','test/import-replay.js','strict','{}',1,'matched'))
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

