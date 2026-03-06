# API: JsEngine

Namespace: `Js2IL.Runtime` (assembly: `JavaScriptRuntime.dll` / NuGet package: `JavaScriptRuntime`)

`JsEngine` is the public entry point for hosting compiled JS2IL assemblies.

## LoadModule<TExports>()

```csharp
public static TExports LoadModule<TExports>() where TExports : class
```

- Uses `[JsModule("<moduleId>")]` on `TExports` to resolve the module id.
- Uses `typeof(TExports).Assembly` as the target compiled module assembly.
- Intended for **generated contracts**.

If the attribute is missing, a `JsContractProjectionException` is thrown with guidance.

## LoadModule<TExports>(string moduleId)

```csharp
public static TExports LoadModule<TExports>(string moduleId) where TExports : class
```

- Loads `moduleId` from `typeof(TExports).Assembly`.
- Requires `TExports` to implement `IDisposable` (so the runtime can be shut down deterministically).

## LoadModule(Assembly compiledAssembly, string moduleId)

```csharp
public static IDisposable LoadModule(Assembly compiledAssembly, string moduleId)
```

- Dynamic / reflection-friendly API.
- Returns an **IDisposable dynamic exports proxy** (can be used via `dynamic`).

## GetModuleIds(Assembly compiledAssembly)

```csharp
public static IReadOnlyList<string> GetModuleIds(Assembly compiledAssembly)
```

- Returns module ids present in a compiled JS2IL assembly.
- Prefer this over scanning types directly; compiled assemblies emitted by JS2IL include an assembly-level manifest via `[JsCompiledModule]`.
- Includes a back-compat fallback for older compiled assemblies.

## Threading model (high level)

Each `LoadModule(...)` call creates a runtime instance with a dedicated script thread.
All calls are marshalled onto that script thread; calls from within the script thread execute directly.
