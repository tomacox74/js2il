using System;
using System.Buffers.Binary;
using System.Numerics;

namespace JavaScriptRuntime
{
    [IntrinsicObject("BigUint64Array")]
    public sealed class BigUint64Array : TypedArrayBase
    {
        private const int ElementSize = 8;
        private static readonly BigInteger Modulus = BigInteger.One << 64;

        internal static JsObject Prototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.BigUint64ArrayPrototype,
                static () => new JsObject());

        public BigUint64Array()
        {
            InitializeEmpty();
        }

        public BigUint64Array(object? arg)
        {
            InitializeFromArgument(arg);
        }

        public BigUint64Array(object? arg, object? byteOffset)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, null);
                return;
            }

            InitializeFromArgument(arg);
        }

        public BigUint64Array(object? arg, object? byteOffset, object? length)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, length);
                return;
            }

            InitializeFromArgument(arg);
        }

        private BigUint64Array(ArrayBuffer buffer, int byteOffset, int length)
        {
            InitializeFromExisting(buffer, byteOffset, length);
        }

        protected override int BytesPerElement => ElementSize;

        protected override string TypedArrayName => nameof(BigUint64Array);

        protected override double ReadElementValue(int index)
            => (double)(BigInteger)ReadElementObject(index)!;

        protected override void WriteElementValue(int index, double value)
            => throw new TypeError("Cannot convert a Number value to a BigInt");

        protected internal override object ReadElementObject(int index)
        {
            var offset = ByteOffsetBytes + (index * ElementSize);
            var span = BufferObject.RawBytes.AsSpan(offset, ElementSize);
            var value = BitConverter.IsLittleEndian
                ? BinaryPrimitives.ReadUInt64LittleEndian(span)
                : BinaryPrimitives.ReadUInt64BigEndian(span);
            return new BigInteger(value);
        }

        protected internal override object CoerceElementValue(object? value)
            => BigInt.ToBigIntForTypedArray(value);

        protected internal override void WriteElementObject(int index, object? value)
        {
            var bigInt = (BigInteger)CoerceElementValue(value);
            var wrapped = bigInt % Modulus;
            if (wrapped < 0)
            {
                wrapped += Modulus;
            }

            var bits = (ulong)wrapped;
            var offset = ByteOffsetBytes + (index * ElementSize);
            var span = BufferObject.RawBytes.AsSpan(offset, ElementSize);
            if (BitConverter.IsLittleEndian)
            {
                BinaryPrimitives.WriteUInt64LittleEndian(span, bits);
            }
            else
            {
                BinaryPrimitives.WriteUInt64BigEndian(span, bits);
            }
        }

        protected override TypedArrayBase CreateSameType(ArrayBuffer buffer, int byteOffset, int length)
            => new BigUint64Array(buffer, byteOffset, length);
    }
}
