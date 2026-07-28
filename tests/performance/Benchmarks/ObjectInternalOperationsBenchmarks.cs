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

/// <summary>
/// Measures the allocation and throughput effects of shape-indexed descriptor state.
/// MemoryDiagnoser reports the empty-object field cost and lazy metadata allocations.
/// </summary>
[MemoryDiagnoser]
[Config(typeof(FullParamsConfig))]
[ShortRunJob]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
[HideColumns("Error", "StdDev")]
public class JsObjectDescriptorStorageBenchmarks
{
    private JavaScriptRuntime.JsObject _queryTarget = null!;
    private JavaScriptRuntime.JsObject _deleteTarget = null!;
    private string _queryKey = "";
    private string _deleteKey = "";

    [Params(0, 1, 2, 4, 8, 16)]
    public int PropertyCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _queryTarget = CreateDefaultObject(PropertyCount);
        _queryKey = PropertyCount == 0 ? "missing" : $"value{PropertyCount - 1}";

        _deleteTarget = CreateDefaultObject(global::System.Math.Max(1, PropertyCount));
        _deleteKey = PropertyCount == 0 ? "value0" : $"value{PropertyCount / 2}";
    }

    [Benchmark(Description = "Allocate empty JsObject")]
    public JavaScriptRuntime.JsObject AllocateEmpty()
        => new();

    [Benchmark(Description = "Populate default properties")]
    public JavaScriptRuntime.JsObject PopulateDefaultProperties()
        => CreateDefaultObject(PropertyCount);

    [Benchmark(Description = "Populate 10,000 default objects")]
    public int PopulateDefaultPropertiesAggregate()
    {
        var total = 0;
        for (var objectIndex = 0; objectIndex < 10_000; objectIndex++)
        {
            total += CreateDefaultObject(PropertyCount).Count;
        }

        return total;
    }

    [Benchmark(Description = "Add first custom descriptor")]
    public JavaScriptRuntime.JsObject AddFirstCustomDescriptor()
    {
        var target = CreateDefaultObject(PropertyCount);
        target.DefineOwnProperty("custom", new JavaScriptRuntime.JsPropertyDescriptor
        {
            Kind = JavaScriptRuntime.JsPropertyDescriptorKind.Data,
            Value = 42d,
            Writable = false,
            Enumerable = false,
            Configurable = true
        });
        return target;
    }

    [Benchmark(Description = "Add accessor descriptor")]
    public JavaScriptRuntime.JsObject AddAccessorDescriptor()
    {
        var target = CreateDefaultObject(PropertyCount);
        target.DefineOwnProperty("accessor", new JavaScriptRuntime.JsPropertyDescriptor
        {
            Kind = JavaScriptRuntime.JsPropertyDescriptorKind.Accessor,
            Get = (Func<object[], object?[]?, object?>)(static (_, _) => 42d),
            Enumerable = true,
            Configurable = true
        });
        return target;
    }

    [Benchmark(Description = "Query own descriptor")]
    public bool QueryOwnDescriptor()
        => JavaScriptRuntime.PropertyDescriptorStore.TryGetOwn(
            _queryTarget,
            _queryKey,
            out _);

    [Benchmark(Description = "Delete and re-add property")]
    public void DeleteAndReAdd()
    {
        _deleteTarget.DeleteOwnProperty(_deleteKey);
        _deleteTarget.SetNumber(_deleteKey, 42d);
    }

    private static JavaScriptRuntime.JsObject CreateDefaultObject(int propertyCount)
    {
        var target = new JavaScriptRuntime.JsObject();
        for (var propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
        {
            target.SetNumber($"value{propertyIndex}", propertyIndex);
        }

        return target;
    }
}
