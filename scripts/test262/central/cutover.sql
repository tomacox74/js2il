-- Explicit operator CAS. Never auto-activate as part of migration/PR merge.
-- psql -v expected_epoch=1 -v new_state=active -f this-file
BEGIN;
SELECT pg_advisory_xact_lock(hashtextextended('test262-authority',0));
CREATE TEMP TABLE expected_cutover(epoch bigint, new_state text) ON COMMIT DROP;
INSERT INTO expected_cutover VALUES(:'expected_epoch',:'new_state');
DO $$
DECLARE e bigint; state text;
BEGIN
 SELECT epoch,new_state INTO e,state FROM expected_cutover;
 IF state NOT IN ('shadow','active') THEN RAISE EXCEPTION 'Invalid authority state'; END IF;
 IF NOT EXISTS(SELECT 1 FROM test262.schema_contract WHERE minimum_writer_epoch=e FOR UPDATE) THEN RAISE EXCEPTION 'Authority CAS failed'; END IF;
 UPDATE test262.schema_contract SET deployment_state=state,minimum_writer_epoch=minimum_writer_epoch+1;
 UPDATE test262.reconciliation_state SET authority_epoch=e+1,version=version+1;
 -- Reclaim will settle abandoned reservations conservatively; rollback does not refund running attempts.
 INSERT INTO test262.audit_events(repository_id,actor_subject,action,entity_kind,details)
 SELECT repository_id,session_user,'authority-transition','schema_contract',jsonb_build_object('old_epoch',e,'new_epoch',e+1,'state',state) FROM test262.repositories;
END $$;
COMMIT;
SELECT api_contract_version,minimum_writer_epoch,deployment_state FROM test262.schema_contract;
