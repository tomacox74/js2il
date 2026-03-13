using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.Utilities.Ecma335
{
    /// <summary>
    /// Registry for type references in a MetadataBuilder.
    /// Caches TypeReferenceHandle instances to avoid duplicate type references.
    /// </summary>
    public sealed class TypeReferenceRegistry
    {
        private readonly MetadataBuilder _metadataBuilder;
        private readonly AssemblyReferenceRegistry _assemblyRefRegistry;
        private readonly Dictionary<Type, TypeReferenceHandle> _cache = new();

        // Mapping from type full names to their reference assembly names
        // This is needed because at runtime types may be in implementation assemblies
        // but we want to reference them from their reference assemblies in generated metadata
        private static readonly Dictionary<string, string> _referenceAssemblyMap = new()
        {
            // System.Collections types
            ["System.Collections.Generic.List`1"] = "System.Collections",
            ["System.Collections.Generic.IList`1"] = "System.Collections",
            ["System.Collections.Generic.ICollection`1"] = "System.Collections",
            ["System.Collections.Generic.IEnumerable`1"] = "System.Collections",
        };

        public TypeReferenceRegistry(MetadataBuilder metadataBuilder)
        {
            _metadataBuilder = metadataBuilder ?? throw new ArgumentNullException(nameof(metadataBuilder));
            _assemblyRefRegistry = new(metadataBuilder);
        }

        /// <summary>
        /// Gets or adds a type reference for the specified .NET type.
        /// </summary>
        /// <param name="type">The .NET type to reference.</param>
        /// <returns>The TypeReferenceHandle for the type.</returns>
        public TypeReferenceHandle GetOrAdd(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (_cache.TryGetValue(type, out var handle))
                return handle;

            // Get the assembly reference for this type
            // Check if we have a known reference assembly mapping for this type
            var typeName = type.IsGenericTypeDefinition ? type.GetGenericTypeDefinition().FullName : type.FullName;
            string assemblyName;
            if (typeName != null && _referenceAssemblyMap.TryGetValue(typeName, out var refAsmName))
            {
                assemblyName = refAsmName;
            }
            else
            {
                assemblyName = type.Assembly.GetName().Name ?? throw new InvalidOperationException($"Type {type.FullName} has no assembly name");
            }
            
            var assemblyRef = _assemblyRefRegistry.GetOrAdd(assemblyName);

            // Handle nested types
            TypeReferenceHandle typeRef;
            if (type.DeclaringType != null)
            {
                // For nested types, the resolution scope is the parent type reference
                var parentRef = GetOrAdd(type.DeclaringType);
                typeRef = _metadataBuilder.AddTypeReference(
                    parentRef,
                    _metadataBuilder.GetOrAddString(string.Empty), // Nested types don't have a namespace
                    _metadataBuilder.GetOrAddString(type.Name));
            }
            else
            {
                // For top-level types, the resolution scope is the assembly reference
                typeRef = _metadataBuilder.AddTypeReference(
                    assemblyRef,
                    _metadataBuilder.GetOrAddString(type.Namespace ?? string.Empty),
                    _metadataBuilder.GetOrAddString(type.Name));
            }

            _cache[type] = typeRef;
            return typeRef;
        }

        /// <summary>
        /// Clears the cache. Useful for testing or when starting a new compilation unit.
        /// </summary>
        public void Clear() => _cache.Clear();

        /// <summary>
        /// Gets the number of cached type references.
        /// </summary>
        public int Count => _cache.Count;
    }
}
