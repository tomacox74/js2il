# Async/Await Lowering: JS2IL vs C# Compiler vs TypeScript (ES5)

This document provides a concise, side‑by‑side comparison of how async/await is lowered by:
- **JS2IL** (JavaScript → .NET IL)
- **C# compiler** (C# → .NET IL)
- **TypeScript compiler targeting ES5** (TypeScript → JavaScript)

The goal is to highlight **state machine structure**, **exception handling**, **continuations**, and **observable semantics**.

## 1) At-a-Glance Summary

| Aspect | JS2IL | C# Compiler | TypeScript (ES5)
|---|---|---|---|
| Output target | .NET IL + runtime helper methods | .NET IL + BCL runtime helpers | ES5 JavaScript + helper functions
| State storage | Fields in a generated scope class + `_asyncState` | Fields in a struct/class state machine (`<>1__state`, etc.) | Local vars captured by generator state + `__awaiter`/`__generator`
| Continuations | Runtime helper (`Promise.SetupAwaitContinuation*`) | `AsyncTaskMethodBuilder` / `AwaitUnsafeOnCompleted` | `Promise` + `then` in `__awaiter`
| Exception handling | Try/catch lowered with async-aware resume labels; pending exception stored | Try/catch translated to state machine blocks with `try` regions | Exceptions propagate through generator `try` blocks and `Promise` rejection handling
| Await suspension | `LIRAwait` stores resume state, schedules continuation, returns (fulfillment stores result into `_awaited{awaitId}`) | `await` yields to builder, returns to caller | `yield` inside generator body
| Resume | Switch on `_asyncState` → labels | Switch on state → labels | `switch` in generator `step` function

## 2) JS2IL Lowering (JavaScript → .NET IL)

**Conceptual pipeline** (simplified):
1. Parse JS to AST.
2. Validate supported constructs.
3. Build symbol table / scope tree.
4. Generate scope classes and fields.
5. Lower to LIR, then emit IL.

**Core shape of the async state machine**:
- Each async function becomes a scope class with:
  - `_asyncState` (int)
  - `_deferred` (Promise-like handle)
  - `_moveNext` (delegate to resume)
  - `_awaited*` fields (store awaited *result* values)
  - `_pendingException` (used when await rejects inside `try/catch`)
- `LIRAwait` emits:
  - Allocate an **await ID** (for choosing `_awaited{N}` storage)
  - Allocate a **resume state ID** (for `_asyncState` dispatch)
  - Save resume state ID to `_asyncState`
  - Schedule continuations through `JavaScriptRuntime.Promise`
  - Return (async suspension)
- Resume uses a **state switch** on `_asyncState`, branching to the appropriate label.

Important: JS2IL intentionally keeps **await IDs** (result storage) separate from **resume state IDs** (control-flow dispatch). This allows additional synthetic resume states (e.g., async try/catch catch-resume) without shifting `_awaitedN` numbering.

**Try/catch with await**:
- JS2IL uses **async-aware catch resumption**:
  - Await rejection sets `_pendingException` and resume state to a **catch label**.
  - Upon resume, catch loads `_pendingException` and executes catch body.
- This avoids illegal branches into .NET `try` regions by resuming *outside* the protected region and routing control to catch explicitly.

**Key properties**:
- Model is close to a traditional IL state machine.
- Uses runtime helpers to bridge JS promises to continuation scheduling.
- Designed to maintain JS semantics (async function returns a Promise-like object).

## 3) C# Compiler Lowering (C# → .NET IL)

**Core shape**:
- Each async method produces a compiler‑generated **state machine struct/class** implementing `IAsyncStateMachine`.
- Key fields include:
  - `<>1__state` (int)
  - `<>t__builder` (AsyncTaskMethodBuilder / AsyncValueTaskMethodBuilder)
  - locals hoisted into fields if needed across `await`
- The method body is split into:
  - an **entry stub** that initializes the state machine
  - a `MoveNext()` method that contains the state machine switch and logic

**Await lowering**:
- `await` becomes:
  - Capture awaiter (`GetAwaiter()`)
  - If incomplete: save state, store awaiter, schedule continuation with builder
  - Return to caller
- Resume: `MoveNext()` checks state, restores locals, continues

**Try/catch with await**:
- The compiler transforms try/catch into state machine blocks and uses IL exception regions.
- It carefully structures `try` regions so resumption is legal and the continuation resumes at the correct block.

**Key properties**:
- Relies on BCL builder/awaiter pattern rather than custom runtime helpers.
- Strongly typed awaiters and results with minimal boxing.

## 4) TypeScript Lowering for ES5

**Core shape**:
- Uses helper functions (either emitted inline or from `tslib`):
  - `__awaiter` wraps a generator and returns a `Promise`
  - `__generator` builds a state machine using a `switch`
- `await` becomes a `yield` inside the generator body

**Example shape** (conceptual):
```js
function f() {
  return __awaiter(this, void 0, void 0, function* () {
    try {
      const v = yield g();
      return v;
    } catch (e) {
      return -1;
    }
  });
}
```

**Try/catch with await**:
- Preserved via `try/catch` inside the generator body.
- `__awaiter` converts `yield` into `Promise` chaining:
  - On fulfillment, resume generator via `next`.
  - On rejection, resume generator via `throw`.

**Key properties**:
- Pure JS lowering with no .NET IL involved.
- Semantics depend on JS Promises and generator protocol.

## 5) Behavioral Comparison

### 5.1 Suspension and Resume
- **JS2IL**: Stores resume state in `_asyncState`; returns; fulfillment continuation stores the resolved value into `_awaited{awaitId}` then calls `_moveNext`.
- **C#**: Stores awaiter in fields; schedules continuation via builder; resumes in `MoveNext()`.
- **TypeScript**: `yield` in generator; resume via `next`/`throw` from `__awaiter`.

### 5.2 Exception Flow
- **JS2IL**: Uses `_pendingException` for rejected awaits in try/catch; resume into catch label.
- **C#**: Handles exceptions through IL try/catch regions in `MoveNext()`.
- **TypeScript**: Rejection calls `throw` on the generator, letting `try/catch` handle it.

### 5.3 State Encoding
- **JS2IL**: `_asyncState` integer in scope class.
- **C#**: `<>1__state` integer in state machine.
- **TypeScript**: `label` integer in `__generator`’s state object.

### 5.4 Helper Runtime
- **JS2IL**: `JavaScriptRuntime.Promise` helpers.
- **C#**: `System.Runtime.CompilerServices` + `AsyncTaskMethodBuilder`.
- **TypeScript**: `__awaiter` and `__generator` (either emitted or imported).

## 6) Practical Implications

| Concern | JS2IL | C# Compiler | TypeScript (ES5)
|---|---|---|---|
| Performance overhead | Promise helpers + IL state switch | Optimized builders/awaiters | Generator + Promise overhead
| Debuggability | IL / snapshots | IL / PDBs | Source maps
| Semantics fidelity | Matches JS async promise behavior | Matches C# async semantics | Matches JS async semantics
| Interop | JS runtime helpers in .NET | .NET async ecosystem | ES5 runtime only

## 7) Notes for JS2IL Contributors

- JS2IL uses **scope-as-class** and fields for continuation state.
- Async lowering must respect .NET exception region rules; resumption points must avoid invalid branch targets.
- Await in try/catch requires explicit reject‑resume handling and a pending exception slot.

## 8) References (Internal)

- Async lowering spec: [docs/AsyncAwait_LoweringSpec.md](AsyncAwait_LoweringSpec.md)
- Try/catch strategy: [docs/CapturedVariables_ScopesABI.md](CapturedVariables_ScopesABI.md)
