using System;
using System.Buffers.Binary;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Int16Array")]
    public sealed class Int16Array : TypedArrayBase
    {
        private const int ElementSize = 2;

        public Int16Array()
        {
            InitializeEmpty();
        }

        public Int16Array(object? arg)
        {
            InitializeFromArgument(arg);
        }

        public Int16Array(object? arg, object? byteOffset)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, null);
                return;
            }

            InitializeFromArgument(arg);
        }

        public Int16Array(object? arg, object? byteOffset, object? length)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, length);
                return;
            }

            InitializeFromArgument(arg);
        }

        private Int16Array(ArrayBuffer buffer, int byteOffset, int length)
        {
            InitializeFromExisting(buffer, byteOffset, length);
        }

        public static Int16Array from(object? source)
            => FromSource(nameof(Int16Array), source, null, null, static values => new Int16Array(values));

        public static Int16Array from(object? source, object? mapper)
            => FromSource(nameof(Int16Array), source, mapper, null, static values => new Int16Array(values));

        public static Int16Array from(object? source, object? mapper, object? thisArg)
            => FromSource(nameof(Int16Array), source, mapper, thisArg, static values => new Int16Array(values));

        public static Int16Array of(object[]? args)
            => new Int16Array(args ?? global::System.Array.Empty<object?>());

        protected override int BytesPerElement => ElementSize;

        protected override string TypedArrayName => nameof(Int16Array);

        public Int16Array slice()
            => (Int16Array)SliceCore(null, null);

        public Int16Array slice(object? start)
            => (Int16Array)SliceCore(start, null);

        public Int16Array slice(object? start, object? end)
            => (Int16Array)SliceCore(start, end);

        public Int16Array subarray()
            => (Int16Array)SubarrayCore(null, null);

        public Int16Array subarray(object? start)
            => (Int16Array)SubarrayCore(start, null);

        public Int16Array subarray(object? start, object? end)
            => (Int16Array)SubarrayCore(start, end);

        protected override double ReadElementValue(int index)
        {
            var offset = ByteOffsetBytes + (index * ElementSize);
            var span = BufferObject.RawBytes.AsSpan(offset, ElementSize);
            return BitConverter.IsLittleEndian
                ? BinaryPrimitives.ReadInt16LittleEndian(span)
                : BinaryPrimitives.ReadInt16BigEndian(span);
        }

        protected override void WriteElementValue(int index, double value)
        {
            var offset = ByteOffsetBytes + (index * ElementSize);
            var span = BufferObject.RawBytes.AsSpan(offset, ElementSize);
            var coerced = TypeUtilities.ToInt16(value);
            if (BitConverter.IsLittleEndian)
            {
                BinaryPrimitives.WriteInt16LittleEndian(span, coerced);
            }
            else
            {
                BinaryPrimitives.WriteInt16BigEndian(span, coerced);
            }
        }

        protected override TypedArrayBase CreateSameType(ArrayBuffer buffer, int byteOffset, int length)
            => new Int16Array(buffer, byteOffset, length);
    }
}
