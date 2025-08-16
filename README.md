
# JS2IL — JavaScript to .NET IL compiler

JS2IL compiles JavaScript source code to .NET Intermediate Language (IL), producing managed assemblies that run on the .NET runtime. It enables execution of JavaScript code and libraries as native .NET assemblies.

## Usage

Prerequisite: .NET 8 SDK

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


To compile the project locally, run:

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
