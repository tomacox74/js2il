-- Replace JSON-based per-row read scope with equivalent typed ownership predicates.
-- No data, write authorization, allowlists, grants or resource bounds change.
-- Keep the deployed indexed legacy-control path and the metadata allowlist.
SET LOCAL lock_timeout='5s';
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
 -- Typed scalar subqueries become initplans. A lateral record relation would
 -- leave the cursor as a late join filter and rescan earlier rows on every page.
 SELECT coalesce(array_agg(format('x.%1$I=(SELECT f.%1$I FROM jsonb_populate_record(NULL::test262.%2$I,$1) f)',k,t)),'{}')
  INTO preds FROM jsonb_object_keys(filter) k;

 IF EXISTS(SELECT 1 FROM pg_attribute WHERE attrelid=rel AND attname='repository_id' AND NOT attisdropped) THEN preds=preds||'x.repository_id=$3'::text;
 ELSE
  -- Equivalent to api_scope, exposing typed ownership keys to the planner.
  -- Do not serialize wide fixture metadata solely to authorize a read.
  CASE t
   WHEN 'corpora','fixtures' THEN
    preds=preds||'EXISTS (SELECT 1 FROM test262.repository_corpora rc WHERE rc.repository_id=$3 AND rc.corpus_id=x.corpus_id)'::text;
   WHEN 'fixture_variants','fixture_dependencies','fixture_specification_metadata' THEN
    preds=preds||'EXISTS (SELECT 1 FROM test262.fixtures f JOIN test262.repository_corpora rc USING(corpus_id) WHERE rc.repository_id=$3 AND f.fixture_id=x.fixture_id)'::text;
   WHEN 'registrations' THEN
    preds=preds||'EXISTS (SELECT 1 FROM test262.registration_snapshots p WHERE p.repository_id=$3 AND p.snapshot_id=x.snapshot_id)'::text;
   WHEN 'observation_payloads','observation_invalidations' THEN
    preds=preds||'EXISTS (SELECT 1 FROM test262.observations p WHERE p.repository_id=$3 AND p.observation_id=x.observation_id)'::text;
   WHEN 'batch_fixtures','batch_evidence' THEN
    preds=preds||'EXISTS (SELECT 1 FROM test262.native_batches p WHERE p.repository_id=$3 AND p.batch_id=x.batch_id)'::text;
   WHEN 'publication_checks' THEN
    preds=preds||'EXISTS (SELECT 1 FROM test262.publications p WHERE p.repository_id=$3 AND p.publication_id=x.publication_id)'::text;
   WHEN 'import_records' THEN
    preds=preds||'EXISTS (SELECT 1 FROM test262.imports p WHERE p.repository_id=$3 AND p.import_id=x.import_id)'::text;
   ELSE preds=preds||'false'::text;
  END CASE;
 END IF;
 SELECT string_agg(format('x.%I',k),',' ORDER BY ord),
        string_agg(format('(SELECT a.%1$I FROM jsonb_populate_record(NULL::test262.%2$I,$2) a)',k,t),',' ORDER BY ord)
  INTO keycols,afters FROM unnest(keys) WITH ORDINALITY u(k,ord);
 IF after IS NOT NULL THEN
  IF EXISTS(SELECT 1 FROM unnest(keys) k WHERE NOT after ? k) THEN RAISE EXCEPTION 'Cursor requires the exact primary key'; END IF;
  preds=preds||format('ROW(%s)>ROW(%s)',keycols,afters);
 END IF;
 EXECUTE format('SELECT coalesce(jsonb_agg(r.j ORDER BY r.n),''[]''::jsonb) FROM (SELECT to_jsonb(x) j,row_number() OVER (ORDER BY %1$s) n FROM test262.%2$I x WHERE %3$s ORDER BY %1$s LIMIT %4$s) r',
  keycols,t,array_to_string(preds,' AND '),page_size) INTO rows USING filter,coalesce(after,'{}'::jsonb),s.repository_id,t;
 IF jsonb_array_length(rows)=page_size THEN
  SELECT jsonb_object_agg(k,rows->-1->k) INTO next FROM unnest(keys) k;
 END IF;
 RETURN jsonb_build_object('rows',rows,'next',next);
END $function$;


