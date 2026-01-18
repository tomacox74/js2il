using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

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
        
        // Track the current parent module for establishing parent-child relationships
        private Module? _currentParentModule;

        public Require(LocalModulesAssembly localModulesAssembly)
        {
            // Preload local modules from the provided assembly
            _localModulesAssembly = localModulesAssembly.ModulesAssembly;
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
            var isLocalModule = key.StartsWith("./") || key.StartsWith("../") || key.StartsWith("/");

            if (_instances.TryGetValue(key, out var existing))
            {
                // Track parent-child relationship for already-loaded modules
                if (_currentParentModule != null && _modules.TryGetValue(key, out var existingModule))
                {
                    _currentParentModule.AddChild(existingModule);
                }
                return existing;
            }

            if (isLocalModule)
            {
                return RequireLocalModule(key);
            }
            else
            {
                return RequireNodeModule(key, specifier);
            }
        }

        /// <summary>
        /// Requires a local module (user code compiled with the main script).
        /// </summary>
        private object? RequireLocalModule(string key)
        {
            if (_localModulesAssembly == null)
                throw new ReferenceError($"Cannot require local module '{key}': no local modules assembly provided");
            
            var moduleId = ModuleName.GetModuleIdFromSpecifier(key);
            var moduleTypeNameCandidates = new[]
            {
                $"Modules.{moduleId}",
                $"Scripts.{moduleId}",
            };

            Type? localType = null;
            string? resolvedTypeName = null;
            foreach (var candidate in moduleTypeNameCandidates)
            {
                localType = _localModulesAssembly.GetType(candidate);
                if (localType != null)
                {
                    resolvedTypeName = candidate;
                    break;
                }
            }

            if (localType == null)
                throw new ReferenceError($"Cannot find local module type '{moduleTypeNameCandidates[0]}' (or legacy '{moduleTypeNameCandidates[1]}') in assembly");

            // Store current parent before we change it
            var parentModule = _currentParentModule;

            // Create a per-module require() delegate that resolves relative specifiers
            // against this module's own path.
            RequireDelegate moduleRequire = (moduleIdParam) => 
            {
                if (moduleIdParam is not string requestedSpecifier || requestedSpecifier == null)
                {
                    throw new TypeError("The \"id\" argument must be of type string.");
                }

                var resolved = ResolveLocalSpecifier(key, requestedSpecifier);
                return RequireModule(resolved);
            };

            var dirName = GetDirectoryNameForwardSlash(key);
            
            // Create the Module object for this module
            var module = new Module(key, key, parentModule, moduleRequire);
            _modules[key] = module;

            // Cache exports before executing (for circular dependency support)
            // The exports object is shared between `exports` param and `module.exports`
            // Note: exports starts as ExpandoObject, so this should never be null initially,
            // but we handle null defensively in case module.exports is set to null by user code.
            _instances[key] = module.exports ?? new object();

            // Track parent-child relationship
            if (parentModule != null)
            {
                parentModule.AddChild(module);
            }

            // Method is a static member. Prefer the current compiler output, but allow legacy names.
            var entryPointCandidates = new[]
            {
                "__js_module_init__",
                "Main",
            };

            MethodInfo? moduleEntryPoint = null;
            foreach (var candidate in entryPointCandidates)
            {
                moduleEntryPoint = localType.GetMethod(candidate, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (moduleEntryPoint != null)
                    break;
            }

            if (moduleEntryPoint == null)
                throw new TypeError($"Local module '{resolvedTypeName ?? moduleId}' does not have a static __js_module_init__ (or legacy Main) method");

            var moduleDelegate = (ModuleMainDelegate)Delegate.CreateDelegate(
                typeof(ModuleMainDelegate), moduleEntryPoint);

            // Set this module as the current parent for any requires within
            _currentParentModule = module;
            
            try
            {
                // Invoke module with `exports` parameter initially pointing to module.exports.
                // IMPORTANT: The `exports` parameter is the initial module.exports value only.
                // If the module body later reassigns module.exports, the `exports` parameter
                // will not be updated and may diverge from module.exports. This matches
                // Node.js CommonJS semantics, where module.exports is the authoritative value
                // used for caching and for the return of require().
                moduleDelegate(module.exports, moduleRequire, module, key, dirName);
            }
            finally
            {
                // Restore parent and mark module as loaded
                _currentParentModule = parentModule;
                module.MarkLoaded();
            }

            // Return module.exports (which may have been reassigned during execution)
            // Update cache with final exports value
            _instances[key] = module.exports!;
            return module.exports;
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
