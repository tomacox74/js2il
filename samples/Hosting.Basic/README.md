# Hosting.Basic

Minimal end-to-end hosting sample:
- compile a JS module to a .NET assembly using `js2il` (global tool)
- load that compiled assembly and call `module.exports` via typed C# interfaces

## Layout

- `compiler/` – MSBuild `.proj` that runs `js2il` to produce `out/HostedMathModule.dll`
- `host/` – C# console app that loads the compiled assembly and calls exports

## Prerequisites

- .NET 10 SDK
- `js2il` installed as a global tool:

```powershell
dotnet tool install --global js2il --version 0.7.3
```

## Build

```powershell
dotnet build .\host
```

## Run

```powershell
dotnet run --project .\host
```

## Expected output

Prints the module version and a sum result.
