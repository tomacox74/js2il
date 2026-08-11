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

        const dynamicArgument1 = { value: 1 };
        const dynamicArgument2 = { value: 2 };
        const dynamicArgument3 = { value: 3 };
        const dynamicArgument4 = { value: 4 };
        const dynamicArgument5 = { value: 5 };
        const dynamicArgument6 = { value: 6 };

        function selectFifth(a, b, c, d, e) {
            return e.value;
        }

        function runDynamicFive(iterations) {
            let target = selectFifth;
            let result = 0;
            for (let index = 0; index < iterations; index++) {
                result = target(
                    dynamicArgument1,
                    dynamicArgument2,
                    dynamicArgument3,
                    dynamicArgument4,
                    dynamicArgument5);
            }
            return result;
        }

        function runDynamicSix(iterations) {
            let target = selectFifth;
            let result = 0;
            for (let index = 0; index < iterations; index++) {
                result = target(
                    dynamicArgument1,
                    dynamicArgument2,
                    dynamicArgument3,
                    dynamicArgument4,
                    dynamicArgument5,
                    dynamicArgument6);
            }
            return result;
        }

        module.exports = { run, runDynamicFive, runDynamicSix };
        """;

    private JrocLoadedAssembly? _assembly;
    private IDisposable? _steadyStateExports;
    private dynamic _steadyState = null!;
    private string _moduleId = string.Empty;
    private readonly object[] _capturedScopes = [new object()];

    [GlobalSetup]
    public void Setup()
    {
        var request = new JrocInMemoryCompileRequest(
            Path.GetFullPath(
                Path.Combine(
                    "BenchmarkDotNet.Artifacts",
                    "jroc-callable-architecture-baseline.js")))
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

    [Benchmark(Description = "Generated arrow object materialization")]
    public object MaterializeGeneratedArrowObject()
        => new BenchmarkArrowFunctionObject(_capturedScopes);

    [Benchmark(Description = "Repeated compiled module load")]
    public void LoadCompiledModule()
    {
        using var exports = JsEngine.LoadModule(
            _assembly!.Assembly,
            _moduleId);
        GC.KeepAlive(exports);
    }

    [Benchmark(Description = "Loaded module direct-call loop", OperationsPerInvoke = DirectCallsPerOperation)]
    public double InvokeSteadyState()
        => (double)_steadyState.run(DirectCallsPerOperation);

    [Benchmark(
        Description = "Loaded module dynamic five-argument loop",
        OperationsPerInvoke = DirectCallsPerOperation)]
    public double InvokeDynamicFiveArguments()
        => (double)_steadyState.runDynamicFive(DirectCallsPerOperation);

    [Benchmark(
        Description = "Loaded module dynamic six-argument loop",
        OperationsPerInvoke = DirectCallsPerOperation)]
    public double InvokeDynamicSixArguments()
        => (double)_steadyState.runDynamicSix(DirectCallsPerOperation);

    private sealed class BenchmarkArrowFunctionObject(object[] scopes) : JavaScriptRuntime.JsFunctionObject
    {
        private readonly object[] _scopes = scopes;

        public override bool RequiresInvocationContext => false;

        protected override object? CallCore(
            object? thisArgument,
            in JavaScriptRuntime.JsCallArguments arguments)
            => _scopes.Length;
    }
}
