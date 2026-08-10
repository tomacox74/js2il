using System.Reflection;
using System.Runtime.ExceptionServices;
using JavaScriptRuntime;

namespace Jroc.Runtime;

internal sealed class HostedDelegateFunctionObject : JsFunctionObject
{
    private readonly JsRuntimeInstance _runtime;
    private readonly Delegate _target;
    private readonly ParameterInfo[] _parameters;
    private readonly JsCallableScopeAbiDescriptor _abi;
    private readonly Closure.DelegateInvokeMetadata _invokeMetadata;
    private readonly int _jsArgumentStart;
    private readonly bool _usesGeneratedAbi;
    private readonly object[] _scopes;

    internal HostedDelegateFunctionObject(
        JsRuntimeInstance runtime,
        Delegate target)
    {
        _runtime = runtime;
        _target = target;
        _parameters = target.GetType().GetMethod(nameof(Action.Invoke))?.GetParameters()
            ?? throw new ArgumentException(
                $"Delegate type '{target.GetType()}' does not define Invoke().",
                nameof(target));
        _abi = JsCallableScopeAbiResolver.ResolveHosted(target);
        _invokeMetadata = Closure.GetDelegateInvokeMetadata(target);
        _usesGeneratedAbi = _abi.IsFromAttribute
            || JsFuncDelegates.IsJsFuncDelegateType(target.GetType());
        _scopes = [target.Target!];
        _jsArgumentStart = HostedClrInvocation.GetJsArgumentStart(
            _parameters,
            _abi.Kind);

        Function.InitializeFunctionInstance(
            this,
            HostedClrInvocation.GetVisibleLength(_parameters, _jsArgumentStart),
            target.Method.Name,
            requiresInvocationContext: false);
    }

    public override bool RequiresInvocationContext => false;

    protected override object? CallCore(
        object? thisArgument,
        in JsCallArguments arguments)
    {
        if (_usesGeneratedAbi)
        {
            return _runtime.NormalizeHostValue(
                Closure.InvokeBuiltinDelegate(
                    _target,
                    _invokeMetadata,
                    _scopes,
                    arguments,
                    newTarget: null));
        }

        if (HostedClrInvocation.TryInvokeFixedObjectDelegate(
                _target,
                arguments,
                _runtime,
                out var fixedResult))
        {
            return _runtime.NormalizeHostValue(fixedResult);
        }

        var invokeArguments = HostedClrInvocation.BuildArguments(
            _parameters,
            _abi,
            _scopes,
            arguments,
            _runtime.ProjectHostValue);

        object? result;
        try
        {
            result = _target.DynamicInvoke(invokeArguments);
        }
        catch (TargetInvocationException exception)
            when (exception.InnerException != null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }

        return _runtime.NormalizeHostValue(result);
    }
}

internal sealed class HostedCallbackFunctionObject : JsFunctionObject
{
    private readonly JsRuntimeInstance _runtime;
    private readonly JsHostFunction _hostFunction;

    internal HostedCallbackFunctionObject(
        JsRuntimeInstance runtime,
        JsHostFunction hostFunction)
    {
        _runtime = runtime;
        _hostFunction = hostFunction;

        Function.InitializeFunctionInstance(
            this,
            hostFunction.Length,
            hostFunction.Name,
            requiresInvocationContext: false);
    }

    public override bool IsConstructor => _hostFunction.IsConstructor;

    public override bool RequiresInvocationContext => false;

    protected override object? CallCore(
        object? thisArgument,
        in JsCallArguments arguments)
    {
        // JsHostFunction intentionally defines an array ABI.
        var result = _hostFunction.Invoke(
            _runtime.ProjectHostValue(thisArgument),
            _runtime.ProjectHostArguments(arguments.ToArray()));
        return _runtime.NormalizeHostValue(result);
    }

    protected override object? ConstructCore(
        in JsCallArguments arguments,
        object? newTarget)
    {
        // JsHostFunction intentionally defines an array ABI.
        var result = _hostFunction.Construct(
            _runtime.ProjectHostArguments(arguments.ToArray()),
            _runtime.ProjectHostValue(newTarget));
        result = _runtime.NormalizeHostValue(result);
        if (!TypeUtilities.IsConstructorReturnOverride(result))
        {
            throw new TypeError("Host constructor must return an object");
        }

        return result;
    }
}

internal sealed class HostedMethodFunctionObject : JsFunctionObject
{
    private readonly JsRuntimeInstance _runtime;
    private readonly object _target;
    private readonly MethodInfo _method;
    private readonly ParameterInfo[] _parameters;
    private readonly JsCallableScopeAbiDescriptor _abi;
    private readonly int _jsArgumentStart;
    private readonly object[] _scopes;

    internal HostedMethodFunctionObject(
        JsRuntimeInstance runtime,
        object target,
        MethodInfo method)
    {
        _runtime = runtime;
        _target = target;
        _method = method;
        _parameters = method.GetParameters();
        _abi = JsCallableScopeAbiResolver.ResolveHosted(method);
        _scopes = [target];
        _jsArgumentStart = HostedClrInvocation.GetJsArgumentStart(
            _parameters,
            _abi.Kind);

        Function.InitializeFunctionInstance(
            this,
            HostedClrInvocation.GetVisibleLength(
                _parameters,
                _jsArgumentStart),
            method.Name,
            requiresInvocationContext: false);
    }

    public override bool RequiresInvocationContext => false;

    internal static bool CanAccept(MethodInfo method, int argumentCount)
    {
        var parameters = method.GetParameters();
        var abi = JsCallableScopeAbiResolver.ResolveHosted(method);
        var argumentStart = HostedClrInvocation.GetJsArgumentStart(
            parameters,
            abi.Kind);
        var hasParamsArray = HostedClrInvocation.HasPackedArguments(
            parameters,
            argumentStart);
        var visibleCount = parameters.Length - argumentStart;
        return hasParamsArray || argumentCount <= visibleCount;
    }

    protected override object? CallCore(
        object? thisArgument,
        in JsCallArguments arguments)
    {
        var invokeArguments = HostedClrInvocation.BuildArguments(
            _parameters,
            _abi,
            _scopes,
            arguments,
            _abi.IsFromAttribute
                ? static value => value
                : _runtime.ProjectHostValue);

        try
        {
            var result = _method.Invoke(_target, invokeArguments);
            return _runtime.NormalizeHostValue(result);
        }
        catch (TargetInvocationException exception)
            when (exception.InnerException != null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
    }
}

internal static class HostedClrInvocation
{
    internal static bool TryInvokeFixedObjectDelegate(
        Delegate target,
        in JsCallArguments arguments,
        JsRuntimeInstance runtime,
        out object? result)
    {
        switch (target)
        {
            case Func<object?> function:
                result = function();
                return true;
            case Action action:
                action();
                result = null;
                return true;
            case Func<object, object?> function:
                result = function(Project(runtime, arguments, 0)!);
                return true;
            case Action<object> action:
                action(Project(runtime, arguments, 0)!);
                result = null;
                return true;
            case Func<object, object, object?> function:
                result = function(
                    Project(runtime, arguments, 0)!,
                    Project(runtime, arguments, 1)!);
                return true;
            case Action<object, object> action:
                action(
                    Project(runtime, arguments, 0)!,
                    Project(runtime, arguments, 1)!);
                result = null;
                return true;
            case Func<object, object, object, object?> function:
                result = function(
                    Project(runtime, arguments, 0)!,
                    Project(runtime, arguments, 1)!,
                    Project(runtime, arguments, 2)!);
                return true;
            case Action<object, object, object> action:
                action(
                    Project(runtime, arguments, 0)!,
                    Project(runtime, arguments, 1)!,
                    Project(runtime, arguments, 2)!);
                result = null;
                return true;
            case Func<object, object, object, object, object?> function:
                result = function(
                    Project(runtime, arguments, 0)!,
                    Project(runtime, arguments, 1)!,
                    Project(runtime, arguments, 2)!,
                    Project(runtime, arguments, 3)!);
                return true;
            case Action<object, object, object, object> action:
                action(
                    Project(runtime, arguments, 0)!,
                    Project(runtime, arguments, 1)!,
                    Project(runtime, arguments, 2)!,
                    Project(runtime, arguments, 3)!);
                result = null;
                return true;
            case Func<object, object, object, object, object, object?> function:
                result = function(
                    Project(runtime, arguments, 0)!,
                    Project(runtime, arguments, 1)!,
                    Project(runtime, arguments, 2)!,
                    Project(runtime, arguments, 3)!,
                    Project(runtime, arguments, 4)!);
                return true;
            case Action<object, object, object, object, object> action:
                action(
                    Project(runtime, arguments, 0)!,
                    Project(runtime, arguments, 1)!,
                    Project(runtime, arguments, 2)!,
                    Project(runtime, arguments, 3)!,
                    Project(runtime, arguments, 4)!);
                result = null;
                return true;
            default:
                result = null;
                return false;
        }
    }

    private static object? Project(
        JsRuntimeInstance runtime,
        in JsCallArguments arguments,
        int index)
        => runtime.ProjectHostValue(arguments.GetArgument(index));

    internal static int GetJsArgumentStart(
        ParameterInfo[] parameters,
        CallableScopeAbiKind abiKind)
        => (abiKind == CallableScopeAbiKind.NoScopes ? 0 : 1)
            + (JsCallableScopeAbiResolver.HasNewTargetParameter(
                parameters,
                abiKind)
                ? 1
                : 0);

    internal static bool HasPackedArguments(
        ParameterInfo[] parameters,
        int jsArgumentStart)
        => parameters.Length > jsArgumentStart
            && (Attribute.IsDefined(
                    parameters[^1],
                    typeof(ParamArrayAttribute))
                || parameters[^1].ParameterType == typeof(object[]));

    internal static object?[] BuildArguments(
        ParameterInfo[] parameters,
        JsCallableScopeAbiDescriptor abi,
        object[] scopes,
        in JsCallArguments jsArguments,
        Func<object?, object?> projectArgument)
    {
        var invokeArguments = new object?[parameters.Length];
        var invokeIndex = 0;

        if (abi.Kind != CallableScopeAbiKind.NoScopes)
        {
            invokeArguments[invokeIndex++] = abi.Kind == CallableScopeAbiKind.ScopeArray
                ? scopes
                : JsCallableScopeAbiResolver.GetSingleScopeArgument(
                    scopes,
                    abi.SingleScopeType);
        }

        if (JsCallableScopeAbiResolver.HasNewTargetParameter(
                parameters,
                abi.Kind))
        {
            invokeArguments[invokeIndex++] = null;
        }

        var jsArgumentStart = invokeIndex;
        var hasPackedArguments = HasPackedArguments(
            parameters,
            jsArgumentStart);
        var fixedCount = parameters.Length
            - jsArgumentStart
            - (hasPackedArguments ? 1 : 0);

        for (var index = 0; index < fixedCount; index++)
        {
            invokeArguments[invokeIndex++] = index < jsArguments.Count
                ? projectArgument(jsArguments.GetArgument(index))
                : parameters[jsArgumentStart + index].HasDefaultValue
                    ? Type.Missing
                    : null;
        }

        if (hasPackedArguments)
        {
            var restCount = System.Math.Max(
                0,
                jsArguments.Count - fixedCount);
            var elementType = parameters[^1].ParameterType.GetElementType()
                ?? typeof(object);
            var rest = System.Array.CreateInstance(elementType, restCount);
            for (var index = 0; index < restCount; index++)
            {
                rest.SetValue(
                    projectArgument(
                        jsArguments.GetArgument(fixedCount + index)),
                    index);
            }

            invokeArguments[invokeIndex] = rest;
        }

        return invokeArguments;
    }

    internal static double GetVisibleLength(
        ParameterInfo[] parameters,
        int argumentStart)
    {
        var length = 0;
        for (var index = argumentStart; index < parameters.Length; index++)
        {
            if (parameters[index].HasDefaultValue
                || Attribute.IsDefined(
                    parameters[index],
                    typeof(ParamArrayAttribute))
                || parameters[index].ParameterType == typeof(object[]))
            {
                break;
            }

            length++;
        }

        return length;
    }
}
