using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class JsShapePropertyNameTests
{
    private const int PropertyCount = 256;

    private static volatile int _sink;

    [Fact]
    public void RepeatedInternalPropertyNameEnumeration_DoesNotAllocate()
    {
        var shape = CreateShape(PropertyCount);

        var allocated = MeasureAllocatedBytes(() =>
        {
            var total = 0;
            for (var iteration = 0; iteration < 1_000; iteration++)
            {
                foreach (var name in shape.PropertyNamesInSlotOrder)
                {
                    total += name.Length;
                }
            }
            _sink = total;
        });

        Assert.Equal(0, allocated);
    }

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

        Assert.InRange(allocated, 0, 4 * 1024);
    }

    [Fact]
    public void PublicKeysAndValues_DoNotAllocateShapeNameSnapshots()
    {
        var obj = CreateObject(PropertyCount);

        var allocated = MeasureAllocatedBytes(() =>
        {
            var total = 0;
            for (var iteration = 0; iteration < 10; iteration++)
            {
                foreach (var key in obj.Keys)
                {
                    total += key.Length;
                }

                foreach (var value in obj.Values)
                {
                    total += ((string)value!).Length;
                }
            }
            _sink = total;
        });

        Assert.InRange(allocated, 0, 128 * 1024);
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
    public void DeletionPreservesOrderAndAvoidsTransientPropertyNameSnapshot()
    {
        var warmup = CreateObject(PropertyCount);
        Assert.True(warmup.Remove("p128"));

        var obj = CreateObject(PropertyCount);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var before = GC.GetAllocatedBytesForCurrentThread();
        Assert.True(obj.Remove("p128"));
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(PropertyCount - 1, obj.Count);
        Assert.Equal("p127", obj.GetOwnPropertyNames().ElementAt(127));
        Assert.Equal("p129", obj.GetOwnPropertyNames().ElementAt(128));
        Assert.InRange(allocated, 0, 256 * 1024);
    }

    private static JsShape CreateShape(int propertyCount)
    {
        var shape = new JsShape();
        for (var i = 0; i < propertyCount; i++)
        {
            shape = shape.TransitionTo($"p{i}");
        }
        return shape;
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
