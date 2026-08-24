# Domino

This sample demonstrates compiling and hosting a real npm package: `@mixmark-io/domino`.

The sample is self-contained directly in `samples/Domino`, which includes the C# console app plus the npm manifest/lock file used during build.

## Prerequisites

- .NET SDK (see repo root `global.json`)
- Node.js + npm

## Package entry resolution

This sample restores domino into `node_modules`, declares `@mixmark-io/domino` directly in `JrocCompile`, and relies on the SDK's default module-resolution base (`$(MSBuildProjectDirectory)`), so no extra module-resolution metadata is required.

## Running the sample

From `samples/Domino`:

- `dotnet run -c Release`

This will:

1. Run `npm ci` in the host project directory (to restore `@mixmark-io/domino`)
2. Compile `@mixmark-io/domino` via `Jroc.SDK` using the default same-directory module resolution
3. Import the generated `mixmark_io_domino` facade and print stats for `sample.html`

Expected output:

```text
title=JROC Domino Sample
elements=12
links=2
```

The SDK emits the normalized assembly/facade name
`mixmark-io.domino` / `mixmark_io_domino`. Package declaration metadata drives
the generated `Window`, `Document`, and collection contracts, so the host uses
normal C# properties and methods without runtime APIs, reflection, module ids,
or `dynamic`. This is general package declaration inference rather than a
Domino-specific handwritten contract.

Set `JROC_DOMINO_DIAG=1` and `JROC_DOMINO_HTML_PATH` to an unreadable path to
exercise the diagnostic failure path.
