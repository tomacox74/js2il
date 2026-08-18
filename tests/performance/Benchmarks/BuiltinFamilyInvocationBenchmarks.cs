using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;
using JavaScriptRuntime.Node;

namespace Benchmarks;

/// <summary>
/// Measures representative runtime-owned built-in adapters migrated by issue #1895.
/// Property resolution happens during setup so the benchmark isolates invocation ABI cost.
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "StdDev")]
public class BuiltinFamilyInvocationBenchmarks
{
    private readonly object _arrayElement = new();
    private readonly object _mapValue = new();
    private IDisposable? _realmScope;
    private object _arrayAt = null!;
    private JavaScriptRuntime.Array _array = null!;
    private object _regExpTest = null!;
    private RegExp _regExp = null!;
    private object _mapGet = null!;
    private JavaScriptRuntime.Map _map = null!;
    private object _urlSearchParamsGet = null!;
    private URLSearchParams _urlSearchParams = null!;

    [GlobalSetup]
    public void Setup()
    {
        var context = RuntimeExecutionContext.GetOrCreate(
            RuntimeServices.BuildServiceProvider());
        _realmScope = context.EnterAsRoot();
        context.GetOrCreateGlobalObject();

        _array = new JavaScriptRuntime.Array { _arrayElement };
        _arrayAt = GetCallable(_array, "at");

        _regExp = new RegExp("a");
        _regExpTest = GetCallable(_regExp, "test");

        _map = new JavaScriptRuntime.Map();
        _map.set("key", _mapValue);
        _mapGet = GetCallable(_map, "get");

        _urlSearchParams = new URLSearchParams("key=value");
        _urlSearchParamsGet = GetCallable(_urlSearchParams, "get");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _realmScope?.Dispose();
        _realmScope = null;
    }

    [Benchmark(Description = "Array.prototype.at adapter")]
    public object ArrayAt()
        => CallableOperations.Call1(_arrayAt, _array, 0d)!;

    [Benchmark(Description = "RegExp.prototype.test adapter")]
    public object RegExpTest()
        => CallableOperations.Call1(_regExpTest, _regExp, "a")!;

    [Benchmark(Description = "Map.prototype.get adapter")]
    public object MapGet()
        => CallableOperations.Call1(_mapGet, _map, "key")!;

    [Benchmark(Description = "URLSearchParams.prototype.get adapter")]
    public object UrlSearchParamsGet()
        => CallableOperations.Call1(_urlSearchParamsGet, _urlSearchParams, "key")!;

    private static object GetCallable(object receiver, string methodName)
    {
        var value = ObjectRuntime.GetItem(receiver, methodName);
        if (!CallableOperations.IsCallable(value))
        {
            throw new InvalidOperationException($"{methodName} is not callable.");
        }

        return value!;
    }
}
