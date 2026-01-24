using System.Reflection;

namespace Js2IL.Runtime;

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
                var name = targetMethod.Name.Substring(4);
                try
                {
                    return runtime.Invoke(() =>
                    {
                        var value = ExportMemberResolver.GetExportMember(runtime.Exports, name);
                        return JsReturnConverter.ConvertReturn(runtime, value, targetMethod.ReturnType);
                    });
                }
                catch (Exception ex)
                {
                    throw JsHostingExceptionTranslator.TranslateProxyCall(ex, runtime, memberName: name, contractType: targetMethod.DeclaringType);
                }
            }

            if (targetMethod.Name.StartsWith("set_", StringComparison.Ordinal))
            {
                throw new NotSupportedException("Exports are read-only via the hosting API.");
            }
        }

        // Default path: method name maps to an exported callable function.
        var exportName = targetMethod.Name;
        try
        {
            return runtime.Invoke(() =>
            {
                var callable = ExportMemberResolver.GetExportMember(runtime.Exports, exportName);
                if (callable is not Delegate d)
                {
                    throw new MissingMethodException($"Export '{exportName}' is not a callable function.");
                }

                var result = ExportMemberResolver.InvokeJsDelegate(d, args ?? Array.Empty<object?>());
                return JsReturnConverter.ConvertReturn(runtime, result, targetMethod.ReturnType);
            });
        }
        catch (Exception ex)
        {
            throw JsHostingExceptionTranslator.TranslateProxyCall(ex, runtime, memberName: exportName, contractType: targetMethod.DeclaringType);
        }
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
