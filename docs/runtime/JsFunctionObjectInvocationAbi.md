# JsFunctionObject invocation ABI

This document defines the compiler/runtime boundary for dynamic calls to
object-backed JavaScript functions. It complements the typed direct-call path:
when the compiler knows the exact callable and inferred CLR signature, it
continues to call that typed implementation directly.

## Argument transport

`JsCallArguments` is a readonly, non-`ref struct` value carrier:

- arities 0 through 5 use inline fields and allocate no argument array;
- arbitrary and spread calls use an already-materialized `object?[]`;
- `Count` preserves the distinction between missing and present arguments;
- `GetArgument(index)` returns JavaScript `undefined` (CLR `null`) when the
  index is outside the supplied argument count;
- `ToArray()` returns existing arbitrary storage or materializes inline
  storage only when array semantics are requested.

The arbitrary array is invocation-owned storage. Compiler spread lowering
creates it after evaluating the iterable, and host adapters must not mutate or
reuse it while the call can observe or retain an `arguments` object.

The carrier is deliberately not stack-only. It can safely cross async storage
boundaries, unlike a `Span<T>` or `ref struct`, and it never uses pooled mutable
arrays that could be corrupted by recursion, reentrancy, or retained
`arguments` objects.

`CallableOperations` exposes `Call0` through `Call5` for common arities and
`Call` for an arbitrary array. Construction is separate through `Construct0`
through `Construct5` and `Construct`; fixed construction entry points carry
`newTarget` explicitly.

## Invocation context

Every `JsFunctionObject` receives `this`, `JsCallArguments`, and (for
construction) `newTarget` explicitly. Generated adapters override
`RequiresInvocationContext` with `false` when the callable does not read
ambient call state. That path installs no `AsyncLocal` values and performs no
argument materialization.

Callables that use `arguments`, reflective callee state, or another runtime
feature that requires ambient invocation state retain the default value of
`true`. `CallableOperations` then installs receiver, packed arguments, callee,
and `newTarget` for the duration of the call and restores outer state in a
`finally` block. `RuntimeServices.GetCurrentArguments()` lazily materializes
inline arguments on first observation and returns the same array for the
remainder of that invocation.

## Generated adapter contract

A generated callee-shaped type keeps its inferred typed implementation as the
canonical method:

```csharp
public double __js_call_typed(double value) => value + 1d;

protected override object? CallCore(
    object? thisArgument,
    in JsCallArguments arguments)
{
    var value = TypeUtilities.ToNumber(arguments.GetArgument(0));
    return __js_call_typed(value);
}
```

The adapter performs missing/default/rest handling and conversion at the
dynamic boundary, then boxes the return only when re-entering generic
JavaScript value flow. Materializing a function object does not widen the
typed implementation. Additional call-site-specialized methods use the same
generated object type and identity.

Constructor adapters apply ECMAScript constructor-return rules separately.
Async and generator adapters return their JavaScript-visible Promise or
iterator object rather than exposing an internal state-machine result.

Legacy delegates remain supported through
`LegacyDelegateFunctionAdapter`. They materialize arguments when their current
delegate ABI requires an array; this is an explicit migration path, not the
generated function-object ABI.

## Arity evidence

Run the checked-in analyzer:

```bash
dotnet run --project tests/performance/Benchmarks/Benchmarks.csproj -- \
  --callable-arity-analysis
```

The 2026-08-05 analysis covered all 26 checked-in benchmark scenarios:

| Arity | Call expressions | Construct expressions |
| ---: | ---: | ---: |
| 0 | 441 (23.52%) | 45 (12.71%) |
| 1 | 983 (52.43%) | 161 (45.48%) |
| 2 | 410 (21.87%) | 30 (8.47%) |
| 3 | 29 (1.55%) | 99 (27.97%) |
| 4 | 12 (0.64%) | 7 (1.98%) |
| 5 | 0 | 2 (0.56%) |
| 6+ | 0 | 10 (2.82%) |

Thus arities 0 through 2 cover 97.82% of lexical benchmark calls, and 0
through 5 cover every benchmark call plus 97.18% of constructions. This is a
static call-site distribution, not execution-frequency weighting.

The same analyzer found 86 current runtime callback-dispatch references:
4 each at fixed arities 0 and 1, 5 at arity 2, 3 at arity 3, and 70 using the
arbitrary delegate dispatcher. This identifies later callback migrations where
fixed entry points can remove existing arrays.

## Allocation benchmark

Run:

```bash
dotnet run -c Release \
  --project tests/performance/Benchmarks/Benchmarks.csproj -- \
  --callable-abi --filter "*"
```

The local .NET 10 ShortRun on 2026-08-05 produced:

| Path | Mean | Allocated |
| --- | ---: | ---: |
| `JsFunctionObject` fixed arity 3 | 1.537 ns | 0 B |
| `JsFunctionObject` pre-materialized arbitrary arguments | 1.572 ns | 0 B |
| `JsFunctionObject` spread materialization | 12.161 ns | 48 B |
| legacy `Closure.InvokeWithArgs3` | 18.774 ns | 48 B |
| `JsFunctionObject` ambient-context fixed arity 3 | 325.786 ns | 304 B |

These microbenchmarks guard the ABI shape; end-to-end benchmark results remain
the authority for application performance. The ambient-context result also
makes planner classification important: generated adapters should opt into
ambient state only when their semantics require it.
