# Hosting.Typed

Typed hosting sample demonstrating:
- typed `module.exports` projection
- exporting a class and constructing it via `IJsConstructor<T>`
- instance handles via `IJsHandle`

## Layout

- `compiler/JavaScript/` – source JS module compiled by the host project
- `host/` – C# console app that restores `Js2IL.SDK` + `Js2IL.Runtime`, builds the JS module, and calls exports

## Prerequisites

- .NET 10 SDK

## Build

```powershell
dotnet build .\host
```

This restores the NuGet packages, compiles `compiler\JavaScript\HostedCounterModule.js`, and copies the generated module outputs next to the host executable.

## Run

```powershell
dotnet run --project .\host
```

## Expected output

Prints version and counter values.
