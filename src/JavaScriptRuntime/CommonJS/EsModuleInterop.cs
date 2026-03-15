using System.Collections.Generic;
using System.Dynamic;

namespace JavaScriptRuntime.CommonJS
{
    internal static class EsModuleInterop
    {
        private const string EsModuleProperty = "__esModule";
        private const string NamespaceCacheProperty = "__js2il_esm_namespace";

        public static object ToDynamicImportResult(object? exports)
        {
            if (IsEsModuleObject(exports))
            {
                return exports!;
            }

            if (!IsNamespaceObjectCandidate(exports))
            {
                return CreatePrimitiveNamespace(exports);
            }

            if (TryGetCachedNamespace(exports!, out var cachedNamespace))
            {
                return cachedNamespace!;
            }

            var namespaceObject = new ExpandoObject();
            DefineNamespaceGetter(namespaceObject, "default", () => exports);
            DefineNamespaceGetter(namespaceObject, "module.exports", () => exports);

            foreach (var key in JavaScriptRuntime.Object.GetEnumerableKeys(exports!))
            {
                var keyName = DotNet2JSConversions.ToString(key);
                if (string.IsNullOrWhiteSpace(keyName) || IsReservedNamespaceKey(keyName))
                {
                    continue;
                }

                var capturedKey = keyName;
                DefineNamespaceGetter(namespaceObject, capturedKey, () => JavaScriptRuntime.ObjectRuntime.GetProperty(exports!, capturedKey));
            }

            CacheNamespace(exports!, namespaceObject);
            return namespaceObject;
        }

        private static bool IsEsModuleObject(object? exports)
        {
            if (exports is null || exports is JsNull)
            {
                return false;
            }

            return JavaScriptRuntime.Object.hasOwn(exports, EsModuleProperty)
                && JavaScriptRuntime.ObjectRuntime.GetProperty(exports, EsModuleProperty) is bool isEsModule
                && isEsModule;
        }

        private static bool IsNamespaceObjectCandidate(object? exports)
        {
            if (exports is null || exports is JsNull)
            {
                return false;
            }

            if (exports is string)
            {
                return false;
            }

            return !exports.GetType().IsValueType;
        }

        private static bool TryGetCachedNamespace(object exports, out object? namespaceObject)
        {
            namespaceObject = null;
            if (!JavaScriptRuntime.Object.hasOwn(exports, NamespaceCacheProperty))
            {
                return false;
            }

            namespaceObject = JavaScriptRuntime.ObjectRuntime.GetProperty(exports, NamespaceCacheProperty);
            return namespaceObject is not null && namespaceObject is not JsNull;
        }

        private static void CacheNamespace(object exports, object namespaceObject)
        {
            if (!JavaScriptRuntime.Object.isExtensible(exports)
                || JavaScriptRuntime.Object.hasOwn(exports, NamespaceCacheProperty))
            {
                return;
            }

            JavaScriptRuntime.Object.defineProperty(
                exports,
                NamespaceCacheProperty,
                CreateDataDescriptor(namespaceObject, enumerable: false, configurable: false, writable: false));
        }

        private static object CreatePrimitiveNamespace(object? exports)
        {
            var namespaceObject = new ExpandoObject();
            JavaScriptRuntime.ObjectRuntime.SetProperty(namespaceObject, "default", exports);
            JavaScriptRuntime.ObjectRuntime.SetProperty(namespaceObject, "module.exports", exports);
            return namespaceObject;
        }

        private static void DefineNamespaceGetter(object target, string name, Func<object?> getter)
        {
            JavaScriptRuntime.ObjectRuntime.DefineObjectLiteralAccessorProperty(target, name, getter, null);
        }

        private static bool IsReservedNamespaceKey(string key)
        {
            return key == "default"
                || key == "module.exports"
                || key == EsModuleProperty
                || key == NamespaceCacheProperty;
        }

        private static object CreateDataDescriptor(object? value, bool enumerable, bool configurable, bool writable)
        {
            var descriptor = new ExpandoObject();
            var properties = (IDictionary<string, object?>)descriptor;
            properties["value"] = value;
            properties["enumerable"] = enumerable;
            properties["configurable"] = configurable;
            properties["writable"] = writable;
            return descriptor;
        }
    }
}
