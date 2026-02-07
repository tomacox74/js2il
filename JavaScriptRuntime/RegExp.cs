using System;
using System.Text.RegularExpressions;

namespace JavaScriptRuntime
{
    [IntrinsicObject("RegExp", IntrinsicCallKind.ConstructorLike)]
    public sealed class RegExp
    {
        private readonly Regex _regex;
        private readonly bool _global;
        private readonly string _source;

        // Minimal support for RegExp.prototype.lastIndex (used heavily by parsers).
        // In JS this participates in exec() when /g or /y are set. We currently only
        // model /g behavior.
        public double lastIndex { get; set; }

        public RegExp()
        {
            _source = string.Empty;
            _regex = new Regex(_source);
            _global = false;
            lastIndex = 0;
        }

        public RegExp(object? pattern)
        {
            _source = DotNet2JSConversions.ToString(pattern);
            _regex = new Regex(_source);
            _global = false;
            lastIndex = 0;
        }

        public RegExp(object? pattern, object? flags)
        {
            _source = DotNet2JSConversions.ToString(pattern);
            var f = DotNet2JSConversions.ToString(flags) ?? string.Empty;
            var opts = RegexOptions.None;
            if (f.IndexOf('i') >= 0) opts |= RegexOptions.IgnoreCase;
            if (f.IndexOf('m') >= 0) opts |= RegexOptions.Multiline;
            // 'g' (global) does not affect test(); keep for completeness but unused here
            _global = f.IndexOf('g') >= 0;
            _regex = new Regex(_source, opts);
            lastIndex = 0;
        }

        internal Regex Regex => _regex;
        internal bool Global => _global;

        // ECMAScript RegExp instance properties (subset).
        public string source => _source;
        public bool global => _global;
        public bool ignoreCase => (_regex.Options & RegexOptions.IgnoreCase) != 0;
        public bool multiline => (_regex.Options & RegexOptions.Multiline) != 0;

        public object test(object? input)
        {
            var s = DotNet2JSConversions.ToString(input);
            return _regex.IsMatch(s);
        }

        // Minimal RegExp.prototype.exec(string)
        // Returns: Array of matches (group 0..n) or null.
        public object exec(object? input)
        {
            var s = DotNet2JSConversions.ToString(input);

            int startAt = 0;
            if (_global)
            {
                // Clamp/normalize lastIndex to a valid starting offset.
                if (double.IsNaN(lastIndex) || double.IsInfinity(lastIndex) || lastIndex < 0)
                {
                    startAt = 0;
                }
                else
                {
                    startAt = (int)lastIndex;
                    if (startAt > s.Length) startAt = s.Length;
                }
            }

            var match = _regex.Match(s, startAt);
            if (!match.Success)
            {
                if (_global)
                {
                    lastIndex = 0;
                }

                return JsNull.Null;
            }

            if (_global)
            {
                lastIndex = match.Index + match.Length;
            }

            var result = new JavaScriptRuntime.Array(match.Groups.Count);
            for (int i = 0; i < match.Groups.Count; i++)
            {
                var g = match.Groups[i];
                result.Add(g.Success ? g.Value : null);
            }

            // JS exec result carries .index and .input.
            JavaScriptRuntime.Object.SetProperty(result, "index", (double)match.Index);
            JavaScriptRuntime.Object.SetProperty(result, "input", s);
            return result;
        }
    }
}
