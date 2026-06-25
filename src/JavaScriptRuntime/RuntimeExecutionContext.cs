namespace JavaScriptRuntime;

using JavaScriptRuntime.CommonJS;

internal sealed class RuntimeExecutionContext
{
    private readonly List<KeyValuePair<string, RequireDelegate>> _registeredModuleRequires = new();

    internal RuntimeExecutionContext(bool isHosted, string? compiledAssemblyPath = null)
    {
        IsHosted = isHosted;
        CompiledAssemblyPath = compiledAssemblyPath;
    }

    internal bool IsHosted { get; }

    internal string? CompiledAssemblyPath { get; }

    internal IReadOnlyList<KeyValuePair<string, RequireDelegate>> RegisteredModuleRequires => _registeredModuleRequires;

    internal void TrackModuleRequire(string moduleId, RequireDelegate require)
    {
        _registeredModuleRequires.Add(new KeyValuePair<string, RequireDelegate>(moduleId, require));
    }
}
