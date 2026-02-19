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

        public static Buffer allocUnsafe(object? size)
        {
            var length = ToLength(size);
            return new Buffer(new byte[length]);
        }

        public double readInt8(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            return (sbyte)_bytes[idx];
        }

        public double readUInt8(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            return _bytes[idx];
        }

        public double readInt16BE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 1 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            return (short)((_bytes[idx] << 8) | _bytes[idx + 1]);
        }

        public double readInt16LE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 1 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            return (short)(_bytes[idx] | (_bytes[idx + 1] << 8));
        }

        public double readUInt16BE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 1 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            return (ushort)((_bytes[idx] << 8) | _bytes[idx + 1]);
        }

        public double readUInt16LE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 1 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            return (ushort)(_bytes[idx] | (_bytes[idx + 1] << 8));
        }

        public double readInt32BE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 3 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            return (_bytes[idx] << 24) | (_bytes[idx + 1] << 16) | (_bytes[idx + 2] << 8) | _bytes[idx + 3];
        }

        public double readInt32LE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 3 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            return _bytes[idx] | (_bytes[idx + 1] << 8) | (_bytes[idx + 2] << 16) | (_bytes[idx + 3] << 24);
        }

        public double readUInt32BE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 3 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            return (uint)((_bytes[idx] << 24) | (_bytes[idx + 1] << 16) | (_bytes[idx + 2] << 8) | _bytes[idx + 3]);
        }

        public double readUInt32LE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 3 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            return (uint)(_bytes[idx] | (_bytes[idx + 1] << 8) | (_bytes[idx + 2] << 16) | (_bytes[idx + 3] << 24));
        }

        public double writeInt8(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            var intValue = (int)TypeUtilities.ToNumber(value);
            _bytes[idx] = (byte)(sbyte)intValue;
            return idx + 1;
        }

        public double writeUInt8(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            _bytes[idx] = ToUint8(value);
            return idx + 1;
        }

        public double writeInt16BE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 1 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            var intValue = (short)TypeUtilities.ToNumber(value);
            _bytes[idx] = (byte)(intValue >> 8);
            _bytes[idx + 1] = (byte)intValue;
            return idx + 2;
        }

        public double writeInt16LE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 1 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            var intValue = (short)TypeUtilities.ToNumber(value);
            _bytes[idx] = (byte)intValue;
            _bytes[idx + 1] = (byte)(intValue >> 8);
            return idx + 2;
        }

        public double writeUInt16BE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 1 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            var intValue = (ushort)TypeUtilities.ToNumber(value);
            _bytes[idx] = (byte)(intValue >> 8);
            _bytes[idx + 1] = (byte)intValue;
            return idx + 2;
        }

        public double writeUInt16LE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 1 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            var intValue = (ushort)TypeUtilities.ToNumber(value);
            _bytes[idx] = (byte)intValue;
            _bytes[idx + 1] = (byte)(intValue >> 8);
            return idx + 2;
        }

        public double writeInt32BE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 3 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            var intValue = (int)TypeUtilities.ToNumber(value);
            _bytes[idx] = (byte)(intValue >> 24);
            _bytes[idx + 1] = (byte)(intValue >> 16);
            _bytes[idx + 2] = (byte)(intValue >> 8);
            _bytes[idx + 3] = (byte)intValue;
            return idx + 4;
        }

        public double writeInt32LE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 3 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            var intValue = (int)TypeUtilities.ToNumber(value);
            _bytes[idx] = (byte)intValue;
            _bytes[idx + 1] = (byte)(intValue >> 8);
            _bytes[idx + 2] = (byte)(intValue >> 16);
            _bytes[idx + 3] = (byte)(intValue >> 24);
            return idx + 4;
        }

        public double writeUInt32BE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 3 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            var intValue = (uint)TypeUtilities.ToNumber(value);
            _bytes[idx] = (byte)(intValue >> 24);
            _bytes[idx + 1] = (byte)(intValue >> 16);
            _bytes[idx + 2] = (byte)(intValue >> 8);
            _bytes[idx + 3] = (byte)intValue;
            return idx + 4;
        }

        public double writeUInt32LE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            if (idx < 0 || idx + 3 >= _bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds");
            }
            var intValue = (uint)TypeUtilities.ToNumber(value);
            _bytes[idx] = (byte)intValue;
            _bytes[idx + 1] = (byte)(intValue >> 8);
            _bytes[idx + 2] = (byte)(intValue >> 16);
            _bytes[idx + 3] = (byte)(intValue >> 24);
            return idx + 4;
        }

        public double write(object? text)
        {
            return write(text, null, null, null);
        }

        public double write(object? text, object? offset)
        {
            return write(text, offset, null, null);
        }

        public double write(object? text, object? offset, object? length)
        {
            return write(text, offset, length, null);
        }

        public double write(object? text, object? offset, object? length, object? encoding)
        {
            var str = DotNet2JSConversions.ToString(text);
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }

            var enc = ResolveEncoding(encoding);
            var bytes = enc.GetBytes(str);

            var idx = CoerceToIndex(offset, 0, _bytes.Length);
            var maxLen = _bytes.Length - idx;

            int len;
            if (length == null || length is JsNull)
            {
                len = maxLen;
            }
            else
            {
                len = System.Math.Min(ToLength(length), maxLen);
            }

            var bytesToWrite = System.Math.Min(bytes.Length, len);
            if (bytesToWrite > 0)
            {
                System.Buffer.BlockCopy(bytes, 0, _bytes, idx, bytesToWrite);
            }

            return bytesToWrite;
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

        public static double compare(object? buf1, object? buf2)
        {
            if (buf1 is not Buffer buffer1)
            {
                throw new ArgumentException("First argument must be a Buffer");
            }

            if (buf2 is not Buffer buffer2)
            {
                throw new ArgumentException("Second argument must be a Buffer");
            }

            var len = System.Math.Min(buffer1._bytes.Length, buffer2._bytes.Length);
            for (int i = 0; i < len; i++)
            {
                if (buffer1._bytes[i] != buffer2._bytes[i])
                {
                    return buffer1._bytes[i] < buffer2._bytes[i] ? -1 : 1;
                }
            }

            if (buffer1._bytes.Length < buffer2._bytes.Length) return -1;
            if (buffer1._bytes.Length > buffer2._bytes.Length) return 1;
            return 0;
        }

        public string toString()
        {
            return toString("utf8");
        }

        public string toString(object? encoding)
        {
            return ResolveEncoding(encoding).GetString(_bytes);
        }

        public Buffer slice()
        {
            return slice(null, null);
        }

        public Buffer slice(object? start)
        {
            return slice(start, null);
        }

        public Buffer slice(object? start, object? end)
        {
            var len = _bytes.Length;
            var startIdx = CoerceToIndex(start, 0, len);
            var endIdx = CoerceToIndex(end, len, len);

            if (startIdx >= endIdx || startIdx >= len)
            {
                return new Buffer(System.Array.Empty<byte>());
            }

            var sliceLength = endIdx - startIdx;
            var sliced = new byte[sliceLength];
            System.Buffer.BlockCopy(_bytes, startIdx, sliced, 0, sliceLength);
            return new Buffer(sliced);
        }

        public double copy(object? target)
        {
            return copy(target, null, null, null);
        }

        public double copy(object? target, object? targetStart)
        {
            return copy(target, targetStart, null, null);
        }

        public double copy(object? target, object? targetStart, object? sourceStart)
        {
            return copy(target, targetStart, sourceStart, null);
        }

        public double copy(object? target, object? targetStart, object? sourceStart, object? sourceEnd)
        {
            if (target is not Buffer targetBuffer)
            {
                throw new ArgumentException("Target must be a Buffer");
            }

            var targetLength = targetBuffer._bytes.Length;
            var sourceLength = _bytes.Length;

            var targetIdx = CoerceToIndex(targetStart, 0, targetLength);
            var sourceStartIdx = CoerceToIndex(sourceStart, 0, sourceLength);
            var sourceEndIdx = CoerceToIndex(sourceEnd, sourceLength, sourceLength);

            if (sourceStartIdx >= sourceEndIdx || sourceStartIdx >= sourceLength)
            {
                return 0;
            }

            var bytesToCopy = System.Math.Min(sourceEndIdx - sourceStartIdx, targetLength - targetIdx);
            if (bytesToCopy <= 0)
            {
                return 0;
            }

            System.Buffer.BlockCopy(_bytes, sourceStartIdx, targetBuffer._bytes, targetIdx, bytesToCopy);
            return bytesToCopy;
        }

        public object? this[double index]
        {
            get
            {
                var idx = (int)index;
                if (idx < 0 || idx >= _bytes.Length)
                {
                    return null; // undefined
                }
                return (double)_bytes[idx];
            }
            set
            {
                var idx = (int)index;
                if (idx >= 0 && idx < _bytes.Length)
                {
                    _bytes[idx] = ToUint8(value);
                }
            }
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

        private static int CoerceToIndex(object? value, int defaultValue, int length)
        {
            if (value == null || value is JsNull)
            {
                return defaultValue;
            }

            double n;
            try
            {
                n = TypeUtilities.ToNumber(value);
            }
            catch
            {
                return defaultValue;
            }

            if (double.IsNaN(n))
            {
                return defaultValue;
            }

            if (double.IsInfinity(n))
            {
                return n > 0 ? length : 0;
            }

            var index = (int)System.Math.Truncate(n);
            if (index < 0)
            {
                index = System.Math.Max(0, length + index);
            }

            return System.Math.Min(index, length);
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