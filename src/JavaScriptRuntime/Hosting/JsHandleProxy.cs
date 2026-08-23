using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using JsArrayBuffer = JavaScriptRuntime.SharedArrayBuffer;
using JsCallableOperations = JavaScriptRuntime.CallableOperations;
using JsObjectRuntime = JavaScriptRuntime.ObjectRuntime;
using JsSymbol = JavaScriptRuntime.Symbol;
using JsTypedArray = JavaScriptRuntime.TypedArrayBase;

namespace Jroc.Runtime;

internal class JsHandleProxy : DispatchProxy
{
    private JsRuntimeInstance? _runtime;
    private object? _target;
    private Type? _contractType;
    private int _disposed;

    internal void Initialize(
        JsRuntimeInstance runtime,
        object? target,
        Type contractType)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(contractType);

        _runtime = runtime;
        _target = target;
        _contractType = contractType;
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
        var contractType = _contractType
            ?? throw new ObjectDisposedException(nameof(JsHandleProxy));

        if (targetMethod.DeclaringType == typeof(object))
        {
            return HandleObjectMethod(targetMethod, args);
        }

        if (TryInvokeEnumerableMember(
                runtime,
                target,
                contractType,
                targetMethod,
                args,
                out var enumerableResult))
        {
            return enumerableResult;
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
                        if (TryGetGeneratedBuiltinProperty(
                                runtime,
                                target,
                                contractType,
                                targetMethod,
                                out var builtinValue))
                        {
                            return builtinValue;
                        }

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
                    runtime.Invoke(() =>
                    {
                        if (TrySetGeneratedBuiltinProperty(
                                runtime,
                                target,
                                contractType,
                                targetMethod,
                                args is { Length: > 0 } ? args[0] : null))
                        {
                            return;
                        }

                        ExportMemberResolver.SetExportMember(
                            runtime,
                            target,
                            name,
                            args is { Length: > 0 } ? args[0] : null);
                    });
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

                if (TryInvokeGeneratedBuiltinHelper(
                    runtime,
                    target,
                    contractType,
                    targetMethod,
                    args ?? Array.Empty<object?>(),
                    out var builtinResult))
                {
                    return builtinResult;
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

    private static bool TryInvokeEnumerableMember(
        JsRuntimeInstance runtime,
        object target,
        Type contractType,
        MethodInfo targetMethod,
        object?[]? args,
        out object? result)
    {
        result = null;
        var declaringType = targetMethod.DeclaringType;
        if (declaringType == null)
        {
            return false;
        }

        var isAsync = declaringType.IsGenericType
            && declaringType.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)
            && targetMethod.Name == nameof(IAsyncEnumerable<object>.GetAsyncEnumerator);
        var isGenericSync = declaringType.IsGenericType
            && declaringType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            && targetMethod.Name == nameof(IEnumerable<object>.GetEnumerator);
        var isNonGenericSync = declaringType == typeof(System.Collections.IEnumerable)
            && targetMethod.Name == nameof(System.Collections.IEnumerable.GetEnumerator);
        if (!isAsync && !isGenericSync && !isNonGenericSync)
        {
            return false;
        }

        var enumerableInterface = contractType
            .GetInterfaces()
            .Append(contractType)
            .FirstOrDefault(type =>
                type.IsGenericType
                && type.GetGenericTypeDefinition()
                    == (isAsync ? typeof(IAsyncEnumerable<>) : typeof(IEnumerable<>)));
        if (enumerableInterface == null)
        {
            return false;
        }

        var elementType = enumerableInterface.GetGenericArguments()[0];
        var adapter = runtime.GetOrCreateIterableAdapter(
            target,
            elementType,
            isAsync,
            targetMethod.Name,
            contractType);
        result = targetMethod.Invoke(adapter, args);
        return true;
    }

    private static bool TryGetGeneratedBuiltinProperty(
        JsRuntimeInstance runtime,
        object target,
        Type contractType,
        MethodInfo targetMethod,
        out object? result)
    {
        result = null;
        var kind = GeneratedContractMetadata.GetBuiltinContractKind(contractType);
        var propertyName = targetMethod.Name.Substring(4);
        switch (kind, propertyName)
        {
            case ("Error", "Name"):
            case ("Error", "Message"):
            case ("Error", "Cause"):
            case ("Error", "Stack"):
                var errorPropertyName = char.ToLowerInvariant(propertyName[0]) + propertyName[1..];
                result = JsReturnConverter.ConvertReturn(
                    runtime,
                    JsObjectRuntime.GetProperty(target, errorPropertyName),
                    targetMethod.ReturnType,
                    errorPropertyName,
                    contractType);
                return true;
            case ("Symbol", "Description"):
                result = target is JsSymbol describedSymbol
                    ? describedSymbol.Description
                    : throw new InvalidCastException("The generated Symbol contract target is not a Symbol.");
                return true;
            case ("Symbol", "RegistryKey"):
                result = target is JsSymbol symbol
                    ? JsSymbol.keyFor(symbol)
                    : throw new InvalidCastException("The generated Symbol contract target is not a Symbol.");
                return true;
            case ("Symbol", "WellKnownName"):
                result = GetWellKnownSymbolName(target);
                return true;
            case ("ArrayBuffer", "IsShared"):
                result = target is JsArrayBuffer;
                return true;
            case ("TypedArray", "Kind"):
                result = target is JsTypedArray
                    ? target.GetType().Name
                    : throw new InvalidCastException("The generated typed-array contract target is not a typed array.");
                return true;
            case ("MapEntry", "Key"):
                result = JsReturnConverter.ConvertReturn(
                    runtime,
                    JsObjectRuntime.GetItem(target, 0d),
                    targetMethod.ReturnType,
                    propertyName,
                    contractType);
                return true;
            case ("MapEntry", "Value"):
                result = JsReturnConverter.ConvertReturn(
                    runtime,
                    JsObjectRuntime.GetItem(target, 1d),
                    targetMethod.ReturnType,
                    propertyName,
                    contractType);
                return true;
            default:
                return false;
        }
    }

    private static bool TrySetGeneratedBuiltinProperty(
        JsRuntimeInstance runtime,
        object target,
        Type contractType,
        MethodInfo targetMethod,
        object? value)
    {
        var kind = GeneratedContractMetadata.GetBuiltinContractKind(contractType);
        var propertyName = targetMethod.Name.Substring(4);
        if (kind != "Error"
            || propertyName is not ("Name" or "Message" or "Cause"))
        {
            return false;
        }

        var errorPropertyName = char.ToLowerInvariant(propertyName[0]) + propertyName[1..];
        JsObjectRuntime.SetItem(
            target,
            errorPropertyName,
            runtime.NormalizeHostValue(value));
        return true;
    }

    private static bool TryInvokeGeneratedBuiltinHelper(
        JsRuntimeInstance runtime,
        object target,
        Type contractType,
        MethodInfo targetMethod,
        object?[] args,
        out object? result)
    {
        result = null;
        var kind = GeneratedContractMetadata.GetBuiltinContractKind(contractType);
        if (kind == "SymbolConstructor" && targetMethod.Name == "Create")
        {
            result = JsReturnConverter.ConvertReturn(
                runtime,
                ExportMemberResolver.InvokeJsCallable(
                    runtime,
                    target,
                    args,
                    receiver: null),
                targetMethod.ReturnType,
                targetMethod.Name,
                contractType);
            return true;
        }

        if (kind == "Symbol" && targetMethod.Name == "ToDisplayString")
        {
            result = target is JsSymbol symbol
                ? symbol.ToString()
                : throw new InvalidCastException(
                    "The generated Symbol contract target is not a Symbol.");
            return true;
        }

        if (kind == "TypedArray")
        {
            switch (targetMethod.Name)
            {
                case "Get":
                    RequireArgumentCount(targetMethod, args, 1);
                    var getIndex = GetTypedArrayIndex(target, args[0]);
                    result = JsReturnConverter.ConvertReturn(
                        runtime,
                        JsObjectRuntime.GetItem(target, getIndex),
                        targetMethod.ReturnType,
                        targetMethod.Name,
                        contractType);
                    return true;
                case "Set":
                    RequireArgumentCount(targetMethod, args, 2);
                    var setIndex = GetTypedArrayIndex(target, args[0]);
                    JsObjectRuntime.SetItem(
                        target,
                        setIndex,
                        runtime.NormalizeHostValue(args[1]));
                    return true;
            }
        }

        if (kind == "ArrayBuffer"
            && target is JavaScriptRuntime.ArrayBuffer arrayBuffer)
        {
            if (targetMethod.Name == "Slice")
            {
                var sliceArguments = UnpackParamsArray(targetMethod, args);
                var slice = target is JavaScriptRuntime.SharedArrayBuffer sharedBuffer
                    ? sharedBuffer.slice(
                        sliceArguments.ElementAtOrDefault(0),
                        sliceArguments.ElementAtOrDefault(1))
                    : arrayBuffer.slice(
                        sliceArguments.ElementAtOrDefault(0),
                        sliceArguments.ElementAtOrDefault(1));
                result = JsReturnConverter.ConvertReturn(
                    runtime,
                    slice,
                    targetMethod.ReturnType,
                    "slice",
                    contractType);
                return true;
            }

            if (targetMethod.Name == "Resize")
            {
                _ = arrayBuffer.resize(args.ElementAtOrDefault(0));
                return true;
            }
        }

        return false;
    }

    private static string? GetWellKnownSymbolName(object target)
    {
        if (target is not JsSymbol symbol)
        {
            throw new InvalidCastException(
                "The generated Symbol contract target is not a Symbol.");
        }

        foreach (var name in new[]
                 {
                     "iterator", "asyncIterator", "hasInstance", "isConcatSpreadable",
                     "match", "matchAll", "replace", "search", "species", "split",
                     "toPrimitive", "toStringTag", "unscopables", "dispose", "asyncDispose"
                 })
        {
            if (ReferenceEquals(symbol, JsSymbol.GetWellKnown(name)))
            {
                return name;
            }
        }

        return null;
    }

    private static double GetTypedArrayIndex(object target, object? value)
    {
        if (target is not JsTypedArray typedArray)
        {
            throw new InvalidCastException(
                "The generated typed-array contract target is not a typed array.");
        }

        var index = ToArrayIndex(value);
        if (!double.IsFinite(index)
            || index < 0
            || index != Math.Truncate(index)
            || index >= typedArray.length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Typed-array host indexes must identify an existing element.");
        }

        return index;
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
        Interlocked.Exchange(ref _contractType, null);
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
    private Type? _contractType;
    private int _disposed;

    internal void Initialize(
        JsRuntimeInstance runtime,
        object? constructor,
        Type contractType)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(constructor);
        ArgumentNullException.ThrowIfNull(contractType);

        _runtime = runtime;
        _constructor = constructor;
        _contractType = contractType;
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
        var contractType = _contractType
            ?? throw new ObjectDisposedException(nameof(JsConstructorProxy));

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
        Interlocked.Exchange(ref _contractType, null);
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
