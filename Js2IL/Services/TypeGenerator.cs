using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using Js2IL.Scoping;
using Js2IL.Services.VariableBindings;
using System.Linq;

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
        /// Generates .NET types from the scope tree.
        /// Returns the root type definition handle.
        /// </summary>
        public TypeDefinitionHandle GenerateTypes(ScopeTree scopeTree)
        {            
            // Create all types with proper nesting (depth-first: children first, then parents)
            var rootType = CreateAllTypes(scopeTree.Root, scopeTree.Root.Name);
            
            // Nesting relationships are established during type creation
            
            // Populate variable registry with all discovered variables
            PopulateVariableRegistry(scopeTree.Root);
            
            return rootType;
        }

        /// <summary>
        /// Gets the variable registry populated during type generation.
        /// </summary>
        public VariableRegistry GetVariableRegistry()
        {
            return _variableRegistry;
        }


        private void CreateTypeFields(ScopeNode scope)
        {
            // Create fields for this scope
            _scopeFields[scope.Name] = new List<FieldDefinitionHandle>();
            var scopeFields = _scopeFields[scope.Name];

            foreach (var binding in scope.Bindings.Values)
            {
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
                var fieldHandle = _metadataBuilder.AddFieldDefinition(
                    fieldAttributes,
                    _metadataBuilder.GetOrAddString(binding.Name),
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
        private TypeDefinitionHandle CreateAllTypes(ScopeNode scope, string typeName)
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
        private TypeDefinitionHandle CreateAllTypesNested(ScopeNode scope, string typeName, TypeDefinitionHandle parentType)
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
        private void CreateNestingRelationshipsSorted(ScopeNode rootScope)
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
        private void CollectNestingRelationships(ScopeNode scope, List<(TypeDefinitionHandle, TypeDefinitionHandle)> relationships)
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
        private void CreateNestingRelationships(ScopeNode scope)
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
        private TypeDefinitionHandle CreateScopeType(ScopeNode scope, TypeDefinitionHandle? parentType, string typeName, string namespaceString)
        {
            CreateTypeFields(scope);

            // Set appropriate visibility: Public for root types, NestedPublic for nested types
            var typeAttributes = parentType.HasValue 
                ? TypeAttributes.NestedPublic | TypeAttributes.Class | TypeAttributes.BeforeFieldInit
                : TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit;

            // Calculate the correct field handle for this type
            var scopeFields = _scopeFields[scope.Name];
            var firstField = scopeFields.Count > 0 
                ? scopeFields[0] 
                : MetadataTokens.FieldDefinitionHandle(_metadataBuilder.GetRowCount(TableIndex.Field) + 1);

            // Create the constructor for this type
            var ctorHandle = CreateScopeConstructor();

            // For nested types, use empty namespace; for root types, use the provided namespace
            var actualNamespace = parentType.HasValue ? "" : namespaceString;

            // Create the type definition
            var typeHandle = _metadataBuilder.AddTypeDefinition(
                typeAttributes,
                _metadataBuilder.GetOrAddString(actualNamespace),
                _metadataBuilder.GetOrAddString(typeName),
                _bclReferences.ObjectType, // base type
                firstField, // first field for this type
                ctorHandle // first method for this type (the constructor)
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
        private MethodDefinitionHandle CreateScopeConstructor()
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
            var ctorHandle = _metadataBuilder.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                MethodImplAttributes.IL,
                _metadataBuilder.GetOrAddString(".ctor"),
                ctorSigHandle,
                bodyOffset, // IL body offset in the stream
                parameterList: MetadataTokens.ParameterHandle(_metadataBuilder.GetRowCount(TableIndex.Param) + 1)
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
        /// Gets the field handles for a scope by name.
        /// </summary>
        public IReadOnlyList<FieldDefinitionHandle> GetScopeFields(string scopeName)
        {
            return _scopeFields.TryGetValue(scopeName, out var fields) 
                ? fields 
                : new List<FieldDefinitionHandle>();
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
        private void PopulateVariableRegistry(ScopeNode scope)
        {
            var scopeTypeHandle = _scopeTypes[scope.Name];
            var scopeFields = _scopeFields[scope.Name];

            // Add each binding as a variable in the registry
            int fieldIndex = 0;
            foreach (var binding in scope.Bindings)
            {
                var variableName = binding.Key;
                var bindingInfo = binding.Value;
                
                // Convert BindingKind to VariableType
                var variableType = bindingInfo.Kind switch
                {
                    BindingKind.Var => VariableType.Variable,
                    BindingKind.Let => VariableType.Variable,
                    BindingKind.Const => VariableType.Variable,
                    _ => VariableType.Variable
                };

                // Get the field handle for this variable (fields are created in order)
                if (fieldIndex < scopeFields.Count)
                {
                    var fieldHandle = scopeFields[fieldIndex];

                    _variableRegistry.AddVariable(
                        scope.Name, 
                        variableName, 
                        variableType, 
                        fieldHandle, 
                        scopeTypeHandle
                    );
                }

                fieldIndex++;
            }

            // Recursively process child scopes
            foreach (var childScope in scope.Children)
            {
                PopulateVariableRegistry(childScope);
            }
        }
    }
}
