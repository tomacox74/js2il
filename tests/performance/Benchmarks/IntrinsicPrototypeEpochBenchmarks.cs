using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using JavaScriptRuntime;

namespace Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "StdDev")]
public class IntrinsicPrototypeEpochBenchmarks
{
    private RuntimeAgentCluster? _cluster;
    private IDisposable? _scope;
    private object _stringPrototype = null!;

    [GlobalSetup]
    public void Setup()
    {
        var services = RuntimeServices.BuildServiceProvider();
        _cluster = services.OwningRealm!.Agent.Cluster;
        var context = RuntimeExecutionContext.GetOrCreate(services);
        _scope = context.EnterAsRoot();
        _ = GlobalThis.globalThis;
        _stringPrototype = JavaScriptRuntime.String.Prototype;
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _scope?.Dispose();
        _cluster?.Dispose();
    }

    [Benchmark(Baseline = true, Description = "Read String prototype epoch")]
    public long ReadStringEpoch()
        => IntrinsicPrototypeEpochs.Read(
            IntrinsicPrototypeFamily.String);

    [Benchmark(Description = "Validate pristine String prototype epoch")]
    public bool ValidatePristineStringEpoch()
        => IntrinsicPrototypeEpochs.IsPristine(
            IntrinsicPrototypeFamily.String);

    [Benchmark(Description = "Invalidate String prototype epoch")]
    public void InvalidateStringEpoch()
        => RuntimeIntrinsics.NotifyPrototypeMutation(
            _stringPrototype);
}
