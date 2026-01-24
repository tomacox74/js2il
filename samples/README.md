# Samples

These samples demonstrate consuming **compiled** JavaScript modules as a .NET library via `Js2IL.Runtime` hosting APIs.

- [samples/Hosting.Basic](Hosting.Basic/README.md): load a compiled module and call exports.
- [samples/Hosting.Typed](Hosting.Typed/README.md): typed exports + constructors/instance handles.

## How samples work

Each sample is split into two parts:

- `compiler/` – an MSBuild `.proj` that invokes the `js2il` compiler (assumes `js2il` is installed as a global tool)
- `host/` – a C# console app that loads the compiled module DLL and calls into it using `Js2IL.Runtime` hosting APIs
