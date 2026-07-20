using JavaScriptRuntime;
using System.Runtime.CompilerServices;

namespace Jroc.Tests;

public sealed class JsShapeTransitionTests
{
    public static TheoryData<int, bool> ShapeStorageCases => new()
    {
        { 0, false },
        { 1, false },
        { JsShape.DictionaryPromotionThreshold, false },
        { JsShape.DictionaryPromotionThreshold + 1, true },
        { 16, true }
    };

    [Fact]
    public void NewShape_DoesNotAllocateTransitionCache()
    {
        var shape = new JsShape();

        Assert.False(shape.HasTransitionCache);
        Assert.Equal(0, shape.TransitionCacheCount);
    }

    [Fact]
    public void TransitionTo_AllocatesTransitionCacheOnlyOnParentAndReusesLiveChild()
    {
        var parent = new JsShape();

        var first = parent.TransitionTo("value");
        var second = parent.TransitionTo("value");

        Assert.Same(first, second);
        Assert.True(parent.HasTransitionCache);
        Assert.Equal(1, parent.TransitionCacheCount);
        Assert.False(first.HasTransitionCache);
        Assert.Equal(0, first.TransitionCacheCount);
    }

    [Fact]
    public void TransitionToUncached_DoesNotAllocateTransitionCache()
    {
        var parent = new JsShape();

        var child = parent.TransitionToUncached("value");

        Assert.False(parent.HasTransitionCache);
        Assert.Equal(0, parent.TransitionCacheCount);
        Assert.False(child.HasTransitionCache);
        Assert.Equal(0, child.TransitionCacheCount);
    }

    [Theory]
    [MemberData(nameof(ShapeStorageCases))]
    public void Storage_PromotesOnlyAboveCompactThreshold(int propertyCount, bool expectedDictionary)
    {
        var shape = CreateUncachedShape(propertyCount);

        Assert.Equal(propertyCount, shape.PropertyCount);
        Assert.Equal(expectedDictionary, shape.UsesDictionaryLookup);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    public void GetSlot_ResolvesOrderedNamesAcrossStorageRepresentations(int propertyCount)
    {
        var shape = CreateUncachedShape(propertyCount);

        Assert.Equal(0, shape.GetSlot("p0"));
        Assert.Equal(propertyCount / 2, shape.GetSlot($"p{propertyCount / 2}"));
        Assert.Equal(propertyCount - 1, shape.GetSlot($"p{propertyCount - 1}"));
        Assert.Equal(-1, shape.GetSlot("missing"));
    }

    [Fact]
    public void PropertyNamesInSlotOrder_RepeatedEnumerationDoesNotAllocate()
    {
        var shape = CreateUncachedShape(16);
        var total = SumPropertyNameLengths(shape);

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var iteration = 0; iteration < 1_000; iteration++)
        {
            total += SumPropertyNameLengths(shape);
        }
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.True(total > 0);
        Assert.Equal(0, allocated);
    }

    [Fact]
    public void TransitionAway_CompactsSlotsAndDemotesAtThreshold()
    {
        var shape = CreateUncachedShape(JsShape.DictionaryPromotionThreshold + 2);
        var lastPropertyName = $"p{JsShape.DictionaryPromotionThreshold + 1}";

        shape = shape.TransitionAway("p1");

        Assert.True(shape.UsesDictionaryLookup);
        Assert.Equal("p2", shape.GetPropertyNameAtSlot(1));
        Assert.Equal(1, shape.GetSlot("p2"));

        shape = shape.TransitionAway(lastPropertyName);

        Assert.False(shape.UsesDictionaryLookup);
        Assert.Equal(JsShape.DictionaryPromotionThreshold, shape.PropertyCount);
        Assert.Equal(
            JsShape.DictionaryPromotionThreshold - 1,
            shape.GetSlot($"p{JsShape.DictionaryPromotionThreshold}"));
    }

    [Fact]
    public void RemovedProperty_ReaddedAtEndOfSlotOrder()
    {
        var shape = CreateUncachedShape(4)
            .TransitionAway("p1")
            .TransitionToUncached("p1");

        Assert.Equal(new[] { "p0", "p2", "p3", "p1" }, shape.PropertyNamesInSlotOrder.ToArray());
        Assert.Equal(3, shape.GetSlot("p1"));
    }

    [Fact]
    public void UncachedLeafShape_DoesNotRetainIntermediateShapes()
    {
        var (leaf, intermediateShapes) = CreateUncachedShapeWithWeakIntermediateReferences(16);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.Equal(16, leaf.PropertyCount);
        Assert.All(intermediateShapes, reference => Assert.False(reference.TryGetTarget(out _)));
    }

    private static JsShape CreateUncachedShape(int propertyCount)
    {
        var shape = new JsShape();
        for (var index = 0; index < propertyCount; index++)
        {
            shape = shape.TransitionToUncached($"p{index}");
        }
        return shape;
    }

    private static int SumPropertyNameLengths(JsShape shape)
    {
        var total = 0;
        foreach (var propertyName in shape.PropertyNamesInSlotOrder)
        {
            total += propertyName.Length;
        }
        return total;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (JsShape Leaf, WeakReference<JsShape>[] IntermediateShapes)
        CreateUncachedShapeWithWeakIntermediateReferences(int propertyCount)
    {
        var intermediateShapes = new List<WeakReference<JsShape>>(propertyCount - 1);
        var shape = new JsShape();
        for (var index = 0; index < propertyCount; index++)
        {
            var nextShape = shape.TransitionToUncached($"retained-{index}");
            if (index > 0)
            {
                intermediateShapes.Add(new WeakReference<JsShape>(shape));
            }
            shape = nextShape;
        }
        return (shape, intermediateShapes.ToArray());
    }
}
