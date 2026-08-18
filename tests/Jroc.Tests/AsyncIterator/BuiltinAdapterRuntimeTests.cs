using JavaScriptRuntime;

namespace Jroc.Tests.AsyncIterator;

public sealed class BuiltinAdapterRuntimeTests
{
    [Fact]
    public void AsyncIteratorPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in new[] { "next", "return", Symbol.asyncIterator.DebugId })
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.AsyncIterator.Prototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }
        });
    }

    [Fact]
    public void AsyncIteratorPrototypeNextThrowsOnIncompatibleReceiver()
    {
        WithRealm(() =>
        {
            var next = ObjectRuntime.GetItem(JavaScriptRuntime.AsyncIterator.Prototype, "next");
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
