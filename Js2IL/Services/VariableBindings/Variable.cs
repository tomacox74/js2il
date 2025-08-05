using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Js2IL.Services
{
    /// <summary>
    /// Represents a JavaScript variable that maps to a field in a scope type.
    /// </summary>
    internal record Variable
    {
        public required string Name;
        
        // Scope field properties
        public string ScopeName { get; set; } = string.Empty;                    // Which scope contains this field
        public FieldDefinitionHandle FieldHandle { get; set; }                   // Metadata handle for the field
        public TypeDefinitionHandle ScopeTypeHandle { get; set; }               // Handle for the scope type
        
        public JavascriptType Type = JavascriptType.Unknown;
    }


    /// <summary>
    /// Registry of variables that uses scope instances instead of local variables.
    /// </summary>
    internal class Variables : Dictionary<string, Variable>
    {
        private readonly VariableBindings.VariableRegistry? _registry;
        private readonly Dictionary<string, int> _scopeLocalSlots = new();
        private int _nextScopeSlot = 0;

        public Variables()
        {
            // Default constructor for backward compatibility
        }

        public Variables(VariableBindings.VariableRegistry registry)
        {
            _registry = registry;
            InitializeScopeSlots();
        }

        private void InitializeScopeSlots()
        {
            if (_registry == null) return;
            
            // Assign local variable slots for each scope type
            foreach (var scopeName in _registry.GetAllScopeNames())
            {
                _scopeLocalSlots[scopeName] = _nextScopeSlot++;
            }
        }

        public void CreateFunctionVariable(string name)
        {
            if (this.ContainsKey(name))
            {
                throw new InvalidOperationException($"Variable '{name}' already exists.");
            }

            var variable = new Variable { Name = name, Type = JavascriptType.Function };
            this[name] = variable;
        }

        public Variable CreateLocal(string name)
        {
            // If we have a registry, try to get the variable from it
            if (_registry != null)
            {
                var variableInfo = _registry.FindVariable(name);
                if (variableInfo != null)
                {
                    var variable = new Variable 
                    { 
                        Name = name,
                        ScopeName = variableInfo.ScopeName,
                        FieldHandle = variableInfo.FieldHandle,
                        ScopeTypeHandle = variableInfo.ScopeTypeHandle,
                        Type = JavascriptType.Unknown // Map from VariableType to JavascriptType if needed
                    };
                    
                    if (this.ContainsKey(name))
                    {
                        throw new InvalidOperationException($"Variable '{name}' already exists.");
                    }
                    this[name] = variable;
                    return variable;
                }
            }

            // If we reach here, the variable is not in the registry
            throw new InvalidOperationException($"Variable '{name}' not found in registry. All variables should be pre-registered.");
        }

        public Variable Get(string name)
        {
            if (this.TryGetValue(name, out var variable))
            {
                return variable;
            }
            throw new KeyNotFoundException($"Variable '{name}' not found.");
        }

        public int GetScopeLocalSlot(string scopeName)
        {
            return _scopeLocalSlots.GetValueOrDefault(scopeName, -1);
        }

        /// <summary>
        /// Gets the variable registry if available.
        /// </summary>
        public VariableBindings.VariableRegistry? GetVariableRegistry()
        {
            return _registry;
        }

        /// <summary>
        /// Creates a scope instance and returns the local variable index for it.
        /// </summary>
        public int CreateScopeInstance(string scopeName)
        {
            if (!_scopeLocalSlots.ContainsKey(scopeName))
            {
                _scopeLocalSlots[scopeName] = _nextScopeSlot++;
            }
            return _scopeLocalSlots[scopeName];
        }

        public IEnumerable<string> GetAllScopeNames()
        {
            return _scopeLocalSlots.Keys;
        }

        public int GetNumberOfLocals()
        {
            // Return number of scope instances (not individual variables)
            return _nextScopeSlot;
        }
    }
}
