using System;
using System.Text;

namespace JavaScriptRuntime.Node
{
    [NodeModule("buffer")]
    public sealed class BufferModule
    {
        private static readonly UTF8Encoding StrictUtf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        public Type Buffer => typeof(Buffer);

        public Func<object?, bool> isUtf8 => IsUtf8;

        public Func<object?, object?> resolveObjectURL => ResolveObjectURL;

        private static bool IsUtf8(object? input)
        {
            if (!TryGetBytes(input, out var bytes))
            {
                return false;
            }

            try
            {
                _ = StrictUtf8.GetString(bytes);
                return true;
            }
            catch (DecoderFallbackException)
            {
                return false;
            }
        }

        // JROC does not yet support Blob object URL creation, so no object can resolve here.
        // Returning undefined matches Node's result for an unrecognized object URL.
        private static object? ResolveObjectURL(object? id) => null;

        private static bool TryGetBytes(object? input, out byte[] bytes)
        {
            switch (input)
            {
                case Buffer buffer:
                    bytes = buffer.ToByteArray();
                    return true;
                case ArrayBuffer arrayBuffer:
                    bytes = arrayBuffer.RawBytes;
                    return true;
                case TypedArrayBase typedArray:
                    bytes = typedArray.CopyRawBytes();
                    return true;
                default:
                    bytes = System.Array.Empty<byte>();
                    return false;
            }
        }
    }
}
