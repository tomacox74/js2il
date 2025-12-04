using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.Utilities.Ecma335
{
    /// <summary>
    /// Registry for assembly references in a MetadataBuilder.
    /// Caches AssemblyReferenceHandles to avoid duplicate references.
    /// Supports System.* BCL assemblies and JavaScriptRuntime assembly.
    /// </summary>
    internal sealed class AssemblyReferenceRegistry
    {
        private readonly MetadataBuilder _metadataBuilder;
        private readonly Dictionary<string, AssemblyReferenceHandle> _cache = new(StringComparer.Ordinal);

        public AssemblyReferenceRegistry(MetadataBuilder metadataBuilder)
        {
            _metadataBuilder = metadataBuilder ?? throw new ArgumentNullException(nameof(metadataBuilder));
        }

        /// <summary>
        /// Gets or creates an AssemblyReferenceHandle for the specified assembly name.
        /// If the assembly is not yet cached, adds it to the metadata builder and caches the handle.
        /// </summary>
        /// <param name="assemblyName">The simple assembly name (e.g., "System.Runtime", "JavaScriptRuntime").</param>
        /// <returns>The AssemblyReferenceHandle for the assembly.</returns>
        public AssemblyReferenceHandle GetOrAdd(string assemblyName)
        {
            if (string.IsNullOrWhiteSpace(assemblyName))
            {
                throw new ArgumentException("Assembly name cannot be null or whitespace.", nameof(assemblyName));
            }

            if (_cache.TryGetValue(assemblyName, out var cached))
            {
                return cached;
            }

            // Resolve version and public key token from the runtime
            var version = ResolveAssemblyVersion(assemblyName);
            var publicKeyToken = ResolvePublicKeyToken(assemblyName);

            var handle = _metadataBuilder.AddAssemblyReference(
                name: _metadataBuilder.GetOrAddString(assemblyName),
                version: version,
                culture: default,
                publicKeyOrToken: publicKeyToken,
                flags: 0,
                hashValue: default
            );

            _cache[assemblyName] = handle;
            return handle;
        }

        /// <summary>
        /// Resolves the version for a known assembly name.
        /// For System.* assemblies, uses the BCL version from the current runtime.
        /// For JavaScriptRuntime, uses its assembly version.
        /// </summary>
        private Version ResolveAssemblyVersion(string assemblyName)
        {
            if (assemblyName.StartsWith("System.", StringComparison.Ordinal))
            {
                // Use the BCL version from the current runtime
                return typeof(object).Assembly.GetName().Version ?? new Version(1, 0, 0, 0);
            }

            if (assemblyName.Equals("JavaScriptRuntime", StringComparison.Ordinal))
            {
                // Use JavaScriptRuntime's version
                var jsRuntimeType = Type.GetType("JavaScriptRuntime.Console, JavaScriptRuntime");
                if (jsRuntimeType != null)
                {
                    return jsRuntimeType.Assembly.GetName().Version ?? new Version(1, 0, 0, 0);
                }
            }

            // Default version for unknown assemblies
            return new Version(1, 0, 0, 0);
        }

        /// <summary>
        /// Resolves the public key token for a known assembly name.
        /// For System.* assemblies, uses the BCL public key token.
        /// For JavaScriptRuntime, returns default (no strong name by default).
        /// </summary>
        private BlobHandle ResolvePublicKeyToken(string assemblyName)
        {
            if (assemblyName.StartsWith("System.", StringComparison.Ordinal))
            {
                // Use the BCL public key token
                var publicKeyToken = typeof(object).Assembly.GetName().GetPublicKeyToken();
                if (publicKeyToken != null && publicKeyToken.Length > 0)
                {
                    return _metadataBuilder.GetOrAddBlob(publicKeyToken);
                }
            }

            // JavaScriptRuntime and unknown assemblies: no public key token
            return default;
        }

        /// <summary>
        /// Clears the cache. Useful for testing or when starting a new compilation unit.
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Gets the number of cached assembly references.
        /// </summary>
        public int Count => _cache.Count;
    }
}
