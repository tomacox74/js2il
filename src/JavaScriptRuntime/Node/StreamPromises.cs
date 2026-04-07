using System;

namespace JavaScriptRuntime.Node
{
    [NodeModule("stream/promises")]
    public sealed class StreamPromises
    {
        private readonly Stream _stream = new();

        public object finished(object[] args)
        {
            try
            {
                return _stream.finished(args);
            }
            catch (Exception ex)
            {
                return Promise.reject(ex as Error ?? new Error(ex.Message, ex))!;
            }
        }

        public object pipeline(object[] args)
        {
            try
            {
                if (_stream.pipeline(args) is not Promise promise)
                {
                    return Promise.resolve(null)!;
                }

                return promise.then(new Func<object?[], object?, object?>((_, _) => null))!;
            }
            catch (Exception ex)
            {
                return Promise.reject(ex as Error ?? new Error(ex.Message, ex))!;
            }
        }
    }
}
