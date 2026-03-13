using System;
using System.IO;

namespace JavaScriptRuntime.Node
{
    [NodeModule("os")]
    public sealed class OS
    {
        public string tmpdir()
        {
            var p = System.IO.Path.GetTempPath();
            return p.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
        }

        public string homedir()
        {
            // Closest equivalent to Node's os.homedir().
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) ?? string.Empty;
        }
    }
}
