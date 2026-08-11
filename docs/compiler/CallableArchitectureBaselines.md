# Callable architecture baselines

This document defines the callable baseline after issue #1722 retired
delegate-backed compiled function values. It separates semantic compatibility,
generated signatures, callable materialization, repeated module loading, and
steady-state invocation so later changes cannot trade one dimension for
another without visible evidence.

## Boundary inventory

`CallableBoundaryInventory.json` classifies direct calls, object
materialization, dynamic calls, construction, callbacks, export/interop,
reflection, private continuations, host adapters, and bootstrap ABIs. It also
lists every intentional remaining delegate boundary and why it remains.

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
generator snapshot below records representative canonical MethodDefs and
generated function-object adapters so each migration can show exactly what
changed.

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

The stable dynamic-call ABI carries zero through five arguments in
`JsCallArguments` inline storage. Compiler IL snapshots cover variable,
expression-valued, named-member, computed-member, and dynamic construction
sites and require `InvokeFunctionCallWithArgs0` through
`InvokeFunctionCallWithArgs5`, `CallMember0` through `CallMember5`,
`CallComputedMember0` through `CallComputedMember5`, or `Construct0` through
`Construct5` as applicable. Arity six and spread remain explicit array
fallbacks.

`CallableArchitectureBenchmarks` includes loaded-module loops for dynamic
five- and six-argument calls. The pair keeps module loading and host invocation
outside the per-operation count, making the allocation difference between the
inline and array paths visible without presenting the arbitrary-arity fallback
as a semantic equivalent optimization target.

Run the focused BenchmarkDotNet suite:

```bash
dotnet run -c Release \
  --project tests/performance/Benchmarks/Benchmarks.csproj -- \
  --callable-baselines --filter '*'
```

`Generated arrow object materialization` allocates the object-backed
representation. `Repeated compiled module load` creates and disposes a runtime
for the same already-compiled assembly, covering repeated callable
materialization without recompilation.

`Loaded module direct-call loop` loads the same module once during setup, then
invokes an exported loop containing 1,000 direct calls to a compiled function.
The single host invocation is amortized across those calls and
`OperationsPerInvoke` reports normalized per-call timing and allocation.

## Issue #1763 validation

The dynamic five/six benchmark pair was run on the issue #1763 working tree on
2026-08-11 using .NET 10.0.10, BenchmarkDotNet 0.15.8, Ubuntu 24.04.4, and an
Intel Xeon Platinum 8488C host. Both rows used ShortRun with one launch, three
warmups, three measured iterations, and 1,000 JavaScript calls per benchmark
operation.

| Dynamic call path | Mean | Allocated |
| --- | ---: | ---: |
| Five arguments, inline ABI | 113.95 ns | 57 B/op |
| Six arguments, array fallback | 130.60 ns | 129 B/op |

The benchmark reuses preallocated object arguments, so the 72 B/op allocation
difference isolates the six-element argument array used by the fallback.
ShortRun timing had only three samples, so this result records allocation
evidence and does not claim a stable timing improvement. The raw reports are produced under
`tests/performance/Benchmarks/BenchmarkDotNet.Artifacts/results/`.

## Issue #1722 validation

The identical benchmark source and ShortRun harness were run on commit
`97525e41c`, the final callable-retirement working tree before the repeated-load
fix, and the fixed working tree on 2026-08-09. All results use .NET 10.0.10,
BenchmarkDotNet 0.15.8, Ubuntu 24.04.4, and the same Intel Xeon Platinum 8488C
host.

| Revision / run | Generated materialization mean / allocated | Direct loop mean / allocated | Repeated load mean / allocated |
| --- | ---: | ---: | ---: |
| `97525e41c` master | 18.55 ns / 72 B | 74.27 ns / 57 B | 472,319 ns / 67,976 B |
| Final retirement before fix | 26.78 ns / 72 B | 77.15 ns / 57 B | 726,443 ns / 69,839 B |
| Lazy require fix, run 1 | 18.21 ns / 72 B | 70.20 ns / 57 B | 410,354 ns / 66,507 B |
| Lazy require fix, run 2 (globally noisy) | 29.46 ns / 72 B | 109.88 ns / 57 B | 1,688,037 ns / 66,527 B |
| Lazy require fix, run 3 (globally noisy) | 21.76 ns / 72 B | 92.05 ns / 57 B | 555,229 ns / 66,506 B |

ShortRun timing was noisy on this shared host: the second and third fixed runs
also inflated the independent materialization and direct-loop controls. The
allocation result was stable across all fixed runs. Repeated load fell by
3,312-3,333 B from the regressed working tree and by 1,449-1,470 B from master,
while the quiet run was faster than master in all three phases. The fixed load
path no longer eagerly creates either per-module require adapter; direct
require calls retain the raw internal delegate ABI and materialize the stable
JavaScript adapter only when the require value is observed.
