namespace JavaScriptRuntime.Node
{
    [NodeModule("timers")]
    public sealed class TimersModule
    {
        public Func<object, object, object[], object> setTimeout => GlobalThis.setTimeout;

        public Func<object, object?> clearTimeout => GlobalThis.clearTimeout;
    }
}
