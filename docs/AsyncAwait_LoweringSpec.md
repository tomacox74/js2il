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

### Where `MoveNext` lives (open design)

We have three viable options:

1. **Static method** on the function’s registry type
   - Signature like `static void MoveNext(object[] scopes)` or `static void MoveNext(<ScopeType> scope, object[] scopes)`
   - Pros: simple metadata model; reuse existing callable plumbing
   - Cons: must be careful to avoid boxing/casts for typed scope locals

2. **Instance method** on the leaf scope type
   - Pros: natural encapsulation; `this` is the typed scope
   - Cons: scope types are currently “data holders”; adding methods affects metadata layout and generator patterns

3. **Stored delegate** on the scope
   - Pros: easiest to schedule
   - Cons: delegate allocation and more runtime plumbing

Recommendation for first implementation: (1) static `MoveNext` plus a small runtime helper to create continuations.

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

- Decide `MoveNext` placement (static vs instance) once the first async state machine prototype exists.
- Expand handler support: `await` in `try/catch/finally`.
- Add `for await (...)` and async iterators.
