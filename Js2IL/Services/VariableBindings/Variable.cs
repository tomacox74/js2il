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
    internal abstract record Variable
    {
        public required string Name;
        
        public JavascriptType Type = JavascriptType.Unknown;

    // Unified optional metadata for compatibility with existing emitters
    public bool IsParameter { get; init; } = false;
    // For parameters: IL argument index (including any leading non-JS params already accounted for by caller)
    public int ParameterIndex { get; init; } = -1;
    // Declaring scope name for field-backed variables
    public string ScopeName { get; init; } = string.Empty;
    // Field handle for ldfld/stfld (for field-backed variables)
    public FieldDefinitionHandle FieldHandle { get; init; }
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variables"/> to pass to a function or arrow function or a block with nested scope
        /// </summary>
        /// <param name="parentVariables">The parent variables context to inherit from.</param>
        /// <param name="scopeName">The name of the scope for this instance</param>
        /// <param name="parameterNames">The names of the parameters if these variables are for a function or arrow function</param>
        public Variables(Variables parentVariables, string scopeName, IEnumerable<string> parameterNames)
            : this(parentVariables, scopeName, parameterNames, isNestedFunction: true)
        {
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


    // Indexer for backward compatibility: get-only resolves via FindVariable
    public Variable this[string name] => FindVariable(name)!;

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

            // Parameter resolution: prefer field-backed local if present; otherwise treat as direct argument
            if (_parameterIndices.TryGetValue(name, out var pindex))
            {
                try
                {
                    var fh = _registry.GetFieldHandle(_scopeName, name);
                    var lvParamField = new LocalVariable
                    {
                        Name = name,
                        FieldHandle = fh,
                        ScopeName = _scopeName,
                        Type = JavascriptType.Unknown
                    };
                    _variables[name] = lvParamField;
                    return lvParamField;
                }
                catch (KeyNotFoundException)
                {
                    var p = new ParameterVariable { Name = name, ParameterIndex = pindex, IsParameter = true, Type = JavascriptType.Object };
                    _variables[name] = p;
                    return p;
                }
            }

            // Prefer a field in the current function scope (if any) before falling back to registry-wide search
            if (_registry != null)
            {
                try
                {
                    var currentScopeField = _registry.GetFieldHandle(_scopeName, name);
                    var lvDirect = new LocalVariable
                    {
                        Name = name,
                        FieldHandle = currentScopeField,
                        ScopeName = _scopeName,
                        Type = JavascriptType.Unknown
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
                    Type = JavascriptType.Unknown
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
                    Type = JavascriptType.Unknown
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

        // Back-compat: create a local scope instance for the current function only.
        public ScopeObjectReference CreateScopeInstance(string scopeName)
        {
            if (scopeName == _scopeName)
            {
                _hasLocalScope = true;
                // Do NOT register the current scope in _createdLocalScopes; it is already
                // accounted for by _hasLocalScope. Registering it caused double-counting
                // in GetNumberOfLocals producing an extra (unused) local slot and widespread
                // snapshot diffs (.locals init ([0] object, [1] object) instead of a single local).
                return new ScopeObjectReference { Location = ObjectReferenceLocation.Local, Address = 0 };
            }
            // Non-current scopes are not created as locals in the new model; indicate not available
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

    // CreateLocal removed: callers should use FindVariable(name) for resolution.




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
        /// </summary>
    public int GetNumberOfLocals()
        {
            // Base local is function/global scope if present plus any additional registered scopes (functions, blocks)
            int count = _hasLocalScope ? 1 : 0;
            // _createdLocalScopes should no longer contain the current scope (see CreateScopeInstance).
            // Guard against legacy state where it might have been added previously (defensive cleanup semantics).
            if (_createdLocalScopes.ContainsKey(_scopeName))
            {
                _createdLocalScopes.Remove(_scopeName);
            }
            count += _createdLocalScopes.Count; // function + block scopes added
            return count;
        }

        public VariableBindings.VariableRegistry GetVariableRegistry()
        {
            return _registry;
        }

        /// <summary>
        /// Register an additional local slot for a named scope (used by Main to model top-level and nested function scopes).
        /// </summary>
        public void RegisterAdditionalLocalScope(string scopeName, int localIndex)
        {
            if (string.IsNullOrEmpty(scopeName)) return;
            _createdLocalScopes[scopeName] = localIndex;
        }

        private bool TryResolveFieldBackedVariable(string scopeName, string name, out Variable variable)
        {
            variable = null!;
            try
            {
                var fh = _registry.GetFieldHandle(scopeName, name);
                variable = new LocalVariable
                {
                    Name = name,
                    FieldHandle = fh,
                    ScopeName = scopeName,
                    Type = JavascriptType.Unknown
                };
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        // Lexical scope management for blocks
        public void PushLexicalScope(string scopeName) {
            if (!string.IsNullOrEmpty(scopeName)) _lexicalScopeStack.Push(scopeName);
        }
        public void PopLexicalScope(string scopeName) {
            if (_lexicalScopeStack.Count == 0) return;
            if (_lexicalScopeStack.Peek() == scopeName) _lexicalScopeStack.Pop();
            else
            {
                // Remove first occurrence if mis-nested (defensive)
                var temp = new Stack<string>();
                bool removed = false;
                while (_lexicalScopeStack.Count > 0)
                {
                    var s = _lexicalScopeStack.Pop();
                    if (!removed && s == scopeName) { removed = true; continue; }
                    temp.Push(s);
                }
                while (temp.Count > 0) _lexicalScopeStack.Push(temp.Pop());
            }
        }

        // Allocate a new local slot for a block scope and register it; returns local index.
        public int AllocateBlockScopeLocal(string scopeName)
        {
            int index = GetNumberOfLocals();
            RegisterAdditionalLocalScope(scopeName, index);
            return index;
        }
    }
}
