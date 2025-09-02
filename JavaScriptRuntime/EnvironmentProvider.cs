namespace JavaScriptRuntime
{
    // Central place to provide a mockable environment implementation
    public static class EnvironmentProvider
    {
        private static IEnvironment _current = new DefaultEnvironment();

        public static IEnvironment Current => _current;

        public static void SetEnvironment(IEnvironment env)
        {
            _current = env ?? new DefaultEnvironment();
        }

        /// <summary>
        /// Returns the command-line arguments for the current process via the active environment.
        /// </summary>
        public static string[] GetCommandLineArgs()
        {
            return _current.GetCommandLineArgs();
        }
    }
}
