using System.Globalization;

namespace JavaScriptRuntime
{
    /// <summary>
    /// Minimal Number intrinsic surface needed by tests/runtime.
    /// </summary>
    [IntrinsicObject("Number")]
    public static class Number
    {
        internal const string NumberDataPropertyName = "[[NumberData]]";

        /// <summary>
        /// ECMAScript: Number.isNaN(x) returns true only when x is a Number and is NaN (no coercion).
        /// </summary>
        public static bool isNaN(object? value)
        {
            return value switch
            {
                double d => double.IsNaN(d),
                float f => float.IsNaN(f),
                _ => false
            };
        }

        public static bool isFinite(object? value)
        {
            return value switch
            {
                double d => double.IsFinite(d),
                float f => float.IsFinite(f),
                int or long or short or byte or sbyte or uint or ulong or ushort => true,
                _ => false
            };
        }

        /// <summary>
        /// ECMAScript: Number.isInteger(x) returns true only for finite integral Number values.
        /// </summary>
        public static bool isInteger(object? value)
        {
            return value switch
            {
                double d => double.IsFinite(d) && double.IsInteger(d),
                float f => float.IsFinite(f) && float.IsInteger(f),
                int or long or short or byte or sbyte or uint or ulong or ushort => true,
                _ => false
            };
        }

        /// <summary>
        /// ECMAScript: Number.isSafeInteger(x) returns true only for finite integral Number values
        /// within the IEEE-754 safe integer range [-2^53 + 1, 2^53 - 1].
        /// </summary>
        public static bool isSafeInteger(object? value)
        {
            const double maxSafeInteger = 9007199254740991d;

            return value switch
            {
                double d => double.IsFinite(d) && double.IsInteger(d) && System.Math.Abs(d) <= maxSafeInteger,
                float f => float.IsFinite(f) && float.IsInteger(f) && System.Math.Abs(f) <= maxSafeInteger,
                int or short or byte or sbyte or uint or ushort => true,
                long l => System.Math.Abs((double)l) <= maxSafeInteger,
                ulong ul => ul <= (ulong)maxSafeInteger,
                _ => false
            };
        }

        internal static bool IsNumberConstructor(Delegate candidate)
        {
            ArgumentNullException.ThrowIfNull(candidate);

            var intrinsic = GlobalThis.Number;
            return ReferenceEquals(candidate, intrinsic)
                || (intrinsic is Delegate intrinsicDelegate && candidate.Method == intrinsicDelegate.Method);
        }

        internal static object Construct(object?[]? args, object? newTarget)
        {
            var value = args != null && args.Length > 0
                ? TypeUtilities.ToNumber(args[0])
                : 0d;

            var wrapper = new JsObject();
            var prototype = GlobalThis.NumberPrototypeValue;

            if (newTarget is not null and not JsNull)
            {
                var candidatePrototype = ObjectRuntime.GetItem(newTarget, "prototype");
                if (candidatePrototype is JsNull || TypeUtilities.IsConstructorReturnOverride(candidatePrototype))
                {
                    prototype = candidatePrototype;
                }
            }

            PrototypeChain.SetPrototype(wrapper, prototype);
            PropertyDescriptorStore.DefineOrUpdate(wrapper, NumberDataPropertyName, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = true,
                Value = value
            });

            return wrapper;
        }

        internal static double ThisNumberValue(object? value)
        {
            if (value is double or float or int or long or short or byte or System.Numerics.BigInteger)
            {
                return TypeUtilities.ToNumber(value);
            }

            if (value is not null
                && PropertyDescriptorStore.TryGetOwn(value, NumberDataPropertyName, out var descriptor)
                && descriptor.Kind == JsPropertyDescriptorKind.Data)
            {
                return TypeUtilities.ToNumber(descriptor.Value);
            }

            throw new TypeError("Number.prototype method called on incompatible receiver");
        }

        internal static bool TryGetWrappedNumberValue(object? value, out double numberValue)
        {
            numberValue = default;

            if (value is null || value is JsNull)
            {
                return false;
            }

            if (!PropertyDescriptorStore.TryGetOwn(value, NumberDataPropertyName, out var descriptor)
                || descriptor.Kind != JsPropertyDescriptorKind.Data)
            {
                return false;
            }

            numberValue = TypeUtilities.ToNumber(descriptor.Value);
            return true;
        }

        internal static string ToExponentialString(object? value, object? fractionDigitsArgument)
        {
            var number = ThisNumberValue(value);
            if (double.IsNaN(number)) return "NaN";
            if (double.IsPositiveInfinity(number)) return "Infinity";
            if (double.IsNegativeInfinity(number)) return "-Infinity";

            var fractionDigits = ToDigitsArgument(fractionDigitsArgument, defaultValue: 0, minimum: 0, maximum: 100, "toExponential");
            var formattedNumber = number;
            var absolute = System.Math.Abs(number);
            if (absolute != 0d)
            {
                var exponent = (int)System.Math.Floor(System.Math.Log10(absolute));
                var scaled = absolute * System.Math.Pow(10d, fractionDigits - exponent);
                var floor = System.Math.Floor(scaled);
                var fractionalPart = scaled - floor;
                if (System.Math.Abs(fractionalPart - 0.5d) <= 1e-12)
                {
                    var roundedScaled = floor + 1d;
                    var roundedNormalized = roundedScaled / System.Math.Pow(10d, fractionDigits);
                    if (roundedNormalized >= 10d)
                    {
                        roundedNormalized /= 10d;
                        exponent++;
                    }

                    formattedNumber = System.Math.CopySign(
                        roundedNormalized * System.Math.Pow(10d, exponent),
                        number);
                }
            }

            return NormalizeExponent(formattedNumber.ToString($"e{fractionDigits}", CultureInfo.InvariantCulture), trimMantissaTrailingZeros: false);
        }

        internal static string ToFixedString(object? value, object? fractionDigitsArgument)
        {
            var number = ThisNumberValue(value);
            if (double.IsNaN(number)) return "NaN";
            if (double.IsPositiveInfinity(number)) return "Infinity";
            if (double.IsNegativeInfinity(number)) return "-Infinity";

            var fractionDigits = ToDigitsArgument(fractionDigitsArgument, defaultValue: 0, minimum: 0, maximum: 100, "toFixed");
            return number.ToString($"F{fractionDigits}", CultureInfo.InvariantCulture);
        }

        internal static string ToLocaleStringString(object? value)
        {
            var number = ThisNumberValue(value);
            return DotNet2JSConversions.ToString(number);
        }

        internal static string ToPrecisionString(object? value, object? precisionArgument)
        {
            var number = ThisNumberValue(value);
            if (precisionArgument is null)
            {
                return DotNet2JSConversions.ToString(number);
            }

            if (double.IsNaN(number)) return "NaN";
            if (double.IsPositiveInfinity(number)) return "Infinity";
            if (double.IsNegativeInfinity(number)) return "-Infinity";

            var precision = ToDigitsArgument(precisionArgument, defaultValue: 0, minimum: 1, maximum: 100, "toPrecision");
            var negative = number < 0 || double.IsNegative(number);
            var absolute = System.Math.Abs(number);
            var exponentForm = NormalizeExponent(absolute.ToString($"e{precision - 1}", CultureInfo.InvariantCulture), trimMantissaTrailingZeros: false);
            var exponentIndex = exponentForm.IndexOf('e');
            var mantissa = exponentForm[..exponentIndex];
            var exponent = int.Parse(exponentForm[(exponentIndex + 1)..], CultureInfo.InvariantCulture);
            var digits = mantissa.Replace(".", string.Empty, StringComparison.Ordinal);

            string formatted;
            if (exponent < -6 || exponent >= precision)
            {
                formatted = precision == 1
                    ? $"{digits[0]}e{FormatExponent(exponent)}"
                    : $"{digits[0]}.{digits[1..]}e{FormatExponent(exponent)}";
            }
            else if (exponent >= precision - 1)
            {
                formatted = digits + new string('0', exponent + 1 - digits.Length);
            }
            else if (exponent >= 0)
            {
                formatted = digits[..(exponent + 1)] + "." + digits[(exponent + 1)..];
            }
            else
            {
                formatted = "0." + new string('0', -(exponent + 1)) + digits;
            }

            return negative ? "-" + formatted : formatted;
        }

        private static int ToDigitsArgument(object? argument, int defaultValue, int minimum, int maximum, string methodName)
        {
            if (argument is null)
            {
                return defaultValue;
            }

            var number = TypeUtilities.ToNumber(argument);
            if (double.IsNaN(number))
            {
                return 0;
            }

            var integer = System.Math.Truncate(number);
            if (double.IsInfinity(integer) || integer < minimum || integer > maximum)
            {
                throw new RangeError($"Number.prototype.{methodName} digits argument must be between {minimum} and {maximum}");
            }

            return (int)integer;
        }

        private static string NormalizeExponent(string value, bool trimMantissaTrailingZeros)
        {
            var normalized = value.Replace('E', 'e');
            var exponentIndex = normalized.IndexOf('e');
            if (exponentIndex < 0)
            {
                return normalized;
            }

            var mantissa = normalized[..exponentIndex];
            if (trimMantissaTrailingZeros)
            {
                mantissa = mantissa.TrimEnd('0').TrimEnd('.');
            }

            var exponent = normalized[(exponentIndex + 1)..];
            var sign = string.Empty;
            if (exponent.StartsWith("+", StringComparison.Ordinal) || exponent.StartsWith("-", StringComparison.Ordinal))
            {
                sign = exponent[..1];
                exponent = exponent[1..];
            }

            exponent = exponent.TrimStart('0');
            if (exponent.Length == 0)
            {
                exponent = "0";
            }

            return $"{mantissa}e{sign}{exponent}";
        }

        private static string FormatExponent(int exponent)
            => exponent >= 0
                ? $"+{exponent.ToString(CultureInfo.InvariantCulture)}"
                : exponent.ToString(CultureInfo.InvariantCulture);
    }
}
