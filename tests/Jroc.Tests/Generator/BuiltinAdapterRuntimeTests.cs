using JavaScriptRuntime;

namespace Jroc.Tests.Generator;

public sealed class BuiltinAdapterRuntimeTests
{
    [Fact]
    public void GeneratorPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in new[] { "next", "return", "throw" })
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(GeneratorObject.GeneratorPrototypeObject, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }
        });
    }

    [Fact]
    public void GeneratorPrototypeNextThrowsOnIncompatibleReceiver()
    {
        WithRealm(() =>
        {
            var next = ObjectRuntime.GetItem(GeneratorObject.GeneratorPrototypeObject, "next");
            Assert.Throws<TypeError>(() => CallableOperations.Call0(next, new JsObject()));
        });
    }

    private static T WithRealm<T>(Func<T> body)
    {
        var context = RuntimeExecutionContext.GetOrCreate(
            RuntimeServices.BuildServiceProvider());
        using var scope = context.EnterAsRoot();
        _ = GlobalThis.globalThis;
        return body();
    }

    private static void WithRealm(Action body)
        => WithRealm(
            () =>
            {
                body();
                return true;
            });
}
