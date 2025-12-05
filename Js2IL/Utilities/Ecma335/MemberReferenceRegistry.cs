using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.Utilities.Ecma335
{
    /// <summary>
    /// Centralized registry for MemberReferenceHandles to avoid duplicate metadata entries.
    /// Automatically builds method signatures via reflection for intrinsic runtime methods.
    /// </summary>
    internal sealed class MemberReferenceRegistry
    {
        private readonly MetadataBuilder _metadataBuilder;
        private readonly TypeReferenceRegistry _typeRefRegistry;
        private readonly Dictionary<string, MemberReferenceHandle> _cache = new(StringComparer.Ordinal);

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

            // Use reflection to find the method
            var methods = declaringType.GetMethods(
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

            // Build the member reference
            var typeRef = _typeRefRegistry.GetOrAdd(declaringType);
            var signature = BuildMethodSignature(method);
            var nameHandle = _metadataBuilder.GetOrAddString(methodName);

            var handle = _metadataBuilder.AddMemberReference(typeRef, nameHandle, signature);
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
            throw new NotImplementedException("GetOrAddField not yet implemented.");
        }

        private BlobHandle BuildMethodSignature(MethodInfo method)
        {
            var sigBuilder = new BlobBuilder();
            var isInstance = !method.IsStatic;
            var returnType = method.ReturnType;
            var parameters = method.GetParameters();

            new BlobEncoder(sigBuilder)
                .MethodSignature(isInstanceMethod: isInstance)
                .Parameters(parameters.Length,
                    returnTypeEncoder => EncodeReturnType(returnTypeEncoder, returnType),
                    parametersEncoder =>
                    {
                        foreach (var param in parameters)
                        {
                            EncodeSignatureType(parametersEncoder.AddParameter().Type(), param.ParameterType);
                        }
                    });

            return _metadataBuilder.GetOrAddBlob(sigBuilder);
        }

        private void EncodeReturnType(ReturnTypeEncoder encoder, Type type)
        {
            if (type == typeof(void))
                encoder.Void();
            else
                EncodeSignatureType(encoder.Type(), type);
        }

        private void EncodeSignatureType(SignatureTypeEncoder encoder, Type type)
        {
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
            // Arrays
            else if (type == typeof(object[])) encoder.SZArray().Object();
            else if (type == typeof(string[])) encoder.SZArray().String();
            // Common BCL types
            else if (type == typeof(Action))
            {
                var actionTypeRef = _typeRefRegistry.GetOrAdd(typeof(Action));
                encoder.Type(actionTypeRef, isValueType: false);
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
                    $"Only JavaScriptRuntime types and primitive BCL types (object, string, double, bool, int, object[], Action) are supported.");
            }
        }
    }
}
