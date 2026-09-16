# API: generated handles and constructors

MSBuild-generated imports use contracts to represent non-primitive JavaScript
values. These types are emitted into the compiled assembly alongside `Import`
and `Run`; the host does not implement them itself.

## Generated facade handles

Generated facade contracts use only BCL and generated assembly types:

```csharp
public interface IExports : IDisposable
{
    ICounterConstructor Counter { get; }
}

public interface ICounterConstructor : IDisposable
{
    ICounter Construct(params object?[] args);
    string Description { get; }
}

public interface ICounter : IDisposable
{
    double Add(object delta);
}
```

Object, array, constructor, and instance proxies are bound to the owning
`Import()` runtime. Repeated access to the same JavaScript value through one
runtime returns the same generated proxy for a given contract, so aliases and
cycles do not duplicate host identity. Disposing the root exports object shuts
down the runtime; later handle access throws `ObjectDisposedException`.

Generated array contracts expose:

```csharp
double Length { get; set; }
object? Get(double index);
void Set(double index, object? value);
bool HasIndex(double index);
double Push(params object?[] values);
```

`HasIndex` lets consumers distinguish sparse holes from present values whose
value is `undefined`/`null` in the public projection.

Generated object contracts expose known properties, methods, and accessors plus
`GetDynamicProperty`, `SetDynamicProperty`, and `HasDynamicProperty` for
unknown/computed names. Callable return values use a generated `ICallable`
contract with `Invoke(params object?[] args)`. Anonymous classes returned from
functions use a generated `IConstructor` fallback whose `Construct(...)`
returns the generated `IObject` fallback.

## Passing proxies back into JS

If you call into JS and pass arguments that were previously returned via hosting proxies, the hosting layer unwraps them back to the underlying JS value before invoking.
This avoids accidentally passing the proxy object itself into JS APIs.

Wrappers from different imports cannot be mixed. Generated members preserve
their JavaScript receiver and marshal reads, writes, and calls onto the owning
script thread.

Runtime-supplied scripts instead return dynamic values through the
[in-memory API](InMemoryCompiler.md#export-access); those are not generated
compile-time contracts.
