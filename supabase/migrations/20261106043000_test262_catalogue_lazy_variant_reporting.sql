-- Defer required-variant lookups until an aggregate needs them or a detail row is selected.
SET LOCAL lock_timeout='5s';
SET LOCAL statement_timeout='60s';
CREATE OR REPLACE VIEW test262_reporting.v_current_fixture_status WITH (security_barrier=true) AS
WITH target_eligibility AS MATERIALIZED (
 SELECT e.provenance_id,e.fixture_id,e.eligibility,e.reason_codes
 FROM test262.provenance_fixture_eligibility e
 WHERE EXISTS(SELECT 1 FROM test262.reporting_targets t WHERE t.provenance_id=e.provenance_id)
), target_status AS MATERIALIZED (
 SELECT s.* FROM test262_reporting.v_variant_status s
 WHERE EXISTS(SELECT 1 FROM test262.reporting_targets t
              WHERE t.repository_id=s.repository_id AND t.provenance_id=s.provenance_id)
), recorded_counts AS (
 SELECT s.repository_id,s.provenance_id,s.fixture_id,
 count(*) AS recorded_variant_count,
 count(*) FILTER(WHERE s.outcome='pass') AS pass_variant_count,
 count(*) FILTER(WHERE s.outcome IN ('fail','unsupported')) AS failure_count,
 bool_or(s.conflict_flag) AS conflict_flag,max(s.last_received_at) AS last_received_at
 FROM target_status s JOIN test262.fixture_variants v
 ON v.fixture_id=s.fixture_id AND v.variant=s.variant AND v.required
 GROUP BY s.repository_id,s.provenance_id,s.fixture_id
)
SELECT t.repository_id,t.channel,t.evidence_kind,t.provenance_id,t.registration_snapshot_id,t.validation_id,
 f.corpus_id,f.fixture_id,f.upstream_path,f.feature_tags,e.eligibility,e.reason_codes,
 (SELECT count(*) FROM test262.fixture_variants rv WHERE rv.fixture_id=f.fixture_id AND rv.required) AS required_variant_count,
 coalesce(s.recorded_variant_count,0::bigint) AS recorded_variant_count,
 coalesce(s.pass_variant_count,0::bigint) AS pass_variant_count,
 coalesce(s.conflict_flag,false) AS conflict_flag,s.last_received_at,
 CASE WHEN f.is_support_file THEN 'excluded' WHEN f.metadata_state<>'valid' THEN 'metadata-error'
      WHEN e.eligibility IS NULL OR e.eligibility='unresolved' THEN 'incomplete'
      WHEN e.eligibility<>'runnable' THEN 'excluded' WHEN coalesce(s.conflict_flag,false) THEN 'conflicted'
      WHEN s.recorded_variant_count IS NULL THEN 'incomplete'
      WHEN s.recorded_variant_count<(SELECT count(*) FROM test262.fixture_variants rv WHERE rv.fixture_id=f.fixture_id AND rv.required) THEN 'incomplete'
      WHEN s.pass_variant_count=(SELECT count(*) FROM test262.fixture_variants rv WHERE rv.fixture_id=f.fixture_id AND rv.required) THEN 'complete-pass'
      WHEN s.failure_count>0 THEN 'fail' ELSE 'incomplete' END AS status,
 EXISTS(SELECT 1 FROM test262.registrations reg WHERE reg.snapshot_id=t.registration_snapshot_id
        AND reg.upstream_path=f.upstream_path) AS source_registered,
 t.updated_at AS target_updated_at,statement_timestamp() AS as_of
FROM test262.reporting_targets t JOIN test262.provenances p USING(provenance_id)
JOIN test262.fixtures f USING(corpus_id)
LEFT JOIN recorded_counts s ON s.repository_id=t.repository_id AND s.provenance_id=t.provenance_id
 AND s.fixture_id=f.fixture_id
LEFT JOIN target_eligibility e
 ON e.provenance_id=t.provenance_id AND e.fixture_id=f.fixture_id;

CREATE VIEW test262_reporting.v_current_required_variants WITH (security_barrier=true) AS
SELECT t.repository_id,t.channel,t.evidence_kind,t.provenance_id,f.fixture_id,
 (SELECT count(*) FROM test262.fixture_variants v WHERE v.fixture_id=f.fixture_id AND v.required) AS required_variant_count
FROM test262.reporting_targets t JOIN test262.provenances p USING(provenance_id)
JOIN test262.fixtures f USING(corpus_id);
REVOKE ALL ON test262_reporting.v_current_required_variants FROM PUBLIC,anon,authenticated,service_role;
GRANT SELECT ON test262_reporting.v_current_required_variants TO test262_reporter;
DO $grant$
BEGIN
 IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='test262_backup') THEN
  EXECUTE 'GRANT SELECT ON test262_reporting.v_current_required_variants TO test262_backup';
 END IF;
END $grant$;

