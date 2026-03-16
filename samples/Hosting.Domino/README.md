# Hosting.Domino

This sample demonstrates compiling and hosting a real npm package: `@mixmark-io/domino`.

It is split into two parts:

- `compiler/` – the npm manifest/lock file plus restored package contents consumed during build.
- `host/` – a C# console app that restores `Js2IL.SDK` + `Js2IL.Runtime`, runs `npm ci`, compiles the package entry into a .NET assembly, and parses an included HTML document.

## Prerequisites

- .NET SDK (see repo root `global.json`)
- Node.js + npm

## Package entry resolution

This sample restores domino into `compiler\node_modules`, declares `@mixmark-io/domino` directly in `Js2ILCompile`, and sets `ModuleResolutionBaseDirectory` to `compiler\` so the SDK can resolve the package entrypoint using Node-style package resolution.

## Running the sample

From `samples/Hosting.Domino/host`:

- `dotnet run -c Release`

This will:

1. Run `npm ci` in `..\compiler` (to restore `@mixmark-io/domino`)
2. Compile `@mixmark-io/domino` via `Js2IL.SDK`, resolving it from `..\compiler`
3. Run the host which loads the compiled module and prints stats for `sample.html`
