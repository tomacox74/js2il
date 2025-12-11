namespace JavaScriptRuntime;

/// <summary>
/// Configuration options for the JavaScript engine.
/// Allows mocks to be substituted for testing.
/// </summary>
public sealed class EngineOptions
{
    public required EngineCore.ITickSource TickSource { get; set; }
    public required EngineCore.IWaitHandle WaitHandle { get; set; }

    public static Func<EngineOptions>? DefaultOptionsProvider { get; set; } = GetDefaultOptions;

    public static EngineOptions GetDefaultOptions()
    {
        return new EngineOptions
        {
            TickSource = new EngineCore.TickSource(),
            WaitHandle = new EngineCore.WaitHandle(),
        };
    }
}

/// <summary>
/// Entry point for executiing JavaScript code that has been compiled to a dotnet assembly.
/// </summary>
public class Engine
{
    public void Execute(Action scriptEntryPoint)
    {
        var engineOptions = EngineOptions.DefaultOptionsProvider?.Invoke();
        if (engineOptions == null)
        {
            throw new InvalidOperationException("No default engine options has been configured.");
        }

        var ctx = new EngineCore.NodeSychronizationContext(engineOptions.TickSource, engineOptions.WaitHandle);
        SynchronizationContext.SetSynchronizationContext(ctx);

        // switch to dependency injection in the future
        GlobalThis.Scheduler = ctx;

        scriptEntryPoint();

        while (ctx.HasPendingWork())
        {
            ctx.RunOneIteration();
            ctx.WaitForWorkOrNextTimer();
        }
    }
}