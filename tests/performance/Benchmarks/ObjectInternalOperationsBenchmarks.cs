using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using Jroc;

namespace Benchmarks;

/// <summary>
/// Tracks the steady-state cost of ordinary-object reads and writes through the
/// generic runtime dispatch and through the hosted C# dynamic boundary.
/// </summary>
[MemoryDiagnoser]
[Config(typeof(FullParamsConfig))]
[ShortRunJob]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "StdDev")]
public class ObjectInternalOperationsBenchmarks : IDisposable
{
    private readonly JavaScriptRuntime.JsObject _readTarget = new();
    private readonly JavaScriptRuntime.JsObject _writeTarget = new();
    private JrocInMemoryModule? _hostedModule;
    private dynamic _hostedTarget = null!;

    [GlobalSetup]
    public void Setup()
    {
        JavaScriptRuntime.Object.SetProperty(_readTarget, "value", 42d);
        JavaScriptRuntime.Object.SetProperty(_writeTarget, "value", 0d);

        var request = new JrocInMemoryCompileRequest(
            Path.Combine(Path.GetTempPath(), "jroc-hosted-object-operations.js"))
        {
            SourceText = "\"use strict\"; module.exports = { target: { value: 42 } };"
        };
        _hostedModule = JrocInMemoryCompiler.CompileAndLoadModule(request);
        dynamic exports = _hostedModule.Exports;
        _hostedTarget = exports.target;
    }

    [GlobalCleanup]
    public void Dispose() => _hostedModule?.Dispose();

    [Benchmark(Description = "Ordinary JsObject read")]
    public object? Read()
        => JavaScriptRuntime.Object.GetProperty(_readTarget, "value");

    [Benchmark(Description = "Ordinary JsObject write")]
    public object? Write()
        => JavaScriptRuntime.Object.SetProperty(_writeTarget, "value", 42d);

    [Benchmark(Description = "Hosted dynamic proxy read")]
    public object? HostedRead()
        => _hostedTarget.value;

    [Benchmark(Description = "Hosted dynamic proxy write")]
    public void HostedWrite()
        => _hostedTarget.value = 42d;
}
