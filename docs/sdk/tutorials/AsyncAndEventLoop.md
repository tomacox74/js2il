# Tutorial: async calls and event loop

JROC runs hosted JavaScript on an owning script thread and pumps its event
loop so Promises and timers can progress while the .NET host is idle.

## MSBuild-generated imports

For a module compiled as `HostedMath`:

```javascript
exports.addAsync = async (left, right) => left + right;
```

Call its generated method and await the result:

```csharp
using var exports = HostedMath.Import();
var result = await exports.AddAsync(1d, 2d);
Console.WriteLine(result);
```

Generated async exports use `Task` or `Task<T>`. Exact parameter and result
types depend on inference or package declarations; do not assume JavaScript
arithmetic alone guarantees `double` parameters or a `Task<double>` result.
Use the generated signature rather than declaring an alternative interface.

## In-memory compilation

When the script is supplied at runtime, access its async callable through the
loaded module:

```csharp
using Jroc.Runtime;

// module is the live result of CompileAndLoadModule(request).
var addAsync = (JsCallable)module.Exports.Get("addAsync")!;
var result = await addAsync.CallAsync<double>(1d, 2d);
```

Here the host explicitly requests the Promise result projection. See the
[in-memory tutorial](InMemoryCompileAndRun.md) for compilation and ownership.

## Running a whole script

Generated `HostedMath.Run(...)` calls, and the command-line program entry
point, execute the selected script and drain its asynchronous work before
completing. Use `Run` for whole-script execution; use `Import` or in-memory
loading when the host needs to retain exports and call them over time.

## Lifetime, timeouts, and cancellation

Keep the import or in-memory module alive until needed tasks settle. Promise
rejections fault their tasks. Disposing the owner while a bridge task is
pending faults it with `ObjectDisposedException`.

For a timeout on a host wait:

```csharp
using var exports = HostedMath.Import();
var result = await exports.AddAsync(1d, 2d)
    .WaitAsync(TimeSpan.FromSeconds(2));
```

A host timeout does not cancel JavaScript execution. Synchronous export calls
do not accept cancellation tokens. Do not treat task timeouts as isolation
or resource limits for untrusted scripts.
