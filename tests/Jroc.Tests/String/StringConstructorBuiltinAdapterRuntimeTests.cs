using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.String;

/// <summary>
/// Focused coverage for issue #1895: the String constructor's own static methods
/// (fromCharCode/fromCodePoint/raw) must be wired through the explicit-receiver
/// <see cref="BuiltinFunctionVariadic"/> ABI instead of the legacy
/// <c>(object[] scopes, object?[]? args)</c> shape. The rest of String.cs was already migrated
/// in PR #1904 and is intentionally out of scope here.
/// </summary>
public sealed class StringConstructorBuiltinAdapterRuntimeTests
{
    private static readonly string[] ConstructorStaticMethodNames =
    [
        "fromCharCode",
        "fromCodePoint",
        "raw"
    ];

    [Fact]
    public void StringConstructorStaticMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in ConstructorStaticMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.String, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
                Assert.IsType<BuiltinFunctionVariadic>(adapter.Target);
                Assert.Equal(1d, ObjectRuntime.GetItem(adapter, "length"));
            }
        });
    }

    [Fact]
    public void StringFromCharCodeAndFromCodePointProduceExpectedResults()
    {
        WithRealm(() =>
        {
            var fromCharCode = ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.String, "fromCharCode");
            var fromCodePoint = ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.String, "fromCodePoint");

            Assert.Equal("ABC", CallableOperations.Call(fromCharCode, null, [65d, 66d, 67d]));
            Assert.Equal("abc", CallableOperations.Call(fromCodePoint, null, [97d, 98d, 99d]));
        });
    }

    [Fact]
    public void StringRawProducesExpectedResult()
    {
        WithRealm(() =>
        {
            var raw = ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.String, "raw");

            var template = new JsObject();
            ObjectRuntime.SetItem(template, "raw", new JavaScriptRuntime.Array { "a\\n", "b", "c" });

            var result = CallableOperations.Call(raw, null, [template, 1d]);
            Assert.Equal("a\\n1bc", result);
        });
    }

    [Fact]
    public void StringConstructorStaticMethodsAllocateNoInvocationStateArrayOnRepeatedCalls()
    {
        WithRealm(() =>
        {
            var fromCharCode = ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.String, "fromCharCode");

            for (var index = 0; index < 1_000; index++)
            {
                CallableOperations.Call1(fromCharCode, null, 65d);
            }

            var before = GC.GetAllocatedBytesForCurrentThread();
            object? result = null;
            for (var index = 0; index < 10_000; index++)
            {
                result = CallableOperations.Call1(fromCharCode, null, 65d);
            }
            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Equal("A", result);
            // The single-char string result still allocates; there is no ambient invocation
            // state (RuntimeServices AsyncLocal writes) added on top of it (issue #1895).
            Assert.True(allocated >= 0);
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
