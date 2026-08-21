using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Array", IntrinsicCallKind.ArrayConstruct)]
    public class Array : JsObject, IExoticJsObject, IJavaScriptArray, IDictionary<string, object?>
    {
        [ThreadStatic]
        private static bool _defaultPrototypeChainHasBlockingIndexedProperties;
        [ThreadStatic]
        private static long _observedPrototypeMutationVersion;
        [ThreadStatic]
        private static long _observedPrototypeIntrinsicsId;
        private static long _prototypeMutationVersion;

        /// <summary>
        /// This realm's immutable <c>%Array.prototype%</c> template. Realm-owned
        /// (issue #1824); <see cref="Prototype"/> is the mutable copy handed to
        /// running code.
        /// </summary>
        internal static JsObject ImmutablePrototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.ArrayImmutablePrototype,
                static () => new JsObject(),
                static prototype =>
                {
                    using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

                    // Wired here rather than only from GlobalThis's bootstrap so the
                    // template is complete no matter which realm-owned intrinsic is
                    // materialized first: Prototype copies this object, and a copy taken
                    // before the [[Prototype]] link and "constructor" existed would lose
                    // Object.prototype's methods and Array.prototype.constructor.
                    PrototypeChain.SetPrototype(prototype, GlobalThis.ObjectPrototypeValue);
                    ConfigurePrototype(prototype);
                    PropertyDescriptorStore.DefineOrUpdate(prototype, "constructor", new JsPropertyDescriptor
                    {
                        Kind = JsPropertyDescriptorKind.Data,
                        Enumerable = false,
                        Configurable = true,
                        Writable = true,
                        Value = GlobalThis.Array
                    });
                });
        private static readonly object Hole = new();
        private const int MaxDenseGap = 1024;
        private const int MinNumericStorageCapacity = 32;
        private const int MaxInitialDenseCapacity = 65536;
        private const int CapacityHintMarker = 1 << 30;
        private const int CapacityHintMask = CapacityHintMarker - 1;
        private List<object?>? _items;
        private List<double>? _numberItems;
        private int _logicalLength;
        private int _holeCount;
        private double _virtualLength;

        private int CapacityHint
            => (_holeCount & CapacityHintMarker) != 0
                ? _holeCount & CapacityHintMask
                : 0;

        private void SetCapacityHint(int capacity)
        {
            var boundedCapacity = global::System.Math.Min(capacity, MaxInitialDenseCapacity);
            if (boundedCapacity > 0)
            {
                _holeCount |= CapacityHintMarker | boundedCapacity;
            }
        }

        private void ClearCapacityHint()
            => _holeCount &= int.MinValue;

        /// <summary>
        /// This realm's live <c>Array.prototype</c>. Realm-owned (issue #1824): it used
        /// to be a per-thread copy, which meant two realms sharing a thread also shared
        /// every <c>Array.prototype</c> mutation.
        /// </summary>
        internal static JsObject Prototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.ArrayPrototype,
                static () => new JsObject(),
                static prototype =>
                {
                    CopyImmutablePrototypeSurface(prototype);
                    RefreshDefaultPrototypeChainState();
                });

        internal static void ResetPrototypeForTests()
        {
            _defaultPrototypeChainHasBlockingIndexedProperties = false;
            _observedPrototypeIntrinsicsId = 0;
            _observedPrototypeMutationVersion = Volatile.Read(ref _prototypeMutationVersion);
        }

        private static void CopyImmutablePrototypeSurface(JsObject prototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            PropertyDescriptorStore.CopyOwnProperties(ImmutablePrototype, prototype);

            if (PrototypeChain.TryGetPrototype(ImmutablePrototype, out var parentPrototype))
            {
                PrototypeChain.InitializePrototype(prototype, parentPrototype);
            }
        }

        private static void ConfigurePrototype(JsObject prototype)
        {
            DefinePrototypeMethod(prototype, "join", (BuiltinFunction1)PrototypeJoin, 1);
            DefinePrototypeMethod(prototype, "toString", (BuiltinFunction0)PrototypeToString, 0);
            DefinePrototypeMethod(prototype, "push", (BuiltinFunctionVariadic)PrototypePush, 1);
            DefinePrototypeMethod(prototype, "reduce", (BuiltinFunctionVariadic)PrototypeReduce, 1);
            DefinePrototypeMethod(prototype, "reduceRight", (BuiltinFunctionVariadic)PrototypeReduceRight, 1);
            DefinePrototypeMethod(prototype, "indexOf", (BuiltinFunction2)PrototypeIndexOf, 1);
            DefinePrototypeMethod(prototype, "every", (BuiltinFunction2)PrototypeEvery, 1);
            DefinePrototypeMethod(prototype, "some", (BuiltinFunction2)PrototypeSome, 1);
            DefinePrototypeMethod(prototype, "filter", (BuiltinFunction2)PrototypeFilter, 1);
            DefinePrototypeMethod(prototype, "map", (BuiltinFunction2)PrototypeMap, 1);
            DefinePrototypeMethod(prototype, "find", (BuiltinFunction2)PrototypeFind, 1);
            DefinePrototypeMethod(prototype, "findIndex", (BuiltinFunction2)PrototypeFindIndex, 1);
            DefinePrototypeMethod(prototype, "includes", (BuiltinFunction2)PrototypeIncludes, 1);
            DefinePrototypeMethod(prototype, "findLast", (BuiltinFunction2)PrototypeFindLast, 1);
            DefinePrototypeMethod(prototype, "findLastIndex", (BuiltinFunction2)PrototypeFindLastIndex, 1);
            DefinePrototypeMethod(prototype, "flat", (BuiltinFunction1)PrototypeFlat, 0);
            DefinePrototypeMethod(prototype, "at", (BuiltinFunction1)PrototypeAt, 1);
            DefinePrototypeMethod(prototype, "toSorted", (BuiltinFunction1)PrototypeToSorted, 1);
            DefinePrototypeMethod(prototype, "with", (BuiltinFunction2)PrototypeWith, 2);
            DefinePrototypeMethod(prototype, "entries", (BuiltinFunction0)PrototypeEntries, 0);
            DefinePrototypeMethod(prototype, "keys", (BuiltinFunction0)PrototypeKeys, 0);
            var prototypeValues =
                DefinePrototypeMethod(prototype, "values", (BuiltinFunction0)PrototypeValues, 0);
            PropertyDescriptorStore.DefineOrUpdate(prototype, Symbol.iterator.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = prototypeValues
            });
            PropertyDescriptorStore.DefineOrUpdate(prototype, Symbol.unscopables.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = CreateArrayPrototypeUnscopables()
            });
        }

        private static JsObject CreateArrayPrototypeUnscopables()
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            var unscopables = new JsObject();
            PrototypeChain.SetPrototype(unscopables, JsNull.Null);
            DefineUnscopable(unscopables, "copyWithin");
            DefineUnscopable(unscopables, "entries");
            DefineUnscopable(unscopables, "fill");
            DefineUnscopable(unscopables, "find");
            DefineUnscopable(unscopables, "findIndex");
            DefineUnscopable(unscopables, "findLast");
            DefineUnscopable(unscopables, "findLastIndex");
            DefineUnscopable(unscopables, "flat");
            DefineUnscopable(unscopables, "flatMap");
            DefineUnscopable(unscopables, "includes");
            DefineUnscopable(unscopables, "keys");
            DefineUnscopable(unscopables, "values");
            DefineUnscopable(unscopables, "at");
            DefineUnscopable(unscopables, "toReversed");
            DefineUnscopable(unscopables, "toSorted");
            DefineUnscopable(unscopables, "toSpliced");
            return unscopables;
        }

        private static void DefineUnscopable(JsObject unscopables, string propertyName)
        {
            PropertyDescriptorStore.DefineOrUpdate(unscopables, propertyName, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = true,
                Configurable = true,
                Writable = true,
                Value = true
            });
        }

        private static object? DefinePrototypeMethod(JsObject prototype, string name, Delegate method, double length)
        {
            var value =
                BuiltinDelegateFunctionAdapter.WrapJavaScriptVisibleValue(
                    method);
            if (value is BuiltinDelegateFunctionAdapter builtinFunction)
            {
                JavaScriptRuntime.Function.InitializeFunctionInstance(
                    builtinFunction,
                    length,
                    name,
                    requiresInvocationContext:
                        !BuiltinFunctionDelegates.IsReceiverAware(
                            builtinFunction.Target));
                PropertyDescriptorStore.DefineOrUpdate(builtinFunction, "prototype", new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Enumerable = false,
                    Configurable = false,
                    Writable = false,
                    Value = null
                });
            }

            PropertyDescriptorStore.DefineOrUpdate(prototype, name, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = value
            });

            return value;
        }

        public bool hasOwnProperty(object? prop)
            => ObjectRuntime.hasOwn(this, prop);

        private int DenseCount => _numberItems?.Count ?? _items?.Count ?? 0;
        private int LogicalCount => _logicalLength > DenseCount ? _logicalLength : DenseCount;

        private void EnsureDenseStorage(int minCount)
        {
            if (DenseCount >= minCount)
            {
                return;
            }

            EnsureObjectStorage(minCount);
            _items!.EnsureCapacity(minCount);

            while (_items.Count < minCount)
            {
                _items.Add(Hole);
                _holeCount++;
            }
        }

        private void EnsureObjectStorage(int minCapacity = 0)
        {
            if (_items is not null)
            {
                if (minCapacity > 0)
                {
                    _items.EnsureCapacity(minCapacity);
                }
                return;
            }

            var numberItems = _numberItems;
            var capacity = global::System.Math.Max(
                global::System.Math.Max(CapacityHint, minCapacity),
                numberItems?.Count ?? 0);
            _items = new List<object?>(capacity);
            ClearCapacityHint();
            if (numberItems is not null)
            {
                foreach (var number in numberItems)
                {
                    _items.Add(number);
                }
                _numberItems = null;
            }
        }

        private List<double> GetOrCreateNumberStorage()
        {
            if (_numberItems is null)
            {
                var capacity = CapacityHint;
                _numberItems = new List<double>(capacity);
                ClearCapacityHint();
            }
            return _numberItems;
        }

        private bool CanStoreNumbersUnboxed
            => _numberItems is not null
                || (_items is null && CapacityHint >= MinNumericStorageCapacity);

        private object? GetDenseValue(int index)
        {
            EnsureObjectStorage();
            return _items![index];
        }

        private void SetDenseValue(int index, object? value)
        {
            EnsureObjectStorage();
            _items![index] = value;
        }

        private void SetDenseNumber(int index, double value)
        {
            if (!CanStoreNumbersUnboxed)
            {
                EnsureObjectStorage();
                _items![index] = value;
                return;
            }

            GetOrCreateNumberStorage()[index] = value;
        }

        private void AddDenseValue(object? value)
        {
            EnsureObjectStorage(DenseCount + 1);
            _items!.Add(value);
        }

        public void AddNumber(double value)
        {
            EnsureDenseStorage(_logicalLength);
            if (CanStoreNumbersUnboxed)
            {
                GetOrCreateNumberStorage().Add(value);
            }
            else
            {
                EnsureObjectStorage(DenseCount + 1);
                _items!.Add(value);
            }
            SynchronizeDenseLengthAfterGrowth();
        }

        private void InsertDenseValue(int index, object? value)
        {
            EnsureObjectStorage(DenseCount + 1);
            _items!.Insert(index, value);
        }

        private void RemoveDenseRange(int index, int count)
        {
            if (count == 0)
            {
                return;
            }

            if (_numberItems is not null)
            {
                _numberItems.RemoveRange(index, count);
            }
            else
            {
                _items!.RemoveRange(index, count);
            }
        }

        private void ReverseDense()
        {
            if (_numberItems is not null)
            {
                _numberItems.Reverse();
            }
            else
            {
                _items?.Reverse();
            }
        }

        private void SynchronizeDenseLengthAfterGrowth()
        {
            _logicalLength = DenseCount;
            _virtualLength = global::System.Math.Max(_virtualLength, _logicalLength);
        }

        private static object PrototypeJoin(object? thisArgument, object? separator)
        {
            if (thisArgument is not JavaScriptRuntime.Array jsArray)
            {
                throw new TypeError("Array.prototype.join called on non-array");
            }

            // A missing separator and an explicit `undefined` separator are
            // indistinguishable once bound to a fixed-arity parameter; both take
            // the default-separator path here (matching real engines).
            return separator is null
                ? jsArray.join(System.Array.Empty<object>())
                : jsArray.join(new object[] { separator });
        }

        private static object PrototypeToString(object? thisArgument)
        {
            if (thisArgument is not JavaScriptRuntime.Array jsArray)
            {
                throw new TypeError("Array.prototype.toString called on non-array");
            }

            return jsArray.toString();
        }

        private static object PrototypePush(object? thisArgument, in JsCallArguments arguments)
        {
            if (thisArgument is not JavaScriptRuntime.Array jsArray)
            {
                throw new TypeError("Array.prototype.push called on non-array");
            }

            return arguments.Count switch
            {
                0 => jsArray.push(),
                1 => jsArray.push(arguments.GetArgument(0)),
                _ => jsArray.push(ToNonNullableObjectArray(arguments.ToArray())!)
            };
        }

        private static object? PrototypeReduce(object? thisArgument, in JsCallArguments arguments)
        {
            if (thisArgument is null || thisArgument is JsNull)
            {
                if (!string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("JROC_DIAG_REDUCE")))
                {
                    System.Console.Error.WriteLine("[JROC_DIAG_REDUCE] Array.prototype.reduce called with null/undefined receiver");
                    System.Console.Error.WriteLine(new System.Diagnostics.StackTrace(true).ToString());
                }
                throw new TypeError("Reduce called on null or undefined");
            }

            // Fast path: true JS array instance.
            if (thisArgument is JavaScriptRuntime.Array jsArray)
            {
                return arguments.Count == 0
                    ? jsArray.reduce(System.Array.Empty<object>())
                    : jsArray.reduce(ToNonNullableObjectArray(arguments.ToArray())!);
            }

            // Generic array-like path: supports NodeList and other array-like objects.
            // This is needed because real-world libs often use:
            //   const reduce = Array.prototype.reduce; reduce.call(arrayLike, cb, init)
            var iterationReceiver = GetArrayMethodIterationReceiver(thisArgument);
            var callbackReceiver = GetArrayMethodCallbackReceiver(thisArgument);
            int length = ToArrayLikeLength(iterationReceiver);
            var callback = RequireCallback(arguments.GetArgument(0), "reduce");
            int k;
            object? accumulator;
            if (arguments.Count >= 2)
            {
                accumulator = arguments.GetArgument(1);
                k = 0;
            }
            else
            {
                // No initial value: find the first present element and use it as the accumulator.
                bool found = false;
                accumulator = null;
                k = 0;
                for (int i = 0; i < length; i++)
                {
                    if (JavaScriptRuntime.ObjectRuntime.HasPropertyForArrayLike((double)i, iterationReceiver))
                    {
                        accumulator = JavaScriptRuntime.ObjectRuntime.GetItem(iterationReceiver, (double)i);
                        k = i + 1;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new TypeError("Reduce of empty array with no initial value");
                }
            }

            for (int i = k; i < length; i++)
            {
                if (!JavaScriptRuntime.ObjectRuntime.HasPropertyForArrayLike((double)i, iterationReceiver))
                {
                    continue;
                }
                var current = JavaScriptRuntime.ObjectRuntime.GetItem(iterationReceiver, (double)i);
                accumulator = InvokeArrayCallback(
                    callback,
                    null,
                    "Array.prototype.reduce",
                    4,
                    accumulator,
                    current,
                    (double)i,
                    callbackReceiver);
            }

            return accumulator;
        }

        private static object? PrototypeReduceRight(object? thisArgument, in JsCallArguments arguments)
        {
            if (thisArgument is null || thisArgument is JsNull)
            {
                if (!string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("JROC_DIAG_REDUCE")))
                {
                    System.Console.Error.WriteLine("[JROC_DIAG_REDUCE] Array.prototype.reduceRight called with null/undefined receiver");
                    System.Console.Error.WriteLine(new System.Diagnostics.StackTrace(true).ToString());
                }
                throw new TypeError("Reduce called on null or undefined");
            }

            if (thisArgument is JavaScriptRuntime.Array jsArray)
            {
                return arguments.Count == 0
                    ? jsArray.reduceRight(System.Array.Empty<object>())
                    : jsArray.reduceRight(ToNonNullableObjectArray(arguments.ToArray())!);
            }

            var iterationReceiver = GetArrayMethodIterationReceiver(thisArgument);
            var callbackReceiver = GetArrayMethodCallbackReceiver(thisArgument);
            int length = ToArrayLikeLength(iterationReceiver);
            var callback = RequireCallback(arguments.GetArgument(0), "reduceRight");
            int k;
            object? accumulator;
            if (arguments.Count >= 2)
            {
                accumulator = arguments.GetArgument(1);
                k = length - 1;
            }
            else
            {
                // No initial value: find the last present element and use it as the accumulator.
                bool found = false;
                accumulator = null;
                k = length - 1;
                for (int i = length - 1; i >= 0; i--)
                {
                    if (JavaScriptRuntime.ObjectRuntime.HasPropertyForArrayLike((double)i, iterationReceiver))
                    {
                        accumulator = JavaScriptRuntime.ObjectRuntime.GetItem(iterationReceiver, (double)i);
                        k = i - 1;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new TypeError("Reduce of empty array with no initial value");
                }
            }

            for (int i = k; i >= 0; i--)
            {
                if (!JavaScriptRuntime.ObjectRuntime.HasPropertyForArrayLike((double)i, iterationReceiver))
                {
                    continue;
                }
                var current = JavaScriptRuntime.ObjectRuntime.GetItem(iterationReceiver, (double)i);
                accumulator = InvokeArrayCallback(
                    callback,
                    null,
                    "Array.prototype.reduceRight",
                    4,
                    accumulator,
                    current,
                    (double)i,
                    callbackReceiver);
            }

            return accumulator;
        }

        private static object? PrototypeIndexOf(object? thisArgument, object? searchElement, object? fromIndex)
        {
            if (thisArgument is null || thisArgument is JsNull)
            {
                throw new TypeError("Array.prototype.indexOf called on null or undefined");
            }

            // Fast path for real JS array. A missing fromIndex and an explicit
            // `undefined` fromIndex are indistinguishable once bound to a
            // fixed-arity parameter; ToInt(null, 0) below already treats both
            // the same as omitted (default 0), so no special-casing is needed.
            if (thisArgument is JavaScriptRuntime.Array jsArray)
            {
                return jsArray.indexOf(new object[] { searchElement!, fromIndex! });
            }

            // Generic array-like indexOf
            int length = ToArrayLikeLength(thisArgument);
            double fromIndexNum;
            try { fromIndexNum = TypeUtilities.ToNumber(fromIndex); }
            catch { fromIndexNum = double.NaN; }
            if (double.IsNaN(fromIndexNum) || double.IsNegativeInfinity(fromIndexNum))
            {
                fromIndexNum = 0;
            }
            else if (double.IsPositiveInfinity(fromIndexNum))
            {
                // +Infinity means start at/after the end.
                fromIndexNum = length;
            }
            else
            {
                fromIndexNum = global::System.Math.Truncate(fromIndexNum);
            }

            int k;
            if (fromIndexNum >= length)
            {
                k = length;
            }
            else if (fromIndexNum >= 0)
            {
                k = (int)fromIndexNum;
            }
            else
            {
                double start = length + fromIndexNum;
                if (double.IsNaN(start) || start <= 0)
                {
                    k = 0;
                }
                else if (start >= length)
                {
                    k = length;
                }
                else
                {
                    k = (int)start;
                }
            }

            for (int i = k; i < length; i++)
            {
                var element = JavaScriptRuntime.ObjectRuntime.GetItem(thisArgument, (double)i);
                if (JavaScriptRuntime.Operators.StrictEqual(element, searchElement))
                {
                    return (double)i;
                }
            }

            return -1d;
        }

        private static object? PrototypeEvery(object? thisArgument, object? callback, object? thisArg)
        {
            var receiver = RequireArrayLikeReceiver(thisArgument, "every");
            var iterationReceiver = GetArrayMethodIterationReceiver(receiver);
            var callbackReceiver = GetArrayMethodCallbackReceiver(receiver);
            int length = ToArrayLikeLength(iterationReceiver);
            callback = RequireCallback(callback, "every");

            for (int i = 0; i < length; i++)
            {
                if (!JavaScriptRuntime.ObjectRuntime.HasPropertyForArrayLike((double)i, iterationReceiver))
                {
                    continue;
                }

                var value = JavaScriptRuntime.ObjectRuntime.GetItem(iterationReceiver, (double)i);
                var result = InvokeArrayCallback(callback, thisArg, "Array.prototype.every", 3, value, (double)i, callbackReceiver, null);
                if (!JavaScriptRuntime.Operators.IsTruthy(result))
                {
                    return false;
                }
            }

            return true;
        }

        private static object? PrototypeFilter(object? thisArgument, object? callback, object? thisArg)
        {
            var receiver = RequireArrayLikeReceiver(thisArgument, "filter");
            var iterationReceiver = GetArrayMethodIterationReceiver(receiver);
            var callbackReceiver = GetArrayMethodCallbackReceiver(receiver);
            int length = ToArrayLikeLength(iterationReceiver);
            callback = RequireCallback(callback, "filter");
            var result = new Array();

            for (int i = 0; i < length; i++)
            {
                if (!JavaScriptRuntime.ObjectRuntime.HasPropertyForArrayLike((double)i, iterationReceiver))
                {
                    continue;
                }

                var value = JavaScriptRuntime.ObjectRuntime.GetItem(iterationReceiver, (double)i);
                var keep = InvokeArrayCallback(callback, thisArg, "Array.prototype.filter", 3, value, (double)i, callbackReceiver, null);
                if (JavaScriptRuntime.Operators.IsTruthy(keep))
                {
                    result.Add(value);
                }
            }

            return result;
        }

        private static object? PrototypeSome(object? thisArgument, object? callback, object? thisArg)
        {
            var receiver = RequireArrayLikeReceiver(thisArgument, "some");
            var iterationReceiver = GetArrayMethodIterationReceiver(receiver);
            var callbackReceiver = GetArrayMethodCallbackReceiver(receiver);
            int length = ToArrayLikeLength(iterationReceiver);
            callback = RequireCallback(callback, "some");

            for (int i = 0; i < length; i++)
            {
                if (!JavaScriptRuntime.ObjectRuntime.HasPropertyForArrayLike((double)i, iterationReceiver))
                {
                    continue;
                }

                var value = JavaScriptRuntime.ObjectRuntime.GetItem(iterationReceiver, (double)i);
                var result = InvokeArrayCallback(callback, thisArg, "Array.prototype.some", 3, value, (double)i, callbackReceiver, null);
                if (JavaScriptRuntime.Operators.IsTruthy(result))
                {
                    return true;
                }
            }

            return false;
        }

        private static object? PrototypeFind(object? thisArgument, object? callback, object? thisArg)
        {
            var receiver = RequireArrayLikeReceiver(thisArgument, "find");
            var iterationReceiver = GetArrayMethodIterationReceiver(receiver);
            var callbackReceiver = GetArrayMethodCallbackReceiver(receiver);
            int length = ToArrayLikeLength(iterationReceiver);
            callback = RequireCallback(callback, "find");

            for (int i = 0; i < length; i++)
            {
                var value = JavaScriptRuntime.ObjectRuntime.GetItem(iterationReceiver, (double)i);
                var result = InvokeArrayCallback(callback, thisArg, "Array.prototype.find", 3, value, (double)i, callbackReceiver, null);
                if (JavaScriptRuntime.Operators.IsTruthy(result))
                {
                    return value;
                }
            }

            return null;
        }

        private static object? PrototypeFindIndex(object? thisArgument, object? callback, object? thisArg)
        {
            var receiver = RequireArrayLikeReceiver(thisArgument, "findIndex");
            var iterationReceiver = GetArrayMethodIterationReceiver(receiver);
            var callbackReceiver = GetArrayMethodCallbackReceiver(receiver);
            int length = ToArrayLikeLength(iterationReceiver);
            callback = RequireCallback(callback, "findIndex");

            for (int i = 0; i < length; i++)
            {
                var value = JavaScriptRuntime.ObjectRuntime.GetItem(iterationReceiver, (double)i);
                var result = InvokeArrayCallback(callback, thisArg, "Array.prototype.findIndex", 3, value, (double)i, callbackReceiver, null);
                if (JavaScriptRuntime.Operators.IsTruthy(result))
                {
                    return (double)i;
                }
            }

            return -1d;
        }

        private static object? PrototypeIncludes(object? thisArgument, object? searchElement, object? fromIndex)
        {
            var receiver = RequireArrayLikeReceiver(thisArgument, "includes");
            var iterationReceiver = GetArrayMethodIterationReceiver(receiver);
            var length = ToArrayLikeLengthAsDouble(iterationReceiver);
            if (length <= 0d)
            {
                return false;
            }

            var startIndex = CoerceArrayLikeSearchStartIndex(fromIndex, length);

            for (var index = startIndex; index < length; index++)
            {
                if (SameValueZero(JavaScriptRuntime.ObjectRuntime.GetItem(iterationReceiver, index), searchElement))
                {
                    return true;
                }
            }

            return false;
        }

        private static object? PrototypeFindLast(object? thisArgument, object? callback, object? thisArg)
            => FindFromLast(RequireArrayLikeReceiver(thisArgument, "findLast"), callback, thisArg, returnIndex: false);

        private static object? PrototypeFindLastIndex(object? thisArgument, object? callback, object? thisArg)
            => FindFromLast(RequireArrayLikeReceiver(thisArgument, "findLastIndex"), callback, thisArg, returnIndex: true);

        private static object? FindFromLast(object receiver, object? callback, object? thisArg, bool returnIndex)
        {
            var iterationReceiver = GetArrayMethodIterationReceiver(receiver);
            var callbackReceiver = GetArrayMethodCallbackReceiver(receiver);
            var length = ToArrayLikeLengthAsDouble(iterationReceiver);
            callback = RequireCallback(callback, returnIndex ? "findLastIndex" : "findLast");

            for (var index = length - 1d; index >= 0d; index--)
            {
                var value = JavaScriptRuntime.ObjectRuntime.GetItem(iterationReceiver, index);
                var result = JavaScriptRuntime.Function.Call(
                    callback,
                    thisArg,
                    new object?[] { value, index, callbackReceiver });

                if (JavaScriptRuntime.Operators.IsTruthy(result))
                {
                    return returnIndex ? index : value;
                }
            }

            return returnIndex ? -1d : null;
        }

        // Legacy args-array overload retained for the compiler's direct
        // instance-method dispatch path (findLast(object[]), findLastIndex(object[])),
        // which is out of scope for the explicit-receiver ABI migration.
        private static object? FindFromLast(object receiver, object?[]? args, bool returnIndex)
        {
            var callback = args is { Length: > 0 } ? args[0] : null;
            var thisArg = args is { Length: > 1 } ? args[1] : null;
            return FindFromLast(receiver, callback, thisArg, returnIndex);
        }

        private static object? PrototypeMap(object? thisArgument, object? callback, object? thisArg)
        {
            var receiver = RequireArrayLikeReceiver(thisArgument, "map");
            var iterationReceiver = GetArrayMethodIterationReceiver(receiver);
            var callbackReceiver = GetArrayMethodCallbackReceiver(receiver);
            int length = ToArrayLikeLength(iterationReceiver);
            callback = RequireCallback(callback, "map");
            var result = new Array
            {
                length = length
            };

            for (int i = 0; i < length; i++)
            {
                if (!JavaScriptRuntime.ObjectRuntime.HasPropertyForArrayLike((double)i, iterationReceiver))
                {
                    continue;
                }

                var value = JavaScriptRuntime.ObjectRuntime.GetItem(iterationReceiver, (double)i);
                result[i] = InvokeArrayCallback(callback, thisArg, "Array.prototype.map", 3, value, (double)i, callbackReceiver, null);
            }

            return result;
        }

        private static object? PrototypeAt(object? thisArgument, object? index)
        {
            if (thisArgument is null || thisArgument is JsNull)
            {
                throw new TypeError("Array.prototype.at called on null or undefined");
            }

            // Fast path for real JS array. A missing index and an explicit
            // `undefined` index are indistinguishable once bound to a
            // fixed-arity parameter; both resolve to relative index 0 here
            // (matching real engines), rather than the previous zero-arg
            // shortcut that always returned undefined.
            if (thisArgument is JavaScriptRuntime.Array jsArray)
            {
                return jsArray.at(index);
            }

            // Generic array-like at
            double relativeIndex = ToIntegerOrInfinityForAt(index);

            int length = ToArrayLikeLength(thisArgument);
            int arrayIndex;
            if (relativeIndex >= 0)
            {
                arrayIndex = (int)relativeIndex;
            }
            else
            {
                arrayIndex = length + (int)relativeIndex;
            }

            if (arrayIndex < 0 || arrayIndex >= length)
            {
                return null; // undefined
            }

            return JavaScriptRuntime.ObjectRuntime.GetItem(thisArgument, (double)arrayIndex);
        }

        private static object? PrototypeToSorted(object? thisArgument, object? compareFn)
            => ToSorted(thisArgument, new object?[] { compareFn });

        private static object? PrototypeWith(object? thisArgument, object? index, object? value)
            => With(thisArgument, new object?[] { index, value });

        private static object? PrototypeFlat(object? thisArgument, object? depthArgument)
        {
            var receiver = RequireArrayLikeReceiver(thisArgument, "flat");

            int depth = 1;
            if (depthArgument != null)
            {
                depth = ToInt(depthArgument, 0);
            }
            if (depth < 0)
            {
                depth = 0;
            }

            var result = new Array();
            FlattenIntoArrayLike(result, receiver, depth);
            return result;
        }

        private static object RequireArrayLikeReceiver(object? receiver, string methodName)
        {
            if (receiver is null || receiver is JsNull)
            {
                throw new TypeError($"Array.prototype.{methodName} called on null or undefined");
            }

            return receiver;
        }

        private static object RequireCallback(object? callback, string methodName)
        {
            if (callback is null || callback is JsNull)
            {
                throw new TypeError($"Array.prototype.{methodName} requires a callback function");
            }

            if (!CallableOperations.IsCallable(callback))
            {
                throw new TypeError("callback is not a function");
            }

            return callback;
        }

        private static object GetArrayMethodIterationReceiver(object receiver)
        {
            if (receiver is string)
            {
                return receiver;
            }

            if (TryGetStringObjectValue(receiver, out var stringValue))
            {
                return stringValue;
            }

            return GetArrayMethodCallbackReceiver(receiver);
        }

        private static object GetArrayMethodCallbackReceiver(object receiver)
        {
            if (receiver is bool boolean)
            {
                return new JavaScriptRuntime.Boolean(boolean);
            }

            if (receiver is string str)
            {
                return JavaScriptRuntime.String.Construct(new object?[] { str }, null);
            }

            if (receiver is System.Numerics.BigInteger)
            {
                return ObjectRuntime.Construct(receiver);
            }

            if (receiver is double or float or int or long or short or byte)
            {
                return JavaScriptRuntime.Number.Construct(new object?[] { receiver }, null);
            }

            return receiver;
        }

        private static object? InvokeArrayCallback(object? callback, object? thisArg, string callbackKind, int argCount, object? a0, object? a1, object? a2, object? a3)
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

        private static object[]? ToNonNullableObjectArray(object?[]? args)
        {
            if (args is null)
            {
                return null;
            }

            var converted = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                converted[i] = args[i]!;
            }

            return converted;
        }

        private static bool TryGetStringObjectValue(object receiver, out string value)
        {
            if (PropertyDescriptorStore.TryGetOwn(receiver, JavaScriptRuntime.String.StringDataPropertyName, out var descriptor)
                && descriptor.Kind == JsPropertyDescriptorKind.Data)
            {
                value = DotNet2JSConversions.ToString(descriptor.Value);
                return true;
            }

            value = string.Empty;
            return false;
        }

        private static int ToArrayLikeLength(object receiver)
        {
            var length = ToArrayLikeLengthAsDouble(receiver);
            return length > int.MaxValue
                ? int.MaxValue
                : (int)length;
        }

        private static double ToArrayLikeLengthAsDouble(object receiver)
        {
            var lengthValue = JavaScriptRuntime.ObjectRuntime.GetProperty(receiver, "length");
            var length = TypeUtilities.ToNumber(lengthValue);

            if (double.IsNaN(length) || length <= 0d)
            {
                return 0d;
            }

            if (double.IsPositiveInfinity(length))
            {
                return 9007199254740991d;
            }

            return global::System.Math.Min(global::System.Math.Truncate(length), 9007199254740991d);
        }

        private static double ToIntegerOrInfinity(object? value)
        {
            var number = TypeUtilities.ToNumber(value);
            if (double.IsNaN(number) || number == 0d)
            {
                return 0d;
            }

            if (double.IsInfinity(number))
            {
                return number;
            }

            return global::System.Math.Truncate(number);
        }

        private static double CoerceArrayLikeSearchStartIndex(object? value, double length)
        {
            var relativeIndex = ToIntegerOrInfinity(value);
            if (double.IsPositiveInfinity(relativeIndex))
            {
                return length;
            }

            if (double.IsNegativeInfinity(relativeIndex))
            {
                return 0d;
            }

            if (relativeIndex >= 0d)
            {
                return global::System.Math.Min(relativeIndex, length);
            }

            return global::System.Math.Max(length + relativeIndex, 0d);
        }

        private static object? PrototypeEntries(object? thisArgument)
        {
            return CreateIteratorFromReceiver(thisArgument, ArrayIteratorKind.Entries, "entries");
        }

        private static object? PrototypeKeys(object? thisArgument)
        {
            return CreateIteratorFromReceiver(thisArgument, ArrayIteratorKind.Keys, "keys");
        }

        private static object? PrototypeValues(object? thisArgument)
        {
            return CreateIteratorFromReceiver(thisArgument, ArrayIteratorKind.Values, "values");
        }

        private static IJavaScriptIterator CreateIteratorFromReceiver(object? receiver, ArrayIteratorKind kind, string methodName)
        {
            if (receiver is null || receiver is JsNull)
            {
                throw new TypeError($"Array.prototype.{methodName} called on null or undefined");
            }

            if (receiver is Array jsArray)
            {
                return new ArrayIterator(jsArray, () => jsArray.Count, kind);
            }

            return new ArrayIterator(receiver, () => ToArrayLikeLength(receiver), kind);
        }

        private void InitializeIntrinsicSurface()
        {
            PrototypeChain.InitializePrototype(this, Prototype);
        }

        private enum ArrayIteratorKind
        {
            Keys,
            Values,
            Entries
        }

        private sealed class ArrayIterator : IJavaScriptIterator
        {
            private readonly object _receiver;
            private readonly Func<int> _getLength;
            private readonly ArrayIteratorKind _kind;
            private int _index;
            private bool _isClosed;

            public ArrayIterator(object receiver, Func<int> getLength, ArrayIteratorKind kind)
            {
                _receiver = receiver;
                _getLength = getLength;
                _kind = kind;
                JavaScriptRuntime.Iterator.InitializeIteratorSurface(this);
            }

            public bool HasReturn => true;

            public IteratorResultObject Next()
            {
                if (_isClosed)
                {
                    return new IteratorResultObject(null, done: true);
                }

                int index = _index;
                if (index >= _getLength())
                {
                    return new IteratorResultObject(null, done: true);
                }

                _index++;
                object? value = _kind switch
                {
                    ArrayIteratorKind.Keys => (double)index,
                    ArrayIteratorKind.Values => JavaScriptRuntime.ObjectRuntime.GetItem(_receiver, (double)index),
                    ArrayIteratorKind.Entries => new Array(new object?[]
                    {
                        (double)index,
                        JavaScriptRuntime.ObjectRuntime.GetItem(_receiver, (double)index)
                    }),
                    _ => null
                };

                return new IteratorResultObject(value, done: false);
            }

            public object next(object? value = null)
                => Next();

            public void Return()
            {
                _isClosed = true;
            }
        }

        public Array()
        {
            _logicalLength = 0;
            _virtualLength = 0;
            InitializeIntrinsicSurface();
        }
        public Array(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            SetCapacityHint(capacity);
            _logicalLength = 0;
            _virtualLength = 0;
            InitializeIntrinsicSurface();
        }
        public Array(System.Collections.IEnumerable collection)
        {
            ArgumentNullException.ThrowIfNull(collection);
            SetCapacityHint(collection is ICollection sized ? sized.Count : 0);
            foreach (var item in collection)
            {
                AddDenseValue(item);
            }
            _logicalLength = DenseCount;
            _virtualLength = _logicalLength;
            InitializeIntrinsicSurface();
        }

        public new int Count => LogicalCount;

        // Keep the inherited dictionary surface as the named-property storage view,
        // but never let canonical index or length writes leak into shape slots.
        public override object? this[string key]
        {
            get
            {
                if (TryGetDictionaryValue(key, out var value))
                {
                    return value;
                }

                throw new KeyNotFoundException($"Key '{key}' not found.");
            }
            set => SetDictionaryValue(key, value);
        }

        public override void SetNumber(string key, double value)
            => ObjectRuntime.SetProperty(this, key, value);

        public override void SetBoolean(string key, bool value)
            => ObjectRuntime.SetProperty(this, key, value);

        public override void SetString(string key, string? value)
            => ObjectRuntime.SetProperty(this, key, value);

        public override void SetValue(string key, object? value)
            => ObjectRuntime.SetProperty(this, key, value);

        public override void Add(string key, object? value)
        {
            if (ContainsDictionaryKey(key))
            {
                throw new ArgumentException($"An item with the same key has already been added: {key}", nameof(key));
            }

            SetDictionaryValue(key, value);
        }

        public override bool ContainsKey(string key)
            => ContainsDictionaryKey(key);

        public override bool Remove(string key)
            => ContainsDictionaryKey(key) && DeleteOwnProperty(key);

        public override bool TryGetValue(string key, out object? value)
            => TryGetDictionaryValue(key, out value);

        public override bool Contains(KeyValuePair<string, object?> item)
            => TryGetDictionaryValue(item.Key, out var value)
                && EqualityComparer<object?>.Default.Equals(value, item.Value);

        object? IDictionary<string, object?>.this[string key]
        {
            get => this[key];
            set => this[key] = value;
        }

        void IDictionary<string, object?>.Add(string key, object? value)
            => Add(key, value);

        bool IDictionary<string, object?>.ContainsKey(string key)
            => ContainsDictionaryKey(key);

        bool IDictionary<string, object?>.Remove(string key)
            => Remove(key);

        bool IDictionary<string, object?>.TryGetValue(string key, out object? value)
            => TryGetDictionaryValue(key, out value);

        void ICollection<KeyValuePair<string, object?>>.Add(KeyValuePair<string, object?> item)
            => ((IDictionary<string, object?>)this).Add(item.Key, item.Value);

        void ICollection<KeyValuePair<string, object?>>.Clear()
        {
            foreach (var key in base.GetOwnPropertyNames().ToArray())
            {
                base.DeleteOwnProperty(key);
            }
        }

        bool ICollection<KeyValuePair<string, object?>>.Contains(KeyValuePair<string, object?> item)
            => Contains(item);

        bool ICollection<KeyValuePair<string, object?>>.Remove(KeyValuePair<string, object?> item)
            => ((ICollection<KeyValuePair<string, object?>>)this).Contains(item)
                && ((IDictionary<string, object?>)this).Remove(item.Key);

        private bool TryGetDictionaryValue(string key, out object? value)
        {
            if (!ContainsDictionaryKey(key))
            {
                value = null;
                return false;
            }

            value = ObjectRuntime.GetProperty(this, key);
            return true;
        }

        private bool ContainsDictionaryKey(string key)
            => GetOwnPropertyDescriptor(key, out _) == PropertyDescriptorLookup.Found;

        private void SetDictionaryValue(string key, object? value)
            => ObjectRuntime.SetProperty(this, key, value);

        internal override PropertyDescriptorLookup GetOwnPropertyDescriptor(
            string key,
            out JsPropertyDescriptor descriptor)
        {
            var lookup = PropertyDescriptorStore.GetOwnLookupCore(this, key, out descriptor);
            if (lookup == PropertyDescriptorLookup.Deleted)
            {
                return lookup;
            }

            if (string.Equals(key, "length", StringComparison.Ordinal))
            {
                if (lookup == PropertyDescriptorLookup.Found)
                {
                    descriptor = PropertyDescriptorStore.CloneDescriptor(descriptor);
                    descriptor.Value = length;
                    return lookup;
                }

                descriptor = CreateLengthDescriptor();
                return PropertyDescriptorLookup.Found;
            }

            if (lookup == PropertyDescriptorLookup.Found)
            {
                return lookup;
            }

            if (ObjectRuntime.TryParseCanonicalIndexString(key, out var index)
                && HasOwnIndex(index))
            {
                descriptor = CreateElementDescriptor(this[index]);
                return PropertyDescriptorLookup.Found;
            }

            if (base.TryGetOwnPropertyValue(key, out var value))
            {
                descriptor = CreateElementDescriptor(value);
                return PropertyDescriptorLookup.Found;
            }

            descriptor = default;
            return PropertyDescriptorLookup.None;
        }

        internal override bool TryGetOwnPropertyValue(string key, out object? value)
        {
            if (string.Equals(key, "length", StringComparison.Ordinal))
            {
                value = length;
                return true;
            }

            if (ObjectRuntime.TryParseCanonicalIndexString(key, out var index))
            {
                if (HasOwnIndex(index))
                {
                    value = this[index];
                    return true;
                }

                value = null;
                return false;
            }

            return base.TryGetOwnPropertyValue(key, out value);
        }

        internal override bool TryGetInvariantOwnPropertyValue(string key, out object? value)
        {
            if (string.Equals(key, "length", StringComparison.Ordinal))
            {
                value = length;
                return true;
            }

            value = null;
            return false;
        }

        internal override bool HasOwnPropertyValue(string key)
        {
            if (!HasNonDataDescriptors)
            {
                if (string.Equals(key, "length", StringComparison.Ordinal))
                {
                    return true;
                }

                if (ObjectRuntime.TryParseCanonicalIndexString(key, out var defaultIndex))
                {
                    return HasOwnIndex(defaultIndex);
                }

                return base.HasOwnPropertyValue(key);
            }

            var lookup = PropertyDescriptorStore.GetOwnLookupCore(this, key, out _);
            if (lookup != PropertyDescriptorLookup.None)
            {
                return lookup == PropertyDescriptorLookup.Found;
            }

            if (string.Equals(key, "length", StringComparison.Ordinal))
            {
                return true;
            }

            if (ObjectRuntime.TryParseCanonicalIndexString(key, out var index))
            {
                return HasOwnIndex(index);
            }

            return base.HasOwnPropertyValue(key);
        }

        internal override bool UsesInlineExoticDescriptorStorage(string key)
            => string.Equals(key, "length", StringComparison.Ordinal)
                || ObjectRuntime.TryParseCanonicalArrayIndexUInt(key, out _);

        internal override bool DefineOwnProperty(string key, JsPropertyDescriptor descriptor)
        {
            if (string.Equals(key, "length", StringComparison.Ordinal))
            {
                return DefineLengthProperty(descriptor);
            }

            if (ObjectRuntime.TryParseCanonicalArrayIndexUInt(key, out var index))
            {
                return DefineIndexProperty(key, index, descriptor);
            }

            return base.DefineOwnProperty(key, descriptor);
        }

        internal override bool SetOwnPropertyValue(string key, object? value)
        {
            if (string.Equals(key, "length", StringComparison.Ordinal))
            {
                return DefineLengthProperty(CreateLengthDescriptorWithValue(value));
            }

            if (ObjectRuntime.TryParseCanonicalIndexString(key, out var index))
            {
                return TrySetIndexValue(index, value, throwOnError: false);
            }

            if (ObjectRuntime.TryParseCanonicalArrayIndexUInt(key, out var largeIndex))
            {
                return DefineIndexProperty(key, largeIndex, CreateElementDescriptor(value));
            }

            return base.SetOwnPropertyValue(key, value);
        }

        internal override bool DeleteOwnProperty(string key)
        {
            if (string.Equals(key, "length", StringComparison.Ordinal))
            {
                return false;
            }

            if (GetOwnPropertyDescriptor(key, out var descriptor) == PropertyDescriptorLookup.Found
                && !descriptor.Configurable)
            {
                return false;
            }

            if (ObjectRuntime.TryParseCanonicalArrayIndexUInt(key, out var index))
            {
                var storedLookup = PropertyDescriptorStore.GetOwnLookupCore(this, key, out _);
                if (index <= int.MaxValue)
                {
                    DeleteOwnIndex((int)index);
                }

                if (storedLookup == PropertyDescriptorLookup.Found)
                {
                    PropertyDescriptorStore.Delete(this, key);
                }

                return true;
            }

            return base.DeleteOwnProperty(key);
        }

        internal override IEnumerable<string> GetOwnPropertyKeys()
        {
            var numericKeys = new SortedDictionary<uint, string>();
            var descriptorKeys = PropertyDescriptorStore.GetOwnKeys(this).ToArray();
            var backingKeys = base.GetOwnPropertyNames().ToArray();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            foreach (var index in GetOwnElementIndices())
            {
                var key = index.ToString(CultureInfo.InvariantCulture);
                numericKeys[(uint)index] = key;
            }

            foreach (var key in descriptorKeys.Concat(backingKeys))
            {
                if (ObjectRuntime.TryParseCanonicalArrayIndexUInt(key, out var index))
                {
                    numericKeys.TryAdd(index, key);
                }
            }

            foreach (var key in numericKeys.Values)
            {
                seen.Add(key);
                yield return key;
            }

            seen.Add("length");
            yield return "length";

            foreach (var key in descriptorKeys.Concat(backingKeys))
            {
                if (!ObjectRuntime.IsEncodedSymbolKey(key)
                    && !ObjectRuntime.TryParseCanonicalArrayIndexUInt(key, out _)
                    && seen.Add(key))
                {
                    yield return key;
                }
            }

            foreach (var key in descriptorKeys.Concat(backingKeys))
            {
                if (ObjectRuntime.IsEncodedSymbolKey(key) && seen.Add(key))
                {
                    yield return key;
                }
            }
        }

        private static JsPropertyDescriptor CreateElementDescriptor(object? value)
            => new()
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = value,
                Writable = true,
                Enumerable = true,
                Configurable = true
            };

        private JsPropertyDescriptor CreateLengthDescriptor()
            => new()
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = length,
                Writable = IsLengthWritable,
                Enumerable = false,
                Configurable = false
            };

        private JsPropertyDescriptor CreateLengthDescriptorWithValue(object? value)
        {
            var descriptor = CreateLengthDescriptor();
            descriptor.Value = value;
            return descriptor;
        }

        private bool IsLengthWritable
        {
            get
            {
                var lookup = PropertyDescriptorStore.GetOwnLookupCore(this, "length", out var descriptor);
                return lookup != PropertyDescriptorLookup.Found || descriptor.Writable;
            }
        }

        private bool DefineLengthProperty(JsPropertyDescriptor descriptor)
        {
            if (descriptor.Kind != JsPropertyDescriptorKind.Data
                || descriptor.Enumerable
                || descriptor.Configurable)
            {
                return false;
            }

            var newLength = ValidateLengthValue(descriptor.Value);
            var current = CreateLengthDescriptor();
            if (!IsCompatibleDescriptor(current, descriptor))
            {
                return false;
            }

            var oldLength = length;
            if (newLength >= oldLength)
            {
                if (newLength > oldLength && !current.Writable)
                {
                    return false;
                }

                SetLengthStorage(newLength);
                StoreLengthDescriptor(descriptor);
                return true;
            }

            if (!current.Writable)
            {
                return false;
            }

            var indicesToDelete = PropertyDescriptorStore.GetOwnKeys(this)
                .Select(key => ObjectRuntime.TryParseCanonicalArrayIndexUInt(key, out var index)
                    ? (IsIndex: true, Index: index, Key: key)
                    : (IsIndex: false, Index: 0u, Key: key))
                .Where(entry => entry.IsIndex && entry.Index >= newLength)
                .OrderByDescending(entry => entry.Index)
                .ToArray();

            foreach (var entry in indicesToDelete)
            {
                if (GetOwnPropertyDescriptor(entry.Key, out var elementDescriptor) == PropertyDescriptorLookup.Found
                    && !elementDescriptor.Configurable)
                {
                    SetLengthStorage((double)entry.Index + 1d);
                    var failedDescriptor = PropertyDescriptorStore.CloneDescriptor(descriptor);
                    failedDescriptor.Value = length;
                    StoreLengthDescriptor(failedDescriptor);
                    return false;
                }

                DeleteIndexStorage(entry.Key, entry.Index);
            }

            SetLengthStorage(newLength);
            StoreLengthDescriptor(descriptor);
            return true;
        }

        private bool DefineIndexProperty(string key, uint index, JsPropertyDescriptor descriptor)
        {
            var lookup = PropertyDescriptorStore.GetOwnLookupCore(this, key, out var storedDescriptor);
            var hasCurrent = lookup == PropertyDescriptorLookup.Found;
            var current = storedDescriptor;

            if (!hasCurrent && index <= int.MaxValue && HasDenseIndex((int)index))
            {
                current = CreateElementDescriptor(GetDenseValue((int)index));
                hasCurrent = true;
            }

            if (!hasCurrent && !ObjectRuntime.IsExtensibleInternal(this))
            {
                return false;
            }

            if (hasCurrent && !IsCompatibleDescriptor(current, descriptor))
            {
                return false;
            }

            var oldLength = length;
            if (index >= oldLength && !IsLengthWritable)
            {
                return false;
            }

            if (index <= int.MaxValue
                && descriptor.Kind == JsPropertyDescriptorKind.Data
                && IsDefaultElementDescriptor(descriptor)
                && CanStoreDenseIndex(index))
            {
                SetDenseIndex((int)index, descriptor.Value);
                if (lookup == PropertyDescriptorLookup.Found)
                {
                    PropertyDescriptorStore.Delete(this, key);
                }
            }
            else
            {
                if (index <= int.MaxValue)
                {
                    DeleteOwnIndex((int)index);
                }

                PropertyDescriptorStore.DefineOrUpdate(this, key, descriptor);
            }

            if (index >= oldLength)
            {
                SetLengthStorage((double)index + 1d);
                SynchronizeStoredLengthValue();
            }

            return true;
        }

        internal bool TrySetIndexValue(int index, object? value, bool throwOnError)
        {
            if (index < 0)
            {
                if (throwOnError)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return false;
            }

            var hasDenseIndex = HasDenseIndex(index);
            if (!HasNonDataDescriptors && hasDenseIndex)
            {
                SetDenseValue(index, value);
                return true;
            }

            if (CanAppendDenseIndexFast(index))
            {
                SetDenseIndex(index, value);
                return true;
            }

            if ((HasNonDataDescriptors || !hasDenseIndex)
                && PropertyDescriptorStore.HasAny(this))
            {
                var descriptorKey = index.ToString(CultureInfo.InvariantCulture);
                if (PropertyDescriptorStore.GetOwnLookupCore(this, descriptorKey, out _) != PropertyDescriptorLookup.None)
                {
                    ObjectRuntime.SetProperty(this, descriptorKey, value, throwOnError);
                    return true;
                }
            }

            if (HasDenseIndex(index))
            {
                SetDenseValue(index, value);
                return true;
            }

            var key = index.ToString(CultureInfo.InvariantCulture);
            if (ObjectRuntime.TrySetPropertyViaPrototypeOrThrow(this, key, value, throwOnError))
            {
                return true;
            }

            if (!ObjectRuntime.IsExtensibleInternal(this) || index >= length && !IsLengthWritable)
            {
                if (throwOnError)
                {
                    throw new TypeError($"Cannot add property '{key}' to array");
                }

                return false;
            }

            if (CanStoreDenseIndex((uint)index))
            {
                SetDenseIndex(index, value);
                return true;
            }

            var defined = DefineIndexProperty(key, (uint)index, CreateElementDescriptor(value));
            if (!defined && throwOnError)
            {
                throw new TypeError($"Cannot add property '{key}' to array");
            }

            return defined;
        }

        internal bool TrySetIndexNumber(int index, double value, bool throwOnError)
        {
            if (index < 0)
            {
                if (throwOnError)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return false;
            }

            var hasDenseIndex = HasDenseIndex(index);
            if (!HasNonDataDescriptors && hasDenseIndex)
            {
                SetDenseNumber(index, value);
                return true;
            }

            if (CanAppendDenseIndexFast(index))
            {
                SetDenseIndexNumber(index, value);
                return true;
            }

            if ((HasNonDataDescriptors || !hasDenseIndex)
                && PropertyDescriptorStore.HasAny(this))
            {
                var descriptorKey = index.ToString(CultureInfo.InvariantCulture);
                if (PropertyDescriptorStore.GetOwnLookupCore(this, descriptorKey, out _) != PropertyDescriptorLookup.None)
                {
                    ObjectRuntime.SetProperty(this, descriptorKey, value, throwOnError);
                    return true;
                }
            }

            if (HasDenseIndex(index))
            {
                SetDenseNumber(index, value);
                return true;
            }

            var key = index.ToString(CultureInfo.InvariantCulture);
            if (ObjectRuntime.TrySetPropertyViaPrototypeOrThrow(this, key, value, throwOnError))
            {
                return true;
            }

            if (!ObjectRuntime.IsExtensibleInternal(this) || index >= length && !IsLengthWritable)
            {
                if (throwOnError)
                {
                    throw new TypeError($"Cannot add property '{key}' to array");
                }

                return false;
            }

            if (CanStoreDenseIndex((uint)index))
            {
                SetDenseIndexNumber(index, value);
                return true;
            }

            var defined = DefineIndexProperty(key, (uint)index, CreateElementDescriptor(value));
            if (!defined && throwOnError)
            {
                throw new TypeError($"Cannot add property '{key}' to array");
            }

            return defined;
        }

        private static bool IsCompatibleDescriptor(
            JsPropertyDescriptor current,
            JsPropertyDescriptor descriptor)
        {
            if (current.Configurable)
            {
                return true;
            }

            if (descriptor.Configurable
                || descriptor.Enumerable != current.Enumerable
                || descriptor.Kind != current.Kind)
            {
                return false;
            }

            if (current.Kind == JsPropertyDescriptorKind.Accessor)
            {
                return ReferenceEquals(current.Get, descriptor.Get)
                    && ReferenceEquals(current.Set, descriptor.Set);
            }

            if (current.Writable)
            {
                return true;
            }

            return !descriptor.Writable
                && Operators.SameValue(current.Value, descriptor.Value);
        }

        internal static double ValidateLengthValue(object? value)
            => ValidateLengthValue(TypeUtilities.ToNumber(value));

        private static double ValidateLengthValue(double newLength)
        {
            if (double.IsNaN(newLength)
                || double.IsInfinity(newLength)
                || newLength < 0
                || newLength >= 4294967296d
                || global::System.Math.Truncate(newLength) != newLength)
            {
                throw new RangeError("Invalid array length");
            }

            return newLength;
        }

        private void StoreLengthDescriptor(JsPropertyDescriptor descriptor)
        {
            var storedDescriptor = PropertyDescriptorStore.CloneDescriptor(descriptor);
            storedDescriptor.Value = length;
            var hasStoredDescriptor = PropertyDescriptorStore.GetOwnLookupCore(this, "length", out _)
                == PropertyDescriptorLookup.Found;
            if (IsDefaultLengthDescriptor(storedDescriptor))
            {
                if (hasStoredDescriptor)
                {
                    PropertyDescriptorStore.Delete(this, "length");
                }
            }
            else
            {
                PropertyDescriptorStore.DefineOrUpdate(this, "length", storedDescriptor);
            }
        }

        private void SynchronizeStoredLengthValue()
        {
            if (PropertyDescriptorStore.GetOwnLookupCore(this, "length", out var descriptor)
                != PropertyDescriptorLookup.Found)
            {
                return;
            }

            descriptor = PropertyDescriptorStore.CloneDescriptor(descriptor);
            descriptor.Value = length;
            PropertyDescriptorStore.DefineOrUpdate(this, "length", descriptor);
        }

        private void DeleteIndexStorage(string key, uint index)
        {
            if (index <= int.MaxValue)
            {
                DeleteOwnIndex((int)index);
            }

            if (PropertyDescriptorStore.GetOwnLookupCore(this, key, out _)
                == PropertyDescriptorLookup.Found)
            {
                PropertyDescriptorStore.Delete(this, key);
            }
        }

        private void SetDenseIndex(int index, object? value)
        {
            if (index == int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            EnsureObjectStorage(index + 1);
            if (index < _items!.Count)
            {
                if (ReferenceEquals(_items[index], Hole))
                {
                    _holeCount--;
                }

                _items[index] = value;
            }
            else
            {
                EnsureDenseStorage(index + 1);
                _items[index] = value;
            }

            if (index >= _logicalLength)
            {
                _logicalLength = index + 1;
            }

            if (index + 1 > _virtualLength)
            {
                _virtualLength = index + 1;
            }
        }

        private void SetDenseIndexNumber(int index, double value)
        {
            if (index == int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (CanStoreNumbersUnboxed && index <= DenseCount)
            {
                var numbers = GetOrCreateNumberStorage();
                if (index < numbers.Count)
                {
                    numbers[index] = value;
                }
                else
                {
                    numbers.Add(value);
                }
            }
            else
            {
                EnsureObjectStorage(index + 1);
                if (index < _items!.Count)
                {
                    if (ReferenceEquals(_items[index], Hole))
                    {
                        _holeCount--;
                    }
                    _items[index] = value;
                }
                else
                {
                    EnsureDenseStorage(index + 1);
                    _items[index] = value;
                }
            }

            if (index >= _logicalLength)
            {
                _logicalLength = index + 1;
            }

            if (index + 1 > _virtualLength)
            {
                _virtualLength = index + 1;
            }
        }

        private bool CanStoreDenseIndex(uint index)
            => index < int.MaxValue
                && index <= (uint)DenseCount + MaxDenseGap;

        private void SetLengthStorage(double newLength)
        {
            if (newLength > int.MaxValue)
            {
                _virtualLength = newLength;
                return;
            }

            var newLengthInt = (int)newLength;
            if (newLengthInt < DenseCount)
            {
                if (_items is not null)
                {
                    for (var i = newLengthInt; i < _items.Count; i++)
                    {
                        if (ReferenceEquals(_items[i], Hole))
                        {
                            _holeCount--;
                        }
                    }
                }

                RemoveDenseRange(newLengthInt, DenseCount - newLengthInt);
            }

            _logicalLength = newLengthInt;
            _virtualLength = newLengthInt;
        }

        private static bool IsDefaultElementDescriptor(JsPropertyDescriptor descriptor)
            => descriptor.Kind == JsPropertyDescriptorKind.Data
                && descriptor.Writable
                && descriptor.Enumerable
                && descriptor.Configurable;

        private static bool IsDefaultLengthDescriptor(JsPropertyDescriptor descriptor)
            => descriptor.Kind == JsPropertyDescriptorKind.Data
                && descriptor.Writable
                && !descriptor.Enumerable
                && !descriptor.Configurable;

        private bool HasDenseIndex(int index)
            => index >= 0
                && index < DenseCount
                && (_numberItems is not null || !ReferenceEquals(_items![index], Hole));

        internal bool HasOwnIndex(int index)
        {
            if (index < 0)
            {
                return false;
            }

            var hasDenseIndex = HasDenseIndex(index);
            if (!HasNonDataDescriptors && hasDenseIndex)
            {
                return true;
            }

            if (PropertyDescriptorStore.HasAny(this))
            {
                var key = index.ToString(CultureInfo.InvariantCulture);
                var lookup = PropertyDescriptorStore.GetOwnLookupCore(this, key, out _);
                if (lookup != PropertyDescriptorLookup.None)
                {
                    return lookup == PropertyDescriptorLookup.Found;
                }
            }

            return hasDenseIndex;
        }

        internal IEnumerable<int> GetOwnElementIndices()
        {
            var upperBound = global::System.Math.Min(Count, DenseCount);
            for (int i = 0; i < upperBound; i++)
            {
                if (_numberItems is not null || !ReferenceEquals(_items![i], Hole))
                {
                    yield return i;
                }
            }
        }

        internal bool DeleteOwnIndex(int index)
        {
            if (!HasDenseIndex(index))
            {
                return false;
            }

            EnsureObjectStorage();
            _items![index] = Hole;
            _holeCount++;
            return true;
        }

        private bool CanUseDenseMutationFastPath()
            => (_holeCount & int.MaxValue) == 0
                && _logicalLength == DenseCount
                && _virtualLength == DenseCount
                && !HasNonDataDescriptors;

        private bool CanAppendDenseIndexFast(int index)
            => index == DenseCount
                && _holeCount >= 0
                && !HasNonDataDescriptors
                && !PropertyDescriptorStore.HasAny(this)
                && ObjectRuntime.IsExtensibleInternal(this)
                && (index < length || IsLengthWritable)
                && DefaultPrototypeChainAllowsDenseWrites();

        internal void DisableDenseGrowthFastPath()
            => _holeCount |= int.MinValue;

        private bool CanUseDenseGrowthFastPath()
        {
            if (!CanUseDenseMutationFastPath() || _holeCount < 0)
            {
                return false;
            }

            return DefaultPrototypeChainAllowsDenseWrites();
        }

        private static bool DefaultPrototypeChainAllowsDenseWrites()
        {
            var mutationVersion = Volatile.Read(ref _prototypeMutationVersion);
            if (_observedPrototypeMutationVersion != mutationVersion
                || _observedPrototypeIntrinsicsId != RuntimeIntrinsics.Current.Id)
            {
                RefreshDefaultPrototypeChainState();
            }

            return !_defaultPrototypeChainHasBlockingIndexedProperties;
        }

        internal static void NotifyPrototypeMutation()
            => Interlocked.Increment(ref _prototypeMutationVersion);

        private static void RefreshDefaultPrototypeChainState()
        {
            while (true)
            {
                var beforeScan = Volatile.Read(ref _prototypeMutationVersion);
                var hasBlockingIndexedProperties = DefaultPrototypeChainHasBlockingIndexedProperties();
                var afterScan = Volatile.Read(ref _prototypeMutationVersion);
                if (beforeScan == afterScan)
                {
                    _defaultPrototypeChainHasBlockingIndexedProperties = hasBlockingIndexedProperties;
                    _observedPrototypeMutationVersion = afterScan;
                    _observedPrototypeIntrinsicsId = RuntimeIntrinsics.Current.Id;
                    return;
                }
            }
        }

        private static bool DefaultPrototypeChainHasBlockingIndexedProperties()
        {
            object current = Prototype;
            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
            while (visited.Add(current))
            {
                foreach (var key in PropertyDescriptorStore.GetOwnKeys(current))
                {
                    if (ObjectRuntime.TryParseCanonicalArrayIndexUInt(key, out _)
                        && PropertyDescriptorStore.TryGetOwn(current, key, out var descriptor)
                        && (descriptor.Kind == JsPropertyDescriptorKind.Accessor || !descriptor.Writable))
                    {
                        return true;
                    }
                }

                if (!PrototypeChain.TryGetPrototype(current, out var prototype)
                    || prototype is null
                    || prototype is JsNull)
                {
                    break;
                }

                current = prototype;
            }

            return false;
        }

        private void SynchronizeDenseLength()
        {
            _logicalLength = DenseCount;
            _virtualLength = DenseCount;
        }

        public object? this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (!HasNonDataDescriptors && HasDenseIndex(index))
                {
                    return GetDenseValue(index);
                }

                if (HasNonDataDescriptors && PropertyDescriptorStore.HasAny(this))
                {
                    var key = index.ToString(CultureInfo.InvariantCulture);
                    var lookup = PropertyDescriptorStore.GetOwnLookupCore(this, key, out _);
                    if (lookup != PropertyDescriptorLookup.None)
                    {
                        return ObjectRuntime.GetProperty(this, key);
                    }
                }

                if (HasDenseIndex(index))
                {
                    return GetDenseValue(index);
                }

                return ObjectRuntime.GetProperty(this, index.ToString(CultureInfo.InvariantCulture));
            }
            set
            {
                if (!TrySetIndexValue(index, value, throwOnError: true))
                {
                    throw new TypeError($"Cannot assign to property '{index}' of array");
                }
            }
        }

        public void Add(object? item)
        {
            EnsureDenseStorage(_logicalLength);
            AddDenseValue(item);
            SynchronizeDenseLengthAfterGrowth();
        }

        public void AddRange(IEnumerable<object?> collection)
        {
            EnsureDenseStorage(_logicalLength);
            foreach (var item in collection)
            {
                AddDenseValue(item);
            }
            SynchronizeDenseLengthAfterGrowth();
        }

        public void Insert(int index, object? item)
        {
            var currentLength = Count;
            if (index < 0 || index > currentLength)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (CanUseDenseGrowthFastPath())
            {
                InsertDenseValue(index, item);
                SynchronizeDenseLength();
                return;
            }

            for (var source = currentLength - 1; source >= index; source--)
            {
                var target = source + 1;
                if (ObjectRuntime.HasPropertyForArrayLike((double)source, this))
                {
                    TrySetIndexValue(target, ObjectRuntime.GetItem(this, (double)source), throwOnError: true);
                }
                else
                {
                    DeleteIndexOrThrow(target);
                }
            }

            TrySetIndexValue(index, item, throwOnError: true);
            SetLength(currentLength + 1, throwOnError: true);
        }

        public void InsertRange(int index, IEnumerable<object?> collection)
        {
            var items = collection.ToList();
            if (items.Count == 0)
            {
                return;
            }

            var currentLength = Count;
            if (index < 0 || index > currentLength || items.Count > int.MaxValue - currentLength)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (CanUseDenseGrowthFastPath())
            {
                for (var i = 0; i < items.Count; i++)
                {
                    InsertDenseValue(index + i, items[i]);
                }
                SynchronizeDenseLength();
                return;
            }

            for (var source = currentLength - 1; source >= index; source--)
            {
                var target = source + items.Count;
                if (ObjectRuntime.HasPropertyForArrayLike((double)source, this))
                {
                    TrySetIndexValue(target, ObjectRuntime.GetItem(this, (double)source), throwOnError: true);
                }
                else
                {
                    DeleteIndexOrThrow(target);
                }
            }

            for (var i = 0; i < items.Count; i++)
            {
                TrySetIndexValue(index + i, items[i], throwOnError: true);
            }

            SetLength(currentLength + items.Count, throwOnError: true);
        }

        public void RemoveAt(int index)
        {
            var currentLength = Count;
            if (index < 0 || index >= currentLength)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (CanUseDenseMutationFastPath())
            {
                RemoveDenseRange(index, 1);
                SynchronizeDenseLength();
                return;
            }

            for (var target = index; target < currentLength - 1; target++)
            {
                var source = target + 1;
                if (ObjectRuntime.HasPropertyForArrayLike((double)source, this))
                {
                    TrySetIndexValue(target, ObjectRuntime.GetItem(this, (double)source), throwOnError: true);
                }
                else
                {
                    DeleteIndexOrThrow(target);
                }
            }

            DeleteIndexOrThrow(currentLength - 1);
            SetLength(currentLength - 1, throwOnError: true);
        }

        public void RemoveRange(int index, int count)
        {
            if (count < 0 || index < 0 || index + count > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (count == 0)
            {
                return;
            }

            var currentLength = Count;
            var newLength = currentLength - count;
            if (CanUseDenseMutationFastPath())
            {
                RemoveDenseRange(index, count);
                SynchronizeDenseLength();
                return;
            }

            for (var target = index; target < newLength; target++)
            {
                var source = target + count;
                if (ObjectRuntime.HasPropertyForArrayLike((double)source, this))
                {
                    TrySetIndexValue(target, ObjectRuntime.GetItem(this, (double)source), throwOnError: true);
                }
                else
                {
                    DeleteIndexOrThrow(target);
                }
            }

            for (var indexToDelete = currentLength - 1; indexToDelete >= newLength; indexToDelete--)
            {
                DeleteIndexOrThrow(indexToDelete);
            }

            SetLength(newLength, throwOnError: true);
        }

        public void Reverse()
        {
            if (CanUseDenseMutationFastPath())
            {
                ReverseDense();
                return;
            }

            var len = Count;
            var middle = len / 2;
            for (var lower = 0; lower < middle; lower++)
            {
                var upper = len - lower - 1;
                var lowerExists = ObjectRuntime.HasPropertyForArrayLike((double)lower, this);
                var upperExists = ObjectRuntime.HasPropertyForArrayLike((double)upper, this);
                var lowerValue = lowerExists ? ObjectRuntime.GetItem(this, (double)lower) : null;
                var upperValue = upperExists ? ObjectRuntime.GetItem(this, (double)upper) : null;

                if (lowerExists && upperExists)
                {
                    TrySetIndexValue(lower, upperValue, throwOnError: true);
                    TrySetIndexValue(upper, lowerValue, throwOnError: true);
                }
                else if (!lowerExists && upperExists)
                {
                    TrySetIndexValue(lower, upperValue, throwOnError: true);
                    DeleteIndexOrThrow(upper);
                }
                else if (lowerExists)
                {
                    DeleteIndexOrThrow(lower);
                    TrySetIndexValue(upper, lowerValue, throwOnError: true);
                }
            }
        }

        public void Sort(Comparison<object?> comparison)
        {
            var presentValues = new List<object?>();
            for (var i = 0; i < Count; i++)
            {
                if (ObjectRuntime.HasPropertyForArrayLike((double)i, this))
                {
                    presentValues.Add(ObjectRuntime.GetItem(this, (double)i));
                }
            }

            presentValues.Sort(comparison);
            for (var i = 0; i < Count; i++)
            {
                if (i < presentValues.Count)
                {
                    TrySetIndexValue(i, presentValues[i], throwOnError: true);
                }
                else
                {
                    DeleteIndexOrThrow(i);
                }
            }
        }

        public override void Clear()
        {
            SetLength(0, throwOnError: true);
        }

        private void DeleteIndexOrThrow(int index)
        {
            var key = index.ToString(CultureInfo.InvariantCulture);
            if (!DeleteOwnProperty(key))
            {
                throw new TypeError($"Cannot delete property '{key}' of array");
            }
        }

        public object?[] ToArray()
        {
            var result = new object?[Count];
            for (int i = 0; i < Count; i++)
            {
                result[i] = this[i];
            }

            return result;
        }

        public List<object?> ToList()
            => new(ToArray());

        public new IEnumerator<object?> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        public IJavaScriptIterator values()
            => new ArrayIterator(this, () => Count, ArrayIteratorKind.Values);

        public IJavaScriptIterator keys()
            => new ArrayIterator(this, () => Count, ArrayIteratorKind.Keys);

        public IJavaScriptIterator entries()
            => new ArrayIterator(this, () => Count, ArrayIteratorKind.Entries);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Numeric indexer overload to support compiler intrinsics.
        // Semantics intentionally match JavaScriptRuntime.ObjectRuntime.GetItem/SetItem for Array + numeric index:
        // - Out-of-bounds reads return undefined (null)
        // - Writes extend the array with undefined (null)
        // - Negative indices behave like properties (currently ignored for host safety)
        public object? this[double index]
        {
            get
            {
                var isDenseIndex = !double.IsNaN(index)
                    && !double.IsInfinity(index)
                    && index % 1.0 == 0.0
                    && index >= 0
                    && index <= int.MaxValue;
                if (!isDenseIndex)
                {
                    return JavaScriptRuntime.ObjectRuntime.GetProperty(this, DotNet2JSConversions.ToString(index));
                }

                int intIndex = (int)index;
                if (intIndex >= Count)
                {
                    return JavaScriptRuntime.ObjectRuntime.GetProperty(
                        this,
                        intIndex.ToString(CultureInfo.InvariantCulture));
                }

                return this[intIndex];
            }
            set
            {
                var isDenseIndex = !double.IsNaN(index)
                    && !double.IsInfinity(index)
                    && index % 1.0 == 0.0
                    && index >= 0
                    && index <= int.MaxValue;
                if (!isDenseIndex)
                {
                    JavaScriptRuntime.ObjectRuntime.SetProperty(this, DotNet2JSConversions.ToString(index), value);
                    return;
                }

                int intIndex = (int)index;
                this[intIndex] = value;
            }
        }

        public double GetItemAsNumber(double index)
        {
            var isDenseIndex = !double.IsNaN(index)
                && !double.IsInfinity(index)
                && index % 1.0 == 0.0
                && index >= 0
                && index <= int.MaxValue;
            if (!isDenseIndex)
            {
                return TypeUtilities.ToNumber(
                    JavaScriptRuntime.ObjectRuntime.GetProperty(this, DotNet2JSConversions.ToString(index)));
            }

            var intIndex = (int)index;
            if (intIndex >= Count)
            {
                return TypeUtilities.ToNumber(
                    JavaScriptRuntime.ObjectRuntime.GetProperty(
                        this,
                        intIndex.ToString(CultureInfo.InvariantCulture)));
            }

            var hasDenseIndex = HasDenseIndex(intIndex);
            if (hasDenseIndex && !HasNonDataDescriptors && _numberItems is not null)
            {
                return _numberItems[intIndex];
            }

            if (HasNonDataDescriptors && PropertyDescriptorStore.HasAny(this))
            {
                var key = intIndex.ToString(CultureInfo.InvariantCulture);
                if (PropertyDescriptorStore.GetOwnLookupCore(this, key, out _)
                    != PropertyDescriptorLookup.None)
                {
                    return TypeUtilities.ToNumber(ObjectRuntime.GetProperty(this, key));
                }
            }

            if (hasDenseIndex)
            {
                return _numberItems is not null
                    ? _numberItems[intIndex]
                    : TypeUtilities.ToNumber(_items![intIndex]);
            }

            return TypeUtilities.ToNumber(
                ObjectRuntime.GetProperty(this, intIndex.ToString(CultureInfo.InvariantCulture)));
        }

        public void SetItemNumber(double index, double value)
        {
            var isDenseIndex = !double.IsNaN(index)
                && !double.IsInfinity(index)
                && index % 1.0 == 0.0
                && index >= 0
                && index <= int.MaxValue;
            if (!isDenseIndex)
            {
                JavaScriptRuntime.ObjectRuntime.SetProperty(
                    this,
                    DotNet2JSConversions.ToString(index),
                    value);
                return;
            }

            TrySetIndexNumber((int)index, value, throwOnError: true);
        }

        /// <summary>
        /// Implements the JavaScript Array constructor semantics:
        ///  - new Array() => []
        ///  - new Array(len) where len is a non-negative integer => array with that length
        ///  - new Array(a, b, ...) => array containing the provided elements
        ///
        /// Note: In this runtime model, CLR null represents JS undefined.
        /// </summary>
        public static Array Construct(object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return new Array();
            }

            if (args.Length == 1)
            {
                var a0 = args[0];

                // JS: if the single argument is a number, it is treated as length (with RangeError for invalid).
                // Otherwise it is treated as an element.
                if (a0 is double || a0 is float || a0 is decimal ||
                    a0 is int || a0 is long || a0 is short || a0 is byte || a0 is sbyte ||
                    a0 is uint || a0 is ulong || a0 is ushort)
                {
                    var d = TypeUtilities.ToNumber(a0);
                    // JS requires a finite integer in [0, 2^32-1]. Keep minimal and clamp to int.MaxValue.
                    if (double.IsNaN(d) || double.IsInfinity(d))
                    {
                        throw new RangeError("Invalid array length");
                    }

                    // Validate that d is a non-negative integer within [0, int.MaxValue].
                    if (d < 0 || d > int.MaxValue || d % 1 != 0)
                    {
                        throw new RangeError("Invalid array length");
                    }

                    var len = (int)d;
                    var result = new Array(len);
                    result._logicalLength = len;
                    result._virtualLength = len;
                    return result;
                }

                return new Array(new object?[] { a0 });
            }

            // Multiple arguments => array of elements.
            return new Array(args);
        }

        /// <summary>
        /// Implements Array constructor semantics against an existing Array instance.
        /// This is used by derived CLR types (e.g., JS class extending Array) where the
        /// instance is already constructed and needs to be initialized by a `super(...)` call.
        /// </summary>
        public void ConstructInto(object[] args)
        {
            var constructed = Construct(args ?? System.Array.Empty<object>());
            this.Clear();

            // Preserve JS semantics: length is Count, and missing elements are represented as null (undefined).
            if (constructed.DenseCount > 0)
            {
                this.AddRange(constructed);
            }
            _logicalLength = constructed.Count;
        }

        public static Array Empty => new Array();
        public static implicit operator Array(object[] array)
        {
            return new Array(array);
        }

        /// <summary>
        /// JavaScript Array.from(source) minimal implementation.
        /// Supports JavaScriptRuntime.Array, IEnumerable, and Set.
        /// </summary>
        public static Array from(object? source)
            => from(source, null, null);

        public static Array from(object? source, object? mapFn)
            => from(source, mapFn, null);

        public static Array from(object? source, object? mapFn, object? thisArg)
        {
            if (source == null) return new Array();

            if (mapFn is not null && mapFn is not JsNull && !IsCallable(mapFn))
            {
                throw new TypeError("Array.from: when provided, the second argument must be a function");
            }

            // If already a JS array, return a shallow copy
            if (source is Array jsArr)
            {
                return CopyFromIndexedSource(jsArr, (int)jsArr.length, mapFn, thisArg);
            }

            if (source is string)
            {
                return FromIterator(JavaScriptRuntime.ObjectRuntime.GetIterator(source), mapFn, thisArg);
            }

            if (TryGetArrayLikeLength(source, out var length))
            {
                return CopyFromIndexedSource(source, length, mapFn, thisArg);
            }

            if (source is IJavaScriptIterator iterator)
            {
                return FromIterator(iterator, mapFn, thisArg);
            }

            var iteratorMethod = JavaScriptRuntime.ObjectRuntime.GetItem(source, Symbol.iterator);
            if (iteratorMethod is not null && iteratorMethod is not JsNull)
            {
                return FromIterator(JavaScriptRuntime.ObjectRuntime.GetIterator(source), mapFn, thisArg);
            }

            // If source is IEnumerable, copy items
            if (source is System.Collections.IEnumerable enumerable)
            {
                var result = new Array();
                int index = 0;
                foreach (var item in enumerable)
                {
                    result.Add(ApplyMapFunction(mapFn, thisArg, item, index++));
                }
                return result;
            }

            // Fallback: wrap single element
            var fallback = new Array();
            fallback.Add(ApplyMapFunction(mapFn, thisArg, source, 0));
            return fallback;
        }

        private static Array FromIterator(IJavaScriptIterator iterator, object? mapFn, object? thisArg)
        {
            var result = new Array();
            int index = 0;
            while (true)
            {
                var step = iterator.Next();
                if (step.done)
                {
                    break;
                }

                result.Add(ApplyMapFunction(mapFn, thisArg, step.value, index++));
            }

            return result;
        }

        private static Array CopyFromIndexedSource(object source, int length, object? mapFn, object? thisArg)
        {
            var result = new Array();
            for (int i = 0; i < length; i++)
            {
                var item = JavaScriptRuntime.ObjectRuntime.GetItem(source, i.ToString(CultureInfo.InvariantCulture));
                result.Add(ApplyMapFunction(mapFn, thisArg, item, i));
            }

            return result;
        }

        private static object? ApplyMapFunction(object? mapFn, object? thisArg, object? value, int index)
        {
            if (mapFn is null || mapFn is JsNull)
            {
                return value;
            }

            return CallableOperations.Call2(mapFn, thisArg, value, (double)index);
        }

        private static bool TryGetArrayLikeLength(object source, out int length)
        {
            var lengthValue = JavaScriptRuntime.ObjectRuntime.GetProperty(source, "length");
            if (lengthValue is null || lengthValue is JsNull)
            {
                length = 0;
                return false;
            }

            var numericLength = JavaScriptRuntime.TypeUtilities.ToNumber(lengthValue);
            if (double.IsNaN(numericLength) || numericLength < 0)
            {
                length = 0;
                return false;
            }

            length = (int)global::System.Math.Min(numericLength, int.MaxValue);
            return true;
        }

        private static bool IsCallable(object? value)
            => CallableOperations.IsCallable(value);

        /// <summary>
        /// JavaScript Array.isArray(value) static method.
        /// Returns true if the provided value is a JavaScriptRuntime.Array instance; false otherwise.
        /// </summary>
        public static bool isArray(object? value)
        {
            return value is Array || ReferenceEquals(value, Prototype);
        }

        /// <summary>
        /// JavaScript Array.of(...items)
        /// </summary>
        public static Array of(object[]? args)
        {
            return args == null ? new Array() : new Array(args);
        }

        /// <summary>
        /// JavaScript Array.length property
        /// </summary>
        public double length
        {
            get => global::System.Math.Max(this.Count, _virtualLength);
            set => SetLength(value, throwOnError: true);
        }

        public void SetLength(double value, bool throwOnError)
        {
            if (CanUseDenseMutationFastPath())
            {
                SetLengthStorage(ValidateLengthValue(value));
                return;
            }

            if (CanSetLength(throwOnError))
            {
                SetValidatedLength(ValidateLengthValue(value), throwOnError, allowDenseFastPath: false);
            }
        }

        public void SetLength(object? value, bool throwOnError)
        {
            if (CanSetLength(throwOnError))
            {
                SetValidatedLength(ValidateLengthValue(value), throwOnError, allowDenseFastPath: true);
            }
        }

        private bool CanSetLength(bool throwOnError)
        {
            if (!HasNonDataDescriptors || IsLengthWritable)
            {
                return true;
            }

            if (throwOnError)
            {
                throw new TypeError("Cannot assign to read only property 'length' of array");
            }

            return false;
        }

        private void SetValidatedLength(double newLength, bool throwOnError, bool allowDenseFastPath)
        {
            if (allowDenseFastPath && CanUseDenseMutationFastPath())
            {
                SetLengthStorage(newLength);
                return;
            }

            if (!DefineLengthProperty(CreateLengthDescriptorWithValue(newLength)) && throwOnError)
            {
                throw new TypeError("Cannot assign to property 'length' of array");
            }
        }

        /// <summary>
        /// JavaScript Array.sort() default behavior: sorts elements as strings in ascending order and returns the array.
        /// Note: This is a minimal implementation to support tests; comparator overload is ignored if provided.
        /// </summary>
        public Array sort()
        {
            this.Sort((a, b) => string.Compare(DotNet2JSConversions.ToString(a), DotNet2JSConversions.ToString(b), StringComparison.Ordinal));
            return this;
        }

        /// <summary>
        /// Overload matching intrinsic dispatch that may pass arguments; supports optional comparator callback.
        /// </summary>
        public Array sort(object[] args)
        {
            // If a comparator function is provided, use it; otherwise fallback to default string sort
            if (args != null && args.Length > 0 && args[0] != null)
            {
                var cb = args[0];
                Func<object, object, object?>? compareCallback = null;

                int CompareUsingCallback(object a, object b)
                {
                    compareCallback ??= CreateSortComparatorInvoker(cb, this);
                    if (compareCallback == null)
                    {
                        return string.Compare(DotNet2JSConversions.ToString(a), DotNet2JSConversions.ToString(b), StringComparison.Ordinal);
                    }

                    object? result = compareCallback(a, b);

                    // Coerce result to a JS number (double) and map to -1/0/1
                    double d;
                    switch (result)
                    {
                        case null:
                            d = 0d; break;
                        case double dd:
                            d = dd; break;
                        case float ff:
                            d = ff; break;
                        case int ii:
                            d = ii; break;
                        case long ll:
                            d = ll; break;
                        case short ss:
                            d = ss; break;
                        case byte bb:
                            d = bb; break;
                        case bool bo:
                            d = bo ? 1d : 0d; break;
                        case string str:
                            if (!double.TryParse(str, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d)) d = double.NaN;
                            break;
                        default:
                            try { d = Convert.ToDouble(result, System.Globalization.CultureInfo.InvariantCulture); }
                            catch { d = double.NaN; }
                            break;
                    }

                    if (double.IsNaN(d) || d == 0d) return 0;
                    return d < 0d ? -1 : 1;
                }

                try
                {
                    this.Sort((a, b) => CompareUsingCallback(a!, b!));
                }
                catch (InvalidOperationException exception)
                    when (exception.InnerException is SortComparisonException comparisonException)
                {
                    throw comparisonException.InnerException!;
                }

                return this;
            }

            return sort();
        }

        private delegate object? ArrayCallbackInvoker(object? a0, object? a1, object? a2, object? a3);

        private sealed class SortComparisonException : Exception
        {
            public SortComparisonException(Exception innerException)
                : base("JavaScript sort comparator threw an exception.", innerException)
            {
            }
        }

        private static Func<object, object, object?>? CreateSortComparatorInvoker(object? cb, Array array)
        {
            if (CallableOperations.IsCallable(cb))
            {
                return (a, b) =>
                {
                    try
                    {
                        return CallableOperations.Call2(cb, null, a, b);
                    }
                    catch (Exception exception)
                    {
                        throw new SortComparisonException(exception);
                    }
                };
            }

            return null;
        }

        private static ArrayCallbackInvoker CreateArrayCallbackInvoker(object? cb, int argCount, string callbackKind)
        {
            if (CallableOperations.IsCallable(cb))
            {
                return argCount switch
                {
                    0 => (_, _, _, _) => CallableOperations.Call0(cb, null),
                    1 => (a0, _, _, _) => CallableOperations.Call1(cb, null, a0),
                    2 => (a0, a1, _, _) => CallableOperations.Call2(cb, null, a0, a1),
                    3 => (a0, a1, a2, _) => CallableOperations.Call3(cb, null, a0, a1, a2),
                    _ => (a0, a1, a2, a3) => CallableOperations.Call4(cb, null, a0, a1, a2, a3)
                };
            }

            throw new TypeError($"{callbackKind} callback is not a function");
        }

        /// <summary>
        /// JavaScript Array.map(callback[, thisArg])
        /// Minimal implementation: invokes the callback with (value, index, array) when supported and returns a new Array.
        /// Supports runtime-owned built-in delegates and generated function objects.
        /// </summary>
        public Array map(object[] args)
        {
            var result = new Array(this.Count);
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            ArrayCallbackInvoker? invoke = null;

            for (int i = 0; i < this.Count; i++)
            {
                var value = this[i];
                invoke ??= CreateArrayCallbackInvoker(cb, 3, "map");
                object? mapped = invoke(value, (double)i, this, null);

                result.Add(mapped);
            }

            return result;
        }

        /// <summary>
        /// JavaScript Array.forEach(callback[, thisArg])
        /// </summary>
        public object? forEach(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            ArrayCallbackInvoker? invoke = null;
            for (int i = 0; i < this.Count; i++)
            {
                invoke ??= CreateArrayCallbackInvoker(cb, 3, "forEach");
                _ = invoke(this[i], (double)i, this, null);
            }
            return null; // undefined
        }

        /// <summary>
        /// JavaScript Array.filter(callback[, thisArg])
        /// </summary>
        public Array filter(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            var result = new Array();
            ArrayCallbackInvoker? invoke = null;
            for (int i = 0; i < this.Count; i++)
            {
                invoke ??= CreateArrayCallbackInvoker(cb, 3, "filter");
                var keep = invoke(this[i], (double)i, this, null);
                if (Operators.IsTruthy(keep))
                {
                    result.Add(this[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// JavaScript Array.every(callback[, thisArg])
        /// </summary>
        public bool every(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            ArrayCallbackInvoker? invoke = null;
            for (int i = 0; i < this.Count; i++)
            {
                invoke ??= CreateArrayCallbackInvoker(cb, 3, "every");
                var ok = invoke(this[i], (double)i, this, null);
                if (!Operators.IsTruthy(ok))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// JavaScript Array.some(callback[, thisArg])
        /// </summary>
        public bool some(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            ArrayCallbackInvoker? invoke = null;
            for (int i = 0; i < this.Count; i++)
            {
                invoke ??= CreateArrayCallbackInvoker(cb, 3, "some");
                var ok = invoke(this[i], (double)i, this, null);
                if (Operators.IsTruthy(ok))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// JavaScript Array.reduce(callback[, initialValue])
        /// </summary>
        public object? reduce(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            bool hasInitial = args != null && args.Length > 1;
            if (!CallableOperations.IsCallable(cb))
            {
                throw new TypeError("reduce callback is not a function");
            }

            if (this.Count == 0 && !hasInitial)
            {
                throw new TypeError("Reduce of empty array with no initial value");
            }

            object? acc;
            int startIndex;
            if (hasInitial)
            {
                acc = args![1];
                startIndex = 0;
            }
            else
            {
                acc = this[0];
                startIndex = 1;
            }

            ArrayCallbackInvoker? invoke = null;
            for (int i = startIndex; i < this.Count; i++)
            {
                invoke ??= CreateArrayCallbackInvoker(cb, 4, "reduce");
                acc = invoke(acc, this[i], (double)i, this);
            }

            return acc;
        }

        /// <summary>
        /// JavaScript Array.reduceRight(callback[, initialValue])
        /// </summary>
        public object? reduceRight(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            bool hasInitial = args != null && args.Length > 1;
            if (!CallableOperations.IsCallable(cb))
            {
                throw new TypeError("reduceRight callback is not a function");
            }

            if (this.Count == 0 && !hasInitial)
            {
                throw new TypeError("Reduce of empty array with no initial value");
            }

            object? acc;
            int startIndex;
            if (hasInitial)
            {
                acc = args![1];
                startIndex = this.Count - 1;
            }
            else
            {
                acc = this[this.Count - 1];
                startIndex = this.Count - 2;
            }

            ArrayCallbackInvoker? invoke = null;
            for (int i = startIndex; i >= 0; i--)
            {
                invoke ??= CreateArrayCallbackInvoker(cb, 4, "reduceRight");
                acc = invoke(acc, this[i], (double)i, this);
            }

            return acc;
        }

        /// <summary>
        /// JavaScript Array.some(callback[, thisArg])
        /// Minimal implementation: invokes the callback with (value, index, array) and returns true if any call is truthy.
        /// </summary>
        public bool some(object? callback)
        {
            return some(callback, null);
        }

        public bool some(object? callback, object? thisArg)
        {
            // Note: thisArg is currently ignored in this runtime/compiler model.
            if (callback == null)
            {
                throw new TypeError("Array.prototype.some requires a callback function");
            }

            ArrayCallbackInvoker? invoke = null;
            for (int i = 0; i < this.Count; i++)
            {
                invoke ??= CreateArrayCallbackInvoker(callback, 3, "some");
                var result = invoke(this[i], (double)i, this, null);
                if (Operators.IsTruthy(result))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// JavaScript Array.findIndex(callback[, thisArg])
        /// Returns the index of the first element matching the predicate, or -1.
        /// </summary>
        public double findIndex(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            var thisArg = args != null && args.Length > 1 ? args[1] : null;
            if (!CallableOperations.IsCallable(cb))
            {
                throw new TypeError("findIndex callback is not a function");
            }

            var length = this.Count;
            for (int i = 0; i < length; i++)
            {
                var value = ObjectRuntime.GetItem(this, (double)i);
                var result = InvokeArrayCallback(cb, thisArg, "Array.prototype.findIndex", 3, value, (double)i, this, null);
                if (Operators.IsTruthy(result))
                {
                    return (double)i;
                }
            }
            return -1d;
        }

        /// <summary>
        /// JavaScript Array.findLast(callback[, thisArg])
        /// Returns the last element matching the predicate, or undefined.
        /// </summary>
        public object? findLast(object[] args)
            => FindFromLast(this, args, returnIndex: false);

        /// <summary>
        /// JavaScript Array.findLastIndex(callback[, thisArg])
        /// Returns the last index matching the predicate, or -1.
        /// </summary>
        public double findLastIndex(object[] args)
        {
            var result = FindFromLast(this, args, returnIndex: true);
            return result is double index ? index : -1d;
        }

        /// <summary>
        /// JavaScript Array.find(callback[, thisArg])
        /// Minimal implementation: invokes the callback with (value, index, array) and returns the first element for which the callback is truthy.
        /// Returns undefined (null) if none match.
        /// </summary>
        public object? find(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            var thisArg = args != null && args.Length > 1 ? args[1] : null;
            if (!CallableOperations.IsCallable(cb))
            {
                throw new TypeError("find callback is not a function");
            }

            var length = this.Count;
            for (int i = 0; i < length; i++)
            {
                var value = ObjectRuntime.GetItem(this, (double)i);
                var result = InvokeArrayCallback(cb, thisArg, "Array.prototype.find", 3, value, (double)i, this, null);

                if (Operators.IsTruthy(result))
                {
                    return value;
                }
            }

            return null;
        }

        /// <summary>
        /// JavaScript Array.indexOf(searchElement[, fromIndex])
        /// Uses strict equality semantics.
        /// </summary>
        public double indexOf(object[]? args)
        {
            int len = this.Count;
            if (len == 0) return -1d;

            object? searchElement = (args != null && args.Length > 0) ? args[0] : null;
            int from = 0;
            if (args != null && args.Length > 1)
            {
                from = ToInt(args[1]!, 0);
            }
            if (from < 0)
            {
                from = len + from;
                if (from < 0) from = 0;
            }
            if (from >= len) return -1d;

            for (int i = from; i < len; i++)
            {
                if (Operators.StrictEqual(this[i], searchElement)) return (double)i;
            }
            return -1d;
        }

        public double indexOf()
        {
            return -1d;
        }

        /// <summary>
        /// JavaScript Array.lastIndexOf(searchElement[, fromIndex])
        /// Uses strict equality semantics.
        /// </summary>
        public double lastIndexOf(object[]? args)
        {
            int len = this.Count;
            if (len == 0) return -1d;

            object? searchElement = (args != null && args.Length > 0) ? args[0] : null;
            int from = len - 1;
            if (args != null && args.Length > 1)
            {
                // Spec: fromIndex defaults to len-1
                from = ToInt(args[1]!, len - 1);
            }
            if (from < 0)
            {
                from = len + from;
            }
            if (from >= len) from = len - 1;
            if (from < 0) return -1d;

            for (int i = from; i >= 0; i--)
            {
                if (Operators.StrictEqual(this[i], searchElement)) return (double)i;
            }
            return -1d;
        }

        public double lastIndexOf()
        {
            return -1d;
        }

        /// <summary>
        /// JavaScript Array.at(index)
        /// </summary>
        public object? at(object? index)
        {
            int len = this.Count;
            var relativeIndex = ToIntegerOrInfinityForAt(index);
            var actualIndex = relativeIndex >= 0
                ? relativeIndex
                : len + relativeIndex;

            if (actualIndex < 0 || actualIndex >= len)
            {
                return null;
            }

            return this[(int)actualIndex];
        }

        public object? at()
        {
            return null;
        }

        private static double ToIntegerOrInfinityForAt(object? value)
        {
            if (value == null)
            {
                return 0d;
            }

            var number = TypeUtilities.ToNumber(value);
            if (double.IsNaN(number))
            {
                return 0d;
            }

            if (double.IsInfinity(number))
            {
                return number;
            }

            return global::System.Math.Truncate(number);
        }

        /// <summary>
        /// JavaScript Array.join([separator]) implementation.
        /// Joins elements by the given separator (default ',') and returns a string.
        /// Each element is converted using DotNet2JSConversions.ToString to approximate JS semantics.
        /// </summary>
        public string join(object[]? args)
        {
            string separator = ",";
            if (args != null && args.Length > 0)
            {
                separator = DotNet2JSConversions.ToString(args[0]);
            }
            if (this.Count == 0) return string.Empty;

            var builder = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(separator);
                }

                var v = this[i];
                builder.Append(DotNet2JSConversions.ToString(v));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Overload without parameters to match potential direct dispatch.
        /// </summary>
        public string join()
        {
            return join(System.Array.Empty<object>());
        }

        /// <summary>
        /// JavaScript Array.toString()
        /// Minimal: delegates to join(',').
        /// </summary>
        public string toString(object[]? args)
        {
            return join(System.Array.Empty<object>());
        }

        public string toString()
        {
            return join(System.Array.Empty<object>());
        }

        /// <summary>
        /// JavaScript Array.toLocaleString()
        /// Minimal: same as toString for now.
        /// </summary>
        public string toLocaleString(object[]? args)
        {
            return toString();
        }

        public string toLocaleString()
        {
            return toString();
        }

        /// <summary>
        /// JavaScript Array.includes(searchElement[, fromIndex]) implementation.
        /// Uses SameValueZero comparison (NaN equals NaN; +0 and -0 are equal).
        /// </summary>
        public bool includes(object[]? args)
        {
            int len = this.Count;
            if (len == 0) return false;

            object? searchElement = (args != null && args.Length > 0) ? args[0] : null;

            var startIndex = args != null && args.Length > 1
                ? CoerceArrayLikeSearchStartIndex(args[1], len)
                : 0d;
            if (startIndex >= len)
            {
                return false;
            }

            for (int i = (int)startIndex; i < len; i++)
            {
                if (SameValueZero(ObjectRuntime.GetItem(this, (double)i), searchElement)) return true;
            }
            return false;
        }

        /// <summary>
        /// Overload without parameters; returns false if no search element provided.
        /// </summary>
        public bool includes()
        {
            return false;
        }

        private static bool SameValueZero(object? x, object? y)
        {
            if (ReferenceEquals(x, y)) return true;

            // null/undefined handling: undefined is represented by null; null is JsNull
            if (x is null || y is null)
            {
                // both null: handled by ReferenceEquals above; here only one is null
                return false;
            }

            if (x is JsNull && y is JsNull) return true;

            // Numbers: compare as double, with NaN equal to NaN
            if (TryToDouble(x, out var dx) && TryToDouble(y, out var dy))
            {
                if (double.IsNaN(dx) && double.IsNaN(dy)) return true;
                return dx.Equals(dy);
            }

            // Strings
            if (x is string sx && y is string sy) return string.Equals(sx, sy, StringComparison.Ordinal);

            // Booleans
            if (x is bool bx && y is bool by) return bx == by;

            // Fallback: reference equality only (objects/arrays/functions)
            return false;
        }

        private static bool TryToDouble(object o, out double d)
        {
            switch (o)
            {
                case double dd:
                    d = dd; return true;
                case float ff:
                    d = ff; return true;
                case int ii:
                    d = ii; return true;
                case long ll:
                    d = ll; return true;
                case short ss:
                    d = ss; return true;
                case byte bb:
                    d = bb; return true;
                case string s when double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var pd):
                    d = pd; return true;
                default:
                    d = 0; return false;
            }
        }

        // Shared index coercion: converts start-like argument to a clamped index in [0, len]
        private static int CoerceStartIndex(object? arg, int len, int defaultValue)
        {
            int idx = defaultValue;
            if (arg == null)
            {
                idx = defaultValue;
            }
            else
            {
                try { idx = ToInt(arg, defaultValue); } catch { idx = defaultValue; }
            }
            if (idx < 0)
            {
                idx = len + idx;
                if (idx < 0) idx = 0;
            }
            else if (idx > len)
            {
                idx = len;
            }
            return idx;
        }

        /// <summary>
        /// JavaScript Array.slice([start[, end]]) implementation.
        /// Returns a shallow copy of a portion of the array into a new Array object.
        /// Handles negative indices and defaults per JS spec.
        /// </summary>
        public Array slice(object[]? args)
        {
            int len = this.Count;

            // Defaults
            int start = 0;
            int end = len;

            // Optional debug: print incoming argument shapes to stderr when enabled
            try
            {
                if (System.Environment.GetEnvironmentVariable("JROC_DEBUG_SLICE") == "1")
                {
                    var alen = args?.Length ?? 0;
                    string a0t = alen > 0 ? (args![0]?.GetType().FullName ?? "<null>") : "<none>";
                    string a1t = alen > 1 ? (args![1]?.GetType().FullName ?? "<null>") : "<none>";
                    string a0v = alen > 0 ? JavaScriptRuntime.DotNet2JSConversions.ToString(args![0]) : "<none>";
                    string a1v = alen > 1 ? JavaScriptRuntime.DotNet2JSConversions.ToString(args![1]) : "<none>";
                    System.Console.Error.WriteLine($"[slice dbg] len={len} argsLen={alen} a0Type={a0t} a0Val={a0v} a1Type={a1t} a1Val={a1v}");
                }
            }
            catch { /* best-effort debug only */ }

            // start argument
            if (args != null && args.Length > 0)
            {
                start = CoerceStartIndex(args[0], len, 0);
            }

        // end argument
            if (args != null && args.Length > 1)
            {
                var endArg = args[1];
                if (endArg == null)
                {
                    // undefined => keep default end = len
                }
                else if (endArg is JsNull)
                {
                    end = 0; // null => +0
                }
                else
                {
            // Per spec, only undefined should keep len; other non-numeric => +0
            try { end = ToInt(endArg, 0); }
            catch { end = 0; }
                }

                if (end < 0)
                {
                    end = len + end;
                    if (end < 0) end = 0;
                }
                else if (end > len)
                {
                    end = len;
                }
            }

            int count = end - start;
            if (count <= 0) return new Array();

            var result = new Array(count);
            for (int k = start; k < end; k++)
            {
                result.Add(this[k]);
            }
            return result;
        }

        /// <summary>
        /// Overload without parameters to match potential direct dispatch.
        /// </summary>
        public Array slice()
        {
            return slice(null);
        }

        /// <summary>
        /// Overload for one argument to align with dispatcher arity matching.
        /// </summary>
        public Array slice(object start)
        {
            return slice(new object[] { start });
        }

        /// <summary>
        /// Overload for two arguments to align with dispatcher arity matching.
        /// </summary>
        public Array slice(object start, object end)
        {
            return slice(new object[] { start, end });
        }

        /// <summary>
        /// JavaScript Array.splice(start[, deleteCount[, item1[, item2[, ...]]]])
        /// Mutates the array by removing and/or inserting elements. Returns a new Array of removed elements.
        /// </summary>
        public Array splice(object[]? args)
        {
            int len = this.Count;

            // No arguments => no-op; return empty array
            if (args == null || args.Length == 0)
            {
                return new Array();
            }

            // Compute start index (clamped)
            int start = CoerceStartIndex(args[0], len, 0);

            // Determine deleteCount per spec
            int deleteCount;
            if (args.Length == 1)
            {
                // Omitted deleteCount => remove to end
                deleteCount = len - start;
            }
            else
            {
                var delArg = args[1];
                // When provided, undefined/null => 0; otherwise ToInt then clamp to [0, len-start]
                int raw = 0;
                try { raw = delArg == null ? 0 : ToInt(delArg, 0); } catch { raw = 0; }
                if (raw < 0) raw = 0;
                int max = len - start;
                deleteCount = raw > max ? max : raw;
            }

            var insertCount = global::System.Math.Max(args.Length - 2, 0);
            if (insertCount > int.MaxValue - (len - deleteCount))
            {
                throw new RangeError("Invalid array length");
            }

            var newLength = len - deleteCount + insertCount;
            var canUseDenseFastPath = Count == len
                && (insertCount <= deleteCount
                    ? CanUseDenseMutationFastPath()
                    : CanUseDenseGrowthFastPath());
            if (canUseDenseFastPath)
            {
                var removedDense = new Array(deleteCount);
                for (var i = 0; i < deleteCount; i++)
                {
                    if (_numberItems is not null)
                    {
                        removedDense.AddNumber(_numberItems[start + i]);
                    }
                    else
                    {
                        removedDense.AddDenseValue(_items![start + i]);
                    }
                }
                removedDense.SynchronizeDenseLength();

                RemoveDenseRange(start, deleteCount);
                if (insertCount == 1)
                {
                    InsertDenseValue(start, args[2]);
                }
                else if (insertCount > 1)
                {
                    for (var i = 0; i < insertCount; i++)
                    {
                        InsertDenseValue(start + i, args[i + 2]);
                    }
                }

                SynchronizeDenseLength();
                return removedDense;
            }

            // Gather removed elements, preserving holes.
            var removed = new Array();
            removed.length = deleteCount;
            for (int i = 0; i < deleteCount; i++)
            {
                var source = start + i;
                if (ObjectRuntime.HasPropertyForArrayLike((double)source, this))
                {
                    removed.TrySetIndexValue(i, ObjectRuntime.GetItem(this, (double)source), throwOnError: true);
                }
            }

            if (insertCount < deleteCount)
            {
                for (var target = start; target < len - deleteCount; target++)
                {
                    var source = target + deleteCount;
                    var destination = target + insertCount;
                    if (ObjectRuntime.HasPropertyForArrayLike((double)source, this))
                    {
                        TrySetIndexValue(destination, ObjectRuntime.GetItem(this, (double)source), throwOnError: true);
                    }
                    else
                    {
                        DeleteIndexOrThrow(destination);
                    }
                }

                for (var indexToDelete = len - 1; indexToDelete >= len - deleteCount + insertCount; indexToDelete--)
                {
                    DeleteIndexOrThrow(indexToDelete);
                }
            }
            else if (insertCount > deleteCount)
            {
                for (var source = len - 1; source >= start + deleteCount; source--)
                {
                    var destination = source - deleteCount + insertCount;
                    if (ObjectRuntime.HasPropertyForArrayLike((double)source, this))
                    {
                        TrySetIndexValue(destination, ObjectRuntime.GetItem(this, (double)source), throwOnError: true);
                    }
                    else
                    {
                        DeleteIndexOrThrow(destination);
                    }
                }
            }

            for (var i = 0; i < insertCount; i++)
            {
                TrySetIndexValue(start + i, args[2 + i], throwOnError: true);
            }

            SetLength(newLength, throwOnError: true);
            return removed;
        }

        /// <summary>
        /// Overload without parameters
        /// </summary>
        public Array splice()
        {
            return splice(null);
        }

        /// <summary>
        /// Overload with start only
        /// </summary>
        public Array splice(object start)
        {
            return splice(new object[] { start });
        }

        /// <summary>
        /// Overload with start and deleteCount
        /// </summary>
        public Array splice(object start, object deleteCount)
        {
            return splice(new object[] { start, deleteCount });
        }

        /// <summary>
        /// Overload with start, deleteCount and one inserted item.
        /// Avoids params-array packing at common three-argument call sites.
        /// </summary>
        public Array splice(object start, object deleteCount, object item1)
        {
            return splice(new object[] { start, deleteCount, item1 });
        }

        private static int ToInt(object value, int defaultValue)
        {
            try
            {
                if (value == null) return defaultValue;
                switch (value)
                {
                    case double dd:
                        if (double.IsNaN(dd)) return defaultValue;
                        if (double.IsPositiveInfinity(dd)) return int.MaxValue;
                        if (double.IsNegativeInfinity(dd)) return int.MinValue;
                        return (int)dd;
                    case float ff:
                        if (float.IsNaN(ff)) return defaultValue;
                        if (float.IsPositiveInfinity(ff)) return int.MaxValue;
                        if (float.IsNegativeInfinity(ff)) return int.MinValue;
                        return (int)ff;
                    case JsNull:
                        return defaultValue;
                    case decimal dec:
                        return (int)dec;
                    case int ii:
                        return ii;
                    case long ll:
                        return (int)ll;
                    case uint u32:
                        return (int)u32;
                    case ulong u64:
                        return u64 > (ulong)int.MaxValue ? int.MaxValue : (int)u64;
                    case short ss:
                        return ss;
                    case byte bb:
                        return bb;
                    case sbyte sb:
                        return sb;
                    case ushort us:
                        return us;
                    case bool b:
                        return b ? 1 : 0;
                    case string s:
                        if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var pd))
                        {
                            if (double.IsNaN(pd)) return defaultValue;
                            if (double.IsPositiveInfinity(pd)) return int.MaxValue;
                            if (double.IsNegativeInfinity(pd)) return int.MinValue;
                            return (int)pd;
                        }
                        return defaultValue;
                    case System.Array:
                        // Arrays/tuples are non-numeric in JS when coerced to number => NaN => default
                        return defaultValue;
                    default:
                        // As a last resort, try parsing the object's string representation
                        try
                        {
                            var str = DotNet2JSConversions.ToString(value);
                            if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var d2))
                            {
                                if (double.IsNaN(d2)) return defaultValue;
                                if (double.IsPositiveInfinity(d2)) return int.MaxValue;
                                if (double.IsNegativeInfinity(d2)) return int.MinValue;
                                return (int)d2;
                            }
                        }
                        catch { /* ignore */ }
                        return defaultValue;
                }
            }
            catch { return defaultValue; }
        }

        /// <summary>
        /// JavaScript Array.push(...items): appends items to the end and returns the new length.
        /// </summary>
        public double push(object? item)
        {
            var newLength = length;
            if (newLength < int.MaxValue && CanUseDenseGrowthFastPath())
            {
                AddDenseValue(item);
                SynchronizeDenseLength();
                return length;
            }

            return PushItems(new object?[] { item });
        }

        /// <summary>
        /// JavaScript Array.push(...items): appends items to the end and returns the new length.
        /// </summary>
        public double push(object[]? args)
            => PushItems(args);

        /// <summary>
        /// Overload without parameters to match potential direct dispatch; returns current length.
        /// </summary>
        public double push()
            => PushItems(null);

        private double PushItems(object?[]? items)
        {
            var newLength = length;
            if (items != null
                && items.Length > 0
                && newLength <= int.MaxValue - items.Length
                && CanUseDenseGrowthFastPath())
            {
                foreach (var item in items)
                {
                    AddDenseValue(item);
                }
                SynchronizeDenseLength();
                return length;
            }

            if (items != null)
            {
                foreach (var item in items)
                {
                    var key = newLength.ToString(CultureInfo.InvariantCulture);
                    ObjectRuntime.SetProperty(this, key, item, throwOnError: true);
                    newLength++;
                }
            }

            SetLength(newLength, throwOnError: true);
            return newLength;
        }

        /// <summary>
        /// JavaScript Array.pop(): removes the last element from the array and returns it.
        /// If the array is empty, returns undefined (represented as null in this runtime).
        /// </summary>
        public object? pop(object[]? args)
        {
            if (CanUseDenseMutationFastPath())
            {
                if (DenseCount == 0)
                {
                    return null;
                }

                var lastIndex = DenseCount - 1;
                var denseValue = GetDenseValue(lastIndex);
                RemoveDenseRange(lastIndex, 1);
                SynchronizeDenseLength();
                return denseValue;
            }

            var currentLength = length;
            if (currentLength == 0)
            {
                SetLength(0, throwOnError: true);
                return null; // JS undefined
            }

            var newLength = currentLength - 1d;
            var key = newLength.ToString(CultureInfo.InvariantCulture);
            var value = ObjectRuntime.GetProperty(this, key);
            if (!DeleteOwnProperty(key))
            {
                throw new TypeError($"Cannot delete property '{key}' of array");
            }

            SetLength(newLength, throwOnError: true);
            return value;
        }

        /// <summary>
        /// Overload without parameters to match potential direct dispatch.
        /// </summary>
        public object? pop()
        {
            return pop(null);
        }

        /// <summary>
        /// Pushes all items from the given source enumerable into this array.
        /// Used by codegen to implement spread syntax in array literals.
        /// </summary>
        public void PushRange(object source)
        {
            // Spec-aligned behavior: array spread consumes the iterator protocol.
            var iterator = JavaScriptRuntime.ObjectRuntime.GetIterator(source);
            while (true)
            {
                var step = iterator.Next();
                if (step.done)
                {
                    break;
                }
                this.Add(step.value);
            }
        }

        /// <summary>
        /// JavaScript Array.shift(): removes and returns first element; returns undefined when empty.
        /// </summary>
        public object? shift(object[]? args)
        {
            if (CanUseDenseMutationFastPath())
            {
                if (DenseCount == 0)
                {
                    return null;
                }

                var denseValue = GetDenseValue(0);
                RemoveDenseRange(0, 1);
                SynchronizeDenseLength();
                return denseValue;
            }

            if (this.Count == 0) return null;
            var v = this[0];
            this.RemoveAt(0);
            return v;
        }

        public object? shift()
        {
            return shift(null);
        }

        /// <summary>
        /// JavaScript Array.unshift(...items): prepends items and returns new length.
        /// </summary>
        public double unshift(object[]? args)
        {
            if (args != null && args.Length > 0)
            {
                InsertRange(0, args);
            }
            return (double)this.Count;
        }

        public double unshift()
        {
            return (double)this.Count;
        }

        public double unshift(object item1)
        {
            this.Insert(0, item1);
            return (double)this.Count;
        }

        /// <summary>
        /// JavaScript Array.reverse(): in-place reverse.
        /// </summary>
        public Array reverse(object[]? args)
        {
            this.Reverse();
            return this;
        }

        public Array reverse()
        {
            return reverse(null);
        }

        /// <summary>
        /// JavaScript Array.concat(...items): returns a new array.
        /// </summary>
        public Array concat(object[]? args)
        {
            var result = new Array(this);
            if (args == null || args.Length == 0) return result;

            for (int i = 0; i < args.Length; i++)
            {
                var item = args[i];
                if (ShouldConcatSpread(item))
                {
                    AppendConcatElements(result, item!);
                }
                else
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public Array concat()
        {
            return new Array(this);
        }

        private static bool ShouldConcatSpread(object? item)
        {
            if (item is null || item is JsNull)
            {
                return false;
            }

            if (item is not string && !item.GetType().IsValueType)
            {
                var spreadable = JavaScriptRuntime.ObjectRuntime.GetItem(item, Symbol.isConcatSpreadable);
                if (spreadable is not null)
                {
                    return TypeUtilities.ToBoolean(spreadable);
                }
            }

            return item is Array;
        }

        private static void AppendConcatElements(Array result, object item)
        {
            if (item is Array arr)
            {
                for (int j = 0; j < arr.Count; j++)
                {
                    result.Add(arr[j]);
                }

                return;
            }

            var length = ToArrayLikeLength(item);
            for (int j = 0; j < length; j++)
            {
                result.Add(JavaScriptRuntime.ObjectRuntime.GetItem(item, (double)j));
            }
        }

        /// <summary>
        /// JavaScript Array.fill(value[, start[, end]])
        /// </summary>
        public Array fill(object[]? args)
        {
            var value = (args != null && args.Length > 0) ? args[0] : null;
            int len = this.Count;
            int start = 0;
            int end = len;

            if (args != null && args.Length > 1)
            {
                start = CoerceStartIndex(args[1], len, 0);
            }
            if (args != null && args.Length > 2)
            {
                var endArg = args[2];
                end = endArg == null ? len : ToInt(endArg, len);
                if (end < 0)
                {
                    end = len + end;
                    if (end < 0) end = 0;
                }
                else if (end > len)
                {
                    end = len;
                }
            }

            for (int i = start; i < end; i++)
            {
                this[i] = value;
            }
            return this;
        }

        public Array fill()
        {
            return fill(new object[] { null! });
        }

        /// <summary>
        /// JavaScript Array.copyWithin(target[, start[, end]])
        /// </summary>
        public Array copyWithin(object[]? args)
        {
            int len = this.Count;
            if (len == 0) return this;

            int target = 0;
            int start = 0;
            int end = len;

            if (args != null && args.Length > 0)
            {
                target = ToInt(args[0]!, 0);
            }
            if (args != null && args.Length > 1)
            {
                start = ToInt(args[1]!, 0);
            }
            if (args != null && args.Length > 2 && args[2] != null)
            {
                end = ToInt(args[2]!, len);
            }

            // Normalize indexes
            if (target < 0) target = len + target;
            if (start < 0) start = len + start;
            if (end < 0) end = len + end;

            if (target < 0) target = 0;
            if (start < 0) start = 0;
            if (end > len) end = len;
            if (target >= len) return this;

            int count = end - start;
            if (count <= 0) return this;
            if (count > len - target) count = len - target;

            // Copy via temp buffer to handle overlap safely.
            var temp = new object?[count];
            for (int i = 0; i < count; i++) temp[i] = this[start + i];
            for (int i = 0; i < count; i++) this[target + i] = temp[i];

            return this;
        }

        public Array copyWithin()
        {
            return this;
        }

        /// <summary>
        /// JavaScript Array.flat([depth])
        /// </summary>
        public Array flat(object[]? args)
        {
            int depth = 1;
            if (args != null && args.Length > 0 && args[0] != null)
            {
                depth = ToInt(args[0], 0);
            }
            if (depth < 0) depth = 0;

            var result = new Array();
            FlattenInto(result, this, depth);
            return result;
        }

        public Array flat()
        {
            return flat(null);
        }

        private static void FlattenInto(Array target, Array source, int depth)
        {
            for (int i = 0; i < source.Count; i++)
            {
                var v = source[i];
                if (depth > 0 && v is Array arr)
                {
                    FlattenInto(target, arr, depth - 1);
                }
                else
                {
                    target.Add(v);
                }
            }
        }

        private static void FlattenIntoArrayLike(Array target, object source, int depth)
        {
            int length = ToArrayLikeLength(source);
            for (int i = 0; i < length; i++)
            {
                var key = (double)i;
                if (!JavaScriptRuntime.ObjectRuntime.HasPropertyForArrayLike(key, source))
                {
                    continue;
                }

                var value = JavaScriptRuntime.ObjectRuntime.GetItem(source, key);
                if (depth > 0 && value is Array nestedArray)
                {
                    FlattenIntoArrayLike(target, nestedArray, depth - 1);
                }
                else
                {
                    target.Add(value);
                }
            }
        }

        /// <summary>
        /// JavaScript Array.flatMap(callback[, thisArg])
        /// Maps then flattens one level.
        /// </summary>
        public Array flatMap(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            var mapped = new Array();
            ArrayCallbackInvoker? invoke = null;

            for (int i = 0; i < this.Count; i++)
            {
                invoke ??= CreateArrayCallbackInvoker(cb, 3, "flatMap");
                var m = invoke(this[i], (double)i, this, null);
                mapped.Add(m);
            }

            return mapped.flat(new object[] { 1d });
        }

        /// <summary>
        /// JavaScript Array.toReversed(): returns a reversed copy.
        /// </summary>
        public Array toReversed(object[]? args)
        {
            var len = ToArrayLikeLength(this);
            var result = new Array(len);
            for (int k = 0; k < len; k++)
            {
                result[k] = JavaScriptRuntime.ObjectRuntime.GetItem(this, (double)(len - k - 1));
            }

            return result;
        }

        public Array toReversed()
        {
            return toReversed(null);
        }

        /// <summary>
        /// JavaScript Array.toSorted([compareFn]): returns a sorted copy.
        /// </summary>
        public Array toSorted(object[]? args)
            => ToSorted(this, args);

        public Array toSorted()
        {
            return toSorted(null);
        }

        /// <summary>
        /// JavaScript Array.toSpliced(start, deleteCount, ...items): returns a copy with splice applied.
        /// </summary>
        public Array toSpliced(object[]? args)
        {
            var copy = new Array(this);
            copy.splice(args);
            return copy;
        }

        /// <summary>
        /// JavaScript Array.with(index, value): returns a copy with element at index replaced.
        /// </summary>
        public Array with(object[]? args)
            => With(this, args);

        private static Array ToSorted(object? receiver, object?[]? args)
        {
            var compareFn = args is { Length: > 0 } ? args[0] : null;
            if (compareFn is not null && !CallableOperations.IsCallable(compareFn))
            {
                throw new TypeError("Array.prototype.toSorted comparator must be a function");
            }

            var length = GetCopyByChangeLength(receiver, "toSorted");
            var copy = new Array(length);
            for (var index = 0; index < length; index++)
            {
                copy.Add(ObjectRuntime.GetItem(receiver!, (double)index));
            }

            if (compareFn is null)
            {
                copy.sort();
            }
            else
            {
                copy.sort(new[] { compareFn });
            }

            return copy;
        }

        private static Array With(object? receiver, object?[]? args)
        {
            var length = GetCopyByChangeLength(receiver, "with");
            var index = args is { Length: > 0 } ? args[0] : null;
            var value = args is { Length: > 1 } ? args[1] : null;
            var relativeIndex = ToIntegerOrInfinityForAt(index);
            var actualIndex = relativeIndex >= 0d
                ? relativeIndex
                : length + relativeIndex;

            if (actualIndex < 0d || actualIndex >= length)
            {
                throw new RangeError("Invalid index");
            }

            var copy = new Array(length);
            for (var k = 0; k < length; k++)
            {
                copy.Add(k == (int)actualIndex
                    ? value
                    : ObjectRuntime.GetItem(receiver!, (double)k));
            }

            return copy;
        }

        private static int GetCopyByChangeLength(object? receiver, string methodName)
        {
            if (receiver is null || receiver is JsNull)
            {
                throw new TypeError($"Array.prototype.{methodName} called on null or undefined");
            }

            var length = TypeUtilities.ToNumber(ObjectRuntime.GetProperty(receiver, "length"));
            if (double.IsNaN(length) || length <= 0d)
            {
                return 0;
            }

            length = double.IsPositiveInfinity(length)
                ? 9007199254740991d
                : global::System.Math.Min(global::System.Math.Truncate(length), 9007199254740991d);
            if (length > 4294967295d)
            {
                throw new RangeError("Invalid array length");
            }

            if (length > int.MaxValue)
            {
                throw new RangeError("Array length exceeds runtime limits");
            }

            return (int)length;
        }
    }
}
