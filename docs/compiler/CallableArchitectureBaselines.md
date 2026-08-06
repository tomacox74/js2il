# Callable architecture baselines

This document defines the baseline used while replacing delegate-backed
compiled functions under issue #1707. It separates semantic compatibility,
generated signatures, callable materialization, and steady-state invocation so
later changes cannot trade one dimension for another without visible evidence.

## Boundary inventory

`CallableBoundaryInventory.json` classifies the current compiler and runtime
callable producers and consumers as direct calls, materialization, dynamic
calls, construction, callbacks, export/interop, or reflection. Each entry maps
to one or more #1707 implementation issues.

`CallableBoundaryInventoryTests` scans source files for the current callable
markers and fails when a detected boundary is not covered by the inventory. Add
or refine an inventory entry whenever a callable boundary moves or a new one is
introduced.

## Semantic baseline

`Function_CallableArchitecture_SemanticBaseline.js` locks the behavior that
must survive representation changes:

- function identity across repeated evaluations;
- shared mutable captures and independent closure instances;
- ordinary and lexical `this`;
- recursion and callback reentrancy;
- `new.target`, constructability, and arrow non-constructability;
- function `length` descriptors;
- proxy `apply`;
- runtime callback invocation.

The execution snapshot records observable behavior. The inferred-signature
generator snapshot below records representative IL and delegate materialization
paths so each migration can show exactly what changed.

## Inferred signature baseline

`Function_CallableArchitecture_InferredSignatureBaseline.js` and its generator
test inspect the emitted `__js_call__` methods directly.

| Callable | Emitted return | Emitted JavaScript parameters | Baseline purpose |
| --- | --- | --- | --- |
| `returnNumber` | `double` | none | Typed zero-argument numeric return |
| `returnBoolean` | `object` | none | Inferred boolean currently adapted through the generic return ABI |
| `negate` | `object` | `bool` | Typed boolean parameter with generic return |
| `returnString` | `object` | none | Inferred string currently adapted through the generic return ABI |
| `stringLength` | `object` | `string` | Typed string parameter with generic return |
| `identity` | `object` | `object` | Polymorphic fallback |

The object-return rows are intentional records of current adapter boundaries,
not the desired final architecture. Generated function-object work must retain
the typed rows and make any widening or boxing at the generic adapter boundary
explicit.

## Performance baseline

Run the focused BenchmarkDotNet pair:

```bash
dotnet run -c Release \
  --project tests/performance/Benchmarks/Benchmarks.csproj -- \
  --callable-baselines --filter '*'
```

`Legacy arrow delegate materialization` calls `Closure.BindArrow` for every
benchmark operation, while `Generated arrow object materialization` allocates
the replacement object-backed representation. Together they isolate the old
expression-tree/`DynamicMethod` cost from the new heap-object cost.

`Loaded module direct-call loop` loads the same module once during setup, then
invokes an exported loop containing 1,000 direct calls to a compiled function.
The single host invocation is amortized across those calls and
`OperationsPerInvoke` reports normalized per-call timing and allocation.

A reference ShortRun on 2026-08-05 used .NET 10.0.10 on Linux x64:

| Phase | Mean | Allocated |
| --- | ---: | ---: |
| Legacy arrow delegate materialization | 152.064 us | 5,821 B |
| Generated arrow object materialization | 15.496 ns | 64 B |
| Loaded module direct-call loop | 163.9 ns | 89 B |

These values are diagnostic reference points, not cross-machine pass/fail
thresholds. Compare changes on the same host and commit configuration, and
retain the end-to-end Dromaeo benchmarks for user-visible performance claims.
