using System;
using JavaScriptRuntime.Modules.Shared;

namespace JavaScriptRuntime.Modules.ESM
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
            => Import(specifier, currentModuleId, null, "None");

        public static Promise Import(object? specifier, object? currentModuleId, object? options, string phase)
        {
            try
            {
                var specifierStr = DotNet2JSConversions.ToStringRejectingSymbols(specifier);
                string? type = null;
                if (options != null)
                {
                    if (TypeUtilities.IsPrimitive(options))
                        throw new TypeError("import() options must be an object");
                    var attributes = ObjectRuntime.GetProperty(options, "with");
                    if (attributes != null)
                    {
                        if (TypeUtilities.IsPrimitive(attributes))
                            throw new TypeError("import() attributes must be an object");
                        foreach (var key in ObjectRuntime.GetOwnPropertyKeysInOrder(attributes))
                        {
                            if (!ObjectRuntime.IsOwnPropertyPresentAndEnumerable(attributes, key))
                                continue;
                            if (ObjectRuntime.GetProperty(attributes, key) is not string attributeValue)
                                throw new TypeError("import() attribute values must be strings");
                            if (key == "type")
                                type = attributeValue;
                        }
                    }
                }

                // Resolve require delegate by current module id/filename.
                var requireDelegate = RuntimeServices.GetRequireForModule(currentModuleId as string);
                
                if (requireDelegate == null)
                {
                    return (Promise)Promise.reject(new ReferenceError("import() requires a CommonJS module context"))!;
                }

                return (Promise)((Promise)Promise.resolve(null)!).then(
                    new BuiltinFunction1((_, _) =>
                    {
                        if (phase != "None" || type is not null)
                            return new CommonJS.Require(RuntimeExecutionContext.Current!.Realm.ModuleState)
                                .ImportWithPhase((string)currentModuleId!, specifierStr, phase, type);
                        var exports = requireDelegate(specifierStr);
                        return EsModuleInterop.ToDynamicImportResult(exports);
                    }))!;
            }
            catch (Exception ex) when (ex is not ScriptProcessExitException)
            {
                return (Promise)Promise.reject(Promise.GetThrownJsValue(ex))!;
            }
        }
    }
}
