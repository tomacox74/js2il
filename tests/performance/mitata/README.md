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

To compile and run the benchmark through js2il:

```powershell
npm run bench:string-width:js2il
```

Set `JS2IL` to use a specific js2il executable or `Js2IL.dll`; otherwise the script runs the repo's `src\Cli` project.
