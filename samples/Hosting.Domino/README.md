# Hosting.Domino

This sample demonstrates compiling and hosting a real npm package: `@mixmark-io/domino`.

It is split into two parts:

- `compiler/` – restores the npm package and uses `js2il` to compile the package entry `index.js` into a .NET assembly.
- `host/` – a C# console app that loads the compiled module and parses an included HTML document, printing simple stats.

## Prerequisites

- .NET SDK (see repo root `global.json`)
- Node.js + npm
- `js2il` installed as a global tool (`dotnet tool install -g js2il`)

## Package entry resolution

This sample compiles domino via `--moduleid @mixmark-io/domino`, which uses Node/CommonJS `package.json` resolution (`exports` / `main` / `index.js`) at compile time.

## Running the sample

From `samples/Hosting.Domino/host`:

- `dotnet run -c Release`

This will:

1. Run `npm ci` in `compiler/` (to restore `@mixmark-io/domino`)
2. Compile domino via `js2il --moduleid @mixmark-io/domino`
3. Run the host which loads the compiled module and prints stats for `sample.html`
