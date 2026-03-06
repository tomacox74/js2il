# Tutorial: Async exports + event loop

JS2IL’s runtime includes a per-instance event-loop pump so that Promises and timers can make progress even when the host is idle.

## Promise → Task mapping

If your JS export returns a Promise:

```js
async function addAsync(x, y) {
  return x + y;
}
module.exports = { addAsync };
```

The generated contract will use `Task<T>`:

```csharp
Task<double> AddAsync(double x, double y);
```

At runtime:

- If the JS value is a `Promise`, it is bridged to a `Task`.
- If the JS value is not a Promise but the contract expects a `Task`, it is treated as already completed.

## Host-side usage

```csharp
using var exports = JsEngine.LoadModule<IMyExports>();
var result = await exports.AddAsync(1, 2);
```

## Event loop pumping model

Each module runtime instance:

- owns a dedicated script thread,
- processes host invocations serially, and
- periodically pumps the JS event loop (microtasks + timers) even if no new host calls arrive.

This prevents deadlocks where a Promise resolves via `setTimeout` but nothing is driving the runtime forward.

## Timeouts and cancellation

Today, hosting APIs are synchronous (for non-Task signatures) and do not accept cancellation tokens.
If you need timeouts, apply them at the Task boundary:

```csharp
using var exports = JsEngine.LoadModule<IMyExports>();

var task = exports.AddAsync(1, 2);
var completed = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(2)));
if (completed != task) throw new TimeoutException();

Console.WriteLine(await task);
```
