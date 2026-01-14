# js2il

[![NuGet](https://img.shields.io/nuget/v/js2il.svg)](https://www.nuget.org/packages/js2il/)

Modern JavaScript dotnet compiler.

# JS2IL — JavaScript to .NET IL compiler

JS2IL compiles JavaScript source code to native .NET assemblies. It includes runtime support for some Node.js core modules, enabling JavaScript applications and libraries to run on the .NET runtime.

## Usage

Prerequisite: .NET 10 SDK.

- Convert a JavaScript file (writes output next to the input file by default):

```powershell
js2il .\tests\simple.js
```

- Specify an output directory and optional flags:

```powershell
js2il -i .\tests\simple.js -o .\out -v -a
```

Options

  --version         Show version information and exit
Help: `-h`, `--help`, `-?`

Generated files
- <name>.dll: compiled .NET assembly (name is based on the input .js filename)
- <name>.runtimeconfig.json: runtime configuration file
- JavaScriptRuntime.dll (+ .pdb if available): runtime dependency copied alongside the output

Run the generated assembly

```powershell
dotnet .\out\simple.dll
```

### Try it

First, install js2il as a global tool:

```powershell
dotnet tool install -g js2il
```

Use the sample script at `tests/simple.js`:

```javascript
var x = 1 + 2;
console.log('x is ', x);
```

Compile and run it:

```powershell
js2il .\tests\simple.js .\out
dotnet .\out\simple.dll
```

Expected output:

```
x is  3
```

## Status and scope
- Experimental.
- Not all JavaScript features are supported; `eval` is not supported.
- Two-phase compilation pipeline is always enabled. See `docs/TwoPhaseCompilationPipeline.md`.
- See [JavaScript Feature Coverage](docs/ECMAScript2025_FeatureCoverage.md) for a comprehensive breakdown of supported JavaScript language features organized by specification section.
- See [Node.js Feature Coverage](docs/NodeSupport.md) for details on supported Node.js modules, APIs, and globals.

### Recent improvements (refactor/method-signature-builder branch)
- **Default values in destructuring patterns**: Object destructuring now fully supports default parameter values in function signatures, class constructors, and class methods (e.g., `function config({host = "localhost", port = 8080}) {...}`).
- **Refactored method signature builder**: Consolidated parameter destructuring logic into shared `MethodBuilder` helpers with improved IL generation for conditional default value handling.
- **Enhanced test coverage**: Comprehensive tests validate default values work correctly across all function types (regular functions, arrow functions, class constructors, and class methods).

Errors and exit codes
- Known failures (validation, invalid output path, etc.) print to stderr and exit with a non-zero code.
- Unexpected exceptions propagate and crash normally (standard .NET behavior).

## Roadmap
- Phase 1: Implement sufficient JavaScript semantics to compile most libraries without optimizations (excluding `eval`).
- Phase 2: Apply static and runtime optimizations (e.g., unboxed integers, selective closure fields, direct call paths, shape-based optimizations) to approach or exceed typical Node.js performance.

Note: the Roadmap “Phase 1/Phase 2” is a product maturity plan and is separate from the compiler’s “two-phase compilation pipeline” terminology.

.NET provides a rich type system, cross-platform support, and an out-of-the-box GC implementation that has benefited from many years of optimizations.

## Performance notes
  - The generic implementation represents all locals as fields on a class to support closures. Analysis could eliminate unnecessary closures or add only the fields to closures that are needed by nested functions and arrow functions.
  - Values are always boxed as objects. Analysis could reveal when a variable is always an integer value; for example, it would be more optimal to always represent it as a simple integer in .NET.
  - Functions are invoked through delegates. Analysis could find places where functions could be invoked directly without the need for abstraction.


## Building


To compile the project locally (after installing .NET 10 SDK), run:

```
dotnet build
```


For a release build:

```
dotnet publish -c Release
```


## Release pipeline


When a tag beginning with `v` is pushed, GitHub Actions runs `.github/workflows/release.yml` to build the solution in Release mode and upload the published files as an artifact.

Local development note
- You can still run from source during development:

```powershell
dotnet run --project .\Js2IL -- .\tests\simple.js .\out
```

## Release Workflow

**IMPORTANT**: Always create the release branch FIRST, before running any version bump commands.

### Preferred (Automated)

Use the release automation script to do the full flow (release branch → version bump → commit → PR → optional merge + tag/release):

```powershell
# Patch / minor / major
npm run release:cut -- patch
# npm run release:cut -- minor
# npm run release:cut -- major

# Fully automate through merge + GitHub release creation
npm run release:cut -- patch --merge
```

What it does:
- Validates you're on a clean, up-to-date `master`
- Creates `release/<version>` branch
- Runs the existing `scripts/bumpVersion.js` to update `CHANGELOG.md` + project versions
- Commits, pushes, and opens a PR
- With `--merge`: waits for CI checks (if configured), merges the PR, then creates the GitHub release/tag using the `CHANGELOG.md` section

Requirements: `git`, `gh` (authenticated), `node`/`npm`.

### Complete Release Process

Follow these steps IN ORDER:

#### 1. Create Release Branch

Start from a clean master branch:

```powershell
git checkout master
git pull
git checkout -b release/0.x.y
```

#### 2. Bump Version

Run the appropriate version bump script on the release branch:

```powershell
npm run release:patch  # For patch version (0.x.y -> 0.x.y+1)
# OR
npm run release:minor  # For minor version (0.x.y -> 0.x+1.0)
# OR
npm run release:major  # For major version (0.x.y -> x+1.0.0)
```

What the script does:
- Reads current version from `Js2IL/Js2IL.csproj`
- Extracts the `## Unreleased` section from `CHANGELOG.md`
- Creates a new section: `## vNEW_VERSION - YYYY-MM-DD` with that content
- Resets the `## Unreleased` section to placeholder
- Updates the `<Version>` in both `Js2IL.csproj` and `JavaScriptRuntime.csproj`

#### 3. Commit Version Bump

Commit the changes on the release branch:

```powershell
git add CHANGELOG.md Js2IL/Js2IL.csproj JavaScriptRuntime/JavaScriptRuntime.csproj
git commit -m "chore(release): cut v0.x.y"
```

#### 4. Push and Create PR

Push the release branch and create a pull request:

```powershell
git push -u origin release/0.x.y
gh pr create --title "chore(release): Release v0.x.y" --base master --head release/0.x.y
```

#### 5. Merge and Create Release

After the PR is merged:

```powershell
git checkout master
git pull
gh release create v0.x.y --title "v0.x.y" --notes "See CHANGELOG.md for details" --target master
```

This creates the tag and triggers the GitHub Actions workflow (`.github/workflows/release.yml`) which builds and publishes to NuGet.

### Manual Version Override

You can also set an explicit version:

```powershell
node scripts/bumpVersion.js 0.2.0
```

### Notes

- Empty Unreleased: If there is no real content (only the placeholder) the script still creates an empty release section unless you pass `--skip-empty`
- The script does not preserve pre-release identifiers
- Assumes a single `## Unreleased` sentinel header in CHANGELOG.md
