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
construction) `newTarget` explicitly. `GeneratedFunctionObjectPlanner`
classifies each generated callable with independent `This`, `Arguments`,
`Callee`, `NewTarget`, and `LexicalSuper` requirements.

Synchronous generated functions whose canonical ABI can accept explicit state
receive a readonly `GeneratedInvocationContext` value after `newTarget`.
The generated adapter populates only the planned values. Ordinary-function
`this` is loaded from that parameter rather than
`RuntimeServices.GetCurrentThis()`. Strict `arguments` objects and rest
parameters consume the packed `JsCallArguments` from the same value and
materialize an array only when source semantics request one. `new.target`
continues to use its existing explicit canonical parameter.

Generated callables needing no invocation data, plus callables using the
explicit context, install no `AsyncLocal` values. Compatibility paths for
named-function identity, non-strict `arguments.callee`, bound `with`
environments, lexical `super`, and resumable state machines retain ambient
transport. `CallableOperations` and generated array adapters publish only the
values selected by `InvocationContextRequirements` in one immutable
`AsyncLocal` frame, then restore the previous frame in a `finally` block.
Inline arguments remain lazy and stable for the duration of an invocation.
See [Residual invocation-state transport](ResidualInvocationState.md) for the
thread-hop audit and issue #1890 decision.

Known spread/rest/`arguments` calls still end in the canonical typed MethodDef.
Their generated static array adapter receives the actual `JsFunctionObject`
alongside scopes and the existing argument array whenever the callable is
materialized. A proven direct-only rest callable passes no function object and
installs only argument state because no JavaScript-visible identity exists.
`RuntimeServices.PushGeneratedFunctionDirectCall` establishes only the
planned receiver, callee, arguments, `new.target`, and lexical-super values
still required by a compatibility path; the adapter restores that state with
`PopGeneratedFunctionDirectCall` from a `finally` block. This preserves
`arguments.callee` identity and nested-call isolation without creating a CLR
delegate.

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

Runtime-owned built-ins that still use CLR delegates are isolated behind
`BuiltinDelegateFunctionAdapter` and `BuiltinDelegateMetadata`. They may
materialize arguments when their explicit host ABI requires an array, but
compiled JavaScript functions never enter this path.

Generated built-in specialization must retain an exact generic fallback.
Realm-owned assumption tokens are exposed through
[`IntrinsicPrototypeEpochs`](IntrinsicPrototypeMutationEpochs.md); receiver
type alone is never sufficient to skip dynamic prototype lookup.

The public .NET hosting boundary projects callable values as runtime-owned
`Jroc.Runtime.JsCallable` wrappers. Calls, receiver-aware calls, construction,
and alternate `newTarget` construction marshal to the owning script thread and
then enter `CallableOperations`. Promise results can be bridged with
`CallAsync<T>`. Per-runtime weak-key caches preserve wrapper identity and unwrap
round-tripped wrappers to the original JavaScript callable.

CLR delegates entering JavaScript through hosting are first adapted to explicit
`JsFunctionObject` instances. Raw delegates remain supported only behind the
explicit built-in/host adapter boundary. Unannotated public CLR `object[]`
parameters are visible packed-argument parameters, not generated scope
payloads. Only explicit callable ABI metadata and known generated delegate
types retain the hidden scope convention. Callback results of type `Task` or
`Task<T>` are converted to JavaScript Promises and settled on the owning
runtime thread. CommonJS `ModuleMainDelegate` and `RequireDelegate` are
intentional internal bootstrap signatures, not public compiled-function
representations.

Async and generator step delegates are a separate private implementation
boundary. The compiler immediately encloses each one in
`CompiledContinuation`; that object is not callable, is never stored as a
JavaScript function value, and is allowlisted only in resumable lowering.

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

The local .NET 10 ShortRun on 2026-08-19 produced:

| Path | Mean | Allocated |
| --- | ---: | ---: |
| `JsFunctionObject` pre-materialized arbitrary arguments | 1.179 ns | 0 B |
| `JsFunctionObject` fixed arity 3 | 1.397 ns | 0 B |
| receiver-aware built-in adapter fixed arity 3 | 6.388 ns | 0 B |
| built-in delegate adapter fixed arity 3 | 8.633 ns | 0 B |
| `JsFunctionObject` spread materialization | 10.409 ns | 48 B |
| thread-static compatibility-frame prototype | 17.358 ns | 0 B |
| `JsFunctionObject` ambient-context fixed arity 3 | 70.840 ns | 200 B |

Issue #1890 also retains a benchmark-only reusable thread-static frame control.
It measures 0 B/call but is not used in production because it cannot preserve
point-in-time compatibility state across arbitrary CLR `ExecutionContext`
captures without reintroducing copy-on-write publication.

These microbenchmarks guard the ABI shape; end-to-end benchmark results remain
the authority for application performance. The ambient-context result also
makes planner classification important: generated adapters should opt into
ambient state only when their semantics require it.
