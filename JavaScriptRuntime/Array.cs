using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaScriptRuntime
{
    public class Array : List<object>
    {
        public Array() : base()
        {
        }
        public Array(int capacity) : base(capacity)
        {
        }
        public Array(IEnumerable<object> collection) : base(collection)
        {
        }

        public static Array Empty => new Array();
        public static implicit operator Array(object[] array)
        {
            return new Array(array);
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

                this.Sort((a, b) => CompareUsingCallback(a, b));
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
                object mapped;

                if (cb is Func<object[], object, object, object, object> f3)
                {
                    // (scopes, value, index, array)
                    mapped = f3(System.Array.Empty<object>(), value, (double)i, this);
                }
                else if (cb is Func<object[], object, object, object> f2)
                {
                    // (scopes, value, index)
                    mapped = f2(System.Array.Empty<object>(), value, (double)i);
                }
                else if (cb is Func<object[], object, object> f1)
                {
                    // (scopes, value)
                    mapped = f1(System.Array.Empty<object>(), value);
                }
                else if (cb is Func<object[], object> f0)
                {
                    mapped = f0(System.Array.Empty<object>());
                }
                else
                {
                    throw new InvalidOperationException("map callback is not a supported function type");
                }

                result.Add(mapped);
            }

            return result;
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
    }
}
