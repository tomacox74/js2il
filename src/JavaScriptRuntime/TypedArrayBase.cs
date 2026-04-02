using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

        public double BYTES_PER_ELEMENT => BytesPerElement;
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
            if (args == null || args.Length == 0 || args[0] == null || args[0] is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
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
                var leftValue = ReadElementValue(left);
                var rightValue = ReadElementValue(right);
                WriteElementValue(left, rightValue);
                WriteElementValue(right, leftValue);
            }

            return this;
        }

        public TypedArrayBase fill(object[]? args)
        {
            var fillValue = args != null && args.Length > 0
                ? TypeUtilities.ToNumber(args[0])
                : TypeUtilities.ToNumber(null);
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
                WriteElementValue(i, fillValue);
            }

            return this;
        }

        public bool every(object[]? args)
        {
            var callback = GetRequiredCallback(args, "every");
            var thisArg = GetThisArg(args);

            for (int i = 0; i < _length; i++)
            {
                var result = InvokeCallback(callback, thisArg, $"{TypedArrayName}.prototype.every", 3, ReadElementValue(i), (double)i, this, null);
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
                var result = InvokeCallback(callback, thisArg, $"{TypedArrayName}.prototype.some", 3, ReadElementValue(i), (double)i, this, null);
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
                var value = ReadElementValue(i);
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
                var result = InvokeCallback(callback, thisArg, $"{TypedArrayName}.prototype.findIndex", 3, ReadElementValue(i), (double)i, this, null);
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
                _ = InvokeCallback(callback, thisArg, $"{TypedArrayName}.prototype.forEach", 3, ReadElementValue(i), (double)i, this, null);
            }

            return null;
        }

        public TypedArrayBase map(object[]? args)
        {
            var callback = GetRequiredCallback(args, "map");
            var thisArg = GetThisArg(args);
            var mapped = CreateSameTypeWithLength(_length);

            for (int i = 0; i < _length; i++)
            {
                var value = ReadElementValue(i);
                var result = InvokeCallback(callback, thisArg, $"{TypedArrayName}.prototype.map", 3, value, (double)i, this, null);
                mapped.WriteElementValue(i, TypeUtilities.ToNumber(result));
            }

            return mapped;
        }

        public TypedArrayBase filter(object[]? args)
        {
            var callback = GetRequiredCallback(args, "filter");
            var thisArg = GetThisArg(args);
            var keptValues = new List<double>();

            for (int i = 0; i < _length; i++)
            {
                var value = ReadElementValue(i);
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
                accumulator = ReadElementValue(0);
                startIndex = 1;
            }

            for (int i = startIndex; i < _length; i++)
            {
                accumulator = InvokeCallback(callback, null, $"{TypedArrayName}.prototype.reduce", 4, accumulator, ReadElementValue(i), (double)i, this);
            }

            return accumulator;
        }

        protected void InitializeEmpty()
        {
            _buffer = new ArrayBuffer();
            _byteOffset = 0;
            _length = 0;
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
            InitializeIntrinsicSurface();
        }

        protected void InitializeFromExisting(ArrayBuffer buffer, int byteOffset, int length)
        {
            _buffer = buffer;
            _byteOffset = byteOffset;
            _length = length;
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

        protected TypedArrayBase CreateSameTypeWithLength(int length)
        {
            if (length <= 0)
            {
                return CreateSameType(new ArrayBuffer(), 0, 0);
            }

            var byteLength = checked(length * BytesPerElement);
            return CreateSameType(new ArrayBuffer(new byte[byteLength], cloneBuffer: false), 0, length);
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

        private void InitializeIntrinsicSurface()
        {
            PropertyDescriptorStore.DefineOrUpdate(this, Symbol.iterator.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = new Func<IJavaScriptIterator>(values)
            });
            PropertyDescriptorStore.DefineOrUpdate(this, Symbol.toStringTag.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = TypedArrayName
            });
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
            if (!double.IsNaN(index)
                && !double.IsInfinity(index)
                && index >= 0
                && index <= int.MaxValue)
            {
                var candidate = (int)index;
                if (candidate == index && (uint)candidate < (uint)_length)
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

        private double LastIndexOfCore(object? searchElement, object? fromIndex)
        {
            if (_length == 0)
            {
                return -1.0;
            }

            var searchNumber = TypeUtilities.ToNumber(searchElement);
            if (double.IsNaN(searchNumber))
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
                if (ReadElementValue(i) == searchNumber)
                {
                    return i;
                }
            }

            return -1.0;
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
                parts[i] = DotNet2JSConversions.ToString(ReadElementValue(i));
            }

            return string.Join(actualSeparator, parts);
        }

        private object? GetRequiredCallback(object[]? args, string methodName)
        {
            var callback = args != null && args.Length > 0 ? args[0] : null;
            if (callback is not Delegate)
            {
                throw new TypeError($"{TypedArrayName}.prototype.{methodName} requires a callback function");
            }

            return callback;
        }

        private static object? GetThisArg(object[]? args)
            => args != null && args.Length > 1 ? args[1] : null;

        private static object? InvokeCallback(object? callback, object? thisArg, string callbackKind, int argCount, object? a0, object? a1, object? a2, object? a3)
        {
            if (callback is not Delegate del)
            {
                throw new TypeError($"{callbackKind} callback is not a function");
            }

            var previousThis = RuntimeServices.SetCurrentThis(thisArg);
            try
            {
                return argCount switch
                {
                    <= 0 => Closure.InvokeWithArgs0(del, System.Array.Empty<object>()),
                    1 => Closure.InvokeWithArgs1(del, System.Array.Empty<object>(), a0),
                    2 => Closure.InvokeWithArgs2(del, System.Array.Empty<object>(), a0, a1),
                    3 => Closure.InvokeWithArgs3(del, System.Array.Empty<object>(), a0, a1, a2),
                    _ => Closure.InvokeWithArgs(del, System.Array.Empty<object>(), new object?[] { a0, a1, a2, a3 })
                };
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
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
                            values.Add(typedArray.ReadElementValue(i));
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

                case IEnumerable enumerable when source is not string && !TryGetArrayLikeLength(source, out _):
                    {
                        var values = new List<object?>();
                        foreach (var item in enumerable)
                        {
                            values.Add(item);
                        }

                        return values;
                    }
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

        private static List<double> CaptureSourceValues(object? source)
        {
            var items = CaptureSourceItems(source);
            var values = new List<double>(items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                values.Add(TypeUtilities.ToNumber(items[i]));
            }

            return values;
        }

        private static bool TryGetArrayLikeLength(object? source, out int length)
        {
            length = 0;
            if (source is null || source is JsNull || source is string)
            {
                return false;
            }

            var lengthValue = JavaScriptRuntime.Object.GetProperty(source, "length");
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

    internal sealed class TypedArrayIterator : IJavaScriptIterator
    {
        private readonly TypedArrayBase _typedArray;
        private readonly TypedArrayIteratorKind _kind;
        private int _index;

        public TypedArrayIterator(TypedArrayBase typedArray, TypedArrayIteratorKind kind)
        {
            _typedArray = typedArray;
            _kind = kind;
            JavaScriptRuntime.Iterator.InitializeIteratorSurface(this);
        }

        public IteratorResultObject Next()
        {
            if (_index >= _typedArray.length)
            {
                return IteratorResult.Create(null, true);
            }

            object? value = _kind switch
            {
                TypedArrayIteratorKind.Keys => (double)_index,
                TypedArrayIteratorKind.Entries => new JavaScriptRuntime.Array(new object?[] { (double)_index, _typedArray[(double)_index] }),
                _ => _typedArray[(double)_index]
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
