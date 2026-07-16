using System.Globalization;

namespace JavaScriptRuntime;

/// <summary>
/// Allocation-free classification of ECMAScript array-index property keys.
/// </summary>
internal static class CanonicalArrayIndex
{
    public static bool TryParse(string? key, out uint index)
    {
        index = 0;
        if (string.IsNullOrEmpty(key)
            || !uint.TryParse(key, NumberStyles.None, CultureInfo.InvariantCulture, out var parsed)
            || parsed == uint.MaxValue
            || key != parsed.ToString(CultureInfo.InvariantCulture))
        {
            return false;
        }

        index = parsed;
        return true;
    }

    public static bool TryParseInt32(string? key, out int index)
    {
        index = 0;
        if (!TryParse(key, out var arrayIndex) || arrayIndex > int.MaxValue)
        {
            return false;
        }

        index = (int)arrayIndex;
        return true;
    }
}
