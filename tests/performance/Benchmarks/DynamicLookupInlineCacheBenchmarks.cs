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
    private const string Call1HitSite = "benchmark:call1-hit";
    private const string Call1SharedPrototypeHitSite =
        "benchmark:call1-shared-prototype-hit";
    private const string Call1ReassignmentSite =
        "benchmark:call1-reassignment";
    private const string Call1PolymorphicSite =
        "benchmark:call1-polymorphic";
    private const string Call1MegamorphicSite =
        "benchmark:call1-megamorphic";
    private const string Call1AccessorSite =
        "benchmark:call1-accessor";
    private const string Call1ProxySite =
        "benchmark:call1-proxy";
    private const string PolymorphicSite = "benchmark:polymorphic";
    private const string MegamorphicSite = "benchmark:megamorphic";
    private const string StringSite = "benchmark:string";
    private const string ArraySite = "benchmark:array";
    private static int _propertyHitTerminal;
    private static int _propertyMissTerminal;
    private static int _propertyInvalidationTerminal;
    private static int _callHitTerminal;
    private static int _call1HitTerminal;
    private static int _call1SharedPrototypeHitTerminal;
    private static int _call1ReassignmentTerminal;
    private static int _call1PolymorphicTerminal;
    private static int _call1MegamorphicTerminal;
    private static int _call1AccessorTerminal;
    private static int _call1ProxyTerminal;
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
    private readonly JavaScriptRuntime.Array _call1Receiver = new();
    private readonly JavaScriptRuntime.Array
        _call1SharedPrototypeReceiver = new();
    private readonly JavaScriptRuntime.Array _call1ReassignmentReceiver = new();
    private readonly JavaScriptRuntime.Array _call1AccessorReceiver = new();
    private JsObject[] _call1PolymorphicReceivers = null!;
    private JsObject[] _call1MegamorphicReceivers = null!;
    private JavaScriptRuntime.Proxy _call1Proxy = null!;
    private JsObject _call1ReassignmentPrototype = null!;
    private object _call1FirstFunction = null!;
    private object _call1SecondFunction = null!;
    private RuntimeAgentCluster? _cluster;
    private IDisposable? _scope;
    private int _polymorphicIndex;
    private int _megamorphicIndex;
    private int _sameShapeIndex;
    private int _call1PolymorphicIndex;
    private int _call1MegamorphicIndex;
    private bool _invalidationToggle;
    private bool _call1ReassignmentToggle;

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

        BuiltinFunction1 call1Function =
            static (_, argument0) => argument0;
        _call1FirstFunction =
            BuiltinDelegateFunctionAdapter
                .FromDelegate(call1Function);
        BuiltinFunction1 secondCall1Function =
            static (_, argument0) => argument0;
        _call1SecondFunction =
            BuiltinDelegateFunctionAdapter
                .FromDelegate(secondCall1Function);
        var call1Prototype = new JsObject();
        call1Prototype.SetValue(MethodName, _call1FirstFunction);
        PrototypeChain.SetPrototype(_call1Receiver, call1Prototype);
        JavaScriptRuntime.Array.Prototype.SetValue(
            "benchmarkPhase4Method",
            _call1FirstFunction);
        PrototypeChain.SetPrototype(
            _call1SharedPrototypeReceiver,
            JavaScriptRuntime.Array.Prototype);

        _call1ReassignmentPrototype = new JsObject();
        _call1ReassignmentPrototype.SetValue(
            MethodName,
            _call1FirstFunction);
        PrototypeChain.SetPrototype(
            _call1ReassignmentReceiver,
            _call1ReassignmentPrototype);

        var call1AccessorPrototype = new JsObject();
        BuiltinFunction0 call1Getter = _ => _call1FirstFunction;
        PropertyDescriptorStore.DefineOrUpdate(
            call1AccessorPrototype,
            MethodName,
            new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Get = BuiltinDelegateFunctionAdapter
                    .FromDelegate(call1Getter),
                Enumerable = true,
                Configurable = true
            });
        PrototypeChain.SetPrototype(
            _call1AccessorReceiver,
            call1AccessorPrototype);

        var proxyTarget = new JsObject();
        proxyTarget.SetValue(MethodName, _call1FirstFunction);
        _call1Proxy = new JavaScriptRuntime.Proxy(
            proxyTarget,
            new JsObject());

        _call1PolymorphicReceivers =
            CreateCall1Receivers(
                DynamicLookupInlineCacheSite
                    .MaxPolymorphicEntries,
                call1Prototype);
        _call1MegamorphicReceivers =
            CreateCall1Receivers(
                DynamicLookupInlineCacheSite
                    .MaxPolymorphicEntries + 1,
                call1Prototype);

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
        _ = DynamicLookupInlineCache.CallMember1(
            _call1Receiver,
            MethodName,
            "argument",
            Call1HitSite,
            ref _call1HitTerminal);
        _ = DynamicLookupInlineCache.CallMember1(
            _call1SharedPrototypeReceiver,
            "benchmarkPhase4Method",
            "argument",
            Call1SharedPrototypeHitSite,
            ref _call1SharedPrototypeHitTerminal);
        _ = DynamicLookupInlineCache.CallMember1(
            _call1ReassignmentReceiver,
            MethodName,
            "argument",
            Call1ReassignmentSite,
            ref _call1ReassignmentTerminal);

        foreach (var receiver in _call1PolymorphicReceivers)
        {
            _ = DynamicLookupInlineCache.CallMember1(
                receiver,
                MethodName,
                "argument",
                Call1PolymorphicSite,
                ref _call1PolymorphicTerminal);
        }

        foreach (var receiver in _call1MegamorphicReceivers)
        {
            _ = DynamicLookupInlineCache.CallMember1(
                receiver,
                MethodName,
                "argument",
                Call1MegamorphicSite,
                ref _call1MegamorphicTerminal);
        }

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
    public object? GenericCallMember1()
        => ObjectRuntime.CallMember1(
            _call1Receiver,
            MethodName,
            "argument");

    [Benchmark]
    public object? CachedCallMember1PrototypeHit()
        => DynamicLookupInlineCache.CallMember1(
            _call1Receiver,
            MethodName,
            "argument",
            Call1HitSite,
            ref _call1HitTerminal);

    [Benchmark]
    public object? CachedCallMember1SharedArrayPrototypeHit()
        => DynamicLookupInlineCache.CallMember1(
            _call1SharedPrototypeReceiver,
            "benchmarkPhase4Method",
            "argument",
            Call1SharedPrototypeHitSite,
            ref _call1SharedPrototypeHitTerminal);

    [Benchmark]
    public object? CachedCallMember1PrototypeReassignment()
    {
        _call1ReassignmentToggle =
            !_call1ReassignmentToggle;
        _call1ReassignmentPrototype.SetValue(
            MethodName,
            _call1ReassignmentToggle
                ? _call1FirstFunction
                : _call1SecondFunction);
        return DynamicLookupInlineCache.CallMember1(
            _call1ReassignmentReceiver,
            MethodName,
            "argument",
            Call1ReassignmentSite,
            ref _call1ReassignmentTerminal);
    }

    [Benchmark]
    public object? CachedCallMember1PolymorphicHit()
    {
        var receiver =
            _call1PolymorphicReceivers[
                _call1PolymorphicIndex++
                % _call1PolymorphicReceivers.Length];
        return DynamicLookupInlineCache.CallMember1(
            receiver,
            MethodName,
            "argument",
            Call1PolymorphicSite,
            ref _call1PolymorphicTerminal);
    }

    [Benchmark]
    public object? CallMember1MegamorphicFallback()
    {
        var receiver =
            _call1MegamorphicReceivers[
                _call1MegamorphicIndex++
                % _call1MegamorphicReceivers.Length];
        if (Volatile.Read(ref _call1MegamorphicTerminal) != 0)
        {
            return ObjectRuntime.CallMember1(
                receiver,
                MethodName,
                "argument");
        }

        return DynamicLookupInlineCache.CallMember1(
            receiver,
            MethodName,
            "argument",
            Call1MegamorphicSite,
            ref _call1MegamorphicTerminal);
    }

    [Benchmark]
    public object? CallMember1AccessorFallback()
        => DynamicLookupInlineCache.CallMember1(
            _call1AccessorReceiver,
            MethodName,
            "argument",
            Call1AccessorSite,
            ref _call1AccessorTerminal);

    [Benchmark]
    public object? CallMember1ProxyFallback()
        => DynamicLookupInlineCache.CallMember1(
            _call1Proxy,
            MethodName,
            "argument",
            Call1ProxySite,
            ref _call1ProxyTerminal);

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

    private static JsObject[] CreateCall1Receivers(
        int count,
        JsObject prototype)
        => Enumerable
            .Range(0, count)
            .Select(
                index =>
                {
                    var receiver =
                        new JavaScriptRuntime.Array();
                    receiver.SetValue(
                        $"shape{index}",
                        true);
                    PrototypeChain.SetPrototype(
                        receiver,
                        prototype);
                    return (JsObject)receiver;
                })
            .ToArray();
}
