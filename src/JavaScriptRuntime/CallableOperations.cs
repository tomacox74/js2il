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
        var callArguments = JsCallArguments.FromArray(arguments);
        return CallCore(target, thisArgument, callArguments);
    }

    public static object? Call0(object? target, object? thisArgument)
    {
        var arguments = JsCallArguments.Empty;
        return CallCore(target, thisArgument, arguments);
    }

    public static object? Call1(object? target, object? thisArgument, object? argument0)
    {
        var arguments = JsCallArguments.From(argument0);
        return CallCore(target, thisArgument, arguments);
    }

    public static object? Call2(
        object? target,
        object? thisArgument,
        object? argument0,
        object? argument1)
    {
        var arguments = JsCallArguments.From(argument0, argument1);
        return CallCore(target, thisArgument, arguments);
    }

    public static object? Call3(
        object? target,
        object? thisArgument,
        object? argument0,
        object? argument1,
        object? argument2)
    {
        var arguments = JsCallArguments.From(argument0, argument1, argument2);
        return CallCore(target, thisArgument, arguments);
    }

    public static object? Call4(
        object? target,
        object? thisArgument,
        object? argument0,
        object? argument1,
        object? argument2,
        object? argument3)
    {
        var arguments = JsCallArguments.From(argument0, argument1, argument2, argument3);
        return CallCore(target, thisArgument, arguments);
    }

    public static object? Call5(
        object? target,
        object? thisArgument,
        object? argument0,
        object? argument1,
        object? argument2,
        object? argument3,
        object? argument4)
    {
        var arguments = JsCallArguments.From(
            argument0,
            argument1,
            argument2,
            argument3,
            argument4);
        return CallCore(target, thisArgument, arguments);
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
        var constructArguments = JsCallArguments.FromArray(arguments);
        return ConstructCore(target, newTarget, constructArguments);
    }

    public static object? Construct0(object? target, object? newTarget)
    {
        var arguments = JsCallArguments.Empty;
        return ConstructCore(target, newTarget, arguments);
    }

    public static object? Construct1(
        object? target,
        object? newTarget,
        object? argument0)
    {
        var arguments = JsCallArguments.From(argument0);
        return ConstructCore(target, newTarget, arguments);
    }

    public static object? Construct2(
        object? target,
        object? newTarget,
        object? argument0,
        object? argument1)
    {
        var arguments = JsCallArguments.From(argument0, argument1);
        return ConstructCore(target, newTarget, arguments);
    }

    public static object? Construct3(
        object? target,
        object? newTarget,
        object? argument0,
        object? argument1,
        object? argument2)
    {
        var arguments = JsCallArguments.From(argument0, argument1, argument2);
        return ConstructCore(target, newTarget, arguments);
    }

    public static object? Construct4(
        object? target,
        object? newTarget,
        object? argument0,
        object? argument1,
        object? argument2,
        object? argument3)
    {
        var arguments = JsCallArguments.From(argument0, argument1, argument2, argument3);
        return ConstructCore(target, newTarget, arguments);
    }

    public static object? Construct5(
        object? target,
        object? newTarget,
        object? argument0,
        object? argument1,
        object? argument2,
        object? argument3,
        object? argument4)
    {
        var arguments = JsCallArguments.From(
            argument0,
            argument1,
            argument2,
            argument3,
            argument4);
        return ConstructCore(target, newTarget, arguments);
    }

    private static object? CallCore(
        object? target,
        object? thisArgument,
        in JsCallArguments arguments)
    {
        return target switch
        {
            JsFunctionObject functionObject => CallFunctionObject(
                functionObject,
                thisArgument,
                arguments),
            Delegate legacyDelegate => LegacyDelegateFunctionAdapter.Invoke(
                legacyDelegate,
                RuntimeServices.EmptyScopes,
                thisArgument,
                arguments.ToArray()),
            Proxy proxy when proxy.IsCallableTarget => CallProxy(
                proxy,
                thisArgument,
                arguments.ToArray()),
            ClassConstructorValue => throw new TypeError("Class constructor cannot be invoked without 'new'"),
            _ => throw new TypeError("Value is not callable")
        };
    }

    private static object? ConstructCore(
        object? target,
        object? newTarget,
        in JsCallArguments arguments)
    {
        if (target is JsFunctionObject functionObject)
        {
            if (!functionObject.IsConstructor)
            {
                throw new TypeError("Value is not a constructor");
            }

            return ConstructFunctionObject(functionObject, arguments, newTarget);
        }

        if (!ObjectRuntime.IsConstructibleValue(target))
        {
            throw new TypeError("Value is not a constructor");
        }

        var legacyArguments = System.Array.ConvertAll(
            arguments.ToArray(),
            static argument => argument!);
        return ObjectRuntime.ConstructValue(target!, legacyArguments, newTarget);
    }

    private static object? CallFunctionObject(
        JsFunctionObject functionObject,
        object? thisArgument,
        in JsCallArguments arguments)
    {
        var effectiveThisArgument = functionObject.ResolveThisArgument(thisArgument);
        if (!functionObject.RequiresInvocationContext)
        {
            return functionObject.InvokeCall(effectiveThisArgument, arguments);
        }

        var previousThis = RuntimeServices.SetCurrentThis(effectiveThisArgument);
        var previousArguments = RuntimeServices.SetCurrentCallArguments(arguments);
        var previousCallee = RuntimeServices.SetCurrentCallee(functionObject);
        var previousNewTarget = RuntimeServices.SetCurrentNewTarget(
            functionObject.ResolveCallNewTarget());
        var lexicalSuperScopes = functionObject.GetLexicalSuperScopes();
        var previousSuperReceiver = lexicalSuperScopes is null
            ? null
            : RuntimeServices.SetCurrentLexicalSuperReceiver(
                functionObject.GetLexicalSuperReceiver());
        var previousSuperScopes = lexicalSuperScopes is null
            ? null
            : RuntimeServices.SetCurrentLexicalSuperScopes(lexicalSuperScopes);
        try
        {
            return functionObject.InvokeCall(effectiveThisArgument, arguments);
        }
        finally
        {
            if (lexicalSuperScopes is not null)
            {
                RuntimeServices.SetCurrentLexicalSuperScopes(previousSuperScopes);
                RuntimeServices.SetCurrentLexicalSuperReceiver(previousSuperReceiver);
            }
            RuntimeServices.SetCurrentNewTarget(previousNewTarget);
            RuntimeServices.SetCurrentCallee(previousCallee);
            RuntimeServices.RestoreCurrentCallArguments(previousArguments);
            RuntimeServices.SetCurrentThis(previousThis);
        }
    }

    private static object? ConstructFunctionObject(
        JsFunctionObject functionObject,
        in JsCallArguments arguments,
        object? newTarget)
    {
        if (!functionObject.RequiresInvocationContext)
        {
            return functionObject.InvokeConstruct(arguments, newTarget);
        }

        var previousArguments = RuntimeServices.SetCurrentCallArguments(arguments);
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
            RuntimeServices.RestoreCurrentCallArguments(previousArguments);
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
