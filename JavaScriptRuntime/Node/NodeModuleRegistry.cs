using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JavaScriptRuntime.Node
{
    public static class NodeModuleRegistry
    {
        private static readonly Lazy<Dictionary<string, Type>> ModulesByName = new(() =>
        {
            var asm = typeof(NodeModuleAttribute).Assembly;
            var modules = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            foreach (var t in asm.GetTypes())
            {
                if (!t.IsClass || t.IsAbstract) continue;
                if (!string.Equals(t.Namespace, "JavaScriptRuntime.Node", StringComparison.Ordinal)) continue;

                var attr = t.GetCustomAttribute<NodeModuleAttribute>(false);
                if (attr == null || string.IsNullOrWhiteSpace(attr.Name)) continue;

                var name = NormalizeModuleName(attr.Name);
                if (string.IsNullOrWhiteSpace(name)) continue;

                if (!modules.ContainsKey(name))
                {
                    modules.Add(name, t);
                }
            }

            return modules;
        });

        public static string NormalizeModuleName(string specifier)
        {
            if (specifier == null)
            {
                return string.Empty;
            }

            var trimmed = specifier.Trim();
            if (trimmed.StartsWith("node:", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed.Substring("node:".Length);
            }

            return trimmed;
        }

        public static IReadOnlyCollection<string> GetSupportedModuleNames()
        {
            return ModulesByName.Value.Keys.ToArray();
        }

        public static bool TryGetModuleType(string specifier, out Type? type)
        {
            var key = NormalizeModuleName(specifier);
            if (string.IsNullOrWhiteSpace(key))
            {
                type = null;
                return false;
            }

            return ModulesByName.Value.TryGetValue(key, out type);
        }
    }
}
