using JavaScriptRuntime;
using Xunit;

namespace Jroc.Tests;

public sealed class ArrayReceiverSpecializationRuntimeTests
{
    [Fact]
    public void HelpersPreserveArrayAndFallbackSemantics()
    {
        WithRealm(
            () =>
            {
                var array = new JavaScriptRuntime.Array(
                    new object?[] { "dense" });
                array.length = 2d;
                ObjectRuntime.SetProperty(
                    JavaScriptRuntime.Array.Prototype,
                    "1",
                    "prototype");

                Assert.Equal(
                    2d,
                    ObjectRuntime.GetArrayLengthWithFallback(
                        array));
                Assert.Equal(
                    "dense",
                    ObjectRuntime.GetArrayElementWithFallback(
                        array,
                        0d));
                Assert.Equal(
                    "prototype",
                    ObjectRuntime.GetArrayElementWithFallback(
                        array,
                        1d));

                var ordinary = new JsObject();
                ObjectRuntime.SetProperty(
                    ordinary,
                    "length",
                    1d);
                ObjectRuntime.SetProperty(
                    ordinary,
                    "0",
                    "ordinary");

                Assert.Equal(
                    1d,
                    ObjectRuntime.GetArrayLengthWithFallback(
                        ordinary));
                Assert.Equal(
                    "ordinary",
                    ObjectRuntime.GetArrayElementWithFallback(
                        ordinary,
                        0d));
            });
    }

    [Fact]
    public void ArrayLengthFastPathAllocatesNothing()
    {
        WithRealm(
            () =>
            {
                var array = new JavaScriptRuntime.Array(
                    new object?[] { 1d, 2d, 3d });
                var result = 0d;
                for (var index = 0; index < 32; index++)
                {
                    result +=
                        ObjectRuntime
                            .GetArrayLengthWithFallback(array);
                }

                var before =
                    GC.GetAllocatedBytesForCurrentThread();
                for (var index = 0; index < 10_000; index++)
                {
                    result +=
                        ObjectRuntime
                            .GetArrayLengthWithFallback(array);
                }
                var allocated =
                    GC.GetAllocatedBytesForCurrentThread() - before;

                GC.KeepAlive(result);
                Assert.Equal(0, allocated);
            });
    }

    [Fact]
    public void DenseFrontMutationsAvoidGenericPropertyDispatchAllocations()
    {
        WithRealm(
            () =>
            {
                var shiftWarmup = CreateDenseArray(128);
                shiftWarmup.shift();
                var unshiftWarmup = CreateDenseArray(128, 256);
                unshiftWarmup.unshift("first");

                var shiftArray = CreateDenseArray(10_000);
                var beforeShift =
                    GC.GetAllocatedBytesForCurrentThread();
                var shifted = shiftArray.shift();
                var shiftAllocated =
                    GC.GetAllocatedBytesForCurrentThread() - beforeShift;

                var unshiftArray = CreateDenseArray(10_000, 20_000);
                var beforeUnshift =
                    GC.GetAllocatedBytesForCurrentThread();
                var newLength = unshiftArray.unshift("first");
                var unshiftAllocated =
                    GC.GetAllocatedBytesForCurrentThread() - beforeUnshift;

                Assert.Equal("value", shifted);
                Assert.Equal(9_999d, shiftArray.length);
                Assert.InRange(shiftAllocated, 0, 8_192);
                Assert.Equal(10_001d, newLength);
                Assert.Equal("first", unshiftArray[0]);
                Assert.InRange(unshiftAllocated, 0, 8_192);
            });
    }

    private static JavaScriptRuntime.Array CreateDenseArray(
        int count,
        int? capacity = null)
    {
        var array = new JavaScriptRuntime.Array(capacity ?? count);
        for (var index = 0; index < count; index++)
        {
            array.Add("value");
        }

        return array;
    }

    private static void WithRealm(Action body)
    {
        var services = RuntimeServices.BuildServiceProvider();
        var context =
            RuntimeExecutionContext.GetOrCreate(services);
        try
        {
            using var scope = context.EnterAsRoot();
            body();
        }
        finally
        {
            services.OwningRealm!.Agent.Cluster.Dispose();
        }
    }
}
