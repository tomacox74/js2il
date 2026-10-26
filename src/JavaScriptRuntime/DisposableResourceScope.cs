using System;
using System.Collections.Generic;

namespace JavaScriptRuntime;

public sealed class DisposableResourceScope
{
    private readonly record struct Resource(
        object? Value, object? Method, bool Asynchronous, bool SyncFallback);

    private readonly List<Resource> _resources = [];
    private bool _needsAwait;
    private bool _hasAwaited;
    private bool _hasError;
    private object? _error;
    private object? _awaitedValue;

    public static object CreateScope() => new DisposableResourceScope();

    public static object? Add(object scope, object? value, bool asynchronous)
    {
        var resources = ((DisposableResourceScope)scope)._resources;
        if (value is null or JsNull)
        {
            if (asynchronous)
                resources.Add(new Resource(null, null, true, false));
            return value;
        }

        if (!Proxy.IsObjectLikeValue(value))
            throw new TypeError("Disposable resource must be an object");

        var method = GetDisposeMethod(value, asynchronous, out var syncFallback);
        resources.Add(new Resource(value, method, asynchronous, syncFallback));
        return value;
    }

    internal static object? GetDisposeMethod(object value, bool asynchronous, out bool syncFallback)
    {
        syncFallback = false;
        var method = asynchronous ? ObjectRuntime.GetItem(value, Symbol.asyncDispose) : null;
        if (!asynchronous || method is null or JsNull)
        {
            method = ObjectRuntime.GetItem(value, Symbol.dispose);
            syncFallback = asynchronous;
        }
        if (!CallableOperations.IsCallable(method))
            throw new TypeError("Disposable resource method is not callable");
        return method;
    }

    public static void BeginDisposal(object scope, bool hasError, object? error)
    {
        var resources = (DisposableResourceScope)scope;
        resources._hasError = hasError;
        resources._error = error is JsThrownValueException thrown ? thrown.Value : error;
    }

    public static bool MoveNext(object scope)
    {
        var resources = (DisposableResourceScope)scope;
        while (resources._resources.Count > 0)
        {
            var index = resources._resources.Count - 1;
            var resource = resources._resources[index];
            if (resource.Method is null)
            {
                resources._resources.RemoveAt(index);
                resources._needsAwait = true;
                continue;
            }

            if (!resource.Asynchronous && resources._needsAwait && !resources._hasAwaited)
                return resources.Await(null);

            resources._resources.RemoveAt(index);
            object? result;
            try
            {
                result = CallableOperations.Call0(resource.Method, resource.Value);
            }
            catch (Exception exception) when (exception is not ScriptProcessExitException)
            {
                var error = exception is JsThrownValueException thrown ? thrown.Value : exception;
                if (resource.SyncFallback)
                    return resources.Await(Promise.reject(error));
                RecordError(scope, error);
                continue;
            }

            if (resource.Asynchronous)
                return resources.Await(resource.SyncFallback ? null : result);
        }

        if (resources._needsAwait && !resources._hasAwaited)
            return resources.Await(null);
        resources._awaitedValue = null;
        return false;
    }

    private bool Await(object? value)
    {
        _hasAwaited = true;
        _awaitedValue = value;
        return true;
    }

    public static object? GetAwaitedValue(object scope)
        => ((DisposableResourceScope)scope)._awaitedValue;

    public static void RecordError(object scope, object? error)
    {
        var resources = (DisposableResourceScope)scope;
        resources._error = resources._hasError
            ? SuppressedError.Construct([error, resources._error])
            : error;
        resources._hasError = true;
    }

    public static bool HasError(object scope) => ((DisposableResourceScope)scope)._hasError;

    public static object? GetError(object scope) => ((DisposableResourceScope)scope)._error;
}
