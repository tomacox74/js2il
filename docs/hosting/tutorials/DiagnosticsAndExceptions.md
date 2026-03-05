# Tutorial: Diagnostics + exceptions

The hosting layer translates internal exceptions to a small set of stable, user-facing exception types.

## The main exception families

- **Module load failures** → `JsModuleLoadException`
  - Example: module id not found, module throws during initialization.
- **Contract projection failures** → `JsContractProjectionException`
  - Example: your contract expects `Add(x,y)` but the export isn’t callable, or a member is missing.
- **Invocation failures** → `JsInvocationException`
  - Example: calling an export throws.
- **JavaScript throws** → `JsErrorException`
  - Wraps a JS `Error` (name/message/stack when available).

## Module load example

```csharp
try
{
    using var exports = JsEngine.LoadModule<IMyExports>("boom");
}
catch (JsModuleLoadException ex)
{
    Console.WriteLine($"module={ex.ModuleId}");
    Console.WriteLine(ex.InnerException);
}
```

## Invocation example

```csharp
try
{
    using var exports = JsEngine.LoadModule<IMyExports>("throws");
    exports.Boom();
}
catch (JsInvocationException ex)
{
    Console.WriteLine($"module={ex.ModuleId} member={ex.MemberName}");
    if (ex.InnerException is JsErrorException js)
    {
        Console.WriteLine($"js.name={js.JsName}");
        Console.WriteLine($"js.message={js.JsMessage}");
        Console.WriteLine(js.JsStack);
    }
}
```

## Debugging tips

- If you want source file/line information when debugging hosted code, compile your module with `--pdb` and keep the generated `.pdb` next to the compiled `.dll`.
- If you need to understand what module ids exist in an assembly, use `JsEngine.GetModuleIds(assembly)`.
- If you see `JsContractProjectionException`, confirm your contract matches the JS exports shape.
