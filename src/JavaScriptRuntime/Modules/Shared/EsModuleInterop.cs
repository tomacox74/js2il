namespace JavaScriptRuntime.Modules.Shared
{
    internal static class EsModuleInterop
    {
        private const string EsModuleProperty = "__esModule";

        private static RuntimeModuleState State
            => RuntimeExecutionContext.Current?.Realm.ModuleState
                ?? throw new InvalidOperationException(
                    "ES module interop requires an active JavaScript runtime.");

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

            var namespaceObject = new JsObject();
            DefineNamespaceGetter(namespaceObject, "default", () => exports);
            DefineNamespaceGetter(namespaceObject, "module.exports", () => exports);

            foreach (var key in JavaScriptRuntime.ObjectRuntime.GetEnumerableKeys(exports!))
            {
                var keyName = DotNet2JSConversions.ToString(key);
                if (string.IsNullOrWhiteSpace(keyName) || IsReservedNamespaceKey(keyName))
                {
                    continue;
                }

                var capturedKey = keyName;
                DefineNamespaceGetter(namespaceObject, capturedKey, () => JavaScriptRuntime.ObjectRuntime.GetProperty(exports!, capturedKey));
            }

            return CacheNamespace(exports!, namespaceObject);
        }

        private static bool IsEsModuleObject(object? exports)
        {
            if (exports is null || exports is JsNull)
            {
                return false;
            }

            return ObjectRuntime.hasOwn(exports, EsModuleProperty)
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
            return State.CommonJsNamespaceCache.TryGetValue(exports, out namespaceObject);
        }

        private static object CacheNamespace(object exports, object namespaceObject)
        {
            return State.CommonJsNamespaceCache.GetValue(exports, _ => namespaceObject);
        }

        private static object CreatePrimitiveNamespace(object? exports)
        {
            var namespaceObject = new JsObject();
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
                || key == EsModuleProperty;
        }
    }
}
