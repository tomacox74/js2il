namespace JavaScriptRuntime;

/// <summary>Iterator records used by compiled async generator yield* expressions.</summary>
public static class AsyncGeneratorDelegation
{
    private sealed record IteratorRecord(object Iterator, object? NextMethod, bool IsSync);

    public static object Create(object iterable)
    {
        var method = ObjectRuntime.GetProperty(iterable, Symbol.asyncIterator.DebugId);
        var isSync = method is null or JsNull;
        if (isSync)
        {
            method = ObjectRuntime.GetProperty(iterable, Symbol.iterator.DebugId);
        }

        if (!CallableOperations.IsCallable(method))
        {
            throw new TypeError("Object is not iterable");
        }

        var iterator = CallableOperations.Call0(method, iterable);
        ValidateResult(iterator);
        return new IteratorRecord(iterator!, ObjectRuntime.GetProperty(iterator!, "next"), isSync);
    }

    public static object ValidateResult(object? result)
        => Proxy.IsObjectLikeValue(result)
            ? result!
            : throw new TypeError("Iterator result is not an object");

    public static object? Next(object iterator, object? value)
    {
        var record = (IteratorRecord)iterator;
        return Invoke(record, record.NextMethod, value);
    }

    public static object? Return(object iterator, object? value)
    {
        var record = (IteratorRecord)iterator;
        var method = ObjectRuntime.GetProperty(record.Iterator, "return");
        return method is null or JsNull
            ? IteratorResult.Create(value, done: true)
            : Invoke(record, method, value);
    }

    public static object? Throw(object iterator, object? value)
    {
        var record = (IteratorRecord)iterator;
        var method = ObjectRuntime.GetProperty(record.Iterator, "throw");
        if (method is not null and not JsNull)
        {
            return Invoke(record, method, value);
        }

        var returnMethod = ObjectRuntime.GetProperty(record.Iterator, "return");
        if (returnMethod is null or JsNull)
        {
            throw new TypeError("Delegated iterator has no throw method");
        }

        var result = CallableOperations.Call0(returnMethod, record.Iterator);
        if (record.IsSync)
        {
            ValidateResult(result);
            throw new TypeError("Delegated iterator has no throw method");
        }

        var promise = (Promise)Promise.resolve(result)!;
        return promise.then((BuiltinFunction1)((_, resolved) =>
        {
            ValidateResult(resolved);
            throw new TypeError("Delegated iterator has no throw method");
        }));
    }

    private static object? Invoke(IteratorRecord record, object? method, object? value)
    {
        var result = CallableOperations.Call1(method, record.Iterator, value);
        if (!record.IsSync)
        {
            return result;
        }

        ValidateResult(result);
        var done = ObjectRuntime.IteratorResultDone(result!);
        var yielded = ObjectRuntime.IteratorResultValue(result!);
        return ((Promise)Promise.resolve(yielded)!).then(
            (BuiltinFunction1)((_, resolved) => IteratorResult.Create(resolved, done)));
    }
}
