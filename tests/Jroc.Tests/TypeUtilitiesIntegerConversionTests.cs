using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class TypeUtilitiesIntegerConversionTests
{
    [Theory]
    [InlineData(0d, 0)]
    [InlineData(1.9d, 1)]
    [InlineData(-1.9d, -1)]
    [InlineData(2147483647d, int.MaxValue)]
    [InlineData(-2147483648d, int.MinValue)]
    [InlineData(2147483648d, int.MinValue)]
    [InlineData(4294967295d, -1)]
    [InlineData(4294967296d, 0)]
    [InlineData(-4294967297d, -1)]
    [InlineData(double.NaN, 0)]
    [InlineData(double.PositiveInfinity, 0)]
    [InlineData(double.NegativeInfinity, 0)]
    public void ToInt32_UsesFastAndModuloPaths(double value, int expected)
    {
        Assert.Equal(expected, TypeUtilities.ToInt32(value));
    }

    [Theory]
    [InlineData(0d, 0L)]
    [InlineData(1.9d, 1L)]
    [InlineData(-1.9d, 4294967295L)]
    [InlineData(2147483648d, 2147483648L)]
    [InlineData(4294967295d, 4294967295L)]
    [InlineData(4294967296d, 0L)]
    [InlineData(4294967297d, 1L)]
    [InlineData(-4294967296d, 0L)]
    [InlineData(double.NaN, 0L)]
    [InlineData(double.PositiveInfinity, 0L)]
    [InlineData(double.NegativeInfinity, 0L)]
    public void ToUint32_UsesFastAndModuloPaths(double value, long expected)
    {
        Assert.Equal(expected, TypeUtilities.ToUint32(value));
    }

    [Theory]
    [InlineData(0d, 0)]
    [InlineData(2147483648d, int.MinValue)]
    [InlineData(4294967295d, -1)]
    [InlineData(4294967296d, 0)]
    public void ToUint32AsInt32_PreservesUint32Bits(double value, int expected)
    {
        Assert.Equal(expected, TypeUtilities.ToUint32AsInt32(value));
    }
}
