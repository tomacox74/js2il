using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using Js2IL.SymbolTables;
using Js2IL.Services.VariableBindings;
using System.Linq;
using Js2IL.Utilities.Ecma335;

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
        private readonly Dictionary<string, MethodDefinitionHandle> _scopeConstructors;
        private readonly VariableRegistry _variableRegistry = new();

        public TypeGenerator(MetadataBuilder metadataBuilder, BaseClassLibraryReferences bclReferences, MethodBodyStreamEncoder methodBodyStream)
        {
            _metadataBuilder = metadataBuilder;
            _bclReferences = bclReferences;
            _methodBodyStream = methodBodyStream;
            _scopeTypes = new Dictionary<string, TypeDefinitionHandle>();
            _scopeFields = new Dictionary<string, List<FieldDefinitionHandle>>();
            _scopeConstructors = new Dictionary<string, MethodDefinitionHandle>();
        }

        /// <summary>
        /// Generates .NET types from the symbol table.
        /// Returns the root type definition handle.
        /// </summary>
        public TypeDefinitionHandle GenerateTypes(SymbolTable symbolTable)
        {            
            // Phase 1: Create all scope types (depth-first) for every scope discovered by the SymbolTable.
            // SymbolTable already contains scopes for function declarations, function expressions, arrow functions,
            // class methods, block scopes, etc. We rely on that being exhaustive and treat all of them uniformly.
            var rootType = CreateAllTypes(symbolTable.Root, symbolTable.Root.Name);

            // Phase 2: Populate the variable registry (fields + metadata for every binding).
            PopulateVariableRegistry(symbolTable.Root);

            return rootType;
        }

        /// <summary>
        /// Gets the variable registry populated during type generation.
        /// </summary>
        public VariableRegistry GetVariableRegistry()
        {
            return _variableRegistry;
        }


    private void CreateTypeFields(Scope scope, TypeBuilder typeBuilder)
        {
            
            // Create fields for this scope
            _scopeFields[scope.Name] = new List<FieldDefinitionHandle>();
            var scopeFields = _scopeFields[scope.Name];

            // Determine if this function scope contains nested functions
            bool hasNestedFunctions = scope.Children.Any(c => c.Kind == ScopeKind.Function);
            
            // Check if this is an arrow function scope (arrow functions always need parameter fields for closure semantics)
            bool isArrowFunction = scope.AstNode is Acornima.Ast.ArrowFunctionExpression;
            
            // Check if this scope has parameters with default values
            Acornima.Ast.NodeList<Acornima.Ast.Node>? paramList = scope.AstNode switch
            {
                Acornima.Ast.FunctionDeclaration fd => fd.Params,
                Acornima.Ast.FunctionExpression fe => fe.Params,
                Acornima.Ast.ArrowFunctionExpression af => af.Params,
                _ => null
            };
            bool hasDefaultParameters = paramList.HasValue && paramList.Value.Any(p => p is Acornima.Ast.AssignmentPattern);

            foreach (var binding in scope.Bindings.Values)
            {
                // Parameters are treated as fields on the scope when:
                // 1. Nested functions exist (closure access), OR
                // 2. This is an arrow function (needs fields for proper this binding and closure)
                // 3. The parameter comes from destructuring (needs storage to extract from object)
                // Default parameters use starg to modify IL arguments directly, not fields (except for arrow functions)
                bool isParameter = scope.Parameters.Contains(binding.Name);
                bool isFunction = binding.Kind == BindingKind.Function;
                
                // Note: We CANNOT skip field/local creation for ALL parameters when no nested functions exist.
                // Destructured parameters (from ObjectPattern) need storage because they're extracted from
                // an incoming object parameter, not passed directly as IL arguments.
                // For now, conservatively create fields for all parameters to ensure destructuring works.
                // A future optimization could distinguish between direct parameters vs destructured properties.

                // Skip field creation for uncaptured local variables (not parameters, not function declarations)
                // Function declarations (BindingKind.Function) always need fields to store delegate references
                // Parameters need fields if arrow functions or nested functions exist
                // Uncaptured const/let/var variables can use local variables instead of fields
                // EXCEPTION: If a variable shadows a parent scope variable with the same name, the INNER one
                // must use a field to avoid local slot collision with the outer variable
                bool shadowsParentVariable = DoesShadowParentVariable(scope, binding.Name);
                if (!binding.IsCaptured && !isParameter && !isFunction && !shadowsParentVariable)
                {
                    // Skip creating a field for non-captured non-parameter non-function bindings in nested scopes
                    continue;
                }

                // Create field signature (all variables are object type for now)
                var fieldSignature = new BlobBuilder();
                new BlobEncoder(fieldSignature)
                    .Field()
                    .Type()
                    .Object();

                var fieldSignatureHandle = _metadataBuilder.GetOrAddBlob(fieldSignature);

                // Determine field attributes based on binding kind
                var fieldAttributes = GetFieldAttributes(binding.Kind);

                // Create the field definition
                var fieldHandle = typeBuilder.AddFieldDefinition(
                    fieldAttributes,
                    binding.Name,
                    fieldSignatureHandle
                );

                scopeFields.Add(fieldHandle);
            }
        }

        /// <summary>
        /// Phase 2: Recursively creates type definitions depth-first (children first).
        /// All fields must already be created before this is called.
        /// Root types go in the "Scopes" namespace, nested types are properly nested.
        /// </summary>
        private TypeDefinitionHandle CreateAllTypes(Scope scope, string typeName)
        {
            // First, recursively create all child types (depth-first)
            // We'll create the parent type first, then update children to be nested
            var parentType = CreateScopeType(scope, null, typeName, "Scopes");
            
            // Now create child types as nested types (skip duplicates by name under the same parent)
            var seenChildNames = new HashSet<string>();
            foreach (var childScope in scope.Children)
            {
                if (!seenChildNames.Add(childScope.Name))
                {
                    // Duplicate child scope name under the same parent (e.g., repeated traversal). Skip.
                    continue;
                }
                CreateAllTypesNested(childScope, childScope.Name, parentType);
            }

            return parentType;
        }

        /// <summary>
        /// Creates nested types recursively.
        /// </summary>
        private TypeDefinitionHandle CreateAllTypesNested(Scope scope, string typeName, TypeDefinitionHandle parentType)
        {
            // First, recursively create all child types (depth-first)
            var currentType = CreateScopeType(scope, parentType, typeName, "");
            
            // Now create child types as nested types of this type (dedupe by name under this parent)
            var seenChildNames = new HashSet<string>();
            foreach (var childScope in scope.Children)
            {
                if (!seenChildNames.Add(childScope.Name))
                {
                    continue;
                }
                CreateAllTypesNested(childScope, childScope.Name, currentType);
            }

            return currentType;
        }

        /// <summary>
        /// Recursively collects all nesting relationships from the scope tree.
        /// </summary>
        private void CollectNestingRelationships(Scope scope, List<(TypeDefinitionHandle, TypeDefinitionHandle)> relationships)
        {
            // For each child scope, collect the nesting relationship
            foreach (var childScope in scope.Children)
            {
                var parentTypeHandle = _scopeTypes[scope.Name];
                var nestedTypeHandle = _scopeTypes[childScope.Name];
                
                relationships.Add((nestedTypeHandle, parentTypeHandle));
                
                // Recursively collect child relationships
                CollectNestingRelationships(childScope, relationships);
            }
        }

        /// <summary>
        /// Phase 3: Establishes nesting relationships after all types are created.
        /// </summary>
        private void CreateNestingRelationships(Scope scope)
        {
            // For each child scope, establish the nesting relationship
            foreach (var childScope in scope.Children)
            {
                var parentTypeHandle = _scopeTypes[scope.Name];
                var nestedTypeHandle = _scopeTypes[childScope.Name];
                
                // Establish the nesting relationship
                _metadataBuilder.AddNestedType(nestedTypeHandle, parentTypeHandle);
                
                // Recursively process child relationships
                CreateNestingRelationships(childScope);
            }
        }

        /// <summary>
        /// Creates a single type definition for a scope.
        /// All fields must already exist. All types are created as top-level types.
        /// </summary>
        private TypeDefinitionHandle CreateScopeType(Scope scope, TypeDefinitionHandle? parentType, string typeName, string namespaceString)
        {
            // Set appropriate visibility: Public for root types, NestedPublic for nested types
            var typeAttributes = parentType.HasValue 
                ? TypeAttributes.NestedPublic | TypeAttributes.Class | TypeAttributes.BeforeFieldInit
                : TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit;

            // For nested types, use empty namespace; for root types, use the provided namespace
            var actualNamespace = parentType.HasValue ? "" : namespaceString;

            // Initialize TypeBuilder for this type (handles field/method tracking and first-method/field invariants)
            var tb = new TypeBuilder(_metadataBuilder, actualNamespace, typeName);

            // Create fields via TypeBuilder so it can track the first field
            CreateTypeFields(scope, tb);

            // Create the constructor for this type via TypeBuilder so it can track the first method
            var ctorHandle = CreateScopeConstructor(tb);

            // Create the type definition
            var typeHandle = tb.AddTypeDefinition(
                typeAttributes,
                _bclReferences.ObjectType // base type
            );

            // If this is a nested type, establish the nesting relationship
            if (parentType.HasValue)
            {
                _metadataBuilder.AddNestedType(typeHandle, parentType.Value);
            }

            // Store the type handle and constructor for later reference
            _scopeTypes[scope.Name] = typeHandle;
            _scopeConstructors[scope.Name] = ctorHandle;
            // Register the scope type immediately so even scopes without variables can be instantiated later.
            Console.WriteLine($"[TypeGenerator.CreateTypeDefinition] Registering scope: {scope.Name}, TypeHandle IsNil: {typeHandle.IsNil}");
            _variableRegistry.EnsureScopeType(scope.Name, typeHandle);

            return typeHandle;
        }

        /// <summary>
        /// Creates a constructor method definition for a scope type.
        /// </summary>
    private MethodDefinitionHandle CreateScopeConstructor(TypeBuilder tb)
        {
            // Create constructor method signature
            var ctorSig = new BlobBuilder();
            new BlobEncoder(ctorSig)
                .MethodSignature(SignatureCallingConvention.Default, 0, isInstanceMethod: true)
                .Parameters(0, returnType => returnType.Void(), parameters => { });
            var ctorSigHandle = _metadataBuilder.GetOrAddBlob(ctorSig);

            // Generate IL body for constructor: ldarg.0, call Object::.ctor(), nop, ret
            var ilBuilder = new BlobBuilder();
            var encoder = new InstructionEncoder(ilBuilder);
            
            // ldarg.0 - load 'this' onto the stack
            encoder.OpCode(ILOpCode.Ldarg_0);
            
            // call Object::.ctor() - call base constructor
            encoder.Call(_bclReferences.Object_Ctor_Ref);
            
            // nop - no operation (matches C# compiler output)
            encoder.OpCode(ILOpCode.Nop);
            
            // ret - return from constructor
            encoder.OpCode(ILOpCode.Ret);

            // Add the method body to the stream - pass the encoder, not the builder
            var bodyOffset = _methodBodyStream.AddMethodBody(
                encoder,
                localVariablesSignature: default,
                attributes: MethodBodyAttributes.None);

            // Create the constructor method definition with IL body
            var ctorHandle = tb.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                ".ctor",
                ctorSigHandle,
                bodyOffset // IL body offset in the stream
            );

            return ctorHandle;
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
                    // Const semantics (no reassignment) are enforced at runtime by ILExpressionGenerator
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
            // Use qualified names only for class methods/constructors to avoid collisions (e.g., Point/constructor vs Person/constructor)
            // Use simple names for functions/arrow functions as ILExpressionGenerator looks them up by simple name
            bool isClassMember = scope.Parent?.Kind == ScopeKind.Class;
            var registryName = isClassMember ? scope.GetQualifiedName() : scope.Name;
            
            // Guard: some scopes (e.g., block scopes) may legitimately have zero bindings; ensure a type was created.
            if (!_scopeTypes.TryGetValue(scope.Name, out var scopeTypeHandle))
            {
                // If a scope type was not created (should not normally happen), skip but recurse to children.
                foreach (var child in scope.Children) PopulateVariableRegistry(child);
                return;
            }
            var scopeFields = _scopeFields.GetValueOrDefault(scope.Name, new List<FieldDefinitionHandle>());
            // Ensure scope type is registered
            _variableRegistry.EnsureScopeType(registryName, scopeTypeHandle);

            // Add each binding as a variable in the registry
            int fieldIndex = 0;
            // Determine if this scope contains nested functions (controls whether parameters get fields)
            bool hasNestedFunctions = scope.Children.Any(c => c.Kind == ScopeKind.Function);
            
            // Check if this is an arrow function scope (arrow functions always need parameter fields)
            bool isArrowFunction = scope.AstNode is Acornima.Ast.ArrowFunctionExpression;
            
            // Check if this scope has parameters with default values (must match CreateTypeFields logic)
            Acornima.Ast.NodeList<Acornima.Ast.Node>? paramList = scope.AstNode switch
            {
                Acornima.Ast.FunctionDeclaration fd => fd.Params,
                Acornima.Ast.FunctionExpression fe => fe.Params,
                Acornima.Ast.ArrowFunctionExpression af => af.Params,
                _ => null
            };
            bool hasDefaultParameters = paramList.HasValue && paramList.Value.Any(p => p is Acornima.Ast.AssignmentPattern);
            
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
                // Fields are created for: parameters, functions, captured variables, or shadowing variables
                // Fields are NOT created for: uncaptured non-parameter non-function non-shadowing variables
                bool shadowsParentVariable = DoesShadowParentVariable(scope, variableName);
                bool fieldCreatedForThisBinding = isParameter || isFunction || bindingInfo.IsCaptured || shadowsParentVariable;

                if (fieldCreatedForThisBinding)
                {
                    // Map this binding to the next created field (order matches CreateTypeFields enumeration)
                    if (fieldIndex < scopeFields.Count)
                    {
                        var fieldHandle = scopeFields[fieldIndex];
                        _variableRegistry.AddVariable(
                            registryName,  // Use registry name (qualified for class members, simple for functions)
                            variableName,
                            variableType,
                            fieldHandle,
                            scopeTypeHandle,
                            bindingInfo.Kind
                        );
                        // If SymbolTable discovered a CLR runtime type (e.g., const x = require('path')) assign it now
                        if (bindingInfo.RuntimeIntrinsicType != null)
                        {
                            _variableRegistry.SetRuntimeIntrinsicType(registryName, variableName, bindingInfo.RuntimeIntrinsicType);
                        }
                    }
                    // Only advance when a field exists for this binding
                    fieldIndex++;
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
                            bindingInfo.Kind
                        );
                        if (bindingInfo.RuntimeIntrinsicType != null)
                        {
                            _variableRegistry.SetRuntimeIntrinsicType(registryName, variableName, bindingInfo.RuntimeIntrinsicType);
                        }
                    }
                }
            }

            // Recursively process child scopes
            foreach (var childScope in scope.Children)
            {
                PopulateVariableRegistry(childScope);
            }
        }

        private bool DoesShadowParentVariable(Scope scope, string variableName)
        {
            // Check if this variable exists in any parent scope
            var currentScope = scope.Parent;
            while (currentScope != null)
            {
                if (currentScope.Bindings.ContainsKey(variableName))
                {
                    return true;
                }
                currentScope = currentScope.Parent;
            }
            return false;
        }
    }
}
