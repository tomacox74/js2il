using JavaScriptRuntime.Node;

namespace Js2IL.Runtime;

public sealed class JsModuleLoadOptions
{
    /// <summary>
    /// Launchable compiled assembly path used by hosted <c>child_process.fork()</c>.
    /// Hosted runtimes do not infer this automatically; set it explicitly when hosted code may call <c>fork()</c>.
    /// </summary>
    public string? CompiledAssemblyPath { get; init; }

    /// <summary>
    /// Optional host-controlled process launcher used for hosted <c>child_process.fork()</c> calls.
    /// </summary>
    public IChildProcessLauncher? ChildProcessLauncher { get; init; }
}
