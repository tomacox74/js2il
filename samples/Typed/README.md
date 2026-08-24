# Typed

Typed hosting sample demonstrating:
- importing through the generated `HostedCounterModule.Import()` facade
- constructing an exported class through generated constructor and instance contracts
- awaiting an exported async function as a normal `Task<T>`
- using no `JsEngine`, runtime-specific host APIs, or handwritten contracts

## Layout

- `compiler/JavaScript/` – source JS module compiled by the host project
- `host/` – C# console app that restores `Jroc.SDK`, builds the JS module, and
  calls exports through generated contracts; the runtime is transitive

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

```text
version=1.2.3
add(1,2)=3
counter.add(5)=15
counter.value=15
addAsync(1,2)=3
created.add(1)=3
```
