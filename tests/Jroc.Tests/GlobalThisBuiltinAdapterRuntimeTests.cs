using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests;

/// <summary>
/// Focused coverage for issue #1895: the remaining GlobalThis-owned static/instance builtins
/// (Array.from, Map.groupBy, Promise.resolve/try/all/race/reject, Proxy.revocable, Error.isError,
/// Error.prototype.toString) must be wired through the explicit-receiver
/// <see cref="BuiltinFunctionDelegates"/> ABI instead of ambient
/// <c>RuntimeServices.GetCurrentThis()</c> reads. Real constructors (e.g. <c>Object</c>,
/// <c>Array</c>, <c>Promise</c>) still require <c>new.target</c> and remain ambient/legacy by
/// design.
/// </summary>
public sealed class GlobalThisBuiltinAdapterRuntimeTests
{
    [Fact]
    public void ArrayFromUsesReceiverAwareAdapter()
    {
        WithRealm(() =>
        {
            var from = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(GlobalThis.Array, "from"));

            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(from.Target), "from");
            Assert.False(from.RequiresInvocationContext, "from");
            Assert.IsType<BuiltinFunction3>(from.Target);
            Assert.Equal(1d, ObjectRuntime.GetItem(from, "length"));

            var source = new JavaScriptRuntime.Array { 1d, 2d, 3d };
            var result = CallableOperations.Call1(from, null, source);
            var resultArray = Assert.IsType<JavaScriptRuntime.Array>(result);
            Assert.Equal(3, resultArray.Count);
        });
    }

    [Fact]
    public void MapGroupByUsesReceiverAwareAdapter()
    {
        WithRealm(() =>
        {
            var groupBy = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(GlobalThis.Map, "groupBy"));

            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(groupBy.Target), "groupBy");
            Assert.False(groupBy.RequiresInvocationContext, "groupBy");
            Assert.IsType<BuiltinFunction2>(groupBy.Target);
            Assert.Equal(2d, ObjectRuntime.GetItem(groupBy, "length"));
        });
    }

    [Fact]
    public void PromiseResolveAndTryUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            var resolve = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(GlobalThis.Promise, "resolve"));
            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(resolve.Target), "resolve");
            Assert.False(resolve.RequiresInvocationContext, "resolve");
            Assert.IsType<BuiltinFunction1>(resolve.Target);

            var tryMethod = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(GlobalThis.Promise, "try"));
            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(tryMethod.Target), "try");
            Assert.False(tryMethod.RequiresInvocationContext, "try");
            Assert.IsType<BuiltinFunctionVariadic>(tryMethod.Target);

            // Both read the explicit `this` (the constructor value) instead of ambient state.
            // The `this` must be the wrapped adapter identity (what a real Promise.resolve(...)
            // call site would pass), not the raw constructor delegate.
            var promiseConstructor = BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Promise);
            var resolved = CallableOperations.Call1(resolve, promiseConstructor, 42d);
            Assert.IsType<JavaScriptRuntime.Promise>(resolved);

            var callback = BuiltinDelegateFunctionAdapter.FromDelegate(
                (BuiltinFunctionVariadic)((_, in arguments) => arguments.Count > 0 ? arguments.GetArgument(0) : null));
            var tried = CallableOperations.Call2(tryMethod, promiseConstructor, callback, 7d);
            Assert.IsType<JavaScriptRuntime.Promise>(tried);
        });
    }

    [Fact]
    public void PromiseAllRaceRejectUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            var all = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(GlobalThis.Promise, "all"));
            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(all.Target), "all");
            Assert.False(all.RequiresInvocationContext, "all");
            Assert.IsType<BuiltinFunction1>(all.Target);

            var race = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(GlobalThis.Promise, "race"));
            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(race.Target), "race");
            Assert.False(race.RequiresInvocationContext, "race");
            Assert.IsType<BuiltinFunction1>(race.Target);

            var reject = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(GlobalThis.Promise, "reject"));
            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(reject.Target), "reject");
            Assert.False(reject.RequiresInvocationContext, "reject");
            Assert.IsType<BuiltinFunction1>(reject.Target);

            var source = new JavaScriptRuntime.Array { 1d, 2d };
            var promiseConstructor =
                BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Promise);
            Assert.IsType<JavaScriptRuntime.Promise>(
                CallableOperations.Call1(all, promiseConstructor, source));
            Assert.IsType<JavaScriptRuntime.Promise>(
                CallableOperations.Call1(race, promiseConstructor, source));
            Assert.IsType<JavaScriptRuntime.Promise>(
                CallableOperations.Call1(reject, promiseConstructor, "boom"));
        });
    }

    [Fact]
    public void ProxyRevocableUsesReceiverAwareAdapter()
    {
        WithRealm(() =>
        {
            var revocable = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(GlobalThis.Proxy, "revocable"));

            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(revocable.Target), "revocable");
            Assert.False(revocable.RequiresInvocationContext, "revocable");
            Assert.IsType<BuiltinFunction2>(revocable.Target);

            var target = new JsObject();
            var handler = new JsObject();
            var result = CallableOperations.Call2(revocable, null, target, handler);
            Assert.NotNull(ObjectRuntime.GetItem(result!, "proxy"));
            Assert.NotNull(ObjectRuntime.GetItem(result!, "revoke"));
        });
    }

    [Fact]
    public void ErrorIsErrorUsesReceiverAwareAdapter()
    {
        WithRealm(() =>
        {
            var isError = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(GlobalThis.Error, "isError"));

            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(isError.Target), "isError");
            Assert.False(isError.RequiresInvocationContext, "isError");
            Assert.IsType<BuiltinFunction1>(isError.Target);

            Assert.Equal(true, CallableOperations.Call1(isError, null, new JavaScriptRuntime.Error("boom")));
            Assert.Equal(false, CallableOperations.Call1(isError, null, 42d));
        });
    }

    [Fact]
    public void ErrorPrototypeToStringUsesReceiverAwareAdapter()
    {
        WithRealm(() =>
        {
            var toString = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(RuntimeIntrinsics.Current.ErrorPrototype, "toString"));

            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(toString.Target), "toString");
            Assert.False(toString.RequiresInvocationContext, "toString");
            Assert.IsType<BuiltinFunction0>(toString.Target);

            var error = new JavaScriptRuntime.Error("boom");
            var result = CallableOperations.Call0(toString, error);
            Assert.Equal("Error: boom", result);
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
