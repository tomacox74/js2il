using System;
using System.Buffers.Binary;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Uint32Array")]
    public sealed class Uint32Array : TypedArrayBase
    {
        private const int ElementSize = 4;

        public Uint32Array()
        {
            InitializeEmpty();
        }

        public Uint32Array(object? arg)
        {
            InitializeFromArgument(arg);
        }

        public Uint32Array(object? arg, object? byteOffset)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, null);
                return;
            }

            InitializeFromArgument(arg);
        }

        public Uint32Array(object? arg, object? byteOffset, object? length)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, length);
                return;
            }

            InitializeFromArgument(arg);
        }

        private Uint32Array(ArrayBuffer buffer, int byteOffset, int length)
        {
            InitializeFromExisting(buffer, byteOffset, length);
        }

        public static Uint32Array from(object? source)
            => FromSource(nameof(Uint32Array), source, null, null, static values => new Uint32Array(values));

        public static Uint32Array from(object? source, object? mapper)
            => FromSource(nameof(Uint32Array), source, mapper, null, static values => new Uint32Array(values));

        public static Uint32Array from(object? source, object? mapper, object? thisArg)
            => FromSource(nameof(Uint32Array), source, mapper, thisArg, static values => new Uint32Array(values));

        public static Uint32Array of(object[]? args)
            => new Uint32Array(args ?? global::System.Array.Empty<object?>());

        protected override int BytesPerElement => ElementSize;

        protected override string TypedArrayName => nameof(Uint32Array);

        protected override double ReadElementValue(int index)
        {
            var offset = ByteOffsetBytes + (index * ElementSize);
            var span = BufferObject.RawBytes.AsSpan(offset, ElementSize);
            return BitConverter.IsLittleEndian
                ? BinaryPrimitives.ReadUInt32LittleEndian(span)
                : BinaryPrimitives.ReadUInt32BigEndian(span);
        }

        protected override void WriteElementValue(int index, double value)
        {
            var offset = ByteOffsetBytes + (index * ElementSize);
            var span = BufferObject.RawBytes.AsSpan(offset, ElementSize);
            var coerced = TypeUtilities.ToUint32(value);
            if (BitConverter.IsLittleEndian)
            {
                BinaryPrimitives.WriteUInt32LittleEndian(span, coerced);
            }
            else
            {
                BinaryPrimitives.WriteUInt32BigEndian(span, coerced);
            }
        }

        protected override TypedArrayBase CreateSameType(ArrayBuffer buffer, int byteOffset, int length)
            => new Uint32Array(buffer, byteOffset, length);
    }
}
