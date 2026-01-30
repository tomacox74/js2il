using System;

namespace Js2IL.Tests;

internal static class JavaScriptTestSource
{
    public static string EnsureUseStrict(string js)
    {
        if (string.IsNullOrEmpty(js))
        {
            return "\"use strict\";\n";
        }

        return HasUseStrictDirectivePrologue(js)
            ? js
            : "\"use strict\";\n\n" + js;
    }

    private static bool HasUseStrictDirectivePrologue(string js)
    {
        var i = 0;

        // Skip BOM
        if (js.Length > 0 && js[0] == '\uFEFF')
        {
            i++;
        }

        while (true)
        {
            SkipWhitespace(ref i, js);
            if (i >= js.Length)
            {
                return false;
            }

            // Skip leading comments
            if (TrySkipLineComment(ref i, js) || TrySkipBlockComment(ref i, js))
            {
                continue;
            }

            break;
        }

        // Scan directive prologue: sequence of string-literal expression statements.
        while (true)
        {
            SkipWhitespace(ref i, js);
            if (i >= js.Length)
            {
                return false;
            }

            if (TrySkipLineComment(ref i, js) || TrySkipBlockComment(ref i, js))
            {
                continue;
            }

            var quote = js[i];
            if (quote != '\'' && quote != '"')
            {
                return false;
            }

            i++; // consume quote
            var start = i;
            while (i < js.Length)
            {
                var c = js[i];
                if (c == '\\')
                {
                    i += 2;
                    continue;
                }
                if (c == quote)
                {
                    break;
                }
                i++;
            }

            if (i >= js.Length)
            {
                return false;
            }

            var value = js.Substring(start, i - start);
            i++; // closing quote

            SkipWhitespace(ref i, js);
            if (i < js.Length && js[i] == ';')
            {
                i++;
            }

            if (string.Equals(value, "use strict", StringComparison.Ordinal))
            {
                return true;
            }

            // Continue only if next token starts another string literal statement.
            // Otherwise, directive prologue has ended.
            var checkpoint = i;
            while (true)
            {
                SkipWhitespace(ref checkpoint, js);
                if (checkpoint >= js.Length)
                {
                    return false;
                }
                if (TrySkipLineComment(ref checkpoint, js) || TrySkipBlockComment(ref checkpoint, js))
                {
                    continue;
                }
                break;
            }

            if (js[checkpoint] != '\'' && js[checkpoint] != '"')
            {
                return false;
            }

            i = checkpoint;
        }
    }

    private static void SkipWhitespace(ref int i, string s)
    {
        while (i < s.Length)
        {
            var c = s[i];
            if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
            {
                i++;
                continue;
            }
            break;
        }
    }

    private static bool TrySkipLineComment(ref int i, string s)
    {
        if (i + 1 >= s.Length || s[i] != '/' || s[i + 1] != '/')
        {
            return false;
        }

        i += 2;
        while (i < s.Length && s[i] != '\n')
        {
            i++;
        }

        return true;
    }

    private static bool TrySkipBlockComment(ref int i, string s)
    {
        if (i + 1 >= s.Length || s[i] != '/' || s[i + 1] != '*')
        {
            return false;
        }

        i += 2;
        while (i + 1 < s.Length)
        {
            if (s[i] == '*' && s[i + 1] == '/')
            {
                i += 2;
                return true;
            }
            i++;
        }

        // Unterminated comment; treat as "not present" and stop.
        i = s.Length;
        return true;
    }
}
