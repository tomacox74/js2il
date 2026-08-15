using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class JsObjectRepresentationInventoryTests
{
    private const string BufferViewReason = "Requires a dedicated buffer/view migration that preserves indexed and backing-store semantics.";
    private const string ErrorReason = "Must remain an Exception for CLR throw/catch mechanics pending the dedicated Error design.";
    private const string HostIteratorReason = "Preserves host iterator adaptation and needs a focused iterator migration.";
    private const string IteratorReason = "Must migrate with its owning iterator family to preserve exhaustion and mutation behavior.";
    private const string TypedArrayReason = "Requires the dedicated TypedArrayBase exotic-object migration.";

    private static readonly string[] ExplicitJavaScriptVisibleTypeNames =
    [
        "JavaScriptRuntime.ArgumentsObject",
        "JavaScriptRuntime.IteratorResultObject`1",
        "JavaScriptRuntime.IteratorResultObject",
        "JavaScriptRuntime.PromiseWithResolvers",
        "JavaScriptRuntime.IntlNumberFormat",
        "JavaScriptRuntime.IntlSegmenter",
        "JavaScriptRuntime.TypedArrayBase",
        "JavaScriptRuntime.Node.Buffer",
        "JavaScriptRuntime.Node.URL",
        "JavaScriptRuntime.Node.URLSearchParams",
        "JavaScriptRuntime.AbortController",
        "JavaScriptRuntime.AbortSignal"
    ];

    private static readonly IReadOnlyDictionary<string, string> DocumentedNonJsObjectRepresentations =
        new Dictionary<string, string>
        {
            ["JavaScriptRuntime.AbortController"] = "Requires the AbortController and AbortSignal migration.",
            ["JavaScriptRuntime.AbortSignal"] = "Requires the AbortController and AbortSignal migration.",
            ["JavaScriptRuntime.AggregateError"] = ErrorReason,
            ["JavaScriptRuntime.ArgumentsObject"] = "Requires the dedicated ArgumentsObject exotic-object migration.",
            ["JavaScriptRuntime.ArgumentsObject+ValueIterator"] = "Must migrate with ArgumentsObject to preserve mapped-parameter behavior.",
            ["JavaScriptRuntime.Array+ArrayIterator"] = IteratorReason,
            ["JavaScriptRuntime.ArrayBuffer"] = BufferViewReason,
            ["JavaScriptRuntime.AsyncGeneratorObject"] = "Requires the dedicated AsyncGeneratorObject migration.",
            ["JavaScriptRuntime.DataView"] = BufferViewReason,
            ["JavaScriptRuntime.Error"] = ErrorReason,
            ["JavaScriptRuntime.EvalError"] = ErrorReason,
            ["JavaScriptRuntime.FinalizationRegistry"] = "Requires a dedicated internal-slot wrapper migration.",
            ["JavaScriptRuntime.Float32Array"] = TypedArrayReason,
            ["JavaScriptRuntime.Float64Array"] = TypedArrayReason,
            ["JavaScriptRuntime.ForInIterator"] = IteratorReason,
            ["JavaScriptRuntime.GeneratorObject"] = "Requires the dedicated GeneratorObject migration.",
            ["JavaScriptRuntime.Int16Array"] = TypedArrayReason,
            ["JavaScriptRuntime.Int32Array"] = TypedArrayReason,
            ["JavaScriptRuntime.Int8Array"] = TypedArrayReason,
            ["JavaScriptRuntime.IntlNumberFormat"] = "Needs a dedicated Intl wrapper migration once its constructor surface expands.",
            ["JavaScriptRuntime.IntlSegmenter"] = "Needs a dedicated Intl wrapper migration once its constructor surface expands.",
            ["JavaScriptRuntime.Iterator+DropIteratorHelper"] = IteratorReason,
            ["JavaScriptRuntime.Iterator+FilterIteratorHelper"] = IteratorReason,
            ["JavaScriptRuntime.Iterator+FlatMapIteratorHelper"] = IteratorReason,
            ["JavaScriptRuntime.Iterator+GeneratorIteratorAdapter"] = HostIteratorReason,
            ["JavaScriptRuntime.Iterator+IteratorHelperBase"] = IteratorReason,
            ["JavaScriptRuntime.Iterator+IteratorLikeWrapper"] = HostIteratorReason,
            ["JavaScriptRuntime.Iterator+MapIteratorHelper"] = IteratorReason,
            ["JavaScriptRuntime.Iterator+TakeIteratorHelper"] = IteratorReason,
            ["JavaScriptRuntime.IteratorResultObject"] = "Requires the dedicated iterator result and helper migration.",
            ["JavaScriptRuntime.IteratorResultObject`1"] = "Requires the dedicated iterator result and helper migration.",
            ["JavaScriptRuntime.Map"] = "Requires the Map and MapIterator migration.",
            ["JavaScriptRuntime.Map+MapIterator"] = "Must migrate with Map to preserve collection mutation behavior.",
            ["JavaScriptRuntime.Node.Buffer"] = BufferViewReason,
            ["JavaScriptRuntime.Node.Events+EventEmitterAsyncOnIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.Node.TimersPromises+TimersPromisesIntervalIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.Node.URL"] = "Requires the URL and URLSearchParams migration.",
            ["JavaScriptRuntime.Node.URLSearchParams"] = "Requires the URL and URLSearchParams migration.",
            ["JavaScriptRuntime.Node.URLSearchParams+SearchParamsIterator"] = "Must migrate with URLSearchParams.",
            ["JavaScriptRuntime.Object"] = "Static intrinsic holder; constructed ordinary objects use JsObject.",
            ["JavaScriptRuntime.ObjectRuntime+ArrayIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.ObjectRuntime+AsyncDynamicIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.ObjectRuntime+AsyncFromSyncIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.ObjectRuntime+DynamicIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.ObjectRuntime+EnumerableIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.ObjectRuntime+StringIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.Promise"] = "Requires the Promise migration.",
            ["JavaScriptRuntime.PromiseWithResolvers"] = "Must migrate with Promise to preserve resolver identity.",
            ["JavaScriptRuntime.Proxy"] = "Requires the dedicated Proxy design to preserve traps and invariants.",
            ["JavaScriptRuntime.RangeError"] = ErrorReason,
            ["JavaScriptRuntime.ReferenceError"] = ErrorReason,
            ["JavaScriptRuntime.RegExp"] = "Requires the RegExp wrapper migration.",
            ["JavaScriptRuntime.Set"] = "Requires the Set and SetIterator migration.",
            ["JavaScriptRuntime.Set+SetIterator"] = "Must migrate with Set to preserve collection mutation behavior.",
            ["JavaScriptRuntime.SharedArrayBuffer"] = BufferViewReason,
            ["JavaScriptRuntime.String+PublicStringIterator"] = IteratorReason,
            ["JavaScriptRuntime.SuppressedError"] = ErrorReason,
            ["JavaScriptRuntime.Symbol"] = "Primitive representation with dedicated identity semantics.",
            ["JavaScriptRuntime.SyntaxError"] = ErrorReason,
            ["JavaScriptRuntime.TypedArrayBase"] = TypedArrayReason,
            ["JavaScriptRuntime.TypedArrayIterator"] = TypedArrayReason,
            ["JavaScriptRuntime.TypeError"] = ErrorReason,
            ["JavaScriptRuntime.Uint16Array"] = TypedArrayReason,
            ["JavaScriptRuntime.Uint32Array"] = TypedArrayReason,
            ["JavaScriptRuntime.Uint8Array"] = TypedArrayReason,
            ["JavaScriptRuntime.Uint8ClampedArray"] = TypedArrayReason,
            ["JavaScriptRuntime.URIError"] = ErrorReason,
            ["JavaScriptRuntime.WeakMap"] = "Requires the WeakMap and WeakSet migration.",
            ["JavaScriptRuntime.WeakRef"] = "Requires a dedicated internal-slot wrapper migration.",
            ["JavaScriptRuntime.WeakSet"] = "Requires the WeakMap and WeakSet migration."
        };

    [Fact]
    public void JavaScriptVisibleRepresentations_AreMigratedOrDocumented()
    {
        var runtimeAssembly = typeof(JsObject).Assembly;
        var explicitTypes = ExplicitJavaScriptVisibleTypeNames
            .Select(typeName => runtimeAssembly.GetType(typeName)
                ?? throw new InvalidOperationException($"Missing JavaScript-visible type: {typeName}"))
            .ToArray();

        var candidates = runtimeAssembly
            .GetTypes()
            .Where(IsIntrinsicInstanceType)
            .Concat(explicitTypes)
            .Concat(runtimeAssembly.GetTypes().Where(type => type.IsClass && IsIteratorRepresentation(type)))
            .Distinct()
            .Where(type => !typeof(JsObject).IsAssignableFrom(type))
            .Select(type => type.FullName!)
            .OrderBy(name => name)
            .ToArray();

        Assert.Equal(DocumentedNonJsObjectRepresentations.Keys.OrderBy(name => name), candidates);
        Assert.All(
            DocumentedNonJsObjectRepresentations.Values,
            reason => Assert.False(string.IsNullOrWhiteSpace(reason)));
    }

    private static bool IsIntrinsicInstanceType(Type type)
        => type.GetCustomAttributes(typeof(IntrinsicObjectAttribute), inherit: false).Length > 0
            && (!type.IsAbstract || !type.IsSealed);

    private static bool IsIteratorRepresentation(Type type)
        => typeof(IJavaScriptIterator).IsAssignableFrom(type)
            || typeof(IJavaScriptAsyncIterator).IsAssignableFrom(type)
            || typeof(IIteratorResult).IsAssignableFrom(type);
}
