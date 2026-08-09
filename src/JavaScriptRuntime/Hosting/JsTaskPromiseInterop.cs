using System.Reflection;
using JavaScriptRuntime;

namespace Jroc.Runtime;

internal static class JsTaskPromiseInterop
{
    internal static Promise ToPromise(JsRuntimeInstance runtime, Task task)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(task);

        var deferred = Promise.withResolvers();
        var bridge = new TaskPromiseBridge(runtime, deferred);
        if (!runtime.TryRegisterRuntimeDependentOperation(bridge))
        {
            return deferred.promise;
        }

        _ = task.ContinueWith(
            static (completedTask, state) =>
                ((TaskPromiseBridge)state!).CompleteFromTask(completedTask),
            bridge,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);

        return deferred.promise;
    }

    private static object? GetTaskResult(Task task)
    {
        for (var type = task.GetType(); type != null; type = type.BaseType)
        {
            if (!type.IsGenericType
                || type.GetGenericTypeDefinition() != typeof(Task<>))
            {
                continue;
            }

            return type.GetProperty(
                    nameof(Task<object>.Result),
                    BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(task);
        }

        return null;
    }

    private sealed class TaskPromiseBridge : IRuntimeDependentOperation
    {
        private readonly JsRuntimeInstance _runtime;
        private readonly PromiseWithResolvers _deferred;
        private int _state;

        internal TaskPromiseBridge(
            JsRuntimeInstance runtime,
            PromiseWithResolvers deferred)
        {
            _runtime = runtime;
            _deferred = deferred;
        }

        internal void CompleteFromTask(Task task)
        {
            if (Interlocked.CompareExchange(ref _state, 1, 0) != 0)
            {
                return;
            }

            object? result = null;
            Exception? rejection = null;

            if (task.IsCanceled)
            {
                rejection = new TaskCanceledException(task);
            }
            else if (task.IsFaulted)
            {
                var aggregate = task.Exception;
                rejection = aggregate?.InnerExceptions.Count == 1
                    ? aggregate.InnerException
                    : aggregate;
            }
            else
            {
                try
                {
                    result = GetTaskResult(task);
                }
                catch (Exception exception)
                {
                    rejection = exception;
                }
            }

            if (!_runtime.TryPost(() => Settle(result, rejection)))
            {
                CompleteWithoutSettlement();
            }
        }

        public void OnRuntimeDisposed(ObjectDisposedException exception)
            => CompleteWithoutSettlement();

        private void Settle(object? result, Exception? rejection)
        {
            if (Interlocked.CompareExchange(ref _state, 2, 1) != 1)
            {
                return;
            }

            try
            {
                object callback;
                object? value;
                if (rejection == null)
                {
                    try
                    {
                        value = _runtime.NormalizeHostValue(result);
                        callback = _deferred.resolve;
                    }
                    catch (Exception exception)
                    {
                        value = exception;
                        callback = _deferred.reject;
                    }
                }
                else
                {
                    value = rejection;
                    callback = _deferred.reject;
                }

                _ = CallableOperations.Call1(
                    callback,
                    thisArgument: null,
                    value);
            }
            finally
            {
                _runtime.UnregisterRuntimeDependentOperation(this);
            }
        }

        private void CompleteWithoutSettlement()
        {
            var previousState = Interlocked.Exchange(ref _state, 2);
            if (previousState != 2)
            {
                _runtime.UnregisterRuntimeDependentOperation(this);
            }
        }
    }
}
