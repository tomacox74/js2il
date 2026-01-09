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
- [ ] PL1.5 Ensure call sites always build the correct scopes array for callee requirements:
  - [x] PL1.5a direct calls (`f(...)`)
  - [ ] PL1.5b calls via variables / re-assignment (blocked on IR support for function values / rebinding)
  - [ ] PL1.5c nested functions and function expressions used as values (blocked on IR support for FunctionExpression/ArrowFunctionExpression as values)
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
- [ ] PL3.3 `NewExpression`
  - [x] PL3.3a built-in Error types
  - [x] PL3.3b user-defined classes
  - [ ] PL3.3c argument count checking (match legacy)
  - [x] PL3.3d built-in Array constructor (`new Array()` / `new Array(n)` / `new Array(a, b, ...)`) semantics
  - [x] PL3.3e built-in String constructor (`new String()` / `new String(x)`) as syntactic sugar for native strings
  - [x] PL3.3f built-in Boolean/Number constructors (`new Boolean(x)`, `new Number(x)`) wrapper/sugar semantics
  - [x] PL3.3g built-in constructors for remaining existing runtime intrinsics (e.g., Date, RegExp, Set, Promise, Int32Array)
- [x] PL3.4 `TemplateLiteral` (including interpolation)
- [ ] PL3.5 `ThisExpression` (especially important for class methods)
- [ ] PL3.6 `FunctionExpression` as an expression (closure creation)
- [ ] PL3.7 `ArrowFunctionExpression` as an expression (closure creation)
  - [ ] PL3.7a concise-body arrows must wrap implicit return

### 8) Intrinsic constructor-like calls (no `new`)
These are `CallExpression` forms (e.g., `Date(x)`, `Boolean()`) and are distinct from `NewExpression`.

- [ ] PL8.1 Primitive conversion callables: `String(x)`, `Number(x)`, `Boolean(x)`
- [ ] PL8.2 `Date(...)` callable form (returns string per JS semantics)
- [ ] PL8.3 `RegExp(pattern, flags?)` callable form
- [ ] PL8.4 Error callables: `Error(message?)` and derived errors (TypeError, RangeError, etc.)
- [ ] PL8.5 `Array(...)` callable form
- [ ] PL8.6 `Object(value?)` callable form
- [ ] PL8.7 Other callable-only intrinsics: `Symbol(description?)`, `BigInt(value)`

### 4) Variable declarators & assignment targets
The new HIR currently only supports identifier declarators and identifier assignment LHS.

- [ ] PL4.1 Variable declarator destructuring:
  - [ ] PL4.1a `const {a, b} = obj`
  - [ ] PL4.1b `const [a, b] = arr`
  - [ ] PL4.1c nested patterns + defaults
  - [ ] PL4.1d rest elements (`...rest`)
- [ ] PL4.2 Assignment targets beyond identifiers:
  - [ ] PL4.2a `obj.prop = value`
  - [ ] PL4.2b `obj[index] = value`
  - [ ] PL4.2c destructuring assignment (`({a} = obj)`)
- [ ] PL4.3 Object literal spread properties (`{...x, a: 1}`)
- [ ] PL4.4 Object literal computed keys (`{ [expr]: value }`)
- [ ] PL4.5 Object literal shorthand properties and methods

### 5) Classes: constructors + field initialization
`TryCompileClassConstructor` currently refuses most real constructors.

- [ ] PL5.1 Emit required base constructor call(s):
  - [ ] PL5.1a `System.Object::.ctor` for classes without explicit `extends`
  - [ ] PL5.1b correct `super(...)` behavior for derived classes
- [ ] PL5.2 Support constructor parameters (including defaults / destructuring / rest as applicable).
- [ ] PL5.3 Support field initialization (public fields + private fields + static fields if supported by legacy).
- [ ] PL5.4 Support `this` initialization / return semantics:
  - [ ] PL5.4a constructors return `this` unless explicitly returning an object
- [ ] PL5.5 Ensure instance method default return value matches JS (`undefined`), not `this`.

### 6) Two-phase compilation parity
The repo already has a two-phase coordinator and a `TryCompileCallableBody` API in the new pipeline.

- [ ] PL6.1 Ensure all callable shapes used by two-phase mode can be compiled via IR (functions, arrows, class methods, constructors).
- [ ] PL6.2 Ensure dependency discovery and required scope-chain layout are consistent with IR call-site scopes materialization.

### 7) Deletion targets (what can be removed once punch list is complete)
These are candidates to delete **after** the new pipeline reaches feature parity and no longer needs fallback:

**High confidence once IR is complete**
- [ ] PL7.1 `Js2IL/Services/ILGenerators/BinaryOperators.cs`
- [ ] PL7.2 `Js2IL/Services/ILGenerators/ILExpressionGenerator.cs`
- [ ] PL7.3 `Js2IL/Services/ILGenerators/ILMethodGenerator.cs`
- [ ] PL7.4 `Js2IL/Services/TwoPhaseCompilation/LegacyFunctionBodyCompiler.cs`
- [ ] PL7.5 `Js2IL/Services/TwoPhaseCompilation/LegacyClassBodyCompiler.cs`

**Likely, but requires follow-up audit**
- [ ] PL7.6 `Js2IL/Services/VariableBindings/Variables.cs`
- [ ] PL7.7 `Js2IL/Services/VariableBindings/Variable.cs`

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
