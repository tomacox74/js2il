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
Its exports provide dynamic access or explicit `Get` / `Invoke` operations.
The `Jroc.Runtime` namespace includes the callable and exception types used
at this boundary.

For MSBuild-generated imports, consume the types generated in the compiled
JavaScript assembly. A normal `Jroc.SDK` host does not need a direct runtime
package reference or manual runtime loading.

## Links

- SDK docs: https://github.com/tomacox74/jroc/blob/master/docs/sdk/Index.md
- Source, issues, docs: https://github.com/tomacox74/jroc
- License: Apache-2.0
