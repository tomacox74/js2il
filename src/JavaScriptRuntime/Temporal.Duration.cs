using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace JavaScriptRuntime;

public static partial class Temporal
{
    private static readonly string[] DurationFieldNames =
        ["years", "months", "weeks", "days", "hours", "minutes", "seconds", "milliseconds", "microseconds", "nanoseconds"];
    private static readonly int[] DurationFieldReadOrder = [3, 4, 8, 7, 5, 1, 9, 6, 2, 0];
    [GeneratedRegex(
        @"\A(?<sign>[+-])?[Pp](?:(?<years>[0-9]+)[Yy])?(?:(?<months>[0-9]+)[Mm])?(?:(?<weeks>[0-9]+)[Ww])?(?:(?<days>[0-9]+)[Dd])?(?:[Tt](?:(?<hours>[0-9]+)(?:[.,](?<hourFraction>[0-9]{1,9}))?[Hh])?(?:(?<minutes>[0-9]+)(?:[.,](?<minuteFraction>[0-9]{1,9}))?[Mm])?(?:(?<seconds>[0-9]+)(?:[.,](?<secondFraction>[0-9]{1,9}))?[Ss])?)?\z",
        TemporalRegexOptions)]
    private static partial Regex DurationStringPattern();

    private static DurationConstructor GetDurationConstructor(RuntimeIntrinsics intrinsics)
        => intrinsics.GetOrCreate(
            RuntimeIntrinsicSlot.TemporalDurationConstructor,
            static () => new DurationConstructor(),
            constructor =>
            {
                using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
                Function.InitializeFunctionInstance(constructor, 0d, "Duration", false);
                PropertyDescriptorStore.DefineOrUpdate(constructor, "prototype", new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Value = GetDurationPrototype(intrinsics),
                    Writable = false,
                    Enumerable = false,
                    Configurable = false
                });
                GlobalThis.DefineBuiltinFunctionProperty(constructor, "from",
                    (BuiltinFunction1)(static (_, item) =>
                    {
                        var duration = ToTemporalDuration(item);
                        return new TemporalDuration((double[])duration.Fields.Clone());
                    }), 1d);
            });

    internal static JsObject GetDurationPrototype(RuntimeIntrinsics intrinsics)
        => intrinsics.GetOrCreate(
            RuntimeIntrinsicSlot.TemporalDurationPrototype,
            static () => new JsObject(),
            prototype =>
            {
                using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
                PrototypeChain.SetPrototype(prototype, intrinsics.ObjectPrototype);
                GlobalThis.DefineIntrinsicToStringTagProperty(prototype, "Temporal.Duration");
                GlobalThis.DefineIntrinsicDataProperty(prototype, "constructor", GetDurationConstructor(intrinsics));
                for (var i = 0; i < DurationFieldNames.Length; i++)
                {
                    var field = i;
                    DefineTemporalGetter(prototype, DurationFieldNames[field],
                        receiver => RequireDuration(receiver).Fields[field]);
                }
                DefineTemporalGetter(prototype, "sign", receiver => (double)RequireDuration(receiver).Sign);
                DefineTemporalGetter(prototype, "blank", receiver => RequireDuration(receiver).Sign == 0);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "valueOf",
                    (BuiltinFunction0)(static _ => throw new TypeError("Cannot convert Temporal.Duration to a primitive value")), 0d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "toString",
                    (BuiltinFunction1)(static (receiver, options) => FormatDuration(RequireDuration(receiver), options)), 0d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "toJSON",
                    (BuiltinFunction0)(static receiver => TemporalDurationToString(RequireDuration(receiver), -1)), 0d);
            });

    private static TemporalDuration RequireDuration(object? receiver)
        => receiver as TemporalDuration
            ?? throw new TypeError("Temporal.Duration getter called on incompatible receiver");

    private static string FormatDuration(TemporalDuration duration, object? suppliedOptions)
    {
        var options = GetTemporalOptions(suppliedOptions);
        var digits = GetTemporalFractionalSecondDigitsOption(options);
        var roundingMode = GetTemporalRoundingMode(options, TemporalRoundingMode.Trunc);
        var smallestUnit = GetTimeUnit(GetTemporalOption(options, "smallestUnit"), false);
        if (smallestUnit is TimeUnit.Hour or TimeUnit.Minute)
        {
            throw new RangeError("Temporal.Duration.toString requires second or finer precision");
        }
        var precision = ToSecondsStringPrecisionRecord(smallestUnit, digits);
        if (precision.Unit == TimeUnit.Nanosecond && precision.Increment == 1)
        {
            return TemporalDurationToString(duration, precision.Digits);
        }

        var roundedTime = RoundTimeDuration(duration.TimeNanoseconds, precision.NanosecondIncrement, roundingMode);
        var fields = (double[])duration.Fields.Clone();
        var hasDate = fields[0] != 0 || fields[1] != 0 || fields[2] != 0 || fields[3] != 0;
        var largestTimeUnit = hasDate || fields[4] != 0
            ? TimeUnit.Hour
            : fields[5] != 0 ? TimeUnit.Minute : TimeUnit.Second;
        var remainder = BigInteger.Abs(roundedTime);
        if (hasDate)
        {
            fields[3] += (double)(remainder / NanosecondsPerDay) * roundedTime.Sign;
            remainder %= NanosecondsPerDay;
        }
        for (var unit = TimeUnit.Hour; unit <= TimeUnit.Nanosecond; unit++)
        {
            var index = 4 + (int)unit;
            fields[index] = 0;
            if (unit >= largestTimeUnit)
            {
                var length = TimeUnitLength(unit);
                fields[index] = (double)(remainder / length) * roundedTime.Sign;
                remainder %= length;
            }
        }
        return TemporalDurationToString(new TemporalDuration(fields), precision.Digits);
    }

    private static string TemporalDurationToString(TemporalDuration duration, int digits)
    {
        var fields = duration.Fields;
        var result = new StringBuilder(duration.Sign < 0 ? "-P" : "P");
        const string dateSuffixes = "YMWD";
        for (var i = 0; i < dateSuffixes.Length; i++)
        {
            if (fields[i] != 0)
            {
                result.Append(BigInteger.Abs(new BigInteger(fields[i])).ToString(CultureInfo.InvariantCulture));
                result.Append(dateSuffixes[i]);
            }
        }
        var secondsDuration = new BigInteger(fields[6]) * TimeUnitLength(TimeUnit.Second)
            + new BigInteger(fields[7]) * TimeUnitLength(TimeUnit.Millisecond)
            + new BigInteger(fields[8]) * TimeUnitLength(TimeUnit.Microsecond)
            + new BigInteger(fields[9]);
        var zeroMinutesAndHigher = fields[0] == 0 && fields[1] == 0 && fields[2] == 0
            && fields[3] == 0 && fields[4] == 0 && fields[5] == 0;
        var includeSeconds = !secondsDuration.IsZero || zeroMinutesAndHigher || digits >= 0;
        if (fields[4] != 0 || fields[5] != 0 || includeSeconds)
        {
            result.Append('T');
            if (fields[4] != 0)
            {
                result.Append(BigInteger.Abs(new BigInteger(fields[4])).ToString(CultureInfo.InvariantCulture));
                result.Append('H');
            }
            if (fields[5] != 0)
            {
                result.Append(BigInteger.Abs(new BigInteger(fields[5])).ToString(CultureInfo.InvariantCulture));
                result.Append('M');
            }
            if (includeSeconds)
            {
                var seconds = BigInteger.DivRem(BigInteger.Abs(secondsDuration), TimeUnitLength(TimeUnit.Second), out var nanoseconds);
                result.Append(seconds.ToString(CultureInfo.InvariantCulture));
                var fraction = nanoseconds.ToString("D9", CultureInfo.InvariantCulture);
                fraction = digits < 0 ? fraction.TrimEnd('0') : fraction[..digits];
                if (fraction.Length != 0)
                {
                    result.Append('.');
                    result.Append(fraction);
                }
                result.Append('S');
            }
        }
        return result.ToString();
    }

    private static TemporalDuration ToTemporalDuration(object? item)
    {
        if (item is TemporalDuration duration)
        {
            return duration;
        }
        if (TypeUtilities.IsPrimitive(item))
        {
            if (item is not string text)
            {
                throw new TypeError("Temporal duration must be a string or an object");
            }
            return ParseDurationString(text);
        }
        var fields = new double[10];
        var any = false;
        foreach (var index in DurationFieldReadOrder)
        {
            var value = ObjectRuntime.GetItem(item!, DurationFieldNames[index]);
            if (value is not null)
            {
                fields[index] = ToIntegerIfIntegral(value);
                any = true;
            }
        }
        if (!any)
        {
            throw new TypeError("Temporal duration-like object must contain a duration field");
        }
        return new TemporalDuration(fields);
    }

    private static TemporalDuration ParseDurationString(string text)
    {
        var match = DurationStringPattern().Match(text);
        if (!match.Success)
        {
            throw new RangeError("Invalid Temporal duration string");
        }
        var fields = new double[10];
        var any = false;
        var anyTime = false;
        for (var i = 0; i < 7; i++)
        {
            var group = match.Groups[DurationFieldNames[i]];
            if (group.Length != 0)
            {
                fields[i] = double.Parse(group.Value, CultureInfo.InvariantCulture);
                any = true;
                anyTime |= i >= 4;
            }
        }
        if (!any || (text.Contains('T') || text.Contains('t')) && !anyTime)
        {
            throw new RangeError("Temporal duration string must contain a duration field");
        }
        var hourFraction = match.Groups["hourFraction"].Value;
        var minuteFraction = match.Groups["minuteFraction"].Value;
        var secondFraction = match.Groups["secondFraction"].Value;
        if (hourFraction.Length != 0 && (match.Groups["minutes"].Length != 0 || match.Groups["seconds"].Length != 0)
            || minuteFraction.Length != 0 && match.Groups["seconds"].Length != 0)
        {
            throw new RangeError("Only the smallest duration unit may have a fraction");
        }
        var fractionalNanoseconds = FractionalDurationNanoseconds(hourFraction, TimeUnitLength(TimeUnit.Hour))
            + FractionalDurationNanoseconds(minuteFraction, TimeUnitLength(TimeUnit.Minute))
            + FractionalDurationNanoseconds(secondFraction, TimeUnitLength(TimeUnit.Second));
        for (var i = 5; i < fields.Length; i++)
        {
            var unit = TimeUnitLength((TimeUnit)(i - 4));
            fields[i] += (double)(fractionalNanoseconds / unit);
            fractionalNanoseconds %= unit;
        }
        if (match.Groups["sign"].Value == "-")
        {
            for (var i = 0; i < fields.Length; i++)
            {
                fields[i] = fields[i] == 0 ? 0d : -fields[i];
            }
        }
        return new TemporalDuration(fields);
    }

    private static BigInteger FractionalDurationNanoseconds(string fraction, long unit)
        => fraction.Length == 0
            ? BigInteger.Zero
            : BigInteger.Parse(fraction.PadRight(9, '0'), CultureInfo.InvariantCulture) * unit / 1_000_000_000;

    private sealed class DurationConstructor : JsFunctionObject
    {
        public override bool IsConstructor => true;
        internal override bool SelectsConstructedPrototype => true;

        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => throw new TypeError("Temporal.Duration requires 'new'");

        protected override object? ConstructCore(in JsCallArguments arguments, object? newTarget)
        {
            var fields = new double[10];
            for (var i = 0; i < fields.Length; i++)
            {
                var argument = arguments.GetArgument(i);
                fields[i] = argument is null ? 0d : ToIntegerIfIntegral(argument);
            }
            var duration = new TemporalDuration(fields);
            var prototype = ObjectRuntime.GetItem(newTarget!, "prototype");
            if (!TypeUtilities.IsConstructorReturnOverride(prototype))
            {
                prototype = GetDurationPrototype(RuntimeIntrinsics.GetFunctionRealm(newTarget));
            }
            PrototypeChain.SetPrototype(duration, prototype);
            return duration;
        }
    }
}

public sealed class TemporalDuration : JsObject
{
    internal double[] Fields { get; }
    internal int Sign { get; }
    internal BigInteger TimeNanoseconds { get; }

    internal TemporalDuration(double[] fields)
    {
        var sign = 0;
        foreach (var field in fields)
        {
            if (!double.IsFinite(field) || System.Math.Truncate(field) != field)
            {
                throw new RangeError("Temporal duration fields must be finite integers");
            }
            var nextSign = System.Math.Sign(field);
            if (sign != 0 && nextSign != 0 && sign != nextSign)
            {
                throw new RangeError("Temporal duration fields must have the same sign");
            }
            if (nextSign != 0)
            {
                sign = nextSign;
            }
        }
        if (System.Math.Abs(fields[0]) >= 4_294_967_296
            || System.Math.Abs(fields[1]) >= 4_294_967_296
            || System.Math.Abs(fields[2]) >= 4_294_967_296)
        {
            throw new RangeError("Temporal duration calendar fields exceed their limit");
        }
        var time = new BigInteger(fields[4]) * 3_600_000_000_000L
            + new BigInteger(fields[5]) * 60_000_000_000L
            + new BigInteger(fields[6]) * 1_000_000_000L
            + new BigInteger(fields[7]) * 1_000_000L
            + new BigInteger(fields[8]) * 1_000L + new BigInteger(fields[9]);
        var normalized = new BigInteger(fields[3]) * Temporal.NanosecondsPerDay + time;
        if (BigInteger.Abs(normalized) >= (BigInteger.One << 53) * 1_000_000_000)
        {
            throw new RangeError("Temporal duration exceeds its normalized time limit");
        }
        Fields = fields;
        Sign = sign;
        TimeNanoseconds = time;
        PrototypeChain.InitializePrototype(this, Temporal.GetDurationPrototype(RuntimeIntrinsics.Current));
    }
}
