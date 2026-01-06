using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;

namespace Js2IL.Utilities.Ecma335
{
    /// <summary>
    /// A thin helper over MetadataBuilder that enforces ECMA-335 ordering rules for TypeDefinition
    /// and tracks the first FieldDefinition and MethodDefinition added.
    /// </summary>
    internal sealed class TypeBuilder
    {
        /// <summary>
        /// The namespace used for generated function types (arrows, function expressions, etc.).
        /// </summary>
        public const string FunctionsNamespace = "Functions";

        private readonly MetadataBuilder _metadataBuilder;
        private readonly string _namespaceName;
        private readonly string _typeName;

        private bool _typeDefinitionAdded;
        private FieldDefinitionHandle _firstFieldDefinition = default;
        private MethodDefinitionHandle _firstMethodDefinition = default;
        private int _fieldCount;
        private int _methodCount;

        public TypeBuilder(MetadataBuilder metadataBuilder, string @namespace, string name)
        {
            _metadataBuilder = metadataBuilder ?? throw new ArgumentNullException(nameof(metadataBuilder));
            _namespaceName = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
            _typeName = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Wrapper for MetadataBuilder.AddFieldDefinition. Tracks the first field handle and count.
        /// Throws if AddTypeDefinition was already called.
        /// </summary>
        public FieldDefinitionHandle AddFieldDefinition(FieldAttributes attributes, string name, BlobHandle signature)
        {
            if (_typeDefinitionAdded)
            {
                throw new InvalidOperationException("AddFieldDefinition cannot be called after AddTypeDefinition.");
            }

            if (name is null) throw new ArgumentNullException(nameof(name));
            var nameHandle = _metadataBuilder.GetOrAddString(name);
            var handle = _metadataBuilder.AddFieldDefinition(attributes, nameHandle, signature);

            if (_fieldCount == 0)
            {
                _firstFieldDefinition = handle;
            }
            _fieldCount++;
            return handle;
        }

        /// <summary>
        /// Wrapper for MetadataBuilder.AddMethodDefinition. Tracks the first method handle and count.
        /// Allowed before or after AddTypeDefinition.
        /// </summary>
        public MethodDefinitionHandle AddMethodDefinition(
            MethodAttributes attributes,
            string name,
            BlobHandle signature,
            int bodyOffset,
            ParameterHandle parameterList)
        {
            if (name is null) throw new ArgumentNullException(nameof(name));
            var nameHandle = _metadataBuilder.GetOrAddString(name);
            // If caller passed a nil parameter list, compute the next Param handle as required
            var effectiveParameterList = parameterList.IsNil
                ? MetadataTokens.ParameterHandle(_metadataBuilder.GetRowCount(TableIndex.Param) + 1)
                : parameterList;
            var handle = _metadataBuilder.AddMethodDefinition(attributes, MethodImplAttributes.IL, nameHandle, signature, bodyOffset, effectiveParameterList);
            if (_methodCount == 0)
            {
                _firstMethodDefinition = handle;
            }
            _methodCount++;
            return handle;
        }

        /// <summary>
        /// Overload that omits parameterList; defaults to the next Param row handle.
        /// </summary>
        public MethodDefinitionHandle AddMethodDefinition(
            MethodAttributes attributes,
            string name,
            BlobHandle signature,
            int bodyOffset)
        {
            return AddMethodDefinition(attributes, name, signature, bodyOffset, default);
        }

        /// <summary>
        /// Wrapper for MetadataBuilder.AddTypeDefinition. Can only be called once.
        /// Supplies the first field and first method handles automatically to satisfy ECMA-335 ordering.
        /// Uses constructor-provided namespace and name strings, resolved at call-time via GetOrAddString.
        /// </summary>
        public TypeDefinitionHandle AddTypeDefinition(
            TypeAttributes attributes,
            EntityHandle baseType)
        {
            if (_typeDefinitionAdded)
            {
                throw new InvalidOperationException("AddTypeDefinition can only be called once per TypeBuilder instance.");
            }

            // Resolve namespace and name handles at the time of type definition creation
            var nsHandle = _metadataBuilder.GetOrAddString(_namespaceName);
            var nameHandle = _metadataBuilder.GetOrAddString(_typeName);

            // 5th parameter: first field definition handle or next row if none were added.
            var fieldList = !_firstFieldDefinition.IsNil
                ? _firstFieldDefinition
                : MetadataTokens.FieldDefinitionHandle(_metadataBuilder.GetRowCount(TableIndex.Field) + 1);

            // 6th parameter: first method definition handle or next row if none were added at the time of the call.
            var methodList = !_firstMethodDefinition.IsNil
                ? _firstMethodDefinition
                : MetadataTokens.MethodDefinitionHandle(_metadataBuilder.GetRowCount(TableIndex.MethodDef) + 1);

            var typeHandle = _metadataBuilder.AddTypeDefinition(attributes, nsHandle, nameHandle, baseType, fieldList, methodList);
            _typeDefinitionAdded = true;
            return typeHandle;
        }

        /// <summary>
        /// Gets the number of fields that have been added via this builder.
        /// </summary>
        public int GetFieldCount() => _fieldCount;

        /// <summary>
        /// Gets the number of methods that have been added via this builder.
        /// </summary>
        public int GetMethodCount() => _methodCount;
    }
}
