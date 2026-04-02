using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Int32Array")]
    public class Int32Array : TypedArrayBase
    {
        private const int ElementSize = 4;

        public Int32Array()
        {
            InitializeEmpty();
        }

        public Int32Array(object? arg)
        {
            InitializeFromArgument(arg);
        }

        public Int32Array(object? arg, object? byteOffset)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, null);
                return;
            }

            InitializeFromArgument(arg);
        }

        public Int32Array(object? arg, object? byteOffset, object? length)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, length);
                return;
            }

            InitializeFromArgument(arg);
        }

        private Int32Array(ArrayBuffer buffer, int byteOffset, int length)
        {
            InitializeFromExisting(buffer, byteOffset, length);
        }

        public static Int32Array from(object? source)
            => FromSource(nameof(Int32Array), source, null, null, static values => new Int32Array(values));

        public static Int32Array from(object? source, object? mapper)
            => FromSource(nameof(Int32Array), source, mapper, null, static values => new Int32Array(values));

        public static Int32Array from(object? source, object? mapper, object? thisArg)
            => FromSource(nameof(Int32Array), source, mapper, thisArg, static values => new Int32Array(values));

        public static Int32Array of(object[]? args)
            => new Int32Array(args ?? global::System.Array.Empty<object?>());

        protected override int BytesPerElement => ElementSize;

        protected override string TypedArrayName => nameof(Int32Array);

        public new ArrayBuffer buffer => base.buffer;

        public new double byteOffset => base.byteOffset;

        public new double byteLength => base.byteLength;

        public new double length => base.length;

        internal new void SetFromDouble(int index, double value)
            => base.SetFromDouble(index, value);

        public new double this[double index]
        {
            get => base[index];
            set => base[index] = value;
        }

        public Int32Array slice()
            => (Int32Array)SliceCore(null, null);

        public Int32Array slice(object? start)
            => (Int32Array)SliceCore(start, null);

        public Int32Array slice(object? start, object? end)
            => (Int32Array)SliceCore(start, end);

        public Int32Array subarray()
            => (Int32Array)SubarrayCore(null, null);

        public Int32Array subarray(object? start)
            => (Int32Array)SubarrayCore(start, null);

        public Int32Array subarray(object? start, object? end)
            => (Int32Array)SubarrayCore(start, end);

        protected override double ReadElementValue(int index)
        {
            var offset = ByteOffsetBytes + (index * ElementSize);
            return ReadInt32(BufferObject.RawBytes, offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void WriteElementValue(int index, double value)
        {
            var offset = ByteOffsetBytes + (index * ElementSize);
            WriteInt32(BufferObject.RawBytes, offset, ToInt32(value));
        }

        protected override TypedArrayBase CreateSameType(ArrayBuffer buffer, int byteOffset, int length)
            => new Int32Array(buffer, byteOffset, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ToInt32(double value)
        {
            var candidate = (int)value;
            if (candidate == value)
            {
                return candidate;
            }

            if (double.IsNaN(value) || double.IsInfinity(value) || value == 0.0)
            {
                return 0;
            }

            var truncated = global::System.Math.Truncate(value);
            var modulo = truncated % 4294967296.0;
            if (modulo < 0)
            {
                modulo += 4294967296.0;
            }

            if (modulo >= 2147483648.0)
            {
                return (int)(modulo - 4294967296.0);
            }

            return (int)modulo;
        }

        private static int ReadInt32(byte[] buffer, int offset)
        {
            var span = buffer.AsSpan(offset, ElementSize);
            return BitConverter.IsLittleEndian
                ? BinaryPrimitives.ReadInt32LittleEndian(span)
                : BinaryPrimitives.ReadInt32BigEndian(span);
        }

        private static void WriteInt32(byte[] buffer, int offset, int value)
        {
            var span = buffer.AsSpan(offset, ElementSize);
            if (BitConverter.IsLittleEndian)
            {
                BinaryPrimitives.WriteInt32LittleEndian(span, value);
            }
            else
            {
                BinaryPrimitives.WriteInt32BigEndian(span, value);
            }
        }
    }
}
