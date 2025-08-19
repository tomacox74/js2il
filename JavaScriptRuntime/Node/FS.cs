using System;
using System.IO;
using System.Dynamic;

namespace JavaScriptRuntime.Node
{
    // Minimal fs module: only readFileSync/writeFileSync for now.
    public static class FS
    {
        public static object CreateModule()
        {
            dynamic exp = new ExpandoObject();
            exp.readFileSync = (Func<string, object>)(file => System.IO.File.ReadAllText(file));
            exp.writeFileSync = (Action<string, object>)((file, content) => System.IO.File.WriteAllText(file, content?.ToString() ?? string.Empty));
            return exp;
        }
    }
}
