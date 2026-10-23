namespace JavaScriptRuntime;

/// <summary>Iterator records used by compiled synchronous generator yield* expressions.</summary>
public static class GeneratorDelegation
{
    private sealed record IteratorRecord(object Iterator, object? NextMethod);

    public static object Create(object iterable)
    {
        var method = ObjectRuntime.GetProperty(iterable, Symbol.iterator.DebugId);
        if (!CallableOperations.IsCallable(method))
        {
            throw new TypeError("Object is not iterable");
        }

        var iterator = ValidateResult(CallableOperations.Call0(method, iterable));
        return new IteratorRecord(iterator, ObjectRuntime.GetProperty(iterator, "next"));
    }

    public static object Next(object iterator, object? value)
    {
        var record = (IteratorRecord)iterator;
        return ValidateResult(CallableOperations.Call1(record.NextMethod, record.Iterator, value));
    }

    public static object Return(object iterator, object? value)
    {
        var record = (IteratorRecord)iterator;
        var method = ObjectRuntime.GetProperty(record.Iterator, "return");
        return method is null or JsNull
            ? IteratorResult.Create(value, done: true)
            : ValidateResult(CallableOperations.Call1(method, record.Iterator, value));
    }

    public static object Throw(object iterator, object? value)
    {
        var record = (IteratorRecord)iterator;
        var method = ObjectRuntime.GetProperty(record.Iterator, "throw");
        if (method is not null and not JsNull)
        {
            return ValidateResult(CallableOperations.Call1(method, record.Iterator, value));
        }

        var returnMethod = ObjectRuntime.GetProperty(record.Iterator, "return");
        if (returnMethod is not null and not JsNull)
        {
            ValidateResult(CallableOperations.Call0(returnMethod, record.Iterator));
        }

        throw new TypeError("Delegated iterator has no throw method");
    }

    private static object ValidateResult(object? result)
        => Proxy.IsObjectLikeValue(result)
            ? result!
            : throw new TypeError("Iterator result is not an object");
}
