# BenchmarkDotNet Performance Suite

This directory contains a comprehensive BenchmarkDotNet-based performance benchmark suite for comparing JavaScript execution across multiple runtimes:

- **Node.js** - V8 JIT-compiled JavaScript (industry baseline)
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
│   ├── NodeJsRuntime.cs
│   ├── JintRuntime.cs
│   └── Js2ILRuntime.cs
├── Compliance/          # Licensing and provenance tracking
│   └── PROVENANCE.md
└── Benchmarks.csproj    # Project configuration
```

## Benchmark Scenarios

### Phase 1 Scenarios (Current)

Additional imported scenarios from Jint are available in `Scenarios/` for future use (with targeted js2il-compatibility shims where needed, e.g. strict preamble + dromaeo harness helpers).

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

## Running Benchmarks

### Prerequisites

- .NET 10.0 SDK or later
- Node.js (for Node.js benchmarks)
- BenchmarkDotNet (installed via NuGet)

### Build

```powershell
cd tests/performance/Benchmarks
dotnet build -c Release
```

### Run Benchmarks

#### Default: Cross-Runtime Comparison
Compares all three runtimes across all scenarios:

```powershell
dotnet run -c Release
```

#### Phased Comparison (js2il + Jint prepared)
Benchmarks js2il compile and execute phases separately, alongside Jint prepare and prepared execution for direct comparison:

```powershell
dotnet run -c Release -- --phased
```

#### All Benchmarks
Runs both cross-runtime comparison and phased benchmarks:

```powershell
dotnet run -c Release -- --all
```

### Command-Line Options

BenchmarkDotNet supports additional options:

```powershell
# Run specific benchmark
dotnet run -c Release -- --filter *Jint*

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

1. **Jint vs js2il**: js2il typically 5-10x faster (AOT vs interpreted)
2. **Node.js vs js2il**: Node.js typically 10-50x faster (mature JIT vs young AOT)
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
| Node.js      | minimal    |     12.45 ms |   0.24 ms |   0.22 ms |    1 |
| js2il (...)  | minimal    |    234.12 ms |   4.56 ms |   4.27 ms |    2 |
| Jint         | minimal    |  2,345.67 ms |  45.23 ms |  42.31 ms |    3 |
```

## Benchmark Methodology

### Runtime Lifecycle

- **Node.js**: Process spawned per benchmark iteration
- **Jint**: New engine instance per iteration
- **js2il**: Pre-compiled or compile+execute per iteration

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

(To be implemented in Phase 5)

The BenchmarkDotNet suite will be integrated into CI as informational-only:

- Run reduced scenario set in CI
- Upload structured artifacts
- No hard PR gating initially
- Optional nightly extended runs

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

### Node.js Not Found

Install Node.js or skip Node.js benchmarks:
```powershell
dotnet run -c Release -- --filter *Jint* *js2il*
```

### Compilation Errors

Check js2il logs in `BenchmarkDotNet.Artifacts/logs/`.

## Contributing

When adding new scenarios:

1. Add JavaScript file to `Scenarios/`
2. Update scenario list in benchmark classes
3. Document provenance in `Compliance/PROVENANCE.md`
4. Verify all three runtimes can execute the script
5. Update this README with scenario description

## References

- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
- [Jint Repository](https://github.com/sebastienros/jint)
- [Node.js Documentation](https://nodejs.org/)
- [js2il Repository](https://github.com/tomacox74/js2il)
