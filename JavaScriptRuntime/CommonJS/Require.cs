using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Linq;
using Js2IL.Runtime;

namespace JavaScriptRuntime.CommonJS
{
    internal sealed class Require
    {
        // Registry and instance cache; resolved lazily via [Node.NodeModule] attributes
        private readonly Dictionary<string, Type> _registry = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, object> _instances = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Module> _modules = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _notFound = new(StringComparer.OrdinalIgnoreCase);

        private readonly Assembly? _localModulesAssembly;

        // Mapping emitted by the compiler: logical module id -> CLR type name.
        // Populated lazily from assembly attributes.
        private Dictionary<string, (string CanonicalId, string TypeName)>? _compiledModuleTypeMap;
        
        // Track the current parent module for establishing parent-child relationships
        private Module? _currentParentModule;

        public Require(LocalModulesAssembly localModulesAssembly)
        {
            // Preload local modules from the provided assembly
            _localModulesAssembly = localModulesAssembly.ModulesAssembly;
        }

        private Dictionary<string, (string CanonicalId, string TypeName)> GetCompiledModuleTypeMap()
        {
            if (_compiledModuleTypeMap != null)
            {
                return _compiledModuleTypeMap;
            }

            var map = new Dictionary<string, (string CanonicalId, string TypeName)>(StringComparer.OrdinalIgnoreCase);

            if (_localModulesAssembly != null)
            {
                foreach (var attr in _localModulesAssembly.GetCustomAttributes<JsCompiledModuleTypeAttribute>())
                {
                    if (string.IsNullOrWhiteSpace(attr.ModuleId) || string.IsNullOrWhiteSpace(attr.TypeName))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(attr.CanonicalModuleId))
                    {
                        continue;
                    }

                    // Normalize IDs similar to runtime specifier normalization: forward slashes, no leading './'.
                    var id = NormalizeModuleIdKey(attr.ModuleId);
                    if (!map.ContainsKey(id))
                    {
                        map[id] = (NormalizeModuleIdKey(attr.CanonicalModuleId), attr.TypeName);
                    }
                }
            }

            _compiledModuleTypeMap = map;
            return _compiledModuleTypeMap;
        }

        /// <summary>
        /// Sets the current parent module for tracking parent-child relationships.
        /// </summary>
        internal void SetCurrentParent(Module? parent)
        {
            _currentParentModule = parent;
        }

        /// <summary>
        /// Gets the Module object for a given module key, if it exists.
        /// </summary>
        internal Module? GetModule(string key) => _modules.TryGetValue(key, out var m) ? m : null;

        // Deferred type lookup to avoid startup cost; scans assembly only on demand.
        private static Type? FindModuleType(string name)
        {
            return Node.NodeModuleRegistry.TryGetModuleType(name, out var type) ? type : null;
        }

        // require("module") returns a Node core module instance; modules are singletons.
        // Unknown specifiers throw a ReferenceError.
        public object? RequireModule(string specifier)
        {
            if (string.IsNullOrWhiteSpace(specifier))
                throw new ReferenceError("require specifier must be a non-empty string"); 

            var key = Normalize(specifier);

            // Fast path: already-instantiated Node core module singleton.
            if (_instances.TryGetValue(key, out var existing))
            {
                if (_currentParentModule != null && _modules.TryGetValue(key, out var existingModule))
                {
                    _currentParentModule.AddChild(existingModule);
                }

                return existing;
            }

            // Node core modules always win over compiled packages with the same name.
            if (FindModuleType(key) != null)
            {
                return RequireNodeModule(key, specifier);
            }

            // Everything else (relative paths + bare specifiers resolved at compile-time) is treated
            // as a compiled module inside the provided local modules assembly.
            return RequireLocalModule(key);
        }

        /// <summary>
        /// Requires a local module (user code compiled with the main script).
        /// </summary>
        private object? RequireLocalModule(string key)
        {
            if (_localModulesAssembly == null)
                throw new ReferenceError($"Cannot require local module '{key}': no local modules assembly provided");

            // First: use compiler-emitted mapping attributes.
            var moduleIdKey = NormalizeModuleIdKey(key);
            var map = GetCompiledModuleTypeMap();
            if (TryResolveFromMap(map, moduleIdKey, out var canonicalId, out var mappedTypeName))
            {
                return RequireCompiledModule(canonicalId, mappedTypeName, requestKey: key);
            }

            // Fallback: legacy sanitized type name lookup (Modules.<sanitized> / Scripts.<sanitized>)
            var legacyModuleId = ModuleName.GetModuleIdFromSpecifier(key);
            var moduleTypeNameCandidates = new[]
            {
                $"Modules.{legacyModuleId}",
                $"Scripts.{legacyModuleId}",
            };

            string? resolvedTypeName = null;
            foreach (var candidate in moduleTypeNameCandidates)
            {
                var t = _localModulesAssembly.GetType(candidate);
                if (t != null)
                {
                    resolvedTypeName = candidate;
                    break;
                }
            }

            if (resolvedTypeName == null)
                throw new ReferenceError($"Cannot find local module type '{moduleTypeNameCandidates[0]}' (or legacy '{moduleTypeNameCandidates[1]}') in assembly");

            // Legacy path: treat the legacyModuleId-derived key as canonical for relative resolution.
            return RequireCompiledModule(legacyModuleId, resolvedTypeName, requestKey: key);
        }

        private object? RequireCompiledModule(string canonicalId, string typeName, string requestKey)
        {
            if (_localModulesAssembly == null)
                throw new ReferenceError($"Cannot require compiled module '{requestKey}': no local modules assembly provided");

            var cacheKey = "compiled:" + canonicalId;
            if (_instances.TryGetValue(cacheKey, out var existing))
            {
                if (_modules.TryGetValue(cacheKey, out var existingModule))
                {
                    if (_currentParentModule != null)
                    {
                        _currentParentModule.AddChild(existingModule);
                    }

                    return existingModule.exports;
                }

                return existing;
            }

            var localType = _localModulesAssembly.GetType(typeName);
            if (localType == null)
            {
                throw new ReferenceError($"Cannot find compiled module type '{typeName}' in assembly");
            }

            var parentModule = _currentParentModule;

            RequireDelegate moduleRequire = (moduleIdParam) =>
            {
                if (moduleIdParam is not string requestedSpecifier || requestedSpecifier == null)
                {
                    throw new TypeError("The \"id\" argument must be of type string.");
                }

                var resolved = ResolveLocalSpecifier(canonicalId, requestedSpecifier);
                return RequireModule(resolved);
            };

            var dirName = GetDirectoryNameForwardSlash(canonicalId);
            var module = new Module(canonicalId, canonicalId, parentModule, moduleRequire);
            _modules[cacheKey] = module;
            _instances[cacheKey] = module.exports ?? new object();

            if (parentModule != null)
            {
                parentModule.AddChild(module);
            }

            var entryPointCandidates = new[] { "__js_module_init__", "Main" };
            MethodInfo? moduleEntryPoint = null;
            foreach (var candidate in entryPointCandidates)
            {
                moduleEntryPoint = localType.GetMethod(candidate, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (moduleEntryPoint != null)
                    break;
            }

            if (moduleEntryPoint == null)
                throw new TypeError($"Compiled module '{typeName}' does not have a static __js_module_init__ (or legacy Main) method");

            var moduleDelegate = (ModuleMainDelegate)Delegate.CreateDelegate(typeof(ModuleMainDelegate), moduleEntryPoint);

            _currentParentModule = module;
            try
            {
                moduleDelegate(module.exports, moduleRequire, module, canonicalId, dirName);
            }
            finally
            {
                _currentParentModule = parentModule;
                module.MarkLoaded();
            }

            _instances[cacheKey] = module.exports!;
            return module.exports;
        }

        private static string NormalizeModuleIdKey(string keyOrId)
        {
            var s = Normalize(keyOrId);

            // Local module IDs in the manifest do not include a leading './'.
            if (s.StartsWith("./", StringComparison.Ordinal))
            {
                s = s.Substring(2);
            }

            if (s.StartsWith("/", StringComparison.Ordinal))
            {
                s = s.Substring(1);
            }

            if (s.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
            {
                s = s.Substring(0, s.Length - 3);
            }

            return s;
        }

        private static bool TryResolveFromMap(Dictionary<string, (string CanonicalId, string TypeName)> map, string moduleIdKey, out string canonicalId, out string typeName)
        {
            canonicalId = string.Empty;
            typeName = string.Empty;

            // Minimal probing at the module-id level (not filesystem):
            //   id, id/index
            // Note: map keys are stored without ".js" extension.
            var candidates = new[] { moduleIdKey, moduleIdKey + "/index" };

            foreach (var c in candidates)
            {
                if (map.TryGetValue(c, out var mapped))
                {
                    canonicalId = mapped.CanonicalId;
                    typeName = mapped.TypeName;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Requires a Node.js built-in module.
        /// </summary>
        private object? RequireNodeModule(string key, string specifier)
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
