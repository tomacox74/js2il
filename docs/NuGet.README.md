# jroc – JavaScript .NET compiler (consumer guide)

jroc is a JavaScript to .NET compiler, available as a .NET global tool. It compiles JavaScript source code to native .NET assemblies you can run on .NET.

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

Options

- -i, --input           The JavaScript file to convert (positional supported)
- -o, --output          Output folder for the generated IL/assembly (created if missing; defaults to the input file directory)
- -v, --verbose         Print compiler diagnostics to the console
- --diagnostic-file <path>  Write compiler diagnostics to a text file
- --target-framework <tfm>  Target `net10.0` or `net11.0` independently of the runtime running JROC
- --targeting-pack <path>  Use a specific `Microsoft.NETCore.App.Ref` pack directory (requires `--target-framework`)
- -a, --analyzeunused   Report unused functions/properties/variables
	--version         Show version information and exit

Help: `-h`, `--help`, `-?`

Example:

```powershell
# Convert tests\simple.js and write output next to the file
jroc .\tests\simple.js

# Convert to a specific directory with verbose output
jroc .\tests\simple.js -o .\out -v
```

### Selecting the output framework

Without `--target-framework`, output continues to target the .NET runtime running
the CLI: .NET 10 on a .NET 10 host, or .NET 11 on a .NET 11 host. This default does
not require a targeting pack.

An explicit target uses that framework's reference assemblies and writes matching
assembly identities and runtime configuration:

```powershell
# Build on .NET 11, then copy the complete out directory to a .NET 10 machine.
jroc tests/simple.js -o out --target-framework net10.0
dotnet out/simple.dll

jroc tests/simple.js -o out11 --target-framework net11.0
```

JROC looks for the newest matching pack under
`packs/Microsoft.NETCore.App.Ref/<version>/ref/<tfm>` in the .NET installation
running the CLI. The selected SDK normally supplies its own framework's pack.
For a different target, copy the required pack from another SDK installation into
that `packs` directory, or supply its version directory explicitly:

```powershell
jroc tests/simple.js -o out --target-framework net10.0 `
  --targeting-pack /reference-packs/Microsoft.NETCore.App.Ref/10.0.0
```

The supplied directory must contain `data/FrameworkList.xml`,
`ref/net10.0/System.Runtime.dll` (or the corresponding `net11.0` directory), and
the pack's other reference DLLs. An extracted
`Microsoft.NETCore.App.Ref` NuGet package has this layout too; for example, use
`~/.nuget/packages/microsoft.netcore.app.ref/10.0.0` if a .NET 11 SDK restored that
pack for a .NET 10 project. A targeting pack is compile-time metadata: providing
the .NET 10 pack does **not** require installing a .NET 10 SDK or runtime on the
build machine. JROC does not download missing packs automatically.

Only `net10.0` and `net11.0` are supported. Invalid targets, missing/incomplete packs, and
packs with mismatched `System.Runtime` identities fail with a nonzero exit code
and a diagnostic explaining how to provide the correct pack.
These options apply to standalone assembly compilation, not coverage-only analysis.

## What gets generated?

Given an input like `C:\code\sample.js`, jroc will emit the following into the output directory (default: alongside the input file):

- `sample.dll`
	- A .NET assembly containing IL corresponding to your JavaScript.
	- The assembly name is the input file name without extension.
	- Contains a `Program.Main` entry point that executes your script when run.
- `sample.runtimeconfig.json`
	- Runtime configuration for the selected output framework, or the .NET runtime
	  running the CLI when `--target-framework` is omitted.
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

- SDK (.NET library mode): https://github.com/tomacox74/js2il/blob/master/docs/sdk/Index.md
- Source, issues, docs: https://github.com/tomacox74/js2il
- License: Apache-2.0
