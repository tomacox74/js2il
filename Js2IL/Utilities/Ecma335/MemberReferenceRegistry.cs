using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.Utilities.Ecma335
{
    /// <summary>
    /// Centralized registry for MemberReferenceHandles to avoid duplicate metadata entries.
    /// Automatically builds method signatures via reflection for intrinsic runtime methods.
    /// </summary>
    public sealed class MemberReferenceRegistry
    {
        private readonly MetadataBuilder _metadataBuilder;
        private readonly TypeReferenceRegistry _typeRefRegistry;
        private readonly Dictionary<string, MemberReferenceHandle> _cache = new(StringComparer.Ordinal);
        private readonly Dictionary<Type, TypeSpecificationHandle> _typeSpecCache = new();
        private readonly Dictionary<string, MethodSpecificationHandle> _methodSpecCache = new();

        public MemberReferenceRegistry(
            MetadataBuilder metadataBuilder,
            TypeReferenceRegistry typeRefRegistry)
        {
            _metadataBuilder = metadataBuilder;
            _typeRefRegistry = typeRefRegistry;
        }

        /// <summary>
        /// Gets or creates a method member reference handle.
        /// Uses reflection to discover the method signature automatically.
        /// Throws if the method is not found or has multiple overloads without parameterTypes specified.
        /// </summary>
        /// <param name="declaringType">The type that declares the method.</param>
        /// <param name="methodName">The method name (case-sensitive).</param>
        /// <param name="parameterTypes">Optional parameter types to resolve overloads. If null, requires exactly one method with the given name.</param>
        /// <returns>A cached or newly created MemberReferenceHandle.</returns>
        public MemberReferenceHandle GetOrAddMethod(Type declaringType, string methodName, Type[]? parameterTypes = null)
        {
            var fullTypeName = declaringType.FullName ?? $"{declaringType.Namespace}.{declaringType.Name}";
            var paramKey = parameterTypes != null ? string.Join(",", parameterTypes.Select(t => t.FullName)) : "";
            var key = $"{fullTypeName}::{methodName}({paramKey})";

            if (_cache.TryGetValue(key, out var existing))
                return existing;

            // For constructed generic types, we need to work with the open generic definition
            // to get method signatures with generic type parameters (!0, !1, !2)
            Type typeForReflection = declaringType;
            if (declaringType.IsGenericType && !declaringType.IsGenericTypeDefinition)
            {
                typeForReflection = declaringType.GetGenericTypeDefinition();
            }

            // Use reflection to find the method on the open generic type
            var methods = typeForReflection.GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.Static)
                .Where(m => m.Name == methodName)
                .ToList();

            if (methods.Count == 0)
                throw new ArgumentException($"Method '{methodName}' not found on type '{fullTypeName}'");

            MethodInfo method;
            if (parameterTypes != null)
            {
                // Find method matching parameter types
                method = methods.FirstOrDefault(m =>
                {
                    var pars = m.GetParameters();
                    if (pars.Length != parameterTypes.Length) return false;
                    for (int i = 0; i < pars.Length; i++)
                    {
                        if (pars[i].ParameterType != parameterTypes[i]) return false;
                    }
                    return true;
                }) ?? throw new ArgumentException($"Method '{methodName}' with specified parameter types not found on type '{fullTypeName}'");
            }
            else
            {
                // Require exactly one method when no parameter types specified
                if (methods.Count > 1)
                    throw new ArgumentException($"Multiple overloads of method '{methodName}' found on type '{fullTypeName}'. Specify parameterTypes to resolve.");
                method = methods[0];
            }

            // Build the member reference using the ORIGINAL declaringType (which may be constructed generic)
            // This ensures we get the right TypeSpec, but the signature will use generic type parameters
            var declaringTypeHandle = GetOrAddDeclaringTypeHandle(declaringType);
            var signature = BuildMethodSignature(method);
            var nameHandle = _metadataBuilder.GetOrAddString(methodName);

            var handle = _metadataBuilder.AddMemberReference(declaringTypeHandle, nameHandle, signature);
            _cache[key] = handle;
            return handle;
        }

        /// <summary>
        /// Gets or creates a MethodSpecificationHandle for Array.Empty&lt;object&gt;().
        /// This is a generic method instantiation that returns an empty object[] array.
        /// </summary>
        /// <returns>A cached or newly created MethodSpecificationHandle.</returns>
        public MethodSpecificationHandle GetOrAddArrayEmptyObject()
        {
            var key = "System.Array::Empty<object>()";
            if (_methodSpecCache.TryGetValue(key, out var cached))
                return cached;

            // Step 1: Create member reference for the generic method definition Array.Empty<T>()
            // The signature needs to describe the generic method with return type !!0[] (array of generic method param 0)
            var arrayTypeRef = _typeRefRegistry.GetOrAdd(typeof(Array));
            var methodNameHandle = _metadataBuilder.GetOrAddString("Empty");

            // Build signature for generic method: static !!0[] Empty<T>() with 1 generic parameter
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature(isInstanceMethod: false, genericParameterCount: 1)
                .Parameters(0,
                    returnTypeEncoder => returnTypeEncoder.Type().SZArray().GenericMethodTypeParameter(0),
                    parametersEncoder => { });
            var methodSig = _metadataBuilder.GetOrAddBlob(sigBuilder);

            var methodRef = _metadataBuilder.AddMemberReference(arrayTypeRef, methodNameHandle, methodSig);

            // Step 2: Create MethodSpecification that instantiates Array.Empty<T> with T = object
            var specBlob = new BlobBuilder();
            new BlobEncoder(specBlob)
                .MethodSpecificationSignature(1)
                .AddArgument()
                .Object();
            var specBlobHandle = _metadataBuilder.GetOrAddBlob(specBlob);

            var methodSpec = _metadataBuilder.AddMethodSpecification(methodRef, specBlobHandle);
            _methodSpecCache[key] = methodSpec;
            return methodSpec;
        }

        /// <summary>
        /// Gets or creates a constructor member reference handle.
        /// Uses reflection to discover the constructor signature automatically.
        /// Supports constructed generic types (e.g., Func&lt;object, object&gt;) by automatically creating TypeSpecifications.
        /// </summary>
        /// <param name="declaringType">The type that declares the constructor.</param>
        /// <param name="parameterTypes">The parameter types of the constructor. Use empty array for parameterless constructor.</param>
        /// <returns>A cached or newly created MemberReferenceHandle.</returns>
        public MemberReferenceHandle GetOrAddConstructor(Type declaringType, Type[]? parameterTypes = null)
        {
            var fullTypeName = declaringType.FullName ?? $"{declaringType.Namespace}.{declaringType.Name}";
            var paramKey = parameterTypes == null ? "auto" : (parameterTypes.Length == 0 ? "" : string.Join(",", parameterTypes.Select(p => p.FullName)));
            var key = $"{fullTypeName}::.ctor({paramKey})";

            if (_cache.TryGetValue(key, out var existing))
                return existing;

            // Use reflection to find the constructor
            ConstructorInfo ctor;
            if (parameterTypes == null)
            {
                // Auto-discover: require exactly one constructor
                var ctors = declaringType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (ctors.Length == 0)
                    throw new ArgumentException($"No constructors found on type '{fullTypeName}'");
                if (ctors.Length > 1)
                    throw new ArgumentException($"Multiple constructors found on type '{fullTypeName}'. Specify parameterTypes to resolve.");
                ctor = ctors[0];
            }
            else
            {
                ctor = declaringType.GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    parameterTypes,
                    null) ?? throw new ArgumentException($"Constructor with signature ({paramKey}) not found on type '{fullTypeName}'");
            }

            // Build the member reference - use smart declaring type resolution for generics
            var declaringTypeHandle = GetOrAddDeclaringTypeHandle(declaringType);
            var signature = BuildConstructorSignature(ctor);
            var nameHandle = _metadataBuilder.GetOrAddString(".ctor");

            var handle = _metadataBuilder.AddMemberReference(declaringTypeHandle, nameHandle, signature);
            _cache[key] = handle;
            return handle;
        }

        /// <summary>
        /// Gets or creates a property getter member reference handle.
        /// </summary>
        public MemberReferenceHandle GetOrAddPropertyGetter(Type declaringType, string propertyName)
        {
            throw new NotImplementedException("GetOrAddPropertyGetter not yet implemented. Use GetOrAddMethod with 'get_PropertyName' for now.");
        }

        /// <summary>
        /// Gets or creates a field member reference handle.
        /// </summary>
        public MemberReferenceHandle GetOrAddField(Type declaringType, string fieldName)
        {
            var fullTypeName = declaringType.FullName ?? declaringType.Name;
            var key = $"{fullTypeName}::{fieldName}";

            if (_cache.TryGetValue(key, out var existing))
                return existing;

            // Use reflection to find the field
            var field = declaringType.GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.Static)
                ?? throw new ArgumentException($"Field '{fieldName}' not found on type '{fullTypeName}'");

            // Build the member reference
            var declaringTypeHandle = GetOrAddDeclaringTypeHandle(declaringType);
            var signature = BuildFieldSignature(field);
            var nameHandle = _metadataBuilder.GetOrAddString(fieldName);

            var handle = _metadataBuilder.AddMemberReference(declaringTypeHandle, nameHandle, signature);
            _cache[key] = handle;
            return handle;
        }

        /// <summary>
        /// Gets or creates an appropriate declaring type handle for a given type.
        /// For non-generic or generic definition types, returns a TypeReferenceHandle.
        /// For constructed generic types (e.g., Func&lt;object, object&gt;), builds and caches a TypeSpecificationHandle.
        /// </summary>
        private EntityHandle GetOrAddDeclaringTypeHandle(Type type)
        {
            // Non-generic or generic definition: use simple type reference
            if (!type.IsGenericType || type.IsGenericTypeDefinition)
                return _typeRefRegistry.GetOrAdd(type);

            // Constructed generic type: build TypeSpecification
            if (_typeSpecCache.TryGetValue(type, out var cachedSpec))
                return cachedSpec;

            var openType = type.GetGenericTypeDefinition();
            var openTypeRef = _typeRefRegistry.GetOrAdd(openType);
            var genericArgs = type.GetGenericArguments();

            var specBlob = new BlobBuilder();
            var genInst = new BlobEncoder(specBlob)
                .TypeSpecificationSignature()
                .GenericInstantiation(openTypeRef, genericArgs.Length, isValueType: false);

            // Encode each generic argument
            foreach (var arg in genericArgs)
            {
                EncodeGenericArgument(genInst.AddArgument(), arg);
            }

            var specBlobHandle = _metadataBuilder.GetOrAddBlob(specBlob);
            var specHandle = _metadataBuilder.AddTypeSpecification(specBlobHandle);
            _typeSpecCache[type] = specHandle;
            return specHandle;
        }

        /// <summary>
        /// Encodes a generic type argument into a TypeSpecification.
        /// Supports the common types used in js2il (object, object[], string, int, etc.).
        /// </summary>
        private void EncodeGenericArgument(SignatureTypeEncoder encoder, Type type)
        {
            // Primitive types
            if (type == typeof(object)) encoder.Object();
            else if (type == typeof(string)) encoder.String();
            else if (type == typeof(int)) encoder.Int32();
            else if (type == typeof(double)) encoder.Double();
            else if (type == typeof(bool)) encoder.Boolean();
            else if (type == typeof(long)) encoder.Int64();
            else if (type == typeof(float)) encoder.Single();
            else if (type == typeof(byte)) encoder.Byte();
            else if (type == typeof(short)) encoder.Int16();
            // Arrays
            else if (type == typeof(object[])) encoder.SZArray().Object();
            else if (type == typeof(string[])) encoder.SZArray().String();
            else if (type == typeof(int[])) encoder.SZArray().Int32();
            // IntPtr for delegate constructors
            else if (type == typeof(IntPtr)) encoder.IntPtr();
            // Other types via type reference
            else if (type.Namespace?.StartsWith("JavaScriptRuntime", StringComparison.Ordinal) == true)
            {
                var typeRef = _typeRefRegistry.GetOrAdd(type);
                encoder.Type(typeRef, isValueType: type.IsValueType);
            }
            else
            {
                throw new NotSupportedException(
                    $"Generic argument type '{type.FullName ?? type.Name}' is not yet supported in TypeSpecifications. " +
                    $"Supported types: object, string, int, double, bool, long, float, byte, short, object[], string[], int[], IntPtr, and JavaScriptRuntime types.");
            }
        }

        private BlobHandle BuildMethodSignature(MethodInfo method)
        {
            var sigBuilder = new BlobBuilder();
            var isInstance = !method.IsStatic;
            var returnType = method.ReturnType;
            var parameters = method.GetParameters();
            var genericParamCount = method.GetGenericArguments().Length;

            new BlobEncoder(sigBuilder)
                .MethodSignature(isInstanceMethod: isInstance, genericParameterCount: genericParamCount)
                .Parameters(parameters.Length,
                    returnTypeEncoder => EncodeReturnType(returnTypeEncoder, returnType),
                    parametersEncoder =>
                    {
                        foreach (var param in parameters)
                        {
                            EncodeSignatureType(parametersEncoder.AddParameter().Type(), param.ParameterType);
                        }
                    });

            var signature =  _metadataBuilder.GetOrAddBlob(sigBuilder);

            if (genericParamCount > 0)
            {
                // For generic methods, we need to wrap the signature in a GenericMethodSignature
                var genericSigBuilder = new BlobBuilder();
                var genericTypeArgumentsEncoder = new BlobEncoder(genericSigBuilder).MethodSpecificationSignature(genericParamCount);
                for (int genericTypeIndex = 0; genericTypeIndex < genericParamCount; genericTypeIndex++)
                {
                    genericTypeArgumentsEncoder.AddArgument().Object();
                }

                signature = _metadataBuilder.GetOrAddBlob(genericSigBuilder);
            }

            return signature;
        }

        private BlobHandle BuildConstructorSignature(ConstructorInfo ctor)
        {
            var sigBuilder = new BlobBuilder();
            var parameters = ctor.GetParameters();

            new BlobEncoder(sigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(parameters.Length,
                    returnTypeEncoder => returnTypeEncoder.Void(),
                    parametersEncoder =>
                    {
                        foreach (var param in parameters)
                        {
                            EncodeSignatureType(parametersEncoder.AddParameter().Type(), param.ParameterType);
                        }
                    });

            return _metadataBuilder.GetOrAddBlob(sigBuilder);
        }

        private BlobHandle BuildFieldSignature(FieldInfo field)
        {
            var sigBuilder = new BlobBuilder();
            var encoder = new BlobEncoder(sigBuilder).FieldSignature();
            EncodeSignatureType(encoder, field.FieldType);
            return _metadataBuilder.GetOrAddBlob(sigBuilder);
        }

        private void EncodeReturnType(ReturnTypeEncoder encoder, Type type)
        {
            if (type == typeof(void))
            {
                encoder.Void();
            }
            else
            {
                EncodeSignatureType(encoder.Type(), type);
            }
        }

        private void EncodeSignatureType(SignatureTypeEncoder encoder, Type type)
        {
            // Generic type parameters (e.g., T, TResult from Func<T, TResult>)
            if (type.IsGenericParameter)
            {
                encoder.GenericTypeParameter(type.GenericParameterPosition);
                return;
            }
            
            // Primitive types
            if (type == typeof(object)) encoder.Object();
            else if (type == typeof(string)) encoder.String();
            else if (type == typeof(double)) encoder.Double();
            else if (type == typeof(bool)) encoder.Boolean();
            else if (type == typeof(int)) encoder.Int32();
            else if (type == typeof(long)) encoder.Int64();
            else if (type == typeof(float)) encoder.Single();
            else if (type == typeof(byte)) encoder.Byte();
            else if (type == typeof(short)) encoder.Int16();
            else if (type == typeof(IntPtr)) encoder.IntPtr();
            // Arrays
            else if (type == typeof(object[])) encoder.SZArray().Object();
            else if (type == typeof(string[])) encoder.SZArray().String();
            // Common BCL types (should we just open it up for all types)
            else if (type == typeof(Action) || type == typeof(System.Reflection.MethodBase))
            {
                var bclTypeReference = _typeRefRegistry.GetOrAdd(type);
                encoder.Type(bclTypeReference, isValueType: false);
            }
            // JavaScriptRuntime types (including nested types like Node.Process)
            else if (type.Namespace?.StartsWith("JavaScriptRuntime", StringComparison.Ordinal) == true)
            {
                var typeRef = _typeRefRegistry.GetOrAdd(type);
                encoder.Type(typeRef, isValueType: type.IsValueType);
            }
            // Fallback for other types
            else
            {
                throw new NotSupportedException(
                    $"Type '{type.FullName ?? type.Name}' from namespace '{type.Namespace}' is not supported in method signatures. " +
                    $"Only JavaScriptRuntime types and primitive BCL types (object, string, double, bool, int, object[], Action, IntPtr) are supported.");
            }
        }
    }
}
