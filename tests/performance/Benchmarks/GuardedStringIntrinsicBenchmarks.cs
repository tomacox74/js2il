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
public class GuardedStringIntrinsicBenchmarks
{
    private const string Receiver = "value";
    private const string MethodName = "trim";
    private readonly object _uncertainReceiver = Receiver;
    private object _stringObjectReceiver = null!;
    private RuntimeAgentCluster? _cluster;
    private IDisposable? _scope;

    [GlobalSetup]
    public void Setup()
    {
        var services = RuntimeServices.BuildServiceProvider();
        _cluster = services.OwningRealm!.Agent.Cluster;
        var context = RuntimeExecutionContext.GetOrCreate(services);
        _scope = context.EnterAsRoot();
        _ = GlobalThis.globalThis;
        _stringObjectReceiver =
            JavaScriptRuntime.String.Construct(
                [Receiver],
                newTarget: null);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _scope?.Dispose();
        _cluster?.Dispose();
    }

    [Benchmark(
        Baseline = true,
        Description = "CallMember0 String.trim")]
    public object? GenericCallMember()
        => ObjectRuntime.CallMember0(Receiver, MethodName);

    [Benchmark(Description = "Guarded proven String.trim")]
    public object? GuardedProvenString()
    {
        if (IntrinsicPrototypeEpochs.IsPristine(
                IntrinsicPrototypeFamily.String))
        {
            return JavaScriptRuntime.String.Trim(Receiver);
        }

        return ObjectRuntime.CallMember0(Receiver, MethodName);
    }

    [Benchmark(Description = "Guarded uncertain String.trim")]
    public object? GuardedUncertainReceiver()
    {
        if (IntrinsicPrototypeEpochs.IsPristine(
                IntrinsicPrototypeFamily.String)
            && _uncertainReceiver is string input)
        {
            return JavaScriptRuntime.String.Trim(input);
        }

        return ObjectRuntime.CallMember0(
            _uncertainReceiver,
            MethodName);
    }

    [Benchmark(Description = "CallMember0 String-object.trim")]
    public object? GenericStringObjectCallMember()
        => ObjectRuntime.CallMember0(
            _stringObjectReceiver,
            MethodName);

    [Benchmark(Description = "Guarded String-object.trim")]
    public object? GuardedStringObjectReceiver()
    {
        if (IntrinsicPrototypeEpochs.IsPristine(
                IntrinsicPrototypeFamily.String))
        {
            if (_stringObjectReceiver is string input)
            {
                return JavaScriptRuntime.String.Trim(input);
            }

            var unwrapped = IntrinsicPrototypeEpochs
                .TryUnwrapStringObjectReceiver(
                    _stringObjectReceiver,
                    MethodName);
            if (unwrapped != null)
            {
                return JavaScriptRuntime.String.Trim(unwrapped);
            }
        }

        return ObjectRuntime.CallMember0(
            _stringObjectReceiver,
            MethodName);
    }
}
