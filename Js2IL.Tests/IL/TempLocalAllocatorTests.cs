using Js2IL.IL;
using Js2IL.IR;
using Xunit;

namespace Js2IL.Tests;

public sealed class TempLocalAllocatorTests
{
    [Fact]
    public void TryGetDefinedTemp_Recognizes_LIRSetItem_Result()
    {
        var instruction = new LIRSetItem(
            Object: new TempVariable(0),
            Index: new TempVariable(1),
            Value: new TempVariable(2),
            Result: new TempVariable(3));

        Assert.True(TempLocalAllocator.TryGetDefinedTemp(instruction, out var defined));
        Assert.Equal(3, defined.Index);
    }

    [Fact]
    public void TryGetDefinedTemp_Recognizes_LIRSetJsArrayElement_Result()
    {
        var instruction = new LIRSetJsArrayElement(
            Receiver: new TempVariable(0),
            Index: new TempVariable(1),
            Value: new TempVariable(2),
            Result: new TempVariable(3));

        Assert.True(TempLocalAllocator.TryGetDefinedTemp(instruction, out var defined));
        Assert.Equal(3, defined.Index);
    }

    [Fact]
    public void TryGetDefinedTemp_Recognizes_LIRSetInt32ArrayElement_Result()
    {
        var instruction = new LIRSetInt32ArrayElement(
            Receiver: new TempVariable(0),
            Index: new TempVariable(1),
            Value: new TempVariable(2),
            Result: new TempVariable(3));

        Assert.True(TempLocalAllocator.TryGetDefinedTemp(instruction, out var defined));
        Assert.Equal(3, defined.Index);
    }
}
