using System;

namespace JavaScriptRuntime
{
    [IntrinsicObject("ArrayBuffer")]
    public class ArrayBuffer
    {
        internal static object Prototype
            => RuntimeIntrinsics.Current.ArrayBufferPrototype;
        private readonly RuntimeArrayBufferStorage _storage;
        private readonly int _maxByteLength;
        private readonly bool _isResizable;

        public ArrayBuffer()
        {
            _storage = new RuntimeArrayBufferStorage(System.Array.Empty<byte>());
            _maxByteLength = 0;
            InitializeIntrinsicSurface();
        }

        public ArrayBuffer(object? length)
        {
            var byteLength = CoerceByteLength(length);
            _storage = new RuntimeArrayBufferStorage(
                byteLength == 0
                    ? System.Array.Empty<byte>()
                    : new byte[byteLength]);
            _maxByteLength = byteLength;
            InitializeIntrinsicSurface();
        }

        public ArrayBuffer(object? length, object? options)
            : this(length, options, supportsResizing: true)
        {
        }

        protected ArrayBuffer(object? length, object? options, bool supportsResizing)
        {
            var byteLength = CoerceByteLength(length);
            _storage = new RuntimeArrayBufferStorage(
                byteLength == 0
                    ? System.Array.Empty<byte>()
                    : new byte[byteLength]);

            if (supportsResizing && TryGetMaxByteLength(options, out var maxByteLength))
            {
                if (maxByteLength < byteLength)
                {
                    throw new RangeError("Invalid ArrayBuffer maxByteLength");
                }

                _maxByteLength = maxByteLength;
                _isResizable = true;
                InitializeIntrinsicSurface();
                return;
            }

            _maxByteLength = byteLength;
            InitializeIntrinsicSurface();
        }

        internal ArrayBuffer(byte[] bytes, bool cloneBuffer)
        {
            _storage = new RuntimeArrayBufferStorage(
                cloneBuffer ? (byte[])bytes.Clone() : bytes);
            _maxByteLength = _storage.Bytes.Length;
            InitializeIntrinsicSurface();
        }

        internal ArrayBuffer(RuntimeArrayBufferStorage storage)
        {
            _storage = storage;
            _maxByteLength = storage.Bytes.Length;
            InitializeIntrinsicSurface();
        }

        public double byteLength => _storage.Bytes.Length;
        public double maxByteLength => _maxByteLength;
        public bool resizable => _isResizable;

        public ArrayBuffer slice(object? start)
            => slice(start, null);

        public ArrayBuffer slice(object? start, object? end)
        {
            var bytes = _storage.Bytes;
            var startIndex = CoerceRelativeIndex(start, 0, bytes.Length);
            var endIndex = CoerceRelativeIndex(end, bytes.Length, bytes.Length);
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
            System.Buffer.BlockCopy(bytes, startIndex, copy, 0, length);
            return new ArrayBuffer(copy, cloneBuffer: false);
        }

        public object? resize()
            => resize(null);

        public object? resize(object? newLength)
        {
            if (!_isResizable)
            {
                throw new TypeError("ArrayBuffer is not resizable");
            }

            var byteLength = CoerceByteLength(newLength);
            if (byteLength > _maxByteLength)
            {
                throw new RangeError("Invalid ArrayBuffer length");
            }

            var bytes = _storage.Bytes;
            if (byteLength == bytes.Length)
            {
                return null;
            }

            var resized = byteLength == 0
                ? System.Array.Empty<byte>()
                : new byte[byteLength];
            System.Buffer.BlockCopy(bytes, 0, resized, 0, System.Math.Min(bytes.Length, byteLength));
            _storage.Bytes = resized;
            return null;
        }

        public static bool isView(object? arg)
            => arg is DataView or TypedArrayBase;

        internal int ByteLengthInt => _storage.Bytes.Length;

        internal byte[] RawBytes => _storage.Bytes;
        internal bool IsResizable => _isResizable;

        private void InitializeIntrinsicSurface()
        {
            if (GetType() == typeof(ArrayBuffer))
            {
                PrototypeChain.SetPrototype(this, Prototype);
            }
        }

        internal static int CoerceByteLength(object? value)
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

        private static bool TryGetMaxByteLength(object? options, out int maxByteLength)
        {
            maxByteLength = 0;
            if (options is null || options is JsNull || TypeUtilities.IsPrimitive(options))
            {
                return false;
            }

            var value = ObjectRuntime.GetItem(options, "maxByteLength");
            if (value is null)
            {
                return false;
            }

            maxByteLength = CoerceByteLength(value);
            return true;
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
