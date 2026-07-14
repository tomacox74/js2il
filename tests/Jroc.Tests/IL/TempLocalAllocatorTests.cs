using Jroc.IL;
using Jroc.IR;
using Xunit;

namespace Jroc.Tests;

public sealed class TempLocalAllocatorTests
{
    [Fact]
    public void VisitUsedTemps_VisitsFixedOperandsInOrder()
    {
        var instruction = new LIRSetItem(
            Object: new TempVariable(1),
            Index: new TempVariable(2),
            Value: new TempVariable(3),
            Result: new TempVariable(4));
        var visitor = new CollectingVisitor();

        TempLocalAllocator.VisitUsedTemps(instruction, ref visitor);

        Assert.Equal(new[] { 1, 2, 3 }, visitor.Indices);
    }

    [Fact]
    public void VisitUsedTemps_VisitsVariableOperandsInOrder()
    {
        var instruction = new LIRNewJsArray(
            new[] { new TempVariable(1), new TempVariable(2), new TempVariable(3) },
            new TempVariable(4));
        var visitor = new CollectingVisitor();

        TempLocalAllocator.VisitUsedTemps(instruction, ref visitor);

        Assert.Equal(new[] { 1, 2, 3 }, visitor.Indices);
    }

    [Fact]
    public void VisitUsedTemps_DoesNotAllocateForFixedOperandInstruction()
    {
        var instruction = new LIRSetItem(
            Object: new TempVariable(1),
            Index: new TempVariable(2),
            Value: new TempVariable(3),
            Result: new TempVariable(4));
        var visitor = new CountingVisitor();

        TempLocalAllocator.VisitUsedTemps(instruction, ref visitor);
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 10_000; i++)
        {
            TempLocalAllocator.VisitUsedTemps(instruction, ref visitor);
        }
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(0, allocated);
        Assert.Equal(30_003, visitor.Count);
    }

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

    [Fact]
    public void TryGetDefinedTemp_Recognizes_LIRSetJsArrayLength_Result()
    {
        var instruction = new LIRSetJsArrayLength(
            Receiver: new TempVariable(0),
            Value: new TempVariable(1),
            Result: new TempVariable(2));

        Assert.True(TempLocalAllocator.TryGetDefinedTemp(instruction, out var defined));
        Assert.Equal(2, defined.Index);
    }

    private struct CollectingVisitor : ITempUseVisitor
    {
        public CollectingVisitor()
        {
            Indices = new List<int>();
        }

        public List<int> Indices { get; }

        public void Visit(TempVariable temp)
        {
            Indices.Add(temp.Index);
        }
    }

    private struct CountingVisitor : ITempUseVisitor
    {
        public int Count { get; private set; }

        public void Visit(TempVariable temp)
        {
            Count++;
        }
    }
}
