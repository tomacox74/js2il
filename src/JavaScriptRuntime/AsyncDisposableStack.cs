using System;
using System.Collections.Generic;

namespace JavaScriptRuntime;

/// <summary>
/// ECMAScript <c>AsyncDisposableStack</c>, which owns an ordered collection of
/// synchronous or asynchronous cleanup callbacks.
/// </summary>
[IntrinsicObject("AsyncDisposableStack")]
public sealed class AsyncDisposableStack : JsObject
{
    private enum DisposalKind
    {
        Use,
        Adopt,
        Defer
    }

    private readonly record struct DisposableResource(
        DisposalKind Kind,
        object? Value,
        object Callback);

    private readonly List<DisposableResource> _resources = [];
    private bool _disposed;

    internal static object Prototype
        => RuntimeIntrinsics.Current.GetOrCreate(
            RuntimeIntrinsicSlot.AsyncDisposableStackPrototype,
            static () => new JsObject());

    public AsyncDisposableStack()
    {
        PrototypeChain.InitializePrototype(this, Prototype);
    }

    public AsyncDisposableStack(object? _)
        : this()
    {
    }

    internal static void InitializeIntrinsicSurface(object objectPrototype)
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        GlobalThis.ConfigureBuiltinFunctionObject(typeof(AsyncDisposableStack));
        PrototypeChain.SetPrototype(Prototype, objectPrototype);

        DefineDataProperty(typeof(AsyncDisposableStack), "prototype", Prototype, configurable: false, writable: false);
        DefineDataProperty(typeof(AsyncDisposableStack), "name", "AsyncDisposableStack", configurable: true, writable: false);
        DefineDataProperty(typeof(AsyncDisposableStack), "length", 0d, configurable: true, writable: false);
        DefineDataProperty(Prototype, "constructor", typeof(AsyncDisposableStack));
        DefineFunction(Prototype, "use", (BuiltinFunction1)PrototypeUse, 1d);
        DefineFunction(Prototype, "adopt", (BuiltinFunction2)PrototypeAdopt, 2d);
        DefineFunction(Prototype, "defer", (BuiltinFunction1)PrototypeDefer, 1d);
        DefineFunction(Prototype, "move", (BuiltinFunction0)PrototypeMove, 0d);
        BuiltinFunction0 disposeAsync = PrototypeDisposeAsync;
        DefineFunction(Prototype, "disposeAsync", disposeAsync, 0d);
        DefineDataProperty(Prototype, Symbol.asyncDispose.DebugId, disposeAsync);
        DefineDataProperty(Prototype, Symbol.toStringTag.DebugId, "AsyncDisposableStack", configurable: true, writable: false);

        BuiltinFunction0 disposedGetter = PrototypeDisposed;
        Function.InitializeFunctionInstance(
            disposedGetter,
            0d,
            "get disposed",
            requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(disposedGetter));
        Function.MarkUndefinedPrototype(disposedGetter);
        PropertyDescriptorStore.DefineOrUpdate(Prototype, "disposed", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Accessor,
            Enumerable = false,
            Configurable = true,
            Get = disposedGetter
        });
    }

    public object? use(object? value)
    {
        ThrowIfDisposed();
        if (value is null || value is JsNull)
        {
            return value;
        }

        if (!Proxy.IsObjectLikeValue(value))
        {
            throw new TypeError("AsyncDisposableStack.prototype.use requires an object");
        }

        var callback = ObjectRuntime.GetProperty(value, Symbol.asyncDispose.DebugId);
        if (callback is null || callback is JsNull)
        {
            callback = ObjectRuntime.GetProperty(value, Symbol.dispose.DebugId);
        }

        if (!CallableOperations.IsCallable(callback))
        {
            throw new TypeError("Disposable resource method is not callable");
        }

        _resources.Add(new DisposableResource(DisposalKind.Use, value, callback!));
        return value;
    }

    public object? adopt(object? value, object? onDisposeAsync)
    {
        ThrowIfDisposed();
        if (!CallableOperations.IsCallable(onDisposeAsync))
        {
            throw new TypeError("AsyncDisposableStack.prototype.adopt requires a callable disposer");
        }

        _resources.Add(new DisposableResource(DisposalKind.Adopt, value, onDisposeAsync!));
        return value;
    }

    public object? defer(object? onDisposeAsync)
    {
        ThrowIfDisposed();
        if (!CallableOperations.IsCallable(onDisposeAsync))
        {
            throw new TypeError("AsyncDisposableStack.prototype.defer requires a callable disposer");
        }

        _resources.Add(new DisposableResource(DisposalKind.Defer, null, onDisposeAsync!));
        return null;
    }

    public AsyncDisposableStack move()
    {
        ThrowIfDisposed();
        var result = new AsyncDisposableStack();
        result._resources.AddRange(_resources);
        _resources.Clear();
        _disposed = true;
        return result;
    }

    public object disposeAsync()
    {
        if (_disposed)
        {
            return Promise.resolve(null)!;
        }

        _disposed = true;
        return DisposeNext(_resources.Count - 1, hasPriorError: false, priorError: null);
    }

    private object DisposeNext(int index, bool hasPriorError, object? priorError)
    {
        if (index < 0)
        {
            return hasPriorError ? Promise.reject(priorError)! : Promise.resolve(null)!;
        }

        object? result;
        try
        {
            var resource = _resources[index];
            result = resource.Kind switch
            {
                DisposalKind.Use => CallableOperations.Call0(resource.Callback, resource.Value),
                DisposalKind.Adopt => CallableOperations.Call1(resource.Callback, null, resource.Value),
                _ => CallableOperations.Call0(resource.Callback, null)
            };
        }
        catch (Exception exception)
        {
            return DisposeNext(
                index - 1,
                hasPriorError: true,
                CombineErrors(hasPriorError, priorError, GetThrownValue(exception)));
        }

        var promise = (Promise)Promise.resolve(result)!;
        BuiltinFunction1 fulfilled = (_, _) => DisposeNext(index - 1, hasPriorError, priorError);
        BuiltinFunction1 rejected = (_, reason) => DisposeNext(
            index - 1,
            hasPriorError: true,
            CombineErrors(hasPriorError, priorError, reason));
        return promise.then(fulfilled, rejected)!;
    }

    private static object? CombineErrors(bool hasPriorError, object? priorError, object? newError)
        => !hasPriorError
            ? newError!
            : new SuppressedError(newError, priorError, null);

    private static object? GetThrownValue(Exception exception)
        => exception is JsThrownValueException thrown ? thrown.Value : exception.InnerException ?? exception;

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ReferenceError("AsyncDisposableStack is already disposed");
        }
    }

    private static AsyncDisposableStack GetReceiver(object? thisArgument, string method)
        => thisArgument as AsyncDisposableStack
            ?? throw new TypeError($"AsyncDisposableStack.prototype.{method} called on incompatible receiver");

    private static object? PrototypeUse(object? thisArgument, object? value)
        => GetReceiver(thisArgument, "use").use(value);

    private static object? PrototypeAdopt(object? thisArgument, object? value, object? onDisposeAsync)
        => GetReceiver(thisArgument, "adopt").adopt(value, onDisposeAsync);

    private static object? PrototypeDefer(object? thisArgument, object? onDisposeAsync)
        => GetReceiver(thisArgument, "defer").defer(onDisposeAsync);

    private static object? PrototypeMove(object? thisArgument)
        => GetReceiver(thisArgument, "move").move();

    private static object? PrototypeDisposeAsync(object? thisArgument)
        => GetReceiver(thisArgument, "disposeAsync").disposeAsync();

    private static object? PrototypeDisposed(object? thisArgument)
        => GetReceiver(thisArgument, "disposed")._disposed;

    private static void DefineFunction(object target, string key, Delegate value, double length, string? name = null)
    {
        Function.InitializeFunctionInstance(
            value,
            length,
            name ?? key,
            requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(value));
        Function.MarkUndefinedPrototype(value);
        DefineDataProperty(target, key, value);
    }

    private static void DefineDataProperty(
        object target,
        string key,
        object? value,
        bool configurable = true,
        bool writable = true)
    {
        PropertyDescriptorStore.DefineOrUpdate(target, key, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = configurable,
            Writable = writable,
            Value = value
        });
    }
}
