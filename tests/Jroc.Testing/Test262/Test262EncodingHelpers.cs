using JavaScriptRuntime;

namespace Jroc.Tests;

internal static class Test262EncodingHelpers
{
    private const string HexDigits = "0123456789ABCDEF";

    public static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
    {
        builder
            .AddGlobalFactory("decimalToHexString", () => Test262HostRuntimeIntrinsics.CreateFunction(
                (Func<object?, string>)DecimalToHexString,
                "decimalToHexString",
                1))
            .AddGlobalFactory("decimalToPercentHexString", () => Test262HostRuntimeIntrinsics.CreateFunction(
                (Func<object?, string>)DecimalToPercentHexString,
                "decimalToPercentHexString",
                1));
    }

    private static string DecimalToHexString(object? value)
    {
        var number = TypeUtilities.ToUint32(value);
        var result = number.ToString("X", System.Globalization.CultureInfo.InvariantCulture);
        return result.Length < 4 ? result.PadLeft(4, '0') : result;
    }

    private static string DecimalToPercentHexString(object? value)
    {
        var number = TypeUtilities.ToUint32(value);
        return string.Create(
            3,
            number,
            static (chars, current) =>
            {
                chars[0] = '%';
                chars[1] = HexDigits[(int)((current >> 4) & 0xf)];
                chars[2] = HexDigits[(int)(current & 0xf)];
            });
    }
}
