using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Js2IL.Services
{
    internal sealed class FunctionRegistry
    {
        private readonly Dictionary<string, MethodDefinitionHandle> _functions = new(StringComparer.Ordinal);

        public void Register(string name, MethodDefinitionHandle handle)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (handle.IsNil) throw new ArgumentException("Method handle cannot be nil", nameof(handle));
            _functions[name] = handle;
        }

        public MethodDefinitionHandle Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return default;
            return _functions.TryGetValue(name, out var h) ? h : default;
        }
    }
}