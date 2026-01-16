using JavaScriptRuntime.DependencyInjection;
using System;

namespace JavaScriptRuntime;

public class RuntimeServices
{
    [ThreadStatic]
    private static object? _currentThis;

    public static object? GetCurrentThis()
    {
        return _currentThis;
    }

    public static object? SetCurrentThis(object? value)
    {
        var previous = _currentThis;
        _currentThis = value;
        return previous;
    }

    public static ServiceContainer BuildServiceProvider()
    {
        var container = new ServiceContainer();
        
        // Register default engine dependencies
        container.Register<EngineCore.ITickSource, EngineCore.TickSource>();
        container.Register<EngineCore.IWaitHandle, EngineCore.WaitHandle>();
        container.Register<EngineCore.NodeSychronizationContext>();
        container.Register<CommonJS.Require>();
        container.Register<LocalModulesAssembly>();
        container.Register<IEnvironment, DefaultEnvironment>();
        
        return container;
    }
}