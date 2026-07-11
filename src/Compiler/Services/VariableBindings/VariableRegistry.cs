using System.Reflection.Metadata;
using Jroc.SymbolTables;

namespace Jroc.Services.VariableBindings
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
    /// Populated by TypeGenerator and consumed by the IR/IL compilation pipeline.
    /// Acts as a facade over ScopeMetadataRegistry for backward compatibility.
    /// </summary>
    public class VariableRegistry
    {
        private readonly Dictionary<string, List<VariableInfo>> _scopeVariables = new();
        private readonly ScopeMetadataRegistry _scopeMetadata;
        // Track uncaptured variables (no scope backing field, will use local variables)
        private readonly Dictionary<string, HashSet<string>> _uncapturedVariables = new();
        private readonly Dictionary<ObjectLiteralShapeInfo, ObjectLiteralTypeMetadata> _objectLiteralTypes = new(ReferenceEqualityComparer.Instance);

        /// <summary>
        /// Creates a new VariableRegistry with an internal ScopeMetadataRegistry.
        /// </summary>
        public VariableRegistry() : this(new ScopeMetadataRegistry())
        {
        }

        /// <summary>
        /// Creates a new VariableRegistry with the specified ScopeMetadataRegistry.
        /// </summary>
        public VariableRegistry(ScopeMetadataRegistry scopeMetadata)
        {
            _scopeMetadata = scopeMetadata ?? throw new ArgumentNullException(nameof(scopeMetadata));
        }

        /// <summary>
        /// Gets the underlying ScopeMetadataRegistry for direct access to scope/field handles.
        /// New code should depend on ScopeMetadataRegistry directly instead of VariableRegistry.
        /// </summary>
        public ScopeMetadataRegistry ScopeMetadata => _scopeMetadata;

        /// <summary>
        /// Registers the generated CLR metadata for an eligible object literal shape.
        /// </summary>
        public void RegisterObjectLiteralType(
            ObjectLiteralShapeInfo shape,
            string typeName,
            TypeDefinitionHandle typeHandle,
            MethodDefinitionHandle constructorHandle,
            IReadOnlyDictionary<string, FieldDefinitionHandle> fieldHandlesByMemberName,
            IReadOnlyDictionary<string, Type> fieldClrTypesByMemberName)
        {
            if (shape == null) throw new ArgumentNullException(nameof(shape));
            if (!shape.IsEligible)
            {
                throw new ArgumentException("Only eligible object literal shapes can be registered.", nameof(shape));
            }
            if (typeHandle.IsNil)
            {
                throw new ArgumentException("Object literal type handle cannot be nil.", nameof(typeHandle));
            }
            if (constructorHandle.IsNil)
            {
                throw new ArgumentException("Object literal constructor handle cannot be nil.", nameof(constructorHandle));
            }

            var metadata = new ObjectLiteralTypeMetadata(
                shape,
                typeName,
                typeHandle,
                constructorHandle,
                new Dictionary<string, FieldDefinitionHandle>(fieldHandlesByMemberName, StringComparer.Ordinal),
                new Dictionary<string, Type>(fieldClrTypesByMemberName, StringComparer.Ordinal));

            _objectLiteralTypes[shape] = metadata;
        }

        public bool TryGetObjectLiteralType(ObjectLiteralShapeInfo shape, out ObjectLiteralTypeMetadata metadata)
            => _objectLiteralTypes.TryGetValue(shape, out metadata!);

        public IReadOnlyCollection<ObjectLiteralTypeMetadata> GetObjectLiteralTypes()
            => _objectLiteralTypes.Values.ToList();

        /// <summary>
        /// Adds a variable to the registry with its scope and field information (legacy overload; assumes Var binding).
        /// </summary>
        public void AddVariable(string scopeName, string variableName, VariableType type,
                               FieldDefinitionHandle fieldHandle, TypeDefinitionHandle scopeTypeHandle, Type? clrType)
        {
            AddVariable(scopeName, variableName, type, fieldHandle, scopeTypeHandle, BindingKind.Var, clrType, isStableType: false);
        }

        /// <summary>
        /// Adds a variable to the registry with its scope and field information.
        /// </summary>
        public void AddVariable(string scopeName, string variableName, VariableType type,
                               FieldDefinitionHandle fieldHandle, TypeDefinitionHandle scopeTypeHandle,
                               BindingKind bindingKind, Type? clrType, bool isStableType, Type? declaredFieldClrType = null)
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
                BindingKind = bindingKind,
                ClrType = clrType,
                IsStableType = isStableType
            });

            // Delegate field/type registration to ScopeMetadataRegistry
            _scopeMetadata.RegisterField(scopeName, variableName, fieldHandle);
            if (!fieldHandle.IsNil)
            {
                // Keep this in sync with TypeGenerator's field signature emission.
                var hasExplicitDeclaredFieldType = declaredFieldClrType != null;
                var resolvedDeclaredFieldType = declaredFieldClrType ?? typeof(object);
                if (!hasExplicitDeclaredFieldType && resolvedDeclaredFieldType == typeof(object) && isStableType && clrType != null)
                {
                    if (clrType == typeof(double)
                        || clrType == typeof(bool)
                        || clrType == typeof(string)
                        || clrType == typeof(JavaScriptRuntime.Array)
                        || clrType == typeof(JavaScriptRuntime.RegExp))
                    {
                        resolvedDeclaredFieldType = clrType;
                    }
                }
                _scopeMetadata.RegisterFieldClrType(scopeName, variableName, resolvedDeclaredFieldType);
            }
            if (!scopeTypeHandle.IsNil)
            {
                _scopeMetadata.RegisterScopeType(scopeName, scopeTypeHandle);
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
            return _scopeMetadata.GetFieldHandle(scopeName, variableName);
        }

        /// <summary>
        /// Gets the type handle for a specific scope.
        /// </summary>
        public TypeDefinitionHandle GetScopeTypeHandle(string scopeName)
        {
            return _scopeMetadata.GetScopeTypeHandle(scopeName);
        }

        /// <summary>
        /// Ensures a scope type handle is registered even if there are no variables/fields.
        /// (Used for empty class methods so a scope instance can still be created when needed.)
        /// </summary>
        public void EnsureScopeType(string scopeName, TypeDefinitionHandle typeHandle)
        {
            _scopeMetadata.EnsureScopeType(scopeName, typeHandle);
        }

        /// <summary>
        /// Gets all scope names in the registry.
        /// </summary>
        public IEnumerable<string> GetAllScopeNames()
        {
            // Return all scopes that have either variables or just type definitions
            return _scopeVariables.Keys.Union(_scopeMetadata.GetAllScopeNames());
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
        public void SetClrType(string scopeName, string variableName, Type? clrType)
        {
            if (clrType == null) return;
            if (_scopeVariables.TryGetValue(scopeName, out var list))
            {
                var vi = list.FirstOrDefault(v => v.Name == variableName);
                if (vi != null)
                {
                    if (vi.IsStableType && vi.ClrType != null && vi.ClrType != clrType)
                    {
                        throw new InvalidOperationException(
                            $"Attempted to change ClrType of stable-typed variable '{variableName}' in scope '{scopeName}' " +
                            $"from '{vi.ClrType.FullName}' to '{clrType.FullName}'. This indicates a bug in type inference.");
                    }
                    vi.ClrType = clrType;
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

        /// <summary>
        /// Checks if a scope has any field-backed variables (non-nil FieldHandle).
        /// Scopes with only uncaptured variables (using locals) return false.
        /// </summary>
        public bool ScopeHasFieldBackedVariables(string scopeName)
        {
            if (!_scopeVariables.TryGetValue(scopeName, out var variables))
                return false;
            
            return variables.Any(v => !v.FieldHandle.IsNil);
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
        public Type? ClrType { get; set; }
        /// <summary>
        /// Indicates whether the variable's type has been inferred during static analysis
        /// and is known to never change. When true, any attempt to change ClrType is a bug.
        /// </summary>
        public bool IsStableType { get; set; }
    }
}
