using System.Threading.Tasks;
using JavaScriptRuntime;

namespace Jroc.Runtime;

internal static class JsPromiseTaskInterop
{
    internal static Task ToTask(JsRuntimeInstance runtime, Promise promise)
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
                        completion.TrySetException(ToException(reason));
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

    internal static Task<T> ToTask<T>(JsRuntimeInstance runtime, Promise promise)
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
                            var converted = JsReturnConverter.ConvertReturn(runtime, value, typeof(T));
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
                        completion.TrySetException(ToException(reason));
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

    private static Exception ToException(object? reason)
    {
        if (reason is Exception ex)
        {
            return ex;
        }

        return new JsThrownValueException(reason);
    }

    private sealed class RuntimeTaskCompletion<T> : IRuntimeDependentOperation
    {
        private readonly JsRuntimeInstance _runtime;
        private readonly TaskCompletionSource<T> _completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal RuntimeTaskCompletion(JsRuntimeInstance runtime)
        {
            _runtime = runtime;
        }

        internal Task<T> Task => _completion.Task;

        internal void TrySetResult(T result)
        {
            if (_completion.TrySetResult(result))
            {
                _runtime.UnregisterRuntimeDependentOperation(this);
            }
        }

        internal void TrySetException(Exception exception)
        {
            if (_completion.TrySetException(exception))
            {
                _runtime.UnregisterRuntimeDependentOperation(this);
            }
        }

        public void OnRuntimeDisposed(ObjectDisposedException exception)
            => _completion.TrySetException(exception);
    }
}
