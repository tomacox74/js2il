using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.Proxy;

/// <summary>
/// Verifies the Proxy.revocable() `revoke` closure migrated to the explicit-receiver
/// BuiltinFunction0 ABI (issue #1895) no longer requires ambient invocation state.
/// </summary>
public sealed class BuiltinAdapterRuntimeTests
{
    [Fact]
    public void RevocableRevokeUsesReceiverAwareAdapter()
    {
        WithRealm(() =>
        {
            var target = new JsObject();
            var handler = new JsObject();
            var revocable = JavaScriptRuntime.Proxy.revocable(target, handler);

            var revoke = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(revocable, "revoke"));

            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(revoke.Target), "revoke");
            Assert.False(revoke.RequiresInvocationContext, "revoke");
            Assert.IsType<BuiltinFunction0>(revoke.Target);
        });
    }

    [Fact]
    public void RevocableRevokeRevokesTheProxyWhenCalled()
    {
        WithRealm(() =>
        {
            var target = new JsObject();
            ObjectRuntime.SetItem(target, "prop", 1d);
            var handler = new JsObject();
            var revocableResult = JavaScriptRuntime.Proxy.revocable(target, handler);

            var proxy = ObjectRuntime.GetItem(revocableResult, "proxy");
            var revoke = ObjectRuntime.GetItem(revocableResult, "revoke");

            // Proxy still usable before revocation.
            Assert.Equal(1d, ObjectRuntime.GetItem(proxy!, "prop"));

            // Explicit-receiver call: `this` argument is intentionally ignored by revoke.
            var result = CallableOperations.Call0(revoke, null);
            Assert.Null(result);

            Assert.Throws<TypeError>(() => ObjectRuntime.GetItem(proxy!, "prop"));
        });
    }

    [Fact]
    public void RevocableRevokeAllocatesNoInvocationStateOnRepeatedCalls()
    {
        WithRealm(() =>
        {
            var target = new JsObject();
            var handler = new JsObject();
            var revocableResult = JavaScriptRuntime.Proxy.revocable(target, handler);
            var revoke = ObjectRuntime.GetItem(revocableResult, "revoke");

            for (var index = 0; index < 1_000; index++)
            {
                CallableOperations.Call0(revoke, null);
            }

            var before = GC.GetAllocatedBytesForCurrentThread();
            for (var index = 0; index < 10_000; index++)
            {
                CallableOperations.Call0(revoke, null);
            }
            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Equal(0, allocated);
        });
    }

    private static T WithRealm<T>(Func<T> body)
    {
        var context = RuntimeExecutionContext.GetOrCreate(
            RuntimeServices.BuildServiceProvider());
        using var scope = context.EnterAsRoot();
        context.GetOrCreateGlobalObject();
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
