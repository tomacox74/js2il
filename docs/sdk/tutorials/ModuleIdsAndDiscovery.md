# Tutorial: module IDs and generated facade names

An assembly name identifies the .NET output; a module ID identifies a
JavaScript module compiled into it. Consumers normally select a generated
facade, not a module ID string.

## MSBuild output names

```xml
<JrocCompile Include="JavaScript/math.js"
             AssemblyName="HostedMath"
             RootModuleId="calculator/math" />
```

This produces assembly `HostedMath.dll`. Its entry module can be imported
through either generated path:

```csharp
using var exports = HostedMath.Import();
// Alternative for explicitly selecting the published module:
// using var exports = HostedMath.Scripts.calculator.math.Import();
```

Use the root facade unless the host needs a specific published module.
`Run()` has equivalent root and nested entry points.

With no overrides, the source filename determines the assembly name and the
entry's normal module identity. Package entrypoints derive names from their
package ID. For example, `@mixmark-io/domino` has facade
`mixmark_io_domino`; use an explicit `AssemblyName` for a different root name.

Facade names are normalized to valid CLR identifiers. Ambiguous normalized
names fail compilation instead of silently selecting one module. See
[generated facade naming](../api/GeneratedFacades.md).

## In-memory source identity

The request's `EntryFilePath` supplies the source identity even when the
source comes from `SourceText`. Optional `AssemblyName` and
`RootModuleIdOverride` control output identity.

`CompileAndLoadModule(request)` evaluates the compiler-selected entry by
default. The returned module exposes `EntryModuleId`, `EntryModuleAliases`,
and `ModuleIds` for diagnostics. Artifact-only compilation exposes the same
identifiers on `JrocCompiledAssemblyArtifact`; no reflection-based type scan
is needed.
