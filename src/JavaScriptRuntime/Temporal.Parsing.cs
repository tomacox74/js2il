using System.Globalization;
using System.Text.RegularExpressions;

namespace JavaScriptRuntime;

public static partial class Temporal
{
    private const RegexOptions TemporalRegexOptions = RegexOptions.CultureInvariant;

    [GeneratedRegex(
        @"\A(?:(?<date>(?:[+-][0-9]{6}|[0-9]{4})(?:-[0-9]{2}-[0-9]{2}|[0-9]{4}))[Tt ]|(?<prefix>[Tt]))?(?<clock>[0-9]{2}(?::[0-9]{2}(?::[0-9]{2})?|[0-9]{2}(?:[0-9]{2})?)?)(?<fraction>[.,][0-9]{1,9})?(?<offset>[Zz]|[+-][0-9]{2}(?::[0-9]{2}(?::[0-9]{2}(?:[.,][0-9]{1,9})?)?|[0-9]{2}(?:[0-9]{2}(?:[.,][0-9]{1,9})?)?)?)?\z",
        TemporalRegexOptions)]
    private static partial Regex TimeStringPattern();

    [GeneratedRegex(
        @"\A(?<year>[+-][0-9]{6}|[0-9]{4})(?:-(?<month>[0-9]{2})-(?<day>[0-9]{2})|(?<month>[0-9]{2})(?<day>[0-9]{2}))\z",
        TemporalRegexOptions)]
    private static partial Regex DateStringPattern();

    [GeneratedRegex(
        @"\A(?:[a-z_][a-z0-9_-]*=[a-zA-Z0-9]+(?:-[a-zA-Z0-9]+)*|[a-zA-Z._][a-zA-Z0-9._+-]*(?:/[a-zA-Z._][a-zA-Z0-9._+-]*)*|[+-][0-9]{2}(?::?[0-9]{2})?)\z",
        TemporalRegexOptions)]
    private static partial Regex AnnotationPattern();

    private readonly record struct ISODate(int Year, int Month, int Day);
    private readonly record struct ISOAnnotations(string? Calendar, string? TimeZone);
    private readonly record struct ISODateTime(
        ISODate? Date, string Clock, string Fraction, string Offset, string Core,
        bool HasTimePrefix, ISOAnnotations Annotations);

    private static ISODateTime ParseISODateTime(string text, bool allowDateOnly = false)
    {
        var annotationStart = text.IndexOf('[');
        var core = annotationStart < 0 ? text : text[..annotationStart];
        var annotations = annotationStart < 0 ? default : ParseISOAnnotations(text[annotationStart..]);
        if (allowDateOnly && DateStringPattern().IsMatch(core))
        {
            return new ISODateTime(ParseISODate(core), "", "", "", core, false, annotations);
        }
        var match = TimeStringPattern().Match(core);
        if (!match.Success)
        {
            throw new RangeError("Invalid Temporal time string");
        }
        var dateText = match.Groups["date"].Value;
        ISODate? date = dateText.Length == 0 ? null : ParseISODate(dateText);
        var clock = match.Groups["clock"].Value.Replace(":", "", StringComparison.Ordinal);
        var fraction = match.Groups["fraction"].Value;
        if (fraction.Length != 0 && clock.Length != 6)
        {
            throw new RangeError("Temporal time fractions require seconds");
        }
        return new ISODateTime(date, clock, fraction, match.Groups["offset"].Value, core,
            match.Groups["prefix"].Length != 0, annotations);
    }

    private static ISODate ParseISODate(string date)
    {
        var match = DateStringPattern().Match(date);
        if (!match.Success || match.Groups["year"].Value == "-000000")
        {
            throw new RangeError("Invalid date in Temporal time string");
        }
        var year = int.Parse(match.Groups["year"].Value, CultureInfo.InvariantCulture);
        var month = int.Parse(match.Groups["month"].Value, CultureInfo.InvariantCulture);
        var day = int.Parse(match.Groups["day"].Value, CultureInfo.InvariantCulture);
        if (!IsValidMonthDay(year, month, day))
        {
            throw new RangeError("Invalid date in Temporal time string");
        }
        return new ISODate(year, month, day);
    }

    private static bool IsValidMonthDay(int year, int month, int day)
    {
        var maximum = month switch
        {
            2 => year % 4 == 0 && (year % 100 != 0 || year % 400 == 0) ? 29 : 28,
            4 or 6 or 9 or 11 => 30,
            >= 1 and <= 12 => 31,
            _ => 0
        };
        return day >= 1 && day <= maximum;
    }

    private static void ValidateTimeOffset(string offset)
    {
        var digits = offset[1..].Replace(":", "", StringComparison.Ordinal);
        var fraction = digits.IndexOfAny(['.', ',']);
        if (fraction >= 0)
        {
            digits = digits[..fraction];
        }
        if (int.Parse(digits[..2], CultureInfo.InvariantCulture) > 23
            || (digits.Length >= 4 && int.Parse(digits.Substring(2, 2), CultureInfo.InvariantCulture) > 59)
            || (digits.Length >= 6 && int.Parse(digits.Substring(4, 2), CultureInfo.InvariantCulture) > 59))
        {
            throw new RangeError("Invalid Temporal time offset");
        }
    }

    private static ISOAnnotations ParseISOAnnotations(string annotations)
    {
        var position = 0;
        string? calendar = null;
        var calendarCritical = false;
        string? zone = null;
        var keyValueSeen = false;
        while (position < annotations.Length)
        {
            if (annotations[position] != '[')
            {
                throw new RangeError("Invalid Temporal time annotation");
            }
            var end = annotations.IndexOf(']', position + 1);
            if (end < 0)
            {
                throw new RangeError("Invalid Temporal time annotation");
            }
            var annotation = annotations[(position + 1)..end];
            var critical = annotation.StartsWith('!');
            if (critical)
            {
                annotation = annotation[1..];
            }
            if (!AnnotationPattern().IsMatch(annotation))
            {
                throw new RangeError("Invalid Temporal time annotation");
            }
            if (annotation.Contains('='))
            {
                keyValueSeen = true;
            }
            if (annotation.StartsWith("u-ca=", StringComparison.Ordinal))
            {
                if (calendar is not null && (critical || calendarCritical))
                {
                    throw new RangeError("Duplicate critical Temporal calendar annotation");
                }
                calendar ??= annotation[5..];
                calendarCritical |= critical;
            }
            else if (annotation.Contains('='))
            {
                if (critical)
                {
                    throw new RangeError("Unknown critical Temporal time annotation");
                }
            }
            else
            {
                foreach (var component in annotation.Split('/'))
                {
                    if (component is "." or "..")
                    {
                        throw new RangeError("Invalid Temporal time zone annotation component");
                    }
                }
                if (zone is not null || keyValueSeen)
                {
                    throw new RangeError("Invalid Temporal time zone annotation order");
                }
                if (annotation.StartsWith('+') || annotation.StartsWith('-'))
                {
                    ValidateTimeOffset(annotation);
                }
                zone = annotation;
            }
            position = end + 1;
        }
        return new ISOAnnotations(calendar, zone);
    }
}
