# Hosting.Basic

Minimal end-to-end hosting sample:
- compile a JS module to a .NET assembly during `dotnet build` via `Js2IL.SDK`
- load that compiled assembly and call `module.exports` via typed C# interfaces

## Layout

- `compiler/JavaScript/` – source JS module compiled by the host project
- `host/` – C# console app that restores `Js2IL.SDK` + `Js2IL.Runtime`, builds the JS module, and calls exports

## Prerequisites

- .NET 10 SDK

## Build

```powershell
dotnet build .\host
```

This restores the NuGet packages, compiles `compiler\JavaScript\HostedMathModule.js`, and copies the generated module outputs next to the host executable.

## Run

```powershell
dotnet run --project .\host
```

## Expected output

Prints the module version and a sum result.
