using System;
using System.IO;
using System.Reflection;

namespace JavaScriptRuntime.Node
{
    /// <summary>
    /// Holds Node-like global intrinsic variables for the current program.
    /// Minimal surface for js2il codegen: __dirname, __filename, and process.exitCode.
    /// </summary>
    public static class GlobalVariables
    {
        static GlobalVariables()
        {
            // Provide sensible defaults when running out-of-proc: resolve to the entry assembly path.
            try
            {
                var entry = Assembly.GetEntryAssembly();
                var file = entry?.Location;
                if (!string.IsNullOrEmpty(file))
                {
                    __filename = file!;
                    __dirname = System.IO.Path.GetDirectoryName(file!) ?? string.Empty;
                }
            }
            catch
            {
                // Best-effort; leave defaults if anything goes wrong.
            }
        }

        /// <summary>Directory name of the current module (script).</summary>
        public static string __dirname { get; private set; } = string.Empty;

        /// <summary>Absolute filename of the current module (script).</summary>
        public static string __filename { get; private set; } = string.Empty;

    /// <summary>Minimal process global with writable exitCode.</summary>
    public static Process process { get; } = new Process();

        /// <summary>
        /// Sets the active module path context. Call before executing a translated script.
        /// </summary>
        public static void SetModuleContext(string? dirname, string? filename)
        {
            __dirname = dirname ?? string.Empty;
            __filename = filename ?? string.Empty;
        }

        /// <summary>
        /// Resets globals to defaults (useful for tests).
        /// </summary>
        public static void Reset()
        {
            __dirname = string.Empty;
            __filename = string.Empty;
            process.exitCode = 0;
        }

    // Process class is now in its own file (Process.cs)
    }
}
