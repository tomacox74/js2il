# Lowering Pipeline Migration Punch List (AST → HIR → LIR → IL)

## Purpose
This document is a punch list of remaining work to migrate functionality from the legacy **AST → IL** emitters to the new lowering pipeline embodied by `JsMethodCompiler` (**AST → HIR → LIR → IL**).

The end-state goal is to remove the legacy method-body emitters (e.g. `BinaryOperators`, `ILExpressionGenerator`, `ILMethodGenerator`) and any supporting infrastructure that becomes redundant.

## Scope / Definitions
- **New pipeline**: `JsMethodCompiler` + `HIRBuilder` + `HIRToLIRLowerer` + `LIRToILCompiler`.
- **Legacy pipeline**: `Js2IL/Services/ILGenerators/*` (notably `ILExpressionGenerator`, `ILMethodGenerator`, `BinaryOperators`) plus two-phase legacy body compilers.
- **“Migrated”** means the construct is parsed to HIR, lowered to LIR, and emitted to IL in the new pipeline (not merely “exists as an instruction type”).

## Current State (Audit Summary)
### New pipeline entrypoints that exist today
- `JsMethodCompiler.TryCompileMainMethod` (module wrapper entry)
- `JsMethodCompiler.TryCompileMethod` (function declarations + class methods via `MethodDefinition`)
- `JsMethodCompiler.TryCompileArrowFunction` (block-body arrows only)
- `JsMethodCompiler.TryCompileCallableBody` (two-phase body-only compilation)
- Partial `JsMethodCompiler.TryCompileClassConstructor` exists but is intentionally guarded to fail fast (see “Constructors” below).

### Hard fallback gates (new pipeline declines and legacy takes over)
The new pipeline currently falls back (returns `false` / default) for:
- **Closures / captured variables**
  - `HIRBuilder.TryParseMethod(...)` refuses when:
    - current scope has captured bindings (`scope.Bindings.Values.Any(b => b.IsCaptured)`), and/or
    - method scope references parent variables (`scope.ReferencesParentScopeVariables`).
- **Arrow functions**
  - concise-body arrows (`() => expr`) fall back (HIR doesn’t wrap implicit return yet).
  - parameter patterns beyond simple identifiers/defaults fall back.
- **Function expression parameters**
  - parameter destructuring/rest patterns fall back.
- **Class constructors**
  - `TryCompileClassConstructor` explicitly refuses constructors that:
    - need scopes (`needsScopes == true`), and/or
    - have parameters (`ctorFunc.Params.Count > 0`).
  - even when allowed, the method comments note base `.ctor` call + field init are not implemented.

### What is already migrated (supported end-to-end by new pipeline)
This is the set of AST constructs that the new pipeline can currently parse and lower (subject to the closure restrictions above):

**Statements (HIRBuilder)**
- Variable declarations with *identifier* declarators only (`var/let/const x = ...`), including multiple declarators in a single `VariableDeclaration`.
- Expression statements.
- `if/else`.
- Block statements (including child block scopes for `{}` and `for (let ...)` scopes).
- `return` (with or without value).
- `for (init; test; update) { ... }` where:
  - `init` is a `VariableDeclaration` (identifier declarators only) or an expression,
  - `test` is optional expression,
  - `update` is optional expression.

**Expressions (HIRBuilder)**
- Binary expressions (`BinaryExpression`) (operator support depends on lowering + LIR/IL support).
- Call expressions (`CallExpression`) with expression callee.
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

**Lowering / IL emission highlights (LIR)**
- Numeric arithmetic + bitwise + shifts + comparisons have LIR instruction types and IL emission cases.
- Dynamic equality/inequality and strict equality/inequality are represented in LIR and emitted via runtime helpers.
- Array/object literal construction and array spread lowering have LIR instruction types and IL emission cases.
- Default parameter initialization exists in lowering (AssignmentPattern where LHS is Identifier).

## Legacy Pipeline Coverage (what old AST → IL can do today that HIR can’t)
Based on the legacy emitters:

**Statements supported in legacy but not in HIRBuilder**
- `throw`
- `try/catch/finally`
- `while`
- `do { } while (...)`
- `for (x of y)`
- `break` / `continue`

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

- [ ] Enable new pipeline for methods where `scope.ReferencesParentScopeVariables` is true.
- [ ] Enable new pipeline for scopes that contain captured bindings (`IsCaptured == true`).
- [ ] Audit + complete captured-variable *writes* semantics (reads/writes must agree with legacy closure behavior).
- [ ] Ensure leaf scope instance creation is correct in all cases:
  - [ ] create leaf scope instance exactly when required (and only once)
  - [ ] correct leaf local lifetime across control-flow and loops
- [ ] Ensure call sites always build the correct scopes array for callee requirements:
  - [ ] direct calls (`f(...)`)
  - [ ] calls via variables / re-assignment
  - [ ] nested functions and function expressions used as values
  - [ ] class method calls that access parent scopes
- [ ] Confirm the scopes source selection is correct for:
  - [ ] static methods (scopes passed as arg)
  - [ ] instance methods (scopes loaded from `this._scopes`)
- [ ] Expand execution tests for closures (both reads and writes across nesting levels).

### 2) Expand HIR statement support to match legacy
- [ ] `while` statements
- [ ] `do/while` statements
- [ ] `for..of` statements
- [ ] `for..in` statements (if legacy supports it; confirm)
- [ ] `break` / `continue` (requires loop label tracking)
- [ ] `switch` statements (if legacy supports it; confirm)
- [ ] `try/catch/finally`
  - [ ] Add HIR nodes for try/catch/finally (including catch parameter binding)
  - [ ] Add LIR representation for exception regions (try region + handler regions)
  - [ ] Emit IL exception regions in `LIRToILCompiler` (ExceptionRegion / EH tables)
  - [ ] Ensure `finally` executes on all exits (normal fallthrough, `return`, `throw`, `break`/`continue` once supported)
  - [ ] Validate catch variable scoping semantics match legacy (block-scoped catch param)
- [ ] `throw`

### 3) Expand HIR expression support to match legacy
- [ ] `ConditionalExpression` (ternary)
- [ ] `LogicalExpression` (`&&`, `||`) with correct short-circuit semantics
- [ ] `NewExpression`
  - [ ] built-in Error types
  - [ ] user-defined classes
  - [ ] argument count checking (match legacy)
- [ ] `TemplateLiteral` (including interpolation)
- [ ] `ThisExpression` (especially important for class methods)
- [ ] `FunctionExpression` as an expression (closure creation)
- [ ] `ArrowFunctionExpression` as an expression (closure creation)
  - [ ] concise-body arrows must wrap implicit return

### 4) Variable declarators & assignment targets
The new HIR currently only supports identifier declarators and identifier assignment LHS.

- [ ] Variable declarator destructuring:
  - [ ] `const {a, b} = obj`
  - [ ] `const [a, b] = arr`
  - [ ] nested patterns + defaults
  - [ ] rest elements (`...rest`)
- [ ] Assignment targets beyond identifiers:
  - [ ] `obj.prop = value`
  - [ ] `obj[index] = value`
  - [ ] destructuring assignment (`({a} = obj)`)
- [ ] Object literal spread properties (`{...x, a: 1}`)
- [ ] Object literal computed keys (`{ [expr]: value }`)
- [ ] Object literal shorthand properties and methods

### 5) Classes: constructors + field initialization
`TryCompileClassConstructor` currently refuses most real constructors.

- [ ] Emit required base constructor call(s):
  - [ ] `System.Object::.ctor` for classes without explicit `extends`
  - [ ] correct `super(...)` behavior for derived classes
- [ ] Support constructor parameters (including defaults / destructuring / rest as applicable).
- [ ] Support field initialization (public fields + private fields + static fields if supported by legacy).
- [ ] Support `this` initialization / return semantics:
  - constructors return `this` unless explicitly returning an object
- [ ] Ensure instance method default return value matches JS (`undefined`), not `this`.

### 6) Two-phase compilation parity
The repo already has a two-phase coordinator and a `TryCompileCallableBody` API in the new pipeline.

- [ ] Ensure all callable shapes used by two-phase mode can be compiled via IR (functions, arrows, class methods, constructors).
- [ ] Ensure dependency discovery and required scope-chain layout are consistent with IR call-site scopes materialization.

### 7) Deletion targets (what can be removed once punch list is complete)
These are candidates to delete **after** the new pipeline reaches feature parity and no longer needs fallback:

**High confidence once IR is complete**
- [ ] `Js2IL/Services/ILGenerators/BinaryOperators.cs`
- [ ] `Js2IL/Services/ILGenerators/ILExpressionGenerator.cs`
- [ ] `Js2IL/Services/ILGenerators/ILMethodGenerator.cs`
- [ ] `Js2IL/Services/TwoPhaseCompilation/LegacyFunctionBodyCompiler.cs`
- [ ] `Js2IL/Services/TwoPhaseCompilation/LegacyClassBodyCompiler.cs`

**Likely, but requires follow-up audit**
- [ ] `Js2IL/Services/VariableBindings/Variables.cs`
- [ ] `Js2IL/Services/VariableBindings/Variable.cs`

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
6. Remove fallbacks; then delete legacy emitters.

## Validation Checklist (per feature)
- Add/extend execution tests under `Js2IL.Tests/*/ExecutionTests`.
- Update generator snapshots only after execution behavior matches legacy (if snapshots are used for the area).
- Confirm `IRPipelineMetrics` has no new failure hotspots for common test suites.

**Exception handling validation (try/catch/finally)**
- `try/finally` runs finally on `return` and `throw`.
- `try/catch` binds catch parameter correctly and does not leak it outside the catch block.
- Nested try blocks (try inside catch/finally) behave as expected.
