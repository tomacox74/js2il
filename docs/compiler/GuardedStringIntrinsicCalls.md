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
3. calls the exact fixed-arity String helper.

The cold path executes the original `ObjectRuntime.CallMember0..5` operation
with the original receiver and arguments. Argument expressions are evaluated
before the guard, so both branches preserve JavaScript evaluation order and
coercion behavior.

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

Candidate sets are currently analysis and diagnostics facts only. Existing
guarded String lowering continues to classify receivers from representation-safe
storage facts; a candidate never overrides a known non-string classification
or bypasses storage compatibility checks. In particular, `new String(...)`
produces a boxed String object and is not recorded as a primitive-string
candidate. A later `+=` can still establish a primitive-string result because
JavaScript addition coerces that object to a primitive.

Candidate sets also retain Array and typed-array observations for future
guarded specializations, but do not affect their lowering today.

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
The facts remain analysis-only and do not change generated IL.

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

The initial allowlist matches the previously supported String early-binding
surface:

- arity 0: `charAt`, `charCodeAt`, trim aliases, and case conversion;
- arity 1: character access, substring/slice operations, searches, and prefix
  or suffix checks;
- arity 2: substring/slice operations, searches, and prefix or suffix checks.

A call is specialized only when a fixed-arity helper preserves each argument's
existing representation. Otherwise the original generic call remains.

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

The 2026-08-19 local DefaultJob run of `dromaeo-object-string` on .NET
10.0.11 measured JROC execution at 22.03 ms (N=13) with 44.24 MB allocated
per module load. Compiling that exact fixture before and after receiver
candidate tracking produced the same 22,528-byte module assembly. This confirms
the current candidate analysis is code-generation-neutral.

## Validation

Coverage includes:

- direct-helper and exact-fallback generator snapshots;
- replacement, accessor, deletion, inherited override, and aliased-call
  behavior;
- uncertain receivers that alternate between strings and ordinary objects;
- cross-realm prototype isolation using one compiled module loaded twice;
- guarded numeric fallback conversion;
- zero-allocation guard plus `String.trim` fast-path execution;
- the `dromaeo-object-string` fixture, including captured writes and deferred
  callbacks whose hot calls must keep an `isinst string` plus generic fallback;
- targeted `built-ins/String/prototype/**` test262 tests.

The realm-owned invalidation contract is documented in
[Intrinsic prototype mutation epochs](../runtime/IntrinsicPrototypeMutationEpochs.md).
