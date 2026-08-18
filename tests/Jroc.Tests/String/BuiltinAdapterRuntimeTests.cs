using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.String;

public sealed class BuiltinAdapterRuntimeTests
{
    private static readonly string[] PrototypeMethodNames =
    [
        "at",
        "charAt",
        "charCodeAt",
        "codePointAt",
        "concat",
        "endsWith",
        "includes",
        "indexOf",
        "isWellFormed",
        "lastIndexOf",
        "localeCompare",
        "match",
        "matchAll",
        "normalize",
        "padEnd",
        "padStart",
        "repeat",
        "replace",
        "replaceAll",
        "search",
        "slice",
        "split",
        "startsWith",
        "substr",
        "substring",
        "toLowerCase",
        "toLocaleLowerCase",
        "toLocaleUpperCase",
        "toString",
        "toUpperCase",
        "toWellFormed",
        "trim",
        "trimEnd",
        "trimLeft",
        "trimRight",
        "trimStart",
        "valueOf",
        Symbol.iterator.DebugId
    ];

    [Fact]
    public void StringPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in PrototypeMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.String.Prototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }

            var concat = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(JavaScriptRuntime.String.Prototype, "concat"));
            Assert.IsType<BuiltinFunctionVariadic>(concat.Target);

            var iteratorNext = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(JavaScriptRuntime.String.StringIteratorPrototype, "next"));
            var iteratorSelf = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(
                    JavaScriptRuntime.String.StringIteratorPrototype,
                    Symbol.iterator.DebugId));
            Assert.False(iteratorNext.RequiresInvocationContext);
            Assert.False(iteratorSelf.RequiresInvocationContext);
        });
    }

    [Fact]
    public void StringPrototypeAliasesShareTheirCanonicalAdapters()
    {
        WithRealm(() =>
        {
            Assert.Same(
                ObjectRuntime.GetItem(JavaScriptRuntime.String.Prototype, "trimStart"),
                ObjectRuntime.GetItem(JavaScriptRuntime.String.Prototype, "trimLeft"));
            Assert.Same(
                ObjectRuntime.GetItem(JavaScriptRuntime.String.Prototype, "trimEnd"),
                ObjectRuntime.GetItem(JavaScriptRuntime.String.Prototype, "trimRight"));
        });
    }

    [Fact]
    public void StringPrototypeCommonAritiesAllocateNoInvocationState()
    {
        WithRealm(() =>
        {
            const string receiver = "abc";
            object argument = 1d;
            var trim = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(JavaScriptRuntime.String.Prototype, "trim"));

            for (var index = 0; index < 1_000; index++)
            {
                InvokeTrim(index % 6);
            }

            var before = GC.GetAllocatedBytesForCurrentThread();
            object? result = null;
            for (var index = 0; index < 10_000; index++)
            {
                for (var arity = 0; arity <= 5; arity++)
                {
                    result = InvokeTrim(arity);
                }
            }
            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Same(receiver, result);
            Assert.Equal(0, allocated);

            object? InvokeTrim(int arity)
                => arity switch
                {
                    0 => CallableOperations.Call0(trim, receiver),
                    1 => CallableOperations.Call1(trim, receiver, argument),
                    2 => CallableOperations.Call2(trim, receiver, argument, argument),
                    3 => CallableOperations.Call3(trim, receiver, argument, argument, argument),
                    4 => CallableOperations.Call4(trim, receiver, argument, argument, argument, argument),
                    5 => CallableOperations.Call5(trim, receiver, argument, argument, argument, argument, argument),
                    _ => throw new ArgumentOutOfRangeException(nameof(arity))
                };
        });
    }

    [Fact]
    public void StringPrototypeAdapterCanBeCalledFromAnotherRealm()
    {
        var firstRealmSlice = WithRealm(
            () => ObjectRuntime.GetItem(JavaScriptRuntime.String.Prototype, "slice")!);

        var result = WithRealm(() =>
        {
            var secondRealmSlice =
                ObjectRuntime.GetItem(JavaScriptRuntime.String.Prototype, "slice");
            Assert.NotSame(firstRealmSlice, secondRealmSlice);

            return CallableOperations.Call2(
                firstRealmSlice,
                "abcdef",
                1d,
                4d);
        });

        Assert.Equal("bcd", result);
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
