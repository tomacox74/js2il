using System.Reflection.Metadata;

namespace Js2IL.Services;

/// <summary>
/// Tracks generated TypeDef handles for anonymous callable owner types (arrow functions and function expressions).
/// These types are emitted before all nesting relationships are established, so the enclosing relationship
/// (NestedClass table rows) is recorded later.
/// </summary>
public sealed class AnonymousCallableTypeMetadataRegistry
{
    public sealed record Entry(
        string DeclaringScopeName,
        string OwnerTypeName,
        TypeDefinitionHandle OwnerTypeHandle);

    private readonly Dictionary<string, List<Entry>> _byModule = new(StringComparer.Ordinal);
    private readonly Dictionary<(string Module, string DeclaringScopeName, string OwnerTypeName), TypeDefinitionHandle> _byKey = new();

    public void Add(string moduleName, string declaringScopeName, string ownerTypeName, TypeDefinitionHandle ownerTypeHandle)
    {
        if (string.IsNullOrWhiteSpace(moduleName)) throw new ArgumentException("Module name is required.", nameof(moduleName));
        if (string.IsNullOrWhiteSpace(declaringScopeName)) throw new ArgumentException("Declaring scope name is required.", nameof(declaringScopeName));
        if (string.IsNullOrWhiteSpace(ownerTypeName)) throw new ArgumentException("Owner type name is required.", nameof(ownerTypeName));
        if (ownerTypeHandle.IsNil) throw new ArgumentException("Type handle must not be nil.", nameof(ownerTypeHandle));

        if (!_byModule.TryGetValue(moduleName, out var list))
        {
            list = new List<Entry>();
            _byModule[moduleName] = list;
        }

        var entry = new Entry(declaringScopeName, ownerTypeName, ownerTypeHandle);
        list.Add(entry);

        _byKey[(moduleName, declaringScopeName, ownerTypeName)] = ownerTypeHandle;
    }

    public IReadOnlyList<Entry> GetAllForModule(string moduleName)
    {
        if (_byModule.TryGetValue(moduleName, out var list))
        {
            return list;
        }

        return Array.Empty<Entry>();
    }

    public bool TryGetOwnerTypeHandle(string moduleName, string declaringScopeName, string ownerTypeName, out TypeDefinitionHandle ownerTypeHandle)
    {
        return _byKey.TryGetValue((moduleName, declaringScopeName, ownerTypeName), out ownerTypeHandle);
    }
}
