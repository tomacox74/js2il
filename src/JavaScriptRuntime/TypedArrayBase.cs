using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime
{
    public abstract class TypedArrayBase : JsObject, IExoticJsObject
    {
        private ArrayBuffer _buffer = new ArrayBuffer();
        private int _byteOffset;
        private int _length;
        private bool _isLengthTracking;

        /// <summary>
        /// Element length used by the indexed fast path. It is only non-zero when the
        /// backing buffer is not resizable, in which case the view's length can never
        /// change and needs no invalidation. Views over resizable buffers keep this at
        /// zero so every access falls through to the dynamic slow path.
        /// </summary>
        private int _fastLength;

        protected abstract int BytesPerElement { get; }
        protected abstract string TypedArrayName { get; }
        protected abstract double ReadElementValue(int index);
        protected abstract void WriteElementValue(int index, double value);
        protected abstract TypedArrayBase CreateSameType(ArrayBuffer buffer, int byteOffset, int length);
        protected internal virtual object? ReadElementObject(int index)
            => ReadElementValue(index);
        protected internal virtual object? CoerceElementValue(object? value)
            => TypeUtilities.ToNumber(value);
        protected internal virtual void WriteElementObject(int index, object? value)
            => WriteElementValue(index, (double)CoerceElementValue(value)!);

        internal string TypedArrayNameValue => TypedArrayName;
        protected ArrayBuffer BufferObject => _buffer;
        protected int ByteOffsetBytes => _byteOffset;
        protected int LengthElements => GetCurrentLengthOrZero();

        public double BYTES_PER_ELEMENT => BytesPerElement;
        public ArrayBuffer buffer => _buffer;
        public double byteOffset => IsOutOfBounds ? 0 : _byteOffset;
        public double byteLength => (double)GetCurrentLengthOrZero() * BytesPerElement;
        public double length => GetCurrentLengthOrZero();

        public double this[double index]
        {
            get => GetElement(index);
            set => SetElement(index, value);
        }

        internal void SetFromDouble(int index, double value)
        {
            if ((uint)index >= (uint)GetCurrentLengthOrZero())
            {
                return;
            }

            WriteElementValue(index, value);
        }

        internal bool TrySetElementValue(int index, object? value)
        {
            if ((uint)index >= (uint)GetCurrentLengthOrZero())
            {
                return false;
            }

            WriteElementObject(index, value);
            return true;
        }

        internal byte[] CopyRawBytes()
        {
            var byteLength = checked(GetCurrentLengthOrZero() * BytesPerElement);
            if (byteLength == 0)
            {
                return System.Array.Empty<byte>();
            }

            var bytes = new byte[byteLength];
            Buffer.BlockCopy(_buffer.RawBytes, _byteOffset, bytes, 0, byteLength);
            return bytes;
        }

        public object? set(object[]? args)
        {
            if (args == null || args.Length == 0 || args[0] == null || args[0] is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            var sourceValues = CaptureSourceItems(args[0]);
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
                WriteElementObject(offset + i, sourceValues[i]);
            }

            return null;
        }

        public object? at()
            => AtCore(null);

        public object? at(object? index)
            => AtCore(index);

        public bool includes()
            => false;

        public bool includes(object? searchElement)
            => IncludesCore(searchElement, null);

        public bool includes(object? searchElement, object? fromIndex)
            => IncludesCore(searchElement, fromIndex);

        public double indexOf()
            => -1.0;

        public double indexOf(object? searchElement)
            => IndexOfCore(searchElement, null);

        public double indexOf(object? searchElement, object? fromIndex)
            => IndexOfCore(searchElement, fromIndex);

        public double lastIndexOf()
            => -1.0;

        public double lastIndexOf(object? searchElement)
            => LastIndexOfCore(searchElement, null);

        public double lastIndexOf(object? searchElement, object? fromIndex)
            => LastIndexOfCore(searchElement, fromIndex);

        public IJavaScriptIterator values()
            => new TypedArrayIterator(this, TypedArrayIteratorKind.Values);

        public IJavaScriptIterator keys()
            => new TypedArrayIterator(this, TypedArrayIteratorKind.Keys);

        public IJavaScriptIterator entries()
            => new TypedArrayIterator(this, TypedArrayIteratorKind.Entries);

        public string join()
            => JoinCore(null);

        public string join(object? separator)
            => JoinCore(separator);

        public string toString()
            => JoinCore(null);

        public string toString(object[]? args)
            => toString();

        public string toLocaleString()
            => JoinCore(null);

        public string toLocaleString(object[]? args)
            => toLocaleString();

        public TypedArrayBase reverse()
        {
            for (int left = 0, right = _length - 1; left < right; left++, right--)
            {
                var leftValue = ReadElementObject(left);
                var rightValue = ReadElementObject(right);
                WriteElementObject(left, rightValue);
                WriteElementObject(right, leftValue);
            }

            return this;
        }

        public TypedArrayBase copyWithin(object?[]? args)
        {
            var target = CoerceRelativeIndex(GetArgument(args, 0), 0, _length);
            var start = CoerceRelativeIndex(GetArgument(args, 1), 0, _length);
            var end = args != null && args.Length > 2
                ? CoerceRelativeIndex(args[2], _length, _length)
                : _length;
            var count = global::System.Math.Min(end - start, _length - target);
            if (count <= 0)
            {
                return this;
            }

            var sourceByteOffset = checked(_byteOffset + (start * BytesPerElement));
            var targetByteOffset = checked(_byteOffset + (target * BytesPerElement));
            var byteCount = checked(count * BytesPerElement);
            Buffer.BlockCopy(_buffer.RawBytes, sourceByteOffset, _buffer.RawBytes, targetByteOffset, byteCount);
            return this;
        }

        public TypedArrayBase fill(object[]? args)
        {
            var fillValue = CoerceElementValue(args != null && args.Length > 0 ? args[0] : null);
            var start = args != null && args.Length > 1
                ? CoerceRelativeIndex(args[1], 0, _length)
                : 0;
            var end = args != null && args.Length > 2
                ? CoerceRelativeIndex(args[2], _length, _length)
                : _length;

            if (end < start)
            {
                end = start;
            }

            for (int i = start; i < end; i++)
            {
                WriteElementObject(i, fillValue);
            }

            return this;
        }

        public bool every(object[]? args)
        {
            var callback = GetRequiredCallback(args, "every");
            var thisArg = GetThisArg(args);

            for (int i = 0; i < _length; i++)
            {
                var result = InvokeCallback(callback, thisArg, $"{TypedArrayName}.prototype.every", 3, ReadElementObject(i), (double)i, this, null);
                if (!Operators.IsTruthy(result))
                {
                    return false;
                }
            }

            return true;
        }

        public bool some(object[]? args)
        {
            var callback = GetRequiredCallback(args, "some");
            var thisArg = GetThisArg(args);

            for (int i = 0; i < _length; i++)
            {
                var result = InvokeCallback(callback, thisArg, $"{TypedArrayName}.prototype.some", 3, ReadElementObject(i), (double)i, this, null);
                if (Operators.IsTruthy(result))
                {
                    return true;
                }
            }

            return false;
        }

        public object? find(object[]? args)
        {
            var callback = GetRequiredCallback(args, "find");
            var thisArg = GetThisArg(args);

            for (int i = 0; i < _length; i++)
            {
                var value = ReadElementObject(i);
                var result = InvokeCallback(callback, thisArg, $"{TypedArrayName}.prototype.find", 3, value, (double)i, this, null);
                if (Operators.IsTruthy(result))
                {
                    return value;
                }
            }

            return null;
        }

        public double findIndex(object[]? args)
        {
            var callback = GetRequiredCallback(args, "findIndex");
            var thisArg = GetThisArg(args);

            for (int i = 0; i < _length; i++)
            {
                var result = InvokeCallback(callback, thisArg, $"{TypedArrayName}.prototype.findIndex", 3, ReadElementObject(i), (double)i, this, null);
                if (Operators.IsTruthy(result))
                {
                    return i;
                }
            }

            return -1.0;
        }

        public object? findLast(object?[]? args)
        {
            var callback = GetRequiredCallback(args, "findLast");
            var thisArg = GetThisArg(args);

            for (int i = _length - 1; i >= 0; i--)
            {
                var value = ReadElementObject(i);
                var result = InvokeCallback(callback, thisArg, $"{TypedArrayName}.prototype.findLast", 3, value, (double)i, this, null);
                if (Operators.IsTruthy(result))
                {
                    return value;
                }
            }

            return null;
        }

        public double findLastIndex(object?[]? args)
        {
            var callback = GetRequiredCallback(args, "findLastIndex");
            var thisArg = GetThisArg(args);

            for (int i = _length - 1; i >= 0; i--)
            {
                var result = InvokeCallback(callback, thisArg, $"{TypedArrayName}.prototype.findLastIndex", 3, ReadElementObject(i), (double)i, this, null);
                if (Operators.IsTruthy(result))
                {
                    return i;
                }
            }

            return -1.0;
        }

        public object? forEach(object[]? args)
        {
            var callback = GetRequiredCallback(args, "forEach");
            var thisArg = GetThisArg(args);

            for (int i = 0; i < _length; i++)
            {
                _ = InvokeCallback(callback, thisArg, $"{TypedArrayName}.prototype.forEach", 3, ReadElementObject(i), (double)i, this, null);
            }

            return null;
        }

        public TypedArrayBase map(object[]? args)
        {
            var callback = GetRequiredCallback(args, "map");
            var thisArg = GetThisArg(args);
            ObserveSpeciesConstructor();
            var mapped = CreateSameTypeWithLength(_length);

            for (int i = 0; i < _length; i++)
            {
                var value = ReadElementObject(i);
                var result = InvokeCallback(callback, thisArg, $"{TypedArrayName}.prototype.map", 3, value, (double)i, this, null);
                mapped.WriteElementObject(i, result);
            }

            return mapped;
        }

        public TypedArrayBase filter(object[]? args)
        {
            var callback = GetRequiredCallback(args, "filter");
            var thisArg = GetThisArg(args);
            var keptValues = new List<object?>();

            for (int i = 0; i < _length; i++)
            {
                var value = ReadElementObject(i);
                var result = InvokeCallback(callback, thisArg, $"{TypedArrayName}.prototype.filter", 3, value, (double)i, this, null);
                if (Operators.IsTruthy(result))
                {
                    keptValues.Add(value);
                }
            }

            return CreateSameTypeFromValues(keptValues);
        }

        public object? reduce(object[]? args)
        {
            var callback = GetRequiredCallback(args, "reduce");
            var hasInitialValue = args != null && args.Length > 1;

            if (_length == 0 && !hasInitialValue)
            {
                throw new TypeError("Reduce of empty typed array with no initial value");
            }

            object? accumulator;
            int startIndex;
            if (hasInitialValue)
            {
                accumulator = args![1];
                startIndex = 0;
            }
            else
            {
                accumulator = ReadElementObject(0);
                startIndex = 1;
            }

            for (int i = startIndex; i < _length; i++)
            {
                accumulator = InvokeCallback(callback, null, $"{TypedArrayName}.prototype.reduce", 4, accumulator, ReadElementObject(i), (double)i, this);
            }

            return accumulator;
        }

        public object? reduceRight(object?[]? args)
        {
            var callback = GetRequiredCallback(args, "reduceRight");
            var hasInitialValue = args != null && args.Length > 1;

            if (_length == 0 && !hasInitialValue)
            {
                throw new TypeError("Reduce of empty typed array with no initial value");
            }

            object? accumulator;
            int startIndex;
            if (hasInitialValue)
            {
                accumulator = args![1];
                startIndex = _length - 1;
            }
            else
            {
                accumulator = ReadElementObject(_length - 1);
                startIndex = _length - 2;
            }

            for (int i = startIndex; i >= 0; i--)
            {
                accumulator = InvokeCallback(
                    callback,
                    null,
                    $"{TypedArrayName}.prototype.reduceRight",
                    4,
                    accumulator,
                    ReadElementObject(i),
                    (double)i,
                    this);
            }

            return accumulator;
        }

        public TypedArrayBase toReversed()
        {
            var reversed = CreateSameTypeWithLength(_length);
            for (int i = 0; i < _length; i++)
            {
                reversed.WriteElementObject(i, ReadElementObject(_length - i - 1));
            }

            return reversed;
        }

        public TypedArrayBase sort(object?[]? args)
        {
            var sortedValues = GetSortedValues(args);
            for (int i = 0; i < sortedValues.Count; i++)
            {
                WriteElementObject(i, sortedValues[i]);
            }

            return this;
        }

        public TypedArrayBase toSorted(object?[]? args)
            => CreateSameTypeFromValues(GetSortedValues(args));

        public TypedArrayBase with(object?[]? args)
        {
            var index = GetArgument(args, 0);
            var value = GetArgument(args, 1);
            var relativeIndex = ToIntegerOrInfinity(index);
            var actualIndex = relativeIndex < 0
                ? _length + relativeIndex
                : relativeIndex;

            // Coerce the replacement before validating the index. Its side effects
            // are observable before the copy is made and before a RangeError.
            var elementValue = CoerceElementValue(value);
            if (actualIndex < 0 || actualIndex >= _length)
            {
                throw new RangeError($"Invalid {TypedArrayName} index");
            }

            var result = CreateSameTypeWithLength(_length);
            for (var i = 0; i < _length; i++)
            {
                result.WriteElementObject(i, ReadElementObject(i));
            }

            result.WriteElementObject((int)actualIndex, elementValue);
            return result;
        }

        protected void InitializeEmpty()
        {
            _buffer = new ArrayBuffer();
            _byteOffset = 0;
            _length = 0;
            _isLengthTracking = false;
            UpdateFastLength();
            InitializeIntrinsicSurface();
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
            _isLengthTracking = false;
            UpdateFastLength();
            InitializeIntrinsicSurface();
        }

        protected void InitializeFromExisting(ArrayBuffer buffer, int byteOffset, int length)
        {
            _buffer = buffer;
            _byteOffset = byteOffset;
            _length = length;
            _isLengthTracking = false;
            UpdateFastLength();
            InitializeIntrinsicSurface();
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
            _isLengthTracking = (length is null || length is JsNull) && buffer.IsResizable;
            UpdateFastLength();
            InitializeIntrinsicSurface();
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

            if (IsConstructorLengthArgument(arg))
            {
                InitializeFromLength(ToConstructorLength(TypeUtilities.ToNumber(arg), $"Invalid {TypedArrayName} length"));
                return;
            }

            var values = CaptureSourceItems(arg);
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

        protected TypedArrayBase CreateSameTypeWithLength(int length)
        {
            if (length <= 0)
            {
                return CreateSameType(new ArrayBuffer(), 0, 0);
            }

            var byteLength = checked(length * BytesPerElement);
            return CreateSameType(new ArrayBuffer(new byte[byteLength], cloneBuffer: false), 0, length);
        }

        protected TypedArrayBase CreateSameTypeFromValues(IReadOnlyList<object?> values)
        {
            var result = CreateSameTypeWithLength(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                result.WriteElementObject(i, values[i]);
            }

            return result;
        }

        protected TypedArrayBase CreateSameTypeFromValues(IReadOnlyList<double> values)
        {
            var result = CreateSameTypeWithLength(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                result.WriteElementValue(i, values[i]);
            }

            return result;
        }

        private void ObserveSpeciesConstructor()
        {
            var constructor = ObjectRuntime.GetItem(this, "constructor");
            if (constructor is null || constructor is JsNull)
            {
                return;
            }

            _ = ObjectRuntime.GetItem(constructor, Symbol.species);
        }

        private List<object?> GetSortedValues(object?[]? args)
        {
            var compareFunction = args != null && args.Length > 0 ? args[0] : null;
            if (compareFunction is not null && !CallableOperations.IsCallable(compareFunction))
            {
                throw new TypeError($"{TypedArrayName}.prototype.sort requires a callback function");
            }

            var values = new List<SortableTypedArrayValue>(_length);
            for (int i = 0; i < _length; i++)
            {
                values.Add(new SortableTypedArrayValue(ReadElementObject(i)));
            }

            StableSortValues(values, compareFunction);

            var sorted = new List<object?>(_length);
            foreach (var value in values)
            {
                sorted.Add(value.Value);
            }

            return sorted;
        }

        private void StableSortValues(List<SortableTypedArrayValue> values, object? compareFunction)
        {
            var buffer = new SortableTypedArrayValue[values.Count];
            for (var width = 1; width < values.Count; width *= 2)
            {
                for (var start = 0; start < values.Count; start += width * 2)
                {
                    var middle = global::System.Math.Min(start + width, values.Count);
                    var end = global::System.Math.Min(start + (width * 2), values.Count);
                    var left = start;
                    var right = middle;
                    var write = start;

                    while (left < middle && right < end)
                    {
                        if (CompareValues(values[left].Value, values[right].Value, compareFunction) <= 0)
                        {
                            buffer[write++] = values[left++];
                        }
                        else
                        {
                            buffer[write++] = values[right++];
                        }
                    }

                    while (left < middle)
                    {
                        buffer[write++] = values[left++];
                    }

                    while (right < end)
                    {
                        buffer[write++] = values[right++];
                    }

                    for (var index = start; index < end; index++)
                    {
                        values[index] = buffer[index];
                    }
                }

                if (width > values.Count / 2)
                {
                    break;
                }
            }
        }

        private int CompareValues(object? left, object? right, object? compareFunction)
            => compareFunction is not null
                ? CompareUsingCallback(compareFunction, left, right)
                : CompareDefaultValues(left, right);

        private int CompareUsingCallback(object callback, object? left, object? right)
        {
            var result = TypeUtilities.ToNumber(
                InvokeCallback(callback, null, $"{TypedArrayName}.prototype.sort", 2, left, right, null, null));

            if (double.IsNaN(result) || result == 0)
            {
                return 0;
            }

            return result < 0 ? -1 : 1;
        }

        private static int CompareDefaultValues(object? leftValue, object? rightValue)
        {
            if (leftValue is BigInteger leftBigInt && rightValue is BigInteger rightBigInt)
            {
                return leftBigInt.CompareTo(rightBigInt);
            }

            var left = TypeUtilities.ToNumber(leftValue);
            var right = TypeUtilities.ToNumber(rightValue);
            if (double.IsNaN(left))
            {
                return double.IsNaN(right) ? 0 : 1;
            }

            if (double.IsNaN(right))
            {
                return -1;
            }

            if (left == 0 && right == 0)
            {
                var leftIsNegative = BitConverter.DoubleToInt64Bits(left) < 0;
                var rightIsNegative = BitConverter.DoubleToInt64Bits(right) < 0;
                return leftIsNegative == rightIsNegative ? 0 : leftIsNegative ? -1 : 1;
            }

            return left < right ? -1 : left > right ? 1 : 0;
        }

        private readonly record struct SortableTypedArrayValue(object? Value);

        protected static T FromSource<T>(string typedArrayName, object? source, object? mapper, object? thisArg, Func<object?[], T> factory)
            where T : TypedArrayBase
        {
            if (source is null || source is JsNull)
            {
                throw new TypeError($"{typedArrayName}.from requires a source value");
            }

            var items = CaptureSourceItems(source);
            if (mapper is null || mapper is JsNull)
            {
                return factory(items.Count == 0 ? System.Array.Empty<object?>() : items.ToArray());
            }

            var mapped = new object?[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                mapped[i] = InvokeCallback(mapper, thisArg, $"{typedArrayName}.from", 2, items[i], (double)i, null, null);
            }

            return factory(mapped);
        }

        private static bool IsConstructorLengthArgument(object? value)
        {
            switch (value)
            {
                case double:
                case float:
                case decimal:
                case int:
                case long:
                case short:
                case sbyte:
                case byte:
                case uint:
                case ulong:
                case ushort:
                case bool:
                case string:
                    return true;
                default:
                    return false;
            }
        }

        protected static int ToConstructorLength(double value, string errorMessage)
        {
            if (double.IsNaN(value) || value == 0)
            {
                return 0;
            }

            if (double.IsInfinity(value) || value < 0)
            {
                throw new RangeError(errorMessage);
            }

            var truncated = global::System.Math.Truncate(value);
            if (truncated > int.MaxValue)
            {
                throw new RangeError(errorMessage);
            }

            return (int)truncated;
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
            if (value is null)
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

        private static double ToIntegerOrInfinity(object? value)
        {
            var number = TypeUtilities.ToNumber(value);
            if (double.IsNaN(number) || number == 0)
            {
                return 0;
            }

            if (double.IsInfinity(number))
            {
                return number;
            }

            return global::System.Math.Truncate(number);
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

        private void InitializeFromValues(IReadOnlyList<object?> values)
        {
            InitializeFromLength(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                WriteElementObject(i, values[i]);
            }
        }

        private void InitializeIntrinsicSurface()
        {
            PrototypeChain.InitializePrototype(this, GlobalThis.GetTypedArrayInstancePrototype(this));
        }

        internal override bool TryGetInvariantOwnPropertyValue(string key, out object? value)
        {
            if (ObjectRuntime.TryParseCanonicalIndexString(key, out var index)
                && (uint)index < (uint)GetCurrentLengthOrZero())
            {
                value = ReadElementObject(index);
                return true;
            }

            value = null;
            return false;
        }

        internal override bool TryGetOwnPropertyValue(string key, out object? value)
        {
            if (TryGetInvariantOwnPropertyValue(key, out value))
            {
                return true;
            }

            if (PropertyDescriptorStore.GetOwnLookupCore(this, key, out _) == PropertyDescriptorLookup.None)
            {
                if (string.Equals(key, "BYTES_PER_ELEMENT", StringComparison.Ordinal))
                {
                    value = BYTES_PER_ELEMENT;
                    return true;
                }
            }

            return base.TryGetOwnPropertyValue(key, out value);
        }

        internal override bool HasOwnPropertyValue(string key)
            => (ObjectRuntime.TryParseCanonicalIndexString(key, out var index)
                && (uint)index < (uint)GetCurrentLengthOrZero())
                || base.HasOwnPropertyValue(key);

        internal override bool SetOwnPropertyValue(string key, object? value)
        {
            if (ObjectRuntime.TryParseCanonicalIndexString(key, out var index))
            {
                return TrySetElementValue(index, value);
            }

            return base.SetOwnPropertyValue(key, value);
        }

        internal override bool DefineOwnProperty(string key, JsPropertyDescriptor descriptor)
        {
            if (!ObjectRuntime.TryParseCanonicalIndexString(key, out var index)
                || (uint)index >= (uint)GetCurrentLengthOrZero())
            {
                return base.DefineOwnProperty(key, descriptor);
            }

            if (descriptor.Kind != JsPropertyDescriptorKind.Data
                || descriptor.Configurable
                || !descriptor.Enumerable
                || !descriptor.Writable)
            {
                return false;
            }

            WriteElementObject(index, descriptor.Value);
            return true;
        }

        internal override bool DeleteOwnProperty(string key)
        {
            if (ObjectRuntime.TryParseCanonicalIndexString(key, out var index)
                && (uint)index < (uint)GetCurrentLengthOrZero())
            {
                return false;
            }

            return base.DeleteOwnProperty(key);
        }

        internal override IEnumerable<string> GetOwnPropertyKeys()
        {
            var keys = new List<string>();
            for (var index = 0; index < GetCurrentLengthOrZero(); index++)
            {
                var key = index.ToString(System.Globalization.CultureInfo.InvariantCulture);
                if (!PropertyDescriptorStore.IsDeleted(this, key))
                {
                    keys.Add(key);
                }
            }

            keys.AddRange(base.GetOwnPropertyKeys().Where(key => !keys.Contains(key, StringComparer.Ordinal)));
            return keys;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double GetElement(double index)
        {
            if (!TryGetElementIndex(index, out var elementIndex))
            {
                return 0.0;
            }

            return ReadElementValue(elementIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetElement(double index, double value)
        {
            if (!TryGetElementIndex(index, out var elementIndex))
            {
                return;
            }

            WriteElementValue(elementIndex, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetElementIndex(double index, out int elementIndex)
        {
            // Fast path for views over non-resizable buffers: a single field compare with
            // no buffer dereference. The equality check rejects NaN, infinities, negative
            // values, and non-integers, so they fall through to the slow path.
            var candidate = (int)index;
            if (candidate == index && (uint)candidate < (uint)_fastLength)
            {
                elementIndex = candidate;
                return true;
            }

            return TryGetElementIndexSlow(index, out elementIndex);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool TryGetElementIndexSlow(double index, out int elementIndex)
        {
            if (!double.IsNaN(index)
                && !double.IsInfinity(index)
                && index >= 0
                && index <= int.MaxValue)
            {
                var candidate = (int)index;
                if (candidate == index && (uint)candidate < (uint)GetCurrentLengthOrZero())
                {
                    elementIndex = candidate;
                    return true;
                }
            }

            elementIndex = 0;
            return false;
        }

        /// <summary>
        /// Recomputes <see cref="_fastLength"/>. This is safe to compute once per
        /// initialization because <c>ArrayBuffer.resizable</c> is fixed at construction and
        /// the runtime has no <c>transfer</c>/detach support, so the byte length of a
        /// non-resizable buffer is immutable. If detach, transfer, or growable
        /// SharedArrayBuffer support is added, this cache must be invalidated there.
        /// </summary>
        private void UpdateFastLength()
            => _fastLength = _buffer.IsResizable ? 0 : _length;

        internal int GetCurrentLengthForIteration()
        {
            if (IsOutOfBounds)
            {
                throw new TypeError("TypedArray is out of bounds");
            }

            return GetCurrentLengthOrZero();
        }

        private bool IsOutOfBounds
        {
            get
            {
                if (!_buffer.IsResizable)
                {
                    return false;
                }

                if (_isLengthTracking)
                {
                    return _byteOffset > _buffer.ByteLengthInt;
                }

                return (long)_byteOffset + ((long)_length * BytesPerElement) > _buffer.ByteLengthInt;
            }
        }

        private int GetCurrentLengthOrZero()
        {
            if (_fastLength != 0)
            {
                return _fastLength;
            }

            if (IsOutOfBounds)
            {
                return 0;
            }

            if (!_isLengthTracking)
            {
                return _length;
            }

            return (_buffer.ByteLengthInt - _byteOffset) / BytesPerElement;
        }

        private object? AtCore(object? index)
        {
            var length = GetCurrentLengthOrZero();
            var relativeIndex = ToIntegerOrInfinity(index);
            var elementIndex = relativeIndex >= 0
                ? relativeIndex
                : length + relativeIndex;

            if (elementIndex < 0 || elementIndex >= length)
            {
                return null;
            }

            return ReadElementObject((int)elementIndex);
        }

        private bool IncludesCore(object? searchElement, object? fromIndex)
        {
            var startIndex = CoerceRelativeIndex(fromIndex, 0, _length);
            if (startIndex >= _length)
            {
                return false;
            }

            for (int i = startIndex; i < _length; i++)
            {
                if (ElementValuesEqual(ReadElementObject(i), searchElement, sameValueZero: true))
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

            for (int i = startIndex; i < _length; i++)
            {
                if (ElementValuesEqual(ReadElementObject(i), searchElement, sameValueZero: false))
                {
                    return i;
                }
            }

            return -1.0;
        }

        private double LastIndexOfCore(object? searchElement, object? fromIndex)
        {
            if (_length == 0)
            {
                return -1.0;
            }

            var startIndex = _length - 1;
            if (fromIndex is not null && fromIndex is not JsNull)
            {
                var number = TypeUtilities.ToNumber(fromIndex);
                if (!double.IsNaN(number))
                {
                    if (double.IsNegativeInfinity(number))
                    {
                        return -1.0;
                    }

                    if (!double.IsPositiveInfinity(number))
                    {
                        var truncated = (int)global::System.Math.Truncate(number);
                        startIndex = truncated < 0 ? _length + truncated : truncated;
                    }
                }
            }

            if (startIndex >= _length)
            {
                startIndex = _length - 1;
            }

            for (int i = startIndex; i >= 0; i--)
            {
                if (ElementValuesEqual(ReadElementObject(i), searchElement, sameValueZero: false))
                {
                    return i;
                }
            }

            return -1.0;
        }

        private static bool ElementValuesEqual(object? element, object? searchElement, bool sameValueZero)
        {
            if (element is BigInteger elementBigInt)
            {
                return searchElement is BigInteger searchBigInt
                    && elementBigInt == searchBigInt;
            }

            if (!TryGetNumberPrimitive(searchElement, out var searchNumber))
            {
                return false;
            }

            var elementNumber = TypeUtilities.ToNumber(element);
            if (double.IsNaN(elementNumber) || double.IsNaN(searchNumber))
            {
                return sameValueZero
                    && double.IsNaN(elementNumber)
                    && double.IsNaN(searchNumber);
            }

            return elementNumber == searchNumber;
        }

        private static bool TryGetNumberPrimitive(object? value, out double number)
        {
            switch (value)
            {
                case double doubleValue:
                    number = doubleValue;
                    return true;
                case float floatValue:
                    number = floatValue;
                    return true;
                case decimal decimalValue:
                    number = (double)decimalValue;
                    return true;
                case int intValue:
                    number = intValue;
                    return true;
                case uint uintValue:
                    number = uintValue;
                    return true;
                case long longValue:
                    number = longValue;
                    return true;
                case ulong ulongValue:
                    number = ulongValue;
                    return true;
                case short shortValue:
                    number = shortValue;
                    return true;
                case ushort ushortValue:
                    number = ushortValue;
                    return true;
                case sbyte sbyteValue:
                    number = sbyteValue;
                    return true;
                case byte byteValue:
                    number = byteValue;
                    return true;
                default:
                    number = 0;
                    return false;
            }
        }

        private string JoinCore(object? separator)
        {
            if (_length == 0)
            {
                return string.Empty;
            }

            var actualSeparator = separator is null
                ? ","
                : DotNet2JSConversions.ToString(separator);
            var parts = new string[_length];
            for (int i = 0; i < _length; i++)
            {
                parts[i] = DotNet2JSConversions.ToString(ReadElementObject(i));
            }

            return string.Join(actualSeparator, parts);
        }

        private object? GetRequiredCallback(object?[]? args, string methodName)
        {
            var callback = args != null && args.Length > 0 ? args[0] : null;
            if (!CallableOperations.IsCallable(callback))
            {
                throw new TypeError($"{TypedArrayName}.prototype.{methodName} requires a callback function");
            }

            return callback;
        }

        private static object? GetThisArg(object?[]? args)
            => args != null && args.Length > 1 ? args[1] : null;

        private static object? GetArgument(object?[]? args, int index)
            => args != null && args.Length > index ? args[index] : null;

        private static object? InvokeCallback(object? callback, object? thisArg, string callbackKind, int argCount, object? a0, object? a1, object? a2, object? a3)
        {
            if (!CallableOperations.IsCallable(callback))
            {
                throw new TypeError($"{callbackKind} callback is not a function");
            }

            return argCount switch
            {
                <= 0 => CallableOperations.Call0(callback, thisArg),
                1 => CallableOperations.Call1(callback, thisArg, a0),
                2 => CallableOperations.Call2(callback, thisArg, a0, a1),
                3 => CallableOperations.Call3(callback, thisArg, a0, a1, a2),
                _ => CallableOperations.Call4(callback, thisArg, a0, a1, a2, a3)
            };
        }

        private static List<object?> CaptureSourceItems(object? source)
        {
            switch (source)
            {
                case TypedArrayBase typedArray:
                    {
                        var values = new List<object?>(typedArray.LengthElements);
                        for (int i = 0; i < typedArray.LengthElements; i++)
                        {
                            values.Add(typedArray.ReadElementObject(i));
                        }

                        return values;
                    }

                case Array jsArray:
                    {
                        var values = new List<object?>(jsArray.Count);
                        for (int i = 0; i < jsArray.Count; i++)
                        {
                            values.Add(jsArray[i]);
                        }

                        return values;
                    }

                case System.Array array:
                    {
                        var values = new List<object?>(array.Length);
                        for (int i = 0; i < array.Length; i++)
                        {
                            values.Add(array.GetValue(i));
                        }

                        return values;
                    }

                case IJavaScriptIterator iterator:
                    {
                        var values = new List<object?>();
                        while (true)
                        {
                            var next = iterator.Next();
                            if (next.done)
                            {
                                return values;
                            }

                            values.Add(next.value);
                        }
                    }

            }

            if (source is not null && source is not JsNull
                && ObjectRuntime.GetItem(source, Symbol.iterator) is not null)
            {
                var iterator = ObjectRuntime.GetIterator(source);
                var values = new List<object?>();
                while (true)
                {
                    var next = iterator.Next();
                    if (next.done)
                    {
                        return values;
                    }

                    values.Add(next.value);
                }
            }

            if (source is IEnumerable enumerable && source is not string && !TryGetArrayLikeLength(source, out _))
            {
                var values = new List<object?>();
                foreach (var item in enumerable)
                {
                    values.Add(item);
                }

                return values;
            }

            if (TryGetArrayLikeLength(source, out var length))
            {
                var values = new List<object?>(length);
                for (int i = 0; i < length; i++)
                {
                    values.Add(JavaScriptRuntime.ObjectRuntime.GetItem(source!, (double)i));
                }

                return values;
            }

            return new List<object?>();
        }

        private static bool TryGetArrayLikeLength(object? source, out int length)
        {
            length = 0;
            if (source is null || source is JsNull || source is string)
            {
                return false;
            }

            var lengthValue = JavaScriptRuntime.ObjectRuntime.GetProperty(source, "length");
            if (lengthValue is null || lengthValue is JsNull)
            {
                return false;
            }

            length = ToLength(TypeUtilities.ToNumber(lengthValue));
            return true;
        }
    }

    internal enum TypedArrayIteratorKind
    {
        Keys,
        Values,
        Entries
    }

    internal sealed class TypedArrayIterator : JsObject, IJavaScriptIterator
    {
        private readonly TypedArrayBase _typedArray;
        private readonly TypedArrayIteratorKind _kind;
        private int _index;

        public TypedArrayIterator(TypedArrayBase typedArray, TypedArrayIteratorKind kind)
        {
            _typedArray = typedArray;
            _kind = kind;
            // Spec (22.2.3.6/22.2.3.7/22.2.3.19): %TypedArray%.prototype.{entries,keys,values}
            // all invoke the same CreateArrayIterator abstract operation used by
            // Array.prototype.{entries,keys,values}, so the returned iterator's
            // [[Prototype]] is the shared %ArrayIteratorPrototype%, not %IteratorPrototype%.
            PrototypeChain.InitializePrototype(this, JavaScriptRuntime.Array.IteratorPrototype);
        }

        public IteratorResultObject Next()
        {
            if (_index >= _typedArray.GetCurrentLengthForIteration())
            {
                return IteratorResult.Create(null, true);
            }

            object? value = _kind switch
            {
                TypedArrayIteratorKind.Keys => (double)_index,
                TypedArrayIteratorKind.Entries => new JavaScriptRuntime.Array(new object?[] { (double)_index, _typedArray.ReadElementObject(_index) }),
                _ => _typedArray.ReadElementObject(_index)
            };

            _index++;
            return new IteratorResultObject(value, done: false);
        }

        public object next(object? value = null)
            => Next();

        public bool HasReturn => false;

        public void Return()
        {
        }
    }
}
