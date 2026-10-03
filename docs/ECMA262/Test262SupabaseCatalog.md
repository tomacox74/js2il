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

The selected production project currently remains `schema-only`, epoch 1. SQL API
regressions were rehearsed there in rollback-only subtransactions: neither API objects
nor synthetic catalogue rows were retained. The repository CI applies both migrations
to an empty PostgreSQL 17 database and runs concurrent claim/budget and privilege tests.
The rollback rehearsals are validation entries in migration history, not deployments.

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
host PID namespace, capabilities, secret environment or credential/outbox mounts. Only
repo and pinned source are read-only mounts; one disposable work directory is writable.
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
| `api_transition` | Exact primary key + version/CAS; allowlisted mutable fields; inventory digest and fresh native seal checks |
| `api_reconcile` | Verified plan digest including next cursor, cursor CAS and durable queue insertion in one transaction; recovery identity differs from SHA |
| `api_snapshot` | Explicit REPEATABLE READ transaction; coherent MVCC export, not a sequence-max incremental cursor |

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
harness gaps. Contradictory trusted observations or invalidated evidence mark an already
sealed batch conflicted; advancing its reporting target marks it awaiting refresh.
Publication requires a fresh sealed batch, reserves a unique branch/PR centrally, adopts
a crash-after-push only after matching parent/commit marker/exact patch, and detects head
ownership changes. It never automatically merges or claims required PR CI has passed.

## Import history before cutover

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
schema. Also archive original imports and failed-run outboxes; 90-day Actions artifacts
are recovery convenience, not the long-term backup policy.

Perform a restore drill into a **new empty disposable** PostgreSQL 17 database:

```bash
python -m scripts.test262.central.backup restore \
  --archive downloaded/catalogue.dump --manifest downloaded/manifest.json
```

Set `TEST262_RESTORE_DATABASE_URL` through the secret manager. The tool refuses a database
containing catalogue schemas or `public.perf_results`, verifies the digest and exact
snapshot counts, queries restored views and checks constraints/permissions. It disables
all restored credential bindings, increments epoch and leaves authority shadow. Archive
the drill report off-project and schedule repeat drills independently. Credentials and
role memberships must be reprovisioned explicitly; restoring a dump must not reactivate
workers. A backup job's successful upload is not proof of a completed restore drill.

## Validation

```bash
python -m unittest discover -s tests/test262/central -p 'test_client.py' -v
node --test scripts/test262/catalog.test.js scripts/test262/nativePorting.test.js
node --test scripts/test262/nativeScreeningHost.test.js
# Empty disposable PostgreSQL only; CI supplies a PostgreSQL 17 service:
python -m unittest discover -s tests/test262/central -p 'test_*.py' -v
```

The client tests cover lost acknowledgments, durable completion replay, scope binding,
secret environment exclusion, inventory identities and read-only import digests. SQL
regressions cover immutable replay, run/trust assignment, collision rejection, fenced
settlement, stale epochs, incomplete/untrusted native acceptance, conflicts and blocking
publication. The PostgreSQL test service also exercises simultaneous workers against
one shared attempt/time budget and a real restricted login. Full production activation,
artifact history parity, a live reporter connection, an S3 retention configuration and a
restore drill must be recorded on #2230 with actual evidence before closing it.
