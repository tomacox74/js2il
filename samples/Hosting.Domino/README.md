# Hosting.Domino

This sample demonstrates compiling and hosting a real npm package: `@mixmark-io/domino`.

The sample is self-contained under `host/`, which includes the C# console app plus the npm manifest/lock file used during build.

## Prerequisites

- .NET SDK (see repo root `global.json`)
- Node.js + npm

## Package entry resolution

This sample restores domino into `host\node_modules`, declares `@mixmark-io/domino` directly in `Js2ILCompile`, and relies on the SDK's default module-resolution base (`$(MSBuildProjectDirectory)`), so no extra module-resolution metadata is required.

## Running the sample

From `samples/Hosting.Domino/host`:

- `dotnet run -c Release`

This will:

1. Run `npm ci` in the host project directory (to restore `@mixmark-io/domino`)
2. Compile `@mixmark-io/domino` via `Js2IL.SDK` using the default same-directory module resolution
3. Run the host which loads the compiled module and prints stats for `sample.html`
