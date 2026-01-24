# Hosting.Typed

Typed hosting sample demonstrating:
- typed `module.exports` projection
- exporting a class and constructing it via `IJsConstructor<T>`
- instance handles via `IJsHandle`

## Layout

- `compiler/` – MSBuild `.proj` that runs `js2il` to produce `out/HostedCounterModule.dll`
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

Prints version and counter values.
