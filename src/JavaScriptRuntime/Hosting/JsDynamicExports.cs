using System.Dynamic;
using System.Runtime.ExceptionServices;

namespace Jroc.Runtime;

/// <summary>
/// Reflection/dynamic-friendly exports proxy.
/// Member access and invocations are marshalled onto the owning runtime thread.
/// </summary>
public sealed class JsDynamicExports : DynamicObject, IDisposable
{
    private readonly JsRuntimeInstance _runtime;

    internal JsDynamicExports(JsRuntimeInstance runtime)
    {
        _runtime = runtime;
    }

    internal object UnwrapExports() => _runtime.Exports ?? throw new InvalidOperationException("Runtime exports are not available.");

    internal object UnwrapExports(JsRuntimeInstance runtime)
    {
        if (!ReferenceEquals(_runtime, runtime))
        {
            throw new InvalidOperationException(
                "Module exports cannot cross module runtime instances.");
        }

        return UnwrapExports();
    }

    /// <summary>
    /// Gets the module's complete exports value through the public hosting projection.
    /// This supports modules whose <c>module.exports</c> value is itself callable.
    /// </summary>
    public object? Value
    {
        get
        {
            try
            {
                var value = _runtime.Invoke(() => _runtime.Exports);
                return _runtime.ProjectHostValue(value);
            }
            catch (Exception ex)
            {
                var translated = JsHostingExceptionTranslator.TranslateProxyCall(
                    ex,
                    _runtime,
                    memberName: "<exports>",
                    contractType: typeof(JsDynamicExports));
                ExceptionDispatchInfo.Capture(translated).Throw();
                throw;
            }
        }
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
            var value = _runtime.Invoke(() => ExportMemberResolver.GetExportMember(_runtime.Exports, name));
            return _runtime.ProjectHostValue(value);
        }
        catch (Exception ex)
        {
            var translated = JsHostingExceptionTranslator.TranslateProxyCall(ex, _runtime, memberName: name, contractType: null);
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }

    public object? Invoke(string name, params object?[] args)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        try
        {
            var value = _runtime.Invoke(() =>
            {
                var callable = ExportMemberResolver.GetExportMember(_runtime.Exports, name);
                if (!JavaScriptRuntime.CallableOperations.IsCallable(callable))
                {
                    throw new MissingMethodException($"Export '{name}' is not a callable function.");
                }

                return ExportMemberResolver.InvokeJsCallable(
                    _runtime,
                    callable!,
                    args ?? Array.Empty<object?>());
            });
            return _runtime.ProjectHostValue(value);
        }
        catch (Exception ex)
        {
            var translated = JsHostingExceptionTranslator.TranslateProxyCall(ex, _runtime, memberName: name, contractType: null);
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        try
        {
            result = _runtime.Invoke(() => ExportMemberResolver.GetExportMember(_runtime.Exports, binder.Name));
            result = _runtime.ProjectHostValue(result);
            return true;
        }
        catch (MissingMemberException)
        {
            result = null;
            return false;
        }
        catch (Exception ex)
        {
            var translated = JsHostingExceptionTranslator.TranslateProxyCall(ex, _runtime, memberName: binder.Name, contractType: null);
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        try
        {
            result = _runtime.Invoke(() =>
            {
                var callable = ExportMemberResolver.GetExportMember(_runtime.Exports, binder.Name);
                if (!JavaScriptRuntime.CallableOperations.IsCallable(callable))
                {
                    throw new MissingMethodException($"Export '{binder.Name}' is not a callable function.");
                }

                return ExportMemberResolver.InvokeJsCallable(
                    _runtime,
                    callable!,
                    args ?? Array.Empty<object?>());
            });
            result = _runtime.ProjectHostValue(result);
            return true;
        }
        catch (MissingMemberException)
        {
            result = null;
            return false;
        }
        catch (Exception ex)
        {
            var translated = JsHostingExceptionTranslator.TranslateProxyCall(ex, _runtime, memberName: binder.Name, contractType: null);
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        try
        {
            _runtime.Invoke(() => ExportMemberResolver.SetExportMember(_runtime, _runtime.Exports, binder.Name, value));
            return true;
        }
        catch (Exception ex)
        {
            var translated = JsHostingExceptionTranslator.TranslateProxyCall(ex, _runtime, memberName: binder.Name, contractType: null);
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }

    public override bool TryInvoke(
        InvokeBinder binder,
        object?[]? args,
        out object? result)
    {
        try
        {
            result = _runtime.Invoke(() =>
            {
                var callable = _runtime.Exports;
                if (!JavaScriptRuntime.CallableOperations.IsCallable(callable))
                {
                    throw new MissingMethodException("The module exports value is not callable.");
                }

                return ExportMemberResolver.InvokeJsCallable(
                    _runtime,
                    callable!,
                    args ?? Array.Empty<object?>());
            });
            result = _runtime.ProjectHostValue(result);
            return true;
        }
        catch (Exception ex)
        {
            var translated = JsHostingExceptionTranslator.TranslateProxyCall(
                ex,
                _runtime,
                memberName: "<exports>",
                contractType: typeof(JsDynamicExports));
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }
}
