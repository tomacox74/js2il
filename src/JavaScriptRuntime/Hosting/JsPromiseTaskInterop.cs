using System.Threading.Tasks;
using JavaScriptRuntime;

namespace Jroc.Runtime;

internal static class JsPromiseTaskInterop
{
    internal static Task ToTask(
        JsRuntimeInstance runtime,
        Promise promise,
        string? memberName = null,
        Type? contractType = null)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(promise);

        var completion = new RuntimeTaskCompletion<object?>(runtime);
        if (!runtime.TryRegisterRuntimeDependentOperation(completion))
        {
            return completion.Task;
        }

        try
        {
            // Promise.then() queues microtasks via the runtime-thread service provider.
            runtime.Invoke(() =>
            {
                promise.then(
                    onFulfilled: new Func<object[]?, object?, object?>((_, _) =>
                    {
                        completion.TrySetResult(null);
                        return null;
                    }),
                    onRejected: new Func<object[]?, object?, object?>((_, reason) =>
                    {
                        completion.TrySetException(ToException(runtime, reason, memberName, contractType));
                        return null;
                    }));

                return (object?)null;
            });
        }
        catch (Exception exception)
        {
            completion.TrySetException(exception);
        }

        return completion.Task;
    }

    internal static Task<T> ToTask<T>(
        JsRuntimeInstance runtime,
        Promise promise,
        string? memberName = null,
        Type? contractType = null)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(promise);

        var completion = new RuntimeTaskCompletion<T>(runtime);
        if (!runtime.TryRegisterRuntimeDependentOperation(completion))
        {
            return completion.Task;
        }

        try
        {
            runtime.Invoke(() =>
            {
                promise.then(
                    onFulfilled: new Func<object[]?, object?, object?>((_, value) =>
                    {
                        try
                        {
                            var converted = JsReturnConverter.ConvertReturn(runtime, value, typeof(T), memberName, contractType);
                            completion.TrySetResult((T)converted!);
                        }
                        catch (Exception ex)
                        {
                            completion.TrySetException(ex);
                        }

                        return null;
                    }),
                    onRejected: new Func<object[]?, object?, object?>((_, reason) =>
                    {
                        completion.TrySetException(ToException(runtime, reason, memberName, contractType));
                        return null;
                    }));

                return (object?)null;
            });
        }
        catch (Exception exception)
        {
            completion.TrySetException(exception);
        }

        return completion.Task;
    }

    internal static Task<object?> ToRawTask(
        JsRuntimeInstance runtime,
        Promise promise,
        string? memberName = null,
        Type? contractType = null,
        bool duringRuntimeDisposal = false)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(promise);

        var completion = new RuntimeTaskCompletion<object?>(
            runtime,
            unregisterOnCompletion: !duringRuntimeDisposal);
        if (!duringRuntimeDisposal
            && !runtime.TryRegisterRuntimeDependentOperation(completion))
        {
            return completion.Task;
        }

        try
        {
            object? Attach()
            {
                promise.then(
                    onFulfilled: new Func<object[]?, object?, object?>((_, value) =>
                    {
                        completion.TrySetResult(value);
                        return null;
                    }),
                    onRejected: new Func<object[]?, object?, object?>((_, reason) =>
                    {
                        completion.TrySetException(ToException(
                            runtime,
                            reason,
                            memberName,
                            contractType));
                        return null;
                    }));
                return null;
            }

            if (duringRuntimeDisposal)
            {
                runtime.InvokeDuringDisposal(Attach);
            }
            else
            {
                runtime.Invoke(Attach);
            }
        }
        catch (Exception exception)
        {
            completion.TrySetException(exception);
        }

        return completion.Task;
    }

    private static Exception ToException(
        JsRuntimeInstance runtime,
        object? reason,
        string? memberName,
        Type? contractType)
    {
        if (memberName == null && contractType == null)
        {
            return reason is Exception existingException
                ? existingException
                : new JsThrownValueException(reason);
        }

        if (reason is Error jsError)
        {
            return new JsErrorException(
                $"JavaScript {jsError.Name}: {jsError.Message} rejected async operation '{memberName ?? "<promise>"}'.",
                innerException: jsError,
                moduleId: runtime.ModuleId,
                memberName: memberName,
                contractType: contractType,
                compiledAssemblyName: runtime.CompiledAssemblyName,
                jsName: jsError.Name,
                jsMessage: jsError.Message,
                jsStack: jsError.stack);
        }

        if (reason is Exception ex)
        {
            return ex;
        }

        var thrown = new JsThrownValueException(reason);
        return new JsErrorException(
            $"JavaScript threw non-error value '{reason}' while rejecting async operation '{memberName ?? "<promise>"}'.",
            innerException: thrown,
            moduleId: runtime.ModuleId,
            memberName: memberName,
            contractType: contractType,
            compiledAssemblyName: runtime.CompiledAssemblyName,
            thrownValue: reason);
    }

    private sealed class RuntimeTaskCompletion<T> : IRuntimeDependentOperation
    {
        private readonly JsRuntimeInstance _runtime;
        private readonly bool _unregisterOnCompletion;
        private readonly TaskCompletionSource<T> _completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal RuntimeTaskCompletion(
            JsRuntimeInstance runtime,
            bool unregisterOnCompletion = true)
        {
            _runtime = runtime;
            _unregisterOnCompletion = unregisterOnCompletion;
        }

        internal Task<T> Task => _completion.Task;

        internal void TrySetResult(T result)
        {
            if (_completion.TrySetResult(result) && _unregisterOnCompletion)
            {
                _runtime.UnregisterRuntimeDependentOperation(this);
            }
        }

        internal void TrySetException(Exception exception)
        {
            if (_completion.TrySetException(exception) && _unregisterOnCompletion)
            {
                _runtime.UnregisterRuntimeDependentOperation(this);
            }
        }

        public void OnRuntimeDisposed(ObjectDisposedException exception)
            => _completion.TrySetException(exception);
    }
}
