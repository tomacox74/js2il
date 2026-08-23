using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Jroc.Runtime;

/// <summary>
/// DispatchProxy that projects a module's CommonJS exports object onto a .NET interface.
/// - Interface methods map to callable exports (functions).
/// - Interface properties map to exports members (get_/set_).
/// All work is marshalled to the owning <see cref="JsRuntimeInstance"/> thread.
/// </summary>
internal class JsExportsProxy : DispatchProxy
{
    private JsRuntimeInstance? _runtime;

    internal void Initialize(JsRuntimeInstance runtime)
    {
        // Late-bound initialization used by DispatchProxy.Create<T, TProxy>() patterns.
        _runtime = runtime;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        // DispatchProxy can pass null if something goes wrong with reflection.
        if (targetMethod == null)
        {
            throw new ArgumentNullException(nameof(targetMethod));
        }

        // Treat missing runtime as disposed/uninitialized proxy.
        var runtime = _runtime ?? throw new ObjectDisposedException(nameof(JsExportsProxy));

        // Allow consumers to dispose the runtime by disposing the proxy interface.
        if (targetMethod.DeclaringType == typeof(IDisposable) && targetMethod.Name == nameof(IDisposable.Dispose))
        {
            runtime.Dispose();
            return null;
        }

        // Handle Object virtuals locally (no JS interaction).
        if (targetMethod.DeclaringType == typeof(object))
        {
            return HandleObjectMethod(targetMethod, args);
        }

        // Map interface properties to exports members:
        //   get_Foo -> exports.Foo
        //   set_Foo(v) -> exports.Foo = v
        if (targetMethod.IsSpecialName)
        {
            if (targetMethod.Name.StartsWith("get_", StringComparison.Ordinal))
            {
                var name = GetContractExportName(targetMethod, targetMethod.Name.Substring(4));
                try
                {
                    return runtime.Invoke(() =>
                    {
                        var value = IsExportValueMember(targetMethod)
                            ? runtime.Exports
                            : ExportMemberResolver.GetExportMember(runtime.Exports, name);
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
                var name = GetContractExportName(targetMethod, targetMethod.Name.Substring(4));
                try
                {
                    runtime.Invoke(() => ExportMemberResolver.SetExportMember(runtime, runtime.Exports, name, args is { Length: > 0 } ? args[0] : null));
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

        // Default path: method name maps to an exported callable function.
        var exportName = GetContractExportName(targetMethod, targetMethod.Name);
        try
        {
            return runtime.Invoke(() =>
            {
                var usesWholeExportsValue = IsExportValueMember(targetMethod);
                var callable = usesWholeExportsValue
                    ? runtime.Exports
                    : ExportMemberResolver.GetExportMember(runtime.Exports, exportName);

                if (usesWholeExportsValue
                    && string.Equals(targetMethod.Name, "Construct", StringComparison.Ordinal))
                {
                    var constructed = ExportMemberResolver.Construct(
                        runtime,
                        callable!,
                        UnpackParamsArray(targetMethod, args));
                    return JsReturnConverter.ConvertReturn(
                        runtime,
                        constructed,
                        targetMethod.ReturnType,
                        exportName,
                        targetMethod.DeclaringType);
                }

                if (!JavaScriptRuntime.CallableOperations.IsCallable(callable))
                {
                    throw new MissingMethodException($"Export '{exportName}' is not a callable function.");
                }

                var result = ExportMemberResolver.InvokeJsCallable(
                    runtime,
                    callable!,
                    UnpackParamsArray(targetMethod, args),
                    receiver: usesWholeExportsValue ? null : runtime.Exports);
                return JsReturnConverter.ConvertReturn(
                    runtime,
                    result,
                    targetMethod.ReturnType,
                    exportName,
                    targetMethod.DeclaringType);
            });
        }
        catch (Exception ex)
        {
            var translated = JsHostingExceptionTranslator.TranslateProxyCall(ex, runtime, memberName: exportName, contractType: targetMethod.DeclaringType);
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }

    private static string GetContractExportName(MethodInfo targetMethod, string fallback)
        => GeneratedContractMetadata.GetExportName(targetMethod) ?? fallback;

    private static bool IsExportValueMember(MethodInfo targetMethod)
        => GeneratedContractMetadata.IsExportValue(targetMethod);

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
        // Keep proxy behavior predictable for diagnostics/logging/collections.
        return targetMethod.Name switch
        {
            nameof(ToString) => nameof(JsExportsProxy),
            nameof(GetHashCode) => base.GetHashCode(),
            nameof(Equals) => ReferenceEquals(this, args != null && args.Length > 0 ? args[0] : null),
            _ => null,
        };
    }

}
