# ADR 0003: Class Method Scopes ABI for Resumable Callables (Wrapper + Internal Scoped Entrypoint)

- Date: 2026-01-24
- Status: Proposed

## Context

JS2IL implements closures via a `scopes` chain ABI (`object[] scopes`):
- Each JS scope is represented as a .NET class instance.
- Variables become fields on that scope instance.
- Captured variables are accessed via the scope chain.

JS2IL implements resumable execution (`async`/`await` today, `yield`/generators planned) using **scope-attached state** (see ADR 0002):
- A leaf scope instance stores a program counter (`_asyncState` / `_genState`) and spill slots.
- Resumption binds and re-invokes a compiled “move-next” function with the correct environment chain.

### Why classes are special

For non-resumable class instance methods, the natural model is:
- The constructor captures the declaring environment once (when needed) into an instance field like `_scopes`.
- Instance methods do not require a `scopes` argument; they can read `this._scopes` when accessing outer variables.

During Issue #343 work, async class methods introduced pressure to pass `object[] scopes` as an explicit method parameter to support state-machine binding/resumption. This creates a split-brain situation:
- The instance may have a captured `this._scopes`.
- The async method may also accept a `scopes` parameter.
- Call sites may supply an “empty scopes” fallback, which can diverge from `this._scopes` and is fragile.

This ADR chooses a stable ABI that:
- preserves the original intuition (“classes capture scopes at instantiation”),
- supports resumable semantics (async/generator),
- minimizes runtime dispatch heuristics and call-path-specific plumbing.

## Decision (proposed)

Adopt a **two-entrypoint pattern** for resumable class methods:

1) A **public stable method** with the normal JS signature (no `scopes` parameter).
2) An **internal scoped entrypoint** that takes `object[] scopes` as a leading argument and is used exclusively by resumable state machines.

### Instance resumable methods

For a JS instance method `m(a,b)` that is async or a generator:

- Public method:
  - `instance object m(object a, object b, ...)`
- Internal scoped entrypoint:
  - `instance object m__scoped(object[] scopes, object a, object b, ...)`

The public method implementation:
- loads scopes from `this._scopes` if present;
- otherwise uses an ABI-compatible empty/default scopes array;
- forwards to `m__scoped(scopes, ...)`.

The async/generator state machine:
- binds `_moveNext` to `m__scoped`;
- prepends the leaf AsyncScope/GeneratorScope to `scopes`;
- resumes by re-invoking `m__scoped` with the same (updated) scopes array.

### Static resumable methods

Static methods have no receiver to hold `_scopes`. For resumable static methods, capture the declaring scopes chain once:

- Emit a private static field on the class:
  - `private static object[] __js2il_class_scopes;`
- Initialize it during module/class initialization using the declaring environment.

Then apply the same wrapper pattern:
- Public static method has normal JS signature and forwards to `m__scoped(__js2il_class_scopes, ...)`.
- State machine binds/resumes `m__scoped`.

### Non-resumable methods

Non-async/non-generator methods remain unchanged:
- No scoped entrypoint.
- Methods that need outer variables read them from `this._scopes` (instance) or captured static scopes (static), as applicable.

## Consequences

### Positive

- **Single source of truth for environment**:
  - Instance methods always derive scopes from the receiver (or a single captured static field for static methods).
- **Stable public signatures**:
  - A method’s callable shape does not change because it is async/generator.
- **Reduced ABI leakage**:
  - `object[] scopes` becomes an internal compiler concern again.
- **Simpler and safer call paths**:
  - Typed/early-bound calls invoke the public method; only resumable plumbing targets `__scoped`.
  - Runtime reflection dispatch does not need to guess which overload expects scopes.

### Negative

- **Extra method emitted** per resumable method (`m__scoped`).
- **Wrapper overhead** for resumable calls (one additional call).
- **Static scope capture** requires careful semantics for classes declared inside functions (see Open Questions).

### Mitigations

- Wrapper overhead is small relative to async/generator overhead and can be optimized by inlining in the future.
- The internal entrypoint can be marked non-public (e.g., `private` or `assembly`) to reduce surface exposure.

## Alternatives Considered

### 1) Always require `object[] scopes` parameter on class methods

Pros:
- Uniform ABI for all callables.

Cons:
- ABI leaks into user-visible method signatures.
- Increases runtime dispatch complexity.
- Requires all typed/direct call paths to thread scopes correctly forever.

### 2) Never pass scopes; use only instance `_scopes`

Pros:
- Very clean instance story.

Cons:
- Static methods need a different environment channel.
- Async/generator leaf-scope prepending becomes indirect without an explicit scoped entrypoint.

### 3) Introduce an explicit `ExecutionContext` parameter everywhere

Pros:
- Cleaner than raw arrays; can carry `this`, strict-mode flags, etc.

Cons:
- Large refactor and touches most of the pipeline.

## Migration Plan (incremental)

1. Implement wrapper + `__scoped` entrypoint for **async instance methods**.
2. Update async binding/resumption to target `__scoped`.
3. Switch typed/early-bound member calls to target the public wrapper only.
4. Add static scope capture + wrappers for **async static methods**.
5. Reduce/remove runtime reflection heuristics that try scopes-leading overloads.

## Open Questions

- **Classes declared inside functions**:
  - If the enclosing function can be invoked multiple times, should `__js2il_class_scopes` be per invocation rather than a single static field?
  - Do we need “class objects” or factory patterns for correct per-instantiation capture?
- **ES class fields/initializers**:
  - How should initializers that reference outer scopes interact with captured environment?

## References

- [docs/compiler/Scopes_Classes_AsyncGenerator_Design.md](../compiler/Scopes_Classes_AsyncGenerator_Design.md)
- [ADR 0002](0002-resumable-state-scope-vs-stateobject.md)
