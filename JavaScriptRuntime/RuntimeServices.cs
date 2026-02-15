using JavaScriptRuntime.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace JavaScriptRuntime;

public class RuntimeServices
{
    private static readonly System.Threading.AsyncLocal<object?> _currentThis = new();
    private static readonly System.Threading.AsyncLocal<object?[]?> _currentArguments = new();
    private static readonly ConcurrentDictionary<string, JavaScriptRuntime.CommonJS.RequireDelegate> _requireByModuleId = new(StringComparer.OrdinalIgnoreCase);

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
    /// Registers a module-scoped require delegate by module id/filename.
    /// Used by dynamic import() to resolve the module loading context.
    /// </summary>
    public static void RegisterModuleRequire(string moduleId, CommonJS.RequireDelegate require)
    {
        if (string.IsNullOrWhiteSpace(moduleId) || require == null)
        {
            return;
        }

        _requireByModuleId[moduleId] = require;
    }

    /// <summary>
    /// Resolves a previously-registered module-scoped require delegate.
    /// </summary>
    public static CommonJS.RequireDelegate? GetRequireForModule(string? moduleId)
    {
        if (string.IsNullOrWhiteSpace(moduleId))
        {
            return null;
        }

        return _requireByModuleId.TryGetValue(moduleId, out var require) ? require : null;
    }

    /// <summary>
    /// Creates the backing object for a JavaScript object literal.
    /// Kept in the runtime so generated IL can avoid directly referencing BCL dynamic types.
    /// </summary>
    public static object CreateObjectLiteral()
    {
        return new System.Dynamic.ExpandoObject();
    }

    /// <summary>
    /// Cache for template objects indexed by call site ID.
    /// Per ECMA-262 spec, each unique call site should return the same template object identity.
    /// </summary>
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Array> _templateObjectCache = new();

    /// <summary>
    /// Creates a template object for tagged template expressions.
    /// Returns a cached instance for the same call site to preserve object identity.
    /// The template object is an array with the cooked strings and a .raw property with raw strings.
    /// </summary>
    /// <param name="callSiteId">Unique identifier for the call site (e.g., "Module:Line:Column")</param>
    /// <param name="cooked">Cooked string array (with escape sequences processed)</param>
    /// <param name="raw">Raw string array (escape sequences not processed)</param>
    public static object CreateTemplateObject(object callSiteIdObj, object cookedObj, object rawObj)
    {
        // Unbox parameters - IL compiler passes all RuntimeServices parameters as object
        var callSiteId = (string)callSiteIdObj;
        var cooked = (object[])cookedObj;
        var raw = (object[])rawObj;
        
        return _templateObjectCache.GetOrAdd(callSiteId, _ =>
        {
            // Create array with cooked strings
            var templateObject = new Array(cooked);
            
            // Add .raw property with raw strings
            var rawJsArray = new Array(raw);
            Object.SetProperty(templateObject, "raw", rawJsArray);
            
            // Template objects should be frozen (immutable)
            // For now we just return the object - freezing can be added later if needed
            return templateObject;
        });
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