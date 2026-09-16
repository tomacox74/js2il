using System.Globalization;
using System.Text;

namespace JavaScriptRuntime;

internal static class UnicodeCaseConversion
{
    // Full mappings and host Unicode-data deltas are pinned to the Unicode 16.0
    // version required by ECMA-262. The CLR supplies older simple mappings.
    public static string ToLower(string value, CultureInfo culture)
    {
        // ASCII needs no full mappings or contextual rules. Keep culture-aware
        // CLR casing here: Turkic ASCII 'I' still maps to a non-ASCII character.
        if (Ascii.IsValid(value))
        {
            return value.ToLower(culture);
        }

        StringBuilder? result = null;
        var segmentStart = 0;

        for (var index = 0; index < value.Length; index++)
        {
            var sourceLength = 1;
            string? mapping;
            if (char.IsHighSurrogate(value[index])
                && index + 1 < value.Length
                && char.IsLowSurrogate(value[index + 1]))
            {
                mapping = GetUnicode16LowercaseMapping(
                    char.ConvertToUtf32(value[index], value[index + 1]));
                sourceLength = 2;
            }
            else
            {
                mapping = value[index] switch
                {
                    '\u0130' when !IsTurkic(culture) => "i\u0307",
                    '\u03A3' => IsFinalSigma(value, index) ? "\u03C2" : "\u03C3",
                    _ => GetUnicode16LowercaseMapping(value[index])
                };
            }

            if (mapping is null)
            {
                continue;
            }

            result ??= new StringBuilder(value.Length);
            AppendLowercase(result, value, segmentStart, index - segmentStart, culture);
            result.Append(mapping);
            index += sourceLength - 1;
            segmentStart = index + 1;
        }

        if (result is null)
        {
            return value.ToLower(culture);
        }

        AppendLowercase(result, value, segmentStart, value.Length - segmentStart, culture);
        return result.ToString();
    }

    public static string ToUpper(string value, CultureInfo culture)
    {
        if (Ascii.IsValid(value))
        {
            return value.ToUpper(culture);
        }

        StringBuilder? result = null;
        var segmentStart = 0;

        for (var index = 0; index < value.Length; index++)
        {
            var scalar = GetScalar(value, index);
            var mapping = GetFullUppercaseMapping(scalar);
            if (mapping is null)
            {
                continue;
            }

            result ??= new StringBuilder(value.Length);
            AppendUppercase(result, value, segmentStart, index - segmentStart, culture);
            result.Append(mapping);
            if (scalar > char.MaxValue)
            {
                index++;
            }

            segmentStart = index + 1;
        }

        if (result is null)
        {
            return value.ToUpper(culture);
        }

        AppendUppercase(result, value, segmentStart, value.Length - segmentStart, culture);
        return result.ToString();
    }

    private static bool IsFinalSigma(string value, int sigmaIndex)
    {
        var hasCasedBefore = false;
        for (var index = sigmaIndex - 1; index >= 0;)
        {
            var runeStart = index > 0
                && char.IsLowSurrogate(value[index])
                && char.IsHighSurrogate(value[index - 1])
                    ? index - 1
                    : index;
            var category = CharUnicodeInfo.GetUnicodeCategory(value, runeStart);
            if (IsCaseIgnorable(value, runeStart, category))
            {
                index = runeStart - 1;
                continue;
            }

            hasCasedBefore = IsCased(value, runeStart, category);
            break;
        }

        if (!hasCasedBefore)
        {
            return false;
        }

        for (var index = sigmaIndex + 1; index < value.Length;)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(value, index);
            if (IsCaseIgnorable(value, index, category))
            {
                index += char.IsHighSurrogate(value[index])
                    && index + 1 < value.Length
                    && char.IsLowSurrogate(value[index + 1])
                        ? 2
                        : 1;
                continue;
            }

            return !IsCased(value, index, category);
        }

        return true;
    }

    private static bool IsCased(
        string value,
        int index,
        UnicodeCategory category)
    {
        if (category is UnicodeCategory.UppercaseLetter
            or UnicodeCategory.LowercaseLetter
            or UnicodeCategory.TitlecaseLetter)
        {
            return true;
        }

        return GetScalar(value, index) switch
        {
            0x00AA or 0x00BA or 0x0345 or 0x037A or 0x10FC
                or 0x1D78 or 0x2071 or 0x207F or 0xA770 or 0xAB69
                or 0x10780 => true,
            >= 0x02B0 and <= 0x02B8 => true,
            >= 0x02C0 and <= 0x02C1 => true,
            >= 0x02E0 and <= 0x02E4 => true,
            >= 0x1D2C and <= 0x1D6A => true,
            >= 0x1D9B and <= 0x1DBF => true,
            >= 0x1C89 and <= 0x1C8A => true,
            >= 0x2090 and <= 0x209C => true,
            >= 0x2160 and <= 0x217F => true,
            >= 0x24B6 and <= 0x24E9 => true,
            >= 0x2C7C and <= 0x2C7D => true,
            >= 0xA69C and <= 0xA69D => true,
            >= 0xA7CB and <= 0xA7CD => true,
            >= 0xA7DA and <= 0xA7DC => true,
            >= 0xA7F2 and <= 0xA7F4 => true,
            >= 0xA7F8 and <= 0xA7F9 => true,
            >= 0xAB5C and <= 0xAB5F => true,
            >= 0x10783 and <= 0x10785 => true,
            >= 0x10787 and <= 0x107B0 => true,
            >= 0x107B2 and <= 0x107BA => true,
            >= 0x10D50 and <= 0x10D65 => true,
            >= 0x10D70 and <= 0x10D85 => true,
            >= 0x1E030 and <= 0x1E06D => true,
            >= 0x1F130 and <= 0x1F149 => true,
            >= 0x1F150 and <= 0x1F169 => true,
            >= 0x1F170 and <= 0x1F189 => true,
            _ => false
        };
    }

    private static bool IsCaseIgnorable(
        string value,
        int index,
        UnicodeCategory category)
    {
        if (category is UnicodeCategory.NonSpacingMark
            or UnicodeCategory.EnclosingMark
            or UnicodeCategory.Format
            or UnicodeCategory.ModifierLetter
            or UnicodeCategory.ModifierSymbol)
        {
            return true;
        }

        return GetScalar(value, index) switch
        {
            0x0027 or 0x002E or 0x003A or 0x00B7 or 0x0387 or 0x055F
                or 0x05F4 or 0x0897 or 0x2018 or 0x2019 or 0x2024
                or 0x2027 or 0xFE13 or 0xFE52 or 0xFE55 or 0xFF07
                or 0xFF0E or 0xFF1A or 0x10D4E or 0x10D6F or 0x10EFC
                or 0x113CE or 0x113D0 or 0x113D2 or 0x11F5A => true,
            >= 0x10D69 and <= 0x10D6D => true,
            >= 0x113BB and <= 0x113C0 => true,
            >= 0x113E1 and <= 0x113E2 => true,
            >= 0x1611E and <= 0x16129 => true,
            >= 0x1612D and <= 0x1612F => true,
            >= 0x16D40 and <= 0x16D42 => true,
            >= 0x16D6B and <= 0x16D6C => true,
            >= 0x1E5EE and <= 0x1E5EF => true,
            _ => false
        };
    }

    private static int GetScalar(string value, int index)
        => char.IsHighSurrogate(value[index])
            && index + 1 < value.Length
            && char.IsLowSurrogate(value[index + 1])
                ? char.ConvertToUtf32(value[index], value[index + 1])
                : value[index];

    private static bool IsTurkic(CultureInfo culture)
        => culture.TwoLetterISOLanguageName is "tr" or "az";

    private static string? GetUnicode16LowercaseMapping(int scalar)
    {
        if (scalar is >= 0x10D50 and <= 0x10D65)
        {
            return char.ConvertFromUtf32(scalar + 0x20);
        }

        return scalar switch
        {
            0x1C89 => "\u1C8A",
            0xA7CB => "\u0264",
            0xA7CC => "\uA7CD",
            0xA7DA => "\uA7DB",
            0xA7DC => "\u019B",
            _ => null
        };
    }

    private static string? GetFullUppercaseMapping(int scalar)
    {
        if (scalar is >= 0x10D70 and <= 0x10D85)
        {
            return char.ConvertFromUtf32(scalar - 0x20);
        }

        if (scalar == 0x1C8A)
        {
            return "\u1C89";
        }

        if (scalar == 0x0264)
        {
            return "\uA7CB";
        }

        if (scalar == 0xA7CD)
        {
            return "\uA7CC";
        }

        if (scalar == 0xA7DB)
        {
            return "\uA7DA";
        }

        if (scalar == 0x019B)
        {
            return "\uA7DC";
        }

        if (scalar > char.MaxValue)
        {
            return null;
        }

        var character = (char)scalar;
        if (character is >= '\u1F80' and <= '\u1F87')
        {
            return $"{(char)(character - 0x78)}\u0399";
        }

        if (character is >= '\u1F88' and <= '\u1F8F')
        {
            return $"{(char)(character - 0x80)}\u0399";
        }

        if (character is >= '\u1F90' and <= '\u1F97')
        {
            return $"{(char)(character - 0x68)}\u0399";
        }

        if (character is >= '\u1F98' and <= '\u1F9F')
        {
            return $"{(char)(character - 0x70)}\u0399";
        }

        if (character is >= '\u1FA0' and <= '\u1FA7')
        {
            return $"{(char)(character - 0x38)}\u0399";
        }

        if (character is >= '\u1FA8' and <= '\u1FAF')
        {
            return $"{(char)(character - 0x40)}\u0399";
        }

        return character switch
        {
            '\u00DF' => "\u0053\u0053",
            '\u0130' => "\u0130",
            '\u0149' => "\u02BC\u004E",
            '\u01F0' => "\u004A\u030C",
            '\u0390' => "\u0399\u0308\u0301",
            '\u03B0' => "\u03A5\u0308\u0301",
            '\u0587' => "\u0535\u0552",
            '\u1E96' => "\u0048\u0331",
            '\u1E97' => "\u0054\u0308",
            '\u1E98' => "\u0057\u030A",
            '\u1E99' => "\u0059\u030A",
            '\u1E9A' => "\u0041\u02BE",
            '\u1F50' => "\u03A5\u0313",
            '\u1F52' => "\u03A5\u0313\u0300",
            '\u1F54' => "\u03A5\u0313\u0301",
            '\u1F56' => "\u03A5\u0313\u0342",
            '\u1FB2' => "\u1FBA\u0399",
            '\u1FB3' => "\u0391\u0399",
            '\u1FB4' => "\u0386\u0399",
            '\u1FB6' => "\u0391\u0342",
            '\u1FB7' => "\u0391\u0342\u0399",
            '\u1FBC' => "\u0391\u0399",
            '\u1FC2' => "\u1FCA\u0399",
            '\u1FC3' => "\u0397\u0399",
            '\u1FC4' => "\u0389\u0399",
            '\u1FC6' => "\u0397\u0342",
            '\u1FC7' => "\u0397\u0342\u0399",
            '\u1FCC' => "\u0397\u0399",
            '\u1FD2' => "\u0399\u0308\u0300",
            '\u1FD3' => "\u0399\u0308\u0301",
            '\u1FD6' => "\u0399\u0342",
            '\u1FD7' => "\u0399\u0308\u0342",
            '\u1FE2' => "\u03A5\u0308\u0300",
            '\u1FE3' => "\u03A5\u0308\u0301",
            '\u1FE4' => "\u03A1\u0313",
            '\u1FE6' => "\u03A5\u0342",
            '\u1FE7' => "\u03A5\u0308\u0342",
            '\u1FF2' => "\u1FFA\u0399",
            '\u1FF3' => "\u03A9\u0399",
            '\u1FF4' => "\u038F\u0399",
            '\u1FF6' => "\u03A9\u0342",
            '\u1FF7' => "\u03A9\u0342\u0399",
            '\u1FFC' => "\u03A9\u0399",
            '\uFB00' => "\u0046\u0046",
            '\uFB01' => "\u0046\u0049",
            '\uFB02' => "\u0046\u004C",
            '\uFB03' => "\u0046\u0046\u0049",
            '\uFB04' => "\u0046\u0046\u004C",
            '\uFB05' => "\u0053\u0054",
            '\uFB06' => "\u0053\u0054",
            '\uFB13' => "\u0544\u0546",
            '\uFB14' => "\u0544\u0535",
            '\uFB15' => "\u0544\u053B",
            '\uFB16' => "\u054E\u0546",
            '\uFB17' => "\u0544\u053D",
            _ => null
        };
    }

    private static void AppendLowercase(
        StringBuilder result,
        string value,
        int start,
        int length,
        CultureInfo culture)
    {
        if (length > 0)
        {
            result.Append(value.Substring(start, length).ToLower(culture));
        }
    }

    private static void AppendUppercase(
        StringBuilder result,
        string value,
        int start,
        int length,
        CultureInfo culture)
    {
        if (length > 0)
        {
            result.Append(value.Substring(start, length).ToUpper(culture));
        }
    }
}
