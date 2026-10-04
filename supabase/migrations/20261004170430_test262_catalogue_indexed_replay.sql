-- Use typed primary keys so immutable replay lookups use existing indexes.
-- Keep API authorization, scope, size, immutability and sealed-inventory checks.
CREATE OR REPLACE FUNCTION test262.api_put(epoch bigint, t text, rows jsonb) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog AS $$
DECLARE s test262.api_subjects; d jsonb; old jsonb; cols text; vals text; predicate text; supplied jsonb; n int=0;
BEGIN
 s=test262.api_identity(epoch,true); PERFORM test262.api_lock(s.repository_id);
 IF NOT t=ANY(ARRAY['corpora','repository_corpora','fixtures','fixture_variants','fixture_dependencies','provenances','provenance_fixture_eligibility','registration_snapshots','registrations','validation_events','reporting_targets','reconciliation_state','reconciliations','native_batches','batch_fixtures','batch_evidence','publications','publication_checks','imports','import_records','legacy_control_records','run_metrics','budget_scopes','budget_work_items','work_items','observation_invalidations']) THEN RAISE EXCEPTION 'Unsupported record type'; END IF;
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
  -- Primary key is required; defaults cannot conceal identity conflicts.
  SELECT string_agg(format('y.%1$I=x.%1$I',a.attname),' AND ' ORDER BY u.ord)
  INTO predicate FROM pg_constraint c CROSS JOIN LATERAL unnest(c.conkey) WITH ORDINALITY u(attnum,ord) JOIN pg_attribute a ON a.attrelid=c.conrelid AND a.attnum=u.attnum WHERE c.conrelid=format('test262.%I',t)::regclass AND c.contype='p';
  IF EXISTS(SELECT 1 FROM pg_constraint c CROSS JOIN LATERAL unnest(c.conkey) u(attnum) JOIN pg_attribute a ON a.attrelid=c.conrelid AND a.attnum=u.attnum WHERE c.conrelid=format('test262.%I',t)::regclass AND c.contype='p' AND NOT d ? a.attname) THEN RAISE EXCEPTION 'Missing primary key'; END IF;
  -- Read before INSERT because sealed inventory INSERT triggers intentionally reject mutation.
  EXECUTE format('SELECT to_jsonb(y) FROM test262.%I y CROSS JOIN jsonb_populate_record(NULL::test262.%I,$1) x WHERE %s',t,t,predicate) INTO old USING d;
  IF old IS NULL THEN
    EXECUTE format('INSERT INTO test262.%I (%s) SELECT %s FROM jsonb_populate_record(NULL::test262.%I,$1) x RETURNING to_jsonb(%I.*)',t,cols,vals,t,t) INTO old USING d;
  END IF;
  -- Compare normalized values (bytea JSON encoding, timestamps, arrays), not caller formatting.
  EXECUTE format('SELECT to_jsonb(x) FROM jsonb_populate_record(NULL::test262.%I,$1) x',t) INTO d USING d;
  IF EXISTS(SELECT 1 FROM jsonb_object_keys(supplied) k WHERE old->k IS DISTINCT FROM d->k) THEN RAISE EXCEPTION 'Conflicting immutable replay for %',t; END IF;
  n=n+1;
 END LOOP;
 RETURN jsonb_build_object('records',n);
END $$;

