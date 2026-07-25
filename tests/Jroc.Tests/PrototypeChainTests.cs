using JavaScriptRuntime;
using System.Runtime.CompilerServices;

namespace Jroc.Tests;

public sealed class PrototypeChainTests
{
    [Fact]
    public void JsObjectInlineStorage_TreatsNullAsNoPrototype()
    {
        PrototypeChain.Enable();
        var target = new JsObject();

        PrototypeChain.InitializePrototype(target, null);

        Assert.False(PrototypeChain.TryGetPrototype(target, out _));
        Assert.Null(PrototypeChain.GetPrototypeOrNull(target));
    }

    [Fact]
    public void UninitializedJsObject_DoesNotReportExplicitNullPrototype()
    {
        PrototypeChain.Enable();
        var target = (JsObject)RuntimeHelpers.GetUninitializedObject(typeof(JsObject));

        Assert.False(PrototypeChain.TryGetPrototype(target, out _));
        Assert.Null(PrototypeChain.GetPrototypeOrNull(target));
    }

    [Fact]
    public void NonJsObjectTarget_UsesPrototypeFallback()
    {
        Func<double> target = static () => 1d;
        var prototype = new JsObject();

        PrototypeChain.SetPrototype(target, prototype);

        Assert.True(PrototypeChain.TryGetPrototype(target, out var actual));
        Assert.Same(prototype, actual);
        Assert.Same(prototype, PrototypeChain.GetPrototypeOrNull(target));
    }

    [Fact]
    public void PrototypeBearingJsObjects_DoNotAllocatePerObjectSideStorage()
    {
        const int objectCount = 10_000;
        var prototype = new JsObject();

        _ = MeasureAllocations(1, initializePrototype: false, prototype);
        _ = MeasureAllocations(1, initializePrototype: true, prototype);

        var bareAllocations = MeasureAllocations(objectCount, initializePrototype: false, prototype);
        var prototypeAllocations = MeasureAllocations(objectCount, initializePrototype: true, prototype);

        Assert.InRange(prototypeAllocations, 0, bareAllocations + objectCount * 8L);
    }

    private static long MeasureAllocations(int count, bool initializePrototype, object prototype)
    {
        var objects = new JsObject[count];
        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var index = 0; index < objects.Length; index++)
        {
            var target = new JsObject();
            if (initializePrototype)
            {
                PrototypeChain.InitializePrototype(target, prototype);
            }

            objects[index] = target;
        }

        GC.KeepAlive(objects);
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }
}
