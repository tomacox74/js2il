using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.WeakRef;

public sealed class BuiltinAdapterRuntimeTests
{
    [Fact]
    public void WeakRefPrototypeDerefUsesReceiverAwareAdapter()
    {
        WithRealm(() =>
        {
            var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(JavaScriptRuntime.WeakRef.Prototype, "deref"));

            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target));
            Assert.False(adapter.RequiresInvocationContext);
        });
    }

    [Fact]
    public void WeakRefPrototypeAdapterCanBeCalledFromAnotherRealm()
    {
        var firstRealmDeref = WithRealm(
            () => ObjectRuntime.GetItem(JavaScriptRuntime.WeakRef.Prototype, "deref")!);

        var result = WithRealm(() =>
        {
            var secondRealmDeref =
                ObjectRuntime.GetItem(JavaScriptRuntime.WeakRef.Prototype, "deref");
            Assert.NotSame(firstRealmDeref, secondRealmDeref);

            var target = new object();
            var weakRef = new JavaScriptRuntime.WeakRef(target);
            return CallableOperations.Call0(firstRealmDeref, weakRef);
        });

        Assert.NotNull(result);
    }

    [Fact]
    public void WeakRefPrototypeDerefRejectsIncompatibleReceivers()
    {
        WithRealm(() =>
        {
            var deref = ObjectRuntime.GetItem(JavaScriptRuntime.WeakRef.Prototype, "deref");
            Assert.Throws<TypeError>(
                () => CallableOperations.Call0(deref, "not a weak ref"));
        });
    }

    private static T WithRealm<T>(Func<T> body)
    {
        var context = RuntimeExecutionContext.GetOrCreate(
            RuntimeServices.BuildServiceProvider());
        using var scope = context.EnterAsRoot();
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
