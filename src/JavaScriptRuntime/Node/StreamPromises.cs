using System;

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
            if (_stream.pipeline(args) is not Promise promise)
            {
                return Promise.resolve(null)!;
            }

            return promise.then(new Func<object?[], object?, object?>((_, _) => null))!;
        }
    }
}
