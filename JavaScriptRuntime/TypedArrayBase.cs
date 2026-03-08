using System;
using System.Collections;
using System.Collections.Generic;

namespace JavaScriptRuntime
{
    public abstract class TypedArrayBase
    {
        private ArrayBuffer _buffer = new ArrayBuffer();
        private int _byteOffset;
        private int _length;

        protected abstract int BytesPerElement { get; }
        protected abstract string TypedArrayName { get; }
        protected abstract double ReadElementValue(int index);
        protected abstract void WriteElementValue(int index, double value);
        protected abstract TypedArrayBase CreateSameType(ArrayBuffer buffer, int byteOffset, int length);

        protected ArrayBuffer BufferObject => _buffer;
        protected int ByteOffsetBytes => _byteOffset;
        protected int LengthElements => _length;

        public ArrayBuffer buffer => _buffer;
        public double byteOffset => _byteOffset;
        public double byteLength => (double)_length * BytesPerElement;
        public double length => _length;

        public double this[double index]
        {
            get => GetElement(index);
            set => SetElement(index, value);
        }

        internal void SetFromDouble(int index, double value)
        {
            if ((uint)index >= (uint)_length)
            {
                return;
            }

            WriteElementValue(index, value);
        }

        public object? set(object[]? args)
        {
            if (args == null || args.Length == 0 || args[0] == null)
            {
                return null;
            }

            var sourceValues = CaptureSourceValues(args[0]);
            var offset = args.Length > 1
                ? CoerceNonNegativeIndex(args[1], 0, $"Invalid {TypedArrayName} offset")
                : 0;

            if (offset > _length)
            {
                throw new RangeError($"Invalid {TypedArrayName} offset");
            }

            if (sourceValues.Count > _length - offset)
            {
                throw new RangeError("Source is too large for the destination typed array");
            }

            for (int i = 0; i < sourceValues.Count; i++)
            {
                WriteElementValue(offset + i, sourceValues[i]);
            }

            return null;
        }

        public double at()
            => AtCore(null);

        public double at(object? index)
            => AtCore(index);

        public bool includes(object? searchElement)
            => IncludesCore(searchElement, null);

        public bool includes(object? searchElement, object? fromIndex)
            => IncludesCore(searchElement, fromIndex);

        public double indexOf(object? searchElement)
            => IndexOfCore(searchElement, null);

        public double indexOf(object? searchElement, object? fromIndex)
            => IndexOfCore(searchElement, fromIndex);

        public IJavaScriptIterator values()
            => new TypedArrayIterator(this);

        protected void InitializeEmpty()
        {
            _buffer = new ArrayBuffer();
            _byteOffset = 0;
            _length = 0;
        }

        protected void InitializeFromLength(int length)
        {
            if (length < 0)
            {
                throw new RangeError($"Invalid {TypedArrayName} length");
            }

            var byteLengthLong = (long)length * BytesPerElement;
            if (byteLengthLong > int.MaxValue)
            {
                throw new RangeError($"Invalid {TypedArrayName} length");
            }

            _buffer = byteLengthLong == 0
                ? new ArrayBuffer()
                : new ArrayBuffer(new byte[(int)byteLengthLong], cloneBuffer: false);
            _byteOffset = 0;
            _length = length;
        }

        protected void InitializeFromExisting(ArrayBuffer buffer, int byteOffset, int length)
        {
            _buffer = buffer;
            _byteOffset = byteOffset;
            _length = length;
        }

        protected void InitializeFromBuffer(ArrayBuffer buffer, object? byteOffset, object? length)
        {
            if (buffer == null)
            {
                throw new TypeError($"First argument to {TypedArrayName} constructor must be an ArrayBuffer");
            }

            var offset = CoerceNonNegativeIndex(byteOffset, 0, $"Invalid {TypedArrayName} byteOffset");
            if (offset > buffer.ByteLengthInt || offset % BytesPerElement != 0)
            {
                throw new RangeError($"Invalid {TypedArrayName} byteOffset");
            }

            var remainingBytes = buffer.ByteLengthInt - offset;
            int elementLength;
            if (length is null || length is JsNull)
            {
                if (remainingBytes % BytesPerElement != 0)
                {
                    throw new RangeError($"Invalid {TypedArrayName} length");
                }

                elementLength = remainingBytes / BytesPerElement;
            }
            else
            {
                elementLength = CoerceNonNegativeIndex(length, 0, $"Invalid {TypedArrayName} length");
                if ((long)elementLength * BytesPerElement > remainingBytes)
                {
                    throw new RangeError($"Invalid {TypedArrayName} length");
                }
            }

            _buffer = buffer;
            _byteOffset = offset;
            _length = elementLength;
        }

        protected void InitializeFromArgument(object? arg)
        {
            if (arg is null || arg is JsNull)
            {
                InitializeEmpty();
                return;
            }

            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, null, null);
                return;
            }

            if (TryToNumber(arg, out var number))
            {
                InitializeFromLength(ToLength(number));
                return;
            }

            var values = CaptureSourceValues(arg);
            InitializeFromValues(values);
        }

        protected TypedArrayBase SliceCore(object? start, object? end)
        {
            var startIndex = CoerceRelativeIndex(start, 0, _length);
            var endIndex = CoerceRelativeIndex(end, _length, _length);
            if (endIndex < startIndex)
            {
                endIndex = startIndex;
            }

            var sliceLength = endIndex - startIndex;
            if (sliceLength <= 0)
            {
                return CreateSameType(new ArrayBuffer(), 0, 0);
            }

            var byteLength = checked(sliceLength * BytesPerElement);
            var copy = new byte[byteLength];
            var sourceByteOffset = checked(_byteOffset + (startIndex * BytesPerElement));
            Buffer.BlockCopy(_buffer.RawBytes, sourceByteOffset, copy, 0, byteLength);
            return CreateSameType(new ArrayBuffer(copy, cloneBuffer: false), 0, sliceLength);
        }

        protected TypedArrayBase SubarrayCore(object? start, object? end)
        {
            var startIndex = CoerceRelativeIndex(start, 0, _length);
            var endIndex = CoerceRelativeIndex(end, _length, _length);
            if (endIndex < startIndex)
            {
                endIndex = startIndex;
            }

            var subarrayLength = endIndex - startIndex;
            var byteOffset = checked(_byteOffset + (startIndex * BytesPerElement));
            return CreateSameType(_buffer, byteOffset, subarrayLength);
        }

        protected static bool TryToNumber(object? value, out double number)
        {
            switch (value)
            {
                case double d:
                    number = d;
                    return true;
                case float f:
                    number = f;
                    return true;
                case int i:
                    number = i;
                    return true;
                case long l:
                    number = l;
                    return true;
                case short s:
                    number = s;
                    return true;
                case byte b:
                    number = b;
                    return true;
                case bool boolean:
                    number = boolean ? 1 : 0;
                    return true;
                case string text when double.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed):
                    number = parsed;
                    return true;
                default:
                    number = 0;
                    return false;
            }
        }

        protected static int ToLength(double value)
        {
            if (double.IsNaN(value) || value <= 0)
            {
                return 0;
            }

            if (double.IsInfinity(value) || value > int.MaxValue)
            {
                return int.MaxValue;
            }

            return (int)global::System.Math.Truncate(value);
        }

        protected static int CoerceRelativeIndex(object? value, int defaultValue, int length)
        {
            if (value is null || value is JsNull)
            {
                return defaultValue;
            }

            var number = TypeUtilities.ToNumber(value);
            if (double.IsNaN(number) || double.IsNegativeInfinity(number))
            {
                return 0;
            }

            if (double.IsPositiveInfinity(number))
            {
                return length;
            }

            var truncated = global::System.Math.Truncate(number);
            if (truncated < 0)
            {
                truncated = global::System.Math.Max(length + truncated, 0);
            }

            if (truncated > length)
            {
                truncated = length;
            }

            return (int)truncated;
        }

        protected static int CoerceNonNegativeIndex(object? value, int defaultValue, string errorMessage)
        {
            if (value is null || value is JsNull)
            {
                return defaultValue;
            }

            var number = TypeUtilities.ToNumber(value);
            if (double.IsNaN(number))
            {
                return defaultValue;
            }

            if (double.IsInfinity(number) || number < 0)
            {
                throw new RangeError(errorMessage);
            }

            var truncated = global::System.Math.Truncate(number);
            if (truncated > int.MaxValue)
            {
                throw new RangeError(errorMessage);
            }

            return (int)truncated;
        }

        private void InitializeFromValues(IReadOnlyList<double> values)
        {
            InitializeFromLength(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                WriteElementValue(i, values[i]);
            }
        }

        private double GetElement(double index)
        {
            if (!TryGetElementIndex(index, out var elementIndex))
            {
                return 0.0;
            }

            return ReadElementValue(elementIndex);
        }

        private void SetElement(double index, double value)
        {
            if (!TryGetElementIndex(index, out var elementIndex))
            {
                return;
            }

            WriteElementValue(elementIndex, value);
        }

        private bool TryGetElementIndex(double index, out int elementIndex)
        {
            if (!double.IsNaN(index)
                && !double.IsInfinity(index)
                && index % 1.0 == 0.0
                && index >= 0
                && index <= int.MaxValue)
            {
                var candidate = (int)index;
                if ((uint)candidate < (uint)_length)
                {
                    elementIndex = candidate;
                    return true;
                }
            }

            elementIndex = 0;
            return false;
        }

        private double AtCore(object? index)
        {
            int elementIndex;
            if (index is null || index is JsNull)
            {
                elementIndex = 0;
            }
            else
            {
                var number = TypeUtilities.ToNumber(index);
                if (double.IsNaN(number))
                {
                    elementIndex = 0;
                }
                else if (double.IsNegativeInfinity(number))
                {
                    elementIndex = -1;
                }
                else if (double.IsPositiveInfinity(number))
                {
                    elementIndex = _length;
                }
                else
                {
                    var truncated = (int)global::System.Math.Truncate(number);
                    elementIndex = truncated < 0 ? _length + truncated : truncated;
                }
            }

            if ((uint)elementIndex >= (uint)_length)
            {
                return 0.0;
            }

            return ReadElementValue(elementIndex);
        }

        private bool IncludesCore(object? searchElement, object? fromIndex)
        {
            var startIndex = CoerceRelativeIndex(fromIndex, 0, _length);
            if (startIndex >= _length)
            {
                return false;
            }

            var searchNumber = TypeUtilities.ToNumber(searchElement);
            if (double.IsNaN(searchNumber))
            {
                for (int i = startIndex; i < _length; i++)
                {
                    if (double.IsNaN(ReadElementValue(i)))
                    {
                        return true;
                    }
                }

                return false;
            }

            for (int i = startIndex; i < _length; i++)
            {
                if (ReadElementValue(i) == searchNumber)
                {
                    return true;
                }
            }

            return false;
        }

        private double IndexOfCore(object? searchElement, object? fromIndex)
        {
            var startIndex = CoerceRelativeIndex(fromIndex, 0, _length);
            if (startIndex >= _length)
            {
                return -1.0;
            }

            var searchNumber = TypeUtilities.ToNumber(searchElement);
            if (double.IsNaN(searchNumber))
            {
                return -1.0;
            }

            for (int i = startIndex; i < _length; i++)
            {
                if (ReadElementValue(i) == searchNumber)
                {
                    return i;
                }
            }

            return -1.0;
        }

        private static List<double> CaptureSourceValues(object? source)
        {
            switch (source)
            {
                case TypedArrayBase typedArray:
                    {
                        var values = new List<double>((int)typedArray.length);
                        for (int i = 0; i < typedArray.length; i++)
                        {
                            values.Add(typedArray[(double)i]);
                        }

                        return values;
                    }

                case Array jsArray:
                    {
                        var values = new List<double>(jsArray.Count);
                        for (int i = 0; i < jsArray.Count; i++)
                        {
                            values.Add(TypeUtilities.ToNumber(jsArray[i]));
                        }

                        return values;
                    }

                case System.Array array:
                    {
                        var values = new List<double>(array.Length);
                        for (int i = 0; i < array.Length; i++)
                        {
                            values.Add(TypeUtilities.ToNumber(array.GetValue(i)));
                        }

                        return values;
                    }

                case IEnumerable enumerable when source is not string:
                    {
                        var values = new List<double>();
                        foreach (var item in enumerable)
                        {
                            values.Add(TypeUtilities.ToNumber(item));
                        }

                        return values;
                    }
            }

            return new List<double>();
        }
    }

    internal sealed class TypedArrayIterator : IJavaScriptIterator
    {
        private readonly TypedArrayBase _typedArray;
        private int _index;

        public TypedArrayIterator(TypedArrayBase typedArray)
        {
            _typedArray = typedArray;
        }

        public IteratorResultObject Next()
        {
            if (_index >= _typedArray.length)
            {
                return IteratorResult.Create(null, true);
            }

            return new IteratorResultObject(_typedArray[(double)_index++], done: false);
        }

        public object next(object? value = null)
            => Next();

        public bool HasReturn => false;

        public void Return()
        {
        }
    }
}
