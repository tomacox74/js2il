using System;

namespace JavaScriptRuntime.CommonJS
{
    /// <summary>
    /// Runtime support for dynamic import() expressions per ECMA-262 §13.3.10.
    /// </summary>
    public static class DynamicImport
    {
        /// <summary>
        /// Implements dynamic import() by loading a module and returning a Promise.
        /// </summary>
        /// <param name="specifier">The module specifier (string).</param>
        /// <param name="currentModuleId">The ID of the current module for relative path resolution.</param>
        /// <returns>A Promise that resolves to the module's exports or rejects on error.</returns>
        public static Promise Import(object? specifier, object? currentModuleId)
        {
            if (specifier is not string specifierStr)
            {
                return (Promise)Promise.reject(new TypeError("import() requires a string specifier"))!;
            }

            try
            {
                // Resolve require delegate by current module id/filename.
                var requireDelegate = RuntimeServices.GetRequireForModule(currentModuleId as string);
                
                if (requireDelegate == null)
                {
                    return (Promise)Promise.reject(new ReferenceError("import() requires a CommonJS module context"))!;
                }

                // Use the existing require mechanism to load the module synchronously
                // In a real async implementation, this would be async, but for now we
                // maintain compatibility with the existing synchronous module loader
                var exports = requireDelegate(specifierStr);
                
                return (Promise)Promise.resolve(exports)!;
            }
            catch (Exception ex)
            {
                return (Promise)Promise.reject(ex)!;
            }
        }
    }
}
