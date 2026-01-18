# 0. Synchronous Generator Lowering Spec (JS2IL)

## 1. Goals

- Support ECMAScript synchronous generators:
  - `function* f() { ... }`, `function* () { ... }`, generator methods.
  - `yield <expr>` and `yield* <iterable>`.
- Preserve JavaScript semantics:
  - Left-to-right evaluation order.
  - Correct `next(value)`, `throw(error)`, and `return(value)` behavior.
  - Correct interaction with `try/catch/finally` inside generator bodies.
- Reuse JS2IL’s **scope-as-class** closure model and scopes-array ABI.
- Keep the runtime small and avoid reflection-heavy hot paths.

## 2. Non-Goals (initial implementation)

- Async generators (`async function*`) and `for await (...)`.
- Symbol keys (e.g., `Symbol.iterator`) beyond what JS2IL already models.
- Full spec-accurate iterator closing semantics for every exotic host iterable on day 1.

## 3. Terminology

- **Generator function**: `function* g() { ... }`.
- **Generator object**: result of calling a generator function; implements `next`, `throw`, `return`.
- **Suspension point**: a `yield` / `yield*` where execution pauses and control returns to caller.
- **Resume**: a subsequent `next/throw/return` call.
- **Leaf scope**: per-invocation scope instance allocated for the generator body (JS2IL “scope-as-class”).

### 3.1. ECMA-262 references (local docs)

Primary ECMA-262 sections relevant to synchronous generators:

- Index / navigation: [docs/ECMA262/Index.md](ECMA262/Index.md)
- Generator syntax + semantics:
  - Generator Function Definitions: [docs/ECMA262/Section15_5.md](ECMA262/Section15_5.md)
  - Method Definitions (generator methods are a form of method definition): [docs/ECMA262/Section15_4.md](ECMA262/Section15_4.md)
- Generator runtime objects:
  - GeneratorFunction Objects: [docs/ECMA262/Section27_3.md](ECMA262/Section27_3.md)
  - Generator Objects (`next`/`throw`/`return`): [docs/ECMA262/Section27_5.md](ECMA262/Section27_5.md)
- Iterator protocol and iterator closing used by `yield*`:
  - Operations on Iterator Objects: [docs/ECMA262/Section7_4.md](ECMA262/Section7_4.md)
- Statements that interact with generator control flow:
  - The `return` Statement: [docs/ECMA262/Section14_10.md](ECMA262/Section14_10.md)
  - The `throw` Statement: [docs/ECMA262/Section14_14.md](ECMA262/Section14_14.md)
  - The `try` Statement: [docs/ECMA262/Section14_15.md](ECMA262/Section14_15.md)

Coverage tracking reference:

- [docs/ECMA262/FeatureCoverage.md](ECMA262/FeatureCoverage.md)

## 4. Semantics Summary

### 4.1. Calling a generator function

Calling a generator function does **not** execute the body immediately.

Reference: [docs/ECMA262/Section15_5.md](ECMA262/Section15_5.md), [docs/ECMA262/Section27_5.md](ECMA262/Section27_5.md)

- It returns a generator object `gen`.
- The body runs only when `gen.next(...)` / `gen.throw(...)` / `gen.return(...)` is invoked.

### 4.2. Iterator result shape

Each `next/throw/return` returns an **iterator result** object:

- `{ value: any, done: boolean }`

JS2IL runtime will model this as an `ExpandoObject` (or a small dedicated runtime type), consistent with existing object-literal behavior.

### 4.3. `next(value)`

- On first `next(value)`, `value` is ignored (per JS semantics).
- On subsequent `next(value)`, `value` becomes the result of the paused `yield` expression.

### 4.4. `yield expr`

- Evaluates `expr`.
- Produces `{ value: <expr>, done: false }` and suspends.
- When resumed by `next(v)`, the `yield expr` expression evaluates to `v`.

Reference: [docs/ECMA262/Section15_5.md](ECMA262/Section15_5.md)

### 4.5. `throw(error)`

- Resumes the generator by throwing `error` at the suspended `yield`.
- If not caught in the generator, the call to `throw` rethrows to the caller and the generator becomes completed.

Reference: [docs/ECMA262/Section27_5.md](ECMA262/Section27_5.md), [docs/ECMA262/Section14_14.md](ECMA262/Section14_14.md), [docs/ECMA262/Section14_15.md](ECMA262/Section14_15.md)

### 4.6. `return(value)`

- Forces generator completion.
- Runs any pending `finally` blocks.
- Returns `{ value, done: true }`.

Reference: [docs/ECMA262/Section27_5.md](ECMA262/Section27_5.md), [docs/ECMA262/Section14_10.md](ECMA262/Section14_10.md), [docs/ECMA262/Section14_15.md](ECMA262/Section14_15.md)

## 5. High-level lowering strategy

### 5.1. Shape

Lower each generator function into:

1. An **outer factory method** matching the original callable signature that returns a runtime generator object.
2. A **state machine step method** (like async’s `MoveNext`) that:
   - runs until it hits a `yield` or completes
   - can be resumed with one of three operations: `next`, `throw`, `return`

### 5.2. Where state lives

All state that must survive across `yield` lives on the **leaf scope instance** for that generator invocation.

Reference: [ADR 0002: Resumable Callable State Storage (Scope-Attached vs Separate State Object)](adr/0002-resumable-state-scope-vs-stateobject.md)

Proposed approach: have the generated leaf scope type inherit from a runtime base class (e.g., `JavaScriptRuntime.GeneratorScope`) that provides the standard resumable fields.

Proposed fields on `GeneratorScope`:

- `int _genState` — program counter
- `bool _started` — whether first `next` has happened
- `bool _done` — completion flag
- `object? _resumeValue` — the value passed to `next(v)`
- `object? _resumeException` — exception passed to `throw(e)`
- `bool _hasResumeException`
- `object? _returnValue` — value passed to `return(v)`
- `bool _hasReturn`
- Spill slots for temps/locals that must survive across yields (same approach as async).

Rationale: matches JS2IL’s existing closure lifetime and avoids separate heap allocations.

### 5.3. Step method signature

A minimal, explicit interface avoids dynamic dispatch:

```csharp
// Conceptual signature (exact types TBD)
static object Step(object[] scopes, int op, object? arg)
// op: 0=Next, 1=Throw, 2=Return
// returns: iterator-result object { value, done }
```

An optimized variant can accept the strongly typed leaf scope to avoid casts:

```csharp
static object Step(Scopes.MyGeneratorScope scope, object[] scopes, int op, object? arg)
```

### 5.4. Runtime generator object

The factory method returns a runtime object that stores:

- the scopes array
- a reference to the compiled step function
- a small bit of per-generator bookkeeping (or reuse the leaf scope fields exclusively)

It exposes:

- `next(arg)`
- `throw(arg)`
- `return(arg)`

Each method calls into `Step(...)`.

## 6. Detailed lowering

### 6.1. Basic `yield`

Example:

```js
function* g() {
  const a = 1;
  yield a;
  yield 2;
  return 3;
}
```

Lowered conceptually into:

- Factory:
  - allocate leaf scope instance
  - build scopes array
  - return `JavaScriptRuntime.GeneratorObject` with `Step` delegate + scopes

- Step method:
  - `switch (_genState)`
  - case 0: init locals; set state=1; return `{ value: a, done:false }`
  - case 1: set state=2; return `{ value: 2, done:false }`
  - case 2: mark done; return `{ value: 3, done:true }`

### 6.2. `yield` expression result wiring

At each suspension point, the generator must resume with either:

- a **value** from `next(v)` that becomes the yield-expression result
- an **exception** from `throw(e)` that is thrown at the suspension point
- a **forced return** from `return(v)` that unwinds and completes

Plan:

- When `Step(op,arg)` is called:
  - if `op==Next` and not started: ignore `arg`
  - store `arg` into `_resumeValue` for later consumption
  - if `op==Throw`: store into `_resumeException` and set `_hasResumeException`
  - if `op==Return`: store into `_returnValue` and set `_hasReturn`

Then, immediately before executing code “after a yield”, emit:

- if `_hasReturn`: jump to a dedicated completion/unwind path
- else if `_hasResumeException`: clear flag and `throw _resumeException`
- else: use `_resumeValue` as the yield expression value

### 6.3. `try/catch/finally`

`finally` must run when:

- the generator completes normally
- the caller invokes `return(v)`
- the caller invokes `throw(e)` and it propagates out

Plan (phase-based):

- Phase 1: support `try/catch` around yields (propagate correctly).
- Phase 2: support `finally` by adding explicit unwind labels and storing an unwind “reason” in scope:
  - `int _unwindKind` (0=none, 1=return, 2=throw)
  - `object? _unwindValue`

This mirrors the async lowering style: switch-based resume + explicit labels for try regions.

### 6.4. `yield* iterable`

`yield*` delegates iteration to another iterator.

Reference: [docs/ECMA262/Section7_4.md](ECMA262/Section7_4.md), [docs/ECMA262/Section15_5.md](ECMA262/Section15_5.md)

Plan:

- Lower `yield* expr` into a small nested “delegation” sub-state machine:
  - Evaluate `expr` once
  - Get an iterator from runtime (`RuntimeServices.GetIterator(expr)` or similar)
  - Repeatedly call `.next(resumeValue)` and yield each produced value
  - If caller calls `throw(e)`, forward to inner `.throw(e)` if present; otherwise throw into outer generator
  - If caller calls `return(v)`, forward to inner `.return(v)` if present; otherwise complete outer
  - When inner is done, the `yield*` expression evaluates to the inner iterator’s completion value

This requires a runtime helper for “get iterator” and optional `throw/return` forwarding.

## 7. Compiler work breakdown

### 7.1. Parser / AST validation

- Accept `FunctionDeclaration` / `FunctionExpression` / `MethodDefinition` with generator flag.
- Validate `yield` only inside generator bodies.
- Validate `yield*` only inside generator bodies.

### 7.2. Symbol table / scopes

- Ensure generator bodies still follow scope-as-class rules.
- Spill across yields:
  - any local that is live across a `yield` becomes a field on the leaf scope.

### 7.3. IR

- Add HIR nodes:
  - `HIRYieldExpression` (value expression, location)
  - `HIRYieldStarExpression` (iterable expression, location)
- LIR:
  - `LIRYield` / `LIRYieldStar` or reuse a generalized “suspend” instruction, similar to async’s `LIRAwait`.

### 7.4. IL emission

- Add a generator-oriented orchestrator similar to `JavaScriptFunctionGenerator`:
  - Emits factory + step method
  - Emits a `switch` over `_genState`
  - Emits resume plumbing for Next/Throw/Return

### 7.5. Runtime additions (proposed)

- `JavaScriptRuntime.GeneratorObject`
  - stores scopes + step delegate
  - methods: `next`, `throw`, `return`
- `JavaScriptRuntime.IteratorResult`
  - either a tiny class/struct or an ExpandoObject creator helper
- Optional: `RuntimeServices.GetIterator(obj)` and helpers for `yield*`

## 8. Testing plan

Add tests under `Js2IL.Tests/Generator/`:

1. Basic yield sequence:
   - `g().next()` returns expected `{value,done}` triples
2. Passing values into `next(v)`:
   - `const x = yield 1; console.log(x);`
3. `throw(e)` into suspended generator:
   - caught and uncaught cases
4. `return(v)`:
   - completes and runs `finally`
5. `yield*`:
   - delegates over arrays and over another generator

Execution tests should validate observable JS behavior; generator tests can snapshot IL for stability.

## 9. Rollout phases

- **Phase 1 (MVP)**: `function*`, `yield`, `next()`, completion via `return` statement in body.
- **Phase 2**: `throw` and `return` methods on generator object; `try/catch/finally` support.
- **Phase 3**: `yield*` delegation semantics.
- **Phase 4**: Generator methods in classes/object literals + `super`/`this` interactions.

---

This document is a plan/spec and intentionally leaves some runtime/API naming flexible so we can fit it cleanly into existing `JavaScriptRuntime` patterns.
