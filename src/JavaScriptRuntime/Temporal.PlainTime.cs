using System.Globalization;
using System.Numerics;

namespace JavaScriptRuntime;

public static partial class Temporal
{
    private static readonly string[] TimeFieldNames =
        ["hour", "minute", "second", "millisecond", "microsecond", "nanosecond"];
    private static readonly int[] TimeFieldReadOrder = [0, 4, 3, 1, 5, 2];
    private static readonly int[] TimeFieldMaximums = [23, 59, 59, 999, 999, 999];
    internal const long NanosecondsPerDay = 86_400_000_000_000L;

    private static PlainTimeConstructor GetPlainTimeConstructor(RuntimeIntrinsics intrinsics)
        => intrinsics.GetOrCreate(
            RuntimeIntrinsicSlot.TemporalPlainTimeConstructor,
            static () => new PlainTimeConstructor(),
            constructor =>
            {
                using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
                Function.InitializeFunctionInstance(constructor, 0d, "PlainTime", false);
                PropertyDescriptorStore.DefineOrUpdate(constructor, "prototype", new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Value = GetPlainTimePrototype(intrinsics),
                    Writable = false,
                    Enumerable = false,
                    Configurable = false
                });
                GlobalThis.DefineBuiltinFunctionProperty(constructor, "from",
                    (BuiltinFunction2)(static (_, item, options) => ToTemporalTime(item, options)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(constructor, "compare",
                    (BuiltinFunction2)(static (_, left, right) =>
                    {
                        var first = ToTemporalTime(left);
                        var second = ToTemporalTime(right);
                        return (double)first.Nanoseconds.CompareTo(second.Nanoseconds);
                    }), 2d);
            });

    internal static JsObject GetPlainTimePrototype(RuntimeIntrinsics intrinsics)
        => intrinsics.GetOrCreate(
            RuntimeIntrinsicSlot.TemporalPlainTimePrototype,
            static () => new JsObject(),
            prototype =>
            {
                using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
                PrototypeChain.SetPrototype(prototype, intrinsics.ObjectPrototype);
                GlobalThis.DefineIntrinsicToStringTagProperty(prototype, "Temporal.PlainTime");
                GlobalThis.DefineIntrinsicDataProperty(prototype, "constructor", GetPlainTimeConstructor(intrinsics));
                for (var i = 0; i < TimeFieldNames.Length; i++)
                {
                    var field = i;
                    DefineTemporalGetter(prototype, TimeFieldNames[field],
                        receiver => (double)RequirePlainTime(receiver).GetField(field));
                }
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "equals",
                    (BuiltinFunction1)(static (receiver, other) =>
                    {
                        var time = RequirePlainTime(receiver);
                        return time.Nanoseconds == ToTemporalTime(other).Nanoseconds;
                    }), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "with",
                    (BuiltinFunction2)(static (receiver, fields, options) => WithTime(receiver, fields, options)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "add",
                    (BuiltinFunction1)(static (receiver, duration) => AddTime(receiver, duration, false)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "subtract",
                    (BuiltinFunction1)(static (receiver, duration) => AddTime(receiver, duration, true)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "round",
                    (BuiltinFunction1)(static (receiver, options) => RoundPlainTime(receiver, options)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "toString",
                    (BuiltinFunction1)(static (receiver, options) => FormatPlainTime(RequirePlainTime(receiver), options)), 0d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "toJSON",
                    (BuiltinFunction0)(static receiver => FormatPlainTime(RequirePlainTime(receiver), null)), 0d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "toLocaleString",
                    (BuiltinFunction0)(static receiver => FormatPlainTime(RequirePlainTime(receiver), null)), 0d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "valueOf",
                    (BuiltinFunction0)(static _ => throw new TypeError("Cannot convert Temporal.PlainTime to a primitive value")), 0d);
            });

    private static void DefineTemporalGetter(JsObject prototype, string name, BuiltinFunction0 getter)
    {
        Function.InitializeFunctionInstance(getter, 0d, "get " + name, false);
        Function.MarkUndefinedPrototype(getter);
        PropertyDescriptorStore.DefineOrUpdate(prototype, name, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Accessor,
            Get = getter,
            Enumerable = false,
            Configurable = true
        });
    }

    private static TemporalPlainTime RequirePlainTime(object? value)
        => value as TemporalPlainTime
            ?? throw new TypeError("Temporal.PlainTime method called on incompatible receiver");

    private static object? GetTemporalOptions(object? options)
    {
        if (options is null)
        {
            return null;
        }
        if (TypeUtilities.IsPrimitive(options))
        {
            throw new TypeError("Temporal options must be an object");
        }
        return options;
    }

    private static object? GetTemporalOption(object? options, string key)
        => options is null ? null : ObjectRuntime.GetItem(options, key);

    private static bool GetRejectOverflow(object? options)
    {
        var value = GetTemporalOption(options, "overflow");
        if (value is null)
        {
            return false;
        }
        return DotNet2JSConversions.ToStringRejectingSymbols(value) switch
        {
            "constrain" => false,
            "reject" => true,
            _ => throw new RangeError("Invalid Temporal overflow option")
        };
    }

    private static double[] ReadTimeFields(object item, TemporalPlainTime? initial = null)
    {
        var fields = new double[6];
        if (initial is not null)
        {
            for (var i = 0; i < fields.Length; i++)
            {
                fields[i] = initial.GetField(i);
            }
        }
        var any = false;
        foreach (var index in TimeFieldReadOrder)
        {
            var value = ObjectRuntime.GetItem(item, TimeFieldNames[index]);
            if (value is not null)
            {
                fields[index] = ToIntegerWithTruncation(value);
                any = true;
            }
        }
        if (!any)
        {
            throw new TypeError("Temporal time-like object must contain a time field");
        }
        return fields;
    }

    private static long RegulateTime(double[] fields, bool reject)
    {
        long result = 0;
        for (var i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            if (reject && (field < 0 || field > TimeFieldMaximums[i]))
            {
                throw new RangeError("Temporal.PlainTime field is outside its valid range");
            }
            result += (long)System.Math.Clamp(field, 0, TimeFieldMaximums[i]) * TimeUnitLength((TimeUnit)i);
        }
        return result;
    }

    private static TemporalPlainTime ToTemporalTime(object? item, object? options = null)
    {
        long nanoseconds;
        if (item is TemporalPlainTime time)
        {
            GetRejectOverflow(GetTemporalOptions(options));
            return new TemporalPlainTime(time.Nanoseconds);
        }
        if (!TypeUtilities.IsPrimitive(item))
        {
            var fields = ReadTimeFields(item!);
            var reject = GetRejectOverflow(GetTemporalOptions(options));
            nanoseconds = RegulateTime(fields, reject);
        }
        else
        {
            if (item is not string text)
            {
                throw new TypeError("Temporal time must be a string or an object");
            }
            nanoseconds = ParseTimeString(text);
            GetRejectOverflow(GetTemporalOptions(options));
        }
        return new TemporalPlainTime(nanoseconds);
    }

    private static TemporalPlainTime WithTime(object? receiver, object? item, object? options)
    {
        var time = RequirePlainTime(receiver);
        if (TypeUtilities.IsPrimitive(item) || item is TemporalPlainTime
            || ObjectRuntime.GetItem(item!, "calendar") is not null
            || ObjectRuntime.GetItem(item!, "timeZone") is not null)
        {
            throw new TypeError("Temporal.PlainTime.with requires a partial time object");
        }
        var fields = ReadTimeFields(item!, time);
        var reject = GetRejectOverflow(GetTemporalOptions(options));
        return new TemporalPlainTime(RegulateTime(fields, reject));
    }

    private static TemporalPlainTime AddTime(object? receiver, object? durationLike, bool subtract)
    {
        var time = RequirePlainTime(receiver);
        var duration = ToTemporalDuration(durationLike);
        var delta = duration.TimeNanoseconds;
        var result = (new BigInteger(time.Nanoseconds) + (subtract ? -delta : delta)) % NanosecondsPerDay;
        if (result.Sign < 0)
        {
            result += NanosecondsPerDay;
        }
        return new TemporalPlainTime((long)result);
    }

    private static TemporalPlainTime RoundPlainTime(object? receiver, object? roundTo)
    {
        var time = RequirePlainTime(receiver);
        if (roundTo is null)
        {
            throw new TypeError("Temporal.PlainTime.round requires an argument");
        }
        object? options;
        if (roundTo is string unitName)
        {
            options = new JsObject();
            PrototypeChain.SetPrototype(options, JsNull.Null);
            ObjectRuntime.SetProperty(options, "smallestUnit", unitName);
        }
        else
        {
            options = GetTemporalOptions(roundTo);
        }
        var rawIncrement = GetTemporalOption(options, "roundingIncrement");
        var increment = rawIncrement is null ? 1d : System.Math.Floor(TypeUtilities.ToNumber(rawIncrement));
        if (!double.IsFinite(increment) || increment < 1 || increment > 1_000_000_000)
        {
            throw new RangeError("Invalid Temporal rounding increment");
        }
        var mode = GetTemporalRoundingMode(options, TemporalRoundingMode.HalfExpand);
        var unit = GetTimeUnit(GetTemporalOption(options, "smallestUnit"), true);
        var maximum = unit switch
        {
            TimeUnit.Hour => 24,
            TimeUnit.Minute or TimeUnit.Second => 60,
            _ => 1000
        };
        if (increment >= maximum || maximum % increment != 0)
        {
            throw new RangeError("Temporal rounding increment must divide its unit");
        }
        return new TemporalPlainTime(RoundTime(time.Nanoseconds, (long)increment * TimeUnitLength(unit), mode));
    }

    private static long RoundTime(long nanoseconds, long increment, TemporalRoundingMode mode)
    {
        var quotient = nanoseconds / increment;
        var remainder = nanoseconds % increment;
        var increase = mode switch
        {
            TemporalRoundingMode.Ceil or TemporalRoundingMode.Expand => remainder != 0,
            TemporalRoundingMode.Floor or TemporalRoundingMode.Trunc => false,
            TemporalRoundingMode.HalfCeil or TemporalRoundingMode.HalfExpand => remainder * 2 >= increment,
            TemporalRoundingMode.HalfFloor or TemporalRoundingMode.HalfTrunc => remainder * 2 > increment,
            TemporalRoundingMode.HalfEven => remainder * 2 > increment || (remainder * 2 == increment && quotient % 2 != 0),
            _ => throw new RangeError("Invalid Temporal rounding mode")
        };
        return ((quotient + (increase ? 1 : 0)) * increment) % NanosecondsPerDay;
    }

    private static string FormatPlainTime(TemporalPlainTime time, object? suppliedOptions)
    {
        var options = GetTemporalOptions(suppliedOptions);
        var digits = GetTemporalFractionalSecondDigitsOption(options);
        var mode = GetTemporalRoundingMode(options, TemporalRoundingMode.Trunc);
        var unit = GetTimeUnit(GetTemporalOption(options, "smallestUnit"), false);
        if (unit == TimeUnit.Hour)
        {
            throw new RangeError("Temporal.PlainTime.toString does not support hour precision");
        }
        var precision = ToSecondsStringPrecisionRecord(unit, digits);
        digits = precision.Digits;
        var rounded = RoundTime(time.Nanoseconds, precision.NanosecondIncrement, mode);
        var hour = rounded / TimeUnitLength(TimeUnit.Hour);
        var minute = rounded / TimeUnitLength(TimeUnit.Minute) % 60;
        var result = hour.ToString("D2", CultureInfo.InvariantCulture) + ":"
            + minute.ToString("D2", CultureInfo.InvariantCulture);
        if (unit == TimeUnit.Minute)
        {
            return result;
        }
        result += ":" + (rounded / TimeUnitLength(TimeUnit.Second) % 60).ToString("D2", CultureInfo.InvariantCulture);
        var fraction = (rounded % TimeUnitLength(TimeUnit.Second)).ToString("D9", CultureInfo.InvariantCulture);
        fraction = digits >= 0 ? fraction[..digits] : fraction.TrimEnd('0');
        return fraction.Length == 0 ? result : result + "." + fraction;
    }

    private static long ParseTimeString(string text)
    {
        var parsed = ParseISODateTime(text);
        var clock = parsed.Clock;
        var fraction = parsed.Fraction;
        var offset = parsed.Offset;
        if (offset is "Z" or "z")
        {
            throw new RangeError("Temporal.PlainTime does not accept a UTC designator");
        }
        if (offset.Length != 0)
        {
            ValidateTimeOffset(offset);
        }
        if (parsed.Date is null && !parsed.HasTimePrefix)
        {
            ValidateUnambiguousTime(parsed.Core, clock, offset);
        }
        var hour = int.Parse(clock[..2], CultureInfo.InvariantCulture);
        var minute = clock.Length >= 4 ? int.Parse(clock.Substring(2, 2), CultureInfo.InvariantCulture) : 0;
        var second = clock.Length == 6 ? int.Parse(clock.Substring(4, 2), CultureInfo.InvariantCulture) : 0;
        if (hour > 23 || minute > 59 || second > 60)
        {
            throw new RangeError("Invalid Temporal time string field");
        }
        var nanos = fraction.Length == 0 ? 0 : int.Parse(fraction[1..].PadRight(9, '0'), CultureInfo.InvariantCulture);
        return hour * TimeUnitLength(TimeUnit.Hour) + minute * TimeUnitLength(TimeUnit.Minute)
            + System.Math.Min(second, 59) * TimeUnitLength(TimeUnit.Second) + nanos;
    }

    private static void ValidateUnambiguousTime(string core, string clock, string offset)
    {
        if (offset.Length == 0 && !core.Contains(':') && !core.Contains('.') && !core.Contains(','))
        {
            if (clock.Length == 4)
            {
                var month = int.Parse(clock[..2], CultureInfo.InvariantCulture);
                var day = int.Parse(clock[2..], CultureInfo.InvariantCulture);
                if (IsValidMonthDay(1972, month, day))
                {
                    throw new RangeError("Temporal time string is ambiguous with a month-day");
                }
            }
            if (clock.Length == 6)
            {
                var month = int.Parse(clock[4..], CultureInfo.InvariantCulture);
                if (month is >= 1 and <= 12)
                {
                    throw new RangeError("Temporal time string is ambiguous with a year-month");
                }
            }
        }
        if (core.Length == 5 && core[2] == '-'
            && int.TryParse(core.AsSpan(0, 2), NumberStyles.None, CultureInfo.InvariantCulture, out var m)
            && int.TryParse(core.AsSpan(3, 2), NumberStyles.None, CultureInfo.InvariantCulture, out var d)
            && IsValidMonthDay(1972, m, d))
        {
            throw new RangeError("Temporal time string is ambiguous with a month-day");
        }
        if (core.Length == 7 && core[4] == '-'
            && int.TryParse(core.AsSpan(5, 2), NumberStyles.None, CultureInfo.InvariantCulture, out var monthNumber)
            && monthNumber is >= 1 and <= 12)
        {
            throw new RangeError("Temporal time string is ambiguous with a year-month");
        }
    }

    private sealed class PlainTimeConstructor : JsFunctionObject
    {
        public override bool IsConstructor => true;
        internal override bool SelectsConstructedPrototype => true;

        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => throw new TypeError("Temporal.PlainTime requires 'new'");

        protected override object? ConstructCore(in JsCallArguments arguments, object? newTarget)
        {
            var fields = new double[6];
            for (var i = 0; i < fields.Length; i++)
            {
                var argument = arguments.GetArgument(i);
                fields[i] = argument is null ? 0d : ToIntegerWithTruncation(argument);
            }
            var nanoseconds = RegulateTime(fields, true);
            var prototype = ObjectRuntime.GetItem(newTarget!, "prototype");
            if (!TypeUtilities.IsConstructorReturnOverride(prototype))
            {
                prototype = GetPlainTimePrototype(RuntimeIntrinsics.GetFunctionRealm(newTarget));
            }
            var time = new TemporalPlainTime(nanoseconds);
            PrototypeChain.SetPrototype(time, prototype);
            return time;
        }
    }
}

public sealed class TemporalPlainTime : JsObject
{
    internal long Nanoseconds { get; }

    internal TemporalPlainTime(long nanoseconds)
    {
        Nanoseconds = nanoseconds;
        PrototypeChain.InitializePrototype(this, Temporal.GetPlainTimePrototype(RuntimeIntrinsics.Current));
    }

    internal int GetField(int index)
        => index switch
        {
            0 => (int)(Nanoseconds / 3_600_000_000_000L),
            1 => (int)(Nanoseconds / 60_000_000_000L % 60),
            2 => (int)(Nanoseconds / 1_000_000_000L % 60),
            3 => (int)(Nanoseconds / 1_000_000L % 1000),
            4 => (int)(Nanoseconds / 1000 % 1000),
            5 => (int)(Nanoseconds % 1000),
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
}
