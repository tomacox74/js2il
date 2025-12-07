using System.Reflection.Metadata;
using Js2IL.SymbolTables;

namespace Js2IL.Services.VariableBindings
{
    /// <summary>
    /// Type of variable based on JavaScript declaration.
    /// </summary>
    public enum VariableType
    {
        Variable,
        Function,
        Parameter
    }

    /// <summary>
    /// Registry that contains all variables discovered through static analysis of the JavaScript AST.
    /// Populated by TypeGenerator and consumed by Variables class.
    /// </summary>
    public class VariableRegistry
    {
        private readonly Dictionary<string, List<VariableInfo>> _scopeVariables = new();
        private readonly Dictionary<string, TypeDefinitionHandle> _scopeTypes = new();
        private readonly Dictionary<string, Dictionary<string, FieldDefinitionHandle>> _scopeFields = new();
        // Track scope type handles even when no variables (so empty method scopes can still be instantiated)
        private readonly Dictionary<string, TypeDefinitionHandle> _allScopeTypes = new();
        // Track uncaptured variables (no scope backing field, will use local variables)
        private readonly Dictionary<string, HashSet<string>> _uncapturedVariables = new();

        /// <summary>
        /// Adds a variable to the registry with its scope and field information (legacy overload; assumes Var binding).
        /// </summary>
        public void AddVariable(string scopeName, string variableName, VariableType type,
                               FieldDefinitionHandle fieldHandle, TypeDefinitionHandle scopeTypeHandle)
        {
            AddVariable(scopeName, variableName, type, fieldHandle, scopeTypeHandle, BindingKind.Var);
        }

        /// <summary>
        /// Adds a variable to the registry with its scope and field information.
        /// </summary>
        public void AddVariable(string scopeName, string variableName, VariableType type,
                               FieldDefinitionHandle fieldHandle, TypeDefinitionHandle scopeTypeHandle,
                               BindingKind bindingKind)
        {
            if (!_scopeVariables.ContainsKey(scopeName))
                _scopeVariables[scopeName] = new List<VariableInfo>();

            _scopeVariables[scopeName].Add(new VariableInfo
            {
                Name = variableName,
                ScopeName = scopeName,
                VariableType = type,
                FieldHandle = fieldHandle,
                ScopeTypeHandle = scopeTypeHandle,
                BindingKind = bindingKind
            });

            if (!_scopeFields.ContainsKey(scopeName))
                _scopeFields[scopeName] = new Dictionary<string, FieldDefinitionHandle>();
            
            _scopeFields[scopeName][variableName] = fieldHandle;
            // Only set scope type if it's not NIL (avoid overwriting valid handle with default)
            if (!scopeTypeHandle.IsNil)
            {
                _scopeTypes[scopeName] = scopeTypeHandle;
            }
        }

        /// <summary>
        /// Gets all variables for a specific scope.
        /// </summary>
        public IEnumerable<VariableInfo> GetVariablesForScope(string scopeName)
        {
            return _scopeVariables.GetValueOrDefault(scopeName, new List<VariableInfo>());
        }

        /// <summary>
        /// Gets the field handle for a specific variable in a scope.
        /// </summary>
        public FieldDefinitionHandle GetFieldHandle(string scopeName, string variableName)
        {
            return _scopeFields[scopeName][variableName];
        }

        /// <summary>
        /// Gets the type handle for a specific scope.
        /// </summary>
        public TypeDefinitionHandle GetScopeTypeHandle(string scopeName)
        {
            if (_scopeTypes.TryGetValue(scopeName, out var h))
                return h;
            if (_allScopeTypes.TryGetValue(scopeName, out var any))
                return any;
            throw new KeyNotFoundException($"Scope type handle not found for scope '{scopeName}'");
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
        /// Gets all scope names in the registry.
        /// </summary>
        public IEnumerable<string> GetAllScopeNames()
        {
            // Return all scopes that have either variables or just type definitions
            return _scopeVariables.Keys.Union(_allScopeTypes.Keys);
        }

        /// <summary>
        /// Finds a variable by name across all scopes.
        /// </summary>
        public VariableInfo? FindVariable(string variableName)
        {
            foreach (var scopeName in _scopeVariables.Keys)
            {
                var variable = _scopeVariables[scopeName].FirstOrDefault(v => v.Name == variableName);
                if (variable != null)
                    return variable;
            }
            return null;
        }

        /// <summary>
        /// Gets variable info for a specific variable name within a specific scope, or null if not found.
        /// </summary>
        public VariableInfo? GetVariableInfo(string scopeName, string variableName)
        {
            if (_scopeVariables.TryGetValue(scopeName, out var list))
            {
                return list.FirstOrDefault(v => v.Name == variableName);
            }
            return null;
        }

        /// <summary>
        /// Records the runtime intrinsic CLR type for a variable (e.g., result of require('fs')).
        /// This enables other generator contexts (nested functions) to recognize intrinsic instances.
        /// </summary>
        public void SetRuntimeIntrinsicType(string scopeName, string variableName, Type? runtimeType)
        {
            if (runtimeType == null) return;
            if (_scopeVariables.TryGetValue(scopeName, out var list))
            {
                var vi = list.FirstOrDefault(v => v.Name == variableName);
                if (vi != null)
                {
                    vi.RuntimeIntrinsicType = runtimeType;
                }
            }
        }

        /// <summary>
        /// Marks a variable as uncaptured (no scope backing field needed, will use local variable).
        /// </summary>
        public void MarkAsUncaptured(string scopeName, string variableName)
        {
            if (!_uncapturedVariables.ContainsKey(scopeName))
                _uncapturedVariables[scopeName] = new HashSet<string>();
            
            _uncapturedVariables[scopeName].Add(variableName);
        }

        /// <summary>
        /// Checks if a variable is uncaptured (no scope backing field, uses local variable).
        /// </summary>
        public bool IsUncaptured(string scopeName, string variableName)
        {
            return _uncapturedVariables.TryGetValue(scopeName, out var vars) && vars.Contains(variableName);
        }

        /// <summary>
        /// Gets all uncaptured variables for a specific scope.
        /// </summary>
        public IEnumerable<string> GetUncapturedVariables(string scopeName)
        {
            return _uncapturedVariables.GetValueOrDefault(scopeName, new HashSet<string>());
        }
    }

    /// <summary>
    /// Information about a variable including its scope, type, and metadata handles.
    /// </summary>
    public class VariableInfo
    {
        public string Name { get; set; } = string.Empty;
        public string ScopeName { get; set; } = string.Empty;
        public VariableType VariableType { get; set; }
        public FieldDefinitionHandle FieldHandle { get; set; }
        public TypeDefinitionHandle ScopeTypeHandle { get; set; }
        public BindingKind BindingKind { get; set; }
        // Optional: CLR runtime type when known (e.g., Node module instance or intrinsic)
        public Type? RuntimeIntrinsicType { get; set; }
    }
}
