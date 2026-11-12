# jroc - JavaScript-to-.NET command-line compiler

JROC compiles JavaScript ahead of time to .NET IL assemblies. Use the `jroc`
global tool to compile and run scripts without creating a C# host project.

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
| `--target-framework <tfm>` | Target `net10.0` or `net11.0` for the standalone output. Defaults to the runtime running JROC. |
| `--targeting-pack <path>` | Use a specific `Microsoft.NETCore.App.Ref` pack version directory. Requires `--target-framework`. |
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
dotnet .\out\hello.dll

# Compile an installed npm package from the current project
jroc --moduleid turndown --output .\out

# Inspect compilation modes and save a JSON report
jroc coverage .\hello.js --coverage-report .\coverage.json
```

### Selecting the output framework

Without `--target-framework`, output continues to target the .NET runtime running
the CLI: .NET 10 on a .NET 10 host, or .NET 11 on a .NET 11 host. This default does
not require a targeting pack.

An explicit target uses that framework's reference assemblies and writes matching
assembly identities and runtime configuration:

```powershell
# Build on .NET 11, then copy the complete out directory to a .NET 10 machine.
jroc hello.js -o out --target-framework net10.0
dotnet out/hello.dll

jroc hello.js -o out11 --target-framework net11.0
```

JROC looks for the newest matching pack under
`packs/Microsoft.NETCore.App.Ref/<version>/ref/<tfm>` in the .NET installation
running the CLI. The selected SDK normally supplies its own framework's pack.
For a different target, copy the required pack from another SDK installation into
that `packs` directory, or supply its version directory explicitly:

```powershell
jroc hello.js -o out --target-framework net10.0 `
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
	- The assembly name defaults to the entry file name without its extension. Use `--assemblyname` to override it.
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

## .NET compatibility

The tool is packaged for `net10.0` and can execute on .NET 10 or, through
major roll-forward, .NET 11. By default, its generated framework references
and `runtimeconfig.json` follow the runtime **executing the compiler**:
default output compiled on .NET 11 requires .NET 11, not just the tool's
`net10.0` target. Use `--target-framework` with the matching reference pack
to select a different output framework, as described above.

For JavaScript compiled during a .NET project build, `Jroc.SDK` automatically
uses the consuming project's chosen target framework. An SDK 11 build of a
`net10.0` project generates output for .NET 10 rather than for the compiler's
.NET 11 process.

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

- Quick start: https://github.com/tomacox74/js2il/blob/master/docs/sdk/tutorials/GettingStarted.md
- CLI usage: https://github.com/tomacox74/js2il#command-line-usage
- SDK (.NET library mode): https://github.com/tomacox74/js2il/blob/master/docs/sdk/Index.md
- Migrating from JS2IL: https://github.com/tomacox74/js2il/blob/master/docs/sdk/LegacyPackageMigration.md
- Release history: https://github.com/tomacox74/js2il/blob/master/CHANGELOG.md
- Source: https://github.com/tomacox74/js2il
- Issues: https://github.com/tomacox74/js2il/issues
- License: Apache-2.0
