using System.Reflection;

namespace JavaScriptRuntime.CommonJS
{
    sealed class Require
    {
        // Registry and instance cache; resolved lazily via [Node.NodeModule] attributes
        private readonly Dictionary<string, Type> _registry = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, object> _instances = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _notFound = new(StringComparer.OrdinalIgnoreCase);

        private readonly Assembly _localModulesAssembly;

        public Require(LocalModulesAssembly localModulesAssembly)
        {
            // Preload local modules from the provided assembly
            this._localModulesAssembly = localModulesAssembly.ModulesAssembly;
        }

        // Deferred type lookup to avoid startup cost; scans assembly only on demand.
        private Type? FindModuleType(string name)
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
        public object? RequireModule(string specifier)
        {
            if (string.IsNullOrWhiteSpace(specifier))
                throw new ReferenceError("require specifier must be a non-empty string"); 


            var key = Normalize(specifier);
            var isLocalModule = key.StartsWith("./") || key.StartsWith("../") || key.StartsWith("/");

            if (_instances.TryGetValue(key, out var existing))
                return existing;

            if (isLocalModule)
            {
                var moduleName = Path.GetFileNameWithoutExtension(key);
                var TypeName = $"Scripts.{moduleName}";
                var localType = _localModulesAssembly.GetType(TypeName);
                if (localType == null)
                    throw new ReferenceError($"Cannot find local module type '{TypeName}' in assembly");

                // method is a static member
                var moduleEntryPoint = localType.GetMethod("Main");
                if (moduleEntryPoint == null)
                    throw new TypeError($"Local module '{moduleName}' does not have a static Main method");

                var moduleDelegate = (ModuleMainDelegate)Delegate.CreateDelegate(
                    typeof(ModuleMainDelegate), moduleEntryPoint);

                RequireDelegate require = (moduleId) => 
                {
                    if (moduleId is not string moduleName || moduleId == null)
                    {
                        throw new TypeError("The \"id\" argument must be of type string.");
                    }
                    return RequireModule(moduleName);
                };

                moduleDelegate(null, require, null, key, Path.GetDirectoryName(key) ?? "");

                // todo
                return null;

            }
            else
            {
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
