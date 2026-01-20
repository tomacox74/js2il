# Async/Await Lowering Spec (JS2IL)

## Goals

- Support JavaScript `async function` and `await` by lowering to a resumable state machine.
- No blocking and **no manual message-loop pumping** in the runtime.
- Preserve JS2IL’s **scope-as-class** closure model and scopes-array ABI.
- Stay compatible with **CommonJS** execution (module wrapper; no top-level await).

## Non-Goals (initial implementation)

- Top-level `await` (ESM feature; explicitly out of scope).
- Full spec-accurate microtask/job semantics beyond what `JavaScriptRuntime` already provides.
- Cancellation, abort signals, async iterators, or `for await (...)`.

## Terminology

- **Async function**: `async function f() { ... }`, `async () => { ... }`, async methods.
- **Await point**: `await <expr>`.
- **Continuation**: code that runs after an await point.
- **Leaf scope**: the per-invocation scope object allocated for a callable (JS2IL “scope-as-class” instance).

## Semantics Summary

### `async function`

An `async` function always returns a `Promise`.

- If the body returns a value `v`, the returned promise resolves to `v`.
- If the body throws `e`, the returned promise rejects with `e`.
- `await x` suspends execution until `Promise.resolve(x)` settles:
  - if fulfilled with `v`, `await x` evaluates to `v` and execution continues
  - if rejected with `r`, the `await` expression throws `r`

### CommonJS constraints

- Parser/validator must reject `await` outside function bodies.
- Module-level code remains synchronous; async features are only inside callables.

## Lowering Strategy

### High-level shape

Lower each async function into:

1. An **outer entry method** (same callable signature) that allocates per-call state and returns a promise.
2. A **state machine** represented by:
   - a state integer stored in the leaf scope object
   - any temps/locals that must survive suspension stored in the leaf scope object
   - a `MoveNext` routine that runs the body until it hits an await point or completes
3. `await` lowers into a `Promise.then` chain that schedules `MoveNext` as a continuation.

The key property: the async function does not block; it returns immediately with its promise.

### Where state lives

All state that must survive across suspension lives on the **leaf scope instance** for the async function invocation.

This includes:

- `int _asyncState`
- `Promise _asyncPromise` (or the “deferred” returned by `Promise.withResolvers()`)
- awaited intermediate values that must be referenced after resumption
- values for `try/catch/finally` bookkeeping (see below)

Rationale: JS2IL already uses scope instances for closure lifetime; storing async state there keeps lifetime rules consistent and avoids separate heap allocations.

### Where `MoveNext` lives (chosen design)

JS2IL will implement async functions using a **static `MoveNext` method** on the function’s registry type.

- Signature shape: `static void MoveNext(object[] scopes)` (optionally `static void MoveNext(<ScopeType> scope, object[] scopes)` as an optimization to reduce casts).
- Rationale: simple metadata model, fits the existing callable plumbing, and keeps leaf scope types primarily as state containers.

Alternatives (not chosen):

- Instance `MoveNext` method on the leaf scope type.
- Storing a `MoveNext` delegate on the scope.

## Detailed Lowering

### Entry method

For an async callable `f(scopes, a, b, ...)`:

- Allocate leaf scope instance as usual (this is already required for stable scopes ABI when nested callables exist).
- Create a deferred `{ promise, resolve, reject }` using `Promise.withResolvers()`.
- Initialize state:
  - `_asyncState = 0`
  - store deferred resolve/reject
- Call `MoveNext(scopes)` once to start execution.
- Return deferred.promise.

### `await` lowering

At an await point `await expr` inside `MoveNext`:

- Evaluate `expr` to value `x`.
- Compute `p = Promise.resolve(x)`.
- Store any needed post-await locals into fields on the leaf scope.
- Set `_asyncState = <resumeStateId>`.
- Return from `MoveNext` after scheduling continuations:
  - `p.then(onFulfilled, onRejected)`

Where:

- `onFulfilled(v)` stores the resume value (if needed), then calls `MoveNext(scopes)`
- `onRejected(r)` stores the error (or rethrows into the state machine), then calls `MoveNext(scopes)`

### Completion

When `MoveNext` reaches a normal return:

- call deferred.resolve(returnValue)
- set `_asyncState = -1` (completed)

If `MoveNext` throws:

- call deferred.reject(thrownValue)
- set `_asyncState = -1`

## Worked Example: `for` loop with nested `await`

This example demonstrates the *shape* of the emitted types and the `MoveNext` logic when an async function contains a `for` loop and a nested `await`.

### Input JavaScript

```js
async function f(n) {
  let total = 0;
  for (let i = 0; i < n; i++) {
  total += await foo(await bar(i));
  }
  return total;
}
```

Key points:

- The loop body has **two await points** per iteration:
  - inner: `await bar(i)`
  - outer: `await foo(<innerResult>)`
- The loop variables (`i`, `n`, `total`) must survive suspension.

### Emitted Types (conceptual)

JS2IL already emits a scope class per callable. For an async callable, the leaf scope additionally holds async state and any surviving locals.

```csharp
// Scope-as-class for the invocation of f
sealed class Scopes.f
{
  // --- async state ---
  public int _asyncState; // -1 = completed, otherwise resume label

  // The deferred produced by Promise.withResolvers()
  public JavaScriptRuntime.PromiseWithResolvers _deferred;

  // --- locals that survive across await ---
  public object n;      // parameter (boxed)
  public double i;      // loop counter (unboxed where possible)
  public double total;  // accumulator

  // --- temps that survive across await ---
  public object _awaitedInner; // result of await bar(i)

  public object _awaitedOuter; // result of await foo(_awaitedInner)

  // (Optional) a pending-exception slot if/when try/finally lowering is supported
  // public object _pendingException;
}

static class Functions.f
{
  // Entry method: returns Promise immediately
  public static object f(object[] scopes, object n)
  {
    var scope = new Scopes.f();
    scope.n = n;
    scope.i = 0;
    scope.total = 0;
    scope._asyncState = 0;
    scope._deferred = JavaScriptRuntime.Promise.withResolvers();

    // Start execution synchronously
    MoveNext(scopes /* includes leaf scope slot */);

    return scope._deferred.promise;
  }

  // Static state machine driver
  public static void MoveNext(object[] scopes)
  {
    // Implementation shown below.
  }
}
```

Notes:

- The *exact* `scopes` layout and how the leaf scope instance is placed into it must follow the JS2IL scopes ABI.
- The numeric locals shown as `double` assume stable type inference; otherwise they are boxed as `object`.

### `MoveNext` Shape (pseudo-code)

This is the core of the lowering: a `switch` on `_asyncState` plus explicit scheduling of continuations via `then`.

```csharp
static void MoveNext(object[] scopes)
{
  var scope = (Scopes.f)scopes[0];

  try
  {
    while (true)
    {
      switch (scope._asyncState)
      {
        case 0:
          // initial entry falls through into the loop
          goto LoopCheck;

        case 1:
          // resumed after: await bar(i)
          // (the continuation has already stored the value into _awaitedInner)
          goto AfterInnerAwait;

        case 2:
          // resumed after: await foo(_awaitedInner)
          goto AfterOuterAwait;

        default:
          return;
      }

    LoopCheck:
      if (scope.i >= JavaScriptRuntime.TypeUtilities.ToNumber(scope.n))
      {
        scope._asyncState = -1;
        scope._deferred.resolve(scope.total);
        return;
      }

      // --- inner await: await bar(i) ---
      {
        object innerExpr = bar(scope.i);        // normal call lowering
        var p = JavaScriptRuntime.Promise.resolve(innerExpr);

        scope._asyncState = 1;

        p.then(
          onFulfilled: v => { scope._awaitedInner = v; MoveNext(scopes); },
          // In emitted IL this is another bound callable. For the minimal semantics
          // (no surrounding try/catch), it can directly reject the outer promise.
          onRejected:  r => { scope._asyncState = -1; scope._deferred.reject(r); }
        );
        return; // suspend
      }

    AfterInnerAwait:
      // --- outer await: await foo(awaitedInner) ---
      {
        object outerExpr = foo(scope._awaitedInner);
        var p = JavaScriptRuntime.Promise.resolve(outerExpr);

        scope._asyncState = 2;
        p.then(
          onFulfilled: v => { scope._awaitedOuter = v; MoveNext(scopes); },
          onRejected:  r => { scope._asyncState = -1; scope._deferred.reject(r); }
        );
        return; // suspend
      }

    AfterOuterAwait:
      // total += awaitedOuter
      scope.total = scope.total + JavaScriptRuntime.TypeUtilities.ToNumber(scope._awaitedOuter);
      scope.i = scope.i + 1;

      // Next loop iteration
      scope._asyncState = 0;
      continue;
    }
  }
  catch (JavaScriptRuntime.JsThrownValueException ex)
  {
    scope._asyncState = -1;
    scope._deferred.reject(ex.Value);
  }
}
```

Important details:

- Each `await` site is assigned a stable resume state id (`1`, `2`, …).
- The loop variables (`i`, `total`) live on the scope so they survive suspension.
- Nested awaits become sequential suspension points.
- In real emitted IL, the `onFulfilled` / `onRejected` lambdas are emitted as normal JS2IL callables (static methods + `Closure.Bind(scopes)`), not as C# closures.
- The rejection path should flow back into `MoveNext` with “throw into state machine” semantics; the pseudo-code uses `throw r` as shorthand.


## Exception Handling / try-catch-finally

`await` interacts with `try/finally` because the continuation must resume *inside* the correct handler region.

Initial implementation approach:

- Restrict `await` within `finally` blocks (validator error) OR
- Implement a structured lowering:
  - Convert try/catch/finally into explicit state-machine edges
  - Maintain a “pending exception” field on the scope
  - Ensure `finally` executes on both normal and exceptional paths

This spec recommends starting with restrictions to ship a minimal correct feature set, then expanding.

## IR and Codegen Integration

### Parser/Validator

- Keep `AllowAwaitOutsideFunction = false`.
- Add validator support for:
  - allowing `AwaitExpression` only within async functions
  - rejecting `await` in unsupported regions (initially `finally`, maybe `catch` depending on implementation)

### HIR / LIR

- Add `HIRAwaitExpression` (already represented) and lower it only when the enclosing method is async.
- Extend `MethodBodyIR` to carry async metadata (already has `IsAsync`).
- Add an async lowering pass before LIR-to-IL:
  - split method into basic blocks
  - introduce explicit suspension/continuation blocks
  - rewrite `await` into `Promise.resolve(...).then(...)` scheduling

### IL emission

- Reuse existing callable invocation ABI (`object[] scopes` + boxed `object` values).
- Ensure no `Promise.await` blocking helper is used.

## Required Runtime APIs

- `Promise.withResolvers()` returning `{ promise, resolve, reject }`.
- A reliable microtask/job queue mechanism (already present via scheduler).

## Testing Strategy

- Add runtime-level tests for `Promise.withResolvers()`.
- Add compiler execution tests that validate:
  - async function returns a promise
  - `await` suspends and resumes correctly (timer-based)
  - rejection propagates through await (throws)

Generator tests should verify IL changes are stable via snapshots.

## Follow-ups

- Expand handler support: `await` in `try/catch/finally`.
- Add `for await (...)` and async iterators.
