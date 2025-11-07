namespace JavaScriptRuntime
{
    // Central place to provide a mockable environment implementation
    public static class EnvironmentProvider
    {
        private static IEnvironment _current = new DefaultEnvironment();
        /// <summary>
        /// When true, DefaultEnvironment will not terminate the process on Exit/Exit(code).
        /// Tests set this to true; production leaves it false.
        /// </summary>
        public static bool SuppressExit { get; set; } = false;

        public static IEnvironment Current => _current;

        public static void SetEnvironment(IEnvironment env)
        {
            _current = env ?? new DefaultEnvironment();
        }

        /// <summary>
        /// The last exit code that was explicitly set via process.exit(code) or inferred by process.exit().
        /// Useful for testing when process termination is suppressed.
        /// </summary>
        public static int? LastExitCodeSet { get; internal set; }

        /// <summary>
        /// Returns the command-line arguments for the current process via the active environment.
        /// </summary>
        public static string[] GetCommandLineArgs()
        {
            return _current.GetCommandLineArgs();
        }

    /// <summary>
    /// Alias for semantic clarity in Node interop.
    /// </summary>
    public static string[] GetProcessArgs() => GetCommandLineArgs();
    }
}
