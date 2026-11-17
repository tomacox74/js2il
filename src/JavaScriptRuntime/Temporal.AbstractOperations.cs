using System.Numerics;

namespace JavaScriptRuntime;

public static partial class Temporal
{
    private enum TimeUnit
    {
        Unset = -1,
        Hour,
        Minute,
        Second,
        Millisecond,
        Microsecond,
        Nanosecond
    }

    private enum TemporalRoundingMode
    {
        Ceil,
        Floor,
        Expand,
        Trunc,
        HalfCeil,
        HalfFloor,
        HalfExpand,
        HalfTrunc,
        HalfEven
    }

    private readonly record struct SecondsStringPrecision(int Digits, TimeUnit Unit, int Increment)
    {
        internal long NanosecondIncrement => TimeUnitLength(Unit) * Increment;
    }

    private static double ToIntegerWithTruncation(object? value)
    {
        var number = TypeUtilities.ToNumber(value);
        if (!double.IsFinite(number))
        {
            throw new RangeError("Temporal fields must be finite integers");
        }
        var result = System.Math.Truncate(number);
        return result == 0 ? 0d : result;
    }

    private static double ToIntegerIfIntegral(object? value)
    {
        var number = TypeUtilities.ToNumber(value);
        if (!double.IsFinite(number) || System.Math.Truncate(number) != number)
        {
            throw new RangeError("Temporal fields must be finite integers");
        }
        return number == 0 ? 0d : number;
    }

    private static long TimeUnitLength(TimeUnit unit)
        => unit switch
        {
            TimeUnit.Hour => 3_600_000_000_000L,
            TimeUnit.Minute => 60_000_000_000L,
            TimeUnit.Second => 1_000_000_000L,
            TimeUnit.Millisecond => 1_000_000L,
            TimeUnit.Microsecond => 1_000L,
            TimeUnit.Nanosecond => 1L,
            _ => throw new ArgumentOutOfRangeException(nameof(unit))
        };

    private static TimeUnit GetTimeUnit(object? value, bool required)
    {
        if (value is null)
        {
            return required
                ? throw new RangeError("Temporal rounding requires smallestUnit")
                : TimeUnit.Unset;
        }
        return DotNet2JSConversions.ToStringRejectingSymbols(value) switch
        {
            "hour" or "hours" => TimeUnit.Hour,
            "minute" or "minutes" => TimeUnit.Minute,
            "second" or "seconds" => TimeUnit.Second,
            "millisecond" or "milliseconds" => TimeUnit.Millisecond,
            "microsecond" or "microseconds" => TimeUnit.Microsecond,
            "nanosecond" or "nanoseconds" => TimeUnit.Nanosecond,
            _ => throw new RangeError("Invalid Temporal time unit")
        };
    }

    private static TemporalRoundingMode GetTemporalRoundingMode(object? options, TemporalRoundingMode fallback)
    {
        var value = GetTemporalOption(options, "roundingMode");
        if (value is null)
        {
            return fallback;
        }
        return DotNet2JSConversions.ToStringRejectingSymbols(value) switch
        {
            "ceil" => TemporalRoundingMode.Ceil,
            "floor" => TemporalRoundingMode.Floor,
            "expand" => TemporalRoundingMode.Expand,
            "trunc" => TemporalRoundingMode.Trunc,
            "halfCeil" => TemporalRoundingMode.HalfCeil,
            "halfFloor" => TemporalRoundingMode.HalfFloor,
            "halfExpand" => TemporalRoundingMode.HalfExpand,
            "halfTrunc" => TemporalRoundingMode.HalfTrunc,
            "halfEven" => TemporalRoundingMode.HalfEven,
            _ => throw new RangeError("Invalid Temporal rounding mode")
        };
    }

    private static int GetTemporalFractionalSecondDigitsOption(object? options)
    {
        var rawDigits = GetTemporalOption(options, "fractionalSecondDigits");
        if (rawDigits is double or float or int or long or short or byte)
        {
            var value = System.Math.Floor(TypeUtilities.ToNumber(rawDigits));
            if (!double.IsFinite(value) || value < 0 || value > 9)
            {
                throw new RangeError("Invalid fractionalSecondDigits option");
            }
            return (int)value;
        }
        if (rawDigits is not null && DotNet2JSConversions.ToStringRejectingSymbols(rawDigits) != "auto")
        {
            throw new RangeError("Invalid fractionalSecondDigits option");
        }
        return -1;
    }

    private static SecondsStringPrecision ToSecondsStringPrecisionRecord(TimeUnit unit, int digits)
    {
        if (unit == TimeUnit.Minute)
        {
            return new SecondsStringPrecision(-1, TimeUnit.Minute, 1);
        }
        digits = unit switch
        {
            TimeUnit.Second => 0,
            TimeUnit.Millisecond => 3,
            TimeUnit.Microsecond => 6,
            TimeUnit.Nanosecond => 9,
            _ => digits
        };
        unit = digits switch
        {
            0 => TimeUnit.Second,
            <= 3 and >= 1 => TimeUnit.Millisecond,
            <= 6 and >= 4 => TimeUnit.Microsecond,
            _ => TimeUnit.Nanosecond
        };
        var increment = 1;
        if (digits >= 0)
        {
            var unitDigits = unit switch
            {
                TimeUnit.Second => 0,
                TimeUnit.Millisecond => 3,
                TimeUnit.Microsecond => 6,
                _ => 9
            };
            for (var i = digits; i < unitDigits; i++)
            {
                increment *= 10;
            }
        }
        return new SecondsStringPrecision(digits, unit, increment);
    }

    private static BigInteger RoundTimeDuration(BigInteger nanoseconds, long increment, TemporalRoundingMode mode)
    {
        var magnitude = BigInteger.Abs(nanoseconds);
        var quotient = BigInteger.DivRem(magnitude, increment, out var remainder);
        var comparison = (remainder * 2).CompareTo(increment);
        var increase = mode switch
        {
            TemporalRoundingMode.Ceil => nanoseconds.Sign > 0 && !remainder.IsZero,
            TemporalRoundingMode.Floor => nanoseconds.Sign < 0 && !remainder.IsZero,
            TemporalRoundingMode.Expand => !remainder.IsZero,
            TemporalRoundingMode.Trunc => false,
            TemporalRoundingMode.HalfCeil => comparison > 0 || comparison == 0 && nanoseconds.Sign > 0,
            TemporalRoundingMode.HalfFloor => comparison > 0 || comparison == 0 && nanoseconds.Sign < 0,
            TemporalRoundingMode.HalfExpand => comparison >= 0,
            TemporalRoundingMode.HalfTrunc => comparison > 0,
            TemporalRoundingMode.HalfEven => comparison > 0 || comparison == 0 && !quotient.IsEven,
            _ => throw new ArgumentOutOfRangeException(nameof(mode))
        };
        var rounded = (quotient + (increase ? 1 : 0)) * increment * nanoseconds.Sign;
        if (BigInteger.Abs(rounded) >= (BigInteger.One << 53) * TimeUnitLength(TimeUnit.Second))
        {
            throw new RangeError("Temporal rounded time duration exceeds its limit");
        }
        return rounded;
    }

    private static double GetTemporalRoundingIncrement(object? options)
    {
        var raw = GetTemporalOption(options, "roundingIncrement");
        var increment = raw is null ? 1d : System.Math.Truncate(TypeUtilities.ToNumber(raw));
        if (!double.IsFinite(increment) || increment < 1 || increment > 1_000_000_000)
        {
            throw new RangeError("Invalid Temporal rounding increment");
        }
        return increment;
    }

    private static string? GetTemporalUnitOption(object? options, string name)
    {
        var value = GetTemporalOption(options, name);
        if (value is null)
        {
            return null;
        }
        var unit = DotNet2JSConversions.ToStringRejectingSymbols(value);
        return unit switch
        {
            "year" or "years" or "month" or "months" or "week" or "weeks" or "day" or "days"
                or "hour" or "hours" or "minute" or "minutes" or "second" or "seconds"
                or "millisecond" or "milliseconds" or "microsecond" or "microseconds"
                or "nanosecond" or "nanoseconds" or "auto" => unit,
            _ => throw new RangeError("Invalid Temporal unit")
        };
    }

    private static void ValidateTimeRoundingIncrement(double increment, TimeUnit unit, bool wholeDay)
    {
        var maximum = wholeDay ? NanosecondsPerDay / TimeUnitLength(unit)
            : unit switch
            {
                TimeUnit.Hour => 24,
                TimeUnit.Minute or TimeUnit.Second => 60,
                _ => 1000
            };
        if (increment > maximum || (!wholeDay && increment == maximum) || maximum % increment != 0)
        {
            throw new RangeError("Temporal rounding increment must divide its unit");
        }
    }

    private static TemporalDuration DifferenceTimeNanoseconds(BigInteger difference, object? suppliedOptions, TimeUnit defaultLargest)
    {
        var options = GetTemporalOptions(suppliedOptions);
        var largestName = GetTemporalUnitOption(options, "largestUnit");
        var increment = GetTemporalRoundingIncrement(options);
        var mode = GetTemporalRoundingMode(options, TemporalRoundingMode.Trunc);
        var smallestName = GetTemporalUnitOption(options, "smallestUnit");
        var largest = largestName is null or "auto" ? TimeUnit.Unset : GetTimeUnit(largestName, true);
        var smallest = smallestName is null ? TimeUnit.Nanosecond : GetTimeUnit(smallestName, true);
        if (largest == TimeUnit.Unset)
        {
            largest = (TimeUnit)System.Math.Min((int)defaultLargest, (int)smallest);
        }
        if (largest > smallest)
        {
            throw new RangeError("largestUnit must not be smaller than smallestUnit");
        }
        ValidateTimeRoundingIncrement(increment, smallest, false);
        var rounded = RoundTimeDuration(difference, (long)increment * TimeUnitLength(smallest), mode);
        var fields = new double[10];
        var remainder = BigInteger.Abs(rounded);
        for (var unit = largest; unit <= TimeUnit.Nanosecond; unit++)
        {
            var length = TimeUnitLength(unit);
            var quotient = remainder / length;
            fields[(int)unit + 4] = quotient.IsZero ? 0d : Number.ConvertBigIntegerToDouble(quotient) * rounded.Sign;
            remainder %= length;
        }
        return new TemporalDuration(fields);
    }
}
