using System;

namespace JavaScriptRuntime.Node
{
    [NodeModule("stream")]
    public sealed class Stream
    {
        public Type Readable => typeof(Readable);
        public Type Writable => typeof(Writable);
    }
}
