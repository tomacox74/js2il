# Typed AST validation traversal

## Scope and compatibility

Issue #2309 replaces reflective child discovery in `ValidateAsyncAwait` and
`ValidateGenerators` with `AstChildVisitor`. The passes remain separate, in their
original order, with the same function/async/generator depth rules, finally
handling, reference-identity visited sets, diagnostic strings, and locations.
This is not a change to JavaScript feature support.

Acornima's generated `ChildNodes` is already used elsewhere in the compiler.
It is not a drop-in replacement for these two defensive validation walks:
class elements enumerate children in a different order, shorthand properties
omit aliases, and a null required child can terminate enumeration before later
children. The typed visitor preserves the old public-property order, visits
both shorthand property slots, skips individual nulls without ending a list,
and retains statement-label and node-valued `UserData` traversal. The callers
still suppress repeated/shared/cyclic node visits. The async pass retains its
special try/catch/finally path unchanged.

`ValidatorTraversalTests` parses examples containing every concrete Acornima AST
node type and compares the visitor's children and reference-distinct order with
the public child properties. A parser upgrade introducing an unrepresented
node fails that inventory; an unhandled node also fails explicitly rather than
silently losing children. Reflection is used by this test oracle, not by the
production child visitor. Additional tests compare suspension diagnostics with
the previous reflection walk after deliberately removing async/generator flags
from otherwise valid parsed programs. They also cover optional/required nulls,
shared nodes, metadata cycles, sibling functions, loops, and try/finally.

## Same-host measurements

Measured on 2026-10-21 against master
`253a82c32a0e984b5840400619a7b1876a272159` and the typed-visitor change.
Both builds are JROC 0.12.33. Host: Ubuntu 24.04.5, Intel Xeon 6975P-C,
4 physical/8 logical cores, x64 RyuJIT, SDK 10.0.113,
.NET 10.0.13 (`10.0.1326.50504`), workstation concurrent GC.

These are local comparisons, not CI/release benchmark results. The host differs
from the issue's original Xeon 8573C. Do not compare their absolute times.
The original `stopwatch.js` and `stopwatch-modern.js` workloads are unchanged.
Corresponding generated IL is identical before/after for both scenarios.

### Isolated fresh-compilation diagnostics

Each scenario/revision runs in its own new process. Every invocation creates a
fresh compiler, compiles in memory, loads the artifact, executes it in a fresh
hosted realm, and disposes the load context. Only the compile call is timed
below; file reading, explicit GC, assembly loading, and execution are outside
that interval. Settings: Release, `AssumeUnmodifiedHostGlobals=true`,
`EmitPdb=false`, normal/default CLR tiering. Nothing caches compiled artifacts.

Process-wide `GC.GetTotalAllocatedBytes(true)` deltas isolate compilation.
These bytes are **phase diagnostics**, not MemoryDiagnoser results. Independent
medians are reported; MB would be decimal.

First comparison: 30 invocations, discard the first 10, retain all 20 others.

| Scenario | Before median ms | After median ms | Before SD ms | After SD ms | Before median bytes | After median bytes |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| stopwatch | 50.125 | 33.049 | 5.719 | 5.579 | 3,553,528 | 2,549,968 |
| stopwatch-modern | 48.618 | 32.042 | 5.492 | 2.308 | 3,440,768 | 2,437,208 |

An after-then-before order check gave baseline medians of 49.270/47.852 ms
for classic/modern, with the same baseline median allocation values.
The retained early-process samples span tiering transitions, so this table
must not be interpreted as stable steady-state throughput.

A second independent comparison extends the **same harness** to 100 invocations
per new process, discarding the first 50 and retaining all 50 others:

| Scenario | Before median ms | After median ms | Before SD ms | After SD ms | Before median bytes | After median bytes |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| stopwatch | 21.146 | 9.866 | 0.918 | 1.647 | 3,289,320 | 2,305,240 |
| stopwatch-modern | 21.115 | 9.215 | 2.062 | 2.835 | 3,306,104 | 2,321,664 |

Fresh compilation improves in both host-warmup windows, saving approximately
0.98-1.00 MB per compilation in these diagnostics. Absolute timing and the
percentage improvement depend materially on host/JIT warmup.

### BenchmarkDotNet allocation confirmation

The existing `JavaScriptRuntimeBenchmarks.Jroc_Total` benchmark includes fresh
compilation, assembly loading, and hosted module execution. Its boundary is
different from the phase runner; compare revisions within each table.

BenchmarkDotNet 0.16.0-preview.2, MemoryDiagnoser, in-process toolchain,
one launch, five warmups, twelve requested measurement iterations, one
invocation/unroll. BDN performs additional jitting iterations and filters
outliers; the retained sample counts are shown. This is a short focused local
run, not a default-duration release benchmark. BDN warns that the approximately
71-84 ms iterations are below its recommended 100 ms. Elevating process priority
was denied; both revisions ran with normal permissions.

| Scenario | Before mean +/- SD ms | After mean +/- SD ms | N before/after | Before B/op | After B/op | Saved B/op |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| stopwatch | 83.666 +/- 0.292 | 70.760 +/- 0.239 | 11 / 12 | 17,658,072 | 16,693,456 | 964,616 |
| stopwatch-modern | 83.858 +/- 0.281 | 70.920 +/- 0.199 | 10 / 9 | 17,680,528 | 16,715,000 | 965,528 |

This confirms the allocation reduction in the existing end-to-end harness;
it is not an execution-only speedup claim.

## Matched EventPipe profiles

Separate fresh processes collect sampled thread stacks and GC allocation ticks
for 100 invocations per revision/scenario, with PDB emission enabled for profiling
only. `JrocCompileProfile` begin/end events delimit each compile call. Analysis
keeps only samples/ticks on the compiling thread inside invocations 10-99:
90 matched compile windows, excluding load, execution, explicit GC, and other
threads. Tracing timings are not used in the timing tables.

| Inclusive sampled stack | Classic before/after | Modern before/after |
| --- | ---: | ---: |
| All retained compiler-thread samples | 2,969 / 1,526 | 3,120 / 1,615 |
| `JavaScriptAstValidator.Validate` | 1,566 / 24 | 1,604 / 29 |
| `ValidateAsyncAwait` | 1,202 / 5 | 1,237 / 5 |
| `ValidateGenerators` | 346 / 1 | 353 / 4 |
| `RuntimeMethodInfo.Invoke` | 1,446 / 0 | 1,459 / 0 |
| `RuntimeType.GetProperties` | 13 / 0 | 29 / 0 |

These are inclusive sampled-thread-time stacks, not hardware CPU percentages;
parents/children overlap and counts must not be added. Zero means no sampled
frame, not proof that all compiler reflection has been removed.

Allocation ticks with a suspension-validator ancestor decline from 881 to 16
for classic, and from 884 to 23 for modern. The remaining ticks are visited-set
storage (`Entry[Acornima.Ast.Node][]` and `Int32[]`). In the complete retained
compile windows, `PropertyInfo[]`/`RuntimePropertyInfo[]` ticks decline from
63/23 to 0/0 for classic and 52/6 to 0/0 for modern. Reflection invocation
stub/dynamic-method allocations disappear from these two validator stacks.
Allocation ticks are sampled estimates, not exact object/byte accounting;
use MemoryDiagnoser above for allocation claims.

Remaining dominant inclusive stacks are assembly generation (1,327/1,526
classic and 1,401/1,615 modern samples), planned two-phase callable compilation
(821/876), LIR stack scheduling (503/536), and symbol-table building (114/106).
GC polling/safepoint stacks are also prominent in sampled thread time and must
not be mistaken for exclusive productive compiler CPU. Runtime/IL metadata
reflection elsewhere, the main validation walker, and other separate early-error
passes remain outside this change.

## Reproduction

Build the baseline and modified compiler into separate Release outputs and run
each in a separate process. The diagnostic runner and phase-filtering analyzer,
raw measurements, BDN reports, and extracted profile data are retained with the
PR for #2309. The runner references `Jroc.Core`, uses the unchanged scenario
source as `SourceText`, and saves the first generated assembly outside the
measured interval.

For the existing BDN confirmation, build the benchmark project, then launch
each isolated output without rebuilding it:

```sh
dotnet <output>/Benchmarks.dll --all \
  --filter '*JavaScriptRuntimeBenchmarks.Jroc_Total*stopwatch*' \
  --inProcess --warmupCount 5 --iterationCount 12 --launchCount 1 \
  --invocationCount 1 --unrollFactor 1 --artifacts <results-directory>
```

For the diagnostic runner, arguments are
`<original-scenario-path> <results-directory> <30|100> [pdb]`.
Collect the profiling variant with:

```sh
dotnet-trace collect --profile dotnet-sampled-thread-time \
  --providers 'Microsoft-Windows-DotNETRuntime:0x100003801D:5,JrocCompileProfile' \
  -o <trace>.nettrace -- dotnet <output>/CompileProfile.dll \
  <original-scenario-path> <results-directory> 100 pdb
```

The verbose runtime level is required for allocation ticks; retain JIT/loader
keywords as well as GC keywords so stack symbols can be resolved.
