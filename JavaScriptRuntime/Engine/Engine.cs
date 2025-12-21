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
/// Entry point for executing JavaScript code that has been compiled to a dotnet assembly.
/// </summary>
public class Engine
{
    public void Execute(CommonJS.ModuleMainDelegate scriptEntryPoint)
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
        GlobalThis.MicrotaskScheduler = ctx;

        var moduleContext = CommonJS.ModuleContext.CreateModuleContext();

        // Invoke script with module parameters (all null for now)
        // Parameters: exports, require, module, __filename, __dirname
        scriptEntryPoint(moduleContext.Exports, moduleContext.require, null, moduleContext.__filename, moduleContext.__dirname);    

        while (ctx.HasPendingWork())
        {
            ctx.RunOneIteration();
            ctx.WaitForWorkOrNextTimer();
        }
    }
}