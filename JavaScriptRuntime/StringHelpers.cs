using System;
using System.Text.RegularExpressions;

namespace JavaScriptRuntime
{
    /// <summary>
    /// Helpers for JavaScript String.prototype behaviors used by the IL generator.
    /// Focused, minimal surface to support common patterns during compilation.
    /// </summary>
    public static class StringHelpers
    {
        /// <summary>
        /// Implements a subset of String.prototype.replace when the pattern is a regular expression literal
        /// and the replacement is a string. Supports global and ignoreCase flags.
        /// </summary>
        public static string ReplaceRegex(string input, string pattern, string replacement, bool global, bool ignoreCase)
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
    }
}
