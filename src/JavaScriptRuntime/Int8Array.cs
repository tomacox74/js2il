using System;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Int8Array")]
    public sealed class Int8Array : TypedArrayBase
    {
        private const int ElementSize = 1;

        public Int8Array()
        {
            InitializeEmpty();
        }

        public Int8Array(object? arg)
        {
            InitializeFromArgument(arg);
        }

        public Int8Array(object? arg, object? byteOffset)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, null);
                return;
            }

            InitializeFromArgument(arg);
        }

        public Int8Array(object? arg, object? byteOffset, object? length)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, length);
                return;
            }

            InitializeFromArgument(arg);
        }

        private Int8Array(ArrayBuffer buffer, int byteOffset, int length)
        {
            InitializeFromExisting(buffer, byteOffset, length);
        }

        public static Int8Array from(object? source)
            => FromSource(nameof(Int8Array), source, null, null, static values => new Int8Array(values));

        public static Int8Array from(object? source, object? mapper)
            => FromSource(nameof(Int8Array), source, mapper, null, static values => new Int8Array(values));

        public static Int8Array from(object? source, object? mapper, object? thisArg)
            => FromSource(nameof(Int8Array), source, mapper, thisArg, static values => new Int8Array(values));

        public static Int8Array of(object[]? args)
            => new Int8Array(args ?? global::System.Array.Empty<object?>());

        protected override int BytesPerElement => ElementSize;

        protected override string TypedArrayName => nameof(Int8Array);

        public Int8Array slice()
            => (Int8Array)SliceCore(null, null);

        public Int8Array slice(object? start)
            => (Int8Array)SliceCore(start, null);

        public Int8Array slice(object? start, object? end)
            => (Int8Array)SliceCore(start, end);

        public Int8Array subarray()
            => (Int8Array)SubarrayCore(null, null);

        public Int8Array subarray(object? start)
            => (Int8Array)SubarrayCore(start, null);

        public Int8Array subarray(object? start, object? end)
            => (Int8Array)SubarrayCore(start, end);

        protected override double ReadElementValue(int index)
            => unchecked((sbyte)BufferObject.RawBytes[ByteOffsetBytes + index]);

        protected override void WriteElementValue(int index, double value)
            => BufferObject.RawBytes[ByteOffsetBytes + index] = unchecked((byte)TypeUtilities.ToInt8(value));

        protected override TypedArrayBase CreateSameType(ArrayBuffer buffer, int byteOffset, int length)
            => new Int8Array(buffer, byteOffset, length);
    }
}
