using System.Text;

namespace JavaScriptRuntime;

public sealed partial class RegExp
{
    private sealed class UnicodeSet
    {
        internal List<(int Start, int End)> Ranges { get; private set; } = new();
        internal HashSet<string> Strings { get; } = new(StringComparer.Ordinal);
        internal bool MayContainStrings { get; set; }

        internal void Normalize()
        {
            Ranges.Sort(static (left, right) => left.Start.CompareTo(right.Start));
            var merged = new List<(int Start, int End)>();
            foreach (var range in Ranges)
            {
                if (merged.Count > 0 && range.Start <= merged[^1].End + 1)
                {
                    merged[^1] = (merged[^1].Start, System.Math.Max(merged[^1].End, range.End));
                }
                else merged.Add(range);
            }
            Ranges = merged;
        }

        internal bool Contains(int point)
        {
            var low = 0;
            var high = Ranges.Count - 1;
            while (low <= high)
            {
                var middle = low + (high - low) / 2;
                var range = Ranges[middle];
                if (point < range.Start) high = middle - 1;
                else if (point > range.End) low = middle + 1;
                else return true;
            }
            return false;
        }

        internal void Fold(bool unicode = true)
        {
            Normalize();
            var mappings = unicode ? RegExpUnicodeProperties.GetSimpleCaseFoldMappings()
                : RegExpUnicodeProperties.GetLegacyCaseFoldMappings();
            var additions = new List<(int, int)>();
            for (var index = 0; index < mappings.Length; index += 2)
            {
                if (Contains(mappings[index])) additions.Add((mappings[index + 1], mappings[index + 1]));
            }
            Ranges.AddRange(additions);
            Normalize();
            additions.Clear();
            for (var index = 0; index < mappings.Length; index += 2)
            {
                if (Contains(mappings[index + 1])) additions.Add((mappings[index], mappings[index]));
            }
            Ranges.AddRange(additions);
            Normalize();
        }

        internal void Combine(UnicodeSet other, string operation)
        {
            Normalize();
            other.Normalize();
            if (operation == "union")
            {
                Ranges.AddRange(other.Ranges);
                Strings.UnionWith(other.Strings);
                MayContainStrings |= other.MayContainStrings;
            }
            else if (operation == "&&")
            {
                var intersection = new List<(int, int)>();
                var right = 0;
                foreach (var range in Ranges)
                {
                    while (right < other.Ranges.Count && other.Ranges[right].End < range.Start) right++;
                    for (var index = right; index < other.Ranges.Count && other.Ranges[index].Start <= range.End; index++)
                    {
                        intersection.Add((System.Math.Max(range.Start, other.Ranges[index].Start),
                            System.Math.Min(range.End, other.Ranges[index].End)));
                    }
                }
                Ranges = intersection;
                Strings.IntersectWith(other.Strings);
                MayContainStrings &= other.MayContainStrings;
            }
            else
            {
                var difference = new List<(int, int)>();
                var right = 0;
                foreach (var range in Ranges)
                {
                    var next = range.Start;
                    while (right < other.Ranges.Count && other.Ranges[right].End < next) right++;
                    for (var index = right; index < other.Ranges.Count && other.Ranges[index].Start <= range.End; index++)
                    {
                        var excluded = other.Ranges[index];
                        if (next < excluded.Start) difference.Add((next, excluded.Start - 1));
                        next = System.Math.Max(next, excluded.End + 1);
                    }
                    if (next <= range.End) difference.Add((next, range.End));
                }
                Ranges = difference;
                Strings.ExceptWith(other.Strings);
            }
            Normalize();
        }

        internal void Complement()
        {
            if (MayContainStrings) throw new SyntaxError("Cannot complement a Unicode set containing strings");
            Normalize();
            var result = new List<(int, int)>();
            var next = 0;
            foreach (var range in Ranges)
            {
                if (next < range.Start) result.Add((next, range.Start - 1));
                next = range.End + 1;
            }
            if (next <= 0x10FFFF) result.Add((next, 0x10FFFF));
            Ranges = result;
        }

        internal void AddString(string value, bool ignoreCase)
        {
            var normalized = new StringBuilder();
            var points = 0;
            var first = -1;
            for (var index = 0; index < value.Length; index++)
            {
                var point = (int)value[index];
                if (char.IsHighSurrogate(value[index]) && index + 1 < value.Length && char.IsLowSurrogate(value[index + 1]))
                {
                    point = char.ConvertToUtf32(value[index], value[++index]);
                }
                if (ignoreCase) point = CanonicalizeUnicodePoint(point);
                first = point;
                points++;
                AppendUnicodePoint(normalized, point);
            }
            if (points == 1) Ranges.Add((first, first));
            else
            {
                MayContainStrings = true;
                Strings.Add(normalized.ToString());
            }
        }

        internal string ToPattern(bool ignoreCase)
        {
            Normalize();
            var alternatives = new List<string>();
            foreach (var value in Strings.OrderByDescending(static value => value.Length))
            {
                var builder = new StringBuilder();
                for (var index = 0; index < value.Length; index++)
                {
                    var point = (int)value[index];
                    if (char.IsHighSurrogate(value[index]) && index + 1 < value.Length && char.IsLowSurrogate(value[index + 1]))
                    {
                        point = char.ConvertToUtf32(value[index], value[++index]);
                    }
                    var scalar = new UnicodeSet();
                    scalar.Ranges.Add((point, point));
                    if (ignoreCase) scalar.Fold();
                    builder.Append(CodePointRangesPattern(scalar.Flatten(), false, default));
                }
                alternatives.Add(builder.ToString());
            }
            if (Ranges.Count > 0) alternatives.Add(CodePointRangesPattern(Flatten(), false, default));
            return alternatives.Count == 0 ? "(?!)" : "(?-i:(?:" + string.Join("|", alternatives) + "))";
        }

        internal string ToCodeUnitPattern()
        {
            Normalize();
            var builder = new StringBuilder("(?-i:[");
            foreach (var (start, end) in Ranges)
            {
                if (start > 0xFFFF) break;
                builder.Append("\\u").Append(start.ToString("X4"));
                if (end != start) builder.Append("-\\u").Append(System.Math.Min(end, 0xFFFF).ToString("X4"));
            }
            return builder.Length == 6 ? "(?!)" : builder.Append("])").ToString();
        }

        private int[] Flatten()
        {
            var result = new int[Ranges.Count * 2];
            for (var index = 0; index < Ranges.Count; index++)
            {
                result[index * 2] = Ranges[index].Start;
                result[index * 2 + 1] = Ranges[index].End;
            }
            return result;
        }
    }

    private static void AppendUnicodePoint(StringBuilder builder, int point)
    {
        if (point <= 0xFFFF) builder.Append((char)point);
        else builder.Append(char.ConvertFromUtf32(point));
    }

    private static int CanonicalizeUnicodePoint(int point)
    {
        var mappings = RegExpUnicodeProperties.GetSimpleCaseFoldMappings();
        var low = 0;
        var high = mappings.Length / 2 - 1;
        while (low <= high)
        {
            var middle = low + (high - low) / 2;
            var source = mappings[middle * 2];
            if (point < source) high = middle - 1;
            else if (point > source) low = middle + 1;
            else return mappings[middle * 2 + 1];
        }
        return point;
    }

    private static UnicodeSet ReadUnicodeSet(string source, ref int index, bool ignoreCase)
    {
        index++;
        var negate = index < source.Length && source[index] == '^';
        if (negate) index++;
        var result = new UnicodeSet();
        var operation = "union";
        var first = true;
        while (index < source.Length && source[index] != ']')
        {
            if (!first && (source.AsSpan(index).StartsWith("&&") || source.AsSpan(index).StartsWith("--")))
            {
                operation = source.Substring(index, 2);
                index += 2;
            }
            var operand = ReadUnicodeSetOperand(source, ref index, ignoreCase);
            if (first)
            {
                result = operand;
                first = false;
            }
            else result.Combine(operand, operation);
        }
        if (index >= source.Length) throw new SyntaxError("Unterminated Unicode set");
        index++;
        if (negate) result.Complement();
        return result;
    }

    private static UnicodeSet ReadUnicodeSetOperand(string source, ref int index, bool ignoreCase)
    {
        if (source[index] == '[') return ReadUnicodeSet(source, ref index, ignoreCase);
        if (source.AsSpan(index).StartsWith(@"\q{"))
        {
            index += 3;
            var result = new UnicodeSet();
            var value = new StringBuilder();
            while (index < source.Length)
            {
                if (source[index] is '|' or '}')
                {
                    result.AddString(value.ToString(), ignoreCase);
                    value.Clear();
                    if (source[index++] == '}')
                    {
                        if (ignoreCase) result.Fold();
                        return result;
                    }
                }
                else
                {
                    var point = ReadClassScalar(source, ref index, true)
                        ?? throw new SyntaxError("Invalid Unicode set string escape");
                    AppendUnicodePoint(value, point);
                }
            }
            throw new SyntaxError("Unterminated Unicode set string");
        }
        if (TryReadUnicodePropertySet(source, ref index, ignoreCase, out var property)) return property;
        if (index + 1 < source.Length && source[index] == '\\' && "dDsSwW".Contains(source[index + 1]))
        {
            var escape = source[index + 1];
            index += 2;
            var set = CharacterEscapeSet(char.ToLowerInvariant(escape));
            if (ignoreCase) set.Fold();
            if (char.IsUpper(escape)) set.Complement();
            return set;
        }
        var first = ReadClassScalar(source, ref index, true)
            ?? throw new SyntaxError("Invalid Unicode set character");
        var last = first;
        if (index < source.Length && source[index] == '-' && !source.AsSpan(index).StartsWith("--"))
        {
            index++;
            last = ReadClassScalar(source, ref index, true)
                ?? throw new SyntaxError("Invalid Unicode set range");
        }
        if (last < first) throw new SyntaxError("Character class range is out of order");
        var scalar = new UnicodeSet();
        scalar.Ranges.Add((first, last));
        if (ignoreCase) scalar.Fold();
        return scalar;
    }

    private static UnicodeSet CharacterEscapeSet(char escape)
    {
        var result = new UnicodeSet();
        if (escape == 'd') result.Ranges.Add(('0', '9'));
        else if (escape == 'w')
        {
            result.Ranges.AddRange([('A', 'Z'), ('a', 'z'), ('0', '9'), ('_', '_')]);
        }
        else
        {
            result.Ranges.AddRange([(9, 13), (0x20, 0x20), (0xA0, 0xA0), (0x1680, 0x1680),
                (0x2000, 0x200A), (0x2028, 0x2029), (0x202F, 0x202F), (0x205F, 0x205F),
                (0x3000, 0x3000), (0xFEFF, 0xFEFF)]);
        }
        result.Normalize();
        return result;
    }

    private static bool TryReadUnicodePropertySet(
        string source, ref int index, bool ignoreCase, out UnicodeSet result)
    {
        result = new UnicodeSet();
        if (index + 3 >= source.Length || source[index] != '\\'
            || source[index + 1] is not ('p' or 'P') || source[index + 2] != '{') return false;
        var end = source.IndexOf('}', index + 3);
        if (end < 0) throw new SyntaxError("Invalid Unicode property escape");
        var property = source[(index + 3)..end];
        var separator = property.IndexOf('=');
        ReadOnlySpan<int> ranges;
        if (separator < 0 && RegExpUnicodeProperties.GetStrings(property) is { } strings)
        {
            result.MayContainStrings = true;
            foreach (var value in strings.Split('\n')) result.AddString(value, ignoreCase);
            ranges = default;
        }
        else if (separator >= 0 && property[..separator] is "Script" or "sc" or "Script_Extensions" or "scx")
        {
            var script = RegExpUnicodeScripts.ResolveScript(property[(separator + 1)..]);
            if (script < 0) throw new SyntaxError("Invalid Unicode script property escape");
            ranges = RegExpUnicodeScripts.GetRanges(script, property[..separator] is "Script_Extensions" or "scx");
        }
        else
        {
            var id = separator < 0
                ? RegExpUnicodeProperties.ResolveBinaryProperty(property)
                : -1;
            if (id < 0) id = RegExpUnicodeProperties.ResolveGeneralCategory(
                separator < 0 ? property : property[(separator + 1)..]);
            if (id < 0) throw new SyntaxError("Invalid Unicode property escape");
            ranges = RegExpUnicodeProperties.GetRanges(id);
        }
        for (var offset = 0; offset < ranges.Length; offset += 2)
        {
            result.Ranges.Add((ranges[offset], ranges[offset + 1]));
        }
        if (ignoreCase) result.Fold();
        else result.Normalize();
        if (source[index + 1] == 'P') result.Complement();
        index = end + 1;
        return true;
    }
}
