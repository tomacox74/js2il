# Stable function-valued binding calls

JROC can lower eligible bare calls through a stable `const` binding directly to
the callable's predeclared `MethodDef`:

```javascript
const add = (left, right) => left + right;
const multiply = function (left, right) { return left * right; };

add(1, 2);
multiply(3, 4);
```

The function object is still created and stored at the declaration. Reads,
identity comparisons, metadata, callbacks, construction, `.call`, `.apply`,
and `.bind` therefore continue to observe the same JavaScript function object.
Only a proven-equivalent bare call site bypasses function-object dispatch.

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
`tests/Jroc.Tests/Function/Function_StableConstCallable_*`.
HIR descriptor eligibility coverage is in
`tests/Jroc.Tests/HIRStableDirectCallableTests.cs`.

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
