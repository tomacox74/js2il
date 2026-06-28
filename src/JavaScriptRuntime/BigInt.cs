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
    private const double MaxSafeInteger = 9007199254740991d;

    public static object Call()
    {
        // ECMAScript: BigInt() requires an argument; BigInt(undefined) throws.
        throw new TypeError("Cannot convert undefined to a BigInt");
    }

    public static object Call(object? value)
    {
        return ToBigInteger(value);
    }

    public static object AsIntN(object? bits, object? bigint)
    {
        var bitCount = ToIndex(bits);
        var value = ToBigInteger(bigint);

        if (bitCount == 0)
        {
            return BigInteger.Zero;
        }

        if (bitCount > int.MaxValue)
        {
            throw new RangeError("BigInt bit width is out of range");
        }

        var bitWidth = (int)bitCount;
        var modulus = BigInteger.One << bitWidth;
        var modulo = value % modulus;
        if (modulo.Sign < 0)
        {
            modulo += modulus;
        }

        var signedThreshold = BigInteger.One << (bitWidth - 1);
        return modulo >= signedThreshold ? modulo - modulus : modulo;
    }

    public static object AsUintN(object? bits, object? bigint)
    {
        var bitCount = ToIndex(bits);
        var value = ToBigInteger(bigint);

        if (bitCount == 0)
        {
            return BigInteger.Zero;
        }

        if (bitCount > int.MaxValue)
        {
            throw new RangeError("BigInt bit width is out of range");
        }

        var modulus = BigInteger.One << (int)bitCount;
        var modulo = value % modulus;
        return modulo.Sign < 0 ? modulo + modulus : modulo;
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

    internal static bool TryParseStringToBigInt(string? str, out BigInteger value)
    {
        value = BigInteger.Zero;
        if (str == null)
        {
            return false;
        }

        var trimmed = str.Trim();
        if (trimmed.Length == 0)
        {
            value = BigInteger.Zero;
            return true;
        }

        var sign = 1;
        if (trimmed[0] is '+' or '-')
        {
            sign = trimmed[0] == '-' ? -1 : 1;
            trimmed = trimmed[1..];
            if (trimmed.Length == 0)
            {
                return false;
            }

            if (IsNonDecimalPrefix(trimmed))
            {
                return false;
            }
        }

        if (TryParseNonDecimal(trimmed, out var nonDecimal))
        {
            value = sign < 0 ? BigInteger.Negate(nonDecimal) : nonDecimal;
            return true;
        }

        if (!BigInteger.TryParse(trimmed, NumberStyles.None, CultureInfo.InvariantCulture, out var bi))
        {
            return false;
        }

        value = sign < 0 ? BigInteger.Negate(bi) : bi;
        return true;
    }

    private static BigInteger ParseStringToBigInt(string str)
    {
        if (str == null)
        {
            throw new TypeError("Cannot convert string to a BigInt");
        }

        if (TryParseStringToBigInt(str, out var value))
        {
            return value;
        }

        throw new SyntaxError("Cannot convert string to a BigInt");
    }

    private static long ToIndex(object? bits)
    {
        if (bits is null)
        {
            return 0;
        }

        var number = TypeUtilities.ToNumber(bits);
        if (double.IsNaN(number) || number == 0d)
        {
            return 0;
        }

        if (double.IsInfinity(number))
        {
            throw new RangeError("Invalid BigInt bit width");
        }

        var integer = global::System.Math.Truncate(number);
        if (integer < 0d)
        {
            throw new RangeError("Invalid BigInt bit width");
        }

        if (integer > MaxSafeInteger)
        {
            throw new RangeError("Invalid BigInt bit width");
        }

        return (long)integer;
    }

    private static bool TryParseNonDecimal(string trimmed, out BigInteger value)
    {
        value = BigInteger.Zero;
        if (!IsNonDecimalPrefix(trimmed))
        {
            return false;
        }

        var digits = trimmed[2..];
        var radix = GetNonDecimalRadix(trimmed[1]);

        if (digits.Length == 0)
        {
            return false;
        }

        foreach (var ch in digits)
        {
            var digit = ch switch
            {
                >= '0' and <= '9' => ch - '0',
                >= 'a' and <= 'f' => ch - 'a' + 10,
                >= 'A' and <= 'F' => ch - 'A' + 10,
                _ => -1
            };

            if (digit < 0 || digit >= radix)
            {
                value = BigInteger.Zero;
                return false;
            }

            value = (value * radix) + digit;
        }

        return true;
    }

    private static bool IsNonDecimalPrefix(string value)
        => value.Length >= 2 && value[0] == '0' && GetNonDecimalRadix(value[1]) != 0;

    private static int GetNonDecimalRadix(char prefix)
        => prefix switch
        {
            'b' or 'B' => 2,
            'o' or 'O' => 8,
            'x' or 'X' => 16,
            _ => 0
        };
}
