using System;

namespace JavaScriptRuntime.Node
{
    [NodeModule("events")]
    public sealed class Events
    {
        public Type EventEmitter => typeof(EventEmitter);
    }
}
