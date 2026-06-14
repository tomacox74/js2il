# Samples

These samples demonstrate consuming **compiled** JavaScript modules as a .NET library via `Jroc.Runtime` hosting APIs.

- [samples/Hosting.Basic](Hosting.Basic/README.md): load a compiled module and call exports.
- [samples/Hosting.Typed](Hosting.Typed/README.md): typed exports + constructors/instance handles.
- [samples/Hosting.Domino](Hosting.Domino/README.md): compile and host a real npm package (@mixmark-io/domino).

## How samples work

Most samples are split into two parts:

- `compiler/` – the JavaScript source inputs consumed during `dotnet build`.
- `host/` – a C# console app that restores `Jroc.SDK` and `Jroc.Runtime`, compiles the JavaScript input via `JrocCompile`, and calls into the resulting module assembly using `Jroc.Runtime` hosting APIs.

`Hosting.Domino` is the exception: its `host/` directory also contains `package.json` / `package-lock.json`, runs `npm ci` in place, and compiles `@mixmark-io/domino` directly by module id with the SDK defaults.

No separate `jroc` CLI shell-out project is required.
