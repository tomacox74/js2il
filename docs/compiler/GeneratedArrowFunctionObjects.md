# Generated arrow function objects

Compiled arrow expressions now materialize their predeclared, callee-shaped
`JsFunctionObject` subclass directly. Module initialization no longer creates a
delegate and calls `Closure.BindArrow`, so repeated loads do not compile an
expression tree or `DynamicMethod` per arrow instance.

## Materialization

For each arrow evaluation the compiler:

1. Loads the typed shared environment references required by the generated
   constructor.
2. Captures lexical `this`, `new.target`, and `super` receiver/scope state only
   when the arrow body requires them.
3. Constructs the generated function-object type.
4. Initializes JavaScript-visible `name`, `length`, prototype, restricted
   property, and async-function metadata.

`ScopeArray` callables temporarily retain their positional scope array in
addition to typed captures because current resumable and multi-environment
canonical bodies still consume that ABI. Capture-free and single-scope arrows,
including the modern cube arrows, do not carry this compatibility field.

Central callable operations restore lexical invocation state in `finally`
blocks, preserving recursion, reentrancy, abrupt completion, and detached
`.call`/`.apply`/`.bind` behavior. Common runtime callback consumers now use
`CallableOperations` rather than requiring CLR delegates.

## Compatibility

Focused coverage includes:

- mutable and multi-scope captures;
- lexical `this` in constructors, methods, object properties, and callbacks;
- lexical `new.target`;
- lexical `super` from constructors and methods;
- default/rest/destructured parameters and full `arguments` materialization;
- async arrows and Promise handlers;
- function identity, metadata, `typeof`, `.call`, `.apply`, `.bind`, and
  non-constructability;
- Array/Iterator callbacks, timers, nextTick, EventEmitter, FS, networking,
  child processes, JSON callbacks, and FinalizationRegistry.

The compiled-function binder API has been removed. Runtime-owned built-ins and
host functions use explicit adapters, while generated async/generator step
delegates are private `CompiledContinuation` state and cannot become a
JavaScript function value.

Eligible bare calls through `const` arrow bindings can also bypass the
materialized object and target the canonical callable `MethodDef`. When symbol
and HIR analysis prove that every runtime use is such a call, evaluation omits
the otherwise unobservable object allocation entirely. Identity-observable or
uncertain bindings still materialize normally. See
[Stable function-valued binding calls](StableFunctionBindingCalls.md) for the
initialization proof and semantic fallbacks.

## Performance

Commands:

```bash
dotnet run -c Release \
  --project tests/performance/Benchmarks/Benchmarks.csproj -- \
  --callable-baselines --filter "*"

node scripts/runPhasedBenchmarkScenario.js dromaeo-3d-cube-modern
node scripts/runPhasedBenchmarkScenario.js dromaeo-3d-cube
node scripts/runCubePhasedGuardrails.js --dry --il-smells
```

Local results from 2026-08-06 used Ubuntu 24.04.4, Intel Xeon Platinum 8488C,
and .NET 10.0.10.

### Historical focused materialization

| Representation | Mean | Allocated |
| --- | ---: | ---: |
| Generated arrow object | 15.50 ns | 64 B |
| Pre-retirement compiled delegate binder | 152.064 us | 5,821 B |

Both rows used BenchmarkDotNet ShortRun with three measured iterations before
issue #1722 retired the binder. The current benchmark no longer invokes or
retains that API; it measures generated object allocation, repeated compiled
module loading, and a loaded direct-call loop instead. This historical table
must not be treated as a new #1722 before/after speedup claim.

### Modern cube before/after

The exact pre-change `master` commit was
`8d3061ff9e1b614f16542dcdafc9aaa65234ebc1`.
The generated-arrow implementation commit was
`afcf9988620fb2712bcd505f5676cdb373f5ada8`.

| `dromaeo-3d-cube-modern` JROC | Mean | N | Allocated |
| --- | ---: | ---: | ---: |
| Before generated arrow materialization | 9.658 ms | 46 | 3,002.22 KB |
| After generated arrow materialization | 5.437 ms | 35 | 2,667.02 KB |

On the same host, the new path is 43.7% faster and allocates 335.20 KB
(11.2%) less per operation. The current legacy cube result was 6.154 ms
(`N = 27`) and 3,121.01 KB, so the modern fixture now beats the legacy fixture
while allocating less.

Generated IL guardrails report no compiled-function binder sites and fourteen
generated arrow-object constructions for `dromaeo-3d-cube-modern`.
