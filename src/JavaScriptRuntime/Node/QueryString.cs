using System;
using System.Collections.Generic;
using System.Dynamic;

using JsArray = JavaScriptRuntime.Array;

namespace JavaScriptRuntime.Node
{
    [NodeModule("querystring")]
    public sealed class QueryString
    {
        public object parse(object value)
            => parse(value, null, null, null);

        public object parse(object value, object? separator)
            => parse(value, separator, null, null);

        public object parse(object value, object? separator, object? assignment)
            => parse(value, separator, assignment, null);

        public object parse(object value, object? separator, object? assignment, object? options)
        {
            _ = options;

            var separatorText = NormalizeToken(separator, "&");
            var assignmentText = NormalizeToken(assignment, "=");
            var entries = UrlQueryHelpers.ParseEntries(value, separatorText, assignmentText, plusAsSpace: true);

            dynamic result = new ExpandoObject();
            var dict = (IDictionary<string, object?>)result;

            foreach (var entry in entries)
            {
                if (!dict.TryGetValue(entry.Key, out var existing))
                {
                    dict[entry.Key] = entry.Value;
                    continue;
                }

                if (existing is JsArray values)
                {
                    values.Add(entry.Value);
                    continue;
                }

                dict[entry.Key] = new JsArray(new object?[] { existing, entry.Value });
            }

            return result;
        }

        public string stringify(object value)
            => stringify(value, null, null, null);

        public string stringify(object value, object? separator)
            => stringify(value, separator, null, null);

        public string stringify(object value, object? separator, object? assignment)
            => stringify(value, separator, assignment, null);

        public string stringify(object value, object? separator, object? assignment, object? options)
        {
            _ = options;

            var separatorText = NormalizeToken(separator, "&");
            var assignmentText = NormalizeToken(assignment, "=");
            var entries = new List<KeyValuePair<string, string>>();

            if (value is IDictionary<string, object?> dict)
            {
                foreach (var pair in dict)
                {
                    AppendEntries(entries, pair.Key, pair.Value);
                }
            }

            return UrlQueryHelpers.SerializeEntries(entries, separatorText, assignmentText, spaceAsPlus: false);
        }

        private static void AppendEntries(List<KeyValuePair<string, string>> entries, string key, object? value)
        {
            if (value is JsArray values)
            {
                foreach (var item in values)
                {
                    entries.Add(new KeyValuePair<string, string>(
                        key,
                        UrlQueryHelpers.CoerceString(item)));
                }

                return;
            }

            entries.Add(new KeyValuePair<string, string>(
                key,
                UrlQueryHelpers.CoerceString(value)));
        }

        private static string NormalizeToken(object? value, string fallback)
        {
            var text = UrlQueryHelpers.CoerceString(value);
            return text.Length == 0 ? fallback : text;
        }
    }
}
