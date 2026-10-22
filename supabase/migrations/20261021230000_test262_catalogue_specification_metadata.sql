-- Additive source annotations leave sealed inventories and historical evidence intact.
SET LOCAL lock_timeout='5s';
CREATE TABLE test262.fixture_specification_metadata (
 fixture_id uuid PRIMARY KEY REFERENCES test262.fixtures(fixture_id),
 upstream_revision text NOT NULL,
 upstream_path test262.relative_path NOT NULL,
 content_sha256 test262.sha256 NOT NULL,
 metadata jsonb NOT NULL CHECK(jsonb_typeof(metadata)='object'
   AND metadata @> '{"specification":{"schema_version":1}}'
   AND coalesce(jsonb_typeof(metadata#>'{specification,raw_identifiers}')='object',false)
   AND coalesce(metadata#>>'{specification,reference_state}' IN ('absent','legacy','unresolved','ambiguous'),false)),
 metadata_sha256 test262.sha256 NOT NULL,
 recorded_at timestamptz NOT NULL DEFAULT clock_timestamp(),
 actor text NOT NULL DEFAULT session_user
);
ALTER TABLE test262.fixture_specification_metadata ENABLE ROW LEVEL SECURITY;
REVOKE ALL ON test262.fixture_specification_metadata FROM PUBLIC,anon,authenticated,service_role,
 test262_ingest,test262_coordinator,test262_reporter;

CREATE FUNCTION test262.guard_specification_metadata() RETURNS trigger
LANGUAGE plpgsql SET search_path=pg_catalog AS $$
BEGIN
 IF NOT EXISTS(SELECT 1 FROM test262.fixtures f JOIN test262.corpora c USING(corpus_id)
   WHERE f.fixture_id=NEW.fixture_id AND f.upstream_path=NEW.upstream_path
   AND f.content_sha256=NEW.content_sha256 AND c.revision=NEW.upstream_revision
   AND c.inventory_state='sealed') THEN
  RAISE EXCEPTION 'Specification metadata source identity mismatch' USING ERRCODE='23514';
 END IF;
 IF NEW.metadata_sha256<>sha256(convert_to(test262.canonical_json(NEW.metadata),'UTF8')) THEN
  RAISE EXCEPTION 'Specification metadata digest mismatch' USING ERRCODE='23514';
 END IF;
 NEW.actor=session_user;
 NEW.recorded_at=clock_timestamp();
 RETURN NEW;
END $$;
REVOKE ALL ON FUNCTION test262.guard_specification_metadata() FROM PUBLIC,anon,authenticated,service_role;
CREATE TRIGGER specification_metadata_source_guard BEFORE INSERT ON test262.fixture_specification_metadata
 FOR EACH ROW EXECUTE FUNCTION test262.guard_specification_metadata();
CREATE TRIGGER immutable_evidence BEFORE UPDATE OR DELETE ON test262.fixture_specification_metadata
 FOR EACH ROW EXECUTE FUNCTION test262.reject_evidence_mutation();

CREATE OR REPLACE FUNCTION test262.api_scope(t text, d jsonb, r uuid) RETURNS boolean
LANGUAGE plpgsql SET search_path=pg_catalog AS $$
BEGIN
 IF d ? 'repository_id' THEN RETURN (d->>'repository_id')::uuid=r; END IF;
 CASE t
 WHEN 'corpora' THEN RETURN EXISTS(SELECT 1 FROM test262.repository_corpora WHERE repository_id=r AND corpus_id=(d->>'corpus_id')::uuid);
 WHEN 'fixtures' THEN RETURN EXISTS(SELECT 1 FROM test262.repository_corpora WHERE repository_id=r AND corpus_id=(d->>'corpus_id')::uuid);
 WHEN 'fixture_variants','fixture_dependencies','fixture_specification_metadata' THEN RETURN EXISTS(SELECT 1 FROM test262.fixtures f JOIN test262.repository_corpora rc USING(corpus_id) WHERE rc.repository_id=r AND f.fixture_id=(d->>'fixture_id')::uuid);
 WHEN 'registrations' THEN RETURN EXISTS(SELECT 1 FROM test262.registration_snapshots WHERE repository_id=r AND snapshot_id=(d->>'snapshot_id')::uuid);
 WHEN 'observation_payloads','observation_invalidations' THEN RETURN EXISTS(SELECT 1 FROM test262.observations WHERE repository_id=r AND observation_id=(d->>'observation_id')::uuid);
 WHEN 'batch_fixtures','batch_evidence' THEN RETURN EXISTS(SELECT 1 FROM test262.native_batches WHERE repository_id=r AND batch_id=(d->>'batch_id')::uuid);
 WHEN 'publication_checks' THEN RETURN EXISTS(SELECT 1 FROM test262.publications WHERE repository_id=r AND publication_id=(d->>'publication_id')::uuid);
 WHEN 'import_records','legacy_control_records' THEN RETURN EXISTS(SELECT 1 FROM test262.imports WHERE repository_id=r AND import_id=(d->>'import_id')::uuid);
 ELSE RETURN false;
 END CASE;
END $$;

CREATE OR REPLACE FUNCTION test262.api_put(epoch bigint, t text, rows jsonb) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; d jsonb; old jsonb; cols text; vals text; predicate text; supplied jsonb; n int=0;
BEGIN
 s=test262.api_identity(epoch,true); PERFORM test262.api_lock(s.repository_id);
 IF NOT t=ANY(ARRAY['corpora','repository_corpora','fixtures','fixture_specification_metadata','fixture_variants','fixture_dependencies','provenances','provenance_fixture_eligibility','registration_snapshots','registrations','validation_events','reporting_targets','reconciliation_state','reconciliations','native_batches','batch_fixtures','batch_evidence','publications','publication_checks','imports','import_records','legacy_control_records','run_metrics','budget_scopes','budget_work_items','work_items','observation_invalidations']) THEN RAISE EXCEPTION 'Unsupported record type'; END IF;
 IF jsonb_typeof(rows)<>'array' OR jsonb_array_length(rows)>250 OR octet_length(rows::text)>4194304 THEN RAISE EXCEPTION 'Oversized record chunk'; END IF;
 IF s.trust_class<>'trusted' AND NOT t=ANY(ARRAY['corpora','repository_corpora','fixtures','fixture_variants','fixture_dependencies','provenances','provenance_fixture_eligibility','registration_snapshots','registrations','imports','import_records','legacy_control_records']) THEN RAISE EXCEPTION 'Trusted coordinator required' USING ERRCODE='42501'; END IF;
 FOR d IN SELECT value FROM jsonb_array_elements(rows) LOOP
  supplied=d;
  IF NOT test262.api_scope(t,d,s.repository_id) AND t<>'corpora' THEN RAISE EXCEPTION 'Cross-repository write' USING ERRCODE='42501'; END IF;
  IF t='corpora' AND coalesce(d->>'inventory_state','staging')<>'staging' THEN RAISE EXCEPTION 'Corpus must start staging'; END IF;
  IF t='registration_snapshots' AND coalesce(d->>'state','staging')<>'staging' THEN RAISE EXCEPTION 'Snapshot must start staging'; END IF;
  IF t='reconciliation_state' AND coalesce((d->>'authority_epoch')::bigint,1)<>epoch THEN RAISE EXCEPTION 'Reconciliation state must use the current authority epoch' USING ERRCODE='40001'; END IF;
  IF t='work_items' AND (coalesce(d->>'state','pending')<>'pending' OR coalesce((d->>'lease_generation')::bigint,0)<>0 OR d ? 'lease_owner' OR d ? 'completion_observation_id') THEN RAISE EXCEPTION 'Work must start pending'; END IF;
  IF EXISTS(SELECT 1 FROM jsonb_object_keys(d) k WHERE NOT EXISTS(SELECT 1 FROM pg_attribute WHERE attrelid=format('test262.%I',t)::regclass AND attname=k AND attnum>0 AND NOT attisdropped)) THEN RAISE EXCEPTION 'Unknown column'; END IF;
  SELECT string_agg(format('%I',key),',' ORDER BY key),string_agg(format('x.%I',key),',' ORDER BY key) INTO cols,vals FROM jsonb_object_keys(d) key;
  SELECT string_agg(format('y.%1$I=x.%1$I',a.attname),' AND ' ORDER BY u.ord)
  INTO predicate FROM pg_constraint c CROSS JOIN LATERAL unnest(c.conkey) WITH ORDINALITY u(attnum,ord) JOIN pg_attribute a ON a.attrelid=c.conrelid AND a.attnum=u.attnum WHERE c.conrelid=format('test262.%I',t)::regclass AND c.contype='p';
  IF EXISTS(SELECT 1 FROM pg_constraint c CROSS JOIN LATERAL unnest(c.conkey) u(attnum) JOIN pg_attribute a ON a.attrelid=c.conrelid AND a.attnum=u.attnum WHERE c.conrelid=format('test262.%I',t)::regclass AND c.contype='p' AND NOT d ? a.attname) THEN RAISE EXCEPTION 'Missing primary key'; END IF;
  EXECUTE format('SELECT to_jsonb(y) FROM test262.%I y CROSS JOIN jsonb_populate_record(NULL::test262.%I,$1) x WHERE %s',t,t,predicate) INTO old USING d;
  IF old IS NULL THEN
    EXECUTE format('INSERT INTO test262.%I (%s) SELECT %s FROM jsonb_populate_record(NULL::test262.%I,$1) x RETURNING to_jsonb(%I.*)',t,cols,vals,t,t) INTO old USING d;
  END IF;
  EXECUTE format('SELECT to_jsonb(x) FROM jsonb_populate_record(NULL::test262.%I,$1) x',t) INTO d USING d;
  IF EXISTS(SELECT 1 FROM jsonb_object_keys(supplied) k WHERE old->k IS DISTINCT FROM d->k) THEN RAISE EXCEPTION 'Conflicting immutable replay for %',t; END IF;
  n=n+1;
 END LOOP;
 RETURN jsonb_build_object('records',n);
END $$;

-- Preserve the production 20261006101949 indexed legacy-control read path.
CREATE OR REPLACE FUNCTION test262.api_read(epoch bigint, t text, filter jsonb, after jsonb, page_size integer)
 RETURNS jsonb
 LANGUAGE plpgsql
 SECURITY DEFINER
 SET search_path TO 'pg_catalog'
AS $function$
DECLARE s test262.api_subjects; rel regclass; preds text[]; keys text[]; keycols text; afters text; rows jsonb; next jsonb;
BEGIN
 s=test262.api_identity(epoch);
 IF NOT t=ANY(ARRAY['corpora','repository_corpora','fixtures','fixture_specification_metadata','fixture_variants','fixture_dependencies','provenances','provenance_fixture_eligibility','runs','observations','observation_payloads','observation_invalidations','validation_events','work_items','work_leases','budget_reservations','budget_scopes','budget_work_items','registration_snapshots','registrations','reporting_targets','native_batches','batch_fixtures','batch_evidence','publications','publication_checks','imports','import_records','legacy_control_records','reconciliation_state','reconciliations','run_metrics']) THEN RAISE EXCEPTION 'Unsupported record type'; END IF;
 IF page_size NOT BETWEEN 1 AND 1000 THEN RAISE EXCEPTION 'Invalid page size'; END IF;
 -- Payload rows may each approach the 4 MiB ingest bound.
 IF t='observation_payloads' THEN page_size=least(page_size,25); END IF;
 rel=format('test262.%I',t)::regclass;
 filter=coalesce(filter,'{}'); after=nullif(after,'null'::jsonb);
 IF jsonb_typeof(filter)<>'object' OR (after IS NOT NULL AND jsonb_typeof(after)<>'object') THEN RAISE EXCEPTION 'Invalid read arguments'; END IF;
 IF EXISTS(SELECT 1 FROM jsonb_each(filter) e WHERE jsonb_typeof(e.value)='null' OR NOT EXISTS(SELECT 1 FROM pg_attribute WHERE attrelid=rel AND attname=e.key AND attnum>0 AND NOT attisdropped)) THEN RAISE EXCEPTION 'Invalid filter column'; END IF;
 SELECT array_agg(a.attname::text ORDER BY u.ord) INTO keys FROM pg_constraint c CROSS JOIN LATERAL unnest(c.conkey) WITH ORDINALITY u(attnum,ord) JOIN pg_attribute a ON a.attrelid=c.conrelid AND a.attnum=u.attnum WHERE c.conrelid=rel AND c.contype='p';

 -- Scalar typed filters become initplans/index conditions, rather than late join filters.
 -- Typed ownership lookup is equivalent to api_scope for legacy_control_records.
 IF t='legacy_control_records' THEN
  SELECT coalesce(array_agg(format('x.%1$I=(SELECT f.%1$I FROM jsonb_populate_record(NULL::test262.legacy_control_records,$1) f)',k)),'{}')
   INTO preds FROM jsonb_object_keys(filter) k;
  preds=preds||'EXISTS (SELECT 1 FROM test262.imports i WHERE i.import_id=x.import_id AND i.repository_id=$3)'::text;
  SELECT string_agg(format('x.%I',k),',' ORDER BY ord),
         string_agg(format('(SELECT a.%I FROM jsonb_populate_record(NULL::test262.legacy_control_records,$2) a)',k),',' ORDER BY ord)
   INTO keycols,afters FROM unnest(keys) WITH ORDINALITY u(k,ord);
  IF after IS NOT NULL THEN
   IF EXISTS(SELECT 1 FROM unnest(keys) k WHERE NOT after ? k) THEN RAISE EXCEPTION 'Cursor requires the exact primary key'; END IF;
   preds=preds||format('ROW(%s)>ROW(%s)',keycols,afters);
  END IF;
  EXECUTE format('SELECT coalesce(jsonb_agg(r.j ORDER BY r.n),''[]''::jsonb) FROM (SELECT to_jsonb(x) j,row_number() OVER (ORDER BY %1$s) n FROM test262.legacy_control_records x WHERE %2$s ORDER BY %1$s LIMIT %3$s) r',
   keycols,array_to_string(preds,' AND '),page_size) INTO rows USING filter,coalesce(after,'{}'::jsonb),s.repository_id;
  IF jsonb_array_length(rows)=page_size THEN
   SELECT jsonb_object_agg(k,rows->-1->k) INTO next FROM unnest(keys) k;
  END IF;
  RETURN jsonb_build_object('rows',rows,'next',next);
 END IF;
 SELECT coalesce(array_agg(format('x.%1$I=f.%1$I',k)),'{}') INTO preds FROM jsonb_object_keys(filter) k;

 IF EXISTS(SELECT 1 FROM pg_attribute WHERE attrelid=rel AND attname='repository_id' AND NOT attisdropped) THEN preds=preds||'x.repository_id=$3'::text;
 ELSE preds=preds||'test262.api_scope($4,to_jsonb(x),$3)'::text; END IF;
 SELECT string_agg(format('x.%I',k),','),string_agg(format('a.%I',k),',') INTO keycols,afters FROM unnest(keys) k;
 IF after IS NOT NULL THEN
  IF EXISTS(SELECT 1 FROM unnest(keys) k WHERE NOT after ? k) THEN RAISE EXCEPTION 'Cursor requires the exact primary key'; END IF;
  preds=preds||format('ROW(%s)>ROW(%s)',keycols,afters);
 END IF;
 EXECUTE format('SELECT coalesce(jsonb_agg(r.j ORDER BY r.n),''[]''::jsonb) FROM (SELECT to_jsonb(x) j,row_number() OVER (ORDER BY %1$s) n FROM test262.%2$I x CROSS JOIN jsonb_populate_record(NULL::test262.%2$I,$1) f CROSS JOIN jsonb_populate_record(NULL::test262.%2$I,$2) a WHERE %3$s ORDER BY %1$s LIMIT %4$s) r',
  keycols,t,array_to_string(preds,' AND '),page_size) INTO rows USING filter,coalesce(after,'{}'::jsonb),s.repository_id,t;
 IF jsonb_array_length(rows)=page_size THEN
  SELECT jsonb_object_agg(k,rows->-1->k) INTO next FROM unnest(keys) k;
 END IF;
 RETURN jsonb_build_object('rows',rows,'next',next);
END $function$


CREATE VIEW test262_reporting.v_fixture_specification_references WITH (security_barrier=true) AS
SELECT rc.repository_id,f.corpus_id,f.fixture_id,c.inventory_state,c.upstream_url,c.revision AS upstream_revision,
 c.revision_algorithm,f.upstream_path,encode(f.content_sha256,'hex') AS content_sha256,
 encode(c.inventory_digest,'hex') AS inventory_digest,
 CASE WHEN e.fixture_id IS NOT NULL THEN 'enrichment'
      WHEN f.metadata ? 'specification' THEN 'inventory' ELSE 'unavailable' END AS metadata_source,
 coalesce(d.metadata#>>'{specification,reference_state}','unresolved') AS reference_state,
 d.metadata#>'{specification,raw_identifiers}' AS raw_identifiers,
 d.metadata->'esid' AS esid,d.metadata->'es5id' AS es5id,d.metadata->'es6id' AS es6id,
 d.metadata->>'description' AS description,d.metadata->>'info' AS info,d.metadata->>'author' AS author,
 d.metadata#>>'{specification,raw_frontmatter}' AS raw_frontmatter,
 d.metadata#>>'{specification,diagnostic}' AS diagnostic,
 d.metadata#>>'{specification,schema_version}' AS metadata_schema_version,
 encode(e.metadata_sha256,'hex') AS enrichment_metadata_sha256,e.recorded_at AS enriched_at,
 'not-attempted'::text AS mapping_status,NULL::text AS specification_edition,NULL::text AS interpreted_clause,
 statement_timestamp() AS as_of
FROM test262.repository_corpora rc JOIN test262.corpora c USING(corpus_id)
JOIN test262.fixtures f USING(corpus_id)
LEFT JOIN test262.fixture_specification_metadata e USING(fixture_id)
CROSS JOIN LATERAL (SELECT coalesce(e.metadata,f.metadata) AS metadata) d;
REVOKE ALL ON test262_reporting.v_fixture_specification_references FROM PUBLIC,anon,authenticated,service_role;
GRANT SELECT ON test262_reporting.v_fixture_specification_references TO test262_reporter;
