using JavaScriptRuntime.Node;

namespace Js2IL.Runtime;

public sealed class JsModuleLoadOptions
{
    public string? CompiledAssemblyPath { get; init; }

    public IChildProcessLauncher? ChildProcessLauncher { get; init; }
}
