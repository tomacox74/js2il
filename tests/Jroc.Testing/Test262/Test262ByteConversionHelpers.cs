using JavaScriptRuntime;

namespace Jroc.Tests;

internal static class Test262ByteConversionHelpers
{
    public static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
        => builder.AddGlobalFactory("byteConversionValues", CreateByteConversionValues);

    private static object CreateByteConversionValues()
    {
        var result = new JsObject();
        var values = new object?[]
        {
            127d,
            128d,
            32767d,
            32768d,
            2147483647d,
            2147483648d,
            255d,
            256d,
            65535d,
            65536d,
            4294967295d,
            4294967296d,
            9007199254740991d,
            9007199254740992d,
            1.1d,
            0.1d,
            0.5d,
            0.50000001d,
            0.6d,
            0.7d,
            null,
            -1d,
            -0d,
            -0.1d,
            -1.1d,
            double.NaN,
            -127d,
            -128d,
            -32767d,
            -32768d,
            -2147483647d,
            -2147483648d,
            -255d,
            -256d,
            -65535d,
            -65536d,
            -4294967295d,
            -4294967296d,
            double.PositiveInfinity,
            double.NegativeInfinity,
            0d,
            2049d,
            2051d,
            0.00006103515625d,
            0.00006097555160522461d,
            5.960464477539063e-8d,
            2.9802322387695312e-8d,
            2.980232238769532e-8d,
            8.940696716308594e-8d,
            1.4901161193847656e-7d,
            1.490116119384766e-7d,
            65504d,
            65520d,
            65519.99999999999d,
            0.000061005353927612305d,
            0.0000610053539276123d
        };

        ObjectRuntime.SetItem(result, "values", new JavaScriptRuntime.Array(values));

        var expected = new JsObject();
        ObjectRuntime.SetItem(expected, "Int8", CreateExpectedValues(values, value => ToSignedInteger(value, 8)));
        ObjectRuntime.SetItem(expected, "Uint8", CreateExpectedValues(values, value => ToUnsignedInteger(value, 8)));
        ObjectRuntime.SetItem(expected, "Int16", CreateExpectedValues(values, value => ToSignedInteger(value, 16)));
        ObjectRuntime.SetItem(expected, "Uint16", CreateExpectedValues(values, value => ToUnsignedInteger(value, 16)));
        ObjectRuntime.SetItem(expected, "Int32", CreateExpectedValues(values, value => ToSignedInteger(value, 32)));
        ObjectRuntime.SetItem(expected, "Uint32", CreateExpectedValues(values, value => ToUnsignedInteger(value, 32)));
        ObjectRuntime.SetItem(expected, "Float32", CreateExpectedValues(values, value => (double)(float)value));
        ObjectRuntime.SetItem(expected, "Float64", CreateExpectedValues(values, value => value));
        ObjectRuntime.SetItem(result, "expected", expected);
        return result;
    }

    private static JavaScriptRuntime.Array CreateExpectedValues(
        object?[] values,
        Func<double, double> convert)
    {
        var expected = new object?[values.Length];
        for (var index = 0; index < values.Length; index++)
        {
            expected[index] = convert(values[index] is double value ? value : double.NaN);
        }

        return new JavaScriptRuntime.Array(expected);
    }

    private static double ToUnsignedInteger(double value, int bitWidth)
    {
        if (!double.IsFinite(value))
        {
            return 0d;
        }

        var modulo = System.Math.Pow(2d, bitWidth);
        var integer = System.Math.Truncate(value) % modulo;
        if (integer == 0d)
        {
            return 0d;
        }

        return integer < 0d ? integer + modulo : integer;
    }

    private static double ToSignedInteger(double value, int bitWidth)
    {
        var unsigned = ToUnsignedInteger(value, bitWidth);
        var signBit = System.Math.Pow(2d, bitWidth - 1);
        return unsigned >= signBit ? unsigned - (signBit * 2d) : unsigned;
    }
}
