# Basic

Minimal end-to-end hosting sample:
- compile a JS module to a .NET assembly during `dotnet build` via `Jroc.SDK`
- import the compiled module through the generated `HostedMathModule.Import()` facade
- call generated export members without `JsEngine` or runtime-specific host APIs

## Layout

- `compiler/JavaScript/` – source JS module compiled by the host project
- `host/` – C# console app that restores `Jroc.SDK`, builds the JS module, and
  calls exports through the generated facade; the runtime is transitive

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

```text
version=1.0.0
1+2=3
```
