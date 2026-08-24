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
        return ConvertToBigIntForConstructor(value);
    }

    /// <summary>
    /// ECMAScript BigInt ( value ) [[Construct]] semantics: step 1 requires that if NewTarget
    /// is not undefined, a TypeError is thrown immediately. Since [[Construct]] is only ever
    /// invoked with a defined NewTarget, `new BigInt(...)` (and any other construct-style
    /// invocation, e.g. via Reflect.construct) always throws without evaluating <paramref name="args"/>.
    /// </summary>
    internal static object Construct(object?[]? args, object? newTarget)
    {
        throw new TypeError("BigInt is not a constructor");
    }

    public static object AsIntN(object? bits, object? bigint)
    {
        var bitCount = ToIndex(bits);
        var value = ToBigInt(bigint);

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
        var value = ToBigInt(bigint);

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

    public static string ToLocaleString(object? value)
    {
        return ToString(ThisBigIntValue(value), null);
    }

    public static string ToString(object? value, object? radix)
    {
        // Every caller (BigInt.prototype.toString via ThisBigIntValue, and the raw
        // BigInteger primitive dispatch in ObjectRuntime) already supplies a BigInteger.
        var bigInt = ConvertPrimitiveToBigInt(value);
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

    internal static BigInteger ThisBigIntValue(object? value)
    {
        if (value is BigInteger primitive)
        {
            return primitive;
        }

        if (value is not null
            && PropertyDescriptorStore.TryGetOwn(value, ObjectRuntime.PrimitiveValuePropertyName, out var descriptor)
            && descriptor.Kind == JsPropertyDescriptorKind.Data
            && descriptor.Value is BigInteger wrapped)
        {
            return wrapped;
        }

        throw new TypeError("BigInt.prototype method called on incompatible receiver");
    }

    /// <summary>
    /// BigInt ( value ) semantics (sec-bigint-constructor-number-value):
    /// 1. Let primitive be ? ToPrimitive(value, number).
    /// 2. If primitive is a Number, return ? NumberToBigInt(primitive).
    /// 3. Return ? ToBigInt(primitive).
    /// Unlike the strict <see cref="ToBigInt"/> abstract operation (used by
    /// BigInt.asIntN / BigInt.asUintN), the constructor allows an implicit
    /// Number -&gt; BigInt widening for integral Numbers.
    /// </summary>
    private static BigInteger ConvertToBigIntForConstructor(object? value)
    {
        var primitive = ToPrimitiveForBigInt(value);
        return TryGetNumberValue(primitive, out var numberValue)
            ? NumberToBigInt(numberValue)
            : ConvertPrimitiveToBigInt(primitive);
    }

    /// <summary>
    /// ECMA-262 ToBigInt ( argument ) abstract operation, used directly by
    /// BigInt.asIntN / BigInt.asUintN. Unlike the BigInt(value) constructor, this
    /// operation never performs an implicit Number -&gt; BigInt widening: any Number
    /// primitive (including one unboxed from a Number wrapper object) throws a
    /// TypeError instead of being converted.
    /// </summary>
    private static BigInteger ToBigInt(object? value)
    {
        var primitive = ToPrimitiveForBigInt(value);
        if (TryGetNumberValue(primitive, out _))
        {
            throw new TypeError("Cannot convert a Number value to a BigInt");
        }

        return ConvertPrimitiveToBigInt(primitive);
    }

    /// <summary>
    /// ToPrimitive(value, hint number), without applying any BigInt-specific
    /// conversion yet. Non-object primitives (including Symbol) pass through
    /// unchanged; undefined/null are preserved so callers can report the
    /// appropriate TypeError for them.
    /// </summary>
    private static object? ToPrimitiveForBigInt(object? value)
    {
        if (value is null
            or JsNull
            or BigInteger
            or bool
            or string
            or int
            or long
            or short
            or byte
            or double
            or float
            or Symbol)
        {
            return value;
        }

        if (!TypeUtilities.TryCoerceObjectToPrimitive(value, "number", out var primitive))
        {
            throw new TypeError("Cannot convert object to primitive value");
        }

        return primitive;
    }

    private static bool TryGetNumberValue(object? primitive, out double numberValue)
    {
        switch (primitive)
        {
            case double d:
                numberValue = d;
                return true;
            case float f:
                numberValue = f;
                return true;
            case int i:
                numberValue = i;
                return true;
            case long l:
                numberValue = l;
                return true;
            case short s:
                numberValue = s;
                return true;
            case byte b:
                numberValue = b;
                return true;
            default:
                numberValue = default;
                return false;
        }
    }

    /// <summary>
    /// NumberToBigInt ( number ): throws a RangeError unless number is an integral
    /// Number. NaN and +/-Infinity are never integral, so they are RangeErrors too
    /// (not TypeErrors).
    /// </summary>
    private static BigInteger NumberToBigInt(double number)
    {
        if (double.IsNaN(number) || double.IsInfinity(number))
        {
            throw new RangeError("The number cannot be converted to a BigInt because it is not an integer");
        }

        var truncated = global::System.Math.Truncate(number);
        if (truncated != number)
        {
            throw new RangeError("The number cannot be converted to a BigInt because it is not an integer");
        }

        // Clamp/convert via decimal string to avoid precision surprises for large doubles.
        return BigInteger.Parse(truncated.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Applies the BigInt Conversions table (table-tobigint) to a value that has
    /// already been through ToPrimitive and is known not to be a Number.
    /// </summary>
    private static BigInteger ConvertPrimitiveToBigInt(object? primitive)
    {
        switch (primitive)
        {
            case BigInteger bi:
                return bi;

            case bool boolean:
                return boolean ? BigInteger.One : BigInteger.Zero;

            case string str:
                return ParseStringToBigInt(str);

            case null:
                throw new TypeError("Cannot convert undefined to a BigInt");

            case JsNull:
                throw new TypeError("Cannot convert null to a BigInt");

            case Symbol:
                throw new TypeError("Cannot convert a Symbol value to a BigInt");

            default:
                throw new TypeError("Cannot convert value to a BigInt");
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
