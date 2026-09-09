using JavaScriptRuntime;
using System.Runtime.CompilerServices;
using JSONRuntime = JavaScriptRuntime.JSON;

namespace Jroc.Tests;

public sealed class JSONShapeStorageTests
{
    [Fact]
    public void RepeatedLayoutsShareOnlyShapeAndKeepIndependentValuesAndDescriptors()
    {
        const string text = """{"price":1,"quantity":2,"stock":"ABC"}""";
        _ = JSONRuntime.Parse(text);
        var first = Assert.IsType<JsObject>(JSONRuntime.Parse(text));
        var second = Assert.IsType<JsObject>(JSONRuntime.Parse(text));

        Assert.Same(first.Shape, second.Shape);
        Assert.False(first.HasInlineDescriptorState);
        Assert.True(PropertyDescriptorStore.TryGetOwn(first, "price", out var descriptor));
        Assert.True(descriptor.Writable && descriptor.Enumerable && descriptor.Configurable);

        descriptor.Writable = false;
        descriptor.Value = 10d;
        first.DefineOwnProperty("price", descriptor);
        first.Remove("quantity");
        first.SetString("added", "new");

        Assert.Equal(1d, second["price"]);
        Assert.Equal(2d, second["quantity"]);
        Assert.False(second.ContainsKey("added"));
        Assert.False(second.HasInlineDescriptorState);
        Assert.Equal(new[] { "price", "stock", "added" }, first.Keys);
        Assert.Equal(new[] { "price", "quantity", "stock" }, second.Keys);
        Assert.Same(second.Shape, Assert.IsType<JsObject>(JSONRuntime.Parse(text)).Shape);
    }

    [Fact]
    public void CachedTransitionsHandleEscapedNamesDuplicatesAndDifferentInsertionOrder()
    {
        _ = JSONRuntime.Parse("""{"a":1,"b":2,"c":3}""");
        var duplicate = Assert.IsType<JsObject>(JSONRuntime.Parse("""{"\u0061":4,"b":5,"a":6,"c":7}"""));
        var reordered = Assert.IsType<JsObject>(JSONRuntime.Parse("""{"c":8,"a":9,"b":10}"""));

        Assert.Equal(3, duplicate.Count);
        Assert.Equal(new[] { "a", "b", "c" }, duplicate.Keys);
        Assert.Equal(new object?[] { 6d, 5d, 7d }, duplicate.Values);
        Assert.Equal(new[] { "c", "a", "b" }, reordered.Keys);
        Assert.Equal(new object?[] { 8d, 9d, 10d }, reordered.Values);
    }

    [Fact]
    public void NestedLayoutsCannotChangeParentSlotSelection()
    {
        const string text = """{"a":{"a":1,"b":2},"b":{"b":3,"a":4},"c":[{"a":5,"b":6}]}""";
        for (var i = 0; i < 3; i++)
        {
            var root = Assert.IsType<JsObject>(JSONRuntime.Parse(text));
            Assert.Equal(1d, Assert.IsType<JsObject>(root["a"])["a"]);
            Assert.Equal(4d, Assert.IsType<JsObject>(root["b"])["a"]);
            var array = Assert.IsType<JavaScriptRuntime.Array>(root["c"]);
            Assert.Equal(6d, Assert.IsType<JsObject>(array[0])["b"]);
        }
    }

    [Fact]
    public void CacheSaturationDoesNotChangeValuesOrRetainParsedObjects()
    {
        var references = new List<(WeakReference Value, WeakReference Key)>();
        for (var i = 0; i < 300; i++)
        {
            references.Add(ParseUniqueLayout(i));
        }
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Assert.All(references, reference => Assert.False(reference.Value.IsAlive));
        Assert.InRange(references.Count(reference => reference.Key.IsAlive), 0, 128);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (WeakReference Value, WeakReference Key) ParseUniqueLayout(int index)
    {
        var key = $"json-shape-saturation-{index}";
        var value = Assert.IsType<JsObject>(JSONRuntime.Parse($$$"""{"{{{key}}}":{{{index}}},"nested":{}}"""));
        Assert.Equal((double)index, value[key]);
        var storedKey = value.Shape.GetPropertyNameAtSlot(0);
        Assert.Null(string.IsInterned(storedKey));
        return (new WeakReference(value), new WeakReference(storedKey));
    }

    [Fact]
    public void OversizedUntrustedKeysAreNotInternedOrRetainedByTheCache()
    {
        var reference = ParseOversizedKey();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Assert.False(reference.IsAlive);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference ParseOversizedKey()
    {
        var key = new string('x', 9000) + Guid.NewGuid().ToString("N");
        var result = Assert.IsType<JsObject>(JSONRuntime.Parse($$"""{"{{key}}":42}"""));
        var storedKey = Assert.Single(result.Keys);
        Assert.Null(string.IsInterned(storedKey));
        Assert.Equal(42d, result[storedKey]);
        return new WeakReference(storedKey);
    }

    [Fact]
    public void CompletePropertyStoreRejectsMismatchedSlotCounts()
    {
        var shape = new JsShape().TransitionToUncached("value");
        Assert.Throws<ArgumentException>(() => new JsObject(shape, []));
        var result = new JsObject(shape, [JsValue.FromNumber(42d)]);
        Assert.Equal(42d, result["value"]);
        result.SetNumber("next", 7d);
        Assert.Equal(new object?[] { 42d, 7d }, result.Values);
    }
}
