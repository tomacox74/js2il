using JavaScriptRuntime;
using Xunit;

namespace Jroc.Tests;

public sealed class ArrayReceiverSpecializationRuntimeTests
{
    [Fact]
    public void NormalizeForOfIterablePreservesHostEnumerableValues()
    {
        WithRealm(
            () =>
            {
                var source = new List<object?> { 1d, null, "host" };
                var result = Assert.IsType<JavaScriptRuntime.Array>(
                    ObjectRuntime.NormalizeForOfIterable(source));

                Assert.Equal(3d, result.length);
                Assert.Equal(1d, result[0]);
                Assert.Null(result[1]);
                Assert.Equal("host", result[2]);
            });
    }

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
                var shiftWarmup = CreateDenseArray(10_000);
                shiftWarmup.shift();
                var unshiftWarmup = CreateDenseArray(10_000, 20_000);
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

    [Fact]
    public void JoinRechecksDenseStorageAfterElementCoercion()
    {
        WithRealm(() =>
        {
            var first = new JsObject();
            var array = new JavaScriptRuntime.Array(new object?[] { first, "old", "removed" });
            ObjectRuntime.SetProperty(first, "toString",
                new BuiltinDelegateFunctionAdapter((BuiltinFunction0)(_ =>
                {
                    array.length = 2d;
                    PropertyDescriptorStore.DefineOrUpdate(array, "1", new JsPropertyDescriptor
                    {
                        Kind = JsPropertyDescriptorKind.Accessor,
                        Get = new BuiltinDelegateFunctionAdapter((BuiltinFunction0)(_ => "getter")),
                        Enumerable = true,
                        Configurable = true
                    });
                    ObjectRuntime.SetProperty(JavaScriptRuntime.Array.Prototype, "2", "inherited");
                    return "first";
                })));

            Assert.Equal("first,getter,inherited", array.join());
        });
    }

    [Fact]
    public void DenseJoinAvoidsPerElementDispatchAllocations()
    {
        WithRealm(() =>
        {
            var array = CreateDenseArray(10_000);
            ObjectRuntime.SetProperty(array, "note", "non-indexed metadata");
            var args = new object[] { "" };
            array.join(args);
            var before = GC.GetAllocatedBytesForCurrentThread();
            var result = array.join(args);
            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Equal(50_000, result.Length);
            // Allow builder growth and the result string, but not property-key
            // strings for every dense element of an array with named metadata.
            Assert.InRange(allocated, 0, 250_000);
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
