using System;
using System.Text.RegularExpressions;

namespace JavaScriptRuntime
{
    [IntrinsicObject("RegExp", IntrinsicCallKind.ConstructorLike)]
    public sealed class RegExp
    {
        private readonly Regex _regex;
        private readonly bool _global;
        private readonly bool _sticky;
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
            _sticky = false;
            lastIndex = 0;
        }

        public RegExp(object? pattern)
        {
            _source = DotNet2JSConversions.ToString(pattern);
            _regex = new Regex(_source);
            _global = false;
            _sticky = false;
            lastIndex = 0;
        }

        public RegExp(object? pattern, object? flags)
        {
            _source = DotNet2JSConversions.ToString(pattern);
            var f = DotNet2JSConversions.ToString(flags) ?? string.Empty;
            var opts = RegexOptions.None;
            if (f.IndexOf('i') >= 0) opts |= RegexOptions.IgnoreCase;
            if (f.IndexOf('m') >= 0) opts |= RegexOptions.Multiline;
            // 'g' (global) enables lastIndex tracking (affects exec() and test()).
            _global = f.IndexOf('g') >= 0;
            _sticky = f.IndexOf('y') >= 0;
            _regex = new Regex(_source, opts);
            lastIndex = 0;
        }

        internal Regex Regex => _regex;
        internal bool Global => _global;
        internal bool Sticky => _sticky;
        private bool UsesLastIndexSemantics => _global || _sticky;

        // ECMAScript RegExp instance properties (subset).
        public string source => _source;
        public bool global => _global;
        public bool ignoreCase => (_regex.Options & RegexOptions.IgnoreCase) != 0;
        public bool multiline => (_regex.Options & RegexOptions.Multiline) != 0;
        
        // Additional flag properties (currently unsupported flags return false)
        public bool dotAll => false;  // 's' flag - not supported yet
        public bool sticky => _sticky;
        public bool unicode => false; // 'u' flag - not supported yet
        public bool unicodeSets => false; // 'v' flag - not supported yet
        public bool hasIndices => false; // 'd' flag - not supported yet
        
        // Return flags in canonical order: "dgimsuvy"
        public string flags
        {
            get
            {
                var result = string.Empty;
                // Alphabetical order as per spec
                if (hasIndices) result += "d";
                if (global) result += "g";
                if (ignoreCase) result += "i";
                if (multiline) result += "m";
                if (dotAll) result += "s";
                if (unicode) result += "u";
                if (unicodeSets) result += "v";
                if (sticky) result += "y";
                return result;
            }
        }

        public object test(object? input)
        {
            var s = DotNet2JSConversions.ToString(input) ?? string.Empty;

            if (!TryMatch(s, out var match))
            {
                return false;
            }

            UpdateLastIndexAfterSuccess(match);
            return true;
        }

        // Minimal RegExp.prototype.exec(string)
        // Returns: Array of matches (group 0..n) or null.
        public object exec(object? input)
        {
            var s = DotNet2JSConversions.ToString(input) ?? string.Empty;

            if (!TryMatch(s, out var match))
            {
                return JsNull.Null;
            }

            UpdateLastIndexAfterSuccess(match);

            var result = new JavaScriptRuntime.Array(match.Groups.Count);
            for (int i = 0; i < match.Groups.Count; i++)
            {
                var g = match.Groups[i];
                result.Add(g.Success ? g.Value : null);
            }

            // JS exec result carries .index and .input.
            JavaScriptRuntime.ObjectRuntime.SetProperty(result, "index", (double)match.Index);
            JavaScriptRuntime.ObjectRuntime.SetProperty(result, "input", s);
            return result;
        }
        
        // RegExp.prototype.toString() returns "/source/flags" format
        public string toString()
        {
            return "/" + _source + "/" + flags;
        }

        private bool TryMatch(string input, out Match match)
        {
            var startAt = 0;
            if (!TryGetMatchStart(input, out startAt))
            {
                match = Match.Empty;
                return false;
            }

            match = _regex.Match(input, startAt);
            if (!match.Success || (_sticky && match.Index != startAt))
            {
                if (UsesLastIndexSemantics)
                {
                    lastIndex = 0;
                }

                match = Match.Empty;
                return false;
            }

            return true;
        }

        private bool TryGetMatchStart(string input, out int startAt)
        {
            startAt = 0;
            if (!UsesLastIndexSemantics)
            {
                return true;
            }

            if (double.IsNaN(lastIndex) || double.IsNegativeInfinity(lastIndex) || lastIndex < 0)
            {
                return true;
            }

            if (double.IsPositiveInfinity(lastIndex))
            {
                lastIndex = 0;
                return false;
            }

            var truncated = System.Math.Truncate(lastIndex);
            if (truncated > input.Length)
            {
                lastIndex = 0;
                return false;
            }

            startAt = (int)truncated;
            return true;
        }

        private void UpdateLastIndexAfterSuccess(Match match)
        {
            if (!UsesLastIndexSemantics)
            {
                return;
            }

            // Keep the existing exec()/test() behavior that advances past empty matches
            // so user loops do not get stuck on zero-length results.
            int nextIndex = match.Index + match.Length;
            if (match.Length == 0)
            {
                nextIndex = match.Index + 1;
            }

            lastIndex = nextIndex;
        }
    }
}
