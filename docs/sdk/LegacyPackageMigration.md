# Migrating from JS2IL packages

JROC is the current package family for this project. The repository remains
[`tomacox74/js2il`](https://github.com/tomacox74/js2il); the project rename did
not move it to a `tomacox74/jroc` repository.

The legacy packages remain available for existing consumers. This guide does
not unlist, unpublish, or alter them.

## Package mapping

| Legacy package | Current package | Workflow |
|---|---|---|
| `js2il` | [`jroc`](https://www.nuget.org/packages/jroc) | Command-line compilation |
| `Js2IL.Core` | [`Jroc.Core`](https://www.nuget.org/packages/Jroc.Core) | In-memory compilation of runtime-supplied scripts |
| `Js2IL.Runtime` | [`Jroc.Runtime`](https://www.nuget.org/packages/Jroc.Runtime) | Execution support |
| `Js2IL.SDK` | [`Jroc.SDK`](https://www.nuget.org/packages/Jroc.SDK) | Compile during MSBuild |

Choose one released JROC version and keep all JROC package versions aligned.
The legacy `0.9.x` and current `0.12.x` version lines are not interchangeable.

## Command-line users

For a global tool installation:

```shell
dotnet tool uninstall --global js2il
dotnet tool install --global jroc
jroc hello.js out
dotnet out/hello.dll
```

For a local tool manifest, use `dotnet tool uninstall js2il` and
`dotnet tool install jroc` in the manifest's directory, then
`dotnet tool run jroc -- hello.js out`.

Recompile JavaScript rather than replacing the runtime DLL beside older
generated assemblies. See the [quick start](tutorials/GettingStarted.md) for
the input example and the executing-runtime requirements.

## .NET projects

Replace legacy package references, and any corresponding central
`PackageVersion` entries, with the chosen JROC packages. Remove obsolete
generated assembly references and rebuild from JavaScript sources.

For build-time scripts, reference `Jroc.SDK` and replace `Js2ILCompile` items
with `JrocCompile`. Review any `Js2IL...` MSBuild properties against the
[current metadata/property reference](../Jroc.SDK.NuGet.README.md).
The current SDK supplies the runtime transitively; a normal host needs only
the `Jroc.SDK` package reference.

Consume the generated assembly's typed `Import()` / `Run()` APIs rather than
copying historical `JsEngine` or assembly-loading examples. Follow the
[MSBuild tutorial](tutorials/MSBuildBuildTask.md) for a complete host.

For runtime-supplied scripts, use `Jroc.Core` and
`Jroc.JrocInMemoryCompiler.CompileAndLoadModule(...)`, following the
[in-memory tutorial](tutorials/InMemoryCompileAndRun.md).
This is not a promise of source or binary compatibility with the old compiler
entry points. Review namespace, API, and assembly-identity changes; do not
perform a blind text replacement or mix old and new runtime assemblies.

The current library packages target .NET 10. A .NET 11 SDK can build a
`net10.0` SDK consumer without retargeting the host; see
[framework compatibility](../Jroc.SDK.NuGet.README.md#net-compatibility).

## Maintainer-managed gallery messaging

The legacy `0.9.27` gallery pages still describe the JS2IL package family.
Recommended next step: mark maintained-by-replacement versions as **Legacy**
in NuGet's package-management UI and select each corresponding JROC package
as the alternative. Include this guide in the gallery message.

[Deprecation](https://learn.microsoft.com/en-us/nuget/nuget-org/deprecate-packages)
is distinct from unlisting: it informs consumers without preventing existing
restores. Changing a repository README cannot update the immutable README
inside an already published package. Gallery/account changes require a
separate maintainer action; none were made as part of this audit.
