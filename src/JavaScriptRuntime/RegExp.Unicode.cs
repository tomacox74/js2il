using System.Globalization;
using System.Text;

namespace JavaScriptRuntime;

public sealed partial class RegExp
{
    private static string RewriteUnicodeClassesAndProperties(string source, bool unicodeSets)
    {
        var builder = new StringBuilder();
        for (var index = 0; index < source.Length;)
        {
            if (source[index] == '[')
            {
                builder.Append(ReadUnicodeClass(source, ref index, unicodeSets));
            }
            else if (source[index] == '\\')
            {
                if (TryReadUnicodeProperty(source, ref index, out var property))
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
                builder.Append(source[index++]);
            }
        }
        return builder.ToString();
    }

    private static string ReadUnicodeClass(string source, ref int index, bool unicodeSets)
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
            if (TryReadUnicodeProperty(source, ref index, out var property))
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

    private static bool TryReadUnicodeProperty(string source, ref int index, out string pattern)
    {
        pattern = string.Empty;
        if (index + 3 >= source.Length || source[index] != '\\'
            || source[index + 1] is not ('p' or 'P') || source[index + 2] != '{') return false;
        var end = source.IndexOf('}', index + 3);
        if (end < 0) return false;
        var property = source[(index + 3)..end];
        if (property is "ASCII" or "Any")
        {
            pattern = property == "ASCII" ? CodePointRangePattern(0, 127) : UnicodeDotAllPattern;
            if (source[index + 1] == 'P') pattern = "(?!" + pattern + ")" + UnicodeDotAllPattern;
            index = end + 1;
            return true;
        }
        if (property is not ("Script=Han" or "Script=Hani" or "sc=Han" or "sc=Hani")) return false;
        // Unicode 17.0 Script=Han, as used by the pinned Test262 property-escape corpus.
        (int Start, int End)[] ranges =
        [
            (0x2E80, 0x2E99), (0x2E9B, 0x2EF3), (0x2F00, 0x2FD5),
            (0x3005, 0x3005), (0x3007, 0x3007), (0x3021, 0x3029), (0x3038, 0x303B),
            (0x3400, 0x4DBF), (0x4E00, 0x9FFF), (0xF900, 0xFA6D), (0xFA70, 0xFAD9),
            (0x16FE2, 0x16FE3), (0x16FF0, 0x16FF6), (0x20000, 0x2A6DF),
            (0x2A700, 0x2B81D), (0x2B820, 0x2CEAD), (0x2CEB0, 0x2EBE0),
            (0x2EBF0, 0x2EE5D), (0x2F800, 0x2FA1D), (0x30000, 0x3134A), (0x31350, 0x33479)
        ];
        pattern = "(?:" + string.Join("|", ranges.Select(static range =>
            CodePointRangePattern(range.Start, range.End))) + ")";
        if (source[index + 1] == 'P') pattern = "(?!" + pattern + ")" + UnicodeDotAllPattern;
        index = end + 1;
        return true;
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
