# Tutorial: compile runtime-supplied scripts in memory

Use this workflow when the JavaScript is not known when your .NET application
is built: for example, an application selects a script file or receives source
text at runtime. For scripts known in advance, prefer
[MSBuild and generated typed APIs](MSBuildBuildTask.md).

In-memory compilation does not provide a security sandbox. Execute only code
you trust within the host process.

## 1. Reference packages

In a .NET 10 console project:

```xml
<ItemGroup>
  <PackageReference Include="Jroc.Core" Version="VERSION" />
  <PackageReference Include="Jroc.Runtime" Version="VERSION" />
</ItemGroup>
```

Replace `VERSION` with the same released version for both packages.
`Jroc.Core` provides `JrocInMemoryCompiler`; `Jroc.Runtime` provides execution
support and the export/callable types used below. This workflow does not
require `JrocCompile` items or a global `jroc` installation.

## 2. Compile and evaluate source text

```csharp
using Jroc;

// Substitute the source selected by your application at runtime.
var sourceText = """
    exports.version = "1.0.0";
    exports.add = (left, right) => left + right;
    """;
var sourcePath = Path.Combine(Path.GetTempPath(), "jroc-input", "math.js");

var request = new JrocInMemoryCompileRequest(sourcePath)
{
    SourceText = sourceText
};

using var module = JrocInMemoryCompiler.CompileAndLoadModule(request);

dynamic exports = module.Exports;
Console.WriteLine((string)exports.version);
Console.WriteLine((double)exports.add(1d, 2d));
```

Expected output is `1.0.0` followed by `3`, on separate lines.

`EntryFilePath` gives source text an identity for module IDs, diagnostics,
relative dependencies, and optional debug symbols. The entry file need not
exist when `SourceText` is set. Choose a path in the application's intended
module-resolution directory if the script imports other files or packages;
additional dependencies still need to be available.

`CompileAndLoadModule` compiles the assembly into memory, loads it in a
collectible context, and evaluates the entry module. It does not create
generated DLL, PDB, or runtime-config files. A side-effect-only script can be
evaluated the same way; there are simply no exports for the host to call.

## 3. Access exports without C# `dynamic`

The non-generic module's `Exports` property also provides explicit operations:

```csharp
Console.WriteLine(module.Exports.Get("version"));
Console.WriteLine(module.Exports.Invoke("add", 1d, 2d));
```

Use `module.Exports.Value` for the complete `module.exports` value, including
modules that export a function or primitive directly. Member names in this
workflow use their JavaScript spelling, such as `add`, not generated
PascalCase names.

The host cannot statically reference types in an assembly that does not exist
until runtime. This is why dynamic access or `Get` / `Invoke` is appropriate
here, while MSBuild consumers use generated contracts.

## 4. Await an exported asynchronous function

For source such as `exports.addAsync = async (x, y) => x + y`, retrieve the
exported callable and bridge its result to a .NET task:

```csharp
using Jroc.Runtime;

var addAsync = (JsCallable)module.Exports.Get("addAsync")!;
var result = await addAsync.CallAsync<double>(1d, 2d);
Console.WriteLine(result);
```

This snippet assumes the source passed to compilation exports `addAsync`.
Keep `module` alive while awaiting the task. Promise rejection faults the task;
disposing the owning module also faults pending Promise bridges.

## 5. Read a runtime-selected file instead

Omit `SourceText` to read the entry source from disk:

```csharp
var selectedPath = Path.GetFullPath(args[0]);
using var module = JrocInMemoryCompiler.CompileAndLoadModule(
    new JrocInMemoryCompileRequest(selectedPath));
```

The input is file-backed, but generated assemblies are still emitted and
loaded in memory. Validate application arguments before using this fragment.

## Lifetime and limitations

Always dispose the returned module. It owns both the hosted runtime and the
collectible load context; retain it for as long as exports or asynchronous
results are needed. Release references to exports, the loaded assembly, and
derived values after disposal so the load context can be collected.

Unlike a generated `Run()` call, this API returns a live module for continued
interaction; do not assume a short `using` scope waits for arbitrary timers
or background work. Await the exported work your application needs.

The loaded assembly has no physical `Assembly.Location`. Code that needs a
launchable DLL, such as `child_process.fork(...)`, requires explicit
materialization and launch-path configuration; it is not automatic in this
workflow. See [in-memory API details](../api/InMemoryCompiler.md).
