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
        SyncUse,
        Adopt,
        Defer
    }

    private readonly record struct DisposableResource(
        DisposalKind Kind,
        object? Value,
        object? Callback);

    private List<DisposableResource> _resources = [];
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
        // AddDisposableResource captures the capability before GetDisposeMethod
        // can reenter JavaScript (for example, moving this stack from a getter).
        var resources = _resources;
        if (value is null || value is JsNull)
        {
            resources.Add(new DisposableResource(DisposalKind.Use, null, null));
            return value;
        }

        if (!Proxy.IsObjectLikeValue(value))
        {
            throw new TypeError("AsyncDisposableStack.prototype.use requires an object");
        }

        var callback = ObjectRuntime.GetProperty(value, Symbol.asyncDispose.DebugId);
        var kind = DisposalKind.Use;
        if (callback is null || callback is JsNull)
        {
            callback = ObjectRuntime.GetProperty(value, Symbol.dispose.DebugId);
            kind = DisposalKind.SyncUse;
        }

        if (!CallableOperations.IsCallable(callback))
        {
            throw new TypeError("Disposable resource method is not callable");
        }

        resources.Add(new DisposableResource(kind, value, callback));
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
        result._resources = _resources;
        _resources = [];
        _disposed = true;
        return result;
    }

    public object disposeAsync()
    {
        var capability = Promise.withResolvers();
        if (_disposed)
        {
            CallableOperations.Call1(capability.resolve, null, null);
            return capability.promise;
        }

        _disposed = true;
        var resources = _resources;
        _resources = [];
        new Disposal(resources, capability).Run();
        return capability.promise;
    }

    private sealed class Disposal(List<DisposableResource> resources, PromiseWithResolvers capability)
    {
        private bool _needsAwait;
        private bool _hasAwaited;
        private bool _hasError;
        private object? _error;

        internal void Run()
        {
            // Synchronously throwing async disposers do not suspend. Iterate rather
            // than recursively walking an arbitrarily large stack of such resources.
            while (resources.Count > 0)
            {
                var index = resources.Count - 1;
                var resource = resources[index];
                resources.RemoveAt(index);
                if (resource.Callback is null)
                {
                    _needsAwait = true;
                    continue;
                }

                object? result;
                try
                {
                    result = resource.Kind switch
                    {
                        DisposalKind.Use or DisposalKind.SyncUse =>
                            CallableOperations.Call0(resource.Callback, resource.Value),
                        DisposalKind.Adopt => CallableOperations.Call1(resource.Callback, null, resource.Value),
                        _ => CallableOperations.Call0(resource.Callback, null)
                    };
                }
                catch (Exception exception) when (exception is not ScriptProcessExitException)
                {
                    var error = GetThrownValue(exception);
                    if (resource.Kind == DisposalKind.SyncUse)
                    {
                        if (TryAwait(Promise.reject(error)))
                        {
                            return;
                        }
                        continue;
                    }
                    AddError(error);
                    continue;
                }

                // GetDisposeMethod's sync-to-async wrapper ignores the sync
                // method's return value, including promises and hostile thenables.
                if (TryAwait(resource.Kind == DisposalKind.SyncUse ? Promise.resolve(null) : result))
                {
                    return;
                }
            }

            if (_needsAwait && !_hasAwaited)
            {
                if (TryAwait(null))
                {
                    return;
                }
            }

            CallableOperations.Call1(_hasError ? capability.reject : capability.resolve, null, _error);
        }

        private bool TryAwait(object? value)
        {
            _hasAwaited = true;
            Promise promise;
            try
            {
                promise = (Promise)Promise.resolve(value)!;
            }
            catch (Exception exception) when (exception is not ScriptProcessExitException)
            {
                AddError(GetThrownValue(exception));
                return false;
            }

            BuiltinFunction1 fulfilled = (_, _) => { Run(); return null; };
            BuiltinFunction1 rejected = (_, reason) => { AddError(reason); Run(); return null; };
            promise.then(fulfilled, rejected);
            return true;
        }

        private void AddError(object? error)
        {
            _error = _hasError ? SuppressedError.Construct([error, _error]) : error;
            _hasError = true;
        }
    }

    private static object? GetThrownValue(Exception exception)
        => exception is JsThrownValueException thrown ? thrown.Value : exception;

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
        => thisArgument is AsyncDisposableStack stack
            ? stack.disposeAsync()
            : Promise.reject(new TypeError("AsyncDisposableStack.prototype.disposeAsync called on incompatible receiver"));

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
