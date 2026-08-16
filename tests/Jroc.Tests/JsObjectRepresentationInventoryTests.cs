using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class JsObjectRepresentationInventoryTests
{
    private const string BufferViewReason = "Requires a dedicated buffer/view migration that preserves indexed and backing-store semantics.";
    private const string ErrorReason = "Must remain an Exception for CLR throw/catch mechanics pending the dedicated Error design.";
    private const string HostIteratorReason = "Preserves host iterator adaptation and needs a focused iterator migration.";
    private const string IteratorReason = "Must migrate with its owning iterator family to preserve exhaustion and mutation behavior.";
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
            ["JavaScriptRuntime.Array+ArrayIterator"] = IteratorReason,
            ["JavaScriptRuntime.ArrayBuffer"] = BufferViewReason,
            ["JavaScriptRuntime.DataView"] = BufferViewReason,
            ["JavaScriptRuntime.Error"] = ErrorReason,
            ["JavaScriptRuntime.EvalError"] = ErrorReason,
            ["JavaScriptRuntime.FinalizationRegistry"] = "Requires a dedicated internal-slot wrapper migration.",
            ["JavaScriptRuntime.ForInIterator"] = IteratorReason,
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
            ["JavaScriptRuntime.Proxy"] = "Requires the dedicated Proxy design to preserve traps and invariants.",
            ["JavaScriptRuntime.RangeError"] = ErrorReason,
            ["JavaScriptRuntime.ReferenceError"] = ErrorReason,
            ["JavaScriptRuntime.SharedArrayBuffer"] = BufferViewReason,
            ["JavaScriptRuntime.String+PublicStringIterator"] = IteratorReason,
            ["JavaScriptRuntime.SuppressedError"] = ErrorReason,
            ["JavaScriptRuntime.Symbol"] = "Primitive representation with dedicated identity semantics.",
            ["JavaScriptRuntime.SyntaxError"] = ErrorReason,
            ["JavaScriptRuntime.TypeError"] = ErrorReason,
            ["JavaScriptRuntime.URIError"] = ErrorReason,
            ["JavaScriptRuntime.WeakRef"] = "Requires a dedicated internal-slot wrapper migration.",
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
