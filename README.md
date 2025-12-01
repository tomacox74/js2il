# js2il
Modern JS to IL prototype.

## Release Workflow Automation

To cut a new release while keeping `CHANGELOG.md` and `Js2IL/Js2IL.csproj` in sync you can use the version bump script added in `scripts/bumpVersion.js`.

Usage (from repo root):

```
npm run release:patch     # bumps patch version (x.y.z -> x.y.(z+1))
npm run release:minor     # bumps minor version (x.y.z -> x.(y+1).0)
npm run release:major     # bumps major version ((x+1).0.0)
node scripts/bumpVersion.js 0.2.0   # set an explicit version
```

What it does:
1. Reads current version from `Js2IL/Js2IL.csproj`.
2. Extracts the `## Unreleased` section from `CHANGELOG.md`.
3. Creates a new section: `## vNEW_VERSION - YYYY-MM-DD` populated with that content (skipping the placeholder `_Nothing yet._`).
4. Resets the `## Unreleased` section body to the placeholder.
5. Updates the `<Version>` element in the csproj.
6. Prints next git commands (add/commit/tag/push).

Empty Unreleased:
If there is no real content (only the placeholder) the script still creates an empty release section unless you pass `--skip-empty` (explicit invocation only).

Example manual flow after bump:

```powershell
# Create release branch
git checkout -b release/<new-version>

# Bump version
npm run release:patch

# Review changes
git add CHANGELOG.md Js2IL/Js2IL.csproj
git commit -m "chore(release): cut <new-version>"

# Create PR back to master
gh pr create --base master --head release/<new-version> --title "chore(release): cut <new-version>"

# After PR is merged:
git checkout master
git pull origin master

# Create release (this creates the tag and triggers GitHub Actions)
gh release create v<new-version> --title "v<new-version>" --notes "Release notes from CHANGELOG" --target master
```

CI / GitHub Actions:
There is an existing workflow (`.github/workflows/release.yml`) for building & publishing artifacts when a release is created. The `gh release create` command automatically creates the tag and triggers the workflow.

Limitations / TODO:
- Does not preserve pre-release identifiers or generate them.
- No automatic generation of release notes beyond what you curate in Unreleased.
- Assumes a single `## Unreleased` sentinel header.


# JS2IL — JavaScript to .NET IL compiler

JS2IL compiles JavaScript source code to .NET Intermediate Language (IL), producing managed assemblies that run on the .NET runtime. It enables execution of JavaScript code and libraries as native .NET assemblies.

## Usage

Prerequisite: .NET 10 SDK (upgrade branch). Master currently targets .NET 8.

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
