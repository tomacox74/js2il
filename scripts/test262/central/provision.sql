-- Operator template, run with psql variables. No passwords in source, logs or issue bodies.
-- psql -v repository_id=<uuid> -v provider_id=<GitHub immutable numeric ID>
--      -v canonical_name=tomacox74/js2il -v producer_id=<uuid> -v login_name=<dedicated LOGIN>
--      -v permission=coordinator -v trust_class=trusted -f this-file
-- Create the dedicated LOGIN and its secret in your secret manager first. Do not reuse postgres/grafana_ro.
BEGIN;
INSERT INTO test262.repositories(repository_id,provider,provider_repository_id,canonical_name)
VALUES(:'repository_id','github',:'provider_id',:'canonical_name') ON CONFLICT DO NOTHING;
INSERT INTO test262.producers(producer_id,repository_id,kind,display_name,credential_subject)
VALUES(:'producer_id',:'repository_id','actions',:'login_name',:'login_name') ON CONFLICT DO NOTHING;
INSERT INTO test262.api_subjects(login_name,repository_id,producer_id,permission,trust_class)
VALUES(:'login_name',:'repository_id',:'producer_id',:'permission',:'trust_class');
-- The subject row is the actual repository/trust boundary. Group membership grants only API calls.
SELECT format('GRANT %I TO %I',CASE WHEN :'permission'='coordinator' THEN 'test262_coordinator' ELSE 'test262_ingest' END,:'login_name') \gexec
COMMIT;
