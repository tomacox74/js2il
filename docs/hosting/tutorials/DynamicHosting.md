# Tutorial: Dynamic hosting

Dynamic hosting is useful when:

- you don’t want to reference generated contract types at compile time,
- you load compiled assemblies dynamically (plugins), or
- you are exploring/debugging a module.

## Load by (Assembly, moduleId)

```csharp
using Js2IL.Runtime;
using System.Reflection;

var asm = Assembly.LoadFrom("path\\to\\compiled.dll");

// The returned object is an IDisposable dynamic exports proxy.
using dynamic exports = JsEngine.LoadModule(asm, moduleId: "math");

Console.WriteLine((string)exports.version);
Console.WriteLine((double)exports.add(1, 2));
```

## Nested object graphs and returned functions

Dynamic hosting wraps non-primitive return values so you can keep using dynamic member access and invocation:

```csharp
using dynamic exports = JsEngine.LoadModule(asm, "nestedReturn");

dynamic win = exports.getWindow();
Console.WriteLine((string)win.document.title);

// Returned function values are invokable.
dynamic inc = exports.getIncrementer();
Console.WriteLine((double)inc(1));
```

## Passing values back into JS

If you pass a value that came from the hosting layer back into another hosting call, it is automatically unwrapped to the underlying JS value before the invocation.
This lets patterns like this work:

```csharp
dynamic win = exports.getWindow();
Console.WriteLine((string)exports.getTitle(win));
```

## Member writes

- The **exports object itself** is mutable via the hosting API.
- Writes are marshalled to the owning script thread, just like reads and calls.
- Objects returned from exports are also dynamic value proxies that support setting members:

```csharp
exports.mutableValue = 123; // writes module.exports.mutableValue on the script thread

dynamic obj = exports.getSomeObject();
obj.count = 123; // marshalled to the script thread
```

## Error handling

Dynamic calls can throw the same hosting exceptions as typed calls.
See [Diagnostics + exceptions](DiagnosticsAndExceptions.md).
