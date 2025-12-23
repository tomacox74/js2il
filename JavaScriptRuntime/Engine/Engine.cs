using System.Diagnostics.CodeAnalysis;
using JavaScriptRuntime.DependencyInjection;
using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime;

/// <summary>
/// Entry point for executing JavaScript code that has been compiled to a dotnet assembly.
/// </summary>
public class Engine
{
    /// <summary>
    /// Only used for testing purposes to override the default service provider.
    /// </summary>
    internal readonly static ThreadLocal<ServiceContainer?> _serviceProviderOverride = new(() => null);

    public void Execute([NotNull] CommonJS.ModuleMainDelegate scriptEntryPoint)
    {
        try 
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(scriptEntryPoint));

            var serviceProvider = _serviceProviderOverride.Value ?? RuntimeServices.BuildServiceProvider();
            
            var ctx = serviceProvider.Resolve<EngineCore.NodeSychronizationContext>();
            SynchronizationContext.SetSynchronizationContext(ctx);

            serviceProvider.RegisterInstance<IMicrotaskScheduler>(ctx);
            serviceProvider.RegisterInstance<IScheduler>(ctx);

            GlobalThis.ServiceProvider = serviceProvider;;

            // use for lookup of dependencies
            serviceProvider.Resolve<LocalModulesAssembly>().ModulesAssembly = scriptEntryPoint.Method.Module.Assembly;

            var moduleContext = CommonJS.ModuleContext.CreateModuleContext(serviceProvider);

            // Invoke script with module parameters (all null for now)
            // Parameters: exports, require, module, __filename, __dirname
            scriptEntryPoint(moduleContext.Exports, moduleContext.require, null, moduleContext.__filename, moduleContext.__dirname);    

            while (ctx.HasPendingWork())
            {
                ctx.RunOneIteration();
                ctx.WaitForWorkOrNextTimer();
            }
        }
        finally
        {
            // to do.. change globalthis to be a instance
            GlobalThis.ServiceProvider = null;        
            _serviceProviderOverride.Value = null;
        }
    }
}