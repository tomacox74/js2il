# Js2IL.SDK

`Js2IL.SDK` is the consumer-facing NuGet package for compiling JavaScript sources during `dotnet build`.

It imports MSBuild props/targets, invokes `Js2IL.Core` in-process, and exposes the generated module assembly back to the host project as a normal MSBuild output.

## Install

```xml
<ItemGroup>
  <PackageReference Include="Js2IL.SDK" Version="VERSION" />
  <PackageReference Include="JavaScriptRuntime" Version="VERSION" />
</ItemGroup>
```

`JavaScriptRuntime` is still the package your host references for `Js2IL.Runtime.JsEngine` and the runtime support assembly.

## Basic usage

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Js2IL.SDK" Version="VERSION" />
    <PackageReference Include="JavaScriptRuntime" Version="VERSION" />

    <Js2ILCompile Include="JavaScript\HostedMathModule.js" />
  </ItemGroup>
</Project>
```

By default:

- `Js2ILCompile` items are compiled before `ResolveAssemblyReferences`
- the generated module assembly is added as a normal `Reference`
- generated files are written under `$(IntermediateOutputPath)\js2il\<SourceFileName>\`

That means a host project can compile against the generated exports contracts with no extra custom `.proj` file or CLI invocation.

## `Js2ILCompile` metadata and matching properties

You can set metadata per item or apply defaults with properties:

- `OutputDirectory` / `Js2ILOutputRoot`
  - Where generated files are written.
- `RootModuleId` / `Js2ILRootModuleId`
  - Optional module id override for the root entry module. This is the key migration knob for package-style inputs such as `@mixmark-io/domino`.
- `ReferenceOutputAssembly` / `Js2ILReferenceOutputAssembly`
  - Defaults to `true`. When enabled, the generated module assembly is added to `Reference` before assembly resolution.
- `CopyToOutputDirectory` / `Js2ILCopyToOutputDirectory`
  - Defaults to `false`. When enabled, the generated outputs are copied into `$(TargetDir)` after build.
- `EmitPdb` / `Js2ILEmitPdb`
  - Defaults to `true` for Debug builds and `false` otherwise.
- `Verbose` / `Js2ILVerbose`
- `AnalyzeUnused` / `Js2ILAnalyzeUnused`
- `StrictMode` / `Js2ILStrictMode`
  - `Error`, `Warn`, or `Ignore`
- `DiagnosticFilePath` / `Js2ILDiagnosticFilePath`

## MSBuild outputs

After `Js2ILCompile` runs, the package populates:

- `@(Js2ILCompiledAssembly)`
  - ItemSpec is the generated `.dll`
  - Metadata includes `SourcePath`, `OutputDirectory`, `AssemblyName`, `RuntimeConfigPath`, `RuntimeAssemblyPath`, `PdbPath`, and `RootModuleId`
- `@(Js2ILGeneratedOutput)`
  - ItemSpec is every generated file
  - Metadata includes `Kind` (`Assembly`, `AssemblyPdb`, `RuntimeConfig`, `RuntimeAssembly`, `RuntimePdb`)

## Migration notes for the existing hosting samples

- `samples\Hosting.Basic` and `samples\Hosting.Typed`
  - Replace the separate `compiler\*.proj` file and manual `Reference HintPath=...` wiring with a `PackageReference` to `Js2IL.SDK` plus a `Js2ILCompile` item in the host project.
- `samples\Hosting.Domino`
  - Keep `npm ci`, then point `Js2ILCompile` at the restored entry file under `node_modules` and set `RootModuleId="@mixmark-io/domino"` so `JsEngine.LoadModule(..., "@mixmark-io/domino")` continues to work.
