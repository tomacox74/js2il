using System.Dynamic;
using System.Runtime.ExceptionServices;

namespace Jroc.Runtime;

/// <summary>
/// Dynamic proxy for JS values returned from the runtime.
/// Routes member access and invocations through JavaScriptRuntime.Object so prototype-chain
/// semantics and JS calling conventions are preserved.
/// </summary>
internal sealed class JsDynamicValueProxy : DynamicObject
{
    private readonly JsRuntimeInstance _runtime;
    private readonly object _target;

    internal JsDynamicValueProxy(JsRuntimeInstance runtime, object target)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(target);

        _runtime = runtime;
        _target = target;
    }

    internal static object? Wrap(JsRuntimeInstance runtime, object? value)
    {
        if (value is null)
        {
            return null;
        }

        if (value is JsDynamicValueProxy or JsDynamicExports)
        {
            return value;
        }

        if (JavaScriptRuntime.CallableOperations.IsCallable(value))
        {
            return runtime.GetOrCreateCallableWrapper(value);
        }

        // Avoid wrapping primitives/value-types; callers expect normal CLR behavior here.
        if (value is string
            || value is bool
            || value is double
            || value is float
            || value is decimal
            || value is char
            || value is byte
            || value is sbyte
            || value is short
            || value is ushort
            || value is int
            || value is uint
            || value is long
            || value is ulong)
        {
            return value;
        }

        // Wrap other reference values so dynamic member access is marshalled to the runtime
        // thread and uses JavaScript semantics.
        return new JsDynamicValueProxy(runtime, value);
    }

    internal object Unwrap() => _target;

    internal object Unwrap(JsRuntimeInstance runtime)
    {
        if (!ReferenceEquals(_runtime, runtime))
        {
            throw new InvalidOperationException(
                "JavaScript values cannot cross module runtime instances.");
        }

        return _target;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        try
        {
            result = _runtime.Invoke(() => JavaScriptRuntime.ObjectRuntime.GetItem(_target, binder.Name));
            result = Wrap(_runtime, result);
            return true;
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
            _ = _runtime.Invoke(
                () => JavaScriptRuntime.ObjectRuntime.SetItem(
                    _target,
                    binder.Name,
                    _runtime.NormalizeHostValue(value)));
            return true;
        }
        catch (Exception ex)
        {
            var translated = JsHostingExceptionTranslator.TranslateProxyCall(ex, _runtime, memberName: binder.Name, contractType: null);
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }

    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
    {
        try
        {
            if (!JavaScriptRuntime.CallableOperations.IsCallable(_target))
            {
                result = null;
                return false;
            }

            result = _runtime.Invoke(
                () => JavaScriptRuntime.CallableOperations.Call(
                    _target,
                    thisArgument: null,
                    _runtime.NormalizeHostArguments(args)));
            result = _runtime.ProjectHostValue(result);
            return true;
        }
        catch (Exception ex)
        {
            var translated = JsHostingExceptionTranslator.TranslateProxyCall(ex, _runtime, memberName: "<invoke>", contractType: null);
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        try
        {
            result = _runtime.Invoke(
                () => JavaScriptRuntime.ObjectRuntime.CallMember(
                    _target,
                    binder.Name,
                    _runtime.NormalizeHostArguments(args)));
            result = _runtime.ProjectHostValue(result);
            return true;
        }
        catch (Exception ex)
        {
            var translated = JsHostingExceptionTranslator.TranslateProxyCall(ex, _runtime, memberName: binder.Name, contractType: null);
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }

}
