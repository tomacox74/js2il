using System.Reflection;
using System.Dynamic;

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
                var moduleId = ModuleName.GetModuleIdFromSpecifier(key);
                var TypeName = $"Scripts.{moduleId}";
                var localType = _localModulesAssembly.GetType(TypeName);
                if (localType == null)
                    throw new ReferenceError($"Cannot find local module type '{TypeName}' in assembly");

                // Create (and cache) exports before executing to ensure modules are singletons
                // and to support cycles (require() during initialization returns the same exports).
                var exports = new ExpandoObject();
                _instances[key] = exports;

                // method is a static member
                var moduleEntryPoint = localType.GetMethod("Main");
                if (moduleEntryPoint == null)
                    throw new TypeError($"Local module '{moduleId}' does not have a static Main method");

                var moduleDelegate = (ModuleMainDelegate)Delegate.CreateDelegate(
                    typeof(ModuleMainDelegate), moduleEntryPoint);

                // Create a per-module require() delegate that resolves relative specifiers
                // against this module's own path.
                RequireDelegate require = (moduleId) => 
                {
                    if (moduleId is not string requestedSpecifier || requestedSpecifier == null)
                    {
                        throw new TypeError("The \"id\" argument must be of type string.");
                    }

                    var resolved = ResolveLocalSpecifier(key, requestedSpecifier);
                    return RequireModule(resolved);
                };

                var dirName = GetDirectoryNameForwardSlash(key);
                moduleDelegate(exports, require, null, key, dirName);

                return exports;

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
            trimmed = trimmed.Replace('\\', '/');
            // Accept both 'node:fs' and 'fs' by stripping optional node: prefix
            if (trimmed.StartsWith("node:", StringComparison.OrdinalIgnoreCase))
                trimmed = trimmed.Substring("node:".Length);
            return trimmed;
        }

        private static string ResolveLocalSpecifier(string parentModuleFilename, string requestedSpecifier)
        {
            var request = Normalize(requestedSpecifier);

            // Absolute local import: treat as already rooted.
            if (request.StartsWith("/", StringComparison.Ordinal))
            {
                return request;
            }

            // Only resolve truly relative specifiers against parent module.
            if (!request.StartsWith("./", StringComparison.Ordinal) && !request.StartsWith("../", StringComparison.Ordinal))
            {
                return request;
            }

            var parent = Normalize(parentModuleFilename);

            // Use forward-slash path semantics for module ids.
            var parentDir = GetDirectoryNameForwardSlash(parent);

            // Combine and normalize dot segments.
            var combined = string.IsNullOrEmpty(parentDir)
                ? request
                : $"{parentDir}/{request}";

            var normalized = NormalizeDotSegments(combined);

            // Ensure local module specifier stays local.
            return normalized.StartsWith("./", StringComparison.Ordinal) || normalized.StartsWith("../", StringComparison.Ordinal) || normalized.StartsWith("/", StringComparison.Ordinal)
                ? normalized
                : $"./{normalized}";
        }

        private static string GetDirectoryNameForwardSlash(string path)
        {
            var p = Normalize(path);

            var lastSlash = p.LastIndexOf('/');
            if (lastSlash < 0)
            {
                return string.Empty;
            }

            return p.Substring(0, lastSlash);
        }

        private static string NormalizeDotSegments(string path)
        {
            var p = Normalize(path);

            // Preserve a leading '/' if present.
            var isAbsolute = p.StartsWith("/", StringComparison.Ordinal);

            var parts = p.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var stack = new List<string>(parts.Length);

            foreach (var part in parts)
            {
                if (part == ".")
                {
                    continue;
                }

                if (part == "..")
                {
                    if (stack.Count > 0)
                    {
                        stack.RemoveAt(stack.Count - 1);
                    }
                    continue;
                }

                stack.Add(part);
            }

            var joined = string.Join('/', stack);
            if (isAbsolute)
            {
                return "/" + joined;
            }

            return joined;
        }
    }
}
