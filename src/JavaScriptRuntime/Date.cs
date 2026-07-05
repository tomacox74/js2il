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
    public class Date
    {
        private static readonly Regex DateOnlyRegex = new(
            @"^(?<year>[+-]?\d{4,6})(?:-(?<month>\d{2})(?:-(?<day>\d{2}))?)?$",
            RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static readonly Regex LocalDateTimeRegex = new(
            @"^(?<year>[+-]?\d{4,6})-(?<month>\d{2})-(?<day>\d{2})T(?<hour>\d{2}):(?<minute>\d{2})(?::(?<second>\d{2})(?:\.(?<fraction>\d{1,3}))?)?$",
            RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private double _msSinceEpoch; // milliseconds since Unix epoch (UTC), or NaN for invalid dates

        private static double NowMs() => System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        private static double TimeClipLike(double time)
        {
            if (double.IsNaN(time) || double.IsInfinity(time) || System.Math.Abs(time) > 8_640_000_000_000_000d)
            {
                return double.NaN;
            }

            var clipped = System.Math.Truncate(time);
            return clipped == 0d ? 0d : clipped;
        }

        private static double CoerceToMs(object? value)
        {
            if (value == null) return NowMs(); // undefined -> now (best-effort minimal behavior)
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
                    try
                    {
                        var s = DotNet2JSConversions.ToString(value);
                        var p = ParseInternal(s);
                        if (!double.IsNaN(p)) return p;
                        if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var nd2)) return TimeClipLike(nd2);
                    }
                    catch { }
                    return double.NaN;
            }
        }

        // Constructors
        public Date()
        {
            _msSinceEpoch = NowMs();
            PrototypeChain.SetPrototype(this, GlobalThis.DatePrototypeValue);
        }

        public Date(object? arg)
        {
            _msSinceEpoch = CoerceToMs(arg);
            PrototypeChain.SetPrototype(this, GlobalThis.DatePrototypeValue);
        }

        private Date(double msSinceEpoch, bool _)
        {
            _msSinceEpoch = msSinceEpoch;
            PrototypeChain.SetPrototype(this, GlobalThis.DatePrototypeValue);
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

            if (TryParseIsoDateOnly(input, out var dateOnlyMs))
            {
                return dateOnlyMs;
            }

            if (TryParseIsoLocalDateTime(input, out var localDateTimeMs))
            {
                return localDateTimeMs;
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
                var localDateTime = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Local);
                dto = new DateTimeOffset(localDateTime);
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
            double month = args.Length > 1 ? TypeUtilities.ToNumber(args[1]) : double.NaN;
            double date = args.Length > 2 ? TypeUtilities.ToNumber(args[2]) : 1d;
            double hours = args.Length > 3 ? TypeUtilities.ToNumber(args[3]) : 0d;
            double minutes = args.Length > 4 ? TypeUtilities.ToNumber(args[4]) : 0d;
            double seconds = args.Length > 5 ? TypeUtilities.ToNumber(args[5]) : 0d;
            double milliseconds = args.Length > 6 ? TypeUtilities.ToNumber(args[6]) : 0d;

            if (double.IsNaN(year)
                || double.IsNaN(month)
                || double.IsNaN(date)
                || double.IsNaN(hours)
                || double.IsNaN(minutes)
                || double.IsNaN(seconds)
                || double.IsNaN(milliseconds))
            {
                return double.NaN;
            }

            var yearInteger = (int)System.Math.Truncate(year);
            if (yearInteger is >= 0 and <= 99)
            {
                yearInteger += 1900;
            }

            try
            {
                if (useLocalTime)
                {
                    var localDateTime = new DateTime(yearInteger, 1, 1, 0, 0, 0, DateTimeKind.Local)
                        .AddMonths((int)System.Math.Truncate(month))
                        .AddDays((int)System.Math.Truncate(date) - 1)
                        .AddHours((int)System.Math.Truncate(hours))
                        .AddMinutes((int)System.Math.Truncate(minutes))
                        .AddSeconds((int)System.Math.Truncate(seconds))
                        .AddMilliseconds(System.Math.Truncate(milliseconds));
                    return TimeClipLike(new DateTimeOffset(localDateTime).ToUniversalTime().ToUnixTimeMilliseconds());
                }

                var utcDateTime = new DateTimeOffset(yearInteger, 1, 1, 0, 0, 0, TimeSpan.Zero)
                    .AddMonths((int)System.Math.Truncate(month))
                    .AddDays((int)System.Math.Truncate(date) - 1)
                    .AddHours((int)System.Math.Truncate(hours))
                    .AddMinutes((int)System.Math.Truncate(minutes))
                    .AddSeconds((int)System.Math.Truncate(seconds))
                    .AddMilliseconds(System.Math.Truncate(milliseconds));
                return TimeClipLike(utcDateTime.ToUnixTimeMilliseconds());
            }
            catch
            {
                return double.NaN;
            }
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

            return -GetLocalDateTime().Offset.TotalMinutes;
        }

        public object getUTCDate() => GetUtcPart(static date => date.Day);

        public object getUTCDay() => GetUtcPart(static date => (double)date.DayOfWeek);

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

            return GetLocalDateTime().ToString("ddd MMM dd yyyy", CultureInfo.InvariantCulture);
        }

        public object toJSON()
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                return JsNull.Null;
            }

            return toISOString();
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

            return GetUtcDateTime().UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture);
        }

        public string toString()
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                return "Invalid Date";
            }

            var local = GetLocalDateTime();
            return $"{local:ddd MMM dd yyyy} {toTimeString()}";
        }

        public string toTimeString()
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                return "Invalid Date";
            }

            var local = GetLocalDateTime();
            return local.ToString("HH:mm:ss 'GMT'zzz", CultureInfo.InvariantCulture);
        }

        public string toUTCString()
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                return "Invalid Date";
            }

            return GetUtcDateTime().UtcDateTime.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture);
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

        private object GetUtcPart(Func<DateTimeOffset, double> selector)
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                return double.NaN;
            }

            return selector(GetUtcDateTime());
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
            return SetDateParts(year, month, day, hour, minute, second, millisecond, useLocalTime: true);
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
            return SetDateParts(year, month, day, hour, minute, second, millisecond, useLocalTime: false);
        }

        private object SetDateParts(
            object? year,
            object? month,
            object? day,
            object? hour,
            object? minute,
            object? second,
            object? millisecond,
            bool useLocalTime)
        {
            if (double.IsNaN(_msSinceEpoch))
            {
                _msSinceEpoch = double.NaN;
                return _msSinceEpoch;
            }

            var current = useLocalTime ? GetLocalDateTime() : GetUtcDateTime();
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
                var offset = useLocalTime ? current.Offset : TimeSpan.Zero;
                var updated = new DateTimeOffset(resolvedYear, 1, 1, 0, 0, 0, offset)
                    .AddMonths(resolvedMonth)
                    .AddDays(resolvedDay - 1)
                    .AddHours(resolvedHour)
                    .AddMinutes(resolvedMinute)
                    .AddSeconds(resolvedSecond)
                    .AddMilliseconds(resolvedMillisecond);
                _msSinceEpoch = TimeClipLike(updated.ToUniversalTime().ToUnixTimeMilliseconds());
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
    }
}
