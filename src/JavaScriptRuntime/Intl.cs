using System.Globalization;

namespace JavaScriptRuntime;

public sealed class IntlNumberFormat
{
    public string format(object? value)
    {
        var number = TypeUtilities.ToNumber(value);

        if (double.IsNaN(number))
        {
            return "NaN";
        }

        if (double.IsPositiveInfinity(number))
        {
            return "Infinity";
        }

        if (double.IsNegativeInfinity(number))
        {
            return "-Infinity";
        }

        if (number == System.Math.Truncate(number))
        {
            return number.ToString("#,0", CultureInfo.InvariantCulture);
        }

        return number.ToString("#,0.###", CultureInfo.InvariantCulture);
    }
}

public sealed class IntlSegmenter
{
    public Array segment(object? input)
    {
        var text = DotNet2JSConversions.ToString(input);
        var segments = new Array();

        if (string.IsNullOrEmpty(text))
        {
            return segments;
        }

        var enumerator = StringInfo.GetTextElementEnumerator(text);
        while (enumerator.MoveNext())
        {
            var segment = new JsObject();
            segment.SetObject("segment", enumerator.GetTextElement());
            segments.Add(segment);
        }

        return segments;
    }
}
