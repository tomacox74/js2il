using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

namespace JavaScriptRuntime
{
    [IntrinsicObject("RegExp", IntrinsicCallKind.ConstructorLike)]
    public sealed partial class RegExp : JsObject, IExoticJsObject
    {
        private static readonly Func<object[], object?[], object?> _regExpEscapeValue = static (_, args) =>
            JavaScriptRuntime.RegExp.Escape(args != null && args.Length > 0 ? args[0] : null);

        internal static void ConfigureIntrinsicSurface(object constructorValue, object objectPrototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            GlobalThis.ConfigureConstructorPrototypeSurface(constructorValue, JavaScriptRuntime.RegExp.Prototype, objectPrototype);
            Function.DefineMetadataProperty(constructorValue, "length", 2d);
            GlobalThis.DefineSpeciesAccessorProperty(constructorValue);
            GlobalThis.DefineBuiltinFunctionProperty(constructorValue, "escape", _regExpEscapeValue, 1d);
        }

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
        private const string UnicodeDotPattern = "(?:[\uD800-\uDBFF][\uDC00-\uDFFF]|[^\uD800-\uDFFF\n\r\u2028\u2029]|[\uD800-\uDBFF](?![\uDC00-\uDFFF])|(?<![\uD800-\uDBFF])[\uDC00-\uDFFF])";
        private const string UnicodeDotAllPattern = "(?:[\uD800-\uDBFF][\uDC00-\uDFFF]|[^\uD800-\uDFFF]|[\uD800-\uDBFF](?![\uDC00-\uDFFF])|(?<![\uD800-\uDBFF])[\uDC00-\uDFFF])";
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
            => GetPrototype(RuntimeIntrinsics.Current);

        internal static JsObject GetPrototype(RuntimeIntrinsics intrinsics)
            => intrinsics.GetOrCreate(
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
        private readonly bool _unicodeSets;
        private bool FullUnicode => _unicode || _unicodeSets;
        private readonly bool _hasIndices;
        private readonly string _source;
        private readonly string? _simpleLiteralPattern;
        private readonly (string Name, int Number)[] _namedGroups;
        private readonly int[] _captureResetAncestors;
        private readonly CaptureBoundaryKind[] _captureBoundaryKinds;
        private readonly int _captureCount;
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
            : this(pattern, flags, null)
        {
        }

        private RegExp(object? pattern, object? flags, bool? isRegExp)
        {
            var patternIsRegExp = isRegExp ?? IsRegExp(pattern);
            object? source = pattern;
            if (pattern is RegExp existing)
            {
                source = existing._source;
                flags ??= existing.flags;
            }
            else if (patternIsRegExp)
            {
                source = ObjectRuntime.GetItem(pattern!, "source");
                flags ??= ObjectRuntime.GetItem(pattern!, "flags");
            }
            _source = source is null ? string.Empty : DotNet2JSConversions.ToStringRejectingSymbols(source);
            var parsedFlags = ParseFlags(flags is null ? string.Empty : DotNet2JSConversions.ToStringRejectingSymbols(flags));

            _global = parsedFlags.Global;
            _sticky = parsedFlags.Sticky;
            _dotAll = parsedFlags.DotAll;
            _unicode = parsedFlags.Unicode;
            _unicodeSets = parsedFlags.UnicodeSets;
            _hasIndices = parsedFlags.HasIndices;
            _simpleLiteralPattern = null;

            var entry = GetOrCreateCompiledPattern(_source, FullUnicode, _dotAll, parsedFlags.ToRegexOptions(), _unicodeSets);
            var compiled = entry.Value;
            _regex = entry.GetRegexForConstruction();
            _namedGroups = compiled.NamedGroups!;
            _captureResetAncestors = compiled.CaptureResetAncestors!;
            _captureBoundaryKinds = compiled.CaptureBoundaryKinds!;
            _captureCount = compiled.CaptureCount;
            _simpleLiteralPattern = IsSimpleLiteralPattern(compiled, parsedFlags) ? _source : null;

            lastIndex = 0;
            _wellKnownSymbolFastPathFlags = WellKnownSymbolFastPathFlags.All;
            InitializeIntrinsicSurface();
        }

        private RegExp(LiteralTemplate template)
        {
            var flags = template.Flags!;
            var entry = template.Compiled!;
            var compiled = entry.Value;
            _source = template.Source;
            _global = flags.Global;
            _sticky = flags.Sticky;
            _dotAll = flags.DotAll;
            _unicode = flags.Unicode;
            _unicodeSets = flags.UnicodeSets;
            _hasIndices = flags.HasIndices;
            _regex = entry.GetRegexForConstruction();
            _namedGroups = compiled.NamedGroups!;
            _captureResetAncestors = compiled.CaptureResetAncestors!;
            _captureBoundaryKinds = compiled.CaptureBoundaryKinds!;
            _captureCount = compiled.CaptureCount;
            _simpleLiteralPattern = IsSimpleLiteralPattern(compiled, flags)
                ? _source
                : null;
            lastIndex = 0;
            _wellKnownSymbolFastPathFlags = WellKnownSymbolFastPathFlags.All;
            InitializeIntrinsicSurface();
        }

        public static RegExp CreateLiteral(string source, string flags)
        {
            source ??= string.Empty;
            flags ??= string.Empty;
            var templatesByFlags = LiteralTemplates.GetValue(
                source,
                static _ => new ConcurrentDictionary<string, Lazy<LiteralTemplate>>(
                    StringComparer.Ordinal));
            var template = templatesByFlags.GetOrAdd(
                flags,
                currentFlags => new Lazy<LiteralTemplate>(
                    () => CreateLiteralTemplate(source, currentFlags),
                    LazyThreadSafetyMode.ExecutionAndPublication)).Value;
            // Each evaluation must surface a fresh error object; a throwing Lazy would
            // replay the same SyntaxError instance (and any caller mutations of it).
            if (template.SyntaxErrorMessage is { } syntaxErrorMessage)
            {
                throw new SyntaxError(syntaxErrorMessage);
            }

            return new RegExp(template);
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
            var isRegExp = IsRegExp(pattern);
            if (flags == null && isRegExp)
            {
                var constructor = JavaScriptRuntime.ObjectRuntime.GetItem(pattern!, "constructor");
                if (IsSameRegExpConstructor(constructor))
                {
                    return pattern!;
                }
            }

            return new RegExp(pattern, flags, isRegExp);
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
        public bool unicodeSets => _unicodeSets;
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

            var result = new JavaScriptRuntime.Array(_captureCount + 1);
            for (int i = 0; i <= _captureCount; i++)
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

        internal static object? RegExpExec(object receiver, string input)
        {
            var exec = ObjectRuntime.GetItem(receiver, "exec");
            var result = CallableOperations.IsCallable(exec)
                ? CallableOperations.Call1(exec, receiver, input)
                : receiver is RegExp regExp
                    ? regExp.exec(input)
                    : throw new TypeError("RegExp exec method is not callable");

            if (result is not JsNull && (result is null || TypeUtilities.IsPrimitive(result)))
            {
                throw new TypeError("RegExp exec method must return an object or null");
            }

            return result;
        }

        private static object? MatchSymbolMethod(object? thisArgument, object? input)
        {
            if (thisArgument is null or JsNull || TypeUtilities.IsPrimitive(thisArgument))
            {
                throw new TypeError("RegExp.prototype[@@match] requires an object receiver");
            }

            var text = DotNet2JSConversions.ToStringRejectingSymbols(input);
            var flags = DotNet2JSConversions.ToStringRejectingSymbols(ObjectRuntime.GetItem(thisArgument, "flags"));
            if (!flags.Contains('g'))
            {
                return RegExpExec(thisArgument, text) ?? JsNull.Null;
            }

            var unicode = flags.Contains('u') || flags.Contains('v');
            ObjectRuntime.SetItem(thisArgument, nameof(lastIndex), 0d, true);
            var matches = new JavaScriptRuntime.Array();
            while (true)
            {
                var result = RegExpExec(thisArgument, text);
                if (result is null or JsNull)
                {
                    return matches.Count == 0 ? JsNull.Null : matches;
                }

                var matched = DotNet2JSConversions.ToStringRejectingSymbols(ObjectRuntime.GetItem(result, 0d));
                matches.Add(matched);
                if (matched.Length == 0)
                {
                    var index = ToLength(ObjectRuntime.GetItem(thisArgument, nameof(lastIndex)));
                    ObjectRuntime.SetItem(thisArgument, nameof(lastIndex),
                        RegExpStringIterator.AdvanceStringIndex(text, index, unicode), true);
                }
            }
        }

        private static object? MatchAllSymbolMethod(object? thisArgument, object? input)
        {
            if (thisArgument is null or JsNull || TypeUtilities.IsPrimitive(thisArgument))
            {
                throw new TypeError("RegExp.prototype[@@matchAll] requires an object receiver");
            }

            return CreateMatchAllIterator(DotNet2JSConversions.ToStringRejectingSymbols(input), thisArgument);
        }

        private static object? ReplaceSymbolMethod(object? thisArgument, object? input, object? replacement)
        {
            if (thisArgument is null or JsNull || TypeUtilities.IsPrimitive(thisArgument))
            {
                throw new TypeError("RegExp.prototype[@@replace] requires an object receiver");
            }

            return JavaScriptRuntime.String.ReplaceWithRegExp(
                DotNet2JSConversions.ToStringRejectingSymbols(input), thisArgument, replacement);
        }

        private static object? SearchSymbolMethod(object? thisArgument, object? input)
        {
            if (thisArgument is null or JsNull || TypeUtilities.IsPrimitive(thisArgument))
            {
                throw new TypeError("RegExp.prototype[@@search] requires an object receiver");
            }

            var text = DotNet2JSConversions.ToStringRejectingSymbols(input);
            var previousIndex = ObjectRuntime.GetItem(thisArgument, nameof(lastIndex));
            if (!Operators.SameValue(previousIndex, 0d))
            {
                ObjectRuntime.SetItem(thisArgument, nameof(lastIndex), 0d, true);
            }

            var result = RegExpExec(thisArgument, text);
            var currentIndex = ObjectRuntime.GetItem(thisArgument, nameof(lastIndex));
            if (!Operators.SameValue(currentIndex, previousIndex))
            {
                ObjectRuntime.SetItem(thisArgument, nameof(lastIndex), previousIndex, true);
            }

            return result is null or JsNull ? -1d : ObjectRuntime.GetItem(result, "index");
        }

        private static object? SplitSymbolMethod(object? thisArgument, object? input, object? limit)
        {
            if (thisArgument is null or JsNull || TypeUtilities.IsPrimitive(thisArgument))
            {
                throw new TypeError("RegExp.prototype[@@split] requires an object receiver");
            }

            return JavaScriptRuntime.String.SplitWithRegExp(
                DotNet2JSConversions.ToStringRejectingSymbols(input), thisArgument, limit);
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
            DefinePrototypeAccessor(prototype, "flags", (BuiltinFunction0)PrototypeFlags);
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
            => DefinePrototypeAccessor(
                prototype,
                key,
                thisArgument => ReferenceEquals(thisArgument, prototype)
                    ? key == "source" ? "(?:)" : null
                    : getter(GetRegExpReceiver(thisArgument, key)));

        private static void DefinePrototypeAccessor(
            JsObject prototype,
            string key,
            BuiltinFunction0 getter)
        {
            Function.InitializeFunctionInstance(
                getter,
                0d,
                $"get {key}",
                requiresInvocationContext: false);
            PropertyDescriptorStore.DefineOrUpdate(prototype, key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = getter
            });
        }

        private static object PrototypeFlags(object? thisArgument)
        {
            if (TypeUtilities.IsPrimitive(thisArgument))
            {
                throw new TypeError("RegExp.prototype.flags called on incompatible receiver");
            }

            var flags = new StringBuilder(8);
            if (TypeUtilities.ToBoolean(ObjectRuntime.GetItem(thisArgument!, "hasIndices"))) flags.Append('d');
            if (TypeUtilities.ToBoolean(ObjectRuntime.GetItem(thisArgument!, "global"))) flags.Append('g');
            if (TypeUtilities.ToBoolean(ObjectRuntime.GetItem(thisArgument!, "ignoreCase"))) flags.Append('i');
            if (TypeUtilities.ToBoolean(ObjectRuntime.GetItem(thisArgument!, "multiline"))) flags.Append('m');
            if (TypeUtilities.ToBoolean(ObjectRuntime.GetItem(thisArgument!, "dotAll"))) flags.Append('s');
            if (TypeUtilities.ToBoolean(ObjectRuntime.GetItem(thisArgument!, "unicode"))) flags.Append('u');
            if (TypeUtilities.ToBoolean(ObjectRuntime.GetItem(thisArgument!, "unicodeSets"))) flags.Append('v');
            if (TypeUtilities.ToBoolean(ObjectRuntime.GetItem(thisArgument!, "sticky"))) flags.Append('y');
            return flags.ToString();
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

        internal bool CanUseEnumerateMatchesFastPath => !_sticky && !FullUnicode;

        internal bool CanUseBuiltinExecFastPath()
        {
            return !HasOwnPropertyValue("exec")
                && ReferenceEquals(PrototypeChain.GetPrototypeOrNull(this), Prototype)
                && PropertyDescriptorStore.TryGetOwn(Prototype, "exec", out var descriptor)
                && descriptor.Kind == JsPropertyDescriptorKind.Data
                && CallableOperations.HasSameBuiltinDelegateMethod(
                    descriptor.Value,
                    BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction1)PrototypeExec));
        }

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

        internal static bool IsSameRegExpConstructor(object? value)
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

        private static RegExpStringIterator CreateMatchAllIterator(string input, object regExp)
        {
            var constructor = ObjectRuntime.GetItem(regExp, "constructor");
            object? species = null;
            if (constructor is not null)
            {
                if (TypeUtilities.IsPrimitive(constructor))
                {
                    throw new TypeError("RegExp constructor is not an object");
                }

                species = ObjectRuntime.GetItem(constructor, Symbol.species);
            }

            var flagsValue = ObjectRuntime.GetItem(regExp, "flags");
            var flags = DotNet2JSConversions.ToStringRejectingSymbols(flagsValue);
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

            return new RegExpStringIterator(matcher, input, flags.IndexOf('g') >= 0,
                flags.IndexOf('u') >= 0 || flags.IndexOf('v') >= 0);
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

            internal static double AdvanceStringIndex(string input, double index, bool unicode)
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

        private static bool IsSimpleLiteralPattern(CompiledPattern compiled, ParsedFlags parsedFlags)
        {
            return compiled.SourceHasNoSpecialCharacters
                && !parsedFlags.IgnoreCase
                && !parsedFlags.Sticky
                && !parsedFlags.FullUnicode
                && !parsedFlags.HasIndices;
        }

        private static bool HasNoSpecialRegexCharacters(string source)
        {
            if (source.Length == 0)
            {
                return false;
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
                        return false;
                }
            }

            return true;
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
                        parsed.UnicodeSets = true;
                        break;
                    case 'y':
                        parsed.Sticky = true;
                        break;
                }
            }

            if (parsed.Unicode && parsed.UnicodeSets)
            {
                throw new SyntaxError("RegExp flags 'u' and 'v' are mutually exclusive");
            }
            return parsed;
        }

        private static string PreparePatternForDotNetRegex(
            string source, bool unicode, bool dotAll, bool unicodeSets, bool ignoreCase, bool multiline)
        {
            var prepared = unicode ? source : RewriteLegacyIdentityEscapes(source);
            prepared = RewritePatternCharacters(prepared, unicode, unicodeSets, ignoreCase, dotAll, multiline);
            if (unicode) prepared = RewriteUnicodeCodePointEscapes(prepared);

            if (!unicode)
            {
                // Annex B.1.2: outside Unicode mode, a backslash before a character with no
                // special regex meaning (e.g. "\X", or "\x"/"\u" not followed by the right
                // number of hex digits) is just an identity escape for that literal character.
                // .NET has no such fallback and throws, so rewrite these before compiling.

                // Annex B.1.2: "\k<name>" is only a backreference when "name" is an
                // actually-defined named group; otherwise it is a legacy literal match for
                // the characters "k<name>". .NET has no such fallback and throws, so rewrite
                // unresolved references before compiling.
                prepared = RewriteUnresolvedNamedBackreferences(prepared);
            }

            return prepared;
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
                        var name = DecodeCaptureName(pattern.Substring(i + 3, closeIndex - (i + 3)));
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
                        names.Add(DecodeCaptureName(pattern.Substring(i + 3, closeIndex - (i + 3))));
                    }
                }
            }

            return names;
        }

        private static string RewriteCaptureGroups(string pattern, out (string Name, int Number)[] namedGroups, out int captureCount)
        {
            var captures = new Dictionary<int, (int Number, int HeaderLength)>();
            var names = new Dictionary<string, List<int>>(StringComparer.Ordinal);
            var groups = new List<(string Name, int Number)>();
            var inCharacterClass = false;
            for (var index = 0; index < pattern.Length; index++)
            {
                if (pattern[index] == '\\')
                {
                    index++;
                    continue;
                }
                if (pattern[index] == '[') inCharacterClass = true;
                else if (pattern[index] == ']') inCharacterClass = false;
                if (inCharacterClass || pattern[index] != '(')
                {
                    continue;
                }
                if (index + 1 >= pattern.Length || pattern[index + 1] != '?')
                {
                    captures.Add(index, (captures.Count + 1, 1));
                    continue;
                }
                if (index + 3 < pattern.Length && pattern[index + 2] == '<'
                    && pattern[index + 3] is not '=' and not '!')
                {
                    var end = pattern.IndexOf('>', index + 3);
                    if (end < 0) throw new SyntaxError("Invalid named capture group");
                    var name = DecodeCaptureName(pattern[(index + 3)..end]);
                    var number = captures.Count + 1;
                    captures.Add(index, (number, end - index + 1));
                    if (!names.TryGetValue(name, out var numbers))
                    {
                        numbers = new List<int>();
                        names.Add(name, numbers);
                    }
                    numbers.Add(number);
                    groups.Add((name, number));
                    index = end;
                }
            }
            namedGroups = groups.ToArray();
            captureCount = captures.Count;
            if (captures.Count == 0) return pattern;

            var builder = new StringBuilder(pattern.Length);
            var stack = new Stack<(int BodyStart, List<int> Captures)>();
            var resets = new List<(int Offset, string Pattern)>();
            var hasBackReferences = false;
            inCharacterClass = false;
            for (var index = 0; index < pattern.Length; index++)
            {
                if (pattern[index] == '\\' && index + 1 < pattern.Length)
                {
                    List<int>? referencedGroups = null;
                    var end = index + 1;
                    if (!inCharacterClass && pattern[index + 1] == 'k'
                        && index + 2 < pattern.Length && pattern[index + 2] == '<')
                    {
                        end = pattern.IndexOf('>', index + 3);
                        if (end >= 0)
                        {
                            names.TryGetValue(DecodeCaptureName(pattern[(index + 3)..end]), out referencedGroups);
                        }
                    }
                    else if (!inCharacterClass && pattern[index + 1] is >= '1' and <= '9')
                    {
                        while (end + 1 < pattern.Length && char.IsAsciiDigit(pattern[end + 1])) end++;
                        if (int.TryParse(pattern[(index + 1)..(end + 1)], out var number)
                            && number <= captures.Count)
                        {
                            referencedGroups = [number];
                        }
                    }
                    if (referencedGroups != null)
                    {
                        foreach (var number in referencedGroups)
                        {
                            builder.Append("(?(").Append(number).Append(@")\k<").Append(number).Append(">|");
                        }
                        builder.Append(')', referencedGroups.Count);
                        index = end;
                        hasBackReferences = true;
                        continue;
                    }

                    builder.Append(pattern[index]).Append(pattern[++index]);
                    continue;
                }

                if (pattern[index] == '[') inCharacterClass = true;
                else if (pattern[index] == ']') inCharacterClass = false;

                if (!inCharacterClass && pattern[index] == '(')
                {
                    var nested = new List<int>();
                    if (captures.TryGetValue(index, out var capture))
                    {
                        // Explicit numbers retain JS capture order and keep duplicate names distinct.
                        foreach (var frame in stack) frame.Captures.Add(capture.Number);
                        nested.Add(capture.Number);
                        builder.Append("(?<").Append(capture.Number).Append('>');
                        stack.Push((builder.Length, nested));
                        index += capture.HeaderLength - 1;
                        continue;
                    }
                    var headerLength = index + 1 < pattern.Length && pattern[index + 1] == '?'
                        ? index + 2 < pattern.Length && pattern[index + 2] == '<' ? 4 : 3
                        : 1;
                    stack.Push((builder.Length + headerLength, nested));
                }
                else if (!inCharacterClass && pattern[index] == ')' && stack.Count > 0)
                {
                    var frame = stack.Pop();
                    if (index + 1 < pattern.Length
                        && pattern[index + 1] is '*' or '+' or '?' or '{'
                        && frame.Captures.Count > 0)
                    {
                        var reset = new StringBuilder();
                        foreach (var number in frame.Captures)
                        {
                            reset.Append("(?>(?<-").Append(number).Append(">)*)");
                        }
                        resets.Add((frame.BodyStart, reset.Append("(?:").ToString()));
                        resets.Add((builder.Length, ")"));
                    }
                }
                builder.Append(pattern[index]);
            }
            if (hasBackReferences)
            {
                // Balancing groups clear earlier iterations before testing an unset backreference.
                foreach (var reset in resets.OrderByDescending(static item => item.Offset))
                {
                    builder.Insert(reset.Offset, reset.Pattern);
                }
            }
            return builder.ToString();
        }

        private static string DecodeCaptureName(string name)
        {
            if (!name.Contains('\\')) return name;
            var result = new StringBuilder();
            for (var index = 0; index < name.Length;)
            {
                if (name[index] != '\\') result.Append(name[index++]);
                else
                {
                    var point = ReadClassCodeUnit(name, ref index, false)
                        ?? throw new SyntaxError("Invalid named capture identifier");
                    AppendUnicodePoint(result, point);
                }
            }
            return result.ToString();
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

        private void InitializeIntrinsicSurface()
        {
            PrototypeChain.InitializePrototype(this, Prototype);
        }

        private static void DefineSymbolMethod(object target, string symbolPropertyKey, Delegate method)
        {
            var function = BuiltinDelegateFunctionAdapter.FromDelegate(method);
            var functionName = symbolPropertyKey == MatchSymbolPropertyKey ? "[Symbol.match]"
                : symbolPropertyKey == MatchAllSymbolPropertyKey ? "[Symbol.matchAll]"
                : symbolPropertyKey == ReplaceSymbolPropertyKey ? "[Symbol.replace]"
                : symbolPropertyKey == SearchSymbolPropertyKey ? "[Symbol.search]"
                : symbolPropertyKey == SplitSymbolPropertyKey ? "[Symbol.split]"
                : "[Symbol.iterator]";
            Function.InitializeFunctionInstance(
                function,
                method is BuiltinFunction2 ? 2d : method is BuiltinFunction1 ? 1d : 0d,
                functionName,
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(method));
            PropertyDescriptorStore.DefineOrUpdate(function, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = null
            });
            PropertyDescriptorStore.DefineOrUpdate(target, symbolPropertyKey, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = function
            });
        }

        private JavaScriptRuntime.Array CreateIndicesArray(Match match)
        {
            var indices = new JavaScriptRuntime.Array(_captureCount + 1);
            for (int i = 0; i <= _captureCount; i++)
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

        /// <summary>
        /// Pattern-derived artifacts shared by every <see cref="RegExp"/> with the same source/flags.
        /// Cold CLR runner state is weakly retained rather than owned by this process-wide metadata.
        /// </summary>
        private sealed class CompiledPattern
        {
            private readonly WeakReference<Regex>? _regex;
            private readonly string? _preparedPattern;
            private readonly RegexOptions _options;
            private readonly TimeSpan _matchTimeout;

            public Regex? Regex
            {
                get
                {
                    if (_regex is null) return null;
                    if (_regex.TryGetTarget(out var regex)) return regex;
                    regex = new Regex(_preparedPattern!, _options, _matchTimeout);
                    _regex.SetTarget(regex);
                    return regex;
                }
                init
                {
                    if (value is null) return;
                    _preparedPattern = value.ToString();
                    _options = value.Options;
                    _matchTimeout = value.MatchTimeout;
                    // CLR runners retain input-sized backtracking buffers, not just compilation data.
                    _regex = new WeakReference<Regex>(value);
                }
            }
            public (string Name, int Number)[]? NamedGroups;
            public int CaptureCount;
            public int[]? CaptureResetAncestors;
            public CaptureBoundaryKind[]? CaptureBoundaryKinds;
            public bool SourceHasNoSpecialCharacters;
            public string? SyntaxErrorMessage;
        }

        private sealed class CompiledPatternCacheEntry : Lazy<CompiledPattern>
        {
            private RegExpAdaptiveCompilation? _compilation;
            private int _constructed;
            private int _retired;

            internal CompiledPatternCacheEntry(Func<CompiledPattern> factory)
                : base(factory, LazyThreadSafetyMode.ExecutionAndPublication)
            {
            }

            internal Regex GetRegexForConstruction()
            {
                var interpreted = Value.Regex!;
                if (Volatile.Read(ref _retired) != 0)
                {
                    return interpreted;
                }
                var compilation = Volatile.Read(ref _compilation);
                if (compilation is null)
                {
                    if (Interlocked.Exchange(ref _constructed, 1) == 0)
                    {
                        return interpreted;
                    }
                    // Cold entries do not allocate promotion state or synchronization.
                    var candidate = new RegExpAdaptiveCompilation(CompiledRegexBudget, initialConstructionCount: 1);
                    compilation = Interlocked.CompareExchange(ref _compilation, candidate, null) ?? candidate;
                    if (Volatile.Read(ref _retired) != 0)
                    {
                        compilation.Retire();
                    }
                }
                return compilation.GetRegexForConstruction(interpreted);
            }

            internal void Retire()
            {
                Volatile.Write(ref _retired, 1);
                Volatile.Read(ref _compilation)?.Retire();
            }
        }

        private sealed class LiteralTemplate
        {
            public required string Source { get; init; }
            public ParsedFlags? Flags { get; init; }
            public CompiledPatternCacheEntry? Compiled { get; init; }
            public string? SyntaxErrorMessage { get; init; }
        }

        private static readonly ConditionalWeakTable<
            string,
            ConcurrentDictionary<string, Lazy<LiteralTemplate>>> LiteralTemplates = new();

        private readonly record struct CompiledPatternKey(
            string Source,
            bool Unicode,
            bool DotAll,
            RegexOptions Options,
            bool UnicodeSets);

        private const int CompiledPatternCacheCapacity = 1024;
        private const int CompiledPatternCacheSourceCharacterBudget = 4 * 1024 * 1024;

        internal const int CompiledRegexPromotionThreshold = 64;
        private static readonly RegExpCompilationBudget CompiledRegexBudget =
            new(capacity: 256, threshold: CompiledRegexPromotionThreshold);
        private static readonly ConcurrentDictionary<CompiledPatternKey, CompiledPatternCacheEntry>
            CompiledPatternCache = new();
        private static readonly ConcurrentQueue<CompiledPatternKey> CompiledPatternCacheInsertionOrder = new();
        private static long _compiledPatternCacheSourceCharacters;
        private static int _compiledPatternCacheEntryCount;

        internal static (int Entries, long SourceCharacters, int CompiledEntries) GetPatternCacheStatistics()
            => (Volatile.Read(ref _compiledPatternCacheEntryCount),
                Volatile.Read(ref _compiledPatternCacheSourceCharacters),
                CompiledRegexBudget.Count);

        /// <summary>
        /// Compiling a JavaScript pattern into a .NET <see cref="Regex"/> is expensive (milliseconds
        /// for large patterns) and is a pure function of the source plus the flags that affect
        /// compilation, so results are cached. Real-world code re-creates the same literal inside hot
        /// loops (for example <c>emoji-regex</c>), which engines such as V8 also serve from a
        /// compilation cache. Matching state (<c>lastIndex</c>) stays per <see cref="RegExp"/>
        /// instance, and .NET <see cref="Regex"/> instances are immutable and thread-safe, so sharing
        /// them is observationally transparent.
        /// </summary>
        private static CompiledPatternCacheEntry GetOrCreateCompiledPattern(
            string source,
            bool unicode,
            bool dotAll,
            RegexOptions options,
            bool unicodeSets)
        {
            var entry = GetOrCreateCompiledPatternCore(source, unicode, dotAll, options, unicodeSets);
            var compiled = entry.Value;
            if (compiled.SyntaxErrorMessage is not null)
            {
                throw new SyntaxError(compiled.SyntaxErrorMessage);
            }

            return entry;
        }

        /// <summary>
        /// Returns the cached artifacts without throwing; a failed compilation is reported through
        /// <see cref="CompiledPattern.SyntaxErrorMessage"/> so callers decide when to raise it.
        /// </summary>
        private static CompiledPatternCacheEntry GetOrCreateCompiledPatternCore(
            string source,
            bool unicode,
            bool dotAll,
            RegexOptions options,
            bool unicodeSets)
        {
            var key = new CompiledPatternKey(source, unicode, dotAll, options, unicodeSets);
            if (CompiledPatternCache.TryGetValue(key, out var existing))
            {
                return existing;
            }
            var candidate = new CompiledPatternCacheEntry(
                () => CreateCompiledPattern(source, unicode, dotAll, options, unicodeSets));
            var cached = CompiledPatternCache.GetOrAdd(key, candidate);
            if (ReferenceEquals(candidate, cached))
            {
                CompiledPatternCacheInsertionOrder.Enqueue(key);
                Interlocked.Increment(ref _compiledPatternCacheEntryCount);
                Interlocked.Add(ref _compiledPatternCacheSourceCharacters, source.Length);
                TrimCompiledPatternCache();
            }

            return cached;
        }

        private static void TrimCompiledPatternCache()
        {
            // ConcurrentDictionary.Count acquires every lock stripe, so the bounds are tracked
            // with interlocked counters that only inserting threads touch.
            while ((Volatile.Read(ref _compiledPatternCacheEntryCount) > CompiledPatternCacheCapacity
                    || Volatile.Read(ref _compiledPatternCacheSourceCharacters)
                        > CompiledPatternCacheSourceCharacterBudget)
                && CompiledPatternCacheInsertionOrder.TryDequeue(out var oldestKey))
            {
                if (CompiledPatternCache.TryRemove(oldestKey, out var removed))
                {
                    removed.Retire();
                    Interlocked.Decrement(ref _compiledPatternCacheEntryCount);
                    Interlocked.Add(
                        ref _compiledPatternCacheSourceCharacters,
                        -oldestKey.Source.Length);
                }
            }
        }

        private static CompiledPattern CreateCompiledPattern(
            string source,
            bool unicode,
            bool dotAll,
            RegexOptions options,
            bool unicodeSets)
        {
            try
            {
                var validationFlags = (unicodeSets ? "v" : unicode ? "u" : "")
                    + ((options & RegexOptions.IgnoreCase) != 0 ? "i" : "");
                if (!Acornima.Tokenizer.ValidateRegExp(source, validationFlags, out var error))
                {
                    return new CompiledPattern { SyntaxErrorMessage = error.Description };
                }
                var preparedPattern = RewriteCaptureGroups(
                    PreparePatternForDotNetRegex(
                        source, unicode, dotAll, unicodeSets, (options & RegexOptions.IgnoreCase) != 0,
                        (options & RegexOptions.Multiline) != 0),
                    out var namedGroups, out var captureCount);
                preparedPattern = new RepetitionRewriter(preparedPattern).Rewrite();
                var regex = new Regex(preparedPattern, options);
                var (ancestors, boundaryKinds) = GetCaptureResetAncestors(source, regex);

                return new CompiledPattern
                {
                    Regex = regex,
                    NamedGroups = namedGroups,
                    CaptureCount = captureCount,
                    CaptureResetAncestors = ancestors,
                    CaptureBoundaryKinds = boundaryKinds,
                    SourceHasNoSpecialCharacters = HasNoSpecialRegexCharacters(source)
                };
            }
            catch (RegexParseException ex)
            {
                return new CompiledPattern { SyntaxErrorMessage = ex.Message };
            }
            catch (ArgumentException ex)
            {
                return new CompiledPattern { SyntaxErrorMessage = ex.Message };
            }
            catch (SyntaxError ex)
            {
                // Pattern translation rejects unsupported syntax with a JS SyntaxError; it must be
                // recorded rather than thrown so the caching Lazy never replays one instance.
                return new CompiledPattern { SyntaxErrorMessage = ex.Message };
            }
        }

        private static LiteralTemplate CreateLiteralTemplate(string source, string flags)
        {
            // Never let the caching Lazy observe an exception: a cached failure would replay one
            // SyntaxError instance to every evaluation of the literal.
            try
            {
                var parsedFlags = ParseFlags(flags);
                var compiled = GetOrCreateCompiledPatternCore(
                    source,
                    parsedFlags.FullUnicode,
                    parsedFlags.DotAll,
                    parsedFlags.ToRegexOptions(),
                    parsedFlags.UnicodeSets);
                if (compiled.Value.SyntaxErrorMessage is { } syntaxError)
                {
                    return new LiteralTemplate
                    {
                        Source = source,
                        SyntaxErrorMessage = syntaxError
                    };
                }

                return new LiteralTemplate
                {
                    Source = source,
                    Flags = parsedFlags,
                    Compiled = compiled
                };
            }
            catch (SyntaxError ex)
            {
                return new LiteralTemplate { Source = source, SyntaxErrorMessage = ex.Message };
            }
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
                else if (groups.HasOwnPropertyValue(name))
                {
                    continue;
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
            if (FullUnicode
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
            => FullUnicode
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
                            captureNumber = nextUnnamedCapture++;
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
            if (!FullUnicode || index < 0 || index >= input.Length)
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
            public bool UnicodeSets { get; set; }
            public bool FullUnicode => Unicode || UnicodeSets;
            public bool Sticky { get; set; }
            public bool HasIndices { get; set; }

            public RegexOptions ToRegexOptions()
            {
                var options = RegexOptions.CultureInvariant;
                if (IgnoreCase)
                {
                    options |= RegexOptions.IgnoreCase;
                }

                if (Multiline)
                {
                    options |= RegexOptions.Multiline;
                }

                if (DotAll && !FullUnicode)
                {
                    options |= RegexOptions.Singleline;
                }

                return options;
            }
        }
    }
}
