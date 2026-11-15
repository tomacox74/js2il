using System.Globalization;
using System.Numerics;

namespace JavaScriptRuntime;

public static partial class Temporal
{
    private static TemporalInstant RequireInstant(object? value)
        => value as TemporalInstant
            ?? throw new TypeError("Temporal.Instant method called on incompatible receiver");

    private static BigInteger FloorDivide(BigInteger value, long divisor)
    {
        var quotient = BigInteger.DivRem(value, divisor, out var remainder);
        return remainder.Sign < 0 ? quotient - 1 : quotient;
    }

    private static TemporalInstant ToTemporalInstant(object? value)
    {
        if (value is TemporalInstant instant)
        {
            return instant;
        }
        if (TypeUtilities.IsPrimitive(value) && value is not string)
        {
            throw new TypeError("Temporal.Instant requires an instant or an ISO string");
        }
        var parsed = ParseISODateTime(DotNet2JSConversions.ToStringRejectingSymbols(value));
        if (parsed.Date is not ISODate date || parsed.Offset.Length == 0)
        {
            throw new RangeError("Temporal.Instant requires a date, time and UTC offset");
        }
        var time = ParseInstantClock(parsed);
        return new TemporalInstant(new BigInteger(ISODaysFromEpoch(date)) * NanosecondsPerDay
            + time - ParseInstantOffset(parsed.Offset));
    }

    private static long ParseInstantClock(ISODateTime parsed)
    {
        var clock = parsed.Clock;
        var hour = int.Parse(clock[..2], CultureInfo.InvariantCulture);
        var minute = clock.Length >= 4 ? int.Parse(clock.Substring(2, 2), CultureInfo.InvariantCulture) : 0;
        var second = clock.Length >= 6 ? int.Parse(clock.Substring(4, 2), CultureInfo.InvariantCulture) : 0;
        if (hour > 23 || minute > 59 || second > 60)
        {
            throw new RangeError("Invalid Temporal.Instant time");
        }
        var fraction = parsed.Fraction.Length == 0 ? 0
            : int.Parse(parsed.Fraction[1..].PadRight(9, '0'), CultureInfo.InvariantCulture);
        return hour * TimeUnitLength(TimeUnit.Hour) + minute * TimeUnitLength(TimeUnit.Minute)
            + System.Math.Min(second, 59) * TimeUnitLength(TimeUnit.Second) + fraction;
    }

    private static long ParseInstantOffset(string offset)
    {
        if (offset is "Z" or "z")
        {
            return 0;
        }
        ValidateTimeOffset(offset);
        var digits = offset[1..].Replace(":", "", StringComparison.Ordinal);
        var separator = digits.IndexOfAny(['.', ',']);
        var fraction = separator < 0 ? 0
            : int.Parse(digits[(separator + 1)..].PadRight(9, '0'), CultureInfo.InvariantCulture);
        if (separator >= 0)
        {
            digits = digits[..separator];
        }
        var hour = int.Parse(digits[..2], CultureInfo.InvariantCulture);
        var minute = digits.Length >= 4 ? int.Parse(digits.Substring(2, 2), CultureInfo.InvariantCulture) : 0;
        var second = digits.Length >= 6 ? int.Parse(digits.Substring(4, 2), CultureInfo.InvariantCulture) : 0;
        var result = hour * TimeUnitLength(TimeUnit.Hour) + minute * TimeUnitLength(TimeUnit.Minute)
            + second * TimeUnitLength(TimeUnit.Second) + fraction;
        return offset[0] == '-' ? -result : result;
    }

    // Proleptic Gregorian civil dates, including year zero and extended years,
    // cannot be represented by System.DateTime.
    private static long ISODaysFromEpoch(ISODate date)
    {
        var year = date.Year - (date.Month <= 2 ? 1 : 0);
        var era = (year >= 0 ? year : year - 399) / 400;
        var yearOfEra = year - era * 400;
        var month = date.Month + (date.Month > 2 ? -3 : 9);
        var dayOfYear = (153 * month + 2) / 5 + date.Day - 1;
        return era * 146097L + yearOfEra * 365L + yearOfEra / 4 - yearOfEra / 100 + dayOfYear - 719468;
    }

    private static ISODate ISODateFromEpochDays(long days)
    {
        days += 719468;
        var era = (days >= 0 ? days : days - 146096) / 146097;
        var dayOfEra = days - era * 146097;
        var yearOfEra = (dayOfEra - dayOfEra / 1460 + dayOfEra / 36524 - dayOfEra / 146096) / 365;
        var year = yearOfEra + era * 400;
        var dayOfYear = dayOfEra - (365 * yearOfEra + yearOfEra / 4 - yearOfEra / 100);
        var month = (5 * dayOfYear + 2) / 153;
        var day = dayOfYear - (153 * month + 2) / 5 + 1;
        month += month < 10 ? 3 : -9;
        year += month <= 2 ? 1 : 0;
        return new ISODate((int)year, (int)month, (int)day);
    }

    private static TemporalInstant AddInstant(object? receiver, object? value, bool subtract)
    {
        var instant = RequireInstant(receiver);
        var duration = ToTemporalDuration(value);
        if (HasCalendarDuration(duration) || duration.Fields[3] != 0)
        {
            throw new RangeError("Temporal.Instant arithmetic cannot use calendar or day units");
        }
        return new TemporalInstant(instant.EpochNanoseconds
            + (subtract ? -duration.TimeNanoseconds : duration.TimeNanoseconds));
    }

    private static double GetInstantRoundingIncrement(object? options)
    {
        var raw = GetTemporalOption(options, "roundingIncrement");
        var increment = raw is null ? 1d : System.Math.Truncate(TypeUtilities.ToNumber(raw));
        if (!double.IsFinite(increment) || increment < 1 || increment > 1_000_000_000)
        {
            throw new RangeError("Invalid Temporal rounding increment");
        }
        return increment;
    }

    private static string? GetInstantUnitOption(object? options, string name)
    {
        var value = GetTemporalOption(options, name);
        return value is null ? null : DotNet2JSConversions.ToStringRejectingSymbols(value);
    }

    private static void ValidateInstantIncrement(double increment, TimeUnit unit, bool wholeDay)
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

    private static TemporalInstant RoundInstant(object? receiver, object? roundTo)
    {
        var instant = RequireInstant(receiver);
        if (roundTo is null)
        {
            throw new TypeError("Temporal.Instant.round requires an argument");
        }
        object? options;
        if (roundTo is string text)
        {
            options = new JsObject();
            PrototypeChain.SetPrototype(options, JsNull.Null);
            ObjectRuntime.SetProperty(options, "smallestUnit", text);
        }
        else
        {
            options = GetTemporalOptions(roundTo);
        }
        var increment = GetInstantRoundingIncrement(options);
        var mode = GetTemporalRoundingMode(options, TemporalRoundingMode.HalfExpand);
        var unit = GetTimeUnit(GetInstantUnitOption(options, "smallestUnit"), true);
        ValidateInstantIncrement(increment, unit, true);
        return new TemporalInstant(RoundInstantNanoseconds(instant.EpochNanoseconds,
            (long)increment * TimeUnitLength(unit), mode));
    }

    private static BigInteger RoundInstantNanoseconds(BigInteger nanoseconds, long increment, TemporalRoundingMode mode)
    {
        // Instant rounding is directed along the timeline, not away from the
        // Unix epoch. Expand/trunc and their tie modes act as if positive.
        mode = mode switch
        {
            TemporalRoundingMode.Expand => TemporalRoundingMode.Ceil,
            TemporalRoundingMode.Trunc => TemporalRoundingMode.Floor,
            TemporalRoundingMode.HalfExpand => TemporalRoundingMode.HalfCeil,
            TemporalRoundingMode.HalfTrunc => TemporalRoundingMode.HalfFloor,
            _ => mode
        };
        return RoundTimeDuration(nanoseconds, increment, mode);
    }

    private static TemporalDuration DifferenceInstant(object? receiver, object? other, object? suppliedOptions, bool since)
    {
        var instant = RequireInstant(receiver);
        var second = ToTemporalInstant(other);
        var options = GetTemporalOptions(suppliedOptions);
        var largestName = GetInstantUnitOption(options, "largestUnit");
        var increment = GetInstantRoundingIncrement(options);
        var mode = GetTemporalRoundingMode(options, TemporalRoundingMode.Trunc);
        var smallestName = GetInstantUnitOption(options, "smallestUnit");
        var smallest = smallestName is null ? TimeUnit.Nanosecond : GetTimeUnit(smallestName, true);
        var largest = largestName is null or "auto"
            ? (TimeUnit)System.Math.Min((int)TimeUnit.Second, (int)smallest)
            : GetTimeUnit(largestName, true);
        if (largest > smallest)
        {
            throw new RangeError("largestUnit must not be smaller than smallestUnit");
        }
        ValidateInstantIncrement(increment, smallest, false);
        var difference = second.EpochNanoseconds - instant.EpochNanoseconds;
        if (since)
        {
            difference = -difference;
        }
        var rounded = RoundTimeDuration(difference, (long)increment * TimeUnitLength(smallest), mode);
        var fields = new double[10];
        var remainder = BigInteger.Abs(rounded);
        for (var i = (int)largest; i <= (int)TimeUnit.Nanosecond; i++)
        {
            var length = TimeUnitLength((TimeUnit)i);
            fields[i + 4] = remainder.IsZero ? 0d : Number.ConvertBigIntegerToDouble(remainder / length) * rounded.Sign;
            remainder %= length;
        }
        return new TemporalDuration(fields);
    }

    private static string FormatInstant(TemporalInstant instant, object? suppliedOptions)
    {
        var options = GetTemporalOptions(suppliedOptions);
        var digits = GetTemporalFractionalSecondDigitsOption(options);
        var mode = GetTemporalRoundingMode(options, TemporalRoundingMode.Trunc);
        var unit = GetTimeUnit(GetInstantUnitOption(options, "smallestUnit"), false);
        if (unit == TimeUnit.Hour)
        {
            throw new RangeError("Temporal.Instant.toString does not support hour precision");
        }
        var zone = GetTemporalOption(options, "timeZone");
        if (zone is not null)
        {
            throw new RangeError("Temporal.Instant time zone formatting is not supported");
        }
        var precision = ToSecondsStringPrecisionRecord(unit, digits);
        var rounded = RoundInstantNanoseconds(instant.EpochNanoseconds, precision.NanosecondIncrement, mode);
        _ = new TemporalInstant(rounded);
        var days = FloorDivide(rounded, NanosecondsPerDay);
        var time = (long)(rounded - days * NanosecondsPerDay);
        var date = ISODateFromEpochDays((long)days);
        var year = date.Year is >= 0 and <= 9999
            ? date.Year.ToString("D4", CultureInfo.InvariantCulture)
            : (date.Year < 0 ? "-" : "+") + System.Math.Abs(date.Year).ToString("D6", CultureInfo.InvariantCulture);
        var result = year + "-" + date.Month.ToString("D2", CultureInfo.InvariantCulture)
            + "-" + date.Day.ToString("D2", CultureInfo.InvariantCulture)
            + "T" + (time / TimeUnitLength(TimeUnit.Hour)).ToString("D2", CultureInfo.InvariantCulture)
            + ":" + (time / TimeUnitLength(TimeUnit.Minute) % 60).ToString("D2", CultureInfo.InvariantCulture);
        if (unit != TimeUnit.Minute)
        {
            result += ":" + (time / TimeUnitLength(TimeUnit.Second) % 60).ToString("D2", CultureInfo.InvariantCulture);
            var fraction = (time % TimeUnitLength(TimeUnit.Second)).ToString("D9", CultureInfo.InvariantCulture);
            fraction = precision.Digits >= 0 ? fraction[..precision.Digits] : fraction.TrimEnd('0');
            if (fraction.Length != 0)
            {
                result += "." + fraction;
            }
        }
        return result + "Z";
    }
}
