using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using JavaScriptRuntime.Modules.CommonJS;
using JavaScriptRuntime.Modules.ESM;
using Assembly = System.Reflection.Assembly;

namespace JavaScriptRuntime.Modules;

internal sealed class RuntimeModuleState : IDisposable
{
    internal Dictionary<string, Type> NodeModuleRegistry { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    internal Dictionary<string, object> Instances { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    internal Dictionary<string, Module> CommonJsModules { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    internal HashSet<string> MissingNodeModules { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    internal ConcurrentDictionary<string, JsObject> ImportMetaByUrl { get; } =
        new(StringComparer.Ordinal);

    internal ConcurrentDictionary<string, RequireDelegate> RequireByModuleId { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    internal ConcurrentDictionary<string, ConcurrentDictionary<string, EsModuleBinding>> EsModuleBindings { get; } =
        new(StringComparer.Ordinal);

    internal ConditionalWeakTable<object, ModuleNamespaceMarker> EsModuleNamespaces { get; } = new();

    internal ConditionalWeakTable<object, object> CommonJsNamespaceCache { get; } = new();

    internal Assembly? ModulesAssembly { get; set; }

    internal Dictionary<string, (string CanonicalId, string TypeName)>?
        CompiledModuleTypeMap
    {
        get;
        set;
    }

    internal Module? MainModule { get; set; }

    public void Dispose()
    {
        NodeModuleRegistry.Clear();
        Instances.Clear();
        CommonJsModules.Clear();
        MissingNodeModules.Clear();
        ImportMetaByUrl.Clear();
        RequireByModuleId.Clear();
        EsModuleBindings.Clear();
        EsModuleNamespaces.Clear();
        CommonJsNamespaceCache.Clear();
        ModulesAssembly = null;
        CompiledModuleTypeMap = null;
        MainModule = null;
    }
}
