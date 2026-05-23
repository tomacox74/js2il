using System;
using System.Buffers.Binary;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Float32Array")]
    public sealed class Float32Array : TypedArrayBase
    {
        private const int ElementSize = 4;

        public Float32Array()
        {
            InitializeEmpty();
        }

        public Float32Array(object? arg)
        {
            InitializeFromArgument(arg);
        }

        public Float32Array(object? arg, object? byteOffset)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, null);
                return;
            }

            InitializeFromArgument(arg);
        }

        public Float32Array(object? arg, object? byteOffset, object? length)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, length);
                return;
            }

            InitializeFromArgument(arg);
        }

        private Float32Array(ArrayBuffer buffer, int byteOffset, int length)
        {
            InitializeFromExisting(buffer, byteOffset, length);
        }

        public static Float32Array from(object? source)
            => FromSource(nameof(Float32Array), source, null, null, static values => new Float32Array(values));

        public static Float32Array from(object? source, object? mapper)
            => FromSource(nameof(Float32Array), source, mapper, null, static values => new Float32Array(values));

        public static Float32Array from(object? source, object? mapper, object? thisArg)
            => FromSource(nameof(Float32Array), source, mapper, thisArg, static values => new Float32Array(values));

        public static Float32Array of(object[]? args)
            => new Float32Array(args ?? global::System.Array.Empty<object?>());

        protected override int BytesPerElement => ElementSize;

        protected override string TypedArrayName => nameof(Float32Array);

        public Float32Array slice()
            => (Float32Array)SliceCore(null, null);

        public Float32Array slice(object? start)
            => (Float32Array)SliceCore(start, null);

        public Float32Array slice(object? start, object? end)
            => (Float32Array)SliceCore(start, end);

        public Float32Array subarray()
            => (Float32Array)SubarrayCore(null, null);

        public Float32Array subarray(object? start)
            => (Float32Array)SubarrayCore(start, null);

        public Float32Array subarray(object? start, object? end)
            => (Float32Array)SubarrayCore(start, end);

        protected override double ReadElementValue(int index)
        {
            var offset = ByteOffsetBytes + (index * ElementSize);
            var span = BufferObject.RawBytes.AsSpan(offset, ElementSize);
            var bits = BitConverter.IsLittleEndian
                ? BinaryPrimitives.ReadInt32LittleEndian(span)
                : BinaryPrimitives.ReadInt32BigEndian(span);
            return BitConverter.Int32BitsToSingle(bits);
        }

        protected override void WriteElementValue(int index, double value)
        {
            var offset = ByteOffsetBytes + (index * ElementSize);
            var span = BufferObject.RawBytes.AsSpan(offset, ElementSize);
            var bits = BitConverter.SingleToInt32Bits((float)value);
            if (BitConverter.IsLittleEndian)
            {
                BinaryPrimitives.WriteInt32LittleEndian(span, bits);
            }
            else
            {
                BinaryPrimitives.WriteInt32BigEndian(span, bits);
            }
        }

        protected override TypedArrayBase CreateSameType(ArrayBuffer buffer, int byteOffset, int length)
            => new Float32Array(buffer, byteOffset, length);
    }
}
