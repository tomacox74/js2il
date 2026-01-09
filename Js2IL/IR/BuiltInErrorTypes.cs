using System;
using System.Collections.Generic;

namespace Js2IL.IR;

internal static class BuiltInErrorTypes
{
    private static readonly Dictionary<string, Type> NameToClrType = new(StringComparer.Ordinal)
    {
        ["Error"] = typeof(global::JavaScriptRuntime.Error),
        ["EvalError"] = typeof(global::JavaScriptRuntime.EvalError),
        ["RangeError"] = typeof(global::JavaScriptRuntime.RangeError),
        ["ReferenceError"] = typeof(global::JavaScriptRuntime.ReferenceError),
        ["SyntaxError"] = typeof(global::JavaScriptRuntime.SyntaxError),
        ["TypeError"] = typeof(global::JavaScriptRuntime.TypeError),
        ["URIError"] = typeof(global::JavaScriptRuntime.URIError),
        ["AggregateError"] = typeof(global::JavaScriptRuntime.AggregateError),
    };

    public static bool IsBuiltInErrorTypeName(string name) => NameToClrType.ContainsKey(name);

    public static Type GetRuntimeErrorClrType(string name)
    {
        if (!NameToClrType.TryGetValue(name, out var clrType))
        {
            throw new InvalidOperationException($"Unknown built-in error type: {name}");
        }

        return clrType;
    }
}
