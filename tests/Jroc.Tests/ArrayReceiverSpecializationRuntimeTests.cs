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
