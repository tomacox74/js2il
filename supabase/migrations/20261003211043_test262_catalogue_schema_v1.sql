DO $deployment$
BEGIN
 EXECUTE $schema_sql$-- Approved schema from tomacox74/js2il#2230. Schema foundation; worker/API cutover remains separate.
SET LOCAL lock_timeout = '5s';
SET LOCAL statement_timeout = '60s';
CREATE SCHEMA test262;
CREATE SCHEMA test262_reporting;
REVOKE ALL ON SCHEMA test262, test262_reporting FROM PUBLIC, anon, authenticated, service_role;
CREATE ROLE test262_ingest NOLOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT;
CREATE ROLE test262_coordinator NOLOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT;
CREATE ROLE test262_reporter NOLOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT;
CREATE DOMAIN test262.sha256 AS bytea CHECK (octet_length(VALUE)=32);
CREATE DOMAIN test262.relative_path AS text CHECK (VALUE<>'' AND VALUE !~ '(^/|(^|/)\.\.(/|$)|\\|//)' );
CREATE DOMAIN test262.nonnegative_ms AS bigint CHECK (VALUE >= 0);
CREATE DOMAIN test262.evidence_kind AS text CHECK (VALUE IN ('mvp-composite','native'));

CREATE TABLE test262.repositories (
repository_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), provider text NOT NULL, provider_repository_id text NOT NULL, canonical_name text NOT NULL, created_at timestamptz NOT NULL DEFAULT clock_timestamp(), UNIQUE(provider,provider_repository_id)
);
CREATE TABLE test262.corpora (
corpus_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), upstream_url text NOT NULL, revision text NOT NULL CHECK(revision ~ '^[0-9a-fA-F]{40}([0-9a-fA-F]{24})?$'), revision_algorithm text NOT NULL CHECK(revision_algorithm IN ('sha1','sha256')), inventory_digest test262.sha256 NOT NULL, expected_fixture_count integer NOT NULL CHECK(expected_fixture_count>=0), inventory_state text NOT NULL DEFAULT 'staging' CHECK(inventory_state IN ('staging','incomplete','sealed')), created_at timestamptz NOT NULL DEFAULT clock_timestamp(), UNIQUE(upstream_url,revision_algorithm,revision,inventory_digest), CHECK((revision_algorithm='sha1' AND length(revision)=40) OR (revision_algorithm='sha256' AND length(revision)=64))
);
CREATE TABLE test262.repository_corpora (
repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), corpus_id uuid NOT NULL REFERENCES test262.corpora(corpus_id), first_seen_at timestamptz NOT NULL DEFAULT clock_timestamp(), PRIMARY KEY(repository_id,corpus_id)
);
CREATE TABLE test262.fixtures (
fixture_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), corpus_id uuid NOT NULL REFERENCES test262.corpora(corpus_id), upstream_path test262.relative_path NOT NULL, content_sha256 test262.sha256 NOT NULL, metadata jsonb NOT NULL DEFAULT '{}', metadata_state text NOT NULL CHECK(metadata_state IN ('valid','error','unresolved')), metadata_diagnostic text, is_support_file boolean NOT NULL DEFAULT false, feature_tags text[] NOT NULL DEFAULT '{}', dependency_manifest_digest test262.sha256, UNIQUE(corpus_id,upstream_path), UNIQUE(corpus_id,fixture_id)
);
CREATE TABLE test262.fixture_variants (
fixture_id uuid NOT NULL REFERENCES test262.fixtures(fixture_id), variant text NOT NULL CHECK(variant IN ('strict','non-strict','module')), expected_phase text CHECK(expected_phase IN ('parse','early','resolution','runtime')), expected_error_type text, required boolean NOT NULL DEFAULT true, PRIMARY KEY(fixture_id,variant), CHECK((expected_phase IS NULL)=(expected_error_type IS NULL))
);
CREATE TABLE test262.fixture_dependencies (
fixture_id uuid NOT NULL REFERENCES test262.fixtures(fixture_id), dependency_path test262.relative_path NOT NULL, content_sha256 test262.sha256 NOT NULL, dependency_kind text NOT NULL CHECK(dependency_kind IN ('sibling','harness','module','other')), resolved_fixture_id uuid REFERENCES test262.fixtures(fixture_id), PRIMARY KEY(fixture_id,dependency_path)
);
CREATE TABLE test262.provenances (
provenance_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), corpus_id uuid NOT NULL, evidence_kind test262.evidence_kind NOT NULL, identity_version integer NOT NULL CHECK(identity_version>0), identity_sha256 test262.sha256 NOT NULL, identity_document jsonb NOT NULL CHECK(jsonb_typeof(identity_document)='object'), capability_sha256 test262.sha256 NOT NULL, created_at timestamptz NOT NULL DEFAULT clock_timestamp(), FOREIGN KEY(repository_id,corpus_id) REFERENCES test262.repository_corpora(repository_id,corpus_id), UNIQUE(repository_id,provenance_id), UNIQUE(repository_id,evidence_kind,identity_version,identity_sha256)
);
CREATE TABLE test262.provenance_fixture_eligibility (
repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), provenance_id uuid NOT NULL, fixture_id uuid NOT NULL REFERENCES test262.fixtures(fixture_id), eligibility text NOT NULL CHECK(eligibility IN ('runnable','harness-gap','policy-excluded','metadata-error','unresolved')), reason_codes text[] NOT NULL DEFAULT '{}', diagnostic jsonb NOT NULL DEFAULT '{}', PRIMARY KEY(provenance_id,fixture_id), FOREIGN KEY(repository_id,provenance_id) REFERENCES test262.provenances(repository_id,provenance_id)
);
CREATE TABLE test262.producers (
producer_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), kind text NOT NULL CHECK(kind IN ('actions','local','importer')), display_name text NOT NULL, credential_subject text NOT NULL, enabled boolean NOT NULL DEFAULT true, last_seen_at timestamptz, UNIQUE(repository_id,producer_id), UNIQUE(repository_id,credential_subject)
);
CREATE TABLE test262.runs (
run_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), producer_id uuid NOT NULL, provenance_id uuid NOT NULL, external_run_key text NOT NULL, source_revision text NOT NULL CHECK(source_revision ~ '^[0-9a-fA-F]{40}([0-9a-fA-F]{24})?$'), validated_target_revision text CHECK(validated_target_revision ~ '^[0-9a-fA-F]{40}([0-9a-fA-F]{24})?$'), run_kind text NOT NULL CHECK(run_kind IN ('mvp','native','import','verification')), trust_class text NOT NULL DEFAULT 'untrusted' CHECK(trust_class IN ('untrusted','trusted','legacy')), state text NOT NULL DEFAULT 'planned' CHECK(state IN ('planned','running','complete','incomplete','failed','cancelled')), started_at timestamptz NOT NULL DEFAULT clock_timestamp(), ended_at timestamptz, environment_diagnostics jsonb NOT NULL DEFAULT '{}', UNIQUE(repository_id,run_id), UNIQUE(repository_id,producer_id,external_run_key), FOREIGN KEY(repository_id,producer_id) REFERENCES test262.producers(repository_id,producer_id), FOREIGN KEY(repository_id,provenance_id) REFERENCES test262.provenances(repository_id,provenance_id), CHECK(ended_at IS NULL OR ended_at>=started_at)
);
CREATE TABLE test262.work_items (
work_item_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), provenance_id uuid NOT NULL, fixture_id uuid NOT NULL, variant text NOT NULL, retry_generation integer NOT NULL DEFAULT 0 CHECK(retry_generation>=0), coherent_area text NOT NULL, priority integer NOT NULL DEFAULT 0, selection_reason jsonb NOT NULL DEFAULT '{}', state text NOT NULL DEFAULT 'pending' CHECK(state IN ('pending','leased','completed','deferred','cancelled')), eligible_after timestamptz NOT NULL DEFAULT clock_timestamp(), attempt_limit integer NOT NULL CHECK(attempt_limit>0), lease_generation bigint NOT NULL DEFAULT 0 CHECK(lease_generation>=0), lease_owner uuid, lease_expires_at timestamptz, completion_observation_id uuid, version bigint NOT NULL DEFAULT 0 CHECK(version>=0), UNIQUE(repository_id,work_item_id), UNIQUE(repository_id,provenance_id,fixture_id,variant,retry_generation), FOREIGN KEY(repository_id,provenance_id) REFERENCES test262.provenances(repository_id,provenance_id), FOREIGN KEY(repository_id,lease_owner) REFERENCES test262.producers(repository_id,producer_id), FOREIGN KEY(fixture_id,variant) REFERENCES test262.fixture_variants(fixture_id,variant), CHECK((state='leased' AND lease_owner IS NOT NULL AND lease_expires_at IS NOT NULL AND lease_generation>0) OR (state<>'leased' AND lease_owner IS NULL AND lease_expires_at IS NULL)), CHECK((state='completed')=(completion_observation_id IS NOT NULL))
);
CREATE TABLE test262.observations (
observation_id uuid PRIMARY KEY, repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), run_id uuid NOT NULL, provenance_id uuid NOT NULL, fixture_id uuid NOT NULL, variant text NOT NULL, work_item_id uuid, lease_generation bigint CHECK(lease_generation>0), outcome text NOT NULL CHECK(outcome IN ('pass','fail','unsupported','infrastructure-error','incomplete','deferred')), phase text NOT NULL CHECK(phase IN ('load','parse','early','resolution','compile','runtime','execution','timeout','planning','unknown')), observed_error_type text, failure_class text CHECK(failure_class IN ('product-feature-gap','semantic-defect','harness-gap','policy-exclusion','infrastructure-error','unresolved')), diagnostic_summary text NOT NULL DEFAULT '', started_at timestamptz NOT NULL, finished_at timestamptz NOT NULL, active_ms test262.nonnegative_ms NOT NULL, received_at timestamptz NOT NULL DEFAULT clock_timestamp(), payload_sha256 test262.sha256 NOT NULL, source_kind text NOT NULL CHECK(source_kind IN ('live','legacy-snapshot')), UNIQUE(repository_id,observation_id), FOREIGN KEY(repository_id,run_id) REFERENCES test262.runs(repository_id,run_id), FOREIGN KEY(repository_id,provenance_id) REFERENCES test262.provenances(repository_id,provenance_id), FOREIGN KEY(repository_id,work_item_id) REFERENCES test262.work_items(repository_id,work_item_id), FOREIGN KEY(fixture_id,variant) REFERENCES test262.fixture_variants(fixture_id,variant), CHECK(finished_at>=started_at), CHECK(outcome<>'pass' OR (failure_class IS NULL AND phase<>'timeout')), CHECK(outcome<>'infrastructure-error' OR failure_class='infrastructure-error'), CHECK(outcome<>'unsupported' OR failure_class IN ('harness-gap','policy-exclusion')), CHECK((work_item_id IS NULL)=(lease_generation IS NULL))
);
ALTER TABLE test262.work_items ADD FOREIGN KEY(repository_id,completion_observation_id) REFERENCES test262.observations(repository_id,observation_id);
CREATE TABLE test262.observation_payloads (
observation_id uuid PRIMARY KEY REFERENCES test262.observations(observation_id), payload jsonb, archive_uri text, archive_sha256 test262.sha256, availability text NOT NULL CHECK(availability IN ('hot','archived','missing')), archived_at timestamptz, CHECK(availability<>'hot' OR payload IS NOT NULL), CHECK(availability<>'archived' OR (archive_uri IS NOT NULL AND archive_sha256 IS NOT NULL AND archived_at IS NOT NULL))
);
CREATE TABLE test262.observation_invalidations (
observation_id uuid PRIMARY KEY REFERENCES test262.observations(observation_id), reason text NOT NULL CHECK(reason<>''), actor text NOT NULL, created_at timestamptz NOT NULL DEFAULT clock_timestamp()
);
CREATE TABLE test262.ingest_requests (
repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), producer_id uuid NOT NULL, request_id uuid NOT NULL, request_sha256 test262.sha256 NOT NULL, record_count integer NOT NULL CHECK(record_count BETWEEN 0 AND 1000), state text NOT NULL CHECK(state IN ('pending','committed','rejected')), committed_at timestamptz, receipt jsonb NOT NULL DEFAULT '{}', PRIMARY KEY(repository_id,producer_id,request_id), FOREIGN KEY(repository_id,producer_id) REFERENCES test262.producers(repository_id,producer_id), CHECK((state='committed')=(committed_at IS NOT NULL))
);
CREATE TABLE test262.run_metrics (
repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), run_id uuid NOT NULL, stage text NOT NULL, duration_ms test262.nonnegative_ms NOT NULL, attempt_count integer NOT NULL CHECK(attempt_count>=0), timeout_count integer NOT NULL CHECK(timeout_count>=0 AND timeout_count<=attempt_count), measurement_version integer NOT NULL CHECK(measurement_version>0), PRIMARY KEY(run_id,stage), FOREIGN KEY(repository_id,run_id) REFERENCES test262.runs(repository_id,run_id)
);
CREATE TABLE test262.work_leases (
work_item_id uuid NOT NULL, lease_generation bigint NOT NULL CHECK(lease_generation>0), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), producer_id uuid NOT NULL, run_id uuid NOT NULL, claimed_at timestamptz NOT NULL DEFAULT clock_timestamp(), expires_at timestamptz NOT NULL, released_at timestamptz, release_reason text, PRIMARY KEY(work_item_id,lease_generation), UNIQUE(repository_id,work_item_id,lease_generation), FOREIGN KEY(repository_id,work_item_id) REFERENCES test262.work_items(repository_id,work_item_id), FOREIGN KEY(repository_id,producer_id) REFERENCES test262.producers(repository_id,producer_id), FOREIGN KEY(repository_id,run_id) REFERENCES test262.runs(repository_id,run_id), CHECK(expires_at>claimed_at), CHECK(released_at IS NULL OR released_at>=claimed_at)
);
ALTER TABLE test262.observations ADD FOREIGN KEY(repository_id,work_item_id,lease_generation) REFERENCES test262.work_leases(repository_id,work_item_id,lease_generation);
CREATE TABLE test262.budget_scopes (
budget_scope_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), scope_kind text NOT NULL CHECK(scope_kind IN ('batch','run','repository-window')), external_key text NOT NULL, candidate_limit integer NOT NULL CHECK(candidate_limit>0), accepted_limit integer NOT NULL CHECK(accepted_limit>0 AND accepted_limit<=candidate_limit), attempt_limit integer NOT NULL CHECK(attempt_limit>0), time_limit_ms test262.nonnegative_ms NOT NULL CHECK(time_limit_ms>0), reserved_attempts integer NOT NULL DEFAULT 0 CHECK(reserved_attempts>=0), charged_attempts integer NOT NULL DEFAULT 0 CHECK(charged_attempts>=0), reserved_ms test262.nonnegative_ms NOT NULL DEFAULT 0, charged_ms test262.nonnegative_ms NOT NULL DEFAULT 0, state text NOT NULL DEFAULT 'open' CHECK(state IN ('open','exhausted','closed')), version bigint NOT NULL DEFAULT 0 CHECK(version>=0), UNIQUE(repository_id,budget_scope_id), UNIQUE(repository_id,scope_kind,external_key), CHECK(reserved_attempts+charged_attempts<=attempt_limit), CHECK(reserved_ms+charged_ms<=time_limit_ms)
);
CREATE TABLE test262.budget_reservations (
reservation_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), budget_scope_id uuid NOT NULL, work_item_id uuid NOT NULL, lease_generation bigint NOT NULL, reserved_attempts integer NOT NULL CHECK(reserved_attempts>0), reserved_ms test262.nonnegative_ms NOT NULL, charged_attempts integer NOT NULL DEFAULT 0 CHECK(charged_attempts>=0 AND charged_attempts<=reserved_attempts), charged_ms test262.nonnegative_ms NOT NULL DEFAULT 0 CHECK(charged_ms<=reserved_ms), state text NOT NULL DEFAULT 'reserved' CHECK(state IN ('reserved','settled','uncertain','released')), UNIQUE(repository_id,reservation_id), UNIQUE(budget_scope_id,work_item_id,lease_generation), FOREIGN KEY(repository_id,budget_scope_id) REFERENCES test262.budget_scopes(repository_id,budget_scope_id), FOREIGN KEY(repository_id,work_item_id,lease_generation) REFERENCES test262.work_leases(repository_id,work_item_id,lease_generation)
);
CREATE TABLE test262.execution_events (
event_id uuid PRIMARY KEY, repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), run_id uuid NOT NULL, reservation_id uuid NOT NULL, kind text NOT NULL CHECK(kind IN ('variant','group-compile','bisection')), group_identity text, parent_event_id uuid, state text NOT NULL CHECK(state IN ('started','complete','timeout','failed','incomplete')), active_ms test262.nonnegative_ms NOT NULL, payload_sha256 test262.sha256 NOT NULL, UNIQUE(repository_id,event_id), FOREIGN KEY(repository_id,run_id) REFERENCES test262.runs(repository_id,run_id), FOREIGN KEY(repository_id,reservation_id) REFERENCES test262.budget_reservations(repository_id,reservation_id), FOREIGN KEY(repository_id,parent_event_id) REFERENCES test262.execution_events(repository_id,event_id)
);
CREATE TABLE test262.retry_decisions (
decision_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), work_item_id uuid NOT NULL, decision text NOT NULL CHECK(decision IN ('defer','reopen','cancel','backoff')), reason text NOT NULL, trigger_revision text, capability_sha256 test262.sha256, created_at timestamptz NOT NULL DEFAULT clock_timestamp(), FOREIGN KEY(repository_id,work_item_id) REFERENCES test262.work_items(repository_id,work_item_id)
);
CREATE TABLE test262.registration_snapshots (
snapshot_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), source_revision text NOT NULL CHECK(source_revision ~ '^[0-9a-fA-F]{40}([0-9a-fA-F]{24})?$'), inventory_sha256 test262.sha256 NOT NULL, state text NOT NULL DEFAULT 'staging' CHECK(state IN ('staging','incomplete','sealed')), warnings jsonb NOT NULL DEFAULT '[]', created_at timestamptz NOT NULL DEFAULT clock_timestamp(), UNIQUE(repository_id,snapshot_id), UNIQUE(repository_id,source_revision,inventory_sha256)
);
CREATE TABLE test262.registrations (
snapshot_id uuid NOT NULL REFERENCES test262.registration_snapshots(snapshot_id), upstream_path test262.relative_path NOT NULL, source_file test262.relative_path NOT NULL, native_fixture_sha256 test262.sha256 NOT NULL, registration_kind text NOT NULL, fixture_id uuid REFERENCES test262.fixtures(fixture_id), PRIMARY KEY(snapshot_id,upstream_path,source_file,registration_kind)
);
CREATE TABLE test262.validation_events (
validation_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), workflow_identity text NOT NULL, external_run_key text NOT NULL, target_revision text NOT NULL CHECK(target_revision ~ '^[0-9a-fA-F]{40}([0-9a-fA-F]{24})?$'), conclusion text NOT NULL CHECK(conclusion IN ('success','failure','cancelled','incomplete')), verified_at timestamptz NOT NULL DEFAULT clock_timestamp(), proof jsonb NOT NULL, UNIQUE(repository_id,validation_id), UNIQUE(repository_id,workflow_identity,external_run_key)
);
CREATE TABLE test262.reporting_targets (
repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), channel text NOT NULL, evidence_kind test262.evidence_kind NOT NULL, provenance_id uuid NOT NULL, registration_snapshot_id uuid NOT NULL, validation_id uuid, version bigint NOT NULL DEFAULT 0 CHECK(version>=0), updated_at timestamptz NOT NULL DEFAULT clock_timestamp(), PRIMARY KEY(repository_id,channel,evidence_kind), FOREIGN KEY(repository_id,provenance_id) REFERENCES test262.provenances(repository_id,provenance_id), FOREIGN KEY(repository_id,registration_snapshot_id) REFERENCES test262.registration_snapshots(repository_id,snapshot_id), FOREIGN KEY(repository_id,validation_id) REFERENCES test262.validation_events(repository_id,validation_id)
);
CREATE TABLE test262.reconciliation_state (
repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), pipeline text NOT NULL, channel text NOT NULL, last_validation_id uuid, last_reconciled_revision text, fallback_cursor bigint NOT NULL DEFAULT 0 CHECK(fallback_cursor>=0), authority_epoch bigint NOT NULL DEFAULT 1 CHECK(authority_epoch>0), version bigint NOT NULL DEFAULT 0 CHECK(version>=0), PRIMARY KEY(repository_id,pipeline,channel), FOREIGN KEY(repository_id,last_validation_id) REFERENCES test262.validation_events(repository_id,validation_id)
);
CREATE TABLE test262.reconciliations (
reconciliation_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), pipeline text NOT NULL, channel text NOT NULL, validation_id uuid NOT NULL, base_revision text NOT NULL, attribution text NOT NULL, components text[] NOT NULL DEFAULT '{}', plan_sha256 test262.sha256 NOT NULL, expected_cursor_version bigint NOT NULL CHECK(expected_cursor_version>=0), state text NOT NULL DEFAULT 'planned' CHECK(state IN ('planned','committed','superseded','failed')), committed_at timestamptz, UNIQUE(repository_id,reconciliation_id), FOREIGN KEY(repository_id,validation_id) REFERENCES test262.validation_events(repository_id,validation_id), FOREIGN KEY(repository_id,pipeline,channel) REFERENCES test262.reconciliation_state(repository_id,pipeline,channel), CHECK((state='committed')=(committed_at IS NOT NULL))
);
CREATE TABLE test262.native_batches (
batch_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), batch_key text NOT NULL, provenance_id uuid NOT NULL, validation_id uuid NOT NULL, registration_snapshot_id uuid NOT NULL, budget_scope_id uuid NOT NULL, state text NOT NULL DEFAULT 'planned' CHECK(state IN ('planned','screening','sealed','awaiting-refresh','conflicted','cancelled')), version bigint NOT NULL DEFAULT 0 CHECK(version>=0), created_at timestamptz NOT NULL DEFAULT clock_timestamp(), UNIQUE(repository_id,batch_id), UNIQUE(repository_id,batch_key), FOREIGN KEY(repository_id,provenance_id) REFERENCES test262.provenances(repository_id,provenance_id), FOREIGN KEY(repository_id,validation_id) REFERENCES test262.validation_events(repository_id,validation_id), FOREIGN KEY(repository_id,registration_snapshot_id) REFERENCES test262.registration_snapshots(repository_id,snapshot_id), FOREIGN KEY(repository_id,budget_scope_id) REFERENCES test262.budget_scopes(repository_id,budget_scope_id)
);
CREATE TABLE test262.batch_fixtures (
batch_id uuid NOT NULL REFERENCES test262.native_batches(batch_id), fixture_id uuid NOT NULL REFERENCES test262.fixtures(fixture_id), state text NOT NULL DEFAULT 'pending' CHECK(state IN ('pending','accepted','deferred','excluded')), PRIMARY KEY(batch_id,fixture_id)
);
CREATE TABLE test262.batch_evidence (
batch_id uuid NOT NULL, fixture_id uuid NOT NULL, variant text NOT NULL, observation_id uuid NOT NULL REFERENCES test262.observations(observation_id), PRIMARY KEY(batch_id,fixture_id,variant), FOREIGN KEY(batch_id,fixture_id) REFERENCES test262.batch_fixtures(batch_id,fixture_id), FOREIGN KEY(fixture_id,variant) REFERENCES test262.fixture_variants(fixture_id,variant)
);
CREATE TABLE test262.publications (
publication_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), batch_id uuid NOT NULL UNIQUE, branch_name text NOT NULL, pr_number integer CHECK(pr_number>0), expected_head text, patch_sha256 test262.sha256 NOT NULL, manifest_sha256 test262.sha256 NOT NULL, state text NOT NULL DEFAULT 'reserved' CHECK(state IN ('reserved','open','updating','merged','closed-deferred','ownership-lost','blocked','cancelled')), closure_reason text, version bigint NOT NULL DEFAULT 0 CHECK(version>=0), UNIQUE(repository_id,publication_id), UNIQUE(repository_id,branch_name), FOREIGN KEY(repository_id,batch_id) REFERENCES test262.native_batches(repository_id,batch_id)
);
CREATE UNIQUE INDEX publications_pr ON test262.publications(repository_id,pr_number) WHERE pr_number IS NOT NULL;
CREATE UNIQUE INDEX publications_one_active ON test262.publications(repository_id) WHERE state IN ('reserved','open','updating');
CREATE TABLE test262.publication_checks (
publication_id uuid NOT NULL REFERENCES test262.publications(publication_id), check_identity text NOT NULL, external_run_key text NOT NULL, head_revision text NOT NULL, conclusion text NOT NULL CHECK(conclusion IN ('success','failure','cancelled','pending')), verified_at timestamptz NOT NULL DEFAULT clock_timestamp(), PRIMARY KEY(publication_id,check_identity,external_run_key)
);
CREATE TABLE test262.imports (
import_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), source_sha256 test262.sha256 NOT NULL, source_schema text NOT NULL, source_uri text NOT NULL, state text NOT NULL DEFAULT 'planned' CHECK(state IN ('planned','running','complete','incomplete','failed')), expected_counts jsonb NOT NULL DEFAULT '{}', imported_counts jsonb NOT NULL DEFAULT '{}', limitations jsonb NOT NULL DEFAULT '{}', started_at timestamptz NOT NULL DEFAULT clock_timestamp(), completed_at timestamptz, UNIQUE(repository_id,import_id), UNIQUE(repository_id,source_sha256,source_schema), CHECK((state='complete')=(completed_at IS NOT NULL))
);
CREATE TABLE test262.import_records (
import_id uuid NOT NULL REFERENCES test262.imports(import_id), source_table text NOT NULL, source_key text NOT NULL, entity_kind text NOT NULL, entity_id uuid NOT NULL, PRIMARY KEY(import_id,source_table,source_key)
);
CREATE TABLE test262.audit_events (
event_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), actor_subject text NOT NULL, action text NOT NULL, entity_kind text NOT NULL, entity_id uuid, details jsonb NOT NULL DEFAULT '{}', created_at timestamptz NOT NULL DEFAULT clock_timestamp()
);
CREATE TABLE test262.exports (
export_id uuid PRIMARY KEY DEFAULT gen_random_uuid(), repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), target_document jsonb NOT NULL, schema_version integer NOT NULL CHECK(schema_version>0), snapshot_token text NOT NULL, content_sha256 test262.sha256 NOT NULL, row_counts jsonb NOT NULL, created_at timestamptz NOT NULL DEFAULT clock_timestamp(), UNIQUE(repository_id,snapshot_token)
);
CREATE TABLE test262.variant_result_projection (
repository_id uuid NOT NULL REFERENCES test262.repositories(repository_id), provenance_id uuid NOT NULL, fixture_id uuid NOT NULL, variant text NOT NULL, representative_observation_id uuid NOT NULL, attempt_count bigint NOT NULL CHECK(attempt_count>0), conflict_flag boolean NOT NULL, policy_version integer NOT NULL CHECK(policy_version>0), updated_at timestamptz NOT NULL DEFAULT clock_timestamp(), PRIMARY KEY(repository_id,provenance_id,fixture_id,variant), FOREIGN KEY(repository_id,provenance_id) REFERENCES test262.provenances(repository_id,provenance_id), FOREIGN KEY(repository_id,representative_observation_id) REFERENCES test262.observations(repository_id,observation_id), FOREIGN KEY(fixture_id,variant) REFERENCES test262.fixture_variants(fixture_id,variant)
);
CREATE TABLE test262.schema_contract (
singleton boolean PRIMARY KEY DEFAULT true CHECK(singleton), schema_version integer NOT NULL CHECK(schema_version>0), api_contract_version integer NOT NULL CHECK(api_contract_version>0), minimum_writer_epoch bigint NOT NULL CHECK(minimum_writer_epoch>0), deployment_state text NOT NULL CHECK(deployment_state IN ('schema-only','shadow','active')), migration_name text NOT NULL, applied_at timestamptz NOT NULL DEFAULT clock_timestamp()
);
INSERT INTO test262.schema_contract(singleton,schema_version,api_contract_version,minimum_writer_epoch,deployment_state,migration_name) VALUES(true,1,1,1,'schema-only','test262_catalogue_schema_v1');

CREATE FUNCTION test262.reject_evidence_mutation() RETURNS trigger LANGUAGE plpgsql SET search_path=pg_catalog AS $$
BEGIN RAISE EXCEPTION 'Immutable evidence: %.%',TG_TABLE_SCHEMA,TG_TABLE_NAME USING ERRCODE='23514'; END $$;
CREATE FUNCTION test262.guard_inventory() RETURNS trigger LANGUAGE plpgsql SET search_path=pg_catalog AS $$
DECLARE cid uuid; sid uuid; st text;
BEGIN
 IF TG_TABLE_NAME='corpora' THEN
   IF OLD.inventory_state='sealed' THEN RAISE EXCEPTION 'Sealed corpus is immutable' USING ERRCODE='23514'; END IF;
   IF TG_OP='UPDATE' AND NEW.inventory_state='sealed' THEN
     IF (SELECT count(*) FROM test262.fixtures WHERE corpus_id=NEW.corpus_id)<>NEW.expected_fixture_count THEN RAISE EXCEPTION 'Incomplete corpus inventory' USING ERRCODE='23514'; END IF;
   END IF;
 ELSIF TG_TABLE_NAME='registration_snapshots' THEN
   IF OLD.state='sealed' THEN RAISE EXCEPTION 'Sealed registration snapshot is immutable' USING ERRCODE='23514'; END IF;
 ELSE
   IF TG_TABLE_NAME='registrations' THEN
     sid=CASE WHEN TG_OP='DELETE' THEN OLD.snapshot_id ELSE NEW.snapshot_id END;
     SELECT state INTO st FROM test262.registration_snapshots WHERE snapshot_id=sid FOR UPDATE;
   ELSE
     IF TG_TABLE_NAME='fixtures' THEN cid=CASE WHEN TG_OP='DELETE' THEN OLD.corpus_id ELSE NEW.corpus_id END;
     ELSE
       SELECT corpus_id INTO cid FROM test262.fixtures WHERE fixture_id=CASE WHEN TG_OP='DELETE' THEN OLD.fixture_id ELSE NEW.fixture_id END;
     END IF;
     SELECT inventory_state INTO st FROM test262.corpora WHERE corpus_id=cid FOR UPDATE;
   END IF;
   IF st='sealed' THEN RAISE EXCEPTION 'Cannot modify sealed inventory' USING ERRCODE='23514'; END IF;
 END IF;
 IF TG_OP='DELETE' THEN RETURN OLD; END IF;
 RETURN NEW;
END $$;
CREATE TRIGGER corpus_seal_guard BEFORE UPDATE OR DELETE ON test262.corpora FOR EACH ROW EXECUTE FUNCTION test262.guard_inventory();
CREATE TRIGGER snapshot_seal_guard BEFORE UPDATE OR DELETE ON test262.registration_snapshots FOR EACH ROW EXECUTE FUNCTION test262.guard_inventory();

CREATE TRIGGER inventory_guard BEFORE INSERT OR UPDATE OR DELETE ON test262.fixtures FOR EACH ROW EXECUTE FUNCTION test262.guard_inventory();
CREATE TRIGGER inventory_guard BEFORE INSERT OR UPDATE OR DELETE ON test262.fixture_variants FOR EACH ROW EXECUTE FUNCTION test262.guard_inventory();
CREATE TRIGGER inventory_guard BEFORE INSERT OR UPDATE OR DELETE ON test262.fixture_dependencies FOR EACH ROW EXECUTE FUNCTION test262.guard_inventory();
CREATE TRIGGER inventory_guard BEFORE INSERT OR UPDATE OR DELETE ON test262.registrations FOR EACH ROW EXECUTE FUNCTION test262.guard_inventory();
CREATE TRIGGER immutable_evidence BEFORE UPDATE OR DELETE ON test262.provenances FOR EACH ROW EXECUTE FUNCTION test262.reject_evidence_mutation();
CREATE TRIGGER immutable_evidence BEFORE UPDATE OR DELETE ON test262.observations FOR EACH ROW EXECUTE FUNCTION test262.reject_evidence_mutation();
CREATE TRIGGER immutable_evidence BEFORE UPDATE OR DELETE ON test262.validation_events FOR EACH ROW EXECUTE FUNCTION test262.reject_evidence_mutation();
CREATE TRIGGER immutable_evidence BEFORE UPDATE OR DELETE ON test262.audit_events FOR EACH ROW EXECUTE FUNCTION test262.reject_evidence_mutation();
CREATE TRIGGER immutable_evidence BEFORE UPDATE OR DELETE ON test262.observation_invalidations FOR EACH ROW EXECUTE FUNCTION test262.reject_evidence_mutation();

CREATE FUNCTION test262.guard_fixture_provenance() RETURNS trigger LANGUAGE plpgsql SET search_path=pg_catalog AS $$
DECLARE pc uuid; fc uuid; ps text; p test262.provenances; r test262.runs; w test262.work_items; v test262.fixture_variants; f test262.fixtures;
BEGIN
 SELECT * INTO p FROM test262.provenances WHERE provenance_id=NEW.provenance_id;
 SELECT * INTO f FROM test262.fixtures WHERE fixture_id=NEW.fixture_id;
 IF p.corpus_id IS DISTINCT FROM f.corpus_id OR p.repository_id IS DISTINCT FROM NEW.repository_id THEN
   RAISE EXCEPTION 'Fixture/provenance scope mismatch' USING ERRCODE='23514';
 END IF;
 SELECT inventory_state INTO ps FROM test262.corpora WHERE corpus_id=p.corpus_id;
 IF ps<>'sealed' THEN RAISE EXCEPTION 'Execution requires sealed corpus' USING ERRCODE='23514'; END IF;
 IF TG_TABLE_NAME IN ('work_items','observations') AND f.is_support_file THEN RAISE EXCEPTION 'Support file is not executable' USING ERRCODE='23514'; END IF;
 IF TG_TABLE_NAME='observations' THEN
   SELECT * INTO r FROM test262.runs WHERE run_id=NEW.run_id;
   IF r.provenance_id IS DISTINCT FROM NEW.provenance_id THEN RAISE EXCEPTION 'Observation/run provenance mismatch' USING ERRCODE='23514'; END IF;
   IF NEW.work_item_id IS NOT NULL THEN
     SELECT * INTO w FROM test262.work_items WHERE work_item_id=NEW.work_item_id;
     IF ROW(w.provenance_id,w.fixture_id,w.variant) IS DISTINCT FROM ROW(NEW.provenance_id,NEW.fixture_id,NEW.variant) THEN RAISE EXCEPTION 'Observation/work mismatch' USING ERRCODE='23514'; END IF;
   END IF;
   SELECT * INTO v FROM test262.fixture_variants WHERE fixture_id=NEW.fixture_id AND variant=NEW.variant;
   IF p.evidence_kind='native' AND NEW.outcome='pass' AND v.expected_phase IS NOT NULL THEN
     IF v.expected_phase<>'runtime' OR NEW.phase<>'runtime' OR NEW.observed_error_type IS DISTINCT FROM v.expected_error_type THEN
       RAISE EXCEPTION 'Native negative lacks matching phase/type' USING ERRCODE='23514';
     END IF;
   END IF;
 ELSIF TG_TABLE_NAME='work_items' THEN
  IF NEW.state='completed' THEN
   IF NOT EXISTS(SELECT 1 FROM test262.observations o WHERE o.observation_id=NEW.completion_observation_id AND
     ROW(o.repository_id,o.provenance_id,o.fixture_id,o.variant,o.work_item_id,o.lease_generation)=
     ROW(NEW.repository_id,NEW.provenance_id,NEW.fixture_id,NEW.variant,NEW.work_item_id,NEW.lease_generation)) THEN
     RAISE EXCEPTION 'Completion observation does not match current work generation' USING ERRCODE='23514';
   END IF;
  END IF;
 END IF;
 RETURN NEW;
END $$;

CREATE TRIGGER fixture_provenance_guard BEFORE INSERT OR UPDATE ON test262.provenance_fixture_eligibility FOR EACH ROW EXECUTE FUNCTION test262.guard_fixture_provenance();
CREATE TRIGGER fixture_provenance_guard BEFORE INSERT OR UPDATE ON test262.work_items FOR EACH ROW EXECUTE FUNCTION test262.guard_fixture_provenance();
CREATE TRIGGER fixture_provenance_guard BEFORE INSERT OR UPDATE ON test262.observations FOR EACH ROW EXECUTE FUNCTION test262.guard_fixture_provenance();
CREATE TRIGGER fixture_provenance_guard BEFORE INSERT OR UPDATE ON test262.variant_result_projection FOR EACH ROW EXECUTE FUNCTION test262.guard_fixture_provenance();

CREATE FUNCTION test262.guard_target_batch() RETURNS trigger LANGUAGE plpgsql SET search_path=pg_catalog AS $$
DECLARE p test262.provenances; s test262.registration_snapshots; v test262.validation_events;
BEGIN
 SELECT * INTO p FROM test262.provenances WHERE provenance_id=NEW.provenance_id;
 SELECT * INTO s FROM test262.registration_snapshots WHERE snapshot_id=NEW.registration_snapshot_id;
 IF s.state<>'sealed' THEN RAISE EXCEPTION 'Target needs sealed registration snapshot' USING ERRCODE='23514'; END IF;
 IF TG_TABLE_NAME='reporting_targets' THEN
 IF p.evidence_kind IS DISTINCT FROM NEW.evidence_kind THEN RAISE EXCEPTION 'Target evidence-kind mismatch' USING ERRCODE='23514'; END IF;
 END IF;
 IF TG_TABLE_NAME='native_batches' AND p.evidence_kind<>'native' THEN RAISE EXCEPTION 'Batch needs native provenance' USING ERRCODE='23514'; END IF;
 IF NEW.validation_id IS NOT NULL THEN
   SELECT * INTO v FROM test262.validation_events WHERE validation_id=NEW.validation_id;
   IF v.conclusion<>'success' OR s.source_revision IS DISTINCT FROM v.target_revision THEN RAISE EXCEPTION 'Target validation/registration revision mismatch' USING ERRCODE='23514'; END IF;
 END IF;
 RETURN NEW;
END $$;
CREATE TRIGGER target_guard BEFORE INSERT OR UPDATE ON test262.reporting_targets FOR EACH ROW EXECUTE FUNCTION test262.guard_target_batch();
CREATE TRIGGER batch_target_guard BEFORE INSERT OR UPDATE ON test262.native_batches FOR EACH ROW EXECUTE FUNCTION test262.guard_target_batch();

CREATE FUNCTION test262.guard_batch_members() RETURNS trigger LANGUAGE plpgsql SET search_path=pg_catalog AS $$
DECLARE b test262.native_batches; o test262.observations; fv test262.fixture_variants; trust text; cid uuid;
BEGIN
 SELECT * INTO b FROM test262.native_batches WHERE batch_id=CASE WHEN TG_OP='DELETE' THEN OLD.batch_id ELSE NEW.batch_id END FOR UPDATE;
 IF b.state IN ('sealed','awaiting-refresh','conflicted') THEN RAISE EXCEPTION 'Accepted batch bindings are frozen' USING ERRCODE='23514'; END IF;
 IF TG_OP='DELETE' THEN RETURN OLD; END IF;
 SELECT p.corpus_id INTO cid FROM test262.provenances p WHERE p.provenance_id=b.provenance_id;
 IF NOT EXISTS(SELECT 1 FROM test262.fixtures f WHERE f.fixture_id=NEW.fixture_id AND f.corpus_id=cid AND NOT f.is_support_file) THEN RAISE EXCEPTION 'Invalid batch fixture scope' USING ERRCODE='23514'; END IF;
 IF TG_TABLE_NAME='batch_evidence' THEN
   SELECT * INTO o FROM test262.observations WHERE observation_id=NEW.observation_id;
   SELECT trust_class INTO trust FROM test262.runs WHERE run_id=o.run_id;
   IF ROW(o.repository_id,o.provenance_id,o.fixture_id,o.variant) IS DISTINCT FROM ROW(b.repository_id,b.provenance_id,NEW.fixture_id,NEW.variant) OR o.outcome<>'pass' OR trust<>'trusted' OR o.source_kind<>'live' OR EXISTS(SELECT 1 FROM test262.observation_invalidations WHERE observation_id=o.observation_id) THEN
     RAISE EXCEPTION 'Batch requires trusted matching live native pass' USING ERRCODE='23514';
   END IF;
   IF NOT EXISTS(SELECT 1 FROM test262.fixture_variants WHERE fixture_id=NEW.fixture_id AND variant=NEW.variant AND required) THEN RAISE EXCEPTION 'Nonrequired variant cannot establish acceptance' USING ERRCODE='23514'; END IF;
 END IF;
 RETURN NEW;
END $$;
CREATE TRIGGER batch_member_guard BEFORE INSERT OR UPDATE OR DELETE ON test262.batch_fixtures FOR EACH ROW EXECUTE FUNCTION test262.guard_batch_members();
CREATE TRIGGER batch_evidence_guard BEFORE INSERT OR UPDATE OR DELETE ON test262.batch_evidence FOR EACH ROW EXECUTE FUNCTION test262.guard_batch_members();

CREATE FUNCTION test262.guard_batch_seal() RETURNS trigger LANGUAGE plpgsql SET search_path=pg_catalog AS $$
BEGIN
 IF TG_OP='DELETE' THEN
   IF OLD.state IN ('sealed','awaiting-refresh','conflicted') THEN RAISE EXCEPTION 'Cannot delete sealed batch' USING ERRCODE='23514'; END IF;
   RETURN OLD;
 END IF;
 IF TG_OP='INSERT' AND NEW.state<>'planned' THEN RAISE EXCEPTION 'Batch must start planned' USING ERRCODE='23514'; END IF;
 IF TG_OP='UPDATE' AND OLD.state IN ('sealed','awaiting-refresh','conflicted') THEN
   IF ROW(OLD.repository_id,OLD.batch_key,OLD.provenance_id,OLD.validation_id,OLD.registration_snapshot_id,OLD.budget_scope_id) IS DISTINCT FROM ROW(NEW.repository_id,NEW.batch_key,NEW.provenance_id,NEW.validation_id,NEW.registration_snapshot_id,NEW.budget_scope_id) OR NEW.state NOT IN ('sealed','awaiting-refresh','conflicted','cancelled') THEN RAISE EXCEPTION 'Sealed batch identity is frozen' USING ERRCODE='23514'; END IF;
 END IF;
 IF NEW.state='sealed' THEN
   IF NOT EXISTS(SELECT 1 FROM test262.batch_fixtures WHERE batch_id=NEW.batch_id AND state='accepted') THEN RAISE EXCEPTION 'Empty acceptance' USING ERRCODE='23514'; END IF;
   IF EXISTS(
     SELECT 1 FROM test262.batch_fixtures bf JOIN test262.fixtures f ON f.fixture_id=bf.fixture_id
     WHERE bf.batch_id=NEW.batch_id AND bf.state='accepted' AND (
       f.metadata_state<>'valid' OR
       NOT EXISTS(SELECT 1 FROM test262.fixture_variants v WHERE v.fixture_id=f.fixture_id AND v.required) OR
       EXISTS(SELECT 1 FROM test262.fixture_variants v WHERE v.fixture_id=f.fixture_id AND v.required AND NOT EXISTS(
         SELECT 1 FROM test262.batch_evidence be JOIN test262.observations o ON o.observation_id=be.observation_id JOIN test262.runs r ON r.run_id=o.run_id
         WHERE be.batch_id=NEW.batch_id AND be.fixture_id=f.fixture_id AND be.variant=v.variant AND o.provenance_id=NEW.provenance_id AND o.outcome='pass' AND r.trust_class='trusted' AND o.source_kind='live'
         AND NOT EXISTS(SELECT 1 FROM test262.observation_invalidations i WHERE i.observation_id=o.observation_id))) OR
       NOT EXISTS(SELECT 1 FROM test262.provenance_fixture_eligibility e WHERE e.provenance_id=NEW.provenance_id AND e.fixture_id=f.fixture_id AND e.eligibility='runnable') OR
       (EXISTS(SELECT 1 FROM test262.fixture_dependencies d WHERE d.fixture_id=f.fixture_id) AND f.dependency_manifest_digest IS NULL) OR
       EXISTS(SELECT 1 FROM test262.observations o JOIN test262.runs r ON r.run_id=o.run_id WHERE o.provenance_id=NEW.provenance_id AND o.fixture_id=f.fixture_id AND o.outcome<>'pass' AND r.trust_class='trusted' AND NOT EXISTS(SELECT 1 FROM test262.observation_invalidations i WHERE i.observation_id=o.observation_id))
     )) THEN RAISE EXCEPTION 'Incomplete, excluded, invalidated or conflicting acceptance' USING ERRCODE='23514'; END IF;
 END IF;
 RETURN NEW;
END $$;
CREATE TRIGGER batch_seal_guard BEFORE INSERT OR UPDATE OR DELETE ON test262.native_batches FOR EACH ROW EXECUTE FUNCTION test262.guard_batch_seal();
CREATE FUNCTION test262.guard_publication() RETURNS trigger LANGUAGE plpgsql SET search_path=pg_catalog AS $$
BEGIN
 IF NEW.state IN ('reserved','open','updating') AND NOT EXISTS(SELECT 1 FROM test262.native_batches WHERE batch_id=NEW.batch_id AND state='sealed') THEN RAISE EXCEPTION 'Publication requires sealed acceptance' USING ERRCODE='23514'; END IF;
 RETURN NEW;
END $$;
CREATE TRIGGER publication_guard BEFORE INSERT OR UPDATE ON test262.publications FOR EACH ROW EXECUTE FUNCTION test262.guard_publication();

CREATE INDEX observations_lookup ON test262.observations(repository_id,provenance_id,fixture_id,variant,received_at DESC,observation_id);
CREATE INDEX observations_run ON test262.observations(run_id);
CREATE INDEX observations_failures ON test262.observations(repository_id,failure_class,received_at DESC);
CREATE INDEX work_claim ON test262.work_items(repository_id,provenance_id,priority DESC,eligible_after,work_item_id) WHERE state='pending';
CREATE INDEX work_expiry ON test262.work_items(lease_expires_at) WHERE state='leased';
CREATE INDEX runs_revision ON test262.runs(repository_id,source_revision,started_at DESC);
CREATE INDEX registrations_path ON test262.registrations(snapshot_id,upstream_path);
CREATE INDEX imports_pending ON test262.imports(repository_id,state) WHERE state<>'complete';
CREATE INDEX ingest_pending ON test262.ingest_requests(repository_id,state) WHERE state='pending';

-- Add supporting indexes for FK-leading columns not already covered by an index.
DO $$
DECLARE k record;
BEGIN
 FOR k IN SELECT con.conrelid,con.conname,con.conkey,
   string_agg(quote_ident(a.attname),',' ORDER BY u.ord) AS cols
 FROM pg_constraint con
 JOIN pg_class c ON c.oid=con.conrelid JOIN pg_namespace n ON n.oid=c.relnamespace
 CROSS JOIN LATERAL unnest(con.conkey) WITH ORDINALITY u(attnum,ord)
 JOIN pg_attribute a ON a.attrelid=con.conrelid AND a.attnum=u.attnum
 WHERE n.nspname='test262' AND con.contype='f'
 GROUP BY con.conrelid,con.conname,con.conkey
 LOOP
   IF NOT EXISTS(SELECT 1 FROM pg_index i WHERE i.indrelid=k.conrelid AND i.indpred IS NULL AND
     (i.indkey::smallint[])[0:array_length(k.conkey,1)-1]=k.conkey) THEN
     EXECUTE format('CREATE INDEX %I ON %s (%s)','fk_'||substr(md5(k.conname||k.conrelid::text),1,24),k.conrelid::regclass,k.cols);
   END IF;
 END LOOP;
END $$;


-- Raw trusted-observation projection: immediately consistent, no background refresh dependency.
CREATE VIEW test262_reporting.v_variant_status WITH (security_barrier=true) AS
WITH eligible AS (
 SELECT o.* FROM test262.observations o JOIN test262.runs r ON r.run_id=o.run_id
 WHERE r.trust_class='trusted' AND NOT EXISTS(SELECT 1 FROM test262.observation_invalidations i WHERE i.observation_id=o.observation_id)
), grouped AS (
 SELECT repository_id,provenance_id,fixture_id,variant,count(*) AS attempt_count,
 count(DISTINCT jsonb_build_array(outcome,phase,observed_error_type))>1 AS conflict_flag,
 max(received_at) AS last_received_at
 FROM eligible GROUP BY repository_id,provenance_id,fixture_id,variant
), ranked AS (
 SELECT e.*,row_number() OVER(PARTITION BY repository_id,provenance_id,fixture_id,variant ORDER BY received_at DESC,observation_id DESC) AS rn FROM eligible e
)
SELECT r.repository_id,r.provenance_id,r.fixture_id,r.variant,r.observation_id AS representative_observation_id,
r.outcome,r.phase,r.observed_error_type,r.failure_class,g.attempt_count,g.conflict_flag,g.last_received_at,1 AS policy_version
FROM ranked r JOIN grouped g USING(repository_id,provenance_id,fixture_id,variant) WHERE r.rn=1;

CREATE VIEW test262_reporting.v_current_fixture_status WITH (security_barrier=true) AS
SELECT t.repository_id,t.channel,t.evidence_kind,t.provenance_id,t.registration_snapshot_id,t.validation_id,
p.corpus_id,f.fixture_id,f.upstream_path,f.feature_tags,e.eligibility,e.reason_codes,
a.required_variant_count,a.recorded_variant_count,a.pass_variant_count,a.conflict_flag,a.last_received_at,
CASE WHEN f.is_support_file THEN 'excluded'
 WHEN f.metadata_state<>'valid' THEN 'metadata-error'
 WHEN e.eligibility IS NULL OR e.eligibility='unresolved' THEN 'incomplete'
 WHEN e.eligibility<>'runnable' THEN 'excluded'
 WHEN a.conflict_flag THEN 'conflicted'
 WHEN a.required_variant_count=0 OR a.recorded_variant_count<a.required_variant_count THEN 'incomplete'
 WHEN a.pass_variant_count=a.required_variant_count THEN 'complete-pass'
 WHEN a.failure_count>0 THEN 'fail' ELSE 'incomplete' END AS status,
EXISTS(SELECT 1 FROM test262.registrations reg WHERE reg.snapshot_id=t.registration_snapshot_id AND reg.upstream_path=f.upstream_path) AS source_registered,
t.updated_at AS target_updated_at,statement_timestamp() AS as_of
FROM test262.reporting_targets t JOIN test262.provenances p ON p.provenance_id=t.provenance_id
JOIN test262.fixtures f ON f.corpus_id=p.corpus_id
LEFT JOIN test262.provenance_fixture_eligibility e ON e.provenance_id=p.provenance_id AND e.fixture_id=f.fixture_id
CROSS JOIN LATERAL (
 SELECT count(*) AS required_variant_count,count(s.variant) AS recorded_variant_count,
 count(*) FILTER(WHERE s.outcome='pass') AS pass_variant_count,
 count(*) FILTER(WHERE s.outcome IN ('fail','unsupported')) AS failure_count,
 coalesce(bool_or(s.conflict_flag),false) AS conflict_flag,max(s.last_received_at) AS last_received_at
 FROM test262.fixture_variants v LEFT JOIN test262_reporting.v_variant_status s
 ON s.repository_id=t.repository_id AND s.provenance_id=p.provenance_id AND s.fixture_id=v.fixture_id AND s.variant=v.variant
 WHERE v.fixture_id=f.fixture_id AND v.required
) a;

CREATE VIEW test262_reporting.v_historical_candidates WITH (security_barrier=true) AS
SELECT p.repository_id,p.provenance_id,p.corpus_id,f.fixture_id,f.upstream_path,
max(s.last_received_at) AS last_received_at,count(*) AS required_variant_count,'historical-mvp-lead'::text AS evidence_status
FROM test262.provenances p JOIN test262.fixtures f ON f.corpus_id=p.corpus_id
JOIN test262.fixture_variants v ON v.fixture_id=f.fixture_id AND v.required
LEFT JOIN test262_reporting.v_variant_status s ON s.repository_id=p.repository_id AND s.provenance_id=p.provenance_id AND s.fixture_id=f.fixture_id AND s.variant=v.variant
WHERE p.evidence_kind='mvp-composite' AND NOT f.is_support_file AND f.metadata_state='valid'
GROUP BY p.repository_id,p.provenance_id,p.corpus_id,f.fixture_id,f.upstream_path
HAVING count(*)=count(s.variant) AND bool_and(s.outcome='pass' AND NOT s.conflict_flag)
AND NOT EXISTS(SELECT 1 FROM test262.reporting_targets t WHERE t.repository_id=p.repository_id AND t.provenance_id=p.provenance_id AND t.evidence_kind='mvp-composite');

CREATE VIEW test262_reporting.v_native_acceptance WITH (security_barrier=true) AS
SELECT b.repository_id,b.batch_id,b.batch_key,b.provenance_id,b.state AS batch_state,
v.target_revision,b.registration_snapshot_id,bf.fixture_id,f.upstream_path,
count(be.variant) AS bound_variant_count,
EXISTS(SELECT 1 FROM test262.reporting_targets t WHERE t.repository_id=b.repository_id AND t.evidence_kind='native' AND t.provenance_id=b.provenance_id AND t.validation_id=b.validation_id AND t.registration_snapshot_id=b.registration_snapshot_id) AS target_current,
bool_or(i.observation_id IS NOT NULL) AS has_invalidated_evidence,
EXISTS(SELECT 1 FROM test262.observations x JOIN test262.runs r ON r.run_id=x.run_id WHERE x.provenance_id=b.provenance_id AND x.fixture_id=bf.fixture_id AND x.outcome<>'pass' AND r.trust_class='trusted' AND NOT EXISTS(SELECT 1 FROM test262.observation_invalidations z WHERE z.observation_id=x.observation_id)) AS has_conflicting_evidence,
statement_timestamp() AS as_of
FROM test262.native_batches b JOIN test262.validation_events v ON v.validation_id=b.validation_id
JOIN test262.batch_fixtures bf ON bf.batch_id=b.batch_id AND bf.state='accepted'
JOIN test262.fixtures f ON f.fixture_id=bf.fixture_id
LEFT JOIN test262.batch_evidence be ON be.batch_id=b.batch_id AND be.fixture_id=bf.fixture_id
LEFT JOIN test262.observation_invalidations i ON i.observation_id=be.observation_id
WHERE b.state IN ('sealed','awaiting-refresh','conflicted')
GROUP BY b.repository_id,b.batch_id,b.batch_key,b.provenance_id,b.state,v.target_revision,b.registration_snapshot_id,bf.fixture_id,f.upstream_path;

CREATE VIEW test262_reporting.v_queue_health WITH (security_barrier=true) AS
SELECT repository_id,provenance_id,coherent_area,state,count(*) AS work_count,
count(*) FILTER(WHERE state='leased' AND lease_expires_at<statement_timestamp()) AS expired_lease_count,
min(eligible_after) AS oldest_eligible_at,statement_timestamp() AS as_of
FROM test262.work_items GROUP BY repository_id,provenance_id,coherent_area,state;

CREATE VIEW test262_reporting.v_failure_clusters WITH (security_barrier=true) AS
SELECT o.repository_id,o.provenance_id,p.evidence_kind,o.failure_class,o.phase,o.observed_error_type,
count(*) AS attempt_count,count(DISTINCT o.fixture_id) AS fixture_count,
min(f.upstream_path) AS representative_path,max(o.received_at) AS last_received_at
FROM test262.observations o JOIN test262.runs r ON r.run_id=o.run_id
JOIN test262.provenances p ON p.provenance_id=o.provenance_id JOIN test262.fixtures f ON f.fixture_id=o.fixture_id
WHERE o.outcome<>'pass' AND r.trust_class='trusted' AND NOT EXISTS(SELECT 1 FROM test262.observation_invalidations i WHERE i.observation_id=o.observation_id)
GROUP BY o.repository_id,o.provenance_id,p.evidence_kind,o.failure_class,o.phase,o.observed_error_type;

CREATE VIEW test262_reporting.v_pipeline_metrics WITH (security_barrier=true) AS
SELECT m.repository_id,m.run_id,r.provenance_id,r.run_kind,r.state,m.stage,m.duration_ms,m.attempt_count,m.timeout_count,m.measurement_version,r.started_at,r.ended_at
FROM test262.run_metrics m JOIN test262.runs r ON r.run_id=m.run_id;

CREATE VIEW test262_reporting.v_ingestion_health WITH (security_barrier=true) AS
SELECT p.repository_id,p.producer_id,p.kind,p.display_name,p.enabled,p.last_seen_at,
(SELECT count(*) FROM test262.ingest_requests i WHERE i.producer_id=p.producer_id AND i.state='pending') AS pending_requests,
(SELECT max(committed_at) FROM test262.ingest_requests i WHERE i.producer_id=p.producer_id) AS last_commit_at,
statement_timestamp() AS as_of FROM test262.producers p;

CREATE VIEW test262_reporting.v_storage_growth WITH (security_barrier=true) AS
SELECT n.nspname AS schema_name,c.relname AS table_name,pg_total_relation_size(c.oid) AS total_bytes,statement_timestamp() AS as_of
FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace WHERE n.nspname='test262' AND c.relkind='r';

DO $$
DECLARE item record;
BEGIN
 FOR item IN SELECT tablename FROM pg_tables WHERE schemaname='test262' LOOP
   EXECUTE format('ALTER TABLE test262.%I ENABLE ROW LEVEL SECURITY',item.tablename);
 END LOOP;
END $$;
REVOKE ALL ON ALL TABLES IN SCHEMA test262 FROM PUBLIC,anon,authenticated,service_role;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA test262 FROM PUBLIC,anon,authenticated,service_role;
REVOKE EXECUTE ON ALL FUNCTIONS IN SCHEMA test262 FROM PUBLIC,anon,authenticated,service_role,test262_ingest,test262_coordinator,test262_reporter;
REVOKE ALL ON ALL TABLES IN SCHEMA test262_reporting FROM PUBLIC,anon,authenticated,service_role;
GRANT USAGE ON SCHEMA test262_reporting TO test262_reporter;
GRANT SELECT ON ALL TABLES IN SCHEMA test262_reporting TO test262_reporter;
-- No worker/coordinator table grants or login memberships until the authenticated API is implemented.
COMMENT ON SCHEMA test262 IS 'Issue #2230: durable Test262 catalogue schema v1. Schema-only; no workflow cutover.';
COMMENT ON SCHEMA test262_reporting IS 'Read-only, owner-backed reporting views for the Test262 catalogue. Grant membership separately.';
$schema_sql$;
 BEGIN
  EXECUTE $schema_tests$
DO $test$
DECLARE rid uuid; other_rid uuid; cid uuid; fid uuid; pid uuid; producer uuid; runid uuid; obs1 uuid=gen_random_uuid(); obs2 uuid=gen_random_uuid(); snap uuid; val uuid; budget uuid; batch uuid; work uuid;
 h bytea=decode(repeat('ab',32),'hex'); rev text=repeat('a',40);
BEGIN
 INSERT INTO test262.repositories(provider,provider_repository_id,canonical_name) VALUES('test','schema-validation','schema-validation') RETURNING repository_id INTO rid;
 INSERT INTO test262.repositories(provider,provider_repository_id,canonical_name) VALUES('test','other-validation','other-validation') RETURNING repository_id INTO other_rid;
 INSERT INTO test262.corpora(upstream_url,revision,revision_algorithm,inventory_digest,expected_fixture_count) VALUES('test://schema',rev,'sha1',h,1) RETURNING corpus_id INTO cid;
 INSERT INTO test262.repository_corpora VALUES(rid,cid,clock_timestamp());
 INSERT INTO test262.fixtures(corpus_id,upstream_path,content_sha256,metadata_state) VALUES(cid,'test/language/example.js',h,'valid') RETURNING fixture_id INTO fid;
 INSERT INTO test262.fixture_variants(fixture_id,variant) VALUES(fid,'strict'),(fid,'non-strict');
 UPDATE test262.corpora SET inventory_state='sealed' WHERE corpus_id=cid;
 BEGIN
  INSERT INTO test262.fixtures(corpus_id,upstream_path,content_sha256,metadata_state) VALUES(cid,'test/illegal.js',h,'valid');
  RAISE EXCEPTION 'test failed: sealed corpus allowed write';
 EXCEPTION WHEN check_violation THEN NULL; END;
 BEGIN
  PERFORM decode('aa','hex')::test262.sha256;
  RAISE EXCEPTION 'test failed: invalid SHA allowed';
 EXCEPTION WHEN check_violation THEN NULL; END;
 BEGIN
  PERFORM '../escape'::test262.relative_path;
  RAISE EXCEPTION 'test failed: invalid path allowed';
 EXCEPTION WHEN check_violation THEN NULL; END;
 INSERT INTO test262.provenances(repository_id,corpus_id,evidence_kind,identity_version,identity_sha256,identity_document,capability_sha256) VALUES(rid,cid,'native',1,h,'{}',h) RETURNING provenance_id INTO pid;
 INSERT INTO test262.provenance_fixture_eligibility(repository_id,provenance_id,fixture_id,eligibility) VALUES(rid,pid,fid,'runnable');
 INSERT INTO test262.producers(repository_id,kind,display_name,credential_subject) VALUES(rid,'local','test','test') RETURNING producer_id INTO producer;
 INSERT INTO test262.runs(repository_id,producer_id,provenance_id,external_run_key,source_revision,run_kind,trust_class) VALUES(rid,producer,pid,'test',rev,'native','trusted') RETURNING run_id INTO runid;
 BEGIN
  INSERT INTO test262.runs(repository_id,producer_id,provenance_id,external_run_key,source_revision,run_kind,trust_class) VALUES(other_rid,producer,pid,'cross-repo',rev,'native','trusted');
  RAISE EXCEPTION 'test failed: cross-repository reference allowed';
 EXCEPTION WHEN foreign_key_violation THEN NULL; END;
 INSERT INTO test262.observations(observation_id,repository_id,run_id,provenance_id,fixture_id,variant,outcome,phase,started_at,finished_at,active_ms,payload_sha256,source_kind)
 VALUES(obs1,rid,runid,pid,fid,'strict','pass','runtime',clock_timestamp(),clock_timestamp(),1,h,'live');
 BEGIN
  UPDATE test262.observations SET diagnostic_summary='changed' WHERE observation_id=obs1;
  RAISE EXCEPTION 'test failed: observation updated';
 EXCEPTION WHEN check_violation THEN NULL; END;
 INSERT INTO test262.registration_snapshots(repository_id,source_revision,inventory_sha256) VALUES(rid,rev,h) RETURNING snapshot_id INTO snap;
 UPDATE test262.registration_snapshots SET state='sealed' WHERE snapshot_id=snap;
 INSERT INTO test262.validation_events(repository_id,workflow_identity,external_run_key,target_revision,conclusion,proof) VALUES(rid,'test','test',rev,'success','{}') RETURNING validation_id INTO val;
 INSERT INTO test262.budget_scopes(repository_id,scope_kind,external_key,candidate_limit,accepted_limit,attempt_limit,time_limit_ms) VALUES(rid,'batch','test',2,1,4,10000) RETURNING budget_scope_id INTO budget;
 BEGIN
  UPDATE test262.budget_scopes SET reserved_attempts=5 WHERE budget_scope_id=budget;
  RAISE EXCEPTION 'test failed: over budget accepted';
 EXCEPTION WHEN check_violation THEN NULL; END;
 INSERT INTO test262.native_batches(repository_id,batch_key,provenance_id,validation_id,registration_snapshot_id,budget_scope_id) VALUES(rid,'test',pid,val,snap,budget) RETURNING batch_id INTO batch;
 INSERT INTO test262.batch_fixtures VALUES(batch,fid,'accepted');
 INSERT INTO test262.batch_evidence VALUES(batch,fid,'strict',obs1);
 BEGIN
  UPDATE test262.native_batches SET state='sealed' WHERE batch_id=batch;
  RAISE EXCEPTION 'test failed: missing variant sealed';
 EXCEPTION WHEN check_violation THEN NULL; END;
 INSERT INTO test262.observations(observation_id,repository_id,run_id,provenance_id,fixture_id,variant,outcome,phase,started_at,finished_at,active_ms,payload_sha256,source_kind)
 VALUES(obs2,rid,runid,pid,fid,'non-strict','pass','runtime',clock_timestamp(),clock_timestamp(),1,h,'live');
 INSERT INTO test262.batch_evidence VALUES(batch,fid,'non-strict',obs2);
 UPDATE test262.native_batches SET state='sealed' WHERE batch_id=batch;
 BEGIN
  DELETE FROM test262.batch_evidence WHERE batch_id=batch;
  RAISE EXCEPTION 'test failed: sealed batch changed';
 EXCEPTION WHEN check_violation THEN NULL; END;
 INSERT INTO test262.reporting_targets(repository_id,channel,evidence_kind,provenance_id,registration_snapshot_id,validation_id) VALUES(rid,'master','native',pid,snap,val);
 IF NOT EXISTS(SELECT 1 FROM test262_reporting.v_current_fixture_status WHERE repository_id=rid AND status='complete-pass' AND required_variant_count=2) THEN RAISE EXCEPTION 'test failed: reporting'; END IF;
 IF (SELECT count(*) FROM test262_reporting.v_native_acceptance WHERE batch_id=batch)<>1 THEN RAISE EXCEPTION 'test failed: acceptance reporting'; END IF;
 IF has_table_privilege('test262_ingest','test262.observations','INSERT') OR has_table_privilege('test262_reporter','test262.observations','SELECT') OR has_schema_privilege('anon','test262','USAGE') THEN RAISE EXCEPTION 'test failed: privilege isolation'; END IF;
 IF NOT has_table_privilege('test262_reporter','test262_reporting.v_current_fixture_status','SELECT') THEN RAISE EXCEPTION 'test failed: reporting privilege'; END IF;
END $test$;
$schema_tests$;
  RAISE EXCEPTION USING ERRCODE='Z0001',MESSAGE='validation passed; remove only test rows';
 EXCEPTION WHEN SQLSTATE 'Z0001' THEN NULL;
 END;
END $deployment$;
