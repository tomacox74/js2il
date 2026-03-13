using System;
using System.Collections;
using System.Collections.Generic;
using JsArray = JavaScriptRuntime.Array;

namespace JavaScriptRuntime.Node
{
    [NodeModule("url")]
    public sealed class Url
    {
        public Type URL => typeof(URL);

        public Type URLSearchParams => typeof(URLSearchParams);

        public string fileURLToPath(object input)
        {
            Uri uri;
            try
            {
                uri = input switch
                {
                    URL url => new Uri(url.href, UriKind.Absolute),
                    _ => new Uri(UrlQueryHelpers.CoerceString(input), UriKind.Absolute),
                };
            }
            catch (UriFormatException)
            {
                throw new TypeError("Invalid URL");
            }

            if (!uri.IsFile)
            {
                throw new TypeError("The URL must be of scheme file");
            }

            return uri.LocalPath;
        }

        public URL pathToFileURL(object path)
        {
            var fullPath = System.IO.Path.GetFullPath(UrlQueryHelpers.CoerceString(path));
            var builder = new UriBuilder
            {
                Scheme = Uri.UriSchemeFile,
                Host = string.Empty,
                Path = fullPath,
            };
            return new URL(builder.Uri.AbsoluteUri);
        }
    }

    public sealed class URL
    {
        private readonly URLSearchParams _searchParams;

        private string _protocol;
        private string _username;
        private string _password;
        private string _hostname;
        private string _port;
        private string _pathname;
        private string _hash;

        public URL(object input)
            : this(input, null)
        {
        }

        public URL(object input, object? baseValue)
        {
            var uri = ResolveUri(input, baseValue);

            _protocol = uri.Scheme + ":";
            _hostname = uri.Host;
            _port = uri.IsDefaultPort ? string.Empty : uri.Port.ToString(System.Globalization.CultureInfo.InvariantCulture);
            _pathname = NormalizePath(uri.AbsolutePath);
            _hash = uri.Fragment ?? string.Empty;

            ParseUserInfo(uri.UserInfo, out _username, out _password);

            _searchParams = new URLSearchParams(uri.Query);
        }

        public string href => BuildHref();

        public string origin => string.Equals(_protocol, "file:", StringComparison.Ordinal)
            ? "null"
            : $"{_protocol}//{host}";

        public string protocol => _protocol;

        public string username => _username;

        public string password => _password;

        public string host => string.IsNullOrEmpty(_port) ? _hostname : $"{_hostname}:{_port}";

        public string hostname => _hostname;

        public string port => _port;

        public string pathname => _pathname;

        public string search
        {
            get
            {
                var serialized = _searchParams.toString();
                return serialized.Length == 0 ? string.Empty : "?" + serialized;
            }
            set
            {
                _searchParams.ReplaceFromQuery(value);
            }
        }

        public string hash
        {
            get => _hash;
            set => _hash = NormalizeHash(value);
        }

        public URLSearchParams searchParams => _searchParams;

        public string toString() => href;

        public string toJSON() => href;

        public override string ToString() => href;

        private string BuildHref()
        {
            return $"{_protocol}//{BuildAuthority()}{_pathname}{search}{_hash}";
        }

        private string BuildAuthority()
        {
            var credentials = string.Empty;
            if (_username.Length > 0)
            {
                credentials = UrlQueryHelpers.EncodeComponent(_username, spaceAsPlus: false);
                if (_password.Length > 0)
                {
                    credentials += ":" + UrlQueryHelpers.EncodeComponent(_password, spaceAsPlus: false);
                }

                credentials += "@";
            }

            return credentials + host;
        }

        private static Uri ResolveUri(object input, object? baseValue)
        {
            var inputText = UrlQueryHelpers.CoerceString(input);
            if (string.IsNullOrWhiteSpace(inputText))
            {
                throw new TypeError("Invalid URL");
            }

            if (baseValue == null || baseValue is JsNull)
            {
                if (!Uri.TryCreate(inputText, UriKind.Absolute, out var absoluteUri))
                {
                    throw new TypeError("Invalid URL");
                }

                return absoluteUri;
            }

            var baseText = UrlQueryHelpers.CoerceString(baseValue);
            if (!Uri.TryCreate(baseText, UriKind.Absolute, out var baseUri))
            {
                throw new TypeError("Invalid base URL");
            }

            try
            {
                return new Uri(baseUri, inputText);
            }
            catch (UriFormatException)
            {
                throw new TypeError("Invalid URL");
            }
        }

        private static void ParseUserInfo(string userInfo, out string username, out string password)
        {
            username = string.Empty;
            password = string.Empty;

            if (string.IsNullOrEmpty(userInfo))
            {
                return;
            }

            var separatorIndex = userInfo.IndexOf(':');
            if (separatorIndex < 0)
            {
                username = Uri.UnescapeDataString(userInfo);
                return;
            }

            username = Uri.UnescapeDataString(userInfo.Substring(0, separatorIndex));
            password = Uri.UnescapeDataString(userInfo.Substring(separatorIndex + 1));
        }

        private static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "/";
            }

            return path;
        }

        private static string NormalizeHash(string? value)
        {
            var text = value ?? string.Empty;
            if (text.Length == 0)
            {
                return string.Empty;
            }

            return text.StartsWith("#", StringComparison.Ordinal) ? text : "#" + text;
        }
    }

    public sealed class URLSearchParams
    {
        private readonly List<KeyValuePair<string, string>> _entries;

        public URLSearchParams()
        {
            _entries = new List<KeyValuePair<string, string>>();
        }

        public URLSearchParams(object? init)
        {
            _entries = InitializeEntries(init);
        }

        public double size => _entries.Count;

        public object get(object name)
        {
            var key = UrlQueryHelpers.CoerceString(name);
            foreach (var entry in _entries)
            {
                if (string.Equals(entry.Key, key, StringComparison.Ordinal))
                {
                    return entry.Value;
                }
            }

            return JsNull.Null;
        }

        public JsArray getAll(object name)
        {
            var key = UrlQueryHelpers.CoerceString(name);
            var values = new JsArray();

            foreach (var entry in _entries)
            {
                if (string.Equals(entry.Key, key, StringComparison.Ordinal))
                {
                    values.Add(entry.Value);
                }
            }

            return values;
        }

        public bool has(object name)
        {
            var key = UrlQueryHelpers.CoerceString(name);
            foreach (var entry in _entries)
            {
                if (string.Equals(entry.Key, key, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public void append(object name, object value)
        {
            _entries.Add(new KeyValuePair<string, string>(
                UrlQueryHelpers.CoerceString(name),
                UrlQueryHelpers.CoerceString(value)));
        }

        public void set(object name, object value)
        {
            var key = UrlQueryHelpers.CoerceString(name);
            var normalizedValue = UrlQueryHelpers.CoerceString(value);
            var replaced = false;

            for (int i = 0; i < _entries.Count; i++)
            {
                if (!string.Equals(_entries[i].Key, key, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!replaced)
                {
                    _entries[i] = new KeyValuePair<string, string>(key, normalizedValue);
                    replaced = true;
                }
                else
                {
                    _entries.RemoveAt(i);
                    i--;
                }
            }

            if (!replaced)
            {
                _entries.Add(new KeyValuePair<string, string>(key, normalizedValue));
            }
        }

        public void @delete(object name)
        {
            var key = UrlQueryHelpers.CoerceString(name);

            for (int i = 0; i < _entries.Count; i++)
            {
                if (string.Equals(_entries[i].Key, key, StringComparison.Ordinal))
                {
                    _entries.RemoveAt(i);
                    i--;
                }
            }
        }

        public void sort()
        {
            _entries.Sort(static (left, right) => string.Compare(left.Key, right.Key, StringComparison.Ordinal));
        }

        public IJavaScriptIterator entries()
            => new SearchParamsIterator(_entries, SearchParamsIteratorKind.Entries);

        public IJavaScriptIterator keys()
            => new SearchParamsIterator(_entries, SearchParamsIteratorKind.Keys);

        public IJavaScriptIterator values()
            => new SearchParamsIterator(_entries, SearchParamsIteratorKind.Values);

        public object? forEach(object callback)
        {
            return forEach(callback, null);
        }

        public object? forEach(object callback, object? thisArg)
        {
            if (callback is not Delegate del)
            {
                throw new TypeError("URLSearchParams.forEach callback must be callable");
            }

            var previousThis = RuntimeServices.SetCurrentThis(thisArg);
            try
            {
                for (int i = 0; i < _entries.Count; i++)
                {
                    var entry = _entries[i];
                    Closure.InvokeWithArgs(del, System.Array.Empty<object>(), entry.Value, entry.Key, this);
                }
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }

            return null;
        }

        public string toString()
            => UrlQueryHelpers.SerializeEntries(_entries, spaceAsPlus: true);

        public override string ToString() => toString();

        internal void ReplaceFromQuery(object? query)
        {
            _entries.Clear();
            _entries.AddRange(UrlQueryHelpers.ParseEntries(query, plusAsSpace: true));
        }

        private static List<KeyValuePair<string, string>> InitializeEntries(object? init)
        {
            if (init == null || init is JsNull)
            {
                return new List<KeyValuePair<string, string>>();
            }

            if (init is URLSearchParams other)
            {
                return new List<KeyValuePair<string, string>>(other._entries);
            }

            if (init is string)
            {
                return UrlQueryHelpers.ParseEntries(init, plusAsSpace: true);
            }

            if (TryReadPairEntries(init, out var pairEntries))
            {
                return pairEntries;
            }

            if (init is IDictionary<string, object?> dict)
            {
                var entries = new List<KeyValuePair<string, string>>(dict.Count);
                foreach (var pair in dict)
                {
                    entries.Add(new KeyValuePair<string, string>(
                        pair.Key,
                        UrlQueryHelpers.CoerceString(pair.Value)));
                }

                return entries;
            }

            return UrlQueryHelpers.ParseEntries(init, plusAsSpace: true);
        }

        private static bool TryReadPairEntries(object init, out List<KeyValuePair<string, string>> entries)
        {
            entries = new List<KeyValuePair<string, string>>();

            if (init is not IEnumerable enumerable || init is string)
            {
                return false;
            }

            foreach (var item in enumerable)
            {
                if (TryReadPair(item, out var pair))
                {
                    entries.Add(pair);
                    continue;
                }

                entries.Clear();
                return false;
            }

            return entries.Count > 0;
        }

        private static bool TryReadPair(object? item, out KeyValuePair<string, string> pair)
        {
            switch (item)
            {
                case JsArray jsArray when jsArray.Count >= 2:
                    pair = new KeyValuePair<string, string>(
                        UrlQueryHelpers.CoerceString(jsArray[0]),
                        UrlQueryHelpers.CoerceString(jsArray[1]));
                    return true;

                case object?[] array when array.Length >= 2:
                    pair = new KeyValuePair<string, string>(
                        UrlQueryHelpers.CoerceString(array[0]),
                        UrlQueryHelpers.CoerceString(array[1]));
                    return true;

                default:
                    pair = default;
                    return false;
            }
        }

        private enum SearchParamsIteratorKind
        {
            Keys,
            Values,
            Entries,
        }

        private sealed class SearchParamsIterator : IJavaScriptIterator
        {
            private readonly IReadOnlyList<KeyValuePair<string, string>> _entries;
            private readonly SearchParamsIteratorKind _kind;
            private int _index;
            private bool _closed;

            public SearchParamsIterator(IReadOnlyList<KeyValuePair<string, string>> entries, SearchParamsIteratorKind kind)
            {
                _entries = entries;
                _kind = kind;
            }

            public bool HasReturn => true;

            public IteratorResultObject Next()
            {
                if (_closed || _index >= _entries.Count)
                {
                    return new IteratorResultObject(null, done: true);
                }

                var entry = _entries[_index++];
                object value = _kind switch
                {
                    SearchParamsIteratorKind.Keys => entry.Key,
                    SearchParamsIteratorKind.Values => entry.Value,
                    SearchParamsIteratorKind.Entries => new JsArray(new object?[] { entry.Key, entry.Value }),
                    _ => JsNull.Null,
                };

                return new IteratorResultObject(value, done: false);
            }

            public object next(object? value = null)
                => Next();

            public void Return()
            {
                _closed = true;
            }
        }
    }
}
