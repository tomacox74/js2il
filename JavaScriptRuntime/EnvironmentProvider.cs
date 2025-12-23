namespace JavaScriptRuntime
{
    // Central place to provide a mockable environment implementation
    public static class EnvironmentProvider
    {
        /// <summary>
        /// When true, DefaultEnvironment will not terminate the process on Exit/Exit(code).
        /// Tests set this to true; production leaves it false.
        /// </summary>
        public static bool SuppressExit { get; set; } = false;

        /// <summary>
        /// Returns the command-line arguments for the current process via the active environment.
        /// </summary>
        public static string[] GetCommandLineArgs()
        {
            var _current = JavaScriptRuntime.GlobalThis.ServiceProvider!.Resolve<IEnvironment>();
            return _current.GetCommandLineArgs();
        }

        /// <summary>
        /// Alias for semantic clarity in Node interop.
        /// </summary>
        public static string[] GetProcessArgs() => GetCommandLineArgs();
    }
}
