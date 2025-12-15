using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using JavaScriptRuntime;

namespace Js2IL.Services
{
    /// <summary>
    /// Represents a JavaScript variable that maps to a field in a scope type.
    /// </summary>
    internal abstract record Variable
    {
        public required string Name;
        
        public JavascriptType Type = JavascriptType.Unknown;

        private Type? _clrType;

        // If this variable holds a known intrinsic runtime object (e.g., Node module instance),
        // capture its CLR type so emitters can bind directly to its methods.
        public required Type? ClrType 
        { 
            get => _clrType;
            set
            {
                if (IsStableType && _clrType != null && _clrType != value)
                {
                    throw new InvalidOperationException(
                        $"Attempted to change ClrType of stable-typed variable '{Name}' " +
                        $"from '{_clrType.FullName}' to '{value?.FullName}'. This indicates a bug in type inference.");
                }
                _clrType = value;
            }
        }

        /// <summary>
        /// Indicates whether the variable's type has been inferred during static analysis
        /// and is known to never change. When true, any attempt to change ClrType is a bug.
        /// </summary>
        public bool IsStableType { get; init; } = false;

        // Unified optional metadata for compatibility with existing emitters
        public bool IsParameter { get; init; } = false;
        // For parameters: IL argument index (including any leading non-JS params already accounted for by caller)
        public int ParameterIndex { get; init; } = -1;
        // Declaring scope name for field-backed variables
        public string ScopeName { get; init; } = string.Empty;
        // Field handle for ldfld/stfld (for field-backed variables)
        public FieldDefinitionHandle FieldHandle { get; init; }
        // Local variable slot index for uncaptured variables (or -1 if not a local)
        public int LocalSlot { get; init; } = -1;
    }

    internal record LocalVariable : Variable;

    internal record ParameterVariable : Variable;

    internal record ScopeVariable : Variable
    {
        // Index into the scopes[] array for parent scope access
        public int ParentScopeIndex { get; init; } = -1;
    }

    internal enum ObjectReferenceLocation
    {
        Local,
        Parameter,
        ScopeArray
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
    internal class Variables
    {
        // Cache of resolved variables by identifier name
        private readonly Dictionary<string, Variable> _variables = new();

        // Backing registry (kept internal; callers shouldn't need it)
        private readonly VariableBindings.VariableRegistry _registry;

        // Parent scope name -> index in object[] scopes passed to this function
        private readonly Dictionary<string, int> _parentScopeIndices = new();

        // Parameter name -> 0-based parameter index
        private readonly Dictionary<string, int> _parameterIndices = new();

        // Current function (or global) scope name and global root scope name
        private readonly string _scopeName;
        private readonly string _globalScopeName;

        // Whether this function has a local scope instance (ldloc.0)
        private bool _hasLocalScope;

        // Back-compat support for creating locals for arbitrary scope names when requested
        private readonly Dictionary<string, int> _createdLocalScopes = new();
        // Stack of active lexical (block) scope names (innermost on top)
        private readonly Stack<string> _lexicalScopeStack = new();
        
        // Local variable slot allocation for uncaptured variables
        private readonly Dictionary<string, int> _localVariableSlots = new();
        // Cache of block-scope local variables by slot, for GetLocalVariableType lookup
        private readonly Dictionary<int, Variable> _blockScopeLocalsBySlot = new();
        private int _nextLocalSlot = 0; // Start after scope instance local (if any)

        /// <summary>
        /// Initializes a new instance of the <see cref="Variables"/> class for the global scope
        /// </summary>
        /// <param name="scopeName">The name of the global scope.</param>
        public Variables(VariableBindings.VariableRegistry registry, string scopeName)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _scopeName = scopeName ?? throw new ArgumentNullException(nameof(scopeName));
            _globalScopeName = scopeName;
            // Main/global should allocate one local slot for its scope instance (ldloc.0)
            _hasLocalScope = true;
            // Reserve slot 0 for scope instance
            _nextLocalSlot = 1;
        }

        public Variables(Variables parentVariables, string scopeName, IEnumerable<string> parameterNames, bool isNestedFunction)
        {
            if (parentVariables == null) throw new ArgumentNullException(nameof(parentVariables));
            if (scopeName == null) throw new ArgumentNullException(nameof(scopeName));
            if (parameterNames == null) throw new ArgumentNullException(nameof(parameterNames));

            _registry = parentVariables._registry;
            _scopeName = scopeName;
            _globalScopeName = parentVariables._globalScopeName;
            // Do not assume a local scope exists; only create when needed in emitter
            _hasLocalScope = false;

            // Build parameter map using IL argument indexes for JS params.
            // Arg0 is the scopes array; JS params start at 1.
            int i = 1;
            foreach (var p in parameterNames)
            {
                if (!_parameterIndices.ContainsKey(p))
                    _parameterIndices[p] = i;
                i++;
            }
            // Do not force a local for parameters; a local scope will be created only if fields exist

            // Parent scopes passed to this function
            // Top-level function: only [global]
            // Nested function: [global, parent function]
            _parentScopeIndices[_globalScopeName] = 0;
            if (isNestedFunction && parentVariables._scopeName != _globalScopeName)
            {
                _parentScopeIndices[parentVariables._scopeName] = 1;
            }
        }

        /// <summary>
        /// Constructor for class methods/constructors that receive explicit parent scopes via this._scopes field.
        /// </summary>
        /// <param name="parameterStartIndex">IL argument index where JS parameters begin (1 for methods, 2 for constructors with scopes parameter)</param>
        public Variables(Variables parentVariables, string scopeName, IEnumerable<string> parameterNames, IReadOnlyList<string> parentScopeNames, int parameterStartIndex = 1)
        {
            if (parentVariables == null) throw new ArgumentNullException(nameof(parentVariables));
            if (scopeName == null) throw new ArgumentNullException(nameof(scopeName));
            if (parameterNames == null) throw new ArgumentNullException(nameof(parameterNames));
            if (parentScopeNames == null) throw new ArgumentNullException(nameof(parentScopeNames));

            _registry = parentVariables._registry;
            _scopeName = scopeName;
            _globalScopeName = parentVariables._globalScopeName;
            _hasLocalScope = false;

            // Build parameter map using IL argument indexes for JS params.
            // Arg0 is 'this' for instance methods.
            // For methods: JS params start at 1 (arg0=this).
            // For constructors with scopes: JS params start at 2 (arg0=this, arg1=scopes[]).
            int i = parameterStartIndex;
            foreach (var p in parameterNames)
            {
                if (!_parameterIndices.ContainsKey(p))
                    _parameterIndices[p] = i;
                i++;
            }

            // Parent scopes are explicitly specified (from DetermineScopesForDelegateCall result)
            // Map each scope name to its index in the runtime scopes array
            for (int idx = 0; idx < parentScopeNames.Count; idx++)
            {
                _parentScopeIndices[parentScopeNames[idx]] = idx;
            }
        }

        public Variable? FindVariable(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            // 1. Check innermost active lexical (block) scopes first for shadowing (do NOT cache)
            foreach (var scopeName in _lexicalScopeStack)
            {
                if (TryResolveFieldBackedVariable(scopeName, name, out var v))
                {
                    return v; // no caching so pop restores outer binding automatically
                }
            }

            // 2. Existing cached (non-lexical) variable
            if (_variables.TryGetValue(name, out var cached)) return cached;

            // Parameter resolution: parameters are always loaded via ldarg, even if they have a field
            // The field is used for closure capture AFTER the parameter is copied to it
            if (_parameterIndices.TryGetValue(name, out var pindex))
            {
                FieldDefinitionHandle paramFieldHandle = default;
                Type? paramRuntimeType = null;
                bool paramIsStableType = false;
                
                try
                {
                    paramFieldHandle = _registry.GetFieldHandle(_scopeName, name);
                    var viParamField = _registry.GetVariableInfo(_scopeName, name) ?? _registry.FindVariable(name);
                    paramRuntimeType = viParamField?.ClrType;
                    paramIsStableType = viParamField?.IsStableType ?? false;
                }
                catch (KeyNotFoundException)
                {
                    // Parameter has no field backing, which is normal for simple parameters
                    var viParam = _registry.GetVariableInfo(_scopeName, name) ?? _registry.FindVariable(name);
                    paramRuntimeType = viParam?.ClrType;
                    paramIsStableType = viParam?.IsStableType ?? false;
                }
                
                // Always return ParameterVariable with IsParameter = true so it loads via ldarg
                var p = new ParameterVariable 
                { 
                    Name = name, 
                    ParameterIndex = pindex, 
                    IsParameter = true, 
                    FieldHandle = paramFieldHandle,
                    ScopeName = _scopeName,
                    Type = JavascriptType.Object, 
                    ClrType = paramRuntimeType,
                    IsStableType = paramIsStableType
                };
                _variables[name] = p;
                return p;
            }
            
            // Check if variable is uncaptured in the current function scope first
            if (_registry != null && _registry.IsUncaptured(_scopeName, name))
            {
                // Check if we've already resolved this scope-qualified variable
                var cacheKey = $"{_scopeName}::{name}";
                if (_variables.ContainsKey(cacheKey))
                {
                    return _variables[cacheKey];
                }
                
                var viUncaptured = _registry.GetVariableInfo(_scopeName, name);
                var localSlot = AllocateLocalSlot(_scopeName, name);
                var lvUncaptured = new LocalVariable
                {
                    Name = name,
                    LocalSlot = localSlot,
                    ScopeName = _scopeName,
                    Type = JavascriptType.Unknown,
                    ClrType = viUncaptured?.ClrType,
                    IsStableType = viUncaptured?.IsStableType ?? false
                };
                // Cache with scope-qualified key to allow proper shadowing in nested blocks
                _variables[cacheKey] = lvUncaptured;
                return lvUncaptured;
            }
            
            // Prefer a field in the current function scope (if any) before falling back to registry-wide search
            if (_registry != null)
            {
                try
                {
                    var currentScopeField = _registry.GetFieldHandle(_scopeName, name);
                    var viDirect = _registry.GetVariableInfo(_scopeName, name) ?? _registry.FindVariable(name);
                    var lvDirect = new LocalVariable
                    {
                        Name = name,
                        FieldHandle = currentScopeField,
                        ScopeName = _scopeName,
                        Type = JavascriptType.Unknown,
                        ClrType = viDirect?.ClrType,
                        IsStableType = viDirect?.IsStableType ?? false
                    };
                    _variables[name] = lvDirect; // cache since it's stable for duration of method
                    return lvDirect;
                }
                catch (KeyNotFoundException)
                {
                    // Not in current scope; continue
                }
            }

            // Look up field-backed variables anywhere (may return parent/global first depending on insertion order)
            var variableInfo = _registry?.FindVariable(name);
            if (variableInfo == null)
            {
                return null;
            }

            // Parent or other ancestor scope field
            if (_parentScopeIndices.TryGetValue(variableInfo.ScopeName, out var idx))
            {
                var sv = new ScopeVariable
                {
                    Name = name,
                    ScopeName = variableInfo.ScopeName,
                    ParentScopeIndex = idx,
                    FieldHandle = variableInfo.FieldHandle,
                    Type = JavascriptType.Unknown,
                    ClrType = variableInfo.ClrType,
                    IsStableType = variableInfo.IsStableType
                };
                _variables[name] = sv;
                return sv;
            }

            // If the variable actually belongs to current scope but was not found earlier (edge case), treat as local
            if (variableInfo.ScopeName == _scopeName)
            {
                var lv = new LocalVariable
                {
                    Name = name,
                    FieldHandle = variableInfo.FieldHandle,
                    ScopeName = variableInfo.ScopeName,
                    Type = JavascriptType.Unknown,
                    ClrType = variableInfo.ClrType,
                    IsStableType = variableInfo.IsStableType
                };
                _variables[name] = lv;
                return lv;
            }

            // Unknown scope in current context
            return null;
        }


        // Back-compat helper: maps a scope name to where it can be loaded (ldloc.0 or scopes[])
        public ScopeObjectReference GetScopeLocalSlot(string scopeName)
        {
            if (string.IsNullOrEmpty(scopeName))
            {
                return new ScopeObjectReference { Location = ObjectReferenceLocation.Local, Address = -1 };
            }

            if (scopeName == _scopeName)
            {
                return new ScopeObjectReference
                {
                    Location = ObjectReferenceLocation.Local,
                    Address = _hasLocalScope ? 0 : -1
                };
            }

            // Additional locals explicitly registered (e.g., Main's extra function scope locals)
            if (_createdLocalScopes.TryGetValue(scopeName, out var localIndex))
            {
                return new ScopeObjectReference
                {
                    Location = ObjectReferenceLocation.Local,
                    Address = localIndex
                };
            }

            if (_parentScopeIndices.TryGetValue(scopeName, out var idx))
            {
                return new ScopeObjectReference { Location = ObjectReferenceLocation.ScopeArray, Address = idx };
            }

            return new ScopeObjectReference { Location = ObjectReferenceLocation.Local, Address = -1 };
        }

        /// <summary>
        /// Creates a scope instance reference for the given scope name.
        /// </summary>
        /// <param name="scopeName">name of the scope</param>
        /// <returns>a object reference</returns>
        public ScopeObjectReference CreateScopeInstance(string scopeName)
        {
            if (scopeName == _scopeName)
            {
                _hasLocalScope = true;
                // Reserve slot 0 for the scope instance if not already reserved
                // This happens for nested functions (arrow functions, regular functions) where
                // _hasLocalScope starts as false and _nextLocalSlot starts at 0
                if (_nextLocalSlot == 0)
                {
                    _nextLocalSlot = 1; // Reserve slot 0, next allocations start at 1
                }
                // Do NOT register the current scope in _createdLocalScopes; it is already
                // accounted for by _hasLocalScope. Registering it caused double-counting
                // in GetNumberOfLocals producing an extra (unused) local slot and widespread
                // snapshot diffs (.locals init ([0] object, [1] object) instead of a single local).
                return new ScopeObjectReference { Location = ObjectReferenceLocation.Local, Address = 0 };
            }
            // Support block lexical scopes: allocate a new local slot on demand.
            if (scopeName.StartsWith("Block_L", StringComparison.Ordinal))
            {
                // Avoid double allocation if already present
                var existing = GetScopeLocalSlot(scopeName);
                if (existing.Address >= 0) return existing;
                int newIndex = AllocateBlockScopeLocal(scopeName);
                return new ScopeObjectReference { Location = ObjectReferenceLocation.Local, Address = newIndex };
            }
            // Other non-current scopes (e.g., parent function scopes) are accessed via scope array, not locals.
            return new ScopeObjectReference { Location = ObjectReferenceLocation.Local, Address = -1 };
        }

        // Back-compat: used by some emit logic to enumerate known scopes; include current + parents + global
        public IEnumerable<string> GetAllScopeNames()
        {
            var list = new List<string> { _globalScopeName, _scopeName };
            list.AddRange(_parentScopeIndices.Keys);
            // Include any additional locals registered (for Main)
            list.AddRange(_createdLocalScopes.Keys);
            return list.Distinct();
        }

        /// <summary>
        /// Checks if the given scope name is the current function's scope.
        /// </summary>
        public bool IsCurrentFunctionScope(string scopeName)
        {
            return scopeName == _scopeName;
        }

        /// <summary>
        /// Gets the local scope slot for the current function.
        /// </summary>
        public ScopeObjectReference GetLocalScopeSlot()
        {
            return new ScopeObjectReference
            {
                Location = ObjectReferenceLocation.Local,
                Address = _hasLocalScope ? 0 : -1
            };
        }

        /// <summary>
        /// Gets the current function's scope name.
        /// </summary>
        public string GetLeafScopeName()
        {
            return _scopeName;
        }

        /// <summary>
        /// Gets the number of local variables in the current function's scope.
        /// Includes scope instance (if present), additional scope locals, and uncaptured variables.
        /// </summary>
        public int GetNumberOfLocals()
        {
            // Use the allocated slot count which includes everything
            return _nextLocalSlot;
        }

        public VariableBindings.VariableRegistry GetVariableRegistry()
        {
            return _registry;
        }

        public string GetCurrentScopeName()
        {
            return _scopeName;
        }
        
        /// <summary>
        /// Allocates a local variable slot for an uncaptured variable.
        /// Uses scope-qualified name (scopeName::variableName) to ensure variables
        /// with the same name in different scopes get different slots.
        /// Returns the slot index.
        /// </summary>
        public int AllocateLocalSlot(string scopeName, string variableName)
        {
            var key = $"{scopeName}::{variableName}";
            if (!_localVariableSlots.ContainsKey(key))
            {
                _localVariableSlots[key] = _nextLocalSlot++;
            }
            return _localVariableSlots[key];
        }
        
        /// <summary>
        /// Gets the local variable slot index for a variable, or -1 if not allocated.
        /// </summary>
        public int GetLocalSlot(string variableName)
        {
            return _localVariableSlots.TryGetValue(variableName, out var slot) ? slot : -1;
        }
        
        /// <summary>
        /// Gets the total number of local variable slots allocated (including scope instance).
        /// Used for creating the local variable signature.
        /// </summary>
        public int GetLocalSlotCount()
        {
            return _nextLocalSlot;
        }

        /// <summary>
        ///  Gets the type handle for a local variable at the specified index.
        ///  Returns the scope class type for scope instances, or null for non-scope locals.
        /// </summary>
        /// <param name="localIndex">The local variable index.</param>
        /// <returns>The type handle for scope locals, or null for non-scope locals (which will default to Object type).</returns>
        public EntityHandle? GetLocalVariableType(int localIndex, BaseClassLibraryReferences bclReferences)
        {
            // Local 0 is always the current function/global scope (if _hasLocalScope)
            if (localIndex == 0 && _hasLocalScope)
            {
                try
                {
                    var scopeTypeHandle = _registry?.GetScopeTypeHandle(_scopeName);
                    if (scopeTypeHandle.HasValue && !scopeTypeHandle.Value.IsNil)
                    {
                        return scopeTypeHandle.Value;
                    }
                }
                catch (KeyNotFoundException)
                {
                    // Scope type not found, fall back to Object
                }
            }

            // Check if this local is a registered scope (from _createdLocalScopes)
            foreach (var kvp in _createdLocalScopes)
            {
                if (kvp.Value == localIndex)
                {
                    try
                    {
                        var scopeTypeHandle = _registry?.GetScopeTypeHandle(kvp.Key);
                        if (scopeTypeHandle.HasValue && !scopeTypeHandle.Value.IsNil)
                        {
                            return scopeTypeHandle.Value;
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        // Scope type not found (e.g., temporary locals like EqTmp_RHS_*), fall back to Object
                    }
                    break;
                }
            }

            // partial support for known types.. if we have a clrtype of double then return that
            // phase in support for more types in future work
            var variable = _variables.Values.FirstOrDefault(v => v.LocalSlot == localIndex);
            
            // Also check block-scope locals cache (these aren't stored in _variables for shadowing reasons)
            if (variable == null && _blockScopeLocalsBySlot.TryGetValue(localIndex, out var blockScopeVar))
            {
                variable = blockScopeVar;
            }

            // default behavior is to use the clr type object 
            // if we have detected during analysis that the variable type remains a single type for its lifetime
            // we can use a more specific clr type in the implementation
            // the advanage of this is that we don't need to box and unbox values.
            if (variable != null && variable.ClrType != null && variable.IsStableType)
            {
                if (variable.ClrType == typeof(double))
                {
                    return bclReferences.DoubleType;
                }
            }

            // Default to Object type for all other locals (including temps allocated via AllocateBlockScopeLocal)
            return null;
        }

        /// <summary>
        /// Register an additional local slot for a named scope (used by Main to model top-level and nested function scopes).
        /// </summary>
        public void RegisterAdditionalLocalScope(string scopeName, int localIndex)
        {
            if (string.IsNullOrEmpty(scopeName)) return;
            _createdLocalScopes[scopeName] = localIndex;
        }

        public void PushLexicalScope(string scopeName)
        {
            if (!string.IsNullOrEmpty(scopeName))
            {
                _lexicalScopeStack.Push(scopeName);
            }
        }

        public void PopLexicalScope(string scopeName)
        {
            if (_lexicalScopeStack.Count == 0)
            {
                return;
            }

            if (_lexicalScopeStack.Peek() == scopeName)
            {
                _lexicalScopeStack.Pop();
            }
            else
            {
                // Remove first occurrence if mis-nested (defensive)
                var temp = new Stack<string>();
                bool removed = false;
                while (_lexicalScopeStack.Count > 0)
                {
                    var s = _lexicalScopeStack.Pop();
                    if (!removed && s == scopeName)
                    {
                        removed = true;
                        continue;
                    }
                    temp.Push(s);
                }
                while (temp.Count > 0)
                {
                    _lexicalScopeStack.Push(temp.Pop());
                }
            }
        }

        public int AllocateBlockScopeLocal(string scopeName)
        {
            int index = _nextLocalSlot;
            _nextLocalSlot++; // Reserve the slot
            RegisterAdditionalLocalScope(scopeName, index);
            return index;
        }

        private bool TryResolveFieldBackedVariable(string scopeName, string name, out Variable variable)
        {
            variable = null!;
            
            // First check if this is an uncaptured variable in this specific scope
            if (_registry != null && _registry.IsUncaptured(scopeName, name))
            {
                var viUncaptured = _registry.GetVariableInfo(scopeName, name);
                // Allocate a unique local slot for this scope's variable
                // Use scope name + variable name as key to ensure each block scope gets its own slot
                var localSlot = AllocateLocalSlot(scopeName, name);
                variable = new LocalVariable
                {
                    Name = name,
                    LocalSlot = localSlot,
                    ScopeName = scopeName,
                    Type = JavascriptType.Unknown,
                    ClrType = viUncaptured?.ClrType,
                    IsStableType = viUncaptured?.IsStableType ?? false
                };
                // Cache by slot for GetLocalVariableType lookup (block scope locals aren't cached in _variables)
                _blockScopeLocalsBySlot[localSlot] = variable;
                return true;
            }
            
            // Otherwise check for field-backed variable
            try
            {
                if (_registry == null) return false;
                var fh = _registry.GetFieldHandle(scopeName, name);
                var viField = _registry.GetVariableInfo(scopeName, name);
                variable = new LocalVariable
                {
                    Name = name,
                    FieldHandle = fh,
                    ScopeName = scopeName,
                    Type = JavascriptType.Unknown,
                    ClrType = viField?.ClrType,
                    IsStableType = viField?.IsStableType ?? false
                };
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
    }
}
