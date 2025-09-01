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
    }
}
