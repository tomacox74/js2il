using System;
using System.Dynamic;
using System.IO;

namespace JavaScriptRuntime.Node
{
    // Minimal path module: only join for now.
    public static class Path
    {
        public static object CreateModule()
        {
            dynamic exp = new ExpandoObject();
            exp.join = (Func<object[], string>)(parts => System.IO.Path.Combine(Array.ConvertAll(parts, p => p?.ToString() ?? string.Empty)));
            return exp;
        }
    }
}
