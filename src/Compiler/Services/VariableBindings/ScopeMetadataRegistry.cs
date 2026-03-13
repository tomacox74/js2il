using System.Reflection.Metadata;

namespace Js2IL.Services.VariableBindings;

/// <summary>
/// Registry for scope type handles and field handles.
/// This is the minimal interface needed for IL emission to look up metadata handles.
/// Consumed by EnvironmentLayoutBuilder and other new code; VariableRegistry delegates to this.
/// </summary>
public class ScopeMetadataRegistry
{
    private readonly Dictionary<string, TypeDefinitionHandle> _scopeTypes = new();
    private readonly Dictionary<string, Dictionary<string, FieldDefinitionHandle>> _scopeFields = new();
    private readonly Dictionary<string, Dictionary<string, Type>> _scopeFieldClrTypes = new();
    // Track scope type handles even when no variables (so empty method scopes can still be instantiated)
    private readonly Dictionary<string, TypeDefinitionHandle> _allScopeTypes = new();

    /// <summary>
    /// Registers a scope type handle.
    /// </summary>
    public void RegisterScopeType(string scopeName, TypeDefinitionHandle typeHandle)
    {
        if (scopeName == null || typeHandle.IsNil) return;
        _scopeTypes[scopeName] = typeHandle;
    }

    /// <summary>
    /// Registers a field handle for a variable in a scope.
    /// </summary>
    public void RegisterField(string scopeName, string variableName, FieldDefinitionHandle fieldHandle)
    {
        if (!_scopeFields.ContainsKey(scopeName))
            _scopeFields[scopeName] = new Dictionary<string, FieldDefinitionHandle>();
        
        _scopeFields[scopeName][variableName] = fieldHandle;
    }

    /// <summary>
    /// Registers the declared CLR type of a scope field.
    /// This is used by IL emission to box/unbox correctly when fields are not System.Object.
    /// </summary>
    public void RegisterFieldClrType(string scopeName, string variableName, Type fieldClrType)
    {
        if (!_scopeFieldClrTypes.ContainsKey(scopeName))
            _scopeFieldClrTypes[scopeName] = new Dictionary<string, Type>();

        _scopeFieldClrTypes[scopeName][variableName] = fieldClrType;
    }

    /// <summary>
    /// Ensures a scope type handle is registered even if there are no variables/fields.
    /// (Used for empty class methods so a scope instance can still be created when needed.)
    /// </summary>
    public void EnsureScopeType(string scopeName, TypeDefinitionHandle typeHandle)
    {
        if (scopeName == null) return;
        if (typeHandle.IsNil) return;
        if (!_allScopeTypes.ContainsKey(scopeName))
            _allScopeTypes[scopeName] = typeHandle;
    }

    /// <summary>
    /// Gets the type handle for a specific scope.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown if scope type handle not found.</exception>
    public TypeDefinitionHandle GetScopeTypeHandle(string scopeName)
    {
        if (_scopeTypes.TryGetValue(scopeName, out var h))
            return h;
        if (_allScopeTypes.TryGetValue(scopeName, out var any))
            return any;
        throw new KeyNotFoundException($"Scope type handle not found for scope '{scopeName}'");
    }

    /// <summary>
    /// Tries to get the type handle for a specific scope.
    /// </summary>
    /// <returns>True if found, false otherwise.</returns>
    public bool TryGetScopeTypeHandle(string scopeName, out TypeDefinitionHandle typeHandle)
    {
        if (_scopeTypes.TryGetValue(scopeName, out typeHandle))
            return true;
        if (_allScopeTypes.TryGetValue(scopeName, out typeHandle))
            return true;
        typeHandle = default;
        return false;
    }

    /// <summary>
    /// Gets the field handle for a specific variable in a scope.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown if field handle not found.</exception>
    public FieldDefinitionHandle GetFieldHandle(string scopeName, string variableName)
    {
        return _scopeFields[scopeName][variableName];
    }

    /// <summary>
    /// Tries to get the field handle for a specific variable in a scope.
    /// </summary>
    /// <returns>True if found, false otherwise.</returns>
    public bool TryGetFieldHandle(string scopeName, string variableName, out FieldDefinitionHandle fieldHandle)
    {
        if (_scopeFields.TryGetValue(scopeName, out var fields) && 
            fields.TryGetValue(variableName, out fieldHandle))
        {
            return true;
        }
        fieldHandle = default;
        return false;
    }

    public bool TryGetFieldClrType(string scopeName, string variableName, out Type fieldClrType)
    {
        if (_scopeFieldClrTypes.TryGetValue(scopeName, out var fields) &&
            fields.TryGetValue(variableName, out var t) &&
            t != null)
        {
            fieldClrType = t;
            return true;
        }

        fieldClrType = typeof(object);
        return false;
    }

    /// <summary>
    /// Gets all registered scope names.
    /// </summary>
    public IEnumerable<string> GetAllScopeNames()
    {
        return _scopeTypes.Keys.Union(_allScopeTypes.Keys);
    }

    /// <summary>
    /// Checks if a scope has any registered fields.
    /// </summary>
    public bool ScopeHasFields(string scopeName)
    {
        return _scopeFields.TryGetValue(scopeName, out var fields) && fields.Count > 0;
    }
}
