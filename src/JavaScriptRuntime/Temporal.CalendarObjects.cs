using System.Globalization;
using System.Numerics;

namespace JavaScriptRuntime;

public static partial class Temporal
{
    // Calendar-bearing types share ISO storage, but retain distinct brands.
    // Their constructors and field accessors allow calendar conversion to use
    // internal slots without invoking user-defined calendar/calendarId getters.
    // Operations beyond this constructor/accessor surface are not yet supplied.
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
                if (kind == "ZonedDateTime")
                {
                    DefineTemporalGetter(prototype, "epochNanoseconds", receiver => Require(receiver).EpochNanoseconds);
                    DefineTemporalGetter(prototype, "timeZoneId", receiver => Require(receiver).TimeZoneId);
                }
            });

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
            if (kind == "ZonedDateTime")
            {
                epochNanoseconds = BigInt.ToBigInt(arguments.GetArgument(0));
                if (arguments.GetArgument(1) is not string zone)
                {
                    throw new TypeError("Temporal.ZonedDateTime requires a time-zone string");
                }
                var minutes = GetInstantTimeZoneOffsetMinutes(zone);
                ValidateCalendarIdentifier(arguments.GetArgument(2));
                _ = new TemporalInstant(epochNanoseconds);
                timeZoneId = zone.Equals("utc", StringComparison.OrdinalIgnoreCase) ? "UTC" : zone;
                var local = epochNanoseconds + minutes * TimeUnitLength(TimeUnit.Minute);
                var days = FloorDivide(local, NanosecondsPerDay);
                date = ISODateFromEpochDays((long)days);
                clock = (long)(local - days * NanosecondsPerDay);
            }
            else
            {
                var first = ToIntegerWithTruncation(arguments.GetArgument(0));
                var second = ToIntegerWithTruncation(arguments.GetArgument(1));
                double year, month, day;
                if (kind == "PlainMonthDay")
                {
                    ValidateCalendarIdentifier(arguments.GetArgument(2));
                    year = arguments.GetArgument(3) is null ? 1972 : ToIntegerWithTruncation(arguments.GetArgument(3));
                    month = first;
                    day = second;
                }
                else if (kind == "PlainYearMonth")
                {
                    ValidateCalendarIdentifier(arguments.GetArgument(2));
                    year = first;
                    month = second;
                    day = arguments.GetArgument(3) is null ? 1 : ToIntegerWithTruncation(arguments.GetArgument(3));
                }
                else
                {
                    year = first;
                    month = second;
                    day = ToIntegerWithTruncation(arguments.GetArgument(2));
                    var fields = new double[6];
                    for (var i = 0; i < fields.Length; i++)
                    {
                        var argument = arguments.GetArgument(i + 3);
                        fields[i] = argument is null ? 0 : ToIntegerWithTruncation(argument);
                    }
                    clock = RegulateTime(fields, true);
                    ValidateCalendarIdentifier(arguments.GetArgument(9));
                }
                var regulated = RegulateDate(year, month, null, day, true);
                date = new ISODate(regulated.Year, regulated.Month, regulated.Day);
            }
            var prototype = ObjectRuntime.GetItem(newTarget!, "prototype");
            if (!TypeUtilities.IsConstructorReturnOverride(prototype))
            {
                prototype = GetCalendarObjectPrototype(RuntimeIntrinsics.GetFunctionRealm(newTarget), kind);
            }
            var result = new TemporalCalendarObject(kind, date.Year, date.Month, date.Day, clock, epochNanoseconds, timeZoneId);
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
    public string CalendarId => "iso8601";

    internal TemporalCalendarObject(string kind, int year, int month, int day, long nanoseconds,
        BigInteger epochNanoseconds, string? timeZoneId)
    {
        Kind = kind;
        Year = year;
        Month = month;
        Day = day;
        Time = new TemporalPlainTime(nanoseconds);
        EpochNanoseconds = epochNanoseconds;
        TimeZoneId = timeZoneId;
        PrototypeChain.InitializePrototype(this, Temporal.GetCalendarObjectPrototype(RuntimeIntrinsics.Current, kind));
    }
}
