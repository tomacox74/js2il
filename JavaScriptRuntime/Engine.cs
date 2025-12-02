namespace JavaScriptRuntime;

/// <summary>
/// Entry point for executiing JavaScript code that has been compiled to a dotnet assembly.
/// </summary>
public class Engine
{
    public void Execute(Action scriptEntryPoint)
    {
        scriptEntryPoint();
    }
}