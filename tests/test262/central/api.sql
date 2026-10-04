-- Transaction-scoped synthetic rows only. Run after both migrations in a disposable database.
DO $test$
DECLARE r uuid=gen_random_uuid(); p uuid=gen_random_uuid(); c uuid=gen_random_uuid(); f uuid=gen_random_uuid(); pv uuid=gen_random_uuid(); run uuid=gen_random_uuid(); b uuid=gen_random_uuid(); w uuid=gen_random_uuid(); obs uuid=gen_random_uuid(); request uuid=gen_random_uuid(); claim jsonb; result jsonb; data jsonb; rejected boolean; page jsonb; other uuid=gen_random_uuid();
BEGIN
 INSERT INTO test262.repositories VALUES(r,'github','central-test-'||r,'test/central',now());
 INSERT INTO test262.producers(producer_id,repository_id,kind,display_name,credential_subject) VALUES(p,r,'local','synthetic',session_user);
 INSERT INTO test262.api_subjects VALUES(session_user,r,p,'coordinator','trusted',true);
 INSERT INTO test262.corpora(corpus_id,upstream_url,revision,revision_algorithm,inventory_digest,expected_fixture_count) VALUES(c,'https://test.invalid',repeat('a',40),'sha1',sha256(''::bytea),1);
 INSERT INTO test262.repository_corpora VALUES(r,c,now());
 PERFORM test262.api_put(1,'fixtures',jsonb_build_array(jsonb_build_object('fixture_id',f,'corpus_id',c,'upstream_path','test/central.js','content_sha256','\x'||repeat('b',64),'metadata_state','valid','dependency_manifest_digest','\x'||encode(sha256(''::bytea),'hex'))));
 PERFORM test262.api_put(1,'fixture_variants',jsonb_build_array(jsonb_build_object('fixture_id',f,'variant','strict','required',true)));
 UPDATE test262.corpora SET inventory_digest=test262.inventory_hash(c) WHERE corpus_id=c;
 PERFORM test262.api_transition(1,'corpora',jsonb_build_object('corpus_id',c),0,'{"inventory_state":"sealed"}');
 -- Sealed inventory replay is readable without trying to INSERT through its mutation guard.
 PERFORM test262.api_put(1,'fixtures',jsonb_build_array(jsonb_build_object('fixture_id',f,'corpus_id',c,'upstream_path','test/central.js','content_sha256','\x'||repeat('b',64),'metadata_state','valid','dependency_manifest_digest','\x'||encode(sha256(''::bytea),'hex'))));
 INSERT INTO test262.provenances(provenance_id,repository_id,corpus_id,evidence_kind,identity_version,identity_sha256,identity_document,capability_sha256) VALUES(pv,r,c,'native',1,sha256('native'::bytea),'{}',sha256('cap'::bytea));
 INSERT INTO test262.provenance_fixture_eligibility VALUES(r,pv,f,'runnable','{}','{}');
 PERFORM test262.api_start_run(1,jsonb_build_object('run_id',run,'provenance_id',pv,'source_revision',repeat('c',40),'external_run_key','synthetic','run_kind','native','trust_class','untrusted'));
 IF (SELECT trust_class FROM test262.runs WHERE run_id=run)<>'trusted' THEN RAISE EXCEPTION 'Caller assigned trust'; END IF;
 BEGIN PERFORM test262.api_start_run(1,jsonb_build_object('run_id',run,'provenance_id',pv,'source_revision',repeat('d',40),'external_run_key','synthetic','run_kind','native')); RAISE EXCEPTION 'Run conflict accepted'; EXCEPTION WHEN raise_exception THEN IF SQLERRM='Run conflict accepted' THEN RAISE; END IF; END;
 INSERT INTO test262.budget_scopes(budget_scope_id,repository_id,scope_kind,external_key,candidate_limit,accepted_limit,attempt_limit,time_limit_ms) VALUES(b,r,'run','synthetic',1,1,2,2000);
 INSERT INTO test262.work_items(work_item_id,repository_id,provenance_id,fixture_id,variant,coherent_area,attempt_limit) VALUES(w,r,pv,f,'strict','test',2);
 INSERT INTO test262.budget_work_items VALUES(r,b,w);
 UPDATE test262.schema_contract SET deployment_state='active';
 claim=test262.api_claim(1,run,b,1000,31);
 IF claim->>'work_item_id'<>w::text OR (SELECT reserved_attempts FROM test262.budget_scopes WHERE budget_scope_id=b)<>1 THEN RAISE EXCEPTION 'Claim/reservation mismatch'; END IF;
 IF test262.api_claim(1,run,b,1000,31) IS NOT NULL THEN RAISE EXCEPTION 'Duplicate claim'; END IF;
 data=jsonb_build_array(jsonb_build_object('observation_id',obs,'repository_id',r,'run_id',run,'provenance_id',pv,'fixture_id',f,'variant','strict','work_item_id',w,'lease_generation',1,'outcome','pass','phase','runtime','diagnostic_summary','','started_at',now(),'finished_at',now(),'active_ms',20,'source_kind','live','payload',jsonb_build_object('native',true)));
 result=test262.api_ingest(1,request,data);
 IF test262.api_ingest(1,request,data)<>result THEN RAISE EXCEPTION 'Request replay changed receipt'; END IF;
 PERFORM test262.api_ingest(1,gen_random_uuid(),data);
 IF (SELECT count(*) FROM test262.observations WHERE run_id=run)<>1 THEN RAISE EXCEPTION 'Observation replay duplicated'; END IF;
 BEGIN PERFORM test262.api_ingest(1,request,jsonb_set(data,'{0,active_ms}','21')); RAISE EXCEPTION 'Request collision accepted'; EXCEPTION WHEN raise_exception THEN IF SQLERRM='Request collision accepted' THEN RAISE; END IF; END;
 BEGIN PERFORM test262.api_ingest(1,gen_random_uuid(),jsonb_set(data,'{0,active_ms}','21')); RAISE EXCEPTION 'Observation collision accepted'; EXCEPTION WHEN raise_exception THEN IF SQLERRM='Observation collision accepted' THEN RAISE; END IF; END;
 PERFORM test262.api_complete(1,w,1,obs);
 PERFORM test262.api_complete(1,w,1,obs);
 IF (SELECT charged_attempts FROM test262.budget_scopes WHERE budget_scope_id=b)<>1 OR (SELECT charged_ms FROM test262.budget_scopes WHERE budget_scope_id=b)<>20 THEN RAISE EXCEPTION 'Budget double settlement'; END IF;
 BEGIN PERFORM test262.api_renew(1,w,1,30); RAISE EXCEPTION 'Completed lease renewed'; EXCEPTION WHEN serialization_failure THEN NULL; END;
 -- Bounded keyset reads: scoped, filtered, primary-key ordered, and resumable.
 INSERT INTO test262.repositories VALUES(other,'github','central-other-'||other,'test/other',now());
 INSERT INTO test262.reconciliation_state(repository_id,pipeline,channel,authority_epoch) VALUES(r,'native','master',1),(r,'native','pr',1),(other,'native','master',1);
 page=test262.api_read(1,'reconciliation_state','{}',NULL,1);
 IF jsonb_array_length(page->'rows')<>1 OR page->'next' IS NULL OR page->'next'->>'channel'<>'master' THEN RAISE EXCEPTION 'First page/cursor mismatch: %',page; END IF;
 page=test262.api_read(1,'reconciliation_state','{}',page->'next',1);
 IF page->'rows'->0->>'channel'<>'pr' THEN RAISE EXCEPTION 'Keyset continuation mismatch: %',page; END IF;
 IF test262.api_read(1,'reconciliation_state','{}',page->'next',1)->'rows'<>'[]'::jsonb THEN RAISE EXCEPTION 'Other repository rows leaked through read'; END IF;
 IF jsonb_array_length(test262.api_read(1,'reconciliation_state','{"channel":"pr"}',NULL,10)->'rows')<>1 THEN RAISE EXCEPTION 'Equality filter ignored'; END IF;
 IF jsonb_array_length(test262.api_read(1,'fixtures',jsonb_build_object('fixture_id',f),NULL,10)->'rows')<>1 THEN RAISE EXCEPTION 'Corpus-scoped read failed'; END IF;
 BEGIN PERFORM test262.api_read(1,'reconciliation_state','{"channel;--":"x"}',NULL,10); RAISE EXCEPTION 'Unknown filter column accepted'; EXCEPTION WHEN raise_exception THEN IF SQLERRM<>'Invalid filter column' THEN RAISE; END IF; END;
 BEGIN PERFORM test262.api_read(1,'reconciliation_state','{}',NULL,1001); RAISE EXCEPTION 'Unbounded page accepted'; EXCEPTION WHEN raise_exception THEN IF SQLERRM<>'Invalid page size' THEN RAISE; END IF; END;
 BEGIN PERFORM test262.api_read(1,'api_subjects','{}',NULL,10); RAISE EXCEPTION 'Credential table readable'; EXCEPTION WHEN raise_exception THEN IF SQLERRM<>'Unsupported record type' THEN RAISE; END IF; END;
 -- Authority epoch changes only through the audited cutover, never the writer API.
 BEGIN PERFORM test262.api_transition(1,'reconciliation_state',jsonb_build_object('repository_id',r,'pipeline','native','channel','master'),0,'{"authority_epoch":2}'); RAISE EXCEPTION 'Writer changed authority epoch'; EXCEPTION WHEN raise_exception THEN IF SQLERRM='Writer changed authority epoch' THEN RAISE; END IF; END;
 BEGIN PERFORM test262.api_put(1,'reconciliation_state',jsonb_build_array(jsonb_build_object('repository_id',r,'pipeline','native','channel','other','authority_epoch',2))); RAISE EXCEPTION 'Writer created foreign epoch'; EXCEPTION WHEN serialization_failure THEN NULL; END;
 UPDATE test262.api_subjects SET permission='ingest' WHERE login_name=session_user;
 BEGIN PERFORM test262.api_put(1,'fixtures','[]'); RAISE EXCEPTION 'Ingest obtained coordination'; EXCEPTION WHEN insufficient_privilege THEN NULL; END;
 BEGIN PERFORM test262.api_ingest(2,gen_random_uuid(),'[]'); RAISE EXCEPTION 'Stale epoch accepted'; EXCEPTION WHEN serialization_failure THEN NULL; END;
 IF has_table_privilege('test262_ingest','test262.observations','INSERT') OR has_function_privilege('test262_reporter','test262.api_ingest(bigint,uuid,jsonb)','EXECUTE') THEN RAISE EXCEPTION 'Privilege leak'; END IF;
END $test$;
