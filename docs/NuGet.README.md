# js2il – JavaScript to .NET IL (consumer guide)

js2il is a .NET global tool that parses JavaScript (ES) and emits ECMA‑335 IL you can run on .NET.

## Install

- Prerequisite: .NET 8.0 SDK or runtime installed.
- Install the tool globally:

```powershell
dotnet tool install --global js2il
```

Update later:

```powershell
dotnet tool update --global js2il
```

Uninstall:

```powershell
dotnet tool uninstall --global js2il
```

## Usage

```powershell
js2il <InputFile> [<OutputPath>] [-V] [-A]
```

- InputFile (-I)       The JavaScript file to convert (required)
- OutputPath (-O)      Output folder for the generated IL/assembly (defaults to the input file directory)
- Verbose (-V)         Prints AST and scope info
- AnalyzeUnused (-A)   Reports unused functions/properties/variables

Example:

```powershell
# Convert tests\simple.js and write output next to the file
js2il .\tests\simple.js

# Convert to a specific directory with verbose output
js2il .\tests\simple.js .\out -V
```

## What gets generated?

Given an input like `C:\code\sample.js`, js2il will emit the following into the output directory (default: alongside the input file):

- `sample.dll`
	- A .NET assembly (targeting net8.0) containing IL corresponding to your JavaScript.
	- The assembly name is the input file name without extension.
	- Contains a `Program.Main` entry point that executes your script when run.
- `sample.runtimeconfig.json`
	- Runtime configuration for the `dotnet` host (framework: .NET 8).
- `JavaScriptRuntime.dll` (+ optional `JavaScriptRuntime.pdb` if available)
	- Required runtime support library that provides JS primitives (e.g., `console.log`, arrays, objects) used by the emitted IL.
	- This file is copied next to your generated assembly and must be present at runtime.

Run it with:

```powershell
dotnet .\sample.dll
```

Notes:

- Console output (e.g., `console.log`) is implemented via the bundled `JavaScriptRuntime.dll`.
- This is a prototype and doesn’t yet support all JavaScript features. See the repo docs for supported syntax and feature coverage.

## Limitations

- Target framework: net8.0
- Not all JS features are supported; some constructs may be validated and rejected with explanations.
- Emitted IL and runtime surface are subject to change between previews.

## Troubleshooting

- Ensure the .NET 8.0 SDK/runtime is on PATH: `dotnet --info`
- Use `-V` to print extra diagnostics.
- File an issue with a minimal JS sample if you suspect a bug.

## Links

- Source, issues, docs: https://github.com/tomacox74/js2il
- License: Apache-2.0
