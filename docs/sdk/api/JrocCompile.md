# API: `JrocCompile` MSBuild task

Package: `Jroc.SDK`

`Jroc.SDK` imports MSBuild props and targets that compile JavaScript sources during `dotnet build`. Consumers do not call the task directly; they declare `JrocCompile` items and configure them with item metadata or matching project properties.

## Basic item

```xml
<ItemGroup>
  <PackageReference Include="Jroc.SDK" Version="VERSION" />
  <PackageReference Include="Jroc.Runtime" Version="VERSION" />

  <JrocCompile Include="JavaScript\math.js" />
</ItemGroup>
```

The target runs before `ResolveAssemblyReferences`, writes generated files under `$(IntermediateOutputPath)\jroc\<SourceFileName>\` by default, and adds the generated module assembly to `@(Reference)` when `ReferenceOutputAssembly` is enabled.

## Item metadata and default properties

| Metadata | Default property | Default | Purpose |
| --- | --- | --- | --- |
| `OutputDirectory` | `JrocOutputRoot` | `$(IntermediateOutputPath)\jroc` | Output root for generated DLL, PDB, runtime config, and runtime assembly files. |
| `ModuleResolutionBaseDirectory` | `JrocModuleResolutionBaseDirectory` | `$(MSBuildProjectDirectory)` | Base directory for resolving package/module-id entrypoints. |
| `RootModuleId` | `JrocRootModuleId` | empty | Overrides the root module id embedded in the compiled assembly. |
| `ReferenceOutputAssembly` | `JrocReferenceOutputAssembly` | `true` | Adds the generated module assembly as a normal project reference. |
| `CopyToOutputDirectory` | `JrocCopyToOutputDirectory` | `false` | Copies generated outputs to `$(TargetDir)` after build. |
| `EmitPdb` | `JrocEmitPdb` | `true` in Debug, otherwise `false` | Emits a Portable PDB for the compiled module. |
| `AnalyzeUnused` | `JrocAnalyzeUnused` | `false` | Enables unused analysis diagnostics. |
| `Verbose` | `JrocVerbose` | `false` | Enables verbose compiler output. |
| `DiagnosticFilePath` | `JrocDiagnosticFilePath` | empty | Writes compiler diagnostics to the specified file. |

Per-item metadata wins over the matching project property.

## Package/module-id entrypoints

`Include` can be a JavaScript file path or a package/module id. If the item does not resolve to a file and looks like a module id, `Jroc.SDK` resolves it with Node-style module resolution from `ModuleResolutionBaseDirectory`.

```xml
<ItemGroup>
  <JrocCompile Include="@mixmark-io/domino"
               CopyToOutputDirectory="true" />
</ItemGroup>
```

When `RootModuleId` is not set for a package-style entrypoint, the SDK preserves the item specifier as the root module id.

## Outputs

After the target runs, MSBuild exposes:

| Item | Contents |
| --- | --- |
| `@(JrocCompiledAssembly)` | The generated module assembly. Metadata includes `SourcePath`, `OutputDirectory`, `AssemblyName`, `RuntimeConfigPath`, `RuntimeAssemblyPath`, `PdbPath`, and `RootModuleId`. |
| `@(JrocGeneratedOutput)` | Every generated file. Metadata includes `Kind` (`Assembly`, `AssemblyPdb`, `RuntimeConfig`, `RuntimeAssembly`, or `RuntimePdb`). |

## Targets

| Target | Behavior |
| --- | --- |
| `JrocCompile` | Resolves `JrocCompile` items, invokes the in-process compiler task, and adds generated assemblies as references. |
| `CopyJrocOutputsToOutputDirectory` | Copies generated outputs with `CopyToOutputDirectory="true"` after `Build`. |
| `CleanJrocOutputs` | Removes generated output directories after `Clean`. |

Set `JrocCompileOnBuild` to `false` to disable the automatic build target.
