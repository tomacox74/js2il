using System;
using System.Text.RegularExpressions;

namespace JavaScriptRuntime
{
    /// <summary>
    /// JavaScript String intrinsic helpers used by the IL generator.
    /// Focused, minimal surface to support common patterns during compilation.
    /// </summary>
    [IntrinsicObject("String")]
    public static class String
    {
        /// <summary>
        /// Implements a subset of String.prototype.startsWith(searchString[, position]).
        /// Uses ordinal comparison and basic ToIntegerOrInfinity coercion for position.
        /// </summary>
    public static object StartsWith(string input, string searchString)
        {
            return StartsWith(input, searchString, null);
        }

        /// <summary>
        /// Implements a subset of String.prototype.startsWith with optional position argument.
        /// </summary>
    public static object StartsWith(string input, string searchString, object? position)
        {
            input ??= string.Empty;
            searchString ??= string.Empty;

            int pos = 0;
            if (position != null)
            {
                try
                {
                    double d;
                    if (position is double dd) d = dd;
                    else if (position is float ff) d = ff;
                    else if (position is int ii) d = ii;
                    else if (position is long ll) d = ll;
                    else if (position is string s && double.TryParse(s, out var parsed)) d = parsed;
                    else if (position is IConvertible conv) d = conv.ToDouble(System.Globalization.CultureInfo.InvariantCulture);
                    else d = 0d;

                    if (double.IsNaN(d)) d = 0d;
                    if (double.IsPositiveInfinity(d)) d = input.Length;
                    if (double.IsNegativeInfinity(d)) d = 0d;
                    // Truncate toward zero
                    d = d >= 0 ? global::System.Math.Floor(d) : global::System.Math.Ceiling(d);
                    if (d < 0) d = 0;
                    if (d > input.Length) d = input.Length;
                    pos = (int)d;
                }
                catch { pos = 0; }
            }

            if (searchString.Length == 0)
            {
                return true; // empty string starts at any position (pos is clamped to length)
            }
            if (pos < 0 || pos > input.Length) return false;
            if (pos + searchString.Length > input.Length) return false;
            return input.AsSpan(pos).StartsWith(searchString.AsSpan(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Implements a subset of String.prototype.replace when the pattern is a plain string.
        /// Only replaces the first occurrence, matching JS behavior for string patterns.
        /// Replacement is coerced to string via ToString.
        /// </summary>
        public static string Replace(string input, object patternOrString, object replacement)
        {
            input ??= string.Empty;
            var pattern = patternOrString?.ToString() ?? string.Empty;
            var repl = replacement?.ToString() ?? string.Empty;
            if (pattern.Length == 0)
            {
                // JS inserts at start for empty pattern; keep simple and return input for now.
                return input;
            }
            var idx = input.IndexOf(pattern, StringComparison.Ordinal);
            if (idx < 0) return input;
            return input.Substring(0, idx) + repl + input.Substring(idx + pattern.Length);
        }

        /// <summary>
        /// Implements a subset of String.prototype.replace when the pattern is a regular expression literal
        /// and the replacement is a string. Supports global and ignoreCase flags.
        /// </summary>
        public static string Replace(string input, string pattern, string replacement, bool global, bool ignoreCase)
        {
            if (input == null) return string.Empty;
            var options = RegexOptions.CultureInvariant;
            if (ignoreCase) options |= RegexOptions.IgnoreCase;

            var re = new Regex(pattern, options);
            if (global)
            {
                return re.Replace(input, replacement);
            }
            else
            {
                // Replace only the first occurrence
                return re.Replace(input, replacement, 1);
            }
        }

        /// <summary>
        /// Implements a subset of String.prototype.localeCompare.
        /// Supports locales (ignored) and options; recognizes options.numeric === true to enable numeric-aware comparison.
        /// Returns -1, 0, or 1 as a JavaScript number (double).
        /// </summary>
        public static object LocaleCompare(string input, string other, object? locales, object? options)
        {
            input ??= string.Empty;
            other ??= string.Empty;
            var numeric = OptionsHasNumericTrue(options);
            if (!numeric)
            {
                return string.Compare(input, other, StringComparison.Ordinal) switch
                {
                    < 0 => -1d,
                    0 => 0d,
                    _ => 1d
                };
            }
            var cmp = NumericAwareCompare(input, other);
            return cmp < 0 ? -1d : (cmp > 0 ? 1d : 0d);
        }

        private static bool OptionsHasNumericTrue(object? options)
        {
            if (options == null) return false;

            // Handle ExpandoObject/dynamic dictionaries
            if (options is System.Collections.Generic.IDictionary<string, object> dict)
            {
                foreach (var kv in dict)
                {
                    if (string.Equals(kv.Key, "numeric", StringComparison.OrdinalIgnoreCase))
                    {
                        var val = kv.Value;
                        if (val is bool b) return b;
                        if (val is string s && bool.TryParse(s, out var pb)) return pb;
                        if (val is IConvertible c) { try { return Convert.ToBoolean(c); } catch { }
                        }
                        return false;
                    }
                }
            }

            // Fallback: try reflection for POCO-style options objects
            try
            {
                var t = options.GetType();
                var prop = t.GetProperty("numeric", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                if (prop != null)
                {
                    var val = prop.GetValue(options);
                    if (val is bool b) return b;
                    if (val is string s && bool.TryParse(s, out var pb)) return pb;
                    if (val is IConvertible c) { try { return Convert.ToBoolean(c); } catch { } }
                }
            }
            catch { }
            return false;
        }

        private static int NumericAwareCompare(string a, string b)
        {
            int ia = 0, ib = 0;
            while (ia < a.Length && ib < b.Length)
            {
                char ca = a[ia];
                char cb = b[ib];
                bool da = char.IsDigit(ca);
                bool db = char.IsDigit(cb);
                if (da && db)
                {
                    // Compare consecutive digit runs numerically
                    long na = 0, nb = 0;
                    int sa = ia, sb = ib;
                    while (ia < a.Length && char.IsDigit(a[ia])) { na = na * 10 + (a[ia] - '0'); ia++; }
                    while (ib < b.Length && char.IsDigit(b[ib])) { nb = nb * 10 + (b[ib] - '0'); ib++; }
                    if (na != nb) return na < nb ? -1 : 1;
                    // If equal numbers, shorter digit run sorts before longer (e.g., 2 vs 02 -> 2 < 02)
                    int la = ia - sa, lb = ib - sb;
                    if (la != lb) return la < lb ? -1 : 1;
                    continue;
                }
                if (ca != cb) return ca < cb ? -1 : 1;
                ia++; ib++;
            }
            if (ia == a.Length && ib == b.Length) return 0;
            return ia == a.Length ? -1 : 1;
        }

        /// <summary>
        /// Implements a subset of String.prototype.split(separator[, limit]).
        /// Supports string or regular expression separators and optional limit.
        /// Returns a JavaScriptRuntime.Array of strings.
        /// </summary>
        public static object Split(string input, object? separatorOrPattern)
        {
            return Split(input, separatorOrPattern, null);
        }

        /// <summary>
        /// Split with optional limit. Separator can be string, Regex, or null/undefined.
        /// </summary>
        public static object Split(string input, object? separatorOrPattern, object? limit)
        {
            input ??= string.Empty;
            int maxCount = ToSplitLimit(limit);
            if (maxCount == 0)
            {
                return new JavaScriptRuntime.Array();
            }

            // Undefined/null separator => return [input]
            if (separatorOrPattern == null)
            {
                var one = new JavaScriptRuntime.Array(1) { input };
                return one;
            }

            // Regex separator path
            if (separatorOrPattern is Regex rx)
            {
                return SplitWithRegex(input, rx, maxCount);
            }

            // String (or coercible) separator
            var sep = DotNet2JSConversions.ToString(separatorOrPattern);
            if (sep.Length == 0)
            {
                // Empty string => split into UTF-16 code units
                int take = global::System.Math.Min(input.Length, maxCount);
                var arr = new JavaScriptRuntime.Array(take);
                for (int i = 0; i < take; i++) arr.Add(input[i].ToString());
                return arr;
            }

            // Manual split to enforce Ordinal comparison and JS-like limit behavior
            var result = new JavaScriptRuntime.Array();
            int start = 0;
            while (true)
            {
                if (result.Count >= maxCount) break;
                int idx = input.IndexOf(sep, start, StringComparison.Ordinal);
                if (idx < 0)
                {
                    // Remainder (including empty string when separator at end)
                    result.Add(idx == -1 ? input.Substring(start) : string.Empty);
                    break;
                }

                result.Add(input.Substring(start, idx - start));
                start = idx + sep.Length;

                // If start == input.Length and we split at the end, append empty string (JS behavior)
                if (start == input.Length && result.Count < maxCount)
                {
                    result.Add(string.Empty);
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Split using a regex pattern with flags provided by the generator.
        /// Currently only ignoreCase is observed for split behavior.
        /// </summary>
        public static object Split(string input, string pattern, bool ignoreCase)
        {
            return Split(input, pattern, ignoreCase, null);
        }

        /// <summary>
        /// Split using a regex pattern with optional ignoreCase and limit.
        /// </summary>
        public static object Split(string input, string pattern, bool ignoreCase, object? limit)
        {
            input ??= string.Empty;
            int maxCount = ToSplitLimit(limit);
            if (maxCount == 0) return new JavaScriptRuntime.Array();
            var options = RegexOptions.CultureInvariant;
            if (ignoreCase) options |= RegexOptions.IgnoreCase;
            var rx = new Regex(pattern, options);
            return SplitWithRegex(input, rx, maxCount);
        }

        private static object SplitWithRegex(string input, Regex rx, int maxCount)
        {
            var parts = rx.Split(input);
            var arr = new JavaScriptRuntime.Array(parts.Length);
            for (int i = 0; i < parts.Length && i < maxCount; i++)
            {
                arr.Add(parts[i] ?? string.Empty);
            }
            return arr;
        }

        private static int ToSplitLimit(object? limit)
        {
            if (limit == null) return int.MaxValue; // Omitted => effectively unlimited
            try
            {
                double d;
                switch (limit)
                {
                    case double dd: d = dd; break;
                    case float ff: d = ff; break;
                    case int ii: d = ii; break;
                    case long ll: d = ll; break;
                    case short ss: d = ss; break;
                    case byte bb: d = bb; break;
                    case string s when double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var pd):
                        d = pd; break;
                    case IConvertible conv:
                        d = conv.ToDouble(System.Globalization.CultureInfo.InvariantCulture); break;
                    default:
                        return int.MaxValue;
                }
                if (double.IsNaN(d) || d <= 0) return 0;
                if (d > int.MaxValue) return int.MaxValue;
                // Truncate toward zero
                d = d >= 0 ? global::System.Math.Floor(d) : global::System.Math.Ceiling(d);
                return (int)d;
            }
            catch { return int.MaxValue; }
        }
    }
}
