using JavaScriptRuntime.DependencyInjection;

namespace JavaScriptRuntime;

public class RuntimeServices
{
    public static ServiceContainer BuildServiceProvider()
    {
        var container = new ServiceContainer();
        
        // Register default engine dependencies
        container.Register<EngineCore.ITickSource, EngineCore.TickSource>();
        container.Register<EngineCore.IWaitHandle, EngineCore.WaitHandle>();
        container.Register<EngineCore.NodeSychronizationContext>();
        container.Register<CommonJS.Require>();
        container.Register<LocalModulesAssembly>();
        
        return container;
    }
}