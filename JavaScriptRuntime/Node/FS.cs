using System;
using System.IO;

namespace JavaScriptRuntime.Node
{
    // Minimal fs module as a stable class: readFileSync/writeFileSync.
    [NodeModule("fs")]
    public sealed class FS
    {
        public object readFileSync(string file)
            => System.IO.File.ReadAllText(file);

        public void writeFileSync(string file, object? content)
            => System.IO.File.WriteAllText(file, content?.ToString() ?? string.Empty);
    }
}
