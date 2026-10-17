using System.Globalization;
using System.Numerics;
using System.Text;

namespace JavaScriptRuntime;

public sealed partial class RegExp
{
    private sealed class RepetitionRewriter(string pattern)
    {
        private int _index;
        private int _progress;

        internal string Rewrite() => ReadAlternatives(false).Pattern;

        private (string Pattern, bool Nullable) ReadAlternatives(bool reverse)
        {
            var result = new StringBuilder();
            var nullable = false;
            var sequenceNullable = true;
            while (_index < pattern.Length && pattern[_index] != ')')
            {
                if (pattern[_index] == '|')
                {
                    nullable |= sequenceNullable;
                    sequenceNullable = true;
                    result.Append(pattern[_index++]);
                    continue;
                }
                var atom = ReadAtom(reverse);
                var minimum = BigInteger.One;
                BigInteger? maximum = BigInteger.One;
                var hasQuantifier = true;
                switch (_index < pattern.Length ? pattern[_index] : '\0')
                {
                    case '*': minimum = 0; maximum = null; _index++; break;
                    case '+': maximum = null; _index++; break;
                    case '?': minimum = 0; _index++; break;
                    case '{':
                        var end = pattern.IndexOf('}', _index);
                        var fields = end >= 0 ? pattern[(_index + 1)..end].Split(',') : [];
                        if (fields.Length is >= 1 and <= 2
                            && BigInteger.TryParse(fields[0], NumberStyles.None, CultureInfo.InvariantCulture, out minimum)
                            && (fields.Length == 1 || fields[1].Length == 0
                                || BigInteger.TryParse(fields[1], NumberStyles.None, CultureInfo.InvariantCulture, out _)))
                        {
                            maximum = fields.Length == 1 ? minimum : fields[1].Length == 0 ? null
                                : BigInteger.Parse(fields[1], CultureInfo.InvariantCulture);
                            _index = end + 1;
                        }
                        else hasQuantifier = false;
                        break;
                    default: hasQuantifier = false; break;
                }
                if (!hasQuantifier) minimum = BigInteger.One;
                var lazy = hasQuantifier && _index < pattern.Length && pattern[_index] == '?';
                if (lazy) _index++;
                if (hasQuantifier)
                {
                    if (atom.Nullable && !atom.Balancing && (maximum is null || maximum > minimum))
                    {
                        // A captured suffix/prefix identifies the iteration's exact input position,
                        // so an optional empty iteration fails and backtracks inside its body.
                        var name = "__jrocProgress" + ++_progress;
                        var start = reverse
                            ? "(?<=(?<" + name + ">\\A[\\u0000-\\uFFFF]*))"
                            : "(?=(?<" + name + ">[\\u0000-\\uFFFF]*\\z))";
                        var end = reverse ? "(?<!\\A\\k<" + name + ">)" : "(?!\\k<" + name + ">\\z)";
                        var guarded = reverse ? "(?:" + end + atom.Pattern + start + ")"
                            : "(?:" + start + atom.Pattern + end + ")";
                        result.Append(RepeatExactly(atom.Pattern, minimum));
                        result.Append(RepeatOptional(guarded, maximum - minimum, lazy));
                    }
                    else if (maximum > int.MaxValue || minimum > int.MaxValue)
                    {
                        result.Append(RepeatExactly(atom.Pattern, minimum));
                        result.Append(RepeatOptional(atom.Pattern, maximum - minimum, lazy));
                    }
                    else
                    {
                        result.Append(atom.Pattern).Append(Quantifier(minimum, maximum, lazy));
                    }
                }
                else result.Append(atom.Pattern);
                sequenceNullable &= minimum.IsZero || atom.Nullable;
            }
            return (result.ToString(), nullable || sequenceNullable);
        }

        private (string Pattern, bool Nullable, bool Balancing) ReadAtom(bool reverse)
        {
            var start = _index;
            var value = pattern[_index++];
            if (value == '(')
            {
                var assertion = false;
                var balancing = false;
                var bodyReverse = reverse;
                if (_index < pattern.Length && pattern[_index] == '?')
                {
                    _index++;
                    if (pattern[_index] == '<')
                    {
                        assertion = pattern[_index + 1] is '=' or '!';
                        balancing = pattern[_index + 1] == '-';
                        if (assertion)
                        {
                            _index += 2;
                            bodyReverse = true;
                        }
                        else _index = pattern.IndexOf('>', _index) + 1;
                    }
                    else if (pattern[_index] is '=' or '!')
                    {
                        assertion = true;
                        bodyReverse = false;
                        _index++;
                    }
                    else if (pattern[_index] == '(')
                    {
                        _index = pattern.IndexOf(')', _index) + 1;
                    }
                    else if (pattern[_index] == '>') _index++;
                    else
                    {
                        while (pattern[_index++] != ':') { }
                    }
                }
                var header = pattern[start.._index];
                var body = ReadAlternatives(bodyReverse);
                _index++;
                return (header + body.Pattern + ")", assertion || body.Nullable, balancing);
            }
            if (value == '[')
            {
                while (_index < pattern.Length)
                {
                    if (pattern[_index++] == '\\') _index++;
                    else if (pattern[_index - 1] == ']') break;
                }
                return (pattern[start.._index], false, false);
            }
            if (value == '\\')
            {
                var escape = pattern[_index++];
                if (escape == 'k' && _index < pattern.Length && pattern[_index] == '<')
                {
                    _index = pattern.IndexOf('>', _index) + 1;
                }
                else if (escape is 'u' or 'x') _index += escape == 'u' ? 4 : 2;
                else if (escape == 'c') _index++;
                else if (char.IsAsciiDigit(escape))
                {
                    while (_index < pattern.Length && char.IsAsciiDigit(pattern[_index])) _index++;
                }
                return (pattern[start.._index], "AbBGzk".Contains(escape), false);
            }
            return (pattern[start.._index], value is '^' or '$', false);
        }

        private static string Quantifier(BigInteger minimum, BigInteger? maximum, bool lazy)
            => "{" + minimum + (maximum == minimum ? "" : "," + maximum) + "}" + (lazy ? "?" : "");

        private static string RepeatExactly(string atom, BigInteger count)
        {
            if (count <= int.MaxValue) return "(?:" + atom + "){" + count + "}";
            var quotient = BigInteger.DivRem(count, int.MaxValue, out var remainder);
            return RepeatExactly("(?:" + atom + "){" + int.MaxValue + "}", quotient)
                + (remainder.IsZero ? "" : RepeatExactly(atom, remainder));
        }

        private static string RepeatOptional(string atom, BigInteger? count, bool lazy)
        {
            if (count is null || count <= int.MaxValue) return "(?:" + atom + ")" + Quantifier(0, count, lazy);
            var quotient = BigInteger.DivRem(count.Value, int.MaxValue, out var remainder);
            var chunk = "(?:" + atom + "){0," + int.MaxValue + "}" + (lazy ? "?" : "");
            return RepeatOptional(chunk, quotient, lazy)
                + (remainder.IsZero ? "" : RepeatOptional(atom, remainder, lazy));
        }
    }
}
