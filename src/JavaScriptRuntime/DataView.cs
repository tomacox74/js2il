using System;
using System.Buffers.Binary;
using System.Numerics;

namespace JavaScriptRuntime
{
    [IntrinsicObject("DataView")]
    public sealed class DataView
    {
        private static readonly BigInteger BigInt64Modulus = BigInteger.One << 64;

        /// <summary>Realm-owned <c>DataView.prototype</c> (issue #1824).</summary>
        internal static object Prototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.DataViewPrototype,
                static () => new JsObject());

        internal static void ConfigureIntrinsicSurface(object constructorValue, object objectPrototype)
        {
            GlobalThis.ConfigureConstructorPrototypeSurface(constructorValue, Prototype, objectPrototype);
            PropertyDescriptorStore.DefineOrUpdate(constructorValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = 1d
            });
            PropertyDescriptorStore.DefineOrUpdate(constructorValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "DataView"
            });
            DefineDataViewAccessor("buffer", DataViewBufferGetter);
            DefineDataViewAccessor("byteLength", DataViewByteLengthGetter);
            DefineDataViewAccessor("byteOffset", DataViewByteOffsetGetter);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "getInt8", (BuiltinFunction1)DataViewGetInt8, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "getUint8", (BuiltinFunction1)DataViewGetUint8, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "getInt16", (BuiltinFunction2)DataViewGetInt16, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "getUint16", (BuiltinFunction2)DataViewGetUint16, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "getInt32", (BuiltinFunction2)DataViewGetInt32, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "getUint32", (BuiltinFunction2)DataViewGetUint32, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "getFloat16", (BuiltinFunction2)DataViewGetFloat16, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "getFloat32", (BuiltinFunction2)DataViewGetFloat32, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "getFloat64", (BuiltinFunction2)DataViewGetFloat64, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "getBigInt64", (BuiltinFunction2)DataViewGetBigInt64, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "getBigUint64", (BuiltinFunction2)DataViewGetBigUint64, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "setInt8", (BuiltinFunction2)DataViewSetInt8, 2d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "setUint8", (BuiltinFunction2)DataViewSetUint8, 2d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "setInt16", (BuiltinFunction3)DataViewSetInt16, 2d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "setUint16", (BuiltinFunction3)DataViewSetUint16, 2d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "setInt32", (BuiltinFunction3)DataViewSetInt32, 2d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "setUint32", (BuiltinFunction3)DataViewSetUint32, 2d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "setFloat16", (BuiltinFunction3)DataViewSetFloat16, 2d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "setFloat32", (BuiltinFunction3)DataViewSetFloat32, 2d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "setFloat64", (BuiltinFunction3)DataViewSetFloat64, 2d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "setBigInt64", (BuiltinFunction3)DataViewSetBigInt64, 2d);
            GlobalThis.DefineBuiltinFunctionProperty(Prototype, "setBigUint64", (BuiltinFunction3)DataViewSetBigUint64, 2d);
            GlobalThis.DefineIntrinsicToStringTagProperty(Prototype, "DataView");
        }

        private static void DefineDataViewAccessor(
            string propertyName,
            BuiltinFunction0 getter)
        {
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                getter,
                0d,
                $"get {propertyName}",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(getter));
            PropertyDescriptorStore.DefineOrUpdate(Prototype, propertyName, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = getter
            });
        }

        private static object? DataViewBufferGetter(object? thisArgument)
            => GetDataViewThis(thisArgument, "buffer", isAccessor: true).buffer;

        private static object? DataViewByteLengthGetter(object? thisArgument)
            => GetDataViewThis(thisArgument, "byteLength", isAccessor: true).byteLength;

        private static object? DataViewByteOffsetGetter(object? thisArgument)
            => GetDataViewThis(thisArgument, "byteOffset", isAccessor: true).byteOffset;

        private static object? DataViewGetInt8(object? thisArgument, object? byteOffset)
            => GetDataViewThis(thisArgument, "getInt8").getInt8(byteOffset);

        private static object? DataViewGetUint8(object? thisArgument, object? byteOffset)
            => GetDataViewThis(thisArgument, "getUint8").getUint8(byteOffset);

        private static object? DataViewGetInt16(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getInt16").getInt16(byteOffset, littleEndian);

        private static object? DataViewGetUint16(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getUint16").getUint16(byteOffset, littleEndian);

        private static object? DataViewGetInt32(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getInt32").getInt32(byteOffset, littleEndian);

        private static object? DataViewGetUint32(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getUint32").getUint32(byteOffset, littleEndian);

        private static object? DataViewGetFloat16(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getFloat16").getFloat16(byteOffset, littleEndian);

        private static object? DataViewGetFloat32(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getFloat32").getFloat32(byteOffset, littleEndian);

        private static object? DataViewGetFloat64(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getFloat64").getFloat64(byteOffset, littleEndian);

        private static object? DataViewGetBigInt64(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getBigInt64").getBigInt64(byteOffset, littleEndian);

        private static object? DataViewGetBigUint64(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getBigUint64").getBigUint64(byteOffset, littleEndian);

        private static object? DataViewSetInt8(object? thisArgument, object? byteOffset, object? value)
            => GetDataViewThis(thisArgument, "setInt8").setInt8(byteOffset, value);

        private static object? DataViewSetUint8(object? thisArgument, object? byteOffset, object? value)
            => GetDataViewThis(thisArgument, "setUint8").setUint8(byteOffset, value);

        private static object? DataViewSetInt16(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setInt16").setInt16(byteOffset, value, littleEndian);

        private static object? DataViewSetUint16(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setUint16").setUint16(byteOffset, value, littleEndian);

        private static object? DataViewSetInt32(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setInt32").setInt32(byteOffset, value, littleEndian);

        private static object? DataViewSetUint32(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setUint32").setUint32(byteOffset, value, littleEndian);

        private static object? DataViewSetFloat16(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setFloat16").setFloat16(byteOffset, value, littleEndian);

        private static object? DataViewSetFloat32(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setFloat32").setFloat32(byteOffset, value, littleEndian);

        private static object? DataViewSetFloat64(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setFloat64").setFloat64(byteOffset, value, littleEndian);

        private static object? DataViewSetBigInt64(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setBigInt64").setBigInt64(byteOffset, value, littleEndian);

        private static object? DataViewSetBigUint64(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setBigUint64").setBigUint64(byteOffset, value, littleEndian);

        private static DataView GetDataViewThis(
            object? thisArgument,
            string memberName,
            bool isAccessor = false)
        {
            if (thisArgument is not DataView dataView)
            {
                var prefix = isAccessor ? "get " : string.Empty;
                throw new TypeError($"{prefix}DataView.prototype.{memberName} called on incompatible receiver");
            }

            return dataView;
        }

        private readonly ArrayBuffer _buffer;
        private readonly int _byteOffset;
        private readonly int _byteLength;
        private readonly bool _isLengthTracking;

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
            var requestedByteOffset = CoerceIndex(byteOffset, 0, "Invalid DataView byteOffset");
            arrayBuffer.EnsureAttached();

            if (requestedByteOffset > arrayBuffer.ByteLengthInt)
            {
                throw new RangeError("Invalid DataView byteOffset");
            }

            _byteOffset = (int)requestedByteOffset;
            long remainingLong = (long)arrayBuffer.ByteLengthInt - _byteOffset;
            var remaining = (int)remainingLong;
            _isLengthTracking = byteLength is null && arrayBuffer.IsResizable;
            var requestedByteLength = byteLength is null
                ? remaining
                : CoerceIndex(byteLength, 0, "Invalid DataView byteLength");
            arrayBuffer.EnsureAttached();

            if (_byteOffset + requestedByteLength > arrayBuffer.ByteLengthInt)
            {
                throw new RangeError("Invalid DataView byteLength");
            }

            _byteLength = (int)requestedByteLength;
            PrototypeChain.SetPrototype(this, Prototype);
        }

        public ArrayBuffer buffer => _buffer;

        public double byteOffset
        {
            get
            {
                _buffer.EnsureAttached();
                if (IsOutOfBounds)
                {
                    throw new TypeError("DataView is out of bounds");
                }

                return _byteOffset;
            }
        }

        public double byteLength
        {
            get
            {
                _buffer.EnsureAttached();
                if (IsOutOfBounds)
                {
                    throw new TypeError("DataView is out of bounds");
                }

                return CurrentByteLength;
            }
        }

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

        public double getFloat16(object? byteOffset)
            => getFloat16(byteOffset, null);

        public double getFloat16(object? byteOffset, object? littleEndian)
        {
            var index = GetAbsoluteIndex(byteOffset, 2);
            var bytes = _buffer.RawBytes;
            var bits = UseLittleEndian(littleEndian)
                ? (ushort)(bytes[index] | (bytes[index + 1] << 8))
                : (ushort)((bytes[index] << 8) | bytes[index + 1]);
            return (double)BitConverter.UInt16BitsToHalf(bits);
        }

        public double getFloat32(object? byteOffset)
            => getFloat32(byteOffset, null);

        public double getFloat32(object? byteOffset, object? littleEndian)
            => ReadSingle(GetAbsoluteIndex(byteOffset, 4), UseLittleEndian(littleEndian));

        public double getFloat64(object? byteOffset)
            => getFloat64(byteOffset, null);

        public double getFloat64(object? byteOffset, object? littleEndian)
            => ReadDouble(GetAbsoluteIndex(byteOffset, 8), UseLittleEndian(littleEndian));

        public object getBigInt64(object? byteOffset)
            => getBigInt64(byteOffset, null);

        public object getBigInt64(object? byteOffset, object? littleEndian)
        {
            var index = GetAbsoluteIndex(byteOffset, 8);
            var bytes = _buffer.RawBytes.AsSpan(index, 8);
            var value = UseLittleEndian(littleEndian)
                ? BinaryPrimitives.ReadInt64LittleEndian(bytes)
                : BinaryPrimitives.ReadInt64BigEndian(bytes);
            return new BigInteger(value);
        }

        public object getBigUint64(object? byteOffset)
            => getBigUint64(byteOffset, null);

        public object getBigUint64(object? byteOffset, object? littleEndian)
        {
            var index = GetAbsoluteIndex(byteOffset, 8);
            var bytes = _buffer.RawBytes.AsSpan(index, 8);
            var value = UseLittleEndian(littleEndian)
                ? BinaryPrimitives.ReadUInt64LittleEndian(bytes)
                : BinaryPrimitives.ReadUInt64BigEndian(bytes);
            return new BigInteger(value);
        }

        public object? setInt8(object? byteOffset, object? value)
        {
            EnsureMutableBuffer();
            var index = CoerceIndex(byteOffset, 0, "Offset is outside the bounds of the DataView");
            WriteByte(index, unchecked((byte)TypeUtilities.ToInt8(value)));
            return null;
        }

        public object? setUint8(object? byteOffset, object? value)
        {
            EnsureMutableBuffer();
            var index = CoerceIndex(byteOffset, 0, "Offset is outside the bounds of the DataView");
            WriteByte(index, TypeUtilities.ToUint8(value));
            return null;
        }

        public object? setInt16(object? byteOffset, object? value)
            => setInt16(byteOffset, value, null);

        public object? setInt16(object? byteOffset, object? value, object? littleEndian)
        {
            EnsureMutableBuffer();
            var index = CoerceIndex(byteOffset, 0, "Offset is outside the bounds of the DataView");
            WriteUInt16(index, unchecked((ushort)TypeUtilities.ToInt16(value)), UseLittleEndian(littleEndian));
            return null;
        }

        public object? setUint16(object? byteOffset, object? value)
            => setUint16(byteOffset, value, null);

        public object? setUint16(object? byteOffset, object? value, object? littleEndian)
        {
            EnsureMutableBuffer();
            var index = CoerceIndex(byteOffset, 0, "Offset is outside the bounds of the DataView");
            WriteUInt16(index, TypeUtilities.ToUint16(value), UseLittleEndian(littleEndian));
            return null;
        }

        public object? setInt32(object? byteOffset, object? value)
            => setInt32(byteOffset, value, null);

        public object? setInt32(object? byteOffset, object? value, object? littleEndian)
        {
            EnsureMutableBuffer();
            var index = CoerceIndex(byteOffset, 0, "Offset is outside the bounds of the DataView");
            WriteUInt32(index, unchecked((uint)TypeUtilities.ToInt32(value)), UseLittleEndian(littleEndian));
            return null;
        }

        public object? setUint32(object? byteOffset, object? value)
            => setUint32(byteOffset, value, null);

        public object? setUint32(object? byteOffset, object? value, object? littleEndian)
        {
            EnsureMutableBuffer();
            var index = CoerceIndex(byteOffset, 0, "Offset is outside the bounds of the DataView");
            WriteUInt32(index, unchecked((uint)TypeUtilities.ToInt32(value)), UseLittleEndian(littleEndian));
            return null;
        }

        public object? setFloat16(object? byteOffset, object? value)
            => setFloat16(byteOffset, value, null);

        public object? setFloat16(object? byteOffset, object? value, object? littleEndian)
        {
            EnsureMutableBuffer();
            var index = CoerceIndex(byteOffset, 0, "Offset is outside the bounds of the DataView");
            var bits = BitConverter.HalfToUInt16Bits((Half)TypeUtilities.ToNumber(value));
            WriteUInt16(index, bits, UseLittleEndian(littleEndian));
            return null;
        }

        public object? setFloat32(object? byteOffset, object? value)
            => setFloat32(byteOffset, value, null);

        public object? setFloat32(object? byteOffset, object? value, object? littleEndian)
        {
            EnsureMutableBuffer();
            var index = CoerceIndex(byteOffset, 0, "Offset is outside the bounds of the DataView");
            WriteSingle(index, (float)TypeUtilities.ToNumber(value), UseLittleEndian(littleEndian));
            return null;
        }

        public object? setFloat64(object? byteOffset, object? value)
            => setFloat64(byteOffset, value, null);

        public object? setFloat64(object? byteOffset, object? value, object? littleEndian)
        {
            EnsureMutableBuffer();
            var index = CoerceIndex(byteOffset, 0, "Offset is outside the bounds of the DataView");
            WriteDouble(index, TypeUtilities.ToNumber(value), UseLittleEndian(littleEndian));
            return null;
        }

        public object? setBigInt64(object? byteOffset, object? value)
            => setBigInt64(byteOffset, value, null);

        public object? setBigInt64(object? byteOffset, object? value, object? littleEndian)
        {
            EnsureMutableBuffer();
            var index = CoerceIndex(byteOffset, 0, "Offset is outside the bounds of the DataView");
            WriteBigInt64(index, BigInt.ToBigIntForTypedArray(value), UseLittleEndian(littleEndian));
            return null;
        }

        public object? setBigUint64(object? byteOffset, object? value)
            => setBigUint64(byteOffset, value, null);

        public object? setBigUint64(object? byteOffset, object? value, object? littleEndian)
        {
            EnsureMutableBuffer();
            var index = CoerceIndex(byteOffset, 0, "Offset is outside the bounds of the DataView");
            WriteBigInt64(index, BigInt.ToBigIntForTypedArray(value), UseLittleEndian(littleEndian));
            return null;
        }

        private void EnsureMutableBuffer()
        {
            if (_buffer.IsImmutable)
            {
                throw new TypeError("Cannot write to an immutable ArrayBuffer");
            }
        }

        private byte ReadByte(object? requestedOffset)
        {
            var index = GetAbsoluteIndex(requestedOffset, 1);
            return _buffer.RawBytes[index];
        }

        private void WriteByte(long requestedOffset, byte value)
        {
            var index = GetAbsoluteIndex(requestedOffset, 1);
            _buffer.RawBytes[index] = value;
        }

        private void WriteUInt16(long requestedOffset, ushort value, bool littleEndian)
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

        private void WriteUInt32(long requestedOffset, uint value, bool littleEndian)
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

        private void WriteSingle(long requestedOffset, float value, bool littleEndian)
        {
            var index = GetAbsoluteIndex(requestedOffset, 4);
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian != littleEndian)
            {
                System.Array.Reverse(bytes);
            }

            System.Buffer.BlockCopy(bytes, 0, _buffer.RawBytes, index, 4);
        }

        private void WriteDouble(long requestedOffset, double value, bool littleEndian)
        {
            var index = GetAbsoluteIndex(requestedOffset, 8);
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian != littleEndian)
            {
                System.Array.Reverse(bytes);
            }

            System.Buffer.BlockCopy(bytes, 0, _buffer.RawBytes, index, 8);
        }

        private void WriteBigInt64(long requestedOffset, BigInteger value, bool littleEndian)
        {
            var wrapped = value % BigInt64Modulus;
            if (wrapped < 0)
            {
                wrapped += BigInt64Modulus;
            }

            var index = GetAbsoluteIndex(requestedOffset, 8);
            var bytes = _buffer.RawBytes.AsSpan(index, 8);
            if (littleEndian)
            {
                BinaryPrimitives.WriteUInt64LittleEndian(bytes, (ulong)wrapped);
            }
            else
            {
                BinaryPrimitives.WriteUInt64BigEndian(bytes, (ulong)wrapped);
            }
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
            return GetAbsoluteIndex(relativeIndex, elementSize);
        }

        private int GetAbsoluteIndex(long relativeIndex, int elementSize)
        {
            _buffer.EnsureAttached();
            if (IsOutOfBounds)
            {
                throw new TypeError("DataView is out of bounds");
            }

            long elementSizeLong = elementSize;
            long byteLengthLong = CurrentByteLength;
            if (relativeIndex + elementSizeLong > byteLengthLong)
            {
                throw new RangeError("Offset is outside the bounds of the DataView");
            }

            return checked(_byteOffset + (int)relativeIndex);
        }

        private bool IsOutOfBounds
            => _buffer.IsDetached
                || (_isLengthTracking
                ? _byteOffset > _buffer.ByteLengthInt
                : (long)_byteOffset + _byteLength > _buffer.ByteLengthInt);

        private int CurrentByteLength
            => _isLengthTracking
                ? _buffer.ByteLengthInt - _byteOffset
                : _byteLength;

        private static bool UseLittleEndian(object? value)
            => value is not null && value is not JsNull && TypeUtilities.ToBoolean(value);

        private static long CoerceIndex(object? value, long defaultValue, string errorMessage)
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

            var truncated = System.Math.Truncate(number);
            if (double.IsInfinity(number) || truncated < 0 || truncated > 9007199254740991d)
            {
                throw new RangeError(errorMessage);
            }

            return (long)truncated;
        }
    }
}
