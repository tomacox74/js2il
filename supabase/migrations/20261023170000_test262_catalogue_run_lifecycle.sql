-- Collection lifecycle only: no test verdicts, queue/budget settlement or legacy rewrites.
SET LOCAL lock_timeout='5s';
CREATE FUNCTION test262.guard_run_lifecycle() RETURNS trigger
LANGUAGE plpgsql SET search_path=pg_catalog AS $$
BEGIN
 IF OLD.trust_class='trusted' AND OLD.run_kind IN ('mvp','native') THEN
  IF OLD.state IN ('complete','incomplete','failed','cancelled') AND
     ROW(NEW.state,NEW.ended_at,NEW.environment_diagnostics->'collection_lifecycle') IS DISTINCT FROM
     ROW(OLD.state,OLD.ended_at,OLD.environment_diagnostics->'collection_lifecycle') THEN
   RAISE EXCEPTION 'Terminal collection lifecycle is immutable' USING ERRCODE='40001';
  END IF;
  IF ROW(NEW.state,NEW.ended_at) IS DISTINCT FROM ROW(OLD.state,OLD.ended_at) AND
     (NEW.state NOT IN ('complete','incomplete','failed','cancelled') OR NEW.ended_at IS NULL OR
      NEW.environment_diagnostics->'collection_lifecycle'->>'state' IS DISTINCT FROM NEW.state) THEN
   RAISE EXCEPTION 'Collection transition requires lifecycle evidence';
  END IF;
 END IF;
 RETURN NEW;
END $$;
CREATE TRIGGER run_lifecycle_guard BEFORE UPDATE ON test262.runs
 FOR EACH ROW EXECUTE FUNCTION test262.guard_run_lifecycle();

CREATE FUNCTION test262.api_finish_run(epoch bigint, id uuid, terminal_state text, ended timestamptz, evidence jsonb)
RETURNS jsonb LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; r test262.runs; result test262.runs; proof jsonb;
BEGIN
 s=test262.api_identity(epoch,true); PERFORM test262.api_lock(s.repository_id);
 SELECT * INTO r FROM test262.runs WHERE run_id=id AND repository_id=s.repository_id AND producer_id=s.producer_id FOR UPDATE;
 IF s.trust_class<>'trusted' OR r.run_id IS NULL OR r.trust_class<>'trusted' OR r.run_kind NOT IN ('mvp','native') OR
    (SELECT deployment_state FROM test262.schema_contract)<>'active' THEN
  RAISE EXCEPTION 'Trusted active collection owner required' USING ERRCODE='42501';
 END IF;
 IF terminal_state IS NULL OR terminal_state NOT IN ('complete','incomplete','failed','cancelled') OR ended IS NULL OR
    ended<r.started_at OR ended>clock_timestamp()+interval '5 minutes' OR
    jsonb_typeof(evidence) IS DISTINCT FROM 'object' OR
    EXISTS(SELECT 1 FROM jsonb_object_keys(evidence) k WHERE k NOT IN ('source','external_run_key','source_revision','reason','flush_verified')) OR
    evidence->>'source' IS NULL OR evidence->>'source' NOT IN ('supervisor','github-actions') OR
    evidence->>'external_run_key' IS DISTINCT FROM r.external_run_key OR
    evidence->>'source_revision' IS DISTINCT FROM r.source_revision OR
    evidence->>'reason' IS NULL OR evidence->>'reason' NOT IN ('collection-finished','job-failed','job-cancelled','job-interrupted','flush-unproven') OR
    jsonb_typeof(evidence->'flush_verified') IS DISTINCT FROM 'boolean' THEN
  RAISE EXCEPTION 'Invalid collection lifecycle evidence';
 END IF;
 IF terminal_state='complete' AND (evidence->'flush_verified'<>'true'::jsonb OR
    evidence->>'reason'<>'collection-finished' OR NOT EXISTS(SELECT 1 FROM test262.run_metrics WHERE run_id=id AND stage='screening')) THEN
  RAISE EXCEPTION 'Complete collection requires flushed evidence and screening receipt';
 END IF;
 -- Lost response/repeated reconciliation preserves the first accepted timestamp/proof.
 IF r.state=terminal_state AND r.ended_at IS NOT NULL THEN RETURN to_jsonb(r); END IF;
 IF r.state NOT IN ('planned','running') THEN RAISE EXCEPTION 'Conflicting terminal collection state' USING ERRCODE='40001'; END IF;
 proof=evidence||jsonb_build_object('state',terminal_state);
 UPDATE test262.runs SET state=terminal_state,ended_at=ended,
  environment_diagnostics=environment_diagnostics||jsonb_build_object('collection_lifecycle',proof)
 WHERE run_id=id RETURNING * INTO result;
 INSERT INTO test262.audit_events(repository_id,actor_subject,action,entity_kind,details)
 VALUES(s.repository_id,session_user,'finish-run','runs',jsonb_build_object('run_id',id,'before_state',r.state,'state',terminal_state,'ended_at',ended,'evidence',proof));
 RETURN to_jsonb(result);
END $$;
REVOKE ALL ON FUNCTION test262.guard_run_lifecycle(),test262.api_finish_run(bigint,uuid,text,timestamptz,jsonb) FROM PUBLIC,anon,authenticated,service_role;
GRANT EXECUTE ON FUNCTION test262.api_finish_run(bigint,uuid,text,timestamptz,jsonb) TO test262_coordinator;
