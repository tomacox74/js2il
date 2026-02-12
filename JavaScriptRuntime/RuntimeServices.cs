using JavaScriptRuntime.DependencyInjection;
using System;

namespace JavaScriptRuntime;

[IntrinsicObject("RuntimeServices")]
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

    /// <summary>
    /// Gets the count of arguments passed to the current function.
    /// Used for rest parameter initialization.
    /// </summary>
    public static int GetArgumentCount()
    {
        var args = _currentArguments.Value;
        return args?.Length ?? 0;
    }

    /// <summary>
    /// Collects rest arguments starting from the specified index into an array.
    /// Used for rest parameter (...args) initialization.
    /// </summary>
    public static object CollectRestArguments(object startIndexObj)
    {
        // Convert to int using JavaScript number conversion
        int startIndex = startIndexObj switch
        {
            int i => i,
            double d => (int)d,
            _ => 0
        };
        
        var args = _currentArguments.Value;
        
        if (args == null || startIndex >= args.Length)
        {
            return new Array();
        }

        // Collect arguments from startIndex to end
        var restArgs = new object?[args.Length - startIndex];
        System.Array.Copy(args, startIndex, restArgs, 0, restArgs.Length);
        return new Array(restArgs);
    }

    /// <summary>
    /// Creates the backing object for a JavaScript object literal.
    /// Kept in the runtime so generated IL can avoid directly referencing BCL dynamic types.
    /// </summary>
    public static object CreateObjectLiteral()
    {
        return new System.Dynamic.ExpandoObject();
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