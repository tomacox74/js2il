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
keep `history_complete=false`. The owner accepted #2242's 41-container preservation
ledger and historical gap handoff on October 26, 2026. Longer-term retention and
verification belong to reopened #2343; client run/budget/publication lifecycle
bookkeeping remains a separate follow-up under #2230.
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
versioning/Object Lock and independently verify the existing finite retention for daily
recovery snapshots and selected historical archives. Long-term class horizons remain
owner decisions under #2343, not an approved indefinite policy. The workflow uses
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
recoveries; it is an explicitly selected scope, not the entire import/recovery archive
inventory. The frozen #2242 inventory below identifies all 62 original source ZIP/SQLite
pairs inside the retained discovery container. At its observation, only three of 36
import/recovery containers had identified independent-retention proofs; the subsequent
33-container preservation and reconciled ledger below now cover all 41 required containers.
Further retention horizons remain a separate long-term policy decision.
The manifest cannot set `history_complete=true` while the known gaps remain.

### Discovering unmatched retained candidates (#2242)

The existing `verify-retained` mode accepts only the original eight receipt-pinned
objects. It cannot locate independent copies for the **33 unmatched recovery containers**.
The metadata-only `discover-retained` operation is a separate reviewed path in the same
master-only protected workflow; it is not preservation or payload verification.

It reads the committed, validated frozen inventory and lists versions **only under the
configured `cutover-evidence/` prefix**, using at most ten pages of 100 entries and 100
candidate versions. It recognizes the existing `<run>-<attempt>/<artifact-id>.zip`
layout and records every matching version, not merely the latest one. It checks exact
version metadata, expected source ZIP length, configured KMS identity and current
COMPLIANCE retention without downloading/decrypting payloads. It does not substitute an
ETag, filename or S3 checksum field for the source ZIP SHA-256. Expected source hashes
and candidate metadata remain explicitly distinct; `payload_digest_verified=false`.

Public reports record source inventory hash, archive ID, candidate preservation run/attempt,
opaque version, lengths, lock deadline and metadata disposition. Private bucket/prefix/key
and KMS identifiers, raw error text and permission audits are excluded. The mode cannot
upload, extend retention, change a policy, import data or execute fixtures. Its inline
session policy permits only `s3:ListBucketVersions`, `s3:GetObjectVersion` (HEAD metadata)
and `s3:GetObjectRetention`, intersected with the existing role; it does not grant decrypt.

```sh
# Only after review/merge and operator authorization; do not run an unreviewed branch.
gh workflow run test262-cutover-evidence.yml --ref master \
  -f operation=discover-retained
```

`discovery_complete=true` means only that this bounded metadata scan completed, not that
the candidates were preserved or their bytes match. Any matching versions require a
separate exact-version payload-verification decision before promotion into the retention
ledger. `no_candidate_in_scoped_listing` means no candidate **in that prefix/layout at
that observation**, not no independent copy anywhere or irretrievable loss.
Listings are not atomic snapshots. Objects in other prefixes, buckets, providers or
unrecognized layouts require an operator-provided inventory or separately reviewed scope.

Permission denial, missing/repeated pagination markers, exhausted page/candidate budgets
or candidate metadata failures fail the job and leave a sanitized incomplete report.
Unfinished listings leave rows `not_assessed`; they never manufacture absence findings.
Do not broaden the existing role/bucket/KMS policy on denial. Retain sanitized success
or failure results and exact-SHA/run provenance outside Actions expiration after the run.

#### Initial discovery access denial — October 26, 2026

The first authorized metadata-only discovery
[run 40385997470](https://github.com/tomacox74/js2il/actions/runs/40385997470), attempt 1,
used merged master `a3acf2307df96176a9ad6e6c9c9956ed3ab344bd`. It failed with
**`AccessDenied` at `ListObjectVersions` before the first page**. AWS role assumption
succeeded, but that does not imply effective permission to list versions. The sanitized
report does not identify which policy layer denied access; no private permission audit
is published or inferred.

The [durable sanitized denial receipt](evidence/retained-discovery-denial-40385997470.json)
records the exact revision/run/attempt/job, downloaded artifact ZIP SHA-256, original
report SHA-256, Actions expiration and execution guards. Its embedded report bytes can
be reconstructed as UTF-8 `json.dumps(report, indent=2) + "\n"` and checked against
`report_sha256`. The receipt ZIP was independently downloaded and its digest/length
matched to Actions metadata before this record was generated.

| Observed field | Result |
|---|---|
| `listing_complete` / `discovery_complete` | Both `false` |
| `pages_read` / `entries_scanned` | Both `0` |
| Unmatched recovery archives | All **33 `not_assessed`**, no candidates returned |
| Preservation / payload-readback steps | Both skipped |
| Payload reads / S3 changes / database access | All `false` |

This is an explicit access gate, **not proof that the 33 objects are absent**. Do not
repeat the same denied request, broaden the search or change role/bucket/KMS policies
without a separate operator decision. Existing eight-object full readback evidence,
the 11 original gaps and finite November 12 lock deadlines are unchanged.

At that observation, proceeding required operator-provided scoped inventory or
operator-managed access resolution and an explicitly authorized retry. The subsequent
successful retry below resolves the listing-access gate without rewriting this denied
attempt. Storage locations and policy diagnostics remain private. Metadata still does
not verify payload digests, and resolving this gate does not authorize preservation uploads.

#### Successful discovery retry — October 26, 2026

After the owner reported adding the listing policy and authorized continuation,
[run 40388618658](https://github.com/tomacox74/js2il/actions/runs/40388618658), attempt 1,
succeeded on the same master revision
`a3acf2307df96176a9ad6e6c9c9956ed3ab344bd`. Its
[sanitized durable receipt](evidence/retained-discovery-40388618658.json) records
`listing_complete=true` and `discovery_complete=true`: **one page, nine entries, zero
matching candidate versions for the 33 target archives**. All target rows now read
`no_candidate_in_scoped_listing`, not `not_assessed` for this later observation.
One scanned entry was outside the recognized layout; no claim is made about its contents.

The complete receipt ZIP was independently downloaded and its length/SHA-256 matched
to Actions metadata. Exact source/run/attempt/job and guarded step outcomes, embedded
report hash, inventory hash, all 33 source pins and timestamps were checked. The
preservation and full-payload readback steps were skipped. No payload read/decrypt,
S3 upload, import, restore, fixture execution or agent-managed permission change occurred.

The listing-access blocker was resolved **for this successful run**. At that observation,
the substantive retention gap remained: no candidate was identified under the **configured prefix and
recognized run-attempt/artifact ZIP layout** at that observation. This does not rule out
copies in other prefixes, layouts, buckets or providers, and is not evidence of global
absence or irretrievable loss. `full_archive_coverage_verified=false` and
`history_complete=false`; the 11 original source gaps remain unchanged.

The next owner decision at that observation was whether to provide an independently retained inventory
from another location, authorize a separately reviewed discovery scope, or authorize
preservation of the 33 source recovery archives still lacking retained-copy proof.
Any preservation must retain each original ZIP, including partial failed outboxes and
their WAL/SHM members, without treating those failures as acknowledged successful imports.
Do not add them blindly to the complete-report verifier's `import_report_ids`, rerun
historical imports or extrapolate the eight selected proofs to these 33 containers.

#### Authorized preservation of the 33 recovery archives

The owner subsequently authorized preservation and exact-version verification of
these **33 original recovery ZIPs** in the configured destination, under the existing
encryption and retention policy. This authorization excludes permission changes,
imports, restores and fixture execution. Permission to merge related PRs is separate
from this explicit storage-write authorization.

The `preserve-recovery` operation uses only the reviewed frozen inventory SHA-256
`ec75aea22258310fbb80f33b5c2f5c853e8be4575bf5efee565ca9ab90268bb7`.
It requires the explicit authorization token below, merged master and the existing
protected backup environment. It cannot select another manifest or substitute the
complete-report verifier's `import_report_ids`.

Before any S3 writes, it checks all 33 source artifacts' identity, run/attempt,
master revision, workflow, availability, digest and length, then downloads and
hashes every complete ZIP. Member indexes and original import-report digests and
completion flags must match the frozen inventory. It never extracts or opens SQLite.
The **68,135,697 source ZIP bytes**, **62 outbox databases**, **four WAL/SHM members**
and **four incomplete reports** are preserved verbatim. Incomplete reports remain
incomplete; preservation never manufactures import acknowledgements.

Each upload uses a new run/attempt prefix. The version returned by `PutObject` is
captured before readback, then passed explicitly to the shared exact-version verifier
with the original ZIP hash and length. Encryption must match the configured KMS key,
and COMPLIANCE retention must be current. The bucket must retain the existing
versioning and at least 35-day default COMPLIANCE protection. No retention headers,
policy changes, deletes or overwrites of earlier run/attempt objects are requested.

After all 33 archive readbacks pass, a sanitized report is itself uploaded and
exact-version verified. The outer report embeds those exact retained report bytes
as `retained_report`, with a separate public `receipt` proving their hash, length,
version and retention. `complete=true` requires both the archive readbacks and
receipt verification. This is not a claim of successful historical imports, full
history or acceptance of an indefinite retention horizon.

```sh
# Only after the preservation implementation is merged and uploads are authorized.
gh workflow run test262-cutover-evidence.yml --ref master \
  -f operation=preserve-recovery \
  -f recovery_authorization=preserve-33-recovery-archives
```

The operation's temporary session policy permits only the required bucket checks,
object upload/versioned read/retention read and KMS data-key/decrypt operations,
intersected with the existing role. A denial fails without changing permissions.
Partial reports retain exact uploaded-version identities where a response was
received, stage and sanitized errors; an attempted upload without a response may
still have reached S3. Automatic SDK retries are disabled for this operation.
Reconcile partial results before any retry rather than
duplicating uploads blindly. Existing eight-object proofs, finite November 12
deadlines and the eleven original source gaps remain unchanged.

#### Verified recovery preservation and scoped ledger — October 26, 2026

Owner-authorized [run 40393597860](https://github.com/tomacox74/js2il/actions/runs/40393597860),
attempt 1, completed successfully on merged master
`8f37ec656a3e06f95b52a57221253014b69203f3` after #2405 passed its required checks.
Only the guarded recovery-preservation step ran; legacy preservation, read-only
selected-object verification and metadata discovery were skipped.

The [authenticated sanitized receipt](evidence/retained-recovery-preservation-40393597860.json)
records **33/33 original ZIPs, 68,135,697 bytes**, with exact returned upload-version
readbacks matching source SHA-256/length, configured KMS identity and current
COMPLIANCE locks. The retained report itself also passed full exact-version readback:
**32,367 bytes**, SHA-256
`61d0fff3c0f96feadcdb3a4fa948b1bfd7fbd0032f38e8e4febb9179df3bfd42`,
protected through **November 30, 2026 UTC**. Its exact pre-receipt document is embedded
in the outer receipt, so the retained bytes can be reconstructed without a circular
self-hash claim. The Actions ZIP digest/length, source/run/attempt/job and guarded
steps, original report hash, embedded retained-report hash, all source pins and
timestamps were independently checked before retaining this public projection.

The [historical retention ledger](evidence/historical-retention-ledger-2242.json)
reconciles the frozen inventory without rewriting it:

| Scoped disposition | Verified coverage |
|---|---|
| Required containers | **41/41**, no unmatched required container |
| Earlier selected-object readbacks | **8 reused**, no re-upload or lock extension |
| Newly preserved recovery containers | **33**, including all four incomplete reports |
| Outboxes and WAL/SHM members | Original ZIP bytes unchanged; no new acknowledgements or SQLite checks |
| Original `no catalogue artifact` gaps | **11**, retained verbatim |
| Raw original private preservation receipt | Its own retained version remains unproven; authenticated sanitized Git projection remains durable |

`required_container_coverage_verified=true` refers only to these **41 frozen required
containers at their respective recorded observation times**. Full-history/archive
claims remain false; planning-only artifacts and no-artifact runs retain their frozen
separate dispositions. No imports, restores, production fixture workload, permission
change or retention-policy change occurred.

**Finite retention is still a separate limitation.** The earlier eight locks start
expiring **November 12**, and the 33 new archive locks expire **November 30**. These lock
deadlines do not cover the later known Actions source expirations; the ledger records
that comparison per container rather than claiming protection beyond Actions expiry.
An expired COMPLIANCE lock is not evidence that an object was deleted, but neither
object lifecycle behavior nor indefinite retention is inferred. Ongoing horizons,
ownership and verification cadence belong to #2343; coherent reconciliation stays with
#2342. Owner acceptance of the historical ledger does not approve an unadopted policy.
No retention extension or policy change is authorized by this preservation result.

#### Owner-accepted historical handoff — October 26, 2026

The owner explicitly accepted the **41-container preservation ledger and 11 documented
historical gaps**, authorized closure of #2242 for that bounded historical scope, and
instructed that **#2343 be reopened for longer-term retention and verification**.
The [accepted ledger](evidence/historical-retention-ledger-2242.json), SHA-256
`5b1f6081dc7b5e61229a1be44bca4afc43889b735d6f5201e32c3022c8132be4`,
and its underlying receipts remain unchanged.

The acceptance records on #2242 and #2230 distinguish completed historical preservation
from #2343's outstanding policy, horizon and verification-cadence decisions. The owner
acknowledged the **November 12/30 lock deadlines** and that protection beyond Actions
source expiry remains **unproven**. Lock expiry is not object deletion. Neither
full-history completeness nor indefinite retention, lifecycle behavior or approved
RPO/RTO targets are inferred. #2342's accepted reconciliation was not rerun.

This handoff authorizes no new storage operation, retention extension, permission
change, import, restore or production fixture execution. Future work under #2343
requires its own scoped decisions and authorization.

For an **already preserved** archive, use the separate `verify-retained` operation
of that same master-only protected workflow; do not rerun `preserve` to check
availability. It uses a read/decrypt-only AWS session policy intersected with the
existing role, the pinned original run/receipt ZIP/report digests in
`retained-evidence-receipt.json`, and the existing exact-version readback helper.
It verifies one selected original version, KMS identity, current COMPLIANCE retention,
length and original digest without uploading, importing, restoring or executing fixtures.
No persistent IAM, bucket or KMS policy is changed.

The [sanitized original receipt inspection](evidence/retained-receipt-37715977350.json)
independently checks the original eight-archive/five-outbox receipt. It records
**historical**, not fresh, retention through November 12, 2026. In particular,
artifact `11522307991` has the expected original SHA-256 and a recorded immutable
version even though its GitHub metadata is unavailable. That historical receipt alone
neither establishes current S3 retrievability nor increases
the 11 known unavailable history gaps. The subsequent read-only verification
[40251228151](https://github.com/tomacox74/js2il/actions/runs/40251228151) successfully
read back this one exact version; its sanitized receipt is retained inside the inventory
below. The legacy receipt omits lengths and explicit KMS/lock fields; reviewed source
checks those, but fresh verification must re-read the exact version.

```sh
# Receipt integrity/provenance only; requires no AWS/database access.
python -m scripts.test262.central.retained_evidence --receipt-only \
  --output /private/retained-receipt.json
# Already-reviewed read-only mode, using the existing protected role.
gh workflow run test262-cutover-evidence.yml --ref master \
  -f operation=verify-retained -f artifact_id=11522307991
```

Keep `historical_receipt_verified` separate from `selected_object_verified`.
The verification report allowlists public object version/digest/retention fields;
bucket/prefix/object keys, KMS identifiers, private database identities and audits
are not copied into it. Retain its sanitized result and exact-SHA/run provenance
in the existing issue/Git ledger before Actions expiry. Neither selected-object success
flag alone covers every original archive or useful failed outbox. The subsequent
41-container ledger supplies the separately accepted historical coverage; #2343
retains ongoing retention/recovery ownership.

### Frozen historical archive inventory (#2242)

The [durable sanitized inventory](evidence/historical-archive-inventory-2242.json)
records the October 26, 2026 observation of all **104 GitHub-listed historical import
workflow runs**, including failed/cancelled attempts and later guard skips. Both pages of
run metadata were reconciled against the workflow's total count and all 33 artifact links
in #2242. The pinned original manifest was independently hash-verified, not reconstructed
from the issue's narrative. Its **62 source entries and 11 original gaps** are preserved
verbatim; each source's ZIP and SQLite member is present in the discovery ZIP index.
The original gap reasons remain `no catalogue artifact`, not newly inferred expiration
or an assertion that recovery is impossible.

| Required evidence | Container artifact(s) identifying the retained object | Coverage/disposition |
|---|---|---|
| Original discovery manifest, 62 original source ZIPs and 62 SQLite files | `11293700607` | One historical exact-version container proof; indexed membership covers all manifest sources |
| Final five native-source import reports/outboxes | `11510693007`, `11513473147`, `11515534653` | Three historical exact-version container proofs |
| Cutover native/reconciled checkpoints | `11523270256`, `11523056094` | Each contains `native.sqlite`; separately retained containers, not additional original-manifest imports |
| Cutover isolated-restore report | `11522581310` | Contains `restore-report.json`; not a claim about every backup |
| Final retained-only legacy checkpoint | `11522307991` | Actions metadata returns 404; reuse the successful exact-version readback from `40251228151`, not a new import |
| Other 33 import/recovery containers | **No independent retained object identified** | Actions metadata/indexes remain available; not classified as lost or independently preserved |

`archives` enumerates every container/member, ZIP digest, Actions expiration, source
run/revision and `retained_object_artifact_id` mapping. Resolve that ID in
`retained_objects` for the original **`version_id`, `sha256`, `retain_until`** and
`preservation_receipt` provenance. The eight historical retention deadlines are
**November 12, 2026**, not the newer backup deadlines. A null mapping explicitly means
**proof not identified**, not that no copy could exist elsewhere. Original source
`archive_sha256`/SQLite `sha256`, container `zip_sha256`, manifest hashes and receipt
hashes retain their distinct meanings.

There are **41 required containers**, of which eight have identified retained objects.
The **36 import/recovery archives contain 67 outbox databases and four WAL/SHM members**;
only three containers/five outboxes have identified retained proofs. The four incomplete
attempts below are useful original failure/recovery evidence and remain required even
where a later import succeeded:

| Run | Recovery artifact | Outbox databases | Additional sidecars |
|---|---|---|---|
| `37177718428` | `11296419607` | 1 | WAL and SHM |
| `37188910885` | `11305023882` | 1 | WAL and SHM |
| `37237023718` | `11316103861` | 2 | None indexed |
| `37405627009` | `11386604548` | 1 | None indexed |

All four lack an identified independent-retention proof. Preserve each observed
DB/WAL/SHM set together in any separately approved preservation work. Inventorying
their members does **not** recheck SQLite integrity, payloads, acknowledgements or
successful ingestion.

The ledger also accounts for **43 planning-only artifacts** (40 blocked, three completed
selection no-ops) and **24 invocations with no artifact** (21 skipped, three unsuccessful).
Planning-only ZIPs contain no import report/outbox and are excluded from this required
original/import/outbox scope, not silently counted as import success. Runs `37219426911`,
`37364118548` and `37426152050` have no produced recovery artifact to map; the second has
a cancelled import job with no recorded steps, so its precise execution stage is unknown.
These are not additional missing-original-archive gaps.

Reports contain 63 verified source references covering 62 unique manifest sources:
`11290148901` appears twice. Neither number is a distinct-import count or replaces the
existing reconciliation of **61 distinct imports**. Supporting verification provenance is
retained in `preservation_receipt` and `reused_readback`, including the byte-hash-verifiable
sanitized single-object report. The original private preservation receipt was uploaded
off-project, but no exact self-version proof was recorded in it; the durable sanitized
Git projection does not establish retention of its private raw bytes.

```sh
# Offline: no credentials, GitHub, S3, database access or replacement ledger.
python3 -m scripts.test262.central.archive_inventory --verify \
  docs/ECMA262/evidence/historical-archive-inventory-2242.json
# Optional reproduction while source Actions artifacts remain available.
# GitHub reads only; output must be new. This does not verify or upload S3 objects.
python3 -m scripts.test262.central.archive_inventory --collect \
  --output /private/new-historical-archive-inventory.json
```

Collection uses bounded absolute ZIP byte ranges, including ZIP64 indexes, rather than
downloading the 4.28 GB history archive. `whole_zip_digest_verified` refers only to a
GitHub ZIP fully fetched within that range budget; all other outer digests are explicitly
metadata/previous-receipt pins. Indexed membership is not a fresh retained-member rehash.
At that frozen observation time, the original history container and six other selected
objects still needed separate fresh verification; the one completed readback was reused
without S3 access. Later readbacks belong in the supplemental ledger below, not a rewrite
of the original observation.

This completes the scoped inventory, **not #2242 acceptance**. Review the 33 unmatched
recovery containers for existing independently retained copies or separately authorize
preservation before Actions expiration. The existing preservation verifier expects complete
selected import reports: do not blindly add failed archives to `import_report_ids` or
pretend their partial outboxes are acknowledged. Hand the ledger and November 12 deadlines
to #2343 for retention/receipt ownership; planning/monitoring diagnostics remain coordinated
with #2341. No proposed RPO, RTO, indefinite compact retention or verbose-log horizon is
approved or deployed by this inventory.

### Exact-version readback evidence (#2242)

The [supplemental readback ledger](evidence/retained-readbacks-20261026.json) retains
the sanitized reports and source/run/attempt/artifact provenance for the seven subsequent
`verify-retained` jobs, plus the original eighth readback from `40251228151` unchanged.
The new jobs use master revision `cd90c4edda81988c7adb6b0457c760714f37da23`.
They are dispatched **serially**: the existing shared concurrency group allows only one
pending run, so dispatching all seven together could cancel pending verifications.

All seven completed successfully on October 26, 2026. Together with the reused receipt,
**eight of eight selected containers have successful exact-version readback evidence**:

| Selected artifact | Read-only verification run | Disposition |
|---|---|---|
| `11293700607` | [40286520492](https://github.com/tomacox74/js2il/actions/runs/40286520492) | Verified; all 4,279,618,183 bytes match |
| `11510693007` | [40286745826](https://github.com/tomacox74/js2il/actions/runs/40286745826) | Verified |
| `11513473147` | [40286830247](https://github.com/tomacox74/js2il/actions/runs/40286830247) | Verified |
| `11515534653` | [40286909493](https://github.com/tomacox74/js2il/actions/runs/40286909493) | Verified |
| `11522581310` | [40286989640](https://github.com/tomacox74/js2il/actions/runs/40286989640) | Verified |
| `11523270256` | [40287069451](https://github.com/tomacox74/js2il/actions/runs/40287069451) | Verified |
| `11523056094` | [40287147515](https://github.com/tomacox74/js2il/actions/runs/40287147515) | Verified |
| `11522307991` | [40251228151](https://github.com/tomacox74/js2il/actions/runs/40251228151) | Earlier verification reused, not rerun |

Each new receipt is downloaded and its complete ZIP hash matched to Actions metadata
before assembly. Its recorded original `report_sha256` is reproducible from the retained
sanitized report bytes. The offline verifier checks the exact original object version,
digest, expected ZIP length, encryption/COMPLIANCE flags, unshortened retention deadline,
report time within the successful job, complete seven-object selection and skipped
preservation step. It rejects omissions, duplicates, scope changes and private fields.
KMS identity is checked by the existing protected readback implementation; the private
configured identifier is not published.

`selected_set_readback_verified` concerns **only the eight selected containers**.
It does not establish retrieval of the 33 unmatched recovery containers, new per-member
SQLite/ZIP integrity, outbox acknowledgement, a new database reconciliation or current
trusted fixture acceptance. `full_archive_coverage_verified=false`,
`history_complete=false`, the frozen inventory and its 11 original gaps remain unchanged.
The proof is scoped to each report's `checked_at`; finite locks are not indefinite
retention, and readback does not extend their November 12 deadlines.

```sh
# Offline: no AWS/GitHub access, fixtures, dispatch, upload or policy change.
python3 -m scripts.test262.central.retained_readbacks --verify \
  docs/ECMA262/evidence/retained-readbacks-20261026.json
# Assemble only after authenticating the seven small downloaded receipts/provenance.
# An incomplete/failed set is rejected; output must not already exist.
python3 -m scripts.test262.central.retained_readbacks \
  --reports /private/readback-1.json /private/readback-2.json \
    /private/readback-3.json /private/readback-4.json /private/readback-5.json \
    /private/readback-6.json /private/readback-7.json \
  --output /private/new-readback-ledger.json
```

At this selected-set observation, 33 recovery containers were still unmatched. Their
subsequent preservation and the owner-accepted 41-container ledger above resolve that
historical coverage/handoff scope. Reopened #2343 owns ongoing retention and verification;
the accepted handoff is not approval of a retention policy. No new upload, import,
restore, fixture execution, authority transition or storage/IAM policy change is
authorized by these selected-set receipts.

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

See [retention and recovery acceptance](Test262Recovery.md) for the proposed retention
classes/objectives, coverage ownership and cadence, recovery-point/isolated-duration
measurements, retained sanitized reports, and operator-approved rehearsal/cutover/rollback
gates. The existing hourly monitor also produces a bounded metadata-only recovery
coverage inventory; unknown archive/storage proof is never treated as preservation.

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

### Retained acceptance evidence - October 27, 2026

The implementation was merged in PR #2383
(`2fcaf570ca0a60d9ac818c3017f52a6133a43d85`). The bounded, sanitized
[scheduled-recovery evidence](evidence/scheduled-recovery-2340.json) now retains
authenticated GitHub run/artifact provenance, original ZIP/member digests,
per-invocation source/identity hashes, variants, cursors, work/observation IDs,
charges, reservations and the distinct acceptance boundaries outside Actions
artifact expiry. It is an observation record, not another scheduler or a copy of
the original database/outboxes.

| Evidence | Proven scope | Not proven |
|---|---|---|
| Disposable native pilot [40027859210](https://github.com/tomacox74/js2il/actions/runs/40027859210), artifact 12532773635 | Both normal attempt/time exhaustion scenarios execute one variant, resume only the missing variant at identical source/execution identity, seal one complete fixture, then execute zero variants on repeat. Original finalization-loss, process-kill, replay and lease/fencing drills pass in their recorded disposable scopes. | GitHub cron firing those simulated invocations, or production exhaustion/recovery. |
| Actual scheduled collection [40282309564](https://github.com/tomacox74/js2il/actions/runs/40282309564), artifact 12633231943 | The October 26 schedule selected native with automatic publication disabled. Source `cd90c4edda81988c7adb6b0457c760714f37da23`; 100 acknowledged observations/completions, zero pending outbox messages/reservations. | Normal shared-budget exhaustion or same-SHA recovery: this worker stopped at its 100-attempt limit, below the allocation's 400-attempt limit. Its budget finalized `interrupted`, with 238 variants pending and zero resumed fixtures. |

The pilot's GitHub PR head is
`8f923340fb0a5c2dc04a1a4fbedd1c260a07a9a1`; its actual executed test-merge source
is `4be389cc39a771c9225e62df051be89b094ced3b`. Verified commit ancestry links them;
they must not be conflated. The partial disposable inventory, synthetic successful
master validation and simulated schedule event IDs remain explicitly labelled.
The pilot and actual scheduled collection also have different source/provenance
and cannot be combined into an actual same-SHA resume claim.

The actual collection's 100 observations are product failures, not passes or
accepted fixtures. A green workflow and collection state `complete` mean neither
full-suite success nor an exhausted/recovered allocation. The retained record
therefore leaves `actual_scheduled_normal_budget_exhaustion_observed`,
`actual_scheduled_same_sha_exhaustion_resume_observed` and
`all_issue_gates_proven` false.

At that evidence-retention point, finishing the remaining #2340 gate required
authenticating a later actual scheduled
exhaustion/resume chain at the same source and full execution identity. Confirm
that deferred required variants are resumed without repeating terminal variants,
the exhausted predecessor's charges remain unchanged, the recovery allocation
is distinct, reservations/outboxes drain and native acceptance uses only complete
matching evidence. A changed SHA/provenance, worker stop, infrastructure error
or successful job is not a substitute. Start with existing receipts; obtain
specific operator approval for any new live drill instead of dispatching one,
resetting an allocation, lowering limits or freezing production authority merely
to manufacture acceptance.

This evidence collection used GitHub artifact reads and read-only local SQLite
inspection only: no workflow dispatch, fixture execution, database/S3 access,
publication, permission change or authority change. It does not extend the
original artifacts' retention or authorize future operational mutations.

### Owner-authorized bounded live witness

For #2340 the owner explicitly accepted an authenticated bounded manual live
drill instead of the prior actual-cron witness for #2340. The fixed authorization
is `live-recovery-2340-two-variants-20261027`. This is a one-campaign authorization,
not a general screening, publication, permission or authority grant.

The manual-only `test262-recovery-drill.yml` workflow uses the protected catalogue
environment and the existing native-target concurrency group. A zero-execution
read-only verification run freezes the baseline first. Two distinct live
Actions runs are then needed because the established budget finalizer binds an
allocation to its real `actions:<run-id>` and collection to
`actions:<run-id>:1`. Custom fabricated Actions IDs, reopening an old allocation,
or a second scheduler are not used.

Before any writes, the supervisor authenticates its workflow/run/source, checks
current master validation and the matching sealed native reporting target, then
selects one currently eligible unregistered fixture with exactly the strict and
non-strict required variants. Source, upstream, binary/harness/tooling/runtime
identity and immutable fixture image must remain unchanged across both runs.
The verification run builds the image once using the source commit's
`SOURCE_DATE_EPOCH`, saves it, and authenticates its archive hash and immutable
image ID in the baseline report. Both live phases download that same image
archive, authenticate the successful baseline report/source/artifact and archive
hash before loading it, then verify the loaded image ID without catalogue
credentials. They do not rebuild or pull a replacement image.

Verification uses a repeatable-read read-only transaction with a transaction-local
read-only default and both effective settings checked before target/fixture
reads. It creates no provenance, markers, allocations, observations or work.
Both live phases must match its exact source, native and drill identities,
image/archive, fixture and control-plane digest before any catalogue writes.
The seven-day image artifact is an operational input; report artifacts last
90 days and sanitized acceptance evidence is retained in Git.

The drill has a separate explicitly labelled provenance and a private
single-fixture plan. It reuses production preparation, missing-variant planning,
claiming, credential-free container execution, outboxes and finalization, but
never changes shared reporting targets or the global reconciliation cursor.
There is no sealing/generation/publication path. Each work item has a one-attempt
limit; each phase has one attempt in its own allocation. Fixed campaign marker
runs are latched transactionally before execution and prevent another dispatch,
rerun or changed SHA from obtaining a fresh pair of executions.
These non-executing administrative marker runs are immediately cancelled; they
have no budgets, leases, observations or screening receipts and do not claim a
successful collection. Only the genuine Actions collections carry that proof.

The first phase must execute one variant, stop on its actual one-attempt budget,
drain acknowledgments/reservations and leave exactly the other variant pending.
Only after that Actions run succeeds may recovery authenticate its artifact
digest/report and verify its original live budget/marker. The second phase must
execute only the missing variant under a different allocation, reuse the first
observation, preserve the exhausted budget byte-for-field, and drain both work
items without changing the source, image or control plane. Before claims the
supervisor refuses unrelated leased work; before every physical execution it
checks the exact allowed work/variant, identity and journaled two-execution bound.
The first report freezes the full original observation's digest, including its
stored payload digest. Recovery rechecks that row and the original budget before
planning, claiming and execution, and after recovery; retaining an observation ID
alone is insufficient. The execution journal also limits each phase to one
physical execution, independently of the lifetime cap.
The no-leased-work check and claim share the existing repository advisory lock,
preventing the claim API's expiry sweep from mutating unrelated work in a race.

Operator sequence, **only after merging the guarded controls, successful
exact-master validation and the normal registration refresh**:

**The recorded campaign below is now complete and its two-execution allowance
fully consumed. These historical commands do not authorize redispatch.**

```bash
gh workflow run test262-recovery-drill.yml --ref master \
  -f authorization=live-recovery-2340-two-variants-20261027 -f phase=verify
# Authenticate the zero-execution read-only baseline and matching native identity.
gh workflow run test262-recovery-drill.yml --ref master \
  -f authorization=live-recovery-2340-two-variants-20261027 \
  -f phase=exhaust -f baseline_run_id=<verified-baseline-run-id>
# Authenticate the exact successful run/report and record its Actions run ID.
gh workflow run test262-recovery-drill.yml --ref master \
  -f authorization=live-recovery-2340-two-variants-20261027 \
  -f phase=recover -f baseline_run_id=<verified-baseline-run-id> \
  -f previous_run_id=<verified-exhaustion-run-id>
```

Do not dispatch recovery after a failed/uncertain first phase, changed master or
changed identity. Do not retry a consumed phase, reset charges, replace the
fixture or add executions after a failure. The permanent campaign latches
deliberately stop these cases; report the partial evidence and obtain a new
explicit decision if further work would exceed the original authorization.

Both runs retain `report.json`, contexts and scoped outboxes for authenticated
sanitized evidence retention. The first report has `phase_complete=true` and
`complete=false`; only the verified recovery report can have `complete=true`.
It remains a manual live witness, not evidence that GitHub cron fired. Product
failures/unsupported outcomes are not passes; no full-suite conformance or native
fixture publication is inferred. Imports/restores, S3 operations and
permission/authority changes remain outside the authorization.

#### Reproducible native build and renewed bounded authorization

After the read-only diagnostics below, the owner explicitly authorized fixing
the two-DLL drift, related squash merges after required checks, selection of a
new validated SHA and its normal scoped native registration/provenance refresh,
then one frozen two-phase live drill. The lifetime maximum remains two physical
fixture executions, including earlier attempts; the original stopped run used
zero. The same permanent campaign keys remain in use: no charge or latch reset,
automatic live retry, replacement fixture, publication, permission/authority
change, import/restore, S3 operation or unrelated live execution is authorized.

The cause is Verify `30.4.0`'s `buildTransitive/Verify.props`, which sets
`Deterministic=false` in `Jroc.Testing` and transitively in
`NativeScreeningHost`. Evaluated properties and MSBuild binary-log replay show
`/deterministic-` for precisely those two projects. Two forced builds at unchanged
source/path/SDK changed both DLLs; their only byte differences were random module
MVIDs and COFF timestamps. The other three own compiler/runtime DLLs stayed
identical. This is a controlled reproduction, not recovery of the original
historical failed bytes.

The fix explicitly sets `Deterministic=true` in those two projects after the
package props. It leaves Verify's `DeterministicSourcePaths=false` policy intact
to preserve snapshot caller paths. Two further forced unchanged builds produce
byte-identical DLLs, stable MVIDs/timestamps and reproducible PE debug entries.
[Controlled build proof](evidence/reproducible-native-build-2340.json) retains
the local SDK, commands, full hashes, PE metadata and diagnostic-only comparison.
Full raw binary hashes remain authoritative: diagnostic metadata normalization
is used only to explain the earlier drift, never for acceptance or identity
comparison. Live execution still requires the separate authenticated matching
native baseline at the final merged SHA.

#### Verified manual exhaustion and recovery, October 27

The owner's revised manual-witness acceptance is now verified at execution
source **`7058c7c55f7d401de0452d32e020e06ad9f68fb3`** (PR #2423).
Exact-source master .NET `40591322634`, MVP `40591322360`, catalogue invariants
`40591322604` and Canary Smoke `40591322358` passed. Automatic native refresh
`40591519487` has an authenticated `status=refreshed` receipt, with screening and
publication disabled. It establishes native provenance
`03560ebe-7448-5eb2-b2b5-0684793c9fe4` and registration snapshot
`594c9741-1aa8-5f4d-a53a-64ce922c1dcd`.

| Authenticated run, attempt 1 | Phase | Physical fixture executions | Verified result |
|---|---|---:|---|
| [40592440970](https://github.com/tomacox74/js2il/actions/runs/40592440970) | Read-only baseline | 0 | All nine native identity components match; fixed fixture/control plane and saved image frozen before live work. |
| [40593043603](https://github.com/tomacox74/js2il/actions/runs/40593043603) | Exhaustion | 1 | Non-strict completes, strict remains pending; genuine one-attempt allocation exhausts with zero reservations/unacknowledged evidence. |
| [40593457941](https://github.com/tomacox74/js2il/actions/runs/40593457941) | Recovery | 1 | Only strict executes under a distinct allocation; original observation/full-row digest and exhausted budget remain unchanged; both variants complete. |

Both live phases use the same validated source, full native/drill identities,
fixed fixture content/dependencies, image/archive and control-plane digest:

- native identity: `be236123cb92394b635221e4acd769bde0dec018786a7ad94c180e6e85e75339`;
- drill identity: `5f10ae49f795bc57ded167eb764b8fa5c30bbc7ddc570c75f8575cdcd34d8181`;
- immutable image: `sha256:5e4c2b04b61f8e163dfe25e8c3719bff023549fb66168672d97c5cd98a135153`;
- archive SHA-256: `147e74f5088103a02a9cef11e2ac7092b12704856185ff97a4e24e49c9989832`;
- control-plane digest: `54e32f5390fc77856890f4418142849e6194ffa9a6fe82f295c31be4df4682d1`.

The first genuine allocation, `70c8556d-f20c-5ab2-bd44-70ff78948024`, remains
`exhausted`, charged once for 1598 ms. The distinct recovery allocation,
`e29c75d8-79bb-50b0-98d7-6bb50e0fd502`, closes drained, charged once for 1194 ms.
Both have zero reservations; each scoped outbox has one acknowledged message
and completion, with no pending evidence and successful read-only integrity
inspection. Both work items have lease generation 1. The original observation
`830001fe-d04a-4216-99fb-33030146eede` is reused, not replaced or repeated.

The complete workflow inventory contains the original zero-execution stop,
this zero-execution verification and these two one-execution live phases only.
**The lifetime maximum of two physical executions is fully consumed.** Campaign
latches and exhausted charges remain intact; no retry, replacement fixture,
publication, permission/authority change, import/restore, S3 operation or
unrelated live execution occurred.

The fixed eligible fixture is
`test/built-ins/Temporal/Duration/prototype/nanoseconds/prop-desc.js`.
**Both product outcomes are `fail`, not conformance passes.** This proves
operational exhaustion/resumption, not Temporal support, native fixture
acceptance or full-suite conformance. No fixture was ported, sealed or published.
It is the owner-accepted **manual** witness, not evidence that cron fired;
earlier disposable complete-only acceptance proofs remain separately scoped.

[Durable authenticated live evidence](evidence/live-recovery-2340.json) has
SHA-256 `c285d544d95b256ee8affbb6e6f4492c8ab0887673d9adba9435bcd5512e93b2`.
It retains all run/source/attempt and artifact/member digests, frozen bindings,
before/after contexts, observation/work identities, accounting, acknowledged
outbox counts and acceptance boundaries. It does not retain credentials or raw
SQLite payloads, download the large image, or extend artifact retention. The
execution source remains the implementation SHA above, not the later
documentation-only evidence merge.

#### Recorded guarded stop, October 27

Controls PR #2417 was merged at
`08b50be490e521bbd50c63ca3799f8bb33b58707`. Required PR checks and exact-source
master CI/MVP/catalogue invariants passed. The automatic registration refresh
run `40565762524` has an authenticated `status=refreshed` receipt for that SHA,
with screening and publication disabled.

The first authorized manual phase, run `40566656556` attempt 1, **failed closed
at preflight**. The pinned supervisor rejected the combined native-target/built
identity and sealed-upstream binding at source lines 270-272, before fixture
selection, allocation/campaign-marker creation, catalogue mutations or execution.
Its authenticated report has empty execution-intent and invocation lists:
**zero fixture executions**, not an exhausted one-attempt allocation.

The compared documents/component hashes were not retained, so the exact
differing identity component is unproven. Do not infer successful exhaustion,
silently relax that equality check, or substitute job success/failure for the
missing live recovery evidence. No recovery, retry, charge reset, permission
change or additional execution was dispatched after this stop.

The [durable sanitized preflight record](evidence/live-recovery-preflight-2340.json)
retains authenticated run/artifact/member digests, the failed report/diagnostic,
successful master prerequisites and the exact registration-refresh receipt.
Its zero-execution/mutation-path conclusion is based on the authenticated report
and pinned source ordering, not an independently collected production SQL diff.
Original Actions artifact expiry is unchanged.

At this historical stop, #2340 remained open pending diagnosis and a new explicit
operator decision, without weakening provenance or changing authority. It did
not witness normal exhaustion or recovery; the subsequent owner-authorized
verified manual chain above supplies that separate evidence.

#### Read-only identity diagnosis

The owner separately authorized rebuilding the failed original source and
comparing it with the original immutable provenance, with no live fixtures,
diagnostic catalogue writes, publication, permission/authority changes,
imports/restores or S3 operations. This does **not** resume the stopped campaign.

`test262-identity-diagnostics.yml` is a protected manual-only workflow. It copies
its reviewed controller outside the checkout, then rebuilds the exact original
SHA `08b50be490e521bbd50c63ca3799f8bb33b58707` in two original profiles:
registration refresh (full history, host build) and drill (shallow history, CLI
then host build). Both use the originally observed .NET SDK `10.0.403` and Node
`24.22.1`. The comparison targets immutable provenance
`b60fd015-e157-5f39-a8a6-b26b6ff1638c`, not the moving master reporting pointer.

Database credentials exist only in the comparison step. Its connection requests
read only at startup, then starts an explicit repeatable-read read-only
transaction and sets its local default to read only. It verifies both effective
settings before any catalogue API read and performs only bounded primary-key
reads of the original provenance/corpus/registration/validation. Keeping the
assignment inside the transaction avoids relying on startup/session settings
surviving a transaction-pooling boundary; no setting escapes the transaction.
Only the native host's `--capabilities` operation is used; there is no fixture,
worker, Docker, budget, outbox, import or publication path.

The report contains component/field hashes and differing file paths/digests,
not arbitrary values, exception messages, connection details or file contents.
Unrecognized field names and unsafe paths are redacted. A successful diagnostic
means the comparison completed, not that the identities matched or live recovery
succeeded. Reproduced identities must remain distinct from the original failed
build document, which was not retained.

The initial diagnostic `40573445098` rebuilt both profiles but stopped before
the identity probe. After isolated-controller guard reporting was added in
PR #2420, diagnostic `40575448904` identified the exact stop in both profiles:
`SHOW default_transaction_read_only` was not `on`, despite the startup request.
No catalogue API read or identity probe was reached. Explicit transaction-local
enforcement addresses this observed diagnostic setup issue; it does not
change roles, permissions, authority, stored rows or any live-recovery guard.

**Authenticated comparison result:** diagnostic
[`40577213648`](https://github.com/tomacox74/js2il/actions/runs/40577213648),
attempt 1, controller `ac7ae0def5869826460addaa3f5caf07f0282994`, completed both
original-source rebuild profiles with verified server read-only settings.
[Durable evidence](evidence/identity-diagnostics-2340.json) retains authenticated
run/artifact metadata, ZIP/member SHA-256 digests, the complete sanitized reports
and both earlier diagnostic setup stops.

Both rebuilds differ from original provenance **only in `binaries`**:
`Jroc.Testing.dll` and `NativeScreeningHost.dll`. Runner, upstream, inventory,
harness, tooling (including generated C#), capabilities, recorded environment
identity and timeouts all match. There are no added/missing files or nonfile
field differences; the sealed upstream binding matches in both profiles.

| File | Original SHA-256 prefix | Refresh rebuild prefix | Drill rebuild prefix |
|---|---|---|---|
| `Jroc.Testing.dll` | `83c14567f6d4dc6f` | `24fda8cc934fe939` | `a4f3f5f16a083fc7` |
| `NativeScreeningHost.dll` | `47a8252468648b4d` | `0a0db53dd437f5d2` | `8e6a6c18d1b3e60a` |

These are 16-character prefixes; the evidence contains full file/component/root
digests. The original root is
`89e13962b4791d22b89f0cfde614b0cf15f0ac3d2aa0d36ddfc63ffcbfcc2305`.
Both rebuilt roots differ from it and from each other. Thus neither rebuild
would satisfy the original full-identity guard. This localizes **reproduced**
drift to two DLL byte streams, but does not prove which historical component
failed in `40566656556` or why the DLL bytes drift: the historical failed
document and baseline/failed DLL bytes are absent from the reviewed evidence.
Do not attribute the result to generated tooling, SDK, pin, permissions or a
specific build-profile mechanism without further evidence.

The diagnostic executed zero live fixtures and made no catalogue writes,
publication, permission/authority changes, imports/restores or S3 operations.
At that diagnostic stage the live-recovery guard was unchanged, no recovery had
been dispatched, and #2340 remained open pending a separate operator decision
and live acceptance. The later renewed authorization, build fix and verified
manual chain above are separately recorded, not retroactive diagnostic effects.

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

The GitHub-only recovery coverage artifact now includes the owner-accepted
41-container historical ledger, not the superseded selected-eight source
inventory. It distinguishes dated exact-version proofs from current S3 protection,
preserves original fields/findings and uses the separately authenticated
[42-version April 30 extension](Test262Recovery.md#october-27-authorized-retry-all-42-versions-protected-through-april-30)
for latest-recorded archive/receipt deadlines. Schema 3 retains the original
11 gaps and one unknown source expiry; it does not claim a fresh S3 read or
indefinite protection. Missing/invalid/future proof stays explicit unknown.
Ongoing archive retention findings belong to #2414. The protected preservation workflow's explicit `audit-retention` operation
adds read-only configuration and pinned-version metadata checks without payload
reads or writes. See [retention audit and draft policy](Test262Recovery.md#read-only-retention-audit);
only the separate fixed42-version horizon was approved/verified; merge permission
does not adopt broader retention policy or recovery objectives.

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
objectives. #2414 owns ongoing retention; #2415 owns recovery objectives. A sample older than
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

For rollout or subsequent changes: deploy only the exact authorized migration,
retain both existing dashboard UIDs, let the protected collector produce a sample,
and repeat representative filtered queries through Grafana at current volume.
Verify all new panels, actual
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

### Read-only acceptance rehearsal

Use the existing Grafana token and datasource; no database-owner or coordinator
credential is needed for measurement:

```sh
python -m scripts.test262.central.monitoring_acceptance \
  --repository-id "$TEST262_REPOSITORY_ID" --samples 3 \
  --output "$PRIVATE_EVIDENCE_DIRECTORY/test262-monitoring-acceptance.json"
```

Set `PRIVATE_EVIDENCE_DIRECTORY` to a private absolute directory outside the
checkout. The collector refuses checkout output, writes the completed report
atomically with mode `0600`, and never loads `.env`. Keep the report private:
it contains effective grants, execution plans, operational rows and identifiers.
Do not attach it to a public issue or PR without separate sanitization approval.

The collector compares deployed query/datasource contracts, panel IDs, variables
and navigation against the two versioned dashboards. Visual normalization is not
SQL drift. Drift prevents that dashboard's SQL from executing; it is not silently
adopted. The two reviewed performance-summary queries are checked separately.
Only fixed GET endpoints and the datasource query endpoint are used; redirects
are refused. There are no configuration calls, catalogue writes, new grants,
fixture execution, workflow dispatch, notification delivery or S3 operations.

All versioned SQL panels are sampled once. Representative operational,
conformance and performance queries receive one to three samples. Representative
or slow queries additionally get a separate `EXPLAIN (ANALYZE, FORMAT JSON,
TIMING OFF)` measurement and private plan. These are SELECT-only rehearsals,
**not server-enforced read-only transactions or a coherent cross-panel snapshot**.
Transport elapsed time, instrumented database time and browser rendering are
different measurements; browser rendering is not measured. No-data, malformed
frames, HTTP/query errors and unreviewed SQL remain distinct. Failed requests
are not automatically retried.

Exit `0` means collection completed, not that latency, operational signals or
service acceptance passed; exit `2` means drift or incomplete collection.
Every report retains `acceptance=operator-review-required` and declines a
sustained latency-SLO claim. Seven-day counts must be reviewed with actual
active days, provenance/run coverage and classification completeness; a seven-day
query against newly instrumented work is not automatically a representative week.
Retain an authenticated existing monitor artifact and the historical #2340
exhaustion/recovery receipts alongside the measurement, rather than rerunning
fixtures. #2340's lifetime live allowance is consumed and must not be reused.

### Prepared import-report optimization

`20261027230000_test262_import_reporting_performance.sql` is a separate,
reviewable performance change, **not deployment authorization**. It replaces
per-import mapping/observation aggregation in `v_catalogue_imports` with
set-based per-repository/import counts and adds a partial covering index for
observation mappings. Metadata-only projections can omit the unused count join.
Empty-import zeros, complete/partial coverage, missing or wrong-repository
uploads, legacy checkpoint/source metadata, column types, the security barrier
and existing ACLs remain unchanged. No cached evidence, counters, observations,
charges, reporting targets or authority bindings are substituted or changed.

Use the established authorized database-owner migration procedure only after
approval. Apply the whole file in one explicit transaction so both `SET LOCAL`
timeouts take effect. The index build takes a write-blocking table lock; lock
acquisition is bounded to five seconds and each statement to 60 seconds. Plan an
appropriate maintenance window, check storage/headroom, and do not retry an
uncertain deployment blindly. The read-only Grafana identity must not be promoted
to apply DDL. After deployment, repeat the private acceptance rehearsal and compare
full-row semantics and timing against retained baseline evidence.

Disposable synthetic measurements and parity tests validate the candidate, not
its production latency. Do not waive the adopted two-second target, relabel
historical repetition as unnecessary execution, clear publication ownership
warnings, or activate notification rules to make acceptance appear complete.

## Offline export reconciliation (#2342)

### Accepted snapshot and compact evidence

[Verification run 40181091117](https://github.com/tomacox74/js2il/actions/runs/40181091117),
attempt 1, succeeded for both jobs on source
`e7fc19954e91dce9d8b905757787f9dbe943e056`. The coordinator opted into
reconciliation: **32 tables / 13,178,203 records**, **48 relationship** and
**18 semantic** checks, all zero failures. Its last offline progress event
(`history`) was at **283.082 seconds**, within the 1,800-second processing
budget; this is not an interactive query SLO.

The [sanitized compact record](evidence/reconciliation-40181091117.json) and
[source pins](evidence/reconciliation-40181091117-pins.json) preserve the
snapshot, table digests, checks and warning counts in versioned Git, independently
of the original Actions artifacts' January 23, 2027 expiry. The acceptance
summary and digests are also mirrored to #2342's existing issue ledger. This selected compact
evidence preservation does not approve indefinite retention, retain the full
export, or verify historical archive preservation.

| #2342 acceptance | Evidence and exact boundary |
| --- | --- |
| Single-snapshot semantic reconciliation | Retained live record above; every exported table and defined relationship/check is present. Nonexported tables remain outside the contract. |
| Deterministic logical comparison | #2390 and #2394 frozen-input/reverse-serialization, numeric and field-mutation tests; logical version 1 is preserved. Exact stream `sha256`, whole-file `file_sha256` and `logical_sha256` are distinct fields, not interchangeable hashes. |
| Concurrent ingestion/scope isolation | PostgreSQL 17 test `test_bulk_export_snapshot_deadline_and_portal_cleanup`, scope-parity tests and [CI 40139543154](https://github.com/tomacox74/js2il/actions/runs/40139543154); the late provenance/run/observation chain and budget are excluded from the original snapshot and visible afterward. #2394 [CI 40163686943](https://github.com/tomacox74/js2il/actions/runs/40163686943) retains the scaled implementation's regressions. |
| Historical handoff | Stored receipts attribute **62 source entries to 61 distinct imports**, with no ambiguous/unmatched/orphan entries; recorded original-manifest SHA matches the existing cutover manifest. **11 unavailable gaps** remain, `history_complete=false`, `original_manifest_inspected=false`, and `archive_preservation_verified=false`. #2242 owns original-archive coverage. |
| Scoped performance compatibility/cadence | Reuse #2341's scoped performance-panel evidence and #2389's release/manual cadence and compatibility validation. Private permission audits are not republished. This does not establish newer or continuous ingestion, retrospective row equality, or complete #2341 monitoring acceptance. |
| Retained receipts and reproduction | Versioned sanitized record plus issue ledger, pinned ZIPs and offline verification command below. Private identities, permission audits and individual import identities are excluded; original payloads are not republished. |

The live record also reports **11 batches requiring refresh or marked conflicted**
and **one publication requiring attention**. These are unresolved operational
warnings owned by #2332/#2344, not failed reconciliation relationships. No repair,
publication, refund, authority change or additional execution follows from this
acceptance. #2343 owns ongoing retention and recovery decisions.

To independently verify the already-produced receipt ZIPs (no database or S3
access, export, fixture execution or publication):

```sh
gh api repos/tomacox74/js2il/actions/artifacts/12594876207/zip > /private/coordinator.zip
gh api repos/tomacox74/js2il/actions/artifacts/12593334632/zip > /private/reporter.zip
python -m scripts.test262.central.acceptance_receipt \
  --pins docs/ECMA262/evidence/reconciliation-40181091117-pins.json \
  --coordinator /private/coordinator.zip --reporter /private/reporter.zip \
  --output /private/sanitized-reconciliation.json
cmp /private/sanitized-reconciliation.json docs/ECMA262/evidence/reconciliation-40181091117.json
```

Keep downloaded originals private: they contain database/project identity and
permission-audit fields. The offline command checks both ZIP pins, source and
snapshot agreement, counts/check coverage, the version-1 logical root from
per-table digests, historical parity and the stored ledger handoff. It does not
reread the discarded 13-million-row export or independently recompute table
digests. Source jobs did that while the payload existed. After Actions expiry,
reproduction requires independently available originals; the public compact
record remains evidence, not a replacement full snapshot or archive.

Use `scripts.test262.central.reconciliation` against an **existing** export from
`Client.export`, with its independently retained receipt. This consumes the
32-table logical export contract; it does not introduce another exporter, read
PostgreSQL, import history, execute fixtures or change catalogue authority.
The newer monitoring samples, API bindings, producers, execution events, retry
decisions, derived projections and performance tables are outside that contract.
The report lists references to nonexported tables as unverified boundaries.

```sh
python -m scripts.test262.central.reconciliation \
  --export /private/snapshot.ndjson \
  --receipt /private/export-receipt.json \
  --history-manifest /private/history-manifest.json \
  --repository-name tomacox74/js2il \
  --scratch /private/temporary-disk \
  --output /private/reconciliation.json
```

The receipt must identify the repository, epoch, schema and API. Supplying its
snapshot token, as-of, counts and digest also binds those fields exactly; use the
complete retained receipt for acceptance. Missing headers/footers, mismatched
counts/digests, duplicate JSON keys, duplicate primary identities, foreign
repository rows, unknown tables and missing identity/reference fields are errors.
Exit 1 means semantic/relationship/import parity checks failed; malformed input
raises an error and produces no new success report. Never use an old report after
a failed invocation. Keep the input and report private; no source rows, payloads,
archive locations or DSNs are emitted into the reconciliation report.

A temporary SQLite file indexes the verified rows on private disk and is removed
when processing ends. It is disposable working storage, never shared authority.
Memory is bounded by the record limit (32 MiB), SQLite's 256 MiB page cache and
small report structures; disk must fit the source rows, indexes and sort workspace.
This offline workload has no claimed production runtime or interactive SLO.
It is separate from the export's 1,800-second bound. Do not repeat the completed
production export merely to exercise these checks.

### Reconciliation scope and interpretation

Every exported table has a row count, logical digest and relationship-failure
count. `reconciliation_contract.json` records primary identities and foreign-key
relationships. A PostgreSQL integration test compares its relationships with
`pg_constraint` after all reviewed migrations, so schema drift cannot silently
remove a check. Nullable foreign keys follow PostgreSQL MATCH SIMPLE semantics.
Repository identity is checked against the export header; references to omitted
repository/producer tables cannot establish credential or permission correctness.

Additional semantic checks cover fixture/corpus/provenance consistency,
observation/run provenance, completed-work evidence identity, batch-evidence
fixture/variant/provenance, target evidence kind and native validation/registration
revision, annotation content/path/pin identity, sealed inventory counts,
registration paths, reservation balances/limits and imported observation mappings.
For accepted fixtures in sealed native batches, required variants must exist and
all have bound trusted passing observations without invalidation or contradictory
trusted observations. Native batch provenance/validation/registration must agree.
The verifier does not publish or promote an accepted fixture. Conflicted and
awaiting-refresh batches remain explicit evidence, not repaired rows.

Missing payloads, observations without payloads, invalidations, conflicting trusted
variants, publication ownership/blocking and incomplete historical coverage are
reported independently. `verified=true` means the defined structural/semantic and
import-parity checks passed for this export; it does **not** mean all fixtures
pass, the entire suite completed, all payloads/archives are preserved, credentials
are safe, or historical coverage is complete. Operational run/budget/publication
repairs remain with their existing owners (#2332/#2344); reconciliation never
mutates evidence to make a report pass.

### Logical comparison versus exact file integrity

The existing footer SHA-256 covers exact header/row bytes before the footer; the
report also records the whole-file SHA-256. Keep both, the snapshot token and
as-of time. They identify a particular serialization and MVCC snapshot.

Logical digest version 1 encodes sorted object keys, exact finite decimal numbers
without insignificant scale (including normalized zero), and ordered arrays.
Each table hashes the sorted multiset of canonical row SHA-256 values, framed by
newlines. The root hashes repository/epoch/schema/API and sorted
`[table, count, table_digest]` records. Row timestamps, payloads and all other
exported fields remain content. Only snapshot token/as-of metadata is excluded
from logical comparison. Thus reordered bulk streams of the same frozen logical
input match, while a row/payload/array-order change differs. This is not a promise
that two exports from a changing live database will match, and it does not replace
exact file integrity checks. Missing and empty tables remain distinguished by the
required complete footer table set.

### Historical ledger and retained evidence

Supply the original `history.py` manifest to join each source URI to
`legacy_control_records.artifact_source` within the same export. The report retains
hashed source identities, matching import IDs, unmatched/ambiguous entry counts,
exact-file and logical manifest digests and a digest/count of unavailable gaps.
Multiple source entries may map to one immutable import. Keep the recorded
#2242 distinction: **62 processed source entries, 61 distinct imports and 11
unavailable archive gaps**. These are historical receipt totals, not constants to
force onto a different snapshot. Import parity compares expected/mapped/uploaded/
reported observations independently and leaves historical completeness false.
The join proves attribution only; it does not read retained object bytes, verify
archive hashes/versions/retention or establish recovery objectives. Those remain
#2242/#2343. Absence of a manifest is explicitly `not_supplied`, not a passed ledger
handoff. Do not substitute a newly invented manifest for unavailable source data.

Reuse the complete export receipt from verification run `39884233665` documented
in #2338/#2342. Its counts/digest are evidence for that historical snapshot, but
its discarded full payload cannot retroactively produce this new semantic report.
A disposable report validates the implementation; it is not a production semantic
reconciliation. If no original export remains, attach this verifier to the next
independently approved verification/export opportunity rather than rerunning a
full production export just because the issue is old. Keep #2342 open until the
chosen verified snapshot's semantic report and ledger handoff are retained.

The existing `test_bulk_export_snapshot_deadline_and_portal_cleanup` PostgreSQL 17
test now commits an entire late provenance/run/observation chain during export.
The original snapshot excludes the chain and the late budget; subsequent scoped
reads see all of them. It also reconciles the real exported rows and repeats their
serialization in reverse order with an equal logical digest. Existing all-table
scope-parity, permission, deadline and portal-cleanup tests remain in place.
`test_reconciliation.py` injects corruption, wrong provenance, missing variants,
legacy/conflicting/invalidated acceptance evidence and duplicate-source ledger
entries. CI retains a sanitized **disposable** report for 90 days; copy the reviewed
receipt/digest and exact source/CI references to durable operator records before
expiry, in coordination with #2343. The report explicitly identifies intentional
evidence anomalies in the shared test database; it is not a clean production
acceptance certificate.

For performance compatibility, reuse #2341's scoped datasource/grant audit,
representative performance-panel checks and intended release/manual ingestion
cadence. This verifier neither reads nor changes `public.perf_results`; its report
cannot assert performance-row equality or newer ingestion. Never infer continuous
performance ingestion from a successful SELECT or manufacture a retroactive
checksum for an earlier deployment.

### Opt-in protected reconciliation while the export exists

PR #2390's disposable proof does not retroactively reconcile a deleted production
payload. The existing **Test262 specification deployment verification** workflow
now has an optional `reconcile` input, default **false**. After review/merge, an
operator can choose that input for a separately authorized read-only verification.
No new scheduler or workflow dispatch is performed by this change.

The coordinator uses its existing bounded coherent export exactly once, then runs
the offline verifier **before the temporary export is deleted**. The reporter
check remains unchanged. The same sanitized coordinator receipt retains the
semantic checks, table digests, snapshot identity and stored-ledger handoff. A
semantic or ledger failure leaves verification failed; unknown/missing ledger
records cannot silently pass. Export and offline processing each have a 30-minute
bound; the job has a 65-minute ceiling. Offline SQLite work uses runner disk/CPU;
only the existing export reads PostgreSQL. This is a batch workload, with no
interactive latency or production completion-time claim.

The stored-ledger mode joins `artifact_source` and `history_verified_snapshot`
records already present **in that same export**, checks source/archive receipt
hash shape and observation mapping counts, and hashes the source-to-import and
gap ledgers. It preserves multiple sources mapping to one immutable import,
recorded original-manifest digests and unavailable gaps from import limitations.
Missing receipts, orphan receipts, conflicting gap lists and absent/invalid
manifest attribution fail this mode. Source/archive hashes are retained receipt
claims, not new reads of original archive bytes. This mode explicitly records
`original_manifest_inspected=false`, `archive_preservation_verified=false` and
`history_complete=false`. The separately accepted #2242 preservation ledger supplies
historical archive coverage; #2343 owns ongoing retention and verification. This
stored-ledger mode does not itself verify those independent archive proofs.

The original 4 GB archive need not be downloaded just to establish this stored
attribution join. If the original manifest is separately available, the standalone
verifier's manifest join remains useful. Retain the reviewed receipt privately
beyond Actions expiration; do not publish live database/permission metadata into
git. Existing performance-cadence and scoped permission receipts from #2341 remain
separate compatibility evidence.


### Offline processing scale and failure receipts

The first opt-in run completed its export but failed later with an offline
`OperationalError` after approximately the reconciliation deadline. That older
receipt did not retain a stage, so it cannot establish which offline operation
exceeded the bound. Do not treat the completed export or reporter job as proof
of completed semantic acceptance.

The verifier now stages only typed relationship/semantic fields plus the full
canonical row digest and primary identity. Unread payloads and uninterpreted
legacy-control documents are hashed before being discarded from the temporary
index. Every exported field still contributes to logical digest version 1;
this changes working storage, not comparison semantics or evidence policy.
Append loading defers uniqueness indexes to sorted passes, checks still reject
duplicate identities, and covering indexes serve relationship lookups without
repeated JSON parsing. SQLite's page cache is bounded at 256 MiB. The 30-minute
offline deadline and existing export bound remain unchanged.

Progress events contain stage/table/check names, row counts and elapsed times,
never source records or exception text. An offline failure now preserves the
independently verified export receipt and records safe stage/exception metadata
with `reconciliation.verified=false`. The temporary input and index are still
removed; retaining their receipt is not retaining their payload. A retry must
produce a new verified snapshot unless an original input was independently kept.

CI exercises a deterministic synthetic workload with 252,203 rows across large
eligibility, registration and legacy-control relations and retains its timing,
counts and digest alongside the PostgreSQL report. A larger operator probe is:

```sh
PYTHONPATH=. python tests/test262/central/reconciliation_scale.py \
  --fixtures 53432 --output /private/synthetic-scale
```

This synthesizes approximately the live row-count scale using no database or
production records. It is an offline performance/consistency test, not a
production acceptance receipt, archive proof or guaranteed live runtime. Review
its stage timings separately from export/network latency. After merge, the
reviewed protected opt-in run still needs a passing live reconciliation receipt
before closing #2342.
