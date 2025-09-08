using System;

namespace JavaScriptRuntime
{
    /// <summary>
    /// Minimal Math intrinsic exposing a subset of ECMAScript Math.* functions used by tests/benchmarks.
    /// Numbers are represented as double; arguments are coerced to number via Convert.ToDouble when possible.
    /// </summary>
    [IntrinsicObject("Math")]
    public static class Math
    {
        /// <summary>
        /// Math.ceil(x): returns the smallest integer greater than or equal to x.
        /// For NaN returns NaN; for +/-Infinity returns the same infinity.
        /// </summary>
        public static object ceil(object? x)
        {
            double d = ToNumber(x);
            if (double.IsNaN(d)) return double.NaN;
            if (double.IsPositiveInfinity(d)) return double.PositiveInfinity;
            if (double.IsNegativeInfinity(d)) return double.NegativeInfinity;
            return System.Math.Ceiling(d);
        }

        /// <summary>
        /// Math.sqrt(x): returns the square root of x. If x is negative or NaN, returns NaN. Infinity maps to Infinity.
        /// </summary>
        public static object sqrt(object? x)
        {
            double d = ToNumber(x);
            if (double.IsNaN(d)) return double.NaN;
            if (double.IsNegativeInfinity(d)) return double.NaN; // per JS: sqrt(-Infinity) => NaN
            if (d < 0) return double.NaN;
            return System.Math.Sqrt(d);
        }

        private static double ToNumber(object? x)
        {
            if (x is null) return double.NaN; // undefined => NaN
            switch (x)
            {
                case JsNull: return 0d; // null => +0
                case double dd: return dd;
                case float ff: return ff;
                case int ii: return ii;
                case long ll: return ll;
                case short ss: return ss;
                case byte bb: return bb;
                case bool b: return b ? 1d : 0d;
                case string s:
                    if (double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                        return parsed;
                    return double.NaN;
            }
            try { return Convert.ToDouble(x, System.Globalization.CultureInfo.InvariantCulture); }
            catch { return double.NaN; }
        }
    }
}
