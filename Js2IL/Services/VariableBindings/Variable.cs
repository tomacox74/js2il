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

        // Temporary backward compatibility property - will be removed when IL generation is updated
        [Obsolete("Use scope field access instead of local variables")]
        public int? LocalIndex { get; set; } = null;
        
        [Obsolete("Use scope field access instead of local variables")]
        public bool IsLocal => LocalIndex.HasValue;
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
#pragma warning disable CS0618 // Type or member is obsolete
                    var variable = new Variable 
                    { 
                        Name = name,
                        ScopeName = variableInfo.ScopeName,
                        FieldHandle = variableInfo.FieldHandle,
                        ScopeTypeHandle = variableInfo.ScopeTypeHandle,
                        Type = JavascriptType.Unknown, // Map from VariableType to JavascriptType if needed
                        LocalIndex = null // Scope variables don't use local indices
                    };
#pragma warning restore CS0618 // Type or member is obsolete
                    
                    if (this.ContainsKey(name))
                    {
                        throw new InvalidOperationException($"Variable '{name}' already exists.");
                    }
                    this[name] = variable;
                    return variable;
                }
            }

            // Fallback for cases where registry is not available - use old local variable approach
#pragma warning disable CS0618 // Type or member is obsolete
            var fallbackVariable = new Variable 
            { 
                Name = name,
                LocalIndex = _nextScopeSlot++ // Temporary: use scope slot counter for backward compatibility
            };
#pragma warning restore CS0618 // Type or member is obsolete
            if (this.ContainsKey(name))
            {
                throw new InvalidOperationException($"Variable '{name}' already exists.");
            }
            this[name] = fallbackVariable;
            return fallbackVariable;
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
