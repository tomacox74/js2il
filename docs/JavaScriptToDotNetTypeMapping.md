# JavaScript → .NET Type Mapping (informational)

This document describes how JS2IL maps JavaScript program structure to generated .NET types and methods.

It is intentionally **implementation-oriented** (what JS2IL emits today) and **not a specification**. The emitted shapes are free to change as the compiler evolves.

It complements the deeper closure ABI design in:

- [docs/CapturedVariables_ScopesABI.md](CapturedVariables_ScopesABI.md)
- [docs/compiler/TwoPhaseCompilationPipeline.md](compiler/TwoPhaseCompilationPipeline.md)

## Index

- [Quick mapping table](#quick-mapping-table)
- [Runtime value representation](#runtime-value-representation)
  - [Primitive types](#primitive-types)
- [Module id](#module-id)
- [Module mapping](#module-mapping)
- [Callable mapping](#callable-mapping)
  - [Function declarations](#function-declarations)
  - [Function expressions](#function-expressions)
  - [Arrow functions](#arrow-functions)
- [Type lowering and normalization](#type-lowering-and-normalization)
- [Scope mapping](#scope-mapping)
  - [Root scope type](#root-scope-type)
  - [Nested scope types](#nested-scope-types)
  - [Visibility](#visibility)
  - [What becomes a field vs a local](#what-becomes-a-field-vs-a-local)
  - [Base type for special scopes](#base-type-for-special-scopes)
- [Closure environment ABI](#closure-environment-abi)
- [Class mapping](#class-mapping)
  - [Parent-scope access from class methods](#parent-scope-access-from-class-methods)
- [`globalThis`](#globalthis)
- [Intrinsic objects](#intrinsic-objects)
- [Node-compatible modules (`require`)](#node-compatible-modules)
  - [Specifier normalization and local resolution](#specifier-normalization-and-local-resolution)
- [Example](#example)
- [Related implementation references](#related-implementation-references)

---

<a id="quick-mapping-table"></a>
## Quick mapping table

| JavaScript concept | Emitted .NET artifact | Notes |
|---|---|---|
| Module | `Modules.<ModuleId>` type with static `__js_module_init__(...)` | One per entry file (“module”). Uses CommonJS-style parameters. |
| Lexical scope (global/function/block/class/method) | Nested scope types (e.g., `Modules.<ModuleId>+Scope`, `...+Scope+Block_L<line>C<col>`) | “Scope-as-class”: each scope becomes a reference type; variables become instance fields when needed. |
| Function declaration `function f(){}` | Nested owner type `Modules.<ModuleId>+f` with static `__js_call__(object[] scopes, ...)` | Functions compile to a callable “owner type” so they can be nested and referenced as values. |
| Function expression `const g = function() {}` | Nested owner type `...+FunctionExpression_L<line>C<col>` with static `__js_call__(object[] scopes, ...)` | Uses symbol-table naming (often source location) for stable identity. |
| Arrow function `const a = () => {}` / inline arrows | Nested owner type `...+ArrowFunction_L<line>C<col>` with static `__js_call__(object[] scopes, ...)` | Arrow functions inherit `this` lexically; still use `object[] scopes` for captures. |
| Class `class C { ... }` | Usually nested type `Modules.<ModuleId>+C` (or custom `DotNetNamespace`) | Module-scope and function-local classes are emitted as real .NET types. |

---

<a id="runtime-value-representation"></a>
## Runtime value representation (informational)

JS2IL generally represents JavaScript values as boxed CLR values (`object`) at runtime. Two important sentinels used by the runtime today:

- JavaScript `undefined` is represented as CLR `null`.
- JavaScript `null` is represented as `JavaScriptRuntime.JsNull.Null` (a boxed enum value).

<a id="primitive-types"></a>
### Primitive types

Primitive values are typically represented using CLR primitive types (boxed as `object` when flowing through generic JS operations):

- JavaScript `number` is typically a CLR `double`.
- JavaScript `boolean` is a CLR `bool`.
- JavaScript `string` is a CLR `string`.
- JavaScript `undefined` is CLR `null`.
- JavaScript `null` is `JavaScriptRuntime.JsNull.Null`.

Notes:

- Some runtime helpers (e.g., `JavaScriptRuntime.TypeUtilities.ToNumber(...)`, `ToBoolean(...)`, `Typeof(...)`) implement coercion and `typeof` based on these representations.
- JS2IL may opportunistically store some variables/fields as `double`/`bool`/`string` when type inference marks them stable, but semantically values still flow as JavaScript values.

---

<a id="module-id"></a>
## Module id (`<ModuleId>`)

JS2IL uses a stable module id (derived from the compiled entry file / `ModuleDefinition.Name`) when naming the per-module root type.

- The module id forms the type name `Modules.<ModuleId>`.
- In multi-module builds, every module gets its own `Modules.<ModuleId>` root type, and nested types underneath it.

---

<a id="module-mapping"></a>
## Module mapping (`Modules.<ModuleId>.__js_module_init__`)

Each compiled module emits a type:

- `Modules.<ModuleId>`

and a method:

- `public static void __js_module_init__(exports, require, module, __filename, __dirname)`

This matches a CommonJS-like execution model.

Key points:

- The module main body is compiled through the IR pipeline.
- The module init method **does not** take an `object[] scopes` parameter.
- If the module needs a global scope instance for captured bindings, it is created and managed inside the compiled body.

---

<a id="callable-mapping"></a>
## Callable mapping (functions, arrows, function expressions)

JS2IL represents each callable as a nested “owner type” with a single callable entrypoint method.

Entrypoint method name (current convention):

- `static object __js_call__(object[] scopes, object p0, object p1, ...)`

Nesting rules (high-level):

- Module-scope callables are nested under `Modules.<ModuleId>`.
- Nested callables are nested under the nearest enclosing callable owner type (not under the enclosing scope type).

<a id="function-declarations"></a>
### Function declarations (`function f(){}`)

Function declarations are emitted as a nested owner type under the module type:

- `Modules.<ModuleId>+f`

with a callable entrypoint method:

- `static object __js_call__(object[] scopes, object p0, object p1, ...)`

<a id="function-expressions"></a>
### Function expressions (`const g = function() {}`)

Function expressions are emitted as a nested owner type (nested under the nearest enclosing owner type):

- `...+FunctionExpression_L<line>C<col>`

Rationale:

- Function expressions need a stable identity even when anonymous.
- Using source location (or symbol-table-assigned names) keeps metadata stable for the two-phase pipeline.

<a id="arrow-functions"></a>
### Arrow functions

Arrow functions follow the same “per-callable owner type” model as function expressions:

- `...+ArrowFunction_L<line>C<col>`

Key semantic note:

- Arrow functions inherit `this` lexically (unlike non-arrow functions).

---

<a id="type-lowering-and-normalization"></a>
## Type lowering and normalization (informational)

JS2IL has an internal IR pipeline for compiling executable bodies (module init bodies, functions, class methods, etc.).

High-level shape (today):

- AST → HIR → LIR → IL

Where:

- **HIR** (high-level IR) is a structured representation built from the AST.
- **LIR** (lowered IR) is a more explicit, IL-friendly representation (explicit temps, labels, and runtime calls).

After HIR→LIR lowering, JS2IL runs conservative “normalization” passes that rewrite LIR into more explicit or more typed forms.
These passes are intentionally IL-agnostic: the goal is to keep the LIR→IL compiler focused on IL mechanics (stack/locals/metadata) rather than fragile late pattern matching.

### Intrinsic normalization

`LIRIntrinsicNormalization` rewrites generic operations into intrinsic-specific operations when the receiver type and operands are provably known.

Examples of the kinds of rewrites this enables:

- `GetItem(obj, index)` → `GetInt32ArrayElement(arr, index)` when `obj` is proven to be `JavaScriptRuntime.Int32Array` and `index` is a numeric index
- `GetItem(arr, index)` → `GetJsArrayElement(arr, index)` when `arr` is `JavaScriptRuntime.Array`
- `GetLength(obj)` → `GetJsArrayLength(arr)` / `GetInt32ArrayLength(arr)` when receiver type is known

This typically also updates temp storage so downstream codegen can treat results as unboxed `double` where appropriate.

### Type normalization

`LIRTypeNormalization` performs conservative peephole rewrites that remove unnecessary boxing/conversions and preserve typed locals when safe.

Examples:

- Rewriting `Object.NormalizeForOfIterable(x)` to a direct copy when `x` is already a proven iterable (currently: `JavaScriptRuntime.Array`)
- Rewriting a typed field store to consume an unboxed temp directly when the boxed temp was only created for an intermediate step

Where this fits in the codebase:

- The orchestration is in [Js2IL/JsMethodCompiler.cs](../Js2IL/JsMethodCompiler.cs) (`TryLowerASTToLIR`)
- Intrinsic normalization: [Js2IL/IR/LIR/LIRIntrinsicNormalization.cs](../Js2IL/IR/LIR/LIRIntrinsicNormalization.cs)
- Type normalization: [Js2IL/IR/LIR/LIRTypeNormalization.cs](../Js2IL/IR/LIR/LIRTypeNormalization.cs)
- Tests/examples of normalization behavior: [Js2IL.Tests/LIRIntrinsicNormalizationTests.cs](../Js2IL.Tests/LIRIntrinsicNormalizationTests.cs)

---

<a id="scope-mapping"></a>
## Scope mapping (nested `...+Scope+...` types)

JS2IL uses a **scope-as-class** model:

- Every JavaScript scope becomes a generated .NET **reference type** (“scope class”).
- Scope types are emitted as **nested types**.
- Child scopes become nested types of their parent scope type.

<a id="root-scope-type"></a>
### Root scope type

For a module with id `<ModuleId>`, the root (global/module) scope type is nested under the module root type:

- `Modules.<ModuleId>+Scope`

<a id="nested-scope-types"></a>
### Nested scope types

Nested scopes become nested types. Typical shapes you will see in IL:

- Block scopes: `...+Scope+Block_L<line>C<col>` (pattern depends on symbol table naming)
- Callable scopes: `...+<CallableOwner>+Scope` (function declarations, arrows, and function expressions)
- Class scopes: `...+<ClassName>+Scope` (class lexical scope)
- Class member scopes: nested under the runtime class type as siblings of the class scope (e.g., `...+<ClassName>+Scope_ctor`)

The exact scope names come from the symbol table (see `Js2IL.Tests/ScopeNamingTests.cs`). For example:

- Assigned arrow scope: `ArrowFunction_<varName>`
- Assigned function-expression scope: `FunctionExpression_<varName>`
- Inline arrow scope: `ArrowFunction_L<line>C<col>` (column in the symbol table is 0-based)

<a id="visibility"></a>
### Visibility (current behavior)

Scope types are generally emitted as nested private types (`NestedPrivate`). This is an implementation detail, but it is important for reflection-based tooling and tests.

<a id="what-becomes-a-field-vs-a-local"></a>
### What becomes a field vs a local

A binding becomes a **field** on the scope type when it needs stable addressable storage across call frames:

- Captured variables (referenced by nested callables)
- Function declarations (stored so the function value/delegate can be referenced)
- Some parameters (e.g., destructuring parameters; and arrow parameters for closure semantics)

Bindings that are not captured typically compile to IL locals rather than scope fields.

<a id="base-type-for-special-scopes"></a>
### Base type for special scopes

Scope types may inherit from runtime helper types when the scope represents a resumable callable:

- Async scopes inherit `JavaScriptRuntime.AsyncScope`
- Generator scopes inherit `JavaScriptRuntime.GeneratorScope`
- Otherwise scope types inherit `System.Object`

---

<a id="closure-environment-abi"></a>
## Closure environment ABI (`object[] scopes`)

Most generated callables (functions, arrows, function expressions) use an environment parameter:

- `object[] scopes`

The `scopes` array is a runtime chain of *scope instances* for lexical ancestors.

Ordering (ideal/current direction):

- `scopes[0]` = global/module scope instance
- increasing indices move inward toward the callee’s nearest lexical ancestor

This is specified in detail in [docs/CapturedVariables_ScopesABI.md](CapturedVariables_ScopesABI.md).

---

<a id="class-mapping"></a>
## Class mapping (nested under module / callable owners)

JavaScript classes are emitted as real .NET types:

- Default: `Modules.<ModuleId>+<ClassName>` (module-scope) or `...+<EnclosingCallable>+<ClassName>` (function-local)
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

<a id="parent-scope-access-from-class-methods"></a>
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

- The *runtime JS class* is the generated .NET class type (nested under `Modules.<ModuleId>` or a callable owner type).
- The *lexical scope for class bodies / methods* is still represented as nested scope types (e.g., `...+<ClassName>+Scope`).

Scope types exist to model captured variables and closure semantics; class instances exist to model `this` and instance field storage.

---

<a id="globalthis"></a>
## `globalThis` (informational)

JS2IL provides a minimal `globalThis` surface via the runtime type `JavaScriptRuntime.GlobalThis`.

Current notable globals include:

- `console` (backed by `JavaScriptRuntime.Console`)
- `process` (backed by `JavaScriptRuntime.Node.Process`, including `exitCode`)
- `Infinity` and `NaN`
- Timer functions: `setTimeout`, `clearTimeout`, `setImmediate`, `clearImmediate`, `setInterval`, `clearInterval`
- `parseInt`

Internally, `GlobalThis` is backed by a per-thread service container so tests and hosts can provide implementations.

---

<a id="intrinsic-objects"></a>
## Intrinsic objects (informational)

JS2IL’s runtime includes a set of “intrinsic objects” that model built-ins (e.g., `Array`, `Object`, `Math`, `JSON`, `Promise`).

Implementation details:

- Intrinsic types are annotated with `[JavaScriptRuntime.IntrinsicObject("<Name>")]`.
- `JavaScriptRuntime.IntrinsicObjectRegistry` scans the runtime assembly for these attributes and can map an intrinsic name (like `"Math"`) to the CLR type.
- Some intrinsics also record a call/construct behavior via `IntrinsicCallKind` (e.g., `ArrayConstruct`, `ObjectConstruct`, `BuiltInError`).

Examples present in the runtime today include (non-exhaustive):

- `Array`, `Object`, `Number`, `String`, `Boolean`
- `Math`, `JSON`
- `Promise`
- `RegExp`, `Date`
- `Error` and other built-in errors (`TypeError`, `RangeError`, ...)
- `Set`, `Int32Array`

---

<a id="node-compatible-modules"></a>
## Node-compatible modules (`require`) (informational)

JS2IL supports a Node-like CommonJS `require(...)` model at runtime.

Two categories are handled:

- Local modules (user code compiled into the same output): `require("./relative")`
- Node built-in modules (a supported subset): `require("fs")`, `require("node:fs")`, etc.

Implementation details (current behavior):

- Local module resolution loads another generated module type (e.g., `Modules.<ModuleId>`) from the compiled local-modules assembly.
- The runtime currently accepts either `__js_module_init__` (current) or `Main` (legacy) as the module entrypoint.
- Node module support is implemented by classes in `JavaScriptRuntime.Node` annotated with `[JavaScriptRuntime.Node.NodeModule("<name>")]`.
- `JavaScriptRuntime.Node.NodeModuleRegistry` discovers these types and `JavaScriptRuntime.CommonJS.Require` instantiates and caches singleton module instances.

<a id="specifier-normalization-and-local-resolution"></a>
### Specifier normalization and local resolution

JS2IL’s runtime `require(...)` performs a small amount of normalization and resolution that is useful to know when reasoning about module ids:

- Backslashes are normalized to forward slashes (`\` → `/`).
- A `node:` prefix is accepted for Node built-ins (e.g., `require("node:fs")`) and is stripped before lookup.
- Relative local specifiers (`./...` and `../...`) are resolved against the requiring module’s path, then dot segments are normalized.
- Absolute local specifiers (`/...`) are treated as already rooted and are not re-based.

In other words, local-module resolution is “CommonJS-like” and path-based, while Node built-in module resolution is name-based (from the `[NodeModule]` registry).

For the current supported set, see `docs/NodeSupport.md` / `docs/NodeSupport.json`.

---

<a id="example"></a>
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

- `Modules.foo.__js_module_init__(...)`
- `Modules.foo+Scope` (global scope type)
  - field(s) for captured/hoisted bindings like `x` and `f` (if captured/needed)
- `Modules.foo+f.__js_call__(object[] scopes, object y)`
- `Modules.foo+f+ArrowFunction_L<line>C<col>.__js_call__(object[] scopes)`
- `Modules.foo+C` with:
  - field `v`
  - `.ctor(...)`
  - method `m(...)`

(The exact nested type names and field sets depend on capture analysis and symbol-table naming.)

---

<a id="related-implementation-references"></a>
## Related implementation references

- Closure ABI and scopes-array conventions: [docs/CapturedVariables_ScopesABI.md](CapturedVariables_ScopesABI.md)
- Two-phase compilation and callable identity/token preallocation: [docs/compiler/TwoPhaseCompilationPipeline.md](compiler/TwoPhaseCompilationPipeline.md)
- Module init + scope/class nesting: [Js2IL/JsMethodCompiler.cs](../Js2IL/JsMethodCompiler.cs) (`TryCompileMainMethod`, `EstablishModuleNesting`)
- IR lowering + normalization (AST→HIR→LIR→IL): [Js2IL/JsMethodCompiler.cs](../Js2IL/JsMethodCompiler.cs) (`TryLowerASTToLIR`), [Js2IL/IR/LIR/LIRIntrinsicNormalization.cs](../Js2IL/IR/LIR/LIRIntrinsicNormalization.cs), [Js2IL/IR/LIR/LIRTypeNormalization.cs](../Js2IL/IR/LIR/LIRTypeNormalization.cs)
- Scope type generation: [Js2IL/Services/TypeGenerator.cs](../Js2IL/Services/TypeGenerator.cs)
- Class emission + `_scopes`: [Js2IL/Services/ILGenerators/ClassesGenerator.cs](../Js2IL/Services/ILGenerators/ClassesGenerator.cs)
- Value sentinels (`undefined` vs `null`): [JavaScriptRuntime/TypeUtilities.cs](../JavaScriptRuntime/TypeUtilities.cs), [JavaScriptRuntime/JsNull.cs](../JavaScriptRuntime/JsNull.cs)
- Scope naming behavior: [Js2IL.Tests/ScopeNamingTests.cs](../Js2IL.Tests/ScopeNamingTests.cs)
