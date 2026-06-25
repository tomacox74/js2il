# Tutorial: Lifetime + disposal

Hosting introduces two distinct lifetimes:

1. The **module runtime instance** (script thread + runtime state)
2. Individual **JS handles** (object instances, constructors, etc.)

## What to dispose

- Always dispose the object returned by `JsEngine.LoadModule(...)`.
  - For typed hosting, that’s the generated exports interface (it must implement `IDisposable`).
  - For dynamic hosting, that’s the dynamic exports proxy.
- Dispose handle proxies (`IJsHandle`) when you’re done with them.

## Typed example

```csharp
using var exports = JsEngine.LoadModule<IMyExports>();

using var counter = exports.Counter.Construct(10);
Console.WriteLine(counter.Add(5));

// Disposing the handle makes it unusable.
counter.Dispose();
```

## What happens after Dispose?

- If you dispose the **exports proxy**, the runtime instance is shut down.
- Further calls on that exports proxy throw `ObjectDisposedException`.
- Handles are also tied to that runtime; if the runtime is shut down, handle calls will fail.

## Common pitfalls

- Forgetting to dispose exports in long-running processes can leak a background script thread.
- Disposing a handle and then reusing it later will throw; keep handle ownership clear.

## Patterns

- Prefer `using var exports = ...;` at the top-level scope.
- Prefer `using var handle = ...;` for object instances.
- If you store handles, treat them like any other disposable resource.
