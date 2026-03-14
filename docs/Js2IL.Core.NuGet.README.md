# Js2IL.Core

`Js2IL.Core` is the referenceable NuGet package for the reusable js2il compiler library.

It ships the `Js2IL.Compiler.dll` assembly and compiler dependencies so custom .NET tools, build tasks, test harnesses, and hosts can compile JavaScript to .NET assemblies without shelling out to the `js2il` CLI tool.

The primary entry points are the existing `Js2IL.Compiler`, `Js2IL.CompilerOptions`, and `Js2IL.CompilerServices` types in the `Js2IL` namespace.

## Which package should I use?

- [`Js2IL.Core`](https://www.nuget.org/packages/Js2IL.Core)
  - Use this when you want to embed the compiler directly in your own .NET code.
- [`Js2IL.SDK`](https://www.nuget.org/packages/Js2IL.SDK)
  - Use this when your project should compile JavaScript during `dotnet build`.
- [`js2il`](https://www.nuget.org/packages/js2il)
  - Use this when you want the command-line tool for manual or ad-hoc compilation.
- [`Js2IL.Runtime`](https://www.nuget.org/packages/Js2IL.Runtime)
  - Use this when your host application needs the runtime support library used by generated assemblies.

Official releases publish `Js2IL.Runtime`, `js2il`, `Js2IL.Core`, and `Js2IL.SDK` together at the same version. Keep the versions aligned when you mix them in one workflow.

## Install

```xml
<ItemGroup>
  <PackageReference Include="Js2IL.Core" Version="VERSION" />
</ItemGroup>
```

## Basic usage

```csharp
using Js2IL;
using Microsoft.Extensions.DependencyInjection;

var options = new CompilerOptions
{
    OutputDirectory = @"C:\code\out",
    Verbose = true,
    EmitPdb = true
};

using var services = CompilerServices.BuildServiceProvider(options);
var compiler = services.GetRequiredService<Compiler>();

if (!compiler.Compile(@"C:\code\sample.js"))
{
    throw new InvalidOperationException("Compilation failed.");
}
```

`Compiler.Compile(...)` returns `true` on success and writes the generated files to `CompilerOptions.OutputDirectory`. If `OutputDirectory` is omitted, JS2IL writes next to the input file.

## What gets generated?

Given an input like `C:\code\sample.js`, JS2IL emits the following into the output directory:

- `sample.dll`
  - The compiled .NET assembly for your JavaScript.
- `sample.runtimeconfig.json`
  - Runtime configuration for the `dotnet` host.
- `JavaScriptRuntime.dll` (+ optional `JavaScriptRuntime.pdb`)
  - The runtime support library required to execute the generated assembly.

## Useful options

- `OutputDirectory`
  - Where generated files are written. If omitted, output is written next to the input file.
- `Verbose`
  - Enables compiler progress logging.
- `DiagnosticFilePath`
  - Writes compiler diagnostics to a text file.
- `AnalyzeUnused`
  - Reports unused functions, properties, and variables.
- `StrictMode`
  - Controls how missing `"use strict"` directive prologues are reported.
- `EmitPdb`
  - Emits Portable PDB symbols next to the generated assembly.
- `GenerateModuleExportContracts`
  - Emits typed CommonJS export contracts for .NET hosting scenarios.

## Links

- Hosting docs: https://github.com/tomacox74/js2il/blob/master/docs/hosting/Index.md
- Source, issues, docs: https://github.com/tomacox74/js2il
- License: Apache-2.0
