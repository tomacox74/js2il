using JavaScriptRuntime;
using JsArray = JavaScriptRuntime.Array;

namespace Jroc.Tests;

public sealed class ArrayLengthValidationTests
{
    [Theory]
    [InlineData(0d)]
    [InlineData(1d)]
    [InlineData(2147483647d)]
    [InlineData(2147483648d)]
    [InlineData(4294967295d)]
    public void NumericAndBoxedPathsAcceptValidLengths(double value)
    {
        Assert.Equal(value, JsArray.ValidateLengthValue((object)value));
        WithRealm(() =>
        {
            var array = new JsArray();
            array.SetLength(value, throwOnError: true);
            Assert.Equal(value, array.length);
        });
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(-1d)]
    [InlineData(-0.5d)]
    [InlineData(double.Epsilon)]
    [InlineData(0.5d)]
    [InlineData(4294967295.5d)]
    [InlineData(4294967296d)]
    [InlineData(double.MaxValue)]
    public void NumericAndBoxedPathsRejectInvalidLengths(double value)
    {
        Assert.Throws<RangeError>(() => JsArray.ValidateLengthValue((object)value));
        WithRealm(() =>
        {
            var array = new JsArray();
            Assert.Throws<RangeError>(() => array.SetLength(value, throwOnError: true));
            Assert.Equal(0d, array.length);
        });
    }

    [Fact]
    public void NumericAndBoxedPathsNormalizeNegativeZero()
    {
        var negativeZero = BitConverter.Int64BitsToDouble(long.MinValue);
        Assert.Equal(0L, BitConverter.DoubleToInt64Bits(JsArray.ValidateLengthValue((object)negativeZero)));
        WithRealm(() =>
        {
            var array = new JsArray();
            array.SetLength(negativeZero, throwOnError: true);
            Assert.Equal(0L, BitConverter.DoubleToInt64Bits(array.length));
        });
    }

    [Fact]
    public void PrimitiveRepresentationsPreserveConversionSemantics()
    {
        object[] ones = [1d, 1f, 1, 1L, (short)1, (byte)1, true, "1", " 1 ", "0x1"];
        foreach (var value in ones)
        {
            Assert.Equal(1d, JsArray.ValidateLengthValue(value));
        }

        Assert.Equal(0d, JsArray.ValidateLengthValue(false));
        Assert.Equal(0d, JsArray.ValidateLengthValue(""));
        Assert.Equal(0L, BitConverter.DoubleToInt64Bits(JsArray.ValidateLengthValue("-0")));
        Assert.Throws<RangeError>(() => JsArray.ValidateLengthValue("1.5"));
        Assert.Throws<RangeError>(() => JsArray.ValidateLengthValue("4294967296"));
        Assert.Throws<RangeError>(() => JsArray.ValidateLengthValue((object?)null));
        Assert.Throws<TypeError>(() => JsArray.ValidateLengthValue(System.Numerics.BigInteger.One));
    }

    [Theory]
    [InlineData(4294967296d, 0d)]
    [InlineData(1.5d, 1d)]
    [InlineData(double.NaN, 0d)]
    public void ObjectCoercionsMayReturnDifferentNumbers(double first, double second)
    {
        WithRealm(() =>
        {
            var calls = 0;
            var value = new JsObject();
            ObjectRuntime.SetProperty(value, "valueOf",
                (Func<object[], object>)(_ => ++calls == 1 ? first : second));
            Assert.Equal(second, JsArray.ValidateLengthValue(value));
            Assert.Equal(2, calls);
        });
    }

    [Fact]
    public void SecondObjectCoercionErrorIsNotReplacedByEarlyRangeValidation()
    {
        WithRealm(() =>
        {
            var calls = 0;
            var expected = new TypeError("second conversion");
            var value = new JsObject();
            ObjectRuntime.SetProperty(value, "valueOf",
                (Func<object[], object>)(_ => ++calls == 1 ? 1.5d : throw expected));
            Assert.Same(expected, Assert.Throws<TypeError>(() => JsArray.ValidateLengthValue(value)));
            Assert.Equal(2, calls);
        });
    }

    private static void WithRealm(Action body)
    {
        var services = RuntimeServices.BuildServiceProvider();
        try
        {
            using var scope = RuntimeExecutionContext.GetOrCreate(services).EnterAsRoot();
            body();
        }
        finally
        {
            services.OwningRealm!.Agent.Cluster.Dispose();
        }
    }
}
