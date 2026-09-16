# Tutorial: diagnostics and exceptions

## Compilation diagnostics

Choose the diagnostic options for your workflow:

| Workflow | Options |
|---|---|
| CLI | `-v`, `--diagnostic-file <path>`, and `--pdb` |
| MSBuild | `Verbose`, `DiagnosticFilePath`, and `EmitPdb` on `JrocCompile`, or their corresponding `Jroc...` properties |
| In memory | `Verbose`, `DiagnosticFilePath`, and `EmitPdb` on `JrocInMemoryCompileRequest` |

Known CLI compilation failures return a nonzero exit code. MSBuild reports
compilation errors in the build. In-memory compilation throws
`InvalidOperationException` with captured diagnostics if compilation fails.

PDBs refer to the original source path. Keep source files and emitted PDBs
available when debugging on-disk assemblies; in-memory compilation can load
its PDB bytes with the generated PE.

## Generated import and invocation errors

For a module compiled by MSBuild as `ThrowsModule` with an exported `boom`
function:

```csharp
using Jroc.Runtime;

try
{
    using var exports = ThrowsModule.Import();
    exports.Boom();
}
catch (JsModuleLoadException ex)
{
    Console.WriteLine($"Initialization failed: module={ex.ModuleId}");
    Console.WriteLine(ex.InnerException);
}
catch (JsInvocationException ex)
{
    Console.WriteLine($"module={ex.ModuleId} member={ex.MemberName}");
    if (ex.InnerException is JsErrorException js)
    {
        Console.WriteLine($"{js.JsName}: {js.JsMessage}");
        Console.WriteLine(js.JsStack);
    }
}
```

These exception types are in the runtime dependency provided transitively by
`Jroc.SDK`. Catching them does not require replacing generated `Import()` calls
with runtime loaders. Contract projection failures indicate a missing or
incompatible export, such as an export mutated from a function into a value.

## Generated script execution

`Run(...)` throws `JsScriptRunException` if evaluation or drained async work
fails, or if JavaScript sets a nonzero process exit status:

```csharp
using Jroc.Runtime;

try
{
    ReportScript.Run();
}
catch (JsScriptRunException ex)
{
    Console.WriteLine($"script={ex.ModuleId} exit={ex.ExitCode}");
    Console.WriteLine(ex.InnerException);
}
```

## In-memory execution

Catch compilation failures around `JrocInMemoryCompiler.Compile(...)` or
`CompileAndLoadModule(...)`. After compilation, module evaluation and export
calls use the same hosting exception families as generated imports. Log the
module's `EntryModuleId` and the request's source path to associate failures
with runtime-supplied scripts.

See the [exception reference](../api/Exceptions.md) for details.
