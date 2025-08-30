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
        /// Overload matching intrinsic dispatch that may pass arguments; currently ignores comparator and falls back to default sort.
        /// </summary>
        public object sort(object[] args)
        {
            // TODO: support comparator: if args.Length == 1 and args[0] is a callable, use it to compare
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
    }
}
