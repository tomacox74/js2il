using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace JavaScriptRuntime
{
    /// <summary>
    /// Minimal JavaScript Date intrinsic to support common patterns used by the generator.
    /// Supported APIs:
    ///  - static now(): number (ms since Unix epoch)
    ///  - static parse(string): number (ms since Unix epoch) or NaN on failure
    ///  - new Date(): current time (UTC)
    ///  - new Date(ms): constructs from milliseconds since Unix epoch
    ///  - getTime(): number (ms since Unix epoch)
    ///  - toISOString(): string (UTC ISO 8601, e.g., 1970-01-01T00:00:00.000Z)
    /// Note: This is intentionally small and not spec-complete.
    /// </summary>
    [IntrinsicObject("Date", IntrinsicCallKind.DateToString)]
    public class Date : JsObject
    {
        /// <summary>Realm-owned <c>Date.prototype</c> (issue #1824).</summary>
        internal static object Prototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.DatePrototype,
                static () => new JsObject());

        private static readonly Regex DateOnlyRegex = new(
            @"^(?<year>[+-]?\d{4,6})(?:-(?<month>\d{2})(?:-(?<day>\d{2}))?)?$",
            RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static readonly Regex LocalDateTimeRegex = new(
            @"^(?<year>[+-]?\d{4,6})-(?<month>\d{2})-(?<day>\d{2})T(?<hour>\d{2}):(?<minute>\d{2})(?::(?<second>\d{2})(?:\.(?<fraction>\d{1,3}))?)?$",
            RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static readonly Regex IsoDateTimeRegex = new(
            @"^(?<year>[+-]\d{6}|\d{4})-(?<month>\d{2})-(?<day>\d{2})T(?<hour>\d{2}):(?<minute>\d{2})(?::(?<second>\d{2})(?:\.(?<fraction>\d{1,3}))?)?(?<zone>Z|[+-]\d{2}:\d{2})?$",
            RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static readonly Regex DateStringRegex = new(
            @"^(?:Sun|Mon|Tue|Wed|Thu|Fri|Sat) (?<month>Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) (?<day>\d{2}) (?<year>[+-]?\d{4,6}) (?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2}) GMT(?<zone>[+-]\d{4})$",
            RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private const double MillisecondsPerSecond = 1_000d;
        private const double MillisecondsPerMinute = 60d * MillisecondsPerSecond;
        private const double MillisecondsPerHour = 60d * MillisecondsPerMinute;
        private const double MillisecondsPerDay = 24d * MillisecondsPerHour;
        private const double TimeClipLimit = 8_640_000_000_000_000d;

        private readonly record struct DateParts(long Year, int Month, int Day, int Hour, int Minute, int Second, int Millisecond, int DayOfWeek);

        private double _msSinceEpoch; // milliseconds since Unix epoch (UTC), or NaN for invalid dates

        private static double NowMs() => System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        private static double TimeClipLike(double time)
        {
            if (!double.IsFinite(time) || System.Math.Abs(time) > TimeClipLimit)
            {
                return double.NaN;
            }

            var clipped = System.Math.Truncate(time);
            return clipped == 0d ? 0d : clipped;
        }

        private static double CoerceToMs(object? value)
        {
            switch (value)
            {
                case double d:
                    return TimeClipLike(d);
                case float f:
                    return TimeClipLike(f);
                case int i: return TimeClipLike(i);
                case long l: return TimeClipLike(l);
                case short s: return TimeClipLike(s);
                case byte b: return TimeClipLike(b);
                case bool bo: return TimeClipLike(bo ? 1 : 0);
                case JsNull: return 0d;
                case Date date: return date._msSinceEpoch;
                case string str:
                    var parsedStringDate = ParseInternal(str);
                    if (!double.IsNaN(parsedStringDate))
                    {
                        return parsedStringDate;
                    }

                    if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var nd))
                    {
                        return TimeClipLike(nd);
                    }
                    return double.NaN;
                default:
                    return TimeClipLike(TypeUtilities.ToNumber(value));
            }
        }

        // Constructors
        public Date()
        {
            _msSinceEpoch = NowMs();
            PrototypeChain.InitializePrototype(this, GlobalThis.DatePrototypeValue);
        }

        public Date(object? arg)
        {
            _msSinceEpoch = CoerceToMs(arg);
            PrototypeChain.InitializePrototype(this, GlobalThis.DatePrototypeValue);
        }

        private Date(double msSinceEpoch, bool _)
        {
            _msSinceEpoch = msSinceEpoch;
            PrototypeChain.InitializePrototype(this, GlobalThis.DatePrototypeValue);
        }

        public static object Construct()
        {
            return new Date();
        }

        public static object Construct(object? arg)
        {
            return new Date(arg);
        }

        public static object Construct(object[] args)
        {
            return args.Length switch
            {
                0 => new Date(),
                1 => new Date(args[0]),
                _ => new Date(CoerceComponentsToMs(args, useLocalTime: true), true)
            };
        }

        // Static methods
        public static object now()
        {
            return (double)NowMs();
        }

        public static object parse(string input)
        {
            return ParseInternal(input);
        }

        public static object UTC(object[] args)
        {
            return CoerceComponentsToMs(args, useLocalTime: false);
        }

        internal static void InitializeIntrinsicSurface(object objectPrototype)
        {
            GlobalThis.ConfigureBuiltinFunctionObject(typeof(Date));
            PrototypeChain.SetPrototype(Prototype, objectPrototype);

            PropertyDescriptorStore.DefineOrUpdate(typeof(Date), "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = Prototype
            });
            PropertyDescriptorStore.DefineOrUpdate(Prototype, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = typeof(Date)
            });
            PropertyDescriptorStore.DefineOrUpdate(typeof(Date), "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "Date"
            });
            PropertyDescriptorStore.DefineOrUpdate(typeof(Date), "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = 7d
            });
            PropertyDescriptorStore.DefineOrUpdate(Prototype, Symbol.toStringTag.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "Date"
            });

            DefineConstructorMethod("now", static (_, __) => now(), 0d);
            DefineConstructorMethod("parse", static (_, args) =>
            {
                var input = args != null && args.Length > 0 ? args[0] : null;
                return parse(DotNet2JSConversions.ToString(input));
            }, 1d);
            DefineConstructorMethod("UTC", static (_, args) => UTC(ConvertBuiltinArgs(args)), 7d);

            DefinePrototypeMethod("getDate", static (date, _) => date.getDate(), 0d);
            DefinePrototypeMethod("getDay", static (date, _) => date.getDay(), 0d);
            DefinePrototypeMethod("getFullYear", static (date, _) => date.getFullYear(), 0d);
            DefinePrototypeMethod("getHours", static (date, _) => date.getHours(), 0d);
            DefinePrototypeMethod("getMilliseconds", static (date, _) => date.getMilliseconds(), 0d);
            DefinePrototypeMethod("getMinutes", static (date, _) => date.getMinutes(), 0d);
            DefinePrototypeMethod("getMonth", static (date, _) => date.getMonth(), 0d);
            DefinePrototypeMethod("getSeconds", static (date, _) => date.getSeconds(), 0d);
            DefinePrototypeMethod("getTime", static (date, _) => date.getTime(), 0d);
            DefinePrototypeMethod("getTimezoneOffset", static (date, _) => date.getTimezoneOffset(), 0d);
            DefinePrototypeMethod("getUTCDate", static (date, _) => date.getUTCDate(), 0d);
            DefinePrototypeMethod("getUTCDay", static (date, _) => date.getUTCDay(), 0d);
            DefinePrototypeMethod("getUTCFullYear", static (date, _) => date.getUTCFullYear(), 0d);
            DefinePrototypeMethod("getUTCHours", static (date, _) => date.getUTCHours(), 0d);
            DefinePrototypeMethod("getUTCMilliseconds", static (date, _) => date.getUTCMilliseconds(), 0d);
            DefinePrototypeMethod("getUTCMinutes", static (date, _) => date.getUTCMinutes(), 0d);
            DefinePrototypeMethod("getUTCMonth", static (date, _) => date.getUTCMonth(), 0d);
            DefinePrototypeMethod("getUTCSeconds", static (date, _) => date.getUTCSeconds(), 0d);

            DefinePrototypeMethod("setDate", static (date, args) => date.setDate(GetBuiltinArg(args, 0)), 1d);
            DefinePrototypeMethod("setFullYear", static (date, args) => date.setFullYear(GetBuiltinArg(args, 0), GetBuiltinArg(args, 1), GetBuiltinArg(args, 2)), 3d);
            DefinePrototypeMethod("setHours", static (date, args) => date.setHours(GetBuiltinArg(args, 0), GetBuiltinArg(args, 1), GetBuiltinArg(args, 2), GetBuiltinArg(args, 3)), 4d);
            DefinePrototypeMethod("setMilliseconds", static (date, args) => date.setMilliseconds(GetBuiltinArg(args, 0)), 1d);
            DefinePrototypeMethod("setMinutes", static (date, args) => date.setMinutes(GetBuiltinArg(args, 0), GetBuiltinArg(args, 1), GetBuiltinArg(args, 2)), 3d);
            DefinePrototypeMethod("setMonth", static (date, args) => date.setMonth(GetBuiltinArg(args, 0), GetBuiltinArg(args, 1)), 2d);
            DefinePrototypeMethod("setSeconds", static (date, args) => date.setSeconds(GetBuiltinArg(args, 0), GetBuiltinArg(args, 1)), 2d);
            DefinePrototypeMethod("setTime", static (date, args) => date.setTime(GetBuiltinArg(args, 0)), 1d);
            DefinePrototypeMethod("setUTCDate", static (date, args) => date.setUTCDate(GetBuiltinArg(args, 0)), 1d);
            DefinePrototypeMethod("setUTCFullYear", static (date, args) => date.setUTCFullYear(GetBuiltinArg(args, 0), GetBuiltinArg(args, 1), GetBuiltinArg(args, 2)), 3d);
            DefinePrototypeMethod("setUTCHours", static (date, args) => date.setUTCHours(GetBuiltinArg(args, 0), GetBuiltinArg(args, 1), GetBuiltinArg(args, 2), GetBuiltinArg(args, 3)), 4d);
            DefinePrototypeMethod("setUTCMilliseconds", static (date, args) => date.setUTCMilliseconds(GetBuiltinArg(args, 0)), 1d);
            DefinePrototypeMethod("setUTCMinutes", static (date, args) => date.setUTCMinutes(GetBuiltinArg(args, 0), GetBuiltinArg(args, 1), GetBuiltinArg(args, 2)), 3d);
            DefinePrototypeMethod("setUTCMonth", static (date, args) => date.setUTCMonth(GetBuiltinArg(args, 0), GetBuiltinArg(args, 1)), 2d);
            DefinePrototypeMethod("setUTCSeconds", static (date, args) => date.setUTCSeconds(GetBuiltinArg(args, 0), GetBuiltinArg(args, 1)), 2d);

            DefinePrototypeMethod("toDateString", static (date, _) => date.toDateString(), 0d);
            DefinePrototypeMethod("toISOString", static (date, _) => date.toISOString(), 0d);
            DefineGenericPrototypeMethod("toJSON", static (value, _) => ToJson(value), 1d);
            DefinePrototypeMethod("toLocaleDateString", static (date, _) => date.toLocaleDateString(), 0d);
            DefinePrototypeMethod("toLocaleString", static (date, _) => date.toLocaleString(), 0d);
            DefinePrototypeMethod("toLocaleTimeString", static (date, _) => date.toLocaleTimeString(), 0d);
            DefinePrototypeMethod("toString", static (date, _) => date.toString(), 0d);
            DefinePrototypeMethod("toTimeString", static (date, _) => date.toTimeString(), 0d);
            DefinePrototypeMethod("toUTCString", static (date, _) => date.toUTCString(), 0d);
            DefinePrototypeMethod("valueOf", static (date, _) => date.valueOf(), 0d);

            var toPrimitive = (Func<object[], object?[]?, object?>)((_, args) =>
                ThisDateValue(RuntimeServices.GetCurrentThis()).toPrimitive(DotNet2JSConversions.ToString(GetBuiltinArg(args, 0))));
            Function.InitializeFunctionInstance(toPrimitive, 1d, "[Symbol.toPrimitive]");
            PropertyDescriptorStore.DefineOrUpdate(toPrimitive, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = null
            });
            PropertyDescriptorStore.DefineOrUpdate(Prototype, Symbol.toPrimitive.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = toPrimitive
            });
        }

        private static double ParseInternal(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return double.NaN;
            }

            input = input.Trim();
            if (input.StartsWith("-000000", StringComparison.Ordinal))
            {
                return double.NaN;
            }

            if (TryParseIsoDateTime(input, out var isoDateTimeMs))
            {
                return isoDateTimeMs;
            }

            if (TryParseIsoDateOnly(input, out var dateOnlyMs))
            {
                return dateOnlyMs;
            }

            if (TryParseIsoLocalDateTime(input, out var localDateTimeMs))
            {
                return localDateTimeMs;
            }

            if (TryParseDateString(input, out var dateStringMs))
            {
                return dateStringMs;
            }

            if (System.DateTimeOffset.TryParse(
                input,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var dto))
            {
                return TimeClipLike(dto.ToUnixTimeMilliseconds());
            }

            return double.NaN;
        }

        private static bool TryParseDateString(string input, out double msSinceEpoch)
        {
            var match = DateStringRegex.Match(input);
            if (!match.Success)
            {
                msSinceEpoch = double.NaN;
                return false;
            }

            var month = global::System.Array.IndexOf(MonthNames, match.Groups["month"].Value) + 1;
            if (!long.TryParse(match.Groups["year"].Value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var year)
                || !int.TryParse(match.Groups["day"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var day)
                || !int.TryParse(match.Groups["hour"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var hour)
                || !int.TryParse(match.Groups["minute"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var minute)
                || !int.TryParse(match.Groups["second"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var second)
                || month == 0
                || day is < 1 or > 31
                || day > DaysInMonth(year, month)
                || hour > 23
                || minute > 59
                || second > 59)
            {
                msSinceEpoch = double.NaN;
                return true;
            }

            var zone = match.Groups["zone"].Value;
            var offsetHour = int.Parse(zone.AsSpan(1, 2), CultureInfo.InvariantCulture);
            var offsetMinute = int.Parse(zone.AsSpan(3, 2), CultureInfo.InvariantCulture);
            if (offsetHour > 23 || offsetMinute > 59)
            {
                msSinceEpoch = double.NaN;
                return true;
            }

            var offset = offsetHour * MillisecondsPerHour + offsetMinute * MillisecondsPerMinute;
            if (zone[0] == '-')
            {
                offset = -offset;
            }

            msSinceEpoch = TimeClipLike(
                DaysFromCivil(year, month, day) * MillisecondsPerDay
                + hour * MillisecondsPerHour
                + minute * MillisecondsPerMinute
                + second * MillisecondsPerSecond
                - offset);
            return true;
        }

        private static bool TryParseIsoDateTime(string input, out double msSinceEpoch)
        {
            var match = IsoDateTimeRegex.Match(input);
            if (!match.Success)
            {
                msSinceEpoch = double.NaN;
                return false;
            }

            if (!long.TryParse(match.Groups["year"].Value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var year)
                || !int.TryParse(match.Groups["month"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var month)
                || !int.TryParse(match.Groups["day"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var day)
                || !int.TryParse(match.Groups["hour"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var hour)
                || !int.TryParse(match.Groups["minute"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var minute)
                || !TryParseBoundedInt(match.Groups["second"].Value, 0, 59, 0, out var second)
                || !TryParseMillisecond(match.Groups["fraction"].Value, out var millisecond)
                || month is < 1 or > 12
                || day is < 1 or > 31
                || hour is < 0 or > 24
                || minute is < 0 or > 59
                || (hour == 24 && (minute != 0 || second != 0 || millisecond != 0))
                || day > DaysInMonth(year, month))
            {
                msSinceEpoch = double.NaN;
                return true;
            }

            var days = DaysFromCivil(year, month, day) + (hour == 24 ? 1d : 0d);
            var time = (hour == 24 ? 0d : hour * MillisecondsPerHour)
                + minute * MillisecondsPerMinute
                + second * MillisecondsPerSecond
                + millisecond;
            var offset = ParseTimeZoneOffset(match.Groups["zone"].Value, out var hasTimeZone);
            if (double.IsNaN(offset))
            {
                msSinceEpoch = double.NaN;
                return true;
            }

            var result = days * MillisecondsPerDay + time;
            if (hasTimeZone)
            {
                result -= offset;
            }
            else
            {
                // The date-time form without an offset is local time.
                if (result >= -62_135_596_800_000d && result <= 253_402_300_799_999d)
                {
                    var local = DateTimeOffset.FromUnixTimeMilliseconds((long)result).DateTime;
                    result -= TimeZoneInfo.Local.GetUtcOffset(local).TotalMilliseconds;
                }
            }

            msSinceEpoch = TimeClipLike(result);
            return true;
        }

        private static double ParseTimeZoneOffset(string value, out bool hasTimeZone)
        {
            hasTimeZone = !string.IsNullOrEmpty(value);
            if (!hasTimeZone || value == "Z")
            {
                return 0d;
            }

            if (!int.TryParse(value.AsSpan(1, 2), NumberStyles.None, CultureInfo.InvariantCulture, out var hour)
                || !int.TryParse(value.AsSpan(4, 2), NumberStyles.None, CultureInfo.InvariantCulture, out var minute)
                || hour > 23
                || minute > 59)
            {
                return double.NaN;
            }

            var offset = hour * MillisecondsPerHour + minute * MillisecondsPerMinute;
            return value[0] == '-' ? -offset : offset;
        }

        private static bool TryParseIsoDateOnly(string input, out double msSinceEpoch)
        {
            var match = DateOnlyRegex.Match(input);
            if (!match.Success)
            {
                msSinceEpoch = double.NaN;
                return false;
            }

            if (!TryParseYear(match.Groups["year"].Value, out var year)
                || !TryParseBoundedInt(match.Groups["month"].Value, 1, 12, 1, out var month)
                || !TryParseBoundedInt(match.Groups["day"].Value, 1, 31, 1, out var day))
            {
                msSinceEpoch = double.NaN;
                return true;
            }

            if (!TryCreateDateTimeOffset(year, month, day, 0, 0, 0, 0, TimeSpan.Zero, out var dto))
            {
                msSinceEpoch = double.NaN;
                return true;
            }

            msSinceEpoch = TimeClipLike(dto.ToUnixTimeMilliseconds());
            return true;
        }

        private static bool TryParseIsoLocalDateTime(string input, out double msSinceEpoch)
        {
            var match = LocalDateTimeRegex.Match(input);
            if (!match.Success)
            {
                msSinceEpoch = double.NaN;
                return false;
            }

            if (!TryParseYear(match.Groups["year"].Value, out var year)
                || !TryParseBoundedInt(match.Groups["month"].Value, 1, 12, 1, out var month)
                || !TryParseBoundedInt(match.Groups["day"].Value, 1, 31, 1, out var day)
                || !TryParseBoundedInt(match.Groups["hour"].Value, 0, 24, 0, out var hour)
                || !TryParseBoundedInt(match.Groups["minute"].Value, 0, 59, 0, out var minute)
                || !TryParseBoundedInt(match.Groups["second"].Value, 0, 59, 0, out var second)
                || !TryParseMillisecond(match.Groups["fraction"].Value, out var millisecond))
            {
                msSinceEpoch = double.NaN;
                return true;
            }

            if (hour == 24 && (minute != 0 || second != 0 || millisecond != 0))
            {
                msSinceEpoch = double.NaN;
                return true;
            }

            var normalizedHour = hour == 24 ? 0 : hour;
            if (!TryCreateLocalDateTimeOffset(year, month, day, normalizedHour, minute, second, millisecond, out var dto))
            {
                msSinceEpoch = double.NaN;
                return true;
            }

            if (hour == 24)
            {
                dto = dto.AddDays(1);
            }

            msSinceEpoch = TimeClipLike(dto.ToUniversalTime().ToUnixTimeMilliseconds());
            return true;
        }

        private static bool TryParseYear(string value, out int year)
        {
            year = 0;
            if (!int.TryParse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var parsedYear))
            {
                return false;
            }

            if (parsedYear is < 1 or > 9999)
            {
                return false;
            }

            year = parsedYear;
            return true;
        }

        private static bool TryParseBoundedInt(string value, int min, int max, int defaultValue, out int parsed)
        {
            if (string.IsNullOrEmpty(value))
            {
                parsed = defaultValue;
                return true;
            }

            if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out parsed))
            {
                return false;
            }

            return parsed >= min && parsed <= max;
        }

        private static bool TryParseMillisecond(string value, out int millisecond)
        {
            millisecond = 0;
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            if (!int.TryParse(value.PadRight(3, '0'), NumberStyles.None, CultureInfo.InvariantCulture, out millisecond))
            {
                return false;
            }

            return millisecond is >= 0 and <= 999;
        }

        private static bool TryCreateLocalDateTimeOffset(
            int year,
            int month,
            int day,
            int hour,
            int minute,
            int second,
            int millisecond,
            out DateTimeOffset dto)
        {
            dto = default;

            try
            {
                var localDateTime = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Unspecified);
                dto = new DateTimeOffset(localDateTime, TimeZoneInfo.Local.GetUtcOffset(localDateTime));
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        private static bool TryCreateDateTimeOffset(
            int year,
            int month,
            int day,
            int hour,
            int minute,
            int second,
            int millisecond,
            TimeSpan offset,
            out DateTimeOffset dto)
        {
            dto = default;

            try
            {
                dto = new DateTimeOffset(year, month, day, hour, minute, second, offset).AddMilliseconds(millisecond);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        private static double CoerceComponentsToMs(object[] args, bool useLocalTime)
        {
            double year = args.Length > 0 ? TypeUtilities.ToNumber(args[0]) : double.NaN;
            double month = args.Length > 1 ? TypeUtilities.ToNumber(args[1]) : 0d;
            double date = args.Length > 2 ? TypeUtilities.ToNumber(args[2]) : 1d;
            double hours = args.Length > 3 ? TypeUtilities.ToNumber(args[3]) : 0d;
            double minutes = args.Length > 4 ? TypeUtilities.ToNumber(args[4]) : 0d;
            double seconds = args.Length > 5 ? TypeUtilities.ToNumber(args[5]) : 0d;
            double milliseconds = args.Length > 6 ? TypeUtilities.ToNumber(args[6]) : 0d;

            if (!double.IsFinite(year)
                || !double.IsFinite(month)
                || !double.IsFinite(date)
                || !double.IsFinite(hours)
                || !double.IsFinite(minutes)
                || !double.IsFinite(seconds)
                || !double.IsFinite(milliseconds))
            {
                return double.NaN;
            }

            year = System.Math.Truncate(year);
            month = System.Math.Truncate(month);
            date = System.Math.Truncate(date);
            hours = System.Math.Truncate(hours);
            minutes = System.Math.Truncate(minutes);
            seconds = System.Math.Truncate(seconds);
            milliseconds = System.Math.Truncate(milliseconds);

            if (year >= 0d && year <= 99d)
            {
                year += 1900d;
            }

            var day = MakeDay(year, month, date);
            var time = MakeTime(hours, minutes, seconds, milliseconds);
            var result = MakeDate(day, time);
            if (!useLocalTime || !double.IsFinite(result))
            {
                return TimeClipLike(result);
            }

            if (result < -62_135_596_800_000d || result > 253_402_300_799_999d)
            {
                return double.NaN;
            }

            var local = DateTimeOffset.FromUnixTimeMilliseconds((long)result).DateTime;
            return TimeClipLike(result - TimeZoneInfo.Local.GetUtcOffset(local).TotalMilliseconds);
        }

        // Instance methods
        public object getTime()
        {
            return (double)_msSinceEpoch;
        }

        public object getFullYear()
        {
            return GetLocalPart(static date => date.Year);
        }

        public object getMonth()
        {
            return GetLocalPart(static date => date.Month - 1);
        }

        public object getDate() => GetLocalPart(static date => date.Day);

        public object getDay() => GetLocalPart(static date => (double)date.DayOfWeek);

        public object getHours() => GetLocalPart(static date => date.Hour);

        public object getMilliseconds() => GetLocalPart(static date => date.Millisecond);

        public object getMinutes() => GetLocalPart(static date => date.Minute);

        public object getSeconds() => GetLocalPart(static date => date.Second);

        public object getTimezoneOffset()
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                return double.NaN;
            }

            if (_msSinceEpoch < -62_135_596_800_000d || _msSinceEpoch > 253_402_300_799_999d)
            {
                return 0d;
            }

            var offsetMinutes = -GetLocalDateTime().Offset.TotalMinutes;
            return offsetMinutes == 0d ? 0d : offsetMinutes;
        }

        public object getUTCDate() => GetUtcPart(static date => date.Day);

        public object getUTCDay() => GetUtcPart(static date => date.DayOfWeek);

        public object getUTCFullYear() => GetUtcPart(static date => date.Year);

        public object getUTCHours() => GetUtcPart(static date => date.Hour);

        public object getUTCMilliseconds() => GetUtcPart(static date => date.Millisecond);

        public object getUTCMinutes() => GetUtcPart(static date => date.Minute);

        public object getUTCMonth() => GetUtcPart(static date => date.Month - 1);

        public object getUTCSeconds() => GetUtcPart(static date => date.Second);

        public object valueOf()
        {
            return (double)_msSinceEpoch;
        }

        public object setTime(object? time)
        {
            _msSinceEpoch = TimeClipLike(TypeUtilities.ToNumber(time));
            return _msSinceEpoch;
        }

        public object setDate(object? date) => SetLocalDateParts(day: date);

        public object setFullYear(object? year, object? month = null, object? date = null) =>
            SetLocalDateParts(year: year, month: month, day: date);

        public object setHours(object? hours, object? minutes = null, object? seconds = null, object? milliseconds = null) =>
            SetLocalDateParts(hour: hours, minute: minutes, second: seconds, millisecond: milliseconds);

        public object setMilliseconds(object? milliseconds) => SetLocalDateParts(millisecond: milliseconds);

        public object setMinutes(object? minutes, object? seconds = null, object? milliseconds = null) =>
            SetLocalDateParts(minute: minutes, second: seconds, millisecond: milliseconds);

        public object setMonth(object? month, object? date = null) => SetLocalDateParts(month: month, day: date);

        public object setSeconds(object? seconds, object? milliseconds = null) =>
            SetLocalDateParts(second: seconds, millisecond: milliseconds);

        public object setUTCDate(object? date) => SetUtcDateParts(day: date);

        public object setUTCFullYear(object? year, object? month = null, object? date = null) =>
            SetUtcDateParts(year: year, month: month, day: date);

        public object setUTCHours(object? hours, object? minutes = null, object? seconds = null, object? milliseconds = null) =>
            SetUtcDateParts(hour: hours, minute: minutes, second: seconds, millisecond: milliseconds);

        public object setUTCMilliseconds(object? milliseconds) => SetUtcDateParts(millisecond: milliseconds);

        public object setUTCMinutes(object? minutes, object? seconds = null, object? milliseconds = null) =>
            SetUtcDateParts(minute: minutes, second: seconds, millisecond: milliseconds);

        public object setUTCMonth(object? month, object? date = null) => SetUtcDateParts(month: month, day: date);

        public object setUTCSeconds(object? seconds, object? milliseconds = null) =>
            SetUtcDateParts(second: seconds, millisecond: milliseconds);

        public string toDateString()
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                return "Invalid Date";
            }

            var parts = GetLocalParts(out _);
            return $"{DayNames[parts.DayOfWeek]} {MonthNames[parts.Month - 1]} {parts.Day:D2} {FormatYear(parts.Year)}";
        }

        public object toJSON()
        {
            return ToJson(this)!;
        }

        public string toLocaleDateString() => toDateString();

        public string toLocaleString() => toString();

        public string toLocaleTimeString() => toTimeString();

        internal static Date ThisDateValue(object? value)
        {
            if (value is Date date)
            {
                return date;
            }

            throw new TypeError("Date.prototype method called on incompatible receiver");
        }

        public string toISOString()
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                throw new RangeError("Invalid time value");
            }

            var parts = GetDateParts(_msSinceEpoch);
            return $"{FormatIsoYear(parts.Year)}-{parts.Month:D2}-{parts.Day:D2}T{parts.Hour:D2}:{parts.Minute:D2}:{parts.Second:D2}.{parts.Millisecond:D3}Z";
        }

        public string toString()
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                return "Invalid Date";
            }

            var local = GetLocalParts(out var offsetMinutes);
            return $"{DayNames[local.DayOfWeek]} {MonthNames[local.Month - 1]} {local.Day:D2} {FormatYear(local.Year)} {FormatTime(local)} {FormatTimeZone(offsetMinutes)}";
        }

        public string toTimeString()
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                return "Invalid Date";
            }

            var local = GetLocalParts(out var offsetMinutes);
            return $"{FormatTime(local)} {FormatTimeZone(offsetMinutes)}";
        }

        public string toUTCString()
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                return "Invalid Date";
            }

            var parts = GetDateParts(_msSinceEpoch);
            return $"{DayNames[parts.DayOfWeek]}, {parts.Day:D2} {MonthNames[parts.Month - 1]} {FormatYear(parts.Year)} {FormatTime(parts)} GMT";
        }

        public object toPrimitive(string? hint)
        {
            return hint switch
            {
                "number" => valueOf(),
                "string" => toString(),
                "default" or null => toString(),
                _ => throw new TypeError("Invalid hint")
            };
        }

        private static object? ToJson(object? value)
        {
            if (value is null || value is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            object? primitive = value;
            if (!TypeUtilities.IsPrimitive(value))
            {
                if (!TypeUtilities.TryCoerceObjectToPrimitive(value, "number", out primitive))
                {
                    throw new TypeError("Cannot convert object to primitive value");
                }
            }

            if (primitive is double or float or int or long or short or byte
                && !double.IsFinite(TypeUtilities.ToNumber(primitive)))
            {
                return JsNull.Null;
            }

            var toIsoString = ObjectRuntime.GetProperty(value, "toISOString");
            if (!CallableOperations.IsCallable(toIsoString))
            {
                throw new TypeError("toISOString is not callable");
            }

            return CallableOperations.Call0(toIsoString, value)!;
        }

        private static readonly string[] DayNames = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
        private static readonly string[] MonthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

        private DateParts GetLocalParts(out int offsetMinutes)
        {
            offsetMinutes = 0;
            if (_msSinceEpoch >= -62_135_596_800_000d && _msSinceEpoch <= 253_402_300_799_999d)
            {
                offsetMinutes = (int)GetLocalDateTime().Offset.TotalMinutes;
            }

            return GetDateParts(_msSinceEpoch + offsetMinutes * MillisecondsPerMinute);
        }

        private static DateParts GetDateParts(double milliseconds)
        {
            var days = (long)System.Math.Floor(milliseconds / MillisecondsPerDay);
            var timeWithinDay = (long)(milliseconds - days * MillisecondsPerDay);
            if (timeWithinDay < 0)
            {
                timeWithinDay += (long)MillisecondsPerDay;
                days--;
            }

            var (year, month, day) = CivilFromDays(days);
            var hour = (int)(timeWithinDay / MillisecondsPerHour);
            timeWithinDay %= (long)MillisecondsPerHour;
            var minute = (int)(timeWithinDay / MillisecondsPerMinute);
            timeWithinDay %= (long)MillisecondsPerMinute;
            var second = (int)(timeWithinDay / MillisecondsPerSecond);
            var millisecond = (int)(timeWithinDay % MillisecondsPerSecond);
            var dayOfWeek = (int)PositiveModulo(days + 4, 7);
            return new DateParts(year, month, day, hour, minute, second, millisecond, dayOfWeek);
        }

        private static string FormatYear(long year)
            => year < 0 ? $"-{(-year):D4}" : year.ToString("D4", CultureInfo.InvariantCulture);

        private static string FormatIsoYear(long year)
            => year is >= 0 and <= 9999
                ? year.ToString("D4", CultureInfo.InvariantCulture)
                : $"{(year < 0 ? '-' : '+')}{System.Math.Abs(year):D6}";

        private static string FormatTime(DateParts parts)
            => $"{parts.Hour:D2}:{parts.Minute:D2}:{parts.Second:D2}";

        private static string FormatTimeZone(int offsetMinutes)
        {
            var sign = offsetMinutes < 0 ? '-' : '+';
            var absolute = System.Math.Abs(offsetMinutes);
            return $"GMT{sign}{absolute / 60:D2}{absolute % 60:D2}";
        }

        private static double MakeDay(double year, double month, double date)
        {
            if (!double.IsFinite(year) || !double.IsFinite(month) || !double.IsFinite(date)
                || System.Math.Abs(year) > long.MaxValue
                || System.Math.Abs(month) > long.MaxValue
                || System.Math.Abs(date) > long.MaxValue)
            {
                return double.NaN;
            }

            try
            {
                var normalizedYear = (long)year + FloorDivide((long)month, 12);
                var normalizedMonth = (int)PositiveModulo((long)month, 12) + 1;
                return DaysFromCivil(normalizedYear, normalizedMonth, 1) + (long)date - 1;
            }
            catch (OverflowException)
            {
                return double.NaN;
            }
        }

        private static double MakeTime(double hour, double minute, double second, double millisecond)
            => ((hour * MillisecondsPerHour + minute * MillisecondsPerMinute)
                + second * MillisecondsPerSecond)
                + millisecond;

        private static double MakeDate(double day, double time)
            => day * MillisecondsPerDay + time;

        private static long DaysFromCivil(long year, int month, int day)
        {
            var adjustedYear = year - (month <= 2 ? 1 : 0);
            var era = FloorDivide(adjustedYear, 400);
            var yearOfEra = adjustedYear - era * 400;
            var monthPrime = month + (month > 2 ? -3 : 9);
            var dayOfYear = (153 * monthPrime + 2) / 5 + day - 1;
            var dayOfEra = yearOfEra * 365 + yearOfEra / 4 - yearOfEra / 100 + dayOfYear;
            return era * 146_097 + dayOfEra - 719_468;
        }

        private static (long Year, int Month, int Day) CivilFromDays(long days)
        {
            var z = days + 719_468;
            var era = FloorDivide(z, 146_097);
            var dayOfEra = z - era * 146_097;
            var yearOfEra = (dayOfEra - dayOfEra / 1460 + dayOfEra / 36_524 - dayOfEra / 146_096) / 365;
            var year = yearOfEra + era * 400;
            var dayOfYear = dayOfEra - (365 * yearOfEra + yearOfEra / 4 - yearOfEra / 100);
            var monthPrime = (5 * dayOfYear + 2) / 153;
            var day = (int)(dayOfYear - (153 * monthPrime + 2) / 5 + 1);
            var month = (int)(monthPrime + (monthPrime < 10 ? 3 : -9));
            year += month <= 2 ? 1 : 0;
            return (year, month, day);
        }

        private static int DaysInMonth(long year, int month)
            => month switch
            {
                2 when year % 4 == 0 && (year % 100 != 0 || year % 400 == 0) => 29,
                2 => 28,
                4 or 6 or 9 or 11 => 30,
                _ => 31
            };

        private static long FloorDivide(long value, long divisor)
        {
            var quotient = value / divisor;
            var remainder = value % divisor;
            return remainder < 0 ? quotient - 1 : quotient;
        }

        private static long PositiveModulo(long value, long divisor)
        {
            var remainder = value % divisor;
            return remainder < 0 ? remainder + divisor : remainder;
        }

        private DateTimeOffset GetUtcDateTime()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds((long)_msSinceEpoch);
        }

        private DateTimeOffset GetLocalDateTime()
        {
            return GetUtcDateTime().ToLocalTime();
        }

        private object GetLocalPart(Func<DateTimeOffset, double> selector)
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                return double.NaN;
            }

            return selector(GetLocalDateTime());
        }

        private object GetUtcPart(Func<DateParts, double> selector)
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                return double.NaN;
            }

            return selector(GetDateParts(_msSinceEpoch));
        }

        private object SetLocalDateParts(
            object? year = null,
            object? month = null,
            object? day = null,
            object? hour = null,
            object? minute = null,
            object? second = null,
            object? millisecond = null)
        {
            return SetDateParts(year, month, day, hour, minute, second, millisecond);
        }

        private object SetUtcDateParts(
            object? year = null,
            object? month = null,
            object? day = null,
            object? hour = null,
            object? minute = null,
            object? second = null,
            object? millisecond = null)
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                _msSinceEpoch = double.NaN;
                return _msSinceEpoch;
            }

            var current = GetDateParts(_msSinceEpoch);
            var valid = TryResolveUtcDatePart(year, current.Year, out var resolvedYear);
            valid = TryResolveUtcDatePart(month, current.Month - 1, out var resolvedMonth) & valid;
            valid = TryResolveUtcDatePart(day, current.Day, out var resolvedDay) & valid;
            valid = TryResolveUtcDatePart(hour, current.Hour, out var resolvedHour) & valid;
            valid = TryResolveUtcDatePart(minute, current.Minute, out var resolvedMinute) & valid;
            valid = TryResolveUtcDatePart(second, current.Second, out var resolvedSecond) & valid;
            valid = TryResolveUtcDatePart(millisecond, current.Millisecond, out var resolvedMillisecond) & valid;
            if (!valid)
            {
                _msSinceEpoch = double.NaN;
                return _msSinceEpoch;
            }

            var date = MakeDate(
                MakeDay(resolvedYear, resolvedMonth, resolvedDay),
                MakeTime(resolvedHour, resolvedMinute, resolvedSecond, resolvedMillisecond));
            _msSinceEpoch = TimeClipLike(date);
            return _msSinceEpoch;
        }

        private object SetDateParts(
            object? year,
            object? month,
            object? day,
            object? hour,
            object? minute,
            object? second,
            object? millisecond)
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                _msSinceEpoch = double.NaN;
                return _msSinceEpoch;
            }

            var current = GetLocalDateTime();
            if (!TryResolveDatePart(year, current.Year, out var resolvedYear)
                || !TryResolveDatePart(month, current.Month - 1, out var resolvedMonth)
                || !TryResolveDatePart(day, current.Day, out var resolvedDay)
                || !TryResolveDatePart(hour, current.Hour, out var resolvedHour)
                || !TryResolveDatePart(minute, current.Minute, out var resolvedMinute)
                || !TryResolveDatePart(second, current.Second, out var resolvedSecond)
                || !TryResolveDatePart(millisecond, current.Millisecond, out var resolvedMillisecond))
            {
                _msSinceEpoch = double.NaN;
                return _msSinceEpoch;
            }

            try
            {
                var localDateTime = new DateTime(resolvedYear, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
                    .AddMonths(resolvedMonth)
                    .AddDays(resolvedDay - 1)
                    .AddHours(resolvedHour)
                    .AddMinutes(resolvedMinute)
                    .AddSeconds(resolvedSecond)
                    .AddMilliseconds(resolvedMillisecond);
                var localOffset = TimeZoneInfo.Local.GetUtcOffset(localDateTime);
                _msSinceEpoch = TimeClipLike(new DateTimeOffset(localDateTime, localOffset).ToUniversalTime().ToUnixTimeMilliseconds());
            }
            catch (ArgumentOutOfRangeException)
            {
                _msSinceEpoch = double.NaN;
            }

            return _msSinceEpoch;
        }

        private static bool TryResolveDatePart(object? value, int currentValue, out int resolvedValue)
        {
            if (value == null)
            {
                resolvedValue = currentValue;
                return true;
            }

            var number = TypeUtilities.ToNumber(value);
            if (double.IsNaN(number) || double.IsInfinity(number))
            {
                resolvedValue = 0;
                return false;
            }

            resolvedValue = (int)System.Math.Truncate(number);
            return true;
        }

        private static bool TryResolveUtcDatePart(object? value, double currentValue, out double resolvedValue)
        {
            if (value == null)
            {
                resolvedValue = currentValue;
                return true;
            }

            var number = TypeUtilities.ToNumber(value);
            if (!double.IsFinite(number))
            {
                resolvedValue = 0d;
                return false;
            }

            resolvedValue = System.Math.Truncate(number);
            return true;
        }

        private static void DefineConstructorMethod(string key, Func<object[], object?[]?, object?> implementation, double length)
        {
            Function.InitializeFunctionInstance(implementation, length, key);
            PropertyDescriptorStore.DefineOrUpdate(implementation, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = null
            });
            PropertyDescriptorStore.DefineOrUpdate(typeof(Date), key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = implementation
            });
        }

        private static void DefinePrototypeMethod(string key, Func<Date, object?[]?, object?> implementation, double length)
        {
            Func<object[], object?[]?, object?> functionValue = (_, args) =>
                implementation(ThisDateValue(RuntimeServices.GetCurrentThis()), args);
            DefinePrototypeFunction(key, functionValue, length);
        }

        private static void DefineGenericPrototypeMethod(string key, Func<object?, object?[]?, object?> implementation, double length)
        {
            Func<object[], object?[]?, object?> functionValue = (_, args) =>
                implementation(RuntimeServices.GetCurrentThis(), args);
            DefinePrototypeFunction(key, functionValue, length, hasPrototypeProperty: false);
        }

        private static void DefinePrototypeFunction(
            string key,
            Func<object[], object?[]?, object?> functionValue,
            double length,
            bool hasPrototypeProperty = true)
        {
            Function.InitializeFunctionInstance(functionValue, length, key);
            if (hasPrototypeProperty)
            {
                PropertyDescriptorStore.DefineOrUpdate(functionValue, "prototype", new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Enumerable = false,
                    Configurable = false,
                    Writable = false,
                    Value = null
                });
            }
            PropertyDescriptorStore.DefineOrUpdate(Prototype, key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = functionValue
            });
        }

        private static object? GetBuiltinArg(object?[]? args, int index)
        {
            return args != null && args.Length > index ? args[index] : null;
        }

        private static object[] ConvertBuiltinArgs(object?[]? args)
        {
            if (args == null || args.Length == 0)
            {
                return global::System.Array.Empty<object>();
            }

            var converted = new object[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                converted[i] = args[i]!;
            }

            return converted;
        }
    }
}
