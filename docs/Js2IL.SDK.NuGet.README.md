# Js2IL.SDK

`Js2IL.SDK` is the consumer-facing NuGet package for compiling JavaScript sources during `dotnet build`.

It imports MSBuild props/targets, invokes `Js2IL.Core` in-process, and exposes the generated module assembly back to the host project as a normal MSBuild output.

## Which package should I use?

- [`Js2IL.SDK`](https://www.nuget.org/packages/Js2IL.SDK)
  - Use this when your project should compile JavaScript during `dotnet build`.
- [`js2il`](https://www.nuget.org/packages/js2il)
  - Use this when you want the standalone CLI/global tool for manual compilation.
- [`Js2IL.Core`](https://www.nuget.org/packages/Js2IL.Core)
  - Use this when you need the compiler as a reusable .NET library.
- [`Js2IL.Runtime`](https://www.nuget.org/packages/Js2IL.Runtime)
  - Use this when your host application needs the runtime support library used by compiled modules.

Official releases publish `Js2IL.Runtime`, `js2il`, `Js2IL.Core`, and `Js2IL.SDK` together at the same version. When validating a local feed for SDK consumers, also pack `Js2IL.Runtime` into that feed because host projects reference it directly.

## Install

```xml
<ItemGroup>
  <PackageReference Include="Js2IL.SDK" Version="VERSION" />
  <PackageReference Include="Js2IL.Runtime" Version="VERSION" />
</ItemGroup>
```

`Js2IL.Runtime` is the package your host references for `Js2IL.Runtime.JsEngine` and the runtime support assembly.

## Basic usage

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Js2IL.SDK" Version="VERSION" />
    <PackageReference Include="Js2IL.Runtime" Version="VERSION" />

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

## Bundled hosting samples

This package ships the repo hosting samples under `samples/` so the packaged samples exercise the same MSBuild/NuGet flow end users consume.

To validate a sample from the `.nupkg`:

```powershell
# Download the package (replace VERSION)
$version = "VERSION"
$url = "https://api.nuget.org/v3-flatcontainer/js2il.sdk/$version/js2il.sdk.$version.nupkg"
Invoke-WebRequest -Uri $url -OutFile "Js2IL.SDK.$version.nupkg"

# Extract it (a .nupkg is a zip; Expand-Archive expects a .zip extension)
Copy-Item "Js2IL.SDK.$version.nupkg" "Js2IL.SDK.$version.zip" -Force
Expand-Archive -Path "Js2IL.SDK.$version.zip" -DestinationPath "js2il_sdk_pkg" -Force

# Build + run a sample
dotnet build .\js2il_sdk_pkg\samples\Hosting.Basic\host
dotnet run --project .\js2il_sdk_pkg\samples\Hosting.Basic\host
```

If you are validating a locally packed prerelease feed instead of NuGet.org, pack `Js2IL.Runtime`, `Js2IL.Core`, and `Js2IL.SDK` into the same feed, then override `Js2ILPackageVersion` (or the individual `Js2ILSdkPackageVersion` / `Js2ILRuntimePackageVersion` properties) when building the sample. The legacy `JavaScriptRuntimePackageVersion` property is still accepted as an alias.

## Repo hosting sample pattern

- `samples\Hosting.Basic` and `samples\Hosting.Typed`
  - The host project references `Js2IL.SDK` directly and declares a `Js2ILCompile` item that points at `compiler\JavaScript\*.js`.
- `samples\Hosting.Domino`
  - The host project keeps `npm ci`, then compiles the restored entry file under `compiler\node_modules\@mixmark-io\domino\lib\index.js` with `RootModuleId="@mixmark-io/domino"` so `JsEngine.LoadModule(..., "@mixmark-io/domino")` continues to work.

## Links

- Hosting docs: https://github.com/tomacox74/js2il/blob/master/docs/hosting/Index.md
- Source, issues, docs: https://github.com/tomacox74/js2il
- License: Apache-2.0
