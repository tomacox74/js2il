# Hosting.Domino

This sample demonstrates compiling and hosting a real npm package: `@mixmark-io/domino`.

It is split into two parts:

- `compiler/` – the npm manifest/lock file plus restored package contents consumed during build.
- `host/` – a C# console app that restores `Js2IL.SDK` + `JavaScriptRuntime`, runs `npm ci`, compiles the package entry into a .NET assembly, and parses an included HTML document.

## Prerequisites

- .NET SDK (see repo root `global.json`)
- Node.js + npm

## Package entry resolution

This sample restores domino into `compiler\node_modules`, points `Js2ILCompile` at the resolved `lib\index.js` entry file, and sets `RootModuleId="@mixmark-io/domino"` so the host can still load the package by module id.

## Running the sample

From `samples/Hosting.Domino/host`:

- `dotnet run -c Release`

This will:

1. Run `npm ci` in `..\compiler` (to restore `@mixmark-io/domino`)
2. Compile `compiler\node_modules\@mixmark-io\domino\lib\index.js` via `Js2IL.SDK` with `RootModuleId="@mixmark-io/domino"`
3. Run the host which loads the compiled module and prints stats for `sample.html`
