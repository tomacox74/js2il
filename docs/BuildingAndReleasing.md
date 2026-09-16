# Building and releasing JROC

This guide covers contributor workflows for building JROC from source and
maintainer workflows for publishing a release. For installing and using the
released compiler, start with the [project README](../README.md).

## Prerequisites

Install:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Node.js and npm
- Git
- [GitHub CLI](https://cli.github.com/) authenticated with `gh auth login`
  when running release automation

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

`.github/workflows/publish-tool.yml` builds, tests, packs, and publishes this
package set when a release tag is created. The legacy
`.github/workflows/release.yml` workflow also produces a published artifact
bundle.

The release branch must be created **before** any version-bump command runs.

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

The workflow runs `npm run release:validate`, including the packed-tool canary
suite and package-consumption tests for `Jroc.Runtime`, `Jroc.Core`, and
`Jroc.SDK`. Wait for it to succeed before opening the pull request.

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
