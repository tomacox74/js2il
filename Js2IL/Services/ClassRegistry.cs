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
    private readonly Dictionary<string, Dictionary<string, MemberReferenceHandle>> _classStaticMethods = new(StringComparer.Ordinal);

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
    }
}
