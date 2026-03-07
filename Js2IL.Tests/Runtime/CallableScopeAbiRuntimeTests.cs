using System.Reflection;
using JavaScriptRuntime;
using Js2IL.Runtime;

namespace Js2IL.Tests.Runtime;

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

    [Fact]
    public void Closure_InvokeWithArgs_UsesSingleScopeAttribute()
    {
        SingleScopeDelegate del = AddWithScope;

        var result = Closure.InvokeWithArgs(del, new object[] { new TestScope { BaseValue = 10 } }, 5.0);

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

        var result = Closure.InvokeWithArgs(closed, new object[] { new TestScope { BaseValue = 999 } }, 5.0);

        Assert.Equal(15.0, Convert.ToDouble(result));
    }

    [Fact]
    public void ExportMemberResolver_InvokeJsDelegate_UsesSingleScopeAttribute()
    {
        var host = new ScopedInstanceHost { BaseValue = 10 };
        InstanceSingleScopeDelegate del = host.Run;

        var result = ExportMemberResolver.InvokeJsDelegate(del, new object?[] { 5.0 });

        Assert.Equal(15.0, Convert.ToDouble(result));
    }

    [Fact]
    public void ExportMemberResolver_InvokeInstanceMethod_UsesSingleScopeAttribute()
    {
        var host = new ScopedInstanceHost { BaseValue = 10 };

        var result = ExportMemberResolver.InvokeInstanceMethod(host, nameof(ScopedInstanceHost.Run), new object?[] { 5.0 });

        Assert.Equal(15.0, Convert.ToDouble(result));
    }
}
