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
timeouts or execution environment create a **new provenance**. Old evidence
stays historical. Source commit is recorded as a settings value, while actual
binary hashes—not a claim that the worktree matches a build—establish compiler
identity. Do not import old MVP `summary.json` files or the earlier 568-item list:
they lack trustworthy fingerprints and may aggregate any passing variant.

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
