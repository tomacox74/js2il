using Jroc.IL;
using Jroc.IR;

namespace Jroc.Tests;

public sealed class LIRStackSchedulerAnalysisTests
{
    [Fact]
    public void Analysis_EmptyBody_HasEmptyFacts()
    {
        var analysis = new LIRStackSchedulerAnalysis(new MethodBodyIR());

        Assert.Empty(analysis.DefinitionIndex.ToArray());
        Assert.Empty(analysis.DefinitionCount.ToArray());
        Assert.Empty(analysis.UseIndex.ToArray());
        Assert.Empty(analysis.UseCount.ToArray());
    }

    [Fact]
    public void Analysis_PreservesLastDefinitionsAndOperandOccurrenceOrder()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body);
        var sum = AddTemp(body);
        var unused = AddTemp(body);
        body.Instructions.Add(new LIRConstNumber(1, value));
        body.Instructions.Add(new LIRAddNumber(value, value, sum));
        body.Instructions.Add(new LIRConstNumber(2, value));
        body.Instructions.Add(new LIRReturn(sum));
        body.Instructions.Add(new LIRAddNumber(sum, value, unused));

        var analysis = new LIRStackSchedulerAnalysis(body);

        Assert.Equal(new[] { 2, 1, 4 }, analysis.DefinitionIndex.ToArray());
        Assert.Equal(new[] { 2, 1, 1 }, analysis.DefinitionCount.ToArray());
        Assert.Equal(new[] { 4, 4, -1 }, analysis.UseIndex.ToArray());
        Assert.Equal(new[] { 3, 2, 0 }, analysis.UseCount.ToArray());
        Assert.Empty(analysis.GetUses(0).ToArray());
        Assert.Equal(new[] { value, value }, analysis.GetUses(1).ToArray());
        Assert.Empty(analysis.GetUses(2).ToArray());
        Assert.Equal(new[] { sum }, analysis.GetUses(3).ToArray());
        Assert.Equal(new[] { sum, value }, analysis.GetUses(4).ToArray());
    }

    [Fact]
    public void Analysis_KeepsUnregisteredOperandsWithoutCountingThem()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body);
        var negative = new TempVariable(-1);
        var outside = new TempVariable(100);
        body.Instructions.Add(new LIRAddNumber(negative, value, outside));
        body.Instructions.Add(new LIRReturn(outside));

        var analysis = new LIRStackSchedulerAnalysis(body);

        Assert.Equal(new[] { -1 }, analysis.DefinitionIndex.ToArray());
        Assert.Equal(new[] { 0 }, analysis.DefinitionCount.ToArray());
        Assert.Equal(new[] { 0 }, analysis.UseIndex.ToArray());
        Assert.Equal(new[] { 1 }, analysis.UseCount.ToArray());
        Assert.Equal(new[] { negative, value }, analysis.GetUses(0).ToArray());
        Assert.Equal(new[] { outside }, analysis.GetUses(1).ToArray());
    }

    private static TempVariable AddTemp(MethodBodyIR body)
    {
        var temp = new TempVariable(body.Temps.Count);
        body.Temps.Add(temp);
        return temp;
    }
}
