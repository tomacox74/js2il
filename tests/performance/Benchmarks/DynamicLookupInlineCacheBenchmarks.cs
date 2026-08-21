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

    private readonly JsObject _receiver = new();
    private readonly JsObject[] _polymorphicReceivers =
        CreateReceivers(
            DynamicLookupInlineCacheSite
                .MaxPolymorphicEntries);
    private readonly JsObject[] _megamorphicReceivers =
        CreateReceivers(
            DynamicLookupInlineCacheSite
                .MaxPolymorphicEntries + 1);
    private readonly JavaScriptRuntime.Array _array =
        new(new object?[] { 1d, 2d });
    private RuntimeAgentCluster? _cluster;
    private IDisposable? _scope;
    private int _polymorphicIndex;
    private int _megamorphicIndex;
    private bool _invalidationToggle;

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
            PropertyHitSite);
        _ = DynamicLookupInlineCache.CallMember0(
            _receiver,
            MethodName,
            CallHitSite);

        foreach (var receiver in _polymorphicReceivers)
        {
            _ = DynamicLookupInlineCache.GetItem(
                receiver,
                PropertyName,
                PolymorphicSite);
        }

        foreach (var receiver in _megamorphicReceivers)
        {
            _ = DynamicLookupInlineCache.GetItem(
                receiver,
                PropertyName,
                MegamorphicSite);
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
            PropertyHitSite);

    [Benchmark]
    public object CachedPropertyMissWithReset()
    {
        _ = DynamicLookupInlineCache
            .RemoveSiteForBenchmarks(PropertyMissSite);
        return DynamicLookupInlineCache.GetItem(
            _receiver,
            PropertyName,
            PropertyMissSite);
    }

    [Benchmark]
    public object CachedPropertyInvalidation()
    {
        _invalidationToggle = !_invalidationToggle;
        _receiver.SetValue(
            PropertyName,
            _invalidationToggle
                ? "value:first"
                : "value:second");
        return DynamicLookupInlineCache.GetItem(
            _receiver,
            PropertyName,
            PropertyInvalidationSite);
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
            CallHitSite);

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
            PolymorphicSite);
    }

    [Benchmark]
    public object MegamorphicFallback()
    {
        var receiver =
            _megamorphicReceivers[
                _megamorphicIndex++
                % _megamorphicReceivers.Length];
        return DynamicLookupInlineCache.GetItem(
            receiver,
            PropertyName,
            MegamorphicSite);
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
            StringSite);

    [Benchmark]
    public object ArrayGenericLength()
        => ObjectRuntime.GetItem(
            _array,
            "length");

    [Benchmark]
    public object ArrayCacheStubFallback()
        => DynamicLookupInlineCache.GetItem(
            _array,
            "length",
            ArraySite);

    private static JsObject[] CreateReceivers(
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
