# Dromaeo string-object performance

## Release evidence

The raw `benchmarkdotnet-results-dromaeo-execution` artifact from release run
[35050455622](https://github.com/tomacox74/js2il/actions/runs/35050455622)
(attempt 1, SHA `e707508619df6ae17c82db5f51a7f6c4a23a782f`) confirms the
regression on the same AMD EPYC 7763 runner using .NET 10.0.12:

| Runtime | Mean (ms) | Median (ms) | StdDev (ms) | N | Allocated bytes/op |
|---|---:|---:|---:|---:|---:|
| JROC 0.12.22 | 36.210 | 36.379 | 0.886 | 22 | 46,752,910 |
| JROC 0.12.21, previous-package pass | 29.966 | 29.977 | 0.409 | 15 | 46,588,832 |
| Jint 4.16.2 | 35.099 | 34.926 | 0.329 | 14 | 22,067,109 |

JROC regressed 20.84% against its prior patch; Jint now takes 3.07% less time.
The previous-package pass changes both JROC compiler and runtime packages,
not just the compiler. Jint's package update cannot explain the same-run
JROC-versus-previous-JROC regression. These release timings are not compared
directly with the different Intel host used for local measurements below.

## Regression and executed paths

The v0.12.22 `dromaeo-object-string` regression reproduces on the unmodified
scenario: local JROC execution averages 29.35 ms versus Jint 4.16.2 at
26.48 ms. These are execution-suite measurements, not compilation timings.
JROC compiles once in setup, then each timed invocation creates a fresh module
execution through `JsEngine.LoadModule`. Jint prepares once, then creates an
engine and executes the prepared script per invocation.

The principal regression comes from the Unicode correctness fix in
`4524fc3ce` (#2113). `String.ToLowerCase` and `String.ToUpperCase` previously
used CLR invariant casing directly. They now correctly support Unicode 16.0
full mappings, host-data deltas, and contextual final sigma through
`UnicodeCaseConversion`, but the initial implementation searches for these
exceptions one UTF-16 code unit at a time before calling CLR casing. Even
pure ASCII enters the large scalar-mapping path.

The scenario constructs a 131,072-character lowercase ASCII string and calls
each casing operation five times. Consequently, it pays for 1,310,720 scalar
exception checks where none can match. The generated methods
`FunctionExpression_L172C20.__js_call__` and
`FunctionExpression_L178C20.__js_call__` retain the intrinsic prototype guard,
then call `String.ToLowerCase` / `String.ToUpperCase`; the slow work is inside
the runtime helper, not a new compiler dispatch decision. `localeCompare` is
not called by this scenario and its normalization change is not the cause.
The optimized build emits identical scenario IL to the captured baseline;
only the implementations of its runtime callees change.

An isolated ASCII-only optimization reduces the full scenario from 29.35 ms
to 25.44 ms without changing its JavaScript, compiler, Jint version, or
Unicode results. This establishes the scalar pre-scan as a material cause,
rather than merely correlating the regression with a release or package bump.

Array conformance work in `e21a670d8` (#2101) also changed the emitted
`Array.join(object[])` calls to delegate to the generic spec algorithm.
The two joins in this scenario total 18,884 dense elements. Restoring guarded
dense reads avoids general numeric-key validation and descriptor probing
without undoing that algorithm's length snapshot or coercion order. The
incremental timing difference is small relative to run variability; it is
not presented as an independently proven release regression.

## Implemented improvements

`Ascii.IsValid` uses the framework's vectorized UTF-16 ASCII detection. If the
whole input is ASCII, casing delegates directly to the CLR with the original
`CultureInfo`. This retains Turkish and Azerbaijani `I`/`i` behavior; it does
not replace locale casing with invariant casing or an unconditional ASCII
bit operation. Any non-ASCII input still uses the complete Unicode 16.0 path.
No result cache, mutable static state, or input-length threshold is added.

`Array.prototype.join` reads existing dense elements directly only when the
array has no overriding non-default descriptors. The guard is evaluated for
every element, since earlier `toString` calls can shrink the array, create
holes, or install indexed getters. All other reads retain `ObjectRuntime.GetItem`,
including sparse, inherited, accessor, and Proxy behavior. Length still gets
captured before separator coercion, and element conversion is unchanged.

The compiler's `+=` concatenation builder previously required the local's
initializer to be statically a string, so `var str = new String(); str += "a"`
in a 5,000-iteration loop ran the generic `Operators.Add` path and copied the
growing string every iteration (about 25 MB, more than half of the scenario's
allocations). A dynamic variant now applies when a non-captured, non-parameter
local has an object-typed initializer and at least one `+=` with a string
literal or template literal RHS. The local keeps its object-typed slot; each
qualifying `+=` lowers to `String.AppendConcatValue`, which runs the full `+`
operator on the first append (ToPrimitive on the wrapper, `TypeError` for
symbols) and then appends into a compiler-private `ConcatAccumulator`. Every
JavaScript-visible read of the local lowers to `String.MaterializeConcatValue`,
so the accumulator is never observable; the materialized string is cached until
the next append. Non-string `+=` operands, plain reassignments, `with`
statements, `++`/`--`, captured locals, and generator/async bodies fall back to
the generic path exactly as before.

No compiler intrinsic guards, benchmark bodies, runtime versions, or
benchmark configuration are changed.

## Local BenchmarkDotNet evidence

Measurements use BenchmarkDotNet 0.15.8, .NET 10.0.12 / SDK 10.0.112,
Linux Ubuntu 24.04.5, Intel Xeon 6975P-C, four physical/eight logical cores.
The baseline is the v0.12.22 source at `e707508619df6ae17c82db5f51a7f6c4a23a782f`.
All runs use the exact command linked from the benchmark README with separate
artifact directories. The Jint package remains 4.16.2 in every local run.

| Change | Runtime | Mean (ms) | Median (ms) | StdDev (ms) | N | Allocated bytes/op |
|---|---|---:|---:|---:|---:|---:|
| v0.12.22 baseline | JROC | 29.351 | 29.261 | 1.712 | 95 | 46,750,968 |
| v0.12.22 baseline | Jint | 26.484 | 26.539 | 0.520 | 18 | 22,041,683 |
| ASCII fast path only | JROC | 25.441 | 25.429 | 0.637 | 24 | 46,734,961 |
| ASCII fast path only | Jint | 26.818 | 26.746 | 0.812 | 31 | 22,091,486 |
| ASCII + dense join | JROC | 24.896 | 24.666 | 0.853 | 38 | 46,738,662 |
| ASCII + dense join | Jint | 27.182 | 27.172 | 0.744 | 26 | 22,078,358 |
| Confirmation, both changes | JROC | 25.008 | 25.030 | 0.805 | 34 | 46,746,556 |
| Confirmation, both changes | Jint | 26.503 | 26.407 | 0.822 | 32 | 22,092,775 |
| + dynamic concat accumulator | JROC | 20.157 | 20.146 | 0.330 | 15 | 21,708,978 |
| + dynamic concat accumulator | Jint | 26.577 | 26.514 | 0.646 | 23 | 22,092,616 |

The ASCII and join changes put JROC 8.4% and 5.6% ahead of Jint by mean
duration, and 14.8-15.2% faster than the baseline, without changing
allocations. The dynamic concat accumulator then removes the `Concat String
Object` copying: allocations fall from 46.7 MB to 21.7 MB per operation (below
Jint's 22.1 MB), Gen0 collections from 46 to 37, and the mean improves to
20.16 ms, 24.2% ahead of Jint and 31.3% faster than the v0.12.22 baseline.
An instrumented per-test control (not the benchmark fixture) showed that
phase dropping from 2.7 ms to 0.06 ms steady state, and the GC time previously
attributed to later `lastIndexOf`/`slice`/`split` phases disappearing with it.
The script uses `Math.random`, so split sizes and allocation counts vary
slightly between invocations. Host-specific timings and the single-change join
delta must not be treated as universal speedups.

Raw local reports are in ignored `artifacts/string-baseline`,
`artifacts/string-ascii`, `artifacts/string-ascii-join`,
`artifacts/string-confirmation`, and `artifacts/string-concat`, under
`results/Benchmarks.DromaeoExecutionBenchmarks-report-full-compressed.json`.

## Alternatives considered

| Option | Decision |
|---|---|
| Remove the new Unicode mappings | Rejected: loses full mappings, final sigma, and Unicode 16.0 correctness. |
| Scalar ASCII casing loop | Prefer framework vectorization; a custom loop duplicates optimized CLR work and needs locale exceptions. |
| Cache casing results | Not needed to regain the lead; adds retention/ownership concerns and locale-sensitive cache keys. |
| Restore the old Array join implementation | Rejected: reintroduces incorrect undefined separators, nullish values, and mutation semantics. Keep the generic algorithm with guarded reads. |
| Expand concatenation-builder optimization to `new String()` | Implemented as the dynamic concat accumulator described above; the first append keeps full `+` coercion semantics and all reads materialize. |
| Typed numeric overloads for `charAt`/`charCodeAt`/`slice`/`substr`/`substring` | Remaining opportunity: literal arguments are boxed per call (about 135K `box double` allocations per operation) and re-coerced through `ToNumber`. Compiler-side work with broad snapshot churn; not included here. |
| Improve split sizing or substring caching | Existing presizing, one-character reuse, and bounded substring caching already apply. Further changes need allocation/profile evidence; full-scenario allocations did not materially improve here. |
| Remove prototype guards or hoist mutable global reads | Rejected without invalidation/side-effect proof. `String.prototype` overrides and callback mutations remain observable. |
| Optimize realm/module initialization or dynamic dispatch globally | Broader opportunity, but not the identified casing regression. Measure separately and avoid weakening isolation. |
| Roll back Jint or change the benchmark | Rejected: compare against current stable Jint with the original workload. |

## Compatibility coverage

The existing native Test262 join and four casing families pass (129 fixtures),
including separator-induced growth/shrinkage and full/contextual Unicode
casing. Runtime tests additionally cover all ASCII code units, vector-boundary
lengths, invariant/English/Turkish/Azerbaijani cultures, non-ASCII characters
after long ASCII prefixes, Unicode 16.0 supplementary mappings, and unpaired
surrogates. Array tests cover mutation to indexed getters/inherited values
during element coercion and dense reads on arrays with named metadata.
