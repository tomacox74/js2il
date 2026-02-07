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
        /// Implements String.prototype.charCodeAt([index]).
        /// Returns a UTF-16 code unit as a number, or NaN when out of range.
        /// </summary>
        public static object CharCodeAt(string input)
        {
            return CharCodeAt(input, null);
        }

        /// <summary>
        /// Implements String.prototype.charCodeAt(index).
        /// </summary>
        public static object CharCodeAt(string input, object? index)
        {
            input ??= string.Empty;

            // JS default index is 0 when omitted/undefined/null.
            var idx = index == null ? 0d : JavaScriptRuntime.TypeUtilities.ToNumber(index);

            if (double.IsNaN(idx) || double.IsInfinity(idx))
            {
                return double.NaN;
            }

            // ToIntegerOrInfinity truncates toward zero.
            idx = global::System.Math.Truncate(idx);

            if (idx < 0 || idx >= input.Length)
            {
                return double.NaN;
            }

            return (double)input[(int)idx];
        }

        public static string ToLowerCase(string input)
        {
            input ??= string.Empty;
            return input.ToLowerInvariant();
        }

        public static string ToUpperCase(string input)
        {
            input ??= string.Empty;
            return input.ToUpperInvariant();
        }

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

            // If the pattern is a RegExp object, use its compiled Regex (and global flag)
            // rather than falling back to ToString() which would treat it as a plain string.
            if (patternOrString is RegExp regExp)
            {
                // Replacement may be a string or a callback delegate.
                if (replacement is Func<object[], object, object> f1 || replacement is Func<object[], object> f0)
                {
                    string Invoke(string match)
                    {
                        if (replacement is Func<object[], object, object> cb1)
                        {
                            var r = cb1(System.Array.Empty<object>(), match);
                            return DotNet2JSConversions.ToString(r);
                        }

                        var cb0 = (Func<object[], object>)replacement;
                        var r0 = cb0(System.Array.Empty<object>());
                        return DotNet2JSConversions.ToString(r0);
                    }

                    var evaluator = new MatchEvaluator(m => Invoke(m.Value));
                    if (regExp.Global)
                    {
                        return regExp.Regex.Replace(input, evaluator);
                    }

                    // Replace only the first match
                    return regExp.Regex.Replace(input, evaluator, 1, 0);
                }

                var replacementText = DotNet2JSConversions.ToString(replacement) ?? string.Empty;
                if (regExp.Global)
                {
                    return regExp.Regex.Replace(input, replacementText);
                }

                // Replace only the first match
                return regExp.Regex.Replace(input, replacementText, 1);
            }

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
        /// Implements a subset of String.prototype.endsWith(searchString[, length]).
        /// If length is provided, the string is treated as if it were truncated to that length.
        /// Uses ordinal comparison.
        /// </summary>
        public static object EndsWith(string input, string searchString)
        {
            return EndsWith(input, searchString, null);
        }

        public static object EndsWith(string input, string searchString, object? length)
        {
            input ??= string.Empty;
            searchString ??= string.Empty;

            int len = input.Length;
            if (length != null)
            {
                try
                {
                    double d;
                    if (length is double dd) d = dd;
                    else if (length is float ff) d = ff;
                    else if (length is int ii) d = ii;
                    else if (length is long ll) d = ll;
                    else if (length is string s && double.TryParse(s, out var parsed)) d = parsed;
                    else if (length is IConvertible conv) d = conv.ToDouble(System.Globalization.CultureInfo.InvariantCulture);
                    else d = input.Length;

                    if (double.IsNaN(d)) d = input.Length;
                    if (double.IsNegativeInfinity(d)) d = 0d;
                    if (double.IsPositiveInfinity(d)) d = input.Length;
                    d = d >= 0 ? global::System.Math.Floor(d) : global::System.Math.Ceiling(d);
                    if (d < 0) d = 0;
                    if (d > input.Length) d = input.Length;
                    len = (int)d;
                }
                catch { len = input.Length; }
            }

            if (searchString.Length == 0) return true;
            if (len < searchString.Length) return false;
            return input.AsSpan(0, len).EndsWith(searchString.AsSpan(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Implements a subset of String.prototype.includes(searchString[, position]).
        /// Uses ordinal comparison.
        /// </summary>
        public static object Includes(string input, string searchString)
        {
            return Includes(input, searchString, null);
        }

        public static object Includes(string input, string searchString, object? position)
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
                    if (double.IsNegativeInfinity(d)) d = 0d;
                    if (double.IsPositiveInfinity(d)) d = input.Length;
                    d = d >= 0 ? global::System.Math.Floor(d) : global::System.Math.Ceiling(d);
                    if (d < 0) d = 0;
                    if (d > input.Length) d = input.Length;
                    pos = (int)d;
                }
                catch { pos = 0; }
            }
            if (searchString.Length == 0) return true;
            if (pos < 0 || pos > input.Length) return false;
            return input.IndexOf(searchString, pos, StringComparison.Ordinal) >= 0;
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

            Regex re;
            try
            {
                re = new Regex(pattern, options);
            }
            catch (RegexParseException)
            {
                // Pattern is not a valid regex, treat as literal string (fallback to simple replace)
                // This handles cases like string.replace('\\', '/') where the pattern is a plain string
                var escapedPattern = Regex.Escape(pattern);
                re = new Regex(escapedPattern, options);
            }
            
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
        /// Replace with a callback function: replacement can be a bound delegate. We support common delegate
        /// shapes produced by the compiler (Func<object[], object> and Func<object[], object, object>). The callback
        /// receives the matched substring; its return value is coerced to string.
        /// </summary>
        public static string Replace(string input, string pattern, object replacementCallback, bool global, bool ignoreCase)
        {
            if (input == null) return string.Empty;
            var options = RegexOptions.CultureInvariant;
            if (ignoreCase) options |= RegexOptions.IgnoreCase;
            var re = new Regex(pattern, options);

            string Invoke(object? cb, string match)
            {
                if (cb is Func<object[], object, object> f1)
                {
                    var r = f1(System.Array.Empty<object>(), match);
                    return DotNet2JSConversions.ToString(r);
                }
                if (cb is Func<object[], object> f0)
                {
                    var r = f0(System.Array.Empty<object>());
                    return DotNet2JSConversions.ToString(r);
                }
                // Fallback: ToString on callback object (unlikely useful)
                return DotNet2JSConversions.ToString(cb);
            }

            var evaluator = new MatchEvaluator(m => Invoke(replacementCallback, m.Value));
            if (global)
            {
                return re.Replace(input, evaluator);
            }
            else
            {
                // Replace the first match only
                int count = 0;
                return re.Replace(input, m =>
                {
                    if (count++ == 0) return Invoke(replacementCallback, m.Value);
                    return m.Value;
                });
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
        /// Split using a string separator with optional ignoreCase and limit.
        /// Note: When called with a plain string (not a regex literal), the pattern
        /// should be treated as a literal string, not a regex pattern.
        /// </summary>
        public static object Split(string input, string pattern, bool ignoreCase, object? limit)
        {
            input ??= string.Empty;
            int maxCount = ToSplitLimit(limit);
            if (maxCount == 0) return new JavaScriptRuntime.Array();
            
            // Escape the pattern to treat it as a literal string, not a regex
            var escapedPattern = Regex.Escape(pattern);
            var options = RegexOptions.CultureInvariant;
            if (ignoreCase) options |= RegexOptions.IgnoreCase;
            var rx = new Regex(escapedPattern, options);
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
