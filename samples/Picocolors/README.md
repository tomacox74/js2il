# Picocolors

This sample demonstrates compiling and hosting a real npm package: `picocolors`.

[picocolors](https://www.npmjs.com/package/picocolors) is a tiny terminal-color library that wraps strings in ANSI escape codes. The sample compiles the package to a .NET assembly via `Jroc.SDK`, loads it at runtime, and calls several color/style functions from C#.

## Prerequisites

- .NET SDK (see repo root `global.json`)
- Node.js + npm

## Running the sample

From `samples/Picocolors`:

```
dotnet run -c Release
```

This will:

1. Run `npm ci` to restore `picocolors` into `node_modules`
2. Compile `picocolors` to `picocolors.dll` via `Jroc.SDK`
3. Load the compiled module and call several color functions, printing the results

## Expected output

When a color-capable terminal is detected the strings include ANSI escape codes; when running without a TTY (e.g. piped output) picocolors returns the strings unchanged:

```
red=ERROR: something went wrong
green=OK: all systems go
yellow=WARN: check your config
cyan=INFO: picocolors via JROC
bold=Bold text
done
```
