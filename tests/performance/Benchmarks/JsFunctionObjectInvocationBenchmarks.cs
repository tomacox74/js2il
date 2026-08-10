using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using JavaScriptRuntime;

namespace Benchmarks;

/// <summary>
/// Compares the compiled function-object ABI with the intentional built-in
/// delegate adapter boundary.
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "StdDev")]
public class JsFunctionObjectInvocationBenchmarks
{
    private static readonly object Argument0 = new();
    private static readonly object Argument1 = new();
    private static readonly object Argument2 = new();
    private static readonly object?[] ArbitraryArguments = [Argument0, Argument1, Argument2];

    private readonly BenchmarkFunction _functionObject = new();
    private readonly ContextualBenchmarkFunction _contextualFunctionObject = new();
    private readonly JsFuncNoScopes3 _builtinDelegate =
        static (_, _, _, argument2) => argument2;
    private BuiltinDelegateFunctionAdapter _builtinAdapter = null!;

    [GlobalSetup]
    public void Setup()
    {
        Function.InitializeFunctionInstance(
            _builtinDelegate,
            3d,
            "builtin",
            requiresInvocationContext: false);
        _builtinAdapter =
            BuiltinDelegateFunctionAdapter.FromDelegate(_builtinDelegate);
    }

    [Benchmark(Baseline = true, Description = "Built-in delegate adapter fixed arity 3")]
    public object BuiltinDelegateAdapterFixed3()
        => Closure.InvokeWithArgs3(
            _builtinAdapter,
            RuntimeServices.EmptyScopes,
            Argument0,
            Argument1,
            Argument2);

    [Benchmark(Description = "JsFunctionObject fixed arity 3")]
    public object FunctionObjectFixed3()
        => CallableOperations.Call3(
            _functionObject,
            null,
            Argument0,
            Argument1,
            Argument2)!;

    [Benchmark(Description = "JsFunctionObject pre-materialized arbitrary args")]
    public object FunctionObjectArbitrary()
        => CallableOperations.Call(_functionObject, null, ArbitraryArguments)!;

    [Benchmark(Description = "JsFunctionObject ambient-context fixed arity 3")]
    public object FunctionObjectAmbientContextFixed3()
        => CallableOperations.Call3(
            _contextualFunctionObject,
            null,
            Argument0,
            Argument1,
            Argument2)!;

    [Benchmark(Description = "JsFunctionObject spread materialization")]
    public object FunctionObjectSpreadMaterialization()
        => CallableOperations.Call(
            _functionObject,
            null,
            new object?[] { Argument0, Argument1, Argument2 })!;

    private sealed class BenchmarkFunction : JsFunctionObject
    {
        public override bool RequiresInvocationContext => false;

        protected override object? CallCore(
            object? thisArgument,
            in JsCallArguments arguments)
            => arguments.GetArgument(2);
    }

    private sealed class ContextualBenchmarkFunction : JsFunctionObject
    {
        protected override object? CallCore(
            object? thisArgument,
            in JsCallArguments arguments)
            => arguments.GetArgument(2);
    }
}
