# Tutorial: lifetime and disposal

Ownership follows the chosen workflow:

| Entry point | Owner and completion |
|---|---|
| CLI / generated `Run(...)` | Executes the script, drains asynchronous work, and shuts down its runtime. No exports handle is returned. |
| Generated `Import()` | Returns a generated `IDisposable` exports contract. Dispose it to shut down that import's runtime. |
| `CompileAndLoadModule(...)` | Returns a disposable in-memory module owning both runtime and collectible load context. Dispose the module, not just a local reference to its exports. |

## Generated imports and derived values

```csharp
using var exports = HostedCounterModule.Import();
using var counter = exports.Counter.Construct(10);
Console.WriteLine(counter.Add(5));
```

The generated exports, object, array, constructor, and instance contracts
implement `IDisposable`. Dispose derived handles when finished, and keep the
root import alive while using them. A derived handle does not extend the root
runtime's lifetime.

Each `Import()` creates an isolated runtime and module cache. Values from
different imports cannot be mixed. Host calls are marshalled to the owning
script thread.

## In-memory modules

```csharp
using var module = JrocInMemoryCompiler.CompileAndLoadModule(request);
Console.WriteLine(module.Exports.Invoke("add", 1d, 2d));
```

The returned module is the lifetime boundary even when a separate local
variable holds `module.Exports`. After disposal, release references to the
module, loaded assembly, exports, and derived values so the collectible
context can unload. Disposal requests unloading; retained references prevent
actual collection.

## After disposal

Further calls through disposed exports or derived handles fail with
`ObjectDisposedException`. Pending Promise-to-Task bridges and queued
invocations that have not started are faulted during shutdown rather than
left waiting indefinitely.

Keep the owner alive until asynchronous results needed by the application
have settled. A scope ending immediately after scheduling work is not a
substitute for awaiting that work.
