#if SOURCE_JROC_PROJECTS
using System.Collections.Frozen;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using JavaScriptRuntime;

namespace Benchmarks;

[MemoryDiagnoser]
[Config(typeof(FullParamsConfig))]
[ShortRunJob]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
[HideColumns("Error", "StdDev")]
public class JsShapeStorageBenchmarks
{
    private string[] _propertyNames = [];
    private JsShape _shape = new();
    private Dictionary<string, int> _dictionaryLookup =
        new(StringComparer.Ordinal);
    private FrozenDictionary<string, int> _frozenLookup =
        new Dictionary<string, int>().ToFrozenDictionary(StringComparer.Ordinal);
    private string _lastPropertyName = "";
    private const string MissingPropertyName = "missing";
    private const string AppendedPropertyName = "appended";

    [Params(1, 2, 4, 8, 16)]
    public int PropertyCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _propertyNames = Enumerable.Range(0, PropertyCount)
            .Select(index => $"p{index}")
            .ToArray();
        _lastPropertyName = _propertyNames[^1];
        _shape = CreateShape(_propertyNames);
        _dictionaryLookup = CreateDictionaryLookup(_propertyNames);
        _frozenLookup = CreateFrozenLookup(_propertyNames);
    }

    [Benchmark(Description = "JsShape lookup: last")]
    public int ShapeLookupLast()
        => _shape.GetSlot(_lastPropertyName);

    [Benchmark(Description = "Frozen lookup: last")]
    public int FrozenLookupLast()
        => _frozenLookup.TryGetValue(_lastPropertyName, out var slot) ? slot : -1;

    [Benchmark(Description = "Dictionary lookup: last")]
    public int DictionaryLookupLast()
        => _dictionaryLookup.TryGetValue(_lastPropertyName, out var slot) ? slot : -1;

    [Benchmark(Description = "JsShape lookup: missing")]
    public int ShapeLookupMissing()
        => _shape.GetSlot(MissingPropertyName);

    [Benchmark(Description = "Frozen lookup: missing")]
    public int FrozenLookupMissing()
        => _frozenLookup.TryGetValue(MissingPropertyName, out var slot) ? slot : -1;

    [Benchmark(Description = "Dictionary lookup: missing")]
    public int DictionaryLookupMissing()
        => _dictionaryLookup.TryGetValue(MissingPropertyName, out var slot) ? slot : -1;

    [Benchmark(Description = "JsShape create")]
    public object ShapeCreate()
        => CreateShape(_propertyNames);

    [Benchmark(Description = "Frozen shape create")]
    public FrozenDictionary<string, int> FrozenShapeCreate()
    {
        var slots = new Dictionary<string, int>(StringComparer.Ordinal);
        FrozenDictionary<string, int> frozen =
            slots.ToFrozenDictionary(StringComparer.Ordinal);
        foreach (var propertyName in _propertyNames)
        {
            slots = frozen.ToDictionary(StringComparer.Ordinal);
            slots[propertyName] = slots.Count;
            frozen = slots.ToFrozenDictionary(StringComparer.Ordinal);
        }
        return frozen;
    }

    [Benchmark(Description = "JsShape append")]
    public object ShapeAppend()
        => _shape.TransitionToUncached(AppendedPropertyName);

    [Benchmark(Description = "Frozen shape append")]
    public FrozenDictionary<string, int> FrozenShapeAppend()
    {
        var slots = _frozenLookup.ToDictionary(StringComparer.Ordinal);
        slots[AppendedPropertyName] = slots.Count;
        return slots.ToFrozenDictionary(StringComparer.Ordinal);
    }

    [Benchmark(Description = "JsShape enumerate")]
    public int ShapeEnumerate()
    {
        var totalLength = 0;
        foreach (var propertyName in _shape.PropertyNamesInSlotOrder)
        {
            totalLength += propertyName.Length;
        }
        return totalLength;
    }

    [Benchmark(Description = "Frozen shape enumerate")]
    public int FrozenShapeEnumerate()
    {
        var orderedNames = new string[_frozenLookup.Count];
        foreach (var property in _frozenLookup)
        {
            orderedNames[property.Value] = property.Key;
        }

        var totalLength = 0;
        foreach (var propertyName in orderedNames)
        {
            totalLength += propertyName.Length;
        }
        return totalLength;
    }

    private static JsShape CreateShape(IEnumerable<string> propertyNames)
    {
        var shape = new JsShape();
        foreach (var propertyName in propertyNames)
        {
            shape = shape.TransitionToUncached(propertyName);
        }
        return shape;
    }

    private static FrozenDictionary<string, int> CreateFrozenLookup(
        IReadOnlyList<string> propertyNames)
    {
        var slots = CreateDictionaryLookup(propertyNames);
        return slots.ToFrozenDictionary(StringComparer.Ordinal);
    }

    private static Dictionary<string, int> CreateDictionaryLookup(
        IReadOnlyList<string> propertyNames)
    {
        var slots = new Dictionary<string, int>(propertyNames.Count, StringComparer.Ordinal);
        for (var slot = 0; slot < propertyNames.Count; slot++)
        {
            slots[propertyNames[slot]] = slot;
        }
        return slots;
    }
}
#endif
