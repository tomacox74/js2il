using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JavaScriptRuntime.Node
{
    [NodeModule("string_decoder")]
    public sealed class StringDecoderModule
    {
        public Type StringDecoder => typeof(StringDecoder);
    }

    public sealed class StringDecoder
    {
        private readonly Utf8ChunkDecoder _decoder = new();

        public StringDecoder()
            : this(null)
        {
        }

        public StringDecoder(object? encoding)
        {
            encoding = NormalizeEncoding(encoding);
            this.encoding = encoding;
        }

        public object? encoding { get; }

        public string write(object? buffer)
        {
            var bytes = CoerceToBytes(buffer);
            return _decoder.Decode(bytes, bytes.Length, flush: false);
        }

        public string end()
            => end(null);

        public string end(object? buffer)
        {
            var prefix = buffer == null || buffer is JsNull
                ? string.Empty
                : write(buffer);

            return prefix + _decoder.Flush();
        }

        private static string NormalizeEncoding(object? encoding)
        {
            if (encoding == null || encoding is JsNull)
            {
                return "utf8";
            }

            var name = DotNet2JSConversions.ToString(encoding);
            if (string.IsNullOrWhiteSpace(name)
                || string.Equals(name, "utf8", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "utf-8", StringComparison.OrdinalIgnoreCase))
            {
                return "utf8";
            }

            throw new NotSupportedException($"string_decoder currently supports utf8 only (received '{name}').");
        }

        private static byte[] CoerceToBytes(object? value)
        {
            if (value == null || value is JsNull)
            {
                return System.Array.Empty<byte>();
            }

            if (value is Buffer buffer)
            {
                return buffer.ToByteArray();
            }

            if (value is byte[] bytes)
            {
                return bytes;
            }

            if (value is JavaScriptRuntime.Array jsArray)
            {
                var result = new byte[jsArray.Count];
                for (var i = 0; i < jsArray.Count; i++)
                {
                    result[i] = ToUint8(jsArray[i]);
                }

                return result;
            }

            if (value is IEnumerable enumerable && value is not string)
            {
                var result = new List<byte>();
                foreach (var item in enumerable)
                {
                    result.Add(ToUint8(item));
                }

                return result.ToArray();
            }

            return Encoding.UTF8.GetBytes(DotNet2JSConversions.ToString(value));
        }

        private static byte ToUint8(object? value)
        {
            double number;
            try
            {
                number = TypeUtilities.ToNumber(value);
            }
            catch
            {
                return 0;
            }

            if (double.IsNaN(number) || double.IsInfinity(number))
            {
                return 0;
            }

            return (byte)((int)number & 0xFF);
        }
    }
}
