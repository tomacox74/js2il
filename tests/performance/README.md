# JavaScript Runtime Performance Comparison

This directory contains performance testing infrastructure for comparing JavaScript execution across multiple runtimes.

## Two Benchmark Harnesses

This directory provides two complementary performance testing approaches:

### 1. Quick Comparison Harness (Legacy)

**Location:** `RunComparison.js`, `JintComparison/`, `OkojoComparison/`, `YantraJSComparison/`

**Purpose:** Fast, simple throughput comparisons for smoke testing

**Best For:**
- Quick performance checks during development
- CI smoke tests
- Simple pass-counting benchmarks
- Rapid iteration on optimization attempts

**Run:**
```bash
node RunComparison.js
```

See the results section below for typical output.

### 2. BenchmarkDotNet Suite (New)

**Location:** `Benchmarks/`

**Purpose:** Rigorous statistical benchmarking with detailed phase reporting

**Best For:**
- Detailed performance analysis
- Statistical significance testing
- Compile vs execution phase separation
- Historical trend analysis
- Publication-quality benchmark reports

**Run:**
```powershell
cd Benchmarks
dotnet run -c Release
```

See [Benchmarks/README.md](Benchmarks/README.md) for comprehensive documentation.

---

## Quick Comparison Harness Details

This section documents the legacy quick comparison harness.

## What's Being Tested

The benchmark (`PrimeJavaScript.js`) implements the Prime Sieve algorithm in JavaScript and runs it for 5 seconds to count how many passes it can complete. This tests:
- Arithmetic operations
- Array/bitwise operations
- Control flow (loops, conditionals)
- Function calls
- Class instantiation

## Components

### JintComparison/
A C# console application that uses Jint 4.2.0 to interpret and execute the JavaScript code.

**Build & Run:**
```powershell
cd JintComparison
dotnet run -c Release
```

### RunComparison.js
Node.js script that:
1. Runs the Node.js benchmark
2. Runs the Jint benchmark
3. Runs the Okojo benchmark
4. Runs the YantraJS benchmark
5. Compiles the JavaScript using JROC
6. Runs the JROC-compiled native code
7. Compares all results with detailed statistics

### OkojoComparison/
A C# console application that uses the fully managed Okojo JavaScript engine to execute the benchmark with lightweight host shims for `console`, `process.argv`, and `perf_hooks`.

**Build & Run:**
```powershell
cd OkojoComparison
dotnet run -c Release
```

**Run:**
```bash
node RunComparison.js
```

## Results

Typical results on a modern CPU (more passes = better performance):

| Runtime | Time (5s window) | Passes | Passes/Sec | Performance |
|---------|------------------|--------|------------|-------------|
| **Node.js** | ~5.1s total | ~3600 | ~720 | ⚡ Fastest |
| **JROC** | ~6.0s total | ~92 | ~18 | 39x slower |
| **Jint** | ~7.1s total | ~10 | ~2 | 360x slower |

### Key Findings

**JROC vs Jint:**
- JROC is **~9-10x faster** than Jint
- Both run on .NET, but JROC compiles to native IL while Jint interprets
- Demonstrates the significant advantage of ahead-of-time compilation over interpretation

**JROC vs Node.js:**
- Node.js is **~39x faster** than JROC on this compute-intensive benchmark
- Node.js uses V8's highly optimized JIT compiler with decades of optimization work
- JROC produces native IL code but lacks V8's advanced optimizations (inline caching, speculative optimization, etc.)
- This gap is expected for a young compiler and represents significant room for future optimization

**Node.js vs Jint:**
- Node.js is **~360x faster** than Jint
- V8's JIT compilation provides massive performance advantage over interpretation

## Why This Matters

This demonstrates that JROC:
1. ✅ Produces **functional native code** that runs as a .NET application
2. ✅ Dramatically outperforms **interpreted .NET JavaScript** (Jint) by ~10x
3. ✅ Provides a **solid foundation** for future performance optimizations
4. ✅ Enables JavaScript code to run as **first-class .NET applications** with access to the full .NET ecosystem

While Node.js/V8 remains significantly faster on compute-intensive workloads, JROC fills a unique niche: compiling JavaScript to standalone .NET assemblies with no runtime dependencies beyond the .NET runtime itself. This trade-off between raw performance and deployment simplicity makes JROC suitable for scenarios where native .NET integration and simple deployment are more important than maximum execution speed.

## Notes

- All three tests run the same JavaScript source code
- Node.js and Jint include runtime setup overhead
- JROC compilation time (~470ms) is excluded from execution benchmark
- Results may vary based on CPU, OS, .NET version, and Node.js version
- Test measures computational throughput (passes per second) over a 5-second window

## Choosing Between Harnesses

| Use Case | Recommended Harness |
|----------|---------------------|
| Quick smoke test | Quick Comparison (`RunComparison.js`) |
| Development iteration | Quick Comparison (`RunComparison.js`) |
| Statistical analysis | BenchmarkDotNet (`Benchmarks/`) |
| Compile vs execute timing | BenchmarkDotNet (`Benchmarks/`) |
| CI performance tracking | BenchmarkDotNet (`Benchmarks/`) |
| Publication/reports | BenchmarkDotNet (`Benchmarks/`) |

## CI Integration

The BenchmarkDotNet suite is integrated into CI via `.github/workflows/benchmarkdotnet-suite.yml`:

- **Quick Comparison**: Used in `.github/workflows/performance-comparison.yml` for informational PR comments
- **BenchmarkDotNet**: Available as workflow_dispatch for detailed statistical reporting (informational-only, no PR gating)
- The cross-runtime BenchmarkDotNet suite now benchmarks the full checked-in root scenario catalog under `tests/performance/Benchmarks/Scenarios`, not just the original five bootstrap scripts

## Adding New Benchmarks

For the BenchmarkDotNet suite, see [Benchmarks/README.md](Benchmarks/README.md#contributing).

For the quick comparison harness, modify `PrimeJavaScript.js` or add new scripts in this directory.
