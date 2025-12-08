# JavaScript Runtime Performance Comparison

This directory contains a comprehensive performance comparison between three JavaScript execution environments:
- **Node.js** - Google V8 engine (industry standard)
- **Jint** - JavaScript interpreter for .NET
- **JS2IL** - JavaScript-to-IL compiler

All three execute the same PrimeJavaScript benchmark to compare performance.

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
3. Compiles the JavaScript using JS2IL
4. Runs the JS2IL-compiled native code
5. Compares all three results with detailed statistics

**Run:**
```bash
node RunComparison.js
```

## Results

Typical results on a modern CPU (more passes = better performance):

| Runtime | Time (5s window) | Passes | Passes/Sec | Performance |
|---------|------------------|--------|------------|-------------|
| **Node.js** | ~5.1s total | ~3600 | ~720 | ⚡ Fastest |
| **JS2IL** | ~6.0s total | ~92 | ~18 | 39x slower |
| **Jint** | ~7.1s total | ~10 | ~2 | 360x slower |

### Key Findings

**JS2IL vs Jint:**
- JS2IL is **~9-10x faster** than Jint
- Both run on .NET, but JS2IL compiles to native IL while Jint interprets
- Demonstrates the significant advantage of ahead-of-time compilation over interpretation

**JS2IL vs Node.js:**
- Node.js is **~39x faster** than JS2IL on this compute-intensive benchmark
- Node.js uses V8's highly optimized JIT compiler with decades of optimization work
- JS2IL produces native IL code but lacks V8's advanced optimizations (inline caching, speculative optimization, etc.)
- This gap is expected for a young compiler and represents significant room for future optimization

**Node.js vs Jint:**
- Node.js is **~360x faster** than Jint
- V8's JIT compilation provides massive performance advantage over interpretation

## Why This Matters

This demonstrates that JS2IL:
1. ✅ Produces **functional native code** that runs as a .NET application
2. ✅ Dramatically outperforms **interpreted .NET JavaScript** (Jint) by ~10x
3. ✅ Provides a **solid foundation** for future performance optimizations
4. ✅ Enables JavaScript code to run as **first-class .NET applications** with access to the full .NET ecosystem

While Node.js/V8 remains significantly faster on compute-intensive workloads, JS2IL fills a unique niche: compiling JavaScript to standalone .NET assemblies with no runtime dependencies beyond the .NET runtime itself. This trade-off between raw performance and deployment simplicity makes JS2IL suitable for scenarios where native .NET integration and simple deployment are more important than maximum execution speed.

## Notes

- All three tests run the same JavaScript source code
- Node.js and Jint include runtime setup overhead
- JS2IL compilation time (~470ms) is excluded from execution benchmark
- Results may vary based on CPU, OS, .NET version, and Node.js version
- Test measures computational throughput (passes per second) over a 5-second window
