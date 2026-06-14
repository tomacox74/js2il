# Jroc.SDK

`Jroc.SDK` is the consumer-facing NuGet package for compiling JavaScript sources during `dotnet build`.

It imports MSBuild props/targets, invokes `Jroc.Core` in-process, and exposes the generated module assembly back to the host project as a normal MSBuild output.

## Which package should I use?

- [`Jroc.SDK`](https://www.nuget.org/packages/Jroc.SDK)
  - Use this when your project should compile JavaScript during `dotnet build`.
- [`jroc`](https://www.nuget.org/packages/jroc)
  - Use this when you want the standalone CLI/global tool for manual compilation.
- [`Jroc.Core`](https://www.nuget.org/packages/Jroc.Core)
  - Use this when you need the compiler as a reusable .NET library.
- [`Jroc.Runtime`](https://www.nuget.org/packages/Jroc.Runtime)
  - Use this when your host application needs the runtime support library used by compiled modules.

Official releases publish `Jroc.Runtime`, `jroc`, `Jroc.Core`, and `Jroc.SDK` together at the same version. When validating a local feed for SDK consumers, also pack `Jroc.Runtime` into that feed because host projects reference it directly.

## Install

```xml
<ItemGroup>
  <PackageReference Include="Jroc.SDK" Version="VERSION" />
  <PackageReference Include="Jroc.Runtime" Version="VERSION" />
</ItemGroup>
```

`Jroc.Runtime` is the package your host references for `Jroc.Runtime.JsEngine` and the runtime support assembly.

## Basic usage

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Jroc.SDK" Version="VERSION" />
    <PackageReference Include="Jroc.Runtime" Version="VERSION" />

    <JrocCompile Include="JavaScript\HostedMathModule.js" />
  </ItemGroup>
</Project>
```

By default:

- `JrocCompile` items are compiled before `ResolveAssemblyReferences`
- the generated module assembly is added as a normal `Reference`
- generated files are written under `$(IntermediateOutputPath)\jroc\<SourceFileName>\`

That means a host project can compile against the generated exports contracts with no extra custom `.proj` file or CLI invocation.

Package/module-id entrypoints work too. For example, if the host project restores the npm package in its own directory:

```xml
<JrocCompile Include="@mixmark-io/domino"
              CopyToOutputDirectory="true" />
```

When `Include` is a module id and `RootModuleId` is not set, the SDK preserves that module id as the root module id automatically, matching the CLI `--moduleid` behavior.

## `JrocCompile` metadata and matching properties

You can set metadata per item or apply defaults with properties:

- `OutputDirectory` / `JrocOutputRoot`
  - Where generated files are written.
- `ModuleResolutionBaseDirectory` / `JrocModuleResolutionBaseDirectory`
  - Base directory used when `Include` is a package/module id instead of a file path. Defaults to `$(MSBuildProjectDirectory)`. Set the project property once for the common case, or use per-item metadata only when a specific item needs an override.
- `RootModuleId` / `JrocRootModuleId`
  - Optional module id override for the root entry module. For package-style inputs such as `@mixmark-io/domino`, you usually do not need to set this because the SDK will use the module id automatically.
- `ReferenceOutputAssembly` / `JrocReferenceOutputAssembly`
  - Defaults to `true`. When enabled, the generated module assembly is added to `Reference` before assembly resolution.
- `CopyToOutputDirectory` / `JrocCopyToOutputDirectory`
  - Defaults to `false`. When enabled, the generated outputs are copied into `$(TargetDir)` after build.
- `EmitPdb` / `JrocEmitPdb`
  - Defaults to `true` for Debug builds and `false` otherwise.
- `Verbose` / `JrocVerbose`
- `AnalyzeUnused` / `JrocAnalyzeUnused`
- `DiagnosticFilePath` / `JrocDiagnosticFilePath`

## MSBuild outputs

After `JrocCompile` runs, the package populates:

- `@(JrocCompiledAssembly)`
  - ItemSpec is the generated `.dll`
  - Metadata includes `SourcePath`, `OutputDirectory`, `AssemblyName`, `RuntimeConfigPath`, `RuntimeAssemblyPath`, `PdbPath`, and `RootModuleId`
- `@(JrocGeneratedOutput)`
  - ItemSpec is every generated file
  - Metadata includes `Kind` (`Assembly`, `AssemblyPdb`, `RuntimeConfig`, `RuntimeAssembly`, `RuntimePdb`)

## Bundled hosting samples

This package ships the repo hosting samples under `samples/` so the packaged samples exercise the same MSBuild/NuGet flow end users consume.

To validate a sample from the `.nupkg`:

```powershell
# Download the package (replace VERSION)
$version = "VERSION"
$url = "https://api.nuget.org/v3-flatcontainer/jroc.sdk/$version/jroc.sdk.$version.nupkg"
Invoke-WebRequest -Uri $url -OutFile "Jroc.SDK.$version.nupkg"

# Extract it (a .nupkg is a zip; Expand-Archive expects a .zip extension)
Copy-Item "Jroc.SDK.$version.nupkg" "Jroc.SDK.$version.zip" -Force
Expand-Archive -Path "Jroc.SDK.$version.zip" -DestinationPath "jroc_sdk_pkg" -Force

# Build + run a sample
dotnet build .\jroc_sdk_pkg\samples\Hosting.Basic\host
dotnet run --project .\jroc_sdk_pkg\samples\Hosting.Basic\host
```

If you are validating a locally packed prerelease feed instead of NuGet.org, pack `Jroc.Runtime`, `Jroc.Core`, and `Jroc.SDK` into the same feed, then override `JrocPackageVersion` (or the individual `JrocSdkPackageVersion` / `JrocRuntimePackageVersion` properties) when building the sample. The legacy `JavaScriptRuntimePackageVersion` property is still accepted as an alias.

## Repo hosting sample pattern

- `samples\Hosting.Basic` and `samples\Hosting.Typed`
  - The host project references `Jroc.SDK` directly and declares a `JrocCompile` item that points at `compiler\JavaScript\*.js`.
- `samples\Hosting.Domino`
  - The host project keeps `package.json` / `package-lock.json` next to the `.csproj`, runs `npm ci` in place, and compiles `@mixmark-io/domino` directly using the default module-resolution base.

## Links

- Hosting docs: https://github.com/tomacox74/jroc/blob/master/docs/hosting/Index.md
- Source, issues, docs: https://github.com/tomacox74/jroc
- License: Apache-2.0
