using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime;

/// <summary>
/// Realm-owned CLR type entries and weak instance entries.
/// </summary>
internal sealed class RealmObjectTable<TValue> where TValue : class, new()
{
    // Type keys can outlive the realm. A dependent handle keyed by one could root
    // a value-to-function-to-intrinsics cycle even after the table becomes unreachable.
    private readonly ConcurrentDictionary<Type, TValue> _types = new();
    private readonly ConditionalWeakTable<object, TValue> _instances = new();

    internal bool TryGetValue(object key, out TValue value)
        => key is Type type
            ? _types.TryGetValue(type, out value!)
            : _instances.TryGetValue(key, out value!);

    internal TValue GetOrCreateValue(object key)
        => key is Type type
            ? _types.GetOrAdd(type, static _ => new TValue())
            : _instances.GetOrCreateValue(key);

    internal bool Remove(object key)
        => key is Type type
            ? _types.TryRemove(type, out _)
            : _instances.Remove(key);

    internal void Clear()
    {
        _types.Clear();
        _instances.Clear();
    }
}
