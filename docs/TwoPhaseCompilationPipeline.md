# JS2IL Two-Phase Compilation Pipeline

This document describes a proposed **two-phase compilation pipeline** for JS2IL that is **eventually dependency-safe** across:

- Classes (constructors, methods) *(dynamic member-call dependency tracking such as `obj.m()` remains out-of-scope; see Milestone 2b1)*
- Function declarations/expressions
- Arrow functions

The goal is to make IR compilation reliable without relying on “compile-on-demand” during AST→HIR→LIR→IL.

> Status (Jan 2026): Two-phase compilation is always enabled. **Milestone 1**, **Milestone 2a**, **Milestone 2b**, **Milestone 2b1**, and **Milestone 2c** are implemented. Phase 2 compilation uses the Milestone 2b plan ordering (and treats the plan as authoritative).

This doc is written **ideal-first**:

- Treat the **Phase 1 / Phase 2** mechanics and the invariants as the target design.
- Any discussion of “today”, “migration”, or “legacy” is non-normative context for rollout.

---

## Current implementation snapshot (Milestone 1 + Milestone 2c)

JS2IL now has a real two-phase coordinator:

- **Phase 1: Discovery** runs via `CallableDiscovery` and populates `CallableRegistry` keyed by `CallableId`.
- **Milestone 2b/2b1: Planner** computes a dependency graph, SCC groups, and a deterministic stage order.
- **Phase 1 (token preallocation)** reserves `MethodDefinitionHandle` row ids for planned callables so Phase 2 can compile bodies without changing metadata ordering.
  - Anonymous callables (arrow functions + function expressions) use a deterministic **MethodDef row id reservation** strategy.
  - Class callables are preallocated before class type definitions so `TypeDef.MethodList` can point at a contiguous per-type block (ECMA-335 requirement).
- **Strict mode** is enabled before any body compilation so expression emission is **lookup-only** for callable tokens.
- **Phase 2: Body compilation (Milestone 2c)** compiles callable bodies in the planner order and then finalizes MethodDef rows deterministically.
  - Class callables are finalized grouped by class type (TypeDef order) to preserve per-type `MethodList` contiguity.
  - Function declarations are compiled in plan order and finalized as a deterministic contiguous block.
  - Anonymous callables are compiled in plan order after classes/functions are declared.
  - The main method body is emitted after `DeclareClassesAndFunctions(...)` completes (see `MainGenerator.GenerateMethodBody`).

What is *still incomplete* (post-Milestone 2c):

- The planner does not attempt broad dynamic receiver tracking (`obj.m()` / `obj["m"]()`); it only adds high-confidence member edges (`this.m()`, `super.m()`) per Milestone 2b1.
- Several legacy emitters/registries still exist alongside `CallableRegistry` (see Milestone 3).

---

## Why a two-phase pipeline

Today, compilation is effectively interleaved by generator call order (e.g., “declare classes, then functions, then arrows”). In practice, many of these “declare” steps **also compile bodies**, which creates ordering hazards:

- A class method compiled early might reference a function/arrow that hasn’t been compiled yet.
- IR lowering and/or IL emission can require a callable handle to exist (to emit `ldftn` + delegate construction).
- When caches aren’t populated yet, the IR pipeline may fail and fall back to legacy emission, which strict-IR tests disallow.

A two-phase pipeline fixes this by separating:

1. **Discovery + Declaration**: ensure *all* callables have stable identities and callable metadata.
2. **Body Compilation**: compile bodies in an order that respects dependencies.

The key idea: **IR emission should never trigger compilation**. It should only reference already-declared callables.

---

## Implementation sketch (what we would build)

This section describes **concrete implementation components** to migrate toward a strict two-phase model.

### New concepts to introduce

#### `CallableId`
A stable identifier for a callable, used everywhere (graph nodes, caches, diagnostics). It should be uniquely derivable from existing compiler data.

Recommended representation (record/struct):

- `CallableKind Kind` (`FunctionDeclaration`, `FunctionExpression`, `Arrow`, `ClassConstructor`, `ClassMethod`, `ClassStaticMethod`)
- `ScopeId DeclaringScopeId` (or equivalent scope identity)
- `string? Name` (for named callables: function name, class name, method name)
- `SourceLocation? Location` (line/column from AST) for stable naming of expressions
- `int JsParamCount` (or `MinArgs/MaxArgs` if needed)

This is not a runtime feature; it is strictly a compilation/planning key.

#### `CallableSignature`
The metadata needed to declare a callable and create an IL call target.

Example shape:

- `Type OwnerClrType` or `(TypeDefinitionHandle OwnerTypeHandle)`
- `bool RequiresScopesParameter`
- `int JsParamCount`
- `CallableInvokeShape InvokeShape` (e.g., `Func<object?, object?>`, `Func<object?, object?, object?>`, etc.)
- `MethodSignatureBlobHandle SignatureBlob` (optional cache for fast signature encoding)

#### `CallableRegistry`
Single source of truth mapping `CallableId` → declaration info.

Required operations:

- `Declare(CallableId, CallableSignature)`: ensures method/type metadata exists
- `TryGetMethodDefHandle(CallableId, out MethodDefinitionHandle)`
- `MarkBodyCompiled(CallableId)` for diagnostics/invariants

This is the abstraction that replaces “function cache vs arrow cache vs class registry in isolation” during planning.

##### `CallableRegistry` design proposal (responsibilities + separation of concerns)

`CallableRegistry` is one backing store, but it serves multiple phases. To keep it from becoming a “god object”, define it in terms of **role-focused views** with clear mutation rules.

**Core responsibilities** (what the backing store actually owns):

- **Catalog:** which `CallableId`s exist for the module being compiled.
- **Declarations:** the declared signature/descriptor needed to emit calls and delegate loads.
- **Diagnostics state:** whether a body has been compiled, plus any debug metadata (source location, display name).

**Non-responsibilities** (things that should *not* live here):

- Building the dependency graph / computing [SCC][scc]s (planner owns this)
- Emitting IL (LIR→IL owns this)
- Defining policy decisions like “late-bind within [SCC][scc]” (policy can consult registry state, but should not be embedded as ad-hoc logic)

**Proposed shape: one store, multiple views**

Expose the same underlying registry as different interfaces (or conceptual views in the implementation):

- `ICallableCatalog` (read-mostly)
  - `TryGet(CallableId, out CallableInfo)`
  - `IReadOnlyCollection<CallableId> AllCallables`
- `ICallableDeclarationWriter` (Phase 1 only)
  - `Declare(CallableId, CallableSignature)`
  - `SetToken(CallableId, CallableToken)`
- `ICallableDeclarationReader` (Phase 2+)
  - `GetDeclaredToken(CallableId)` (must succeed in strict mode)
  - `TryGetDeclaredToken(CallableId, out CallableToken)` (for migration/legacy fallback)

This allows Phase 1 code to have write access while Phase 2/LIR→IL consumers only get read access.

**Lifecycle and invariants**

- Discovery registers every callable into the catalog (Phase 1 precondition for strict mode).
- Declaration stores the token/descriptor for every callable before any body compilation begins.
- Phase 2 (body compilation and LIR→IL) must only read from the registry; it must not create/declare new callables.
- Strict invariant: every `CallableId` referenced by IR/LIR has a declared token/descriptor.

**Legacy adapters (migration-only)**

To avoid introducing separate caches (e.g., an arrow-function cache), legacy emitters can be supported with temporary adapters that map legacy keys → `CallableId`:

- `BindingInfo` → `CallableId(FunctionDeclaration)`
- `ArrowFunctionExpression` AST node → `CallableId(Arrow@loc)`

Adapters should live at the boundary (legacy emitter side), while the canonical storage remains `CallableId`-keyed.

#### `CompilationPlanner`
Responsible for:

1. Discovering all callables
2. Building a dependency graph
3. Computing [SCC][scc] + topo order
4. Producing a compile plan: ordered list of callables (or [SCC][scc] groups)

---

## Mapping onto existing code (where changes land)

This plan intentionally reuses today’s architecture, but makes “declare” actually mean **signature-only**.

### Entry point orchestration

Today:

- `MainGenerator.DeclareClassesAndFunctions` calls:
  - `ClassesGenerator.DeclareClasses` (creates types + compiles bodies)
  - `JavaScriptFunctionGenerator.DeclareFunctions` (creates function methods + compiles bodies)
  - `JavaScriptFunctionGenerator.DeclareArrowFunctions` (compiles arrows)

Proposed:

1. `DiscoverAllCallables(symbolTable, ast)`
2. `DeclareAllTypesAndMethods(callables)` (Phase 1)
3. `BuildDependencyGraph(callables)`
4. `CompileBodiesInPlanOrder(plan)` (Phase 2)
5. `CompileMain(ast)` (after bodies, or last [SCC][scc])

Concrete change: `MainGenerator.DeclareClassesAndFunctions` becomes a thin wrapper that calls a new coordinator (e.g., `TwoPhaseCompilationCoordinator`).

### Classes

Split class compilation into two operations:

- `DeclareClassTypes(SymbolTable)`
  - creates class type definitions and registers them in `ClassRegistry`
  - registers member *signatures* (ctor/method names + arity)
- `CompileClassBodies(plan)`
  - compiles constructors/method bodies according to the plan

This avoids today’s ordering hazard where class methods are attempted in IR before function/arrow caches exist.

Important metadata detail (ties back to current behavior):

- In `System.Reflection.Metadata`, a `TypeDefinition` row includes `fieldList` and `methodList` which are **table layout pointers** (the starting row index for that type’s fields/methods).
- This means `MetadataBuilder.AddTypeDefinition(...)` can be *called at any time*, but you can only do it safely “in advance” if you already know what values to use for `fieldList` / `methodList`.
- Practically, Phase 1 must handle this in one of two ways:
  - **Precompute layout:** discover all fields/methods for each type and add type definitions in an order where the starting handles are known deterministically.
  - **Defer emission:** use a builder that buffers member declarations and only writes the `TypeDefinition` row once it can compute the correct list starts (this is effectively what JS2IL’s type-building helpers do today).

### Function declarations

Split into:

- `DeclareFunctionDeclarations(SymbolTable)`
  - creates method definitions (or registers signatures for later memberref)
  - registers into `CallableRegistry`
- `CompileFunctionDeclarationBodies(plan)`
  - compiles bodies (IR-first, fallback as policy)

Important: avoid string-key-only registries for planning. `CallableId` should be derived from the `Symbol`/`BindingInfo`.

### Arrow functions

Split into:

- `DiscoverArrowFunctions(SymbolTable)`
  - enumerates all arrow expressions deterministically
- `DeclareArrowFunctions` (Phase 1)
  - creates method definitions/signatures for all discovered arrows
- `CompileArrowBodies` (Phase 2)
  - compiles arrow bodies according to the plan

This replaces the current “only compile a subset of arrows early” approach.

---

## Dependency discovery (how to build the graph)

### Where to collect dependencies

There are two practical options:

1. **AST-based discovery pass (recommended initially)**
   - walk each callable’s AST body, record references to other callables
   - lower risk: doesn’t depend on IR shapes

2. **HIR/LIR-based discovery**
   - build HIR for each callable, record dependencies as they appear
   - higher fidelity, but harder to keep stable while IR evolves

Start with AST discovery, and add IR-based refinement later.

### What counts as a dependency edge

For a caller callable `A`, record an edge `A -> B` when:

- `B` is referenced as a value inside `A` (delegate creation required)
  - `arr.map(toStr)`
  - `const f = toStr`
- `A` calls `B` (direct symbol resolution)
  - `toStr(x)`

Class-related dependencies (recommended phased approach):

- **Milestone 2b scope (safe subset):** record dependencies only when the target is **identifier-resolvable** via the symbol table.
  - `new C()` depends on `C` constructor signature availability **only when** `C` resolves to a class binding.
  - `C(...)` (rare, but possible if `C` is a callable binding) depends on `C` **only when** it resolves to a callable binding.
- **Milestone 2b1 scope (high-confidence subset):** member-call edges only where the target is statically knowable.
  - `this.m()` inside a class method can depend on the declared class method `m`.
  - `super.m()` can depend on the declared base class method `m` when the base class is identifier-resolvable.
- **Still deferred (post-2b1):** general property/member call edges such as `obj.m()` / `obj["m"]()`.
  - In general these are dynamic in JS, and attempting to infer them too early creates noisy graphs and oversized [SCC][scc]s.

Note: calls through unknown values (`x()`) do not create a compile-time dependency.

### How to resolve identifiers to `CallableId`

For each callable body, maintain a resolution context:

- `Scope` from the symbol table
- ability to resolve `Identifier` → `Symbol` / `BindingInfo`

Then map:

- function decl symbol → `CallableId` (kind `FunctionDeclaration`)
- arrow AST node → `CallableId` (kind `Arrow` + stable location)
- class method → `CallableId` (kind `ClassMethod` with `className + methodName` and scope id)
  - Note: member-call edges are intentionally narrow: `this.m()` / `super.m()` only. `obj.m()` remains treated as dynamic.

---

## SCC (Strongly Connected Component)

An **[SCC][scc] (Strongly Connected Component)** is a set of nodes in a directed graph where every node can reach every other node (possibly through intermediate nodes). In this design, the graph nodes are **callables** (functions, arrows, class methods/ctors), and edges are “A depends on B” references.

Why **[SCC][scc]s** matter here:

- They represent **cycles** in the callable dependency graph (mutual recursion or cross-kind recursion).
- A cycle means there is **no valid linear compile order** that satisfies “compile all of A’s dependencies before A”.

Practical challenges **[SCC][scc]s** introduce:

- **Ordering:** we can topologically order [SCC][scc]s, but not the callables *inside* a single [SCC][scc].
- **Codegen policy:** within an [SCC][scc] we may need a conservative rule (e.g., late-bind [SCC][scc]-internal invocation edges) to avoid relying on assumptions that only hold in acyclic graphs.
- **Debuggability:** without [SCC][scc] awareness, failures look like “missing callable token” or “ordering issue”; with [SCC][scc] awareness, we can diagnose “cycle detected” and apply a deliberate strategy.

---

## Compilation planning ([SCC][scc]/topo)

See [SCC][scc] for the definition and why it matters.

### Output plan representation

The planner should output a list of *stages*:

- each stage is either:
  - a single callable (acyclic)
  - or an [SCC][scc] group of multiple callables

Example:

1. [SCC][scc] #1: `Arrow(A1)`
2. [SCC][scc] #2: `Function(foo)`
3. [SCC][scc] #3: `{ Function(a), Function(b) }` (mutual recursion)
4. [SCC][scc] #4: `ClassMethod(C.m)`

### Policy inside [SCC][scc]s

When compiling an [SCC][scc] group, we should not require early-bound calls among members.

Two acceptable policies:

- Emit early-bound calls/delegates anyway (works because declarations exist), or
- Emit late-bound calls only for edges within the [SCC][scc] (more conservative)

This is a tuning knob for correctness vs. complexity.

---

## [SCC][scc] escape hatch (detailed)

This section explains the “escape hatch” strategy for **strongly connected components ([SCC][scc]s)** in the callable dependency graph.

### What an [SCC][scc] means in this compiler

An [SCC][scc] is a set of callables where each callable is reachable from every other callable in the set (directly or indirectly).

In JS2IL terms, [SCC][scc]s arise from:

- Mutual recursion between functions
- A class method referencing a function value that (transitively) references the class
- Arrows captured into variables that then feed back into other callables

Key point: [SCC][scc]s are not “bad”; they are a signal that **static, one-way dependency ordering is impossible**. We need a policy for codegen that remains correct.

### Why [SCC][scc]s are tricky even with Phase 1 declarations

If Phase 1 produces a resolvable token for every callable, we *can* still emit early-bound calls/delegates in a cycle.

However, [SCC][scc]s are where we often hit cases like:

- “I need a callable value to exist and be callable before I’ve finished compiling all participants.”
- “I want to inline/optimize assuming a target is known, but within [SCC][scc] that assumption may be fragile.”

So the escape hatch is a conservative policy to keep correctness and reduce implementation complexity.

### The escape hatch in one sentence

Within an [SCC][scc], allow codegen to **emit late-bound invocation for [SCC][scc]-internal edges**, while keeping [SCC][scc]-external edges early-bound.

This keeps most of the program early-bound and only degrades the cyclic portion.

### Planner output needed to support [SCC][scc] policy

The compilation planner should produce two artifacts:

1. **Stage order**: [SCC][scc]s in topological order
2. **[SCC][scc] membership map**: `CallableId -> SccId`

Additionally (recommended), the planner can produce an **edge classification function**:

- `IsSccInternalEdge(caller, callee) := SccId(caller) == SccId(callee)`

Codegen can then choose early-bound vs late-bound per call site.

### What we late-bind (and what we don’t)

We should scope the escape hatch to the smallest, safest subset:

- Late-bind only **invocation edges** that target a known callable inside the same [SCC][scc].
- Do **not** late-bind global intrinsic calls (Math/Array/etc.) or obviously non-cyclic operations.

Common cases:

- `a()` where `a` resolves to a specific declared function in the same [SCC][scc] → late-bound invoke
- `arr.map(a)` where `a` is a function value from same [SCC][scc] → still can be early-bound delegate load (see below)

### Late-binding mechanism options

There are two broad approaches. The design can start with the simplest.

#### Option 1: Late-bound invocation via runtime helper

Emit a call to a runtime helper such as `RuntimeServices.Invoke` (name illustrative) that takes:

- a function value (delegate or callable wrapper)
- `object[] args`

This works even if the target is not a direct method token at compile time.

Pros:

- Simple and robust
- Works for many JS call shapes

Cons:

- Slower than direct calls
- Requires a uniform callable representation (delegate or wrapper)

#### Option 2: Late-bound via indirection table (“cell”)

Represent each callable in an [SCC][scc] by a mutable cell that can be assigned once compilation completes.

- Phase 1 declares a static field (or scope field) that will hold the callable delegate
- Phase 2 compiles each method and sets its cell
- [SCC][scc]-internal call sites load the cell and invoke

Pros:

- Often faster than full dynamic dispatch
- Still structured and explicit

Cons:

- More moving parts (cells, initialization ordering)
- Must ensure initialization runs before use (or tolerate null checks)

### Early-bound delegate loads vs late-bound invocation

It’s useful to separate two concepts:

1. **Loading a callable value** (delegate creation): `ldftn` + `newobj`
2. **Invoking** (calling a function)

Even in [SCC][scc]s, it can be reasonable to keep **delegate loads early-bound**, because Phase 1 guarantees the token/signature exists.

The escape hatch can be limited to **invocation**. That provides a good balance:

- `arr.map(toStr)` still gets a fast delegate
- but `toStr(x)` might use late-bound invoke only when `toStr` is [SCC][scc]-internal and the policy says so

### How codegen decides (concrete decision table)

Given a call site inside callable `Caller`:

- If callee is an intrinsic / runtime helper → early-bound as today
- Else if callee is not statically resolvable (e.g., `x()` where `x` is unknown) → late-bound (already required)
- Else if callee resolves to callable `Callee`:
  - If `SccId(Caller) != SccId(Callee)` → early-bound
  - If `SccId(Caller) == SccId(Callee)` → late-bound invoke (escape hatch)

### Implementation hook points

The minimal places we need to thread [SCC][scc] knowledge:

1. **Planner** produces `SccId` mapping.
2. **Lowering / LIR generation** needs access to `SccId` mapping when choosing instruction forms.
3. **IL emitter** needs a late-bound invoke instruction.

Concretely, we can implement as:

- New LIR instruction: `LIRInvokeLateBound(TempVariable CalleeValue, IReadOnlyList<TempVariable> Args, TempVariable Result)`
- Or: `LIRCallFunction` gains a flag `ForceLateBound` computed by the planner.

If we want to keep LIR stable, the flag approach is simplest.

### Diagnostics

To make this debuggable (and avoid silent performance regressions), emit diagnostics per [SCC][scc]:

- [SCC][scc] id and members
- Which edges are late-bound (caller → callee)

In strict-IR tests, it can be useful to assert:

- late-binding only appears when `SccSize > 1` OR when target is not statically resolvable

### Safety constraints

The escape hatch must preserve the architectural rule:

- Late-binding must not perform compilation-on-demand.

It may consult registries/caches for tokens or cells, but compilation must already be complete (Phase 2) or in-progress within the [SCC][scc] stage.

---

## Phase 1: Declaration mechanics (how we “declare” without bodies)

### Callable references (chosen approach)

<a id="option-a"></a>
#### Option A (target): pre-create `MethodDefinitionHandle` for every callable

Mechanics:

1. Allocate the method definition up-front with a fixed signature.
2. Store `MethodDefinitionHandle` in `CallableRegistry`.
3. Later in Phase 2, emit body and attach via `AddMethodBody`.

Constraints:

- Requires knowing exact signatures early.
- Requires ensuring each method is associated with the correct owning type.

Findings (validated by PoC):

- A `MethodDefinitionHandle` (MethodDef token, `0x06xxxxxx`) is valid even if the method body IL has not been emitted yet.
  - Call sites can reference the MethodDef token as long as the final metadata includes the corresponding MethodDef row.
  - Practically this means: **declare/allocate method rows first, emit bodies later** (compiler-style).
- This pushes a hard requirement into Phase 1: **the layout of method and type rows must be deterministic and precomputed**.
  - In `System.Reflection.Metadata`, method ownership is inferred from `TypeDefinition.methodList` ranges, not from an explicit “owner” column.
  - Therefore, Phase 1 needs to decide the complete ordering and row ranges for:
    - `TypeDefinition` rows
    - `MethodDefinition` rows (per type)
    - (and similarly `FieldDefinition` rows if fields are used)

Type handle note:

- A similar “pre-allocate handles first” approach can be used for `TypeDefinitionHandle` (TypeDef token, `0x02xxxxxx`) if we want stable type tokens early.
- In many cases we do not need to embed TypeDef tokens into IL (unless we emit `ldtoken`/reflection-style constructs), but we still must emit TypeDefs in metadata to own methods/fields.
- As already noted earlier in this document: `MetadataBuilder.AddTypeDefinition(...)` requires correct `fieldList`/`methodList`, so pre-allocating TypeDef handles implies the same **precomputed table layout** requirement.

---

#### Option A subset (implemented in Milestone 2a): reserve MethodDef handles for anonymous callables

This is the current approach for **arrow functions** and **function expressions**.

Intent:

- Switch the token model used by `ldftn` / delegate creation from `MemberReferenceHandle` to `MethodDefinitionHandle`.
- Keep Phase 1 **body-free** (no IR lowering, no IL body emission).

Mechanics (current implementation detail):

1. Phase 1 discovery collects all anonymous callables.
2. Phase 1 assigns each anonymous callable an **expected** `MethodDefinitionHandle` by reserving row ids (i.e., predicting what handle will be returned later when methods are actually added).
3. Before compiling those anonymous callables in Phase 2, the coordinator sanity-checks that the number of MethodDefs added by other declaration steps matches what was assumed during reservation.
4. When each anonymous callable body is later compiled, the generator verifies that the real `MethodDefinitionHandle` allocated by metadata emission matches the reserved handle.

Important nuance / gotcha:

- Any additional synthesized methods emitted before anonymous callables are compiled (e.g., class `.cctor` generated for **static field initializers**) must be accounted for in the reservation math, otherwise the sanity check will fail.

What this is *not*:

- This is not “true Option A” (where Phase 1 emits real MethodDef rows that are later given bodies). Instead, it is a deterministic reservation strategy that keeps Phase 1 metadata emission requirements minimal.

---

<a id="option-b"></a>
#### Option B (implemented in Milestone 1): pre-create `MemberReferenceHandle` for anonymous callables

This was the pragmatic approach used by the coordinator for **arrow functions** and **function expressions** in Milestone 1.

Mechanics:

1. Phase 1 discovery records every callable as a `CallableId` in `CallableRegistry`.
2. Phase 1 declaration creates:
  - a `TypeReference` to `Functions.<ILMethodName>` (the type will be emitted later), and
  - a `MemberReference` to `<ILMethodName>(object[] scopes, object p1, ..., object pn) -> object`.
3. Store that `MemberReferenceHandle` in `CallableRegistry`.
4. Enable strict mode so expression emission can only *lookup* tokens.
5. Phase 2 later emits the real method body (and may register a `MethodDefinitionHandle` for the same callable, overwriting the token).

Why this exists:

- It avoids the “precompute all MethodDef row ownership/layout” constraint of Option A.
- It still satisfies the Milestone 1 invariant for anonymous callables: `ldftn` and delegate creation never trigger compilation.

## Phase 2: Body compilation mechanics

### Compiling a single callable

For each callable in the plan:

1. Build HIR (if IR is enabled for this callable)
2. Lower HIR→LIR
3. Emit IL from LIR
4. Attach method body to the declared method

If any step fails, the fallback policy is decided centrally:

- strict-IR tests: no fallback (fail)
- production default: fallback to legacy emitter for that callable only

### Compiling Main

Main should be compiled after all callables (or after the [SCC][scc] that contains Main’s dependencies).

This guarantees:

- callable references for `LIRLoadFunction` / `LIRLoadArrowFunction` always resolve
- no on-demand compilation

---

## Changes required in IL emission (tie back to current behavior)

### Loading callables as values

When lowering emits `LIRLoadFunction` / `LIRLoadArrowFunction`, IL emission must be able to produce:

- `ldnull`
- `ldftn <method token>`
- `newobj instance void class [System.Runtime]System.Func`...

Under Option A, `<method token>` is a `MethodDefinitionHandle`.

Under the current Milestone 2a implementation (Option A subset for arrows/function expressions), `<method token>` is a `MethodDefinitionHandle`.

(Historical: under the Milestone 1 Option B implementation for arrows/function expressions, `<method token>` was a `MemberReferenceHandle`.)

The delegate signature must match `JsParamCount`.

### Calling methods

Direct calls can remain as:

- `call` (static)
- `callvirt` (instance)

But when a direct target cannot be resolved (unknown receiver type, [SCC][scc] edge policy), emit dynamic runtime dispatch as today.

---

## Migration plan (more concrete)

This is intended to be implemented incrementally and keep the system runnable.

### Legacy AST→IL pipeline: file-by-file changes


### Milestone 1: Coordinator + Phase 1 declarations (completed)

Implemented:

- `CallableId`, `CallableSignature`, `CallableDiscovery`, `CallableRegistry`
- `TwoPhaseCompilationCoordinator` to orchestrate Phase 1 + Phase 2
- Phase 1 declaration for **arrows/function expressions** using Option B (MemberRefs) (historical Milestone 1 implementation; now superseded by Milestone 2a)
- Strict-mode enforcement so expression emission is lookup-only for those anonymous callables

Non-goals for Milestone 1:

- Preallocating `MethodDefinitionHandle`s in Phase 1 (Option A). (This was deferred at the time; anonymous callables are now handled in Milestone 2a via the Option A subset reservation strategy. Full Option A for all callables remains deferred.)

Usability expectation:

- After Milestone 1, the compiler should remain in a **fully usable state**.
- The current behavior (“try IR first, then fall back to legacy AST→IL if IR fails”) remains, but the **IR attempt moves into Phase 2 body compilation** rather than happening during declaration.

Legacy AST→IL impact:

- **Minimal / mostly orchestration.** Existing generators still compile bodies; the key Milestone 1 behavior is that *anonymous callable token resolution* is lookup-only in strict mode.

### Milestone 2: Dependency graph + ordering (completed)

Milestone 2 is the point where “two-phase” becomes *dependency-correct*, not just “declare tokens early”.

Recommended sequence (to minimize churn):

- **Milestone 2a: Switch anonymous callables to `MethodDefinitionHandle` (Option A subset)** (completed)
  - Anonymous callables (arrow functions + function expressions) now use a `MethodDefinitionHandle` token model.
  - Phase 1 remains body-free.
  - Implementation uses deterministic **MethodDef handle reservation** with a sanity check to ensure later emission matches the reserved handles.
  - Accounts for synthesized `.cctor` methods introduced by class static field initializers.

- **Milestone 2b: Dependency discovery + planner** (completed)
  - Add an AST-based dependency collector that maps identifier references (via the symbol table) to `CallableId` edges.
  - Compute [SCC][scc] groups + a deterministic topo order of groups.

- **Milestone 2b1: Class-aware dependency edges (member calls)** (completed)
  - Extend dependency discovery to include **class member dependencies** only where they are *high confidence*.
  - Recommended initial rules:
    - Include `new C()` edges as above (identifier-resolvable class bindings).
    - Include `this.m()` edges inside a known class/method body when `m` resolves to a declared method on the same class.
    - Include `super.m()` edges when the base class is identifier-resolvable and `m` resolves to a declared method on that base class.
    - Keep `obj.m()` / `obj["m"]()` out-of-scope unless the receiver is proven to be a specific class (e.g., by a dedicated intrinsic typing rule).
  - Keep the planner unchanged; this milestone is about improving the **graph**, not changing compilation order semantics.

  Notes:
  - Typed intrinsics and broader receiver-type inference are intentionally deferred to later milestones/optimizations.

- **Milestone 2c: True Phase 2 planned compilation** (completed)
  - Compile callable bodies in planner order.
  - Treat the plan as authoritative: if Phase 1 discovers a callable that is missing from the plan, compilation fails fast.
  - Move function declarations and class callables (constructors/methods/accessors/static initializers) to the Phase 2 body-compilation model.
  - Preserve ECMA-335 metadata ordering invariants:
    - Class method bodies are compiled in plan order but **finalized** grouped by class type so each `TypeDef.MethodList` points at a contiguous block.
    - Main method emission happens after planned callables are compiled (via `MainGenerator.DeclareClassesAndFunctions` → `GenerateMethodBody`).

Legacy AST→IL impact:

- **No semantic changes to AST→IL emission are intended.** The change is *when* it runs:
  - body compilation is invoked by the coordinator in planned order.
  - main is emitted after callable bodies so expression emission never needs to compile.

### Milestone 3: Finish unifying callable lookups (remaining work)

Milestone 2c makes Phase 2 compilation ordering plan-driven, but some token/metadata lookups are still split across multiple registries.

Remaining work for Milestone 3:

- Make `CallableRegistry` the single source of truth for callable tokens at all call sites (remove remaining name-based fallback paths).
- Reduce (or eliminate) reliance on separate registries for callable lookup (`FunctionRegistry`, class constructor caches) where practical.
- Consolidate the “body-only compilation” surface:
  - Prefer one API shape (e.g., `TryCompileCallableBody(...)` returning `CompiledCallableBody`) for both IR-first and legacy fallback compilers.
  - Keep legacy AST→IL emitters as an implementation detail, not as an alternate token source.
- Keep module-awareness as a hard invariant for callable identity/lookup (avoid cross-module name collisions).

### Milestone 4: (TBD / optional)

If we later need additional mechanisms for cycles or initialization ordering (e.g., callable indirection tables/cells), capture them as a dedicated milestone. For Option A as described in this document, this milestone is not required.

### Execution plan (how we implement and validate)

This section is intentionally **implementation-oriented**: it describes a practical way to execute the migration while keeping `master` runnable.

Guiding rules:

- (Historical) Land changes behind a compiler option until the final flip.
- Keep Phase 1 (declare) side-effect free with respect to bodies: no IR lowering and no IL body emission.
- Make each milestone shippable: after each PR, the compiler should still build and a representative test slice should pass.

Recommended PR sequence (historical; Milestones 1–2c are completed):

- Note: Two-phase compilation is always enabled; any references to flags/options below are historical context.

1. **Add the coordinator skeleton (no behavior change)**
   - Touchpoints: `MainGenerator` entry-point orchestration.
   - Deliverable: a new coordinator path that can be enabled by option, but initially delegates to the legacy ordering.
   - Done when:
     - Flag-off path is unchanged.
     - Flag-on path runs end-to-end for a trivial input (even if it still uses legacy internals).

2. **Introduce `CallableId` + `CallableRegistry` as adapters over existing state**
   - Touchpoints: caches/registries currently used for function + arrow callable loads, plus class registry.
   - Deliverable: one place that answers “given a callable identity, what token should IL use to reference it?”
   - Done when:
     - All existing call-sites that "load callable as value" can be routed through the registry.
     - Diagnostics can report a missing callable by `CallableId` (not just a string key).

3. **Make “declare” truly signature-only for one category at a time**
   - Suggested order (lowest risk → highest):
     - function declarations
     - class types + method signatures
     - arrow functions
   - Deliverable: `Declare*` paths create the metadata required to reference the callable (methoddef or memberref), without compiling bodies.
   - Done when:
     - Phase 1 completes without emitting any method bodies.
     - Existing compilation still succeeds because Phase 2 compiles bodies the old way.

4. **Add dependency collection (AST-first) + planner ([SCC][scc]/topo)** (completed)
   - Touchpoints: a new AST visitor that resolves identifiers via the symbol table and records `CallableId` edges.
   - Deliverable: a stable plan output (ordered callables and [SCC][scc] groups) that can be logged for debugging.
   - Done when:
     - The planner produces deterministic output for the same input.
     - The plan can be inspected in logs for failing tests.

4b. **(Optional, recommended) Add class-aware dependency edges (Milestone 2b1)** (completed)
    - Touchpoints: extend dependency collection with the class rules described in Milestone 2b1.
    - Deliverable: class-heavy programs produce a more accurate dependency plan without adding noisy edges from dynamic member calls.
    - Done when:
      - `new C()`, `this.m()`, and `super.m()`-style edges show up in the plan where applicable.
      - No broad over-approximation is introduced for `obj.m()` in the general case.

5. **Compile bodies in plan order (still using current body compilers)**
   - Touchpoints: coordinator Phase 2 loop; class/function/arrow “compile body” entry points.
   - Deliverable: Phase 2 compiles callables [SCC][scc]-by-[SCC][scc], then compiles main.
   - Done when:
     - With flag on, a representative set of tests compiles and runs.
     - Cycles do not deadlock compilation; [SCC][scc] policy is exercised (even if conservative).

6. **Enforce the invariant: IR emission never triggers compilation**
   - Touchpoints: IR pipeline entry points and any helper that currently "compiles on demand".
   - Deliverable: in strict mode, on-demand compilation paths throw a diagnostic that points back to the missing Phase 1 declaration.
   - Done when:
     - Strict IR tests stop failing due to ordering and instead either pass or produce a targeted diagnostic.

Validation strategy (keep feedback tight):

- Prefer execution tests that cover callables-as-values and cross-category calls:
  - Functions calling arrows, class methods calling functions, passing functions as callbacks.
- Expect generator snapshot churn once ordering and token strategies change; update snapshots only after execution behavior is stable.

---

## Diagnostics and invariants (implementation enforcement)

Add invariant checks (fail fast in strict mode):

- Phase 2 must not encounter an undeclared callable
- IR emission must not call “compile callable” APIs
- Every `CallableId` referenced by IR has a declared token

Add structured diagnostics:

- “Missing callable token for X”
- “[SCC][scc] cycle detected: [a,b,c] (policy=latebound-within-scc)”

These help avoid vague failures like “lowering failed” when the true issue is declaration ordering.

---

## Definitions

### Callable kinds
A “callable” is any construct that can be invoked or referenced as a function value:

- **Function declaration**: `function foo(...) { ... }`
- **Function expression**: `const f = function(...) { ... }`
- **Arrow function**: `const f = (...) => expr` or `(...) => { ... }`
- **Class constructor**: `class C { constructor(...) { ... } }`
- **Class method**: `class C { m(...) { ... } }` / static methods

---

## Naming and identity (very explicit)

JS2IL uses several different “names” for the same concept (JS name, scope name, registry key, .NET type name, IL method name). This section documents the **current conventions** that the two-phase plan must preserve.

### Terminology

- **JS name**: the user-facing identifier in the source code (e.g., `foo`, `C`, `m`).
- **Scope name**: the `Scope.Name` produced by `SymbolTableBuilder`.
- **Registry scope name**: the string key used by `VariableRegistry` for looking up scope fields; usually `"<module>/<scope>"`.
- **.NET type name / namespace**: the metadata name used when emitting `TypeDefinition`.
- **IL method name**: the metadata name used when emitting `MethodDefinition`.

### Quick reference tables

The tables below are meant to be the “implementation cheat sheet”. When building a two-phase coordinator (discovery → declare → compile), **all identities must remain consistent** with these conventions unless we explicitly decide to migrate them.

> Legend: `moduleName` is the root/global scope name (usually the input module name). `L<line>C<col>` is the source location from Acornima.

#### Table A — Names by construct

| Construct | Example JS | `Scope.Name` (SymbolTable) | Registry scope name (VariableRegistry key) | IL method name | .NET owner type / namespace | Primary cache/registry key |
|---|---|---|---|---|---|---|
| Module / global scope | *(file)* | `moduleName` | `moduleName` (global) | `Main` / entrypoint (not a scope name) | Global scope type is emitted via scope-as-class machinery | Scope name string (`moduleName`) |
| Function declaration | `function foo(a){}` | `foo` | `"{moduleName}/foo"` | `foo` | `Functions.{moduleName}` (static method) | IR: `BindingInfo` → `MethodDefinitionHandle` (CompiledMethodCache) / Legacy: string function name |
| Nested function declaration | `function outer(){ function inner(){} }` | `inner` | `"{moduleName}/inner"` (current) | `inner` | `Functions.{moduleName}` + nested type `Functions.{outer}_Nested` | IR: parent `funcScope.Bindings["inner"]` / Legacy: string name `"inner"` |
| Function expression (assigned) | `const f = function(a){}` | `FunctionExpression_f` | `"{moduleName}/FunctionExpression_f"` | `FunctionExpression_LxCy` | Generated as a nested function method via expression emitter | Legacy: method handle returned from generation; planned: `CallableId(FunctionExpr@loc or assignment)` |
| Function expression (not assigned) | `(function(a){})` | `FunctionExpression_LxCy` | `"{moduleName}/FunctionExpression_LxCy"` | `FunctionExpression_LxCy` | Generated as a nested function method via expression emitter | Same as above |
| Named function expression | `const f = function g(a){}` | `g` (internal) | `"{moduleName}/g"` | `FunctionExpression_LxCy` (still location-based) | Generated as a nested function method; recursion uses internal binding | Same as above |
| Arrow function (assigned) | `const f = (a)=>a+1` | `ArrowFunction_f` | `"{moduleName}/ArrowFunction_f"` | `ArrowFunction_LxCy` | Generated as a nested function method via arrow emitter | IR: `ArrowFunctionExpression` → token (legacy adapter); planned: `CallableId(Arrow@loc/assignment)` |
| Arrow function (not assigned) | `arr.map((x)=>x)` | `ArrowFunction_LxCy` | `"{moduleName}/ArrowFunction_LxCy"` | `ArrowFunction_LxCy` | Generated as a nested function method via arrow emitter | Same as above |
| Class declaration | `class C {}` | `C` (or `Class<N>`) | N/A (class itself not a variable scope key) | N/A | Namespace `Classes.{moduleName}`, type name `SanitizeForMetadata(C)` | `ClassRegistry` keyed by class name string |
| Class constructor | `constructor(a){}` | Method scope name is `constructor` (method pseudo-scope) | `"{moduleName}/constructor"` (current) | `.ctor` | Inside class type `Classes.{moduleName}.C` | `ClassRegistry.RegisterConstructor(className, ...)` |
| Class instance method | `m(a){}` | `m` (or `Method_LxCy`) | `"{moduleName}/{mname}"` (current) | `m` | Inside class type `Classes.{moduleName}.C` | `ClassRegistry.RegisterMethod(className, methodName, ...)` |
| Class static method | `static sm(a){}` | `sm` (or `Method_LxCy`) | `"{moduleName}/{mname}"` (current) | `sm` | Inside class type `Classes.{moduleName}.C` (static) | `ClassRegistry` (static methods may have separate invocation paths) |

Notes:

- “Registry scope name” is the key used to look up scope fields in `VariableRegistry` (captured variables, destructured parameters storage, etc.). It must remain stable for snapshots.
- Some current registry scope names are **not fully qualified** (e.g., `"{moduleName}/constructor"`, `"{moduleName}/{mname}"`, and nested function decls as `"{moduleName}/inner"`). The two-phase plan should use a stable `CallableId` internally, but preserve these strings for variable binding unless/until we deliberately migrate snapshots.

#### Table B — Cache/registry keying summary

| Component | Purpose | Key used today | What two-phase should use internally |
|---|---|---|---|
| `VariableRegistry` (scope field lookup) | Resolve captured variables/fields by scope | `string registryScopeName` (often `"{module}/{scope}"`) | Keep string key for storage lookup; also map from `CallableId` → registryScopeName |
| `CompiledMethodCache` | IR callable token lookup for function decls | `BindingInfo` → `MethodDefinitionHandle` | `CallableId(FunctionDecl)` → token/descriptor (adapter keeps BindingInfo path) |
| *(legacy arrow-function token cache)* | IR callable token lookup for arrows | `ArrowFunctionExpression` → token | `CallableId(Arrow)` → token/descriptor (adapter keeps AST-node path) |
| `FunctionRegistry` | Legacy emitter lookup by name (and arity info) | `string functionName` → method handle/arity | Prefer `CallableId` to avoid collisions; keep name-based lookup for legacy compatibility |
| `ClassRegistry` | Class type + ctor/method metadata for call sites | `string className` (+ member name) | `CallableId(ClassCtor/ClassMethod)` should carry class name + member name; registry remains class-centered |

### Module / global scope

- Global `Scope.Name` is the **module name**: `new Scope(module.Name, ScopeKind.Global, ...)`.
- `Variables.GetGlobalScopeName()` returns this module name.
- Many registry keys are built as: `"{moduleName}/{scopeName}"`.

### Function declarations (`function foo(){}`)

**Scope naming (SymbolTableBuilder)**

- If the AST has an identifier: `foo`
- If missing (rare/edge cases): `Closure<N>` (monotonic counter)

**.NET owner type naming (JavaScriptFunctionGenerator)**

- Module owner type: `Functions.<ModuleName>`
- Nested function owner type for nested function declarations: `Functions.<OuterFunctionName>_Nested` (added as a nested type under `Functions.<ModuleName>`)

**IL method naming**

- Method name is the JS function name, e.g. `foo`.

**Registry scope name (VariableRegistry key)**

- Top-level functions: `"{moduleName}/{functionName}"`.
- Nested function declarations (current behavior): `"{moduleName}/{nestedFunctionName}"`.
  - Note: this is intentionally documented as-is even though it can collide across different outers; the two-phase plan should move toward a stable `CallableId` to avoid collisions, while keeping this string for variable-field lookup where required by existing snapshots.

### Function expressions (`const f = function() {}` / IIFEs)

There are **two distinct names** involved:

**Scope naming (SymbolTableBuilder)**

- Named function expression: uses its internal name `fid.Name` (e.g., `function inner(){}` in an expression position)
- Otherwise, to align with legacy codegen naming:
  - If the expression is assigned: `FunctionExpression_<assignmentTarget>`
  - Else: `FunctionExpression_L<line>C<col>`

This is explicitly required by comments in the builder (“Naming must align with ILExpressionGenerator”).

**Registry scope name (ILExpressionGenerator / VariableRegistry key)**

- `baseScopeName` is chosen as:
  - named function expr: `<declaredName>`
  - else if assigned: `FunctionExpression_<assignmentTarget>`
  - else: `FunctionExpression_L<line>C<col>`
- `registryScopeName` is: `"{moduleName}/{baseScopeName}"`

**IL method name (ILExpressionGenerator)**

- Always uses the location form: `FunctionExpression_L<line>C<col>`
  - even when `baseScopeName` uses assignment target.
  - This is a metadata naming choice; the registry key is the important part for variable binding.

### Arrow functions (`(...) => ...`)

Same structure as function expressions: there is a scope/registry name and a method name.

**Scope naming (SymbolTableBuilder)**

- If assigned: `ArrowFunction_<assignmentTarget>`
- Else: `ArrowFunction_L<line>C<col>`

**Registry scope name (ILExpressionGenerator / VariableRegistry key)**

- `arrowBaseScopeName`:
  - if assigned: `ArrowFunction_<assignmentTarget>`
  - else: `ArrowFunction_L<line>C<col>`
- `registryScopeName`: `"{moduleName}/{arrowBaseScopeName}"`

**IL method name (ILExpressionGenerator)**

- Always uses the location form: `ArrowFunction_L<line>C<col>`

**Arrow precompile pass (JavaScriptFunctionGenerator)**

- Uses `scope.Name` from the symbol table as the `methodName` passed into compilation.
- Under current conventions, that is one of:
  - `ArrowFunction_<assignmentTarget>`
  - `ArrowFunction_L<line>C<col>`

The two-phase plan should keep the *identity* stable and treat the above as display/metadata names.

### Classes (`class C { ... }`)

**Scope naming (SymbolTableBuilder)**

- If class has an identifier: `C`
- Else: `Class<N>` (monotonic counter)

**.NET namespace and type name (SymbolTableBuilder → ClassesGenerator)**

- Namespace is authored on the scope: `Classes.<ModuleName>`
- Type name is authored on the scope: sanitized form of the class name (`SanitizeForMetadata(className)`)
- Emission uses:
  - `ns = classScope.DotNetNamespace ?? "Classes"`
  - `name = classScope.DotNetTypeName ?? classScope.Name`

**Private fields**

- JS private field `#x` is emitted with mangling (currently via `ManglePrivateFieldName`).

### Class constructors (`constructor(...) {}`)

**IL method name**

- Always `.ctor` (standard .NET instance constructor)

**Registry scope name (Variables key)**

- `"{moduleName}/constructor"`
  - This is intentionally module-qualified and does not include class name today.
  - The two-phase plan should not change this without considering snapshots and variable registry usage.

### Class methods (`m(...) {}`)

**Scope naming (SymbolTableBuilder)**

- If method key is an identifier: method name `m`
- Else: `Method_L<line>C<col>`

**IL method name (ClassesGenerator)**

- Uses `mname` directly as the emitted method name.

**Registry scope name (Variables key)**

- `"{moduleName}/{mname}"`
  - As with constructors, current registry scope names for methods do not include class name.

---

### Callable metadata
For dependency-safe compilation we need metadata that can be referenced before the body is compiled:

- A **stable ID** for the callable
- Owner type / scope identity
- JS parameter count (or min/max)
- Whether the callable requires a scopes chain (`object[] scopes`) or can be compiled as a simpler static method
- The eventual method signature shape used by runtime helpers / delegate creation

### Dependency
A dependency is any situation where compiling callable **A** needs to reference callable **B**:

- `A` references `B` as a **value** (e.g., `arr.map(B)` or `const x = () => ...`)
- `A` calls `B` (direct call or through runtime helpers)
- `A` constructs a class (`new C()`) that needs constructor metadata (identifier-resolvable in Milestone 2b)
- `A` invokes a class member (`obj.m()`) where the target is statically resolvable (still treated as dynamic in the general case; only `this.m()` / `super.m()` are modeled as dependencies today)

We’ll treat dependencies as edges in a directed graph.

---

## High-level pipeline overview

This is a **summary view** of the end-to-end flow. Detailed mechanics are defined earlier in this document:

- Phase 0 (unchanged): Parse → Validate → Symbol Table
- Phase 1: Discover + Declare (no bodies)
  - See: “Phase 1: Declaration mechanics (how we declare without bodies)” and the registry/signature sections
- Phase 2: Compile bodies (dependency-safe)
  - See: “Phase 2: Body compilation mechanics” and the [SCC][scc] policy sections
- Ordering and [SCC][scc] handling:
  - See: “Dependency discovery”, “Compilation planning ([SCC][scc]/topo)”, and “[SCC][scc] escape hatch (detailed)” 

---

## IR-specific requirements

### Current architectural constraint
The IR pipeline must not compile callables on-demand. It should only:

- Lower expressions/statements to HIR/LIR
- Emit IL that references callables via caches/registries

Callable reference strategy:

- The callable reference strategy is specified in “Phase 1: Declaration mechanics”.
- In strict mode, the IR pipeline should treat a missing callable reference as a Phase 1 bug (not as a cue to compile-on-demand).

---

## Performance considerations

Two-phase compilation is not just correctness; it enables performance:

- Most calls can become early-bound (direct call or cached delegate) once dependencies are known.
- Late-binding can be restricted to:
  - cycles ([SCC][scc]s)
  - unsupported IR constructs that intentionally fall back

This aligns with the goal: **avoid compile-on-demand** while keeping the fast path fast.

---

## Open questions

- Do we want to eliminate string-keyed function registries in favor of `Symbol`/`BindingInfo` keys to avoid collisions?
- What is the minimal stable key for class methods (type handle + name + arity vs including scope ID)?
- Should arrow functions with captures be declared but compiled via legacy only, or should IR learn closure shapes incrementally?

---

## Appendix: Expected invariants

After Phase 1:

- Every callable used as a value has a resolvable callable reference.
- IR lowering never triggers compilation.

After Phase 2:

- All compiled bodies are present in the module.
- Strict-IR tests can assert no fallback due to missing callables.

[scc]: #scc-strongly-connected-component
