using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.RegularExpressions;

namespace JavaScriptRuntime
{
    /// <summary>
    /// JavaScript String intrinsic helpers used by the IL generator.
    /// Focused, minimal surface to support common patterns during compilation.
    /// </summary>
    [IntrinsicObject("String")]
    public static class String
    {
        private const int MaxRepeatResultLength = 50_000_000;
        private static readonly string MatchSymbolPropertyKey = Symbol.match.DebugId;
        private static readonly string MatchAllSymbolPropertyKey = Symbol.matchAll.DebugId;
        private static readonly string ReplaceSymbolPropertyKey = Symbol.replace.DebugId;
        private static readonly string SearchSymbolPropertyKey = Symbol.search.DebugId;
        private static readonly string SplitSymbolPropertyKey = Symbol.split.DebugId;
        private static readonly string IteratorSymbolPropertyKey = Symbol.iterator.DebugId;
        private static readonly string ToStringTagSymbolPropertyKey = Symbol.toStringTag.DebugId;
        private static readonly string[] Latin1CharStrings = CreateLatin1CharStrings();
        internal static readonly ExpandoObject Prototype = CreatePrototype();
        internal static readonly ExpandoObject StringIteratorPrototype = CreateStringIteratorPrototype();

        private static ExpandoObject CreatePrototype()
        {
            var prototype = new ExpandoObject();

            DefinePrototypeMethod(prototype, "at", (Func<object[], object?[]?, object?>)PrototypeAt);
            DefinePrototypeMethod(prototype, "charAt", (Func<object[], object?[]?, object?>)PrototypeCharAt);
            DefinePrototypeMethod(prototype, "charCodeAt", (Func<object[], object?[]?, object?>)PrototypeCharCodeAt);
            DefinePrototypeMethod(prototype, "codePointAt", (Func<object[], object?[]?, object?>)PrototypeCodePointAt);
            DefinePrototypeMethod(prototype, "endsWith", (Func<object[], object?[]?, object?>)PrototypeEndsWith);
            DefinePrototypeMethod(prototype, "includes", (Func<object[], object?[]?, object?>)PrototypeIncludes);
            DefinePrototypeMethod(prototype, "indexOf", (Func<object[], object?[]?, object?>)PrototypeIndexOf);
            DefinePrototypeMethod(prototype, "isWellFormed", (Func<object[], object?[]?, object?>)PrototypeIsWellFormed);
            DefinePrototypeMethod(prototype, "lastIndexOf", (Func<object[], object?[]?, object?>)PrototypeLastIndexOf);
            DefinePrototypeMethod(prototype, "localeCompare", (Func<object[], object?[]?, object?>)PrototypeLocaleCompare);
            DefinePrototypeMethod(prototype, "match", (Func<object[], object?[]?, object?>)PrototypeMatch);
            DefinePrototypeMethod(prototype, "matchAll", (Func<object[], object?[]?, object?>)PrototypeMatchAll);
            DefinePrototypeMethod(prototype, "padEnd", (Func<object[], object?[]?, object?>)PrototypePadEnd);
            DefinePrototypeMethod(prototype, "padStart", (Func<object[], object?[]?, object?>)PrototypePadStart);
            DefinePrototypeMethod(prototype, "repeat", (Func<object[], object?[]?, object?>)PrototypeRepeat);
            DefinePrototypeMethod(prototype, "replace", (Func<object[], object?[]?, object?>)PrototypeReplace);
            DefinePrototypeMethod(prototype, "replaceAll", (Func<object[], object?[]?, object?>)PrototypeReplaceAll);
            DefinePrototypeMethod(prototype, "search", (Func<object[], object?[]?, object?>)PrototypeSearch);
            DefinePrototypeMethod(prototype, "slice", (Func<object[], object?[]?, object?>)PrototypeSlice);
            DefinePrototypeMethod(prototype, "split", (Func<object[], object?[]?, object?>)PrototypeSplit);
            DefinePrototypeMethod(prototype, "startsWith", (Func<object[], object?[]?, object?>)PrototypeStartsWith);
            DefinePrototypeMethod(prototype, "substring", (Func<object[], object?[]?, object?>)PrototypeSubstring);
            DefinePrototypeMethod(prototype, "toLowerCase", (Func<object[], object?[]?, object?>)PrototypeToLowerCase);
            DefinePrototypeMethod(prototype, "toString", (Func<object[], object?[]?, object?>)PrototypeToString);
            DefinePrototypeMethod(prototype, "toUpperCase", (Func<object[], object?[]?, object?>)PrototypeToUpperCase);
            DefinePrototypeMethod(prototype, "toWellFormed", (Func<object[], object?[]?, object?>)PrototypeToWellFormed);
            DefinePrototypeMethod(prototype, "trim", (Func<object[], object?[]?, object?>)PrototypeTrim);
            DefinePrototypeMethod(prototype, "trimEnd", (Func<object[], object?[]?, object?>)PrototypeTrimEnd);
            DefinePrototypeMethod(prototype, "trimLeft", (Func<object[], object?[]?, object?>)PrototypeTrimStart);
            DefinePrototypeMethod(prototype, "trimRight", (Func<object[], object?[]?, object?>)PrototypeTrimEnd);
            DefinePrototypeMethod(prototype, "trimStart", (Func<object[], object?[]?, object?>)PrototypeTrimStart);
            DefinePrototypeMethod(prototype, "valueOf", (Func<object[], object?[]?, object?>)PrototypeValueOf);
            DefinePrototypeMethod(prototype, IteratorSymbolPropertyKey, (Func<object[], object?[]?, object?>)PrototypeIterator);

            return prototype;
        }

        private static ExpandoObject CreateStringIteratorPrototype()
        {
            var prototype = new ExpandoObject();
            DefinePrototypeMethod(prototype, "next", (Func<object[], object?[]?, object?>)StringIteratorPrototypeNext);
            DefinePrototypeMethod(prototype, IteratorSymbolPropertyKey, (Func<object[], object?[]?, object?>)StringIteratorPrototypeIterator);
            PropertyDescriptorStore.DefineOrUpdate(prototype, ToStringTagSymbolPropertyKey, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "String Iterator"
            });
            return prototype;
        }

        private static void DefinePrototypeMethod(object target, string key, object? value)
        {
            PropertyDescriptorStore.DefineOrUpdate(target, key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = value
            });
        }

        public static void ConfigureIntrinsicSurface(object stringConstructorValue)
        {
            PropertyDescriptorStore.DefineOrUpdate(stringConstructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = Prototype
            });
            DefinePrototypeMethod(stringConstructorValue, "fromCharCode", (Func<object[], object?[]?, object?>)ConstructorFromCharCode);
            DefinePrototypeMethod(stringConstructorValue, "fromCodePoint", (Func<object[], object?[]?, object?>)ConstructorFromCodePoint);
            DefinePrototypeMethod(stringConstructorValue, "raw", (Func<object[], object?[]?, object?>)ConstructorRaw);
            DefinePrototypeMethod(Prototype, "constructor", stringConstructorValue);
        }

        internal static bool TryGetPrototypeProperty(object receiver, string propertyKey, out object? value)
        {
            value = null;

            if (!TryGetDescriptorValue(Prototype, propertyKey, receiver, out value))
            {
                return false;
            }

            return true;
        }

        private static string[] CreateLatin1CharStrings()
        {
            var chars = new string[256];
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = ((char)i).ToString();
            }

            return chars;
        }

        private static string CharToStringFast(char ch)
        {
            return ch <= '\u00FF' ? Latin1CharStrings[ch] : ch.ToString();
        }

        private static bool IsEcmaWhitespaceOrLineTerminator(char ch)
        {
            // ECMA-262 WhiteSpace + LineTerminator code points (BMP subset).
            // This is intentionally explicit to avoid differences with .NET's Unicode trimming.
            return ch switch
            {
                '\u0009' => true, // TAB
                '\u000A' => true, // LF
                '\u000B' => true, // VT
                '\u000C' => true, // FF
                '\u000D' => true, // CR
                '\u0020' => true, // SPACE
                '\u00A0' => true, // NO-BREAK SPACE
                '\u1680' => true, // OGHAM SPACE MARK
                '\u2000' => true, // EN QUAD
                '\u2001' => true, // EM QUAD
                '\u2002' => true, // EN SPACE
                '\u2003' => true, // EM SPACE
                '\u2004' => true, // THREE-PER-EM SPACE
                '\u2005' => true, // FOUR-PER-EM SPACE
                '\u2006' => true, // SIX-PER-EM SPACE
                '\u2007' => true, // FIGURE SPACE
                '\u2008' => true, // PUNCTUATION SPACE
                '\u2009' => true, // THIN SPACE
                '\u200A' => true, // HAIR SPACE
                '\u2028' => true, // LINE SEPARATOR
                '\u2029' => true, // PARAGRAPH SEPARATOR
                '\u202F' => true, // NARROW NO-BREAK SPACE
                '\u205F' => true, // MEDIUM MATHEMATICAL SPACE
                '\u3000' => true, // IDEOGRAPHIC SPACE
                '\uFEFF' => true, // ZERO WIDTH NO-BREAK SPACE (BOM)
                _ => false
            };
        }

        private static string TrimEcma(string? input)
        {
            var s = input ?? string.Empty;
            if (s.Length == 0)
            {
                return s;
            }

            int start = 0;
            int end = s.Length - 1;

            while (start <= end && IsEcmaWhitespaceOrLineTerminator(s[start]))
            {
                start++;
            }

            while (end >= start && IsEcmaWhitespaceOrLineTerminator(s[end]))
            {
                end--;
            }

            if (start == 0 && end == s.Length - 1)
            {
                return s;
            }

            if (end < start)
            {
                return string.Empty;
            }

            return s.Substring(start, end - start + 1);
        }

        private static string TrimStartEcma(string? input)
        {
            var s = input ?? string.Empty;
            if (s.Length == 0)
            {
                return s;
            }

            int start = 0;
            while (start < s.Length && IsEcmaWhitespaceOrLineTerminator(s[start]))
            {
                start++;
            }

            if (start == 0)
            {
                return s;
            }

            return start >= s.Length ? string.Empty : s.Substring(start, s.Length - start);
        }

        private static string TrimEndEcma(string? input)
        {
            var s = input ?? string.Empty;
            if (s.Length == 0)
            {
                return s;
            }

            int end = s.Length - 1;
            while (end >= 0 && IsEcmaWhitespaceOrLineTerminator(s[end]))
            {
                end--;
            }

            if (end == s.Length - 1)
            {
                return s;
            }

            return end < 0 ? string.Empty : s.Substring(0, end + 1);
        }

        private static object? GetArg(object?[]? args, int index)
        {
            return args != null && index < args.Length ? args[index] : null;
        }

        private static string ThisStringValue(object? value)
        {
            return value switch
            {
                string s => s,
                char[] chars => new string(chars),
                StringBuilder builder => builder.ToString(),
                _ => throw new TypeError("String.prototype method called on incompatible receiver")
            };
        }

        private static double ToIntegerOrInfinity(object? value, double defaultValue)
        {
            if (value == null)
            {
                return defaultValue;
            }

            double number;
            try
            {
                number = TypeUtilities.ToNumber(value);
            }
            catch
            {
                return defaultValue;
            }

            if (double.IsNaN(number))
            {
                return 0d;
            }

            if (double.IsInfinity(number))
            {
                return number;
            }

            return global::System.Math.Truncate(number);
        }

        private static int ToLength(object? value, int defaultValue = 0)
        {
            var number = ToIntegerOrInfinity(value, defaultValue);
            if (double.IsNaN(number) || number <= 0)
            {
                return 0;
            }

            if (double.IsPositiveInfinity(number))
            {
                return int.MaxValue;
            }

            return number >= int.MaxValue ? int.MaxValue : (int)number;
        }

        private static bool TryGetDescriptorValue(object target, string propertyKey, object receiver, out object? value)
        {
            value = null;
            if (!PropertyDescriptorStore.TryGetOwn(target, propertyKey, out var descriptor))
            {
                return false;
            }

            if (descriptor.Kind == JsPropertyDescriptorKind.Accessor)
            {
                if (descriptor.Get is null || descriptor.Get is JsNull)
                {
                    value = null;
                    return true;
                }

                if (descriptor.Get is not Delegate getter)
                {
                    throw new TypeError("Property accessor is not a function");
                }

                var previousThis = RuntimeServices.SetCurrentThis(receiver);
                try
                {
                    value = Closure.InvokeWithArgs(getter, System.Array.Empty<object>(), System.Array.Empty<object>());
                    return true;
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }

            value = descriptor.Value;
            return true;
        }

        private static string BuildPadding(string fillString, int totalCount)
        {
            if (totalCount <= 0 || fillString.Length == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder(totalCount);
            while (builder.Length < totalCount)
            {
                var remaining = totalCount - builder.Length;
                if (fillString.Length <= remaining)
                {
                    builder.Append(fillString);
                }
                else
                {
                    builder.Append(fillString, 0, remaining);
                }
            }

            return builder.ToString();
        }

        private static object? PrototypeAt(object[] scopes, object?[]? args)
            => At(ThisStringValue(RuntimeServices.GetCurrentThis()), GetArg(args, 0));

        private static object? PrototypeCharAt(object[] scopes, object?[]? args)
        {
            var input = ThisStringValue(RuntimeServices.GetCurrentThis());
            return args == null || args.Length == 0 ? CharAt(input) : CharAt(input, args[0]);
        }

        private static object? PrototypeCharCodeAt(object[] scopes, object?[]? args)
        {
            var input = ThisStringValue(RuntimeServices.GetCurrentThis());
            return args == null || args.Length == 0 ? CharCodeAt(input) : CharCodeAt(input, args[0]);
        }

        private static object? PrototypeCodePointAt(object[] scopes, object?[]? args)
            => CodePointAt(ThisStringValue(RuntimeServices.GetCurrentThis()), GetArg(args, 0));

        private static object? PrototypeEndsWith(object[] scopes, object?[]? args)
        {
            var input = ThisStringValue(RuntimeServices.GetCurrentThis());
            var searchString = DotNet2JSConversions.ToString(GetArg(args, 0));
            return EndsWith(input, searchString, GetArg(args, 1));
        }

        private static object? PrototypeIncludes(object[] scopes, object?[]? args)
        {
            var input = ThisStringValue(RuntimeServices.GetCurrentThis());
            var searchString = DotNet2JSConversions.ToString(GetArg(args, 0));
            return Includes(input, searchString, GetArg(args, 1));
        }

        private static object? PrototypeIndexOf(object[] scopes, object?[]? args)
        {
            var input = ThisStringValue(RuntimeServices.GetCurrentThis());
            var searchString = DotNet2JSConversions.ToString(GetArg(args, 0));
            return args == null || args.Length < 2
                ? IndexOf(input, searchString)
                : IndexOf(input, searchString, args[1]);
        }

        private static object? PrototypeIsWellFormed(object[] scopes, object?[]? args)
            => IsWellFormed(ThisStringValue(RuntimeServices.GetCurrentThis()));

        private static object? PrototypeIterator(object[] scopes, object?[]? args)
            => CreateIterator(ThisStringValue(RuntimeServices.GetCurrentThis()));

        private static object? PrototypeLastIndexOf(object[] scopes, object?[]? args)
        {
            var input = ThisStringValue(RuntimeServices.GetCurrentThis());
            var searchString = DotNet2JSConversions.ToString(GetArg(args, 0));
            return args == null || args.Length < 2
                ? LastIndexOf(input, searchString)
                : LastIndexOf(input, searchString, args[1]);
        }

        private static object? PrototypeLocaleCompare(object[] scopes, object?[]? args)
        {
            var input = ThisStringValue(RuntimeServices.GetCurrentThis());
            var other = DotNet2JSConversions.ToString(GetArg(args, 0));
            return LocaleCompare(input, other, GetArg(args, 1), GetArg(args, 2));
        }

        private static object? PrototypeMatch(object[] scopes, object?[]? args)
            => Match(ThisStringValue(RuntimeServices.GetCurrentThis()), GetArg(args, 0));

        private static object? PrototypeMatchAll(object[] scopes, object?[]? args)
            => MatchAll(ThisStringValue(RuntimeServices.GetCurrentThis()), GetArg(args, 0));

        private static object? PrototypePadEnd(object[] scopes, object?[]? args)
            => PadEnd(ThisStringValue(RuntimeServices.GetCurrentThis()), GetArg(args, 0), GetArg(args, 1));

        private static object? PrototypePadStart(object[] scopes, object?[]? args)
            => PadStart(ThisStringValue(RuntimeServices.GetCurrentThis()), GetArg(args, 0), GetArg(args, 1));

        private static object? PrototypeRepeat(object[] scopes, object?[]? args)
            => Repeat(ThisStringValue(RuntimeServices.GetCurrentThis()), GetArg(args, 0));

        private static object? PrototypeReplace(object[] scopes, object?[]? args)
            => Replace(ThisStringValue(RuntimeServices.GetCurrentThis()), GetArg(args, 0), GetArg(args, 1));

        private static object? PrototypeReplaceAll(object[] scopes, object?[]? args)
            => ReplaceAll(ThisStringValue(RuntimeServices.GetCurrentThis()), GetArg(args, 0), GetArg(args, 1));

        private static object? PrototypeSearch(object[] scopes, object?[]? args)
            => Search(ThisStringValue(RuntimeServices.GetCurrentThis()), GetArg(args, 0));

        private static object? PrototypeSlice(object[] scopes, object?[]? args)
        {
            var input = ThisStringValue(RuntimeServices.GetCurrentThis());
            return args == null || args.Length < 2
                ? Slice(input, GetArg(args, 0))
                : Slice(input, GetArg(args, 0), args[1]);
        }

        private static object? PrototypeSplit(object[] scopes, object?[]? args)
        {
            var input = ThisStringValue(RuntimeServices.GetCurrentThis());
            return Split(input, GetArg(args, 0), GetArg(args, 1));
        }

        private static object? PrototypeStartsWith(object[] scopes, object?[]? args)
        {
            var input = ThisStringValue(RuntimeServices.GetCurrentThis());
            var searchString = DotNet2JSConversions.ToString(GetArg(args, 0));
            return StartsWith(input, searchString, GetArg(args, 1));
        }

        private static object? PrototypeSubstring(object[] scopes, object?[]? args)
        {
            var input = ThisStringValue(RuntimeServices.GetCurrentThis());
            return args == null || args.Length < 2
                ? Substring(input, GetArg(args, 0))
                : Substring(input, GetArg(args, 0), args[1]);
        }

        private static object? PrototypeToLowerCase(object[] scopes, object?[]? args)
            => ToLowerCase(ThisStringValue(RuntimeServices.GetCurrentThis()));

        private static object? PrototypeToString(object[] scopes, object?[]? args)
            => ThisStringValue(RuntimeServices.GetCurrentThis());

        private static object? PrototypeToUpperCase(object[] scopes, object?[]? args)
            => ToUpperCase(ThisStringValue(RuntimeServices.GetCurrentThis()));

        private static object? PrototypeToWellFormed(object[] scopes, object?[]? args)
            => ToWellFormed(ThisStringValue(RuntimeServices.GetCurrentThis()));

        private static object? PrototypeTrim(object[] scopes, object?[]? args)
            => Trim(ThisStringValue(RuntimeServices.GetCurrentThis()));

        private static object? PrototypeTrimEnd(object[] scopes, object?[]? args)
            => TrimEnd(ThisStringValue(RuntimeServices.GetCurrentThis()));

        private static object? PrototypeTrimStart(object[] scopes, object?[]? args)
            => TrimStart(ThisStringValue(RuntimeServices.GetCurrentThis()));

        private static object? PrototypeValueOf(object[] scopes, object?[]? args)
            => ThisStringValue(RuntimeServices.GetCurrentThis());

        private static object? ConstructorFromCharCode(object[] scopes, object?[]? args)
            => FromCharCode(args);

        private static object? ConstructorFromCodePoint(object[] scopes, object?[]? args)
            => FromCodePoint(args);

        private static object? ConstructorRaw(object[] scopes, object?[]? args)
        {
            var template = GetArg(args, 0);
            var substitutions = args == null || args.Length <= 1
                ? System.Array.Empty<object?>()
                : args[1..];
            return Raw(template, substitutions);
        }

        private static object? StringIteratorPrototypeIterator(object[] scopes, object?[]? args)
            => RuntimeServices.GetCurrentThis();

        private static object? StringIteratorPrototypeNext(object[] scopes, object?[]? args)
        {
            var iterator = RuntimeServices.GetCurrentThis() as PublicStringIterator
                ?? throw new TypeError("String Iterator.prototype.next called on incompatible receiver");
            return iterator.next();
        }

        /// <summary>
        /// Implements String.prototype.substring(start[, end]).
        /// Semantics (subset):
        ///  - start/end are coerced to numbers, NaN -> 0
        ///  - values are truncated toward zero
        ///  - negatives clamp to 0, values > length clamp to length
        ///  - if start > end, the arguments are swapped
        /// </summary>
        public static string Substring(string input, object? start)
        {
            return Substring(input, start, null);
        }

        public static string Substring(string input, object? start, object? end)
        {
            input ??= string.Empty;

            int len = input.Length;

            static int ToClampedIndex(object? value, int length, int defaultValue)
            {
                if (value == null)
                {
                    return defaultValue;
                }

                double d;
                try
                {
                    d = JavaScriptRuntime.TypeUtilities.ToNumber(value);
                }
                catch
                {
                    d = double.NaN;
                }

                if (double.IsNaN(d)) d = 0d;
                if (double.IsNegativeInfinity(d)) d = 0d;
                if (double.IsPositiveInfinity(d)) d = length;

                d = global::System.Math.Truncate(d);

                if (d < 0) d = 0;
                if (d > length) d = length;
                return (int)d;
            }

            int startIndex = ToClampedIndex(start, len, defaultValue: 0);
            int endIndex = ToClampedIndex(end, len, defaultValue: len);

            if (startIndex > endIndex)
            {
                (startIndex, endIndex) = (endIndex, startIndex);
            }

            int count = endIndex - startIndex;
            if (count <= 0)
            {
                return string.Empty;
            }

            return input.Substring(startIndex, count);
        }

        /// <summary>
        /// Implements String.prototype.substr(start[, length]).
        /// Notes:
        ///  - start is coerced to integer (NaN -> 0)
        ///  - if start is negative, it is treated as length+start (clamped to 0)
        ///  - length is coerced to integer; if omitted -> to end; if negative -> 0
        /// </summary>
        public static string Substr(string input, object? start)
        {
            return Substr(input, start, null);
        }

        public static string Substr(string input, object? start, object? length)
        {
            input ??= string.Empty;
            int len = input.Length;

            double startNum;
            try { startNum = TypeUtilities.ToNumber(start); }
            catch { startNum = double.NaN; }
            if (double.IsNaN(startNum) || double.IsNegativeInfinity(startNum)) startNum = 0;
            if (double.IsPositiveInfinity(startNum)) startNum = len;
            startNum = global::System.Math.Truncate(startNum);

            int startIndex = (int)startNum;
            if (startIndex < 0)
            {
                startIndex = len + startIndex;
                if (startIndex < 0) startIndex = 0;
            }
            if (startIndex > len) startIndex = len;

            int maxCount = len - startIndex;

            int count;
            if (length is null)
            {
                count = maxCount;
            }
            else
            {
                double lengthNum;
                try { lengthNum = TypeUtilities.ToNumber(length); }
                catch { lengthNum = double.NaN; }
                if (double.IsNaN(lengthNum) || double.IsNegativeInfinity(lengthNum) || lengthNum < 0) lengthNum = 0;
                if (double.IsPositiveInfinity(lengthNum)) lengthNum = maxCount;
                lengthNum = global::System.Math.Truncate(lengthNum);
                count = (int)lengthNum;
            }

            if (count <= 0 || startIndex >= len)
            {
                return string.Empty;
            }

            if (count > maxCount) count = maxCount;
            return input.Substring(startIndex, count);
        }

        /// <summary>
        /// Implements String.prototype.slice(start[, end]).
        /// </summary>
        public static string Slice(string input, object? start)
        {
            return Slice(input, start, null);
        }

        public static string Slice(string input, object? start, object? end)
        {
            input ??= string.Empty;
            int len = input.Length;

            static int ToSliceIndex(object? value, int length, int defaultValue)
            {
                if (value is null) return defaultValue;

                double d;
                try { d = TypeUtilities.ToNumber(value); }
                catch { d = double.NaN; }
                if (double.IsNaN(d) || double.IsNegativeInfinity(d)) d = 0;
                if (double.IsPositiveInfinity(d)) d = length;
                d = global::System.Math.Truncate(d);

                int i = (int)d;
                if (i < 0) i = length + i;
                if (i < 0) i = 0;
                if (i > length) i = length;
                return i;
            }

            int startIndex = ToSliceIndex(start, len, defaultValue: 0);
            int endIndex = ToSliceIndex(end, len, defaultValue: len);
            if (endIndex < startIndex) return string.Empty;
            return input.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Implements String.prototype.indexOf(searchString[, position]).
        /// </summary>
        public static double IndexOf(string input, string searchString)
        {
            return IndexOf(input, searchString, null);
        }

        public static double IndexOf(string input, string searchString, object? position)
        {
            input ??= string.Empty;
            searchString ??= string.Empty;

            int startIndex = 0;
            if (position is not null)
            {
                double d;
                try { d = TypeUtilities.ToNumber(position); }
                catch { d = double.NaN; }
                if (double.IsNaN(d) || double.IsNegativeInfinity(d)) d = 0;
                if (double.IsPositiveInfinity(d)) d = input.Length;
                d = global::System.Math.Truncate(d);
                startIndex = (int)d;
                if (startIndex < 0) startIndex = 0;
                if (startIndex > input.Length) startIndex = input.Length;
            }

            int idx = input.IndexOf(searchString, startIndex, StringComparison.Ordinal);
            return (double)idx;
        }

        /// <summary>
        /// Implements String.prototype.lastIndexOf(searchString[, position]).
        /// </summary>
        public static double LastIndexOf(string input, string searchString)
        {
            return LastIndexOf(input, searchString, null);
        }

        public static double LastIndexOf(string input, string searchString, object? position)
        {
            input ??= string.Empty;
            searchString ??= string.Empty;

            int len = input.Length;
            int startIndex = len;
            if (position is not null)
            {
                double d;
                try { d = TypeUtilities.ToNumber(position); }
                catch { d = double.NaN; }
                if (double.IsNaN(d)) d = double.PositiveInfinity;
                if (double.IsNegativeInfinity(d)) d = 0;
                if (double.IsPositiveInfinity(d)) d = len;
                d = global::System.Math.Truncate(d);
                startIndex = (int)d;
                if (startIndex < 0) startIndex = 0;
                if (startIndex > len) startIndex = len;
            }

            if (searchString.Length == 0)
            {
                return (double)startIndex;
            }

            if (len == 0)
            {
                return -1;
            }

            int maxStart = len - searchString.Length;
            if (maxStart < 0)
            {
                return -1;
            }

            if (startIndex > maxStart)
            {
                startIndex = maxStart;
            }

            int idx = input.LastIndexOf(searchString, startIndex, StringComparison.Ordinal);
            return (double)idx;
        }

        /// <summary>
        /// Implements String.prototype.trim().
        /// </summary>
        public static string Trim(string input)
        {
            return TrimEcma(input);
        }

        /// <summary>
        /// Implements String.prototype.trimStart()/trimLeft().
        /// </summary>
        public static string TrimStart(string input)
        {
            return TrimStartEcma(input);
        }

        public static string TrimLeft(string input)
        {
            return TrimStart(input);
        }

        /// <summary>
        /// Implements String.prototype.trimEnd()/trimRight().
        /// </summary>
        public static string TrimEnd(string input)
        {
            return TrimEndEcma(input);
        }

        public static string TrimRight(string input)
        {
            return TrimEnd(input);
        }

        /// <summary>
        /// Implements String.prototype.repeat(count).
        /// </summary>
        public static string Repeat(string input, object? count)
        {
            input ??= string.Empty;

            double d;
            try { d = TypeUtilities.ToNumber(count); }
            catch { d = double.NaN; }

            if (double.IsNaN(d)) d = 0;
            if (double.IsInfinity(d) || d < 0)
            {
                throw new RangeError("Invalid count value");
            }

            d = global::System.Math.Truncate(d);
            int n = (int)d;
            if (n <= 0) return string.Empty;

            if (n == 1) return input;

            // Prevent pathological allocations.
            long totalLength = (long)input.Length * n;
            if (totalLength > MaxRepeatResultLength)
            {
                throw new RangeError("Invalid string length");
            }

            var sb = new StringBuilder((int)totalLength);
            for (int i = 0; i < n; i++)
            {
                sb.Append(input);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Implements String.prototype.match(regexp).
        /// Minimal semantics to support common libraries:
        ///  - If regexp is a RegExp with /g, returns an Array of matched substrings or null.
        ///  - Otherwise returns the result of RegExp.exec (Array with groups/index/input) or null.
        ///  - Non-RegExp values are coerced to string and treated as a RegExp pattern.
        /// </summary>
        public static object Match(string input)
        {
            return Match(input, null);
        }

        public static object Match(string input, object? regexp)
        {
            input ??= string.Empty;

            if (TryInvokeWellKnownSymbol(regexp, Symbol.match, MatchSymbolPropertyKey, input, out var symbolResult))
            {
                return symbolResult!;
            }

            JavaScriptRuntime.RegExp re = regexp as JavaScriptRuntime.RegExp
                ?? new JavaScriptRuntime.RegExp(DotNet2JSConversions.ToString(regexp));
            return MatchWithRegExp(input, re);
        }

        /// <summary>
        /// Implements String.prototype.search(regexp).
        /// When a custom @@search override is present its return value is forwarded as-is.
        /// </summary>
        public static object Search(string input)
        {
            return Search(input, null);
        }

        public static object Search(string input, object? regexp)
        {
            input ??= string.Empty;

            if (TryInvokeWellKnownSymbol(regexp, Symbol.search, SearchSymbolPropertyKey, input, out var symbolResult))
            {
                return symbolResult!;
            }

            var re = regexp as JavaScriptRuntime.RegExp
                ?? new JavaScriptRuntime.RegExp(DotNet2JSConversions.ToString(regexp));
            return SearchWithRegExp(input, re);
        }

        public static object MatchAll(string input, object? regexp)
        {
            input ??= string.Empty;

            if (TryInvokeWellKnownSymbol(regexp, Symbol.matchAll, MatchAllSymbolPropertyKey, input, out var symbolResult))
            {
                return symbolResult!;
            }

            RegExp matcher;
            if (regexp is RegExp regExp)
            {
                if (!regExp.global)
                {
                    throw new TypeError("String.prototype.matchAll requires a global RegExp");
                }

                matcher = new RegExp(regExp, regExp.flags);
            }
            else
            {
                matcher = new RegExp(regexp, "g");
            }

            var matches = new JavaScriptRuntime.Array();
            while (true)
            {
                var step = matcher.exec(input);
                if (step is JsNull)
                {
                    break;
                }

                matches.Add(step);
            }

            return matches;
        }

        /// <summary>
        /// Implements String.prototype.charAt([index]).
        /// Returns a one-code-unit string, or the empty string when out of range.
        /// </summary>
        public static string CharAt(string input)
        {
            return CharAt(input, null);
        }

        /// <summary>
        /// Implements String.prototype.charAt(index).
        /// </summary>
        public static string CharAt(string input, object? index)
        {
            input ??= string.Empty;

            var idx = index == null ? 0d : JavaScriptRuntime.TypeUtilities.ToNumber(index);

            if (double.IsNaN(idx))
            {
                idx = 0d;
            }
            else if (double.IsInfinity(idx))
            {
                return string.Empty;
            }

            idx = global::System.Math.Truncate(idx);
            if (idx < 0 || idx >= input.Length)
            {
                return string.Empty;
            }

            return input[(int)idx].ToString();
        }

        /// <summary>
        /// Implements String.prototype.charCodeAt([index]).
        /// Returns a UTF-16 code unit as a number, or NaN when out of range.
        /// </summary>
        public static double CharCodeAt(string input)
        {
            return CharCodeAt(input, null);
        }

        /// <summary>
        /// Implements String.prototype.charCodeAt(index).
        /// </summary>
        public static double CharCodeAt(string input, object? index)
        {
            input ??= string.Empty;

            // JS default index is 0 when omitted/undefined/null.
            var idx = index == null ? 0d : JavaScriptRuntime.TypeUtilities.ToNumber(index);

            if (double.IsNaN(idx) || double.IsInfinity(idx))
            {
                return double.NaN;
            }

            // ToIntegerOrInfinity truncates toward zero.
            idx = global::System.Math.Truncate(idx);

            if (idx < 0 || idx >= input.Length)
            {
                return double.NaN;
            }

            return (double)input[(int)idx];
        }

        public static string ToLowerCase(string input)
        {
            input ??= string.Empty;
            return input.ToLowerInvariant();
        }

        public static string ToUpperCase(string input)
        {
            input ??= string.Empty;
            return input.ToUpperInvariant();
        }

        /// <summary>
        /// Implements String.fromCharCode(codeUnit).
        /// </summary>
        public static string FromCharCode(object? codeUnit)
        {
            return ToCharCodeUnit(codeUnit).ToString();
        }

        /// <summary>
        /// Implements String.fromCharCode(...codeUnits).
        /// </summary>
        public static string FromCharCode(object?[]? codeUnits)
        {
            if (codeUnits == null || codeUnits.Length == 0)
            {
                return string.Empty;
            }

            var chars = new char[codeUnits.Length];
            for (int i = 0; i < codeUnits.Length; i++)
            {
                chars[i] = ToCharCodeUnit(codeUnits[i]);
            }

            return new string(chars);
        }

        private static char ToCharCodeUnit(object? codeUnit)
        {
            double d;
            try
            {
                d = JavaScriptRuntime.TypeUtilities.ToNumber(codeUnit);
            }
            catch
            {
                d = 0;
            }

            if (double.IsNaN(d) || double.IsInfinity(d))
            {
                d = 0;
            }

            // JS: ToUint16
            uint u16 = (uint)((int)global::System.Math.Truncate(d)) & 0xFFFFu;
            return (char)u16;
        }

        public static string FromCodePoint(object?[]? codePoints)
        {
            if (codePoints == null || codePoints.Length == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            foreach (var codePoint in codePoints)
            {
                builder.Append(CodePointToString(codePoint));
            }

            return builder.ToString();
        }

        public static string Raw(object? template, object?[]? substitutions)
        {
            if (template is null || template is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            var raw = JavaScriptRuntime.ObjectRuntime.GetItem(template, "raw");
            if (raw is null || raw is JsNull)
            {
                throw new TypeError("String.raw requires a template.raw property");
            }

            int literalCount = ToLength(JavaScriptRuntime.Object.GetLength(raw));
            if (literalCount == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            for (int i = 0; i < literalCount; i++)
            {
                builder.Append(DotNet2JSConversions.ToString(JavaScriptRuntime.ObjectRuntime.GetItem(raw, (double)i)));
                if (i + 1 >= literalCount)
                {
                    break;
                }

                if (substitutions != null && i < substitutions.Length)
                {
                    builder.Append(DotNet2JSConversions.ToString(substitutions[i]));
                }
            }

            return builder.ToString();
        }

        public static object? At(string input, object? index)
        {
            input ??= string.Empty;

            var relativeIndex = ToIntegerOrInfinity(index, 0d);
            if (double.IsNegativeInfinity(relativeIndex))
            {
                return null;
            }

            var actualIndex = relativeIndex < 0 ? input.Length + (int)relativeIndex : (int)relativeIndex;
            if (actualIndex < 0 || actualIndex >= input.Length)
            {
                return null;
            }

            return CharToStringFast(input[actualIndex]);
        }

        public static object? CodePointAt(string input, object? index)
        {
            input ??= string.Empty;

            var position = ToIntegerOrInfinity(index, 0d);
            if (double.IsInfinity(position))
            {
                return null;
            }

            if (position < 0 || position >= input.Length)
            {
                return null;
            }

            int actualIndex = (int)position;
            char first = input[actualIndex];
            if (char.IsHighSurrogate(first)
                && actualIndex + 1 < input.Length
                && char.IsLowSurrogate(input[actualIndex + 1]))
            {
                return (double)char.ConvertToUtf32(first, input[actualIndex + 1]);
            }

            return (double)first;
        }

        /// <summary>
        /// Implements a subset of String.prototype.startsWith(searchString[, position]).
        /// Uses ordinal comparison and basic ToIntegerOrInfinity coercion for position.
        /// </summary>
        public static bool StartsWith(string input, string searchString)
        {
            return StartsWith(input, searchString, null);
        }

        /// <summary>
        /// Implements a subset of String.prototype.startsWith with optional position argument.
        /// </summary>
        public static bool StartsWith(string input, string searchString, object? position)
        {
            input ??= string.Empty;
            searchString ??= string.Empty;

            int pos = 0;
            if (position != null)
            {
                try
                {
                    double d;
                    if (position is double dd) d = dd;
                    else if (position is float ff) d = ff;
                    else if (position is int ii) d = ii;
                    else if (position is long ll) d = ll;
                    else if (position is string s && double.TryParse(s, out var parsed)) d = parsed;
                    else if (position is IConvertible conv) d = conv.ToDouble(System.Globalization.CultureInfo.InvariantCulture);
                    else d = 0d;

                    if (double.IsNaN(d)) d = 0d;
                    if (double.IsPositiveInfinity(d)) d = input.Length;
                    if (double.IsNegativeInfinity(d)) d = 0d;
                    // Truncate toward zero
                    d = d >= 0 ? global::System.Math.Floor(d) : global::System.Math.Ceiling(d);
                    if (d < 0) d = 0;
                    if (d > input.Length) d = input.Length;
                    pos = (int)d;
                }
                catch { pos = 0; }
            }

            if (searchString.Length == 0)
            {
                return true; // empty string starts at any position (pos is clamped to length)
            }
            if (pos < 0 || pos > input.Length) return false;
            if (pos + searchString.Length > input.Length) return false;
            return input.AsSpan(pos).StartsWith(searchString.AsSpan(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Implements a subset of String.prototype.replace when the pattern is a plain string.
        /// Only replaces the first occurrence, matching JS behavior for string patterns.
        /// When a custom @@replace override is present its return value is forwarded as-is.
        /// </summary>
        public static object Replace(string input, object? patternOrString, object? replacement)
        {
            input ??= string.Empty;

            if (TryInvokeWellKnownSymbol(patternOrString, Symbol.replace, ReplaceSymbolPropertyKey, input, replacement, out var symbolResult))
            {
                return symbolResult!;
            }

            if (patternOrString is RegExp regExp)
            {
                return ReplaceWithRegExp(input, regExp, replacement);
            }

            var pattern = patternOrString?.ToString() ?? string.Empty;
            var repl = replacement?.ToString() ?? string.Empty;
            if (pattern.Length == 0)
            {
                // JS inserts at start for empty pattern; keep simple and return input for now.
                return input;
            }
            var idx = input.IndexOf(pattern, StringComparison.Ordinal);
            if (idx < 0) return input;
            return input.Substring(0, idx) + repl + input.Substring(idx + pattern.Length);
        }

        /// <summary>
        /// Implements a subset of String.prototype.endsWith(searchString[, length]).
        /// If length is provided, the string is treated as if it were truncated to that length.
        /// Uses ordinal comparison.
        /// </summary>
        public static bool EndsWith(string input, string searchString)
        {
            return EndsWith(input, searchString, null);
        }

        public static bool EndsWith(string input, string searchString, object? length)
        {
            input ??= string.Empty;
            searchString ??= string.Empty;

            int len = input.Length;
            if (length != null)
            {
                try
                {
                    double d;
                    if (length is double dd) d = dd;
                    else if (length is float ff) d = ff;
                    else if (length is int ii) d = ii;
                    else if (length is long ll) d = ll;
                    else if (length is string s && double.TryParse(s, out var parsed)) d = parsed;
                    else if (length is IConvertible conv) d = conv.ToDouble(System.Globalization.CultureInfo.InvariantCulture);
                    else d = input.Length;

                    if (double.IsNaN(d)) d = input.Length;
                    if (double.IsNegativeInfinity(d)) d = 0d;
                    if (double.IsPositiveInfinity(d)) d = input.Length;
                    d = d >= 0 ? global::System.Math.Floor(d) : global::System.Math.Ceiling(d);
                    if (d < 0) d = 0;
                    if (d > input.Length) d = input.Length;
                    len = (int)d;
                }
                catch { len = input.Length; }
            }

            if (searchString.Length == 0) return true;
            if (len < searchString.Length) return false;
            return input.AsSpan(0, len).EndsWith(searchString.AsSpan(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Implements a subset of String.prototype.includes(searchString[, position]).
        /// Uses ordinal comparison.
        /// </summary>
        public static bool Includes(string input, string searchString)
        {
            return Includes(input, searchString, null);
        }

        public static bool Includes(string input, string searchString, object? position)
        {
            input ??= string.Empty;
            searchString ??= string.Empty;
            int pos = 0;
            if (position != null)
            {
                try
                {
                    double d;
                    if (position is double dd) d = dd;
                    else if (position is float ff) d = ff;
                    else if (position is int ii) d = ii;
                    else if (position is long ll) d = ll;
                    else if (position is string s && double.TryParse(s, out var parsed)) d = parsed;
                    else if (position is IConvertible conv) d = conv.ToDouble(System.Globalization.CultureInfo.InvariantCulture);
                    else d = 0d;

                    if (double.IsNaN(d)) d = 0d;
                    if (double.IsNegativeInfinity(d)) d = 0d;
                    if (double.IsPositiveInfinity(d)) d = input.Length;
                    d = d >= 0 ? global::System.Math.Floor(d) : global::System.Math.Ceiling(d);
                    if (d < 0) d = 0;
                    if (d > input.Length) d = input.Length;
                    pos = (int)d;
                }
                catch { pos = 0; }
            }
            if (searchString.Length == 0) return true;
            if (pos < 0 || pos > input.Length) return false;
            return input.IndexOf(searchString, pos, StringComparison.Ordinal) >= 0;
        }

        public static bool IsWellFormed(string input)
        {
            input ??= string.Empty;
            for (int i = 0; i < input.Length; i++)
            {
                var ch = input[i];
                if (char.IsHighSurrogate(ch))
                {
                    if (i + 1 >= input.Length || !char.IsLowSurrogate(input[i + 1]))
                    {
                        return false;
                    }

                    i++;
                    continue;
                }

                if (char.IsLowSurrogate(ch))
                {
                    return false;
                }
            }

            return true;
        }

        public static string ToWellFormed(string input)
        {
            input ??= string.Empty;
            if (input.Length == 0)
            {
                return input;
            }

            StringBuilder? builder = null;
            for (int i = 0; i < input.Length; i++)
            {
                var ch = input[i];
                if (char.IsHighSurrogate(ch))
                {
                    if (i + 1 < input.Length && char.IsLowSurrogate(input[i + 1]))
                    {
                        builder?.Append(ch).Append(input[i + 1]);
                        i++;
                        continue;
                    }

                    builder ??= new StringBuilder(input.Length);
                    if (i > 0 && builder.Length == 0)
                    {
                        builder.Append(input, 0, i);
                    }

                    builder.Append('\uFFFD');
                    continue;
                }

                if (char.IsLowSurrogate(ch))
                {
                    builder ??= new StringBuilder(input.Length);
                    if (i > 0 && builder.Length == 0)
                    {
                        builder.Append(input, 0, i);
                    }

                    builder.Append('\uFFFD');
                    continue;
                }

                builder?.Append(ch);
            }

            return builder?.ToString() ?? input;
        }

        public static string PadEnd(string input, object? maxLength, object? fillString)
        {
            return Pad(input, maxLength, fillString, padStart: false);
        }

        public static string PadStart(string input, object? maxLength, object? fillString)
        {
            return Pad(input, maxLength, fillString, padStart: true);
        }

        /// <summary>
        /// Implements a subset of String.prototype.replace when the pattern is a regular expression literal
        /// and the replacement is a string. Supports global and ignoreCase flags.
        /// </summary>
        public static string Replace(string input, string pattern, string replacement, bool global, bool ignoreCase)
        {
            if (input == null) return string.Empty;
            var options = RegexOptions.CultureInvariant;
            if (ignoreCase) options |= RegexOptions.IgnoreCase;

            Regex re;
            try
            {
                re = new Regex(pattern, options);
            }
            catch (RegexParseException)
            {
                // Pattern is not a valid regex, treat as literal string (fallback to simple replace)
                // This handles cases like string.replace('\\', '/') where the pattern is a plain string
                var escapedPattern = Regex.Escape(pattern);
                re = new Regex(escapedPattern, options);
            }
            
            if (global)
            {
                return re.Replace(input, replacement);
            }
            else
            {
                // Replace only the first occurrence
                return re.Replace(input, replacement, 1);
            }
        }

        public static object ReplaceAll(string input, object? searchValue, object? replaceValue)
        {
            input ??= string.Empty;

            if (searchValue is RegExp regExp)
            {
                if (!regExp.global)
                {
                    throw new TypeError("String.prototype.replaceAll called with a non-global RegExp argument");
                }

                return ReplaceWithRegExp(input, regExp, replaceValue);
            }

            if (TryInvokeWellKnownSymbol(searchValue, Symbol.replace, ReplaceSymbolPropertyKey, input, replaceValue, out var symbolResult))
            {
                return symbolResult!;
            }

            var pattern = DotNet2JSConversions.ToString(searchValue);
            if (replaceValue is Func<object[], object, object> callback1)
            {
                if (pattern.Length == 0)
                {
                    return ReplaceEmptyPattern(input, match => DotNet2JSConversions.ToString(callback1(System.Array.Empty<object>(), match)) ?? string.Empty);
                }

                return ReplaceLiteralWithCallback(input, pattern, global: true, match => DotNet2JSConversions.ToString(callback1(System.Array.Empty<object>(), match)) ?? string.Empty);
            }

            if (replaceValue is Func<object[], object> callback0)
            {
                if (pattern.Length == 0)
                {
                    return ReplaceEmptyPattern(input, _ => DotNet2JSConversions.ToString(callback0(System.Array.Empty<object>())) ?? string.Empty);
                }

                return ReplaceLiteralWithCallback(input, pattern, global: true, _ => DotNet2JSConversions.ToString(callback0(System.Array.Empty<object>())) ?? string.Empty);
            }

            var replacementText = DotNet2JSConversions.ToString(replaceValue) ?? string.Empty;
            if (pattern.Length == 0)
            {
                return ReplaceEmptyPattern(input, _ => replacementText);
            }

            return input.Replace(pattern, replacementText, StringComparison.Ordinal);
        }

        private static string Pad(string input, object? maxLength, object? fillString, bool padStart)
        {
            input ??= string.Empty;
            int targetLength = ToLength(maxLength, input.Length);
            if (targetLength <= input.Length)
            {
                return input;
            }

            var filler = fillString == null ? " " : DotNet2JSConversions.ToString(fillString);
            if (filler.Length == 0)
            {
                return input;
            }

            var padding = BuildPadding(filler, targetLength - input.Length);
            return padStart ? padding + input : input + padding;
        }

        /// <summary>
        /// Replace with a callback function: replacement can be a bound delegate. We support common delegate
        /// shapes produced by the compiler (Func<object[], object> and Func<object[], object, object>). The callback
        /// receives the matched substring; its return value is coerced to string.
        /// </summary>
        public static string Replace(string input, string pattern, object replacementCallback, bool global, bool ignoreCase)
        {
            if (input == null) return string.Empty;
            var options = RegexOptions.CultureInvariant;
            if (ignoreCase) options |= RegexOptions.IgnoreCase;
            var re = new Regex(pattern, options);

            string Invoke(object? cb, string match)
            {
                if (cb is Func<object[], object, object> f1)
                {
                    var r = f1(System.Array.Empty<object>(), match);
                    return DotNet2JSConversions.ToString(r);
                }
                if (cb is Func<object[], object> f0)
                {
                    var r = f0(System.Array.Empty<object>());
                    return DotNet2JSConversions.ToString(r);
                }
                // Fallback: ToString on callback object (unlikely useful)
                return DotNet2JSConversions.ToString(cb);
            }

            var evaluator = new MatchEvaluator(m => Invoke(replacementCallback, m.Value));
            if (global)
            {
                return re.Replace(input, evaluator);
            }
            else
            {
                // Replace the first match only
                int count = 0;
                return re.Replace(input, m =>
                {
                    if (count++ == 0) return Invoke(replacementCallback, m.Value);
                    return m.Value;
                });
            }
        }

        /// <summary>
        /// Implements a subset of String.prototype.localeCompare.
        /// Supports locales (ignored) and options; recognizes options.numeric === true to enable numeric-aware comparison.
        /// Returns -1, 0, or 1 as a JavaScript number (double).
        /// </summary>
        public static double LocaleCompare(string input, string other, object? locales, object? options)
        {
            input ??= string.Empty;
            other ??= string.Empty;
            var numeric = OptionsHasNumericTrue(options);
            if (!numeric)
            {
                return string.Compare(input, other, StringComparison.Ordinal) switch
                {
                    < 0 => -1d,
                    0 => 0d,
                    _ => 1d
                };
            }
            var cmp = NumericAwareCompare(input, other);
            return cmp < 0 ? -1d : (cmp > 0 ? 1d : 0d);
        }

        private static bool OptionsHasNumericTrue(object? options)
        {
            if (options == null) return false;

            // Handle ExpandoObject/dynamic dictionaries
            if (options is System.Collections.Generic.IDictionary<string, object> dict)
            {
                foreach (var kv in dict)
                {
                    if (string.Equals(kv.Key, "numeric", StringComparison.OrdinalIgnoreCase))
                    {
                        var val = kv.Value;
                        if (val is bool b) return b;
                        if (val is string s && bool.TryParse(s, out var pb)) return pb;
                        if (val is IConvertible c) { try { return Convert.ToBoolean(c); } catch { }
                        }
                        return false;
                    }
                }
            }

            // Fallback: try reflection for POCO-style options objects
            try
            {
                var t = options.GetType();
                var prop = t.GetProperty("numeric", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                if (prop != null)
                {
                    var val = prop.GetValue(options);
                    if (val is bool b) return b;
                    if (val is string s && bool.TryParse(s, out var pb)) return pb;
                    if (val is IConvertible c) { try { return Convert.ToBoolean(c); } catch { } }
                }
            }
            catch { }
            return false;
        }

        private static int NumericAwareCompare(string a, string b)
        {
            int ia = 0, ib = 0;
            while (ia < a.Length && ib < b.Length)
            {
                char ca = a[ia];
                char cb = b[ib];
                bool da = char.IsDigit(ca);
                bool db = char.IsDigit(cb);
                if (da && db)
                {
                    // Compare consecutive digit runs numerically
                    long na = 0, nb = 0;
                    int sa = ia, sb = ib;
                    while (ia < a.Length && char.IsDigit(a[ia])) { na = na * 10 + (a[ia] - '0'); ia++; }
                    while (ib < b.Length && char.IsDigit(b[ib])) { nb = nb * 10 + (b[ib] - '0'); ib++; }
                    if (na != nb) return na < nb ? -1 : 1;
                    // If equal numbers, shorter digit run sorts before longer (e.g., 2 vs 02 -> 2 < 02)
                    int la = ia - sa, lb = ib - sb;
                    if (la != lb) return la < lb ? -1 : 1;
                    continue;
                }
                if (ca != cb) return ca < cb ? -1 : 1;
                ia++; ib++;
            }
            if (ia == a.Length && ib == b.Length) return 0;
            return ia == a.Length ? -1 : 1;
        }

        /// <summary>
        /// Implements a subset of String.prototype.split(separator[, limit]).
        /// Supports string or regular expression separators and optional limit.
        /// Returns a JavaScriptRuntime.Array of strings.
        /// </summary>
        public static object Split(string input)
        {
            return Split(input, null, null);
        }

        public static object Split(string input, object? separatorOrPattern)
        {
            return Split(input, separatorOrPattern, null);
        }

        /// <summary>
        /// Split with optional limit. Separator can be string, Regex, or null/undefined.
        /// </summary>
        public static object Split(string input, object? separatorOrPattern, object? limit)
        {
            input ??= string.Empty;
            int maxCount = ToSplitLimit(limit);
            if (maxCount == 0)
            {
                return new JavaScriptRuntime.Array();
            }

            // Undefined/null separator => return [input]
            if (separatorOrPattern == null)
            {
                var one = new JavaScriptRuntime.Array(1) { input };
                return one;
            }

            if (TryInvokeWellKnownSymbol(separatorOrPattern, Symbol.split, SplitSymbolPropertyKey, input, limit, out var splitResult))
            {
                return splitResult!;
            }

            // Regex separator path
            if (separatorOrPattern is Regex rx)
            {
                return SplitWithRegex(input, rx, maxCount);
            }

            // String (or coercible) separator
            var sep = DotNet2JSConversions.ToString(separatorOrPattern);
            if (sep.Length == 0)
            {
                // Empty string => split into UTF-16 code units
                int take = global::System.Math.Min(input.Length, maxCount);
                var items = new object?[take];
                for (int i = 0; i < take; i++)
                {
                    items[i] = CharToStringFast(input[i]);
                }

                return new JavaScriptRuntime.Array(items);
            }

            return SplitWithLiteralSeparator(input, sep, maxCount);
        }

        internal static object MatchWithRegExp(string input, JavaScriptRuntime.RegExp re)
        {
            if (!re.Global)
            {
                var single = re.exec(input);
                return single is JsNull ? JsNull.Null : single;
            }

            var result = new JavaScriptRuntime.Array();
            re.lastIndex = 0;

            if (re.SimpleLiteralPattern is string literalPattern)
            {
                var searchIndex = 0;
                while (true)
                {
                    var matchIndex = input.IndexOf(literalPattern, searchIndex, StringComparison.Ordinal);
                    if (matchIndex < 0)
                    {
                        break;
                    }

                    result.Add(literalPattern);
                    searchIndex = matchIndex + literalPattern.Length;
                }

                re.lastIndex = 0;
                return result.Count == 0 ? JsNull.Null : result;
            }

            if (re.CanUseEnumerateMatchesFastPath)
            {
                foreach (var match in re.Regex.EnumerateMatches(input.AsSpan()))
                {
                    result.Add(input.Substring(match.Index, match.Length));
                }

                re.lastIndex = 0;
                return result.Count == 0 ? JsNull.Null : result;
            }

            while (re.TryMatch(input, out var match))
            {
                result.Add(match.Value);
                re.UpdateLastIndexAfterSuccess(input, match);

                if (re.lastIndex > input.Length)
                {
                    break;
                }
            }

            re.lastIndex = 0;
            return result.Count == 0 ? JsNull.Null : result;
        }

        internal static double SearchWithRegExp(string input, JavaScriptRuntime.RegExp re)
        {
            var savedLastIndex = re.lastIndex;
            try
            {
                re.lastIndex = 0;
                if (!re.TryGetMatchBounds(input, out var matchIndex, out _))
                {
                    return -1d;
                }

                return matchIndex;
            }
            finally
            {
                re.lastIndex = savedLastIndex;
            }
        }

        internal static string ReplaceWithRegExp(string input, JavaScriptRuntime.RegExp regExp, object? replacement)
        {
            var savedLastIndex = regExp.lastIndex;
            try
            {
                if (TryReplaceWithLiteralPattern(input, regExp, replacement, out var literalReplaceResult))
                {
                    return literalReplaceResult;
                }

                if (replacement is Func<object[], object, object> || replacement is Func<object[], object>)
                {
                    string Invoke(string match)
                    {
                        if (replacement is Func<object[], object, object> cb1)
                        {
                            var r = cb1(System.Array.Empty<object>(), match);
                            return DotNet2JSConversions.ToString(r) ?? string.Empty;
                        }

                        var cb0 = (Func<object[], object>)replacement;
                        var r0 = cb0(System.Array.Empty<object>());
                        return DotNet2JSConversions.ToString(r0) ?? string.Empty;
                    }

                    var evaluator = new MatchEvaluator(m => Invoke(m.Value));
                    if (regExp.Global)
                    {
                        return regExp.Regex.Replace(input, evaluator);
                    }

                    return regExp.Regex.Replace(input, evaluator, 1, 0);
                }

                var replacementText = DotNet2JSConversions.ToString(replacement) ?? string.Empty;
                if (regExp.Global)
                {
                    return regExp.Regex.Replace(input, replacementText);
                }

                return regExp.Regex.Replace(input, replacementText, 1);
            }
            finally
            {
                regExp.lastIndex = savedLastIndex;
            }
        }

        private static bool TryReplaceWithLiteralPattern(string input, JavaScriptRuntime.RegExp regExp, object? replacement, out string result)
        {
            result = string.Empty;
            if (regExp.SimpleLiteralPattern is not string literalPattern)
            {
                return false;
            }

            if (replacement is Func<object[], object, object> || replacement is Func<object[], object>)
            {
                string InvokeReplacement(string match)
                {
                    if (replacement is Func<object[], object, object> cb1)
                    {
                        var callbackResult = cb1(System.Array.Empty<object>(), match);
                        return DotNet2JSConversions.ToString(callbackResult) ?? string.Empty;
                    }

                    var cb0 = (Func<object[], object>)replacement;
                    var zeroArgResult = cb0(System.Array.Empty<object>());
                    return DotNet2JSConversions.ToString(zeroArgResult) ?? string.Empty;
                }

                result = ReplaceLiteralWithCallback(input, literalPattern, regExp.Global, InvokeReplacement);
                return true;
            }

            var replacementText = DotNet2JSConversions.ToString(replacement) ?? string.Empty;
            if (replacementText.IndexOf('$') >= 0)
            {
                return false;
            }

            if (regExp.Global)
            {
                result = input.Replace(literalPattern, replacementText, StringComparison.Ordinal);
                return true;
            }

            var firstMatchIndex = input.IndexOf(literalPattern, StringComparison.Ordinal);
            if (firstMatchIndex < 0)
            {
                result = input;
                return true;
            }

            result = input.Substring(0, firstMatchIndex)
                + replacementText
                + input.Substring(firstMatchIndex + literalPattern.Length);
            return true;
        }

        private static string ReplaceLiteralWithCallback(string input, string literalPattern, bool global, Func<string, string> replacementFactory)
        {
            var firstMatchIndex = input.IndexOf(literalPattern, StringComparison.Ordinal);
            if (firstMatchIndex < 0)
            {
                return input;
            }

            if (!global)
            {
                return input.Substring(0, firstMatchIndex)
                    + replacementFactory(literalPattern)
                    + input.Substring(firstMatchIndex + literalPattern.Length);
            }

            var builder = new StringBuilder(input.Length);
            var searchIndex = 0;
            while (true)
            {
                var matchIndex = input.IndexOf(literalPattern, searchIndex, StringComparison.Ordinal);
                if (matchIndex < 0)
                {
                    builder.Append(input, searchIndex, input.Length - searchIndex);
                    break;
                }

                builder.Append(input, searchIndex, matchIndex - searchIndex);
                builder.Append(replacementFactory(literalPattern));
                searchIndex = matchIndex + literalPattern.Length;
            }

            return builder.ToString();
        }

        internal static JavaScriptRuntime.Array SplitWithRegExp(string input, JavaScriptRuntime.RegExp regExp, object? limit)
        {
            var savedLastIndex = regExp.lastIndex;
            try
            {
                if (regExp.IsEmptySplitPattern)
                {
                    return SplitWithEmptyRegExp(input, regExp.unicode, ToSplitLimit(limit));
                }

                if (regExp.SimpleLiteralPattern is string literalPattern)
                {
                    return SplitWithLiteralSeparator(input, literalPattern, ToSplitLimit(limit));
                }

                return SplitWithRegex(input, regExp.Regex, ToSplitLimit(limit));
            }
            finally
            {
                regExp.lastIndex = savedLastIndex;
            }
        }

        private static JavaScriptRuntime.Array SplitWithEmptyRegExp(string input, bool unicode, int maxCount)
        {
            var estimatedCount = global::System.Math.Min(input.Length, maxCount);
            if (input.Length == 0 || maxCount == 0)
            {
                return new JavaScriptRuntime.Array();
            }

            var items = new object?[estimatedCount];
            int count = 0;
            for (int index = 0; index < input.Length && count < estimatedCount;)
            {
                int nextIndex = AdvanceSplitIndex(input, index, unicode);
                items[count++] = nextIndex == index + 1
                    ? CharToStringFast(input[index])
                    : input.Substring(index, nextIndex - index);
                index = nextIndex;
            }

            if (count == items.Length)
            {
                return new JavaScriptRuntime.Array(items);
            }

            var trimmed = new object?[count];
            global::System.Array.Copy(items, trimmed, count);
            return new JavaScriptRuntime.Array(trimmed);
        }

        private static int AdvanceSplitIndex(string input, int index, bool unicode)
        {
            if (!unicode || index < 0 || index >= input.Length)
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

        private static JavaScriptRuntime.Array SplitWithLiteralSeparator(string input, string separator, int maxCount)
        {
            var result = new JavaScriptRuntime.Array();
            int start = 0;
            while (true)
            {
                if (result.Count >= maxCount) break;
                int idx = input.IndexOf(separator, start, StringComparison.Ordinal);
                if (idx < 0)
                {
                    result.Add(input.Substring(start));
                    break;
                }

                result.Add(input.Substring(start, idx - start));
                start = idx + separator.Length;

                if (start == input.Length && result.Count < maxCount)
                {
                    result.Add(string.Empty);
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Split using a regex pattern with flags provided by the generator.
        /// Currently only ignoreCase is observed for split behavior.
        /// </summary>
        public static JavaScriptRuntime.Array Split(string input, string pattern, bool ignoreCase)
        {
            return Split(input, pattern, ignoreCase, null);
        }

        /// <summary>
        /// Split using a string separator with optional ignoreCase and limit.
        /// Note: When called with a plain string (not a regex literal), the pattern
        /// should be treated as a literal string, not a regex pattern.
        /// </summary>
        public static JavaScriptRuntime.Array Split(string input, string pattern, bool ignoreCase, object? limit)
        {
            input ??= string.Empty;
            int maxCount = ToSplitLimit(limit);
            if (maxCount == 0) return new JavaScriptRuntime.Array();
            
            // Escape the pattern to treat it as a literal string, not a regex
            var escapedPattern = Regex.Escape(pattern);
            var options = RegexOptions.CultureInvariant;
            if (ignoreCase) options |= RegexOptions.IgnoreCase;
            var rx = new Regex(escapedPattern, options);
            return SplitWithRegex(input, rx, maxCount);
        }

        private static JavaScriptRuntime.Array SplitWithRegex(string input, Regex rx, int maxCount)
        {
            var parts = rx.Split(input);
            var arr = new JavaScriptRuntime.Array(parts.Length);
            for (int i = 0; i < parts.Length && i < maxCount; i++)
            {
                arr.Add(parts[i] ?? string.Empty);
            }
            return arr;
        }

        private static string CodePointToString(object? codePoint)
        {
            double numericCodePoint;
            try
            {
                numericCodePoint = TypeUtilities.ToNumber(codePoint);
            }
            catch
            {
                throw new RangeError("Invalid code point");
            }

            if (!double.IsFinite(numericCodePoint)
                || !double.IsInteger(numericCodePoint)
                || numericCodePoint < 0
                || numericCodePoint > 0x10FFFF)
            {
                throw new RangeError("Invalid code point");
            }

            try
            {
                return char.ConvertFromUtf32((int)numericCodePoint);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new RangeError("Invalid code point");
            }
        }

        private static string ReplaceEmptyPattern(string input, Func<string, string> replacementFactory)
        {
            var builder = new StringBuilder();
            builder.Append(replacementFactory(string.Empty));
            for (int i = 0; i < input.Length; i++)
            {
                builder.Append(input[i]);
                builder.Append(replacementFactory(string.Empty));
            }

            return builder.ToString();
        }

        internal static IJavaScriptIterator CreateIterator(string input)
        {
            return new PublicStringIterator(input);
        }

        /// <summary>
        /// Public iterator object returned by <c>String.prototype[@@iterator]</c>.
        /// Iterates JavaScript strings by Unicode code point while implementing the runtime iterator protocol.
        /// </summary>
        public sealed class PublicStringIterator : IJavaScriptIterator
        {
            private readonly string _input;
            private int _index;
            private bool _isClosed;

            public PublicStringIterator(string input)
            {
                _input = input ?? string.Empty;
                PrototypeChain.SetPrototype(this, StringIteratorPrototype);
            }

            public bool HasReturn => true;

            public IteratorResultObject Next()
            {
                return Advance();
            }

            public IteratorResultObject next()
            {
                return Advance();
            }

            public void Return()
            {
                _isClosed = true;
            }

            internal IteratorResultObject Advance()
            {
                if (_isClosed || _index >= _input.Length)
                {
                    return new IteratorResultObject(null, done: true);
                }

                int nextIndex = AdvanceSplitIndex(_input, _index, unicode: true);
                var value = _input.Substring(_index, nextIndex - _index);
                _index = nextIndex;
                return new IteratorResultObject(value, done: false);
            }
        }

        private static bool TryInvokeWellKnownSymbol(object? target, Symbol symbol, string symbolPropertyKey, string input, out object? result)
        {
            result = null;
            if (target is JavaScriptRuntime.RegExp regexp
                && regexp.TryInvokeIntrinsicWellKnownSymbol(symbol, input, out result))
            {
                return true;
            }

            if (!TryGetWellKnownSymbolCallable(target, symbol, symbolPropertyKey, out var callable))
            {
                return false;
            }

            var previousThis = RuntimeServices.SetCurrentThis(target);
            try
            {
                result = Closure.InvokeWithArgs1(callable, System.Array.Empty<object>(), input);
                return true;
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        private static bool TryInvokeWellKnownSymbol(object? target, Symbol symbol, string symbolPropertyKey, string input, object? arg1, out object? result)
        {
            result = null;
            if (target is JavaScriptRuntime.RegExp regexp
                && regexp.TryInvokeIntrinsicWellKnownSymbol(symbol, input, arg1, out result))
            {
                return true;
            }

            if (!TryGetWellKnownSymbolCallable(target, symbol, symbolPropertyKey, out var callable))
            {
                return false;
            }

            var previousThis = RuntimeServices.SetCurrentThis(target);
            try
            {
                result = Closure.InvokeWithArgs2(callable, System.Array.Empty<object>(), input, arg1);
                return true;
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        private static bool TryGetWellKnownSymbolCallable(object? target, Symbol symbol, string symbolPropertyKey, out Delegate callable)
        {
            callable = null!;
            if (target is null || target is JsNull)
            {
                return false;
            }

            // Bypass generic ToPropertyKey(symbol) conversion in this hot path while preserving
            // the same property/prototype lookup semantics as ObjectRuntime.GetItem(target, symbol).
            var symbolMethod = JavaScriptRuntime.Object.GetProperty(target, symbolPropertyKey);
            if (symbolMethod is null || symbolMethod is JsNull)
            {
                return false;
            }

            if (symbolMethod is not Delegate resolvedCallable)
            {
                throw new JavaScriptRuntime.TypeError($"{GetWellKnownSymbolName(symbol)} is not a function");
            }

            callable = resolvedCallable;
            return true;
        }

        private static string GetWellKnownSymbolName(Symbol symbol)
        {
            var symbolName = symbol.ToString();
            const string prefix = "Symbol(";
            const string suffix = ")";

            if (symbolName.StartsWith(prefix, StringComparison.Ordinal)
                && symbolName.EndsWith(suffix, StringComparison.Ordinal))
            {
                symbolName = symbolName.Substring(prefix.Length, symbolName.Length - prefix.Length - suffix.Length);
            }

            return symbolName;
        }

        private static int ToSplitLimit(object? limit)
        {
            if (limit == null) return int.MaxValue; // Omitted => effectively unlimited
            try
            {
                double d;
                switch (limit)
                {
                    case double dd: d = dd; break;
                    case float ff: d = ff; break;
                    case int ii: d = ii; break;
                    case long ll: d = ll; break;
                    case short ss: d = ss; break;
                    case byte bb: d = bb; break;
                    case string s when double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var pd):
                        d = pd; break;
                    case IConvertible conv:
                        d = conv.ToDouble(System.Globalization.CultureInfo.InvariantCulture); break;
                    default:
                        return int.MaxValue;
                }
                if (double.IsNaN(d) || d <= 0) return 0;
                if (d > int.MaxValue) return int.MaxValue;
                // Truncate toward zero
                d = d >= 0 ? global::System.Math.Floor(d) : global::System.Math.Ceiling(d);
                return (int)d;
            }
            catch { return int.MaxValue; }
        }
    }
}
