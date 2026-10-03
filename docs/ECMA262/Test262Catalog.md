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
`test/language/expressions/assignment/dstr` and its subfolders, an area with
unregistered candidates in the pinned catalog; other areas are rejected at
planning time until their metadata and dependency shapes are implemented.

Create a bounded run and plan candidates from the current catalog:

```sh
python3 scripts/test262/nativePorting.py create-run \
  --trigger-revision "$(git rev-parse HEAD)" \
  --base-revision master --pin "$(git -C "$(npm run --silent test262:root)" rev-parse HEAD)"
python3 scripts/test262/nativePorting.py plan --run-id <run> \
  --catalog artifacts/test262/catalog.sqlite \
  --area language/expressions/assignment/dstr
```

The trusted C# screening host reuses `Test262SharedAssertHarness`, runs each
variant in a killable worker process, and records every completed attempt.
Strict and non-strict variants remain distinct, and runtime-negative tests must
match their declared error type. Module and compile-negative fixtures are
reported as explicit harness gaps in this first release rather than accepted
without exact phase/type evidence. Outcomes are committed transactionally and
include the fixture hash, upstream pin, compiler/runtime identity,
harness/environment identity, phase, diagnostic and failure classification.
The variant and active-execution-time budgets resume from the SQLite
checkpoint; idle time between workflow runs does not consume the time budget.
`report` keeps pending, failed, unsupported and infrastructure outcomes
separate and groups their representative paths and diagnostics.

`generate` copies accepted fixtures byte-for-byte and emits deterministic
identifier-safe C# registrations, then updates the overall, built-in and Array
coverage rows plus the changelog. `validate-patch` requires the changed path set
and every generated file hash to exactly match the generation manifest.
`checkpoint` records the report digest and cursor before durable state is
uploaded. Empty and failure-only runs still publish their database and report.

The manually dispatched `.github/workflows/test262-native-port.yml` restores
only same-repository artifacts from the catalog workflow and its own prior
branch runs. Screening has read-only permissions. Its separate publication job
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

The machine-readable report contains run/batch/base/pin provenance, candidate
and attempt counts, accepted and budget-deferred paths, incomplete paths, and
failure clusters with path, variant, phase and diagnostic. The generation and
patch-validation documents add exact output paths, hashes and the binary patch
digest; `native-manifest.json` binds those files to the originating workflow
run and target revision.
