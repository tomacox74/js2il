using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Jroc.Runtime;

internal class JsHandleProxy : DispatchProxy
{
    private JsRuntimeInstance? _runtime;
    private object? _target;
    private int _disposed;

    internal void Initialize(JsRuntimeInstance runtime, object? target)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(target);

        _runtime = runtime;
        _target = target;
    }

    internal object UnwrapTarget()
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(nameof(JsHandleProxy));
        }

        return _target ?? throw new ObjectDisposedException(nameof(JsHandleProxy));
    }

    internal object UnwrapTarget(JsRuntimeInstance runtime)
    {
        if (!ReferenceEquals(_runtime, runtime))
        {
            throw new InvalidOperationException(
                "JavaScript handles cannot cross module runtime instances.");
        }

        return UnwrapTarget();
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null)
        {
            throw new ArgumentNullException(nameof(targetMethod));
        }

        if (targetMethod.DeclaringType == typeof(IDisposable) && targetMethod.Name == nameof(IDisposable.Dispose))
        {
            DisposeHandle();
            return null;
        }

        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(targetMethod.DeclaringType?.FullName ?? nameof(JsHandleProxy));
        }

        var runtime = _runtime ?? throw new ObjectDisposedException(nameof(JsHandleProxy));
        var target = _target ?? throw new ObjectDisposedException(nameof(JsHandleProxy));

        if (targetMethod.DeclaringType == typeof(object))
        {
            return HandleObjectMethod(targetMethod, args);
        }

        if (targetMethod.IsSpecialName)
        {
            if (targetMethod.Name.StartsWith("get_", StringComparison.Ordinal))
            {
                var name = GetContractMemberName(targetMethod, targetMethod.Name.Substring(4));
                try
                {
                    return runtime.Invoke(() =>
                    {
                        var value = ExportMemberResolver.GetExportMember(target, name);
                        return JsReturnConverter.ConvertReturn(
                            runtime,
                            value,
                            targetMethod.ReturnType,
                            name,
                            targetMethod.DeclaringType);
                    });
                }
                catch (Exception ex)
                {
                    var translated = JsHostingExceptionTranslator.TranslateProxyCall(ex, runtime, memberName: name, contractType: targetMethod.DeclaringType);
                    ExceptionDispatchInfo.Capture(translated).Throw();
                    throw;
                }
            }

            if (targetMethod.Name.StartsWith("set_", StringComparison.Ordinal))
            {
                var name = GetContractMemberName(targetMethod, targetMethod.Name.Substring(4));
                try
                {
                    runtime.Invoke(() => ExportMemberResolver.SetExportMember(
                        runtime,
                        target,
                        name,
                        args is { Length: > 0 } ? args[0] : null));
                    return null;
                }
                catch (Exception ex)
                {
                    var translated = JsHostingExceptionTranslator.TranslateProxyCall(ex, runtime, memberName: name, contractType: targetMethod.DeclaringType);
                    ExceptionDispatchInfo.Capture(translated).Throw();
                    throw;
                }
            }
        }

        var methodName = GetContractMemberName(targetMethod, targetMethod.Name);
        try
        {
            return runtime.Invoke(() =>
            {
                if (TryInvokeGeneratedCallableHelper(
                    runtime,
                    target,
                    targetMethod,
                    args ?? Array.Empty<object?>(),
                    out var callableResult))
                {
                    return callableResult;
                }

                if (TryInvokeGeneratedObjectHelper(
                    runtime,
                    target,
                    targetMethod,
                    args ?? Array.Empty<object?>(),
                    out var objectResult))
                {
                    return objectResult;
                }

                if (TryInvokeGeneratedArrayHelper(
                    runtime,
                    target,
                    targetMethod,
                    args ?? Array.Empty<object?>(),
                    out var helperResult))
                {
                    return helperResult;
                }

                var result = ExportMemberResolver.InvokeInstanceMethod(
                    runtime,
                    target,
                    methodName,
                    args ?? Array.Empty<object?>());
                return JsReturnConverter.ConvertReturn(
                    runtime,
                    result,
                    targetMethod.ReturnType,
                    methodName,
                    targetMethod.DeclaringType);
            });
        }
        catch (Exception ex)
        {
            var translated = JsHostingExceptionTranslator.TranslateProxyCall(ex, runtime, memberName: methodName, contractType: targetMethod.DeclaringType);
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }

    private static bool TryInvokeGeneratedCallableHelper(
        JsRuntimeInstance runtime,
        object target,
        MethodInfo targetMethod,
        object?[] args,
        out object? result)
    {
        result = null;
        if (!GeneratedContractMetadata.IsCallableContract(targetMethod.DeclaringType)
            || targetMethod.Name != "Invoke")
        {
            return false;
        }

        result = JsReturnConverter.ConvertReturn(
            runtime,
            ExportMemberResolver.InvokeJsCallable(
                runtime,
                target,
                UnpackParamsArray(targetMethod, args),
                receiver: null),
            targetMethod.ReturnType,
            targetMethod.Name,
            targetMethod.DeclaringType);
        return true;
    }

    private static bool TryInvokeGeneratedObjectHelper(
        JsRuntimeInstance runtime,
        object target,
        MethodInfo targetMethod,
        object?[] args,
        out object? result)
    {
        result = null;
        if (!GeneratedContractMetadata.IsObjectContract(targetMethod.DeclaringType))
        {
            return false;
        }

        switch (targetMethod.Name)
        {
            case "GetDynamicProperty":
                RequireArgumentCount(targetMethod, args, 1);
                var propertyName = Convert.ToString(
                    args[0],
                    System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
                result = JsReturnConverter.ConvertReturn(
                    runtime,
                    JavaScriptRuntime.ObjectRuntime.GetProperty(target, propertyName),
                    targetMethod.ReturnType,
                    propertyName,
                    targetMethod.DeclaringType);
                return true;

            case "SetDynamicProperty":
                RequireArgumentCount(targetMethod, args, 2);
                JavaScriptRuntime.ObjectRuntime.SetItem(
                    target,
                    Convert.ToString(
                        args[0],
                        System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
                    runtime.NormalizeHostValue(args[1]));
                return true;

            case "HasDynamicProperty":
                RequireArgumentCount(targetMethod, args, 1);
                result = JavaScriptRuntime.ObjectRuntime.HasPropertyIn(
                    Convert.ToString(
                        args[0],
                        System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
                    target);
                return true;

            default:
                return false;
        }
    }

    private static string GetContractMemberName(MethodInfo targetMethod, string fallback)
        => GeneratedContractMetadata.GetExportName(targetMethod) ?? fallback;

    private static bool TryInvokeGeneratedArrayHelper(
        JsRuntimeInstance runtime,
        object target,
        MethodInfo targetMethod,
        object?[] args,
        out object? result)
    {
        result = null;
        if (!IsGeneratedArrayContract(targetMethod.DeclaringType))
        {
            return false;
        }

        switch (targetMethod.Name)
        {
            case "Get":
                RequireArgumentCount(targetMethod, args, 1);
                result = JsReturnConverter.ConvertReturn(
                    runtime,
                    JavaScriptRuntime.ObjectRuntime.GetItem(target, ToArrayIndex(args[0])),
                    targetMethod.ReturnType,
                    targetMethod.Name,
                    targetMethod.DeclaringType);
                return true;

            case "Set":
                RequireArgumentCount(targetMethod, args, 2);
                JavaScriptRuntime.ObjectRuntime.SetItem(
                    target,
                    ToArrayIndex(args[0]),
                    runtime.NormalizeHostValue(args[1]));
                result = null;
                return true;

            case "HasIndex":
                RequireArgumentCount(targetMethod, args, 1);
                result = JavaScriptRuntime.ObjectRuntime.HasPropertyIn(
                    ToArrayIndex(args[0]),
                    target);
                return true;

            case "Push":
                var values = UnpackParamsArray(targetMethod, args)
                    .Select(runtime.NormalizeHostValue)
                    .ToArray();
                var push = JavaScriptRuntime.ObjectRuntime.GetProperty(target, "push");
                result = JsReturnConverter.ConvertReturn(
                    runtime,
                    ExportMemberResolver.InvokeJsCallable(runtime, push!, values, receiver: target),
                    targetMethod.ReturnType,
                    targetMethod.Name,
                    targetMethod.DeclaringType);
                return true;

            default:
                return false;
        }
    }

    private static bool IsGeneratedArrayContract(Type? type)
        => GeneratedContractMetadata.IsArrayContract(type)
           || type?.Name.EndsWith("Array", StringComparison.Ordinal) == true
              && type.GetMethods().Any(method => method.Name == "HasIndex");

    private static object?[] UnpackParamsArray(MethodInfo targetMethod, object?[]? args)
    {
        if (args is { Length: 1 }
            && args[0] is object?[] packed
            && targetMethod.GetParameters() is [{ ParameterType: { IsArray: true } parameterType }]
            && parameterType.GetElementType() == typeof(object))
        {
            return packed;
        }

        return args ?? Array.Empty<object?>();
    }

    private static double ToArrayIndex(object? value)
        => Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);

    private static void RequireArgumentCount(MethodInfo method, object?[] args, int count)
    {
        if (args.Length != count)
        {
            throw new ArgumentException(
                $"Generated array method '{method.Name}' requires {count} argument(s).",
                nameof(args));
        }
    }

    private void DisposeHandle()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        Interlocked.Exchange(ref _runtime, null);
        Interlocked.Exchange(ref _target, null);
    }

    private object? HandleObjectMethod(MethodInfo targetMethod, object?[]? args)
    {
        return targetMethod.Name switch
        {
            nameof(ToString) => nameof(JsHandleProxy),
            nameof(GetHashCode) => base.GetHashCode(),
            nameof(Equals) => ReferenceEquals(this, args != null && args.Length > 0 ? args[0] : null),
            _ => null,
        };
    }
}

internal class JsConstructorProxy : DispatchProxy
{
    private JsRuntimeInstance? _runtime;
    private object? _constructor;
    private int _disposed;

    internal void Initialize(JsRuntimeInstance runtime, object? constructor)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(constructor);

        _runtime = runtime;
        _constructor = constructor;
    }

    internal object UnwrapConstructor()
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(nameof(JsConstructorProxy));
        }

        return _constructor ?? throw new ObjectDisposedException(nameof(JsConstructorProxy));
    }

    internal object UnwrapConstructor(JsRuntimeInstance runtime)
    {
        if (!ReferenceEquals(_runtime, runtime))
        {
            throw new InvalidOperationException(
                "JavaScript constructors cannot cross module runtime instances.");
        }

        return UnwrapConstructor();
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null)
        {
            throw new ArgumentNullException(nameof(targetMethod));
        }

        if (targetMethod.DeclaringType == typeof(IDisposable) && targetMethod.Name == nameof(IDisposable.Dispose))
        {
            DisposeHandle();
            return null;
        }

        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(targetMethod.DeclaringType?.FullName ?? nameof(JsConstructorProxy));
        }

        var runtime = _runtime ?? throw new ObjectDisposedException(nameof(JsConstructorProxy));
        var constructor = _constructor ?? throw new ObjectDisposedException(nameof(JsConstructorProxy));

        if (targetMethod.DeclaringType == typeof(object))
        {
            return HandleObjectMethod(targetMethod, args);
        }

        try
        {
            return runtime.Invoke(() =>
            {
                if (targetMethod.IsSpecialName)
                {
                    if (targetMethod.Name.StartsWith("get_", StringComparison.Ordinal))
                    {
                        var propertyName = GetContractMemberName(targetMethod, targetMethod.Name.Substring(4));
                        var propertyValue = ExportMemberResolver.GetExportMember(constructor, propertyName);
                        return JsReturnConverter.ConvertReturn(
                            runtime,
                            propertyValue,
                            targetMethod.ReturnType,
                            propertyName,
                            targetMethod.DeclaringType);
                    }

                    if (targetMethod.Name.StartsWith("set_", StringComparison.Ordinal))
                    {
                        var propertyName = GetContractMemberName(targetMethod, targetMethod.Name.Substring(4));
                        ExportMemberResolver.SetExportMember(
                            runtime,
                            constructor,
                            propertyName,
                            args is { Length: > 0 } ? args[0] : null);
                        return null;
                    }
                }

                if (targetMethod.Name != "Construct")
                {
                    var memberName = GetContractMemberName(targetMethod, targetMethod.Name);
                    var member = ExportMemberResolver.GetExportMember(constructor, memberName);
                    if (!JavaScriptRuntime.CallableOperations.IsCallable(member))
                    {
                        throw new MissingMethodException($"Constructor member '{memberName}' is not callable.");
                    }

                    return JsReturnConverter.ConvertReturn(
                        runtime,
                        ExportMemberResolver.InvokeJsCallable(
                            runtime,
                            member!,
                            UnpackParamsArray(targetMethod, args),
                            receiver: constructor),
                        targetMethod.ReturnType,
                        memberName,
                        targetMethod.DeclaringType);
                }

                var result = ExportMemberResolver.Construct(
                    runtime,
                    constructor,
                    UnpackParamsArray(targetMethod, args));
                return JsReturnConverter.ConvertReturn(
                    runtime,
                    result,
                    targetMethod.ReturnType,
                    targetMethod.Name,
                    targetMethod.DeclaringType);
            });
        }
        catch (Exception ex)
        {
            var translated = JsHostingExceptionTranslator.TranslateProxyCall(
                ex,
                runtime,
                memberName: GetContractMemberName(targetMethod, targetMethod.Name),
                contractType: targetMethod.DeclaringType);
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }

    private void DisposeHandle()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        Interlocked.Exchange(ref _runtime, null);
        Interlocked.Exchange(ref _constructor, null);
    }

    private static string GetContractMemberName(MethodInfo targetMethod, string fallback)
        => GeneratedContractMetadata.GetExportName(targetMethod) ?? fallback;

    private static object?[] UnpackParamsArray(MethodInfo targetMethod, object?[]? args)
    {
        if (args is { Length: 1 }
            && args[0] is object?[] packed
            && targetMethod.GetParameters() is [{ ParameterType: { IsArray: true } parameterType }]
            && parameterType.GetElementType() == typeof(object))
        {
            return packed;
        }

        return args ?? Array.Empty<object?>();
    }

    private object? HandleObjectMethod(MethodInfo targetMethod, object?[]? args)
    {
        return targetMethod.Name switch
        {
            nameof(ToString) => nameof(JsConstructorProxy),
            nameof(GetHashCode) => base.GetHashCode(),
            nameof(Equals) => ReferenceEquals(this, args != null && args.Length > 0 ? args[0] : null),
            _ => null,
        };
    }
}
