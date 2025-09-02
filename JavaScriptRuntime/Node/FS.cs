using System;
using System.IO;
using System.Text;

namespace JavaScriptRuntime.Node
{
    // Minimal fs module as a stable class: readFileSync/writeFileSync.
    [NodeModule("fs")]
    public sealed class FS
    {
        public object readFileSync(string file)
            => System.IO.File.ReadAllText(file);

        public object writeFileSync(string file, object? content)
        {
            System.IO.File.WriteAllText(file, content?.ToString() ?? string.Empty);
            return null!; // JS: undefined
        }

        // Overload supporting Node-style encoding option: 'utf8'
        public object readFileSync(object file, object? options)
        {
            var path = file?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path must be a non-empty string", nameof(file));

            if (options is string enc && IsUtf8(enc))
            {
                return System.IO.File.ReadAllText(path, Encoding.UTF8);
            }

            // Minimal support: no Buffer type; default to UTF-8 text
            return System.IO.File.ReadAllText(path, Encoding.UTF8);
        }

        // Overload supporting Node-style encoding option: 'utf8'
    public object writeFileSync(object file, object? content, object? options)
        {
            var path = file?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path must be a non-empty string", nameof(file));

            var text = content?.ToString() ?? string.Empty;
            if (options is string enc && IsUtf8(enc))
            {
                System.IO.File.WriteAllText(path, text, Encoding.UTF8);
        return null!; // JS: undefined
            }

            // Minimal support: default to UTF-8 text
            System.IO.File.WriteAllText(path, text, Encoding.UTF8);
        return null!; // JS: undefined
        }

        private static bool IsUtf8(string s)
        {
            return s.Equals("utf8", StringComparison.OrdinalIgnoreCase)
                || s.Equals("utf-8", StringComparison.OrdinalIgnoreCase);
        }
    }
}
