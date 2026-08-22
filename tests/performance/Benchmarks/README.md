# BenchmarkDotNet Performance Suite

This directory contains a comprehensive BenchmarkDotNet-based performance benchmark suite for comparing JavaScript execution across multiple hosted .NET runtimes:

- **ClearScript** - .NET-hosted V8 runtime
- **Jint** - .NET JavaScript interpreter
- **Okojo** - fully managed .NET JavaScript runtime
- **YantraJS** - .NET JavaScript runtime
- **jroc** - JavaScript-to-IL AOT compiler

## Purpose

This suite provides:

1. **Standardized statistical benchmarking** - Uses BenchmarkDotNet for warmup, iterations, and outlier detection
2. **Separate compile/execute phase reporting** - Captures jroc AOT compilation separately from execution
3. **Structured scenario catalog** - Organized, reusable benchmark scenarios
4. **Machine-readable outputs** - JSON and structured reports for historical trend analysis
5. **Licensing compliance** - Tracked provenance for all benchmark scripts

## Project Structure

```
Benchmarks/
├── Scenarios/           # JavaScript benchmark scripts
│   ├── minimal.js
│   ├── evaluation.js
│   ├── evaluation-modern.js
│   ├── stopwatch.js
│   ├── array-stress.js
│   └── ... (additional imported Jint scenarios)
├── Runtimes/            # Runtime adapter implementations
│   ├── IJavaScriptRuntime.cs
│   ├── ClearScriptRuntime.cs
│   ├── JintRuntime.cs
│   ├── OkojoRuntime.cs
│   ├── YantraJsRuntime.cs
│   └── JrocRuntime.cs
├── Compliance/          # Licensing and provenance tracking
│   └── PROVENANCE.md
└── Benchmarks.csproj    # Project configuration
```

## Benchmark Scenarios

### Core Scenarios

The cross-runtime runner discovers the root-level `Scenarios\*.js` catalog, while the Dromaeo execution runner discovers `Scenarios\dromaeo\*.js`. The list below highlights a few representative scenarios.

1. **minimal.js** - Simple arithmetic (`1 + 1 === 2`)
   - Purpose: Baseline minimal execution overhead
   - Tests: Basic expression evaluation

2. **evaluation.js** - Object properties and recursion
   - Purpose: Property access, string concatenation, function calls
   - Tests: Object manipulation, fibonacci recursion

3. **evaluation-modern.js** - ES6+ syntax version
   - Purpose: Modern JavaScript features (const, arrow functions)
   - Tests: ES6 object and function patterns

4. **stopwatch.js** - Class instantiation and methods
   - Purpose: Constructor functions, method calls, state management
   - Tests: Class-based patterns, loops, conditionals

5. **array-stress.js** - Array manipulation stress test
   - Purpose: Array performance (push, pop, shift, unshift, splice, slice)
   - Tests: High-volume array operations

Additional discovered scenarios include the broader Dromaeo-derived object/string/regexp/base64 cases, `linq-js`, and the modern stopwatch variants. The cross-runtime suite now runs this full file-backed catalog rather than the original five-script bootstrap subset.

## Running Benchmarks

### Prerequisites

- .NET 10.0 SDK or later
- BenchmarkDotNet (installed via NuGet)

### Build

```powershell
cd tests/performance/Benchmarks
dotnet build -c Release
```

By default this project references the checked-out `src\Jroc.Core` and `src\JavaScriptRuntime` projects so local benchmark runs measure your current working tree. To benchmark a published package set instead, pass `-p:UsePublishedJrocPackages=true -p:JrocPackageVersion=<version>` to `dotnet restore`, `dotnet build`, or `dotnet run`.

### Run Benchmarks

#### Default: Cross-Runtime Comparison
Compares the hosted .NET runtimes across all scenarios:

```powershell
dotnet run -c Release
```

#### Dromaeo Execution Comparison
Benchmarks prepared Jroc, Jint, and Okojo execution for each Dromaeo scenario:

```powershell
dotnet run -c Release -- --dromaeo
```

Use `--scenario` to run only one scenario (accepts scenario key, script name, or `<script>.js`):

```powershell
dotnet run -c Release -- --dromaeo --scenario dromaeo-3d-cube
```

If any benchmark case fails, the run now exits non-zero and prints the failing benchmark cases instead of silently treating them as successful timings.

#### Kraken Comparison
Runs the selected Kraken 1.1 workload with its test and data scripts loaded during setup, then measures only its registered `runTest()` callback for jroc, Jint, Okojo, and YantraJS. The local suite includes `ai-astar`, `audio-beat-detection`, `audio-dft`, and `audio-oscillator`. `audio-fft` is retained as `audio-fft.js.disabled` and `audio-fft-data.js.disabled`, but is excluded from discovery until it is explicitly re-enabled.

```powershell
dotnet run -c Release -- --kracken
dotnet run -c Release -- --kracken --scenario audio-oscillator
```

#### Prime Execution Comparison
Runs a single `PrimeJavaScript` sieve pass after JROC compilation and interpreter preparation complete. The dedicated `Scenarios/prime/PrimeJavaScript.OnePass.js` fixture is identical to `tests/performance/PrimeJavaScript.js` except that the timed five-second batch is replaced by one pass. It is intentionally outside the root cross-runtime catalog because it requires Node globals.

```powershell
dotnet run -c Release -- --prime-execute
```

#### Branch comparison workflow

Run the manual `Benchmark branch comparison` workflow to compare a scenario from
the `dromaeo` (default) or `kracken` benchmark suite on a baseline ref (default:
`master`) and a private branch on the same GitHub-hosted runner. It runs the
baseline first, prints both
BenchmarkDotNet reports in the workflow log, and uploads raw reports plus console output as the
`benchmark-branch-comparison-results` artifact.

Dispatch it from a checkout with:

```powershell
node scripts/dispatchBenchmarkBranchComparisonWorkflow.js <private-branch> <scenario-name> --watch
```

For example:

```powershell
node scripts/dispatchBenchmarkBranchComparisonWorkflow.js perf/object-shapes dromaeo-3d-cube --watch
```

To compare the Kraken `ai-astar` scenario:

```powershell
node scripts/dispatchBenchmarkBranchComparisonWorkflow.js feat/scheduler-general-regions ai-astar --benchmark kracken --baseline v0.11.40 --watch
```

#### Kraken ai-astar phased guardrails

Run the complete Phase 0 guardrail for issue #1958:

```powershell
npm run perf:phased:kracken-ai-astar
```

This command:

- selects only `kracken-ai-astar` from `KrackenExecutionBenchmarks`;
- preserves the benchmark boundary by compiling, loading, and initializing the
  fixture in `GlobalSetup`, then measuring only the registered A* workload;
- runs focused monomorphic, polymorphic, post-megamorphic, generic-property,
  boxed Array-length, guarded candidate Array-length, and direct numeric
  Array-length controls;
- compiles the exact composed fixture and reports counters from the generated
  `Array.prototype.findGraphNode` method.

Use the Dry job to validate selection, report parsing, and IL analysis without
treating the timing as production evidence:

```powershell
npm run perf:phased:kracken-ai-astar:dry
```

Run only the deterministic IL counters when a Kraken report is not needed:

```powershell
npm run perf:phased:kracken-ai-astar:il
```

The IL report records method size, dynamic-cache property reads, generic
item/property reads, generic numeric `Array.length` reads, explicit boxing,
arity-1 dispatch inside `findGraphNode`, and arity-1 calls to `findGraphNode`
from other generated callables. These are method/path counters rather than
whole-assembly totals.

Write a machine-readable result, including SHA and raw BenchmarkDotNet
provenance. The command writes
`BenchmarkDotNet.Artifacts/results/KrackenAiAStarGuardrails-summary.json` by
default; override it with:

```powershell
node scripts/runKrackenAStarGuardrails.js --il --output-json artifacts/kracken-ai-astar.json
```

Compare a candidate raw report with a parent report from the same host:

```powershell
node scripts/runKrackenAStarGuardrails.js \
  --no-run --no-microbenchmarks \
  --results-file artifacts/candidate/Benchmarks.KrackenExecutionBenchmarks-report-full-compressed.json \
  --candidate-sha <candidate-sha> \
  --baseline-report artifacts/baseline/Benchmarks.KrackenExecutionBenchmarks-report-full-compressed.json \
  --baseline-sha <baseline-sha> \
  --tolerance-percent 5
```

The comparison refuses different CPU, OS, architecture, .NET runtime, logical
core count, or BenchmarkDotNet versions. On compatible reports it exits with
code 2 when the JROC mean regresses beyond the tolerance; use
`--allow-regression` only for exploratory reporting. `--no-run` never assigns
the current HEAD to an existing report: pass `--candidate-sha` explicitly or
the summary records `unknown`. Executed runs record both HEAD and whether the
source tree was dirty.

Timing policy:

- use non-Dry BenchmarkDotNet runs for performance claims;
- compare candidate and parent on the same host and runtime, preferably in one
  workflow run;
- retain mean, median, sample count, allocation, SHA, host, runtime, and the raw
  reports;
- treat the 5% default as a regression flag, not a claim that smaller changes
  are statistically significant;
- repeat noisy timing results, while treating stable allocation and generated
  IL counters as separate evidence;
- never compare a modified fixture as if it were the production scenario.

Phase 0 baseline recorded on 2026-08-21:

- source: master `4678366be0372faaf48c3fca2144f0f4efa62197`
  plus guardrail-only working-tree changes;
- host: Intel Xeon 6975P-C, Ubuntu 24.04.4, 8 logical / 4 physical cores;
- runtime: .NET 10.0.11; BenchmarkDotNet 0.15.8;
- exact `kracken-ai-astar` JROC result: 10.361 s mean, 10.395 s median,
  N=17, 2,314,852,736 B/op;
- competitors in the same run: Jint 4.507 s, Okojo 5.268 s, YantraJS
  6.887 s;
- focused controls: generic property read 43.249 ns, post-megamorphic
  fallback 56.630 ns (1.31x), boxed numeric Array length 29.962 ns / 24 B,
  direct numeric Array length 0.562 ns / 0 B;
- generated `findGraphNode`: 159 IL bytes, 2 dynamic-cache property reads,
  2 generic item/property reads, 1 generic numeric `Array.length` read,
  1 explicit `box`, and 2 external arity-1 `findGraphNode` call sites.

The full benchmark and microbenchmark rows used their normal BenchmarkDotNet
jobs. The runtime sources match master; the recorded dirty state consists of
this Phase 0 benchmark/tooling implementation and does not modify compiler or
runtime execution behavior.

Phase 1 candidate recorded on the same Intel host on 2026-08-21:

- exact `kracken-ai-astar` JROC result: 7.434 s mean, 7.452 s median,
  N=16, 2,314,930,320 B/op, 28.2% faster than the recorded Phase 0 mean;
- competitors in the same run: Jint 4.543 s, Okojo 5.228 s, YantraJS
  7.292 s;
- focused controls: generic property read 28.41 ns, generated terminal
  fallback 25.39 ns, monomorphic hit 10.01 ns, and four-way polymorphic hit
  13.89 ns, all at 0 B/op;
- the active-frame descriptor benchmark measured direct descriptor lookup at
  15.10 ns / 24 B and descriptor-aware property read at 34.72 ns / 24 B. The
  companion pre-change descriptor lookup was 20.77 ns / 24 B; Phase 1 does
  not claim to remove that remaining allocation.

These are separate non-Dry runs on the same host, not simultaneous paired
measurements. Preserve and compare the raw reports with the guardrail helper
when making a regression decision.

Phase 2 Array specialization candidate recorded on the same Intel host on
2026-08-21:

- source: Phase 1 merge `13a74806cc9aad7aad5ae2b774e6dcd85969d9a4`
  plus the Phase 2 working-tree changes;
- exact non-Dry `kracken-ai-astar` JROC result: 4.984 s mean, 4.954 s
  median, N=19, 57,534,240 B/op;
- compared with the same-host Phase 1 result, the mean is 33.0% lower and
  managed allocation is 97.5% lower;
- this was a JROC-only rerun. Competitors were intentionally not repeated;
  their same-host Phase 1 measurements remain above;
- the guarded candidate Array-length allocation control reports 0 B/op,
  matching direct Array length and avoiding the boxed path's 24 B/op;
- generated `findGraphNode`: 217 IL bytes, 2 dynamic-cache property reads,
  3 generic item/property reads, 0 generic numeric `Array.length` reads,
  and 1 explicit `box`. The loop-hot index read uses the guarded Array helper;
  the return-only index read remains generic because it is outside the natural
  loop and executes at most once.

The Phase 2 comparison uses separate non-Dry runs with matching host, OS,
runtime, benchmark version, scenario, and benchmark boundary. The raw reports,
sample counts, and allocation values are the comparison evidence; the focused
Dry control is used only to verify allocation shape, not timing.

Phase 3 shape-keyed property cache candidate recorded on the same Intel host
on 2026-08-22:

- normal focused controls: same-receiver monomorphic hit 7.98 ns,
  10,000-instance same-shape hit 9.29 ns, four-shape polymorphic hit 14.39 ns,
  and generic property read 27.74 ns, all N=3 and 0 B/op;
- bounded diagnostics for the exact `findGraphNode` sites observed one
  compulsory miss followed by approximately 93.4 million hits across
  distinct `GraphNode` receivers sharing one eligible `JsShape`; both `pos`
  sites remained monomorphic;
- sequential non-Dry `kracken-ai-astar`: Phase 2 parent 4.654 s mean /
  4.600 s median, N=98, 57,534,240 B/op; Phase 3 candidate 4.671 s mean /
  4.717 s median, N=15, 57,887,688 B/op. The 0.35% mean difference is within
  noise and is not claimed as a throughput change;
- sequential non-Dry `dromaeo-3d-cube`: parent 7.734 ms mean, N=26,
  4,972,572 B/op; candidate 7.831 ms mean, N=17, 4,723,039 B/op. The
  overlapping timing distributions are flat while allocation is 5.0% lower.

Issue #1324 also requires the N=1 Dry cube guardrail before and after. Those
results are retained as diagnostic counters, not timing claims:

| Scenario | Runtime | Parent mean | Candidate mean | Parent alloc | Candidate alloc |
| --- | --- | ---: | ---: | ---: | ---: |
| `dromaeo-3d-cube` | JROC | 172.60 ms | 191.39 ms | 5.27 MiB | 5.05 MiB |
| `dromaeo-3d-cube` | Jint prepared | 135.08 ms | 124.54 ms | 1.42 MiB | 1.42 MiB |
| `dromaeo-3d-cube` | Okojo | 66.78 ms | 67.60 ms | 0.74 MiB | 0.74 MiB |
| `dromaeo-3d-cube-modern` | JROC | 128.96 ms | 140.81 ms | 3.16 MiB | 2.94 MiB |
| `dromaeo-3d-cube-modern` | Jint prepared | 111.55 ms | 116.51 ms | 1.34 MiB | 1.34 MiB |
| `dromaeo-3d-cube-modern` | Okojo | 95.69 ms | 77.24 ms | 0.69 MiB | 0.69 MiB |

The Phase 3 structural exit gate is independent of a benchmark-only rewrite:
same-shape receivers now reuse a slot-location entry and avoid the full
generic property algorithm, while accessors, inherited properties, Proxies,
symbols, descriptor changes, deletion, and exotic receivers remain generic.
The normal end-to-end comparisons show the resulting throughput is currently
flat; later phases must not treat this foundation as a measured speedup.

#### Cube-focused guardrail workflow
Runs only the Dromaeo cube phased scenarios and prints the execution counters we track for issue #1327:

```powershell
npm run perf:phased:cube:dry
```

Optional IL/codegen smell counters (advisory only):

```powershell
npm run perf:phased:cube:dry:il-smells
```

The helper script is `scripts/runCubePhasedGuardrails.js`. It reports:

- `jroc-execute`, `jint-execute-prepared`, `okojo-execute` mean/alloc counters for:
  - `dromaeo-3d-cube`
  - `dromaeo-3d-cube-modern`
- relative ratios (`jroc / jint`, `jroc / okojo`) for both time and allocation
- optional generated-IL smell counts (`newarr System.Object`, `box`, `Closure.InvokeWithArgs*`, `TypeUtilities.ToNumber`, `ObjectRuntime.GetItem`)

For PRs that optimize cube performance, include the before/after benchmark output and the CI run id / Supabase run id used for comparison.

#### Regexp stable-call guardrails

Runs the exact classic/modern Dromaeo regexp pair:

```powershell
npm run perf:phased:regexp:dry
```

Run only the deterministic generated-IL checks:

```powershell
npm run perf:phased:regexp:il
```

Or combine the Dry benchmark and IL checks:

```powershell
npm run perf:phased:regexp:dry:il
```

`scripts/runRegexpStableCallGuardrails.js` uses exact `--scenario` selection.
For the modern fixture it verifies that every source call to `randomChar` and
`generateTestStrings` is a direct call to the canonical callable `MethodDef`,
that the hot helper bodies contain no runtime callable dispatch, and that the
remaining `fn()` dispatch in `prep`/`test` is present. The classic fixture is
reported as a reference control. The check intentionally avoids
whole-assembly closure/allocation counts because generated function-object
adapters and benchmark callbacks make those counts misleading.

#### Late-Bound Dispatch Comparison
Runs a research-only microbenchmark that compares `JavaScriptRuntime.Object.CallMember*` against CLR-focused DLR call sites produced by C# `dynamic` and by a custom runtime-name `CallSiteBinder`:

```powershell
dotnet run -c Release -- --dispatch
```

Notes:
- This benchmark is intentionally narrow: it measures representative CLR receiver dispatch, not full JavaScript prototype semantics.
- It is useful for feasibility/performance investigations, not for validating JS compatibility.

#### Object Operation Boundaries

Compares internal ordinary-object reads and writes through JROC runtime operations with
hosted C# `dynamic` access through `JsDynamicValueProxy`:

```powershell
dotnet run -c Release -- --object-operations
```

The hosted cases include runtime-thread marshalling and DLR boundary dispatch. Core
JavaScript objects do not participate in DLR dispatch.

#### Prototype Storage
Measures the time and managed allocation required to construct an Array or ordinary `JsObject` with an initialized prototype:

```powershell
dotnet run -c Release -- --prototype-storage
```

#### All Benchmarks
Runs cross-runtime comparison, late-bound dispatch microbenchmarks, and Dromaeo execution benchmarks:

```powershell
dotnet run -c Release -- --all
```

### Command-Line Options

BenchmarkDotNet supports additional options:

```powershell
# Run specific benchmark
dotnet run -c Release -- --filter *Jint*

# Run the late-bound dispatch benchmark class only
dotnet run -c Release -- --dispatch --filter *LateBoundDispatch*

# Run representative Array, RegExp, collection, and Node built-in adapters
dotnet run -c Release -- --builtin-invocation

# Run only one Dromaeo scenario
dotnet run -c Release -- --dromaeo --scenario dromaeo-3d-cube

# Export results to JSON
dotnet run -c Release -- --exporters json

# Generate detailed reports
dotnet run -c Release -- --exporters html,json,markdown
```

## Understanding Results

### Timing Metrics

- **Mean**: Average execution time across all iterations
- **Error**: Half of 99.9% confidence interval
- **StdDev**: Standard deviation of all measurements
- **Median**: Middle value of sorted measurements

### Phased Metrics

- **jroc (compile+execute)**: Total time including AOT compilation
- **jroc compile**: AOT compilation time only
- **jroc execute (pre-compiled)**: Execution time of pre-compiled assembly
- **Jint prepare**: Script preparation/parsing phase via `Engine.PrepareScript`
- **Jint execute (prepared)**: Execution phase using a previously prepared script
- **Okojo execute**: Execution phase using setup-prepared Okojo bytecode

### Interpreting Ratios

When comparing runtimes, consider:

1. **Jint vs jroc**: jroc should generally win on steady-state .NET execution, but the exact ratio is scenario-dependent
2. **ClearScript vs jroc**: ClearScript represents a .NET client hosting V8 in-process, avoiding the process-spawn cost that made direct Node.js numbers misleading in this suite
3. **Compile overhead**: jroc compile time can exceed execution for short-running scripts

## Output

Results are saved in `BenchmarkDotNet.Artifacts/`:

- `results/` - CSV, HTML, and Markdown reports
- `logs/` - Detailed execution logs
- `*.json` - Machine-readable benchmark data

### Sample Output

```
BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460)
Intel Core i7-9700K CPU 3.60GHz (Coffee Lake), 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.102

| Method       | ScriptName | Mean         | Error      | StdDev     | Rank |
|------------- |----------- |-------------:|-----------:|-----------:|-----:|
| ClearScript  | minimal    |     12.45 ms |   0.24 ms |   0.22 ms |    1 |
| jroc (...)  | minimal    |    234.12 ms |   4.56 ms |   4.27 ms |    2 |
| Jint         | minimal    |  2,345.67 ms |  45.23 ms |  42.31 ms |    3 |
```

## Benchmark Methodology

### Runtime Lifecycle

- **ClearScript**: New hosted V8 engine instance per iteration
- **Jint**: New engine instance per iteration
- **Okojo**: New runtime instance per iteration
- **jroc**: Pre-compiled or compile+execute per iteration

Direct Node.js process-per-iteration measurements are intentionally excluded from the default cross-runtime suite because they include temp-file creation, process startup, stdout/stderr capture, and process teardown. Those numbers are useful as a CLI cold-start metric, but they are not a fair comparison to in-process .NET runtimes. ClearScript is used instead as the .NET-hosted V8 comparison point.

### Measurement Approach

1. **Warmup**: Multiple iterations to stabilize JIT/runtime
2. **Measurement**: Timed iterations after warmup
3. **Outlier Detection**: Statistical analysis to remove anomalies
4. **Reporting**: Mean, median, standard deviation

### Determinism

- Fixed seeds where applicable
- No async/event-loop scenarios in Phase 1
- Consistent input data across runtimes

## Compliance

All benchmark scripts are tracked in `Compliance/PROVENANCE.md`:

- Source repository and commit
- License information
- Copyright attribution
- Modification policy

## Relationship to Existing Harness

This BenchmarkDotNet suite **complements** the existing quick comparison harness:

- **Existing harness** (`RunComparison.js`): Quick smoke tests, simple throughput comparisons
- **BenchmarkDotNet suite**: Rigorous statistical analysis, detailed phase reporting

Both are maintained for different use cases:
- Quick checks: Use `RunComparison.js`
- Detailed analysis: Use BenchmarkDotNet suite

## CI Integration

The repository ships a `BenchmarkDotNet Performance Suite` workflow for CI benchmarking:

- Release-triggered runs restore `Jroc.Core` and `Jroc.Runtime` from NuGet using the published release version, with retry logic to wait for NuGet indexing.
- Manual `workflow_dispatch` runs use the checked-out source tree by default, but can benchmark a published package version by setting the `jroc_package_version` input.
- Structured BenchmarkDotNet artifacts are uploaded, and the JSON results can be ingested into Supabase for historical tracking.

## Future Enhancements

### Phase 2+ Scenarios

- Interop/host binding benchmarks
- Larger legacy benchmark suites (after licensing review)
- Real-world application scenarios
- Memory allocation and GC pressure analysis

### Reporting Enhancements

- Historical trend comparison
- Regression detection
- Ratio normalization charts
- Performance dashboard

## Troubleshooting

### "Script not found" Error

Ensure scripts are copied to output directory:
```powershell
dotnet clean
dotnet build -c Release
```

### ClearScript Native V8 Load Errors

ClearScript requires a native V8 package for the current platform. The benchmark project references the Windows x64 and Linux x64 native packages used by local and GitHub Actions runs.

### Compilation Errors

Check jroc logs in `BenchmarkDotNet.Artifacts/logs/`.

## Contributing

When adding new scenarios:

1. Add JavaScript file to `Scenarios/`
2. Document provenance in `Compliance/PROVENANCE.md`
3. Verify all hosted runtimes can execute the script
4. Update this README with scenario description if it adds a new workload family

## References

- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
- [ClearScript Documentation](https://microsoft.github.io/ClearScript/)
- [Jint Repository](https://github.com/sebastienros/jint)
- [jroc Repository](https://github.com/tomacox74/jroc)
