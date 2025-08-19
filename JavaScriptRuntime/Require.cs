using System;
using System.Collections.Generic;
using System.Reflection;

namespace JavaScriptRuntime
{
    public static class Require
    {
        // Registry and instance cache; resolved lazily via [Node.NodeModule] attributes
        private static readonly Dictionary<string, Type> _registry = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, object> _instances = new(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> _notFound = new(StringComparer.OrdinalIgnoreCase);
        private static readonly object _sync = new();

        // Deferred type lookup to avoid startup cost; scans assembly only on demand.
        private static Type? FindModuleType(string name)
        {
            var asm = typeof(Require).Assembly;
            foreach (var t in asm.GetTypes())
            {
                if (!t.IsClass || t.IsAbstract) continue;
                if (!string.Equals(t.Namespace, "JavaScriptRuntime.Node", StringComparison.Ordinal)) continue;
                var attr = t.GetCustomAttribute<Node.NodeModuleAttribute>();
                if (attr == null) continue;
                if (string.Equals(attr.Name, name, StringComparison.OrdinalIgnoreCase))
                    return t;
            }
            return null;
        }

        // require("module") returns a Node core module instance; modules are singletons.
        // Unknown specifiers throw a ReferenceError.
        public static object require(string specifier)
        {
            if (string.IsNullOrWhiteSpace(specifier))
                throw new ReferenceError("require specifier must be a non-empty string");

            var key = Normalize(specifier);
            lock (_sync)
            {
                if (_instances.TryGetValue(key, out var existing))
                    return existing;

                if (!_registry.TryGetValue(key, out var type))
                {
                    if (_notFound.Contains(key))
                        throw new ReferenceError($"Cannot find module '{specifier}'");

                    type = FindModuleType(key);
                    if (type == null)
                    {
                        _notFound.Add(key);
                        throw new ReferenceError($"Cannot find module '{specifier}'");
                    }

                    _registry[key] = type;
                }

                var instance = Activator.CreateInstance(type)
                    ?? throw new TypeError($"Failed to create module instance for '{key}'");
                _instances[key] = instance;
                return instance;
            }
        }

        private static string Normalize(string s)
        {
            var trimmed = s.Trim();
            // Accept both 'node:fs' and 'fs' by stripping optional node: prefix
            if (trimmed.StartsWith("node:", StringComparison.OrdinalIgnoreCase))
                trimmed = trimmed.Substring("node:".Length);
            return trimmed;
        }
    }
}
