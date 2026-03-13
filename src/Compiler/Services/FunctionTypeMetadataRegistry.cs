using System.Reflection.Metadata;

namespace Js2IL.Services;

/// <summary>
/// Tracks generated TypeDef handles for function-declaration owner types.
/// These types are emitted before the enclosing module TypeDef exists, so the
/// nesting relationship (NestedClass table rows) is established later.
/// </summary>
public sealed class FunctionTypeMetadataRegistry
{
    // Keying by function name alone is not sufficient for real-world code:
    // nested function declarations can reuse the same name in different scopes.
    // We key by module -> declaringScopeName -> functionName.
    private readonly Dictionary<string, Dictionary<string, Dictionary<string, TypeDefinitionHandle>>> _byModule = new(StringComparer.Ordinal);

    public void Add(string moduleName, string declaringScopeName, string functionName, TypeDefinitionHandle typeHandle)
    {
        if (string.IsNullOrWhiteSpace(moduleName))
        {
            throw new ArgumentException("Module name is required.", nameof(moduleName));
        }

        if (string.IsNullOrWhiteSpace(declaringScopeName))
        {
            throw new ArgumentException("Declaring scope name is required.", nameof(declaringScopeName));
        }

        if (string.IsNullOrWhiteSpace(functionName))
        {
            throw new ArgumentException("Function name is required.", nameof(functionName));
        }

        if (typeHandle.IsNil)
        {
            throw new ArgumentException("Type handle must not be nil.", nameof(typeHandle));
        }

        if (!_byModule.TryGetValue(moduleName, out var map))
        {
            map = new Dictionary<string, Dictionary<string, TypeDefinitionHandle>>(StringComparer.Ordinal);
            _byModule[moduleName] = map;
        }

        if (!map.TryGetValue(declaringScopeName, out var byName))
        {
            byName = new Dictionary<string, TypeDefinitionHandle>(StringComparer.Ordinal);
            map[declaringScopeName] = byName;
        }

        byName[functionName] = typeHandle;
    }

    public bool TryGet(string moduleName, string declaringScopeName, string functionName, out TypeDefinitionHandle typeHandle)
    {
        typeHandle = default;
        return _byModule.TryGetValue(moduleName, out var map)
            && map.TryGetValue(declaringScopeName, out var byName)
            && byName.TryGetValue(functionName, out typeHandle);
    }

    // Backward-compatible overloads for legacy callsites.
    // For top-level function declarations, declaringScopeName == moduleName.
    public void Add(string moduleName, string functionName, TypeDefinitionHandle typeHandle) =>
        Add(moduleName, declaringScopeName: moduleName, functionName, typeHandle);

    public bool TryGet(string moduleName, string functionName, out TypeDefinitionHandle typeHandle) =>
        TryGet(moduleName, declaringScopeName: moduleName, functionName, out typeHandle);

    public IReadOnlyDictionary<string, Dictionary<string, TypeDefinitionHandle>> GetAllForModule(string moduleName)
    {
        if (_byModule.TryGetValue(moduleName, out var map))
        {
            return map;
        }

        return new Dictionary<string, Dictionary<string, TypeDefinitionHandle>>();
    }
}
