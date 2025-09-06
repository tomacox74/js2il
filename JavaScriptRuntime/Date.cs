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
    [IntrinsicObject("Date")]
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

        // Instance methods
        public object getTime()
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
