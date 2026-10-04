-- Additive read-only reporting; no raw-table grants or changes to performance data.
CREATE VIEW test262_reporting.v_catalogue_summary WITH (security_barrier=true) AS
SELECT r.repository_id,r.canonical_name,
 (SELECT count(*) FROM test262.repository_corpora rc WHERE rc.repository_id=r.repository_id) AS corpora,
 (SELECT count(*) FROM test262.fixtures f JOIN test262.repository_corpora rc USING(corpus_id) WHERE rc.repository_id=r.repository_id) AS fixtures,
 (SELECT count(*) FROM test262.fixture_variants v JOIN test262.fixtures f USING(fixture_id) JOIN test262.repository_corpora rc USING(corpus_id) WHERE rc.repository_id=r.repository_id) AS variants,
 (SELECT count(*) FROM test262.observations o JOIN test262.runs x USING(run_id) WHERE o.repository_id=r.repository_id AND x.trust_class='legacy') AS historical_observations,
 (SELECT count(*) FROM test262.observations o JOIN test262.runs x USING(run_id) WHERE o.repository_id=r.repository_id AND x.trust_class='trusted') AS trusted_observations,
 (SELECT count(*) FROM test262.imports i WHERE i.repository_id=r.repository_id) AS snapshots,
 (SELECT max(received_at) FROM test262.observations o WHERE o.repository_id=r.repository_id) AS last_observation_at,
 c.deployment_state,c.minimum_writer_epoch,statement_timestamp() AS as_of
FROM test262.repositories r CROSS JOIN test262.schema_contract c;

CREATE VIEW test262_reporting.v_catalogue_imports WITH (security_barrier=true) AS
SELECT i.repository_id,i.import_id,i.source_schema,i.state AS import_state,i.started_at,
 coalesce((i.expected_counts->>'results')::bigint,(i.expected_counts->>'attempts')::bigint,0) AS expected_observations,
 m.mapped_observations,m.uploaded_observations,
 coalesce((i.imported_counts->>'observations')::bigint,0) AS reported_observations,
 CASE WHEN i.imported_counts ? 'observations' AND m.mapped_observations=m.uploaded_observations
       AND m.mapped_observations=coalesce((i.expected_counts->>'results')::bigint,(i.expected_counts->>'attempts')::bigint,0)
      THEN 'Observation coverage complete' ELSE 'Partial or in progress' END AS coverage,
 coalesce((i.limitations->>'artifact_history_complete')::boolean,false) AS history_complete,
 (SELECT count(*) FROM test262.legacy_control_records l WHERE l.import_id=i.import_id AND l.source_table='mvp_import_checkpoint') AS completed_provenances,
 (SELECT l.document->>'uri' FROM test262.legacy_control_records l WHERE l.import_id=i.import_id AND l.source_table='artifact_source' ORDER BY l.source_key LIMIT 1) AS source_artifact,
 i.limitations->'missing_artifacts' AS history_limitations,
 statement_timestamp() AS as_of
FROM test262.imports i CROSS JOIN LATERAL (
 SELECT count(*) AS mapped_observations,count(o.observation_id) AS uploaded_observations
 FROM test262.import_records m LEFT JOIN test262.observations o
 ON o.observation_id=m.entity_id AND o.repository_id=i.repository_id
 WHERE m.import_id=i.import_id AND m.entity_kind='observation'
) m;

CREATE VIEW test262_reporting.v_catalogue_outcomes WITH (security_barrier=true) AS
SELECT o.repository_id,o.provenance_id,p.evidence_kind,r.trust_class,o.outcome,
 count(*) AS observations,count(DISTINCT o.fixture_id) AS observed_fixtures,
 max(o.received_at) AS last_received_at
FROM test262.observations o JOIN test262.runs r USING(run_id)
JOIN test262.provenances p ON p.provenance_id=o.provenance_id
WHERE NOT EXISTS(SELECT 1 FROM test262.observation_invalidations x WHERE x.observation_id=o.observation_id)
GROUP BY o.repository_id,o.provenance_id,p.evidence_kind,r.trust_class,o.outcome;

REVOKE ALL ON test262_reporting.v_catalogue_summary,test262_reporting.v_catalogue_imports,
 test262_reporting.v_catalogue_outcomes FROM PUBLIC,anon,authenticated,service_role;
GRANT SELECT ON test262_reporting.v_catalogue_summary,test262_reporting.v_catalogue_imports,
 test262_reporting.v_catalogue_outcomes TO test262_reporter;
-- Disposable databases need not provision the production Grafana login.
DO $$ BEGIN
 IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='grafana_ro') THEN
  GRANT test262_reporter TO grafana_ro;
 END IF;
END $$;
