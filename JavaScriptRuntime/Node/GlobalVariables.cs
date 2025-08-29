using System;

namespace JavaScriptRuntime.Node
{
    /// <summary>
    /// Holds Node-like global intrinsic variables for the current program.
    /// Minimal surface for js2il codegen: __dirname, __filename, and process.exitCode.
    /// </summary>
    public static class GlobalVariables
    {
        /// <summary>Directory name of the current module (script).</summary>
        public static string __dirname { get; private set; } = string.Empty;

        /// <summary>Absolute filename of the current module (script).</summary>
        public static string __filename { get; private set; } = string.Empty;

        /// <summary>Minimal process global with writable exitCode.</summary>
        public static ProcessInfo process { get; } = new ProcessInfo();

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

        public sealed class ProcessInfo
        {
            /// <summary>Matches Node's writable process.exitCode.</summary>
            public int exitCode { get; set; }
        }
    }
}
