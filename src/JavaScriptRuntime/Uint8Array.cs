using System;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Uint8Array")]
    public sealed class Uint8Array : TypedArrayBase
    {
        private const int ElementSize = 1;

        public Uint8Array()
        {
            InitializeEmpty();
        }

        public Uint8Array(object? arg)
        {
            InitializeFromArgument(arg);
        }

        public Uint8Array(object? arg, object? byteOffset)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, null);
                return;
            }

            InitializeFromArgument(arg);
        }

        public Uint8Array(object? arg, object? byteOffset, object? length)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, length);
                return;
            }

            InitializeFromArgument(arg);
        }

        private Uint8Array(ArrayBuffer buffer, int byteOffset, int length)
        {
            InitializeFromExisting(buffer, byteOffset, length);
        }

        public static Uint8Array from(object? source)
            => FromSource(nameof(Uint8Array), source, null, null, static values => new Uint8Array(values));

        public static Uint8Array from(object? source, object? mapper)
            => FromSource(nameof(Uint8Array), source, mapper, null, static values => new Uint8Array(values));

        public static Uint8Array from(object? source, object? mapper, object? thisArg)
            => FromSource(nameof(Uint8Array), source, mapper, thisArg, static values => new Uint8Array(values));

        public static Uint8Array of(object[]? args)
            => new Uint8Array(args ?? global::System.Array.Empty<object?>());

        protected override int BytesPerElement => ElementSize;

        protected override string TypedArrayName => nameof(Uint8Array);

        public Uint8Array slice()
            => (Uint8Array)SliceCore(null, null);

        public Uint8Array slice(object? start)
            => (Uint8Array)SliceCore(start, null);

        public Uint8Array slice(object? start, object? end)
            => (Uint8Array)SliceCore(start, end);

        public Uint8Array subarray()
            => (Uint8Array)SubarrayCore(null, null);

        public Uint8Array subarray(object? start)
            => (Uint8Array)SubarrayCore(start, null);

        public Uint8Array subarray(object? start, object? end)
            => (Uint8Array)SubarrayCore(start, end);

        protected override double ReadElementValue(int index)
            => BufferObject.RawBytes[ByteOffsetBytes + index];

        protected override void WriteElementValue(int index, double value)
            => BufferObject.RawBytes[ByteOffsetBytes + index] = ToUint8(value);

        protected override TypedArrayBase CreateSameType(ArrayBuffer buffer, int byteOffset, int length)
            => new Uint8Array(buffer, byteOffset, length);

        private static byte ToUint8(double value)
            => unchecked((byte)TypeUtilities.ToInt32(value));
    }
}
