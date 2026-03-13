using System;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace JavaScriptRuntime;

/// <summary>
/// Minimal BigInt callable intrinsic support.
///
/// Notes:
/// - This is intentionally incomplete; it currently supports basic conversions used by tests.
/// - JS BigInt values are represented as <see cref="BigInteger"/> (boxed as object).
/// </summary>
[IntrinsicObject("BigInt")]
public static class BigInt
{
    private const string Digits = "0123456789abcdefghijklmnopqrstuvwxyz";

    public static object Call()
    {
        // ECMAScript: BigInt() requires an argument; BigInt(undefined) throws.
        throw new TypeError("Cannot convert undefined to a BigInt");
    }

    public static object Call(object? value)
    {
        return ToBigInteger(value);
    }

    public static string ToString(object? value)
    {
        return ToString(value, null);
    }

    public static string ToString(object? value, object? radix)
    {
        var bigInt = ToBigInteger(value);
        var radixValue = 10;
        if (radix is not null)
        {
            var radixNumber = TypeUtilities.ToNumber(radix);
            if (double.IsNaN(radixNumber) || double.IsInfinity(radixNumber))
            {
                throw new RangeError("toString() radix argument must be between 2 and 36");
            }

            radixValue = (int)global::System.Math.Truncate(radixNumber);
            if (radixValue < 2 || radixValue > 36)
            {
                throw new RangeError("toString() radix argument must be between 2 and 36");
            }
        }

        if (radixValue == 10)
        {
            return bigInt.ToString(CultureInfo.InvariantCulture);
        }

        if (bigInt.IsZero)
        {
            return "0";
        }

        var isNegative = bigInt.Sign < 0;
        if (isNegative)
        {
            bigInt = BigInteger.Negate(bigInt);
        }

        var radixBigInt = new BigInteger(radixValue);
        var builder = new StringBuilder();
        while (bigInt > BigInteger.Zero)
        {
            bigInt = BigInteger.DivRem(bigInt, radixBigInt, out var remainder);
            builder.Insert(0, Digits[(int)remainder]);
        }

        if (isNegative)
        {
            builder.Insert(0, '-');
        }

        return builder.ToString();
    }

    private static BigInteger ToBigInteger(object? value)
    {
        if (value is null)
        {
            throw new TypeError("Cannot convert undefined to a BigInt");
        }

        switch (value)
        {
            case BigInteger bi:
                return bi;

            case int i:
                return new BigInteger(i);

            case long l:
                return new BigInteger(l);

            case short s:
                return new BigInteger(s);

            case byte b:
                return new BigInteger(b);

            case double d:
                // Spec: only integral Numbers can be converted.
                if (double.IsNaN(d) || double.IsInfinity(d))
                {
                    throw new TypeError("Cannot convert non-finite number to a BigInt");
                }

                var truncated = global::System.Math.Truncate(d);
                if (truncated != d)
                {
                    throw new RangeError("The number cannot be converted to a BigInt because it is not an integer");
                }

                // Clamp/convert via decimal string to avoid precision surprises for large doubles.
                return BigInteger.Parse(truncated.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

            case float f:
                return ToBigInteger((double)f);

            case string str:
                return ParseStringToBigInt(str);

            case JsNull:
                throw new TypeError("Cannot convert null to a BigInt");

            case bool:
                throw new TypeError("Cannot convert a boolean to a BigInt");

            default:
                // Best-effort: attempt string conversion like JS would for many objects.
                // This is not spec-complete (missing valueOf/toString precedence rules).
                return ParseStringToBigInt(DotNet2JSConversions.ToString(value));
        }
    }

    private static BigInteger ParseStringToBigInt(string str)
    {
        if (str == null)
        {
            throw new TypeError("Cannot convert string to a BigInt");
        }

        var trimmed = str.Trim();
        if (trimmed.Length == 0)
        {
            throw new SyntaxError("Cannot convert empty string to a BigInt");
        }

        // Minimal decimal parsing (supports optional leading +/-).
        // (We can extend to hex/bin/oct literal forms later if needed.)
        if (!BigInteger.TryParse(trimmed, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var bi))
        {
            throw new SyntaxError("Cannot convert string to a BigInt");
        }

        return bi;
    }
}
