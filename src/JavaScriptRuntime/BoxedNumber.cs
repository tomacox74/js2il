using System.Runtime.CompilerServices;

namespace JavaScriptRuntime
{
    /// <summary>
    /// Shared boxes for small integral doubles. JavaScript numbers have no reference identity,
    /// so runtime helpers that must return a boxed <see cref="double"/> can hand out a cached box
    /// instead of allocating a fresh 24-byte object for every loop counter, coordinate, or index.
    /// </summary>
    public static class BoxedNumber
    {
        private const int MinCached = -128;
        private const int MaxCached = 1023;
        private const int CacheSize = MaxCached - MinCached + 1;
        private static readonly object[] Cache = CreateCache();

        private static object[] CreateCache()
        {
            var cache = new object[CacheSize];
            for (var i = 0; i < cache.Length; i++)
            {
                cache[i] = (double)(i + MinCached);
            }

            return cache;
        }

        /// <summary>
        /// Boxes <paramref name="value"/>, reusing a cached box when the value is a small integer.
        /// Negative zero is never cached because it must stay distinguishable from +0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Box(double value)
        {
            var truncated = (int)value;
            if (truncated == value
                && (uint)(truncated - MinCached) < (uint)CacheSize
                && (truncated != 0 || !double.IsNegative(value)))
            {
                return Cache[truncated - MinCached];
            }

            return value;
        }
    }
}
