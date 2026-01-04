# Captured Variables: Scopes ABI Facade (Design)

## Background

JS2IL historically compiled **AST → IL** directly. Closures and captured variables were supported via a **scope-as-class** model:

- Every JavaScript scope becomes a generated .NET reference type ("scope class").
- Captured bindings (and other field-backed bindings like hoisted function declarations) become **instance fields** on the scope class.
- Nested functions access parent scopes via an `object[] scopes` chain.

JS2IL is migrating to a **AST → HIR → LIR → IL** pipeline to improve correctness (proper analysis, fewer ad-hoc emission decisions) and reduce bugs.

A key adoption blocker is **captured variables / closure environments**. This document proposes a **clean ABI facade** that:

- Fully specifies the runtime calling convention for closure environments (the **ABI**).
- Provides a small set of data structures and lowering rules so both:
  - legacy emitters and
  - the new IR pipeline

can interoperate during migration **without sharing** `VariableRegistry` / `Variables` / `Variable`.

## Goals

- Define a **precise, testable ABI contract** for closure environments.
- Provide an IR-friendly abstraction to lower `BindingInfo` loads/stores into:
  - IL locals, or
  - `ldfld/stfld` on scope instances.
- Enable **legacy ↔ IR interop** while both pipelines generate code.
- Avoid leaking legacy DTO designs (Issue 152 class family) into IR lowering.

## Non-goals

- Changing the existing runtime model (scope-as-class) in the short term.
- Implementing new JavaScript semantics beyond what SymbolTable/BindingInfo already expresses.
- Solving all invocation forms (e.g., exotic `this` binding, `eval`, etc.).
- Encoding .NET/IL-specific decisions (casts, typed locals) into HIR/LIR or the ABI facade.

## Terminology

- **Scope class**: Generated .NET type representing a JS scope (global/function/block/class/method scope as applicable).
- **Leaf scope instance**: The runtime instance for the currently executing scope.
- **Parent scopes**: Scope instances for lexical ancestors.
- **Scopes chain**: The `object[] scopes` passed to a function (and stored into class instances) providing access to parent scopes.
- **Binding**: A variable/function/class binding represented by `BindingInfo` in the symbol table.
- **Captured binding**: A binding declared in an outer scope and referenced by an inner function/class/method; emitted as a scope-field.

## Existing (de-facto) ABI Conventions in JS2IL

This design intentionally aligns with the existing conventions so migration does not break.

### Method signature encoding

`MethodBuilder.BuildMethodSignature(...)` can add an optional leading `object[] scopes` parameter:

- If `hasScopesParam` is true, the *first* parameter is `object[]`.
- Remaining parameters are `object`.
- Return type is `object` (or `void` for constructors).

This matches the current function/arrow emission strategy.

### Class parent scope storage (`_scopes`)

If a class needs access to parent scopes, it contains:

- a private field: `_scopes : object[]`
- constructors store the incoming `object[] scopes` into `_scopes`
- instance methods load parent scopes from `this._scopes` (not from parameters)

`ClassesGenerator.DetermineParentScopesForClassMethod(...)` builds an ordered ancestor list (global first).

### IR pipeline placeholder

The current IR lowering emits `LIRCreateScopesArray(default, ...)` as a placeholder.

This document defines how that instruction (or an equivalent) must be materialized once environments are supported.

## ABI Contract (Authoritative)

This section is the **contract** that both pipelines must obey.

### 1) Callable kinds and how scopes are provided

We define a small set of callable kinds because the ABI differs materially between them.

#### 1.1 User function / arrow function (generated method)

**Signature**

- Static method.
- First parameter is `object[] scopes`.
- Then N JavaScript parameters as `object`.
- Returns `object`.

```csharp
static object Fn(object[] scopes, object p0, object p1, ...)
```

**Scopes source inside callee**

- Parent scopes are read from the `scopes` argument.

**Notes**

- Functionally, `object[] scopes` is only required when the callable needs access to parent scopes.
- Performance optimization: omitting `object[] scopes` when unused is tracked as a separate issue: https://github.com/tomacox74/js2il/issues/213

#### 1.2 Class constructor

**Signature (if class needs parent scopes)**

```csharp
instance void .ctor(object[] scopes, object p0, object p1, ...)
```

**Constructor prologue**

- Must store `scopes` into `this._scopes`.

**Signature (if class does NOT need parent scopes)**

```csharp
instance void .ctor(object p0, object p1, ...)
```

#### 1.3 Class instance method

**Signature**

```csharp
instance object M(object p0, object p1, ...)
```

**Scopes source inside callee**

- Parent scopes are read from `this._scopes`.
- There is no `object[] scopes` parameter.

**Note on class fields vs captured bindings**

ES6 class fields (instance and static) are emitted as .NET fields on the class type itself. They are accessed via standard `this.field` / `ClassName.field` patterns, **not** through the scopes chain. The scopes ABI only governs access to **lexically captured variables** from enclosing scopes (functions, blocks, global). The class scope itself has no runtime "scope instance" — it exists only as compile-time metadata. At runtime:

- Class instance fields → fields on `this`
- Class static fields → static fields on the type
- Parent scope variables → `this._scopes[index].field`

#### 1.4 Main/module entrypoint

No scopes parameter is required.

- The global scope instance (if needed) is typically represented as a leaf local (typed scope class) in the method body.
- When creating delegates or calling generated functions, the caller materializes the `object[] scopes` chain.

### 2) `object[] scopes` ordering

Ordering must be deterministic and shared.

#### 2.1 General rule (recommended)

`object[] scopes` is ordered from **outermost** to **innermost** ancestor scope:

- `scopes[0]` = global/module scope instance
- `scopes[1]` = next lexical ancestor
- ...
- `scopes[k]` = nearest lexical ancestor of the callee

**Null slot allowance (optimization)**

Individual elements of `scopes[]` are permitted to be `null` when the callee never accesses any captured variables declared in that particular scope.

- This is an optimization only; it must not change semantics.
- The callee’s lowering must never dereference a `null` slot because its `EnvironmentLayout` would not contain any `ParentScopeField` accesses that point at that slot.
- For the first implementation of this ABI, it is acceptable (and simpler) to always populate all slots that are part of the required chain.- To determine which slots are required (for future optimization): collect the set of `ParentScopeIndex` values from all `ParentScopeField` entries in `EnvironmentLayout.StorageByBinding`. Slots not in this set may be left `null`.
This is the simplest, scales to deep nesting, and matches the direction used by class method ancestor computation.

#### 2.2 Compatibility rule (current legacy behavior constraints)

Legacy `Variable.ParentScopeIndex` and `Variables.GetParentScopeObject(...)` historically assume:

- Global is always included at index 0.
- Some legacy call paths model only a subset of ancestors (often global + immediate parent).

**Frozen legacy layout contract (Case A)**

During Case A migration, we treat the legacy layout as frozen. Based on the legacy `Variables` constructors used by the legacy pipeline:

- **Top-level function (legacy-emitted):** the callee expects `scopes[0] = <global/module scope instance>`.
- **Nested function (legacy-emitted):** the callee expects:
  - `scopes[0] = <global/module scope instance>`
  - `scopes[1] = <immediate parent function scope instance>`

This contract is established by the legacy constructor:

`Variables(Variables parentVariables, string scopeName, IEnumerable<string> parameterNames, bool isNestedFunction)`

which assigns:

- `_parentScopeIndices[_globalScopeName] = 0` always
- `_parentScopeIndices[parentVariables._scopeName] = 1` only when `isNestedFunction` and the parent scope is not global

Important consequence:

- For regular nested functions, the legacy `object[] scopes` layout does not define indices for deeper ancestors beyond the immediate parent.
- Class instance methods are a separate ABI path: they use `this._scopes` and can include multiple ancestors (global-first), because the legacy pipeline supplies an explicit ordered parent list for class methods.

**Migration choice (Case A): generalized IR layout; legacy stays unchanged**

During migration, there will be two valid layouts:

- **Legacy layout**: whatever ordering a legacy-emitted callee was compiled against.
- **Generalized layout**: the outermost → innermost ancestor chain described in 2.1.

**Interop rule:** the caller must build the scopes array in the **callee’s declared layout**.

This allows IR to adopt the generalized layout immediately without requiring a full retrofit of legacy internal variable machinery.

### 3) Argument indexing (JS params vs IL args)

The IR layer frequently speaks in terms of JavaScript parameter index (0-based). The ABI defines how these map to IL arguments.

Define:

- `HasScopesParam` (bool): whether `object[] scopes` is present as a parameter.
- `IsInstanceMethod` (bool): whether the generated method is instance.

Then:

- IL arg 0 is `this` for instance methods.
- If `HasScopesParam` is true, the `scopes` parameter is the first non-`this` arg.

Mapping function (conceptual):

```text
JsParamToIlArgIndex(jsParamIndex) =
  (IsInstanceMethod ? 1 : 0) + (HasScopesParam ? 1 : 0) + jsParamIndex
```

For class instance methods: `HasScopesParam = false`, `IsInstanceMethod = true`.

For functions/arrow functions: `HasScopesParam = true`, `IsInstanceMethod = false`.

### 4) Scope-field access contract

Captured bindings are stored as **instance fields** on the declaring scope class.

This implies:

- Each captured binding must have a (scope type, field handle) pair.
- Loads/stores must use `ldfld` / `stfld` on the correct scope instance.

**Load patterns**

- Current/leaf scope field:

```il
ldloc.<leafScope>
ldfld <field>
```

- Parent scope field (from `object[] scopes`):

```il
ldarg.<scopesArg>
ldc.i4 <parentIndex>
ldelem.ref
castclass <scopeType>
ldfld <field>
```

Note: `scopes[parentIndex]` may be `null` only if the callee never emits any load/store that targets that slot.

- Parent scope field (from `this._scopes`):

```il
ldarg.0            // this
ldfld object[] _scopes
ldc.i4 <parentIndex>
ldelem.ref
castclass <scopeType>
ldfld <field>
```

**Store patterns**

`stfld` requires stack order: `[obj, value]`.

- For stores to a scope field, codegen must ensure the scope instance is on the stack before the RHS value.

### 5) Delegate ABI (runtime constraints)

The runtime supports a bounded set of closure delegate shapes.

Contract:

- Delegates are `Func<object[], object, ..., object>` (0–6 JS params).
- The first parameter is always `object[] scopes`.

If a call site cannot resolve a supported delegate shape, it must fall back to the runtime dispatcher (existing behavior).

## The Proposed Facade: Data Model

This section defines the new, clean abstractions that the IR pipeline (and optionally legacy) should use.

### Overview

The facade is split into:

1. **Callable ABI metadata**: how to interpret IL arguments and where to fetch scopes.
2. **Scope chain layout**: ordering and concrete scope types for each parent scope slot.
3. **Binding storage map**: for each `BindingInfo`, whether it is a local, leaf-field, or parent-field.

## LayoutKind (Case A migration)

During migration (Case A), we support **two layouts**:

- `LegacyScopesLayout`: whatever ordering and indexing a legacy-emitted callee expects.
- `GeneralizedScopesLayout`: the outermost → innermost chain specified in 2.1.

The key design point is that **layout selection is per callee** and is used at call sites and by the lowering that interprets parent-scope indices.

### Is LayoutKind runtime or compile-time?

`ScopesLayoutKind` is a **compile-time** concern:

- The compiler knows whether a callee is IR-compiled vs legacy-compiled when emitting a call site.
- The runtime does not need to discover layout kind dynamically.

Practically, it should live alongside other compilation metadata (for example, in the compiled method cache entry associated with a `BindingInfo`).

### Pseudocode: selecting a callee layout

```csharp
public enum ScopesLayoutKind
{
  LegacyScopesLayout,
  GeneralizedScopesLayout
}

public static ScopesLayoutKind GetScopesLayoutKind(CompiledCallable callee)
{
  // Case A: IR adopts generalized immediately.
  // Legacy callables keep their existing expectations.
  return callee.IsCompiledByIrPipeline
    ? ScopesLayoutKind.GeneralizedScopesLayout
    : ScopesLayoutKind.LegacyScopesLayout;
}
```

### Pseudocode: building `object[] scopes` at call sites

```csharp
public static object[] BuildScopesForCall(CallerContext caller, CompiledCallable callee)
{
  var kind = GetScopesLayoutKind(callee);

  return kind switch
  {
    ScopesLayoutKind.GeneralizedScopesLayout =>
      BuildGeneralizedScopesArray(caller, callee.RequiredScopeChain),

    ScopesLayoutKind.LegacyScopesLayout =>
      BuildLegacyScopesArray(caller, callee.LegacyScopeIndexMap),

    _ => throw new NotSupportedException()
  };
}
```

Notes:

- `BuildGeneralizedScopesArray(...)` must produce an array matching `ScopeChainLayout` (2.1), optionally using `null` slots as described in 2.1.
- `BuildLegacyScopesArray(...)` may continue to use the existing legacy construction behavior.
- This is intentionally **not** an IL-level concern. This is high-level ABI metadata used to ensure the caller supplies the callee’s expected shape.

### 1) `CallableAbi`

Represents the ABI shape of a compiled method.

Suggested structure:

```csharp
public enum ScopesSource
{
    None,
    Argument,     // object[] is a method parameter
    ThisField     // object[] stored in this._scopes
}

public sealed record CallableAbi(
    bool IsInstanceMethod,
    bool HasScopesParam,
    ScopesSource ScopesSource,
    int JsParameterCount,
    int MaxSupportedDelegateArity // currently 6
);
```

Invariants:

- If `ScopesSource == Argument`, then `HasScopesParam == true`.
- If `ScopesSource == ThisField`, then `IsInstanceMethod == true` and `HasScopesParam == false`.

### 2) `ScopeChainLayout`

Defines the meaning of `scopes[i]`.

```csharp
public sealed record ScopeSlot(
    int Index,
    string ScopeName,                    // module-qualified if needed
    TypeDefinitionHandle ScopeTypeHandle // for castclass
);

public sealed record ScopeChainLayout(
    IReadOnlyList<ScopeSlot> Slots
);
```

Key invariants:

- Slot indices are 0..N-1 contiguous.
- `Slots[0]` is the global/module scope.
- The order is outermost → innermost.

### 3) `BindingStorage`

Where a binding’s value lives at runtime.

```csharp
public enum BindingStorageKind
{
    IlLocal,
    IlArgument,        // used for JS params only (non-captured)
    LeafScopeField,
    ParentScopeField
}

public sealed record BindingStorage(
    BindingStorageKind Kind,

    // IlLocal:
    int LocalIndex = -1,

    // IlArgument:
    int JsParameterIndex = -1,

    // LeafScopeField/ParentScopeField:
    FieldDefinitionHandle FieldHandle = default,
    TypeDefinitionHandle DeclaringScopeType = default,

    // ParentScopeField:
    int ParentScopeIndex = -1
);
```

Important notes:

- Captured bindings should never be represented as `IlLocal` in the inner function; they are `ParentScopeField`.
- Bindings declared in the current scope that are captured by inner functions are `LeafScopeField` within the declaring scope.

### 4) `EnvironmentLayout`

The complete environment specification attached to a compiled callable.

```csharp
public sealed record EnvironmentLayout(
    CallableAbi Abi,
    ScopeChainLayout ScopeChain,
    IReadOnlyDictionary<BindingInfo, BindingStorage> StorageByBinding
);
```

## Algorithms (Facade Responsibilities)

### A) Computing captured bindings

Inputs:

- `SymbolTable` scope tree
- `BindingInfo` capture flags (`IsCaptured`)
- `Scope.ReferencesParentScopeVariables`

Output:

- For each callable scope, compute:
  - which bindings are captured
  - which ancestor scopes must be included in its `object[] scopes`

This already exists conceptually in SymbolTable analysis. The facade consumes those results and normalizes them.

### B) Computing `ScopeChainLayout`

Given a callable’s lexical parent chain, produce ordered ancestors:

1. Walk ancestors from the callable’s parent scope to root
2. Collect scope names
3. Reverse to get global-first order
4. Map each scope to its generated scope type handle

**Layout selection**

The facade must support producing different layouts during migration:

- For IR-compiled callables: produce the generalized layout (2.1).
- For legacy-compiled callables: do not change the callee’s expectations; call sites should continue to use legacy construction logic unless/until the legacy pipeline is migrated.

### C) Assigning `BindingStorage`

For each binding referenced in a callable:

- If binding is a JS parameter and not captured: `IlArgument`
- Else if declared in current scope and not captured: `IlLocal` (IR-friendly)
- Else if declared in current scope and captured by some child: `LeafScopeField`
- Else if declared in an ancestor and referenced here: `ParentScopeField` with `ParentScopeIndex` determined by the ancestor’s slot

### D) Call-site materialization of `object[] scopes`

Whenever a call site invokes a generated function delegate or binds a closure value, the facade must decide which scope instances to capture.

**Contract**

- The scopes array passed to a callee must satisfy the callee’s `ScopeChainLayout`.

**Null slot allowance**

- Call sites may choose to populate only the subset of slots that are actually required by the callee’s environment (leaving other slots as `null`).
- For the initial implementation, always-populated arrays are acceptable; leaving slots `null` is reserved for future optimization.

**Implementation strategy**

- Provide a helper that, given:
  - caller’s current leaf scope instance (if any)
  - caller’s available parent scopes (argument `object[]` or `this._scopes`)
  - callee’s required `ScopeChainLayout`

returns an `object[]` in the required order.

This should replace ad-hoc construction logic spread across emitters.

## IL Lowering Rules (HIR/LIR → IL)

When the IR pipeline needs to emit a binding load/store, it should not consult legacy `Variables`.

Instead it consults `EnvironmentLayout.StorageByBinding[binding]`.

### LoadBinding(binding)

- `IlLocal` → `ldloc`
- `IlArgument` → `ldarg` (using ABI mapping)
- `LeafScopeField` → load leaf scope local, then `ldfld`
- `ParentScopeField` → load scopes source (`arg scopes` or `this._scopes`), index, cast, `ldfld`

### StoreBinding(binding)

- `IlLocal` → `stloc`
- `IlArgument` → `starg` (used rarely; mostly default param init)
- `LeafScopeField` → ensure scope object then value then `stfld`
- `ParentScopeField` → ensure parent scope object then value then `stfld`

### Materialize scopes source

If `EnvironmentLayout.Abi.ScopesSource` is:

- `Argument`: load the scopes arg at computed index
- `ThisField`: load `this`, then load `_scopes` field

### Casting and typed locals (explicit boundary)

Casting and local typing are **.NET/IL emission concerns**.

- HIR and LIR must remain unaware of whether a cast is required.
- The ABI facade must not participate in local allocation policy (e.g., allocating a typed leaf-scope local) because that introduces unnecessary coupling between analysis/layout and IL emission.
- The facade *does* provide the **scope type handles** required by the IL emitter so it can emit `castclass` when it decides it is necessary (for example, when loading a scope instance from an `object[]` parameter or from `this._scopes`).

## Integration Plan (Migration)

### Phase 0: Introduce facade + ABI tests ✅ COMPLETED

- ✅ Add the facade types and a builder that consumes `SymbolTable`/`BindingInfo`.
- ✅ Add targeted tests that verify:
  - method signatures match the ABI
  - scope-chain ordering is deterministic
  - `_scopes` is present only when required

Additionally, introduce an explicit notion of **layout kind** for callables (conceptual):

- ✅ `LegacyScopesLayout`
- ✅ `GeneralizedScopesLayout`

This is not an IL concern; it is metadata used by call sites and by LIR→IL when interpreting indices.

**Implementation Details** (PR #226):
- Created facade types: `ScopesLayoutKind`, `CallableAbi`, `ScopeChainLayout`, `BindingStorage`, `EnvironmentLayout`
- Implemented `EnvironmentLayoutBuilder` that consumes `SymbolTable` and produces `EnvironmentLayout`
- Extracted `ScopeMetadataRegistry` from `VariableRegistry` for minimal handle lookup interface
- Refactored `VariableRegistry` to act as facade over `ScopeMetadataRegistry`
- Added DI singleton registration for both registries
- Created 32 comprehensive tests covering ABI contracts, scope ordering, and binding storage

### Phase 1: IR pipeline reads captured variables ✅ COMPLETED

- ✅ Extend HIR variable nodes to carry `BindingInfo` identity (already partially true).
- ✅ Update HIR→LIR lowering so loads/stores are explicit for non-SSA-captured values.
- ✅ Implement LIR→IL lowering for `BindingStorageKind.*ScopeField`.

**Implementation Details** (PR #229):
- Added new LIR instructions: `LIRLoadLeafScopeField`, `LIRLoadParentScopeField` for reading captured variables from scope objects
- Extended `HIRToLIRLower` to consult `EnvironmentLayout` and emit appropriate field load instructions
- LIR→IL emission generates correct IL patterns:
  - Leaf scope: `ldloc.X` → `ldfld`
  - Parent scope: `ldarg.X` → `ldc.i4 index` → `ldelem.ref` → `castclass` → `ldfld`
  - Class method parent scope: `ldarg.0` → `ldfld _scopes` → `ldc.i4 index` → `ldelem.ref` → `castclass` → `ldfld`
- Scope field loads are stackable and support inline emission in Stackify
- Added `LIRBuildArray` instruction to optimize console.log array creation using dup pattern (newarr → [dup, ldc.i4, ldarg/ldloc, stelem.ref]*)
- Wired `ScopeMetadataRegistry` through full compilation pipeline for field handle lookups

**Known Regressions** (tracked for Phase 2):
- Issue #211: ConsoleLogPeepholeOptimizer doesn't recognize `LIRBuildArray` pattern, causing some methods to use locals where they previously didn't (e.g., `ArrowFunction_DefaultParameterExpression` has 2 locals instead of 0)
- Issue #227: Obsolete LIR instructions (`LIRNewObjectArray`, `LIRBeginInitArrayElement`, `LIRStoreElementRef`) should be removed

**Note on `LIRCreateScopesArray`**: The existing `LIRCreateScopesArray` instruction is a placeholder that emits a 1-element array containing `null`. ~~When implementing captured variable support, it should be replaced or extended to accept the `ScopeChainLayout` (or equivalent metadata) so the LIR explicitly encodes which scope instances populate which slots.~~ **Update (Phase 1):** Captured variable reads are now implemented without modifying `LIRCreateScopesArray`. Read operations use dedicated `LIRLoadLeafScopeField` and `LIRLoadParentScopeField` instructions. The scopes array construction remains a placeholder; proper implementation is deferred to Phase 2 (write support) and Phase 3 (scopes materialization).

A richer instruction shape for future scopes array construction might look like:

```csharp
public record LIRBuildScopesArray(
    IReadOnlyList<TempVariable?> ScopeInstances,  // null = slot not populated
    TempVariable Result
) : LIRInstruction;
```

The IL emitter then simply iterates slots and emits `stelem.ref` for each populated slot.

### Phase 2: IR pipeline writes captured variables

- Ensure assignments to captured variables store to `stfld` rather than temp locals.
- Add closure tests where inner function mutates outer bindings.

### Phase 3: Consolidate call-site scopes materialization

- Move scope-array construction logic behind the facade for both pipelines.
- This reduces interop bugs where one side uses a different ordering or subset.

In Case A migration:

- Legacy call sites may continue to use legacy construction logic for legacy callees.
- For cross-calls, the caller must consult the callee’s layout kind and construct the correct `object[]` shape.
- Over time, call sites can be unified behind the facade without changing legacy callee expectations.

**Minimizing changes to the legacy pipeline**

To avoid regressions in legacy code that is being retired:

- Do not refactor legacy call-site logic wholesale.
- Only introduce the smallest localized change needed for legacy → IR cross-calls:
  - detect `ScopesLayoutKind.GeneralizedScopesLayout` for the callee
  - construct a generalized `object[] scopes` that matches the IR callee’s `ScopeChainLayout`

No common helper in `JavaScriptRuntime` is required for correctness; constructing arrays directly in emitted IL is acceptable.

**Callable metadata store**

The IR pipeline should introduce a new callable metadata registry keyed by `BindingInfo` (not string name), storing:

- `MethodDefinitionHandle`
- `EnvironmentLayout` (including `CallableAbi` and `ScopeChainLayout`)
- `ScopesLayoutKind`

This replaces the role of the legacy `FunctionRegistry` (which is string-keyed and lacks scope metadata) for IR-compiled callables. The legacy `FunctionRegistry` continues to serve the legacy pipeline during migration but should not be extended; the facade's metadata store is the forward path.

## Compatibility and Versioning

- The ABI contract in this document is the compatibility boundary.
- As long as both pipelines obey:
  - method signatures,
  - per-callee scope layout kind (legacy vs generalized),
  - `_scopes` semantics,

they can interoperate regardless of internal representations.

If the ABI changes for an already-emitted callee (e.g., changing a legacy-emitted callee to expect generalized indices), it must be treated as a breaking change unless all affected code is recompiled together.

## Open Questions

- Should the function `object[] scopes` always be present, or omitted when provably unused? (Tracked: https://github.com/tomacox74/js2il/issues/213)
- Do we generalize function scope chains to full ancestor chains immediately, or mimic the legacy “global + immediate parent” behavior until migration completes?
- No open question on casting/typed locals: those decisions remain in the IL emitter by design.

## Future Optimizations: Ephemeral Caller Scopes, Callee Captures

This document’s baseline model (ABI v1) matches current behavior: the caller typically constructs an `object[] scopes` and passes it to the callee, and callees may assume it is stable.

For performance, ABI v2 may evolve toward a model where **caller-provided scope chains are not guaranteed to be stable** (they may be reused, pooled, or otherwise treated as ephemeral by the caller).

### Key idea

- The **caller** provides a scope chain that is *valid for the duration of the call*.
- The **callee** is responsible for **capturing/stabilizing** any scope references it needs to outlive the call.

In other words: correctness does not rely on the caller passing an immutable array.

### Why this matters

The main correctness hazard in sharing/reusing scope arrays is **escape**:

- When a function value is created and stored/returned (closure), the environment must remain valid after the creating call returns.
- If the callee stores a reference to the caller’s scope array directly, and the caller later reuses/mutates that array, the closure would observe incorrect environments.

### Rule: “copy on escape”

If a callee needs to create a closure that will run later, it must bind using a **stable environment**:

- The callee must ensure the array passed to `Closure.Bind(..., boundScopes)` is a stable copy (or otherwise stable representation) of the required scope chain.
- The caller-passed `scopes` argument must be treated as potentially ephemeral.

This fits the existing runtime approach: `JavaScriptRuntime.Closure.Bind` captures the provided `boundScopes` inside the returned delegate. Under an ephemeral-caller model, `boundScopes` must be callee-owned/stable.

### Practical consequences

- Direct calls (no escaping) can potentially reuse shared arrays safely.
- Closure creation sites become the natural place to allocate/copy the exact environment needed.
- This optimization is strictly performance-related and should not change the ABI contract for scope ordering or binding storage; it only changes the **allocation/ownership** model of scope arrays.

### Interaction with null slot allowance

The “null slot” optimization (2.1) becomes even more valuable under copy-on-escape: closure creation can copy only the slots the closure actually uses, leaving other slots `null`.
