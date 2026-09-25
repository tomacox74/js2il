# JROC

[![NuGet](https://img.shields.io/nuget/v/jroc.svg)](https://www.nuget.org/packages/jroc/)

JROC is a JavaScript-to-.NET compiler. It compiles JavaScript source directly
to .NET IL and produces ordinary .NET assemblies that run with the `dotnet`
host.

Unlike an embedded JavaScript interpreter, JROC performs ahead-of-time
compilation: JavaScript is parsed, analyzed, lowered, and emitted as a .NET
assembly before the program runs. JROC includes a JavaScript runtime and
support for a growing set of Node.js APIs, making it possible to bring
JavaScript applications and libraries into the .NET ecosystem.

JROC currently targets ECMAScript 2025 and .NET 10. The project is under active
development; compatibility is broad but not yet complete.

## Install JROC

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0),
then install JROC from NuGet as a global .NET tool:

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
- `hello.runtimeconfig.json` — configuration used by the `dotnet` host.
- `JavaScriptRuntime.dll` — the runtime support required by the generated
  assembly.

If no output directory is supplied, JROC writes the generated files next to
the input file.

## Command-line usage

```text
jroc <InputFile> [<OutputPath>] [options]
jroc <InputFile> [<OutputPath>] -e <file> [-e <file> ...] [options]
jroc --moduleid <ModuleId> [<OutputPath>] [options]
```

| Option | Description |
|---|---|
| `-i`, `--input <file>` | Compile a JavaScript file. The positional form is also supported. |
| `-e`, `--additional-input <file>` | Add another JavaScript input to the same assembly; repeat for more files. Cannot be used with `--moduleid`. |
| `--moduleid <id>` | Compile an npm/CommonJS module ID, such as `turndown` or `@scope/pkg`. |
| `-o`, `--output <directory>` | Select the output directory. |
| `--assemblyname <name>` | Set the generated assembly identity and artifact basename. |
| `-v`, `--verbose` | Print compiler diagnostics. |
| `--diagnostic-file <path>` | Write compiler diagnostics to a text file. |
| `-a`, `--analyzeunused` | Report unused functions, properties, and variables. |
| `--pdb` | Emit Portable PDB debug symbols. |
| `--version` | Print the installed JROC version. |
| `-h`, `-?`, `--help` | Show command-line help. |

For example:

```shell
jroc app.js --output out --pdb
dotnet out/app.dll
```

To include multiple independent input files in one assembly, keep the default
entry first and supply each other file with `-e` or `--additional-input`. The second
positional argument remains the output directory:

```shell
jroc app.js out -e utilities.js -e features.js
dotnet out/app.dll
```

`app.js` is the default entry when the assembly runs, and its basename (`app`)
determines the assembly name and output filenames unless `--assemblyname`
overrides it. Additional files do not produce separate assemblies.

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

JROC is experimental and does not yet support every JavaScript or Node.js
feature. In particular, `eval` is not currently supported.

- [ECMAScript support and Test262 conformance](docs/ECMA262/Index.md)
- [Node.js API support](docs/nodejs/Index.md)
- [Compiler architecture](docs/compiler/Index.md)
- [Runtime architecture](docs/runtime/Index.md)
- [Release notes](CHANGELOG.md)

Errors and diagnostics are written to standard error, and known compilation
failures return a non-zero exit code.

## Contributing

Building JROC from source, running tests, validating packages, and publishing
releases are contributor and maintainer workflows. See
[Building and releasing JROC](docs/BuildingAndReleasing.md).

JROC is licensed under the [Apache License 2.0](LICENSE).
