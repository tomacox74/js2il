# SDK/NuGet package validation

This document is the audit trail for the coordinated package migration tracked by issues `#850` and `#439`.

This is maintainer validation guidance. For the three supported consumer
workflows (CLI, MSBuild-generated APIs, and in-memory compilation), start with
the [SDK overview](Index.md).

The package split is considered release-ready only when the following package flows are validated together:

- `jroc` - dotnet tool for CLI users
- `Jroc.Core` - in-memory compilation for runtime-supplied scripts
- `Jroc.SDK` - MSBuild integration for `JrocCompile`
- `Jroc.Runtime` - runtime and hosting package

## Pre-publish validation

### Coordinated release gate

`.github/workflows/release-validation.yml` is the required remote gate after the release candidate is committed and pushed. `scripts\release.js` dispatches it against the exact release-branch commit and only opens the PR after it succeeds.

The workflow reuses `.github/workflows/dotnet.yml` to run all 11 existing native
CI shards on Windows x64 (`windows-2025`) and macOS ARM64 (`macos-15`). This
includes unit, execution and generator tests, the five ported Test262 shards,
and five SDK packaging shards, retaining the ordinary CI `ReleaseOnly`
exclusions. These are not the central Test262 catalogue/supervisor jobs.

Linux, Windows and macOS additionally run `npm run release:validate`, preserving
the existing Linux package gate. This command includes:

- `npm run diff:test:canary:packed`
- `dotnet test .\tests\Jroc.Tests\Jroc.Tests.csproj -c Release --filter "FullyQualifiedName~JrocSdkPackageTests|FullyQualifiedName~CheerioGeneratorTests" --nologo`

This validates source behavior and local package consumption on all three operating
systems before the tag is cut. Normal PR/push CI keeps its existing Linux
coverage; workflow-change PRs exercise the cross-platform release gate too.

`.github/workflows/publish-tool.yml` repeats this reusable gate after resolving
the release tag to an immutable commit. The publication job depends on successful
validation, uses the same commit/version, and only then receives NuGet OIDC
permissions. Failed or cancelled validation blocks publication; manually
retrying a release tag does not bypass the gate. Read-only validation jobs do
not inherit publication secrets or identity-token permissions.

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
- `Build_ExtractedBasicSample_WithLocalJrocSdkPackage_CompilesAndRuns`
  - extracts `Jroc.SDK` from the packed `.nupkg`, then builds/runs `samples\Basic`
- `Build_ExtractedTypedSample_WithLocalJrocSdkPackage_CompilesAndRuns`
  - extracts `Jroc.SDK` from the packed `.nupkg`, then builds/runs `samples\Typed`

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
  - `samples\Domino`
  - `samples\Basic`
  - `samples\Typed`

Each hosted sample restores `Jroc.SDK` at the tagged version and receives
`Jroc.Runtime` transitively, so release smoke validates the actual end-user
NuGet flow rather than a source-only shortcut.

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

With these checks in place, the coordinated SDK/NuGet migration tracked by issue `#439` is satisfied.
