namespace JavaScriptRuntime;

internal sealed class RuntimeExecutionContext
{
    internal RuntimeExecutionContext(bool isHosted)
    {
        IsHosted = isHosted;
    }

    internal bool IsHosted { get; }
}
