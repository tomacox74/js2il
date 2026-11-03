-- Add bounded worker slots while retaining the single-worker context and proof contracts.
SET LOCAL lock_timeout = '5s';

CREATE OR REPLACE FUNCTION test262.api_register_collection(epoch bigint, plan jsonb) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE subject test262.api_subjects; owner test262.runs; budget test262.budget_scopes;
        existing test262.collection_attempts; ctx jsonb; work jsonb; rec jsonb;
        producers uuid[]; digest bytea; cursor_after bigint; chunk jsonb;
BEGIN
  subject := test262.api_identity(epoch, true);
  PERFORM test262.api_lock(subject.repository_id);
  ctx := plan->'context';
  work := plan->'work';
  rec := plan->'reconciliation';
  IF ctx ? 'worker_producers' THEN
    SELECT array_agg(value::uuid ORDER BY slot) INTO producers
      FROM jsonb_array_elements_text(ctx->'worker_producers') WITH ORDINALITY AS workers(value,slot);
  ELSE
    producers := ARRAY[(ctx->>'worker_producer')::uuid];
  END IF;
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
      OR cardinality(producers) IS NULL OR cardinality(producers) NOT BETWEEN 1 AND 4
      OR cardinality(producers) <> (SELECT count(DISTINCT producer) FROM unnest(producers) AS workers(producer))
      OR subject.producer_id=ANY(producers)
      OR EXISTS (
        SELECT 1 FROM unnest(producers) AS workers(producer)
        WHERE NOT EXISTS (
          SELECT 1 FROM test262.producers p JOIN test262.api_subjects s USING (repository_id,producer_id)
          WHERE p.repository_id=subject.repository_id AND p.producer_id=workers.producer AND p.enabled
            AND s.enabled AND s.trust_class='trusted' AND s.permission='coordinator'
            AND pg_has_role(s.login_name,'test262_worker','MEMBER')
            AND NOT pg_has_role(s.login_name,'test262_coordinator','MEMBER'))) THEN
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
    SELECT owner.repository_id,owner.run_id,slot,producer
    FROM unnest(producers) WITH ORDINALITY AS workers(producer,slot);
  RETURN test262.api_collection(epoch,owner.run_id);
END $$;

CREATE OR REPLACE FUNCTION test262.api_finish_collection(epoch bigint, id uuid, proof jsonb) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE subject test262.api_subjects; collection test262.collection_attempts;
        member test262.collection_workers; owner test262.runs; child test262.runs;
        terminal_state text; reason text; finished jsonb; verified boolean := true;
        worker_proofs jsonb; evidence jsonb; worker_results jsonb := '[]';
        worker_verified boolean; worker_state text; worker_reason text;
        any_failed boolean := false; any_cancelled boolean := false;
BEGIN
  subject := test262.api_identity(epoch,true);
  PERFORM test262.api_lock(subject.repository_id);
  SELECT * INTO collection FROM test262.collection_attempts WHERE run_id=id
    AND repository_id=subject.repository_id AND authority_epoch=epoch;
  SELECT * INTO owner FROM test262.runs WHERE run_id=id AND producer_id=subject.producer_id
    AND repository_id=subject.repository_id AND trust_class='trusted';
  worker_proofs := CASE WHEN collection.context ? 'worker_producers' THEN proof->'workers'
                        ELSE jsonb_build_array(proof || '{"slot":1}'::jsonb) END;
  IF collection.run_id IS NULL OR owner.run_id IS NULL OR subject.trust_class<>'trusted'
      OR (SELECT deployment_state FROM test262.schema_contract)<>'active'
      OR jsonb_typeof(worker_proofs) IS DISTINCT FROM 'array' THEN
    RAISE EXCEPTION 'Invalid managed completion proof' USING ERRCODE='42501';
  END IF;
  IF jsonb_array_length(worker_proofs) <> (SELECT count(*) FROM test262.collection_workers WHERE collection_run_id=id)
      OR jsonb_array_length(worker_proofs) <>
           (SELECT count(DISTINCT p->>'job_id') FROM jsonb_array_elements(worker_proofs) p)
      OR EXISTS (
        SELECT 1 FROM test262.collection_workers w WHERE w.collection_run_id=id AND NOT EXISTS (
          SELECT 1 FROM jsonb_array_elements(worker_proofs) p
          WHERE p->>'slot'=w.slot::text AND p->>'job_id' ~ '^[1-9][0-9]*$'
            AND (w.actions_job_id IS NULL OR w.actions_job_id=(p->>'job_id')::bigint)
            AND p->>'state' IN ('success','failure','cancelled','incomplete','skipped'))) THEN
    RAISE EXCEPTION 'Invalid managed worker completion proof' USING ERRCODE='42501';
  END IF;
  UPDATE test262.collection_attempts SET phase='closing' WHERE run_id=id;
  IF owner.state IN ('complete','incomplete','failed','cancelled') THEN
    RETURN jsonb_build_object('run_id',id,'state',owner.state,'ended_at',owner.ended_at,
      'budget',test262.api_finish_budget(epoch,collection.budget_scope_id,true));
  END IF;

  FOR member IN SELECT * FROM test262.collection_workers WHERE collection_run_id=id ORDER BY slot LOOP
    SELECT p INTO evidence FROM jsonb_array_elements(worker_proofs) p WHERE p->>'slot'=member.slot::text;
    SELECT * INTO child FROM test262.runs WHERE run_id=member.run_id AND producer_id=member.producer_id;
    worker_verified := coalesce(evidence->>'state'='success' AND evidence->'retained'='true'::jsonb
      AND child.state='complete' AND child.environment_diagnostics->'collection_lifecycle'->'flush_verified'='true'::jsonb
      AND EXISTS (SELECT 1 FROM test262.run_metrics WHERE run_id=child.run_id AND stage='screening')
      AND NOT EXISTS (SELECT 1 FROM test262.observations WHERE run_id=child.run_id
                      AND outcome IN ('infrastructure-error','incomplete')),false);
    verified := verified AND worker_verified;
    any_failed := any_failed OR evidence->>'state'='failure';
    any_cancelled := any_cancelled OR evidence->>'state'='cancelled';
    worker_state := CASE WHEN worker_verified THEN 'complete' WHEN evidence->>'state'='cancelled' THEN 'cancelled'
      WHEN evidence->>'state'='failure' THEN 'failed' ELSE 'incomplete' END;
    worker_results := worker_results || jsonb_build_array(evidence || jsonb_build_object(
      'run_id',member.run_id,'producer_id',member.producer_id,'reconciled_state',worker_state));
    IF child.state='running' THEN
      worker_reason := CASE worker_state WHEN 'cancelled' THEN 'job-cancelled'
        WHEN 'failed' THEN 'job-failed' ELSE 'flush-unproven' END;
      UPDATE test262.runs SET state=worker_state, ended_at=clock_timestamp(),
        environment_diagnostics=environment_diagnostics || jsonb_build_object('collection_lifecycle',
          jsonb_build_object('source','github-actions','state',worker_state,'external_run_key',child.external_run_key,
            'source_revision',child.source_revision,'reason',worker_reason,'flush_verified',false))
        WHERE run_id=child.run_id;
      INSERT INTO test262.audit_events(repository_id,actor_subject,action,entity_kind,details)
        VALUES(owner.repository_id,session_user,'finish-collection-worker','runs',
          jsonb_build_object('run_id',child.run_id,'parent_run_id',id,'state',worker_state,'job_id',member.actions_job_id));
    END IF;
  END LOOP;
  verified := verified AND NOT EXISTS (SELECT 1 FROM test262.budget_reservations
    WHERE budget_scope_id=collection.budget_scope_id AND state IN ('reserved','uncertain'));
  terminal_state := CASE WHEN verified THEN 'complete' WHEN any_cancelled THEN 'cancelled'
    WHEN any_failed THEN 'failed' ELSE 'incomplete' END;
  reason := CASE terminal_state WHEN 'complete' THEN 'collection-finished' WHEN 'cancelled' THEN 'job-cancelled'
    WHEN 'failed' THEN 'job-failed' ELSE 'flush-unproven' END;
  IF verified THEN
    INSERT INTO test262.run_metrics(repository_id,run_id,stage,duration_ms,attempt_count,timeout_count,measurement_version)
      SELECT owner.repository_id,id,'screening',
        greatest(0,(extract(epoch FROM clock_timestamp()-owner.started_at)*1000)::bigint),
        sum(m.attempt_count),sum(m.timeout_count),1
      FROM test262.run_metrics m JOIN test262.collection_workers w ON w.run_id=m.run_id
      WHERE w.collection_run_id=id AND m.stage='screening';
  END IF;
  finished := test262.api_finish_run(epoch,id,terminal_state,clock_timestamp(),
    jsonb_build_object('source','github-actions','external_run_key',owner.external_run_key,
      'source_revision',owner.source_revision,'reason',reason,'flush_verified',verified));
  RETURN jsonb_build_object('run_id',id,'state',finished->>'state','ended_at',finished->>'ended_at','workers',worker_results,
    'budget',test262.api_finish_budget(epoch,collection.budget_scope_id,true));
END $$;

CREATE OR REPLACE FUNCTION test262.api_contract() RETURNS jsonb
LANGUAGE sql SECURITY DEFINER SET search_path=pg_catalog AS $function$
 SELECT (to_jsonb(c)-'singleton')||jsonb_build_object(
  'repository_id',s.repository_id,'producer_id',s.producer_id,
  'trust_class',s.trust_class,'permission',s.permission,'bulk_export_version',1,'managed_actions_collection_version',1,
  'managed_actions_collection_slots',4)
 FROM test262.schema_contract c CROSS JOIN test262.api_subjects s
 WHERE s.login_name=session_user AND s.enabled;
$function$;
