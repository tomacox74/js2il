> Central storage implementation and deployment runbook: [Test262SupabaseCatalog.md](Test262SupabaseCatalog.md). Until explicit cutover, the artifact workflow below remains active.

# Test262 artifact catalog

The **Test262 artifact catalog** workflow stores a resumable SQLite database in
GitHub Actions artifacts, not Git. Locally the default is
`artifacts/test262/catalog.sqlite` (already ignored).

This is **MVP composite-JavaScript runner evidence**, not the native C# Test262
harness. Both harnesses can produce different outcomes. A passing catalog entry
is a porting candidate, not a guarantee that its native port passes. In particular,
the MVP runner accepts parse-negative compilation rejection without checking the
exact error type; runtime negatives must match their expected phase and error.
Unsupported module/raw/async/agent requirements and policy-excluded areas are
inventoried but never silently treated as passes. `_FIXTURE` support files are
also inventoried, classified as blocked, and never offered as passing candidates.

## Local use

Requires Node.js, Python 3 (standard-library `sqlite3`), Git and .NET 10.
Build first rather than relying on potentially stale Release output:

```sh
dotnet build src/Cli/Jroc.csproj -c Release
python3 scripts/test262/catalog.py init --expand
python3 scripts/test262/catalog.py scan --limit 20 --seconds 120
python3 scripts/test262/catalog.py export --refresh-registrations
```

`init` inventories **every pinned `test/**/*.js`**, including areas outside the
MVP intake. `--expand` adds all `test/` to the managed sparse checkout only. It
does not change the pin or MVP policy. Explicit `--root` checkouts must already
contain the complete pinned corpus and have no tracked edits. Missing files
abort initialization instead of yielding a deceptively complete inventory;
metadata parse failures are recorded individually and do not abort the scan.

`scan` defaults to at most 100 new **variants**, with a 600-second budget.
It checks the budget between variants; the in-flight variant may additionally
take up to the recorded compile/runtime timeouts. Narrow it with
`--filter built-ins/Array/prototype/at`. A count limit can stop between strict
and non-strict variants: that fixture is **incomplete**, not passing.

Each variant is committed immediately, including failures and runner errors.
Restart the same command to resume only missing evidence. Use `--retry` explicitly
to rerun recorded variants (latest evidence replaces the old result).
Compiled inputs/assemblies are removed after each variant. A hard process kill
can leave the single in-flight `catalog-work-<pid>` directory; once that process
has stopped, remove only that directory if needed. SQLite rollback journaling
protects committed evidence. Never copy a database while its worker is writing.

Re-run `init` after a build/toolchain change. Compatible evidence is retained;
different compiler/runtime binaries, harness, upstream content, runner tooling,
timeouts or execution environment identity create a **new provenance**. The
environment identity intentionally includes only compatibility boundaries: OS
family and release (Linux distribution `ID` and `VERSION_ID`, or the platform
release on other systems), CPU architecture, Node.js major version, and the selected
`Microsoft.NETCore.App` runtime version that executes the JROC compiler and
generated `net10.0` programs. Node.js minor/patch drift within major 24 and
unrelated preinstalled .NET runtimes are recorded as diagnostics only; the full
Linux `os-release` record is diagnostic, not identity. Runner image revisions and
other volatile `os-release` fields do not change the provenance hash or block
shard resume. A Node.js major, OS distribution/release, or selected .NET
execution-runtime change is incompatible and requires a new current provenance.
Old evidence stays historical. `init` requires a readable `Jroc.runtimeconfig.json`
beside `Jroc.dll` with a valid `Microsoft.NETCore.App` framework/version; missing,
malformed or unusable configurations fail before any provenance is stored rather
than creating an `unknown` runtime identity. `scan` also rejects an unusable
runtime config before running variants, while retaining the binary fingerprint
check. Rebuild JROC and run `init` again if the compiler artifact changed.
Source commit is recorded
as a settings value, while actual binary hashes—not a claim that the worktree
matches a build—establish compiler identity. Do not import old MVP `summary.json`
files or the earlier 568-item list: they lack trustworthy fingerprints and may
aggregate any passing variant.

The scan scheduler separates **global discovery** from **current-provenance
validation**. Global discovery means that a runnable fixture variant has appeared
in `results` under any provenance; this is only a cursor for choosing future
work. Current-provenance validation means that the same variant has a result for
the active provenance and is the only evidence used for current pass/fail and
conformance-style summaries. Normal bounded scans prefer globally unobserved
variants, balancing across top-level Test262 areas, while interleaving current
provenance refresh work at a four-discovery-to-one-refresh cadence when stale
variants are also available. Historical evidence can guide breadth, but it never
inflates current conformance.

## Exports and completeness

`export` writes deterministic sorted outputs:

| File | Meaning |
| --- | --- |
| `passing-unported.txt` | All required variants matched under the **current single provenance**, excluding currently registered native tests. |
| `historical-passing-unported.txt` | A complete all-variant pass under an older single provenance, but no current complete pass. Not current evidence. |
| `incomplete.txt` | Missing required variant evidence, unsupported/policy exclusions, support files, and metadata errors. |
| `unrun.txt` | Runnable fixtures with no current variant results. |
| `registered.txt` | Full upstream paths resolved from native registrations. |
| `failures.csv` | Current unexpected variant results and selection/metadata blockers. |
| `summary.json` | Counts, completeness flags and complete fingerprint document. |
| `catalog.sqlite` | Inventory, per-variant results, provenance history and current registration inventory. |

`inventory_complete` means all pinned JavaScript paths are present, **not** that
they have been executed. `runnable_scan_complete` covers only MVP-runnable
fixtures. `complete_passing_unported_list` remains false while any inventory
entry is unresolved/blocked. Thus even a fully scanned MVP cannot claim a
complete native-harness pass list across the full Test262 corpus.
`globally_observed_variants` and `globally_unobserved_variants` describe discovery
breadth across all provenances. `current_provenance_recorded_variants`,
`current_provenance_missing_variants`, and `current_provenance_scan_complete`
describe only the active provenance. Treat historical passing lists as porting
leads, not as current compatibility evidence.

Registration inventory resolves literal `ExecutionTestFromFile`, legacy
`ExecutionTest`, and `CompilationFailureTest` calls in all C# files under
`tests/Jroc.Test262.Tests`, requiring their caller-relative `JavaScript` fixture
to exist. Nested fixture paths are preserved; identically named tests in other
spec folders do not suppress candidates. Commented calls are ignored. Missing
fixture references are reported in `summary.json.registration_warnings`.
This is a source registration inventory, not discovery of executed xUnit cases;
new dynamically computed registration conventions must extend the extractor.
Refresh it independently of execution with `export --refresh-registrations`.

## GitHub artifact workflow

After these files are pushed, manually run **Test262 artifact catalog** in
Actions (or `gh workflow run test262-catalog.yml --ref <branch>`). The workflow
also runs weekly on the default branch. It never runs on pull requests.
Changes to the catalog tooling on `master` or `test262/artifact-catalog` also
trigger a bounded run, allowing initial artifact publication before the workflow
is merged and becomes available for manual dispatch.

- Four deterministic SHA-256 path shards, each with an independent database.
- Default maximum 100 new variants / 600 seconds **per shard**; dispatch inputs
  allow a filter and up to 10,000 variants / 1,200 seconds per shard.
- A single current build is shared across shards.
- Restores the latest non-expired aggregate from this workflow on the same
  branch. Initialization reuses compatible evidence and isolates older evidence
  as historical; registration changes are refreshed.
- Serializes runs on each branch to prevent lost updates.
- Checkpoints are uploaded even on worker failure; aggregation merges available
  shards without inventing results for absent shards.
- Each prepare/scan job logs the compatibility identity plus diagnostic Node.js
  and selected .NET runtime versions, so runner-image drift can be diagnosed
  without becoming execution identity.
- Download **test262-catalog** for the database and exports (90-day retention).
  Per-shard/base/compiler artifacts have 14-day retention.

A workflow file existing locally does **not** mean an artifact has been uploaded.
An Actions run is required. Artifact expiration loses that restore point; download
the database for longer-term archival if needed. Repeated bounded runs on the
same fingerprint advance the inventory; changed builds begin current evidence
again, retaining historical passes. A small seed is useful but is not a full scan.

For manual parallel scans, copy a closed initialized database into one file per
worker, then use `--db <file> scan --shard N --shards M`. Merge only after workers
stop:

```sh
python3 scripts/test262/catalog.py merge artifacts/test262/shard-0.sqlite artifacts/test262/shard-1.sqlite
python3 scripts/test262/catalog.py export --refresh-registrations
node --test scripts/test262/catalog.test.js
```

Merge rejects shards with different current provenance, is idempotent, and picks
the latest timestamped result for overlapping explicit retries (a deterministic
document tie-break handles identical timestamps). It does not union strict and
non-strict evidence from different builds.

## Native porting automation

`scripts/test262/nativePorting.py` and
`scripts/test262/NativeScreeningHost/` are the separate native-evidence and
publication boundary for the merge-triggered porting design. MVP catalog rows
are selection hints only; they are never copied into native acceptance. The
first release intentionally supports
`test/language/computed-property-names/basics` and its subfolders, an area with
unregistered historical single-provenance candidates in the pinned catalog;
other areas are rejected at planning time until their metadata and dependency
shapes are implemented. Historical MVP outcomes remain selection hints only;
every generated fixture still requires fresh native acceptance.

Create a bounded run and plan candidates from the current catalog:

```sh
python3 scripts/test262/nativePorting.py create-run \
  --trigger-revision "$(git rev-parse HEAD)" \
  --base-revision master --pin "$(git -C "$(npm run --silent test262:root)" rev-parse HEAD)"
python3 scripts/test262/nativePorting.py plan --run-id <run> \
  --catalog artifacts/test262/catalog.sqlite \
  --area language/computed-property-names/basics
```

The trusted C# screening host reuses `Test262SharedAssertHarness`, runs each
variant in a killable worker process, and emits each completed attempt as a
flushed result record. Results are newline-delimited JSON: the host writes
exactly one compact record per stdout line, and the workflow imports each line
independently, so parent cancellation preserves completed evidence and consumed
budgets rather than waiting for the whole plan to finish. That producer/consumer
contract is covered end to end by
`scripts/test262/nativeScreeningHost.test.js`, which runs the built host and
imports its real stdout.
Strict and non-strict variants remain distinct, and runtime-negative tests must
match their declared error type. Module and compile-negative fixtures are
reported as explicit harness gaps in this first release rather than accepted
without exact phase/type evidence. Outcomes are committed transactionally and
include the fixture hash, upstream pin, compiler/runtime identity,
harness/environment identity, phase, diagnostic and failure classification.
The active compiler, harness and environment identity is persisted before
screening; resume, report, generation and publication accept only matching
evidence.

### Native capability matrix

The screening host exposes a machine-readable capability report with
`NativeScreeningHost --capabilities`, uploaded as
`native-capabilities.json` by the workflow. The current native contract
supports async completion, strict/no-strict variants, agent cleanup, the
registered Test262 helper set reported by the capability document, ES module
entries whose static import/export syntax establishes module goal (including
relative sibling dependencies), sibling fixture files and harness
files. MVP-blocked async, agent, CanBlock and module fixtures remain eligible
for native selection when they have no other unsupported reason. Unsupported
raw fixtures, module-flag entries without static module syntax, unknown helper
includes, and parse/early/resolution-negative fixtures remain explicit
`harness-gap` outcomes; they are never accepted as passes. Worker isolation
and timeouts apply to every variant, and agent state is disposed with the
worker runtime. Focused host integration tests verify successful async and
agent completion, module dependency loading, agent failure propagation,
process-tree timeout cleanup and a clean success after those failures.

This matrix is an execution capability declaration, not an assertion that
every fixture using a supported shape passes. Product failures remain
`unresolved` and are reported separately from harness gaps.

### Validated-master retry intake

The native workflow listens for a successful `test262 MVP` validation on a
master push and checks out the exact validated `head_sha`; an hourly recovery
schedule resolves the newest successful master-push validation rather than
assuming the current branch tip has passed. The SQLite state records the last
durably reconciled validated revision. Each wake-up diffs its complete
unprocessed ancestor range; first runs and missing/force-pushed ancestors use
an explicit one-commit bounded baseline and report that attribution.
Registration/coverage-only generated batch merges produce an empty selection.
Scheduled and manual recovery can resume durable pending work without relying
on Actions concurrency as a FIFO.

Candidate planning maps changed compiler/runtime/harness paths to callable,
private-member, property, iterator/Promise, regex and native-capability areas.
It assigns 80% of the global candidate budget to relevant current or historical
MVP failure hints and rotates the remaining 20% through fallback/discovery
candidates, redistributing unused retry capacity. Selection remains within one
coherent language or built-in area, preserves its reason, admits native-capable
fixtures blocked by MVP policy, and uses historical passes only to fill spare
capacity. Deferred candidates remain in `pending_work` after the merge cursor
advances. All selected evidence is still re-executed by the native host under
the active compiler, harness and environment provenance before generation.
Reports include build, planning, screening and generation/validation stage
durations plus accepted tests per active screening hour. These measurements are
evidence for future concurrency decisions; this workflow still permits only one
native-porting writer and does not create product-failure issues automatically.
The variant and active-execution-time budgets resume from the SQLite
checkpoint; idle time between workflow runs does not consume the time budget.
`report` keeps pending, failed, unsupported and infrastructure outcomes
separate and groups their representative paths and diagnostics.

`generate` copies accepted fixtures byte-for-byte and emits deterministic
identifier-safe C# registrations, then updates the overall language and
computed-property-name coverage rows plus the changelog. `validate-patch`
requires the changed path set and every generated file hash to exactly match
the generation manifest.
`checkpoint` records the report digest and screening cursor before durable
state is uploaded. The validated-master cursor advances only after that
checkpoint artifact succeeds, then a second reconciled-state artifact publishes
the advanced cursor. If either upload fails, the next run restores the older
unreconciled state and cannot discard the range or pending work. Empty and
failure-only runs still publish their database and report.

The manually dispatched `.github/workflows/test262-native-port.yml` restores
only same-repository artifacts from the catalog workflow and its own prior
branch runs. It considers screening and publication checkpoints together in
artifact creation order, including checkpoints preserved by failed screening
runs, and restores the newest checkpoint whose SQLite integrity, schema version
and table inventory validate. Screening has read-only permissions. Its separate publication job
verifies the producing workflow/run, exact target revision, artifact and patch
digests, and that `master` has not advanced; it never executes generated
fixtures with write credentials. It permits one open
`test262/native-porting-*` PR repository-wide, reuses an unchanged batch branch,
and refuses to overwrite a changed branch or recreate a closed batch.

Publication requires a repository-scoped GitHub App installed on this
repository with **Contents: Read and write** and **Pull requests: Read and
write** permissions. Store its application ID and private key as
`TEST262_PORTING_APP_ID` and `TEST262_PORTING_APP_PRIVATE_KEY`. The App identity
is used so the pushed branch triggers normal PR workflows. No secret is needed
for the default dry-run, which can only upload reports and resumable state.

This initial slice intentionally screens individual variants. Temporary-group
screening and bounded compilation-failure bisection remain follow-up work, as
does native intake of MVP failures without pass history. Actual GitHub App
publication and normal PR-CI acceptance must be exercised after the workflow
is available on the default branch; until then publication is staged and
unverified.

The machine-readable report contains run/batch/base/pin provenance, candidate
and attempt counts, accepted and budget-deferred paths, incomplete paths, and
failure clusters with path, variant, phase and diagnostic. The generation and
patch-validation documents add exact output paths, hashes and the binary patch
digest; `native-manifest.json` binds those files to the originating workflow
run and target revision.
