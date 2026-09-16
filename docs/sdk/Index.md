# Using JROC

JROC compiles JavaScript to .NET assemblies. There are three supported ways to
use it:

| Scenario | Start here | Entry point |
|---|---|---|
| Compile and run from a terminal | [Command-line quick start](tutorials/GettingStarted.md) | `jroc script.js out`, then `dotnet out/script.dll` |
| JavaScript is known when your .NET project is built | [MSBuild integration](tutorials/MSBuildBuildTask.md) | `Jroc.SDK` + `JrocCompile`, then generated `Import()` / `Run()` methods |
| JavaScript is supplied at runtime and is not known in advance | [In-memory compilation](tutorials/InMemoryCompileAndRun.md) | `JrocInMemoryCompiler.CompileAndLoadModule(...)` |

The examples use C#. Generated assemblies expose ordinary .NET types and can
also be consumed by other .NET languages that support those signatures.
JROC targets .NET 10 and ECMAScript 2025; see the
[compatibility documentation](../ECMA262/Index.md) for current limitations.

## 1. Compile and run on the command line

Install the .NET 10 SDK, then install the compiler:

```shell
dotnet tool install --global jroc
jroc hello.js out
dotnet out/hello.dll
```

No C# host project is needed. The generated program runs on .NET with the
runtime files JROC places alongside it.

See the [complete quick start](tutorials/GettingStarted.md) for a sample script,
output files, updates, and diagnostic options.

## 2. Compile during MSBuild and call generated APIs

Use this workflow when the JavaScript source or npm package is known at build
time. In a .NET project, add:

```xml
<ItemGroup>
  <PackageReference Include="Jroc.SDK" Version="VERSION" />
  <JrocCompile Include="JavaScript/math.js" AssemblyName="HostedMath" />
</ItemGroup>
```

Replace `VERSION` with a released JROC version. The SDK compiles the module
before the host language compiler runs and references the newly generated
assembly automatically. It restores and deploys the matching runtime
transitively; no global `jroc` installation or manually loaded assembly is
needed.

For `math.js` that exports `version` and `add`:

```csharp
using var exports = HostedMath.Import();
Console.WriteLine(exports.Version);
Console.WriteLine(exports.Add(1d, 2d));
```

The returned type is a strongly typed contract generated **in the compiled
JavaScript assembly**, not a handwritten host interface. To execute a script
rather than retain its exports:

```csharp
HostedMath.Run();
```

`Run()` owns a temporary runtime and drains asynchronous work before returning.
`Import()` keeps an isolated runtime alive until its returned contract is
disposed. Side-effect-only scripts expose `Run()` without `Import()`.

See [MSBuild integration](tutorials/MSBuildBuildTask.md),
[typed imports](tutorials/TypedHosting.md), and the
[generated API reference](api/GeneratedFacades.md).
The MSBuild tutorial also shows how to restore and compile an npm package,
then invoke it from C# through its generated `Import()` contract.

## 3. Compile source supplied at runtime in memory

Use `Jroc.Core` when the script is not known when the host is built:

```csharp
using Jroc;

// In an application, sourceText can come from a runtime-selected file or service.
var sourceText = "exports.add = (left, right) => left + right;";
var sourcePath = Path.Combine(Path.GetTempPath(), "jroc-input", "math.js");

using var module = JrocInMemoryCompiler.CompileAndLoadModule(
    new JrocInMemoryCompileRequest(sourcePath) { SourceText = sourceText });

dynamic exports = module.Exports;
Console.WriteLine((double)exports.add(1d, 2d));
```

The source path provides identity and dependency-resolution context; with
`SourceText` supplied, the entry file need not exist. Compilation and loading
do not write generated assemblies to disk. Because the assembly is created
after the host was built, its generated types cannot be referenced statically
by that host. Access exports dynamically or use `Get` / `Invoke` on
`module.Exports`.

See [in-memory compilation](tutorials/InMemoryCompileAndRun.md) for packages,
async calls, lifetime, and limitations. In-memory execution is not a sandbox:
only execute scripts you trust within the host process.

## Workflow references

- [Tutorial index](tutorials/Index.md)
- [API reference](api/Index.md)
- [Async calls and event-loop behavior](tutorials/AsyncAndEventLoop.md)
- [Lifetime and disposal](tutorials/LifetimeAndDisposal.md)
- [Diagnostics and exceptions](tutorials/DiagnosticsAndExceptions.md)

Runnable MSBuild examples are in [Basic](../../samples/Basic),
[Typed](../../samples/Typed), [Domino](../../samples/Domino), and
[Picocolors](../../samples/Picocolors).

For maintainers, see [building and releasing](../BuildingAndReleasing.md)
and [package validation](PackagingValidation.md). Compiler/runtime design
documents describe implementation details, not additional supported consumer
workflows.
