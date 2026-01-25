using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Js2IL.Services
{
    /// <summary>
    /// Tracks .NET type handles for JavaScript class declarations by name.
    /// Populated by ClassesGenerator and consumed by IL emitters for new-expressions and instance calls.
    /// </summary>
    internal sealed class ClassRegistry
    {
        private readonly Dictionary<string, TypeDefinitionHandle> _classes = new(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<string, FieldDefinitionHandle>> _classFields = new(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<string, FieldDefinitionHandle>> _classPrivateFields = new(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<string, FieldDefinitionHandle>> _classStaticFields = new(StringComparer.Ordinal);

        private readonly Dictionary<string, Dictionary<string, Type>> _classFieldClrTypes = new(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<string, Type>> _classPrivateFieldClrTypes = new(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<string, Type>> _classStaticFieldClrTypes = new(StringComparer.Ordinal);

        // For strongly-typed user-class fields, we need the declared metadata type handle for castclass/stfld correctness.
        private readonly Dictionary<string, Dictionary<string, EntityHandle>> _classFieldTypeHandles = new(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<string, EntityHandle>> _classPrivateFieldTypeHandles = new(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<string, EntityHandle>> _classStaticFieldTypeHandles = new(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<string, MemberReferenceHandle>> _classStaticMethods = new(StringComparer.Ordinal);
        // Cache per-class constructor definition + signature + parameter count so call sites can reuse
        // and validate instead of rebuilding duplicate member references.
        // MinParamCount = required params (no defaults), MaxParamCount = all params (including defaults)
        private readonly Dictionary<string, (MethodDefinitionHandle Ctor, BlobHandle Signature, bool HasScopesParam, int MinParamCount, int MaxParamCount)> _constructors = new(StringComparer.Ordinal);
        // Track instance methods: className -> methodName -> (MethodDef, Signature, ReturnClrType, ReturnTypeHandle, HasScopesParam, MinParams, MaxParams)
        // NOTE: Min/MaxParamCount are JS parameter counts (do NOT include scopes), kept for call-site validation/padding.
        private readonly Dictionary<string, Dictionary<string, (MethodDefinitionHandle Method, BlobHandle Signature, Type ReturnClrType, EntityHandle ReturnTypeHandle, bool HasScopesParam, int MinParamCount, int MaxParamCount)>> _methods = new(StringComparer.Ordinal);

        public void Register(string className, TypeDefinitionHandle typeHandle)
        {
            if (string.IsNullOrEmpty(className)) return;
            _classes[className] = typeHandle;
        }

        public bool TryGet(string className, out TypeDefinitionHandle handle)
        {
            return _classes.TryGetValue(className, out handle);
        }

        public void RegisterField(string className, string fieldName, FieldDefinitionHandle fieldHandle)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(fieldName)) return;
            if (!_classFields.TryGetValue(className, out var fields))
            {
                fields = new Dictionary<string, FieldDefinitionHandle>(StringComparer.Ordinal);
                _classFields[className] = fields;
            }
            fields[fieldName] = fieldHandle;
        }

        public void RegisterFieldClrType(string className, string fieldName, Type fieldClrType)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(fieldName) || fieldClrType == null) return;
            if (!_classFieldClrTypes.TryGetValue(className, out var fields))
            {
                fields = new Dictionary<string, Type>(StringComparer.Ordinal);
                _classFieldClrTypes[className] = fields;
            }
            fields[fieldName] = fieldClrType;
        }

        public void RegisterFieldTypeHandle(string className, string fieldName, EntityHandle typeHandle)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(fieldName) || typeHandle.IsNil) return;
            if (!_classFieldTypeHandles.TryGetValue(className, out var fields))
            {
                fields = new Dictionary<string, EntityHandle>(StringComparer.Ordinal);
                _classFieldTypeHandles[className] = fields;
            }
            fields[fieldName] = typeHandle;
        }

        public bool TryGetFieldTypeHandle(string className, string fieldName, out EntityHandle typeHandle)
        {
            typeHandle = default;
            if (_classFieldTypeHandles.TryGetValue(className, out var fields) &&
                fields.TryGetValue(fieldName, out var h))
            {
                typeHandle = h;
                return true;
            }
            return false;
        }

        public bool TryGetFieldClrType(string className, string fieldName, out Type fieldClrType)
        {
            fieldClrType = typeof(object);
            if (_classFieldClrTypes.TryGetValue(className, out var fields) &&
                fields.TryGetValue(fieldName, out var t))
            {
                fieldClrType = t;
                return true;
            }
            return false;
        }

        public bool TryGetField(string className, string fieldName, out FieldDefinitionHandle fieldHandle)
        {
            fieldHandle = default;
            if (_classFields.TryGetValue(className, out var fields))
            {
                return fields.TryGetValue(fieldName, out fieldHandle);
            }
            return false;
        }

        public void RegisterPrivateField(string className, string fieldName, FieldDefinitionHandle fieldHandle)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(fieldName)) return;
            if (!_classPrivateFields.TryGetValue(className, out var fields))
            {
                fields = new Dictionary<string, FieldDefinitionHandle>(StringComparer.Ordinal);
                _classPrivateFields[className] = fields;
            }
            fields[fieldName] = fieldHandle;
        }

        public void RegisterPrivateFieldClrType(string className, string fieldName, Type fieldClrType)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(fieldName) || fieldClrType == null) return;
            if (!_classPrivateFieldClrTypes.TryGetValue(className, out var fields))
            {
                fields = new Dictionary<string, Type>(StringComparer.Ordinal);
                _classPrivateFieldClrTypes[className] = fields;
            }
            fields[fieldName] = fieldClrType;
        }

        public void RegisterPrivateFieldTypeHandle(string className, string fieldName, EntityHandle typeHandle)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(fieldName) || typeHandle.IsNil) return;
            if (!_classPrivateFieldTypeHandles.TryGetValue(className, out var fields))
            {
                fields = new Dictionary<string, EntityHandle>(StringComparer.Ordinal);
                _classPrivateFieldTypeHandles[className] = fields;
            }
            fields[fieldName] = typeHandle;
        }

        public bool TryGetPrivateFieldTypeHandle(string className, string fieldName, out EntityHandle typeHandle)
        {
            typeHandle = default;
            if (_classPrivateFieldTypeHandles.TryGetValue(className, out var fields) &&
                fields.TryGetValue(fieldName, out var h))
            {
                typeHandle = h;
                return true;
            }
            return false;
        }

        public bool TryGetPrivateFieldClrType(string className, string fieldName, out Type fieldClrType)
        {
            fieldClrType = typeof(object);
            if (_classPrivateFieldClrTypes.TryGetValue(className, out var fields) &&
                fields.TryGetValue(fieldName, out var t))
            {
                fieldClrType = t;
                return true;
            }
            return false;
        }

        public bool TryGetPrivateField(string className, string fieldName, out FieldDefinitionHandle fieldHandle)
        {
            fieldHandle = default;
            if (_classPrivateFields.TryGetValue(className, out var fields))
            {
                return fields.TryGetValue(fieldName, out fieldHandle);
            }
            return false;
        }

        public void RegisterStaticField(string className, string fieldName, FieldDefinitionHandle fieldHandle)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(fieldName)) return;
            if (!_classStaticFields.TryGetValue(className, out var fields))
            {
                fields = new Dictionary<string, FieldDefinitionHandle>(StringComparer.Ordinal);
                _classStaticFields[className] = fields;
            }
            fields[fieldName] = fieldHandle;
        }

        public void RegisterStaticFieldClrType(string className, string fieldName, Type fieldClrType)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(fieldName) || fieldClrType == null) return;
            if (!_classStaticFieldClrTypes.TryGetValue(className, out var fields))
            {
                fields = new Dictionary<string, Type>(StringComparer.Ordinal);
                _classStaticFieldClrTypes[className] = fields;
            }
            fields[fieldName] = fieldClrType;
        }

        public void RegisterStaticFieldTypeHandle(string className, string fieldName, EntityHandle typeHandle)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(fieldName) || typeHandle.IsNil) return;
            if (!_classStaticFieldTypeHandles.TryGetValue(className, out var fields))
            {
                fields = new Dictionary<string, EntityHandle>(StringComparer.Ordinal);
                _classStaticFieldTypeHandles[className] = fields;
            }
            fields[fieldName] = typeHandle;
        }

        public bool TryGetStaticFieldTypeHandle(string className, string fieldName, out EntityHandle typeHandle)
        {
            typeHandle = default;
            if (_classStaticFieldTypeHandles.TryGetValue(className, out var fields) &&
                fields.TryGetValue(fieldName, out var h))
            {
                typeHandle = h;
                return true;
            }
            return false;
        }

        public bool TryGetStaticFieldClrType(string className, string fieldName, out Type fieldClrType)
        {
            fieldClrType = typeof(object);
            if (_classStaticFieldClrTypes.TryGetValue(className, out var fields) &&
                fields.TryGetValue(fieldName, out var t))
            {
                fieldClrType = t;
                return true;
            }
            return false;
        }

        public bool TryGetStaticField(string className, string fieldName, out FieldDefinitionHandle fieldHandle)
        {
            fieldHandle = default;
            if (_classStaticFields.TryGetValue(className, out var fields))
            {
                return fields.TryGetValue(fieldName, out fieldHandle);
            }
            return false;
        }

        public void RegisterStaticMethod(string className, string methodName, MemberReferenceHandle memberRef)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(methodName)) return;
            if (!_classStaticMethods.TryGetValue(className, out var methods))
            {
                methods = new Dictionary<string, MemberReferenceHandle>(StringComparer.Ordinal);
                _classStaticMethods[className] = methods;
            }
            methods[methodName] = memberRef;
        }

        public bool TryGetStaticMethod(string className, string methodName, out MemberReferenceHandle memberRef)
        {
            memberRef = default;
            if (_classStaticMethods.TryGetValue(className, out var methods))
            {
                return methods.TryGetValue(methodName, out memberRef);
            }
            return false;
        }

        public void RegisterConstructor(string className, MethodDefinitionHandle ctorHandle, BlobHandle signature, bool hasScopesParam, int minParamCount, int maxParamCount)
        {
            if (string.IsNullOrEmpty(className)) return;
            _constructors[className] = (ctorHandle, signature, hasScopesParam, minParamCount, maxParamCount);
        }

        public bool TryGetConstructor(string className, out MethodDefinitionHandle ctorHandle, out bool hasScopesParam, out int minParamCount, out int maxParamCount)
        {
            ctorHandle = default;
            hasScopesParam = false;
            minParamCount = 0;
            maxParamCount = 0;
            if (_constructors.TryGetValue(className, out var info))
            {
                ctorHandle = info.Ctor;
                hasScopesParam = info.HasScopesParam;
                minParamCount = info.MinParamCount;
                maxParamCount = info.MaxParamCount;
                return true;
            }
            return false;
        }

        public void RegisterMethod(string className, string methodName, MethodDefinitionHandle methodHandle, BlobHandle signature, Type returnClrType, EntityHandle returnTypeHandle, bool hasScopesParam, int minParamCount, int maxParamCount)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(methodName)) return;
            if (!_methods.TryGetValue(className, out var methods))
            {
                methods = new Dictionary<string, (MethodDefinitionHandle Method, BlobHandle Signature, Type ReturnClrType, EntityHandle ReturnTypeHandle, bool HasScopesParam, int MinParamCount, int MaxParamCount)>(StringComparer.Ordinal);
                _methods[className] = methods;
            }
            methods[methodName] = (methodHandle, signature, returnClrType ?? typeof(object), returnTypeHandle, hasScopesParam, minParamCount, maxParamCount);
        }

        public bool TryGetMethod(string className, string methodName, out MethodDefinitionHandle methodHandle, out BlobHandle signature, out Type returnClrType, out bool hasScopesParam, out int minParamCount, out int maxParamCount)
        {
            methodHandle = default;
            signature = default;
            returnClrType = typeof(object);
            hasScopesParam = false;
            minParamCount = 0;
            maxParamCount = 0;
            if (_methods.TryGetValue(className, out var methods) && 
                methods.TryGetValue(methodName, out var info))
            {
                methodHandle = info.Method;
                signature = info.Signature;
                returnClrType = info.ReturnClrType;
                hasScopesParam = info.HasScopesParam;
                minParamCount = info.MinParamCount;
                maxParamCount = info.MaxParamCount;
                return true;
            }
            return false;
        }

        public bool TryGetMethod(string className, string methodName, out MethodDefinitionHandle methodHandle, out BlobHandle signature, out Type returnClrType, out EntityHandle returnTypeHandle, out bool hasScopesParam, out int minParamCount, out int maxParamCount)
        {
            methodHandle = default;
            signature = default;
            returnClrType = typeof(object);
            returnTypeHandle = default;
            hasScopesParam = false;
            minParamCount = 0;
            maxParamCount = 0;
            if (_methods.TryGetValue(className, out var methods) &&
                methods.TryGetValue(methodName, out var info))
            {
                methodHandle = info.Method;
                signature = info.Signature;
                returnClrType = info.ReturnClrType;
                returnTypeHandle = info.ReturnTypeHandle;
                hasScopesParam = info.HasScopesParam;
                minParamCount = info.MinParamCount;
                maxParamCount = info.MaxParamCount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to resolve a uniquely-defined instance method across all registered classes
        /// by method name and call-site argument count. This is used for conservative fast-path
        /// emission (type-test + direct call) that can fall back to runtime dispatch when the
        /// receiver is not of the resolved class.
        /// </summary>
        public bool TryResolveUniqueInstanceMethod(
            string methodName,
            int argumentCount,
            out string registryClassName,
            out TypeDefinitionHandle typeHandle,
            out MethodDefinitionHandle methodHandle,
            out Type returnClrType,
            out bool hasScopesParam,
            out int maxParamCount)
        {
            registryClassName = string.Empty;
            typeHandle = default;
            methodHandle = default;
            returnClrType = typeof(object);
            hasScopesParam = false;
            maxParamCount = 0;

            if (string.IsNullOrEmpty(methodName))
            {
                return false;
            }

            string? matchClass = null;
            MethodDefinitionHandle matchMethod = default;
            int matchMaxParams = 0;
            Type matchReturnClrType = typeof(object);
            bool matchHasScopesParam = false;

            foreach (var kvp in _methods)
            {
                var className = kvp.Key;
                var methods = kvp.Value;

                if (!methods.TryGetValue(methodName, out var info))
                {
                    continue;
                }

                // Only consider methods that can accept this call-site arity.
                if (argumentCount < info.MinParamCount || argumentCount > info.MaxParamCount)
                {
                    continue;
                }

                // If there are multiple candidates, it's not safe to pick one.
                if (matchClass != null)
                {
                    return false;
                }

                matchClass = className;
                matchMethod = info.Method;
                matchMaxParams = info.MaxParamCount;
                matchReturnClrType = info.ReturnClrType;
                matchHasScopesParam = info.HasScopesParam;
            }

            if (matchClass == null)
            {
                return false;
            }

            if (!TryGet(matchClass, out var resolvedType))
            {
                return false;
            }

            registryClassName = matchClass;
            typeHandle = resolvedType;
            methodHandle = matchMethod;
            returnClrType = matchReturnClrType;
            hasScopesParam = matchHasScopesParam;
            maxParamCount = matchMaxParams;
            return true;
        }
    }
}
