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

    // Parameter support (not backed by scope field)
    public bool IsParameter { get; set; } = false;            // True if this variable represents a method parameter
    public int ParameterIndex { get; set; } = -1;              // 0-based parameter index in method signature
    }

    internal enum ObjectReferenceLocation
    {
        Local,
        Parameter
    }

    internal record ScopeObjectReference
    {
        public required ObjectReferenceLocation Location;
        public required int Address; // Name of the variable in the scope
    }

    /// <summary>
    /// Variables is a map from a variable name in the AST to where the variable is stored.
    /// There is 1 instance of Variables per dotnet method that is being being generated.
    /// </summary>
    internal class Variables : Dictionary<string, Variable>
    {
        private readonly VariableBindings.VariableRegistry _registry;
        private readonly Dictionary<string, ScopeObjectReference> _scopeLocalSlots = new();
        private int _nextScopeSlot = 0;

        private readonly string _leafScopeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Variables"/> class for a global scope of the given name.
        /// </summary>
        /// <param name="scopeName">The name of the scope.</param>
        public Variables(VariableBindings.VariableRegistry registry, string scopeName)
        {
            _leafScopeName = scopeName;
            _registry = registry;

            // Initialize with a specific scope name
            _scopeLocalSlots[scopeName] = new ScopeObjectReference
            {
                Location = ObjectReferenceLocation.Local,
                Address = _nextScopeSlot++
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variables"/> class for a function or array function declared in the gloval scope.
        /// </summary>
        public Variables(Variables globalVariables)
        {
            _registry = globalVariables._registry;
            _leafScopeName = globalVariables._leafScopeName;

            _scopeLocalSlots[globalVariables._leafScopeName] = new ScopeObjectReference
            {
                Location = ObjectReferenceLocation.Parameter,
                Address = 0
            };
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

        public Variable? FindVariable(string name)
        {
            if (!this.TryGetValue(name, out var variable))
            {
                var variableInfo = _registry?.FindVariable(name);
                if (variableInfo != null)
                {
                    variable = new Variable
                    {
                        Name = name,
                        ScopeName = variableInfo.ScopeName,
                        FieldHandle = variableInfo.FieldHandle,
                        ScopeTypeHandle = variableInfo.ScopeTypeHandle,
                        Type = JavascriptType.Unknown // Map from VariableType to JavascriptType if needed
                    };
                }
            }
            return variable;
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

        public Variable AddParameter(string name, int parameterIndex)
        {
            if (this.ContainsKey(name))
            {
                // if existing variable is a scope field that's shadowed by parameter, favor parameter
                // but for now throw to catch unexpected duplicates
                throw new InvalidOperationException($"Variable '{name}' already exists when adding parameter.");
            }
            var variable = new Variable
            {
                Name = name,
                IsParameter = true,
                ParameterIndex = parameterIndex,
                Type = JavascriptType.Object
            };
            this[name] = variable;
            return variable;
        }


        public ScopeObjectReference GetScopeLocalSlot(string scopeName)
        {
            return _scopeLocalSlots.GetValueOrDefault(scopeName, new ScopeObjectReference
            {
                Location = ObjectReferenceLocation.Local,
                Address = -1
            });
        }

        /// <summary>
        /// Creates a scope instance and returns the local variable index for it.
        /// </summary>
        public ScopeObjectReference CreateScopeInstance(string scopeName)
        {
            if (!_scopeLocalSlots.ContainsKey(scopeName))
            {
                _scopeLocalSlots[scopeName] = new ScopeObjectReference
                {
                    Location = ObjectReferenceLocation.Local,
                    Address = _nextScopeSlot++
                };
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

        public VariableBindings.VariableRegistry GetVariableRegistry()
        {
            return _registry;
        }
    }
}
