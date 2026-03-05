# .NET Hosting (Library Mode)

JS2IL can compile a JavaScript module into a normal .NET assembly and let a .NET application **load that compiled module as a library**, then call `module.exports` from C#.

This page is the **canonical user documentation** for hosting. The older document [`docs/DotNetLibraryHosting.md`](../DotNetLibraryHosting.md) is kept as a design/implementation reference.

## What hosting gives you

- **No filesystem module loading at runtime**: modules are compiled into an assembly.
- A dedicated **script thread** per runtime instance.
- Optional **debug symbols**: compile with `--pdb` to emit a Portable PDB (`.pdb`) next to your compiled module for stepping and better stack traces.
- Two ways to call exports:
  - **Typed**: use compiler-generated C# contract interfaces and `JsEngine.LoadModule<TExports>()`.
  - **Dynamic**: use `dynamic` with `JsEngine.LoadModule(Assembly, moduleId)`.
- Stable hosting exception types (`JsModuleLoadException`, `JsInvocationException`, etc.).

## Quick start (typed)

When contracts are generated into the compiled module assembly (default), the easiest pattern is:

```csharp
using Js2IL.Runtime;

// The generated interface type lives in the compiled module assembly.
// It is annotated with [JsModule("<moduleId>")] so no module id is needed here.
using var exports = JsEngine.LoadModule<IMyModuleExports>();

Console.WriteLine(exports.Version);
Console.WriteLine(exports.Add(1, 2));
```

If you need to target a specific module id in that same compiled assembly:

```csharp
using var exports = JsEngine.LoadModule<IMyModuleExports>("calculator/index");
```

## Quick start (dynamic)

```csharp
using Js2IL.Runtime;
using System.Reflection;

var asm = Assembly.LoadFrom("path\\to\\compiled.dll");

// Returns an IDisposable dynamic exports proxy.
using dynamic exports = JsEngine.LoadModule(asm, moduleId: "math");

Console.WriteLine((string)exports.version);
Console.WriteLine((double)exports.add(1, 2));
```

## Tutorials

- [Getting started](tutorials/GettingStarted.md)
- [Typed hosting](tutorials/TypedHosting.md)
- [Dynamic hosting](tutorials/DynamicHosting.md)
- [Async + event loop](tutorials/AsyncAndEventLoop.md)
- [Lifetime + disposal](tutorials/LifetimeAndDisposal.md)
- [Diagnostics + exceptions](tutorials/DiagnosticsAndExceptions.md)
- [Module ids + discovery](tutorials/ModuleIdsAndDiscovery.md)

## API reference

- [`JsEngine`](api/JsEngine.md)
- [Handles + constructors](api/Handles.md)
- [Exceptions](api/Exceptions.md)
- [JS ↔ CLR type mapping](api/TypeMapping.md)

## Samples

The repo includes runnable samples:

- `samples/Hosting.Basic`
- `samples/Hosting.Typed`
- `samples/Hosting.Domino`
