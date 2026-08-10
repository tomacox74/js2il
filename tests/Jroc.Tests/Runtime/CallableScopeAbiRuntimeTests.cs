using System.Reflection;
using JavaScriptRuntime;
using Jroc.Runtime;

namespace Jroc.Tests.Runtime;

public class CallableScopeAbiRuntimeTests
{
    private delegate object? SingleScopeDelegate(TestScope scope, object? newTarget, object? addend);
    private delegate object? InstanceSingleScopeDelegate(ScopedInstanceHost scope, object? newTarget, object? addend);

    private sealed class TestScope
    {
        public double BaseValue { get; init; }
    }

    private sealed class ScopedInstanceHost
    {
        public double BaseValue { get; init; }

        [JsCallableScopeAbi(CallableScopeAbiKind.SingleScope, SingleScopeType = typeof(ScopedInstanceHost))]
        public object? Run(ScopedInstanceHost scope, object? newTarget, object? addend)
        {
            return scope.BaseValue + Convert.ToDouble(addend);
        }
    }

    [JsCallableScopeAbi(CallableScopeAbiKind.SingleScope, SingleScopeType = typeof(TestScope))]
    private static object? AddWithScope(TestScope scope, object? newTarget, object? addend)
    {
        return scope.BaseValue + Convert.ToDouble(addend);
    }

    [JsCallableScopeAbi(CallableScopeAbiKind.SingleScope, SingleScopeType = typeof(TestScope))]
    private static object? ConstructWithScope(
        TestScope scope,
        object? newTarget,
        object? addend)
    {
        return new JsObject
        {
            ["value"] = scope.BaseValue + Convert.ToDouble(addend),
            ["newTarget"] = newTarget
        };
    }

    [Fact]
    public void Closure_InvokeWithArgs_UsesSingleScopeAttribute()
    {
        SingleScopeDelegate del = AddWithScope;
        var adapter = new BuiltinDelegateFunctionAdapter(
            del,
            new object[] { new TestScope { BaseValue = 10 } });

        var result = Closure.InvokeWithArgs(
            adapter,
            RuntimeServices.EmptyScopes,
            5.0);

        Assert.Equal(15.0, Convert.ToDouble(result));
    }

    [Fact]
    public void Closure_InvokeWithArgs_TreatsClosedSingleScopeDelegate_AsNoScopes()
    {
        var scope = new TestScope { BaseValue = 10 };
        var method = typeof(CallableScopeAbiRuntimeTests).GetMethod(
            nameof(AddWithScope),
            BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Expected AddWithScope method.");
        var closed = (JsFuncNoScopes1)Delegate.CreateDelegate(typeof(JsFuncNoScopes1), scope, method);
        var adapter = new BuiltinDelegateFunctionAdapter(closed);

        var result = Closure.InvokeWithArgs(
            adapter,
            new object[] { new TestScope { BaseValue = 999 } },
            5.0);

        Assert.Equal(15.0, Convert.ToDouble(result));
    }

    [Fact]
    public void BuiltinDelegateFunctionAdapter_UsesSingleScopeAttribute()
    {
        var host = new ScopedInstanceHost { BaseValue = 10 };
        InstanceSingleScopeDelegate del = host.Run;
        var adapter = new BuiltinDelegateFunctionAdapter(
            del,
            new object[] { host });

        var result = CallableOperations.Call(
            adapter,
            null,
            new object?[] { 5.0 });

        Assert.Equal(15.0, Convert.ToDouble(result));
    }

    [Fact]
    public void BuiltinDelegateFunctionAdapter_ConstructPreservesCapturedScopesAndNewTarget()
    {
        SingleScopeDelegate del = ConstructWithScope;
        var adapter = new BuiltinDelegateFunctionAdapter(
            del,
            new object[] { new TestScope { BaseValue = 10 } });
        JavaScriptRuntime.Function.MarkConstructible(adapter);
        var newTarget = new JsObject();

        var result = Assert.IsType<JsObject>(
            CallableOperations.Construct(
                adapter,
                new object?[] { 5.0 },
                newTarget));

        Assert.Equal(15.0, result["value"]);
        Assert.Same(newTarget, result["newTarget"]);
    }
}
