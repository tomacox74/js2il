# Jroc.Runtime

[`Jroc.Runtime`](https://www.nuget.org/packages/Jroc.Runtime) provides
`JavaScriptRuntime.dll`, the execution support library
for JROC-generated assemblies. It is a dependency of the supported workflows,
not a separate assembly-loading workflow.

## Choose your workflow

- [`jroc`](https://www.nuget.org/packages/jroc): compile and run JavaScript on
  the command line. The compiler places the runtime beside generated output.
- [`Jroc.SDK`](https://www.nuget.org/packages/Jroc.SDK): compile known scripts
  during MSBuild and call generated `Import` / `Run` APIs. The SDK restores and
  deploys the runtime transitively.
- [`Jroc.Core`](https://www.nuget.org/packages/Jroc.Core): compile source not
  known until runtime using `JrocInMemoryCompiler`.

Official releases publish `Jroc.Runtime`, `jroc`, `Jroc.Core`, and `Jroc.SDK`
together at the same version. Keep their versions aligned.

## In-memory execution dependency

A host compiling and executing runtime-supplied scripts can reference:

```xml
<ItemGroup>
  <PackageReference Include="Jroc.Core" Version="VERSION" />
  <PackageReference Include="Jroc.Runtime" Version="VERSION" />
</ItemGroup>
```

Use `JrocInMemoryCompiler.CompileAndLoadModule(...)` to obtain the live module.
Its exports provide dynamic access or explicit `Get` / `Invoke` operations
after casting the non-generic module's `object`-typed `Exports` property to
`JsDynamicExports`.
The `Jroc.Runtime` namespace includes the callable and exception types used
at this boundary.

For example, in a console project with the references above:

```csharp
using Jroc;
using Jroc.Runtime;

var path = Path.Combine(Path.GetTempPath(), "jroc-input", "answer.js");
using var module = JrocInMemoryCompiler.CompileAndLoadModule(
    new JrocInMemoryCompileRequest(path) { SourceText = "exports.answer = 42;" });

var exports = (JsDynamicExports)module.Exports;
Console.WriteLine(exports.Get("answer"));
```

This prints `42`. The source path need not exist when `SourceText` is supplied.
Dispose the module when finished; in-memory execution is not a sandbox, so
only run trusted scripts in the host process.

For MSBuild-generated imports, consume the types generated in the compiled
JavaScript assembly. A normal `Jroc.SDK` host does not need a direct runtime
package reference or manual runtime loading.

## .NET compatibility

The runtime library targets `net10.0` and can be used by .NET 10 and .NET 11
hosts. Generated assemblies also carry their own framework requirements:
in-memory compilation follows the executing .NET runtime, while `Jroc.SDK`
follows the consuming project's target framework. The CLI defaults to the
executing runtime, but `--target-framework` with a matching reference pack
can select `net10.0` or `net11.0`. Installing this runtime package does not
make output targeting .NET 11 executable on .NET 10.

For supported JavaScript features and current limitations, see the
[ECMAScript support index](https://github.com/tomacox74/js2il/blob/master/docs/ECMA262/Index.md).

## Links

- SDK docs: https://github.com/tomacox74/js2il/blob/master/docs/sdk/Index.md
- Migrating from JS2IL: https://github.com/tomacox74/js2il/blob/master/docs/sdk/LegacyPackageMigration.md
- Release history: https://github.com/tomacox74/js2il/blob/master/CHANGELOG.md
- Source: https://github.com/tomacox74/js2il
- Issues: https://github.com/tomacox74/js2il/issues
- License: Apache-2.0
