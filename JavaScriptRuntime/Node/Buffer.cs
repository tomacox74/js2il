using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JavaScriptRuntime.Node
{
    [IntrinsicObject("Buffer")]
    public sealed class Buffer
    {
        private readonly byte[] _bytes;

        public Buffer(byte[] bytes)
        {
            _bytes = bytes ?? System.Array.Empty<byte>();
        }

        public double length => _bytes.Length;

        public static Buffer from(object? value)
        {
            return from(value, null);
        }

        public static Buffer from(object? value, object? encoding)
        {
            if (value is Buffer buffer)
            {
                return new Buffer((byte[])buffer._bytes.Clone());
            }

            if (value is byte[] bytes)
            {
                return new Buffer((byte[])bytes.Clone());
            }

            if (value is string text)
            {
                return new Buffer(ResolveEncoding(encoding).GetBytes(text));
            }

            if (value is JavaScriptRuntime.Array jsArray)
            {
                var arrBytes = new byte[jsArray.Count];
                for (int i = 0; i < jsArray.Count; i++)
                {
                    arrBytes[i] = ToUint8(jsArray[i]);
                }

                return new Buffer(arrBytes);
            }

            if (value is IEnumerable enumerable && value is not string)
            {
                var list = new List<byte>();
                foreach (var item in enumerable)
                {
                    list.Add(ToUint8(item));
                }

                return new Buffer(list.ToArray());
            }

            if (value is null || value is JsNull)
            {
                return new Buffer(System.Array.Empty<byte>());
            }

            return new Buffer(ResolveEncoding(encoding).GetBytes(DotNet2JSConversions.ToString(value)));
        }

        public static bool isBuffer(object? value)
        {
            return value is Buffer;
        }

        public string toString()
        {
            return toString("utf8");
        }

        public string toString(object? encoding)
        {
            return ResolveEncoding(encoding).GetString(_bytes);
        }

        internal static Buffer FromBytes(byte[] bytes)
        {
            return new Buffer((byte[])bytes.Clone());
        }

        internal byte[] ToByteArray()
        {
            return (byte[])_bytes.Clone();
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

            var truncated = (int)System.Math.Truncate(number);
            return (byte)(truncated & 0xFF);
        }

        private static Encoding ResolveEncoding(object? encoding)
        {
            var name = encoding?.ToString();
            if (string.IsNullOrWhiteSpace(name)
                || name.Equals("utf8", StringComparison.OrdinalIgnoreCase)
                || name.Equals("utf-8", StringComparison.OrdinalIgnoreCase))
            {
                return Encoding.UTF8;
            }

            throw new NotSupportedException($"Buffer encoding '{name}' is not supported yet.");
        }
    }
}