using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

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

        /// <summary>
        /// Adds a variable to the registry with its scope and field information.
        /// </summary>
        public void AddVariable(string scopeName, string variableName, VariableType type, 
                               FieldDefinitionHandle fieldHandle, TypeDefinitionHandle scopeTypeHandle)
        {
            if (!_scopeVariables.ContainsKey(scopeName))
                _scopeVariables[scopeName] = new List<VariableInfo>();

            _scopeVariables[scopeName].Add(new VariableInfo
            {
                Name = variableName,
                ScopeName = scopeName,
                VariableType = type,
                FieldHandle = fieldHandle,
                ScopeTypeHandle = scopeTypeHandle
            });

            if (!_scopeFields.ContainsKey(scopeName))
                _scopeFields[scopeName] = new Dictionary<string, FieldDefinitionHandle>();
            
            _scopeFields[scopeName][variableName] = fieldHandle;
            _scopeTypes[scopeName] = scopeTypeHandle;
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
            return _scopeTypes[scopeName];
        }

        /// <summary>
        /// Gets all scope names in the registry.
        /// </summary>
        public IEnumerable<string> GetAllScopeNames()
        {
            return _scopeVariables.Keys;
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
    }
}
