using System;
using System.Collections.Generic;
using System.Collections;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Array", IntrinsicCallKind.ArrayConstruct)]
    public class Array : List<object?>
    {
        internal static readonly ExpandoObject Prototype = CreatePrototype();

        private static ExpandoObject CreatePrototype()
        {
            var exp = new ExpandoObject();
            var dict = (IDictionary<string, object?>)exp;
            dict["push"] = (Func<object[], object?[], object?>)PrototypePush;
            return exp;
        }

        private static object PrototypePush(object[] scopes, object?[]? args)
        {
            var receiver = RuntimeServices.GetCurrentThis();
            if (receiver is not JavaScriptRuntime.Array jsArray)
            {
                throw new TypeError("Array.prototype.push called on non-array");
            }

            if (args == null || args.Length == 0)
            {
                return jsArray.push();
            }

            var converted = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                converted[i] = args[i]!;
            }
            return jsArray.push(converted);
        }

        public Array() : base()
        {
        }
        public Array(int capacity) : base(capacity)
        {
        }
        public Array(System.Collections.IEnumerable collection) : base(collection.Cast<object?>())
        {
        }

        // Numeric indexer overload to support compiler intrinsics.
        // Semantics intentionally match JavaScriptRuntime.Object.GetItem/SetItem for Array + numeric index:
        // - Out-of-bounds reads return undefined (null)
        // - Writes extend the array with undefined (null)
        // - Negative indices behave like properties (currently ignored for host safety)
        public object? this[double index]
        {
            get
            {
                int intIndex = (int)index;
                if (intIndex < 0 || intIndex >= Count)
                {
                    return null; // undefined
                }

                return base[intIndex];
            }
            set
            {
                int intIndex;
                if (double.IsNaN(index) || double.IsInfinity(index))
                {
                    intIndex = 0;
                }
                else
                {
                    try { intIndex = (int)index; }
                    catch { intIndex = 0; }
                }

                if (intIndex < 0)
                {
                    JavaScriptRuntime.Object.SetProperty(
                        this,
                        intIndex.ToString(CultureInfo.InvariantCulture),
                        value);
                    return;
                }

                if (intIndex < Count)
                {
                    base[intIndex] = value;
                    return;
                }

                if (intIndex == Count)
                {
                    Add(value);
                    return;
                }

                while (Count < intIndex)
                {
                    Add(null);
                }

                Add(value);
            }
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

                    var result = new Array();
                    for (int i = 0; i < len; i++)
                    {
                        result.Add(null);
                    }
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
            if (constructed.Count > 0)
            {
                this.AddRange(constructed);
            }
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
        public static object from(object? source)
        {
            if (source == null) return new Array();

            // If already a JS array, return a shallow copy
            if (source is Array jsArr)
            {
                return new Array(jsArr);
            }

            // If source is IEnumerable, copy items
            if (source is System.Collections.IEnumerable enumerable)
            {
                var result = new Array();
                foreach (var item in enumerable)
                {
                    result.Add(item!);
                }
                return result;
            }

            // Fallback: wrap single element
            return new Array(new object[] { source });
        }

        /// <summary>
        /// JavaScript Array.isArray(value) static method.
        /// Returns true if the provided value is a JavaScriptRuntime.Array instance; false otherwise.
        /// </summary>
        public static object isArray(object? value)
        {
            return value is Array;
        }

        /// <summary>
        /// JavaScript Array.of(...items)
        /// </summary>
        public static object of(object[]? args)
        {
            return args == null ? new Array() : new Array(args);
        }

        /// <summary>
        /// JavaScript Array.length property
        /// </summary>
        public double length
        {
            get
            {
                return this.Count;
            }
            set
            {
                // Minimal JS semantics for setting length:
                //  - Must be a non-negative integer.
                //  - Truncates/extends the array.
                // NOTE: We clamp to int.MaxValue because this runtime is backed by List<T>.
                double d = value;
                if (double.IsNaN(d) || double.IsInfinity(d) || d < 0)
                {
                    throw new RangeError("Invalid array length");
                }

                // Per ECMAScript, a non-integer length must throw RangeError.
                double truncated = global::System.Math.Truncate(d);
                if (truncated != d)
                {
                    throw new RangeError("Invalid array length");
                }

                d = truncated;
                if (d > int.MaxValue)
                {
                    throw new RangeError("Invalid array length");
                }

                int newLen = (int)d;
                if (newLen < 0)
                {
                    throw new RangeError("Invalid array length");
                }

                if (newLen < this.Count)
                {
                    this.RemoveRange(newLen, this.Count - newLen);
                    return;
                }

                while (this.Count < newLen)
                {
                    this.Add(null);
                }
            }
        }

        /// <summary>
        /// JavaScript Array.sort() default behavior: sorts elements as strings in ascending order and returns the array.
        /// Note: This is a minimal implementation to support tests; comparator overload is ignored if provided.
        /// </summary>
        public object sort()
        {
            this.Sort((a, b) => string.Compare(DotNet2JSConversions.ToString(a), DotNet2JSConversions.ToString(b), StringComparison.Ordinal));
            return this;
        }

        /// <summary>
        /// Overload matching intrinsic dispatch that may pass arguments; supports optional comparator callback.
        /// </summary>
        public object sort(object[] args)
        {
            // If a comparator function is provided, use it; otherwise fallback to default string sort
            if (args != null && args.Length > 0 && args[0] != null)
            {
                var cb = args[0];

                int CompareUsingCallback(object a, object b)
                {
                    object? result;

                    // Support common delegate shapes produced by our compiler/Closure binder
                    if (cb is Func<object[], object, object, object> f2)
                    {
                        result = f2(System.Array.Empty<object>(), a, b);
                    }
                    else if (cb is Func<object[], object, object, object, object> f3)
                    {
                        // Some callsites may pass (a, b, array)
                        result = f3(System.Array.Empty<object>(), a, b, this);
                    }
                    else if (cb is Func<object[], object, object> f1)
                    {
                        // Degenerate: comparator with single arg — treat as default
                        return string.Compare(DotNet2JSConversions.ToString(a), DotNet2JSConversions.ToString(b), StringComparison.Ordinal);
                    }
                    else if (cb is Func<object[], object> f0)
                    {
                        // No-arg function — ignore and use default
                        return string.Compare(DotNet2JSConversions.ToString(a), DotNet2JSConversions.ToString(b), StringComparison.Ordinal);
                    }
                    else
                    {
                        // Unknown type — default
                        return string.Compare(DotNet2JSConversions.ToString(a), DotNet2JSConversions.ToString(b), StringComparison.Ordinal);
                    }

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

                this.Sort((a, b) => CompareUsingCallback(a!, b!));
                return this;
            }

            return sort();
        }

        /// <summary>
        /// JavaScript Array.map(callback[, thisArg])
        /// Minimal implementation: invokes the callback with (value, index, array) when supported and returns a new Array.
        /// Supports delegates produced by Closure.Bind: Func<object[], object, ...> signatures.
        /// </summary>
        public object map(object[] args)
        {
            var result = new Array(this.Count);
            var cb = (args != null && args.Length > 0) ? args[0] : null;

            for (int i = 0; i < this.Count; i++)
            {
                var value = this[i];
                object? mapped;

                if (cb is Func<object?[], object?, object?, object?, object?> f3)
                {
                    // (scopes, value, index, array)
                    mapped = f3(System.Array.Empty<object?>(), value, (double)i, this);
                }
                else if (cb is Func<object?[], object?, object?, object?> f2)
                {
                    // (scopes, value, index)
                    mapped = f2(System.Array.Empty<object>(), value, (double)i);
                }
                else if (cb is Func<object?[], object?, object?> f1)
                {
                    // (scopes, value)
                    mapped = f1(System.Array.Empty<object?>(), value);
                }
                else if (cb is Func<object?[], object?> f0)
                {
                    mapped = f0(System.Array.Empty<object?>());
                }
                else
                {
                    throw new InvalidOperationException("map callback is not a supported function type");
                }

                result.Add(mapped);
            }

            return result;
        }

        private static object? InvokeCallback(object? cb, object? a0 = null, object? a1 = null, object? a2 = null, object? a3 = null)
        {
            // Most array callbacks use up to 4 JS args (value, index, array) or (acc, value, index, array).
            // Our delegate shapes include an extra leading "scopes" parameter.
            var scopes = System.Array.Empty<object?>();

            // Predicates may return bool directly (e.g., filter(Boolean)). Support that shape and box the result.
            if (cb is Func<object?[], object?, object?, object?, object?, bool> b4)
            {
                return b4(scopes, a0, a1, a2, a3);
            }
            if (cb is Func<object?[], object?, object?, object?, bool> b3)
            {
                return b3(scopes, a0, a1, a2);
            }
            if (cb is Func<object?[], object?, object?, bool> b2)
            {
                return b2(scopes, a0, a1);
            }
            if (cb is Func<object?[], object?, bool> b1)
            {
                return b1(scopes, a0);
            }
            if (cb is Func<object?[], bool> b0)
            {
                return b0(scopes);
            }

            if (cb is Func<object?[], object?, object?, object?, object?, object?> f4)
            {
                return f4(scopes, a0, a1, a2, a3);
            }
            if (cb is Func<object?[], object?, object?, object?, object?> f3)
            {
                return f3(scopes, a0, a1, a2);
            }
            if (cb is Func<object?[], object?, object?, object?> f2)
            {
                return f2(scopes, a0, a1);
            }
            if (cb is Func<object?[], object?, object?> f1)
            {
                return f1(scopes, a0);
            }
            if (cb is Func<object?[], object?> f0)
            {
                return f0(scopes);
            }

            throw new InvalidOperationException("array callback is not a supported function type");
        }

        /// <summary>
        /// JavaScript Array.forEach(callback[, thisArg])
        /// </summary>
        public object? forEach(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            for (int i = 0; i < this.Count; i++)
            {
                _ = InvokeCallback(cb, this[i], (double)i, this);
            }
            return null; // undefined
        }

        /// <summary>
        /// JavaScript Array.filter(callback[, thisArg])
        /// </summary>
        public object filter(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            var result = new Array();
            for (int i = 0; i < this.Count; i++)
            {
                var keep = InvokeCallback(cb, this[i], (double)i, this);
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
        public object every(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            for (int i = 0; i < this.Count; i++)
            {
                var ok = InvokeCallback(cb, this[i], (double)i, this);
                if (!Operators.IsTruthy(ok))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// JavaScript Array.reduce(callback[, initialValue])
        /// </summary>
        public object? reduce(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            bool hasInitial = args != null && args.Length > 1;

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

            for (int i = startIndex; i < this.Count; i++)
            {
                acc = InvokeCallback(cb, acc, this[i], (double)i, this);
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

            for (int i = startIndex; i >= 0; i--)
            {
                acc = InvokeCallback(cb, acc, this[i], (double)i, this);
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

            for (int i = 0; i < this.Count; i++)
            {
                var result = InvokeCallback(callback, this[i], (double)i, this);
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
        public object findIndex(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            for (int i = 0; i < this.Count; i++)
            {
                var result = InvokeCallback(cb, this[i], (double)i, this);
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
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            for (int i = this.Count - 1; i >= 0; i--)
            {
                var result = InvokeCallback(cb, this[i], (double)i, this);
                if (Operators.IsTruthy(result))
                {
                    return this[i];
                }
            }
            return null;
        }

        /// <summary>
        /// JavaScript Array.findLastIndex(callback[, thisArg])
        /// Returns the last index matching the predicate, or -1.
        /// </summary>
        public object findLastIndex(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            for (int i = this.Count - 1; i >= 0; i--)
            {
                var result = InvokeCallback(cb, this[i], (double)i, this);
                if (Operators.IsTruthy(result))
                {
                    return (double)i;
                }
            }
            return -1d;
        }

        /// <summary>
        /// JavaScript Array.find(callback[, thisArg])
        /// Minimal implementation: invokes the callback with (value, index, array) and returns the first element for which the callback is truthy.
        /// Returns undefined (null) if none match.
        /// </summary>
        public object? find(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;

            for (int i = 0; i < this.Count; i++)
            {
                var value = this[i];
                object? result;

                if (cb is Func<object?[], object?, object?, object?, object?> f3)
                {
                    // (scopes, value, index, array)
                    result = f3(System.Array.Empty<object?>(), value, (double)i, this);
                }
                else if (cb is Func<object?[], object?, object?, object?> f2)
                {
                    // (scopes, value, index)
                    result = f2(System.Array.Empty<object?>(), value, (double)i);
                }
                else if (cb is Func<object?[], object?, object?> f1)
                {
                    // (scopes, value)
                    result = f1(System.Array.Empty<object?>(), value);
                }
                else if (cb is Func<object?[], object?> f0)
                {
                    result = f0(System.Array.Empty<object?>());
                }
                else
                {
                    throw new InvalidOperationException("find callback is not a supported function type");
                }

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
        public object indexOf(object[]? args)
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

        public object indexOf()
        {
            return -1d;
        }

        /// <summary>
        /// JavaScript Array.lastIndexOf(searchElement[, fromIndex])
        /// Uses strict equality semantics.
        /// </summary>
        public object lastIndexOf(object[]? args)
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

        public object lastIndexOf()
        {
            return -1d;
        }

        /// <summary>
        /// JavaScript Array.at(index)
        /// </summary>
        public object? at(object index)
        {
            int len = this.Count;
            int i = ToInt(index, 0);
            if (i < 0) i = len + i;
            if (i < 0 || i >= len) return null;
            return this[i];
        }

        public object? at()
        {
            return null;
        }

        /// <summary>
        /// JavaScript Array.join([separator]) implementation.
        /// Joins elements by the given separator (default ',') and returns a string.
        /// Each element is converted using DotNet2JSConversions.ToString to approximate JS semantics.
        /// </summary>
        public object join(object[]? args)
        {
            string separator = ",";
            if (args != null && args.Length > 0)
            {
                separator = DotNet2JSConversions.ToString(args[0]);
            }
            if (this.Count == 0) return string.Empty;
            var parts = new System.Collections.Generic.List<string>(this.Count);
            for (int i = 0; i < this.Count; i++)
            {
                var v = this[i];
                parts.Add(DotNet2JSConversions.ToString(v));
            }
            return string.Join(separator, parts);
        }

        /// <summary>
        /// Overload without parameters to match potential direct dispatch.
        /// </summary>
        public object join()
        {
            return join(System.Array.Empty<object>());
        }

        /// <summary>
        /// JavaScript Array.toString()
        /// Minimal: delegates to join(',').
        /// </summary>
        public object toString(object[]? args)
        {
            return join(System.Array.Empty<object>());
        }

        public object toString()
        {
            return join(System.Array.Empty<object>());
        }

        /// <summary>
        /// JavaScript Array.toLocaleString()
        /// Minimal: same as toString for now.
        /// </summary>
        public object toLocaleString(object[]? args)
        {
            return toString();
        }

        public object toLocaleString()
        {
            return toString();
        }

        /// <summary>
        /// JavaScript Array.includes(searchElement[, fromIndex]) implementation.
        /// Uses SameValueZero comparison (NaN equals NaN; +0 and -0 are equal).
        /// </summary>
        public object includes(object[]? args)
        {
            int len = this.Count;
            if (len == 0) return false;

            object? searchElement = (args != null && args.Length > 0) ? args[0] : null;

            // Determine starting index per spec
            int k = 0;
            if (args != null && args.Length > 1 && args[1] != null)
            {
                int fromIndex = 0;
                var idx = args[1];
                if (idx is double dd) fromIndex = (int)dd;
                else if (idx is float ff) fromIndex = (int)ff;
                else if (idx is int ii) fromIndex = ii;
                else if (idx is long ll) fromIndex = (int)ll;
                else if (idx is short ss) fromIndex = ss;
                else if (idx is byte bb) fromIndex = bb;
                else if (idx is string s && double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var pd)) fromIndex = (int)pd;

                if (fromIndex >= 0) k = fromIndex;
                else { k = len + fromIndex; if (k < 0) k = 0; }
                if (k >= len) return false;
            }

            for (int i = k; i < len; i++)
            {
                if (SameValueZero(this[i], searchElement)) return true;
            }
            return false;
        }

        /// <summary>
        /// Overload without parameters; returns false if no search element provided.
        /// </summary>
        public object includes()
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
        public object slice(object[]? args)
        {
            int len = this.Count;

            // Defaults
            int start = 0;
            int end = len;

            // Optional debug: print incoming argument shapes to stderr when enabled
            try
            {
                if (System.Environment.GetEnvironmentVariable("JS2IL_DEBUG_SLICE") == "1")
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
        public object slice()
        {
            return slice(null);
        }

        /// <summary>
        /// Overload for one argument to align with dispatcher arity matching.
        /// </summary>
        public object slice(object start)
        {
            return slice(new object[] { start });
        }

        /// <summary>
        /// Overload for two arguments to align with dispatcher arity matching.
        /// </summary>
        public object slice(object start, object end)
        {
            return slice(new object[] { start, end });
        }

        /// <summary>
        /// JavaScript Array.splice(start[, deleteCount[, item1[, item2[, ...]]]])
        /// Mutates the array by removing and/or inserting elements. Returns a new Array of removed elements.
        /// </summary>
        public object splice(object[]? args)
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

            // Gather removed elements
            var removed = new Array(deleteCount);
            for (int i = 0; i < deleteCount; i++)
            {
                removed.Add(this[start + i]);
            }

            // Remove them from this array
            for (int i = 0; i < deleteCount; i++)
            {
                this.RemoveAt(start);
            }

            // Insert any additional items starting at index 2
            int insertCount = global::System.Math.Max(args.Length - 2, 0);
            if (insertCount > 0)
            {
                var toInsert = new System.Collections.Generic.List<object>(insertCount);
                for (int i = 0; i < insertCount; i++)
                {
                    toInsert.Add(args[2 + i]);
                }
                this.InsertRange(start, toInsert);
            }

            return removed;
        }

        /// <summary>
        /// Overload without parameters
        /// </summary>
        public object splice()
        {
            return splice(null);
        }

        /// <summary>
        /// Overload with start only
        /// </summary>
        public object splice(object start)
        {
            return splice(new object[] { start });
        }

        /// <summary>
        /// Overload with start and deleteCount
        /// </summary>
        public object splice(object start, object deleteCount)
        {
            return splice(new object[] { start, deleteCount });
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
        public object push(object[]? args)
        {
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    this.Add(args[i]);
                }
            }
            // return new length as a JS number (double)
            return (double)this.Count;
        }

        /// <summary>
        /// Overload without parameters to match potential direct dispatch; returns current length.
        /// </summary>
        public object push()
        {
            return (double)this.Count;
        }

        /// <summary>
        /// JavaScript Array.pop(): removes the last element from the array and returns it.
        /// If the array is empty, returns undefined (represented as null in this runtime).
        /// </summary>
        public object? pop(object[]? args)
        {
            if (this.Count == 0)
            {
                return null; // JS undefined
            }
            int lastIndex = this.Count - 1;
            var value = this[lastIndex];
            this.RemoveAt(lastIndex);
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
            if (source == null) return;
            if (source is Array jsArray)
            {
                // Copy elements directly
                for (int i = 0; i < jsArray.Count; i++) this.Add(jsArray[i]);
                return;
            }
            if (source is System.Collections.IEnumerable en)
            {
                foreach (var item in en)
                {
                    this.Add(item!);
                }
                return;
            }
            // Fallback: single item
            this.Add(source);
        }

        /// <summary>
        /// JavaScript Array.shift(): removes and returns first element; returns undefined when empty.
        /// </summary>
        public object? shift(object[]? args)
        {
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
        public object unshift(object[]? args)
        {
            if (args != null && args.Length > 0)
            {
                // Insert preserving order
                for (int i = args.Length - 1; i >= 0; i--)
                {
                    this.Insert(0, args[i]);
                }
            }
            return (double)this.Count;
        }

        public object unshift()
        {
            return (double)this.Count;
        }

        /// <summary>
        /// JavaScript Array.reverse(): in-place reverse.
        /// </summary>
        public object reverse(object[]? args)
        {
            this.Reverse();
            return this;
        }

        public object reverse()
        {
            return reverse(null);
        }

        /// <summary>
        /// JavaScript Array.concat(...items): returns a new array.
        /// </summary>
        public object concat(object[]? args)
        {
            var result = new Array(this);
            if (args == null || args.Length == 0) return result;

            for (int i = 0; i < args.Length; i++)
            {
                var item = args[i];
                if (item is Array arr)
                {
                    for (int j = 0; j < arr.Count; j++) result.Add(arr[j]);
                }
                else
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public object concat()
        {
            return new Array(this);
        }

        /// <summary>
        /// JavaScript Array.fill(value[, start[, end]])
        /// </summary>
        public object fill(object[]? args)
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

        public object fill()
        {
            return this;
        }

        /// <summary>
        /// JavaScript Array.copyWithin(target[, start[, end]])
        /// </summary>
        public object copyWithin(object[]? args)
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

        public object copyWithin()
        {
            return this;
        }

        /// <summary>
        /// JavaScript Array.flat([depth])
        /// </summary>
        public object flat(object[]? args)
        {
            int depth = 1;
            if (args != null && args.Length > 0 && args[0] != null)
            {
                depth = ToInt(args[0], 1);
            }
            if (depth < 0) depth = 0;

            var result = new Array();
            FlattenInto(result, this, depth);
            return result;
        }

        public object flat()
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

        /// <summary>
        /// JavaScript Array.flatMap(callback[, thisArg])
        /// Maps then flattens one level.
        /// </summary>
        public object flatMap(object[] args)
        {
            var cb = (args != null && args.Length > 0) ? args[0] : null;
            var mapped = new Array();

            for (int i = 0; i < this.Count; i++)
            {
                var m = InvokeCallback(cb, this[i], (double)i, this);
                mapped.Add(m);
            }

            return mapped.flat(new object[] { 1d });
        }

        /// <summary>
        /// JavaScript Array.toReversed(): returns a reversed copy.
        /// </summary>
        public object toReversed(object[]? args)
        {
            var copy = new Array(this);
            copy.Reverse();
            return copy;
        }

        public object toReversed()
        {
            return toReversed(null);
        }

        /// <summary>
        /// JavaScript Array.toSorted([compareFn]): returns a sorted copy.
        /// </summary>
        public object toSorted(object[]? args)
        {
            var copy = new Array(this);
            if (args != null && args.Length > 0)
            {
                copy.sort(args!);
            }
            else
            {
                copy.sort();
            }
            return copy;
        }

        public object toSorted()
        {
            return toSorted(null);
        }

        /// <summary>
        /// JavaScript Array.toSpliced(start, deleteCount, ...items): returns a copy with splice applied.
        /// </summary>
        public object toSpliced(object[]? args)
        {
            var copy = new Array(this);
            copy.splice(args);
            return copy;
        }

        /// <summary>
        /// JavaScript Array.with(index, value): returns a copy with element at index replaced.
        /// </summary>
        public object with(object[] args)
        {
            if (args == null || args.Length < 2)
            {
                throw new TypeError("Array.with requires index and value");
            }

            int len = this.Count;
            int index = ToInt(args[0]!, 0);
            if (index < 0) index = len + index;
            if (index < 0 || index >= len)
            {
                throw new RangeError("Invalid index");
            }

            var copy = new Array(this);
            copy[index] = args[1];
            return copy;
        }
    }
}
