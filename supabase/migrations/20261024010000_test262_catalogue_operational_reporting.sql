-- Operational telemetry and additive reporting only. No work/target/budget writes.
SET LOCAL lock_timeout='5s';
-- Preserve the existing report contract but aggregate required variants once.
-- The previous correlated aggregate rescanned evidence for every fixture.
CREATE OR REPLACE VIEW test262_reporting.v_current_fixture_status WITH (security_barrier=true) AS
WITH target_status AS MATERIALIZED (
 SELECT s.* FROM test262_reporting.v_variant_status s
 WHERE EXISTS(SELECT 1 FROM test262.reporting_targets t
              WHERE t.repository_id=s.repository_id AND t.provenance_id=s.provenance_id)
), counts AS (
 SELECT t.repository_id,t.channel,t.evidence_kind,t.provenance_id,t.registration_snapshot_id,t.validation_id,t.updated_at,
 f.corpus_id,f.fixture_id,f.upstream_path,f.feature_tags,f.is_support_file,f.metadata_state,
 count(v.variant) AS required_variant_count,count(s.variant) AS recorded_variant_count,
 count(*) FILTER(WHERE s.outcome='pass') AS pass_variant_count,
 count(*) FILTER(WHERE s.outcome IN ('fail','unsupported')) AS failure_count,
 coalesce(bool_or(s.conflict_flag),false) AS conflict_flag,max(s.last_received_at) AS last_received_at
 FROM test262.reporting_targets t JOIN test262.provenances p USING(provenance_id)
 JOIN test262.fixtures f USING(corpus_id)
 LEFT JOIN test262.fixture_variants v ON v.fixture_id=f.fixture_id AND v.required
 LEFT JOIN target_status s ON s.repository_id=t.repository_id AND s.provenance_id=t.provenance_id
                         AND s.fixture_id=f.fixture_id AND s.variant=v.variant
 GROUP BY t.repository_id,t.channel,t.evidence_kind,t.provenance_id,f.fixture_id
)
SELECT a.repository_id,a.channel,a.evidence_kind,a.provenance_id,a.registration_snapshot_id,a.validation_id,
 a.corpus_id,a.fixture_id,a.upstream_path,a.feature_tags,e.eligibility,e.reason_codes,
 a.required_variant_count,a.recorded_variant_count,a.pass_variant_count,a.conflict_flag,a.last_received_at,
 CASE WHEN a.is_support_file THEN 'excluded' WHEN a.metadata_state<>'valid' THEN 'metadata-error'
      WHEN e.eligibility IS NULL OR e.eligibility='unresolved' THEN 'incomplete'
      WHEN e.eligibility<>'runnable' THEN 'excluded' WHEN a.conflict_flag THEN 'conflicted'
      WHEN a.required_variant_count=0 OR a.recorded_variant_count<a.required_variant_count THEN 'incomplete'
      WHEN a.pass_variant_count=a.required_variant_count THEN 'complete-pass'
      WHEN a.failure_count>0 THEN 'fail' ELSE 'incomplete' END AS status,
 EXISTS(SELECT 1 FROM test262.registrations reg WHERE reg.snapshot_id=a.registration_snapshot_id
        AND reg.upstream_path=a.upstream_path) AS source_registered,
 a.updated_at AS target_updated_at,statement_timestamp() AS as_of
FROM counts a LEFT JOIN test262.provenance_fixture_eligibility e
 ON e.provenance_id=a.provenance_id AND e.fixture_id=a.fixture_id;

CREATE TABLE test262.monitor_samples (
 sample_id uuid PRIMARY KEY,
 repository_id uuid NOT NULL REFERENCES test262.repositories,
 producer_id uuid NOT NULL,
 observed_at timestamptz NOT NULL DEFAULT clock_timestamp(),
 document jsonb NOT NULL,
 FOREIGN KEY(repository_id,producer_id) REFERENCES test262.producers(repository_id,producer_id)
);
ALTER TABLE test262.monitor_samples ENABLE ROW LEVEL SECURITY;
REVOKE ALL ON test262.monitor_samples FROM PUBLIC,anon,authenticated,service_role;
CREATE INDEX monitor_samples_latest ON test262.monitor_samples(repository_id,observed_at DESC,sample_id);
CREATE TRIGGER immutable_monitor_sample BEFORE UPDATE OR DELETE ON test262.monitor_samples
 FOR EACH ROW EXECUTE FUNCTION test262.reject_evidence_mutation();

CREATE FUNCTION test262.api_monitor(epoch bigint,id uuid,d jsonb) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; old test262.monitor_samples; item record;
BEGIN
 s=test262.api_identity(epoch,true); PERFORM test262.api_lock(s.repository_id);
 IF s.trust_class<>'trusted' OR (SELECT deployment_state FROM test262.schema_contract)<>'active' THEN
  RAISE EXCEPTION 'Trusted active monitor required' USING ERRCODE='42501'; END IF;
 IF id IS NULL OR jsonb_typeof(d) IS DISTINCT FROM 'object' OR octet_length(d::text)>32768 OR
    d->>'master_revision' IS NULL OR d->>'master_revision' !~ '^[0-9a-f]{40}([0-9a-f]{24})?$' OR
    d->>'master_committed_at' IS NULL OR jsonb_typeof(d->'checks') IS DISTINCT FROM 'object' OR
    EXISTS(SELECT 1 FROM jsonb_object_keys(d) k WHERE k NOT IN ('master_revision','master_committed_at','checks')) THEN
  RAISE EXCEPTION 'Invalid monitor sample'; END IF;
 PERFORM (d->>'master_committed_at')::timestamptz;
 IF (d->>'master_committed_at')::timestamptz>clock_timestamp()+interval '5 minutes' THEN
  RAISE EXCEPTION 'Future master timestamp'; END IF;
 FOR item IN SELECT key,value FROM jsonb_each(d->'checks') LOOP
  IF item.key NOT IN ('backup','restore','linux-smoke','windows-smoke','benchmarkdotnet-suite') OR
     jsonb_typeof(item.value) IS DISTINCT FROM 'object' OR
     EXISTS(SELECT 1 FROM jsonb_object_keys(item.value) k WHERE k NOT IN
      ('fetch_state','latest_run_id','latest_attempt','latest_state','latest_conclusion','latest_finished_at',
       'success_run_id','success_attempt','success_finished_at','source_revision',
       'artifact_id','artifact_sha256','receipt_verified','retention_verified','retention_until','receipt_checked_at')) OR
     item.value->>'fetch_state' IS NULL OR item.value->>'fetch_state' NOT IN ('observed','unavailable') THEN
   RAISE EXCEPTION 'Invalid workflow health record'; END IF;
  IF (item.value->>'latest_state' IS NOT NULL AND item.value->>'latest_state' NOT IN
       ('queued','in_progress','completed','waiting','pending','requested')) OR
     (item.value->>'latest_conclusion' IS NOT NULL AND item.value->>'latest_conclusion' NOT IN
       ('success','failure','cancelled','timed_out','action_required','neutral','skipped','stale','startup_failure')) OR
     (item.value->>'source_revision' IS NOT NULL AND item.value->>'source_revision' !~ '^[0-9a-f]{40}([0-9a-f]{24})?$') OR
     (item.value->>'artifact_sha256' IS NOT NULL AND item.value->>'artifact_sha256' !~ '^[0-9a-f]{64}$') THEN
   RAISE EXCEPTION 'Invalid workflow metadata'; END IF;
  -- Validate all values consumed by the reporting casts before accepting telemetry.
  PERFORM (item.value->>'latest_run_id')::bigint,(item.value->>'latest_attempt')::integer,
          (item.value->>'success_run_id')::bigint,(item.value->>'success_attempt')::integer,
          (item.value->>'artifact_id')::bigint,(item.value->>'latest_finished_at')::timestamptz,
          (item.value->>'success_finished_at')::timestamptz,(item.value->>'retention_until')::timestamptz,
          (item.value->>'receipt_checked_at')::timestamptz,(item.value->>'receipt_verified')::boolean,
          (item.value->>'retention_verified')::boolean;
 END LOOP;
 SELECT * INTO old FROM test262.monitor_samples WHERE sample_id=id;
 IF old.sample_id IS NOT NULL THEN
  IF old.repository_id<>s.repository_id OR old.producer_id<>s.producer_id OR old.document<>d THEN
   RAISE EXCEPTION 'Conflicting monitor replay'; END IF;
  RETURN jsonb_build_object('sample_id',id,'observed_at',old.observed_at,'replayed',true);
 END IF;
 INSERT INTO test262.monitor_samples(sample_id,repository_id,producer_id,document)
  VALUES(id,s.repository_id,s.producer_id,d) RETURNING * INTO old;
 RETURN jsonb_build_object('sample_id',id,'observed_at',old.observed_at,'replayed',false);
END $$;
REVOKE ALL ON FUNCTION test262.api_monitor(bigint,uuid,jsonb) FROM PUBLIC,anon,authenticated,service_role;
GRANT EXECUTE ON FUNCTION test262.api_monitor(bigint,uuid,jsonb) TO test262_coordinator;

CREATE VIEW test262_reporting.v_monitor_latest WITH (security_barrier=true) AS
SELECT r.repository_id,s.sample_id,s.observed_at,
 extract(epoch FROM statement_timestamp()-s.observed_at) AS sample_age_seconds,
 s.document->>'master_revision' AS master_revision,
 (s.document->>'master_committed_at')::timestamptz AS master_committed_at,
 s.document->'checks' AS checks,statement_timestamp() AS as_of
FROM test262.repositories r LEFT JOIN LATERAL (
 SELECT * FROM test262.monitor_samples x WHERE x.repository_id=r.repository_id
 ORDER BY observed_at DESC,sample_id DESC LIMIT 1
) s ON true;

CREATE VIEW test262_reporting.v_operational_runs WITH (security_barrier=true) AS
SELECT r.repository_id,r.run_id,r.producer_id,r.run_kind,r.source_revision,r.provenance_id,
 r.state,r.started_at,r.ended_at,r.environment_diagnostics->'collection_lifecycle'->>'reason' AS collection_reason,
 extract(epoch FROM coalesce(r.ended_at,statement_timestamp())-r.started_at) AS duration_seconds,
 r.state='running' AND r.started_at<statement_timestamp()-interval '2 hours' AS overdue_collection,
 statement_timestamp() AS as_of FROM test262.runs r
WHERE r.trust_class='trusted' AND r.run_kind IN ('native','mvp');

CREATE VIEW test262_reporting.v_budget_health WITH (security_barrier=true) AS
SELECT b.repository_id,b.budget_scope_id,b.external_key,b.scope_kind,b.state,b.attempt_limit,b.time_limit_ms,
 b.charged_attempts,b.charged_ms,b.reserved_attempts,b.reserved_ms,b.claim_cap_ms,b.ended_at,
 b.lifecycle->>'reason' AS lifecycle_reason,
 coalesce(a.attempts,0) AS ledger_reserved_attempts,coalesce(a.ms,0) AS ledger_reserved_ms,
 b.reserved_attempts<>coalesce(a.attempts,0) OR b.reserved_ms<>coalesce(a.ms,0) AS reservation_mismatch,
 coalesce(a.overdue,0) AS overdue_reservations,statement_timestamp() AS as_of
FROM test262.budget_scopes b LEFT JOIN LATERAL (
 SELECT sum(br.reserved_attempts) AS attempts,sum(br.reserved_ms) AS ms,
 count(*) FILTER(WHERE l.expires_at<statement_timestamp()-interval '15 minutes') AS overdue
 FROM test262.budget_reservations br JOIN test262.work_leases l USING(work_item_id,lease_generation)
 WHERE br.budget_scope_id=b.budget_scope_id AND br.state='reserved'
) a ON true;

CREATE VIEW test262_reporting.v_publication_health WITH (security_barrier=true) AS
SELECT p.repository_id,p.publication_id,p.pr_number,p.state,p.closure_reason,p.expected_head,
 b.provenance_id,b.created_at AS batch_created_at,
 extract(epoch FROM statement_timestamp()-b.created_at) AS batch_age_seconds,
 p.state IN ('blocked','ownership-lost') OR
 (p.state IN ('reserved','updating') AND b.created_at<statement_timestamp()-interval '24 hours') AS needs_attention,
 statement_timestamp() AS as_of
FROM test262.publications p JOIN test262.native_batches b USING(batch_id);

CREATE VIEW test262_reporting.v_target_health WITH (security_barrier=true) AS
SELECT t.repository_id,t.channel,t.evidence_kind,t.provenance_id,t.updated_at,
 s.source_revision AS registration_revision,s.state AS registration_state,
 v.target_revision AS validation_revision,v.conclusion AS validation_conclusion,
 c.revision AS upstream_pin,encode(p.identity_sha256,'hex') AS execution_identity_sha256,
 p.identity_document->'binaries' AS binaries,p.identity_document->'harness' AS harness,
 p.identity_document->'environment_identity' AS environment_identity,
 m.master_revision AS observed_master_revision,m.observed_at AS master_observed_at,
 CASE WHEN m.sample_age_seconds IS NULL OR m.sample_age_seconds>7200 THEN NULL
      ELSE s.source_revision=m.master_revision END AS matches_observed_master,
 s.state='sealed' AND (t.evidence_kind<>'native' OR
   coalesce(v.conclusion='success' AND v.target_revision=s.source_revision,false)) AS registration_validation_coherent,
 (SELECT max(o.received_at) FROM test262.observations o JOIN test262.runs r USING(run_id)
   WHERE o.provenance_id=t.provenance_id AND r.trust_class='trusted' AND
   NOT EXISTS(SELECT 1 FROM test262.observation_invalidations i WHERE i.observation_id=o.observation_id)) AS last_trusted_evidence_at,
 statement_timestamp() AS as_of
FROM test262.reporting_targets t JOIN test262.provenances p USING(provenance_id)
 JOIN test262.corpora c USING(corpus_id) JOIN test262.registration_snapshots s ON s.snapshot_id=t.registration_snapshot_id
 LEFT JOIN test262.validation_events v USING(validation_id)
 LEFT JOIN test262_reporting.v_monitor_latest m ON m.repository_id=t.repository_id;

CREATE VIEW test262_reporting.v_ingestion_latency_7d WITH (security_barrier=true) AS
SELECT o.repository_id,r.producer_id,count(*) AS observations,
 percentile_cont(0.95) WITHIN GROUP(ORDER BY extract(epoch FROM o.received_at-o.finished_at))
   FILTER(WHERE o.received_at>=o.finished_at) AS p95_delivery_seconds,
 count(*) FILTER(WHERE o.received_at<o.finished_at) AS clock_skew_observations,
 max(o.received_at) AS last_received_at,statement_timestamp()-interval '7 days' AS window_start,
 statement_timestamp() AS as_of
FROM test262.observations o JOIN test262.runs r USING(run_id)
WHERE r.trust_class='trusted' AND o.source_kind='live' AND o.received_at>=statement_timestamp()-interval '7 days'
GROUP BY o.repository_id,r.producer_id;

-- A new immutable observation UUID is execution evidence; replayed request IDs
-- do not add observations. Keep unclassified repeats separate from the numerator.
CREATE VIEW test262_reporting.v_execution_classification WITH (security_barrier=true) AS
WITH prior AS (
 SELECT o.*,w.retry_generation,w.selection_reason,
 row_number() OVER scope AS execution_ordinal,
 min(o.finished_at) FILTER(WHERE o.outcome='pass') OVER previous AS prior_pass_finished_at,
 max(o.finished_at) OVER previous AS previous_finished_at
 FROM test262.observations o JOIN test262.runs r USING(run_id)
 JOIN test262.work_items w USING(work_item_id)
 WHERE r.trust_class='trusted' AND o.source_kind='live' AND o.lease_generation IS NOT NULL AND o.phase<>'planning'
 AND NOT EXISTS(SELECT 1 FROM test262.observation_invalidations i WHERE i.observation_id=o.observation_id)
 WINDOW scope AS (PARTITION BY o.repository_id,o.provenance_id,o.fixture_id,o.variant ORDER BY o.started_at,o.observation_id),
 previous AS (scope ROWS BETWEEN UNBOUNDED PRECEDING AND 1 PRECEDING)
)
SELECT o.repository_id,o.provenance_id,o.fixture_id,o.variant,o.observation_id,o.run_id,o.work_item_id,
 o.lease_generation,o.started_at,o.finished_at,o.received_at,
 CASE WHEN (o.retry_generation>0 AND o.selection_reason->>'deliberate_retry'='true') OR EXISTS(SELECT 1 FROM test262.retry_decisions d
       WHERE d.work_item_id=o.work_item_id AND d.decision IN ('reopen','backoff') AND d.created_at<=o.started_at
         AND d.created_at>=coalesce(o.previous_finished_at,'-infinity'::timestamptz))
      THEN 'intentional-retry'
 WHEN EXISTS(SELECT 1 FROM test262.work_leases l WHERE l.work_item_id=o.work_item_id
      AND l.lease_generation<o.lease_generation AND l.release_reason='expired') THEN 'lease-recovery'
 WHEN o.execution_ordinal=1 THEN 'baseline'
 WHEN o.prior_pass_finished_at<=o.started_at THEN 'avoidable-repeat'
 ELSE 'unclassified-repeat' END AS execution_class
FROM prior o;

CREATE VIEW test262_reporting.v_duplicate_executions_7d WITH (security_barrier=true) AS
SELECT r.repository_id,count(e.observation_id) AS execution_observations,
 count(*) FILTER(WHERE e.execution_class='baseline') AS baseline_executions,
 count(*) FILTER(WHERE e.execution_class='avoidable-repeat') AS avoidable_repeats,
 count(*) FILTER(WHERE e.execution_class='intentional-retry') AS intentional_retries,
 count(*) FILTER(WHERE e.execution_class='lease-recovery') AS lease_recoveries,
 count(*) FILTER(WHERE e.execution_class='unclassified-repeat') AS unclassified_repeats,
 100.0*count(*) FILTER(WHERE e.execution_class='avoidable-repeat') /
 nullif(count(*) FILTER(WHERE e.execution_class IN ('baseline','avoidable-repeat')),0) AS classified_duplicate_percent,
 (SELECT count(*) FROM test262.observations o JOIN test262.runs x USING(run_id)
  WHERE o.repository_id=r.repository_id AND x.trust_class='trusted' AND o.source_kind='live' AND o.phase<>'planning'
    AND o.started_at>=statement_timestamp()-interval '7 days' AND (o.work_item_id IS NULL OR o.lease_generation IS NULL)
    AND NOT EXISTS(SELECT 1 FROM test262.observation_invalidations i WHERE i.observation_id=o.observation_id)) AS uninstrumented_observations,
 count(e.observation_id)>0 AND count(*) FILTER(WHERE e.execution_class='unclassified-repeat')=0 AND
 NOT EXISTS(SELECT 1 FROM test262.observations o JOIN test262.runs x USING(run_id)
  WHERE o.repository_id=r.repository_id AND x.trust_class='trusted' AND o.source_kind='live' AND o.phase<>'planning'
    AND o.started_at>=statement_timestamp()-interval '7 days' AND (o.work_item_id IS NULL OR o.lease_generation IS NULL)
    AND NOT EXISTS(SELECT 1 FROM test262.observation_invalidations i WHERE i.observation_id=o.observation_id)) AS classification_complete,
 statement_timestamp()-interval '7 days' AS window_start,statement_timestamp() AS as_of
FROM test262.repositories r LEFT JOIN test262_reporting.v_execution_classification e
 ON e.repository_id=r.repository_id AND e.started_at>=statement_timestamp()-interval '7 days'
GROUP BY r.repository_id;

CREATE VIEW test262_reporting.v_workflow_health WITH (security_barrier=true) AS
SELECT m.repository_id,m.sample_id,m.observed_at,m.sample_age_seconds,k AS workflow,
 m.checks->k->>'fetch_state' AS fetch_state,
 m.checks->k->>'latest_conclusion' AS latest_conclusion,m.checks->k->>'latest_state' AS latest_state,
 (m.checks->k->>'latest_run_id')::bigint AS latest_run_id,
 (m.checks->k->>'success_run_id')::bigint AS success_run_id,
 (m.checks->k->>'success_finished_at')::timestamptz AS success_finished_at,
 (m.checks->k->>'receipt_verified')::boolean AS receipt_verified,
 (m.checks->k->>'retention_verified')::boolean AS retention_verified,
 (m.checks->k->>'retention_until')::timestamptz AS retention_until,
 (m.checks->k->>'artifact_id')::bigint AS artifact_id,
 m.checks->k->>'artifact_sha256' AS artifact_sha256,
 CASE WHEN k='backup' THEN 'daily' WHEN k='restore' THEN 'weekly' ELSE 'release-or-manual' END AS expected_cadence,
 statement_timestamp() AS as_of
FROM test262_reporting.v_monitor_latest m CROSS JOIN
 unnest(ARRAY['backup','restore','linux-smoke','windows-smoke','benchmarkdotnet-suite']) k;

CREATE VIEW test262_reporting.v_operational_signals WITH (security_barrier=true) AS
WITH signal_values AS (
 SELECT repository_id,'monitor-age'::text AS signal,sample_age_seconds AS actual,7200::numeric AS threshold,
 CASE WHEN sample_id IS NULL THEN NULL ELSE (sample_age_seconds>7200)::int END AS breach,
 'Hourly monitor receipt age; missing telemetry is unknown'::text AS detail FROM test262_reporting.v_monitor_latest
 UNION ALL
 SELECT repository_id,workflow||'-health',extract(epoch FROM statement_timestamp()-success_finished_at),
 CASE WHEN workflow='backup' THEN 129600 ELSE 691200 END,
 CASE WHEN sample_age_seconds IS NULL OR sample_age_seconds>7200 OR fetch_state IS DISTINCT FROM 'observed' THEN NULL
 WHEN latest_conclusion IN ('failure','timed_out','cancelled','action_required') THEN 1
 WHEN success_finished_at IS NULL OR receipt_verified IS DISTINCT FROM true THEN NULL
 ELSE (success_finished_at<statement_timestamp()-CASE WHEN workflow='backup' THEN interval '36 hours' ELSE interval '8 days' END
       OR retention_verified IS DISTINCT FROM true OR retention_until<=statement_timestamp())::int END,
 'Digest-verified workflow receipt, not a new S3 read; missing proof is unknown'
 FROM test262_reporting.v_workflow_health WHERE workflow IN ('backup','restore')
 UNION ALL
 SELECT repository_id,'duplicate-execution-percent',classified_duplicate_percent,5,
 CASE WHEN NOT classification_complete OR classified_duplicate_percent IS NULL THEN NULL
 ELSE (classified_duplicate_percent>=5)::int END,'Excludes deliberate retries and lease recovery; unclassified repeats prevent a passing claim'
 FROM test262_reporting.v_duplicate_executions_7d
 UNION ALL
 SELECT r.repository_id,'reservation-health',coalesce(sum(b.overdue_reservations),0)+count(*) FILTER(WHERE b.reservation_mismatch),0,
 (coalesce(sum(b.overdue_reservations),0)+count(*) FILTER(WHERE b.reservation_mismatch)>0)::int,
 'Reservation ledger mismatch or lease expiry overdue by 15 minutes'
 FROM test262.repositories r LEFT JOIN test262_reporting.v_budget_health b USING(repository_id) GROUP BY r.repository_id
 UNION ALL
 SELECT r.repository_id,'collection-lifecycle',count(*) FILTER(WHERE x.overdue_collection),0,
 (count(*) FILTER(WHERE x.overdue_collection)>0)::int,'Trusted collection still running after two hours'
 FROM test262.repositories r LEFT JOIN test262_reporting.v_operational_runs x USING(repository_id) GROUP BY r.repository_id
 UNION ALL
 SELECT r.repository_id,'publication-backlog',count(*) FILTER(WHERE p.needs_attention),0,
 (count(*) FILTER(WHERE p.needs_attention)>0)::int,'Blocked/ownership-lost publication or reservation/update older than 24 hours'
 FROM test262.repositories r LEFT JOIN test262_reporting.v_publication_health p USING(repository_id) GROUP BY r.repository_id
 UNION ALL
 SELECT r.repository_id,'native-target-freshness',NULL,3600,
 CASE WHEN m.sample_age_seconds IS NULL OR m.sample_age_seconds>7200 OR t.provenance_id IS NULL THEN NULL
 WHEN NOT t.registration_validation_coherent THEN 1
 WHEN t.matches_observed_master THEN 0
 WHEN m.master_committed_at>statement_timestamp()-interval '1 hour' THEN 0 ELSE 1 END,
 'Native registration/validation must match observed master; one-hour validation grace after push'
 FROM test262.repositories r LEFT JOIN test262_reporting.v_target_health t ON t.repository_id=r.repository_id AND t.evidence_kind='native' AND t.channel='master'
 LEFT JOIN test262_reporting.v_monitor_latest m ON m.repository_id=r.repository_id
 UNION ALL
 SELECT r.repository_id,'ingestion-p95-seconds',max(i.p95_delivery_seconds),300,
 CASE WHEN count(i.observations)=0 OR sum(i.clock_skew_observations)>0 THEN NULL
 ELSE (max(i.p95_delivery_seconds)>300)::int END,'Worst producer p95, seven days; clock skew or no samples is unknown'
 FROM test262.repositories r LEFT JOIN test262_reporting.v_ingestion_latency_7d i USING(repository_id) GROUP BY r.repository_id
)
SELECT *,CASE WHEN breach IS NULL THEN 'unknown' WHEN breach=1 THEN 'warning' ELSE 'ok' END AS health_state,
 statement_timestamp() AS as_of FROM signal_values;

REVOKE ALL ON test262_reporting.v_monitor_latest,test262_reporting.v_operational_runs,
 test262_reporting.v_budget_health,test262_reporting.v_publication_health,test262_reporting.v_target_health,
 test262_reporting.v_ingestion_latency_7d,test262_reporting.v_execution_classification,
 test262_reporting.v_duplicate_executions_7d,test262_reporting.v_workflow_health,
 test262_reporting.v_operational_signals FROM PUBLIC,anon,authenticated,service_role;
GRANT SELECT ON test262_reporting.v_monitor_latest,test262_reporting.v_operational_runs,
 test262_reporting.v_budget_health,test262_reporting.v_publication_health,test262_reporting.v_target_health,
 test262_reporting.v_ingestion_latency_7d,test262_reporting.v_execution_classification,
 test262_reporting.v_duplicate_executions_7d,test262_reporting.v_workflow_health,
 test262_reporting.v_operational_signals TO test262_reporter;
DO $$ BEGIN
 IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='test262_backup') THEN
  GRANT SELECT ON test262.monitor_samples TO test262_backup;
 END IF;
END $$;
