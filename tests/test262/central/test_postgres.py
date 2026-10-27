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
    def test_reconciliation_relationship_contract_matches_deployed_schema(self):
        from scripts.test262.central.reconciliation import CONTRACT
        rows=self.db.execute("""SELECT child.relname,parent.relname,
          ARRAY(SELECT attname FROM unnest(k.conkey) WITH ORDINALITY a(num,pos)
                JOIN pg_attribute p ON p.attrelid=k.conrelid AND p.attnum=a.num ORDER BY a.pos),
          ARRAY(SELECT attname FROM unnest(k.confkey) WITH ORDINALITY a(num,pos)
                JOIN pg_attribute p ON p.attrelid=k.confrelid AND p.attnum=a.num ORDER BY a.pos)
          FROM pg_constraint k JOIN pg_class child ON child.oid=k.conrelid
          JOIN pg_namespace n ON n.oid=child.relnamespace JOIN pg_class parent ON parent.oid=k.confrelid
          WHERE n.nspname='test262' AND k.contype='f'""").fetchall()
        actual={(child,tuple(columns),parent,tuple(parent_columns))
                for child,parent,columns,parent_columns in rows if child in CONTRACT}
        expected={(child,tuple(r['columns']),r['parent'],tuple(r['parent_columns']))
                  for child,spec in CONTRACT.items() for r in spec['references']}
        self.assertEqual(actual,expected)

    def test_operational_receipts_warn_recover_and_keep_unknown_distinct(self):
        from datetime import datetime, timezone, timedelta
        now=datetime.now(timezone.utc)
        check=dict(fetch_state='observed',latest_run_id=123,latest_attempt=1,latest_state='completed',
            latest_conclusion='failure',success_run_id=122,success_attempt=1,
            success_finished_at=now.isoformat(),receipt_verified=True,retention_verified=True,
            retention_until=(now+timedelta(days=35)).isoformat())
        document=dict(master_revision='b'*40,master_committed_at=(now-timedelta(hours=2)).isoformat(),checks={'backup':check})
        def signal():
            return self.db.execute("SELECT health_state,breach FROM test262_reporting.v_operational_signals WHERE repository_id=%s AND signal='backup-health'",(self.repo,)).fetchone()
        with self.worker() as db:
            sample=str(uuid.uuid4())
            first=db.execute('SELECT test262.api_monitor(1,%s,%s)',(sample,Jsonb(document))).fetchone()[0]
            replay=db.execute('SELECT test262.api_monitor(1,%s,%s)',(sample,Jsonb(document))).fetchone()[0]
            self.assertEqual(first['observed_at'],replay['observed_at'])
            self.assertTrue(replay['replayed'])
            self.assertEqual(signal(),('warning',1))
            with self.assertRaises(psycopg.Error):
                db.execute('SELECT test262.api_monitor(1,%s,%s)',(sample,Jsonb(dict(document,master_revision='c'*40))))
            for patch,expected in [({'latest_conclusion':'success'},('ok',0)),
                    ({'receipt_verified':None},('unknown',None)),
                    ({'receipt_verified':True,'retention_until':(now-timedelta(hours=1)).isoformat()},('warning',1)),
                    ({'retention_until':(now+timedelta(days=35)).isoformat()},('ok',0))]:
                check.update(patch)
                db.execute('SELECT test262.api_monitor(1,%s,%s)',(str(uuid.uuid4()),Jsonb(document)))
                self.assertEqual(signal(),expected)
            with self.assertRaises(psycopg.Error):
                db.execute('SELECT test262.api_monitor(1,%s,%s)',(str(uuid.uuid4()),Jsonb(dict(document,password='must-not-store'))))
            with self.assertRaises(psycopg.errors.InsufficientPrivilege):
                db.execute('INSERT INTO test262.monitor_samples(sample_id,repository_id,producer_id,document) VALUES(%s,%s,%s,%s)',(str(uuid.uuid4()),self.repo,self.producer,Jsonb(document)))
        for role in ('test262_ingest','test262_worker','test262_reporter','anon','authenticated','service_role'):
            self.assertFalse(self.db.execute("SELECT has_function_privilege(%s,'test262.api_monitor(bigint,uuid,jsonb)','EXECUTE')",(role,)).fetchone()[0])
        self.assertTrue(self.db.execute("SELECT has_table_privilege('test262_reporter','test262_reporting.v_operational_signals','SELECT')").fetchone()[0])
        self.assertFalse(self.db.execute("SELECT has_table_privilege('test262_reporter','test262.monitor_samples','SELECT')").fetchone()[0])

    def test_execution_rate_separates_deliberate_recovery_unclassified_and_uninstrumented(self):
        from datetime import datetime, timezone, timedelta
        pid=str(uuid.uuid4());run=str(uuid.uuid4());base=datetime.now(timezone.utc)-timedelta(days=1)
        self.db.execute("INSERT INTO test262.provenances(provenance_id,repository_id,corpus_id,evidence_kind,identity_version,identity_sha256,identity_document,capability_sha256) VALUES(%s,%s,%s,'native',1,sha256(%s::bytea),'{}',sha256('cap'::bytea))",(pid,self.repo,self.corpus,pid.encode()))
        with self.worker() as db:
            db.execute('SELECT test262.api_start_run(1,%s)',(Jsonb(dict(run_id=run,provenance_id=pid,external_run_key=run,source_revision='b'*40,run_kind='native')),))
        work={}
        def add(fixture,generation,index,outcome='pass',intentional=False,lease=1,unlinked=False):
            start=base+timedelta(minutes=index);wid=None
            if not unlinked:
                key=(fixture,generation)
                wid=work.get(key)
                if not wid:
                    wid=str(uuid.uuid4());work[key]=wid
                    self.db.execute("INSERT INTO test262.work_items(work_item_id,repository_id,provenance_id,fixture_id,variant,coherent_area,retry_generation,attempt_limit,selection_reason) VALUES(%s,%s,%s,%s,'strict','metrics',%s,4,%s)",(wid,self.repo,pid,self.fixtures[fixture],generation,Jsonb({'deliberate_retry':intentional})))
                self.db.execute("INSERT INTO test262.work_leases(work_item_id,lease_generation,repository_id,producer_id,run_id,claimed_at,expires_at,release_reason) VALUES(%s,%s,%s,%s,%s,%s,%s,%s)",(wid,lease,self.repo,self.producer,run,start,start+timedelta(seconds=31),'expired' if fixture==2 and lease==1 else 'complete'))
            self.db.execute("INSERT INTO test262.observations(observation_id,repository_id,run_id,provenance_id,fixture_id,variant,work_item_id,lease_generation,outcome,failure_class,phase,started_at,finished_at,active_ms,payload_sha256,source_kind) VALUES(%s,%s,%s,%s,%s,'strict',%s,%s,%s,%s,'execution',%s,%s,10,sha256('fixture'::bytea),'live')",(str(uuid.uuid4()),self.repo,run,pid,self.fixtures[fixture],wid,None if unlinked else lease,outcome,None if outcome=='pass' else 'unresolved',start,start+timedelta(milliseconds=10)))
        add(0,0,0);add(0,1,1);add(0,2,2,intentional=True)
        add(1,0,0,'fail');add(1,1,1,'fail')
        add(2,0,0);add(2,0,1,lease=2)
        add(3,0,0,unlinked=True)
        counts=dict(self.db.execute('SELECT execution_class,count(*) FROM test262_reporting.v_execution_classification WHERE provenance_id=%s GROUP BY execution_class',(pid,)).fetchall())
        self.assertEqual(counts,{'baseline':3,'avoidable-repeat':1,'intentional-retry':1,'unclassified-repeat':1,'lease-recovery':1})
        measured=self.db.execute('SELECT uninstrumented_observations,classification_complete FROM test262_reporting.v_duplicate_executions_7d WHERE repository_id=%s',(self.repo,)).fetchone()
        self.assertGreaterEqual(measured[0],1);self.assertFalse(measured[1])

    def test_execution_only_worker_claims_own_run_and_revocation_fences_live_session(self):
        from psycopg import sql
        name='independent_'+uuid.uuid4().hex
        producer,run,budget,item=[str(uuid.uuid4()) for _ in range(4)]
        self.db.execute(sql.SQL('CREATE ROLE {} LOGIN INHERIT').format(sql.Identifier(name)))
        self.db.execute(sql.SQL('GRANT test262_worker TO {}').format(sql.Identifier(name)))
        self.db.execute("INSERT INTO test262.producers(producer_id,repository_id,kind,display_name,credential_subject) VALUES(%s,%s,'local',%s,%s)",(producer,self.repo,name,name))
        self.db.execute("INSERT INTO test262.api_subjects VALUES(%s,%s,%s,'coordinator','trusted',true)",(name,self.repo,producer))
        self.db.execute("INSERT INTO test262.budget_scopes(budget_scope_id,repository_id,scope_kind,external_key,candidate_limit,accepted_limit,attempt_limit,time_limit_ms) VALUES(%s,%s,'batch',%s,1,1,2,2000)",(budget,self.repo,name))
        self.db.execute("INSERT INTO test262.work_items(work_item_id,repository_id,provenance_id,fixture_id,variant,coherent_area,attempt_limit,retry_generation) VALUES(%s,%s,%s,%s,'strict','independent',2,12345)",(item,self.repo,self.prov,self.fixtures[0]))
        self.db.execute('INSERT INTO test262.budget_work_items VALUES(%s,%s,%s)',(self.repo,budget,item))
        with psycopg.connect(DSN,autocommit=True) as db:
            db.execute(sql.SQL('SET SESSION AUTHORIZATION {}').format(sql.Identifier(name)))
            db.execute('SELECT test262.api_start_run(1,%s)',(Jsonb(dict(run_id=run,provenance_id=self.prov,external_run_key=name,source_revision='b'*40,run_kind='native')),))
            claim=db.execute('SELECT test262.api_claim(1,%s,%s,1000,31)',(run,budget)).fetchone()[0]
            self.assertEqual(claim['work_item_id'],item)
            self.assertEqual(claim['lease_owner'],producer)
            completed=self.complete_budget_work(db,run,claim)
            self.assertTrue(completed['completed'])
            db.execute("SELECT test262.api_metric(1,%s,'screening',1,10,1,0)",(run,))
            ended=db.execute("SELECT test262.api_finish_run(1,%s,'complete',clock_timestamp(),%s)",(run,Jsonb(dict(source='supervisor',external_run_key=name,source_revision='b'*40,reason='collection-finished',flush_verified=True)))).fetchone()[0]
            self.assertEqual(ended['state'],'complete')
            for statement in ("SELECT test262.api_put(1,'work_items','[]')",
                              "SELECT test262.api_transition(1,'reporting_targets','{}',0,'{}')",
                              "SELECT * FROM test262.observations"):
                with self.assertRaises(psycopg.errors.InsufficientPrivilege):
                    db.execute(statement)
            self.assertFalse(self.db.execute("SELECT pg_has_role(%s,'test262_coordinator','MEMBER')",(name,)).fetchone()[0])
            self.db.execute('UPDATE test262.api_subjects SET enabled=false WHERE login_name=%s',(name,))
            with self.assertRaises(psycopg.errors.InsufficientPrivilege):
                db.execute("SELECT test262.api_read(1,'runs','{}',NULL,1)")

    def budget_fixture(self, count=1, attempts=5, milliseconds=5000):
        run,budget,provenance=[str(uuid.uuid4()) for _ in range(3)]
        key='actions:'+str(uuid.uuid4().int)
        self.db.execute("INSERT INTO test262.provenances(provenance_id,repository_id,corpus_id,evidence_kind,identity_version,identity_sha256,identity_document,capability_sha256) VALUES(%s,%s,%s,'native',1,sha256(%s::bytea),'{}',sha256('cap'::bytea))",(provenance,self.repo,self.corpus,provenance.encode()))
        self.db.execute("INSERT INTO test262.budget_scopes(budget_scope_id,repository_id,scope_kind,external_key,candidate_limit,accepted_limit,attempt_limit,time_limit_ms) VALUES(%s,%s,'run',%s,10,10,%s,%s)",(budget,self.repo,key,attempts,milliseconds))
        for fixture in self.fixtures[:count]:
            work=str(uuid.uuid4())
            self.db.execute("INSERT INTO test262.work_items(work_item_id,repository_id,provenance_id,fixture_id,variant,coherent_area,attempt_limit) VALUES(%s,%s,%s,%s,'strict','test',3)",(work,self.repo,provenance,fixture))
            self.db.execute('INSERT INTO test262.budget_work_items VALUES(%s,%s,%s)',(self.repo,budget,work))
        with self.worker() as db:
            db.execute('SELECT test262.api_start_run(1,%s)',(Jsonb(dict(run_id=run,provenance_id=provenance,external_run_key=key+':1',source_revision='c'*40,run_kind='native')),))
        return run,budget,provenance,key

    def end_budget_run(self, db, run, key, state='complete', attempt=1):
        if state=='complete':
            db.execute("SELECT test262.api_metric(1,%s,'screening',1,10,0,0)",(run,))
        proof=dict(source='supervisor',external_run_key=f'{key}:{attempt}',source_revision='c'*40,
                   reason='collection-finished' if state=='complete' else ('job-cancelled' if state=='cancelled' else 'job-failed'),
                   flush_verified=state=='complete')
        return db.execute('SELECT test262.api_finish_run(1,%s,%s,clock_timestamp(),%s)',(run,state,Jsonb(proof))).fetchone()[0]

    def complete_budget_work(self, db, run, claim, outcome='pass', ms=20):
        observation=str(uuid.uuid4())
        row=dict(observation_id=observation,repository_id=self.repo,run_id=run,provenance_id=claim['provenance_id'],
                 fixture_id=claim['fixture_id'],variant=claim['variant'],work_item_id=claim['work_item_id'],
                 lease_generation=claim['lease_generation'],outcome=outcome,phase='runtime',active_ms=ms,
                 started_at='2026-10-23T00:00:00Z',finished_at='2026-10-23T00:00:01Z',source_kind='live',payload={})
        db.execute('SELECT test262.api_ingest(1,%s,%s)',(str(uuid.uuid4()),Jsonb([row])))
        return db.execute('SELECT test262.api_complete(1,%s,%s,%s)',(claim['work_item_id'],claim['lease_generation'],observation)).fetchone()[0]

    def test_budget_drained_ignores_test_outcomes_and_replay_keeps_receipt(self):
        run,budget,_,key=self.budget_fixture()
        with self.worker() as db:
            claim=db.execute('SELECT test262.api_claim(1,%s,%s,1000,31)',(run,budget)).fetchone()[0]
            self.complete_budget_work(db,run,claim,outcome='fail')
            self.end_budget_run(db,run,key)
            dry=db.execute('SELECT test262.api_finish_budget(1,%s,false)',(budget,)).fetchone()[0]
            self.assertEqual((dry['state'],dry['reason']),('closed','drained'))
            self.assertEqual(self.db.execute('SELECT state FROM test262.budget_scopes WHERE budget_scope_id=%s',(budget,)).fetchone()[0],'open')
            result=db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]['result']
            again=db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]
            self.assertEqual(again['result'],result)
            self.assertEqual((result['charged_attempts'],result['charged_ms'],result['reserved_attempts']),(1,20,0))
        self.assertEqual(self.db.execute("SELECT count(*) FROM test262.audit_events WHERE action='finish-budget' AND details->'after'->>'budget_scope_id'=%s",(budget,)).fetchone()[0],1)

    def test_budget_live_lease_is_preserved_then_expiry_is_conservatively_settled_once(self):
        run,budget,_,key=self.budget_fixture()
        with self.worker() as db:
            claim=db.execute('SELECT test262.api_claim(1,%s,%s,1000,31)',(run,budget)).fetchone()[0]
            self.end_budget_run(db,run,key,state='failed')
            self.assertEqual(db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]['disposition'],'live-reservations')
            self.db.execute("UPDATE test262.work_items SET lease_expires_at=clock_timestamp()-interval '1 second' WHERE work_item_id=%s",(claim['work_item_id'],))
            self.db.execute("UPDATE test262.work_leases SET claimed_at=clock_timestamp()-interval '1 minute',expires_at=clock_timestamp()-interval '1 second' WHERE work_item_id=%s",(claim['work_item_id'],))
            dry=db.execute('SELECT test262.api_finish_budget(1,%s,false)',(budget,)).fetchone()[0]
            self.assertEqual((dry['state'],dry['charged_ms'],dry['expired_reservations']),('interrupted',1000,1))
            self.assertEqual(self.db.execute('SELECT reserved_attempts,charged_attempts FROM test262.budget_scopes WHERE budget_scope_id=%s',(budget,)).fetchone(),(1,0))
            result=db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]['result']
            self.assertEqual((result['charged_attempts'],result['charged_ms'],result['reserved_attempts']),(1,1000,0))
            self.assertEqual(db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]['result'],result)
            # Lost local evidence can still arrive after conservative settlement;
            # it is retained without replacing the charge or reviving the lease.
            self.assertTrue(self.complete_budget_work(db,run,claim)['retained_late_evidence'])
            self.assertEqual(self.db.execute('SELECT charged_ms FROM test262.budget_scopes WHERE budget_scope_id=%s',(budget,)).fetchone()[0],1000)

    def test_budget_retry_reopens_only_new_attempt_and_keeps_cumulative_limits(self):
        run,budget,provenance,key=self.budget_fixture(count=2,attempts=2,milliseconds=5000)
        with self.worker() as db:
            claim=db.execute('SELECT test262.api_claim(1,%s,%s,1000,31)',(run,budget)).fetchone()[0]
            self.complete_budget_work(db,run,claim)
            self.end_budget_run(db,run,key)
            self.assertEqual(db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]['state'],'interrupted')
            retry=str(uuid.uuid4())
            db.execute('SELECT test262.api_start_run(1,%s)',(Jsonb(dict(run_id=retry,provenance_id=provenance,external_run_key=key+':2',source_revision='c'*40,run_kind='native')),))
            self.assertEqual(db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]['disposition'],'active-or-unproven-run')
            with self.assertRaises(psycopg.Error):
                db.execute('SELECT test262.api_claim(1,%s,%s,2000,32)',(retry,budget))
            second=db.execute('SELECT test262.api_claim(1,%s,%s,1000,31)',(retry,budget)).fetchone()[0]
            self.complete_budget_work(db,retry,second)
            self.end_budget_run(db,retry,key,attempt=2)
            result=db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]['result']
            self.assertEqual((result['state'],result['charged_attempts'],result['charged_ms']),('closed',2,40))
            with self.assertRaises(psycopg.Error):
                db.execute('SELECT test262.api_claim(1,%s,%s,1000,31)',(run,budget))
        self.assertEqual(self.db.execute("SELECT count(*) FROM test262.audit_events WHERE action='reopen-budget' AND details->>'run_id'=%s",(retry,)).fetchone()[0],1)

    def test_budget_exhaustion_uses_cap_not_zero_reservations_and_cancellation_is_distinct(self):
        for state,expected,milliseconds in [('complete','exhausted',1000),('complete','interrupted',5000),('cancelled','cancelled',5000)]:
            run,budget,_,key=self.budget_fixture(count=2,milliseconds=milliseconds)
            with self.worker() as db:
                claim=db.execute('SELECT test262.api_claim(1,%s,%s,1000,31)',(run,budget)).fetchone()[0]
                self.complete_budget_work(db,run,claim)
                self.end_budget_run(db,run,key,state=state)
                result=db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]
                self.assertEqual(result['state'],expected)
                self.assertEqual(result['pending_work'],1)

    def test_budget_permissions_unproven_runs_and_foreign_scope_are_fenced(self):
        run,budget,_,key=self.budget_fixture(count=0)
        with self.worker() as db:
            self.assertEqual(db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]['disposition'],'active-or-unproven-run')
            with self.assertRaises(psycopg.Error):
                db.execute('SELECT test262.api_finish_budget(0,%s,true)',(budget,))
            unrelated,_,_,_=self.budget_fixture(count=0)
            with self.assertRaises(psycopg.Error):
                db.execute('SELECT test262.api_claim(1,%s,%s,1000,31)',(unrelated,budget))
            self.end_budget_run(db,run,key,state='failed')
            self.assertEqual(db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]['state'],'interrupted')
        for role in ('anon','authenticated','service_role','test262_ingest','test262_reporter'):
            self.assertFalse(self.db.execute("SELECT has_function_privilege(%s,'test262.api_finish_budget(bigint,uuid,boolean)','EXECUTE')",(role,)).fetchone()[0])

    def test_budget_shared_expired_work_waits_for_its_reservation_owner(self):
        run,budget,provenance,key=self.budget_fixture()
        other,other_budget=str(uuid.uuid4()),str(uuid.uuid4())
        other_key='actions:'+str(uuid.uuid4().int)
        self.db.execute("INSERT INTO test262.budget_scopes(budget_scope_id,repository_id,scope_kind,external_key,candidate_limit,accepted_limit,attempt_limit,time_limit_ms) VALUES(%s,%s,'run',%s,10,10,5,5000)",(other_budget,self.repo,other_key))
        self.db.execute('INSERT INTO test262.budget_work_items SELECT repository_id,%s,work_item_id FROM test262.budget_work_items WHERE budget_scope_id=%s',(other_budget,budget))
        with self.worker() as db:
            self.end_budget_run(db,run,key)
            db.execute('SELECT test262.api_start_run(1,%s)',(Jsonb(dict(run_id=other,provenance_id=provenance,external_run_key=other_key+':1',source_revision='c'*40,run_kind='native')),))
            claim=db.execute('SELECT test262.api_claim(1,%s,%s,1000,31)',(other,other_budget)).fetchone()[0]
            self.assertEqual(db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]['disposition'],'live-reservations')
            self.db.execute("UPDATE test262.work_items SET lease_expires_at=clock_timestamp()-interval '1 second' WHERE work_item_id=%s",(claim['work_item_id'],))
            self.db.execute("UPDATE test262.work_leases SET claimed_at=clock_timestamp()-interval '1 minute',expires_at=clock_timestamp()-interval '1 second' WHERE work_item_id=%s",(claim['work_item_id'],))
            self.assertEqual(db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]['disposition'],'awaiting-shared-work-settlement')
            self.assertEqual(self.db.execute('SELECT reserved_attempts,charged_attempts FROM test262.budget_scopes WHERE budget_scope_id=%s',(other_budget,)).fetchone(),(1,0))
            self.end_budget_run(db,other,other_key,state='failed')
            self.assertEqual(db.execute('SELECT test262.api_finish_budget(1,%s,true)',(other_budget,)).fetchone()[0]['result']['charged_ms'],1000)
            self.assertEqual(db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]['state'],'interrupted')

    def test_budget_concurrent_finalizers_commit_one_terminal_receipt(self):
        run,budget,_,key=self.budget_fixture(count=0)
        with self.worker() as db:
            self.end_budget_run(db,run,key)
        def finish(_):
            with self.worker() as db:
                return db.execute('SELECT test262.api_finish_budget(1,%s,true)',(budget,)).fetchone()[0]
        with ThreadPoolExecutor(max_workers=2) as pool:
            results=list(pool.map(finish,range(2)))
        self.assertEqual(sorted(r['disposition'] for r in results),['terminal','unchanged'])
        self.assertEqual(results[0]['result'],results[1]['result'])

    @classmethod
    def setUpClass(cls):
        cls.db=psycopg.connect(DSN,autocommit=True)
        if cls.db.execute("SELECT to_regclass('public.perf_results')").fetchone()[0]:
            raise RuntimeError('Refusing production/performance database')
        cls.db.execute('CREATE ROLE anon; CREATE ROLE authenticated; CREATE ROLE service_role;')
        reporting_migration=ROOT/'supabase/migrations/20261027230000_test262_import_reporting_performance.sql'
        migrations=set((ROOT/'supabase/migrations').glob('*test262_catalogue*.sql')) | {reporting_migration}
        for migration in sorted(migrations):
            if migration == reporting_migration:
                cls.import_reporting_contract=cls.db.execute("""
                    SELECT relacl,reloptions FROM pg_class
                    WHERE oid='test262_reporting.v_catalogue_imports'::regclass
                """).fetchone()
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
                        # Ownership joins may hash/deduplicate their key set. The
                        # exported rows themselves must not be sorted/aggregated
                        # into pages or wrapped in a LIMIT/window operation.
                        self.assertNotIn(plans[0]['Plan']['Node Type'],
                                         ('Limit', 'WindowAgg', 'Aggregate', 'Sort'), plans[0])
                        for fragment in ('jsonb_agg(', 'row_number(', 'OFFSET', 'LIMIT'):
                            self.assertNotIn(fragment, plans[0]['Query Text'])
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
        late_provenance,late_run,late_observation = (str(uuid.uuid4()) for _ in range(3))
        inserted = []

        def concurrent_insert(table):
            with closing(original(table)) as rows:
                for row in rows:
                    if not inserted:
                        with self.db.transaction():
                            self.db.execute("""INSERT INTO test262.budget_scopes(budget_scope_id,repository_id,scope_kind,external_key,candidate_limit,accepted_limit,attempt_limit,time_limit_ms)
                                VALUES(%s,%s,'run',%s,1,1,1,1000)""", (late, self.repo, late))
                            self.db.execute("""INSERT INTO test262.provenances(provenance_id,repository_id,corpus_id,evidence_kind,identity_version,identity_sha256,identity_document,capability_sha256)
                              VALUES(%s,%s,%s,'native',1,sha256(%s::bytea),'{}',sha256('cap'::bytea))""",(late_provenance,self.repo,self.corpus,late_provenance.encode()))
                            self.db.execute("""INSERT INTO test262.runs(run_id,repository_id,producer_id,provenance_id,external_run_key,source_revision,run_kind,trust_class)
                              VALUES(%s,%s,%s,%s,%s,repeat('b',40),'native','trusted')""",(late_run,self.repo,self.producer,late_provenance,late_run))
                            self.db.execute("""INSERT INTO test262.observations(observation_id,repository_id,run_id,provenance_id,fixture_id,variant,outcome,phase,started_at,finished_at,active_ms,payload_sha256,source_kind)
                              VALUES(%s,%s,%s,%s,%s,'strict','pass','execution',now(),now(),1,sha256('late'::bytea),'live')""",(late_observation,self.repo,late_run,late_provenance,self.fixtures[0]))
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
            for table,key,value in [('provenances','provenance_id',late_provenance),('runs','run_id',late_run),('observations','observation_id',late_observation)]:
                self.assertNotIn(value,{r['row'][key] for r in exported if r['table']==table})
                self.assertIn(value,{r[key] for r in client.rows(table)})
            from scripts.test262.central.reconciliation import reconcile
            report=reconcile(output,summary)
            self.assertFalse(any(report['relationship_checks'].values()))
            # A second serialization of this frozen input may reorder all rows.
            # The logical digest remains stable while the exact file SHA changes.
            from test_reconciliation import write_export
            reordered=Path(directory)/'reordered.ndjson'
            changed=write_export(reordered,[(r['table'],r['row']) for r in reversed(exported)],
                                 {k:summary[k] for k in ('repository_id','epoch','schema','api','snapshot_token','as_of')})
            self.assertEqual(report['logical_sha256'],reconcile(reordered,changed)['logical_sha256'])
            if os.getenv('TEST262_RECONCILIATION_REPORT'):
                target=Path(os.environ['TEST262_RECONCILIATION_REPORT']);target.parent.mkdir(parents=True,exist_ok=True)
                target.write_text(json.dumps(dict(report,disposable=True,concurrent_chain_excluded=True,
                  reordered_logical_digest_equal=True),indent=2)+'\n')
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
            # Backfill proofs must be identical through bulk cursors and older contracts.
            from scripts.test262.central.specification_backfill import annotations as verify_annotations, snapshot, KEYS
            expected = {r['fixture_id']: sha({k: v for k, v in r.items() if k not in ('actor', 'recorded_at')},
                                            database_numbers=True) for r in annotations}
            expected_proof = {r['fixture_id']: sha(r) for r in annotations}
            bulk_proof = verify_annotations(client, expected)
            self.assertEqual(bulk_proof, expected_proof)
            self.assertEqual(client.export_transport, 'bulk_cursor')
            proof = snapshot(client, old_corpus)
            for table, key in KEYS.items():
                filters = {'corpus_id': old_corpus} if table in ('corpora', 'fixtures') else {}
                self.assertEqual(proof['rows'][table], {r[key]: sha(r) for r in client.read(table, filters)})
            with self.assertRaises(ValueError):
                verify_annotations(client, dict(expected, **{next(iter(expected)): 'conflict'}))
            self.assertEqual(client.db.execute("SELECT count(*) FROM pg_cursors WHERE name LIKE 'test262_export_%'").fetchone()[0], 0)
            version = client.contract.pop('bulk_export_version')
            try:
                self.assertEqual(verify_annotations(client, expected), expected_proof)
                self.assertEqual(snapshot(client, old_corpus)['rows'], proof['rows'])
                self.assertEqual(client.export_transport, 'keyset_pages')
            finally:
                client.contract['bulk_export_version'] = version
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

    def previous_catalogue_imports(self):
        """Use the pinned original SQL, not a second hand-maintained count oracle."""
        original=(ROOT/'supabase/migrations/20261004083416_test262_catalogue_summary_reporting.sql').read_text()
        definition=original.split('CREATE VIEW test262_reporting.v_catalogue_imports',1)[1]
        definition=definition.split('CREATE VIEW test262_reporting.v_catalogue_outcomes',1)[0]
        self.db.execute('CREATE TEMP VIEW previous_catalogue_imports'+definition)

    def test_catalogue_imports_preserves_column_types_security_barrier_and_grants(self):
        with self.db.transaction(force_rollback=True):
            self.previous_catalogue_imports()
            def columns(view):
                return self.db.execute("""
                    SELECT attname,atttypid,atttypmod FROM pg_attribute
                    WHERE attrelid=%s::regclass AND attnum>0 AND NOT attisdropped ORDER BY attnum
                """,(view,)).fetchall()
            actual=columns('test262_reporting.v_catalogue_imports')
            self.assertEqual(len(actual),15)
            self.assertEqual(actual,columns('pg_temp.previous_catalogue_imports'))
            contract=self.db.execute("""
                SELECT relacl,reloptions FROM pg_class
                WHERE oid='test262_reporting.v_catalogue_imports'::regclass
            """).fetchone()
            self.assertEqual(contract,self.import_reporting_contract)
            self.assertIn('security_barrier=true',contract[1])
            self.assertTrue(self.db.execute("""
                SELECT has_table_privilege('test262_reporter',
                    'test262_reporting.v_catalogue_imports','SELECT')
            """).fetchone()[0])

    def test_catalogue_imports_preserves_previous_rows_and_observation_coverage(self):
        from datetime import datetime, timezone
        from psycopg.rows import dict_row
        started=datetime(2026,10,27,12,tzinfo=timezone.utc)
        complete='Observation coverage complete'
        partial='Partial or in progress'
        with self.db.transaction(force_rollback=True):
            self.previous_catalogue_imports()
            other,producer,provenance=[str(uuid.uuid4()) for _ in range(3)]
            self.db.execute("""
                INSERT INTO test262.repositories(repository_id,provider,provider_repository_id,canonical_name)
                VALUES(%s,'github',%s,'test/import-reporting-other')
            """,(other,other))
            self.db.execute('INSERT INTO test262.repository_corpora(repository_id,corpus_id) VALUES(%s,%s)',(other,self.corpus))
            self.db.execute("""
                INSERT INTO test262.producers(producer_id,repository_id,kind,display_name,credential_subject)
                VALUES(%s,%s,'importer','import reporting','import-reporting')
            """,(producer,other))
            self.db.execute("""
                INSERT INTO test262.provenances(provenance_id,repository_id,corpus_id,evidence_kind,
                    identity_version,identity_sha256,identity_document,capability_sha256)
                VALUES(%s,%s,%s,'native',1,sha256('import-reporting'::bytea),'{}',sha256('cap'::bytea))
            """,(provenance,other,self.corpus))
            uploaded=[]
            for repository,owner,pv,count in ((self.repo,self.producer,self.prov,3),(other,producer,provenance,1)):
                run=str(uuid.uuid4())
                self.db.execute("""
                    INSERT INTO test262.runs(run_id,repository_id,producer_id,provenance_id,
                        external_run_key,source_revision,run_kind,trust_class)
                    VALUES(%s,%s,%s,%s,%s,repeat('b',40),'import','legacy')
                """,(run,repository,owner,pv,run))
                for index in range(count):
                    observation=str(uuid.uuid4())
                    self.db.execute("""
                        INSERT INTO test262.observations(observation_id,repository_id,run_id,provenance_id,
                            fixture_id,variant,outcome,phase,started_at,finished_at,active_ms,payload_sha256,source_kind)
                        VALUES(%s,%s,%s,%s,%s,'strict','pass','runtime',%s,%s,0,
                            sha256('import-reporting'::bytea),'legacy-snapshot')
                    """,(observation,repository,run,pv,self.fixtures[index],started,started))
                    uploaded.append(observation)
            first,second,unmapped,foreign=uploaded
            missing=str(uuid.uuid4())
            # Each expected tuple is (expected, mapped, uploaded, reported, coverage).
            # The third local observation is deliberately unmapped.
            cases=[
                ('empty-unreported',self.repo,{}, {},[],(0,0,0,0,partial)),
                ('empty-reported-zero',self.repo,{'results':0,'attempts':7},{'observations':0},[],(0,0,0,0,complete)),
                ('expected-without-mappings',self.repo,{'results':1},{'observations':0},[],(1,0,0,0,partial)),
                ('partial-unmatched',self.repo,{'results':2},{'observations':1},[first,missing],(2,2,1,1,partial)),
                ('wrong-repository',self.repo,{'results':1},{'observations':1},[foreign],(1,1,0,1,partial)),
                ('non-observation',self.repo,{'results':0},{'observations':0},[],(0,0,0,0,complete)),
                ('complete-results',self.repo,{'results':2,'attempts':7},{'observations':0},[first,second],(2,2,2,0,complete)),
                ('complete-attempts-duplicate',self.repo,{'results':None,'attempts':2},{'observations':2},[first,first],(2,2,2,2,complete)),
                ('expected-count-mismatch',self.repo,{'results':3},{'observations':2},[first,second],(3,2,2,2,partial)),
                ('missing-reported-key',self.repo,{'attempts':2},{},[first,second],(2,2,2,0,partial)),
                ('other-repository-complete',other,{'attempts':1},{'observations':1},[foreign],(1,1,1,1,complete)),
            ]
            imports={}
            for label,repository,expected,reported,mappings,counts in cases:
                imported=str(uuid.uuid4())
                imports[label]=imported
                rich=label=='complete-results'
                limitations=({'artifact_history_complete':True,'missing_artifacts':['older-snapshot']}
                             if rich else {})
                self.db.execute("""
                    INSERT INTO test262.imports(import_id,repository_id,source_sha256,source_schema,source_uri,
                        state,expected_counts,imported_counts,limitations,started_at,completed_at)
                    VALUES(%s,%s,sha256(%s::bytea),%s,%s,%s,%s,%s,%s,%s,%s)
                """,(imported,repository,imported.encode(),'mvp' if rich else 'jroc','test:'+label,
                      'complete' if rich else 'incomplete',Jsonb(expected),Jsonb(reported),
                      Jsonb(limitations),started,started if rich else None))
                for index,entity in enumerate(mappings):
                    self.db.execute("""
                        INSERT INTO test262.import_records(import_id,source_table,source_key,entity_kind,entity_id)
                        VALUES(%s,'results',%s,'observation',%s)
                    """,(imported,str(index),entity))
                if label=='non-observation':
                    # Even an existing observation UUID must not count for another entity kind.
                    self.db.execute("""
                        INSERT INTO test262.import_records VALUES(%s,'fixtures','0','fixture',%s)
                    """,(imported,first))
                if rich:
                    for table,key,document in (
                            ('artifact_source','z',{'uri':'test:later-source'}),
                            ('artifact_source','a',{'uri':'test:first-source'}),
                            ('mvp_import_checkpoint','one',{'observations':1}),
                            ('mvp_import_checkpoint','two',{'observations':1}),
                            ('settings','schema',{'value':'legacy'})):
                        self.db.execute('INSERT INTO test262.legacy_control_records VALUES(%s,%s,%s,%s)',
                                        (imported,table,key,Jsonb(document)))
            with self.db.cursor(row_factory=dict_row) as cursor:
                # One statement preserves exact as_of parity; no timestamp fields are discarded.
                cursor.execute("""
                    SELECT 'current' AS definition,v.* FROM test262_reporting.v_catalogue_imports v
                    WHERE import_id=ANY(%s::uuid[])
                    UNION ALL
                    SELECT 'previous' AS definition,v.* FROM pg_temp.previous_catalogue_imports v
                    WHERE import_id=ANY(%s::uuid[])
                """,(list(imports.values()),list(imports.values())))
                rows=cursor.fetchall()
            current=[{k:v for k,v in row.items() if k!='definition'} for row in rows if row['definition']=='current']
            previous=[{k:v for k,v in row.items() if k!='definition'} for row in rows if row['definition']=='previous']
            self.assertEqual(len(current),len(cases))
            self.assertEqual(sorted(current,key=lambda r:r['import_id']),sorted(previous,key=lambda r:r['import_id']))
            for label,repository,expected,reported,mappings,counts in cases:
                with self.subTest(case=label):
                    matches=[row for row in current if str(row['import_id'])==imports[label]]
                    self.assertEqual(len(matches),1)
                    row=matches[0]
                    self.assertEqual(tuple(row[k] for k in ('expected_observations','mapped_observations',
                        'uploaded_observations','reported_observations','coverage')),counts)
                    rich=label=='complete-results'
                    self.assertEqual((str(row['repository_id']),row['source_schema'],row['import_state'],row['started_at']),
                                     (repository,'mvp' if rich else 'jroc','complete' if rich else 'incomplete',started))
                    self.assertEqual((row['history_complete'],row['completed_provenances'],
                                      row['source_artifact'],row['history_limitations']),
                                     (True,2,'test:first-source',['older-snapshot']) if rich else (False,0,None,None))

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



    def test_collection_lifecycle_owner_epoch_replay_and_immutable_terminal_state(self):
        run = str(uuid.uuid4())
        external = 'actions:123456:2'
        evidence = {'source':'github-actions','external_run_key':external,'source_revision':'b'*40,
                    'reason':'collection-finished','flush_verified':True}
        with self.worker() as db:
            db.execute('SELECT test262.api_start_run(1,%s)', (Jsonb({'run_id':run,'provenance_id':self.prov,
                       'external_run_key':external,'source_revision':'b'*40,'run_kind':'native'}),))
            end = self.db.execute('SELECT clock_timestamp()').fetchone()[0]
            # Successful Actions status alone cannot establish durable screening completion.
            with self.assertRaises(psycopg.errors.RaiseException):
                db.execute('SELECT test262.api_finish_run(1,%s,%s,%s,%s)',(run,'complete',end,Jsonb(evidence)))
            db.execute("SELECT test262.api_metric(1,%s,'screening',1,10,0,0)",(run,))
            for epoch, proof in [(0,evidence),(1,dict(evidence,external_run_key='actions:123456:1')),
                                 (1,dict(evidence,flush_verified=False))]:
                with self.assertRaises(psycopg.Error):
                    db.execute('SELECT test262.api_finish_run(%s,%s,%s,%s,%s)',(epoch,run,'complete',end,Jsonb(proof)))
            stored=db.execute('SELECT test262.api_finish_run(1,%s,%s,%s,%s)',(run,'complete',end,Jsonb(evidence))).fetchone()[0]
            self.assertEqual(stored['state'],'complete')
            replay=db.execute("SELECT test262.api_finish_run(1,%s,'complete',clock_timestamp(),%s)",(run,Jsonb(evidence))).fetchone()[0]
            self.assertEqual(replay,stored)
            with self.assertRaises(psycopg.errors.SerializationFailure):
                db.execute('SELECT test262.api_finish_run(1,%s,%s,%s,%s)',(run,'failed',end,Jsonb(dict(evidence,reason='job-failed'))))
            # The old generic transition endpoint cannot bypass first-terminal-state fencing.
            with self.assertRaises(psycopg.errors.SerializationFailure):
                db.execute('SELECT test262.api_transition(1,%s,%s,0,%s)',('runs',Jsonb({'run_id':run}),Jsonb({'state':'running'})))
        self.assertEqual(self.db.execute("SELECT count(*) FROM test262.audit_events WHERE action='finish-run' AND details->>'run_id'=%s",(run,)).fetchone()[0],1)
        for role in ('anon','authenticated','service_role','test262_ingest','test262_reporter'):
            self.assertFalse(self.db.execute("SELECT has_function_privilege(%s,'test262.api_finish_run(bigint,uuid,text,timestamptz,jsonb)','EXECUTE')",(role,)).fetchone()[0])

    def test_collection_lifecycle_preserves_legacy_and_foreign_producer_runs(self):
        for kind,trust,producer in [('import','legacy',self.producer),('native','trusted',str(uuid.uuid4()))]:
            with self.db.transaction(force_rollback=True):
                if producer != self.producer:
                    self.db.execute("INSERT INTO test262.producers(producer_id,repository_id,kind,display_name,credential_subject) VALUES(%s,%s,'local','other','other')",(producer,self.repo))
                run=str(uuid.uuid4())
                self.db.execute("INSERT INTO test262.runs(run_id,repository_id,producer_id,provenance_id,external_run_key,source_revision,run_kind,trust_class,state) VALUES(%s,%s,%s,%s,%s,repeat('b',40),%s,%s,'running')",(run,self.repo,producer,self.prov,run,kind,trust))
                # Separate connection sees only committed rows; use the operator session's
                # authenticated identity inside this rollback-only fixture transaction.
                self.db.execute('SET LOCAL SESSION AUTHORIZATION catalogue_test_worker')
                with self.db.transaction():
                    with self.assertRaises(psycopg.errors.InsufficientPrivilege):
                        with self.db.transaction():
                            self.db.execute("SELECT test262.api_finish_run(1,%s,'failed',clock_timestamp(),%s)",(run,Jsonb({'source':'github-actions','external_run_key':run,'source_revision':'b'*40,'reason':'job-failed','flush_verified':False})))
                self.db.execute('RESET SESSION AUTHORIZATION')
                self.assertEqual(self.db.execute('SELECT state,ended_at FROM test262.runs WHERE run_id=%s',(run,)).fetchone(),('running',None))

    def test_collection_lifecycle_failed_cancelled_and_incomplete_states(self):
        for state,reason in [('failed','job-failed'),('cancelled','job-cancelled'),('incomplete','job-interrupted')]:
            with self.worker() as db:
                run=str(uuid.uuid4())
                db.execute('SELECT test262.api_start_run(1,%s)',(Jsonb({'run_id':run,'provenance_id':self.prov,'external_run_key':run,'source_revision':'b'*40,'run_kind':'mvp'}),))
                result=db.execute('SELECT test262.api_finish_run(1,%s,%s,clock_timestamp(),%s)',(run,state,Jsonb({'source':'github-actions','external_run_key':run,'source_revision':'b'*40,'reason':reason,'flush_verified':False}))).fetchone()[0]
                self.assertEqual(result['state'],state)
                self.assertIsNotNone(result['ended_at'])


if __name__=='__main__':
    unittest.main()
