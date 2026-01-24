using System.Dynamic;

namespace Js2IL.Runtime;

/// <summary>
/// Reflection/dynamic-friendly exports proxy.
/// Member access and invocations are marshalled onto the owning runtime thread.
/// </summary>
internal sealed class JsDynamicExports : DynamicObject, IDisposable
{
    private readonly JsRuntimeInstance _runtime;

    internal JsDynamicExports(JsRuntimeInstance runtime)
    {
        _runtime = runtime;
    }

    public void Dispose() => _runtime.Dispose();

    /// <summary>
    /// Waits for the runtime's dedicated script thread to terminate.
    /// Intended for diagnostics/tests; normal callers should rely on <see cref="Dispose"/>.
    /// </summary>
    internal bool WaitForShutdown(TimeSpan timeout) => _runtime.WaitForShutdown(timeout);

    public object? Get(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        try
        {
            return _runtime.Invoke(() => ExportMemberResolver.GetExportMember(_runtime.Exports, name));
        }
        catch (Exception ex)
        {
            throw JsHostingExceptionTranslator.TranslateProxyCall(ex, _runtime, memberName: name, contractType: null);
        }
    }

    public object? Invoke(string name, params object?[] args)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        try
        {
            return _runtime.Invoke(() =>
            {
                var callable = ExportMemberResolver.GetExportMember(_runtime.Exports, name);
                if (callable is not Delegate d)
                {
                    throw new MissingMethodException($"Export '{name}' is not a callable function.");
                }

                return ExportMemberResolver.InvokeJsDelegate(d, args ?? Array.Empty<object?>());
            });
        }
        catch (Exception ex)
        {
            throw JsHostingExceptionTranslator.TranslateProxyCall(ex, _runtime, memberName: name, contractType: null);
        }
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        try
        {
            result = _runtime.Invoke(() => ExportMemberResolver.GetExportMember(_runtime.Exports, binder.Name));
            return true;
        }
        catch (MissingMemberException)
        {
            result = null;
            return false;
        }
        catch (Exception ex)
        {
            throw JsHostingExceptionTranslator.TranslateProxyCall(ex, _runtime, memberName: binder.Name, contractType: null);
        }
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        try
        {
            result = _runtime.Invoke(() =>
            {
                var callable = ExportMemberResolver.GetExportMember(_runtime.Exports, binder.Name);
                if (callable is not Delegate d)
                {
                    throw new MissingMethodException($"Export '{binder.Name}' is not a callable function.");
                }

                return ExportMemberResolver.InvokeJsDelegate(d, args ?? Array.Empty<object?>());
            });
            return true;
        }
        catch (MissingMemberException)
        {
            result = null;
            return false;
        }
        catch (Exception ex)
        {
            throw JsHostingExceptionTranslator.TranslateProxyCall(ex, _runtime, memberName: binder.Name, contractType: null);
        }
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        throw new NotSupportedException("Exports are read-only via the hosting API.");
    }
}
