namespace JavaScriptRuntime.Node
{
    [NodeModule("timers")]
    public sealed partial class TimersModule
    {
        public Func<object, object, object[], object> setTimeout => GlobalThis.setTimeout;

        public Func<object, object?> clearTimeout => GlobalThis.clearTimeout;

        public Func<object, object[], object> setImmediate => GlobalThis.setImmediate;

        public Func<object, object?> clearImmediate => GlobalThis.clearImmediate;

        public Func<object, object, object[], object> setInterval => GlobalThis.setInterval;

        public Func<object, object?> clearInterval => GlobalThis.clearInterval;

        private static object ContractSetTimeout(Delegate callback)
            => GlobalThis.setTimeout(callback, 1d);

        private static object ContractSetTimeout(
            Delegate callback,
            object? delay,
            params object?[] args)
            => GlobalThis.setTimeout(callback, delay!, args!);

        private static object ContractSetImmediate(Delegate callback, params object?[] args)
            => GlobalThis.setImmediate(callback, args!);

        private static object ContractSetInterval(Delegate callback)
            => GlobalThis.setInterval(callback, 1d);

        private static object ContractSetInterval(
            Delegate callback,
            object? delay,
            params object?[] args)
            => GlobalThis.setInterval(callback, delay!, args!);

        private static object? ContractClearTimeout(object? handle)
            => GlobalThis.clearTimeout(handle!);

        private static object? ContractClearImmediate(object? handle)
            => GlobalThis.clearImmediate(handle!);

        private static object? ContractClearInterval(object? handle)
            => GlobalThis.clearInterval(handle!);
    }
}
