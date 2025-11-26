using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Js2IL.Services
{
    internal sealed class FunctionRegistry
    {
        private readonly Dictionary<string, (MethodDefinitionHandle Handle, int ParameterCount)> _functions = new(StringComparer.Ordinal);

        public void Register(string name, MethodDefinitionHandle handle, int parameterCount)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (handle.IsNil) throw new ArgumentException("Method handle cannot be nil", nameof(handle));
            _functions[name] = (handle, parameterCount);
        }

        /// <summary>
        /// Pre-register a function with its parameter count before the method handle is available.
        /// This is needed for recursive function expressions where the function body is generated
        /// before the method definition is finalized.
        /// </summary>
        public void PreRegisterParameterCount(string name, int parameterCount)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            // Use a nil handle as a placeholder; will be updated later with full registration
            if (!_functions.ContainsKey(name))
            {
                _functions[name] = (default, parameterCount);
            }
        }

        public MethodDefinitionHandle Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return default;
            return _functions.TryGetValue(name, out var entry) ? entry.Handle : default;
        }

        public int GetParameterCount(string name)
        {
            if (string.IsNullOrEmpty(name)) return 0;
            return _functions.TryGetValue(name, out var entry) ? entry.ParameterCount : 0;
        }

        /// <summary>
        /// Check if a function is registered (either pre-registered or fully registered).
        /// </summary>
        public bool IsRegistered(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return _functions.ContainsKey(name);
        }
    }
}