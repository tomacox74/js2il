# BenchmarkDotNet Performance Suite

This directory contains a comprehensive BenchmarkDotNet-based performance benchmark suite for comparing JavaScript execution across multiple hosted .NET runtimes:

- **ClearScript** - .NET-hosted V8 runtime
- **Jint** - .NET JavaScript interpreter
- **js2il** - JavaScript-to-IL AOT compiler

## Purpose

This suite provides:

1. **Standardized statistical benchmarking** - Uses BenchmarkDotNet for warmup, iterations, and outlier detection
2. **Separate compile/execute phase reporting** - Captures js2il AOT compilation separately from execution
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
│   └── Js2ILRuntime.cs
├── Compliance/          # Licensing and provenance tracking
│   └── PROVENANCE.md
└── Benchmarks.csproj    # Project configuration
```

## Benchmark Scenarios

### Core Scenarios

Benchmark runners discover the full root-level `Scenarios\*.js` catalog at runtime (currently 19 scenarios in the checked-in Jint-derived slice). The list below highlights a few representative scenarios.

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

By default this project references the checked-out `src\Js2IL.Core` and `src\JavaScriptRuntime` projects so local benchmark runs measure your current working tree. To benchmark a published package set instead, pass `-p:UsePublishedJs2ILPackages=true -p:Js2ILPackageVersion=<version>` to `dotnet restore`, `dotnet build`, or `dotnet run`.

### Run Benchmarks

#### Default: Cross-Runtime Comparison
Compares the hosted .NET runtimes across all scenarios:

```powershell
dotnet run -c Release
```

#### Phased Comparison (js2il + Jint prepared)
Benchmarks js2il compile and execute phases separately, alongside Jint prepare and prepared execution for direct comparison:

```powershell
dotnet run -c Release -- --phased
```

If any benchmark case fails, the run now exits non-zero and prints the failing benchmark cases instead of silently treating them as successful timings.

#### Late-Bound Dispatch Comparison
Runs a research-only microbenchmark that compares `JavaScriptRuntime.Object.CallMember*` against CLR-focused DLR call sites produced by C# `dynamic` and by a custom runtime-name `CallSiteBinder`:

```powershell
dotnet run -c Release -- --dispatch
```

Notes:
- This benchmark is intentionally narrow: it measures representative CLR receiver dispatch, not full JavaScript prototype semantics.
- It is useful for feasibility/performance investigations, not for validating JS compatibility.

#### All Benchmarks
Runs cross-runtime comparison, late-bound dispatch microbenchmarks, and phased benchmarks:

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

- **js2il (compile+execute)**: Total time including AOT compilation
- **js2il compile**: AOT compilation time only
- **js2il execute (pre-compiled)**: Execution time of pre-compiled assembly
- **Jint prepare**: Script preparation/parsing phase via `Engine.PrepareScript`
- **Jint execute (prepared)**: Execution phase using a previously prepared script

### Interpreting Ratios

When comparing runtimes, consider:

1. **Jint vs js2il**: js2il should generally win on steady-state .NET execution, but the exact ratio is scenario-dependent
2. **ClearScript vs js2il**: ClearScript represents a .NET client hosting V8 in-process, avoiding the process-spawn cost that made direct Node.js numbers misleading in this suite
3. **Compile overhead**: js2il compile time can exceed execution for short-running scripts

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
| js2il (...)  | minimal    |    234.12 ms |   4.56 ms |   4.27 ms |    2 |
| Jint         | minimal    |  2,345.67 ms |  45.23 ms |  42.31 ms |    3 |
```

## Benchmark Methodology

### Runtime Lifecycle

- **ClearScript**: New hosted V8 engine instance per iteration
- **Jint**: New engine instance per iteration
- **js2il**: Pre-compiled or compile+execute per iteration

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

- Release-triggered runs restore `Js2IL.Core` and `Js2IL.Runtime` from NuGet using the published release version, with retry logic to wait for NuGet indexing.
- Manual `workflow_dispatch` runs use the checked-out source tree by default, but can benchmark a published package version by setting the `js2il_package_version` input.
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

Check js2il logs in `BenchmarkDotNet.Artifacts/logs/`.

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
- [js2il Repository](https://github.com/tomacox74/js2il)
