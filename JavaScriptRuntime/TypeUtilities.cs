using System;
using System.Dynamic;

namespace JavaScriptRuntime
{
    public static class TypeUtilities
    {
        // Minimal typeof implementation for our runtime object shapes
        public static string Typeof(object? value)
        {
            if (value is null) return "object"; // JS: typeof null === 'object'
            switch (value)
            {
                case string: return "string";
                case bool: return "boolean";
                case double: return "number";
                case float: return "number";
                case int: return "number";
                case long: return "number";
                case short: return "number";
                case byte: return "number";
            }
            if (value is ExpandoObject) return "object";
            if (value is Array) return "object";
            // Functions are delegates in our model; detect common delegate base
            if (value is Delegate) return "function";
            return "object";
        }
    }
}
