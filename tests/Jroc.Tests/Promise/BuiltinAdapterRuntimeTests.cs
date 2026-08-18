using JavaScriptRuntime;

namespace Jroc.Tests.Promise;

public sealed class BuiltinAdapterRuntimeTests
{
    [Fact]
    public void PromisePrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in new[] { "then", "catch", "finally" })
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.Promise.Prototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }
        });
    }

    [Fact]
    public void PromisePrototypeThenThrowsOnIncompatibleReceiver()
    {
        WithRealm(() =>
        {
            var then = ObjectRuntime.GetItem(JavaScriptRuntime.Promise.Prototype, "then");
            Assert.Throws<TypeError>(() => CallableOperations.Call0(then, new JsObject()));
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
