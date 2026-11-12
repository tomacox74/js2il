# Tutorial: compile and run on the command line

Use the command-line workflow to turn a JavaScript file into a runnable .NET
program without creating a C# host project.

## 1. Install JROC

Install the [.NET 10 or .NET 11 SDK](https://dotnet.microsoft.com/download/dotnet),
then install the global tool:

```shell
dotnet tool install --global jroc
jroc --version
```

For subsequent updates, use `dotnet tool update --global jroc`.

## 2. Create a script

Save this as `hello.js`:

```javascript
const name = "JROC";
console.log(`Hello from ${name}!`);
```

## 3. Compile and run

```shell
jroc hello.js out
dotnet out/hello.dll
```

Expected output:

```text
Hello from JROC!
```

Keep `hello.dll`, `hello.runtimeconfig.json`, and `JavaScriptRuntime.dll`
together in the output directory. The generated `hello.runtimeconfig.json`
defaults to the runtime version that ran JROC. For example, on a
machine with only .NET 11 installed, JROC runs on .NET 11 and generates a
program that targets .NET 11. If no output directory is specified, JROC writes
next to the input file.

To choose a deployment target explicitly, pass `--target-framework net10.0` or
`--target-framework net11.0`. JROC uses the reference pack for that target.
See [Selecting the output framework](../../NuGet.README.md#selecting-the-output-framework)
for pack discovery and deployment examples.

To compile additional JavaScript files into the same assembly, use the
repeatable `-a` (or `--additional-input`) option:

```shell
jroc hello.js out -a helpers.js -a widgets.js
dotnet out/hello.dll
```

The first file (`hello.js`) remains the default entry and supplies the
assembly name (`hello.dll`) unless you pass `--assemblyname <name>`.
The optional second positional argument is still the output directory; extra
inputs must use `-a` or `--additional-input`. This option cannot be combined with
`--moduleid`.

## Diagnostics and debugging

```shell
jroc hello.js out --pdb -v
```

`--pdb` emits Portable PDB symbols referring to the original source path;
`-v` enables compiler diagnostics. Use `--diagnostic-file <path>` to capture
diagnostics and `jroc --help` for CLI options. Keep the source and PDB available
for source-level debugging.

## Choose a .NET integration workflow

If you want to call JavaScript from a .NET project rather than run it as a
standalone program, use:

- [MSBuild integration](MSBuildBuildTask.md) for scripts known at build time;
  call the resulting assembly's generated `Import()` / `Run()` APIs.
- [In-memory compilation](InMemoryCompileAndRun.md) for scripts supplied at
  runtime.
