using JavaScriptRuntime.DependencyInjection;
using System;

namespace JavaScriptRuntime;

public class RuntimeServices
{
    private static readonly System.Threading.AsyncLocal<object?> _currentThis = new();
    private static readonly System.Threading.AsyncLocal<object?[]?> _currentArguments = new();

    public static object? GetCurrentThis()
    {
        return _currentThis.Value;
    }

    public static object? SetCurrentThis(object? value)
    {
        var previous = _currentThis.Value;
        _currentThis.Value = value;
        return previous;
    }

    public static object?[]? GetCurrentArguments()
    {
        return _currentArguments.Value;
    }

    public static object?[]? SetCurrentArguments(object?[]? value)
    {
        var previous = _currentArguments.Value;
        _currentArguments.Value = value;
        return previous;
    }

    /// <summary>
    /// Materializes the implicit non-arrow function `arguments` object for the current call.
    /// This captures the full runtime argument list (including extra args beyond formal parameters).
    /// </summary>
    public static Array CreateArgumentsObject()
    {
        var args = _currentArguments.Value;
        if (args == null || args.Length == 0)
        {
            return new Array();
        }

        // Copy into a JS Array so later invocations cannot mutate our ambient args.
        return new Array(args);
    }

    public static ServiceContainer BuildServiceProvider()
    {
        var container = new ServiceContainer();
        
        // Register default engine dependencies
        container.Register<EngineCore.ITickSource, EngineCore.TickSource>();
        container.Register<EngineCore.IWaitHandle, EngineCore.WaitHandle>();
        container.Register<EngineCore.NodeSchedulerState>();
        container.Register<EngineCore.NodeEventLoopPump>();
        container.Register<EngineCore.IMicrotaskScheduler, EngineCore.NodeSchedulerState>();
        container.Register<EngineCore.IScheduler, EngineCore.NodeSchedulerState>();
        container.Register<CommonJS.Require>();
        container.Register<LocalModulesAssembly>();
        container.Register<IEnvironment, DefaultEnvironment>();
        
        return container;
    }
}