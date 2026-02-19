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
        private readonly int _offset;
        private readonly int _length;

        public Buffer(byte[] bytes)
            : this(bytes ?? System.Array.Empty<byte>(), 0, bytes?.Length ?? 0)
        {
        }

        private Buffer(byte[] bytes, int offset, int length)
        {
            _bytes = bytes ?? System.Array.Empty<byte>();
            _offset = offset;
            _length = length;
        }

        public double length => _length;

        public static Buffer from(object? value)
        {
            return from(value, null);
        }

        public static Buffer from(object? value, object? encoding)
        {
            if (value is Buffer buffer)
            {
                return new Buffer(buffer.ToByteArray());
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
            return new Buffer(GC.AllocateUninitializedArray<byte>(length));
        }

        public double readInt8(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            return (sbyte)_bytes[_offset + idx];
        }

        public double readUInt8(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            return _bytes[_offset + idx];
        }

        public double readInt16BE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 1 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            return (short)((_bytes[_offset + idx] << 8) | _bytes[_offset + idx + 1]);
        }

        public double readInt16LE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 1 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            return (short)(_bytes[_offset + idx] | (_bytes[_offset + idx + 1] << 8));
        }

        public double readUInt16BE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 1 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            return (ushort)((_bytes[_offset + idx] << 8) | _bytes[_offset + idx + 1]);
        }

        public double readUInt16LE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 1 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            return (ushort)(_bytes[_offset + idx] | (_bytes[_offset + idx + 1] << 8));
        }

        public double readInt32BE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 3 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            return (_bytes[_offset + idx] << 24) | (_bytes[_offset + idx + 1] << 16) | (_bytes[_offset + idx + 2] << 8) | _bytes[_offset + idx + 3];
        }

        public double readInt32LE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 3 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            return _bytes[_offset + idx] | (_bytes[_offset + idx + 1] << 8) | (_bytes[_offset + idx + 2] << 16) | (_bytes[_offset + idx + 3] << 24);
        }

        public double readUInt32BE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 3 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            return (uint)((_bytes[_offset + idx] << 24) | (_bytes[_offset + idx + 1] << 16) | (_bytes[_offset + idx + 2] << 8) | _bytes[_offset + idx + 3]);
        }

        public double readUInt32LE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 3 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            return (uint)(_bytes[_offset + idx] | (_bytes[_offset + idx + 1] << 8) | (_bytes[_offset + idx + 2] << 16) | (_bytes[_offset + idx + 3] << 24));
        }

        public double writeInt8(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            var intValue = (int)TypeUtilities.ToNumber(value);
            _bytes[_offset + idx] = (byte)(sbyte)intValue;
            return idx + 1;
        }

        public double writeUInt8(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            _bytes[_offset + idx] = ToUint8(value);
            return idx + 1;
        }

        public double writeInt16BE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 1 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            var intValue = (short)TypeUtilities.ToNumber(value);
            _bytes[_offset + idx] = (byte)(intValue >> 8);
            _bytes[_offset + idx + 1] = (byte)intValue;
            return idx + 2;
        }

        public double writeInt16LE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 1 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            var intValue = (short)TypeUtilities.ToNumber(value);
            _bytes[_offset + idx] = (byte)intValue;
            _bytes[_offset + idx + 1] = (byte)(intValue >> 8);
            return idx + 2;
        }

        public double writeUInt16BE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 1 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            var intValue = (ushort)TypeUtilities.ToNumber(value);
            _bytes[_offset + idx] = (byte)(intValue >> 8);
            _bytes[_offset + idx + 1] = (byte)intValue;
            return idx + 2;
        }

        public double writeUInt16LE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 1 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            var intValue = (ushort)TypeUtilities.ToNumber(value);
            _bytes[_offset + idx] = (byte)intValue;
            _bytes[_offset + idx + 1] = (byte)(intValue >> 8);
            return idx + 2;
        }

        public double writeInt32BE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 3 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            var intValue = (int)TypeUtilities.ToNumber(value);
            _bytes[_offset + idx] = (byte)(intValue >> 24);
            _bytes[_offset + idx + 1] = (byte)(intValue >> 16);
            _bytes[_offset + idx + 2] = (byte)(intValue >> 8);
            _bytes[_offset + idx + 3] = (byte)intValue;
            return idx + 4;
        }

        public double writeInt32LE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 3 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            var intValue = (int)TypeUtilities.ToNumber(value);
            _bytes[_offset + idx] = (byte)intValue;
            _bytes[_offset + idx + 1] = (byte)(intValue >> 8);
            _bytes[_offset + idx + 2] = (byte)(intValue >> 16);
            _bytes[_offset + idx + 3] = (byte)(intValue >> 24);
            return idx + 4;
        }

        public double writeUInt32BE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 3 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            var intValue = (uint)TypeUtilities.ToNumber(value);
            _bytes[_offset + idx] = (byte)(intValue >> 24);
            _bytes[_offset + idx + 1] = (byte)(intValue >> 16);
            _bytes[_offset + idx + 2] = (byte)(intValue >> 8);
            _bytes[_offset + idx + 3] = (byte)intValue;
            return idx + 4;
        }

        public double writeUInt32LE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 3 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }
            var intValue = (uint)TypeUtilities.ToNumber(value);
            _bytes[_offset + idx] = (byte)intValue;
            _bytes[_offset + idx + 1] = (byte)(intValue >> 8);
            _bytes[_offset + idx + 2] = (byte)(intValue >> 16);
            _bytes[_offset + idx + 3] = (byte)(intValue >> 24);
            return idx + 4;
        }

        public double readFloatLE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 3 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }

            return ReadSingleAt(idx, littleEndian: true);
        }

        public double readFloatBE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 3 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }

            return ReadSingleAt(idx, littleEndian: false);
        }

        public double readDoubleLE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 7 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }

            return ReadDoubleAt(idx, littleEndian: true);
        }

        public double readDoubleBE(object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 7 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }

            return ReadDoubleAt(idx, littleEndian: false);
        }

        public double writeFloatLE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 3 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }

            WriteSingleAt((float)TypeUtilities.ToNumber(value), idx, littleEndian: true);
            return idx + 4;
        }

        public double writeFloatBE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 3 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }

            WriteSingleAt((float)TypeUtilities.ToNumber(value), idx, littleEndian: false);
            return idx + 4;
        }

        public double writeDoubleLE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 7 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }

            WriteDoubleAt(TypeUtilities.ToNumber(value), idx, littleEndian: true);
            return idx + 8;
        }

        public double writeDoubleBE(object? value, object? offset)
        {
            var idx = CoerceToIndex(offset, 0, _length);
            if (idx < 0 || idx + 7 >= _length)
            {
                throw CreateOffsetOutOfRangeError();
            }

            WriteDoubleAt(TypeUtilities.ToNumber(value), idx, littleEndian: false);
            return idx + 8;
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

            object? effectiveEncodingArg;
            object? effectiveLengthArg;
            if (length is string)
            {
                effectiveEncodingArg = length;
                effectiveLengthArg = null;
            }
            else
            {
                effectiveEncodingArg = encoding;
                effectiveLengthArg = length;
            }

            var enc = ResolveEncoding(effectiveEncodingArg);
            var bytes = enc.GetBytes(str);

            var idx = CoerceToIndex(offset, 0, _length);
            var maxLen = _length - idx;

            int len;
            if (effectiveLengthArg == null || effectiveLengthArg is JsNull)
            {
                len = maxLen;
            }
            else
            {
                len = System.Math.Min(ToLength(effectiveLengthArg), maxLen);
            }

            var bytesToWrite = System.Math.Min(bytes.Length, len);
            if (bytesToWrite > 0)
            {
                System.Buffer.BlockCopy(bytes, 0, _bytes, _offset + idx, bytesToWrite);
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
                throw new global::JavaScriptRuntime.TypeError("The \"buf1\" argument must be an instance of Buffer or Uint8Array.");
            }

            if (buf2 is not Buffer buffer2)
            {
                throw new global::JavaScriptRuntime.TypeError("The \"buf2\" argument must be an instance of Buffer or Uint8Array.");
            }

            var len = System.Math.Min(buffer1._length, buffer2._length);
            for (int i = 0; i < len; i++)
            {
                var b1 = buffer1._bytes[buffer1._offset + i];
                var b2 = buffer2._bytes[buffer2._offset + i];
                if (b1 != b2)
                {
                    return b1 < b2 ? -1 : 1;
                }
            }

            if (buffer1._length < buffer2._length) return -1;
            if (buffer1._length > buffer2._length) return 1;
            return 0;
        }

        public string toString()
        {
            return toString("utf8");
        }

        public string toString(object? encoding)
        {
            return toString(encoding, null, null);
        }

        public string toString(object? encoding, object? start)
        {
            return toString(encoding, start, null);
        }

        public string toString(object? encoding, object? start, object? end)
        {
            var enc = ResolveEncoding(encoding);
            var len = _length;

            var startIdx = CoerceToIndex(start, 0, len);
            var endIdx = CoerceToIndex(end, len, len);

            if (startIdx >= endIdx || startIdx >= len)
            {
                return string.Empty;
            }

            return enc.GetString(_bytes, _offset + startIdx, endIdx - startIdx);
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
            var len = _length;
            var startIdx = CoerceToIndex(start, 0, len);
            var endIdx = CoerceToIndex(end, len, len);

            if (startIdx >= endIdx || startIdx >= len)
            {
                return new Buffer(System.Array.Empty<byte>());
            }

            var sliceLength = endIdx - startIdx;
            return new Buffer(_bytes, _offset + startIdx, sliceLength);
        }

        public Buffer subarray()
        {
            return subarray(null, null);
        }

        public Buffer subarray(object? start)
        {
            return subarray(start, null);
        }

        public Buffer subarray(object? start, object? end)
        {
            return slice(start, end);
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

            var targetLength = targetBuffer._length;
            var sourceLength = _length;

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

            System.Buffer.BlockCopy(_bytes, _offset + sourceStartIdx, targetBuffer._bytes, targetBuffer._offset + targetIdx, bytesToCopy);
            return bytesToCopy;
        }

        public bool equals(object? other)
        {
            if (other is not Buffer otherBuffer)
            {
                throw new global::JavaScriptRuntime.TypeError("The \"otherBuffer\" argument must be an instance of Buffer or Uint8Array.");
            }

            if (_length != otherBuffer._length)
            {
                return false;
            }

            for (int i = 0; i < _length; i++)
            {
                if (_bytes[_offset + i] != otherBuffer._bytes[otherBuffer._offset + i])
                {
                    return false;
                }
            }

            return true;
        }

        public double indexOf(object? value)
        {
            return indexOf(value, null, null);
        }

        public double indexOf(object? value, object? byteOffset)
        {
            return indexOf(value, byteOffset, null);
        }

        public double indexOf(object? value, object? byteOffset, object? encoding)
        {
            var needle = CoerceSearchBytes(value, encoding);
            var startIndex = CoerceToIndex(byteOffset, 0, _length);
            return FindForward(needle, startIndex);
        }

        public double lastIndexOf(object? value)
        {
            return lastIndexOf(value, null, null);
        }

        public double lastIndexOf(object? value, object? byteOffset)
        {
            return lastIndexOf(value, byteOffset, null);
        }

        public double lastIndexOf(object? value, object? byteOffset, object? encoding)
        {
            var needle = CoerceSearchBytes(value, encoding);
            var startIndex = byteOffset == null || byteOffset is JsNull
                ? _length - 1
                : CoerceToIndex(byteOffset, 0, _length);
            return FindBackward(needle, startIndex);
        }

        public bool includes(object? value)
        {
            return includes(value, null, null);
        }

        public bool includes(object? value, object? byteOffset)
        {
            return includes(value, byteOffset, null);
        }

        public bool includes(object? value, object? byteOffset, object? encoding)
        {
            return indexOf(value, byteOffset, encoding) >= 0;
        }

        public Buffer fill(object? value)
        {
            return fill(value, null, null, null);
        }

        public Buffer fill(object? value, object? offset)
        {
            return fill(value, offset, null, null);
        }

        public Buffer fill(object? value, object? offset, object? end)
        {
            return fill(value, offset, end, null);
        }

        public Buffer fill(object? value, object? offset, object? end, object? encoding)
        {
            var startIdx = CoerceToIndex(offset, 0, _length);
            var endIdx = CoerceToIndex(end, _length, _length);
            if (startIdx >= endIdx)
            {
                return this;
            }

            byte[] fillBytes;
            if (value is Buffer buffer)
            {
                fillBytes = buffer.ToByteArray();
            }
            else if (value is byte[] byteFill)
            {
                fillBytes = byteFill;
            }
            else if (value is string fillText)
            {
                fillBytes = ResolveEncoding(encoding).GetBytes(fillText);
            }
            else
            {
                fillBytes = new[] { ToUint8(value) };
            }

            if (fillBytes.Length == 0)
            {
                return this;
            }

            for (int i = startIdx; i < endIdx; i++)
            {
                _bytes[_offset + i] = fillBytes[(i - startIdx) % fillBytes.Length];
            }

            return this;
        }

        public object? this[double index]
        {
            get
            {
                if (!TryGetValidElementIndex(index, out var idx))
                {
                    return null; // undefined
                }
                return (double)_bytes[_offset + idx];
            }
            set
            {
                if (TryGetValidElementIndex(index, out var idx))
                {
                    _bytes[_offset + idx] = ToUint8(value);
                }
            }
        }

        internal static Buffer FromBytes(byte[] bytes)
        {
            return new Buffer((byte[])bytes.Clone());
        }

        internal byte[] ToByteArray()
        {
            if (_length == 0)
            {
                return System.Array.Empty<byte>();
            }

            var result = new byte[_length];
            System.Buffer.BlockCopy(_bytes, _offset, result, 0, _length);
            return result;
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

        private static Exception CreateOffsetOutOfRangeError()
        {
            return new global::JavaScriptRuntime.RangeError("The value of \"offset\" is out of range.");
        }

        private bool TryGetValidElementIndex(double index, out int normalizedIndex)
        {
            normalizedIndex = 0;

            if (double.IsNaN(index) || double.IsInfinity(index) || index % 1.0 != 0.0)
            {
                return false;
            }

            if (index < 0 || index > int.MaxValue)
            {
                return false;
            }

            normalizedIndex = (int)index;
            return normalizedIndex < _length;
        }

        private double ReadSingleAt(int idx, bool littleEndian)
        {
            if (BitConverter.IsLittleEndian == littleEndian)
            {
                return BitConverter.ToSingle(_bytes, _offset + idx);
            }

            var tmp = new byte[4];
            System.Buffer.BlockCopy(_bytes, _offset + idx, tmp, 0, 4);
            System.Array.Reverse(tmp);
            return BitConverter.ToSingle(tmp, 0);
        }

        private double ReadDoubleAt(int idx, bool littleEndian)
        {
            if (BitConverter.IsLittleEndian == littleEndian)
            {
                return BitConverter.ToDouble(_bytes, _offset + idx);
            }

            var tmp = new byte[8];
            System.Buffer.BlockCopy(_bytes, _offset + idx, tmp, 0, 8);
            System.Array.Reverse(tmp);
            return BitConverter.ToDouble(tmp, 0);
        }

        private void WriteSingleAt(float value, int idx, bool littleEndian)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian != littleEndian)
            {
                System.Array.Reverse(bytes);
            }

            System.Buffer.BlockCopy(bytes, 0, _bytes, _offset + idx, 4);
        }

        private void WriteDoubleAt(double value, int idx, bool littleEndian)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian != littleEndian)
            {
                System.Array.Reverse(bytes);
            }

            System.Buffer.BlockCopy(bytes, 0, _bytes, _offset + idx, 8);
        }

        private byte[] CoerceSearchBytes(object? value, object? encoding)
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
                return ResolveEncoding(encoding).GetBytes(text);
            }

            return new[] { ToUint8(value) };
        }

        private double FindForward(byte[] needle, int startIndex)
        {
            if (needle.Length == 0)
            {
                return startIndex;
            }

            for (int i = startIndex; i <= _length - needle.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < needle.Length; j++)
                {
                    if (_bytes[_offset + i + j] != needle[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return i;
                }
            }

            return -1;
        }

        private double FindBackward(byte[] needle, int startIndex)
        {
            if (needle.Length == 0)
            {
                return System.Math.Min(startIndex, _length);
            }

            int start = System.Math.Min(startIndex, _length - needle.Length);
            for (int i = start; i >= 0; i--)
            {
                bool match = true;
                for (int j = 0; j < needle.Length; j++)
                {
                    if (_bytes[_offset + i + j] != needle[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return i;
                }
            }

            return -1;
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