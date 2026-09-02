using System;
using System.Numerics;

namespace JavaScriptRuntime
{
    /// <summary>
    /// ECMAScript Math.* intrinsic. Numbers are represented as double; arguments are coerced via ToNumber.
    /// </summary>
    [IntrinsicObject("Math")]
    public static class Math
    {
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

            // Spec steps 3-4: these ranges must be special-cased rather than folded into the
            // general floor(d + 0.5) formula below, which both loses precision for values just
            // under 0.5 (e.g. 0.5 - Number.EPSILON/4) and yields the wrong signed zero for
            // negative values in [-0.5, -0).
            if (d > 0.0 && d < 0.5) return 0.0;
            if (d < 0.0 && d >= -0.5) return -0.0;

            if (double.IsInteger(d)) return d;

            // Spec step 5: the integral Number closest to d, preferring +Infinity on a tie.
            double floorVal = System.Math.Floor(d);
            double ceilVal = System.Math.Ceiling(d);
            double diffFloor = d - floorVal;
            double diffCeil = ceilVal - d;

            return diffCeil <= diffFloor ? ceilVal : floorVal;
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

            if (double.IsInfinity(dy) && global::System.Math.Abs(dx) == 1.0)
            {
                return double.NaN;
            }

            return System.Math.Pow(dx, dy);
        }

        public static double min(params object?[] args)
        {
            if (args == null || args.Length == 0) return double.PositiveInfinity;
            double min = double.PositiveInfinity;
            bool sawNaN = false;
            foreach (var a in args)
            {
                double d = ToNumber(a);
                if (double.IsNaN(d))
                {
                    sawNaN = true;
                    continue;
                }

                if (d < min || (d == 0.0 && min == 0.0 && double.IsNegative(d) && !double.IsNegative(min)))
                {
                    min = d;
                }
            }
            return sawNaN ? double.NaN : min;
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
            return System.Random.Shared.NextDouble();
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

            // Spec: coerce every argument to Number first (in order), propagating any abrupt
            // completion immediately and without coercing later arguments. Only once every
            // argument has been coerced do we inspect the results for +/-Infinity or NaN.
            var coerced = new double[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                coerced[i] = ToNumber(args[i]);
            }

            foreach (var number in coerced)
            {
                if (double.IsPositiveInfinity(number) || double.IsNegativeInfinity(number)) return double.PositiveInfinity;
            }

            foreach (var number in coerced)
            {
                if (double.IsNaN(number)) return double.NaN;
            }

            double sum = 0.0;
            foreach (var number in coerced)
            {
                sum += number * number;
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
            var state = PreciseSumState.MinusZero;
            var exactSum = BigInteger.Zero;
            long count = 0;

            while (true)
            {
                var step = ObjectRuntime.IteratorNext(iterator);
                if (ObjectRuntime.IteratorResultDone(step))
                {
                    break;
                }

                var value = ObjectRuntime.IteratorResultValue(step);
                count++;
                if (count >= 1L << 53)
                {
                    ObjectRuntime.IteratorCloseForThrowCompletion(iterator);
                    throw new RangeError("Math.sumPrecise input is too large");
                }

                if (!TryGetNumberPrimitive(value, out var number))
                {
                    ObjectRuntime.IteratorCloseForThrowCompletion(iterator);
                    throw new TypeError("Math.sumPrecise values must be Numbers");
                }

                if (state == PreciseSumState.NotANumber)
                {
                    continue;
                }

                if (double.IsNaN(number))
                {
                    state = PreciseSumState.NotANumber;
                }
                else if (double.IsPositiveInfinity(number))
                {
                    state = state == PreciseSumState.MinusInfinity
                        ? PreciseSumState.NotANumber
                        : PreciseSumState.PlusInfinity;
                }
                else if (double.IsNegativeInfinity(number))
                {
                    state = state == PreciseSumState.PlusInfinity
                        ? PreciseSumState.NotANumber
                        : PreciseSumState.MinusInfinity;
                }
                else if (!(number == 0.0 && double.IsNegative(number))
                    && state is PreciseSumState.MinusZero or PreciseSumState.Finite)
                {
                    state = PreciseSumState.Finite;
                    exactSum += ToExactBinaryUnits(number);
                }
            }

            return state switch
            {
                PreciseSumState.NotANumber => double.NaN,
                PreciseSumState.PlusInfinity => double.PositiveInfinity,
                PreciseSumState.MinusInfinity => double.NegativeInfinity,
                PreciseSumState.MinusZero => -0.0,
                _ => RoundExactBinaryUnits(exactSum)
            };
        }

        private static BigInteger ToExactBinaryUnits(double value)
        {
            var bits = unchecked((ulong)BitConverter.DoubleToInt64Bits(value));
            var negative = (bits & (1UL << 63)) != 0;
            var exponentBits = (int)((bits >> 52) & 0x7ff);
            var significand = bits & ((1UL << 52) - 1);
            if (exponentBits != 0)
            {
                significand |= 1UL << 52;
            }

            var units = new BigInteger(significand);
            if (exponentBits > 1)
            {
                units <<= exponentBits - 1;
            }

            return negative ? -units : units;
        }

        private static double RoundExactBinaryUnits(BigInteger units)
        {
            if (units.IsZero)
            {
                return 0.0;
            }

            var negative = units.Sign < 0;
            var magnitude = BigInteger.Abs(units);
            var bitLength = checked((int)magnitude.GetBitLength());
            var shift = global::System.Math.Max(0, bitLength - 53);
            var roundedSignificand = magnitude >> shift;

            if (shift > 0)
            {
                var remainder = magnitude - (roundedSignificand << shift);
                var halfway = BigInteger.One << (shift - 1);
                if (remainder > halfway || (remainder == halfway && !roundedSignificand.IsEven))
                {
                    roundedSignificand++;
                    if (roundedSignificand.GetBitLength() > 53)
                    {
                        roundedSignificand >>= 1;
                        shift++;
                    }
                }
            }

            var rounded = global::System.Math.ScaleB((double)roundedSignificand, shift - 1074);
            return negative ? -rounded : rounded;
        }

        private static bool TryGetNumberPrimitive(object? value, out double number)
        {
            switch (value)
            {
                case double d:
                    number = d;
                    return true;
                case float f:
                    number = f;
                    return true;
                case int i:
                    number = i;
                    return true;
                case long l:
                    number = l;
                    return true;
                case short s:
                    number = s;
                    return true;
                case byte b:
                    number = b;
                    return true;
                case sbyte sb:
                    number = sb;
                    return true;
                case uint ui:
                    number = ui;
                    return true;
                case ulong ul:
                    number = ul;
                    return true;
                case ushort us:
                    number = us;
                    return true;
                default:
                    number = double.NaN;
                    return false;
            }
        }

        private enum PreciseSumState
        {
            MinusZero,
            Finite,
            PlusInfinity,
            MinusInfinity,
            NotANumber
        }

        private static double ToNumber(object? x) => TypeUtilities.ToNumber(x);

        private static int ToInt32(object? x) => TypeUtilities.ToInt32(x);

        private static uint ToUint32(object? x) => TypeUtilities.ToUint32(x);
    }
}
