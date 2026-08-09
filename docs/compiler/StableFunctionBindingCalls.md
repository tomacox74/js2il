# Stable function-valued binding calls

JROC can lower eligible bare calls through a stable `const` binding directly to
the callable's predeclared `MethodDef`:

```javascript
const add = (left, right) => left + right;
const multiply = function (left, right) { return left * right; };

add(1, 2);
multiply(3, 4);
```

Symbol/HIR analysis also classifies the binding's complete runtime use set. If
every use is one of these proven direct calls, HIR marks the initialization
`DirectOnly`; HIR-to-LIR then omits the function-object construction,
initialization, binding store, and dead local. Callable discovery, generated
types, canonical bodies, and predeclared MethodDefs are retained.

When any use can observe identity, escape, or require generic callable
semantics, the object is still created once and stored normally. Proven direct
call sites may continue to target the same canonical body, so materialized
identity and shared mutable captures coexist with typed dispatch.

## Eligibility

The current optimization is deliberately const-only. The initializer must be
an `ArrowFunctionExpression` or `FunctionExpression`, and the call must be
proven to execute after initialization. The proof accepts:

- recursive calls from the initializer's deferred callable body;
- calls in a later statement of the same program/block, including calls inside
  a non-hoisted arrow, function expression, class, loop, or conditional.

It rejects forward calls, cross-case switch paths, same-statement ambiguity,
and calls inside hoisted function declarations that could run before the
initializer. Switch lexical bindings use the runtime TDZ sentinel so skipped
case initializers still throw `ReferenceError`.

The emitted `CallableId` matches Phase 1 discovery and retains the registry's
inferred parameter/return types and capture ABI.

Eligibility and callable semantics are derived during AST-to-HIR construction.
Eligible `HIRCallExpression` nodes carry a canonical stable direct-call target;
HIR-to-LIR lowering consumes that descriptor without inspecting the AST.

The complete-use materialization classification runs in symbol analysis and is
encoded on the HIR function/arrow value. LIR contains no AST-dependent escape
metadata and performs no AST inspection. A direct-only binding that reaches
ordinary value lowering is a compiler invariant failure rather than silently
becoming `undefined`.

## Materialization classification

The stable diagnostic categories are:

- `DirectOnly`: every runtime use has a canonical
  `HIRStableDirectCallableTarget`;
- `IdentityObservable`: a known use observes or exports the value;
- `UnknownMaterialize`: initialization, invocation, or callable semantics are
  not proven safe.

Diagnostics include deterministic use/direct-call counts and reason flags:

```text
[CallableMaterialization] direct => DirectOnly; uses=1; direct-calls=1; reasons=None
[CallableMaterialization] escaped => IdentityObservable; uses=1; direct-calls=0; reasons=Alias
```

Materialization is retained for non-call reads, exports, returned functions,
aliases, property/array storage, unknown higher-order arguments,
`call`/`apply`/`bind`, reflection, optional/spread calls, writes, captured value
reads, recursion, mutually recursive SCCs, `with`, named function-expression
self identity, async functions, and generators.

## Conservative fallbacks

The function-object path remains authoritative when direct equivalence is not
proven:

- `async` and generator callables;
- spread calls;
- arrows using lexical `this`, `new.target`, `super`, or `arguments`, including
  those uses in nested arrows;
- strict function expressions;
- function expressions using ordinary `this`, `arguments`, named self
  identity/recursion, nested-arrow invocation context, or a bound `with`
  environment.

An ordinary function expression's own bare-call `new.target` is safe: the
direct ABI passes `undefined`. Default and rest parameters keep their existing
direct-call ABI behavior. Nested ordinary functions and classes are separate
semantic boundaries and do not disqualify an otherwise eligible outer
callable merely because they use their own `this` or `new.target`.

## Validation

Focused execution and IL snapshots are under
`tests/Jroc.Tests/Function/Function_StableConstCallable_*` and
`Function_CallableMaterialization_IdentityAndCaptures`.
HIR descriptor eligibility coverage is in
`tests/Jroc.Tests/HIRStableDirectCallableTests.cs`.
Classification/diagnostic coverage is in
`CallableMaterializationAnalysisTests.cs`; exact emitted-allocation assertions
are in `CallableMaterializationEmissionTests.cs`.

The PrimeJavaScript guardrail proves that module initialization still directly
calls `ArrowFunction_L229C14::__js_call__`, while containing no
`ArrowFunction_L229C14/FunctionObject` construction, initializer, inferred-name
write, or local. The generated wrapper/body remains present for two-phase
planning. This removes one generated function-object allocation, its closure
scope-array setup, and its initialization work per module evaluation.

On the same host, the exact `PrimeExecuteBenchmark.Jroc_ExecuteOnly` benchmark
measured managed allocation falling from 410.78 KB on `master` to 408.86 KB
with materialization elimination, a reduction of about 1.92 KB per module
execution. Mean time was 29.29 ms versus 29.16 ms and is treated as unchanged
within benchmark noise. The deterministic IL guardrail remains the causal
evidence for the removed object, initialization, scope-array, store, and local.

The first implementation deliberately limits allocation elimination to stable
`const` arrow and anonymous function-expression bindings. Hoisted function
declarations, named function expressions, async/generator callables, and any
uncertain use continue to materialize conservatively.

The exact Dromaeo regexp pair can be checked with:

```bash
npm run perf:phased:regexp:il
npm run perf:phased:regexp:dry:il
```

The IL guardrail derives the modern arrow owner names from the fixtures,
compiles both exact scenarios, and verifies every source call to
`randomChar`/`generateTestStrings` in the modern fixture targets its canonical
`MethodDef`. It separately checks the hot helper bodies and the intentional
`fn()` dispatch inside `prep`/`test`; it does not use whole-assembly closure
counts.

At implementation time, the modern fixture was already direct before this
change and remained direct afterward: `randomChar` was 5/5 source call sites
and `generateTestStrings` was 34/34. Consequently that pair's IL is a
regression guardrail, not evidence that this change improved its benchmark.
The strict classic fixture remained on its existing function-object call path
(0/5 and 0/34 direct source call sites); it is reported as the paired control.

A local Dry run on 2026-08-09 used BenchmarkDotNet 0.15.8, Ubuntu 24.04.4,
Intel Xeon Platinum 8488C (4 logical cores), .NET 10.0.10, one launch, and one
measured iteration:

| Scenario/runtime | Mean | N | Allocated |
| --- | ---: | ---: | ---: |
| classic JROC | 636.857 ms | 1 | 170.33 MB |
| classic Jint prepared | 556.6 ms | 1 | 149.48 MB |
| classic Okojo | 4,697.8 ms | 1 | 1,754.13 MB |
| modern JROC | 707.037 ms | 1 | 173.87 MB |
| modern Jint prepared | 662.6 ms | 1 | 155.29 MB |
| modern Okojo | 4,758.2 ms | 1 | 1,756.86 MB |

Dry results are smoke evidence only. No compatible pre-change benchmark run
was measured, and the relevant modern IL did not change, so these numbers are
not presented as a performance improvement.
