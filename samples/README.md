# Samples

These samples demonstrate consuming **compiled** JavaScript modules as a .NET library via `Js2IL.Runtime` hosting APIs.

- [samples/Hosting.Basic](Hosting.Basic/README.md): load a compiled module and call exports.
- [samples/Hosting.Typed](Hosting.Typed/README.md): typed exports + constructors/instance handles.
- [samples/Hosting.Domino](Hosting.Domino/README.md): compile and host a real npm package (@mixmark-io/domino).

## How samples work

Each sample is split into two parts:

- `compiler/` – the JavaScript source inputs consumed during `dotnet build`. `Hosting.Domino` also keeps its npm manifest/lock file here so the host project can restore the package before compiling it.
- `host/` – a C# console app that restores `Js2IL.SDK` and `JavaScriptRuntime`, compiles the JavaScript input via `Js2ILCompile`, and calls into the resulting module assembly using `Js2IL.Runtime` hosting APIs.

No separate `js2il` CLI shell-out project is required.
