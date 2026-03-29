namespace JavaScriptRuntime;

internal sealed class RuntimeExecutionContext
{
    internal RuntimeExecutionContext(bool isHosted, string? compiledAssemblyPath = null)
    {
        IsHosted = isHosted;
        CompiledAssemblyPath = compiledAssemblyPath;
    }

    internal bool IsHosted { get; }

    internal string? CompiledAssemblyPath { get; }
}
