using System;
using System.Buffers.Binary;
using System.Numerics;

namespace JavaScriptRuntime
{
    [IntrinsicObject("BigInt64Array")]
    public sealed class BigInt64Array : TypedArrayBase
    {
        private const int ElementSize = 8;
        private static readonly BigInteger Modulus = BigInteger.One << 64;

        internal static JsObject Prototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.BigInt64ArrayPrototype,
                static () => new JsObject());

        public BigInt64Array()
        {
            InitializeEmpty();
        }

        public BigInt64Array(object? arg)
        {
            InitializeFromArgument(arg);
        }

        public BigInt64Array(object? arg, object? byteOffset)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, null);
                return;
            }

            InitializeFromArgument(arg);
        }

        public BigInt64Array(object? arg, object? byteOffset, object? length)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, length);
                return;
            }

            InitializeFromArgument(arg);
        }

        private BigInt64Array(ArrayBuffer buffer, int byteOffset, int length)
        {
            InitializeFromExisting(buffer, byteOffset, length);
        }

        protected override int BytesPerElement => ElementSize;

        protected override string TypedArrayName => nameof(BigInt64Array);

        protected override double ReadElementValue(int index)
            => (double)(BigInteger)ReadElementObject(index)!;

        protected override void WriteElementValue(int index, double value)
            => throw new TypeError("Cannot convert a Number value to a BigInt");

        protected internal override object ReadElementObject(int index)
        {
            var offset = ByteOffsetBytes + (index * ElementSize);
            var span = BufferObject.RawBytes.AsSpan(offset, ElementSize);
            var value = BitConverter.IsLittleEndian
                ? BinaryPrimitives.ReadInt64LittleEndian(span)
                : BinaryPrimitives.ReadInt64BigEndian(span);
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
            => new BigInt64Array(buffer, byteOffset, length);
    }
}
