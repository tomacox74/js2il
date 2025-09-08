using System;
using System.IO;
using System.Reflection;

namespace JavaScriptRuntime
{
    /// <summary>
    /// Holds global intrinsic variables for the current program (Node-like today, extensible later).
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
                    // argv is resolved on-demand by Process.argv from the environment provider.
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
        public static JavaScriptRuntime.Node.Process process { get; } = new JavaScriptRuntime.Node.Process();

        /// <summary>
        /// Global console object (lowercase) to mirror JS global. Provides access to log/error/warn via the Console intrinsic.
        /// Backed by a single shared instance.
        /// </summary>
        private static readonly JavaScriptRuntime.Console _console = new JavaScriptRuntime.Console();
        public static JavaScriptRuntime.Console console => _console;

        /// <summary>
        /// Dynamic lookup for well-known globals by name.
        /// Returns null when the name is unknown.
        /// </summary>
        public static object? Get(string name)
        {
            switch (name)
            {
                case "__dirname":
                    return __dirname;
                case "__filename":
                    return __filename;
                case "process":
                    return process;
                case "console":
                    return console;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Sets the active module path context. Call before executing a translated script.
        /// </summary>
        public static void SetModuleContext(string? dirname, string? filename)
        {
            __dirname = dirname ?? string.Empty;
            __filename = filename ?? string.Empty;
            // argv is resolved on-demand by Process.argv from the environment provider.
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
    }
}
