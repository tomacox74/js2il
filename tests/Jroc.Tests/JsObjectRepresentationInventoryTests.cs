using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class JsObjectRepresentationInventoryTests
{
    private const string BufferViewReason = "Requires a dedicated buffer/view migration that preserves indexed and backing-store semantics.";
    private const string ErrorReason = "Intentionally remains an Exception to preserve CLR throw/catch and host exception translation; its realm-owned prototype uses fallback storage.";
    private const string HostIteratorReason = "Preserves host iterator adaptation and needs a focused iterator migration.";
    private const string IteratorReason = "Must migrate with its owning iterator family to preserve exhaustion and mutation behavior.";
    private const string ProxyReason = "Intentionally remains non-JsObject so own-property operations dispatch through Proxy traps or its target instead of shape/slot fast paths.";
    private static readonly string[] ExplicitJavaScriptVisibleTypeNames =
    [
        "JavaScriptRuntime.ArgumentsObject",
        "JavaScriptRuntime.PromiseWithResolvers",
        "JavaScriptRuntime.IntlNumberFormat",
        "JavaScriptRuntime.IntlSegmenter",
        "JavaScriptRuntime.TypedArrayBase",
        "JavaScriptRuntime.Node.Buffer"
    ];

    private static readonly IReadOnlyDictionary<string, string> DocumentedNonJsObjectRepresentations =
        new Dictionary<string, string>
        {
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
            ["JavaScriptRuntime.Iterator+GeneratorIteratorAdapter"] = HostIteratorReason,
            ["JavaScriptRuntime.Iterator+IteratorLikeWrapper"] = HostIteratorReason,
            ["JavaScriptRuntime.Node.Events+EventEmitterAsyncOnIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.Node.TimersPromises+TimersPromisesIntervalIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.Object"] = "Static intrinsic holder; constructed ordinary objects use JsObject.",
            ["JavaScriptRuntime.ObjectRuntime+ArrayIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.ObjectRuntime+AsyncDynamicIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.ObjectRuntime+AsyncFromSyncIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.ObjectRuntime+DynamicIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.ObjectRuntime+EnumerableIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.ObjectRuntime+StringIterator"] = HostIteratorReason,
            ["JavaScriptRuntime.Proxy"] = ProxyReason,
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
