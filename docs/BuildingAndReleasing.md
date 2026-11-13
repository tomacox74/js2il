# Building and releasing JROC

This guide covers contributor workflows for building JROC from source and
maintainer workflows for publishing a release. For installing and using the
released compiler, start with the [project README](../README.md).

## Prerequisites

Install:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Node.js 24 LTS and npm
- Git
- [GitHub CLI](https://cli.github.com/) authenticated with `gh auth login`
  when running release automation

Workflows that run Node.js or npm use `actions/setup-node@v6` with
`node-version: '24'` and `check-latest: true` so each run selects the latest
available Node.js 24 release rather than an older runner-cached patch.
Keep these settings when adding jobs. Test262 fixture image builds also use
`docker build --pull` to refresh their Node.js 24 base image.
JavaScript actions target the Node.js 24 runtime supplied by the GitHub Actions
runner; keep action versions on a Node.js 24-compatible release line.

## Build from source

Clone the repository and build the solution:

```shell
git clone https://github.com/tomacox74/jroc.git
cd jroc
dotnet build
```

Create Release-configuration outputs:

```shell
dotnet publish -c Release
```

Run the CLI directly from source:

```shell
dotnet run --project src/Cli/Jroc.csproj -- input.js out
dotnet out/input.dll
```

## NuGet dependency versions

The repository uses [NuGet Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management).
Edit `Directory.Packages.props` to update third-party dependencies; project
files declare versionless `PackageReference` items and retain their existing
asset metadata. `SharpTSVersion` also controls the benchmark tool download and
its auxiliary DLL path. Transitive pinning is not enabled, so centralization
does not add dependencies to the published packages.

The samples have their own `samples/Directory.Packages.props`, which ships in
`Jroc.SDK` and intentionally does not import the repository file. Copy it
alongside `samples/Directory.Build.props` when extracting individual samples.
The existing release-version properties in `Directory.Build.props` continue to
select the sample SDK version, including command-line release-candidate overrides.

Published-package benchmarks use NuGet's `VersionOverride` for `Jroc.Core` and
`Jroc.Runtime`, preserving per-run `JrocPackageVersion` selection and the previous
release environment-variable fallback. This is intentionally separate from
third-party dependency pins and JROC's own package release versions.

## Run tests

Run the standard test suite:

```shell
dotnet test
```

The complete `tests/Jroc.Test262.Tests` project is substantially slower than
the focused suites. During development, prefer a targeted test filter that
covers the changed behavior.

Generator tests use checked-in IL snapshots. When an intentional IL change
produces new `*.received.*` files, update the snapshots and rerun the affected
tests:

```shell
node scripts/updateVerifiedFiles.js
```

## Release package set

JROC releases these coordinated NuGet packages at the same version:

- `Jroc.Runtime`
- `jroc`
- `Jroc.Core`
- `Jroc.SDK`

`.github/workflows/publish-tool.yml` validates and publishes this package set
when a release tag is created. **Release builds and packs use stable .NET 11
SDKs; the packages still target `net10.0` and require only .NET 10 or later.**
The checked-in `global.json` remains on SDK 10 for contributor builds.
Release jobs explicitly select an installed stable SDK 11 using
`scripts/ci/selectReleaseSdk.sh`, pinning it in that job's checkout with
roll-forward disabled; the repository pin cannot silently select SDK 10.
The legacy
`.github/workflows/release.yml` workflow also produces a published artifact
bundle.

Before NuGet login or publication, the publishing workflow resolves the release
tag once and requires Windows x64 (`windows-2025`) and macOS ARM64 (`macos-15`)
validation against that exact commit. Both platforms run the same 11 native
CI shards as ordinary Linux PR CI. Packed-tool canaries and Release
package-consumption/Cheerio tests run on Linux, Windows and macOS, preserving
the existing Linux package gate. A failed, cancelled or skipped required
validation job prevents publication. Publishing checks out the frozen commit,
not a potentially changed tag.

Release validation also calls `sdk11-package-validation.yml` with the frozen
source commit and coordinated release version. It builds and packs the four
packages once, then tests that exact artifact on isolated .NET 10/11 CLI
installations and all three Basic SDK consumer scenarios. The publisher
downloads the validated artifact by its producer ID, verifies the source SHA,
version, stable SDK 11 provenance and package hashes before NuGet login, and
pushes those same four files. It does not rebuild or repack after validation.
The CLI's existing `Major` runtime roll-forward and cross-targeting behavior
are unchanged. Full CI shards remain shared rather than duplicated for each
consumer runtime.

The release branch must be created **before** any version-bump command runs.

### Pre-publication SDK 11 validation

Run the on-demand **SDK 11 coordinated package validation** workflow
(`sdk11-package-validation.yml`) against the desired branch. It temporarily
selects stable SDK 11 in that job's checkout, builds the solution, and packs
one uniquely versioned `net10.0` set of all four packages by default. Its
optional `package_version` input selects an exact coordinated candidate
version. The production release gate reuses this same workflow with the
release tag's version and frozen source commit.

The candidate artifact includes the exact source SHA, SDK version, package
version, build/pack logs, package hashes, and a dependency-complete local feed.
The existing packaged CLI harness consumes that artifact on Linux and Windows
in isolated .NET 10-only and .NET 11-only installations, without roll-forward
overrides. The existing packaged Basic-sample harness validates the same feed
for SDK 10/net10.0, SDK 11/net11.0, and SDK 11/net10.0 executed separately on
.NET 10. Each sample has a fresh consumer directory and package store;
resolved candidate package identities, generated framework references, and
runtime configuration are checked and retained as evidence.

`Jroc.SDK` uses private, compile-only MSBuild reference packages compatible
with .NET 10. Do not reference the executing SDK's MSBuild binaries: SDK 11's
binaries require .NET 11 and cannot be compiled into the shipped net10.0 task.
The consuming MSBuild host supplies these assemblies at execution time.

A passing compatibility run alone is not release authorization. All existing
release validation gates must also pass; use the normal release process below
for publication. To validate a versioned candidate without publishing:

```shell
gh workflow run release-validation.yml --ref release/0.x.y -f package_version=0.x.y
```

## Preferred automated release

The release script handles branch creation, version updates, commit and push,
remote validation, and pull-request creation:

```shell
npm run release:cut -- patch
# npm run release:cut -- minor
# npm run release:cut -- major
```

To continue through pull-request merge and GitHub release creation:

```shell
npm run release:cut -- patch --merge
```

The automation:

1. Verifies that the working tree is clean and `master` is current.
2. Creates `release/<version>`.
3. Updates `CHANGELOG.md` and all coordinated package versions.
4. Commits and pushes the release candidate.
5. Runs `.github/workflows/release-validation.yml` against the pushed commit.
6. Opens a pull request after validation succeeds.
7. With `--merge`, waits for required checks, merges the pull request, and
   creates the GitHub release and tag from the matching changelog section.

Useful options include `--skip-empty`, `--dry-run`, `--repo owner/name`,
`--base master`, and `--verbose`.

## Manual release fallback

Use this process only when the automated release script cannot complete the
workflow.

### 1. Create the release branch

Start from an up-to-date `master`:

```shell
git switch master
git pull --ff-only
git switch -c release/0.x.y
```

### 2. Bump the version

Run exactly one version command on the release branch:

```shell
npm run release:patch
# npm run release:minor
# npm run release:major
```

The version script:

- Moves the `CHANGELOG.md` Unreleased content into a dated release section.
- Updates `samples/Directory.Build.props`.
- Updates the versions in `src/Cli/Jroc.csproj`,
  `src/Jroc.Core/Jroc.Core.csproj`, `src/Jroc.SDK/Jroc.SDK.csproj`, and
  `src/JavaScriptRuntime/JavaScriptRuntime.csproj`.

An explicit version can be supplied when necessary:

```shell
node scripts/bumpVersion.js 0.2.0
```

### 3. Commit and push

```shell
git add CHANGELOG.md samples/Directory.Build.props \
  src/Cli/Jroc.csproj \
  src/Jroc.Core/Jroc.Core.csproj \
  src/Jroc.SDK/Jroc.SDK.csproj \
  src/JavaScriptRuntime/JavaScriptRuntime.csproj
git commit -m "chore(release): cut v0.x.y"
git push -u origin release/0.x.y
```

### 4. Validate the release packages

Dispatch the validation workflow against the pushed branch:

```shell
gh workflow run release-validation.yml --ref release/0.x.y
```

The workflow runs the shared native CI suite on Windows and macOS, including
unit/execution/generator, five Test262 and five SDK packaging shards. It also
runs `npm run release:validate` on Linux, Windows and macOS, including packed-tool canaries
and Release package-consumption/Cheerio tests. Wait for all jobs to succeed
before opening the pull request. This is the existing ported native Test262
suite, not the separate central catalogue screening infrastructure.

The same validation is repeated for the exact tag commit by the publisher;
an earlier green release-branch run is not substituted for tag validation.
Workflow-change PRs also exercise this cross-platform gate without publishing
or receiving NuGet credentials. Ordinary source PR CI remains Linux-only.
Native CI uses an 8 MiB managed-thread stack on all runners, matching the Linux
default for recursive Debug compiler traversals. Test fixture bytes are retained
exactly across checkouts, including files with byte-count assertions.

For the complete restore, build, and post-publish validation matrix, see
[SDK packaging validation](sdk/PackagingValidation.md).

### 5. Open and merge the release pull request

```shell
gh pr create \
  --title "chore(release): Release v0.x.y" \
  --base master \
  --head release/0.x.y
```

Merge only after all required checks pass.

### 6. Create the GitHub release

After the release pull request is merged:

```shell
git switch master
git pull --ff-only
gh release create v0.x.y \
  --title "v0.x.y" \
  --notes-file release-notes.md \
  --target master
```

Create `release-notes.md` from the matching `CHANGELOG.md` section before
running the command. Creating the release creates the version tag and triggers
the publishing workflows.

After publication, the Windows and Linux smoke workflows install the tagged
tool from NuGet and build and run the package samples against the matching SDK
and runtime versions.

## Release notes

- An empty Unreleased section still creates an empty release section unless
  `--skip-empty` is used.
- The version-bump script does not preserve prerelease identifiers.
- `CHANGELOG.md` must contain exactly one `## Unreleased` heading.
- If a tagged publish fails, `.github/workflows/publish-tool.yml` supports
  manual dispatch with the existing release tag; do not move or recreate the
  tag.
