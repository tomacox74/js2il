-- Versioned, database-login-bound supervisor interface. No Supabase public API exposure.
SET LOCAL lock_timeout='5s';
CREATE TABLE test262.api_subjects (
 login_name name PRIMARY KEY, repository_id uuid NOT NULL REFERENCES test262.repositories,
 producer_id uuid NOT NULL, permission text NOT NULL CHECK(permission IN ('ingest','coordinator')),
 trust_class text NOT NULL CHECK(trust_class IN ('untrusted','trusted','legacy')),
 enabled boolean NOT NULL DEFAULT true,
 FOREIGN KEY(repository_id,producer_id) REFERENCES test262.producers(repository_id,producer_id)
);
ALTER TABLE test262.api_subjects ENABLE ROW LEVEL SECURITY;
REVOKE ALL ON test262.api_subjects FROM PUBLIC,anon,authenticated,service_role;

CREATE FUNCTION test262.canonical_json(v jsonb) RETURNS text LANGUAGE plpgsql IMMUTABLE STRICT
SET search_path=pg_catalog AS $$
DECLARE r text;
BEGIN
 CASE jsonb_typeof(v)
 WHEN 'object' THEN SELECT '{'||coalesce(string_agg(to_jsonb(key)::text||':'||test262.canonical_json(value),',' ORDER BY key COLLATE "C"),'')||'}' INTO r FROM jsonb_each(v);
 WHEN 'array' THEN SELECT '['||coalesce(string_agg(test262.canonical_json(value),',' ORDER BY ord),'')||']' INTO r FROM jsonb_array_elements(v) WITH ORDINALITY e(value,ord);
 ELSE r=v::text;
 END CASE;
 RETURN r;
END $$;

CREATE FUNCTION test262.api_identity(epoch bigint, coordinating boolean DEFAULT false)
RETURNS test262.api_subjects LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; c test262.schema_contract;
BEGIN
 SELECT * INTO s FROM test262.api_subjects WHERE login_name=session_user AND enabled;
 IF s.login_name IS NULL OR NOT EXISTS(SELECT 1 FROM test262.producers WHERE producer_id=s.producer_id AND enabled) THEN
   RAISE EXCEPTION 'Unbound or disabled catalogue login' USING ERRCODE='42501'; END IF;
 SELECT * INTO c FROM test262.schema_contract;
 IF epoch<>c.minimum_writer_epoch THEN RAISE EXCEPTION 'Obsolete authority epoch' USING ERRCODE='40001'; END IF;
 IF coordinating AND s.permission<>'coordinator' THEN RAISE EXCEPTION 'Coordinator required' USING ERRCODE='42501'; END IF;
 RETURN s;
END $$;

-- Both ingestion and sealing take this lock. Lock order is repository -> work -> budget.
CREATE FUNCTION test262.api_lock(r uuid) RETURNS void LANGUAGE sql SET search_path=pg_catalog AS $$
 SELECT pg_advisory_xact_lock(hashtextextended('test262:'||r::text,0));
$$;

CREATE FUNCTION test262.api_contract() RETURNS jsonb LANGUAGE sql SECURITY DEFINER SET search_path=pg_catalog AS $$
 SELECT (to_jsonb(c)-'singleton')||jsonb_build_object('repository_id',s.repository_id,'producer_id',s.producer_id,'trust_class',s.trust_class,'permission',s.permission) FROM test262.schema_contract c CROSS JOIN test262.api_subjects s WHERE s.login_name=session_user AND s.enabled;
$$;

CREATE FUNCTION test262.api_scope(t text, d jsonb, r uuid) RETURNS boolean
LANGUAGE plpgsql SET search_path=pg_catalog AS $$
BEGIN
 IF d ? 'repository_id' THEN RETURN (d->>'repository_id')::uuid=r; END IF;
 CASE t
 WHEN 'corpora' THEN RETURN EXISTS(SELECT 1 FROM test262.repository_corpora WHERE repository_id=r AND corpus_id=(d->>'corpus_id')::uuid);
 WHEN 'fixtures' THEN RETURN EXISTS(SELECT 1 FROM test262.repository_corpora WHERE repository_id=r AND corpus_id=(d->>'corpus_id')::uuid);
 WHEN 'fixture_variants','fixture_dependencies' THEN RETURN EXISTS(SELECT 1 FROM test262.fixtures f JOIN test262.repository_corpora rc USING(corpus_id) WHERE rc.repository_id=r AND f.fixture_id=(d->>'fixture_id')::uuid);
 WHEN 'registrations' THEN RETURN EXISTS(SELECT 1 FROM test262.registration_snapshots WHERE repository_id=r AND snapshot_id=(d->>'snapshot_id')::uuid);
 WHEN 'observation_payloads','observation_invalidations' THEN RETURN EXISTS(SELECT 1 FROM test262.observations WHERE repository_id=r AND observation_id=(d->>'observation_id')::uuid);
 WHEN 'batch_fixtures','batch_evidence' THEN RETURN EXISTS(SELECT 1 FROM test262.native_batches WHERE repository_id=r AND batch_id=(d->>'batch_id')::uuid);
 WHEN 'publication_checks' THEN RETURN EXISTS(SELECT 1 FROM test262.publications WHERE repository_id=r AND publication_id=(d->>'publication_id')::uuid);
 WHEN 'import_records','legacy_control_records' THEN RETURN EXISTS(SELECT 1 FROM test262.imports WHERE repository_id=r AND import_id=(d->>'import_id')::uuid);
 ELSE RETURN false;
 END CASE;
END $$;

CREATE FUNCTION test262.api_start_run(epoch bigint, d jsonb) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; x test262.runs; old test262.runs;
BEGIN
 s=test262.api_identity(epoch); PERFORM test262.api_lock(s.repository_id);
 x=jsonb_populate_record(NULL::test262.runs,d);
 IF s.trust_class='legacy' AND x.run_kind<>'import' THEN RAISE EXCEPTION 'Importer run must be legacy import'; END IF;
 IF NOT EXISTS(SELECT 1 FROM test262.provenances WHERE provenance_id=x.provenance_id AND repository_id=s.repository_id) THEN RAISE EXCEPTION 'Invalid provenance'; END IF;
 INSERT INTO test262.runs(run_id,repository_id,producer_id,provenance_id,external_run_key,source_revision,run_kind,trust_class,state,environment_diagnostics)
 VALUES(x.run_id,s.repository_id,s.producer_id,x.provenance_id,x.external_run_key,x.source_revision,x.run_kind,s.trust_class,'running',coalesce(x.environment_diagnostics,'{}')) ON CONFLICT DO NOTHING;
 SELECT * INTO old FROM test262.runs WHERE run_id=x.run_id;
 IF old.run_id IS NULL OR ROW(old.repository_id,old.producer_id,old.provenance_id,old.external_run_key,old.source_revision,old.run_kind,old.trust_class) IS DISTINCT FROM ROW(s.repository_id,s.producer_id,x.provenance_id,x.external_run_key,x.source_revision,x.run_kind,s.trust_class) THEN RAISE EXCEPTION 'Conflicting run identity'; END IF;
 RETURN to_jsonb(old);
END $$;

-- Immutable insert/replay for coordinator inventory and control records. No arbitrary SQL/table names.
CREATE FUNCTION test262.api_put(epoch bigint, t text, rows jsonb) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; d jsonb; old jsonb; cols text; vals text; predicate text; supplied jsonb; n int=0;
BEGIN
 s=test262.api_identity(epoch,true); PERFORM test262.api_lock(s.repository_id);
 IF NOT t=ANY(ARRAY['corpora','repository_corpora','fixtures','fixture_variants','fixture_dependencies','provenances','provenance_fixture_eligibility','registration_snapshots','registrations','validation_events','reporting_targets','reconciliation_state','reconciliations','native_batches','batch_fixtures','batch_evidence','publications','publication_checks','imports','import_records','legacy_control_records','run_metrics','budget_scopes','budget_work_items','work_items','observation_invalidations']) THEN RAISE EXCEPTION 'Unsupported record type'; END IF;
 IF jsonb_typeof(rows)<>'array' OR jsonb_array_length(rows)>250 OR octet_length(rows::text)>4194304 THEN RAISE EXCEPTION 'Oversized record chunk'; END IF;
 IF s.trust_class<>'trusted' AND NOT t=ANY(ARRAY['corpora','repository_corpora','fixtures','fixture_variants','fixture_dependencies','provenances','provenance_fixture_eligibility','registration_snapshots','registrations','imports','import_records','legacy_control_records']) THEN RAISE EXCEPTION 'Trusted coordinator required' USING ERRCODE='42501'; END IF;
 FOR d IN SELECT value FROM jsonb_array_elements(rows) LOOP
  supplied=d;
  IF NOT test262.api_scope(t,d,s.repository_id) AND t<>'corpora' THEN RAISE EXCEPTION 'Cross-repository write' USING ERRCODE='42501'; END IF;
  IF t='corpora' AND coalesce(d->>'inventory_state','staging')<>'staging' THEN RAISE EXCEPTION 'Corpus must start staging'; END IF;
  IF t='registration_snapshots' AND coalesce(d->>'state','staging')<>'staging' THEN RAISE EXCEPTION 'Snapshot must start staging'; END IF;
  IF t='work_items' AND (coalesce(d->>'state','pending')<>'pending' OR coalesce((d->>'lease_generation')::bigint,0)<>0 OR d ? 'lease_owner' OR d ? 'completion_observation_id') THEN RAISE EXCEPTION 'Work must start pending'; END IF;
  IF EXISTS(SELECT 1 FROM jsonb_object_keys(d) k WHERE NOT EXISTS(SELECT 1 FROM pg_attribute WHERE attrelid=format('test262.%I',t)::regclass AND attname=k AND attnum>0 AND NOT attisdropped)) THEN RAISE EXCEPTION 'Unknown column'; END IF;
  SELECT string_agg(format('%I',key),',' ORDER BY key),string_agg(format('x.%I',key),',' ORDER BY key) INTO cols,vals FROM jsonb_object_keys(d) key;
  -- Primary key is required; defaults cannot conceal identity conflicts.
  SELECT string_agg(format('to_jsonb(y.%1$I) IS NOT DISTINCT FROM $1->%2$L',a.attname,a.attname),' AND ' ORDER BY u.ord)
  INTO predicate FROM pg_constraint c CROSS JOIN LATERAL unnest(c.conkey) WITH ORDINALITY u(attnum,ord) JOIN pg_attribute a ON a.attrelid=c.conrelid AND a.attnum=u.attnum WHERE c.conrelid=format('test262.%I',t)::regclass AND c.contype='p';
  IF EXISTS(SELECT 1 FROM pg_constraint c CROSS JOIN LATERAL unnest(c.conkey) u(attnum) JOIN pg_attribute a ON a.attrelid=c.conrelid AND a.attnum=u.attnum WHERE c.conrelid=format('test262.%I',t)::regclass AND c.contype='p' AND NOT d ? a.attname) THEN RAISE EXCEPTION 'Missing primary key'; END IF;
  -- Read before INSERT because sealed inventory INSERT triggers intentionally reject mutation.
  EXECUTE format('SELECT to_jsonb(y) FROM test262.%I y WHERE %s',t,predicate) INTO old USING d;
  IF old IS NULL THEN
    EXECUTE format('INSERT INTO test262.%I (%s) SELECT %s FROM jsonb_populate_record(NULL::test262.%I,$1) x RETURNING to_jsonb(%I.*)',t,cols,vals,t,t) INTO old USING d;
  END IF;
  -- Compare normalized values (bytea JSON encoding, timestamps, arrays), not caller formatting.
  EXECUTE format('SELECT to_jsonb(x) FROM jsonb_populate_record(NULL::test262.%I,$1) x',t) INTO d USING d;
  IF EXISTS(SELECT 1 FROM jsonb_object_keys(supplied) k WHERE old->k IS DISTINCT FROM d->k) THEN RAISE EXCEPTION 'Conflicting immutable replay for %',t; END IF;
  n=n+1;
 END LOOP;
 RETURN jsonb_build_object('records',n);
END $$;

CREATE FUNCTION test262.api_ingest(epoch bigint, request_id uuid, rows jsonb) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; req test262.ingest_requests; d jsonb; o test262.observations; prev test262.observations; h bytea; receipt jsonb; n int=0;
BEGIN
 s=test262.api_identity(epoch); PERFORM test262.api_lock(s.repository_id);
 IF jsonb_typeof(rows)<>'array' OR jsonb_array_length(rows)>250 OR octet_length(rows::text)>4194304 THEN RAISE EXCEPTION 'Oversized ingest'; END IF;
 h=sha256(convert_to(test262.canonical_json(rows),'UTF8'));
 SELECT * INTO req FROM test262.ingest_requests i WHERE i.repository_id=s.repository_id AND i.producer_id=s.producer_id AND i.request_id=api_ingest.request_id;
 IF req.request_id IS NOT NULL THEN
  IF req.request_sha256<>h THEN RAISE EXCEPTION 'Conflicting request payload'; END IF;
  RETURN req.receipt;
 END IF;
 FOR d IN SELECT value FROM jsonb_array_elements(rows) LOOP
  o=jsonb_populate_record(NULL::test262.observations,d-'payload');
  IF o.repository_id<>s.repository_id OR NOT EXISTS(SELECT 1 FROM test262.runs WHERE run_id=o.run_id AND producer_id=s.producer_id AND repository_id=s.repository_id AND trust_class=s.trust_class) THEN RAISE EXCEPTION 'Invalid observation owner' USING ERRCODE='42501'; END IF;
  IF s.trust_class='legacy' AND o.source_kind<>'legacy-snapshot' THEN RAISE EXCEPTION 'Importer cannot create live evidence'; END IF;
  IF s.trust_class<>'legacy' AND o.source_kind<>'live' THEN RAISE EXCEPTION 'Snapshot source requires importer'; END IF;
  o.diagnostic_summary=coalesce(o.diagnostic_summary,'');
  o.payload_sha256=sha256(convert_to(test262.canonical_json(d-'payload_sha256'),'UTF8'));
  SELECT * INTO prev FROM test262.observations WHERE observation_id=o.observation_id;
  IF prev.observation_id IS NOT NULL THEN
   IF prev.payload_sha256<>o.payload_sha256 THEN RAISE EXCEPTION 'Conflicting observation replay'; END IF;
  ELSE
   o.received_at=clock_timestamp();
   INSERT INTO test262.observations SELECT o.*;
   INSERT INTO test262.observation_payloads VALUES(o.observation_id,d->'payload',NULL,NULL,CASE WHEN d ? 'payload' THEN 'hot' ELSE 'missing' END,NULL);
   -- A contradictory late observation cannot silently leave an accepted batch fresh.
   IF o.outcome<>'pass' AND s.trust_class='trusted' THEN
    UPDATE test262.native_batches b SET state='conflicted',version=version+1 WHERE b.repository_id=s.repository_id AND b.provenance_id=o.provenance_id AND b.state IN ('sealed','awaiting-refresh') AND EXISTS(SELECT 1 FROM test262.batch_fixtures f WHERE f.batch_id=b.batch_id AND f.fixture_id=o.fixture_id AND f.state='accepted');
   END IF;
  END IF;
  n=n+1;
 END LOOP;
 receipt=jsonb_build_object('request_id',request_id,'records',n,'committed_at',clock_timestamp());
 INSERT INTO test262.ingest_requests VALUES(s.repository_id,s.producer_id,request_id,h,n,'committed',clock_timestamp(),receipt);
 UPDATE test262.producers SET last_seen_at=clock_timestamp() WHERE producer_id=s.producer_id;
 RETURN receipt;
END $$;

CREATE FUNCTION test262.api_claim(epoch bigint, run_id uuid, budget_id uuid, cap_ms bigint, ttl_seconds int)
RETURNS jsonb LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; r test262.runs; w test262.work_items; b test262.budget_scopes; reservation uuid;
BEGIN
 s=test262.api_identity(epoch,true); PERFORM test262.api_lock(s.repository_id);
 IF s.trust_class<>'trusted' THEN RAISE EXCEPTION 'Trusted coordinator required' USING ERRCODE='42501'; END IF;
 IF (SELECT deployment_state FROM test262.schema_contract)<>'active' THEN RAISE EXCEPTION 'Scheduling requires active authority'; END IF;
 IF cap_ms NOT BETWEEN 1 AND 3600000 OR ttl_seconds NOT BETWEEN 1 AND 7200 OR ttl_seconds*1000<cap_ms+30000 THEN RAISE EXCEPTION 'Invalid cap/TTL; require 30s settlement grace'; END IF;
 SELECT * INTO r FROM test262.runs x WHERE x.run_id=api_claim.run_id AND x.repository_id=s.repository_id AND x.producer_id=s.producer_id AND x.state='running';
 IF r.run_id IS NULL THEN RAISE EXCEPTION 'Invalid run'; END IF;
 -- Conservatively charge abandoned reservations before returning work to pending.
 FOR w IN SELECT * FROM test262.work_items x WHERE x.repository_id=s.repository_id AND x.state='leased' AND x.lease_expires_at<=clock_timestamp() ORDER BY x.work_item_id FOR UPDATE SKIP LOCKED LOOP
  PERFORM test262.api_settle(w.work_item_id,w.lease_generation,NULL);
  UPDATE test262.work_leases SET released_at=clock_timestamp(),release_reason='expired' WHERE work_item_id=w.work_item_id AND lease_generation=w.lease_generation;
  UPDATE test262.work_items SET state=CASE WHEN lease_generation>=attempt_limit THEN 'deferred' ELSE 'pending' END,lease_owner=NULL,lease_expires_at=NULL,version=version+1 WHERE work_item_id=w.work_item_id;
 END LOOP;
 SELECT * INTO w FROM test262.work_items x WHERE x.repository_id=s.repository_id AND x.provenance_id=r.provenance_id AND x.state='pending' AND x.eligible_after<=clock_timestamp() AND x.lease_generation<x.attempt_limit AND EXISTS(SELECT 1 FROM test262.budget_work_items bw WHERE bw.work_item_id=x.work_item_id AND bw.budget_scope_id=budget_id) ORDER BY priority DESC,eligible_after,work_item_id FOR UPDATE SKIP LOCKED LIMIT 1;
 IF w.work_item_id IS NULL THEN RETURN NULL; END IF;
 SELECT * INTO b FROM test262.budget_scopes x WHERE x.budget_scope_id=budget_id AND x.repository_id=s.repository_id FOR UPDATE;
 IF b.budget_scope_id IS NULL THEN RAISE EXCEPTION 'Invalid budget'; END IF;
 IF b.state<>'open' OR b.reserved_attempts+b.charged_attempts+1>b.attempt_limit OR b.reserved_ms+b.charged_ms+cap_ms>b.time_limit_ms THEN RETURN jsonb_build_object('budget_exhausted',true); END IF;
 UPDATE test262.budget_scopes SET reserved_attempts=reserved_attempts+1,reserved_ms=reserved_ms+cap_ms,version=version+1 WHERE budget_scope_id=budget_id;
 UPDATE test262.work_items SET state='leased',lease_generation=lease_generation+1,lease_owner=s.producer_id,lease_expires_at=clock_timestamp()+make_interval(secs=>ttl_seconds),version=version+1 WHERE work_item_id=w.work_item_id RETURNING * INTO w;
 INSERT INTO test262.work_leases(work_item_id,lease_generation,repository_id,producer_id,run_id,expires_at) VALUES(w.work_item_id,w.lease_generation,s.repository_id,s.producer_id,r.run_id,w.lease_expires_at);
 INSERT INTO test262.budget_reservations(repository_id,budget_scope_id,work_item_id,lease_generation,reserved_attempts,reserved_ms) VALUES(s.repository_id,budget_id,w.work_item_id,w.lease_generation,1,cap_ms) RETURNING reservation_id INTO reservation;
 RETURN to_jsonb(w)||jsonb_build_object('reservation_id',reservation,'cap_ms',cap_ms);
END $$;

CREATE FUNCTION test262.api_settle(work_id uuid, generation bigint, measured_ms bigint) RETURNS void
LANGUAGE plpgsql SET search_path=pg_catalog AS $$
DECLARE b test262.budget_reservations; charge bigint;
BEGIN
 FOR b IN SELECT * FROM test262.budget_reservations WHERE work_item_id=work_id AND lease_generation=generation AND state='reserved' ORDER BY budget_scope_id FOR UPDATE LOOP
  charge=CASE WHEN measured_ms IS NULL THEN b.reserved_ms ELSE least(b.reserved_ms,greatest(0,measured_ms)) END;
  UPDATE test262.budget_scopes SET reserved_attempts=reserved_attempts-b.reserved_attempts,reserved_ms=reserved_ms-b.reserved_ms,charged_attempts=charged_attempts+b.reserved_attempts,charged_ms=charged_ms+charge,version=version+1 WHERE budget_scope_id=b.budget_scope_id;
  UPDATE test262.budget_reservations SET state=CASE WHEN measured_ms IS NULL THEN 'uncertain' ELSE 'settled' END,charged_attempts=reserved_attempts,charged_ms=charge WHERE reservation_id=b.reservation_id;
 END LOOP;
END $$;

CREATE FUNCTION test262.api_renew(epoch bigint, work_id uuid, generation bigint, ttl_seconds int) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; w test262.work_items;
BEGIN
 s=test262.api_identity(epoch,true); PERFORM test262.api_lock(s.repository_id);
 IF ttl_seconds NOT BETWEEN 30 AND 7200 THEN RAISE EXCEPTION 'Invalid TTL'; END IF;
 UPDATE test262.work_items SET lease_expires_at=clock_timestamp()+make_interval(secs=>ttl_seconds),version=version+1 WHERE work_item_id=work_id AND repository_id=s.repository_id AND lease_owner=s.producer_id AND lease_generation=generation AND state='leased' AND lease_expires_at>clock_timestamp() RETURNING * INTO w;
 IF w.work_item_id IS NULL THEN RAISE EXCEPTION 'Stale lease' USING ERRCODE='40001'; END IF;
 UPDATE test262.work_leases SET expires_at=w.lease_expires_at WHERE work_item_id=work_id AND lease_generation=generation;
 RETURN to_jsonb(w);
END $$;

CREATE FUNCTION test262.api_complete(epoch bigint, work_id uuid, generation bigint, observation_id uuid) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; w test262.work_items; o test262.observations; live boolean;
BEGIN
 s=test262.api_identity(epoch,true); PERFORM test262.api_lock(s.repository_id);
 SELECT * INTO w FROM test262.work_items WHERE work_item_id=work_id AND repository_id=s.repository_id FOR UPDATE;
 SELECT * INTO o FROM test262.observations x WHERE x.observation_id=api_complete.observation_id AND x.repository_id=s.repository_id;
 IF w.work_item_id IS NULL OR o.observation_id IS NULL OR ROW(o.work_item_id,o.lease_generation) IS DISTINCT FROM ROW(work_id,generation) OR NOT EXISTS(SELECT 1 FROM test262.runs WHERE run_id=o.run_id AND producer_id=s.producer_id) THEN RAISE EXCEPTION 'Completion identity mismatch'; END IF;
 IF w.completion_observation_id=o.observation_id THEN RETURN jsonb_build_object('completed',true,'replay',true); END IF;
 live=w.state='leased' AND w.lease_generation=generation AND w.lease_owner=s.producer_id AND w.lease_expires_at>clock_timestamp();
 IF NOT live THEN RETURN jsonb_build_object('completed',false,'retained_late_evidence',true); END IF;
 PERFORM test262.api_settle(work_id,generation,o.active_ms);
 UPDATE test262.work_leases SET released_at=clock_timestamp(),release_reason=o.outcome WHERE work_item_id=work_id AND lease_generation=generation;
 UPDATE test262.work_items SET state=CASE WHEN o.outcome IN ('incomplete','infrastructure-error') THEN CASE WHEN lease_generation>=attempt_limit THEN 'deferred' ELSE 'pending' END ELSE 'completed' END,completion_observation_id=CASE WHEN o.outcome IN ('incomplete','infrastructure-error') THEN NULL ELSE o.observation_id END,lease_owner=NULL,lease_expires_at=NULL,eligible_after=clock_timestamp()+interval '60 seconds',version=version+1 WHERE work_item_id=work_id;
 RETURN jsonb_build_object('completed',true);
END $$;

-- Controlled CAS for coordinator state. Identity fields cannot be rewritten.
CREATE FUNCTION test262.api_transition(epoch bigint, t text, key jsonb, expected_version bigint, patch jsonb)
RETURNS jsonb LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; old jsonb; result jsonb; allowed text[]; predicate text; assigns text;
BEGIN
 s=test262.api_identity(epoch,true); PERFORM test262.api_lock(s.repository_id);
 IF s.trust_class<>'trusted' AND NOT t=ANY(ARRAY['corpora','registration_snapshots','imports','runs']) THEN RAISE EXCEPTION 'Trusted coordinator required' USING ERRCODE='42501'; END IF;
 CASE t
 WHEN 'runs' THEN allowed=ARRAY['state','ended_at'];
 WHEN 'corpora' THEN allowed=ARRAY['inventory_state'];
 WHEN 'registration_snapshots' THEN allowed=ARRAY['state'];
 WHEN 'imports' THEN allowed=ARRAY['state','imported_counts','limitations','completed_at'];
 WHEN 'native_batches' THEN allowed=ARRAY['state'];
 WHEN 'publications' THEN allowed=ARRAY['state','pr_number','expected_head','closure_reason'];
 WHEN 'reporting_targets' THEN allowed=ARRAY['provenance_id','registration_snapshot_id','validation_id','updated_at'];
 WHEN 'reconciliation_state' THEN allowed=ARRAY['last_validation_id','last_reconciled_revision','fallback_cursor','authority_epoch'];
 WHEN 'reconciliations' THEN allowed=ARRAY['state','committed_at'];
 WHEN 'batch_fixtures' THEN allowed=ARRAY['state'];
 ELSE RAISE EXCEPTION 'Unsupported transition';
 END CASE;
 IF NOT EXISTS(SELECT 1 FROM jsonb_object_keys(key)) OR EXISTS(SELECT 1 FROM jsonb_object_keys(patch) k WHERE NOT k=ANY(allowed)) THEN RAISE EXCEPTION 'Invalid transition fields'; END IF;
 IF (SELECT count(*) FROM jsonb_object_keys(key))<>(SELECT cardinality(conkey) FROM pg_constraint WHERE conrelid=format('test262.%I',t)::regclass AND contype='p') OR EXISTS(SELECT 1 FROM pg_constraint c CROSS JOIN LATERAL unnest(c.conkey) u(attnum) JOIN pg_attribute a ON a.attrelid=c.conrelid AND a.attnum=u.attnum WHERE c.conrelid=format('test262.%I',t)::regclass AND c.contype='p' AND NOT key ? a.attname) THEN RAISE EXCEPTION 'Transition requires the exact primary key'; END IF;

 SELECT string_agg(format('to_jsonb(x.%I)=$1->%L',k,k),' AND ') INTO predicate FROM jsonb_object_keys(key) k;
 EXECUTE format('SELECT to_jsonb(x) FROM test262.%I x WHERE %s FOR UPDATE',t,predicate) INTO old USING key;
 IF old IS NULL OR NOT test262.api_scope(t,old,s.repository_id) THEN RAISE EXCEPTION 'Missing scoped record'; END IF;
 IF old ? 'version' AND (old->>'version')::bigint<>expected_version THEN RAISE EXCEPTION 'CAS conflict' USING ERRCODE='40001'; END IF;
 IF NOT EXISTS(SELECT 1 FROM jsonb_each(patch) e WHERE old->e.key IS DISTINCT FROM e.value) THEN RETURN old; END IF;
 IF t='reporting_targets' THEN patch=patch||jsonb_build_object('updated_at',clock_timestamp()); END IF;
 -- Sealing inventories verifies exact content and cannot infer a complete snapshot from count alone.
 IF t='corpora' AND patch->>'inventory_state'='sealed' THEN
  IF (old->>'inventory_digest')::bytea<>test262.inventory_hash((old->>'corpus_id')::uuid) THEN RAISE EXCEPTION 'Inventory digest mismatch'; END IF;
 END IF;
 IF t='registration_snapshots' AND patch->>'state'='sealed' THEN
  IF (old->>'inventory_sha256')::bytea IS DISTINCT FROM (SELECT sha256(convert_to(test262.canonical_json(coalesce(jsonb_agg(to_jsonb(x)-'snapshot_id' ORDER BY upstream_path COLLATE "C",source_file COLLATE "C",registration_kind COLLATE "C"),'[]')),'UTF8')) FROM test262.registrations x WHERE snapshot_id=(old->>'snapshot_id')::uuid) THEN RAISE EXCEPTION 'Registration snapshot digest mismatch'; END IF;
 END IF;
 IF t='native_batches' AND patch->>'state'='sealed' THEN
  IF NOT EXISTS(SELECT 1 FROM test262.reporting_targets rt WHERE rt.repository_id=s.repository_id AND rt.evidence_kind='native' AND rt.channel='master' AND rt.provenance_id=(old->>'provenance_id')::uuid AND rt.validation_id=(old->>'validation_id')::uuid AND rt.registration_snapshot_id=(old->>'registration_snapshot_id')::uuid) THEN RAISE EXCEPTION 'Stale native batch'; END IF;
  IF EXISTS(SELECT 1 FROM test262.budget_scopes b WHERE b.budget_scope_id=(old->>'budget_scope_id')::uuid AND ((SELECT count(*) FROM test262.batch_fixtures WHERE batch_id=(old->>'batch_id')::uuid)>b.candidate_limit OR (SELECT count(*) FROM test262.batch_fixtures WHERE batch_id=(old->>'batch_id')::uuid AND state='accepted')>b.accepted_limit)) THEN RAISE EXCEPTION 'Native batch candidate/acceptance budget exceeded'; END IF;
 END IF;
 SELECT string_agg(format('%1$I=y.%1$I',k),',') INTO assigns FROM jsonb_object_keys(patch) k;
 IF assigns IS NULL THEN RAISE EXCEPTION 'Empty transition'; END IF;
 IF old ? 'version' THEN assigns=assigns||',version=x.version+1'; END IF;
 EXECUTE format('UPDATE test262.%1$I x SET %2$s FROM jsonb_populate_record(NULL::test262.%1$I,$2) y WHERE %3$s RETURNING to_jsonb(x)',t,assigns,predicate) INTO result USING key,patch;
 INSERT INTO test262.audit_events(repository_id,actor_subject,action,entity_kind,details) VALUES(s.repository_id,session_user,'transition',t,jsonb_build_object('key',key,'before',old,'after',result));
 RETURN result;
END $$;

CREATE FUNCTION test262.inventory_hash(c uuid) RETURNS bytea LANGUAGE sql SET search_path=pg_catalog AS $$
 SELECT sha256(convert_to(coalesce(string_agg(test262.canonical_json(jsonb_build_object(
 'path',f.upstream_path,'sha256',encode(f.content_sha256,'hex'),'metadata',f.metadata,'metadata_state',f.metadata_state,'is_support_file',f.is_support_file,'feature_tags',to_jsonb(f.feature_tags),'dependency_manifest_digest',encode(f.dependency_manifest_digest,'hex'),
 'variants',coalesce((SELECT jsonb_agg(jsonb_build_object('variant',v.variant,'expected_phase',v.expected_phase,'expected_error_type',v.expected_error_type,'required',v.required) ORDER BY v.variant) FROM test262.fixture_variants v WHERE v.fixture_id=f.fixture_id),'[]'),
 'dependencies',coalesce((SELECT jsonb_agg(jsonb_build_object('dependency_path',d.dependency_path,'content_sha256',encode(d.content_sha256,'hex'),'dependency_kind',d.dependency_kind,'resolved_path',rf.upstream_path) ORDER BY d.dependency_path) FROM test262.fixture_dependencies d LEFT JOIN test262.fixtures rf ON rf.fixture_id=d.resolved_fixture_id WHERE d.fixture_id=f.fixture_id),'[]')))||E'\n','' ORDER BY f.upstream_path COLLATE "C"),''),'UTF8')) FROM test262.fixtures f WHERE corpus_id=c;
$$;

CREATE FUNCTION test262.api_snapshot(epoch bigint) RETURNS jsonb LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; result jsonb='{}'; t text; a jsonb;
BEGIN
 s=test262.api_identity(epoch);
 IF current_setting('transaction_isolation') NOT IN ('repeatable read','serializable') THEN RAISE EXCEPTION 'Snapshot export requires REPEATABLE READ'; END IF;
 FOREACH t IN ARRAY ARRAY['corpora','repository_corpora','fixtures','fixture_variants','fixture_dependencies','provenances','provenance_fixture_eligibility','runs','observations','observation_payloads','observation_invalidations','validation_events','work_items','work_leases','budget_reservations','budget_scopes','budget_work_items','registration_snapshots','registrations','reporting_targets','native_batches','batch_fixtures','batch_evidence','publications','publication_checks','imports','import_records','legacy_control_records','reconciliation_state','reconciliations','run_metrics'] LOOP
  EXECUTE format('SELECT coalesce(jsonb_agg(to_jsonb(x)),''[]''::jsonb) FROM test262.%I x WHERE test262.api_scope($1,to_jsonb(x),$2)',t) INTO a USING t,s.repository_id;
  result=result||jsonb_build_object(t,a);
 END LOOP;
 RETURN jsonb_build_object('schema',1,'api',1,'epoch',epoch,'repository_id',s.repository_id,'snapshot_token',txid_current_snapshot()::text,'as_of',transaction_timestamp(),'tables',result);
END $$;

REVOKE EXECUTE ON ALL FUNCTIONS IN SCHEMA test262 FROM PUBLIC,anon,authenticated,service_role,test262_ingest,test262_coordinator,test262_reporter;
GRANT USAGE ON SCHEMA test262 TO test262_ingest,test262_coordinator;
GRANT EXECUTE ON FUNCTION test262.api_contract(),test262.api_start_run(bigint,jsonb),test262.api_ingest(bigint,uuid,jsonb),test262.api_snapshot(bigint) TO test262_ingest,test262_coordinator;
GRANT EXECUTE ON FUNCTION test262.api_put(bigint,text,jsonb),test262.api_claim(bigint,uuid,uuid,bigint,int),test262.api_renew(bigint,uuid,bigint,int),test262.api_complete(bigint,uuid,bigint,uuid),test262.api_transition(bigint,text,jsonb,bigint,jsonb) TO test262_coordinator;
UPDATE test262.schema_contract SET api_contract_version=1;

CREATE FUNCTION test262.guard_dependency_scope() RETURNS trigger LANGUAGE plpgsql SET search_path=pg_catalog AS $$
BEGIN
 IF NEW.resolved_fixture_id IS NOT NULL AND NOT EXISTS(SELECT 1 FROM test262.fixtures a JOIN test262.fixtures b ON a.corpus_id=b.corpus_id WHERE a.fixture_id=NEW.fixture_id AND b.fixture_id=NEW.resolved_fixture_id AND b.upstream_path=NEW.dependency_path AND b.content_sha256=NEW.content_sha256) THEN RAISE EXCEPTION 'Dependency scope/content mismatch'; END IF;
 RETURN NEW;
END $$;
CREATE TRIGGER dependency_scope BEFORE INSERT OR UPDATE ON test262.fixture_dependencies FOR EACH ROW EXECUTE FUNCTION test262.guard_dependency_scope();

CREATE FUNCTION test262.api_reconcile(epoch bigint, reconciliation_id uuid, expected_version bigint, next_cursor bigint, work_rows jsonb)
RETURNS jsonb LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; plan test262.reconciliations; cur test262.reconciliation_state; revision text; chunk jsonb;
BEGIN
 s=test262.api_identity(epoch,true); PERFORM test262.api_lock(s.repository_id);
 IF s.trust_class<>'trusted' THEN RAISE EXCEPTION 'Trusted coordinator required' USING ERRCODE='42501'; END IF;
 SELECT * INTO plan FROM test262.reconciliations x WHERE x.reconciliation_id=api_reconcile.reconciliation_id AND x.repository_id=s.repository_id FOR UPDATE;
 IF plan.reconciliation_id IS NULL THEN RAISE EXCEPTION 'Missing reconciliation'; END IF;
 IF plan.plan_sha256<>sha256(convert_to(test262.canonical_json(jsonb_build_object('work',work_rows,'next_cursor',next_cursor)),'UTF8')) THEN RAISE EXCEPTION 'Conflicting reconciliation replay'; END IF;
 IF jsonb_typeof(work_rows)<>'array' OR jsonb_array_length(work_rows)>2000 THEN RAISE EXCEPTION 'Oversized reconciliation'; END IF;
 IF plan.state='committed' THEN RETURN jsonb_build_object('replay',true); END IF;
 SELECT * INTO cur FROM test262.reconciliation_state WHERE repository_id=s.repository_id AND pipeline=plan.pipeline AND channel=plan.channel FOR UPDATE;
 IF cur.version<>expected_version OR plan.expected_cursor_version<>expected_version OR cur.authority_epoch<>epoch OR next_cursor<cur.fallback_cursor OR plan.state<>'planned' THEN RAISE EXCEPTION 'Stale reconciliation' USING ERRCODE='40001'; END IF;
 SELECT target_revision INTO revision FROM test262.validation_events WHERE validation_id=plan.validation_id AND conclusion='success';
 IF revision IS NULL THEN RAISE EXCEPTION 'Unvalidated target'; END IF;
 FOR chunk IN SELECT jsonb_agg(value ORDER BY ord) FROM jsonb_array_elements(work_rows) WITH ORDINALITY e(value,ord) GROUP BY (ord-1)/250 ORDER BY (ord-1)/250 LOOP
  PERFORM test262.api_put(epoch,'work_items',chunk);
 END LOOP;
 UPDATE test262.reconciliation_state SET last_validation_id=plan.validation_id,last_reconciled_revision=revision,fallback_cursor=next_cursor,version=version+1 WHERE repository_id=s.repository_id AND pipeline=plan.pipeline AND channel=plan.channel;
 UPDATE test262.reconciliations SET state='committed',committed_at=clock_timestamp() WHERE test262.reconciliations.reconciliation_id=plan.reconciliation_id;
 RETURN jsonb_build_object('revision',revision,'cursor',next_cursor,'version',expected_version+1);
END $$;
REVOKE EXECUTE ON FUNCTION test262.guard_dependency_scope(),test262.api_reconcile(bigint,uuid,bigint,bigint,jsonb) FROM PUBLIC;
GRANT EXECUTE ON FUNCTION test262.api_reconcile(bigint,uuid,bigint,bigint,jsonb) TO test262_coordinator;
CREATE FUNCTION test262.guard_lease_run() RETURNS trigger LANGUAGE plpgsql SET search_path=pg_catalog AS $$
BEGIN
 IF NEW.work_item_id IS NOT NULL AND NOT EXISTS(SELECT 1 FROM test262.work_leases l WHERE l.work_item_id=NEW.work_item_id AND l.lease_generation=NEW.lease_generation AND l.run_id=NEW.run_id) THEN RAISE EXCEPTION 'Observation belongs to another lease run'; END IF;
 RETURN NEW;
END $$;
CREATE TRIGGER observation_lease_run BEFORE INSERT ON test262.observations FOR EACH ROW EXECUTE FUNCTION test262.guard_lease_run();
REVOKE EXECUTE ON FUNCTION test262.guard_lease_run() FROM PUBLIC;
CREATE FUNCTION test262.api_metric(epoch bigint, run_id uuid, stage text, measurement_version int, duration_ms bigint, attempt_count int, timeout_count int)
RETURNS jsonb LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; m test262.run_metrics;
BEGIN
 s=test262.api_identity(epoch); PERFORM test262.api_lock(s.repository_id);
 IF NOT EXISTS(SELECT 1 FROM test262.runs r WHERE r.run_id=api_metric.run_id AND r.repository_id=s.repository_id AND r.producer_id=s.producer_id) THEN RAISE EXCEPTION 'Invalid metric run'; END IF;
 SELECT * INTO m FROM test262.run_metrics x WHERE x.run_id=api_metric.run_id AND x.stage=api_metric.stage FOR UPDATE;
 IF m.measurement_version>measurement_version THEN RAISE EXCEPTION 'Stale measurement' USING ERRCODE='40001'; END IF;
 IF m.measurement_version=measurement_version THEN
  IF ROW(m.duration_ms,m.attempt_count,m.timeout_count) IS DISTINCT FROM ROW(duration_ms,attempt_count,timeout_count) THEN RAISE EXCEPTION 'Conflicting measurement replay'; END IF;
  RETURN to_jsonb(m);
 END IF;
 INSERT INTO test262.run_metrics VALUES(s.repository_id,run_id,stage,duration_ms,attempt_count,timeout_count,measurement_version)
 ON CONFLICT ON CONSTRAINT run_metrics_pkey DO UPDATE SET duration_ms=excluded.duration_ms,attempt_count=excluded.attempt_count,timeout_count=excluded.timeout_count,measurement_version=excluded.measurement_version RETURNING * INTO m;
 RETURN to_jsonb(m);
END $$;
REVOKE EXECUTE ON FUNCTION test262.api_metric(bigint,uuid,text,int,bigint,int,int) FROM PUBLIC;
GRANT EXECUTE ON FUNCTION test262.api_metric(bigint,uuid,text,int,bigint,int,int) TO test262_ingest,test262_coordinator;
CREATE FUNCTION test262.guard_native_dependency_digest() RETURNS trigger LANGUAGE plpgsql SET search_path=pg_catalog AS $$
BEGIN
 IF NEW.state='sealed' AND EXISTS(
  SELECT 1 FROM test262.batch_fixtures bf JOIN test262.fixtures f USING(fixture_id)
  WHERE bf.batch_id=NEW.batch_id AND bf.state='accepted' AND f.dependency_manifest_digest IS DISTINCT FROM
  (SELECT sha256(convert_to(test262.canonical_json(coalesce(jsonb_agg(jsonb_build_object('dependency_path',d.dependency_path,'content_sha256',encode(d.content_sha256,'hex'),'dependency_kind',d.dependency_kind,'resolved_path',rf.upstream_path) ORDER BY d.dependency_path),'[]')),'UTF8')) FROM test262.fixture_dependencies d LEFT JOIN test262.fixtures rf ON rf.fixture_id=d.resolved_fixture_id WHERE d.fixture_id=f.fixture_id)
 ) THEN RAISE EXCEPTION 'Unverified dependency manifest cannot seal'; END IF;
 RETURN NEW;
END $$;
CREATE TRIGGER native_dependency_digest BEFORE UPDATE ON test262.native_batches FOR EACH ROW EXECUTE FUNCTION test262.guard_native_dependency_digest();
REVOKE EXECUTE ON FUNCTION test262.guard_native_dependency_digest() FROM PUBLIC;
-- Preserve legacy scheduling/publication state for audit without promoting its ownership/cursor.
CREATE TABLE test262.legacy_control_records (
 import_id uuid NOT NULL REFERENCES test262.imports, source_table text NOT NULL, source_key text NOT NULL,
 document jsonb NOT NULL, PRIMARY KEY(import_id,source_table,source_key)
);
ALTER TABLE test262.legacy_control_records ENABLE ROW LEVEL SECURITY;
REVOKE ALL ON test262.legacy_control_records FROM PUBLIC,anon,authenticated,service_role;
CREATE TRIGGER legacy_control_immutable BEFORE UPDATE OR DELETE ON test262.legacy_control_records FOR EACH ROW EXECUTE FUNCTION test262.reject_evidence_mutation();

CREATE FUNCTION test262.mark_invalidated_batch() RETURNS trigger LANGUAGE plpgsql SET search_path=pg_catalog AS $$
DECLARE r uuid;
BEGIN
 SELECT repository_id INTO r FROM test262.observations WHERE observation_id=NEW.observation_id;
 PERFORM test262.api_lock(r);
 UPDATE test262.native_batches b SET state='conflicted',version=version+1 WHERE b.repository_id=r AND b.state IN ('sealed','awaiting-refresh') AND EXISTS(SELECT 1 FROM test262.batch_evidence e WHERE e.batch_id=b.batch_id AND e.observation_id=NEW.observation_id);
 RETURN NEW;
END $$;
CREATE TRIGGER invalidated_batch AFTER INSERT ON test262.observation_invalidations FOR EACH ROW EXECUTE FUNCTION test262.mark_invalidated_batch();

CREATE FUNCTION test262.mark_stale_batches() RETURNS trigger LANGUAGE plpgsql SET search_path=pg_catalog AS $$
BEGIN
 IF NEW.evidence_kind='native' AND NEW.channel='master' THEN
  UPDATE test262.native_batches b SET state='awaiting-refresh',version=version+1 WHERE b.repository_id=NEW.repository_id AND b.state='sealed' AND ROW(b.provenance_id,b.validation_id,b.registration_snapshot_id) IS DISTINCT FROM ROW(NEW.provenance_id,NEW.validation_id,NEW.registration_snapshot_id);
 END IF;
 RETURN NEW;
END $$;
CREATE TRIGGER stale_batch AFTER INSERT OR UPDATE ON test262.reporting_targets FOR EACH ROW EXECUTE FUNCTION test262.mark_stale_batches();
REVOKE EXECUTE ON FUNCTION test262.mark_invalidated_batch(),test262.mark_stale_batches() FROM PUBLIC;
CREATE TABLE test262.budget_work_items (
 repository_id uuid NOT NULL, budget_scope_id uuid NOT NULL, work_item_id uuid NOT NULL,
 PRIMARY KEY(budget_scope_id,work_item_id),
 FOREIGN KEY(repository_id,budget_scope_id) REFERENCES test262.budget_scopes(repository_id,budget_scope_id),
 FOREIGN KEY(repository_id,work_item_id) REFERENCES test262.work_items(repository_id,work_item_id)
);
ALTER TABLE test262.budget_work_items ENABLE ROW LEVEL SECURITY;
REVOKE ALL ON test262.budget_work_items FROM PUBLIC,anon,authenticated,service_role;
CREATE FUNCTION test262.guard_budget_candidates() RETURNS trigger LANGUAGE plpgsql SET search_path=pg_catalog AS $$
DECLARE b test262.budget_scopes;
BEGIN
 SELECT * INTO b FROM test262.budget_scopes WHERE budget_scope_id=NEW.budget_scope_id FOR UPDATE;
 IF (SELECT count(DISTINCT w.fixture_id) FROM test262.budget_work_items x JOIN test262.work_items w USING(work_item_id) WHERE x.budget_scope_id=NEW.budget_scope_id)>b.candidate_limit THEN RAISE EXCEPTION 'Candidate enrollment exceeds budget'; END IF;
 RETURN NEW;
END $$;
CREATE TRIGGER budget_candidate_limit AFTER INSERT ON test262.budget_work_items FOR EACH ROW EXECUTE FUNCTION test262.guard_budget_candidates();
REVOKE EXECUTE ON FUNCTION test262.guard_budget_candidates() FROM PUBLIC;

-- Exact negative proof is required for live acceptance, not invented for legacy snapshots.
CREATE OR REPLACE FUNCTION test262.guard_fixture_provenance() RETURNS trigger LANGUAGE plpgsql SET search_path=pg_catalog AS $$
DECLARE pc uuid; fc uuid; ps text; p test262.provenances; r test262.runs; w test262.work_items; v test262.fixture_variants; f test262.fixtures;
BEGIN
 SELECT * INTO p FROM test262.provenances WHERE provenance_id=NEW.provenance_id;
 SELECT * INTO f FROM test262.fixtures WHERE fixture_id=NEW.fixture_id;
 IF p.corpus_id IS DISTINCT FROM f.corpus_id OR p.repository_id IS DISTINCT FROM NEW.repository_id THEN
   RAISE EXCEPTION 'Fixture/provenance scope mismatch' USING ERRCODE='23514';
 END IF;
 SELECT inventory_state INTO ps FROM test262.corpora WHERE corpus_id=p.corpus_id;
 IF ps<>'sealed' THEN RAISE EXCEPTION 'Execution requires sealed corpus' USING ERRCODE='23514'; END IF;
 IF TG_TABLE_NAME IN ('work_items','observations') AND f.is_support_file THEN RAISE EXCEPTION 'Support file is not executable' USING ERRCODE='23514'; END IF;
 IF TG_TABLE_NAME='observations' THEN
   SELECT * INTO r FROM test262.runs WHERE run_id=NEW.run_id;
   IF r.provenance_id IS DISTINCT FROM NEW.provenance_id THEN RAISE EXCEPTION 'Observation/run provenance mismatch' USING ERRCODE='23514'; END IF;
   IF NEW.work_item_id IS NOT NULL THEN
     SELECT * INTO w FROM test262.work_items WHERE work_item_id=NEW.work_item_id;
     IF ROW(w.provenance_id,w.fixture_id,w.variant) IS DISTINCT FROM ROW(NEW.provenance_id,NEW.fixture_id,NEW.variant) THEN RAISE EXCEPTION 'Observation/work mismatch' USING ERRCODE='23514'; END IF;
   END IF;
   SELECT * INTO v FROM test262.fixture_variants WHERE fixture_id=NEW.fixture_id AND variant=NEW.variant;
   IF p.evidence_kind='native' AND NEW.source_kind='live' AND NEW.outcome='pass' AND v.expected_phase IS NOT NULL THEN
     IF v.expected_phase<>'runtime' OR NEW.phase<>'runtime' OR NEW.observed_error_type IS DISTINCT FROM v.expected_error_type THEN
       RAISE EXCEPTION 'Native negative lacks matching phase/type' USING ERRCODE='23514';
     END IF;
   END IF;
 ELSIF TG_TABLE_NAME='work_items' THEN
  IF NEW.state='completed' THEN
   IF NOT EXISTS(SELECT 1 FROM test262.observations o WHERE o.observation_id=NEW.completion_observation_id AND
     ROW(o.repository_id,o.provenance_id,o.fixture_id,o.variant,o.work_item_id,o.lease_generation)=
     ROW(NEW.repository_id,NEW.provenance_id,NEW.fixture_id,NEW.variant,NEW.work_item_id,NEW.lease_generation)) THEN
     RAISE EXCEPTION 'Completion observation does not match current work generation' USING ERRCODE='23514';
   END IF;
  END IF;
 END IF;
 RETURN NEW;
END $$;
