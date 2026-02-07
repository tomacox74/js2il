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

        public RegExp()
        {
            _source = string.Empty;
            _regex = new Regex(_source);
            _global = false;
        }

        public RegExp(object? pattern)
        {
            _source = DotNet2JSConversions.ToString(pattern);
            _regex = new Regex(_source);
            _global = false;
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
    }
}
