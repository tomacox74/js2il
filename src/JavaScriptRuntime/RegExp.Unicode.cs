using System.Globalization;
using System.Text;

namespace JavaScriptRuntime;

public sealed partial class RegExp
{
    private static string RewriteUnicodeClassesAndProperties(string source, bool unicodeSets, bool ignoreCase)
    {
        var builder = new StringBuilder();
        var groups = new Stack<bool>();
        for (var index = 0; index < source.Length;)
        {
            if (source[index] == '[')
            {
                builder.Append(ReadUnicodeClass(source, ref index, unicodeSets, ignoreCase));
            }
            else if (source[index] == '\\')
            {
                if (TryReadUnicodeProperty(source, ref index, unicodeSets, ignoreCase, out var property))
                {
                    builder.Append(property);
                }
                else
                {
                    builder.Append(source[index++]);
                    if (index < source.Length) builder.Append(source[index++]);
                }
            }
            else
            {
                if (source[index] == '(')
                {
                    groups.Push(ignoreCase);
                    if (index + 1 < source.Length && source[index + 1] == '?')
                    {
                        var enabled = true;
                        var modified = ignoreCase;
                        var modifier = index + 2;
                        for (; modifier < source.Length && source[modifier] is 'i' or 'm' or 's' or '-'; modifier++)
                        {
                            if (source[modifier] == '-') enabled = false;
                            if (source[modifier] == 'i') modified = enabled;
                        }
                        if (modifier < source.Length && source[modifier] == ':') ignoreCase = modified;
                    }
                }
                else if (source[index] == ')' && groups.Count > 0)
                {
                    ignoreCase = groups.Pop();
                }
                builder.Append(source[index++]);
            }
        }
        return builder.ToString();
    }

    private static string ReadUnicodeClass(string source, ref int index, bool unicodeSets, bool ignoreCase)
    {
        index++;
        var negate = index < source.Length && source[index] == '^';
        if (negate) index++;
        var alternatives = new List<string>();
        while (index < source.Length && source[index] != ']')
        {
            if (unicodeSets && (source[index] == '['
                || source.AsSpan(index).StartsWith("&&")
                || source.AsSpan(index).StartsWith("--")
                || source.AsSpan(index).StartsWith(@"\q{")))
            {
                throw new SyntaxError("Nested Unicode sets, set operations, and string properties are not supported yet");
            }
            if (unicodeSets && (source[index] is '(' or ')' or '{' or '}' or '/' or '|'
                || index + 1 < source.Length && source[index] == source[index + 1]
                    && "!#$%&*+,.:;<=>?@^`~".Contains(source[index])))
            {
                throw new SyntaxError("Invalid character in a Unicode set");
            }
            if (TryReadUnicodeProperty(source, ref index, unicodeSets, ignoreCase, out var property))
            {
                alternatives.Add(property);
                continue;
            }
            var start = index;
            var first = ReadClassScalar(source, ref index, unicodeSets);
            if (first is null)
            {
                var token = source[start..index];
                var positive = token switch
                {
                    @"\d" or @"\D" => "[0-9]",
                    @"\w" or @"\W" => "[A-Za-z0-9_]",
                    @"\s" or @"\S" => "[\\u0009-\\u000D\\u0020\\u00A0\\u1680\\u2000-\\u200A\\u2028\\u2029\\u202F\\u205F\\u3000\\uFEFF]",
                    _ => "[" + token + "]"
                };
                alternatives.Add(token is @"\D" or @"\W" or @"\S"
                    ? "(?!" + positive + ")" + UnicodeDotAllPattern
                    : positive);
                continue;
            }
            if (index + 1 < source.Length && source[index] == '-' && source[index + 1] != ']')
            {
                index++;
                var last = ReadClassScalar(source, ref index, unicodeSets)
                    ?? throw new SyntaxError("Invalid Unicode character class range");
                if (last < first) throw new SyntaxError("Character class range is out of order");
                alternatives.Add(CodePointRangePattern(first.Value, last));
            }
            else
            {
                if (unicodeSets && source[start] == '-')
                {
                    throw new SyntaxError("A literal hyphen must be escaped in a Unicode set");
                }
                alternatives.Add(CodePointRangePattern(first.Value, first.Value));
            }
        }
        if (index >= source.Length) throw new SyntaxError("Unterminated character class");
        index++;
        var union = alternatives.Count == 0 ? "(?!)" : "(?:" + string.Join("|", alternatives) + ")";
        return negate ? "(?!" + union + ")" + UnicodeDotAllPattern : union;
    }

    private static int? ReadClassScalar(string source, ref int index, bool unicodeSets)
    {
        var first = ReadClassCodeUnit(source, ref index, unicodeSets);
        if (first is >= 0xD800 and <= 0xDBFF && index < source.Length)
        {
            var next = index;
            var second = ReadClassCodeUnit(source, ref next, unicodeSets);
            if (second is >= 0xDC00 and <= 0xDFFF)
            {
                index = next;
                return 0x10000 + ((first.Value - 0xD800) << 10) + second.Value - 0xDC00;
            }
        }
        return first;
    }

    private static int? ReadClassCodeUnit(string source, ref int index, bool unicodeSets)
    {
        if (index >= source.Length) throw new SyntaxError("Unterminated character class");
        var value = source[index++];
        if (value != '\\') return value;
        if (index >= source.Length) throw new SyntaxError("Invalid escape");
        var escape = source[index++];
        if (escape is 'u' or 'x')
        {
            if (escape == 'u' && index < source.Length && source[index] == '{')
            {
                var end = source.IndexOf('}', ++index);
                if (end < 0 || !int.TryParse(source.AsSpan(index, end - index),
                    NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var scalar)
                    || scalar is < 0 or > 0x10FFFF)
                {
                    throw new SyntaxError("Invalid Unicode escape");
                }
                index = end + 1;
                return scalar;
            }
            var digits = escape == 'u' ? 4 : 2;
            if (index + digits > source.Length
                || !int.TryParse(source.AsSpan(index, digits), NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture, out var codePoint))
            {
                throw new SyntaxError("Invalid Unicode escape");
            }
            index += digits;
            return codePoint;
        }
        if (escape is 'd' or 'D' or 'w' or 'W' or 's' or 'S') return null;
        if (escape is 'p' or 'P')
        {
            if (index >= source.Length || source[index] != '{') throw new SyntaxError("Invalid property escape");
            var end = source.IndexOf('}', index);
            if (end < 0) throw new SyntaxError("Invalid property escape");
            index = end + 1;
            return null;
        }
        if (escape == 'c' && index < source.Length && char.IsAsciiLetter(source[index]))
        {
            return char.ToUpperInvariant(source[index++]) % 32;
        }
        return escape switch
        {
            'b' => 8, 't' => 9, 'n' => 10, 'v' => 11, 'f' => 12, 'r' => 13,
            '0' when index >= source.Length || !char.IsAsciiDigit(source[index]) => 0,
            _ when "^$\\.*+?()[]{}|/-".Contains(escape)
                || unicodeSets && "!#%&,:;<=>@`~".Contains(escape) => escape,
            _ => throw new SyntaxError("Invalid escape in a Unicode character class")
        };
    }

    private static bool TryReadUnicodeProperty(
        string source, ref int index, bool unicodeSets, bool ignoreCase, out string pattern)
    {
        pattern = string.Empty;
        if (index + 3 >= source.Length || source[index] != '\\'
            || source[index + 1] is not ('p' or 'P') || source[index + 2] != '{') return false;
        var end = source.IndexOf('}', index + 3);
        if (end < 0) return false;
        var property = source[(index + 3)..end];
        if (property is "ASCII" or "Any")
        {
            var negate = source[index + 1] == 'P';
            var invertRanges = negate && !unicodeSets;
            var additions = ignoreCase ? RegExpUnicodeScripts.GetAsciiCaseFoldAdditions(invertRanges) : default;
            pattern = property == "ASCII"
                ? CodePointRangesPattern([0, 127], invertRanges, additions)
                : invertRanges ? "(?!)" : UnicodeDotAllPattern;
            if (ignoreCase) pattern = "(?-i:" + pattern + ")";
            if (negate && unicodeSets) pattern = "(?!" + pattern + ")" + UnicodeDotAllPattern;
            index = end + 1;
            return true;
        }
        var separator = property.IndexOf('=');
        if (separator < 0) return false;
        var propertyName = property[..separator];
        if (propertyName is not ("Script" or "sc" or "Script_Extensions" or "scx")) return false;
        var script = RegExpUnicodeScripts.ResolveScript(property[(separator + 1)..]);
        if (script < 0) throw new SyntaxError("Invalid Unicode script property escape");
        var extensions = propertyName is "Script_Extensions" or "scx";
        var ranges = RegExpUnicodeScripts.GetRanges(script, extensions);
        var complement = source[index + 1] == 'P';
        // u complements before case folding; v complements the case-folded set.
        var invert = complement && !unicodeSets;
        var folded = ignoreCase ? RegExpUnicodeScripts.GetCaseFoldAdditions(script, extensions, invert) : default;
        pattern = CodePointRangesPattern(ranges, invert, folded);
        if (ignoreCase) pattern = "(?-i:" + pattern + ")";
        if (complement && unicodeSets) pattern = "(?!" + pattern + ")" + UnicodeDotAllPattern;
        index = end + 1;
        return true;
    }

    private static string CodePointRangesPattern(
        ReadOnlySpan<int> ranges, bool complement, ReadOnlySpan<int> additions)
    {
        var intervals = new List<(int Start, int End)>();
        void AddInterval(int start, int end)
        {
            if (start <= end) intervals.Add((start, end));
        }

        var next = 0;
        for (var range = 0; range < ranges.Length; range += 2)
        {
            AddInterval(complement ? next : ranges[range], complement ? ranges[range] - 1 : ranges[range + 1]);
            next = ranges[range + 1] + 1;
        }
        if (complement) AddInterval(next, 0x10FFFF);
        foreach (var point in additions) AddInterval(point, point);
        var builder = new StringBuilder("(?:");
        // Each alternative consumes one complete code point; try dense ranges before fragmented gaps.
        foreach (var interval in intervals.OrderByDescending(static interval => interval.End - interval.Start))
        {
            if (builder.Length > 3) builder.Append('|');
            builder.Append(CodePointRangePattern(interval.Start, interval.End));
        }
        return builder.Length == 3 ? "(?!)" : builder.Append(')').ToString();
    }

    private static string CodePointRangePattern(int start, int end)
    {
        var alternatives = new List<string>();
        static string UnitRange(int first, int last)
            => first == last ? $"\\u{first:X4}" : $"[\\u{first:X4}-\\u{last:X4}]";
        void AddBmp(int first, int last, string prefix = "", string suffix = "")
        {
            var low = System.Math.Max(start, first);
            var high = System.Math.Min(end, last);
            if (low <= high) alternatives.Add(prefix + UnitRange(low, high) + suffix);
        }
        AddBmp(0, 0xD7FF);
        AddBmp(0xD800, 0xDBFF, suffix: "(?![\\uDC00-\\uDFFF])");
        AddBmp(0xDC00, 0xDFFF, prefix: "(?<![\\uD800-\\uDBFF])");
        AddBmp(0xE000, 0xFFFF);
        if (end >= 0x10000)
        {
            var first = System.Math.Max(start, 0x10000) - 0x10000;
            var last = end - 0x10000;
            var firstHigh = 0xD800 + (first >> 10);
            var lastHigh = 0xD800 + (last >> 10);
            var firstLow = 0xDC00 + (first & 1023);
            var lastLow = 0xDC00 + (last & 1023);
            if (firstHigh == lastHigh)
            {
                alternatives.Add(UnitRange(firstHigh, firstHigh) + UnitRange(firstLow, lastLow));
            }
            else
            {
                alternatives.Add(UnitRange(firstHigh, firstHigh) + UnitRange(firstLow, 0xDFFF));
                if (firstHigh + 1 <= lastHigh - 1)
                {
                    alternatives.Add(UnitRange(firstHigh + 1, lastHigh - 1) + "[\\uDC00-\\uDFFF]");
                }
                alternatives.Add(UnitRange(lastHigh, lastHigh) + UnitRange(0xDC00, lastLow));
            }
        }
        return "(?:" + string.Join("|", alternatives) + ")";
    }
}
