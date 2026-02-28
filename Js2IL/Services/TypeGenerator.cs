using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using Js2IL.SymbolTables;
using Js2IL.Services.VariableBindings;
using System.Linq;
using Js2IL.Utilities.Ecma335;
using Js2IL.Utilities;
using System.Text;

namespace Js2IL.Services
{
    /// <summary>
    /// Generates .NET type definitions from a JavaScript scope tree.
    /// Each scope becomes a class with variables as instance fields.
    /// Multiple instances can exist for the same scope type.
    /// </summary>
    internal class TypeGenerator
    {
        private readonly MetadataBuilder _metadataBuilder;
        private readonly BaseClassLibraryReferences _bclReferences;
        private readonly MethodBodyStreamEncoder _methodBodyStream;
        private readonly Dictionary<string, TypeDefinitionHandle> _scopeTypes;
        private readonly Dictionary<string, List<FieldDefinitionHandle>> _scopeFields;
        private readonly Dictionary<string, List<string>> _scopeFieldNames;
        private readonly Dictionary<string, Dictionary<string, FieldDefinitionHandle>> _scopeFieldHandlesByName;
        private readonly VariableRegistry _variableRegistry;
        private readonly bool _emitDebuggerDisplay;
        private readonly int _deferredCtorStartRow;
        private int _nextDeferredCtorRow;
        private readonly List<(string ScopeKey, string Namespace, string TypeName, bool IsAsync, bool IsGenerator, MethodDefinitionHandle ExpectedCtor)> _deferredCtorPlan;

        public TypeGenerator(
            MetadataBuilder metadataBuilder,
            BaseClassLibraryReferences bclReferences,
            MethodBodyStreamEncoder methodBodyStream,
            VariableRegistry variableRegistry,
            int deferredCtorStartRow,
            bool emitDebuggerDisplay)
        {
            _metadataBuilder = metadataBuilder;
            _bclReferences = bclReferences;
            _methodBodyStream = methodBodyStream;
            _variableRegistry = variableRegistry;
            _scopeTypes = new Dictionary<string, TypeDefinitionHandle>();
            _scopeFields = new Dictionary<string, List<FieldDefinitionHandle>>();
            _scopeFieldNames = new Dictionary<string, List<string>>();
            _scopeFieldHandlesByName = new Dictionary<string, Dictionary<string, FieldDefinitionHandle>>();
            _deferredCtorStartRow = deferredCtorStartRow;
            _nextDeferredCtorRow = deferredCtorStartRow;
            _deferredCtorPlan = new List<(string, string, string, bool, bool, MethodDefinitionHandle)>();
            _emitDebuggerDisplay = emitDebuggerDisplay;
        }

        /// <summary>
        /// Generates .NET types from the symbol table.
        /// Returns the root type definition handle.
        /// </summary>
        public void GenerateTypes(SymbolTable symbolTable)
        {            
            // Phase 1: Create all scope types (depth-first) for every scope discovered by the SymbolTable.
            // SymbolTable already contains scopes for function declarations, function expressions, arrow functions,
            // class methods, block scopes, etc. We rely on that being exhaustive and treat all of them uniformly.
            //
            // NOTE: This phase intentionally does NOT establish NestedClass relationships.
            // Nesting relationships are recorded later once module and callable-owner TypeDefs exist
            // (see JsMethodCompiler.EstablishModuleNesting and NestedTypeRelationshipRegistry).
            CreateAllTypes(symbolTable.Root, symbolTable.Root.Name);

            // Phase 2: Populate the variable registry (fields + metadata for every binding).
            PopulateVariableRegistry(symbolTable.Root);
        }

        /// <summary>
        /// Emits all deferred scope constructors in the exact order scope TypeDefs were created.
        /// This must be called after all callable method bodies are emitted, and before module init / entrypoint
        /// methods are emitted, so TypeDef.MethodList ordering remains valid.
        /// </summary>
        public void EmitDeferredScopeConstructors()
        {
            foreach (var item in _deferredCtorPlan)
            {
                var tb = new TypeBuilder(_metadataBuilder, item.Namespace, item.TypeName);
                var actual = EmitScopeConstructor(tb, item.IsAsync, item.IsGenerator);
                if (actual != item.ExpectedCtor)
                {
                    throw new InvalidOperationException(
                        $"Deferred scope ctor MethodDef token mismatch for scope '{item.ScopeKey}'. Expected 0x{MetadataTokens.GetToken(item.ExpectedCtor):X8}, got 0x{MetadataTokens.GetToken(actual):X8}.");
                }
            }
        }

        private static string GetRegistryScopeName(Scope scope) => ScopeNaming.GetRegistryScopeName(scope);

        private static bool IsSafeInjectedCommonJsRequireBinding(Scope scope, BindingInfo binding)
        {
            return string.Equals(binding.Name, "require", StringComparison.Ordinal)
                && scope.Kind == ScopeKind.Global
                && scope.Parameters.Contains("require")
                && ReferenceEquals(binding.DeclarationNode, scope.AstNode)
                && !binding.HasWrite;
        }

        private static Type GetDeclaredScopeFieldClrType(Scope scope, BindingInfo binding)
        {
            if (IsSafeInjectedCommonJsRequireBinding(scope, binding))
            {
                return typeof(JavaScriptRuntime.CommonJS.RequireDelegate);
            }

            if (binding.IsStableType && binding.ClrType != null)
            {
                if (binding.ClrType == typeof(double)
                    || binding.ClrType == typeof(bool)
                    || binding.ClrType == typeof(string)
                    || binding.ClrType == typeof(JavaScriptRuntime.Array))
                {
                    return binding.ClrType;
                }
            }

            return typeof(object);
        }

        private static string GetClrTypeNameForScope(Scope scope)
        {
            // If the symbol table authored an explicit CLR type name for a *function scope* and it
            // represents a scope type name (e.g., Scope_ctor / Scope_get_<name> / Scope_set_<name>),
            // prefer it. Do NOT use class scope DotNetTypeName (e.g., "BitBag") here, since class
            // DotNetTypeName refers to the runtime class TypeDef, not the class scope type.
            if (scope.Kind == ScopeKind.Function
                && !string.IsNullOrWhiteSpace(scope.DotNetTypeName)
                && scope.DotNetTypeName.StartsWith("Scope", StringComparison.Ordinal))
            {
                return scope.DotNetTypeName;
            }

            // Function declarations have an associated callable owner type (Modules.<Module>.<FunctionName>).
            // To keep the IL readable and avoid awkward names like Scope/<FunctionName>, we name the
            // function's scope type "Scope" and later nest it under the callable owner type.
            if (scope.Kind == ScopeKind.Function && scope.AstNode is Acornima.Ast.FunctionDeclaration)
            {
                return "Scope";
            }

            // Anonymous callables (function expressions / arrow functions) also have dedicated owner types.
            // Name their scope types "Scope" and nest under the owner type to avoid name collisions.
            if (scope.Kind == ScopeKind.Function && (scope.AstNode is Acornima.Ast.FunctionExpression or Acornima.Ast.ArrowFunctionExpression))
            {
                return "Scope";
            }

            // Class scopes should not take the same CLR type name as the runtime class TypeDef.
            // We emit the class *scope* type as "Scope" and nest it under the class TypeDef.
            if (scope.Kind == ScopeKind.Class)
            {
                return "Scope";
            }

            // For-loop per-iteration lexical environments: name the scope type with a Scope_ prefix
            // so it can live as a sibling of the root Scope type under the callable/module owner.
            // Examples:
            //   For_L3C5   -> Scope_For_L3C5
            //   ForIn_L3C5 -> Scope_ForIn_L3C5
            //   ForOf_L3C5 -> Scope_ForOf_L3C5
            if (scope.Kind == ScopeKind.Block &&
                (scope.Name.StartsWith("For_", StringComparison.Ordinal) ||
                 scope.Name.StartsWith("ForIn_", StringComparison.Ordinal) ||
                 scope.Name.StartsWith("ForOf_", StringComparison.Ordinal)))
            {
                return $"Scope_{scope.Name}";
            }

            return scope.Name;
        }

        private void CreateTypeFields(Scope scope, TypeBuilder typeBuilder)
        {
            var scopeKey = GetRegistryScopeName(scope);
            
            // Create fields for this scope
            _scopeFields[scopeKey] = new List<FieldDefinitionHandle>();
            var scopeFields = _scopeFields[scopeKey];

            _scopeFieldNames[scopeKey] = new List<string>();
            var scopeFieldNames = _scopeFieldNames[scopeKey];

            if (!_scopeFieldHandlesByName.ContainsKey(scopeKey))
                _scopeFieldHandlesByName[scopeKey] = new Dictionary<string, FieldDefinitionHandle>();

            // Check if this is an arrow function scope (arrow functions always need parameter fields for closure semantics)
            bool isArrowFunction = scope.AstNode is Acornima.Ast.ArrowFunctionExpression;

            foreach (var kvp in scope.Bindings)
            {
                var variableName = kvp.Key;
                var binding = kvp.Value;

                // Parameters are treated as fields on the scope when:
                // 1. The parameter is captured (referenced from nested functions), OR
                // 2. This is an arrow function (needs fields for proper this binding and closure), OR
                // 3. The parameter comes from destructuring (needs storage to extract from object)
                // Simple identifier parameters that aren't captured can use ldarg directly.
                bool isParameter = scope.Parameters.Contains(variableName);
                bool isFunction = binding.Kind == BindingKind.Function;
                bool isDestructuredParameter = scope.DestructuredParameters.Contains(variableName);
                
                // Skip field creation for parameters that:
                // - Are NOT captured (not referenced by child scopes)
                // - Are NOT from destructuring (simple identifier params use ldarg)
                // - Are NOT in arrow functions (arrow functions always need parameter fields for closure semantics)
                if (isParameter && !binding.IsCaptured && !isDestructuredParameter && !isArrowFunction)
                {
                    // Simple parameter that can be accessed via ldarg - no field needed
                    continue;
                }

                // Skip field creation for uncaptured local variables (not parameters, not function declarations)
                // Function declarations (BindingKind.Function) always need fields to store delegate references
                // Uncaptured const/let/var variables can use local variables instead of fields
                // Shadowed variables (same name in nested scopes) can also use locals - each scope gets
                // its own unique local slot via AllocateLocalSlot(scopeName, variableName)
                if (!binding.IsCaptured && !isParameter && !isFunction)
                {
                    // Skip creating a field for non-captured non-parameter non-function bindings
                    continue;
                }

                var declaredFieldClrType = GetDeclaredScopeFieldClrType(scope, binding);

                // Create field signature.
                var fieldSignature = new BlobBuilder();
                var fieldTypeEncoder = new BlobEncoder(fieldSignature)
                    .Field()
                    .Type();

                if (declaredFieldClrType == typeof(double))
                {
                    fieldTypeEncoder.Double();
                }
                else if (declaredFieldClrType == typeof(bool))
                {
                    fieldTypeEncoder.Boolean();
                }
                else if (declaredFieldClrType == typeof(string))
                {
                    fieldTypeEncoder.String();
                }
                else if (declaredFieldClrType != typeof(object))
                {
                    fieldTypeEncoder.Type(
                        _bclReferences.TypeReferenceRegistry.GetOrAdd(declaredFieldClrType),
                        isValueType: false);
                }
                else
                {
                    fieldTypeEncoder.Object();
                }

                var fieldSignatureHandle = _metadataBuilder.GetOrAddBlob(fieldSignature);

                // Determine field attributes based on binding kind
                var fieldAttributes = GetFieldAttributes(binding.Kind);

                // Create the field definition
                var fieldHandle = typeBuilder.AddFieldDefinition(
                    fieldAttributes,
                    variableName,
                    fieldSignatureHandle
                );

                scopeFields.Add(fieldHandle);
                scopeFieldNames.Add(variableName);
                _scopeFieldHandlesByName[scopeKey][variableName] = fieldHandle;
            }

            // Add awaited result storage fields for async function scopes: _awaited1, _awaited2, etc.
            // State IDs start at 1, so await point N stores its result in _awaitedN.
            if (scope.IsAsync)
            {
                AddAwaitedResultFields(typeBuilder, scopeFields, scopeFieldNames, scope.AwaitPointCount, scopeKey);
            }
        }

        private void AddAwaitedResultFields(TypeBuilder typeBuilder, List<FieldDefinitionHandle> scopeFields, List<string> scopeFieldNames, int awaitPointCount, string scopeName)
        {
            for (int i = 1; i <= awaitPointCount; i++)
            {
                var fieldName = $"_awaited{i}";
                var awaitedFieldSig = new BlobBuilder();
                new BlobEncoder(awaitedFieldSig).Field().Type().Object();
                var awaitedFieldHandle = typeBuilder.AddFieldDefinition(
                    FieldAttributes.Public,
                    fieldName,
                    _metadataBuilder.GetOrAddBlob(awaitedFieldSig)
                );
                scopeFields.Add(awaitedFieldHandle);
                scopeFieldNames.Add(fieldName);
                _variableRegistry.ScopeMetadata.RegisterField(scopeName, fieldName, awaitedFieldHandle);
                _variableRegistry.ScopeMetadata.RegisterFieldClrType(scopeName, fieldName, typeof(object));
            }
        }

        /// <summary>
        /// Phase 2: Recursively creates type definitions depth-first.
        ///
        /// NOTE: The global scope type is emitted as a nested type under the module type (Modules.<ModuleName>).
        /// This keeps scope layout consistent with other nested scope types.
        /// </summary>
        private void CreateAllTypes(Scope scope, string typeName)
        {
            // Create the root scope type.
            var isGlobalScope = scope.Kind == ScopeKind.Global;
            var rootTypeName = isGlobalScope ? "Scope" : typeName;

            // All scope types are emitted with nested visibility; actual nesting relationships are
            // established later in a single sorted pass via NestedTypeRelationshipRegistry.
            var parentType = CreateScopeType(scope, rootTypeName);
            
            // Now create child types as nested types (skip duplicates by name under the same parent)
            var seenChildNames = new HashSet<string>();
            foreach (var childScope in scope.Children)
            {
                if (!seenChildNames.Add(childScope.Name))
                {
                    // Duplicate child scope name under the same parent (e.g., repeated traversal). Skip.
                    continue;
                }
                CreateAllTypesNested(childScope, GetClrTypeNameForScope(childScope), parentType);
            }
        }

        /// <summary>
        /// Creates nested types recursively.
        /// </summary>
        private TypeDefinitionHandle CreateAllTypesNested(Scope scope, string typeName, TypeDefinitionHandle parentType)
        {
            // First, recursively create all child types (depth-first)
            var currentType = CreateScopeType(scope, typeName);
            
            // Now create child types as nested types of this type (dedupe by name under this parent)
            var seenChildNames = new HashSet<string>();
            foreach (var childScope in scope.Children)
            {
                if (!seenChildNames.Add(childScope.Name))
                {
                    continue;
                }
                CreateAllTypesNested(childScope, GetClrTypeNameForScope(childScope), currentType);
            }

            return currentType;
        }

        /// <summary>
        /// Creates a single type definition for a scope.
        /// All fields must already exist. All types are created as top-level types.
        /// </summary>
        private TypeDefinitionHandle CreateScopeType(Scope scope, string typeName)
        {
            // Scope types are always nested types in metadata. The specific enclosing TypeDef is resolved
            // later (once module + callable-owner + class TypeDefs exist) and emitted via NestedClass rows.
            //
            var typeAttributes = TypeAttributes.NestedPrivate | TypeAttributes.Class | TypeAttributes.BeforeFieldInit;
            var actualNamespace = string.Empty;

            // Initialize TypeBuilder for this type (handles field/method tracking and first-method/field invariants)
            var tb = new TypeBuilder(_metadataBuilder, actualNamespace, typeName);

            // Create fields via TypeBuilder so it can track the first field
            CreateTypeFields(scope, tb);

            // Defer constructor MethodDef emission until after callable methods are emitted.
            // We preassign a stable future MethodDef handle so TypeDef.MethodList points at the ctor,
            // and later emit the MethodDef rows in this exact type-creation order.
            var ctorHandle = MetadataTokens.MethodDefinitionHandle(_nextDeferredCtorRow++);

            // Create the type definition
            var baseType = scope.IsAsync
                ? (scope.IsGenerator
                    ? _bclReferences.TypeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.AsyncGeneratorScope))
                    : _bclReferences.TypeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.AsyncScope)))
                : scope.IsGenerator
                    ? _bclReferences.TypeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.GeneratorScope))
                    : _bclReferences.ObjectType;

            var typeHandle = tb.AddTypeDefinition(
                typeAttributes,
                baseType,
                firstFieldOverride: null,
                firstMethodOverride: ctorHandle);

            var scopeKey = GetRegistryScopeName(scope);

            if (_emitDebuggerDisplay)
            {
                EmitDebuggerDisplayAttribute(typeHandle, scopeKey, typeName);
            }

            // NOTE: We intentionally do NOT emit NestedClass table rows here.
            // Nesting relationships are established later in a single pass (sorted by nested type token)
            // so the NestedClass table satisfies ECMA-335 sorting requirements.

            // Store the type handle and constructor for later reference
            _scopeTypes[scopeKey] = typeHandle;
            // Register the scope type immediately so even scopes without variables can be instantiated later.
            _variableRegistry.EnsureScopeType(scopeKey, typeHandle);

            _deferredCtorPlan.Add((scopeKey, actualNamespace, typeName, scope.IsAsync, scope.IsGenerator, ctorHandle));

            return typeHandle;
        }

        private void EmitDebuggerDisplayAttribute(TypeDefinitionHandle typeHandle, string scopeKey, string typeName)
        {
            if (!_scopeFieldNames.TryGetValue(scopeKey, out var names) || names.Count == 0)
            {
                return;
            }

            // DebuggerDisplay uses a C#-like expression language; only include identifiers that are valid
            // without escaping (common case: x, allBits, value, etc.).
            var userFieldNames = names.Where(n => !n.StartsWith("_", StringComparison.Ordinal)).ToList();
            if (userFieldNames.Count == 0)
            {
                userFieldNames = names;
            }

            var included = new List<string>();
            foreach (var name in userFieldNames)
            {
                if (IsValidDebuggerIdentifier(name))
                {
                    included.Add(name);
                }
                if (included.Count >= 8)
                {
                    break;
                }
            }

            if (included.Count == 0)
            {
                // Still provide a minimal display rather than risking broken expressions.
                included.Add("(scope)");
            }

            var display = new StringBuilder();
            display.Append(typeName);

            if (included.Count == 1 && included[0] == "(scope)")
            {
                // No fields safely renderable.
            }
            else
            {
                display.Append(" ");
                for (int i = 0; i < included.Count; i++)
                {
                    if (i > 0)
                    {
                        display.Append(", ");
                    }

                    var fieldName = included[i];
                    display.Append(fieldName);
                    display.Append("={");
                    display.Append(fieldName);
                    display.Append("}");
                }

                if (userFieldNames.Count > included.Count)
                {
                    display.Append(", …");
                }
            }

            var valueBlob = CreateSingleStringCustomAttributeValue(display.ToString());
            _metadataBuilder.AddCustomAttribute(
                parent: typeHandle,
                constructor: _bclReferences.DebuggerDisplayAttribute_Ctor_Ref,
                value: valueBlob);
        }

        private static bool IsValidDebuggerIdentifier(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            // Conservative check: debugger expression identifiers are roughly C# identifiers.
            // We intentionally do not attempt to support keywords or special chars (e.g. '$').
            if (!(char.IsLetter(name[0]) || name[0] == '_'))
            {
                return false;
            }

            for (int i = 1; i < name.Length; i++)
            {
                var ch = name[i];
                if (!(char.IsLetterOrDigit(ch) || ch == '_'))
                {
                    return false;
                }
            }

            return true;
        }

        private BlobHandle CreateSingleStringCustomAttributeValue(string value)
        {
            // ECMA-335 CustomAttribute blob format:
            // - prolog: 0x0001 (UInt16)
            // - fixed args: SerString
            // - named args count: UInt16 (0)
            var blob = new BlobBuilder();
            blob.WriteUInt16(0x0001);
            WriteSerString(blob, value);
            blob.WriteUInt16(0);
            return _metadataBuilder.GetOrAddBlob(blob);
        }

        private static void WriteSerString(BlobBuilder blob, string value)
        {
            var utf8 = Encoding.UTF8.GetBytes(value);
            WriteCompressedUInt32(blob, (uint)utf8.Length);
            blob.WriteBytes(utf8);
        }

        private static void WriteCompressedUInt32(BlobBuilder blob, uint value)
        {
            // ECMA-335 II.23.2 Blobs and signatures (compressed unsigned integer)
            if (value <= 0x7Fu)
            {
                blob.WriteByte((byte)value);
                return;
            }

            if (value <= 0x3FFFu)
            {
                blob.WriteByte((byte)((value >> 8) | 0x80u));
                blob.WriteByte((byte)(value & 0xFFu));
                return;
            }

            if (value <= 0x1FFFFFFFu)
            {
                blob.WriteByte((byte)((value >> 24) | 0xC0u));
                blob.WriteByte((byte)((value >> 16) & 0xFFu));
                blob.WriteByte((byte)((value >> 8) & 0xFFu));
                blob.WriteByte((byte)(value & 0xFFu));
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), "Value too large for compressed integer encoding.");
        }


        /// <summary>
        /// Creates a constructor method definition for a scope type.
        /// </summary>
        private MethodDefinitionHandle EmitScopeConstructor(TypeBuilder tb, bool isAsync, bool isGenerator)
        {
            // Create constructor method signature
            var ctorSig = new BlobBuilder();
            new BlobEncoder(ctorSig)
                .MethodSignature(SignatureCallingConvention.Default, 0, isInstanceMethod: true)
                .Parameters(0, returnType => returnType.Void(), parameters => { });
            var ctorSigHandle = _metadataBuilder.GetOrAddBlob(ctorSig);

            // Generate IL body for constructor: ldarg.0, call base::.ctor(), nop, ret
            var ilBuilder = new BlobBuilder();
            var encoder = new InstructionEncoder(ilBuilder);
            encoder.OpCode(ILOpCode.Ldarg_0);

            if (isAsync && isGenerator)
            {
                encoder.Call(_bclReferences.AsyncGeneratorScope_Ctor_Ref);
            }
            else if (isAsync)
            {
                encoder.Call(_bclReferences.AsyncScope_Ctor_Ref);
            }
            else if (isGenerator)
            {
                encoder.Call(_bclReferences.GeneratorScope_Ctor_Ref);
            }
            else
            {
                encoder.Call(_bclReferences.Object_Ctor_Ref);
            }

            encoder.OpCode(ILOpCode.Nop);
            encoder.OpCode(ILOpCode.Ret);

            var bodyOffset = _methodBodyStream.AddMethodBody(
                encoder,
                localVariablesSignature: default,
                attributes: MethodBodyAttributes.None);

            return tb.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                ".ctor",
                ctorSigHandle,
                bodyOffset);
        }

        /// <summary>
        /// Determines field attributes based on the JavaScript binding kind.
        /// All fields are instance fields.
        /// </summary>
        private FieldAttributes GetFieldAttributes(BindingKind bindingKind)
        {
            var attributes = FieldAttributes.Public; // Instance fields, not static

            // Add additional attributes based on binding kind
            switch (bindingKind)
            {
                case BindingKind.Const:
                    // Do NOT mark const bindings as InitOnly (readonly) on scope types.
                    // We assign initial values after constructing the scope instance, not inside its .ctor,
                    // and CLR verification forbids stfld to initonly fields outside the declaring .ctor.
                    // Const semantics (no reassignment) are enforced at runtime
                    // via a TypeError on any assignment attempts. Keeping these fields mutable here ensures
                    // verifiable IL while preserving JS semantics.
                    break;
                case BindingKind.Let:
                case BindingKind.Var:
                case BindingKind.Function:
                    // No additional attributes needed
                    break;
            }

            return attributes;
        }

        /// <summary>
        /// Populates the variable registry with all variables found in the scope tree.
        /// This method must be called after all types and fields have been created.
        /// </summary>
        private void PopulateVariableRegistry(Scope scope)
        {
            var registryName = GetRegistryScopeName(scope);
            
            // Guard: some scopes (e.g., block scopes) may legitimately have zero bindings; ensure a type was created.
            if (!_scopeTypes.TryGetValue(registryName, out var scopeTypeHandle))
            {
                // If a scope type was not created (should not normally happen), skip but recurse to children.
                foreach (var child in scope.Children) PopulateVariableRegistry(child);
                return;
            }
            var scopeFields = _scopeFields.GetValueOrDefault(registryName, new List<FieldDefinitionHandle>());
            // Ensure scope type is registered
            _variableRegistry.EnsureScopeType(registryName, scopeTypeHandle);

            // Add each binding as a variable in the registry
            // Check if this is an arrow function scope (arrow functions always need parameter fields)
            bool isArrowFunction = scope.AstNode is Acornima.Ast.ArrowFunctionExpression;
            
            foreach (var binding in scope.Bindings)
            {
                var variableName = binding.Key;
                var bindingInfo = binding.Value;
                
                // Convert BindingKind to VariableType
                var variableType = scope.Parameters.Contains(variableName)
                    ? VariableType.Parameter
                    : bindingInfo.Kind switch
                    {
                        BindingKind.Var => VariableType.Variable,
                        BindingKind.Let => VariableType.Variable,
                        BindingKind.Const => VariableType.Variable,
                        BindingKind.Function => VariableType.Function,
                        _ => VariableType.Variable
                    };

                // Check if this is a parameter or function
                bool isParameter = scope.Parameters.Contains(variableName);
                bool isFunction = bindingInfo.Kind == BindingKind.Function;

                // Determine if a field was actually created for this binding
                // Must match the skip logic in CreateTypeFields above:
                // Fields are created for: parameters (with some exceptions), functions, captured variables
                // Fields are NOT created for: uncaptured non-parameter non-function variables
                // (shadowed variables no longer need fields - each scope gets its own local slot)
                bool fieldCreatedForThisBinding = isParameter || isFunction || bindingInfo.IsCaptured;
                
                // Parameters without fields: simple uncaptured params that aren't destructured or in arrow functions
                if (isParameter && !bindingInfo.IsCaptured)
                {
                    bool isDestructuredParameter = scope.DestructuredParameters.Contains(variableName);
                    if (!isDestructuredParameter && !isArrowFunction)
                    {
                        fieldCreatedForThisBinding = false;
                    }
                }

                if (fieldCreatedForThisBinding)
                {
                    if (!_scopeFieldHandlesByName.TryGetValue(registryName, out var fieldsByName) ||
                        !fieldsByName.TryGetValue(variableName, out var fieldHandle) ||
                        fieldHandle.IsNil)
                    {
                        throw new InvalidOperationException(
                            $"TypeGenerator field mapping missing for '{variableName}' in scope '{registryName}'.");
                    }

                    var declaredFieldClrType = GetDeclaredScopeFieldClrType(scope, bindingInfo);
                    _variableRegistry.AddVariable(
                        registryName,  // Use registry name (qualified for class members, simple for functions)
                        variableName,
                        variableType,
                        fieldHandle,
                        scopeTypeHandle,
                        bindingInfo.Kind,
                        bindingInfo.ClrType,
                        bindingInfo.IsStableType,
                        declaredFieldClrType
                    );
                }
                else
                {
                    // No field created for this binding (uncaptured variable)
                    // Parameters without fields are loaded via ldarg, not local variables
                    // Only mark non-parameter variables as uncaptured
                    if (!scope.Parameters.Contains(variableName))
                    {
                        _variableRegistry.MarkAsUncaptured(registryName, variableName);
                        // Still add to registry so binding kind (const/let/var) is preserved
                        _variableRegistry.AddVariable(
                            registryName,
                            variableName,
                            variableType,
                            default,  // No field handle for uncaptured
                            default,  // No scope type handle
                            bindingInfo.Kind,
                            bindingInfo.ClrType,
                            bindingInfo.IsStableType
                        );
                    }
                }
            }

            // Recursively process child scopes
            foreach (var childScope in scope.Children)
            {
                PopulateVariableRegistry(childScope);
            }
        }
    }
}
