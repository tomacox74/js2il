using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class JsShapePropertyNameTests
{
    private const int PropertyCount = 256;

    private static volatile int _sink;

    [Fact]
    public void GetOwnProperties_DoesNotAllocateOrderedNamesPerProperty()
    {
        var obj = CreateObject(PropertyCount);

        var allocated = MeasureAllocatedBytes(() =>
        {
            var total = 0;
            foreach (var property in obj.GetOwnProperties())
            {
                total += property.Key.Length + ((string)property.Value!).Length;
            }
            _sink = total;
        });

        Assert.InRange(allocated, 0, 1024);
    }

    [Fact]
    public void PublicPropertyNameEnumeration_IsSnapshotSafeAndDoesNotExposeShapeStorage()
    {
        var obj = CreateObject(3);

        var names = obj.GetOwnPropertyNames();
        var keys = obj.Keys;
        keys.Clear();

        obj.SetString("p3", "v3");

        Assert.Equal(new[] { "p0", "p1", "p2" }, names.ToArray());
        Assert.Equal(new[] { "p0", "p1", "p2", "p3" }, obj.GetOwnPropertyNames().ToArray());
    }

    [Fact]
    public void OwnPropertyEnumeration_RemainsSnapshotSafeWhenObjectMutates()
    {
        var obj = CreateObject(3);
        using var enumerator = obj.GetOwnProperties().GetEnumerator();

        Assert.True(enumerator.MoveNext());
        Assert.Equal("p0", enumerator.Current.Key);

        Assert.True(obj.Remove("p1"));
        obj.SetString("p3", "v3");

        var remainingKeys = new List<string>();
        while (enumerator.MoveNext())
        {
            remainingKeys.Add(enumerator.Current.Key);
        }

        Assert.Equal(new[] { "p1", "p2" }, remainingKeys);
        Assert.Equal(new[] { "p0", "p2", "p3" }, obj.GetOwnPropertyNames().ToArray());
    }

    [Fact]
    public void DictionaryEnumeration_RemainsSnapshotSafeWhenObjectMutates()
    {
        var obj = CreateObject(3);
        using var enumerator = obj.GetEnumerator();

        Assert.True(enumerator.MoveNext());
        Assert.Equal("p0", enumerator.Current.Key);

        Assert.True(obj.Remove("p1"));
        obj.SetString("p3", "v3");

        var remainingKeys = new List<string>();
        while (enumerator.MoveNext())
        {
            remainingKeys.Add(enumerator.Current.Key);
        }

        Assert.Equal(new[] { "p1", "p2" }, remainingKeys);
        Assert.Equal(new[] { "p0", "p2", "p3" }, obj.GetOwnPropertyNames().ToArray());
    }

    [Fact]
    public void DeletionPreservesPropertyAndValueOrder()
    {
        var obj = CreateObject(4);

        Assert.True(obj.Remove("p1"));

        Assert.Equal(new[] { "p0", "p2", "p3" }, obj.Keys);
        Assert.Equal(new object?[] { "v0", "v2", "v3" }, obj.Values);
    }

    private static JsObject CreateObject(int propertyCount)
    {
        var obj = new JsObject();
        for (var i = 0; i < propertyCount; i++)
        {
            obj.SetString($"p{i}", $"v{i}");
        }
        return obj;
    }

    private static long MeasureAllocatedBytes(Action action)
    {
        action();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var before = GC.GetAllocatedBytesForCurrentThread();
        action();
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }
}
