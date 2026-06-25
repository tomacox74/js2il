# Tutorial: in-memory compile-and-run

Use the in-memory APIs when a host needs to compile JavaScript source text or file-backed modules without writing generated assemblies to disk.

## 1) Reference packages

```xml
<ItemGroup>
  <PackageReference Include="Jroc.Core" Version="VERSION" />
  <PackageReference Include="Jroc.Runtime" Version="VERSION" />
</ItemGroup>
```

`Jroc.Core` provides `JrocInMemoryCompiler`. `Jroc.Runtime` provides the hosting runtime used after the generated assembly is loaded.

## 2) Compile and load source text

```csharp
using Jroc;

var request = new JrocInMemoryCompileRequest(@"C:\virtual\math.js")
{
    SourceText = """
        exports.version = "1.0.0";
        exports.add = (left, right) => left + right;
        """
};

using var module = JrocInMemoryCompiler.CompileAndLoadModule(request);

dynamic exports = module.Exports;
Console.WriteLine((string)exports.version);
Console.WriteLine((double)exports.add(1, 2));
```

`EntryFilePath` gives the source a stable identity for module ids, diagnostics, and optional PDB sequence points. Because `SourceText` is set, the generated assembly is emitted and loaded from memory.

## 3) Compile and load from files

```csharp
using var module = JrocInMemoryCompiler.CompileAndLoadModule(
    new JrocInMemoryCompileRequest(@"C:\app\index.js"));
```

When `SourceText` is not set, JROC reads the entry file and any imported modules from the file system.

## 4) Compile to an artifact only

```csharp
var artifact = JrocInMemoryCompiler.Compile(
    new JrocInMemoryCompileRequest(@"C:\virtual\math.js")
    {
        SourceText = "exports.answer = 42;",
        EmitPdb = true
    });

Console.WriteLine(artifact.AssemblyName);
Console.WriteLine(artifact.PeBytes.Length);
Console.WriteLine(artifact.PdbBytes?.Length ?? 0);
```

Use artifact-only compilation when the host wants to cache, inspect, or persist PE/PDB bytes itself.

## 5) Dispose the module

`CompileAndLoadModule(...)` returns a disposable module handle that owns the hosted runtime and the collectible load context:

```csharp
using var module = JrocInMemoryCompiler.CompileAndLoadModule(request);
```

Dispose the handle when the host is done with it. After disposal, release references to the module, exports proxy, loaded assembly, and any JS handles so the collectible load context can unload.

## Path-dependent limitations

Pure in-memory modules do not have a physical generated assembly path:

- `module.Assembly.Location` is empty.
- JROC does not implicitly write generated DLL/PDB/runtimeconfig files.
- Hosted `child_process.fork(...)` requires `JsModuleLoadOptions.CompiledAssemblyPath` if the JavaScript code needs to launch a child process.

For hosts that need `fork(...)`, compile or provide a launchable assembly path and pass it explicitly through `JsModuleLoadOptions`.
