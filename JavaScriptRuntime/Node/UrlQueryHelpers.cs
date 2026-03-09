using System;
using System.Collections.Generic;
using System.Text;

namespace JavaScriptRuntime.Node
{
    internal static class UrlQueryHelpers
    {
        public static List<KeyValuePair<string, string>> ParseEntries(
            object? value,
            string separator = "&",
            string assignment = "=",
            bool plusAsSpace = true)
        {
            return ParseEntriesFromString(CoerceString(value), separator, assignment, plusAsSpace);
        }

        public static List<KeyValuePair<string, string>> ParseEntriesFromString(
            string? input,
            string separator = "&",
            string assignment = "=",
            bool plusAsSpace = true)
        {
            var text = input ?? string.Empty;
            if (text.StartsWith("?", StringComparison.Ordinal))
            {
                text = text.Substring(1);
            }

            var entries = new List<KeyValuePair<string, string>>();
            if (text.Length == 0)
            {
                return entries;
            }

            separator = string.IsNullOrEmpty(separator) ? "&" : separator;
            assignment = string.IsNullOrEmpty(assignment) ? "=" : assignment;

            var segments = text.Split(new[] { separator }, StringSplitOptions.None);
            foreach (var segment in segments)
            {
                if (segment.Length == 0)
                {
                    continue;
                }

                var assignmentIndex = segment.IndexOf(assignment, StringComparison.Ordinal);
                string rawKey;
                string rawValue;

                if (assignmentIndex >= 0)
                {
                    rawKey = segment.Substring(0, assignmentIndex);
                    rawValue = segment.Substring(assignmentIndex + assignment.Length);
                }
                else
                {
                    rawKey = segment;
                    rawValue = string.Empty;
                }

                entries.Add(new KeyValuePair<string, string>(
                    DecodeComponent(rawKey, plusAsSpace),
                    DecodeComponent(rawValue, plusAsSpace)));
            }

            return entries;
        }

        public static string SerializeEntries(
            IEnumerable<KeyValuePair<string, string>> entries,
            string separator = "&",
            string assignment = "=",
            bool spaceAsPlus = false)
        {
            separator = string.IsNullOrEmpty(separator) ? "&" : separator;
            assignment = string.IsNullOrEmpty(assignment) ? "=" : assignment;

            var sb = new StringBuilder();
            var first = true;
            foreach (var entry in entries)
            {
                if (!first)
                {
                    sb.Append(separator);
                }

                first = false;
                sb.Append(EncodeComponent(entry.Key, spaceAsPlus));
                sb.Append(assignment);
                sb.Append(EncodeComponent(entry.Value, spaceAsPlus));
            }

            return sb.ToString();
        }

        public static string EncodeComponent(string value, bool spaceAsPlus)
        {
            var encoded = Uri.EscapeDataString(value ?? string.Empty);
            return spaceAsPlus
                ? encoded.Replace("%20", "+", StringComparison.Ordinal)
                : encoded;
        }

        public static string DecodeComponent(string value, bool plusAsSpace)
        {
            var normalized = value ?? string.Empty;
            if (plusAsSpace)
            {
                normalized = normalized.Replace("+", " ", StringComparison.Ordinal);
            }

            return Uri.UnescapeDataString(normalized);
        }

        public static string CoerceString(object? value)
        {
            return value == null || value is JsNull
                ? string.Empty
                : DotNet2JSConversions.ToString(value);
        }
    }
}
