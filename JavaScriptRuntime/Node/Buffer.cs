using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public static Buffer alloc(object? size)
        {
            return alloc(size, null, null);
        }

        public static Buffer alloc(object? size, object? fill)
        {
            return alloc(size, fill, null);
        }

        public static Buffer alloc(object? size, object? fill, object? encoding)
        {
            var length = ToLength(size);
            var bytes = new byte[length];
            if (length == 0)
            {
                return new Buffer(bytes);
            }

            if (fill == null || fill is JsNull)
            {
                return new Buffer(bytes);
            }

            byte[] fillBytes;
            if (fill is Buffer fillBuffer)
            {
                fillBytes = fillBuffer.ToByteArray();
            }
            else if (fill is byte[] byteFill)
            {
                fillBytes = byteFill;
            }
            else if (fill is string fillText)
            {
                fillBytes = ResolveEncoding(encoding).GetBytes(fillText);
            }
            else
            {
                fillBytes = new[] { ToUint8(fill) };
            }

            if (fillBytes.Length == 0)
            {
                return new Buffer(bytes);
            }

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = fillBytes[i % fillBytes.Length];
            }

            return new Buffer(bytes);
        }

        public static double byteLength(object? value)
        {
            return byteLength(value, null);
        }

        public static double byteLength(object? value, object? encoding)
        {
            if (value is null || value is JsNull)
            {
                return 0;
            }

            if (value is Buffer buffer)
            {
                return buffer.length;
            }

            if (value is byte[] bytes)
            {
                return bytes.Length;
            }

            var text = DotNet2JSConversions.ToString(value);
            return ResolveEncoding(encoding).GetByteCount(text);
        }

        public static Buffer concat(object? list)
        {
            return concat(list, null);
        }

        public static Buffer concat(object? list, object? totalLength)
        {
            var chunks = new List<byte[]>();
            var total = 0;

            if (list is JavaScriptRuntime.Array jsArray)
            {
                for (int i = 0; i < jsArray.Count; i++)
                {
                    var chunk = CoerceToBytes(jsArray[i]);
                    chunks.Add(chunk);
                    total += chunk.Length;
                }
            }
            else if (list is IEnumerable enumerable && list is not string)
            {
                foreach (var item in enumerable)
                {
                    var chunk = CoerceToBytes(item);
                    chunks.Add(chunk);
                    total += chunk.Length;
                }
            }

            var outputLength = totalLength == null || totalLength is JsNull
                ? total
                : ToLength(totalLength);

            var result = new byte[outputLength];
            var offset = 0;
            foreach (var chunk in chunks)
            {
                if (offset >= result.Length)
                {
                    break;
                }

                var copyLength = System.Math.Min(chunk.Length, result.Length - offset);
                if (copyLength > 0)
                {
                    System.Buffer.BlockCopy(chunk, 0, result, offset, copyLength);
                    offset += copyLength;
                }
            }

            return new Buffer(result);
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

            if (name.Equals("base64", StringComparison.OrdinalIgnoreCase))
            {
                return Base64PassthroughEncoding.Instance;
            }

            if (name.Equals("hex", StringComparison.OrdinalIgnoreCase))
            {
                return HexPassthroughEncoding.Instance;
            }

            throw new NotSupportedException($"Buffer encoding '{name}' is not supported yet.");
        }

        private static int ToLength(object? value)
        {
            double n;
            try
            {
                n = TypeUtilities.ToNumber(value);
            }
            catch
            {
                return 0;
            }

            if (double.IsNaN(n) || n <= 0)
            {
                return 0;
            }

            if (double.IsInfinity(n) || n >= int.MaxValue)
            {
                return int.MaxValue;
            }

            return (int)System.Math.Truncate(n);
        }

        private static byte[] CoerceToBytes(object? value)
        {
            if (value is Buffer buffer)
            {
                return buffer.ToByteArray();
            }

            if (value is byte[] bytes)
            {
                return bytes;
            }

            if (value is string text)
            {
                return Encoding.UTF8.GetBytes(text);
            }

            if (value is JavaScriptRuntime.Array jsArray)
            {
                var arrBytes = new byte[jsArray.Count];
                for (int i = 0; i < jsArray.Count; i++)
                {
                    arrBytes[i] = ToUint8(jsArray[i]);
                }

                return arrBytes;
            }

            if (value is IEnumerable enumerable && value is not string)
            {
                return enumerable.Cast<object?>().Select(ToUint8).ToArray();
            }

            return System.Array.Empty<byte>();
        }

        private sealed class Base64PassthroughEncoding : Encoding
        {
            public static readonly Base64PassthroughEncoding Instance = new();

            public override int GetByteCount(char[] chars, int index, int count)
                => GetBytes(chars, index, count, System.Array.Empty<byte>(), 0);

            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
            {
                var text = new string(chars, charIndex, charCount);
                if (string.IsNullOrEmpty(text))
                {
                    return 0;
                }

                try
                {
                    var decoded = System.Convert.FromBase64String(text);
                    if (bytes.Length > 0 && byteIndex < bytes.Length)
                    {
                        var available = bytes.Length - byteIndex;
                        if (available > 0)
                        {
                            var copyLength = System.Math.Min(decoded.Length, available);
                            if (copyLength > 0)
                            {
                                System.Buffer.BlockCopy(decoded, 0, bytes, byteIndex, copyLength);
                            }

                            return copyLength;
                        }
                    }

                    return decoded.Length;
                }
                catch (FormatException)
                {
                    return 0;
                }
            }

            public override int GetCharCount(byte[] bytes, int index, int count)
                => count == 0 ? 0 : System.Convert.ToBase64String(bytes, index, count).Length;

            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                if (byteCount == 0)
                {
                    return 0;
                }

                var encoded = System.Convert.ToBase64String(bytes, byteIndex, byteCount);
                encoded.CopyTo(0, chars, charIndex, encoded.Length);
                return encoded.Length;
            }

            public override int GetMaxByteCount(int charCount)
                => charCount;

            public override int GetMaxCharCount(int byteCount)
                => ((byteCount + 2) / 3) * 4;
        }

        private sealed class HexPassthroughEncoding : Encoding
        {
            public static readonly HexPassthroughEncoding Instance = new();

            public override int GetByteCount(char[] chars, int index, int count)
                => GetBytes(chars, index, count, System.Array.Empty<byte>(), 0);

            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
            {
                var text = new string(chars, charIndex, charCount);
                if (string.IsNullOrEmpty(text))
                {
                    return 0;
                }

                if ((text.Length % 2) != 0)
                {
                    text = text.Substring(0, text.Length - 1);
                }

                var written = 0;
                for (int i = 0; i < text.Length; i += 2)
                {
                    if (!byte.TryParse(text.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
                    {
                        break;
                    }

                    if (bytes.Length > 0)
                    {
                        if (byteIndex + written >= bytes.Length)
                        {
                            break;
                        }

                        bytes[byteIndex + written] = b;
                    }

                    written++;
                }

                return written;
            }

            public override int GetCharCount(byte[] bytes, int index, int count)
                => count * 2;

            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                var written = 0;
                for (int i = 0; i < byteCount; i++)
                {
                    var hex = bytes[byteIndex + i].ToString("x2", CultureInfo.InvariantCulture);
                    chars[charIndex + written] = hex[0];
                    chars[charIndex + written + 1] = hex[1];
                    written += 2;
                }

                return written;
            }

            public override int GetMaxByteCount(int charCount)
                => charCount / 2;

            public override int GetMaxCharCount(int byteCount)
                => byteCount * 2;
        }
    }
}