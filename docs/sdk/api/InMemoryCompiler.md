# API: in-memory compiler

Namespace: `Jroc` (assembly / NuGet package: `Jroc.Core`)

The in-memory compiler APIs compile JavaScript without requiring the normal on-disk output path. They are intended for hosts that need source-text compilation, artifact storage, or short-lived compile-and-run workflows.

## `JrocInMemoryCompileRequest`

```csharp
public sealed record JrocInMemoryCompileRequest(string EntryFilePath)
{
    public string? SourceText { get; init; }
    public IFileSystem? FileSystem { get; init; }
    public string? RootModuleIdOverride { get; init; }
    public bool EmitPdb { get; init; }
    public bool Verbose { get; init; }
    public string? DiagnosticFilePath { get; init; }
    public bool AnalyzeUnused { get; init; }
    public bool GenerateModuleExportContracts { get; init; } = true;
}
```

`EntryFilePath` is still required because module ids, diagnostics, and PDB sequence points need a stable source identity. When `SourceText` is set, the compiler overlays that text at `EntryFilePath` and uses the supplied `FileSystem` only for any additional imports or requires.

## Compile to an artifact

```csharp
JrocCompiledAssemblyArtifact artifact =
    JrocInMemoryCompiler.Compile(
        new JrocInMemoryCompileRequest(@"C:\app\math.js")
        {
            SourceText = "exports.add = (left, right) => left + right;",
            EmitPdb = true
        });
```

`JrocCompiledAssemblyArtifact` contains:

| Property | Purpose |
| --- | --- |
| `AssemblyName` | Generated assembly name. |
| `PeBytes` | The compiled assembly PE image. |
| `PdbBytes` | Optional Portable PDB bytes when `EmitPdb` is enabled. |
| `ModuleIds` | Module ids embedded in the compiled assembly manifest. |

This API does not write the generated DLL, PDB, runtime config, or runtime assembly to disk.

## Compile and load

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

Typed hosts can request a generated or handwritten contract:

```csharp
using var module = JrocInMemoryCompiler.CompileAndLoadModule<IMathExports>(
    new JrocInMemoryCompileRequest(@"C:\app\math.js")
    {
        SourceText = "exports.add = (left, right) => left + right;"
    });

Console.WriteLine(module.Exports.Add(1, 2));
```

Both overload families load the generated PE/PDB bytes into a collectible `AssemblyLoadContext`, evaluate the selected module, and return a disposable module handle.

## Module handles

`JrocInMemoryModule` and `JrocInMemoryModule<TExports>` own both the hosted runtime and the collectible load context.

Dispose the module handle when the host is done with it:

```csharp
using var module = JrocInMemoryCompiler.CompileAndLoadModule(request);
```

After disposal, the exports proxy and loaded assembly boundary are closed. Once the host releases all references to the module, exports, loaded assembly, and any JS handles, the collectible load context can unload.

## Path-dependent behavior

Pure in-memory modules are stream-loaded, so `Assembly.Location` is empty and no generated output directory is created. Code that needs a launchable compiled assembly path cannot infer one from the in-memory assembly.

`child_process.fork(...)` is the main path-dependent case. If hosted code may call `fork(...)`, pass `JsModuleLoadOptions.CompiledAssemblyPath` explicitly and ensure that path points to a launchable compiled module assembly.

## Lower-level loading

`JrocInMemoryAssemblyLoader.Load(artifact)` loads an existing artifact into a collectible context without evaluating a module. Use it when the host wants to inspect or manage the loaded assembly before calling `JsEngine`.
