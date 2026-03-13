using System;

namespace JavaScriptRuntime
{
    [IntrinsicObject("ArrayBuffer")]
    public sealed class ArrayBuffer
    {
        private readonly byte[] _bytes;

        public ArrayBuffer()
        {
            _bytes = System.Array.Empty<byte>();
        }

        public ArrayBuffer(object? length)
        {
            var byteLength = CoerceByteLength(length);
            _bytes = byteLength == 0
                ? System.Array.Empty<byte>()
                : new byte[byteLength];
        }

        internal ArrayBuffer(byte[] bytes, bool cloneBuffer)
        {
            _bytes = cloneBuffer ? (byte[])bytes.Clone() : bytes;
        }

        public double byteLength => _bytes.Length;

        public ArrayBuffer slice(object? start)
            => slice(start, null);

        public ArrayBuffer slice(object? start, object? end)
        {
            var startIndex = CoerceRelativeIndex(start, 0, _bytes.Length);
            var endIndex = CoerceRelativeIndex(end, _bytes.Length, _bytes.Length);
            if (endIndex < startIndex)
            {
                endIndex = startIndex;
            }

            var length = endIndex - startIndex;
            if (length <= 0)
            {
                return new ArrayBuffer(System.Array.Empty<byte>(), cloneBuffer: false);
            }

            var copy = new byte[length];
            System.Buffer.BlockCopy(_bytes, startIndex, copy, 0, length);
            return new ArrayBuffer(copy, cloneBuffer: false);
        }

        public static bool isView(object? arg)
            => arg is DataView or TypedArrayBase;

        internal int ByteLengthInt => _bytes.Length;

        internal byte[] RawBytes => _bytes;

        private static int CoerceByteLength(object? value)
        {
            if (value is null || value is JsNull)
            {
                return 0;
            }

            var number = TypeUtilities.ToNumber(value);
            if (double.IsNaN(number) || number == 0.0)
            {
                return 0;
            }

            if (double.IsInfinity(number) || number < 0)
            {
                throw new RangeError("Invalid ArrayBuffer length");
            }

            var truncated = System.Math.Truncate(number);
            if (truncated > int.MaxValue)
            {
                throw new RangeError("Invalid ArrayBuffer length");
            }

            return (int)truncated;
        }

        private static int CoerceRelativeIndex(object? value, int defaultValue, int length)
        {
            if (value is null || value is JsNull)
            {
                return defaultValue;
            }

            var number = TypeUtilities.ToNumber(value);
            if (double.IsNaN(number))
            {
                return 0;
            }

            if (double.IsNegativeInfinity(number))
            {
                return 0;
            }

            if (double.IsPositiveInfinity(number))
            {
                return length;
            }

            var truncated = System.Math.Truncate(number);
            if (truncated < 0)
            {
                truncated = System.Math.Max(length + truncated, 0);
            }

            if (truncated > length)
            {
                truncated = length;
            }

            return (int)truncated;
        }
    }
}
