# Hosting/NuGet package validation

This document is the audit trail for the coordinated package migration tracked by issues `#850` and `#439`.

The package split is considered release-ready only when the following package flows are validated together:

- `js2il` - dotnet tool for CLI users
- `Js2IL.Core` - reusable compiler library
- `Js2IL.SDK` - MSBuild integration for `Js2ILCompile`
- `Js2IL.Runtime` - runtime and hosting package

## Pre-publish validation

### Coordinated release gate

`npm run release:validate` is the required local gate before a release commit is created.

It currently runs:

- `npm run diff:test:canary:packed`
- `dotnet test .\Js2IL.Tests\Js2IL.Tests.csproj -c Release --filter "FullyQualifiedName~Js2ILSdkPackageTests" --nologo`

`scripts\release.js` invokes this command before it creates the release commit, so coordinated package regressions fail before the tag is cut.

### `Js2IL.Core` restore/consumption coverage

`Js2IL.Tests\Js2ILSdkPackageTests.cs` covers the referenceable compiler package with:

- `Pack_Js2ILCore_ContainsReadmeIconAndDiscoverabilityMetadata`
  - packs a local feed
  - verifies package metadata, README links, and dependencies

### `Js2IL.SDK` restore/build integration coverage

`Js2IL.Tests\Js2ILSdkPackageTests.cs` covers the SDK package with:

- `Pack_Js2ILSdk_ContainsBuildAssetsSamplesAndCoreDependency`
  - verifies `.props` / `.targets`, task assets, bundled samples, and the `Js2IL.Core` dependency
- `Build_WithLocalJs2ILSdkPackage_CompilesAndRunsHostedModule`
  - restores the package from a local feed, builds a consumer project, and runs the generated module
- `Build_ExtractedHostingBasicSample_WithLocalJs2ILSdkPackage_CompilesAndRuns`
  - extracts `Js2IL.SDK` from the packed `.nupkg`, then builds/runs `samples\Hosting.Basic`
- `Build_ExtractedHostingTypedSample_WithLocalJs2ILSdkPackage_CompilesAndRuns`
  - extracts `Js2IL.SDK` from the packed `.nupkg`, then builds/runs `samples\Hosting.Typed`

### Package boundary and discoverability checks

The same focused package suite also verifies:

- `Pack_Js2ILTool_DoesNotShipHostingSamples`
  - ensures the `js2il` tool package stays separate from the SDK-hosted samples
- `Pack_Js2ILRuntime_ContainsReadmeIconAndDiscoverabilityMetadata`
  - verifies the runtime package metadata/readme surface used by hosting consumers

## Post-publish smoke validation

The published-package smoke flow is covered by:

- `.github\workflows\windows-smoke.yml`
- `.github\workflows\linux-smoke.yml`

These workflows:

- determine the tagged release version
- install the tagged `js2il` tool from NuGet
- compile and run `tests\simple.js`
- build and run the hosted sample apps:
  - `samples\Hosting.Domino`
  - `samples\Hosting.Basic`
  - `samples\Hosting.Typed`

Each hosted sample restores `Js2IL.SDK` and `Js2IL.Runtime` at the tagged version, so the release smoke validates the actual end-user NuGet flow rather than a source-only shortcut.

## Umbrella issue `#439` completion audit

The umbrella packaging migration is complete because the dependent slices are now in place:

- `#845`
  - `Js2IL.Core` exists and ships `Js2IL.Compiler.dll`
- `#846`
  - `Js2IL.SDK` exists as the MSBuild/task package and depends on `Js2IL.Core`
- `#847`
  - hosting samples and docs migrated to `Js2IL.SDK`, and sample package content ships from `Js2IL.SDK`
- `#848`
  - tagged releases pack and publish `js2il`, `Js2IL.Core`, `Js2IL.SDK`, and `Js2IL.Runtime` together with aligned versions
- `#849`
  - the NuGet.org package pages, ownership, readmes, icons, and package cross-links were verified after first publish
- `#850`
  - restore/build/post-publish validation is covered by the release gate and smoke workflows above

With these checks in place, the coordinated Hosting/NuGet migration tracked by issue `#439` is satisfied.
