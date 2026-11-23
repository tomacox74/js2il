using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace JavaScriptRuntime;

public static partial class Temporal
{
    private static PlainDateConstructor GetPlainDateConstructor(RuntimeIntrinsics intrinsics)
        => intrinsics.GetOrCreate(
            RuntimeIntrinsicSlot.TemporalPlainDateConstructor,
            static () => new PlainDateConstructor(),
            constructor =>
            {
                using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
                Function.InitializeFunctionInstance(constructor, 3d, "PlainDate", false);
                PropertyDescriptorStore.DefineOrUpdate(constructor, "prototype", new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Value = GetPlainDatePrototype(intrinsics),
                    Writable = false, Enumerable = false, Configurable = false
                });
                GlobalThis.DefineBuiltinFunctionProperty(constructor, "from",
                    (BuiltinFunction2)(static (_, item, options) => ToTemporalDate(item, options)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(constructor, "compare",
                    (BuiltinFunction2)(static (_, one, two) =>
                    {
                        var first = ToTemporalDate(one);
                        var second = ToTemporalDate(two);
                        return (double)first.EpochDays.CompareTo(second.EpochDays);
                    }), 2d);
            });

    internal static JsObject GetPlainDatePrototype(RuntimeIntrinsics intrinsics)
        => intrinsics.GetOrCreate(
            RuntimeIntrinsicSlot.TemporalPlainDatePrototype,
            static () => new JsObject(),
            prototype =>
            {
                using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
                PrototypeChain.SetPrototype(prototype, intrinsics.ObjectPrototype);
                GlobalThis.DefineIntrinsicToStringTagProperty(prototype, "Temporal.PlainDate");
                GlobalThis.DefineIntrinsicDataProperty(prototype, "constructor", GetPlainDateConstructor(intrinsics));
                DefineTemporalGetter(prototype, "calendarId", receiver => RequirePlainDate(receiver).CalendarId);
                DefineTemporalGetter(prototype, "year", receiver => (double)RequirePlainDate(receiver).Year);
                DefineTemporalGetter(prototype, "month", receiver => (double)RequirePlainDate(receiver).Month);
                DefineTemporalGetter(prototype, "monthCode",
                    receiver => "M" + RequirePlainDate(receiver).Month.ToString("D2", CultureInfo.InvariantCulture));
                DefineTemporalGetter(prototype, "day", receiver => (double)RequirePlainDate(receiver).Day);
                DefineTemporalGetter(prototype, "era", receiver => { RequirePlainDate(receiver); return null; });
                DefineTemporalGetter(prototype, "eraYear", receiver => { RequirePlainDate(receiver); return null; });
                DefineTemporalGetter(prototype, "dayOfWeek", receiver => (double)ISODayOfWeek(RequirePlainDate(receiver).EpochDays));
                DefineTemporalGetter(prototype, "dayOfYear", receiver =>
                {
                    var date = RequirePlainDate(receiver);
                    return (double)(date.EpochDays - ISODaysFromEpoch(new ISODate(date.Year, 1, 1)) + 1);
                });
                DefineTemporalGetter(prototype, "weekOfYear", receiver => (double)ISOWeek(RequirePlainDate(receiver).EpochDays).Week);
                DefineTemporalGetter(prototype, "yearOfWeek", receiver => (double)ISOWeek(RequirePlainDate(receiver).EpochDays).Year);
                DefineTemporalGetter(prototype, "daysInWeek", receiver => { RequirePlainDate(receiver); return 7d; });
                DefineTemporalGetter(prototype, "daysInMonth", receiver =>
                {
                    var date = RequirePlainDate(receiver);
                    return (double)ISODaysInMonth(date.Year, date.Month);
                });
                DefineTemporalGetter(prototype, "daysInYear",
                    receiver => IsISOLeapYear(RequirePlainDate(receiver).Year) ? 366d : 365d);
                DefineTemporalGetter(prototype, "monthsInYear", receiver => { RequirePlainDate(receiver); return 12d; });
                DefineTemporalGetter(prototype, "inLeapYear", receiver => IsISOLeapYear(RequirePlainDate(receiver).Year));
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "equals",
                    (BuiltinFunction1)(static (receiver, other) =>
                    {
                        var date = RequirePlainDate(receiver);
                        var second = ToTemporalDate(other);
                        return date.EpochDays == second.EpochDays && date.CalendarId == second.CalendarId;
                    }), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "add",
                    (BuiltinFunction2)(static (receiver, duration, options) => AddPlainDate(receiver, duration, options, false)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "subtract",
                    (BuiltinFunction2)(static (receiver, duration, options) => AddPlainDate(receiver, duration, options, true)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "toString",
                    (BuiltinFunction1)(static (receiver, options) => FormatPlainDate(RequirePlainDate(receiver), options)), 0d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "toJSON",
                    (BuiltinFunction0)(static receiver => FormatPlainDate(RequirePlainDate(receiver), null)), 0d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "toLocaleString",
                    (BuiltinFunction0)(static receiver => FormatPlainDate(RequirePlainDate(receiver), null)), 0d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "valueOf",
                    (BuiltinFunction0)(static _ => throw new TypeError("Cannot convert Temporal.PlainDate to a primitive value")), 0d);
            });

    private static TemporalPlainDate RequirePlainDate(object? receiver)
        => receiver as TemporalPlainDate
            ?? throw new TypeError("Temporal.PlainDate method called on incompatible receiver");

    private static bool IsISOLeapYear(int year) => year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);

    private static int ISODaysInMonth(int year, int month)
        => month == 2 ? IsISOLeapYear(year) ? 29 : 28 : month is 4 or 6 or 9 or 11 ? 30 : 31;

    private static int ISODayOfWeek(long epochDays) => (int)(((epochDays + 3) % 7 + 7) % 7 + 1);

    private static (int Year, int Week) ISOWeek(long epochDays)
    {
        // An ISO week belongs to the year containing its Thursday.
        var thursday = epochDays + 4 - ISODayOfWeek(epochDays);
        var year = ISODateFromEpochDays(thursday).Year;
        var firstDay = ISODaysFromEpoch(new ISODate(year, 1, 1));
        return (year, (int)((thursday - firstDay) / 7) + 1);
    }

    // Constructors accept identifiers only; conversion of calendar-like values
    // additionally accepts ISO strings and calendar-bearing Temporal slots.
    private static string ValidateCalendarIdentifier(object? value)
    {
        if (value is null)
        {
            return "iso8601";
        }
        if (value is not string text)
        {
            throw new TypeError("Temporal calendar must be a string");
        }
        if (!text.Equals("iso8601", StringComparison.OrdinalIgnoreCase))
        {
            throw new RangeError("Unsupported Temporal calendar");
        }
        return "iso8601";
    }

    private static string ToCalendarIdentifier(object? value)
    {
        if (value is ITemporalCalendarRecord record)
        {
            return record.CalendarId;
        }
        if (value is not string text)
        {
            throw new TypeError("Temporal calendar must be a string or a calendar-bearing Temporal object");
        }
        if (text.Equals("iso8601", StringComparison.OrdinalIgnoreCase))
        {
            return "iso8601";
        }
        // Calendar conversion also accepts ISO year-month and month-day
        // strings. Resolve these against a leap reference year before parsing.
        var bracket = text.IndexOf('[');
        var core = bracket < 0 ? text : text[..bracket];
        var annotations = bracket < 0 ? "" : text[bracket..];
        if (Regex.IsMatch(core, @"\A(?:[+-][0-9]{6}|[0-9]{4})-[0-9]{2}\z", TemporalRegexOptions))
        {
            text = core + "-01" + annotations;
        }
        else if (Regex.IsMatch(core, @"\A(?:--)?[0-9]{2}-[0-9]{2}\z", TemporalRegexOptions))
        {
            text = "1972-" + core.TrimStart('-') + annotations;
        }
        var parsed = ParseISODateTime(text, allowDateOnly: true);
        if (parsed.Date is null)
        {
            // A TemporalTimeString also selects a calendar (iso8601 unless annotated).
            ParseTimeString(text);
        }
        else
        {
            ValidateParsedDateTime(parsed, allowUtc: true);
        }
        return ValidateCalendarIdentifier(parsed.Annotations.Calendar);
    }

    private static ISODate ValidateParsedDateTime(ISODateTime parsed, bool allowUtc)
    {
        if (parsed.Date is not ISODate date || !allowUtc && parsed.Offset is "Z" or "z")
        {
            throw new RangeError("Invalid Temporal date string");
        }
        if (parsed.Clock.Length != 0)
        {
            ParseInstantClock(parsed);
        }
        if (parsed.Offset.Length != 0 && parsed.Offset is not ("Z" or "z"))
        {
            ValidateTimeOffset(parsed.Offset);
        }
        return date;
    }

    private static TemporalPlainDate ToTemporalDate(object? item, object? suppliedOptions = null)
    {
        if (item is TemporalPlainDate date)
        {
            GetRejectOverflow(GetTemporalOptions(suppliedOptions));
            return new TemporalPlainDate(date.Year, date.Month, date.Day);
        }
        if (item is TemporalCalendarObject calendarObject && calendarObject.Kind is "PlainDateTime" or "ZonedDateTime")
        {
            GetRejectOverflow(GetTemporalOptions(suppliedOptions));
            return new TemporalPlainDate(calendarObject.Year, calendarObject.Month, calendarObject.Day);
        }
        if (!TypeUtilities.IsPrimitive(item))
        {
            // Other calendar-bearing Temporal objects supply their calendar
            // from internal slots; only plain objects read "calendar".
            if (item is not ITemporalCalendarRecord)
            {
                var calendar = ObjectRuntime.GetItem(item!, "calendar");
                if (calendar is not null)
                {
                    ToCalendarIdentifier(calendar);
                }
            }
            var fields = ReadDateFields(item!);
            var reject = GetRejectOverflow(GetTemporalOptions(suppliedOptions));
            return RegulateDate(fields.Year, fields.Month, fields.MonthCode, fields.Day, reject);
        }
        if (item is not string text)
        {
            throw new TypeError("Temporal date must be a string or an object");
        }
        var parsed = ParseISODateTime(text, allowDateOnly: true);
        var iso = ValidateParsedDateTime(parsed, allowUtc: false);
        ValidateCalendarIdentifier(parsed.Annotations.Calendar);
        GetRejectOverflow(GetTemporalOptions(suppliedOptions));
        return new TemporalPlainDate(iso.Year, iso.Month, iso.Day);
    }

    private static (double? Year, double? Month, string? MonthCode, double? Day) ReadDateFields(object item)
    {
        double? ReadNumber(string key)
        {
            var value = ObjectRuntime.GetItem(item, key);
            if (value is null)
            {
                return null;
            }
            var number = ToIntegerWithTruncation(value);
            if (key is "day" or "month" && number <= 0)
            {
                throw new RangeError("Temporal date month and day must be positive");
            }
            return number;
        }
        var day = ReadNumber("day");
        var month = ReadNumber("month");
        var rawCode = ObjectRuntime.GetItem(item, "monthCode");
        string? code = null;
        if (rawCode is not null)
        {
            // ParseMonthCode: ToPrimitive(value, string), then require a String.
            var primitive = rawCode;
            if (!TypeUtilities.IsPrimitive(rawCode)
                && !TypeUtilities.TryCoerceObjectToPrimitive(rawCode, "string", out primitive))
            {
                throw new TypeError("Cannot convert Temporal month code to a primitive value");
            }
            code = primitive as string ?? throw new TypeError("Temporal month code must be a string");
        }
        // MonthCode grammar (M00L, M01–M99, optional L) precedes conversion of
        // later fields; suitability for this calendar is checked after them.
        if (code is not null && !Regex.IsMatch(code, @"\A(?:M00L|M0[1-9]L?|M[1-9][0-9]L?)\z", TemporalRegexOptions))
        {
            throw new RangeError("Invalid Temporal month code");
        }
        var year = ReadNumber("year");
        return (year, month, code, day);
    }

    private static TemporalPlainDate RegulateDate(double? year, double? month, string? code, double? day, bool reject)
    {
        if (year is null || day is null || month is null && code is null)
        {
            throw new TypeError("A date requires year, month or monthCode, and day");
        }
        if (code is not null)
        {
            var codeMonth = int.Parse(code.AsSpan(1, 2), CultureInfo.InvariantCulture);
            if (code.Length != 3 || codeMonth is < 1 or > 12 || month is not null && month != codeMonth)
            {
                throw new RangeError("Invalid or conflicting Temporal month code");
            }
            month = codeMonth;
        }
        if (year < -271821 || year > 275760)
        {
            throw new RangeError("Temporal date year is outside its valid range");
        }
        var y = (int)year.Value;
        if (reject && (month < 1 || month > 12))
        {
            throw new RangeError("Temporal date month is outside its valid range");
        }
        var m = (int)System.Math.Clamp(month!.Value, 1, 12);
        var maximum = ISODaysInMonth(y, m);
        if (reject && (day < 1 || day > maximum))
        {
            throw new RangeError("Temporal date day is outside its valid range");
        }
        return new TemporalPlainDate(y, m, (int)System.Math.Clamp(day.Value, 1, maximum));
    }

    private static TemporalPlainDate AddPlainDate(object? receiver, object? value, object? options, bool subtract)
    {
        var date = RequirePlainDate(receiver);
        var duration = ToTemporalDuration(value);
        var reject = GetRejectOverflow(GetTemporalOptions(options));
        var sign = subtract ? -1 : 1;
        var year = new BigInteger(date.Year) + sign * new BigInteger(duration.Fields[0]);
        var month = new BigInteger(date.Month - 1) + sign * new BigInteger(duration.Fields[1]);
        var yearDelta = FloorDivide(month, 12);
        year += yearDelta;
        month -= yearDelta * 12;
        if (year < -271821 || year > 275760)
        {
            throw new RangeError("Temporal date arithmetic is outside its valid range");
        }
        var y = (int)year;
        var m = (int)month + 1;
        var day = date.Day;
        var maximum = ISODaysInMonth(y, m);
        if (reject && day > maximum)
        {
            throw new RangeError("Temporal date arithmetic overflows the target month");
        }
        day = System.Math.Min(day, maximum);
        // Time is balanced into whole days, truncating towards zero, before
        // calendar arithmetic. BigInteger preserves duration precision.
        var days = new BigInteger(ISODaysFromEpoch(new ISODate(y, m, day)))
            + sign * (new BigInteger(duration.Fields[2]) * 7
                + new BigInteger(duration.Fields[3]) + duration.TimeNanoseconds / NanosecondsPerDay);
        if (days < -100_000_001 || days > 100_000_000)
        {
            throw new RangeError("Temporal date arithmetic is outside its valid range");
        }
        var result = ISODateFromEpochDays((long)days);
        return new TemporalPlainDate(result.Year, result.Month, result.Day);
    }

    private static string FormatPlainDate(TemporalPlainDate date, object? suppliedOptions)
    {
        var calendarName = GetTemporalStringOption(GetTemporalOptions(suppliedOptions), "calendarName",
            ["auto", "always", "never", "critical"], "auto");
        return FormatISODate(date.Year, date.Month, date.Day) + FormatCalendarAnnotation(calendarName);
    }

    private static string GetTemporalStringOption(object? options, string name, string[] allowed, string fallback)
    {
        var raw = GetTemporalOption(options, name);
        var value = raw is null ? fallback : DotNet2JSConversions.ToStringRejectingSymbols(raw);
        return allowed.Contains(value)
            ? value
            : throw new RangeError("Invalid Temporal " + name + " option");
    }

    private static string PadISOYear(int year)
        => year is >= 0 and <= 9999
            ? year.ToString("D4", CultureInfo.InvariantCulture)
            : (year < 0 ? "-" : "+") + System.Math.Abs(year).ToString("D6", CultureInfo.InvariantCulture);

    private static string FormatISODate(int year, int month, int day)
        => PadISOYear(year) + "-" + month.ToString("D2", CultureInfo.InvariantCulture)
            + "-" + day.ToString("D2", CultureInfo.InvariantCulture);

    // Only the ISO calendar is supported, so "auto" never shows an annotation.
    private static string FormatCalendarAnnotation(string calendarName) => calendarName switch
    {
        "always" => "[u-ca=iso8601]",
        "critical" => "[!u-ca=iso8601]",
        _ => ""
    };

    private sealed class PlainDateConstructor : JsFunctionObject
    {
        public override bool IsConstructor => true;
        internal override bool SelectsConstructedPrototype => true;

        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => throw new TypeError("Temporal.PlainDate requires 'new'");

        protected override object? ConstructCore(in JsCallArguments arguments, object? newTarget)
        {
            var year = ToIntegerWithTruncation(arguments.GetArgument(0));
            var month = ToIntegerWithTruncation(arguments.GetArgument(1));
            var day = ToIntegerWithTruncation(arguments.GetArgument(2));
            ValidateCalendarIdentifier(arguments.GetArgument(3));
            var date = RegulateDate(year, month, null, day, true);
            var prototype = ObjectRuntime.GetItem(newTarget!, "prototype");
            if (!TypeUtilities.IsConstructorReturnOverride(prototype))
            {
                prototype = GetPlainDatePrototype(RuntimeIntrinsics.GetFunctionRealm(newTarget));
            }
            PrototypeChain.SetPrototype(date, prototype);
            return date;
        }
    }

    internal static long ValidatePlainDate(int year, int month, int day)
    {
        if (year is < -271821 or > 275760 || !IsValidMonthDay(year, month, day))
        {
            throw new RangeError("Invalid Temporal ISO date");
        }
        var days = ISODaysFromEpoch(new ISODate(year, month, day));
        if (days is < -100_000_001 or > 100_000_000)
        {
            throw new RangeError("Temporal date is outside its valid range");
        }
        return days;
    }
}

internal interface ITemporalCalendarRecord
{
    string CalendarId { get; }
}

public sealed class TemporalPlainDate : JsObject, ITemporalCalendarRecord
{
    internal int Year { get; }
    internal int Month { get; }
    internal int Day { get; }
    internal long EpochDays { get; }
    public string CalendarId => "iso8601";

    internal TemporalPlainDate(int year, int month, int day)
    {
        EpochDays = Temporal.ValidatePlainDate(year, month, day);
        Year = year;
        Month = month;
        Day = day;
        PrototypeChain.InitializePrototype(this, Temporal.GetPlainDatePrototype(RuntimeIntrinsics.Current));
    }
}
