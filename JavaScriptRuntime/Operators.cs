using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime
{
    public static class Operators
    {
        private const string MixedBigIntTypeError = "Cannot mix BigInt and other types, use explicit conversions";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsBigInt(object? value) => value is BigInteger;

        private static bool IsFiniteInteger(double value)
            => !double.IsNaN(value) && !double.IsInfinity(value) && global::System.Math.Truncate(value) == value;

        private static int ToShiftCount(BigInteger shiftCount)
        {
            if (shiftCount > int.MaxValue || shiftCount < int.MinValue)
            {
                throw new RangeError("BigInt shift count is out of range");
            }

            return (int)shiftCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ToInt32FromDouble(double number)
        {
            if (double.IsNaN(number) || double.IsInfinity(number) || number == 0.0)
            {
                return 0;
            }

            return unchecked((int)(long)number);
        }

        [DoesNotReturn]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowMixedBigIntTypeError()
        {
            throw new TypeError(MixedBigIntTypeError);
        }

        private static bool BigIntLooseEquals(BigInteger left, object? right)
        {
            var rightNumber = ToNumber(right);
            if (!IsFiniteInteger(rightNumber))
            {
                return false;
            }

            return left == new BigInteger(rightNumber);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ToNumber(object? value)
        {
            // JS ToNumber(undefined) => NaN (undefined is represented as CLR null)
            if (value == null)
                return double.NaN;
            switch (value)
            {
                case double d:
                    return d;
                case float f:
                    return (double)f;
                case int i:
                    return i;
                case long l:
                    return l;
                case short s:
                    return s;
                case byte b:
                    return b;
                case bool bo:
                    return bo ? 1d : 0d;
                case string str:
                    // JS ToNumber on strings: trim; empty -> 0; 0x.. hex allowed; otherwise parse as float
                    var trimmed = str.Trim();
                    if (trimmed.Length == 0)
                        return 0d;
                    if (trimmed.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        if (long.TryParse(trimmed.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var hex))
                            return (double)hex;
                        return double.NaN;
                    }
                    return double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
                        ? parsed
                        : double.NaN;
                case JsNull:
                    return 0d;
                case BigInteger:
                    throw new TypeError("Cannot convert a BigInt value to a number");
            }
            try
            {
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return double.NaN;
            }
        }
        /// <summary>
        /// Implements JavaScript '+' semantics for two operands boxed as objects.
        /// - If either operand is a string, both are coerced to string and concatenated.
        /// - Otherwise, both are coerced to numbers and added (as double).
        /// This is a minimal subset sufficient for current tests (strings and numbers).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Add(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                return leftDouble + rightDouble;
            }

            // If either is a string, concatenate string representations
            if (a is string || b is string)
            {
                var sa = DotNet2JSConversions.ToString(a);
                var sb = DotNet2JSConversions.ToString(b);
                return string.Concat(sa, sb);
            }

            if (a is BigInteger leftBigInt)
            {
                if (b is BigInteger rightBigInt)
                {
                    return leftBigInt + rightBigInt;
                }

                ThrowMixedBigIntTypeError();
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            double da = ToNumber(a);
            double db = ToNumber(b);
            return da + db;
        }

        /// <summary>
        /// Implements JavaScript '+' semantics where the left operand is already an unboxed double.
        /// Avoids boxing the double in common numeric hot paths.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Add(double a, object? b)
        {
            if (b is double db)
            {
                return a + db;
            }

            if (b is string)
            {
                var sa = DotNet2JSConversions.ToString(a);
                var sb = DotNet2JSConversions.ToString(b);
                return string.Concat(sa, sb);
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            var numberB = ToNumber(b);
            return a + numberB; // boxed double
        }

        /// <summary>
        /// Implements JavaScript '+' semantics where the right operand is already an unboxed double.
        /// Avoids boxing the double in common numeric hot paths.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Add(object? a, double b)
        {
            if (a is double da)
            {
                return da + b;
            }

            if (a is string)
            {
                var sa = DotNet2JSConversions.ToString(a);
                var sb = DotNet2JSConversions.ToString(b);
                return string.Concat(sa, sb);
            }

            if (a is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            var numberA = ToNumber(a);
            return numberA + b; // boxed double
        }

        /// <summary>
        /// Performs JavaScript '+' and immediately applies ToNumber to the result.
        /// Used by compiler hot paths that would otherwise emit Add(...) then ToNumber(...).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AddAndToNumber(object? left, object? right)
        {
            if (left is double leftDouble)
            {
                return AddAndToNumber(leftDouble, right);
            }

            if (right is double rightDouble)
            {
                return AddAndToNumber(left, rightDouble);
            }

            if (left is string || right is string)
            {
                var concatenated = string.Concat(DotNet2JSConversions.ToString(left), DotNet2JSConversions.ToString(right));
                return TypeUtilities.ToNumber(concatenated);
            }

            // Preserve JavaScript BigInt '+' error semantics/messages through existing operator helper.
            if (left is BigInteger || right is BigInteger)
            {
                return TypeUtilities.ToNumber(Add(left, right));
            }

            return TypeUtilities.ToNumber(left) + TypeUtilities.ToNumber(right);
        }

        /// <summary>
        /// Performs JavaScript '+' with an unboxed numeric left operand and immediately applies ToNumber.
        /// Preserves the mixed fast-path without boxing the left operand before Add.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AddAndToNumber(double left, object? right)
        {
            if (right is double rightDouble)
            {
                return left + rightDouble;
            }

            if (right is string)
            {
                var concatenated = string.Concat(DotNet2JSConversions.ToString(left), DotNet2JSConversions.ToString(right));
                return TypeUtilities.ToNumber(concatenated);
            }

            if (right is BigInteger)
            {
                return TypeUtilities.ToNumber(Add(left, right));
            }

            return left + TypeUtilities.ToNumber(right);
        }

        /// <summary>
        /// Performs JavaScript '+' with an unboxed numeric right operand and immediately applies ToNumber.
        /// Preserves the mixed fast-path without boxing the right operand before Add.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AddAndToNumber(object? left, double right)
        {
            if (left is double leftDouble)
            {
                return leftDouble + right;
            }

            if (left is string)
            {
                var concatenated = string.Concat(DotNet2JSConversions.ToString(left), DotNet2JSConversions.ToString(right));
                return TypeUtilities.ToNumber(concatenated);
            }

            if (left is BigInteger)
            {
                return TypeUtilities.ToNumber(Add(left, right));
            }

            return TypeUtilities.ToNumber(left) + right;
        }

        /// <summary>
        /// Implements JavaScript '-' semantics. Both operands are coerced to numbers; result is a double (may be NaN).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Subtract(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                return leftDouble - rightDouble;
            }

            if (a is BigInteger leftBigInt)
            {
                if (b is BigInteger rightBigInt)
                {
                    return leftBigInt - rightBigInt;
                }

                ThrowMixedBigIntTypeError();
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            var da = ToNumber(a);
            var db = ToNumber(b);
            return da - db;
        }

        /// <summary>
        /// Implements JavaScript '*' semantics. Both operands are coerced to numbers; result is a double (may be NaN).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Multiply(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                return leftDouble * rightDouble;
            }

            if (a is BigInteger leftBigInt)
            {
                if (b is BigInteger rightBigInt)
                {
                    return leftBigInt * rightBigInt;
                }

                ThrowMixedBigIntTypeError();
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            var da = ToNumber(a);
            var db = ToNumber(b);
            return da * db;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Divide(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                return leftDouble / rightDouble;
            }

            if (a is BigInteger leftBigInt)
            {
                if (b is BigInteger rightBigInt)
                {
                    if (rightBigInt == BigInteger.Zero)
                    {
                        throw new RangeError("Division by zero");
                    }

                    return leftBigInt / rightBigInt;
                }

                ThrowMixedBigIntTypeError();
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            var da = ToNumber(a);
            var db = ToNumber(b);
            return da / db;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Remainder(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                return leftDouble % rightDouble;
            }

            if (a is BigInteger leftBigInt)
            {
                if (b is BigInteger rightBigInt)
                {
                    if (rightBigInt == BigInteger.Zero)
                    {
                        throw new RangeError("Division by zero");
                    }

                    return leftBigInt % rightBigInt;
                }

                ThrowMixedBigIntTypeError();
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            var da = ToNumber(a);
            var db = ToNumber(b);
            return da % db;
        }

        public static object Exponentiate(object? a, object? b)
        {
            if (a is BigInteger leftBigInt && b is BigInteger rightBigInt)
            {
                if (rightBigInt < BigInteger.Zero)
                {
                    throw new RangeError("Exponent must be positive");
                }

                if (rightBigInt > int.MaxValue)
                {
                    throw new RangeError("BigInt exponent is out of range");
                }

                return BigInteger.Pow(leftBigInt, (int)rightBigInt);
            }

            if (IsBigInt(a) || IsBigInt(b))
            {
                ThrowMixedBigIntTypeError();
            }

            var da = ToNumber(a);
            var db = ToNumber(b);
            return global::System.Math.Pow(da, db);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object BitwiseAnd(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                var leftInt = ToInt32FromDouble(leftDouble);
                var rightInt = ToInt32FromDouble(rightDouble);
                return (double)(leftInt & rightInt);
            }

            if (a is BigInteger leftBigInt)
            {
                if (b is BigInteger rightBigInt)
                {
                    return leftBigInt & rightBigInt;
                }

                ThrowMixedBigIntTypeError();
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            var left = TypeUtilities.ToInt32(a);
            var right = TypeUtilities.ToInt32(b);
            return (double)(left & right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object BitwiseOr(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                var leftInt = ToInt32FromDouble(leftDouble);
                var rightInt = ToInt32FromDouble(rightDouble);
                return (double)(leftInt | rightInt);
            }

            if (a is BigInteger leftBigInt)
            {
                if (b is BigInteger rightBigInt)
                {
                    return leftBigInt | rightBigInt;
                }

                ThrowMixedBigIntTypeError();
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            var left = TypeUtilities.ToInt32(a);
            var right = TypeUtilities.ToInt32(b);
            return (double)(left | right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object BitwiseXor(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                var leftInt = ToInt32FromDouble(leftDouble);
                var rightInt = ToInt32FromDouble(rightDouble);
                return (double)(leftInt ^ rightInt);
            }

            if (a is BigInteger leftBigInt)
            {
                if (b is BigInteger rightBigInt)
                {
                    return leftBigInt ^ rightBigInt;
                }

                ThrowMixedBigIntTypeError();
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            var left = TypeUtilities.ToInt32(a);
            var right = TypeUtilities.ToInt32(b);
            return (double)(left ^ right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object LeftShift(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                var leftInt = ToInt32FromDouble(leftDouble);
                var rightInt = ToInt32FromDouble(rightDouble) & 0x1f;
                return (double)(leftInt << rightInt);
            }

            if (a is BigInteger leftBigInt)
            {
                if (b is BigInteger rightBigInt)
                {
                    var shiftCount = ToShiftCount(rightBigInt);
                    return shiftCount >= 0
                        ? leftBigInt << shiftCount
                        : leftBigInt >> -shiftCount;
                }

                ThrowMixedBigIntTypeError();
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            var left = TypeUtilities.ToInt32(a);
            var right = TypeUtilities.ToInt32(b) & 0x1f;
            return (double)(left << right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object SignedRightShift(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                var leftInt = ToInt32FromDouble(leftDouble);
                var rightInt = ToInt32FromDouble(rightDouble) & 0x1f;
                return (double)(leftInt >> rightInt);
            }

            if (a is BigInteger leftBigInt)
            {
                if (b is BigInteger rightBigInt)
                {
                    var shiftCount = ToShiftCount(rightBigInt);
                    return shiftCount >= 0
                        ? leftBigInt >> shiftCount
                        : leftBigInt << -shiftCount;
                }

                ThrowMixedBigIntTypeError();
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            var left = TypeUtilities.ToInt32(a);
            var right = TypeUtilities.ToInt32(b) & 0x1f;
            return (double)(left >> right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object UnsignedRightShift(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                var leftUint = unchecked((uint)ToInt32FromDouble(leftDouble));
                var rightInt = ToInt32FromDouble(rightDouble) & 0x1f;
                return (double)(leftUint >> rightInt);
            }

            if (a is BigInteger && b is BigInteger)
            {
                throw new TypeError("BigInts have no unsigned right shift, use >> instead");
            }

            if (a is BigInteger || b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            var left = unchecked((uint)TypeUtilities.ToInt32(a));
            var right = TypeUtilities.ToInt32(b) & 0x1f;
            return (double)(left >> right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessThan(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                return leftDouble < rightDouble;
            }

            if (a is BigInteger leftBigInt)
            {
                if (b is BigInteger rightBigInt)
                {
                    return leftBigInt < rightBigInt;
                }

                ThrowMixedBigIntTypeError();
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            return ToNumber(a) < ToNumber(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterThan(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                return leftDouble > rightDouble;
            }

            if (a is BigInteger leftBigInt)
            {
                if (b is BigInteger rightBigInt)
                {
                    return leftBigInt > rightBigInt;
                }

                ThrowMixedBigIntTypeError();
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            return ToNumber(a) > ToNumber(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessThanOrEqual(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                return leftDouble <= rightDouble;
            }

            if (a is BigInteger leftBigInt)
            {
                if (b is BigInteger rightBigInt)
                {
                    return leftBigInt <= rightBigInt;
                }

                ThrowMixedBigIntTypeError();
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            return ToNumber(a) <= ToNumber(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterThanOrEqual(object? a, object? b)
        {
            if (a is double leftDouble && b is double rightDouble)
            {
                return leftDouble >= rightDouble;
            }

            if (a is BigInteger leftBigInt)
            {
                if (b is BigInteger rightBigInt)
                {
                    return leftBigInt >= rightBigInt;
                }

                ThrowMixedBigIntTypeError();
            }

            if (b is BigInteger)
            {
                ThrowMixedBigIntTypeError();
            }

            return ToNumber(a) >= ToNumber(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object UnaryMinus(object? value)
        {
            if (value is double number)
            {
                return -number;
            }

            if (value is BigInteger bigInt)
            {
                return -bigInt;
            }

            return -ToNumber(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object BitwiseNot(object? value)
        {
            if (value is double number)
            {
                return (double)(~ToInt32FromDouble(number));
            }

            if (value is BigInteger bigInt)
            {
                return ~bigInt;
            }

            var intValue = TypeUtilities.ToInt32(value);
            return (double)(~intValue);
        }

        public static bool SameValue(object? a, object? b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a is double da && b is double db)
            {
                if (double.IsNaN(da) && double.IsNaN(db))
                {
                    return true;
                }

                if (da == 0d && db == 0d)
                {
                    return double.IsNegative(da) == double.IsNegative(db);
                }

                return da == db;
            }

            if (a is BigInteger bigIntA && b is BigInteger bigIntB)
            {
                return bigIntA == bigIntB;
            }

            if (a == null || b == null)
            {
                return a == null && b == null;
            }

            if (a.GetType() != b.GetType())
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool SameValueZero(object? a, object? b)
        {
            if (a is double da && b is double db)
            {
                if (double.IsNaN(da) && double.IsNaN(db))
                {
                    return true;
                }

                return da == db;
            }

            return SameValue(a, b);
        }

        /// <summary>
        /// Implements JavaScript '==' (loose equality) semantics.
        /// </summary>
        public static bool Equal(object? a, object? b)
        {
            // Identical references
            if (ReferenceEquals(a, b))
                return true;

            // JavaScript null/undefined special-case: null == undefined is true,
            // but null/undefined are not equal to any other value.
            var aIsNullish = a == null || a is JsNull;
            var bIsNullish = b == null || b is JsNull;
            if (aIsNullish || bIsNullish)
                return aIsNullish && bIsNullish;

            // Type coercion: compare as numbers if both can be numeric
            // Note: JavaScript uses IEEE 754 floating-point comparison semantics where
            // direct equality (==) is intentional and matches JS behavior (NaN != NaN, etc.)
            if (a is double da && b is double db)
                return da == db;

            if (a is BigInteger leftBigInt && b is BigInteger rightBigInt)
                return leftBigInt == rightBigInt;

            if (a is BigInteger leftBigIntLoose)
                return BigIntLooseEquals(leftBigIntLoose, b);

            if (b is BigInteger rightBigIntLoose)
                return BigIntLooseEquals(rightBigIntLoose, a);

            if (a is int ia && b is int ib)
                return ia == ib;

            if (a is bool ba && b is bool bb)
                return ba == bb;

            // String comparison
            if (a is string sa && b is string sb)
                return sa == sb;

            // Mixed types: numeric comparison when both are convertible
            if (CanConvertToNumber(a) && CanConvertToNumber(b))
            {
                var na = ToNumber(a);
                var nb = ToNumber(b);
                return na == nb;
            }

            return false;
        }

        /// <summary>
        /// Checks if a value can be converted to a number without throwing.
        /// </summary>
        private static bool CanConvertToNumber(object? value)
        {
            return value == null
                   || value is JsNull
                   || value is bool
                   || value is int
                   || value is double
                   || value is string;
        }

        /// <summary>
        /// Implements JavaScript '!=' (loose inequality) semantics.
        /// </summary>
        public static bool NotEqual(object? a, object? b)
        {
            return !Equal(a, b);
        }

        /// <summary>
        /// Implements JavaScript '===' (strict equality) semantics.
        /// </summary>
        public static bool StrictEqual(object? a, object? b)
        {
            // Identical references
            if (ReferenceEquals(a, b))
                return true;

            // Must be same type
            if (a == null || b == null)
                return a == null && b == null;

            if (a.GetType() != b.GetType())
            {
                // Special case: int and double can be compared
                // Note: JavaScript uses IEEE 754 floating-point comparison semantics
                if ((a is double || a is int) && (b is double || b is int))
                {
                    return ToNumber(a) == ToNumber(b);
                }
                return false;
            }

            // Same type, value comparison
            return a.Equals(b);
        }

        /// <summary>
        /// Implements JavaScript '!==' (strict inequality) semantics.
        /// </summary>
        public static bool StrictNotEqual(object? a, object? b)
        {
            return !StrictEqual(a, b);
        }

        /// <summary>
        /// Implements JavaScript 'in' operator. Checks if property exists in object.
        /// </summary>
        /// <remarks>
        /// Performance note: For objects that are not dictionaries, arrays, or ExpandoObject,
        /// this method falls back to reflection which can be slow. The reflection results
        /// are not cached, so avoid using 'in' operator in hot paths with CLR objects.
        /// Consider using dictionary-based objects for performance-critical code.
        /// </remarks>
        public static bool In(object? property, object? obj)
        {
            if (obj == null)
                return false;

            // Proxy has trap
            if (obj is JavaScriptRuntime.Proxy proxy)
            {
                // Convert property to string key (minimal; symbols not yet surfaced here)
                var proxyPropName = DotNet2JSConversions.ToString(property);

                var hasTrap = JavaScriptRuntime.ObjectRuntime.GetProperty(proxy.Handler, "has");
                if (hasTrap is not null && hasTrap is not JsNull)
                {
                    var prev = RuntimeServices.SetCurrentThis(proxy.Handler);
                    try
                    {
                        var trapResult = Closure.InvokeWithArgs(hasTrap, System.Array.Empty<object>(), new object?[] { proxy.Target, proxyPropName });
                        return TypeUtilities.ToBoolean(trapResult);
                    }
                    finally
                    {
                        RuntimeServices.SetCurrentThis(prev);
                    }
                }

                // Fallback: apply normal 'in' semantics to the proxy target.
                return In(proxyPropName, proxy.Target);
            }

            // Convert property to string
            var propName = DotNet2JSConversions.ToString(property);
            
            static bool HasOwn(object target, string name)
            {
                if (target is System.Collections.IDictionary dict)
                {
                    return dict.Contains(name);
                }

                // For arrays, check if index exists
                if (target is object?[] array)
                {
                    if (int.TryParse(name, out var index))
                    {
                        return index >= 0 && index < array.Length;
                    }
                    return false;
                }

                // For generic objects (ExpandoObject, JsObject, IDictionary-backed objects)
                if (target is System.Collections.Generic.IDictionary<string, object?> dictGeneric)
                {
                    return dictGeneric.ContainsKey(name);
                }

                // Fallback: check using reflection (not cached - see performance note above)
                var type = target.GetType();
                var prop = type.GetProperty(name);
                if (prop != null)
                    return true;
                var field = type.GetField(name);
                return field != null;
            }

            if (HasOwn(obj, propName))
            {
                return true;
            }

            if (!JavaScriptRuntime.PrototypeChain.Enabled)
            {
                return false;
            }

            // Avoid allocating cycle-detection state for the common case where no prototype
            // has been assigned.
            var current = obj;
            var proto = JavaScriptRuntime.PrototypeChain.GetPrototypeOrNull(current);
            if (proto is null || proto is JsNull)
            {
                return false;
            }

            if (ReferenceEquals(proto, obj))
            {
                return false;
            }

            if (HasOwn(proto, propName))
            {
                return true;
            }

            current = proto;
            proto = JavaScriptRuntime.PrototypeChain.GetPrototypeOrNull(current);
            if (proto is null || proto is JsNull)
            {
                return false;
            }

            var visited = new System.Collections.Generic.HashSet<object>(System.Collections.Generic.ReferenceEqualityComparer.Instance)
            {
                obj,
                current
            };

            while (true)
            {
                if (!visited.Add(proto))
                {
                    return false;
                }

                if (HasOwn(proto, propName))
                {
                    return true;
                }

                current = proto;
                proto = JavaScriptRuntime.PrototypeChain.GetPrototypeOrNull(current);
                if (proto is null || proto is JsNull)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Implements JavaScript <c>instanceof</c> operator.
        /// Minimal semantics: checks whether <paramref name="value"/>'s prototype chain contains <c>ctor.prototype</c>.
        /// </summary>
        public static bool InstanceOf(object? value, object? ctor)
        {
            // Primitives (including undefined/null) are never instances.
            if (value is null) return false;
            if (value is JsNull) return false;
            if (value is string) return false;
            if (value.GetType().IsValueType) return false;

            if (ctor is null || ctor is JsNull)
            {
                throw new TypeError("Right-hand side of 'instanceof' is not callable");
            }

            // Minimal: require a callable delegate-backed function value.
            if (ctor is not Delegate)
            {
                throw new TypeError("Right-hand side of 'instanceof' is not callable");
            }

            // Spec: let proto = ctor.prototype; if proto is not an object, throw.
            var proto = JavaScriptRuntime.ObjectRuntime.GetItem(ctor, "prototype");
            if (proto is null || proto is JsNull || proto is string || proto.GetType().IsValueType)
            {
                throw new TypeError("Function has non-object prototype in instanceof check");
            }

            if (!JavaScriptRuntime.PrototypeChain.Enabled)
            {
                // If prototype chains are not enabled/assigned, we cannot observe any inheritance.
                return false;
            }

            var current = value;
            var next = JavaScriptRuntime.PrototypeChain.GetPrototypeOrNull(current);
            if (next is null || next is JsNull)
            {
                return false;
            }

            if (ReferenceEquals(next, proto))
            {
                return true;
            }

            if (ReferenceEquals(next, current))
            {
                return false;
            }

            current = next;
            next = JavaScriptRuntime.PrototypeChain.GetPrototypeOrNull(current);
            if (next is null || next is JsNull)
            {
                return false;
            }

            var visited = new System.Collections.Generic.HashSet<object>(System.Collections.Generic.ReferenceEqualityComparer.Instance)
            {
                value,
                current
            };

            while (true)
            {
                if (!visited.Add(next))
                {
                    return false;
                }

                if (ReferenceEquals(next, proto))
                {
                    return true;
                }

                current = next;
                next = JavaScriptRuntime.PrototypeChain.GetPrototypeOrNull(current);
                if (next is null || next is JsNull)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Tests if a value is truthy according to JavaScript semantics.
        /// </summary>
        public static bool IsTruthy(double value)
        {
            // JavaScript uses IEEE 754 semantics: 0, -0, and NaN are falsy
            return value != 0 && !double.IsNaN(value);
        }

        public static bool IsTruthy(bool value) => value;

        public static bool IsTruthy(object? value)
        {
            if (value == null)
                return false;
            if (value is JsNull)
                return false;
            if (value is bool b)
                return b;
            // Note: JavaScript uses IEEE 754 semantics - 0, -0, and NaN are falsy
            if (value is double d)
                return d != 0 && !double.IsNaN(d);
            if (value is BigInteger bi)
                return bi != BigInteger.Zero;
            if (value is int i)
                return i != 0;
            if (value is string s)
                return s.Length > 0;
            // Objects are truthy
            return true;
        }
    }
}
