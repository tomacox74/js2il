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
            // Create all types with proper nesting (depth-first: children first, then parents)
            var rootType = CreateAllTypes(symbolTable.Root, symbolTable.Root.Name);
            
            // Nesting relationships are established during type creation
            
            // Populate variable registry with all discovered variables
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

            foreach (var binding in scope.Bindings.Values)
            {
                // Parameters are treated as fields on the scope only when nested functions exist (closure access)
                if (scope.Parameters.Contains(binding.Name) && !hasNestedFunctions)
                {
                    // Skip creating a field for parameters when no nested functions need closure capture
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
            
            // Now create child types as nested types
            foreach (var childScope in scope.Children)
            {
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
            
            // Now create child types as nested types of this type
            foreach (var childScope in scope.Children)
            {
                CreateAllTypesNested(childScope, childScope.Name, currentType);
            }

            return currentType;
        }

        /// <summary>
        /// Phase 3: Establishes nesting relationships in sorted order by nested type handle.
        /// The .NET metadata specification requires NestedClass table to be sorted.
        /// </summary>
        private void CreateNestingRelationshipsSorted(Scope rootScope)
        {
            // Collect all nesting relationships first
            var nestingRelationships = new List<(TypeDefinitionHandle nestedType, TypeDefinitionHandle enclosingType)>();
            CollectNestingRelationships(rootScope, nestingRelationships);
            
            // Sort by nested type handle (required by .NET metadata specification)
            nestingRelationships.Sort((a, b) => MetadataTokens.GetRowNumber(a.nestedType).CompareTo(MetadataTokens.GetRowNumber(b.nestedType)));
            
            // Add the nesting relationships in sorted order
            foreach (var (nestedType, enclosingType) in nestingRelationships)
            {
                _metadataBuilder.AddNestedType(nestedType, enclosingType);
            }
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
                    attributes |= FieldAttributes.InitOnly; // readonly field for const
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
        /// Gets the type definition handle for a scope by name.
        /// </summary>
        public TypeDefinitionHandle GetScopeType(string scopeName)
        {
            return _scopeTypes.TryGetValue(scopeName, out var typeHandle)
                ? typeHandle
                : default;
        }

        /// <summary>
        /// Gets the constructor method handle for a scope by name.
        /// </summary>
        public MethodDefinitionHandle GetScopeConstructor(string scopeName)
        {
            return _scopeConstructors.TryGetValue(scopeName, out var ctorHandle) 
                ? ctorHandle 
                : default;
        }

        /// <summary>
        /// Gets all generated scope types.
        /// </summary>
        public IReadOnlyDictionary<string, TypeDefinitionHandle> GetAllScopeTypes()
        {
            return _scopeTypes;
        }

        /// <summary>
        /// Populates the variable registry with all variables found in the scope tree.
        /// This method must be called after all types and fields have been created.
        /// </summary>
        private void PopulateVariableRegistry(Scope scope)
        {
            var scopeTypeHandle = _scopeTypes[scope.Name];
            var scopeFields = _scopeFields[scope.Name];

            // Add each binding as a variable in the registry
            int fieldIndex = 0;
            // Determine if this scope contains nested functions (controls whether parameters get fields)
            bool hasNestedFunctions = scope.Children.Any(c => c.Kind == ScopeKind.Function);
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

                // Determine if a field was actually created for this binding
                bool fieldCreatedForThisBinding = !(scope.Parameters.Contains(variableName) && !hasNestedFunctions);

                if (fieldCreatedForThisBinding)
                {
                    // Map this binding to the next created field (order matches CreateTypeFields enumeration)
                    if (fieldIndex < scopeFields.Count)
                    {
                        var fieldHandle = scopeFields[fieldIndex];
                        _variableRegistry.AddVariable(
                            scope.Name,
                            variableName,
                            variableType,
                            fieldHandle,
                            scopeTypeHandle,
                            bindingInfo.Kind
                        );
                        // If SymbolTable discovered a CLR runtime type (e.g., const x = require('path')) assign it now
                        if (bindingInfo.RuntimeIntrinsicType != null)
                        {
                            _variableRegistry.SetRuntimeIntrinsicType(scope.Name, variableName, bindingInfo.RuntimeIntrinsicType);
                        }
                    }
                    // Only advance when a field exists for this binding
                    fieldIndex++;
                }
                else
                {
                    // No field created for this parameter; do not register a field-backed variable
                    // (Variables will treat it as a direct parameter at emit time.)
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
