# JS2IL Two-Phase Compilation Pipeline (Design Plan)

This document describes a proposed **two-phase compilation pipeline** for JS2IL that is **dependency-safe** across:

- Classes (constructors, methods)
- Function declarations/expressions
- Arrow functions

The goal is to make IR compilation reliable without relying on “compile-on-demand” during AST→HIR→LIR→IL.

> Status: planning / design. This document describes a migration path and does not imply the implementation is complete.

This doc is written **ideal-first**:

- Treat the **Phase 1 / Phase 2** mechanics and the invariants as the target design.
- Any discussion of “today”, “migration”, or “legacy” is non-normative context for rollout.

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
- `MethodSignatureBlobHandle SignatureBlob` (optional cache for fast `MemberReference` creation)

#### `CallableRegistry`
Single source of truth mapping `CallableId` → declaration info.

Required operations:

- `Declare(CallableId, CallableSignature)`: ensures method/type metadata exists
- `TryGetMethodDefHandle(CallableId, out MethodDefinitionHandle)` (Option A)
- `GetOrCreateMemberRef(CallableId) : MemberReferenceHandle` (Option B)
- `MarkBodyCompiled(CallableId)` for diagnostics/invariants

This is the abstraction that replaces “function cache vs arrow cache vs class registry in isolation” during planning.

#### `CompilationPlanner`
Responsible for:

1. Discovering all callables
2. Building a dependency graph
3. Computing SCC + topo order
4. Producing a compile plan: ordered list of callables (or SCC groups)

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
5. `CompileMain(ast)` (after bodies, or last SCC)

Concrete change: `MainGenerator.DeclareClassesAndFunctions` becomes a thin wrapper that calls a new coordinator (e.g., `TwoPhaseCompilationCoordinator`).

### Classes

Split class compilation into two operations:

- `DeclareClassTypes(SymbolTable)`
  - creates class type definitions and registers them in `ClassRegistry`
  - registers member *signatures* (ctor/method names + arity)
- `CompileClassBodies(plan)`
  - compiles constructors/method bodies according to the plan

This avoids today’s ordering hazard where class methods are attempted in IR before function/arrow caches exist.

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
- `A` references class member metadata (constructor or method)
  - `new C()` depends on `C` constructor signature availability
  - `obj.m()` depends on `C.m` metadata if statically resolvable

Note: calls through unknown values (`x()`) do not create a compile-time dependency.

### How to resolve identifiers to `CallableId`

For each callable body, maintain a resolution context:

- `Scope` from the symbol table
- ability to resolve `Identifier` → `Symbol` / `BindingInfo`

Then map:

- function decl symbol → `CallableId` (kind `FunctionDeclaration`)
- arrow AST node → `CallableId` (kind `Arrow` + stable location)
- class method → `CallableId` (kind `ClassMethod` with `className + methodName` and scope id)

---

## Compilation planning (SCC/topo)

Before we talk about the planner output, it helps to define **SCC**.

An **SCC (Strongly Connected Component)** is a set of nodes in a directed graph where every node can reach every other node (possibly through intermediate nodes). In this design, the graph nodes are **callables** (functions, arrows, class methods/ctors), and edges are “A depends on B” references.

Why SCCs matter here:

- SCCs represent **cycles** in the callable dependency graph (mutual recursion or cross-kind recursion).
- A cycle means there is **no valid linear compile order** that satisfies “compile all of A’s dependencies before A”.

The practical challenges SCCs introduce:

- **Ordering:** we can topologically order SCCs, but not the callables *inside* an SCC.
- **Codegen policy:** within an SCC we may need a conservative rule (e.g., late-bind SCC-internal invocation edges) to avoid relying on assumptions that only hold in acyclic graphs.
- **Debuggability:** without SCC awareness, failures look like “missing callable token” or “ordering issue”; with SCC awareness, we can diagnose “cycle detected” and apply a deliberate strategy.

### Output plan representation

The planner should output a list of *stages*:

- each stage is either:
  - a single callable (acyclic)
  - or an SCC group of multiple callables

Example:

1. SCC #1: `Arrow(A1)`
2. SCC #2: `Function(foo)`
3. SCC #3: `{ Function(a), Function(b) }` (mutual recursion)
4. SCC #4: `ClassMethod(C.m)`

### Policy inside SCCs

When compiling an SCC group, we should not require early-bound calls among members.

Two acceptable policies:

- Emit early-bound calls/delegates anyway (works because declarations exist), or
- Emit late-bound calls only for edges within the SCC (more conservative)

This is a tuning knob for correctness vs. complexity.

---

## SCC escape hatch (detailed)

This section explains the “escape hatch” strategy for **strongly connected components (SCCs)** in the callable dependency graph.

### What an SCC means in this compiler

An SCC is a set of callables where each callable is reachable from every other callable in the set (directly or indirectly).

In JS2IL terms, SCCs arise from:

- Mutual recursion between functions
- A class method referencing a function value that (transitively) references the class
- Arrows captured into variables that then feed back into other callables

Key point: SCCs are not “bad”; they are a signal that **static, one-way dependency ordering is impossible**. We need a policy for codegen that remains correct.

### Why SCCs are tricky even with Phase 1 declarations

If Phase 1 produces a resolvable token for every callable, we *can* still emit early-bound calls/delegates in a cycle.

However, SCCs are where we often hit cases like:

- “I need a callable value to exist and be callable before I’ve finished compiling all participants.”
- “I want to inline/optimize assuming a target is known, but within SCC that assumption may be fragile.”

So the escape hatch is a conservative policy to keep correctness and reduce implementation complexity.

### The escape hatch in one sentence

Within an SCC, allow codegen to **emit late-bound invocation for SCC-internal edges**, while keeping SCC-external edges early-bound.

This keeps most of the program early-bound and only degrades the cyclic portion.

### Planner output needed to support SCC policy

The compilation planner should produce two artifacts:

1. **Stage order**: SCCs in topological order
2. **SCC membership map**: `CallableId -> SccId`

Additionally (recommended), the planner can produce an **edge classification function**:

- `IsSccInternalEdge(caller, callee) := SccId(caller) == SccId(callee)`

Codegen can then choose early-bound vs late-bound per call site.

### What we late-bind (and what we don’t)

We should scope the escape hatch to the smallest, safest subset:

- Late-bind only **invocation edges** that target a known callable inside the same SCC.
- Do **not** late-bind global intrinsic calls (Math/Array/etc.) or obviously non-cyclic operations.

Common cases:

- `a()` where `a` resolves to a specific declared function in the same SCC → late-bound invoke
- `arr.map(a)` where `a` is a function value from same SCC → still can be early-bound delegate load (see below)

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

Represent each callable in an SCC by a mutable cell that can be assigned once compilation completes.

- Phase 1 declares a static field (or scope field) that will hold the callable delegate
- Phase 2 compiles each method and sets its cell
- SCC-internal call sites load the cell and invoke

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

Even in SCCs, it can be reasonable to keep **delegate loads early-bound**, because Phase 1 guarantees the token/signature exists.

The escape hatch can be limited to **invocation**. That provides a good balance:

- `arr.map(toStr)` still gets a fast delegate
- but `toStr(x)` might use late-bound invoke only when `toStr` is SCC-internal and the policy says so

### How codegen decides (concrete decision table)

Given a call site inside callable `Caller`:

- If callee is an intrinsic / runtime helper → early-bound as today
- Else if callee is not statically resolvable (e.g., `x()` where `x` is unknown) → late-bound (already required)
- Else if callee resolves to callable `Callee`:
  - If `SccId(Caller) != SccId(Callee)` → early-bound
  - If `SccId(Caller) == SccId(Callee)` → late-bound invoke (escape hatch)

### Implementation hook points

The minimal places we need to thread SCC knowledge:

1. **Planner** produces `SccId` mapping.
2. **Lowering / LIR generation** needs access to `SccId` mapping when choosing instruction forms.
3. **IL emitter** needs a late-bound invoke instruction.

Concretely, we can implement as:

- New LIR instruction: `LIRInvokeLateBound(TempVariable CalleeValue, IReadOnlyList<TempVariable> Args, TempVariable Result)`
- Or: `LIRCallFunction` gains a flag `ForceLateBound` computed by the planner.

If we want to keep LIR stable, the flag approach is simplest.

### Diagnostics

To make this debuggable (and avoid silent performance regressions), emit diagnostics per SCC:

- SCC id and members
- Which edges are late-bound (caller → callee)

In strict-IR tests, it can be useful to assert:

- late-binding only appears when `SccSize > 1` OR when target is not statically resolvable

### Safety constraints

The escape hatch must preserve the architectural rule:

- Late-binding must not perform compilation-on-demand.

It may consult registries/caches for tokens or cells, but compilation must already be complete (Phase 2) or in-progress within the SCC stage.

---

## Phase 1: Declaration mechanics (how we “declare” without bodies)

### Two options for callable references

#### Option A (incremental): pre-create `MethodDefinitionHandle` for every callable

Mechanics:

1. Allocate the method definition up-front with a fixed signature.
2. Store `MethodDefinitionHandle` in `CallableRegistry`.
3. Later in Phase 2, emit body and attach via `AddMethodBody`.

Constraints:

- Requires knowing exact signatures early.
- Requires ensuring each method is associated with the correct owning type.

#### Option B (robust): caches store a “memberref descriptor” and emit `MemberReferenceHandle`

Mechanics:

1. Phase 1 records `(owner type ref/def, method name, signature blob)`.
2. When IL emission needs a token, create/lookup a `MemberReferenceHandle`.

Benefits:

- Reduces ordering hazards even further.
- Avoids coupling IR emission to the moment methoddefs are created.

Tradeoff:

- Requires standardizing signature blob construction for all callable shapes.

Recommendation: implement Option A first if easiest; keep Option B as the end state.

---

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

Main should be compiled after all callables (or after the SCC that contains Main’s dependencies).

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

Under Option B, `<method token>` is a `MemberReferenceHandle`.

In both cases the delegate signature must match `JsParamCount`.

### Calling methods

Direct calls can remain as:

- `call` (static)
- `callvirt` (instance)

But when a direct target cannot be resolved (unknown receiver type, SCC edge policy), emit dynamic runtime dispatch as today.

---

## Migration plan (more concrete)

This is intended to be implemented incrementally and keep the system runnable.

### Milestone 1: Coordinator + Phase 1 declarations

- Add `CallableDiscovery` and `CallableRegistry`
- Refactor class/function/arrow “declare” entry points into signature-only
- Keep Phase 2 using existing compile routines

Legacy AST→IL impact:

- **Minimal / mostly orchestration.** Legacy AST→IL remains the body compiler for Phase 2 at this point.
- Split the legacy entry points so they can be invoked as:
  - **Phase 1**: “declare signatures only” (types + method defs / descriptors)
  - **Phase 2**: “compile bodies” (existing AST→IL body emission)
- Avoid any AST→IL code path that *implicitly* compiles other callables during emission (e.g., depth-first nested function compilation) by moving that work into Phase 1 discovery/declare.

### Milestone 2: Dependency graph + ordering

- Add AST visitor to build dependencies
- Add SCC/topo planner
- Compile bodies in plan order

Legacy AST→IL impact:

- **No semantic changes to AST→IL emission.** The change is *when* it runs:
  - AST→IL body compilation is invoked by the coordinator in planned order.
- Legacy “compile main” should move to the end (after callable bodies) to match the planned ordering.

### Milestone 3: Replace ad-hoc caches with unified registry

- Migrate `CompiledMethodCache` and `CompiledArrowFunctionCache` behind `CallableRegistry`
- Keep old caches as adapters temporarily to minimize churn

Legacy AST→IL impact:

- Update the legacy AST→IL emitters that “load callable as value” (delegate creation) and any direct-call sites that consult ad-hoc caches to go through the unified `CallableRegistry`.
- Remove any remaining coupling where the legacy pipeline assumes “callee body must have been compiled already” to obtain a token.

### Milestone 4: Switch callable loads to MemberReference (Option B)

- Standardize signature generation for function/arrow/class methods
- Update LIR→IL emitter to use `MemberReferenceHandle` tokens for callable loads

Legacy AST→IL impact:

- If the legacy AST→IL pipeline also emits “function as value” delegates using `ldftn`, it should be updated to accept **either** a methoddef handle (Option A) **or** a memberref token (Option B) from the registry.
- After this milestone, legacy AST→IL should no longer need bodies to be compiled to load a callable as a value; it should be able to emit delegate creation from the declared signature alone.

### Execution plan (how we implement and validate)

This section is intentionally **implementation-oriented**: it describes a practical way to execute the migration while keeping `master` runnable.

Guiding rules:

- Land changes behind a compiler option (e.g., `CompilerOptions.TwoPhaseCompilation`) until the final flip.
- Keep Phase 1 (declare) side-effect free with respect to bodies: no IR lowering and no IL body emission.
- Make each milestone shippable: after each PR, the compiler should still build and a representative test slice should pass.

Recommended PR sequence (small, mergeable increments):

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

4. **Add dependency collection (AST-first) + planner (SCC/topo)**
   - Touchpoints: a new AST visitor that resolves identifiers via the symbol table and records `CallableId` edges.
   - Deliverable: a stable plan output (ordered callables and SCC groups) that can be logged for debugging.
   - Done when:
     - The planner produces deterministic output for the same input.
     - The plan can be inspected in logs for failing tests.

5. **Compile bodies in plan order (still using current body compilers)**
   - Touchpoints: coordinator Phase 2 loop; class/function/arrow “compile body” entry points.
   - Deliverable: Phase 2 compiles callables SCC-by-SCC, then compiles main.
   - Done when:
     - With flag on, a representative set of tests compiles and runs.
     - Cycles do not deadlock compilation; SCC policy is exercised (even if conservative).

6. **Enforce the invariant: IR emission never triggers compilation**
   - Touchpoints: IR pipeline entry points and any helper that currently "compiles on demand".
   - Deliverable: in strict mode, on-demand compilation paths throw a diagnostic that points back to the missing Phase 1 declaration.
   - Done when:
     - Strict IR tests stop failing due to ordering and instead either pass or produce a targeted diagnostic.

7. **Migrate callable loads to Option B (`MemberReference`)**
   - Touchpoints: signature construction and LIR→IL callable-load emission.
   - Deliverable: callable loads as values use `ldftn` with `MemberReferenceHandle`, decoupling from when methoddefs are created.
   - Done when:
     - Delegate construction works for functions, arrows, and class methods.
     - Phase 1/2 separation is robust against reordering.

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
- “SCC cycle detected: [a,b,c] (policy=latebound-within-scc)”

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
| Arrow function (assigned) | `const f = (a)=>a+1` | `ArrowFunction_f` | `"{moduleName}/ArrowFunction_f"` | `ArrowFunction_LxCy` | Generated as a nested function method via arrow emitter | IR: `ArrowFunctionExpression` → `MethodDefinitionHandle` (CompiledArrowFunctionCache); planned: `CallableId(Arrow@loc/assignment)` |
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
| `CompiledArrowFunctionCache` | IR callable token lookup for arrows | `ArrowFunctionExpression` → `MethodDefinitionHandle` | `CallableId(Arrow)` → token/descriptor (adapter keeps AST-node path) |
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
- `A` constructs or invokes a **class** member (`new C()`, `obj.m()`) that needs member metadata

We’ll treat dependencies as edges in a directed graph.

---

## High-level pipeline overview

This is a **summary view** of the end-to-end flow. Detailed mechanics are defined earlier in this document:

- Phase 0 (unchanged): Parse → Validate → Symbol Table
- Phase 1: Discover + Declare (no bodies)
  - See: “Phase 1: Declaration mechanics (how we declare without bodies)” and the registry/signature sections
- Phase 2: Compile bodies (dependency-safe)
  - See: “Phase 2: Body compilation mechanics” and the SCC policy sections
- Ordering and SCC handling:
  - See: “Dependency discovery”, “Compilation planning (SCC/topo)”, and “SCC escape hatch (detailed)” 

---

## IR-specific requirements

### Current architectural constraint
The IR pipeline must not compile callables on-demand. It should only:

- Lower expressions/statements to HIR/LIR
- Emit IL that references callables via caches/registries

Callable reference strategy:

- Options and tradeoffs (Option A methoddef vs Option B memberref) are specified in “Phase 1: Declaration mechanics”.
- In strict mode, the IR pipeline should treat a missing callable reference as a Phase 1 bug (not as a cue to compile-on-demand).

---

## Performance considerations

Two-phase compilation is not just correctness; it enables performance:

- Most calls can become early-bound (direct call or cached delegate) once dependencies are known.
- Late-binding can be restricted to:
  - cycles (SCCs)
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
