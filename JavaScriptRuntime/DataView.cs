using System;

namespace JavaScriptRuntime
{
    [IntrinsicObject("DataView")]
    public sealed class DataView
    {
        private readonly ArrayBuffer _buffer;
        private readonly int _byteOffset;
        private readonly int _byteLength;

        public DataView(object? buffer)
            : this(buffer, null, null)
        {
        }

        public DataView(object? buffer, object? byteOffset)
            : this(buffer, byteOffset, null)
        {
        }

        public DataView(object? buffer, object? byteOffset, object? byteLength)
        {
            if (buffer is not ArrayBuffer arrayBuffer)
            {
                throw new TypeError("First argument to DataView constructor must be an ArrayBuffer");
            }

            _buffer = arrayBuffer;
            _byteOffset = CoerceIndex(byteOffset, 0, "Invalid DataView length");

            var remaining = arrayBuffer.ByteLengthInt - _byteOffset;
            if (_byteOffset > arrayBuffer.ByteLengthInt)
            {
                throw new RangeError("Invalid DataView length");
            }

            _byteLength = byteLength is null || byteLength is JsNull
                ? remaining
                : CoerceIndex(byteLength, 0, "Invalid DataView length");

            if (_byteOffset + _byteLength > arrayBuffer.ByteLengthInt)
            {
                throw new RangeError("Invalid DataView length");
            }
        }

        public ArrayBuffer buffer => _buffer;

        public double byteOffset => _byteOffset;

        public double byteLength => _byteLength;

        public double getInt8(object? byteOffset)
            => (sbyte)ReadByte(byteOffset);

        public double getUint8(object? byteOffset)
            => ReadByte(byteOffset);

        public double getInt16(object? byteOffset)
            => getInt16(byteOffset, null);

        public double getInt16(object? byteOffset, object? littleEndian)
        {
            var index = GetAbsoluteIndex(byteOffset, 2);
            var bytes = _buffer.RawBytes;
            return UseLittleEndian(littleEndian)
                ? (short)(bytes[index] | (bytes[index + 1] << 8))
                : (short)((bytes[index] << 8) | bytes[index + 1]);
        }

        public double getUint16(object? byteOffset)
            => getUint16(byteOffset, null);

        public double getUint16(object? byteOffset, object? littleEndian)
        {
            var index = GetAbsoluteIndex(byteOffset, 2);
            var bytes = _buffer.RawBytes;
            return UseLittleEndian(littleEndian)
                ? (ushort)(bytes[index] | (bytes[index + 1] << 8))
                : (ushort)((bytes[index] << 8) | bytes[index + 1]);
        }

        public double getInt32(object? byteOffset)
            => getInt32(byteOffset, null);

        public double getInt32(object? byteOffset, object? littleEndian)
        {
            var index = GetAbsoluteIndex(byteOffset, 4);
            var bytes = _buffer.RawBytes;
            return UseLittleEndian(littleEndian)
                ? bytes[index] | (bytes[index + 1] << 8) | (bytes[index + 2] << 16) | (bytes[index + 3] << 24)
                : (bytes[index] << 24) | (bytes[index + 1] << 16) | (bytes[index + 2] << 8) | bytes[index + 3];
        }

        public double getUint32(object? byteOffset)
            => getUint32(byteOffset, null);

        public double getUint32(object? byteOffset, object? littleEndian)
        {
            var index = GetAbsoluteIndex(byteOffset, 4);
            var bytes = _buffer.RawBytes;
            return UseLittleEndian(littleEndian)
                ? (uint)(bytes[index] | (bytes[index + 1] << 8) | (bytes[index + 2] << 16) | (bytes[index + 3] << 24))
                : (uint)((bytes[index] << 24) | (bytes[index + 1] << 16) | (bytes[index + 2] << 8) | bytes[index + 3]);
        }

        public double getFloat32(object? byteOffset)
            => getFloat32(byteOffset, null);

        public double getFloat32(object? byteOffset, object? littleEndian)
            => ReadSingle(GetAbsoluteIndex(byteOffset, 4), UseLittleEndian(littleEndian));

        public double getFloat64(object? byteOffset)
            => getFloat64(byteOffset, null);

        public double getFloat64(object? byteOffset, object? littleEndian)
            => ReadDouble(GetAbsoluteIndex(byteOffset, 8), UseLittleEndian(littleEndian));

        public object? setInt8(object? byteOffset, object? value)
        {
            WriteByte(byteOffset, unchecked((byte)(sbyte)TypeUtilities.ToInt32(value)));
            return null;
        }

        public object? setUint8(object? byteOffset, object? value)
        {
            WriteByte(byteOffset, unchecked((byte)TypeUtilities.ToInt32(value)));
            return null;
        }

        public object? setInt16(object? byteOffset, object? value)
            => setInt16(byteOffset, value, null);

        public object? setInt16(object? byteOffset, object? value, object? littleEndian)
        {
            WriteUInt16(byteOffset, unchecked((ushort)(short)TypeUtilities.ToInt32(value)), UseLittleEndian(littleEndian));
            return null;
        }

        public object? setUint16(object? byteOffset, object? value)
            => setUint16(byteOffset, value, null);

        public object? setUint16(object? byteOffset, object? value, object? littleEndian)
        {
            WriteUInt16(byteOffset, unchecked((ushort)TypeUtilities.ToInt32(value)), UseLittleEndian(littleEndian));
            return null;
        }

        public object? setInt32(object? byteOffset, object? value)
            => setInt32(byteOffset, value, null);

        public object? setInt32(object? byteOffset, object? value, object? littleEndian)
        {
            WriteUInt32(byteOffset, unchecked((uint)TypeUtilities.ToInt32(value)), UseLittleEndian(littleEndian));
            return null;
        }

        public object? setUint32(object? byteOffset, object? value)
            => setUint32(byteOffset, value, null);

        public object? setUint32(object? byteOffset, object? value, object? littleEndian)
        {
            WriteUInt32(byteOffset, unchecked((uint)TypeUtilities.ToInt32(value)), UseLittleEndian(littleEndian));
            return null;
        }

        public object? setFloat32(object? byteOffset, object? value)
            => setFloat32(byteOffset, value, null);

        public object? setFloat32(object? byteOffset, object? value, object? littleEndian)
        {
            WriteSingle(byteOffset, (float)TypeUtilities.ToNumber(value), UseLittleEndian(littleEndian));
            return null;
        }

        public object? setFloat64(object? byteOffset, object? value)
            => setFloat64(byteOffset, value, null);

        public object? setFloat64(object? byteOffset, object? value, object? littleEndian)
        {
            WriteDouble(byteOffset, TypeUtilities.ToNumber(value), UseLittleEndian(littleEndian));
            return null;
        }

        private byte ReadByte(object? requestedOffset)
        {
            var index = GetAbsoluteIndex(requestedOffset, 1);
            return _buffer.RawBytes[index];
        }

        private void WriteByte(object? requestedOffset, byte value)
        {
            var index = GetAbsoluteIndex(requestedOffset, 1);
            _buffer.RawBytes[index] = value;
        }

        private void WriteUInt16(object? requestedOffset, ushort value, bool littleEndian)
        {
            var index = GetAbsoluteIndex(requestedOffset, 2);
            var bytes = _buffer.RawBytes;
            if (littleEndian)
            {
                bytes[index] = (byte)value;
                bytes[index + 1] = (byte)(value >> 8);
            }
            else
            {
                bytes[index] = (byte)(value >> 8);
                bytes[index + 1] = (byte)value;
            }
        }

        private void WriteUInt32(object? requestedOffset, uint value, bool littleEndian)
        {
            var index = GetAbsoluteIndex(requestedOffset, 4);
            var bytes = _buffer.RawBytes;
            if (littleEndian)
            {
                bytes[index] = (byte)value;
                bytes[index + 1] = (byte)(value >> 8);
                bytes[index + 2] = (byte)(value >> 16);
                bytes[index + 3] = (byte)(value >> 24);
            }
            else
            {
                bytes[index] = (byte)(value >> 24);
                bytes[index + 1] = (byte)(value >> 16);
                bytes[index + 2] = (byte)(value >> 8);
                bytes[index + 3] = (byte)value;
            }
        }

        private void WriteSingle(object? requestedOffset, float value, bool littleEndian)
        {
            var index = GetAbsoluteIndex(requestedOffset, 4);
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian != littleEndian)
            {
                System.Array.Reverse(bytes);
            }

            System.Buffer.BlockCopy(bytes, 0, _buffer.RawBytes, index, 4);
        }

        private void WriteDouble(object? requestedOffset, double value, bool littleEndian)
        {
            var index = GetAbsoluteIndex(requestedOffset, 8);
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian != littleEndian)
            {
                System.Array.Reverse(bytes);
            }

            System.Buffer.BlockCopy(bytes, 0, _buffer.RawBytes, index, 8);
        }

        private double ReadSingle(int absoluteIndex, bool littleEndian)
        {
            if (BitConverter.IsLittleEndian == littleEndian)
            {
                return BitConverter.ToSingle(_buffer.RawBytes, absoluteIndex);
            }

            var tmp = new byte[4];
            System.Buffer.BlockCopy(_buffer.RawBytes, absoluteIndex, tmp, 0, 4);
            System.Array.Reverse(tmp);
            return BitConverter.ToSingle(tmp, 0);
        }

        private double ReadDouble(int absoluteIndex, bool littleEndian)
        {
            if (BitConverter.IsLittleEndian == littleEndian)
            {
                return BitConverter.ToDouble(_buffer.RawBytes, absoluteIndex);
            }

            var tmp = new byte[8];
            System.Buffer.BlockCopy(_buffer.RawBytes, absoluteIndex, tmp, 0, 8);
            System.Array.Reverse(tmp);
            return BitConverter.ToDouble(tmp, 0);
        }

        private int GetAbsoluteIndex(object? requestedOffset, int elementSize)
        {
            var relativeIndex = CoerceIndex(requestedOffset, 0, "Offset is outside the bounds of the DataView");
            if (relativeIndex < 0 || relativeIndex + elementSize > _byteLength)
            {
                throw new RangeError("Offset is outside the bounds of the DataView");
            }

            return _byteOffset + relativeIndex;
        }

        private static bool UseLittleEndian(object? value)
            => value is not null && value is not JsNull && TypeUtilities.ToBoolean(value);

        private static int CoerceIndex(object? value, int defaultValue, string errorMessage)
        {
            if (value is null || value is JsNull)
            {
                return defaultValue;
            }

            var number = TypeUtilities.ToNumber(value);
            if (double.IsNaN(number) || number == 0.0)
            {
                return 0;
            }

            if (double.IsInfinity(number) || number < 0)
            {
                throw new RangeError(errorMessage);
            }

            var truncated = System.Math.Truncate(number);
            if (truncated > int.MaxValue)
            {
                throw new RangeError(errorMessage);
            }

            return (int)truncated;
        }
    }
}
