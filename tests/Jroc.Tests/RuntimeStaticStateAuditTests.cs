using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using JavaScriptRuntime;
using JavaScriptRuntime.EngineCore;
using JavaScriptRuntime.Modules.CommonJS;
using JavaScriptRuntime.Node;
using Jroc.Runtime;
using AssemblyName = System.Reflection.AssemblyName;

namespace Jroc.Tests;

public sealed class RuntimeStaticStateAuditTests
{
    private const int StressRuntimeCount = 4;
    private const int StressRoundCount = 4;

    private static readonly ApprovedStatic[] ApprovedWritableStatics =
    [
        new("JavaScriptRuntime.Array._defaultPrototypeChainHasBlockingIndexedProperties", "thread-local deoptimization state"),
        new("JavaScriptRuntime.Array._observedPrototypeIntrinsicsId", "thread-local deoptimization state"),
        new("JavaScriptRuntime.Array._observedPrototypeMutationVersion", "thread-local deoptimization state"),
        new("JavaScriptRuntime.Array._prototypeMutationVersion", "process-wide monotonic deoptimization version"),
        new("JavaScriptRuntime.DynamicLookupInlineCache._currentCaches", "thread-local realm cache pointer"),
        new("JavaScriptRuntime.DynamicLookupInlineCache._lastSite", "thread-local cache-site lookup state"),
        new("JavaScriptRuntime.DynamicLookupInlineCache._nextRecentSite", "thread-local cache-site lookup state"),
        new("JavaScriptRuntime.DynamicLookupInlineCache._previousSite", "thread-local cache-site lookup state"),
        new("JavaScriptRuntime.DynamicLookupInlineCache._recentSites", "thread-local cache-site lookup state"),
        new("JavaScriptRuntime.Function._generatedConstructionReceivers", "thread-local invocation state"),
        new("JavaScriptRuntime.Node.AsyncContextRuntime._activeContextRuntimeCount", "process-wide fast-path activity count"),
        new("JavaScriptRuntime.Node.AsyncContextRuntime._enabledHookCount", "process-wide fast-path activity count"),
        new("JavaScriptRuntime.Node.FsCommon._nextFileDescriptor", "process-wide resource identity allocator"),
        new("JavaScriptRuntime.RegExp._prototypeWellKnownSymbolFastPathFlags", "process-wide monotonic deoptimization flags"),
        new("JavaScriptRuntime.RuntimeExecutionContext._threadCurrent", "thread-local execution-context mirror"),
        new("JavaScriptRuntime.RuntimeIntrinsics._initializationDepth", "thread-local bootstrap state"),
        new("JavaScriptRuntime.RuntimeIntrinsics._nextId", "process-wide metadata identity allocator"),
        new("JavaScriptRuntime.RuntimeIntrinsics._processDefault", "context-less process fallback"),
        new("JavaScriptRuntime.RuntimeServices._constructorArgStack", "thread-local invocation state"),
        new("JavaScriptRuntime.RuntimeServices._constructorNewTargetStack", "thread-local invocation state"),
        new("JavaScriptRuntime.RuntimeServices._derivedConstructorThisStack", "thread-local invocation state"),
        new("JavaScriptRuntime.RuntimeServices._generatedFunctionDirectCallStack", "thread-local invocation state"),
        new("JavaScriptRuntime.String.substringCache", "thread-local primitive value cache"),
        new("JavaScriptRuntime.String.substringCacheNextIndex", "thread-local primitive value cache"),
        new("JavaScriptRuntime.Symbol._nextId", "process-wide primitive identity allocator"),
    ];

    private static readonly ApprovedStatic[] ApprovedReadonlyReferenceStatics =
    [
        new("JavaScriptRuntime.ArgumentsObject.FieldCaches", "weak-keyed CLR metadata"),
        new("JavaScriptRuntime.Array.Hole", "immutable ABI sentinel"),
        new("JavaScriptRuntime.BoxedNumber.Cache", "immutable primitive lookup"),
        new("JavaScriptRuntime.Closure._delegateInvokeMetadata", "weak-keyed CLR metadata"),
        new("JavaScriptRuntime.Date.DateOnlyRegex", "immutable regex metadata"),
        new("JavaScriptRuntime.Date.DateStringRegex", "immutable regex metadata"),
        new("JavaScriptRuntime.Date.DayNames", "immutable primitive lookup"),
        new("JavaScriptRuntime.Date.IsoDateTimeRegex", "immutable regex metadata"),
        new("JavaScriptRuntime.Date.LocalDateTimeRegex", "immutable regex metadata"),
        new("JavaScriptRuntime.Date.MonthNames", "immutable primitive lookup"),
        new("JavaScriptRuntime.DotNet2JSConversions.SmallIntStrings", "immutable primitive lookup"),
        new("JavaScriptRuntime.DynamicLookupInlineCacheSite+Snapshot.Empty", "immutable empty cache snapshot"),
        new("JavaScriptRuntime.DynamicLookupInlineCacheSite+Snapshot.Megamorphic", "immutable megamorphic cache snapshot"),
        new("JavaScriptRuntime.Engine._serviceProviderOverride", "ambient override handle"),
        new("JavaScriptRuntime.EngineCore.NodeEventLoopPump+NoOpFinalizationRegistryHost.Instance", "stateless host singleton"),
        new("JavaScriptRuntime.GeneratorObject+DynamicGeneratorIterator.NoYield", "immutable ABI sentinel"),
        new("JavaScriptRuntime.GlobalThis._defaultConsole", "context-less process fallback"),
        new("JavaScriptRuntime.GlobalThis._defaultProcess", "context-less process fallback"),
        new("JavaScriptRuntime.GlobalThis._fallbackGlobalObject", "thread-local context-less fallback"),
        new("JavaScriptRuntime.GlobalThis._strictUtf8", "immutable encoding metadata"),
        new("JavaScriptRuntime.JSON.RawJsonObjects", "weak-keyed value metadata"),
        new("JavaScriptRuntime.JsShape._empty", "thread-local empty shape"),
        new("JavaScriptRuntime.Map.NullKeySentinel", "immutable ABI sentinel"),
        new("JavaScriptRuntime.Node.AsyncResourceObject.States", "weak-keyed async resource metadata"),
        new("JavaScriptRuntime.Node.Buffer+Base64PassthroughEncoding.Instance", "stateless encoding singleton"),
        new("JavaScriptRuntime.Node.Buffer+HexPassthroughEncoding.Instance", "stateless encoding singleton"),
        new("JavaScriptRuntime.Node.BufferModule.StrictUtf8", "immutable encoding metadata"),
        new("JavaScriptRuntime.Node.ChildProcess+StdioConfiguration.AsyncDefault", "immutable configuration"),
        new("JavaScriptRuntime.Node.ChildProcess+StdioConfiguration.ForkDefault", "immutable configuration"),
        new("JavaScriptRuntime.Node.ChildProcess+StdioConfiguration.IgnoreAll", "immutable configuration"),
        new("JavaScriptRuntime.Node.ChildProcess+StdioConfiguration.InheritAll", "immutable configuration"),
        new("JavaScriptRuntime.Node.ChildProcess+StdioConfiguration.SyncDefault", "immutable configuration"),
        new("JavaScriptRuntime.Node.Events.ErrorMonitorSymbol", "immutable Node module symbol"),
        new("JavaScriptRuntime.Node.FsEncodingOptions.Utf8NoBom", "immutable encoding metadata"),
        new("JavaScriptRuntime.Node.NodeModuleRegistry.ContractsByName", "lazy frozen type metadata"),
        new("JavaScriptRuntime.Node.NodeModuleRegistry.ModulesByName", "lazy frozen type metadata"),
        new("JavaScriptRuntime.Node.Process._platform", "lazy immutable primitive metadata"),
        new("JavaScriptRuntime.ObjectRuntime._encodedSymbolKeys", "weak-keyed symbol metadata"),
        new("JavaScriptRuntime.ObjectRuntime._integrityStates", "weak-keyed object metadata"),
        new("JavaScriptRuntime.PropertyDescriptorStore._defaultRuntimeStore", "thread-local context-less fallback"),
        new("JavaScriptRuntime.PropertyDescriptorStore._intrinsicInitializationDepth", "thread-local bootstrap state"),
        new("JavaScriptRuntime.PropertyDescriptorStore+DescriptorSnapshot.Empty", "immutable snapshot"),
        new("JavaScriptRuntime.PropertyDescriptorStore+OverrideSnapshot.Empty", "immutable snapshot"),
        new("JavaScriptRuntime.RuntimeExecutionContext.Ambient", "async-flow execution pointer"),
        new("JavaScriptRuntime.RuntimeIntrinsics._blockedThreads", "transient bootstrap wait graph"),
        new("JavaScriptRuntime.RuntimeIntrinsics._processDefaultGate", "process fallback synchronization"),
        new("JavaScriptRuntime.RuntimeServices.EmptyScopes", "immutable ABI array"),
        new("JavaScriptRuntime.RuntimeServices.TemporalDeadZoneSentinel", "immutable ABI sentinel"),
        new("JavaScriptRuntime.RuntimeServices._currentInvocation", "async-flow invocation frame"),
        new("JavaScriptRuntime.RuntimeServices._generatedClassMethodReceivers", "weak-keyed generated method receiver metadata"),
        new("JavaScriptRuntime.ScriptProcessExitControl.PendingExit", "async-flow process-exit signal"),
        new("JavaScriptRuntime.String.Latin1CharStrings", "immutable primitive lookup"),
        new("JavaScriptRuntime.Symbol._asyncDispose", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.Symbol._asyncIterator", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.Symbol._dispose", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.Symbol._hasInstance", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.Symbol._isConcatSpreadable", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.Symbol._iterator", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.Symbol._match", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.Symbol._matchAll", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.Symbol._replace", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.Symbol._search", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.Symbol._species", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.Symbol._split", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.Symbol._toPrimitive", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.Symbol._toStringTag", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.Symbol._unscopables", "ECMA-262 well-known symbol"),
        new("JavaScriptRuntime.WeakSet._dummyValue", "immutable ABI sentinel"),
        new("Jroc.Runtime.JsReturnConverter.ResultConversions", "weak-keyed CLR metadata"),
    ];

    private static readonly HashSet<Type> MutableReferenceContainerDefinitions =
    [
        typeof(Lazy<>),
        typeof(AsyncLocal<>),
        typeof(ThreadLocal<>),
        typeof(ConditionalWeakTable<,>),
        typeof(ConcurrentDictionary<,>),
        typeof(Dictionary<,>),
        typeof(HashSet<>),
        typeof(List<>),
        typeof(Queue<>),
        typeof(Stack<>),
    ];

    [Fact]
    public void WritableStaticsMatchTheApprovedOwnershipMatrix()
    {
        var actual = GetRuntimeStaticFields()
            .Where(field => !field.IsLiteral && !field.IsInitOnly)
            .Select(GetFieldName)
            .Order(StringComparer.Ordinal);
        var expected = ApprovedWritableStatics
            .Select(item => item.Name)
            .Order(StringComparer.Ordinal);

        Assert.Equal(expected, actual);
        Assert.All(
            ApprovedWritableStatics,
            item => Assert.False(string.IsNullOrWhiteSpace(item.Owner)));
    }

    [Fact]
    public void ReadonlyReferenceStaticsMatchTheApprovedOwnershipMatrix()
    {
        var actual = GetRuntimeStaticFields()
            .Where(field => field.IsInitOnly)
            .Where(field => IsMutableReferenceCandidate(field.FieldType))
            .Select(GetFieldName)
            .Order(StringComparer.Ordinal);
        var expected = ApprovedReadonlyReferenceStatics
            .Select(item => item.Name)
            .Order(StringComparer.Ordinal);

        Assert.Equal(expected, actual);
        Assert.All(
            ApprovedReadonlyReferenceStatics,
            item => Assert.False(string.IsNullOrWhiteSpace(item.Owner)));
    }

    [Fact]
    public async Task ParallelRuntimeStressKeepsObservableStateIsolated()
    {
        for (var round = 0; round < StressRoundCount; round++)
        {
            using var ready = new Barrier(StressRuntimeCount);
            var tasks = Enumerable.Range(0, StressRuntimeCount)
                .Select(index => Task.Run(
                    () => RunStressRuntime($"{round}:{index}", index, ready)))
                .ToArray();

            var results = await Task.WhenAll(tasks);

            Assert.All(results, result =>
            {
                Assert.Equal(result.Id, result.GlobalValue);
                Assert.Equal(result.Id, result.PrototypeValue);
                Assert.Equal(result.Id, result.ImportMetaValue);
                Assert.Equal(result.Id, result.RequireValue);
                Assert.Equal(result.Id, result.MicrotaskThis);
                Assert.Equal(result.Id, result.FsConstantsValue);
                Assert.Equal(result.ExpectedDnsOrder, result.DnsOrder);
                Assert.True(result.TimerRan);
                Assert.True(result.ClusterDisposed);
            });

            for (var first = 0; first < results.Length; first++)
            {
                for (var second = first + 1; second < results.Length; second++)
                {
                    Assert.NotSame(results[first].Global, results[second].Global);
                    Assert.NotSame(results[first].ObjectPrototype, results[second].ObjectPrototype);
                    Assert.NotSame(results[first].ImportMeta, results[second].ImportMeta);
                    Assert.NotSame(results[first].Template, results[second].Template);
                    Assert.NotSame(results[first].RegisteredSymbol, results[second].RegisteredSymbol);
                    Assert.NotSame(results[first].Scheduler, results[second].Scheduler);
                    Assert.NotSame(results[first].AsyncContext, results[second].AsyncContext);
                    Assert.NotSame(results[first].Dns, results[second].Dns);
                    Assert.NotSame(results[first].FsConstants, results[second].FsConstants);
                    Assert.NotSame(results[first].PathPosix, results[second].PathPosix);
                    Assert.NotSame(results[first].PathWin32, results[second].PathWin32);
                }
            }
        }

        Assert.Null(RuntimeExecutionContext.Current);
    }

    [Fact]
    public void DisposedRuntimeGraphsBecomeCollectible()
    {
        var references = CreateDisposedRuntimeGraphReferences();

        CollectUntilDead(references);

        Assert.All(references, reference => Assert.False(reference.IsAlive));
    }

    [Fact]
    public void StaticMetadataCachesDoNotRetainCollectibleTypes()
    {
        var references = PopulateStaticMetadataCachesWithCollectibleType();

        CollectUntilDead(references);

        Assert.All(references, reference => Assert.False(reference.IsAlive));
    }

    [Fact]
    public void EncodedSymbolMetadataDoesNotRetainUnusedSymbols()
    {
        var references = CreateEncodedSymbolReferences();

        CollectUntilDead(references);

        Assert.All(references, reference => Assert.False(reference.IsAlive));
    }

    private static StressResult RunStressRuntime(
        string id,
        int index,
        Barrier ready)
    {
        StressResult result;
        RuntimeAgentCluster cluster;

        using (var lifecycle = RuntimeLifecycle.Create(
            typeof(RuntimeStaticStateAuditTests).Assembly,
            isHostedExecution: true,
            suppressInheritedExecutionContext: true))
        {
            cluster = lifecycle.Cluster;
            object? global = null;
            object? objectPrototype = null;
            object? importMeta = null;
            object? template = null;
            Symbol? registeredSymbol = null;
            NodeSchedulerState? scheduler = null;
            AsyncContextRuntime? asyncContext = null;
            Dns? dns = null;
            object? fsConstants = null;
            object? pathPosix = null;
            object? pathWin32 = null;
            object? globalValue = null;
            object? prototypeValue = null;
            object? importMetaValue = null;
            object? requireValue = null;
            object? fsConstantsValue = null;
            object? microtaskThis = null;
            var timerRan = false;
            var expectedDnsOrder = index % 2 == 0
                ? "ipv4first"
                : "ipv6first";
            string? dnsOrder = null;

            lifecycle.Execute(
                services =>
                {
                    global = GlobalThis.globalThis;
                    objectPrototype = GlobalThis.ObjectPrototypeValue;
                    ObjectRuntime.SetProperty(global, "realmStress", id);
                    ObjectRuntime.SetProperty(objectPrototype, "realmStress", id);

                    importMeta = RuntimeServices.GetImportMeta("shared.js");
                    ObjectRuntime.SetProperty(importMeta, "realmStress", id);
                    RequireDelegate require = _ => id;
                    RuntimeServices.RegisterModuleRequire("shared.js", require);

                    template = RuntimeServices.CreateTemplateObject(
                        "shared.js:1:1",
                        [id],
                        [id]);
                    registeredSymbol = Assert.IsType<Symbol>(
                        Symbol.@for("shared-runtime-symbol"));

                    var requireService = services.Resolve<Require>();
                    dns = Assert.IsType<Dns>(
                        requireService.RequireModule("node:dns"));
                    Assert.Same(
                        dns,
                        requireService.RequireModule("dns"));
                    dns.setDefaultResultOrder(expectedDnsOrder);
                    fsConstants = Assert.IsType<FS>(
                        requireService.RequireModule("node:fs")).constants;
                    ObjectRuntime.SetProperty(
                        fsConstants,
                        "realmStress",
                        id);
                    var path = Assert.IsType<JavaScriptRuntime.Node.Path>(
                        requireService.RequireModule("node:path"));
                    pathPosix = path.posix;
                    pathWin32 = path.win32;

                    scheduler = services.Resolve<NodeSchedulerState>();
                    asyncContext = services.Resolve<AsyncContextRuntime>();
                    RuntimeServices.SetCurrentThis(id);
                    services.Resolve<IMicrotaskScheduler>()
                        .QueueMicrotask(
                            () => microtaskThis = RuntimeServices.GetCurrentThis());
                    _ = services.Resolve<IScheduler>().Schedule(
                        () => timerRan = true,
                        TimeSpan.Zero);

                    Assert.True(
                        ready.SignalAndWait(TimeSpan.FromSeconds(10)));

                    globalValue = ObjectRuntime.GetProperty(
                        global,
                        "realmStress");
                    prototypeValue = ObjectRuntime.GetProperty(
                        objectPrototype,
                        "realmStress");
                    importMetaValue = ObjectRuntime.GetProperty(
                        importMeta,
                        "realmStress");
                    requireValue = RuntimeServices
                        .GetRequireForModule("shared.js")!("ignored");
                    fsConstantsValue = ObjectRuntime.GetProperty(
                        fsConstants,
                        "realmStress");
                    dnsOrder = dns.getDefaultResultOrder();
                },
                waitForTimers: true);

            result = new StressResult(
                id,
                global!,
                objectPrototype!,
                importMeta!,
                template!,
                registeredSymbol!,
                scheduler!,
                asyncContext!,
                dns!,
                fsConstants!,
                pathPosix!,
                pathWin32!,
                globalValue,
                prototypeValue,
                importMetaValue,
                requireValue,
                fsConstantsValue,
                microtaskThis,
                timerRan,
                expectedDnsOrder,
                dnsOrder,
                ClusterDisposed: false);
        }

        return result with { ClusterDisposed = cluster.IsDisposed };
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] CreateDisposedRuntimeGraphReferences()
    {
        WeakReference[] references;

        using (var lifecycle = RuntimeLifecycle.Create(
            typeof(RuntimeStaticStateAuditTests).Assembly,
            isHostedExecution: true,
            suppressInheritedExecutionContext: true))
        {
            object? global = null;
            object? prototype = null;
            object? template = null;
            object? nodeModule = null;

            lifecycle.Execute(
                services =>
                {
                    global = GlobalThis.globalThis;
                    prototype = GlobalThis.ObjectPrototypeValue;
                    template = RuntimeServices.CreateTemplateObject(
                        "collectible",
                        ["value"],
                        ["value"]);
                    nodeModule = services.Resolve<Require>()
                        .RequireModule("node:dns");
                },
                waitForTimers: false);

            references =
            [
                new WeakReference(lifecycle.Cluster),
                new WeakReference(lifecycle.Agent),
                new WeakReference(lifecycle.Realm),
                new WeakReference(global),
                new WeakReference(prototype),
                new WeakReference(template),
                new WeakReference(nodeModule),
            ];
        }

        return references;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] PopulateStaticMetadataCachesWithCollectibleType()
    {
        var assembly = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName($"CollectibleStaticMetadata_{Guid.NewGuid():N}"),
            AssemblyBuilderAccess.RunAndCollect);
        var module = assembly.DefineDynamicModule("main");
        var typeBuilder = module.DefineType("Generated.Scope");
        _ = typeBuilder.DefineField(
            "value",
            typeof(object),
            FieldAttributes.Public);
        var collectibleType = typeBuilder.CreateType()!;
        var scope = Activator.CreateInstance(collectibleType)!;
        collectibleType.GetField("value")!.SetValue(scope, 1d);
        var arguments = new ArgumentsObject(
            [1d],
            scope,
            ["value"],
            calleeValue: null);

        Assert.Equal(1d, arguments["0"]);
        arguments["0"] = 2d;
        Assert.Equal(2d, collectibleType.GetField("value")!.GetValue(scope));
        Assert.False(JsFuncDelegates.IsJsFuncDelegateType(collectibleType));

        var runtime = (JsRuntimeInstance)RuntimeHelpers.GetUninitializedObject(
            typeof(JsRuntimeInstance));
        var taskType = typeof(Task<>).MakeGenericType(collectibleType);
        _ = JsReturnConverter.ConvertReturn(runtime, null, taskType);

        return
        [
            new WeakReference(scope),
            new WeakReference(collectibleType),
            new WeakReference(assembly),
        ];
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] CreateEncodedSymbolReferences()
    {
        var symbol = new Symbol("collectible");
        var encodedKey = ObjectRuntime.ToPropertyKeyString(symbol);

        return
        [
            new WeakReference(symbol),
            new WeakReference(encodedKey),
        ];
    }

    private static void CollectUntilDead(IEnumerable<WeakReference> references)
    {
        var materialized = references.ToArray();
        for (var attempt = 0;
            attempt < 12 && materialized.Any(reference => reference.IsAlive);
            attempt++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }

    private static IEnumerable<FieldInfo> GetRuntimeStaticFields()
        => typeof(RuntimeServices).Assembly
            .GetTypes()
            .Where(type => type.FullName?.Contains(
                '<',
                StringComparison.Ordinal) != true)
            .Where(type => !type.IsDefined(
                typeof(CompilerGeneratedAttribute),
                inherit: false))
            .SelectMany(type => type.GetFields(
                BindingFlags.Static
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.DeclaredOnly))
            .Where(field => !field.IsDefined(
                typeof(CompilerGeneratedAttribute),
                inherit: false));

    private static string GetFieldName(FieldInfo field)
        => $"{field.DeclaringType!.FullName}.{field.Name}";

    private static bool IsMutableReferenceCandidate(Type fieldType)
    {
        var genericDefinition = fieldType.IsGenericType
            ? fieldType.GetGenericTypeDefinition()
            : null;

        return fieldType.IsArray
            || fieldType == typeof(object)
            || fieldType == typeof(Random)
            || fieldType == typeof(Regex)
            || typeof(Encoding).IsAssignableFrom(fieldType)
            || (genericDefinition != null
                && MutableReferenceContainerDefinitions.Contains(
                    genericDefinition))
            || (fieldType.Assembly == typeof(RuntimeServices).Assembly
                && fieldType.IsClass
                && !typeof(Delegate).IsAssignableFrom(fieldType));
    }

    private sealed record ApprovedStatic(string Name, string Owner);

    private sealed record StressResult(
        string Id,
        object Global,
        object ObjectPrototype,
        object ImportMeta,
        object Template,
        Symbol RegisteredSymbol,
        NodeSchedulerState Scheduler,
        AsyncContextRuntime AsyncContext,
        Dns Dns,
        object FsConstants,
        object PathPosix,
        object PathWin32,
        object? GlobalValue,
        object? PrototypeValue,
        object? ImportMetaValue,
        object? RequireValue,
        object? FsConstantsValue,
        object? MicrotaskThis,
        bool TimerRan,
        string ExpectedDnsOrder,
        string? DnsOrder,
        bool ClusterDisposed);
}
