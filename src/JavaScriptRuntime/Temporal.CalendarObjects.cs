using System.Globalization;
using System.Numerics;

namespace JavaScriptRuntime;

public static partial class Temporal
{
    // Calendar-bearing types share ISO storage, but retain distinct brands.
    // Their constructors, field accessors, string conversions and valueOf let
    // calendar conversion use internal slots without invoking user-defined
    // calendar/calendarId getters. Other operations are not yet supplied.
    private static void ConfigureCalendarBearingTypes(JsObject temporal, RuntimeIntrinsics intrinsics)
    {
        foreach (var kind in new[] { "PlainDateTime", "PlainMonthDay", "PlainYearMonth", "ZonedDateTime" })
        {
            GlobalThis.DefineIntrinsicDataProperty(temporal, kind, GetCalendarObjectConstructor(intrinsics, kind));
        }
    }

    private static (RuntimeIntrinsicSlot Constructor, RuntimeIntrinsicSlot Prototype) CalendarObjectSlots(string kind)
        => kind switch
        {
            "PlainDateTime" => (RuntimeIntrinsicSlot.TemporalPlainDateTimeConstructor, RuntimeIntrinsicSlot.TemporalPlainDateTimePrototype),
            "PlainMonthDay" => (RuntimeIntrinsicSlot.TemporalPlainMonthDayConstructor, RuntimeIntrinsicSlot.TemporalPlainMonthDayPrototype),
            "PlainYearMonth" => (RuntimeIntrinsicSlot.TemporalPlainYearMonthConstructor, RuntimeIntrinsicSlot.TemporalPlainYearMonthPrototype),
            "ZonedDateTime" => (RuntimeIntrinsicSlot.TemporalZonedDateTimeConstructor, RuntimeIntrinsicSlot.TemporalZonedDateTimePrototype),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

    private static CalendarObjectConstructor GetCalendarObjectConstructor(RuntimeIntrinsics intrinsics, string kind)
        => intrinsics.GetOrCreate(CalendarObjectSlots(kind).Constructor,
            () => new CalendarObjectConstructor(kind),
            constructor =>
            {
                using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
                Function.InitializeFunctionInstance(constructor, kind == "PlainDateTime" ? 3d : 2d, kind, false);
                PropertyDescriptorStore.DefineOrUpdate(constructor, "prototype", new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Value = GetCalendarObjectPrototype(intrinsics, kind),
                    Writable = false, Enumerable = false, Configurable = false
                });
                if (kind == "PlainDateTime")
                {
                    GlobalThis.DefineBuiltinFunctionProperty(constructor, "from",
                        (BuiltinFunction2)((_, item, options) => ToTemporalDateTime(item, options, intrinsics)), 1d);
                    GlobalThis.DefineBuiltinFunctionProperty(constructor, "compare",
                        (BuiltinFunction2)((_, first, second) => (double)CompareDateTimes(
                            ToTemporalDateTime(first, null, intrinsics), ToTemporalDateTime(second, null, intrinsics))), 2d);
                }
            });

    internal static JsObject GetCalendarObjectPrototype(RuntimeIntrinsics intrinsics, string kind)
        => intrinsics.GetOrCreate(CalendarObjectSlots(kind).Prototype, static () => new JsObject(),
            prototype =>
            {
                using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
                PrototypeChain.SetPrototype(prototype, intrinsics.ObjectPrototype);
                GlobalThis.DefineIntrinsicToStringTagProperty(prototype, "Temporal." + kind);
                GlobalThis.DefineIntrinsicDataProperty(prototype, "constructor", GetCalendarObjectConstructor(intrinsics, kind));
                TemporalCalendarObject Require(object? receiver)
                    => receiver is TemporalCalendarObject value && value.Kind == kind ? value
                        : throw new TypeError("Temporal." + kind + " getter called on incompatible receiver");
                DefineTemporalGetter(prototype, "calendarId", receiver => Require(receiver).CalendarId);
                if (kind != "PlainMonthDay")
                {
                    DefineTemporalGetter(prototype, "year", receiver => (double)Require(receiver).Year);
                    DefineTemporalGetter(prototype, "month", receiver => (double)Require(receiver).Month);
                }
                DefineTemporalGetter(prototype, "monthCode",
                    receiver => "M" + Require(receiver).Month.ToString("D2", CultureInfo.InvariantCulture));
                if (kind != "PlainYearMonth")
                {
                    DefineTemporalGetter(prototype, "day", receiver => (double)Require(receiver).Day);
                }
                if (kind is "PlainDateTime" or "ZonedDateTime")
                {
                    for (var i = 0; i < TimeFieldNames.Length; i++)
                    {
                        var index = i;
                        DefineTemporalGetter(prototype, TimeFieldNames[i], receiver => (double)Require(receiver).Time.GetField(index));
                    }
                }
                if (kind == "PlainDateTime")
                {
                    long GetEpochDays(TemporalCalendarObject date)
                        => ISODaysFromEpoch(new ISODate(date.Year, date.Month, date.Day));

                    (int Year, int Week) GetISOWeek(object? receiver)
                        => ISOWeek(GetEpochDays(Require(receiver)));

                    DefineTemporalGetter(prototype, "era", receiver => { Require(receiver); return null; });
                    DefineTemporalGetter(prototype, "eraYear", receiver => { Require(receiver); return null; });
                    DefineTemporalGetter(prototype, "dayOfWeek", receiver => (double)ISODayOfWeek(GetEpochDays(Require(receiver))));
                    DefineTemporalGetter(prototype, "dayOfYear", receiver =>
                    {
                        var date = Require(receiver);
                        return (double)(GetEpochDays(date) - ISODaysFromEpoch(new ISODate(date.Year, 1, 1)) + 1);
                    });
                    DefineTemporalGetter(prototype, "weekOfYear", receiver => (double)GetISOWeek(receiver).Week);
                    DefineTemporalGetter(prototype, "yearOfWeek", receiver => (double)GetISOWeek(receiver).Year);
                    DefineTemporalGetter(prototype, "daysInWeek", receiver => { Require(receiver); return 7d; });
                    DefineTemporalGetter(prototype, "daysInMonth", receiver =>
                    {
                        var date = Require(receiver);
                        return (double)ISODaysInMonth(date.Year, date.Month);
                    });
                    DefineTemporalGetter(prototype, "daysInYear", receiver => IsISOLeapYear(Require(receiver).Year) ? 366d : 365d);
                    DefineTemporalGetter(prototype, "monthsInYear", receiver => { Require(receiver); return 12d; });
                    DefineTemporalGetter(prototype, "inLeapYear", receiver => IsISOLeapYear(Require(receiver).Year));
                    GlobalThis.DefineBuiltinFunctionProperty(prototype, "equals",
                        (BuiltinFunction1)((receiver, other) => CompareDateTimes(
                            Require(receiver), ToTemporalDateTime(other, null, intrinsics)) == 0), 1d);
                }
                if (kind == "ZonedDateTime")
                {
                    DefineTemporalGetter(prototype, "epochNanoseconds", receiver => Require(receiver).EpochNanoseconds);
                    DefineTemporalGetter(prototype, "timeZoneId", receiver => Require(receiver).TimeZoneId);
                }
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "toString",
                    (BuiltinFunction1)((receiver, options) => FormatCalendarObject(Require(receiver), options)), 0d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "toJSON",
                    (BuiltinFunction0)(receiver => FormatCalendarObject(Require(receiver), null)), 0d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "toLocaleString",
                    (BuiltinFunction0)(receiver => FormatCalendarObject(Require(receiver), null)), 0d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "valueOf",
                    (BuiltinFunction0)(_ => throw new TypeError("Cannot convert Temporal." + kind + " to a primitive value")), 0d);
            });

    private static int CompareDateTimes(TemporalCalendarObject first, TemporalCalendarObject second)
    {
        var days = ISODaysFromEpoch(new ISODate(first.Year, first.Month, first.Day))
            .CompareTo(ISODaysFromEpoch(new ISODate(second.Year, second.Month, second.Day)));
        return days == 0 ? first.Time.Nanoseconds.CompareTo(second.Time.Nanoseconds) : days;
    }

    private static TemporalCalendarObject ToTemporalDateTime(object? item, object? options, RuntimeIntrinsics intrinsics)
    {
        ISODate date;
        long time = 0;
        if (item is TemporalCalendarObject calendarObject && calendarObject.Kind is "PlainDateTime" or "ZonedDateTime")
        {
            GetRejectOverflow(GetTemporalOptions(options));
            date = new ISODate(calendarObject.Year, calendarObject.Month, calendarObject.Day);
            time = calendarObject.Time.Nanoseconds;
        }
        else if (item is TemporalPlainDate plainDate)
        {
            GetRejectOverflow(GetTemporalOptions(options));
            date = new ISODate(plainDate.Year, plainDate.Month, plainDate.Day);
        }
        else if (!TypeUtilities.IsPrimitive(item))
        {
            if (item is not ITemporalCalendarRecord && ObjectRuntime.GetItem(item!, "calendar") is object calendar)
            {
                ToCalendarIdentifier(calendar);
            }

            var day = ReadDateNumber(item!, "day");
            var fields = new double[6];
            // Temporal reads field names in alphabetical order, not clock order.
            fields[(int)TimeUnit.Hour] = ReadDateNumber(item!, "hour") ?? 0;
            fields[(int)TimeUnit.Microsecond] = ReadDateNumber(item!, "microsecond") ?? 0;
            fields[(int)TimeUnit.Millisecond] = ReadDateNumber(item!, "millisecond") ?? 0;
            fields[(int)TimeUnit.Minute] = ReadDateNumber(item!, "minute") ?? 0;
            var month = ReadDateNumber(item!, "month");
            var monthCode = ReadTemporalMonthCode(item!);
            fields[(int)TimeUnit.Nanosecond] = ReadDateNumber(item!, "nanosecond") ?? 0;
            fields[(int)TimeUnit.Second] = ReadDateNumber(item!, "second") ?? 0;
            var year = ReadDateNumber(item!, "year");
            var reject = GetRejectOverflow(GetTemporalOptions(options));
            date = RegulateISODate(year, month, monthCode, day, reject);
            time = RegulateTime(fields, reject);
        }
        else
        {
            if (item is not string text)
            {
                throw new TypeError("Temporal date-time must be a string or an object");
            }
            var parsed = ParseISODateTime(text, allowDateOnly: true);
            date = ValidateParsedDateTime(parsed, allowUtc: false);
            ValidateCalendarIdentifier(parsed.Annotations.Calendar);
            time = parsed.Clock.Length == 0 ? 0 : ParseInstantClock(parsed);
            GetRejectOverflow(GetTemporalOptions(options));
        }
        if (!ISODateTimeWithinLimits(ISODaysFromEpoch(date), time))
        {
            throw new RangeError("Temporal.PlainDateTime is outside its valid range");
        }
        var result = new TemporalCalendarObject("PlainDateTime", date.Year, date.Month, date.Day,
            time, default, null, 0);
        PrototypeChain.SetPrototype(result, GetCalendarObjectPrototype(intrinsics, "PlainDateTime"));
        return result;
    }

    private static string FormatCalendarObject(TemporalCalendarObject value, object? suppliedOptions)
    {
        var options = GetTemporalOptions(suppliedOptions);
        var calendarName = GetTemporalStringOption(options, "calendarName", ["auto", "always", "never", "critical"], "auto");
        var showReferenceField = calendarName is "always" or "critical";
        switch (value.Kind)
        {
            case "PlainYearMonth":
                return PadISOYear(value.Year) + "-" + value.Month.ToString("D2", CultureInfo.InvariantCulture)
                    + (showReferenceField ? "-" + value.Day.ToString("D2", CultureInfo.InvariantCulture) : "")
                    + FormatCalendarAnnotation(calendarName);
            case "PlainMonthDay":
                return (showReferenceField ? PadISOYear(value.Year) + "-" : "")
                    + value.Month.ToString("D2", CultureInfo.InvariantCulture)
                    + "-" + value.Day.ToString("D2", CultureInfo.InvariantCulture)
                    + FormatCalendarAnnotation(calendarName);
        }
        var digits = GetTemporalFractionalSecondDigitsOption(options);
        var showOffset = value.Kind == "ZonedDateTime"
            ? GetTemporalStringOption(options, "offset", ["auto", "never"], "auto")
            : null;
        var mode = GetTemporalRoundingMode(options, TemporalRoundingMode.Trunc);
        var unit = GetTimeUnit(GetTemporalUnitOption(options, "smallestUnit"), false);
        var timeZoneName = value.Kind == "ZonedDateTime"
            ? GetTemporalStringOption(options, "timeZoneName", ["auto", "never", "critical"], "auto")
            : null;
        if (unit == TimeUnit.Hour)
        {
            throw new RangeError("Temporal." + value.Kind + ".toString does not support hour precision");
        }
        var precision = ToSecondsStringPrecisionRecord(unit, digits);
        long epochDays;
        long time;
        if (value.Kind == "ZonedDateTime")
        {
            var rounded = RoundInstantNanoseconds(value.EpochNanoseconds, precision.NanosecondIncrement, mode);
            var local = rounded + value.OffsetMinutes * TimeUnitLength(TimeUnit.Minute);
            var days = FloorDivide(local, NanosecondsPerDay);
            epochDays = (long)days;
            time = (long)(local - days * NanosecondsPerDay);
        }
        else
        {
            var rounded = (long)RoundTimeDuration(value.Time.Nanoseconds, precision.NanosecondIncrement, mode);
            epochDays = ISODaysFromEpoch(new ISODate(value.Year, value.Month, value.Day)) + rounded / NanosecondsPerDay;
            time = rounded % NanosecondsPerDay;
            if (!ISODateTimeWithinLimits(epochDays, time))
            {
                throw new RangeError("Rounded Temporal.PlainDateTime is outside its valid range");
            }
        }
        var date = ISODateFromEpochDays(epochDays);
        var result = FormatISODate(date.Year, date.Month, date.Day) + "T" + FormatTimeOfDay(time, precision);
        if (value.Kind == "ZonedDateTime")
        {
            if (showOffset != "never")
            {
                result += FormatOffsetTimeZoneIdentifier(value.OffsetMinutes);
            }
            if (timeZoneName != "never")
            {
                result += "[" + (timeZoneName == "critical" ? "!" : "") + value.TimeZoneId + "]";
            }
        }
        return result + FormatCalendarAnnotation(calendarName);
    }

    private static string FormatTimeOfDay(long time, SecondsStringPrecision precision)
    {
        var result = (time / TimeUnitLength(TimeUnit.Hour)).ToString("D2", CultureInfo.InvariantCulture)
            + ":" + (time / TimeUnitLength(TimeUnit.Minute) % 60).ToString("D2", CultureInfo.InvariantCulture);
        if (precision.Unit == TimeUnit.Minute)
        {
            return result;
        }
        result += ":" + (time / TimeUnitLength(TimeUnit.Second) % 60).ToString("D2", CultureInfo.InvariantCulture);
        var fraction = (time % TimeUnitLength(TimeUnit.Second)).ToString("D9", CultureInfo.InvariantCulture);
        fraction = precision.Digits >= 0 ? fraction[..precision.Digits] : fraction.TrimEnd('0');
        return fraction.Length == 0 ? result : result + "." + fraction;
    }

    private static string FormatOffsetTimeZoneIdentifier(int offsetMinutes)
    {
        var magnitude = System.Math.Abs(offsetMinutes);
        return (offsetMinutes < 0 ? "-" : "+") + (magnitude / 60).ToString("D2", CultureInfo.InvariantCulture)
            + ":" + (magnitude % 60).ToString("D2", CultureInfo.InvariantCulture);
    }

    // ISODateTimeWithinLimits: strictly within one day of the Instant limits.
    private static bool ISODateTimeWithinLimits(long epochDays, long timeNanoseconds)
    {
        var limit = new BigInteger(100_000_001) * NanosecondsPerDay;
        var value = new BigInteger(epochDays) * NanosecondsPerDay + timeNanoseconds;
        return -limit < value && value < limit;
    }

    private static bool ISOYearMonthWithinLimits(int year, int month)
        => year is > -271821 and < 275760
            || year == -271821 && month >= 4
            || year == 275760 && month <= 9;

    // IsValidISODate. Years beyond the Temporal limits are rejected here too;
    // the subsequent limit check would throw the same RangeError.
    private static ISODate CreateValidISODate(double year, double month, double day)
    {
        if (year is < -300_000 or > 300_000 || month is < 1 or > 12 || day is < 1 or > 31
            || !IsValidMonthDay((int)year, (int)month, (int)day))
        {
            throw new RangeError("Invalid Temporal ISO date");
        }
        return new ISODate((int)year, (int)month, (int)day);
    }

    // The constructor accepts identifiers only: UTC and offset time zones.
    private static (string Identifier, int OffsetMinutes) ParseZonedDateTimeTimeZone(string zone)
    {
        if (zone.Equals("UTC", StringComparison.OrdinalIgnoreCase))
        {
            return ("UTC", 0);
        }
        if (zone.Length != 0 && zone[0] is '+' or '-')
        {
            var minutes = ParseInstantTimeZoneOffsetMinutes(zone);
            return (FormatOffsetTimeZoneIdentifier(minutes), minutes);
        }
        throw new RangeError("Temporal.ZonedDateTime supports UTC and minute-precision offset time zones only");
    }

    private sealed class CalendarObjectConstructor(string kind) : JsFunctionObject
    {
        public override bool IsConstructor => true;
        internal override bool SelectsConstructedPrototype => true;

        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => throw new TypeError("Temporal." + kind + " requires 'new'");

        protected override object? ConstructCore(in JsCallArguments arguments, object? newTarget)
        {
            ISODate date;
            long clock = 0;
            BigInteger epochNanoseconds = default;
            string? timeZoneId = null;
            var offsetMinutes = 0;
            if (kind == "ZonedDateTime")
            {
                epochNanoseconds = BigInt.ToBigInt(arguments.GetArgument(0));
                _ = new TemporalInstant(epochNanoseconds);
                if (arguments.GetArgument(1) is not string zone)
                {
                    throw new TypeError("Temporal.ZonedDateTime requires a time-zone string");
                }
                (timeZoneId, offsetMinutes) = ParseZonedDateTimeTimeZone(zone);
                ValidateCalendarIdentifier(arguments.GetArgument(2));
                var local = epochNanoseconds + offsetMinutes * TimeUnitLength(TimeUnit.Minute);
                var days = FloorDivide(local, NanosecondsPerDay);
                date = ISODateFromEpochDays((long)days);
                clock = (long)(local - days * NanosecondsPerDay);
            }
            else if (kind == "PlainMonthDay")
            {
                var month = ToIntegerWithTruncation(arguments.GetArgument(0));
                var day = ToIntegerWithTruncation(arguments.GetArgument(1));
                ValidateCalendarIdentifier(arguments.GetArgument(2));
                var year = arguments.GetArgument(3) is null ? 1972 : ToIntegerWithTruncation(arguments.GetArgument(3));
                date = CreateValidISODate(year, month, day);
                ValidatePlainDate(date.Year, date.Month, date.Day);
            }
            else if (kind == "PlainYearMonth")
            {
                var year = ToIntegerWithTruncation(arguments.GetArgument(0));
                var month = ToIntegerWithTruncation(arguments.GetArgument(1));
                ValidateCalendarIdentifier(arguments.GetArgument(2));
                var day = arguments.GetArgument(3) is null ? 1 : ToIntegerWithTruncation(arguments.GetArgument(3));
                date = CreateValidISODate(year, month, day);
                if (!ISOYearMonthWithinLimits(date.Year, date.Month))
                {
                    throw new RangeError("Temporal.PlainYearMonth is outside its valid range");
                }
            }
            else
            {
                var year = ToIntegerWithTruncation(arguments.GetArgument(0));
                var month = ToIntegerWithTruncation(arguments.GetArgument(1));
                var day = ToIntegerWithTruncation(arguments.GetArgument(2));
                var fields = new double[6];
                for (var i = 0; i < fields.Length; i++)
                {
                    var argument = arguments.GetArgument(i + 3);
                    fields[i] = argument is null ? 0 : ToIntegerWithTruncation(argument);
                }
                ValidateCalendarIdentifier(arguments.GetArgument(9));
                date = CreateValidISODate(year, month, day);
                clock = RegulateTime(fields, true);
                if (!ISODateTimeWithinLimits(ISODaysFromEpoch(date), clock))
                {
                    throw new RangeError("Temporal.PlainDateTime is outside its valid range");
                }
            }
            var prototype = ObjectRuntime.GetItem(newTarget!, "prototype");
            if (!TypeUtilities.IsConstructorReturnOverride(prototype))
            {
                prototype = GetCalendarObjectPrototype(RuntimeIntrinsics.GetFunctionRealm(newTarget), kind);
            }
            var result = new TemporalCalendarObject(kind, date.Year, date.Month, date.Day, clock,
                epochNanoseconds, timeZoneId, offsetMinutes);
            PrototypeChain.SetPrototype(result, prototype);
            return result;
        }
    }
}

internal sealed class TemporalCalendarObject : JsObject, ITemporalCalendarRecord
{
    internal string Kind { get; }
    internal int Year { get; }
    internal int Month { get; }
    internal int Day { get; }
    internal TemporalPlainTime Time { get; }
    internal BigInteger EpochNanoseconds { get; }
    internal string? TimeZoneId { get; }
    internal int OffsetMinutes { get; }
    public string CalendarId => "iso8601";

    internal TemporalCalendarObject(string kind, int year, int month, int day, long nanoseconds,
        BigInteger epochNanoseconds, string? timeZoneId, int offsetMinutes)
    {
        Kind = kind;
        Year = year;
        Month = month;
        Day = day;
        Time = new TemporalPlainTime(nanoseconds);
        EpochNanoseconds = epochNanoseconds;
        TimeZoneId = timeZoneId;
        OffsetMinutes = offsetMinutes;
        PrototypeChain.InitializePrototype(this, Temporal.GetCalendarObjectPrototype(RuntimeIntrinsics.Current, kind));
    }
}
