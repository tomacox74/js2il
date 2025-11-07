using System;
using System.Text.RegularExpressions;

namespace JavaScriptRuntime
{
    [IntrinsicObject("RegExp")]
    public sealed class RegExp
    {
        private readonly Regex _regex;

        public RegExp()
        {
            _regex = new Regex(string.Empty);
        }

        public RegExp(object? pattern)
        {
            var pat = DotNet2JSConversions.ToString(pattern);
            _regex = new Regex(pat);
        }

        public RegExp(object? pattern, object? flags)
        {
            var pat = DotNet2JSConversions.ToString(pattern);
            var f = DotNet2JSConversions.ToString(flags) ?? string.Empty;
            var opts = RegexOptions.None;
            if (f.IndexOf('i') >= 0) opts |= RegexOptions.IgnoreCase;
            if (f.IndexOf('m') >= 0) opts |= RegexOptions.Multiline;
            // 'g' (global) does not affect test(); keep for completeness but unused here
            _regex = new Regex(pat, opts);
        }

        public object test(object? input)
        {
            var s = DotNet2JSConversions.ToString(input);
            return _regex.IsMatch(s);
        }
    }
}
