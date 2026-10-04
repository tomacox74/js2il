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
reconciled: an owned PR is adopted, an owned orphan branch is deleted and the reservation
cancelled, and a foreign or unverifiable branch/PR blocks it for manual review. None of
these keep the single-active-publication slot. It never automatically merges or claims
required PR CI has passed.

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

## Automatic historical import batches

The existing history workflow also runs hourly on the default branch. Automatic
imports are **disabled by default**. Configure these repository **Actions variables**
(the existing importer environment still supplies the dedicated database secret):

| Variable | Purpose |
|---|---|
| `TEST262_HISTORY_AUTO_IMPORT` | Set `true` to enable; unset or `false` to pause |
| `TEST262_HISTORY_ARCHIVE_RUN_ID` | Successful preserved discovery run; initially `37175509314` |
| `TEST262_HISTORY_MANIFEST_SHA256` | Pin preserved manifest bytes; initially `dd708f3ce7ac05d92bf4b97a4f0680c4a6b7c383dc8ec120198ce22a9febfe1c` |
| `TEST262_HISTORY_AUTO_KIND` | `mvp` (default), `native`, or `all` |
| `TEST262_HISTORY_AUTO_BATCH_SIZE` | 1–5 snapshots per run, default 2 |
| `TEST262_HISTORY_TRACKING_ISSUE` | Progress issue number, default 2242 |
| `TEST262_HISTORY_AUTO_RESUME_RUN_ID` | Exact failed automatic run ID, only after diagnosis, to acknowledge retry |

Enable after merging and configuring the archive/hash, initially using `mvp` and a
batch size of 2. Each scheduled invocation uses the same concurrency group as manual
imports. Schedule timing is best effort; it does not chain dispatches or require an
Actions-write token. Protected environment approval rules still apply.

A successful source now gets an immutable `history_verified_snapshot` receipt in
`legacy_control_records` **after** central observation/mapping parity and unchanged
source bytes are verified. Scheduled selection skips only matching source URI and
ZIP/SQLite hashes; imported counts alone do not qualify. Previously imported sources
without these receipts (including the original pilot) are safely replayed once to
perform verification and create the receipt. No schema migration is needed.

Any failed/cancelled automatic run blocks later automatic batches, even after many
successful blocked/no-op runs. Inspect its preserved report/outbox, resolve the
failure, then set `TEST262_HISTORY_AUTO_RESUME_RUN_ID` to that run's ID. A new failure
requires a new acknowledgement. Turning the enable variable off pauses future
starts; cancel an already-running job explicitly if it must stop immediately.

When all matching archived snapshots have receipts, a completion marker avoids
further archive downloads/imports for that archive/hash/kind. Hourly jobs may still
perform lightweight checks; set the enable variable false, or select the next kind.
Completion of `mvp` does not cover `native`. `history_complete=false` remains explicit
for unavailable history. Scheduled runs append bounded milestone comments to the
tracking issue and save a job summary, planning report and recovery artifact; they
do not overwrite other agents' issue edits. Preserve evidence outside Actions expiry.
An archive with more than 100 matching snapshots fails closed for manual splitting.

## Isolated worker pilot before cutover

`.github/workflows/test262-worker-pilot.yml` runs automatically on changes to the
pilot/worker/image in a PR, or manually after merge. It uses an empty PostgreSQL 17
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

Review the `pilot-report.json` and `test262-worker-pilot-<run>-<attempt>` artifact before
recording the isolated-execution item in #2230. Evidence stays in the disposable run;
never upload this partial pilot corpus to production or activate Supabase from this job.
