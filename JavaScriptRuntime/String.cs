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
    public static double LocaleCompare(string input, string other, object? locales, object? options)
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
    }
}
