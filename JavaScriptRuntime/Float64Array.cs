using System;
using System.Buffers.Binary;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Float64Array")]
    public sealed class Float64Array : TypedArrayBase
    {
        private const int ElementSize = 8;

        public Float64Array()
        {
            InitializeEmpty();
        }

        public Float64Array(object? arg)
        {
            InitializeFromArgument(arg);
        }

        public Float64Array(object? arg, object? byteOffset)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, null);
                return;
            }

            InitializeFromArgument(arg);
        }

        public Float64Array(object? arg, object? byteOffset, object? length)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, length);
                return;
            }

            InitializeFromArgument(arg);
        }

        private Float64Array(ArrayBuffer buffer, int byteOffset, int length)
        {
            InitializeFromExisting(buffer, byteOffset, length);
        }

        public static Float64Array from(object? source)
            => FromSource(nameof(Float64Array), source, null, null, static values => new Float64Array(values));

        public static Float64Array from(object? source, object? mapper)
            => FromSource(nameof(Float64Array), source, mapper, null, static values => new Float64Array(values));

        public static Float64Array from(object? source, object? mapper, object? thisArg)
            => FromSource(nameof(Float64Array), source, mapper, thisArg, static values => new Float64Array(values));

        public static Float64Array of(object[]? args)
            => new Float64Array(args ?? global::System.Array.Empty<object?>());

        protected override int BytesPerElement => ElementSize;

        protected override string TypedArrayName => nameof(Float64Array);

        public Float64Array slice()
            => (Float64Array)SliceCore(null, null);

        public Float64Array slice(object? start)
            => (Float64Array)SliceCore(start, null);

        public Float64Array slice(object? start, object? end)
            => (Float64Array)SliceCore(start, end);

        public Float64Array subarray()
            => (Float64Array)SubarrayCore(null, null);

        public Float64Array subarray(object? start)
            => (Float64Array)SubarrayCore(start, null);

        public Float64Array subarray(object? start, object? end)
            => (Float64Array)SubarrayCore(start, end);

        protected override double ReadElementValue(int index)
        {
            var offset = ByteOffsetBytes + (index * ElementSize);
            return ReadDouble(BufferObject.RawBytes, offset);
        }

        protected override void WriteElementValue(int index, double value)
        {
            var offset = ByteOffsetBytes + (index * ElementSize);
            WriteDouble(BufferObject.RawBytes, offset, value);
        }

        protected override TypedArrayBase CreateSameType(ArrayBuffer buffer, int byteOffset, int length)
            => new Float64Array(buffer, byteOffset, length);

        private static double ReadDouble(byte[] buffer, int offset)
        {
            var span = buffer.AsSpan(offset, ElementSize);
            var bits = BitConverter.IsLittleEndian
                ? BinaryPrimitives.ReadInt64LittleEndian(span)
                : BinaryPrimitives.ReadInt64BigEndian(span);
            return BitConverter.Int64BitsToDouble(bits);
        }

        private static void WriteDouble(byte[] buffer, int offset, double value)
        {
            var span = buffer.AsSpan(offset, ElementSize);
            var bits = BitConverter.DoubleToInt64Bits(value);
            if (BitConverter.IsLittleEndian)
            {
                BinaryPrimitives.WriteInt64LittleEndian(span, bits);
            }
            else
            {
                BinaryPrimitives.WriteInt64BigEndian(span, bits);
            }
        }
    }
}
