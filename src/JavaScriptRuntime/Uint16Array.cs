using System;
using System.Buffers.Binary;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Uint16Array")]
    public sealed class Uint16Array : TypedArrayBase
    {
        private const int ElementSize = 2;

        public Uint16Array()
        {
            InitializeEmpty();
        }

        public Uint16Array(object? arg)
        {
            InitializeFromArgument(arg);
        }

        public Uint16Array(object? arg, object? byteOffset)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, null);
                return;
            }

            InitializeFromArgument(arg);
        }

        public Uint16Array(object? arg, object? byteOffset, object? length)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, length);
                return;
            }

            InitializeFromArgument(arg);
        }

        private Uint16Array(ArrayBuffer buffer, int byteOffset, int length)
        {
            InitializeFromExisting(buffer, byteOffset, length);
        }

        public static Uint16Array from(object? source)
            => FromSource(nameof(Uint16Array), source, null, null, static values => new Uint16Array(values));

        public static Uint16Array from(object? source, object? mapper)
            => FromSource(nameof(Uint16Array), source, mapper, null, static values => new Uint16Array(values));

        public static Uint16Array from(object? source, object? mapper, object? thisArg)
            => FromSource(nameof(Uint16Array), source, mapper, thisArg, static values => new Uint16Array(values));

        public static Uint16Array of(object[]? args)
            => new Uint16Array(args ?? global::System.Array.Empty<object?>());

        protected override int BytesPerElement => ElementSize;

        protected override string TypedArrayName => nameof(Uint16Array);

        protected override double ReadElementValue(int index)
        {
            var offset = ByteOffsetBytes + (index * ElementSize);
            var span = BufferObject.RawBytes.AsSpan(offset, ElementSize);
            return BitConverter.IsLittleEndian
                ? BinaryPrimitives.ReadUInt16LittleEndian(span)
                : BinaryPrimitives.ReadUInt16BigEndian(span);
        }

        protected override void WriteElementValue(int index, double value)
        {
            var offset = ByteOffsetBytes + (index * ElementSize);
            var span = BufferObject.RawBytes.AsSpan(offset, ElementSize);
            var coerced = TypeUtilities.ToUint16(value);
            if (BitConverter.IsLittleEndian)
            {
                BinaryPrimitives.WriteUInt16LittleEndian(span, coerced);
            }
            else
            {
                BinaryPrimitives.WriteUInt16BigEndian(span, coerced);
            }
        }

        protected override TypedArrayBase CreateSameType(ArrayBuffer buffer, int byteOffset, int length)
            => new Uint16Array(buffer, byteOffset, length);
    }
}
