# API: in-memory compilation

Namespace: `Jroc`; package: `Jroc.Core`.

Use these APIs for JavaScript selected or supplied at runtime. For build-time
scripts, use [`JrocCompile`](JrocCompile.md) and the generated assembly's
[`Import` / `Run` APIs](GeneratedFacades.md).

See [JavaScript to .NET type mapping](TypeMapping.md) for the values returned
through dynamic exports, `JsCallable`, and optional host-defined contracts.

## Request

Create a `JrocInMemoryCompileRequest` with a stable entry path:

```csharp
var request = new JrocInMemoryCompileRequest(
    Path.Combine(Path.GetTempPath(), "jroc-input", "math.js"))
{
    SourceText = "exports.add = (left, right) => left + right;",
    AssemblyName = "RuntimeMath",
    EmitPdb = true
};
```

Common request options:

| Property | Purpose |
|---|---|
| `EntryFilePath` | Required source identity, diagnostic path, and dependency-resolution context. |
| `SourceText` | Source supplied at runtime. If omitted, read the entry file. |
| `FileSystem` | Supply a file-system abstraction for source/dependency resolution. |
| `AssemblyName` | Assembly identity and artifact basename; defaults to the entry filename without its extension. |
| `RootModuleIdOverride` | Override the root module's logical ID. |
| `EmitPdb` | Include Portable PDB bytes with original source locations. |
| `Verbose` / `DiagnosticFilePath` | Enable verbose compiler diagnostics or capture diagnostics to a file. |
| `AnalyzeUnused` | Enable unused analysis diagnostics. |
| `AssumeUnmodifiedHostGlobals` | Opt into branch-free built-in global calls when source analysis proves their bindings unchanged and the host guarantees pristine bindings for every invocation. Defaults to `false`. |

When `SourceText` is present, it overlays the entry file at `EntryFilePath`;
other dependencies are resolved from the supplied file system or the default
file system. In-memory output does not mean all source dependencies are
automatically available in memory.

Global function and constructor calls normally check the realm's current
binding before taking an intrinsic fast path. `AssumeUnmodifiedHostGlobals`
removes that per-call check only for names not written or exposed by any
compiled entry or dependency. The opt-in is a host contract: the host must
start each invocation with the original built-ins, must not replace them or
expose the global object for mutation in callbacks, and must keep the same
guarantee for later exported calls. Do not enable it for modules that can
acquire the global object indirectly through an unanalyzed host value.
Known source mutations and explicit host-global replacements still take the
checked path. The option also applies to multi-entry compilation.
Strict identity comparisons (`===` and `!==`) of `this` or `globalThis`
do not by themselves expose the global object, so they preserve the
branch-free path when the host contract and other source analysis allow it.
Passing the object to a callback, storing an alias, or writing its
properties still prevents that optimization.

## Compile and evaluate

```csharp
using Jroc;

using var module = JrocInMemoryCompiler.CompileAndLoadModule(request);
Console.WriteLine(module.Exports.Invoke("add", 1d, 2d));
```

The non-generic overload returns `JrocInMemoryModule`. Its `Exports` property
is a `Jroc.Runtime.JsDynamicExports` value. The module owns the execution
runtime and the collectible assembly load context. It also exposes `Assembly`,
`AssemblyName`, `ModuleIds`, `EntryModuleId`, and `EntryModuleAliases` for
diagnostics.

`CompileAndLoadModule` selects the entry module by default. A caller that
compiled multiple modules can supply the overload's optional `moduleId`
argument to evaluate another published module.

### Export access

| Operation | Purpose |
|---|---|
| `module.Exports.Get("name")` | Read a named export. |
| `module.Exports.Invoke("name", args)` | Invoke a named exported function. |
| `module.Exports.Value` | Access the complete `module.exports` value. |
| `dynamic exports = module.Exports` | Use JavaScript member names via C# dynamic access, invocation, and assignment. |

Returned objects support dynamic member access. Function values are represented
by `JsCallable`; `Call(...)` invokes them and `CallAsync<T>(...)` bridges a
Promise result to a task. For example:

```csharp
using Jroc.Runtime;

var add = (JsCallable)module.Exports.Get("add")!;
Console.WriteLine(add.Call(1d, 2d));
```

These wrappers belong to the loaded module. Do not mix values from separate
module instances or use them after their owner is disposed.

### A host-defined contract for runtime scripts

If runtime-supplied scripts must implement an application-defined shape, the
generic overload accepts that host interface:

```csharp
using var module =
    JrocInMemoryCompiler.CompileAndLoadModule<IMathExports>(request);
Console.WriteLine(module.Exports.Add(1d, 2d));

public interface IMathExports : IDisposable
{
    double Add(double left, double right);
}
```

This is an optional contract for the in-memory workflow, not the
compiler-generated contract used by MSBuild consumers. The host is responsible
for choosing a shape that matches the runtime script. A generated type in the
new runtime assembly cannot itself be a compile-time type argument in the
already-built host.

## Compile to an artifact

`JrocInMemoryCompiler.Compile(request)` returns a
`JrocCompiledAssemblyArtifact` without loading or evaluating the script:

```csharp
var artifact = JrocInMemoryCompiler.Compile(request);
Console.WriteLine(artifact.PeBytes.Length);
Console.WriteLine(artifact.PdbBytes?.Length ?? 0);
```

The artifact contains `AssemblyName`, `PeBytes`, optional `PdbBytes`,
`ModuleIds`, `EntryModuleId`, `EntryModuleAliases`, and `EntryModules`. Use this form when the
application needs to inspect or persist runtime-generated compilation output
rather than execute it immediately. No output files are written implicitly.

### Multiple independent entries

Pass several `JrocInMemoryEntrySource` values to compile unrelated scripts in
one call. Their paths identify the source for diagnostics and relative module
resolution; they need not exist on disk when `SourceText` is provided.

```csharp
using Jroc;
using Jroc.Runtime;

var folder = Path.Combine(Path.GetTempPath(), "test-folder");
var artifact = JrocInMemoryCompiler.Compile(
    new JrocInMemoryMultiEntryCompileRequest(
    [
        new(Path.Combine(folder, "a.js"), "const value = 1; exports.value = value;"),
        new(Path.Combine(folder, "b.js"), "const value = 2; exports.value = value;")
    ])
    {
        AssemblyName = "TestFolder",
        DefaultEntryFilePath = Path.Combine(folder, "a.js"),
        EmitPdb = true
    });

using var loaded = JrocInMemoryAssemblyLoader.Load(artifact);
foreach (var entry in artifact.EntryModules)
{
    using var exports = JsEngine.LoadDynamicModule(loaded.Assembly, entry.ModuleId);
    Console.WriteLine(exports.Get("value"));
}
```

`EntryModules` maps the supplied source paths to their canonical module IDs in
input order. `ModuleIds` also contains dependencies and published aliases.
Use an entry ID to load its exports or select its generated `Scripts` facade
without running any other entry. Each module load creates its own runtime;
generated `Run` facade calls likewise start a fresh runtime. `EntryModuleId`
and the assembly's root `Run`/`Program.Main` identify the selected default
(the first entry unless `DefaultEntryFilePath` is specified). Without an
explicit `AssemblyName`, the default entry filename supplies the assembly
name. Optional `RootModuleIdOverride` on each entry publishes an additional
host-facing alias, without changing its canonical entry ID.

The multi-entry request accepts the same PDB, diagnostics, unused-analysis,
generated-contract, host-intrinsic, and shared `FileSystem` options as the
single-entry request. Inline source text overlays each matching path; entries
without text and dependencies are read through the shared file system.
Duplicate or ambiguous entry identities and errors in any supplied entry
fail the whole compilation rather than returning a partial artifact.

## Lifetime and errors

Dispose `JrocInMemoryModule` or `JrocInMemoryModule<TExports>` when finished.
Disposal shuts down the runtime and requests collectible-context unloading.
Actual collection requires releasing references to the module, exports,
assembly, and derived values; it is not guaranteed to happen immediately.

Failed compilation throws `InvalidOperationException` with compiler
diagnostics. Module evaluation and export invocation use the
[host-facing exception types](Exceptions.md).

## Path-dependent behavior

Stream-loaded assemblies have an empty `Assembly.Location`. If a runtime
script needs `child_process.fork(...)`, a launchable compiled assembly must
exist on disk. `JrocCompiledAssemblyArtifact.Materialize(...)` can create
the required artifacts; the load overload accepts
`Jroc.Runtime.JsModuleLoadOptions` with an explicit `CompiledAssemblyPath`.
The host must arrange that path and keep the materialized files available.
Pure source-text loading does not infer or create a child-process launch path.
