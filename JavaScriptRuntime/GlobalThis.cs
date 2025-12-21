namespace JavaScriptRuntime
{
    /// <summary>
    /// Holds global intrinsic variables for the current program (Node-like today, extensible later).
    /// Minimal surface for js2il codegen: __dirname, __filename, and process.exitCode.
    /// </summary>
    public static class GlobalThis
    {
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

        internal static JavaScriptRuntime.EngineCore.IMicrotaskScheduler? MicrotaskScheduler;

        private static JavaScriptRuntime.EngineCore.IScheduler? _scheduler;


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
    }
}
