-- Operator-only. Dedicated LOGIN/secret created out of band; no credentials here.
-- psql -v repository_id=<existing-uuid> -v producer_id=<new-uuid>
--      -v login_name=<dedicated-login> -f enroll_worker.sql
-- Local and downstream execution clients both use the existing 'local' kind.
BEGIN;
-- The role deliberately has no test262_coordinator/test262_ingest membership.
INSERT INTO test262.producers(producer_id,repository_id,kind,display_name,credential_subject)
VALUES(:'producer_id',:'repository_id','local',:'login_name',:'login_name');
INSERT INTO test262.api_subjects(login_name,repository_id,producer_id,permission,trust_class)
VALUES(:'login_name',:'repository_id',:'producer_id','coordinator','trusted');
SELECT format('GRANT test262_worker TO %I',:'login_name') \gexec
COMMIT;
