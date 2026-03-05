# API: Handles + constructors

Hosting uses proxies to represent non-primitive JS values.

## IJsHandle

```csharp
public interface IJsHandle : IDisposable
{
}
```

- Marker interface for a proxy that represents a JS value living on the script thread.
- When you dispose a handle proxy, further calls on that proxy throw `ObjectDisposedException`.

## IJsConstructor<T>

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

## Typed vs dynamic member access

- **Typed** handle proxies map interface methods/properties to JS member reads and method calls.
- **Dynamic** values are wrapped so `dynamic` member access and invocation uses JS semantics.
