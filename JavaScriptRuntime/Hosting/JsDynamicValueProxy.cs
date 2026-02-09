using System.Dynamic;
using System.Runtime.ExceptionServices;

namespace Js2IL.Runtime;

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

        // Wrap everything else (including delegates) so dynamic invocation and member access
        // are marshalled to the runtime thread and use JS semantics.
        return new JsDynamicValueProxy(runtime, value);
    }

    internal object Unwrap() => _target;

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        try
        {
            result = _runtime.Invoke(() => JavaScriptRuntime.Object.GetItem(_target, binder.Name));
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
            var unwrapped = NormalizeArg(value);
            _ = _runtime.Invoke(() => JavaScriptRuntime.Object.SetItem(_target, binder.Name, unwrapped));
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
            var normalizedArgs = NormalizeArgs(args);

            if (_target is not Delegate del)
            {
                result = null;
                return false;
            }

            result = _runtime.Invoke(() => JavaScriptRuntime.Function.Call(del, thisArg: null, normalizedArgs));
            result = Wrap(_runtime, result);
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
            var normalizedArgs = NormalizeArgs(args);
            result = _runtime.Invoke(() => JavaScriptRuntime.Object.CallMember(_target, binder.Name, normalizedArgs));
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

    private static object[] NormalizeArgs(object?[]? args)
    {
        if (args == null || args.Length == 0)
        {
            return Array.Empty<object>();
        }

        var normalized = new object[args.Length];
        for (var i = 0; i < args.Length; i++)
        {
            normalized[i] = NormalizeArg(args[i])!;
        }

        return normalized;
    }

    private static object? NormalizeArg(object? arg)
    {
        if (arg is null)
        {
            return null;
        }

        // If the caller passes values previously returned by the hosting layer, unwrap them
        // back to the underlying JS value so the runtime doesn't see the proxy object.
        if (arg is JsDynamicValueProxy proxy)
        {
            arg = proxy.Unwrap();
        }
        else if (arg is JsDynamicExports exports)
        {
            arg = exports.UnwrapExports();
        }
        else if (arg is JsHandleProxy handleProxy)
        {
            arg = handleProxy.UnwrapTarget();
        }
        else if (arg is JsConstructorProxy ctorProxy)
        {
            arg = ctorProxy.UnwrapConstructor();
        }

        // JS numbers are represented as System.Double throughout the runtime.
        return arg switch
        {
            double => arg,
            float f => (double)f,
            decimal m => (double)m,

            sbyte or byte or short or ushort or int or uint or long or ulong => Convert.ToDouble(arg),

            char c => c.ToString(),
            _ => arg,
        };
    }
}
