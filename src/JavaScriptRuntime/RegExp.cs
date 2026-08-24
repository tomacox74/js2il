using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace JavaScriptRuntime
{
    [IntrinsicObject("RegExp", IntrinsicCallKind.ConstructorLike)]
    public sealed class RegExp : JsObject, IExoticJsObject
    {
        [Flags]
        private enum WellKnownSymbolFastPathFlags
        {
            None = 0,
            Match = 1,
            Replace = 2,
            Search = 4,
            Split = 8,
            MatchAll = 16,
            All = Match | Replace | Search | Split | MatchAll
        }

        private const string DotPattern = "(?:[^\n\r\u2028\u2029])";
        private const string UnicodeDotPattern = "(?:[\uD800-\uDBFF][\uDC00-\uDFFF]|[^\n\r\u2028\u2029])";
        private const string UnicodeDotAllPattern = "(?:[\uD800-\uDBFF][\uDC00-\uDFFF]|[\\s\\S])";
        private static readonly string MatchSymbolPropertyKey = Symbol.match.DebugId;
        private static readonly string MatchAllSymbolPropertyKey = Symbol.matchAll.DebugId;
        private static readonly string ReplaceSymbolPropertyKey = Symbol.replace.DebugId;
        private static readonly string SearchSymbolPropertyKey = Symbol.search.DebugId;
        private static readonly string SplitSymbolPropertyKey = Symbol.split.DebugId;
        private static readonly BuiltinFunction1 MatchSymbolDelegate = MatchSymbolMethod;
        private static readonly BuiltinFunction1 MatchAllSymbolDelegate = MatchAllSymbolMethod;
        private static readonly BuiltinFunction2 ReplaceSymbolDelegate = ReplaceSymbolMethod;
        private static readonly BuiltinFunction1 SearchSymbolDelegate = SearchSymbolMethod;
        private static readonly BuiltinFunction2 SplitSymbolDelegate = SplitSymbolMethod;
        /// <summary>Realm-owned <c>RegExp.prototype</c> intrinsic (issue #1824).</summary>
        internal static JsObject Prototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.RegExpPrototype,
                static () => new JsObject(),
                static prototype => InitializePrototype(prototype));
        internal static JsObject RegExpStringIteratorPrototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.RegExpStringIteratorPrototype,
                static () => new JsObject(),
                static prototype => InitializeRegExpStringIteratorPrototype(prototype));
        private static WellKnownSymbolFastPathFlags _prototypeWellKnownSymbolFastPathFlags = WellKnownSymbolFastPathFlags.All;
        private readonly Regex _regex;
        private readonly bool _global;
        private readonly bool _sticky;
        private readonly bool _dotAll;
        private readonly bool _unicode;
        private readonly bool _hasIndices;
        private readonly string _source;
        private readonly string? _simpleLiteralPattern;
        private readonly (string Name, int Number)[] _namedGroups;
        private readonly int[] _captureResetAncestors;
        private readonly CaptureBoundaryKind[] _captureBoundaryKinds;
        private WellKnownSymbolFastPathFlags _wellKnownSymbolFastPathFlags;

        private enum CaptureBoundaryKind
        {
            Contained,
            LookAhead,
            LookBehind
        }

        public object? lastIndex { get; set; }

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
                _namedGroups = GetNamedGroups(_regex);
                (_captureResetAncestors, _captureBoundaryKinds) =
                    GetCaptureResetAncestors(_source, _regex);
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

        public static object Call()
        {
            return new RegExp();
        }

        public static object Call(object? pattern)
        {
            return Call(pattern, null);
        }

        public static object Call(object? pattern, object? flags)
        {
            if (flags == null && pattern is not null and not JsNull)
            {
                var constructor = JavaScriptRuntime.ObjectRuntime.GetItem(pattern, "constructor");
                if (IsSameRegExpConstructor(constructor) && (IsRegExp(pattern) || constructor != null))
                {
                    return pattern;
                }
            }

            return new RegExp(pattern, flags);
        }

        public static string Escape(object? input)
        {
            if (input is not string value)
            {
                throw new TypeError("RegExp.escape requires a string");
            }

            var result = new StringBuilder(value.Length);
            for (var index = 0; index < value.Length;)
            {
                var start = index;
                var codePoint = (int)value[index];
                var length = 1;

                if (char.IsHighSurrogate(value[index])
                    && index + 1 < value.Length
                    && char.IsLowSurrogate(value[index + 1]))
                {
                    codePoint = char.ConvertToUtf32(value, index);
                    length = 2;
                }

                if (result.Length == 0 && IsAsciiLetterOrDigit(codePoint))
                {
                    AppendHexEscape(result, codePoint);
                }
                else
                {
                    AppendEscapedCodePoint(result, value, start, length, codePoint);
                }

                index += length;
            }

            return result.ToString();
        }

        private static void AppendEscapedCodePoint(StringBuilder result, string input, int start, int length, int codePoint)
        {
            switch (codePoint)
            {
                case '\t':
                    result.Append(@"\t");
                    return;
                case '\n':
                    result.Append(@"\n");
                    return;
                case '\v':
                    result.Append(@"\v");
                    return;
                case '\f':
                    result.Append(@"\f");
                    return;
                case '\r':
                    result.Append(@"\r");
                    return;
            }

            if (IsSyntaxCharacter(codePoint))
            {
                result.Append('\\').Append((char)codePoint);
                return;
            }

            if (IsOtherPunctuator(codePoint) || IsWhiteSpaceOrLineTerminator(codePoint) || IsLoneSurrogate(codePoint))
            {
                if (codePoint <= byte.MaxValue)
                {
                    AppendHexEscape(result, codePoint);
                    return;
                }

                for (var index = 0; index < length; index++)
                {
                    AppendUnicodeEscape(result, input[start + index]);
                }

                return;
            }

            result.Append(input, start, length);
        }

        private static bool IsAsciiLetterOrDigit(int codePoint)
        {
            return codePoint is >= '0' and <= '9'
                or >= 'A' and <= 'Z'
                or >= 'a' and <= 'z';
        }

        private static bool IsSyntaxCharacter(int codePoint)
        {
            return codePoint is '^' or '$' or '\\' or '.' or '*' or '+' or '?' or '(' or ')' or '[' or ']' or '{' or '}' or '|' or '/';
        }

        private static bool IsOtherPunctuator(int codePoint)
        {
            return codePoint is ',' or '-' or '=' or '<' or '>' or '#' or '&' or '!' or '%' or ':' or ';' or '@' or '~' or '\'' or '`' or '"';
        }

        private static bool IsWhiteSpaceOrLineTerminator(int codePoint)
        {
            return codePoint is '\u0009' or '\u000A' or '\u000B' or '\u000C' or '\u000D' or '\u0020' or '\u00A0' or '\u1680'
                or >= '\u2000' and <= '\u200A'
                or '\u2028' or '\u2029' or '\u202F' or '\u205F' or '\u3000' or '\uFEFF';
        }

        private static bool IsLoneSurrogate(int codePoint)
        {
            return codePoint is >= '\uD800' and <= '\uDFFF';
        }

        private static void AppendHexEscape(StringBuilder result, int codePoint)
        {
            result.Append(@"\x").Append(codePoint.ToString("x2", CultureInfo.InvariantCulture));
        }

        private static void AppendUnicodeEscape(StringBuilder result, int codeUnit)
        {
            result.Append(@"\u").Append(codeUnit.ToString("x4", CultureInfo.InvariantCulture));
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
                result.Add(IsCaptureParticipating(match, i) ? g.Value : null);
            }

            DefineResultProperty(result, "index", (double)match.Index);
            DefineResultProperty(result, "input", s);
            DefineResultProperty(result, "groups", CreateNamedGroupsObject(match, indices: false));
            if (_hasIndices)
            {
                DefineResultProperty(result, "indices", CreateIndicesArray(match));
            }

            return result;
        }

        public object matchSymbol(object? input)
        {
            return JavaScriptRuntime.String.MatchWithRegExp(DotNet2JSConversions.ToString(input) ?? string.Empty, this);
        }

        public object matchAllSymbol(object? input)
        {
            return CreateMatchAllIterator(DotNet2JSConversions.ToString(input) ?? string.Empty, this);
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

        private static RegExp GetCurrentThisRegExp(object? thisArgument, string wellKnownSymbolName)
        {
            if (thisArgument is not RegExp regExp)
            {
                throw new TypeError($"RegExp.prototype[@@{wellKnownSymbolName}] called on incompatible receiver");
            }

            return regExp;
        }

        private static object? MatchSymbolMethod(object? thisArgument, object? input)
        {
            var regExp = GetCurrentThisRegExp(thisArgument, "match");
            return JavaScriptRuntime.String.MatchWithRegExp(DotNet2JSConversions.ToString(input) ?? string.Empty, regExp);
        }

        private static object? MatchAllSymbolMethod(object? thisArgument, object? input)
        {
            var regExp = GetCurrentThisRegExp(thisArgument, "matchAll");
            return CreateMatchAllIterator(DotNet2JSConversions.ToString(input) ?? string.Empty, regExp);
        }

        private static object? ReplaceSymbolMethod(object? thisArgument, object? input, object? replacement)
        {
            var regExp = GetCurrentThisRegExp(thisArgument, "replace");
            return JavaScriptRuntime.String.ReplaceWithRegExp(DotNet2JSConversions.ToString(input) ?? string.Empty, regExp, replacement);
        }

        private static object? SearchSymbolMethod(object? thisArgument, object? input)
        {
            var regExp = GetCurrentThisRegExp(thisArgument, "search");
            return JavaScriptRuntime.String.SearchWithRegExp(DotNet2JSConversions.ToString(input) ?? string.Empty, regExp);
        }

        private static object? SplitSymbolMethod(object? thisArgument, object? input, object? limit)
        {
            var regExp = GetCurrentThisRegExp(thisArgument, "split");
            return JavaScriptRuntime.String.SplitWithRegExp(DotNet2JSConversions.ToString(input) ?? string.Empty, regExp, limit);
        }

        private static void InitializePrototype(JsObject prototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            DefineSymbolMethod(prototype, MatchSymbolPropertyKey, MatchSymbolDelegate);
            DefineSymbolMethod(prototype, MatchAllSymbolPropertyKey, MatchAllSymbolDelegate);
            DefineSymbolMethod(prototype, ReplaceSymbolPropertyKey, ReplaceSymbolDelegate);
            DefineSymbolMethod(prototype, SearchSymbolPropertyKey, SearchSymbolDelegate);
            DefineSymbolMethod(prototype, SplitSymbolPropertyKey, SplitSymbolDelegate);
            DefinePrototypeMethod(prototype, "exec", (BuiltinFunction1)PrototypeExec, 1d);
            DefinePrototypeMethod(prototype, "test", (BuiltinFunction1)PrototypeTest, 1d);
            DefinePrototypeMethod(prototype, "toString", (BuiltinFunction0)PrototypeToString, 0d);
            DefinePrototypeGetter(prototype, "dotAll", static regExp => regExp.dotAll);
            DefinePrototypeGetter(prototype, "flags", static regExp => regExp.flags);
            DefinePrototypeGetter(prototype, "global", static regExp => regExp.global);
            DefinePrototypeGetter(prototype, "hasIndices", static regExp => regExp.hasIndices);
            DefinePrototypeGetter(prototype, "ignoreCase", static regExp => regExp.ignoreCase);
            DefinePrototypeGetter(prototype, "multiline", static regExp => regExp.multiline);
            DefinePrototypeGetter(prototype, "source", static regExp => regExp.source);
            DefinePrototypeGetter(prototype, "sticky", static regExp => regExp.sticky);
            DefinePrototypeGetter(prototype, "unicode", static regExp => regExp.unicode);
            DefinePrototypeGetter(prototype, "unicodeSets", static regExp => regExp.unicodeSets);
        }

        private static void InitializeRegExpStringIteratorPrototype(JsObject prototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            DefinePrototypeMethod(prototype, "next", (BuiltinFunction0)RegExpStringIteratorPrototypeNext, 0d);
            DefineSymbolMethod(prototype, Symbol.iterator.DebugId, (BuiltinFunction0)RegExpStringIteratorPrototypeIterator);
        }

        private static void DefinePrototypeMethod(object target, string key, Delegate value, double length)
        {
            Function.InitializeFunctionInstance(
                value,
                length,
                key,
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(value));
            PropertyDescriptorStore.DefineOrUpdate(value, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = null
            });
            PropertyDescriptorStore.DefineOrUpdate(target, key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = value
            });
        }

        private static void DefinePrototypeGetter(
            JsObject prototype,
            string key,
            Func<RegExp, object?> getter)
        {
            PropertyDescriptorStore.DefineOrUpdate(prototype, key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = (BuiltinFunction0)(thisArgument => getter(GetRegExpReceiver(thisArgument, key)))
            });
        }

        private static RegExp GetRegExpReceiver(object? thisArgument, string propertyName)
        {
            if (thisArgument is not RegExp regExp)
            {
                throw new TypeError($"RegExp.prototype.{propertyName} called on incompatible receiver");
            }

            return regExp;
        }

        private static object? PrototypeExec(object? thisArgument, object? input)
            => GetRegExpReceiver(thisArgument, "exec").exec(input);

        private static object? PrototypeTest(object? thisArgument, object? input)
            => GetRegExpReceiver(thisArgument, "test").test(input);

        private static object? PrototypeToString(object? thisArgument)
        {
            return GetRegExpReceiver(thisArgument, "toString").toString();
        }

        private static object? RegExpStringIteratorPrototypeNext(object? thisArgument)
        {
            if (thisArgument is not RegExpStringIterator iterator)
            {
                throw new TypeError("RegExp String Iterator.prototype.next called on incompatible receiver");
            }

            return iterator.Next();
        }

        private static object? RegExpStringIteratorPrototypeIterator(object? thisArgument)
        {
            if (thisArgument is not RegExpStringIterator iterator)
            {
                throw new TypeError("RegExp String Iterator.prototype[Symbol.iterator] called on incompatible receiver");
            }

            return iterator;
        }

        internal override PropertyDescriptorLookup GetOwnPropertyDescriptor(
            string key,
            out JsPropertyDescriptor descriptor)
        {
            if (string.Equals(key, nameof(lastIndex), StringComparison.Ordinal))
            {
                var lookup = PropertyDescriptorStore.GetOwnLookupCore(this, key, out descriptor);
                if (lookup == PropertyDescriptorLookup.Deleted)
                {
                    return lookup;
                }

                if (lookup == PropertyDescriptorLookup.Found)
                {
                    descriptor = PropertyDescriptorStore.CloneDescriptor(descriptor);
                    descriptor.Value = lastIndex;
                    return lookup;
                }

                descriptor = new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Value = lastIndex,
                    Writable = true,
                    Enumerable = false,
                    Configurable = false
                };
                return PropertyDescriptorLookup.Found;
            }

            return base.GetOwnPropertyDescriptor(key, out descriptor);
        }

        internal override bool TryGetOwnPropertyValue(string key, out object? value)
        {
            if (string.Equals(key, nameof(lastIndex), StringComparison.Ordinal))
            {
                value = lastIndex;
                return true;
            }

            return base.TryGetOwnPropertyValue(key, out value);
        }

        internal override bool TryGetInvariantOwnPropertyValue(string key, out object? value)
        {
            if (string.Equals(key, nameof(lastIndex), StringComparison.Ordinal))
            {
                value = lastIndex;
                return true;
            }

            value = null;
            return false;
        }

        internal override bool HasOwnPropertyValue(string key)
            => string.Equals(key, nameof(lastIndex), StringComparison.Ordinal)
                || base.HasOwnPropertyValue(key);

        internal override bool SetOwnPropertyValue(string key, object? value)
        {
            if (string.Equals(key, nameof(lastIndex), StringComparison.Ordinal))
            {
                if (PropertyDescriptorStore.GetOwnLookupCore(this, key, out var descriptor) == PropertyDescriptorLookup.Found
                    && !descriptor.Writable)
                {
                    return false;
                }

                lastIndex = value;
                return true;
            }

            return base.SetOwnPropertyValue(key, value);
        }

        internal override bool DefineOwnProperty(string key, JsPropertyDescriptor descriptor)
        {
            if (string.Equals(key, nameof(lastIndex), StringComparison.Ordinal))
            {
                if (descriptor.Kind != JsPropertyDescriptorKind.Data)
                {
                    return false;
                }

                lastIndex = descriptor.Value;
            }

            return base.DefineOwnProperty(key, descriptor);
        }

        internal override IEnumerable<string> GetOwnPropertyKeys()
        {
            yield return nameof(lastIndex);

            foreach (var key in base.GetOwnPropertyKeys())
            {
                if (!string.Equals(key, nameof(lastIndex), StringComparison.Ordinal))
                {
                    yield return key;
                }
            }
        }

        internal override bool DeleteOwnProperty(string key)
            => !string.Equals(key, nameof(lastIndex), StringComparison.Ordinal)
                && base.DeleteOwnProperty(key);

        private bool HasIntrinsicWellKnownSymbolFastPath(WellKnownSymbolFastPathFlags flag)
        {
            return (_wellKnownSymbolFastPathFlags & flag) != 0
                && (_prototypeWellKnownSymbolFastPathFlags & flag) != 0;
        }

        internal bool CanUseEnumerateMatchesFastPath => !_sticky && !_unicode;

        public static bool IsRegExp(object? value)
        {
            if (value is null or JsNull or string or Symbol || value.GetType().IsValueType)
            {
                return false;
            }

            var match = JavaScriptRuntime.ObjectRuntime.GetItem(value, Symbol.match);
            if (match is not null)
            {
                return TypeUtilities.ToBoolean(match);
            }

            return value is RegExp;
        }

        private static bool IsSameRegExpConstructor(object? value)
        {
            if (value is null or JsNull)
            {
                return false;
            }

            var intrinsic =
                BuiltinDelegateFunctionAdapter.FromDelegate(
                    GlobalThis.RegExp);
            if (ReferenceEquals(value, intrinsic) || Equals(value, intrinsic))
            {
                return true;
            }

            return CallableOperations.HasSameBuiltinDelegateMethod(
                value,
                intrinsic);
        }

        internal bool TryInvokeIntrinsicWellKnownSymbol(Symbol symbol, string input, out object? result)
        {
            if (ReferenceEquals(symbol, Symbol.match) && HasIntrinsicWellKnownSymbolFastPath(WellKnownSymbolFastPathFlags.Match))
            {
                result = JavaScriptRuntime.String.MatchWithRegExp(input, this);
                return true;
            }

            if (ReferenceEquals(symbol, Symbol.matchAll) && HasIntrinsicWellKnownSymbolFastPath(WellKnownSymbolFastPathFlags.MatchAll))
            {
                result = CreateMatchAllIterator(input, this);
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
            else if (string.Equals(propertyKey, MatchAllSymbolPropertyKey, StringComparison.Ordinal))
            {
                _wellKnownSymbolFastPathFlags &= ~WellKnownSymbolFastPathFlags.MatchAll;
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
            else if (string.Equals(propertyKey, MatchAllSymbolPropertyKey, StringComparison.Ordinal))
            {
                _prototypeWellKnownSymbolFastPathFlags &= ~WellKnownSymbolFastPathFlags.MatchAll;
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

        private static RegExpStringIterator CreateMatchAllIterator(string input, RegExp regExp)
        {
            var constructor = ObjectRuntime.GetItem(regExp, "constructor");
            object? species = null;
            if (constructor is not null and not JsNull)
            {
                if (TypeUtilities.IsPrimitive(constructor))
                {
                    throw new TypeError("RegExp constructor is not an object");
                }

                species = ObjectRuntime.GetItem(constructor, Symbol.species);
            }

            var flags = DotNet2JSConversions.ToString(ObjectRuntime.GetItem(regExp, "flags"));
            var matcher = species is null or JsNull
                ? CallableOperations.Construct(
                    BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.RegExp),
                    new object?[] { regExp, flags })
                : CallableOperations.Construct(species, new object?[] { regExp, flags });

            if (matcher is null || TypeUtilities.IsPrimitive(matcher))
            {
                throw new TypeError("RegExp species constructor must return an object");
            }

            var lastIndex = ToLength(ObjectRuntime.GetItem(regExp, nameof(RegExp.lastIndex)));
            ObjectRuntime.SetItem(matcher, nameof(RegExp.lastIndex), lastIndex);

            return new RegExpStringIterator(matcher, input, flags.IndexOf('g') >= 0, flags.IndexOf('u') >= 0);
        }

        private static double ToLength(object? value)
        {
            var number = TypeUtilities.ToNumber(value);
            if (double.IsNaN(number) || number <= 0)
            {
                return 0;
            }

            if (double.IsPositiveInfinity(number))
            {
                return 9007199254740991d;
            }

            return global::System.Math.Min(global::System.Math.Floor(number), 9007199254740991d);
        }

        private sealed class RegExpStringIterator : IJavaScriptIterator
        {
            private readonly object _matcher;
            private readonly string _input;
            private readonly bool _global;
            private readonly bool _unicode;
            private bool _done;

            public RegExpStringIterator(object matcher, string input, bool global, bool unicode)
            {
                _matcher = matcher;
                _input = input;
                _global = global;
                _unicode = unicode;
                PrototypeChain.SetPrototype(this, RegExpStringIteratorPrototype);
            }

            public bool HasReturn => false;

            public IteratorResultObject Next()
            {
                if (_done)
                {
                    return new IteratorResultObject(null, done: true);
                }

                var exec = ObjectRuntime.GetItem(_matcher, "exec");
                object? match;
                if (CallableOperations.IsCallable(exec))
                {
                    match = CallableOperations.Call1(exec, _matcher, _input);
                }
                else if (_matcher is RegExp regExp)
                {
                    match = regExp.exec(_input);
                }
                else
                {
                    throw new TypeError("RegExp exec method is not callable");
                }

                if (match is null or JsNull)
                {
                    _done = true;
                    return new IteratorResultObject(null, done: true);
                }

                if (TypeUtilities.IsPrimitive(match))
                {
                    throw new TypeError("RegExp exec method must return an object or null");
                }

                if (!_global)
                {
                    _done = true;
                    return new IteratorResultObject(match, done: false);
                }

                var matched = DotNet2JSConversions.ToStringRejectingSymbols(ObjectRuntime.GetItem(match, 0d));
                if (matched.Length == 0)
                {
                    var lastIndex = ToLength(ObjectRuntime.GetItem(_matcher, nameof(RegExp.lastIndex)));
                    ObjectRuntime.SetItem(_matcher, nameof(RegExp.lastIndex), AdvanceStringIndex(_input, lastIndex, _unicode));
                }

                return new IteratorResultObject(match, done: false);
            }

            public void Return()
            {
                _done = true;
            }

            private static double AdvanceStringIndex(string input, double index, bool unicode)
            {
                if (!unicode || index + 1 >= input.Length)
                {
                    return index + 1;
                }

                var intIndex = (int)index;
                return char.IsHighSurrogate(input[intIndex])
                    && char.IsLowSurrogate(input[intIndex + 1])
                    ? index + 2
                    : index + 1;
            }
        }

        public string toString()
        {
            return "/" + _source + "/" + flags;
        }

        private static string NormalizeSource(object? pattern)
        {
            if (pattern is null)
            {
                return string.Empty;
            }

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

            prepared = RewriteEmptyCharacterClasses(prepared);

            if (!unicode)
            {
                // Annex B.1.2: outside Unicode mode, a backslash before a character with no
                // special regex meaning (e.g. "\X", or "\x"/"\u" not followed by the right
                // number of hex digits) is just an identity escape for that literal character.
                // .NET has no such fallback and throws, so rewrite these before compiling.
                prepared = RewriteLegacyIdentityEscapes(prepared);

                // Annex B.1.2: "\k<name>" is only a backreference when "name" is an
                // actually-defined named group; otherwise it is a legacy literal match for
                // the characters "k<name>". .NET has no such fallback and throws, so rewrite
                // unresolved references before compiling.
                prepared = RewriteUnresolvedNamedBackreferences(prepared);
            }

            if (dotAll)
            {
                return unicode
                    ? RewriteDots(prepared, UnicodeDotAllPattern)
                    : prepared;
            }

            return RewriteDots(prepared, unicode ? UnicodeDotPattern : DotPattern);
        }

        private const string RecognizedRegexEscapeLetters = "dDwWsSbBnrtvfcxuk";

        private static string RewriteLegacyIdentityEscapes(string pattern)
        {
            if (pattern.IndexOf('\\') < 0)
            {
                return pattern;
            }

            var builder = new StringBuilder(pattern.Length);

            for (int i = 0; i < pattern.Length; i++)
            {
                var ch = pattern[i];
                if (ch != '\\' || i + 1 >= pattern.Length)
                {
                    builder.Append(ch);
                    continue;
                }

                var next = pattern[i + 1];

                if (next == 'x' && !HasHexDigitsAt(pattern, i + 2, 2))
                {
                    builder.Append('x');
                    i++;
                    continue;
                }

                if (next == 'u'
                    && !HasHexDigitsAt(pattern, i + 2, 4)
                    && !(i + 2 < pattern.Length && pattern[i + 2] == '{'))
                {
                    builder.Append('u');
                    i++;
                    continue;
                }

                if (char.IsAsciiLetter(next) && !RecognizedRegexEscapeLetters.Contains(next))
                {
                    builder.Append(next);
                    i++;
                    continue;
                }

                builder.Append(ch).Append(next);
                i++;
            }

            return builder.ToString();
        }

        private static bool HasHexDigitsAt(string pattern, int start, int count)
        {
            if (start + count > pattern.Length)
            {
                return false;
            }

            for (int i = start; i < start + count; i++)
            {
                if (!char.IsAsciiHexDigit(pattern[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static string RewriteUnresolvedNamedBackreferences(string pattern)
        {
            if (pattern.IndexOf(@"\k<", StringComparison.Ordinal) < 0)
            {
                return pattern;
            }

            var definedNames = CollectNamedGroupDefinitions(pattern);
            var builder = new StringBuilder(pattern.Length);

            for (int i = 0; i < pattern.Length; i++)
            {
                var ch = pattern[i];

                if (ch == '\\' && i + 2 < pattern.Length && pattern[i + 1] == 'k' && pattern[i + 2] == '<')
                {
                    var closeIndex = pattern.IndexOf('>', i + 3);
                    if (closeIndex > i + 2)
                    {
                        var name = pattern.Substring(i + 3, closeIndex - (i + 3));
                        if (!definedNames.Contains(name))
                        {
                            builder.Append("k<").Append(Regex.Escape(name)).Append('>');
                            i = closeIndex;
                            continue;
                        }
                    }
                }

                if (ch == '\\' && i + 1 < pattern.Length)
                {
                    builder.Append(ch).Append(pattern[i + 1]);
                    i++;
                    continue;
                }

                builder.Append(ch);
            }

            return builder.ToString();
        }

        private static HashSet<string> CollectNamedGroupDefinitions(string pattern)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);

            for (int i = 0; i < pattern.Length; i++)
            {
                var ch = pattern[i];

                if (ch == '\\' && i + 1 < pattern.Length)
                {
                    i++;
                    continue;
                }

                if (ch == '(' && i + 3 < pattern.Length && pattern[i + 1] == '?' && pattern[i + 2] == '<'
                    && pattern[i + 3] != '=' && pattern[i + 3] != '!')
                {
                    var closeIndex = pattern.IndexOf('>', i + 3);
                    if (closeIndex > i + 2)
                    {
                        names.Add(pattern.Substring(i + 3, closeIndex - (i + 3)));
                    }
                }
            }

            return names;
        }

        /// <summary>
        /// JS's "[]" (an empty character class, which never matches any character) and "[^]"
        /// (its negation, which matches any single character) are not valid .NET character
        /// classes — .NET requires at least one class member. Rewrite exactly those two forms
        /// to .NET-compatible equivalents ("(?!)" and "[\s\S]" respectively); every other
        /// (non-empty) character class is copied through untouched.
        /// </summary>
        private static string RewriteEmptyCharacterClasses(string pattern)
        {
            if (pattern.IndexOf('[') < 0)
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

                if (!insideCharacterClass && ch == '[')
                {
                    if (i + 1 < pattern.Length && pattern[i + 1] == ']')
                    {
                        builder.Append("(?!)");
                        i++;
                        continue;
                    }

                    if (i + 2 < pattern.Length && pattern[i + 1] == '^' && pattern[i + 2] == ']')
                    {
                        builder.Append(@"[\s\S]");
                        i += 2;
                        continue;
                    }

                    insideCharacterClass = true;
                    builder.Append(ch);
                    continue;
                }

                if (insideCharacterClass && ch == ']')
                {
                    insideCharacterClass = false;
                }

                builder.Append(ch);
            }

            return builder.ToString();
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
            PrototypeChain.InitializePrototype(this, Prototype);
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
                if (!IsCaptureParticipating(match, i))
                {
                    indices.Add(null);
                    continue;
                }

                var bounds = new JavaScriptRuntime.Array(2)
                {
                    (double)group.Index,
                    (double)(group.Index + group.Length)
                };
                indices.Add(bounds);
            }

            DefineResultProperty(indices, "groups", CreateNamedGroupsObject(match, indices: true));
            return indices;
        }

        private static void DefineResultProperty(object target, string name, object? value)
        {
            PropertyDescriptorStore.DefineOrUpdate(target, name, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = value,
                Writable = true,
                Enumerable = true,
                Configurable = true
            });
        }

        private static (string Name, int Number)[] GetNamedGroups(Regex regex)
        {
            var groups = new List<(string Name, int Number)>();
            foreach (var name in regex.GetGroupNames())
            {
                if (int.TryParse(name, NumberStyles.None, CultureInfo.InvariantCulture, out _))
                {
                    continue;
                }

                groups.Add((name, regex.GroupNumberFromName(name)));
            }

            return groups.ToArray();
        }

        private object? CreateNamedGroupsObject(Match match, bool indices)
        {
            if (_namedGroups.Length == 0)
            {
                return null;
            }

            var groups = new JsObject();
            PrototypeChain.SetPrototype(groups, JsNull.Null);

            foreach (var (name, number) in _namedGroups)
            {
                var group = match.Groups[number];
                object? value = null;
                if (IsCaptureParticipating(match, number))
                {
                    value = indices
                        ? new JavaScriptRuntime.Array(2)
                        {
                            (double)group.Index,
                            (double)(group.Index + group.Length)
                        }
                        : group.Value;
                }

                groups.SetBoxedValue(name, value);
            }

            return groups;
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
            while (match.Success && IsInvalidUnicodeMatchStart(input, match.Index))
            {
                match = _regex.Match(input, match.Index + 1);
            }

            if (!match.Success || (_sticky && match.Index != startAt))
            {
                if (UsesLastIndexSemantics)
                {
                    SetLastIndexValue(0d);
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
                while (literalIndex >= 0 && IsInvalidUnicodeMatchStart(input, literalIndex))
                {
                    literalIndex = input.IndexOf(literalPattern, literalIndex + 1, StringComparison.Ordinal);
                }

                if (literalIndex < 0)
                {
                    if (UsesLastIndexSemantics)
                    {
                        SetLastIndexValue(0d);
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
                SetLastIndexValue(0d);
            }

            return false;
        }

        /// <summary>
        /// Attempts to match this RegExp's pattern starting at exactly <paramref name="position"/>
        /// (as if a fresh copy of this regexp had its sticky flag forced on and its lastIndex set
        /// to <paramref name="position"/>). Used by <c>String.prototype.split</c>'s general regex
        /// algorithm (ECMA-262 22.2.6.13), which never reads or mutates this instance's own
        /// lastIndex/global/sticky state.
        /// </summary>
        internal bool TryExactMatchAt(string input, int position, out Match match)
        {
            match = _regex.Match(input, position);
            if (!match.Success || match.Index != position || IsInvalidUnicodeMatchStart(input, match.Index))
            {
                match = Match.Empty;
                return false;
            }

            return true;
        }

        private bool TryGetMatchStart(string input, out int startAt)
        {
            startAt = 0;
            var numericLastIndex = GetLastIndexLength();
            if (!UsesLastIndexSemantics)
            {
                return true;
            }

            if (numericLastIndex > input.Length)
            {
                SetLastIndexValue(0d);
                return false;
            }

            startAt = (int)numericLastIndex;
            if (_unicode
                && startAt > 0
                && startAt < input.Length
                && char.IsHighSurrogate(input[startAt - 1])
                && char.IsLowSurrogate(input[startAt]))
            {
                startAt--;
            }

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

            SetLastIndexValue((double)(matchIndex + matchLength));
        }

        internal double GetLastIndexLength()
            => ToLength(ObjectRuntime.GetItem(this, nameof(lastIndex)));

        internal void SetLastIndexValue(object? value)
            => ObjectRuntime.SetItem(this, nameof(lastIndex), value);

        private bool IsInvalidUnicodeMatchStart(string input, int index)
            => _unicode
                && index > 0
                && index < input.Length
                && char.IsHighSurrogate(input[index - 1])
                && char.IsLowSurrogate(input[index]);

        private bool IsCaptureParticipating(Match match, int groupNumber)
        {
            var group = match.Groups[groupNumber];
            if (!group.Success || groupNumber >= _captureResetAncestors.Length)
            {
                return group.Success;
            }

            var ancestorNumber = _captureResetAncestors[groupNumber];
            if (ancestorNumber == 0)
            {
                return true;
            }

            var ancestor = match.Groups[ancestorNumber];
            if (!ancestor.Success)
            {
                return false;
            }

            var ancestorEnd = ancestor.Index + ancestor.Length;
            return _captureBoundaryKinds[groupNumber] switch
            {
                CaptureBoundaryKind.LookAhead => group.Index >= ancestor.Index,
                CaptureBoundaryKind.LookBehind => group.Index + group.Length <= ancestorEnd,
                _ => group.Index >= ancestor.Index
                    && group.Index + group.Length <= ancestorEnd
            };
        }

        private static (int[] Ancestors, CaptureBoundaryKind[] BoundaryKinds) GetCaptureResetAncestors(
            string source,
            Regex regex)
        {
            var groupNumbers = regex.GetGroupNumbers();
            var maxGroupNumber = 0;
            foreach (var groupNumber in groupNumbers)
            {
                maxGroupNumber = global::System.Math.Max(maxGroupNumber, groupNumber);
            }

            var parents = new int[maxGroupNumber + 1];
            var quantified = new bool[maxGroupNumber + 1];
            var boundaryKinds = new CaptureBoundaryKind[maxGroupNumber + 1];
            var stack = new Stack<(int CaptureNumber, int ParentCapture, CaptureBoundaryKind BoundaryKind)>();
            var nextUnnamedCapture = 1;
            var inCharacterClass = false;

            for (var index = 0; index < source.Length; index++)
            {
                var current = source[index];
                if (current == '\\')
                {
                    index++;
                    continue;
                }

                if (current == '[')
                {
                    inCharacterClass = true;
                    continue;
                }

                if (current == ']' && inCharacterClass)
                {
                    inCharacterClass = false;
                    continue;
                }

                if (inCharacterClass)
                {
                    continue;
                }

                if (current == '(')
                {
                    var parentCapture = 0;
                    foreach (var frame in stack)
                    {
                        if (frame.CaptureNumber != 0)
                        {
                            parentCapture = frame.CaptureNumber;
                            break;
                        }
                    }

                    var captureNumber = 0;
                    var boundaryKind = stack.Count == 0
                        ? CaptureBoundaryKind.Contained
                        : stack.Peek().BoundaryKind;
                    if (index + 1 >= source.Length || source[index + 1] != '?')
                    {
                        captureNumber = nextUnnamedCapture++;
                    }
                    else if (index + 2 < source.Length
                        && source[index + 2] is '=' or '!')
                    {
                        boundaryKind = CaptureBoundaryKind.LookAhead;
                    }
                    else if (index + 3 < source.Length
                        && source[index + 2] == '<'
                        && source[index + 3] is '=' or '!')
                    {
                        boundaryKind = CaptureBoundaryKind.LookBehind;
                    }
                    else if (index + 3 < source.Length
                        && source[index + 2] == '<'
                        && source[index + 3] is not '=' and not '!')
                    {
                        var nameEnd = source.IndexOf('>', index + 3);
                        if (nameEnd >= 0)
                        {
                            captureNumber = regex.GroupNumberFromName(source.Substring(index + 3, nameEnd - index - 3));
                        }
                    }

                    if (captureNumber > 0 && captureNumber < parents.Length)
                    {
                        parents[captureNumber] = parentCapture;
                        boundaryKinds[captureNumber] = boundaryKind;
                    }

                    stack.Push((captureNumber, parentCapture, boundaryKind));
                    continue;
                }

                if (current != ')' || stack.Count == 0)
                {
                    continue;
                }

                var closed = stack.Pop();
                if (closed.CaptureNumber > 0
                    && closed.CaptureNumber < quantified.Length
                    && index + 1 < source.Length
                    && source[index + 1] is '*' or '+' or '?' or '{')
                {
                    quantified[closed.CaptureNumber] = true;
                }
            }

            var resetAncestors = new int[maxGroupNumber + 1];
            for (var groupNumber = 1; groupNumber < resetAncestors.Length; groupNumber++)
            {
                var parent = parents[groupNumber];
                while (parent != 0 && !quantified[parent])
                {
                    parent = parents[parent];
                }

                resetAncestors[groupNumber] = parent;
            }

            return (resetAncestors, boundaryKinds);
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
