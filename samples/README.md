# Samples

These samples demonstrate consuming **compiled** JavaScript modules as a .NET library via `Jroc.Runtime` hosting APIs.

- [samples/Basic](Basic/README.md): load a compiled module and call exports.
- [samples/Typed](Typed/README.md): typed exports + constructors/instance handles.
- [samples/Domino](Domino/README.md): compile and host a real npm package (@mixmark-io/domino).

## How samples work

Most samples are split into two parts:

- `compiler/` – the JavaScript source inputs consumed during `dotnet build`.
- `host/` – a C# console app that restores `Jroc.SDK` and `Jroc.Runtime`, compiles the JavaScript input via `JrocCompile`, and calls into the resulting module assembly using `Jroc.Runtime` hosting APIs.

`Domino` and `Picocolors` are the exceptions: they keep `package.json` / `package-lock.json` next to the `.csproj`, run `npm ci` in place, and compile the npm package directly by module id with the SDK defaults.
