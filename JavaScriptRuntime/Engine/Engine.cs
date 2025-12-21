using JavaScriptRuntime.DependencyInjection;

namespace JavaScriptRuntime;

/// <summary>
/// Entry point for executing JavaScript code that has been compiled to a dotnet assembly.
/// </summary>
public class Engine
{
    /// <summary>
    /// Only used for testing purposes to override the default service provider.
    /// </summary>
    internal static ServiceContainer? _serviceProviderOverride;

    public void Execute(CommonJS.ModuleMainDelegate scriptEntryPoint)
    {
        var serviceProvider = _serviceProviderOverride ?? RuntimeServices.BuildServiceProvider();
        
        var tickSource = serviceProvider.Resolve<EngineCore.ITickSource>();
        var waitHandle = serviceProvider.Resolve<EngineCore.IWaitHandle>();

        var ctx = serviceProvider.Resolve<EngineCore.NodeSychronizationContext>();
        SynchronizationContext.SetSynchronizationContext(ctx);

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
        
        _serviceProviderOverride = null;
    }
}