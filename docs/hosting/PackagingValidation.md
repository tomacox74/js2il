# Hosting/NuGet package validation

This document is the audit trail for the coordinated package migration tracked by issues `#850` and `#439`.

The package split is considered release-ready only when the following package flows are validated together:

- `jroc` - dotnet tool for CLI users
- `Jroc.Core` - reusable compiler library
- `Jroc.SDK` - MSBuild integration for `JrocCompile`
- `Jroc.Runtime` - runtime and hosting package

## Pre-publish validation

### Coordinated release gate

`npm run release:validate` is the required local gate before a release commit is created.

It currently runs:

- `npm run diff:test:canary:packed`
- `dotnet test .\tests\Jroc.Tests\Jroc.Tests.csproj -c Release --filter "FullyQualifiedName~JrocSdkPackageTests" --nologo`

`scripts\release.js` invokes this command before it creates the release commit, so coordinated package regressions fail before the tag is cut.

### `Jroc.Core` restore/consumption coverage

`tests\Jroc.Tests\JrocSdkPackageTests.cs` covers the referenceable compiler package with:

- `Pack_JrocCore_ContainsReadmeIconAndDiscoverabilityMetadata`
  - packs a local feed
  - verifies package metadata, README links, and dependencies

### `Jroc.SDK` restore/build integration coverage

`tests\Jroc.Tests\JrocSdkPackageTests.cs` covers the SDK package with:

- `Pack_JrocSdk_ContainsBuildAssetsSamplesAndCoreDependency`
  - verifies `.props` / `.targets`, task assets, bundled samples, and the `Jroc.Core` dependency
- `Build_WithLocalJrocSdkPackage_CompilesAndRunsHostedModule`
  - restores the package from a local feed, builds a consumer project, and runs the generated module
- `Build_ExtractedHostingBasicSample_WithLocalJrocSdkPackage_CompilesAndRuns`
  - extracts `Jroc.SDK` from the packed `.nupkg`, then builds/runs `samples\Hosting.Basic`
- `Build_ExtractedHostingTypedSample_WithLocalJrocSdkPackage_CompilesAndRuns`
  - extracts `Jroc.SDK` from the packed `.nupkg`, then builds/runs `samples\Hosting.Typed`

### Package boundary and discoverability checks

The same focused package suite also verifies:

- `Pack_JrocTool_DoesNotShipHostingSamples`
  - ensures the `jroc` tool package stays separate from the SDK-hosted samples
- `Pack_JrocRuntime_ContainsReadmeIconAndDiscoverabilityMetadata`
  - verifies the runtime package metadata/readme surface used by hosting consumers

## Post-publish smoke validation

The published-package smoke flow is covered by:

- `.github\workflows\windows-smoke.yml`
- `.github\workflows\linux-smoke.yml`

These workflows:

- determine the tagged release version
- install the tagged `jroc` tool from NuGet
- compile and run `tests\simple.js`
- build and run the hosted sample apps:
  - `samples\Hosting.Domino`
  - `samples\Hosting.Basic`
  - `samples\Hosting.Typed`

Each hosted sample restores `Jroc.SDK` and `Jroc.Runtime` at the tagged version, so the release smoke validates the actual end-user NuGet flow rather than a source-only shortcut.

## Umbrella issue `#439` completion audit

The umbrella packaging migration is complete because the dependent slices are now in place:

- `#845`
  - `Jroc.Core` exists and ships `Jroc.Compiler.dll`
- `#846`
  - `Jroc.SDK` exists as the MSBuild/task package and depends on `Jroc.Core`
- `#847`
  - hosting samples and docs migrated to `Jroc.SDK`, and sample package content ships from `Jroc.SDK`
- `#848`
  - tagged releases pack and publish `jroc`, `Jroc.Core`, `Jroc.SDK`, and `Jroc.Runtime` together with aligned versions
- `#849`
  - the NuGet.org package pages, ownership, readmes, icons, and package cross-links were verified after first publish
- `#850`
  - restore/build/post-publish validation is covered by the release gate and smoke workflows above

With these checks in place, the coordinated Hosting/NuGet migration tracked by issue `#439` is satisfied.
