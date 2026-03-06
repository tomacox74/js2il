using System;

namespace JavaScriptRuntime.Node
{
    [NodeModule("stream")]
    public sealed class Stream
    {
        public Type Readable => typeof(Readable);
        public Type Writable => typeof(Writable);
        public Type Duplex => typeof(Duplex);
        public Type Transform => typeof(Transform);
        public Type PassThrough => typeof(PassThrough);
    }
}
