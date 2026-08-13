using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using JavaScriptRuntime.Modules.Shared;

namespace JavaScriptRuntime.Modules.ESM
{
    internal sealed class EsModuleLinkerState
    {
        internal ConcurrentDictionary<string, ConcurrentDictionary<string, EsModuleBinding>> BindingsByModule { get; } = new();

        internal ConditionalWeakTable<object, ModuleNamespaceMarker> ModuleNamespaces { get; } = new();
    }

    internal sealed class ModuleNamespaceMarker
    {
    }

    /// <summary>
    /// A single ECMAScript module binding cell.
    /// Models a module Environment Record binding: it holds the current value and tracks
    /// whether the binding has been initialized so that reads during the temporal dead zone
    /// (before the declaration is evaluated) throw a <see cref="ReferenceError"/>.
    /// </summary>
    /// <remarks>
    /// Live bindings are implemented by routing exported name reads through the same cell that
    /// the exporting module writes to. This replaces the previous implementation which injected
    /// JavaScript getter closures per export.
    /// </remarks>
    public sealed class EsModuleBinding
    {
        private object? _value;
        private bool _initialized;

        public EsModuleBinding(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public bool Initialized => _initialized;

        /// <summary>
        /// Reads the current binding value, throwing a <see cref="ReferenceError"/> when the binding
        /// has not yet been initialized (temporal dead zone).
        /// </summary>
        public object? Get()
        {
            if (!_initialized)
            {
                throw new ReferenceError($"Cannot access '{Name}' before initialization");
            }

            return _value;
        }

        /// <summary>
        /// Writes the current binding value, marking the binding as initialized.
        /// </summary>
        public void Set(object? value)
        {
            _value = value;
            _initialized = true;
        }
    }

    /// <summary>
    /// Runtime support for statically-linked ECMAScript modules.
    ///
    /// The compiler lowers static <c>import</c>/<c>export</c> declarations into direct calls on this
    /// class instead of injecting JavaScript helper functions and export getter closures. Each
    /// exported name is backed by an <see cref="EsModuleBinding"/> cell keyed by the exporting
    /// module's canonical id; the module's <c>exports</c> object exposes a live, enumerable accessor
    /// over that cell so both ESM importers and CommonJS <c>require</c> consumers observe live values
    /// and temporal-dead-zone semantics.
    /// </summary>
    public static class EsModuleLinker
    {
        private static EsModuleLinkerState State
            => GlobalThis.ServiceProvider?.Resolve<EsModuleLinkerState>()
               ?? throw new InvalidOperationException(
                   "ES module linking requires an active JavaScript runtime.");

        private static ConcurrentDictionary<string, EsModuleBinding> GetModuleBindings(string moduleId)
            => State.BindingsByModule.GetOrAdd(
                moduleId,
                static _ => new ConcurrentDictionary<string, EsModuleBinding>(StringComparer.Ordinal));

        private static EsModuleBinding GetOrCreateBinding(string moduleId, string name)
            => GetModuleBindings(moduleId).GetOrAdd(name, static n => new EsModuleBinding(n));

        /// <summary>
        /// Marks the module's exports object as an ECMAScript module (adds a non-enumerable
        /// <c>__esModule</c> flag) so namespace/interop consumers treat it with ESM semantics.
        /// </summary>
        public static void MarkEsModule(object? exports)
        {
            if (exports is null || exports is JsNull)
            {
                return;
            }

            if (!ObjectRuntime.hasOwn(exports, "__esModule"))
            {
                var descriptor = new JsObject();
                descriptor.SetValue("value", true);
                descriptor.SetBoolean("enumerable", false);
                descriptor.SetBoolean("configurable", true);
                ObjectRuntime.defineProperty(exports, "__esModule", descriptor);
            }

            State.ModuleNamespaces.GetValue(exports, static _ => new ModuleNamespaceMarker());
        }

        /// <summary>
        /// Reports whether an object is a namespace created by the native ESM linker.
        /// Unlike the user-visible <c>__esModule</c> convention, this cannot be spoofed by
        /// transpiled CommonJS modules.
        /// </summary>
        internal static bool IsModuleNamespace(object value)
        {
            var services = GlobalThis.ServiceProvider;
            return services != null
                && services.TryResolve<EsModuleLinkerState>(out var state)
                && state != null
                && state.ModuleNamespaces.TryGetValue(value, out _);
        }

        /// <summary>
        /// Declares a local export: creates (or reuses) the backing binding cell for <paramref name="name"/>
        /// and installs a live, enumerable accessor on the module's <paramref name="exports"/> object.
        /// The accessor observes temporal-dead-zone semantics until <see cref="SetLocalExport"/> initializes
        /// the binding.
        /// </summary>
        public static void RegisterLocalExport(object? exports, string moduleId, string name)
        {
            if (exports is null || exports is JsNull)
            {
                return;
            }

            var binding = GetOrCreateBinding(moduleId, name);
            Func<object?> getter = binding.Get;
            ObjectRuntime.DefineObjectLiteralAccessorProperty(exports, name, getter, null);
        }

        /// <summary>
        /// Writes the current value of a local export binding. Called by the exporting module whenever the
        /// exported name is assigned (initialization and every subsequent reassignment) so importers observe
        /// live values.
        /// </summary>
        public static object? SetLocalExport(string moduleId, string name, object? value)
        {
            GetOrCreateBinding(moduleId, name).Set(value);
            return value;
        }

        /// <summary>
        /// Registers a re-export of a named import (<c>export { imported as exported }</c>) or an indirect
        /// export (<c>export { imported as exported } from "mod"</c>). Installs a live, enumerable
        /// forwarding accessor on <paramref name="exports"/> that reads <paramref name="importName"/> from
        /// <paramref name="sourceModule"/> on every access, so importers observe the source binding's live
        /// value (including temporal-dead-zone errors).
        /// </summary>
        public static void RegisterReexport(object? exports, string exportName, object? sourceModule, string importName)
        {
            if (exports is null || exports is JsNull)
            {
                return;
            }

            Func<object?> getter = () => GetImport(sourceModule, importName);
            ObjectRuntime.DefineObjectLiteralAccessorProperty(exports, exportName, getter, null);
        }

        /// <summary>
        /// Registers a namespace re-export (<c>export * as ns from "mod"</c>). Installs a live, enumerable
        /// accessor on <paramref name="exports"/> that resolves the (stable) namespace object for
        /// <paramref name="sourceModule"/>. The namespace reference itself is stable while its own
        /// properties remain live.
        /// </summary>
        public static void RegisterNamespaceReexport(object? exports, string exportName, object? sourceModule)
        {
            if (exports is null || exports is JsNull)
            {
                return;
            }

            Func<object?> getter = () => GetNamespace(sourceModule);
            ObjectRuntime.DefineObjectLiteralAccessorProperty(exports, exportName, getter, null);
        }

        /// <summary>
        /// Registers a star re-export (<c>export * from "mod"</c>). Enumerates the source module's own
        /// enumerable string keys and installs a live forwarding accessor for each on
        /// <paramref name="exports"/>. Per spec, <c>default</c> is never star-re-exported, and explicit
        /// local/named exports (already installed on <paramref name="exports"/>) take precedence, so keys
        /// that already exist as own properties are skipped.
        /// </summary>
        public static void RegisterStarReexports(object? exports, object? sourceModule)
        {
            if (exports is null || exports is JsNull || sourceModule is null || sourceModule is JsNull)
            {
                return;
            }

            // Enumerate keys only (no value reads) so temporal-dead-zone bindings on the source module do
            // not throw during linking; the installed accessors read values live on demand.
            foreach (var key in ObjectRuntime.GetOwnEnumerableKeysInOrder(sourceModule))
            {
                if (IsReservedStarExportKey(key) || ObjectRuntime.hasOwn(exports, key))
                {
                    continue;
                }

                var capturedKey = key;
                Func<object?> getter = () => GetImport(sourceModule, capturedKey);
                ObjectRuntime.DefineObjectLiteralAccessorProperty(exports, capturedKey, getter, null);
            }
        }

        private static bool IsReservedStarExportKey(string key)
            => key == "default" || key == "__esModule" || key == "module.exports";

        /// <summary>
        /// Reads a named import from a required module object. This is a live read: for ESM sources the
        /// property is a cell-backed accessor (with TDZ), for CommonJS sources it is an ordinary property.
        /// </summary>
        public static object? GetImport(object? mod, string name)
        {
            if (mod is null || mod is JsNull)
            {
                throw new TypeError($"Cannot read import '{name}' from a null or undefined module namespace");
            }

            return ObjectRuntime.GetProperty(mod, name);
        }

        /// <summary>
        /// Resolves the default import value for a required module, applying CommonJS interop
        /// (a CJS module without an own <c>default</c> export binds its <c>module.exports</c> as default).
        /// </summary>
        public static object? GetDefault(object? mod)
        {
            if (mod is not null && mod is not JsNull && ObjectRuntime.hasOwn(mod, "default"))
            {
                return ObjectRuntime.GetProperty(mod, "default");
            }

            return mod;
        }

        /// <summary>
        /// Resolves the namespace object for <c>import * as ns</c> / <c>export * as ns</c>.
        /// ESM modules expose their exports object directly (live, cell-backed accessors); CommonJS modules
        /// get a synthesized interop namespace.
        /// </summary>
        public static object GetNamespace(object? mod)
        {
            return EsModuleInterop.ToDynamicImportResult(mod);
        }
    }
}
