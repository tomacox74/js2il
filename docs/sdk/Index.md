# JROC SDK

JROC's SDK documentation covers the .NET library workflow: compile JavaScript from a .NET project, load compiled modules from C#, or compile and run a module entirely in memory.

This page is the **canonical user documentation** for SDK consumers. The older document [`docs/runtime/DotNetLibraryHosting.md`](../runtime/DotNetLibraryHosting.md) is kept as a design/implementation reference.

## What the SDK gives you

- **Build-integrated compilation**: declare `JrocCompile` items and let `Jroc.SDK` compile JavaScript during `dotnet build`.
- **Library hosting**: load compiled JavaScript assemblies with `Jroc.Runtime.JsEngine` and call `module.exports` from C#.
- **In-memory compile-and-run**: use `Jroc.Core` to produce PE/PDB bytes and optionally load them into a collectible context without writing generated assemblies to disk.
- A dedicated **script thread** per hosted runtime instance.
- Optional **debug symbols**: emit Portable PDB (`.pdb`) data for stepping and better stack traces against the original `.js` / `.mjs` source path, including rewritten `import` / `export` module code.
- Two ways to call exports from hosted modules:
  - **Typed**: use compiler-generated C# contract interfaces and `JsEngine.LoadModule<TExports>()`.
  - **Dynamic**: use `dynamic` with `JsEngine.LoadModule(Assembly, moduleId)`.
- Stable SDK/runtime exception types (`JsModuleLoadException`, `JsInvocationException`, etc.).

For build-integrated host projects, start with the `Jroc.SDK` NuGet package and declare one or more `JrocCompile` items in your `.csproj`. For source-text or artifact-only workflows, use the `Jroc.Core` in-memory APIs.

## Quick start (MSBuild)

```xml
<ItemGroup>
  <PackageReference Include="Jroc.SDK" Version="VERSION" />
  <PackageReference Include="Jroc.Runtime" Version="VERSION" />

  <JrocCompile Include="JavaScript\math.js" />
</ItemGroup>
```

`Jroc.SDK` compiles the JavaScript module before `ResolveAssemblyReferences`, adds the generated module assembly as a normal project reference, and makes generated exports contracts available to C# code in the same build.

## Quick start (typed)

When contracts are generated into the compiled module assembly (default), the easiest pattern is:

```csharp
using Jroc.Runtime;

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
using Jroc.Runtime;
using System.Reflection;

var asm = Assembly.LoadFrom("path\\to\\compiled.dll");

// Returns an IDisposable dynamic exports proxy.
using dynamic exports = JsEngine.LoadModule(asm, moduleId: "math");

Console.WriteLine((string)exports.version);
Console.WriteLine((double)exports.add(1, 2));
```

## Quick start (in memory)

```csharp
using Jroc;

using var module = JrocInMemoryCompiler.CompileAndLoadModule(
    new JrocInMemoryCompileRequest(@"C:\app\math.js")
    {
        SourceText = "exports.add = (left, right) => left + right;"
    });

dynamic exports = module.Exports;
Console.WriteLine((double)exports.add(1, 2));
```

## Tutorials

- [Getting started](tutorials/GettingStarted.md)
- [MSBuild build task](tutorials/MSBuildBuildTask.md)
- [In-memory compile-and-run](tutorials/InMemoryCompileAndRun.md)
- [Typed hosting](tutorials/TypedHosting.md)
- [Dynamic hosting](tutorials/DynamicHosting.md)
- [Async + event loop](tutorials/AsyncAndEventLoop.md)
- [Lifetime + disposal](tutorials/LifetimeAndDisposal.md)
- [Diagnostics + exceptions](tutorials/DiagnosticsAndExceptions.md)
- [Module ids + discovery](tutorials/ModuleIdsAndDiscovery.md)

## API reference

- [`JrocCompile` MSBuild task](api/JrocCompile.md)
- [In-memory compiler APIs](api/InMemoryCompiler.md)
- [`JsEngine`](api/JsEngine.md)
- [Handles + constructors](api/Handles.md)
- [Exceptions](api/Exceptions.md)
- [JS ↔ CLR type mapping](api/TypeMapping.md)

## Samples

The repo includes runnable `Jroc.SDK`-based samples:

- `samples/Basic`
- `samples/Typed`
- `samples/Domino`
- `samples/Picocolors`

## Validation and release smoke

- [SDK/NuGet package validation](PackagingValidation.md)
