-- One managed Actions collection with a separately authenticated execution slot.
SET LOCAL lock_timeout = '5s';

CREATE TABLE test262.collection_attempts (
  run_id uuid PRIMARY KEY,
  repository_id uuid NOT NULL,
  budget_scope_id uuid NOT NULL UNIQUE,
  authority_epoch bigint NOT NULL,
  context jsonb NOT NULL,
  context_sha256 test262.sha256 NOT NULL,
  created_at timestamptz NOT NULL DEFAULT clock_timestamp(),
  expires_at timestamptz NOT NULL,
  phase text NOT NULL DEFAULT 'prepared' CHECK (phase IN ('prepared','open','closing')),
  artifact_id bigint CHECK (artifact_id > 0),
  artifact_digest test262.sha256,
  UNIQUE (repository_id, run_id),
  FOREIGN KEY (repository_id, run_id) REFERENCES test262.runs(repository_id, run_id),
  FOREIGN KEY (repository_id, budget_scope_id) REFERENCES test262.budget_scopes(repository_id, budget_scope_id),
  CHECK ((artifact_id IS NULL) = (artifact_digest IS NULL)),
  CHECK (expires_at > created_at)
);

CREATE TABLE test262.collection_workers (
  repository_id uuid NOT NULL,
  collection_run_id uuid NOT NULL,
  slot integer NOT NULL CHECK (slot > 0),
  producer_id uuid NOT NULL,
  run_id uuid UNIQUE,
  actions_job_id bigint CHECK (actions_job_id > 0),
  PRIMARY KEY (collection_run_id, slot),
  UNIQUE (collection_run_id, producer_id),
  FOREIGN KEY (repository_id, collection_run_id) REFERENCES test262.collection_attempts(repository_id, run_id),
  FOREIGN KEY (repository_id, producer_id) REFERENCES test262.producers(repository_id, producer_id),
  FOREIGN KEY (repository_id, run_id) REFERENCES test262.runs(repository_id, run_id),
  CHECK ((run_id IS NULL) = (actions_job_id IS NULL))
);

ALTER TABLE test262.collection_attempts ENABLE ROW LEVEL SECURITY;
ALTER TABLE test262.collection_workers ENABLE ROW LEVEL SECURITY;
REVOKE ALL ON test262.collection_attempts, test262.collection_workers FROM PUBLIC, anon, authenticated, service_role;

-- The owner or a registered worker may read the registered context. Only the
-- owner may commit artifacts, close admission, or reconcile a worker's state.
CREATE FUNCTION test262.api_collection(epoch bigint, id uuid) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE subject test262.api_subjects;
BEGIN
  subject := test262.api_identity(epoch, true);
  RETURN (
    SELECT to_jsonb(c) || jsonb_build_object(
      'workers', (SELECT jsonb_agg(to_jsonb(w) || jsonb_build_object('run_state', r.state) ORDER BY w.slot)
                  FROM test262.collection_workers w LEFT JOIN test262.runs r USING (run_id)
                  WHERE w.collection_run_id=c.run_id))
    FROM test262.collection_attempts c JOIN test262.runs owner USING (run_id)
    WHERE c.run_id=id AND c.repository_id=subject.repository_id AND c.authority_epoch=epoch
      AND subject.trust_class='trusted'
      AND (owner.producer_id=subject.producer_id OR EXISTS (
        SELECT 1 FROM test262.collection_workers w
        WHERE w.collection_run_id=c.run_id AND w.producer_id=subject.producer_id))
  );
END $$;

CREATE FUNCTION test262.api_register_collection(epoch bigint, plan jsonb) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE subject test262.api_subjects; owner test262.runs; budget test262.budget_scopes;
        existing test262.collection_attempts; ctx jsonb; work jsonb; rec jsonb;
        producer uuid; digest bytea; cursor_after bigint; chunk jsonb;
BEGIN
  subject := test262.api_identity(epoch, true);
  PERFORM test262.api_lock(subject.repository_id);
  ctx := plan->'context';
  work := plan->'work';
  rec := plan->'reconciliation';
  producer := (ctx->>'worker_producer')::uuid;
  digest := sha256(convert_to(test262.canonical_json(ctx),'UTF8'));
  SELECT * INTO owner FROM test262.runs WHERE run_id=(ctx->>'run')::uuid
    AND repository_id=subject.repository_id AND producer_id=subject.producer_id;
  SELECT * INTO budget FROM test262.budget_scopes WHERE budget_scope_id=(ctx->>'budget')::uuid
    AND repository_id=subject.repository_id;
  IF subject.trust_class <> 'trusted' OR owner.trust_class IS DISTINCT FROM 'trusted'
      OR owner.state IS DISTINCT FROM 'running'
      OR (SELECT deployment_state FROM test262.schema_contract) <> 'active'
      OR ctx->>'managed_version' IS DISTINCT FROM '1'
      OR ctx->>'actions_run_id' IS NULL OR ctx->>'actions_run_id' !~ '^[1-9][0-9]*$'
      OR ctx->>'actions_attempt' IS DISTINCT FROM '1'
      OR ctx->>'event_sha' IS NULL OR ctx->>'event_sha' !~ '^[0-9a-f]{40}$'
      OR owner.external_run_key IS DISTINCT FROM ('actions:' || (ctx->>'actions_run_id') || ':1')
      OR budget.external_key IS DISTINCT FROM ('actions:' || (ctx->>'actions_run_id'))
      OR budget.scope_kind IS DISTINCT FROM 'run' OR budget.state IS DISTINCT FROM 'open'
      OR ctx->>'repository' IS DISTINCT FROM subject.repository_id::text
      OR ctx->>'producer' IS DISTINCT FROM subject.producer_id::text
      OR ctx->>'provenance' IS DISTINCT FROM owner.provenance_id::text
      OR ctx->>'revision' IS DISTINCT FROM owner.source_revision
      OR (ctx->>'cap_ms')::bigint IS NULL OR (ctx->>'cap_ms')::bigint NOT BETWEEN 1 AND 3600000
      OR producer IS NULL OR producer=subject.producer_id
      OR NOT EXISTS (SELECT 1 FROM test262.producers p JOIN test262.api_subjects s USING (repository_id,producer_id)
                     WHERE p.repository_id=subject.repository_id AND p.producer_id=producer AND p.enabled
                       AND s.enabled AND s.trust_class='trusted' AND s.permission='coordinator'
                       AND pg_has_role(s.login_name,'test262_worker','MEMBER')
                       AND NOT pg_has_role(s.login_name,'test262_coordinator','MEMBER')) THEN
    RAISE EXCEPTION 'Invalid managed collection owner, scope, or worker' USING ERRCODE='42501';
  END IF;

  SELECT * INTO existing FROM test262.collection_attempts WHERE run_id=owner.run_id;
  IF FOUND THEN
    IF existing.context_sha256 IS DISTINCT FROM digest OR existing.authority_epoch<>epoch THEN
      RAISE EXCEPTION 'Conflicting managed preparation replay' USING ERRCODE='40001';
    END IF;
    RETURN test262.api_collection(epoch, owner.run_id);
  END IF;

  IF jsonb_typeof(work) IS DISTINCT FROM 'array' OR jsonb_array_length(work)>2000
      OR (ctx->>'work_count')::int IS DISTINCT FROM jsonb_array_length(work)
      OR ctx->>'work_digest' IS DISTINCT FROM encode(sha256(convert_to(
           test262.canonical_json((SELECT coalesce(jsonb_agg(w->>'work_item_id' ORDER BY w->>'work_item_id'),'[]')
                                   FROM jsonb_array_elements(work) w)), 'UTF8')), 'hex')
      OR ctx->'limits' IS DISTINCT FROM jsonb_build_object('candidate_limit',budget.candidate_limit,
         'accepted_limit',budget.accepted_limit,'attempt_limit',budget.attempt_limit,'time_limit_ms',budget.time_limit_ms)
      OR EXISTS (SELECT 1 FROM jsonb_array_elements(work) w
                 WHERE w->>'repository_id' IS DISTINCT FROM owner.repository_id::text
                    OR w->>'provenance_id' IS DISTINCT FROM owner.provenance_id::text)
      OR EXISTS (SELECT 1 FROM test262.budget_work_items bw WHERE bw.budget_scope_id=budget.budget_scope_id)
      OR NOT EXISTS (
        SELECT 1 FROM test262.reporting_targets t
        WHERE t.repository_id=owner.repository_id AND t.channel='master'
          AND t.evidence_kind=ctx->>'kind' AND t.provenance_id=owner.provenance_id
          AND t.registration_snapshot_id=(ctx->>'registration_snapshot')::uuid
          AND t.validation_id IS NOT DISTINCT FROM (ctx->>'validation')::uuid
      ) THEN
    RAISE EXCEPTION 'Invalid or stale managed work plan';
  END IF;

  -- Freeze membership and advance discovery in the same transaction.
  IF rec IS NOT NULL AND rec <> 'null'::jsonb THEN
    cursor_after := (plan->>'next_cursor')::bigint;
    PERFORM test262.api_put(epoch,'reconciliations',jsonb_build_array(rec));
    PERFORM test262.api_reconcile(epoch,(rec->>'reconciliation_id')::uuid,
      (rec->>'expected_cursor_version')::bigint,cursor_after,work);
  ELSE
    FOR chunk IN
      SELECT jsonb_agg(value ORDER BY position)
      FROM jsonb_array_elements(work) WITH ORDINALITY AS items(value, position)
      GROUP BY (position-1)/250
    LOOP
      PERFORM test262.api_put(epoch,'work_items',chunk);
    END LOOP;
  END IF;
  INSERT INTO test262.budget_work_items(repository_id,budget_scope_id,work_item_id)
    SELECT owner.repository_id,budget.budget_scope_id,(w->>'work_item_id')::uuid
    FROM jsonb_array_elements(work) w;
  INSERT INTO test262.collection_attempts(run_id,repository_id,budget_scope_id,authority_epoch,context,context_sha256,expires_at)
    VALUES(owner.run_id,owner.repository_id,budget.budget_scope_id,epoch,ctx,digest,clock_timestamp()+interval '60 minutes');
  INSERT INTO test262.collection_workers(repository_id,collection_run_id,slot,producer_id)
    VALUES(owner.repository_id,owner.run_id,1,producer);
  RETURN test262.api_collection(epoch,owner.run_id);
END $$;

CREATE FUNCTION test262.api_open_collection(epoch bigint, id uuid, artifact bigint, digest text) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE subject test262.api_subjects; collection test262.collection_attempts; bytes bytea;
BEGIN
  subject := test262.api_identity(epoch,true);
  PERFORM test262.api_lock(subject.repository_id);
  bytes := decode(digest,'hex');
  SELECT c.* INTO collection FROM test262.collection_attempts c JOIN test262.runs r USING (run_id)
    WHERE c.run_id=id AND c.repository_id=subject.repository_id AND r.producer_id=subject.producer_id
      AND c.authority_epoch=epoch AND r.state='running';
  IF collection.run_id IS NULL OR subject.trust_class<>'trusted' OR artifact IS NULL OR artifact<=0
      OR octet_length(bytes) IS DISTINCT FROM 32 OR collection.expires_at<=clock_timestamp()
      OR collection.phase='closing' THEN
    RAISE EXCEPTION 'Cannot open managed collection' USING ERRCODE='42501';
  END IF;
  IF collection.phase='open' AND ROW(collection.artifact_id,collection.artifact_digest) IS DISTINCT FROM ROW(artifact,bytes) THEN
    RAISE EXCEPTION 'Conflicting preparation artifact' USING ERRCODE='40001';
  END IF;
  UPDATE test262.collection_attempts SET artifact_id=artifact,artifact_digest=bytes,phase='open' WHERE run_id=id;
  RETURN test262.api_collection(epoch,id);
END $$;

CREATE FUNCTION test262.api_join_collection(epoch bigint, id uuid, child uuid, job_id bigint, digest text) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE subject test262.api_subjects; collection test262.collection_attempts;
        member test262.collection_workers; owner test262.runs; result jsonb;
BEGIN
  subject := test262.api_identity(epoch,true);
  PERFORM test262.api_lock(subject.repository_id);
  SELECT * INTO collection FROM test262.collection_attempts WHERE run_id=id
    AND repository_id=subject.repository_id AND authority_epoch=epoch;
  SELECT * INTO member FROM test262.collection_workers WHERE collection_run_id=id AND producer_id=subject.producer_id;
  SELECT * INTO owner FROM test262.runs WHERE run_id=id;
  IF member.collection_run_id IS NULL OR subject.trust_class<>'trusted'
      OR (SELECT deployment_state FROM test262.schema_contract)<>'active'
      OR NOT pg_has_role(session_user,'test262_worker','MEMBER')
      OR pg_has_role(session_user,'test262_coordinator','MEMBER')
      OR collection.phase IS DISTINCT FROM 'open' OR collection.expires_at<=clock_timestamp()
      OR collection.context_sha256 IS DISTINCT FROM decode(digest,'hex')
      OR owner.state IS DISTINCT FROM 'running' OR job_id IS NULL OR job_id<=0
      OR NOT EXISTS (SELECT 1 FROM test262.reporting_targets t
        WHERE t.repository_id=owner.repository_id AND t.channel='master'
          AND t.evidence_kind=collection.context->>'kind' AND t.provenance_id=owner.provenance_id
          AND t.registration_snapshot_id=(collection.context->>'registration_snapshot')::uuid
          AND t.validation_id IS NOT DISTINCT FROM (collection.context->>'validation')::uuid) THEN
    RAISE EXCEPTION 'Worker is not admitted to this managed collection' USING ERRCODE='42501';
  END IF;
  IF member.run_id IS NOT NULL AND ROW(member.run_id,member.actions_job_id) IS DISTINCT FROM ROW(child,job_id) THEN
    RAISE EXCEPTION 'Conflicting worker attempt' USING ERRCODE='40001';
  END IF;
  result := test262.api_start_run(epoch,jsonb_build_object(
    'run_id',child,'provenance_id',owner.provenance_id,'source_revision',owner.source_revision,'run_kind',owner.run_kind,
    'environment_diagnostics',jsonb_build_object('managed_collection_run_id',id),
    'external_run_key','worker:' || subject.producer_id::text || ':' || owner.external_run_key || ':' || member.slot::text || ':1'));
  IF result->>'state' <> 'running' THEN
    RAISE EXCEPTION 'Terminal worker cannot claim again';
  END IF;
  UPDATE test262.collection_workers SET run_id=child,actions_job_id=job_id
    WHERE collection_run_id=id AND slot=member.slot;
  RETURN result;
END $$;

-- proof comes from the trusted coordinator's bounded Actions API read.
-- A worker never gains the ability to close another worker's run.
CREATE FUNCTION test262.api_finish_collection(epoch bigint, id uuid, proof jsonb) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE subject test262.api_subjects; collection test262.collection_attempts;
        member test262.collection_workers; owner test262.runs; child test262.runs;
        terminal_state text; reason text; finished jsonb; verified boolean;
BEGIN
  subject := test262.api_identity(epoch,true);
  PERFORM test262.api_lock(subject.repository_id);
  SELECT * INTO collection FROM test262.collection_attempts WHERE run_id=id
    AND repository_id=subject.repository_id AND authority_epoch=epoch;
  SELECT * INTO owner FROM test262.runs WHERE run_id=id AND producer_id=subject.producer_id
    AND repository_id=subject.repository_id AND trust_class='trusted';
  SELECT * INTO member FROM test262.collection_workers WHERE collection_run_id=id AND slot=1;
  IF collection.run_id IS NULL OR owner.run_id IS NULL OR subject.trust_class<>'trusted'
      OR (SELECT deployment_state FROM test262.schema_contract)<>'active'
      OR proof->>'state' IS NULL OR proof->>'state' NOT IN ('success','failure','cancelled','incomplete','skipped')
      OR (member.actions_job_id IS NOT NULL AND member.actions_job_id IS DISTINCT FROM (proof->>'job_id')::bigint) THEN
    RAISE EXCEPTION 'Invalid managed completion proof' USING ERRCODE='42501';
  END IF;
  UPDATE test262.collection_attempts SET phase='closing' WHERE run_id=id;
  IF owner.state IN ('complete','incomplete','failed','cancelled') THEN
    RETURN jsonb_build_object('run_id',id,'state',owner.state,'ended_at',owner.ended_at,
      'budget',test262.api_finish_budget(epoch,collection.budget_scope_id,true));
  END IF;

  SELECT * INTO child FROM test262.runs WHERE run_id=member.run_id AND producer_id=member.producer_id;
  verified := coalesce(proof->>'state'='success' AND proof->'retained'='true'::jsonb
    AND child.state='complete' AND child.environment_diagnostics->'collection_lifecycle'->'flush_verified'='true'::jsonb
    AND EXISTS (SELECT 1 FROM test262.run_metrics WHERE run_id=child.run_id AND stage='screening')
    AND NOT EXISTS (SELECT 1 FROM test262.observations WHERE run_id=child.run_id
                    AND outcome IN ('infrastructure-error','incomplete'))
    AND NOT EXISTS (SELECT 1 FROM test262.budget_reservations
      WHERE budget_scope_id=collection.budget_scope_id AND state IN ('reserved','uncertain')),false);
  terminal_state := CASE WHEN verified THEN 'complete' WHEN proof->>'state'='cancelled' THEN 'cancelled'
    WHEN proof->>'state'='failure' THEN 'failed' ELSE 'incomplete' END;
  reason := CASE terminal_state WHEN 'complete' THEN 'collection-finished' WHEN 'cancelled' THEN 'job-cancelled'
    WHEN 'failed' THEN 'job-failed' ELSE 'flush-unproven' END;
  IF child.state='running' THEN
    -- Membership and exact terminal job proof bound this coordinator transition.
    UPDATE test262.runs SET state=terminal_state, ended_at=clock_timestamp(),
      environment_diagnostics=environment_diagnostics || jsonb_build_object('collection_lifecycle',
        jsonb_build_object('source','github-actions','state',terminal_state,'external_run_key',child.external_run_key,
          'source_revision',child.source_revision,'reason',reason,'flush_verified',false))
      WHERE run_id=child.run_id;
    INSERT INTO test262.audit_events(repository_id,actor_subject,action,entity_kind,details)
      VALUES(owner.repository_id,session_user,'finish-collection-worker','runs',
        jsonb_build_object('run_id',child.run_id,'parent_run_id',id,'state',terminal_state,'job_id',member.actions_job_id));
  END IF;
  IF verified THEN
    INSERT INTO test262.run_metrics(repository_id,run_id,stage,duration_ms,attempt_count,timeout_count,measurement_version)
      SELECT owner.repository_id,id,'screening',
        greatest(0,(extract(epoch FROM clock_timestamp()-owner.started_at)*1000)::bigint),
        attempt_count,timeout_count,1 FROM test262.run_metrics WHERE run_id=child.run_id AND stage='screening';
  END IF;
  finished := test262.api_finish_run(epoch,id,terminal_state,clock_timestamp(),
    jsonb_build_object('source','github-actions','external_run_key',owner.external_run_key,
      'source_revision',owner.source_revision,'reason',reason,'flush_verified',verified));
  RETURN jsonb_build_object('run_id',id,'state',finished->>'state','ended_at',finished->>'ended_at',
    'budget',test262.api_finish_budget(epoch,collection.budget_scope_id,true));
END $$;

REVOKE ALL ON FUNCTION test262.api_collection(bigint,uuid),
  test262.api_register_collection(bigint,jsonb), test262.api_open_collection(bigint,uuid,bigint,text),
  test262.api_join_collection(bigint,uuid,uuid,bigint,text), test262.api_finish_collection(bigint,uuid,jsonb)
  FROM PUBLIC,anon,authenticated,service_role;
GRANT EXECUTE ON FUNCTION test262.api_collection(bigint,uuid) TO test262_coordinator,test262_worker;
GRANT EXECUTE ON FUNCTION test262.api_join_collection(bigint,uuid,uuid,bigint,text) TO test262_worker;
GRANT EXECUTE ON FUNCTION test262.api_register_collection(bigint,jsonb),
  test262.api_open_collection(bigint,uuid,bigint,text),test262.api_finish_collection(bigint,uuid,jsonb) TO test262_coordinator;

-- Preserve the serial claim and settlement contracts.
CREATE OR REPLACE FUNCTION test262.api_claim(epoch bigint, run_id uuid, budget_id uuid, cap_ms bigint, ttl_seconds int)
RETURNS jsonb LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; b test262.budget_scopes; r test262.runs; w test262.work_items; reservation uuid; old jsonb;
BEGIN
 s=test262.api_identity(epoch,true); PERFORM test262.api_lock(s.repository_id);
 SELECT * INTO b FROM test262.budget_scopes WHERE budget_scope_id=budget_id AND repository_id=s.repository_id FOR UPDATE;
 IF EXISTS (SELECT 1 FROM test262.collection_attempts WHERE budget_scope_id=budget_id) THEN
  IF NOT EXISTS (
    SELECT 1 FROM test262.collection_attempts c JOIN test262.runs owner ON owner.run_id=c.run_id
    JOIN test262.collection_workers member ON member.collection_run_id=c.run_id
    JOIN test262.runs worker ON worker.run_id=member.run_id
    JOIN test262.reporting_targets target ON target.repository_id=c.repository_id AND target.channel='master'
    WHERE c.budget_scope_id=budget_id AND c.repository_id=s.repository_id AND c.authority_epoch=epoch
      AND c.phase='open' AND c.expires_at>clock_timestamp()+make_interval(secs=>ttl_seconds)
      AND owner.state='running' AND worker.state='running' AND worker.run_id=api_claim.run_id
      AND member.producer_id=s.producer_id AND worker.producer_id=s.producer_id
      AND target.evidence_kind=c.context->>'kind' AND target.provenance_id=owner.provenance_id
      AND target.registration_snapshot_id=(c.context->>'registration_snapshot')::uuid
      AND target.validation_id IS NOT DISTINCT FROM (c.context->>'validation')::uuid
      AND cap_ms=(c.context->>'cap_ms')::bigint
  ) THEN RAISE EXCEPTION 'Managed claim denied by collection membership, expiry or freshness' USING ERRCODE='42501'; END IF;
  UPDATE test262.budget_scopes SET claim_cap_ms=cap_ms WHERE budget_scope_id=budget_id AND claim_cap_ms IS NULL;
 ELSIF EXISTS (SELECT 1 FROM test262.collection_workers member
                WHERE member.repository_id=s.repository_id AND member.producer_id=s.producer_id) THEN
  RAISE EXCEPTION 'Managed worker cannot claim another allocation' USING ERRCODE='42501';
 ELSIF b.external_key ~ '^actions:[1-9][0-9]*$' AND b.scope_kind='run' THEN
  SELECT * INTO r FROM test262.runs x WHERE x.run_id=api_claim.run_id AND x.repository_id=s.repository_id
   AND x.producer_id=s.producer_id AND x.trust_class='trusted' AND x.run_kind IN ('native','mvp') AND x.state='running';
  IF s.trust_class<>'trusted' OR r.run_id IS NULL OR
     r.external_run_key !~ ('^'||b.external_key||':[1-9][0-9]*$') OR
     EXISTS(SELECT 1 FROM test262.runs x WHERE x.repository_id=s.repository_id AND
       x.external_run_key ~ ('^'||b.external_key||':[1-9][0-9]*$') AND
       (x.producer_id<>s.producer_id OR x.trust_class<>'trusted' OR x.run_kind NOT IN ('native','mvp'))) THEN
   RAISE EXCEPTION 'Budget attempt identity mismatch' USING ERRCODE='42501';
  END IF;
  IF b.claim_cap_ms IS NOT NULL AND b.claim_cap_ms<>cap_ms THEN RAISE EXCEPTION 'Retry must preserve claim cap'; END IF;
  IF b.state<>'open' THEN
   -- Only a strictly newer Actions attempt may reopen a closed scope. Charges,
   -- memberships and limits survive; replaying the old attempt cannot reopen it.
   IF b.lifecycle->>'external_run_key' IS NULL OR
      split_part(r.external_run_key,':',3)::numeric <= split_part(b.lifecycle->>'external_run_key',':',3)::numeric THEN
    RETURN jsonb_build_object('budget_exhausted',true,'budget_state',b.state);
   END IF;
   old=to_jsonb(b);
   UPDATE test262.budget_scopes SET state='open',ended_at=NULL,lifecycle='{}',version=version+1 WHERE budget_scope_id=budget_id;
   INSERT INTO test262.audit_events(repository_id,actor_subject,action,entity_kind,details)
    VALUES(s.repository_id,session_user,'reopen-budget','budget_scopes',jsonb_build_object('before',old,'run_id',r.run_id,'external_run_key',r.external_run_key));
  END IF;
  UPDATE test262.budget_scopes SET claim_cap_ms=cap_ms WHERE budget_scope_id=budget_id AND claim_cap_ms IS NULL;
 END IF;
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
 IF w.work_item_id IS NULL THEN
  IF EXISTS (SELECT 1 FROM test262.collection_workers member WHERE member.run_id=api_claim.run_id)
     AND EXISTS (SELECT 1 FROM test262.budget_work_items bw JOIN test262.work_items item USING (work_item_id)
       WHERE bw.budget_scope_id=budget_id AND item.state IN ('pending','leased')) THEN
    RETURN jsonb_build_object('temporarily_unavailable',true);
  END IF;
  RETURN NULL;
 END IF;
 SELECT * INTO b FROM test262.budget_scopes x WHERE x.budget_scope_id=budget_id AND x.repository_id=s.repository_id FOR UPDATE;
 IF b.budget_scope_id IS NULL THEN RAISE EXCEPTION 'Invalid budget'; END IF;
 IF b.state<>'open' OR b.reserved_attempts+b.charged_attempts+1>b.attempt_limit OR b.reserved_ms+b.charged_ms+cap_ms>b.time_limit_ms THEN RETURN jsonb_build_object('budget_exhausted',true); END IF;
 UPDATE test262.budget_scopes SET reserved_attempts=reserved_attempts+1,reserved_ms=reserved_ms+cap_ms,version=version+1 WHERE budget_scope_id=budget_id;
 UPDATE test262.work_items SET state='leased',lease_generation=lease_generation+1,lease_owner=s.producer_id,lease_expires_at=clock_timestamp()+make_interval(secs=>ttl_seconds),version=version+1 WHERE work_item_id=w.work_item_id RETURNING * INTO w;
 INSERT INTO test262.work_leases(work_item_id,lease_generation,repository_id,producer_id,run_id,expires_at) VALUES(w.work_item_id,w.lease_generation,s.repository_id,s.producer_id,r.run_id,w.lease_expires_at);
 INSERT INTO test262.budget_reservations(repository_id,budget_scope_id,work_item_id,lease_generation,reserved_attempts,reserved_ms) VALUES(s.repository_id,budget_id,w.work_item_id,w.lease_generation,1,cap_ms) RETURNING reservation_id INTO reservation;
 RETURN to_jsonb(w)||jsonb_build_object('reservation_id',reservation,'cap_ms',cap_ms);
END $$;

CREATE OR REPLACE FUNCTION test262.api_finish_budget(epoch bigint, id uuid, apply boolean)
RETURNS jsonb LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; b test262.budget_scopes; r test262.runs; w test262.work_items;
 res test262.budget_reservations; pending_count bigint; terminal_state text; reason text; plan jsonb; before_row jsonb;
BEGIN
 s=test262.api_identity(epoch,true); PERFORM test262.api_lock(s.repository_id);
 IF s.trust_class<>'trusted' OR (SELECT deployment_state FROM test262.schema_contract)<>'active' THEN
  RAISE EXCEPTION 'Trusted active coordinator required' USING ERRCODE='42501';
 END IF;
 SELECT * INTO b FROM test262.budget_scopes WHERE budget_scope_id=id AND repository_id=s.repository_id FOR UPDATE;
 IF b.budget_scope_id IS NULL THEN RAISE EXCEPTION 'Missing scoped budget' USING ERRCODE='42501'; END IF;
 plan=jsonb_build_object('budget_scope_id',id,'before_state',b.state,'version',b.version);
 IF b.scope_kind<>'run' OR b.external_key !~ '^actions:[1-9][0-9]*$' THEN
  RETURN plan||jsonb_build_object('disposition','unsupported-scope');
 END IF;
 IF EXISTS(SELECT 1 FROM test262.runs x WHERE x.repository_id=s.repository_id AND
    x.external_run_key ~ ('^'||b.external_key||':[1-9][0-9]*$') AND
    (x.producer_id<>s.producer_id OR x.trust_class<>'trusted' OR x.run_kind NOT IN ('native','mvp'))) THEN
  RETURN plan||jsonb_build_object('disposition','ambiguous-owner');
 END IF;
 SELECT * INTO r FROM test262.runs x WHERE x.repository_id=s.repository_id AND x.producer_id=s.producer_id
  AND x.external_run_key ~ ('^'||b.external_key||':[1-9][0-9]*$')
  ORDER BY split_part(x.external_run_key,':',3)::numeric DESC LIMIT 1;
 IF r.run_id IS NULL THEN RETURN plan||jsonb_build_object('disposition','missing-run'); END IF;
 plan=plan||jsonb_build_object('run_id',r.run_id,'external_run_key',r.external_run_key,'source_revision',r.source_revision);
 IF EXISTS(SELECT 1 FROM test262.runs x WHERE x.repository_id=s.repository_id AND
    x.external_run_key ~ ('^'||b.external_key||':[1-9][0-9]*$') AND
    (x.state IN ('planned','running') OR x.ended_at IS NULL OR
     x.environment_diagnostics->'collection_lifecycle'->>'state' IS DISTINCT FROM x.state)) THEN
  RETURN plan||jsonb_build_object('disposition','active-or-unproven-run');
 END IF;
 -- A durable same-attempt receipt is idempotent even if a later, unrelated
 -- budget subsequently claims shared work. It cannot re-settle or re-charge.
 IF b.ended_at IS NOT NULL AND b.lifecycle->>'external_run_key'=r.external_run_key THEN
  RETURN plan||jsonb_build_object('disposition','unchanged','result',to_jsonb(b));
 END IF;
 IF EXISTS (
   SELECT 1 FROM test262.collection_attempts c JOIN test262.collection_workers w ON w.collection_run_id=c.run_id
   LEFT JOIN test262.runs child ON child.run_id=w.run_id
   WHERE c.budget_scope_id=id AND
     (c.phase<>'closing' OR (w.run_id IS NOT NULL AND (child.state IN ('running','planned') OR child.ended_at IS NULL)))
 ) THEN RETURN plan||jsonb_build_object('disposition','active-or-unproven-worker'); END IF;
 IF EXISTS(SELECT 1 FROM test262.work_items x JOIN test262.budget_work_items bw USING(work_item_id)
    WHERE bw.budget_scope_id=id AND x.state='leased' AND x.lease_expires_at>clock_timestamp()) OR
    EXISTS(SELECT 1 FROM test262.budget_reservations br JOIN test262.work_leases wl USING(work_item_id,lease_generation)
    WHERE br.budget_scope_id=id AND br.state='reserved' AND wl.released_at IS NULL AND wl.expires_at>clock_timestamp()) THEN
  RETURN plan||jsonb_build_object('disposition','live-reservations');
 END IF;
 IF EXISTS(SELECT 1 FROM test262.work_items x JOIN test262.budget_work_items bw USING(work_item_id)
    WHERE bw.budget_scope_id=id AND x.state='leased' AND NOT EXISTS(
      SELECT 1 FROM test262.budget_reservations br WHERE br.budget_scope_id=id AND
       br.work_item_id=x.work_item_id AND br.lease_generation=x.lease_generation AND br.state='reserved')) THEN
  RETURN plan||jsonb_build_object('disposition','awaiting-shared-work-settlement');
 END IF;
 IF EXISTS(SELECT 1 FROM test262.budget_reservations br JOIN test262.work_leases wl USING(work_item_id,lease_generation)
    JOIN test262.runs x ON x.run_id=wl.run_id WHERE br.budget_scope_id=id AND
    (x.producer_id<>s.producer_id OR x.trust_class<>'trusted' OR x.external_run_key !~ ('^'||b.external_key||':[1-9][0-9]*$'))
    AND NOT EXISTS (SELECT 1 FROM test262.collection_attempts c JOIN test262.collection_workers w ON w.collection_run_id=c.run_id
      WHERE c.budget_scope_id=id AND c.run_id=r.run_id AND w.run_id=x.run_id
        AND w.producer_id=x.producer_id AND x.trust_class='trusted')) THEN
  RETURN plan||jsonb_build_object('disposition','foreign-reservation');
 END IF;
 IF b.reserved_attempts<>(SELECT coalesce(sum(reserved_attempts),0) FROM test262.budget_reservations WHERE budget_scope_id=id AND state='reserved') OR
    b.reserved_ms<>(SELECT coalesce(sum(reserved_ms),0) FROM test262.budget_reservations WHERE budget_scope_id=id AND state='reserved') THEN
  RETURN plan||jsonb_build_object('disposition','reservation-accounting-mismatch');
 END IF;
 -- Preview conservative abandoned-reservation charging without mutating it.
 plan=plan||jsonb_build_object('expired_reservations',(SELECT count(*) FROM test262.budget_reservations WHERE budget_scope_id=id AND state='reserved'));
 b.charged_attempts=b.charged_attempts+b.reserved_attempts; b.charged_ms=b.charged_ms+b.reserved_ms;
 SELECT count(*) INTO pending_count FROM test262.budget_work_items bw JOIN test262.work_items x USING(work_item_id)
  WHERE bw.budget_scope_id=id AND (x.state='pending' OR (x.state='leased' AND x.lease_generation<x.attempt_limit));
 IF r.state='cancelled' THEN terminal_state='cancelled'; reason='collection-cancelled';
 ELSIF r.state<>'complete' THEN terminal_state='interrupted'; reason='collection-'||r.state;
 ELSIF pending_count=0 THEN terminal_state='closed'; reason='drained';
 ELSIF b.charged_attempts>=b.attempt_limit OR b.charged_ms>=b.time_limit_ms OR
    (b.claim_cap_ms IS NOT NULL AND b.charged_ms+b.claim_cap_ms>b.time_limit_ms) THEN
  terminal_state='exhausted'; reason='remaining-capacity-insufficient';
 ELSE terminal_state='interrupted'; reason='collection-stopped-with-pending-work'; END IF;
 plan=plan||jsonb_build_object('disposition','terminal','state',terminal_state,'reason',reason,'pending_work',pending_count,
  'charged_attempts',b.charged_attempts,'charged_ms',b.charged_ms,'claim_cap_ms',b.claim_cap_ms,
  'collection_state',r.state,'collection_ended_at',r.ended_at,'flush_verified',r.environment_diagnostics->'collection_lifecycle'->'flush_verified');
 IF apply IS DISTINCT FROM true THEN RETURN plan; END IF;
 SELECT to_jsonb(x) INTO before_row FROM test262.budget_scopes x WHERE budget_scope_id=id;
 FOR res IN SELECT * FROM test262.budget_reservations WHERE budget_scope_id=id AND state='reserved' ORDER BY work_item_id LOOP
  PERFORM test262.api_settle(res.work_item_id,res.lease_generation,NULL);
  UPDATE test262.work_leases SET released_at=coalesce(released_at,clock_timestamp()),release_reason=coalesce(release_reason,'expired')
   WHERE work_item_id=res.work_item_id AND lease_generation=res.lease_generation;
  SELECT * INTO w FROM test262.work_items WHERE work_item_id=res.work_item_id FOR UPDATE;
  IF w.state='leased' AND w.lease_generation=res.lease_generation THEN
   UPDATE test262.work_items SET state=CASE WHEN lease_generation>=attempt_limit THEN 'deferred' ELSE 'pending' END,
    lease_owner=NULL,lease_expires_at=NULL,version=version+1 WHERE work_item_id=w.work_item_id;
  END IF;
 END LOOP;
 UPDATE test262.budget_scopes SET state=terminal_state,ended_at=clock_timestamp(),lifecycle=plan,version=version+1
  WHERE budget_scope_id=id RETURNING * INTO b;
 INSERT INTO test262.audit_events(repository_id,actor_subject,action,entity_kind,details)
  VALUES(s.repository_id,session_user,'finish-budget','budget_scopes',jsonb_build_object('before',before_row,'after',to_jsonb(b)));
 RETURN plan||jsonb_build_object('result',to_jsonb(b));
END $$;

CREATE FUNCTION test262.guard_managed_native_seal() RETURNS trigger
LANGUAGE plpgsql SET search_path=pg_catalog AS $$
BEGIN
  IF NEW.state='sealed' AND OLD.state='planned' THEN
    IF EXISTS (SELECT 1 FROM test262.collection_attempts c JOIN test262.runs r USING (run_id)
       JOIN test262.budget_scopes b USING (budget_scope_id)
       WHERE c.budget_scope_id=NEW.budget_scope_id AND
         (c.phase<>'closing' OR r.state<>'complete' OR b.state NOT IN ('closed','exhausted','interrupted')
           OR b.lifecycle->>'collection_state' IS DISTINCT FROM 'complete'
           OR EXISTS (SELECT 1 FROM test262.batch_fixtures bf WHERE bf.batch_id=NEW.batch_id
                      AND NOT ((c.context->'candidate_ids') ? bf.fixture_id::text)))) THEN
      RAISE EXCEPTION 'Managed collection has not reached its acceptance barrier';
    END IF;
    IF EXISTS (
      SELECT 1 FROM test262.batch_evidence e JOIN test262.observations o USING (observation_id)
      JOIN test262.collection_workers member ON member.run_id=o.run_id
      LEFT JOIN test262.work_items work ON work.work_item_id=o.work_item_id
      WHERE e.batch_id=NEW.batch_id AND (work.completion_observation_id IS DISTINCT FROM o.observation_id
        OR work.lease_generation IS DISTINCT FROM o.lease_generation)
    ) THEN
      RAISE EXCEPTION 'Managed evidence has no accepted completion';
    END IF;
  END IF;
  RETURN NEW;
END $$;
REVOKE EXECUTE ON FUNCTION test262.guard_managed_native_seal() FROM PUBLIC;
CREATE TRIGGER managed_native_seal BEFORE UPDATE ON test262.native_batches
  FOR EACH ROW EXECUTE FUNCTION test262.guard_managed_native_seal();


CREATE OR REPLACE FUNCTION test262.api_export_open(epoch bigint, t text) RETURNS refcursor
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog
SET cursor_tuple_fraction='1.0' AS $function$
DECLARE s test262.api_subjects; rel regclass; predicate text; portal refcursor;
BEGIN
 s=test262.api_identity(epoch,true);
 IF s.trust_class<>'trusted' THEN
  RAISE EXCEPTION 'Trusted coordinator required' USING ERRCODE='42501';
 END IF;
 IF current_setting('transaction_read_only')<>'on' OR
    current_setting('transaction_isolation') NOT IN ('repeatable read','serializable') THEN
  RAISE EXCEPTION 'Bulk export requires a read-only repeatable snapshot' USING ERRCODE='25000';
 END IF;
 IF t IS NULL OR NOT t=ANY(ARRAY['corpora','repository_corpora','fixtures','fixture_specification_metadata','fixture_variants','fixture_dependencies','provenances','provenance_fixture_eligibility','runs','observations','observation_payloads','observation_invalidations','validation_events','work_items','work_leases','budget_reservations','budget_scopes','budget_work_items','registration_snapshots','registrations','reporting_targets','native_batches','batch_fixtures','batch_evidence','publications','publication_checks','imports','import_records','legacy_control_records','reconciliation_state','reconciliations','run_metrics','collection_attempts','collection_workers']) THEN
  RAISE EXCEPTION 'Unsupported record type';
 END IF;
 rel=format('test262.%I',t)::regclass;
 IF EXISTS(SELECT 1 FROM pg_attribute WHERE attrelid=rel AND attname='repository_id' AND attnum>0 AND NOT attisdropped) THEN
  predicate='x.repository_id=$1';
 ELSE
  CASE t
   WHEN 'corpora','fixtures' THEN
    predicate='EXISTS (SELECT 1 FROM test262.repository_corpora rc WHERE rc.repository_id=$1 AND rc.corpus_id=x.corpus_id)';
   WHEN 'fixture_variants','fixture_dependencies','fixture_specification_metadata' THEN
    predicate='EXISTS (SELECT 1 FROM test262.fixtures f JOIN test262.repository_corpora rc USING(corpus_id) WHERE rc.repository_id=$1 AND f.fixture_id=x.fixture_id)';
   WHEN 'registrations' THEN
    predicate='EXISTS (SELECT 1 FROM test262.registration_snapshots p WHERE p.repository_id=$1 AND p.snapshot_id=x.snapshot_id)';
   WHEN 'observation_payloads','observation_invalidations' THEN
    predicate='EXISTS (SELECT 1 FROM test262.observations p WHERE p.repository_id=$1 AND p.observation_id=x.observation_id)';
   WHEN 'batch_fixtures','batch_evidence' THEN
    predicate='EXISTS (SELECT 1 FROM test262.native_batches p WHERE p.repository_id=$1 AND p.batch_id=x.batch_id)';
   WHEN 'publication_checks' THEN
    predicate='EXISTS (SELECT 1 FROM test262.publications p WHERE p.repository_id=$1 AND p.publication_id=x.publication_id)';
   WHEN 'import_records','legacy_control_records' THEN
    predicate='EXISTS (SELECT 1 FROM test262.imports p WHERE p.repository_id=$1 AND p.import_id=x.import_id)';
   ELSE RAISE EXCEPTION 'Unsupported ownership relation';
  END CASE;
 END IF;
 portal='test262_export_'||replace(gen_random_uuid()::text,'-','');
 -- No sort, LIMIT, OFFSET or per-page JSON aggregation: consume one scan.
 -- A cursor retains scan position; row identities, not line order, join evidence.
 OPEN portal NO SCROLL FOR EXECUTE format('SELECT to_jsonb(x) FROM test262.%I x WHERE %s',t,predicate)
  USING s.repository_id;
 RETURN portal;
END $function$;

CREATE OR REPLACE FUNCTION test262.api_contract() RETURNS jsonb
LANGUAGE sql SECURITY DEFINER SET search_path=pg_catalog AS $function$
 SELECT (to_jsonb(c)-'singleton')||jsonb_build_object(
  'repository_id',s.repository_id,'producer_id',s.producer_id,
  'trust_class',s.trust_class,'permission',s.permission,'bulk_export_version',1,'managed_actions_collection_version',1)
 FROM test262.schema_contract c CROSS JOIN test262.api_subjects s
 WHERE s.login_name=session_user AND s.enabled;
$function$;

CREATE VIEW test262_reporting.v_collection_workers WITH (security_barrier=true) AS
SELECT c.repository_id,c.run_id AS collection_run_id,c.budget_scope_id,c.phase,
       parent.state AS collection_state,parent.source_revision,parent.provenance_id,
       w.slot,w.producer_id,w.run_id AS worker_run_id,w.actions_job_id,
       child.state AS worker_state,m.duration_ms AS worker_active_ms,m.attempt_count,m.timeout_count
FROM test262.collection_attempts c
JOIN test262.runs parent ON parent.run_id=c.run_id
JOIN test262.collection_workers w ON w.collection_run_id=c.run_id
LEFT JOIN test262.runs child ON child.run_id=w.run_id
LEFT JOIN test262.run_metrics m ON m.run_id=child.run_id AND m.stage='screening';
REVOKE ALL ON test262_reporting.v_collection_workers FROM PUBLIC,anon,authenticated,service_role;
GRANT SELECT ON test262_reporting.v_collection_workers TO test262_reporter;
DO $$
BEGIN
  IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname='test262_backup') THEN
    GRANT SELECT ON test262.collection_attempts,test262.collection_workers,
      test262_reporting.v_collection_workers TO test262_backup;
  END IF;
END $$;


CREATE OR REPLACE FUNCTION test262.api_renew(epoch bigint, work_id uuid, generation bigint, ttl_seconds int) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; w test262.work_items;
BEGIN
 s=test262.api_identity(epoch,true); PERFORM test262.api_lock(s.repository_id);
 IF EXISTS (SELECT 1 FROM test262.work_leases l JOIN test262.collection_workers managed_worker ON managed_worker.run_id=l.run_id
   WHERE l.repository_id=s.repository_id AND l.work_item_id=work_id AND l.lease_generation=generation) THEN
  RAISE EXCEPTION 'Managed leases have a fixed expiration' USING ERRCODE='42501';
 END IF;
 IF ttl_seconds NOT BETWEEN 30 AND 7200 THEN RAISE EXCEPTION 'Invalid TTL'; END IF;
 UPDATE test262.work_items SET lease_expires_at=clock_timestamp()+make_interval(secs=>ttl_seconds),version=version+1 WHERE work_item_id=work_id AND repository_id=s.repository_id AND lease_owner=s.producer_id AND lease_generation=generation AND state='leased' AND lease_expires_at>clock_timestamp() RETURNING * INTO w;
 IF w.work_item_id IS NULL THEN RAISE EXCEPTION 'Stale lease' USING ERRCODE='40001'; END IF;
 UPDATE test262.work_leases SET expires_at=w.lease_expires_at WHERE work_item_id=work_id AND lease_generation=generation;
 RETURN to_jsonb(w);
END $$;

CREATE FUNCTION test262.guard_collection_membership() RETURNS trigger
LANGUAGE plpgsql SET search_path=pg_catalog AS $$
BEGIN
  IF TG_OP <> 'INSERT' AND EXISTS (SELECT 1 FROM test262.collection_attempts
                                    WHERE budget_scope_id=OLD.budget_scope_id) THEN
    RAISE EXCEPTION 'Managed candidate membership is frozen';
  END IF;
  IF TG_OP <> 'DELETE' AND EXISTS (SELECT 1 FROM test262.collection_attempts
                                    WHERE budget_scope_id=NEW.budget_scope_id) THEN
    RAISE EXCEPTION 'Managed candidate membership is frozen';
  END IF;
  IF TG_OP='DELETE' THEN RETURN OLD; END IF;
  RETURN NEW;
END $$;
REVOKE EXECUTE ON FUNCTION test262.guard_collection_membership() FROM PUBLIC;
CREATE TRIGGER frozen_collection_membership BEFORE INSERT OR UPDATE OR DELETE ON test262.budget_work_items
  FOR EACH ROW EXECUTE FUNCTION test262.guard_collection_membership();

-- Keep the aggregate in the established pipeline view and expose child timings
-- separately through v_collection_workers.
CREATE OR REPLACE VIEW test262_reporting.v_pipeline_metrics WITH (security_barrier=true) AS
SELECT m.repository_id,m.run_id,r.provenance_id,r.run_kind,r.state,m.stage,m.duration_ms,
       m.attempt_count,m.timeout_count,m.measurement_version,r.started_at,r.ended_at
FROM test262.run_metrics m JOIN test262.runs r ON r.run_id=m.run_id
WHERE NOT EXISTS (SELECT 1 FROM test262.collection_workers w WHERE w.run_id=m.run_id);
