using System;
using System.Collections.Generic;

namespace JavaScriptRuntime;

/// <summary>
/// ECMAScript synchronous resource stack.
/// </summary>
[IntrinsicObject("DisposableStack")]
public sealed class DisposableStack : JsObject
{
    private enum DisposalKind { Use, Adopt, Defer }

    private readonly record struct DisposableResource(DisposalKind Kind, object? Value, object? Callback);

    private List<DisposableResource> _resources = [];
    private bool _disposed;

    internal static object Prototype
        => RuntimeIntrinsics.Current.GetOrCreate(
            RuntimeIntrinsicSlot.DisposableStackPrototype,
            static () => new JsObject());

    public DisposableStack()
    {
        PrototypeChain.InitializePrototype(this, Prototype);
    }

    public DisposableStack(object? _) : this()
    {
    }

    internal static void InitializeIntrinsicSurface(object objectPrototype)
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        GlobalThis.ConfigureBuiltinFunctionObject(typeof(DisposableStack));
        PrototypeChain.SetPrototype(Prototype, objectPrototype);
        DefineDataProperty(typeof(DisposableStack), "prototype", Prototype, configurable: false, writable: false);
        DefineDataProperty(typeof(DisposableStack), "name", "DisposableStack", configurable: true, writable: false);
        DefineDataProperty(typeof(DisposableStack), "length", 0d, configurable: true, writable: false);
        DefineDataProperty(Prototype, "constructor", typeof(DisposableStack));
        DefineFunction(Prototype, "use", (BuiltinFunction1)PrototypeUse, 1d);
        DefineFunction(Prototype, "adopt", (BuiltinFunction2)PrototypeAdopt, 2d);
        DefineFunction(Prototype, "defer", (BuiltinFunction1)PrototypeDefer, 1d);
        DefineFunction(Prototype, "move", (BuiltinFunction0)PrototypeMove, 0d);
        BuiltinFunction0 dispose = PrototypeDispose;
        DefineFunction(Prototype, "dispose", dispose, 0d);
        DefineDataProperty(Prototype, Symbol.dispose.DebugId, dispose);
        DefineDataProperty(Prototype, Symbol.toStringTag.DebugId, "DisposableStack", configurable: true, writable: false);

        BuiltinFunction0 disposedGetter = PrototypeDisposed;
        Function.InitializeFunctionInstance(disposedGetter, 0d, "get disposed", requiresInvocationContext: false);
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
        // Capture the capability before a disposer getter can move the stack.
        var resources = _resources;
        if (value is null || value is JsNull)
        {
            return value;
        }

        if (!Proxy.IsObjectLikeValue(value))
        {
            throw new TypeError("DisposableStack.prototype.use requires an object");
        }

        var callback = ObjectRuntime.GetProperty(value, Symbol.dispose.DebugId);
        if (!CallableOperations.IsCallable(callback))
        {
            throw new TypeError("Disposable resource method is not callable");
        }

        resources.Add(new DisposableResource(DisposalKind.Use, value, callback));
        return value;
    }

    public object? adopt(object? value, object? onDispose)
    {
        ThrowIfDisposed();
        if (!CallableOperations.IsCallable(onDispose))
        {
            throw new TypeError("DisposableStack.prototype.adopt requires a callable disposer");
        }

        _resources.Add(new DisposableResource(DisposalKind.Adopt, value, onDispose));
        return value;
    }

    public object? defer(object? onDispose)
    {
        ThrowIfDisposed();
        if (!CallableOperations.IsCallable(onDispose))
        {
            throw new TypeError("DisposableStack.prototype.defer requires a callable disposer");
        }

        _resources.Add(new DisposableResource(DisposalKind.Defer, null, onDispose));
        return null;
    }

    public DisposableStack move()
    {
        ThrowIfDisposed();
        var result = new DisposableStack { _resources = _resources };
        _resources = [];
        _disposed = true;
        return result;
    }

    public object? dispose()
    {
        if (_disposed)
        {
            return null;
        }

        _disposed = true;
        var resources = _resources;
        _resources = [];
        bool hasError = false;
        object? error = null;
        while (resources.Count > 0)
        {
            var index = resources.Count - 1;
            var resource = resources[index];
            resources.RemoveAt(index);
            try
            {
                // Synchronous disposal ignores every return value, including promises.
                switch (resource.Kind)
                {
                    case DisposalKind.Use:
                        CallableOperations.Call0(resource.Callback, resource.Value);
                        break;
                    case DisposalKind.Adopt:
                        CallableOperations.Call1(resource.Callback, null, resource.Value);
                        break;
                    default:
                        CallableOperations.Call0(resource.Callback, null);
                        break;
                }
            }
            catch (Exception exception) when (exception is not ScriptProcessExitException)
            {
                var nextError = exception is JsThrownValueException thrown ? thrown.Value : exception;
                error = hasError ? SuppressedError.Construct([nextError, error]) : nextError;
                hasError = true;
            }
        }

        if (hasError)
        {
            throw error as Exception ?? new JsThrownValueException(error);
        }

        return null;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ReferenceError("DisposableStack is already disposed");
        }
    }

    private static DisposableStack GetReceiver(object? thisArgument, string method)
        => thisArgument as DisposableStack
            ?? throw new TypeError($"DisposableStack.prototype.{method} called on incompatible receiver");

    private static object? PrototypeUse(object? thisArgument, object? value)
        => GetReceiver(thisArgument, "use").use(value);

    private static object? PrototypeAdopt(object? thisArgument, object? value, object? onDispose)
        => GetReceiver(thisArgument, "adopt").adopt(value, onDispose);

    private static object? PrototypeDefer(object? thisArgument, object? onDispose)
        => GetReceiver(thisArgument, "defer").defer(onDispose);

    private static object? PrototypeMove(object? thisArgument)
        => GetReceiver(thisArgument, "move").move();

    private static object? PrototypeDispose(object? thisArgument)
        => GetReceiver(thisArgument, "dispose").dispose();

    private static object? PrototypeDisposed(object? thisArgument)
        => GetReceiver(thisArgument, "disposed")._disposed;

    private static void DefineFunction(object target, string key, Delegate value, double length)
    {
        Function.InitializeFunctionInstance(
            value, length, key,
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
