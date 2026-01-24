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

            // Map System.Private.CoreLib to System.Runtime (reference assembly)
            if (assemblyName == "System.Private.CoreLib")
            {
                // the mapping may have to be expanded to be per type, not per assembly
                // and as such probably should be moved to TypeReferenceRegistry
                assemblyName = "System.Runtime";
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
                // Prefer the version from the actual assembly being referenced.
                // Using System.Private.CoreLib's version works for many cases, but it can cause
                // mismatches when consumers reference the generated assembly at compile-time.
                try
                {
                    var bclAsm = Assembly.Load(new AssemblyName(assemblyName));
                    return bclAsm.GetName().Version ?? new Version(1, 0, 0, 0);
                }
                catch
                {
                    // Fallback: Use the BCL version from the current runtime.
                    return typeof(object).Assembly.GetName().Version ?? new Version(1, 0, 0, 0);
                }
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
                // IMPORTANT:
                // JS2IL emits assemblies intended to be referenced by normal SDK-style projects.
                // Those projects compile against reference assemblies in Microsoft.NETCore.App.Ref,
                // where System.* assemblies are strong-named with the standard Microsoft token
                // (b03f5f7f11d50a3a). Using the runtime token (e.g. System.Private.CoreLib's 7cec...)
                // causes CS0012 when the generated assembly is referenced at compile-time.
                var bclToken = new byte[] { 0xB0, 0x3F, 0x5F, 0x7F, 0x11, 0xD5, 0x0A, 0x3A };
                return _metadataBuilder.GetOrAddBlob(bclToken);
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
