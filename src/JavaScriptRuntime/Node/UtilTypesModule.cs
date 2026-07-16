namespace JavaScriptRuntime.Node
{
    [NodeModule("util/types")]
    public sealed class UtilTypesModule
    {
        public Func<object?, bool> isUint8Array => throw new InvalidOperationException("util/types is an alias for util.types.");

        public Func<object?, bool> isArrayBuffer => throw new InvalidOperationException("util/types is an alias for util.types.");
    }
}
