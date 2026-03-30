using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace JavaScriptRuntime
{
    [IntrinsicObject("RegExp", IntrinsicCallKind.ConstructorLike)]
    public sealed class RegExp
    {
        [Flags]
        private enum WellKnownSymbolFastPathFlags
        {
            None = 0,
            Match = 1,
            Replace = 2,
            Search = 4,
            Split = 8,
            All = Match | Replace | Search | Split
        }

        private const string DotPattern = "(?:[^\n\r\u2028\u2029])";
        private const string UnicodeDotPattern = "(?:[\uD800-\uDBFF][\uDC00-\uDFFF]|[^\n\r\u2028\u2029])";
        private const string UnicodeDotAllPattern = "(?:[\uD800-\uDBFF][\uDC00-\uDFFF]|[\\s\\S])";
        private static readonly string MatchSymbolPropertyKey = Symbol.match.DebugId;
        private static readonly string ReplaceSymbolPropertyKey = Symbol.replace.DebugId;
        private static readonly string SearchSymbolPropertyKey = Symbol.search.DebugId;
        private static readonly string SplitSymbolPropertyKey = Symbol.split.DebugId;
        private static readonly Func<object?, object> MatchSymbolDelegate = MatchSymbolMethod;
        private static readonly Func<object?, object?, object> ReplaceSymbolDelegate = ReplaceSymbolMethod;
        private static readonly Func<object?, object> SearchSymbolDelegate = SearchSymbolMethod;
        private static readonly Func<object?, object?, object> SplitSymbolDelegate = SplitSymbolMethod;
        internal static readonly ExpandoObject Prototype = CreatePrototype();
        private static WellKnownSymbolFastPathFlags _prototypeWellKnownSymbolFastPathFlags = WellKnownSymbolFastPathFlags.All;
        private readonly Regex _regex;
        private readonly bool _global;
        private readonly bool _sticky;
        private readonly bool _dotAll;
        private readonly bool _unicode;
        private readonly bool _hasIndices;
        private readonly string _source;
        private readonly string? _simpleLiteralPattern;
        private WellKnownSymbolFastPathFlags _wellKnownSymbolFastPathFlags;

        // Minimal support for RegExp.prototype.lastIndex (used heavily by parsers).
        public double lastIndex { get; set; }

        public RegExp()
            : this(null, null)
        {
        }

        public RegExp(object? pattern)
            : this(pattern, null)
        {
        }

        public RegExp(object? pattern, object? flags)
        {
            _source = NormalizeSource(pattern);
            var parsedFlags = ParseFlags(NormalizeFlags(pattern, flags));

            _global = parsedFlags.Global;
            _sticky = parsedFlags.Sticky;
            _dotAll = parsedFlags.DotAll;
            _unicode = parsedFlags.Unicode;
            _hasIndices = parsedFlags.HasIndices;
            _simpleLiteralPattern = TryGetSimpleLiteralPattern(_source, parsedFlags);

            try
            {
                var preparedPattern = PreparePatternForDotNetRegex(_source, _unicode, _dotAll);
                _regex = new Regex(preparedPattern, parsedFlags.ToRegexOptions());
            }
            catch (RegexParseException ex)
            {
                throw new SyntaxError(ex.Message);
            }
            catch (ArgumentException ex)
            {
                throw new SyntaxError(ex.Message);
            }

            lastIndex = 0;
            _wellKnownSymbolFastPathFlags = WellKnownSymbolFastPathFlags.All;
            InitializeIntrinsicSurface();
        }

        internal Regex Regex => _regex;
        internal bool Global => _global;
        internal bool Sticky => _sticky;
        internal bool IsEmptySplitPattern => _source.Length == 0 || string.Equals(_source, "(?:)", StringComparison.Ordinal);
        internal string? SimpleLiteralPattern => _simpleLiteralPattern;
        private bool UsesLastIndexSemantics => _global || _sticky;

        public string source => _source;
        public bool global => _global;
        public bool ignoreCase => (_regex.Options & RegexOptions.IgnoreCase) != 0;
        public bool multiline => (_regex.Options & RegexOptions.Multiline) != 0;
        public bool dotAll => _dotAll;
        public bool sticky => _sticky;
        public bool unicode => _unicode;
        public bool unicodeSets => false;
        public bool hasIndices => _hasIndices;

        public string flags
        {
            get
            {
                var result = string.Empty;
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

            if (!UsesLastIndexSemantics)
            {
                if (_simpleLiteralPattern is string literalPattern)
                {
                    return s.IndexOf(literalPattern, StringComparison.Ordinal) >= 0;
                }

                return _regex.IsMatch(s);
            }

            if (!TryGetMatchBounds(s, out var matchIndex, out var matchLength))
            {
                return false;
            }

            UpdateLastIndexAfterSuccess(s, matchIndex, matchLength);
            return true;
        }

        public object exec(object? input)
        {
            var s = DotNet2JSConversions.ToString(input) ?? string.Empty;

            if (!TryMatch(s, out var match))
            {
                return JsNull.Null;
            }

            UpdateLastIndexAfterSuccess(s, match);

            var result = new JavaScriptRuntime.Array(match.Groups.Count);
            for (int i = 0; i < match.Groups.Count; i++)
            {
                var g = match.Groups[i];
                result.Add(g.Success ? g.Value : null);
            }

            JavaScriptRuntime.ObjectRuntime.SetProperty(result, "index", (double)match.Index);
            JavaScriptRuntime.ObjectRuntime.SetProperty(result, "input", s);
            if (_hasIndices)
            {
                JavaScriptRuntime.ObjectRuntime.SetProperty(result, "indices", CreateIndicesArray(match));
            }

            return result;
        }

        public object matchSymbol(object? input)
        {
            return JavaScriptRuntime.String.MatchWithRegExp(DotNet2JSConversions.ToString(input) ?? string.Empty, this);
        }

        public object replaceSymbol(object? input, object? replacement)
        {
            return JavaScriptRuntime.String.ReplaceWithRegExp(DotNet2JSConversions.ToString(input) ?? string.Empty, this, replacement);
        }

        public object searchSymbol(object? input)
        {
            return JavaScriptRuntime.String.SearchWithRegExp(DotNet2JSConversions.ToString(input) ?? string.Empty, this);
        }

        public object splitSymbol(object? input, object? limit)
        {
            return JavaScriptRuntime.String.SplitWithRegExp(DotNet2JSConversions.ToString(input) ?? string.Empty, this, limit);
        }

        private static RegExp GetCurrentThisRegExp(string wellKnownSymbolName)
        {
            if (RuntimeServices.GetCurrentThis() is not RegExp regExp)
            {
                throw new TypeError($"RegExp.prototype[@@{wellKnownSymbolName}] called on incompatible receiver");
            }

            return regExp;
        }

        private static object MatchSymbolMethod(object? input)
        {
            var regExp = GetCurrentThisRegExp("match");
            return JavaScriptRuntime.String.MatchWithRegExp(DotNet2JSConversions.ToString(input) ?? string.Empty, regExp);
        }

        private static object ReplaceSymbolMethod(object? input, object? replacement)
        {
            var regExp = GetCurrentThisRegExp("replace");
            return JavaScriptRuntime.String.ReplaceWithRegExp(DotNet2JSConversions.ToString(input) ?? string.Empty, regExp, replacement);
        }

        private static object SearchSymbolMethod(object? input)
        {
            var regExp = GetCurrentThisRegExp("search");
            return JavaScriptRuntime.String.SearchWithRegExp(DotNet2JSConversions.ToString(input) ?? string.Empty, regExp);
        }

        private static object SplitSymbolMethod(object? input, object? limit)
        {
            var regExp = GetCurrentThisRegExp("split");
            return JavaScriptRuntime.String.SplitWithRegExp(DotNet2JSConversions.ToString(input) ?? string.Empty, regExp, limit);
        }

        private static ExpandoObject CreatePrototype()
        {
            var prototype = new ExpandoObject();
            DefineSymbolMethod(prototype, MatchSymbolPropertyKey, MatchSymbolDelegate);
            DefineSymbolMethod(prototype, ReplaceSymbolPropertyKey, ReplaceSymbolDelegate);
            DefineSymbolMethod(prototype, SearchSymbolPropertyKey, SearchSymbolDelegate);
            DefineSymbolMethod(prototype, SplitSymbolPropertyKey, SplitSymbolDelegate);
            return prototype;
        }

        private bool HasIntrinsicWellKnownSymbolFastPath(WellKnownSymbolFastPathFlags flag)
        {
            return (_wellKnownSymbolFastPathFlags & flag) != 0
                && (_prototypeWellKnownSymbolFastPathFlags & flag) != 0;
        }

        internal bool CanUseEnumerateMatchesFastPath => !_sticky && !_unicode;

        internal bool TryInvokeIntrinsicWellKnownSymbol(Symbol symbol, string input, out object? result)
        {
            if (ReferenceEquals(symbol, Symbol.match) && HasIntrinsicWellKnownSymbolFastPath(WellKnownSymbolFastPathFlags.Match))
            {
                result = JavaScriptRuntime.String.MatchWithRegExp(input, this);
                return true;
            }

            if (ReferenceEquals(symbol, Symbol.search) && HasIntrinsicWellKnownSymbolFastPath(WellKnownSymbolFastPathFlags.Search))
            {
                result = JavaScriptRuntime.String.SearchWithRegExp(input, this);
                return true;
            }

            result = null;
            return false;
        }

        internal bool TryInvokeIntrinsicWellKnownSymbol(Symbol symbol, string input, object? arg1, out object? result)
        {
            if (ReferenceEquals(symbol, Symbol.replace) && HasIntrinsicWellKnownSymbolFastPath(WellKnownSymbolFastPathFlags.Replace))
            {
                result = JavaScriptRuntime.String.ReplaceWithRegExp(input, this, arg1);
                return true;
            }

            if (ReferenceEquals(symbol, Symbol.split) && HasIntrinsicWellKnownSymbolFastPath(WellKnownSymbolFastPathFlags.Split))
            {
                result = JavaScriptRuntime.String.SplitWithRegExp(input, this, arg1);
                return true;
            }

            result = null;
            return false;
        }

        internal void InvalidateIntrinsicWellKnownSymbolFastPath(string propertyKey)
        {
            if (string.Equals(propertyKey, MatchSymbolPropertyKey, StringComparison.Ordinal))
            {
                _wellKnownSymbolFastPathFlags &= ~WellKnownSymbolFastPathFlags.Match;
            }
            else if (string.Equals(propertyKey, ReplaceSymbolPropertyKey, StringComparison.Ordinal))
            {
                _wellKnownSymbolFastPathFlags &= ~WellKnownSymbolFastPathFlags.Replace;
            }
            else if (string.Equals(propertyKey, SearchSymbolPropertyKey, StringComparison.Ordinal))
            {
                _wellKnownSymbolFastPathFlags &= ~WellKnownSymbolFastPathFlags.Search;
            }
            else if (string.Equals(propertyKey, SplitSymbolPropertyKey, StringComparison.Ordinal))
            {
                _wellKnownSymbolFastPathFlags &= ~WellKnownSymbolFastPathFlags.Split;
            }
        }

        internal void InvalidateAllIntrinsicWellKnownSymbolFastPaths()
        {
            _wellKnownSymbolFastPathFlags = WellKnownSymbolFastPathFlags.None;
        }

        internal static bool IsIntrinsicPrototypeTarget(object target)
        {
            return ReferenceEquals(target, Prototype);
        }

        internal static void InvalidatePrototypeWellKnownSymbolFastPath(string propertyKey)
        {
            if (string.Equals(propertyKey, MatchSymbolPropertyKey, StringComparison.Ordinal))
            {
                _prototypeWellKnownSymbolFastPathFlags &= ~WellKnownSymbolFastPathFlags.Match;
            }
            else if (string.Equals(propertyKey, ReplaceSymbolPropertyKey, StringComparison.Ordinal))
            {
                _prototypeWellKnownSymbolFastPathFlags &= ~WellKnownSymbolFastPathFlags.Replace;
            }
            else if (string.Equals(propertyKey, SearchSymbolPropertyKey, StringComparison.Ordinal))
            {
                _prototypeWellKnownSymbolFastPathFlags &= ~WellKnownSymbolFastPathFlags.Search;
            }
            else if (string.Equals(propertyKey, SplitSymbolPropertyKey, StringComparison.Ordinal))
            {
                _prototypeWellKnownSymbolFastPathFlags &= ~WellKnownSymbolFastPathFlags.Split;
            }
        }

        internal static void InvalidateAllPrototypeWellKnownSymbolFastPaths()
        {
            _prototypeWellKnownSymbolFastPathFlags = WellKnownSymbolFastPathFlags.None;
        }

        public string toString()
        {
            return "/" + _source + "/" + flags;
        }

        private static string NormalizeSource(object? pattern)
        {
            if (pattern is RegExp existing)
            {
                return existing.source;
            }

            return DotNet2JSConversions.ToString(pattern) ?? string.Empty;
        }

        private static string NormalizeFlags(object? pattern, object? flags)
        {
            if (flags is null || flags is JsNull)
            {
                return pattern is RegExp existing ? existing.flags : string.Empty;
            }

            return DotNet2JSConversions.ToString(flags) ?? string.Empty;
        }

        private static string? TryGetSimpleLiteralPattern(string source, ParsedFlags parsedFlags)
        {
            if (source.Length == 0
                || parsedFlags.IgnoreCase
                || parsedFlags.Sticky
                || parsedFlags.Unicode
                || parsedFlags.HasIndices)
            {
                return null;
            }

            foreach (var ch in source)
            {
                switch (ch)
                {
                    case '\\':
                    case '.':
                    case '^':
                    case '$':
                    case '*':
                    case '+':
                    case '?':
                    case '(':
                    case ')':
                    case '[':
                    case ']':
                    case '{':
                    case '}':
                    case '|':
                        return null;
                }
            }

            return source;
        }

        private static ParsedFlags ParseFlags(string flags)
        {
            var parsed = new ParsedFlags();
            var seenFlags = 0;

            foreach (var ch in flags)
            {
                var flagBit = ch switch
                {
                    'd' => 1 << 0,
                    'g' => 1 << 1,
                    'i' => 1 << 2,
                    'm' => 1 << 3,
                    's' => 1 << 4,
                    'u' => 1 << 5,
                    'v' => 1 << 6,
                    'y' => 1 << 7,
                    _ => -1
                };

                if (flagBit < 0)
                {
                    throw new SyntaxError($"Invalid flags supplied to RegExp constructor '{flags}'");
                }

                if ((seenFlags & flagBit) != 0)
                {
                    throw new SyntaxError($"Invalid flags supplied to RegExp constructor '{flags}'");
                }

                seenFlags |= flagBit;

                switch (ch)
                {
                    case 'd':
                        parsed.HasIndices = true;
                        break;
                    case 'g':
                        parsed.Global = true;
                        break;
                    case 'i':
                        parsed.IgnoreCase = true;
                        break;
                    case 'm':
                        parsed.Multiline = true;
                        break;
                    case 's':
                        parsed.DotAll = true;
                        break;
                    case 'u':
                        parsed.Unicode = true;
                        break;
                    case 'v':
                        throw new SyntaxError("RegExp flag 'v' is not supported yet");
                    case 'y':
                        parsed.Sticky = true;
                        break;
                }
            }

            return parsed;
        }

        private static string PreparePatternForDotNetRegex(string source, bool unicode, bool dotAll)
        {
            var prepared = unicode
                ? RewriteUnicodeCodePointEscapes(source)
                : source;

            if (dotAll)
            {
                return unicode
                    ? RewriteDots(prepared, UnicodeDotAllPattern)
                    : prepared;
            }

            return RewriteDots(prepared, unicode ? UnicodeDotPattern : DotPattern);
        }

        private static string RewriteUnicodeCodePointEscapes(string pattern)
        {
            if (pattern.IndexOf(@"\u{", StringComparison.Ordinal) < 0)
            {
                return pattern;
            }

            var builder = new StringBuilder(pattern.Length);

            for (int i = 0; i < pattern.Length; i++)
            {
                var ch = pattern[i];
                if (ch != '\\' || i == pattern.Length - 1)
                {
                    builder.Append(ch);
                    continue;
                }

                var next = pattern[i + 1];
                if (next == 'u' && i + 2 < pattern.Length && pattern[i + 2] == '{')
                {
                    var closeIndex = pattern.IndexOf('}', i + 3);
                    if (closeIndex < 0)
                    {
                        throw new SyntaxError("Invalid Unicode escape in RegExp pattern");
                    }

                    var hex = pattern.Substring(i + 3, closeIndex - (i + 3));
                    if (hex.Length == 0
                        || hex.Length > 6
                        || !int.TryParse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var codePoint)
                        || codePoint < 0
                        || codePoint > 0x10FFFF)
                    {
                        throw new SyntaxError("Invalid Unicode escape in RegExp pattern");
                    }

                    AppendCodePoint(builder, codePoint);
                    i = closeIndex;
                    continue;
                }

                builder.Append(ch);
                builder.Append(next);
                i++;
            }

            return builder.ToString();
        }

        private static void AppendCodePoint(StringBuilder builder, int codePoint)
        {
            if (codePoint <= 0xFFFF)
            {
                builder.Append((char)codePoint);
                return;
            }

            builder.Append(char.ConvertFromUtf32(codePoint));
        }

        private static string RewriteDots(string pattern, string replacement)
        {
            if (pattern.IndexOf('.') < 0)
            {
                return pattern;
            }

            var builder = new StringBuilder(pattern.Length);
            var insideCharacterClass = false;

            for (int i = 0; i < pattern.Length; i++)
            {
                var ch = pattern[i];
                if (ch == '\\')
                {
                    builder.Append(ch);
                    if (i + 1 < pattern.Length)
                    {
                        builder.Append(pattern[i + 1]);
                        i++;
                    }

                    continue;
                }

                if (ch == '[')
                {
                    insideCharacterClass = true;
                    builder.Append(ch);
                    continue;
                }

                if (ch == ']' && insideCharacterClass)
                {
                    insideCharacterClass = false;
                    builder.Append(ch);
                    continue;
                }

                if (ch == '.' && !insideCharacterClass)
                {
                    builder.Append(replacement);
                    continue;
                }

                builder.Append(ch);
            }

            return builder.ToString();
        }

        private void InitializeIntrinsicSurface()
        {
            PrototypeChain.SetPrototype(this, Prototype);
        }

        private static void DefineSymbolMethod(object target, string symbolPropertyKey, Delegate method)
        {
            PropertyDescriptorStore.DefineOrUpdate(target, symbolPropertyKey, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = method
            });
        }

        private JavaScriptRuntime.Array CreateIndicesArray(Match match)
        {
            var indices = new JavaScriptRuntime.Array(match.Groups.Count);
            for (int i = 0; i < match.Groups.Count; i++)
            {
                var group = match.Groups[i];
                if (!group.Success)
                {
                    indices.Add(JsNull.Null);
                    continue;
                }

                var bounds = new JavaScriptRuntime.Array(2)
                {
                    (double)group.Index,
                    (double)(group.Index + group.Length)
                };
                indices.Add(bounds);
            }

            return indices;
        }

        internal bool TryMatch(string input, out Match match)
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

        internal bool TryGetMatchBounds(string input, out int matchIndex, out int matchLength)
        {
            matchIndex = 0;
            matchLength = 0;

            if (_simpleLiteralPattern is string literalPattern)
            {
                var literalStartAt = 0;
                if (!TryGetMatchStart(input, out literalStartAt))
                {
                    return false;
                }

                var literalIndex = input.IndexOf(literalPattern, literalStartAt, StringComparison.Ordinal);
                if (literalIndex < 0)
                {
                    if (UsesLastIndexSemantics)
                    {
                        lastIndex = 0;
                    }

                    return false;
                }

                matchIndex = literalIndex;
                matchLength = literalPattern.Length;
                return true;
            }

            if (!CanUseEnumerateMatchesFastPath)
            {
                if (!TryMatch(input, out var match))
                {
                    return false;
                }

                matchIndex = match.Index;
                matchLength = match.Length;
                return true;
            }

            var startAt = 0;
            if (!TryGetMatchStart(input, out startAt))
            {
                return false;
            }

            foreach (var match in _regex.EnumerateMatches(input.AsSpan(), startAt))
            {
                matchIndex = match.Index;
                matchLength = match.Length;
                return true;
            }

            if (UsesLastIndexSemantics)
            {
                lastIndex = 0;
            }

            return false;
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

            var truncated = global::System.Math.Truncate(lastIndex);
            if (truncated > input.Length)
            {
                lastIndex = 0;
                return false;
            }

            startAt = (int)truncated;
            return true;
        }

        internal void UpdateLastIndexAfterSuccess(string input, Match match)
        {
            UpdateLastIndexAfterSuccess(input, match.Index, match.Length);
        }

        internal void UpdateLastIndexAfterSuccess(string input, int matchIndex, int matchLength)
        {
            if (!UsesLastIndexSemantics)
            {
                return;
            }

            int nextIndex = matchIndex + matchLength;
            if (matchLength == 0)
            {
                nextIndex = AdvanceStringIndex(input, matchIndex);
            }

            lastIndex = nextIndex;
        }

        private int AdvanceStringIndex(string input, int index)
        {
            if (!_unicode || index < 0 || index >= input.Length)
            {
                return index + 1;
            }

            if (char.IsHighSurrogate(input[index])
                && index + 1 < input.Length
                && char.IsLowSurrogate(input[index + 1]))
            {
                return index + 2;
            }

            return index + 1;
        }

        private sealed class ParsedFlags
        {
            public bool Global { get; set; }
            public bool IgnoreCase { get; set; }
            public bool Multiline { get; set; }
            public bool DotAll { get; set; }
            public bool Unicode { get; set; }
            public bool Sticky { get; set; }
            public bool HasIndices { get; set; }

            public RegexOptions ToRegexOptions()
            {
                var options = RegexOptions.None;
                if (IgnoreCase)
                {
                    options |= RegexOptions.IgnoreCase;
                }

                if (Multiline)
                {
                    options |= RegexOptions.Multiline;
                }

                if (DotAll && !Unicode)
                {
                    options |= RegexOptions.Singleline;
                }

                return options;
            }
        }
    }
}
