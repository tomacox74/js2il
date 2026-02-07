# Hosting.Domino

This sample demonstrates compiling and hosting a real npm package: `@mixmark-io/domino`.

It is split into two parts:

- `compiler/` – restores the npm package and uses `js2il` to compile the package entry `index.js` into a .NET assembly.
- `host/` – a C# console app that loads the compiled module and parses an included HTML document, printing simple stats.

## Prerequisites

- .NET SDK (see repo root `global.json`)
- Node.js + npm
- `js2il` installed as a global tool (`dotnet tool install -g js2il`)

## Important: hardcoded package entry (current limitation)

Today the compiler does **not** resolve npm packages via `package.json` (e.g. `main` / `exports`) or handle module-type selection (CommonJS vs ESM) automatically.

For now, this sample hardcodes the path to domino’s entry file inside `node_modules`.

Future plan:
- Add first-class npm package support to the compiler (use `package.json` to locate the entry file, determine module type, and handle resolution).

## Running the sample

From `samples/Hosting.Domino/host`:

- `dotnet run -c Release`

This will:

1. Run `npm ci` in `compiler/` (to restore `@mixmark-io/domino`)
2. Compile the hardcoded domino `index.js` via `js2il`
3. Run the host which loads the compiled module and prints stats for `sample.html`
