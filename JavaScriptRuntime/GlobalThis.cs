using System;
using System.IO;
using System.Reflection;

namespace JavaScriptRuntime
{
    /// <summary>
    /// Holds global intrinsic variables for the current program (Node-like today, extensible later).
    /// Minimal surface for js2il codegen: __dirname, __filename, and process.exitCode.
    /// </summary>
    public static class GlobalThis
    {
        static GlobalThis()
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

        internal static JavaScriptRuntime.EngineCore.IScheduler? Scheduler
        {
            get => _scheduler;
            set
            {
                _scheduler = value;
                // Reset timers when scheduler changes (important for test isolation)
                _timers = null;
            }
        }
        private static JavaScriptRuntime.EngineCore.IScheduler? _scheduler;

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

        private static Timers? _timers;

        /// <summary>
        /// ECMAScript global Infinity value (+∞).
        /// Exposed as a static property so identifiers bind at compile-time.
        /// </summary>
        public static double Infinity => double.PositiveInfinity;

        /// <summary>
        /// ECMAScript global NaN value.
        /// Exposed as a static property so identifiers bind at compile-time.
        /// </summary>
        public static double NaN => double.NaN;

        private static Timers EnsureTimers()
        {
            if (_timers == null)
            {
                if (Scheduler == null)
                {
                    throw new InvalidOperationException("{INTERNAL ERROR} No scheduler available for timers");
                }
                _timers = new Timers(Scheduler);
            }
            return _timers;
        }

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
                case "Infinity":
                    return Infinity;
                case "NaN":
                    return NaN;
                default:
                    return null;
            }
        }

        public static object setTimeout(object callback, object delay, params object[] args)
        {
            return EnsureTimers().setTimeout(callback, delay, args);
        }

        public static object clearTimeout(object handle)
        {
            return EnsureTimers().clearTimeout(handle);
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
