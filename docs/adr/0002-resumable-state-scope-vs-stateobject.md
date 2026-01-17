# ADR 0002: Resumable Callable State Storage (Scope-Attached vs Separate State Object)

- Date: 2026-01-17
- Status: Proposed

## Context

JS2IL lowers certain JavaScript constructs into **resumable** execution:

- `async function` / `await` (already implemented)
- `function*` / `yield` / `yield*` (planned)

Resumable execution requires persisting state across suspension points:

- a program counter (state id)
- spill slots for locals/temps that are live across suspension
- bookkeeping for exceptional resume (`throw`) and forced completion (`return`)

JS2IL’s core compilation model is **scope-as-class**:

- Every JS scope becomes a .NET class.
- Variables become fields on that scope instance.
- Closures are implemented by allocating scope instances and passing them through a `scopes` array ABI.

Given this, we must choose where to store resumable state:

1. Attach resumable state to the **leaf scope instance** (per invocation) that JS2IL already allocates.
2. Allocate a **separate state-machine object** (activation record) to hold resumable state.

This ADR is about both async/await and synchronous generators because the tradeoffs are largely shared.

## Decision (proposed)

Prefer **scope-attached resumable state** as the default strategy:

- Store resumable state (e.g., `_asyncState` / `_genState` plus spill fields) on the **leaf scope instance** for that invocation.
- Keep any protocol wrapper objects minimal:
  - async: the returned `Promise` / resolver handles
  - generators: a small generator object exposing `next/throw/return` that calls into a compiled step function

This choice aligns with the existing JS2IL architecture and minimizes additional mandatory allocations.

## Consequences

### Positive

- **Avoids an extra allocation per invocation** in the common case.
  - The leaf scope instance must exist anyway for closure semantics.
- **Unified lifetime model**:
  - Variables captured by closures and variables spilled across `await`/`yield` share the same storage mechanism.
- **Simpler plumbing**:
  - No need to synchronize a separate state machine object with a scope instance.
- **Naturally supports closures** in resumable callables:
  - Resumed execution already has access to the correct scope chain via the scope instance.

### Negative

- **Leaf scope bloat**:
  - Adding resumable fields and many spill slots can increase memory footprint of the scope object.
- **Potentially worse locality**:
  - A monolithic scope object can mix unrelated fields (closure vars + spill slots + bookkeeping).
- **Harder to optimize with pooling / structs**:
  - Separate state objects are sometimes easier to pool or specialize (though pooling is complicated for JS semantics).

### Mitigations

- **Hybrid approach** (recommended even with scope-attached state):
  - Keep “protocol” objects small and store only what must persist in the scope.
  - For generators, keep iterator protocol bookkeeping minimal in the generator object and store spill slots/state in the scope.
- **Minimize spill fields**:
  - Only promote locals to scope fields when they are live across suspension points.
- **Typed locals optimization**:
  - Continue using strongly-typed scope locals where possible to avoid extra casts; keep step methods optimized.

## Alternatives Considered

### 1) Separate state-machine object per resumable invocation

**Description**: allocate an activation/state object that stores `_state` and spill slots; the scope instance only stores closure variables.

**Pros**

- Potentially **smaller leaf scope objects** for functions that are resumable but have few captured variables.
- State object can be **layout-optimized** specifically for the state machine.
- Clear conceptual separation: scope = closures, state object = suspension.

**Cons**

- **Extra allocation** per async invocation (and per generator invocation) becomes unavoidable.
- Additional indirection and plumbing:
  - step method must reference both the scope chain and the state object
  - careful lifetime handling needed when closures reference state
- Can become complex when closures and spilled locals overlap:
  - values may need to move between scope and state object or be duplicated.

### 2) .NET compiler-generated state machines (C#-style)

**Description**: emit a struct/class similar to C# async/generator state machines.

Rejected because JS2IL’s runtime model and ABI differs:

- Scope-as-class is already the closure mechanism.
- JS semantics (dynamic types, `this`, `arguments`, `with`-like patterns, etc.) complicate using idiomatic C# patterns directly.
- It would likely introduce significant churn in IL emission and runtime conventions.

## Notes

- This ADR does not lock in the exact runtime surface for generators. It only records the state-storage decision.
- If memory footprint becomes an issue, we can revisit and adopt more of the hybrid strategy (or selectively use a separate state object for specific cases).

## References

- [Synchronous Generator Lowering Spec](../SynchronousGenerators_LoweringSpec.md)
