# Lowering Pipeline Migration Punch List (AST → HIR → LIR → IL)

## Purpose
This document is a punch list of remaining work to migrate functionality from the legacy **AST → IL** emitters to the new lowering pipeline embodied by `JsMethodCompiler` (**AST → HIR → LIR → IL**).

The end-state goal is to remove the legacy method-body emitters (e.g. `BinaryOperators`, `ILExpressionGenerator`, `ILMethodGenerator`) and any supporting infrastructure that becomes redundant.

> Status (Jan 2026): The legacy AST→IL generator stack (`ILMethodGenerator` / `ILExpressionGenerator` / `BinaryOperators` and helpers) has been deleted. The compiler now relies on IR compilation and fails fast when unsupported constructs are encountered (no legacy fallback).

## Scope / Definitions
- **New pipeline**: `JsMethodCompiler` + `HIRBuilder` + `HIRToLIRLowerer` + `LIRToILCompiler`.
- **Legacy pipeline** (historical): the deleted AST→IL emitters under `Js2IL/Services/ILGenerators/*` (notably `ILExpressionGenerator`, `ILMethodGenerator`, `BinaryOperators`) plus legacy body compilers that previously handled unsupported IR cases.
- **“Migrated”** means the construct is parsed to HIR, lowered to LIR, and emitted to IL in the new pipeline (not merely “exists as an instruction type”).

## Current State (Audit Summary)
### New pipeline entrypoints that exist today
- `JsMethodCompiler.TryCompileMainMethod` (module wrapper entry)
- `JsMethodCompiler.TryCompileMethod` (function declarations + class methods via `MethodDefinition`)
- `JsMethodCompiler.TryCompileArrowFunction` (block-body arrows only)
- `JsMethodCompiler.TryCompileCallableBody` (two-phase body-only compilation)
- Partial `JsMethodCompiler.TryCompileClassConstructor` exists but is intentionally guarded to fail fast (see “Constructors” below).

### Hard fail gates (new pipeline declines and compilation fails)
The new pipeline currently declines (returns `false` / default) for some constructs. Notable current gaps:

- **Rest parameters**
  - Top-level rest parameters (`function f(...args) {}` / `(...args) => {}`) are not supported by the IR pipeline.
  - Rest *destructuring* (`const {a, ...rest} = obj`, `const [a, ...rest] = arr`) is supported.
- **Intrinsic constructor-like calls (no `new`)**
  - `String(x)`, `Number(x)`, `Boolean(x)` are supported.
  - Other callable-only intrinsics (`Date(...)`, `RegExp(...)`, `Error(...)`, `Array(...)`, `Object(...)`, `Symbol(...)`, `BigInt(...)`) are not yet supported.
- **Object literal feature gaps**
  - Spread properties (`{...x}`), computed keys (`{[expr]: v}`), and shorthand/method properties are not yet supported in the IR pipeline.
- **Class constructors**
  - `TryCompileClassConstructor` exists but is still intentionally limited (base `.ctor` / `super(...)` semantics and field initialization are not fully implemented).

### What is already migrated (supported end-to-end by new pipeline)
This is the set of AST constructs that the new pipeline can currently parse and lower (subject to the closure restrictions above):

**Statements (HIRBuilder)**
- Variable declarations with *identifier* declarators only (`var/let/const x = ...`), including multiple declarators in a single `VariableDeclaration`.
- Expression statements.
- `if/else`.
- Block statements (including child block scopes for `{}` and `for (let ...)` scopes).
- `while`.
- `do { } while (...)`.
- `return` (with or without value).
- `for (init; test; update) { ... }` where:
  - `init` is a `VariableDeclaration` (identifier declarators only) or an expression,
  - `test` is optional expression,
  - `update` is optional expression.
- `for..of`.
- `for..in`.
- `break` / `continue` (including labeled `break`/`continue`).
- `switch`.
- `try/catch/finally`.
- `throw`.

**Expressions (HIRBuilder)**
- Binary expressions (`BinaryExpression`) (operator support depends on lowering + LIR/IL support).
- Call expressions (`CallExpression`) with expression callee.
  - Note: IR lowering supports direct calls to function *bindings* like `foo(...)`, plus indirect calls through function values (e.g., `const f = makeFn(); f()` and IIFEs) via runtime dispatch.
  - Selected intrinsic/member-call shapes like `console.log(...)` are handled by specialized lowering.
- Update expressions (`++x`, `x++`, `--x`, `x--`).
- Unary expressions (e.g. `!`, `-`, `+`, `~`, `typeof` depending on lowering).
- Identifier references (including known global constants `undefined`, `NaN`, `Infinity` when not shadowed).
- Assignment expressions where the LHS is an identifier only.
- Member expressions:
  - computed index access `obj[expr]`
  - non-computed property access `obj.prop` (identifier properties only)
- Literals: number, string, boolean, null.
- Array literals, including `...spread` elements.
- Object literals with simple properties only:
  - keys: identifier / string literal / numeric literal
  - values: expression
  - (no spread properties, computed keys, methods/getters/setters, or shorthand)
- `this` (`ThisExpression`) in class methods/constructors

**Lowering / IL emission highlights (LIR)**
- Numeric arithmetic + bitwise + shifts + comparisons have LIR instruction types and IL emission cases.
- Dynamic equality/inequality and strict equality/inequality are represented in LIR and emitted via runtime helpers.
- Array/object literal construction and array spread lowering have LIR instruction types and IL emission cases.
- Default parameter initialization exists in lowering (AssignmentPattern where LHS is Identifier).

## Legacy Pipeline Coverage (what old AST → IL can do today that HIR can’t)
Based on the (now-deleted) legacy emitters:

**Statements supported in legacy but not in HIRBuilder**
- (none currently identified; statement parity items are tracked in PL2.*)

**Expressions supported in legacy but not in HIRBuilder**
- `new` expressions
- template literals
- conditional/ternary (`test ? a : b`)
- arrow functions as expressions (including non-block bodies)
- function expressions

**Parameters / patterns supported in legacy but not in HIRBuilder**
- Parameter destructuring (`ObjectPattern`, `ArrayPattern`), including defaults in patterns.

## Punch List (Remaining Migration Work)

### 1) Closures & captured variables (Phase 3: scopes materialization)
**Goal**: remove the fallback restrictions around `scope.ReferencesParentScopeVariables` and `IsCaptured`.

- [x] PL1.1 Enable new pipeline for methods where `scope.ReferencesParentScopeVariables` is true.
- [x] PL1.2 Enable new pipeline for scopes that contain captured bindings (`IsCaptured == true`).
- [x] PL1.3 Audit + complete captured-variable *writes* semantics (reads/writes must agree with legacy closure behavior).
- [x] PL1.4 Ensure leaf scope instance creation is correct in all cases:
  - [x] PL1.4a create leaf scope instance exactly when required (and only once)
  - [x] PL1.4b correct leaf local lifetime across control-flow and loops
- [x] PL1.5 Ensure call sites always build the correct scopes array for callee requirements:
  - [x] PL1.5a direct calls (`f(...)`)
  - [x] PL1.5b calls via variables / re-assignment (e.g., `const f = makeFn(); f()`)
  - [x] PL1.5c nested functions and function expressions used as values (as call targets)
  - [x] PL1.5d class method calls that access parent scopes
- [x] PL1.6 Confirm the scopes source selection is correct for:
  - [x] PL1.6a static methods (scopes passed as arg)
  - [x] PL1.6b instance methods (scopes loaded from `this._scopes`)
- [x] PL1.7 Expand execution tests for closures (both reads and writes across nesting levels).

### 2) Expand HIR statement support to match legacy
- [x] PL2.1 `while` statements
- [x] PL2.2 `do/while` statements
- [x] PL2.3 `for..of` statements
- [x] PL2.4 `for..in` statements (if legacy supports it; confirm)
- [x] PL2.5 `break` / `continue` (requires loop label tracking)
- [x] PL2.6 `switch` statements (if legacy supports it; confirm)
- [x] PL2.7 `try/catch/finally`
  - [x] PL2.7a Add HIR nodes for try/catch/finally (including catch parameter binding)
  - [x] PL2.7b Add LIR representation for exception regions (try region + handler regions)
  - [x] PL2.7c Emit IL exception regions in `LIRToILCompiler` (ExceptionRegion / EH tables)
  - [x] PL2.7d Ensure `finally` executes on all exits (normal fallthrough, `return`, `throw`, `break`/`continue` once supported)
  - [x] PL2.7e Validate catch variable scoping semantics match legacy (block-scoped catch param)
- [x] PL2.8 `throw`

### 3) Expand HIR expression support to match legacy
- [x] PL3.1 `ConditionalExpression` (ternary)
- [x] PL3.2 `LogicalExpression` (`&&`, `||`) with correct short-circuit semantics
- [x] PL3.3 `NewExpression`
  - [x] PL3.3a built-in Error types
  - [x] PL3.3b user-defined classes
  - [x] PL3.3c argument count checking (match legacy)
  - [x] PL3.3d built-in Array constructor (`new Array()` / `new Array(n)` / `new Array(a, b, ...)`) semantics
  - [x] PL3.3e built-in String constructor (`new String()` / `new String(x)`) as syntactic sugar for native strings
  - [x] PL3.3f built-in Boolean/Number constructors (`new Boolean(x)`, `new Number(x)`) wrapper/sugar semantics
  - [x] PL3.3g built-in constructors for remaining existing runtime intrinsics (e.g., Date, RegExp, Set, Promise, Int32Array)
- [x] PL3.4 `TemplateLiteral` (including interpolation)
- [x] PL3.5 `ThisExpression` (supported for class methods/constructors)
- [x] PL3.6 `FunctionExpression` as an expression (closure creation)
- [x] PL3.7 `ArrowFunctionExpression` as an expression (closure creation)
  - [x] PL3.7a concise-body arrows must wrap implicit return

### 8) Intrinsic constructor-like calls (no `new`)
These are `CallExpression` forms (e.g., `Date(x)`, `Boolean()`) and are distinct from `NewExpression`.

- [x] PL8.1 Primitive conversion callables: `String(x)`, `Number(x)`, `Boolean(x)`

### 4) Variable declarators & assignment targets
The new HIR currently only supports identifier declarators and identifier assignment LHS.

- [x] PL4.1 Variable declarator destructuring:
  - [x] PL4.1a `const {a, b} = obj`
  - [x] PL4.1b `const [a, b] = arr`
  - [x] PL4.1c nested patterns + defaults
  - [x] PL4.1d rest elements (`...rest`)
- [x] PL4.2 Assignment targets beyond identifiers:
  - [x] PL4.2a `obj.prop = value`
  - [x] PL4.2b `obj[index] = value`
  - [x] PL4.2c destructuring assignment (`({a} = obj)`)

### 5) Classes: constructors + field initialization
IR supports many constructor bodies (including defaults/destructuring parameters), injects a `System.Object::.ctor()` call, and supports public/private/static field initializers; derived `super(...)` behavior and some return semantics still need work.

- [x] PL5.1a `System.Object::.ctor` for classes without explicit `extends`
- [x] PL5.2 Support constructor parameters (including defaults / destructuring / rest as applicable).
- [x] PL5.3 Support field initialization (public fields + private fields + static fields if supported by legacy).

### 6) Two-phase compilation parity
The repo already has a two-phase coordinator and a `TryCompileCallableBody` API in the new pipeline.

- [x] PL6.1 Ensure all callable shapes used by two-phase mode can be compiled via IR (functions, arrows, class methods, constructors).
- [x] PL6.2 Ensure dependency discovery and required scope-chain layout are consistent with IR call-site scopes materialization.

### 7) Deletion targets (what can be removed once punch list is complete)
These are candidates to delete as IR reaches feature parity and remaining legacy scaffolding becomes redundant.

> Note (Jan 2026): several of the high-confidence legacy deletion targets have already been removed to enforce IR-only compilation.

**High confidence once IR is complete**
- [x] PL7.1 `Js2IL/Services/ILGenerators/BinaryOperators.cs` (deleted)
- [x] PL7.2 `Js2IL/Services/ILGenerators/ILExpressionGenerator.cs` (deleted)
- [x] PL7.3 `Js2IL/Services/ILGenerators/ILMethodGenerator.cs` (deleted)
- [x] PL7.4 `Js2IL/Services/TwoPhaseCompilation/LegacyFunctionBodyCompiler.cs` (legacy compilation removed; stub throws)
- [x] PL7.5 Remove legacy class body compiler (IR-only class bodies)

**Likely, but requires follow-up audit**


Notes on `Variables`/`Variable` deletion:
- Today they are heavily referenced by the *legacy* body compilers and generators.
- The new pipeline primarily relies on `ScopeMetadataRegistry` + `EnvironmentLayoutBuilder` + LIR temp/local allocation, not `Variables`.
- After legacy removal, reassess whether `VariableRegistry` (and the stable type analysis it enables) should be:
  - [ ] removed,
  - [ ] replaced by IR-native type facts, or
  - [ ] retained for optimizations only.

## Suggested Migration Order (minimize risk)
1. Closures + scopes materialization (unblocks most real-world code).
2. Statement parity (loops + try/throw + break/continue).
3. Expression parity (logical/ternary/new/template/function expressions).
4. Destructuring and richer assignments (variables + assignment targets).
5. Constructors + class field initialization.
6. Continue expanding IR coverage; legacy fallback has been removed.

## Validation Checklist (per feature)
- Add/extend execution tests under `Js2IL.Tests/*/ExecutionTests`.
- Update generator snapshots only after execution behavior matches legacy (if snapshots are used for the area).
- Confirm `IRPipelineMetrics` has no new failure hotspots for common test suites.

**Exception handling validation (try/catch/finally)**
- `try/finally` runs finally on `return` and `throw`.
- `try/catch` binds catch parameter correctly and does not leak it outside the catch block.
- Nested try blocks (try inside catch/finally) behave as expected.
