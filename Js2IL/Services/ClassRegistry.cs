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
        private readonly Dictionary<string, Dictionary<string, MemberReferenceHandle>> _classStaticMethods = new(StringComparer.Ordinal);
        // Cache per-class constructor definition + signature + parameter count so call sites can reuse
        // and validate instead of rebuilding duplicate member references.
        // MinParamCount = required params (no defaults), MaxParamCount = all params (including defaults)
        private readonly Dictionary<string, (MethodDefinitionHandle Ctor, BlobHandle Signature, int MinParamCount, int MaxParamCount)> _constructors = new(StringComparer.Ordinal);
        // Track instance methods: className -> methodName -> (MethodDef, Signature, MinParams, MaxParams)
        private readonly Dictionary<string, Dictionary<string, (MethodDefinitionHandle Method, BlobHandle Signature, int MinParamCount, int MaxParamCount)>> _methods = new(StringComparer.Ordinal);

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

        public void RegisterConstructor(string className, MethodDefinitionHandle ctorHandle, BlobHandle signature, int minParamCount, int maxParamCount)
        {
            if (string.IsNullOrEmpty(className)) return;
            _constructors[className] = (ctorHandle, signature, minParamCount, maxParamCount);
        }

        public bool TryGetConstructor(string className, out MethodDefinitionHandle ctorHandle, out int minParamCount, out int maxParamCount)
        {
            ctorHandle = default;
            minParamCount = 0;
            maxParamCount = 0;
            if (_constructors.TryGetValue(className, out var info))
            {
                ctorHandle = info.Ctor;
                minParamCount = info.MinParamCount;
                maxParamCount = info.MaxParamCount;
                return true;
            }
            return false;
        }

        public void RegisterMethod(string className, string methodName, MethodDefinitionHandle methodHandle, BlobHandle signature, int minParamCount, int maxParamCount)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(methodName)) return;
            if (!_methods.TryGetValue(className, out var methods))
            {
                methods = new Dictionary<string, (MethodDefinitionHandle, BlobHandle, int, int)>(StringComparer.Ordinal);
                _methods[className] = methods;
            }
            methods[methodName] = (methodHandle, signature, minParamCount, maxParamCount);
        }

        public bool TryGetMethod(string className, string methodName, out MethodDefinitionHandle methodHandle, out BlobHandle signature, out int minParamCount, out int maxParamCount)
        {
            methodHandle = default;
            signature = default;
            minParamCount = 0;
            maxParamCount = 0;
            if (_methods.TryGetValue(className, out var methods) && 
                methods.TryGetValue(methodName, out var info))
            {
                methodHandle = info.Method;
                signature = info.Signature;
                minParamCount = info.MinParamCount;
                maxParamCount = info.MaxParamCount;
                return true;
            }
            return false;
        }
    }
}
