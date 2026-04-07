namespace JavaScriptRuntime.Node
{
    [NodeModule("stream/promises")]
    public sealed class StreamPromises
    {
        private readonly Stream _stream = new();

        public object finished(object[] args)
        {
            return _stream.finished(args);
        }

        public object pipeline(object[] args)
        {
            return _stream.pipeline(args);
        }
    }
}
