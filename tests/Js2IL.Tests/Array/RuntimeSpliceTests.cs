using System;
using System.Linq;
using JavaScriptRuntime;
using Xunit;

namespace Js2IL.Tests.Array;

public class RuntimeSpliceTests
{
    [Fact]
    public void Splice_Delete_From_Middle()
    {
        var a = new JavaScriptRuntime.Array(new object[] { 0, 1, 2, 3, 4, 5 });
    var removed = a.splice(new object[] { 2, 2 }) as JavaScriptRuntime.Array;
        Assert.NotNull(removed);
        Assert.Equal(new object[] { 2, 3 }, removed!.ToList());
        Assert.Equal(new object[] { 0, 1, 4, 5 }, a.ToList());
    }

    [Fact]
    public void Splice_Insert_Without_Delete()
    {
        var a = new JavaScriptRuntime.Array(new object[] { 0, 1, 4, 5 });
    var removed = a.splice(new object[] { 2, 0, 2, 3 }) as JavaScriptRuntime.Array;
        Assert.NotNull(removed);
        Assert.Empty(removed!);
        Assert.Equal(new object[] { 0, 1, 2, 3, 4, 5 }, a.ToList());
    }

    [Fact]
    public void Splice_Delete_To_End_When_DeleteCount_Omitted()
    {
        var a = new JavaScriptRuntime.Array(new object[] { 0, 1, 2, 3, 4, 5 });
    var removed = a.splice(new object[] { 4 }) as JavaScriptRuntime.Array;
        Assert.NotNull(removed);
        Assert.Equal(new object[] { 4, 5 }, removed!.ToList());
        Assert.Equal(new object[] { 0, 1, 2, 3 }, a.ToList());
    }

    [Fact]
    public void Splice_Negative_Start_And_Overlarge_DeleteCount()
    {
        var a = new JavaScriptRuntime.Array(new object[] { 0, 1, 2, 3 });
    var removed = a.splice(new object[] { -3, 10 }) as JavaScriptRuntime.Array; // start becomes 1, delete to end
        Assert.NotNull(removed);
        Assert.Equal(new object[] { 1, 2, 3 }, removed!.ToList());
        Assert.Equal(new object[] { 0 }, a.ToList());
    }

    [Fact]
    public void Splice_Returned_Array_Is_Independent_Shallow_Copy()
    {
        var inner = new object();
        var a = new JavaScriptRuntime.Array(new object[] { inner, 1, 2 });
    var removed = a.splice(new object[] { 0, 1 }) as JavaScriptRuntime.Array;
        Assert.NotNull(removed);
        Assert.Single(removed!);
        // Shallow copy: removed[0] is same reference
        Assert.Same(inner, removed![0]);

        // Mutate original array; removed should not change size/content
    a.push(new object[] { 99 });
        Assert.Single(removed);
        Assert.Same(inner, removed[0]);
    }

    [Fact]
    public void Splice_DeleteCount_NonNumeric_Coerces_To_Zero()
    {
        var a = new JavaScriptRuntime.Array(new object[] { 0, 1, 2 });
    var removed = a.splice(new object[] { 1, "foo" }) as JavaScriptRuntime.Array; // deleteCount -> +0
        Assert.NotNull(removed);
        Assert.Empty(removed!);
        Assert.Equal(new object[] { 0, 1, 2 }, a.ToList());
    }
}
