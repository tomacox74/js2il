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
    }
}
