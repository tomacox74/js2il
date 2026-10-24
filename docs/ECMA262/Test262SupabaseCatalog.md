# Central Test262 catalogue

Supabase owns evidence, work leases, budgets, registration snapshots, reporting targets,
reconciliation cursors and publication reservations. Fixture bytes and generated native
registrations remain in Git. SQLite is a disposable generation cache or a durable upload
outbox. Copying a checkout or restoring an Actions artifact never replaces central state.

## Implementation and deployment boundary

Issue #2230 tracks this transition. The exact already-deployed foundation migration is
`supabase/migrations/20261003211043_test262_catalogue_schema_v1.sql`. **Do not replay that
migration on the existing project:** its original migration version is already recorded.
The subsequent `20261004000100_test262_catalogue_api_v1.sql` migration adds the scoped API,
login bindings, legacy control-record preservation and explicit budget enrollment.
It does not activate authority, create passwords, grant an existing performance login
new privileges, import history, or touch `public.perf_results`.

Production cutover was verified on 2026-10-08: both migrations are deployed,
authority is `active`, and minimum writer epoch is 2. The core path includes live
MVP ingestion/completion, GitHub App native publication with independent PR CI,
merged-master CI and exact-source registration refresh; see
[the activation record](https://github.com/tomacox74/js2il/issues/2230#issuecomment-6052583593)
and [post-merge verification](https://github.com/tomacox74/js2il/issues/2230#issuecomment-6058883917).
Verify the live scoped API contract and selected target before each authorized run;
do not hardcode a saved epoch as future authority or replay cutover during porting.

Historical import parity covers 62 recoverable entries; 11 unavailable archive gaps
keep `history_complete=false`. Complete long-term artifact retention and client
run/budget/publication lifecycle bookkeeping remain follow-ups in #2230/#2242.
The repository CI applies both migrations to empty PostgreSQL 17 and tests
concurrent claims/budgets, privilege isolation and native evidence invariants.

For coding agents, use the catalogue-first rules in
[`test262-porting`](../../.github/skills/test262-porting/SKILL.md). Read-only
reporting or an approved coherent export guides selection; local branch tests
remain PR validation until separately recorded through the reviewed central path.
An agent without central access must disclose offline scope, not fall back to
SQLite authority. The central workflow checks out master; after merging a manual
port/fix, verify exact-SHA master CI and a subsequent central registration refresh.

## Upstream specification references

New inventories retain upstream `esid`, legacy `es5id`/`es6id`, `description`,
`info`, and `author` in fixture metadata. The versioned `specification` object
also retains the parsed identifier values without trimming or translating them,
and the exact frontmatter text, including its original line endings. The fixture
content SHA-256 identifies the original source bytes; the corpus records the
Test262 upstream URL and exact revision. No identifier is inferred from a filename.

Run `npm ci --ignore-scripts` from the repository root before local catalogue or
enrichment commands. Reference extraction uses the lockfile-pinned `yaml` reader
with YAML 1.2 core scalar semantics: unquoted `null`/`~`, numbers and booleans keep
their parsed types, while `"null"`, `"19.1"` and `"false"` remain strings. Explicit
null identifiers remain present in `raw_identifiers`, distinct from an absent
field. This reader is separate from the existing runner metadata parser and does
not change execution eligibility, variants, flags or harness selection.

YAML errors, unsupported tags/aliases, non-finite numbers and integers outside the
lossless JSON number range produce an `unresolved` reference with a diagnostic
and the original frontmatter, not a manufactured identifier or silent JSON null.
The exact source text and content digest remain available for interpretation.

Inventory and enrichment metadata hashes use PostgreSQL-compatible numeric JSON
formatting, expanding exponents (for example `1e-7` to `0.0000001`) without changing
parsed values or quoted numeric strings. General-purpose identity serialization
remains unchanged; already accepted inventory/annotation hashes are not rewritten.

The additive migration
`20261021230000_test262_catalogue_specification_metadata.sql` exposes
`test262_reporting.v_fixture_specification_references` to `test262_reporter`.
Deploy this migration before using the updated central exporter or enrichment
command. It does not grant public Data API access, activate authority, change
reporting targets, or touch performance data. This document describes the
supported migration/tooling, not a claim that production has already been enriched.
The backup operator must include the new table in the existing catalogue-only
SELECT/backup allowlist, without broadening access to performance or public data.

The view has one row per repository/corpus/fixture. Join it to
`v_current_fixture_status` or observation exports by **repository ID and fixture ID**,
not just the upstream path: a path can have different source bytes or references
at different pins. Filter by repository and corpus/fixture and bound the result,
for example:

```sql
SELECT fixture_id, upstream_revision, upstream_path, content_sha256,
       raw_identifiers, description, reference_state, metadata_source,
       mapping_status, specification_edition
FROM test262_reporting.v_fixture_specification_references
WHERE repository_id = '<repository-uuid>'::uuid
  AND corpus_id = '<corpus-uuid>'::uuid
  AND inventory_state = 'sealed'
ORDER BY upstream_path
LIMIT 100;
```

`raw_identifiers` retains the upstream field names and parsed values. The
`esid`/`es5id`/`es6id` columns are JSON values so malformed non-string values
are not silently coerced. Reference states describe the source, not conformance:

| `reference_state` | Meaning |
| --- | --- |
| `absent` | Parsed frontmatter declares none of the three identifier fields. |
| `legacy` | A single nonempty legacy `es5id` or `es6id` is present. |
| `unresolved` | A modern `esid` has not been resolved to an edition, or source metadata is unavailable/unparseable. |
| `ambiguous` | Multiple/repeated identifier fields, an empty identifier, or a non-string identifier need interpretation. |

Multiple fields are preserved, not assumed equivalent. Unparseable metadata has
a diagnostic and any extractable raw frontmatter. Older stripped inventories have
`metadata_source='unavailable'` and `reference_state='unresolved'`, **not** `absent`.
The view explicitly returns `mapping_status='not-attempted'`, a null
`specification_edition`, and a null `interpreted_clause`. Even a `sec-*` anchor
is only an upstream reference; this surface neither resolves it against today's
ECMA-262 edition nor establishes conformance to an entire clause.

### Enriching existing sealed corpora

Use the existing dedicated **trusted coordinator** login and current epoch only
in the trusted supervisor process. Given a clean upstream checkout at the original
corpus revision, an authorized operator can run:

```bash
python -m scripts.test262.central.cli \
  --repository '<repository-uuid>' --producer '<coordinator-producer-uuid>' \
  enrich-specification-metadata --corpus '<corpus-uuid>' --root /path/to/test262
```

The command performs no fixture execution, work allocation, observation ingestion,
or publication. It reads the selected corpus in bounded pages, extracts metadata
in batches of at most 250, verifies the checkout revision and each original
fixture path/content SHA-256, then appends one immutable
`fixture_specification_metadata` record per fixture. SQL rechecks the sealed
source identity and canonical metadata digest, records the actor/time, and
rejects cross-repository, untrusted, conflicting, or mutable-inventory writes.
Sealed fixture metadata, inventory digests, fixture IDs, observations and payloads
are unchanged. Repeating identical enrichment is idempotent; an interrupted
multi-batch operation can be retried, while conflicting extraction requires an
explicit reviewed migration rather than overwriting the previous annotation.
Sparse/missing files or mismatched bytes fail explicitly.

#### Compatibility with earlier scalar extraction

The #2336 correction does not rewrite sealed inventory metadata or annotations.
Both extractors use the same version-1 metadata shape; the schema version alone
does not establish which extractor produced a record. Freeze and record the
tooling source revision and canonical metadata digest for any backfill.

Before #2338, compare existing annotations with a fresh, source-verified extraction
at each original corpus revision. Earlier extraction could store plain YAML
`null`, numeric or boolean identifiers as strings and report a false `legacy`
state; identically spelled quoted strings are legitimate and must not be
blanket-converted. If no annotation exists, corrected enrichment can attach
metadata to an older inventory without changing its original fields. If an
incompatible annotation already exists, the immutable API rejects corrected
replay with `Conflicting immutable replay`; the existing annotation, digest,
actor/time and historical evidence remain unchanged. Stop the affected backfill,
retain the conflict and before/after extraction fingerprints, and require an
explicitly reviewed append-only supersession/migration plan before proceeding.
No annotation overwrite, deletion, automatic repair or supersession schema is
introduced by this fix.

New ingestion includes references in the normal inventory identity, so re-ingesting
the same source with richer metadata creates a new corpus rather than rewriting
the old one. Enrichment is the separate path for attaching references to existing
evidence. The coherent NDJSON export includes original `fixtures.metadata`,
`corpora`, and the additive `fixture_specification_metadata` records with source
revision/path/content digest, metadata digest, and actor/time. Consumers can retain
the original evidence and distinguish inventory metadata from later annotation.

## Local agent read access

### Registered MCP access

A user-scoped Copilot CLI MCP server named `test262-catalog` is registered in the
configured agent environment. It connects to the catalogue project
`nluxctxxzchguuzaalxn` through the official Supabase hosted MCP endpoint:

```text
https://mcp.supabase.com/mcp?project_ref=nluxctxxzchguuzaalxn&read_only=true&features=database
```

The enabled tools are `execute_sql` and `list_tables`. Keep the project scope,
read-only mode and tool allowlist; restrict catalogue queries to approved
`test262_reporting` views and their schema metadata, not raw tables or
coordinator/publication APIs.

**Verified on 2026-10-15 UTC (2026-10-14 PDT):** authenticated MCP SQL access
successfully inspected the columns of `test262_reporting.v_current_fixture_status`
and read a bounded row filtered to this repository, `master`, native evidence and
the await expression area. This verifies reporting access, not complete coverage,
fresh native acceptance, registration refresh or publication.

When the server's tools are available, use this authenticated read-only connection
before falling back to the Data API or declaring central reporting unavailable.
Inspect actual view columns first. Scope evidence queries to repository
`faf01df8-aa65-4375-a708-6c1e555f957b`, the corpus matching the pinned revision,
channel, evidence kind and requested area; record the returned as-of,
provenance, registration and validation identities. Apply the same reconciliation
and evidence-freshness rules as other central reporting reads.

MCP SQL access is independent of PostgREST schema exposure and `anon` grants:
a REST `Invalid schema: test262_reporting` error does not mean MCP reporting is
unavailable. This connection does not change those Data API permissions.

Server registration is user-scoped (`~/.copilot/mcp-config.json`), and
authentication is local to that profile as well; neither is automatically shared
with other machines. Other environments must register and authenticate the server
separately. Use `/mcp` to check connection/authentication, and restart the CLI if
the tools are not visible to an existing session. Keep OAuth credentials or a
scoped Supabase personal access token outside Git, prompts and logs. Never
substitute a publishable key, service-role key or supervisor credential for MCP
authentication, or expose query credentials to upstream fixture processes.

### Data API alternative

Use the existing JROC performance-analysis environment variables in each local
checkout for Supabase Data API reads:

```bash
export SUPABASE_URL="https://<project>.supabase.co"
export SUPABASE_PUBLISHABLE_KEY="<publishable-key>"
```

These are the same project URL and publishable key used to read `public.perf_results`;
do not introduce a second catalogue key or use a PostgreSQL connection string as
`SUPABASE_URL`. Neither Bash nor Copilot automatically loads an ignored `.env` file:
load it explicitly in a trusted shell before starting the agent. Check that the
variables are set without printing their values. Never commit them, enable shell
tracing around requests, or put them in PRs, logs or generated reports.

### Permissions are a prerequisite

A publishable key identifies the application; it does not grant database access.
With no signed-in user JWT, Data API requests use the `anon` role. Catalogue REST
reads work only when an operator has configured a reviewed read-only API surface:
the schema must be exposed to PostgREST and the request role must have the required
schema/view permissions. A client-side repository filter is not an authorization
boundary. Review which repositories and columns a public key would expose before
enabling that surface; never grant catalogue table writes or coordinator API access.

**Verified on 2026-10-09 UTC (2026-10-08 PDT):** the deployed `anon` role has neither
`USAGE` on `test262_reporting` nor `SELECT` on its reporting views. The checked-in
foundation and summary migrations also revoke those rights from `anon` and
`authenticated`. Consequently, performance reads succeeding with these variables
does **not yet** prove catalogue REST access. This documentation change does not
alter database grants or API schema exposure. Use the authorized read-only JROC
Supabase connector, a dedicated reporter connection, or an approved coherent export
until an operator enables and verifies the read-only Data API surface. Do not ask
for a service-role key, supervisor DSN or publication credential to bypass a denial.

### Bounded REST lookup after access is enabled

For the existing reporting-view surface, select the custom schema with
`Accept-Profile: test262_reporting`; the endpoint uses the unqualified view name.
Send the publishable key in `apikey` only. A modern `sb_publishable_...` key is not
a JWT and must not be sent as `Authorization: Bearer`. See
[Supabase API key guidance](https://supabase.com/docs/guides/getting-started/api-keys).
If an approved surface requires a signed-in user, its separate user JWT and role
permissions are required; the two environment variables alone do not supply them.

This Bash example reads at most 100 current native fixture rows for this repository
and one area; it is a selection sample, not a complete export:

```bash
(
  set -euo pipefail
  : "${SUPABASE_URL:?Set SUPABASE_URL in the trusted shell}"
  : "${SUPABASE_PUBLISHABLE_KEY:?Set SUPABASE_PUBLISHABLE_KEY in the trusted shell}"
  curl --fail --silent --show-error --get \
    "${SUPABASE_URL%/}/rest/v1/v_current_fixture_status" \
    -H "apikey: $SUPABASE_PUBLISHABLE_KEY" \
    -H "Accept-Profile: test262_reporting" \
    --data-urlencode 'repository_id=eq.faf01df8-aa65-4375-a708-6c1e555f957b' \
    --data-urlencode 'channel=eq.master' \
    --data-urlencode 'evidence_kind=eq.native' \
    --data-urlencode 'upstream_path=like.test/built-ins/Math/abs/*' \
    --data-urlencode 'select=upstream_path,status,source_registered,required_variant_count,recorded_variant_count,pass_variant_count,conflict_flag,provenance_id,corpus_id,registration_snapshot_id,validation_id,target_updated_at,as_of' \
    --data-urlencode 'order=upstream_path.asc,fixture_id.asc' \
    --data-urlencode 'limit=100' |
    jq .
)
```

Inspect actual view columns before changing the query. Use the same schema header
for `v_historical_candidates`, `v_native_acceptance` and `v_failure_clusters`, with
repository/provenance/area filters appropriate to each view. Keep pins, evidence
kinds and selected target identities separate; historical or MVP passes are leads,
not fresh native acceptance. An empty result is not proof of full coverage or absent
registrations. A 401/403, unavailable schema/view or failed query is an access
limitation, not a catalogue outcome; use the connector/export or explicit offline
diagnosis rules and report the limitation. Do not change grants from an agent task.

### Concurrent readers and local validation

Independent agents may read the catalogue concurrently from separate checkouts;
GET/reporting SELECT requests do not claim work, spend screening budgets, change
authority or reserve publications. Keep requests bounded. Each REST request is a
separate snapshot: record `as_of` and target/provenance/registration identities,
recheck before opening a PR, and use an approved coherent export for multi-page
completeness claims. Reconcile selected master evidence with the checkout's pinned
corpus, compiler revision, local registrations and pending PRs. Local branch tests
remain PR validation and do not upload trusted central evidence.

Keep database access in the agent's query process. Execute upstream fixtures in a
separate process/container with Supabase and other credential variables removed.
Reading during other agents' work is supported; publication has a separate freshness
guard and can reject a sealed batch if `master` advances during its run.

## Restricted identities and process boundary

Provision dedicated PostgreSQL LOGINs through the database/secret-management operator.
Use the same scoped database user through a direct or session-pooled TLS connection;
`session_user` must be that dedicated login. Do not connect as `postgres`, `service_role`,
or a shared pool identity, and do not accept client-supplied trust claims.

`central/provision.sql` binds the login to one immutable repository ID, producer ID,
permission and server-assigned trust class. Provision separately:

| Identity | Membership / binding | Use |
| --- | --- | --- |
| Trusted supervisor | `test262_coordinator`, coordinator/trusted | Reviewed master inventory, scheduling, target verification, native batches and publication |
| Historical importer | `test262_coordinator`, coordinator/legacy | Snapshot import while in shadow mode; cannot establish native acceptance |
| Optional untrusted collector | `test262_ingest`, ingest/untrusted | Append its own results; no claims, trusted runs, targets or acceptance |
| New Grafana login | `test262_reporter` only | Reporting views; no raw table reads, API execution or writes |
| Backup login | Explicit catalogue-only SELECT plus required schema/type/function metadata visibility | Catalogue dump; no performance table rights |

NOLOGIN group roles grant function execution, never worker table writes. Every API call
checks the actual login binding and writer epoch. A supplied producer/repository in a
CLI context must agree with the authenticated binding. The provision template does not
create or print a secret. Disable a compromised producer and its binding server-side.

Fixture execution occurs in a new Docker container per variant. It has no network,
host PID namespace, capabilities, secret environment or credential/outbox mounts. The
checkout itself is never mounted (it holds `.git` credentials and the outbox): a staged
copy of only `scripts/test262/*.js`, `tests/test262/*.json` and the entry binary's output
directory is mounted read-only at `/repo`, with the pinned source read-only at `/upstream`;
one disposable work directory is writable.
Allowlisting child environment alone would leave a same-user `/proc` credential risk.
The Python supervisor alone connects to PostgreSQL. Build the fixture image before
supplying supervisor secrets. The C# host also strips inherited credentials from its
worker environment for callers outside this pipeline.

## API and queue contract

| Operation | Contract |
| --- | --- |
| `api_contract` | Advertises version/epoch/state and authenticated repository/producer binding |
| `api_put` | Coordinator-only bounded immutable insert/replay; supplied fields must agree, exact primary key required |
| `api_start_run` | Immutable source/provenance/external-run identity; trust and producer assigned by server |
| `api_ingest` | At most 250 records / 4 MiB per atomic chunk; request UUID and full payload digest; observation UUID dedup across chunks |
| `api_claim` | Active authority only; pending eligible work enrolled in this budget; fenced generation, server TTL, atomic reservation |
| `api_renew` | Owner + generation + unexpired lease; no resurrection |
| `api_complete` | Requires the observation's exact lease run/generation; repeat is safe; late evidence never completes a newer generation |
| `api_metric` | Stage measurement version/CAS; retries do not increment totals |
| `api_transition` | Exact primary key + version/CAS; allowlisted mutable fields; inventory digest and fresh native seal checks; never changes `authority_epoch` (only the audited `cutover.sql`) |
| `api_reconcile` | Verified plan digest including next cursor, cursor CAS and durable queue insertion in one transaction; recovery identity differs from SHA |
| `api_read` | One allowlisted table, repository-scoped, exact-column equality filters, primary-key keyset pages of at most 1000 rows (25 for payloads); callers needing a coherent multi-page view read inside one REPEATABLE READ READ ONLY transaction |

Lock order is repository advisory lock, queue rows, budget rows. The repository lock
serializes short catalogue transactions (including seal/ingest), while fixtures execute
in parallel outside transactions. Queue rows use `FOR UPDATE SKIP LOCKED`; the additional
lock intentionally favors correctness over maximal ingestion throughput for the pilot.

Claims reserve one attempt and its maximum execution time. TTL must include 30 seconds
of settlement grace beyond the execution cap. Expired claims are conservatively charged
at their full reservation, marked uncertain, and retried with a new generation. An
infrastructure/incomplete result releases ownership and returns work to pending with
backoff, up to the bounded lease-attempt limit; thereafter it is deferred. Terminal
product/harness failures remain evidence and are revisited under a new relevant binary,
harness, capability or environment provenance. Incomplete variants never become passes.
A dead worker cannot refund budget, silently complete work, or overwrite another result.

Outbox writes use SQLite WAL + `synchronous=FULL`. Store a result before upload; mark it
acknowledged only after its server receipt. Completion has a separate durable outbox
entry. A commit whose response was lost is replayed under the same UUID/payload. A new
actual execution gets a new observation UUID. Outboxes are bound to repository, producer
and epoch; preserve failed-run outboxes until acknowledged. Do not change their scope by
copying an enlistment or edit UUIDs to bypass a conflicting-payload rejection.

Native acceptance needs every required variant, one native provenance, trusted live
results, valid metadata, a verified dependency-manifest digest and a current validated
master target. MVP, legacy and untrusted observations cannot qualify. Runtime-negative
results include the type verified by the existing C# harness; compile negatives remain
harness gaps. Contradictory trusted product evidence (`fail`/`unsupported`) or invalidated
evidence blocks sealing and marks an already sealed batch conflicted; retryable
`infrastructure-error`, `incomplete` and `deferred` attempts do not. Advancing its
reporting target marks it awaiting refresh.
Publication requires a fresh sealed batch, reserves a unique branch/PR centrally, adopts
a crash-after-push only after matching parent/commit marker/exact patch, and detects head
ownership changes. Before reserving, any other active reservation without a PR number is
reconciled: an owned PR is adopted using its retained pull-request head ref, including
closed/merged PRs whose branch GitHub deleted. Recovery verifies the exact single parent
against the batch validation event, commit marker, patch digest, repository and master
base. A foreign or unverifiable branch/PR blocks the reservation for manual review.
An absent PR never proves a publisher stopped: reservations without a confirmed PR
retain their single-active-publication slot and any branch for an original-batch retry.
The reconciler does not automatically delete or cancel them. A deliberate cancellation
requires operator reconciliation after confirming the publisher has stopped. It never automatically merges or claims
required PR CI has passed.


### Native publication recovery verification

The central invariants suite exercises real commits/pushes/fetches in temporary local
Git repositories. GitHub PR responses and coordinator persistence are explicitly
simulated; no test publishes a native PR or accesses production credentials. Cases cover
a crash after branch push, PR creation followed by a lost acknowledgement, reuse of
the same PR/commit, deleted branches on closed/merged PRs, absent-PR reservation fencing,
and rejection of wrong parent, repository/base and changed head. Publication also checks
that the validation manifest binds the exact patch digest and base revision. A closed PR
retry reconciles its state before branch creation and cannot recreate its deleted branch.

Passing these regressions establishes local Git recovery behavior. A protected GitHub
App end-to-end native batch publication (fresh trusted evidence, independent focused
validation, simulated interruption, same-PR recovery and required PR CI) remains separate
operational acceptance on #2230. Neither local tests nor a green invariants workflow
activate production or make historical native snapshots fresh acceptance.

## Import history before cutover

### Manual GitHub Actions import

Timed-out MVP imports can be retried with the same discovery run and original
artifact IDs after checking database capacity. The retry revalidates the source
archive hashes and regenerates observations from the immutable snapshot; it does
not require copying an old outbox into the runner. Preserve the recovery artifact
(including SQLite WAL files) for diagnosis. Stable observation and request IDs
allow replay of partial uploads.

MVP imports reuse a repository-scoped sealed inventory only after matching its
upstream revision, normalized inventory digest and fixture count. An unfinished
inventory is replayed through the existing immutable API. Result mappings are
uploaded in batches, and each provenance's observation outbox is acknowledged
before recording an immutable completion checkpoint. A retry skips checkpointed
provenances; the final central observation/mapping parity check still runs before
the snapshot is marked verified. The manual import job allows six hours and logs
committed table/chunk counts without printing database credentials or payloads.

Create environment `test262-catalogue-import`, restricted to `master`, with secret
`TEST262_IMPORTER_DATABASE_URL` (the dedicated `test262_importer` session-pooler TLS
connection string) and variables `TEST262_REPOSITORY_ID`,
`TEST262_IMPORTER_PRODUCER_ID` and `TEST262_AUTHORITY_EPOCH`. The workflows map the
importer producer variable to `TEST262_PRODUCER_ID`; use the legacy binding, not the
trusted supervisor. Leave `TEST262_CATALOGUE_AUTHORITY` unset.

1. Run **Test262 importer connection test** on `master`. It verifies TLS, the exact
   login/repository/producer binding, coordinator/legacy trust, API version, epoch and
   inactive authority in an explicitly read-only transaction. No data is uploaded.
2. Run **Test262 historical catalogue import** on `master` with `mode=discover`.
   This job has no database credential. Download its
   `test262-history-archive-<run-id>` artifact, inspect `history-manifest.json` and
   preserve the original ZIPs in your independent archive before importing.
3. Run the same workflow with `mode=import`, `archive_run_id` equal to that successful
   discovery run, and `kind=mvp` initially. Choose original `artifact_ids` from the
   manifest; the default bound is five snapshots. An explicit selection exceeding
   the bound is rejected rather than silently truncated. Native/all imports
   materialize the complete current pinned upstream inventory; mismatched historical
   pins fail and must be handled separately, never silently substituted.
4. Download `test262-history-import-recovery-<run-id>-<attempt>` and inspect
   `import-report.json`. It records selected IDs, verified observation mappings,
   source hashes and missing history. Failed uploads retain their SQLite outboxes.
   Import reports and archive artifacts have 90-day Actions retention, which is
   recovery convenience, not the independent long-term retention policy.
5. Continue with additional explicit artifact IDs in bounded invocations. Defaults
   select the first matching snapshots in the manifest, not the next unimported
   snapshots. Replaying the same selection uses the importer's stable evidence IDs.

The workflow validates the source discovery run's repository/workflow/master identity,
checks original ZIP and SQLite hashes before writes, opens source databases read-only,
checks central observation mapping coverage and confirms source bytes remain unchanged.
It neither schedules tests nor changes authority/targets/publications. Original native
state artifacts may lack the publication proof manifest; they remain legacy evidence,
never proof of a publishable native batch. Local snapshots are not discovered by Actions;
use the CLI below to import separately archived local copies.

Inventory all available `test262-catalog`, shard and native state artifacts, using
GitHub pagination. The helper archives recoverable master-workflow databases and records
expired/missing runs in its manifest:

```bash
python -m scripts.test262.central.history --output durable/history
```

 Record expired/missing runs as limitations instead of silently
calling the history complete. Download only artifacts attributable to the intended
repository/workflow/branch; verify the native manifest before trusting its source.
Keep original downloaded archives in the independently retained encrypted archive.

Install `scripts/test262/central/requirements.txt`; set the importer's scoped DSN,
`TEST262_REPOSITORY_ID`, `TEST262_PRODUCER_ID`, and `TEST262_AUTHORITY_EPOCH` through a
secret manager. Do not include the DSN in shell arguments or print environment variables.
Run from the repository root:

```bash
python -m scripts.test262.central.cli import historical/catalog.sqlite \
  --source-uri 'github-actions:repository/workflow/run/artifact' \
  --missing-artifacts 'expired run/artifact ID' --outbox durable/import-outbox.sqlite
python -m scripts.test262.central.cli import historical/native.sqlite \
  --root "$TEST262_ROOT" --source-uri 'github-actions:repository/workflow/run/artifact' \
  --outbox durable/native-import-outbox.sqlite
```

The importer accepts MVP SQLite schema 1 and native schema 4 **read-only** and does not
upgrade originals. Its semantic source digest ignores SQLite file layout/WAL/vacuum.
Overlapping snapshots deduplicate exact attempts; distinct native run/attempt identities
retain real reruns. MVP has only its last replaceable result, so overwritten history is
explicitly unrecoverable. Native import requires the full pinned upstream inventory and
matching fixture hashes. Legacy control rows (runs, candidates, cursor, pending work,
publications, metrics and metadata) are retained in `legacy_control_records` for audit,
without taking ownership of live scheduling/publication. Missing metadata/closures and
missing artifacts are explicit limitations. Import state stays `incomplete` for history
coverage even when every recoverable row in the supplied snapshot has uploaded.

Use one designated importer identity for overlapping snapshots. Compare source counts,
observation/import-record counts, required variants, failure samples and payload hashes.
An import conflict is an investigation, not permission to silently update evidence.
Legacy native passes must be freshly verified before batch acceptance. Never turn
legacy SQLite publication/cursor fields directly into trusted active state.

### Overlapping MVP snapshots and historical run revisions

An MVP catalogue stores several provenances while `settings.compiler_commit` is replaced
at each initialization. That snapshot-wide setting cannot attribute every older provenance
to a compiler source commit. New MVP import runs therefore use the provenance's upstream
pin as `source_revision`; it is explicitly an upstream revision, not a compiler revision.
Original settings remain archived in `legacy_control_records`, and each completed import
records this attribution limitation.

Existing MVP import runs retain their exact run IDs and original immutable source revisions.
The importer verifies repository, producer, provenance, legacy trust and import kind before
replaying through the normal API. Observation IDs, request IDs and payloads remain stable,
so overlapping snapshots and pre-fix outboxes can replay without rewriting old evidence.
Mismatched scope/provenance still fails closed; native import attribution is unchanged.

The pre-cutover retry and automatic-resume incidents are recorded in #2242. Historical
imports now remain manual maintenance only: use current reviewed master scripts and the
original pinned discovery archive. A historical retry requires an explicit inactive
authority maintenance window; it must not run alongside active central workers.

## Workflow setup and authority transition

Create environment `test262-catalogue` and configure:

- Secret `TEST262_COORDINATOR_DATABASE_URL`: the dedicated supervisor login, TLS required.
- Variables `TEST262_REPOSITORY_ID`, `TEST262_COORDINATOR_PRODUCER_ID`,
  `TEST262_AUTHORITY_EPOCH` (matching database contract).
- Repository variable `TEST262_CATALOGUE_AUTHORITY`: leave unset during shadow migration;
  set to `supabase` only at the explicit switch.

For manual native publication, configure protected environment `test262-catalogue-publish`
with the same scoped supervisor connection and the existing GitHub App's
`TEST262_PORTING_APP_ID` / `TEST262_PORTING_APP_PRIVATE_KEY`. App publication triggers
independent required PR CI. The central workflow defaults to discovery without publishing.
It rebuilds a disposable native generator cache only from central sealed evidence.
The credentialed `supervise` job only seals, generates and records the patch. A separate
`validate` job with no environment secrets and non-persisted checkout credentials applies
that patch at the target revision, runs the focused xUnit batch and the exact
allowlist/hash `validate-patch` check, and requires its digest to equal the sealed patch.
`publish` requires both jobs. Run artifacts are named per run ID (overwritten on rerun),
so rerunning only a failed downstream job finds them.

### Workflow operating policy

Keep the following GitHub Actions enabled/disabled settings after merging. YAML trigger
changes do not alter GitHub's separate workflow enablement flags.

| Workflow file | Normal state | Trigger/use |
| --- | --- | --- |
| `test262-central-refresh.yml` | Enabled (new) | Refresh only after successful exact-SHA master-push MVP CI; manual master fallback |
| `test262-central.yml` | Enabled | Manual bounded screening; existing weekly MVP discovery; native publication only with explicit `publish=true` |
| `test262-mvp.yml` | Enabled | Required PR/master regression gates and daily conformance |
| `test262-central-tests.yml` | Enabled | Central code/schema or any Test262 workflow changes; manual verification |
| `test262-worker-pilot.yml` | Enabled | Shared central/tooling/schema changes; manual isolated worker drill |
| `test262-central-backup.yml` | Enabled | Daily 03:40 UTC catalogue-only backup, exact-version encrypted readback and receipt |
| `test262-retained-restore.yml` | Enabled | Sunday 05:10 UTC restore latest verified backup to disposable PG17; manual exact-prefix recovery |
| `test262-connection-test.yml` | Enabled | Manual trusted coordinator connection check |
| `test262-cutover-evidence.yml` | Enabled | Manual preservation of the reviewed manifest's entire required artifact set |
| `test262-catalog.yml` | Disabled | Retained legacy manual recovery; no automatic triggers |
| `test262-native-port.yml` | Disabled | Retained legacy manual recovery; no workflow-run trigger |
| `test262-history-import.yml` | Disabled | Manual discovery/import maintenance only; no hourly cron |
| `test262-importer-connection-test.yml` | Disabled | Manual inactive-authority historical importer maintenance |

Delete none of the legacy workflows while archive, recovery or audit references depend on
them. Leave `TEST262_HISTORY_AUTO_IMPORT=false`; re-enabling a legacy file must not restore
automatic SQLite authority.

The new registration refresh accepts only a successful **master push** `test262 MVP` run
for the exact current master SHA. PR, scheduled, failed, foreign-repository and obsolete
runs do not authorize a refresh. A manual fallback repeats the same exact-SHA validation.
It prepares the pinned corpus, sealed native registration inventory and current trusted
binary/harness/tooling provenance, then selects the native reporting target through CAS.
It does not allocate runs, budgets, queue work or reconciliation cursors, execute fixtures,
seal batches or publish a PR. An already current validated snapshot is a read-only no-op;
a newer push during preparation leaves immutable records but cannot select the obsolete
target. Refresh and manual native supervision share a non-cancelling concurrency group;
GitHub may coalesce pending refreshes. Failed refreshes retain sanitized diagnostics and
require a manual retry after diagnosis; the next successful push also retries the latest
revision. Fresh native evidence still comes from explicit native screening. This automatic
path does not change the MVP reporting target, which advances through central MVP runs.

For initial production verification, dispatch the current `master` with
`kind=mvp-composite`, `area=built-ins/Math/abs`, `publish=false`, and
`export_snapshot=false`. Preparation reuses a sealed inventory only after checking
the complete normalized inventory identity, pin and count. Native registration
hashes resolve each fixture under its C# caller's `JavaScript` directory, including
nested literal names; the upstream path alone does not identify its local bytes.

Native porting discovery and sealing use the generator's supported roots:
`test/language/` and `test/built-ins/`. Other roots, including `test/harness/`, remain
in the full catalogue and may be screened by MVP, but native preparation records
them as `policy-excluded` with reason `native-porting-area` and never schedules
them for porting. An explicit area cannot override that policy. Sealing also
filters older pending contexts; generation rejects an already sealed mixed-scope
batch before creating a generation cache or copying fixtures. It never silently
trims immutable sealed evidence. After a scope-related generation failure, retain
the old batch/outbox and start a new native run on the fixed master after its
master-push MVP validation succeeds, with `publish=false` and
`export_snapshot=false`; inspect fresh patch validation before publication.

The central workflow preserves context, diagnostics and outbox/WAL files before
attempting a bounded recovery flush, and then uploads the final recovery artifact.
Preparation has a 35-minute limit and recovery flush has a four-minute process
deadline. Full-history NDJSON export is optional and disabled by default; enable
`export_snapshot` only when that export is needed. It has a 270-second client
deadline plus a 300-second process limit. A timed-out export remains `.partial`,
has no completion footer, and is excluded from recovery uploads. A missing export
is not a successful snapshot. The independently retained backup workflow remains
the full-catalogue backup path; a local outbox artifact is not a substitute for it.

Failure diagnostics retain only operation stage, exception class, SQLSTATE when
present, safe repository-relative missing-file paths/allowlisted missing tools,
and repository file/line locations. They never print exception messages, connection
strings, stack locals or database payloads. Confirm live observations, completed
work, acknowledged outboxes and settled reservations before accepting production
execution; a green workflow or historical row count alone is insufficient.

Before activation, require import parity, an off-project backup and completed restore
drill, reporter query validation, the PostgreSQL CI concurrency suite, and a pilot of
isolated fixture execution. Stop legacy workflow invocations and drain/disable their
writers. Apply `central/cutover.sql` with the expected current epoch and `new_state=active`;
it increments epoch and records an audit event. Update the environment's epoch, then set
`TEST262_CATALOGUE_AUTHORITY=supabase`. The old MVP/native workflow jobs are gated off by
that variable; the new central workflow is gated on. Verify actual deployed API version
and state before dispatch. A code merge alone performs none of these operational changes.

For rollback, stop central dispatch, drain executing claims/outboxes, switch the database
to shadow with another audited epoch increment, and disable the central variable.
Export the last coherent central snapshot before explicitly seeding any temporary legacy
recovery path; never resume a pre-cutover SQLite artifact as if it contained central writes.
Re-enable only one authority. Old contexts cannot claim/renew/complete at a newer epoch.
Preserve obsolete-epoch outboxes for a trusted, audited evidence-only recovery rather
than attempting to complete obsolete leases. Existing performance ingestion stays running.

## Grafana and retained backups

Import `grafana/dashboards/test262-catalogue.json` with a new PostgreSQL datasource using
the **new reporter login** and TLS verification. Do not change `grafana_ro`, its existing
performance datasource, or performance dashboards. Panels separate current fixture
coverage, native acceptance/freshness, source registrations, queue health, failure
clusters, ingestion lag, stages, extra attempts and storage. Each provenance/channel is
visible; fixture counts are denominators. The extra-attempt percentage includes necessary
verification and abandoned leases and is not a claim of unnecessary duplication. Measure
that separately during the pilot; the proposed <5% goal is not an observed baseline.

Configure protected environment `test262-catalogue-backup`: catalogue-only backup DSN,
`TEST262_BACKUP_S3_URI` outside the Supabase project, `TEST262_BACKUP_KMS_KEY`,
`TEST262_BACKUP_AWS_ROLE` (GitHub OIDC-bound) and `TEST262_BACKUP_AWS_REGION`. Enable bucket
versioning/Object Lock and independently enforce retention (daily recovery snapshots,
long-lived historical archives; no automatic compact-evidence purge). The workflow uses
PostgreSQL 17 `pg_dump`, an exported MVCC snapshot shared with exact table/view counts,
SHA-256 manifests and KMS-encrypted off-project objects. It never dumps the performance
schema. Client tools receive the DSN as discrete libpq environment variables, never a
URI in `PGDATABASE` (which libpq does not expand) or a password in process arguments. Also archive original imports and failed-run outboxes; 90-day Actions artifacts
are recovery convenience, not the long-term backup policy.

Each daily backup verifies bucket versioning and at least 35 days default COMPLIANCE
Object Lock retention, checks the exact stored object versions' KMS key and unexpired lock,
and streams a full SHA-256/length readback matching the exported snapshot. Only after both
`catalogue.dump` and `manifest.json` pass does it publish and verify an encrypted retained
`backup-report.json` receipt at that run/attempt prefix. The sanitized Actions report
records object version IDs, digests, retention and receipt identity. The OIDC role needs
versioned object reads, retention/bucket-policy reads and prefix listing as well as writes.

The weekly retained-restore workflow chooses the newest completed receipt within 48 hours,
verifies the receipt's own immutable bytes/retention, then downloads **its pinned object
versions**, even if the latest S3 keys changed. A missing, stale or broken newest receipt
fails instead of falling back to an older apparently green backup. It restores into the
empty loopback `catalogue_retained_restore` PostgreSQL 17 service, checks counts/views,
constraints/permissions and shadow/epoch fencing, and retains a sanitized report. No
production database secret is available in the restore job. Scheduled restores need a
post-upgrade verified backup receipt; manual exact-prefix restore remains available for
older snapshots. Upload/readback success and restore verification remain separate evidence.

`test262-cutover-evidence.yml` reads `scripts/test262/central/cutover-evidence.json` (or another
reviewed committed manifest supplied manually). Add every required archive/checkpoint,
recovery/outbox and restore artifact's immutable ID/ZIP SHA, history-manifest SHA and expected
source parity to that manifest by PR. The verifier requires the entire configured artifact
set, independently verifies source hashes/import mappings/outbox acknowledgements/restore
reports, and reads back every preserved encrypted Object Lock version plus its report.
The default manifest retains the original eight cutover artifacts and the last five source
recoveries; it is an explicitly selected scope, not a claim that all 62 entries' archives
have been independently retained. Further historical archive retention remains tracked in
#2230/#2242; archive retention horizons require a separate long-term policy. The manifest
cannot set `history_complete=true` while the known gaps remain.

Perform a restore drill into a **new empty disposable** PostgreSQL 17 database:

```bash
python -m scripts.test262.central.backup restore \
  --archive downloaded/catalogue.dump --manifest downloaded/manifest.json
```

Set `TEST262_RESTORE_DATABASE_URL` through the secret manager. The tool refuses a database
containing catalogue schemas or `public.perf_results`, verifies the digest and exact
snapshot counts, queries restored views and checks constraints/permissions. It disables
all restored credential bindings, increments epoch and leaves authority shadow. Archive
the drill report off-project; the retained-restore workflow supplies recurring disposable verification. Credentials and
role memberships must be reprovisioned explicitly; restoring a dump must not reactivate
workers. A backup job's successful upload is not proof of a completed restore drill.

## Validation

```bash
python -m unittest discover -s tests/test262/central -p 'test_client.py' -v
node --test scripts/test262/catalog.test.js scripts/test262/specificationMetadata.test.js scripts/test262/nativePorting.test.js
node --test scripts/test262/nativeScreeningHost.test.js
# Empty disposable PostgreSQL only; CI supplies a PostgreSQL 17 service:
python -m unittest discover -s tests/test262/central -p 'test_*.py' -v
```

The client tests cover lost acknowledgments, durable completion replay, scope binding,
secret environment exclusion, inventory identities, read-only import digests, backup
connection translation and the staged container mount allowlist. SQL
regressions cover immutable replay, run/trust assignment, collision rejection, fenced
settlement, stale epochs, writer-forbidden epoch changes, incomplete/untrusted native
acceptance, infrastructure-error tolerance, product conflicts, bounded scoped paged reads
and blocking publication. The PostgreSQL test service also exercises simultaneous workers
against one shared attempt/time budget, a real restricted login, and paged client reads
with a streamed NDJSON export. Full production activation,
artifact history parity, a live reporter connection, an S3 retention configuration and a
restore drill must be recorded on #2230 with actual evidence before closing it.

## Historical import maintenance

`test262-history-import.yml` has no cron or automatic import branch. Keep it and the
importer connection test disabled in normal operation; `TEST262_HISTORY_AUTO_IMPORT=false`
remains the paused operator setting. Historical archives and `history_auto.py` are retained
for attribution/recovery, not as live scheduling authority. Re-enable manual maintenance
only for an approved recovery window, verify the dedicated legacy importer binding and
inactive authority, then select the exact archive run, kind and source artifact IDs.

A successful source gets an immutable `history_verified_snapshot` receipt in
`legacy_control_records` only after central observation/mapping parity and unchanged source
bytes are verified. Counts or a green run alone do not qualify. Inspect `import-report.json`
and preserve source ZIP/SQLite hashes, outboxes/WAL, recovery links and missing-artifact
limitations. Verification of a selected source set does not cover every historical archive:
11 unavailable archive gaps still keep `history_complete=false`. Record a material recovery
milestone on #2242 without overwriting other agents' issue edits.

## Isolated worker pilot before cutover

`.github/workflows/test262-worker-pilot.yml` runs automatically on changes to the
shared central coordinator/client/helpers, catalogue bridge/porting tooling, invariants
tests and Supabase definitions in a PR, or manually after merge. It uses an empty PostgreSQL 17
service named `catalogue_pilot` on loopback and a newly bound restricted login.
It references no protected environment or production connection secret. The pilot
refuses remote hosts, existing catalogue schemas and performance data before
applying migrations. Only that disposable service receives active authority.

The pilot registers a clearly identified partial inventory of five pinned Math.abs
fixtures and executes their ten non-strict/strict variants through the production
worker's shared Docker isolation, API claims, durable outbox ingestion and completion.
It checks isolation with the same container arguments: no external network,
non-root UID, read-only root, zero effective capabilities, no-new-privileges,
separate PID namespace, and no database/GitHub credentials, checkout or outbox mount.
It verifies ten observations/completed work items, ten charged attempts, zero reserved
budget and zero pending outbox messages. Repeated acknowledged flushes must not add
observations. Product pass/fail outcomes are recorded; infrastructure/incomplete
outcomes fail operational acceptance. This is not full Test262 conformance or a native
publication pilot.

The workflow now passes `--workers 2`: two independent supervisor connections,
run identities and durable outboxes race their first claims against the same queue
and ten-attempt shared budget. Each executes five variants. Worker zero deliberately
loses its first ingestion acknowledgement after the real server commits. Its
supervisor stops, verifies one committed observation and pending upload/completion,
then reconnects and resumes from that exact outbox before claiming more work.
The final report requires distinct initial claims, exactly one copy of the recovered
observation, ten completed items and zero pending uploads/reservations. Both outboxes
are retained. `--workers 1` preserves the earlier baseline. This is a simulated
transport acknowledgement loss/reconnect check; process-kill, lease-expiry, conflict,
budget-exhaustion and native publication recovery remain separate acceptance drills.

With `--queue-drills` (enabled by the PR workflow), the pilot then creates separate,
explicitly synthetic control-test provenances. These never count as the ten real
compiler fixture results. One lease expires at its actual 31-second server deadline;
renewal must reject its stale generation, reclamation must increment the fence and
conservatively charge the abandoned reservation, and late evidence must be retained
without completing or changing the replacement lease/budget. Current completion must
settle all reservations. The drill also rejects changed payloads under the same request
or observation ID while preserving the original receipt/evidence. A one-attempt budget
with two items must block excess claims both while reserved and after charging, leaving
one untouched pending item and no reservations. The module checks the loopback pilot
database/login before writes, and saves these results under `queue_drills` in the report.
No clocks or lease rows are altered to simulate expiry. Native publication/restore
acceptance remains separate.

With `--process-kill` (enabled by the PR workflow), a spawned supervisor executes
one additional real strict fixture under a separate marked provenance and budget.
It pauses only after the production worker has durably saved its result/completion
in the FULL-synchronous WAL outbox, before upload. The parent verifies one pending
message/completion, zero server observations and one reserved attempt, then sends
SIGKILL exclusively to its owned supervisor process. This boundary follows fixture
container cleanup; it does not kill a running fixture container. Pending spool bytes
are retained as `process-kill-pending.sqlite`. A fresh spawned supervisor resumes
the same outbox with execution limit zero: one original result and completed item,
generation one, one charged attempt, no reserved budget or pending uploads, and no
new fixture execution. The final acknowledged outbox is also retained. Results live
under `process_kill` in the report and do not change the ten-variant baseline counts.
This checks supervisor death after persistence; it does not establish termination
or cleanup of an in-flight fixture container, nor loss before durable persistence.

Review the `pilot-report.json` and `test262-worker-pilot-<run>-<attempt>` artifact before
recording the isolated-execution item in #2230. Evidence stays in the disposable run;
never upload this partial pilot corpus to production or activate Supabase from this job.


### Disposable backup/restore pilot

The isolated worker workflow also enables `--restore-drill`. After real execution,
process-kill replay and queue controls, the pilot runs the production backup and restore
tools with PostgreSQL 17 clients in a trusted container. It dumps the populated local
`catalogue_pilot` database and restores into a new `catalogue_pilot_restore` database
on the same disposable service. No Supabase or external-storage credentials are used.
The source must be the loopback pilot admin connection, and the restore target name
must be unused. Both databases are discarded with the Actions service.

The drill independently compares canonical contents of every catalogue table,
excluding only the deliberately changed binding-enabled/authority/epoch fields,
and compares RLS, ACL and function security metadata. It checks that old bindings
are disabled, the previous worker login cannot use the scoped API, authority is
shadow at epoch two, and source contents/control state remain unchanged. A disposable
`public.perf_results` sentinel verifies public performance data is excluded from the
archive and preserved in the source. The normal restore tool also verifies the archive
digest, exact snapshot table/view counts and validated constraints.

Retained evidence includes `pilot-report.json`, `restore-drill/catalogue.dump`, and
`restore-drill/manifest.json`; these contain only disposable pilot data. Passing this
drill demonstrates the tool path and restored data/security behavior. It does not
verify production backup-role permissions, KMS encryption, independent object retention,
Object Lock, original historical archive preservation, or a restore of an off-project
production backup. Those remain separate operational acceptance before cutover.

## Collection run lifecycle and Actions reconciliation (#2332)

A terminal **collection batch** is not full-suite completion, conformance, native
acceptance or publication success. A bounded batch can be `complete` with failing
JavaScript tests and unexecuted required variants. Run/budget/publication states
are separate. The budget follow-up below settles only proven abandoned reservations;
publication reconciliation runs independently.

Deploy reviewed migration
`20261023170000_test262_catalogue_run_lifecycle.sql` before enabling the new
finalization/reconciliation code in production. It adds a scoped, epoch-fenced
`api_finish_run` endpoint and terminal-state guard, without rewriting existing rows,
changing authority or granting raw-table/public access. A missing endpoint is a
failure, not permission to fall back to raw SQL or fabricate a terminal receipt.
Existing reporter views/exported run rows expose terminal state and `ended_at`;
`runs.environment_diagnostics.collection_lifecycle` and audit events retain the
sanitized identity, reason and flush proof.

The supervisor finalizes after the always-run durable recovery flush. It replays
any unacknowledged outbox observations/completions before recording `complete`.
Failure/cancellation records the known batch termination, while missing flush proof
on an otherwise successful job gives `incomplete`. Pending recovery artifacts and
late evidence remain preserved; finalization neither releases live leases nor
rejects late observation replay. A screening metric receipt is required for
`complete`. The first terminal timestamp/proof is immutable, and a lost finalization
response can be replayed without changing it.

`test262-run-lifecycle.yml` reconciles after a central Actions workflow completes,
including cancellation/timeouts which prevented its finalizer. It runs reviewed
master code with the existing protected coordinator environment, downloads no
artifacts and executes no fixtures. The exact run/attempt, repository, master
branch, workflow path, compiler SHA and completed `supervise` job must agree.
Only the collection job's timestamp/conclusion is used; downstream validation or
publication results cannot rewrite collection state. A successful Actions job
without successful preparation and durable-flush steps is not complete evidence.
Missing, deleted, active, unsupported or ambiguous history is reported explicitly
and left unchanged. Imported/legacy runs and another producer's runs are untouched;
no historical timestamp is inferred from an import or observation time.

For existing stale records, first manually run **Test262 collection lifecycle
reconciliation** on master with `apply=false`. Review its sanitized report, then
use `apply=true` to apply only proven terminal states. Optional `actions_run_id`
and `actions_run_attempt` select a precise retry identity. Each invocation inspects
at most 100 owner-scoped running records with a ten-minute processing bound and
30-second REST request timeouts; it does not claim exhaustive reconciliation when
bounded. Unresolved records can occupy a bounded scan; use exact IDs/attempts for
later records instead of deleting or force-closing ambiguous ones.

Equivalent trusted-supervisor commands (credentials configured out of band):

```bash
python -m scripts.test262.central.cli reconcile-runs --limit 100
python -m scripts.test262.central.cli reconcile-runs \
  --actions-run-id 39878208600 --actions-run-attempt 1 --apply
```

These commands never publish a PR, enroll a writer, change authority, replay a
legacy import. Retain finalization and reconciliation
reports with the existing recovery/retention policy. Live migration and historical
repair are operational steps after PR review, not implied by disposable test success.

## Budget lifecycle and independent publication recovery (#2344)

Deploy `20261023193000_test262_catalogue_budget_lifecycle.sql` after the #2332
migration and before running this code in production. It changes no historical
rows or authority settings. It extends the existing claim API and adds the
coordinator-only, epoch-fenced `api_finish_budget(epoch, id, apply)` endpoint.
Existing scoped reads/exports include `ended_at`, `claim_cap_ms` and `lifecycle`.
The lifecycle receipt and `finish-budget`/`reopen-budget` audits retain counters,
run/attempt/SHA, collection proof, reason and before/after values.

| Budget state | Meaning |
| --- | --- |
| `open` | Active or awaiting proven collection termination/settlement. Zero reservations alone proves nothing. |
| `closed`, reason `drained` | Collection completed, reservations settled, and all selected work is terminal (completed, deferred or cancelled). This does not establish test success or full-suite coverage. |
| `exhausted` | Pending work exists but the attempt limit or remaining time cannot fund another claim at the recorded cap. Historical unknown caps remain explicit; they are not guessed. |
| `interrupted` | Collection failed/incomplete, or a successful bounded worker stopped with pending work and remaining capacity. |
| `cancelled` | The latest proven collection attempt was cancelled; pending evidence/work remains retained. |

Only `run` scopes with an exact `actions:<run ID>` key are finalized. All matching
attempts must belong to the trusted coordinator and have #2332 terminal proof.
Unproven/active attempts, foreign/ambiguous ownership, mismatched accounting and
live leases/reservations are reported and left unchanged. No counters are inferred
from test results. The same repository advisory lock serializes finalization with
claims/completions. Only expired reservations can be conservatively charged through
the existing settlement API, once, at their full reserved amount. Expired work
returns to pending/deferred as in claim recovery. Recovery outboxes are not
downloaded/deleted; later evidence can still be ingested without double charging.

Normal finalization follows durable outbox flush and #2332 run finalization. If
it crashes between those operations, replay of `finalize` recovers the budget
even when the run is already terminal. The existing collection reconciliation
workflow also reconciles budgets after runs; an exact Actions ID scopes both.
An active lease may require a later reconciliation after expiry. Repeating the
same terminal budget receipt preserves its timestamp and counters. A strictly
newer Actions attempt can reopen the same scope on its first claim; charged
attempts/time, limits and membership remain cumulative, and the claim cap cannot
change. Prior terminal receipts remain in the audit history. A retry without
claims is finalized against its own latest terminal run proof.

```bash
python -m scripts.test262.central.cli reconcile-budgets --limit 100
python -m scripts.test262.central.cli reconcile-budgets --actions-run-id 40008295578 --apply
python -m scripts.test262.central.cli reconcile-publications --pr-number 2326
python -m scripts.test262.central.cli reconcile-publications --pr-number 2326 --apply
```

Both commands default to dry-run, inspect at most 100 records and enforce a
ten-minute processing deadline. Budget plans execute under the same database
lock as apply, but dry-run never settles reservations. Publication reconciliation
reuses the publisher's existing reservation recovery/head fencing, with bounded
30-second GitHub/git network calls and isolated unavailable-history reports.
It never pushes, creates/reopens PRs, recreates branches, executes fixtures or
generates acceptance. Merged/closed PRs can reconcile after branch deletion;
an open PR with a deleted/changed branch loses ownership. Changed PR heads lose
ownership even if merged. Existing `ownership-lost`/`blocked` records are not
promoted to accepted evidence. Missing/ambiguous history stays unresolved.

**Test262 publication reconciliation** runs on reviewed master in the protected
coordinator environment after central completion (including no-patch/MVP runs).
Manual dispatch defaults to `apply=false` and accepts an optional exact PR number.
It uses read-only GitHub permissions and retains sanitized before/after
publication, batch, provenance, expected head, target revision and check receipts.
No publisher credentials are required. #2340 scheduling may invoke these bounded
paths; #2341 monitoring should show unresolved dispositions, deadline limits,
pending work and explicit ownership loss rather than treating them as success.
Scopes that occupy a bounded scan can be inspected with an exact Actions ID/PR.

After review/merge: take a fresh verified backup, apply the additive migration,
inspect dry runs, then apply only reviewed scopes. Verify stale PR #2326 with
its original batch/head/provenance links; #2276 is an already-merged baseline
and #2315's changed-head ownership fence must remain. Retain before/after
receipts, verify counters/evidence are unchanged except conservative expired
charges, and run one scoped no-publication batch to prove automatic finalization.
These are live acceptance steps, separate from disposable regression coverage.

## Same-SHA scheduled native recovery (#2340)

The existing central workflow's **Monday 06:15 UTC** timer selects `native`.
There is one scheduler and its frequency is unchanged. Manual dispatch still
defaults to MVP; master-push MVP validation remains the prerequisite for native
preparation. The shared `central.schedule.collection_kind` policy resolves the
event into the actual worker kind, native generation and credential-free focused
validation. A scheduled collection never publishes: publication still requires
manual dispatch, native mode and the explicit publication checkbox.

Each scheduled Actions run gets a **new allocation** identified by its own run ID.
Rerunning an attempt of that Actions run preserves its existing budget identity,
charged attempts/time, limits and claim cap. A new recovery allocation does not
reset, extend or transfer charges out of an exhausted predecessor. Default limits
remain 200 candidate files, 100 accepted files, 400 attempts and 1,200,000 ms;
workers independently stop at 100 attempts or 600 seconds. A worker limit,
infrastructure failure or queue delay is not evidence of budget exhaustion.
`central-worker.json` records the actual stop reason; the #2344 lifecycle receipt
is authoritative for the settled budget state.

Planning first resolves exact corpus, binary, harness, tooling, capabilities,
environment and timeout provenance. At that same provenance, unfinished pending/
leased fixtures precede new discovery and component-based retries. Only missing
nonterminal variants are enrolled in the new budget. Existing completed,
cancelled and per-item deferred variants are not reset or rescreened. Per-item
attempt exhaustion still requires an explicit reviewed retry policy; budget
exhaustion merely leaves unattempted variants pending for a later allocation.
Recovery-only selections leave the discovery cursor unchanged. New discovery
continues to use its rotating share, and incompatible provenance cannot supply
missing variants or current acceptance. Explicit area, native-porting exclusions
and registration filters remain in force.

Live/expired leases, eligibility delays, late writes, contradictory evidence and
outbox replay retain the existing claim/completion/settlement fences. A scheduled
run cannot steal a live lease; delayed or leased-only work may produce no claim
and remains visible for a subsequent invocation. A successful collection can have
an exhausted or interrupted budget. Native sealing still requires every required
variant to have trusted, matching, non-invalidated live passes; a single partial
fixture cannot be accepted. A new merge or changed provenance starts independent
evidence instead of combining historical passes.

### Bounded disposable acceptance

The existing isolated worker pilot includes `--scheduled-recovery`; no production
credentials, database writes or publication are needed. It runs the production
policy resolver, planner, claim API, native worker, outboxes, finalizers and sealer
on a pinned two-variant Math.abs fixture, against the same built binaries/source
SHA and unchanged native identity. Two explicit scenarios exercise normal
**attempt** exhaustion (1 attempt) and **time** exhaustion (remaining time cannot
fund another 120,000 ms reservation). Worker limits are higher, so these are not
worker-limit or infrastructure stops.

For each scenario, the first allocation executes one required variant, fails to
claim the other because of its budget, remains incomplete for native acceptance,
and finalizes its budget as exhausted. The next simulated scheduled invocation
gets its own allocation, resumes exactly the missing variant, reuses the first
immutable pass only at the same provenance, and seals one complete fixture.
A third invocation performs zero executions and leaves discovery cursor/old
charges unchanged. Lost responses after run finalization and after budget
finalization are replayed without duplicate charging or timestamp changes.
The existing concurrent-worker, process-kill, lease-expiry and late-write drills
remain part of the same pilot and preserve their separate scopes.

```bash
# Only after provisioning the empty loopback catalogue_pilot service and building
# the fixture image, CLI and NativeScreeningHost. This driver rejects remote DSNs.
python -m scripts.test262.central.pilot --workers 2 --queue-drills --process-kill \
  --scheduled-recovery --restore-drill --root "$TEST262_ROOT" \
  --output artifacts/test262/worker-pilot
```

Retained `scheduled-recovery/scheduled-recovery-report.json` and the enclosing
pilot report contain source/pin and full execution identity, per-invocation
run/attempt/budget/work/observation IDs, required variants, before/after cursors,
charges, reservations, stop reasons and acceptance bindings. Artifacts also retain
the per-invocation contexts and acknowledged outboxes under the existing policy.
The driver refuses remote/existing production catalogues. Its partial inventory,
empty registration snapshot and successful master-validation input are controlled
disposable fixtures, explicitly labelled; actual native execution/accounting is
real. A simulated `schedule` input is **not** an observation of GitHub's clock.
After merge, verify the next actual scheduled workflow's `central-schedule.json`,
same-SHA/provenance when applicable, worker and lifecycle receipts. Do not claim
same-SHA recovery across a changed compiler/pin/environment, or infer full-suite
conformance from this bounded proof.

## Independent local and downstream workers (#2339)

The supported execution path is `central.cli enlist` on Linux with Docker,
Python 3.12, Node 24 and the reviewed .NET 10 runtime/build. A downstream worker is
an independently managed checkout of the same reviewed repository revision; it
uses the existing producer kind `local`. Windows/macOS native supervisors and
remote-host connectivity are not covered by the acceptance pilot. They must use
an equivalent supported Linux environment before contributing trusted work.

### Enrollment and permission boundary

Deploy the reviewed `20261024001000_test262_catalogue_worker_execution.sql`
migration before enrollment. It creates the NOLOGIN `test262_worker` execution
role only; it creates no login, producer, allocation or authority change. An
operator creates a dedicated LOGIN and secret out of band, binds it with
`central/enroll_worker.sql`, and grants **only** `test262_worker`. Never share the
coordinator, importer, reporter, backup or another worker's credential. Each
worker needs a distinct LOGIN, producer UUID, checkout, run key and durable outbox.

The v1 binding uses permission `coordinator` / trust `trusted` because the existing
claim/complete API checks those labels. Its actual SQL grants are execution-only:
contract/scoped reads, own-run start/metrics/finalization, claims, renewals, ingestion
and completion. It has no raw-table access, coordinator membership, allocation
creation/finalization, inventory/target/cursor changes, batch sealing, export
portal, or publication API. Repository scope and run/lease ownership are enforced
server-side on every call. The role can read the approved repository's catalogue
and execute centrally enrolled work; it is not a sandbox for arbitrary untrusted
contributors. An untrusted collector remains append-only and cannot claim trusted
work. The operator must review the host, source, dependencies and role memberships
before assigning trusted execution credentials.

Operator commands (IDs verified against the intended repository, LOGIN/secret
already provisioned; use the operator connection configured outside shell history):

```sh
psql -X -v ON_ERROR_STOP=1 -v repository_id='<repository-uuid>' \
  -v producer_id='<new-worker-uuid>' -v login_name='<dedicated-login>' \
  -f scripts/test262/central/enroll_worker.sql
```

### Prepare once, enlist independently

The trusted coordinator prepares a **shared non-Actions allocation** using the
existing planner. Keep candidate/attempt/time limits explicit and bounded.
An `actions:<id>` allocation is reserved for its original Actions owner/attempts
by #2344 and cannot be borrowed by independent workers. Native preparation still
requires current master and successful exact-SHA validation. MVP and native use
the same enlistment command; the context determines the kind.

```sh
python -m scripts.test262.central.cli prepare --kind native \
  --revision '<reviewed-source-sha>' --root '<pinned-test262-root>' \
  --run-key 'shared:reviewed-session' --budget-key 'shared:reviewed-session' \
  --candidate-limit 5 --accepted-limit 5 --attempt-limit 20 --budget-ms 1200000 \
  --output artifacts/test262/shared-context.json
```

For MVP use `--kind mvp-composite`. The coordinator's repository/producer/database
settings stay on the coordinator. Deliver only the sanitized context and verified
build artifacts to the workers. The context is a plan reference, not a credential
or permission grant. Each worker configures its own `TEST262_DATABASE_URL` through
its secret manager plus `TEST262_REPOSITORY_ID`, `TEST262_PRODUCER_ID` and
`TEST262_AUTHORITY_EPOCH`. A direct/session-pooled TLS connection must preserve its
own `session_user` binding. No service-role or admin credentials are supported.

Before supplying secrets, each worker checks out the exact clean source SHA,
installs lockfile dependencies, acquires the exact upstream pin, and builds the
reviewed fixture image. Build both Cli and NativeScreeningHost for the needed mode.
Use the coordinator's exact reviewed binary bundle when a local rebuild produces
a different identity. Never override a mismatch or copy a newer identity into an
old context. Compiler/runtime/harness/tooling, capabilities, timeouts and environment
must all match; matching source SHA alone is insufficient.

```sh
npm ci --ignore-scripts
python -m pip install -r scripts/test262/central/requirements.txt
dotnet build src/Cli/Jroc.csproj -c Release --nologo
dotnet build scripts/test262/NativeScreeningHost/NativeScreeningHost.csproj -c Release --nologo
docker build -t test262-fixture:local -f scripts/test262/central/fixture.Dockerfile .
# Configure this worker's scoped connection/identities after building.
python -m scripts.test262.central.cli enlist \
  --context artifacts/test262/shared-context.json --root '<pinned-test262-root>' \
  --run-key 'local-session-1' --outbox '<durable-worker-directory>/outbox.sqlite' \
  --image test262-fixture:local --limit 10 --seconds 300 \
  --output artifacts/test262/enlistment-receipt.json
```

A second worker runs that command from its own checkout with its own identity and
outbox. Run identity includes the producer UUID; repeating an interrupted run key
resumes the same run rather than resetting charges. An ended run key cannot execute
again; use a new explicitly bounded session to claim remaining work. Membership
and shared reservations are central; SQLite is only the local durable outbox.
The coordinator remains responsible for the shared allocation's lifecycle and
fresh native acceptance/publication. `collection_state=complete` means this bounded
worker's upload/collection ended, not full-suite completion or publication approval.

### Disconnection, stale clients, interruption and revocation

Cold starts require the central contract, active authority, current target,
sealed registration snapshot and matching native validation. They fail before
creating a run/outbox or claiming work if unavailable/stale/incompatible. The
central target is checked again before each new claim. A target can change just
after that check; the existing server lease remains bounded and any eventual result
retains its original provenance. This is not an atomic guarantee of current master
throughout execution, and results never themselves authorize publication.

An already leased variant may finish while disconnected. Its result/completion
are committed locally before upload. The supervisor then stops on upload failure;
it does not execute an offline queue of new central claims. Preserve the SQLite
file **and WAL/SHM files** together while active, or take a SQLite backup after the
supervisor stops. Never copy another producer's spool or edit its scope, epoch,
request IDs, observations, generations or receipts to force a replay.

After restoring connectivity, use the same producer/run key/context and outbox:

```sh
python -m scripts.test262.central.cli enlist --replay-only \
  --context artifacts/test262/shared-context.json --root '<pinned-test262-root>' \
  --run-key 'local-session-1' --outbox '<durable-worker-directory>/outbox.sqlite' \
  --output artifacts/test262/recovery-receipt.json
```

Replay performs zero new executions, permits historical results from an older
checkout/target, and ends a still-running interrupted collection as `incomplete`.
It preserves any already terminal collection state. Existing `cli flush --outbox`
is also supported for upload-only recovery. A live lease can complete once; after
expiry/reclaim, late evidence remains auditable without completing the replacement
lease or refunding/recharging its reservation. Conflicting immutable replays fail;
distinct contradictory observations remain evidence and cannot bypass trusted
native acceptance. Changed epochs/revocation fail closed: retain/quarantine the old
outbox for operator review; do not rewrite its scope or re-enable a revoked identity
just to flush it. Offline experiments without a central lease are local diagnostics,
not current central conformance or suite-completion evidence.

Revoke a worker with the operator-only `central/revoke_worker.sql` using the same
three verified IDs. It disables its subject and producer, revokes the worker role
and sets NOLOGIN; existing connections are fenced on subsequent API calls. Already
issued leases expire and are conservatively settled by the existing claim path.
Keep evidence/outboxes for audit. Do not delete historical runs, observations or
charges as part of cleanup. Operator changes in production require explicit approval;
implementing/testing this issue does not enroll production workers.

### Retained independent-enlistment acceptance drill

The existing `test262-worker-pilot.yml` adds `--independent-enlistments`; no second
scheduler/workflow or production connection is needed. It provisions two distinct
execution-only LOGINS/producers in the already protected empty loopback database,
clones two separate source and upstream checkouts, independently stages the exact
reviewed binary outputs, and launches two supervisor processes from those checkouts.
They share a central allocation but have separate runs/outboxes and race their first
claims. Dependencies/runtime artifacts are shared read-only inputs on one CI host;
this establishes independent enlistments, not different hardware or a remote WAN.

Three labelled scenarios execute real strict/non-strict Math.abs variants: MVP
with a lost committed acknowledgement, native with SIGKILL after durable persistence
and fixture cleanup, and a deliberate native retry with connection loss before
upload. Standard flush/replay-only commands must recover with zero new executions,
two distinct completed variants, two charges and no reservations/pending outboxes.
A separate synthetic control provenance waits for real server lease expiry, reclaims
across the two producer identities, fences old-owner renewals/late completion,
preserves contradictory evidence and rejects changed immutable replay payloads.
Dirty-checkout, changed-binary and offline cold launches must leave central state
unchanged. Unit regressions cover stale target, pin and environment mismatch checks.
Both logins are revoked at the end, including a rejection on an already open session.

The drill uses a controlled partial inventory, empty registrations and synthetic
successful central validation, explicitly labelled in its report. It does not claim
production conformance, actual remote onboarding, a live cutover or in-flight fixture
container termination. The existing native acceptance, process-kill, queue and restore
regressions remain in the same CI workflow and supply complementary evidence.

Retain `independent-enlistments/independent-enlistments-report.json`, sanitized
per-process configuration/logs, contexts, execution/recovery receipts and outboxes
under the existing 90-day pilot artifact. Review exact checkout/build/harness/pin,
producer/run/variant identities, charges, fault checkpoints and revocation results
before closing #2339. The ordinary catalogue-only restore drill also includes these
new records and limited function ACLs. Production onboarding is a separate operator
decision and is not required to establish the disposable acceptance gates.

## Operational monitoring (#2341)

The operator selected **dashboard warnings first, with notification rules paused**,
and adopted these targets on 23 October 2026: avoidable recorded execution repeats
below 5% over seven days, interactive queries within two seconds, and verified
backup age within 36 hours. The versioned policy is
`grafana/alerting/test262-monitoring-policy.json`. No contact point or notification
routing is enabled by this implementation. The paused provisioning definitions are
`grafana/alerting/test262-operational-paused.json`.

### Read-only access policy and compatibility

Retain the existing shared read-only performance/catalogue datasource. Its role
must have only the intended reporting-view and existing performance SELECT access,
without raw catalogue access, writes, scheduling or publication API execution.
This implementation does not change the datasource or expand its privileges.
Keep effective-grant audits and measurements in the operator's private records;
do not publish live database permission snapshots or operational evidence in git.
Backup access remains catalogue-only and must not gain performance-table access.

### Reports, collector and freshness semantics

Deploy the reviewed additive operational-reporting migration, then update the
existing `jroc-test262-summary` and `jroc-test262-conformance` dashboards from the
matching repository JSON. Keep their UIDs and existing panels/variables/links;
operational panels own IDs 100 and above. The older `test262-catalogue.json`
template is separate and is not a replacement for the live conformance dashboard.
Rebuild only the operational additions with:

```sh
python -m scripts.test262.central.monitoring_dashboard
```

The added views expose collection states/timestamps, budget charges/reservations
and ledger mismatches, current versus historical queues, delivery latency,
publication ownership/backlog, exact build/harness/environment fingerprint and
upstream pin, registration/validation revision, duplicate classification and
backup/restore receipt freshness. They grant SELECT only to the existing reporter
role; no raw-table grants are added. The conformance view preserves its columns
and verdict rules while replacing repeated per-fixture evidence aggregation with
one set-based aggregation. Missing required variants and untrusted/conflicting
observations keep their original semantics.

`test262-operational-monitor.yml` observes GitHub metadata hourly at minute 47,
using existing protected coordinator settings and a read-only Actions token. It
builds or runs no fixtures, claims no work, changes no targets/cursors/budgets,
performs no publication and sends no notifications. This is an operational-health
collector, not another work scheduler. It appends sanitized samples through
`api_monitor`; ingest-only, reporter and execution-only workers cannot call it.
The server assigns repository, producer and observation time, validates bounded
metadata, fences epoch/revocation and rejects conflicting immutable replay.

The new operational sample table is included in catalogue-only backups. It is
telemetry rather than conformance evidence and is outside the existing 32-table
logical result-export contract. Its backup SELECT grant is added only for an
already provisioned backup role; no backup login or performance access is created.

A no-write rehearsal is available without a database connection:

```sh
python -m scripts.test262.central.monitoring --dry-run \
  --repository-name tomacox74/js2il --output artifacts/test262/monitoring-receipt.json
```

Do not put real credentials in commands/history; configure the variable through
the existing credential manager. Normal recording additionally uses the protected
`TEST262_DATABASE_URL`, repository/producer IDs and authority epoch. The workflow
retains sanitized receipts/diagnostics for 90 days. No existing workflow is
manually dispatched as part of implementation or baseline measurement.

The collector queries the latest 20 runs of each reviewed workflow, verifies the
repository/event/attempt and keeps the latest observed state separate from the
last successful run. It downloads only the exact small receipt artifact, verifies
its ZIP SHA-256 against GitHub metadata and validates both retained object proofs.
Backup receipts require catalogue-only verification; restore receipts require
shadow authority, disabled writer bindings and no production cutover. It retains
only timestamps, IDs/digests and verification flags, not storage destinations,
secrets, database dumps or fixture payloads. Missing, expired, mismatched or
unreadable receipts become unknown. A successful Actions conclusion alone does
not establish backup or restore success.

Receipt retention describes the exact-version storage checks made by that job;
it is **not a fresh S3 query** and does not prove indefinite archival or recovery
objectives. #2343 continues to own retention policy/RPO/RTO. A sample older than
two hours or a missing collector is unknown, never silently healthy. Current
master means the last GitHub observation; it is not an atomic statement that no
new push occurred during a query. New master pushes have one hour for validation
and registration refresh before becoming a freshness warning. The exact revisions
and observation times remain visible alongside that grace period.

### Thresholds and response

| Signal | Warning / interpretation |
| --- | --- |
| Monitoring receipt | Older than two hours; absent metadata is unknown |
| Backup | Last successful verified receipt older than 36 hours, failed latest job, failed retention or expired object retention; absent proof is unknown |
| Restore | Verified disposable restore older than eight days or failed latest job; absent proof is unknown |
| Native target | Incoherent registration/validation or mismatch with observed master after the one-hour validation grace |
| Collection | Trusted collection still running after two hours; collection completion is not full-suite completion |
| Reservations | Ledger mismatch or reserved lease still expired after a 15-minute settlement grace |
| Publication | Blocked/ownership-lost, or reserved/updating batch older than 24 hours; batch age is explicitly not publication creation time |
| Delivery latency | Worst producer p95 exceeds 300 seconds over seven days; negative clock skew or no samples is unknown |
| Duplicate execution | At least 5% avoidable repeats among classified eligible recorded executions; incomplete classification is unknown |
| Interactive SQL | Target <=2,000 ms; measure database time and browser/transport time separately, and never substitute the 1,800-second export budget |

On a warning, use the linked run/PR, exact revision/provenance, reservation ledger
and durable receipts before changing state. Historical interrupted budgets and
old-provenance pending work are not automatically current leaks/backlog. A failed
or disconnected local outbox is not visible until it reconnects; the delivery panel
measures received evidence only. Do not refund charges, reopen work, change epochs,
republish or re-enable credentials merely to clear a dashboard warning.

### Duplicate measurement and limits

The seven-day numerator is a new immutable live execution observation at identical
provenance/fixture/variant after a prior pass finished, without a recorded deliberate
retry or lease recovery. The denominator includes baseline and avoidable recorded
executions. Explicit deliberate retry generations and timely reopen/backoff decisions
are separated, as are recovery generations following real lease expiry. Replaying an
acknowledged upload adds no observation and therefore does not inflate execution
counts. Repeat evidence without a prior pass is unclassified, rather than automatically
labelled unnecessary. Missing lease/work identities are counted as uninstrumented.
Either uncertainty prevents a passing classification claim. This measures auditable
recorded executions, not unobservable CPU work lost before durable evidence; it
makes no exactly-once execution claim.

Keep representative seven-day counts and classification completeness in private
operator evidence. Do not extrapolate one measurement to future independent
writers. Verify representative performance panels through the shared datasource.
Reviewed Linux/Windows smoke and BenchmarkDotNet suite workflows run on
release/manual dispatch, so quiet time is not a missed daily job.
An expected release/manual run needs its own ingestion receipt before declaring
continuous ingestion or attributing a missing measurement.

### Query regression and rollout acceptance

Retain before/after timing and full-row equivalence results privately. Compare
the current native/MVP report against the set-based replacement in one read-only
statement snapshot before deployment. Measure database execution separately from
connector overhead and browser rendering; a read-only SQL rehearsal is not proof
of deployed Grafana latency, and a small diagnostic sample is not a sustained p95
acceptance test. Never publish live measurements or permission/database metadata
without explicit operator authorization.

After merge: deploy the exact reviewed migration, update both existing dashboard
UIDs, let the protected collector produce a sample, and repeat representative
filtered queries through Grafana at current volume. Verify all new panels, actual
native/MVP conformance latency, unknown/no-data behavior, unchanged performance
panels and effective grants. Retain that receipt on #2341 before marking deployment
acceptance complete. The PR's PostgreSQL tests exercise warning→recovery→unknown,
permission/idempotency checks, retry/recovery/unclassified measurement, and partial,
untrusted and conflicting conformance evidence; the existing real-worker and restore
pilot checks the new schema and ACLs too.

All proposed rules are paused, with NoData and query errors preserved as distinct
states, a five-minute evaluation interval and 15-minute pending period. Grafana's
advertised alert-management tool returned `tool not found` during this review; no
live rule was created and no delivery test was claimed. The versioned paused
provisioning file is reviewable independently. Notification activation, contact-point
selection and a real delivery/recovery test remain a later operator decision under
the chosen dashboard-first policy; do not route notifications to an invented address.
