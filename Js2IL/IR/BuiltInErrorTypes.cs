using System;
using System.Collections.Generic;

namespace Js2IL.IR;

internal static class BuiltInErrorTypes
{
    private static readonly HashSet<string> Names = new(StringComparer.Ordinal)
    {
        "Error",
        "EvalError",
        "RangeError",
        "ReferenceError",
        "SyntaxError",
        "TypeError",
        "URIError",
        "AggregateError",
    };

    public static bool IsBuiltInErrorTypeName(string name) => Names.Contains(name);
}
