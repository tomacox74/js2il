using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jroc.Runtime.Node.Contracts;

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

        private static readonly Lazy<Dictionary<string, Type>> ContractsByName = new(() =>
        {
            var contracts = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            var contractTypes = typeof(NodeModuleInterfaceAttribute).Assembly
                .GetTypes()
                .Where(static type => type.IsInterface)
                .OrderBy(static type => type.FullName, StringComparer.Ordinal);

            foreach (var type in contractTypes)
            {
                var attribute = type.GetCustomAttribute<NodeModuleInterfaceAttribute>(false);
                if (attribute == null)
                {
                    continue;
                }

                var name = NormalizeModuleName(attribute.ModuleName);
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                if (!contracts.TryAdd(name, type))
                {
                    throw new InvalidOperationException(
                        $"Multiple Node module contracts are registered for '{name}'.");
                }
            }

            return contracts;
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

        public static bool TryGetModuleContractType(string specifier, out Type? type)
        {
            var key = NormalizeModuleName(specifier);
            if (string.IsNullOrWhiteSpace(key))
            {
                type = null;
                return false;
            }

            return ContractsByName.Value.TryGetValue(key, out type);
        }
    }
}
