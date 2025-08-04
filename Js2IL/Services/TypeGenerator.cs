using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using Js2IL.Scoping;

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
        private readonly Dictionary<string, TypeDefinitionHandle> _scopeTypes;
        private readonly Dictionary<string, List<FieldDefinitionHandle>> _scopeFields;

        public TypeGenerator(MetadataBuilder metadataBuilder, BaseClassLibraryReferences bclReferences)
        {
            _metadataBuilder = metadataBuilder;
            _bclReferences = bclReferences;
            _scopeTypes = new Dictionary<string, TypeDefinitionHandle>();
            _scopeFields = new Dictionary<string, List<FieldDefinitionHandle>>();
        }

        /// <summary>
        /// Generates .NET types from the scope tree.
        /// Returns the root type definition handle.
        /// </summary>
        public TypeDefinitionHandle GenerateTypes(ScopeTree scopeTree)
        {
            // Phase 1: Create all fields first (depth-first traversal)
            CreateAllFields(scopeTree.Root);
            
            // Phase 2: Create all types (depth-first: children first, then parents)
            var rootType = CreateAllTypes(scopeTree.Root, null, scopeTree.Root.Name);
            
            // Phase 3: Establish nesting relationships
            CreateNestingRelationships(scopeTree.Root);
            
            return rootType;
        }

        /// <summary>
        /// Phase 1: Recursively creates all field definitions depth-first.
        /// This must be done before creating any type definitions.
        /// </summary>
        private void CreateAllFields(ScopeNode scope)
        {
            // First, recursively create fields for all children (depth-first)
            foreach (var childScope in scope.Children)
            {
                CreateAllFields(childScope);
            }

            // Then create fields for this scope
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
        /// All types are created as top-level types initially.
        /// </summary>
        private TypeDefinitionHandle CreateAllTypes(ScopeNode scope, TypeDefinitionHandle? parentType, string typeName)
        {
            // First, recursively create all child types (depth-first)
            foreach (var childScope in scope.Children)
            {
                CreateAllTypes(childScope, null, childScope.Name); // All types are top-level initially
            }

            // Now create this type (always as top-level initially)
            return CreateScopeType(scope, null, typeName);
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
        /// All fields must already exist. Types are created as top-level initially.
        /// </summary>
        private TypeDefinitionHandle CreateScopeType(ScopeNode scope, TypeDefinitionHandle? parentType, string typeName)
        {
            // Create regular classes that can be instantiated for scope instances
            var typeAttributes = TypeAttributes.Public | TypeAttributes.Class;

            // Calculate the correct field handle for this type
            var scopeFields = _scopeFields[scope.Name];
            var firstField = scopeFields.Count > 0 
                ? scopeFields[0] 
                : MetadataTokens.FieldDefinitionHandle(_metadataBuilder.GetRowCount(TableIndex.Field) + 1);

            // For methods, we'll use the next available method handle (no methods for now)
            var nextMethod = MetadataTokens.MethodDefinitionHandle(_metadataBuilder.GetRowCount(TableIndex.MethodDef) + 1);

            // Create the type definition
            var typeHandle = _metadataBuilder.AddTypeDefinition(
                typeAttributes,
                _metadataBuilder.GetOrAddString(""), // namespace (empty for nested types)
                _metadataBuilder.GetOrAddString(typeName),
                _bclReferences.ObjectType, // base type
                firstField, // first field for this type
                nextMethod // first method for this type
            );

            // Store the type handle for later reference
            _scopeTypes[scope.Name] = typeHandle;

            return typeHandle;
        }

        /// <summary>
        /// Determines field attributes based on the JavaScript binding kind.
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
        /// Gets all generated scope types.
        /// </summary>
        public IReadOnlyDictionary<string, TypeDefinitionHandle> GetAllScopeTypes()
        {
            return _scopeTypes;
        }
    }
}
