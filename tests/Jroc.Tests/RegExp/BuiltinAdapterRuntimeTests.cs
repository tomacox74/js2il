using System.Linq;
using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.RegExp;

public sealed class BuiltinAdapterRuntimeTests
{
    private static readonly string[] PrototypeMethodNames =
    [
        "exec",
        "test",
        "toString"
    ];

    private static readonly string[] PrototypeGetterNames =
    [
        "dotAll",
        "flags",
        "global",
        "hasIndices",
        "ignoreCase",
        "multiline",
        "source",
        "sticky",
        "unicode",
        "unicodeSets"
    ];

    private static readonly string[] PrototypeSymbolMethodNames =
    [
        Symbol.match.DebugId,
        Symbol.matchAll.DebugId,
        Symbol.replace.DebugId,
        Symbol.search.DebugId,
        Symbol.split.DebugId
    ];

    [Fact]
    public void RegExpPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in PrototypeMethodNames.Concat(PrototypeSymbolMethodNames))
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.RegExp.Prototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }

            foreach (var getterName in PrototypeGetterNames)
            {
                Assert.True(
                    PropertyDescriptorStore.TryGetOwn(JavaScriptRuntime.RegExp.Prototype, getterName, out var descriptor),
                    getterName);
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(descriptor.Get);

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), getterName);
                Assert.False(adapter.RequiresInvocationContext, getterName);
            }

            var iteratorNext = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(JavaScriptRuntime.RegExp.RegExpStringIteratorPrototype, "next"));
            var iteratorSelf = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(
                    JavaScriptRuntime.RegExp.RegExpStringIteratorPrototype,
                    Symbol.iterator.DebugId));
            Assert.False(iteratorNext.RequiresInvocationContext);
            Assert.False(iteratorSelf.RequiresInvocationContext);
        });
    }

    [Fact]
    public void RegExpPrototypeAdapterCanBeCalledFromAnotherRealm()
    {
        var firstRealmExec = WithRealm(
            () => ObjectRuntime.GetItem(JavaScriptRuntime.RegExp.Prototype, "exec")!);

        var result = WithRealm(() =>
        {
            var secondRealmExec =
                ObjectRuntime.GetItem(JavaScriptRuntime.RegExp.Prototype, "exec");
            Assert.NotSame(firstRealmExec, secondRealmExec);

            var regExp = new JavaScriptRuntime.RegExp("b", "");
            return CallableOperations.Call1(firstRealmExec, regExp, "abc");
        });

        var match = Assert.IsType<JavaScriptRuntime.Array>(result);
        Assert.Equal("b", match[0]);
    }

    [Fact]
    public void RegExpPrototypeMethodsRejectIncompatibleReceivers()
    {
        WithRealm(() =>
        {
            var exec = ObjectRuntime.GetItem(JavaScriptRuntime.RegExp.Prototype, "exec");
            Assert.Throws<TypeError>(
                () => CallableOperations.Call1(exec, "not a regexp", "abc"));
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
