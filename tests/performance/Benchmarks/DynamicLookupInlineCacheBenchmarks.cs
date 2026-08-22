using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using JavaScriptRuntime;

namespace Benchmarks;

[MemoryDiagnoser]
[JsonExporterAttribute.FullCompressed]
[ShortRunJob]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "StdDev")]
public class DynamicLookupInlineCacheBenchmarks
{
    private const string PropertyName = "value";
    private const string MethodName = "method";
    private const string PropertyHitSite = "benchmark:property-hit";
    private const string PropertyMissSite = "benchmark:property-miss";
    private const string PropertyInvalidationSite =
        "benchmark:property-invalidation";
    private const string CallHitSite = "benchmark:call-hit";
    private const string PolymorphicSite = "benchmark:polymorphic";
    private const string MegamorphicSite = "benchmark:megamorphic";
    private const string StringSite = "benchmark:string";
    private const string ArraySite = "benchmark:array";
    private static int _propertyHitTerminal;
    private static int _propertyMissTerminal;
    private static int _propertyInvalidationTerminal;
    private static int _callHitTerminal;
    private static int _polymorphicTerminal;
    private static int _megamorphicTerminal;
    private static int _stringTerminal;
    private static int _arrayTerminal;

    private readonly JsObject _receiver = new();
    private readonly JsObject[] _polymorphicReceivers =
        CreateReceivers(
            DynamicLookupInlineCacheSite
                .MaxPolymorphicEntries);
    private readonly JsObject[] _megamorphicReceivers =
        CreateReceivers(
            DynamicLookupInlineCacheSite
                .MaxPolymorphicEntries + 1);
    private readonly JsObject[] _sameShapeReceivers =
        CreateSameShapeReceivers(SameShapeReceiverCount);
    private readonly JavaScriptRuntime.Array _array =
        new(new object?[] { 1d, 2d });
    private RuntimeAgentCluster? _cluster;
    private IDisposable? _scope;
    private int _polymorphicIndex;
    private int _megamorphicIndex;
    private int _sameShapeIndex;
    private bool _invalidationToggle;

    private const int SameShapeReceiverCount = 10_000;
    private const string SameShapeSite = "benchmark:same-shape";
    private static int _sameShapeTerminal;

    [GlobalSetup]
    public void Setup()
    {
        var services =
            RuntimeServices.BuildServiceProvider();
        _cluster = services.OwningRealm!.Agent.Cluster;
        var context =
            RuntimeExecutionContext.GetOrCreate(services);
        _scope = context.EnterAsRoot();
        _ = GlobalThis.globalThis;

        _receiver.SetValue(PropertyName, "value:first");
        BuiltinFunction0 function =
            static _ => "called";
        _receiver.SetValue(
            MethodName,
            BuiltinDelegateFunctionAdapter
                .FromDelegate(function));

        _ = DynamicLookupInlineCache.GetItem(
            _receiver,
            PropertyName,
            PropertyHitSite,
            ref _propertyHitTerminal);
        _ = DynamicLookupInlineCache.CallMember0(
            _receiver,
            MethodName,
            CallHitSite,
            ref _callHitTerminal);

        foreach (var receiver in _polymorphicReceivers)
        {
            _ = DynamicLookupInlineCache.GetItem(
                receiver,
                PropertyName,
                PolymorphicSite,
                ref _polymorphicTerminal);
        }

        foreach (var receiver in _megamorphicReceivers)
        {
            _ = DynamicLookupInlineCache.GetItem(
                receiver,
                PropertyName,
                MegamorphicSite,
                ref _megamorphicTerminal);
        }

        foreach (var receiver in _sameShapeReceivers)
        {
            _ = DynamicLookupInlineCache.GetItem(
                receiver,
                PropertyName,
                SameShapeSite,
                ref _sameShapeTerminal);
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _scope?.Dispose();
        _cluster?.Dispose();
    }

    [Benchmark(Baseline = true)]
    public object GenericPropertyRead()
        => ObjectRuntime.GetItem(
            _receiver,
            PropertyName);

    [Benchmark]
    public object CachedPropertyHit()
        => DynamicLookupInlineCache.GetItem(
            _receiver,
            PropertyName,
            PropertyHitSite,
            ref _propertyHitTerminal);

    [Benchmark]
    public object CachedPropertyMissWithReset()
    {
        _ = DynamicLookupInlineCache
            .RemoveSiteForBenchmarks(PropertyMissSite);
        return DynamicLookupInlineCache.GetItem(
            _receiver,
            PropertyName,
            PropertyMissSite,
            ref _propertyMissTerminal);
    }

    [Benchmark]
    public object CachedPropertyInvalidation()
    {
        // Phase 3 (shape-keyed cache): this is a plain value write to an
        // already-cached slot on the same shape, so it is a cache hit on every
        // iteration rather than a rebuild (unlike the identity-keyed cache this
        // benchmark predates).
        _invalidationToggle = !_invalidationToggle;
        _receiver.SetValue(
            PropertyName,
            _invalidationToggle
                ? "value:first"
                : "value:second");
        return DynamicLookupInlineCache.GetItem(
            _receiver,
            PropertyName,
            PropertyInvalidationSite,
            ref _propertyInvalidationTerminal);
    }

    [Benchmark]
    public object? GenericCallMember0()
        => ObjectRuntime.CallMember0(
            _receiver,
            MethodName);

    [Benchmark]
    public object? CachedCallMember0Hit()
        => DynamicLookupInlineCache.CallMember0(
            _receiver,
            MethodName,
            CallHitSite,
            ref _callHitTerminal);

    [Benchmark]
    public object CachedPolymorphicHit()
    {
        var receiver =
            _polymorphicReceivers[
                _polymorphicIndex++
                % _polymorphicReceivers.Length];
        return DynamicLookupInlineCache.GetItem(
            receiver,
            PropertyName,
            PolymorphicSite,
            ref _polymorphicTerminal);
    }

    [Benchmark]
    public object MegamorphicFallback()
    {
        var receiver =
            _megamorphicReceivers[
                _megamorphicIndex++
                % _megamorphicReceivers.Length];
        if (Volatile.Read(ref _megamorphicTerminal) != 0)
        {
            return ObjectRuntime.GetItem(
                receiver,
                PropertyName);
        }

        return DynamicLookupInlineCache.GetItem(
            receiver,
            PropertyName,
            MegamorphicSite,
            ref _megamorphicTerminal);
    }

    /// <summary>
    /// Cycles through thousands of distinct <see cref="JsObject"/> instances that
    /// all share one <see cref="JsShape"/> (the GraphNode.pos scenario from
    /// #1958/#1324): the Phase 3 shape-keyed cache should serve every instance
    /// from a single monomorphic entry instead of collapsing to megamorphic.
    /// </summary>
    [Benchmark]
    public object CachedPropertyHit_SameShapeAcrossInstances()
    {
        var receiver =
            _sameShapeReceivers[
                _sameShapeIndex++
                % _sameShapeReceivers.Length];
        return DynamicLookupInlineCache.GetItem(
            receiver,
            PropertyName,
            SameShapeSite,
            ref _sameShapeTerminal);
    }

    [Benchmark]
    public object StringGenericLength()
        => ObjectRuntime.GetItem(
            "value",
            "length");

    [Benchmark]
    public object StringCacheStubFallback()
        => DynamicLookupInlineCache.GetItem(
            "value",
            "length",
            StringSite,
            ref _stringTerminal);

    [Benchmark]
    public object ArrayGenericLength()
        => ObjectRuntime.GetItem(
            _array,
            "length");

    [Benchmark]
    public double ArrayLengthBoxedThenConsumed()
        => TypeUtilities.ToNumber(
            ObjectRuntime.GetItem(
                _array,
                "length"));

    [Benchmark]
    public double ArrayLengthDirectNumber()
        => _array.length;

    [Benchmark]
    public double ArrayLengthGuardedCandidate()
        => ObjectRuntime.GetArrayLengthWithFallback(
            _array);

    [Benchmark]
    public object ArrayCacheStubFallback()
        => DynamicLookupInlineCache.GetItem(
            _array,
            "length",
            ArraySite,
            ref _arrayTerminal);

    private static JsObject[] CreateReceivers(
        int count)
        => Enumerable
            .Range(0, count)
            .Select(
                index =>
                {
                    var receiver = new JsObject();
                    // A unique marker property per receiver produces a
                    // distinct JsShape, so this exercises genuine
                    // polymorphic/megamorphic transitions under the
                    // Phase 3 shape-keyed cache instead of collapsing to
                    // one shared monomorphic entry.
                    receiver.SetValue($"shape{index}", true);
                    receiver.SetValue(
                        PropertyName,
                        $"value:{index}");
                    return receiver;
                })
            .ToArray();

    private static JsObject[] CreateSameShapeReceivers(
        int count)
        => Enumerable
            .Range(0, count)
            .Select(
                index =>
                {
                    var receiver = new JsObject();
                    receiver.SetValue(
                        PropertyName,
                        $"value:{index}");
                    return receiver;
                })
            .ToArray();
}
