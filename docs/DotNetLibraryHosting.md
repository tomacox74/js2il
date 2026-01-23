# Hosting Compiled JavaScript as a .NET Library (design)

This document describes a proposed hosting mode where a compiled assembly can be consumed as a **.NET library**, and JavaScript `module.exports` is surfaced to C# as **strongly typed** APIs.

This is a design document and is intentionally implementation-oriented; it is not an ECMA-262 specification.

The API described here is intentionally geared toward C# developers.

---

## Section 1: User guide (how to use it)

### What you get

- Load and execute a **compiled** module (modules are compiled into an assembly; nothing is loaded from the filesystem).
- A dedicated **script thread** per module runtime instance.
- Strongly typed projections for `module.exports` (interfaces/classes generated per module).
- A `JsEngine` entry point provided by **JavaScriptRuntime** (not generated into each compiled assembly).

### Core concept: a runtime instance

Calling `JsEngine.LoadModule(...)` creates a runtime instance that:

- owns the script thread,
- owns per-thread JavaScript runtime state,
- returns a proxy to the module exports.

Everything you interact with (exports, functions, constructors, object instances) is a proxy that marshals work to that script thread.

### End-to-end example (script + C#)

Example JavaScript module (for example, `example.js`) compiled into *some compiled assembly* under module id `example`:

```js
class Calculator {
      add(x, y) {
            return x + y;
      }
}

function add(x, y) {
      return x + y;
}

async function warmUp() {
      // Any async function returns a Promise in JS.
      // The hosting API projects it as a Task in C#.
}

const version = "1.0.0";

module.exports = {
      version,
      add,
      warmUp,
      Calculator,
};
```

Corresponding C# contract (generated during compilation):

```csharp
public interface IExampleExports : IDisposable
{
    string Version { get; }

    double Add(double x, double y);

    Task WarmUp();

    IJsConstructor<ICalculator> Calculator { get; }
}

public interface ICalculator : IJsHandle
{
    double Add(double x, double y);
}
```

Calling it from C# (recommended patterns):

```csharp
// Option A (best UX): contract carries module + compiled-assembly metadata
using var exports = JsEngine.LoadModule<IExampleExports>();

Console.WriteLine(exports.Version);

var sum = exports.Add(1, 2);
Console.WriteLine(sum);

await exports.WarmUp();

using var calculator = exports.Calculator.Construct();
var sum2 = calculator.Add(3, 4);
Console.WriteLine(sum2);
```

```csharp
// Option B: contract type identifies which compiled assembly to load from,
// moduleId selects the module within that compiled assembly
using var exports2 = JsEngine.LoadModule<IExampleExports>("example");

Console.WriteLine(exports2.Version);

var sum = exports2.Add(1, 2);
Console.WriteLine(sum);

await exports2.WarmUp();

using var calculator = exports2.Calculator.Construct();
var sum2 = calculator.Add(3, 4);
Console.WriteLine(sum2);
```

### Quick start

Load a module that exports an object:

```csharp
// Option A: no module id needed if the contract type is annotated with module metadata
using var exports = JsEngine.LoadModule<IMyExports>();

var answer = exports.Add(40, 2);
```

If you want to specify a module id explicitly (still no assembly parameter needed because it can be inferred from `IMyExports`’s assembly):

```csharp
using var exports = JsEngine.LoadModule<IMyExports>("math");
var answer = exports.Add(40, 2);
```

### Async exports

If an exported function is `async` (returns a JS `Promise`), the generated return type is `Task`:

```csharp
using var exports = JsEngine.LoadModule<IMyExports>("fetcher");
await exports.RefreshAsync();
```

### Exported classes (constructors) and `new`

If a module exports a class:

```js
class Calculator {
   add(x, y) {
      return x + y;
   }
}

module.exports = Calculator;
```

Then in C# you don’t use the C# `new` operator. Instead, the export is treated as a **constructor** and you call `Construct(...)`:

```csharp
using var calculatorCtor = JsEngine.LoadModule<IJsConstructor<ICalculator>>("calculator");
using var calculator = calculatorCtor.Construct();

var sum = calculator.Add(1, 2);
```

If the module exports an object containing a class:

```csharp
using var exports = JsEngine.LoadModule<IMyExports>("calculator");
using var calculator = exports.Calculator.Construct();
```

### Lifetime and disposal (what to `Dispose`)

There are two distinct “cleanup needs”:

1. **Shutting down the script thread/runtime instance**
2. **Releasing handles to individual JS objects**

Recommended pattern:

- Always dispose the object returned by `LoadModule(...)` (the module runtime instance boundary).
- Proxies that represent JS object handles (instances, exported objects, constructors) should be disposable so callers can explicitly release them.

Example:

```csharp
using var exports = JsEngine.LoadModule<IMyExports>("calculator");
using var calculator = exports.Calculator.Construct();

while (condition)
{
      _ = calculator.Add(x, y);
}
```

Important safety property: the runtime should not shut down “too early” just because the exports proxy was garbage collected.

This is achieved by ensuring that every proxy (exports, constructor, instance, etc.) keeps a reference to the underlying runtime instance and participates in reference counting.

---

## Section 2: API reference (proposed)

### Overview

The public API is designed around a single entry point class (`JsEngine`) that activates compiled modules and returns strongly typed proxies.

**Important:** `JsEngine` must live in **JavaScriptRuntime** (a single, shared assembly) to avoid type confusion when multiple compiled JS assemblies are referenced. Compiled output assemblies should *not* each define their own `JsEngine` type.

The canonical entry point type name is:

- `Js2IL.Runtime.JsEngine` (project/type casing: `Js2IL`)

The API uses:

- `IDisposable` to express lifetime boundaries
- generated interfaces/classes for strongly typed exports
- proxy objects to enforce thread affinity

Generated types are expected to come in two layers:

- **Contracts** (interfaces) that callers code against, e.g. `IExampleExports`, `ICalculator`.
- **Proxies** (classes) that implement those interfaces and marshal to the script thread, typically suffixed with `Proxy`.

### Entry point

```csharp
// Js2IL.Runtime.JsEngine
public static class JsEngine
{
      // Loads a module using metadata associated with TExports (module id + compiled assembly identity).
      // This is the preferred API when contracts are generated.
      public static TExports LoadModule<TExports>()
            where TExports : class;

      // Loads a module by id, inferring the target compiled assembly from typeof(TExports).Assembly.
      // IMPORTANT: TExports must be a generated exports contract interface produced during compilation.
      // Hand-authored interfaces are not supported for this overload because they won't carry the
      // compiled-assembly manifest/attributes needed for module resolution and projection.
      public static TExports LoadModule<TExports>(string moduleId)
            where TExports : class;

      // Dynamic / reflection-friendly form: caller specifies which compiled assembly contains modules.
      public static object LoadModule(System.Reflection.Assembly compiledAssembly, string moduleId);

      // Optional convenience for non-Assembly call sites:
      // public static object LoadModule(Type compiledAssemblyMarkerType, string moduleId);
}
```

Intended usage variations:

1. **Strongly typed + no parameters**: `LoadModule<TExports>()` when `TExports` carries enough metadata (recommended).
2. **Strongly typed + module id**: `LoadModule<TExports>(moduleId)` when module id is not embedded (still assembly-inferred).
3. **Dynamic**: `LoadModule(compiledAssembly, moduleId)` when the host decides the target compiled assembly at runtime.

Notes:

- `JsEngine` is in JavaScriptRuntime and is shared across the process/AppDomain.
- `moduleId` is resolved against modules compiled into the **specified/inferred compiled assembly**, not “the assembly containing JsEngine”.

### Exception types (proposed)

The hosting API should expose a small, stable set of exception types that C# consumers can depend on, while allowing the compiler/runtime implementation to evolve internally.

**Design goal:** internal runtime/compiler exceptions should be *translated at the hosting boundary* into public, documented exception types.

Proposed public exception contract:

- `JsRuntimeException : Exception`
      - Base type for all exceptions intentionally surfaced by the hosting API.
      - Should include helpful context when available (module id, export/member name, etc.).

- `JsModuleLoadException : JsRuntimeException`
      - Thrown when a module cannot be loaded/evaluated via `JsEngine.LoadModule(...)`.
      - Examples: module id not found in compiled assembly, module evaluation failed.

- `JsContractProjectionException : JsRuntimeException`
      - Thrown when `module.exports` cannot be projected onto the requested contract type.
      - Examples: missing export member, export type mismatch (e.g. expected function but got object).

- `JsInvocationException : JsRuntimeException`
      - Thrown when a call through a hosting proxy fails (exported function call, instance method call, constructor call).
      - The underlying cause should be preserved in `InnerException`.

- `JsErrorException : JsRuntimeException`
      - Represents a JavaScript `Error` (or other thrown JS value) that was raised during module evaluation or invocation.
      - Should carry JS-facing details when available (message, name, stack).

Notes:

- The exact mapping/translation behavior is tracked as a separate implementation issue.
- This document proposes the stable API surface; it does not require the internal exception representation to match 1:1.

### How does `LoadModule<TExports>()` find the module?

`LoadModule<TExports>()` requires that `TExports` is a generated exports contract type and can provide module identity
information, for example via:

- an attribute on `TExports` (e.g., module id and/or a stable module key), and/or
- a compiled-assembly manifest that maps exports contract types → module ids.

If required metadata is missing, `LoadModule<TExports>()` should throw a helpful exception telling the user to call `LoadModule<TExports>(moduleId)` or `LoadModule(compiledAssembly, moduleId)`.

### What if the module does not implement `TExports`?

`LoadModule<TExports>(...)` is a convenience API: the host is projecting the runtime exports value into an expected contract.

If the loaded module cannot be projected to `TExports`, `LoadModule<TExports>` should throw with a helpful error message describing:

- the requested `TExports` type
- the module id (if provided / resolved)
- the compiled assembly being targeted
- the module’s actual exports contract (if known via manifest)

Recommended behavior:

- If the compiled assembly contains a module manifest describing each module’s generated contract type, validate `TExports` against that manifest and throw an `InvalidOperationException` (or a dedicated `ModuleContractMismatchException`) when it does not match.
- If the host does not have contract metadata, fall back to “attempt projection and fail” (e.g., an `InvalidCastException` if the proxy cannot be cast/converted).

### Naming generated export contracts for nested modules

Modules commonly have path-like ids (e.g., `calculator/index`). Generated contract names should avoid collisions while remaining pleasant to use.

Generated contract namespaces should default to being prefixed with `Js2IL.` + the **compiled assembly name**.

Notes:

- During compilation, the assembly name currently defaults to the script name being compiled.
  - Example: `js2il foo.js` produces `foo.dll` with `<AssemblyName> = foo`.

Recommended convention:

1. Root namespace is `Js2IL.<AssemblyName>`.
2. The “entry module” exports interface is `I<AssemblyName>Exports`.
      - Example: `Js2IL.foo.IFooExports`
3. For non-entry modules (including `require(...)` dependencies), use:
      - namespace: `Js2IL.<AssemblyName>.<ModuleName>`
      - exports interface: `I<ModuleName>Exports`
4. `<ModuleName>` is derived from the module id:
      - split on `/` and `\\`
      - if the last segment is `index`, use the parent segment as `<ModuleName>`
      - otherwise use the last segment

Examples (within `foo.dll`):

- entry module → `Js2IL.foo.IFooExports`
- module id `calculator/index` → `Js2IL.foo.Calculator.ICalculatorExports`
- module id `calculator/advanced` → `Js2IL.foo.Calculator.IAdvancedExports`

Exports for `require(...)` dependencies follow the same rule:

- `Js2IL.<AssemblyName>.<ModuleName>.I<ModuleName>Exports`

 This is preferred over `calculator.IIndexExports` because:

- the namespace is PascalCase (`Calculator`), which is idiomatic in C#
- `ICalculatorExports` is more descriptive than `IIndexExports` for a folder module

If you want an even simpler per-module naming model, an alternative is:

- namespace per module (`JsModules.Calculator`)
- exports interface always named `IExports`

Example: `calculator/index` → `JsModules.Calculator.IExports`

This is concise but makes `IExports` harder to search for globally, so the `I<DisplayName>Exports` approach is usually friendlier.

### Strongly typed exports

The return value of `LoadModule(moduleId)` is whatever the module assigned to `module.exports` during evaluation.

In practice there are two common shapes:

- **Typed per-module interface**: e.g. `I<DisplayName>Exports` (read-only surface)
- **Typed object graph**: an exports interface plus additional interfaces for nested objects/instances

Exports are read-only:

- Exported values can be read, but cannot be changed via the hosting API (no exported setters).
- Exported functions can be invoked.

#### Contract interfaces vs proxy classes

The generator should produce both interfaces and classes (the interfaces are the public contract surface):

- **Generated interfaces** are the public contract and should be pleasant to use and stable over time (not hand-authored for the generic `LoadModule<TExports>(...)` APIs).
      - Example: `public interface IExampleExports : IDisposable { ... }`
      - These are useful to client code as abstractions (DI/mocking/testing, layering, etc.).

- **Generated classes** are proxy implementations that do the actual marshaling/thread-affinity enforcement.
      - Example: `internal sealed class ExampleExportsProxy : IExampleExports { ... }`
      - Naming: prefer a `Proxy` suffix for implementation types because it is accurate and debuggable.
      - Consumers should generally not reference proxy classes directly; they are an implementation detail.

### Constructors (exported classes)

Exported JS classes are represented as constructors.

```csharp
public interface IJsConstructor<out TInstance> : IJsHandle
      where TInstance : class
{
      TInstance Construct(params object[] args);
}
```

Constructed instances are also handles:

```csharp
public interface ICalculator : IJsHandle
{
      double Add(double x, double y);
}
```

### Handle/proxy lifetime

Most non-primitive values returned from exports are proxies that represent JS values living on the script thread.

These proxies should support explicit lifetime management:

```csharp
public interface IJsHandle : IDisposable
{
      // Marker interface: indicates this value is a runtime handle.
}
```

Suggested conventions:

- Generated exports interfaces should implement `IDisposable` (they are the runtime instance boundary).
- Proxies representing JS object handles should implement `IJsHandle`.
- Primitive results (`double`, `bool`, `string`) and `Task` are by-value and not disposable.

### CLR type mapping for exports

When projecting exports into a strongly typed C# surface, the following CLR types are used:

- `object` for unknown/`any`
- `double` for JavaScript `number`
- `string` for JavaScript `string`
- `bool` for JavaScript `boolean`
- `Task` for JavaScript `Promise`

Null-ish values:

- JS `undefined` is represented as CLR `null`
- JS `null` is represented as `JavaScriptRuntime.JsNull.Null`

### Module id

`moduleId` must identify a module compiled into the **target compiled assembly** (e.g., `Modules.<ModuleId>` within that compiled assembly).

---

## Section 3: Implementation details (how it works)

### Goals and non-goals

Goals:

- Allow external C# code to load and execute a compiled module by id.
- Enforce strict thread affinity for the runtime.
- Provide strongly typed exports.

Non-goals (initially):

- Sharing JS runtime state across runtime instances.
- Allowing arbitrary cross-thread access to raw runtime objects.
- Implementing a full Node.js runtime.

### Threading model

Each runtime instance has:

- a dedicated **script thread**
- a per-instance work queue

All JavaScript execution occurs on the script thread:

- module initialization
- reading exported values
- calling exported functions
- timers/callbacks

This is required because the runtime maintains thread-affine state.

### Proxying/marshaling

Because runtime state is thread-affine, all interactions from caller threads must be marshaled:

- Each exported member access or invocation becomes a work item.
- Work is posted to the per-instance queue.
- The script thread executes it and returns a result.

Exceptions thrown on the script thread are captured and rethrown on the calling thread.

### Lifetime and reference counting

To prevent accidental premature shutdown (e.g., exports proxy is GC’d while a derived instance is still alive), the implementation should use reference counting:

- The underlying runtime instance maintains a refcount.
- Every proxy/handle increments the refcount when created.
- Disposing a proxy decrements the refcount.
- When the refcount reaches 0, the runtime shuts down and the script thread exits.

This model supports:

- deterministic shutdown via `Dispose()`
- safe GC fallback (finalizers may call `Release()` as a safety net)

### Module entry point

The module entry point is the generated static initializer method inside the **compiled module assembly**:

- `Modules.<ModuleId>.__js_module_init__(exports, require, module, __filename, __dirname)`

The hosting layer (`JavaScriptRuntime.JsEngine`) is responsible for:

- identifying the **target compiled assembly** (via `TExports` metadata or explicit `Assembly` parameter),
- locating `Modules.<ModuleId>.__js_module_init__(...)` inside that target assembly,
- creating/owning the runtime instance (thread + queue + state),
- returning the strongly typed exports proxy to the caller.

The hosting layer is also responsible for creating:

- the `exports` value
- the `module` object (including `module.exports`)
- the module-scoped `require` delegate
- `__filename` and `__dirname`

### Error handling

During `LoadModule`:

- if module initialization throws, rethrow to the caller
- ensure the script thread and partially initialized state are cleaned up

During export calls:

- capture exceptions on the script thread
- rethrow on the calling thread

Open question: whether some exceptions should permanently fault a runtime instance (e.g., unhandled promise rejection policy) vs. allowing continued use.

### Testing strategy (suggested)

- Execution tests verifying:
  - `LoadModule` returns an `IDisposable`
  - calls route through the script thread (e.g., capture `Thread.ManagedThreadId`)
  - disposing shuts down timers and prevents subsequent calls
- Concurrency tests verifying:
  - multiple runtime instances have isolated thread state
  - reentrancy behavior (calling exports while JS is mid-execution)


### Related docs

- [JavaScriptToDotNetTypeMapping.md](JavaScriptToDotNetTypeMapping.md)
- [compiler/CapturedVariables_ScopesABI.md](compiler/CapturedVariables_ScopesABI.md)
- [compiler/TwoPhaseCompilationPipeline.md](compiler/TwoPhaseCompilationPipeline.md)
- [nodejs/NodeSupport.md](nodejs/NodeSupport.md)
