using System;
using System.Globalization;

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
        private long _msSinceEpoch; // milliseconds since Unix epoch (UTC)

        private static long NowMs() => System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        private static long CoerceToMs(object? value)
        {
            if (value == null) return NowMs(); // undefined -> now (best-effort minimal behavior)
            switch (value)
            {
                case double d:
                    if (double.IsNaN(d)) return 0; // treat NaN as +0 for minimal behavior
                    return (long)d;
                case float f:
                    if (float.IsNaN(f)) return 0;
                    return (long)f;
                case int i: return i;
                case long l: return l;
                case short s: return s;
                case byte b: return b;
                case bool bo: return bo ? 1 : 0;
                case JsNull: return 0; // Number(null) === +0
                case string str:
                    // Try parse as number first, then as date string
                    if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var nd))
                    {
                        if (double.IsNaN(nd)) return 0;
                        return (long)nd;
                    }
                    var parsed = ParseInternal(str);
                    if (double.IsNaN(parsed)) return 0;
                    return (long)parsed;
                default:
                    try
                    {
                        var s = DotNet2JSConversions.ToString(value);
                        var p = ParseInternal(s);
                        if (!double.IsNaN(p)) return (long)p;
                        if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var nd2)) return (long)nd2;
                    }
                    catch { }
                    return 0;
            }
        }

        // Constructors
        public Date()
        {
            _msSinceEpoch = NowMs();
        }

        public Date(object? arg)
        {
            _msSinceEpoch = CoerceToMs(arg);
        }

        private Date(long msSinceEpoch, bool _)
        {
            _msSinceEpoch = msSinceEpoch;
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
                _ => new Date(CoerceComponentsToMs(args), true)
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

        private static double ParseInternal(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return double.NaN;
            // Prefer DateTimeOffset parsing with invariant + Assume/Adjust to UTC
            if (System.DateTimeOffset.TryParse(
                input,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces,
                out var dto))
            {
                return (double)dto.ToUnixTimeMilliseconds();
            }
            return double.NaN;
        }

        private static long CoerceComponentsToMs(object[] args)
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
                return 0;
            }

            var yearInteger = (int)System.Math.Truncate(year);
            if (yearInteger is >= 0 and <= 99)
            {
                yearInteger += 1900;
            }

            try
            {
                var epoch = new DateTimeOffset(yearInteger, 1, 1, 0, 0, 0, TimeSpan.Zero)
                    .AddMonths((int)System.Math.Truncate(month))
                    .AddDays((int)System.Math.Truncate(date) - 1)
                    .AddHours((int)System.Math.Truncate(hours))
                    .AddMinutes((int)System.Math.Truncate(minutes))
                    .AddSeconds((int)System.Math.Truncate(seconds))
                    .AddMilliseconds(System.Math.Truncate(milliseconds));
                return epoch.ToUnixTimeMilliseconds();
            }
            catch
            {
                return 0;
            }
        }

        // Instance methods
        public object getTime()
        {
            return (double)_msSinceEpoch;
        }

        public object valueOf()
        {
            return (double)_msSinceEpoch;
        }

        public string toISOString()
        {
            try
            {
                var dto = System.DateTimeOffset.FromUnixTimeMilliseconds(_msSinceEpoch);
                return dto.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture);
            }
            catch
            {
                // In case of out-of-range, fall back to epoch
                return "1970-01-01T00:00:00.000Z";
            }
        }
    }
}
