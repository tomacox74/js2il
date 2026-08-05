using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using Jroc;
using Jroc.Runtime;

namespace Benchmarks;

/// <summary>
/// Separates arrow callable materialization from calls made after a compiled
/// module has already been loaded.
/// </summary>
[MemoryDiagnoser]
[Config(typeof(FullParamsConfig))]
[ShortRunJob]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "StdDev")]
public class CallableArchitectureBenchmarks : IDisposable
{
    private const int DirectCallsPerOperation = 1_000;
    private static readonly Func<object[], object?> ArrowTarget = static _ => 0d;

    private const string Source = """
        "use strict";

        const arrow0 = () => 0;
        const arrow1 = () => 1;
        const arrow2 = () => 2;
        const arrow3 = () => 3;
        const arrow4 = () => 4;
        const arrow5 = () => 5;
        const arrow6 = () => 6;
        const arrow7 = () => 7;
        const arrow8 = () => 8;
        const arrow9 = () => 9;
        const arrow10 = () => 10;
        const arrow11 = () => 11;
        const arrow12 = () => 12;
        const arrow13 = () => 13;

        function increment(value) {
            return value + 1;
        }

        function run(iterations) {
            let result = 0;
            for (let index = 0; index < iterations; index++) {
                result = increment(result);
            }

            return result + arrow0();
        }

        module.exports = { run };
        """;

    private JrocLoadedAssembly? _assembly;
    private IDisposable? _steadyStateExports;
    private dynamic _steadyState = null!;
    private string _moduleId = string.Empty;
    private readonly object[] _boundScopes = [new object()];

    [GlobalSetup]
    public void Setup()
    {
        var request = new JrocInMemoryCompileRequest(
            Path.Combine(Path.GetTempPath(), "jroc-callable-architecture-baseline.js"))
        {
            SourceText = Source
        };

        var artifact = JrocInMemoryCompiler.Compile(request);
        _assembly = JrocInMemoryAssemblyLoader.Load(artifact);
        _moduleId = artifact.ModuleIds.Single();
        _steadyStateExports = JsEngine.LoadModule(_assembly.Assembly, _moduleId);
        _steadyState = _steadyStateExports;
    }

    [GlobalCleanup]
    public void Dispose()
    {
        _steadyStateExports?.Dispose();
        _steadyStateExports = null;
        _steadyState = null!;
        _assembly?.Dispose();
        _assembly = null;
    }

    [Benchmark(Description = "Arrow delegate materialization")]
    public object MaterializeArrow()
        => JavaScriptRuntime.Closure.BindArrow(ArrowTarget, _boundScopes, boundThis: null);

    [Benchmark(Description = "Loaded module direct-call loop", OperationsPerInvoke = DirectCallsPerOperation)]
    public double InvokeSteadyState()
        => (double)_steadyState.run(DirectCallsPerOperation);
}
