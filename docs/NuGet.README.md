# jroc – JavaScript .NET compiler (consumer guide)

jroc is a JavaScript to .NET compiler, available as a .NET global tool. It compiles JavaScript source code to native .NET assemblies you can run on .NET.

## Install

- Install the .NET 10 or .NET 11 SDK. The CLI targets `net10.0` and rolls
  forward to .NET 11 when only the .NET 11 runtime is installed.
- Install the tool globally:

```powershell
dotnet tool install --global jroc
```

Update later:

```powershell
dotnet tool update --global jroc
```

Uninstall:

```powershell
dotnet tool uninstall --global jroc
```

## Which package should I use?

- [`jroc`](https://www.nuget.org/packages/jroc)
  - The global CLI tool for ad-hoc/manual JavaScript compilation from a terminal.
- [`Jroc.SDK`](https://www.nuget.org/packages/Jroc.SDK)
  - The MSBuild/NuGet integration for host projects that should compile JavaScript during `dotnet build`.
- [`Jroc.Core`](https://www.nuget.org/packages/Jroc.Core)
  - In-memory compilation for scripts not known until runtime.
- [`Jroc.Runtime`](https://www.nuget.org/packages/Jroc.Runtime)
  - Execution support for generated assemblies and in-memory modules;
    `Jroc.SDK` supplies it transitively to MSBuild consumers.

Official releases publish `Jroc.Runtime`, `jroc`, `Jroc.Core`, and `Jroc.SDK` together at the same version. When you mix these packages in one workflow, keep the versions aligned.

## Usage

```powershell
jroc <InputFile> [<OutputPath>] [options]
jroc <InputFile> [<OutputPath>] -a <file> [-a <file> ...] [options]
jroc --moduleid <ModuleId> [--output <directory>] [options]
jroc coverage <InputFile> [--json] [--coverage-report <file>]
```

| Option | Description |
| --- | --- |
| `-i`, `--input <file>` | Compile a JavaScript file. The positional form is also supported. |
| `-a`, `--additional-input <file>` | Add another JavaScript input to the same assembly. Repeat for more files. Cannot be combined with `--moduleid`. |
| `--moduleid <id>` | Compile an npm/CommonJS module ID resolved from the current working directory. |
| `-o`, `--output <directory>` | Set the output directory. The default is the directory containing the entry file. Use this option to set the output directory with `--moduleid`. |
| `--assemblyname <name>` | Set the generated assembly identity and artifact basename. |
| `-v`, `--verbose` | Print compiler diagnostics to the console. |
| `--diagnostic-file <path>` | Write compiler diagnostics to a text file. |
| `--analyzeunused` | Report unused functions, properties, and variables. |
| `--pdb` | Emit Portable PDB debug symbols. |
| `--coverage` | Analyze compilation modes in memory without writing or executing an assembly. |
| `--json` | Print machine-readable JSON with `coverage` or `--coverage`. Cannot be combined with `--verbose` or `--analyzeunused`. |
| `--coverage-report <file>` | Write a coverage JSON report during analysis or normal compilation. |
| `--assume-unmodified-host-globals` | Allow guard-free calls where source analysis permits and the host guarantees that built-in global bindings stay unchanged. |
| `--version` | Show the installed JROC version. |
| `-h`, `-?`, `--help` | Show command-line help. |

Save this as `hello.js`:

```javascript
console.log("Hello from JROC!");
```

```powershell
# Compile hello.js and write output next to the file
jroc .\hello.js

# Convert to a specific directory with verbose output
jroc .\hello.js -o .\out -v

# Compile an installed npm package from the current project
jroc --moduleid turndown --output .\out

# Inspect compilation modes and save a JSON report
jroc coverage .\hello.js --coverage-report .\coverage.json
```

## What gets generated?

Given an input like `C:\code\sample.js`, jroc will emit the following into the output directory (default: alongside the input file):

- `sample.dll`
	- A .NET assembly containing IL corresponding to your JavaScript.
	- The assembly name defaults to the entry file name without its extension. Use `--assemblyname` to override it.
	- Contains a `Program.Main` entry point that executes your script when run.
- `sample.runtimeconfig.json`
	- Runtime configuration for the `dotnet` host. It targets the runtime version
	  that ran JROC (for example, .NET 10 or .NET 11).
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
- Compiler errors go to stderr, and compilation failures return a nonzero exit code.

## Limitations

- The CLI targets `net10.0` and supports running on .NET 10 or .NET 11.
- Not all JS features are supported; some constructs may be validated and rejected with explanations.
- Emitted IL and runtime surface are subject to change between previews.

## Troubleshooting

- Run `dotnet --info` to check which SDK and runtimes the `dotnet` command can
  see. The JROC CLI can run on .NET 10 or .NET 11.
- Use `-v` to print diagnostics to the console, or `--diagnostic-file <path>` to capture diagnostics to a file.
- File an issue with a minimal JS sample if you suspect a bug.

## Hosting samples

The `jroc` tool package no longer carries the hosted C# samples. Those samples now ship with `Jroc.SDK`, because they are intended to validate the MSBuild/NuGet integration path that hosting consumers use in real projects.

See:

- the repo `samples\` directory
- `Jroc.SDK` package page: `https://www.nuget.org/packages/Jroc.SDK`
- `Jroc.Core` package page: `https://www.nuget.org/packages/Jroc.Core`

## Links

- CLI usage: https://github.com/tomacox74/js2il#command-line-usage
- SDK (.NET library mode): https://github.com/tomacox74/js2il/blob/master/docs/sdk/Index.md
- Source, issues, docs: https://github.com/tomacox74/js2il
- License: Apache-2.0
