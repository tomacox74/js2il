using System.Dynamic;
using System.Runtime.ExceptionServices;
using JavaScriptRuntime;

namespace Jroc.Runtime;

/// <summary>
/// Runtime-owned host projection of a JavaScript callable value.
/// </summary>
public sealed class JsCallable : DynamicObject
{
    private readonly JsRuntimeInstance _runtime;
    private readonly object _target;

    internal JsCallable(JsRuntimeInstance runtime, object target)
    {
        _runtime = runtime;
        _target = target;
    }

    public string Name
        => Convert.ToString(GetProperty("name"), System.Globalization.CultureInfo.InvariantCulture)
            ?? string.Empty;

    public double Length
        => Convert.ToDouble(GetProperty("length"), System.Globalization.CultureInfo.InvariantCulture);

    public bool IsConstructor
        => InvokeWithTranslation(
            "<is-constructor>",
            () => CallableOperations.IsConstructor(_target));

    public object? Call(params object?[] arguments)
        => CallWithReceiver(receiver: null, arguments);

    public object? CallWithReceiver(
        object? receiver,
        params object?[] arguments)
    {
        var result = InvokeWithTranslation(
            "<call>",
            () => CallableOperations.Call(
                _target,
                _runtime.NormalizeHostValue(receiver),
                _runtime.NormalizeHostArguments(arguments)));
        return _runtime.ProjectHostValue(result);
    }

    public Task<object?> CallAsync(params object?[] arguments)
        => CallAsyncWithReceiver(receiver: null, arguments);

    public Task<object?> CallAsyncWithReceiver(
        object? receiver,
        params object?[] arguments)
        => CallAsyncWithReceiver<object?>(receiver, arguments);

    public Task<T> CallAsync<T>(params object?[] arguments)
        => CallAsyncWithReceiver<T>(receiver: null, arguments);

    public Task<T> CallAsyncWithReceiver<T>(
        object? receiver,
        params object?[] arguments)
    {
        var result = InvokeWithTranslation(
            "<call>",
            () => CallableOperations.Call(
                _target,
                _runtime.NormalizeHostValue(receiver),
                _runtime.NormalizeHostArguments(arguments)));

        if (result is Promise promise)
        {
            return JsPromiseTaskInterop.ToTask<T>(_runtime, promise);
        }

        var converted = JsReturnConverter.ConvertReturn(_runtime, result, typeof(T));
        return Task.FromResult((T)converted!);
    }

    public object? Construct(params object?[] arguments)
        => ConstructWithNewTarget(_target, arguments);

    public object? ConstructWithNewTarget(
        object? newTarget,
        params object?[] arguments)
    {
        var result = InvokeWithTranslation(
            "<construct>",
            () =>
            {
                var normalizedNewTarget = _runtime.NormalizeHostValue(newTarget);
                if (!CallableOperations.IsConstructor(normalizedNewTarget))
                {
                    throw new TypeError("newTarget is not a constructor");
                }

                return CallableOperations.Construct(
                    _target,
                    _runtime.NormalizeHostArguments(arguments),
                    normalizedNewTarget);
            });
        return _runtime.ProjectHostValue(result);
    }

    public object? GetProperty(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var result = InvokeWithTranslation(
            name,
            () => ObjectRuntime.GetProperty(_target, name));
        return _runtime.ProjectHostValue(result);
    }

    public void SetProperty(string name, object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _ = InvokeWithTranslation(
            name,
            () => ObjectRuntime.SetItem(
                _target,
                name,
                _runtime.NormalizeHostValue(value)));
    }

    internal object Unwrap(JsRuntimeInstance runtime)
    {
        if (!ReferenceEquals(_runtime, runtime))
        {
            throw new InvalidOperationException(
                "JavaScript callable values cannot cross module runtime instances.");
        }

        return _target;
    }

    public override bool TryGetMember(
        GetMemberBinder binder,
        out object? result)
    {
        result = GetProperty(binder.Name);
        return true;
    }

    public override bool TrySetMember(
        SetMemberBinder binder,
        object? value)
    {
        SetProperty(binder.Name, value);
        return true;
    }

    public override bool TryInvoke(
        InvokeBinder binder,
        object?[]? args,
        out object? result)
    {
        result = Call(args ?? System.Array.Empty<object?>());
        return true;
    }

    public override bool TryInvokeMember(
        InvokeMemberBinder binder,
        object?[]? args,
        out object? result)
    {
        result = InvokeWithTranslation(
            binder.Name,
            () => ObjectRuntime.CallMember(
                _target,
                binder.Name,
                _runtime.NormalizeHostArguments(args)));
        result = _runtime.ProjectHostValue(result);
        return true;
    }

    private T InvokeWithTranslation<T>(
        string memberName,
        Func<T> operation)
    {
        try
        {
            return _runtime.Invoke(operation);
        }
        catch (Exception ex)
        {
            var translated = JsHostingExceptionTranslator.TranslateProxyCall(
                ex,
                _runtime,
                memberName,
                typeof(JsCallable));
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }
}
