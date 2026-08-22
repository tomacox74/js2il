# Guarded String intrinsic calls

Issue #1891 restores direct AOT calls for common primitive String operations
without bypassing JavaScript prototype semantics. Receiver type alone is not a
valid specialization assumption because user code can replace, delete, or
reconfigure methods on `String.prototype` or its `Object.prototype` ancestor.

## LIR contract

`LIRCallGuardedStringIntrinsic` records:

- the original receiver, member name, arguments, and result;
- the exact fixed-arity `JavaScriptRuntime.String` helper signature;
- whether static analysis already proves a primitive-string receiver;
- any result conversion that both branches must perform.

The instruction is a scheduling boundary because its IL contains internal
control flow.

## Emitted paths

The generated fast path:

1. validates
   `IntrinsicPrototypeEpochs.IsPristine(IntrinsicPrototypeFamily.String)`;
2. performs `isinst string` when the receiver type is uncertain;
3. if that check fails, accepts a boxed String only when it has the current
   realm's exact `String.prototype`, no own override for the called member, and
   an intact string-valued `[[StringData]]` slot;
4. calls the exact fixed-arity String helper.

The cold path executes the original `ObjectRuntime.CallMember0..5` operation
with the original receiver and arguments. Argument expressions are evaluated
before the guard, so both branches preserve JavaScript evaluation order and
coercion behavior.

Boxed Strings with a custom prototype, own assignment or descriptor override,
or a prototype from another realm take the exact generic fallback. Prototype
epoch invalidation also bypasses both primitive and boxed fast paths.

The compiler keeps result storage as `object` when an override can return an
arbitrary value. The `charCodeAt` plus `ToNumber` fusion is the current
exception: both branches produce the same numeric result because the fallback
performs ordinary member dispatch and then applies `ToNumber`, so the result
can remain an unboxed `double`.

Stable `substring` calls now pass through this guarded instruction instead of
using their former unconditional HIR-level direct call. The previous
unguarded `charCodeAt` numeric fusion is guarded as well.

## Receiver candidates

The symbol-table fixed point separately records reference receiver candidates
observed at declarations and writes. A candidate is not a CLR storage claim:
conflicting assignments, captured fields, and deferred callbacks can all make
the value at a particular read unknown. Candidates therefore never remove a
runtime check or authorize an unguarded intrinsic call.

Candidate sets never override representation-safe storage facts or bypass
runtime compatibility checks. In particular, `new String(...)` produces a
boxed String object and is not recorded as a primitive-string candidate. A
later `+=` can still establish a primitive-string result because JavaScript
addition coerces that object to a primitive.

Final per-program-point Array and concrete typed-array candidates drive
guarded instance-method specialization. A candidate-only site retains an
`isinst` check and generic fallback; only a fact containing one candidate and
no unknown or non-candidate paths can omit the type test. Object-literal shapes
and generated user-class metadata remain separate identity domains and
continue through their existing specialized lowering rather than being
collapsed into CLR receiver candidates.

An ordinary function expression assigned directly to a property of the
unshadowed intrinsic `Array.prototype` also receives an Array candidate for
its dynamic `this` value. The summary deliberately includes unknown and
non-candidate inputs: the function can be extracted, invoked with `call`, or
used as a constructor. It therefore authorizes only guarded specialization,
never an Array-typed callable ABI or an unconditional cast.

### Array length and indexed reads

Loop-hot numeric consumption of `.length` on an Array candidate lowers to
`ObjectRuntime.GetArrayLengthWithFallback`. The helper returns `double`
directly for an actual `JavaScriptRuntime.Array`, avoiding the generic
property result box and preserving an unboxed value through the loop
comparison. A non-Array receiver executes `GetItem(receiver, "length")`
followed by ordinary numeric conversion.

Loop-hot numeric index reads use
`ObjectRuntime.GetArrayElementWithFallback`. An actual Array enters the
existing exotic Array indexer; every other receiver uses the generic numeric
`GetItem` algorithm. The Array indexer itself still handles descriptor-backed
indices, holes, sparse and out-of-range reads, and prototype-visible misses.
Proxies do not satisfy the Array type check and therefore remain on the
generic path.

These rewrites do not hoist either operation. A source-level `this.length`
inside a loop is still evaluated at its original point on every iteration, so
length changes made by the loop body remain observable.

### Per-program-point facts

After final LIR normalization and variable-slot coalescing, the compiler runs a
forward fixed-point analysis over the method control-flow graph. It tracks
receiver candidates separately for SSA temps, mutable variable slots,
parameters, and captured scope fields. Plain assignments are strong updates;
branch joins and loop back edges union the incoming candidates until the graph
stabilizes.

To bound compile-time and memory cost, facts are materialized only for the
backward slice rooted at dynamic receiver operands. The slice follows copies,
object coercions, parameter loads/stores, mutable slots, and captured field
loads/stores, so receiver-relevant dependencies are retained without recording
unrelated method state.

Normal compilation narrows those roots further to member/index operations that
the specialization pass can actually consume. A lightweight backward
reachability check must also find a candidate-producing definition, binding,
parameter, or callable summary before the compiler builds the CFG and solves
full flow facts. Verbose and diagnostic-file compilation retains the broader
dynamic-receiver analysis so diagnostics remain complete.

Each recorded fact distinguishes explicitly observed candidate CLR types from
non-candidate values and unconstrained values. This preserves uncertainty at
joins instead of treating a candidate seen on one path as proof for every path.
The facts are available before instruction operands and after instruction
definitions, including phi-like LIR temps written by copies in multiple
branches. Calls and suspension points conservatively invalidate captured field
facts. Dynamic property reads and writes do the same because an accessor can
run an escaping closure that mutates captured state. An unclassified LIR
instruction invalidates every mutable location, and control leaving a protected
region with a `finally` does likewise rather than assuming the handler has no
relevant writes. Plain heap mutation does not invalidate a local binding's
receiver type because changing an object does not replace the binding value.
The facts remain representation-neutral. Only the guarded receiver
specialization pass consumes eligible Array and typed-array facts.

### Loop hotness gate

Post-flow Array and typed-array guards are emitted only for calls inside a
natural loop. The final-LIR control-flow graph computes dominators and
back-edge natural loops, then records the nesting depth of every instruction.
An otherwise eligible site requires depth one or greater; a cold site keeps
its original `ObjectRuntime.CallMemberN` instruction and therefore adds no
guard branches or fallback duplication.

Multiple latches for one loop header are merged before depth is counted, while
nested loop headers increase the depth independently. Exception handlers are
separate control-flow roots so an unrelated handler edge cannot make a cold
block appear loop-hot. The analysis is lazy: methods pay its cost only after
the receiver specialization pass finds an otherwise eligible Array or
typed-array call.

This gate does not change existing proven-Array direct calls or guarded String
method calls. It controls the code-size expansion introduced when post-flow
receiver specialization selects a guarded method, candidate Array
length/index helper, or numeric String-index fast path.

### Guard hoisting

The symbol-table pass computes a deterministic effect summary for every
callable and lexical block/class scope. Summaries record intrinsic-prototype
mutation, property definition/deletion/reconfiguration, unknown calls,
suspension, and escaping values. Known direct-call and lexical-scope edges
propagate those effects to a monotone fixed point before lowering.

After final LIR normalization, a natural loop with one external preheader can
capture its prototype-family epoch assumption once when the complete loop is
free of invalidating effects. Otherwise, a straight-line region can share one
capture across at least two eligible calls until the next effect barrier.
Captured assumptions use pinned locals because textual last-use allocation is
not sufficient across CFG back edges.

The hoisted value represents only the realm-owned pristine-epoch check. Every
call still validates its receiver type, own member override, and exact default
prototype where those checks are required. A failed receiver guard clears the
captured assumption before generic dispatch, so user code reached by the
fallback cannot leave later calls on a stale fast path. Methods with exception
regions retain per-call guards.

Unknown or escaped calls, observable property access, property/prototype
mutation, suspension, unsupported LIR, mutable-scope replacement, unsafe
argument coercion, and calls guarded by another prototype family terminate a
sharing region. String and typed-array calls are eligible only when every
argument is proven primitive or boxed primitive. Array calls are deliberately
excluded: even a successful native Array operation can invoke user code through
indexed accessors and invalidate an assumption.

Function multi-versioning was evaluated but not selected. The epoch comparison
is the repeated cost this change can safely remove; cloning complete generic
and specialized function bodies would duplicate receiver/fallback control flow
and increase code size without removing the checks that JavaScript semantics
still require.

### Diagnostics

Pass `-v` or `--diagnostic-file <path>` to explain receiver-flow decisions.
`[ReceiverFlow]` records identify the compiled scope and final-LIR instruction,
then report:

- branch/loop merges with each predecessor fact and the joined result;
- mutable facts invalidated by calls, accessors, suspension, scope replacement,
  `finally` exits, or unsupported barriers;
- candidate facts retained at dynamic receiver sites;
- specialization candidates, loop depth, whether the receiver type is proven,
  and the resulting `guarded` or `retained-generic(cold)` action.

Events are emitted in deterministic instruction/kind/message order. Diagnostics
are collected only when one of these established channels is enabled, so normal
compilation does not allocate trace records or format diagnostic messages.

### Interprocedural summaries

Statically proven direct-only callables also receive receiver summaries for
their arguments and return values. Conflicting known call sites retain the
union of candidate types rather than forcing the callable ABI to one CLR type;
missing, non-candidate, or unconstrained paths remain explicit in the summary.
Direct-call results feed those summaries back into the caller's final-LIR flow
facts.

Captured entry facts are narrower. A closure must be proven direct-only and
non-escaping, and each call must either observe a lifetime-stable captured
binding or be immediately preceded in the same statement list by the captured
binding's assignment. If that ordering proof is unavailable, the closure
entry remains unknown. Unknown callbacks, exported/aliased callables, optional
calls, spread calls, async functions, generators, and invocation-context-
dependent functions do not contribute interprocedural receiver facts. Direct
ESM declaration exports are classified as escaping even when the export syntax
contains no separate identifier read.

## Current specialization surface

The String allowlist matches the previously supported early-binding surface:

- arity 0: `charAt`, `charCodeAt`, trim aliases, and case conversion;
- arity 1: character access, substring/slice operations, searches, and prefix
  or suffix checks;
- arity 2: substring/slice operations, searches, and prefix or suffix checks.

A call is specialized only when a fixed-arity helper preserves each argument's
existing representation. Otherwise the original generic call remains.

Numeric element reads on String candidates also have a loop-gated fast path.
For an in-range canonical integer index on a primitive String, it returns the
cached one-character string directly. A non-String receiver, non-canonical
index, or out-of-range index executes the original `ObjectRuntime.GetItem`
operation, preserving inherited numeric properties and other dynamic behavior.
Candidate evidence authorizes this checked helper but is never treated as
proof.

Array specialization currently covers `push`, `unshift`, `pop`, `shift`,
`slice`, and `splice`. Typed-array specialization covers `at`, `includes`,
`indexOf`, `lastIndexOf`, `join`, and `reverse`. Their fast paths require a
pristine realm-owned prototype-family epoch, no own member override, the
receiver's default intrinsic prototype, and—unless the flow fact proves one
exact type—a successful CLR type test. Failure of any guard performs the
original `ObjectRuntime.CallMemberN` operation. These post-flow fast paths
are emitted only at loop-nested call sites; eligible calls outside loops remain
generic to avoid cold-site IL growth.

## Performance

Run the focused benchmark with:

```bash
dotnet run -c Release \
  --project tests/performance/Benchmarks/Benchmarks.csproj -- \
  --guarded-string-intrinsics
```

The 2026-08-19 .NET 10 ShortRun measured the post-#1889 residual lookup cost:

| Path | Mean | Allocated |
| --- | ---: | ---: |
| Guarded proven `String.trim` | 7.835 ns | 0 B |
| Guarded uncertain `String.trim` | 8.069 ns | 0 B |
| Generic `CallMember0` `String.trim` | 193.275 ns | 88 B |

The guard removes about 96% of the generic dispatch time in this focused case
and eliminates its steady-state allocation. The uncertain receiver type test
adds about 0.2 ns while retaining the complete generic fallback.

The 2026-08-21 ShortRun added a focused boxed-String control:

| Path | Mean | Allocated |
| --- | ---: | ---: |
| Guarded String-object `trim` | 34.894 ns | 0 B |
| Generic `CallMember0` String-object `trim` | 183.714 ns | 32 B |

Both rows used N=3. This is causal microbenchmark evidence for the boxed
receiver arm, not an end-to-end application result.

The 2026-08-21 .NET 10 ShortRun isolated the epoch comparison inside a
256-iteration String loop on an Intel Xeon 6975P-C:

| Guard state | Per-call mean / median | Hoisted mean / median | Allocated |
| --- | ---: | ---: | ---: |
| Pristine hit | 6.790 / 6.791 ns | 3.449 / 3.432 ns | 0 B |
| Invalidated miss | 201.915 / 201.941 ns | 183.354 / 182.689 ns | 88 B |

Each row used N=3 and reports per-operation time through
`OperationsPerInvoke=256`. Hoisting removes about 49% of the pristine guard
cost and about 9% of the miss-path cost without changing allocation. The miss
path still performs ordinary member lookup and dispatch.

The 2026-08-19 local DefaultJob run of `dromaeo-object-string` on .NET
10.0.11 measured JROC execution at 22.03 ms (N=13) with 44.24 MB allocated
per module load. Compiling that exact fixture before and after receiver
candidate tracking produced the same 22,528-byte module assembly. String
candidate tracking remains code-generation-neutral until a later
specialization consumes its final-LIR fact.

On 2026-08-21, the exact `dromaeo-object-string` fixture provided the
end-to-end item-8 control. Before the numeric-index specialization, one
DefaultJob run measured 20.426 ms mean / 20.359 ms median (N=19). Three
post-change runs measured 20.190, 20.273, and 20.165 ms means (N=18, 15, and
17), averaging 20.210 ms; their medians averaged 20.255 ms. This is about a
1.1% mean and 0.5% median improvement, but the single baseline and small effect
make the end-to-end result noise-sensitive. Generated IL confirms the causal
change: five hot numeric reads now call
`GetStringElementWithFallback` instead of generic `GetItem`.

The generated fixture assembly grew from 22,528 to 23,040 bytes because its 37
uncertain guarded String method sites now include the safe boxed-receiver arm.
Per-load allocation was effectively unchanged across these runs.

### Final master-relative validation

The item-9 validation used the repository's branch-comparison workflow, which
checks out `master` and the PR branch on one GitHub runner, runs the baseline
first, and uploads both raw BenchmarkDotNet reports. Both candidate checkouts
were commit `3d07da395` on .NET 10.0.11:

| Workflow | Scenario | Master mean / median | PR mean / median | Mean change | Allocation change |
| --- | --- | ---: | ---: | ---: | ---: |
| [32458452258](https://github.com/tomacox74/js2il/actions/runs/32458452258) | `dromaeo-object-string` | 24.282 / 24.380 ms (N=15) | 23.807 / 23.773 ms (N=14) | -1.96% | -0.04% |
| [32458452751](https://github.com/tomacox74/js2il/actions/runs/32458452751) | `dromaeo-object-regexp` | 148.011 / 148.465 ms (N=39) | 145.743 / 145.870 ms (N=25) | -1.53% | -0.98% |

These are single sequential baseline/candidate runs, so the percentages remain
noise-sensitive. Generated IL supplies the causal checks:

- `dromaeo-object-string` changes five loop-hot numeric reads from generic
  `GetItem` to `GetStringElementWithFallback`; its assembly grows 22,528 to
  23,040 bytes and total method IL grows 9,119 to 9,797 bytes.
- `dromaeo-object-regexp` adds one loop-hot guarded Array `push`; its assembly
  remains 47,616 bytes while total method IL grows 16,680 to 16,798 bytes.
- `dromaeo-object-array` remains byte-identical at 10,752 assembly bytes and
  2,593 method IL bytes.

The first final-PR compile control exposed a separate regression: three
interleaved compilations of the 466 KB `ai-astar-data.js` fixture averaged
4.063 seconds on the PR versus 3.662 seconds on `master`, about 11% slower.
Receiver candidates had been recomputed inside the unrelated core type
fixed point, and normal compilation solved broad receiver flow even when no
eligible specialization had a candidate-producing path.

Item 9 moves candidate inference after core type convergence and gates the
normal flow solver by specialization eligibility plus backward candidate
reachability. Two subsequent interleaved five-run batches averaged 3.855
seconds for the PR and 3.834 seconds for `master` (+0.5%); medians were 3.884
and 3.816 seconds (+1.8%). The residual difference is within observed
same-host run-to-run variation. String and RegExp generated IL is
byte-for-byte unchanged by this compile-time gate.

Array and typed-array facts are consumed by the post-flow specialization pass.
On a controlled fixture containing one cold Array-candidate call and one call
inside a loop, the loop-depth gate reduced guarded sites from two to one and
total method IL from 498 to 413 bytes. The hot call retained its guard while
the cold call returned to the original generic instruction. Across five
existing cold typed-array generator fixtures, the gate removed 16 guards and
reduced total method IL from 4,704 to 3,699 bytes. The controlled assembly file
remained 4,608 bytes because both method bodies fit in the same PE file-alignment
bucket.

### Hoisting validation

The issue-1897 branch-comparison workflows checked `master` commit
`264edb8ad` against candidate commit `8f44bde92`. Each workflow ran its
baseline and candidate sequentially on the same GitHub runner with .NET
10.0.11:

| Workflow | Scenario / host | Master mean / median | Candidate mean / median | Mean change | Allocation change |
| --- | --- | ---: | ---: | ---: | ---: |
| [32466258013](https://github.com/tomacox74/js2il/actions/runs/32466258013) | `dromaeo-object-string` / AMD EPYC 7763 | 30.549 / 30.644 ms (N=17) | 27.511 / 27.227 ms (N=61) | -9.95% | +2,027 B (+0.004%) |
| [32466257837](https://github.com/tomacox74/js2il/actions/runs/32466257837) | `dromaeo-object-array` / AMD EPYC 9V74 | 6.064 / 6.134 ms (N=28) | 5.846 / 5.847 ms (N=13) | -3.60% | -3 B |

These are single sequential comparisons, so the percentages remain
noise-sensitive. Generated IL establishes the causal limit:

- `dromaeo-object-string` reduces dynamic `IsPristine` sites from 37 to 11.
  Total method IL grows only from 9,797 to 9,820 bytes, and both generated
  assemblies remain 23,040 bytes.
- `dromaeo-object-array` has zero `IsPristine` sites on both revisions and
  byte-for-byte identical decompiled IL: 2,593 method bytes in a 10,752-byte
  assembly. Its measured change is therefore not attributed to guard hoisting.

## Validation

Coverage includes:

- direct-helper and exact-fallback generator snapshots;
- replacement, accessor, deletion, inherited override, and aliased-call
  behavior;
- uncertain receivers that alternate between strings and ordinary objects;
- same-realm boxed Strings plus own-assignment, descriptor, custom-prototype,
  and cross-realm boxed-String fallbacks;
- cross-realm prototype isolation using one compiled module loaded twice;
- guarded numeric fallback conversion;
- zero-allocation guard plus `String.trim` fast-path execution;
- loop-gated numeric String indexing, including generic cold and invalid-index
  fallback behavior;
- whole-loop and barrier-delimited guard sharing, pinned assumption locals,
  exact fallback invalidation, exception-region exclusion, and unsafe
  coercion/accessor barriers;
- deterministic fixed-point effect propagation through direct calls, blocks,
  class static blocks, unknown calls, mutation, property reads, and suspension;
- the `dromaeo-object-string` fixture, including captured writes and deferred
  callbacks whose guarded method calls keep an `isinst string` plus exact
  generic fallback;
- targeted `built-ins/String/prototype/**` test262 tests.

The realm-owned invalidation contract is documented in
[Intrinsic prototype mutation epochs](../runtime/IntrinsicPrototypeMutationEpochs.md).
