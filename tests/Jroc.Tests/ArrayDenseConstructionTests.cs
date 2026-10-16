using JavaScriptRuntime;
using JsArray = JavaScriptRuntime.Array;
using JsObjectConstructor = JavaScriptRuntime.Object;

namespace Jroc.Tests;

public sealed class ArrayDenseConstructionTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(65_536)]
    [InlineData(65_540)]
    public void BulkFilledStoragePreservesDenseArrayState(int length)
    {
        using var lifecycle = RuntimeLifecycle.Create(typeof(ArrayDenseConstructionTests).Assembly, isHostedExecution: true);
        using var scope = lifecycle.EnterAsRoot();
        var value = new JsObject();
        var array = JsArray.CreateWithDenseLength(length, out var elements);
        Assert.Equal(length, elements.Length);
        elements.Fill(value);

        Assert.Equal(length, array.Count);
        Assert.Equal((double)length, array.length);
        Assert.Same(JsArray.Prototype, JsObjectConstructor.getPrototypeOf(array));
        for (var index = 0; index < length; index++)
        {
            Assert.Same(value, array[index]);
        }

        if (length > 0)
        {
            var descriptor = Assert.IsType<JsObject>(
                JsObjectConstructor.getOwnPropertyDescriptor(array, (length - 1).ToString()));
            Assert.Same(value, descriptor["value"]);
            Assert.Equal(true, descriptor["writable"]);
            Assert.Equal(true, descriptor["enumerable"]);
            Assert.Equal(true, descriptor["configurable"]);
        }

        var lengthDescriptor = Assert.IsType<JsObject>(
            JsObjectConstructor.getOwnPropertyDescriptor(array, "length"));
        Assert.Equal((double)length, lengthDescriptor["value"]);
        Assert.Equal(true, lengthDescriptor["writable"]);
        Assert.Equal(false, lengthDescriptor["enumerable"]);
        Assert.Equal(false, lengthDescriptor["configurable"]);

        array.Add("tail");
        Assert.Equal(length + 1, array.Count);
        Assert.Equal("tail", array.pop());
        Assert.Equal(length, array.Count);
        array.length = 0d;
        Assert.Empty(array);
        Assert.Equal(0d, array.length);
        array.Add(value);
        Assert.Equal(1d, array.length);
        Assert.Same(value, array[0]);
    }

    [Fact]
    public void NegativeDenseLengthIsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => JsArray.CreateWithDenseLength(-1, out _));
    }

    [Theory]
    [InlineData("", 65_537, 65_537)]
    [InlineData("u", 65_537, 65_537)]
    [InlineData("v", 65_537, 65_537)]
    [InlineData("", 65_540, 65_540)]
    [InlineData("u", 65_540, 65_539)]
    [InlineData("v", 65_540, 65_539)]
    public void EmptySplitFillsLargeResultsAtCodePointAndCapacityBoundaries(
        string flags, int limit, int expectedLength)
    {
        using var lifecycle = RuntimeLifecycle.Create(typeof(ArrayDenseConstructionTests).Assembly, isHostedExecution: true);
        using var scope = lifecycle.EnterAsRoot();
        var input = new string('a', 65_536) + "\uD83D\uDE00\uD800b";
        var separator = JavaScriptRuntime.RegExp.CreateLiteral("(?:)", flags);
        separator.lastIndex = 7d;

        var result = JavaScriptRuntime.String.SplitWithRegExp(input, separator, (double)limit);
        var expectedLast = limit == 65_540 ? "b" : flags.Length == 0 ? "\uD83D" : "\uD83D\uDE00";

        Assert.Equal(expectedLength, result.Count);
        Assert.Equal((double)expectedLength, result.length);
        Assert.Equal(expectedLast, result[expectedLength - 1]);
        Assert.Same(JavaScriptRuntime.String.CharToStringFast('a'), result[0]);
        Assert.Same(result[0], result[65_535]);
        if (limit == 65_540)
        {
            Assert.Equal("\uD800", result[expectedLength - 2]);
        }
        Assert.Equal(7d, separator.lastIndex);
    }
}
