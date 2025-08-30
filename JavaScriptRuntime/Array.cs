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
    }
}
