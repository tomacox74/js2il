using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;

namespace Benchmarks;

[MemoryDiagnoser]
[Config(typeof(FullParamsConfig))]
[ShortRunJob]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "StdDev")]
public class PrototypeStorageBenchmarks
{
    private readonly JavaScriptRuntime.JsObject _prototype = new();
    private static readonly Func<object[], object?[]?, object?> _identityMapper =
        static (_, args) => args![0];

    [Benchmark(Description = "Array with initialized prototype")]
    public JavaScriptRuntime.Array ConstructArray()
        => new();

    [Benchmark(Description = "Boolean wrapper with initialized prototype")]
    public JavaScriptRuntime.Boolean ConstructBooleanWrapper()
        => new(true);

    [Benchmark(Description = "Date wrapper with initialized prototype")]
    public JavaScriptRuntime.Date ConstructDateWrapper()
        => new(0d);

    [Benchmark(Description = "RegExp wrapper with initialized prototype")]
    public JavaScriptRuntime.RegExp ConstructRegExpWrapper()
        => new("a", "g");

    [Benchmark(Description = "Map with initialized prototype")]
    public JavaScriptRuntime.Map ConstructMap()
        => new();

    [Benchmark(Description = "Set with initialized prototype")]
    public JavaScriptRuntime.Set ConstructSet()
        => new();

    [Benchmark(Description = "WeakMap with initialized prototype")]
    public JavaScriptRuntime.WeakMap ConstructWeakMap()
        => new();

    [Benchmark(Description = "WeakSet with initialized prototype")]
    public JavaScriptRuntime.WeakSet ConstructWeakSet()
        => new();

    [Benchmark(Description = "Promise with initialized prototype")]
    public JavaScriptRuntime.Promise ConstructPromise()
        => (JavaScriptRuntime.Promise)JavaScriptRuntime.Promise.resolve(null)!;

    [Benchmark(Description = "Typed array with initialized prototype")]
    public JavaScriptRuntime.Uint8Array ConstructTypedArray()
        => new(0d);

    [Benchmark(Description = "Arguments object with initialized prototype")]
    public JavaScriptRuntime.ArgumentsObject ConstructArgumentsObject()
        => new(null, null, null, null);

    [Benchmark(Description = "Buffer with inline ordinary properties")]
    public JavaScriptRuntime.Node.Buffer ConstructBuffer()
        => new(System.Array.Empty<byte>());

    [Benchmark(Description = "AbortController with initialized prototype")]
    public JavaScriptRuntime.AbortController ConstructAbortController()
        => new();

    [Benchmark(Description = "AbortSignal with initialized prototype")]
    public JavaScriptRuntime.AbortSignal ConstructAbortSignal()
        => new();

    [Benchmark(Description = "URL with initialized prototype")]
    public JavaScriptRuntime.Node.URL ConstructUrl()
        => new("https://example.test/path?key=value");

    [Benchmark(Description = "URLSearchParams with initialized prototype")]
    public JavaScriptRuntime.Node.URLSearchParams ConstructUrlSearchParams()
        => new("key=value");

    [Benchmark(Description = "URLSearchParams iterator with initialized prototype")]
    public JavaScriptRuntime.IJavaScriptIterator ConstructUrlSearchParamsIterator()
        => new JavaScriptRuntime.Node.URLSearchParams("key=value").entries();

    [Benchmark(Description = "Iterator helper with initialized prototype")]
    public JavaScriptRuntime.IJavaScriptIterator ConstructIteratorHelper()
    {
        _ = JavaScriptRuntime.GlobalThis.globalThis;
        var source = JavaScriptRuntime.Iterator.From(
            new JavaScriptRuntime.Array(new object?[] { 1d }));
        var map = JavaScriptRuntime.ObjectRuntime.GetProperty(source, "map");
        return (JavaScriptRuntime.IJavaScriptIterator)JavaScriptRuntime.CallableOperations.Call1(
            map,
            source,
            JavaScriptRuntime.BuiltinDelegateFunctionAdapter.FromDelegate(_identityMapper))!;
    }

    [Benchmark(Description = "Iterator result with initialized prototype and own values")]
    public JavaScriptRuntime.IteratorResultObject ConstructIteratorResult()
        => JavaScriptRuntime.IteratorResult.Create(null, done: false);

    [Benchmark(Description = "Ordinary object with initialized prototype")]
    public JavaScriptRuntime.JsObject ConstructOrdinaryObject()
    {
        var result = new JavaScriptRuntime.JsObject();
        JavaScriptRuntime.PrototypeChain.InitializePrototype(result, _prototype);
        return result;
    }
}
