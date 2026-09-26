using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Int32Array")]
    public class Int32Array : TypedArrayBase
    {
        private const int ElementSize = 4;
        private readonly bool _hasFixedContiguousBacking;

        public Int32Array()
        {
            InitializeEmpty();
            _hasFixedContiguousBacking = HasFixedContiguousBacking();
        }

        public Int32Array(object? arg)
        {
            InitializeFromArgument(arg);
            _hasFixedContiguousBacking = HasFixedContiguousBacking();
        }

        public Int32Array(object? arg, object? byteOffset)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, null);
                _hasFixedContiguousBacking = HasFixedContiguousBacking();
                return;
            }

            InitializeFromArgument(arg);
            _hasFixedContiguousBacking = HasFixedContiguousBacking();
        }

        public Int32Array(object? arg, object? byteOffset, object? length)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, length);
                _hasFixedContiguousBacking = HasFixedContiguousBacking();
                return;
            }

            InitializeFromArgument(arg);
            _hasFixedContiguousBacking = HasFixedContiguousBacking();
        }

        private Int32Array(ArrayBuffer buffer, int byteOffset, int length)
        {
            InitializeFromExisting(buffer, byteOffset, length);
            _hasFixedContiguousBacking = HasFixedContiguousBacking();
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var candidate = (int)index;
                if (candidate == index
                    && (uint)candidate < (uint)FixedLengthElements
                    && HasContiguousBacking)
                {
                    return MemoryMarshal.Cast<byte, int>(BufferObject.RawBytes)[
                        (ByteOffsetBytes / ElementSize) + candidate];
                }

                return base[index];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                var candidate = (int)index;
                if (candidate == index
                    && (uint)candidate < (uint)FixedLengthElements
                    && HasContiguousBacking)
                {
                    var elements = MemoryMarshal.Cast<byte, int>(BufferObject.RawBytes.AsSpan());
                    elements[(ByteOffsetBytes / ElementSize) + candidate] = ToInt32(value);
                    return;
                }

                base[index] = value;
            }
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
            if (TryGetContiguousElements(out var elements))
            {
                return elements[index];
            }

            var offset = ByteOffsetBytes + (index * ElementSize);
            return ReadInt32(BufferObject.RawBytes, offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void WriteElementValue(int index, double value)
        {
            if (TryGetContiguousElements(out var elements))
            {
                elements[index] = ToInt32(value);
                return;
            }

            var offset = ByteOffsetBytes + (index * ElementSize);
            WriteInt32(BufferObject.RawBytes, offset, ToInt32(value));
        }

        // The returned span is only valid until the next operation that can change the buffer.
        // Callers must not retain it across JavaScript execution or a buffer transfer.
        internal bool TryGetContiguousElements(out Span<int> elements)
        {
            if (!HasContiguousBacking)
            {
                elements = default;
                return false;
            }

            elements = MemoryMarshal.Cast<byte, int>(
                BufferObject.RawBytes.AsSpan(ByteOffsetBytes, LengthElements * ElementSize));
            return true;
        }

        internal static double FindFirstZeroBitOrNegative(Int32Array? words, double index)
        {
            if (words is null || words.GetType() != typeof(Int32Array)
                || words.BufferObject is SharedArrayBuffer
                || index < 0 || index > uint.MaxValue || index != System.Math.Truncate(index)
                || (index == 0 && double.IsNegative(index))
                || !words.TryGetContiguousElements(out var elements))
            {
                return -1;
            }

            var end = (double)elements.Length * 32;
            if (end > uint.MaxValue || index >= end)
            {
                return end <= uint.MaxValue && index >= end ? index : -1;
            }

            var word = (int)((uint)index >> 5);
            var available = ~(uint)elements[word] & (uint.MaxValue << ((int)index & 31));
            while (true)
            {
                if (available != 0)
                {
                    return (double)word * 32 + BitOperations.TrailingZeroCount(available);
                }

                if (++word == elements.Length)
                {
                    return end;
                }
                available = ~(uint)elements[word];
            }
        }

        private bool HasContiguousBacking => _hasFixedContiguousBacking && !BufferObject.IsDetached;

        private bool HasFixedContiguousBacking()
            => !BufferObject.IsResizable
                && BufferObject is not SharedArrayBuffer { growable: true }
                && (ByteOffsetBytes % ElementSize) == 0;

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
