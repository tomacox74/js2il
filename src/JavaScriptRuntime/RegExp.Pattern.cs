using System.Globalization;
using System.Text;

namespace JavaScriptRuntime;

public sealed partial class RegExp
{
    private static string RewritePatternCharacters(
        string source, bool unicode, bool unicodeSets, bool ignoreCase, bool dotAll, bool multiline)
    {
        var result = new StringBuilder();
        var groups = new Stack<(bool IgnoreCase, bool DotAll, bool Multiline)>();
        for (var index = 0; index < source.Length;)
        {
            var current = source[index];
            if (current == '[')
            {
                result.Append(unicode
                    ? ReadUnicodeClass(source, ref index, unicodeSets, ignoreCase)
                    : ReadLegacyClass(source, ref index, ignoreCase));
            }
            else if (current == '(')
            {
                groups.Push((ignoreCase, dotAll, multiline));
                var end = index + 1;
                if (end < source.Length && source[end] == '?')
                {
                    end++;
                    if (end < source.Length && source[end] == '<')
                    {
                        end = source[end + 1] is '=' or '!' ? end + 2 : source.IndexOf('>', end) + 1;
                    }
                    else if (end < source.Length && source[end] is ':' or '=' or '!') end++;
                    else
                    {
                        var enabled = true;
                        while (end < source.Length && source[end] != ':')
                        {
                            switch (source[end++])
                            {
                                case '-': enabled = false; break;
                                case 'i': ignoreCase = enabled; break;
                                case 's': dotAll = enabled; break;
                                case 'm': multiline = enabled; break;
                            }
                        }
                        end++;
                    }
                }
                result.Append(source, index, end - index);
                index = end;
            }
            else if (current == ')')
            {
                result.Append(current);
                index++;
                if (groups.TryPop(out var flags))
                {
                    (ignoreCase, dotAll, multiline) = flags;
                }
            }
            else if (current == '\\')
            {
                if (unicode && TryReadUnicodeProperty(source, ref index, unicodeSets, ignoreCase, out var property))
                {
                    result.Append(property);
                    continue;
                }
                var escape = source[index + 1];
                if ("dDsSwWbB".Contains(escape))
                {
                    var set = CharacterEscapeSet(escape is 'b' or 'B' ? 'w' : char.ToLowerInvariant(escape));
                    if (ignoreCase) set.Fold(unicode);
                    var positive = unicode ? set.ToPattern(ignoreCase) : set.ToCodeUnitPattern();
                    if (escape is 'b' or 'B')
                    {
                        result.Append(escape == 'b'
                            ? "(?:(?<=" + positive + ")(?!" + positive + ")|(?<!" + positive + ")(?=" + positive + "))"
                            : "(?:(?<=" + positive + ")(?=" + positive + ")|(?<!" + positive + ")(?!" + positive + "))");
                    }
                    else
                    {
                        if (char.IsUpper(escape)) set.Complement();
                        result.Append(unicode ? set.ToPattern(ignoreCase) : set.ToCodeUnitPattern());
                    }
                    index += 2;
                }
                else if (!unicode && escape == 'u' && index + 2 < source.Length && source[index + 2] == '{')
                {
                    result.Append(ScalarPattern('u', false, ignoreCase));
                    index += 2;
                }
                else if (escape is 'u' or 'x')
                {
                    var point = (unicode ? ReadClassScalar(source, ref index, false)
                        : ReadClassCodeUnit(source, ref index, false))
                        ?? throw new SyntaxError("Invalid Unicode escape");
                    result.Append(ScalarPattern(point, unicode, ignoreCase));
                }
                else if (escape == 'c' && index + 2 < source.Length && char.IsAsciiLetter(source[index + 2]))
                {
                    AppendUnicodeEscape(result, source[index + 2] % 32);
                    index += 3;
                }
                else if (char.IsAsciiDigit(escape))
                {
                    var end = index + 2;
                    while (end < source.Length && char.IsAsciiDigit(source[end])) end++;
                    result.Append(source, index, end - index);
                    index = end;
                }
                else if (escape == 'k' && index + 2 < source.Length && source[index + 2] == '<')
                {
                    var end = source.IndexOf('>', index + 3);
                    result.Append(source, index, end - index + 1);
                    index = end + 1;
                }
                else
                {
                    result.Append(source, index, 2);
                    index += 2;
                }
            }
            else if (current == '.')
            {
                result.Append(unicode ? dotAll ? UnicodeDotAllPattern : UnicodeDotPattern
                    : dotAll ? "[\\u0000-\\uFFFF]" : DotPattern);
                index++;
            }
            else if (current == '^')
            {
                result.Append(multiline ? "(?:\\A|(?<=[\\r\\n\\u2028\\u2029]))" : "\\A");
                index++;
            }
            else if (current == '$')
            {
                result.Append(multiline ? "(?:\\z|(?=[\\r\\n\\u2028\\u2029]))" : "\\z");
                index++;
            }
            else if (current == '{' && source.IndexOf('}', index) is var end && end >= 0)
            {
                result.Append(source, index, end - index + 1);
                index = end + 1;
            }
            else if (ignoreCase && !"*+?{}|".Contains(current))
            {
                var point = (int)source[index++];
                if (unicode && char.IsHighSurrogate((char)point) && index < source.Length && char.IsLowSurrogate(source[index]))
                {
                    point = char.ConvertToUtf32((char)point, source[index++]);
                }
                result.Append(ScalarPattern(point, unicode, true));
            }
            else result.Append(source[index++]);
        }
        return result.ToString();
    }

    private static string ScalarPattern(int point, bool unicode, bool ignoreCase)
    {
        var set = new UnicodeSet();
        set.Ranges.Add((point, point));
        if (ignoreCase) set.Fold(unicode);
        return unicode ? set.ToPattern(ignoreCase) : set.ToCodeUnitPattern();
    }

    private static string ReadLegacyClass(string source, ref int index, bool ignoreCase)
    {
        index++;
        var negate = index < source.Length && source[index] == '^';
        if (negate) index++;
        var result = new UnicodeSet();
        while (index < source.Length && source[index] != ']')
        {
            var first = ReadLegacyClassOperand(source, ref index, out var firstPoint);
            if (firstPoint is not null && index + 1 < source.Length && source[index] == '-' && source[index + 1] != ']')
            {
                index++;
                var last = ReadLegacyClassOperand(source, ref index, out var lastPoint);
                if (lastPoint is not null) first.Ranges[0] = (firstPoint.Value, lastPoint.Value);
                else
                {
                    first.Ranges.Add(('-', '-'));
                    first.Combine(last, "union");
                }
            }
            result.Combine(first, "union");
        }
        index++;
        if (ignoreCase) result.Fold(false);
        if (negate) result.Complement();
        return result.ToCodeUnitPattern();
    }

    private static UnicodeSet ReadLegacyClassOperand(string source, ref int index, out int? point)
    {
        var value = source[index++];
        point = value;
        if (value == '\\' && index < source.Length)
        {
            var escape = source[index++];
            if ("dDsSwW".Contains(escape))
            {
                point = null;
                var set = CharacterEscapeSet(char.ToLowerInvariant(escape));
                if (char.IsUpper(escape)) set.Complement();
                return set;
            }
            if (escape is >= '0' and <= '7')
            {
                var number = escape - '0';
                var count = escape <= '3' ? 3 : 2;
                while (--count > 0 && index < source.Length && source[index] is >= '0' and <= '7')
                {
                    number = number * 8 + source[index++] - '0';
                }
                point = number;
            }
            else if (escape is 'x' or 'u' && HasHexDigitsAt(source, index, escape == 'u' ? 4 : 2))
            {
                var digits = escape == 'u' ? 4 : 2;
                point = int.Parse(source.AsSpan(index, digits), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                index += digits;
            }
            else if (escape == 'c' && index < source.Length
                && (char.IsAsciiLetterOrDigit(source[index]) || source[index] == '_'))
            {
                point = source[index++] % 32;
            }
            else point = escape switch
            {
                'b' => 8, 't' => 9, 'n' => 10, 'v' => 11, 'f' => 12, 'r' => 13, _ => escape
            };
        }
        var result = new UnicodeSet();
        result.Ranges.Add((point.Value, point.Value));
        return result;
    }
}
