using BenchmarkDotNet.Attributes;
using JavaScriptRuntime;

namespace Benchmarks;

[MemoryDiagnoser]
[SimpleJob(launchCount: 1, warmupCount: 3, iterationCount: 10)]
public class RegExpDispatchBenchmarks
{
    private IDisposable _scope = null!;
    private RegExp _expression = null!;

    [GlobalSetup]
    public void Setup()
    {
        var context = RuntimeExecutionContext.GetOrCreate(RuntimeServices.BuildServiceProvider());
        _scope = context.EnterAsRoot();
        context.GetOrCreateGlobalObject();
        _expression = new RegExp("a[bc]+d", "");
    }

    [GlobalCleanup]
    public void Cleanup() => _scope.Dispose();

    [Benchmark]
    public object? Test1() => ObjectRuntime.CallMember1(_expression, "test", "abbd");

    [Benchmark]
    public object? Test2() => ObjectRuntime.CallMember2(_expression, "test", "abbd", "ignored");

    [Benchmark]
    public object? ExecMiss1() => ObjectRuntime.CallMember1(_expression, "exec", "miss");

    [Benchmark]
    public object? ExecMiss2() => ObjectRuntime.CallMember2(_expression, "exec", "miss", "ignored");

    [Benchmark]
    public object? ExecMatch1() => ObjectRuntime.CallMember1(_expression, "exec", "abbd");

    [Benchmark]
    public object? ExecMatch2() => ObjectRuntime.CallMember2(_expression, "exec", "abbd", "ignored");
}
