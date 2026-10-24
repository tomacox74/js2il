-- Operator-only. Retain identities/evidence/outboxes; disable new and live sessions.
-- psql -v repository_id=<uuid> -v producer_id=<uuid> -v login_name=<login> -f revoke_worker.sql
BEGIN;
-- Fail before role changes if the supplied identities do not describe one binding.
SELECT 1 / count(*) FROM test262.api_subjects
 WHERE login_name=:'login_name' AND repository_id=:'repository_id' AND producer_id=:'producer_id';
UPDATE test262.api_subjects SET enabled=false
 WHERE login_name=:'login_name' AND repository_id=:'repository_id' AND producer_id=:'producer_id';
UPDATE test262.producers SET enabled=false
 WHERE repository_id=:'repository_id' AND producer_id=:'producer_id' AND credential_subject=:'login_name';
SELECT format('REVOKE test262_worker FROM %I',:'login_name') \gexec
SELECT format('ALTER ROLE %I NOLOGIN',:'login_name') \gexec
COMMIT;
