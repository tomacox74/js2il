-- Single-pass, login-bound bulk reads. Existing read/write APIs stay unchanged.
SET LOCAL lock_timeout='5s';
CREATE FUNCTION test262.api_export_open(epoch bigint, t text) RETURNS refcursor
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog
SET cursor_tuple_fraction='1.0' AS $function$
DECLARE s test262.api_subjects; rel regclass; predicate text; portal refcursor;
BEGIN
 s=test262.api_identity(epoch,true);
 IF s.trust_class<>'trusted' THEN
  RAISE EXCEPTION 'Trusted coordinator required' USING ERRCODE='42501';
 END IF;
 IF current_setting('transaction_read_only')<>'on' OR
    current_setting('transaction_isolation') NOT IN ('repeatable read','serializable') THEN
  RAISE EXCEPTION 'Bulk export requires a read-only repeatable snapshot' USING ERRCODE='25000';
 END IF;
 IF t IS NULL OR NOT t=ANY(ARRAY['corpora','repository_corpora','fixtures','fixture_specification_metadata','fixture_variants','fixture_dependencies','provenances','provenance_fixture_eligibility','runs','observations','observation_payloads','observation_invalidations','validation_events','work_items','work_leases','budget_reservations','budget_scopes','budget_work_items','registration_snapshots','registrations','reporting_targets','native_batches','batch_fixtures','batch_evidence','publications','publication_checks','imports','import_records','legacy_control_records','reconciliation_state','reconciliations','run_metrics']) THEN
  RAISE EXCEPTION 'Unsupported record type';
 END IF;
 rel=format('test262.%I',t)::regclass;
 IF EXISTS(SELECT 1 FROM pg_attribute WHERE attrelid=rel AND attname='repository_id' AND attnum>0 AND NOT attisdropped) THEN
  predicate='x.repository_id=$1';
 ELSE
  CASE t
   WHEN 'corpora','fixtures' THEN
    predicate='EXISTS (SELECT 1 FROM test262.repository_corpora rc WHERE rc.repository_id=$1 AND rc.corpus_id=x.corpus_id)';
   WHEN 'fixture_variants','fixture_dependencies','fixture_specification_metadata' THEN
    predicate='EXISTS (SELECT 1 FROM test262.fixtures f JOIN test262.repository_corpora rc USING(corpus_id) WHERE rc.repository_id=$1 AND f.fixture_id=x.fixture_id)';
   WHEN 'registrations' THEN
    predicate='EXISTS (SELECT 1 FROM test262.registration_snapshots p WHERE p.repository_id=$1 AND p.snapshot_id=x.snapshot_id)';
   WHEN 'observation_payloads','observation_invalidations' THEN
    predicate='EXISTS (SELECT 1 FROM test262.observations p WHERE p.repository_id=$1 AND p.observation_id=x.observation_id)';
   WHEN 'batch_fixtures','batch_evidence' THEN
    predicate='EXISTS (SELECT 1 FROM test262.native_batches p WHERE p.repository_id=$1 AND p.batch_id=x.batch_id)';
   WHEN 'publication_checks' THEN
    predicate='EXISTS (SELECT 1 FROM test262.publications p WHERE p.repository_id=$1 AND p.publication_id=x.publication_id)';
   WHEN 'import_records','legacy_control_records' THEN
    predicate='EXISTS (SELECT 1 FROM test262.imports p WHERE p.repository_id=$1 AND p.import_id=x.import_id)';
   ELSE RAISE EXCEPTION 'Unsupported ownership relation';
  END CASE;
 END IF;
 portal='test262_export_'||replace(gen_random_uuid()::text,'-','');
 -- No sort, LIMIT, OFFSET or per-page JSON aggregation: consume one scan.
 -- A cursor retains scan position; row identities, not line order, join evidence.
 OPEN portal NO SCROLL FOR EXECUTE format('SELECT to_jsonb(x) FROM test262.%I x WHERE %s',t,predicate)
  USING s.repository_id;
 RETURN portal;
END $function$;
REVOKE ALL ON FUNCTION test262.api_export_open(bigint,text) FROM PUBLIC,anon,authenticated,service_role,test262_ingest;
GRANT EXECUTE ON FUNCTION test262.api_export_open(bigint,text) TO test262_coordinator;

CREATE OR REPLACE FUNCTION test262.api_contract() RETURNS jsonb
LANGUAGE sql SECURITY DEFINER SET search_path=pg_catalog AS $function$
 SELECT (to_jsonb(c)-'singleton')||jsonb_build_object(
  'repository_id',s.repository_id,'producer_id',s.producer_id,
  'trust_class',s.trust_class,'permission',s.permission,'bulk_export_version',1)
 FROM test262.schema_contract c CROSS JOIN test262.api_subjects s
 WHERE s.login_name=session_user AND s.enabled;
$function$;
