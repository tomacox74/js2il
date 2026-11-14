using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace JavaScriptRuntime;

public static partial class Temporal
{
    private static readonly string[] TimeFieldNames =
        ["hour", "minute", "second", "millisecond", "microsecond", "nanosecond"];
    private static readonly int[] TimeFieldReadOrder = [0, 4, 3, 1, 5, 2];
    private static readonly int[] TimeFieldMaximums = [23, 59, 59, 999, 999, 999];
    private static readonly long[] TimeUnitNanoseconds =
        [3_600_000_000_000L, 60_000_000_000L, 1_000_000_000L, 1_000_000L, 1_000L, 1L];
    internal const long NanosecondsPerDay = 86_400_000_000_000L;
    private const RegexOptions TemporalRegexOptions =
        RegexOptions.CultureInvariant | RegexOptions.NonBacktracking;
    private static readonly Regex TimeStringPattern = new(
        @"\A(?:(?<date>(?:[+-][0-9]{6}|[0-9]{4})(?:-[0-9]{2}-[0-9]{2}|[0-9]{4}))[Tt ]|(?<prefix>[Tt]))?(?<clock>[0-9]{2}(?::[0-9]{2}(?::[0-9]{2})?|[0-9]{2}(?:[0-9]{2})?)?)(?<fraction>[.,][0-9]{1,9})?(?<offset>[Zz]|[+-][0-9]{2}(?::[0-9]{2}(?::[0-9]{2}(?:[.,][0-9]{1,9})?)?|[0-9]{2}(?:[0-9]{2}(?:[.,][0-9]{1,9})?)?)?)?\z",
        TemporalRegexOptions);
    private static readonly Regex DateStringPattern = new(
        @"\A(?<year>[+-][0-9]{6}|[0-9]{4})(?:-(?<month>[0-9]{2})-(?<day>[0-9]{2})|(?<month>[0-9]{2})(?<day>[0-9]{2}))\z",
        TemporalRegexOptions);
    private static readonly Regex AnnotationPattern = new(
        @"\A(?:[a-z_][a-z0-9_-]*=[a-zA-Z0-9]+(?:-[a-zA-Z0-9]+)*|[a-zA-Z._][a-zA-Z0-9._+-]*(?:/[a-zA-Z0-9._+-]+)*|[+-][0-9]{2}(?::?[0-9]{2})?)\z",
        TemporalRegexOptions);

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

    internal static double ToTemporalInteger(object? value, bool integral = false)
    {
        var number = TypeUtilities.ToNumber(value);
        if (!double.IsFinite(number) || (integral && System.Math.Truncate(number) != number))
        {
            throw new RangeError("Temporal fields must be finite integers");
        }
        var result = System.Math.Truncate(number);
        return result == 0 ? 0d : result;
    }

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
                fields[index] = ToTemporalInteger(value);
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
            result += (long)System.Math.Clamp(field, 0, TimeFieldMaximums[i]) * TimeUnitNanoseconds[i];
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

    private static int GetTimeUnit(object? value, bool required)
    {
        if (value is null)
        {
            if (required)
            {
                throw new RangeError("Temporal rounding requires smallestUnit");
            }
            return -1;
        }
        return DotNet2JSConversions.ToStringRejectingSymbols(value) switch
        {
            "hour" or "hours" => 0,
            "minute" or "minutes" => 1,
            "second" or "seconds" => 2,
            "millisecond" or "milliseconds" => 3,
            "microsecond" or "microseconds" => 4,
            "nanosecond" or "nanoseconds" => 5,
            _ => throw new RangeError("Invalid Temporal time unit")
        };
    }

    private static string GetTemporalRoundingMode(object? options, string fallback)
    {
        var value = GetTemporalOption(options, "roundingMode");
        if (value is null)
        {
            return fallback;
        }
        var mode = DotNet2JSConversions.ToStringRejectingSymbols(value);
        return mode switch
        {
            "ceil" or "floor" or "expand" or "trunc" or "halfCeil" or "halfFloor"
                or "halfExpand" or "halfTrunc" or "halfEven" => mode,
            _ => throw new RangeError("Invalid Temporal rounding mode")
        };
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
        var mode = GetTemporalRoundingMode(options, "halfExpand");
        var unit = GetTimeUnit(GetTemporalOption(options, "smallestUnit"), true);
        var maximum = unit switch
        {
            0 => 24,
            1 or 2 => 60,
            _ => 1000
        };
        if (increment >= maximum || maximum % increment != 0)
        {
            throw new RangeError("Temporal rounding increment must divide its unit");
        }
        return new TemporalPlainTime(RoundTime(time.Nanoseconds, (long)increment * TimeUnitNanoseconds[unit], mode));
    }

    private static long RoundTime(long nanoseconds, long increment, string mode)
    {
        var quotient = nanoseconds / increment;
        var remainder = nanoseconds % increment;
        var increase = mode switch
        {
            "ceil" or "expand" => remainder != 0,
            "floor" or "trunc" => false,
            "halfCeil" or "halfExpand" => remainder * 2 >= increment,
            "halfFloor" or "halfTrunc" => remainder * 2 > increment,
            "halfEven" => remainder * 2 > increment || (remainder * 2 == increment && quotient % 2 != 0),
            _ => throw new RangeError("Invalid Temporal rounding mode")
        };
        return ((quotient + (increase ? 1 : 0)) * increment) % NanosecondsPerDay;
    }

    private static string FormatPlainTime(TemporalPlainTime time, object? suppliedOptions)
    {
        var options = GetTemporalOptions(suppliedOptions);
        var rawDigits = GetTemporalOption(options, "fractionalSecondDigits");
        var digits = -1;
        if (rawDigits is double or float or int or long or short or byte)
        {
            var value = System.Math.Floor(TypeUtilities.ToNumber(rawDigits));
            if (!double.IsFinite(value) || value < 0 || value > 9)
            {
                throw new RangeError("Invalid fractionalSecondDigits option");
            }
            digits = (int)value;
        }
        else if (rawDigits is not null && DotNet2JSConversions.ToStringRejectingSymbols(rawDigits) != "auto")
        {
            throw new RangeError("Invalid fractionalSecondDigits option");
        }
        var mode = GetTemporalRoundingMode(options, "trunc");
        var unit = GetTimeUnit(GetTemporalOption(options, "smallestUnit"), false);
        if (unit == 0)
        {
            throw new RangeError("Temporal.PlainTime.toString does not support hour precision");
        }
        if (unit >= 2)
        {
            digits = (unit - 2) * 3;
        }
        long increment = unit == 1 ? TimeUnitNanoseconds[1] : 1;
        if (unit != 1 && digits >= 0)
        {
            for (var i = digits; i < 9; i++)
            {
                increment *= 10;
            }
        }
        var rounded = RoundTime(time.Nanoseconds, increment, mode);
        var hour = rounded / TimeUnitNanoseconds[0];
        var minute = rounded / TimeUnitNanoseconds[1] % 60;
        var result = hour.ToString("D2", CultureInfo.InvariantCulture) + ":"
            + minute.ToString("D2", CultureInfo.InvariantCulture);
        if (unit == 1)
        {
            return result;
        }
        result += ":" + (rounded / TimeUnitNanoseconds[2] % 60).ToString("D2", CultureInfo.InvariantCulture);
        var fraction = (rounded % TimeUnitNanoseconds[2]).ToString("D9", CultureInfo.InvariantCulture);
        fraction = digits >= 0 ? fraction[..digits] : fraction.TrimEnd('0');
        return fraction.Length == 0 ? result : result + "." + fraction;
    }

    private static long ParseTimeString(string text)
    {
        var annotationStart = text.IndexOf('[');
        var core = annotationStart < 0 ? text : text[..annotationStart];
        if (annotationStart >= 0)
        {
            ValidateTimeAnnotations(text[annotationStart..]);
        }
        var match = TimeStringPattern.Match(core);
        if (!match.Success)
        {
            throw new RangeError("Invalid Temporal time string");
        }
        var date = match.Groups["date"].Value;
        if (date.Length != 0)
        {
            ValidateTimeDate(date);
        }
        var clock = match.Groups["clock"].Value.Replace(":", "", StringComparison.Ordinal);
        var fraction = match.Groups["fraction"].Value;
        if (fraction.Length != 0 && clock.Length != 6)
        {
            throw new RangeError("Temporal time fractions require seconds");
        }
        var offset = match.Groups["offset"].Value;
        if (offset is "Z" or "z")
        {
            throw new RangeError("Temporal.PlainTime does not accept a UTC designator");
        }
        if (offset.Length != 0)
        {
            ValidateTimeOffset(offset);
        }
        if (date.Length == 0 && match.Groups["prefix"].Length == 0)
        {
            ValidateUnambiguousTime(core, clock, offset);
        }
        var hour = int.Parse(clock[..2], CultureInfo.InvariantCulture);
        var minute = clock.Length >= 4 ? int.Parse(clock.Substring(2, 2), CultureInfo.InvariantCulture) : 0;
        var second = clock.Length == 6 ? int.Parse(clock.Substring(4, 2), CultureInfo.InvariantCulture) : 0;
        if (hour > 23 || minute > 59 || second > 60)
        {
            throw new RangeError("Invalid Temporal time string field");
        }
        var nanos = fraction.Length == 0 ? 0 : int.Parse(fraction[1..].PadRight(9, '0'), CultureInfo.InvariantCulture);
        return hour * TimeUnitNanoseconds[0] + minute * TimeUnitNanoseconds[1]
            + System.Math.Min(second, 59) * TimeUnitNanoseconds[2] + nanos;
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

    private static void ValidateTimeDate(string date)
    {
        var match = DateStringPattern.Match(date);
        if (!match.Success || match.Groups["year"].Value == "-000000")
        {
            throw new RangeError("Invalid date in Temporal time string");
        }
        var year = int.Parse(match.Groups["year"].Value, CultureInfo.InvariantCulture);
        var month = int.Parse(match.Groups["month"].Value, CultureInfo.InvariantCulture);
        var day = int.Parse(match.Groups["day"].Value, CultureInfo.InvariantCulture);
        if (!IsValidMonthDay(year, month, day))
        {
            throw new RangeError("Invalid date in Temporal time string");
        }
    }

    private static bool IsValidMonthDay(int year, int month, int day)
    {
        var maximum = month switch
        {
            2 => year % 4 == 0 && (year % 100 != 0 || year % 400 == 0) ? 29 : 28,
            4 or 6 or 9 or 11 => 30,
            >= 1 and <= 12 => 31,
            _ => 0
        };
        return day >= 1 && day <= maximum;
    }

    private static void ValidateTimeOffset(string offset)
    {
        var digits = offset[1..].Replace(":", "", StringComparison.Ordinal);
        var fraction = digits.IndexOfAny(['.', ',']);
        if (fraction >= 0)
        {
            digits = digits[..fraction];
        }
        if (int.Parse(digits[..2], CultureInfo.InvariantCulture) > 23
            || (digits.Length >= 4 && int.Parse(digits.Substring(2, 2), CultureInfo.InvariantCulture) > 59)
            || (digits.Length >= 6 && int.Parse(digits.Substring(4, 2), CultureInfo.InvariantCulture) > 59))
        {
            throw new RangeError("Invalid Temporal time offset");
        }
    }

    private static void ValidateTimeAnnotations(string annotations)
    {
        var position = 0;
        var calendarSeen = false;
        var calendarCritical = false;
        var zoneSeen = false;
        var keyValueSeen = false;
        while (position < annotations.Length)
        {
            if (annotations[position] != '[')
            {
                throw new RangeError("Invalid Temporal time annotation");
            }
            var end = annotations.IndexOf(']', position + 1);
            if (end < 0)
            {
                throw new RangeError("Invalid Temporal time annotation");
            }
            var annotation = annotations[(position + 1)..end];
            var critical = annotation.StartsWith('!');
            if (critical)
            {
                annotation = annotation[1..];
            }
            if (!AnnotationPattern.IsMatch(annotation))
            {
                throw new RangeError("Invalid Temporal time annotation");
            }
            if (annotation.Contains('='))
            {
                keyValueSeen = true;
            }
            if (annotation.StartsWith("u-ca=", StringComparison.Ordinal))
            {
                if (calendarSeen && (critical || calendarCritical))
                {
                    throw new RangeError("Duplicate critical Temporal calendar annotation");
                }
                calendarSeen = true;
                calendarCritical |= critical;
            }
            else if (annotation.Contains('='))
            {
                if (critical)
                {
                    throw new RangeError("Unknown critical Temporal time annotation");
                }
            }
            else
            {
                if (zoneSeen || keyValueSeen)
                {
                    throw new RangeError("Invalid Temporal time zone annotation order");
                }
                if (annotation.StartsWith('+') || annotation.StartsWith('-'))
                {
                    ValidateTimeOffset(annotation);
                }
                zoneSeen = true;
            }
            position = end + 1;
        }
    }

    private sealed class PlainTimeConstructor : JsFunctionObject
    {
        public override bool IsConstructor => true;

        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => throw new TypeError("Temporal.PlainTime requires 'new'");

        protected override object? ConstructCore(in JsCallArguments arguments, object? newTarget)
        {
            var fields = new double[6];
            for (var i = 0; i < fields.Length; i++)
            {
                var argument = arguments.GetArgument(i);
                fields[i] = argument is null ? 0d : ToTemporalInteger(argument);
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
