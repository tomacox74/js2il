# Samples

These samples demonstrate consuming **compiled** JavaScript modules as a .NET library via `Js2IL.Runtime` hosting APIs.

- [samples/Hosting.Basic](Hosting.Basic/README.md): load a compiled module and call exports.
- [samples/Hosting.Typed](Hosting.Typed/README.md): typed exports + constructors/instance handles.
- [samples/Hosting.Domino](Hosting.Domino/README.md): compile and host a real npm package (@mixmark-io/domino).

## How samples work

Each sample is split into two parts:

- `compiler/` – an MSBuild `.proj` that invokes the `js2il` compiler. In a repo checkout it defaults to `dotnet run --project Js2IL/Js2IL.csproj`, falling back to the global `js2il` tool when the local project is not available.
- `host/` – a C# console app that loads the compiled module DLL and calls into it using `Js2IL.Runtime` hosting APIs
