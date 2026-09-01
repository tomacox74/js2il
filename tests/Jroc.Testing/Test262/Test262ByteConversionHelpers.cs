using JavaScriptRuntime;

namespace Jroc.Tests;

internal static class Test262ByteConversionHelpers
{
    public static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
        => builder.AddGlobalFactory("byteConversionValues", CreateByteConversionValues);

    private static object CreateByteConversionValues()
    {
        var result = new JsObject();
        ObjectRuntime.SetItem(result, "values", new JavaScriptRuntime.Array(
        new object?[]
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
        }));
        return result;
    }
}
