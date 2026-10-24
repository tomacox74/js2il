DO $native$
DECLARE s test262.api_subjects; c uuid=gen_random_uuid(); f uuid=gen_random_uuid(); pv uuid=gen_random_uuid(); run uuid=gen_random_uuid(); snap uuid=gen_random_uuid(); valid uuid=gen_random_uuid(); b uuid=gen_random_uuid(); batch uuid=gen_random_uuid(); o1 uuid=gen_random_uuid(); o2 uuid=gen_random_uuid(); untrusted uuid=gen_random_uuid(); fail_id uuid=gen_random_uuid(); obs jsonb;
BEGIN
 SELECT * INTO s FROM test262.api_subjects WHERE login_name=session_user;
 UPDATE test262.api_subjects SET permission='coordinator',trust_class='trusted' WHERE login_name=session_user;
 INSERT INTO test262.corpora(corpus_id,upstream_url,revision,revision_algorithm,inventory_digest,expected_fixture_count) VALUES(c,'native-test',repeat('f',40),'sha1',sha256(''::bytea),1);
 INSERT INTO test262.repository_corpora(repository_id,corpus_id) VALUES(s.repository_id,c);
 INSERT INTO test262.fixtures(fixture_id,corpus_id,upstream_path,content_sha256,metadata_state,dependency_manifest_digest) VALUES(f,c,'test/native.js',sha256('fixture'::bytea),'valid',sha256('[]'::bytea));
 INSERT INTO test262.fixture_variants(fixture_id,variant) VALUES(f,'strict'),(f,'non-strict');
 UPDATE test262.corpora SET inventory_digest=test262.inventory_hash(c),inventory_state='sealed' WHERE corpus_id=c;
 INSERT INTO test262.provenances(provenance_id,repository_id,corpus_id,evidence_kind,identity_version,identity_sha256,identity_document,capability_sha256) VALUES(pv,s.repository_id,c,'native',1,sha256('native-seal-test'::bytea),'{}',sha256('cap'::bytea));
 INSERT INTO test262.provenance_fixture_eligibility VALUES(s.repository_id,pv,f,'runnable','{}','{}');
 PERFORM test262.api_start_run(1,jsonb_build_object('run_id',run,'provenance_id',pv,'external_run_key','native-seal','source_revision',repeat('e',40),'run_kind','native'));
 INSERT INTO test262.registration_snapshots(snapshot_id,repository_id,source_revision,inventory_sha256,state) VALUES(snap,s.repository_id,repeat('e',40),sha256('[]'::bytea),'staging');
 PERFORM test262.api_transition(1,'registration_snapshots',jsonb_build_object('snapshot_id',snap),0,'{"state":"sealed"}');
 INSERT INTO test262.validation_events(validation_id,repository_id,workflow_identity,external_run_key,target_revision,conclusion,proof) VALUES(valid,s.repository_id,'synthetic','native-seal',repeat('e',40),'success','{}');
 INSERT INTO test262.reporting_targets VALUES(s.repository_id,'master','native',pv,snap,valid,0,now());
 INSERT INTO test262.budget_scopes(budget_scope_id,repository_id,scope_kind,external_key,candidate_limit,accepted_limit,attempt_limit,time_limit_ms) VALUES(b,s.repository_id,'batch','native-seal',1,1,10,100000);
 INSERT INTO test262.native_batches(batch_id,repository_id,batch_key,provenance_id,validation_id,registration_snapshot_id,budget_scope_id) VALUES(batch,s.repository_id,'native-seal',pv,valid,snap,b);
 INSERT INTO test262.batch_fixtures VALUES(batch,f,'accepted');
 obs=jsonb_build_object('observation_id',o1,'repository_id',s.repository_id,'run_id',run,'provenance_id',pv,'fixture_id',f,'variant','strict','outcome','pass','phase','execution','started_at',now(),'finished_at',now(),'active_ms',1,'source_kind','live','payload','{}'::jsonb);
 PERFORM test262.api_ingest(1,gen_random_uuid(),jsonb_build_array(obs));
 INSERT INTO test262.batch_evidence VALUES(batch,f,'strict',o1);
 IF NOT EXISTS(SELECT 1 FROM test262_reporting.v_current_fixture_status WHERE provenance_id=pv AND fixture_id=f
  AND status='incomplete' AND required_variant_count=2 AND recorded_variant_count=1 AND pass_variant_count=1) THEN
  RAISE EXCEPTION 'Partial fixture reporting changed'; END IF;
 BEGIN PERFORM test262.api_transition(1,'native_batches',jsonb_build_object('batch_id',batch),0,'{"state":"sealed"}'); RAISE EXCEPTION 'Incomplete variants accepted'; EXCEPTION WHEN check_violation THEN NULL; END;
 -- Untrusted evidence is neither acceptance nor a contradiction of trusted native execution.
 UPDATE test262.api_subjects SET trust_class='untrusted' WHERE login_name=session_user;
 PERFORM test262.api_start_run(1,jsonb_build_object('run_id',untrusted,'provenance_id',pv,'external_run_key','untrusted-native-seal','source_revision',repeat('e',40),'run_kind','native'));
 obs=obs||jsonb_build_object('observation_id',gen_random_uuid(),'run_id',untrusted,'variant','non-strict');
 PERFORM test262.api_ingest(1,gen_random_uuid(),jsonb_build_array(obs));
 BEGIN INSERT INTO test262.batch_evidence VALUES(batch,f,'non-strict',(obs->>'observation_id')::uuid); RAISE EXCEPTION 'Untrusted acceptance'; EXCEPTION WHEN check_violation THEN NULL; END;
 IF NOT EXISTS(SELECT 1 FROM test262_reporting.v_current_fixture_status WHERE provenance_id=pv AND fixture_id=f
  AND status='incomplete' AND recorded_variant_count=1) THEN RAISE EXCEPTION 'Untrusted report evidence counted'; END IF;
 UPDATE test262.api_subjects SET trust_class='trusted' WHERE login_name=session_user;
 -- A retryable infrastructure error before the trusted pass must not block acceptance.
 PERFORM test262.api_ingest(1,gen_random_uuid(),jsonb_build_array(obs||jsonb_build_object('observation_id',gen_random_uuid(),'run_id',run,'outcome','infrastructure-error','failure_class','infrastructure-error','phase','timeout')));
 PERFORM test262.api_ingest(1,gen_random_uuid(),jsonb_build_array(obs||jsonb_build_object('observation_id',gen_random_uuid(),'run_id',run,'outcome','incomplete')));
 obs=obs||jsonb_build_object('observation_id',o2,'run_id',run);
 PERFORM test262.api_ingest(1,gen_random_uuid(),jsonb_build_array(obs));
 INSERT INTO test262.batch_evidence VALUES(batch,f,'non-strict',o2);
 PERFORM test262.api_transition(1,'native_batches',jsonb_build_object('batch_id',batch),0,'{"state":"sealed"}');
 IF (SELECT state FROM test262.native_batches WHERE batch_id=batch)<>'sealed' THEN RAISE EXCEPTION 'Complete trusted native evidence not accepted'; END IF;
 PERFORM test262.api_ingest(1,gen_random_uuid(),jsonb_build_array(obs||jsonb_build_object('observation_id',gen_random_uuid(),'outcome','infrastructure-error','failure_class','infrastructure-error','phase','unknown')));
 IF (SELECT state FROM test262.native_batches WHERE batch_id=batch)<>'sealed' THEN RAISE EXCEPTION 'Late infrastructure error conflicted accepted batch'; END IF;
 obs=obs||jsonb_build_object('observation_id',fail_id,'outcome','fail','failure_class','semantic-defect');
 PERFORM test262.api_ingest(1,gen_random_uuid(),jsonb_build_array(obs));
 IF (SELECT state FROM test262.native_batches WHERE batch_id=batch)<>'conflicted' THEN RAISE EXCEPTION 'Late contradiction left batch sealed'; END IF;
 IF NOT EXISTS(SELECT 1 FROM test262_reporting.v_current_fixture_status WHERE provenance_id=pv AND fixture_id=f
  AND status='conflicted' AND conflict_flag AND required_variant_count=2 AND recorded_variant_count=2) THEN
  RAISE EXCEPTION 'Conflicted fixture reporting changed'; END IF;
 BEGIN INSERT INTO test262.publications(repository_id,batch_id,branch_name,patch_sha256,manifest_sha256) VALUES(s.repository_id,batch,'invalid',sha256('patch'::bytea),sha256('manifest'::bytea)); RAISE EXCEPTION 'Conflicted publication permitted'; EXCEPTION WHEN check_violation THEN NULL; END;
 BEGIN PERFORM test262.api_transition(1,'native_batches',jsonb_build_object('repository_id',s.repository_id),0,'{"state":"cancelled"}'); RAISE EXCEPTION 'Broad transition permitted'; EXCEPTION WHEN raise_exception THEN IF SQLERRM='Broad transition permitted' THEN RAISE; END IF; END;
END $native$;

-- Complete/invalidation reporting remains separate from native batch state.
DO $reporting$
DECLARE s test262.api_subjects; pv uuid=gen_random_uuid(); run uuid=gen_random_uuid();
 f uuid; cid uuid; snap uuid; valid uuid; o uuid=gen_random_uuid(); payload jsonb;
BEGIN
 SELECT * INTO s FROM test262.api_subjects WHERE login_name=session_user;
 SELECT t.registration_snapshot_id,t.validation_id,p.corpus_id INTO snap,valid,cid
  FROM test262.reporting_targets t JOIN test262.provenances p USING(provenance_id)
  WHERE t.repository_id=s.repository_id AND t.channel='master' AND t.evidence_kind='native';
 SELECT fixture_id INTO f FROM test262.fixtures WHERE corpus_id=cid AND upstream_path='test/native.js';
 INSERT INTO test262.provenances(provenance_id,repository_id,corpus_id,evidence_kind,identity_version,identity_sha256,identity_document,capability_sha256)
  VALUES(pv,s.repository_id,cid,'native',1,sha256('reporting-regression'::bytea),'{}',sha256('cap'::bytea));
 INSERT INTO test262.provenance_fixture_eligibility VALUES(s.repository_id,pv,f,'runnable','{}','{}');
 INSERT INTO test262.reporting_targets VALUES(s.repository_id,'operational-regression','native',pv,snap,valid,0,now());
 PERFORM test262.api_start_run(1,jsonb_build_object('run_id',run,'provenance_id',pv,'external_run_key','reporting-regression','source_revision',repeat('e',40),'run_kind','native'));
 payload=jsonb_build_object('observation_id',o,'repository_id',s.repository_id,'run_id',run,'provenance_id',pv,
  'fixture_id',f,'variant','strict','outcome','pass','phase','execution','started_at',now(),'finished_at',now(),'active_ms',1,'source_kind','live','payload','{}'::jsonb);
 PERFORM test262.api_ingest(1,gen_random_uuid(),jsonb_build_array(payload,payload||jsonb_build_object('observation_id',gen_random_uuid(),'variant','non-strict')));
 IF NOT EXISTS(SELECT 1 FROM test262_reporting.v_current_fixture_status WHERE provenance_id=pv AND channel='operational-regression'
  AND status='complete-pass' AND required_variant_count=2 AND recorded_variant_count=2 AND pass_variant_count=2) THEN
  RAISE EXCEPTION 'All-variant complete reporting changed'; END IF;
 INSERT INTO test262.observation_invalidations(observation_id,reason,actor) VALUES(o,'synthetic reporting regression',session_user);
 IF NOT EXISTS(SELECT 1 FROM test262_reporting.v_current_fixture_status WHERE provenance_id=pv AND channel='operational-regression'
  AND status='incomplete' AND recorded_variant_count=1) THEN RAISE EXCEPTION 'Invalidated report evidence counted'; END IF;
END $reporting$;
