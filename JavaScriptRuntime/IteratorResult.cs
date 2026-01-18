using System.Collections.Generic;
using System.Dynamic;

namespace JavaScriptRuntime;

/// <summary>
/// Helper for creating iterator result objects of the form: { value: any, done: boolean }.
/// </summary>
public static class IteratorResult
{
    public static object Create(object? value, bool done)
    {
        var expando = new ExpandoObject() as IDictionary<string, object?>;
        expando!["value"] = value;
        expando["done"] = done;
        return (ExpandoObject)expando;
    }
}
