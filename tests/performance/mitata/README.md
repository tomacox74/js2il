# Mitata benchmarks

This folder contains focused JavaScript benchmarks that use [mitata](https://github.com/evanwashere/mitata), following the structure of Bun's `bench` folder.

## Setup

```powershell
npm install
```

## Run

```powershell
npm run bench:string-width
```

To compile and run the benchmark through jroc:

```powershell
npm run bench:string-width:jroc
```

Set `JROC` to use a specific jroc executable or `Jroc.dll`; otherwise the script runs the repo's `src\Cli` project.

## Managed runtime comparisons

The default release suite compares Node, JROC, ClearScript, Jint, YantraJS, and Okojo
using `runtime-baseline`, a dependency-free benchmark. Managed runtimes execute an
import-free bundle of the same benchmark body, generated with esbuild so their hosts do
not need Node.js module APIs. `string-width` remains available for Node and JROC.

```powershell
npm run bench:release
npm run bench:string-width:jint
```

Use repeated `--runtime` arguments to select a subset:

```powershell
node scripts/run-release-suite.mjs --runtime jroc --runtime jint --runtime yantrajs
```
