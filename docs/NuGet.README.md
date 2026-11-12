# jroc - JavaScript-to-.NET command-line compiler

JROC compiles JavaScript ahead of time to .NET IL assemblies. Use the `jroc`
global tool to compile and run scripts without creating a C# host project.

## Install

- Prerequisite: .NET 10 SDK or later installed. The CLI targets .NET 10 and
  rolls forward to a later installed .NET runtime when .NET 10 is unavailable.
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
```

Common options:

| Option | Purpose |
|---|---|
| `-i`, `--input` | Input JavaScript file; also accepted positionally |
| `-o`, `--output` | Output directory; defaults to the input file's directory |
| `-a`, `--additional-input <file>` | Additional entry file; repeat for multiple files |
| `-v` | Print compiler diagnostics |
| `--diagnostic-file <path>` | Write compiler diagnostics to a text file |
| `--analyzeunused` | Report unused functions, properties, and variables |
| `--pdb` | Emit Portable PDB debug symbols for the generated assembly |
| `--version` | Show version information and exit |

Help: `-h`, `--help`, `-?`

Create `hello.js`:

```javascript
console.log("Hello from JROC!");
```

Compile and run it from the same directory:

```shell
jroc hello.js out
dotnet out/hello.dll
```

Output:

```text
Hello from JROC!
```

## What gets generated?

Given an input like `C:\code\sample.js`, jroc will emit the following into the output directory (default: alongside the input file):

- `sample.dll`
	- A .NET assembly containing IL corresponding to your JavaScript.
	- The assembly name is the input file name without extension.
	- Contains a `Program.Main` entry point that executes your script when run.
- `sample.runtimeconfig.json`
	- Runtime configuration for the `dotnet` host, based on the .NET runtime
	  running the CLI (for example, .NET 10 or .NET 11).
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
- Errors are written to stderr and known failures return a non-zero exit code.

## .NET compatibility

The tool is packaged for `net10.0` and can execute on .NET 10 or, through
major roll-forward, .NET 11. Its generated framework references and
`runtimeconfig.json` follow the runtime **executing the compiler**: output
compiled on .NET 11 requires .NET 11, not just the tool's `net10.0` target.

To compile JavaScript for a .NET project's chosen target framework instead,
use `Jroc.SDK`. An SDK 11 build of a `net10.0` project then generates output
for .NET 10 rather than for the compiler's .NET 11 process.

## Limitations

- CLI target framework: net10.0; later installed .NET runtimes are supported
  through major roll-forward.
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

- Quick start: https://github.com/tomacox74/js2il/blob/master/docs/sdk/tutorials/GettingStarted.md
- SDK (.NET library mode): https://github.com/tomacox74/js2il/blob/master/docs/sdk/Index.md
- Migrating from JS2IL: https://github.com/tomacox74/js2il/blob/master/docs/sdk/LegacyPackageMigration.md
- Release history: https://github.com/tomacox74/js2il/blob/master/CHANGELOG.md
- Source: https://github.com/tomacox74/js2il
- Issues: https://github.com/tomacox74/js2il/issues
- License: Apache-2.0
