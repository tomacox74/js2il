using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Benchmarks;

[MemoryDiagnoser]
[Config(typeof(FullParamsConfig))]
[ShortRunJob]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "StdDev")]
public class AsyncContextBenchmarks
{
    private const int Operations = 1_000;
    private readonly Func<object?> _callback = static () => 1d;
    private IAsyncResource _resource = null!;
    private IAsyncLocalStorage _storage = null!;

    [GlobalSetup]
    public void Setup()
    {
        var module = new AsyncHooks();
        _resource = (IAsyncResource)CallableOperations.Construct1(
            module.AsyncResource,
            module.AsyncResource,
            "BENCHMARK")!;
        _storage = (IAsyncLocalStorage)CallableOperations.Construct0(
            module.AsyncLocalStorage,
            module.AsyncLocalStorage)!;
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
    public object? DirectCallback()
    {
        object? result = null;
        for (var index = 0; index < Operations; index++)
        {
            result = _callback();
        }

        return result;
    }

    [Benchmark(OperationsPerInvoke = Operations)]
    public object? ExplicitAsyncResource()
    {
        object? result = null;
        for (var index = 0; index < Operations; index++)
        {
            result = _resource.runInAsyncScope(_callback);
        }

        return result;
    }

    [Benchmark(OperationsPerInvoke = Operations)]
    public object? ActiveAsyncLocalStorage()
    {
        object? result = null;
        for (var index = 0; index < Operations; index++)
        {
            result = _storage.run("context", _callback);
        }

        return result;
    }
}
