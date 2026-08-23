# API: Handles + constructors

Hosting uses proxies to represent non-primitive JS values.

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

## Advanced dynamic handles

The types below remain available for explicit `Jroc.Runtime` hosting. They are
not emitted in generated facade public signatures.

### JsCallable

JavaScript function values obtained through advanced dynamic hosting are
represented by the public `JsCallable` class.

```csharp
var result = callable.Call(1, 2);
var resultWithThis = callable.CallWithReceiver(receiver, 1, 2);
var asyncResult = await callable.CallAsync<double>(21);
var instance = callable.Construct("Ada");
var derivedInstance = callable.ConstructWithNewTarget(otherConstructor, "Ada");
```

`Name`, `Length`, `IsConstructor`, `GetProperty`, and `SetProperty` expose the
function surface. Repeated retrieval of the same callable from one runtime
returns the same wrapper reference. `JsCallable` is owned by the module runtime
and is not separately disposable. `ConstructWithNewTarget` requires a
constructable alternate target and reports JavaScript `TypeError` otherwise.

### IJsHandle

```csharp
public interface IJsHandle : IDisposable
{
}
```

- Marker interface for a proxy that represents a JS value living on the script thread.
- When you dispose a handle proxy, further calls on that proxy throw `ObjectDisposedException`.

### IJsConstructor<T>

Exported JS classes are represented as constructors:

```csharp
public interface IJsConstructor<out TInstance> : IJsHandle
    where TInstance : class
{
    TInstance Construct(params object?[] args);
}
```

Notes:

- `Construct(...)` pads missing args with `undefined` semantics.
- The returned instance is typically another handle proxy.

## Passing proxies back into JS

If you call into JS and pass arguments that were previously returned via hosting proxies, the hosting layer unwraps them back to the underlying JS value before invoking.
This avoids accidentally passing the proxy object itself into JS APIs.

The same rule applies to `JsCallable`, preserving strict callback identity.
Wrappers from different module runtimes cannot be mixed.

## Typed vs dynamic member access

- **Typed** handle proxies map interface methods/properties to JS member reads and method calls.
- **Dynamic** values are wrapped so `dynamic` member access and invocation uses JS semantics.
