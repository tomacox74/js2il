-- Keep import coverage live, without rebuilding the observation join per import.
SET LOCAL lock_timeout='5s';
SET LOCAL statement_timeout='60s';

CREATE INDEX import_observation_entities ON test262.import_records(import_id,entity_id)
 WHERE entity_kind='observation';

CREATE OR REPLACE VIEW test262_reporting.v_catalogue_imports WITH (security_barrier=true) AS
WITH mapping_counts AS (
 SELECT i.repository_id,m.import_id,count(*) AS mapped_observations,
 count(o.observation_id) AS uploaded_observations
 FROM test262.import_records m JOIN test262.imports i USING(import_id)
 LEFT JOIN test262.observations o
 ON o.observation_id=m.entity_id AND o.repository_id=i.repository_id
 WHERE m.entity_kind='observation'
 GROUP BY i.repository_id,m.import_id
)
SELECT i.repository_id,i.import_id,i.source_schema,i.state AS import_state,i.started_at,
 coalesce((i.expected_counts->>'results')::bigint,(i.expected_counts->>'attempts')::bigint,0) AS expected_observations,
 coalesce(m.mapped_observations,0::bigint) AS mapped_observations,
 coalesce(m.uploaded_observations,0::bigint) AS uploaded_observations,
 coalesce((i.imported_counts->>'observations')::bigint,0) AS reported_observations,
 CASE WHEN i.imported_counts ? 'observations' AND coalesce(m.mapped_observations,0)=coalesce(m.uploaded_observations,0)
       AND coalesce(m.mapped_observations,0)=coalesce((i.expected_counts->>'results')::bigint,(i.expected_counts->>'attempts')::bigint,0)
      THEN 'Observation coverage complete' ELSE 'Partial or in progress' END AS coverage,
 coalesce((i.limitations->>'artifact_history_complete')::boolean,false) AS history_complete,
 (SELECT count(*) FROM test262.legacy_control_records l WHERE l.import_id=i.import_id AND l.source_table='mvp_import_checkpoint') AS completed_provenances,
 (SELECT l.document->>'uri' FROM test262.legacy_control_records l WHERE l.import_id=i.import_id AND l.source_table='artifact_source' ORDER BY l.source_key LIMIT 1) AS source_artifact,
 i.limitations->'missing_artifacts' AS history_limitations,
 statement_timestamp() AS as_of
FROM test262.imports i LEFT JOIN mapping_counts m
 ON m.repository_id=i.repository_id AND m.import_id=i.import_id;
