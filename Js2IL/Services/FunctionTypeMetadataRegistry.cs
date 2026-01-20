using System.Reflection.Metadata;

namespace Js2IL.Services;

/// <summary>
/// Tracks generated TypeDef handles for function-declaration owner types.
/// These types are emitted before the enclosing module TypeDef exists, so the
/// nesting relationship (NestedClass table rows) is established later.
/// </summary>
public sealed class FunctionTypeMetadataRegistry
{
    private readonly Dictionary<string, Dictionary<string, TypeDefinitionHandle>> _byModule = new(StringComparer.Ordinal);

    public void Add(string moduleName, string functionName, TypeDefinitionHandle typeHandle)
    {
        if (string.IsNullOrWhiteSpace(moduleName))
        {
            throw new ArgumentException("Module name is required.", nameof(moduleName));
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
            map = new Dictionary<string, TypeDefinitionHandle>(StringComparer.Ordinal);
            _byModule[moduleName] = map;
        }

        map[functionName] = typeHandle;
    }

    public bool TryGet(string moduleName, string functionName, out TypeDefinitionHandle typeHandle)
    {
        typeHandle = default;
        return _byModule.TryGetValue(moduleName, out var map) && map.TryGetValue(functionName, out typeHandle);
    }

    public IReadOnlyDictionary<string, TypeDefinitionHandle> GetAllForModule(string moduleName)
    {
        if (_byModule.TryGetValue(moduleName, out var map))
        {
            return map;
        }

        return new Dictionary<string, TypeDefinitionHandle>();
    }
}
