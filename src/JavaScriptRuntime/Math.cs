using System;
using System.Collections.Generic;

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
        public static double ceil(double d)
        {
            if (double.IsNaN(d)) return double.NaN;
            if (double.IsPositiveInfinity(d)) return double.PositiveInfinity;
            if (double.IsNegativeInfinity(d)) return double.NegativeInfinity;
            return System.Math.Ceiling(d);
        }

        public static double ceil(object? x) => ceil(ToNumber(x));

        /// <summary>
        /// Math.sqrt(x): returns the square root of x. If x is negative or NaN, returns NaN. Infinity maps to Infinity.
        /// </summary>
        public static double sqrt(double d)
        {
            if (double.IsNaN(d)) return double.NaN;
            if (double.IsNegativeInfinity(d)) return double.NaN; // per JS: sqrt(-Infinity) => NaN
            if (d < 0) return double.NaN;
            return System.Math.Sqrt(d);
        }

        public static double sqrt(object? x) => sqrt(ToNumber(x));

        public static double abs(double d) => double.IsNaN(d) ? double.NaN : System.Math.Abs(d);

        public static double abs(object? x) => abs(ToNumber(x));

        public static double floor(object? x)
        {
            double d = ToNumber(x);
            if (double.IsNaN(d)) return double.NaN;
            if (double.IsPositiveInfinity(d)) return double.PositiveInfinity;
            if (double.IsNegativeInfinity(d)) return double.NegativeInfinity;
            return System.Math.Floor(d);
        }

        public static double round(double d)
        {
            if (double.IsNaN(d) || double.IsPositiveInfinity(d) || double.IsNegativeInfinity(d)) return d;
            if (d == 0) return d; // preserve +0/-0 when the argument already equals zero

            double r = System.Math.Floor(d + 0.5);
            
            // ECMAScript spec: if result is 0 and input was negative, return -0
            if (r == 0.0 && d < 0.0)
            {
                return -0.0;
            }
            
            return r;
        }

        public static double round(object? x) => round(ToNumber(x));

        public static double trunc(object? x)
        {
            double d = ToNumber(x);
            if (double.IsNaN(d) || double.IsPositiveInfinity(d) || double.IsNegativeInfinity(d)) return d;
            if (d == 0) return d; // preserve signed zero
            if (d > 0) return System.Math.Floor(d);
            // negative: toward zero
            double res = System.Math.Ceiling(d);
            return res;
        }

        public static double sign(object? x)
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

        public static double sin(double d) => System.Math.Sin(d);
        public static double sin(object? x) => sin(ToNumber(x));
        public static double cos(double d) => System.Math.Cos(d);
        public static double cos(object? x) => cos(ToNumber(x));
        public static double tan(object? x) => System.Math.Tan(ToNumber(x));
        public static double asin(object? x) => System.Math.Asin(ToNumber(x));
        public static double acos(object? x) => System.Math.Acos(ToNumber(x));
        public static double atan(object? x) => System.Math.Atan(ToNumber(x));
        public static double atan2(object? y, object? x)
        {
            double dy = ToNumber(y);
            double dx = ToNumber(x);
            return System.Math.Atan2(dy, dx);
        }

        public static double sinh(object? x) => System.Math.Sinh(ToNumber(x));
        public static double cosh(object? x) => System.Math.Cosh(ToNumber(x));
        public static double tanh(object? x) => System.Math.Tanh(ToNumber(x));
        public static double asinh(object? x) => System.Math.Asinh(ToNumber(x));
        public static double acosh(object? x) => System.Math.Acosh(ToNumber(x));
        public static double atanh(object? x) => System.Math.Atanh(ToNumber(x));

        public static double exp(object? x) => System.Math.Exp(ToNumber(x));
        public static double expm1(object? x)
        {
            double d = ToNumber(x);
            if (double.IsNaN(d)) return double.NaN;
            if (double.IsNegativeInfinity(d)) return -1.0;
            if (double.IsPositiveInfinity(d)) return double.PositiveInfinity;
            if (d == 0.0) return d; // preserve +0/-0
            return System.Math.Exp(d) - 1.0;
        }
        public static double log(object? x) => System.Math.Log(ToNumber(x));
        public static double log10(object? x) => System.Math.Log10(ToNumber(x));
        public static double log1p(object? x)
        {
            double d = ToNumber(x);
            if (double.IsNaN(d) || d < -1.0) return double.NaN;
            if (d == -1.0) return double.NegativeInfinity;
            if (double.IsPositiveInfinity(d) || d == 0.0) return d; // preserve +0/-0
            return System.Math.Log(1.0 + d);
        }
        public static double log2(object? x)
        {
            double d = ToNumber(x);
            return System.Math.Log(d, 2.0);
        }

        public static double pow(object? x, object? y)
        {
            double dx = ToNumber(x);
            double dy = ToNumber(y);
            if (double.IsNaN(dy))
            {
                return double.NaN;
            }
            return System.Math.Pow(dx, dy);
        }

        public static double min(params object?[] args)
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

        public static double max(params object?[] args)
        {
            if (args == null || args.Length == 0) return double.NegativeInfinity;
            double max = double.NegativeInfinity;
            bool sawNaN = false;
            foreach (var a in args)
            {
                double d = TypeUtilities.ToNumber(a);
                if (double.IsNaN(d))
                {
                    sawNaN = true;
                    continue;
                }

                if (d > max || (d == 0.0 && max == 0.0 && double.IsNegative(max) && !double.IsNegative(d)))
                {
                    max = d;
                }
            }
            return sawNaN ? double.NaN : max;
        }

        public static double random()
        {
            // 0 <= x < 1
            return _random.NextDouble();
        }

        public static double cbrt(object? x)
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

        public static double hypot(params object?[] args)
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

        public static double fround(object? x)
        {
            float f = (float)ToNumber(x);
            double d = f;
            // Preserve -0
            if (d == 0.0 && 1.0 / (double)f == double.NegativeInfinity) return -0.0;
            return d;
        }

        public static double f16round(object? x)
        {
            double d = ToNumber(x);
            Half half = (Half)d;
            double rounded = (double)half;
            if (rounded == 0.0 && double.IsNegative(d))
            {
                return -0.0;
            }

            return rounded;
        }

        public static double imul(object? a, object? b)
        {
            int x = ToInt32(a);
            int y = ToInt32(b);
            int prod = unchecked(x * y);
            return (double)prod;
        }

        public static double clz32(object? x)
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

        public static double sumPrecise(object? items)
        {
            var iterator = ObjectRuntime.GetIterator(items);
            var completedNormally = false;
            var partials = new List<double>();

            try
            {
                while (true)
                {
                    var step = Object.IteratorNext(iterator);
                    if (Object.IteratorResultDone(step))
                    {
                        break;
                    }

                    var value = Object.IteratorResultValue(step);
                    var number = ToNumber(value);
                    if (double.IsNaN(number))
                    {
                        return double.NaN;
                    }

                    AddExactPartial(partials, number);
                }

                completedNormally = true;
            }
            finally
            {
                if (!completedNormally)
                {
                    Object.IteratorClose(iterator);
                }
            }

            return SumExactPartials(partials);
        }

        private static void AddExactPartial(List<double> partials, double value)
        {
            var x = value;
            for (var i = 0; i < partials.Count; i++)
            {
                var y = partials[i];
                if (System.Math.Abs(x) < System.Math.Abs(y))
                {
                    (x, y) = (y, x);
                }

                var hi = x + y;
                var lo = y - (hi - x);
                if (lo != 0.0)
                {
                    partials[i] = lo;
                    x = hi;
                }
                else
                {
                    partials[i] = hi;
                    return;
                }
            }

            partials.Add(x);
        }

        private static double SumExactPartials(List<double> partials)
        {
            if (partials.Count == 0)
            {
                return 0.0;
            }

            var total = 0.0;
            var sawNegativeZero = false;
            foreach (var partial in partials)
            {
                total += partial;
                if (partial == 0.0 && double.IsNegative(partial))
                {
                    sawNegativeZero = true;
                }
            }

            if (total == 0.0 && sawNegativeZero)
            {
                return -0.0;
            }

            return total;
        }

        private static double ToNumber(object? x) => TypeUtilities.ToNumber(x);

        private static int ToInt32(object? x) => TypeUtilities.ToInt32(x);

        private static uint ToUint32(object? x) => TypeUtilities.ToUint32(x);
    }
}
