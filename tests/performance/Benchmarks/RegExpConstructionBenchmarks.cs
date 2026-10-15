using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using JavaScriptRuntime;

namespace Benchmarks;

[MemoryDiagnoser]
[SimpleJob(launchCount: 1, warmupCount: 5, iterationCount: 15)]
public class RegExpConstructionBenchmarks
{
    private IDisposable _scope = null!;
    private int _nextPattern;
    private readonly string _hotPattern = "a[bc]+d";

    [GlobalSetup]
    public void Setup()
    {
        _scope = RuntimeExecutionContext.GetOrCreate(RuntimeServices.BuildServiceProvider()).EnterAsRoot();
        _ = BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.RegExp);
    }

    [GlobalCleanup]
    public void Cleanup() => _scope.Dispose();

    [Benchmark(OperationsPerInvoke = 1_000)]
    public RegExp ColdUnique()
    {
        RegExp result = null!;
        for (var index = 0; index < 1_000; index++)
        {
            result = new RegExp($"a[bc]+d{_nextPattern++}", "");
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = 1_000)]
    public RegExp HotDynamic()
    {
        RegExp result = null!;
        for (var index = 0; index < 1_000; index++)
        {
            result = new RegExp(_hotPattern, "");
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = 1_000)]
    public RegExp HotLiteral()
    {
        RegExp result = null!;
        for (var index = 0; index < 1_000; index++)
        {
            result = RegExp.CreateLiteral(_hotPattern, "");
        }
        return result;
    }
}
