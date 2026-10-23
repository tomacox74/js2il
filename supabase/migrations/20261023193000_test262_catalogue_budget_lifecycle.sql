-- Reuse the claim/settlement owner and repository lock. No historical row repair.
SET LOCAL lock_timeout='5s';
ALTER TABLE test262.budget_scopes
 DROP CONSTRAINT budget_scopes_state_check,
 ADD CONSTRAINT budget_scopes_state_check CHECK(state IN ('open','exhausted','closed','interrupted','cancelled')),
 ADD COLUMN ended_at timestamptz,
 ADD COLUMN claim_cap_ms bigint CHECK(claim_cap_ms BETWEEN 1 AND 3600000),
 ADD COLUMN lifecycle jsonb NOT NULL DEFAULT '{}' CHECK(jsonb_typeof(lifecycle)='object');

-- Extend the established claim/settlement owner; charges and allocation remain shared.
CREATE OR REPLACE FUNCTION test262.api_claim(epoch bigint, run_id uuid, budget_id uuid, cap_ms bigint, ttl_seconds int)
RETURNS jsonb LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; b test262.budget_scopes; r test262.runs; w test262.work_items; reservation uuid; old jsonb;
BEGIN
 s=test262.api_identity(epoch,true); PERFORM test262.api_lock(s.repository_id);
 SELECT * INTO b FROM test262.budget_scopes WHERE budget_scope_id=budget_id AND repository_id=s.repository_id FOR UPDATE;
 IF b.external_key ~ '^actions:[1-9][0-9]*$' AND b.scope_kind='run' THEN
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

-- Computes a dry-run plan or applies exactly that plan under the same repository
-- lock as claims, completions and run finalization. Never releases a live lease.
CREATE FUNCTION test262.api_finish_budget(epoch bigint, id uuid, apply boolean)
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
    (x.producer_id<>s.producer_id OR x.trust_class<>'trusted' OR x.external_run_key !~ ('^'||b.external_key||':[1-9][0-9]*$'))) THEN
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
REVOKE ALL ON FUNCTION test262.api_claim(bigint,uuid,uuid,bigint,int),test262.api_finish_budget(bigint,uuid,boolean)
 FROM PUBLIC,anon,authenticated,service_role;
GRANT EXECUTE ON FUNCTION test262.api_claim(bigint,uuid,uuid,bigint,int),test262.api_finish_budget(bigint,uuid,boolean) TO test262_coordinator;
