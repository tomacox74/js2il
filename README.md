# JROC

[![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?logo=javascript&logoColor=black)](https://developer.mozilla.org/en-US/docs/Web/JavaScript)
[![.NET](https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Target framework: net10.0](https://img.shields.io/badge/.NET_target-net10.0-512BD4?logo=dotnet&logoColor=white)](#compatibility-and-project-status)
[![Build and tests on master](https://img.shields.io/github/actions/workflow/status/tomacox74/js2il/dotnet.yml?branch=master&event=push&label=build%20%26%20tests)](https://github.com/tomacox74/js2il/actions/workflows/dotnet.yml?query=branch%3Amaster)
[![Windows](https://img.shields.io/badge/Windows-0078D4?logo=windows&logoColor=white)](#compatibility-and-project-status)
[![macOS](https://img.shields.io/badge/macOS-000000?logo=apple&logoColor=white)](#compatibility-and-project-status)
[![Linux](https://img.shields.io/badge/Linux-FCC624?logo=linux&logoColor=black)](#compatibility-and-project-status)
[![NuGet version](https://img.shields.io/nuget/v/jroc.svg)](https://www.nuget.org/packages/jroc/)

JROC is a JavaScript compiler for .NET. It produces assemblies you can run with
`dotnet` and includes a JavaScript runtime with support for a growing set of
Node.js APIs. You can also compile JavaScript as part of a .NET build with
[`Jroc.SDK`](#use-jroc-from-net-projects).

## Install JROC

Install the [.NET 10 or .NET 11 SDK](https://dotnet.microsoft.com/download/dotnet).
The CLI targets .NET 10 and rolls forward to .NET 11 when only the .NET 11
runtime is installed.

Install JROC from NuGet as a global .NET tool:

```shell
dotnet tool install --global jroc
```

Confirm that the command is available:

```shell
jroc --version
```

To update an existing installation:

```shell
dotnet tool update --global jroc
```

## Compile and run JavaScript

Create `hello.js`:

```javascript
const name = "JROC";
console.log(`Hello from ${name}!`);
```

Compile it into a .NET assembly:

```shell
jroc hello.js out
```

Run the generated program:

```shell
dotnet out/hello.dll
```

Expected output:

```text
Hello from JROC!
```

JROC writes these files to the output directory:

- `hello.dll` — the compiled .NET assembly.
- `hello.runtimeconfig.json` — tells the `dotnet` host which runtime the
  generated program targets. By default, this matches the .NET runtime running
  JROC.
- `JavaScriptRuntime.dll` — the runtime support required by the generated
  assembly.

If no output directory is supplied, JROC writes the generated files next to
the input file.

## Command-line usage

```text
jroc <InputFile> [<OutputPath>] [options]
jroc <InputFile> [<OutputPath>] -a <file> [-a <file> ...] [options]
jroc --moduleid <ModuleId> [--output <directory>] [options]
jroc coverage <InputFile> [--json] [--coverage-report <file>]
```

| Option | Description |
|---|---|
| `-i`, `--input <file>` | Compile a JavaScript file. The positional form is also supported. |
| `-a`, `--additional-input <file>` | Add another JavaScript input to the same assembly; repeat for more files. Cannot be used with `--moduleid`. |
| `--moduleid <id>` | Compile an npm/CommonJS module ID, such as `turndown` or `@scope/pkg`. |
| `-o`, `--output <directory>` | Select the output directory. |
| `--assemblyname <name>` | Set the generated assembly identity and artifact basename. |
| `-v`, `--verbose` | Print compiler diagnostics. |
| `--diagnostic-file <path>` | Write compiler diagnostics to a text file. |
| `--target-framework <tfm>` | Target `net10.0` or `net11.0` for the standalone output. Defaults to the runtime running JROC. |
| `--targeting-pack <path>` | Use a specific `Microsoft.NETCore.App.Ref` pack version directory. Requires `--target-framework`. |
| `--analyzeunused` | Report unused functions, properties, and variables. |
| `--pdb` | Emit Portable PDB debug symbols. |
| `--coverage` | Analyze compilation modes in memory without writing or executing an assembly. |
| `--json` | Print machine-readable JSON with `coverage` or `--coverage`. Cannot be combined with `--verbose` or `--analyzeunused`. |
| `--coverage-report <file>` | Write a coverage JSON report, also during normal compilation. |
| `--assume-unmodified-host-globals` | Opt into guard-free calls to provably unchanged built-ins when the host guarantees pristine global bindings throughout execution. |
| `--version` | Print the installed JROC version. |
| `-h`, `-?`, `--help` | Show command-line help. |

For example:

```shell
jroc app.js --output out --pdb
dotnet out/app.dll
```

By default, compiled global calls check that a built-in still has its original
binding and call a replacement dynamically if it does not. The
`--assume-unmodified-host-globals` option removes per-call checks only where
source analysis finds no possible global-object exposure or rebinding across
the compiled modules. Use it only if the host also guarantees the original
bindings at startup and does not replace or expose them for mutation during
callbacks or later invocations; it does not make JavaScript globals immutable.

To include multiple independent input files in one assembly, keep the default
entry first and supply each other file with `-a` or `--additional-input`. The second
positional argument remains the output directory:

```shell
jroc app.js out -a utilities.js -a features.js
dotnet out/app.dll
```

`app.js` is the default entry when the assembly runs, and its basename (`app`)
determines the assembly name and output filenames unless `--assemblyname`
overrides it. Additional files do not produce separate assemblies.

Use `-o` or `--output` to select the output directory with `--moduleid`.
JROC resolves the module ID from the current working directory.

To compile on .NET 11 for deployment to .NET 10, use
`jroc app.js out --target-framework net10.0` with a .NET 10 reference pack
available. See [Selecting the output framework](docs/NuGet.README.md#selecting-the-output-framework)
for pack discovery and how to supply a pack explicitly.

Use `jroc coverage app.js` to inspect direct IL, bound runtime intrinsics,
dynamic/fallback paths, and unsupported sites independently of correctness.
Use `--coverage-report <file>` to save the analysis to a file.
See [compilation-mode coverage](docs/compiler/CompilationCoverage.md) for the
source-site measurement and Test262 aggregation.

## Use JROC from .NET projects

The JROC packages serve different integration scenarios:

| Package | Use it when... |
|---|---|
| [`jroc`](https://www.nuget.org/packages/jroc) | You want the command-line compiler shown above. |
| [`Jroc.SDK`](https://www.nuget.org/packages/Jroc.SDK) | Your project should compile JavaScript as part of `dotnet build`. |
| [`Jroc.Core`](https://www.nuget.org/packages/Jroc.Core) | Your scripts are not known until runtime and need in-memory compilation. |
| [`Jroc.Runtime`](https://www.nuget.org/packages/Jroc.Runtime) | You need execution support for generated code; `Jroc.SDK` supplies it transitively. |

See the [JROC SDK documentation](docs/sdk/Index.md) for the three supported
workflows: command-line execution, MSBuild with generated typed `Import` / `Run`
APIs, and in-memory compilation. Keep JROC package versions aligned
when using more than one package in the same application.

## Compatibility and project status

JROC supports Windows, Linux, and macOS. It targets ECMAScript 2025.

JROC is experimental and does not yet support every JavaScript or Node.js
feature. In particular, `eval` is not currently supported.

- [ECMAScript support and Test262 conformance](docs/ECMA262/Index.md)
- [Node.js API support](docs/nodejs/Index.md)
- [Compiler architecture](docs/compiler/Index.md)
- [Runtime architecture](docs/runtime/Index.md)
- [Release notes](CHANGELOG.md)

Compiler errors go to standard error, and compilation failures return a
nonzero exit code.

## Contributing

Building JROC from source, running tests, validating packages, and publishing
releases are contributor and maintainer workflows. See
[Building and releasing JROC](docs/BuildingAndReleasing.md).

JROC is licensed under the [Apache License 2.0](LICENSE).
