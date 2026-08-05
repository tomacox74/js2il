namespace JavaScriptRuntime;

/// <summary>
/// Centralized ECMAScript callable and constructor operations.
/// </summary>
public static class CallableOperations
{
    public static bool IsCallable(object? value)
    {
        return value switch
        {
            JsFunctionObject => true,
            Delegate => true,
            ClassConstructorValue => true,
            Proxy proxy => proxy.IsCallableTarget,
            _ => false
        };
    }

    public static object? Call(object? target, object? thisArgument, object?[]? arguments)
    {
        var callArguments = arguments ?? System.Array.Empty<object?>();

        return target switch
        {
            JsFunctionObject functionObject => CallFunctionObject(functionObject, thisArgument, callArguments),
            Delegate legacyDelegate => LegacyDelegateFunctionAdapter.Invoke(
                legacyDelegate,
                RuntimeServices.EmptyScopes,
                thisArgument,
                callArguments),
            Proxy proxy when proxy.IsCallableTarget => CallProxy(proxy, thisArgument, callArguments),
            ClassConstructorValue => throw new TypeError("Class constructor cannot be invoked without 'new'"),
            _ => throw new TypeError("Value is not callable")
        };
    }

    public static bool IsConstructor(object? value)
    {
        return value switch
        {
            JsFunctionObject functionObject => functionObject.IsConstructor,
            _ => ObjectRuntime.IsConstructibleValue(value)
        };
    }

    public static object? Construct(object? target, object?[]? arguments)
        => Construct(target, arguments, target);

    public static object? Construct(object? target, object?[]? arguments, object? newTarget)
    {
        var constructArguments = arguments ?? System.Array.Empty<object?>();

        if (target is JsFunctionObject functionObject)
        {
            if (!functionObject.IsConstructor)
            {
                throw new TypeError("Value is not a constructor");
            }

            return ConstructFunctionObject(functionObject, constructArguments, newTarget);
        }

        if (!ObjectRuntime.IsConstructibleValue(target))
        {
            throw new TypeError("Value is not a constructor");
        }

        var legacyArguments = System.Array.ConvertAll(constructArguments, static argument => argument!);
        return ObjectRuntime.ConstructValue(target!, legacyArguments, newTarget);
    }

    private static object? CallFunctionObject(
        JsFunctionObject functionObject,
        object? thisArgument,
        object?[] arguments)
    {
        var previousThis = RuntimeServices.SetCurrentThis(thisArgument);
        var previousArguments = RuntimeServices.SetCurrentArguments(arguments);
        var previousCallee = RuntimeServices.SetCurrentCallee(functionObject);
        var previousNewTarget = RuntimeServices.SetCurrentNewTarget(null);
        try
        {
            return functionObject.InvokeCall(thisArgument, arguments);
        }
        finally
        {
            RuntimeServices.SetCurrentNewTarget(previousNewTarget);
            RuntimeServices.SetCurrentCallee(previousCallee);
            RuntimeServices.SetCurrentArguments(previousArguments);
            RuntimeServices.SetCurrentThis(previousThis);
        }
    }

    private static object? ConstructFunctionObject(
        JsFunctionObject functionObject,
        object?[] arguments,
        object? newTarget)
    {
        var previousArguments = RuntimeServices.SetCurrentArguments(arguments);
        var previousCallee = RuntimeServices.SetCurrentCallee(functionObject);
        var previousNewTarget = RuntimeServices.SetCurrentNewTarget(newTarget);
        try
        {
            return functionObject.InvokeConstruct(arguments, newTarget);
        }
        finally
        {
            RuntimeServices.SetCurrentNewTarget(previousNewTarget);
            RuntimeServices.SetCurrentCallee(previousCallee);
            RuntimeServices.SetCurrentArguments(previousArguments);
        }
    }

    private static object? CallProxy(Proxy proxy, object? thisArgument, object?[] arguments)
    {
        var previousThis = RuntimeServices.SetCurrentThis(thisArgument);
        try
        {
            return Closure.InvokeWithArgs(proxy, RuntimeServices.EmptyScopes, arguments);
        }
        finally
        {
            RuntimeServices.SetCurrentThis(previousThis);
        }
    }
}
