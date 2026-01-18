# JavaScript → .NET Type Mapping (Modules, Scopes, Functions, Arrows, Classes)

This document describes how JS2IL maps JavaScript program structure to generated .NET types and methods.

It is intentionally **implementation-oriented** (what JS2IL emits today), and complements the deeper closure ABI design in:

- `docs/CapturedVariables_ScopesABI.md`
- `docs/TwoPhaseCompilationPipeline.md`

---

## Quick mapping table

| JavaScript concept | Emitted .NET artifact | Notes |
|---|---|---|
| Module | `Scripts.<ModuleId>` type with static `Main(...)` | One per module. Entry point uses CommonJS-style parameters. |
| Lexical scope (global/function/block/class/method) | `Scopes.<ModuleId>` root type + **nested scope types** | “Scope-as-class”: each scope becomes a reference type; variables become instance fields when needed. |
| Function declaration `function f(){}` | static method on `Functions.<ModuleId>` | Method name is the function name. |
| Function expression `const g = function() {}` | `Functions.FunctionExpression_L<line>C<col>` type with a static method | Uses source location for stable identity. |
| Arrow function `const a = () => {}` / inline arrows | `Functions.ArrowFunction_L<line>C<col>` type with a static method | Uses source location for stable identity. |
| Class `class C { ... }` | `Classes.<ModuleId>.C` (or custom `DotNetNamespace`) | Instance/static fields become .NET fields; methods become .NET methods. |

---

## Namespaces and top-level generated types

JS2IL uses a small number of namespaces to keep generated artifacts discoverable:

- `Scripts`: module entrypoints.
- `Scopes`: closure/environment types generated from the symbol-table scope tree.
- `Functions`: generated methods for functions (declarations) and anonymous callables (arrows/function expressions).
- `Classes`: generated types for JavaScript classes (can be overridden per class via `Scope.DotNetNamespace`).

### Module id (`<ModuleId>`)

JS2IL uses a stable module id (from `ModuleDefinition.Name`) when naming per-module types.

- The module id is used to form type names like `Scripts.<ModuleId>` and `Scopes.<ModuleId>`.
- In multi-module builds, every module gets its own independent `Scripts/Scopes/Functions` artifacts to avoid name collisions.

---

## Module mapping (`Scripts.<ModuleId>.Main`)

Each module emits a type:

- `Scripts.<ModuleId>`

and a method:

- `public static void Main(exports, require, module, __filename, __dirname)`

This matches a CommonJS-like execution model.

Key points:

- The module main body is compiled through the IR pipeline.
- The main method **does not** take an `object[] scopes` parameter.
- If the module needs a global scope instance for captured bindings, it is created and managed inside the compiled body.

---

## Scope mapping (`Scopes.<ModuleId>+...`)

JS2IL uses a **scope-as-class** model:

- Every JavaScript scope becomes a generated .NET **reference type** (“scope class”).
- Scope types are emitted under the `Scopes` namespace.
- Child scopes become **nested .NET types** of their parent scope type.

### Root scope type

For a module with id `<ModuleId>`, the root scope type is:

- `Scopes.<ModuleId>`

### Nested scope types

Nested scopes become nested types:

- Block scopes: `Scopes.<ModuleId>+Block_L<line>C<col>` (pattern depends on SymbolTableBuilder naming)
- Function scopes: `Scopes.<ModuleId>+...+<FunctionOrSyntheticName>`
- Class scopes: nested under the surrounding scope type
- Class method scopes: nested under the class scope type (these are *scope* types, not the runtime JS class)

The exact scope names come from the symbol table (see `Js2IL.Tests/ScopeNamingTests.cs`). For example:

- Assigned arrow scope: `ArrowFunction_<varName>`
- Assigned function-expression scope: `FunctionExpression_<varName>`
- Inline arrow scope: `ArrowFunction_L<line>C<col>` (column in the symbol table is 0-based)

### What becomes a field vs a local

A binding becomes a **field** on the scope type when it needs stable addressable storage across call frames:

- Captured variables (referenced by nested callables)
- Function declarations (stored so the function value/delegate can be referenced)
- Some parameters (e.g., destructuring parameters; and arrow parameters for closure semantics)

Bindings that are not captured typically compile to IL locals rather than scope fields.

### Base type for special scopes

Scope types may inherit from runtime helper types when the scope represents a resumable callable:

- Async scopes inherit `JavaScriptRuntime.AsyncScope`
- Generator scopes inherit `JavaScriptRuntime.GeneratorScope`
- Otherwise scope types inherit `System.Object`

---

## Closure environment ABI (`object[] scopes`)

Most generated callables (functions, arrows, function expressions) use an environment parameter:

- `object[] scopes`

The `scopes` array is a runtime chain of *scope instances* for lexical ancestors.

Ordering (ideal/current direction):

- `scopes[0]` = global/module scope instance
- increasing indices move inward toward the callee’s nearest lexical ancestor

This is specified in detail in `docs/CapturedVariables_ScopesABI.md`.

---

## Function mapping

### Function declarations (`function f(){}`)

Function declarations are emitted as static methods on a per-module container type:

- `Functions.<ModuleId>`

Method name:

- `f`

Signature shape (conceptual):

- `static object f(object[] scopes, object p0, object p1, ...)`

Notes:

- The scopes parameter is present to support lexical captures and forwarding.
- The callable’s *scope type* (for captured locals/params) is separate and lives under `Scopes.<ModuleId>+...`.

### Function expressions (`const g = function() {}`)

Function expressions are emitted as their own per-callable type:

- `Functions.FunctionExpression_L<line>C<col>`

and a static method (also named `FunctionExpression_L<line>C<col>`).

Rationale:

- Function expressions need a stable identity even when anonymous.
- Using source location makes metadata ordering and token lookup deterministic in the two-phase pipeline.

---

## Arrow function mapping

Arrow functions follow the same “per-callable type” model as function expressions:

- `Functions.ArrowFunction_L<line>C<col>`

with a static method `ArrowFunction_L<line>C<col>`.

Key semantic note:

- Arrow functions inherit `this` lexically (unlike non-arrow functions).
- JS2IL’s closure model relies on scope instances + the `scopes` chain to provide access to captured state.

---

## Class mapping (`Classes.<ModuleId>.<ClassName>`)

JavaScript classes are emitted as real .NET types:

- Default: `Classes.<ModuleId>.<ClassName>`
- Override: `Scope.DotNetNamespace` / `Scope.DotNetTypeName` (when provided by the symbol table)

### Fields

- JavaScript instance fields become .NET instance fields on the class type.
- JavaScript static fields become .NET static fields on the class type.
- Private fields are name-mangled (current convention: `__js2il_priv_<name>`).

### Methods and constructors

- Constructors emit `.ctor`.
- Instance methods are .NET instance methods.
- Static methods are .NET static methods.
- Accessors use `get_<name>` / `set_<name>`.

### Parent-scope access from class methods (`_scopes`)

Classes can capture lexically enclosing variables (e.g., class declared inside a function).

When a class needs parent scopes, JS2IL adds a private field:

- `_scopes : object[]`

and:

- Constructors accept an `object[] scopes` parameter and store it in `this._scopes`.
- Instance methods read parent scopes from `this._scopes` (they do *not* take `object[] scopes` as a parameter).

This keeps the “scopes chain” available throughout instance method execution.

### Class scope vs runtime class instance

Important distinction:

- The *runtime JS class* is the generated .NET type under `Classes.*`.
- The *lexical scope for class bodies / methods* is still represented in `Scopes.*` as scope types.

Scope types exist to model captured variables and closure semantics; class instances exist to model `this` and instance field storage.

---

## Example (conceptual)

Given:

```js
// module: foo.js
const x = 1;

function f(y) {
  const z = 2;
  return () => x + y + z;
}

class C {
  constructor(v) { this.v = v; }
  m() { return this.v; }
}
```

A typical shape of emitted artifacts:

- `Scripts.foo.Main(...)`
- `Scopes.foo` (global scope type)
  - field(s) for captured/hoisted bindings like `x` and `f` (if captured/needed)
  - nested scope types for `f` and the arrow
- `Functions.foo.f(object[] scopes, object y)`
- `Functions.ArrowFunction_L<line>C<col>.ArrowFunction_L<line>C<col>(object[] scopes)`
- `Classes.foo.C` with:
  - field `v`
  - `.ctor(...)`
  - method `m(...)`

(The exact nested type names and field sets depend on capture analysis and symbol-table naming.)

---

## Related implementation references

- Closure ABI and scopes-array conventions: `docs/CapturedVariables_ScopesABI.md`
- Two-phase compilation and callable identity/token preallocation: `docs/TwoPhaseCompilationPipeline.md`
- Scope naming behavior: `Js2IL.Tests/ScopeNamingTests.cs`
