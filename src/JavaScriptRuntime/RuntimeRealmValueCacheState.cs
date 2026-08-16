using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime;

/// <summary>
/// Realm-owned caches whose entries contain JavaScript values or captured scopes.
/// </summary>
internal sealed class RuntimeRealmValueCacheState : IDisposable
{
    internal ConcurrentDictionary<string, Array> TemplateObjects { get; } =
        new(StringComparer.Ordinal);

    internal ConcurrentDictionary<
        RuntimeServices.ClassConstructorCacheKey,
        JsClassConstructorObject> MaterializedClassConstructors { get; } = new();

    internal ConditionalWeakTable<Type, RuntimeServices.LazyClassMetadataSlot>
        LazyClassMetadata { get; } = new();

    public void Dispose()
    {
        TemplateObjects.Clear();
        MaterializedClassConstructors.Clear();
        LazyClassMetadata.Clear();
    }
}
