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
    private readonly JavaScriptRuntime.JsObject _deleteTarget = new();
    private readonly JavaScriptRuntime.JsObject _descriptorTarget = new();
    private readonly JavaScriptRuntime.JsObject _frozenTarget = new();
    private readonly JavaScriptRuntime.Array _indexedTarget = new(new object?[] { 42d });
    private readonly JavaScriptRuntime.JsObject _iterableTarget = new();
    private JrocInMemoryModule? _hostedModule;
    private dynamic _hostedTarget = null!;

    [GlobalSetup]
    public void Setup()
    {
        JavaScriptRuntime.ObjectRuntime.SetProperty(_readTarget, "value", 42d);
        JavaScriptRuntime.ObjectRuntime.SetProperty(_writeTarget, "value", 0d);
        for (var i = 0; i < 16; i++)
        {
            JavaScriptRuntime.ObjectRuntime.SetProperty(_descriptorTarget, $"value{i}", (double)i);
            JavaScriptRuntime.ObjectRuntime.SetProperty(_frozenTarget, $"value{i}", (double)i);
        }
        JavaScriptRuntime.ObjectRuntime.freeze(_frozenTarget);

        var iterator = new JavaScriptRuntime.JsObject();
        JavaScriptRuntime.ObjectRuntime.SetProperty(
            iterator,
            "next",
            (Func<object[], object?[]?, object?>)((_, _) => JavaScriptRuntime.IteratorResult.Create(null, done: true)));
        JavaScriptRuntime.ObjectRuntime.DefineObjectLiteralDataProperty(
            _iterableTarget,
            JavaScriptRuntime.Symbol.iterator,
            (Func<object[], object?[]?, object?>)((_, _) => iterator));

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
        => JavaScriptRuntime.ObjectRuntime.GetProperty(_readTarget, "value");

    [Benchmark(Description = "Ordinary JsObject write")]
    public object? Write()
        => JavaScriptRuntime.ObjectRuntime.SetProperty(_writeTarget, "value", 42d);

    [Benchmark(Description = "Hosted dynamic proxy read")]
    public object? HostedRead()
        => _hostedTarget.value;

    [Benchmark(Description = "Hosted dynamic proxy write")]
    public void HostedWrite()
        => _hostedTarget.value = 42d;

    [Benchmark(Description = "Delete missing property (non-strict)")]
    public bool DeleteMissingNonStrict()
        => JavaScriptRuntime.ObjectRuntime.DeletePropertyNonStrict(_deleteTarget, "missing");

    [Benchmark(Description = "Get own property descriptors")]
    public object GetOwnPropertyDescriptors()
        => JavaScriptRuntime.ObjectRuntime.getOwnPropertyDescriptors(_descriptorTarget);

    [Benchmark(Description = "Test frozen object")]
    public bool IsFrozen()
        => JavaScriptRuntime.ObjectRuntime.isFrozen(_frozenTarget);

    [Benchmark(Description = "Numeric indexed read")]
    public object GetNumericIndex()
        => JavaScriptRuntime.ObjectRuntime.GetItem(_indexedTarget, 0d);

    [Benchmark(Description = "Resolve custom iterator")]
    public object GetIterator()
        => JavaScriptRuntime.ObjectRuntime.GetIterator(_iterableTarget);
}
