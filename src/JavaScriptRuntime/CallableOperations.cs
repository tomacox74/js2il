namespace JavaScriptRuntime;

/// <summary>
/// Centralized ECMAScript callable and constructor operations.
/// </summary>
public static class CallableOperations
{
    internal static bool TryGetBuiltinAdapter(
        object? value,
        out BuiltinDelegateFunctionAdapter adapter)
    {
        if (value is BuiltinDelegateFunctionAdapter candidate)
        {
            adapter = candidate;
            return true;
        }

        adapter = null!;
        return false;
    }

    internal static bool IsBuiltinAdapter(object? value)
        => TryGetBuiltinAdapter(value, out _);

    internal static bool HasSameBuiltinDelegateMethod(
        object? left,
        object? right)
        => TryGetBuiltinAdapter(left, out var leftAdapter)
            && TryGetBuiltinAdapter(right, out var rightAdapter)
            && leftAdapter.Target.Method == rightAdapter.Target.Method;

    public static bool IsCallable(object? value)
    {
        return value switch
        {
            JsFunctionObject => true,
            Type type => ObjectRuntime.IsConstructibleValue(type),
            Proxy proxy => proxy.IsCallableTarget,
            _ => false
        };
    }

    public static object? Call(object? target, object? thisArgument, object?[]? arguments)
    {
        var callArguments = JsCallArguments.FromArray(arguments);
        return CallCore(target, thisArgument, callArguments);
    }

    internal static object? Call(
        object? target,
        object? thisArgument,
        in JsCallArguments arguments)
        => CallCore(target, thisArgument, arguments);

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

    internal static object? ConstructWithReceiver(
        JsFunctionObject functionObject,
        object receiver,
        object?[]? arguments,
        object? newTarget)
    {
        ArgumentNullException.ThrowIfNull(functionObject);
        ArgumentNullException.ThrowIfNull(receiver);
        if (!functionObject.IsConstructor)
        {
            throw new TypeError("Value is not a constructor");
        }

        var callArguments = JsCallArguments.FromArray(arguments);
        var result = InvokeConstructBody(
            functionObject,
            receiver,
            callArguments,
            newTarget);
        return TypeUtilities.IsConstructorReturnOverride(result)
            ? result
            : receiver;
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

    internal static object? Construct(
        object? target,
        in JsCallArguments arguments,
        object? newTarget)
        => ConstructCore(target, newTarget, arguments);

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
            Proxy proxy when proxy.IsCallableTarget => CallProxy(
                proxy,
                thisArgument,
                arguments),
            Type => throw new TypeError("Class constructor cannot be invoked without 'new'"),
            _ => throw new TypeError("Value is not callable")
        };
    }

    private static object? ConstructCore(
        object? target,
        object? newTarget,
        in JsCallArguments arguments)
    {
        if (target is Proxy proxy)
        {
            return ConstructProxy(proxy, arguments, newTarget);
        }

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
        => CallFunctionObject(
            functionObject,
            thisArgument,
            arguments,
            functionObject.ResolveCallNewTarget());

    private static object? CallFunctionObject(
        JsFunctionObject functionObject,
        object? thisArgument,
        in JsCallArguments arguments,
        object? newTarget)
    {
        var effectiveThisArgument = functionObject.ResolveThisArgument(thisArgument);
        if (!functionObject.RequiresInvocationContext)
        {
            return functionObject.InvokeCall(effectiveThisArgument, arguments);
        }

        var previousThis = RuntimeServices.SetCurrentThis(effectiveThisArgument);
        var previousArguments = RuntimeServices.SetCurrentCallArguments(arguments);
        var previousCallee = RuntimeServices.SetCurrentCallee(functionObject);
        var previousNewTarget = RuntimeServices.SetCurrentNewTarget(newTarget);
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

    private static object? InvokeConstructBody(
        JsFunctionObject functionObject,
        object receiver,
        in JsCallArguments arguments,
        object? newTarget)
    {
        var previousThis = RuntimeServices.SetCurrentThis(receiver);
        if (!functionObject.RequiresInvocationContext)
        {
            try
            {
                return functionObject.InvokeConstructBody(
                    receiver,
                    arguments,
                    newTarget);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        var previousArguments = RuntimeServices.SetCurrentCallArguments(arguments);
        var previousCallee = RuntimeServices.SetCurrentCallee(functionObject);
        var previousNewTarget = RuntimeServices.SetCurrentNewTarget(newTarget);
        try
        {
            return functionObject.InvokeConstructBody(
                receiver,
                arguments,
                newTarget);
        }
        finally
        {
            RuntimeServices.SetCurrentNewTarget(previousNewTarget);
            RuntimeServices.SetCurrentCallee(previousCallee);
            RuntimeServices.RestoreCurrentCallArguments(previousArguments);
            RuntimeServices.SetCurrentThis(previousThis);
        }
    }

    private static object? CallProxy(
        Proxy proxy,
        object? thisArgument,
        in JsCallArguments arguments)
    {
        var target = proxy.GetTarget("apply");
        if (!IsCallable(target))
        {
            throw new TypeError("Proxy target is not callable");
        }

        var trapArguments = new JavaScriptRuntime.Array(arguments.ToArray());
        if (proxy.TryInvokeTrap(
                "apply",
                "apply",
                [target, thisArgument, trapArguments],
                out var trapResult))
        {
            return trapResult;
        }

        return CallCore(target, thisArgument, arguments);
    }

    private static object? ConstructProxy(
        Proxy proxy,
        in JsCallArguments arguments,
        object? newTarget)
    {
        var target = proxy.GetTarget("construct");
        if (!ObjectRuntime.IsConstructibleValue(target))
        {
            throw new TypeError("Proxy target is not a constructor");
        }

        var effectiveNewTarget = newTarget ?? proxy;
        var trapArguments = new JavaScriptRuntime.Array(arguments.ToArray());
        if (proxy.TryInvokeTrap(
                "construct",
                "construct",
                [target, trapArguments, effectiveNewTarget],
                out var trapResult))
        {
            if (!Proxy.IsObjectLikeValue(trapResult))
            {
                throw new TypeError("Proxy construct trap must return an object");
            }
            return trapResult;
        }

        return ConstructCore(target, effectiveNewTarget, arguments);
    }
}
