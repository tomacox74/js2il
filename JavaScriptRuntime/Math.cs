using System;

namespace JavaScriptRuntime
{
    /// <summary>
    /// ECMAScript Math.* intrinsic. Numbers are represented as double; arguments are coerced via ToNumber.
    /// </summary>
    [IntrinsicObject("Math")]
    public static class Math
    {
        private static readonly System.Random _random = new System.Random();

    // 20.2.1 Value Properties of the Math Object
    public static double E => global::System.Math.E;
    public static double LN10 => global::System.Math.Log(10.0);
    public static double LN2 => global::System.Math.Log(2.0);
    public static double LOG10E => global::System.Math.Log10(global::System.Math.E);
    public static double LOG2E => global::System.Math.Log(global::System.Math.E, 2.0);
    public static double PI => global::System.Math.PI;
    public static double SQRT1_2 => global::System.Math.Sqrt(0.5);
    public static double SQRT2 => global::System.Math.Sqrt(2.0);

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

        public static object abs(object? x)
        {
            double d = ToNumber(x);
            return double.IsNaN(d) ? double.NaN : System.Math.Abs(d);
        }

        public static object floor(object? x)
        {
            double d = ToNumber(x);
            if (double.IsNaN(d)) return double.NaN;
            if (double.IsPositiveInfinity(d)) return double.PositiveInfinity;
            if (double.IsNegativeInfinity(d)) return double.NegativeInfinity;
            return System.Math.Floor(d);
        }

        public static object round(object? x)
        {
            double d = ToNumber(x);
            if (double.IsNaN(d) || double.IsPositiveInfinity(d) || double.IsNegativeInfinity(d)) return d;
            if (d == 0) return d; // preserve +0/-0 when the argument already equals zero

            double r = System.Math.Floor(d + 0.5);
            return r;
        }

        public static object trunc(object? x)
        {
            double d = ToNumber(x);
            if (double.IsNaN(d) || double.IsPositiveInfinity(d) || double.IsNegativeInfinity(d)) return d;
            if (d == 0) return d; // preserve signed zero
            if (d > 0) return System.Math.Floor(d);
            // negative: toward zero
            double res = System.Math.Ceiling(d);
            return res;
        }

        public static object sign(object? x)
        {
            double d = ToNumber(x);
            if (double.IsNaN(d)) return double.NaN;
            if (d == 0)
            {
                // preserve sign of zero
                return 1.0 / d == double.NegativeInfinity ? -0.0 : 0.0;
            }
            return d > 0 ? 1.0 : -1.0;
        }

        public static object sin(object? x) => System.Math.Sin(ToNumber(x));
        public static object cos(object? x) => System.Math.Cos(ToNumber(x));
        public static object tan(object? x) => System.Math.Tan(ToNumber(x));
        public static object asin(object? x) => System.Math.Asin(ToNumber(x));
        public static object acos(object? x) => System.Math.Acos(ToNumber(x));
        public static object atan(object? x) => System.Math.Atan(ToNumber(x));
        public static object atan2(object? y, object? x)
        {
            double dy = ToNumber(y);
            double dx = ToNumber(x);
            return System.Math.Atan2(dy, dx);
        }

        public static object sinh(object? x) => System.Math.Sinh(ToNumber(x));
        public static object cosh(object? x) => System.Math.Cosh(ToNumber(x));
        public static object tanh(object? x) => System.Math.Tanh(ToNumber(x));
        public static object asinh(object? x) => System.Math.Asinh(ToNumber(x));
        public static object acosh(object? x) => System.Math.Acosh(ToNumber(x));
        public static object atanh(object? x) => System.Math.Atanh(ToNumber(x));

        public static object exp(object? x) => System.Math.Exp(ToNumber(x));
        public static object expm1(object? x)
        {
            double d = ToNumber(x);
            // Use Math.Exp - 1; for small values, .NET also has Math.Exp(d) - 1
            return System.Math.Exp(d) - 1.0;
        }
        public static object log(object? x) => System.Math.Log(ToNumber(x));
        public static object log10(object? x) => System.Math.Log10(ToNumber(x));
        public static object log1p(object? x)
        {
            double d = ToNumber(x);
            return System.Math.Log(1.0 + d);
        }
        public static object log2(object? x)
        {
            double d = ToNumber(x);
            return System.Math.Log(d, 2.0);
        }

        public static object pow(object? x, object? y)
        {
            double dx = ToNumber(x);
            double dy = ToNumber(y);
            return System.Math.Pow(dx, dy);
        }

        public static object min(params object?[] args)
        {
            if (args == null || args.Length == 0) return double.PositiveInfinity;
            double min = double.PositiveInfinity;
            foreach (var a in args)
            {
                double d = ToNumber(a);
                if (double.IsNaN(d)) return double.NaN;
                if (d < min) min = d;
            }
            return min;
        }

        public static object max(params object?[] args)
        {
            if (args == null || args.Length == 0) return double.NegativeInfinity;
            double max = double.NegativeInfinity;
            foreach (var a in args)
            {
                double d = ToNumber(a);
                if (double.IsNaN(d)) return double.NaN;
                if (d > max) max = d;
            }
            return max;
        }

        public static object random()
        {
            // 0 <= x < 1
            return _random.NextDouble();
        }

        public static object cbrt(object? x)
        {
            double d = ToNumber(x);
            double r = global::System.Math.Cbrt(d);
            // Snap very-close-to-integer results to the integer to avoid -3.0000000000000004 style outputs
            if (!double.IsNaN(r) && !double.IsInfinity(r))
            {
                double n = global::System.Math.Round(r);
                if (global::System.Math.Abs(r - n) <= 1e-12)
                {
                    return n;
                }
            }
            return r;
        }

        public static object hypot(params object?[] args)
        {
            if (args == null || args.Length == 0) return 0.0;
            bool anyNaN = false;
            foreach (var a in args)
            {
                double d = ToNumber(a);
                if (double.IsPositiveInfinity(d) || double.IsNegativeInfinity(d)) return double.PositiveInfinity;
                if (double.IsNaN(d)) anyNaN = true;
            }
            if (anyNaN) return double.NaN;
            double sum = 0.0;
            foreach (var a in args)
            {
                double d = ToNumber(a);
                sum += d * d;
            }
            return System.Math.Sqrt(sum);
        }

        public static object fround(object? x)
        {
            float f = (float)ToNumber(x);
            double d = f;
            // Preserve -0
            if (d == 0.0 && 1.0 / (double)f == double.NegativeInfinity) return -0.0;
            return d;
        }

        public static object imul(object? a, object? b)
        {
            int x = ToInt32(a);
            int y = ToInt32(b);
            int prod = unchecked(x * y);
            return (double)prod;
        }

        public static object clz32(object? x)
        {
            uint u = ToUint32(x);
            if (u == 0) return 32.0;
            int count = 0;
            for (int i = 31; i >= 0; i--)
            {
                if (((u >> i) & 1u) == 0) count++;
                else break;
            }
            return (double)count;
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

        private static int ToInt32(object? x)
        {
            double d = ToNumber(x);
            if (double.IsNaN(d) || double.IsInfinity(d) || d == 0) return 0;
            // ECMAScript ToInt32: modulo 2^32 into the signed range [-2^31, 2^31-1]
            double two32 = 4294967296.0; // 2^32
            double two31 = 2147483648.0; // 2^31
            double n = System.Math.Floor(System.Math.Abs(d));
            double int32bit = System.Math.IEEERemainder(n, two32);
            if (int32bit < 0) int32bit += two32;
            if (d < 0) int32bit = -int32bit;
            double signed = int32bit >= two31 ? int32bit - two32 : int32bit;
            return (int)signed;
        }

        private static uint ToUint32(object? x)
        {
            double d = ToNumber(x);
            if (double.IsNaN(d) || double.IsInfinity(d) || d == 0) return 0u;
            double two32 = 4294967296.0; // 2^32
            double n = System.Math.Floor(System.Math.Abs(d));
            double uint32bit = n % two32;
            if (d < 0) uint32bit = two32 - uint32bit;
            return (uint)uint32bit;
        }
    }
}
