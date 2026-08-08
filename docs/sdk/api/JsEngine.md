# API: JsEngine

Namespace: `Jroc.Runtime` (assembly: `JavaScriptRuntime.dll` / NuGet package: `Jroc.Runtime`)

`JsEngine` is the public entry point for hosting compiled JROC assemblies.

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

- Binary-compatible dynamic API retained with its original
  `IDisposable` return type.
- The runtime object is still a dynamic exports proxy and can be assigned to
  `dynamic`.

## LoadDynamicModule(Assembly compiledAssembly, string moduleId)

```csharp
public static JsDynamicExports LoadDynamicModule(
    Assembly compiledAssembly,
    string moduleId)
```

- Preferred dynamic / reflection-friendly API for new source.
- Exposes `JsDynamicExports.Value`, `Get(...)`, and `Invoke(...)` directly.
- An overload accepts `JsModuleLoadOptions`.

## GetModuleIds(Assembly compiledAssembly)

```csharp
public static IReadOnlyList<string> GetModuleIds(Assembly compiledAssembly)
```

- Returns module ids present in a compiled JROC assembly.
- Prefer this over scanning types directly; compiled assemblies emitted by JROC include an assembly-level manifest via `[JsCompiledModule]`.
- Includes a back-compat fallback for older compiled assemblies.

## Threading model (high level)

Each load call creates a runtime instance with a dedicated script thread.
All calls are marshalled onto that script thread; calls from within the script thread execute directly.
Disposal faults queued calls and pending Promise bridge tasks rather than
leaving callers blocked.
