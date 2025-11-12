using System;
using System.IO;

namespace JavaScriptRuntime.Node
{
    // Minimal path module as a stable class: join(...)
    [NodeModule("path")]
    public sealed class Path
    {
        public string join(params object[] parts)
        {
            var strings = System.Array.ConvertAll(parts ?? System.Array.Empty<object>(), p => p?.ToString() ?? string.Empty);
            return System.IO.Path.Combine(strings);
        }

        public string resolve(params object[] parts)
        {
            // Node's path.resolve resolves from right to left until an absolute path is constructed,
            // then normalizes. For our minimal needs in docs generation, combining and full-normalizing is sufficient.
            var strings = System.Array.ConvertAll(parts ?? System.Array.Empty<object>(), p => p?.ToString() ?? string.Empty);
            if (strings.Length == 0)
            {
                return System.IO.Path.GetFullPath(".");
            }

            string combined = strings[0] ?? string.Empty;
            for (int i = 1; i < strings.Length; i++)
            {
                combined = System.IO.Path.Combine(combined, strings[i] ?? string.Empty);
            }

            // If the last segment is absolute, Path.Combine handles it by resetting the path.
            // GetFullPath will produce an absolute, normalized path.
            return System.IO.Path.GetFullPath(combined);
        }

        public string relative(object from, object to)
        {
            var fromStr = from?.ToString();
            var toStr = to?.ToString();

            if (string.IsNullOrWhiteSpace(fromStr)) fromStr = ".";
            if (string.IsNullOrWhiteSpace(toStr)) toStr = ".";

            // Use .NET's relative path util; platform-specific separators match Node's behavior.
            return System.IO.Path.GetRelativePath(fromStr, toStr);
        }

        public string basename(object path, object? ext = null)
        {
            var p = path?.ToString() ?? string.Empty;
            var name = System.IO.Path.GetFileName(p);
            if (ext is string es && es.Length > 0 && name.EndsWith(es, StringComparison.Ordinal))
            {
                return name.Substring(0, name.Length - es.Length);
            }
            return name;
        }

        // Overload without extension argument to support dynamic invocation sites that provide only one argument.
        public string basename(object path)
        {
            var p = path?.ToString() ?? string.Empty;
            return System.IO.Path.GetFileName(p);
        }

        public string dirname(object path)
        {
            var p = path?.ToString() ?? string.Empty;
            return System.IO.Path.GetDirectoryName(p) ?? string.Empty;
        }
    }
}
