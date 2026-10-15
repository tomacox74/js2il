using JavaScriptRuntime;

namespace Jroc.Tests.Math;

public sealed class BuiltinAdapterRuntimeTests
{
    [Fact]
    public void MathMethodsUseFixedArityReceiverIndependentAdapters()
    {
        using var lifecycle = RuntimeLifecycle.Create(typeof(BuiltinAdapterRuntimeTests).Assembly, isHostedExecution: true);
        using var scope = lifecycle.EnterAsRoot();
        _ = GlobalThis.globalThis;

        foreach (var name in new[] { "abs", "ceil", "floor", "round", "sqrt", "sin", "cos", "fround", "f16round", "sumPrecise" })
        {
            var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(ObjectRuntime.GetItem(GlobalThis.Math, name));
            Assert.IsType<BuiltinFunction1>(adapter.Target);
            Assert.False(adapter.RequiresInvocationContext);
            Assert.False(adapter.IsConstructor);
            Assert.Equal(1d, ObjectRuntime.GetItem(adapter, "length"));
        }
        foreach (var name in new[] { "atan2", "pow", "imul" })
        {
            var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(ObjectRuntime.GetItem(GlobalThis.Math, name));
            Assert.IsType<BuiltinFunction2>(adapter.Target);
            Assert.False(adapter.RequiresInvocationContext);
        }
    }

    [Fact]
    public void ResolvedRoundAllocatesOnlyItsBoxedResult()
    {
        using var lifecycle = RuntimeLifecycle.Create(typeof(BuiltinAdapterRuntimeTests).Assembly, isHostedExecution: true);
        using var scope = lifecycle.EnterAsRoot();
        _ = GlobalThis.globalThis;
        var round = ObjectRuntime.GetItem(GlobalThis.Math, "round");
        object input = 1.25d;
        for (var index = 0; index < 1000; index++)
        {
            _ = CallableOperations.Call1(round, null, input);
        }

        const int iterations = 10000;
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var index = 0; index < iterations; index++)
        {
            _ = CallableOperations.Call1(round, null, input);
        }
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.InRange(allocated, 0, iterations * 32L);
    }

    [Fact]
    public void MathLookupDoesNotScanCompiledClassFields()
    {
        using var lifecycle = RuntimeLifecycle.Create(typeof(BuiltinAdapterRuntimeTests).Assembly, isHostedExecution: true);
        using var scope = lifecycle.EnterAsRoot();
        _ = GlobalThis.globalThis;
        for (var index = 0; index < 1000; index++)
        {
            _ = ObjectRuntime.GetItem(GlobalThis.Math, "round");
        }

        const int iterations = 10000;
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var index = 0; index < iterations; index++)
        {
            _ = ObjectRuntime.GetItem(GlobalThis.Math, "round");
        }
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.InRange(allocated, 0, iterations * 16L);
    }

    [Fact]
    public void RoundPreservesCoercionOrderAndDoesNotPublishAnAmbientReceiver()
    {
        using var lifecycle = RuntimeLifecycle.Create(typeof(BuiltinAdapterRuntimeTests).Assembly, isHostedExecution: true);
        using var scope = lifecycle.EnterAsRoot();
        _ = GlobalThis.globalThis;
        var ambientReceiver = new object();
        RuntimeServices.SetCurrentThis(ambientReceiver);
        var value = new JsObject();
        var calls = 0;
        value["valueOf"] = (BuiltinFunction0)(_ =>
        {
            Assert.Same(ambientReceiver, RuntimeServices.GetCurrentThis());
            calls++;
            ObjectRuntime.SetItem(GlobalThis.Math, "round", (BuiltinFunction1)((_, _) => 99d));
            return -0.5d;
        });

        var round = ObjectRuntime.GetItem(GlobalThis.Math, "round");
        var result = Assert.IsType<double>(CallableOperations.Call1(round, new object(), value));
        Assert.Equal(1, calls);
        Assert.Equal(double.NegativeInfinity, 1d / result);
        Assert.Equal(99d, ObjectRuntime.CallMember1(GlobalThis.Math, "round", 1d));
        Assert.Same(ambientReceiver, RuntimeServices.GetCurrentThis());
    }
}
