# Proposed Design: Scopes, Classes, and Async/Generator State

Status: draft (design note / future TODO)

## 1. Background
JS2IL uses a **scope-as-class** model:
- Each JavaScript scope (global/function/block/class method scope) is represented as a .NET type.
- Variables are stored as fields on scope instances.
- Captured variables are implemented by passing around a **scopes chain** (`object[] scopes`) that contains scope instances.

Async functions and generators are compiled to **state machines** backed by specialized scope objects:
- `AsyncScope` for `async`/`await`
- `GeneratorScope` for `function*`/`yield`

Resumption uses a bound delegate ("moveNext") plus persisted state.

This note proposes a stable, coherent ABI for:
- scoping + closures
- classes (`this`, instance vs static)
- async/generator state machines

## 2. Goals
1. **Single source of truth for captured environment**
   - Avoid having both `this._scopes` and a separately-passed `scopes` argument that can diverge.
2. **Stable public surface**
   - Prefer keeping user-visible method shapes stable regardless of async/generator lowering details.
3. **No fragile call-site heuristics**
   - Avoid runtime reflection dispatch that “guesses” overloads by signature shape.
4. **Correctness first; optimization second**
   - Typed/early-bound calls must remain correct and not accidentally bypass required environment threading.

## 3. Current Model (as of Issue #343 work)
### 3.1 Non-async instance classes
- If a class needs parent scopes, the constructor captures them:
  - ctor receives `object[] scopes`
  - instance stores `private object[] _scopes`
- Instance methods typically do not require a scopes argument; they can read from `this._scopes` when needed.

### 3.2 Async/generator class methods (current direction)
To support async state machines for class methods, the implementation introduced a per-call `scopes` parameter on async methods:
- static async method: `M(object[] scopes, ...jsArgs)`
- instance async method: `M(this, object[] scopes, ...jsArgs)`

This can create a split-brain environment if the class also has a captured `_scopes`.

## 4. Problems Observed
1. **Dual environment channels**
   - Instance may have `this._scopes` (captured at construction) while async method also accepts `scopes`.
   - Callers may pass an “empty scopes array” fallback that does not match `this._scopes`.

2. **ABI leakage**
   - `object[] scopes` is an internal compiler ABI detail but becomes part of emitted method signatures.

3. **Dispatch complexity**
   - Runtime member call dispatch may need to prefer scopes-leading overloads.
   - Typed/early-bound call optimizations must be aware of “has scopes param” or IL becomes invalid.

4. **Maintenance risk**
   - Every call path (direct, typed, fallback) must remain consistent with method ABI.

## 5. Proposed Core Invariants
### Invariant A: One authoritative environment chain per receiver
- For instance methods, the authoritative captured environment is **`this._scopes`** (if present).
- For static methods, the authoritative captured environment is **the declaring scope’s environment**, not the call-site.

### Invariant B: State machines always resume with the same environment
- The state machine must always bind/resume using the exact environment chain that the original call used.

### Invariant C: Public-facing method signatures should not vary with async/generator lowering
- A JavaScript method `m(a,b)` should remain callable as `m(a,b)` regardless of being async or not.

## 6. Proposed ABI: “Public Wrapper + Internal Scoped Entrypoint”
### 6.1 Instance methods
Emit two CLR methods for async/generator instance methods:

1) **Public method** (stable signature, no scopes arg):
- `instance object m(object a, object b, ...)`

2) **Internal scoped entrypoint** (compiler ABI, used by state machine):
- `instance object m__scoped(object[] scopes, object a, object b, ...)`

The public method implementation:
- loads `scopes` from `this._scopes` if present
- otherwise uses an ABI-compatible empty/default scopes array
- calls `m__scoped(scopes, ...)`

The async/generator state machine:
- binds `_moveNext` to `m__scoped`
- prepends the leaf `AsyncScope`/`GeneratorScope` onto the scopes array
- resumes by calling `m__scoped` with the modified scopes array

Result:
- Callers never manufacture scopes.
- `this._scopes` and resumption scopes cannot diverge.

### 6.2 Static methods
Static methods have no `this`, so we need a stable source for scopes.

Proposed approach:
- For each class, emit a hidden static field capturing the declaring scopes chain:
  - `private static object[] __js2il_class_scopes;`
- Initialize it during module/class initialization using the module’s current scopes.

Then apply the same wrapper pattern:
- public static `m(a,b)` calls `m__scoped(__js2il_class_scopes, a,b)`
- async/generator machinery binds/resumes `m__scoped`

This keeps the public signature stable and avoids passing scopes as a public parameter.

## 7. Representation in IR/Metadata
With the wrapper approach, IR does not need “async methods have scopes param” as a public ABI rule.
Instead:
- The registry records whether a method has an internal scoped entrypoint.
- Typed/early-bound calls target the **public method** for normal JS calls.
- Only the async/generator compiler targets the scoped entrypoint.

This reduces the number of places needing `HasScopesParameter` propagation.

## 8. Runtime Dispatch
Runtime member calls should prefer calling the **public** stable signature.
- Reflection dispatch no longer needs to guess whether to pass scopes.
- The wrapper ensures correct environment threading.

Optional: keep support for scoped entrypoints for backward compatibility (but not required for correctness).

## 9. Migration Plan (incremental)
1. Implement wrapper + scoped entrypoint for **async instance methods** only.
2. Update async state machine binding to target `m__scoped`.
3. Update typed member-call optimizations to call the public method.
4. Add static method scope capture field + wrapper for **async static methods**.
5. Deprecate/remove reliance on runtime reflection choosing scopes-leading overloads.

## 10. Alternatives Considered
### Alt 1: Always require `object[] scopes` for all class methods
Pros:
- Very simple compiler ABI; uniform.
Cons:
- ABI leaks everywhere; higher coupling; more runtime dispatch complexity.

### Alt 2: Store scopes only in `_scopes` and never pass scopes as a parameter
Pros:
- Very “OO”: receiver carries all environment.
Cons:
- Hard for static methods; harder to represent nested scopes without explicit chain; async leaf-scope prepending becomes more indirect.

### Alt 3: Introduce an explicit `ExecutionContext` parameter
Pros:
- Cleaner than raw array; can include `this`, strict mode, etc.
Cons:
- Larger refactor; touches many pipelines.

## 11. Open Questions
- How should class static scope capture behave for classes declared inside functions (multiple instantiations of the enclosing function)?
  - Do we need per-instantiation “class objects” rather than static fields?
- Should `__js2il_class_scopes` be per class or per declaring scope instance?
- How to model ES2022 class fields initializers that reference outer scope variables?

## 12. Recommendation
Adopt **Public Wrapper + Internal Scoped Entrypoint** as the long-term direction.
It preserves the original intuition (“classes capture scopes at instantiation”) while still giving async/generator state machines a clean, reliable environment chain for resumption.
