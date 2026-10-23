# Bounded Differential Test Harness

The [Node compatibility measurement baseline](../../docs/nodejs/compatibility/README.md)
defines the pinned reference, API/behavior identities, outcome vocabulary and
denominators for Node-specific evidence. The opt-in
[compatibility evidence mode](#node-compatibility-evidence-mode) extends this
harness without changing legacy corpus/canary behavior. Passing a selected
corpus is not an overall Node.js compatibility percentage.

Detects semantic regressions by running every JS program under **both Node.js
and JROC** and comparing the observable output (stdout, exit code).  Any
mismatch is a potential compiler bug.

## Quick start

```sh
# Run fixed corpus only (fast, suitable for CI)
npm run diff:test

# Run the small real-world canary PR gate
npm run diff:test:canary

# Run the packaged-tool PR gate (useful before releases)
npm run diff:test:canary:packed

# Run the PR gate plus the expanded nightly canaries
npm run diff:test:canary:nightly

# Run the packaged-tool nightly canaries
npm run diff:test:canary:nightly:packed

# Run corpus + 50 generated programs (weekly-style)
npm run diff:test:generate

# Custom options
node scripts/differential-test/run.js \
  --corpus  scripts/differential-test/corpus \
  --timeout 15 \
  --generate 100 \
  --seed 1234 \
  --verbose
```

## Options

| Flag | Default | Description |
|------|---------|-------------|
| `--corpus <dir>` | `./corpus` | Directory of `.js` seed programs |
| `--timeout <secs>` | `10` | Per-execution wall-clock limit |
| `--compile-timeout <secs>` | `2× --timeout` | Compilation wall-clock limit |
| `--generate <n>` | `0` | Also run `n` generated programs |
| `--seed <n>` | `42` | RNG seed for generated programs (deterministic) |
| `--output <dir>` | OS temp | Scratch directory for compiled DLLs |
| `--jroc <path>` | auto-detected | Path to `Jroc.dll` or `jroc` executable |
| `--verbose` | off | Print full diff for every test (including passes) |

## Corpus

| File | Risk area |
|------|-----------|
| `cf-ternary.js` | Control-flow joins via `?:` |
| `cf-logical.js` | `&&` / `\|\|` value semantics & short-circuit |
| `loop-for.js` | `for`-loop back-edges and accumulator patterns |
| `loop-while.js` | `while`-loop with conditional variable updates |
| `numeric-int.js` | Integer `+ - * / % **` |
| `numeric-float.js` | Float / NaN / Infinity / boxing boundary |
| `array-index.js` | `Array.length` and index arithmetic |
| `array-mixed.js` | `map / filter / reduce / indexOf` |

## Canary suites

The canary suites live under `scripts/differential-test/corpus/canary/` and validate committed expected output rather than just Node-vs-JROC parity.

- **PR gate**: `npm run diff:test:canary`
  - `dromaeo-object-array-modern.js`
  - `dromaeo-object-regexp.js`
- **PR gate against packed tool**: `npm run diff:test:canary:packed`
  - Same PR-gate cases, but installs and exercises the locally packed `jroc` tool package
- **Nightly-expanded**: `npm run diff:test:canary:nightly`
  - PR gate cases, plus:
  - `array-stress.js`
  - `stopwatch-modern.js`
- **Nightly against packed tool**: `npm run diff:test:canary:nightly:packed`
  - Same nightly cases, but through the locally packed `jroc` tool package

Each canary consists of a `.js` driver plus a sibling `.expected.txt`. The runner compiles the driver with JROC, runs both Node and the compiled assembly with bounded timeouts, and requires both to match the committed expected stdout.

## Generated programs

`generate.js` produces deterministic JS programs from five templates that
cover the same four risk areas.  Pass `--generate N` to add N programs on top
of the corpus.  The same `--seed` value always yields the same programs.

```sh
# Write 20 programs to /tmp/gen without running them
node scripts/differential-test/generate.js --seed 7 --count 20 --output /tmp/gen
```

## CI integration

`differential.yml` wires this harness into GitHub Actions:

* **Weekly** – runs corpus + 50 generated programs (scheduled Monday at 02:00 UTC).
* **Canary smoke** – `canary-smoke.yml` runs the small real-world canary gate on pull requests/pushes and the expanded suite on a nightly schedule against the locally packed `jroc` tool package.
* **Manual** – `workflow_dispatch` accepts custom `seed` / `generate` inputs.
* To keep PR CI fast, this workflow is intentionally **not** triggered on pull requests.
* If the scheduled weekly run fails, the workflow opens a GitHub issue automatically.

## Output normalisation

Stdout lines are trimmed of trailing whitespace and compared verbatim.  When
both processes exit non-zero the JS-level error type+message is extracted and
compared so that Node's V8 stack trace and the .NET runtime trace do not
produce spurious mismatches.

## Reproducing a failure

Every mismatch is printed with the **file path** (corpus) or **full program
source** path (generated) so you can reproduce it locally:

```sh
# Re-run a single failing program
node   scripts/differential-test/corpus/loop-for.js
dotnet src/Cli/bin/Release/net10.0/Jroc.dll scripts/differential-test/corpus/loop-for.js -o /tmp/out
dotnet /tmp/out/loop-for.dll
```

## Node compatibility evidence mode

The [first public-API behavior corpus](../../docs/nodejs/compatibility/BehaviorCorpus.md)
is available with `npm run diff:test:node:public-api`. It retains original
assertions, upstream discovery/triage provenance and fixture/dependency hashes
through optional `corpusMetadata`, verified both before execution and during
offline readback. The default command below remains runner validation only.

Issue [#2373](https://github.com/tomacox74/js2il/issues/2373) adds an opt-in mode
to this entry point, reusing compiler discovery and invocation from the legacy
corpus/canary runners. It exercises the public JavaScript CLI compilation path,
not direct C# runtime calls.

```sh
nvm use 24.22.1
npm ci --ignore-scripts
dotnet build src/Cli/Jroc.csproj -c Release

# Requires the exact reference profile and a clean committed checkout.
npm run diff:test:node

# Target one registered scenario and retain every attempt, even after a pass.
npm run diff:test:node -- \
  --case node:scenario/runner/observable-smoke --attempts 2

# Local edits only: explicitly diagnostic, never trusted baseline acceptance.
npm run diff:test:node -- --allow-dirty

# Portable schema/comparison/process tests; no exact .NET reference required.
npm run test:node-compatibility

# Optional end-to-end classification tests, requiring the exact reference and
# built CLI. Deliberate negative fixtures are expected; the test asserts them.
npm run test:node-compatibility:integration
```

The exact Linux x64/Node/.NET profile is defined by
[`baseline.json`](../nodeCompatibility/baseline.json). Do not silently substitute
a moving Node/.NET patch version. `--allow-dirty` relaxes only worktree
cleanliness, not the pinned runtime checks. The portable tests run in the
existing PR canary workflow; broad compatibility reporting/gates remain #2378.

| Evidence-mode option | Meaning |
| --- | --- |
| `--manifest <json>` | Versioned executable scenario manifest; defaults to `corpus/node-compatibility/manifest.json`. |
| `--case <stable-id>` | Select a scenario; repeat for several. Unknown IDs fail explicitly. |
| `--attempts <1..5>` | Run every selected case this many times, not just retry until green. |
| `--jroc <dll>` | Built CLI DLL; otherwise use existing built Release/Debug discovery. This mode does not build or install a global tool implicitly. |
| `--output <dir>` | Parent of a fresh UUID run directory; default `artifacts/nodeCompatibility/runs`. Prefer an ignored directory or one outside the checkout. |
| `--allow-dirty` | Allow diagnostic local changes, recorded in provenance. |

### Scenarios and comparison policy

The manifest, result and bundle contracts live in
[`execution.schema.json`](../nodeCompatibility/execution.schema.json), separate
from the inventory/reference-environment contracts. Each scenario has a stable
`node:scenario/...` ID, inventory API/module/behavior targets or explicit
`node:workflow/...` targets, entry file, complete fixture-file list, completion
budgets with rationale, expected reference exit status, normalization and named
observables. Optional committed stdout/stderr expectations additionally validate
the reference. Fixtures must assert the intended behavior; a successful process
exit alone is not whole-API conformance.

Applicability is an explicit `applicable`, `unknown` or `excluded` decision with
a reason against the manifest's exact `referenceProfileSha256`. Unknown/excluded
cases retain `not-run` observations and never execute. Missing JROC features
are gaps, not applicability exclusions. A profile upgrade requires reviewing
these decisions, not silently treating them as universal platform support.

Reference and product compare exit status, signal, stdout and stderr. Default
`exact` comparison preserves every byte, including trailing whitespace. The
only optional stream normalization is `crlf-to-lf`; it does not trim or decode
arbitrary binary data. Expected JS errors should be caught/asserted in fixtures
and exposed as type/code/message data, rather than erasing meaningful errors
or comparing unrelated V8/CLR stack frames.

Fixtures can write named observable files in their working directory:

| Format | Comparison |
| --- | --- |
| `bytes` | Exact binary contents, suitable for filesystem effects, buffers and protocol bytes. |
| `json` | UTF-8 JSON values compared with type-sensitive deep equality; object-key order is irrelevant, array order is meaningful. |
| `event-order` | A JSON array of unique event IDs. Both runs must contain the same events and satisfy the declared `order` pairs; unconstrained scheduling order is not compared. |

Use explicit tagged JSON values for `undefined`, NaN/infinities, negative zero,
BigInt and other values JSON cannot represent losslessly. Do not allow
`JSON.stringify` to silently turn them into null or lose them. The runner does
not compare exact timing, manufacture random-value equality, or discard
error codes, side effects or required event ordering.

The committed smoke scenario is an original **runner-validation fixture** for
typed values, error fields, stdout/stderr, bytes/effects and ordering. It is not
the public-API behavior corpus (#2374), does not execute the inventory's eight
pending loader registrations and does not establish an API compatibility score.
Integration tests deliberately force mismatches/timeouts using host argv
differences, a self-contained data-URL compile failure, unsupported eval,
reference failures and resource limits; these are harness tests, not newly
reported JROC regressions.

### Isolation, budgets and failure classification

Each attempt copies the same frozen source bytes into separate temporary
reference, compilation and product workspaces. It snapshots the compiler DLL,
dependency DLLs and runtime configuration, then invokes the same CLI command
builder used by canaries with a structured compilation-coverage report.
The product runs the generated assembly with `dotnet`.

All three phases use a closed environment allowlist: executable directories
plus `/usr/bin:/bin` for PATH, fresh HOME/DOTNET_CLI_HOME/temp directories,
`LANG/LC_ALL=C.UTF-8`, `TZ=UTC`, .NET no-logo/first-use/telemetry opt-outs and
`DOTNET_EnableDiagnostics=0`. Parent GitHub, database, publication, npm/NuGet
credentials, runtime injection flags and proxy variables are not forwarded.
Effective command/arguments, working directory and the full non-secret
allowlisted environment are recorded for each process.

Each process has its own Linux process group. Timeout, resource/output failure,
normal completion and supervisor SIGINT/SIGTERM terminate its remaining group;
workspaces are removed after an attempt. Current limits are 1 MiB combined
stdout/stderr per phase (partial bytes retained), 1 MiB per fixture/observable
file, 64 MiB and 256 filesystem entries per workspace, and 128 MiB/4,096
retained files per bundle. Workspace checks run every 50 ms and at completion;
they are sampled safeguards, **not hard disk quotas**. Symlinks/special files
are a visible harness limitation, not a product failure. Larger package
workloads require a deliberately reviewed profile, not unlimited defaults.

This is process/workspace isolation for **reviewed trusted fixtures**, not a
sandbox or protection against hostile filesystem/network access. Use local or
loopback services only; do not use uncontrolled external endpoints. SIGKILL,
host loss and deliberate process-group escape are outside these cleanup
guarantees.

A valid reference is required before any product outcome. Reference timeouts,
unexpected exit/committed output, malformed or missing reference observables
are `reference-failure`; spawn, capture, cleanup and resource failures are
`infrastructure-failure`. Compilation timeout is `inconclusive`, not a pass.
An explicit consistent compiler report with unsupported source sites identifies
`unsupported`; another ordinary compiler rejection is `compile-failure`.
Missing/malformed compiler reports or successful compilation without a DLL are
infrastructure failures. Runtime stub exceptions remain ordinary mismatches
unless a future reviewed structured capability mechanism establishes otherwise.

After a healthy reference and successful compilation, product timeout against
the declared suitable budget is `execution-timeout`. Product error/status,
missing/invalid product observables or different meaningful data are
`mismatch`. Resource-budget failures stay infrastructure failures. Conflicting
attempt outcomes produce a `flaky` case summary while preserving every result.
The CLI exits nonzero for any non-pass summary, including not-run/flaky cases;
legacy corpus/canary exit rules are unchanged.

### Evidence layout and offline readback

Each UUID run contains an environment and inventory snapshot, the complete
executable manifest/fixtures, runner sources/lockfile, frozen compiler
dependencies, and immutable per-attempt raw output, observables, compiler report,
generated assemblies/configuration and typed `result.json`. Files use exclusive
creation; attempts and runs never overwrite earlier evidence.

`bundle.json` references every retained file by canonical relative path,
SHA-256 and byte length; `bundle.sha256` checks its exact bytes. A missing final
manifest/checksum or failed validation means an incomplete run, not successful
zero-test evidence. Setup failures log explicitly and may leave an incomplete
diagnostic directory. Interrupted controllers may leave scratch directories;
never treat them as accepted results.

```sh
node -e 'require("./scripts/differential-test/compatibility").verifyBundle(process.argv[1]); console.log("verified")' \
  artifacts/nodeCompatibility/runs/<run-uuid>
```

Readback is offline: it checks schemas, hashes, file references, corpus/profile
identity, attempt uniqueness/completeness, case summaries and classifications
against retained paired observations. It rejects conflicting/unregistered files
or an invented pass even if file checksums were recomputed. No execution,
database connector or publication credentials are needed.

`provenance.corpusRevision` is now a SHA-256 identity over the **executable**
manifest digest and ordered fixture references, not the pending loader
registration hash. Runner and corpus Git revisions are recorded only for clean
tracked inputs; otherwise they are null with actual content hashes retained.
The checkout SHA is still recorded explicitly. `compilerSourceRevision` is
deliberately null: retained compiler/dependency bytes prove artifact identity,
not which source built them. Build from the intended checkout; a future trusted
pipeline must independently attest that source-to-binary relationship.

All results remain `trustClass: local-unverified`, even on master or inside CI.
Environment eligibility, a green local result and successful offline validation
do not establish catalogue/native acceptance. A future publisher must bind
repository identity and verified workflow/source provenance separately. Node
and Test262 results keep separate semantics and denominators; no catalogue
schema, authority, leases or publication integration is introduced here.
