# Jroc.Core

`Jroc.Core` provides in-memory JavaScript compilation for .NET applications
whose scripts are not known at build time. It ships `Jroc.Compiler.dll` and
the compiler dependencies.

The consumer entry point is `Jroc.JrocInMemoryCompiler`: compile runtime source
to PE/PDB bytes, or compile and evaluate it in a disposable in-memory module.

## Choose your workflow

- [`jroc`](https://www.nuget.org/packages/jroc): compile and run from the command line.
- [`Jroc.SDK`](https://www.nuget.org/packages/Jroc.SDK): compile known JavaScript
  during MSBuild, then call the generated assembly's typed `Import` / `Run` APIs.
- [`Jroc.Core`](https://www.nuget.org/packages/Jroc.Core): compile scripts supplied
  at runtime in memory.

[`Jroc.Runtime`](https://www.nuget.org/packages/Jroc.Runtime) supplies execution
support. Official releases publish all four packages together; keep versions
aligned when using them in one application.

## Install

For a .NET 10 application that compiles and executes runtime-supplied scripts:

```xml
<ItemGroup>
  <PackageReference Include="Jroc.Core" Version="VERSION" />
  <PackageReference Include="Jroc.Runtime" Version="VERSION" />
</ItemGroup>
```

Replace `VERSION` with the same released version for both packages.

## Compile and execute in memory

```csharp
using Jroc;
using Jroc.Runtime;

// Replace with source selected by the application at runtime.
var sourceText = "exports.add = (left, right) => left + right;";
var path = Path.Combine(Path.GetTempPath(), "jroc-input", "math.js");

using var module = JrocInMemoryCompiler.CompileAndLoadModule(
    new JrocInMemoryCompileRequest(path) { SourceText = sourceText });

var exports = (JsDynamicExports)module.Exports;
Console.WriteLine(exports.Invoke("add", 1d, 2d));
```

The entry path supplies identity and dependency-resolution context. It need
not exist when `SourceText` is provided. Omitting `SourceText` reads an
existing entry file while still emitting and loading the generated assembly
in memory.

The non-generic `module.Exports` property is typed as `object`; cast it to
`JsDynamicExports` for `Get`, `Invoke`, and `Value`, as above, or assign it to
a C# `dynamic` variable for JavaScript-style member access.
Dispose the module when finished; it owns the execution runtime and
collectible load context. Keep it alive until any required async work settles.
No generated output files are written implicitly.

In-memory execution is not a sandbox. Only execute trusted scripts in the
host process.

## Artifact-only compilation

Use `JrocInMemoryCompiler.Compile(request)` when the application needs
`JrocCompiledAssemblyArtifact.PeBytes` and optional `PdbBytes` without
evaluating the script. Set `EmitPdb` on the request for debug symbols,
`AssemblyName` for a custom identity, and `DiagnosticFilePath` to capture
compiler diagnostics.

For JavaScript known at build time, prefer `Jroc.SDK` and generated typed
contracts rather than embedding compiler services in application code.

## .NET compatibility

The compiler library targets `net10.0`; .NET 10 and .NET 11 hosts can reference
it. In-memory compilation uses the runtime executing the host, so artifacts
compiled in a .NET 11 process require .NET 11. A `net10.0` package does not
mean that every generated assembly targets .NET 10.

For build-time compilation that follows a project's target framework, use
`Jroc.SDK`. JavaScript compatibility is still evolving; see the
[ECMAScript support index](https://github.com/tomacox74/js2il/blob/master/docs/ECMA262/Index.md).
`eval` is not currently supported.

## Links

- SDK docs: https://github.com/tomacox74/js2il/blob/master/docs/sdk/Index.md
- In-memory tutorial: https://github.com/tomacox74/js2il/blob/master/docs/sdk/tutorials/InMemoryCompileAndRun.md
- Migrating from JS2IL: https://github.com/tomacox74/js2il/blob/master/docs/sdk/LegacyPackageMigration.md
- Release history: https://github.com/tomacox74/js2il/blob/master/CHANGELOG.md
- Source: https://github.com/tomacox74/js2il
- Issues: https://github.com/tomacox74/js2il/issues
- License: Apache-2.0
